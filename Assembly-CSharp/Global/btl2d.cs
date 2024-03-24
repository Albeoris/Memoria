using System;
using System.Collections.Generic;
using System.IO;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Prime;
using Memoria.Data;
using NCalc;
using UnityEngine;
using static SiliconStudio.Social.ResponseData;

public static class btl2d
{
    public static void Btl2dInit()
    {
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        ff9Battle.btl2d_work_set.NewID = 0;
        ff9Battle.btl2d_work_set.Timer = 0;
        ff9Battle.btl2d_work_set.OldDisappear = Byte.MaxValue;
        BTL2D_ENT[] entry = ff9Battle.btl2d_work_set.Entry;
        for (Int16 num = 0; num < 16; num++)
            entry[num].BtlPtr = null;
    }

    public static void InitBattleSPSBin()
    {
        String[] spsBinNames = new String[]
        {
            "st_doku.dat",
            "st_mdoku.dat",
            "st_moku.dat",
            "st_moum.dat",
            "st_nemu.dat",
            "st_heat.dat",
            "st_friz.dat",
            "st_basak.dat",
            "st_meiwa.dat",
            "st_slow.dat",
            "st_heis.dat",
            "st_rif.dat"
        };
        for (Int32 i = 0; i < btl2d.wStatIconTbl.Length; i++)
        {
            Byte[] bytes = AssetManager.LoadBytes("BattleMap/BattleSPS/" + spsBinNames[i] + ".sps");
            if (bytes == null)
                return;
        }
    }

    public static void Btl2dReq(BattleUnit pBtl)
    {
        Btl2dReq(pBtl.Data, ref pBtl.Data.fig_info, ref pBtl.Data.fig, ref pBtl.Data.m_fig);
    }

    public static void Btl2dReq(BTL_DATA pBtl)
    {
        Btl2dReq(pBtl, ref pBtl.fig_info, ref pBtl.fig, ref pBtl.m_fig);
    }

    public static void Btl2dReq(BTL_DATA pBtl, ref UInt16 fig_info, ref Int32 fig, ref Int32 m_fig)
    {
        Byte delay = 0;
        if (pBtl.bi.disappear == 0)
        {
            if ((fig_info & Param.FIG_INFO_TROUBLE) != 0)
                btl_para.SetTroubleDamage(new BattleUnit(pBtl), fig >> 1);
            if ((fig_info & Param.FIG_INFO_GUARD) != 0)
            {
                btl2d.Btl2dReqSymbol(pBtl, 2, 0, 0);
            }
            else if ((fig_info & (Param.FIG_INFO_MISS | Param.FIG_INFO_DEATH)) != 0)
            {
                if ((fig_info & Param.FIG_INFO_MISS) != 0)
                {
                    btl2d.Btl2dReqSymbol(pBtl, 0, 0, 0);
                    delay = 2;
                }
                if ((fig_info & Param.FIG_INFO_DEATH) != 0)
                    btl2d.Btl2dReqSymbol(pBtl, 1, 0, delay);
            }
            else
            {
                if ((fig_info & Param.FIG_INFO_DISP_HP) != 0)
                {
                    if ((fig_info & Param.FIG_INFO_HP_CRITICAL) != 0)
                    {
                        btl2d.Btl2dReqSymbol(pBtl, 3, 128, 0);
                        delay = 2;
                    }
                    if ((fig_info & Param.FIG_INFO_HP_RECOVER) != 0)
                        btl2d.Btl2dReqHP(pBtl, fig, 192, delay);
                    else
                        btl2d.Btl2dReqHP(pBtl, fig, 0, delay);
                    delay += 4;
                }
                if ((fig_info & Param.FIG_INFO_DISP_MP) != 0)
                {
                    if ((fig_info & Param.FIG_INFO_MP_RECOVER) != 0)
                        btl2d.Btl2dReqMP(pBtl, m_fig, 192, delay);
                    else
                        btl2d.Btl2dReqMP(pBtl, m_fig, 0, delay);
                }
            }
        }
        fig_info = 0;
        fig = 0;
        m_fig = 0;
    }

