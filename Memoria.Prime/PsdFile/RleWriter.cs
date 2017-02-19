/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2015 Tao Yue
//
// Portions of this file are provided under the BSD 3-clause License:
//   Copyright (c) 2006, Jonas Beckeman
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace Memoria.Prime.PsdFile
{
  public class RleWriter
  {
    private Int32 _maxPacketLength = 128;

    // Current task
    private Object _rleLock;
    private Stream _stream;
    private Byte[] _data;
    private Int32 _offset;

    // Current packet
    private Boolean _isRepeatPacket;
    private Int32 _idxPacketStart;
    private Int32 _packetLength;

    private Byte _runValue;
    private Int32 _runLength;

    public RleWriter(Stream stream)
    {
      _rleLock = new Object();
      this._stream = stream;
    }

    /// <summary>
    /// Encodes byte data using PackBits RLE compression.
    /// </summary>
    /// <param name="data">Raw data to be encoded.</param>
    /// <param name="offset">Offset at which to begin transferring data.</param>
    /// <param name="count">Number of bytes of data to transfer.</param>
    /// <returns>Number of compressed bytes written to the stream.</returns>
    /// <remarks>
    /// There are multiple ways to encode two-byte runs:
    ///   1. Apple PackBits only encodes three-byte runs as repeats.
    ///   2. Adobe Photoshop encodes two-byte runs as repeats, unless preceded
    ///      by literals.
    ///   3. TIFF PackBits recommends that two-byte runs be encoded as repeats,
    ///      unless preceded *and* followed by literals.
    ///
    /// This class adopts the Photoshop behavior, as it has slightly better
    /// compression efficiency than Apple PackBits, and is easier to implement
    /// than TIFF PackBits.
    /// </remarks>
    unsafe public Int32 Write(Byte[] data, Int32 offset, Int32 count)
    {
      if (!Util.CheckBufferBounds(data, offset, count))
        throw new ArgumentOutOfRangeException();

      // We cannot encode a count of 0, because the PackBits flag-counter byte
      // uses 0 to indicate a length of 1.
      if (count == 0)
      {
        throw new ArgumentOutOfRangeException(nameof(count));
      }

      lock (_rleLock)
      {
        var startPosition = _stream.Position;

        this._data = data;
        this._offset = offset;
        fixed (Byte* ptrData = &data[0])
        {
          Byte* ptr = ptrData + offset;
          Byte* ptrEnd = ptr + count;
          var bytesEncoded = EncodeToStream(ptr, ptrEnd);
          Debug.Assert(bytesEncoded == count, "Encoded byte count should match the argument.");
        }

        return (Int32)(_stream.Position - startPosition);
      }
    }

    private void ClearPacket()
    {
      this._isRepeatPacket = false;
      this._packetLength = 0;
    }

    private void WriteRepeatPacket(Int32 length)
    {
      var header = unchecked((Byte)(1 - length));
      _stream.WriteByte(header);
      _stream.WriteByte(_runValue);
    }

    private void WriteLiteralPacket(Int32 length)
    {
      var header = unchecked((Byte)(length - 1));
      _stream.WriteByte(header);
      _stream.Write(_data, _idxPacketStart, length);
    }

    private void WritePacket()
    {
      if (_isRepeatPacket)
        WriteRepeatPacket(_packetLength);
      else
        WriteLiteralPacket(_packetLength);
    }

    private void StartPacket(Int32 count,
      Boolean isRepeatPacket, Int32 runLength, Byte value)
    {
      this._isRepeatPacket = isRepeatPacket;

      this._packetLength = runLength;
      this._runLength = runLength;
      this._runValue = value;

      this._idxPacketStart = _offset + count;
    }

    private void ExtendPacketAndRun(Byte value)
    {
      _packetLength++;
      _runLength++;
    }

    private void ExtendPacketStartNewRun(Byte value)
    {
      _packetLength++;
      _runLength = 1;
      _runValue = value;
    }

    unsafe private Int32 EncodeToStream(Byte* ptr, Byte* ptrEnd)
    {
      // Begin the first packet.
      StartPacket(0, false, 1, *ptr);
      Int32 numBytesEncoded = 1;
      ptr++;

      // Loop invariant: Packet is never empty.
      while (ptr < ptrEnd)
      {
        var value = *ptr;

        if (_packetLength == 1)
        {
          _isRepeatPacket = (value == _runValue);
          if (_isRepeatPacket)
            ExtendPacketAndRun(value);
          else
            ExtendPacketStartNewRun(value);
        }
        else if (_packetLength == _maxPacketLength)
        {
          // Packet is full, so write it out and start a new one.
          WritePacket();
          StartPacket(numBytesEncoded, false, 1, value);
        }
        else if (_isRepeatPacket)
        {
          // Decide whether to continue the repeat packet.
          if (value == _runValue)
            ExtendPacketAndRun(value);
          else
          {
            // Different color, so terminate the run and start a new packet.
            WriteRepeatPacket(_packetLength);
            StartPacket(numBytesEncoded, false, 1, value);
          }
        }
        else
        {
          // Decide whether to continue the literal packet.
          if (value == _runValue)
          {
            ExtendPacketAndRun(value);

            // A 3-byte run terminates the literal and starts a new repeat
            // packet.  That's because the 3-byte run can be encoded as a
            // 2-byte repeat.  So even if the run ends at 3, we've already
            // paid for the next flag-counter byte.
            if (_runLength == 3)
            {
              // The 3-byte run can come in the middle of a literal packet,
              // but not at the beginning.  The first 2 bytes of the run
              // should've triggered a repeat packet.
              Debug.Assert(_packetLength > 3);

              // -2 because numBytesEncoded has not yet been incremented
              WriteLiteralPacket(_packetLength - 3);
              StartPacket(numBytesEncoded - 2, true, 3, value);
            }
          }
          else
          {
            ExtendPacketStartNewRun(value);
          }
        }

        ptr++;
        numBytesEncoded++;
      }

      // Loop terminates with a non-empty packet waiting to be written out.
      WritePacket();
      ClearPacket();

      return numBytesEncoded;
    }
  }
}
