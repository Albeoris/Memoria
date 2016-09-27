using System;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;

namespace Memoria.Debugger
{
    internal sealed class SafeVirtualMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private readonly IntPtr _processHandle;
        private readonly Int64 _size;

        public SafeVirtualMemoryHandle(IntPtr processHandle, Int64 address, Int64 size, AllocationType allocation, MemoryProtection protection)
            : base(true)
        {
            _processHandle = processHandle;
            _size = size;
            handle = Kernel32.VirtualAllocEx(processHandle, (IntPtr)address, (IntPtr)_size, allocation, protection);
            if (IsInvalid)
                throw new Win32Exception();
        }

        protected override Boolean ReleaseHandle()
        {
            return Kernel32.VirtualFreeEx(_processHandle, handle, (IntPtr)_size, FreeType.Release);
        }

        public void Write(Byte[] buffer)
        {
            if (buffer.Length > _size)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            if (!Kernel32.WriteProcessMemory(_processHandle, handle, buffer, (IntPtr)buffer.Length, IntPtr.Zero))
                throw new Win32Exception();
        }
    }
}