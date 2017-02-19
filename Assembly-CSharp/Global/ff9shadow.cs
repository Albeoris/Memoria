using System;
using UnityEngine;

public class ff9shadow
{
	public static void FF9ShadowOnField(Int32 uid)
	{
		FF9Char ff9Char = FF9StateSystem.Common.FF9.FF9GetCharPtr(uid);
		ff9Char.attr = (UInt32)((UInt64)ff9Char.attr & 18446744073709551599UL);
		FF9StateSystem.Field.FF9Field.loc.map.shadowArray[uid].needUpdate = true;
	}

	public static void FF9ShadowOnBattle(Int32 uid)
	{
		FF9Char ff9Char = FF9StateSystem.Common.FF9.FF9GetCharPtr(uid);
		ff9Char.attr = (UInt32)((UInt64)ff9Char.attr & 18446744073709551599UL);
		GameObject[] shadowArray = FF9StateSystem.Battle.FF9Battle.map.shadowArray;
		MeshRenderer component = shadowArray[uid].GetComponent<MeshRenderer>();
		component.enabled = true;
	}

	public static void FF9ShadowOnWorld(Int32 uid)
	{
		global::Debug.LogError("Not implement FF9ShadowOnWorld");
	}

	public static void FF9ShadowOffField(Int32 uid)
	{
		FF9Char ff9Char = FF9StateSystem.Common.FF9.FF9GetCharPtr(uid);
		ff9Char.attr |= 16u;
		FF9StateSystem.Field.FF9Field.loc.map.shadowArray[uid].needUpdate = true;
	}

	public static void FF9ShadowOffBattle(Int32 uid)
	{
		FF9Char ff9Char = FF9StateSystem.Common.FF9.FF9GetCharPtr(uid);
		ff9Char.attr |= 16u;
		GameObject[] shadowArray = FF9StateSystem.Battle.FF9Battle.map.shadowArray;
		MeshRenderer component = shadowArray[uid].GetComponent<MeshRenderer>();
		component.enabled = false;
	}

	public static void FF9ShadowOffWorld(Int32 uid)
	{
		global::Debug.LogError("Not implement FF9ShadowOffWorld");
	}

	public static void FF9ShadowSetScaleField(Int32 uid, Int32 xScale, Int32 zScale)
	{
		FF9StateSystem.Field.FF9Field.loc.map.shadowArray[uid].xScale = (Single)(224 * xScale) * 1f / 16f;
		FF9StateSystem.Field.FF9Field.loc.map.shadowArray[uid].zScale = (Single)(192 * zScale) * 1f / 16f;
		FF9StateSystem.Field.FF9Field.loc.map.shadowArray[uid].needUpdate = true;
	}

	public static void FF9ShadowSetScaleBattle(Int32 uid, Int32 xScale, Int32 zScale)
	{
		GameObject[] shadowArray = FF9StateSystem.Battle.FF9Battle.map.shadowArray;
		Vector3 localScale = shadowArray[uid].transform.localScale;
		localScale.x = (Single)(224 * xScale) * 1f / 16f;
		localScale.z = (Single)(192 * zScale) * 1f / 16f;
		shadowArray[uid].transform.localScale = localScale;
	}

	public static void FF9ShadowSetScaleWorld(Int32 uid, Int32 xScale, Int32 zScale)
	{
		global::Debug.LogError("Not implement FF9ShadowOffWorld");
	}

	public static void FF9ShadowSetOffsetField(Int32 uid, Single xOffset, Single zOffset)
	{
		FF9StateSystem.Field.FF9Field.loc.map.shadowArray[uid].xOffset = xOffset;
		FF9StateSystem.Field.FF9Field.loc.map.shadowArray[uid].zOffset = zOffset;
	}

	public static void FF9ShadowSetOffsetBattle(Int32 uid, Single xOffset, Single zOffset)
	{
		global::Debug.LogError("Not implement FF9ShadowSetOffsetBattle");
	}

	public static void FF9ShadowSetOffsetWorld(Int32 uid, Single xOffset, Single zOffset)
	{
		global::Debug.LogError("Not implement FF9ShadowSetOffsetWorld");
	}

	public static void FF9ShadowLockYRotField(Int32 uid, Single rotY)
	{
		FF9Char ff9Char = FF9StateSystem.Common.FF9.FF9GetCharPtr(uid);
		ff9Char.attr |= 32u;
		FF9StateSystem.Field.FF9Field.loc.map.shadowArray[uid].yRot = rotY;
		FF9StateSystem.Field.FF9Field.loc.map.shadowArray[uid].needUpdate = true;
	}

	public static void FF9ShadowLockYRotBattle(Int32 uid, Single rotY)
	{
		global::Debug.LogError("Not implement FF9ShadowLockYRotBattle");
	}

	public static void FF9ShadowLockYRotWorld(Int32 uid, Single rotY)
	{
		global::Debug.LogError("Not implement FF9ShadowLockYRotWorld");
	}

	public static void FF9ShadowUnlockYRotField(Int32 uid)
	{
		FF9Char ff9Char = FF9StateSystem.Common.FF9.FF9GetCharPtr(uid);
		ff9Char.attr = (UInt32)((UInt64)ff9Char.attr & 18446744073709551583UL);
	}

	public static void FF9ShadowUnlockYRotBattle(Int32 uid)
	{
		ff9shadow.FF9ShadowUnlockYRotField(uid);
	}

	public static void FF9ShadowUnlockYRotWorld(Int32 uid)
	{
		ff9shadow.FF9ShadowUnlockYRotField(uid);
	}

	public static void FF9ShadowSetAmpField(Int32 uid, Int32 amp)
	{
		FF9StateFieldMap map = FF9StateSystem.Field.FF9Field.loc.map;
		FF9StateSystem.Field.FF9Field.loc.map.shadowArray[uid].amp = amp;
		FF9StateSystem.Field.FF9Field.loc.map.shadowArray[uid].needUpdate = true;
		FF9StateSystem.Field.FF9Field.loc.map.shadowArray[uid].needUpdateAmp = true;
	}

	public static void FF9ShadowSetAmpBattle(Int32 uid, Int32 amp)
	{
		GameObject[] shadowArray = FF9StateSystem.Battle.FF9Battle.map.shadowArray;
		MeshRenderer component = shadowArray[uid].GetComponent<MeshRenderer>();
		Byte b = (Byte)amp;
		component.material.SetColor("_Color", new Color32(b, b, b, b));
	}

	public static void FF9ShadowSetAmpWorld(Int32 uid, Int32 amp)
	{
	}

	public const Int32 FF9SHADOW_RADIUS = 16;

	public const Int32 FF9SHADOW_SCALE_X = 224;

	public const Int32 FF9SHADOW_SCALE_Z = 192;

	public const Int32 FF9SHADOW_ATTR_HIDE = 16;

	public const Int32 FF9SHADOW_ATTR_ROTFIXED = 32;
}
