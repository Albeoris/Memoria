using System;
using System.IO;

public static class WMBinarayReaderExtension
{
	public static ff9.SVECTOR ReadSVECTOR(this BinaryReader reader)
	{
		return new ff9.SVECTOR
		{
			vx = reader.ReadInt16(),
			vy = reader.ReadInt16(),
			vz = reader.ReadInt16(),
			pad = reader.ReadInt16()
		};
	}

	public static ff9.sw_weatherColor ReadWeatherColor(this BinaryReader reader)
	{
		ff9.sw_weatherColor sw_weatherColor = new ff9.sw_weatherColor();
		for (Int32 i = 0; i < 23; i++)
		{
			ff9.sw_weatherColorElement sw_weatherColorElement = sw_weatherColor.Color[i];
			sw_weatherColorElement.light0 = reader.ReadSVECTOR();
			sw_weatherColorElement.light1 = reader.ReadSVECTOR();
			sw_weatherColorElement.light2 = reader.ReadSVECTOR();
			sw_weatherColorElement.light0c = reader.ReadSVECTOR();
			sw_weatherColorElement.ambient = reader.ReadSVECTOR();
			sw_weatherColorElement.ambientcl = reader.ReadSVECTOR();
			sw_weatherColorElement.goffsetup = reader.ReadUInt16();
			sw_weatherColorElement.toffsetup = reader.ReadUInt16();
			sw_weatherColorElement.fogUP = reader.ReadSVECTOR();
			sw_weatherColorElement.goffsetdw = reader.ReadUInt16();
			sw_weatherColorElement.toffsetdw = reader.ReadUInt16();
			sw_weatherColorElement.fogDW = reader.ReadSVECTOR();
			sw_weatherColorElement.goffsetcl = reader.ReadUInt16();
			sw_weatherColorElement.toffsetcl = reader.ReadUInt16();
			sw_weatherColorElement.fogCL = reader.ReadSVECTOR();
			sw_weatherColorElement.chrBIAS = reader.ReadSVECTOR();
			sw_weatherColorElement.fogAMP = reader.ReadUInt16();
			sw_weatherColor.Color[i] = sw_weatherColorElement;
		}
		return sw_weatherColor;
	}

	public static ff9.sNWBBlockHeader ReadNWBBlockHeader(this BinaryReader reader)
	{
		Int64 position = reader.BaseStream.Position;
		ff9.sNWBBlockHeader sNWBBlockHeader = new ff9.sNWBBlockHeader();
		sNWBBlockHeader.cellno = reader.ReadSByte();
		sNWBBlockHeader.timno = reader.ReadSByte();
		sNWBBlockHeader.areaid = reader.ReadSByte();
		sNWBBlockHeader.special = reader.ReadByte();
		sNWBBlockHeader.offsettexture = reader.ReadUInt32();
		sNWBBlockHeader.offsetcell = new UInt32[(Int32)sNWBBlockHeader.cellno];
		sNWBBlockHeader.cellHeader = new ff9.sNWBCellHeader[(Int32)sNWBBlockHeader.cellno];
		for (Int32 i = 0; i < (Int32)sNWBBlockHeader.cellno; i++)
		{
			sNWBBlockHeader.offsetcell[i] = reader.ReadUInt32();
		}
		for (Int32 j = 0; j < (Int32)sNWBBlockHeader.cellno; j++)
		{
			reader.BaseStream.Position = (Int64)((UInt64)sNWBBlockHeader.offsetcell[j] + (UInt64)position);
			sNWBBlockHeader.cellHeader[j] = reader.ReadNWBCellHeader();
		}
		return sNWBBlockHeader;
	}

	public static ff9.sNWBCellHeader ReadNWBCellHeader(this BinaryReader reader)
	{
		ff9.sNWBCellHeader sNWBCellHeader = new ff9.sNWBCellHeader();
		sNWBCellHeader.vtxno = reader.ReadByte();
		sNWBCellHeader.areaid = reader.ReadByte();
		sNWBCellHeader.surno = reader.ReadUInt16();
		sNWBCellHeader.offsetvertex = reader.ReadUInt32();
		sNWBCellHeader.offsetsurface = reader.ReadUInt32();
		for (Int32 i = 0; i < 20; i++)
		{
			sNWBCellHeader.checkpoint[i] = reader.ReadByte();
		}
		sNWBCellHeader.checkfig = reader.ReadByte();
		for (Int32 j = 0; j < 3; j++)
		{
			sNWBCellHeader.special[j] = reader.ReadByte();
		}
		return sNWBCellHeader;
	}

