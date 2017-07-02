using System;
using System.IO;

namespace Memoria.Test
{
    public sealed class Int64Message : IValueMessage
    {
        public ValueMessageType ValueType => ValueMessageType.Int64;

        ValueType IValueMessage.Object => Value;

        public Int64 Value;

        public Int64Message(Int64 value)
        {
            Value = value;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Value);
        }

        public void Deserialize(BinaryReader br)
        {
            Value = br.ReadInt64();
        }
    }
}