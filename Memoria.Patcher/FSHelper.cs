using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Memoria.Patcher
{
    public static class FsHelper
    {
        public static Boolean IsSamePaths(String path1, String path2)
        {
            if (String.Equals(path1, path2, StringComparison.OrdinalIgnoreCase))
                return true;

            using (SafeFileHandle
                handle1 = GetFsHandle(path1),
                handle2 = GetFsHandle(path2))
            {
                if (handle1.IsInvalid)
                    return false;
                if (handle2.IsInvalid)
                    return false;

                ByHandleFileInformation info1, info2;
                if (!GetFileInformationByHandle(handle1.DangerousGetHandle(), out info1))
                    return false;
                if (!GetFileInformationByHandle(handle2.DangerousGetHandle(), out info2))
                    return false;

                return info1.Equals(info2);
            }
        }

        private static SafeFileHandle GetFsHandle(String dirName)
        {
            const Int32 fileAccessNeither = 0;
            const Int32 fileShareRead = 1;
            const Int32 fileShareWrite = 2;
            const Int32 creationDispositionOpenExisting = 3;
            const Int32 fileFlagBackupSemantics = 0x02000000;
            return CreateFile(dirName, fileAccessNeither, fileShareRead | fileShareWrite, IntPtr.Zero, creationDispositionOpenExisting, fileFlagBackupSemantics, IntPtr.Zero);
        }

        private static ByHandleFileInformation? MY_GetFileInfo(SafeFileHandle directoryHandle)
        {
            ByHandleFileInformation objectFileInfo;
            if (!GetFileInformationByHandle(directoryHandle.DangerousGetHandle(), out objectFileInfo))
            {
                return null;
            }
            return objectFileInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ByHandleFileInformation
        {
            public UInt32 FileAttributes;
            public FileTime CreationTime;
            public FileTime LastAccessTime;
            public FileTime LastWriteTime;
            public UInt32 VolumeSerialNumber;
            public UInt32 FileSizeHigh;
            public UInt32 FileSizeLow;
            public UInt32 NumberOfLinks;
            public UInt32 FileIndexHigh;
            public UInt32 FileIndexLow;

            public Boolean Equals(ByHandleFileInformation other)
            {
                return FileIndexHigh == other.FileIndexHigh
                       && FileIndexLow == other.FileIndexLow
                       && VolumeSerialNumber == other.VolumeSerialNumber;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern Boolean GetFileInformationByHandle(IntPtr hFile, out ByHandleFileInformation lpFileInformation);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(String lpFileName, Int32 dwDesiredAccess, Int32 dwShareMode, IntPtr SecurityAttributes, Int32 dwCreationDisposition, Int32 dwFlagsAndAttributes, IntPtr hTemplateFile);
    }
}