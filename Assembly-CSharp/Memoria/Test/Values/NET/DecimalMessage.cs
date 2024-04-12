using System;
using System.IO;

namespace Memoria.Test
{
	public sealed class DecimalMessage : IValueMessage
	{
		public ValueMessageType ValueType => ValueMessageType.Decimal;

		ValueType IValueMessage.Object => Value;

		public Decimal Value;

		public DecimalMessage(Decimal value)
		{
			Value = value;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(Value);
		}

		public void Deserialize(BinaryReader br)
		{
			Value = br.ReadDecimal();
		}
	}
}
