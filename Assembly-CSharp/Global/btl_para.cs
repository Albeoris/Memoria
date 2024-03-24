using FF9;
using System;
using System.Collections.Generic;
using Memoria;
using Memoria.Data;
using NCalc;
using Object = System.Object;
using System.Text;

// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable SuspiciousTypeConversion.Global

public class btl_para
{
    public static UInt32 GetLogicalHP(BTL_DATA btl, Boolean max)
    {
        // Custom Memoria method for enemies with 10 000 HP more than they should for scripting purposes
        // It might be a good idea to rework completly the system (let some enemies be able to survive / perform ending attacks even with 0 HP for instance), but any solution requires a rewrite of AI scripts
        // Even this solution requires a rework of many AI scripts since the HP gap is not always 10 000 (Kraken's tentacle for instance)
        UInt32 hp = max ? btl.max.hp : btl.cur.hp;
        if (Configuration.Battle.CustomBattleFlagsMeaning == 1 && btl.bi.player == 0 && (btl_util.getEnemyPtr(btl).info.flags & 0x4) != 0)
        {
            if (hp > 10000)
                return hp - 10000;
            return 0;
        }
        return hp;
    }

    public static void SetLogicalHP(BTL_DATA btl, UInt32 newHP, Boolean max)
    {
        if (Configuration.Battle.CustomBattleFlagsMeaning != 0 && btl.bi.player == 0 && (btl_util.getEnemyPtr(btl).info.flags & 0x4) != 0)
        {
            if (newHP == 0)
                newHP = 1;
            else
                newHP += 10000;
        }
        else if (IsNonDyingVanillaBoss(btl))
		{
            // Weak security for enemies that should never reach 0 HP in vanilla
            newHP = Math.Max(newHP, 1);
        }
        if (max)
            btl.max.hp = newHP;
        else
            btl.cur.hp = newHP;
    }

    public static void InitATB(BTL_DATA btl)
    {
        btl.cur.at_coef = GetATBCoef();
    }

    public static SByte GetATBCoef()
    {
        SettingsState settings = (SettingsState)(Object)FF9StateSystem.Settings;
        if (settings.cfg.btl_speed == 0uL)
            return 8;
        else if (settings.cfg.btl_speed == 2uL)
            return 14;
        return 10;
    }

    public static void CheckPointData(BTL_DATA btl)
    {
        if (btl.cur.hp * 6 > btl.max.hp)
        {
            btl_stat.RemoveStatus(btl, BattleStatus.LowHP);
            if (btl.cur.hp > btl.max.hp)
                btl.cur.hp = btl.max.hp;
        }
        else
        {
            if (btl.cur.hp == 0)
            {
                if (!Status.checkCurStat(btl, BattleStatus.Death))
                    btl_stat.AlterStatus(btl, BattleStatus.Death);
                return;
            }
            if (!Status.checkCurStat(btl, BattleStatus.LowHP))
                btl_stat.AlterStatus(btl, BattleStatus.LowHP);
        }
        btl.cur.mp = btl.cur.mp <= btl.max.mp ? btl.cur.mp : btl.max.mp;
        if (btl.bi.player != 0 && !btl.is_monster_transform)
            btl.bi.def_idle = (Byte)(btl_stat.CheckStatus(btl, BattleStatusConst.IdleDying) || btl.special_status_old ? 1 : 0);
    }

    public static Int32 SetDamage(BattleUnit btl, Int32 damage, Byte dmg_mot, CMD_DATA cmd = null)
    {
        // "damage" and the different "fig" numbers are signed integers now
        // Maybe choose to have these unsigned or have everything signed (including "hp.cur" etc...) or to keep things as they are now
        // Note that "btl2d" is currently adjusted to display unsigned numbers only
        if (btl.IsUnderStatus(BattleStatus.Death))
        {
            btl.Data.fig_info = Param.FIG_INFO_MISS;
            return 0;
        }

        if (btl.IsUnderAnyStatus(BattleStatus.Petrify))
        {
            btl.Fig = 0;
            return 0;
        }

        if (!btl_util.IsBtlBusy(btl.Data, btl_util.BusyMode.CASTER))
            btl.FaceTheEnemy();

        if (!FF9StateSystem.Battle.isDebug)
        {
            if (btl.CurrentHp > damage)
            {
                if (!btl.IsPlayer || !FF9StateSystem.Settings.IsHpMpFull)
                    btl.CurrentHp -= (UInt32)damage;
            }
            else
            {
                btl.CurrentHp = 0;
            }
        }

        btl.Fig = damage;
        if (dmg_mot != 0)
            btl_mot.SetDamageMotion(btl, cmd);
        else if (btl.CurrentHp == 0)
            btl.Kill(cmd?.regist);
        if (btl.CurrentHp == 0)
            btl.Data.killer_track = cmd?.regist;
        return damage;
    }

