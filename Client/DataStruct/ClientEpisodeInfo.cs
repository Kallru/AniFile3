using AniFile3.Native;
using CoreLib.DataStruct;
using System.Collections.Generic;
using System.ComponentModel;

namespace AniFile3.DataStruct
{
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
                TorrentManager.Download(_header, UpdateState, (id) => _torrentId = id, (id, stateInfo) =>
                {
                    IsCompleted = true;
                    TotalSize = GetTotalSizeFormat(stateInfo);
                    DownloadPayloadRate = string.Empty;
                });
            }
        }

        public bool Compare(ClientEpisodeInfo left)
        {
            return Subject == left.Subject
                && Episode == left.Episode
                && Resolution == left.Resolution;
        }

        private string GetTotalSizeFormat(StateInfo stateInfo)
        {
            string[] unit = { "KB", "MB", "GB" };
            int unitIndex = 0;
            float wanted = stateInfo.TotalWanted;
            float done = stateInfo.TotalDone;
            while (wanted > 1024)
            {
                wanted /= 1024.0f;
                done /= 1024.0f;
                ++unitIndex;
            }

            unitIndex = System.Math.Min(unitIndex, unit.Length);

            state_t state = (state_t)stateInfo.State;

            if (state == state_t.finished
               || state == state_t.seeding)
            {
                return string.Format("{0:F2} {1}", wanted, unit[unitIndex]);
            }
            else
            {
                return string.Format("{0:F2}/{1:F2} {2}", done, wanted, unit[unitIndex]);
            }
        }

        private void UpdateState(StateInfo stateInfo)
        {
            DownloadState = stateInfo.StateText;
            DownloadRate = stateInfo.Progress;
            TotalSize = GetTotalSizeFormat(stateInfo);

            if (stateInfo.DownloadPayloadRate > 1024)
                DownloadPayloadRate = string.Format("{0:F1} MB/s", stateInfo.DownloadPayloadRate / 1024.0f);
            else
                DownloadPayloadRate = string.Format("{0:F1} kB/s", stateInfo.DownloadPayloadRate);
        }
    }
}
