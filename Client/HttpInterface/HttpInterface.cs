﻿using CoreLib;
using CoreLib.Extentions;
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
            Log.WriteLine("Connetion..." + host);
        }

        public async Task<TOut> RequestWithTimeout<TOut>(string rest)
        {
            return await Request<bool, TOut>(rest, true).WithTimeout(Preference.Instance.DefaultTimeOut);
        }

        public async Task<TOut> RequestWithTimeout<TIn, TOut>(string rest, TIn input)
        {
            return await Request<TIn, TOut>(rest, input).WithTimeout(Preference.Instance.DefaultTimeOut);
        }

        public async Task<TOut> Request<TIn, TOut>(string rest, TIn input)
        {
            byte[] byteArray = MessagePackSerializer.Serialize(input);

            var request = WebRequest.Create(_host + rest);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            TOut result = default(TOut);
            Stream dataStream = null;
            WebResponse response = null;
            try
            {
                // Request
                Log.WriteLine("Request...'{0}'", rest);
                dataStream = await request.GetRequestStreamAsync();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                // Response
                response = await request.GetResponseAsync();
                var httpResponse = response as HttpWebResponse;
                Log.WriteLine("Response..." + httpResponse.StatusDescription);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    dataStream = response.GetResponseStream();
                    result = MessagePackSerializer.Deserialize<TOut>(dataStream);
                }
            }
            catch (System.Net.WebException e)
            {
                string hint = string.Empty;
                if (e.Status == WebExceptionStatus.ConnectFailure)
                {
                    hint = _host + rest;
                }

                Log.WriteLine("{0} '{1}'", e.Message, hint);
            }
            catch (System.Exception e)
            {
                Log.WriteLine("{0}", e.Message);
            }
            finally
            {
                dataStream?.Close();
                response?.Close();
            }

            return result;
        }
    }
}
