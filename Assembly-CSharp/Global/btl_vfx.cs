using System;
using FF9;
using Memoria;
using Memoria.Data;
using UnityEngine;
using Object = System.Object;

public class btl_vfx
{
	public static void SetBattleVfx(CMD_DATA cmd, UInt32 fx_no, Int16[] arg)
	{
		BTL_VFX_REQ fx_req = FF9StateSystem.Battle.FF9Battle.fx_req;
		PSX_LIBGTE.VECTOR vector = default(PSX_LIBGTE.VECTOR);
		vector.vx = 0;
		vector.vy = 0;
		vector.vz = 0;
		UInt16 num = 0;
		fx_req.exe = cmd.regist;
		if (arg != null)
		{
			fx_req.monbone[0] = (Byte)arg[0];
			fx_req.monbone[1] = (Byte)arg[1];
			fx_req.arg0 = (Int16)((Byte)arg[2]);
			fx_req.flgs = (UInt16)((Byte)arg[3]);
		}
		else
		{
			fx_req.monbone[0] = (fx_req.monbone[1] = 0);
			fx_req.flgs = 0;
			fx_req.arg0 = 0;
		}
		fx_req.trgno = (fx_req.rtrgno = 0);
		cmd.regist.fig = (cmd.regist.m_fig = 0);
		cmd.regist.fig_info = 0;
		if (cmd.info.reflec == 0)
		{
			num = btl_cmd.CheckReflec(cmd);
			if (cmd.cmd_no == BattleCommandId.Item || cmd.cmd_no == BattleCommandId.AutoPotion)
			{
				fx_req.flgs = 2;
			}
			else if (cmd.cmd_no == BattleCommandId.MagicSword)
			{
				fx_req.flgs = 4;
			}
			else if (cmd.cmd_no == BattleCommandId.SysPhantom || cmd.cmd_no == BattleCommandId.SysLastPhoenix)
			{
				fx_req.flgs = 1;
			}
		}
		else if (cmd.info.reflec == 2)
		{
			cmd.info.reflec = 1;
			if (cmd.regist.bi.player == 0)
			{
				cmd.info.mon_reflec = 1;
			}
			fx_req.flgs = 17;
		}
		for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
		{
			if ((next.btl_id & cmd.tar_id) != 0)
			{
				BTL_DATA[] trg = fx_req.trg;
				BTL_VFX_REQ btl_VFX_REQ = fx_req;
				SByte b;
				btl_VFX_REQ.trgno = (SByte)((b = btl_VFX_REQ.trgno) + 1);
				trg[(Int32)((Byte)b)] = next;
				next.fig = (next.m_fig = 0);
				next.fig_info = 0;
				vector.vx += (Int32)next.base_pos[0];
				vector.vz += (Int32)next.base_pos[2];
			}
			if ((next.btl_id & num) != 0)
			{
				BTL_DATA[] rtrg = fx_req.rtrg;
				BTL_VFX_REQ btl_VFX_REQ2 = fx_req;
				SByte b;
				btl_VFX_REQ2.rtrgno = (SByte)((b = btl_VFX_REQ2.rtrgno) + 1);
				rtrg[(Int32)((Byte)b)] = next;
			}
			if (cmd.cmd_no == BattleCommandId.MagicSword && btl_util.getSerialNumber(next) == 2)
			{
				fx_req.mexe = next;
			}
		}
		fx_req.trgcpos.vx = vector.vx / (Int32)fx_req.trgno;
		fx_req.trgcpos.vy = 0;
		fx_req.trgcpos.vz = vector.vz / (Int32)fx_req.trgno;
		ENEMY_TYPE[] enemy_type = FF9StateSystem.Battle.FF9Battle.enemy_type;
		SFX.Begin(fx_req.flgs, fx_req.arg0, fx_req.monbone, fx_req.trgcpos);
		SFX.SetExe(fx_req.exe);
		SFX.SetMExe(fx_req.mexe);
		SFX.SetTrg(fx_req.trg, fx_req.trgno);
		SFX.SetRTrg(fx_req.rtrg, fx_req.rtrgno);
		SFX.Play((Int32)fx_no);
	}

