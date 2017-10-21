using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AniFile3
{
    public class HttpInterface
    {
        private string _host;

        public HttpInterface(string host)
        {
            _host = host;
        }

        public async Task<TOut> Request<TIn, TOut>(string rest, TIn input)
        {
            byte[] byteArray = MessagePackSerializer.Serialize(input);

            var request = WebRequest.Create(_host + rest);
            request.Method = "POST";

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            
            return await Task<TOut>.Run(() =>
            {
                WebResponse response = request.GetResponse();

                //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

                dataStream = response.GetResponseStream();
                var result = MessagePackSerializer.Deserialize<TOut>(dataStream);
                response.Close();
                dataStream.Close();

                return result;
            });
        }
    }
}
