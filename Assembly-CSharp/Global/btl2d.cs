using System;
using System.IO;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using UnityEngine;

public class btl2d
{
	static btl2d()
	{
		// Note: this type is marked as 'beforefieldinit'.
		SByte[][] array = new SByte[19][];
		array[0] = new SByte[]
		{
			-1,
			-2,
			-9,
			-10,
			-6,
			0
		};
		array[1] = new SByte[]
		{
			-1,
			-2,
			-9,
			-10,
			-6,
			0
		};
		array[2] = new SByte[]
		{
			0,
			-1,
			-7,
			-8,
			-3,
			0
		};
		array[3] = new SByte[]
		{
			-1,
			-2,
			-9,
			-8,
			-5,
			0
		};
		array[4] = new SByte[]
		{
			-1,
			-2,
			-9,
			-8,
			-5,
			0
		};
		array[5] = new SByte[]
		{
			-1,
			-2,
			-9,
			-8,
			-5,
			0
		};
		array[6] = new SByte[]
		{
			-1,
			-2,
			-9,
			-8,
			-5,
			0
		};
		array[7] = new SByte[]
		{
			0,
			-3,
			-8,
			-9,
			-5,
			0
		};
		array[8] = new SByte[]
		{
			0,
			-3,
			-8,
			-9,
			-5,
			0
		};
		array[9] = new SByte[]
		{
			0,
			-1,
			-10,
			-9,
			-5,
			0
		};
		array[10] = new SByte[]
		{
			-1,
			0,
			-7,
			-7,
			-5,
			0
		};
		array[11] = new SByte[]
		{
			-1,
			0,
			-7,
			-7,
			-5,
			0
		};
		array[12] = new SByte[]
		{
			0,
			-2,
			-8,
			-8,
			-4,
			0
		};
		Int32 num = 13;
		SByte[] array2 = new SByte[6];
		array2[2] = -8;
		array2[3] = -11;
		array2[4] = -6;
		array[num] = array2;
		array[14] = new SByte[]
		{
			-2,
			-2,
			-9,
			-9,
			-6,
			0
		};
		array[15] = new SByte[]
		{
			-1,
			-1,
			-9,
			-9,
			-5,
			0
		};
		array[16] = new SByte[]
		{
			-1,
			-1,
			-8,
			-8,
			-5,
			0
		};
		array[17] = new SByte[]
		{
			-1,
			-1,
			-8,
			-8,
			-5,
			0
		};
		array[18] = new SByte[]
		{
			-1,
			-2,
			-7,
			-8,
			-5,
			0
		};
		btl2d.wZofsPC = array;
	}

	public static void Btl2dInit()
	{
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		ff9Battle.btl2d_work_set.NewID = 0;
		ff9Battle.btl2d_work_set.Timer = 0;
		ff9Battle.btl2d_work_set.OldDisappear = Byte.MaxValue;
		BTL2D_ENT[] entry = ff9Battle.btl2d_work_set.Entry;
		for (Int16 num = 0; num < 16; num = (Int16)(num + 1))
		{
			entry[num].BtlPtr = null;
		}
	}

	public static void InitBattleSPSBin()
	{
		String[] arg = new String[]
		{
			"st_doku.dat",
			"st_mdoku.dat",
			"st_moku.dat",
			"st_moum.dat",
			"st_nemu.dat",
			"st_heat.dat",
			"st_friz.dat",
			"st_basak.dat",
			"st_meiwa.dat",
			"st_slow.dat",
			"st_heis.dat",
			"st_rif.dat"
		};
		for (Int32 i = 0; i < btl2d.wStatIconTbl.Length; i++)
		{
			Byte[] bytes = AssetManager.LoadBytes("BattleMap/BattleSPS/" + arg + ".sps", out _, false);
			if (bytes == null)
			{
				return;
			}
		}
	}