    public static Int32 SetRecover(BattleUnit btl, UInt32 recover)
    {
        if (btl.IsUnderAnyStatus(BattleStatus.Death))
        {
            btl.Data.fig_info = Param.FIG_INFO_MISS;
            return 0;
        }
        if (btl.IsUnderAnyStatus(BattleStatus.Petrify))
        {
            btl.Data.fig = 0;
            return 0;
        }
        if (btl.CurrentHp + recover < btl.MaximumHp)
            btl.CurrentHp += recover;
        else
            btl.CurrentHp = btl.MaximumHp;
        btl.Data.fig = (Int32)recover;
        return (Int32)recover;
    }

    public static Int32 SetMpDamage(BattleUnit btl, UInt32 damage)
    {
        if (btl.IsUnderAnyStatus(BattleStatus.Death))
        {
            btl.Data.fig_info = Param.FIG_INFO_MISS;
            return 0;
        }
        else if (btl.IsUnderAnyStatus(BattleStatus.Petrify))
        {
            btl.Data.m_fig = 0;
            return 0;
        }
        else if (!FF9StateSystem.Battle.isDebug && (btl.IsPlayer || !FF9StateSystem.Settings.IsHpMpFull))
        {
            if (btl.CurrentMp > damage)
                btl.CurrentMp -= damage;
            else
                btl.CurrentMp = 0;
        }
        btl.Data.m_fig = (Int32)damage;
        return (Int32)damage;
    }

    public static Int32 SetMpRecover(BattleUnit btl, UInt32 recover)
    {
        if (btl.IsUnderAnyStatus(BattleStatus.Death))
        {
            btl.Data.fig_info = Param.FIG_INFO_MISS;
            return 0;
        }
        if (btl.IsUnderAnyStatus(BattleStatus.Petrify))
        {
            btl.Data.m_fig = 0;
            return 0;
        }
        if (btl.CurrentMp + recover < btl.MaximumMp)
            btl.CurrentMp += recover;
        else
            btl.CurrentMp = btl.MaximumMp;
        btl.Data.m_fig = (Int32)recover;
        return (Int32)recover;
    }

    public static void SetPoisonDamage(BTL_DATA btl)
    {
        BattleUnit unit = new BattleUnit(btl);
        if (!btl_stat.CheckStatus(btl, BattleStatus.Petrify))
        {
            UInt32 damage = GetLogicalHP(btl, true) >> 4;
            if (btl_stat.CheckStatus(btl, BattleStatus.EasyKill))
                damage >>= 2;
            if (!String.IsNullOrEmpty(Configuration.Battle.PoisonHPDamage))
            {
                Expression e = new Expression(Configuration.Battle.PoisonHPDamage);
                e.Parameters["IsZombie"] = unit.IsZombie;
                e.Parameters["IsEasyKill"] = ((btl.stat.cur & BattleStatus.EasyKill) != 0 || (btl.stat.permanent & BattleStatus.EasyKill) != 0);
                e.Parameters["IsPoison"] = ((btl.stat.cur & BattleStatus.Poison) != 0 || (btl.stat.permanent & BattleStatus.Poison) != 0);
                e.Parameters["IsVenom"] = ((btl.stat.cur & BattleStatus.Venom) != 0 || (btl.stat.permanent & BattleStatus.Venom) != 0);
                e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                NCalcUtility.InitializeExpressionUnit(ref e, new BattleUnit(btl), "Target");
                Int64 val = NCalcUtility.ConvertNCalcResult(e.Evaluate(), Int64.MinValue);
                if (val != Int64.MinValue)
                {
                    if (val < 0)
                    {
                        btl.fig_stat_info |= Param.FIG_STAT_INFO_REGENE_HP;
                        val = Math.Abs(val);
                    }
                    damage = (UInt32)val;
                }
            }
            if (!FF9StateSystem.Battle.isDebug)
            {
                if ((btl.fig_stat_info & Param.FIG_STAT_INFO_REGENE_HP) != 0)
                {
                    if (btl.cur.hp + damage < btl.max.hp)
                    {
                        btl.cur.hp += damage;
                    }
                    else
                    {
                        btl.cur.hp = btl.max.hp;
                    }
                    btl.fig_regene_hp = (Int32)damage;
                }
                else
                {
                    if (GetLogicalHP(btl, false) > damage)
                    {
                        if (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull)
                            btl.cur.hp -= damage;
                    }
                    else
                    {
                        new BattleUnit(btl).Kill();
                    }
                    btl.fig_stat_info |= Param.FIG_STAT_INFO_POISON_HP;
                    btl.fig_poison_hp = (Int32)damage;
                }
            }
            BattleVoice.TriggerOnStatusChange(btl, "Used", btl_stat.CheckStatus(btl, BattleStatus.Venom) ? BattleStatus.Venom : BattleStatus.Poison);
        }
    }

