using System;
using UnityEngine;
using Memoria.Data;

namespace FF9
{
	public class geo
	{
		public static void geoScaleSetXYZ(GameObject go, Int32 sizex, Int32 sizey, Int32 sizez)
		{
			Vector3 localScale = go.transform.localScale;
			localScale.x = (Single)sizex / 4096f;
			localScale.y = (Single)sizey / 4096f;
			localScale.z = (Single)sizez / 4096f;
			if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
			{
				localScale.x *= -1f;
				localScale.y *= -1f;
			}
			else if (PersistenSingleton<EventEngine>.Instance.gMode == 3)
			{
				localScale.x *= 0.00390625f;
				localScale.y *= 0.00390625f;
				localScale.z *= 0.00390625f;
			}
			go.transform.localScale = localScale;
		}

		public static void geoScaleSetXYZ(BTL_DATA btl, Int32 sizex, Int32 sizey, Int32 sizez, Boolean updateShadow = false, Boolean bypassMini = false)
		{
			btl.flags = (UInt16)(btl.flags | geo.GEO_FLAGS_SCALE);
			btl.geo_scale_x = sizex;
			btl.geo_scale_y = sizey;
			btl.geo_scale_z = sizez;
			Single ssizex = (Single)sizex / 4096f;
			Single ssizey = (Single)sizey / 4096f;
			Single ssizez = (Single)sizez / 4096f;
			if (btl_stat.CheckStatus(btl, BattleStatus.Mini))
			{
				if (bypassMini)
				{
					btl.geo_scale_x *= 2;
					btl.geo_scale_y *= 2;
					btl.geo_scale_z *= 2;
				}
				else
				{
					ssizex *= 0.5f;
					ssizey *= 0.5f;
					ssizez *= 0.5f;
				}
			}
			Vector3 localScale = btl.gameObject.transform.localScale;
			localScale.x = ssizex;
			localScale.y = ssizey;
			localScale.z = ssizez;
			btl.gameObject.transform.localScale = localScale;
			if (btl.bi.player != 0)
			{
				Vector3 localScaleOriginal = btl.originalGo.transform.localScale;
				localScaleOriginal.x = ssizex;
				localScaleOriginal.y = ssizey;
				localScaleOriginal.z = ssizez;
				btl.originalGo.transform.localScale = localScaleOriginal;
				if (btl.tranceGo != null)
				{
					Vector3 localScaleTrance = btl.tranceGo.transform.localScale;
					localScaleTrance.x = ssizex;
					localScaleTrance.y = ssizey;
					localScaleTrance.z = ssizez;
					btl.tranceGo.transform.localScale = localScaleTrance;
				}
			}
			if (updateShadow)
				btlshadow.FF9ShadowSetScaleBattle(btl_util.GetFF9CharNo(btl), (Byte)Math.Round(btl.shadow_x * ssizex), (Byte)Math.Round(btl.shadow_z *ssizez));
		}

		public static void geoScaleSet(BTL_DATA btl, Int32 size, Boolean updateShadow = false, Boolean bypassMini = false)
		{
			geoScaleSetXYZ(btl, size, size, size, updateShadow, bypassMini);
		}

		public static void geoScaleReset(BTL_DATA btl, Boolean updateShadow = false, Boolean bypassMini = false)
		{
			geoScaleSetXYZ(btl, 4096, 4096, 4096, updateShadow, bypassMini);
		}

		public static void geoScaleUpdate(BTL_DATA btl, Boolean updateShadow = false)
		{
			geoScaleSetXYZ(btl, btl.geo_scale_x, btl.geo_scale_y, btl.geo_scale_z, updateShadow);
		}

		public static Int16 geoMeshChkFlags(BTL_DATA btl, Int32 mesh)
		{
			return (Int16)((UInt64)btl.meshflags & (UInt64)(1L << (mesh & 31)));
		}

		public static Int16 geoWeaponMeshChkFlags(BTL_DATA btl, Int32 mesh)
		{
			return (Int16)((UInt64)btl.weaponMeshFlags & (UInt64)(1L << (mesh & 31)));
		}