	public static void Btl2dReq(BTL_DATA pBtl)
	{
		Byte b = 0;
		UInt16 fig_info = pBtl.fig_info;
		if (pBtl.bi.disappear == 0)
		{
			if ((fig_info & 256) != 0)
			{
			    btl_para.SetTroubleDamage(new BattleUnit(pBtl));
			}
			if ((fig_info & 128) != 0)
			{
				btl2d.Btl2dReqSymbol(pBtl, 2, 0, 0);
			}
			else if ((fig_info & 96) != 0)
			{
				if ((fig_info & 32) != 0)
				{
					btl2d.Btl2dReqSymbol(pBtl, 0, 0, 0);
					b = 2;
				}
				if ((fig_info & 64) != 0)
				{
					btl2d.Btl2dReqSymbol(pBtl, 1, 0, b);
				}
			}
			else
			{
				if ((fig_info & 1) != 0)
				{
					if ((fig_info & 4) != 0)
					{
						BTL2D_ENT btl2D_ENT = btl2d.Btl2dReqSymbol(pBtl, 3, 128, 0);
						b = 2;
					}
					if ((fig_info & 2) != 0)
					{
						btl2d.Btl2dReqHP(pBtl, pBtl.fig, 192, b);
					}
					else
					{
						btl2d.Btl2dReqHP(pBtl, pBtl.fig, 0, b);
					}
					b = (Byte)(b + 4);
				}
				if ((fig_info & 8) != 0)
				{
					if ((fig_info & 16) != 0)
					{
						btl2d.Btl2dReqMP(pBtl, pBtl.m_fig, 192, b);
					}
					else
					{
						btl2d.Btl2dReqMP(pBtl, pBtl.m_fig, 0, b);
					}
				}
			}
		}
		pBtl.fig_info = 0;
		pBtl.fig = 0;
		pBtl.m_fig = 0;
	}

	public static void Btl2dStatReq(BTL_DATA pBtl)
	{
		Byte b = 0;
		UInt16 fig_stat_info = pBtl.fig_stat_info;
		if (pBtl.bi.disappear == 0)
		{
			if ((fig_stat_info & 1) != 0)
			{
				BTL2D_ENT btl2D_ENT = btl2d.Btl2dReqHP(pBtl, pBtl.fig_regene_hp, (UInt16)(((fig_stat_info & 8) == 0) ? 192 : 0), 0);
				btl2D_ENT.NoClip = 1;
				btl2D_ENT.Yofs = -12;
				b = 4;
			}
			if ((fig_stat_info & 2) != 0)
			{
				BTL2D_ENT btl2D_ENT = btl2d.Btl2dReqHP(pBtl, pBtl.fig_poison_hp, 0, b);
				btl2D_ENT.NoClip = 1;
				btl2D_ENT.Yofs = -12;
				b = (Byte)(b + 4);
			}
			if ((fig_stat_info & 4) != 0)
			{
				BTL2D_ENT btl2D_ENT = btl2d.Btl2dReqMP(pBtl, pBtl.fig_poison_mp, 0, b);
				btl2D_ENT.NoClip = 1;
				btl2D_ENT.Yofs = -12;
			}
		}
		pBtl.fig_stat_info = 0;
		pBtl.fig_regene_hp = 0;
		pBtl.fig_poison_hp = 0;
		pBtl.fig_poison_mp = 0;
	}

	public static BTL2D_ENT GetFreeEntry(BTL_DATA pBtl)
	{
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		BTL2D_WORK btl2d_work_set = ff9Battle.btl2d_work_set;
		Int16 num = (Int16)(btl2d_work_set.NewID - 1);
		if (num < 0)
		{
			num = 15;
		}
		btl2d_work_set.NewID = num;
		BTL2D_ENT btl2D_ENT = btl2d_work_set.Entry[num];
		btl2D_ENT.BtlPtr = pBtl;
		btl2D_ENT.Cnt = 0;
		btl2D_ENT.Delay = 0;
		btl2D_ENT.trans = pBtl.gameObject.transform.GetChildByName("bone" + pBtl.tar_bone.ToString("D3"));
		Vector3 position = btl2D_ENT.trans.position;
		BTL2D_ENT btl2D_ENT2 = btl2D_ENT;
		btl2D_ENT2.Yofs = (SByte)(btl2D_ENT2.Yofs + 4);
		btl2D_ENT.trans.position = position;
		return btl2D_ENT;
	}

