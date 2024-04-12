using System;
using System.IO;

namespace Memoria.Test
{
	public sealed class UInt16Message : IValueMessage
	{
		public ValueMessageType ValueType => ValueMessageType.UInt16;

		ValueType IValueMessage.Object => Value;

		public UInt16 Value;

		public UInt16Message(UInt16 value)
		{
			Value = value;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(Value);
		}

		public void Deserialize(BinaryReader br)
		{
			Value = br.ReadUInt16();
		}
	}
}
