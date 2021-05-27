using FF9;
using System;
using Memoria;
using Memoria.Data;
using UnityEngine;
using Object = System.Object;

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
        if (Configuration.Battle.CustomBattleFlagsMeaning == 1 && btl.bi.player == 0 && (btl_util.getEnemyPtr(btl).info.flags & 0x4) != 0)
        {
            if (newHP == 0)
                newHP = 1;
            else
                newHP += 10000;
        }
        if (max)
            btl.max.hp = newHP;
        else
            btl.cur.hp = newHP;
    }

    public static void InitATB(BTL_DATA btl)
    {
        SettingsState settings = (SettingsState)(Object)FF9StateSystem.Settings;
        btl.cur.at_coef = 10;
        if (settings.cfg.btl_speed == 0uL)
        {
            btl.cur.at_coef = (SByte)(btl.cur.at_coef - 2);
        }
        else if (settings.cfg.btl_speed == 2uL)
        {
            btl.cur.at_coef = (SByte)(btl.cur.at_coef + 4);
        }
    }

    public static void CheckPointData(BTL_DATA btl)
    {
        if (btl.cur.hp * 6 > btl.max.hp)
        {
            btl_stat.RemoveStatus(btl, BattleStatus.LowHP);
            if (btl.cur.hp > btl.max.hp)
            {
                btl.cur.hp = btl.max.hp;
            }
        }
        else
        {
            if (btl.cur.hp == 0)
            {
                if (btl.die_seq == 0)
                {
                    btl_stat.AlterStatus(btl, BattleStatus.Death);
                }
                return;
            }
            if (!Status.checkCurStat(btl, BattleStatus.LowHP))
            {
                btl_stat.AlterStatus(btl, BattleStatus.LowHP);
            }
        }
        btl.cur.mp = ((btl.cur.mp <= btl.max.mp) ? btl.cur.mp : btl.max.mp);
        if (btl.bi.player != 0)
        {
            btl.bi.def_idle = (Byte)((!btl_stat.CheckStatus(btl, BattleStatus.IdleDying)) ? 0 : 1);
        }
    }

    public static void SetDamage(BattleUnit btl, Int32 damage, Byte dmg_mot)
    {
        // "damage" and the different "fig" numbers are signed integers now
        // Maybe choose to have these unsigned or have everything signed (including "hp.cur" etc...) or to keep things as they are now
        // Note that "btl2d" is currently adjusted to display unsigned numbers only
        if (btl.IsUnderStatus(BattleStatus.Death))
        {
            btl.Data.fig_info = 32;
            return;
        }

        if (btl.IsUnderAnyStatus(BattleStatus.Petrify))
        {
            btl.Fig = 0;
            return;
        }

        if (FF9StateSystem.Battle.FF9Battle.cur_cmd != null && btl.Data != FF9StateSystem.Battle.FF9Battle.cur_cmd.regist)
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
        {
            btl_mot.SetDamageMotion(btl);
        }
        else if (btl.CurrentHp == 0)
        {
            btl.Kill();
        }
    }

    public static void SetRecover(BTL_DATA btl, UInt32 recover)
    {
        if (Status.checkCurStat(btl, BattleStatus.Death))
        {
            btl.fig_info = 32;
        }
        else if (btl_stat.CheckStatus(btl, BattleStatus.Petrify))
        {
            recover = 0;
        }
        else if (btl.cur.hp + recover < btl.max.hp)
        {
            btl.cur.hp += recover;
        }
        else
        {
            btl.cur.hp = btl.max.hp;
        }
        btl.fig = (Int32)recover;
    }

    public static void SetMpDamage(BTL_DATA btl, UInt32 damage)
    {
        if (Status.checkCurStat(btl, BattleStatus.Death))
        {
            btl.fig_info = 32;
        }
        else if (btl_stat.CheckStatus(btl, BattleStatus.Petrify))
        {
            damage = 0;
        }
        else if (!FF9StateSystem.Battle.isDebug && (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull))
        {
            if (btl.cur.mp > damage)
            {
                btl.cur.mp -= damage;
            }
            else
            {
                btl.cur.mp = 0;
            }
        }
        btl.m_fig = (Int32)damage;
    }

    public static void SetMpRecover(BTL_DATA btl, UInt32 recover)
    {
        if (Status.checkCurStat(btl, BattleStatus.Death))
        {
            btl.fig_info = 32;
        }
        else if (btl_stat.CheckStatus(btl, BattleStatus.Petrify))
        {
            recover = 0;
        }
        else if (btl.cur.mp + recover < btl.max.mp)
        {
            btl.cur.mp += recover;
        }
        else
        {
            btl.cur.mp = btl.max.mp;
        }
        btl.m_fig = (Int32)recover;
    }

    public static void SetPoisonDamage(BTL_DATA btl)
    {
        UInt32 num = 0;
        if (!btl_stat.CheckStatus(btl, BattleStatus.Petrify))
        {
            num = GetLogicalHP(btl, true) >> 4;
            if (Status.checkCurStat(btl, BattleStatus.EasyKill))
                num >>= 2;
            if (!FF9StateSystem.Battle.isDebug)
            {
                if (GetLogicalHP(btl, false) > num)
                {
                    if (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull)
                    {
                        btl.cur.hp -= num;
                    }
                }
                else
                {
                    new BattleUnit(btl).Kill();
                }
            }
        }
        btl.fig_stat_info |= 2;
        btl.fig_poison_hp = (Int32)num;
    }

    public static void SetRegeneRecover(BTL_DATA btl)
    {
        UInt32 num = 0;
        if (!btl_stat.CheckStatus(btl, BattleStatus.Petrify))
        {
            num = GetLogicalHP(btl, true) >> 4;
            if (btl_stat.CheckStatus(btl, BattleStatus.Zombie) || btl_util.CheckEnemyCategory(btl, 16))
            {
                btl.fig_stat_info |= 8;
                if (GetLogicalHP(btl, false) > num)
                {
                    btl.cur.hp -= num;
                }
                else
                {
                    new BattleUnit(btl).Kill();
                }
            }
            else if (btl.cur.hp + num < btl.max.hp)
            {
                btl.cur.hp += num;
            }
            else
            {
                btl.cur.hp = btl.max.hp;
            }
        }
        btl.fig_stat_info |= 1;
        btl.fig_regene_hp = (Int32)num;
    }

    public static void SetPoisonMpDamage(BTL_DATA btl)
    {
        UInt32 num = 0;
        if (!btl_stat.CheckStatus(btl, BattleStatus.Petrify))
        {
            num = btl.max.mp >> 4;
            if (Status.checkCurStat(btl, BattleStatus.EasyKill))
                num >>= 2;
            if (!FF9StateSystem.Battle.isDebug && (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull))
            {
                if (btl.cur.mp > num)
                {
                    btl.cur.mp -= num;
                }
                else
                {
                    btl.cur.mp = 0;
                }
            }
        }
        btl.fig_stat_info |= 4;
        btl.fig_poison_mp = (Int32)num;
    }

    public static void SetTroubleDamage(BattleUnit btl)
    {
        foreach (BattleUnit next in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (next.IsPlayer == btl.IsPlayer && next.Id != btl.Id && next.IsSelected)
            {
                next.Data.fig_info = 1;
                SetDamage(next, btl.Fig >> 1, 0);
                btl2d.Btl2dReq(next.Data);
            }
        }
    }

    public static void SwitchPlayerRow(BTL_DATA btl)
    {
        Int32 delta = (btl.bi.row == 0) ? 400 : -400;
        btl.pos[2] = btl.base_pos[2] + delta;
        btl.base_pos[2] += delta;
        btl.bi.row = (Byte)(btl.bi.row == 0 ? 1 : 0);
    }
}