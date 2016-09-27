using System;
using Memoria;
using UnityEngine;

#pragma warning disable 414

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable RedundantExplicitArraySize

[ExportedType("Ñ¢čy*!!!ĥŀbĠÒæÃûìĠª¥*pñĀÜķËŃv×°¦Àľéþ¬»Xī@!!!VĲ0A÷ĄNĨ'Åû»J5üİY/ĕĞÒĢðąē(Kłohlċ<#$Öoÿ÷Ð{­/ĦĹJĸMãRÏÏÃ©$¹ÑġÉ«ėÝk°ěÁÎrí9i`JôÍÙó§@ĭľġ¡úĦT+´çìÒlòÖË6~äĭvĭKR¿£33)!aĶŁ6çčĆÇÇÿwB!!!ĄÀļěþkēÚí¤sôþ¾ĩĉÛ°G°'S©QÃ=(xĂ¸ĴÈąLÁÞĨV)§cß¿áa¼čäĎĄćOÍÊâ8ċ-?ĝęĿ°­/7ĤŃ5!ªÙ¡ęy^M_ĊäjvÃģpoj1_5Ćh1ĉČį9I7ö¸Ćùâ®¥ĀÕßìĪÈÁMîÛV«-H¯ļ·ĘvÒîÁ#!!!(6§Gńńńń%!!!õ¬ĢŁ<Ė'Ì»Ĵ]vńńńńńńńń")]
internal class EIcon
{
    public const Int32 kHereIconTime = 60;
    public const Int32 questionIcon = 0;
    public const Int32 exclamationIcon = 1;
    public const Int32 cardIcon = 2;
    public const Int32 beachIcon = 3;
    public const Int32 exclamationAndBeachIcon = 4;
    //private const int kAIconInterval = 44;
    //private const int kAIconInterval2 = 15;
    //private const int kAIconForce = 4;
    //private const int kAIconModeMask = 3;
    private static Boolean sFIconPolled;
    private static Boolean sFIconLastPolled;
    private static Int32 sFIconType;
    private static Int32 sFIconLastType;
    private static PollType lastPollType;
    private static Boolean dialogBubble;
    private static Boolean dialogAlternativeKey;
    private static Boolean processFIcon;
    private static Boolean recheckScript; // Don't remove it!
    private static Boolean hereIconShow;
    private static Int32 sHereIconTimer;
    private static Boolean sHereIconForce; // Don't remove it!
    public static readonly Vector3 worldActorOffset;
    private static Camera worldCamera;
    private static FieldMap currentFieldMap;
    private static Int32 sAIconMode;
    private static Int32 sAIconTimer;
    private static ATEType currentATE;

    public static FieldMap FieldMap
    {
        get
        {
            if (currentFieldMap == null)
                currentFieldMap = PersistenSingleton<EventEngine>.Instance.fieldmap;
            return currentFieldMap;
        }
    }

    public static Camera WorldCamera => worldCamera ?? (worldCamera = GameObject.Find("WorldCamera").GetComponent<Camera>());

    public static Single ShowDelay { get; set; }

    public static Single HideDelay { get; set; }

    public static Boolean IsProcessingFIcon
    {
        get { return processFIcon; }
        set { processFIcon = value; }
    }

    public static Boolean IsDialogBubble => dialogBubble;

    public static Int32 SFIconType => sFIconType;

    public static ATEType CurrentATE => currentATE;

    static EIcon()
    {
        dialogBubble = false;
        dialogAlternativeKey = false;
        processFIcon = true;
        recheckScript = false;
        hereIconShow = false;
        ShowDelay = 0.0f;
        HideDelay = 0.0f;
        worldActorOffset = new Vector3(0.0f, 1.8f, 0.0f);
        currentATE = ATEType.Blue;
    }

    public static void InitFIcon()
    {
        sFIconPolled = sFIconLastPolled = false;
        dialogBubble = false;
        lastPollType = PollType.NONE;
        hereIconShow = false;
        processFIcon = true;
        ShowDelay = 0.0f;
        HideDelay = 0.0f;
    }

    public static void PollFIcon(Int32 type)
    {
        lastPollType = PollType.EVENT_SCRIPT;
        sFIconPolled = true;
        sFIconType = type;
        CloseHereIcon();
    }

