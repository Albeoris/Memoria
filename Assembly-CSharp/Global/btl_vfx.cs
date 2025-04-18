using System;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;

public static class btl_vfx
{
    public static void SetBattleVfx(CMD_DATA cmd, SpecialEffect fx_no, Int16[] arg = null)
    {
        cmd.vfxRequest.SetupVfxRequest(cmd, arg);
        cmd.vfxRequest.PlaySFX(fx_no);
    }

    public static void LoopBattleVfxForReflect(CMD_DATA cmd, UInt32 fx_no)
    {
        cmd.tar_id = btl_cmd.MergeReflecTargetID(cmd.reflec);
        cmd.info.reflec = 1;
        if (cmd.regist.bi.player == 0)
            cmd.info.mon_reflec = 1;
        cmd.vfxRequest.SetupVfxRequest(cmd, new Int16[] { cmd.vfxRequest.monbone[0], cmd.vfxRequest.monbone[1], cmd.vfxRequest.arg0, (Int16)cmd.vfxRequest.flgs });
        cmd.vfxRequest.flgs |= 17;
        cmd.vfxRequest.PlaySFX((SpecialEffect)fx_no);
    }

    public static Boolean UseBeatrixAlternateVfx(BTL_DATA caster, SpecialEffect vfx1, SpecialEffect vfx2)
    {
        // Check if vfx1 and vfx2 are the two versions of a Sword Art spell animation: use the 2nd version when used by Beatrix
        if (caster.bi.player == 0 || (CharacterId)caster.bi.slot_no != CharacterId.Beatrix) return false;
        if (vfx1 == SpecialEffect.Darkside_1 && vfx2 == SpecialEffect.Darkside_2) return true;
        if (vfx1 == SpecialEffect.Minus_Strike_1 && vfx2 == SpecialEffect.Minus_Strike_2) return true;
        if (vfx1 == SpecialEffect.Iai_Strike_1 && vfx2 == SpecialEffect.Iai_Strike_2) return true;
        if (vfx1 == SpecialEffect.Thunder_Slash_1 && vfx2 == SpecialEffect.Thunder_Slash_2) return true;
        if (vfx1 == SpecialEffect.Shock_1 && vfx2 == SpecialEffect.Shock_2) return true;
        if (vfx1 == SpecialEffect.Stock_Break_1 && vfx2 == SpecialEffect.Stock_Break_2) return true;
        if (vfx1 == SpecialEffect.Climhazzard_1 && vfx2 == SpecialEffect.Climhazzard_2) return true;
        return false;
    }

    public static SpecialEffect GetPlayerCommandSFX(CMD_DATA cmd)
    {
        BTL_DATA regist = cmd.regist;
        BattleCommandId cmd_no = cmd.cmd_no;
        CharacterCommandType cmdType = btl_util.GetCommandTypeSafe(cmd_no);
        if (cmdType == CharacterCommandType.Item)
            return (SpecialEffect)ff9item.GetItemEffect(btl_util.GetCommandItem(cmd)).info.VfxIndex;
        else if (cmd_no == BattleCommandId.SysTrans)
            return btl_stat.CheckStatus(regist, BattleStatus.Trance) ? SpecialEffect.Special_Trance_Activate : SpecialEffect.Special_Trance_End;
        else if (cmd_no == BattleCommandId.Attack)
            return btl_vfx.GetPlayerAttackVfx(regist);
        else if (cmd_no == BattleCommandId.Defend || cmd_no == BattleCommandId.Change)
            return SpecialEffect.Special_No_Effect;
        else if (cmd_no == BattleCommandId.Steal)
        {
            Byte serialNumber = (Byte)btl_util.getSerialNumber(regist);
            if (serialNumber == 0)
                return SpecialEffect.Steal_Zidane_Dagger;
            else if (serialNumber == 1)
                return SpecialEffect.Steal_Zidane_Sword;
            else if (serialNumber == 14)
                return SpecialEffect.Steal_Cinna;
            else if (serialNumber == 15)
                return SpecialEffect.Steal_Marcus;
            else
                return SpecialEffect.Steal_Blank;
        }
        else if (cmdType == CharacterCommandType.Throw)
        {
            Int32 shape = ff9item._FF9Item_Data[btl_util.GetCommandItem(cmd)].shape;
            if (shape == 1)
                return SpecialEffect.Throw_Dagger;
            else if (shape == 2)
                return SpecialEffect.Throw_Thief_Sword;
            else if (shape == 3 || shape == 4)
                return SpecialEffect.Throw_Sword;
            else if (shape == 5)
                return SpecialEffect.Throw_Spear;
            else if (shape == 6)
                return SpecialEffect.Throw_Claw;
            else if (shape == 7)
                return SpecialEffect.Throw_Racket;
            else if (shape == 8 || shape == 9 || shape == 10)
                return SpecialEffect.Throw_Rod;
            else if (shape == 11)
                return SpecialEffect.Throw_Fork;
            else
                return SpecialEffect.Throw_Disc;
        }
        else
        {
            BattleAbilityId abilId = btl_util.GetCommandMainActionIndex(cmd);
            if (btl_util.IsCommandMonsterTransformAttack(cmd))
                return btl_vfx.GetPlayerAttackVfx(regist);
            else if (cmd.PatchedVfx != SpecialEffect.Special_No_Effect)
                return cmd.PatchedVfx;
            else if (abilId == BattleAbilityId.Attack)
                return btl_vfx.GetPlayerAttackVfx(regist);
            else if ((cmd.aa.Info.Target == TargetType.ManyAny && cmd.info.cursor == 0) || cmd.info.meteor_miss != 0 || cmd.info.short_summon != 0 || btl_vfx.UseBeatrixAlternateVfx(regist, (SpecialEffect)cmd.aa.Info.VfxIndex, (SpecialEffect)cmd.aa.Vfx2))
                return (SpecialEffect)cmd.aa.Vfx2;
            else
                return (SpecialEffect)cmd.aa.Info.VfxIndex;
        }
    }

