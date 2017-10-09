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

            DownloadState = "테스트중";
            DownloadRate = 65;
        }

        public async void Test()
        {
            DownloadRate = 0;

            await Task.Run(() =>
            {
                while(DownloadRate < 100)
                {
                    Thread.Sleep(1000);
                    ++DownloadRate;
                }
            });
        }

        public void Start()
        {
            NativeInterface.Download(_header.Magnet, ".", UpdateState);
        }

        private void UpdateState(StateInfo stateInfo)
        {
            DownloadState = stateInfo.StateText;
            DownloadRate = stateInfo.Progress;
        }
    }
}
