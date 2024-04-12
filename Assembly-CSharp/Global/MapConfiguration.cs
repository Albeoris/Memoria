using System;
using System.IO;

public class MapConfiguration
{
	public static DMSMapConf? LoadMapConfigData(String fileName)
	{
		Byte[] binAsset = AssetManager.LoadBytes("CommonAsset/MapConfigData/" + fileName, true);
		if (binAsset == null)
			return null;
		MemoryStream memoryStream = new MemoryStream(binAsset);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		DMSMapConf value = MapConfiguration.DMSParseMapConfFile(binaryReader);
		binaryReader.Close();
		memoryStream.Close();
		return new DMSMapConf?(value);
	}

	public static DMSMapConf DMSParseMapConfFile(BinaryReader reader)
	{
		DMSMapConf result = default(DMSMapConf);
		result.attr = reader.ReadUInt16();
		result.version = reader.ReadUInt16();
		result.bgNo = reader.ReadUInt16();
		result.lightCount = reader.ReadByte();
		result.lightUse = reader.ReadByte();
		result.charCount = reader.ReadByte();
		result.charUse = reader.ReadByte();
		result.evtCount = reader.ReadByte();
		result.evtUse = reader.ReadByte();
		Int32 lightCount = (Int32)result.lightCount;
		result.DMSMapLight = new DMSMapLight[(Int32)result.lightCount];
		for (Int32 i = 0; i < lightCount; i++)
		{
			result.DMSMapLight[i].Read(reader);
		}
		Int32 charCount = (Int32)result.charCount;
		result.DMSMapChar = new DMSMapChar[(Int32)result.charCount];
		for (Int32 j = 0; j < charCount; j++)
		{
			result.DMSMapChar[j].Read(reader);
		}
		return result;
	}

	public static Int32 DMSGetMapConfSize(DMSMapConf? ObjPtr)
	{
		if (ObjPtr != null)
		{
			return 12 + 12 * MapConfiguration.DMSGetMapConfMember(ObjPtr.Value, DMSMapConfMember.DMS_MAPCONF_LIGHTCOUNT) + 8 * MapConfiguration.DMSGetMapConfMember(ObjPtr.Value, DMSMapConfMember.DMS_MAPCONF_CHARCOUNT) + 2 * MapConfiguration.DMSGetMapConfMember(ObjPtr.Value, DMSMapConfMember.DMS_MAPCONF_EVTCOUNT);
		}
		return -1;
	}

	public static Int32 DMSGetMapConfMember(DMSMapConf ObjPtr, DMSMapConfMember ObjMember)
	{
		switch (ObjMember)
		{
			case DMSMapConfMember.DMS_MAPCONF_ATTR:
				return (Int32)ObjPtr.attr;
			case DMSMapConfMember.DMS_MAPCONF_VERSION:
				return (Int32)ObjPtr.version;
			case DMSMapConfMember.DMS_MAPCONF_BGNO:
				return (Int32)ObjPtr.bgNo;
			case DMSMapConfMember.DMS_MAPCONF_LIGHTCOUNT:
				return (Int32)ObjPtr.lightCount;
			case DMSMapConfMember.DMS_MAPCONF_LIGHTUSE:
				return (Int32)ObjPtr.lightUse;
			case DMSMapConfMember.DMS_MAPCONF_CHARCOUNT:
				return (Int32)ObjPtr.charCount;
			case DMSMapConfMember.DMS_MAPCONF_CHARUSE:
				return (Int32)ObjPtr.charUse;
			case DMSMapConfMember.DMS_MAPCONF_EVTCOUNT:
				return (Int32)ObjPtr.evtCount;
			case DMSMapConfMember.DMS_MAPCONF_EVTUSE:
				return (Int32)ObjPtr.evtUse;
			default:
				global::Debug.LogError(new ArgumentNullException("Null DMSMapConfMember"));
				return -1;
		}
	}
}
