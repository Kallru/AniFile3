using CoreLib.MessagePackets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AniFile3.DataStruct
{
    public partial class EpisodeInfoClient : INotifyPropertyChanged
    {
        private EpisodeInfo _header;
        
        public EpisodeInfoClient(EpisodeInfo header)
        {
            _header = header;
        }

        public void Start()
        {
            NativeInterface.Download(_header.Magnet, ".", UpdateState);
        }

        private void UpdateState(StateInfo stateInfo)
        {
            DownloadState = stateInfo.StateText;
            DownloadRate = stateInfo.Progress;

            if (stateInfo.DownloadPayloadRate > 1024)
                DownloadPayloadRate = string.Format("{0:F1} MB/s", stateInfo.DownloadPayloadRate / 1024.0f);
            else
                DownloadPayloadRate = string.Format("{0:F1} kB/s", stateInfo.DownloadPayloadRate);
        }
    }
}
