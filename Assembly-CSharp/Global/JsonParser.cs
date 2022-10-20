using System;
using System.Collections.Generic;
using System.Globalization;
using Memoria.Data;
using SimpleJSON;
using UnityEngine;

public class JsonParser : ISharedDataParser
{
	public override void ParseFromFF9StateSystem()
	{
		JSONClass rootNode = new JSONClass();
		JSONClass schemaNode = new JSONClass();
		this.ParseFromFF9StateSystem(rootNode, schemaNode);
		this.StoreData(rootNode);
	}

	private void AddReservedBuffer(JSONClass rootNode, JSONClass schemaNode, String moduleName, Int32 size)
	{
		Int32 count = size / 4;
		JSONArray moduleArray = new JSONArray();
		for (Int32 i = 0; i < count; i++)
			moduleArray.Add("0");
		rootNode.Add(moduleName, moduleArray);
		JSONArray schemaArray = new JSONArray();
		for (Int32 i = 0; i < count; i++)
			schemaArray.Add(typeof(Int32).ToString());
		schemaNode.Add(moduleName, schemaArray);
	}

	public void ParseFromFF9StateSystem(JSONClass rootNode, JSONClass schemaNode)
	{
		JSONClass rootMainClass = new JSONClass();
		rootNode.Add("Data", rootMainClass);
		JSONClass schemaMainClass = new JSONClass();
		schemaNode.Add("Data", schemaMainClass);
		this.ParseStateDataToJson(rootMainClass, schemaMainClass);
		this.ParseEventDataToJson(FF9StateSystem.EventState, rootMainClass, schemaMainClass);
		this.ParseQuadMistDataToJson(FF9StateSystem.MiniGame.SavedData, rootMainClass, schemaMainClass);
		this.ParseCommonDataToJson(FF9StateSystem.Common.FF9, rootMainClass, schemaMainClass);
		this.ParseSettingDataToJson(FF9StateSystem.Settings, rootMainClass, schemaMainClass);
		this.ParseSoundDataToJson(FF9StateSystem.Sound, rootMainClass, schemaMainClass);
		this.ParseWorldDataToJson(FF9StateSystem.World, rootMainClass, schemaMainClass);
		this.ParseAchievementDataToJson(FF9StateSystem.Achievement, rootMainClass, schemaMainClass);
		this.AddReservedBuffer(rootMainClass, schemaMainClass, "91000_State", 512);
		this.AddReservedBuffer(rootMainClass, schemaMainClass, "92000_Event", 512);
		this.AddReservedBuffer(rootMainClass, schemaMainClass, "93000_MiniGame", 512);
		this.AddReservedBuffer(rootMainClass, schemaMainClass, "94000_Common", 512);
		JSONClass rootCommonClass = new JSONClass();
		JSONArray rootCommonArray = new JSONArray();
		PLAYER[] playerArray = new PLAYER[9];
		foreach (PLAYER player in FF9StateSystem.Common.FF9.PlayerList)
		{
			Int32 arrIndex = SelectOldSaveSlot(player);
			if (arrIndex >= 0)
				playerArray[arrIndex] = player;
		}
		for (Int32 i = 0; i < 9; i++)
			rootCommonArray.Add(playerArray[i].bonus.cap.ToString());
		rootCommonClass.Add("00001_player_bonus", rootCommonArray);
		rootMainClass.Add("94000_Common", rootCommonClass);
		JSONClass schemaCommonClass = new JSONClass();
		JSONArray schemaCommonArray = new JSONArray();
		for (Int32 j = 0; j < 9; j++)
			schemaCommonArray.Add(typeof(UInt32).ToString());
		schemaCommonClass.Add("00001_player_bonus", schemaCommonArray);
		schemaMainClass.Add("94000_Common", schemaCommonClass);
		this.AddReservedBuffer(rootMainClass["94000_Common"].AsObject, schemaMainClass["94000_Common"].AsObject, "99999_ReservedBuffer", 476);
		this.AddReservedBuffer(rootMainClass, schemaMainClass, "95000_Setting", 512);
		rootMainClass.Add("95000_Setting", new JSONClass
		{
			{
				"00001_time",
				FF9StateSystem.Settings.time.ToString()
			}
		});
		schemaMainClass.Add("95000_Setting", new JSONClass
		{
			{
				"00001_time",
				FF9StateSystem.Settings.time.GetType().ToString()
			}
		});
		this.AddReservedBuffer(rootMainClass["95000_Setting"].AsObject, schemaMainClass["95000_Setting"].AsObject, "99999_ReservedBuffer", 504);
		this.AddReservedBuffer(rootMainClass, schemaMainClass, "96000_Sound", 512);
		this.AddReservedBuffer(rootMainClass, schemaMainClass, "97000_World", 512);
		this.AddReservedBuffer(rootMainClass, schemaMainClass, "98000_Achievement", 512);
		rootMainClass.Add("98000_Achievement", new JSONClass
		{
			{
				"00001_abnormal_status",
				((UInt32)FF9StateSystem.Achievement.abnormal_status).ToString()
			},
			{
				"00002_summon_shiva",
				FF9StateSystem.Achievement.summon_shiva.ToString()
			},
			{
				"00003_summon_ifrit",
				FF9StateSystem.Achievement.summon_ifrit.ToString()
			},
			{
				"00004_summon_ramuh",
				FF9StateSystem.Achievement.summon_ramuh.ToString()
			},
			{
				"00005_summon_carbuncle_reflector",
				FF9StateSystem.Achievement.summon_carbuncle_reflector.ToString()
			},
			{
				"00006_summon_carbuncle_haste",
				FF9StateSystem.Achievement.summon_carbuncle_haste.ToString()
			},
			{
				"00007_summon_carbuncle_protect",
				FF9StateSystem.Achievement.summon_carbuncle_protect.ToString()
			},
			{
				"00008_summon_carbuncle_shell",
				FF9StateSystem.Achievement.summon_carbuncle_shell.ToString()
			},
			{
				"00009_summon_fenrir_earth",
				FF9StateSystem.Achievement.summon_fenrir_earth.ToString()
			},
			{
				"000010_summon_fenrir_wind",
				FF9StateSystem.Achievement.summon_fenrir_wind.ToString()
			},
			{
				"000011_summon_atomos",
				FF9StateSystem.Achievement.summon_atomos.ToString()
			},
			{
				"000012_summon_phoenix",
				FF9StateSystem.Achievement.summon_phoenix.ToString()
			},
			{
				"000013_summon_leviathan",
				FF9StateSystem.Achievement.summon_leviathan.ToString()
			},
			{
				"000014_summon_odin",
				FF9StateSystem.Achievement.summon_odin.ToString()
			},
			{
				"000015_summon_madeen",
				FF9StateSystem.Achievement.summon_madeen.ToString()
			},
			{
				"000016_summon_bahamut",
				FF9StateSystem.Achievement.summon_bahamut.ToString()
			},
			{
				"000017_summon_arc",
				FF9StateSystem.Achievement.summon_arc.ToString()
			}
		});
		schemaMainClass.Add("98000_Achievement", new JSONClass
		{
			{
				"00001_abnormal_status",
				((UInt32)FF9StateSystem.Achievement.abnormal_status).GetType().ToString()
			},
			{
				"00002_summon_shiva",
				FF9StateSystem.Achievement.summon_shiva.GetType().ToString()
			},
			{
				"00003_summon_ifrit",
				FF9StateSystem.Achievement.summon_ifrit.GetType().ToString()
			},
			{
				"00004_summon_ramuh",
				FF9StateSystem.Achievement.summon_ramuh.GetType().ToString()
			},
			{
				"00005_summon_carbuncle_reflector",
				FF9StateSystem.Achievement.summon_carbuncle_reflector.GetType().ToString()
			},
			{
				"00006_summon_carbuncle_haste",
				FF9StateSystem.Achievement.summon_carbuncle_haste.GetType().ToString()
			},
			{
				"00007_summon_carbuncle_protect",
				FF9StateSystem.Achievement.summon_carbuncle_protect.GetType().ToString()
			},
			{
				"00008_summon_carbuncle_shell",
				FF9StateSystem.Achievement.summon_carbuncle_shell.GetType().ToString()
			},
			{
				"00009_summon_fenrir_earth",
				FF9StateSystem.Achievement.summon_fenrir_earth.GetType().ToString()
			},
			{
				"000010_summon_fenrir_wind",
				FF9StateSystem.Achievement.summon_fenrir_wind.GetType().ToString()
			},
			{
				"000011_summon_atomos",
				FF9StateSystem.Achievement.summon_atomos.GetType().ToString()
			},
			{
				"000012_summon_phoenix",
				FF9StateSystem.Achievement.summon_phoenix.GetType().ToString()
			},
			{
				"000013_summon_leviathan",
				FF9StateSystem.Achievement.summon_leviathan.GetType().ToString()
			},
			{
				"000014_summon_odin",
				FF9StateSystem.Achievement.summon_odin.GetType().ToString()
			},
			{
				"000015_summon_madeen",
				FF9StateSystem.Achievement.summon_madeen.GetType().ToString()
			},
			{
				"000016_summon_bahamut",
				FF9StateSystem.Achievement.summon_bahamut.GetType().ToString()
			},
			{
				"000017_summon_arc",
				FF9StateSystem.Achievement.summon_arc.GetType().ToString()
			}
		});
		this.AddReservedBuffer(rootMainClass["98000_Achievement"].AsObject, schemaMainClass["98000_Achievement"].AsObject, "99999_ReservedBuffer", 492);
		this.AddReservedBuffer(rootMainClass, schemaMainClass, "99000_Other", 1536);

		JSONClass rootMemoriaClass = new JSONClass();
		JSONClass schemaMemoriaClass = new JSONClass();
		rootNode.Add("MemoriaExtraData", rootMemoriaClass);
		rootMemoriaClass.Add("95000_Setting", new JSONClass
		{
			{ "00001_time", FF9StateSystem.Settings.time.ToString() }
		});
		this.ParseCommonDataToJson(FF9StateSystem.Common.FF9, rootMemoriaClass, schemaMemoriaClass, false);
	}

