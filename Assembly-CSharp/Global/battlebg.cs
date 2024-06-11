using System;
using System.Linq;
using System.Collections.Generic;
using Memoria.Scripts;
using UnityEngine;
using Object = System.Object;

public class battlebg
{
	public static void nf_InitBattleBG(BBGINFO bbginfoPtr, GEOTEXHEADER tab)
	{
		battlebg.btlModel = FF9StateSystem.Battle.FF9Battle.map.btlBGPtr;
		battlebg.nf_BbgInfoPtr = bbginfoPtr;
		battlebg.nf_BbgNumber = (Int32)battlebg.nf_BbgInfoPtr.bbgnumber;
		battlebg.nf_SkyFixPositionFlag = 0;
		if (battlebg.nf_BbgInfoPtr.fog != 0)
		{
			battlebg.nf_SkyFixPositionFlag++;
		}
		battlebg.nf_BbgSkyRotation = (Int32)battlebg.nf_BbgInfoPtr.skyrotation;
		battlebg.nf_BbgTexAnm = (Int32)battlebg.nf_BbgInfoPtr.texanim;
		battlebg.nf_BbgTabAddress = tab;
		battlebg.nf_BbgUVChangeCount = (Int32)battlebg.nf_BbgInfoPtr.uvcount;
		battlebg.nf_SetBbgDispAttribute(15);
		battlebg.nf_BbgSkyAngle_Y = 0;
		battlebg.SetDefaultShader(battlebg.btlModel);
		battlebg.objAnimModel = FF9StateSystem.Battle.FF9Battle.map.btlBGObjAnim;
		battlebg.objAnimModel = new GameObject[(Int32)battlebg.nf_BbgInfoPtr.objanim];
		for (Int32 i = 0; i < (Int32)battlebg.nf_BbgInfoPtr.objanim; i++)
		{
			String text = String.Concat(new Object[]
			{
				"BBG_B",
				battlebg.nf_BbgNumber.ToString("D3"),
				"_OBJ",
				i + 1
			});
			battlebg.objAnimModel[i] = ModelFactory.CreateModel("BattleMap/BattleModel/battleMap_all/" + text + "/" + text, false);
			battlebg.SetDefaultShader(battlebg.objAnimModel[i]);
			if (battlebg.nf_BbgNumber == 171 && i == 1)
			{
				battlebg.SetMaterailShader(battlebg.objAnimModel[i], "PSX/BattleMap_Cystal");
			}
		}
		FF9StateSystem.Battle.FF9Battle.map.btlBGObjAnim = battlebg.objAnimModel;
		battlebg.nf_BbgTabAddress.InitTextureAnim();
		if (battlebg.nf_BbgTexAnm != 0)
		{
			for (Int32 j = 0; j < battlebg.nf_BbgTexAnm; j++)
			{
				battlebg.geoBGTexAnimPlay(battlebg.nf_BbgTabAddress, j);
			}
		}
	}

	private static void SetDefaultShader(GameObject go)
	{
		foreach (Object obj in go.transform)
		{
			Transform transform = (Transform)obj;
			if (battlebg.getBbgAttr(transform.name) == 0)
			{
				battlebg.SetMaterailShader(transform.gameObject, "PSX/BattleMap_Plus");
			}
			else if (battlebg.getBbgAttr(transform.name) == 2)
			{
				battlebg.SetMaterailShader(transform.gameObject, "PSX/BattleMap_Ground");
			}
			else if (battlebg.getBbgAttr(transform.name) == 4)
			{
				battlebg.SetMaterailShader(transform.gameObject, "PSX/BattleMap_Minus");
			}
			else if (battlebg.getBbgAttr(transform.name) == 8)
			{
				battlebg.SetMaterailShader(transform.gameObject, "PSX/BattleMap_Sky");
				Int32[] source = new Int32[]
				{
					1,
					2,
					12,
					109,
					119,
					68,
					69,
					60,
					53,
					49,
					90,
					20,
					77,
					125,
					141,
					95
				};
				if (!source.Contains(battlebg.nf_BbgNumber))
				{
					transform.localScale = new Vector3(1.01f, 1.01f, 1.01f);
				}
			}
		}
	}

