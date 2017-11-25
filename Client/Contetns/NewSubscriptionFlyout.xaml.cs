using System;
using System.Collections.Generic;
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
    /// Interaction logic for NewSubscriptionFlyout.xaml
    /// </summary>
    public partial class NewSubscriptionFlyout
    {
        public NewSubscriptionFlyout()
        {
            InitializeComponent();
        }

        private void Clear()
        {
            _name.Clear();
        }

        private void Close()
        {
            this.IsOpen = false;
            Clear();
        }
        
        private void Confirm()
        {
            // 구독 리스트에 등록
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.AddSubscription(_name.Text);
            Close();
        }

#region 각종 이벤트 메소드들
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Confirm();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _name_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Confirm();
            }
        }
#endregion
    }
}
