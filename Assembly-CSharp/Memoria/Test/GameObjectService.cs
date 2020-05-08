using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Memoria.Prime;
using Memoria.Prime.Threading;
using UnityEngine;

namespace Memoria.Test
{
    public sealed class GameObjectService
    {
        public const UInt16 ListenerPort = 49021;

        private static TcpListener Listener;
        private static MessageFactory MessageFactory;

        public static void Start()
        {
            try
            {
                MessageFactory = new MessageFactory();

                IPEndPoint ip = new IPEndPoint(IPAddress.Loopback, ListenerPort);
                Listener = new TcpListener(ip);
                Listener.Start();
                Listener.BeginAcceptSocket(OnAcceptSocket, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to start listener.");
            }
        }

        private static void OnAcceptSocket(IAsyncResult ar)
        {
            try
            {
                using (Socket socket = TryEndAcceptSocket(ar))
                {
                    if (socket == null)
                        return;

                    Task w = Task.Run(WriteData, socket);
                    Task r = Task.Run(ReadData, socket);
                    r.WaitSafe(-1, true);
                    w.WaitSafe(-1, true);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to accept socket.");
            }
        }

        private static Socket TryEndAcceptSocket(IAsyncResult ar)
        {
            try
            {
                return Listener.EndAcceptSocket(ar);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to end accept socket.");
                return null;
            }
            finally
            {
                Listener.BeginAcceptTcpClient(OnAcceptSocket, null);
            }
        }

        private static void WriteData(Socket socket)
        {
            try
            {
                //using (NetworkStream stream = new MyStream(socket, FileAccess.Write, false))
                using (NetworkStream stream = new NetworkStream(socket, FileAccess.Write, false))
                //using (BinaryWriter bw = new CrossPlatformBinaryWriter(stream, Encoding.UTF8))
                using (BinaryWriter bw = new BinaryWriter(stream, Encoding.UTF8))
                {
                    List<Component> components = new List<Component>();

                    do
                    {
                        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
                        bw.Write(GameObjectMessage.Type);
                        bw.Write(objects.Length);

                        for (Int32 index = 0; index < objects.Length; index++)
                        {
                            GameObject obj = objects[index];
                            GameObjectMessage objectMessage = MessageFactory.CreateGameObjectMessage(obj);
                            objectMessage.OrderedNumber = index;
                            objectMessage.Serialize(bw);
                            //bw.Write(objectMessage.CheckField1); // 

                            obj.GetComponents(components);
                            bw.Write(components.Count);
                            foreach (Component component in components)
                            {
                                ComponentMessage componentMessage = MessageFactory.CreateComponentMessage(component);
                                bw.Write(componentMessage.ComponentIndex);
                                componentMessage.Serialize(bw);
                                //bw.Write(componentMessage.CheckField1); // 
                            }
                            components.Clear();
                        }

                        bw.Flush();

                        Thread.Sleep(10000);
                    } while (socket.Connected);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to write data to the remote channel.");
                IDisposable disposable = socket;
                disposable.Dispose();
            }
        }

        //private sealed class MyStream : NetworkStream
        //{
        //    private static readonly FileStream _input = new FileStream("sourceFile.bin", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        //    private static readonly StreamWriter _inputText = new StreamWriter(new FileStream("sourceFile.txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
        //    private static readonly FileStream _output = new FileStream("targetFile.bin", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

        //    public MyStream([NotNull] Socket socket) : base(socket)
        //    {
        //    }

        //    public MyStream([NotNull] Socket socket, Boolean ownsSocket) : base(socket, ownsSocket)
        //    {
        //    }

        //    public MyStream([NotNull] Socket socket, FileAccess access) : base(socket, access)
        //    {
        //    }

        //    public MyStream([NotNull] Socket socket, FileAccess access, Boolean ownsSocket) : base(socket, access, ownsSocket)
        //    {
        //    }

        //    public override void Write(Byte[] buffer, Int32 offset, Int32 size)
        //    {
        //        _inputText.WriteLine($"Position: {_input.Position}, Stack: {Environment.StackTrace}");
        //        _inputText.WriteLine("-------------------------------------------------------------");
        //        _input.Write(buffer, offset, size);
        //        base.Write(buffer, offset, size);
        //    }

        //    public override void WriteByte(Byte value)
        //    {
        //        _inputText.WriteLine($"Position: {_input.Position}, Value: {value} Stack: {Environment.StackTrace}");
        //        _inputText.WriteLine("-------------------------------------------------------------");
        //        _input.WriteByte(value);
        //        base.WriteByte(value);
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

        private static void ReadData(Socket socket)
        {
            try
            {
                using (NetworkStream stream = new NetworkStream(socket, FileAccess.Read, false))
                {
                    BinaryReader br = new BinaryReader(stream, Encoding.UTF8);

                    do
                    {
                        Log.Message("[GameObjectService] Waiting for a next message...");

                        RemotingMessageType type = br.ReadRemotingMessageType();
                        switch (type)
                        {
                            case RemotingMessageType.Command:
                                CommandMessageType commandType = br.ReadCommandMessageType();
                                Log.Message("[GameObjectService] Deserializing command of type {0}", commandType);
                                CommandMessage command = CreateCommand(commandType);
                                command.Deserialize(br);
                                Log.Message("[GameObjectService] Executing command {0}", commandType);
                                command.Execute();
                                break;
                            default:
                                throw new NotSupportedException(type.ToString());
                        }

                    } while (socket.Connected);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to read data from the remote channel.");
                IDisposable disposable = socket;
                disposable.Dispose();
            }
        }

        private static CommandMessage CreateCommand(CommandMessageType type)
        {
            switch (type)
            {
                case CommandMessageType.ChangeValue:
                    return new ChangeValueCommandMessage();
                case CommandMessageType.ChangeReference:
                    return new ChangeReferenceCommandMessage();
                case CommandMessageType.Duplicate:
                    return new DuplicateCommandMessage();
                default:
                    throw new NotSupportedException(type.ToString());
            }
        }
    }
}