    public static void Btl2dStatReq(BTL_DATA pBtl)
    {
        Byte b = 0;
        UInt16 fig_stat_info = pBtl.fig_stat_info;
        if (pBtl.bi.disappear == 0)
        {
            if ((fig_stat_info & Param.FIG_STAT_INFO_REGENE_HP) != 0)
            {
                BTL2D_ENT btl2D_ENT = btl2d.Btl2dReqHP(pBtl, pBtl.fig_regene_hp, (UInt16)(((fig_stat_info & Param.FIG_STAT_INFO_REGENE_DMG) == 0) ? 192 : 0), 0);
                btl2D_ENT.NoClip = 1;
                btl2D_ENT.Yofs = -12;
                b = 4;
            }
            if ((fig_stat_info & Param.FIG_STAT_INFO_POISON_HP) != 0)
            {
                BTL2D_ENT btl2D_ENT = btl2d.Btl2dReqHP(pBtl, pBtl.fig_poison_hp, 0, b);
                btl2D_ENT.NoClip = 1;
                btl2D_ENT.Yofs = -12;
                // HonoluluBattleMain.battleSPS.ResetBtlSPSObj(pBtl, BattleStatus.Poison); // [DV] Reset the poison display every time it tick, as it tends to disappear randomly...
                b += 4;
            }
            if ((fig_stat_info & Param.FIG_STAT_INFO_POISON_MP) != 0)
            {
                BTL2D_ENT btl2D_ENT = btl2d.Btl2dReqMP(pBtl, pBtl.fig_poison_mp, 0, b);
                btl2D_ENT.NoClip = 1;
                btl2D_ENT.Yofs = -12;
                b += 4;
            }
            if ((fig_stat_info & Param.FIG_STAT_INFO_REGENE_MP) != 0)
            {
                BTL2D_ENT btl2D_ENT = btl2d.Btl2dReqMP(pBtl, pBtl.fig_regene_mp, 192, b);
                btl2D_ENT.NoClip = 1;
                btl2D_ENT.Yofs = -12;
            }
        }
        pBtl.fig_stat_info = 0;
        pBtl.fig_regene_hp = 0;
        pBtl.fig_regene_mp = 0;
        pBtl.fig_poison_hp = 0;
        pBtl.fig_poison_mp = 0;
    }

    public static BTL2D_ENT GetFreeEntry(BTL_DATA pBtl)
    {
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        BTL2D_WORK btl2d_work_set = ff9Battle.btl2d_work_set;
        Int16 num = (Int16)(btl2d_work_set.NewID - 1);
        if (num < 0)
            num = 15;
        btl2d_work_set.NewID = num;
        BTL2D_ENT btl2D_ENT = btl2d_work_set.Entry[num];
        btl2D_ENT.BtlPtr = pBtl;
        btl2D_ENT.Cnt = 0;
        btl2D_ENT.Delay = 0;
        btl2D_ENT.trans = pBtl.gameObject.transform.GetChildByName("bone" + pBtl.tar_bone.ToString("D3"));
        Vector3 position = btl2D_ENT.trans.position;
        btl2D_ENT.Yofs += 4;
        btl2D_ENT.trans.position = position;
        return btl2D_ENT;
    }

    public static BTL2D_ENT Btl2dReqHP(BTL_DATA pBtl, Int32 pNum, UInt16 pCol, Byte pDelay)
    {
        BTL2D_ENT freeEntry = btl2d.GetFreeEntry(pBtl);
        freeEntry.Type = 0;
        freeEntry.Delay = pDelay;
        freeEntry.Work.Num.Color = pCol;
        freeEntry.Work.Num.Value = (UInt32)pNum;
        return freeEntry;
    }

    public static BTL2D_ENT Btl2dReqMP(BTL_DATA pBtl, Int32 pNum, UInt16 pCol, Byte pDelay)
    {
        BTL2D_ENT freeEntry = btl2d.GetFreeEntry(pBtl);
        freeEntry.Type = 1;
        freeEntry.Delay = pDelay;
        freeEntry.Work.Num.Color = pCol;
        freeEntry.Work.Num.Value = (UInt32)pNum;
        return freeEntry;
    }

    public static BTL2D_ENT Btl2dReqSymbol(BTL_DATA pBtl, UInt32 pNum, UInt16 pCol, Byte pDelay)
    {
        BTL2D_ENT freeEntry = btl2d.GetFreeEntry(pBtl);
        freeEntry.Type = 2;
        freeEntry.Delay = pDelay;
        freeEntry.Work.Num.Color = pCol;
        freeEntry.Work.Num.Value = pNum;
        return freeEntry;
    }

