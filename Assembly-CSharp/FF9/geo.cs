using System;
using UnityEngine;

namespace FF9
{
	public class geo
	{
		public static void geoScaleSet(BTL_DATA btl, Int32 size)
		{
			btl.flags = (UInt16)(btl.flags | geo.GEO_FLAGS_SCALE);
			Vector3 localScale = btl.gameObject.transform.localScale;
			localScale.x = (localScale.y = (localScale.z = (Single)size / 4096f));
			btl.gameObject.transform.localScale = localScale;
		}

		public static void geoScaleSetXYZ(BTL_DATA btl, Int32 sizex, Int32 sizey, Int32 sizez)
		{
			btl.flags = (UInt16)(btl.flags | geo.GEO_FLAGS_SCALE);
			Vector3 localScale = btl.gameObject.transform.localScale;
			localScale.x = (Single)sizex / 4096f;
			localScale.y = (Single)sizey / 4096f;
			localScale.z = (Single)sizez / 4096f;
			btl.gameObject.transform.localScale = localScale;
		}

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

		public static void geoScaleReset(BTL_DATA btl)
		{
			btl.flags = (UInt16)(btl.flags & (UInt16)(~geo.GEO_FLAGS_SCALE));
			Vector3 localScale = btl.gameObject.transform.localScale;
			localScale.x = (localScale.y = (localScale.z = 1f));
			btl.gameObject.transform.localScale = localScale;
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
