using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Windows;

namespace AniFile3.Contents
{
    /// <summary>
    /// Interaction logic for SearchResultPage.xaml
    /// </summary>
    public partial class SearchResultTabItem : MetroTabItem
    {        
        public delegate void SubscriptionClickEvent(object sender);
        public event SubscriptionClickEvent SubsriptionClicked;

        public SearchResultTabItem()
        {
            InitializeComponent();
        }

        private void Subscription_Click(object sender, RoutedEventArgs e)
        {
            if (SubsriptionClicked != null)
                SubsriptionClicked(sender);
        }

        public void UpdateResult(List<SearchResultContent> result)
        {            
            _ResultList.ItemsSource = result;
        }
    }
}
