using System;
using Memoria;
using Memoria.Data;
using UnityEngine;

namespace FF9
{
	public class btl_abil
	{
		public static Boolean CheckPartyAbility(UInt32 sa_buf_no, UInt32 sa_bit)
		{
			for (Int32 i = 0; i < 4; i++)
			{
				PLAYER player = FF9StateSystem.Common.FF9.party.member[i];
				if (player != null && (player.sa[(Int32)((UIntPtr)sa_buf_no)] & sa_bit) != 0u)
				{
					return true;
				}
			}
			return false;
		}

		public static Boolean CheckCounterAbility(BTL_DATA btl, BTL_DATA target, CMD_DATA cmd)
		{
			if (!Status.checkCurStat(btl, 1107300611u) && cmd.cmd_no < 48)
			{
				if ((btl.sa[1] & 16u) != 0u && (cmd.aa.Category & 8) != 0)
				{
					Int32 num = (Int32)btl.elem.wpr;
					if ((btl.sa[1] & 128u) != 0u)
					{
						num *= 2;
					}
					if (num > Comn.random16() % 100)
					{
						btl_cmd.SetCounter(btl, 49u, 176, target.btl_id);
						return true;
					}
				}
				if ((btl.sa[1] & 4194304u) != 0u && (cmd.aa.Category & 128) != 0)
				{
					btl_cmd.SetCounter(btl, 50u, (Int32)cmd.sub_no, target.btl_id);
					return true;
				}
			}
			return false;
		}

		public static void CheckAutoItemAbility(BTL_DATA btl, Byte cmd_no)
		{
			if (!Status.checkCurStat(btl, 1107300611u) && cmd_no < 48 && (btl.sa[1] & 16777216u) != 0u)
			{
				for (Byte b = 236; b < 238; b = (Byte)(b + 1))
				{
					if (ff9item.FF9Item_GetCount((Int32)b) != 0)
					{
						UIManager.Battle.ItemRequest((Int32)b);
						btl_cmd.SetCounter(btl, 51u, (Int32)b, btl.btl_id);
						break;
					}
				}
			}
		}

	    public static UInt16 CheckCoverAbility(UInt16 tar_id)
	    {
	        BattleUnit coverBy = null;
	        BattleUnit targetUnit = btl_scrp.FindBattleUnit(tar_id);
	        if (targetUnit.IsUnderStatus(BattleStatus.Death | BattleStatus.Petrify))
	            return 0;

	        if (targetUnit.HasCategory(CharacterCategory.Female) && targetUnit.CurrentHp < targetUnit.MaximumHp >> 1)
	            coverBy = FindStrongestDefender(SupportAbility2.ProtectGirls, targetUnit);

	        if (coverBy == null && targetUnit.IsUnderStatus(BattleStatus.LowHP))
	            coverBy = FindStrongestDefender(SupportAbility2.Cover, targetUnit);

	        if (coverBy == null)
	            return 0;

	        coverBy.FaceTheEnemy();
	        coverBy.Data.pos[0] = targetUnit.Data.pos[0];
	        coverBy.Data.pos[2] = targetUnit.Data.pos[2];

	        targetUnit.Data.pos[2] -= 400f;

	        btl_mot.setMotion(coverBy.Data, 15);
	        coverBy.IsCovered = true;

            return coverBy.Id;
	    }

	    private static BattleUnit FindStrongestDefender(SupportAbility2 ability, BattleUnit targetUnit)
	    {
	        BattleUnit coverBy = null;
	        foreach (BattleUnit next in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
	        {
	            if (!next.HasSupportAbility(ability) || next.IsUnderStatus((BattleStatus)1124077827u) || next.Id == targetUnit.Id)
	                continue;

	            if (coverBy == null || coverBy.CurrentHp < next.CurrentHp)
	                coverBy = next;
	        }
	        return coverBy;
	    }

	    public static void CheckReactionAbility(BTL_DATA btl, AA_DATA aa)
		{
			if (!Status.checkCurStat(btl, 1107300611u))
			{
				if ((btl.sa[1] & 1048576u) != 0u && btl.cur.hp != 0 && Status.checkCurStat(btl, 512u))
				{
					if (btl.cur.hp + btl.max.hp / 2 < btl.max.hp)
					{
						POINTS cur = btl.cur;
						cur.hp = (UInt16)(cur.hp + (UInt16)(btl.max.hp / 2));
					}
					else
					{
						btl.cur.hp = btl.max.hp;
					}
				}
				if ((btl.sa[1] & 8388608u) != 0u && aa.MP != 0)
				{
					if (btl.cur.mp + (Int16)aa.MP < btl.max.mp)
					{
						POINTS cur2 = btl.cur;
						cur2.mp = (Int16)(cur2.mp + (Int16)aa.MP);
					}
					else
					{
						btl.cur.mp = btl.max.mp;
					}
				}
			}
		}

		public static void CheckStatusAbility(BTL_DATA btl)
		{
			if ((btl.sa[0] & 1u) != 0u)
			{
				btl.stat.permanent |= 536870912u;
				HonoluluBattleMain.battleSPS.AddBtlSPSObj(btl, 536870912u);
			}
			if ((btl.sa[0] & 2u) != 0u)
			{
				btl.stat.permanent |= 2097152u;
			}
			if ((btl.sa[0] & 4u) != 0u)
			{
				btl.stat.permanent |= 524288u;
				btl.stat.invalid |= 1048576u;
				btl.cur.at_coef = (SByte)((Int32)btl.cur.at_coef * 3 / 2);
				HonoluluBattleMain.battleSPS.AddBtlSPSObj(btl, 524288u);
			}
			if ((btl.sa[0] & 8u) != 0u)
			{
				btl.stat.permanent |= 262144u;
				btl_stat.SetOprStatusCount(btl, 18u);
			}
			if ((btl.sa[0] & 16u) != 0u)
			{
				btl.stat.cur |= 8192u;
			}
			if ((btl.sa[1] & 256u) != 0u)
			{
				btl.stat.invalid |= 50331648u;
			}
			if ((btl.sa[1] & 65536u) != 0u)
			{
				btl.stat.invalid |= 131072u;
			}
			if ((btl.sa[1] & 131072u) != 0u)
			{
				btl.stat.invalid |= 65538u;
			}
			if ((btl.sa[1] & 262144u) != 0u)
			{
				btl.stat.invalid |= 16u;
			}
			if ((btl.sa[1] & 524288u) != 0u)
			{
				btl.stat.invalid |= 8u;
			}
			if ((btl.sa[1] & 2097152u) != 0u)
			{
				btl.stat.invalid |= 2147483649u;
			}
			if ((btl.sa[1] & 33554432u) != 0u)
			{
				btl.stat.invalid |= 4096u;
			}
			if ((btl.sa[1] & 67108864u) != 0u)
			{
				btl.stat.invalid |= 1024u;
			}
		}
	}
}
