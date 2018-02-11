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

        static void ParseRSS(string xml)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            var root = xmlDocument.FirstChild;
            Console.WriteLine(root.InnerXml);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Start Main");
            using (var reader = new MyXmlReader(new FileStream("output.xml", FileMode.Open)))
            {
                Console.WriteLine("[Debug] Start Load-----");

                //try
                {
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    reader.Close();
                    foreach (SyndicationItem item in feed.Items)
                    {
                        //Console.WriteLine("{0}, {1}", item.Title.Text, item.Links[0].Uri);
                    }
                }
                //catch (System.FormatException e)
                //{
                //    Console.WriteLine("Got Exception - {0}", e.Message);
                //}
            }
            return;

            Console.WriteLine("Starting to read XML...");
            ReadFromXML();

            foreach (var uri in _rssUris)
            {
                var xmlstring = Request(uri);
                
                Regex regex = new Regex(@"&(?![a-z]{2,5};)");
                xmlstring = regex.Replace(xmlstring, "&amp;");

                // TestCode
                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(xmlstring);
                //doc.Save("output.xml");

                //using (XmlReader reader = XmlReader.Create(new StringReader(xmlstring)))
                //using(var reader = new MyXmlReader(new StringReader(xmlstring)))
                using (var reader = new MyXmlReader(new FileStream("output.xml", FileMode.Open)))
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

    class MyXmlReader : XmlTextReader
    {
        private bool readingDate = false;
        const string CustomUtcDateTimeFormat = "ddd MMM dd HH:mm:ss Z yyyy"; // Wed Oct 07 08:00:07 GMT 2009

        public MyXmlReader(Stream s) : base(s) { }
        public MyXmlReader(TextReader tr) : base(tr) { }
        
        public override string ReadElementContentAsString()
        {
            Console.WriteLine("[Debug] ReadElementContentAsString - {0}, Value - {1}, Line:{2},Pos:{3}", base.LocalName, base.Value, base.LineNumber, base.LinePosition);

            if (string.Equals(base.NamespaceURI, string.Empty, StringComparison.InvariantCultureIgnoreCase) &&
                (string.Equals(base.LocalName, "lastBuildDate", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(base.LocalName, "pubDate", StringComparison.InvariantCultureIgnoreCase)))
            {
                string dateString = base.ReadString();

                DateTime dt;

                if (!DateTime.TryParse(dateString, out dt))
                {
                    if (!DateTime.TryParseExact(dateString, CustomUtcDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        // date 값이 없을때 기본값을 준다.
                        dt = new DateTime(DateTime.Today.Year - 2, 1, 1);
                    }
                }
                return dt.ToUniversalTime().ToString("R", CultureInfo.InvariantCulture);
            }

            // default
            return base.ReadElementContentAsString();
        }

        public override XmlNodeType MoveToContent()
        {
            //Console.WriteLine(Environment.StackTrace);
            //Console.WriteLine("[Debug] --- MoveToContent ---");
            //Console.WriteLine("Before, MoveToContent -" + base.LocalName);
            //var value = base.MoveToContent();
            //Console.WriteLine("After, MoveToContent -" + base.LocalName);
            //Console.WriteLine("After Value, MoveToContent -" + value);
            //Console.WriteLine("-----------------------------------");
            //return value;

            return base.MoveToContent();
        }

        public override void ReadStartElement()
        {
            Console.WriteLine("[Debug] ReadStartElement ---");
            Console.WriteLine("[Debug] NamespaceURI - " + base.NamespaceURI);
            Console.WriteLine("[Debug] LocalName - " + base.LocalName);
            Console.WriteLine("[Debug] --------------------");

            if (string.Equals(base.NamespaceURI, string.Empty, StringComparison.InvariantCultureIgnoreCase) &&
                (string.Equals(base.LocalName, "lastBuildDate", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(base.LocalName, "pubDate", StringComparison.InvariantCultureIgnoreCase)))
            {
                readingDate = true;

                Console.WriteLine("[Debug] Turn flag on");
            }
            base.ReadStartElement();
        }

        public override void ReadEndElement()
        {
            if (readingDate)
            {
                readingDate = false;
            }
            base.ReadEndElement();
        }

        public override string ReadString()
        {
            Console.WriteLine("[Debug] Start ReadString Method");
            if (readingDate)
            {
                Console.WriteLine("[Debug] Start ReadString Method");

                string dateString = base.ReadString();

                DateTime dt;

                if (!DateTime.TryParse(dateString, out dt))
                {
                    if(!DateTime.TryParseExact(dateString, CustomUtcDateTimeFormat, CultureInfo.InvariantCulture,DateTimeStyles.None, out dt))
                    {
                        // date 값이 없을때 기본값을 준다.
                        dt = new DateTime(DateTime.Today.Year - 2, 1, 1);
                    }
                }
                //return dt.ToUniversalTime().ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                string value = dt.ToUniversalTime().ToString("R", CultureInfo.InvariantCulture);
                
                Console.WriteLine("[Debug] " + value);
                //return value;
                return "Wed, 24 Feb 2010 18:56:04 GMT";
            }
            else
            {
                return base.ReadString();
            }
        }
    }
}
