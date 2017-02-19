using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnimationFactory
{
	public static void LoadAnimationUseInEvent(String animationEventData)
	{
		AnimationFactory.animationEventClip.Clear();
		AnimationFactory.animationMapping.Clear();
		TextAsset textAsset = AssetManager.Load<TextAsset>("CommonAsset/EventEngine/EventAnimation/" + animationEventData + ".txt", false);
		if (textAsset == (UnityEngine.Object)null)
		{
			return;
		}
		String text = textAsset.text.Replace("\r", String.Empty);
		if (!text.StartsWith("animation:"))
		{
			return;
		}
		String[] array = text.Split(new Char[]
		{
			'\n'
		});
		String[] array2 = array[0].Replace("animation:", String.Empty).Split(new Char[]
		{
			','
		});
		String[] array3 = array2;
		for (Int32 i = 0; i < (Int32)array3.Length; i++)
		{
			String text2 = array3[i];
			if (!String.IsNullOrEmpty(text2) && !AnimationFactory.animationEventClip.ContainsKey(text2))
			{
				String animationFolder = AnimationFactory.GetAnimationFolder(text2);
				String text3 = "Animations/" + animationFolder + "/" + text2;
				text3 = AnimationFactory.GetRenameAnimationPath(text3);
				AnimationClip value = AssetManager.Load<AnimationClip>(text3, false);
				AnimationFactory.animationEventClip.Add(text2, value);
			}
		}
		Int32 num = (Int32)array.Length;
		for (Int32 j = 1; j < num; j++)
		{
			String text4 = array[j];
			if (!String.IsNullOrEmpty(text4))
			{
				String text5 = text4.Split(new Char[]
				{
					':'
				})[0];
				text4 = text4.Replace(text5 + ":", String.Empty);
				String[] value2 = text4.Split(new Char[]
				{
					','
				});
				AnimationFactory.animationMapping.Add(text5, value2);
			}
		}
	}

	private static String GetAnimationFolder(String animationName)
	{
		String text = String.Empty;
		String[] array = animationName.Split(new Char[]
		{
			'_'
		});
		text = String.Concat(new String[]
		{
			"GEO_",
			array[1],
			"_",
			array[2],
			"_",
			array[3]
		});
		if (AnimationFactory.animationMapping.ContainsKey(text))
		{
			text = AnimationFactory.animationPathTable[text];
		}
		return text;
	}

	public static void AddAnimWithAnimatioName(GameObject go, String animationName)
	{
		Animation component = go.GetComponent<Animation>();
		if (component.GetClip(animationName) == (UnityEngine.Object)null)
		{
			String[] array = animationName.Split(new Char[]
			{
				'_'
			});
			String str = String.Concat(new String[]
			{
				"GEO_",
				array[1],
				"_",
				array[2],
				"_",
				array[3]
			});
			String text = "Animations/" + str + "/" + animationName;
			text = AnimationFactory.GetRenameAnimationPath(text);
			AnimationClip clip = AssetManager.Load<AnimationClip>(text, false);
			component.AddClip(clip, animationName);
		}
	}

	public static void AddAnimToGameObject(GameObject go, String modelName)
	{
		Animation component = go.GetComponent<Animation>();
		if (AnimationFactory.animationMapping.ContainsKey(modelName))
		{
			String[] array = AnimationFactory.animationMapping[modelName];
			String[] array2 = array;
			for (Int32 i = 0; i < (Int32)array2.Length; i++)
			{
				String text = array2[i];
				AnimationClip clip = AnimationFactory.animationEventClip[text];
				component.AddClip(clip, text);
			}
		}
		if (modelName.Contains("_B0_") || modelName.Contains("_B3_") || modelName.Contains("_BF_") || modelName.Contains("_W0_"))
		{
			String name = "Animations/" + modelName;
			AnimationClip[] array3 = AssetManager.LoadAll<AnimationClip>(name);
			if (array3 == null)
			{
				return;
			}
			AnimationClip[] array4 = array3;
			for (Int32 j = 0; j < (Int32)array4.Length; j++)
			{
				AnimationClip animationClip = array4[j];
				Int32 key = -1;
				String text2 = animationClip.name;
				if (Int32.TryParse(animationClip.name, out key))
				{
					text2 = FF9DBAll.AnimationDB.GetValue(key);
				}
				if (!AnimationFactory.animationEventClip.ContainsKey(text2))
				{
					AnimationFactory.animationEventClip.Add(text2, animationClip);
				}
				component.AddClip(animationClip, text2);
			}
			if (modelName.CompareTo("GEO_MON_B3_034") == 0)
			{
				String[] array5 = new String[]
				{
					"Animations/GEO_MON_B3_156/ANH_MON_B3_156_020",
					"Animations/GEO_MON_B3_156/ANH_MON_B3_156_021",
					"Animations/GEO_MON_B3_156/ANH_MON_B3_156_022"
				};
				for (Int32 k = 0; k < (Int32)array5.Length; k++)
				{
					String renameAnimationPath = AnimationFactory.GetRenameAnimationPath(array5[k]);
					AnimationClip animationClip2 = AssetManager.Load<AnimationClip>(renameAnimationPath, false);
					String name2 = animationClip2.name;
					component.AddClip(animationClip2, name2);
				}
			}
		}
	}

	public static String GetRenameAnimationDirectory(String animationDirectory)
	{
		String fileNameWithoutExtension = Path.GetFileNameWithoutExtension(animationDirectory);
		Int32 num = -1;
		if (fileNameWithoutExtension.Equals("GEO_MON_B3_110"))
		{
			return "Animations/347";
		}
		if (fileNameWithoutExtension.Equals("GEO_MON_B3_109"))
		{
			return "Animations/5461";
		}
		if (FF9BattleDB.GEO.TryGetKey(fileNameWithoutExtension, out num))
		{
			return "Animations/" + num;
		}
		return animationDirectory;
	}

	public static String GetRenameAnimationPath(String animationPath)
	{
		String fileNameWithoutExtension = Path.GetFileNameWithoutExtension(animationPath);
		Int32 num = -1;
		if (FF9DBAll.AnimationDB.TryGetKey(fileNameWithoutExtension, out num))
		{
			String text = Path.GetDirectoryName(animationPath);
			text = AnimationFactory.GetRenameAnimationDirectory(text);
			return text + "/" + num;
		}
		return animationPath;
	}

	public static Dictionary<String, String> animationPathTable = new Dictionary<String, String>
	{
		{
			"GEO_MAIN_F1_GRN",
			"GEO_MAIN_F0_GRN"
		},
		{
			"GEO_MAIN_F1_STN",
			"GEO_MAIN_F0_STN"
		},
		{
			"GEO_MAIN_F3_GRN",
			"GEO_MAIN_F4_GRN"
		},
		{
			"GEO_MAIN_F7_VIV",
			"GEO_MAIN_F0_VIV"
		},
		{
			"GEO_NPC_F1_APF",
			"GEO_NPC_F0_APF"
		},
		{
			"GEO_NPC_F1_APM",
			"GEO_NPC_F0_APM"
		},
		{
			"GEO_NPC_F1_BBA",
			"GEO_NPC_F0_BBA"
		},
		{
			"GEO_NPC_F1_CAT",
			"GEO_NPC_F0_CAT"
		},
		{
			"GEO_NPC_F1_CHO",
			"GEO_NPC_F0_CHO"
		},
		{
			"GEO_NPC_F1_DAC",
			"GEO_NPC_F0_DAC"
		},
		{
			"GEO_NPC_F1_DAF",
			"GEO_NPC_F0_DAF"
		},
		{
			"GEO_NPC_F1_DOC",
			"GEO_NPC_F0_DOC"
		},
		{
			"GEO_NPC_F1_DOF",
			"GEO_NPC_F0_DOF"
		},
		{
			"GEO_NPC_F1_DOG",
			"GEO_NPC_F0_DOG"
		},
		{
			"GEO_NPC_F1_DOK",
			"GEO_NPC_F0_DOK"
		},
		{
			"GEO_NPC_F1_DOM",
			"GEO_NPC_F0_DOM"
		},
		{
			"GEO_NPC_F1_G17",
			"GEO_NPC_F0_G17"
		},
		{
			"GEO_NPC_F1_GUD",
			"GEO_NPC_F0_GUD"
		},
		{
			"GEO_NPC_F1_HTH",
			"GEO_NPC_F0_HTH"
		},
		{
			"GEO_NPC_F1_HUF",
			"GEO_NPC_F0_HUF"
		},
		{
			"GEO_NPC_F1_HUM",
			"GEO_NPC_F0_HUM"
		},
		{
			"GEO_NPC_F1_JJY",
			"GEO_NPC_F0_JJY"
		},
		{
			"GEO_NPC_F1_KAC",
			"GEO_NPC_F0_KAC"
		},
		{
			"GEO_NPC_F1_MOG",
			"GEO_NPC_F0_MOG"
		},
		{
			"GEO_NPC_F1_OFF",
			"GEO_NPC_F0_OFF"
		},
		{
			"GEO_NPC_F1_RAS",
			"GEO_NPC_F0_RAS"
		},
		{
			"GEO_NPC_F1_TBY",
			"GEO_NPC_F0_TBY"
		},
		{
			"GEO_NPC_F1_TCK",
			"GEO_NPC_F0_TCK"
		},
		{
			"GEO_NPC_F1_TGR",
			"GEO_NPC_F0_TGR"
		},
		{
			"GEO_NPC_F1_TMF",
			"GEO_NPC_F0_TMF"
		},
		{
			"GEO_NPC_F1_TMM",
			"GEO_NPC_F0_TMM"
		},
		{
			"GEO_NPC_F1_WRK",
			"GEO_NPC_F0_WRK"
		},
		{
			"GEO_NPC_F2_APF",
			"GEO_NPC_F0_APF"
		},
		{
			"GEO_NPC_F2_APM",
			"GEO_NPC_F0_APM"
		},
		{
			"GEO_NPC_F2_BBA",
			"GEO_NPC_F0_BBA"
		},
		{
			"GEO_NPC_F2_CHO",
			"GEO_NPC_F0_CHO"
		},
		{
			"GEO_NPC_F2_DAC",
			"GEO_NPC_F0_DAC"
		},
		{
			"GEO_NPC_F2_DOM",
			"GEO_NPC_F0_DOM"
		},
		{
			"GEO_NPC_F2_G17",
			"GEO_NPC_F0_G17"
		},
		{
			"GEO_NPC_F2_HTH",
			"GEO_NPC_F0_HTH"
		},
		{
			"GEO_NPC_F2_HUM",
			"GEO_NPC_F0_HUM"
		},
		{
			"GEO_NPC_F2_JJY",
			"GEO_NPC_F0_JJY"
		},
		{
			"GEO_NPC_F2_KAC",
			"GEO_NPC_F0_KAC"
		},
		{
			"GEO_NPC_F2_TBY",
			"GEO_NPC_F0_TBY"
		},
		{
			"GEO_NPC_F2_TGR",
			"GEO_NPC_F0_TGR"
		},
		{
			"GEO_NPC_F3_APM",
			"GEO_NPC_F0_APM"
		},
		{
			"GEO_NPC_F3_BBA",
			"GEO_NPC_F0_BBA"
		},
		{
			"GEO_NPC_F3_CHO",
			"GEO_NPC_F0_CHO"
		},
		{
			"GEO_NPC_F3_HUF",
			"GEO_NPC_F0_HUF"
		},
		{
			"GEO_NPC_F3_JJY",
			"GEO_NPC_F0_JJY"
		},
		{
			"GEO_NPC_F3_TBY",
			"GEO_NPC_F0_TBY"
		},
		{
			"GEO_NPC_F3_TGR",
			"GEO_NPC_F0_TGR"
		},
		{
			"GEO_NPC_F4_APM",
			"GEO_NPC_F0_APM"
		},
		{
			"GEO_NPC_F4_CHO",
			"GEO_NPC_F0_CHO"
		},
		{
			"GEO_NPC_F4_CSM",
			"GEO_NPC_F1_CSM"
		},
		{
			"GEO_NPC_F4_CSO",
			"GEO_NPC_F0_CSO"
		},
		{
			"GEO_NPC_F4_JJY",
			"GEO_NPC_F0_JJY"
		},
		{
			"GEO_NPC_F5_CSA",
			"GEO_NPC_F1_CSA"
		},
		{
			"GEO_NPC_F5_CSM",
			"GEO_NPC_F2_CSM"
		},
		{
			"GEO_NPC_F5_MOG",
			"GEO_NPC_F0_MOG"
		},
		{
			"GEO_NPC_F6_CSA",
			"GEO_NPC_F2_CSA"
		},
		{
			"GEO_NPC_F6_CSM",
			"GEO_NPC_F3_CSM"
		},
		{
			"GEO_NPC_F7_CSM",
			"GEO_NPC_F0_CSM"
		},
		{
			"GEO_SUB_F1_ZON",
			"GEO_SUB_F0_ZON"
		},
		{
			"GEO_SUB_F4_SSB",
			"GEO_SUB_F1_SSB"
		},
		{
			"GEO_SUB_F7_BLN",
			"GEO_SUB_F0_BLN"
		},
		{
			"GEO_SUB_F7_CNA",
			"GEO_SUB_F0_CNA"
		},
		{
			"GEO_SUB_F7_MRC",
			"GEO_SUB_F0_MRC"
		},
		{
			"GEO_SUB_F3_KJA",
			"GEO_SUB_F0_KJA"
		},
		{
			"GEO_NPC_F1_OSC",
			"GEO_NPC_F0_OSC"
		},
		{
			"GEO_NPC_F2_OSC",
			"GEO_NPC_F0_OSC"
		},
		{
			"GEO_NPC_F1_FRM",
			"GEO_NPC_F0_FRM"
		},
		{
			"GEO_NPC_F0_FRF",
			"GEO_NPC_F0_FRM"
		},
		{
			"GEO_NPC_F0_FRC",
			"GEO_NPC_F0_FRM"
		},
		{
			"GEO_NPC_F1_G20",
			"GEO_NPC_F0_G20"
		},
		{
			"GEO_NPC_F2_G20",
			"GEO_NPC_F0_G20"
		},
		{
			"GEO_ACC_F2_TBX",
			"GEO_ACC_F0_TBX"
		},
		{
			"GEO_ACC_F3_TBX",
			"GEO_ACC_F1_TBX"
		},
		{
			"GEO_ACC_F1_MGP",
			"GEO_ACC_F0_MGP"
		},
		{
			"GEO_ACC_F2_LTT",
			"GEO_ACC_F0_LTT"
		},
		{
			"GEO_ACC_F1_BLL",
			"GEO_ACC_F0_BLL"
		},
		{
			"GEO_ACC_F2_BLL",
			"GEO_ACC_F0_BLL"
		},
		{
			"GEO_ACC_F3_BLL",
			"GEO_ACC_F0_BLL"
		},
		{
			"GEO_ACC_F0_KGG",
			"GEO_ACC_F0_LEV"
		},
		{
			"GEO_ACC_F0_BBX",
			"GEO_ACC_F0_LEV"
		},
		{
			"GEO_ACC_F0_BBT",
			"GEO_ACC_F0_LEV"
		},
		{
			"GEO_ACC_F0_IFE",
			"GEO_ACC_F0_SUP"
		},
		{
			"GEO_ACC_F0_GAS",
			"GEO_ACC_F0_GAB"
		},
		{
			"GEO_ACC_F0_BON",
			"GEO_ACC_F0_CUP"
		},
		{
			"GEO_ACC_F1_HDB",
			"GEO_ACC_F0_HDB"
		},
		{
			"GEO_ACC_F0_KOR",
			"GEO_ACC_F0_CUP"
		},
		{
			"GEO_ACC_F1_SWD",
			"GEO_ACC_F0_CUP"
		},
		{
			"GEO_ACC_F0_LNW",
			"GEO_ACC_F0_CUP"
		},
		{
			"GEO_ACC_F0_KOM",
			"GEO_ACC_F0_CUP"
		},
		{
			"GEO_ACC_F0_KOS",
			"GEO_ACC_F0_CUP"
		},
		{
			"GEO_ACC_F0_ELE",
			"GEO_ACC_F0_CUP"
		},
		{
			"GEO_ACC_F0_HDB",
			"GEO_ACC_F0_CUP"
		},
		{
			"GEO_ACC_F0_STQ",
			"GEO_ACC_F0_CUP"
		},
		{
			"GEO_MON_F0_DRA",
			"GEO_MON_F0_EFM"
		},
		{
			"GEO_MON_F0_EEE",
			"GEO_MON_F0_EFM"
		},
		{
			"GEO_MON_F0_FFF",
			"GEO_MON_F0_EFM"
		},
		{
			"GEO_MON_F0_WWW",
			"GEO_MON_F0_EFM"
		}
	};

	private static String[] defaultAnimation = new String[]
	{
		"IDLE",
		"WALK",
		"RUN",
		"TURN_L",
		"TURN_R"
	};

	private static Dictionary<String, AnimationClip> animationEventClip = new Dictionary<String, AnimationClip>();

	private static Dictionary<String, String[]> animationMapping = new Dictionary<String, String[]>();

	public static Int64 timeUse;
}
