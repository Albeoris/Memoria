using System;
using System.IO;
using UnityEngine;
using Object = System.Object;

namespace Memoria.Test
{
    public sealed class RectMessage : IValueMessage
    {
        public ValueMessageType ValueType => ValueMessageType.Rect;

        ValueType IValueMessage.Object => Value;

        public Rect Value;

        public RectMessage(Rect value)
        {
            Value = value;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Value);
        }

        public void Deserialize(BinaryReader br)
        {
            Value = br.ReadRect();
        }
    }
}