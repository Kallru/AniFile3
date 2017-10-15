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
            Get["/"] = test => "hi";
        }
    }
}
