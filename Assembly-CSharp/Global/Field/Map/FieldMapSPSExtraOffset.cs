using System;
using System.Collections.Generic;
using System.IO;

public class FieldMapSPSExtraOffset
{
	public void Load()
	{
		this.offsetDict = new Dictionary<String, FieldMapSPSExtraOffset.SPSExtraOffset[]>();
		String name = "EmbeddedAsset/Manifest/FieldMap/mapSPSExtraOffsetList.txt";
		String textAsset = AssetManager.LoadString(name);
		StringReader stringReader = new StringReader(textAsset);
		String text;
		while ((text = stringReader.ReadLine()) != null)
		{
			String[] array = text.Split(new Char[]
			{
				','
			});
			String key = array[0];
			Int32 num = 0;
			Int32.TryParse(array[1], out num);
			if (num > 0)
			{
				FieldMapSPSExtraOffset.SPSExtraOffset[] array2 = new FieldMapSPSExtraOffset.SPSExtraOffset[num];
				for (Int32 i = 0; i < num; i++)
				{
					array2[i] = new FieldMapSPSExtraOffset.SPSExtraOffset();
					Int32.TryParse(array[i * 2 + 2], out array2[i].spsIndex);
					Int32.TryParse(array[i * 2 + 3], out array2[i].zOffset);
				}
				this.offsetDict.Add(key, array2);
			}
		}
	}

	public void SetSPSOffset(String name, List<FieldSPS> spsList)
	{
		if (this.offsetDict.ContainsKey(name))
		{
			FieldMapSPSExtraOffset.SPSExtraOffset[] array = this.offsetDict[name];
			for (Int32 i = 0; i < (Int32)array.Length; i++)
			{
				FieldMapSPSExtraOffset.SPSExtraOffset spsextraOffset = array[i];
				if (spsextraOffset.spsIndex >= 0)
				{
					spsList[spsextraOffset.spsIndex].zOffset = spsextraOffset.zOffset;
				}
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