	public static void SetMaterailShader(GameObject go, String shaderName)
	{
		MeshRenderer[] componentsInChildren = go.GetComponentsInChildren<MeshRenderer>();
		for (Int32 i = 0; i < (Int32)componentsInChildren.Length; i++)
		{
			Int32 num = 0;
			Material[] materials = componentsInChildren[i].materials;
			for (Int32 j = 0; j < (Int32)materials.Length; j++)
			{
				Material material = materials[j];
				String text = material.name.Replace("(Instance)", String.Empty);
				if (battlebg.nf_BbgNumber == 171 && num == 0 && shaderName.Contains("Minus"))
				{
					material.shader = ShadersLoader.Find("PSX/BattleMap_Moon");
				}
				else if ((battlebg.nf_BbgNumber == 92 && num == 3 && shaderName.Contains("Plus")) || (battlebg.nf_BbgNumber == 52 && num == 6 && shaderName.Contains("Plus")))
				{
					material.shader = ShadersLoader.Find("PSX/BattleMap_Plus_Abr_1_Off");
				}
				else if (text.Contains("a"))
				{
					material.shader = ShadersLoader.Find(shaderName + "_Abr_1");
				}
				else if (text.Contains("s"))
				{
					material.shader = ShadersLoader.Find(shaderName + "_Abr_0");
					material.SetColor("_Color", new Color32(Byte.MaxValue, Byte.MaxValue, Byte.MaxValue, 110));
				}
				else
				{
					material.shader = ShadersLoader.Find(shaderName);
					if (shaderName.CompareTo("PSX/BattleMap_Ground") == 0)
						material.SetInt("_ZWrite", 0); // DEBUG: Can't make the default value in "_ZWrite ("ZWrite", Int) = 0" works correctly for some reason
				}
				num++;
			}
		}
	}

	public static List<Material> GetShaders(Int32 bbgAttr)
	{
		List<Material> list = new List<Material>();
		foreach (Object obj in battlebg.btlModel.transform)
		{
			Transform transform = (Transform)obj;
			if (battlebg.getBbgAttr(transform.name) == bbgAttr)
			{
				MeshRenderer[] componentsInChildren = transform.gameObject.GetComponentsInChildren<MeshRenderer>();
				for (Int32 i = 0; i < (Int32)componentsInChildren.Length; i++)
				{
					Material[] materials = componentsInChildren[i].materials;
					for (Int32 j = 0; j < (Int32)materials.Length; j++)
					{
							list.Add(materials[j]);
					}
				}
			}
		}
		return list;
	}

