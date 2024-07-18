using System;
using System.Collections.Generic;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Scripts;
using Assets.Sources.Scripts.UI.Common;

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
        if (Configuration.Battle.CustomBattleFlagsMeaning == 1 && btl.bi.player == 0 && (btl_util.getEnemyPtr(btl).info.flags & ENEMY.ENEMY_INFO.FLG_NON_DYING_BOSS) != 0)
        {
            // In that mode, an enemy can be flagged as unkillable with "set SV_FunctionEnemy[109] |=$ 4" and killable with "set SV_FunctionEnemy[109] &=$ 251" in its AI script
            if (hp > 10000)
                return hp - 10000;
            return 0;
        }
        return hp;
    }

    public static void SetLogicalHP(BTL_DATA btl, UInt32 newHP, Boolean max)
    {
        if (Configuration.Battle.CustomBattleFlagsMeaning == 1)
        {
            // TODO [Tirlititi] Check that "IsNonDyingVanillaBoss" isn't needed at all in AF
            if (btl.bi.player == 0 && (btl_util.getEnemyPtr(btl).info.flags & ENEMY.ENEMY_INFO.FLG_NON_DYING_BOSS) != 0)
            {
                if (newHP == 0)
                    newHP = 1;
                else
                    newHP += 10000;
            }
        }
        else if (IsNonDyingVanillaBoss(btl))
        {
            // Weak security for enemies that should never reach 0 HP in vanilla
            newHP = Math.Max(newHP, 1);
        }
        if (FF9StateSystem.Settings.IsHpMpFull && !max && btl.bi.player != 0)
            newHP = btl.max.hp;
        if (max)
            btl.max.hp = newHP;
        else
            btl.cur.hp = newHP;
    }

    public static Int16 GetMaxATB(BattleUnit unit)
    {
        return (Int16)((60 - unit.Dexterity) * 40 << 2);
    }

    public static void SetupATBCoef(BTL_DATA btl)
    {
        btl.cur.at_coef = GetATBCoef();
    }

    public static SByte GetATBCoef()
    {
        switch (FF9StateSystem.Settings.cfg.btl_speed)
        {
            case 0: return 8;
            case 1: return 10;
            case 2: return 14;
        }
        return 10;
    }

    public static void CheckPointData(BattleUnit unit)
    {
        if (unit.CurrentHp > unit.MaximumHp)
            unit.CurrentHp = unit.MaximumHp;
        if (unit.CurrentMp > unit.MaximumMp)
            unit.CurrentMp = unit.MaximumMp;
        BattleStatus checkPointStatus = CheckPointDataStatus(unit);
        if ((checkPointStatus & BattleStatus.Death) != 0)
        {
            if (!btl_stat.CheckStatus(unit, BattleStatus.Death))
                btl_stat.AlterStatus(unit, BattleStatusId.Death);
            return;
        }
        if (unit.IsNonMorphedPlayer)
            unit.Data.bi.def_idle = (Byte)(btl_stat.CheckStatus(unit, BattleStatusConst.IdleDying) ? 1 : 0);
    }

    public static BattleStatus CheckPointDataStatus(BattleUnit unit)
    {
        if (unit.Data.cur.hp == 0) // Using this instead of "CurrentHp" avoids considering bosses under 10 000 HP as dead here
            return BattleStatus.Death;
        IOverloadUnitCheckPointScript overloadedMethod = ScriptsLoader.GetOverloadedMethod(typeof(IOverloadUnitCheckPointScript)) as IOverloadUnitCheckPointScript;
        if (overloadedMethod != null)
            return overloadedMethod.UpdatePointStatus(unit);
        // Default method
        Boolean isLowHP = unit.IsPlayer && unit.CurrentHp * 6 <= unit.MaximumHp;
        if (isLowHP)
        {
            unit.UIColorHP = FF9TextTool.Yellow;
            if (!btl_stat.CheckStatus(unit, BattleStatus.LowHP))
                btl_stat.AlterStatus(unit, BattleStatusId.LowHP);
        }
        else
        {
            unit.UIColorHP = FF9TextTool.White;
            btl_stat.RemoveStatus(unit, BattleStatusId.LowHP);
        }
        unit.UIColorMP = unit.CurrentMp <= unit.MaximumMp / 6f ? FF9TextTool.Yellow : FF9TextTool.White;
        return isLowHP ? BattleStatus.LowHP : 0;
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
        // Dummied
        BattleUnit battleUnit = new BattleUnit(btl);
        UInt32 damage = 0;
        if (!btl_stat.CheckStatus(btl, BattleStatus.Petrify))
        {
            // TODO [DV]
            if (Configuration.Mod.TranceSeek)
            {
                damage = GetLogicalHP(btl, true) >> (battleUnit.IsUnderStatus(BattleStatus.EasyKill) ? 8 : 5);
                if (battleUnit.IsUnderStatus(BattleStatus.Venom))
                    damage = GetLogicalHP(btl, true) >> (battleUnit.IsUnderStatus(BattleStatus.EasyKill) ? 7 : 4);
            }
            else
            {
                damage = GetLogicalHP(btl, true) >> 4;
                if (btl_stat.CheckStatus(btl, BattleStatus.EasyKill))
                    damage >>= 2;
            }
            if (!FF9StateSystem.Battle.isDebug)
            {
                if (Configuration.Mod.TranceSeek && battleUnit.IsZombie)
                {
                    if (battleUnit.IsUnderStatus(BattleStatus.Poison)) // [DV] Zombie get healed by Poison in Trance Seek.
                    {
                        btl.cur.hp += damage;
                        btl.fig_stat_info |= Param.FIG_STAT_INFO_REGENE_HP;
                        btl.fig_regene_hp = (Int32)damage;
                        return;
                    }
                    if (battleUnit.IsUnderStatus(BattleStatus.Venom)) // [DV] Zombie get half damage by Venom in Trance Seek.
                    {
                        damage /= 2U;
                        btl.cur.hp -= damage;
                        btl.fig_stat_info |= Param.FIG_STAT_INFO_POISON_HP;
                        btl.fig_poison_hp = (Int32)damage;
                        return;
                    }
                }
                if (GetLogicalHP(btl, false) > damage)
                {
                    if (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull)
                        btl.cur.hp -= damage;
                }
                else
                {
                    new BattleUnit(btl).Kill();
                }
            }
        }
        btl.fig_stat_info |= Param.FIG_STAT_INFO_POISON_HP;
        btl.fig_poison_hp = (Int32)damage;
        BattleVoice.TriggerOnStatusChange(btl, "Used", btl_stat.CheckStatus(btl, BattleStatus.Venom) ? BattleStatusId.Venom : BattleStatusId.Poison);
    }

    public static void SetRegeneRecover(BTL_DATA btl)
    {
        // Dummied
        UInt32 recover = 0;
        if (!btl_stat.CheckStatus(btl, BattleStatus.Petrify))
        {
            recover = GetLogicalHP(btl, true) >> (Configuration.Mod.TranceSeek ? (btl_stat.CheckStatus(btl, BattleStatus.EasyKill) ? 7 : 5) : 4);
            if (new BattleUnit(btl).IsZombie)
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
        }
        btl.fig_stat_info |= Param.FIG_STAT_INFO_REGENE_HP;
        btl.fig_regene_hp = (Int32)recover;
        BattleVoice.TriggerOnStatusChange(btl, "Used", BattleStatusId.Regen);
    }

    public static void SetPoisonMpDamage(BTL_DATA btl)
    {
        // Dummied
        // TODO [DV]
        if (Configuration.Mod.TranceSeek && btl_stat.CheckStatus(btl, BattleStatus.EasyKill)) // TRANCE SEEK - Venom didn't remove MP on bosses.
            return;
        UInt32 damage = 0;
        if (!btl_stat.CheckStatus(btl, BattleStatus.Petrify))
        {
            damage = btl.max.mp >> (Configuration.Mod.TranceSeek ? 5 : 4);
            if (btl_stat.CheckStatus(btl, BattleStatus.EasyKill))
                damage >>= 2;
            if (Configuration.Mod.TranceSeek && btl_stat.CheckStatus(btl, BattleStatus.Venom))
                damage = btl.max.mp >> 4;
            if (!FF9StateSystem.Battle.isDebug && (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull))
            {
                if (btl.cur.mp > damage)
                    btl.cur.mp -= damage;
                else
                    btl.cur.mp = 0;
            }
        }
        btl.fig_stat_info |= Param.FIG_STAT_INFO_POISON_MP;
        btl.fig_poison_mp = (Int32)damage;
    }

    public static void SetTroubleDamage(BattleUnit btl, Int32 dmg)
    {
        // Dummied
        foreach (BattleUnit next in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (next.IsPlayer == btl.IsPlayer && next.Id != btl.Id && next.IsTargetable)
            {
                next.Data.fig_info = Param.FIG_INFO_DISP_HP;
                SetDamage(next, dmg, 0);
                btl2d.Btl2dReq(next.Data);
                BattleVoice.TriggerOnStatusChange(next.Data, "Used", BattleStatusId.Trouble);
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
