using CacheServerSystem;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheServerCmd
{
    class Program
    {
        private static ServerEngine _framework;

        static void Main(string[] args)
        {
            string hostUrl = ConfigurationManager.AppSettings["host"];
            _framework = new ServerEngine(hostUrl, int.Parse(ConfigurationManager.AppSettings["timeout"]));
            _framework.Start();
            Console.WriteLine("Start '{0}'", hostUrl);

            while (true)
            {
                Console.WriteLine("Press <E> to exit..");
                if (Console.ReadKey().Key == ConsoleKey.E)
                {
                    _framework.Dispose();
                    _framework = null;
                    break;
                }
            }

            Console.WriteLine("exit.");
        }
    }
}
