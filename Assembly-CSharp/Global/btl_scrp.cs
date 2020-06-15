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
					num |= next.btl_id;
				}
				break;
			case 1u:
				if (next.bi.player == 0)
				{
					num |= next.btl_id;
				}
				break;
			case 2u:
				num |= next.btl_id;
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

	#region Memoria Battle Script
	public static UInt16 GetCurrentCommandData(UInt16 data_type)
	{
		// Access the current command's stats from battle scripts (SV_5 and SV_6)
		// Usage:
		// set SV_5 = Spell stat ID of the currently used spell to access
		// set spellstat = SV_6
		if (FF9StateSystem.Battle.FF9Battle.cur_cmd == null)
			return 0;
		if (data_type < 17 && FF9StateSystem.Battle.FF9Battle.cur_cmd.aa == null)
			return 0;
		switch (data_type) {
			case 0: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.Target;
			case 1: return (UInt16)(FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.DefaultAlly ? 1 : 0);
			case 2: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.DisplayStats;
			case 3: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.VfxIndex;
			case 4: return (UInt16)0; // SoundFxIndex... Memoria ignores it
			case 5: return (UInt16)(FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.ForDead ? 1 : 0);
			case 6: return (UInt16)(FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.DefaultCamera ? 1 : 0);
			case 7: return (UInt16)(FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.DefaultOnDead ? 1 : 0);
			case 8: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Ref.ScriptId;
			case 9: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Ref.Power;
			case 10: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Ref.Elements;
			case 11: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Ref.Rate;
			case 12: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Category;
			case 13: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.AddNo;
			case 14: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.MP;
			case 15: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Type;
			case 16: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Vfx2;
			case 17: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.tar_id;
			case 18: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.cmd_no;
			case 19: return (UInt16)FF9StateSystem.Battle.FF9Battle.cur_cmd.sub_no;
		}
		return 0;
	}
	
	public static void SetCurrentCommandData(UInt16 data_type, Int32 val)
	{
		// Modify the current command's stats from battle scripts (SV_5 and SV_6)
		// Usage:
		// set SV_5 = Spell stat ID of the currently used spell to modify
		// set SV_6 = newvalue
		// Note that changes to party's spells are permanent until the game closes
		if (FF9StateSystem.Battle.FF9Battle.cur_cmd == null)
			return;
		if (data_type < 17 && FF9StateSystem.Battle.FF9Battle.cur_cmd.aa == null)
			return;
		switch (data_type) {
			case 0: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.Target = (TargetType)val; break;
			case 1: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.DefaultAlly = val != 0; break;
			case 2: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.DisplayStats = (TargetDisplay)val; break;
			case 3: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.VfxIndex = (Int16)val; break;
			case 4: break;
			case 5: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.ForDead = val != 0; break;
			case 6: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.DefaultCamera = val != 0; break;
			case 7: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Info.DefaultOnDead = val != 0; break;
			case 8: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Ref.ScriptId = (Byte)val; break;
			case 9: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Ref.Power = (Byte)val; break;
			case 10: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Ref.Elements = (Byte)val; break;
			case 11: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Ref.Rate = (Byte)val; break;
			case 12: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Category = (Byte)val; break;
			case 13: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.AddNo = (Byte)val; break;
			case 14: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.MP = (Byte)val; break;
			case 15: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Type = (Byte)val; break;
			case 16: FF9StateSystem.Battle.FF9Battle.cur_cmd.aa.Vfx2 = (UInt16)val; break;
			case 17: FF9StateSystem.Battle.FF9Battle.cur_cmd.tar_id = (UInt16)val; break;
			case 18: FF9StateSystem.Battle.FF9Battle.cur_cmd.cmd_no = (BattleCommandId)val; break;
			case 19: FF9StateSystem.Battle.FF9Battle.cur_cmd.sub_no = (Byte)val; break;
		}
	}
	#endregion

	public static Int32 GetCharacterData(BTL_DATA btl, UInt32 id)
	{
		Int32 result = 16777215;
		switch (id)
		{
		case 35u:
			result = (Int32)btl.max.hp;
			break;
		case 36u:
			result = (Int32)btl.cur.hp;
			break;
		case 37u:
			result = (Int32)btl.max.mp;
			break;
		case 38u:
			result = (Int32)btl.cur.mp;
			break;
		case 39u:
			result = (Int32)btl.max.at;
			break;
		case 40u:
			result = (Int32)btl.cur.at;
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
			result = (Int32)((UInt32)btl.stat.invalid >> 24);
			break;
		case 43u:
			result = (Int32)((UInt32)btl.stat.invalid & 16777215u);
			break;
		case 44u:
			result = (Int32)((UInt32)btl.stat.permanent >> 24);
			break;
		case 45u:
			result = (Int32)((UInt32)btl.stat.permanent & 16777215u);
			break;
		case 46u:
			result = (Int32)((UInt32)btl.stat.cur >> 24);
			break;
		case 47u:
			result = (Int32)((UInt32)btl.stat.cur & 16777215u);
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
			result = (Int32)btl.dms_geo_id;
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
		case 100u: // access/modify an enemy's item to steal with SV_FunctionEnemy[100] and followings
			if (btl.bi.player == 0)
				result = btl_util.getEnemyPtr(btl).steal_item[0];
			break;
		case 101u:
			if (btl.bi.player == 0)
				result = btl_util.getEnemyPtr(btl).steal_item[1];
			break;
		case 102u:
			if (btl.bi.player == 0)
				result = btl_util.getEnemyPtr(btl).steal_item[2];
			break;
		case 103u:
			if (btl.bi.player == 0)
				result = btl_util.getEnemyPtr(btl).steal_item[3];
			break;
		case 104u: // access/modify an enemy's item to drop with SV_FunctionEnemy[104] and followings
			if (btl.bi.player == 0)
				result = btl_util.getEnemyTypePtr(btl).bonus.item[0];
			break;
		case 105u:
			if (btl.bi.player == 0)
				result = btl_util.getEnemyTypePtr(btl).bonus.item[1];
			break;
		case 106u:
			if (btl.bi.player == 0)
				result = btl_util.getEnemyTypePtr(btl).bonus.item[2];
			break;
		case 107u:
			if (btl.bi.player == 0)
				result = btl_util.getEnemyTypePtr(btl).bonus.item[3];
			break;
		case 108u:
			if (btl.bi.player == 0)
				result = (Int32)(btl_util.getEnemyTypePtr(btl).bonus.card);
			break;
		case 109u: // flags (die_atk, die_dmg, then 6 custom flags)
			if (btl.bi.player == 0)
				result = btl_util.getEnemyPtr(btl).info.flags;
			break;
		case 110u:
			// Out of reach: enemies inside battles flagged with "NoNeighboring" are all set to out of reach but they can also be placed individually using SV_FunctionEnemy[110] or setting the "OutOfReach" flag in the battle's ".memnfo" file
			result = btl.out_of_reach ? 1 : 0;
			break;
		case 111u: // SV_FunctionEnemy[111] can be used to control an enemy's transparency
			if (btl.bi.player == 0)
				result = btl_util.getEnemyPtr(btl).info.die_fade_rate;
			break;
		case 112u:
			result = btl_mot.getMotion(btl); // Current battle animation by motion ID (stand = 0, etc... see btl_mot)
			break;
		case 113u:
			if (btl.bi.player == 0)
				result = btl_util.getEnemyTypePtr(btl).category;
			else
				result = ENEMY.ENEMY_CATEGORY_HUMANOID;
			break;
		case 114u:
		{
			// Return the current attack ID if it is currently performed by the indicated character
			CMD_DATA cur_cmd = FF9StateSystem.Battle.FF9Battle.cur_cmd;
			if (cur_cmd != null && cur_cmd.regist != null && cur_cmd.regist == btl)
			{
				if (btl.bi.player == 0)
				{
					if (cur_cmd == cur_cmd.regist.cmd[3] || cur_cmd == cur_cmd.regist.cmd[4] || cur_cmd == cur_cmd.regist.cmd[5])
						result = cur_cmd.sub_no;
				}
				else
				{
					if (cur_cmd == cur_cmd.regist.cmd[1])
						result = cur_cmd.sub_no;
				}
			}
			break;
		}
		case 140:
			result = (Int32)btl.pos.x;
			break;
		case 141:
			result = (Int32)(-btl.pos.y);
			break;
		case 142:
			result = (Int32)btl.pos.z;
			break;
		case 143:
			result = (Int32)btl.rot.eulerAngles.x;
			break;
		case 144:
			result = (Int32)btl.rot.eulerAngles.y;
			break;
		case 145:
			result = (Int32)btl.rot.eulerAngles.z;
			break;
		}
		return result;
	}

	public static void SetCharacterData(BTL_DATA btl, UInt32 id, Int32 val)
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
			btl.max.hp = (UInt32)val;
			break;
		case 36u:
			btl.cur.hp = (UInt32)val;
			break;
		case 37u:
			btl.max.mp = (UInt32)val;
			break;
		case 38u:
			btl.cur.mp = (UInt32)val;
			break;
		case 40u:
			btl.cur.at = (Int16)val;
			break;
		case 42u:
			btl.stat.invalid = (BattleStatus)(((UInt32)btl.stat.invalid & 0xFFFFFFu) | ((UInt32)val << 24));
			break;
		case 43u:
			btl.stat.invalid = (BattleStatus)(((UInt32)btl.stat.invalid & 0xFF000000u) | (UInt32)val);
			break;
		case 44u:
			btl.stat.permanent = (BattleStatus)(((UInt32)btl.stat.permanent & 0xFFFFFFu) | ((UInt32)val << 24));
			break;
		case 45u:
			btl.stat.permanent = (BattleStatus)(((UInt32)btl.stat.permanent & 0xFF000000u) | (UInt32)val);
			break;
		case 46u: // Statuses can be modified by battle scripts thanks to these: set SV_FunctionEnemy[STATUS_CURRENT_A] |= Status to add
			btl_stat.RemoveStatuses(btl, (BattleStatus)((UInt32)btl.stat.cur & (~((UInt32)val << 24)) & 0xFF000000u));
			btl_stat.AlterStatuses(btl, (BattleStatus)(val << 24));
			break;
		case 47u:
			btl_stat.RemoveStatuses(btl, (BattleStatus)((UInt32)btl.stat.cur & (~(UInt32)val) & 0xFFFFFFu));
			btl_stat.AlterStatuses(btl, (BattleStatus)(val & 0xFFFFFFu));
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
			btl.stat_modifier[0] = false;
			break;
		case 73u:
			btl.elem.mgc = (Byte)val;
			btl.stat_modifier[1] = false;
			break;
		case 74u:
			btl.defence.PhisicalDefence = (Byte)val;
			btl.stat_modifier[2] = false;
			break;
		case 75u:
			btl.defence.PhisicalEvade = (Byte)val;
			btl.stat_modifier[3] = false;
			break;
		case 76u:
			btl.defence.MagicalDefence = (Byte)val;
			btl.stat_modifier[4] = false;
			break;
		case 77u:
			btl.defence.MagicalEvade = (Byte)val;
			btl.stat_modifier[5] = false;
			break;
		case 78u:
			btl.cur.at = btl.max.at;
			btl.sel_mode = 1;
			btl_cmd.SetCommand(btl.cmd[0], BattleCommandId.SummonEiko, 187u, (UInt16)val, 8u);
			UIManager.Battle.FF9BMenu_EnableMenu(true);
			break;
		// The modifiers that Memoria adds
		case 100u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).steal_item[0] = (byte)val;
			break;
		case 101u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).steal_item[1] = (byte)val;
			break;
		case 102u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).steal_item[2] = (byte)val;
			break;
		case 103u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).steal_item[3] = (byte)val;
			break;
		case 104u:
			if (btl.bi.player == 0)
				btl_util.getEnemyTypePtr(btl).bonus.item[0] = (byte)val;
			break;
		case 105u:
			if (btl.bi.player == 0)
				btl_util.getEnemyTypePtr(btl).bonus.item[1] = (byte)val;
			break;
		case 106u:
			if (btl.bi.player == 0)
				btl_util.getEnemyTypePtr(btl).bonus.item[2] = (byte)val;
			break;
		case 107u:
			if (btl.bi.player == 0)
				btl_util.getEnemyTypePtr(btl).bonus.item[3] = (byte)val;
			break;
		case 108u:
			if (btl.bi.player == 0)
				btl_util.getEnemyTypePtr(btl).bonus.card = (byte)val;
			break;
		case 109u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).info.flags = (byte)val;
			break;
		case 110u:
			btl.out_of_reach = val != 0;
			break;
		case 111u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).info.die_fade_rate = (byte)val;
			btl_util.SetFadeRate(btl, val);
			break;
		case 112u:
			btl_mot.setMotion(btl, (byte)val);
			btl.evt.animFrame = 0;
			break;
		case 113u:
			if (btl.bi.player == 0)
				btl_util.getEnemyTypePtr(btl).category = (byte)val;
			break;
		case 114u:
		{
			ushort tar_id = (ushort)PersistenSingleton<EventEngine>.Instance.GetSysList(0);
			if (btl.bi.player == 0)
			{
				if (btl_cmd.CheckUsingCommand(btl.cmd[3]))
				{
					if (btl_cmd.CheckUsingCommand(btl.cmd[4]))
					{
						if (!btl_cmd.CheckUsingCommand(btl.cmd[5]))
							btl_cmd.SetEnemyCommand((ushort)PersistenSingleton<EventEngine>.Instance.GetSysList(1), tar_id, BattleCommandId.ScriptCounter3, (uint)val);
					}
					else
					{
						btl_cmd.SetEnemyCommand((ushort)PersistenSingleton<EventEngine>.Instance.GetSysList(1), tar_id, BattleCommandId.ScriptCounter2, (uint)val);
					}
				}
				else
				{
					btl_cmd.SetEnemyCommand((ushort)PersistenSingleton<EventEngine>.Instance.GetSysList(1), tar_id, BattleCommandId.ScriptCounter1, (uint)val);
				}
			}
			else
			{
				int target_number = 0;
				for (int i = 0; i < 8; i++)
					if ((tar_id & (1 << i)) != 0)
						target_number++;
				if (!btl_cmd.CheckUsingCommand(btl.cmd[1]))
					btl_cmd.SetCommand(btl.cmd[1], BattleCommandId.Counter, (uint)val, tar_id, (target_number > 1) ? 1u : 0u);
			}
			break;
		}
		case 130u: // Note: Very crafty. Creates elemental orbs (special SPS) rotating around the BTL_DATA...
			if (val == 3)
			{
				UnityEngine.Vector3 btl_pos = btl.gameObject.transform.GetChildByName("bone000").position;
				HonoluluBattleMain.battleSPS.AddSpecialSPSObj(0, 12, btl_pos, 4.0f);
				HonoluluBattleMain.battleSPS.AddSpecialSPSObj(1, 13, btl_pos, 4.0f);
				HonoluluBattleMain.battleSPS.AddSpecialSPSObj(2, 14, btl_pos, 4.0f);
			}
			else if (val == 2) // ... and remove them 1 by 1
				HonoluluBattleMain.battleSPS.RemoveSpecialSPSObj(2);
			else if (val == 1)
				HonoluluBattleMain.battleSPS.RemoveSpecialSPSObj(1);
			else if (val == 0)
				HonoluluBattleMain.battleSPS.RemoveSpecialSPSObj(0);
			break;
		case 140:
			btl.pos.x = (float)val;
			btl.evt.posBattle.x = btl.pos.x;
			btl.evt.pos[0] = btl.pos.z;
			break;
		case 141:
			btl.pos.y = -(float)val;
			btl.evt.posBattle.y = btl.pos.y;
			btl.evt.pos[1] = btl.pos.z;
			break;
		case 142:
			btl.pos.z = (float)val;
			btl.evt.posBattle.z = btl.pos.z;
			btl.evt.pos[2] = btl.pos.z;
			break;
		case 143:
			btl.rot.eulerAngles = new UnityEngine.Vector3((float)val, btl.rot.eulerAngles.y, btl.rot.eulerAngles.z);
			btl.evt.rotBattle.eulerAngles = new UnityEngine.Vector3((float)val, btl.rot.eulerAngles.y, btl.rot.eulerAngles.z);
			break;
		case 144:
			btl.rot.eulerAngles = new UnityEngine.Vector3(btl.rot.eulerAngles.x, (float)val, btl.rot.eulerAngles.z);
			btl.evt.rotBattle.eulerAngles = new UnityEngine.Vector3(btl.rot.eulerAngles.x, (float)val, btl.rot.eulerAngles.z);
			break;
		case 145:
			btl.rot.eulerAngles = new UnityEngine.Vector3(btl.rot.eulerAngles.x, btl.rot.eulerAngles.y, (float)val);
			btl.evt.rotBattle.eulerAngles = new UnityEngine.Vector3(btl.rot.eulerAngles.x, btl.rot.eulerAngles.y, (float)val);
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
		case 43:
			result = (UInt32)btl_cmd.cmd_effect_counter; // The number of times "SBattleCalculator.CalcMain" has been called for the current attack, for multi-hit pattern changes
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
		case 43u:
			btl_cmd.cmd_effect_counter = val;
			break;
		}
	}
}
