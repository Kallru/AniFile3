using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestTorrentWithCpp
{
    public partial class NativeInterface
    {
        const string DllFileName = "TorrentInterface.dll";

        [DllImport(DllFileName, EntryPoint = "InitializeEngine", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern void InitializeEngine();

        [DllImport(DllFileName, EntryPoint = "Request", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Request(string message, byte[] inputData, uint size, ref IntPtr pOutputData, ref uint outputSize);

        //[DllImport(DllFileName, EntryPoint = "StartEntityToolInterface", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void StartInternal();

        //[DllImport(DllFileName, EntryPoint = "SendCommandInterface", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        //private static extern bool SendCommandInternal(string action, byte[] data, uint dataSize);        

        //[DllImport(DllFileName, EntryPoint = "ReadPoolInterface", CallingConvention = CallingConvention.Cdecl)]
        //private static extern bool ReadPoolInternal(byte[] outputData);
    }
}
