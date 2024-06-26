using System;

namespace Unity.IO.Compression
{
	internal interface IFileFormatWriter
	{
		Byte[] GetHeader();

		void UpdateWithBytesRead(Byte[] buffer, Int32 offset, Int32 bytesToCopy);

		Byte[] GetFooter();
	}
}