	public static void nf_BattleBG()
	{
		Int32 num = 0;
		Int32 num2 = 0;
		Int32 num3 = 0;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		battlebg.nf_BbgTick++;
		if (battlebg.nf_BbgTexAnm != 0)
		{
			battlebg.geoBGTexAnimService(battlebg.nf_BbgTabAddress);
		}
		GameObject gameObject = battlebg.btlModel;
		for (;;)
		{
			foreach (Object obj in gameObject.transform)
			{
				Transform transform = (Transform)obj;
				if (battlebg.getBbgAttr(transform.name) == 8 && battlebg.nf_BbgSkyRotation != 0 && num == 0)
				{
					battlebg.nf_BbgSkyAngle_Y += battlebg.nf_BbgSkyRotation;
					Vector3 eulerAngles = transform.localRotation.eulerAngles;
					eulerAngles.y = (battlebg.nf_BbgSkyAngle_Y / 8f) / 4096f * 360f;
					transform.localRotation = Quaternion.Euler(eulerAngles);
					num++;
				}
				battlebg.setBGColor(transform.gameObject);
			}
			if (num2 == (Int32)battlebg.nf_BbgInfoPtr.objanim)
			{
				break;
			}
			num2++;
			gameObject = battlebg.objAnimModel[num2 - 1];
			Single num4 = Time.realtimeSinceStartup;
			Int16 bbgnumber = battlebg.nf_BbgInfoPtr.bbgnumber;
			switch (bbgnumber)
			{
			case 168:
				zero.x = 0f;
				zero.z = 0f;
				if (num2 == 1)
				{
					zero.y = (Single)((Int32)num4 / 16 & 4095);
				}
				else
				{
					zero.y = (Single)((Int32)num4 / 8 & 4095);
				}
				break;
			case 169:
			case 170:
				IL_158:
				switch (bbgnumber)
				{
				case 110:
				{
					Int32 num5 = (Int32)(num4 * 12f) & 4095;
					num5 = (Int32)(Mathf.Sin((Single)num5 / 4096f * 360f) * 4096f);
					num5 /= 64;
					zero.z = (Single)num5;
					zero.y = 512f;
					zero2.x = 1500f;
					zero2.y = -7000f;
					zero2.z = 3750f;
					break;
				}
				case 111:
					IL_16E:
					if (bbgnumber != 7)
					{
						if (bbgnumber != 68)
						{
							zero.y += 3f;
							zero2.x = 0f;
							zero2.y = -10f;
							zero2.z = 0f;
						}
						else
						{
							Int32 num5 = (Int32)(num4 * 26f) & 4095;
							num5 = (Int32)(Mathf.Sin((Single)num5 / 4096f * 360f) * 4096f);
							num5 /= 5;
							zero.z = (Single)num5;
							zero2.x = 1065f;
							zero2.y = -1345f;
							zero2.z = 3749f;
						}
					}
					else if (num2 == 1)
					{
						if ((battlebg.nf_BbgTick + 31 & 63) == 0)
						{
							battlebg.nf_b007a = (battlebg.rand() & 511);
						}
						zero.y = (Single)(battlebg.nf_b007a + (battlebg.rand() & 63));
					}
					else
					{
						if ((battlebg.nf_BbgTick & 63) == 0)
						{
							battlebg.nf_b007b = (battlebg.rand() & 1023);
						}
						zero.y = (Single)(battlebg.nf_b007b + (battlebg.rand() & 127));
					}
					break;
				case 112:
					switch (num2)
					{
					case 1:
					{
						Int32 num5 = (Int32)(num4 * 5f) & 4095;
						zero.z = (Single)(4095 - num5);
						num5 = ((Int32)(num4 * 3f) & 4095);
						zero.y = (Single)(4095 - num5);
						num5 = ((Int32)((num4 + 8f) * 22f) & 4095);
						num5 = (Int32)(Mathf.Sin((Single)num5 / 4096f * 360f) * 4096f);
						zero2.x = -2100f;
						zero2.y = (Single)(-250 + num5 / 45);
						zero2.z = -850f;
						break;
					}
					case 2:
					{
						zero.y = 0f;
						zero.z = 0f;
						Int32 num5 = (Int32)(num4 * 20f) & 4095;
						num5 = (Int32)(Mathf.Sin((Single)num5 / 4096f * 360f) * 4096f);
						zero2.x = 1725f;
						zero2.y = (Single)(-1500 + num5 / 64);
						zero2.z = -75f;
						break;
					}
					case 3:
					{
						Int32 num5 = (Int32)(num4 * 4f) & 4095;
						zero.z = (Single)num5;
						num5 = ((Int32)(num4 * 3f) & 4095);
						zero.y = (Single)num5;
						num5 = ((Int32)((num4 + 16f) * 21f) & 4095);
						num5 = (Int32)(Mathf.Sin((Single)num5 / 4096f * 360f) * 4096f);
						zero2.x = 1750f;
						zero2.y = (Single)(-775 + num5 / 50);
						zero2.z = 1025f;
						break;
					}
					}
					break;
				default:
					goto IL_16E;
				}
				break;
			case 171:
				num4 = (Single)(-(Single)(battlebg.nf_BbgTick * 2));
				if (num2 == 1)
				{
					Int32 num5 = (Int32)(num4 * -12f) & 4095;
					zero.z = (Single)num5;
					zero2.x = 0f;
					zero2.y = -2375f;
					zero2.z = 3750f;
				}
				else
				{
					Int32 num5 = (Int32)(num4 * -22f) & 4095;
					zero.z = 3584f;
					zero.y = (Single)num5;
					zero2.x = 0f;
					zero2.y = -2250f;
					zero2.z = 7625f;
					num3 = 1;
				}
				break;
			default:
				goto IL_158;
			}
			zero2.y *= -1f;
			zero.x = zero.x / 4096f * 360f;
			zero.y = zero.y / 4096f * 360f;
			zero.z = zero.z / 4096f * 360f;
			gameObject.transform.localPosition = zero2;
			if (num3 != 0)
			{
				gameObject.transform.localRotation = Quaternion.Inverse(Quaternion.Euler(zero));
			}
			else
			{
				gameObject.transform.localRotation = Quaternion.Euler(zero);
			}
		}
	}

