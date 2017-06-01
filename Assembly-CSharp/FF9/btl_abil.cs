using System;
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
			BTL_DATA btl_DATA = (BTL_DATA)null;
			BTL_DATA btlDataPtr = btl_scrp.GetBtlDataPtr(tar_id);
			if (Status.checkCurStat(btlDataPtr, 257u))
			{
				return 0;
			}
			if ((btl_util.getPlayerPtr(btlDataPtr).category & 2) != 0 && (Int32)btlDataPtr.cur.hp < (int)btlDataPtr.max.hp >> 1)
			{
				for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
				{
					if ((next.sa[1] & 64u) != 0u && !Status.checkCurStat(next, 1124077827u) && next != btlDataPtr)
					{
						if (btl_DATA != null)
						{
							if (btl_DATA.cur.hp < next.cur.hp)
							{
								btl_DATA = next;
							}
						}
						else
						{
							btl_DATA = next;
						}
					}
				}
			}
			if (btl_DATA == null && Status.checkCurStat(btlDataPtr, 512u))
			{
				for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
				{
					if ((next.sa[1] & 32u) != 0u && !Status.checkCurStat(next, 1124077827u) && next != btlDataPtr)
					{
						if (btl_DATA != null)
						{
							if (btl_DATA.cur.hp < next.cur.hp)
							{
								btl_DATA = next;
							}
						}
						else
						{
							btl_DATA = next;
						}
					}
				}
			}
			if (btl_DATA != null)
			{
				Int32 num = btl_mot.setDirection(btl_DATA);
				btl_DATA.evt.rotBattle.eulerAngles = new Vector3(btl_DATA.evt.rotBattle.eulerAngles.x, (Single)num, btl_DATA.evt.rotBattle.eulerAngles.z);
				btl_DATA.rot.eulerAngles = new Vector3(btl_DATA.rot.eulerAngles.x, (Single)num, btl_DATA.rot.eulerAngles.z);
				btl_DATA.pos[0] = btlDataPtr.pos[0];
				btl_DATA.pos[2] = btlDataPtr.pos[2];
				BTL_DATA btl_DATA2 = btlDataPtr;
				Int32 index2;
				Int32 index = index2 = 2;
				Single num2 = btl_DATA2.pos[index2];
				btl_DATA2.pos[index] = num2 + -400f;
				btl_mot.setMotion(btl_DATA, 15);
				btl_DATA.bi.cover = 1;
				return btl_DATA.btl_id;
			}
			return 0;
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
