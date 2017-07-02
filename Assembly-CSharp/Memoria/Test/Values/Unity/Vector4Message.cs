using System;
using System.IO;
using UnityEngine;
using Object = System.Object;

namespace Memoria.Test
{
    public sealed class Vector4Message : IValueMessage
    {
        public ValueMessageType ValueType => ValueMessageType.Vector4;

        ValueType IValueMessage.Object => Value;

        public Vector4 Value;

        public Vector4Message(Vector4 value)
        {
            Value = value;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Value);
        }

        public void Deserialize(BinaryReader br)
        {
            Value = br.ReadVector4();
        }
    }
}