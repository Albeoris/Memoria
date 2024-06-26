using System;
using System.IO;
using Object = System.Object;

namespace Memoria.Test
{
    public sealed class Int32Message : IValueMessage
    {
        public ValueMessageType ValueType => ValueMessageType.Int32;

        ValueType IValueMessage.Object => Value;

        public Int32 Value;

        public Int32Message(Int32 value)
        {
            Value = value;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Value);
        }

        public void Deserialize(BinaryReader br)
        {
            Value = br.ReadInt32();
        }
    }
}