    public static Boolean UseBeatrixAlternateVfx(BattleUnit caster, Int16 vfx1, UInt16 vfx2)
    {
        // Check if vfx1 and vfx2 are the two versions of a Sword Art spell animation: use the 2nd version when used by Beatrix
        if (!caster.IsPlayer || caster.PlayerIndex != CharacterIndex.Beatrix) return false;
        if (vfx1 == 188u && vfx2 == 409u) return true; // Darkside
        if (vfx1 == 189u && vfx2 == 410u) return true; // Minus Strike
        if (vfx1 == 190u && vfx2 == 411u) return true; // Iai Strike
        if (vfx1 == 191u && vfx2 == 412u) return true; // Thunder Slash
        if (vfx1 == 192u && vfx2 == 413u) return true; // Shock
        if (vfx1 == 207u && vfx2 == 414u) return true; // Stock Break
        if (vfx1 == 397u && vfx2 == 417u) return true; // Climhazzard
        return false;
    }

    public static void SelectCommandVfx(CMD_DATA cmd)
    {
        BTL_DATA regist = cmd.regist;
        BattleUnit caster = regist == null ? null : new BattleUnit(regist);
        BattleCommandId cmd_no = cmd.cmd_no;
        switch (cmd_no)
        {
            case BattleCommandId.EnemyAtk:
                btlseq.RunSequence(cmd);
                return;
            case BattleCommandId.AutoPotion:
                SetBattleVfx(cmd, (UInt32)ff9item._FF9Item_Info[btl_util.btlItemNum(cmd.sub_no)].info.VfxIndex, null);
                return;
            case BattleCommandId.EnemyCounter:
            case BattleCommandId.EnemyDying:
            case BattleCommandId.EnemyReaction:
                btlseq.RunSequence(cmd);
                return;
            case BattleCommandId.SysTrans:
                SetBattleVfx(cmd, !caster.IsUnderStatus(BattleStatus.Trance) ? 489u : 257u, null);
                return;
            case BattleCommandId.Attack:
                SetBattleVfx(cmd, (UInt32)(100 + btl_util.getSerialNumber(regist)), null);
                return;
            case BattleCommandId.Item:
                SetBattleVfx(cmd, (UInt32)ff9item._FF9Item_Info[btl_util.btlItemNum(cmd.sub_no)].info.VfxIndex, null);
                return;
            case BattleCommandId.Defend:
                UIManager.Battle.SetBattleCommandTitle(cmd);
                btl_mot.setMotion(regist, 12);
                regist.evt.animFrame = 0;
                btl_cmd.ExecVfxCommand(regist);
                return;
            case BattleCommandId.Change:
                UIManager.Battle.SetBattleCommandTitle(cmd);
                btl_mot.setMotion(regist, 9);
                return;
            case BattleCommandId.Steal:
            {
                Byte serialNumber = btl_util.getSerialNumber(regist);
                if (serialNumber == 0)
                {
                    SetBattleVfx(cmd, 200u, null);
                }
                else if (serialNumber == 1)
                {
                    SetBattleVfx(cmd, 273u, null);
                }
                else if (serialNumber == 14)
                {
                    SetBattleVfx(cmd, 19u, null);
                }
                else if (serialNumber == 15)
                    SetBattleVfx(cmd, 119u, null);
                else
                    SetBattleVfx(cmd, 20u, null);
                return;
            }
            case BattleCommandId.Throw:
                switch (ff9item._FF9Item_Data[(Int32)cmd.sub_no].shape)
                {
                    case 1:
                        SetBattleVfx(cmd, 272u, null);
                        break;
                    case 2:
                        SetBattleVfx(cmd, 266u, null);
                        break;
                    case 3:
                    case 4:
                        SetBattleVfx(cmd, 267u, null);
                        break;
                    case 5:
                        SetBattleVfx(cmd, 268u, null);
                        break;
                    case 6:
                        SetBattleVfx(cmd, 269u, null);
                        break;
                    case 7:
                        SetBattleVfx(cmd, 265u, null);
                        break;
                    case 8:
                    case 9:
                    case 10:
                        SetBattleVfx(cmd, 270u, null);
                        break;
                    case 11:
                        SetBattleVfx(cmd, 271u, null);
                        break;
                    case 12:
                        SetBattleVfx(cmd, 277u, null);
                        break;
                }
                return;
            case BattleCommandId.Phantom:
            {
                FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
                switch (cmd.sub_no)
                {
                    case 49:
                        ff9Battle.phantom_no = 153;
                        break;
                    case 51:
                        ff9Battle.phantom_no = 154;
                        break;
                    case 53:
                        ff9Battle.phantom_no = 155;
                        break;
                    case 55:
                        ff9Battle.phantom_no = 156;
                        break;
                    case 58:
                        ff9Battle.phantom_no = 157;
                        break;
                    case 60:
                        ff9Battle.phantom_no = 158;
                        break;
                    case 62:
                        ff9Battle.phantom_no = 159;
                        break;
                    case 64:
                        ff9Battle.phantom_no = 160;
                        break;
                }

                if ((cmd.aa.Info.Target == TargetType.ManyAny && cmd.info.cursor == 0) || cmd.info.meteor_miss != 0 || cmd.info.short_summon != 0 || btl_vfx.UseBeatrixAlternateVfx(caster, cmd.aa.Info.VfxIndex, cmd.aa.Vfx2))
                    SetBattleVfx(cmd, cmd.aa.Vfx2, null);
                else
                    SetBattleVfx(cmd, (UInt32)cmd.aa.Info.VfxIndex, null);

                return;
            }
            case BattleCommandId.BoundaryCheck:
            case BattleCommandId.MagicCounter:
            case BattleCommandId.SysEscape:
            case BattleCommandId.SysPhantom:
            case BattleCommandId.SysLastPhoenix:
            case BattleCommandId.Jump:
            case BattleCommandId.Escape:
            case BattleCommandId.FinishBlow:
            case BattleCommandId.Counter:
            case BattleCommandId.RushAttack:
            default:
                if (cmd_no != BattleCommandId.MagicCounter && cmd.sub_no == 176)
                    SetBattleVfx(cmd, (UInt32)(100 + btl_util.getSerialNumber(regist)), null);
                else if ((cmd.aa.Info.Target == TargetType.ManyAny && cmd.info.cursor == 0) || cmd.info.meteor_miss != 0 || cmd.info.short_summon != 0 || btl_vfx.UseBeatrixAlternateVfx(caster, cmd.aa.Info.VfxIndex, cmd.aa.Vfx2))
                    SetBattleVfx(cmd, cmd.aa.Vfx2, null);
                else
                    SetBattleVfx(cmd, (UInt32)cmd.aa.Info.VfxIndex, null);
                return;
        }
    }

