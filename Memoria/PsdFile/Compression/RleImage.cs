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
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Memoria.PSD.Compression
{
  internal class RleImage : ImageData
  {
    private byte[] _rleData;
    private RleRowLengths _rleRowLengths;

    protected override bool AltersWrittenData
    {
      get { return false; }
    }

    public RleImage(byte[] rleData, RleRowLengths rleRowLengths,
      Size size, int bitDepth)
      : base(size, bitDepth)
    {
      this._rleData = rleData;
      this._rleRowLengths = rleRowLengths;
    }

    internal override void Read(byte[] buffer)
    {
      var rleStream = new MemoryStream(_rleData);
      var rleReader = new RleReader(rleStream);
      var bufferIndex = 0;
      for (int i = 0; i < Size.Height; i++)
      {
        var bytesRead = rleReader.Read(buffer, bufferIndex, BytesPerRow);
        if (bytesRead != BytesPerRow)
        {
          throw new Exception("RLE row decompressed to unexpected length.");
        }
        bufferIndex += bytesRead;
      }
    }

    public override byte[] ReadCompressed()
    {
      return _rleData;
    }

    internal override void WriteInternal(byte[] array)
    {
      if (_rleData != null)
      {
        throw new Exception(
          "Cannot write to RLE image in Decompress mode.");
      }

      using (var dataStream = new MemoryStream())
      {
        var rleWriter = new RleWriter(dataStream);
        for (int row = 0; row < Size.Height; row++)
        {
          int rowIndex = row * BytesPerRow;
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
