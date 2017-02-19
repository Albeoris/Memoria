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
  internal class RawImage : ImageData
  {
    private Byte[] _data;

    protected override Boolean AltersWrittenData
    {
      get { return false; }
    }

    public RawImage(Byte[] data, Size size, Int32 bitDepth)
      : base(size, bitDepth)
    {
      this._data = data;
    }

    internal override void Read(Byte[] buffer)
    {
      Array.Copy(_data, buffer, _data.Length);
    }

    public override Byte[] ReadCompressed()
    {
      return _data;
    }

    internal override void WriteInternal(Byte[] array)
    {
      _data = array;
    }
  }
}