	public void StoreData(JSONClass rootNode)
	{
		this.RootNodeInParser = rootNode;
	}

	public override void ParseToFF9StateSystem(JSONClass rootNode)
	{
		this.ParseToFF9StateSystem(rootNode);
	}

	public void ParseToFF9StateSystem(JSONNode rootNode)
	{
		JSONNode rootMainClass = rootNode["Data"];
		if (rootMainClass == null)
			return;
		if (rootMainClass["10000_State"] != null)
			this.ParseStateJsonToData(rootMainClass["10000_State"]);
		if (rootMainClass["20000_Event"] != null)
			this.ParseEventJsonToData(rootMainClass["20000_Event"]);
		if (rootMainClass["30000_MiniGame"] != null)
			FF9StateSystem.MiniGame.SavedData = this.ParseQuadMistJsonToData(rootMainClass["30000_MiniGame"]);
		if (rootMainClass["40000_Common"] != null)
			this.ParseCommonJsonToData(rootMainClass["40000_Common"]);
		if (rootMainClass["50000_Setting"] != null)
			this.ParseSettingJsonToData(rootMainClass["50000_Setting"]);
		if (rootMainClass["60000_Sound"] != null)
			this.ParseSoundJsonToData(rootMainClass["60000_Sound"]);
		if (rootMainClass["70000_World"] != null)
			this.ParseWorldJsonToData(rootMainClass["70000_World"]);
		if (rootMainClass["80000_Achievement"] != null)
			this.ParseAchievementJsonToData(rootMainClass["80000_Achievement"]);
		if (rootMainClass["94000_Common"]["00001_player_bonus"] != null)
		{
			for (Int32 i = 0; i < 9; i++)
			{
				PLAYER player = GetPlayerFromOldSave(i, false);
				player.bonus.cap = (UInt16)rootMainClass["94000_Common"]["00001_player_bonus"][i].AsUInt;
			}
		}
		if (rootMainClass["50000_Setting"] != null && rootMainClass["95000_Setting"] != null && rootMainClass["50000_Setting"]["time"] != null && rootMainClass["95000_Setting"]["00001_time"] != null)
		{
			if (rootMainClass["50000_Setting"]["time"].AsFloat >= 0.0)
				FF9StateSystem.Settings.time = rootMainClass["50000_Setting"]["time"].AsFloat;
			else
				FF9StateSystem.Settings.time = rootMainClass["95000_Setting"]["00001_time"].AsDouble;
		}
		if (rootMainClass["98000_Achievement"]["00001_abnormal_status"] != null)
			FF9StateSystem.Achievement.abnormal_status = rootMainClass["98000_Achievement"]["00001_abnormal_status"].AsUInt;
		if (rootMainClass["98000_Achievement"]["00002_summon_shiva"] != null)
			FF9StateSystem.Achievement.summon_shiva = rootMainClass["98000_Achievement"]["00002_summon_shiva"].AsBool;
		if (rootMainClass["98000_Achievement"]["00003_summon_ifrit"] != null)
			FF9StateSystem.Achievement.summon_ifrit = rootMainClass["98000_Achievement"]["00003_summon_ifrit"].AsBool;
		if (rootMainClass["98000_Achievement"]["00004_summon_ramuh"] != null)
			FF9StateSystem.Achievement.summon_ramuh = rootMainClass["98000_Achievement"]["00004_summon_ramuh"].AsBool;
		if (rootMainClass["98000_Achievement"]["00005_summon_carbuncle_reflector"] != null)
			FF9StateSystem.Achievement.summon_carbuncle_reflector = rootMainClass["98000_Achievement"]["00005_summon_carbuncle_reflector"].AsBool;
		if (rootMainClass["98000_Achievement"]["00006_summon_carbuncle_haste"] != null)
			FF9StateSystem.Achievement.summon_carbuncle_haste = rootMainClass["98000_Achievement"]["00006_summon_carbuncle_haste"].AsBool;
		if (rootMainClass["98000_Achievement"]["00007_summon_carbuncle_protect"] != null)
			FF9StateSystem.Achievement.summon_carbuncle_protect = rootMainClass["98000_Achievement"]["00007_summon_carbuncle_protect"].AsBool;
		if (rootMainClass["98000_Achievement"]["00008_summon_carbuncle_shell"] != null)
			FF9StateSystem.Achievement.summon_carbuncle_shell = rootMainClass["98000_Achievement"]["00008_summon_carbuncle_shell"].AsBool;
		if (rootMainClass["98000_Achievement"]["00009_summon_fenrir_earth"] != null)
			FF9StateSystem.Achievement.summon_fenrir_earth = rootMainClass["98000_Achievement"]["00009_summon_fenrir_earth"].AsBool;
		if (rootMainClass["98000_Achievement"]["000010_summon_fenrir_wind"] != null)
			FF9StateSystem.Achievement.summon_fenrir_wind = rootMainClass["98000_Achievement"]["000010_summon_fenrir_wind"].AsBool;
		if (rootMainClass["98000_Achievement"]["000011_summon_atomos"] != null)
			FF9StateSystem.Achievement.summon_atomos = rootMainClass["98000_Achievement"]["000011_summon_atomos"].AsBool;
		if (rootMainClass["98000_Achievement"]["000012_summon_phoenix"] != null)
			FF9StateSystem.Achievement.summon_phoenix = rootMainClass["98000_Achievement"]["000012_summon_phoenix"].AsBool;
		if (rootMainClass["98000_Achievement"]["000013_summon_leviathan"] != null)
			FF9StateSystem.Achievement.summon_leviathan = rootMainClass["98000_Achievement"]["000013_summon_leviathan"].AsBool;
		if (rootMainClass["98000_Achievement"]["000014_summon_odin"] != null)
			FF9StateSystem.Achievement.summon_odin = rootMainClass["98000_Achievement"]["000014_summon_odin"].AsBool;
		if (rootMainClass["98000_Achievement"]["000015_summon_madeen"] != null)
			FF9StateSystem.Achievement.summon_madeen = rootMainClass["98000_Achievement"]["000015_summon_madeen"].AsBool;
		if (rootMainClass["98000_Achievement"]["000016_summon_bahamut"] != null)
			FF9StateSystem.Achievement.summon_bahamut = rootMainClass["98000_Achievement"]["000016_summon_bahamut"].AsBool;
		if (rootMainClass["98000_Achievement"]["000017_summon_arc"] != null)
			FF9StateSystem.Achievement.summon_arc = rootMainClass["98000_Achievement"]["000017_summon_arc"].AsBool;

		JSONNode memoriaClass = rootNode["MemoriaExtraData"];
		Boolean isMemoriaValid = true;
		if (memoriaClass["95000_Setting"] != null)
		{
			// Maybe check if memoriaClass["95000_Setting"]["00001_time"].AsDOuble == FF9StateSystem.Settings.time
		}
		if (isMemoriaValid)
		{
			if (memoriaClass["40000_Common"] != null)
				this.ParseCommonJsonToData(memoriaClass["40000_Common"], false);
		}
		foreach (PLAYER player in FF9StateSystem.Common.FF9.PlayerList)
		{
			if (player.bonus.cap == 0)
			{
				player.bonus.cap = (UInt16)((player.level - 1) * 5);
				if (player.info.slot_no == CharacterId.Eiko)
					player.bonus.cap += this.getExtraCap(player);
			}
			// Run "FF9Play_Update" anyway when loading data, mostly for "SupportingAbilityFeature.TriggerOnEnable"
			player.ValidateMaxStone();
			ff9play.FF9Play_Update(player);
		}
	}

	private UInt16 getExtraCap(PLAYER player)
	{
		Byte level = player.level;
		UInt16 result;
		if (level <= 10)
			result = 10;
		else if (level <= 20)
			result = 20;
		else if (level <= 30)
			result = 30;
		else if (level <= 40)
			result = 40;
		else if (level <= 50)
			result = 50;
		else if (level <= 60)
			result = 60;
		else if (level <= 70)
			result = 70;
		else if (level <= 80)
			result = 80;
		else if (level <= 90)
			result = 90;
		else
			result = 99;
		return result;
	}

