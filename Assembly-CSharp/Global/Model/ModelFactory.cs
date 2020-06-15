﻿using System;
using System.Collections.Generic;
using System.IO;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Scripts;
using UnityEngine;

public class ModelFactory
{
	public static String GetRenameModelPath(String upscalePath)
	{
		String text = Path.GetFileNameWithoutExtension(upscalePath);
		if (ModelFactory.revertUpscaleTable.ContainsKey(text))
		{
			text = ModelFactory.revertUpscaleTable[text];
		}
		else
		{
			text = ModelFactory.GetNameFromFF9DBALL(text);
		}
		Int32 geoid = ModelFactory.GetGEOID(text);
		String text2 = String.Empty;
		if (geoid == -1)
		{
			return upscalePath;
		}
		ModelType modelType;
		if (text.StartsWith("GEO_WEP"))
		{
			text2 = FF9BattleDB.GEO.GetValue(geoid);
			modelType = ModelType.battle_weapon;
			return String.Format("BattleMap/BattleModel/{0}/{1}/{1}", (Int32)modelType, geoid, text);
		}
		text2 = FF9BattleDB.GEO.GetValue(geoid);
		modelType = ModelFactory.GetModelType(upscalePath);
		if (geoid == 429 || geoid == 430)
		{
			modelType = ModelType.sub;
		}
		return String.Format("Models/{0}/{1}/{1}", (Int32)modelType, geoid, text);
	}

	public static String GetRenameTexturePath(String texturePath)
	{
		OSDLogger.AddStaticMessage("---------End GetRenameTexturePath-----------------");
		String result = String.Empty;
		String fileNameWithoutExtension = Path.GetFileNameWithoutExtension(texturePath);
		String nameFromFF9DBALL = ModelFactory.GetNameFromFF9DBALL(fileNameWithoutExtension);
		Int32 geoid = ModelFactory.GetGEOID(nameFromFF9DBALL);
		String modelName = FF9BattleDB.GEO.GetValue(geoid);
		ModelType modelType = ModelFactory.GetModelType(modelName);
		Char c = fileNameWithoutExtension[fileNameWithoutExtension.Length - 1];
		result = String.Format("Models/{0}/{1}/{1}", (Int32)modelType, geoid);
		OSDLogger.AddStaticMessage("---------End GetRenameModelPath-----------------");
		return result;
	}

	public static GameObject CreateModel(String path, Boolean isBattle = false)
	{
		String text = path;
		path = ModelFactory.CheckUpscale(path);
		String renameModelPath = ModelFactory.GetRenameModelPath(path);
		UnityEngine.Object @object = AssetManager.Load<GameObject>(renameModelPath, out _, false);
		if (@object == (UnityEngine.Object)null)
		{
			return (GameObject)null;
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(@object);
		if (text == "GEO_MAIN_F3_ZDN" || text == "GEO_MAIN_F4_ZDN" || text == "GEO_MAIN_F5_ZDN")
		{
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			for (Int32 i = 0; i < (Int32)array.Length; i++)
			{
				Renderer renderer = array[i];
				String name = renderer.material.mainTexture.name;
				Char c = name[name.Length - 1];
				String text2 = ModelFactory.GetGEOID(text).ToString();
				String str = text2 + "_" + c;
				String name2 = "Models/2/" + text2 + "/" + str;
				String[] pngInfo;
				Texture texture = AssetManager.Load<Texture>(name2, out pngInfo, false);
				renderer.material.SetTexture("_MainTex", texture);
			}
		}
		Shader shader;
		if (text.Contains("GEO_SUB_W0"))
		{
			if (text.Contains("GEO_SUB_W0_025"))
			{
				shader = ShadersLoader.Find("WorldMap/ShadowActor");
			}
			else
			{
				shader = ShadersLoader.Find("WorldMap/Actor");
			}
		}
		else
		{
			shader = ShadersLoader.Find((!isBattle) ? "Unlit/Transparent Cutout" : "BattleMap_Common");
		}
		SkinnedMeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (Int32 j = 0; j < (Int32)componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].material.shader = shader;
		}
		MeshRenderer[] componentsInChildren3 = gameObject.GetComponentsInChildren<MeshRenderer>();
		for (Int32 k = 0; k < (Int32)componentsInChildren3.Length; k++)
		{
			Material[] materials = componentsInChildren3[k].materials;
			for (Int32 l = 0; l < (Int32)materials.Length; l++)
			{
				Material material = materials[l];
				String text3 = material.name.Replace("(Instance)", String.Empty);
				if (componentsInChildren3[k].name == "Group_2")
				{
					material.shader = ShadersLoader.Find("BattleMap_Ground");
				}
				else if (text3.Contains("a"))
				{
					material.shader = ShadersLoader.Find("PSX/BattleMap_Abr_1");
				}
				else
				{
					material.shader = shader;
				}
			}
		}
		if (ModelFactory.garnetShortHairTable.Contains(text))
		{
		    Boolean garnetShortHair;

		    if (Configuration.Graphics.GarnetHair == 1)
		        garnetShortHair = false;
            else if (Configuration.Graphics.GarnetHair == 2)
		        garnetShortHair = true;
		    else
		        garnetShortHair = BitConverter.ToUInt16(FF9StateSystem.EventState.gEventGlobal, 0) >= 10300;

            if (garnetShortHair)
			{
				Renderer[] componentsInChildren4 = gameObject.transform.GetChildByName("long_hair").GetComponentsInChildren<Renderer>();
				Renderer[] array2 = componentsInChildren4;
				for (Int32 m = 0; m < (Int32)array2.Length; m++)
				{
					Renderer renderer2 = array2[m];
					renderer2.enabled = false;
				}
			}
			else
			{
				Renderer[] componentsInChildren5 = gameObject.transform.GetChildByName("short_hair").GetComponentsInChildren<Renderer>();
				Renderer[] array3 = componentsInChildren5;
				for (Int32 n = 0; n < (Int32)array3.Length; n++)
				{
					Renderer renderer3 = array3[n];
					renderer3.enabled = false;
				}
			}
		}
		if (gameObject != (UnityEngine.Object)null)
		{
			AnimationFactory.AddAnimToGameObject(gameObject, text);
			if (text.Contains("GEO_MON_"))
			{
				if (ModelFactory.upscaleTable.ContainsKey(text))
				{
					text = ModelFactory.upscaleTable[text];
				}
				text = text.Replace("_UP0", "_B3");
				AnimationFactory.AddAnimToGameObject(gameObject, text);
			}
		}
		if (gameObject != (UnityEngine.Object)null)
		{
			if (isBattle)
			{
				Transform childByName = gameObject.transform.GetChildByName("field_model");
				if (childByName != (UnityEngine.Object)null)
				{
					Renderer[] componentsInChildren6 = childByName.GetComponentsInChildren<Renderer>();
					Renderer[] array4 = componentsInChildren6;
					for (Int32 num2 = 0; num2 < (Int32)array4.Length; num2++)
					{
						Renderer renderer4 = array4[num2];
						renderer4.enabled = false;
					}
				}
			}
			else
			{
				Transform childByName2 = gameObject.transform.GetChildByName("battle_model");
				if (childByName2 != (UnityEngine.Object)null)
				{
					Renderer[] componentsInChildren7 = childByName2.GetComponentsInChildren<Renderer>();
					Renderer[] array5 = componentsInChildren7;
					for (Int32 num3 = 0; num3 < (Int32)array5.Length; num3++)
					{
						Renderer renderer5 = array5[num3];
						renderer5.enabled = false;
					}
				}
			}
		}
		return gameObject;
	}

