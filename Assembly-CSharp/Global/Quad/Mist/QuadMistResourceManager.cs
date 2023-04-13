using System;
using System.Collections.Generic;
using Memoria.Assets;
using UnityEngine;

public class QuadMistResourceManager : MonoBehaviour
{
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
		String text = String.Empty;
		String language = Localization.CurrentLanguage;
		switch (language)
		{
		case "English(US)":
			text = "quadmist_text_us";
			break;
		case "Japanese":
			text = "quadmist_text_jp";
			break;
		case "German":
			text = "quadmist_text_gr";
			break;
		case "Spanish":
			text = "quadmist_text_es";
			break;
		case "Italian":
			text = "quadmist_text_it";
			break;
		case "French":
			text = "quadmist_text_fr";
			break;
		case "English(UK)":
			text = "quadmist_text_uk";
			break;
		}
		atlasPathList[0] = "EmbeddedAsset/QuadMist/Atlas/quadmist_image0";
		atlasPathList[1] = "EmbeddedAsset/QuadMist/Atlas/quadmist_image1";
		atlasPathList[2] = "EmbeddedAsset/QuadMist/Atlas/" + text;
		atlasNameList[0] = "quadmist_image0";
		atlasNameList[1] = "quadmist_image1";
		atlasNameList[2] = text;
	}

	private void AddCenterTable(List<String> centerTable)
	{
		for (Int32 i = 1; i <= 9; i++)
		{
			centerTable.Add("bomb_0" + i + ".png");
		}
		for (Int32 j = 10; j <= 24; j++)
		{
			centerTable.Add("bomb_" + j + ".png");
		}
		centerTable.Add("quadmist_dialog.png");
		centerTable.Add("quadmist_dialog_stock.png");
		centerTable.Add("quadmist_dialog_cardname.png");
		centerTable.Add("card_tray_bg.png");
		centerTable.Add("quadmist_dialog_getcard.png");
	}

	private void LoadSprite()
	{
		spriteData = new Dictionary<String, Dictionary<String, Sprite>>();
		String[] atlasArray = atlasPathList;
		for (Int32 i = 0; i < (Int32)atlasArray.Length; i++)
		{
			String text = atlasArray[i];
			Sprite[] spriteArray = Resources.LoadAll<Sprite>(text);
			Texture2D moddedAtlas = null;
			String atlasOnDisc = AssetManager.SearchAssetOnDisc(text, true, false);
			if (!String.IsNullOrEmpty(atlasOnDisc))
			{
                moddedAtlas = AssetManager.LoadFromDisc<Texture2D>(atlasOnDisc, "");
            }
            Dictionary<String, Sprite> dictionary = new Dictionary<String, Sprite>();
			List<String> list = new List<String>();
			AddCenterTable(list);
			for (Int32 j = 0; j < spriteArray.Length; j++)
			{
				Sprite sprite = spriteArray[j];
				if (list.Contains(sprite.name))
				{
					// Todo: use moddedAtlas for these
					dictionary.Add(sprite.name, sprite);
				}
				else
				{
					Sprite value = Sprite.Create(moddedAtlas != null ? moddedAtlas : sprite.texture, sprite.rect, new Vector2(0f, 1f), 482f);
					dictionary.Add(sprite.name, value);
				}
			}
			spriteData.Add(text, dictionary);
		}
	}

    private void CreateUpScaleMapperData()
	{
		mapperData = new Dictionary<String, List<QuadMistResourceManager.QuadMistMapperData>>();
		if (mapperData.ContainsKey("BattleNum"))
		{
			mapperData.Remove("BattleNum");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list = new List<QuadMistResourceManager.QuadMistMapperData>();
		list.Add(new QuadMistResourceManager.QuadMistMapperData("card_digit_0.png"));
		list.Add(new QuadMistResourceManager.QuadMistMapperData("card_digit_1.png"));
		list.Add(new QuadMistResourceManager.QuadMistMapperData("card_digit_2.png"));
		list.Add(new QuadMistResourceManager.QuadMistMapperData("card_digit_3.png"));
		list.Add(new QuadMistResourceManager.QuadMistMapperData("card_digit_4.png"));
		list.Add(new QuadMistResourceManager.QuadMistMapperData("card_digit_5.png"));
		list.Add(new QuadMistResourceManager.QuadMistMapperData("card_digit_6.png"));
		list.Add(new QuadMistResourceManager.QuadMistMapperData("card_digit_7.png"));
		list.Add(new QuadMistResourceManager.QuadMistMapperData("card_digit_8.png"));
		list.Add(new QuadMistResourceManager.QuadMistMapperData("card_digit_9.png"));
		mapperData.Add("BattleNum", list);
		if (mapperData.ContainsKey("CardIconCounter"))
		{
			mapperData.Remove("CardIconCounter");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list2 = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 i = 0; i <= 9; i++)
		{
			list2.Add(new QuadMistResourceManager.QuadMistMapperData("card_digit_total_" + i + ".png"));
		}
		mapperData.Add("CardIconCounter", list2);
		if (mapperData.ContainsKey("CardStat"))
		{
			mapperData.Remove("CardStat");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list3 = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 j = 0; j <= 9; j++)
		{
			list3.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_" + j + ".png"));
		}
		list3.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_a.png"));
		list3.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_b.png"));
		list3.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_c.png"));
		list3.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_d.png"));
		list3.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_e.png"));
		list3.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_f.png"));
		for (Int32 k = 0; k < 6; k++)
		{
			list3.Add(new QuadMistResourceManager.QuadMistMapperData(String.Empty));
		}
		list3.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_m.png"));
		for (Int32 l = 0; l < 2; l++)
		{
			list3.Add(new QuadMistResourceManager.QuadMistMapperData(String.Empty));
		}
		list3.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_p.png"));
		for (Int32 m = 0; m < 7; m++)
		{
			list3.Add(new QuadMistResourceManager.QuadMistMapperData(String.Empty));
		}
		list3.Add(new QuadMistResourceManager.QuadMistMapperData("card_point_x.png"));
		for (Int32 n = 0; n < 2; n++)
		{
			list3.Add(new QuadMistResourceManager.QuadMistMapperData(String.Empty));
		}
		mapperData.Add("CardStat", list3);
		if (mapperData.ContainsKey("Combo"))
		{
			mapperData.Remove("Combo");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list4 = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 num = 0; num <= 9; num++)
		{
			list4.Add(new QuadMistResourceManager.QuadMistMapperData("text_combo_" + num + ".png"));
		}
		mapperData.Add("Combo", list4);
		if (mapperData.ContainsKey("EnemyScore"))
		{
			mapperData.Remove("EnemyScore");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list5 = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 num2 = 0; num2 <= 10; num2++)
		{
			list5.Add(new QuadMistResourceManager.QuadMistMapperData("card_score_digit_" + num2 + "_red.png"));
		}
		mapperData.Add("EnemyScore", list5);
		if (mapperData.ContainsKey("PlayerScore"))
		{
			mapperData.Remove("PlayerScore");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list6 = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 num3 = 0; num3 <= 10; num3++)
		{
			list6.Add(new QuadMistResourceManager.QuadMistMapperData("card_score_digit_" + num3 + "_blue.png"));
		}
		mapperData.Add("PlayerScore", list6);
		if (mapperData.ContainsKey("Background"))
		{
			mapperData.Remove("Background");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list7 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list7.Add(new QuadMistResourceManager.QuadMistMapperData("card_bg.png"));
		if (Localization.CurrentLanguage == "Japanese")
		{
			list7.Add(new QuadMistResourceManager.QuadMistMapperData("card_mg_jp.png"));
		}
		else
		{
			list7.Add(new QuadMistResourceManager.QuadMistMapperData("card_mg.png"));
		}
		mapperData.Add("Background", list7);
		if (mapperData.ContainsKey("Card"))
		{
			mapperData.Remove("Card");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list8 = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 num4 = 0; num4 <= 9; num4++)
		{
			list8.Add(new QuadMistResourceManager.QuadMistMapperData("card_0" + num4 + ".png"));
		}
		for (Int32 num5 = 10; num5 <= 65; num5++)
		{
			list8.Add(new QuadMistResourceManager.QuadMistMapperData("card_" + num5 + ".png"));
		}
		for (Int32 num6 = 66; num6 <= 99; num6++)
		{
			list8.Add(new QuadMistResourceManager.QuadMistMapperData("card_" + num6 + ".png"));
		}
		mapperData.Add("Card", list8);
		if (mapperData.ContainsKey("CardArrow"))
		{
			mapperData.Remove("CardArrow");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list9 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list9.Add(new QuadMistResourceManager.QuadMistMapperData("card_arrow_top.png"));
		list9.Add(new QuadMistResourceManager.QuadMistMapperData("card_arrow_righttop.png"));
		list9.Add(new QuadMistResourceManager.QuadMistMapperData("card_arrow_right.png"));
		list9.Add(new QuadMistResourceManager.QuadMistMapperData("card_arrow_rightbuttom.png"));
		list9.Add(new QuadMistResourceManager.QuadMistMapperData("card_arrow_buttom.png"));
		list9.Add(new QuadMistResourceManager.QuadMistMapperData("card_arrow_leftbuttom.png"));
		list9.Add(new QuadMistResourceManager.QuadMistMapperData("card_arrow_left.png"));
		list9.Add(new QuadMistResourceManager.QuadMistMapperData("card_arrow_lefttop.png"));
		mapperData.Add("CardArrow", list9);
		if (mapperData.ContainsKey("CardBackground"))
		{
			mapperData.Remove("CardBackground");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list10 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list10.Add(new QuadMistResourceManager.QuadMistMapperData("card_player_bg.png"));
		list10.Add(new QuadMistResourceManager.QuadMistMapperData("card_opponent_bg.png"));
		list10.Add(new QuadMistResourceManager.QuadMistMapperData("block_a.png"));
		list10.Add(new QuadMistResourceManager.QuadMistMapperData("block_b.png"));
		list10.Add(new QuadMistResourceManager.QuadMistMapperData("card_back.png"));
		list10.Add(new QuadMistResourceManager.QuadMistMapperData("card_player_frame.png"));
		list10.Add(new QuadMistResourceManager.QuadMistMapperData("card_opponent_frame.png"));
		list10.Add(new QuadMistResourceManager.QuadMistMapperData("goldenbluecardframe.png"));
		list10.Add(new QuadMistResourceManager.QuadMistMapperData("goldenredcardframe.png"));
		mapperData.Add("CardBackground", list10);
		if (mapperData.ContainsKey("CardBlock"))
		{
			mapperData.Remove("CardBlock");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list11 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list11.Add(new QuadMistResourceManager.QuadMistMapperData("block_a.png"));
		list11.Add(new QuadMistResourceManager.QuadMistMapperData("block_b.png"));
		mapperData.Add("CardBlock", list11);
		if (mapperData.ContainsKey("CardIcon"))
		{
			mapperData.Remove("CardIcon");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list12 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list12.Add(new QuadMistResourceManager.QuadMistMapperData("card_slot.png"));
		for (Int32 num7 = 0; num7 <= 6; num7++)
		{
			list12.Add(new QuadMistResourceManager.QuadMistMapperData("card_type" + num7 + "_normal.png"));
		}
		for (Int32 num8 = 0; num8 <= 6; num8++)
		{
			list12.Add(new QuadMistResourceManager.QuadMistMapperData("card_type" + num8 + "_select.png"));
		}
		mapperData.Add("CardIcon", list12);
		if (mapperData.ContainsKey("CardSelect"))
		{
			mapperData.Remove("CardSelect");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list13 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list13.Add(new QuadMistResourceManager.QuadMistMapperData("text_select.png"));
		mapperData.Add("CardSelect", list13);
		if (mapperData.ContainsKey("Coin"))
		{
			mapperData.Remove("Coin");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list14 = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 num9 = 8; num9 >= 1; num9--)
		{
			list14.Add(new QuadMistResourceManager.QuadMistMapperData("coin_0" + num9 + ".png"));
		}
		mapperData.Add("Coin", list14);
		if (mapperData.ContainsKey("Cursor"))
		{
			mapperData.Remove("Cursor");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list15 = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 num10 = 0; num10 <= 6; num10++)
		{
			list15.Add(new QuadMistResourceManager.QuadMistMapperData("card_cursor_" + num10 + ".png"));
		}
		mapperData.Add("Cursor", list15);
		if (mapperData.ContainsKey("CursorPreBoard"))
		{
			mapperData.Remove("CursorPreBoard");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list16 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list16.Add(new QuadMistResourceManager.QuadMistMapperData("cursor_hand_choice.png"));
		mapperData.Add("CursorPreBoard", list16);
		if (mapperData.ContainsKey("Explosion"))
		{
			mapperData.Remove("Explosion");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list17 = new List<QuadMistResourceManager.QuadMistMapperData>();
		for (Int32 num11 = 0; num11 <= 8; num11++)
		{
			list17.Add(new QuadMistResourceManager.QuadMistMapperData("bomb_0" + (num11 + 1) + ".png"));
		}
		for (Int32 num12 = 9; num12 <= 13; num12++)
		{
			list17.Add(new QuadMistResourceManager.QuadMistMapperData("bomb_" + (num12 + 1) + ".png"));
		}
		mapperData.Add("Explosion", list17);
		if (mapperData.ContainsKey("GetCardMessage"))
		{
			mapperData.Remove("GetCardMessage");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list18 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list18.Add(new QuadMistResourceManager.QuadMistMapperData("card_notice_new.png"));
		list18.Add(new QuadMistResourceManager.QuadMistMapperData("card_notice_last.png"));
		mapperData.Add("GetCardMessage", list18);
		if (mapperData.ContainsKey("LRButton"))
		{
			mapperData.Remove("LRButton");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list19 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list19.Add(new QuadMistResourceManager.QuadMistMapperData("button_previous.png"));
		list19.Add(new QuadMistResourceManager.QuadMistMapperData("button_next.png"));
		mapperData.Add("LRButton", list19);
		if (mapperData.ContainsKey("PreBoardTitle"))
		{
			mapperData.Remove("PreBoardTitle");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list20 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list20.Add(new QuadMistResourceManager.QuadMistMapperData("text_card.png"));
		list20.Add(new QuadMistResourceManager.QuadMistMapperData("text_selection.png"));
		mapperData.Add("PreBoardTitle", list20);
		if (mapperData.ContainsKey("Result"))
		{
			mapperData.Remove("Result");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list21 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list21.Add(new QuadMistResourceManager.QuadMistMapperData("card_win.png"));
		list21.Add(new QuadMistResourceManager.QuadMistMapperData("card_lose.png"));
		list21.Add(new QuadMistResourceManager.QuadMistMapperData("card_draw.png"));
		list21.Add(new QuadMistResourceManager.QuadMistMapperData("card_perfect.png"));
		mapperData.Add("Result", list21);
		if (mapperData.ContainsKey("ResultShadow"))
		{
			mapperData.Remove("ResultShadow");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list22 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list22.Add(new QuadMistResourceManager.QuadMistMapperData("card_win_shadow.png"));
		list22.Add(new QuadMistResourceManager.QuadMistMapperData("card_lose_shadow.png"));
		list22.Add(new QuadMistResourceManager.QuadMistMapperData("card_draw_shadow.png"));
		list22.Add(new QuadMistResourceManager.QuadMistMapperData("card_perfect_shadow.png"));
		mapperData.Add("ResultShadow", list22);
		if (mapperData.ContainsKey("ScoreDivider"))
		{
			mapperData.Remove("ScoreDivider");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list23 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list23.Add(new QuadMistResourceManager.QuadMistMapperData("arrow.png"));
		mapperData.Add("ScoreDivider", list23);
		if (mapperData.ContainsKey("CardNameToggle"))
		{
			mapperData.Remove("CardNameToggle");
		}
		List<QuadMistResourceManager.QuadMistMapperData> list24 = new List<QuadMistResourceManager.QuadMistMapperData>();
		list24.Add(new QuadMistResourceManager.QuadMistMapperData("text_name.png"));
		list24.Add(new QuadMistResourceManager.QuadMistMapperData("text_name_hilight.png"));
		mapperData.Add("CardNameToggle", list24);
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
				String spriteName = quadMistMapperData.SpriteName;
				QuadMistAssetData quadMistAssetData = (QuadMistAssetData)null;
				for (Int32 j = 0; j < 3; j++)
				{
					if (!String.IsNullOrEmpty(quadMistMapperData.SpriteName) && spriteData.ContainsKey(atlasPathList[j]) && spriteData[atlasPathList[j]].ContainsKey(quadMistMapperData.SpriteName))
					{
						Sprite sprite = spriteData[atlasPathList[j]][quadMistMapperData.SpriteName];
						quadMistAssetData = new QuadMistAssetData(keyValuePair.Key, i, sprite);
					}
				}
				if (quadMistAssetData == null)
				{
					quadMistAssetData = new QuadMistAssetData(String.Empty, i, (Sprite)null);
				}
				list.Add(quadMistAssetData);
			}
			assetData.Add(keyValuePair.Key, list);
		}
	}

	private void CreateFontMap()
	{
		fontMap = new Dictionary<String, Dictionary<Char, Int32>>();
		Int32 num = 48;
		Dictionary<Char, Int32> dictionary = new Dictionary<Char, Int32>();
		for (Int32 i = 0; i <= 9; i++)
		{
			dictionary.Add((Char)(i + num), i);
		}
		fontMap.Add("BattleNum", dictionary);
		Dictionary<Char, Int32> dictionary2 = new Dictionary<Char, Int32>();
		for (Int32 j = 0; j <= 9; j++)
		{
			dictionary2.Add((Char)(j + num), j);
		}
		fontMap.Add("CardIconCounter", dictionary2);
		Dictionary<Char, Int32> dictionary3 = new Dictionary<Char, Int32>();
		for (Int32 k = 0; k <= 9; k++)
		{
			dictionary3.Add((Char)(k + num), k);
		}
		dictionary3.Add('a', 10);
		dictionary3.Add('b', 11);
		dictionary3.Add('c', 12);
		dictionary3.Add('d', 13);
		dictionary3.Add('e', 14);
		dictionary3.Add('f', 15);
		dictionary3.Add('m', 22);
		dictionary3.Add('p', 25);
		dictionary3.Add('x', 33);
		fontMap.Add("CardStat", dictionary3);
		Dictionary<Char, Int32> dictionary4 = new Dictionary<Char, Int32>();
		for (Int32 l = 0; l <= 9; l++)
		{
			dictionary4.Add((Char)(l + num), l);
		}
		fontMap.Add("Combo", dictionary4);
		Dictionary<Char, Int32> dictionary5 = new Dictionary<Char, Int32>();
		for (Int32 m = 0; m <= 9; m++)
		{
			dictionary5.Add((Char)(m + num), m);
		}
		dictionary5.Add('a', 10);
		fontMap.Add("EnemyScore", dictionary5);
		Dictionary<Char, Int32> dictionary6 = new Dictionary<Char, Int32>();
		for (Int32 n = 0; n <= 9; n++)
		{
			dictionary6.Add((Char)(n + num), n);
		}
		dictionary6.Add('a', 10);
		fontMap.Add("PlayerScore", dictionary6);
	}

	public QuadMistAssetData GetFont(String sheetAssetName, Char character)
	{
		Dictionary<Char, Int32> dictionary = fontMap[sheetAssetName];
		return GetResource(sheetAssetName, fontMap[sheetAssetName][character]);
	}

	public QuadMistAssetData GetResource(String sheetAssetName, Int32 spriteIndex)
	{
		Int32 count = assetData[sheetAssetName].Count;
		return assetData[sheetAssetName][spriteIndex];
	}

	public Sprite GetSprite(String spriteName)
	{
		Sprite result = (Sprite)null;
		foreach (KeyValuePair<String, Dictionary<String, Sprite>> keyValuePair in spriteData)
		{
			if (keyValuePair.Value.ContainsKey(spriteName))
			{
				result = keyValuePair.Value[spriteName];
				break;
			}
		}
		return result;
	}

	private Dictionary<String, Dictionary<String, Sprite>> spriteData;

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
