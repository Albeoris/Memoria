using System;
using System.Linq;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Speedrun;

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
					num |= next.btl_id;
				break;
			case 1u:
				if (next.bi.player == 0)
					num |= next.btl_id;
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
			if (ff9Battle.btl_data[i].btl_id == btl_id)
			    return new BattleUnit(ff9Battle.btl_data[i]);
		return null;
	}

	// Try to have the most possible AI script compatibility
	public static CMD_DATA GetCurrentCommandSmart(BTL_DATA btl)
	{
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		if (btl == null || Configuration.Battle.Speed < 3)
			return ff9Battle.cur_cmd;
		CMD_DATA cmd = PersistenSingleton<EventEngine>.Instance.GetTriggeringCommand(btl);
		if (cmd != null)
			return cmd;
		for (Int32 i = 0; i < 6; i++)
			if (ff9Battle.cur_cmd_list.Contains(btl.cmd[i]))
				return btl.cmd[i];
		return ff9Battle.cur_cmd;
	}

	#region Memoria Battle Script
	public static UInt16 GetCurrentCommandData(UInt16 data_type, BTL_DATA btl = null)
	{
		// Access the current command's stats from battle scripts (SV_5 and SV_6)
		// Usage:
		// set SV_5 = Spell stat ID of the currently used spell to access
		// set spellstat = SV_6
		CMD_DATA cmd = GetCurrentCommandSmart(btl);
		if (cmd == null)
			return 0;
		switch (data_type)
		{
			case 1000: return (UInt16)cmd.tar_id;
			case 1001: return (UInt16)cmd.cmd_no;
			case 1002: return (UInt16)cmd.sub_no;
			case 1003: return (UInt16)(cmd.IsShortRange ? 1 : 0);
			case 1004: return (UInt16)cmd.info.cursor;
			case 1005: return (UInt16)cmd.info.stat;
			case 1006: return (UInt16)cmd.info.priority;
			case 1007: return (UInt16)cmd.info.cover;
			case 1008: return (UInt16)cmd.ScriptId;
			case 1009: return (UInt16)cmd.Power;
			case 1010: return (UInt16)cmd.Element;
			case 1011: return (UInt16)cmd.HitRate;
			case 1012: return (UInt16)cmd.AbilityCategory;
			case 1013: return (UInt16)0;
			case 1014: return (UInt16)cmd.info.CustomMPCost;
			case 1015: return (UInt16)cmd.AbilityType;
			case 1016: return (UInt16)((UInt32)cmd.AbilityStatus >> 16);
			case 1017: return (UInt16)((UInt32)cmd.AbilityStatus & 0xFFFF);
			case 1018: return (UInt16)cmd.info.dodge;
			case 1019: return (UInt16)cmd.info.reflec;
			case 1020: return (UInt16)cmd.info.meteor_miss;
			case 1021: return (UInt16)cmd.info.short_summon;
			case 1022: return (UInt16)cmd.info.mon_reflec;
			case 1023: return (UInt16)(cmd.info.IsZeroMP ? 1 : 0);
			case 1024: return (UInt16)(cmd.info.ReflectNull ? 1 : 0);
		}
		if (data_type < 17 && cmd.aa == null)
			return 0;
		switch (data_type)
		{
			case 0: return (UInt16)cmd.aa.Info.Target;
			case 1: return (UInt16)(cmd.aa.Info.DefaultAlly ? 1 : 0);
			case 2: return (UInt16)cmd.aa.Info.DisplayStats;
			case 3: return (UInt16)cmd.aa.Info.VfxIndex;
			case 4: return (UInt16)0; // SoundFxIndex... Memoria ignores it
			case 5: return (UInt16)(cmd.aa.Info.ForDead ? 1 : 0);
			case 6: return (UInt16)(cmd.aa.Info.DefaultCamera ? 1 : 0);
			case 7: return (UInt16)(cmd.aa.Info.DefaultOnDead ? 1 : 0);
			case 8: return (UInt16)cmd.aa.Ref.ScriptId;
			case 9: return (UInt16)cmd.aa.Ref.Power;
			case 10: return (UInt16)cmd.aa.Ref.Elements;
			case 11: return (UInt16)cmd.aa.Ref.Rate;
			case 12: return (UInt16)cmd.aa.Category;
			case 13: return (UInt16)cmd.aa.AddStatusNo;
			case 14: return (UInt16)cmd.aa.MP;
			case 15: return (UInt16)cmd.aa.Type;
			case 16: return (UInt16)cmd.aa.Vfx2;
			case 17: return (UInt16)cmd.tar_id;
			case 18: return (UInt16)cmd.cmd_no;
			case 19: return (UInt16)cmd.sub_no;
		}
		return 0;
	}
	
	public static void SetCurrentCommandData(UInt16 data_type, Int32 val, BTL_DATA btl = null)
	{
		// Modify the current command's stats from battle scripts (SV_5 and SV_6)
		// Usage:
		// set SV_5 = Spell stat ID of the currently used spell to modify
		// set SV_6 = newvalue
		// Note that changes to party's spells are permanent until the game closes
		CMD_DATA cmd = GetCurrentCommandSmart(btl);
		if (cmd == null)
			return;
		switch (data_type)
		{
			case 1000: cmd.tar_id = (UInt16)val; return;
			case 1001: cmd.cmd_no = (BattleCommandId)val; return;
			case 1002: cmd.sub_no = val; return;
			case 1003: cmd.IsShortRange = val != 0; return;
			case 1004: cmd.info.cursor = (Byte)val; return;
			case 1005: cmd.info.stat = (Byte)val; return;
			case 1006: cmd.info.priority = (Byte)val; return;
			case 1007: cmd.info.cover = (Byte)val; return;
			case 1008: cmd.ScriptId = val; return;
			case 1009: cmd.Power = val; return;
			case 1010: cmd.Element = (EffectElement)val; return;
			case 1011: cmd.HitRate = val; return;
			case 1012: cmd.AbilityCategory = (Byte)val; return;
			case 1013: return;
			case 1014: cmd.info.CustomMPCost = val; return;
			case 1015: cmd.AbilityType = (Byte)val; return;
			case 1016: cmd.AbilityStatus = (BattleStatus)(((UInt32)cmd.AbilityStatus & 0xFFFFu) | ((UInt32)(val & 0xFFFF) << 16)); return;
			case 1017: cmd.AbilityStatus = (BattleStatus)(((UInt32)cmd.AbilityStatus & 0xFFFF0000u) | (UInt32)(val & 0xFFFF)); return;
			case 1018: cmd.info.dodge = (Byte)val; return;
			case 1019: cmd.info.reflec = (Byte)val; return;
			case 1020: cmd.info.meteor_miss = (Byte)val; return;
			case 1021: cmd.info.short_summon = (Byte)val; return;
			case 1022: cmd.info.mon_reflec = (Byte)val; return;
			case 1023: cmd.info.IsZeroMP = val != 0; return;
			case 1024: cmd.info.ReflectNull = val != 0; return;
		}
		if (data_type < 17 && cmd.aa == null)
			return;
		switch (data_type)
		{
			case 0: cmd.aa.Info.Target = (TargetType)val; return;
			case 1: cmd.aa.Info.DefaultAlly = val != 0; return;
			case 2: cmd.aa.Info.DisplayStats = (TargetDisplay)val; return;
			case 3: cmd.aa.Info.VfxIndex = (Int16)val; return;
			case 4: return;
			case 5: cmd.aa.Info.ForDead = val != 0; return;
			case 6: cmd.aa.Info.DefaultCamera = val != 0; return;
			case 7: cmd.aa.Info.DefaultOnDead = val != 0; return;
			case 8: cmd.aa.Ref.ScriptId = val; return;
			case 9: cmd.aa.Ref.Power = val; return;
			case 10: cmd.aa.Ref.Elements = (Byte)val; return;
			case 11: cmd.aa.Ref.Rate = val; return;
			case 12: cmd.aa.Category = (Byte)val; return;
			case 13: cmd.aa.AddStatusNo = (BattleStatusIndex)val; return;
			case 14: cmd.aa.MP = val; return;
			case 15: cmd.aa.Type = (Byte)val; return;
			case 16: cmd.aa.Vfx2 = (UInt16)val; return;
			case 17: cmd.tar_id = (UInt16)val; return;
			case 18: cmd.cmd_no = (BattleCommandId)val; return;
			case 19: cmd.sub_no = val; return;
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
				result = btl_util.getPlayerPtr(btl).level;
			else
				result = btl_util.getEnemyTypePtr(btl).level;
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
			result = (Int32)btl_util.getPlayerPtr(btl).info.serial_no;
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
			result = btl.defence.PhysicalDefence;
			break;
		case 75u:
			result = btl.defence.PhysicalEvade;
			break;
		case 76u:
			result = btl.defence.MagicalDefence;
			break;
		case 77u:
			result = btl.defence.MagicalEvade;
			break;
		case 100u: // access/modify an enemy's item to steal with SV_FunctionEnemy[100] and followings
			if (btl.bi.player == 0)
				result = (Int32)btl_util.getEnemyPtr(btl).steal_item[0];
			break;
		case 101u:
			if (btl.bi.player == 0)
				result = (Int32)btl_util.getEnemyPtr(btl).steal_item[1];
			break;
		case 102u:
			if (btl.bi.player == 0)
				result = (Int32)btl_util.getEnemyPtr(btl).steal_item[2];
			break;
		case 103u:
			if (btl.bi.player == 0)
				result = (Int32)btl_util.getEnemyPtr(btl).steal_item[3];
			break;
		case 104u: // access/modify an enemy's item to drop with SV_FunctionEnemy[104] and followings
			if (btl.bi.player == 0)
				result = (Int32)btl_util.getEnemyPtr(btl).bonus_item[0];
			break;
		case 105u:
			if (btl.bi.player == 0)
				result = (Int32)btl_util.getEnemyPtr(btl).bonus_item[1];
			break;
		case 106u:
			if (btl.bi.player == 0)
				result = (Int32)btl_util.getEnemyPtr(btl).bonus_item[2];
			break;
		case 107u:
			if (btl.bi.player == 0)
				result = (Int32)btl_util.getEnemyPtr(btl).bonus_item[3];
			break;
		case 108u:
			if (btl.bi.player == 0)
				result = (Int32)btl_util.getEnemyPtr(btl).bonus_card;
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
			result = (Int32)btl_mot.getMotion(btl); // Current battle animation by motion ID (stand = 0, etc... see btl_mot)
			break;
		case 113u:
			if (btl.bi.player == 0)
				result = btl_util.getEnemyTypePtr(btl).category;
			else
				result = (Int32)EnemyCategory.Humanoid;
			break;
		case 114u:
		{
			// Return the current attack ID if it is currently performed by the indicated character
			CMD_DATA charCmd;
			if (btl_util.IsBtlUsingCommand(btl, out charCmd))
			{
				if (btl.bi.player == 0)
				{
					if (charCmd == btl.cmd[3] || charCmd == btl.cmd[4] || charCmd == btl.cmd[5])
						result = charCmd.sub_no;
				}
				else
				{
					if (charCmd == btl.cmd[1])
						result = charCmd.sub_no;
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
        case 146:
            if (btl.bi.player == 0)
            {
                result = (Int32)btl_util.getEnemyPtr(btl).bonus_exp;
            }
            break;
        case 147:
            if (btl.bi.player == 0)
            {
                result = (Int32)btl_util.getEnemyPtr(btl).bonus_gil;
            }
            break;
        case 148:
            result = battle.btl_bonus.ap;
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
				btl.SetIsEnabledMeshRenderer(i, true);
			btl.getShadow().SetActive(true);
			btl.SetIsEnabledWeaponRenderer(true);
			btl.SetIsEnabledBattleModelRenderer(true);
			btl_sys.AddCharacter(btl);
			break;
		case 33u:
			for (Int32 j = 0; j < btl.meshCount; j++)
				btl.SetIsEnabledMeshRenderer(j, false);
			btl.getShadow().SetActive(false);
			btl.SetIsEnabledWeaponRenderer(false);
			btl.SetIsEnabledBattleModelRenderer(false);
			btl_cmd.KillCommand2(btl);
			btl_sys.DelCharacter(btl);
			break;
		case 34u:
			if (btl_cmd.KillCommand2(btl))
				btl.sel_mode = 0;
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
			// Ozma battle
			if ((Configuration.Battle.Speed == 1 || Configuration.Battle.Speed == 2) && (FF9StateSystem.Battle.battleMapIndex == 57 || FF9StateSystem.Battle.battleMapIndex == 211))
				break;
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
		case 46u: // Statuses can be modified by battle scripts thanks to these: set SV_FunctionEnemy[STATUS_CURRENT_A] |=$ Status to add
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
			//very start of battle
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
			btl.SetDisappear(val != 0, 3);
			break;
		case 54u:
			btl.bi.shadow = (Byte)val;
			btl.getShadow().SetActive(btl.bi.shadow != 0);
			break;
		case 55u:
			// Many AI scripts setup a custom model size for enemies; when they do, they handle Mini in the EventEngine.Request(tagNumber = 7) function ("CounterEx" in Hades Workshop)
			geo.geoScaleSet(btl, (Int32)val, true, true);
			if (ff9Battle.btl_phase == 2) // When modified during BattleLoad, consider it to be the default
				btl.geo_scale_default = val;
			break;
		case 56u:
			geo.geoScaleReset(btl, true);
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
			btl_sys.SetBonus(btl_util.getEnemyPtr(btl));
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
			btl.defence.PhysicalDefence = val;
			btl.stat_modifier[2] = false;
			break;
		case 75u:
			btl.defence.PhysicalEvade = val;
			btl.stat_modifier[3] = false;
			break;
		case 76u:
			btl.defence.MagicalDefence = val;
			btl.stat_modifier[4] = false;
			break;
		case 77u:
			btl.defence.MagicalEvade = val;
			btl.stat_modifier[5] = false;
			break;
		case 78u:
			btl.cur.at = btl.max.at;
			btl.sel_mode = 1;
			btl_cmd.SetCommand(btl.cmd[0], BattleCommandId.SummonEiko, (Int32)BattleAbilityId.TerraHoming, (UInt16)val, Comn.countBits((UInt64)val) > 1 ? 9u : 8u);
			UIManager.Battle.FF9BMenu_EnableMenu(true);
			break;
		// The modifiers that Memoria adds
		case 100u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).steal_item[0] = (RegularItem)val;
			break;
		case 101u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).steal_item[1] = (RegularItem)val;
			break;
		case 102u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).steal_item[2] = (RegularItem)val;
			break;
		case 103u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).steal_item[3] = (RegularItem)val;
			break;
		case 104u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).bonus_item[0] = (RegularItem)val;
			break;
		case 105u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).bonus_item[1] = (RegularItem)val;
			break;
		case 106u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).bonus_item[2] = (RegularItem)val;
			break;
		case 107u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).bonus_item[3] = (RegularItem)val;
			break;
		case 108u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).bonus_card = (TetraMasterCardId)val;
			break;
		case 109u:
			if (btl.bi.player == 0)
				btl_util.getEnemyPtr(btl).info.flags = (UInt16)val;
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
			UInt16 tar_id = (UInt16)PersistenSingleton<EventEngine>.Instance.GetSysList(0);
			if (btl.bi.player == 0)
			{
				if (btl_cmd.CheckUsingCommand(btl.cmd[3]))
				{
					if (btl_cmd.CheckUsingCommand(btl.cmd[4]))
					{
						if (!btl_cmd.CheckUsingCommand(btl.cmd[5]))
							btl_cmd.SetEnemyCommand((UInt16)PersistenSingleton<EventEngine>.Instance.GetSysList(1), tar_id, BattleCommandId.ScriptCounter3, val);
					}
					else
					{
						btl_cmd.SetEnemyCommand((UInt16)PersistenSingleton<EventEngine>.Instance.GetSysList(1), tar_id, BattleCommandId.ScriptCounter2, val);
					}
				}
				else
				{
					btl_cmd.SetEnemyCommand((UInt16)PersistenSingleton<EventEngine>.Instance.GetSysList(1), tar_id, BattleCommandId.ScriptCounter1, val);
				}
			}
			else
			{
				if (!btl_cmd.CheckUsingCommand(btl.cmd[1]))
					btl_cmd.SetCommand(btl.cmd[1], BattleCommandId.Counter, val, tar_id, (Comn.countBits(tar_id) > 1) ? 1u : 0u);
			}
			break;
		}
		case 130u: // Note: Very crafty. Creates elemental orbs (special SPS) rotating around the BTL_DATA...
			if (val == 3)
			{
				// UnityEngine.Vector3 btl_pos = btl.gameObject.transform.GetChildByName("bone000").position;
				HonoluluBattleMain.battleSPS.AddSpecialSPSObj(0, 12, btl, 0, 4.0f, out _, true);
				HonoluluBattleMain.battleSPS.AddSpecialSPSObj(1, 13, btl, 0, 4.0f, out _, true);
                HonoluluBattleMain.battleSPS.AddSpecialSPSObj(2, 14, btl, 0, 4.0f, out _, true);
                }
			else if (val == 2) // ... and remove them 1 by 1
				HonoluluBattleMain.battleSPS.RemoveSpecialSPSObj(2);
			else if (val == 1)
				HonoluluBattleMain.battleSPS.RemoveSpecialSPSObj(1);
			else if (val == 0)
				HonoluluBattleMain.battleSPS.RemoveSpecialSPSObj(0);
			break;
		case 140:
			btl.pos.x = val;
			btl.evt.posBattle.x = btl.pos.x;
			btl.evt.pos[0] = btl.pos.x;
			break;
		case 141:
			btl.pos.y = -val;
			btl.evt.posBattle.y = btl.pos.y;
			btl.evt.pos[1] = btl.pos.y;
			break;
		case 142:
			btl.pos.z = val;
			btl.evt.posBattle.z = btl.pos.z;
			btl.evt.pos[2] = btl.pos.z;
			break;
		case 143:
			btl.rot.eulerAngles = new UnityEngine.Vector3(val, btl.rot.eulerAngles.y, btl.rot.eulerAngles.z);
			btl.evt.rotBattle.eulerAngles = new UnityEngine.Vector3(val, btl.rot.eulerAngles.y, btl.rot.eulerAngles.z);
			break;
		case 144:
			btl.rot.eulerAngles = new UnityEngine.Vector3(btl.rot.eulerAngles.x, val, btl.rot.eulerAngles.z);
			btl.evt.rotBattle.eulerAngles = new UnityEngine.Vector3(btl.rot.eulerAngles.x, val, btl.rot.eulerAngles.z);
			break;
		case 145:
			btl.rot.eulerAngles = new UnityEngine.Vector3(btl.rot.eulerAngles.x, btl.rot.eulerAngles.y, val);
			btl.evt.rotBattle.eulerAngles = new UnityEngine.Vector3(btl.rot.eulerAngles.x, btl.rot.eulerAngles.y, val);
			break;
        case 146:
            if (btl.bi.player == 0)
                btl_util.getEnemyPtr(btl).bonus_exp = (uint)val;
            break;
        case 147:
            if (btl.bi.player == 0)
                btl_util.getEnemyPtr(btl).bonus_gil = (uint)val;
            break;
        case 148:
            battle.btl_bonus.ap = (ushort)val;
            break;
        }
	}

	public static UInt32 GetBattleData(Int32 id)
	{
		UInt32 result = UInt32.MaxValue;
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		FF9StateGlobal ff = FF9StateSystem.Common.FF9;
		BattleUnit unit = FindBattleUnitUnlimited((UInt16)PersistenSingleton<EventEngine>.Instance.GetSysList(1));
		CMD_DATA curCmd = GetCurrentCommandSmart(unit?.Data);
		switch (id)
		{
		case 32:
			result = (Byte)ff9Battle.btl_scene.Info.StartType;
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
			if (curCmd != null && curCmd.regist != null)
				result = curCmd.regist.btl_id;
			break;
		case 37:
			if (curCmd != null)
				result = curCmd.tar_id;
			break;
		case 38:
			if (curCmd != null && curCmd.regist != null && curCmd.regist.weapon != null)
				result = curCmd.regist.weapon.Ref.Elements;
			break;
		case 39:
			if (curCmd != null && curCmd.regist != null)
				result = (UInt32)curCmd.Element;
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
				CharacterSerialNumber serialNumber = btl_util.getSerialNumber(next);
				if (serialNumber == CharacterSerialNumber.EIKO_FLUTE || serialNumber == CharacterSerialNumber.EIKO_KNIFE)
				{
					result = !btl_cmd.CheckSpecificCommand(next, BattleCommandId.SysLastPhoenix) ? 0u : 1u;
					break;
				}
			}
			break;
		case 43:
			result = curCmd != null ? (UInt32)curCmd.info.effect_counter : 0; // The number of times the effect point has been reached for the current attack, for multi-hit pattern changes
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
		case 32u: // Disable ATB
			AutoSplitterPipe.SignalBattleStop();
			UIManager.Battle.FF9BMenu_EnableMenu(false);
			ff9Battle.btl_escape_key = 0;
			ff9Battle.cmd_status &= 65533;
			ff9Battle.btl_phase = 5;
			ff9Battle.btl_seq = 2;
			btl_cmd.KillAllCommand(ff9Battle);
			for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
				next.bi.cmd_idle = 0;
			break;
		case 33u: // End battle
			if (ff9Battle.btl_phase == 1)
			{
				ff.btl_result = (Byte)val;
				if (val == 1 && ff9Battle.btl_scene.Info.WinPose)
				{
					ff9Battle.btl_phase = 5;
					ff9Battle.btl_seq = 4;
				}
				else
				{
					if (ff.btl_result == 1)
						ff.btl_result = 2;
					ff9Battle.btl_phase = 8;
					ff9Battle.btl_seq = 0;
				}
				if (ff.btl_result == 5) // Scripted interruption, such as Black Waltz 3 (first time)
					BattleVoice.TriggerOnBattleInOut("BattleInterrupted");
				else if (ff.btl_result == 7) // Enemy flee, such as (Magic) Vice or friendly monsters when attacked
					BattleVoice.TriggerOnBattleInOut("EnemyEscape");
			}
			break;
		case 34u: // Game Over
			if (ff9Battle.btl_phase == 1)
			{
				ff9Battle.btl_phase = 7;
				ff9Battle.btl_seq = 0;
				ff.btl_result = 3;
			}
			break;
		case 35u: // Enable ATB
			if (ff9Battle.btl_phase == 1)
			{
				BTL_SCENE_INFO info = ff9Battle.btl_scene.Info;
				ff9Battle.btl_phase = 3;
				ff9Battle.btl_seq = 0;
				if (!info.SpecialStart || !info.BackAttack || !info.Preemptive)
					info.StartType = battle_start_type_tags.BTL_START_NORMAL_ATTACK;
			}
			break;
		case 36u: // Run Camera
			if (ff9Battle.btl_phase == 1)
				SFX.SetCamera(val);
			break;
		case 37u: // Change next field
		{
			FF9StateGlobal ff9StateGlobal = ff;
			ff9StateGlobal.btl_flag = (Byte)(ff9StateGlobal.btl_flag | 1);
			PersistenSingleton<EventEngine>.Instance.SetNextMap(val);
			break;
		}
		case 38u:
			ff.party.gil += (UInt32)val;
			if (ff.party.gil > 9999999u)
				ff.party.gil = 9999999u;
			break;
		case 39u:
			if (ff.categoryKillCount[3] < 9999)
				ff.categoryKillCount[3]++; // Manually increase the counter of dragons killed
			break;
		case 40u:
		{
			btlsnd.ff9btlsnd_song_vol_intplall(30, 0);
			FF9StateGlobal ff2 = FF9StateSystem.Common.FF9;
			ff2.btl_flag = (Byte)(ff2.btl_flag | 16);
			break;
		}
		case 43u:
			CMD_DATA curCmd = GetCurrentCommandSmart(FindBattleUnitUnlimited((UInt16)PersistenSingleton<EventEngine>.Instance.GetSysList(1))?.Data);
			if (curCmd != null)
				curCmd.info.effect_counter = val;
			break;
		}
	}
}
