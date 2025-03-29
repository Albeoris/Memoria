using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using NCalc;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Btl2dParam
{
    public UInt16 info = 0;
    public Int32 hp = 0;
    public Int32 mp = 0;
    public HashSet<IFigurePointStatusScript> modifiers = new HashSet<IFigurePointStatusScript>();
}

public static class btl2d
{
    public static void Btl2dInit()
    {
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        ff9Battle.btl2d_work_set.Timer = 0;
        ff9Battle.btl2d_work_set.OldDisappear = Byte.MaxValue;
        foreach (BTL2D_ENT entry in ff9Battle.btl2d_work_set.Entry)
            entry.BtlPtr = null;
        List<HUDMessageChild> nonClearedStatusMessages = new List<HUDMessageChild>(btl2d.StatusMessages);
        btl2d.StatusMessages.Clear();
        foreach (HUDMessageChild message in nonClearedStatusMessages)
            Singleton<HUDMessage>.Instance.ReleaseObject(message);
    }

    public static void Btl2dReqInstant(BTL_DATA btl, UInt16 fig_info, Int32 fig, Int32 m_fig)
    {
        Btl2dParam param = new Btl2dParam()
        {
            info = fig_info,
            hp = fig,
            mp = m_fig
        };
        foreach (BattleStatusId statusId in btl.stat.cur.ToStatusList())
            if (btl.stat.effects.TryGetValue(statusId, out StatusScriptBase effect) && effect is IFigurePointStatusScript)
                param.modifiers.Add(effect as IFigurePointStatusScript);
        Btl2dReq(btl, param);
    }

    public static void Btl2dReq(BattleUnit unit)
    {
        Btl2dReq(unit.Data, unit.Data.fig);
    }

    public static void Btl2dReq(BTL_DATA btl)
    {
        Btl2dReq(btl, btl.fig);
    }

    public static void Btl2dReq(BTL_DATA btl, Btl2dParam param)
    {
        Byte delay = 0;
        if (btl.bi.disappear == 0)
        {
            foreach (IFigurePointStatusScript modifier in param.modifiers)
                modifier.OnFigurePoint(ref param.info, ref param.hp, ref param.mp);
            if ((param.info & Param.FIG_INFO_GUARD) != 0)
            {
                btl2d.Btl2dReqSymbol(btl, 2, btl2d.DMG_COL_WHITE, 0);
            }
            else if ((param.info & (Param.FIG_INFO_MISS | Param.FIG_INFO_DEATH)) != 0)
            {
                if ((param.info & Param.FIG_INFO_MISS) != 0)
                {
                    btl2d.Btl2dReqSymbol(btl, 0, btl2d.DMG_COL_WHITE, 0);
                    delay = 2;
                }
                if ((param.info & Param.FIG_INFO_DEATH) != 0)
                    btl2d.Btl2dReqSymbol(btl, 1, btl2d.DMG_COL_WHITE, delay);
            }
            else
            {
                if ((param.info & Param.FIG_INFO_DISP_HP) != 0)
                {
                    if ((param.info & Param.FIG_INFO_HP_CRITICAL) != 0)
                    {
                        btl2d.Btl2dReqSymbol(btl, 3, btl2d.DMG_COL_YELLOW, 0);
                        delay = 2;
                    }
                    if ((param.info & Param.FIG_INFO_HP_RECOVER) != 0)
                        btl2d.Btl2dReqHP(btl, param.hp, btl2d.DMG_COL_GREEN, delay);
                    else
                        btl2d.Btl2dReqHP(btl, param.hp, btl2d.DMG_COL_WHITE, delay);
                    delay += 4;
                }
                if ((param.info & Param.FIG_INFO_DISP_MP) != 0)
                {
                    if ((param.info & Param.FIG_INFO_MP_RECOVER) != 0)
                        btl2d.Btl2dReqMP(btl, param.mp, btl2d.DMG_COL_GREEN, delay);
                    else
                        btl2d.Btl2dReqMP(btl, param.mp, btl2d.DMG_COL_WHITE, delay);
                }
            }
        }
        param.info = 0;
        param.hp = 0;
        param.mp = 0;
    }

