using System;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Data;
using Memoria.Assets;
using UnityEngine;

public static class ETb
{
    public static SoundDatabase voiceDatabase = new SoundDatabase();

    public static void InitMessage()
    {
        if (ETb.sInitMesInh)
            ETb.sInitMesInh = false;
        else
            ETb.YWindow_CloseAll();
        ETb.InitMesWin();
    }

    public static void InitMesWin()
    {
        ETb.sChoose = 0;
        DialogManager.SelectChoice = ETb.sChoose;
        ETb.sChooseMask = -1;
        ETb.sChooseInit = 0;
        ETb.gMesCount = 0;
        EventInput.IsNeedAddStartSignal = false;
    }

    public static void InitMovieHitPoint(Int32 MapNo)
    {
        if (PersistenSingleton<UIManager>.Instance.FieldHUDScene.MovieHitArea.activeSelf)
        {
            global::Debug.LogWarning("InitMovieHitPoint() => FieldHUD.MovieHitArea has not been deactivated after played movie in MapNo. " + MapNo + " So, I will deactivate it.");
            PersistenSingleton<UIManager>.Instance.FieldHUDScene.MovieHitArea.SetActive(false);
        }
    }

    public static void InhInitMes()
    {
        ETb.sInitMesInh = true;
    }

    public static void InitKeyEvents()
    {
        ETb.sKey = 0u;
    }

    public static void ProcessKeyEvents()
    {
        UInt32 inputs = ETb.GetInputs();
        ETb.sKeyOn = inputs & ~ETb.sKey;
        ETb.sKeyOff = ~inputs & ETb.sKey;
        ETb.sKey = inputs;
    }

    public static UInt32 GetInputs(Boolean swapJapaneseLayout = false)
    {
        if (swapJapaneseLayout)
            return ETb.ProcessJapaneseLayout(FPSManager.DelayedInputs & 0x3FFFFFFu);
        return FPSManager.DelayedInputs & 0x3FFFFFFu;
    }

    public static UInt32 KeyOn(Boolean swapJapaneseLayout = false)
    {
        if (swapJapaneseLayout)
            return ETb.ProcessJapaneseLayout(ETb.sKeyOn & 0x3FFFFFFu);
        return ETb.sKeyOn & 0x3FFFFFFu;
    }

    public static UInt32 KeyOff(Boolean swapJapaneseLayout = false)
    {
        if (swapJapaneseLayout)
            return ETb.ProcessJapaneseLayout(ETb.sKeyOff & 0x3FFFFFFu);
        return ETb.sKeyOff & 0x3FFFFFFu;
    }

    /// <summary>Take into account that Japanese event scripts use Control.Cancel in place of Confirm and vice versa (eg. "IsButton(589824L)" to close a dialog by script)</summary>
    private static UInt32 ProcessJapaneseLayout(UInt32 inputs)
    {
        Boolean hasConfirm = (inputs & EventInput.Confirm) != 0;
        Boolean hasCancel = (inputs & EventInput.Cancel) != 0;
        if (hasConfirm && !hasCancel)
            inputs = (inputs & ~EventInput.Confirm) | EventInput.Cancel;
        else if (hasCancel && !hasConfirm)
            inputs = (inputs & ~EventInput.Cancel) | EventInput.Confirm;
        return inputs;
    }

    public static void NewMesWin(Int32 mes, Int32 winID, Int32 flags, PosObj targetPo)
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (ETb.IsSkipped(instance, mes, winID, flags, targetPo))
            return;
        ETb.DisposWindowByID(winID, true);
        ETb.FlagsToStyles(flags, out Dialog.WindowStyle windowStyle, out Dialog.CaptionType captionType);
        if ((flags & ETb.WindowChatStyle) == 0)
            targetPo = null;
        if ((flags & ETb.ResetChooseMask) <= 0)
        {
            ETb.sChoose = ETb.sChooseInit;
            ETb.sChooseInit = 0;
        }
        if (instance.gMode == 3)
        {
            targetPo = null;
            if (mes == 40)
                EIcon.ShowDialogBubble(false);
            else if (mes == 41)
                EIcon.ShowDialogBubble(true);
        }
        EventHUD.CheckSpecialHUDFromMesId(mes, true);
        if (FF9StateSystem.Common.FF9.fldMapNo == 1850 && FF9StateSystem.AndroidTVPlatform && (mes == 147 || mes == 148))
            NGUIText.ForceShowButton = true; // Alexandria/Main Street, "Press [BTN] and [BTN] alternately!"

