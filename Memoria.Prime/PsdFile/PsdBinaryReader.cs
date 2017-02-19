/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2013 Tao Yue
//
// Portions of this file are provided under the BSD 3-clause License:
//   Copyright (c) 2006, Jonas Beckeman
//
// 
//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text;

namespace Memoria.Prime.PsdFile
{
  /// <summary>
  /// Reads PSD data types in big-endian byte order.
  /// </summary>
  public class PsdBinaryReader : IDisposable
  {
    private BinaryReader _reader;
    private Encoding _encoding;

    public Stream BaseStream
    {
      get { return _reader.BaseStream; }
    }

    public PsdBinaryReader(Stream stream, PsdBinaryReader reader)
      : this (stream, reader._encoding)
    {
    }
    
    public PsdBinaryReader(Stream stream, Encoding encoding)
    {
      this._encoding = encoding;

      // ReadPascalString and ReadUnicodeString handle encoding explicitly.
      // BinaryReader.ReadString() is never called, so it is constructed with
      // ASCII encoding to make accidental usage obvious.
      _reader = new BinaryReader(stream, Encoding.ASCII);
    }

    public Byte ReadByte()
    {
      return _reader.ReadByte();
    }

    public Byte[] ReadBytes(Int32 count)
    {
      return _reader.ReadBytes(count);
    }

    public Boolean ReadBoolean()
    {
      return _reader.ReadBoolean();
    }

    public Int16 ReadInt16()
    {
      var val = _reader.ReadInt16();
      unsafe
      {
        Util.SwapBytes((Byte*)&val, 2);
      }
      return val;
    }

    public Int32 ReadInt32()
    {
      var val = _reader.ReadInt32();
      unsafe
      {
        Util.SwapBytes((Byte*)&val, 4);
      }
      return val;
    }

    public Int64 ReadInt64()
    {
      var val = _reader.ReadInt64();
      unsafe
      {
        Util.SwapBytes((Byte*)&val, 8);
      }
      return val;
    }

    public UInt16 ReadUInt16()
    {
      var val = _reader.ReadUInt16();
      unsafe
      {
        Util.SwapBytes((Byte*)&val, 2);
      }
      return val;
    }

    public UInt32 ReadUInt32()
    {
      var val = _reader.ReadUInt32();
      unsafe
      {
        Util.SwapBytes((Byte*)&val, 4);
      }
      return val;
    }

    public UInt64 ReadUInt64()
    {
      var val = _reader.ReadUInt64();
      unsafe
      {
        Util.SwapBytes((Byte*)&val, 8);
      }
      return val;
    }

    //////////////////////////////////////////////////////////////////

    /// <summary>
    /// Read padding to get to the byte multiple for the block.
    /// </summary>
    /// <param name="startPosition">Starting position of the padded block.</param>
    /// <param name="padMultiple">Byte multiple that the block is padded to.</param>
    public void ReadPadding(Int64 startPosition, Int32 padMultiple)
    {
      // Pad to specified byte multiple
      var totalLength = _reader.BaseStream.Position - startPosition;
      var padBytes = Util.GetPadding((Int32)totalLength, padMultiple);
      ReadBytes(padBytes);
    }

    public Rectangle ReadRectangle()
    {
      var rect = new Rectangle();
      rect.Y = ReadInt32();
      rect.X = ReadInt32();
      rect.Height = ReadInt32() - rect.Y;
      rect.Width = ReadInt32() - rect.X;
      return rect;
    }

    /// <summary>
    /// Read a fixed-length ASCII string.
    /// </summary>
    public String ReadAsciiChars(Int32 count)
    {
      var bytes = _reader.ReadBytes(count); ;
      var s = Encoding.ASCII.GetString(bytes);
      return s;
    }

    /// <summary>
    /// Read a Pascal string using the specified encoding.
    /// </summary>
    /// <param name="padMultiple">Byte multiple that the Pascal string is padded to.</param>
    public String ReadPascalString(Int32 padMultiple)
    {
      var startPosition = _reader.BaseStream.Position;

      Byte stringLength = ReadByte();
      var bytes = ReadBytes(stringLength);
      ReadPadding(startPosition, padMultiple);

      // Default decoder uses best-fit fallback, so it will not throw any
      // exceptions if unknown characters are encountered.
      var str = _encoding.GetString(bytes);
      return str;
    }

    public String ReadUnicodeString()
    {
      var numChars = ReadInt32();
      var length = 2 * numChars;
      var data = ReadBytes(length);
      var str = Encoding.BigEndianUnicode.GetString(data, 0, length);

      return str;
    }

    //////////////////////////////////////////////////////////////////

    # region IDisposable

    private Boolean _disposed = false;

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(Boolean disposing)
    {
      // Check to see if Dispose has already been called. 
      if (_disposed)
        return;

      if (disposing)
      {
        if (_reader != null)
        {
          // BinaryReader.Dispose() is protected.
          _reader.Close();
          _reader = null;
        }
      }

      _disposed = true;
    }

    #endregion

  }

}