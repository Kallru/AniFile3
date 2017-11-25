using MahApps.Metro.Controls;
using System.Windows;

namespace AniFile3.Contetns
{
    /// <summary>
    /// HomeTabItem.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HomeTabItem : MetroTabItem
    {
        public HomeTabItem()
        {
            InitializeComponent();
        }

        private void NewSubscription_Click(object sender, RoutedEventArgs e)
        {
            // 표시 이름\
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.NewSubscriptionFlyout.IsOpen = true;
        }
    }
}
