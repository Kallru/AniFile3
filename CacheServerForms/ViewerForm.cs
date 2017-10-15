using RichGrassHopper.Core.IO;
using Scriping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CacheServerForms
{
    public partial class ViewerForm : Form
    {
        private HttpServer _server;
        private FirstSite _scriper;
        private DataStorage _storage;

        public ViewerForm()
        {
            InitializeComponent();

            Console.SetOut(new LogWriter(logTextBox));

            _storage = new DataStorage();
            _storage.CreateStandard();

            _scriper = new FirstSite(webBrowser1, _storage);
        }

        private void TestViewerForm_Load(object sender, EventArgs e)
        {
            _scriper.InitializeCompleted += () =>
            {
                _scriper.SearchBox("비정상");
            };

            _scriper.Initialize();

            _server = new HttpServer();

            int a = 20;
            a = 50;
        }
        
        //private void Web_BeforeNavigate2(object pDisp, ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel)
        //{
        //    if(PostData == null)
        //    {
        //        string url = URL as string;
        //        if (string.IsNullOrEmpty(url) == false && 
        //            url.Contains("magnet:"))
        //        {
        //            // GET
        //            int a = 20;
        //            a = 50;
        //            Cancel = true;
        //            //testFlag = true;
        //        }   
        //    }
        //}

        private void DebugSaveDocument()
        {
            using (var stream = new StreamWriter("test.txt", false))
            {
                stream.Write(webBrowser1.Document.Body.OuterHtml);
            }
        }
    }
}
