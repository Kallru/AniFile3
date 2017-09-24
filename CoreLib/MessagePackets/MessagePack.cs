using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.MessagePackets
{
    public class MessagePack
    {
        public static byte[] Serialize<T>(T obj) => MessagePackSerializer.Serialize<T>(obj);
        public static void Serialize<T>(Stream stream, T obj) => MessagePackSerializer.Serialize<T>(stream, obj);

        public static T Deserialize<T>(Stream stream) => MessagePackSerializer.Deserialize<T>(stream);
        public static T Deserialize<T>(byte[] bytes) => MessagePackSerializer.Deserialize<T>(bytes);
    }
}
