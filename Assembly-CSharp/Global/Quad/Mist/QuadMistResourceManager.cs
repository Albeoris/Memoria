using System;
using System.Collections.Generic;
using Memoria;
using Memoria.Assets;
using Memoria.Prime;
using UnityEngine;

public class QuadMistResourceManager : MonoBehaviour
{
	public static Boolean UseArrowGoldenFrame => Configuration.Mod.TranceSeek && Configuration.TetraMaster.TripleTriad <= 1;

	private void Start()
	{
		InitAtlasPath();
		LoadSprite();
		CreateUpScaleMapperData();
		CreateAssetData();
		CreateFontMap();
		QuadMistResourceManager.Instance = this;
	}

	private void InitAtlasPath()
	{
		String textAtlas = String.Empty;
		switch (Localization.CurrentLanguage)
		{
			case "English(US)":
				textAtlas = "quadmist_text_us";
				break;
			case "Japanese":
				textAtlas = "quadmist_text_jp";
				break;
			case "German":
				textAtlas = "quadmist_text_gr";
				break;
			case "Spanish":
				textAtlas = "quadmist_text_es";
				break;
			case "Italian":
				textAtlas = "quadmist_text_it";
				break;
			case "French":
				textAtlas = "quadmist_text_fr";
				break;
			case "English(UK)":
				textAtlas = "quadmist_text_uk";
				break;
		}
		atlasPathList[0] = "EmbeddedAsset/QuadMist/Atlas/quadmist_image0";
		atlasPathList[1] = "EmbeddedAsset/QuadMist/Atlas/quadmist_image1";
		atlasPathList[2] = "EmbeddedAsset/QuadMist/Atlas/" + textAtlas;
        atlasNameList[0] = "quadmist_image0";
		atlasNameList[1] = "quadmist_image1";
		atlasNameList[2] = textAtlas;
    }

	private void AddCenterTable(List<String> centerTable)
	{
		for (Int32 i = 1; i <= 24; i++)
			centerTable.Add($"bomb_{i:D2}.png");
		centerTable.Add("quadmist_dialog.png");
		centerTable.Add("quadmist_dialog_stock.png");
		centerTable.Add("quadmist_dialog_cardname.png");
		centerTable.Add("card_tray_bg.png");
		centerTable.Add("quadmist_dialog_getcard.png");
	}

	private void LoadSprite()
	{
		allAtlasSprites = new Dictionary<String, Dictionary<String, Sprite>>();
		for (Int32 i = 0; i < atlasPathList.Length; i++)
		{
			String atlasPath = atlasPathList[i];
			Sprite[] spriteArray = Resources.LoadAll<Sprite>(atlasPath);
			Dictionary<String, Sprite> atlasSprites = new Dictionary<String, Sprite>();
			List<String> excludeList = new List<String>();
			AddCenterTable(excludeList);
			for (Int32 j = 0; j < spriteArray.Length; j++)
			{
				Sprite sprite = spriteArray[j];
				if (excludeList.Contains(sprite.name))
				{
					// TODO: use moddedAtlas for these
					atlasSprites.Add(sprite.name, sprite);
				}
				else
				{
					Sprite spriteFromTexture = Sprite.Create(sprite.texture, sprite.rect, new Vector2(0f, 1f), 482f);
					atlasSprites.Add(sprite.name, spriteFromTexture);
				}
			}
			foreach (AssetManager.AssetFolder folder in AssetManager.FolderLowToHigh)
			{
				if (String.IsNullOrEmpty(folder.FolderPath))
					continue;
				if (folder.TryFindAssetInModOnDisc(atlasPath, out String fullPath, AssetManagerUtil.GetResourcesAssetsPath(true) + "/"))
					UIAtlas.ReadRawSpritesFromDisc(fullPath, atlasSprites, excludeList);
			}
			allAtlasSprites.Add(atlasPath, atlasSprites);
		}
	}

