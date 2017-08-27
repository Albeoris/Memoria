using System;
using System.Runtime.InteropServices;

namespace Memoria.Prime.WinAPI
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlFillMemory", SetLastError = false)]
        public static extern void FillMemory(IntPtr destination, UInt32 length, Byte fill);
    }
}