using System;
using FF9;
using Memoria;
using Memoria.Data;
using UnityEngine;
using Object = System.Object;

public class btl_vfx
{
	public static void SetBattleVfx(CMD_DATA cmd, UInt32 fx_no, Int16[] arg = null)
	{
        cmd.vfxRequest.SetupVfxRequest(cmd, arg);
        cmd.vfxRequest.PlaySFX((SpecialEffect)fx_no);
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
        if (caster.bi.player != 0 || caster.bi.slot_no != CharacterIndex.Beatrix) return false;
        if (vfx1 == SpecialEffect.Darkside_1 && vfx2 == SpecialEffect.Darkside_2) return true;
        if (vfx1 == SpecialEffect.Minus_Strike_1 && vfx2 == SpecialEffect.Minus_Strike_2) return true;
        if (vfx1 == SpecialEffect.Iai_Strike_1 && vfx2 == SpecialEffect.Iai_Strike_2) return true;
        if (vfx1 == SpecialEffect.Thunder_Slash_1 && vfx2 == SpecialEffect.Thunder_Slash_2) return true;
        if (vfx1 == SpecialEffect.Shock_1 && vfx2 == SpecialEffect.Shock_2) return true;
        if (vfx1 == SpecialEffect.Stock_Break_1 && vfx2 == SpecialEffect.Stock_Break_2) return true;
        if (vfx1 == SpecialEffect.Climhazzard_1 && vfx2 == SpecialEffect.Climhazzard_2) return true;
        return false;
    }

    public static Int32 GetPlayerCommandSFX(CMD_DATA cmd)
    {
        BTL_DATA regist = cmd.regist;
        BattleCommandId cmd_no = cmd.cmd_no;
        if (cmd_no == BattleCommandId.AutoPotion || cmd_no == BattleCommandId.Item)
            return ff9item._FF9Item_Info[btl_util.btlItemNum(cmd.sub_no)].info.VfxIndex;
        else if (cmd_no == BattleCommandId.SysTrans)
            return btl_stat.CheckStatus(regist, BattleStatus.Trance) ? (Int32)SpecialEffect.Special_Trance_Activate : (Int32)SpecialEffect.Special_Trance_End;
        else if (cmd_no == BattleCommandId.Attack)
            return 100 + btl_util.getSerialNumber(regist);
        else if (cmd_no == BattleCommandId.Defend || cmd_no == BattleCommandId.Change)
            return -1;
        else if (cmd_no == BattleCommandId.Steal)
        {
            Byte serialNumber = btl_util.getSerialNumber(regist);
            if (serialNumber == 0)
                return (Int32)SpecialEffect.Steal_Zidane_Dagger;
            else if (serialNumber == 1)
                return (Int32)SpecialEffect.Steal_Zidane_Sword;
            else if (serialNumber == 14)
                return (Int32)SpecialEffect.Steal_Cinna;
            else if (serialNumber == 15)
                return (Int32)SpecialEffect.Steal_Marcus;
            else
                return (Int32)SpecialEffect.Steal_Blank;
        }
        else if (cmd_no == BattleCommandId.Throw)
        {
            Byte shape = ff9item._FF9Item_Data[(Int32)cmd.sub_no].shape;
            if (shape == 1)
                return (Int32)SpecialEffect.Throw_Dagger;
            else if (shape == 2)
                return (Int32)SpecialEffect.Throw_Thief_Sword;
            else if (shape == 3 || shape == 4)
                return (Int32)SpecialEffect.Throw_Sword;
            else if (shape == 5)
                return (Int32)SpecialEffect.Throw_Spear;
            else if (shape == 6)
                return (Int32)SpecialEffect.Throw_Claw;
            else if (shape == 7)
                return (Int32)SpecialEffect.Throw_Racket;
            else if (shape == 8 || shape == 9 || shape == 10)
                return (Int32)SpecialEffect.Throw_Rod;
            else if (shape == 11)
                return (Int32)SpecialEffect.Throw_Fork;
            else if (shape == 12)
                return (Int32)SpecialEffect.Throw_Disc;
        }
        else
        {
            if (cmd_no != BattleCommandId.MagicCounter && cmd.sub_no == 176)
                return 100 + btl_util.getSerialNumber(regist);
            else if (regist.is_monster_transform && cmd.sub_no == regist.monster_transform.attack)
                return 100 + btl_util.getSerialNumber(regist);
            else if (cmd.PatchedVfx != SpecialEffect.Special_No_Effect)
                return (Int32)cmd.PatchedVfx;
            else if ((cmd.aa.Info.Target == TargetType.ManyAny && cmd.info.cursor == 0) || cmd.info.meteor_miss != 0 || cmd.info.short_summon != 0 || btl_vfx.UseBeatrixAlternateVfx(regist, (SpecialEffect)cmd.aa.Info.VfxIndex, (SpecialEffect)cmd.aa.Vfx2))
                return cmd.aa.Vfx2;
            else
                return cmd.aa.Info.VfxIndex;
        }
        return -1;
    }

