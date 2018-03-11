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
        public SearchResultTabItem()
        {
            InitializeComponent();
        }

        public void UpdateResult(List<SearchResultContent> result)
        {
            ResultList.ItemsSource = result;
        }
        
        private void ResultList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedItem = e.AddedItems[0] as SearchResultContent;

                var mainWindow = Window.GetWindow(this) as MainWindow;
                var newSubscriptionFlyout = mainWindow.NewSubscriptionFlyout;
                newSubscriptionFlyout.NameField.Text = selectedItem.Name;
                newSubscriptionFlyout.NameField.IsReadOnly = true;
                newSubscriptionFlyout.IsOpen = true;

                newSubscriptionFlyout.CloseEvent += Subscripted;
            }
        }

        private void Subscripted(object sender, NewSubscriptionFlyout.CloseEventArg e)
        {
            if (e == NewSubscriptionFlyout.CloseEventArg.Confirm)
            {
                var list = ResultList.ItemsSource as List<SearchResultContent>;
                list.Remove(ResultList.SelectedItem as SearchResultContent);
                ResultList.Items.Refresh();
            }
            else
                ResultList.SelectedItem = null;

            (sender as NewSubscriptionFlyout).CloseEvent -= Subscripted;
        }
    }
}
