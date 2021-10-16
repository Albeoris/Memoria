using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class FF9SndMetaData
{
	public static void LoadBattleEncountBgmMetaData()
	{
		String path = "EmbeddedAsset/Manifest/Sounds/BtlEncountBgmMetaData.txt";
		FF9SndMetaData.CreateBattleEncountBgmMetaData(path, out FF9SndMetaData.BtlBgmMapperForFieldMap);
		String path2 = "EmbeddedAsset/Manifest/Sounds/WldBtlEncountBgmMetaData.txt";
		FF9SndMetaData.CreateBattleEncountBgmMetaData(path2, out FF9SndMetaData.BtlBgmMapperForWorldMap);
	}

	private static void CreateBattleEncountBgmMetaData(String path, out Dictionary<Int32, Dictionary<Int32, Int32>> mapper)
	{
		String[] bgmInfo;
		String textAsset = AssetManager.LoadString(path, out bgmInfo);
		mapper = new Dictionary<Int32, Dictionary<Int32, Int32>>();
		if (textAsset == null)
		{
			SoundLib.LogError("File not found AT path: " + path);
			return;
		}
		JSONClass jsonclass = (JSONClass)JSONNode.Parse(textAsset);
		foreach (String text in jsonclass.Dict.Keys)
		{
			Dictionary<Int32, Int32> dictionary = new Dictionary<Int32, Int32>();
			Int32 key = Convert.ToInt32(text);
			JSONClass jsonclass2 = (JSONClass)jsonclass[text];
			foreach (String text2 in jsonclass2.Dict.Keys)
			{
				Int32 key2 = Convert.ToInt32(text2);
				Int32 value = Convert.ToInt32(jsonclass2[text2]);
				dictionary.Add(key2, value);
			}
			mapper.Add(key, dictionary);
		}
	}

	public static Dictionary<Int32, Dictionary<Int32, Int32>> BtlBgmMapperForFieldMap;

	public static Dictionary<Int32, Dictionary<Int32, Int32>> BtlBgmMapperForWorldMap;
}
