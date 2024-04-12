using System;
using System.IO;
using UnityEngine;

namespace Memoria.Test
{
	public sealed class Matrix4x4Message : IValueMessage
	{
		public ValueMessageType ValueType => ValueMessageType.Matrix4x4;

		ValueType IValueMessage.Object => Value;

		public Matrix4x4 Value;

		public Matrix4x4Message(Matrix4x4 value)
		{
			Value = value;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(Value);
		}

		public void Deserialize(BinaryReader br)
		{
			Value = br.ReadMatrix4x4();
		}
	}
}
