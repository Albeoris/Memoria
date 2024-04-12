using System;
using System.IO;

namespace Memoria.Prime
{
	public static class ExtensionMethodsStream
	{
		public static void SetPosition(this Stream self, Int32 position)
		{
			if (self.Position != position)
				self.Position = position;
		}

		public static void CopyTo(this Stream self, Stream destination, Int64 bufferSize)
		{
			Int32 count;
			Byte[] buffer = new Byte[bufferSize];
			while ((count = self.Read(buffer, 0, buffer.Length)) != 0)
				destination.Write(buffer, 0, count);
		}

		public static void EnsureRead(this Stream self, Byte[] buff, Int32 offset, Int32 size)
		{
			Int32 readed;
			while (size > 0 && (readed = self.Read(buff, offset, size)) != 0)
			{
				size -= readed;
				offset += readed;
			}

			if (size != 0)
				throw new EndOfStreamException("Unexpected end of stream.");
		}

		public static T ReadStruct<T>(this Stream input) where T : unmanaged
		{
			return ReadStructs<T>(input, count: 1)[0];
		}

		public static T[] ReadStructs<T>(this Stream input, Int32 count) where T : unmanaged
		{
			if (count < 1)
				return new T[0];

			unsafe
			{
				Array result = new T[count];
				Int32 entrySize = UnsafeTypeCache<T>.UnsafeSize;
				using (UnsafeTypeCache<Byte>.ChangeArrayTypes(result, entrySize))
					input.EnsureRead((Byte[])result, 0, result.Length);
				return (T[])result;
			}
		}
	}
}
