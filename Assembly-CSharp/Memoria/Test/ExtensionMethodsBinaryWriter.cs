using System;
using System.IO;
using UnityEngine;

namespace Memoria.Test
{
	public static partial class ExtensionMethodsBinaryWriter
	{
		public static void Write(this BinaryWriter bw, Vector2 value)
		{
			bw.Write(value.x);
			bw.Write(value.y);
		}

		public static void Write(this BinaryWriter bw, Vector3 value)
		{
			bw.Write(value.x);
			bw.Write(value.y);
			bw.Write(value.z);
		}

		public static void Write(this BinaryWriter bw, Vector4 value)
		{
			bw.Write(value.x);
			bw.Write(value.y);
			bw.Write(value.z);
			bw.Write(value.w);
		}

		public static void Write(this BinaryWriter bw, Quaternion value)
		{
			bw.Write(value.x);
			bw.Write(value.y);
			bw.Write(value.z);
			bw.Write(value.w);
		}

		public static void Write(this BinaryWriter bw, Rect value)
		{
			bw.Write(value.x);
			bw.Write(value.y);
			bw.Write(value.width);
			bw.Write(value.height);
		}

		public static void Write(this BinaryWriter bw, Matrix4x4 value)
		{
			for (int i = 0; i < 4 * 4; i++)
				bw.Write(value[i]);
		}

		public static void Write(this BinaryWriter bw, RemotingMessageType messageType)
		{
			bw.Write((UInt16)messageType);
		}

		public static void Write(this BinaryWriter bw, CommandMessageType messageType)
		{
			bw.Write((UInt16)messageType);
		}

		public static void Write(this BinaryWriter bw, ValueMessageType messageType)
		{
			bw.Write((UInt16)messageType);
		}

		public static void Write(this BinaryWriter bw, ReferenceMessageType messageType)
		{
			bw.Write((UInt16)messageType);
		}
	}
}