	public static Boolean IsUseAsEnemyCharacter(String path)
	{
		return path.Contains("GEO_MON_B3_148") || path.Contains("GEO_MON_B3_155") || path.Contains("GEO_MON_B3_168") || path.Contains("GEO_MON_B3_169") || path.Contains("GEO_MON_B3_170") || path.Contains("GEO_MON_B3_171") || path.Contains("GEO_MON_B3_172") || path.Contains("GEO_MON_B3_173") || path.Contains("GEO_MON_B3_174") || path.Contains("GEO_MON_B3_175") || path.Contains("GEO_MON_B3_182") || path.Contains("GEO_MON_B3_195");
	}

	public static GameObject CreateDefaultWeaponForCharacterWhenUseAsEnemy(String path)
	{
		Int32 num = ModelFactory.defaultWeaponTable[path];
		ItemAttack weapon = ff9weap.WeaponData[num];
		String text = FF9BattleDB.GEO.GetValue((Int32)weapon.ModelId);
		global::Debug.LogWarning("-------------------------------------------------------------------------");
		return ModelFactory.CreateModel("BattleMap/BattleModel/battle_weapon/" + text + "/" + text, true);
	}

	public static Int32 GetDefaultWeaponBoneIdForCharacterWhenUseAsEnemy(String path)
	{
		return ModelFactory.defaultWeaponBoneTable[path];
	}

	public static Boolean HaveUpScaleModel(String modelName)
	{
		return ModelFactory.upscaleTable.ContainsKey(modelName);
	}

	public static String GetUpScaleModel(String modelName)
	{
		String result;
		if (!ModelFactory.HaveUpScaleModel(modelName))
		{
			result = modelName;
		}
		else
		{
			result = ModelFactory.upscaleTable[modelName];
		}
		return result;
	}

	public static String CheckUpscale(String path)
	{
		String text = Path.GetFileNameWithoutExtension(path);
		String extension = Path.GetExtension(path);
		String text2 = String.Empty;
		if (ModelFactory.upscaleTable.ContainsKey(text))
		{
			text2 = ModelFactory.findModelFilePathFromModelCode(text);
			text = ModelFactory.upscaleTable[text];
		}
		if (path.StartsWith("GEO_"))
		{
			text2 = ModelFactory.findModelFilePathFromModelCode(text);
		}
		else
		{
			text2 = Path.GetDirectoryName(Path.GetDirectoryName(path));
			text2 = Path.GetDirectoryName(Path.GetDirectoryName(path));
		}
		if (extension != String.Empty)
		{
			text = text + "." + extension;
		}
		return String.Concat(new String[]
		{
			text2,
			"/",
			text,
			"/",
			text
		});
	}

	private static String findModelFilePathFromModelCode(String modelCode)
	{
		if (modelCode == null)
		{
			return (String)null;
		}
		String result;
		if (modelCode.Contains("GEO_WEP"))
		{
			result = "BattleMap/BattleModel/battle_weapon";
		}
		else
		{
			Int32 num = modelCode.IndexOf('_');
			Int32 num2 = modelCode.IndexOf('_', num + 1);
			String text = modelCode.Substring(num + 1, num2 - num - 1);
			result = "Models/" + text.ToLower();
		}
		return result;
	}

	public static GameObject CreateModel(String path, Vector3 position, Vector3 rotation, Boolean isBattle = false)
	{
		GameObject gameObject = ModelFactory.CreateModel(path, isBattle);
		gameObject.transform.localPosition = position;
		gameObject.transform.localRotation = Quaternion.Euler(rotation);
		return gameObject;
	}

