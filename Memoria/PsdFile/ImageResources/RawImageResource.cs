/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2013 Tao Yue
//

//
/////////////////////////////////////////////////////////////////////////////////

namespace Memoria.PSD
{
  /// <summary>
  /// Stores the raw data for unimplemented image resource types.
  /// </summary>
  public class RawImageResource : ImageResource
  {
    public byte[] Data { get; private set; }

    private ResourceID _id;
    public override ResourceID ID
    {
      get { return _id; }
    }

    public RawImageResource(ResourceID resourceId, string name)
      : base(name)
    {
      this._id = resourceId;
    }

    public RawImageResource(PsdBinaryReader reader, string signature, 
      ResourceID resourceId, string name, int numBytes)
      : base(name)
    {
      this.Signature = signature;
      this._id = resourceId;
      Data = reader.ReadBytes(numBytes);
    }
    
    protected override void WriteData(PsdBinaryWriter writer)
    {
      writer.Write(Data);
    }

  }
}
