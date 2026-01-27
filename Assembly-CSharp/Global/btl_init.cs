using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Linq;
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

        if (FF9BattleDB.GEO.TryGetKey(modelId, out Int32 geoId))
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
            SB2_PUT enemyPlacement = btl_scene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].Monster[enemyIndex];
            SB2_MON_PARM monParam = btl_scene.MonAddr[enemyPlacement.TypeNo];
            btlshadow.ff9battleShadowInit(monBtl);
            enemy.info.die_fade_rate = 32;
            if ((monBtl.dms_geo_id = BTL_SCENE.GetMonGeoID(enemyPlacement)) < 0)
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
        pBtl.uiColorHP = FF9TextTool.White;
        pBtl.uiColorMP = FF9TextTool.White;
        pBtl.uiSpriteATB = BattleHUD.ATENormal;
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
        enemy.info.flags = pParm.Flags;
        btl_util.SetShadow(pBtl, pParm.ShadowX, pParm.ShadowZ);
        pBtl.shadow_bone[0] = pParm.ShadowBone;
        pBtl.shadow_bone[1] = pParm.ShadowBone2;
        pBtl.geo_scale_x = pBtl.geo_scale_y = pBtl.geo_scale_z = pBtl.geo_scale_default = 4096;
        pBtl.geoScaleStatus = Vector3.one;
        pBtl.animSpeedStatusFactor = 1f;
        btl_abil.CheckStatusAbility(new BattleUnit(pBtl));
    }

    public static void PutMonster(SB2_PUT pPut, BTL_DATA pBtl, BTL_SCENE pScene, Int32 pNo)
    {
        Int16 startTypeAngle = (Int16)(pScene.Info.StartType == battle_start_type_tags.BTL_START_FIRST_ATTACK ? 180 : 0);
        ENEMY enemy = FF9StateSystem.Battle.FF9Battle.enemy[pBtl.bi.slot_no];
        enemy.et = FF9StateSystem.Battle.FF9Battle.enemy_type[pPut.TypeNo];
        pBtl.bi.target = (Byte)(((pPut.Flags & SB2_PUT.FLG_TARGETABLE) != 0) ? 1 : 0);
        pBtl.bi.row = 2;
        CopyPoints(pBtl.max, FF9StateSystem.Battle.FF9Battle.enemy_type[pPut.TypeNo].max);
        pBtl.cur.hp = pBtl.max.hp;
        pBtl.cur.mp = pBtl.max.mp;
        enemy.info.multiple = (Byte)(((pPut.Flags & SB2_PUT.FLG_MULTIPART) != 0) ? 1 : 0);
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
                if (btl_stat.CheckStatus(btl, BattleStatus.Death))
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
        Int16 playerCount = 0;
        for (Int32 i = 0; i < 4; i++)
            if (FF9StateSystem.Common.FF9.party.member[i] != null)
                playerCount++;
        Int32 gap = 632;
        Int32 posLeftRight = (Int16)((playerCount - 1) * gap / 2);
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.bi.player == 0)
                break;
            SetupBattlePlayerSingle(btl, posLeftRight, btl_scene.Info.StartType);
            posLeftRight -= gap;
        }
    }

    public static void SetupBattlePlayerSingle(BTL_DATA btl, Int32 posLeftRight, battle_start_type_tags startType)
    {
        Int32 baseAngle = (Int16)((startType != battle_start_type_tags.BTL_START_BACK_ATTACK) ? 180 : 0);
        CharacterId charId = (CharacterId)btl.bi.slot_no;
        btl.bi.row = FF9StateSystem.Common.FF9.player[charId].info.row;
        if (startType == battle_start_type_tags.BTL_START_BACK_ATTACK)
            btl.bi.row ^= 1;
        btl.original_pos[0] = posLeftRight;
        btl.evt.posBattle[0] = posLeftRight;
        btl.base_pos[0] = posLeftRight;
        btl.pos[0] = posLeftRight;
        Single posHeight = 0; // btl_stat.CheckStatus(btl, BattleStatus.Float) ? -200 : 0;
        btl.original_pos[1] = 0;
        btl.evt.posBattle[1] = posHeight;
        btl.base_pos[1] = posHeight;
        btl.pos[1] = posHeight;
        Single posz = btl_init.PLAYER_ORIGINAL_Z + ((btl.bi.row == 0) ? -400 : 0);
        btl.original_pos[2] = btl_init.PLAYER_ORIGINAL_Z;
        btl.evt.posBattle[2] = posz;
        btl.base_pos[2] = posz;
        btl.pos[2] = posz;
        btl.evt.rotBattle = Quaternion.Euler(new Vector3(0f, 180f, 180f));
        btl.rot = Quaternion.Euler(new Vector3(0f, baseAngle, 180f));
        //next.rot = (next.evt.rotBattle = Quaternion.Euler(new Vector3(0f, baseAngle, 180f)));
        btl.gameObject.transform.localPosition = btl.pos;
        btl.gameObject.transform.localRotation = btl.rot;
        CharacterBattleParameter btlParam = btl_mot.BattleParameterList[FF9StateSystem.Common.FF9.player[charId].info.serial_no];
        btl.shadow_bone[0] = btlParam.ShadowData[0];
        btl.shadow_bone[1] = btlParam.ShadowData[1];
        btl_util.SetShadow(btl, btlParam.ShadowData[2], btlParam.ShadowData[3]);
        btl.geo_scale_x = btl.geo_scale_y = btl.geo_scale_z = btl.geo_scale_default = 4096;
        GameObject shadowObj = FF9StateSystem.Battle.FF9Battle.map.shadowArray[btl];
        Vector3 shadowPos = shadowObj.transform.localPosition;
        shadowPos.z = btlParam.ShadowData[4];
        shadowObj.transform.localPosition = shadowPos;
    }

    public static void OrganizePlayerData(PLAYER p, BTL_DATA btl, UInt16 cnt, UInt16 btl_no, Boolean reinit = false)
    {
        BattleUnit unit = new BattleUnit(btl);
        CharacterBattleParameter btlParam = btl_mot.BattleParameterList[p.info.serial_no];
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
        btl.tar_bone = FF9BattleDBHeightAndRadius.TryFindNewTargetBone(btl.dms_geo_id);
        btl.sa = p.sa;
        btl.saExtended = p.saExtended;
        btl.saMonster = new List<SupportingAbilityFeature>();
        btl.elem.dex = p.elem.dex;
        btl.elem.str = p.elem.str;
        btl.elem.mgc = p.elem.mgc;
        btl.elem.wpr = p.elem.wpr;
        btl.level = p.level;
        Single atbProgress = reinit ? (Single)btl.cur.at / btl.max.at : 0f;
        btl_init.CopyPoints(btl.max, p.max);
        btl_init.CopyPoints(btl.cur, p.cur);
        btl.maxDamageLimit = p.maxDamageLimit;
        btl.maxMpDamageLimit = p.maxMpDamageLimit;
        btl.uiColorHP = FF9TextTool.White;
        btl.uiColorMP = FF9TextTool.White;
        btl.uiLabelHP = null;
        btl.uiLabelMP = null;
        btl.uiSpriteATB = BattleHUD.ATENormal;
        btl.uiSpriteATB = BattleHUD.ATENormal;
        FF9Char ff9Char = reinit ? FF9StateSystem.Common.FF9.charArray.Values.FirstOrDefault(ch => ch.btl == btl) : new FF9Char();
        if (ff9Char == null)
            ff9Char = new FF9Char();
        ff9Char.btl = btl;
        ff9Char.evt = btl.evt;
        if (ff9play.CharacterIDToEventId(p.Index) >= 0)
            FF9StateSystem.Common.FF9.charArray[ff9play.CharacterIDToEventId(p.Index)] = ff9Char;
        else
            FF9StateSystem.Common.FF9.charArray[9 + (Int32)p.Index] = ff9Char;
        btl_init.InitBattleData(btl, ff9Char);
        btl.mesh_banish = UInt16.MaxValue;
        btl_para.SetupATBCoef(btl, btl_para.GetATBCoef());
        if (reinit)
        {
            btl.max.at = btl_para.GetMaxATB(unit);
            btl.cur.at = (Int16)Math.Floor(atbProgress * btl.max.at);
        }
        else
        {
            btl.max.at = btl_para.GetMaxATB(unit);
            if (FF9StateSystem.Battle.FF9Battle.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_BACK_ATTACK)
                btl.cur.at = 0;
            else if (FF9StateSystem.Battle.FF9Battle.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_FIRST_ATTACK)
                btl.cur.at = (Int16)(btl.max.at - 1);
            else
                btl.cur.at = (Int16)(Comn.random16() % btl.max.at);
        }
        btl.geoScaleStatus = Vector3.one;
        btl.animSpeedStatusFactor = 1f;
        btl.mot = new String[34];
        btl_mot.SetPlayerDefMotion(btl, p.info.serial_no);
        BattlePlayerCharacter.InitAnimation(btl);
        btl.weapon_bone = btlParam.WeaponBone;
        btl_eqp.InitWeapon(p, btl);
        btl.weaponModels[0].scale = btlParam.WeaponSize.ToVector3(true);
        btl.weaponModels[0].offset_pos = btlParam.WeaponOffsetPos.ToVector3(false);
        btl.weaponModels[0].offset_rot = btlParam.GetWeaponRotationFixed(btl.weapon.ModelId, false);
        btl.defence.PhysicalDefence = p.defence.PhysicalDefence;
        btl.defence.PhysicalEvade = p.defence.PhysicalEvade;
        btl.defence.MagicalDefence = p.defence.MagicalDefence;
        btl.defence.MagicalEvade = p.defence.MagicalEvade;
        btl_eqp.InitEquipPrivilegeAttrib(p, btl);
        btl_util.GeoSetColor2Source(btl.weapon_geo, 0, 0, 0);
        btl.stat.permanent = p.permanent_status;
        btl.stat.cur = p.status;
        btl_abil.CheckStatusAbility(unit);
        if (reinit)
            btl.stat.cur = p.status;
        BattleStatus resist_stat = btl.stat.invalid;
        BattleStatus permanent_stat = btl.stat.permanent;
        BattleStatus current_stat = btl.stat.cur;
        btl.stat.invalid = 0;
        btl.stat.permanent = 0;
        btl.stat.cur = 0;
        btl_stat.MakeStatusesPermanent(unit, permanent_stat);
        btl_stat.AlterStatuses(unit, current_stat);
        btl.stat.invalid = resist_stat;
        btl.base_pos = btl.evt.posBattle;
        Int16 geoID = btl.dms_geo_id;
        btl.height = 0;
        btl.radius_effect = 0;
        btl.radius_collision = 256;

        FF9BattleDBHeightAndRadius.TryFindHeightAndRadius(geoID, ref btl.height, ref btl.radius_effect);

        if (btl.cur.hp == 0 && btl_stat.AlterStatus(unit, BattleStatusId.Death) == btl_stat.ALTER_SUCCESS)
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
            BattleUnit unit = new BattleUnit(btl);
            btl.level = et.level;
            btl.max.at = btl_para.GetMaxATB(unit);
            btl_para.SetupATBCoef(btl, btl_para.GetATBCoef());
            btl.cur.at = (Int16)(Comn.random16() % btl.max.at);
            btl.weapon = null;
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
                btl.gameObject = btl_util.GetMasterEnemyBtlPtr(btl).Data.gameObject;
            }
            else
            {
                btl.base_pos[0] = btl.evt.posBattle[0];
                btl.base_pos[1] = btl.evt.posBattle[1];
                btl.base_pos[2] = btl.evt.posBattle[2];
                for (Int16 j = 0; j < btl.mot.Length; j++) // [DV] Check each anims if a clip exist, otherwise create them (if we don't that for custom anim, the battle is frozen).
                    AnimationFactory.AddAnimWithAnimatioName(btl.gameObject, btl.mot[j]);
                btl.currentAnimationName = btl.mot[btl.bi.def_idle];
                btl.evt.animFrame = (Byte)(Comn.random8() % GeoAnim.geoAnimGetNumFrames(btl));
            }
            BattleStatus permanent_stat = btl.stat.permanent;
            BattleStatus current_stat = btl.stat.cur;
            btl.stat.permanent = 0;
            btl.stat.cur = 0;
            btl_stat.MakeStatusesPermanent(unit, permanent_stat);
            btl_stat.AlterStatuses(unit, current_stat);
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
        btl.fig.info = 0;
        btl.fig.hp = 0;
        btl.fig.mp = 0;
        btl.fig.modifiers.Clear();
        btl.die_seq = 0;
        ff9char.btl = btl;
        btl.evt = ff9char.evt;
        GeoTexAnim.geoTexAnimPlay(btl.texanimptr, 2);
        btl_util.GeoSetColor2Source(btl.gameObject, 0, 0, 0);
        btl.mesh_current = 0;
        // Out of reach: enemies inside battles flagged with "NoNeighboring" are all set to out of reach but they can also be placed individually using SV_FunctionEnemy[110] in battle scripts or setting the "OutOfReach" flag in the battle's ".memnfo" file
        // False by default for player character, initialized in "btl_init.SetMonsterData" for enemies
        btl.out_of_reach = false;
        btl.delayedModifierList.Clear();
        btl.summon_count = 0;
        btl.critical_rate_deal_bonus = 0;
        btl.critical_rate_receive_resistance = 0;
        btl.is_monster_transform = false;
        btl.killer_track = null;
        btl.bonus_given = false;
        btl.enable_trance_glow = false;
    }

    public static void SetBattleModel(BTL_DATA btl)
    {
        // Set normal model
        String modelName = (btl.dms_geo_id == -1) ? String.Empty : FF9BattleDB.GEO.GetValue(btl.dms_geo_id);

        GEOTEXHEADER textureAnim = new GEOTEXHEADER();
        textureAnim.ReadBattleTextureAnim(btl, "Models/GeoTexAnim/" + modelName + ".tab");
        btl.texanimptr = textureAnim;

        // Set trance model
        if (btl.bi.player == 0)
            return;

        String tranceModelName = btl_mot.BattleParameterList[btl_util.getSerialNumber(btl)].TranceModelId;
        GEOTEXHEADER tranceTextureAnim = new GEOTEXHEADER();
        tranceTextureAnim.ReadTranceTextureAnim(btl, tranceModelName);
        btl.tranceTexanimptr = tranceTextureAnim;
    }

    public static void SwapPlayerCharacter(BattleUnit unit, PLAYER newChar)
    {
        BTL_DATA btl = unit.Data;
        // HonoluluBattleMain.CreateBattleData
        BattlePlayerCharacter.CreatePlayer(btl, newChar);
        Int32 meshCount = 0;
        foreach (Transform transform in btl.gameObject.transform)
            if (transform.name.Contains("mesh"))
                meshCount++;
        btl.meshCount = meshCount;
        btl.meshIsRendering = new Boolean[meshCount];
        for (Int32 i = 0; i < meshCount; ++i)
            btl.meshIsRendering[i] = true;
        btl.animation = btl.gameObject.GetComponent<Animation>();
        // InitPlayerData
        btl.dms_geo_id = GetModelID(newChar.info.serial_no);
        OrganizePlayerData(newChar, btl, btl.bi.line_no, (UInt16)unit.GetIndex(), true);
        SetBattleModel(btl);
        if (btl_stat.CheckStatus(btl, BattleStatus.Death))
        {
            GeoTexAnim.geoTexAnimStop(btl.texanimptr, 2);
            GeoTexAnim.geoTexAnimPlayOnce(btl.texanimptr, 0);
            GeoTexAnim.geoTexAnimStop(btl.tranceTexanimptr, 2);
            GeoTexAnim.geoTexAnimPlayOnce(btl.tranceTexanimptr, 0);
        }
        else
        {
            GeoTexAnim.geoTexAnimPlay(btl.texanimptr, 2);
        }
        btl_sys.AddCharacter(btl);
        // Part of SetupBattlePlayer
        SetupBattlePlayerSingle(btl, (Int32)btl.original_pos[0], battle_start_type_tags.BTL_START_NORMAL_ATTACK);
    }

    public const Int32 PLAYER_ORIGINAL_Z = -1560;

    public const Byte BTL_LOAD_BG_DONE = 1;
    public const Byte BTL_LOAD_ENEMY_DONE = 2;
    public const Byte BTL_LOAD_PLAYER_DONE = 4;
    public const Byte BTL_WAIT_ENEMY_STONE_DONE = 8;
    public const Byte BTL_WAIT_WEAPON_STONE_DONE = 16;
    public const Byte BTL_WAIT_ENEMY_APPEAR_DONE = 32;
    public const Byte BTL_WAIT_PLAYER_APPEAR_DONE = 64;

    private static readonly UInt32[] enemy_dummy_sa = new UInt32[2];
}
