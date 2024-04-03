using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using UnityEngine;

public static class btl_init
{
	public static Int16 GetModelID(CharacterSerialNumber characterModelIndex, Boolean isTrance = false)
	{
		if (characterModelIndex == CharacterSerialNumber.NONE)
		{
			Log.Warning("[btl_init] Invalid serial number: {0}", characterModelIndex);
			return 0;
		}

		CharacterBattleParameter param = btl_mot.BattleParameterList[characterModelIndex];
		String modelId = isTrance ? param.TranceModelId : param.ModelId;

		Int32 geoId;
		if (FF9BattleDB.GEO.TryGetKey(modelId, out geoId))
			return (Int16)geoId;

		Log.Warning("[btl_init] Unknown model id: {0}", modelId);
		return 0;
	}

	public static void InitEnemyData(FF9StateBattleSystem btlsys)
	{
		BTL_DATA monLastBtl = null;
		ObjList objList = new ObjList();
		if (!FF9StateSystem.Battle.isDebug)
			objList = PersistenSingleton<EventEngine>.Instance.GetActiveObjList().next;
		Int32 monCount = FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].MonsterCount;
		Int32 enemyIndex = 0;
		Int32 btlIndex = 4;
		while (enemyIndex < monCount)
		{
			ENEMY enemy = btlsys.enemy[enemyIndex];
			BTL_DATA monBtl = btlsys.btl_data[btlIndex];
			BTL_SCENE btl_scene = FF9StateSystem.Battle.FF9Battle.btl_scene;
			SB2_PATTERN battlePattern = btl_scene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum];
			SB2_MON_PARM monParam = btl_scene.MonAddr[battlePattern.Monster[enemyIndex].TypeNo];
			btlshadow.ff9battleShadowInit(monBtl);
			enemy.info.die_fade_rate = 32;
			if ((monBtl.dms_geo_id = BTL_SCENE.GetMonGeoID(enemyIndex)) < 0)
			{
				enemy.info.slave = 1;
			}
			else
			{
				btl_init.SetBattleModel(monBtl);
				enemy.info.slave = 0;
				if (!FF9StateSystem.Battle.isDebug)
					objList = objList.next;
			}
			monBtl.btl_id = (UInt16)(16 << enemyIndex);
			monBtl.bi.player = 0;
			monBtl.bi.slot_no = (Byte)enemyIndex;
			monBtl.bi.line_no = (Byte)(4 + enemyIndex);
			monBtl.bi.t_gauge = (Byte)((monParam.ResistStatus & BattleStatus.Trance) == 0 ? 1 : 0); // Enemies can have trance when they are not immune to it
			monBtl.bi.slave = enemy.info.slave;
			monBtl.height = 0;
			monBtl.radius_effect = 0;
			monBtl.radius_collision = monParam.Radius;
			FF9Char ff9char = new FF9Char();
			btl_init.InitBattleData(monBtl, ff9char);
			monBtl.bi.def_idle = 0;
			monBtl.base_pos = enemy.base_pos;
			String path = (monBtl.dms_geo_id == -1) ? String.Empty : FF9BattleDB.GEO.GetValue(monBtl.dms_geo_id);
			if (!ModelFactory.IsUseAsEnemyCharacter(path))
				monBtl.weapon_geo = null;
			monBtl.sa = btl_init.enemy_dummy_sa;
			monBtl.saExtended = new HashSet<SupportAbility>();
			monBtl.saMonster = new List<SupportingAbilityFeature>();
			foreach (SupportingAbilityFeature feature in monParam.SupportingAbilityFeatures)
				if (feature.EnableAsEnemy)
					monBtl.saMonster.Add(feature);

			FF9BattleDBHeightAndRadius.TryFindHeightAndRadius(monParam.Geo, ref monBtl.height, ref monBtl.radius_effect);

