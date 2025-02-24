using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(UIPanel))]
public class Dialog : MonoBehaviour
{
    public Dialog()
    {
        this.startChoiceRow = -1;
        this.selectedChoice = -1;
        this.chooseMask = -1;
        this.choiceList = new List<GameObject>();
        this.maskChoiceList = new List<GameObject>();
        this.disableIndexes = new List<Int32>();
        this.activeIndexes = new List<Int32>();
    }

    public Int32 StartChoiceRow
    {
        get => this.startChoiceRow;
        set => this.startChoiceRow = value;
    }

    public Int32 ChoiceNumber
    {
        get => this.choiceNumber;
        set => this.choiceNumber = value;
    }

    public Int32 DefaultChoice
    {
        get => this.defaultChoice;
        set => this.defaultChoice = value;
    }

    public Int32 CancelChoice
    {
        get => this.cancelChoice;
        set => this.cancelChoice = value;
    }

    public Int32 SelectChoice
    {
        get => this.selectedChoice;
        set
        {
            try
            {
                if (this.OnOptionChange != null && this.selectedChoice != value)
                    this.OnOptionChange(this.id, value);
            }
            catch (Exception e)
            {
                SoundLib.VALog(String.Format("Exception in onOptionChange: {0}", e.ToString()));
            }
            this.selectedChoice = value;
            DialogManager.SelectChoice = value;
        }
    }

    public GameObject ActiveChoice => ButtonGroupState.ActiveButton;
    public List<Int32> ActiveIndexes => this.activeIndexes;
    public List<Int32> DisableIndexes => this.disableIndexes;

    public Int32 ChooseMask => this.chooseMask;

    public Boolean IsChoiceReady => this.isChoiceReady;

    public Boolean IsClosedByScript
    {
        get => this.isClosedByScript;
        set => this.isClosedByScript = value;
    }

    private void InitializeChoice()
    {
        if (this.isNeedResetChoice)
        {
            this.SelectChoice = 0;
            ETb.sChooseInit = 0;
        }
        ETb.sChoose = this.SelectChoice;
        if (this.startChoiceRow > -1)
        {
            this.isMuteSelectSound = true;
            base.StartCoroutine("InitializeChoiceProcess");
        }
    }

    private IEnumerator InitializeChoiceProcess()
    {
        yield return new WaitForEndOfFrame();
        while (PersistenSingleton<UIManager>.Instance.IsPause)
            yield return new WaitForEndOfFrame();
        PersistenSingleton<UIManager>.Instance.Dialogs.ShowChoiceHud();
        Single startYPos = this.phraseLabel.transform.localPosition.y;
        Int32 totalLine = this.CurrentParser.LineInfo.Count + this.disableIndexes.Count();
        Int32 lineControl = this.startChoiceRow;
        Single choiceCursorX = 0f;
        if (!IsETbDialog)
        {
            // UI dialogs with choices: choices are centered
            for (Int32 line = this.startChoiceRow; line < this.CurrentParser.LineInfo.Count; line++)
                choiceCursorX = Math.Max(choiceCursorX, this.CurrentParser.LineInfo[line].Width);
            choiceCursorX = (this.phraseLabel.width - choiceCursorX) / 2f;
        }
        for (Int32 line = this.startChoiceRow; line < totalLine; line++)
        {
            if (this.disableIndexes.Contains(line - this.startChoiceRow))
            {
                this.choiceList.Add(null);
                continue;
            }
            GameObject choice = NGUITools.AddChild(this.ChooseContainerGameObject, this.DialogChoicePrefab);
            UIWidget choiceWidget = choice.GetComponent<UIWidget>();
            UIKeyNavigation choiceKeyNav = choice.GetComponent<UIKeyNavigation>();
            choice.name = "Choice#" + line;
            choice.transform.parent = this.ChooseContainerGameObject.transform;
            choice.transform.position = this.PhraseGameObject.transform.position;
            Rect lineRect = this.CurrentParser.GetLineRenderRect(lineControl);
            Vector3 localPos = new Vector3(choiceCursorX, startYPos + (lineRect.yMin + lineRect.yMax) / 2f, 0f);
            choice.transform.localPosition = localPos;
            choiceWidget.width = Mathf.RoundToInt(this.phraseLabel.width - choiceCursorX); // [DBG] to check
            choiceWidget.height = (Int32)Dialog.DialogLineHeight;
            if (line - this.startChoiceRow == this.defaultChoice)
                choiceKeyNav.startsSelected = true;
            this.maskChoiceList.Add(choice);
            this.choiceList.Add(choice);
            lineControl++;
        }
        NGUIExtension.SetKeyNevigation(this.maskChoiceList);
        ButtonGroupState.RemoveCursorMemorize(Dialog.DialogGroupButton);
        ButtonGroupState.SetPointerDepthToGroup(this.phrasePanel.depth + 1, Dialog.DialogGroupButton);
        ButtonGroupState.UpdatePointerPropertyForGroup(Dialog.DialogGroupButton);
        ButtonGroupState.ActiveGroup = Dialog.DialogGroupButton;
        this.isMuteSelectSound = true;
        this.SetCurrentChoice(this.defaultChoice);
        yield return new WaitForEndOfFrame();
        this.isChoiceReady = true;
        yield break;
    }

    public void ResetChoose()
    {
        foreach (GameObject obj in this.maskChoiceList)
            UnityEngine.Object.DestroyObject(obj);
        this.StartChoiceRow = -1;
        this.choiceNumber = 0;
        this.defaultChoice = 0;
        this.cancelChoice = 0;
        this.activeIndexes.Clear();
        this.disableIndexes.Clear();
        this.choiceList.Clear();
        this.maskChoiceList.Clear();
    }

    public void SetCurrentChoice(Int32 choice)
    {
        this.SelectChoice = choice;
        ButtonGroupState.ActiveButton = this.choiceList[choice];
    }

    public void MoveCurrentChoice(Int32 delta)
    {
        Int32 selectChoice = this.SelectChoice;
        Int32 choiceIndexMasked = this.maskChoiceList.IndexOf(this.choiceList[this.SelectChoice]);
        Int32 choiceIndexAbsolute = choiceIndexMasked + delta;
        choiceIndexAbsolute = Mathf.Clamp(choiceIndexAbsolute, 0, this.maskChoiceList.Count - 1);
        Int32 choiceIndexUnmasked = this.choiceList.IndexOf(this.maskChoiceList[choiceIndexAbsolute]);
        if (selectChoice != choiceIndexUnmasked)
            this.SetCurrentChoice(choiceIndexUnmasked);
    }

    public void SetupChooseMask(Int32 mask, Int32 choiceExactNumber)
    {
        this.chooseMask = mask;
        this.activeIndexes.Clear();
        this.disableIndexes.Clear();
        if (mask == -1)
            return;
        for (Int32 i = 0; i < choiceExactNumber; i++)
        {
            if (i < this.choiceNumber && (mask & 1) == 0)
                this.disableIndexes.Add(i);
            else
                this.activeIndexes.Add(i);
            mask >>= 1;
        }
    }

    public Int32 Id
    {
        get => this.id;
        set => this.id = value;
    }

    public DialogAnimator DialogAnimate => this.dialogAnimator;

    public Dialog.TailPosition Tail
    {
        get => this.tailPosition;
        set => this.setTailPosition(value);
    }

    public Single TailMargin => this.tailMargin;

    public Dialog.WindowStyle Style
    {
        get => this.windowStyle;
        set
        {
            this.windowStyle = value;
            switch (value)
            {
                case Dialog.WindowStyle.WindowStyleAuto:
                case Dialog.WindowStyle.WindowStyleNoTail:
                    this.borderSprite.spriteName = "dialog_frame_chat";
                    break;
                case Dialog.WindowStyle.WindowStylePlain:
                    this.borderSprite.spriteName = "dialog_frame_info";
                    break;
                case Dialog.WindowStyle.WindowStyleTransparent:
                    this.borderSprite.spriteName = String.Empty;
                    break;
            }
        }
    }

