using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Memoria.Client.Interaction;
using Memoria.Test;
using Memoria.Prime;

namespace Memoria.Client
{
    public static class NetworkClient
    {
        private static readonly DisposableStack Disposables = new DisposableStack();
        private static readonly ConcurrentQueue<CommandMessage> CommandQueue = new ConcurrentQueue<CommandMessage>();

        public static event Action Connected;
        public static event Action<String> Disconnecting;

        public static void Reconnect()
        {
            lock (Disposables)
            {
                Disconnect("Reconnect");

                try
                {
                    Connect();
                }
                catch (Exception ex)
                {
                    Disconnect(ex.ToString());
                }
            }
        }

        public static void Disconnect(String reason)
        {
            lock (Disposables)
            {
                InteractionService.RemoteGameObjects.ClearValue();
                Disconnecting?.Invoke(reason);
                Disposables.Dispose();
            }
        }

        //private sealed class MyStream2 : NetworkStream
        //{
        //    private static readonly FileStream _output = new FileStream("tempFile.bin", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

        //    public MyStream2([NotNull] Socket socket) : base(socket)
        //    {
        //    }

        //    public MyStream2([NotNull] Socket socket, Boolean ownsSocket) : base(socket, ownsSocket)
        //    {
        //    }

        //    public MyStream2([NotNull] Socket socket, FileAccess access) : base(socket, access)
        //    {
        //    }

        //    public MyStream2([NotNull] Socket socket, FileAccess access, Boolean ownsSocket) : base(socket, access, ownsSocket)
        //    {
        //    }

        //    public override Int32 Read(Byte[] buffer, Int32 offset, Int32 size)
        //    {
        //        var readed = base.Read(buffer, offset, size);
        //        _output.Write(buffer, offset, readed);
        //        _output.Flush();
        //        return readed;
        //    }

        //    public override Int32 ReadByte()
        //    {
        //        var b = base.ReadByte();
        //        if (b != -1)
        //        {
        //            _output.WriteByte(checked((Byte)b));
        //            _output.Flush();
        //        }
        //        return b;
        //    }
        //}

        private static void Connect()
        {
            lock (Disposables)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Disposables.Add(socket);

                IPEndPoint ip = new IPEndPoint(IPAddress.Loopback, 49021);
                socket.Connect(ip);

                //NetworkStream stream = new MyStream2(socket, FileAccess.Read, false);
                NetworkStream stream = new NetworkStream(socket, FileAccess.ReadWrite, false);
                Disposables.Add(stream);

                BinaryReader br = new BinaryReader(stream, Encoding.UTF8, true);
                //BinaryReader br = new CrossPlatformBinaryReader(stream, Encoding.UTF8);
                Disposables.Add(br);

                BinaryWriter bw = new BinaryWriter(stream, Encoding.UTF8, true);
                Disposables.Add(bw);

                Thread readingThread = new Thread(() =>
                {
                    try
                    {
                        ProcessInput(br);
                    }
                    catch (Exception ex)
                    {
                        Disconnect(ex.ToString());
                    }
                }) {IsBackground = true, Name = "NetworkReadingThread"};
                readingThread.Start();

                Thread writingThread = new Thread(() =>
                {
                    try
                    {
                        ProcessOutput(bw);
                    }
                    catch (Exception ex)
                    {
                        Disconnect(ex.ToString());
                    }
                })
                { IsBackground = true, Name = "NetworkWritingThread" };
                writingThread.Start();

                InteractionService.RemoteGameObjects.Provide();
                Connected?.Invoke();
            }
        }

        private static void ProcessInput(BinaryReader br)
        {
            while (true)
            {
                RemotingMessageType type = br.ReadRemotingMessageType();
                switch (type)
                {
                    case RemotingMessageType.GameObject:
                        RemoteGameObjects remoteObjects = InteractionService.RemoteGameObjects.Provide();
                        remoteObjects.ProcessGameObjectMessages(br);
                        break;
                    default:
                        throw new InvalidDataException();
                }
            }
        }

        private static void ProcessOutput(BinaryWriter bw)
        {
            while (true)
            {
                CommandMessage command;
                if (CommandQueue.TryDequeue(out command))
                {
                    Log.Message("Executing command: {0}", command.MessageType);
                    bw.Write(RemotingMessageType.Command);
                    bw.Write(command.MessageType);
                    command.Serialize(bw);
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        internal static void Execute(CommandMessage commandMessage)
        {
            Log.Message("Enqueu new command: {0}", commandMessage.MessageType);
            CommandQueue.Enqueue(commandMessage);
        }
    }
}