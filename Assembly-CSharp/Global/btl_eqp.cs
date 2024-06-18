using System;
using FF9;
using UnityEngine;
using Memoria.Data;
using Memoria.Prime;

public class btl_eqp
{
	public static void InitWeapon(PLAYER p, BTL_DATA btl)
	{
		btl.weapon = ff9item.GetItemWeapon(p.equip[0]);
		p.wep_bone = btl_mot.BattleParameterList[p.info.serial_no].WeaponBone;
		if (btl.weapon.ModelId != UInt16.MaxValue)
		{
			String modelName = FF9BattleDB.GEO.GetValue(btl.weapon.ModelId);
			btl.weapon_geo = ModelFactory.CreateModel("BattleMap/BattleModel/battle_weapon/" + modelName + "/" + modelName, true);
		}
		else
		{
			btl.weapon_geo = new GameObject("Dummy weapon");
		}
		MeshRenderer[] componentsInChildren = btl.weapon_geo.GetComponentsInChildren<MeshRenderer>();
		btl.weaponMeshCount = componentsInChildren.Length;
		btl.weaponRenderer = new Renderer[btl.weaponMeshCount];
		for (Int32 i = 0; i < btl.weaponMeshCount; i++)
		{
			btl.weaponRenderer[i] = componentsInChildren[i].GetComponent<Renderer>();
			if (btl.weapon.CustomTexture != null && btl.weapon.CustomTexture.Length > i && !String.IsNullOrEmpty(btl.weapon.CustomTexture[i]))
				btl.weaponRenderer[i].material.mainTexture = AssetManager.Load<Texture2D>(btl.weapon.CustomTexture[i], false);
		}
		btl_util.SetBBGColor(btl.weapon_geo);
		if (btl.is_monster_transform)
			geo.geoAttach(btl.weapon_geo, btl.originalGo, p.wep_bone);
		else
			geo.geoAttach(btl.weapon_geo, btl.gameObject, p.wep_bone);

        Single[] WeaponOffsetPos = btl_mot.BattleParameterList[p.info.serial_no].WeaponOffsetPos;
        Single[] WeaponOffsetRot = btl_mot.BattleParameterList[p.info.serial_no].WeaponOffsetRot;

        if (WeaponOffsetPos.Length == 3 && WeaponOffsetRot.Length == 3)
        {
            Log.Message("WeaponOffsetRot[0] = " + WeaponOffsetRot[0]);
            Log.Message("WeaponOffsetRot[1] = " + WeaponOffsetRot[1]);
            Log.Message("WeaponOffsetRot[2] = " + WeaponOffsetRot[2]);
            btl.weapon_geo.transform.localPosition = new Vector3(WeaponOffsetPos[0], WeaponOffsetPos[1], WeaponOffsetPos[2]);
            btl.weapon_geo.transform.localRotation = Quaternion.Euler(WeaponOffsetRot[0], WeaponOffsetRot[1], WeaponOffsetRot[2]);
            Log.Message("btl.weapon_geo.transform.localPosition = " + btl.weapon_geo.transform.localPosition.ToString("F8"));
            Log.Message("btl.weapon_geo.transform.localRotation = " + btl.weapon_geo.transform.localRotation.ToString("F8"));
        }
    }

    public static void InitEquipPrivilegeAttrib(PLAYER p, BTL_DATA btl)
	{
		btl.def_attr.invalid = (btl.def_attr.absorb = (btl.def_attr.half = (btl.def_attr.weak = (btl.p_up_attr = 0))));
		for (Int32 i = 0; i < 5; i++)
		{
			if (p.equip[i] != RegularItem.NoItem)
			{
				btl_init.IncrementDefAttr(btl.def_attr, ff9equip.ItemStatsData[ff9item._FF9Item_Data[p.equip[i]].bonus].def_attr);
				btl.p_up_attr = (Byte)(btl.p_up_attr | ff9equip.ItemStatsData[ff9item._FF9Item_Data[p.equip[i]].bonus].p_up_attr);
			}
		}
	}
}
