using System;
using FF9;
using UnityEngine;
using Memoria.Data;
using System.Linq;

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
        InitOffSetWeapon(btl);
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

    public static void InitOffSetWeapon(BTL_DATA btl)
    {
        if (btl.bi.player != 0)
        {
            Single[] WeaponOffset = btl_mot.BattleParameterList[FF9StateSystem.Common.FF9.player[(CharacterId)btl.bi.slot_no].info.serial_no].WeaponOffset;

            if (WeaponOffset.Sum() != 0) // Don't edit values if all values are 0
            {
                btl.weapon_geo.transform.localPosition = new Vector3(WeaponOffset[0], WeaponOffset[1], WeaponOffset[2]);
                btl.weapon_geo.transform.localRotation = Quaternion.Euler(WeaponOffset[3], WeaponOffset[4], WeaponOffset[5]);
            }
        }
    }
}