	public static BTL2D_ENT Btl2dReqHP(BTL_DATA pBtl, Int32 pNum, UInt16 pCol, Byte pDelay)
	{
		BTL2D_ENT freeEntry = btl2d.GetFreeEntry(pBtl);
		freeEntry.Type = 0;
		freeEntry.Delay = pDelay;
		freeEntry.Work.Num.Color = pCol;
		freeEntry.Work.Num.Value = (UInt32)pNum;
		return freeEntry;
	}

	public static BTL2D_ENT Btl2dReqMP(BTL_DATA pBtl, Int32 pNum, UInt16 pCol, Byte pDelay)
	{
		BTL2D_ENT freeEntry = btl2d.GetFreeEntry(pBtl);
		freeEntry.Type = 1;
		freeEntry.Delay = pDelay;
		freeEntry.Work.Num.Color = pCol;
		freeEntry.Work.Num.Value = (UInt32)pNum;
		return freeEntry;
	}

	public static BTL2D_ENT Btl2dReqSymbol(BTL_DATA pBtl, Byte pNum, UInt16 pCol, Byte pDelay)
	{
		BTL2D_ENT freeEntry = btl2d.GetFreeEntry(pBtl);
		freeEntry.Type = 2;
		freeEntry.Delay = pDelay;
		freeEntry.Work.Num.Color = pCol;
		freeEntry.Work.Num.Value = pNum;
		return freeEntry;
	}

	public static void Btl2dMain()
	{
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		BTL2D_WORK btl2d_work_set = ff9Battle.btl2d_work_set;
		Int16 num = btl2d_work_set.NewID;
		for (Int16 num2 = 0; num2 < 16; num2 = (Int16)(num2 + 1))
		{
			BTL2D_ENT btl2D_ENT = btl2d_work_set.Entry[num];
			if (btl2D_ENT.BtlPtr != null)
			{
				if (btl2D_ENT.Type > 2)
				{
					btl2D_ENT.BtlPtr = null;
				}
				else if (btl2D_ENT.Delay != 0)
				{
					BTL2D_ENT btl2D_ENT2 = btl2D_ENT;
					btl2D_ENT2.Delay = (Byte)(btl2D_ENT2.Delay - 1);
				}
				else
				{
					String text = String.Empty;
					HUDMessage.MessageStyle style = HUDMessage.MessageStyle.DAMAGE;
					if (btl2D_ENT.Type == 0)
					{
						if (btl2D_ENT.Work.Num.Color == 0)
						{
							style = HUDMessage.MessageStyle.DAMAGE;
						}
						else
						{
							style = HUDMessage.MessageStyle.RESTORE_HP;
						}
						text = btl2D_ENT.Work.Num.Value.ToString();
					}
					else if (btl2D_ENT.Type == 1)
					{
						if (btl2D_ENT.Work.Num.Color == 0)
						{
							style = HUDMessage.MessageStyle.DAMAGE;
						}
						else
						{
							style = HUDMessage.MessageStyle.RESTORE_MP;
						}
						text = btl2D_ENT.Work.Num.Value.ToString() + " " + Localization.Get("MPCaption");
					}
					else if (btl2D_ENT.Type == 2)
					{
						if (btl2D_ENT.Work.Num.Value == 0u)
						{
							text = Localization.Get("Miss");
							style = HUDMessage.MessageStyle.MISS;
						}
						else if (btl2D_ENT.Work.Num.Value == 1u)
						{
							text = Localization.Get("Death");
							style = HUDMessage.MessageStyle.DEATH;
						}
						else if (btl2D_ENT.Work.Num.Value == 2u)
						{
							text = Localization.Get("Guard");
							style = HUDMessage.MessageStyle.GUARD;
						}
						else if (btl2D_ENT.Work.Num.Value == 3u)
						{
							text = NGUIText.FF9YellowColor + Localization.Get("Critical") + "[-] \n " + text;
							style = HUDMessage.MessageStyle.CRITICAL;
						}
					}
					Singleton<HUDMessage>.Instance.Show(btl2D_ENT.trans, text, style, new Vector3(0f, btl2D_ENT.Yofs, 0f), 0);
				    UIManager.Battle.DisplayParty();
				    btl2D_ENT.BtlPtr = null;
				}
			}
			num = (Int16)(num + 1);
			if (num >= 16)
			{
				num = 0;
			}
		}
		btl2d.Btl2dStatCount();
		if (SFX.GetEffectJTexUsed() == 0)
		{
			btl2d.Btl2dStatIcon();
		}
		BTL2D_WORK btl2D_WORK = btl2d_work_set;
		btl2D_WORK.Timer = (UInt16)(btl2D_WORK.Timer + 1);
		Byte b = Byte.MaxValue;
		for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
		{
			if (next.bi.disappear == 0)
			{
				b = (Byte)(b & (Byte)(~(Byte)next.btl_id));
			}
		}
		btl2d_work_set.OldDisappear = b;
	}