    public static String GetNameFromFF9DBALL(String modelName)
    {
        String text = Path.GetFileNameWithoutExtension(modelName);
        if (text == null)
            return null;

        switch (text)
        {
            case "GEO_MAIN_UP3_ZDN":
                text = "GEO_MAIN_F3_ZDN";
                break;
            case "GEO_MAIN_UP4_ZDN":
                text = "GEO_MAIN_F4_ZDN";
                break;
            case "GEO_MAIN_UP5_ZDN":
                text = "GEO_MAIN_F5_ZDN";
                break;
            case "GEO_MAIN_UP3_ZDN_0":
                text = "GEO_MAIN_F3_ZDN";
                break;
            case "GEO_MAIN_UP4_ZDN_0":
                text = "GEO_MAIN_F4_ZDN";
                break;
            case "GEO_MAIN_UP5_ZDN_0":
                text = "GEO_MAIN_F5_ZDN";
                break;
            case "GEO_MAIN_UP3_ZDN_1":
                text = "GEO_MAIN_F3_ZDN";
                break;
            case "GEO_MAIN_UP4_ZDN_1":
                text = "GEO_MAIN_F4_ZDN";
                break;
            case "GEO_MAIN_UP5_ZDN_1":
                text = "GEO_MAIN_F5_ZDN";
                break;
            case "GEO_MAIN_UP4_GRN":
                text = "GEO_MAIN_F4_GRN";
                break;
        }

        String result;
        if (ModelFactory.revertUpscaleTable.TryGetValue(text, out result))
            text = result;

        return text;
    }

    public static Int32 GetGEOID(String modelName)
    {
        String fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modelName);
        if (fileNameWithoutExtension.Equals("GEO_MON_B3_110"))
        {
            return 347;
        }
        if (modelName.Equals("GEO_MON_B3_109"))
        {
            return 5461;
        }

