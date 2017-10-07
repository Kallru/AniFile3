using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AniFile3
{
    [MessagePackObject]
    public struct StateInfo
    {
        [Key(0)]
        public int State;
        [Key(1)]
        public int DownloadPayloadRate;
        [Key(2)]
        public Int64 Total;
        [Key(3)]
        public int Progress;

        public StateInfo(int state, int downloadPayloadRate, long total, int progress)
        {
            State = state;
            DownloadPayloadRate = downloadPayloadRate;
            Total = total;
            Progress = progress;
        }
    }

    public partial class NativeInterface
    {
        public delegate void UpdateStateEvent(StateInfo stateInfo);
        public static event UpdateStateEvent UpdatedState;

        public static void Initialize()
        {
            InitializeEngine();
        }

        public async static void Download(string magnetLink, string savePath)
        {
            var bytes = MessagePackSerializer.Serialize(new Tuple<string, string>(magnetLink, savePath));

            IntPtr pData = IntPtr.Zero;
            uint outputSize = 0;
            Request("StartDownload", bytes, (uint)bytes.Length, ref pData, ref outputSize);

            await Task.Run(() =>
            {
                while (true)
                {
                    Request("QueryState", null, 0, ref pData, ref outputSize);

                    if (outputSize > 0)
                    {
                        byte[] data = new byte[outputSize];
                        Marshal.Copy(pData, data, 0, (int)outputSize);

                        var stateInfo = MessagePackSerializer.Deserialize<StateInfo>(data);
                        if (UpdatedState != null)
                            UpdatedState(stateInfo);
                    }
                    Thread.Sleep(10);
                }
            });
        }

        public static void Request()
        {
            
        }
    }
}
