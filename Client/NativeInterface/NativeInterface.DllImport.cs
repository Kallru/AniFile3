using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AniFile3
{
    public partial class NativeInterface
    {
        const string DllFileName = "TorrentInterface.dll";

        [DllImport(DllFileName, EntryPoint = "InitializeEngine", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern void InitializeEngine();

        [DllImport(DllFileName, EntryPoint = "UninitializeEngine", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern void UninitializeEngine();

        [DllImport(DllFileName, EntryPoint = "Request", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Request(string message, byte[] inputData, uint size, ref IntPtr pOutputData, ref uint outputSize);
    }
}
