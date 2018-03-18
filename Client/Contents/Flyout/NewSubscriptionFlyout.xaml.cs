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

namespace AniFile3.Contents
{
    /// <summary>
    /// Interaction logic for NewSubscriptionFlyout.xaml
    /// </summary>
    public partial class NewSubscriptionFlyout
    {
        public enum CloseEventArg
        {
            Confirm,
            Cancel
        }

        public delegate void CloseEventHander(object sender, CloseEventArg result);
        public event CloseEventHander CloseEvent;

        private CloseEventArg _closeEventArg;

        public NewSubscriptionFlyout()
        {
            InitializeComponent();

            _candidate = new ObservableCollection<TestSearchFromTMDB>();
            _candidate.Add(new TestSearchFromTMDB());
            _candidate.Add(new TestSearchFromTMDB());
            _candidate.Add(new TestSearchFromTMDB());

            NameComboBox.ItemsSource = Candidate;
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
            mainWindow.AddSubscription(NameField.Text);
            Close();
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
        #endregion
        
        private void NameComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //await Task.Delay(1000);
            //NameComboBox.IsDropDownOpen = true;
        }
    }
}
