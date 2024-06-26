using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FieldMapSPSExtraOffset
{
    public void Load()
    {
        this.offsetDict = new Dictionary<String, FieldMapSPSExtraOffset.SPSExtraOffset[]>();
        String name = "EmbeddedAsset/Manifest/FieldMap/mapSPSExtraOffsetList.txt";
        String textAsset = AssetManager.LoadString(name);
        StringReader stringReader = new StringReader(textAsset);
        String line;
        while ((line = stringReader.ReadLine()) != null)
        {
            String[] entries = line.Split(',');
            String key = entries[0];
            Int32.TryParse(entries[1], out Int32 offsetCount);
            if (offsetCount > 0)
            {
                FieldMapSPSExtraOffset.SPSExtraOffset[] offsetList = new FieldMapSPSExtraOffset.SPSExtraOffset[offsetCount];
                for (Int32 i = 0; i < offsetCount; i++)
                {
                    offsetList[i] = new FieldMapSPSExtraOffset.SPSExtraOffset();
                    Int32.TryParse(entries[i * 2 + 2], out offsetList[i].spsIndex);
                    Int32.TryParse(entries[i * 2 + 3], out offsetList[i].zOffset);
                }
                this.offsetDict.Add(key, offsetList);
            }
        }
    }

    public void SetSPSOffset(String name, List<SPSEffect> spsList)
    {
        if (this.offsetDict.ContainsKey(name))
        {
            FieldMapSPSExtraOffset.SPSExtraOffset[] offsetList = this.offsetDict[name];
            for (Int32 i = 0; i < offsetList.Length; i++)
            {
                FieldMapSPSExtraOffset.SPSExtraOffset spsextraOffset = offsetList[i];
                if (spsextraOffset.spsIndex >= 0)
                    spsList[spsextraOffset.spsIndex].zOffset = spsextraOffset.zOffset;
            }
        }
    }

    public Dictionary<String, FieldMapSPSExtraOffset.SPSExtraOffset[]> offsetDict;

    public class SPSExtraOffset
    {
        public Int32 spsIndex;
        public Int32 zOffset;
    }
}
