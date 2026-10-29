using System;
using System.IO;
using System.Collections.Generic;

public class MapConfiguration
{
    public static DMSMapConf LoadMapConfigData(String fileName)
    {
        Byte[] binAsset = AssetManager.LoadBytes("CommonAsset/MapConfigData/" + fileName, true);
        if (binAsset == null)
            return null;
        MemoryStream memoryStream = new MemoryStream(binAsset);
        BinaryReader binaryReader = new BinaryReader(memoryStream);
        DMSMapConf lightConfig = MapConfiguration.DMSParseMapConfFile(binaryReader);
        binaryReader.Close();
        memoryStream.Close();
        return lightConfig;
    }

    public static DMSMapConf DMSParseMapConfFile(BinaryReader reader)
    {
        DMSMapConf result = new DMSMapConf();
        result.attr = reader.ReadUInt16();
        result.version = reader.ReadUInt16();
        result.bgNo = reader.ReadUInt16();
        result.lightCount = reader.ReadByte();
        result.lightUse = reader.ReadByte();
        result.charCount = reader.ReadByte();
        result.charUse = reader.ReadByte();
        result.evtCount = reader.ReadByte();
        result.evtUse = reader.ReadByte();
        result.DMSMapLight = new List<DMSMapLight>(result.lightCount);
        result.DMSMapChar = new List<DMSMapChar>(result.charCount);
        for (Int32 i = 0; i < result.lightCount; i++)
        {
            DMSMapLight light = new DMSMapLight();
            light.Read(reader);
            result.DMSMapLight.Add(light);
        }
        for (Int32 i = 0; i < result.charCount; i++)
        {
            DMSMapChar light = new DMSMapChar();
            light.Read(reader);
            result.DMSMapChar.Add(light);
        }
        return result;
    }

    public static Int32 DMSGetMapConfSize(DMSMapConf lightConfig)
    {
        if (lightConfig != null)
            return 12 + 12 * MapConfiguration.DMSGetMapConfMember(lightConfig, DMSMapConfMember.DMS_MAPCONF_LIGHTCOUNT) + 8 * MapConfiguration.DMSGetMapConfMember(lightConfig, DMSMapConfMember.DMS_MAPCONF_CHARCOUNT) + 2 * MapConfiguration.DMSGetMapConfMember(lightConfig, DMSMapConfMember.DMS_MAPCONF_EVTCOUNT);
        return -1;
    }

    public static Int32 DMSGetMapConfMember(DMSMapConf lightConfig, DMSMapConfMember ObjMember)
    {
        switch (ObjMember)
        {
            case DMSMapConfMember.DMS_MAPCONF_ATTR:
                return (Int32)lightConfig.attr;
            case DMSMapConfMember.DMS_MAPCONF_VERSION:
                return (Int32)lightConfig.version;
            case DMSMapConfMember.DMS_MAPCONF_BGNO:
                return (Int32)lightConfig.bgNo;
            case DMSMapConfMember.DMS_MAPCONF_LIGHTCOUNT:
                return (Int32)lightConfig.lightCount;
            case DMSMapConfMember.DMS_MAPCONF_LIGHTUSE:
                return (Int32)lightConfig.lightUse;
            case DMSMapConfMember.DMS_MAPCONF_CHARCOUNT:
                return (Int32)lightConfig.charCount;
            case DMSMapConfMember.DMS_MAPCONF_CHARUSE:
                return (Int32)lightConfig.charUse;
            case DMSMapConfMember.DMS_MAPCONF_EVTCOUNT:
                return (Int32)lightConfig.evtCount;
            case DMSMapConfMember.DMS_MAPCONF_EVTUSE:
                return (Int32)lightConfig.evtUse;
            default:
                global::Debug.LogError(new ArgumentNullException("Null DMSMapConfMember"));
                return -1;
        }
    }
}
