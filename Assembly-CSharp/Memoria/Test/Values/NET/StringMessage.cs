using System;
using System.IO;

namespace Memoria.Test
{
    public sealed class StringMessage : IReferenceMessage
    {
        public ReferenceMessageType ReferenceType => ReferenceMessageType.String;

        Object IReferenceMessage.Object => Value;

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