    public static void SelectCommandVfx(CMD_DATA cmd)
    {
        BTL_DATA regist = cmd.regist;

        if (Configuration.Battle.SFXRework)
        {
            if (cmd.cmd_no != BattleCommandId.MagicCounter)
            {
                if (cmd.aa.Info.VfxAction == null && !String.IsNullOrEmpty(cmd.aa.Info.SequenceFile))
                {
                    String sequenceText = AssetManager.LoadString(cmd.aa.Info.SequenceFile);
                    if (sequenceText != null)
                        cmd.aa.Info.VfxAction = new UnifiedBattleSequencer.BattleAction(sequenceText);
                }
                if (cmd.aa.Info.SequenceFile == null && regist.bi.player == 0)
                {
                    if (regist.dms_geo_id == 427 && cmd.aa.Info.VfxIndex == 457) // Thunder Slash with Beatrix (Boss version)
                        cmd.aa.Info.VfxAction = new UnifiedBattleSequencer.BattleAction(ThunderSlashBeatrixFix);
                }
                if (cmd.aa.Info.VfxAction != null)
                {
                    UnifiedBattleSequencer.BattleAction action = new UnifiedBattleSequencer.BattleAction(cmd.aa.Info.VfxAction);
                    action.Execute(cmd);
                    return;
                }
                if (regist != null && regist.bi.player == 0 && cmd.cmd_no != BattleCommandId.SysTrans)
                {
                    UnifiedBattleSequencer.BattleAction action = new UnifiedBattleSequencer.BattleAction(UnifiedBattleSequencer.EffectType.EnemySequence, cmd.sub_no);
                    action.Execute(cmd);
                    return;
                }
            }
            SpecialEffect sfxNum = GetPlayerCommandSFX(cmd);
            if (sfxNum != SpecialEffect.Special_No_Effect)
            {
                UnifiedBattleSequencer.BattleAction action = new UnifiedBattleSequencer.BattleAction(UnifiedBattleSequencer.EffectType.SpecialEffect, (Int32)sfxNum);
                action.Execute(cmd);
                return;
            }
            else if (cmd.cmd_no == BattleCommandId.Change || cmd.cmd_no == BattleCommandId.Defend)
            {
                String seqName = cmd.cmd_no == BattleCommandId.Change ? "SequenceChange" : "SequenceDefend";
                String sequenceText = AssetManager.LoadString(DataResources.PureDataDirectory + "SpecialEffects/Common/" + seqName + UnifiedBattleSequencer.EXTENSION_SEQ);
                if (sequenceText != null)
                {
                    UnifiedBattleSequencer.BattleAction action = new UnifiedBattleSequencer.BattleAction(sequenceText);
                    action.Execute(cmd);
                    return;
                }
            }
        }
        else
        {
            switch (cmd.cmd_no)
            {
                case BattleCommandId.EnemyAtk:
                case BattleCommandId.EnemyCounter:
                case BattleCommandId.EnemyDying:
                case BattleCommandId.EnemyReaction:
                    btlseq.RunSequence(cmd);
                    break;
                default:
                    SpecialEffect sfxNum = GetPlayerCommandSFX(cmd);
                    if (sfxNum != SpecialEffect.Special_No_Effect)
                        SetBattleVfx(cmd, sfxNum, null);
                    else if (cmd.cmd_no == BattleCommandId.Change)
                        UIManager.Battle.SetBattleCommandTitle(cmd);
                    else if (cmd.cmd_no == BattleCommandId.Defend)
                    {
                        cmd.info.effect_counter++;
                        UIManager.Battle.SetBattleCommandTitle(cmd);
                        btl_cmd.ExecVfxCommand(regist, cmd);
                    }
                    break;
            }
        }
    }

