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
    public class FirstSite : ScriperBase
    {        
        private DataStorage _storage;
       
        public delegate void InitializedEvent();
        public event InitializedEvent InitializeCompleted;
        
        public FirstSite(WebBrowser web, DataStorage storage)
            : base(web)
        {
            _storage = storage;

            _browser.ScriptErrorsSuppressed = true;
            _browser.ObjectForScripting = true;
        }

        public async void Initialize()
        {
            var result = await GetPageAsync((document) =>
            {
                string url = "https://torrentkim10.net/";
                _browser.Navigate(url);
            },
            (document) => document.GetElementById("k") != null);

            if (result == PageAsyncResult.None)
            {
                if (InitializeCompleted != null)
                    InitializeCompleted();
            }
            else
                MessageBox.Show("Page Loading is failed, Timeout");

            //string url = "https://torrentkim10.net/";

            //var thread = new Thread(() =>
            //{
            //    //var browser = new WebBrowser();
            //    var browser = _browser;
            //    //browser.Visible = true;
            //    //browser.ScriptErrorsSuppressed = true;
            //    browser.DocumentCompleted += WebBrowserDocumentCompletedEventHandler;
            //    browser.Navigate(url);
            //    Application.Run();
            //});

            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();
        }

        public async void SearchBox(string keyword)
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

                // TestCode
                Console.WriteLine("------Stores------");
                //foreach (var item in _storage)
                //{
                //    Console.WriteLine("Subject - " + item.subject);
                //    Console.WriteLine("Magnet - " + item.magnet);
                //    Console.WriteLine();
                //}
                Console.WriteLine("------End------");
                MessageBox.Show("Done");
                //--------
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
                        var newContents = new DataStorage.Contents();
                        newContents.Subject = subject;

                        var result = await GetPageAsync((document) =>
                        {
                            // 네비게이팅 설정
                            void Navigating(object sender, WebBrowserNavigatingEventArgs e)
                            {
                                // 주소에 마그넷이 있는지 확인
                                if (e.Url.OriginalString.Contains("magnet:"))
                                {
                                    // 마그넷 정보를 담고, 클린 처리
                                    newContents.Magnet = e.Url.OriginalString;
                                    _browser.Navigating -= Navigating;
                                    e.Cancel = true;
                                }
                            }
                            _browser.Navigating += Navigating;

                            // 마그넷 링크를 클릭한다.
                            var linkElement = magneticLinkElement.GetElementsByTagName("a");
                            linkElement[0].InvokeMember("click");

                        },
                        (document) => string.IsNullOrEmpty(newContents.Magnet) == false);

                        if (result == PageAsyncResult.None)
                        {
                            _storage.Add(newContents);
                        }
                        else
                        {
                            MessageBox.Show("Time Out?");
                        }
                    }
                }
            }
        }
    }
}
