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
using System.Drawing;

namespace Memoria.PSD.Compression
{
  internal class RawImage : ImageData
  {
    private byte[] _data;

    protected override bool AltersWrittenData
    {
      get { return false; }
    }

    public RawImage(byte[] data, Size size, int bitDepth)
      : base(size, bitDepth)
    {
      this._data = data;
    }

    internal override void Read(byte[] buffer)
    {
      Array.Copy(_data, buffer, _data.Length);
    }

    public override byte[] ReadCompressed()
    {
      return _data;
    }

    internal override void WriteInternal(byte[] array)
    {
      _data = array;
    }
  }
}