    public static void SetTranceModel(BTL_DATA btl, Boolean isTrance)
    {
        CharacterSerialNumber serialNo = btl_util.getSerialNumber(btl);
        CharacterBattleParameter btlParam = btl_mot.BattleParameterList[serialNo];
        BattlePlayerCharacter.PlayerMotionIndex currentMotion = btl_mot.getMotion(btl);
        if (isTrance)
        {
            btl.battleModelIsRendering = true;
            btl.ChangeModel(btl.tranceGo, btl_init.GetModelID(serialNo, isTrance));
            GeoTexAnim.geoTexAnimPlay(btl.tranceTexanimptr, 2);
        }
        else
        {
            btl.battleModelIsRendering = true;
            btl.ChangeModel(btl.originalGo, btl_init.GetModelID(serialNo, isTrance));
            GeoTexAnim.geoTexAnimPlay(btl.texanimptr, 2);
        }
        btl_util.GeoSetABR(btl.gameObject, "PSX/BattleMap_StatusEffect", btl);
        btl_mot.SetPlayerDefMotion(btl, serialNo, isTrance);
        BattlePlayerCharacter.InitAnimation(btl);
        if (currentMotion != BattlePlayerCharacter.PlayerMotionIndex.MP_MAX)
            btl.currentAnimationName = btl.mot[(Int32)currentMotion];
        if (isTrance && btlParam.TranceParameters)
        {
            btl.weapon_bone = btl.weapon.ModelId != UInt16.MaxValue ? btlParam.TranceWeaponBone : -1;
            btl.weaponModels[0].scale = btlParam.TranceWeaponSize.ToVector3(true);
            btl.weaponModels[0].offset_pos = btlParam.TranceWeaponOffsetPos.ToVector3(false);
            btl.weaponModels[0].offset_rot = btlParam.GetWeaponRotationFixed(btl.weapon.ModelId, true);
        }
        else
        {
            btl.weapon_bone = btl.weapon.ModelId != UInt16.MaxValue ? btlParam.WeaponBone : -1;
            btl.weaponModels[0].scale = btlParam.WeaponSize.ToVector3(true);
            btl.weaponModels[0].offset_pos = btlParam.WeaponOffsetPos.ToVector3(false);
            btl.weaponModels[0].offset_rot = btlParam.GetWeaponRotationFixed(btl.weapon.ModelId, false);
        }
        geo.geoAttach(btl.weapon_geo, btl.gameObject, btl.weapon_bone);
        btl2d.ShowMessages(true);
    }

    public static SpecialEffect GetPlayerAttackVfx(BTL_DATA btl)
    {
        CharacterSerialNumber serialNo = btl_util.getSerialNumber(btl);
        if (serialNo != CharacterSerialNumber.NONE)
            return (btl_mot.BattleParameterList[serialNo].TranceParameters && btl_stat.CheckStatus(btl, BattleStatus.Trance)) ? 
                btl_mot.BattleParameterList[serialNo].TranceAttackSequence : btl_mot.BattleParameterList[serialNo].AttackSequence;
        return SpecialEffect.Special_No_Effect;
    }

    const String ThunderSlashBeatrixFix = "SetupReflect: Delay=SFXLoaded\r\nLoadMonsterSFX: SFX=Thunder_Slash_3 ; FirstBone=0 ; SecondBone=0 ; Args=0 ; Reflect=True\r\nWaitMonsterSFXLoaded: Reflect=True\r\nWaitAnimation: Char=Caster\r\nMessage: Text=[CastName] ; Priority=1 ; Title=True ; Reflect=False\r\nPlayAnimation: Char=Caster ; Anim=ANH_MON_B3_155_010\r\nWaitAnimation: Char=Caster\r\nPlayAnimation: Char=Caster ; Anim=ANH_MON_B3_155_011\r\nTurn: Char=Caster ; BaseAngle=AllTargets ; Angle=0 ; Time=6\r\nMoveToTarget: Char=Caster ; Target=AllTargets ; Time=6 ; Distance=1300\r\nWaitMove: Char=Caster\r\nPlayAnimation: Char=Caster ; Anim=ANH_MON_B3_155_012\r\nStartThread\r\n\tWait: Time=1\r\n\tPlaySound: Sound=673 ; Volume=1 ; Once=False\r\nEndThread\r\nWait: Time=10\r\nPlayMonsterSFX: Reflect=True\r\nWaitAnimation: Char=Caster\r\nPlayAnimation: Char=Caster ; Anim=ANH_MON_B3_155_013\r\nTurn: Char=Caster ; BaseAngle=Default ; Angle=0 ; Time=5\r\nMoveToPosition: Char=Caster ; Time=5 ; AbsolutePosition=Default ; MoveHeight=true\r\nWaitMove: Char=Caster\r\nPlayAnimation: Char=Caster ; Anim=ANH_MON_B3_155_014\r\nWaitAnimation: Char=Caster\r\nPlayAnimation: Char=Caster ; Anim=Idle\r\nWaitMonsterSFXDone: Reflect=True\r\nWaitTurn: Char=Caster\r\nActivateReflect\r\nWaitReflect\r\n";
}
