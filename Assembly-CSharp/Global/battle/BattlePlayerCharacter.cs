using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria;
using Object = System.Object;

public class BattlePlayerCharacter : MonoBehaviour
{
	public static String GetCasterAnimationName(String nameId, BattlePlayerCharacter.PlayerMotionIndex motID)
	{
		return "ANH_MAIN_" + nameId + "_" + BattlePlayerCharacter.PlayerMotionNum[(Int32)motID];
	}

	public static String GetCasterAnimationNameID(String name)
	{
		String key = name.Substring(9, 6);
		String text = (!BattlePlayerCharacter.PlayerModelToAnimationID.TryGetValue(key, out text)) ? "no key" : text;
		return (!(text != "no key")) ? name : text;
	}

	public static Int32 PlayMotion(GameObject goPlayerCharacter, Int32 playerMotionIndex)
	{
		Animation component = goPlayerCharacter.GetComponent<Animation>();
		String casterAnimationNameID = BattlePlayerCharacter.GetCasterAnimationNameID(component.name);
		String casterAnimationName = BattlePlayerCharacter.GetCasterAnimationName(casterAnimationNameID, (BattlePlayerCharacter.PlayerMotionIndex)playerMotionIndex);
		Int32 result = 0;
		if (component[casterAnimationName] != (TrackedReference)null)
		{
			result = Mathf.CeilToInt(component[casterAnimationName].length * component[casterAnimationName].clip.frameRate) * (int)(component[casterAnimationName].clip.frameRate / (float)Configuration.Graphics.BattleFPS);
			component[casterAnimationName].speed = component[casterAnimationName].clip.frameRate / (float)Configuration.Graphics.BattleFPS;
			component.Play(casterAnimationName);
		}
		return result;
	}

	public static void InitMaxFrame(BTL_DATA goPlayerCharacter, Byte[] maxFrames)
	{
		Animation component = goPlayerCharacter.gameObject.GetComponent<Animation>();
		String text = String.Empty;
		for (Int32 i = 0; i < (Int32)goPlayerCharacter.mot.Length; i++)
		{
			String text2 = goPlayerCharacter.mot[i];
			Int32 num = 1;
			if (component[text2] != (TrackedReference)null)
			{
				component[text2].speed = component[text2].clip.frameRate / (float)Configuration.Graphics.BattleFPS;
				num = Mathf.CeilToInt(component[text2].length * ((float)Configuration.Graphics.BattleFPS / component[text2].clip.frameRate) * component[text2].clip.frameRate);
			}
			maxFrames[i] = (Byte)num;
			String text3 = text;
			text = String.Concat(new Object[]
			{
				text3,
				"animName:",
				text2,
				":",
				num,
				"\n"
			});
		}
		global::Debug.Log(text);
	}

	public static void InitAnimation(BTL_DATA btl)
	{
		for (Int32 i = 0; i < 34; i++)
		{
			Int32 num = i;
			String name = btl.mot[num];
			if (!(btl.gameObject.GetComponent<Animation>()[name] == (TrackedReference)null))
			{
				AnimationState animationState = btl.gameObject.GetComponent<Animation>()[name];
				if (num == 0 || num == 1 || num == 9 || num == 13 || num == 19 || num == 27 || num == 17 || animationState.name.Substring(animationState.name.Length - 3) == "000")
				{
					animationState.wrapMode = WrapMode.Loop;
				}
				else
				{
					animationState.wrapMode = WrapMode.Once;
				}
				animationState.speed = animationState.clip.frameRate / (float)Configuration.Graphics.BattleFPS;
			}
		}
	}

	public static void CreatePlayer(BTL_DATA btl, BattlePlayerCharacter.PlayerSerialNumber playerSerialNumber)
	{
		String text = BattlePlayerCharacter.PlayerModelFileName[(Int32)playerSerialNumber];
		String path = text;
		GameObject gameObject = ModelFactory.CreateModel(path, true);
		BattlePlayerCharacter.CreateTranceModel(btl, (Int32)playerSerialNumber);
		BattlePlayerCharacter.CheckToHideBattleModel(gameObject, (Int32)playerSerialNumber);
		btl.gameObject = gameObject;
		btl.originalGo = gameObject;
	}

	private static void CreateTranceModel(BTL_DATA btl, Int32 serial)
	{
		if (serial + 19 >= (Int32)btl_init.model_id.Length)
		{
			return;
		}
		String path = btl_init.model_id[serial + 19];
		btl.tranceGo = ModelFactory.CreateModel(path, true);
		BattlePlayerCharacter.CheckToHideBattleModel(btl.tranceGo, serial);
		btl.tranceGo.transform.localPosition = new Vector3(btl.tranceGo.transform.localPosition.x, -10000f, btl.tranceGo.transform.localPosition.z);
		btl.tranceGo.SetActive(false);
	}