	private static void Btl2dStatIcon()
	{
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		BTL2D_WORK btl2d_work_set = ff9Battle.btl2d_work_set;
		Vector3 rot;
		rot.x = 0f;
		rot.z = 0f;
		for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
		{
			if (next.bi.disappear == 0)
			{
				if ((next.flags & geo.GEO_FLAGS_CLIP) == 0)
				{
					if ((btl2d_work_set.OldDisappear & next.btl_id) == 0)
					{
						BattleStatus num = next.stat.cur | next.stat.permanent;
						if ((num & BattleStatus.Death) == 0u)
						{
							if ((num & STATUS_2D_ICON) != 0u)
							{
								if (next.bi.player == 0 || !btl_mot.checkMotion(next, BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE))
								{
									Int32 num2 = ff9.rsin(fixedPointAngle: (Int32)(next.rot.eulerAngles.y / 360f * 4096f));
									Int32 num3 = ff9.rcos(fixedPointAngle: (Int32)(next.rot.eulerAngles.y / 360f * 4096f));
									Int16 num4;
									Byte[] array;
									SByte[] array2;
									SByte[] array3;
									if (next.bi.player != 0)
									{
										if (next.is_monster_transform)
										{
											array = next.monster_transform.icon_bone;
											array2 = next.monster_transform.icon_y;
											array3 = next.monster_transform.icon_z;
										}
										else
										{
											num4 = FF9StateSystem.Common.FF9.player[next.bi.slot_no].info.serial_no;
											array = btl2d.wBonePC[num4];
											array2 = btl2d.wYofsPC[num4];
											array3 = btl2d.wZofsPC[num4];
										}
									}
									else
									{
										ENEMY_TYPE et = ff9Battle.enemy[next.bi.slot_no].et;
										array = et.icon_bone;
										array2 = et.icon_y;
										array3 = et.icon_z;
									}
									Int32 num5 = 0;
									num4 = 12;
									for (;;)
									{
										Int16 num6 = num4;
										num4 = (Int16)(num6 - 1);
										if (num6 == 0)
										{
											break;
										}
										btl2d.STAT_ICON_TBL stat_ICON_TBL = btl2d.wStatIconTbl[num5];
										if ((num & stat_ICON_TBL.Mask) != 0u)
										{
											if ((num & stat_ICON_TBL.Mask2) == 0u)
											{
												Int16 num7 = (Int16)(array2[stat_ICON_TBL.Pos] << 4);
												Int16 num8 = (Int16)(array3[stat_ICON_TBL.Pos] << 4);
												if ((next.flags & geo.GEO_FLAGS_SCALE) != 0)
												{
													num7 = (Int16)((Int32)(num7 * next.gameObject.transform.localScale.y));
													num8 = (Int16)((Int32)(num8 * next.gameObject.transform.localScale.z));
												}
												Vector3 position = next.gameObject.transform.GetChildByName("bone" + array[stat_ICON_TBL.Pos].ToString("D3")).position;
												Vector3 pos;
												pos.x = position.x + (num8 * num2 >> 12);
												pos.y = position.y - num7;
												pos.z = position.z + (num8 * num3 >> 12);
												if (stat_ICON_TBL.Type != 0)
												{
													rot.y = 0f;
													HonoluluBattleMain.battleSPS.UpdateBtlStatus(next, stat_ICON_TBL.Mask, pos, rot, btl2d_work_set.Timer);
												}
												else
												{
													Int32 ang = stat_ICON_TBL.Ang;
													if (ang != 0)
													{
														Int32 num9 = (Int32)(next.rot.eulerAngles.y / 360f * 4095f);
														num9 = (num9 + 3072 & 4095);
														rot.y = num9 / 4095f * 360f;
													}
													else
													{
														rot.y = 0f;
													}
													HonoluluBattleMain.battleSPS.UpdateBtlStatus(next, stat_ICON_TBL.Mask, pos, rot, btl2d_work_set.Timer);
												}
											}
										}
										num5++;
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public static Int16 S_GetShpFrame(BinaryReader shp)
	{
		shp.BaseStream.Seek(0L, SeekOrigin.Begin);
		return (Int16)(shp.ReadInt16() & Int16.MaxValue);
	}

	public static UInt16 acUShort(BinaryReader p, Int32 index = 0)
	{
		p.BaseStream.Seek(index, SeekOrigin.Begin);
		return p.ReadUInt16();
	}

	public static Byte acChar(BinaryReader p, Int32 index = 0)
	{
		p.BaseStream.Seek(index, SeekOrigin.Begin);
		return p.ReadByte();
	}

	public static UInt64 acULong(BinaryReader p, Int32 index = 0)
	{
		p.BaseStream.Seek(index, SeekOrigin.Begin);
		return p.ReadUInt64();
	}

	public static Int32 SAbrID(Int32 abr)
	{
		return (abr & 3) << 5;
	}

	public static Int32 getSprtcode(Int32 abr)
	{
		return 100 | ((abr != 255) ? 2 : 0);
	}

	public static void S_ShpNScPut(BinaryReader shp, Vector3 pos, Int32 frame, Int32 abr, Int32 fade)
	{
	}

	public static void S_SpsNScPut(BinaryReader sps, Vector3 pos, Vector3 ang, Int32 sc, Int32 frame, Int32 abr, Int32 fade, Int32 pad)
	{
	}

	private static void Btl2dStatCount()
	{
		btl2d.STAT_CNT_TBL[] array = new btl2d.STAT_CNT_TBL[]
		{
			new btl2d.STAT_CNT_TBL(BattleStatus.Doom, 11, 0),
			new btl2d.STAT_CNT_TBL(BattleStatus.GradualPetrify, 15, 1)
		};
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
	    BattleStatus status = BattleStatus.Doom | BattleStatus.GradualPetrify;
		Int16 num2 = 2;
		for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
		{
			if (next.bi.disappear == 0)
			{
				if ((next.flags & geo.GEO_FLAGS_CLIP) == 0)
				{
					if ((ff9Battle.btl2d_work_set.OldDisappear & next.btl_id) == 0)
					{
						BattleStatus cur = next.stat.cur;
						if ((cur & BattleStatus.Death) == 0u)
						{
							if ((cur & status) != 0u)
							{
								Int16 num3;
								Int16 num4;
								Int16 num5;
								if (next.bi.player != 0)
								{
									num3 = FF9StateSystem.Common.FF9.player[next.bi.slot_no].info.serial_no;
									num4 = btl2d.wBonePC[num3][5];
									num5 = btl2d.wYofsPC[num3][5];
								}
								else
								{
									ENEMY_TYPE et = ff9Battle.enemy[next.bi.slot_no].et;
									num4 = et.icon_bone[5];
									num5 = et.icon_y[5];
								}
								if ((next.flags & geo.GEO_FLAGS_SCALE) != 0)
								{
									num5 = (Int16)((Int32)(num5 * next.gameObject.transform.localScale.y));
								}
								Transform childByName = next.gameObject.transform.GetChildByName("bone" + num4.ToString("D3"));
								Int32 num6 = -(num5 << 4);
								Int32 num7 = 0;
								num3 = num2;
								for (;;)
								{
									Int16 num8 = num3;
									num3 = (Int16)(num8 - 1);
									if (num8 == 0)
									{
										break;
									}
									btl2d.STAT_CNT_TBL stat_CNT_TBL = array[num7];
									if ((cur & stat_CNT_TBL.Mask) != 0u)
									{
										Int16 cdown_max;
										if ((cdown_max = next.stat.cnt.cdown_max) < 1)
										{
											break;
										}
										Int16 num9;
										if ((num9 = next.stat.cnt.conti[stat_CNT_TBL.Idx]) < 0)
										{
											break;
										}
										Int16 num10 = next.cur.at_coef;
										num4 = (Int16)(num9 * 10 / cdown_max);
										UInt16 num11;
										if (num9 <= 0)
										{
											num11 = 2;
										}
										else
										{
											num5 = (Int16)((num9 - num10) * 10 / cdown_max);
											num11 = (UInt16)((num4 == num5) ? 0 : 2);
										}
										Int32 num12;
										if (stat_CNT_TBL.Col != 0)
										{
											Byte b = (Byte)((num4 << 4) + 32);
											num12 = (b << 16 | b << 8 | b);
										}
										else
										{
											num12 = 16777216;
										}
										num12 |= num11 << 24;
										num4 = (Int16)(num4 + 1);
										if (num4 > 10)
										{
											num4 = 10;
										}
										if (num7 == 0)
										{
											if (next.deathMessage == null)
											{
												next.deathMessage = Singleton<HUDMessage>.Instance.Show(childByName, "10", HUDMessage.MessageStyle.DEATH_SENTENCE, new Vector3(0f, num6), 0);
											    UIManager.Battle.DisplayParty();
											}
											else
											{
												next.deathMessage.Label = num4.ToString();
											}
										}
										else if (num7 == 1)
										{
											if (next.petrifyMessage == null)
											{
												next.petrifyMessage = Singleton<HUDMessage>.Instance.Show(childByName, "10", HUDMessage.MessageStyle.PETRIFY, new Vector3(0f, num6), 0);
											    UIManager.Battle.DisplayParty();
											}
											else
											{
												String str = "[" + (num12 & 16777215).ToString("X6") + "]";
												next.petrifyMessage.Label = str + num4.ToString();
											}
										}
									}
									num7++;
								}
							}
						}
					}
				}
			}
		}
	}

	public const Byte BTL2D_NUM = 16;

	public const Byte BTL2D_TYPE_HP = 0;

	public const Byte BTL2D_TYPE_MP = 1;

	public const Byte BTL2D_TYPE_SYM = 2;

	public const Byte BTL2D_TYPE_MAX = 2;

	public const Byte DMG_COL_WHITE = 0;

	public const Byte DMG_COL_RED = 64;

	public const Byte DMG_COL_YELLOW = 128;

	public const Byte DMG_COL_GREEN = 192;

    public const BattleStatus STATUS_2D_ICON = BattleStatus.Venom | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Berserk | BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Haste | BattleStatus.Slow | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Reflect;

    public const UInt32 STATUS_2D_ICON_MASK = 588974138u;

	public const Byte ABR_OFF = 255;

	public const Byte ABR_50ADD = 0;

	public const Byte ABR_ADD = 1;

	public const Byte ABR_SUB = 2;

	public const Byte ABR_25ADD = 3;

	public const Int16 STAT_ICON_NUM = 12;

	public const Int32 SOTSIZE = 4096;

	public const Byte Sprtcode = 100;

	public static btl2d.STAT_ICON_TBL[] wStatIconTbl = new btl2d.STAT_ICON_TBL[]
	{
		new btl2d.STAT_ICON_TBL(BattleStatus.Poison, 0u, null, 0, 1, 0, 0),
		new btl2d.STAT_ICON_TBL(BattleStatus.Venom, 0u, null, 0, 1, 0, 0),
		new btl2d.STAT_ICON_TBL(BattleStatus.Slow, 0u, null, 0, Byte.MaxValue, 1, 0),
		new btl2d.STAT_ICON_TBL(BattleStatus.Haste, 0u, null, 0, Byte.MaxValue, 1, 0),
		new btl2d.STAT_ICON_TBL(BattleStatus.Sleep, 0u, null, 0, Byte.MaxValue, 0, 1),
		new btl2d.STAT_ICON_TBL(BattleStatus.Heat, 0u, null, 1, 1, 0, 0),
		new btl2d.STAT_ICON_TBL(BattleStatus.Freeze, 0u, null, 1, 1, 0, 0),
		new btl2d.STAT_ICON_TBL(BattleStatus.Reflect, BattleStatus.Petrify, null, 1, 1, 0, 0),
		new btl2d.STAT_ICON_TBL(BattleStatus.Silence, 0u, null, 2, Byte.MaxValue, 1, 1),
		new btl2d.STAT_ICON_TBL(BattleStatus.Blind, 0u, null, 3, 2, 0, 0),
		new btl2d.STAT_ICON_TBL(BattleStatus.Trouble, 0u, null, 4, Byte.MaxValue, 1, 0),
		new btl2d.STAT_ICON_TBL(BattleStatus.Berserk, 0u, null, 4, 1, 0, 0)
	};

	public static Byte[][] wBonePC = new Byte[][]
	{
		new Byte[]
		{
			8,
			8,
			8,
			8,
			8,
			1
		},
		new Byte[]
		{
			8,
			8,
			8,
			8,
			8,
			1
		},
		new Byte[]
		{
			7,
			7,
			7,
			7,
			7,
			1
		},
		new Byte[]
		{
			19,
			19,
			19,
			19,
			19,
			1
		},
		new Byte[]
		{
			19,
			19,
			19,
			19,
			19,
			1
		},
		new Byte[]
		{
			19,
			19,
			19,
			19,
			19,
			1
		},
		new Byte[]
		{
			19,
			19,
			19,
			19,
			19,
			1
		},
		new Byte[]
		{
			20,
			20,
			20,
			20,
			20,
			1
		},
		new Byte[]
		{
			20,
			20,
			20,
			20,
			20,
			1
		},
		new Byte[]
		{
			6,
			6,
			6,
			6,
			6,
			1
		},
		new Byte[]
		{
			19,
			19,
			19,
			19,
			19,
			1
		},
		new Byte[]
		{
			19,
			19,
			19,
			19,
			19,
			1
		},
		new Byte[]
		{
			8,
			8,
			8,
			8,
			8,
			1
		},
		new Byte[]
		{
			18,
			18,
			18,
			18,
			18,
			1
		},
		new Byte[]
		{
			12,
			12,
			12,
			12,
			12,
			1
		},
		new Byte[]
		{
			8,
			8,
			8,
			8,
			8,
			1
		},
		new Byte[]
		{
			3,
			3,
			3,
			3,
			3,
			1
		},
		new Byte[]
		{
			3,
			3,
			3,
			3,
			3,
			1
		},
		new Byte[]
		{
			18,
			18,
			18,
			18,
			18,
			1
		}
	};

	private static SByte[][] wYofsPC = new SByte[][]
	{
		new SByte[]
		{
			-10,
			0,
			-7,
			-1,
			-7,
			-18
		},
		new SByte[]
		{
			-10,
			0,
			-7,
			-1,
			-7,
			-18
		},
		new SByte[]
		{
			-11,
			0,
			-8,
			-3,
			-8,
			-18
		},
		new SByte[]
		{
			-10,
			0,
			-7,
			-1,
			-7,
			-18
		},
		new SByte[]
		{
			-10,
			0,
			-7,
			-1,
			-7,
			-18
		},
		new SByte[]
		{
			-10,
			0,
			-7,
			-1,
			-7,
			-18
		},
		new SByte[]
		{
			-10,
			0,
			-7,
			-1,
			-7,
			-18
		},
		new SByte[]
		{
			-11,
			0,
			-6,
			-1,
			-6,
			-22
		},
		new SByte[]
		{
			-11,
			0,
			-6,
			-1,
			-6,
			-22
		},
		new SByte[]
		{
			-12,
			-2,
			-9,
			-1,
			-9,
			-23
		},
		new SByte[]
		{
			-10,
			0,
			-7,
			-1,
			-7,
			-17
		},
		new SByte[]
		{
			-10,
			0,
			-7,
			-1,
			-7,
			-17
		},
		new SByte[]
		{
			-13,
			0,
			-7,
			-2,
			-7,
			-23
		},
		new SByte[]
		{
			-13,
			-2,
			-7,
			1,
			-7,
			-21
		},
		new SByte[]
		{
			-11,
			0,
			-8,
			-2,
			-8,
			-19
		},
		new SByte[]
		{
			-9,
			0,
			-6,
			-1,
			-6,
			-21
		},
		new SByte[]
		{
			-12,
			0,
			-8,
			-2,
			-8,
			-19
		},
		new SByte[]
		{
			-10,
			0,
			-8,
			-2,
			-8,
			-18
		},
		new SByte[]
		{
			-10,
			0,
			-6,
			-1,
			-6,
			-18
		}
	};

	public static SByte[][] wZofsPC;

	public class STAT_CNT_TBL
	{
		public STAT_CNT_TBL(BattleStatus mask, Int16 idx, UInt16 col)
		{
			this.Mask = mask;
			this.Idx = idx;
			this.Col = col;
		}

		public BattleStatus Mask;

		public Int16 Idx;

		public UInt16 Col;
	}

	public class STAT_ICON_TBL
	{
		public STAT_ICON_TBL(BattleStatus mask, BattleStatus mask2, BinaryReader spr, Byte pos, Byte abr, Byte type, Byte ang)
		{
			this.Mask = mask;
			this.Mask2 = mask2;
			this.Spr = spr;
			this.Pos = pos;
			this.Abr = abr;
			this.Type = type;
			this.Ang = ang;
			this.texture = null;
		}

		public BattleStatus Mask;

		public BattleStatus Mask2;

		public BinaryReader Spr;

		public Byte Pos;

		public Byte Abr;

		public Byte Type;

		public Byte Ang;

		public Texture2D texture;
	}

	public class S_InShpWork
	{
		public UInt32 rgbcode;

		public Int32 sx;

		public Int32 sy;

		public Int32 abr;

		public Int32 clut;

		public Int32 prim;

		public Int32 otadd;
	}

	public class S_InSpsWork
	{
		public BinaryReader pt;

		public BinaryReader rgb;

		public Int32 w;

		public Int32 h;

		public Int32 tpage;

		public Int32 clut;

		public Int32 fade;

		public Int32 prim;

		public Int32 otadd;

		public Int32 code;
	}
}
