using CoreLib.DataStruct;
using MessagePack;
using System.ComponentModel;

namespace AniFile3.DataStruct
{
    public partial class ClientEpisodeInfo : INotifyPropertyChanged
    {
        public enum StateType
        {
            // 기본값
            None,
            // 다운로드를 시작했다.
            Downloading,
            // 다운로드가 끝났다.(언제든 사용가능)
            DownloadCompleted,
            // 다 봤다(다 사용해서, 데이터는 삭제할수 있다)
            Used,
        }

        private EpisodeInfo _header;
        private int _downloadRate;
        [IgnoreMember]
        private string _downloadPayloadRate;
        [IgnoreMember]
        private string _downloadState;
        [IgnoreMember]
        private string _totalSize;

        [IgnoreMember]
        public bool IsCompleted
        {
            get => State == StateType.DownloadCompleted
                || State == StateType.Used;
        }

        public StateType State { get; set; }
        public string Subject { get => _header.Fullname; }
        public int Episode { get => _header.Episode; }
        public string Resolution { get => _header.Resolution; }
        public string Location { get; private set; }
        
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