	private void ParseStateJsonToData(JSONNode jsonData)
	{
		FF9StateGlobal ffglobal = FF9StateSystem.Common.FF9;
		FF9StateSystem ffsystem = PersistenSingleton<FF9StateSystem>.Instance;
		if (jsonData["mode"] != null)
			ffsystem.mode = (Byte)jsonData["mode"].AsInt;
		if (jsonData["prevMode"] != null)
			ffsystem.prevMode = (Byte)jsonData["prevMode"].AsInt;
		if (jsonData["fldMapNo"] != null)
			ffglobal.fldMapNo = (Int16)jsonData["fldMapNo"].AsInt;
		if (jsonData["fldLocNo"] != null)
			ffglobal.fldLocNo = (Int16)jsonData["fldLocNo"].AsInt;
		if (jsonData["btlMapNo"] != null)
			ffglobal.btlMapNo = (Int16)jsonData["btlMapNo"].AsInt;
		if (jsonData["btlSubMapNo"] != null)
			ffglobal.btlSubMapNo = (SByte)jsonData["btlSubMapNo"].AsInt;
		if (jsonData["wldMapNo"] != null)
			ffglobal.wldMapNo = (Int16)jsonData["wldMapNo"].AsInt;
		if (jsonData["wldLocNo"] != null)
			ffglobal.wldLocNo = (Int16)jsonData["wldLocNo"].AsInt;
		if (jsonData["timeCounter"] != null)
			TimerUI.SetTimeFromAutoSave((Int32)jsonData["timeCounter"].AsFloat);
		if (jsonData["timerControl"] != null)
			ffglobal.timerControl = jsonData["timerControl"].AsBool;
		if (jsonData["timerDisplay"] != null)
			ffglobal.timerDisplay = jsonData["timerDisplay"].AsBool;
	}

	private void ParseStateDataToJson(JSONClass dataNode, JSONClass dataSchemaNode)
	{
		FF9StateGlobal ffglobal = FF9StateSystem.Common.FF9;
		FF9StateSystem ffsystem = PersistenSingleton<FF9StateSystem>.Instance;
		Single time = TimerUI.Time;
		dataNode.Add("10000_State", new JSONClass
		{
			{
				"mode",
				ffsystem.mode.ToString()
			},
			{
				"prevMode",
				ffsystem.prevMode.ToString()
			},
			{
				"fldMapNo",
				ffglobal.fldMapNo.ToString()
			},
			{
				"fldLocNo",
				ffglobal.fldLocNo.ToString()
			},
			{
				"btlMapNo",
				ffglobal.btlMapNo.ToString()
			},
			{
				"btlSubMapNo",
				ffglobal.btlSubMapNo.ToString()
			},
			{
				"wldMapNo",
				ffglobal.wldMapNo.ToString()
			},
			{
				"wldLocNo",
				ffglobal.wldLocNo.ToString()
			},
			{
				"timeCounter",
				time.ToString()
			},
			{
				"timerControl",
				ffglobal.timerControl.ToString()
			},
			{
				"timerDisplay",
				ffglobal.timerDisplay.ToString()
			}
		});
		dataSchemaNode.Add("10000_State", new JSONClass
		{
			{
				"mode",
				ffsystem.mode.GetType().ToString()
			},
			{
				"prevMode",
				ffsystem.prevMode.GetType().ToString()
			},
			{
				"fldMapNo",
				ffglobal.fldMapNo.GetType().ToString()
			},
			{
				"fldLocNo",
				ffglobal.fldLocNo.GetType().ToString()
			},
			{
				"btlMapNo",
				ffglobal.btlMapNo.GetType().ToString()
			},
			{
				"btlSubMapNo",
				ffglobal.btlSubMapNo.GetType().ToString()
			},
			{
				"wldMapNo",
				ffglobal.wldMapNo.GetType().ToString()
			},
			{
				"wldLocNo",
				ffglobal.wldLocNo.GetType().ToString()
			},
			{
				"timeCounter",
				time.GetType().ToString()
			},
			{
				"timerControl",
				ffglobal.timerControl.GetType().ToString()
			},
			{
				"timerDisplay",
				ffglobal.timerDisplay.GetType().ToString()
			}
		});
	}

	private void ParseEventJsonToData(JSONNode jsonData)
	{
		EventState eventState = FF9StateSystem.EventState;
		if (jsonData["gStepCount"] != null)
			eventState.gStepCount = jsonData["gStepCount"].AsInt;
		if (jsonData["gEventGlobal"] != null)
			eventState.gEventGlobal = Convert.FromBase64String(jsonData["gEventGlobal"].Value);
	}

	private void ParseEventDataToJson(EventState data, JSONClass dataNode, JSONClass schemaNode)
	{
		dataNode.Add("20000_Event", new JSONClass
		{
			{
				"gStepCount",
				data.gStepCount.ToString()
			},
			{
				"gEventGlobal",
				Convert.ToBase64String(data.gEventGlobal)
			}
		});
		schemaNode.Add("20000_Event", new JSONClass
		{
			{
				"gStepCount",
				data.gStepCount.GetType().ToString()
			},
			{
				"gEventGlobal",
				SharedDataBytesStorage.TypeString4K
			}
		});
	}

	private FF9SAVE_MINIGAME ParseQuadMistJsonToData(JSONNode jsonData)
	{
		FF9SAVE_MINIGAME tetraMasterProfile = new FF9SAVE_MINIGAME();
		if (jsonData["sWin"] != null)
			tetraMasterProfile.sWin = (Int16)jsonData["sWin"].AsInt;
		if (jsonData["sLose"] != null)
			tetraMasterProfile.sLose = (Int16)jsonData["sLose"].AsInt;
		if (jsonData["sDraw"] != null)
			tetraMasterProfile.sDraw = (Int16)jsonData["sDraw"].AsInt;
		if (jsonData["MiniGameCard"] != null)
		{
			Int32 count = jsonData["MiniGameCard"].Count;
			for (Int32 i = 0; i < count; i++)
			{
				JSONClass asObject = jsonData["MiniGameCard"][i].AsObject;
				if ((Byte)asObject["id"].AsInt != 255)
				{
					QuadMistCard card = new QuadMistCard();
					if (asObject["id"] != null)
						card.id = (Byte)asObject["id"].AsInt;
					if (asObject["side"] != null)
						card.side = (Byte)asObject["side"].AsInt;
					if (asObject["atk"] != null)
						card.atk = (Byte)asObject["atk"].AsInt;
					if (asObject["type"] != null)
					{
						String cardTypeName = asObject["type"];
						Int32 cardTypeId = asObject["type"].AsInt;
						if (cardTypeName == "PHYSICAL" || cardTypeId == 0)
							card.type = QuadMistCard.Type.PHYSICAL;
						else if (cardTypeName == "MAGIC" || cardTypeId == 1)
							card.type = QuadMistCard.Type.MAGIC;
						else if (cardTypeName == "FLEXIABLE" || cardTypeId == 2)
							card.type = QuadMistCard.Type.FLEXIABLE;
						else if (cardTypeName == "ASSAULT" || cardTypeId == 3)
							card.type = QuadMistCard.Type.ASSAULT;
					}
					if (asObject["pdef"] != null)
						card.pdef = (Byte)asObject["pdef"].AsInt;
					if (asObject["mdef"] != null)
						card.mdef = (Byte)asObject["mdef"].AsInt;
					if (asObject["cpoint"] != null)
						card.cpoint = (Byte)asObject["cpoint"].AsInt;
					if (asObject["arrow"] != null)
						card.arrow = (Byte)asObject["arrow"].AsInt;
					tetraMasterProfile.MiniGameCard.Add(card);
				}
			}
		}
		return tetraMasterProfile;
	}

	private void ParseQuadMistDataToJson(FF9SAVE_MINIGAME data, JSONClass dataNode, JSONClass schemaNode)
	{
		JSONClass dataProfileClass = new JSONClass();
		dataProfileClass.Add("sWin", data.sWin.ToString());
		dataProfileClass.Add("sLose", data.sLose.ToString());
		dataProfileClass.Add("sDraw", data.sDraw.ToString());
		JSONArray dataProfileArray = new JSONArray();
		for (Int32 i = 0; i < 100; i++)
		{
			if (i < data.MiniGameCard.Count)
			{
				QuadMistCard card = data.MiniGameCard[i];
				JSONClass cardClass = new JSONClass();
				cardClass.Add("id", card.id.ToString());
				cardClass.Add("side", "0");
				cardClass.Add("atk", card.atk.ToString());
				cardClass.Add("type", ((Int32)card.type).ToString());
				cardClass.Add("pdef", card.pdef.ToString());
				cardClass.Add("mdef", card.mdef.ToString());
				cardClass.Add("cpoint", card.cpoint.ToString());
				cardClass.Add("arrow", card.arrow.ToString());
				dataProfileArray.Add(cardClass);
			}
			else
			{
				dataProfileArray.Add(new JSONClass
				{
					{
						"id",
						"255"
					},
					{
						"side",
						"255"
					},
					{
						"atk",
						"255"
					},
					{
						"type",
						"255"
					},
					{
						"pdef",
						"255"
					},
					{
						"mdef",
						"255"
					},
					{
						"cpoint",
						"255"
					},
					{
						"arrow",
						"255"
					}
				});
			}
		}
		dataProfileClass.Add("MiniGameCard", dataProfileArray);
		dataNode.Add("30000_MiniGame", dataProfileClass);
		JSONClass schemaProfileClass = new JSONClass();
		schemaProfileClass.Add("sWin", data.sWin.GetType().ToString());
		schemaProfileClass.Add("sLose", data.sLose.GetType().ToString());
		schemaProfileClass.Add("sDraw", data.sDraw.GetType().ToString());
		JSONArray schemaProfileArray = new JSONArray();
		for (Int32 j = 0; j < 100; j++)
		{
			QuadMistCard card = new QuadMistCard();
			schemaProfileArray.Add(new JSONClass
			{
				{
					"id",
					card.id.GetType().ToString()
				},
				{
					"side",
					card.side.GetType().ToString()
				},
				{
					"atk",
					card.atk.GetType().ToString()
				},
				{
					"type",
					typeof(Int32).ToString()
				},
				{
					"pdef",
					card.pdef.GetType().ToString()
				},
				{
					"mdef",
					card.mdef.GetType().ToString()
				},
				{
					"cpoint",
					card.cpoint.GetType().ToString()
				},
				{
					"arrow",
					card.arrow.GetType().ToString()
				}
			});
		}
		schemaProfileClass.Add("MiniGameCard", schemaProfileArray);
		schemaNode.Add("30000_MiniGame", schemaProfileClass);
	}

