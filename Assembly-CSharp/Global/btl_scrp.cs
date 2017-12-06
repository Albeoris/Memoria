using System;
using System.Linq;
using FF9;
using Memoria;
using Memoria.Data;

public class btl_scrp
{
	public static UInt16 GetBattleID(UInt32 list_no)
	{
		UInt16 num = 0;
		for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
		{
			switch (list_no)
			{
			case 0u:
				if (next.bi.player != 0)
				{
					num = (UInt16)(num | next.btl_id);
				}
				break;
			case 1u:
				if (next.bi.player == 0)
				{
					num = (UInt16)(num | next.btl_id);
				}
				break;
			case 2u:
				num = (UInt16)(num | next.btl_id);
				break;
			}
		}
		return num;
	}

	public static BattleUnit FindBattleUnit(UInt16 btl_id)
	{
	    return FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits().FirstOrDefault(u => u.Id == btl_id);
	}

	public static BattleUnit FindBattleUnitUnlimited(UInt16 btl_id)
	{
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		for (Int32 i = 0; i < 8; i++)
		{
			if (ff9Battle.btl_data[i].btl_id == btl_id)
			{
			    return new BattleUnit(ff9Battle.btl_data[i]);
			}
		}
		return null;
	}

	public static UInt32 GetCharacterData(BTL_DATA btl, UInt32 id)
	{
		UInt32 result = 16777215u;
		switch (id)
		{
		case 35u:
			result = btl.max.hp;
			break;
		case 36u:
			result = btl.cur.hp;
			break;
		case 37u:
			result = (UInt32)btl.max.mp;
			break;
		case 38u:
			result = (UInt32)btl.cur.mp;
			break;
		case 39u:
			result = (UInt32)btl.max.at;
			break;
		case 41u:
			if (btl.bi.player != 0)
			{
				result = btl_util.getPlayerPtr(btl).level;
			}
			else
			{
				result = btl_util.getEnemyTypePtr(btl).level;
			}
			break;
		case 42u:
			result = ((UInt32)btl.stat.invalid >> 24);
			break;
		case 43u:
			result = ((UInt32)btl.stat.invalid & 16777215u);
			break;
		case 44u:
			result = (UInt32)btl.stat.permanent >> 24;
			break;
		case 45u:
			result = ((UInt32)btl.stat.permanent & 16777215u);
			break;
		case 46u:
			result = (UInt32)btl.stat.cur >> 24;
			break;
		case 47u:
			result = ((UInt32)btl.stat.cur & 16777215u);
			break;
		case 48u:
			result = btl.def_attr.invalid;
			break;
		case 49u:
			result = btl.def_attr.absorb;
			break;
		case 50u:
			result = btl.def_attr.half;
			break;
		case 51u:
			result = btl.def_attr.weak;
			break;
		case 52u:
			result = btl.bi.target;
			break;
		case 53u:
			result = btl.bi.disappear;
			break;
		case 57u:
			result = (UInt32)btl.dms_geo_id;
			break;
		case 58u:
			result = btl.mesh_current;
			break;
		case 64u:
			result = btl.bi.row;
			break;
		case 65u:
			result = btl.bi.line_no;
			break;
		case 66u:
			result = btl_util.getPlayerPtr(btl).info.serial_no;
			break;
		case 67u:
			result = btl_util.getPlayerPtr(btl).category;
			break;
		case 68u:
			result = btl_util.getEnemyTypePtr(btl).category;
			break;
		case 69u:
			result = btl.bi.def_idle;
			break;
		case 70u:
			result = btl.bi.slot_no;
			break;
		case 72u:
			result = btl.elem.str;
			break;
		case 73u:
			result = btl.elem.mgc;
			break;
		case 74u:
			result = btl.defence.PhisicalDefence;
			break;
		case 75u:
			result = btl.defence.PhisicalEvade;
			break;
		case 76u:
			result = btl.defence.MagicalDefence;
			break;
		case 77u:
			result = btl.defence.MagicalEvade;
			break;
		}
		return result;
	}

