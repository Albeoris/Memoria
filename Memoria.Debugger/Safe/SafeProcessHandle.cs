using System;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;

namespace Memoria.Debugger
{
    internal sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeProcessHandle(Int32 processId, ProcessAccessFlags processAccess, Boolean bInheritHandle)
            : base(true)
        {
            handle = Kernel32.OpenProcess(processAccess, bInheritHandle, processId);
            if (IsInvalid)
                throw new Win32Exception();
        }

        protected override Boolean ReleaseHandle()
        {
            return Kernel32.CloseHandle(handle);
        }

        public SafeVirtualMemoryHandle Allocate(Int64 size, AllocationType allocation, MemoryProtection protection)
        {
            return new SafeVirtualMemoryHandle(handle, 0, size, allocation, protection);
        }

        public SafeRemoteThread CreateThread(IntPtr functionAddress, SafeVirtualMemoryHandle parametersAddress)
        {
            return new SafeRemoteThread(handle, functionAddress, parametersAddress.DangerousGetHandle());
        }
    }
}