	private void CreateUpScaleMapperData()
	{
		mapperData = new Dictionary<String, List<QuadMistResourceManager.QuadMistMapperData>>()
		{
			{ "BattleNum", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("card_digit_0.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_digit_1.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_digit_2.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_digit_3.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_digit_4.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_digit_5.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_digit_6.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_digit_7.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_digit_8.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_digit_9.png")
				}
			},
			{ "CardArrow", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("card_arrow_top.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_arrow_righttop.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_arrow_right.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_arrow_rightbuttom.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_arrow_buttom.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_arrow_leftbuttom.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_arrow_left.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_arrow_lefttop.png")
				}
			},
			{ "CardBackground", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("card_player_bg.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_opponent_bg.png"),
					new QuadMistResourceManager.QuadMistMapperData("block_a.png"),
					new QuadMistResourceManager.QuadMistMapperData("block_b.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_back.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_player_frame.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_opponent_frame.png"),
					new QuadMistResourceManager.QuadMistMapperData("goldenbluecardframe"),
					new QuadMistResourceManager.QuadMistMapperData("goldenredcardframe"),
                    new QuadMistResourceManager.QuadMistMapperData("fire_element"),
                    new QuadMistResourceManager.QuadMistMapperData("ice_element")
                }
			},
			{ "CardBlock", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("block_a.png"),
					new QuadMistResourceManager.QuadMistMapperData("block_b.png")
				}
			},
			{ "CardSelect", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("text_select.png")
				}
			},
			{ "CursorPreBoard", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("cursor_hand_choice.png")
				}
			},
			{ "GetCardMessage", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("card_notice_new.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_notice_last.png")
				}
			},
			{ "LRButton", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("button_previous.png"),
					new QuadMistResourceManager.QuadMistMapperData("button_next.png")
				}
			},
			{ "PreBoardTitle", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("text_card.png"),
					new QuadMistResourceManager.QuadMistMapperData("text_selection.png")
				}
			},
			{ "Result", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("card_win.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_lose.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_draw.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_perfect.png"),
					new QuadMistResourceManager.QuadMistMapperData("sametripletriad"),
					new QuadMistResourceManager.QuadMistMapperData("plustripletriad"),
					new QuadMistResourceManager.QuadMistMapperData("combotripletriad")
				}
			},
			{ "ResultShadow", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("card_win_shadow.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_lose_shadow.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_draw_shadow.png"),
					new QuadMistResourceManager.QuadMistMapperData("card_perfect_shadow.png"),
					new QuadMistResourceManager.QuadMistMapperData("sametripletriad_shadow"),
					new QuadMistResourceManager.QuadMistMapperData("plustripletriad_shadow"),
					new QuadMistResourceManager.QuadMistMapperData("combotripletriad_shadow")
				}
			},
			{ "ScoreDivider", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("arrow.png")
				}
			},
			{ "CardNameToggle", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("text_name.png"),
					new QuadMistResourceManager.QuadMistMapperData("text_name_hilight.png")
				}
			},
			{ "Background", new List<QuadMistMapperData>()
				{
					new QuadMistResourceManager.QuadMistMapperData("card_bg.png"),
					new QuadMistResourceManager.QuadMistMapperData(Localization.CurrentLanguage == "Japanese" ? "card_mg_jp.png" : "card_mg.png")
				}
			},
        };
		List<QuadMistResourceManager.QuadMistMapperData> cardIconCounter = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 i = 0; i <= 9; i++)
			cardIconCounter.Add(new QuadMistResourceManager.QuadMistMapperData($"card_digit_total_{i}.png"));
		mapperData["CardIconCounter"] = cardIconCounter;
		List<QuadMistResourceManager.QuadMistMapperData> cardStat = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 i = 0; i <= 9; i++)
			cardStat.Add(new QuadMistResourceManager.QuadMistMapperData($"card_point_{i}.png"));
		cardStat.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_a.png"));
		cardStat.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_b.png"));
		cardStat.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_c.png"));
		cardStat.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_d.png"));
		cardStat.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_e.png"));
		cardStat.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_f.png"));
		for (Int32 i = 0; i < 6; i++)
			cardStat.Add(new QuadMistResourceManager.QuadMistMapperData(String.Empty));
		cardStat.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_m.png"));
		for (Int32 i = 0; i < 2; i++)
			cardStat.Add(new QuadMistResourceManager.QuadMistMapperData(String.Empty));
		cardStat.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_p.png"));
		for (Int32 i = 0; i < 7; i++)
			cardStat.Add(new QuadMistResourceManager.QuadMistMapperData(String.Empty));
		cardStat.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_x.png"));
		for (Int32 i = 0; i < 2; i++)
			cardStat.Add(new QuadMistResourceManager.QuadMistMapperData(String.Empty));
        mapperData["CardStat"] = cardStat;
        List<QuadMistResourceManager.QuadMistMapperData> combo = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 i = 0; i <= 9; i++)
			combo.Add(new QuadMistResourceManager.QuadMistMapperData($"text_combo_{i}.png"));
		mapperData["Combo"] = combo;
		List<QuadMistResourceManager.QuadMistMapperData> enemyScore = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 i = 0; i <= 10; i++)
			enemyScore.Add(new QuadMistResourceManager.QuadMistMapperData($"card_score_digit_{i}_red.png"));
		mapperData["EnemyScore"] = enemyScore;
		List<QuadMistResourceManager.QuadMistMapperData> playerScore = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 i = 0; i <= 10; i++)
			playerScore.Add(new QuadMistResourceManager.QuadMistMapperData($"card_score_digit_{i}_blue.png"));
		mapperData["PlayerScore"] = playerScore;
		List<QuadMistResourceManager.QuadMistMapperData> card = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 i = 0; i <= 99; i++)
			card.Add(new QuadMistResourceManager.QuadMistMapperData($"card_{i:D2}.png"));
		mapperData["Card"] = card;
		List<QuadMistResourceManager.QuadMistMapperData> cardIcon = new List<QuadMistResourceManager.QuadMistMapperData>();
		cardIcon.Add(new QuadMistResourceManager.QuadMistMapperData("card_slot.png"));
		for (Int32 i = 0; i <= 6; i++)
			cardIcon.Add(new QuadMistResourceManager.QuadMistMapperData($"card_type{i}_normal.png"));
		for (Int32 i = 0; i <= 6; i++)
			cardIcon.Add(new QuadMistResourceManager.QuadMistMapperData($"card_type{i}_select.png"));
		mapperData["CardIcon"] = cardIcon;
		List<QuadMistResourceManager.QuadMistMapperData> coin = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 i = 8; i >= 1; i--)
			coin.Add(new QuadMistResourceManager.QuadMistMapperData($"coin_0{i}.png"));
		mapperData["Coin"] = coin;
		List<QuadMistResourceManager.QuadMistMapperData> cursor = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 i = 0; i <= 6; i++)
			cursor.Add(new QuadMistResourceManager.QuadMistMapperData($"card_cursor_{i}.png"));
		mapperData["Cursor"] = cursor;
		List<QuadMistResourceManager.QuadMistMapperData> explosion = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 i = 1; i <= 14; i++)
			explosion.Add(new QuadMistResourceManager.QuadMistMapperData($"bomb_{i:D2}.png"));
		mapperData["Explosion"] = explosion;
	}

	private void CreateAssetData()
	{
		assetData = new Dictionary<String, List<QuadMistAssetData>>();
		foreach (KeyValuePair<String, List<QuadMistResourceManager.QuadMistMapperData>> keyValuePair in mapperData)
		{
			List<QuadMistAssetData> list = new List<QuadMistAssetData>();
			for (Int32 i = 0; i < keyValuePair.Value.Count; i++)
			{
				QuadMistResourceManager.QuadMistMapperData quadMistMapperData = keyValuePair.Value[i];
				QuadMistAssetData quadMistAssetData = null;
				for (Int32 j = 0; j < 3; j++)
					if (!String.IsNullOrEmpty(quadMistMapperData.SpriteName) && allAtlasSprites.TryGetValue(atlasPathList[j], out Dictionary<String, Sprite> altasSprites) && altasSprites.TryGetValue(quadMistMapperData.SpriteName, out Sprite sprite))
                        quadMistAssetData = new QuadMistAssetData(keyValuePair.Key, i, sprite);
				if (quadMistAssetData == null)
					quadMistAssetData = new QuadMistAssetData(String.Empty, i, null);
                list.Add(quadMistAssetData);
//				Log.Message("keyValuePair.Key = " + keyValuePair.Key + " / quadMistMapperData.SpriteName = " + quadMistMapperData.SpriteName);
			}
			assetData.Add(keyValuePair.Key, list);
		}
	}

	private void CreateFontMap()
	{
		const Int32 zeroChar = '0';
		fontMap = new Dictionary<String, Dictionary<Char, Int32>>();
		Dictionary<Char, Int32> battleNum = new Dictionary<Char, Int32>();
		for (Int32 i = 0; i <= 9; i++)
			battleNum.Add((Char)(i + zeroChar), i);
		fontMap.Add("BattleNum", battleNum);
		Dictionary<Char, Int32> cardIcon = new Dictionary<Char, Int32>();
		for (Int32 i = 0; i <= 9; i++)
			cardIcon.Add((Char)(i + zeroChar), i);
		fontMap.Add("CardIconCounter", cardIcon);
		Dictionary<Char, Int32> cardStat = new Dictionary<Char, Int32>();
		for (Int32 i = 0; i <= 9; i++)
			cardStat.Add((Char)(i + zeroChar), i);
		cardStat.Add('a', 10);
		cardStat.Add('b', 11);
		cardStat.Add('c', 12);
		cardStat.Add('d', 13);
		cardStat.Add('e', 14);
		cardStat.Add('f', 15);
		cardStat.Add('m', 22);
		cardStat.Add('p', 25);
		cardStat.Add('x', 33);
		fontMap.Add("CardStat", cardStat);
		Dictionary<Char, Int32> combo = new Dictionary<Char, Int32>();
		for (Int32 i = 0; i <= 9; i++)
			combo.Add((Char)(i + zeroChar), i);
		fontMap.Add("Combo", combo);
		Dictionary<Char, Int32> enemyScore = new Dictionary<Char, Int32>();
		for (Int32 i = 0; i <= 9; i++)
			enemyScore.Add((Char)(i + zeroChar), i);
		enemyScore.Add('a', 10);
		fontMap.Add("EnemyScore", enemyScore);
		Dictionary<Char, Int32> playerScore = new Dictionary<Char, Int32>();
		for (Int32 i = 0; i <= 9; i++)
			playerScore.Add((Char)(i + zeroChar), i);
		playerScore.Add('a', 10);
		fontMap.Add("PlayerScore", playerScore);
	}

	public QuadMistAssetData GetFont(String sheetAssetName, Char character)
	{
		return GetResource(sheetAssetName, fontMap[sheetAssetName][character]);
	}

	public QuadMistAssetData GetResource(String sheetAssetName, Int32 spriteIndex)
	{
		return assetData[sheetAssetName][spriteIndex];
	}

	public Sprite GetSprite(String spriteName)
	{
		foreach (KeyValuePair<String, Dictionary<String, Sprite>> keyValuePair in allAtlasSprites)
			if (keyValuePair.Value.TryGetValue(spriteName, out Sprite result))
                return result;				
        return null;
	}

    private Dictionary<String, Dictionary<String, Sprite>> allAtlasSprites;
	private Dictionary<String, List<QuadMistResourceManager.QuadMistMapperData>> mapperData;
	private Dictionary<String, List<QuadMistAssetData>> assetData;
	private Dictionary<String, Dictionary<Char, Int32>> fontMap;

	public static QuadMistResourceManager Instance;

	private String[] atlasPathList = new String[]
	{
		"EmbeddedAsset/QuadMist/Atlas/quadmist_image0",
		"EmbeddedAsset/QuadMist/Atlas/quadmist_image1",
		"EmbeddedAsset/QuadMist/Atlas/quadmist_text_us"
	};

	private String[] atlasNameList = new String[]
	{
		"quadmist_image0",
		"quadmist_image1",
		"quadmist_text_us"
	};

	private class QuadMistMapperData
	{
		public QuadMistMapperData(String spriteName)
		{
			SpriteName = spriteName;
		}

		public String SpriteName;
	}
}
