using System;
using System.IO;
using UnityEngine;
using Object = System.Object;

namespace Memoria.Test
{
    public sealed class Vector3Message : IValueMessage
    {
        public ValueMessageType ValueType => ValueMessageType.Vector3;

        ValueType IValueMessage.Object => Value;

        public Vector3 Value;

        public Vector3Message(Vector3 value)
        {
            Value = value;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Value);
        }

        public void Deserialize(BinaryReader br)
        {
            Value = br.ReadVector3();
        }
    }
}