        Dialog dialog = null;
        if (Configuration.VoiceActing.Enabled) // Lambda expression captures the variable "dialog", taking the correct assignment into account
            dialog = Singleton<DialogManager>.Instance.AttachDialog(winID, windowStyle, mes, targetPo, choiceIndex => VoicePlayer.FieldZoneDialogClosed(dialog), captionType);
        else
            dialog = Singleton<DialogManager>.Instance.AttachDialog(winID, windowStyle, mes, targetPo, null, captionType);

        if (FF9StateSystem.Common.FF9.fldMapNo == 1657) // Iifa Tree/Tree Roots
        {
            switch (Localization.CurrentSymbol)
            {
                case "US":
                case "UK":
                case "ES":
                case "GR":
                case "IT":
                    dialog.FocusToActor = mes != 183 && mes != 166; // "H-Hey!" or "Whoa!" when Amarant passes next to Vivi/Garnet/Eiko on the narrow root
                    break;
                case "JP":
                    dialog.FocusToActor = mes != 187 && mes != 170;
                    break;
                case "FR":
                    dialog.FocusToActor = mes != 185 && mes != 168;
                    break;
            }
        }

        if (dialog == null)
            return;
        if ((flags & ETb.WindowNotFollowActor) > 0)
            dialog.FocusToActor = false;
        if (ETb.isMessageDebug)
            global::Debug.Log($"NewMesWin => sid:{instance.gCur.sid}, mes: {mes}, field id:{FF9TextTool.FieldZoneId}, winID: {winID}, flags: {flags}, text:{dialog.Phrase}");

        VoicePlayer.PlayFieldZoneDialogAudio(FF9TextTool.FieldZoneId, mes, dialog);

