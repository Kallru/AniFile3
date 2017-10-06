using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        public static bool Request()
        {
            //Sample test = new Sample();
            //test.First = "Hellow";
            //test.Second = "What's your name";
            //test.Thrid = 555;
            //test.Forth = 232.2f;
            //test.Five = 11111;

            Tuple<string, string> test = new Tuple<string, string>("hihihi", "toooo");

            var bytes = MessagePackSerializer.Serialize(test);

            IntPtr pData = IntPtr.Zero;
            uint outputSize = 0;

            Request("StartDownload", bytes, (uint)bytes.Length, ref pData, ref outputSize);

            byte[] data = new byte[outputSize];
            Marshal.Copy(pData, data, 0, (int)outputSize);
            
            var testNew = MessagePackSerializer.Deserialize<Tuple<bool, string, int>>(data);

            //outputData = null;
            //IntPtr pData = IntPtr.Zero;

            //uint outputSize = 0;
            //if (Request(cmd.Action, cmd.Data, cmd.Data == null ? 0 : (uint)cmd.Data.Length, ref pData, ref outputSize))
            //{
            //    byte[] data = new byte[outputSize];
            //    Marshal.Copy(pData, data, 0, (int)outputSize);

            //    outputData = new MessageStream(data);
            //    return true;
            //}
            return false;
        }
    }
}