    public static void SetRegeneRecover(BTL_DATA btl)
    {
        UInt32 recover = 0;
        if (!btl_stat.CheckStatus(btl, BattleStatus.Petrify))
        {
            recover = GetLogicalHP(btl, true) >> 4;
            if (!String.IsNullOrEmpty(Configuration.Battle.RegenHPRecovery))
            {
                btl.fig_stat_info = Param.FIG_STAT_INFO_REGENE_HP;
                Expression e = new Expression(Configuration.Battle.RegenHPRecovery);
                e.Parameters["IsZombie"] = ((btl.stat.cur & BattleStatus.Zombie) != 0 || (btl.stat.permanent & BattleStatus.Zombie) != 0);
                e.Parameters["IsEasyKill"] = ((btl.stat.cur & BattleStatus.EasyKill) != 0 || (btl.stat.permanent & BattleStatus.EasyKill) != 0);
                e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                NCalcUtility.InitializeExpressionUnit(ref e, new BattleUnit(btl), "Target");
                Int64 val = NCalcUtility.ConvertNCalcResult(e.Evaluate(), Int64.MinValue);
                if (val != Int64.MinValue)
                {
                    if (val < 0)
                    {
                        btl.fig_stat_info |= Param.FIG_STAT_INFO_REGENE_DMG;
                        val = Math.Abs(val);
                    }
                    recover = (UInt32)val;
                }
                if (!FF9StateSystem.Battle.isDebug)
                {
                    if ((btl.fig_stat_info & Param.FIG_STAT_INFO_REGENE_DMG) != 0)
                    {
                        btl.fig_stat_info |= Param.FIG_STAT_INFO_REGENE_DMG;
                        if (GetLogicalHP(btl, false) > recover)
                            btl.cur.hp -= recover;
                        else
                            new BattleUnit(btl).Kill();
                    }
                    else
                    {
                        if (btl.cur.hp + recover < btl.max.hp)
                            btl.cur.hp += recover;
                        else
                            btl.cur.hp = btl.max.hp;
                    }
                    btl.fig_regene_hp = (Int32)recover;
                }
            }
            else
            {
                if (btl_stat.CheckStatus(btl, BattleStatus.Zombie) || btl_util.CheckEnemyCategory(btl, 16))
                {
                    btl.fig_stat_info |= Param.FIG_STAT_INFO_REGENE_DMG;
                    if (GetLogicalHP(btl, false) > recover)
                        btl.cur.hp -= recover;
                    else
                        new BattleUnit(btl).Kill();
                }
                else if (btl.cur.hp + recover < btl.max.hp)
                {
                    btl.cur.hp += recover;
                }
                else
                {
                    btl.cur.hp = btl.max.hp;
                }
                btl.fig_stat_info |= Param.FIG_STAT_INFO_REGENE_HP;
                btl.fig_regene_hp = (Int32)recover;
            }
        }
        BattleVoice.TriggerOnStatusChange(btl, "Used", BattleStatus.Regen);
    }

