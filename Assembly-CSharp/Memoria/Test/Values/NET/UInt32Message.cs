using System;
using System.IO;

namespace Memoria.Test
{
    public sealed class UInt32Message : IValueMessage
    {
        public ValueMessageType ValueType => ValueMessageType.UInt32;

        ValueType IValueMessage.Object => Value;

        public UInt32 Value;

        public UInt32Message(UInt32 value)
        {
            Value = value;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Value);
        }

        public void Deserialize(BinaryReader br)
        {
            Value = br.ReadUInt32();
        }
    }
}