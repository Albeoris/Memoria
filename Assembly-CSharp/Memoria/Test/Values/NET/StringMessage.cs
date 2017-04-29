using System;
using System.IO;

namespace Memoria.Test
{
    public sealed class StringMessage : IValueMessage
    {
        public ValueMessageType ValueType => ValueMessageType.String;

        Object IValueMessage.Object => Value;

        public String Value;

        public StringMessage(String value)
        {
            Value = value;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Value);
        }

        public void Deserialize(BinaryReader br)
        {
            Value = br.ReadString();
        }
    }
}