    public static Boolean PollCollisionIcon(Obj targetObject)
    {
        Boolean flag1 = false;
        if (EventHUD.CurrentHUD == MinigameHUD.MogTutorial)
            return false;
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (instance.gMode == 1)
        {
            Boolean flag2 = instance.GetIP(targetObject.sid, 3, targetObject.ebData) != instance.nil && 1 < targetObject.level;
            Boolean flag3 = instance.GetIP(targetObject.sid, 8, targetObject.ebData) != instance.nil && 1 < targetObject.level;
            if (flag2 && flag3)
            {
                PollFIcon(2);
                flag1 = true;
            }
            else if (flag2 && instance.IsActuallyTalkable(targetObject))
            {
                PollFIcon(1);
                flag1 = true;
            }
        }
        else if (instance.GetIP(targetObject.sid, 2, targetObject.ebData) != instance.nil && 1 < targetObject.level)
        {
            if (EMinigame.CheckBeachMinigame() && !EventCollision.IsWorldTrigger())
                PollFIcon(4);
            else
                PollFIcon(1);
            flag1 = true;
        }
        return flag1;
    }

    public static void ProcessFIcon()
    {
        if (!processFIcon || dialogBubble)
            return;

        EventEngine instance1 = PersistenSingleton<EventEngine>.Instance; // Don't remove it!
        BubbleUI instance2 = Singleton<BubbleUI>.Instance;
        if (instance2 == null)
            return;
        if (!instance2.gameObject.activeSelf)
            instance2.gameObject.SetActive(true);
        Int32 num = 0;
        if (!sFIconLastPolled && sFIconPolled)
            num = 1;
        else if (sFIconLastPolled && !sFIconPolled)
            num = 2;
        else if (sFIconLastPolled && sFIconPolled && !instance2.IsActive)
        {
            if (HideDelay > 0.0)
                HideDelay -= Time.deltaTime;
            else
                num = 1;
        }
        else if (!sFIconLastPolled && !sFIconPolled && (instance2.IsActive && !hereIconShow))
        {
            if (ShowDelay > 0.0)
                ShowDelay -= Time.deltaTime;
            else
                num = 2;
        }
        sFIconLastPolled = sFIconPolled;
        sFIconPolled = false;
        if (num == 1)
            ShowBubble();
        else if (num == 2)
            HideBubble();
        else if (!hereIconShow && instance2.IsActive)
        {
            Boolean flag = sFIconType != sFIconLastType;
            if (flag)
            {
                HideBubble();
                sFIconPolled = false;
                sFIconLastPolled = sFIconPolled;
            }
        }
        sFIconLastType = sFIconType;
    }

    private static void ShowBubble()
    {
        if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
            ShowFieldBubble();
        else
            ShowWorldBubble();
    }

    public static void HideBubble()
    {
        if (!(Singleton<BubbleUI>.Instance != null) || !Singleton<BubbleUI>.Instance.IsActive)
            return;
        HideDelay = Singleton<BubbleUI>.Instance.AnimationDuration;
        lastPollType = PollType.NONE;
        Singleton<BubbleUI>.Instance.Hide();
    }

