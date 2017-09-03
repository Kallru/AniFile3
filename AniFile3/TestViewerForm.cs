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
            webBrowser1.ObjectForScripting = true;
        }

        private async void TestViewerForm_Load(object sender, EventArgs e)
        {
            var result = await GetPageAsync((document) =>
            {
                string url = "https://torrentkim10.net/";
                webBrowser1.Navigate(url);
            },
            (document) => document.GetElementById("k") != null);

            if (result == PageAsyncResult.Timeout)
                MessageBox.Show("Page Loading is failed, Timeout");

            //using (var stream = new StreamWriter("test.txt", false))
            //{
            //    stream.Write(outerHtml);
            //}

            SearchBox("비정상");
        }

        public enum PageAsyncResult
        {
            None,
            Timeout,
        }

        // 기본 5초
        private int _defaultTimeout = 5000;

        private async Task<PageAsyncResult> GetPageAsync(Action<HtmlDocument> requestWork, Func<HtmlDocument, bool> finishingCondition = null)
        {
            var compeltionSource = new TaskCompletionSource<PageAsyncResult>();

            void OnPageCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
            {
                var browser = sender as WebBrowser;
                if (browser != null)
                {
                    HtmlDocument document = browser.Document;

                    // 이번 로딩을 끝낼지 말지 체크한다
                    if (finishingCondition == null ||
                        finishingCondition(document))
                    {
                        compeltionSource.SetResult(PageAsyncResult.None);
                        webBrowser1.DocumentCompleted -= OnPageCompleted;
                    }
                }

                Console.WriteLine("OnPageCompleted");
            }

            webBrowser1.DocumentCompleted += OnPageCompleted;

            if (requestWork != null)
                requestWork(webBrowser1.Document);

            // 타임아웃 처리
            var task = compeltionSource.Task;
            if (await Task.WhenAny(task, Task.Delay(_defaultTimeout)) == task)
            {
                // Done
                return task.Result;
            }
            else
            {
                // timeout
                return PageAsyncResult.Timeout;
            }
        }

        private HtmlElement FindElement(string tag, string className = "", string typeName = "", HtmlElement parents = null)
        {
            // 비교 조건 넣기
            List<Func<HtmlElement, bool>> conditions = new List<Func<HtmlElement, bool>>();
            if (string.IsNullOrEmpty(className) == false)
                conditions.Add((element => element.GetAttribute("className") == className));

            if (string.IsNullOrEmpty(typeName) == false)
                conditions.Add((element => element.GetAttribute("type") == typeName));

            HtmlElementCollection collection;
            if (parents == null)
                collection = webBrowser1.Document.GetElementsByTagName(tag);
            else
                collection = parents.GetElementsByTagName(tag);

            foreach (HtmlElement element in collection)
            {
                if (conditions.TrueForAll((condition) => condition(element)))
                {
                    return element;
                }
            }
            return null;
        }

        private async void SearchBox(string keyword)
        {
            var result = await GetPageAsync(document =>
            {
                HtmlElement searchBox = document.GetElementById("k");
                if (searchBox != null)
                {
                    searchBox.SetAttribute("value", keyword);

                    var submitElement = FindElement("input", "thumb_up", "submit");
                    if (submitElement != null)
                    {
                        submitElement.InvokeMember("click");
                    }

                    //var collection = document.GetElementsByTagName("input");
                    //foreach (HtmlElement element in collection)
                    //{
                    //    if (element.GetAttribute("className") == "thumb_up"
                    //        && element.GetAttribute("type") == "submit")
                    //    {
                    //        element.InvokeMember("click");
                    //        break;
                    //    }
                    //}
                }
            },
            (document) => FindElement("table", "board_list") != null);

            if (result == PageAsyncResult.None)
            {
                // 파싱 도큐먼트
                var table = FindElement("table", "board_list");
                var tbody = table.GetElementsByTagName("tbody");
                var dataTable = tbody[0].GetElementsByTagName("tr");
                foreach (HtmlElement element in dataTable)
                {
                    await ParsingLineInTable(element);
                }
            }
            else
                MessageBox.Show("error");
        }

        private async Task ParsingLineInTable(HtmlElement element)
        {
            var subjectElement = FindElement("td", "subject", parents: element);
            if (subjectElement != null)
            {
                // 링크가 2개 나옴, 카테고리 | 제목
                var links = subjectElement.GetElementsByTagName("a");
                if (links != null && links.Count >= 2)
                {
                    var subject = links[1].InnerText;
                    Console.WriteLine("[Subject]" + subject);

                    var magneticLinkElement = FindElement("td", "num", parents: element);
                    if (magneticLinkElement != null)
                    {
                        Console.WriteLine("[Magnetic]" + magneticLinkElement.OuterHtml);
                        
                        var result = await GetPageAsync((document) =>
                        {
                            var linkElement = magneticLinkElement.GetElementsByTagName("a");
                            var aaa = linkElement[0].InvokeMember("click");
                        },
                        (document) =>
                        {
                            Console.WriteLine("--[After Click Link]--");
                            Console.WriteLine(document.Body.OuterHtml);
                            Console.WriteLine("--[Done]--");
                            return false;
                        });

                        if (result == PageAsyncResult.None)
                        {

                        }
                    }
                }
            }
        }

        private void DebugSaveDocument()
        {
            using (var stream = new StreamWriter("test.txt", false))
            {
                stream.Write(webBrowser1.Document.Body.OuterHtml);
            }
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            MessageBox.Show(e.Url.AbsoluteUri);
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {

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