	private void ParseCommonDataToJson(FF9StateGlobal data, JSONClass dataNode, JSONClass schemaNode, Boolean oldSaveFormat = true)
	{
		JSONClass dataProfileClass = new JSONClass();
		JSONArray dataPlayerArray = new JSONArray();
		PLAYER[] playerArray;
		if (oldSaveFormat)
		{
			playerArray = new PLAYER[9];
			foreach (PLAYER p in data.PlayerList)
			{
				Int32 arrIndex = SelectOldSaveSlot(p);
				if (arrIndex >= 0)
					playerArray[arrIndex] = p;
			}
		}
		else
		{
			Int32 index = 0;
			playerArray = new PLAYER[data.player.Count];
			foreach (PLAYER p in data.PlayerList)
				playerArray[index++] = p;
		}
		for (Int32 i = 0; i < playerArray.Length; i++)
		{
			PLAYER p = playerArray[i];
			JSONClass playerClass = new JSONClass();
			playerClass.Add("name", p.Name);
			playerClass.Add("category", p.category.ToString());
			playerClass.Add("level", p.level.ToString());
			playerClass.Add("exp", p.exp.ToString());
			playerClass.Add("cur", new JSONClass
			{
				{ "hp", ((UInt16)p.cur.hp).ToString() }, // In order to avoid saves to change format, we keep 16-bit datas instead of 32-bits
				{ "mp", ((Int16)p.cur.mp).ToString() },
				{ "at", p.cur.at.ToString() },
				{ "at_coef", p.cur.at_coef.ToString() },
				{ "capa", p.cur.capa.ToString() }
			});
			playerClass.Add("max", new JSONClass
			{
				{ "hp", ((UInt16)p.max.hp).ToString() },
				{ "mp", ((Int16)p.max.mp).ToString() },
				{ "at", p.max.at.ToString() },
				{ "at_coef", p.max.at_coef.ToString() },
				{ "capa", p.max.capa.ToString() }
			});
			playerClass.Add("trance", p.trance.ToString());
			playerClass.Add("web_bone", p.wep_bone.ToString());
			playerClass.Add("elem", new JSONClass
			{
				{ "dex", p.elem.dex.ToString() },
				{ "str", p.elem.str.ToString() },
				{ "mgc", p.elem.mgc.ToString() },
				{ "wpr", p.elem.wpr.ToString() }
			});
			playerClass.Add("defence", new JSONClass
			{
				{ "p_def", p.defence.PhisicalDefence.ToString() },
				{ "p_ev", p.defence.PhisicalEvade.ToString() },
				{ "m_def", p.defence.MagicalDefence.ToString() },
				{ "m_ev", p.defence.MagicalEvade.ToString() }
			});
			playerClass.Add("basis", new JSONClass
			{
				{ "max_hp", ((Int16)p.basis.max_hp).ToString() },
				{ "max_mp", ((Int16)p.basis.max_mp).ToString() },
				{ "dex", p.basis.dex.ToString() },
				{ "str", p.basis.str.ToString() },
				{ "mgc", p.basis.mgc.ToString() },
				{ "wpr", p.basis.wpr.ToString() }
			});
			playerClass.Add("info", oldSaveFormat ? new JSONClass
			{
				{ "slot_no", ((Byte)SelectOldSaveSlot(p)).ToString() },
				{ "serial_no", ((Byte)p.info.serial_no).ToString() },
				{ "row", p.info.row.ToString() },
				{ "win_pose", p.info.win_pose.ToString() },
				{ "party", p.info.party.ToString() },
				{ "menu_type", ((Byte)p.info.menu_type).ToString() }
			} : new JSONClass
			{
				{ "slot_no", ((Byte)p.info.slot_no).ToString() },
				{ "serial_no", ((Byte)p.info.serial_no).ToString() },
				{ "row", p.info.row.ToString() },
				{ "win_pose", p.info.win_pose.ToString() },
				{ "party", p.info.party.ToString() },
				{ "menu_type", ((Byte)p.info.menu_type).ToString() },
				{ "sub_replaced", p.info.sub_replaced.ToString() }
			});
			playerClass.Add("status", p.status.ToString());
			JSONArray equipClass = new JSONArray();
			for (Int32 j = 0; j < 5; j++)
				equipClass.Add(p.equip[j].ToString());
			playerClass.Add("equip", equipClass);
			playerClass.Add("bonus", oldSaveFormat ? new JSONClass
			{
				{ "dex", p.bonus.dex.ToString() },
				{ "str", p.bonus.str.ToString() },
				{ "mgc", p.bonus.mgc.ToString() },
				{ "wpr", p.bonus.wpr.ToString() }
			} : new JSONClass
			{
				{ "dex", p.bonus.dex.ToString() },
				{ "str", p.bonus.str.ToString() },
				{ "mgc", p.bonus.mgc.ToString() },
				{ "wpr", p.bonus.wpr.ToString() },
				{ "cap", p.bonus.cap.ToString() }
			});
			JSONArray apClass = new JSONArray();
			Int32 paCount = oldSaveFormat ? 48 : p.pa.Length;
			for (Int32 j = 0; j < paCount; j++)
				apClass.Add((j < p.pa.Length ? p.pa[j] : 0).ToString());
			playerClass.Add("pa", apClass);
			JSONArray saClass = new JSONArray();
			for (Int32 j = 0; j < 2; j++)
				saClass.Add(p.sa[j].ToString());
			playerClass.Add("sa", saClass);
			dataPlayerArray.Add(playerClass);
		}
		dataProfileClass.Add("players", dataPlayerArray);
		JSONArray dataMemberArray = new JSONArray();
		PARTY_DATA party = data.party;
		for (Int32 i = 0; i < 4; i++)
		{
			if (party.member[i] != null)
				dataMemberArray.Add((oldSaveFormat ? unchecked((Byte)SelectOldSaveSlot(party.member[i])) : (Byte)party.member[i].info.slot_no).ToString());
			else
				dataMemberArray.Add("255");
		}
		dataProfileClass.Add("slot", dataMemberArray);
		dataProfileClass.Add("escape_no", party.escape_no.ToString());
		dataProfileClass.Add("summon_flag", party.summon_flag.ToString());
		dataProfileClass.Add("gil", party.gil.ToString());
		dataProfileClass.Add("frog_no", data.Frogs.Number.ToString());
		dataProfileClass.Add("steal_no", data.steal_no.ToString());
		dataProfileClass.Add("dragon_no", data.dragon_no.ToString());
		FF9ITEM[] item = data.item;
		JSONArray dataItemClass = new JSONArray();
		for (Int32 i = 0; i < 256; i++)
		{
			if (i < item.Length)
			{
				dataItemClass.Add(new JSONClass
				{
					{ "id", item[i].id.ToString() },
					{ "count",item[i].count.ToString() }
				});
			}
			else
			{
				dataItemClass.Add(new JSONClass
				{
					{ "id", "255" },
					{ "count", "255" }
				});
			}
		}
		dataProfileClass.Add("items", dataItemClass);
		JSONArray dataRareItemClass = new JSONArray();
		for (Int32 i = 0; i < 64; i++)
			dataRareItemClass.Add(data.rare_item[i].ToString());
		dataProfileClass.Add("rareItems", dataRareItemClass);
		dataNode.Add("40000_Common", dataProfileClass);
		if (!oldSaveFormat)
			return;
		JSONClass schemaProfileClass = new JSONClass();
		JSONArray schemaPlayerArray = new JSONArray();
		for (Int32 i = 0; i < 9; i++)
		{
			JSONClass playerClass = new JSONClass();
			PLAYER p = data.player[CharacterId.Zidane];
			playerClass.Add("name", p.Name.GetType().ToString());
			playerClass.Add("category", p.category.GetType().ToString());
			playerClass.Add("level", p.level.GetType().ToString());
			playerClass.Add("exp", p.exp.GetType().ToString());
			playerClass.Add("cur", new JSONClass
			{
				{ "hp", typeof(UInt16).ToString() },
				{ "mp", typeof(Int16).ToString() },
				{ "at", p.cur.at.GetType().ToString() },
				{ "at_coef", p.cur.at_coef.GetType().ToString() },
				{ "capa", p.cur.capa.GetType().ToString() }
			});
			playerClass.Add("max", new JSONClass
			{
				{ "hp", typeof(UInt16).ToString() },
				{ "mp", typeof(Int16).ToString() },
				{ "at", p.max.at.GetType().ToString() },
				{ "at_coef", p.max.at_coef.GetType().ToString() },
				{ "capa", p.max.capa.GetType().ToString() }
			});
			playerClass.Add("trance", p.trance.GetType().ToString());
			playerClass.Add("web_bone", p.wep_bone.GetType().ToString());
			playerClass.Add("elem", new JSONClass
			{
				{ "dex", p.elem.dex.GetType().ToString() },
				{ "str", p.elem.str.GetType().ToString() },
				{ "mgc", p.elem.mgc.GetType().ToString() },
				{ "wpr", p.elem.wpr.GetType().ToString() }
			});
			playerClass.Add("defence", new JSONClass
			{
				{ "p_def", p.defence.PhisicalDefence.GetType().ToString() },
				{ "p_ev", p.defence.PhisicalEvade.GetType().ToString() },
				{ "m_def", p.defence.MagicalDefence.GetType().ToString() },
				{ "m_ev", p.defence.MagicalEvade.GetType().ToString() }
			});
			playerClass.Add("basis", new JSONClass
			{
				{ "max_hp", typeof(Int16).ToString() },
				{ "max_mp", typeof(Int16).ToString() },
				{ "dex", p.basis.dex.GetType().ToString() },
				{ "str", p.basis.str.GetType().ToString() },
				{ "mgc", p.basis.mgc.GetType().ToString() },
				{ "wpr", p.basis.wpr.GetType().ToString() }
			});
			playerClass.Add("info", new JSONClass
			{
				{ "slot_no", typeof(Byte).ToString() },
				{ "serial_no", typeof(Byte).ToString() },
				{ "row", p.info.row.GetType().ToString() },
				{ "win_pose", p.info.win_pose.GetType().ToString() },
				{ "party", p.info.party.GetType().ToString() },
				{ "menu_type", typeof(Byte).ToString() }
			});
			playerClass.Add("status", p.status.GetType().ToString());
			JSONArray equipArray = new JSONArray();
			for (Int32 j = 0; j < 5; j++)
				equipArray.Add(typeof(Byte).ToString());
			playerClass.Add("equip", equipArray);
			playerClass.Add("bonus", new JSONClass
			{
				{ "dex", p.bonus.dex.GetType().ToString() },
				{ "str", p.bonus.str.GetType().ToString() },
				{ "mgc", p.bonus.mgc.GetType().ToString() },
				{ "wpr", p.bonus.wpr.GetType().ToString() }
			});
			JSONArray apArray = new JSONArray();
			Int32 paCount = oldSaveFormat ? 48 : p.pa.Length;
			for (Int32 j = 0; j < paCount; j++)
				apArray.Add(typeof(Byte).ToString());
			playerClass.Add("pa", apArray);
			JSONArray saArray = new JSONArray();
			for (Int32 j = 0; j < 2; j++)
				saArray.Add(p.sa[j].GetType().ToString());
			playerClass.Add("sa", saArray);
			schemaPlayerArray.Add(playerClass);
		}
		schemaProfileClass.Add("players", schemaPlayerArray);
		JSONArray schemaMemberArray = new JSONArray();
		for (Int32 i = 0; i < 4; i++)
			schemaMemberArray.Add(typeof(Byte).ToString());
		schemaProfileClass.Add("slot", schemaMemberArray);
		schemaProfileClass.Add("escape_no", party.escape_no.GetType().ToString());
		schemaProfileClass.Add("summon_flag", party.summon_flag.GetType().ToString());
		schemaProfileClass.Add("gil", party.gil.GetType().ToString());
		schemaProfileClass.Add("frog_no", data.Frogs.Number.GetType().ToString());
		schemaProfileClass.Add("steal_no", data.steal_no.GetType().ToString());
		schemaProfileClass.Add("dragon_no", data.dragon_no.GetType().ToString());
		JSONArray schemaItemArray = new JSONArray();
		for (Int32 i = 0; i < 256; i++)
		{
			schemaItemArray.Add(new JSONClass
			{
				{ "id", typeof(Byte).ToString() },
				{ "count", typeof(Byte).ToString() }
			});
		}
		schemaProfileClass.Add("items", schemaItemArray);
		JSONArray schemaRareItemArray = new JSONArray();
		for (Int32 i = 0; i < 64; i++)
			schemaRareItemArray.Add(typeof(Byte).ToString());
		schemaProfileClass.Add("rareItems", schemaRareItemArray);
		schemaNode.Add("40000_Common", schemaProfileClass);
	}

