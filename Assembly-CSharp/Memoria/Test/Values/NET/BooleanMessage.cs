using System;
using System.IO;

namespace Memoria.Test
{
    public sealed class BooleanMessage : IValueMessage
    {
        public ValueMessageType ValueType => ValueMessageType.Boolean;

        Object IValueMessage.Object => Value;

        public Boolean Value;

        public BooleanMessage(Boolean value)
        {
            Value = value;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Value);
        }

        public void Deserialize(BinaryReader br)
        {
            Value = br.ReadBoolean();
        }
    }
}