        Int32 id;
        if (!FF9BattleDB.GEO.TryGetKey(modelName, out id))
            id = -1;
        return id;
    }

    public static ModelType GetModelType(String modelName)
	{
		Int32 num = modelName.IndexOf('_');
		Int32 num2 = modelName.IndexOf('_', num + 1);
		String value = modelName.Substring(num + 1, num2 - num - 1).ToLower();
		return (ModelType)((Int32)Enum.Parse(typeof(ModelType), value));
	}

	public static Dictionary<String, String> upscaleTable = new Dictionary<String, String>
	{
		{
			"GEO_SUB_F0_BRN",
			"GEO_SUB_UP0_BRN"
		},
		{
			"GEO_SUB_F0_CID",
			"GEO_SUB_UP0_CID"
		},
		{
			"GEO_SUB_F0_CDW",
			"GEO_SUB_UP0_CDW"
		},
		{
			"GEO_SUB_F0_KUW",
			"GEO_SUB_UP0_KUW"
		},
		{
			"GEO_MAIN_F0_ZDD",
			"GEO_MAIN_UP0_ZDD"
		},
		{
			"GEO_SUB_F0_FLT",
			"GEO_SUB_UP0_FLT"
		},
		{
			"GEO_SUB_F0_RBY",
			"GEO_SUB_UP0_RBY"
		},
		{
			"GEO_SUB_F0_ZNR",
			"GEO_SUB_UP0_ZNR"
		},
		{
			"GEO_SUB_F0_KUT",
			"GEO_SUB_UP0_KUT"
		},
		{
			"GEO_NPC_F0_MOG",
			"GEO_NPC_UP0_MOG"
		},
		{
			"GEO_NPC_F1_MOG",
			"GEO_NPC_UP1_MOG"
		},
		{
			"GEO_NPC_F2_MOG",
			"GEO_NPC_UP2_MOG"
		},
		{
			"GEO_NPC_F3_MOG",
			"GEO_NPC_UP3_MOG"
		},
		{
			"GEO_NPC_F4_MOG",
			"GEO_NPC_UP4_MOG"
		},
		{
			"GEO_NPC_F5_MOG",
			"GEO_NPC_UP5_MOG"
		},
		{
			"GEO_NPC_F0_CHO",
			"GEO_NPC_UP0_CHO"
		},
		{
			"GEO_NPC_F1_CHO",
			"GEO_NPC_UP1_CHO"
		},
		{
			"GEO_NPC_F2_CHO",
			"GEO_NPC_UP2_CHO"
		},
		{
			"GEO_NPC_F3_CHO",
			"GEO_NPC_UP3_CHO"
		},
		{
			"GEO_NPC_F4_CHO",
			"GEO_NPC_UP4_CHO"
		},
		{
			"GEO_NPC_F0_CHD",
			"GEO_NPC_UP0_CHD"
		},
		{
			"GEO_MAIN_F0_ZDN",
			"GEO_MAIN_UP0_ZDN"
		},
		{
			"GEO_MAIN_B0_000",
			"GEO_MAIN_UP0_ZDN"
		},
		{
			"GEO_MAIN_B0_001",
			"GEO_MAIN_UP0_ZDN"
		},
		{
			"GEO_MON_B3_168",
			"GEO_MON_UP0_168"
		},
		{
			"GEO_MAIN_B0_022",
			"GEO_MAIN_UP0_ZDNT"
		},
		{
			"GEO_MAIN_B0_023",
			"GEO_MAIN_UP0_ZDNT"
		},
		{
			"GEO_MAIN_B0_006",
			"GEO_MAIN_UP0_VIV"
		},
		{
			"GEO_MON_B3_151",
			"GEO_MAIN_UP0_VIV"
		},
		{
			"GEO_MAIN_F0_VIV",
			"GEO_MAIN_UP1_VIV"
		},
		{
			"GEO_MON_B3_170",
			"GEO_MAIN_UP1_VIV"
		},
		{
			"GEO_MAIN_B0_028",
			"GEO_MAIN_UP0_VIVT"
		},
		{
			"GEO_MAIN_F0_GRN",
			"GEO_MAIN_UP0_GRN"
		},
		{
			"GEO_MAIN_F1_GRN",
			"GEO_MAIN_UP0_GRN"
		},
		{
			"GEO_MAIN_B0_002",
			"GEO_MAIN_UP0_GRN"
		},
		{
			"GEO_MAIN_B0_003",
			"GEO_MAIN_UP0_GRN"
		},
		{
			"GEO_MAIN_B0_004",
			"GEO_MAIN_UP0_GRN"
		},
		{
			"GEO_MAIN_B0_005",
			"GEO_MAIN_UP0_GRN"
		},
		{
			"GEO_MON_B3_149",
			"GEO_MAIN_UP0_GRN"
		},
		{
			"GEO_MON_B3_169",
			"GEO_MAIN_UP0_GRN"
		},
		{
			"GEO_MAIN_B0_024",
			"GEO_MAIN_UP0_GRNT"
		},
		{
			"GEO_MAIN_B0_025",
			"GEO_MAIN_UP0_GRNT"
		},
		{
			"GEO_MAIN_B0_026",
			"GEO_MAIN_UP0_GRNT"
		},
		{
			"GEO_MAIN_B0_027",
			"GEO_MAIN_UP0_GRNT"
		},
		{
			"GEO_MAIN_F0_STN",
			"GEO_MAIN_UP0_STN"
		},
		{
			"GEO_MAIN_B0_007",
			"GEO_MAIN_UP0_STN"
		},
		{
			"GEO_MAIN_F1_STN",
			"GEO_MAIN_UP1_STN"
		},
		{
			"GEO_MAIN_B0_018",
			"GEO_MAIN_UP1_STN"
		},
		{
			"GEO_MON_B3_148",
			"GEO_MAIN_UP1_STN"
		},
		{
			"GEO_MON_B3_171",
			"GEO_MAIN_UP1_STN"
		},
		{
			"GEO_MAIN_B0_029",
			"GEO_MAIN_UP0_STNT"
		},
		{
			"GEO_MAIN_F0_FRJ",
			"GEO_MAIN_UP0_FRJ"
		},
		{
			"GEO_MAIN_B0_011",
			"GEO_MAIN_UP0_FRJ"
		},
		{
			"GEO_MON_B3_174",
			"GEO_MAIN_UP0_FRJ"
		},
		{
			"GEO_MAIN_B0_033",
			"GEO_MAIN_UP0_FRJT"
		},
		{
			"GEO_MAIN_F0_KUI",
			"GEO_MAIN_UP0_KUI"
		},
		{
			"GEO_MAIN_B0_008",
			"GEO_MAIN_UP0_KUI"
		},
		{
			"GEO_MON_B3_172",
			"GEO_MAIN_UP0_KUI"
		},
		{
			"GEO_MAIN_B0_030",
			"GEO_MAIN_UP0_KUIT"
		},
		{
			"GEO_MAIN_F0_SLM",
			"GEO_MAIN_UP0_SLM"
		},
		{
			"GEO_MAIN_B0_012",
			"GEO_MAIN_UP0_SLM"
		},
		{
			"GEO_MON_B3_175",
			"GEO_MAIN_UP0_SLM"
		},
		{
			"GEO_MON_B3_182",
			"GEO_MAIN_UP0_SLM"
		},
		{
			"GEO_MAIN_B0_034",
			"GEO_MAIN_UP0_SLMT"
		},
		{
			"GEO_MAIN_F0_EIK",
			"GEO_MAIN_UP0_EIK"
		},
		{
			"GEO_MAIN_B0_009",
			"GEO_MAIN_UP0_EIK"
		},
		{
			"GEO_MAIN_B0_010",
			"GEO_MAIN_UP0_EIK"
		},
		{
			"GEO_MON_B3_173",
			"GEO_MAIN_UP0_EIK"
		},
		{
			"GEO_MAIN_B0_031",
			"GEO_MAIN_UP0_EIKT"
		},
		{
			"GEO_MAIN_B0_032",
			"GEO_MAIN_UP0_EIKT"
		},
		{
			"GEO_MAIN_F0_STD",
			"GEO_MAIN_UP0_STD"
		},
		{
			"GEO_MAIN_F1_ZDN",
			"GEO_MAIN_UP1_ZDN"
		},
		{
			"GEO_MAIN_F2_ZDN",
			"GEO_MAIN_UP2_ZDN"
		},
		{
			"GEO_MAIN_F3_ZDN",
			"GEO_MAIN_UP0_ZDN"
		},
		{
			"GEO_MAIN_F4_ZDN",
			"GEO_MAIN_UP0_ZDN"
		},
		{
			"GEO_MAIN_F5_ZDN",
			"GEO_MAIN_UP0_ZDN"
		},
		{
			"GEO_MAIN_F3_GRN",
			"GEO_MAIN_UP3_GRN"
		},
		{
			"GEO_MAIN_F4_GRN",
			"GEO_MAIN_UP3_GRN"
		},
		{
			"GEO_MAIN_F5_GRN",
			"GEO_MAIN_UP5_GRN"
		},
		{
			"GEO_SUB_F0_CNA",
			"GEO_SUB_UP0_CNA"
		},
		{
			"GEO_SUB_F7_CNA",
			"GEO_SUB_UP0_CNA"
		},
		{
			"GEO_MAIN_B0_013",
			"GEO_SUB_UP0_CNA"
		},
		{
			"GEO_SUB_F0_MRC",
			"GEO_SUB_UP0_MRC"
		},
		{
			"GEO_SUB_F7_MRC",
			"GEO_SUB_UP0_MRC"
		},
		{
			"GEO_MAIN_B0_014",
			"GEO_SUB_UP0_MRC"
		},
		{
			"GEO_SUB_F0_BLN",
			"GEO_SUB_UP0_BLN"
		},
		{
			"GEO_SUB_F7_BLN",
			"GEO_SUB_UP0_BLN"
		},
		{
			"GEO_MAIN_B0_015",
			"GEO_SUB_UP0_BLN"
		},
		{
			"GEO_MON_B3_195",
			"GEO_SUB_UP0_BLN"
		},
		{
			"GEO_SUB_F1_BLN",
			"GEO_SUB_UP1_BLN"
		},
		{
			"GEO_MAIN_B0_016",
			"GEO_SUB_UP1_BLN"
		},
		{
			"GEO_SUB_F0_BTX",
			"GEO_SUB_UP0_BTX"
		},
		{
			"GEO_MAIN_B0_017",
			"GEO_SUB_UP0_BTX"
		},
		{
			"GEO_MON_B3_155",
			"GEO_SUB_UP0_BTX"
		},
		{
			"GEO_SUB_F0_BAK",
			"GEO_SUB_UP0_BAK"
		},
		{
			"GEO_SUB_F1_BAK",
			"GEO_SUB_UP2_BAK"
		},
		{
			"GEO_MON_B3_106",
			"GEO_SUB_UP2_BAK"
		},
		{
			"GEO_SUB_F2_BAK",
			"GEO_SUB_UP1_BAK"
		},
		{
			"GEO_MON_B3_107",
			"GEO_SUB_UP1_BAK"
		},
		{
			"GEO_MON_B3_118",
			"GEO_MON_UP0_118"
		},
		{
			"GEO_MON_F0_CLB",
			"GEO_MON_UP0_118"
		},
		{
			"GEO_MON_B3_132",
			"GEO_MON_UP0_132"
		},
		{
			"GEO_MON_F0_EEE",
			"GEO_MON_UP0_132"
		},
		{
			"GEO_MON_B3_114",
			"GEO_MON_UP0_114"
		},
		{
			"GEO_MON_F0_DRA",
			"GEO_MON_UP0_114"
		},
		{
			"GEO_MON_B3_110",
			"GEO_MON_UP0_110"
		},
		{
			"GEO_MON_F2_EFM",
			"GEO_MON_UP0_110"
		},
		{
			"GEO_MON_B3_109",
			"GEO_MON_UP0_109"
		},
		{
			"GEO_MON_F0_EFM",
			"GEO_MON_UP0_109"
		},
		{
			"GEO_MON_B3_117",
			"GEO_MON_UP0_117"
		},
		{
			"GEO_MON_F0_TOM",
			"GEO_MON_UP0_117"
		},
		{
			"GEO_MON_B3_176",
			"GEO_MON_UP0_176"
		},
		{
			"GEO_MON_F0_CDR",
			"GEO_MON_UP0_176"
		},
		{
			"GEO_MON_B3_131",
			"GEO_MON_UP0_131"
		},
		{
			"GEO_MON_F0_DAH",
			"GEO_MON_UP0_131"
		},
		{
			"GEO_MON_B3_136",
			"GEO_MON_UP0_136"
		},
		{
			"GEO_MON_F0_SDR",
			"GEO_MON_UP0_136"
		},
		{
			"GEO_MON_B3_124",
			"GEO_MON_UP0_124"
		},
		{
			"GEO_MON_F0_ZZZ",
			"GEO_MON_UP0_124"
		},
		{
			"GEO_MAIN_F7_VIV",
			"GEO_MAIN_UP1_VIV"
		},
		{
			"GEO_MON_B3_121",
			"GEO_MON_UP3_121"
		},
		{
			"GEO_MON_F1_TOM",
			"GEO_MON_UP3_121"
		},
		{
			"GEO_MON_B3_112",
			"GEO_MON_UP3_112"
		},
		{
			"GEO_MON_B3_186",
			"GEO_MON_UP3_186"
		},
		{
			"GEO_SUB_F0_BW1",
			"GEO_MON_UP3_111"
		},
		{
			"GEO_MON_B3_111",
			"GEO_MON_UP3_111"
		},
		{
			"GEO_SUB_F0_BW2",
			"GEO_MON_UP3_113"
		},
		{
			"GEO_MON_B3_113",
			"GEO_MON_UP3_113"
		},
		{
			"GEO_SUB_F0_BW3",
			"GEO_MON_UP3_115"
		},
		{
			"GEO_MON_B3_115",
			"GEO_MON_UP3_115"
		},
		{
			"GEO_SUB_F0_KJA",
			"GEO_MON_UP3_125"
		},
		{
			"GEO_MON_B3_125",
			"GEO_MON_UP3_125"
		},
		{
			"GEO_SUB_F3_KJA",
			"GEO_SUB_UP1_KJA"
		},
		{
			"GEO_SUB_F0_KJG",
			"GEO_SUB_UP0_KJAT"
		},
		{
			"GEO_MON_B3_122",
			"GEO_MON_UP3_122"
		},
		{
			"GEO_SUB_F0_ZON",
			"GEO_MON_UP3_119"
		},
		{
			"GEO_MON_B3_119",
			"GEO_MON_UP3_119"
		},
		{
			"GEO_SUB_F1_BW3",
			"GEO_MON_UP3_116"
		},
		{
			"GEO_MON_B3_116",
			"GEO_MON_UP3_116"
		},
		{
			"GEO_MON_B3_185",
			"GEO_MON_UP3_185"
		},
		{
			"GEO_MON_B3_144",
			"GEO_MON_UP3_144"
		},
		{
			"GEO_MON_B3_181",
			"GEO_MON_UP3_181"
		},
		{
			"GEO_SUB_F1_SBW",
			"GEO_MON_UP3_122"
		},
		{
			"GEO_SUB_F1_ZON",
			"GEO_MON_UP3_120"
		},
		{
			"GEO_MON_B3_120",
			"GEO_MON_UP3_120"
		},
		{
			"GEO_MON_B3_145",
			"GEO_MON_UP3_145"
		},
		{
			"GEO_MON_F1_EFM",
			"GEO_MON_B3_108"
		},
		{
			"GEO_ACC_F1_LTT",
			"GEO_ACC_F0_LTT"
		},
		{
			"GEO_ACC_F1_TKT",
			"GEO_ACC_F0_TKT"
		},
		{
			"GEO_MON_B3_130",
			"GEO_MON_UP3_130"
		},
		{
			"GEO_ACC_F1_HDB",
			"GEO_ACC_F0_HDB"
		},
		{
			"GEO_MAIN_F8_GRN",
			"GEO_MAIN_UP8_GRN"
		},
		{
			"GEO_MON_B3_123",
			"GEO_MON_UP3_123"
		},
		{
			"GEO_MON_B3_137",
			"GEO_MON_UP3_137"
		},
		{
			"GEO_MON_B3_138",
			"GEO_MON_UP3_138"
		},
		{
			"GEO_MON_B3_140",
			"GEO_MON_UP3_140"
		},
		{
			"GEO_MON_B3_143",
			"GEO_MON_UP3_143"
		},
		{
			"GEO_MON_B3_146",
			"GEO_MON_UP3_146"
		},
		{
			"GEO_MON_B3_187",
			"GEO_MON_UP3_187"
		},
		{
			"GEO_MON_B3_191",
			"GEO_MON_UP3_191"
		},
		{
			"GEO_MON_B3_192",
			"GEO_MON_UP3_192"
		},
		{
			"GEO_MON_B3_194",
			"GEO_MON_UP3_194"
		},
		{
			"GEO_MON_B3_183",
			"GEO_SUB_UP0_MASK_183"
		},
		{
			"GEO_MON_B3_184",
			"GEO_SUB_UP0_MASK_184"
		},
		{
			"GEO_SUB_F0_GRL",
			"GEO_SUB_UP0_GRL"
		},
		{
			"GEO_MON_B3_128",
			"GEO_MON_UP3_128"
		},
		{
			"GEO_MON_B3_141",
			"GEO_MON_UP3_141"
		},
		{
			"GEO_MON_B3_142",
			"GEO_MON_UP3_142"
		},
		{
			"GEO_MON_B3_147",
			"GEO_MON_UP3_147"
		},
		{
			"GEO_MON_B3_199",
			"GEO_MON_UP3_199"
		},
		{
			"GEO_MON_B3_193",
			"GEO_MON_UP3_193"
		},
		{
			"GEO_MON_B3_152",
			"GEO_SUB_UP0_ZNR"
		},
		{
			"GEO_MON_B3_198",
			"GEO_SUB_UP0_ZNR"
		},
		{
			"GEO_SUB_F0_SBW",
			"GEO_SUB_UP0_SBW"
		}
	};

	public static Dictionary<String, String> revertUpscaleTable = new Dictionary<String, String>
	{
		{
			"GEO_SUB_UP0_BRN",
			"GEO_SUB_F0_BRN"
		},
		{
			"GEO_SUB_UP0_CID",
			"GEO_SUB_F0_CID"
		},
		{
			"GEO_SUB_UP0_CDW",
			"GEO_SUB_F0_CDW"
		},
		{
			"GEO_SUB_UP0_KUW",
			"GEO_SUB_F0_KUW"
		},
		{
			"GEO_MAIN_UP0_ZDD",
			"GEO_MAIN_F0_ZDD"
		},
		{
			"GEO_SUB_UP0_FLT",
			"GEO_SUB_F0_FLT"
		},
		{
			"GEO_SUB_UP0_RBY",
			"GEO_SUB_F0_RBY"
		},
		{
			"GEO_SUB_UP0_ZNR",
			"GEO_SUB_F0_ZNR"
		},
		{
			"GEO_SUB_UP0_KUT",
			"GEO_SUB_F0_KUT"
		},
		{
			"GEO_NPC_UP0_MOG",
			"GEO_NPC_F0_MOG"
		},
		{
			"GEO_NPC_UP1_MOG",
			"GEO_NPC_F1_MOG"
		},
		{
			"GEO_NPC_UP2_MOG",
			"GEO_NPC_F2_MOG"
		},
		{
			"GEO_NPC_UP3_MOG",
			"GEO_NPC_F3_MOG"
		},
		{
			"GEO_NPC_UP4_MOG",
			"GEO_NPC_F4_MOG"
		},
		{
			"GEO_NPC_UP5_MOG",
			"GEO_NPC_F5_MOG"
		},
		{
			"GEO_NPC_UP0_CHO",
			"GEO_NPC_F0_CHO"
		},
		{
			"GEO_NPC_UP1_CHO",
			"GEO_NPC_F1_CHO"
		},
		{
			"GEO_NPC_UP2_CHO",
			"GEO_NPC_F2_CHO"
		},
		{
			"GEO_NPC_UP3_CHO",
			"GEO_NPC_F3_CHO"
		},
		{
			"GEO_NPC_UP4_CHO",
			"GEO_NPC_F4_CHO"
		},
		{
			"GEO_NPC_UP0_CHD",
			"GEO_NPC_F0_CHD"
		},
		{
			"GEO_MAIN_UP0_ZDN",
			"GEO_MAIN_F0_ZDN"
		},
		{
			"GEO_MON_UP0_168",
			"GEO_MON_B3_168"
		},
		{
			"GEO_MAIN_UP0_ZDNT",
			"GEO_MAIN_B0_022"
		},
		{
			"GEO_MAIN_UP0_VIV",
			"GEO_MAIN_B0_006"
		},
		{
			"GEO_MAIN_UP1_VIV",
			"GEO_MAIN_F0_VIV"
		},
		{
			"GEO_MAIN_UP0_VIVT",
			"GEO_MAIN_B0_028"
		},
		{
			"GEO_MAIN_UP0_GRN",
			"GEO_MAIN_F0_GRN"
		},
		{
			"GEO_MAIN_UP0_GRNT",
			"GEO_MAIN_B0_024"
		},
		{
			"GEO_MAIN_UP0_STN",
			"GEO_MAIN_F0_STN"
		},
		{
			"GEO_MAIN_UP1_STN",
			"GEO_MAIN_F1_STN"
		},
		{
			"GEO_MAIN_UP0_STNT",
			"GEO_MAIN_B0_029"
		},
		{
			"GEO_MAIN_UP0_FRJ",
			"GEO_MAIN_F0_FRJ"
		},
		{
			"GEO_MAIN_UP0_FRJT",
			"GEO_MAIN_B0_033"
		},
		{
			"GEO_MAIN_UP0_KUI",
			"GEO_MAIN_F0_KUI"
		},
		{
			"GEO_MAIN_UP0_KUIT",
			"GEO_MAIN_B0_030"
		},
		{
			"GEO_MAIN_UP0_SLM",
			"GEO_MAIN_F0_SLM"
		},
		{
			"GEO_MAIN_UP0_SLMT",
			"GEO_MAIN_B0_034"
		},
		{
			"GEO_MAIN_UP0_EIK",
			"GEO_MAIN_F0_EIK"
		},
		{
			"GEO_MAIN_UP0_EIKT",
			"GEO_MAIN_B0_031"
		},
		{
			"GEO_MAIN_UP0_STD",
			"GEO_MAIN_F0_STD"
		},
		{
			"GEO_MAIN_UP1_ZDN",
			"GEO_MAIN_F1_ZDN"
		},
		{
			"GEO_MAIN_UP2_ZDN",
			"GEO_MAIN_F2_ZDN"
		},
		{
			"GEO_MAIN_UP3_GRN",
			"GEO_MAIN_F3_GRN"
		},
		{
			"GEO_MAIN_UP5_GRN",
			"GEO_MAIN_F5_GRN"
		},
		{
			"GEO_SUB_UP0_CNA",
			"GEO_SUB_F0_CNA"
		},
		{
			"GEO_SUB_UP0_MRC",
			"GEO_SUB_F0_MRC"
		},
		{
			"GEO_SUB_UP0_BLN",
			"GEO_SUB_F0_BLN"
		},
		{
			"GEO_SUB_UP1_BLN",
			"GEO_SUB_F1_BLN"
		},
		{
			"GEO_SUB_UP0_BTX",
			"GEO_SUB_F0_BTX"
		},
		{
			"GEO_SUB_UP0_BAK",
			"GEO_SUB_F0_BAK"
		},
		{
			"GEO_SUB_UP2_BAK",
			"GEO_SUB_F1_BAK"
		},
		{
			"GEO_SUB_UP1_BAK",
			"GEO_SUB_F2_BAK"
		},
		{
			"GEO_MON_UP0_118",
			"GEO_MON_B3_118"
		},
		{
			"GEO_MON_UP0_132",
			"GEO_MON_B3_132"
		},
		{
			"GEO_MON_UP0_114",
			"GEO_MON_B3_114"
		},
		{
			"GEO_MON_UP0_110",
			"GEO_MON_F2_EFM"
		},
		{
			"GEO_MON_UP0_109",
			"GEO_MON_F0_EFM"
		},
		{
			"GEO_MON_UP0_117",
			"GEO_MON_B3_117"
		},
		{
			"GEO_MON_UP0_176",
			"GEO_MON_B3_176"
		},
		{
			"GEO_MON_UP0_131",
			"GEO_MON_B3_131"
		},
		{
			"GEO_MON_UP0_136",
			"GEO_MON_B3_136"
		},
		{
			"GEO_MON_UP0_124",
			"GEO_MON_B3_124"
		},
		{
			"GEO_MAIN_UP7_VIV",
			"GEO_MAIN_F7_VIV"
		},
		{
			"GEO_MON_UP3_121",
			"GEO_MON_B3_121"
		},
		{
			"GEO_MON_UP3_112",
			"GEO_MON_B3_112"
		},
		{
			"GEO_MON_UP3_186",
			"GEO_MON_B3_186"
		},
		{
			"GEO_MON_UP3_111",
			"GEO_MON_B3_111"
		},
		{
			"GEO_MON_UP3_113",
			"GEO_MON_B3_113"
		},
		{
			"GEO_MON_UP3_115",
			"GEO_MON_B3_115"
		},
		{
			"GEO_MON_UP3_125",
			"GEO_MON_B3_125"
		},
		{
			"GEO_SUB_UP1_KJA",
			"GEO_SUB_F3_KJA"
		},
		{
			"GEO_SUB_UP0_KJAT",
			"GEO_SUB_F0_KJG"
		},
		{
			"GEO_MON_UP3_122",
			"GEO_MON_B3_122"
		},
		{
			"GEO_MON_UP3_119",
			"GEO_MON_B3_119"
		},
		{
			"GEO_MON_UP3_116",
			"GEO_MON_B3_116"
		},
		{
			"GEO_MON_UP3_185",
			"GEO_MON_B3_185"
		},
		{
			"GEO_MON_UP3_144",
			"GEO_MON_B3_144"
		},
		{
			"GEO_MON_UP3_181",
			"GEO_MON_B3_181"
		},
		{
			"GEO_MON_UP3_120",
			"GEO_MON_B3_120"
		},
		{
			"GEO_MON_UP3_145",
			"GEO_MON_B3_145"
		},
		{
			"GEO_MON_B3_108",
			"GEO_MON_F1_EFM"
		},
		{
			"GEO_ACC_F0_LTT",
			"GEO_ACC_F1_LTT"
		},
		{
			"GEO_ACC_F0_TKT",
			"GEO_ACC_F1_TKT"
		},
		{
			"GEO_MON_UP3_130",
			"GEO_MON_B3_130"
		},
		{
			"GEO_ACC_F0_HDB",
			"GEO_ACC_F1_HDB"
		},
		{
			"GEO_MON_UP3_123",
			"GEO_MON_B3_123"
		},
		{
			"GEO_MON_UP3_137",
			"GEO_MON_B3_137"
		},
		{
			"GEO_MON_UP3_138",
			"GEO_MON_B3_138"
		},
		{
			"GEO_MON_UP3_140",
			"GEO_MON_B3_140"
		},
		{
			"GEO_MON_UP3_143",
			"GEO_MON_B3_143"
		},
		{
			"GEO_MON_UP3_146",
			"GEO_MON_B3_146"
		},
		{
			"GEO_MON_UP3_187",
			"GEO_MON_B3_187"
		},
		{
			"GEO_MON_UP3_191",
			"GEO_MON_B3_191"
		},
		{
			"GEO_MON_UP3_192",
			"GEO_MON_B3_192"
		},
		{
			"GEO_MON_UP3_194",
			"GEO_MON_B3_194"
		},
		{
			"GEO_SUB_UP0_MASK_183",
			"GEO_MON_B3_183"
		},
		{
			"GEO_SUB_UP0_MASK_184",
			"GEO_MON_B3_184"
		},
		{
			"GEO_SUB_UP0_GRL",
			"GEO_SUB_F0_GRL"
		},
		{
			"GEO_MON_UP3_128",
			"GEO_MON_B3_128"
		},
		{
			"GEO_MON_UP3_141",
			"GEO_MON_B3_141"
		},
		{
			"GEO_MON_UP3_142",
			"GEO_MON_B3_142"
		},
		{
			"GEO_MON_UP3_147",
			"GEO_MON_B3_147"
		},
		{
			"GEO_MON_UP3_199",
			"GEO_MON_B3_199"
		},
		{
			"GEO_MON_UP3_193",
			"GEO_MON_B3_193"
		},
		{
			"GEO_SUB_UP0_SBW",
			"GEO_SUB_F0_SBW"
		},
		{
			"GEO_MAIN_UP8_GRN",
			"GEO_MAIN_F8_GRN"
		}
	};

	private static Dictionary<String, Int32> defaultWeaponTable = new Dictionary<String, Int32>
	{
		{
			"GEO_MON_B3_168",
			1
		},
		{
			"GEO_MON_B3_170",
			70
		},
		{
			"GEO_MON_B3_169",
			57
		},
		{
			"GEO_MON_B3_148",
			16
		},
		{
			"GEO_MON_B3_171",
			16
		},
		{
			"GEO_MON_B3_174",
			31
		},
		{
			"GEO_MON_B3_172",
			79
		},
		{
			"GEO_MON_B3_173",
			64
		},
		{
			"GEO_MON_B3_175",
			41
		},
		{
			"GEO_MON_B3_182",
			41
		},
		{
			"GEO_MON_B3_195",
			16
		},
		{
			"GEO_MON_B3_155",
			26
		}
	};

	private static Dictionary<String, Int32> defaultWeaponBoneTable = new Dictionary<String, Int32>
	{
		{
			"GEO_MON_B3_168",
			19
		},
		{
			"GEO_MON_B3_170",
			17
		},
		{
			"GEO_MON_B3_169",
			15
		},
		{
			"GEO_MON_B3_148",
			16
		},
		{
			"GEO_MON_B3_171",
			16
		},
		{
			"GEO_MON_B3_174",
			6
		},
		{
			"GEO_MON_B3_172",
			14
		},
		{
			"GEO_MON_B3_173",
			15
		},
		{
			"GEO_MON_B3_175",
			16
		},
		{
			"GEO_MON_B3_182",
			16
		},
		{
			"GEO_MON_B3_195",
			14
		},
		{
			"GEO_MON_B3_155",
			16
		}
	};

	public static List<String> garnetShortHairTable = new List<String>
	{
		"GEO_MAIN_F0_GRN",
		"GEO_MAIN_F1_GRN",
		"GEO_MAIN_B0_002",
		"GEO_MAIN_B0_003",
		"GEO_MAIN_B0_004",
		"GEO_MAIN_B0_005",
		"GEO_MON_B3_149",
		"GEO_MON_B3_169",
		"GEO_MAIN_B0_024",
		"GEO_MAIN_B0_025",
		"GEO_MAIN_B0_026",
		"GEO_MAIN_B0_027"
	};
}
