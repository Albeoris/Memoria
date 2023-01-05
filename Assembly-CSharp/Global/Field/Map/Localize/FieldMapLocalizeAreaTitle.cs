using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FieldMapLocalizeAreaTitle
{
	public FieldMapLocalizeAreaTitle()
	{
		this.dict = new Dictionary<String, FieldMapLocalizeAreaTitleInfo>();
	}

	public void Load()
	{
		String name = "EmbeddedAsset/Manifest/FieldMap/mapLocalizeAreaTitle.txt";
		String textAsset = AssetManager.LoadString(name);
		StringReader stringReader = new StringReader(textAsset);
		String text;
		while ((text = stringReader.ReadLine()) != null)
		{
			String[] array = text.Split(new Char[]
			{
				','
			});
			FieldMapLocalizeAreaTitleInfo fieldMapLocalizeAreaTitleInfo = new FieldMapLocalizeAreaTitleInfo();
			fieldMapLocalizeAreaTitleInfo.mapName = array[0];
			Int32.TryParse(array[1], out _); // fieldMapLocalizeAreaTitleInfo.atlasWidth
			Int32.TryParse(array[2], out _); // fieldMapLocalizeAreaTitleInfo.atlasHeight
			Int32.TryParse(array[3], out fieldMapLocalizeAreaTitleInfo.startOvrIdx);
			Int32.TryParse(array[4], out fieldMapLocalizeAreaTitleInfo.endOvrIdx);
			Int32 num = 0;
			Int32.TryParse(array[5], out num);
			fieldMapLocalizeAreaTitleInfo.hasUK = (num == 1);
			Int32 num2 = 6;
			Int32 num3 = 3;
			for (Int32 i = 0; i < 7; i++)
			{
				Int32 num4 = i * num3 + num2;
				Int32.TryParse(array[num4 + 1], out fieldMapLocalizeAreaTitleInfo.data[i * 2]);
				Int32.TryParse(array[num4 + 2], out fieldMapLocalizeAreaTitleInfo.data[i * 2 + 1]);
			}
			this.dict.Add(fieldMapLocalizeAreaTitleInfo.mapName, fieldMapLocalizeAreaTitleInfo);
		}
	}

	public FieldMapLocalizeAreaTitleInfo GetInfo(String mapName)
	{
		if (!this.dict.ContainsKey(mapName))
		{
			return (FieldMapLocalizeAreaTitleInfo)null;
		}
		return this.dict[mapName];
	}

	public Dictionary<String, FieldMapLocalizeAreaTitleInfo> dict;
}
