using System;
using System.IO;

namespace Memoria.Test
{
	public sealed class SingleMessage : IValueMessage
	{
		public ValueMessageType ValueType => ValueMessageType.Single;

		ValueType IValueMessage.Object => Value;

		public Single Value;

		public SingleMessage(Single value)
		{
			Value = value;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(Value);
		}

		public void Deserialize(BinaryReader br)
		{
			Value = br.ReadSingle();
		}
	}
}
