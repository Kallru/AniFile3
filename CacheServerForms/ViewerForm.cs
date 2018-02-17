using CacheServerSystem;
using RichGrassHopper.Core.IO;
using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace CacheServerForms
{
    public partial class ViewerForm : Form
    {
        private ServerEngine _server;

        public ViewerForm()
        {
            InitializeComponent();

            Console.SetOut(new LogWriter(logTextBox));
        }

        static string Request(string uri)
        {
            WebRequest request = WebRequest.Create(uri);
            request.Credentials = CredentialCache.DefaultCredentials;

            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return responseFromServer;
        }

        private void TestViewerForm_Load(object sender, EventArgs e)
        {
            _server = new ServerEngine();
            _server.Start();            
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
            if (_server != null)
            {
                _server.Dispose();
                _server = null;
            }
        }
    }
}