	public static void SetCharacterData(BTL_DATA btl, UInt32 id, UInt32 val)
	{
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		switch (id)
		{
		case 32u:
			for (Int32 i = 0; i < btl.meshCount; i++)
			{
				btl.SetIsEnabledMeshRenderer(i, true);
			}
			btl.getShadow().SetActive(true);
			btl.SetIsEnabledWeaponRenderer(true);
			btl.SetIsEnabledBattleModelRenderer(true);
			btl_sys.AddCharacter(btl);
			break;
		case 33u:
			for (Int32 j = 0; j < btl.meshCount; j++)
			{
				btl.SetIsEnabledMeshRenderer(j, false);
			}
			btl.getShadow().SetActive(false);
			btl.SetIsEnabledWeaponRenderer(false);
			btl.SetIsEnabledBattleModelRenderer(false);
			btl_cmd.KillCommand2(btl);
			btl_sys.DelCharacter(btl);
			break;
		case 34u:
			if (btl_cmd.KillCommand2(btl))
			{
				btl.sel_mode = 0;
			}
			break;
		case 35u:
			btl.max.hp = (UInt16)val;
			break;
		case 36u:
			btl.cur.hp = (UInt16)val;
			break;
		case 37u:
			btl.max.mp = (Int16)val;
			break;
		case 38u:
			btl.cur.mp = (Int16)val;
			break;
		case 40u:
			btl.cur.at = (Int16)val;
			break;
		case 48u:
			btl.def_attr.invalid = (Byte)val;
			break;
		case 49u:
			btl.def_attr.absorb = (Byte)val;
			break;
		case 50u:
			btl.def_attr.half = (Byte)val;
			break;
		case 51u:
			btl.def_attr.weak = (Byte)val;
			break;
		case 52u:
			if (ff9Battle.btl_phase == 2)
			{
				btl.tar_mode = 0;
				btl.bi.target = 0;
				btl.bi.atb = 0;
				btl.bi.select = 0;
			}
			else
			{
				btl.tar_mode = (Byte)(2u + val);
			}
			break;
		case 53u:
			btl.SetDisappear((Byte)val);
			if (val == 0u || ff9Battle.btl_phase == 2)
			{
			}
			break;
		case 54u:
			btl.bi.shadow = (Byte)val;
			if (btl.bi.shadow != 0)
			{
				btl.getShadow().SetActive(true);
			}
			else
			{
				btl.getShadow().SetActive(false);
			}
			break;
		case 55u:
			geo.geoScaleSet(btl, (Int32)val);
			btlshadow.FF9ShadowSetScaleBattle(btl_util.GetFF9CharNo(btl), (Byte)(btl.shadow_x * val >> 12), (Byte)(btl.shadow_z * val >> 12));
			break;
		case 56u:
			geo.geoScaleReset(btl);
			btlshadow.FF9ShadowSetScaleBattle(btl_util.GetFF9CharNo(btl), btl.shadow_x, btl.shadow_z);
			break;
		case 59u:
			btl_mot.HideMesh(btl, (UInt16)val, false);
			btl.mesh_current = (UInt16)(btl.mesh_current | (UInt16)val);
			break;
		case 60u:
			btl_mot.ShowMesh(btl, (UInt16)val, false);
			btl.mesh_current = (UInt16)(btl.mesh_current & (UInt16)(~(UInt16)val));
			break;
		case 61u:
			GeoTexAnim.geoTexAnimPlay(btl.texanimptr, (Int32)val);
			break;
		case 62u:
			GeoTexAnim.geoTexAnimStop(btl.texanimptr, (Int32)val);
			break;
		case 63u:
			GeoTexAnim.geoTexAnimPlayOnce(btl.texanimptr, (Int32)val);
			break;
		case 69u:
			btl.bi.def_idle = (Byte)val;
			break;
		case 71u:
			btl_sys.SetBonus(btl_util.getEnemyTypePtr(btl));
			break;
		case 72u:
			btl.elem.str = (Byte)val;
			break;
		case 73u:
			btl.elem.mgc = (Byte)val;
			break;
		case 74u:
			btl.defence.PhisicalDefence = (Byte)val;
			break;
		case 75u:
			btl.defence.PhisicalEvade = (Byte)val;
			break;
		case 76u:
			btl.defence.MagicalDefence = (Byte)val;
			break;
		case 77u:
			btl.defence.MagicalEvade = (Byte)val;
			break;
		case 78u:
			btl.cur.at = btl.max.at;
			btl.sel_mode = 1;
			btl_cmd.SetCommand(btl.cmd[0], BattleCommandId.SummonEiko, 187u, (UInt16)val, 8u);
		        UIManager.Battle.FF9BMenu_EnableMenu(true);
			break;
		}
	}

