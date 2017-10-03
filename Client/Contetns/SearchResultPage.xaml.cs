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
    /// Interaction logic for SearchResultPage.xaml
    /// </summary>
    public partial class SearchResultPage : Page
    {
        private List<string> _tempResponse;

        public delegate void SubscriptionClickEvent(object sender);
        public event SubscriptionClickEvent SubsriptionClicked;

        public SearchResultPage()
        {
            InitializeComponent();

            _tempResponse = new List<string>();

            _ResultList.ItemsSource = _tempResponse;

            _tempResponse.Add("11111");
            _tempResponse.Add("22222");
            _tempResponse.Add("333333");
            _tempResponse.Add("4444");
            _tempResponse.Add("무한도전");
            _tempResponse.Add("5555");
        }

        private void Subscription_Click(object sender, RoutedEventArgs e)
        {
            if (SubsriptionClicked != null)
                SubsriptionClicked(sender);
        }
    }
}
