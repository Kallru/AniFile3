using Scriping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;

namespace AniFile3.MagnetLising
{
    // 이름 짓기 어렵네
    public class MagnetListUp
    {
        private FirstSite _scriper;
        private List<SyndicationItem> _feeds;

        public MagnetListUp()
        {
            // 스크랩퍼 초기화
            _scriper = new Scriping.FirstSite(new System.Windows.Forms.WebBrowser());
            _scriper.InitializeCompleted += async () =>
            {
                // 여기서 검색등 자료를 찾고, 그 자료를 서버로 보낸다.

                // Test
                //var result = await _scriper.SearchBox("아는 형님");

                //foreach(var item in result)
                //{
                //    Console.WriteLine("{0}:{1}", item.Name, item.Episode);
                //}
            };

            _scriper.Initialize();
        }
        
        private string RequestXMLString(string uri)
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

        // feeds에 모든 RSS 피드를 채워 넣음
        public void UpdateRSS()
        {
            _feeds = new List<SyndicationItem>();

            foreach (var uri in Preference.Instance.RSSList)
            {
                var xmlstring = RequestXMLString(uri);

                Regex regex = new Regex(@"&(?![a-z]{2,5};)");
                xmlstring = regex.Replace(xmlstring, "&amp;");

                using (XmlReader reader = XmlReader.Create(new StringReader(xmlstring)))
                {
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    reader.Close();
                    foreach (SyndicationItem item in feed.Items)
                    {
                        _feeds.Add(item);
                        Console.WriteLine("{0}, {1}", item.Title.Text, item.Links[0].Uri);
                    }
                }
            }
        }
    }
}
