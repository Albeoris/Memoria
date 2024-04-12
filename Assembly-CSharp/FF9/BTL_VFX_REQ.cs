using Memoria.Data;
using System;

namespace FF9
{
	public class BTL_VFX_REQ
	{
		public BTL_VFX_REQ()
		{
			this.trg = new BTL_DATA[8];
			this.monbone = new Byte[2];
			this.rtrg = new BTL_DATA[4];
		}

		public void PlaySFX(SpecialEffect fx_no)
		{
			SFX.Begin(flgs, arg0, monbone, trgcpos);
			SFX.SetExe(exe);
			SFX.SetMExe(mexe);
			SFX.SetTrg(trg, trgno);
			SFX.SetRTrg(rtrg, rtrgno);
			SFX.Play(fx_no);
		}

		public void SetupVfxRequest(CMD_DATA cmd, Int16[] arg = null, BTL_DATA caster = null, Int32 targ = -1)
		{
			if (caster == null)
				caster = cmd.regist;
			UInt16 reflect_tar = 0;
			this.exe = caster;
			this.mexe = null;
			if (arg != null)
			{
				this.monbone[0] = (Byte)arg[0];
				this.monbone[1] = (Byte)arg[1];
				this.arg0 = (Int16)arg[2];
				this.flgs = (UInt16)arg[3];
			}
			else
			{
				this.monbone[0] = this.monbone[1] = 0;
				this.flgs = 0;
				this.arg0 = 0;
				if (!cmd.info.HasCheckedReflect)
				{
					if (cmd.cmd_no == BattleCommandId.Item || cmd.cmd_no == BattleCommandId.AutoPotion)
						this.flgs |= 2;
					else if (cmd.cmd_no == BattleCommandId.MagicSword)
						this.flgs |= 4;
					else if (cmd.cmd_no == BattleCommandId.SysPhantom || cmd.cmd_no == BattleCommandId.SysLastPhoenix)
						this.flgs |= 1;
				}
			}
			this.trgno = this.rtrgno = 0;
			if (!cmd.info.HasCheckedReflect)
				reflect_tar = btl_cmd.CheckReflec(cmd);
			if (targ < 0)
				targ = cmd.tar_id;
			for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
			{
				if ((next.btl_id & targ) != 0)
					this.trg[this.trgno++] = next;
				if ((next.btl_id & reflect_tar) != 0)
					this.rtrg[this.rtrgno++] = next;
				if ((this.flgs & 4) != 0 && btl_util.getSerialNumber(next) == CharacterSerialNumber.VIVI)
					this.mexe = next;
			}
			this.useTargetAveragePosition = true;
			UpdateTargetAveragePosition();
		}

		public void UpdateTargetAveragePosition()
		{
			if (!this.useTargetAveragePosition)
				return;
			if (this.trgno == 0)
			{
				this.trgcpos.vx = 0;
				this.trgcpos.vy = 0;
				this.trgcpos.vz = 0;
				return;
			}
			Int32 vx = 0, vz = 0;
			for (Int32 i = 0; i < this.trgno; i++)
			{
				vx += (Int32)this.trg[i].base_pos[0];
				vz += (Int32)this.trg[i].base_pos[2];
			}
			this.trgcpos.vx = vx / this.trgno;
			this.trgcpos.vy = 0;
			this.trgcpos.vz = vz / this.trgno;
		}

		public const UInt16 BTL_REQ_TRG_MAX = 8;

		public const UInt16 BTL_REQ_RTRG_MAX = 4;

		public BTL_DATA exe;

		public BTL_DATA[] trg;

		public SByte trgno;

		public Byte[] monbone;

		public SByte rtrgno;

		public BTL_DATA[] rtrg;

		public PSX_LIBGTE.VECTOR trgcpos;

		public UInt16 flgs;

		public Int16 arg0;

		public BTL_DATA mexe;

		public Boolean useTargetAveragePosition;
	}
}
