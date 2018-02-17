using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace CacheServer_Test
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

        //static void ParseRSS(string xml)
        //{
        //    var xmlDocument = new XmlDocument();
        //    xmlDocument.LoadXml(xml);

        //    var root = xmlDocument.FirstChild;
        //    Console.WriteLine(root.InnerXml);
        //}

        static void Main(string[] args)
        {
            Console.WriteLine("Starting to read XML...");
            ReadFromXML();

            foreach (var uri in _rssUris)
            {
                var xmlstring = Request(uri);
                
                Regex regex = new Regex(@"&(?![a-z]{2,5};)");
                xmlstring = regex.Replace(xmlstring, "&amp;");

                using (XmlReader reader = XmlReader.Create(new StringReader(xmlstring)))
                {
                    Console.WriteLine("[Debug] Start Load-----");
                    Console.WriteLine(xmlstring);
                    Console.WriteLine("========================================");

                    try
                    {
                        SyndicationFeed feed = SyndicationFeed.Load(reader);
                        reader.Close();
                        foreach (SyndicationItem item in feed.Items)
                        {
                            Console.WriteLine("{0}, {1}", item.Title.Text, item.Links[0].Uri);
                        }
                    }
                    catch (System.FormatException e)
                    {
                        Console.WriteLine("Got Exception - {0}", e.Message);
                    }
                }

                //using (var file = new StreamWriter("text.xml"))
                //{
                //    file.Write(xmlstring);
                //}
            }

            Console.WriteLine("Finished, Please press any key");
            Console.ReadKey();
        }
    }
}
