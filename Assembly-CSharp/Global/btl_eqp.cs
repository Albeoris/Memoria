using System;
using FF9;
using UnityEngine;

public class btl_eqp
{
	public static void InitWeapon(PLAYER p, BTL_DATA btl)
	{
		btl.weapon = ff9weap._FF9Weapon_Data[(Int32)p.equip[0]];
		String text = FF9BattleDB.GEO.GetValue((Int32)btl.weapon.model_no);
		btl.weapon_geo = ModelFactory.CreateModel("BattleMap/BattleModel/battle_weapon/" + text + "/" + text, true);
		MeshRenderer[] componentsInChildren = btl.weapon_geo.GetComponentsInChildren<MeshRenderer>();
		btl.weaponMeshCount = (Int32)componentsInChildren.Length;
		btl.weaponRenderer = new Renderer[btl.weaponMeshCount];
		for (Int32 i = 0; i < btl.weaponMeshCount; i++)
		{
			btl.weaponRenderer[i] = componentsInChildren[i].GetComponent<Renderer>();
		}
		btl_util.SetBBGColor(btl.weapon_geo);
		switch (p.info.serial_no)
		{
		case 0:
			p.wep_bone = btl_eqp.b0_000_sk_r_sw;
			break;
		case 1:
			p.wep_bone = btl_eqp.b0_001_sk_r_sw;
			break;
		case 2:
			p.wep_bone = btl_eqp.b0_006_sk_r_wep;
			break;
		case 3:
		case 5:
			p.wep_bone = btl_eqp.b0_002_r_wep;
			break;
		case 4:
		case 6:
			p.wep_bone = btl_eqp.b0_003_r_wep;
			break;
		case 7:
		case 8:
			p.wep_bone = btl_eqp.b0_007_sk_r_wep;
			break;
		case 9:
			p.wep_bone = btl_eqp.b0_008_r_wep;
			break;
		case 10:
		case 11:
			p.wep_bone = btl_eqp.b0_009_wep;
			break;
		case 12:
			p.wep_bone = btl_eqp.b0_011_sk_l_wep;
			break;
		case 13:
			p.wep_bone = btl_eqp.b0_012_r_hand;
			break;
		case 14:
			p.wep_bone = btl_eqp.b0_013_l_sw;
			break;
		case 15:
			p.wep_bone = btl_eqp.b0_014_r_sw;
			break;
		case 16:
		case 17:
			p.wep_bone = btl_eqp.b0_015_r_sw;
			break;
		case 18:
			p.wep_bone = btl_eqp.b0_017_r_wep;
			break;
		}
		geo.geoAttach(btl.weapon_geo, btl.gameObject, (Int32)p.wep_bone);
	}

	public static void InitEquipPrivilegeAttrib(PLAYER p, BTL_DATA btl)
	{
		btl.def_attr.invalid = (btl.def_attr.absorb = (btl.def_attr.half = (btl.def_attr.weak = (btl.p_up_attr = 0))));
		for (Int32 i = 0; i < 5; i++)
		{
			if (p.equip[i] != 255)
			{
				btl_init.IncrementDefAttr(btl.def_attr, ff9equip._FF9EquipBonus_Data[(Int32)ff9item._FF9Item_Data[(Int32)p.equip[i]].bonus].def_attr);
				btl.p_up_attr = (Byte)(btl.p_up_attr | ff9equip._FF9EquipBonus_Data[(Int32)ff9item._FF9Item_Data[(Int32)p.equip[i]].bonus].p_up_attr);
			}
		}
	}

	private static Byte b0_000_sk_r_sw = 13;

	private static Byte b0_001_sk_r_sw = 13;

	private static Byte b0_006_sk_r_wep = 16;

	private static Byte b0_002_r_wep = 15;

	private static Byte b0_003_r_wep = 15;

	private static Byte b0_007_sk_r_wep = 16;

	private static Byte b0_008_r_wep = 14;

	private static Byte b0_009_wep = 15;

	private static Byte b0_011_sk_l_wep = 6;

	private static Byte b0_012_r_hand = 16;

	private static Byte b0_013_l_sw = 25;

	private static Byte b0_014_r_sw = 6;

	private static Byte b0_015_r_sw = 14;

	private static Byte b0_017_r_wep = 16;
}