	public static UInt32 GetBattleData(Int32 id)
	{
		UInt32 result = UInt32.MaxValue;
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		FF9StateGlobal ff = FF9StateSystem.Common.FF9;
		switch (id)
		{
		case 32:
			result = ff9Battle.btl_scene.Info.StartType;
			break;
		case 33:
			result = ff.party.gil;
			break;
		case 34:
			result = ff9Battle.btl_phase;
			break;
		case 35:
			result = ff9Battle.btl_seq;
			break;
		case 36:
			if (ff9Battle.cur_cmd != null && ff9Battle.cur_cmd.regist != null)
			{
				result = ff9Battle.cur_cmd.regist.btl_id;
			}
			break;
		case 37:
			if (ff9Battle.cur_cmd != null)
			{
				result = ff9Battle.cur_cmd.tar_id;
			}
			break;
		case 38:
			if (ff9Battle.cur_cmd != null && ff9Battle.cur_cmd.regist != null && ff9Battle.cur_cmd.regist.weapon != null)
			{
				result = ff9Battle.cur_cmd.regist.weapon.Ref.Elements;
			}
			break;
		case 39:
			if (ff9Battle.cur_cmd != null && ff9Battle.cur_cmd.regist != null)
			{
				if ((ff9Battle.cur_cmd.regist.sa[1] & 1u) != 0u) // MagElemNull
                    {
					result = 0u;
				}
				else
				{
					result = ff9Battle.cur_cmd.aa.Ref.Elements;
				}
			}
			break;
		case 40:
			result = ff9Battle.btl_load_status;
			break;
		case 41:
			result = battle.btl_bonus.exp;
			break;
		case 42:
			result = 0u;
			for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
			{
				UInt32 serialNumber = btl_util.getSerialNumber(next);
				if (serialNumber == 10u || serialNumber == 11u)
				{
					result = !btl_cmd.CheckSpecificCommand(next, BattleCommandId.SysLastPhoenix) ? 0u : 1u;
					break;
				}
			}
			break;
		}
		return result;
	}

	public static void SetBattleData(UInt32 id, Int32 val)
	{
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		FF9StateGlobal ff = FF9StateSystem.Common.FF9;
		switch (id)
		{
		case 32u:
			UIManager.Battle.FF9BMenu_EnableMenu(false);
			ff9Battle.btl_escape_key = 0;
			ff9Battle.cmd_status = (UInt16)(ff9Battle.cmd_status & -2);
			ff9Battle.btl_phase = 5;
			ff9Battle.btl_seq = 2;
			btl_cmd.KillAllCommand(ff9Battle);
			for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
			{
				next.bi.cmd_idle = 0;
			}
			break;
		case 33u:
			if (ff9Battle.btl_phase == 1)
			{
				ff.btl_result = (Byte)val;
				if (val == 1 && ff9Battle.btl_scene.Info.WinPose != 0)
				{
					ff9Battle.btl_phase = 5;
					ff9Battle.btl_seq = 4;
				}
				else
				{
					if (ff.btl_result == 1)
					{
						ff.btl_result = 2;
					}
					ff9Battle.btl_phase = 8;
					ff9Battle.btl_seq = 0;
				}
			}
			break;
		case 34u:
			if (ff9Battle.btl_phase == 1)
			{
				ff9Battle.btl_phase = 7;
				ff9Battle.btl_seq = 0;
				ff.btl_result = 3;
			}
			break;
		case 35u:
			if (ff9Battle.btl_phase == 1)
			{
				BTL_SCENE_INFO info = ff9Battle.btl_scene.Info;
				ff9Battle.btl_phase = 3;
				ff9Battle.btl_seq = 0;
				if (info.SpecialStart == 0 || info.BackAttack == 0)
				{
					info.StartType = 2;
				}
			}
			break;
		case 36u:
			if (ff9Battle.btl_phase == 1)
			{
				SFX.SetCamera(val);
			}
			break;
		case 37u:
		{
			FF9StateGlobal ff9StateGlobal = ff;
			ff9StateGlobal.btl_flag = (Byte)(ff9StateGlobal.btl_flag | 1);
			PersistenSingleton<EventEngine>.Instance.SetNextMap(val);
			break;
		}
		case 38u:
			ff.party.gil += (UInt32)val;
			if (ff.party.gil > 9999999u)
			{
				ff.party.gil = 9999999u;
			}
			break;
		case 39u:
			if (ff.dragon_no < 9999)
			{
				FF9StateGlobal ff9StateGlobal2 = ff;
				ff9StateGlobal2.dragon_no = (Int16)(ff9StateGlobal2.dragon_no + 1);
			}
			break;
		case 40u:
		{
			btlsnd.ff9btlsnd_song_vol_intplall(30, 0);
			FF9StateGlobal ff2 = FF9StateSystem.Common.FF9;
			ff2.btl_flag = (Byte)(ff2.btl_flag | 16);
			break;
		}
		}
	}
}
