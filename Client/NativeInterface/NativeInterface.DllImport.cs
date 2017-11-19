using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AniFile3.Native
{
    public partial class NativeInterface
    {
        const string DllFileName = "TorrentInterface.dll";

        public delegate void Win32HandleCallback(Int64 id, string message, IntPtr inputData, uint size);

        [DllImport(DllFileName, EntryPoint = "InitializeEngine", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern void InitializeEngine(Win32HandleCallback callback);

        [DllImport(DllFileName, EntryPoint = "UninitializeEngine", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern void UninitializeEngine();

        [DllImport(DllFileName, EntryPoint = "Request", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool RequestInternal(Int64 id, string message, byte[] inputData, uint size, ref IntPtr pOutputData, ref uint outputSize);
    }
}
