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
        private TaskCompletionSource<string> _compeltionSource;

        public TestViewerForm()
        {
            InitializeComponent();

            _compeltionSource = new TaskCompletionSource<string>();

            string url = "https://torrentkim10.net/";

            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += FirstOnPageCompleted;
            webBrowser1.Navigate(url);

            //var task = GetPageAsync(null);

            //task.Start();

            //string some = task.Result;

            //using (var stream = new StreamWriter("test.txt", false))
            //{
            //    stream.Write(task.Result);
            //}
        }

        async void FirstOnPageCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.DocumentCompleted -= FirstOnPageCompleted;

            webBrowser1.DocumentCompleted += OnPageCompleted;

            string result = await GetPageAsync(null);

            int a = 20;
        }

        void OnPageCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // 여기서 결과를 파싱
            var browser = sender as WebBrowser;
            if (browser != null)
            {
                HtmlDocument document = browser.Document;
                _compeltionSource.SetResult(document.Body.OuterHtml);
            }
        }

        private async Task<string> GetPageAsync(Action<HtmlDocument> exWorking)
        {
            if(_compeltionSource.Task.Status == TaskStatus.Running)
                _compeltionSource.SetCanceled();

            if(exWorking != null)
                exWorking(webBrowser1.Document);

            // need to wait
            return await _compeltionSource.Task;
        }

        private void TestParsing(HtmlDocument document)
        {

        }

        //private async void SearchBox(string keyword)
        //{
        //    HtmlDocument doc = webBrowser1.Document;

        //    HtmlElement searchBox = doc.GetElementById("k");
        //    if (searchBox != null)
        //    {
        //        searchBox.SetAttribute("value", keyword);

        //        var collection = doc.GetElementsByTagName("input");
        //        foreach (HtmlElement element in collection)
        //        {
        //            if (element.GetAttribute("className") == "thumb_up"
        //                && element.GetAttribute("type") == "submit")
        //            {
        //                element.InvokeMember("click");
                       
        //                var result = await SearchCompleted();

        //                // get list
        //            }
        //        }
        //    }
        //}

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
