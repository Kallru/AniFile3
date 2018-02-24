using MongoDB.Driver;
using Nancy.Hosting.Self;
using System;

namespace CacheServerSystem
{
    public class ServerEngine : IDisposable
    {
        private MainModule _serverModule;
        private NancyHost _host;

        public readonly string URL;

        public ServerEngine(string url, int timeOutms)
        {
            URL = url;
            MainModule.TIME_OUT = timeOutms;
        }

        public void Dispose()
        {
            if (_host != null)
            {
                _host.Dispose();
                _host = null;
                _serverModule = null;
            }       
        }
        
        public void Start()
        {
            _serverModule = new MainModule();

            var configuration = new HostConfiguration()
            {
                UrlReservations = new UrlReservations()
                {
                    CreateAutomatically = true,
                }
            };

            _host = new NancyHost(configuration, new Uri(URL));
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
    }
}
