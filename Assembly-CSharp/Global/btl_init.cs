using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using UnityEngine;

public class btl_init
{
	public static Int16 GetModelID(Int32 characterModelIndex)
	{
	    String[] playerModels = model_id;

        if (characterModelIndex < 0 || characterModelIndex > playerModels.Length - 1)
        {
            Log.Warning("[btl_init] Invalid serial number: {0}", characterModelIndex);
			return 0;
		}

	    String modelId = playerModels[characterModelIndex];

        Int32 geoId;
	    if (FF9BattleDB.GEO.TryGetKey(modelId, out geoId))
	        return (Int16)geoId;

	    Log.Warning("[btl_init] Unknown model id: {0}", modelId);
        return 0;
	}

	public static void InitEnemyData(FF9StateBattleSystem btlsys)
	{
		BTL_DATA btl_DATA = null;
		ObjList objList = new ObjList();
		if (!FF9StateSystem.Battle.isDebug)
		{
			objList = PersistenSingleton<EventEngine>.Instance.GetActiveObjList().next;
		}
		Int32 monCount = FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].MonCount;
		Int32 i = 0;
		Int32 j = 4;
		while (i < monCount)
		{
			ENEMY enemy = btlsys.enemy[i];
			BTL_DATA btl_DATA2 = btlsys.btl_data[j];
			enemy.info.die_fade_rate = 32;
			if ((btl_DATA2.dms_geo_id = BTL_SCENE.GetMonGeoID(i)) < 0)
			{
				enemy.info.slave = 1;
			}
			else
			{
				btl_init.SetBattleModel(btl_DATA2);
				enemy.info.slave = 0;
				if (!FF9StateSystem.Battle.isDebug)
				{
					objList = objList.next;
				}
			}
			btl_DATA2.btl_id = (UInt16)(16 << i);
			btl_DATA2.bi.player = 0;
			btl_DATA2.bi.slot_no = (Byte)i;
			btl_DATA2.bi.line_no = (Byte)(4 + i);
			btl_DATA2.bi.t_gauge = 0;
			btl_DATA2.bi.slave = enemy.info.slave;
			BTL_SCENE btl_scene = FF9StateSystem.Battle.FF9Battle.btl_scene;
			SB2_PATTERN sb2_PATTERN = btl_scene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum];
			SB2_MON_PARM sb2_MON_PARM = btl_scene.MonAddr[sb2_PATTERN.Put[i].TypeNo];
			UInt16 geoID = sb2_MON_PARM.Geo;
			btl_DATA2.height = 0;
			btl_DATA2.radius = 0;
			FF9Char ff9char = new FF9Char();
			btl_init.InitBattleData(btl_DATA2, ff9char);
			btl_DATA2.bi.def_idle = 0;
			btl_DATA2.base_pos = enemy.base_pos;
			String path = (btl_DATA2.dms_geo_id == -1) ? String.Empty : FF9BattleDB.GEO.GetValue(btl_DATA2.dms_geo_id);
			if (!ModelFactory.IsUseAsEnemyCharacter(path))
			{
				btl_DATA2.weapon_geo = null;
			}
			btl_DATA2.sa = btl_init.enemy_dummy_sa; // Might want to use "= new UInt32[2]" for letting enemies use SA... doesn't seem to be useful for now though

		    FF9BattleDBHeightAndRadius.TryFindHeightAndRadius(geoID, ref btl_DATA2.height, ref btl_DATA2.radius);
            
