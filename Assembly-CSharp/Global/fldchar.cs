using System;
using System.Collections.Generic;
using UnityEngine;

public class fldchar
{
	private static void geoSlice(GameObject gObj, Int32 arg2)
	{
		Renderer[] componentsInChildren = gObj.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			Renderer renderer = array[i];
			renderer.material.SetInt("_Slice", arg2);
		}
	}

	public static void FF9FieldCharDispatch(Int32 uid, Int32 Parm, Int32 Arg1, Int32 Arg2, Int32 Arg3)
	{
		FF9FieldCharState ff9FieldCharState = FF9StateSystem.Field.FF9Field.loc.map.charStateArray[uid];
		FF9Char ff9Char = FF9StateSystem.Common.FF9.charArray[uid];
		switch (Parm)
		{
		case 0:
			if ((Arg1 & 255) == 255)
			{
				ff9Char.attr = (UInt32)((UInt64)ff9Char.attr & 18446744073709486079UL);
			}
			else
			{
				if (Arg1 != (Int32)ff9FieldCharState.arate)
				{
					ff9Char.attr |= 262144u;
				}
				ff9Char.attr |= 65536u;
			}
			ff9FieldCharState.arate = (SByte)Arg1;
			break;
		case 4:
			if (Arg1 != 0)
			{
				fldchar.geoSlice(ff9Char.geo, Arg2);
				ff9Char.attr |= 1048576u;
			}
			else
			{
				ff9Char.attr = (UInt32)((UInt64)ff9Char.attr & 18446744073708503039UL);
			}
			break;
		case 8:
		case 9:
		case 10:
		case 11:
		{
			FF9FieldCharMirror ff9FieldCharMirror = ff9FieldCharState.mirror;
			FF9Char ff9Char2;
			if (ff9FieldCharMirror == null)
			{
				ff9FieldCharMirror = (ff9FieldCharState.mirror = new FF9FieldCharMirror());
				ff9Char2 = (ff9FieldCharMirror.chr = ff9Char);
				ff9Char2.attr = 0u;
				ff9FieldCharMirror.geo = ModelFactory.CreateModel(FF9BattleDB.GEO.GetValue((Int32)ff9Char.evt.model), false);
				ff9FieldCharMirror.geo.name = ff9Char.geo.name + "_mirror";
				Shader shader = Shader.Find("PSX/FieldMapActor");
				Renderer[] componentsInChildren = ff9FieldCharMirror.geo.GetComponentsInChildren<Renderer>();
				Renderer[] array = componentsInChildren;
				for (Int32 i = 0; i < (Int32)array.Length; i++)
				{
					Renderer renderer = array[i];
					renderer.material.shader = shader;
					renderer.material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f));
					if (FF9StateSystem.Common.FF9.fldMapNo == 2653 || FF9StateSystem.Common.FF9.fldMapNo == 2654)
					{
						renderer.material.renderQueue = 2000;
					}
				}
				ff9FieldCharMirror.geo.transform.SetParent(ff9Char.geo.transform.transform.parent);
				ff9FieldCharMirror.evt = ff9Char.evt;
				ff9FieldCharMirror.geo.transform.localScale = new Vector3(-1f, 1f, 1f);
				ff9FieldCharMirror.geo.transform.localEulerAngles = Vector3.zero;
				ff9FieldCharMirror.geo.transform.localPosition = Vector3.zero;
				ff9FieldCharMirror.actor = ff9FieldCharMirror.geo.AddComponent<FieldMapActor>();
				ff9FieldCharMirror.actor.meshRenderer = ff9FieldCharMirror.geo.GetComponentsInChildren<Renderer>();
				ff9FieldCharMirror.parent = ff9Char;
				ff9FieldCharMirror.point = Vector3.zero;
				ff9FieldCharMirror.normal = Vector3.zero;
				ff9FieldCharMirror.clr[0] = 0;
			}
			ff9Char2 = ff9FieldCharMirror.chr;
			if (FF9Char.ff9charptr_attr_test(ff9FieldCharMirror.chr, 1) == 0)
			{
				ff9Char2.evt = ff9Char.evt;
				FF9Char.ff9charptr_attr_set(ff9FieldCharMirror.chr, 33554433);
			}
			switch (Parm)
			{
			case 8:
				if (Arg1 != 0)
				{
					FF9Char.ff9charptr_attr_set(ff9FieldCharMirror.chr, 16777216);
					ff9FieldCharMirror.geo.SetActive(true);
				}
				else
				{
					FF9Char.ff9charptr_attr_clear(ff9FieldCharMirror.chr, 16777216);
					ff9FieldCharMirror.geo.SetActive(false);
				}
				break;
			case 9:
				ff9FieldCharMirror.point = new Vector3((Single)Arg1, (Single)Arg2, (Single)Arg3);
				break;
			case 10:
				ff9FieldCharMirror.normal = new Vector3((Single)(Arg1 >> 12), (Single)(Arg2 >> 12), (Single)(Arg3 >> 12));
				break;
			case 11:
				ff9FieldCharMirror.clr[0] = (Byte)Arg1;
				ff9FieldCharMirror.clr[1] = (Byte)Arg2;
				ff9FieldCharMirror.clr[2] = (Byte)Arg3;
				ff9FieldCharMirror.clr[3] = 2;
				break;
			}
			break;
		}
		case 16:
		case 17:
		case 18:
		case 19:
		{
			FF9FieldCharSound ff9FieldCharSound;
			if ((ff9FieldCharSound = FF9Snd.ff9fieldSoundGetChar(ff9Char, Arg1, Arg2)) == null && Parm != 19)
			{
				ff9FieldCharSound = FF9Snd.ff9fieldSoundNewChar(ff9Char, Arg1, Arg2);
			}
			switch (Parm)
			{
			case 16:
				ff9FieldCharSound.sndEffectID[0] = (UInt16)Arg3;
				break;
			case 17:
				ff9FieldCharSound.sndEffectID[1] = (UInt16)Arg3;
				break;
			case 18:
				ff9FieldCharSound.pitchRand = (SByte)((Arg3 == 0) ? 0 : 1);
				break;
			case 19:
				FF9Snd.ff9fieldSoundDeleteChar(ff9Char, Arg1, Arg2);
				break;
			}
			break;
		}
		}
	}

	public static void ff9fieldCharEffectService()
	{
		Dictionary<Int32, FF9Char> charArray = FF9StateSystem.Common.FF9.charArray;
		Dictionary<Int32, FF9FieldCharState> charStateArray = FF9StateSystem.Field.FF9Field.loc.map.charStateArray;
		if (charArray == null)
		{
			return;
		}
		List<Int32> list = new List<Int32>(charArray.Keys);
		foreach (Int32 key in list)
		{
			if (charArray.ContainsKey(key))
			{
				FF9Char ff9Char = charArray[key];
				if (charStateArray.ContainsKey(key))
				{
					FF9FieldCharState ff9FieldCharState = charStateArray[key];
					UInt32 num = (UInt32)(ff9Char.attr & 65536u);
					Int32 num2 = (Int32)((UInt64)(ff9FieldCharState.attr & 65536u) ^ (UInt64)((Int64)num));
					Int32 num3 = FF9Char.ff9charptr_attr_test(ff9Char, 262144);
					Int32 num4 = FF9Char.ff9charptr_attr_test(ff9Char, 131072);
					Boolean flag = num2 != 0 || num3 != 0 || num4 != 0;
					if (ff9Char.geo)
					{
						Renderer[] componentsInChildren = ff9Char.geo.GetComponentsInChildren<Renderer>();
						if (flag)
						{
							if (FF9Char.ff9charptr_attr_test(ff9Char, 65536) != 0)
							{
								Shader shader = Shader.Find("PSX/Actor_Abr_" + ff9FieldCharState.arate);
								Renderer[] array = componentsInChildren;
								for (Int32 i = 0; i < (Int32)array.Length; i++)
								{
									Renderer renderer = array[i];
									Material[] materials = renderer.materials;
									for (Int32 j = 0; j < (Int32)materials.Length; j++)
									{
										Material material = materials[j];
										material.shader = shader;
									}
								}
							}
							else
							{
								Shader shader2 = Shader.Find("PSX/FieldMapActor");
								Renderer[] array2 = componentsInChildren;
								for (Int32 k = 0; k < (Int32)array2.Length; k++)
								{
									Renderer renderer2 = array2[k];
									renderer2.material.shader = shader2;
								}
							}
							if (FF9Char.ff9charptr_attr_test(ff9Char, 131072) != 0)
							{
								FF9Char.ff9charptr_attr_clear(ff9Char, 131072);
							}
							FF9Char.ff9charptr_attr_clear(ff9Char, 262144);
							if (num2 != 0 || num3 != 0)
							{
								FF9Char.ff9charptr_attr_set(ff9Char, 131072);
							}
							ff9FieldCharState.attr = (ff9FieldCharState.attr & 0xFFFEFFFF) | num;
						}
						FF9FieldCharColor ff9FieldCharColor = ff9FieldCharState.clr[0];
						if (ff9FieldCharColor.active && !FF9StateSystem.Field.isDebugWalkMesh && (Int32)componentsInChildren.Length > 0)
						{
							Renderer[] array3 = componentsInChildren;
							for (Int32 l = 0; l < (Int32)array3.Length; l++)
							{
								Renderer renderer3 = array3[l];
								Material[] materials2 = renderer3.materials;
								for (Int32 m = 0; m < (Int32)materials2.Length; m++)
								{
									Material material2 = materials2[m];
									Color32 c = renderer3.material.GetColor("_Color");
									c.r = ff9FieldCharColor.r;
									c.g = ff9FieldCharColor.g;
									c.b = ff9FieldCharColor.b;
									material2.SetColor("_Color", c);
									ff9FieldCharState.clr[0] = ff9FieldCharState.clr[1];
								}
							}
							ff9FieldCharState.clr[1] = default(FF9FieldCharColor);
						}
						FF9FieldCharMirror mirror = ff9FieldCharState.mirror;
						if (mirror != null && FF9Char.ff9charptr_attr_test(mirror.chr, 16777216) != 0)
						{
							FF9Char chr = mirror.chr;
							Vector3 vector = mirror.point + ff9Char.geo.transform.localPosition;
							vector += mirror.point;
							vector.y *= -1f;
							mirror.geo.transform.position = vector;
							mirror.geo.transform.eulerAngles = ff9Char.geo.transform.eulerAngles;
							Animation component = ff9Char.geo.GetComponent<Animation>();
							Animation component2 = mirror.geo.GetComponent<Animation>();
							String text = FF9DBAll.AnimationDB.GetValue((Int32)ff9Char.evt.anim);
							if (component2.GetClip(text) == (UnityEngine.Object)null)
							{
								AnimationClip clip = component.GetClip(text);
								component2.AddClip(clip, text);
							}
							component2.Play(text);
							component2[text].speed = 0f;
							component2[text].time = component[text].time;
							component2.Sample();
							Renderer[] componentsInChildren2 = mirror.geo.GetComponentsInChildren<Renderer>();
							if (mirror.clr[3] != 0)
							{
								Color32 c2 = default(Color32);
								c2.r = mirror.clr[0];
								c2.g = mirror.clr[1];
								c2.b = mirror.clr[2];
								Renderer[] array4 = componentsInChildren2;
								for (Int32 n = 0; n < (Int32)array4.Length; n++)
								{
									Renderer renderer4 = array4[n];
									renderer4.material.SetColor("_Color", c2);
								}
								Byte[] clr = mirror.clr;
								Int32 num5 = 3;
								clr[num5] = (Byte)(clr[num5] - 1);
							}
						}
					}
				}
			}
		}
	}

	private void geoMirror(GameObject mirror, GameObject charPtr, ref Vector3 normal, ref Vector3 point)
	{
	}

	public const Int32 FLDCHAR_PARMTYPE_ALPHA = 0;

	public const Int32 FLDCHAR_PARMTYPE_SLICE = 4;

	public const Int32 FLDCHAR_PARMTYPE_MIRROR_SET = 8;

	public const Int32 FLDCHAR_PARMTYPE_MIRROR_POINT = 9;

	public const Int32 FLDCHAR_PARMTYPE_MIRROR_NORMAL = 10;

	public const Int32 FLDCHAR_PARMTYPE_MIRROR_COLOR = 11;

	public const Int32 FLDCHAR_PARMTYPE_SOUND_BINDZERO = 16;

	public const Int32 FLDCHAR_PARMTYPE_SOUND_BINDONE = 17;

	public const Int32 FLDCHAR_PARMTYPE_SOUND_PITCHRAND = 18;

	public const Int32 FLDCHAR_PARMTYPE_SOUND_UNBIND = 19;

	public const Int32 FLDCHAR_PARM_ALPHA_50P50 = 0;

	public const Int32 FLDCHAR_PARM_ALPHA_100P100 = 1;

	public const Int32 FLDCHAR_PARM_ALPHA_100M100 = 2;

	public const Int32 FLDCHAR_PARM_ALPHA_100P25 = 3;

	public const Int32 FLDCHAR_PARM_ALPHA_OFF = 255;

	public const Int32 FLDCHAR_PARM_SLICE_ON = 1;

	public const Int32 FLDCHAR_PARM_SLICE_OFF = 0;

	public const Int32 FLDCHAR_PARM_MIRROR_ON = 1;

	public const Int32 FLDCHAR_PARM_MIRROR_OFF = 0;

	public const Int32 FLDCHAR_PARM_SFXBIND_PITCHRAND_ON = 1;

	public const Int32 FLDCHAR_PARM_SFXBIND_PITCHRAND_OFF = 0;
}
