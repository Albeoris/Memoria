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
		Int32 num = size / 4;
		JSONArray jsonarray = new JSONArray();
		for (Int32 i = 0; i < num; i++)
		{
			jsonarray.Add("0");
		}
		rootNode.Add(moduleName, jsonarray);
		JSONArray jsonarray2 = new JSONArray();
		Int32 num2 = 0;
		String s = num2.GetType().ToString();
		for (Int32 j = 0; j < num; j++)
		{
			jsonarray2.Add(s);
		}
		schemaNode.Add(moduleName, jsonarray2);
	}

	public void ParseFromFF9StateSystem(JSONClass rootNode, JSONClass schemaNode)
	{
		JSONClass jsonclass = new JSONClass();
		rootNode.Add("Data", jsonclass);
		JSONClass jsonclass2 = new JSONClass();
		schemaNode.Add("Data", jsonclass2);
		this.ParseStateDataToJson(jsonclass, jsonclass2);
		this.ParseEventDataToJson(FF9StateSystem.EventState, jsonclass, jsonclass2);
		this.ParseQuadMistDataToJson(FF9StateSystem.MiniGame.SavedData, jsonclass, jsonclass2);
		this.ParseCommonDataToJson(FF9StateSystem.Common.FF9, jsonclass, jsonclass2);
		this.ParseSettingDataToJson(FF9StateSystem.Settings, jsonclass, jsonclass2);
		this.ParseSoundDataToJson(FF9StateSystem.Sound, jsonclass, jsonclass2);
		this.ParseWorldDataToJson(FF9StateSystem.World, jsonclass, jsonclass2);
		this.ParseAchievementDataToJson(FF9StateSystem.Achievement, jsonclass, jsonclass2);
		this.AddReservedBuffer(jsonclass, jsonclass2, "91000_State", 512);
		this.AddReservedBuffer(jsonclass, jsonclass2, "92000_Event", 512);
		this.AddReservedBuffer(jsonclass, jsonclass2, "93000_MiniGame", 512);
		this.AddReservedBuffer(jsonclass, jsonclass2, "94000_Common", 512);
		JSONClass jsonclass3 = new JSONClass();
		PLAYER[] player = FF9StateSystem.Common.FF9.player;
		JSONArray jsonarray = new JSONArray();
		for (Int32 i = 0; i < 9; i++)
		{
			jsonarray.Add(player[i].bonus.cap.ToString());
		}
		jsonclass3.Add("00001_player_bonus", jsonarray);
		jsonclass.Add("94000_Common", jsonclass3);
		JSONClass jsonclass4 = new JSONClass();
		PLAYER[] player2 = FF9StateSystem.Common.FF9.player;
		JSONArray jsonarray2 = new JSONArray();
		for (Int32 j = 0; j < 9; j++)
		{
			jsonarray2.Add(((UInt32)player2[j].bonus.cap).GetType().ToString());
		}
		jsonclass4.Add("00001_player_bonus", jsonarray2);
		jsonclass2.Add("94000_Common", jsonclass4);
		this.AddReservedBuffer(jsonclass["94000_Common"].AsObject, jsonclass2["94000_Common"].AsObject, "99999_ReservedBuffer", 476);
		this.AddReservedBuffer(jsonclass, jsonclass2, "95000_Setting", 512);
		jsonclass.Add("95000_Setting", new JSONClass
		{
			{
				"00001_time",
				FF9StateSystem.Settings.time.ToString()
			}
		});
		jsonclass2.Add("95000_Setting", new JSONClass
		{
			{
				"00001_time",
				FF9StateSystem.Settings.time.GetType().ToString()
			}
		});
		this.AddReservedBuffer(jsonclass["95000_Setting"].AsObject, jsonclass2["95000_Setting"].AsObject, "99999_ReservedBuffer", 504);
		this.AddReservedBuffer(jsonclass, jsonclass2, "96000_Sound", 512);
		this.AddReservedBuffer(jsonclass, jsonclass2, "97000_World", 512);
		this.AddReservedBuffer(jsonclass, jsonclass2, "98000_Achievement", 512);
		jsonclass.Add("98000_Achievement", new JSONClass
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
		jsonclass2.Add("98000_Achievement", new JSONClass
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
		this.AddReservedBuffer(jsonclass["98000_Achievement"].AsObject, jsonclass2["98000_Achievement"].AsObject, "99999_ReservedBuffer", 492);
		this.AddReservedBuffer(jsonclass, jsonclass2, "99000_Other", 1536);
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
		JSONNode jsonnode = rootNode["Data"];
		if (jsonnode == null)
		{
			return;
		}
		if (jsonnode["10000_State"] != null)
		{
			this.ParseStateJsonToData(jsonnode["10000_State"]);
		}
		if (jsonnode["20000_Event"] != null)
		{
			this.ParseEventJsonToData(jsonnode["20000_Event"]);
		}
		if (jsonnode["30000_MiniGame"] != null)
		{
			FF9StateSystem.MiniGame.SavedData = this.ParseQuadMistJsonToData(jsonnode["30000_MiniGame"]);
		}
		if (jsonnode["40000_Common"] != null)
		{
			this.ParseCommonJsonToData(jsonnode["40000_Common"]);
		}
		if (jsonnode["50000_Setting"] != null)
		{
			this.ParseSettingJsonToData(jsonnode["50000_Setting"]);
		}
		if (jsonnode["60000_Sound"] != null)
		{
			this.ParseSoundJsonToData(jsonnode["60000_Sound"]);
		}
		if (jsonnode["70000_World"] != null)
		{
			this.ParseWorldJsonToData(jsonnode["70000_World"]);
		}
		if (jsonnode["80000_Achievement"] != null)
		{
			this.ParseAchievementJsonToData(jsonnode["80000_Achievement"]);
		}
		if (jsonnode["94000_Common"]["00001_player_bonus"] != null)
		{
			for (Int32 i = 0; i < 9; i++)
			{
				PLAYER player = FF9StateSystem.Common.FF9.player[i];
				UInt16 num = (UInt16)jsonnode["94000_Common"]["00001_player_bonus"][i].AsUInt;
				if (num == 0)
				{
					player.bonus.cap = (UInt16)((player.level - 1) * 5);
					if (player.info.serial_no == 10 || player.info.serial_no == 11)
					{
						FF9LEVEL_BONUS bonus = player.bonus;
						bonus.cap = (UInt16)(bonus.cap + this.getExtraCap(player));
					}
				}
				else
				{
					player.bonus.cap = num;
				}
				player.ValidateMaxStone();
			}
		}
		if (jsonnode["50000_Setting"] != null && jsonnode["95000_Setting"] != null && jsonnode["50000_Setting"]["time"] != null && jsonnode["95000_Setting"]["00001_time"] != null)
		{
			if ((Double)jsonnode["50000_Setting"]["time"].AsFloat >= 0.0)
			{
				FF9StateSystem.Settings.time = (Double)jsonnode["50000_Setting"]["time"].AsFloat;
			}
			else
			{
				FF9StateSystem.Settings.time = jsonnode["95000_Setting"]["00001_time"].AsDouble;
			}
		}
		if (jsonnode["98000_Achievement"]["00001_abnormal_status"] != null)
		{
			FF9StateSystem.Achievement.abnormal_status = jsonnode["98000_Achievement"]["00001_abnormal_status"].AsUInt;
		}
		if (jsonnode["98000_Achievement"]["00002_summon_shiva"] != null)
		{
			FF9StateSystem.Achievement.summon_shiva = jsonnode["98000_Achievement"]["00002_summon_shiva"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["00003_summon_ifrit"] != null)
		{
			FF9StateSystem.Achievement.summon_ifrit = jsonnode["98000_Achievement"]["00003_summon_ifrit"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["00004_summon_ramuh"] != null)
		{
			FF9StateSystem.Achievement.summon_ramuh = jsonnode["98000_Achievement"]["00004_summon_ramuh"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["00005_summon_carbuncle_reflector"] != null)
		{
			FF9StateSystem.Achievement.summon_carbuncle_reflector = jsonnode["98000_Achievement"]["00005_summon_carbuncle_reflector"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["00006_summon_carbuncle_haste"] != null)
		{
			FF9StateSystem.Achievement.summon_carbuncle_haste = jsonnode["98000_Achievement"]["00006_summon_carbuncle_haste"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["00007_summon_carbuncle_protect"] != null)
		{
			FF9StateSystem.Achievement.summon_carbuncle_protect = jsonnode["98000_Achievement"]["00007_summon_carbuncle_protect"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["00008_summon_carbuncle_shell"] != null)
		{
			FF9StateSystem.Achievement.summon_carbuncle_shell = jsonnode["98000_Achievement"]["00008_summon_carbuncle_shell"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["00009_summon_fenrir_earth"] != null)
		{
			FF9StateSystem.Achievement.summon_fenrir_earth = jsonnode["98000_Achievement"]["00009_summon_fenrir_earth"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["000010_summon_fenrir_wind"] != null)
		{
			FF9StateSystem.Achievement.summon_fenrir_wind = jsonnode["98000_Achievement"]["000010_summon_fenrir_wind"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["000011_summon_atomos"] != null)
		{
			FF9StateSystem.Achievement.summon_atomos = jsonnode["98000_Achievement"]["000011_summon_atomos"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["000012_summon_phoenix"] != null)
		{
			FF9StateSystem.Achievement.summon_phoenix = jsonnode["98000_Achievement"]["000012_summon_phoenix"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["000013_summon_leviathan"] != null)
		{
			FF9StateSystem.Achievement.summon_leviathan = jsonnode["98000_Achievement"]["000013_summon_leviathan"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["000014_summon_odin"] != null)
		{
			FF9StateSystem.Achievement.summon_odin = jsonnode["98000_Achievement"]["000014_summon_odin"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["000015_summon_madeen"] != null)
		{
			FF9StateSystem.Achievement.summon_madeen = jsonnode["98000_Achievement"]["000015_summon_madeen"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["000016_summon_bahamut"] != null)
		{
			FF9StateSystem.Achievement.summon_bahamut = jsonnode["98000_Achievement"]["000016_summon_bahamut"].AsBool;
		}
		if (jsonnode["98000_Achievement"]["000017_summon_arc"] != null)
		{
			FF9StateSystem.Achievement.summon_arc = jsonnode["98000_Achievement"]["000017_summon_arc"].AsBool;
		}
	}

	private UInt16 getExtraCap(PLAYER player)
	{
		Byte level = player.level;
		UInt16 result;
		if (level <= 10)
		{
			result = 10;
		}
		else if (level <= 20)
		{
			result = 20;
		}
		else if (level <= 30)
		{
			result = 30;
		}
		else if (level <= 40)
		{
			result = 40;
		}
		else if (level <= 50)
		{
			result = 50;
		}
		else if (level <= 60)
		{
			result = 60;
		}
		else if (level <= 70)
		{
			result = 70;
		}
		else if (level <= 80)
		{
			result = 80;
		}
		else if (level <= 90)
		{
			result = 90;
		}
		else
		{
			result = 99;
		}
		return result;
	}

	private void ParseStateJsonToData(JSONNode jsonData)
	{
		FF9StateGlobal ff = FF9StateSystem.Common.FF9;
		FF9StateSystem instance = PersistenSingleton<FF9StateSystem>.Instance;
		if (jsonData["mode"] != null)
		{
			instance.mode = (Byte)jsonData["mode"].AsInt;
		}
		if (jsonData["prevMode"] != null)
		{
			instance.prevMode = (Byte)jsonData["prevMode"].AsInt;
		}
		if (jsonData["fldMapNo"] != null)
		{
			ff.fldMapNo = (Int16)jsonData["fldMapNo"].AsInt;
		}
		if (jsonData["fldLocNo"] != null)
		{
			ff.fldLocNo = (Int16)jsonData["fldLocNo"].AsInt;
		}
		if (jsonData["btlMapNo"] != null)
		{
			ff.btlMapNo = (Int16)jsonData["btlMapNo"].AsInt;
		}
		if (jsonData["btlSubMapNo"] != null)
		{
			ff.btlSubMapNo = (SByte)jsonData["btlSubMapNo"].AsInt;
		}
		if (jsonData["wldMapNo"] != null)
		{
			ff.wldMapNo = (Int16)jsonData["wldMapNo"].AsInt;
		}
		if (jsonData["wldLocNo"] != null)
		{
			ff.wldLocNo = (Int16)((SByte)jsonData["wldLocNo"].AsInt);
		}
		if (jsonData["timeCounter"] != null)
		{
			Int32 timeFromAutoSave = (Int32)jsonData["timeCounter"].AsFloat;
			TimerUI.SetTimeFromAutoSave(timeFromAutoSave);
		}
		if (jsonData["timerControl"] != null)
		{
			ff.timerControl = jsonData["timerControl"].AsBool;
		}
		if (jsonData["timerDisplay"] != null)
		{
			ff.timerDisplay = jsonData["timerDisplay"].AsBool;
		}
	}

	private void ParseStateDataToJson(JSONClass dataNode, JSONClass dataSchemaNode)
	{
		FF9StateGlobal ff = FF9StateSystem.Common.FF9;
		FF9StateSystem instance = PersistenSingleton<FF9StateSystem>.Instance;
		Single time = TimerUI.Time;
		dataNode.Add("10000_State", new JSONClass
		{
			{
				"mode",
				instance.mode.ToString()
			},
			{
				"prevMode",
				instance.prevMode.ToString()
			},
			{
				"fldMapNo",
				ff.fldMapNo.ToString()
			},
			{
				"fldLocNo",
				ff.fldLocNo.ToString()
			},
			{
				"btlMapNo",
				ff.btlMapNo.ToString()
			},
			{
				"btlSubMapNo",
				ff.btlSubMapNo.ToString()
			},
			{
				"wldMapNo",
				ff.wldMapNo.ToString()
			},
			{
				"wldLocNo",
				ff.wldLocNo.ToString()
			},
			{
				"timeCounter",
				time.ToString()
			},
			{
				"timerControl",
				ff.timerControl.ToString()
			},
			{
				"timerDisplay",
				ff.timerDisplay.ToString()
			}
		});
		dataSchemaNode.Add("10000_State", new JSONClass
		{
			{
				"mode",
				instance.mode.GetType().ToString()
			},
			{
				"prevMode",
				instance.prevMode.GetType().ToString()
			},
			{
				"fldMapNo",
				ff.fldMapNo.GetType().ToString()
			},
			{
				"fldLocNo",
				ff.fldLocNo.GetType().ToString()
			},
			{
				"btlMapNo",
				ff.btlMapNo.GetType().ToString()
			},
			{
				"btlSubMapNo",
				ff.btlSubMapNo.GetType().ToString()
			},
			{
				"wldMapNo",
				ff.wldMapNo.GetType().ToString()
			},
			{
				"wldLocNo",
				ff.wldLocNo.GetType().ToString()
			},
			{
				"timeCounter",
				time.GetType().ToString()
			},
			{
				"timerControl",
				ff.timerControl.GetType().ToString()
			},
			{
				"timerDisplay",
				ff.timerDisplay.GetType().ToString()
			}
		});
	}

	private void ParseEventJsonToData(JSONNode jsonData)
	{
		EventState eventState = FF9StateSystem.EventState;
		if (jsonData["gStepCount"] != null)
		{
			eventState.gStepCount = jsonData["gStepCount"].AsInt;
		}
		if (jsonData["gEventGlobal"] != null)
		{
			eventState.gEventGlobal = Convert.FromBase64String(jsonData["gEventGlobal"].Value);
		}
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
		FF9SAVE_MINIGAME ff9SAVE_MINIGAME = new FF9SAVE_MINIGAME();
		if (jsonData["sWin"] != null)
		{
			ff9SAVE_MINIGAME.sWin = (Int16)jsonData["sWin"].AsInt;
		}
		if (jsonData["sLose"] != null)
		{
			ff9SAVE_MINIGAME.sLose = (Int16)jsonData["sLose"].AsInt;
		}
		if (jsonData["sDraw"] != null)
		{
			ff9SAVE_MINIGAME.sDraw = (Int16)jsonData["sDraw"].AsInt;
		}
		if (jsonData["MiniGameCard"] != null)
		{
			Int32 count = jsonData["MiniGameCard"].Count;
			for (Int32 i = 0; i < count; i++)
			{
				JSONClass asObject = jsonData["MiniGameCard"][i].AsObject;
				if ((Byte)asObject["id"].AsInt != 255)
				{
					QuadMistCard quadMistCard = new QuadMistCard();
					if (asObject["id"] != null)
					{
						quadMistCard.id = (Byte)asObject["id"].AsInt;
					}
					if (asObject["side"] != null)
					{
						quadMistCard.side = (Byte)asObject["side"].AsInt;
					}
					if (asObject["atk"] != null)
					{
						quadMistCard.atk = (Byte)asObject["atk"].AsInt;
					}
					if (asObject["type"] != null)
					{
						String a = asObject["type"];
						Int32 asInt = asObject["type"].AsInt;
						if (a == "PHYSICAL" || asInt == 0)
						{
							quadMistCard.type = QuadMistCard.Type.PHYSICAL;
						}
						else if (a == "MAGIC" || asInt == 1)
						{
							quadMistCard.type = QuadMistCard.Type.MAGIC;
						}
						else if (a == "FLEXIABLE" || asInt == 2)
						{
							quadMistCard.type = QuadMistCard.Type.FLEXIABLE;
						}
						else if (a == "ASSAULT" || asInt == 3)
						{
							quadMistCard.type = QuadMistCard.Type.ASSAULT;
						}
					}
					if (asObject["pdef"] != null)
					{
						quadMistCard.pdef = (Byte)asObject["pdef"].AsInt;
					}
					if (asObject["mdef"] != null)
					{
						quadMistCard.mdef = (Byte)asObject["mdef"].AsInt;
					}
					if (asObject["cpoint"] != null)
					{
						quadMistCard.cpoint = (Byte)asObject["cpoint"].AsInt;
					}
					if (asObject["arrow"] != null)
					{
						quadMistCard.arrow = (Byte)asObject["arrow"].AsInt;
					}
					ff9SAVE_MINIGAME.MiniGameCard.Add(quadMistCard);
				}
			}
		}
		return ff9SAVE_MINIGAME;
	}

	private void ParseQuadMistDataToJson(FF9SAVE_MINIGAME data, JSONClass dataNode, JSONClass schemaNode)
	{
		JSONClass jsonclass = new JSONClass();
		jsonclass.Add("sWin", data.sWin.ToString());
		jsonclass.Add("sLose", data.sLose.ToString());
		jsonclass.Add("sDraw", data.sDraw.ToString());
		JSONArray jsonarray = new JSONArray();
		for (Int32 i = 0; i < 100; i++)
		{
			if (i < data.MiniGameCard.Count)
			{
				QuadMistCard quadMistCard = data.MiniGameCard[i];
				JSONClass jsonclass2 = new JSONClass();
				jsonclass2.Add("id", quadMistCard.id.ToString());
				jsonclass2.Add("side", "0");
				jsonclass2.Add("atk", quadMistCard.atk.ToString());
				JSONClass jsonclass3 = jsonclass2;
				String aKey = "type";
				Int32 type = (Int32)quadMistCard.type;
				jsonclass3.Add(aKey, type.ToString());
				jsonclass2.Add("pdef", quadMistCard.pdef.ToString());
				jsonclass2.Add("mdef", quadMistCard.mdef.ToString());
				jsonclass2.Add("cpoint", quadMistCard.cpoint.ToString());
				jsonclass2.Add("arrow", quadMistCard.arrow.ToString());
				jsonarray.Add(jsonclass2);
			}
			else
			{
				jsonarray.Add(new JSONClass
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
		jsonclass.Add("MiniGameCard", jsonarray);
		dataNode.Add("30000_MiniGame", jsonclass);
		JSONClass jsonclass4 = new JSONClass();
		jsonclass4.Add("sWin", data.sWin.GetType().ToString());
		jsonclass4.Add("sLose", data.sLose.GetType().ToString());
		jsonclass4.Add("sDraw", data.sDraw.GetType().ToString());
		JSONArray jsonarray2 = new JSONArray();
		for (Int32 j = 0; j < 100; j++)
		{
			QuadMistCard quadMistCard2 = new QuadMistCard();
			jsonarray2.Add(new JSONClass
			{
				{
					"id",
					quadMistCard2.id.GetType().ToString()
				},
				{
					"side",
					quadMistCard2.side.GetType().ToString()
				},
				{
					"atk",
					quadMistCard2.atk.GetType().ToString()
				},
				{
					"type",
					((Int32)quadMistCard2.type).GetType().ToString()
				},
				{
					"pdef",
					quadMistCard2.pdef.GetType().ToString()
				},
				{
					"mdef",
					quadMistCard2.mdef.GetType().ToString()
				},
				{
					"cpoint",
					quadMistCard2.cpoint.GetType().ToString()
				},
				{
					"arrow",
					quadMistCard2.arrow.GetType().ToString()
				}
			});
		}
		jsonclass4.Add("MiniGameCard", jsonarray2);
		schemaNode.Add("30000_MiniGame", jsonclass4);
	}

	private void ParseCommonDataToJson(FF9StateGlobal data, JSONClass dataNode, JSONClass schemaNode)
	{
		JSONClass jsonclass = new JSONClass();
		PLAYER[] player = data.player;
		PARTY_DATA party = data.party;
		JSONArray jsonarray = new JSONArray();
		for (Int32 i = 0; i < 9; i++)
		{
			PLAYER player2 = player[i];
			JSONClass jsonclass2 = new JSONClass();
			jsonclass2.Add("name", player2.GetRealName());
			jsonclass2.Add("category", player2.category.ToString());
			jsonclass2.Add("level", player2.level.ToString());
			jsonclass2.Add("exp", player2.exp.ToString());
			jsonclass2.Add("cur", new JSONClass
			{
				{
					"hp",
					player2.cur.hp.ToString()
				},
				{
					"mp",
					player2.cur.mp.ToString()
				},
				{
					"at",
					player2.cur.at.ToString()
				},
				{
					"at_coef",
					player2.cur.at_coef.ToString()
				},
				{
					"capa",
					player2.cur.capa.ToString()
				}
			});
			jsonclass2.Add("max", new JSONClass
			{
				{
					"hp",
					player2.max.hp.ToString()
				},
				{
					"mp",
					player2.max.mp.ToString()
				},
				{
					"at",
					player2.max.at.ToString()
				},
				{
					"at_coef",
					player2.max.at_coef.ToString()
				},
				{
					"capa",
					player2.max.capa.ToString()
				}
			});
			jsonclass2.Add("trance", player2.trance.ToString());
			jsonclass2.Add("web_bone", player2.wep_bone.ToString());
			jsonclass2.Add("elem", new JSONClass
			{
				{
					"dex",
					player2.elem.dex.ToString()
				},
				{
					"str",
					player2.elem.str.ToString()
				},
				{
					"mgc",
					player2.elem.mgc.ToString()
				},
				{
					"wpr",
					player2.elem.wpr.ToString()
				}
			});
			jsonclass2.Add("defence", new JSONClass
			{
				{
					"p_def",
					player2.defence.PhisicalDefence.ToString()
				},
				{
					"p_ev",
					player2.defence.PhisicalEvade.ToString()
				},
				{
					"m_def",
					player2.defence.MagicalDefence.ToString()
				},
				{
					"m_ev",
					player2.defence.MagicalEvade.ToString()
				}
			});
			jsonclass2.Add("basis", new JSONClass
			{
				{
					"max_hp",
					player2.basis.max_hp.ToString()
				},
				{
					"max_mp",
					player2.basis.max_mp.ToString()
				},
				{
					"dex",
					player2.basis.dex.ToString()
				},
				{
					"str",
					player2.basis.str.ToString()
				},
				{
					"mgc",
					player2.basis.mgc.ToString()
				},
				{
					"wpr",
					player2.basis.wpr.ToString()
				}
			});
			jsonclass2.Add("info", new JSONClass
			{
				{
					"slot_no",
					player2.info.slot_no.ToString()
				},
				{
					"serial_no",
					player2.info.serial_no.ToString()
				},
				{
					"row",
					player2.info.row.ToString()
				},
				{
					"win_pose",
					player2.info.win_pose.ToString()
				},
				{
					"party",
					player2.info.party.ToString()
				},
				{
					"menu_type",
					player2.info.menu_type.ToString()
				}
			});
			jsonclass2.Add("status", player2.status.ToString());
			JSONArray jsonarray2 = new JSONArray();
			for (Int32 j = 0; j < 5; j++)
			{
				jsonarray2.Add(player2.equip[j].ToString());
			}
			jsonclass2.Add("equip", jsonarray2);
			jsonclass2.Add("bonus", new JSONClass
			{
				{
					"dex",
					player2.bonus.dex.ToString()
				},
				{
					"str",
					player2.bonus.str.ToString()
				},
				{
					"mgc",
					player2.bonus.mgc.ToString()
				},
				{
					"wpr",
					player2.bonus.wpr.ToString()
				}
			});
			JSONArray jsonarray3 = new JSONArray();
			for (Int32 k = 0; k < 48; k++)
			{
				jsonarray3.Add(player2.pa[k].ToString());
			}
			jsonclass2.Add("pa", jsonarray3);
			JSONArray jsonarray4 = new JSONArray();
			for (Int32 l = 0; l < 2; l++)
			{
				jsonarray4.Add(player2.sa[l].ToString());
			}
			jsonclass2.Add("sa", jsonarray4);
			jsonarray.Add(jsonclass2);
		}
		jsonclass.Add("players", jsonarray);
		JSONArray jsonarray5 = new JSONArray();
		for (Int32 m = 0; m < 4; m++)
		{
			if (party.member[m] != null)
			{
				jsonarray5.Add(party.member[m].info.slot_no.ToString());
			}
			else
			{
				jsonarray5.Add("255");
			}
		}
		jsonclass.Add("slot", jsonarray5);
		jsonclass.Add("escape_no", party.escape_no.ToString());
		jsonclass.Add("summon_flag", party.summon_flag.ToString());
		jsonclass.Add("gil", party.gil.ToString());
		jsonclass.Add("frog_no", data.Frogs.Number.ToString());
		jsonclass.Add("steal_no", data.steal_no.ToString());
		jsonclass.Add("dragon_no", data.dragon_no.ToString());
		FF9ITEM[] item = data.item;
		JSONArray jsonarray6 = new JSONArray();
		for (Int32 n = 0; n < 256; n++)
		{
			if (n < (Int32)item.Length)
			{
				FF9ITEM ff9ITEM = item[n];
				jsonarray6.Add(new JSONClass
				{
					{
						"id",
						ff9ITEM.id.ToString()
					},
					{
						"count",
						ff9ITEM.count.ToString()
					}
				});
			}
			else
			{
				jsonarray6.Add(new JSONClass
				{
					{
						"id",
						"255"
					},
					{
						"count",
						"255"
					}
				});
			}
		}
		jsonclass.Add("items", jsonarray6);
		Byte[] rare_item = data.rare_item;
		JSONArray jsonarray7 = new JSONArray();
		for (Int32 num = 0; num < 64; num++)
		{
			jsonarray7.Add(rare_item[num].ToString());
		}
		jsonclass.Add("rareItems", jsonarray7);
		dataNode.Add("40000_Common", jsonclass);
		JSONClass jsonclass3 = new JSONClass();
		PLAYER player3 = data.player[0];
		PARTY_DATA party2 = data.party;
		JSONArray jsonarray8 = new JSONArray();
		for (Int32 num2 = 0; num2 < 9; num2++)
		{
			JSONClass jsonclass4 = new JSONClass();
			jsonclass4.Add("name", player3.GetRealName().GetType().ToString());
			jsonclass4.Add("category", player3.category.GetType().ToString());
			jsonclass4.Add("level", player3.level.GetType().ToString());
			jsonclass4.Add("exp", player3.exp.GetType().ToString());
			jsonclass4.Add("cur", new JSONClass
			{
				{
					"hp",
					player3.cur.hp.GetType().ToString()
				},
				{
					"mp",
					player3.cur.mp.GetType().ToString()
				},
				{
					"at",
					player3.cur.at.GetType().ToString()
				},
				{
					"at_coef",
					player3.cur.at_coef.GetType().ToString()
				},
				{
					"capa",
					player3.cur.capa.GetType().ToString()
				}
			});
			jsonclass4.Add("max", new JSONClass
			{
				{
					"hp",
					player3.max.hp.GetType().ToString()
				},
				{
					"mp",
					player3.max.mp.GetType().ToString()
				},
				{
					"at",
					player3.max.at.GetType().ToString()
				},
				{
					"at_coef",
					player3.max.at_coef.GetType().ToString()
				},
				{
					"capa",
					player3.max.capa.GetType().ToString()
				}
			});
			jsonclass4.Add("trance", player3.trance.GetType().ToString());
			jsonclass4.Add("web_bone", player3.wep_bone.GetType().ToString());
			jsonclass4.Add("elem", new JSONClass
			{
				{
					"dex",
					player3.elem.dex.GetType().ToString()
				},
				{
					"str",
					player3.elem.str.GetType().ToString()
				},
				{
					"mgc",
					player3.elem.mgc.GetType().ToString()
				},
				{
					"wpr",
					player3.elem.wpr.GetType().ToString()
				}
			});
			jsonclass4.Add("defence", new JSONClass
			{
				{
					"p_def",
					player3.defence.PhisicalDefence.GetType().ToString()
				},
				{
					"p_ev",
					player3.defence.PhisicalEvade.GetType().ToString()
				},
				{
					"m_def",
					player3.defence.MagicalDefence.GetType().ToString()
				},
				{
					"m_ev",
					player3.defence.MagicalEvade.GetType().ToString()
				}
			});
			jsonclass4.Add("basis", new JSONClass
			{
				{
					"max_hp",
					player3.basis.max_hp.GetType().ToString()
				},
				{
					"max_mp",
					player3.basis.max_mp.GetType().ToString()
				},
				{
					"dex",
					player3.basis.dex.GetType().ToString()
				},
				{
					"str",
					player3.basis.str.GetType().ToString()
				},
				{
					"mgc",
					player3.basis.mgc.GetType().ToString()
				},
				{
					"wpr",
					player3.basis.wpr.GetType().ToString()
				}
			});
			jsonclass4.Add("info", new JSONClass
			{
				{
					"slot_no",
					player3.info.slot_no.GetType().ToString()
				},
				{
					"serial_no",
					player3.info.serial_no.GetType().ToString()
				},
				{
					"row",
					player3.info.row.GetType().ToString()
				},
				{
					"win_pose",
					player3.info.win_pose.GetType().ToString()
				},
				{
					"party",
					player3.info.party.GetType().ToString()
				},
				{
					"menu_type",
					player3.info.menu_type.GetType().ToString()
				}
			});
			jsonclass4.Add("status", player3.status.GetType().ToString());
			JSONArray jsonarray9 = new JSONArray();
			for (Int32 num3 = 0; num3 < 5; num3++)
			{
				jsonarray9.Add(player3.equip[num3].GetType().ToString());
			}
			jsonclass4.Add("equip", jsonarray9);
			jsonclass4.Add("bonus", new JSONClass
			{
				{
					"dex",
					player3.bonus.dex.GetType().ToString()
				},
				{
					"str",
					player3.bonus.str.GetType().ToString()
				},
				{
					"mgc",
					player3.bonus.mgc.GetType().ToString()
				},
				{
					"wpr",
					player3.bonus.wpr.GetType().ToString()
				}
			});
			JSONArray jsonarray10 = new JSONArray();
			for (Int32 num4 = 0; num4 < 48; num4++)
			{
				jsonarray10.Add(player3.pa[num4].GetType().ToString());
			}
			jsonclass4.Add("pa", jsonarray10);
			JSONArray jsonarray11 = new JSONArray();
			for (Int32 num5 = 0; num5 < 2; num5++)
			{
				jsonarray11.Add(player3.sa[num5].GetType().ToString());
			}
			jsonclass4.Add("sa", jsonarray11);
			jsonarray8.Add(jsonclass4);
		}
		jsonclass3.Add("players", jsonarray8);
		JSONArray jsonarray12 = new JSONArray();
		for (Int32 num6 = 0; num6 < 4; num6++)
		{
			Byte b = 0;
			jsonarray12.Add(b.GetType().ToString());
		}
		jsonclass3.Add("slot", jsonarray12);
		jsonclass3.Add("escape_no", party2.escape_no.GetType().ToString());
		jsonclass3.Add("summon_flag", party2.summon_flag.GetType().ToString());
		jsonclass3.Add("gil", party2.gil.GetType().ToString());
		jsonclass3.Add("frog_no", data.Frogs.Number.GetType().ToString());
		jsonclass3.Add("steal_no", data.steal_no.GetType().ToString());
		jsonclass3.Add("dragon_no", data.dragon_no.GetType().ToString());
		FF9ITEM ff9ITEM2 = new FF9ITEM(0, 0);
		JSONArray jsonarray13 = new JSONArray();
		for (Int32 num7 = 0; num7 < 256; num7++)
		{
			jsonarray13.Add(new JSONClass
			{
				{
					"id",
					ff9ITEM2.id.GetType().ToString()
				},
				{
					"count",
					ff9ITEM2.count.GetType().ToString()
				}
			});
		}
		jsonclass3.Add("items", jsonarray13);
		Byte b2 = 0;
		JSONArray jsonarray14 = new JSONArray();
		for (Int32 num8 = 0; num8 < 64; num8++)
		{
			jsonarray14.Add(b2.GetType().ToString());
		}
		jsonclass3.Add("rareItems", jsonarray14);
		schemaNode.Add("40000_Common", jsonclass3);
	}

	private void ParseCommonJsonToData(JSONNode jsonData)
	{
		FF9StateGlobal ff = FF9StateSystem.Common.FF9;
		if (jsonData["players"] != null)
		{
			Int32 count = jsonData["players"].Count;
			for (Int32 i = 0; i < count; i++)
			{
				JSONClass asObject = jsonData["players"][i].AsObject;
				PLAYER player = ff.player[i];
				if (asObject["name"] != null)
				{
					player.SetRealName(asObject["name"]);
				}
				if (asObject["category"] != null)
				{
					player.category = (Byte)asObject["category"].AsInt;
				}
				if (asObject["level"] != null)
				{
					player.level = (Byte)asObject["level"].AsInt;
				}
				if (asObject["exp"] != null)
				{
					player.exp = (UInt32)asObject["exp"].AsInt;
				}
				JSONClass asObject2 = asObject["cur"].AsObject;
				if (asObject2["hp"] != null)
				{
					player.cur.hp = (UInt16)asObject2["hp"].AsInt;
				}
				if (asObject2["mp"] != null)
				{
					player.cur.mp = (Int16)asObject2["mp"].AsInt;
				}
				if (asObject2["at"] != null)
				{
					player.cur.at = (Int16)asObject2["at"].AsInt;
				}
				if (asObject2["at_coef"] != null)
				{
					player.cur.at_coef = (SByte)asObject2["at_coef"].AsInt;
				}
				if (asObject2["capa"] != null)
				{
					player.cur.capa = (Byte)asObject2["capa"].AsInt;
				}
				JSONClass asObject3 = asObject["max"].AsObject;
				if (asObject3["hp"] != null)
				{
					player.max.hp = (UInt16)asObject3["hp"].AsInt;
				}
				if (asObject3["mp"] != null)
				{
					player.max.mp = (Int16)asObject3["mp"].AsInt;
				}
				if (asObject3["at"] != null)
				{
					player.max.at = (Int16)asObject3["at"].AsInt;
				}
				if (asObject3["at_coef"] != null)
				{
					player.max.at_coef = (SByte)asObject3["at_coef"].AsInt;
				}
				if (asObject3["capa"] != null)
				{
					player.max.capa = (Byte)asObject3["capa"].AsInt;
				}
				if (asObject["trance"] != null)
				{
					player.trance = (Byte)asObject["trance"].AsInt;
				}
				if (asObject["wep_bone"] != null)
				{
					player.wep_bone = (Byte)asObject["wep_bone"].AsInt;
				}
				JSONClass asObject4 = asObject["elem"].AsObject;
				if (asObject4["dex"] != null)
				{
					player.elem.dex = (Byte)asObject4["dex"].AsInt;
				}
				if (asObject4["str"] != null)
				{
					player.elem.str = (Byte)asObject4["str"].AsInt;
				}
				if (asObject4["mgc"] != null)
				{
					player.elem.mgc = (Byte)asObject4["mgc"].AsInt;
				}
				if (asObject4["wpr"] != null)
				{
					player.elem.wpr = (Byte)asObject4["wpr"].AsInt;
				}
				JSONClass asObject5 = asObject["defence"].AsObject;
				if (asObject5["p_def"] != null)
				{
					player.defence.PhisicalDefence = (Byte)asObject5["p_def"].AsInt;
				}
				if (asObject5["p_ev"] != null)
				{
					player.defence.PhisicalEvade = (Byte)asObject5["p_ev"].AsInt;
				}
				if (asObject5["m_def"] != null)
				{
					player.defence.MagicalDefence = (Byte)asObject5["m_def"].AsInt;
				}
				if (asObject5["m_ev"] != null)
				{
					player.defence.MagicalEvade = (Byte)asObject5["m_ev"].AsInt;
				}
				JSONClass asObject6 = asObject["basis"].AsObject;
				if (asObject6["max_hp"] != null)
				{
					player.basis.max_hp = (Int16)asObject6["max_hp"].AsInt;
				}
				if (asObject6["max_mp"] != null)
				{
					player.basis.max_mp = (Int16)asObject6["max_mp"].AsInt;
				}
				if (asObject6["dex"] != null)
				{
					player.basis.dex = (Byte)asObject6["dex"].AsInt;
				}
				if (asObject6["str"] != null)
				{
					player.basis.str = (Byte)asObject6["str"].AsInt;
				}
				if (asObject6["mgc"] != null)
				{
					player.basis.mgc = (Byte)asObject6["mgc"].AsInt;
				}
				if (asObject6["wpr"] != null)
				{
					player.basis.wpr = (Byte)asObject6["wpr"].AsInt;
				}
				JSONClass asObject7 = asObject["info"].AsObject;
				if (asObject7["slot_no"] != null)
				{
					player.info.slot_no = (Byte)asObject7["slot_no"].AsInt;
				}
				if (asObject7["serial_no"] != null)
				{
					player.info.serial_no = (Byte)asObject7["serial_no"].AsInt;
				}
				if (asObject7["row"] != null)
				{
					player.info.row = (Byte)asObject7["row"].AsInt;
				}
				if (asObject7["win_pose"] != null)
				{
					player.info.win_pose = (Byte)asObject7["win_pose"].AsInt;
				}
				if (asObject7["party"] != null)
				{
					player.info.party = (Byte)asObject7["party"].AsInt;
				}
				if (asObject7["menu_type"] != null)
				{
					player.info.menu_type = (Byte)asObject7["menu_type"].AsInt;
				}
				if (asObject["status"] != null)
				{
					player.status = (Byte)asObject["status"].AsInt;
				}
				if (asObject["equip"] != null)
				{
					for (Int32 j = 0; j < asObject["equip"].Count; j++)
					{
						if (asObject["equip"][j] != null)
							player.equip[j] = (Byte)asObject["equip"][j].AsInt;
					}
				}
				JSONClass asObject8 = asObject["bonus"].AsObject;
				if (asObject8["dex"] != null)
				{
					player.bonus.dex = (UInt16)asObject8["dex"].AsInt;
				}
				if (asObject8["str"] != null)
				{
					player.bonus.str = (UInt16)asObject8["str"].AsInt;
				}
				if (asObject8["mgc"] != null)
				{
					player.bonus.mgc = (UInt16)asObject8["mgc"].AsInt;
				}
				if (asObject8["wpr"] != null)
				{
					player.bonus.wpr = (UInt16)asObject8["wpr"].AsInt;
				}
				if (asObject["pa"] != null)
				{
					for (Int32 k = 0; k < asObject["pa"].Count; k++)
					{
						if (asObject["pa"][k] != null)
						{
							player.pa[k] = (Byte)asObject["pa"][k].AsInt;
						}
					}
				}
				if (asObject["sa"] != null)
				{
					for (Int32 l = 0; l < asObject["sa"].Count; l++)
					{
						if (asObject["sa"][l] != null)
						{
							player.sa[l] = asObject["sa"][l].AsUInt;
						}
					}
				}
				player.ValidateSupportAbility();
				player.ValidateBasisStatus();
			}
		}
		if (jsonData["slot"] != null)
		{
			for (Int32 m = 0; m < jsonData["slot"].Count; m++)
			{
				if (jsonData["slot"][m].AsInt != 255 && jsonData["slot"][m].AsInt != -1)
				{
					Int32 num = (Int32)((Byte)jsonData["slot"][m].AsInt);
					PLAYER player2 = FF9StateSystem.Common.FF9.player[num];
					ff.party.member[m] = player2;
				}
				else
				{
					ff.party.member[m] = (PLAYER)null;
				}
			}
		}
		if (jsonData["escape_no"] != null)
		{
			ff.party.escape_no = (UInt16)jsonData["escape_no"].AsInt;
		}
		if (jsonData["summon_flag"] != null)
		{
			ff.party.summon_flag = (UInt16)jsonData["summon_flag"].AsInt;
		}
		if (jsonData["gil"] != null)
		{
			ff.party.gil = (UInt32)jsonData["gil"].AsInt;
		}
		if (jsonData["frog_no"] != null)
		{
		    ff.Frogs.Initialize((Int16)jsonData["frog_no"].AsInt);
		}
		if (jsonData["steal_no"] != null)
		{
			ff.steal_no = (Int16)jsonData["steal_no"].AsInt;
		}
		if (jsonData["dragon_no"] != null)
		{
			ff.dragon_no = (Int16)jsonData["dragon_no"].AsInt;
		}
		if (jsonData["items"] != null)
		{
			for (Int32 n = 0; n < jsonData["items"].Count; n++)
			{
				if (jsonData["items"][n] != null || jsonData["items"][n].AsInt != 255)
				{
					JSONClass asObject9 = jsonData["items"][n].AsObject;
					ff.item[n].id = (Byte)asObject9["id"].AsInt;
					ff.item[n].count = (Byte)asObject9["count"].AsInt;
				}
			}
		}
		if (jsonData["rareItems"] != null)
		{
			for (Int32 num2 = 0; num2 < 64; num2++)
			{
				ff.rare_item[num2] = (Byte)jsonData["rareItems"][num2].AsInt;
			}
		}
	}

	private void ParseSettingDataToJson(SettingsState data, JSONClass dataNode, JSONClass schemaNode)
	{
		JSONClass jsonclass = new JSONClass();
		JSONClass jsonclass2 = new JSONClass();
		jsonclass2.Add("sound", data.cfg.sound.ToString());
		jsonclass2.Add("sound_effect", data.cfg.sound_effect.ToString());
		jsonclass2.Add("control", data.cfg.control.ToString());
		jsonclass2.Add("cursor", data.cfg.cursor.ToString());
		jsonclass2.Add("atb", data.cfg.atb.ToString());
		jsonclass2.Add("camera", data.cfg.camera.ToString());
		jsonclass2.Add("move", data.cfg.move.ToString());
		jsonclass2.Add("vibe", data.cfg.vibe.ToString());
		jsonclass2.Add("btl_speed", data.cfg.btl_speed.ToString());
		jsonclass2.Add("fld_msg", data.cfg.fld_msg.ToString());
		jsonclass2.Add("here_icon", data.cfg.here_icon.ToString());
		jsonclass2.Add("win_type", data.cfg.win_type.ToString());
		jsonclass2.Add("target_win", data.cfg.target_win.ToString());
		jsonclass2.Add("control_data", data.cfg.control_data.ToString());
		JSONArray jsonarray = new JSONArray();
		for (Int32 i = 0; i < (Int32)data.cfg.control_data_keyboard.Length; i++)
		{
			JSONNode jsonnode = jsonarray;
			Int32 num = (Int32)data.cfg.control_data_keyboard[i];
			jsonnode.Add(num.ToString());
		}
		jsonclass2.Add("control_data_keyboard", jsonarray);
		JSONArray jsonarray2 = new JSONArray();
		for (Int32 j = 0; j < (Int32)data.cfg.control_data_joystick.Length; j++)
		{
			jsonarray2.Add(data.cfg.control_data_joystick[j]);
		}
		jsonclass2.Add("control_data_joystick", jsonarray2);
		jsonclass2.Add("camera", data.cfg.camera.ToString());
		jsonclass2.Add("skip_btl_camera", data.cfg.skip_btl_camera.ToString());
		jsonclass.Add("cfg", jsonclass2);
		jsonclass.Add("time", (-1f).ToString(CultureInfo.InvariantCulture));
		dataNode.Add("50000_Setting", jsonclass);
		JSONClass jsonclass3 = new JSONClass();
		JSONClass jsonclass4 = new JSONClass();
		jsonclass4.Add("sound", data.cfg.sound.GetType().ToString());
		jsonclass4.Add("sound_effect", data.cfg.sound_effect.GetType().ToString());
		jsonclass4.Add("control", data.cfg.control.GetType().ToString());
		jsonclass4.Add("cursor", data.cfg.cursor.GetType().ToString());
		jsonclass4.Add("atb", data.cfg.atb.GetType().ToString());
		jsonclass4.Add("camera", data.cfg.camera.GetType().ToString());
		jsonclass4.Add("move", data.cfg.move.GetType().ToString());
		jsonclass4.Add("vibe", data.cfg.vibe.GetType().ToString());
		jsonclass4.Add("btl_speed", data.cfg.btl_speed.GetType().ToString());
		jsonclass4.Add("fld_msg", data.cfg.fld_msg.GetType().ToString());
		jsonclass4.Add("here_icon", data.cfg.here_icon.GetType().ToString());
		jsonclass4.Add("win_type", data.cfg.win_type.GetType().ToString());
		jsonclass4.Add("target_win", data.cfg.target_win.GetType().ToString());
		jsonclass4.Add("control_data", data.cfg.control_data.GetType().ToString());
		JSONArray jsonarray3 = new JSONArray();
		for (Int32 k = 0; k < (Int32)data.cfg.control_data_keyboard.Length; k++)
		{
			jsonarray3.Add(((Int32)data.cfg.control_data_keyboard[k]).GetType().ToString());
		}
		jsonclass4.Add("control_data_keyboard", jsonarray3);
		JSONArray jsonarray4 = new JSONArray();
		for (Int32 l = 0; l < (Int32)data.cfg.control_data_joystick.Length; l++)
		{
			jsonarray4.Add(data.cfg.control_data_joystick[l].GetType().ToString());
		}
		jsonclass4.Add("control_data_joystick", jsonarray4);
		jsonclass4.Add("camera", data.cfg.camera.GetType().ToString());
		jsonclass4.Add("skip_btl_camera", data.cfg.skip_btl_camera.GetType().ToString());
		jsonclass3.Add("cfg", jsonclass4);
		Single num2 = -1f;
		jsonclass3.Add("time", num2.GetType().ToString());
		schemaNode.Add("50000_Setting", jsonclass3);
	}

	private void ParseSettingJsonToData(JSONNode jsonData)
	{
		SettingsState settings = FF9StateSystem.Settings;
		if (jsonData["cfg"] != null)
		{
			JSONClass asObject = jsonData["cfg"].AsObject;
			FF9CFG cfg = settings.cfg;
			if (asObject["sound"] != null)
			{
				cfg.sound = (UInt64)((Int64)asObject["sound"].AsInt);
			}
			if (asObject["sound_effect"] != null)
			{
				cfg.sound_effect = (UInt64)((Int64)asObject["sound_effect"].AsInt);
			}
			if (asObject["control"] != null)
			{
				cfg.control = (UInt64)((Int64)asObject["control"].AsInt);
			}
			if (asObject["cursor"] != null)
			{
				cfg.cursor = (UInt64)((Int64)asObject["cursor"].AsInt);
			}
			if (asObject["atb"] != null)
			{
				cfg.atb = (UInt64)((Int64)asObject["atb"].AsInt);
			}
			if (asObject["camera"] != null)
			{
				cfg.camera = (UInt64)((Int64)asObject["camera"].AsInt);
			}
			if (asObject["move"] != null)
			{
				cfg.move = (UInt64)((Int64)asObject["move"].AsInt);
			}
			if (asObject["vibe"] != null)
			{
				cfg.vibe = (UInt64)((Int64)asObject["vibe"].AsInt);
			}
			if (asObject["btl_speed"] != null)
			{
				cfg.btl_speed = (UInt64)((Int64)asObject["btl_speed"].AsInt);
			}
			if (asObject["fld_msg"] != null)
			{
				cfg.fld_msg = (UInt64)((Int64)asObject["fld_msg"].AsInt);
			}
			if (asObject["here_icon"] != null)
			{
				cfg.here_icon = (UInt64)((Int64)asObject["here_icon"].AsInt);
			}
			if (asObject["win_type"] != null)
			{
				cfg.win_type = (UInt64)((Int64)asObject["win_type"].AsInt);
			}
			if (asObject["target_win"] != null)
			{
				cfg.target_win = (UInt64)((Int64)asObject["target_win"].AsInt);
			}
			if (asObject["control_data"] != null)
			{
				cfg.control_data = (UInt64)((Int64)asObject["control_data"].AsInt);
			}
			if (asObject["control_data_keyboard"] != null)
			{
				JSONArray asArray = asObject["control_data_keyboard"].AsArray;
				if (cfg.control_data_keyboard == null)
				{
					cfg.control_data_keyboard = new KeyCode[asArray.Count];
				}
				for (Int32 i = 0; i < asArray.Count; i++)
				{
					cfg.control_data_keyboard[i] = (KeyCode)asArray[i].AsInt;
				}
			}
			if (asObject["control_data_joystick"] != null)
			{
				JSONArray asArray2 = asObject["control_data_joystick"].AsArray;
				if (cfg.control_data_joystick == null)
				{
					cfg.control_data_joystick = new String[asArray2.Count];
				}
				for (Int32 j = 0; j < asArray2.Count; j++)
				{
					cfg.control_data_joystick[j] = asArray2[j];
				}
			}
			if (asObject["camera"] != null)
			{
				settings.cfg.camera = (UInt64)((Int64)asObject["camera"].AsInt);
			}
			if (asObject["skip_btl_camera"] != null)
			{
				settings.cfg.skip_btl_camera = (UInt64)((Int64)asObject["skip_btl_camera"].AsInt);
			}
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
		{
			FF9StateSystem.Sound.auto_save_bgm_id = jsonData["auto_save_bgm_id"].AsInt;
		}
	}

	private void ParseWorldDataToJson(WorldState data_unused, JSONClass dataNode, JSONClass schemaNode)
	{
		ff9.FF9SAVE_WORLD ff9SAVE_WORLD;
		ff9.SaveWorld(out ff9SAVE_WORLD);
		dataNode.Add("70000_World", new JSONClass
		{
			{
				"data.cameraState.rotationMax",
				ff9SAVE_WORLD.cameraState.rotationMax.ToString()
			},
			{
				"data.cameraState.upperCounter",
				ff9SAVE_WORLD.cameraState.upperCounter.ToString()
			},
			{
				"data.cameraState.upperCounterSpeed",
				ff9SAVE_WORLD.cameraState.upperCounterSpeed.ToString()
			},
			{
				"data.cameraState.upperCounterForce",
				ff9SAVE_WORLD.cameraState.upperCounterForce.ToString()
			},
			{
				"data.cameraState.rotation",
				ff9SAVE_WORLD.cameraState.rotation.ToString()
			},
			{
				"data.cameraState.rotationRev",
				ff9SAVE_WORLD.cameraState.rotationRev.ToString()
			},
			{
				"data.hintmap",
				ff9SAVE_WORLD.hintmap.ToString()
			}
		});
		schemaNode.Add("70000_World", new JSONClass
		{
			{
				"data.cameraState.rotationMax",
				ff9SAVE_WORLD.cameraState.rotationMax.GetType().ToString()
			},
			{
				"data.cameraState.upperCounter",
				ff9SAVE_WORLD.cameraState.upperCounter.GetType().ToString()
			},
			{
				"data.cameraState.upperCounterSpeed",
				ff9SAVE_WORLD.cameraState.upperCounterSpeed.GetType().ToString()
			},
			{
				"data.cameraState.upperCounterForce",
				ff9SAVE_WORLD.cameraState.upperCounterForce.GetType().ToString()
			},
			{
				"data.cameraState.rotation",
				ff9SAVE_WORLD.cameraState.rotation.GetType().ToString()
			},
			{
				"data.cameraState.rotationRev",
				ff9SAVE_WORLD.cameraState.rotationRev.GetType().ToString()
			},
			{
				"data.hintmap",
				ff9SAVE_WORLD.hintmap.GetType().ToString()
			}
		});
	}

	private void ParseWorldJsonToData(JSONNode jsonData)
	{
		ff9.FF9SAVE_WORLD ff9SAVE_WORLD = new ff9.FF9SAVE_WORLD();
		if (jsonData["data.cameraState.rotationMax"] != null)
		{
			ff9SAVE_WORLD.cameraState.rotationMax = jsonData["data.cameraState.rotationMax"].AsFloat;
		}
		if (jsonData["data.cameraState.upperCounter"] != null)
		{
			ff9SAVE_WORLD.cameraState.upperCounter = (Int16)jsonData["data.cameraState.upperCounter"].AsInt;
		}
		if (jsonData["data.cameraState.upperCounterSpeed"] != null)
		{
			ff9SAVE_WORLD.cameraState.upperCounterSpeed = (Int32)((Int16)jsonData["data.cameraState.upperCounterSpeed"].AsInt);
		}
		if (jsonData["data.cameraState.upperCounterForce"] != null)
		{
			ff9SAVE_WORLD.cameraState.upperCounterForce = jsonData["data.cameraState.upperCounterForce"].AsBool;
		}
		if (jsonData["data.cameraState.rotation"] != null)
		{
			ff9SAVE_WORLD.cameraState.rotation = jsonData["data.cameraState.rotation"].AsFloat;
		}
		if (jsonData["data.cameraState.rotationRev"] != null)
		{
			ff9SAVE_WORLD.cameraState.rotationRev = jsonData["data.cameraState.rotationRev"].AsFloat;
		}
		if (jsonData["data.cameraState.rotationRev"] != null)
		{
			ff9SAVE_WORLD.hintmap = jsonData["data.cameraState.rotationRev"].AsUInt;
		}
		ff9.LoadWorld(ff9SAVE_WORLD);
	}

	private void ParseAchievementDataToJson(AchievementState data, JSONClass dataNode, JSONClass schemaNode)
	{
		JSONClass jsonclass = new JSONClass();
		JSONArray jsonarray = new JSONArray();
		for (Int32 i = 0; i < 100; i++)
		{
			jsonarray.Add(data.AteCheck[i].ToString());
		}
		jsonclass.Add("AteCheckArray", jsonarray);
		JSONArray jsonarray2 = new JSONArray();
		for (Int32 j = 0; j < 17; j++)
		{
			jsonarray2.Add(data.EvtReservedArray[j].ToString());
		}
		jsonclass.Add("EvtReservedArray", jsonarray2);
		jsonclass.Add("blkMag_no", data.blkMag_no.ToString());
		jsonclass.Add("whtMag_no", data.whtMag_no.ToString());
		jsonclass.Add("bluMag_no", data.bluMag_no.ToString());
		jsonclass.Add("summon_no", data.summon_no.ToString());
		jsonclass.Add("enemy_no", data.enemy_no.ToString());
		jsonclass.Add("backAtk_no", data.backAtk_no.ToString());
		jsonclass.Add("defence_no", data.defence_no.ToString());
		jsonclass.Add("trance_no", data.trance_no.ToString());
		JSONArray jsonarray3 = new JSONArray();
		foreach (Int32 num in data.abilities)
		{
			jsonarray3.Add(num.ToString());
		}
		for (Int32 k = data.abilities.Count; k < 221; k++)
		{
			jsonarray3.Add("-1");
		}
		jsonclass.Add("abilities", jsonarray3);
		JSONArray jsonarray4 = new JSONArray();
		foreach (Int32 num2 in data.passiveAbilities)
		{
			jsonarray4.Add(num2.ToString());
		}
		for (Int32 l = data.passiveAbilities.Count; l < 63; l++)
		{
			jsonarray4.Add("-1");
		}
		jsonclass.Add("passiveAbilities", jsonarray4);
		jsonclass.Add("synthesisCount", data.synthesisCount.ToString());
		jsonclass.Add("AuctionTime", data.AuctionTime.ToString());
		jsonclass.Add("StiltzkinBuy", data.StiltzkinBuy.ToString());
		JSONArray jsonarray5 = new JSONArray();
		foreach (Int32 num3 in data.QuadmistWinList)
		{
			jsonarray5.Add(num3.ToString());
		}
		for (Int32 m = data.QuadmistWinList.Count; m < 300; m++)
		{
			jsonarray5.Add("-1");
		}
		jsonclass.Add("QuadmistWinList", jsonarray5);
		dataNode.Add("80000_Achievement", jsonclass);
		JSONClass jsonclass2 = new JSONClass();
		JSONArray jsonarray6 = new JSONArray();
		for (Int32 n = 0; n < 100; n++)
		{
			jsonarray6.Add(data.AteCheck[n].GetType().ToString());
		}
		jsonclass2.Add("AteCheckArray", jsonarray6);
		JSONArray jsonarray7 = new JSONArray();
		for (Int32 num4 = 0; num4 < 17; num4++)
		{
			jsonarray7.Add(data.EvtReservedArray[num4].GetType().ToString());
		}
		jsonclass2.Add("EvtReservedArray", jsonarray7);
		jsonclass2.Add("blkMag_no", data.blkMag_no.GetType().ToString());
		jsonclass2.Add("whtMag_no", data.whtMag_no.GetType().ToString());
		jsonclass2.Add("bluMag_no", data.bluMag_no.GetType().ToString());
		jsonclass2.Add("summon_no", data.summon_no.GetType().ToString());
		jsonclass2.Add("enemy_no", data.enemy_no.GetType().ToString());
		jsonclass2.Add("backAtk_no", data.backAtk_no.GetType().ToString());
		jsonclass2.Add("defence_no", data.defence_no.GetType().ToString());
		jsonclass2.Add("trance_no", data.trance_no.GetType().ToString());
		JSONArray jsonarray8 = new JSONArray();
		for (Int32 num5 = 0; num5 < 221; num5++)
		{
			jsonarray8.Add(SharedDataBytesStorage.TypeInt32);
		}
		jsonclass2.Add("abilities", jsonarray8);
		JSONArray jsonarray9 = new JSONArray();
		for (Int32 num6 = 0; num6 < 63; num6++)
		{
			jsonarray9.Add(SharedDataBytesStorage.TypeInt32);
		}
		jsonclass2.Add("passiveAbilities", jsonarray9);
		jsonclass2.Add("synthesisCount", data.synthesisCount.GetType().ToString());
		jsonclass2.Add("AuctionTime", data.AuctionTime.GetType().ToString());
		jsonclass2.Add("StiltzkinBuy", data.StiltzkinBuy.GetType().ToString());
		JSONArray jsonarray10 = new JSONArray();
		for (Int32 num7 = 0; num7 < 300; num7++)
		{
			jsonarray10.Add((-1).GetType().ToString());
		}
		jsonclass2.Add("QuadmistWinList", jsonarray10);
		schemaNode.Add("80000_Achievement", jsonclass2);
	}

	private void ParseAchievementJsonToData(JSONNode jsonData)
	{
		AchievementState achievement = FF9StateSystem.Achievement;
		achievement.AteCheck = new Int32[100];
		if (jsonData["AteCheckArray"] != null)
		{
			JSONArray asArray = jsonData["AteCheckArray"].AsArray;
			for (Int32 i = 0; i < asArray.Count; i++)
			{
				achievement.AteCheck[i] = asArray[i].AsInt;
			}
		}
		achievement.EvtReservedArray = new Int32[17];
		if (jsonData["EvtReservedArray"] != null)
		{
			JSONArray asArray2 = jsonData["EvtReservedArray"].AsArray;
			for (Int32 j = 0; j < asArray2.Count; j++)
			{
				achievement.EvtReservedArray[j] = asArray2[j].AsInt;
			}
		}
		if (jsonData["blkMag_no"] != null)
		{
			achievement.blkMag_no = jsonData["blkMag_no"].AsInt;
		}
		if (jsonData["whtMag_no"] != null)
		{
			achievement.whtMag_no = jsonData["whtMag_no"].AsInt;
		}
		if (jsonData["bluMag_no"] != null)
		{
			achievement.bluMag_no = jsonData["bluMag_no"].AsInt;
		}
		if (jsonData["summon_no"] != null)
		{
			achievement.summon_no = jsonData["summon_no"].AsInt;
		}
		if (jsonData["enemy_no"] != null)
		{
			achievement.enemy_no = jsonData["enemy_no"].AsInt;
		}
		if (jsonData["backAtk_no"] != null)
		{
			achievement.backAtk_no = jsonData["backAtk_no"].AsInt;
		}
		if (jsonData["defence_no"] != null)
		{
			achievement.defence_no = jsonData["defence_no"].AsInt;
		}
		if (jsonData["trance_no"] != null)
		{
			achievement.trance_no = jsonData["trance_no"].AsInt;
		}
		achievement.abilities = new HashSet<Int32>();
		if (jsonData["abilities"] != null)
		{
			JSONArray asArray3 = jsonData["abilities"].AsArray;
			for (Int32 k = 0; k < asArray3.Count; k++)
			{
				Int32 asInt = asArray3[k].AsInt;
				if (asInt != -1)
				{
					achievement.abilities.Add(asInt);
				}
			}
		}
		achievement.passiveAbilities = new HashSet<Int32>();
		if (jsonData["passiveAbilities"] != null)
		{
			JSONArray asArray4 = jsonData["passiveAbilities"].AsArray;
			for (Int32 l = 0; l < asArray4.Count; l++)
			{
				Int32 asInt2 = asArray4[l].AsInt;
				if (asInt2 != -1)
				{
					achievement.passiveAbilities.Add(asInt2);
				}
			}
		}
		if (jsonData["synthesisCount"] != null)
		{
			achievement.synthesisCount = jsonData["synthesisCount"].AsInt;
		}
		if (jsonData["AuctionTime"] != null)
		{
			achievement.AuctionTime = jsonData["AuctionTime"].AsInt;
		}
		if (jsonData["StiltzkinBuy"] != null)
		{
			achievement.StiltzkinBuy = jsonData["StiltzkinBuy"].AsInt;
		}
		achievement.QuadmistWinList = new HashSet<Int32>();
		if (jsonData["QuadmistWinList"] != null)
		{
			JSONArray asArray5 = jsonData["QuadmistWinList"].AsArray;
			for (Int32 m = 0; m < asArray5.Count; m++)
			{
				Int32 asInt3 = asArray5[m].AsInt;
				if (asInt3 != -1)
				{
					achievement.QuadmistWinList.Add(asInt3);
				}
			}
		}
	}
}