    public static void SelectCommandVfx(CMD_DATA cmd)
    {
        if (cmd.cmd_no == BattleCommandId.Phantom)
        {
            if (cmd.sub_no == 49)
                FF9StateSystem.Battle.FF9Battle.phantom_no = 153;
            else if (cmd.sub_no == 51)
                FF9StateSystem.Battle.FF9Battle.phantom_no = 154;
            else if (cmd.sub_no == 53)
                FF9StateSystem.Battle.FF9Battle.phantom_no = 155;
            else if (cmd.sub_no == 55)
                FF9StateSystem.Battle.FF9Battle.phantom_no = 156;
            else if (cmd.sub_no == 58)
                FF9StateSystem.Battle.FF9Battle.phantom_no = 157;
            else if (cmd.sub_no == 60)
                FF9StateSystem.Battle.FF9Battle.phantom_no = 158;
            else if (cmd.sub_no == 62)
                FF9StateSystem.Battle.FF9Battle.phantom_no = 159;
            else if (cmd.sub_no == 64)
                FF9StateSystem.Battle.FF9Battle.phantom_no = 160;
        }
        BTL_DATA regist = cmd.regist;
        if (Configuration.Battle.SFXRework)
        {
            if (cmd.aa.Info.VfxAction == null && !String.IsNullOrEmpty(cmd.aa.Info.SequenceFile))
			{
                String sequenceText = AssetManager.LoadString(cmd.aa.Info.SequenceFile, out _);
                if (sequenceText != null)
                    cmd.aa.Info.VfxAction = new UnifiedBattleSequencer.BattleAction(sequenceText);
            }
            if (cmd.aa.Info.VfxAction != null)
            {
                UnifiedBattleSequencer.BattleAction action = new UnifiedBattleSequencer.BattleAction(cmd.aa.Info.VfxAction);
                action.Execute(cmd);
                return;
            }
            if (regist != null && regist.bi.player == 0)
            {
                UnifiedBattleSequencer.BattleAction action = new UnifiedBattleSequencer.BattleAction(UnifiedBattleSequencer.EffectType.EnemySequence, cmd.sub_no);
                action.Execute(cmd);
                return;
            }
            Int32 sfxNum = GetPlayerCommandSFX(cmd);
            if (sfxNum >= 0)
            {
                UnifiedBattleSequencer.BattleAction action = new UnifiedBattleSequencer.BattleAction(UnifiedBattleSequencer.EffectType.SpecialEffect, sfxNum);
                action.Execute(cmd);
                return;
            }
            else if (cmd.cmd_no == BattleCommandId.Change || cmd.cmd_no == BattleCommandId.Defend)
            {
                String seqName = cmd.cmd_no == BattleCommandId.Change ? "SequenceChange" : "SequenceDefend";
                String[] efInfo;
                String sequenceText = AssetManager.LoadString(AssetManagerUtil.GetMemoriaAssetsPath() + "SpecialEffects/Common/" + seqName + UnifiedBattleSequencer.EXTENSION_SEQ, out efInfo);
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
                    Int32 sfxNum = GetPlayerCommandSFX(cmd);
                    if (sfxNum >= 0)
                        SetBattleVfx(cmd, (UInt32)sfxNum, null);
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
		Byte serialNo = btl_util.getSerialNumber(btl);
		if (isTrance && serialNo + 19 >= (Int32)btl_init.model_id.Length)
			return;
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
		btl_eqp.InitWeapon(FF9StateSystem.Common.FF9.player[btl.bi.slot_no], btl);
		if (serialNo == 7)
            serialNo = 8;
		AnimationFactory.AddAnimToGameObject(btl.gameObject, btl_init.model_id[serialNo], true);
	}
}
