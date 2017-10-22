using CoreLib.DataStruct;
using MessagePack;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nancy;
using Scriping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheServerForms
{
    public class HttpServer : Nancy.NancyModule
    {
        public HttpServer()
        {
            // 1. 클라가 필요한 구독들을 요청해옴
            Post["/subscription"] = parameter =>
            {
                // 파라메터로 요청 데이터들이 옴(어떤 것들이 필요한지)
                // 디비에서 찾아서 필요한것들 리턴
                return 1212;
            };

            Get["/test"] = (parameter) =>
            {
                return "hi";
            };

            Post["/search", true] = async (paramter, ct) =>
            {
                string text = GetRequestData<string>();

                var collection = DataBase.Instance.Collection;

                var task = collection.AsQueryable()
                                            .Where(item => item.Info.Name.Contains(text))
                                            .ToListAsync();

                var result = await Task.WhenAny(task, Task.Delay(5000));

                var response = new Response();

                int code = 0;
                code = (result == task) ? 0x01 : 0x00;
                code |= (task.Result != null && task.Result.Count > 0) ? 0x10 : 0x00;

                switch (code)
                {
                    case 0x11:
                        // 성공
                        Stream dataStream = new MemoryStream();
                        var sendingDatas = task.Result.Select(item => item.Info).ToList();

                        MessagePackSerializer.Serialize(dataStream, sendingDatas);
                        dataStream.Seek(0, SeekOrigin.Begin);

                        response = Response.FromStream(dataStream, "application/octet-stream");
                        response.StatusCode = HttpStatusCode.OK;
                        break;
                    case 0x01:
                        response.StatusCode = HttpStatusCode.NoContent;
                        break;
                    default:
                        response.StatusCode = HttpStatusCode.NotFound;
                        break;
                }

                // Error
                return response;
            };
        }

        private T GetRequestData<T>()
        {
            var body = this.Request.Body;
            var data = new byte[body.Length];
            body.Read(data, 0, (int)body.Length);
            return MessagePackSerializer.Deserialize<T>(data);
        }
    }
}
