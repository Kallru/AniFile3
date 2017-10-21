using Nancy.Hosting.Self;
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
        private NancyHost _host;
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

            var configuration = new HostConfiguration()
            {
                UrlReservations = new UrlReservations()
                {
                    CreateAutomatically = true,
                }
            };

            _host = new NancyHost(configuration, new Uri("http://localhost:2323"));            
            _host.Start();
        }

        private void DebugSaveDocument()
        {
            using (var stream = new StreamWriter("test.txt", false))
            {
                stream.Write(webBrowser1.Document.Body.OuterHtml);
            }
        }

        private void ViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }
        }
    }
}
