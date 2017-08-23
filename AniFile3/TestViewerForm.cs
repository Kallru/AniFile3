using Scriping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniFile3
{
    public partial class TestViewerForm : Form
    {
        private IScriper _scriper;

        public TestViewerForm()
        {
            InitializeComponent();

            string url = "https://torrentkim10.net/";

            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += WebBrowserDocumentCompletedEventHandler;
            webBrowser1.Navigate(url);
        }

        private async void SearchBox(string keyword)
        {
            HtmlDocument doc = webBrowser1.Document;

            HtmlElement searchBox = doc.GetElementById("k");
            if (searchBox != null)
            {
                searchBox.SetAttribute("value", keyword);

                var collection = doc.GetElementsByTagName("input");
                foreach (HtmlElement element in collection)
                {
                    if (element.GetAttribute("className") == "thumb_up"
                        && element.GetAttribute("type") == "submit")
                    {
                        element.InvokeMember("click");
                       
                        await SearchCompleted();

                        // get list
                    }
                }
            }
        }

        struct SearchResult
        {
            public string subject;
            public string magnet;
        }

        private async Task<List<SearchResult>> SearchCompleted()
        {
            var result = new List<SearchResult>();

            Task.Factory.StartNew(()=>
            {
                void onCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
                {
                    // 여기서 결과를 파싱

                    result.Add(new SearchResult() { subject = "test" });

                    webBrowser1.DocumentCompleted -= onCompleted;
                }

                webBrowser1.DocumentCompleted += onCompleted;
            });

            // need to wait

            return result;
        }

        private void WebBrowserDocumentCompletedEventHandler(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var browser = sender as WebBrowser;

            HtmlDocument doc = browser.Document;

            HtmlElement searchBox = doc.GetElementById("k");

            if (searchBox == null)
                return;

            searchBox.SetAttribute("value", "비정상");

            var collection = doc.GetElementsByTagName("input");
            foreach (HtmlElement element in collection)
            {
                if (element.GetAttribute("className") == "thumb_up"
                    && element.GetAttribute("type") == "submit")
                {
                    element.InvokeMember("click");
                    break;
                }
            }

            using (var stream = new StreamWriter("test.txt", false))
            {
                stream.Write(doc.Body.OuterHtml);
            }
        }
    }
}