    public static void Btl2dStatReq(BTL_DATA btl, Int32 hp, Int32 mp)
    {
        if (btl.bi.disappear != 0)
            return;
        Byte delay = 0;
        if (hp != 0)
        {
            UInt16 pCol = btl2d.DMG_COL_WHITE;
            if (hp < 0)
            {
                hp = -hp;
                pCol = btl2d.DMG_COL_GREEN;
            }
            BTL2D_ENT entry = btl2d.Btl2dReqHP(btl, hp, pCol, delay);
            entry.Yofs = -12;
            delay += 4;
        }
        if (mp != 0)
        {
            UInt16 pCol = btl2d.DMG_COL_WHITE;
            if (mp < 0)
            {
                mp = -mp;
                pCol = btl2d.DMG_COL_GREEN;
            }
            BTL2D_ENT entry = btl2d.Btl2dReqMP(btl, mp, pCol, delay);
            entry.Yofs = -12;
        }
    }

    public static BTL2D_ENT GetFreeEntry(BTL_DATA pBtl)
    {
        BTL2D_ENT freeEntry = FF9StateSystem.Battle.FF9Battle.btl2d_work_set.Entry.Find(entry => entry.BtlPtr == null);
        if (freeEntry == null)
        {
            freeEntry = new BTL2D_ENT();
            FF9StateSystem.Battle.FF9Battle.btl2d_work_set.Entry.Add(freeEntry);
        }
        freeEntry.BtlPtr = pBtl;
        freeEntry.Delay = 0;
        freeEntry.trans = pBtl.gameObject.transform.GetChildByName($"bone{pBtl.tar_bone:D3}");
        freeEntry.Yofs += 4;
        return freeEntry;
    }

    public static BTL2D_ENT Btl2dReqHP(BTL_DATA pBtl, Int32 pNum, UInt16 pCol, Byte pDelay)
    {
        BTL2D_ENT freeEntry = btl2d.GetFreeEntry(pBtl);
        freeEntry.Type = 0;
        freeEntry.Delay = pDelay;
        freeEntry.NumColor = pCol;
        freeEntry.NumValue = (UInt32)pNum;
        return freeEntry;
    }

    public static BTL2D_ENT Btl2dReqMP(BTL_DATA pBtl, Int32 pNum, UInt16 pCol, Byte pDelay)
    {
        BTL2D_ENT freeEntry = btl2d.GetFreeEntry(pBtl);
        freeEntry.Type = 1;
        freeEntry.Delay = pDelay;
        freeEntry.NumColor = pCol;
        freeEntry.NumValue = (UInt32)pNum;
        return freeEntry;
    }

    public static BTL2D_ENT Btl2dReqSymbol(BTL_DATA pBtl, UInt32 pNum, UInt16 pCol, Byte pDelay)
    {
        BTL2D_ENT freeEntry = btl2d.GetFreeEntry(pBtl);
        freeEntry.Type = 2;
        freeEntry.Delay = pDelay;
        freeEntry.NumColor = pCol;
        freeEntry.NumValue = pNum;
        return freeEntry;
    }

    public static BTL2D_ENT Btl2dReqSymbolMessage(BTL_DATA pBtl, String messageColor, Dictionary<String, String> multiLangMessage, HUDMessage.MessageStyle style, Byte pDelay)
    {
        if (!multiLangMessage.TryGetValue(Localization.CurrentDisplaySymbol, out String msg))
            multiLangMessage.TryGetValue(Localization.GetFallbackSymbol(), out msg);
        return Btl2dReqSymbolMessage(pBtl, messageColor, msg, style, pDelay);
    }

    public static BTL2D_ENT Btl2dReqSymbolMessage(BTL_DATA pBtl, String messageColor, String message, HUDMessage.MessageStyle style, Byte pDelay)
    {
        BTL2D_ENT freeEntry = btl2d.GetFreeEntry(pBtl);
        freeEntry.Type = 3;
        freeEntry.Delay = pDelay;
        freeEntry.CustomColor = messageColor;
        freeEntry.CustomMessage = message;
        freeEntry.CustomStyle = style;
        return freeEntry;
    }

