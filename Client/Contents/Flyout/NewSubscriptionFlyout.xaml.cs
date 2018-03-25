using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AniFile3.Contents
{
    /// <summary>
    /// Interaction logic for NewSubscriptionFlyout.xaml
    /// </summary>
    public partial class NewSubscriptionFlyout : INotifyPropertyChanged
    {
        public enum CloseEventArg
        {
            Confirm,
            Cancel
        }

        public delegate void CloseEventHander(object sender, CloseEventArg result);
        public event CloseEventHander CloseEvent;

        private CloseEventArg _closeEventArg;
        private CancellationTokenSource _waitCtSource;

        private int _startEpisode;
        public int StartEpisode
        {
            get { return _startEpisode; }
            set
            {
                _startEpisode = value;
                RaisePropertyChanged("StartEpisode");
            }
        }
        
        public NewSubscriptionFlyout()
        {
            InitializeComponent();
            InitializeMethod();
        }

        private void Clear()
        {
            NameField.Clear();
            NameField.IsReadOnly = false;
        }

        private void Close()
        {
            this.IsOpen = false;
            Clear();
            CloseEvent?.Invoke(this, _closeEventArg);
        }
        
        private void Confirm()
        {
            // 구독 리스트에 등록
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.AddSubscription(NameField.Text, StartEpisode);
            Close();
        }

        private async void NameField_TextChanged(object sender, TextChangedEventArgs e)
        {
            _waitCtSource?.Cancel();
            _waitCtSource = new CancellationTokenSource();

            try
            {
                await Task.Delay(500, _waitCtSource.Token);

                string text = (e.Source as TextBox).Text;
                if (string.IsNullOrEmpty(text) == false)
                    ChangedText(text);
            }
            catch (OperationCanceledException)
            { }
        }

        private void CandidateView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0] as SearchResult;
                NameField.Text = item.Title;
            }
        }

        #region 각종 이벤트 메소드들
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            _closeEventArg = CloseEventArg.Confirm;
            Confirm();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _closeEventArg = CloseEventArg.Cancel;
            Close();
        }

        private void NameField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Confirm();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
