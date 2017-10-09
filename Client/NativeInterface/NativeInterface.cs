using MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AniFile3
{
    [MessagePackObject]
    public struct StateInfo
    {
        [Key(0)]
        public int State;
        [Key(1)]
        public float DownloadPayloadRate;
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
                    default: return "준비중";
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
        private static Win32HandleCallback _recvMessageFromWin32;
        private static HashSet<Int64> _idIssued;
        private static Dictionary<Int64, Instance> _instances;

        private class Instance
        {
            private Int64 _id;
            private CancellationTokenSource _cancelTokenSource;
            private Task _main;

            public Action<StateInfo> StateUpdated;

            public Instance(Int64 id)
            {
                _id = id;
                _cancelTokenSource = new CancellationTokenSource();
            }

            public void Destroy()
            {
                _cancelTokenSource.Cancel();                
                _main.Wait();
            }

            private async void AddTorrentHandle(string hash)
            {
                _main = Task.Run(() =>
                {
                    while (true)
                    {
                        if (_cancelTokenSource.IsCancellationRequested)
                            break;

                        StateInfo stateInfo;
                        if (Request(_id, "QueryState", out stateInfo))
                        {
                            if (StateUpdated != null)
                                StateUpdated(stateInfo);
                        }

                        Thread.Sleep(10);
                    }
                }, _cancelTokenSource.Token);

                await _main;
            }
        }

        private static Int64 CreateInstance()
        {
            Int64 newId = 0;
            foreach (var usedId in _idIssued)
            {
                if (newId != usedId)
                {
                    break;
                }
                ++newId;
            }

            _idIssued.Add(newId);
            _instances.Add(newId, new Instance(newId));
            return newId;
        }

        private static void DestroyInstance(Int64 id)
        {
            _idIssued.Remove(id);
            _instances.Remove(id);
        }

        public static void Initialize()
        {
            _idIssued = new HashSet<Int64>();
            _instances = new Dictionary<Int64, Instance>();
            _recvMessageFromWin32 = RecvMessage;
            InitializeEngine(_recvMessageFromWin32);
        }

        public static void Uninitialize()
        {
            foreach(var instance in _instances)
            {
                instance.Value.Destroy();
            }

            UninitializeEngine();
        }

        public static void RecvMessage(Int64 id, string message, IntPtr inputData, uint size)
        {
            Debug.Assert(_instances.ContainsKey(id));

            var instance = _instances[id];

            var output = new byte[size];
            Marshal.Copy(inputData, output, 0, (int)size);

            Application.Current.Dispatcher.Invoke(() =>
            {
                var medthods = typeof(Instance).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
                var medthod = medthods.FirstOrDefault(element => element.Name == message);
                var parameters = medthod.GetParameters();
                if (parameters.Length > 1)
                {
                    var parametersObj = MessagePackSerializer.Deserialize<object[]>(output);
                    medthod.Invoke(instance, parametersObj);
                }
                else
                {
                    var parametersObj = MessagePackSerializer.Deserialize<object>(output);
                    medthod.Invoke(instance, new[] { parametersObj });
                }
            });
        }

        public static Int64 Download(string magnetLink, string savePath, Action<StateInfo> stateUpdatedCallback)
        {
            var id = CreateInstance();
            if(id != -1)
            {
                if (Request(id, "StartDownload", new Tuple<string, string>(magnetLink, savePath)))
                {
                    _instances[id].StateUpdated = stateUpdatedCallback;
                    return id;
                }
            }

            DestroyInstance(id);
            return -1;
        }

        public static bool RequestInfo(Int64 id)
        {
            Tuple<Int64> data;
            bool b = Request(id, "QueryInfo", out data);

            //Request("QueryInfo", bytes, (uint)bytes.Length, ref pData, ref outputSize);
            return true;
        }

#region 데이터 통신 유틸 메소드들
        public static bool Request(Int64 id, string message, byte[] bytes, out byte[] output)
        {
            output = null;

            IntPtr pData = IntPtr.Zero;
            uint outputSize = 0;
            int inputSize = bytes?.Length ?? 0;

            bool bResult = RequestInternal(id, message, bytes, (uint)inputSize, ref pData, ref outputSize);
            if (bResult == true)
            {
                if (outputSize > 0)
                {
                    output = new byte[outputSize];
                    Marshal.Copy(pData, output, 0, (int)outputSize);
                    return true;
                }
            }
            return bResult;
        }

        public static bool Request<OUT>(Int64 id, string message, out OUT output, byte[] input = null)
        {
            output = default(OUT);            

            byte[] data;
            bool bResult = Request(id, message, input, out data);
            if (data != null &&
                data.Length > 0)
            {
                output = MessagePackSerializer.Deserialize<OUT>(data);
                return true;
            }

            return bResult;
        }

        public static bool Request<IN>(Int64 id, string message, IN input)
        {
            byte[] data = null;
            return Request(id, message, MessagePackSerializer.Serialize(input), out data);
        }

        public static bool Request<IN, OUT>(Int64 id, string message, IN input, out OUT output)
        {
            byte[] bytes = MessagePackSerializer.Serialize(input);
            return Request(id, message, out output, bytes);
        }
    }
#endregion
}
