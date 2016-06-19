using System;
using Memoria;
using UnityEngine;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable RedundantExplicitArraySize

[ExportedType("Ñ¢čy*!!!ĥŀbĠÒæÃûìĠª¥*pñĀÜķËŃv×°¦Àľéþ¬»Xī@!!!VĲ0A÷ĄNĨ'Åû»J5üİY/ĕĞÒĢðąē(Kłohlċ<#$Öoÿ÷Ð{­/ĦĹJĸMãRÏÏÃ©$¹ÑġÉ«ėÝk°ěÁÎrí9i`JôÍÙó§@ĭľġ¡úĦT+´çìÒlòÖË6~äĭvĭKR¿£33)!aĶŁ6çčĆÇÇÿwB!!!ĄÀļěþkēÚí¤sôþ¾ĩĉÛ°G°'S©QÃ=(xĂ¸ĴÈąLÁÞĨV)§cß¿áa¼čäĎĄćOÍÊâ8ċ-?ĝęĿ°­/7ĤŃ5!ªÙ¡ęy^M_ĊäjvÃģpoj1_5Ćh1ĉČį9I7ö¸Ćùâ®¥ĀÕßìĪÈÁMîÛV«-H¯ļ·ĘvÒîÁ#!!!(6§Gńńńń%!!!õ¬ĢŁ<Ė'Ì»Ĵ]vńńńńńńńń")]
internal class EIcon
{
    public static readonly Vector3 worldActorOffset = new Vector3(0.0f, 1.8f, 0.0f);
    public const int kHereIconTime = 60;
    public const int questionIcon = 0;
    public const int exclamationIcon = 1;
    public const int cardIcon = 2;
    public const int beachIcon = 3;
    public const int exclamationAndBeachIcon = 4;

    private static bool s_dialogAlternativeKey;
    private static bool s_hereIconShow;
    private static ATEType s_currentAte = ATEType.Blue;

    //private const int kAIconInterval = 44;
    //private const int kAIconInterval2 = 15;
    //private const int kAIconForce = 4;
    //private const int kAIconModeMask = 3;

    private static bool s_FIconPolled;
    private static bool s_FIconLastPolled;
    private static int s_FIconType;
    private static int s_FIconLastType;
    private static PollType s_lastPollType;
    private static int s_hereIconTimer;
    private static Camera s_worldCamera;
    private static FieldMap s_currentFieldMap;
    private static int s_AIconMode;
    private static int s_AIconTimer;

    public static FieldMap FieldMap => s_currentFieldMap ?? (s_currentFieldMap = PersistenSingleton<EventEngine>.Instance.fieldmap);
    public static Camera WorldCamera => s_worldCamera ?? (s_worldCamera = GameObject.Find("WorldCamera").GetComponent<Camera>());
    public static float ShowDelay { get; set; }
    public static float HideDelay { get; set; }

    public static bool IsProcessingFIcon { get; set; } = true;
    public static bool IsDialogBubble { get; private set; }
    public static int SFIconType => s_FIconType;
    public static ATEType CurrentATE => s_currentAte;

    public static void InitFIcon()
    {
        s_FIconPolled = s_FIconLastPolled = false;
        IsDialogBubble = false;
        s_lastPollType = PollType.NONE;
        s_hereIconShow = false;
        IsProcessingFIcon = true;
        ShowDelay = 0.0f;
        HideDelay = 0.0f;
    }

    public static void PollFIcon(int type)
    {
        s_lastPollType = PollType.EVENT_SCRIPT;
        s_FIconPolled = true;
        s_FIconType = type;
        CloseHereIcon();
    }

