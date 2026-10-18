using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class SystemD3DShaderAssembler : ID3DShaderAssembler
    {
        private const String CompilerLibraryName = "d3dcompiler_47.dll";
        private const UInt32 LoadLibrarySearchSystem32 = 0x00000800;
        private static readonly Lazy<SystemD3DShaderAssembler> SharedInstance = new(() => new SystemD3DShaderAssembler());

        private readonly SafeLibraryHandle _library;
        private readonly D3DAssembleDelegate _assemble;

        private SystemD3DShaderAssembler()
        {
            _library = LoadSystemCompiler();
            IntPtr address = GetProcAddress(_library, "D3DAssemble");
            if (address == IntPtr.Zero)
                throw new EntryPointNotFoundException($"The system component {CompilerLibraryName} does not export D3DAssemble.");
            _assemble = Marshal.GetDelegateForFunctionPointer<D3DAssembleDelegate>(address);
        }

        public static SystemD3DShaderAssembler Instance => SharedInstance.Value;

        public D3DShaderAssemblyResult Assemble(String source, String sourceName)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
            GCHandle sourceHandle = default;
            IntPtr shaderBlob = IntPtr.Zero;
            IntPtr errorBlob = IntPtr.Zero;
            try
            {
                sourceHandle = GCHandle.Alloc(sourceBytes, GCHandleType.Pinned);
                Int32 result = _assemble(sourceHandle.AddrOfPinnedObject(), new UIntPtr(checked((UInt64)sourceBytes.LongLength)), sourceName, IntPtr.Zero, IntPtr.Zero, 0, out shaderBlob, out errorBlob);
                String diagnostics = ReadBlob(errorBlob);
                if (result < 0 && String.IsNullOrWhiteSpace(diagnostics))
                    diagnostics = $"D3DAssemble failed with HRESULT 0x{result:X8}.";
                return new D3DShaderAssemblyResult(result >= 0, diagnostics);
            }
            finally
            {
                ReleaseBlob(errorBlob);
                ReleaseBlob(shaderBlob);
                if (sourceHandle.IsAllocated)
                    sourceHandle.Free();
            }
        }

        private static SafeLibraryHandle LoadSystemCompiler()
        {
            // d3dcompiler_47.dll is the final side-by-side D3DCompiler API version. LOAD_LIBRARY_SEARCH_SYSTEM32 is the documented secure way to select the OS copy without allowing the mod or launcher directories to participate in DLL resolution.
            String expectedPath = System.IO.Path.Combine(GetWindowsSystemDirectory(), CompilerLibraryName);
            SafeLibraryHandle library = LoadLibraryEx(CompilerLibraryName, IntPtr.Zero, LoadLibrarySearchSystem32);
            if (!library.IsInvalid && IsExpectedSystemModule(library, expectedPath))
                return library;

            // Windows 7 installations without KB2533623 do not understand LOAD_LIBRARY_SEARCH_SYSTEM32. An absolute path returned by GetSystemDirectory keeps the fallback equally constrained to the OS directory.
            library.Dispose();
            library = LoadLibraryEx(expectedPath, IntPtr.Zero, 0);
            if (!library.IsInvalid && IsExpectedSystemModule(library, expectedPath))
                return library;

            Int32 error = Marshal.GetLastWin32Error();
            Boolean unexpectedModule = !library.IsInvalid;
            library.Dispose();
            if (unexpectedModule)
                throw new SecurityException($"Windows redirected {CompilerLibraryName} outside the system directory; the module was rejected.");
            throw new DllNotFoundException($"Windows Direct3D compiler component {CompilerLibraryName} could not be loaded from the system directory. Win32 error {error}: {new Win32Exception(error).Message}");
        }

        private static Boolean IsExpectedSystemModule(SafeLibraryHandle library, String expectedPath)
        {
            StringBuilder buffer = new(512);
            while (true)
            {
                UInt32 length = GetModuleFileName(library.DangerousGetHandle(), buffer, checked((UInt32)buffer.Capacity));
                if (length == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "GetModuleFileName failed.");
                if (length < buffer.Capacity - 1)
                    return String.Equals(System.IO.Path.GetFullPath(buffer.ToString()), System.IO.Path.GetFullPath(expectedPath), StringComparison.OrdinalIgnoreCase);
                buffer.Capacity *= 2;
            }
        }

        private static String GetWindowsSystemDirectory()
        {
            StringBuilder buffer = new(260);
            UInt32 length = GetSystemDirectory(buffer, checked((UInt32)buffer.Capacity));
            if (length == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "GetSystemDirectory failed.");
            if (length < buffer.Capacity)
                return buffer.ToString();

            buffer = new StringBuilder(checked((Int32)length + 1));
            length = GetSystemDirectory(buffer, checked((UInt32)buffer.Capacity));
            if (length == 0 || length >= buffer.Capacity)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "GetSystemDirectory failed.");
            return buffer.ToString();
        }

        private static String ReadBlob(IntPtr blob)
        {
            if (blob == IntPtr.Zero)
                return String.Empty;

            IntPtr virtualTable = Marshal.ReadIntPtr(blob);
            GetBufferPointerDelegate getBufferPointer = Marshal.GetDelegateForFunctionPointer<GetBufferPointerDelegate>(Marshal.ReadIntPtr(virtualTable, IntPtr.Size * 3));
            GetBufferSizeDelegate getBufferSize = Marshal.GetDelegateForFunctionPointer<GetBufferSizeDelegate>(Marshal.ReadIntPtr(virtualTable, IntPtr.Size * 4));
            Int32 length = checked((Int32)getBufferSize(blob).ToUInt64());
            if (length == 0)
                return String.Empty;

            Byte[] bytes = new Byte[length];
            Marshal.Copy(getBufferPointer(blob), bytes, 0, length);
            Int32 textLength = Array.IndexOf(bytes, (Byte)0);
            if (textLength < 0)
                textLength = bytes.Length;
            return Encoding.UTF8.GetString(bytes, 0, textLength);
        }

        private static void ReleaseBlob(IntPtr blob)
        {
            if (blob == IntPtr.Zero)
                return;
            IntPtr virtualTable = Marshal.ReadIntPtr(blob);
            ReleaseDelegate release = Marshal.GetDelegateForFunctionPointer<ReleaseDelegate>(Marshal.ReadIntPtr(virtualTable, IntPtr.Size * 2));
            release(blob);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Int32 D3DAssembleDelegate(IntPtr sourceData, UIntPtr sourceDataSize, [MarshalAs(UnmanagedType.LPStr)] String sourceName, IntPtr defines, IntPtr include, UInt32 flags, out IntPtr shader, out IntPtr errorMessages);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 ReleaseDelegate(IntPtr instance);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr GetBufferPointerDelegate(IntPtr instance);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UIntPtr GetBufferSizeDelegate(IntPtr instance);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryExW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern SafeLibraryHandle LoadLibraryEx(String fileName, IntPtr file, UInt32 flags);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr GetProcAddress(SafeLibraryHandle module, String procedureName);

        [DllImport("kernel32.dll", EntryPoint = "GetSystemDirectoryW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern UInt32 GetSystemDirectory(StringBuilder buffer, UInt32 size);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleFileNameW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern UInt32 GetModuleFileName(IntPtr module, StringBuilder fileName, UInt32 size);

        private sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeLibraryHandle() : base(true)
            {
            }

            protected override Boolean ReleaseHandle() => FreeLibrary(handle);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern Boolean FreeLibrary(IntPtr module);
        }
    }
}
