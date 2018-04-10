using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;

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

        private void Flyout_IsOpenChanged(object sender, RoutedEventArgs e)
        {
            // Load from Preference to stage
            if (IsOpen == true)
            {
                var preferenceProperties = Preference.Instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var propertyInfo in preferenceProperties)
                {
                    var tt = propertyInfo.GetValue(Preference.Instance);
                    _stageProperties[propertyInfo.Name] = propertyInfo.GetValue(Preference.Instance);
                }

                MainPanel.DataContext = null;
                MainPanel.DataContext = this;
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
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

            IsOpen = false;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            
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
