﻿using CoreLib;
using CoreLib.DataStruct;
using CoreLib.Extentions;
using CoreLib.MessagePackets;
using MessagePack;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nancy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CacheServerSystem
{
    public class MainModule : Nancy.NancyModule
    {
        public static int TIME_OUT = 5000;  // ms

        public MainModule()
        {
            // 1. 클라가 필요한 구독들을 요청해옴
            //Post["/subscription"] = parameter =>
            //{
            //    // 파라메터로 요청 데이터들이 옴(어떤 것들이 필요한지)
            //    // 디비에서 찾아서 필요한것들 리턴
            //    return 1212;
            //};

            Post["/update_subscription", true] = async (parameter, cancellation) =>
            {
                WirteStartingLog();
                var request = UnpackRequest<UpdateSubscriptionRequest>();
                var response = new UpdateSubscriptionResponse()
                {
                    EpisodeInfos = new List<EpisodeInfo>()
                };

                var collection = DataBase.Instance.Contents;
                var query = MakeFilterForGettingNameAndEpisode(request.Subscriptions);

                var result = await collection.Find(query).ToListAsync().WithTimeout(TIME_OUT);
                if (result != null)
                {
                    response.EpisodeInfos = result.Select(item => item.Info).ToList();
                }

                WriteEndingLog();
                // 넘김
                return PackResponse(response);
            };

            Post["/search_episode", true] = async (paramter, ct) =>
            {
                WirteStartingLog();
                string text = UnpackRequest<string>();

                var collection = DataBase.Instance.Contents;

                var result = await collection.AsQueryable()
                                             .Where(item => item.Info.Name.Contains(text))
                                             .ToListAsync()
                                             .WithTimeout(TIME_OUT);

                var response = new Response();

                int code = 0;
                code = (result == null) ? 0x01 : 0x00;
                code |= (result != null && result.Count > 0) ? 0x10 : 0x00;

                switch (code)
                {
                    case 0x10:
                        // 성공
                        var sendingDatas = result.Select(item => item.Info).ToList();
                        response = PackResponse(sendingDatas);
                        break;
                    case 0x01:
                        response.StatusCode = HttpStatusCode.NoContent;
                        break;
                    default:
                        response.StatusCode = HttpStatusCode.NotFound;
                        break;
                }

                WriteEndingLog();
                // Error
                return response;
            };

            // 서버에 있는 RSS 리스트 전달
            Post["/update_rsslist", true] = async (parameter, cancellation) =>
            {
                WirteStartingLog();

                var collection = DataBase.Instance.RssList;

                Log.WriteLine("Query to DB...");
                var rsslist = await collection.Find(Builders<RssListInfo>.Filter.Empty)
                                              .ToListAsync();
                Log.WriteLine("Found '{0}'", rsslist.Count);

                // 클라에 넘겨줄 포맷으로 바꾼다.
                List<string> response = rsslist.Select(info => info.RssAddress).ToList();

                WriteEndingLog();
                return PackResponse(response);
            };

            // 클라가 갖고 있는 RSS Feed 정보들을 서버로 보내준다.
            Post["/store_feeds", true] = async (parameter, cancellation) =>
            {
                WirteStartingLog();

                string errorMessage = "OK";

                var clientEpisodes = UnpackRequest<List<EpisodeInfo>>();

                var collection = DataBase.Instance.Contents;

                Log.WriteLine("Insert({0}) to DB...", clientEpisodes.Count);

                var episodes = clientEpisodes.Select(info => new ServerEpisodeInfo()
                {
                    Info = info
                });

                // 일단 그냥 때려 넣지만, [데이터 중복문제]가 남아 있다.
                var tokenSource = new CancellationTokenSource();
                var task = collection.InsertManyAsync(episodes, null, tokenSource.Token);
                if (await Task.WhenAny(task, Task.Delay(TIME_OUT)) != task)
                {
                    // 타임아웃 처리
                    tokenSource.Cancel();
                    errorMessage = "Timeout";
                    Log.Error("{0}...{1}", Request.Path, errorMessage);
                }

                WriteEndingLog();                
                return PackResponse(errorMessage);
            };
        }

        private void WirteStartingLog() => Log.WriteLine("{0}...Post", Request.Path);
        private void WriteEndingLog() => Log.WriteLine("{0}...Finished", Request.Path);

        private Response PackResponse<T>(T data)
        {
            // 이 데이터 스트림을 여기서 닫아버리면, response의 데이터가 깨진다.
            Stream dataStream = new MemoryStream();

            MessagePackSerializer.Serialize(dataStream, data);
            dataStream.Seek(0, SeekOrigin.Begin);

            var response = Response.FromStream(dataStream, "application/octet-stream");
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        private T UnpackRequest<T>()
        {
            var body = this.Request.Body;
            var data = new byte[body.Length];
            body.Read(data, 0, (int)body.Length);
            return MessagePackSerializer.Deserialize<T>(data);
        }

        // - 현재 안씀
        // $In 쿼리를 사용해서, 입력 받은 여러 텍스트를 $In로 리턴
        // 입력 받는 각 텍스트는 OR 연산
        //private string GetInQueryToSearchTextsOR(string[] texts)
        //{
        //    string result = "{\"Info.Name\": { $in:[";

        //    for (int i = 0; i < texts.Length; ++i)
        //    {
        //        if (i > 0)
        //            result += ",";

        //        // 텍스트 양쪽에 '/'를 추가해서 부분 검색도 가능하게 추가 한다.
        //        result += "/" + texts[i] + "/";
        //    }

        //    result += " ] } }";
        //    return result;
        //}

        // This is making filter for getting 'Name' and 'Episode
        private FilterDefinition<ServerEpisodeInfo> MakeFilterForGettingNameAndEpisode(List<UpdateSubscriptionRequest.Request> datas)
        {
            var filters = new List<FilterDefinition<ServerEpisodeInfo>>();

            foreach (var item in datas)
            {
                filters.Add(string.Format("{{ $and :[ {{ \"Info.Name\" : /{0}/ }}, {{ \"Info.Episode\" : {{ $gt : {1} }} }} ] }}"
                                                                        , item.SubscriptionName
                                                                        , item.LatestEpisode));
            }
            return Builders<ServerEpisodeInfo>.Filter.Or(filters);
        }
    }
}