    public static void Btl2dMain()
    {
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        BTL2D_WORK workSet = ff9Battle.btl2d_work_set;
        foreach (BTL2D_ENT btl2dMessage in workSet.Entry)
        {
            if (btl2dMessage.Type > 3)
                btl2dMessage.BtlPtr = null;
            if (btl2dMessage.BtlPtr == null)
                continue;
            if (btl2dMessage.Delay != 0)
            {
                btl2dMessage.Delay--;
                continue;
            }
            HUDMessage.MessageStyle style = HUDMessage.MessageStyle.DAMAGE;
            String message = String.Empty;
            String format = String.Empty;
            if (btl2dMessage.Type == 0)
            {
                if (btl2dMessage.NumColor == btl2d.DMG_COL_WHITE)
                {
                    style = HUDMessage.MessageStyle.DAMAGE;
                    if (!String.IsNullOrEmpty(Configuration.Interface.BattleDamageTextFormat))
                        format = Configuration.Interface.BattleDamageTextFormat;
                }
                else
                {
                    style = HUDMessage.MessageStyle.RESTORE_HP;
                    if (!String.IsNullOrEmpty(Configuration.Interface.BattleRestoreTextFormat))
                        format = Configuration.Interface.BattleRestoreTextFormat;
                }
                message = btl2dMessage.NumValue.ToString();
            }
            else if (btl2dMessage.Type == 1)
            {
                if (btl2dMessage.NumColor == btl2d.DMG_COL_WHITE)
                {
                    style = HUDMessage.MessageStyle.DAMAGE;
                    if (!String.IsNullOrEmpty(Configuration.Interface.BattleMPDamageTextFormat))
                        format = Configuration.Interface.BattleMPDamageTextFormat;
                }
                else
                {
                    style = HUDMessage.MessageStyle.RESTORE_MP;
                    if (!String.IsNullOrEmpty(Configuration.Interface.BattleMPRestoreTextFormat))
                        format = Configuration.Interface.BattleMPRestoreTextFormat;
                }
                message = btl2dMessage.NumValue.ToString() + " " + Localization.Get("MPCaption");
            }
            else if (btl2dMessage.Type == 2)
            {
                if (btl2dMessage.NumValue == 0u)
                {
                    message = Localization.Get("Miss");
                    style = HUDMessage.MessageStyle.MISS;
                }
                else if (btl2dMessage.NumValue == 1u)
                {
                    message = Localization.Get("Death");
                    style = HUDMessage.MessageStyle.DEATH;
                }
                else if (btl2dMessage.NumValue == 2u)
                {
                    message = Localization.Get("Guard");
                    style = HUDMessage.MessageStyle.GUARD;
                }
                else if (btl2dMessage.NumValue == 3u)
                {
                    message = NGUIText.FF9YellowColor + Localization.Get("Critical") + "[-] \n ";
                    style = HUDMessage.MessageStyle.CRITICAL;
                }
                else if (btl2dMessage.NumValue == 0x10000u)
                {
                    message = NGUIText.FF9PinkColor + "DLL Error!\nCheck Memoria.log";
                    style = HUDMessage.MessageStyle.DAMAGE;
                }
            }
            else if (btl2dMessage.Type == 3)
            {
                message = btl2dMessage.CustomColor + btl2dMessage.CustomMessage;
                style = btl2dMessage.CustomStyle;
            }
            if (!String.IsNullOrEmpty(format))
            {
                try
                {
                    Expression expr = new Expression(format);
                    NCalcUtility.InitializeExpressionUnit(ref expr, new BattleUnit(btl2dMessage.BtlPtr), "Target");
                    expr.Parameters["DamageValue"] = btl2dMessage.NumValue;
                    expr.Parameters["HealValue"] = btl2dMessage.NumValue;
                    expr.Parameters["BaseText"] = message;
                    expr.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                    expr.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                    message = NCalcUtility.EvaluateNCalcString(expr.Evaluate(), message);
                }
                catch (Exception err)
                {
                    Log.Error(err);
                }
            }
            Singleton<HUDMessage>.Instance.Show(btl2dMessage.trans, message, style, new Vector3(0f, btl2dMessage.Yofs, 0f), 0);
            UIManager.Battle.DisplayParty();
            btl2dMessage.BtlPtr = null;
        }
        btl2d.ShouldShowSPS = SFX.GetEffectJTexUsed() == 0;
        btl2d.StatusUpdateVisuals(0f);
        workSet.Timer++;
        Byte oldDisappear = Byte.MaxValue;
        for (BTL_DATA btl = ff9Battle.btl_list.next; btl != null; btl = btl.next)
            if (btl.bi.disappear == 0)
                oldDisappear &= (Byte)~btl.btl_id;
        workSet.OldDisappear = oldDisappear;
    }

