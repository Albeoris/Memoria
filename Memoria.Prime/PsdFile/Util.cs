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
using System.Diagnostics;
using System.IO;

namespace Memoria.Prime.PsdFile
{
  public static class Util
  {
    [DebuggerDisplay("Top = {Top}, Bottom = {Bottom}, Left = {Left}, Right = {Right}")]
    public struct RectanglePosition
    {
      public Int32 Top { get; set; }
      public Int32 Bottom { get; set; }
      public Int32 Left { get; set; }
      public Int32 Right { get; set; }
    }

    public static Rectangle IntersectWith(
      this Rectangle thisRect, Rectangle rect)
    {
      thisRect.Intersect(rect);
      return thisRect;
    }

    /////////////////////////////////////////////////////////////////////////// 

    /// <summary>
    /// Fills a buffer with a byte value.
    /// </summary>
    unsafe static public void Fill(Byte* ptr, Byte* ptrEnd, Byte value)
    {
      while (ptr < ptrEnd)
      {
        *ptr = value;
        ptr++;
      }
    }

    unsafe static public void Invert(Byte* ptr, Byte* ptrEnd)
    {
      while (ptr < ptrEnd)
      {
        *ptr = (Byte)(*ptr ^ 0xff);
        ptr++;
      }
    }

    /////////////////////////////////////////////////////////////////////////// 

    /// <summary>
    /// Reverses the endianness of a 2-byte word.
    /// </summary>
    unsafe static public void SwapBytes2(Byte* ptr)
    {
      Byte byte0 = *ptr;
      *ptr = *(ptr + 1);
      *(ptr + 1) = byte0;
    }

    /////////////////////////////////////////////////////////////////////////// 

    /// <summary>
    /// Reverses the endianness of a 4-byte word.
    /// </summary>
    unsafe static public void SwapBytes4(Byte* ptr)
    {
      Byte byte0 = *ptr;
      Byte byte1 = *(ptr + 1);

      *ptr = *(ptr + 3);
      *(ptr + 1) = *(ptr + 2);
      *(ptr + 2) = byte1;
      *(ptr + 3) = byte0;
    }

    /// <summary>
    /// Reverses the endianness of a word of arbitrary length.
    /// </summary>
    unsafe static public void SwapBytes(Byte* ptr, Int32 nLength)
    {
      for (Int64 i = 0; i < nLength / 2; ++i)
      {
        Byte t = *(ptr + i);
        *(ptr + i) = *(ptr + nLength - i - 1);
        *(ptr + nLength - i - 1) = t;
      }
    }

    /////////////////////////////////////////////////////////////////////////// 

    public static void SwapByteArray(Int32 bitDepth,
      Byte[] byteArray, Int32 startIdx, Int32 count)
    {
      switch (bitDepth)
      {
        case 1:
        case 8:
          break;

        case 16:
          SwapByteArray2(byteArray, startIdx, count);
          break;

        case 32:
          SwapByteArray4(byteArray, startIdx, count);
          break;

        default:
          throw new Exception(
            "Byte-swapping implemented only for 16-bit and 32-bit depths.");
      }
    }

    /// <summary>
    /// Reverses the endianness of 2-byte words in a byte array.
    /// </summary>
    /// <param name="byteArray">Byte array containing the sequence on which to swap endianness</param>
    /// <param name="startIdx">Byte index of the first word to swap</param>
    /// <param name="count">Number of words to swap</param>
    public static void SwapByteArray2(Byte[] byteArray, Int32 startIdx, Int32 count)
    {
      Int32 endIdx = startIdx + count * 2;
      if (byteArray.Length < endIdx)
        throw new IndexOutOfRangeException();

      unsafe
      {
        fixed (Byte* arrayPtr = &byteArray[0])
        {
          Byte* ptr = arrayPtr + startIdx;
          Byte* endPtr = arrayPtr + endIdx;
          while (ptr < endPtr)
          {
            SwapBytes2(ptr);
            ptr += 2;
          }
        }
      }
    }

    /// <summary>
    /// Reverses the endianness of 4-byte words in a byte array.
    /// </summary>
    /// <param name="byteArray">Byte array containing the sequence on which to swap endianness</param>
    /// <param name="startIdx">Byte index of the first word to swap</param>
    /// <param name="count">Number of words to swap</param>
    public static void SwapByteArray4(Byte[] byteArray, Int32 startIdx, Int32 count)
    {
      Int32 endIdx = startIdx + count * 4;
      if (byteArray.Length < endIdx)
        throw new IndexOutOfRangeException();

      unsafe
      {
        fixed (Byte* arrayPtr = &byteArray[0])
        {
          Byte* ptr = arrayPtr + startIdx;
          Byte* endPtr = arrayPtr + endIdx;
          while (ptr < endPtr)
          {
            SwapBytes4(ptr);
            ptr += 4;
          }
        }
      }
    }

