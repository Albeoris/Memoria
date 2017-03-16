using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Memoria.Test;
using UnityEngine;

namespace Memoria.Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //new Player();
            //new Client();
        }
    }

    public sealed class Player
    {
        public Player()
        {
            Method();
        }

        [HandleProcessCorruptedStateExceptions()]
        private static void Method()
        {
            while (true)
            {
                var instance = new SdLibAPIWithProLicense();
                ISdLibAPIProxy.Instance = instance;
                Int32 num = instance.SdSoundSystem_Create(String.Empty);
                if (num < 0)
                    return;
                
                var musicPlayer = new MusicPlayer();
                var soundPlayerList = new List<SoundPlayer>();
                soundPlayerList.Add(musicPlayer);

                SoundProfile soundProfile = new SoundProfile();
                soundProfile.Code = 1.ToString();
                soundProfile.Name = "music033";
                soundProfile.SoundIndex = 1;
                soundProfile.ResourceID = "Sounds01/BGM_/music033.ogg";
                soundProfile.SoundProfileType = SoundProfileType.Music;


                String path = @"W:\Steam\steamapps\common\FINAL FANTASY IX\StreamingAssets\Sounds\Sounds01\BGM_\music033";
                DateTime writeTime = File.GetLastWriteTimeUtc(path);

                Byte[] array = File.ReadAllBytes(path);

                IntPtr intPtr = Marshal.AllocHGlobal((Int32)array.Length);
                Marshal.Copy(array, 0, intPtr, (Int32)array.Length);
                Int32 bankID = instance.SdSoundSystem_AddData(intPtr);
                soundProfile.AkbBin = intPtr;
                soundProfile.BankID = bankID;

                try
                {
                    musicPlayer.soundDatabase.Create(soundProfile);
                    musicPlayer.PlayMusic(soundProfile, 9);

                    while (File.GetLastWriteTimeUtc(path) == writeTime)
                        Thread.Sleep(1000);

                    instance.SdSoundSystem_RemoveData(bankID);
                    Marshal.FreeHGlobal(intPtr);
                }
                catch
                {
                    instance.SdSoundSystem_RemoveData(bankID);
                    Marshal.FreeHGlobal(intPtr);

                    while (File.GetLastWriteTimeUtc(path) == writeTime)
                        Thread.Sleep(1000);
                }
                finally
                {
                }
            }
        }
    }

    public sealed class Client
    {
        private readonly MessageFactory _messageFactory = new MessageFactory();
        private readonly Dictionary<Int32, IRemotingMessage> _messages = new Dictionary<Int32, IRemotingMessage>();

        public Client()
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Loopback, 49021);
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(ip);

                using (NetworkStream stream = new NetworkStream(socket, FileAccess.Read, false))
                using (BinaryReader br = new BinaryReader(stream))
                {
                    RemotingMessageType type = br.ReadRemotingMessageType();
                    if (type != RemotingMessageType.GameObject)
                        throw new InvalidDataException();

                    Int32 objectsCount = br.ReadInt32();
                    GameObjectMessage[] objectMessages = new GameObjectMessage[objectsCount];
                    for (Int32 o = 0; o < objectsCount; o++)
                    {
                        GameObjectMessage objectMessage = new GameObjectMessage();
                        objectMessage.Deserialize(br);
                        objectMessages[o] = objectMessage;

                        _messages[objectMessage.InstanceId] = objectMessage;

                        Int32 componentCount = br.ReadInt32();
                        ComponentMessage[] componentMessages = new ComponentMessage[componentCount];
                        for (Int32 c = 0; c < componentCount; c++)
                        {
                            Int32 componentIndex = br.ReadInt32();
                            ComponentMessage componentMessage = _messageFactory.CreateComponentMessage(componentIndex);
                            componentMessage.Deserialize(br);
                            componentMessages[c] = componentMessage;

                            _messages[componentMessage.InstanceId] = componentMessage;
                        }
                    }
                }
            }

            Console.WriteLine(_messages);
        }
    }
}
