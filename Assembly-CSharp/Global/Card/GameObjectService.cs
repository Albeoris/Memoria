using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
                    r.WaitSafe();
                    w.WaitSafe();
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
                using (NetworkStream stream = new NetworkStream(socket, FileAccess.Write, false))
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    List<Component> components = new List<Component>();

                    do
                    {
                        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
                        bw.Write(GameObjectMessage.Type);
                        bw.Write(objects.Length);

                        foreach (GameObject obj in objects)
                        {
                            GameObjectMessage objectMessage = MessageFactory.CreateGameObjectMessage(obj);
                            objectMessage.Serialize(bw);

                            obj.GetComponents<Component>(components);
                            bw.Write(components.Count);
                            foreach (Component component in components)
                            {
                                ComponentMessage componentMessage = MessageFactory.CreateComponentMessage(component);
                                bw.Write(componentMessage.ComponentIndex);
                                componentMessage.Serialize(bw);
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
                socket.Dispose();
            }
        }

        private static void ReadData(Socket socket)
        {
            try
            {
                using (NetworkStream stream = new NetworkStream(socket, FileAccess.Read, false))
                {
                    Byte[] buffer = new Byte[4096];

                    do
                    {
                        stream.Read(buffer, 0, buffer.Length);
                        // TODO

                    } while (socket.Connected);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to read data from the remote channel.");
                socket.Dispose();
            }
        }
    }

    public interface IRemotingMessage
    {
        void Serialize(BinaryWriter bw);
        void Deserialize(BinaryReader br);
    }

    public sealed class MessageFactory
    {
        public GameObjectMessage CreateGameObjectMessage(GameObject obj)
        {
            return new GameObjectMessage(obj);
        }

        public ScriptableObjectMessage CreateScriptableObjectMessage(ScriptableObject obj)
        {
            return new ScriptableObjectMessage(obj);
        }

        public ComponentMessage CreateComponentMessage(Component obj)
        {
            Type type = obj.GetType();

            Int32 index;
            ComponentFactoryWrapDelegate wrapper;
            if (ComponentFactories.TryGetWrapper(type, out index, out wrapper))
            {
                ComponentMessage result = wrapper(obj);
                result.ComponentIndex = index;
                return result;
            }

            List<Type> types = new List<Type>();
            do
            {
                types.Add(type);
                type = type.BaseType;

                if (type == null)
                    throw new InvalidOperationException("Something went wrong.");

            } while (!ComponentFactories.TryGetIndex(type, out index));

            foreach (Type t in types)
                ComponentFactories.Link(t, index);

            wrapper = ComponentFactories.GetWrapper(index);
            return wrapper(obj);
        }

        public ComponentMessage CreateComponentMessage(Int32 index)
        {
            ComponentFactoryCreateDelegate creator;
            if (ComponentFactories.TryGetCreator(index, out creator))
            {
                ComponentMessage result = creator();
                result.ComponentIndex = index;
                return result;
            }

            return null;
        }

        private static readonly ComponentFactories ComponentFactories = new ComponentFactories()
        {
            {typeof(Component), () => new ComponentMessage(), obj => new ComponentMessage(obj)},
            {typeof(Transform), () => new TransformMessage(), obj => new TransformMessage((Transform)obj)}
        };
    }

    internal delegate ComponentMessage ComponentFactoryCreateDelegate();

    internal delegate ComponentMessage ComponentFactoryWrapDelegate(Component obj);

    internal sealed class ComponentFactories : IEnumerable
    {
        private readonly List<ComponentFactoryCreateDelegate> _creators = new List<ComponentFactoryCreateDelegate>();
        private readonly List<ComponentFactoryWrapDelegate> _wrappers = new List<ComponentFactoryWrapDelegate>();
        private readonly Dictionary<Type, Int32> _map = new Dictionary<Type, Int32>();

        public void Add(Type type, ComponentFactoryCreateDelegate createFactory, ComponentFactoryWrapDelegate wrapFactory)
        {
            Int32 index = _creators.Count;
            _creators.Add(createFactory);
            _wrappers.Add(wrapFactory);
            _map.Add(type, index);
        }

        public void Link(Type type, Int32 index)
        {
            _map.Add(type, index);
        }

        public Boolean TryGetIndex(Type type, out Int32 index)
        {
            return _map.TryGetValue(type, out index);
        }

        public Boolean TryGetCreator(Int32 index, out ComponentFactoryCreateDelegate factory)
        {
            if (index >= 0 && index < _creators.Count)
            {
                factory = _creators[index];
                return true;
            }

            factory = null;
            return false;
        }

        public Boolean TryGetWrapper(Type type, out Int32 index, out ComponentFactoryWrapDelegate factory)
        {
            if (_map.TryGetValue(type, out index))
            {
                factory = _wrappers[index];
                return true;
            }

            factory = null;
            return false;
        }

        public ComponentFactoryWrapDelegate GetWrapper(Int32 index)
        {
            return _wrappers[index];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _map.Keys.GetEnumerator();
        }
    }

    public abstract class ObjectMessage : IRemotingMessage
    {
        public Int32 InstanceId;
        public String Name;
        public HideFlags HideFlags;

        public ObjectMessage()
        {
        }

        public ObjectMessage(UnityEngine.Object obj)
        {
            InstanceId = obj.GetInstanceID();
            Name = obj.name;
            HideFlags = obj.hideFlags;
        }

        public virtual void Serialize(BinaryWriter bw)
        {
            bw.Write(InstanceId);
            bw.Write(Name);
            bw.Write((Int32)HideFlags);
        }

        public virtual void Deserialize(BinaryReader br)
        {
            InstanceId = br.ReadInt32();
            Name = br.ReadString();
            HideFlags = (HideFlags)br.ReadInt32();
        }
    }

    public sealed class GameObjectMessage : ObjectMessage
    {
        public const RemotingMessageType Type = RemotingMessageType.GameObject;

        public String Tag;
        public Boolean IsActive;
        public Boolean IsActiveInHierarchy;

        public GameObjectMessage()
        {
        }

        public GameObjectMessage(GameObject gameObject)
            : base(gameObject)
        {
            Tag = gameObject.tag;
            IsActive = gameObject.activeSelf;
            IsActiveInHierarchy = gameObject.activeInHierarchy;
        }

        public override void Serialize(BinaryWriter bw)
        {
            base.Serialize(bw);

            bw.Write(Tag);
            bw.Write(IsActive);
            bw.Write(IsActiveInHierarchy);
        }

        public override void Deserialize(BinaryReader br)
        {
            base.Deserialize(br);

            Tag = br.ReadString();
            IsActive = br.ReadBoolean();
            IsActiveInHierarchy = br.ReadBoolean();
        }
    }

    public class ScriptableObjectMessage : ObjectMessage
    {
        public const RemotingMessageType Type = RemotingMessageType.ScriptableObject;

        public ScriptableObjectMessage()
        {
        }

        public ScriptableObjectMessage(ScriptableObject scriptableObject)
            : base(scriptableObject)
        {
        }
    }

    public enum RemotingMessageType
    {
        GameObject = 1,
        ScriptableObject = 2,
        Component = 3
    }

    public class ComponentMessage : ObjectMessage
    {
        public const RemotingMessageType Type = RemotingMessageType.Component;

        public Int32 ComponentIndex { get; set; }

        public ComponentMessage()
        {
        }

        public ComponentMessage(Component component)
            : base(component)
        {
        }
    }

    public class TransformMessage : ComponentMessage
    {
        public Vector3 LocalPosition;
        public Vector3 LocalScale;
        public Vector3 LocalEulerAngles;
        public Quaternion LocalRotation;

        public TransformMessage()
        {
        }

        public TransformMessage(Transform transform)
            : base(transform)
        {
            LocalPosition = transform.localPosition;
            LocalEulerAngles = transform.localEulerAngles;
            LocalRotation = transform.localRotation;
            LocalScale = transform.localScale;
        }

        public override void Serialize(BinaryWriter bw)
        {
            base.Serialize(bw);

            bw.Write(LocalPosition);
            bw.Write(LocalScale);
            bw.Write(LocalEulerAngles);
            bw.Write(LocalRotation);
        }

        public override void Deserialize(BinaryReader br)
        {
            base.Deserialize(br);

            LocalPosition = br.ReadVector3();
            LocalScale = br.ReadVector3();
            LocalEulerAngles = br.ReadVector3();
            LocalRotation = br.ReadQuaternion();
        }
    }

    public static class ExtensionMethodsBinaryWriter
    {
        public static void Write(this BinaryWriter bw, Vector3 value)
        {
            bw.Write(value.x);
            bw.Write(value.y);
            bw.Write(value.z);
        }

        public static void Write(this BinaryWriter bw, Quaternion value)
        {
            bw.Write(value.x);
            bw.Write(value.y);
            bw.Write(value.z);
            bw.Write(value.w);
        }

        public static void Write(this BinaryWriter bw, RemotingMessageType messageType)
        {
            bw.Write((Int32)messageType);
        }
    }

    public static class ExtensionMethodsBinaryReader
    {
        public static Vector3 ReadVector3(this BinaryReader br)
        {
            return new Vector3(
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle());
        }

        public static Quaternion ReadQuaternion(this BinaryReader br)
        {
            return new Quaternion(
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle());
        }

        public static RemotingMessageType ReadRemotingMessageType(this BinaryReader br)
        {
            return (RemotingMessageType)br.ReadInt32();
        }
    }
}