    private static void ShowFieldBubble()
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        hereIconShow = false;
        Obj objByUid = instance.FindObjByUID(instance.GetControlUID());
        if (objByUid.go == null)
            return;
        ShowDelay = Singleton<BubbleUI>.Instance.AnimationDuration;
        BubbleUI.Flag[] bubbleFlagData = GetBubbleFlagData(sFIconType);
        if (objByUid.cid == 4 && objByUid.go.activeSelf)
        {
            Transform bone;
            Vector3 offset;
            BubbleMappingInfo.GetActorInfo((PosObj)objByUid, out bone, out offset);
            Singleton<BubbleUI>.Instance.Show(bone, (PosObj)objByUid, null, FieldMap, offset, bubbleFlagData, null);
        }
        else
            Singleton<BubbleUI>.Instance.Show(null, (PosObj)objByUid, null, FieldMap, Vector3.zero, bubbleFlagData, null);
    }

    public static void ShowWorldBubble()
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        BubbleUI.Flag[] bubbleFlagData = GetBubbleFlagData(sFIconType);
        System.Action<PosObj, Obj, UInt32>[] listener = new System.Action<PosObj, Obj, UInt32>[1] {EventCollision.BubbleUIListener};
        PosObj controlChar = instance.GetControlChar();
        Vector3 uiDefaultOffset = BubbleUI.UIDefaultOffset;
        if (controlChar.go == null)
            return;
        if (EventCollision.IsChocoboFlyingOverForest() || dialogBubble && dialogAlternativeKey)
        {
            Singleton<BubbleUI>.Instance.ChangePrimaryKey(Control.Cancel);
            dialogAlternativeKey = false;
        }
        else
            Singleton<BubbleUI>.Instance.ChangePrimaryKey(Control.Confirm);
        ShowDelay = Singleton<BubbleUI>.Instance.AnimationDuration;
        Vector3 actorOffset;
        GetWorldActorOffset(out actorOffset, ref uiDefaultOffset);
        Singleton<BubbleUI>.Instance.Show(controlChar.go.transform, controlChar, null, WorldCamera, actorOffset, uiDefaultOffset, bubbleFlagData, listener);
    }

    public static void ShowDialogBubble(Boolean useAlternativeKey = false)
    {
        dialogBubble = true;
        dialogAlternativeKey = useAlternativeKey;
        sFIconType = 1;
        ShowWorldBubble();
    }

    public static void HideDialogBubble()
    {
        dialogBubble = false;
        HideBubble();
    }

    public static Vector3 GetWorldActorOffset(out Vector3 actorOffset, ref Vector3 uiOffset)
    {
        switch (WMUIData.ControlNo)
        {
            case 5:
                actorOffset = worldActorOffset * 1.4f;
                break;
            case 6:
                actorOffset = worldActorOffset * 1.9f;
                break;
            case 8:
                actorOffset = worldActorOffset * 2.2f;
                uiOffset.x -= 70f;
                break;
            case 9:
                actorOffset = worldActorOffset * 1.7f;
                uiOffset.x -= 70f;
                break;
            default:
                actorOffset = worldActorOffset;
                break;
        }
        return actorOffset;
    }

    public static BubbleUI.Flag[] GetBubbleFlagData(Int32 pollCode)
    {
        switch (pollCode)
        {
            case 0:
                return new BubbleUI.Flag[1]
                {
                    BubbleUI.Flag.QUESTION
                };
            case 2:
                return new BubbleUI.Flag[2]
                {
                    BubbleUI.Flag.EXCLAMATION,
                    BubbleUI.Flag.DUEL
                };
            case 3:
                return new BubbleUI.Flag[1]
                {
                    BubbleUI.Flag.BEACH
                };
            case 4:
                return new BubbleUI.Flag[2]
                {
                    BubbleUI.Flag.EXCLAMATION,
                    BubbleUI.Flag.BEACH
                };
            default:
                return new BubbleUI.Flag[1];
        }
    }

    public static void SetHereIcon(Int32 f)
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (f <= 0 || EventHUD.CurrentHUD == MinigameHUD.ChocoHot)
        {
            sHereIconTimer = 0;
            sHereIconForce = false;
        }
        else
        {
            UInt64 num = FF9StateSystem.Settings.cfg.here_icon > 0UL ? 0UL : 1UL;
            if (f <= 2 && (!instance.GetUserControl() || num <= 1UL && ((Int64)num != 1L || f != 2 && instance.gAnimCount <= 0 && instance.eTb.gMesCount < 3)))
                return;
            sHereIconTimer = 60;
        }
    }

    public static void ProcessHereIcon(PosObj po)
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (instance.GetUserControl() && (ETb.KeyOn() & 1U) > 0U)
        {
            sHereIconTimer = 60;
            sHereIconForce = true;
            hereIconShow = false;
        }
        if (sHereIconTimer > 0)
        {
            --sHereIconTimer;
            if (sHereIconTimer <= 0)
                sHereIconForce = false;
            if (instance.gMode != 1 || hereIconShow || (lastPollType != PollType.NONE || sHereIconTimer <= 0) || sHereIconTimer >= 58)
                return;
            ShowHereIcon(po);
        }
        else
            CloseHereIcon();
    }

    private static void CloseHereIcon()
    {
        if (!hereIconShow || sHereIconTimer > 0)
            return;
        HideBubble();
        hereIconShow = false;
    }

    private static void ShowHereIcon(PosObj po)
    {
        hereIconShow = true;
        ShowDelay = 0.175f;
        BubbleUI.Flag flag = BubbleUI.Flag.CURSOR;
        Transform bone;
        Vector3 offset;
        BubbleMappingInfo.GetActorInfo(po, out bone, out offset);
        Singleton<BubbleUI>.Instance.Show(bone, po, null, FieldMap, offset, new BubbleUI.Flag[1] {flag}, null);
    }

    public static void ProcessAIcon()
    {
        if (sAIconMode > 0 && ((sAIconMode & 4) > 0 || PersistenSingleton<EventEngine>.Instance.GetUserControl()))
        {
            ++sAIconTimer;
            if ((sAIconMode & 3) != 2)
            {
                currentATE = ATEType.Blue;
                ShowAIcon(true, currentATE);
            }
            else
            {
                if ((sAIconTimer / 15 & 1) <= 0)
                    return;
                currentATE = ATEType.Gray;
                ShowAIcon(true, currentATE);
            }
        }
        else
            ShowAIcon(false, currentATE);
    }

    public static void ShowAIcon(Boolean isActive, ATEType type)
    {
        UIManager.Field.EnableATE(isActive, type);
    }

    public static void SetAIcon(Int32 mode)
    {
        if (mode == 0)
            ShowAIcon(false, CurrentATE);
        if (sAIconMode == mode)
            return;
        sAIconMode = mode;
        sAIconTimer = 44;
    }

    public enum PollType
    {
        NONE,
        EVENT_SCRIPT,
    }
}