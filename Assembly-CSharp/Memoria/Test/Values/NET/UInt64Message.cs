using System;
using System.IO;

namespace Memoria.Test
{
    public sealed class UInt64Message : IValueMessage
    {
        public ValueMessageType ValueType => ValueMessageType.UInt64;

        ValueType IValueMessage.Object => Value;

        public UInt64 Value;

        public UInt64Message(UInt64 value)
        {
            Value = value;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Value);
        }

        public void Deserialize(BinaryReader br)
        {
            Value = br.ReadUInt64();
        }
    }
}