    public Vector2 Size => this.size;
    public Vector2 ClipSize => new Vector2(this.size.x + Dialog.DialogXPadding * 2f, this.size.y + Dialog.DialogYPadding);

    public Vector2 Position
    {
        get => this.position;
        set => this.position = value * UIManager.ResourceYMultipier;
    }

    public Vector3 OffsetPosition
    {
        get => this.offset;
        set => this.offset = value;
    }

    public String Phrase
    {
        get => this.phrase;
        set
        {
            this.PageParsers.Clear();
            this.subPage = DialogBoxSymbols.ParseTextSplitTags(value);
            for (Int32 i = 0; i < this.subPage.Count; i++)
            {
                this.subPage[i] = NGUIText.FF9WhiteColor + this.subPage[i];
                this.PageParsers.Add(new TextParser(this.phraseLabel, this.subPage[i]));
            }
            this.PrepareNextPage();
        }
    }

    public String Caption
    {
        get => this.caption;
        set
        {
            if (this.captionLabel.rawText != value && !String.IsNullOrEmpty(value))
            {
                this.caption = value;
                this.captionLabel.rawText = value;
                this.captionLabel.ProcessText();
                this.captionWidth = this.captionLabel.Parser.MaxWidth;
            }
        }
    }

    public Dialog.CaptionType CapType
    {
        get => this.capType;
        set => this.capType = value;
    }

    public Single CaptionWidth => this.captionWidth;

    public Single WidthHint { get; set; }
    public Single LineNumberHint { get; set; }
    public Single HeightHint => LineNumberHint * Dialog.DialogLineHeight;
    public Vector2 SizeHint => new Vector2(WidthHint, HeightHint);

    public Single OriginalWidth => this.originalWidth;

    public Single Width
    {
        get => this.size.x;
        set
        {
            this.originalWidth = value;
            value *= UIManager.ResourceXMultipier;
            this.size.x = Mathf.Max(value, Dialog.WindowMinWidth - Dialog.AdjustWidth);
            this.bodySprite.width = (Int32)this.size.x;
            this.phraseWidget.width = (Int32)this.size.x - (Int32)(Dialog.DialogPhraseXPadding * 2f);
            this.phraseWidget.transform.localPosition = new Vector3(-this.phraseWidget.width / 2f, this.phraseWidget.transform.localPosition.y, this.phraseWidget.transform.localPosition.z);
            this.clipPanel.baseClipRegion = new Vector4(this.clipPanel.baseClipRegion.x, this.clipPanel.baseClipRegion.y, this.ClipSize.x, this.ClipSize.y);
        }
    }

    public Single LineNumber
    {
        get => this.lineNumber;
        set
        {
            this.lineNumber = (Int32)value;
            this.size.y = value * Dialog.DialogLineHeight + Dialog.DialogPhraseYPadding;
            this.bodySprite.height = (Int32)this.size.y;
            this.phraseWidget.height = (Int32)(this.size.y - Dialog.DialogPhraseYPadding);
            this.phraseWidget.transform.localPosition = new Vector3(this.phraseWidget.transform.localPosition.x, this.phraseWidget.height / 2f, this.phraseWidget.transform.localPosition.z);
            this.clipPanel.baseClipRegion = new Vector4(this.clipPanel.baseClipRegion.x, this.clipPanel.baseClipRegion.y, this.ClipSize.x, this.ClipSize.y);
        }
    }

    public Int32 EndMode
    {
        get => this.endMode;
        set => this.endMode = value;
    }

    /// <summary>This is about the text appearing progressively</summary>
    public Boolean TypeEffect
    {
        get => this.typeAnimationEffect;
        set => this.typeAnimationEffect = value;
    }

    public Boolean FlagButtonInh
    {
        get => this.ignoreInputFlag;
        set => this.ignoreInputFlag = value;
    }

    public Boolean FlagResetChoice
    {
        get => this.isNeedResetChoice;
        set => this.isNeedResetChoice = value;
    }

    public PosObj Po
    {
        get => this.targetPos;
        set
        {
            this.targetPos = value;
            if (this.targetPos != null)
            {
                this.sid = this.targetPos.sid;
                this.followObject = this.targetPos.go;
            }
            else
            {
                this.sid = -1;
                this.followObject = null;
            }
        }
    }

    public Vector2 FF9Position
    {
        get => this.ff9Position;
        set => this.ff9Position = value;
    }

    public UIPanel Panel
    {
        get
        {
            if (this.panel == null)
                this.panel = base.GetComponent<UIPanel>();
            return this.panel;
        }
    }

    public Boolean IsActive => this.isActive;

    public Boolean FocusToActor
    {
        get => this.focusToActor;
        set => this.focusToActor = value;
    }

    public Boolean IsReadyToFollow => this.isReadyToFollow;

    public Dialog.State CurrentState
    {
        get => this.currentState;
        set => this.currentState = value;
    }

    public UILabel PhraseLabel => this.phraseLabel;

    public Int32 TextId
    {
        get => this.textId;
        set => this.textId = value;
    }

    public Dictionary<Int32, Int32> MessageValues => this.messageValues;
    public Boolean MessageNeedUpdate
    {
        get => this.messageNeedUpdate;
        set => this.messageNeedUpdate = value;
    }

    public List<String> SubPage => this.subPage;
    public Int32 CurrentPage => this.currentPage;

    public Single DialogShowTime => this.dialogShowTime;
    public Single DialogHideTime => this.dialogHideTime;

    public Boolean IsOverlayDialog
    {
        get
        {
            if (this.isOverlayChecked)
                return this.isOverlayDialog;
            EventEngine instance = PersistenSingleton<EventEngine>.Instance;
            if (instance == null)
                return this.isOverlayDialog;
            if (instance.gMode == 1)
            {
                if (FF9TextTool.FieldZoneId == 23) // Mist Gates, buying potions
                {
                    String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
                    switch (currentLanguage)
                    {
                        case "Japanese":
                        case "French":
                            this.isOverlayDialog = this.textId == 154 || this.textId == 155;
                            break;
                        case "Italian":
                            this.isOverlayDialog = this.textId == 149 || this.textId == 150;
                            break;
                        default:
                            this.isOverlayDialog = this.textId == 134 || this.textId == 135;
                            break;
                    }
                }
                else if (FF9TextTool.FieldZoneId == 70 || FF9TextTool.FieldZoneId == 741) // Treno, bidding in Auction House
                {
                    String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
                    switch (currentLanguage)
                    {
                        case "English(US)":
                        case "English(UK)":
                            this.isOverlayDialog = this.textId == 204 || this.textId == 205 || this.textId == 206;
                            break;
                        default:
                            this.isOverlayDialog = this.textId == 205 || this.textId == 206 || this.textId == 207;
                            break;
                    }
                }
                else if (FF9TextTool.FieldZoneId == 166) // Daguerreo, transforming ores into aquamarine
                {
                    this.isOverlayDialog = this.textId == 106 || this.textId == 107;
                }
                else if (FF9TextTool.FieldZoneId == 358) // Madain Sari, choosing for how many people to cook
                {
                    String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
                    switch (currentLanguage)
                    {
                        case "Japanese":
                        case "French":
                            this.isOverlayDialog = this.textId == 874 || this.textId == 875;
                            break;
                        case "Spanish":
                            this.isOverlayDialog = this.textId == 859 || this.textId == 860;
                            break;
                        case "German":
                            this.isOverlayDialog = this.textId == 875 || this.textId == 876;
                            break;
                        case "Italian":
                            this.isOverlayDialog = this.textId == 889 || this.textId == 890;
                            break;
                        default:
                            this.isOverlayDialog = this.textId == 861 || this.textId == 862;
                            break;
                    }
                }
                else if (FF9TextTool.FieldZoneId == 945) // Chocobo Places, buying Gysahl Greens
                {
                    String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
                    if (currentLanguage == "Japanese")
                        this.isOverlayDialog = this.textId == 251 || this.textId == 252;
                    else
                        this.isOverlayDialog = this.textId == 252 || this.textId == 253;
                }
            }
            if (this.isOverlayDialog)
            {
                Vector3 localPosition = base.transform.localPosition;
                localPosition.x = 10000f;
                base.transform.localPosition = localPosition;
            }
            this.isOverlayChecked = true;
            return this.isOverlayDialog;
        }
    }

