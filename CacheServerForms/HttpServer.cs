using System;
using System.Collections.Generic;
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

            Get["/test"] = (parameter)=>
            {
                return "hi";
            };

            Post["/some"] = (paramter) =>
            {
                return "post";
            };
        }
    }
}
