using System;
using System.IO;

namespace Memoria.Test
{
	public sealed class Int8Message : IValueMessage
	{
		public ValueMessageType ValueType => ValueMessageType.Int8;

		ValueType IValueMessage.Object => Value;

		public SByte Value;

		public Int8Message(SByte value)
		{
			Value = value;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(Value);
		}

		public void Deserialize(BinaryReader br)
		{
			Value = br.ReadSByte();
		}
	}
}
