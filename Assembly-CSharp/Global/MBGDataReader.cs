using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MBGDataReader
{
	public MBGDataReader(BinaryReader binary)
	{
		this.frameCount = (UInt32)binary.ReadInt32();
		Int32 num = 1;
		while ((Int64)num <= (Int64)((UInt64)(this.frameCount * 2u)))
		{
			if (binary.BaseStream.Position < binary.BaseStream.Length)
			{
				binary.BaseStream.Seek((Int64)(4 + num * 100), SeekOrigin.Begin);
				AUDIO_HDR_DEF item = default(AUDIO_HDR_DEF);
				item.ExtractHeaderData(binary);
				this.audioRefList.Add(item);
			}
			num += 2;
		}
		binary.Close();
	}

	public static MBGDataReader Load(String binaryName)
	{
		Byte[] binAsset = AssetManager.LoadBytes("CommonAsset/MBGData/" + binaryName + ".mbgdata");
		if (binAsset == null)
			return null;
		return new MBGDataReader(new BinaryReader(new MemoryStream(binAsset)));
	}

	public MBG_CAM_DEF GetMBGCam(Int32 frameIndex)
	{
		if (this.audioRefList.Count > frameIndex)
		{
			return this.audioRefList[frameIndex].mbgCameraA;
		}
		return this.audioRefList[this.audioRefList.Count - 1].mbgCameraA;
	}

	private const Int32 CDXA_SECTOR_SIZE = 2352;

	public List<AUDIO_HDR_DEF> audioRefList = new List<AUDIO_HDR_DEF>();

	private UInt32 frameCount;
}
