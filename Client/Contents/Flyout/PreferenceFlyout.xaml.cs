using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using MahApps.Metro.Controls.Dialogs;

namespace AniFile3.Contents
{
    /// <summary>
    /// Interaction logic for PreferenceFlyout.xaml
    /// </summary>
    public partial class PreferenceFlyout : INotifyPropertyChanged
    {
        // 중간 저장을 위한 스테이지용
        private Dictionary<string, object> _stageProperties;
        public Dictionary<string, object> Properties
        {
            get => _stageProperties;
            set
            {
                _stageProperties = value;
                NotifyPropertyChanged("Properties");
            }
        }

        public PreferenceFlyout()
        {
            InitializeComponent();
            
            _stageProperties = new Dictionary<string, object>();
            MainPanel.DataContext = this;
        }

        private void LoadFromPreference()
        {
            // Normal Properties
            var preferenceProperties = Preference.Instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in preferenceProperties)
            {
                var tt = propertyInfo.GetValue(Preference.Instance);
                _stageProperties[propertyInfo.Name] = propertyInfo.GetValue(Preference.Instance);
            }

            // for Rss List
            rssView.Items.Clear();
            foreach (var item in Preference.Instance.RSSList)
            {
                rssView.Items.Add(item);
            }

            MainPanel.DataContext = null;
            MainPanel.DataContext = this;
        }

        private void SaveToPreference()
        {
            // Save from Preference to stage
            var preferenceProperties = Preference.Instance.GetType()
                                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                        .ToDictionary(item => item.Name);

            foreach (var pair in _stageProperties)
            {
                var property = preferenceProperties[pair.Key];

                object value = pair.Value;
                if (property.PropertyType != pair.Value.GetType())
                {
                    var converter = TypeDescriptor.GetConverter(property.PropertyType);
                    value = converter.ConvertFrom(pair.Value);
                }
                else
                {
                    property.SetValue(Preference.Instance, value);
                }
            }

            // For Rss
            Preference.Instance.RSSList.Clear();
            foreach (var item in rssView.Items)
            {
                Preference.Instance.RSSList.Add(item as string);
            }
        }

        private void Flyout_IsOpenChanged(object sender, RoutedEventArgs e)
        {
            // Load from Preference to stage
            if (IsOpen == true)
            {
                LoadFromPreference();
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            SaveToPreference();
            IsOpen = false;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        private async void AddToList_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            var result = await mainWindow.ShowInputAsync("RSS 주소", "등록할 RSS 주소를 입력해주세요.");
            if (string.IsNullOrEmpty(result) == false)
            {
                rssView.Items.Add(result);
            }
        }

        private void DeleteInList_Click(object sender, RoutedEventArgs e)
        {
            if (rssView.SelectedItem != null)
                rssView.Items.Remove(rssView.SelectedItem);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }        
    }
}
