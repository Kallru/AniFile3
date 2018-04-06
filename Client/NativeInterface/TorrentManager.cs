using AniFile3.Native;
using CoreLib.DataStruct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AniFile3
{
    public class TorrentManager
    {
        public static TorrentManager Instance { get; private set; } = new TorrentManager();

        private struct DownloadRequest
        {
            public EpisodeInfo _header;
            public string _childPathName;
            public Action<StateInfo> _updateEvent;
            public Action<long> _startEvent;
            public Action<long, StateInfo> _finishEvent;
        }

        private SortedSet<DownloadRequest> _queue;
        private HashSet<long> _useds;
        private ScheduleTask _schedule;

        private TorrentManager()
        {
            _useds = new HashSet<long>();
            _queue = new SortedSet<DownloadRequest>(Comparer<DownloadRequest>.Create((x, y) => EpisodeInfo.Compare(x._header, y._header)));

            _schedule = new ScheduleTask();
            _schedule.Start(1000, Update);
            
            var path = Preference.GetAbsoluteDownloadPath();
            if (Directory.Exists(path) == false)
            {
                var check = Directory.CreateDirectory(path);
            }
        }

        public static void Download(EpisodeInfo header,
                                    string childPathName,
                                    Action<StateInfo> stateUpdatedCallback,
                                    Action<long> startCallback = null,
                                    Action<long, StateInfo> finishCallback = null)
        {
            Instance._queue.Add(new DownloadRequest()
            {
                _header = header,
                _childPathName = childPathName,
                _updateEvent = stateUpdatedCallback,
                _startEvent = startCallback,
                _finishEvent = finishCallback
            });
        }

        public static void Initialize()
        {
            NativeInterface.Initialize();
        }

        public static void Dispose()
        {
            Instance.Clear();
            NativeInterface.Uninitialize();
        }

        private void Clear()
        {
            _schedule.Dispose();
            _queue.Clear();
            foreach (var id in _useds)
            {
                NativeInterface.DestroyTorrent(id);
            }
            _useds.Clear();
        }

        private void Update()
        {
            if (_queue.Count > 0
                && _useds.Count < Preference.Instance.CurrentlyTorrentCount)
            {
                var info = _queue.First();
                _queue.Remove(info);

                var id = NativeInterface.CreateTorrent();

                // for child directory
                var savePath = Path.Combine(Preference.GetAbsoluteDownloadPath(), info._childPathName);
                if (Directory.Exists(savePath) == false)
                    Directory.CreateDirectory(savePath);

                var result = NativeInterface.StartDownload(id, info._header.Magnet, savePath, (stateInfo) =>
                 {
                     info._updateEvent(stateInfo);

                     state_t state = (state_t)stateInfo.State;

                     if (state == state_t.finished
                        || state == state_t.seeding)
                     {
                         info._finishEvent?.Invoke(id, stateInfo);
                         _useds.Remove(id);
                         NativeInterface.DestroyTorrent(id);
                     }
                 });

                if (result)
                {
                    info._startEvent?.Invoke(id);
                    _useds.Add(id);
                }
                else
                {
                    // 뭔진 모르겠지만 다운로드를 시작하지 못했다.
                    // 토렌트 id는 자동 파괴됨
                    // ERROR
                    Console.WriteLine("[Error] Torrent download is failed\nFile:{0}, Episode:{1}", info._header.Name, info._header.Episode);
                }
            }
        }
    }
}
