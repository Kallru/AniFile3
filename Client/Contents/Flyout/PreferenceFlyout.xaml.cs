using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AniFile3.Contents
{
    /// <summary>
    /// Interaction logic for PreferenceFlyout.xaml
    /// </summary>
    public partial class PreferenceFlyout
    {
        public PreferenceFlyout()
        {
            InitializeComponent();
            MainPanel.DataContext = Preference.Instance;
        }
    }
}