    public static void StatusUpdateVisuals(Single frameFrac)
    {
        BTL2D_WORK btl2d_work_set = FF9StateSystem.Battle.FF9Battle.btl2d_work_set;
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.bi.disappear == 0 && (btl.flags & geo.GEO_FLAGS_CLIP) == 0 && (btl2d_work_set.OldDisappear & btl.btl_id) == 0)
            {
                BattleUnit unit = new BattleUnit(btl);
                foreach (BattleStatusId statusId in btl.stat.cur.ToStatusList())
                {
                    if (btl2d.ShouldShowSPS && !unit.IsUnderAnyStatus(BattleStatus.Death) && (!unit.IsPlayer || !btl_mot.checkMotion(unit, BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE)))
                    {
                        BattleStatusDataEntry statusData = statusId.GetStatData();
                        if (statusData.SPSEffect >= 0 || statusData.SHPEffect >= 0)
                        {
                            Vector3 spsPos = default;
                            Vector3 shpPos = default;
                            if (statusData.SPSEffect >= 0)
                            {
                                btl2d.GetIconPosition(unit, statusData.SPSAttach, out Transform attachment, out Vector3 iconOff);
                                spsPos = attachment.position + iconOff;
                            }
                            if (statusData.SHPEffect >= 0)
                            {
                                if (statusData.SPSEffect >= 0 && statusData.SPSAttach == statusData.SHPAttach)
                                {
                                    shpPos = spsPos;
                                }
                                else
                                {
                                    btl2d.GetIconPosition(unit, statusData.SHPAttach, out Transform attachment, out Vector3 iconOff);
                                    shpPos = attachment.position + iconOff;
                                }
                            }
                            HonoluluBattleMain.battleSPS.UpdateBtlStatus(unit, statusId, spsPos, shpPos, btl2d_work_set.Timer);
                        }
                    }
                }
            }
        }
    }

    public static void ShowMessages(Boolean show = true)
    {
        foreach (HUDMessageChild message in btl2d.StatusMessages)
        {
            message.gameObject.SetActive(show);
            if (show && message.Follower.targetBtl != null && message.Follower.iconPosition >= 0)
            {
                // Check if the follow is still valid or needs update
                btl2d.GetIconPosition(message.Follower.targetBtl, message.Follower.iconPosition, out Transform attach, out Vector3 off);
                if (attach != message.Follower.target)
                {
                    message.Follower.target = attach;
                    message.Follower.targetTransformOffset = off;
                }
            }
        }
    }

    public static void GetIconPosition(BTL_DATA btl, out Byte[] iconBone, out SByte[] iconOffY, out SByte[] iconOffZ)
    {
        if (btl.bi.player != 0)
        {
            if (btl.is_monster_transform)
            {
                iconBone = btl.monster_transform.icon_bone;
                iconOffY = btl.monster_transform.icon_y;
                iconOffZ = btl.monster_transform.icon_z;
            }
            else
            {
                CharacterBattleParameter param = btl_mot.BattleParameterList[FF9StateSystem.Common.FF9.player[(CharacterId)btl.bi.slot_no].info.serial_no];
                if (param.TranceParameters && btl_stat.CheckStatus(btl, BattleStatus.Trance))
                {
                    iconBone = param.TranceStatusBone;
                    iconOffY = param.TranceStatusOffsetY;
                    iconOffZ = param.TranceStatusOffsetZ;
                }
                else
                {
                    iconBone = param.StatusBone;
                    iconOffY = param.StatusOffsetY;
                    iconOffZ = param.StatusOffsetZ;
                }
            }
        }
        else
        {
            ENEMY_TYPE et = FF9StateSystem.Battle.FF9Battle.enemy[btl.bi.slot_no].et;
            iconBone = et.icon_bone;
            iconOffY = et.icon_y;
            iconOffZ = et.icon_z;
        }
    }

    public static void GetIconPosition(BTL_DATA btl, Int32 index, out Transform attach, out Vector3 offset)
    {
        if (index == ICON_POS_ROOT)
        {
            attach = btl.gameObject.transform.GetChildByName($"bone{0:D3}");
            offset = Vector3.zero;
            return;
        }
        if (index == ICON_POS_WEAPON)
        {
            attach = btl.gameObject.transform.GetChildByName($"bone{btl.weapon_bone:D3}");
            offset = Vector3.zero;
            return;
        }
        if (index == ICON_POS_TARGET)
        {
            attach = btl.gameObject.transform.GetChildByName($"bone{btl.tar_bone:D3}");
            offset = Vector3.zero;
            return;
        }
        Single angleY = btl.rot.eulerAngles.y * 0.0174532924f;
        Single angledx = Mathf.Sin(angleY);
        Single angledz = Mathf.Cos(angleY);
        GetIconPosition(btl, out Byte[] iconBone, out SByte[] iconOffY, out SByte[] iconOffZ);
        Int16 dy = (Int16)(iconOffY[index] << 4);
        Int16 dz = (Int16)(iconOffZ[index] << 4);
        if ((btl.flags & geo.GEO_FLAGS_SCALE) != 0)
        {
            dy = (Int16)(dy * btl.gameObject.transform.localScale.y);
            dz = (Int16)(dz * btl.gameObject.transform.localScale.z);
        }
        attach = btl.gameObject.transform.GetChildByName($"bone{iconBone[index]:D3}");
        offset = new Vector3(dz * angledx, -dy, dz * angledz);
    }

    public const Byte BTL2D_INITIAL_COUNT = 16;

    public const Byte BTL2D_TYPE_HP = 0;
    public const Byte BTL2D_TYPE_MP = 1;
    public const Byte BTL2D_TYPE_SYM = 2;
    public const Byte BTL2D_TYPE_MAX = 2;

    public const Byte DMG_COL_WHITE = 0;
    public const Byte DMG_COL_RED = 64;
    public const Byte DMG_COL_YELLOW = 128;
    public const Byte DMG_COL_GREEN = 192;

    public const Byte ABR_OFF = 255;
    public const Byte ABR_50ADD = 0;
    public const Byte ABR_ADD = 1;
    public const Byte ABR_SUB = 2;
    public const Byte ABR_25ADD = 3;

    public const Int32 ICON_POS_DEFAULT = 0; // Venom, Poison, Sleep, Haste, Slow
    public const Int32 ICON_POS_HEAD = 1; // Heat, Freeze, Reflect
    public const Int32 ICON_POS_MOUTH = 2; // Silence
    public const Int32 ICON_POS_EYES = 3; // Blind
    public const Int32 ICON_POS_FOREHEAD = 4; // Trouble, Berserk
    public const Int32 ICON_POS_NUMBER = 5; // Doom, Gradual Petrify
    public const Int32 ICON_POS_ROOT = 100; // Root bone (0)
    public const Int32 ICON_POS_WEAPON = 101; // Weapon attachment
    public const Int32 ICON_POS_TARGET = 102; // Targeting cursor

    public const Int32 SOTSIZE = 4096;

    public const Byte Sprtcode = 100;

    public static Boolean ShouldShowSPS = false;
    public static List<HUDMessageChild> StatusMessages = new List<HUDMessageChild>();
}
