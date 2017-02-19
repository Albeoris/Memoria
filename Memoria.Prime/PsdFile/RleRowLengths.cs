/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2014 Tao Yue
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;

namespace Memoria.Prime.PsdFile
{
  public class RleRowLengths
  {
    public Int32[] Values { get; private set; }

    public Int64 Total
    {
      get { return Values.Sum(x => (Int64)x); }
    }

    public Int32 this[Int32 i]
    {
      get { return Values[i]; }
      set { Values[i] = value; }
    }

    public RleRowLengths(Int32 rowCount)
    {
      Values = new Int32[rowCount];
    }

    public RleRowLengths(PsdBinaryReader reader, Int32 rowCount, Boolean isLargeDocument)
      : this(rowCount)
    {
      for (Int32 i = 0; i < rowCount; i++)
      {
        Values[i] = isLargeDocument
          ? reader.ReadInt32()
          : reader.ReadUInt16();
      }
    }

    public void Write(PsdBinaryWriter writer, Boolean isLargeDocument)
    {
      for (Int32 i = 0; i < Values.Length; i++)
      {
        if (isLargeDocument)
        {
          writer.Write(Values[i]);
        }
        else
        {
          writer.Write((UInt16)Values[i]);
        }
      }
    }
  }

}