	private static void CheckToHideBattleModel(GameObject characterGo, Int32 serial)
	{
		if (serial == 1)
		{
			Transform childByName = characterGo.transform.GetChildByName("battle_model");
			if (childByName != (UnityEngine.Object)null)
			{
				Renderer[] componentsInChildren = childByName.GetComponentsInChildren<Renderer>();
				Renderer[] array = componentsInChildren;
				for (Int32 i = 0; i < (Int32)array.Length; i++)
				{
					Renderer renderer = array[i];
					renderer.enabled = false;
				}
			}
		}
	}

	public static readonly String[] PlayerMotionNum = new String[]
	{
		"000",
		"022",
		"300",
		"310",
		"033",
		"020",
		"032",
		"002",
		"023",
		"011",
		"001",
		"021",
		"400",
		"401",
		"402",
		"410",
		"420",
		"430",
		"500",
		"501",
		"100",
		"101",
		"102",
		"103",
		"104",
		"105",
		"200",
		"201",
		"202",
		"040",
		"050",
		"210",
		"010",
		"220"
	};

	public static readonly Int32[] PlayerMotionMapFF9PlayerCharacter = new Int32[]
	{
		0,
		0,
		1,
		2,
		2,
		2,
		2,
		3,
		3,
		9,
		6,
		6,
		4,
		7,
		8,
		9,
		10,
		10,
		11
	};

	public static String[] PlayerModelFileName = new String[]
	{
		"GEO_MAIN_B0_000", // ZIDANE_DAGGER
		"GEO_MAIN_B0_001", // ZIDANE_SWORD,
		"GEO_MAIN_B0_006", // VIVI,
		"GEO_MAIN_B0_002", // GARNET_LH_ROD,
		"GEO_MAIN_B0_003", // GARNET_LH_KNIFE,
		"GEO_MAIN_B0_004", // GARNET_SH_ROD,
		"GEO_MAIN_B0_005", // GARNET_SH_KNIFE,
		"GEO_MAIN_B0_007", // STEINER_OUTDOOR,
		"GEO_MAIN_B0_018", // STEINER_INDOOR,
		"GEO_MAIN_B0_008", // KUINA,
		"GEO_MAIN_B0_009", // EIKO_FLUTE,
		"GEO_MAIN_B0_010", // EIKO_KNIFE,
		"GEO_MAIN_B0_011", // FREIJA,
		"GEO_MAIN_B0_012", // SALAMANDER,
		"GEO_MAIN_B0_013", // CINNA,
		"GEO_MAIN_B0_014", // MARCUS,
		"GEO_MAIN_B0_015", // BLANK,
		"GEO_MAIN_B0_016", // BLANK_ARMOR,
		"GEO_MAIN_B0_017"  // BEATRIX,
	};

	public static Byte[] PlayerWeaponToBoneName = new Byte[]
	{
		13,
		13,
		16,
		15,
		15,
		15,
		15,
		16,
		16,
		14,
		15,
		15,
		6,
		16,
		25,
		6,
		14,
		14,
		16
	};

	public static readonly Int32[] PlayerDefaultWeaponID = new Int32[]
	{
		1,
		7,
		70,
		57,
		51,
		57,
		51,
		16,
		16,
		79,
		64,
		51,
		31,
		41,
		0,
		16,
		16,
		16,
		26
	};

	public static readonly Dictionary<String, String> PlayerModelToAnimationID = new Dictionary<String, String>
	{
		{
			"B0_000",
			"B0_000"
		},
		{
			"B0_001",
			"B0_001"
		},
		{
			"B0_006",
			"B0_006"
		},
		{
			"B0_002",
			"B0_002"
		},
		{
			"B0_003",
			"B0_003"
		},
		{
			"B0_004",
			"B0_004"
		},
		{
			"B0_005",
			"B0_005"
		},
		{
			"B0_007",
			"B0_007"
		},
		{
			"B0_018",
			"B0_007"
		},
		{
			"B0_008",
			"B0_008"
		},
		{
			"B0_009",
			"B0_009"
		},
		{
			"B0_010",
			"B0_010"
		},
		{
			"B0_011",
			"B0_011"
		},
		{
			"B0_012",
			"B0_012"
		},
		{
			"B0_013",
			"B0_013"
		},
		{
			"B0_014",
			"B0_014"
		},
		{
			"B0_015",
			"B0_015"
		},
		{
			"B0_016",
			"B0_016"
		},
		{
			"B0_017",
			"B0_017"
		},
		{
			"F0_ZDN",
			"B0_000"
		},
		{
			"F0_GRN",
			"B0_002"
		}
	};

	public static readonly Int32[,] PlayerEquipWeapon = new Int32[,]
	{
		{
			0,
			1,
			2
		},
		{
			0,
			1,
			2
		},
		{
			0,
			1,
			2
		},
		{
			0,
			1,
			2
		},
		{
			0,
			1,
			2
		},
		{
			0,
			1,
			2
		},
		{
			0,
			1,
			2
		},
		{
			0,
			1,
			2
		},
		{
			0,
			1,
			2
		},
		{
			0,
			1,
			2
		},
		{
			0,
			1,
			2
		},
		{
			0,
			1,
			2
		}
	};

