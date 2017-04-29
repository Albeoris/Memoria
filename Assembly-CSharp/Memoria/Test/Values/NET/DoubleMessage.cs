using System;
using System.IO;

namespace Memoria.Test
{
    public sealed class DoubleMessage : IValueMessage
    {
        public ValueMessageType ValueType => ValueMessageType.Double;

        Object IValueMessage.Object => Value;

        public Double Value;

        public DoubleMessage(Double value)
        {
            Value = value;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Value);
        }

        public void Deserialize(BinaryReader br)
        {
            Value = br.ReadDouble();
        }
    }
}