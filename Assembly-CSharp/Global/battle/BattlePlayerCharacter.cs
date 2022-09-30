using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Data;
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
		String text = BattlePlayerCharacter.PlayerModelToAnimationID.TryGetValue(key, out text) ? text : "no key";
		return text == "no key" ? name : text;
	}

	public static Int32 PlayMotion(GameObject goPlayerCharacter, BattlePlayerCharacter.PlayerMotionIndex playerMotionIndex)
	{
		Animation component = goPlayerCharacter.GetComponent<Animation>();
		String casterAnimationNameID = BattlePlayerCharacter.GetCasterAnimationNameID(component.name);
		String casterAnimationName = BattlePlayerCharacter.GetCasterAnimationName(casterAnimationNameID, playerMotionIndex);
		Int32 result = 0;
		if (component[casterAnimationName] != null)
		{
			Int32 num = Mathf.CeilToInt(component[casterAnimationName].length * component[casterAnimationName].clip.frameRate);
			result = num;
			component.Play(casterAnimationName);
		}
		return result;
	}

	public static void InitMaxFrame(BTL_DATA btl, Byte[] maxFrames)
	{
		Animation component = btl.gameObject.GetComponent<Animation>();
		String dbgMessage = String.Empty;
		for (Int32 i = 0; i < btl.mot.Length; i++)
		{
			String motAnim = btl.mot[i];
			maxFrames[i] = 1;
			if (component[motAnim] != null)
			{
				component[motAnim].speed = 0.5f * FF9StateSystem.Settings.FastForwardFactor;
				maxFrames[i] = (Byte)Mathf.CeilToInt(component[motAnim].length * component[motAnim].clip.frameRate);
			}
			dbgMessage += "animName:" + motAnim + ":" + maxFrames[i] + "\n";
		}
		global::Debug.Log(dbgMessage);
	}

	public static void InitAnimation(BTL_DATA btl)
	{
		for (Int32 i = 0; i < 34; i++)
		{
			String motAnim = btl.mot[i];
			if (btl.gameObject.GetComponent<Animation>()[motAnim] != null)
			{
				AnimationState animationState = btl.gameObject.GetComponent<Animation>()[motAnim];
				if (i == 0 || i == 1 || i == 9 || i == 13 || i == 19 || i == 27 || i == 17 || animationState.name.Substring(animationState.name.Length - 3) == "000")
					animationState.wrapMode = WrapMode.Loop;
				else
					animationState.wrapMode = WrapMode.Once;
				animationState.speed = 0.5f * FF9StateSystem.Settings.FastForwardFactor;
			}
		}
	}

	public static void CreatePlayer(BTL_DATA btl, CharacterSerialNumber playerSerialNumber)
	{
		String text = BattlePlayerCharacter.PlayerModelFileName[(Int32)playerSerialNumber];
		String path = text;
		GameObject gameObject = ModelFactory.CreateModel(path, true);
		BattlePlayerCharacter.CreateTranceModel(btl, playerSerialNumber);
		BattlePlayerCharacter.CheckToHideBattleModel(gameObject, playerSerialNumber);
		btl.gameObject = gameObject;
		btl.originalGo = gameObject;
	}

	private static void CreateTranceModel(BTL_DATA btl, CharacterSerialNumber serial)
	{
		String path = btl_init.trance_model_id[(Int32)serial];
		btl.tranceGo = ModelFactory.CreateModel(path, true);
		BattlePlayerCharacter.CheckToHideBattleModel(btl.tranceGo, serial);
		btl.tranceGo.transform.localPosition = new Vector3(btl.tranceGo.transform.localPosition.x, -10000f, btl.tranceGo.transform.localPosition.z);
		btl.tranceGo.SetActive(false);
	}

	private static void CheckToHideBattleModel(GameObject characterGo, CharacterSerialNumber serial)
	{
		if (serial == CharacterSerialNumber.ZIDANE_SWORD)
		{
			Transform childByName = characterGo.transform.GetChildByName("battle_model");
			if (childByName != null)
			{
				Renderer[] renderers = childByName.GetComponentsInChildren<Renderer>();
				for (Int32 i = 0; i < renderers.Length; i++)
					renderers[i].enabled = false;
			}
		}
	}

	public static readonly String[] PlayerMotionNum = new String[] // Indented by "PlayerMotionIndex"
	{
		"000", // MP_IDLE_NORMAL
		"022", // MP_IDLE_DYING
		"300", // MP_DAMAGE1
		"310", // MP_DAMAGE2
		"033", // MP_DISABLE
		"020", // MP_GET_UP_DYING
		"032", // MP_GET_UP_DISABLE
		"002", // MP_DOWN_DYING
		"023", // MP_DOWN_DISABLE
		"011", // MP_IDLE_CMD
		"001", // MP_NORMAL_TO_CMD
		"021", // MP_DYING_TO_CMD
		"400", // MP_IDLE_TO_DEF
		"401", // MP_DEFENCE
		"402", // MP_DEF_TO_IDLE
		"410", // MP_COVER
		"420", // MP_AVOID
		"430", // MP_ESCAPE
		"500", // MP_WIN
		"501", // MP_WIN_LOOP
		"100", // MP_SET
		"101", // MP_RUN
		"102", // MP_RUN_TO_ATTACK
		"103", // MP_ATTACK
		"104", // MP_BACK
		"105", // MP_ATK_TO_NORMAL
		"200", // MP_IDLE_TO_CHANT
		"201", // MP_CHANT
		"202", // MP_MAGIC
		"040", // MP_STEP_FORWARD
		"050", // MP_STEP_BACK
		"210", // MP_ITEM1
		"010", // MP_CMD_TO_NORMAL
		"220"  // MP_SPECIAL1
	};

	public static String[] PlayerModelFileName = new String[] // Indented by "CharacterSerialNumber"
	{
		"GEO_MAIN_B0_000", // ZIDANE_DAGGER
		"GEO_MAIN_B0_001", // ZIDANE_SWORD
		"GEO_MAIN_B0_006", // VIVI
		"GEO_MAIN_B0_002", // GARNET_LH_ROD
		"GEO_MAIN_B0_003", // GARNET_LH_KNIFE
		"GEO_MAIN_B0_004", // GARNET_SH_ROD
		"GEO_MAIN_B0_005", // GARNET_SH_KNIFE
		"GEO_MAIN_B0_007", // STEINER_OUTDOOR
		"GEO_MAIN_B0_018", // STEINER_INDOOR
		"GEO_MAIN_B0_008", // KUINA
		"GEO_MAIN_B0_009", // EIKO_FLUTE
		"GEO_MAIN_B0_010", // EIKO_KNIFE
		"GEO_MAIN_B0_011", // FREIJA
		"GEO_MAIN_B0_012", // SALAMANDER
		"GEO_MAIN_B0_013", // CINNA
		"GEO_MAIN_B0_014", // MARCUS
		"GEO_MAIN_B0_015", // BLANK
		"GEO_MAIN_B0_016", // BLANK_ARMOR
		"GEO_MAIN_B0_017"  // BEATRIX
	};

	public static Byte[] PlayerWeaponToBoneName = new Byte[] // Indented by "CharacterSerialNumber"
	{
		13, // ZIDANE_DAGGER
		13, // ZIDANE_SWORD
		16, // VIVI
		15, // GARNET_LH_ROD
		15, // GARNET_LH_KNIFE
		15, // GARNET_SH_ROD
		15, // GARNET_SH_KNIFE
		16, // STEINER_OUTDOOR
		16, // STEINER_INDOOR
		14, // KUINA
		15, // EIKO_FLUTE
		15, // EIKO_KNIFE
		6,  // FREIJA
		16, // SALAMANDER
		25, // CINNA
		6,  // MARCUS
		14, // BLANK
		14, // BLANK_ARMOR
		16  // BEATRIX
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
}
