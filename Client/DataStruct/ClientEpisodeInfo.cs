using CoreLib.DataStruct;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AniFile3.DataStruct
{
    public static class ClientEpisodeInfoExtentions
    {
        public static bool Contains(this ObservableCollection<ClientEpisodeInfo> _this, EpisodeInfo info)
        {
            foreach (var item in _this)
            {
                if (item.Subject == info.Name
                    && item.Episode == info.Episode
                    && item.Resolution == info.Resolution)
                {
                    return true;
                }
            }

            return false;
        }

        public static void AddRange(this ObservableCollection<ClientEpisodeInfo> _this, IEnumerable<EpisodeInfo> infos)
        {
            foreach (var episode in infos)
            {
                // 중복 체크해서 클라에 데이터 넣기.
                if (_this.Contains(episode) == false)
                {
                    _this.Add(new ClientEpisodeInfo(episode));
                }
            }
        }
    }

    public partial class ClientEpisodeInfo : INotifyPropertyChanged
    {
        [MessagePack.IgnoreMember]
        private long _torrentId;
        
        public class Comparer : IEqualityComparer<ClientEpisodeInfo>
        {
            public bool Equals(ClientEpisodeInfo left, ClientEpisodeInfo right)
            {
                if (right == null && left == null)
                    return true;
                else if (left == null | right == null)
                    return false;
                else if (left.Subject == right.Subject
                    && left.Episode == right.Episode
                    && left.Resolution == right.Resolution)
                    return true;
                else
                    return false;
            }

            public int GetHashCode(ClientEpisodeInfo info)
            {
                return info.GetHashCode();
            }
        }

        public ClientEpisodeInfo(EpisodeInfo header)
        {
            _header = header;
        }

        // for serialize
        public ClientEpisodeInfo()
        { }

        public void Start()
        {
            if (IsCompleted == false)
            {
                _torrentId = NativeInterface.Download(_header.Magnet, Preference.Instance.RootDownloadPath, UpdateState);
            }
        }

        public bool Compare(ClientEpisodeInfo left)
        {
            return Subject == left.Subject
                && Episode == left.Episode
                && Resolution == left.Resolution;
        }

        private void UpdateState(StateInfo stateInfo)
        {
            DownloadState = stateInfo.StateText;
            DownloadRate = stateInfo.Progress;
            IsCompleted = stateInfo.State == (int)state_t.finished;

            if (stateInfo.DownloadPayloadRate > 1024)
                DownloadPayloadRate = string.Format("{0:F1} MB/s", stateInfo.DownloadPayloadRate / 1024.0f);
            else
                DownloadPayloadRate = string.Format("{0:F1} kB/s", stateInfo.DownloadPayloadRate);
        }
    }
}