	public static Int32 getBbgAttr(String name)
	{
		switch (name)
		{
		case "Group_0":
			return 0;
		case "Group_2":
			return 2;
		case "Group_4":
			return 4;
		case "Group_8":
			return 8;
		}
		return 0;
	}

	public static BBGINFO nf_GetBbgInfoPtr()
	{
		return FF9StateSystem.Battle.FF9Battle.map.btlBGInfoPtr;
	}

	public static void nf_SetBbgDispAttribute(Int32 attribute)
	{
		battlebg.nf_BbgDispAttribute = attribute;
	}

	public static void geoBGTexAnimPlay(GEOTEXHEADER tab, Int32 anum)
	{
		if (tab.geotex[anum].numframes == 0)
		{
			tab.TexAnimTextures[anum].wrapMode = TextureWrapMode.Repeat;
			if (battlebg.nf_BbgNumber == 57)
			{
				tab.extraTexAnimTrTextures[anum].wrapMode = TextureWrapMode.Repeat;
			}
		}
		GEOTEXANIMHEADER geotexanimheader = tab.geotex[anum];
		geotexanimheader.flags = (Byte)(geotexanimheader.flags | 1);
		tab.geotex[anum].frame = 0;
		tab.geotex[anum].lastframe = 4096;
	}

	public static void geoBGTexAnimService(GEOTEXHEADER texheaderptr)
	{
		UInt16 count = texheaderptr.count;
		GEOTEXANIMHEADER[] geotex = texheaderptr.geotex;
		for (Int32 i = 0; i < (Int32)count; i++)
		{
			GEOTEXANIMHEADER geotexanimheader = geotex[i];
			if ((geotexanimheader.flags & 1) != 0)
			{
				if (geotexanimheader.numframes != 0)
				{
					Int32 num = geotexanimheader.frame;
					Int16 lastframe = geotexanimheader.lastframe;
					Int16 num2 = (Int16)(num >> 12);
					if (num2 >= 0)
					{
						if (geotexanimheader.numframes <= 0)
						{
							goto IL_2C3;
						}
						if (num2 != lastframe)
						{
							for (Int32 j = 0; j < (Int32)geotexanimheader.count; j++)
							{
								Single x = (geotexanimheader.coords[(Int32)num2].x - geotexanimheader.target.x) / (Single)texheaderptr.TexAnimTextures[j].width;
								Single num3 = (geotexanimheader.coords[(Int32)num2].y - geotexanimheader.target.y) / (Single)texheaderptr.TexAnimTextures[j].height;
								texheaderptr.texAnimMaterials[i].SetTextureOffset("_MainTex", new Vector2(x, -num3));
							}
							geotexanimheader.lastframe = num2;
						}
						Int16 rate = geotexanimheader.rate;
						num += (Int32)rate;
					}
					else
					{
						num += 4096;
					}
					if (num >> 12 < (Int32)geotexanimheader.numframes)
					{
						geotexanimheader.frame = num;
					}
					else if (geotexanimheader.randrange > 0)
					{
						UInt32 num4 = battlebg.geoBGTexAnimRandom((UInt32)geotexanimheader.randmin, (UInt32)geotexanimheader.randrange);
						geotexanimheader.frame = (Int32)(-(Int32)((UInt64)((UInt64)num4 << 12)));
					}
					else if ((geotexanimheader.flags & 2) != 0)
					{
						Byte b = 3;
						GEOTEXANIMHEADER geotexanimheader2 = geotexanimheader;
						geotexanimheader2.flags = (Byte)(geotexanimheader2.flags & (Byte)(~b));
					}
					else
					{
						geotexanimheader.frame = 0;
					}
				}
				else if ((geotexanimheader.flags & 4) != 0)
				{
					Int32 num = geotexanimheader.frame;
					Int16 num2 = (Int16)(num >> 12);
					for (Int32 k = 0; k < (Int32)geotexanimheader.count; k++)
					{
						Int32 num5 = 0;
						Int32 height = texheaderptr.TexAnimTextures[k].height;
						Single num6 = (Single)num2 / (Single)height;
						if (battlebg.nf_BbgNumber == 69 && i == 3)
						{
							num6 *= -1f;
						}
						texheaderptr.texAnimMaterials[i].SetTextureOffset("_MainTex", new Vector2((Single)num5, -num6));
						if (battlebg.nf_BbgNumber == 57)
						{
							texheaderptr.extraTexAimMaterials[i].SetTextureOffset("_MainTex", new Vector2((Single)num5, -num6));
						}
						else if (battlebg.nf_BbgNumber == 71 && k == 0)
						{
							texheaderptr.extraTexAimMaterials[i].SetTextureOffset("_MainTex", new Vector2((Single)num5, -num6));
						}
					}
					Int16 rate = geotexanimheader.rate;
					geotexanimheader.frame += (Int32)rate;
				}
			}
			IL_2C3:;
		}
	}

