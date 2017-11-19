using CoreLib.DataStruct;
using MessagePack;
using System.ComponentModel;

namespace AniFile3.DataStruct
{
    public partial class ClientEpisodeInfo : INotifyPropertyChanged
    {
        private EpisodeInfo _header;
        private int _downloadRate;
        [IgnoreMember]
        private string _downloadPayloadRate;
        [IgnoreMember]
        private string _downloadState;
        [IgnoreMember]
        private string _totalSize;

        public string Subject { get => _header.Name; }
        public int Episode { get => _header.Episode; }
        public string Resolution { get => _header.Resolution; }
        public string Location { get; private set; }
        public bool IsCompleted { get; private set; }
        // 처음 값이 '0' 일때는 아무것도 안보여주기 위해서 string으로 처리
        public string TotalSize
        {
            get => _totalSize;
            set { _totalSize = value; NotifyPropertyChanged("TotalSize"); }
        }

        [IgnoreMember]
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

        [IgnoreMember]
        public string DownloadPayloadRate
        {
            get => _downloadPayloadRate;
            set { _downloadPayloadRate = value; NotifyPropertyChanged("DownloadPayloadRate"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
