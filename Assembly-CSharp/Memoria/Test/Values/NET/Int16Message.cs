using System;
using System.IO;

namespace Memoria.Test
{
	public sealed class Int16Message : IValueMessage
	{
		public ValueMessageType ValueType => ValueMessageType.Int16;

		ValueType IValueMessage.Object => Value;

		public Int16 Value;

		public Int16Message(Int16 value)
		{
			Value = value;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(Value);
		}

		public void Deserialize(BinaryReader br)
		{
			Value = br.ReadInt16();
		}
	}
}
