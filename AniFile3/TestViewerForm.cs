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
        public struct Program
        {
            public string subject;
            public string magnet;
            public int episode;
        }

        private IScriper _scriper;        

        public TestViewerForm()
        {
            InitializeComponent();
            
            webBrowser1.ScriptErrorsSuppressed = true;
        }

        private async void TestViewerForm_Load(object sender, EventArgs e)
        {
            string outerHtml = await GetPageAsync((document) =>
            {
                string url = "https://torrentkim10.net/";
                webBrowser1.Navigate(url);
            }, 
            (document, contents) => document.GetElementById("k") != null);

            //using (var stream = new StreamWriter("test.txt", false))
            //{
            //    stream.Write(outerHtml);
            //}

            SearchBox("비정상");
        }

        private async Task<string> GetPageAsync(Action<HtmlDocument> requestWork, Func<HtmlDocument, string, bool> finishingCondition = null)
        {
            var compeltionSource = new TaskCompletionSource<string>();

            void OnPageCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
            {
                // 여기서 결과를 파싱
                var browser = sender as WebBrowser;
                if (browser != null)
                {
                    HtmlDocument document = browser.Document;
                    string outerHtml = document.Body.OuterHtml;

                    if (finishingCondition == null ||
                        finishingCondition(document, outerHtml))
                    {
                        compeltionSource.SetResult(outerHtml);
                        webBrowser1.DocumentCompleted -= OnPageCompleted;
                    }
                }

                Console.WriteLine("OnPageCompleted");
            }

            webBrowser1.DocumentCompleted += OnPageCompleted;

            if (requestWork != null)
                requestWork(webBrowser1.Document);

            return await compeltionSource.Task;
        }

        private async void SearchBox(string keyword)
        {
            await GetPageAsync(document =>
            {
                HtmlElement searchBox = document.GetElementById("k");
                if (searchBox != null)
                {
                    searchBox.SetAttribute("value", keyword);

                    var collection = document.GetElementsByTagName("input");
                    foreach (HtmlElement element in collection)
                    {
                        if (element.GetAttribute("className") == "thumb_up"
                            && element.GetAttribute("type") == "submit")
                        {
                            element.InvokeMember("click");
                            break;
                        }
                    }
                }
            });

            // 파싱 도큐먼트
            
        }

        //struct SearchResult
        //{
        //    public string subject;
        //    public string magnet;
        //}

        //private async Task<List<SearchResult>> SearchCompleted()
        //{
        //    var completionSource = new TaskCompletionSource<List<SearchResult>>();

        //    void onCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //    {
        //        // 여기서 결과를 파싱
        //        var parsingResult = new List<SearchResult>();
        //        parsingResult.Add(new SearchResult() { subject = "test", magnet = "httss" });

        //        completionSource.SetResult(parsingResult);
        //        webBrowser1.DocumentCompleted -= onCompleted;
        //    }

        //    webBrowser1.DocumentCompleted += onCompleted;

        //    // need to wait
        //    return await completionSource.Task;
        //}

        //private void WebBrowserDocumentCompletedEventHandler(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    var browser = sender as WebBrowser;

        //    HtmlDocument doc = browser.Document;

        //    HtmlElement searchBox = doc.GetElementById("k");

        //    if (searchBox == null)
        //        return;

        //    searchBox.SetAttribute("value", "비정상");

        //    var collection = doc.GetElementsByTagName("input");
        //    foreach (HtmlElement element in collection)
        //    {
        //        if (element.GetAttribute("className") == "thumb_up"
        //            && element.GetAttribute("type") == "submit")
        //        {
        //            element.InvokeMember("click");
        //            break;
        //        }
        //    }

        //    using (var stream = new StreamWriter("test.txt", false))
        //    {
        //        stream.Write(doc.Body.OuterHtml);
        //    }
        //}
    }
}
