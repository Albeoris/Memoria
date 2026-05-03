using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Memoria.Speedrun
{
    public static class AutoSplitterPipe
    {
        // Let the game communicate with LiveSplit through a fifo pipe
        // Because the game's framework is rather old, System.Core.dll doesn't provide System.IO.Pipes and we thus must use another API
        // Also, for some reason, sending messages to LiveSplit works more or less correctly but receiving its answers doesn't work (even when setting up an asynchronous transmission)
        // LiveSplit's server is set up there: https://github.com/LiveSplit/LiveSplit/blob/master/LiveSplit/LiveSplit.Core/Server/CommandServer.cs

        public static void SignalGameStart()
        {
            if (!Configuration.Speedrun.IsEnabled)
                return;
            AutoSplitterPipe.SendMessageToLiveSplit("reset");
            AutoSplitterPipe.SendMessageToLiveSplit("start");
            SpeedrunSettings.CurrentSplitIndex = 0;
        }

        public static void SignalGameResume()
        {
            if (!Configuration.Speedrun.IsEnabled)
                return;
            SpeedrunSettings.CurrentSplitIndex = -2;
            RequestCurrentSplitIndex(index =>
            {
                if (index >= 0 && index < SpeedrunSettings.Splits.Count)
                    SpeedrunSettings.CurrentSplitIndex = index;
            });
        }

        public static void SignalFieldChange()
        {
            SignalTrigger(Split.TriggerType.FIELD_TRANSITION);
        }

        public static void SignalBattleWin()
        {
            SignalTrigger(Split.TriggerType.BATTLE_WIN);
        }

        public static void SignalBattleStop()
        {
            SignalTrigger(Split.TriggerType.BATTLE_STOP);
        }

        public static void SignalBattleEnd()
        {
            SignalTrigger(Split.TriggerType.BATTLE_END);
        }

        public static void SignalLoadStart()
        {
            AutoSplitterPipe.SendMessageToLiveSplit("alwayspausegametime");
        }

        public static void SignalLoadEnd()
        {
            AutoSplitterPipe.SendMessageToLiveSplit("unpausegametime");
        }

        public static void RequestCurrentSplitIndex(Action<Int32> callback)
        {
            SendMessageToLiveSplit("getsplitindex", str =>
            {
                if (Int32.TryParse(str, out Int32 index))
                    callback(index);
            });
        }

        private static void SignalTrigger(Split.TriggerType trigger)
        {
            if (!Configuration.Speedrun.IsEnabled)
                return;
            if (SpeedrunSettings.CurrentSplitIndex == -2)
            {
                for (Int32 i = 0; i < SpeedrunSettings.Splits.Count; i++)
                {
                    if (SpeedrunSettings.Splits[i].IsConditionFulfilled(trigger))
                    {
                        SpeedrunSettings.CurrentSplitIndex = i;
                        SplitOnce();
                        return;
                    }
                }
            }
            else
            {
                if (SpeedrunSettings.CurrentSplitIndex < 0 || SpeedrunSettings.CurrentSplitIndex >= SpeedrunSettings.Splits.Count)
                    return;
                Split currentSplit = SpeedrunSettings.Splits[SpeedrunSettings.CurrentSplitIndex];
                if (currentSplit.IsConditionFulfilled(trigger))
                    SplitOnce();
            }
        }

        private static void SplitOnce()
        {
            if (!Configuration.Speedrun.IsEnabled)
                return;
            SpeedrunSettings.CurrentSplitIndex++;
            if (SpeedrunSettings.CurrentSplitIndex >= SpeedrunSettings.Splits.Count)
                SpeedrunSettings.CurrentSplitIndex = -1;
            AutoSplitterPipe.SendMessageToLiveSplit("split");
        }

        private static void SendMessageToLiveSplit(String message, Action<String> answerCallback = null)
        {
            if (!Configuration.Speedrun.IsEnabled)
                return;
            Thread messageThread = new Thread(() =>
            {
                try
                {
                    IntPtr pipe = CreateFile(@"\\.\pipe\LiveSplit", GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_NO_BUFFERING | FILE_FLAG_WRITE_THROUGH, IntPtr.Zero);
                    if (pipe.ToInt32() == -1)
                        return;
                    IntPtr buffer = Marshal.AllocHGlobal(message.Length);
                    Byte[] charStr = Encoding.ASCII.GetBytes(message);
                    Marshal.Copy(charStr, 0, buffer, charStr.Length);
                    Boolean success = WriteFile(pipe, buffer, (UInt32)charStr.Length, out UInt32 bufferWritten, IntPtr.Zero);

                    // Not currently working (it waits indefinitely for ReadFile to return, blocking the thread)
                    //if (answerCallback != null)
                    //{
                    //	Thread.Sleep(10);
                    //	IntPtr inBuffer = Marshal.AllocHGlobal(4096);
                    //	Log.Message($"[Split] request {message}");
                    //	ReadFile(pipe, inBuffer, 4096, out UInt32 bufferRead, IntPtr.Zero);
                    //	Byte[] answerStr = new Byte[bufferRead];
                    //	Marshal.Copy(inBuffer, answerStr, 0, (Int32)bufferRead);
                    //	String answer = Encoding.ASCII.GetString(answerStr);
                    //	Log.Message($"[Split] answer = {answer}");
                    //	answerCallback(answer);
                    //	Marshal.FreeHGlobal(inBuffer);
                    //}

                    // For some reason (maybe because C methods close the handle without making sure LiveSplit's server intercepted the message before),
                    // there's a loss of ~10% of message sent if we don't wait a bit there
                    Thread.Sleep(10);
                    CloseHandle(pipe);
                    Marshal.FreeHGlobal(buffer);
                }
                catch (Exception err)
                {
                    Log.Error(err);
                }
                finally
                {
                    // Starting the threads one by one is done to make sure the signal order is preserved... surely this could be done better
                    MessageThreads.Remove(Thread.CurrentThread);
                    if (MessageThreads.Count > 0)
                        MessageThreads[0].Start();
                }
            });
            MessageThreads.Add(messageThread);
            if (MessageThreads.Count <= 1)
                messageThread.Start();
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);
        [DllImport("kernel32", SetLastError = true)]
        private static extern Boolean WriteFile(IntPtr hFile, IntPtr lpBuffer, UInt32 nNumberOfBytesToWrite, out UInt32 lpNumberOfBytesWritten, IntPtr lpOverlapped);
        [DllImport("kernel32", SetLastError = true)]
        private static extern Boolean ReadFile(IntPtr hFile, IntPtr lpBuffer, UInt32 nNumberOfBytesToRead, out UInt32 lpNumberOfBytesRead, IntPtr lpOverlapped);
        [DllImport("kernel32", SetLastError = true)]
        private static extern Boolean CloseHandle(IntPtr hObject);
        [DllImport("kernel32", SetLastError = true)]
        private static extern Boolean ConnectNamedPipe(IntPtr hNamedPipe, IntPtr lpOverlapped);
        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr CreateEvent(IntPtr lpEventAttributes, Boolean bManualReset, Boolean bInitialState, String lpName);
        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr CreateNamedPipe(String lpName, UInt32 dwOpenMode, UInt32 dwPipeMode, UInt32 nMaxInstances, UInt32 nOutBufferSize, UInt32 nInBufferSize, UInt32 nDefaultTimeOut, IntPtr lpSecurityAttributes);
        [DllImport("kernel32", SetLastError = true)]
        private static extern Boolean CallNamedPipe(String lpNamedPipeName, IntPtr lpInBuffer, UInt32 nInBufferSize, IntPtr lpOutBuffer, UInt32 nOutBufferSize, IntPtr lpBytesRead, UInt32 nTimeOut);
        [DllImport("kernel32", SetLastError = true)]
        private static extern unsafe Boolean SetNamedPipeHandleState(String hNamedPipe, UInt32* lpMode, UInt32* lpMaxCollectionCount, UInt32* lpCollectDataTimeout);

        private static List<Thread> MessageThreads = new List<Thread>();

        private const UInt32 GENERIC_READ = 0x80000000;
        private const UInt32 GENERIC_WRITE = 0x40000000;
        private const UInt32 FILE_SHARE_READ = 1;
        private const UInt32 FILE_SHARE_WRITE = 2;
        private const UInt32 OPEN_EXISTING = 3;
        private const UInt32 FILE_ATTRIBUTE_NORMAL = 0x80;
        private const UInt32 PIPE_READMODE_BYTE = 0;
        private const UInt32 PIPE_READMODE_MESSAGE = 2;
        private const UInt32 NMPWAIT_NOWAIT = 1;
        private const UInt32 NMPWAIT_WAIT_FOREVER = 0xFFFFFFFF;
        private const UInt32 PIPE_ACCESS_INBOUND = 1;
        private const UInt32 PIPE_ACCESS_OUTBOUND = 2;
        private const UInt32 PIPE_ACCESS_DUPLEX = 3;
        private const UInt32 FILE_FLAG_NO_BUFFERING = 0x20000000;
        private const UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;
        private const UInt32 FILE_FLAG_WRITE_THROUGH = 0x80000000;
        private const UInt32 PIPE_WAIT = 0;
        private const UInt32 PIPE_TYPE_BYTE = 0;

        private static IntPtr StructToPtr(Object obj)
        {
            IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
            Marshal.StructureToPtr(obj, intPtr, false);
            return intPtr;
        }

        [StructLayout(LayoutKind.Explicit, Size = 20)]
        private struct OVERLAPPED
        {
            [FieldOffset(0)]
            public UInt32 Internal;
            [FieldOffset(4)]
            public UInt32 InternalHigh;
            [FieldOffset(8)]
            public UInt32 Offset;
            [FieldOffset(12)]
            public UInt32 OffsetHigh;
            [FieldOffset(8)]
            public IntPtr Offsets;
            [FieldOffset(16)]
            public IntPtr hEvent;
        }

        /* Old trials and errors

		private static void CreatePipe()
		{
			try
			{
				Memoria.Prime.Log.Message($"[Pipe] Start");
				IntPtr buffer = Marshal.AllocHGlobal(4096);
				IntPtr outBuffer = Marshal.AllocHGlobal(4096);
				IntPtr outLengthBuffer = Marshal.AllocHGlobal(4);
				String signal = "start";
				Byte[] charStr = Encoding.ASCII.GetBytes(signal);
				Memoria.Prime.Log.Message($"[Pipe] charStr length = {charStr.Length}");
				Memoria.Prime.Log.Message($"[Pipe] charStr = {String.Join(" | ", charStr.Select(b => b.ToString()).ToArray())}");
				Marshal.Copy(charStr, 0, buffer, charStr.Length);

				unsafe
				{
					UInt32 newMode = PIPE_READMODE_MESSAGE;
					Boolean changed = SetNamedPipeHandleState(@"\\.\pipe\LiveSplit", &newMode, null, null);
					Memoria.Prime.Log.Message($"[Pipe] SetNamedPipeHandleState: {changed}");
					Memoria.Prime.Log.Message($"[Pipe] last error = {Marshal.GetLastWin32Error()}");
				}
				Boolean success = CallNamedPipe(@"\\.\pipe\LiveSplit", buffer, (UInt32)charStr.Length, outBuffer, 4096, outLengthBuffer, NMPWAIT_WAIT_FOREVER);
				Memoria.Prime.Log.Message($"[Pipe] CallNamedPipe: {success}");
				Memoria.Prime.Log.Message($"[Pipe] last error = {Marshal.GetLastWin32Error()}");

				IntPtr pipe = CreateFile(@"\\.\pipe\LiveSplit", GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
				Memoria.Prime.Log.Message($"[Pipe] CreateFile: {pipe}");
				Memoria.Prime.Log.Message($"[Pipe] last error = {Marshal.GetLastWin32Error()}");
				Boolean flag = WriteFile(pipe, buffer, (UInt32)charStr.Length, out UInt32 bufferWritten, IntPtr.Zero);
				Memoria.Prime.Log.Message($"[Pipe] WriteFile: {flag}");
				Memoria.Prime.Log.Message($"[Pipe] bufferWritten: {bufferWritten}");
				Memoria.Prime.Log.Message($"[Pipe] last error = {Marshal.GetLastWin32Error()}");
				signal = "split";
				charStr = Encoding.ASCII.GetBytes(signal);
				Marshal.Copy(charStr, 0, buffer, charStr.Length);
				flag = WriteFile(pipe, buffer, (UInt32)charStr.Length, out bufferWritten, IntPtr.Zero);
				Memoria.Prime.Log.Message($"[Pipe] WriteFile: {flag}");
				Memoria.Prime.Log.Message($"[Pipe] last error = {Marshal.GetLastWin32Error()}");
				Boolean close = CloseHandle(pipe);
				Memoria.Prime.Log.Message($"[Pipe] CloseHandle: {close}");
				Memoria.Prime.Log.Message($"[Pipe] last error = {Marshal.GetLastWin32Error()}");

				Overlapped overlap = new Overlapped();
				OVERLAPPED overlap1 = new OVERLAPPED();
				Memoria.Prime.Log.Message($"[Pipe] Overlapped: {overlap1}");
				IntPtr overlapPtr = Marshal.GetIDispatchForObject(overlap);
				IntPtr overlapPtr1 = StructToPtr(overlap1);
				Memoria.Prime.Log.Message($"[Pipe] GetIDispatchForObject: {overlapPtr1}");
				overlap.EventHandleIntPtr = CreateEvent(IntPtr.Zero, false, true, null);
				overlap1.hEvent = CreateEvent(IntPtr.Zero, false, true, null);
				Memoria.Prime.Log.Message($"[Pipe] CreateEvent: {overlap1.hEvent}");
				new NamedPipeServerStream("LiveSplit", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
				IntPtr pipe = CreateNamedPipe(@"\\.\pipe\LiveSplit", PIPE_ACCESS_DUPLEX | FILE_FLAG_OVERLAPPED, PIPE_TYPE_BYTE | PIPE_WAIT, 0xFF, 4096, 4096, 100, IntPtr.Zero);
				Memoria.Prime.Log.Message($"[Pipe] CallNamedPipe: {pipe}");
				Memoria.Prime.Log.Message($"[Pipe] last error = {Marshal.GetLastWin32Error()}");
				Boolean isConnecting = true;
				while (true)
				{
					if (isConnecting)
					{
						isConnecting = !ConnectNamedPipe(pipe, overlapPtr1);
						Memoria.Prime.Log.Message($"[Pipe] ConnectNamedPipe: {!isConnecting}");
						Memoria.Prime.Log.Message($"[Pipe] last error = {Marshal.GetLastWin32Error()}");
					}
					else
					{
						Boolean flag = WriteFile(pipe, buffer, (UInt32)charStr.Length, out UInt32 bufferWritten, overlapPtr1);
						Memoria.Prime.Log.Message($"[Pipe] WriteFile: {flag}");
						Memoria.Prime.Log.Message($"[Pipe] last error = {Marshal.GetLastWin32Error()}");
						if (flag)
						{
							Memoria.Prime.Log.Message($"[Pipe] buffer: {bufferWritten}");
						}
					}
					Thread.Sleep(1000);
				}
				
				
				IntPtr pipe = CreateFile(@"\\.\pipe\LiveSplit", GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, IntPtr.Zero);
				if (pipe.ToInt32() == -1)
					return;
				AutoResetEvent stateChangeEvent = new AutoResetEvent(true);
				NativeOverlapped nativeOverlapped = new NativeOverlapped();
				nativeOverlapped.EventHandle = stateChangeEvent.SafeWaitHandle.DangerousGetHandle();
				IntPtr buffer = Marshal.AllocHGlobal(message.Length);
				Byte[] charStr = Encoding.ASCII.GetBytes(message);
				Marshal.Copy(charStr, 0, buffer, charStr.Length);
				unsafe
				{
					Boolean success = WriteFile(pipe, buffer, (UInt32)charStr.Length, null, &nativeOverlapped);
					while (!success)
					{
						Thread.Sleep(100);
						success = WriteFile(pipe, buffer, (UInt32)charStr.Length, null, &nativeOverlapped);
					}
				}
				CloseHandle(pipe);
				Marshal.FreeHGlobal(buffer);
			}
			catch (Exception err)
			{
				Memoria.Prime.Log.Error(err);
			}
		}
		*/
    }
}
