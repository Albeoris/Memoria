using System;
using System.IO;
using UnityEngine;

namespace Memoria.Test
{
	public sealed class QuaternionMessage : IValueMessage
	{
		public ValueMessageType ValueType => ValueMessageType.Quaternion;

		ValueType IValueMessage.Object => Value;

		public Quaternion Value;

		public QuaternionMessage(Quaternion value)
		{
			Value = value;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(Value);
		}

		public void Deserialize(BinaryReader br)
		{
			Value = br.ReadQuaternion();
		}
	}
}