        ETb.gMesCount++;
        EIcon.SetHereIcon(0);
        String lang = Localization.CurrentSymbol;
        EMinigame.EidolonMuralAchievement(lang, mes);
        EMinigame.ExcellentLuckColorFortuneTellingAchievement(lang, mes);
        EMinigame.ProvokeMogAchievement(lang, mes);
        EMinigame.JumpingRopeAchievement(lang, mes);
        EMinigame.GetRewardFromQueenStellaAchievement();
        EMinigame.ShuffleGameAchievement(lang, mes);
        EMinigame.ChocoboBeakLV99Achievement(lang, mes);
        EMinigame.AtleteQueenAchievement_ByMessage(lang, mes);
        EMinigame.TreasureHunterSAchievement(lang, mes);
        ETb.FixChocoAccidenlyFly(dialog);
    }

    public static void FlagsToStyles(Int32 flags, out Dialog.WindowStyle winStyle, out Dialog.CaptionType captionType)
    {
        captionType = Dialog.CaptionType.None;
        if ((flags & ETb.WindowChatStyle) > 0)
        {
            winStyle = Dialog.WindowStyle.WindowStyleAuto;
        }
        else
        {
            winStyle = Dialog.WindowStyle.WindowStylePlain;
            if ((flags & ETb.winMOG) > 0)
                captionType = Dialog.CaptionType.Mognet;
            else if ((flags & ETb.winATE) > 0)
                captionType = Dialog.CaptionType.ActiveTimeEvent;
        }
        if ((flags & ETb.WindowTransparentStyle) > 0)
            winStyle = Dialog.WindowStyle.WindowStyleTransparent;
        else if ((flags & ETb.WindowChatStyleWithoutTail) > 0)
            winStyle = Dialog.WindowStyle.WindowStyleNoTail;
    }

    public static Int32 StylesToFlag(Dialog.WindowStyle winStyle, Dialog.CaptionType captionType)
    {
        Int32 flags = 0;
        if (winStyle == Dialog.WindowStyle.WindowStyleAuto)
            flags |= ETb.WindowChatStyle;
        else if (winStyle == Dialog.WindowStyle.WindowStyleTransparent)
            flags |= ETb.WindowTransparentStyle;
        else if (winStyle == Dialog.WindowStyle.WindowStyleNoTail)
            flags |= ETb.WindowChatStyleWithoutTail;
        if (captionType == Dialog.CaptionType.Mognet)
            flags |= ETb.winMOG;
        else if (captionType == Dialog.CaptionType.ActiveTimeEvent)
            flags |= ETb.winATE;
        return flags;
    }

    public static Boolean MesWinActive(Int32 num)
    {
        return Singleton<DialogManager>.Instance.CheckDialogShowing(num);
    }

    public static CharacterId GetPartyMember(Int32 index)
    {
        FF9StateGlobal ff = FF9StateSystem.Common.FF9;
        PLAYER player = ff.party.member[index];
        return (player == null) ? CharacterId.NONE : player.info.slot_no;
    }

    public static void YWindow_CloseAll(Boolean scriptedClose = false)
    {
        Singleton<DialogManager>.Instance.CloseAll(scriptedClose);
    }

    public static void DisposWindowByID(Int32 windowID, Boolean scriptedClose = false)
    {
        Singleton<DialogManager>.Instance.Close(windowID, scriptedClose);
    }

    public static void RaiseAllWindow()
    {
        Singleton<DialogManager>.Instance.RiseAll();
    }

    public static void SetMesValue(Int32 scriptID, Int32 value)
    {
        if (scriptID >= 0 && scriptID < 8)
            ETb.gMesValue[scriptID] = value;
    }

    public static String GetItemName(Int32 itemId)
    {
        if (ff9item.IsItemRegular(itemId))
            return FF9TextTool.ItemName(ff9item.GetRegularIdFromItemId(itemId));
        if (ff9item.IsItemImportant(itemId))
            return FF9TextTool.ImportantItemName(ff9item.GetImportantIdFromItemId(itemId));
        if (ff9item.IsItemCard(itemId))
            return FF9TextTool.CardName(ff9item.GetCardIdFromItemId(itemId));
        return String.Empty;
    }

    public static void SetChooseParam(Int32 availMask, Int32 initAbsoluteOptionIndex)
    {
        ETb.sChooseMask = availMask;
        Int32 initAvailOptionIndex = -1;
        while (initAbsoluteOptionIndex >= 0 && availMask > 0)
        {
            if ((availMask & 1) != 0)
                initAvailOptionIndex++;
            initAbsoluteOptionIndex--;
            availMask >>= 1;
        }
        ETb.sChooseInit = Math.Max(0, initAvailOptionIndex);
    }

    public static Int32 GetChoose()
    {
        ETb.sChoose = DialogManager.SelectChoice;
        if (ETb.isMessageDebug)
            global::Debug.Log("Event choice value:" + ETb.sChoose);
        return ETb.sChoose;
    }

    public static String GetStringFromTable(UInt32 bank, UInt32 index)
    {
        if (index < 8u)
        {
            String[] tableText = FF9TextTool.GetTableText(bank);
            if (tableText != null)
            {
                Int32 tableIndex = ETb.gMesValue[index];
                if (tableIndex < tableText.Length)
                    return tableText[tableIndex];
            }
        }
        return String.Empty;
    }

    private static Boolean IsSkipped(EventEngine eventEngine, Int32 mes, Int32 winID, Int32 flags, PosObj targetPo)
    {
        if (eventEngine.gMode == 1)
        {
            Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
            if (fldMapNo == 1652) // Iifa Tree/Roots
            {
                switch (Localization.CurrentSymbol)
                {
                    case "JP":
                        return mes == 146;
                    case "FR":
                        return mes == 144;
                }
                return mes == 142; // Zidane: "How far is it gonna go...?"
            }
            if (fldMapNo == 1659) // Iifa Tree/Seashore
            {
                if (targetPo.sid == 1) // Queen Brahne
                {
                    Dialog dialogByTextId = Singleton<DialogManager>.Instance.GetDialogByTextId(mes);
                    return dialogByTextId != null;
                }
            }
            else if (fldMapNo == 2209 && targetPo.sid == 9) // Palace/Sanctum, Zidane
            {
                return mes == 393; // "No!!!" (when chasing after Kuja)
            }
        }
        return false;
    }

    public static void World2Screen(Vector3 v, out Single x, out Single y)
    {
        FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
        BGCAM_DEF currentBgCamera = fieldmap.GetCurrentBgCamera();
        Vector3 vector = PSX.CalculateGTE_RTPT(v, Matrix4x4.identity, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), fieldmap.GetProjectionOffset());
        Vector2 cameraOffset = fieldmap.GetCameraOffset();
        Single screenX = vector.x - cameraOffset.x;
        Single screenY = vector.y - cameraOffset.y;
        ETb.ConvertGTEToUIScreenPosition(ref screenX, ref screenY);
        x = screenX;
        y = screenY;
    }

    public static void ConvertGTEToUIScreenPosition(ref Single x, ref Single y)
    {
        x = x * UIManager.ResourceXMultipier + UIManager.UIContentSize.x / 2f;
        y = y * UIManager.ResourceYMultipier + UIManager.UIContentSize.y / 2f;
    }

    public static void GetMesPos(PosObj po, out Single x, out Single y)
    {
        x = 0f;
        y = 0f;
        if (po.go == null)
            return;
        Vector3 worldMesPos = new Vector3(po.pos[0], po.pos[1], po.pos[2]);
        worldMesPos.y += ((-po.eye * po.scaley) >> 6) + 50f;
        if (po.cid == 4)
        {
            Actor actor = (Actor)po;
            worldMesPos.x += actor.mesofsX;
            worldMesPos.y -= actor.mesofsY;
            worldMesPos.z += actor.mesofsZ;
        }
        ETb.World2Screen(worldMesPos, out x, out y);
        y = UIManager.UIContentSize.y - y;
    }

    public static void SndMove()
    {
        if (RealTime.time - ETb.lastPlaySound < 0.01f)
            return;
        ETb.lastPlaySound = RealTime.time;
        FF9Sfx.FF9SFX_Play(103);
    }

    public static void SndCancel()
    {
        if (RealTime.time - ETb.lastPlaySound < 0.01f)
            return;
        ETb.lastPlaySound = RealTime.time;
        FF9Sfx.FF9SFX_Play(101);
    }

    public static void SndOK()
    {
        ETb.SndMove();
    }

    public static void SndConfirm(UIScene scene)
    {
        Type type = scene.GetType();
        if (type != typeof(FieldHUD) && type != typeof(WorldHUD))
            ETb.SndOK();
    }

    public static void SndCancel(UIScene scene)
    {
        Type type = scene.GetType();
        if (type != typeof(FieldHUD) && type != typeof(WorldHUD))
            ETb.SndCancel();
    }

    public static void ProcessDialog(Dialog dialog)
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (instance == null)
            return;
        if (instance.gMode == 3) // World map
        {
            if (dialog.TextId == 40 || dialog.TextId == 41) // "Enter with [BTN]"
                EIcon.HideDialogBubble();
            ETb.CheckVehicleTutorial(dialog);
        }
        else if (instance.gMode == 1 && FF9StateSystem.Common.FF9.fldMapNo == 1850 && FF9StateSystem.AndroidTVPlatform && (dialog.TextId == 147 || dialog.TextId == 148))
        {
            // Alexandria/Main Street, "Press [BTN] and [BTN] alternately!"
            NGUIText.ForceShowButton = false;
        }
    }

    public static void ProcessATEDialog(Dialog dialog)
    {
        if (dialog.CapType == Dialog.CaptionType.ActiveTimeEvent)
        {
            global::Debug.Log($"ProcessATBDialog: DialogManager.SelectChoice {DialogManager.SelectChoice}, numOfChoices {dialog.ChoiceNumber}");
            Boolean isCompulsory = ETb.LastATEDialogID == -1 && dialog.Id == 0;
            if (dialog.Id != 1 || DialogManager.SelectChoice != dialog.ChoiceNumber - 1 || dialog.ChoiceNumber <= 0)
            {
                if (dialog.Id != 0 || ETb.LastATEDialogID != 1)
                {
                    Int32 ateID = EMinigame.MappingATEID(dialog, DialogManager.SelectChoice, isCompulsory);
                    EMinigame.ATE80Achievement(ateID);
                    global::Debug.Log("ATEID = " + ateID);
                }
            }
            ETb.LastATEDialogID = dialog.Id;
            if (FF9StateSystem.Common.FF9.fldLocNo == 40 && FF9StateSystem.Common.FF9.fldMapNo == 206 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 1900)
            {
                if (DialogManager.SelectChoice == 1) // Prima Vista/Crash Site, trying to avoid the ATE
                    ETb.LastATEDialogID = -1;
            }
            else if (dialog.Id == 0)
            {
                ETb.LastATEDialogID = -1;
            }
        }
    }

    private static void CheckVehicleTutorial(Dialog dialog)
    {
        if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD && UIManager.World.CurrentState != WorldHUD.State.FullMap)
        {
            switch (dialog.TextId)
            {
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                    EventInput.IsDialogConfirm = true;
                    UIManager.World.ForceShowButton();
                    break;
            }
        }
    }

    private static void FixChocoAccidenlyFly(Dialog dialog)
    {
        if (PersistenSingleton<EventEngine>.Instance.gMode == 3 && EventCollision.IsChocoboWalkingInForestArea() && (dialog.TextId == 54 || dialog.TextId == 55 || dialog.TextId == 56 || dialog.TextId == 57 || dialog.TextId == 58 || dialog.TextId == 59 || dialog.TextId == 60 || dialog.TextId == 61 || dialog.TextId == 62))
            PersistenSingleton<UIManager>.Instance.Dialogs.EnableCollider(false); // "Kweh?" and the other messages displayed with the beak button
    }

    public const Int32 kMesValueN = 8;
    public const Int32 kMesOfsY = 50;
    public const Int32 kMesItem = 14;
    public const Int32 kMesValue = 64;
    public const Int32 kMesPlayer0 = 16;
    public const Int32 kMesPlayer1 = 17;
    public const Int32 kMesPlayer2 = 18;
    public const Int32 kMesPlayer3 = 19;
    public const Int32 kMesPlayer4 = 20;
    public const Int32 kMesPlayer5 = 21;
    public const Int32 kMesPlayer6 = 22;
    public const Int32 kMesPlayer7 = 23;
    public const Int32 kMesParty0 = 24;
    public const Int32 kMesParty1 = 25;
    public const Int32 kMesParty2 = 26;
    public const Int32 kMesParty3 = 27;
    public const Int32 kMesString = 6;
    public const Int32 kMesNew = 112;
    public const Int32 kStringTableN = 4;

    public const Int32 ResetChooseMask = 1;
    public const Int32 WindowChatStyleWithoutTail = 4;
    public const Int32 winMOG = 8;
    public const Int32 WindowTransparentStyle = 16;
    public const Int32 WindowNotFollowActor = 32;
    public const Int32 winATE = 64;
    public const Int32 WindowChatStyle = 128;
    public const Int32 EnterTownMesId = 40;
    public const Int32 EnterDungeonMesId = 41;

    private static readonly Boolean isMessageDebug;

    private static Boolean sInitMesInh;

    public static UInt32 sKey;
    public static UInt32 sKeyOn;
    public static UInt32 sKeyOff;

    public static Int32 gMesCount;
    public static Int32[] gMesValue;

    public static Int32 gMesSignal;
    public static Int32 sChoose;
    public static Int32 sChooseMask = -1;
    public static Int32 sChooseInit;

    private static Single lastPlaySound;
    private static Int32 LastATEDialogID = -1;
}
