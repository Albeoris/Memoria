using System;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Assets;
using UnityEngine;
using Object = System.Object;

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
        if (cmd_no == BattleCommandId.AutoPotion || cmd_no == BattleCommandId.Item)
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
        else if (cmd_no == BattleCommandId.Throw)
        {
            Byte shape = ff9item._FF9Item_Data[btl_util.GetCommandItem(cmd)].shape;
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
        return SpecialEffect.Special_No_Effect;
    }

    public static void SelectCommandVfx(CMD_DATA cmd)
    {
        if (cmd.cmd_no == BattleCommandId.Phantom)
        {
            BattleAbilityId abilId = btl_util.GetCommandMainActionIndex(cmd);
            if (abilId == BattleAbilityId.Shiva)
                FF9StateSystem.Battle.FF9Battle.phantom_no = BattleAbilityId.DiamondDust;
            else if (abilId == BattleAbilityId.Ifrit)
                FF9StateSystem.Battle.FF9Battle.phantom_no = BattleAbilityId.FlamesofHell;
            else if (abilId == BattleAbilityId.Ramuh)
                FF9StateSystem.Battle.FF9Battle.phantom_no = BattleAbilityId.JudgementBolt;
            else if (abilId == BattleAbilityId.Atomos)
                FF9StateSystem.Battle.FF9Battle.phantom_no = BattleAbilityId.WormHole;
            else if (abilId == BattleAbilityId.Odin)
                FF9StateSystem.Battle.FF9Battle.phantom_no = BattleAbilityId.Zantetsuken;
            else if (abilId == BattleAbilityId.Leviathan)
                FF9StateSystem.Battle.FF9Battle.phantom_no = BattleAbilityId.Tsunami;
            else if (abilId == BattleAbilityId.Bahamut)
                FF9StateSystem.Battle.FF9Battle.phantom_no = BattleAbilityId.MegaFlare;
            else if (abilId == BattleAbilityId.Ark)
                FF9StateSystem.Battle.FF9Battle.phantom_no = BattleAbilityId.EternalDarkness;
        }
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
            btl.dms_geo_id = btl_init.GetModelID(serialNo, isTrance);
            GeoTexAnim.geoTexAnimPlay(btl.texanimptr, 2);
        }
        btl.meshCount = 0;
        foreach (Object obj in btl.gameObject.transform)
        {
            Transform transform = (Transform)obj;
            if (transform.name.Contains("mesh"))
                btl.meshCount++;
        }
        btl.meshIsRendering = new Boolean[btl.meshCount];
        for (Int32 i = 0; i < btl.meshCount; i++)
            btl.meshIsRendering[i] = true;
        btl_util.GeoSetABR(btl.gameObject, "PSX/BattleMap_StatusEffect");
        BattlePlayerCharacter.InitAnimation(btl);
        //btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL);
        geo.geoAttach(btl.weapon_geo, btl.gameObject, FF9StateSystem.Common.FF9.player[(CharacterId)btl.bi.slot_no].wep_bone);
        //btl_eqp.InitWeapon(FF9StateSystem.Common.FF9.player[(CharacterId)btl.bi.slot_no], btl);
        AnimationFactory.AddAnimToGameObject(btl.gameObject, btl_mot.BattleParameterList[serialNo].ModelId, true);
    }

    public static SpecialEffect GetPlayerAttackVfx(BTL_DATA btl)
    {
        CharacterSerialNumber serialNo = btl_util.getSerialNumber(btl);
        if (serialNo != CharacterSerialNumber.NONE)
            return btl_mot.BattleParameterList[serialNo].AttackSequence;
        return SpecialEffect.Special_No_Effect;
    }
}