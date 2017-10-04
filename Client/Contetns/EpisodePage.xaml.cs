﻿using AniFile3.DataStruct;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AniFile3.Contetns
{
    /// <summary>
    /// Interaction logic for EpisodePage.xaml
    /// </summary>
    public partial class EpisodePage : Page
    {
        // note
        // EpisodeInfo 말고 자체 데이터 포맷이 있어야 할것 같다
        // 실제 파일이 저장되는 위치 정보
        // 현재 다운로드 상태 정보
        // 

        public EpisodePage()
        {
            InitializeComponent();
        }

        private void CleanUp()
        {
            _EpsiodeListView.ItemsSource = null;
        }

        public void LoadEpisode(ObservableCollection<EpisodeInfoClient> episodes)
        {
            CleanUp();
            _EpsiodeListView.ItemsSource = episodes;
        }
    }
}
