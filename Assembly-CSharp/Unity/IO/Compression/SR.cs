using System;

namespace Unity.IO.Compression
{
	internal class SR
	{
		private SR()
		{
		}

		internal static String GetString(String p)
		{
			return p;
		}

		public const String ArgumentOutOfRange_Enum = "Argument out of range";

		public const String CorruptedGZipHeader = "Corrupted gzip header";

		public const String CannotReadFromDeflateStream = "Cannot read from deflate stream";

		public const String CannotWriteToDeflateStream = "Cannot write to deflate stream";

		public const String GenericInvalidData = "Invalid data";

		public const String InvalidCRC = "Invalid CRC";

		public const String InvalidStreamSize = "Invalid stream size";

		public const String InvalidHuffmanData = "Invalid Huffman data";

		public const String InvalidBeginCall = "Invalid begin call";

		public const String InvalidEndCall = "Invalid end call";

		public const String InvalidBlockLength = "Invalid block length";

		public const String InvalidArgumentOffsetCount = "Invalid argument offset count";

		public const String NotSupported = "Not supported";

		public const String NotWriteableStream = "Not a writeable stream";

		public const String NotReadableStream = "Not a readable stream";

		public const String ObjectDisposed_StreamClosed = "Object disposed";

		public const String UnknownState = "Unknown state";

		public const String UnknownCompressionMode = "Unknown compression mode";

		public const String UnknownBlockType = "Unknown block type";
	}
}
