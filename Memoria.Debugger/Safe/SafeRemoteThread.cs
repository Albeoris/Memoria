using System;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;

namespace Memoria.Debugger
{
    internal sealed class SafeRemoteThread : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeRemoteThread(IntPtr processHandle, IntPtr functionAddress, IntPtr parametersAddress)
            : base(true)
        {
            IntPtr threadId;
            handle = Kernel32.CreateRemoteThread(processHandle, IntPtr.Zero, IntPtr.Zero, functionAddress, parametersAddress, 0, out threadId);
            if (IsInvalid)
                throw new Win32Exception();
        }

        protected override Boolean ReleaseHandle()
        {
            return Kernel32.CloseHandle(handle);
        }

        public void Join()
        {
            if (Kernel32.WaitForSingleObject(handle, 0xFFFFFFFF) == 0xFFFFFFFF)
                throw new Win32Exception();
        }
    }
}