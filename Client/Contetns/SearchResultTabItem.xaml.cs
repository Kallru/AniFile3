using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Windows;

namespace AniFile3.Contetns
{
    /// <summary>
    /// Interaction logic for SearchResultPage.xaml
    /// </summary>
    public partial class SearchResultTabItem : MetroTabItem
    {
        private List<string> _tempResponse;

        public delegate void SubscriptionClickEvent(object sender);
        public event SubscriptionClickEvent SubsriptionClicked;

        public SearchResultTabItem()
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
