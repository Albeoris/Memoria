/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2012 Tao Yue
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Memoria.Prime.PsdFile
{
  public class VersionInfo : ImageResource
  {
    public override ResourceID ID
    {
      get { return ResourceID.VersionInfo; }
    }

    public UInt32 Version { get; set; }

    public Boolean HasRealMergedData { get; set; }

    public String ReaderName { get; set; }

    public String WriterName { get; set; }

    public UInt32 FileVersion { get; set; }

    
    public VersionInfo() : base(String.Empty)
    {
    }

    public VersionInfo(PsdBinaryReader reader, String name)
      : base(name)
    {
      Version = reader.ReadUInt32();
      HasRealMergedData = reader.ReadBoolean();
      ReaderName = reader.ReadUnicodeString();
      WriterName = reader.ReadUnicodeString();
      FileVersion = reader.ReadUInt32();
    }

    protected override void WriteData(PsdBinaryWriter writer)
    {
      writer.Write(Version);
      writer.Write(HasRealMergedData);
      writer.WriteUnicodeString(ReaderName);
      writer.WriteUnicodeString(WriterName);
      writer.Write(FileVersion);
    }
  }
}
