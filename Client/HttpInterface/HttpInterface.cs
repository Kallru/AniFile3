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

            TOut result = default(TOut);
            WebResponse response = null;
            try
            {
                response = await request.GetResponseAsync();
            }
            catch(System.Net.WebException e)
            {
                Console.WriteLine(e.Message);
            }

            if (response != null)
            {
                var httpResponse = response as HttpWebResponse;
                Console.WriteLine(httpResponse.StatusDescription);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    dataStream = response.GetResponseStream();
                    result = MessagePackSerializer.Deserialize<TOut>(dataStream);
                    dataStream.Close();
                }

                response.Close();
            }

            return result;
        }
    }
}
