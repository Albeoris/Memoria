/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is ptortorovided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2016 Tao Yue
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace Memoria.Prime.PsdFile
{
	internal class RleImage : ImageData
	{
		private Byte[] _rleData;
		private RleRowLengths _rleRowLengths;

		protected override Boolean AltersWrittenData
		{
			get { return false; }
		}

		public RleImage(Byte[] rleData, RleRowLengths rleRowLengths,
		  Size size, Int32 bitDepth)
		  : base(size, bitDepth)
		{
			this._rleData = rleData;
			this._rleRowLengths = rleRowLengths;
		}

		internal override void Read(Byte[] buffer)
		{
			var rleStream = new MemoryStream(_rleData);
			var rleReader = new RleReader(rleStream);
			var bufferIndex = 0;
			for (Int32 i = 0; i < Size.Height; i++)
			{
				var bytesRead = rleReader.Read(buffer, bufferIndex, BytesPerRow);
				if (bytesRead != BytesPerRow)
				{
					throw new Exception("RLE row decompressed to unexpected length.");
				}
				bufferIndex += bytesRead;
			}
		}

		public override Byte[] ReadCompressed()
		{
			return _rleData;
		}

		internal override void WriteInternal(Byte[] array)
		{
			if (_rleData != null)
			{
				throw new Exception(
				  "Cannot write to RLE image in Decompress mode.");
			}

			using (var dataStream = new MemoryStream())
			{
				var rleWriter = new RleWriter(dataStream);
				for (Int32 row = 0; row < Size.Height; row++)
				{
					Int32 rowIndex = row * BytesPerRow;
					_rleRowLengths[row] = rleWriter.Write(
					  array, rowIndex, BytesPerRow);
				}

				// Save compressed data
				dataStream.Flush();
				_rleData = dataStream.ToArray();
				Debug.Assert(_rleRowLengths.Total == _rleData.Length,
				  "RLE row lengths do not sum to the compressed data length.");
			}
		}
	}
}
