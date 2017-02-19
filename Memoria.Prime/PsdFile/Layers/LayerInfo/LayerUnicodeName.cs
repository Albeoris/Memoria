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

namespace Memoria.Prime.PsdFile
{
  public class LayerUnicodeName : LayerInfo
  {
    public override String Signature
    {
      get { return "8BIM"; }
    }

    public override String Key
    {
      get { return "luni"; }
    }

    public String Name { get; set; }

    public LayerUnicodeName(String name)
    {
      Name = name;
    }

    public LayerUnicodeName(PsdBinaryReader reader)
    {
      Name = reader.ReadUnicodeString();
    }

    protected override void WriteData(PsdBinaryWriter writer)
    {
      var startPosition = writer.BaseStream.Position;

      writer.WriteUnicodeString(Name);
      writer.WritePadding(startPosition, 4);
    }
  }
}
