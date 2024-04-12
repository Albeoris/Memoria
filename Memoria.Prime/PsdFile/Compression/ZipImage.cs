/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2016 Tao Yue
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.IO.Compression;

namespace Memoria.Prime.PsdFile
{
	public class ZipImage : ImageData
	{
		private MemoryStream _zipDataStream;
		private DeflateStream _zipStream;

		protected override Boolean AltersWrittenData
		{
			get { return false; }
		}

		public ZipImage(Byte[] data, Size size, Int32 bitDepth)
		  : base(size, bitDepth)
		{
			if (data == null)
			{
				InitCompress();
			}
			else
			{
				InitDecompress(data);
			}
		}

		private void InitCompress()
		{
			_zipDataStream = new MemoryStream();

			// Write 2-byte zlib (RFC 1950) header
			//
			// CMF Compression Method and flags:
			//   CM     0:3 = 8 = deflate
			//   CINFO  4:7 = 4 = undefined, RFC 1950 only defines CINFO = 8
			//
			// FLG Flags:
			//   FCHECK  0:4 = 9 = check bits for CMF and FLG
			//   FDICT     5 = 0 = no preset dictionary
			//   FLEVEL  6:7 = 2 = default compression level

			_zipDataStream.WriteByte(0x48);
			_zipDataStream.WriteByte(0x89);
			_zipStream = new DeflateStream(_zipDataStream, CompressionMode.Compress,
			  true);
		}

		private void InitDecompress(Byte[] data)
		{
			_zipDataStream = new MemoryStream(data);

			// .NET implements Deflate (RFC 1951) but not zlib (RFC 1950),
			// so we have to skip the first two bytes.
			_zipDataStream.ReadByte();
			_zipDataStream.ReadByte();
			_zipStream = new DeflateStream(_zipDataStream, CompressionMode.Decompress,
			  true);
		}

		internal override void Read(Byte[] buffer)
		{
			var bytesToRead = (Int64)Size.Height * BytesPerRow;
			Util.CheckByteArrayLength(bytesToRead);

			var bytesRead = _zipStream.Read(buffer, 0, (Int32)bytesToRead);
			if (bytesRead != bytesToRead)
			{
				throw new Exception("ZIP stream was not fully decompressed.");
			}
		}

		public override Byte[] ReadCompressed()
		{
			// Write out the last block.  (Flush leaves the last block open.)
			_zipStream.Close();

			// Do not write the zlib header when the image data is empty
			var result = (_zipDataStream.Length == 2)
			  ? new Byte[0]
			  : _zipDataStream.ToArray();
			return result;
		}

		internal override void WriteInternal(Byte[] array)
		{
			_zipStream.Write(array, 0, array.Length);
		}
	}
}
