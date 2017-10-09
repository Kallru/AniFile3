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
        private string _downloadState;
        private int _downloadRate;

        public string Subject { get => _header.Subject; }
        public int Episode { get => _header.Episode; }
        public string Resolution { get => _header.Resolution; }
        public string Location { get; private set; }
        
        public string DownloadState
        {
            get => _downloadState;
            set { _downloadState = value; NotifyPropertyChanged("DownloadState"); }
        }

        // 0 to 100        
        public int DownloadRate
        {
            get => _downloadRate;
            set { _downloadRate = value; NotifyPropertyChanged("DownloadRate"); }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
