using MongoDB.Driver.Linq;
using Nancy.Hosting.Self;
using RichGrassHopper.Core.IO;
using Scriping;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

using MongoDB.Driver;

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

        private async void TestViewerForm_Load(object sender, EventArgs e)
        {
           _scriper.InitializeCompleted += async () =>
            {
                // Test
                //var result = await _scriper.SearchBox("아는 형님");

                //var collection = DataBase.Instance.Main.GetCollection<ServerEpisodeInfo>("content");

                //if (result.Count > 0)
                //{
                //    await collection.InsertManyAsync(result.Select(item => new ServerEpisodeInfo() { Info = item }));
                //}

                ////var collection = _db.GetCollection<DataStorage.Contents>("content");
                ////collection.Indexes

                ////var result = await collection.Find((itme) => true).FirstOrDefaultAsync();
                ////var test = collection.AsQueryable<DataStorage.Contents>().Where(item => true).ToList();
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

            {
                // Test Code
                var filter = MongoDB.Driver.Builders<ServerEpisodeInfo>.Filter;

                // 주의. Search 태그로 텍스트 검색시, 공백없는 단어 단위로만 가능함.
                // 예) '비정상회담' 검색시 "비정상" 이렇게 안됨.
                // 검색 패턴에서 공백을 넣으면 or로 연산하기 때문.
                // "아는 형님" 이라고 넣으면, "아는" or "형님"으로 검색됨!!!
                //var fi = filter.Text("아는 비정상회담");
                //var fi = filter.Text("360p");

                //var fi = filter.And(
                //    filter.Eq(field=>field.Info.Name, "/NEXT/")
                //    );

                //var fi = filter.ElemMatch(field => field.Info.Name, "NEXT");
                //FilterDefinition<ServerEpisodeInfo> some = "{ $and :[ { \"Info.Name\" : /비정상/ },{ \"Info.Episode\" : { $gt : 167 } } ] }";
                //FilterDefinition<ServerEpisodeInfo> some2 = "{ $and :[ { \"Info.Name\" : /아는/ },{ \"Info.Episode\" : { $gt : 98 } } ] }";
                //var f2 = some | some2;

                //var v2 = filter.In(node => node.Info.Name, new string[] { "/NEXT/", "아는" });

                //var list = new System.Collections.Generic.List<Tuple<string, int>>();
                //list.Add(Tuple.Create("비정상", 167));
                //list.Add(Tuple.Create("아는", 98));

                //var f2 = MakeFilter(list);

                //var collection = DataBase.Instance.Collection;
                //var a = await collection.Find(f2).ToListAsync();

                //int d = 20;
            }
        }

        private FilterDefinition<ServerEpisodeInfo> MakeFilter(System.Collections.Generic.List<Tuple<string, int>> datas)
        {
            System.Collections.Generic.List<FilterDefinition<ServerEpisodeInfo>> filters = new System.Collections.Generic.List<FilterDefinition<ServerEpisodeInfo>>();

            foreach (var item in datas)
            {
                filters.Add(string.Format("{{ $and :[ {{ \"Info.Name\" : /{0}/ }}, {{ \"Info.Episode\" : {{ $gt : {1} }} }} ] }}"
                                                                        , item.Item1
                                                                        , item.Item2));
            }
            return Builders<ServerEpisodeInfo>.Filter.Or(filters);
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