	private void ParseCommonJsonToData(JSONNode jsonData, Boolean oldSaveFormat = true)
	{
		FF9StateGlobal ffglobal = FF9StateSystem.Common.FF9;
		Boolean[] isTempPlayer = null;
		if (jsonData["players"] != null)
		{
			Int32 count = jsonData["players"].Count;
			isTempPlayer = new Boolean[count];
			for (Int32 i = 0; i < count; i++)
			{
				CharacterId charId = CharacterId.NONE;
				JSONClass playerClass = jsonData["players"][i].AsObject;
				JSONClass playerInfoClass = playerClass["info"].AsObject;
				if (playerInfoClass != null && playerInfoClass["slot_no"] != null)
					charId = (CharacterId)playerInfoClass["slot_no"].AsInt;
				PLAYER player = ffglobal.GetPlayer(charId);
				if (player == null)
					continue;
				if (oldSaveFormat && playerClass["category"] != null)
				{
					isTempPlayer[i] = (playerClass["category"].AsInt & 16) != 0;
					player = GetPlayerFromOldSave(i, isTempPlayer[i]);
				}
				if (playerClass["name"] != null)
					player.Name = playerClass["name"].Value;
				if (playerClass["category"] != null)
					player.category = (Byte)playerClass["category"].AsInt;
				if (playerClass["level"] != null)
					player.level = (Byte)playerClass["level"].AsInt;
				if (playerClass["exp"] != null)
					player.exp = (UInt32)playerClass["exp"].AsInt;
				JSONClass playerCurClass = playerClass["cur"].AsObject;
				if (playerCurClass["hp"] != null)
					player.cur.hp = (UInt32)playerCurClass["hp"].AsInt;
				if (playerCurClass["mp"] != null)
					player.cur.mp = (UInt32)playerCurClass["mp"].AsInt;
				if (playerCurClass["at"] != null)
					player.cur.at = (Int16)playerCurClass["at"].AsInt;
				if (playerCurClass["at_coef"] != null)
					player.cur.at_coef = (SByte)playerCurClass["at_coef"].AsInt;
				if (playerCurClass["capa"] != null)
					player.cur.capa = (Byte)playerCurClass["capa"].AsInt;
				JSONClass playerMaxClass = playerClass["max"].AsObject;
				if (playerMaxClass["hp"] != null)
					player.max.hp = (UInt32)playerMaxClass["hp"].AsInt;
				if (playerMaxClass["mp"] != null)
					player.max.mp = (UInt32)playerMaxClass["mp"].AsInt;
				if (playerMaxClass["at"] != null)
					player.max.at = (Int16)playerMaxClass["at"].AsInt;
				if (playerMaxClass["at_coef"] != null)
					player.max.at_coef = (SByte)playerMaxClass["at_coef"].AsInt;
				if (playerMaxClass["capa"] != null)
					player.max.capa = (Byte)playerMaxClass["capa"].AsInt;
				if (playerClass["trance"] != null)
					player.trance = (Byte)playerClass["trance"].AsInt;
				if (playerClass["wep_bone"] != null)
					player.wep_bone = (Byte)playerClass["wep_bone"].AsInt;
				JSONClass playerElemClass = playerClass["elem"].AsObject;
				if (playerElemClass["dex"] != null)
					player.elem.dex = (Byte)playerElemClass["dex"].AsInt;
				if (playerElemClass["str"] != null)
					player.elem.str = (Byte)playerElemClass["str"].AsInt;
				if (playerElemClass["mgc"] != null)
					player.elem.mgc = (Byte)playerElemClass["mgc"].AsInt;
				if (playerElemClass["wpr"] != null)
					player.elem.wpr = (Byte)playerElemClass["wpr"].AsInt;
				JSONClass playerDefClass = playerClass["defence"].AsObject;
				if (playerDefClass["p_def"] != null)
					player.defence.PhisicalDefence = (Byte)playerDefClass["p_def"].AsInt;
				if (playerDefClass["p_ev"] != null)
					player.defence.PhisicalEvade = (Byte)playerDefClass["p_ev"].AsInt;
				if (playerDefClass["m_def"] != null)
					player.defence.MagicalDefence = (Byte)playerDefClass["m_def"].AsInt;
				if (playerDefClass["m_ev"] != null)
					player.defence.MagicalEvade = (Byte)playerDefClass["m_ev"].AsInt;
				JSONClass playerBasisClass = playerClass["basis"].AsObject;
				if (playerBasisClass["max_hp"] != null)
					player.basis.max_hp = (UInt32)playerBasisClass["max_hp"].AsInt;
				if (playerBasisClass["max_mp"] != null)
					player.basis.max_mp = (UInt32)playerBasisClass["max_mp"].AsInt;
				if (playerBasisClass["dex"] != null)
					player.basis.dex = (Byte)playerBasisClass["dex"].AsInt;
				if (playerBasisClass["str"] != null)
					player.basis.str = (Byte)playerBasisClass["str"].AsInt;
				if (playerBasisClass["mgc"] != null)
					player.basis.mgc = (Byte)playerBasisClass["mgc"].AsInt;
				if (playerBasisClass["wpr"] != null)
					player.basis.wpr = (Byte)playerBasisClass["wpr"].AsInt;
				if (playerInfoClass["serial_no"] != null)
					player.info.serial_no = (CharacterSerialNumber)playerInfoClass["serial_no"].AsInt;
				if (playerInfoClass["row"] != null)
					player.info.row = (Byte)playerInfoClass["row"].AsInt;
				if (playerInfoClass["win_pose"] != null)
					player.info.win_pose = (Byte)playerInfoClass["win_pose"].AsInt;
				if (playerInfoClass["party"] != null)
					player.info.party = (Byte)playerInfoClass["party"].AsInt;
				if (playerInfoClass["menu_type"] != null)
					player.info.menu_type = (CharacterPresetId)playerInfoClass["menu_type"].AsInt;
				if (playerClass["status"] != null)
					player.status = (Byte)playerClass["status"].AsInt;
				if (playerClass["equip"] != null)
					for (Int32 j = 0; j < playerClass["equip"].Count && j < CharacterEquipment.Length; j++)
						player.equip[j] = (Byte)playerClass["equip"][j].AsInt;
				JSONClass playerBonusClass = playerClass["bonus"].AsObject;
				if (playerBonusClass["dex"] != null)
					player.bonus.dex = (UInt16)playerBonusClass["dex"].AsInt;
				if (playerBonusClass["str"] != null)
					player.bonus.str = (UInt16)playerBonusClass["str"].AsInt;
				if (playerBonusClass["mgc"] != null)
					player.bonus.mgc = (UInt16)playerBonusClass["mgc"].AsInt;
				if (playerBonusClass["wpr"] != null)
					player.bonus.wpr = (UInt16)playerBonusClass["wpr"].AsInt;
				if (playerBonusClass["cap"] != null)
					player.bonus.cap = (UInt16)playerBonusClass["cap"].AsInt;
				if (playerClass["pa"] != null)
					for (Int32 j = 0; j < player.pa.Length; j++)
						player.pa[j] = (Byte)(j < playerClass["pa"].Count ? playerClass["pa"][j].AsInt : 0);
				if (playerClass["sa"] != null)
					for (Int32 j = 0; j < playerClass["sa"].Count && j < player.sa.Length; j++)
						player.sa[j] = playerClass["sa"][j].AsUInt;
				player.ValidateSupportAbility();
				player.ValidateBasisStatus();
			}
		}
		if (jsonData["slot"] != null)
		{
			for (Int32 i = 0; i < jsonData["slot"].Count && i < 4; i++)
			{
				Int32 slotNo = jsonData["slot"][i].AsInt;
				if (slotNo == 255 || slotNo < 0)
					ffglobal.party.member[i] = null;
				else if (oldSaveFormat && isTempPlayer != null && slotNo < isTempPlayer.Length)
					ffglobal.party.member[i] = GetPlayerFromOldSave(slotNo, isTempPlayer[slotNo]);
				else
					ffglobal.party.member[i] = FF9StateSystem.Common.FF9.GetPlayer((CharacterId)slotNo);
			}
		}
		if (jsonData["escape_no"] != null)
			ffglobal.party.escape_no = (UInt16)jsonData["escape_no"].AsInt;
		if (jsonData["summon_flag"] != null)
			ffglobal.party.summon_flag = (UInt16)jsonData["summon_flag"].AsInt;
		if (jsonData["gil"] != null)
			ffglobal.party.gil = (UInt32)jsonData["gil"].AsInt;
		if (jsonData["frog_no"] != null)
		    ffglobal.Frogs.Initialize((Int16)jsonData["frog_no"].AsInt);
		if (jsonData["steal_no"] != null)
			ffglobal.steal_no = (Int16)jsonData["steal_no"].AsInt;
		if (jsonData["dragon_no"] != null)
			ffglobal.dragon_no = (Int16)jsonData["dragon_no"].AsInt;
		if (jsonData["items"] != null)
		{
			for (Int32 i = 0; i < jsonData["items"].Count && i < 256; i++)
			{
				if (jsonData["items"][i].AsInt != 255)
				{
					JSONClass itemClass = jsonData["items"][i].AsObject;
					ffglobal.item[i].id = (Byte)itemClass["id"].AsInt;
					ffglobal.item[i].count = (Byte)itemClass["count"].AsInt;
				}
			}
		}
		if (jsonData["rareItems"] != null)
			for (Int32 i = 0; i < jsonData["rareItems"].Count && i < 64; i++)
				ffglobal.rare_item[i] = (Byte)jsonData["rareItems"][i].AsInt;
	}

