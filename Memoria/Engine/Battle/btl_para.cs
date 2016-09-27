using FF9;
using System;
using Memoria;
using UnityEngine;
using Object = System.Object;

// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable SuspiciousTypeConversion.Global

[ExportedType("$ĭø©ńńńńńńńń.!!!ĄÀļěíxą=ĞóBÜĥăĎĹ­^ġ.ħÃÜòĴ!Ãbľ¾ª$ČÆZ³)ºĀĿĮÇÚÞ¹ù9zńńńń")]
public class btl_para
{
    public static void InitATB(BTL_DATA btl)
    {
        SettingsState settings = (SettingsState)(Object)FF9StateSystem.Settings;
        btl.cur.at_coef = 10;
        if (settings.cfg.btl_speed == 0uL)
        {
            POINTS expr_29 = btl.cur;
            expr_29.at_coef = (SByte)(expr_29.at_coef - 2);
        }
        else if (settings.cfg.btl_speed == 2uL)
        {
            POINTS expr_55 = btl.cur;
            expr_55.at_coef = (SByte)(expr_55.at_coef + 4);
        }
    }

    public static void CheckPointData(BTL_DATA btl)
    {
        if (btl.cur.hp * 6 > btl.max.hp)
        {
            btl_stat.RemoveStatus(btl, 512u);
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
                    btl_stat.AlterStatus(btl, 256u);
                }
                return;
            }
            if (!Status.checkCurStat(btl, 512u))
            {
                btl_stat.AlterStatus(btl, 512u);
            }
        }
        btl.cur.mp = ((btl.cur.mp <= btl.max.mp) ? btl.cur.mp : btl.max.mp);
        if (btl.bi.player != 0)
        {
            btl.bi.def_idle = (Byte)((!btl_stat.CheckStatus(btl, 197122u)) ? 0 : 1);
        }
    }

    public static void SetDamage(BTL_DATA btl, Int32 damage, Byte dmg_mot)
    {
        if (Status.checkCurStat(btl, 256u))
        {
            btl.fig_info = 32;
            return;
        }
        if (Status.checkCurStat(btl, 1u))
        {
            btl.fig = 0;
            return;
        }

        if (btl != FF9StateSystem.Battle.FF9Battle.cur_cmd.regist)
        {
            Int32 num = btl_mot.setDirection(btl);
            btl.evt.rotBattle.eulerAngles = new Vector3(btl.evt.rotBattle.eulerAngles.x, num, btl.evt.rotBattle.eulerAngles.z);
            btl.rot.eulerAngles = new Vector3(btl.rot.eulerAngles.x, num, btl.rot.eulerAngles.z);
        }
        if (!FF9StateSystem.Battle.isDebug)
        {
            if (btl.cur.hp > damage)
            {
                if (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull)
                {
                    POINTS expr_116 = btl.cur;
                    expr_116.hp -= (UInt16)damage;
                }
            }
            else
            {
                btl.cur.hp = 0;
            }
        }
        btl.fig = (Int16)damage;
        if (dmg_mot != 0)
        {
            btl_mot.SetDamageMotion(btl);
        }
        else if (btl.cur.hp == 0)
        {
            new BattleUnit(btl).Kill();
        }
    }

    public static void SetRecover(BTL_DATA btl, Int32 recover)
    {
        if (Status.checkCurStat(btl, 256u))
        {
            btl.fig_info = 32;
        }
        else if (Status.checkCurStat(btl, 1u))
        {
            recover = 0;
        }
        else if (btl.cur.hp + recover < btl.max.hp)
        {
            POINTS expr_54 = btl.cur;
            expr_54.hp += (UInt16)recover;
        }
        else
        {
            btl.cur.hp = btl.max.hp;
        }
        btl.fig = (Int16)recover;
    }

    public static void SetMpDamage(BTL_DATA btl, Int32 damage)
    {
        if (Status.checkCurStat(btl, 256u))
        {
            btl.fig_info = 32;
        }
        else if (Status.checkCurStat(btl, 1u))
        {
            damage = 0;
        }
        else if (!FF9StateSystem.Battle.isDebug && (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull))
        {
            if (btl.cur.mp > damage)
            {
                POINTS expr_76 = btl.cur;
                expr_76.mp -= (Int16)damage;
            }
            else
            {
                btl.cur.mp = 0;
            }
        }
        btl.m_fig = (Int16)damage;
    }

    public static void SetMpRecover(BTL_DATA btl, Int32 recover)
    {
        if (Status.checkCurStat(btl, 256u))
        {
            btl.fig_info = 32;
        }
        else if (Status.checkCurStat(btl, 1u))
        {
            recover = 0;
        }
        else if (btl.cur.mp + recover < btl.max.mp)
        {
            POINTS expr_54 = btl.cur;
            expr_54.mp += (Int16)recover;
        }
        else
        {
            btl.cur.mp = btl.max.mp;
        }
        btl.m_fig = (Int16)recover;
    }

    public static void SetPoisonDamage(BTL_DATA btl)
    {
        Int32 num = 0;
        if (!Status.checkCurStat(btl, 1u))
        {
            num = btl.max.hp >> 4;
            if (!FF9StateSystem.Battle.isDebug)
            {
                if (btl.cur.hp > num)
                {
                    if (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull)
                    {
                        POINTS expr_61 = btl.cur;
                        expr_61.hp -= (UInt16)num;
                    }
                }
                else
                {
                    new BattleUnit(btl).Kill();
                }
            }
        }
        btl.fig_stat_info |= 2;
        btl.fig_poison_hp = (Int16)num;
    }

    public static void SetRegeneRecover(BTL_DATA btl)
    {
        Int32 num = 0;
        if (!Status.checkCurStat(btl, 1u))
        {
            num = btl.max.hp >> 4;
            if (Status.checkCurStat(btl, 64u) || btl_util.CheckEnemyCategory(btl, 16))
            {
                btl.fig_stat_info |= 8;
                if (btl.cur.hp > num)
                {
                    POINTS expr_5C = btl.cur;
                    expr_5C.hp -= (UInt16)num;
                }
                else
                {
                    new BattleUnit(btl).Kill();
                }
            }
            else if (btl.cur.hp + num < btl.max.hp)
            {
                POINTS expr_9E = btl.cur;
                expr_9E.hp += (UInt16)num;
            }
            else
            {
                btl.cur.hp = btl.max.hp;
            }
        }
        btl.fig_stat_info |= 1;
        btl.fig_regene_hp = (Int16)num;
    }

    public static void SetPoisonMpDamage(BTL_DATA btl)
    {
        Int32 num = 0;
        if (!Status.checkCurStat(btl, 1u))
        {
            num = btl.max.mp >> 4;
            if (!FF9StateSystem.Battle.isDebug && (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull))
            {
                if (btl.cur.mp > num)
                {
                    POINTS expr_61 = btl.cur;
                    expr_61.mp -= (Int16)num;
                }
                else
                {
                    btl.cur.mp = 0;
                }
            }
        }
        btl.fig_stat_info |= 4;
        btl.fig_poison_mp = (Int16)num;
    }

    public static void SetTroubleDamage(BTL_DATA btl)
    {
        for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
        {
            if (next.bi.player == btl.bi.player && next != btl && next.bi.target != 0)
            {
                next.fig_info = 1;
                SetDamage(next, btl.fig >> 1, 0);
                btl2d.Btl2dReq(next);
            }
        }
    }

    public static void SwitchPlayerRow(BTL_DATA btl)
    {
        Int32 num = (btl.bi.row == 0) ? 400 : -400;
        btl.pos[2] = btl.base_pos[2] + num;
        Int32 index;
        Int32 expr_44 = index = 2;
        Single num2 = btl.base_pos[index];
        btl.base_pos[expr_44] = num2 + num;
        btl.bi.row = (Byte)((btl.bi.row == 0) ? 1 : 0);
    }
}