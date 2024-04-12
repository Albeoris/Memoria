using System;
using System.IO;
using UnityEngine;

namespace Memoria.Test
{
	public sealed class Vector2Message : IValueMessage
	{
		public ValueMessageType ValueType => ValueMessageType.Vector2;

		ValueType IValueMessage.Object => Value;

		public Vector2 Value;

		public Vector2Message(Vector2 value)
		{
			Value = value;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(Value);
		}

		public void Deserialize(BinaryReader br)
		{
			Value = br.ReadVector2();
		}
	}
}
