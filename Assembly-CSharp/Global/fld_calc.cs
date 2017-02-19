using System;
using FF9;

public class fld_calc
{
	public Boolean FieldCalcMain(PLAYER caster, PLAYER target, AA_DATA tbl, Byte prog_no, UInt32 cursor)
	{
		fld_calc.ITEM_AA_DATA tbl2 = new fld_calc.ITEM_AA_DATA(tbl);
		return this.FieldCalcMain(caster, target, tbl2, prog_no, cursor);
	}

	public Boolean FieldCalcMain(PLAYER caster, PLAYER target, ITEM_DATA tbl, Byte prog_no, UInt32 cursor)
	{
		fld_calc.ITEM_AA_DATA tbl2 = new fld_calc.ITEM_AA_DATA(tbl);
		return this.FieldCalcMain(caster, target, tbl2, prog_no, cursor);
	}

	private Boolean FieldCalcMain(PLAYER caster, PLAYER target, fld_calc.ITEM_AA_DATA tbl, Byte prog_no, UInt32 cursor)
	{
		fld_calc.FCALC_VAR fcalc_VAR = new fld_calc.FCALC_VAR();
		fcalc_VAR.caster = caster;
		fcalc_VAR.target = target;
		fcalc_VAR.tbl = tbl;
		fcalc_VAR.cursor = cursor;
		fcalc_VAR.flags = 0;
		fcalc_VAR.tg_hp = (fcalc_VAR.tg_mp = 0);
		switch (prog_no)
		{
		case 62:
		case 73:
			if (tbl.Info.vfx_no == 289)
			{
				return PersistenSingleton<UIManager>.Instance.ItemScene.FF9FItem_Vegetable();
			}
			this.FldCalcSub_305(fcalc_VAR);
			break;
		case 63:
		case 65:
		case 66:
		case 67:
		case 68:
			IL_82:
			switch (prog_no)
			{
			case 10:
				if (this.FldCalcSub_12A(fcalc_VAR))
				{
					if (target.cur.hp == target.max.hp)
					{
						fld_calc.FCALC_VAR fcalc_VAR2 = fcalc_VAR;
						fcalc_VAR2.flags = (Byte)(fcalc_VAR2.flags | 1);
					}
					else
					{
						fld_calc.FldCalcSub_137(fcalc_VAR);
						this.FldCalcSub_146(fcalc_VAR);
						this.FldCalcSub_144(fcalc_VAR);
						this.FldCalcSub_156(fcalc_VAR);
						fld_calc.FldCalcSub_202(fcalc_VAR);
					}
				}
				break;
			case 12:
				this.FldCalcSub_302(fcalc_VAR);
				break;
			case 13:
				if (this.FldCalcSub_12B(fcalc_VAR))
				{
					fld_calc.FldCalcSub_204(fcalc_VAR);
				}
				break;
			}
			break;
		case 64:
			break;
		case 69:
			if (this.FldCalcSub_12A(fcalc_VAR))
			{
				if (target.cur.hp == target.max.hp)
				{
					fld_calc.FCALC_VAR fcalc_VAR3 = fcalc_VAR;
					fcalc_VAR3.flags = (Byte)(fcalc_VAR3.flags | 1);
				}
				else
				{
					fld_calc.FldCalcSub_171(fcalc_VAR);
					fld_calc.FldCalcSub_202(fcalc_VAR);
				}
			}
			break;
		case 70:
			if (this.FldCalcSub_12A(fcalc_VAR))
			{
				if (target.cur.mp == target.max.mp)
				{
					fld_calc.FCALC_VAR fcalc_VAR4 = fcalc_VAR;
					fcalc_VAR4.flags = (Byte)(fcalc_VAR4.flags | 1);
				}
				else
				{
					fld_calc.FldCalcSub_171(fcalc_VAR);
					fld_calc.FldCalcSub_21E(fcalc_VAR);
				}
			}
			break;
		case 71:
			if (this.FldCalcSub_12A(fcalc_VAR))
			{
				if (target.cur.hp == target.max.hp && target.cur.mp == target.max.mp)
				{
					fld_calc.FCALC_VAR fcalc_VAR5 = fcalc_VAR;
					fcalc_VAR5.flags = (Byte)(fcalc_VAR5.flags | 1);
				}
				else
				{
					fld_calc.FldCalcSub_21F(fcalc_VAR);
				}
			}
			break;
		case 72:
			if (this.FldCalcSub_12B(fcalc_VAR))
			{
				fld_calc.FldCalcSub_220(fcalc_VAR);
			}
			break;
		case 74:
			return PersistenSingleton<UIManager>.Instance.ItemScene.FF9FItem_Vegetable();
		case 75:
			break;
		case 76:
			if (this.FldCalcSub_12A(fcalc_VAR))
			{
				fld_calc.FldCalcSub_223(fcalc_VAR);
			}
			break;
		default:
			goto IL_82;
		}
		return this.FieldCalcResult(fcalc_VAR);
	}