	public enum FF9PlayerCharacter
	{
		FF9PLAY_CHAR_ZIDANE,
		FF9PLAY_CHAR_VIVI,
		FF9PLAY_CHAR_GARNET,
		FF9PLAY_CHAR_STEINER,
		FF9PLAY_CHAR_FREIJA,
		FF9PLAY_CHAR_KUINA,
		FF9PLAY_CHAR_EIKO,
		FF9PLAY_CHAR_SALAMANDER,
		FF9PLAY_CHAR_CINNA,
		FF9PLAY_CHAR_MARCUS,
		FF9PLAY_CHAR_BLANK,
		FF9PLAY_CHAR_BEATRIX,
		FF9PLAY_CHAR_MAX
	}

	// Enemies have 6 default battle animations that more or less relate to the first 6 PlayerMotionIndex:
	// Idle, Idle Alternate, Hit, Hit Alternate, Death, Death Alternate
	public enum PlayerMotionIndex
	{
		MP_IDLE_NORMAL,
		MP_IDLE_DYING,
		MP_DAMAGE1,
		MP_DAMAGE2,
		MP_DISABLE,
		MP_GET_UP_DYING,
		MP_GET_UP_DISABLE,
		MP_DOWN_DYING,
		MP_DOWN_DISABLE,
		MP_IDLE_CMD,
		MP_NORMAL_TO_CMD,
		MP_DYING_TO_CMD,
		MP_IDLE_TO_DEF,
		MP_DEFENCE,
		MP_DEF_TO_IDLE,
		MP_COVER,
		MP_AVOID,
		MP_ESCAPE,
		MP_WIN,
		MP_WIN_LOOP,
		MP_SET,
		MP_RUN,
		MP_RUN_TO_ATTACK,
		MP_ATTACK,
		MP_BACK,
		MP_ATK_TO_NORMAL,
		MP_IDLE_TO_CHANT,
		MP_CHANT,
		MP_MAGIC,
		MP_STEP_FORWARD,
		MP_STEP_BACK,
		MP_ITEM1,
		MP_CMD_TO_NORMAL,
		MP_SPECIAL1,
		MP_MAX
	}

	public enum PlayerMotionStance
	{
		NORMAL,
		DYING,
		DISABLE,
		CMD,
		DEFEND,
		WIN,
		SPECIAL_ANY_IDLE,
		SPECIAL_INDIFFERENT,
		SPECIAL_UNKNOWN
	}

	public enum PlayerSerialNumber
	{
		SERIAL_ZIDANE_DAGGER,
		SERIAL_ZIDANE_SWORD,
		SERIAL_VIVI,
		SERIAL_GARNET_LH_ROD,
		SERIAL_GARNET_LH_KNIFE,
		SERIAL_GARNET_SH_ROD,
		SERIAL_GARNET_SH_KNIFE,
		SERIAL_STEINER_OUTDOOR,
		SERIAL_STEINER_INDOOR,
		SERIAL_KUINA,
		SERIAL_EIKO_FLUTE,
		SERIAL_EIKO_KNIFE,
		SERIAL_FREIJA,
		SERIAL_SALAMANDER,
		SERIAL_CINNA,
		SERIAL_MARCUS,
		SERIAL_BLANK,
		SERIAL_BLANK_ARMOR,
		SERIAL_BEATRIX,
		SERIAL_MAX
	}

	public enum PlayerWeaponID
	{
		WEP_011,
		WEP_012,
		WEP_013,
		WEP_014,
		WEP_015,
		WEP_016,
		WEP_017,
		WEP_018,
		WEP_019,
		WEP_020,
		WEP_021,
		WEP_022,
		WEP_023,
		WEP_024,
		WEP_025,
		WEP_026,
		WEP_027,
		WEP_028,
		WEP_029,
		WEP_030,
		WEP_031,
		WEP_032,
		WEP_033,
		WEP_034,
		WEP_035,
		WEP_036,
		WEP_037,
		WEP_038,
		WEP_039,
		WEP_040,
		WEP_041,
		WEP_042,
		WEP_043,
		WEP_044,
		WEP_045,
		WEP_046,
		WEP_047,
		WEP_048,
		WEP_049,
		WEP_050,
		WEP_051,
		WEP_052,
		WEP_053,
		WEP_054,
		WEP_055,
		WEP_056,
		WEP_057,
		WEP_058,
		WEP_059,
		WEP_060,
		WEP_061,
		WEP_062,
		WEP_063,
		WEP_064,
		WEP_065,
		WEP_066,
		WEP_067,
		WEP_068,
		WEP_069,
		WEP_070,
		WEP_071,
		WEP_072,
		WEP_073,
		WEP_074,
		WEP_075,
		WEP_076,
		WEP_077,
		WEP_078,
		WEP_079,
		WEP_080,
		WEP_081,
		WEP_082,
		WEP_083,
		WEP_084,
		WEP_085,
		WEP_086,
		WEP_087,
		WEP_088,
		WEP_089,
		WEP_090,
		WEP_091,
		WEP_092,
		WEP_093,
		WEP_094,
		WEP_095,
		WEP_MAX
	}
}
