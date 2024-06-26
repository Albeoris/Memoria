using System;
using System.IO;
using UnityEngine;

namespace Memoria.Test
{
    public static partial class ExtensionMethodsBinaryReader
    {
        public static Vector2 ReadVector2(this BinaryReader br)
        {
            return new Vector2(
                br.ReadSingle(),
                br.ReadSingle());
        }

        public static Vector3 ReadVector3(this BinaryReader br)
        {
            return new Vector3(
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle());
        }

        public static Vector4 ReadVector4(this BinaryReader br)
        {
            return new Vector4(
                br.ReadSingle(),
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

        public static Rect ReadRect(this BinaryReader br)
        {
            return new Rect(
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle());
        }

        public static Matrix4x4 ReadMatrix4x4(this BinaryReader br)
        {
            Matrix4x4 result = new Matrix4x4();
            for (Int32 i = 0; i < 4 * 4; i++)
                result[i] = br.ReadInt32();
            return result;
        }

        public static RemotingMessageType ReadRemotingMessageType(this BinaryReader br)
        {
            return (RemotingMessageType)br.ReadUInt16();
        }

        public static CommandMessageType ReadCommandMessageType(this BinaryReader br)
        {
            return (CommandMessageType)br.ReadUInt16();
        }

        public static ValueMessageType ReadValueMessageType(this BinaryReader br)
        {
            return (ValueMessageType)br.ReadUInt16();
        }

        public static ReferenceMessageType ReadReferenceMessageType(this BinaryReader br)
        {
            return (ReferenceMessageType)br.ReadUInt16();
        }
    }
}