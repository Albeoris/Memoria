using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public static class FF9SndMetaData
{
	public static Dictionary<Int32, Dictionary<Int32, Int32>> BtlBgmMapperForFieldMap;
	public static Dictionary<Int32, Dictionary<Int32, Int32>> BtlBgmMapperForWorldMap;
	public static Dictionary<Int32, Int32> BtlBgmPatcherMapper = new Dictionary<Int32, Int32>();

	public static void LoadBattleEncountBgmMetaData()
	{
		FF9SndMetaData.CreateBattleEncountBgmMetaData("EmbeddedAsset/Manifest/Sounds/BtlEncountBgmMetaData.txt", out FF9SndMetaData.BtlBgmMapperForFieldMap);
		FF9SndMetaData.CreateBattleEncountBgmMetaData("EmbeddedAsset/Manifest/Sounds/WldBtlEncountBgmMetaData.txt", out FF9SndMetaData.BtlBgmMapperForWorldMap);
	}

	public static Int32 GetMusicForBattle(Dictionary<Int32, Dictionary<Int32, Int32>> mapper, Int32 originMap, Int32 battleId)
	{
		if (BtlBgmPatcherMapper.TryGetValue(battleId, out Int32 defaultMusicId))
			return defaultMusicId;
		if (mapper.TryGetValue(originMap, out Dictionary<Int32, Int32> bgmDictionary))
			if (bgmDictionary.Count > 0 && bgmDictionary.TryGetValue(battleId, out Int32 musicId))
				return musicId;
		return -1;
	}

	private static void CreateBattleEncountBgmMetaData(String path, out Dictionary<Int32, Dictionary<Int32, Int32>> mapper)
	{
		String jsonFile = AssetManager.LoadString(path);
		mapper = new Dictionary<Int32, Dictionary<Int32, Int32>>();
		if (jsonFile == null)
		{
			SoundLib.LogError("File not found AT path: " + path);
			return;
		}
		JSONClass rootClass = (JSONClass)JSONNode.Parse(jsonFile);
		foreach (String originKey in rootClass.Dict.Keys)
		{
			Dictionary<Int32, Int32> battleBgmDict = new Dictionary<Int32, Int32>();
			Int32 originId = Convert.ToInt32(originKey);
			JSONClass battleClass = (JSONClass)rootClass[originKey];
			foreach (String battleKey in battleClass.Dict.Keys)
			{
				Int32 battleId = Convert.ToInt32(battleKey);
				Int32 musicId = Convert.ToInt32(battleClass[battleKey]);
				battleBgmDict.Add(battleId, musicId);
			}
			mapper.Add(originId, battleBgmDict);
		}
	}
}
