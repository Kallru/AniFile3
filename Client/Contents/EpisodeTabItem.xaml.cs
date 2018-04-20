using AniFile3.DataStruct;
using MahApps.Metro.Controls;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace AniFile3.Contents
{
    /// <summary>
    /// Interaction logic for EpisodeTabItem.xaml
    /// </summary>
    public partial class EpisodeTabItem : MetroTabItem
    {
        private class ResolutionDesendingComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var a = x as ClientEpisodeInfo;
                var b = y as ClientEpisodeInfo;
                int first = int.Parse(a.Resolution.Replace("p", ""));
                int second = int.Parse(b.Resolution.Replace("p", ""));
                return second - first;
            }
        }

        // note
        // EpisodeInfo 말고 자체 데이터 포맷이 있어야 할것 같다
        // 실제 파일이 저장되는 위치 정보
        // 현재 다운로드 상태 정보
        // 

        public EpisodeTabItem()
        {
            InitializeComponent();
        }

        private void CleanUp()
        {
            _EpsiodeListView.ItemsSource = null;
        }
        
        public void LoadEpisode(ObservableCollection<ClientEpisodeInfo> episodes)
        {
            CleanUp();

            _EpsiodeListView.ItemsSource = episodes;
            
            // Make Sorting
            var view = (CollectionView)CollectionViewSource.GetDefaultView(_EpsiodeListView.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Episode", ListSortDirection.Descending));
            var listView = (ListCollectionView)view;
            listView.CustomSort = new ResolutionDesendingComparer();

            //var groupDescription = new PropertyGroupDescription("Episode");
            //view.GroupDescriptions.Add(groupDescription);
        }

        private void PlayItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void PauseItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = _EpsiodeListView.SelectedItem as ClientEpisodeInfo;
            item?.Pause(true);
        }

        private void UsedItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = _EpsiodeListView.SelectedItem as ClientEpisodeInfo;
            if (item.State == ClientEpisodeInfo.StateType.Downloading)
            {
                item.DestoryTorrent();
            }

            item.State = ClientEpisodeInfo.StateType.Used;
        }

        private void DeleteItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