	private static UInt32 geoBGTexAnimRandom(UInt32 randmin, UInt32 randrange)
	{
		return (UInt32)UnityEngine.Random.Range((Int32)randmin, (Int32)(randrange + 1u));
	}

	public static Int32 rand()
	{
		return UnityEngine.Random.Range(0, 32767);
	}

	public static Int32 nf_GetBbgIntensity()
	{
		return (Int32)battlebg.nf_BbgBrite;
	}

	public static void nf_SetBbgIntensity(Byte fade)
	{
		battlebg.nf_BbgBrite = fade;
	}

	private static void setBGColor(GameObject go)
	{
		Single num = (Single)battlebg.nf_BbgBrite;
		MeshRenderer[] componentsInChildren = go.GetComponentsInChildren<MeshRenderer>();
		for (Int32 i = 0; i < (Int32)componentsInChildren.Length; i++)
		{
			if (num == 0f)
			{
				componentsInChildren[i].enabled = false;
			}
			else
			{
				componentsInChildren[i].enabled = true;
				Material[] materials = componentsInChildren[i].materials;
				for (Int32 j = 0; j < (Int32)materials.Length; j++)
				{
					Material material = materials[j];
					material.SetFloat("_Intensity", num);
				}
			}
		}
	}

	public const Int32 BBG_DISP_ATTRIBUTE_PLUS = 1;

	public const Int32 BBG_DISP_ATTRIBUTE_GROUND = 2;

	public const Int32 BBG_DISP_ATTRIBUTE_MINUS = 4;

	public const Int32 BBG_DISP_ATTRIBUTE_SKY = 8;

	public const Int32 BBG_DISP_ATTRIBUTE_ALL = 15;

	public const Byte BBG_ATTR_PLUS = 0;

	public const Byte BBG_ATTR_GROUND = 2;

	public const Byte BBG_ATTR_MINUS = 4;

	public const Byte BBG_ATTR_SKY = 8;

	public static Int32 nf_BbgUVscroll;

	public static GEOTEXHEADER nf_BbgTabAddress;

	public static Int32 nf_BbgNumber;

	public static Int32 nf_BbgTexAnm;

	public static Int32 nf_BbgSkyRotation;

	public static Int32 nf_BbgUVChangeCount;

	public static BBGINFO nf_BbgInfoPtr;

	public static Int32 nf_BbgSkyAngle_Y;

	public static Int32 nf_BbgDispAttribute;

	public static Int32 nf_SkyFixPositionFlag;

	public static Int32 nf_BbgRainFlag;

	public static Int32 nf_b007a;

	public static Int32 nf_b007b;

	public static Int32 nf_BbgTick;

	private static GameObject btlModel;

	private static GameObject[] objAnimModel;

	private static Byte nf_BbgBrite = 128;
}