			if (btl_DATA != null)
			{
				btl_DATA.next = btl_DATA2;
			}
			btl_DATA = btl_DATA2;
			i++;
			j++;
		}
		while (j < 8)
		{
			btlsys.btl_data[j].btl_id = 0;
			j++;
		}
		btl_DATA.next = null;
		btlsys.btl_list.next = btlsys.btl_data[4];
		btlsys.btl_load_status = (Byte)(btlsys.btl_load_status | 8);
		btl_init.SetupBattleEnemy();
		btlseq.InitSequencer();
	}

	public static void SetupBattleEnemy()
	{
		SB2_PATTERN[] patAddr = FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr;
		SB2_HEAD header = FF9StateSystem.Battle.FF9Battle.btl_scene.header;
		BTL_SCENE btl_scene = FF9StateSystem.Battle.FF9Battle.btl_scene;
		if (FF9StateSystem.Battle.isDebug)
		{
			btl_scene.Info.StartType = FF9StateSystem.Battle.debugStartType;
		}
		else
		{
			btl_scene.Info.StartType = btl_sys.StartType(btl_scene.Info);
		}
		SB2_MON_PARM[] monAddr = FF9StateSystem.Battle.FF9Battle.btl_scene.MonAddr;
		AA_DATA[] atk = FF9StateSystem.Battle.FF9Battle.btl_scene.atk;
		Int16 num = (Int16)(header.TypCount + header.AtkCount);
		UInt16 num2 = 0;
		SB2_MON_PARM[] array = monAddr;
		ENEMY_TYPE[] enemy_type = FF9StateSystem.Battle.FF9Battle.enemy_type;
		Int16 num3;
		for (num3 = 0; num3 < (Int16)header.TypCount; num3 = (Int16)(num3 + 1))
		{
			btl_init.SetMonsterParameter(array[num3], ref enemy_type[num3]);
			enemy_type[num3].name = FF9TextTool.BattleText(num2);
			enemy_type[num3].mes = (Byte)num;
			num = (Int16)(num + array[num3].MesCnt);
			num2 = (UInt16)(num2 + 1);
		}
		AA_DATA[] array2 = atk;
		FF9StateSystem.Battle.FF9Battle.enemy_attack = new AA_DATA[header.AtkCount];
		AA_DATA[] enemy_attack = FF9StateSystem.Battle.FF9Battle.enemy_attack;
		for (num3 = 0; num3 < (Int16)header.AtkCount; num3 = (Int16)(num3 + 1))
		{
			btl_init.SetAttackData(ref array2[num3], ref enemy_attack[num3]);
			enemy_attack[num3].Name = FF9TextTool.BattleText(num2);
			num2 = (UInt16)(num2 + 1);
		}
		BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next;
		SB2_PATTERN sb2_PATTERN = patAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum];
		num3 = 0;
		while (num3 < sb2_PATTERN.MonCount && next != null)
		{
			btl_init.PutMonster(sb2_PATTERN.Put[num3], next, btl_scene, num3);
			btl_init.SetMonsterData(monAddr[sb2_PATTERN.Put[num3].TypeNo], next, num3);
			num3 = (Int16)(num3 + 1);
			next = next.next;
		}
	}

	public static void SetMonsterData(SB2_MON_PARM pParm, BTL_DATA pBtl, Int16 pNo)
	{
		pBtl.stat.invalid = pParm.Status[0];
		pBtl.stat.permanent = pParm.Status[1];
		pBtl.stat.cur = pParm.Status[2];
		pBtl.cur.hp = pParm.MaxHP;
		pBtl.cur.mp = pParm.MaxMP;
		pBtl.defence.PhisicalDefence = pParm.P_DP;
		pBtl.defence.PhisicalEvade = pParm.P_AV;
		pBtl.defence.MagicalDefence = pParm.M_DP;
		pBtl.defence.MagicalEvade = pParm.M_AV;
		pBtl.elem.dex = pParm.Element.dex;
		pBtl.elem.str = pParm.Element.str;
		pBtl.elem.mgc = pParm.Element.mgc;
		pBtl.elem.wpr = pParm.Element.wpr;
		pBtl.def_attr.invalid = pParm.Attr[0];
		pBtl.def_attr.absorb = pParm.Attr[1];
		pBtl.def_attr.half = pParm.Attr[2];
		pBtl.def_attr.weak = pParm.Attr[3];
		pBtl.mesh_current = pParm.Mesh[0];
		pBtl.mesh_banish = pParm.Mesh[1];
		pBtl.tar_bone = pParm.Bone[3];
		// New field "out_of_reach"
		pBtl.out_of_reach = pParm.OutOfReach < 0 ? FF9StateSystem.Battle.FF9Battle.btl_scene.Info.NoNeighboring != 0 : pParm.OutOfReach != 0;
		ENEMY enemy = FF9StateSystem.Battle.FF9Battle.enemy[pBtl.bi.slot_no];
		for (Int32 i = 0; i < pParm.StealItems.Length; i++)
		{
			enemy.steal_item[i] = pParm.StealItems[i];
		}
		enemy.steal_unsuccessful_counter = 0; // New field used for counting unsuccessful steals and force a successful steal when it becomes high enough
		enemy.info.die_atk = (Byte)(((pParm.Flags & 1) == 0) ? 0 : 1);
		enemy.info.die_dmg = (Byte)(((pParm.Flags & 2) == 0) ? 0 : 1);
		enemy.info.flags = pParm.Flags;
		btl_util.SetShadow(pBtl, pParm.ShadowX, pParm.ShadowZ);
		pBtl.shadow_bone[0] = pParm.ShadowBone;
		pBtl.shadow_bone[1] = pParm.ShadowBone2;
	}

	public static void PutMonster(SB2_PUT pPut, BTL_DATA pBtl, BTL_SCENE pScene, Int16 pNo)
	{
		Int16 startTypeAngle = (Int16)(pScene.Info.StartType == battle_start_type_tags.BTL_START_FIRST_ATTACK ? 180 : 0);
		ENEMY enemy = FF9StateSystem.Battle.FF9Battle.enemy[pBtl.bi.slot_no];
		enemy.et = FF9StateSystem.Battle.FF9Battle.enemy_type[pPut.TypeNo];
		pBtl.bi.target = (Byte)(((pPut.Flags & 1) == 0) ? 0 : 1);
		pBtl.bi.row = 2;
		pBtl.max = FF9StateSystem.Battle.FF9Battle.enemy_type[pPut.TypeNo].max;
		pBtl.cur.hp = pBtl.max.hp;
		pBtl.cur.mp = pBtl.max.mp;
		enemy.info.multiple = (Byte)(((pPut.Flags & 2) == 0) ? 0 : 1);
		if (enemy.info.slave == 0)
		{
			Int32 index = 0;
			Single num = pPut.Xpos;
			pBtl.evt.posBattle[0] = num;
			pBtl.original_pos[0] = num;
			pBtl.base_pos[0] = num;
			pBtl.pos[index] = num;
			Int32 index2 = 1;
			num = pPut.Ypos * -1;
			pBtl.evt.posBattle[1] = num;
			pBtl.original_pos[1] = num;
			pBtl.base_pos[1] = num;
			pBtl.pos[index2] = num;
			Int32 index3 = 2;
			num = pPut.Zpos;
			pBtl.evt.posBattle[2] = num;
			pBtl.original_pos[2] = num;
			pBtl.base_pos[2] = num;
			pBtl.pos[index3] = num;
			pBtl.evt.rotBattle = Quaternion.Euler(new Vector3(0f, (Single)(pPut.Rot & 4095), 180f));
			pBtl.rot = Quaternion.Euler(new Vector3(0f, (Single)(pPut.Rot + startTypeAngle & 4095), 180f));
			//pBtl.rot = (pBtl.evt.rotBattle = Quaternion.Euler(new Vector3(0f, pPut.Rot + 180 & 4095, 180f)));
		}
		else
		{
			pBtl.rot = (pBtl.evt.rotBattle = Quaternion.Euler(new Vector3(0f, 0f, 180f)));
		}
		pBtl.gameObject.transform.localPosition = pBtl.pos;
		pBtl.gameObject.transform.localRotation = pBtl.rot;
		pBtl.mot = enemy.et.mot;
	}

	public static void SetAttackData(ref AA_DATA pAttk, ref AA_DATA pEatk)
	{
		pEatk = pAttk;
	}

	public static void SetMonsterParameter(SB2_MON_PARM pParm, ref ENEMY_TYPE pType)
	{
		pType.radius = pParm.Radius;
		pType.category = pParm.Category;
		pType.level = pParm.Level;
		pType.blue_magic_no = pParm.Blue;
		pType.max.hp = pParm.MaxHP;
		pType.max.mp = pParm.MaxMP;
		pType.bonus.gil = pParm.WinGil;
		pType.bonus.exp = pParm.WinExp;
		pType.bonus.card = pParm.Card;
		pType.bonus.item = pParm.WinItems;
		for (Int16 num = 0; num < 6; num = (Int16)(num + 1))
		{
			pType.mot[num] = FF9BattleDB.Animation[pParm.Mot[num]];
		}
		for (Int16 num = 0; num < 3; num = (Int16)(num + 1))
		{
			pType.cam_bone[num] = pParm.Bone[num];
		}
		pType.die_snd_no = pParm.DieSfx;
		pType.p_atk_no = pParm.Konran;
		for (Int16 num = 0; num < 6; num = (Int16)(num + 1))
		{
			pType.icon_bone[num] = pParm.IconBone[num];
			pType.icon_y[num] = pParm.IconY[num];
			pType.icon_z[num] = pParm.IconZ[num];
		}
	}

	public static void InitPlayerData(FF9StateBattleSystem btlsys)
	{
		ObjList objList = new ObjList();
		if (!FF9StateSystem.Battle.isDebug)
		{
			objList = PersistenSingleton<EventEngine>.Instance.GetActiveObjList().next;
		}
		Int16 num2;
		Int16 num = num2 = 0;
		PLAYER p;
		while (num2 < 4)
		{
			p = FF9StateSystem.Common.FF9.party.member[num2];
			if (p != null)
			{
				BTL_DATA btl_DATA = btlsys.btl_data[num];

			    btl_DATA.dms_geo_id = GetModelID(characterModelIndex: p.info.serial_no);

				btl_init.OrganizePlayerData(p, btl_DATA, (UInt16)num2, (UInt16)num);
				btl_init.SetBattleModel(btl_DATA);
				if (Status.checkCurStat(btl_DATA, BattleStatus.Death))
				{
					GeoTexAnim.geoTexAnimStop(btl_DATA.texanimptr, 2);
					GeoTexAnim.geoTexAnimPlayOnce(btl_DATA.texanimptr, 0);
					if (btl_DATA.bi.player != 0)
					{
						GeoTexAnim.geoTexAnimStop(btl_DATA.tranceTexanimptr, 2);
						GeoTexAnim.geoTexAnimPlayOnce(btl_DATA.tranceTexanimptr, 0);
					}
				}
				else
				{
					GeoTexAnim.geoTexAnimPlay(btl_DATA.texanimptr, 2);
				}
				if (!FF9StateSystem.Battle.isDebug)
				{
					objList = objList.next;
				}
				num = (Int16)(num + 1);
				btl_sys.AddCharacter(btl_DATA);
			}
			num2 = (Int16)(num2 + 1);
		}
		while (num < 4)
		{
			btlsys.btl_data[num].btl_id = 0;
			num = (Int16)(num + 1);
		}
		btlsys.btl_load_status = (Byte)(btlsys.btl_load_status | 16);
		btl_init.SetupBattlePlayer();
		if (btlsys.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_BACK_ATTACK)
		{
			for (BTL_DATA btl_DATA = btlsys.btl_list.next; btl_DATA != null; btl_DATA = btl_DATA.next)
			{
				if (btl_DATA.bi.player != 0)
				{
					btl_DATA.cur.at = 0;
				}
			}
		}
		else if (btlsys.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_FIRST_ATTACK)
		{
			for (BTL_DATA btl_DATA = btlsys.btl_list.next; btl_DATA != null; btl_DATA = btl_DATA.next)
			{
				if (btl_DATA.bi.player == 0)
				{
					btl_DATA.cur.at = 0;
				}
			}
		}
	}

	public static void SetupBattlePlayer()
	{
		BTL_SCENE btl_scene = FF9StateSystem.Battle.FF9Battle.btl_scene;
		Int16 num2;
		Int16 num = num2 = 0;
		while (num2 < 4)
		{
			if (FF9StateSystem.Common.FF9.party.member[num2] != null)
			{
				num = (Int16)(num + 1);
			}
			num2 = (Int16)(num2 + 1);
		}
		Int16 num3 = 632;
		Int16 num4 = -1560;
		Int16 num5 = (Int16)((num - 1) * num3 / 2);
		Int16 num6 = (Int16)((btl_scene.Info.StartType != battle_start_type_tags.BTL_START_BACK_ATTACK) ? 180 : 0);
		num2 = 0;
		BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next;
		while (num2 < num)
		{
			if (next.bi.player == 0)
			{
				break;
			}
			next.bi.row = FF9StateSystem.Common.FF9.player[next.bi.slot_no].info.row;
			if (btl_scene.Info.StartType == battle_start_type_tags.BTL_START_BACK_ATTACK)
			{
				BTL_INFO bi = next.bi;
				bi.row = (Byte)(bi.row ^ 1);
			}
			BTL_DATA btl_DATA = next;
			Int32 index = 0;
			Single num7 = num5;
			next.evt.posBattle[0] = num7;
			next.base_pos[0] = num7;
			btl_DATA.pos[index] = num7;
			BTL_DATA btl_DATA2 = next;
			Int32 index2 = 1;
			num7 = (!btl_stat.CheckStatus(next, BattleStatus.Float)) ? 0 : -200;
			next.evt.posBattle[1] = num7;
			next.base_pos[1] = num7;
			btl_DATA2.pos[index2] = num7;
			BTL_DATA btl_DATA3 = next;
			Int32 index3 = 2;
			num7 = num4 + (Int16)((next.bi.row == 0) ? -400 : 0);
			next.evt.posBattle[2] = num7;
			next.base_pos[2] = num7;
			btl_DATA3.pos[index3] = num7;
			next.evt.rotBattle = Quaternion.Euler(new Vector3(0f, 180f, 180f));
			next.rot = Quaternion.Euler(new Vector3(0f, (Single)num6, 180f));
//			next.rot = (next.evt.rotBattle = Quaternion.Euler(new Vector3(0f, num6, 180f)));
			next.gameObject.transform.localPosition = next.pos;
			next.gameObject.transform.localRotation = next.rot;
			Int16 serial_no = FF9StateSystem.Common.FF9.player[next.bi.slot_no].info.serial_no;
			next.shadow_bone[0] = btl_init.ShadowDataPC[serial_no][0];
			next.shadow_bone[1] = btl_init.ShadowDataPC[serial_no][1];
			btl_util.SetShadow(next, btl_init.ShadowDataPC[serial_no][2], btl_init.ShadowDataPC[serial_no][3]);
			GameObject gameObject = FF9StateSystem.Battle.FF9Battle.map.shadowArray[next.bi.slot_no];
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.z = btl_init.ShadowDataPC[serial_no][4];
			gameObject.transform.localPosition = localPosition;
			num2 = (Int16)(num2 + 1);
			num5 = (Int16)(num5 - num3);
			next = next.next;
		}
	}

	public static void OrganizePlayerData(PLAYER p, BTL_DATA btl, UInt16 cnt, UInt16 btl_no)
	{
		btl.btl_id = (UInt16)(1 << btl_no);
		BONUS btl_bonus = battle.btl_bonus;
		btl_bonus.member_flag = (Byte)(btl_bonus.member_flag | (Byte)(1 << cnt));
		btl.bi.player = 1;
		btl.bi.slot_no = p.info.slot_no;
		btl.bi.target = 1;
		btl.bi.line_no = (Byte)cnt;
		btl.bi.slave = 0;
		if (battle.TRANCE_GAUGE_FLAG == 0 || (p.category & 16) != 0 || (btl.bi.slot_no == 2 && battle.GARNET_DEPRESS_FLAG != 0))
		{
			btl.bi.t_gauge = 0;
			btl.trance = 0;
		}
		else
		{
			btl.bi.t_gauge = 1;
			btl.trance = p.trance;
		}
		btl.tar_bone = 0;
		btl.sa = p.sa;
		btl.elem.dex = p.elem.dex;
		btl.elem.str = p.elem.str;
		btl.elem.mgc = p.elem.mgc;
		btl.elem.wpr = p.elem.wpr;
		btl.level = p.level;
		btl.max = p.max;
		btl_init.CopyPoints(btl.cur, p.cur);
		Byte serial_no = p.info.serial_no;
		FF9Char ff9Char = new FF9Char();
		ff9Char.btl = btl;
		ff9Char.evt = btl.evt;
		FF9StateSystem.Common.FF9.charArray.Add(p.info.slot_no, ff9Char);
		btl_init.InitBattleData(btl, ff9Char);
		btl.mesh_banish = UInt16.MaxValue;
		btl_stat.InitCountDownStatus(btl);
		btl.max.at = (Int16)((60 - btl.elem.dex) * 40 << 2);
		btl_para.InitATB(btl);
		if (FF9StateSystem.Battle.FF9Battle.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_BACK_ATTACK)
		{
			btl.cur.at = 0;
		}
		else if (FF9StateSystem.Battle.FF9Battle.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_FIRST_ATTACK)
		{
			btl.cur.at = (Int16)(btl.max.at - 1);
		}
		else
		{
			btl.cur.at = (Int16)(Comn.random16() % btl.max.at);
		}
		btl_mot.SetPlayerDefMotion(btl, p.info.serial_no, btl_no);
		BattlePlayerCharacter.InitAnimation(btl);
		btl_eqp.InitWeapon(p, btl);
		btl.defence.PhisicalDefence = p.defence.PhisicalDefence;
		btl.defence.PhisicalEvade = p.defence.PhisicalEvade;
		btl.defence.MagicalDefence = p.defence.MagicalDefence;
		btl.defence.MagicalEvade = p.defence.MagicalEvade;
		btl_eqp.InitEquipPrivilegeAttrib(p, btl);
		btl_util.GeoSetColor2Source(btl.weapon_geo, 0, 0, 0);
		if (btl.cur.hp * 6 < btl.max.hp)
		{
			btl.stat.cur |= BattleStatus.LowHP;
		}

        btl_stat.AlterStatuses(btl, (BattleStatus)p.status & (BattleStatus.Venom | BattleStatus.Virus | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Zombie | BattleStatus.EasyKill | BattleStatus.Death | BattleStatus.LowHP | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Stop | BattleStatus.AutoLife | BattleStatus.Trance | BattleStatus.Defend | BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Regen | BattleStatus.Haste | BattleStatus.Slow | BattleStatus.Float | BattleStatus.Shell | BattleStatus.Protect | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Vanish | BattleStatus.Doom | BattleStatus.Mini | BattleStatus.Reflect | BattleStatus.Jump | BattleStatus.GradualPetrify));
		if ((p.status & 1) != 0)
		{
			btl_stat.AlterStatus(btl, BattleStatus.Petrify);
		}
		btl_abil.CheckStatusAbility(new BattleUnit(btl));
		BattleStatus permanent_stat = btl.stat.permanent;
		BattleStatus current_stat = btl.stat.cur;
		btl.stat.permanent = 0;
		btl.stat.cur = 0;
		btl_stat.MakeStatusesPermanent(btl, permanent_stat);
		btl_stat.AlterStatuses(btl, current_stat);
		btl.base_pos = btl.evt.posBattle;
		Int16 geoID = btl.dms_geo_id;
		btl.height = 0;
		btl.radius = 0;

        FF9BattleDBHeightAndRadius.TryFindHeightAndRadius(geoID, ref btl.height, ref btl.radius);

        if (btl.cur.hp == 0 && btl_stat.AlterStatus(btl, BattleStatus.Death) == 2u)
		{
			btl.die_seq = 5;
			btl_mot.DecidePlayerDieSequence(btl);
			return;
		}
		btl.bi.def_idle = Convert.ToByte(btl_stat.CheckStatus(btl, BattleStatus.IdleDying));
		btl_mot.setMotion(btl, btl.bi.def_idle);
		btl.evt.animFrame = 0;
	}

	public static void OrganizeEnemyData(FF9StateBattleSystem btlsys)
	{
		for (Int32 i = 0; i < BTL_SCENE.GetMonCount(); i++)
		{
			ENEMY_TYPE et = btlsys.enemy[i].et;
			BTL_DATA btl_DATA = btlsys.btl_data[4 + i];
			btl_DATA.level = et.level;
			btl_DATA.max.at = (Int16)((60 - btl_DATA.elem.dex) * 40 << 2);
			btl_para.InitATB(btl_DATA);
			btl_DATA.cur.at = (Int16)(Comn.random16() % btl_DATA.max.at);
			btl_DATA.weapon = null;
			btl_stat.InitCountDownStatus(btl_DATA);
			btl_mot.HideMesh(btl_DATA, btl_DATA.mesh_current, false);
			if (btl_DATA.bi.slave != 0)
			{
				btl_DATA.cur.at = 0;
				btl_DATA.cur.at_coef = 0;
				btl_DATA.gameObject.transform.localRotation = btl_DATA.rot;
				btl_DATA.gameObject.transform.localPosition = btl_DATA.evt.posBattle;
				btl_DATA.currentAnimationName = btl_DATA.mot[btl_DATA.bi.def_idle];
				btl_mot.setMotion(btl_DATA, btl_DATA.currentAnimationName);
				btl_mot.setSlavePos(btl_DATA, ref btl_DATA.base_pos);
				UnityEngine.Object.Destroy(btl_DATA.gameObject);
				UnityEngine.Object.Destroy(btl_DATA.getShadow());
				btl_DATA.gameObject = btl_util.GetMasterEnemyBtlPtr().Data.gameObject;
			}
			else
			{
				btl_DATA.base_pos[0] = btl_DATA.evt.posBattle[0];
				btl_DATA.base_pos[1] = btl_DATA.evt.posBattle[1];
				btl_DATA.base_pos[2] = btl_DATA.evt.posBattle[2];
				btl_DATA.currentAnimationName = btl_DATA.mot[btl_DATA.bi.def_idle];
				btl_DATA.evt.animFrame = (Byte)(Comn.random8() % GeoAnim.geoAnimGetNumFrames(btl_DATA));
			}
			BattleStatus permanent_stat = btl_DATA.stat.permanent;
			BattleStatus current_stat = btl_DATA.stat.cur;
			btl_DATA.stat.permanent = 0;
			btl_DATA.stat.cur = 0;
			btl_stat.MakeStatusesPermanent(btl_DATA, permanent_stat);
			btl_stat.AlterStatuses(btl_DATA, current_stat);
		}
	}

	public static void CopyPoints(POINTS d, POINTS s)
	{
		d.hp = s.hp;
		d.mp = s.mp;
		d.at = s.at;
		d.capa = s.capa;
	}

	public static void IncrementDefAttr(DEF_ATTR d, DEF_ATTR s)
	{
		d.invalid = (Byte)(d.invalid | s.invalid);
		d.absorb = (Byte)(d.absorb | s.absorb);
		d.half = (Byte)(d.half | s.half);
		d.weak = (Byte)(d.weak | s.weak);
	}

	public static void InitBattleData(BTL_DATA btl, FF9Char ff9char)
	{
		BTL_INFO bi = btl.bi;
		btl_cmd.InitCommand(btl);
		btl_stat.InitStatus(btl);
		bi.dmg_mot_f = 0;
		bi.cmd_idle = 0;
		bi.death_f = 0;
		bi.stop_anim = 0;
		btl.SetDisappear(0);
		bi.shadow = 1;
		bi.cover = 0;
		bi.dodge = 0;
		bi.die_snd_f = 0;
		bi.select = 0;
		btl.escape_key = 0;
		btl.sel_menu = 0;
		btl.fig_info = 0;
		btl.fig = 0;
		btl.m_fig = 0;
		btl.fig_stat_info = 0;
		btl.fig_regene_hp = 0;
		btl.fig_poison_hp = 0;
		btl.fig_poison_mp = 0;
		btl.die_seq = 0;
		ff9char.btl = btl;
		btl.evt = ff9char.evt;
		GeoTexAnim.geoTexAnimPlay(btl.texanimptr, 2);
		btl_util.GeoSetColor2Source(btl.gameObject, 0, 0, 0);
		btl.mesh_current = 0;
		// Out of reach: enemies inside battles flagged with "NoNeighboring" are all set to out of reach but they can also be placed individually using SV_FunctionEnemy[110] in battle scripts or setting the "OutOfReach" flag in the battle's ".memnfo" file
		// False by default for player character, initialized in "btl_init.SetMonsterData" for enemies
		btl.out_of_reach = false;
		for (int i = 0; i < btl.stat_modifier.Length; i++)
			btl.stat_modifier[i] = false;
		btl.summon_count = 0;
		btl.critical_rate_deal_bonus = 0;
		btl.critical_rate_receive_bonus = 0;
	}

    public static void SetBattleModel(BTL_DATA btl)
    {
        // Set normal model
        String modelName = (btl.dms_geo_id == -1) ? String.Empty : FF9BattleDB.GEO.GetValue(btl.dms_geo_id);
        Int32 scale = 1;
        if (ModelFactory.HaveUpScaleModel(modelName))
        {
            scale = 4;
        }

        GEOTEXHEADER geotexheader = new GEOTEXHEADER();
        geotexheader.ReadPlayerTextureAnim(btl, "Models/GeoTexAnim/" + modelName + ".tab", scale);
        btl.texanimptr = geotexheader;

        // Set trance model
        Byte serialNumber = btl_util.getSerialNumber(btl);
        if (serialNumber + 19 >= btl_init.model_id.Length)
            return;

        Int32 num = serialNumber + 19;
        String geoName = btl_init.model_id[num];
        
        GEOTEXHEADER geotexheader2 = new GEOTEXHEADER();
        geotexheader2.ReadTrancePlayerTextureAnim(btl, geoName, scale);
        btl.tranceTexanimptr = geotexheader2;
    }

    public const Byte BTL_LOAD_BG_DONE = 1;

	public const Byte BTL_LOAD_ENEMY_DONE = 2;

	public const Byte BTL_LOAD_PLAYER_DONE = 4;

	public const Byte BTL_WAIT_ENEMY_STONE_DONE = 8;

	public const Byte BTL_WAIT_WEAPON_STONE_DONE = 16;

	public const Byte BTL_WAIT_ENEMY_APPEAR_DONE = 32;

	public const Byte BTL_WAIT_PLAYER_APPEAR_DONE = 64;

	public static String[] model_id = new String[] // Follow the list of "BattlePlayerCharacter.PlayerSerialNumber"
	{
		// Normal models:
		"GEO_MAIN_B0_000", // ZIDANE_DAGGER
		"GEO_MAIN_B0_001", // ZIDANE_SWORD,
		"GEO_MAIN_B0_006", // VIVI,
		"GEO_MAIN_B0_002", // GARNET_LH_ROD,
		"GEO_MAIN_B0_003", // GARNET_LH_KNIFE,
		"GEO_MAIN_B0_004", // GARNET_SH_ROD,
		"GEO_MAIN_B0_005", // GARNET_SH_KNIFE,
		"GEO_MAIN_B0_018", // STEINER_OUTDOOR,
		"GEO_MAIN_B0_007", // STEINER_INDOOR,
		"GEO_MAIN_B0_008", // KUINA,
		"GEO_MAIN_B0_009", // EIKO_FLUTE,
		"GEO_MAIN_B0_010", // EIKO_KNIFE,
		"GEO_MAIN_B0_011", // FREIJA,
		"GEO_MAIN_B0_012", // SALAMANDER,
		"GEO_MAIN_B0_013", // CINNA,
		"GEO_MAIN_B0_014", // MARCUS,
		"GEO_MAIN_B0_015", // BLANK,
		"GEO_MAIN_B0_016", // BLANK_ARMOR,
		"GEO_MAIN_B0_017", // BEATRIX,
		// Trance models:
		"GEO_MAIN_B0_022", // ZIDANE_DAGGER
		"GEO_MAIN_B0_023", // ZIDANE_SWORD,
		"GEO_MAIN_B0_028", // VIVI,
		"GEO_MAIN_B0_024", // GARNET_LH_ROD,
		"GEO_MAIN_B0_025", // GARNET_LH_KNIFE,
		"GEO_MAIN_B0_026", // GARNET_SH_ROD,
		"GEO_MAIN_B0_027", // GARNET_SH_KNIFE,
		"GEO_MAIN_B0_029", // STEINER_OUTDOOR,
		"GEO_MAIN_B0_029", // STEINER_INDOOR,
		"GEO_MAIN_B0_030", // KUINA,
		"GEO_MAIN_B0_031", // EIKO_FLUTE,
		"GEO_MAIN_B0_032", // EIKO_KNIFE,
		"GEO_MAIN_B0_033", // FREIJA,
		"GEO_MAIN_B0_034", // SALAMANDER,
		// Trance models of temporary characters: use the same as normal models
		"GEO_MAIN_B0_013", // CINNA,
		"GEO_MAIN_B0_014", // MARCUS,
		"GEO_MAIN_B0_015", // BLANK,
		"GEO_MAIN_B0_016", // BLANK_ARMOR,
		"GEO_MAIN_B0_017"  // BEATRIX,
	};

	private static readonly UInt32[] enemy_dummy_sa = new UInt32[2];

	private static Byte[][] ShadowDataPC = new Byte[][]
	{
		new Byte[]
		{
			1,
			14,
			196,
			204,
			0
		},
		new Byte[]
		{
			1,
			14,
			196,
			204,
			0
		},
		new Byte[]
		{
			1,
			1,
			154,
			168,
			0
		},
		new Byte[]
		{
			1,
			10,
			140,
			144,
			36
		},
		new Byte[]
		{
			1,
			10,
			140,
			144,
			36
		},
		new Byte[]
		{
			1,
			10,
			140,
			144,
			36
		},
		new Byte[]
		{
			1,
			10,
			140,
			144,
			36
		},
		new Byte[]
		{
			1,
			10,
			210,
			204,
			0
		},
		new Byte[]
		{
			1,
			10,
			210,
			204,
			0
		},
		new Byte[]
		{
			1,
			1,
			224,
			228,
			0
		},
		new Byte[]
		{
			1,
			1,
			126,
			132,
			0
		},
		new Byte[]
		{
			1,
			1,
			126,
			132,
			0
		},
		new Byte[]
		{
			1,
			13,
			182,
			192,
			0
		},
		new Byte[]
		{
			1,
			10,
			238,
			240,
			0
		},
		new Byte[]
		{
			1,
			10,
			168,
			168,
			0
		},
		new Byte[]
		{
			1,
			1,
			182,
			204,
			0
		},
		new Byte[]
		{
			16,
			20,
			140,
			144,
			0
		},
		new Byte[]
		{
			16,
			20,
			140,
			144,
			0
		},
		new Byte[]
		{
			1,
			11,
			168,
			180,
			0
		}
	};
}