			if (monLastBtl != null)
				monLastBtl.next = monBtl;
			monLastBtl = monBtl;
			enemyIndex++;
			btlIndex++;
		}
		while (btlIndex < 8)
		{
			btlsys.btl_data[btlIndex].btl_id = 0;
			btlIndex++;
		}
		monLastBtl.next = null;
		btlsys.btl_list.next = btlsys.btl_data[4];
		btlsys.btl_load_status |= ff9btl.LOAD_INITNPC;
		btl_init.SetupBattleEnemy();
		btlseq.InitSequencer();
	}

	public static void SetupBattleEnemy()
	{
		BTL_SCENE scene = FF9StateSystem.Battle.FF9Battle.btl_scene;
		SB2_HEAD header = scene.header;
		if (FF9StateSystem.Battle.isDebug)
			scene.Info.StartType = FF9StateSystem.Battle.debugStartType;
		else
			scene.Info.StartType = btl_sys.StartType(scene.Info);
		SB2_PATTERN[] scenePatternList = FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr;
		SB2_MON_PARM[] sceneMonsterList = FF9StateSystem.Battle.FF9Battle.btl_scene.MonAddr;
		AA_DATA[] sceneAttackList = FF9StateSystem.Battle.FF9Battle.btl_scene.atk;
		List<ENEMY_TYPE> monsterList = FF9StateSystem.Battle.FF9Battle.enemy_type;
		Int32 messageIndex = header.TypCount + header.AtkCount;
		Int32 nameIndex = 0;
		for (Int32 i = 0; i < header.TypCount; i++)
		{
			if (i >= monsterList.Count)
				monsterList.Add(new ENEMY_TYPE());
			btl_init.SetMonsterParameter(sceneMonsterList[i], monsterList[i]);
			monsterList[i].name = FF9TextTool.BattleText(nameIndex++);
			monsterList[i].mes = messageIndex;
			messageIndex += sceneMonsterList[i].MesCnt;
		}
		List<AA_DATA> attackList = FF9StateSystem.Battle.FF9Battle.enemy_attack;
		attackList.Clear();
		for (Int32 i = 0; i < header.AtkCount; i++)
		{
			attackList.Add(sceneAttackList[i]);
			attackList[i].Name = FF9TextTool.BattleText(nameIndex++);
		}
		BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next;
		SB2_PATTERN patternPicked = scenePatternList[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum];
		for (Int32 i = 0; i < patternPicked.MonsterCount && next != null; i++)
		{
			btl_init.PutMonster(patternPicked.Monster[i], next, scene, i);
			btl_init.SetMonsterData(sceneMonsterList[patternPicked.Monster[i].TypeNo], next, i);
			next = next.next;
		}
	}

	public static void SetMonsterData(SB2_MON_PARM pParm, BTL_DATA pBtl, Int32 pNo)
	{
		pBtl.stat.invalid = pParm.ResistStatus;
		pBtl.stat.permanent = pParm.AutoStatus;
		pBtl.stat.cur = pParm.InitialStatus;
		pBtl.cur.hp = pParm.MaxHP;
		pBtl.cur.mp = pParm.MaxMP;
		pBtl.defence.PhysicalDefence = pParm.PhysicalDefence;
		pBtl.defence.PhysicalEvade = pParm.PhysicalEvade;
		pBtl.defence.MagicalDefence = pParm.MagicalDefence;
		pBtl.defence.MagicalEvade = pParm.MagicalEvade;
		pBtl.elem.dex = pParm.Element.Speed;
		pBtl.elem.str = pParm.Element.Strength;
		pBtl.elem.mgc = pParm.Element.Magic;
		pBtl.elem.wpr = pParm.Element.Spirit;
		pBtl.maxDamageLimit = pParm.MaxDamageLimit;
		pBtl.maxMpDamageLimit = pParm.MaxMpDamageLimit;
		pBtl.def_attr.invalid = pParm.GuardElement;
		pBtl.def_attr.absorb = pParm.AbsorbElement;
		pBtl.def_attr.half = pParm.HalfElement;
		pBtl.def_attr.weak = pParm.WeakElement;
		pBtl.p_up_attr = pParm.BonusElement;
		pBtl.mesh_current = pParm.Mesh[0];
		pBtl.mesh_banish = pParm.Mesh[1];
		pBtl.tar_bone = pParm.Bone[3];
		// New field "out_of_reach"
		pBtl.out_of_reach = pParm.OutOfReach || FF9StateSystem.Battle.FF9Battle.btl_scene.Info.NoNeighboring;
		ENEMY enemy = FF9StateSystem.Battle.FF9Battle.enemy[pBtl.bi.slot_no];
		enemy.bonus_gil = pParm.WinGil;
		enemy.bonus_exp = pParm.WinExp;
		for (Int32 i = 0; i < pParm.WinItems.Length; i++)
		{
			enemy.bonus_item[i] = pParm.WinItems[i];
			enemy.bonus_item_rate[i] = pParm.WinItemRates[i];
		}
		enemy.bonus_card = pParm.WinCard;
		enemy.bonus_card_rate = pParm.WinCardRate;
		for (Int32 i = 0; i < pParm.StealItems.Length; i++)
		{
			enemy.steal_item[i] = pParm.StealItems[i];
			enemy.steal_item_rate[i] = pParm.StealItemRates[i];
		}
		enemy.trance_glowing_color[0] = pParm.TranceGlowingColor != null && pParm.TranceGlowingColor.Length > 0 ? pParm.TranceGlowingColor[0] : (Byte)0xFF;
		enemy.trance_glowing_color[1] = pParm.TranceGlowingColor != null && pParm.TranceGlowingColor.Length > 1 ? pParm.TranceGlowingColor[1] : (Byte)0x60;
		enemy.trance_glowing_color[2] = pParm.TranceGlowingColor != null && pParm.TranceGlowingColor.Length > 2 ? pParm.TranceGlowingColor[2] : (Byte)0x60;
		enemy.steal_unsuccessful_counter = 0; // New field used for counting unsuccessful steals and force a successful steal when it becomes high enough
		enemy.info.die_atk = (Byte)(((pParm.Flags & 1) == 0) ? 0 : 1);
		enemy.info.die_dmg = (Byte)(((pParm.Flags & 2) == 0) ? 0 : 1);
        enemy.info.die_vulnerable = (Byte)(((pParm.Flags & 4) == 0) ? 0 : 1);
        enemy.info.die_unused4 = (Byte)(((pParm.Flags & 8) == 0) ? 0 : 1);
        enemy.info.die_unused5 = (Byte)(((pParm.Flags & 16) == 0) ? 0 : 1);
        enemy.info.die_unused6 = (Byte)(((pParm.Flags & 32) == 0) ? 0 : 1);
        enemy.info.die_unused7 = (Byte)(((pParm.Flags & 64) == 0) ? 0 : 1);
        enemy.info.die_unused8 = (Byte)(((pParm.Flags & 128) == 0) ? 0 : 1);
        enemy.info.flags = pParm.Flags;
        btl_util.SetShadow(pBtl, pParm.ShadowX, pParm.ShadowZ);
		pBtl.shadow_bone[0] = pParm.ShadowBone;
		pBtl.shadow_bone[1] = pParm.ShadowBone2;
		pBtl.geo_scale_x = pBtl.geo_scale_y = pBtl.geo_scale_z = pBtl.geo_scale_default = 4096;
        pBtl.special_status_old = false; // TRANCE SEEK - Old Status
        btl_abil.CheckStatusAbility(new BattleUnit(pBtl));
	}

	public static void PutMonster(SB2_PUT pPut, BTL_DATA pBtl, BTL_SCENE pScene, Int32 pNo)
	{
		Int16 startTypeAngle = (Int16)(pScene.Info.StartType == battle_start_type_tags.BTL_START_FIRST_ATTACK ? 180 : 0);
		ENEMY enemy = FF9StateSystem.Battle.FF9Battle.enemy[pBtl.bi.slot_no];
		enemy.et = FF9StateSystem.Battle.FF9Battle.enemy_type[pPut.TypeNo];
		pBtl.bi.target = (Byte)(((pPut.Flags & 1) == 0) ? 0 : 1);
		pBtl.bi.row = 2;
		CopyPoints(pBtl.max, FF9StateSystem.Battle.FF9Battle.enemy_type[pPut.TypeNo].max);
		pBtl.cur.hp = pBtl.max.hp;
		pBtl.cur.mp = pBtl.max.mp;
		enemy.info.multiple = (Byte)(((pPut.Flags & 2) == 0) ? 0 : 1);
		if (enemy.info.slave == 0)
		{
			pBtl.evt.posBattle = pBtl.original_pos = pBtl.base_pos = pBtl.pos = new Vector3(pPut.Xpos, pPut.Ypos * -1, pPut.Zpos);
			pBtl.evt.rotBattle = Quaternion.Euler(new Vector3(0f, pPut.Rot & 4095, 180f));
			pBtl.rot = Quaternion.Euler(new Vector3(0f, pPut.Rot + startTypeAngle & 4095, 180f));
			//pBtl.rot = pBtl.evt.rotBattle = Quaternion.Euler(new Vector3(0f, pPut.Rot + 180 & 4095, 180f));
		}
		else
		{
			pBtl.rot = pBtl.evt.rotBattle = Quaternion.Euler(new Vector3(0f, 0f, 180f));
		}
		pBtl.gameObject.transform.localPosition = pBtl.pos;
		pBtl.gameObject.transform.localRotation = pBtl.rot;
		pBtl.mot = enemy.et.mot;
		pBtl.animFlag = 0;
		pBtl.animSpeed = 1f;
		pBtl.animFrameFrac = 0f;
		pBtl.animEndFrame = enemy.info.slave == 0;
	}

	public static void SetMonsterParameter(SB2_MON_PARM pParm, ENEMY_TYPE pType)
	{
		pType.radius = pParm.Radius;
		pType.category = pParm.Category;
		pType.level = pParm.Level;
		pType.blue_magic_no = pParm.BlueMagic;
		pType.max.hp = pParm.MaxHP;
		pType.max.mp = pParm.MaxMP;
		for (Int16 i = 0; i < 6; i++)
			pType.mot[i] = FF9BattleDB.Animation[pParm.Mot[i]];
		for (Int16 i = 0; i < 3; i++)
			pType.cam_bone[i] = pParm.Bone[i];
		pType.die_snd_no = pParm.DieSfx;
		pType.p_atk_no = pParm.Konran;
		for (Int16 i = 0; i < 6; i++)
		{
			pType.icon_bone[i] = pParm.IconBone[i];
			pType.icon_y[i] = pParm.IconY[i];
			pType.icon_z[i] = pParm.IconZ[i];
		}
	}

	public static void InitPlayerData(FF9StateBattleSystem btlsys)
	{
		ObjList objList = new ObjList();
		if (!FF9StateSystem.Battle.isDebug)
			objList = PersistenSingleton<EventEngine>.Instance.GetActiveObjList().next;
		Int16 btlIndex = 0;
		PLAYER player;
		for (Int32 memberIndex = 0; memberIndex < 4; memberIndex++)
		{
			player = FF9StateSystem.Common.FF9.party.member[memberIndex];
			if (player != null)
			{
				BTL_DATA btl = btlsys.btl_data[btlIndex];

				btl.dms_geo_id = GetModelID(player.info.serial_no);

				btl_init.OrganizePlayerData(player, btl, (UInt16)memberIndex, (UInt16)btlIndex);
				btl_init.SetBattleModel(btl);
				if (Status.checkCurStat(btl, BattleStatus.Death))
				{
					GeoTexAnim.geoTexAnimStop(btl.texanimptr, 2);
					GeoTexAnim.geoTexAnimPlayOnce(btl.texanimptr, 0);
					if (btl.bi.player != 0)
					{
						GeoTexAnim.geoTexAnimStop(btl.tranceTexanimptr, 2);
						GeoTexAnim.geoTexAnimPlayOnce(btl.tranceTexanimptr, 0);
					}
				}
				else
				{
					GeoTexAnim.geoTexAnimPlay(btl.texanimptr, 2);
				}
				if (!FF9StateSystem.Battle.isDebug)
					objList = objList.next;
				btlIndex++;
				btl_sys.AddCharacter(btl);
				if (btlsys.cmd_escape.regist == null)
					btlsys.cmd_escape.regist = btl;
			}
		}
		while (btlIndex < 4)
		{
			btlsys.btl_data[btlIndex].btl_id = 0;
			btlIndex++;
		}
		btlsys.btl_load_status |= ff9btl.LOAD_INITCHR;
		btl_init.SetupBattlePlayer();
		if (btlsys.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_BACK_ATTACK)
		{
			for (BTL_DATA btl = btlsys.btl_list.next; btl != null; btl = btl.next)
				if (btl.bi.player != 0)
					btl.cur.at = 0;
		}
		else if (btlsys.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_FIRST_ATTACK)
		{
			for (BTL_DATA btl = btlsys.btl_list.next; btl != null; btl = btl.next)
				if (btl.bi.player == 0)
					btl.cur.at = 0;
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
				num++;
			num2++;
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
				break;
			CharacterId charId = (CharacterId)next.bi.slot_no;
			next.bi.row = FF9StateSystem.Common.FF9.player[charId].info.row;
			if (btl_scene.Info.StartType == battle_start_type_tags.BTL_START_BACK_ATTACK)
			{
				BTL_INFO bi = next.bi;
				bi.row = (Byte)(bi.row ^ 1);
			}
			BTL_DATA btl_DATA = next;
			Single num7 = num5;
			next.original_pos[0] = num7;
			next.evt.posBattle[0] = num7;
			next.base_pos[0] = num7;
			btl_DATA.pos[0] = num7;
			BTL_DATA btl_DATA2 = next;
			num7 = (!btl_stat.CheckStatus(next, BattleStatus.Float)) ? 0 : -200;
			next.original_pos[1] = 0;
			next.evt.posBattle[1] = num7;
			next.base_pos[1] = num7;
			btl_DATA2.pos[1] = num7;
			BTL_DATA btl_DATA3 = next;
			num7 = num4 + (Int16)((next.bi.row == 0) ? -400 : 0);
			next.original_pos[2] = num7;
			next.evt.posBattle[2] = num7;
			next.base_pos[2] = num7;
			btl_DATA3.pos[2] = num7;
			next.evt.rotBattle = Quaternion.Euler(new Vector3(0f, 180f, 180f));
			next.rot = Quaternion.Euler(new Vector3(0f, (Single)num6, 180f));
//			next.rot = (next.evt.rotBattle = Quaternion.Euler(new Vector3(0f, num6, 180f)));
			next.gameObject.transform.localPosition = next.pos;
			next.gameObject.transform.localRotation = next.rot;
			CharacterBattleParameter btlParam = btl_mot.BattleParameterList[FF9StateSystem.Common.FF9.player[charId].info.serial_no];
			next.shadow_bone[0] = btlParam.ShadowData[0];
			next.shadow_bone[1] = btlParam.ShadowData[1];
			btl_util.SetShadow(next, btlParam.ShadowData[2], btlParam.ShadowData[3]);
			next.geo_scale_x = next.geo_scale_y = next.geo_scale_z = next.geo_scale_default = 4096;
			GameObject shadowObj = FF9StateSystem.Battle.FF9Battle.map.shadowArray[next];
			Vector3 shadowPos = shadowObj.transform.localPosition;
			shadowPos.z = btlParam.ShadowData[4];
			shadowObj.transform.localPosition = shadowPos;
			num2++;
			num5 -= num3;
			next = next.next;
		}
	}

	public static void OrganizePlayerData(PLAYER p, BTL_DATA btl, UInt16 cnt, UInt16 btl_no)
	{
		btlshadow.ff9battleShadowInit(btl);
		btl.btl_id = (UInt16)(1 << btl_no);
		BONUS btl_bonus = battle.btl_bonus;
		btl_bonus.member_flag |= (Byte)(1 << cnt);
		btl.bi.player = 1;
		btl.bi.slot_no = (Byte)p.info.slot_no;
		btl.bi.target = 1;
		btl.bi.line_no = (Byte)cnt;
		btl.bi.slave = 0;
		if (battle.TRANCE_GAUGE_FLAG == 0 || (p.category & 16) != 0 || (btl.bi.slot_no == (Byte)CharacterId.Garnet && battle.GARNET_DEPRESS_FLAG != 0))
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
		btl.saExtended = p.saExtended;
		btl.saMonster = new List<SupportingAbilityFeature>();
		btl.elem.dex = p.elem.dex;
		btl.elem.str = p.elem.str;
		btl.elem.mgc = p.elem.mgc;
		btl.elem.wpr = p.elem.wpr;
		btl.level = p.level;
		btl_init.CopyPoints(btl.max, p.max);
		btl_init.CopyPoints(btl.cur, p.cur);
		btl.maxDamageLimit = p.maxDamageLimit;
		btl.maxMpDamageLimit = p.maxMpDamageLimit;
		FF9Char ff9Char = new FF9Char();
		ff9Char.btl = btl;
		ff9Char.evt = btl.evt;
		if (ff9play.CharacterIDToEventId(p.Index) >= 0)
			FF9StateSystem.Common.FF9.charArray.Add(ff9play.CharacterIDToEventId(p.Index), ff9Char);
		else
			FF9StateSystem.Common.FF9.charArray.Add(9 + (Int32)p.Index, ff9Char);
		btl_init.InitBattleData(btl, ff9Char);
		btl.mesh_banish = UInt16.MaxValue;
		btl_stat.InitCountDownStatus(btl);
		btl.max.at = (Int16)((60 - btl.elem.dex) * 40 << 2);
		btl_para.InitATB(btl);
		if (FF9StateSystem.Battle.FF9Battle.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_BACK_ATTACK)
			btl.cur.at = 0;
		else if (FF9StateSystem.Battle.FF9Battle.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_FIRST_ATTACK)
			btl.cur.at = (Int16)(btl.max.at - 1);
		else
			btl.cur.at = (Int16)(Comn.random16() % btl.max.at);
		btl_mot.SetPlayerDefMotion(btl, p.info.serial_no, btl_no);
		BattlePlayerCharacter.InitAnimation(btl);
		btl_eqp.InitWeapon(p, btl);
		btl.defence.PhysicalDefence = p.defence.PhysicalDefence;
		btl.defence.PhysicalEvade = p.defence.PhysicalEvade;
		btl.defence.MagicalDefence = p.defence.MagicalDefence;
		btl.defence.MagicalEvade = p.defence.MagicalEvade;
		btl_eqp.InitEquipPrivilegeAttrib(p, btl);
		btl_util.GeoSetColor2Source(btl.weapon_geo, 0, 0, 0);
		if (btl.cur.hp * 6 < btl.max.hp)
			btl.stat.cur |= BattleStatus.LowHP;

		btl_stat.AlterStatuses(btl, p.status & ~BattleStatus.Petrify);
		if ((p.status & BattleStatus.Petrify) != 0)
			btl_stat.AlterStatus(btl, BattleStatus.Petrify);
		btl_abil.CheckStatusAbility(new BattleUnit(btl));
		BattleStatus resist_stat = btl.stat.invalid;
		BattleStatus permanent_stat = btl.stat.permanent;
		BattleStatus current_stat = btl.stat.cur;
		btl.stat.invalid = 0;
		btl.stat.permanent = 0;
		btl.stat.cur = 0;
		btl_stat.MakeStatusesPermanent(btl, permanent_stat);
		btl_stat.AlterStatuses(btl, current_stat);
		btl.stat.invalid = resist_stat;
		btl.base_pos = btl.evt.posBattle;
		Int16 geoID = btl.dms_geo_id;
		btl.height = 0;
		btl.radius_effect = 0;
		btl.radius_collision = 256;

		FF9BattleDBHeightAndRadius.TryFindHeightAndRadius(geoID, ref btl.height, ref btl.radius_effect);

		if (btl.cur.hp == 0 && btl_stat.AlterStatus(btl, BattleStatus.Death) == 2u)
		{
			btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE);
			btl.evt.animFrame = 0;
			btl.die_seq = 5;
			//btl_mot.DecidePlayerDieSequence(btl);
			return;
		}
		btl.bi.def_idle = (Byte)(btl_stat.CheckStatus(btl, BattleStatusConst.IdleDying) ? 1 : 0);
		btl_mot.setMotion(btl, btl.bi.def_idle);
		btl.evt.animFrame = 0;
	}

	public static void OrganizeEnemyData(FF9StateBattleSystem btlsys)
	{
		for (Int32 i = 0; i < BTL_SCENE.GetMonCount(); i++)
		{
			ENEMY_TYPE et = btlsys.enemy[i].et;
			BTL_DATA btl = btlsys.btl_data[4 + i];
			btl.level = et.level;
			btl.max.at = (Int16)((60 - btl.elem.dex) * 40 << 2);
			btl_para.InitATB(btl);
			btl.cur.at = (Int16)(Comn.random16() % btl.max.at);
			btl.weapon = null;
			btl_stat.InitCountDownStatus(btl);
			btl_mot.HideMesh(btl, btl.mesh_current, false);
			if (btl.bi.slave != 0)
			{
				btl.cur.at = 0;
				btl.cur.at_coef = 0;
				btl.gameObject.transform.localRotation = btl.rot;
				btl.gameObject.transform.localPosition = btl.evt.posBattle;
				btl.currentAnimationName = btl.mot[btl.bi.def_idle];
				btl_mot.setMotion(btl, btl.currentAnimationName);
				btl_mot.setSlavePos(btl, ref btl.base_pos);
				UnityEngine.Object.Destroy(btl.gameObject);
				UnityEngine.Object.Destroy(btl.getShadow());
				btl.gameObject = btl_util.GetMasterEnemyBtlPtr().Data.gameObject;
			}
			else
			{
				btl.base_pos[0] = btl.evt.posBattle[0];
				btl.base_pos[1] = btl.evt.posBattle[1];
				btl.base_pos[2] = btl.evt.posBattle[2];
				btl.currentAnimationName = btl.mot[btl.bi.def_idle];
				btl.evt.animFrame = (Byte)(Comn.random8() % GeoAnim.geoAnimGetNumFrames(btl));
			}
			BattleStatus permanent_stat = btl.stat.permanent;
			BattleStatus current_stat = btl.stat.cur;
			btl.stat.permanent = 0;
			btl.stat.cur = 0;
			btl_stat.MakeStatusesPermanent(btl, permanent_stat);
			btl_stat.AlterStatuses(btl, current_stat);
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
		btl.SetDisappear(false, 1);
		bi.shadow = 1;
		bi.cover_unit = null;
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
		btl.delayedModifierList.Clear();
		btl.summon_count = 0;
		btl.critical_rate_deal_bonus = 0;
		btl.critical_rate_receive_bonus = 0;
		btl.is_monster_transform = false;
		btl.killer_track = null;
		btl.enable_trance_glow = false;
	}

	public static void SetBattleModel(BTL_DATA btl)
	{
		// Set normal model
		String modelName = (btl.dms_geo_id == -1) ? String.Empty : FF9BattleDB.GEO.GetValue(btl.dms_geo_id);
		Int32 scale = 1;
		if (ModelFactory.HaveUpScaleModel(modelName))
			scale = 4;

		GEOTEXHEADER textureAnim = new GEOTEXHEADER();
		textureAnim.ReadPlayerTextureAnim(btl, "Models/GeoTexAnim/" + modelName + ".tab", scale);
		btl.texanimptr = textureAnim;

		// Set trance model
		if (btl.bi.player == 0)
			return;

		String tranceModelName = btl_mot.BattleParameterList[btl_util.getSerialNumber(btl)].TranceModelId;
		GEOTEXHEADER tranceTextureAnim = new GEOTEXHEADER();
		tranceTextureAnim.ReadTrancePlayerTextureAnim(btl, tranceModelName, scale);
		btl.tranceTexanimptr = tranceTextureAnim;
	}

	public const Byte BTL_LOAD_BG_DONE = 1;
	public const Byte BTL_LOAD_ENEMY_DONE = 2;
	public const Byte BTL_LOAD_PLAYER_DONE = 4;
	public const Byte BTL_WAIT_ENEMY_STONE_DONE = 8;
	public const Byte BTL_WAIT_WEAPON_STONE_DONE = 16;
	public const Byte BTL_WAIT_ENEMY_APPEAR_DONE = 32;
	public const Byte BTL_WAIT_PLAYER_APPEAR_DONE = 64;

	private static readonly UInt32[] enemy_dummy_sa = new UInt32[2];
}