	private void ParseSettingDataToJson(SettingsState data, JSONClass dataNode, JSONClass schemaNode)
	{
		JSONClass dataSettingRootClass = new JSONClass();
		JSONClass dataSettingClass = new JSONClass();
		dataSettingClass.Add("sound", data.cfg.sound.ToString());
		dataSettingClass.Add("sound_effect", data.cfg.sound_effect.ToString());
		dataSettingClass.Add("control", data.cfg.control.ToString());
		dataSettingClass.Add("cursor", data.cfg.cursor.ToString());
		dataSettingClass.Add("atb", data.cfg.atb.ToString());
		dataSettingClass.Add("camera", data.cfg.camera.ToString());
		dataSettingClass.Add("move", data.cfg.move.ToString());
		dataSettingClass.Add("vibe", data.cfg.vibe.ToString());
		dataSettingClass.Add("btl_speed", data.cfg.btl_speed.ToString());
		dataSettingClass.Add("fld_msg", data.cfg.fld_msg.ToString());
		dataSettingClass.Add("here_icon", data.cfg.here_icon.ToString());
		dataSettingClass.Add("win_type", data.cfg.win_type.ToString());
		dataSettingClass.Add("target_win", data.cfg.target_win.ToString());
		dataSettingClass.Add("control_data", data.cfg.control_data.ToString());
		JSONArray dataKeyboardArray = new JSONArray();
		for (Int32 i = 0; i < data.cfg.control_data_keyboard.Length; i++)
			dataKeyboardArray.Add(((Int32)data.cfg.control_data_keyboard[i]).ToString());
		dataSettingClass.Add("control_data_keyboard", dataKeyboardArray);
		JSONArray dataJoystickArray = new JSONArray();
		for (Int32 i = 0; i < (Int32)data.cfg.control_data_joystick.Length; i++)
			dataJoystickArray.Add(data.cfg.control_data_joystick[i]);
		dataSettingClass.Add("control_data_joystick", dataJoystickArray);
		dataSettingClass.Add("camera", data.cfg.camera.ToString());
		dataSettingClass.Add("skip_btl_camera", data.cfg.skip_btl_camera.ToString());
		dataSettingRootClass.Add("cfg", dataSettingClass);
		dataSettingRootClass.Add("time", (-1f).ToString(CultureInfo.InvariantCulture));
		dataNode.Add("50000_Setting", dataSettingRootClass);
		JSONClass schemaSettingRootClass = new JSONClass();
		JSONClass schemaSettingClass = new JSONClass();
		schemaSettingClass.Add("sound", data.cfg.sound.GetType().ToString());
		schemaSettingClass.Add("sound_effect", data.cfg.sound_effect.GetType().ToString());
		schemaSettingClass.Add("control", data.cfg.control.GetType().ToString());
		schemaSettingClass.Add("cursor", data.cfg.cursor.GetType().ToString());
		schemaSettingClass.Add("atb", data.cfg.atb.GetType().ToString());
		schemaSettingClass.Add("camera", data.cfg.camera.GetType().ToString());
		schemaSettingClass.Add("move", data.cfg.move.GetType().ToString());
		schemaSettingClass.Add("vibe", data.cfg.vibe.GetType().ToString());
		schemaSettingClass.Add("btl_speed", data.cfg.btl_speed.GetType().ToString());
		schemaSettingClass.Add("fld_msg", data.cfg.fld_msg.GetType().ToString());
		schemaSettingClass.Add("here_icon", data.cfg.here_icon.GetType().ToString());
		schemaSettingClass.Add("win_type", data.cfg.win_type.GetType().ToString());
		schemaSettingClass.Add("target_win", data.cfg.target_win.GetType().ToString());
		schemaSettingClass.Add("control_data", data.cfg.control_data.GetType().ToString());
		JSONArray schemaKeyboardArray = new JSONArray();
		for (Int32 i = 0; i < data.cfg.control_data_keyboard.Length; i++)
			schemaKeyboardArray.Add(typeof(Int32).ToString());
		schemaSettingClass.Add("control_data_keyboard", schemaKeyboardArray);
		JSONArray schemaJoystickArray = new JSONArray();
		for (Int32 i = 0; i < data.cfg.control_data_joystick.Length; i++)
			schemaJoystickArray.Add(data.cfg.control_data_joystick[i].GetType().ToString());
		schemaSettingClass.Add("control_data_joystick", schemaJoystickArray);
		schemaSettingClass.Add("camera", data.cfg.camera.GetType().ToString());
		schemaSettingClass.Add("skip_btl_camera", data.cfg.skip_btl_camera.GetType().ToString());
		schemaSettingRootClass.Add("cfg", schemaSettingClass);
		schemaSettingRootClass.Add("time", typeof(Single).ToString());
		schemaNode.Add("50000_Setting", schemaSettingRootClass);
	}