	private Boolean FieldCalcResult(fld_calc.FCALC_VAR v)
	{
		if ((v.flags & 1) != 0)
		{
			v.tg_info = (Byte)(v.tg_info | 32);
			return false;
		}
		if (v.tg_hp > 0)
		{
			this.FieldSetRecover(v.target, (Int32)v.tg_hp);
		}
		if (v.tg_mp > 0)
		{
			this.FieldSetMpRecover(v.target, (Int32)v.tg_mp);
		}
		return true;
	}

	private Boolean FieldConsumeMp(PLAYER btl, Int16 mp)
	{
		if (btl.cur.mp < mp)
		{
			return false;
		}
		POINTS cur = btl.cur;
		cur.mp = (Int16)(cur.mp - mp);
		return true;
	}

	private Boolean FldCalcSub_12A(fld_calc.FCALC_VAR v)
	{
		if (this.FieldCheckStatus(v.target, 65) || v.target.cur.hp == 0)
		{
			v.flags = (Byte)(v.flags | 1);
			return false;
		}
		return true;
	}

	private Boolean FldCalcSub_12B(fld_calc.FCALC_VAR v)
	{
		if (this.FieldCheckStatus(v.target, 1) || v.target.cur.hp > 0 || (this.FieldCheckStatus(v.target, 64) && v.target.cur.hp == 0))
		{
			v.flags = (Byte)(v.flags | 1);
			return false;
		}
		return true;
	}

	private static void FldCalcSub_137(fld_calc.FCALC_VAR v)
	{
		v.at_num = (Int16)((Int32)v.caster.elem.mgc + Comn.random16() % (1 + (v.caster.level + v.caster.elem.mgc >> 3)));
		v.at_pow = (Int16)v.tbl.Ref.power;
		v.df_pow = (Int16)v.target.defence.m_def;
	}

	private static void FldCalcSub_171(fld_calc.FCALC_VAR v)
	{
		v.at_num = 10;
		v.at_pow = (Int16)v.tbl.Ref.power;
		v.df_pow = 0;
	}

	private void FldCalcSub_144(fld_calc.FCALC_VAR v)
	{
		if (this.FieldCheckStatus(v.caster, 0))
		{
			v.at_num = (Int16)(v.at_num / 2);
		}
	}

	private void FldCalcSub_146(fld_calc.FCALC_VAR v)
	{
		if ((v.caster.sa[1] & 2u) != 0u)
		{
			v.at_num = (Int16)(v.at_num * 3 >> 1);
		}
	}

	private void FldCalcSub_156(fld_calc.FCALC_VAR v)
	{
		if (v.cursor == 1u && v.tbl.Info.cursor > 2 && v.tbl.Info.cursor < 6)
		{
			v.at_num = (Int16)(v.at_num / 2);
		}
	}

	private static void FldCalcSub_202(fld_calc.FCALC_VAR v)
	{
		Int16 num = (Int16)(v.at_pow * v.at_num);
		if (num > 9999)
		{
			num = 9999;
		}
		v.tg_hp = num;
	}

	private static void FldCalcSub_204(fld_calc.FCALC_VAR v)
	{
		Int32 num = (Int32)(v.target.max.hp * (UInt16)(v.target.elem.wpr + v.tbl.Ref.power));
		if ((v.caster.sa[1] & 2u) != 0u)
		{
			num /= 50;
		}
		else
		{
			num /= 100;
		}
		if (num > 9999)
		{
			num = 9999;
		}
		v.tg_hp = (Int16)num;
	}

