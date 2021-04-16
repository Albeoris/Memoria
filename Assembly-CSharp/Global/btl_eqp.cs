using System;
using FF9;
using UnityEngine;

public class btl_eqp
{
	public static void InitWeapon(PLAYER p, BTL_DATA btl)
	{
		btl.weapon = ff9weap.WeaponData[(Int32)p.equip[0]];
		String text = FF9BattleDB.GEO.GetValue((Int32)btl.weapon.ModelId);
		btl.weapon_geo = ModelFactory.CreateModel("BattleMap/BattleModel/battle_weapon/" + text + "/" + text, true);
		MeshRenderer[] componentsInChildren = btl.weapon_geo.GetComponentsInChildren<MeshRenderer>();
		btl.weaponMeshCount = (Int32)componentsInChildren.Length;
		btl.weaponRenderer = new Renderer[btl.weaponMeshCount];
		for (Int32 i = 0; i < btl.weaponMeshCount; i++)
		{
			btl.weaponRenderer[i] = componentsInChildren[i].GetComponent<Renderer>();
		}
		btl_util.SetBBGColor(btl.weapon_geo);
		p.wep_bone = BattlePlayerCharacter.PlayerWeaponToBoneName[p.info.serial_no];
		geo.geoAttach(btl.weapon_geo, btl.gameObject, (Int32)p.wep_bone);
	}

	public static void InitEquipPrivilegeAttrib(PLAYER p, BTL_DATA btl)
	{
		btl.def_attr.invalid = (btl.def_attr.absorb = (btl.def_attr.half = (btl.def_attr.weak = (btl.p_up_attr = 0))));
		for (Int32 i = 0; i < 5; i++)
		{
			if (p.equip[i] != 255)
			{
				btl_init.IncrementDefAttr(btl.def_attr, ff9equip.ItemStatsData[(Int32)ff9item._FF9Item_Data[(Int32)p.equip[i]].bonus].def_attr);
				btl.p_up_attr = (Byte)(btl.p_up_attr | ff9equip.ItemStatsData[(Int32)ff9item._FF9Item_Data[(Int32)p.equip[i]].bonus].p_up_attr);
			}
		}
	}
}