    public static void SetTranceModel(BTL_DATA btl, Boolean isTrance)
	{
		Vector3 pos = btl.pos;
		Vector3 eulerAngles = btl.rot.eulerAngles;
		Byte b = btl_util.getSerialNumber(btl);
		if (isTrance && (Int32)(b + 19) >= (Int32)btl_init.model_id.Length)
		{
			return;
		}
		if (isTrance)
		{
			btl.battleModelIsRendering = true;
			btl.tranceGo.SetActive(true);
			btl.gameObject = btl.tranceGo;
			GeoTexAnim.geoTexAnimPlay(btl.tranceTexanimptr, 2);
		}
		else
		{
			btl.battleModelIsRendering = true;
			btl.originalGo.SetActive(true);
			btl.tranceGo.SetActive(false);
			btl.gameObject = btl.originalGo;
			btl.dms_geo_id = btl_init.GetModelID((Int32)btl_util.getSerialNumber(btl));
			GeoTexAnim.geoTexAnimPlay(btl.texanimptr, 2);
		}
		Int32 num = 0;
		foreach (Object obj in btl.gameObject.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.name.Contains("mesh"))
			{
				num++;
			}
		}
		btl.meshIsRendering = new Boolean[num];
		for (Int32 i = 0; i < num; i++)
		{
			btl.meshIsRendering[i] = true;
		}
		btl.meshCount = num;
		btl_util.GeoSetABR(btl.gameObject, "PSX/BattleMap_StatusEffect");
		BattlePlayerCharacter.InitAnimation(btl);
		btl_mot.setMotion(btl, 0);
		btl_eqp.InitWeapon(FF9StateSystem.Common.FF9.player[(Int32)btl.bi.slot_no], btl);
		if (b == 7)
		{
			b = 8;
		}
		AnimationFactory.AddAnimToGameObject(btl.gameObject, btl_init.model_id[(Int32)b], true);
	}
}
