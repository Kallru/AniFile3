using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace TestTorrentWithCpp
{
    class Program
    {
        static void Main(string[] args)
        {
            NativeInterface.Initialize();
            NativeInterface.Request();

            Console.ReadKey();
        }
    }
}
