using System;
using System.IO;

namespace Memoria.Test
{
	public sealed class UiRectPositionMessage : IValueMessage
	{
		public ValueMessageType ValueType => ValueMessageType.UiRectPosition;

		ValueType IValueMessage.Object => Value;

		public UIRect.Position Value;

		public UiRectPositionMessage(UIRect.Position value)
		{
			Value = value;
		}

		public void Serialize(BinaryWriter bw)
		{
			Value.Write(bw);
		}

		public void Deserialize(BinaryReader br)
		{
			Value = UIRect.Position.Read(br);
		}
	}
}
