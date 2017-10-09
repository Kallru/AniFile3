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

        [IgnoreMember]
        public string StateText
        {
            get
            {
                switch ((state_t)State)
                {
                    case state_t.checking_files: return "체크 중";
                    case state_t.downloading_metadata: return "메타데이터";
                    case state_t.downloading: return "다운로드";
                    case state_t.finished: return "완료";
                    case state_t.seeding: return "시드";
                    case state_t.allocating: return "할당 중";
                    case state_t.checking_resume_data: return "체크 중단점";
                    default: return "상태검사중";
                }
            }
        }
    }

    // cpp에 있는 state_t 타입과 어떻게 하면 동기화 할지 고민
    enum state_t : int
    {
        queued_for_checking = 0,
        checking_files,
        downloading_metadata,
        downloading,
        finished,
        seeding,
        allocating,
        checking_resume_data
    };

    public partial class NativeInterface
    {
        public static void Initialize()
        {
            InitializeEngine();
        }

        public async static void Download(string magnetLink, string savePath, Action<StateInfo> updateStateCallback)
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
                        if (updateStateCallback != null)
                            updateStateCallback(stateInfo);
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