	private static void FldCalcSub_21E(fld_calc.FCALC_VAR v)
	{
		Int16 num = (Int16)(v.at_pow * v.at_num);
		if (num > 9999)
		{
			num = 9999;
		}
		v.tg_mp = num;
	}

	private static void FldCalcSub_21F(fld_calc.FCALC_VAR v)
	{
		v.target.cur.hp = v.target.max.hp;
		v.target.cur.mp = v.target.max.mp;
	}

	private static void FldCalcSub_220(fld_calc.FCALC_VAR v)
	{
		v.target.cur.hp = (UInt16)(1 + Comn.random8() % 10);
	}

	private static void FldCalcSub_223(fld_calc.FCALC_VAR v)
	{
		v.tg_hp = (Int16)(v.target.max.hp >> 1);
		v.tg_mp = (Int16)(v.target.max.mp >> 1);
	}

	private void FldCalcSub_302(fld_calc.FCALC_VAR v)
	{
		Byte[] array = new Byte[]
		{
			0,
			0,
			0,
			2,
			1,
			24
		};
		if (this.FieldRemoveStatuses(v.target, array[(Int32)v.tbl.AddNo]) != 2u)
		{
			v.flags = (Byte)(v.flags | 1);
		}
	}

	private void FldCalcSub_305(fld_calc.FCALC_VAR v)
	{
		if (this.FieldRemoveStatuses(v.target, (Byte)v.tbl.status) != 2u)
		{
			v.flags = (Byte)(v.flags | 1);
		}
	}

	private void FieldSetRecover(PLAYER player, Int32 recover)
	{
		if (!this.FieldCheckStatus(player, 1))
		{
			POINTS cur = player.cur;
			cur.hp = (UInt16)(cur.hp + (UInt16)recover);
			if (player.cur.hp > player.max.hp)
			{
				player.cur.hp = player.max.hp;
			}
		}
	}

	private void FieldSetMpRecover(PLAYER player, Int32 recover)
	{
		if (!this.FieldCheckStatus(player, 1))
		{
			POINTS cur = player.cur;
			cur.mp = (Int16)(cur.mp + (Int16)recover);
			if (player.cur.mp > player.max.mp)
			{
				player.cur.mp = player.max.mp;
			}
		}
	}

	public UInt32 FieldRemoveStatus(PLAYER player, Byte status)
	{
		if ((player.status & status) != 0)
		{
			player.status = (Byte)(player.status & (Byte)(~status));
			return 2u;
		}
		return 1u;
	}

	private UInt32 FieldRemoveStatuses(PLAYER player, Byte statuses)
	{
		UInt32 result = 1u;
		for (Int32 i = 0; i < 8; i++)
		{
			Byte b = (Byte)(1 << i);
			if ((statuses & b) != 0 && this.FieldRemoveStatus(player, b) == 2u)
			{
				result = 2u;
			}
		}
		return result;
	}

	private Boolean FieldCheckStatus(PLAYER player, Byte status)
	{
		return (player.status & status) != 0;
	}

	public class ITEM_AA_DATA
	{
		public ITEM_AA_DATA(ITEM_DATA item)
		{
			this.Info = item.info;
			this.Ref = item.Ref;
			this.status = item.status;
		}

		public ITEM_AA_DATA(AA_DATA aa)
		{
			this.Info = aa.Info;
			this.Ref = aa.Ref;
			this.Category = aa.Category;
			this.AddNo = aa.AddNo;
			this.MP = aa.MP;
			this.Type = aa.Type;
			this.Vfx2 = aa.Vfx2;
			this.Name = aa.Name;
		}

		public CMD_INFO Info;

		public BTL_REF Ref;

		public Byte Category;

		public Byte AddNo;

		public Byte MP;

		public Byte Type;

		public UInt16 Vfx2;

		public String Name;

		public UInt32 status;
	}

	public class FCALC_VAR
	{
		public PLAYER caster;

		public PLAYER target;

		public fld_calc.ITEM_AA_DATA tbl;

		public UInt32 cursor;

		public Byte flags;

		public Byte tg_info;

		public Int16 at_pow;

		public Int16 df_pow;

		public Int16 at_num;

		public Int16 tg_hp;

		public Int16 tg_mp;
	}
}
