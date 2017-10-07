using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestTorrentWithCpp
{
    [MessagePackObject]
    public struct Sample
    {
        [Key(0)]
        public string First;
        [Key(1)]
        public string Second;
        [Key(2)]
        public int Thrid;
        [Key(3)]
        public float Forth;
        [Key(4)]
        public uint Five;

        public Sample(string first, string second, int thrid, float forth, uint five)
        {
            First = first;
            Second = second;
            Thrid = thrid;
            Forth = forth;
            Five = five;
        }
    }

    public partial class NativeInterface
    {
        public static void Initialize()
        {
            InitializeEngine();
        }

        public async static void Request()
        {
            Tuple<string, string> test = new Tuple<string, string>("magnet:?xt=urn:btih:95F6D0F207888DDB67F89EDC0F47D39B945D2E95&dn=%5btvN%5d%20%ec%95%8c%eb%b0%94%ed%8a%b8%eb%a1%9c%ec%8a%a4.E04.171004.720p-NEXT.mp4&tr=udp%3a%2f%2fzer0day.to%3a1337%2fannounce&tr=udp%3a%2f%2ftracker1.wasabii.com.tw%3a6969%2fannounce&tr=http%3a%2f%2fmgtracker.org%3a6969%2fannounce&tr=udp%3a%2f%2ftracker.grepler.com%3a6969%2fannounce&tr=http%3a%2f%2ftracker.kamigami.org%3a2710%2fannounce&tr=udp%3a%2f%2f182.176.139.129%3a6969%2fannounce&tr=http%3a%2f%2ftracker.mg64.net%3a6881%2fannounce&tr=udp%3a%2f%2f185.50.198.188%3a1337%2fannounce&tr=udp%3a%2f%2f168.235.67.63%3a6969%2fannounce&tr=udp%3a%2f%2ftracker.leechers-paradise.org%3a6969&tr=udp%3a%2f%2fbt.xxx-tracker.com%3a2710%2fannounce&tr=http%3a%2f%2fexplodie.org%3a6969%2fannounce&tr=udp%3a%2f%2ftracker.coppersurfer.tk%3a6969%2fannounce&tr=udp%3a%2f%2ftracker.ilibr.org%3a80%2fannounce&tr=udp%3a%2f%2ftracker.coppersurfer.tk%3a6969&tr=http%3a%2f%2fbt.ttk.artvid.ru%3a6969%2fannounce&tr=http%3a%2f%2fbt.artvid.ru%3a6969%2fannounce&tr=http%3a%2f%2ftracker2.wasabii.com.tw%3a6969%2fannounce&tr=udp%3a%2f%2fthetracker.org.%2fannounce&tr=udp%3a%2f%2feddie4.nl%3a6969%2fannounce&tr=udp%3a%2f%2f62.212.85.66%3a2710%2fannounce&tr=udp%3a%2f%2ftracker.ilibr.org%3a6969%2fannounce&tr=udp%3a%2f%2ftracker.zer0day.to%3a1337%2fannounce", ".");
            var bytes = MessagePackSerializer.Serialize(test);

            IntPtr pData = IntPtr.Zero;
            uint outputSize = 0;
            Request("StartDownload", bytes, (uint)bytes.Length, ref pData, ref outputSize);

            await Task.Run(() =>
            {
                while(true)
                {
                    Request("QueryState", null, 0, ref pData, ref outputSize);

                    if (outputSize > 0)
                    {
                        byte[] data = new byte[outputSize];
                        Marshal.Copy(pData, data, 0, (int)outputSize);

                        var stateInfo = MessagePackSerializer.Deserialize<Tuple<int, int, Int64, int>>(data);
                        
                        Console.Clear();
                        Console.WriteLine("State:{0}, {1} kB/s, {2} kB, {3}% downloaded\x1b",
                            stateInfo.Item1,
                            stateInfo.Item2,
                            stateInfo.Item3,
                            stateInfo.Item4);

                        if (stateInfo.Item4 > 40)
                            break;
                    }

                    Thread.Sleep(10);
                    break;
                }
            });
        }
    }
}
