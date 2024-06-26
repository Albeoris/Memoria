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
  public class ZipPredict32Image : ImageData
  {
    private ZipImage _zipImage;

    protected override Boolean AltersWrittenData
    {
      // Prediction will pack the data into a temporary buffer, so the
      // original data will remain unchanged.
      get { return false; }
    }

    public ZipPredict32Image(Byte[] zipData, Size size)
      : base(size, 32)
    {
      _zipImage = new ZipImage(zipData, size, 32);
    }

    internal override void Read(Byte[] buffer)
    {
      if (buffer.Length == 0)
      {
        return;
      }

      var predictedData = new Byte[buffer.Length];
      _zipImage.Read(predictedData);

      unsafe
      {
        fixed (Byte* ptrData = &predictedData[0])
        fixed (Byte* ptrOutput = &buffer[0])
        {
          Unpredict(ptrData, (Int32*)ptrOutput);
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

      var predictedData = new Byte[array.Length];
      unsafe
      {
        fixed (Byte* ptrData = &array[0])
        fixed (Byte* ptrOutput = &predictedData[0])
        {
          Predict((Int32*)ptrData, ptrOutput);
        }
      }

      _zipImage.WriteInternal(predictedData);
    }

    unsafe private void Predict(Int32* ptrData, Byte* ptrOutput)
    {
      for (Int32 i = 0; i < Size.Height; i++)
      {
        // Pack together the individual bytes of the 32-bit words, high-order
        // bytes before low-order bytes.
        Int32 offset1 = Size.Width;
        Int32 offset2 = 2 * offset1;
        Int32 offset3 = 3 * offset1;

        Int32* ptrDataRow = ptrData;
        Int32* ptrDataRowEnd = ptrDataRow + Size.Width;
        Byte* ptrOutputRow = ptrOutput;
        Byte* ptrOutputRowEnd = ptrOutputRow + BytesPerRow;
        while (ptrData < ptrDataRowEnd)
        {
          *(ptrOutput)           = (Byte)(*ptrData >> 24);
          *(ptrOutput + offset1) = (Byte)(*ptrData >> 16);
          *(ptrOutput + offset2) = (Byte)(*ptrData >> 8);
          *(ptrOutput + offset3) = (Byte)(*ptrData);

          ptrData++;
          ptrOutput++;
        }

        // Delta-encode the row
        ptrOutput = ptrOutputRowEnd - 1;
        while (ptrOutput > ptrOutputRow)
        {
          *ptrOutput -= *(ptrOutput - 1);
          ptrOutput--;
        }

        // Advance pointer to next row
        ptrOutput = ptrOutputRowEnd;
        Debug.Assert(ptrData == ptrDataRowEnd);
      }
    }

    /// <summary>
    /// Unpredicts the raw decompressed image data into a 32-bpp bitmap with
    /// native endianness.
    /// </summary>
    unsafe private void Unpredict(Byte* ptrData, Int32* ptrOutput)
    {
      for (Int32 i = 0; i < Size.Height; i++)
      {
        Byte* ptrDataRow = ptrData;
        Byte* ptrDataRowEnd = ptrDataRow + BytesPerRow;

        // Delta-decode each row
        ptrData++;
        while (ptrData < ptrDataRowEnd)
        {
          *ptrData += *(ptrData - 1);
          ptrData++;
        }

        // Within each row, the individual bytes of the 32-bit words are
        // packed together, high-order bytes before low-order bytes.
        // We now unpack them into words.
        Int32 offset1 = Size.Width;
        Int32 offset2 = 2 * offset1;
        Int32 offset3 = 3 * offset1;

        ptrData = ptrDataRow;
        Int32* ptrOutputRowEnd = ptrOutput + Size.Width;
        while (ptrOutput < ptrOutputRowEnd)
        {
          *ptrOutput = *(ptrData) << 24
            | *(ptrData + offset1) << 16
            | *(ptrData + offset2) << 8
            | *(ptrData + offset3);

          ptrData++;
          ptrOutput++;
        }

        // Advance pointer to next row
        ptrData = ptrDataRowEnd;
        Debug.Assert(ptrOutput == ptrOutputRowEnd);
      }
    }
  }
}
