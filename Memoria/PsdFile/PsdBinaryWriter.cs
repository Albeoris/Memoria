/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2016 Tao Yue
//
// Portions of this file are provided under the BSD 3-clause License:
//   Copyright (c) 2006, Jonas Beckeman
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Memoria.PSD
{
  /// <summary>
  /// Writes PSD data types in big-endian byte order.
  /// </summary>
  public class PsdBinaryWriter : IDisposable
  {
    private BinaryWriter _writer;
    private Encoding _encoding;

    internal Stream BaseStream
    {
      get
      {
        // Flush the writer so that the Stream.Position is correct.
        Flush();
        return _writer.BaseStream;
      }
    }

    public PsdBinaryWriter(Stream stream, Encoding encoding)
    {
      this._encoding = encoding;

      // Specifying ASCII encoding will help catch any accidental calls to
      // BinaryWriter.Write(String).  Since we do not own the Stream, the
      // constructor is called with leaveOpen = true.
      _writer = new BinaryWriter(stream, Encoding.ASCII);
    }

    public void Flush()
    {
      _writer.Flush();
    }

    public void Write(Rectangle rect)
    {
      Write(rect.Top);
      Write(rect.Left);
      Write(rect.Bottom);
      Write(rect.Right);
    }

    /// <summary>
    /// Pad the length of a block to a multiple.
    /// </summary>
    /// <param name="startPosition">Starting position of the padded block.</param>
    /// <param name="padMultiple">Byte multiple to pad to.</param>
    public void WritePadding(long startPosition, int padMultiple)
    {
      var length = _writer.BaseStream.Position - startPosition;
      var padBytes = Util.GetPadding((int)length, padMultiple);
      for (long i = 0; i < padBytes; i++)
      {
        _writer.Write((byte)0);
      }
    }

    /// <summary>
    /// Write string as ASCII characters, without a length prefix.
    /// </summary>
    public void WriteAsciiChars(string s)
    {
      var bytes = Encoding.ASCII.GetBytes(s);
      _writer.Write(bytes);
    }


    /// <summary>
    /// Writes a Pascal string using the specified encoding.
    /// </summary>
    /// <param name="s">Unicode string to convert to the encoding.</param>
    /// <param name="padMultiple">Byte multiple that the Pascal string is padded to.</param>
    /// <param name="maxBytes">Maximum number of bytes to write.</param>
    public void WritePascalString(string s, int padMultiple, byte maxBytes = 255)
    {
      var startPosition = _writer.BaseStream.Position;

      byte[] bytesArray = _encoding.GetBytes(s);
      if (bytesArray.Length > maxBytes)
      {
        var tempArray = new byte[maxBytes];
        Array.Copy(bytesArray, tempArray, maxBytes);
        bytesArray = tempArray;
      }

      _writer.Write((byte)bytesArray.Length);
      _writer.Write(bytesArray);
      WritePadding(startPosition, padMultiple);
    }

    /// <summary>
    /// Write a Unicode string to the stream.
    /// </summary>
    public void WriteUnicodeString(string s)
    {
      Write(s.Length);
      var data = Encoding.BigEndianUnicode.GetBytes(s);
      Write(data);
    }

    public void Write(bool value)
    {
      _writer.Write(value);
    }

    public void Write(byte[] value)
    {
      _writer.Write(value);
    }

    public void Write(byte value)
    {
      _writer.Write(value);
    }

    public void Write(Int16 value)
    {
      unsafe
      {
        Util.SwapBytes2((byte*)&value);
      }
      _writer.Write(value);
    }

    public void Write(Int32 value)
    {
      unsafe
      {
        Util.SwapBytes4((byte*)&value);
      }
      _writer.Write(value);
    }

    public void Write(Int64 value)
    {
      unsafe
      {
        Util.SwapBytes((byte*)&value, 8);
      }
      _writer.Write(value);
    }

    public void Write(UInt16 value)
    {
      unsafe
      {
        Util.SwapBytes2((byte*)&value);
      }
      _writer.Write(value);
    }

    public void Write(UInt32 value)
    {
      unsafe
      {
        Util.SwapBytes4((byte*)&value);
      }
      _writer.Write(value);
    }

    public void Write(UInt64 value)
    {
      unsafe
      {
        Util.SwapBytes((byte*)&value, 8);
      }
      _writer.Write(value);
    }


    //////////////////////////////////////////////////////////////////

    # region IDisposable

    private bool _disposed = false;

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      // Check to see if Dispose has already been called. 
      if (_disposed)
      {
        return;
      }

      if (disposing)
      {
        if (_writer != null)
        {
                    // BinaryWriter.Dispose() is protected, so we have to call Close.
                    // The BinaryWriter will be automatically flushed on close.
            _writer.Flush();
          _writer = null;
        }
      }

      _disposed = true;
    }

    #endregion

  }
}