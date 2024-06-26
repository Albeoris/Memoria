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
  /// <summary>
  /// Layers that are stored as Additional Info, rather than in the main
  /// Layers section of the PSD file.
  /// </summary>
  /// <remarks>
  /// Photoshop stores layers in the Additional Info section for 16-bit and
  /// 32-bit depth images.  The Layers section in the PSD file is left empty.
  /// 
  /// This appears to be for backward-compatibility purposes, but it is not
  /// required.  Photoshop will successfully load a high-bitdepth image that
  /// puts the layers in the Layers section.
  /// </remarks>
  public class InfoLayers : LayerInfo
  {
    public override String Signature
    {
      get { return "8BIM"; }
    }

      public override String Key { get; }

      public PsdFile PsdFile { get; set; }

    public InfoLayers(PsdFile psdFile, String key)
    {
      PsdFile = psdFile;

      switch (key)
      {
        // The key does not have to match the bit depth, but it does have to
        // be one of the known values.
        case "Layr":
        case "Lr16":
        case "Lr32":
          Key = key;
          break;
        default:
          throw new PsdInvalidException(
            $"{nameof(InfoLayers)} key must be Layr, Lr16, or Lr32.");
      }
    }

    public InfoLayers(PsdBinaryReader reader, PsdFile psdFile,
      String key, Int64 dataLength)
      : this(psdFile, key)
    {
      if (psdFile.Layers.Count > 0)
      {
        throw new PsdInvalidException(
          "Cannot have both regular layers and Additional Info layers");
      }

      var endPosition = reader.BaseStream.Position + dataLength;
      psdFile.LoadLayers(reader, false);

      if (reader.BaseStream.Position != endPosition)
      {
        throw new PsdInvalidException(
          $"Incorrect length for {nameof(InfoLayers)}.");
      }
    }

    protected override void WriteData(PsdBinaryWriter writer)
    {
      PsdFile.SaveLayersData(writer);
    }
  }
}