    /////////////////////////////////////////////////////////////////////////// 

    /// <summary>
    /// Calculates the number of bytes required to store a row of an image
    /// with the specified bit depth.
    /// </summary>
    /// <param name="size">The size of the image in pixels.</param>
    /// <param name="bitDepth">The bit depth of the image.</param>
    /// <returns>The number of bytes needed to store a row of the image.</returns>
    public static Int32 BytesPerRow(Size size, Int32 bitDepth)
    {
      switch (bitDepth)
      {
        case 1:
          return (size.Width + 7) / 8;
        default:
          return size.Width * BytesFromBitDepth(bitDepth);
      }
    }

    /// <summary>
    /// Round the integer to a multiple.
    /// </summary>
    public static Int32 RoundUp(Int32 value, Int32 multiple)
    {
      if (value == 0)
        return 0;

      if (Math.Sign(value) != Math.Sign(multiple))
      {
        throw new ArgumentException(
          $"{nameof(value)} and {nameof(multiple)} cannot have opposite signs.");
      }

      var remainder = value % multiple;
      if (remainder > 0)
      {
        value += (multiple - remainder);
      }
      return value;
    }

    /// <summary>
    /// Get number of bytes required to pad to the specified multiple.
    /// </summary>
    public static Int32 GetPadding(Int32 length, Int32 padMultiple)
    {
      if ((length < 0) || (padMultiple < 0))
        throw new ArgumentException();

      var remainder = length % padMultiple;
      if (remainder == 0)
        return 0;

      var padding = padMultiple - remainder;
      return padding;
    }

    /// <summary>
    /// Returns the number of bytes needed to store a single pixel of the
    /// specified bit depth.
    /// </summary>
    public static Int32 BytesFromBitDepth(Int32 depth)
    {
      switch (depth)
      {
        case 1:
        case 8:
          return 1;
        case 16:
          return 2;
        case 32:
          return 4;
        default:
          throw new ArgumentException("Invalid bit depth.");
      }
    }

    public static Int16 MinChannelCount(this PsdColorMode colorMode)
    {
      switch (colorMode)
      {
        case PsdColorMode.Bitmap:
        case PsdColorMode.Duotone:
        case PsdColorMode.Grayscale:
        case PsdColorMode.Indexed:
        case PsdColorMode.Multichannel:
          return 1;
        case PsdColorMode.Lab:
        case PsdColorMode.Rgb:
          return 3;
        case PsdColorMode.Cmyk:
          return 4;
      }

      throw new ArgumentException("Unknown color mode.");
    }

    /// <summary>
    /// Verify that the offset and count will remain within the bounds of the
    /// buffer.
    /// </summary>
    /// <returns>True if in bounds, false if out of bounds.</returns>
    public static Boolean CheckBufferBounds(Byte[] data, Int32 offset, Int32 count)
    {
      if (offset < 0)
        return false;
      if (count < 0)
        return false;
      if (offset + count > data.Length)
        return false;

      return true;
    }

    public static void CheckByteArrayLength(Int64 length)
    {
      if (length < 0)
      {
        throw new Exception("Byte array cannot have a negative length.");
      }
      if (length > 0x7fffffc7)
      {
        throw new OutOfMemoryException(
          "Byte array cannot exceed 2,147,483,591 in length.");
      }
    }

    /// <summary>
    /// Writes a message to the debug console, indicating the current position
    /// in the stream in both decimal and hexadecimal formats.
    /// </summary>
    [Conditional("DEBUG")]
    public static void DebugMessage(Stream stream, String message,
      params Object[] args)
    {
      var formattedMessage = String.Format(message, args);
      //Debug.WriteLine("0x{0:x}, {0}, {1}", stream.Position, formattedMessage);
    }
  }

  /// <summary>
  /// Fixed-point decimal, with 16-bit integer and 16-bit fraction.
  /// </summary>
  public class UFixed1616
  {
    public UInt16 Integer { get; set; }
    public UInt16 Fraction { get; set; }

    public UFixed1616(UInt16 integer, UInt16 fraction)
    {
      Integer = integer;
      Fraction = fraction;
    }

    /// <summary>
    /// Split the high and low words of a 32-bit unsigned integer into a
    /// fixed-point number.
    /// </summary>
    public UFixed1616(UInt32 value)
    {
      Integer = (UInt16)(value >> 16);
      Fraction = (UInt16)(value & 0x0000ffff);
    }

    public UFixed1616(Double value)
    {
      if (value >= 65536.0) throw new OverflowException();
      if (value < 0) throw new OverflowException();

      Integer = (UInt16)value;

      // Round instead of truncate, because doubles may not represent the
      // fraction exactly.
      Fraction = (UInt16)((value - Integer) * 65536 + 0.5);  
    }

    public static implicit operator Double(UFixed1616 value)
    {
      return (Double)value.Integer + value.Fraction / 65536.0;
    }

  }
  
}