    public Boolean IsETbDialog { get; set; }

    public void Awake()
    {
        this.bodyTransform = this.BodyGameObject.transform;
        this.tailTransform = this.TailGameObject.transform;
        this.clipPanel = base.gameObject.GetComponent<UIPanel>();
        this.phraseWidget = this.PhraseGameObject.GetComponent<UIWidget>();
        this.phraseLabel = this.PhraseGameObject.GetComponent<UILabel>();
        this.phraseEffect = this.PhraseGameObject.GetComponent<TypewriterEffect>();
        this.captionLabel = this.CaptionGameObject.GetComponent<UILabel>();
        this.bodySprite = this.BodyGameObject.GetComponent<UISprite>();
        this.borderSprite = this.BorderGameObject.GetComponent<UISprite>();
        this.tailSprite = this.TailGameObject.GetComponent<UISprite>();
        this.dialogAnimator = base.gameObject.GetComponent<DialogAnimator>();
        this.phraseWidgetDefault = this.phraseWidget.pivot;
        this.phraseEffect.enabled = true;
    }

    public void Show()
    {
        this.OverwriteDialogParameter();
        this.dialogShowTime = RealTime.time;
        this.AutomaticSize();
        this.InitializeDialogTransition();
        this.InitializeWindowType();
        this.currentState = Dialog.State.OpenAnimation;
        this.dialogAnimator.ShowDialog();
        PersistenSingleton<UIManager>.Instance.Dialogs.CurMesId = this.textId;
    }

    public void Hide()
    {
        this.dialogHideTime = RealTime.time;
        base.StopAllCoroutines();
        this.phraseLabel.ReleaseAllIcons();
        this.captionLabel.ReleaseAllIcons();
        if (this.subPage.Count > this.currentPage)
        {
            this.PrepareNextPage();
            this.currentState = Dialog.State.OpenAnimation;
            this.dialogAnimator.ShowNewPage();
            VoicePlayer.PlayFieldZoneDialogAudio(FF9TextTool.FieldZoneId, this.textId, this);
            return;
        }
        this.isActive = false;
        if (this.startChoiceRow > -1)
        {
            ButtonGroupState.DisableAllGroup(true);
            PersistenSingleton<UIManager>.Instance.Dialogs.HideChoiceHud();
            ETb.SndOK();
        }
        if (this.windowStyle == Dialog.WindowStyle.WindowStyleTransparent || this.dialogAnimator.ShowWithoutAnimation)
        {
            this.AfterHidden();
            return;
        }
        this.currentState = Dialog.State.CloseAnimation;
        this.dialogAnimator.HideDialog();
        if (this.CapType == Dialog.CaptionType.Mognet && this.StartChoiceRow > -1)
            UIManager.Input.ResetTriggerEvent();
    }

    public void AfterShown()
    {
        this.InitializeChoice();
        if (!this.typeAnimationEffect)
        {
            this.CurrentParser.AdvanceProgress(this.CurrentParser.AppearProgressMax);
            this.currentState = Dialog.State.CompleteAnimation;
            if (base.gameObject.activeInHierarchy && this.endMode > 0)
                base.StartCoroutine("AutoHide");
        }
        if (this.AfterDialogShown != null)
            this.AfterDialogShown(this.id);
        if (this.targetPos != null)
            this.isReadyToFollow = true;
    }

    public void AfterSentenseShown()
    {
        if (this.currentState != Dialog.State.TextAnimation)
            return;
        this.currentState = Dialog.State.CompleteAnimation;
        UIDebugMarker.DebugLog($"AfterSentenseShown Id:{this.Id} Animation State:{this.currentState}");
        if (this.endMode > 0 && base.gameObject.activeInHierarchy)
            base.StartCoroutine("AutoHide");
        if (this.AfterDialogSentenseShown != null)
            this.AfterDialogSentenseShown();
    }

    public void AfterHidden()
    {
        EventHUD.CheckSpecialHUDFromMesId(this.textId, false);
        ETb.ProcessDialog(this);
        ETb.ProcessATEDialog(this);
        Singleton<DialogManager>.Instance.ReleaseDialogToPool(this);
        if (this.AfterDialogHidden != null)
        {
            this.AfterDialogHidden(this.startChoiceRow > -1 ? this.SelectChoice : -1);
            this.AfterDialogHidden = null;
        }
        this.Reset();
    }

