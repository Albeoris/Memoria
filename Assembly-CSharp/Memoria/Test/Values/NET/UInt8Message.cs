using System;
using System.IO;

namespace Memoria.Test
{
    public sealed class UInt8Message : IValueMessage
    {
        public ValueMessageType ValueType => ValueMessageType.UInt8;

        ValueType IValueMessage.Object => Value;

        public Byte Value;

        public UInt8Message(Byte value)
        {
            Value = value;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Value);
        }

        public void Deserialize(BinaryReader br)
        {
            Value = br.ReadByte();
        }
    }
}