    public static BTL2D_ENT Btl2dReqSymbolMessage(BTL_DATA pBtl, String messageColor, Dictionary<String, String> multiLangMessage, HUDMessage.MessageStyle style, Byte pDelay)
    {
        BTL2D_ENT freeEntry = btl2d.GetFreeEntry(pBtl);
        freeEntry.Type = 3;
        freeEntry.Delay = pDelay;
        freeEntry.CustomColor = messageColor;
        if (!multiLangMessage.TryGetValue(Localization.GetSymbol(), out freeEntry.CustomMessage))
            multiLangMessage.TryGetValue("US", out freeEntry.CustomMessage);
        freeEntry.CustomStyle = style;
        return freeEntry;
    }

    public static void Btl2dMain()
    {
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        BTL2D_WORK workSet = ff9Battle.btl2d_work_set;
        Int16 entryIndex = workSet.NewID;
        for (Int16 i = 0; i < 16; i++)
        {
            BTL2D_ENT btl2dMessage = workSet.Entry[entryIndex];
            if (btl2dMessage.BtlPtr != null)
            {
                if (btl2dMessage.Type > 3)
                {
                    btl2dMessage.BtlPtr = null;
                }
                else if (btl2dMessage.Delay != 0)
                {
                    btl2dMessage.Delay--;
                }
                else
                {
                    HUDMessage.MessageStyle style = HUDMessage.MessageStyle.DAMAGE;
                    String message = String.Empty;
                    String format = String.Empty;
                    if (btl2dMessage.Type == 0)
                    {
                        if (btl2dMessage.Work.Num.Color == 0)
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
                        message = btl2dMessage.Work.Num.Value.ToString();
                    }
                    else if (btl2dMessage.Type == 1)
                    {
                        if (btl2dMessage.Work.Num.Color == 0)
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
                        message = btl2dMessage.Work.Num.Value.ToString() + " " + Localization.Get("MPCaption");
                    }
                    else if (btl2dMessage.Type == 2)
                    {
                        if (btl2dMessage.Work.Num.Value == 0u)
                        {
                            message = Localization.Get("Miss");
                            style = HUDMessage.MessageStyle.MISS;
                        }
                        else if (btl2dMessage.Work.Num.Value == 1u)
                        {
                            message = Localization.Get("Death");
                            style = HUDMessage.MessageStyle.DEATH;
                        }
                        else if (btl2dMessage.Work.Num.Value == 2u)
                        {
                            message = Localization.Get("Guard");
                            style = HUDMessage.MessageStyle.GUARD;
                        }
                        else if (btl2dMessage.Work.Num.Value == 3u)
                        {
                            message = NGUIText.FF9YellowColor + Localization.Get("Critical") + "[-] \n ";
                            style = HUDMessage.MessageStyle.CRITICAL;
                        }
                        else if (btl2dMessage.Work.Num.Value == 0x10000u)
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
                            expr.Parameters["DamageValue"] = btl2dMessage.Work.Num.Value;
                            expr.Parameters["HealValue"] = btl2dMessage.Work.Num.Value;
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
            }
            entryIndex++;
            if (entryIndex >= 16)
                entryIndex = 0;
        }
        btl2d.Btl2dStatCount();
        if (SFX.GetEffectJTexUsed() == 0)
            btl2d.Btl2dStatIcon();
        workSet.Timer++;
        Byte oldDisappear = Byte.MaxValue;
        for (BTL_DATA btl = ff9Battle.btl_list.next; btl != null; btl = btl.next)
            if (btl.bi.disappear == 0)
                oldDisappear &= (Byte)~btl.btl_id;
        workSet.OldDisappear = oldDisappear;
    }

    private static void Btl2dStatIcon()
    {
        BTL2D_WORK btl2d_work_set = FF9StateSystem.Battle.FF9Battle.btl2d_work_set;
        Vector3 rot;
        rot.x = 0f;
        rot.z = 0f;
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.bi.disappear == 0)
            {
                if ((btl.flags & geo.GEO_FLAGS_CLIP) == 0)
                {
                    if ((btl2d_work_set.OldDisappear & btl.btl_id) == 0)
                    {
                        BattleStatus statusOn = btl.stat.cur | btl.stat.permanent;
                        if ((statusOn & BattleStatus.Death) == 0)
                        {
                            if ((statusOn & STATUS_2D_ICON) != 0)
                            {
                                if (btl.bi.player == 0 || !btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE))
                                {
                                    Int32 angledx = ff9.rsin((Int32)(btl.rot.eulerAngles.y / 360f * 4096f));
                                    Int32 angledz = ff9.rcos((Int32)(btl.rot.eulerAngles.y / 360f * 4096f));
                                    btl2d.GetIconPosition(btl, out Byte[] iconBones, out SByte[] iconOffsY, out SByte[] iconOffsZ);
                                    for (Int32 i = 0; i < btl2d.wStatIconTbl.Length; i++)
                                    {
                                        btl2d.STAT_ICON_TBL statTable = btl2d.wStatIconTbl[i];
                                        if ((statusOn & statTable.Mask) != 0)
                                        {
                                            if ((statusOn & statTable.Mask2) == 0)
                                            {
                                                Int16 dy = (Int16)(iconOffsY[statTable.Pos] << 4);
                                                Int16 dz = (Int16)(iconOffsZ[statTable.Pos] << 4);
                                                if ((btl.flags & geo.GEO_FLAGS_SCALE) != 0)
                                                {
                                                    dy = (Int16)(dy * btl.gameObject.transform.localScale.y);
                                                    dz = (Int16)(dz * btl.gameObject.transform.localScale.z);
                                                }
                                                Vector3 pos = btl.gameObject.transform.GetChildByName("bone" + iconBones[statTable.Pos].ToString("D3")).position;
                                                pos.x += dz * angledx >> 12;
                                                pos.y -= dy;
                                                pos.z += dz * angledz >> 12;
                                                if (statTable.Type != 0)
                                                {
                                                    rot.y = 0f;
                                                }
                                                else if (statTable.Ang != 0)
                                                {
                                                    Int32 angle = (Int32)(btl.rot.eulerAngles.y / 360f * 4095f);
                                                    angle = angle + 3072 & 4095;
                                                    rot.y = angle / 4095f * 360f;
                                                }
                                                else
                                                {
                                                    rot.y = 0f;
                                                }
                                                HonoluluBattleMain.battleSPS.UpdateBtlStatus(btl, statTable.Mask, pos, rot, btl2d_work_set.Timer);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static Int16 S_GetShpFrame(BinaryReader shp)
    {
        shp.BaseStream.Seek(0L, SeekOrigin.Begin);
        return (Int16)(shp.ReadInt16() & Int16.MaxValue);
    }

    public static UInt16 acUShort(BinaryReader p, Int32 index = 0)
    {
        p.BaseStream.Seek(index, SeekOrigin.Begin);
        return p.ReadUInt16();
    }

    public static Byte acChar(BinaryReader p, Int32 index = 0)
    {
        p.BaseStream.Seek(index, SeekOrigin.Begin);
        return p.ReadByte();
    }

    public static UInt64 acULong(BinaryReader p, Int32 index = 0)
    {
        p.BaseStream.Seek(index, SeekOrigin.Begin);
        return p.ReadUInt64();
    }

    public static Int32 SAbrID(Int32 abr)
    {
        return (abr & 3) << 5;
    }

    public static Int32 getSprtcode(Int32 abr)
    {
        return 100 | ((abr != 255) ? 2 : 0);
    }

    public static void S_ShpNScPut(BinaryReader shp, Vector3 pos, Int32 frame, Int32 abr, Int32 fade)
    {
    }

    public static void S_SpsNScPut(BinaryReader sps, Vector3 pos, Vector3 ang, Int32 sc, Int32 frame, Int32 abr, Int32 fade, Int32 pad)
    {
    }

    private static void Btl2dStatCount()
    {
        btl2d.STAT_CNT_TBL[] statusTableList = new btl2d.STAT_CNT_TBL[]
        {
            new btl2d.STAT_CNT_TBL(BattleStatus.Doom, (Int32)BattleStatusNumber.Doom - 1, 0),
            new btl2d.STAT_CNT_TBL(BattleStatus.GradualPetrify, (Int32)BattleStatusNumber.GradualPetrify - 1, 1)
        };
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        BattleStatus counterStatus = BattleStatus.Doom | BattleStatus.GradualPetrify;
        for (BTL_DATA btl = ff9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.bi.disappear == 0)
            {
                if ((btl.flags & geo.GEO_FLAGS_CLIP) == 0)
                {
                    if ((ff9Battle.btl2d_work_set.OldDisappear & btl.btl_id) == 0)
                    {
                        BattleStatus statusOn = btl.stat.cur | btl.stat.permanent;
                        if ((statusOn & BattleStatus.Death) == 0)
                        {
                            if ((statusOn & counterStatus) != 0)
                            {
                                btl2d.GetIconPosition(btl, out Byte[] iconBones, out SByte[] iconOffsY, out _);
                                Int16 iconBone = iconBones[5];
                                Int16 iconOffY = iconOffsY[5];
                                if ((btl.flags & geo.GEO_FLAGS_SCALE) != 0)
                                    iconOffY = (Int16)(iconOffY * btl.gameObject.transform.localScale.y);
                                Transform attachTransf = btl.gameObject.transform.GetChildByName("bone" + iconBone.ToString("D3"));
                                Int32 dy = -(iconOffY << 4);
                                for (Int32 i = 0; i < statusTableList.Length; i++)
                                {
                                    btl2d.STAT_CNT_TBL statusTable = statusTableList[i];
                                    if ((statusOn & statusTable.Mask) != 0)
                                    {
                                        Int16 cdownMax = btl.stat.cnt.cdown_max;
                                        if (cdownMax < 1)
                                            break;
                                        Int16 cdownConti = btl.stat.cnt.conti[statusTable.Idx];
                                        if (cdownConti < 0)
                                            break;
                                        Int32 figureNb = (Int16)(cdownConti * 10 / cdownMax);
                                        UInt16 abrCode;
                                        if (cdownConti <= 0)
                                            abrCode = 2;
                                        else
                                            abrCode = (UInt16)((figureNb == (cdownConti - btl.cur.at_coef) * 10 / cdownMax) ? 0 : 2);
                                        Int32 color;
                                        if (statusTable.Col != 0)
                                        {
                                            Byte intensity = (Byte)((figureNb << 4) + 32);
                                            color = intensity << 16 | intensity << 8 | intensity;
                                        }
                                        else
                                        {
                                            color = 0x1000000;
                                        }
                                        color |= abrCode << 24;
                                        figureNb = Math.Min(figureNb + 1, 10);
                                        String figStr = statusTable.Col != 0 ? $"[{color & 0xFFFFFF:X6}]{figureNb}" : $"{figureNb}";
                                        if (statusTable.Mask == BattleStatus.Doom)
                                        {
                                            if (btl.deathMessage == null)
                                            {
                                                btl.deathMessage = Singleton<HUDMessage>.Instance.Show(attachTransf, figStr, HUDMessage.MessageStyle.DEATH_SENTENCE, new Vector3(0f, dy), 0);
                                                UIManager.Battle.DisplayParty();
                                            }
                                            else
                                            {
                                                btl.deathMessage.Label = figStr;
                                            }
                                        }
                                        else if (statusTable.Mask == BattleStatus.GradualPetrify)
                                        {
                                            if (btl.petrifyMessage == null)
                                            {
                                                btl.petrifyMessage = Singleton<HUDMessage>.Instance.Show(attachTransf, figStr, HUDMessage.MessageStyle.PETRIFY, new Vector3(0f, dy), 0);
                                                UIManager.Battle.DisplayParty();
                                            }
                                            else
                                            {
                                                btl.petrifyMessage.Label = figStr;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static void ReleaseBtl2dStatCount()
    {
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
		{
            if (btl.deathMessage != null)
            {
                Singleton<HUDMessage>.Instance.ReleaseObject(btl.deathMessage);
                btl.deathMessage = null;
            }
            if (btl.petrifyMessage != null)
            {
                Singleton<HUDMessage>.Instance.ReleaseObject(btl.petrifyMessage);
                btl.petrifyMessage = null;
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
                iconBone = param.StatusBone;
                iconOffY = param.StatusOffsetY;
                iconOffZ = param.StatusOffsetZ;
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

    public const Byte BTL2D_NUM = 16;

    public const Byte BTL2D_TYPE_HP = 0;
    public const Byte BTL2D_TYPE_MP = 1;
    public const Byte BTL2D_TYPE_SYM = 2;
    public const Byte BTL2D_TYPE_MAX = 2;

    public const Byte DMG_COL_WHITE = 0;
    public const Byte DMG_COL_RED = 64;
    public const Byte DMG_COL_YELLOW = 128;
    public const Byte DMG_COL_GREEN = 192;

    public const BattleStatus STATUS_2D_ICON = BattleStatus.Venom | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Berserk | BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Haste | BattleStatus.Slow | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Reflect;

    public const Byte ABR_OFF = 255;
    public const Byte ABR_50ADD = 0;
    public const Byte ABR_ADD = 1;
    public const Byte ABR_SUB = 2;
    public const Byte ABR_25ADD = 3;

    public const Int16 STAT_ICON_NUM = 12;

    public const Int32 SOTSIZE = 4096;

    public const Byte Sprtcode = 100;

    public static btl2d.STAT_ICON_TBL[] wStatIconTbl = new btl2d.STAT_ICON_TBL[]
    {
        new btl2d.STAT_ICON_TBL(BattleStatus.Poison, 0u, null, 0, 1, 0, 0),
        new btl2d.STAT_ICON_TBL(BattleStatus.Venom, 0u, null, 0, 1, 0, 0),
        new btl2d.STAT_ICON_TBL(BattleStatus.Slow, 0u, null, 0, Byte.MaxValue, 1, 0),
        new btl2d.STAT_ICON_TBL(BattleStatus.Haste, 0u, null, 0, Byte.MaxValue, 1, 0),
        new btl2d.STAT_ICON_TBL(BattleStatus.Sleep, 0u, null, 0, Byte.MaxValue, 0, 1),
        new btl2d.STAT_ICON_TBL(BattleStatus.Heat, 0u, null, 1, 1, 0, 0),
        new btl2d.STAT_ICON_TBL(BattleStatus.Freeze, 0u, null, 1, 1, 0, 0),
        new btl2d.STAT_ICON_TBL(BattleStatus.Reflect, BattleStatus.Petrify, null, 1, 1, 0, 0),
        new btl2d.STAT_ICON_TBL(BattleStatus.Silence, 0u, null, 2, Byte.MaxValue, 1, 1),
        new btl2d.STAT_ICON_TBL(BattleStatus.Blind, 0u, null, 3, 2, 0, 0),
        new btl2d.STAT_ICON_TBL(BattleStatus.Trouble, 0u, null, 4, Byte.MaxValue, 1, 0),
        new btl2d.STAT_ICON_TBL(BattleStatus.Berserk, 0u, null, 4, 1, 0, 0)
    };

    public class STAT_CNT_TBL
    {
        public STAT_CNT_TBL(BattleStatus mask, Int16 idx, UInt16 col)
        {
            this.Mask = mask;
            this.Idx = idx;
            this.Col = col;
        }

        public BattleStatus Mask;

        public Int16 Idx;

        public UInt16 Col;
    }

    public class STAT_ICON_TBL
    {
        public STAT_ICON_TBL(BattleStatus mask, BattleStatus mask2, BinaryReader spr, Byte pos, Byte abr, Byte type, Byte ang)
        {
            this.Mask = mask;
            this.Mask2 = mask2;
            this.Spr = spr;
            this.Pos = pos;
            this.Abr = abr;
            this.Type = type;
            this.Ang = ang;
            this.texture = null;
        }

        public BattleStatus Mask;

        public BattleStatus Mask2;

        public BinaryReader Spr;

        public Byte Pos;

        public Byte Abr;

        public Byte Type;

        public Byte Ang;

        public Texture2D texture;
    }

    public class S_InShpWork
    {
        public UInt32 rgbcode;

        public Int32 sx;

        public Int32 sy;

        public Int32 abr;

        public Int32 clut;

        public Int32 prim;

        public Int32 otadd;
    }

    public class S_InSpsWork
    {
        public BinaryReader pt;

        public BinaryReader rgb;

        public Int32 w;

        public Int32 h;

        public Int32 tpage;

        public Int32 clut;

        public Int32 fade;

        public Int32 prim;

        public Int32 otadd;

        public Int32 code;
    }
}