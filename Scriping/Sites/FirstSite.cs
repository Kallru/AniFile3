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
        public delegate void InitializedEvent();
        public event InitializedEvent InitializeCompleted;
        
        public FirstSite(WebBrowser web)
            : base(web)
        {
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

        public async Task<List<DataStorage.Contents>> SearchBox(string keyword)
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

            var datas = new List<DataStorage.Contents>();
            if (result == PageAsyncResult.None)
            {
                // 파싱 도큐먼트
                var table = FindElement("table", "board_list");
                var tbody = table.GetElementsByTagName("tbody");
                var dataTable = tbody[0].GetElementsByTagName("tr");
                foreach (HtmlElement element in dataTable)
                {
                    var data = await ParsingLineInTable(element);
                    if (data != null)
                    {
                        datas.Add(data);
                    }
                    else
                    {
                        Console.WriteLine("[Error] 'ParsingLineInTable' returning value is null");
                    }
                }

                Console.WriteLine("[Done] Total adding data's count is {0}", datas.Count);
            }
            else
            {
                Console.WriteLine("[Error]");
            }

            return datas;
        }

        private async Task<DataStorage.Contents> ParsingLineInTable(HtmlElement element)
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
                        string magnet = string.Empty;

                        var result = await GetPageAsync((document) =>
                        {
                            // 네비게이팅 설정
                            void Navigating(object sender, WebBrowserNavigatingEventArgs e)
                            {
                                // 주소에 마그넷이 있는지 확인
                                if (e.Url.OriginalString.Contains("magnet:"))
                                {
                                    // 마그넷 정보를 담고, 클린 처리
                                    magnet = e.Url.OriginalString;
                                    _browser.Navigating -= Navigating;
                                    e.Cancel = true;
                                }
                            }
                            _browser.Navigating += Navigating;

                            // 마그넷 링크를 클릭한다.
                            var linkElement = magneticLinkElement.GetElementsByTagName("a");
                            linkElement[0].InvokeMember("click");

                        },
                        (document) => string.IsNullOrEmpty(magnet) == false);

                        if (result == PageAsyncResult.None)
                        {
                            return DataStorage.Contents.Create(subject, magnet);
                        }
                        else
                        {
                            MessageBox.Show("Time Out?");
                        }
                    }
                }
            }
            return null;
        }
    }
}
