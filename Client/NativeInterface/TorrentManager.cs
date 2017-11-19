using CoreLib.DataStruct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniFile3
{
    public class TorrentManager
    {
        public static TorrentManager Instance { get; private set; } = new TorrentManager();

        private struct TaskInfo
        {
            public EpisodeInfo _header;
            public Action<StateInfo> _updateEvent;
            public Action<long> _startEvent;
            public Action<long> _finishEvent;            
        }
        
        private SortedSet<TaskInfo> _queue;
        private HashSet<long> _useds;
        private ScheduleTask _schedule;

        private TorrentManager()
        {
            _queue = new SortedSet<TaskInfo>(Comparer<TaskInfo>.Create((x, y) => x._header.Episode - y._header.Episode));
            _schedule = new ScheduleTask();
            _schedule.Start(1000, Update);
        }

        public void Download(EpisodeInfo header, Action<StateInfo> stateUpdatedCallback)
        {
            _queue.Add(new TaskInfo()
            {
                _header = header,
                _updateEvent = stateUpdatedCallback
            });
        }

        private void Update()
        {
            if (_useds.Count < 3
                && _queue.Count > 0)
            {
                var info = _queue.First();
                _queue.Remove(info);

                var id = NativeInterface.CreateTorrent();

                var result = NativeInterface.StartDownload(id, info._header.Magnet, Preference.Instance.RootDownloadPath, (stateInfo) =>
                 {
                     info._updateEvent(stateInfo);

                     if (stateInfo.State == (int)state_t.finished)
                     {
                         info._finishEvent(id);
                         _useds.Remove(id);
                     }
                 });

                if (result)
                {
                    info._startEvent(id);
                    _useds.Add(id);
                }
                else
                {
                    // 뭔진 모르겠지만 다운로드를 시작하지 못했다.
                    // 토렌트 id는 자동 파괴됨
                    // ERROR
                }
            }
        }
    }
}
