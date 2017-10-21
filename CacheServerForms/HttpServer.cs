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
                var body = this.Request.Body;
                var data = new byte[body.Length];
                body.Read(data, 0, (int)body.Length);

                var text = Encoding.UTF8.GetString(data);

                var collection = DataBase.Instance.Collection;

                var task = collection.AsQueryable()
                                            .Where(item => SearchCondition(item, text))
                                            .ToListAsync();

                var result = await Task.WhenAny(task, Task.Delay(5000));
                if (result == task)
                {

                }

                var found = await task;

                Stream dataStream = new MemoryStream();
                MessagePackSerializer.Serialize(dataStream, found);
                dataStream.Seek(0, SeekOrigin.Begin);

                return Response.FromStream(dataStream, "application/octet-stream");
            };
        }

        // 나중에 더 복잡해질 수 있으니 미리 빼두자
        public bool SearchCondition(DataStorage.Contents item, string text)
        {
            return item.Name.Contains(text);
        }
    }
}