    public void OnKeyConfirm(GameObject go)
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 2951 || FF9StateSystem.Common.FF9.fldMapNo == 2952)
        {
            // Chocobo's Lagoon & Air Garden
            String symbol = Localization.GetSymbol();
            if (symbol == "JP" && Singleton<DialogManager>.Instance.PressMesId == 245 && Singleton<DialogManager>.Instance.ReleaseMesId == 226)
                return;
            if (Singleton<DialogManager>.Instance.PressMesId == 246 && Singleton<DialogManager>.Instance.ReleaseMesId == 227)
                return;
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 2950)
        {
            // Chocobo's Forest
            String symbol = Localization.GetSymbol();
            if (symbol == "JP" && Singleton<DialogManager>.Instance.PressMesId == 245 && Singleton<DialogManager>.Instance.ReleaseMesId == 225)
                return;
            if (Singleton<DialogManager>.Instance.PressMesId == 246 && Singleton<DialogManager>.Instance.ReleaseMesId == 226)
                return;
        }
        if (this.currentState == Dialog.State.CompleteAnimation)
        {
            if (this.startChoiceRow > -1)
                this.SelectChoice = this.choiceList.IndexOf(ButtonGroupState.ActiveButton);
            if (!this.ignoreInputFlag && (this.startChoiceRow < 0 || this.isChoiceReady))
                this.Hide();
        }
        else if (this.startChoiceRow > -1 && this.defaultChoice > -1 && (this.currentState == Dialog.State.OpenAnimation || this.currentState == Dialog.State.TextAnimation))
        {
            // Fix fast player inputs not applying the correct cancel choice for windows that are closed by scripts (eg. Memoria save points or World Map mog dialogs)
            this.SelectChoice = this.defaultChoice;
        }
        if (this.currentState == Dialog.State.TextAnimation && this.typeAnimationEffect)
        {
            this.CurrentParser.AdvanceProgress(this.CurrentParser.AppearProgressMax);
            this.AfterSentenseShown();
        }
    }

    public void OnKeyCancel(GameObject go)
    {
        if (this.startChoiceRow > -1 && this.cancelChoice > -1)
        {
            if (this.currentState == Dialog.State.CompleteAnimation)
            {
                this.isMuteSelectSound = true;
                this.SetCurrentChoice(this.cancelChoice);
                ETb.SndCancel();
            }
            else if (this.currentState == Dialog.State.OpenAnimation || this.currentState == Dialog.State.TextAnimation)
            {
                // Fix fast player inputs not applying the correct cancel choice for windows that are closed by scripts (eg. Memoria save points or World Map mog dialogs)
                this.SelectChoice = this.cancelChoice;
            }
        }
    }

    public void OnItemSelect(GameObject go)
    {
        if (this.currentState == Dialog.State.CompleteAnimation)
        {
            this.SelectChoice = this.choiceList.IndexOf(go);
            if (!this.isMuteSelectSound)
                ETb.SndMove();
            else
                this.isMuteSelectSound = false;
        }
    }

    private IEnumerator AutoHide()
    {
        Single waitTime = (Single)this.endMode / Configuration.Graphics.FieldTPS;
        if (FF9StateSystem.Common.FF9.fldMapNo == 3009)
        {
            // Epilogue: Stage
            DialogManager.Instance.ForceControlByEvent(false);
            yield break;
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 3010)
        {
            // Epilogue: Stage
            DialogManager.Instance.ForceControlByEvent(false);
            yield break;
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 3011)
        {
            // Epilogue: Stage
            DialogManager.Instance.ForceControlByEvent(true);
            //yield break; // Leads to problems
            String lang = Localization.GetSymbol();
            waitTime += lang != "US" && lang != "JP" ? 0.56f : -0.22f;
        }
        while (waitTime > 0f || VoicePlayer.HasDialogVoice(this))
        {
            waitTime -= HonoBehaviorSystem.Instance.IsFastForwardModeActive() ? Time.deltaTime * HonoBehaviorSystem.Instance.GetFastForwardFactor() : Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        this.Hide();
        yield break;
    }

    private void InitializeWindowType()
    {
        UIAtlas windowAtlas = FF9UIDataTool.WindowAtlas;
        this.bodySprite.atlas = windowAtlas;
        this.borderSprite.atlas = windowAtlas;
        this.tailSprite.atlas = windowAtlas;
    }

    private void InitializeDialogTransition()
    {
        this.currentState = Dialog.State.Initial;
        Single posX = this.position.x;
        Single posY = this.position.y;
        Boolean isTailUpper;
        if (this.id == 9)
        {
            this.Panel.depth = Dialog.DialogMaximumDepth + Dialog.DialogAdditionalRaiseDepth + 2;
            this.phrasePanel.depth = this.Panel.depth + 1;
        }
        else if (this.id != -1)
        {
            this.Panel.depth = Dialog.DialogMaximumDepth - this.id * 2;
            this.phrasePanel.depth = this.Panel.depth + 1;
        }
        if (this.position != Vector2.zero)
            this.Po = null;
        if (this.IsAutoPositionMode())
        {
            this.isForceTailPosition = this.ThisDialogContainsForceTailPosition(this.tailPosition);
            if (this.Po.cid == 4)
            {
                Actor actor = (Actor)this.Po;
                actor.mesofsX = Convert.ToInt16(this.offset.x);
                actor.mesofsY = Convert.ToInt16(this.offset.y);
                actor.mesofsZ = Convert.ToInt16(this.offset.z);
            }
            ETb.GetMesPos(this.Po, out Single poPosX, out Single poPosY);
            posX = poPosX;
            posY = poPosY;
            Boolean isTailLeft;
            if (this.tailPosition == Dialog.TailPosition.AutoPosition)
            {
                EventEngine instance = PersistenSingleton<EventEngine>.Instance;
                Obj importantObj = instance.FindObjByUID(this.Po.uid != instance.GetControlUID() ? instance.GetControlUID() : ((Actor)this.Po).listener);
                PosObj importantPosObj = null;
                if (importantObj != null)
                    importantPosObj = instance.isPosObj(importantObj) ? (PosObj)importantObj : null;
                if (importantPosObj != null)
                {
                    ETb.GetMesPos(importantPosObj, out Single impPoPosX, out Single impPoPosY);
                    isTailUpper = poPosY < impPoPosY || this.ForceUpperTail();
                    isTailLeft = poPosX < impPoPosX;
                }
                else
                {
                    isTailUpper = true;
                    isTailLeft = false;
                }
            }
            else
            {
                isTailUpper = ((TailPosition)(((Int32)this.tailPosition) >> 1) & Dialog.TailPosition.LowerLeft) == Dialog.TailPosition.LowerLeft;
                isTailLeft = (this.tailPosition & Dialog.TailPosition.LowerLeft) == Dialog.TailPosition.LowerLeft;
            }
            if (!this.isForceTailPosition)
            {
                Single leftLimitX = (Single)Dialog.DialogLimitLeft + Dialog.InitialMagicNum;
                Single rightLimitX = (Single)Dialog.DialogLimitRight - Dialog.InitialMagicNum;
                isTailLeft ^= isTailLeft ? (posX < leftLimitX) : (posX > rightLimitX);
            }
            posX += Dialog.PosXOffset;
            posY += Dialog.PosYOffset;
            posX = this.setPositionX(posX, this.size.x, isTailLeft, false);
            if (this.isForceTailPosition)
                posY = this.forceSetPositionY(posY, this.size.y, isTailUpper);
            else
                posY = this.setPositionY(posY, this.size.y, ref isTailUpper);
            this.ff9Position = new Vector2(posX, posY);
            if (!this.isForceTailPosition && Singleton<DialogManager>.Instance.CheckDialogOverlap(this))
            {
                isTailUpper ^= true;
                posY = this.setPositionY(poPosY, this.size.y, ref isTailUpper);
                this.ff9Position = new Vector2(posX, posY);
            }
            Dialog.CalculateDialogCenter(ref posX, ref posY, this.ClipSize);
            this.tailMargin -= posX;
            this.tailPosition = (Dialog.TailPosition)(Convert.ToInt32(isTailUpper) << 1 | Convert.ToInt32(isTailLeft));
            this.HideUnusedSprite();
            posY = this.CalculateYPositionAfterHideTail(posY, isTailUpper);
            this.setAutoPosition(posX, posY);
            this.setTailAutoPosition(this.tailPosition, false);
            this.isActive = true;
        }
        else
        {
            if (this.windowStyle == Dialog.WindowStyle.WindowStyleAuto)
                this.windowStyle = Dialog.WindowStyle.WindowStylePlain;
            if (posX == 0f && posY == 0f)
            {
                switch (this.tailPosition)
                {
                    case Dialog.TailPosition.LowerRight:
                        posX = Dialog.DialogLimitRight - Dialog.kMargin - this.size.x / 2f;
                        posY = Dialog.DialogLimitTop + Dialog.kMargin + this.size.y / 2f;
                        break;
                    case Dialog.TailPosition.LowerLeft:
                        posX = Dialog.DialogLimitLeft + Dialog.kMargin + this.size.x / 2f;
                        posY = Dialog.DialogLimitTop + Dialog.kMargin + this.size.y / 2f;
                        break;
                    case Dialog.TailPosition.UpperRight:
                        posX = Dialog.DialogLimitRight - Dialog.kMargin - this.size.x / 2f;
                        posY = Dialog.DialogLimitBottom - Dialog.kMargin - this.size.y / 2f;
                        break;
                    case Dialog.TailPosition.UpperLeft:
                        posX = Dialog.DialogLimitLeft + Dialog.kMargin + this.size.x / 2f;
                        posY = Dialog.DialogLimitBottom - Dialog.kMargin - this.size.y / 2f;
                        if (PersistenSingleton<UIManager>.Instance.IsPauseControlEnable && PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.FieldHUD)
                            posX += UIManager.Field.PauseWidth;
                        break;
                    case Dialog.TailPosition.LowerCenter:
                        posX = Dialog.kCenterX;
                        posY = Dialog.DialogLimitTop + Dialog.kMargin + this.size.y / 2f;
                        break;
                    case Dialog.TailPosition.UpperCenter:
                        posX = Dialog.kCenterX;
                        posY = Dialog.DialogLimitBottom - Dialog.kMargin - this.size.y / 2f;
                        break;
                    case Dialog.TailPosition.DialogPosition:
                        posX = Dialog.kCenterX;
                        posY = Dialog.kDialogY;
                        this.tailPosition = Dialog.TailPosition.Center;
                        break;
                    default:
                        posX = Dialog.kCenterX;
                        posY = Dialog.kCenterY;
                        this.tailPosition = Dialog.TailPosition.Center;
                        break;
                }
                Single x = posX - this.size.x / 2f;
                Single y = UIManager.UIContentSize.y - posY - this.size.y / 2f;
                this.ff9Position = new Vector2(x, y);
                if (this.windowStyle == Dialog.WindowStyle.WindowStylePlain && this.capType != Dialog.CaptionType.None)
                    this.tailPosition = Dialog.TailPosition.Center;
            }
            else
            {
                this.ff9Position = new Vector2(posX, posY);
                posX += this.size.x / 2f;
                posY = UIManager.UIContentSize.y - posY - this.size.y / 2f;
            }
            this.HideUnusedSprite();
            this.setAutoPosition(posX, posY);
            this.setTailAutoPosition(this.tailPosition, false);
            this.isActive = true;
        }
    }

    private void HideUnusedSprite()
    {
        switch (this.windowStyle)
        {
            case Dialog.WindowStyle.WindowStylePlain:
            case Dialog.WindowStyle.WindowStyleTransparent:
            case Dialog.WindowStyle.WindowStyleNoTail:
                this.tailSprite.alpha = 0f;
                if (this.windowStyle == Dialog.WindowStyle.WindowStyleTransparent)
                {
                    this.bodySprite.alpha = 0f;
                    this.borderSprite.alpha = 0f;
                }
                break;
        }
    }

    private Single CalculateYPositionAfterHideTail(Single currentY, Boolean isTailUpper)
    {
        if (this.windowStyle == Dialog.WindowStyle.WindowStyleTransparent || this.windowStyle == Dialog.WindowStyle.WindowStyleNoTail)
            currentY += isTailUpper ? -this.tailSprite.height : this.tailSprite.height;
        return currentY;
    }

    private void FollowTarget()
    {
        ETb.GetMesPos(this.Po, out Single poPosX, out Single poPosY);
        Single posX = poPosX + Dialog.PosXOffset;
        Single posY = poPosY + Dialog.PosYOffset;
        posX = this.setPositionX(posX, this.size.x, (this.tailPosition & TailPosition.LowerLeft) != 0, true);
        posY = this.forceSetPositionY(posY, this.size.y, (this.tailPosition & TailPosition.UpperRight) != 0);
        Dialog.CalculateDialogCenter(ref posX, ref posY, this.ClipSize);
        this.tailMargin -= posX;
        this.tailPosition &= TailPosition.LowerLeft | TailPosition.UpperRight;
        posY = this.CalculateYPositionAfterHideTail(posY, (this.tailPosition & TailPosition.UpperRight) != 0);
        this.setAutoPosition(posX, posY);
        this.setTailAutoPosition(this.tailPosition, true);
    }

    private Boolean IsAutoPositionMode()
    {
        return this.targetPos != null && this.targetPos.go != null && this.windowStyle != Dialog.WindowStyle.WindowStylePlain;
    }

    private static void CalculateDialogCenter(ref Single x0, ref Single y0, Vector2 ClipSize)
    {
        x0 += ClipSize.x / 2f;
        y0 += ClipSize.y / 2f;
        y0 = UIManager.UIContentSize.y - y0;
    }

    private Single setPositionX(Single posX, Single width, Boolean isLeft, Boolean isUpdate)
    {
        Single tailX;
        Single minX;
        Single maxX;
        if (isUpdate)
        {
            if (isLeft && posX - Dialog.DialogTailLeftRightOffset <= Dialog.DialogLimitLeft + Dialog.TailMagicNumber1)
            {
                isLeft = false;
                this.tailPosition = this.tailPosition & TailPosition.UpperRight;
                this.setTailPosition(this.tailPosition);
            }
            else if (!isLeft && posX + Dialog.DialogTailLeftRightOffset >= Dialog.DialogLimitRight - Dialog.TailMagicNumber1)
            {
                isLeft = true;
                this.tailPosition = (this.tailPosition & TailPosition.UpperRight) | TailPosition.LowerLeft;
                this.setTailPosition(this.tailPosition);
            }
        }
        if (isLeft)
        {
            tailX = Mathf.Clamp(posX - Dialog.DialogTailLeftRightOffset, Dialog.DialogLimitLeft + Dialog.TailMagicNumber1, Dialog.DialogLimitRight - Dialog.TailMagicNumber2);
            minX = tailX + Dialog.TailMagicNumber2 - width;
            maxX = tailX - Dialog.TailMagicNumber1;
        }
        else
        {
            tailX = Mathf.Clamp(posX + Dialog.DialogTailLeftRightOffset, Dialog.DialogLimitLeft + Dialog.TailMagicNumber2, Dialog.DialogLimitRight - Dialog.TailMagicNumber1);
            minX = tailX + Dialog.TailMagicNumber1 - width;
            maxX = tailX - Dialog.TailMagicNumber2;
        }
        this.tailMargin = tailX;
        minX = Math.Max(minX, Dialog.DialogLimitLeft);
        maxX = Math.Min(maxX, Dialog.DialogLimitRight - width);
        return (minX + maxX) / 2f;
    }

    private Single setPositionY(Single posY, Single height, ref Boolean isUpper)
    {
        Single newPosY;
        if (isUpper)
        {
            newPosY = posY - height - Dialog.kUpperOffset;
            if (!this.isForceTailPosition && newPosY < Dialog.kLimitTop)
            {
                isUpper ^= true;
                newPosY = posY + Dialog.kLowerOffset;
            }
        }
        else
        {
            newPosY = posY + Dialog.kLowerOffset;
            if (!this.isForceTailPosition && newPosY > Dialog.kLimitBottom - height)
            {
                isUpper ^= true;
                newPosY = posY - height - Dialog.kUpperOffset;
            }
        }
        newPosY = Mathf.Clamp(newPosY, Dialog.kLimitTop, Dialog.kLimitBottom - height);
        return newPosY;
    }

    private Single forceSetPositionY(Single posY, Single height, Boolean isUpper)
    {
        posY = isUpper ? posY - this.size.y - Dialog.kUpperOffset : posY + Dialog.kLowerOffset;
        posY = Mathf.Clamp(posY, Dialog.kLimitTop, Dialog.kLimitBottom - this.size.y);
        return posY;
    }

    private void setAutoPosition(Single x, Single y)
    {
        base.gameObject.transform.localPosition = new Vector3(x - UIManager.UIContentSize.x / 2f, y - UIManager.UIContentSize.y / 2f);
    }

    public void Reset()
    {
        this.bodySprite.pivot = UIWidget.Pivot.Center;
        this.borderSprite.pivot = UIWidget.Pivot.Center;
        this.tailSprite.pivot = UIWidget.Pivot.Center;
        this.BodyGameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        this.BorderGameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        this.TailGameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        this.Po = null;
        this.isReadyToFollow = false;
        this.isForceTailPosition = false;
        this.focusToActor = true;
        this.isActive = false;
        this.ff9Position = Vector2.zero;
        this.bodySprite.alpha = 1f;
        this.bodySprite.width = 0;
        this.bodySprite.height = 0;
        this.borderSprite.alpha = 1f;
        this.tailSprite.alpha = 1f;
        this.phraseWidget.pivot = this.phraseWidgetDefault;
        this.phraseWidget.transform.localPosition = Vector3.zero;
        this.phraseWidget.height = 0;
        this.phraseWidget.width = 0;
        this.clipPanel.baseClipRegion = Vector4.zero;
        this.size = Vector2.zero;
        this.currentState = Dialog.State.Idle;
        this.ignoreInputFlag = false;
        this.isNeedResetChoice = true;
        this.typeAnimationEffect = true;
        this.position = Vector2.zero;
        this.offset = Vector3.zero;
        this.phrase = String.Empty;
        this.caption = String.Empty;
        this.PageParsers.Clear();
        this.CurrentParser = new TextParser(this.phraseLabel, String.Empty);
        this.captionLabel.rawText = this.caption;
        this.captionWidth = 0f;
        this.WidthHint = 0f;
        this.LineNumberHint = 1f;
        this.lineNumber = 0;
        this.id = -1;
        this.textId = -1;
        this.endMode = -1;
        base.transform.localPosition = Vector3.zero;
        this.tailPosition = Dialog.TailPosition.AutoPosition;
        this.phraseLabel.ReleaseAllIcons();
        this.phraseLabel.fixedAlignment = false;
        this.phraseLabel.rawText = String.Empty;
        this.messageValues.Clear();
        this.messageNeedUpdate = false;
        this.subPage.Clear();
        this.currentPage = 0;
        this.dialogAnimator.Pause = false;
        this.dialogAnimator.ShowWithoutAnimation = false;
        this.OnOptionChange = null;
        this.ResetChoose();
        this.isOverlayDialog = false;
        this.isOverlayChecked = false;
        this.overlayMessageNumber = -1;
        this.isChoiceReady = false;
        this.isClosedByScript = false;
        this.IsETbDialog = false;
    }

    private void setTailAutoPosition(Dialog.TailPosition tailPos, Boolean isUpdate)
    {
        if (!isUpdate)
            this.setTailPosition(tailPos);
        this.tailTransform.localPosition = new Vector3(this.tailMargin, this.tailTransform.localPosition.y, 0f);
    }

    private Boolean ThisDialogContainsForceTailPosition(Dialog.TailPosition _position)
    {
        switch (_position)
        {
            case Dialog.TailPosition.LowerRight:
            case Dialog.TailPosition.LowerLeft:
            case Dialog.TailPosition.UpperRight:
            case Dialog.TailPosition.UpperLeft:
            case Dialog.TailPosition.LowerCenter:
            case Dialog.TailPosition.UpperCenter:
                return false;
            case Dialog.TailPosition.LowerRightForce:
            case Dialog.TailPosition.LowerLeftForce:
            case Dialog.TailPosition.UpperRightForce:
            case Dialog.TailPosition.UpperLeftForce:
                return true;
        }
        return false;
    }

    private void setTailPosition(Dialog.TailPosition tailPos)
    {
        this.tailPosition = tailPos;
        UIWidget.Pivot pivot;
        switch (tailPos)
        {
            case Dialog.TailPosition.LowerRight:
            case Dialog.TailPosition.LowerLeft:
            case Dialog.TailPosition.LowerCenter:
            case Dialog.TailPosition.LowerRightForce:
            case Dialog.TailPosition.LowerLeftForce:
                pivot = UIWidget.Pivot.Top;
                break;
            case Dialog.TailPosition.UpperRight:
            case Dialog.TailPosition.UpperLeft:
            case Dialog.TailPosition.UpperCenter:
            case Dialog.TailPosition.UpperRightForce:
            case Dialog.TailPosition.UpperLeftForce:
                pivot = UIWidget.Pivot.Bottom;
                break;
            default:
                pivot = UIWidget.Pivot.Center;
                break;
        }
        this.bodySprite.pivot = pivot;
        this.borderSprite.pivot = pivot;
        Vector2 bodyPos = new Vector2(0f, 0f);
        switch (tailPos)
        {
            case Dialog.TailPosition.LowerRight:
            case Dialog.TailPosition.LowerLeft:
            case Dialog.TailPosition.LowerCenter:
            case Dialog.TailPosition.LowerRightForce:
            case Dialog.TailPosition.LowerLeftForce:
                bodyPos.y = this.size.y / 2f - (Dialog.DialogYPadding / 2f - Dialog.BorderPadding);
                break;
            case Dialog.TailPosition.UpperRight:
            case Dialog.TailPosition.UpperLeft:
            case Dialog.TailPosition.UpperCenter:
            case Dialog.TailPosition.UpperRightForce:
            case Dialog.TailPosition.UpperLeftForce:
                bodyPos.y = -this.size.y / 2f + (Dialog.DialogYPadding / 2f - Dialog.BorderPadding);
                break;
        }
        this.BodyGameObject.transform.localPosition = bodyPos;
        switch (tailPos)
        {
            case Dialog.TailPosition.LowerRight:
            case Dialog.TailPosition.LowerLeft:
            case Dialog.TailPosition.LowerCenter:
            case Dialog.TailPosition.LowerRightForce:
            case Dialog.TailPosition.LowerLeftForce:
                this.bodySprite.birthPosition = new Vector2(this.size.x / 2f, this.size.y);
                break;
            case Dialog.TailPosition.UpperRight:
            case Dialog.TailPosition.UpperLeft:
            case Dialog.TailPosition.UpperCenter:
            case Dialog.TailPosition.UpperRightForce:
            case Dialog.TailPosition.UpperLeftForce:
                this.bodySprite.birthPosition = new Vector2(this.size.x / 2f, 0f);
                break;
            default:
                this.bodySprite.birthPosition = new Vector2(this.size.x / 2f, this.size.y / 2f);
                break;
        }
        Vector2 phrasePos = this.phraseWidget.transform.localPosition;
        phrasePos.y = this.phraseWidget.height / 2;
        switch (tailPos)
        {
            case Dialog.TailPosition.LowerRight:
            case Dialog.TailPosition.LowerLeft:
            case Dialog.TailPosition.LowerCenter:
            case Dialog.TailPosition.LowerRightForce:
            case Dialog.TailPosition.LowerLeftForce:
                phrasePos.y -= 36f;
                break;
            case Dialog.TailPosition.UpperRight:
            case Dialog.TailPosition.UpperLeft:
            case Dialog.TailPosition.UpperCenter:
            case Dialog.TailPosition.UpperRightForce:
            case Dialog.TailPosition.UpperLeftForce:
                phrasePos.y += 6f;
                break;
            default:
                phrasePos.y -= 18f;
                break;
        }
        this.phraseWidget.transform.localPosition = phrasePos;
        switch (tailPos)
        {
            case Dialog.TailPosition.LowerRight:
            case Dialog.TailPosition.LowerRightForce:
                this.tailSprite.pivot = UIWidget.Pivot.TopLeft;
                break;
            case Dialog.TailPosition.LowerLeft:
            case Dialog.TailPosition.LowerLeftForce:
                this.tailSprite.pivot = UIWidget.Pivot.TopRight;
                break;
            case Dialog.TailPosition.UpperRight:
            case Dialog.TailPosition.UpperRightForce:
                this.tailSprite.pivot = UIWidget.Pivot.BottomLeft;
                break;
            case Dialog.TailPosition.UpperLeft:
            case Dialog.TailPosition.UpperLeftForce:
                this.tailSprite.pivot = UIWidget.Pivot.BottomRight;
                break;
            case Dialog.TailPosition.LowerCenter:
                this.tailSprite.pivot = UIWidget.Pivot.Top;
                break;
            case Dialog.TailPosition.UpperCenter:
                this.tailSprite.pivot = UIWidget.Pivot.Bottom;
                break;
            default:
                this.tailSprite.pivot = UIWidget.Pivot.Center;
                break;
        }
        switch (tailPos)
        {
            case Dialog.TailPosition.LowerRight:
            case Dialog.TailPosition.LowerRightForce:
                this.tailSprite.spriteName = "dialog_pointer_topleft";
                break;
            case Dialog.TailPosition.LowerLeft:
            case Dialog.TailPosition.LowerCenter:
            case Dialog.TailPosition.LowerLeftForce:
                this.tailSprite.spriteName = "dialog_pointer_topright";
                break;
            case Dialog.TailPosition.UpperRight:
            case Dialog.TailPosition.UpperRightForce:
                this.tailSprite.spriteName = "dialog_pointer_downleft";
                break;
            case Dialog.TailPosition.UpperLeft:
            case Dialog.TailPosition.UpperCenter:
            case Dialog.TailPosition.UpperLeftForce:
                this.tailSprite.spriteName = "dialog_pointer_downright";
                break;
            default:
                this.tailSprite.spriteName = String.Empty;
                break;
        }
        Single tx = this.tailSprite.localSize.x / 2f;
        Single ty = 0f;
        switch (tailPos)
        {
            case Dialog.TailPosition.LowerRight:
            case Dialog.TailPosition.LowerLeft:
            case Dialog.TailPosition.LowerCenter:
            case Dialog.TailPosition.LowerRightForce:
            case Dialog.TailPosition.LowerLeftForce:
                ty = this.size.y / 2f + Dialog.DialogYPadding / 2f;
                break;
            case Dialog.TailPosition.UpperRight:
            case Dialog.TailPosition.UpperLeft:
            case Dialog.TailPosition.UpperCenter:
            case Dialog.TailPosition.UpperRightForce:
            case Dialog.TailPosition.UpperLeftForce:
                ty = -(this.size.y / 2f) - Dialog.DialogYPadding / 2f + 1f;
                break;
        }
        this.tailTransform.localPosition = new Vector3(tx, ty, 0f);
    }

    private void Update()
    {
        this.UpdatePosition();
        this.UpdateMessageValue();
    }

    private void UpdateMessageValue()
    {
        if (this.currentState == Dialog.State.CompleteAnimation && this.messageNeedUpdate && !this.IsOverlayDialog && (this.HasMessageValueChanged() || this.HasOverlayChanged()))
            this.ReplaceMessageValue();
    }

    private Boolean HasMessageValueChanged()
    {
        foreach (KeyValuePair<Int32, Int32> messPair in this.messageValues)
            if (ETb.gMesValue[messPair.Key] != messPair.Value)
                return true;
        return false;
    }

    private void ReplaceMessageValue()
    {
        // [DBG] To check
        //if (this.overlayMessageNumber == i)
        //    formattedValue = NGUIText.FF9PinkColor + formattedValue + NGUIText.FF9WhiteColor;
        this.CurrentParser.ResetBeforeVariableTags();
        this.phraseLabel.MarkAsChanged();
    }

    private Boolean HasOverlayChanged()
    {
        if (this.IsOverlayDialog)
            return false;
        Dialog overlayDialog = Singleton<DialogManager>.Instance.GetOverlayDialog();
        Int32 messNum = -1;
        if (overlayDialog != null)
        {
            using (Dictionary<Int32, Int32>.Enumerator enumerator = overlayDialog.MessageValues.GetEnumerator())
            {
                if (enumerator.MoveNext())
                    messNum = enumerator.Current.Key;
            }
        }
        if (messNum != this.overlayMessageNumber)
        {
            this.overlayMessageNumber = messNum;
            return true;
        }
        return false;
    }

    public void UpdatePosition()
    {
        if (this.isReadyToFollow && this.focusToActor)
        {
            if (this.IsAutoPositionMode())
                this.FollowTarget();
            else
                this.isReadyToFollow = false;
        }
    }

    public void PauseDialog(Boolean isPause)
    {
        if (isPause)
        {
            if (this.currentState != Dialog.State.CompleteAnimation)
                this.dialogAnimator.Pause = true;
            this.phraseEffect.IsActive = false;
        }
        else
        {
            if (this.currentState != Dialog.State.CompleteAnimation)
                this.dialogAnimator.Pause = false;
            this.phraseEffect.IsActive = true;
            if (this.StartChoiceRow > -1)
            {
                this.isMuteSelectSound = true;
                ButtonGroupState.ActiveGroup = Dialog.DialogGroupButton;
                this.isMuteSelectSound = false;
            }
        }
    }

    public void ForceClose()
    {
        this.dialogAnimator.Pause = false;
        if (this.currentState != Dialog.State.CloseAnimation)
            this.Hide();
    }

    private void PrepareNextPage()
    {
        this.CurrentParser = this.currentPage < this.PageParsers.Count ? this.PageParsers[this.currentPage++] : new TextParser(this.phraseLabel, String.Empty);
        this.phrase = this.CurrentParser.InitialText;
        this.phraseLabel.ReleaseAllIcons();
        this.phraseLabel.Parser = this.CurrentParser;
        this.CurrentParser.ResetProgress();
        this.ResetChoose();
    }

    private void ApplyDialogTextPatch(TextParser parser)
    {
        String text = parser.ParsedText;
        if (text.Length > 0 && parser.LineInfo.Count == 1 && text.Length < 6 && text.Contains("!"))
            parser.InsertTag(new FFIXTextTag(FFIXTextTagCode.Center));
    }

    private void AutomaticSize()
    {
        this.phraseLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
        if (!this.CanAutoResize())
        {
            // Use the size specified by STRT tags
            this.phraseLabel.ProcessText();
            this.ApplyDialogTextPatch(this.CurrentParser);
            this.Width = this.WidthHint;
            this.LineNumber = this.LineNumberHint;
            return;
        }
        this.phraseLabel.width = (Int32)UIManager.UIContentSize.x;
        this.phraseLabel.height = (Int32)UIManager.UIContentSize.y;
        Single allPagesWidth = 0f;
        Single allPagesLineCount = 0f;
        foreach (TextParser parser in this.PageParsers)
        {
            this.phraseLabel.Parser = parser;
            parser.Parse(TextParser.ParseStep.Wrapped);
            this.ApplyDialogTextPatch(parser);
            allPagesWidth = Math.Max(allPagesWidth, (parser.MaxWidth + Dialog.DialogPhraseXPadding * 2f) / UIManager.ResourceXMultipier);
            allPagesLineCount = Math.Max(allPagesLineCount, parser.DialogLineCount);
        }
        this.phraseLabel.Parser = this.CurrentParser;
        if (allPagesWidth < this.originalWidth / 2f)
        {
            allPagesWidth = this.originalWidth - 1f;
        }
        else
        {
            foreach (DialogImage dialogImage in this.CurrentParser.SpecialImages)
                if (dialogImage.Id == 27) // [DBG] check if still taken into account - help_mog_dialog
                    allPagesWidth += 8f;
        }
        Single extraPadding = Dialog.DialogPhraseXPadding * 2f / UIManager.ResourceXMultipier;
        allPagesWidth = Math.Max(allPagesWidth, this.captionWidth + extraPadding) + 1f;
        this.Width = Math.Max(allPagesWidth, this.WidthHint);
        this.LineNumber = Mathf.CeilToInt(Math.Max(this.LineNumberHint, allPagesLineCount));
    }

    private Boolean CanAutoResize()
    {
        if (this.id == 9)
            return false;
        if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
        {
            Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
            if (fldMapNo >= 1400 && fldMapNo <= 1425 && (this.tailPosition == Dialog.TailPosition.UpperLeft || this.tailPosition == Dialog.TailPosition.UpperRight))
            {
                // Fossil Roo (surely the lever dialogs and path displays)
                if (this.windowStyle == Dialog.WindowStyle.WindowStyleTransparent)
                    return false;
                if (this.windowStyle == Dialog.WindowStyle.WindowStylePlain && this.startChoiceRow == -1)
                    return false;
            }
            if (EventHUD.CurrentHUD == MinigameHUD.Auction && this.windowStyle == Dialog.WindowStyle.WindowStyleTransparent)
                return false;
            if (EventHUD.CurrentHUD == MinigameHUD.ChocoHot && this.windowStyle == Dialog.WindowStyle.WindowStylePlain)
            {
                String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
                switch (currentLanguage)
                {
                    case "Japanese":
                    case "English(UK)":
                    case "English(US)":
                        return this.textId != 275; // "Depth:     "
                }
                return this.textId != 276;
            }
        }
        return true;
    }

    private Boolean ForceUpperTail()
    {
        Boolean result = false;
        if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
        {
            Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
            if (fldMapNo == 1608)
            {
                // Mdn. Sari/Secret Room, Zidane and Garnet dialogs
                Int32 scenarioCounter = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
                if (scenarioCounter == 6840)
                {
                    FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
                    Camera mainCamera = fieldmap.GetMainCamera();
                    Vector3 localPosition = mainCamera.transform.localPosition;
                    result = localPosition.x == 32f && localPosition.y == 0f && localPosition.z == 0f && this.targetPos.sid != 11 && this.targetPos.sid != 9;
                }
                else if (scenarioCounter == 6850)
                {
                    result = this.targetPos.sid != 9;
                }
            }
        }
        return result;
    }

    private void OverwriteDialogParameter()
    {
        if (FF9StateSystem.MobilePlatform && this.id < 9)
        {
            if (FF9StateSystem.Common.FF9.fldMapNo == 2951) // Chocobo's Lagoon
            {
                if (this.targetPos != null && this.targetPos.uid == 13 && this.startChoiceRow > -1) // Moogle
                {
                    this.Po = null;
                    this.windowStyle = Dialog.WindowStyle.WindowStyleNoTail;
                    this.tailPosition = Dialog.TailPosition.UpperCenter;
                }
            }
        }
    }

    public GameObject DialogChoicePrefab;

    [SerializeField]
    private Int32 startChoiceRow;

    [SerializeField]
    private Int32 choiceNumber;

    [SerializeField]
    private Int32 defaultChoice;

    [SerializeField]
    private Int32 cancelChoice;

    [SerializeField]
    private Int32 selectedChoice;

    [SerializeField]
    private Int32 chooseMask;
    private List<GameObject> choiceList;
    private List<GameObject> maskChoiceList;

    [SerializeField]
    private List<Int32> disableIndexes;
    private List<Int32> activeIndexes;

    public const String DialogGroupButton = "Dialog.Choice";

    public static Vector2 DefaultOffset = new Vector2(36f, 0f);

    [SerializeField]
    private List<Single> choiceYPositions; // Dummied

    private Boolean isChoiceReady;

    public static Byte DialogMaximumDepth = 68;
    public static Byte DialogAdditionalRaiseDepth = 22;

    public static Byte[] DialogTextAnimationTick = new Byte[]
    {
		// The "Field Message" speed (eg. the slowest is the same as defaulting message speeds to [SPED=4])
		4,
        7,
        10,
        16,
        24,
        40,
        64,
        64
    };

    public Dialog.DialogIntDelegate AfterDialogHidden;
    public Dialog.DialogIntDelegate AfterDialogShown;
    public Dialog.DialogVoidDelegate AfterDialogSentenseShown;

    [NonSerialized]
    public Dialog.OnSelectedOptionChangeDelegate OnOptionChange = null;

    public GameObject BodyGameObject;
    public GameObject BorderGameObject;
    public GameObject CaptionGameObject;
    public GameObject PhraseGameObject;
    public GameObject TailGameObject;
    public GameObject ChooseContainerGameObject;
    public UIPanel phrasePanel;

    public static readonly Int32 WindowMinWidth = (Int32)(UIManager.ResourceXMultipier * 32f);
    public static readonly Int32 AdjustWidth = (Int32)(UIManager.ResourceXMultipier * 3f);

    public static readonly Int32 DialogOffsetY = (Int32)(UIManager.ResourceYMultipier * 20f);

    public static readonly Int32 DialogLimitLeft = (Int32)(UIManager.ResourceXMultipier * 8f);
    public static readonly Int32 DialogLimitRight = (Int32)(UIManager.ResourceXMultipier * (FieldMap.PsxScreenWidth - 8f));
    public static readonly Int32 DialogLimitTop = (Int32)(UIManager.ResourceYMultipier * 8f);
    public static readonly Int32 DialogLimitBottom = (Int32)(UIManager.ResourceYMultipier * 208f);

    public static readonly Int32 DialogTailLeftRightOffset = (Int32)(UIManager.ResourceXMultipier * 6f);
    public static readonly Int32 DialogTailTopOffset = (Int32)(UIManager.ResourceYMultipier * 16f);
    public static readonly Int32 DialogTailBottomOffset = (Int32)(UIManager.ResourceYMultipier * 32f);

    public static readonly Int32 kCenterX = (Int32)(UIManager.ResourceXMultipier * FieldMap.HalfScreenWidth);
    public static readonly Int32 kCenterY = (Int32)(UIManager.ResourceYMultipier * FieldMap.HalfScreenHeight);
    public static readonly Int32 kDialogY = (Int32)(UIManager.ResourceYMultipier * 150f);

    public static readonly Single TailMagicNumber1 = (Int32)(24f * UIManager.ResourceXMultipier);
    public static readonly Single TailMagicNumber2 = (Int32)(8f * UIManager.ResourceXMultipier);

    public static readonly Single PosXOffset = -2f;

    public static readonly Single kUpperOffset = Dialog.DialogTailTopOffset;
    public static readonly Single kLimitTop = 24f;
    public static readonly Single kLowerOffset = Dialog.DialogTailBottomOffset * 3 / 4;
    public static readonly Single kLimitBottom = Dialog.DialogLimitBottom - 4;

    public static readonly Single kMargin = UIManager.ResourceXMultipier * 5f;

    public static readonly Single PosYOffset = -15f;

    public static readonly Single InitialMagicNum = Mathf.Ceil(32f * UIManager.ResourceXMultipier);

    public const Single DialogLineHeight = 68f;
    public const Single DialogYPadding = 80f;
    public const Single DialogXPadding = 18f;
    public const Single BorderPadding = 18f;

    public const Single DialogPhraseXPadding = 16f;
    public const Single DialogPhraseYPadding = 20f;

    public const Int32 FF9TextSpeedRatio = 2;

    private Transform bodyTransform;
    private Transform tailTransform;

    private UISprite bodySprite;
    private UISprite borderSprite;
    private UISprite tailSprite;
    private UIPanel clipPanel;
    private UIWidget phraseWidget;
    private UILabel phraseLabel;
    private UILabel captionLabel;
    private TypewriterEffect phraseEffect;
    private DialogAnimator dialogAnimator;

    [SerializeField]
    private Dialog.TailPosition tailPosition = Dialog.TailPosition.AutoPosition;

    [SerializeField]
    private Int32 id = -1;

    [SerializeField]
    private Int32 sid;

    [SerializeField]
    private GameObject followObject;

    [SerializeField]
    private Int32 textId;

    [SerializeField]
    private Dialog.State currentState;

    private Boolean isForceTailPosition;
    private Single tailMargin;
    private Single originalWidth;
    private Int32 lineNumber; // [DBG] Turn to Single if possible

    private Vector2 size = Vector2.zero;

    [SerializeField]
    private Vector2 position = Vector2.zero;

    [SerializeField]
    private Vector3 offset = Vector3.zero;

    [SerializeField]
    private String phrase = String.Empty;
    private String phraseMessageValue = String.Empty;

    [NonSerialized]
    public List<TextParser> PageParsers = new List<TextParser>();
    [NonSerialized]
    public TextParser CurrentParser;

    [SerializeField]
    private List<String> subPage = new List<String>();

    private String caption = String.Empty;
    private Dialog.CaptionType capType;

    [SerializeField]
    private Dialog.WindowStyle windowStyle;

    private Int32 endMode = -1;

    [SerializeField]
    private Boolean ignoreInputFlag;

    [SerializeField]
    private Boolean isNeedResetChoice = true;

    private Single dialogShowTime;
    private Single dialogHideTime;

    [NonSerialized]
    private Boolean isClosedByScript = false;

    private Boolean isMuteSelectSound;

    private Boolean typeAnimationEffect = true;

    private Dictionary<Int32, Single> messageSpeed; // [DBG] delete if possible
    private Dictionary<Int32, Single> messageWait; // [DBG] delete if possible

    [SerializeField]
    private List<DialogImage> imageList = new List<DialogImage>(); // [DBG] delete if possible

    private PosObj targetPos;

    [SerializeField]
    private Vector2 ff9Position;

    private UIPanel panel;

    private Boolean isActive;

    private UIWidget.Pivot phraseWidgetDefault;

    [SerializeField]
    private Boolean isReadyToFollow;

    [SerializeField]
    private Boolean focusToActor = true;

    private Int32 signalNumber; // [DBG] delete if possible
    private Int32 signalMode; // [DBG] delete if possible

    private Dictionary<Int32, Int32> messageValues = new Dictionary<Int32, Int32>();

    private Boolean messageNeedUpdate;

    private Int32 currentPage;

    [SerializeField]
    private Single captionWidth;

    [SerializeField]
    private Boolean isOverlayDialog;

    [SerializeField]
    private Boolean isOverlayChecked;

    [SerializeField]
    private Int32 overlayMessageNumber = -1;

    public enum TailPosition
    {
        Center = -2,
        AutoPosition,
        LowerRight,
        LowerLeft,
        UpperRight,
        UpperLeft,
        LowerCenter,
        UpperCenter,
        LowerRightForce = 8,
        LowerLeftForce,
        UpperRightForce,
        UpperLeftForce,
        DialogPosition
    }

    public enum WindowStyle
    {
        WindowStyleAuto,
        WindowStylePlain,
        WindowStyleTransparent,
        WindowStyleNoTail
    }

    public enum State
    {
        Idle,
        Initial,
        OpenAnimation,
        TextAnimation,
        CompleteAnimation,
        StartHide,
        CloseAnimation
    }

    public enum CaptionType
    {
        None,
        Mognet,
        ActiveTimeEvent,
        Chocobo,
        Notice
    }

    public enum WindowID
    {
        ID0,
        ID1,
        ID2,
        ID3,
        ID4,
        ID5,
        ID6,
        ID7
    }

    public delegate void DialogVoidDelegate();
    public delegate void DialogIntDelegate(Int32 choice);
    public delegate void OnSelectedOptionChangeDelegate(Int32 msg, Int32 optionIndex);
}