	private void ParseSettingJsonToData(JSONNode jsonData)
	{
		SettingsState settings = FF9StateSystem.Settings;
		if (jsonData["cfg"] != null)
		{
			JSONClass dataSettingClass = jsonData["cfg"].AsObject;
			FF9CFG cfg = settings.cfg;
			if (dataSettingClass["sound"] != null)
				cfg.sound = (UInt64)dataSettingClass["sound"].AsInt;
			if (dataSettingClass["sound_effect"] != null)
				cfg.sound_effect = (UInt64)dataSettingClass["sound_effect"].AsInt;
			if (dataSettingClass["control"] != null)
				cfg.control = (UInt64)dataSettingClass["control"].AsInt;
			if (dataSettingClass["cursor"] != null)
				cfg.cursor = (UInt64)dataSettingClass["cursor"].AsInt;
			if (dataSettingClass["atb"] != null)
				cfg.atb = (UInt64)dataSettingClass["atb"].AsInt;
			if (dataSettingClass["camera"] != null)
				cfg.camera = (UInt64)dataSettingClass["camera"].AsInt;
			if (dataSettingClass["move"] != null)
				cfg.move = (UInt64)dataSettingClass["move"].AsInt;
			if (dataSettingClass["vibe"] != null)
				cfg.vibe = (UInt64)dataSettingClass["vibe"].AsInt;
			if (dataSettingClass["btl_speed"] != null)
				cfg.btl_speed = (UInt64)dataSettingClass["btl_speed"].AsInt;
			if (dataSettingClass["fld_msg"] != null)
				cfg.fld_msg = (UInt64)dataSettingClass["fld_msg"].AsInt;
			if (dataSettingClass["here_icon"] != null)
				cfg.here_icon = (UInt64)dataSettingClass["here_icon"].AsInt;
			if (dataSettingClass["win_type"] != null)
				cfg.win_type = (UInt64)dataSettingClass["win_type"].AsInt;
			if (dataSettingClass["target_win"] != null)
				cfg.target_win = (UInt64)dataSettingClass["target_win"].AsInt;
			if (dataSettingClass["control_data"] != null)
				cfg.control_data = (UInt64)dataSettingClass["control_data"].AsInt;
			if (dataSettingClass["control_data_keyboard"] != null)
			{
				JSONArray keyboardArray = dataSettingClass["control_data_keyboard"].AsArray;
				if (cfg.control_data_keyboard == null)
					cfg.control_data_keyboard = new KeyCode[keyboardArray.Count];
				for (Int32 i = 0; i < keyboardArray.Count; i++)
					cfg.control_data_keyboard[i] = (KeyCode)keyboardArray[i].AsInt;
			}
			if (dataSettingClass["control_data_joystick"] != null)
			{
				JSONArray joystickArray = dataSettingClass["control_data_joystick"].AsArray;
				if (cfg.control_data_joystick == null)
					cfg.control_data_joystick = new String[joystickArray.Count];
				for (Int32 j = 0; j < joystickArray.Count; j++)
					cfg.control_data_joystick[j] = joystickArray[j];
			}
			if (dataSettingClass["camera"] != null)
				settings.cfg.camera = (UInt64)dataSettingClass["camera"].AsInt;
			if (dataSettingClass["skip_btl_camera"] != null)
				settings.cfg.skip_btl_camera = (UInt64)dataSettingClass["skip_btl_camera"].AsInt;
		}
	}

	private void ParseSoundDataToJson(SoundState data, JSONClass dataNode, JSONClass schemaNode)
	{
		dataNode.Add("60000_Sound", new JSONClass
		{
			{
				"auto_save_bgm_id",
				data.auto_save_bgm_id.ToString()
			}
		});
		schemaNode.Add("60000_Sound", new JSONClass
		{
			{
				"auto_save_bgm_id",
				data.auto_save_bgm_id.GetType().ToString()
			}
		});
	}

	private void ParseSoundJsonToData(JSONNode jsonData)
	{
		if (jsonData["auto_save_bgm_id"] != null)
			FF9StateSystem.Sound.auto_save_bgm_id = jsonData["auto_save_bgm_id"].AsInt;
	}

	private void ParseWorldDataToJson(WorldState data_unused, JSONClass dataNode, JSONClass schemaNode)
	{
		ff9.FF9SAVE_WORLD worldSave;
		ff9.SaveWorld(out worldSave);
		dataNode.Add("70000_World", new JSONClass
		{
			{
				"data.cameraState.rotationMax",
				worldSave.cameraState.rotationMax.ToString()
			},
			{
				"data.cameraState.upperCounter",
				worldSave.cameraState.upperCounter.ToString()
			},
			{
				"data.cameraState.upperCounterSpeed",
				worldSave.cameraState.upperCounterSpeed.ToString()
			},
			{
				"data.cameraState.upperCounterForce",
				worldSave.cameraState.upperCounterForce.ToString()
			},
			{
				"data.cameraState.rotation",
				worldSave.cameraState.rotation.ToString()
			},
			{
				"data.cameraState.rotationRev",
				worldSave.cameraState.rotationRev.ToString()
			},
			{
				"data.hintmap",
				worldSave.hintmap.ToString()
			}
		});
		schemaNode.Add("70000_World", new JSONClass
		{
			{
				"data.cameraState.rotationMax",
				worldSave.cameraState.rotationMax.GetType().ToString()
			},
			{
				"data.cameraState.upperCounter",
				worldSave.cameraState.upperCounter.GetType().ToString()
			},
			{
				"data.cameraState.upperCounterSpeed",
				worldSave.cameraState.upperCounterSpeed.GetType().ToString()
			},
			{
				"data.cameraState.upperCounterForce",
				worldSave.cameraState.upperCounterForce.GetType().ToString()
			},
			{
				"data.cameraState.rotation",
				worldSave.cameraState.rotation.GetType().ToString()
			},
			{
				"data.cameraState.rotationRev",
				worldSave.cameraState.rotationRev.GetType().ToString()
			},
			{
				"data.hintmap",
				worldSave.hintmap.GetType().ToString()
			}
		});
	}

	private void ParseWorldJsonToData(JSONNode jsonData)
	{
		ff9.FF9SAVE_WORLD worldSave = new ff9.FF9SAVE_WORLD();
		if (jsonData["data.cameraState.rotationMax"] != null)
			worldSave.cameraState.rotationMax = jsonData["data.cameraState.rotationMax"].AsFloat;
		if (jsonData["data.cameraState.upperCounter"] != null)
			worldSave.cameraState.upperCounter = (Int16)jsonData["data.cameraState.upperCounter"].AsInt;
		if (jsonData["data.cameraState.upperCounterSpeed"] != null)
			worldSave.cameraState.upperCounterSpeed = jsonData["data.cameraState.upperCounterSpeed"].AsInt;
		if (jsonData["data.cameraState.upperCounterForce"] != null)
			worldSave.cameraState.upperCounterForce = jsonData["data.cameraState.upperCounterForce"].AsBool;
		if (jsonData["data.cameraState.rotation"] != null)
			worldSave.cameraState.rotation = jsonData["data.cameraState.rotation"].AsFloat;
		if (jsonData["data.cameraState.rotationRev"] != null)
			worldSave.cameraState.rotationRev = jsonData["data.cameraState.rotationRev"].AsFloat;
		if (jsonData["data.cameraState.rotationRev"] != null)
			worldSave.hintmap = jsonData["data.cameraState.rotationRev"].AsUInt;
		ff9.LoadWorld(worldSave);
	}

