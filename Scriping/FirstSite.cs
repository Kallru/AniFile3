using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scriping
{
    public class FirstSite : IScriper
    {
        private WebBrowser _testOwner;
        public FirstSite(WebBrowser web)
        {
            _testOwner = web;
        }

        public void Start()
        {
            string url = "https://torrentkim10.net/";

            var thread = new Thread(() =>
            {
                //var browser = new WebBrowser();
                var browser = _testOwner;
                //browser.Visible = true;
                //browser.ScriptErrorsSuppressed = true;
                browser.DocumentCompleted += WebBrowserDocumentCompletedEventHandler;
                browser.Navigate(url);
                Application.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void WebBrowserDocumentCompletedEventHandler(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var browser = sender as WebBrowser;

            HtmlDocument doc = browser.Document;
           
            HtmlElement searchBox = doc.GetElementById("k");
            searchBox.SetAttribute("value", "비정상");

            var collection = doc.GetElementsByTagName("input");
            foreach (HtmlElement element in collection)
            {
                if(element.GetAttribute("className") == "thumb_up"
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