		public static void geoMeshHide(BTL_DATA btl, Int32 mesh)
		{
			btl.meshflags |= (UInt32)((UInt16)(1 << mesh));
		}

		public static void geoMeshShow(BTL_DATA btl, Int32 mesh)
		{
			btl.meshflags &= (UInt32)((UInt16)(~(UInt16)(1 << mesh)));
		}

		public static void geoWeaponMeshHide(BTL_DATA btl, Int32 mesh)
		{
			btl.weaponMeshFlags |= (UInt32)((UInt16)(1 << mesh));
		}

		public static void geoWeaponMeshShow(BTL_DATA btl, Int32 mesh)
		{
			btl.weaponMeshFlags &= (UInt32)((UInt16)(~(UInt16)(1 << mesh)));
		}

		public static void geoAttachOffset(GameObject sourceObject, Int32 x, Int32 y, Int32 z)
		{
			if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
			{
				FieldMapActor component = sourceObject.GetComponent<FieldMapActor>();
				component.GeoAttachOffset(new Vector3((Single)(-(Single)x), (Single)(-(Single)y), (Single)(-(Single)z)));
			}
		}

		public static void geoAttach(GameObject sourceObject, GameObject targetObject, Int32 bone_index)
		{
			Transform childByName = targetObject.transform.GetChildByName("bone" + bone_index.ToString("D3"));
			if (PersistenSingleton<EventEngine>.Instance.gMode == 2 || FF9StateSystem.Battle.isDebug)
			{
				sourceObject.transform.parent = childByName;
				sourceObject.transform.localPosition = Vector3.zero;
				sourceObject.transform.localRotation = Quaternion.identity;
				sourceObject.transform.localScale = Vector3.one;
			}
			else if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
			{
				FieldMapActor component = sourceObject.GetComponent<FieldMapActor>();
				FieldMapActor component2 = targetObject.GetComponent<FieldMapActor>();
				if (component != (UnityEngine.Object)null && component2 != (UnityEngine.Object)null && childByName != (UnityEngine.Object)null)
				{
					component.GeoAttach(component2, childByName);
                    PosObj actor = component.actor;
                    actor.attatchTargetUid = (int)component2.actor.uid;
                    actor.attachTargetBoneIndex = bone_index;
                }
			}
			else if (PersistenSingleton<EventEngine>.Instance.gMode == 3)
			{
			}
		}

		public static void geoAttachInWorld(Obj sourceObj, Obj targetObj, Int32 bone_index)
		{
			Actor actor = (Actor)sourceObj;
			actor.objParent = targetObj;
		}

		public static void geoDetachInWorld(Obj obj)
		{
			WMActor wmActor = ((Actor)obj).wmActor;
			wmActor.transform.parent = wmActor.World.TranslatingObjectsGroup;
			Actor actor = (Actor)obj;
			actor.objParent = (Obj)null;
		}

		public static void geoAttachNoReset(GameObject sourceObject, GameObject targetObject, Int32 bone_index)
		{
			Transform childByName = targetObject.transform.GetChildByName("bone" + bone_index.ToString("D3"));
			sourceObject.transform.parent = childByName;
		}

		public static void geoDetach(GameObject sourceObject, Boolean restorePosAndScaling)
		{
			FieldMapActor component = sourceObject.GetComponent<FieldMapActor>();
			if (component != (UnityEngine.Object)null)
			{
				component.GeoDetach(restorePosAndScaling);
			}
			else
			{
				global::Debug.Log("geoDetach() : Cannot get FieldMapActor component.");
			}
		}

		public static UInt16 GEO_FLAGS_LOOK = 1;

		public static UInt16 GEO_FLAGS_SCALE = 2;

		public static UInt16 GEO_FLAGS_TRANSP = 4;

		public static UInt16 GEO_FLAGS_SLICE = 8;

		public static UInt16 GEO_FLAGS_CLIP = 16;

		public static UInt16 GEO_FLAGS_RENDER = 32;
	}
}