	public static EncountData[] ReadEncountData(this BinaryReader reader)
	{
		EncountData[] array = new EncountData[355];
		for (Int32 i = 0; i < 355; i++)
		{
			array[i] = new EncountData();
			for (Int32 j = 0; j < 4; j++)
			{
				array[i].scene[j] = reader.ReadUInt16();
			}
			array[i].pattern = reader.ReadByte();
			array[i].pad = reader.ReadByte();
		}
		return array;
	}

	public static ff9.stextureProject ReadTextureProjWork(this BinaryReader reader)
	{
		ff9.stextureProject stextureProject = new ff9.stextureProject();
		for (Int32 i = 0; i < 8; i++)
		{
			stextureProject.texturePixelScroll[i] = reader.ReadTexturePixelScroll();
		}
		for (Int32 j = 0; j < 4; j++)
		{
			stextureProject.texturePixelAnime[j] = reader.ReadTexturePixelAnime();
		}
		for (Int32 k = 0; k < 18; k++)
		{
			stextureProject.texturePaletScroll[k] = reader.ReadTexturePaletScroll();
		}
		return stextureProject;
	}

	public static ff9.stexturePixelScroll ReadTexturePixelScroll(this BinaryReader reader)
	{
		return new ff9.stexturePixelScroll
		{
			posx = reader.ReadUInt16(),
			posy = reader.ReadUInt16(),
			width = reader.ReadUInt16(),
			hight = reader.ReadUInt16(),
			speed = reader.ReadUInt16()
		};
	}

	public static ff9.stexturePixelAnime ReadTexturePixelAnime(this BinaryReader reader)
	{
		return new ff9.stexturePixelAnime
		{
			posx = reader.ReadUInt16(),
			posy = reader.ReadUInt16(),
			width = reader.ReadUInt16(),
			height = reader.ReadUInt16(),
			speed = reader.ReadUInt16()
		};
	}

	public static ff9.stexturePaletScroll ReadTexturePaletScroll(this BinaryReader reader)
	{
		return new ff9.stexturePaletScroll
		{
			posx = reader.ReadUInt16(),
			posy = reader.ReadUInt16(),
			offset = reader.ReadUInt16(),
			length = reader.ReadUInt16(),
			speed = reader.ReadUInt16()
		};
	}

	public static ff9.sworldEncountSpecial[] ReadWorldEncountSpecial(this BinaryReader reader)
	{
		ff9.sworldEncountSpecial[] array = new ff9.sworldEncountSpecial[9];
		for (Int32 i = 0; i < 9; i++)
		{
			array[i] = new ff9.sworldEncountSpecial();
			for (Int32 j = 0; j < 12; j++)
			{
				array[i].area[j] = reader.ReadUInt16();
			}
		}
		return array;
	}

	public static ff9.s_effectData ReadEffectData(this BinaryReader reader)
	{
		ff9.s_effectData s_effectData = new ff9.s_effectData();
		s_effectData.x = reader.ReadInt32();
		s_effectData.y = reader.ReadInt32();
		s_effectData.z = reader.ReadInt32();
		s_effectData.rx = reader.ReadInt32();
		s_effectData.ry = reader.ReadInt32();
		s_effectData.rz = reader.ReadInt32();
		s_effectData.vx = reader.ReadInt16();
		s_effectData.vy = reader.ReadInt16();
		s_effectData.vz = reader.ReadInt16();
		s_effectData.ax = reader.ReadInt16();
		s_effectData.ay = reader.ReadInt16();
		s_effectData.az = reader.ReadInt16();
		s_effectData.no = reader.ReadInt16();
		s_effectData.size = reader.ReadInt16();
		s_effectData.rnd = reader.ReadInt16();
		s_effectData.temp = reader.ReadInt16();
		for (Int32 i = 0; i < 5; i++)
		{
			s_effectData.pad[i] = reader.ReadInt32();
		}
		return s_effectData;
	}

	public static ff9.s_effectDataList ReadEffectDataList(this BinaryReader reader)
	{
		ff9.s_effectDataList s_effectDataList = new ff9.s_effectDataList();
		ff9.s_effectData s_effectData = reader.ReadEffectData();
		while (s_effectData.no != -1)
		{
			s_effectDataList.effectData.Add(s_effectData);
			s_effectData = reader.ReadEffectData();
		}
		return s_effectDataList;
	}
}