    public static void SetPoisonMpDamage(BTL_DATA btl)
    {
        if (Configuration.Mod.TranceSeek && btl_stat.CheckStatus(btl, BattleStatus.EasyKill)) // TRANCE SEEK - Venom didn't remove MP on bosses.
            return;
        UInt32 damage = 0;
        if (!btl_stat.CheckStatus(btl, BattleStatus.Petrify))
        {
            damage = btl.max.mp >> 4;
            if (btl_stat.CheckStatus(btl, BattleStatus.EasyKill))
                damage >>= 2;
            if (!String.IsNullOrEmpty(Configuration.Battle.PoisonMPDamage))
            {
                Expression e = new Expression(Configuration.Battle.PoisonMPDamage);
                e.Parameters["IsZombie"] = ((btl.stat.cur & BattleStatus.Zombie) != 0 || (btl.stat.permanent & BattleStatus.Zombie) != 0);
                e.Parameters["IsEasyKill"] = ((btl.stat.cur & BattleStatus.EasyKill) != 0 || (btl.stat.permanent & BattleStatus.EasyKill) != 0);
                e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                NCalcUtility.InitializeExpressionUnit(ref e, new BattleUnit(btl), "Target");
                Int64 val = NCalcUtility.ConvertNCalcResult(e.Evaluate(), Int64.MinValue);
                if (val != Int64.MinValue)
                {
                    if (val < 0)
                    {
                        btl.fig_stat_info |= Param.FIG_STAT_INFO_REGENE_HP;
                        val = Math.Abs(val);
                    }
                    damage = (UInt32)val;
                }
            }
            if (!FF9StateSystem.Battle.isDebug && (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull))
            {
                if ((btl.fig_stat_info & Param.FIG_STAT_INFO_REGENE_MP) != 0)
                {
                    if (btl.cur.mp + damage < btl.max.mp)
                    {
                        btl.cur.mp += damage;
                    }
                    else
                    {
                        btl.cur.mp = btl.max.mp;
                    }
                    btl.fig_regene_mp = (Int32)damage;
                }
                else
                {
                    if (btl.cur.mp > damage)
                        btl.cur.mp -= damage;
                    else
                        btl.cur.mp = 0;
                    btl.fig_stat_info |= Param.FIG_STAT_INFO_POISON_MP;
                    btl.fig_poison_mp = (Int32)damage;
                }
            }
        }
    }

    public static void SetTroubleDamage(BattleUnit btl, Int32 dmg)
    {
        foreach (BattleUnit next in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (next.IsPlayer == btl.IsPlayer && next.Id != btl.Id && next.IsTargetable)
            {
                next.Data.fig_info = Param.FIG_INFO_DISP_HP;
                SetDamage(next, dmg, 0);
                btl2d.Btl2dReq(next.Data);
                BattleVoice.TriggerOnStatusChange(next.Data, "Used", BattleStatus.Trouble);
            }
        }
    }

    public static void SwitchPlayerRow(BTL_DATA btl, Boolean updatePos = true)
    {
        btl.base_pos[2] += (btl.bi.row == 0) ? 400 : -400;
        btl.bi.row = (Byte)(btl.bi.row == 0 ? 1 : 0);
        if (updatePos)
            btl.pos[2] = btl.base_pos[2];
    }

    public static Boolean IsNonDyingVanillaBoss(BTL_DATA btl)
	{
        if (Configuration.Battle.CustomBattleFlagsMeaning != 0 || btl.bi.player != 0 || btl_util.getEnemyPtr(btl).info.die_vulnerable == 1)
            return false;
        if (NonDyingBossBattles.Contains(FF9StateSystem.Battle.battleMapIndex))
		{
            if (FF9StateSystem.Battle.battleMapIndex == 338 && btl.max.hp < 10000) // King Leo + Zenero + Benero
                return false;
            return true;
        }
        return false;
	}

    private static HashSet<Int32> NonDyingBossBattles = new HashSet<Int32>()
    {
        300, // Antlion
        84, // Armodullahan
        295, // Baku
        4, // Beatrix 1
        299, // Beatrix 2
        73, // Beatrix 3
        294, // Black Waltz 2
        296, // Black Waltz 3
        936, // Deathguise
        890, // Garland
        326, // Gizamaluke
        191, // Hades
        338, // King Leo + Zenero + Benero
        891, // Kuja + Kuja Trance
        937, // Kuja Trance
        83, // Lani
        336, // Masked Man + Mask (right) + Mask (left)
        938, // Necron + Dummy x 3
        931, // Nova Dragon
        211, // Ozma
        57, // Ozma
        301, // Prison Cage + Vivi
        302, // Prison Cage + Garnet
        76, // Ralvurahva
        132, // Scarlet Hair
        889, // Silver Dragon
        334, // Steiner + Bomb
        335, // Steiner + Plutos
        337, // Steiner + Blank
        525, // Valia Pira
        74, // Zorn + Thorn
        723, // Friendly Garuda
        192, 193, 196, 197, 199, // Friendly Ghost
        365, 367, 368, 595, 605, 606, // Friendly Jabberwock
        235, 239, 270, 682, 686, 687, 689, 841, // Friendly Ladybug
        252, 363, 364, 838, // Friendly Mu
        188, 189, 268, 636, 637, 641, 647, // Friendly Nymph
        216, 217, 652, 664, 668, 670, 751, // Friendly Yeti
        627, 634, 753, 755, 941, 942, 943, 944 // Ragtime Mouse + True + False
    };
}