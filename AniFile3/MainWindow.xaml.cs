using Scriping;
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
    public partial class MainWindow : Window
    {
        IScriper _test_scriper;

        public MainWindow()
        {
            InitializeComponent();

            Console.SetOut(new LogWriter(_testLog));

            var form = new TestViewerForm();
            //var interoper = new WindowInteropHelper(this);
            //interoper.Owner = form.Handle;
            form.ShowDialog();
        }

        //private void _testWebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        //{
        //    dynamic doc = _testWebBrowser.Document;

        //    dynamic searchBox = doc.GetElementById("k");
        //    searchBox.SetAttribute("value", "비정상");

        //    var collection = doc.GetElementsByTagName("input");
        //    foreach (dynamic element in collection)
        //    {
        //        if (element.GetAttribute("className") == "thumb_up"
        //            && element.GetAttribute("type") == "submit")
        //        {
        //            element.InvokeMember("click");
        //            break;
        //        }
        //    }
        //}
    }
}
