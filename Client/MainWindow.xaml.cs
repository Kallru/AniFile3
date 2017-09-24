using AniFile3.DataStruct;
using CoreLib.MessagePackets;
using MahApps.Metro.Controls;
using RichGrassHopper.Core.IO;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AniFile3
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Subscriptions _subscriptions;
        private List<string> _tempResponse;

        public MainWindow()
        {
            InitializeComponent();

            _tempResponse = new List<string>();

            Console.SetOut(new LogWriter(_testLog));

            _subscriptions = new Subscriptions();

            _MainTreeView.ItemsSource = _subscriptions;
            _TempResultList.ItemsSource = _tempResponse;

            //---- Test Data
            var node = new Subscriptions.Node()
            {
                Subject = "구독중"
            };
            node.Children.Add(new Subscriptions.Node()
            {
                Subject = "무한도전"
            });

            node.Count = node.Children.Count;

            _subscriptions.Add(node);

            _tempResponse.Add("11111");
            _tempResponse.Add("22222");
            _tempResponse.Add("333333");
            _tempResponse.Add("4444");
            _tempResponse.Add("무한도전");
            _tempResponse.Add("5555");

            // TestCode - Serialize and Deserialize
            var aaa = MessagePack.Serialize(_tempResponse);
            var bbb = MessagePack.Deserialize<List<string>>(aaa);
        }
        
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            //_SerachText.Text
        }

        private void Subscription_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            string subject = button.DataContext as string;
            
            var result = _subscriptions.FirstOrDefault((element) => element.Subject == subject);
            if(result == null)
            {
                var node = new Subscriptions.Node()
                {
                    Subject = subject
                };
                _subscriptions.Add(node);
            }
            else
            {
                Console.WriteLine("이미 같은 것을 구독중입니다");
            }
        }
    }
}