	private void ParseAchievementDataToJson(AchievementState data, JSONClass dataNode, JSONClass schemaNode)
	{
		JSONClass dataAchievementClass = new JSONClass();
		JSONArray dataATEArray = new JSONArray();
		for (Int32 i = 0; i < 100; i++)
			dataATEArray.Add(data.AteCheck[i].ToString());
		dataAchievementClass.Add("AteCheckArray", dataATEArray);
		JSONArray dataEvtArray = new JSONArray();
		for (Int32 i = 0; i < 17; i++)
			dataEvtArray.Add(data.EvtReservedArray[i].ToString());
		dataAchievementClass.Add("EvtReservedArray", dataEvtArray);
		dataAchievementClass.Add("blkMag_no", data.blkMag_no.ToString());
		dataAchievementClass.Add("whtMag_no", data.whtMag_no.ToString());
		dataAchievementClass.Add("bluMag_no", data.bluMag_no.ToString());
		dataAchievementClass.Add("summon_no", data.summon_no.ToString());
		dataAchievementClass.Add("enemy_no", data.enemy_no.ToString());
		dataAchievementClass.Add("backAtk_no", data.backAtk_no.ToString());
		dataAchievementClass.Add("defence_no", data.defence_no.ToString());
		dataAchievementClass.Add("trance_no", data.trance_no.ToString());
		JSONArray dataAbilitiesArray = new JSONArray();
		foreach (Int32 abil in data.abilities)
			dataAbilitiesArray.Add(abil.ToString());
		for (Int32 i = data.abilities.Count; i < 221; i++)
			dataAbilitiesArray.Add("-1");
		dataAchievementClass.Add("abilities", dataAbilitiesArray);
		JSONArray dataPassiveArray = new JSONArray();
		foreach (Int32 abil in data.passiveAbilities)
			dataPassiveArray.Add(abil.ToString());
		for (Int32 i = data.passiveAbilities.Count; i < 63; i++)
			dataPassiveArray.Add("-1");
		dataAchievementClass.Add("passiveAbilities", dataPassiveArray);
		dataAchievementClass.Add("synthesisCount", data.synthesisCount.ToString());
		dataAchievementClass.Add("AuctionTime", data.AuctionTime.ToString());
		dataAchievementClass.Add("StiltzkinBuy", data.StiltzkinBuy.ToString());
		JSONArray dataCardArray = new JSONArray();
		foreach (Int32 card in data.QuadmistWinList)
			dataCardArray.Add(card.ToString());
		for (Int32 i = data.QuadmistWinList.Count; i < 300; i++)
			dataCardArray.Add("-1");
		dataAchievementClass.Add("QuadmistWinList", dataCardArray);
		dataNode.Add("80000_Achievement", dataAchievementClass);
		JSONClass schemaAchievementClass = new JSONClass();
		JSONArray schemaATEArray = new JSONArray();
		for (Int32 i = 0; i < 100; i++)
			schemaATEArray.Add(data.AteCheck[i].GetType().ToString());
		schemaAchievementClass.Add("AteCheckArray", schemaATEArray);
		JSONArray schemaEvtArray = new JSONArray();
		for (Int32 i = 0; i < 17; i++)
			schemaEvtArray.Add(data.EvtReservedArray[i].GetType().ToString());
		schemaAchievementClass.Add("EvtReservedArray", schemaEvtArray);
		schemaAchievementClass.Add("blkMag_no", data.blkMag_no.GetType().ToString());
		schemaAchievementClass.Add("whtMag_no", data.whtMag_no.GetType().ToString());
		schemaAchievementClass.Add("bluMag_no", data.bluMag_no.GetType().ToString());
		schemaAchievementClass.Add("summon_no", data.summon_no.GetType().ToString());
		schemaAchievementClass.Add("enemy_no", data.enemy_no.GetType().ToString());
		schemaAchievementClass.Add("backAtk_no", data.backAtk_no.GetType().ToString());
		schemaAchievementClass.Add("defence_no", data.defence_no.GetType().ToString());
		schemaAchievementClass.Add("trance_no", data.trance_no.GetType().ToString());
		JSONArray schemaAbilitiesArray = new JSONArray();
		for (Int32 i = 0; i < 221; i++)
			schemaAbilitiesArray.Add(SharedDataBytesStorage.TypeInt32);
		schemaAchievementClass.Add("abilities", schemaAbilitiesArray);
		JSONArray schemaPassiveArray = new JSONArray();
		for (Int32 i = 0; i < 63; i++)
			schemaPassiveArray.Add(SharedDataBytesStorage.TypeInt32);
		schemaAchievementClass.Add("passiveAbilities", schemaPassiveArray);
		schemaAchievementClass.Add("synthesisCount", data.synthesisCount.GetType().ToString());
		schemaAchievementClass.Add("AuctionTime", data.AuctionTime.GetType().ToString());
		schemaAchievementClass.Add("StiltzkinBuy", data.StiltzkinBuy.GetType().ToString());
		JSONArray schemaCardArray = new JSONArray();
		for (Int32 i = 0; i < 300; i++)
			schemaCardArray.Add(typeof(Int32).ToString());
		schemaAchievementClass.Add("QuadmistWinList", schemaCardArray);
		schemaNode.Add("80000_Achievement", schemaAchievementClass);
	}

	private void ParseAchievementJsonToData(JSONNode jsonData)
	{
		AchievementState achievement = FF9StateSystem.Achievement;
		achievement.AteCheck = new Int32[100];
		if (jsonData["AteCheckArray"] != null)
		{
			JSONArray ATEArray = jsonData["AteCheckArray"].AsArray;
			for (Int32 i = 0; i < ATEArray.Count; i++)
				achievement.AteCheck[i] = ATEArray[i].AsInt;
		}
		achievement.EvtReservedArray = new Int32[17];
		if (jsonData["EvtReservedArray"] != null)
		{
			JSONArray EvtArray = jsonData["EvtReservedArray"].AsArray;
			for (Int32 i = 0; i < EvtArray.Count; i++)
				achievement.EvtReservedArray[i] = EvtArray[i].AsInt;
		}
		if (jsonData["blkMag_no"] != null)
			achievement.blkMag_no = jsonData["blkMag_no"].AsInt;
		if (jsonData["whtMag_no"] != null)
			achievement.whtMag_no = jsonData["whtMag_no"].AsInt;
		if (jsonData["bluMag_no"] != null)
			achievement.bluMag_no = jsonData["bluMag_no"].AsInt;
		if (jsonData["summon_no"] != null)
			achievement.summon_no = jsonData["summon_no"].AsInt;
		if (jsonData["enemy_no"] != null)
			achievement.enemy_no = jsonData["enemy_no"].AsInt;
		if (jsonData["backAtk_no"] != null)
			achievement.backAtk_no = jsonData["backAtk_no"].AsInt;
		if (jsonData["defence_no"] != null)
			achievement.defence_no = jsonData["defence_no"].AsInt;
		if (jsonData["trance_no"] != null)
			achievement.trance_no = jsonData["trance_no"].AsInt;
		achievement.abilities = new HashSet<Int32>();
		if (jsonData["abilities"] != null)
		{
			JSONArray abilityArray = jsonData["abilities"].AsArray;
			for (Int32 i = 0; i < abilityArray.Count; i++)
			{
				Int32 abil = abilityArray[i].AsInt;
				if (abil != -1)
					achievement.abilities.Add(abil);
			}
		}
		achievement.passiveAbilities = new HashSet<Int32>();
		if (jsonData["passiveAbilities"] != null)
		{
			JSONArray passiveArray = jsonData["passiveAbilities"].AsArray;
			for (Int32 i = 0; i < passiveArray.Count; i++)
			{
				Int32 abil = passiveArray[i].AsInt;
				if (abil != -1)
					achievement.passiveAbilities.Add(abil);
			}
		}
		if (jsonData["synthesisCount"] != null)
			achievement.synthesisCount = jsonData["synthesisCount"].AsInt;
		if (jsonData["AuctionTime"] != null)
			achievement.AuctionTime = jsonData["AuctionTime"].AsInt;
		if (jsonData["StiltzkinBuy"] != null)
			achievement.StiltzkinBuy = jsonData["StiltzkinBuy"].AsInt;
		achievement.QuadmistWinList = new HashSet<Int32>();
		if (jsonData["QuadmistWinList"] != null)
		{
			JSONArray cardArray = jsonData["QuadmistWinList"].AsArray;
			for (Int32 i = 0; i < cardArray.Count; i++)
			{
				Int32 card = cardArray[i].AsInt;
				if (card != -1)
					achievement.QuadmistWinList.Add(card);
			}
		}
	}

	private PLAYER GetPlayerFromOldSave(Int32 oldSlot, Boolean isTemp)
	{
		if (oldSlot == (Int32)CharacterId.Quina)
		{
			if (isTemp)
				return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Cinna);
			FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Quina).info.sub_replaced = true;
		}
		else if (oldSlot == (Int32)CharacterId.Eiko)
		{
			if (isTemp)
				return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Marcus);
			FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Eiko).info.sub_replaced = true;
		}
		else if (oldSlot == (Int32)CharacterId.Amarant)
		{
			if (isTemp)
				return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Blank);
			FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Amarant).info.sub_replaced = true;
		}
		else if (oldSlot == (Int32)CharacterOldIndex.Beatrix)
		{
			return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Beatrix);
		}
		return FF9StateSystem.Common.FF9.GetPlayer((CharacterId)oldSlot);
	}

	private Int32 SelectOldSaveSlot(PLAYER player)
	{
		CharacterId charId = player.info.slot_no;
		if (charId <= CharacterId.Freya)
			return (Int32)charId;
		if (charId == CharacterId.Beatrix)
			return (Int32)CharacterOldIndex.Beatrix;
		if (charId <= CharacterId.Amarant)
		{
			if (player.info.sub_replaced)
				return (Int32)charId;
			return -1;
		}
		if (charId <= CharacterId.Blank)
		{
			if (FF9StateSystem.Common.FF9.GetPlayer(charId - 3).info.sub_replaced)
				return -1;
			return (Int32)charId - 3;
		}
		return -1;
	}
}
