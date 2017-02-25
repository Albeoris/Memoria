using System;
using FF9;
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
			if (cmd.cmd_no == 14 || cmd.cmd_no == 51)
			{
				fx_req.flgs = 2;
			}
			else if (cmd.cmd_no == 31)
			{
				fx_req.flgs = 4;
			}
			else if (cmd.cmd_no == 57 || cmd.cmd_no == 58)
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
			if (cmd.cmd_no == 31 && btl_util.getSerialNumber(next) == 2)
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

	public static void SelectCommandVfx(CMD_DATA cmd)
	{
		BTL_DATA regist = cmd.regist;
		Byte cmd_no = cmd.cmd_no;
		switch (cmd_no)
		{
		case 47:
			if (cmd.tar_id < 16 && (cmd.aa.Category & 8) != 0 && cmd.info.cursor == 0)
			{
				UInt16 num = btl_abil.CheckCoverAbility(cmd.tar_id);
				if (num != 0)
				{
					cmd.tar_id = num;
					cmd.info.cover = 1;
				}
			}
			btlseq.RunSequence(cmd);
			return;
		case 48:
		case 50:
		case 56:
		case 57:
		case 58:
			IL_4B:
			switch (cmd_no)
			{
			case 1:
				break;
			case 2:
			{
				Byte serialNumber = btl_util.getSerialNumber(regist);
				if (serialNumber != 0)
				{
					if (serialNumber != 1)
					{
						if (serialNumber != 14)
						{
							if (serialNumber != 15)
							{
								btl_vfx.SetBattleVfx(cmd, 20u, null);
							}
							else
							{
								btl_vfx.SetBattleVfx(cmd, 119u, null);
							}
						}
						else
						{
							btl_vfx.SetBattleVfx(cmd, 19u, null);
						}
					}
					else
					{
						btl_vfx.SetBattleVfx(cmd, 273u, null);
					}
				}
				else
				{
					btl_vfx.SetBattleVfx(cmd, 200u, null);
				}
				return;
			}
			case 3:
			case 5:
			case 6:
				IL_6F:
				switch (cmd_no)
				{
				case 14:
					goto IL_168;
				case 15:
					switch (ff9item._FF9Item_Data[(Int32)cmd.sub_no].shape)
					{
					case 1:
						btl_vfx.SetBattleVfx(cmd, 272u, null);
						break;
					case 2:
						btl_vfx.SetBattleVfx(cmd, 266u, null);
						break;
					case 3:
					case 4:
						btl_vfx.SetBattleVfx(cmd, 267u, null);
						break;
					case 5:
						btl_vfx.SetBattleVfx(cmd, 268u, null);
						break;
					case 6:
						btl_vfx.SetBattleVfx(cmd, 269u, null);
						break;
					case 7:
						btl_vfx.SetBattleVfx(cmd, 265u, null);
						break;
					case 8:
					case 9:
					case 10:
						btl_vfx.SetBattleVfx(cmd, 270u, null);
						break;
					case 11:
						btl_vfx.SetBattleVfx(cmd, 271u, null);
						break;
					case 12:
						btl_vfx.SetBattleVfx(cmd, 277u, null);
						break;
					}
					return;
				case 18:
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
					if ((cmd.aa.Info.Target ==  Memoria.Data.TargetType.ManyAny && cmd.info.cursor == 0) || cmd.info.meteor_miss != 0 || cmd.info.short_summon != 0 || cmd.cmd_no == 32 || cmd.cmd_no == 33)
					{
						btl_vfx.SetBattleVfx(cmd, (UInt32)cmd.aa.Vfx2, null);
					}
					else
					{
						btl_vfx.SetBattleVfx(cmd, (UInt32)cmd.aa.Info.VfxIndex, null);
					}
					return;
				}
				}
				if ((cmd.aa.Info.Target == Memoria.Data.TargetType.ManyAny && cmd.info.cursor == 0) || cmd.info.meteor_miss != 0 || cmd.info.short_summon != 0 || cmd.cmd_no == 32 || cmd.cmd_no == 33)
				{
					btl_vfx.SetBattleVfx(cmd, (UInt32)cmd.aa.Vfx2, null);
				}
				else
				{
					btl_vfx.SetBattleVfx(cmd, (UInt32)cmd.aa.Info.VfxIndex, null);
				}
				return;
			case 4:
				UIManager.Battle.SetBattleCommandTitle(cmd);
				btl_mot.setMotion(regist, 12);
				regist.evt.animFrame = 0;
				btl_cmd.ExecVfxCommand(regist);
				return;
			case 7:
				UIManager.Battle.SetBattleCommandTitle(cmd);
				btl_mot.setMotion(regist, 9);
				return;
			default:
				goto IL_6F;
			}
			break;
		case 49:
		case 52:
			break;
		case 51:
			goto IL_168;
		case 53:
		case 54:
		case 55:
			btlseq.RunSequence(cmd);
			return;
		case 59:
			btl_vfx.SetBattleVfx(cmd, (UInt32)((!Status.checkCurStat(regist, 16384u)) ? 489u : 257u), null);
			return;
		default:
			goto IL_4B;
		}
		btl_vfx.SetBattleVfx(cmd, (UInt32)(100 + btl_util.getSerialNumber(regist)), null);
		return;
		IL_168:
		btl_vfx.SetBattleVfx(cmd, (UInt32)ff9item._FF9Item_Info[btl_util.btlItemNum((Int32)cmd.sub_no)].info.VfxIndex, null);
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
		AnimationFactory.AddAnimToGameObject(btl.gameObject, btl_init.model_id[(Int32)b]);
	}
}
