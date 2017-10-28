using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace CacheServer
{
    class Program
    {
        static string Request(string uri)
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

        static List<string> _rssUris;
        static void ReadFromXML()
        {
            _rssUris = new List<string>();

            string filename = "RSSlist.xml";
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);
            
            var root = xmlDocument.DocumentElement.GetElementsByTagName("rss");
            foreach (XmlElement node in root)
            {
                _rssUris.Add(node.InnerText);
            }
        }

        static void ParseRSS(string xml)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            var root = xmlDocument.FirstChild;
            Console.WriteLine(root.InnerXml);
        }

        static void Main(string[] args)
        {
            var read = XmlReader.Create(@"https://torrentkim10.net/bbs/rss.php?k=720p&b=torrent_variety");

            

            read.Close();

            ReadFromXML();

            foreach(var uri in _rssUris)
            {
                var xmlstring = Request(uri);

                Regex reg = new Regex(@"&(?![a-z]{2,5};)");

                var aa = reg.Matches(xmlstring);

                var ss = reg.Replace(xmlstring, "&amp;");

                var test = new XmlDocument();
                test.LoadXml(ss);

                using (var file = new StreamWriter("text.xml"))
                {
                    file.Write(xmlstring);
                }
            }

            Console.ReadKey();
        }
    }
}