    public static bool PollCollisionIcon(Obj targetObject)
    {
        if (EventHUD.CurrentHUD == MinigameHUD.MogTutorial)
            return false;

        bool flag1 = false;

        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (instance.gMode == 1)
        {
            bool flag2 = instance.GetIP(targetObject.sid, 3, targetObject.ebData) != instance.nil && 1 < targetObject.level;
            bool flag3 = instance.GetIP(targetObject.sid, 8, targetObject.ebData) != instance.nil && 1 < targetObject.level;
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
        if (!IsProcessingFIcon || IsDialogBubble)
            return;

        //EventEngine instance1 = PersistenSingleton<EventEngine>.Instance;
        BubbleUI instance2 = Singleton<BubbleUI>.Instance;
        if (instance2 == null)
            return;

        if (!instance2.gameObject.activeSelf)
            instance2.gameObject.SetActive(true);

        int num = 0;
        if (!s_FIconLastPolled && s_FIconPolled)
            num = 1;
        else if (s_FIconLastPolled && !s_FIconPolled)
            num = 2;
        else if (s_FIconLastPolled && s_FIconPolled && !instance2.IsActive)
        {
            if (HideDelay > 0.0)
                HideDelay -= Time.deltaTime;
            else
                num = 1;
        }
        else if (!s_FIconLastPolled && !s_FIconPolled && (instance2.IsActive && !s_hereIconShow))
        {
            if (ShowDelay > 0.0)
                ShowDelay -= Time.deltaTime;
            else
                num = 2;
        }

        s_FIconLastPolled = s_FIconPolled;
        s_FIconPolled = false;

        if (num == 1)
            ShowBubble();
        else if (num == 2)
            HideBubble();
        else if (!s_hereIconShow && instance2.IsActive)
        {
            bool flag = s_FIconType != s_FIconLastType;
            if (flag)
            {
                HideBubble();
                s_FIconPolled = false;
                s_FIconLastPolled = s_FIconPolled;
            }
        }

        s_FIconLastType = s_FIconType;
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
        s_lastPollType = PollType.NONE;
        Singleton<BubbleUI>.Instance.Hide();
    }

    private static void ShowFieldBubble()
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        s_hereIconShow = false;
        Obj objByUid = instance.FindObjByUID(instance.GetControlUID());
        if (objByUid.go == null)
            return;
        ShowDelay = Singleton<BubbleUI>.Instance.AnimationDuration;
        BubbleUI.Flag[] bubbleFlagData = GetBubbleFlagData(s_FIconType);
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
        BubbleUI.Flag[] bubbleFlagData = GetBubbleFlagData(s_FIconType);
        Action<PosObj, Obj, uint>[] listener = new Action<PosObj, Obj, uint>[1] {EventCollision.BubbleUIListener};
        PosObj controlChar = instance.GetControlChar();
        Vector3 uiOffset = BubbleUI.UIDefaultOffset;
        if (controlChar.go == null)
            return;

        if (EventCollision.IsChocoboFlyingOverForest() || IsDialogBubble && s_dialogAlternativeKey)
        {
            Singleton<BubbleUI>.Instance.ChangePrimaryKey(Control.Cancel);
            s_dialogAlternativeKey = false;
        }
        else
        {
            Singleton<BubbleUI>.Instance.ChangePrimaryKey(Control.Confirm);
        }

        ShowDelay = Singleton<BubbleUI>.Instance.AnimationDuration;
        Vector3 actorOffset;
        GetWorldActorOffset(out actorOffset, ref uiOffset);
        Singleton<BubbleUI>.Instance.Show(controlChar.go.transform, controlChar, null, WorldCamera, actorOffset, uiOffset, bubbleFlagData, listener);
    }

    public static void ShowDialogBubble(bool useAlternativeKey = false)
    {
        IsDialogBubble = true;
        s_dialogAlternativeKey = useAlternativeKey;
        s_FIconType = 1;
        ShowWorldBubble();
    }

    public static void HideDialogBubble()
    {
        IsDialogBubble = false;
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

    public static BubbleUI.Flag[] GetBubbleFlagData(int pollCode)
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

    public static void SetHereIcon(int f)
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (f <= 0 || EventHUD.CurrentHUD == MinigameHUD.ChocoHot)
        {
            s_hereIconTimer = 0;
        }
        else
        {
            ulong num = FF9StateSystem.Settings.cfg.here_icon > 0UL ? 0UL : 1UL;
            if (f <= 2 && (!instance.GetUserControl() || num <= 1UL && ((long)num != 1L || f != 2 && instance.gAnimCount <= 0 && instance.eTb.gMesCount < 3)))
                return;
            s_hereIconTimer = 60;
        }
    }

    public static void ProcessHereIcon(PosObj po)
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (instance.GetUserControl() && (ETb.KeyOn() & 1U) > 0U)
        {
            s_hereIconTimer = 60;
            s_hereIconShow = false;
        }
        if (s_hereIconTimer > 0)
        {
            --s_hereIconTimer;
            if (instance.gMode != 1 || s_hereIconShow || (s_lastPollType != PollType.NONE || s_hereIconTimer <= 0) || s_hereIconTimer >= 58)
                return;
            ShowHereIcon(po);
        }
        else
            CloseHereIcon();
    }

    private static void CloseHereIcon()
    {
        if (!s_hereIconShow || s_hereIconTimer > 0)
            return;
        HideBubble();
        s_hereIconShow = false;
    }

    private static void ShowHereIcon(PosObj po)
    {
        s_hereIconShow = true;
        ShowDelay = 0.175f;
        BubbleUI.Flag flag = BubbleUI.Flag.CURSOR;
        Transform bone;
        Vector3 offset;
        BubbleMappingInfo.GetActorInfo(po, out bone, out offset);
        Singleton<BubbleUI>.Instance.Show(bone, po, null, FieldMap, offset, new BubbleUI.Flag[1] { flag }, null);
    }

    public static void ProcessAIcon()
    {
        if (s_AIconMode > 0 && ((s_AIconMode & 4) > 0 || PersistenSingleton<EventEngine>.Instance.GetUserControl()))
        {
            ++s_AIconTimer;
            if ((s_AIconMode & 3) != 2)
            {
                s_currentAte = ATEType.Blue;
                ShowAIcon(true, s_currentAte);
            }
            else
            {
                if ((s_AIconTimer / 15 & 1) <= 0)
                    return;
                s_currentAte = ATEType.Gray;
                ShowAIcon(true, s_currentAte);
            }
        }
        else
            ShowAIcon(false, s_currentAte);
    }

    public static void ShowAIcon(bool isActive, ATEType type)
    {
        UIManager.Field.EnableATE(isActive, type);
    }

    public static void SetAIcon(int mode)
    {
        if (mode == 0)
            ShowAIcon(false, CurrentATE);
        if (s_AIconMode == mode)
            return;
        s_AIconMode = mode;
        s_AIconTimer = 44;
    }

    public enum PollType
    {
        NONE,
        EVENT_SCRIPT,
    }
}
