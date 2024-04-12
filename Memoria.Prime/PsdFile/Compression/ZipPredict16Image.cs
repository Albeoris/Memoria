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

namespace Memoria.Prime.PsdFile
{
	public class ZipPredict16Image : ImageData
	{
		private ImageData _zipImage;

		protected override Boolean AltersWrittenData
		{
			get { return true; }
		}

		public ZipPredict16Image(Byte[] zipData, Size size)
		  : base(size, 16)
		{
			// 16-bitdepth images are delta-encoded word-by-word.  The deltas
			// are thus big-endian and must be reversed for further processing.
			var zipRawImage = new ZipImage(zipData, size, 16);
			_zipImage = new EndianReverser(zipRawImage);
		}

		internal override void Read(Byte[] buffer)
		{
			if (buffer.Length == 0)
			{
				return;
			}

			_zipImage.Read(buffer);
			unsafe
			{
				fixed (Byte* ptrData = &buffer[0])
				{
					Unpredict((UInt16*)ptrData);
				}
			}
		}

		public override Byte[] ReadCompressed()
		{
			return _zipImage.ReadCompressed();
		}

		internal override void WriteInternal(Byte[] array)
		{
			if (array.Length == 0)
			{
				return;
			}

			unsafe
			{
				fixed (Byte* ptrData = &array[0])
				{
					Predict((UInt16*)ptrData);
				}
			}

			_zipImage.WriteInternal(array);
		}

		unsafe private void Predict(UInt16* ptrData)
		{
			// Delta-encode each row
			for (Int32 i = 0; i < Size.Height; i++)
			{
				UInt16* ptrDataRow = ptrData;
				UInt16* ptrDataRowEnd = ptrDataRow + Size.Width;

				// Start with the last column in the row
				ptrData = ptrDataRowEnd - 1;
				while (ptrData > ptrDataRow)
				{
					*ptrData -= *(ptrData - 1);
					ptrData--;
				}
				ptrData = ptrDataRowEnd;
			}
		}

		/// <summary>
		/// Unpredicts the decompressed, native-endian image data.
		/// </summary>
		unsafe private void Unpredict(UInt16* ptrData)
		{
			// Delta-decode each row
			for (Int32 i = 0; i < Size.Height; i++)
			{
				UInt16* ptrDataRowEnd = ptrData + Size.Width;

				// Start with column index 1 on each row
				ptrData++;
				while (ptrData < ptrDataRowEnd)
				{
					*ptrData += *(ptrData - 1);
					ptrData++;
				}

				// Advance pointer to the next row
				ptrData = ptrDataRowEnd;
			}
		}
	}
}
