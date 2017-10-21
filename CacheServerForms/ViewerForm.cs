using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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

        public ViewerForm()
        {
            InitializeComponent();

            Console.SetOut(new LogWriter(logTextBox));
            
            _scriper = new FirstSite(webBrowser1);
        }

        private void TestViewerForm_Load(object sender, EventArgs e)
        {
            _scriper.InitializeCompleted += async () =>
            {
                var result = await _scriper.SearchBox("비정상");

                var collection = DataBase.Instance.Main.GetCollection<DataStorage.Contents>("content");

                if (result.Count > 0)
                {
                    await collection.InsertManyAsync(result);
                }

                //var collection = _db.GetCollection<DataStorage.Contents>("content");
                //collection.Indexes

                //var result = await collection.Find((itme) => true).FirstOrDefaultAsync();
                //var test = collection.AsQueryable<DataStorage.Contents>().Where(item => true).ToList();

                int a = 20;
                a = 50;
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
