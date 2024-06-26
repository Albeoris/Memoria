using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Memoria.Debugger
{
    internal static class Program
    {
        public static void Main(String[] args)
        {
            try
            {
                Byte[] unicodeDllPath = PrepareDllPath();
                TimeSpan timeout = GetTimeout(args);

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (o, s) =>
                {
                    Console.WriteLine();
                    Console.WriteLine("Stopping...");
                    cts.Cancel();
                };

                Task task = Task.Factory.StartNew(() => MainLoop(unicodeDllPath, cts, timeout), cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);

                Console.WriteLine("Waiting for an debug invitation.");
                Console.WriteLine("Type 'help' to show an documenantion or press Ctrl+C to exit.");

                while (!(cts.IsCancellationRequested || task.IsCompleted))
                {
                    Task<String> readLine = Task.Factory.StartNew(() => Console.ReadLine()?.ToLower(), cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                    readLine.Wait(cts.Token);

                    switch (readLine.Result)
                    {
                        case "help":
                            Console.WriteLine();
                            Console.WriteLine("help\t\t This message.");
                            Console.WriteLine("stop\t\t Stop waiting and close the application.");
                            break;
                        case "stop":
                            Console.WriteLine();
                            Console.WriteLine("Stopping...");
                            cts.Cancel();
                            task.Wait(cts.Token);
                            break;
                        default:
                            Console.WriteLine();
                            Console.WriteLine("Unrecognized command.");
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Unexpected error has occurred.");
                Console.WriteLine(ex);
                Console.WriteLine();
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
        }

        private static void MainLoop(Byte[] unicodeDllPath, CancellationTokenSource cts, TimeSpan timeout)
        {
            while (true)
            {
                cts.Token.ThrowIfCancellationRequested();

                try
                {
                    KeepAlive(cts, timeout);
                    WindowsObject window = WindowsObject.Wait("Debug", "You can attach a debugger now if you want", cts.Token);

                    Int32 processId = window.GetProcessId();

                    Console.WriteLine();
                    Console.WriteLine($"A new debuggable process [PID: {processId}] was found. Trying to inject DLL...");

                    using (SafeProcessHandle processHandle = new SafeProcessHandle(processId, ProcessAccessFlags.All, false))
                    using (SafeVirtualMemoryHandle memoryHandle = processHandle.Allocate(unicodeDllPath.Length, AllocationType.Commit, MemoryProtection.ReadWrite))
                    {
                        memoryHandle.Write(unicodeDllPath);

                        // Uncomment to debug
                        // System.Diagnostics.Debugger.Launch();
                        // KeepAlive(cts, TimeSpan.FromMinutes(10));

                        IntPtr loadLibraryAddress = GetLoadLibraryAddress();
                        using (SafeRemoteThread thread = processHandle.CreateThread(loadLibraryAddress, memoryHandle))
                        {
                            thread.Join();
                            window.Close();
                        }
                    }

                    KeepAlive(cts, timeout);
                    Console.WriteLine($"DLL was successfully injected to the process with PID: {processId}.");
                    return;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Faield to inject DLL.");
                    Console.WriteLine(ex);

                    cts.Token.ThrowIfCancellationRequested();
                    Console.WriteLine("Waiting 20 seconds to try again...");
                    Console.WriteLine("Press Ctrl+C to exit...");
                    Thread.Sleep(20 * 1000);
                }
            }
        }

        private static void KeepAlive(CancellationTokenSource cts, TimeSpan timeout)
        {
            if (timeout != TimeSpan.MaxValue)
                cts.CancelAfter(timeout);
        }

        private static Byte[] PrepareDllPath()
        {
            String dllPath = Path.GetFullPath("Memoria.Injection.dll");
            if (!File.Exists(dllPath))
                throw new FileNotFoundException("DLL not found: " + dllPath, dllPath);

            return Encoding.Unicode.GetBytes(dllPath);
        }

        private static TimeSpan GetTimeout(IReadOnlyList<String> args)
        {
            if (args.Count < 1)
                return TimeSpan.MaxValue;

            Int32 miliseconds;
            if (!Int32.TryParse(args[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out miliseconds))
                throw new InvalidDataException($"Failed to parse [{args[1]}] as integer.");

            return TimeSpan.FromMilliseconds(miliseconds);
        }

        private static IntPtr GetLoadLibraryAddress()
        {
            IntPtr kernelHandle = Kernel32.GetModuleHandle("kernel32.dll");
            if (kernelHandle == IntPtr.Zero)
                throw new Win32Exception();

            IntPtr loadLibraryAddress = Kernel32.GetProcAddress(kernelHandle, "LoadLibraryW");
            if (loadLibraryAddress == IntPtr.Zero)
                throw new Win32Exception();

            return loadLibraryAddress;
        }
    }
}
