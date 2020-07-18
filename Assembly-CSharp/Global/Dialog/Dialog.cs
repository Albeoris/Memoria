using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;
using Memoria.Prime.Text;
using UnityEngine;
using Object = System.Object;

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
		this.choiceYPositions = new List<Single>();
		}

	static Dialog()
	{
		// Note: this type is marked as 'beforefieldinit'.
		Dialog.DialogGroupButton = "Dialog.Choice";
		Dialog.DefaultOffset = new Vector2(36f, 0f);
	}

	public Int32 StartChoiceRow
	{
		get
		{
			return this.startChoiceRow;
		}
		set
		{
			this.startChoiceRow = value;
		}
	}

	public Int32 ChoiceNumber
	{
		get
		{
			return this.choiceNumber;
		}
		set
		{
			this.choiceNumber = value;
		}
	}

	public Int32 DefaultChoice
	{
		get
		{
			return this.defaultChoice;
		}
		set
		{
			this.defaultChoice = value;
		}
	}

	public Int32 CancelChoice
	{
		get
		{
			return this.cancelChoice;
		}
		set
		{
			this.cancelChoice = value;
		}
	}

	public Int32 SelectChoice
	{
		get
		{
			return this.selectedChoice;
		}
		set
		{
			this.selectedChoice = value;
			DialogManager.SelectChoice = value;
		}
	}

	public GameObject ActiveChoice
	{
		get
		{
			return ButtonGroupState.ActiveButton;
		}
	}

	public List<Int32> ActiveIndexes
	{
		get
		{
			return this.activeIndexes;
		}
	}

	public List<Int32> DisableIndexes
	{
		get
		{
			return this.disableIndexes;
		}
	}

	public Int32 ChooseMask
	{
		get
		{
			return this.chooseMask;
		}
		set
		{
			this.chooseMask = value;
			this.ProcessChooseMask();
			this.LineNumber -= (Single)this.disableIndexes.Count<Int32>();
		}
	}

	public Boolean IsChoiceReady
	{
		get
		{
			return this.isChoiceReady;
		}
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
		do
		{
			yield return new WaitForEndOfFrame();
		}
		while (PersistenSingleton<UIManager>.Instance.IsPause);
		PersistenSingleton<UIManager>.Instance.Dialogs.ShowChoiceHud();
		this.CalculateYLine();
		Int32 choiceCounter = 0;
		Int32 lineControl = this.startChoiceRow;
		Single startYPos = this.phraseLabel.transform.localPosition.y;
		Single colliderHeight = Dialog.DialogLineHeight;
		Int32 totalLine = this.lineNumber + this.disableIndexes.Count<Int32>();
		Int32 line = this.startChoiceRow;
		while (line < totalLine)
		{
			GameObject choice = NGUITools.AddChild(this.ChooseContainerGameObject, this.DialogChoicePrefab);
			UIWidget choiceWidget = choice.GetComponent<UIWidget>();
			UIKeyNavigation choiceKeyNav = choice.GetComponent<UIKeyNavigation>();
			UIWidget phraseWidget = this.PhraseGameObject.GetComponent<UIWidget>();
			choice.name = "Choice#" + line;
			choice.transform.parent = this.ChooseContainerGameObject.transform;
			choice.transform.position = this.PhraseGameObject.transform.position;
			Vector3 localPos = (lineControl >= this.choiceYPositions.Count) ? Vector3.zero : new Vector3(0f, startYPos + this.choiceYPositions[lineControl], 0f);
			choice.transform.localPosition = localPos;
			choiceWidget.width = phraseWidget.width;
			choiceWidget.height = (Int32)colliderHeight;
			if (line - this.startChoiceRow == this.defaultChoice)
			{
				choiceKeyNav.startsSelected = true;
			}
			if (this.disableIndexes.Contains(choiceCounter))
			{
				choice.SetActive(false);
			}
			else
			{
				lineControl++;
				this.maskChoiceList.Add(choice);
			}
			this.choiceList.Add(choice);
			line++;
			choiceCounter++;
		}
		NGUIExtension.SetKeyNevigation(this.choiceList);
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
		foreach (GameObject obj in this.choiceList)
		{
			UnityEngine.Object.DestroyObject(obj);
		}
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

	public void SetCurrentChoiceRef(Int32 choiceRef)
	{
		Int32 selectChoice = this.SelectChoice;
		Int32 num = this.SelectChoice;
		Int32 num2 = this.maskChoiceList.IndexOf(this.choiceList[this.SelectChoice]);
		Int32 num3 = choiceRef + num2;
		num3 = Mathf.Clamp(num3, 0, this.maskChoiceList.Count - 1);
		num = this.choiceList.IndexOf(this.maskChoiceList[num3]);
		if (selectChoice != num)
		{
			this.SetCurrentChoice(num);
		}
	}

	private void ProcessChooseMask()
	{
		Int32 num = this.chooseMask;
		if (num == -1)
		{
			return;
		}
		for (Int32 i = 0; i < this.choiceNumber; i++)
		{
			if (num % 2 == 0)
			{
				this.disableIndexes.Add(i);
			}
			else
			{
				this.activeIndexes.Add(i);
			}
			num >>= 1;
		}
	}

	public Boolean SkipThisChoice(Int32 choiceIndex)
	{
		return this.disableIndexes.Contains(choiceIndex);
	}

	private void CalculateYLine()
	{
		this.choiceYPositions.Clear();
		this.phraseLabel.UpdateNGUIText();
		BetterList<Vector3> verts = this.phraseLabel.geometry.verts;
		Int32 num = verts.size / 2;
		foreach (Int32 num2 in this.phraseLabel.VertsLineOffsets)
		{
			if (num2 < num)
			{
				this.choiceYPositions.Add((verts[num2].y + verts[num2 + 1].y) / 2f);
			}
		}
	}

	public Int32 Id
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
		}
	}

	public DialogAnimator DialogAnimate
	{
		get
		{
			return this.dialogAnimator;
		}
	}

	public Dialog.TailPosition Tail
	{
		get
		{
			return this.tailPosition;
		}
		set
		{
			this.setTailPosition(value);
		}
	}

	public Single TailMargin
	{
		get
		{
			return this.tailMargin;
		}
	}

	public Dialog.WindowStyle Style
	{
		get
		{
			return this.windowStyle;
		}
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

	public Vector2 Size
	{
		get
		{
			return this.size;
		}
	}

	public Vector2 ClipSize
	{
		get
		{
			return new Vector2(this.size.x + Dialog.DialogXPadding * 2f, this.size.y + Dialog.DialogYPadding);
		}
	}

	public Vector2 Position
	{
		get
		{
			return this.position;
		}
		set
		{
			this.position = value * UIManager.ResourceYMultipier;
		}
	}

	public Vector3 OffsetPosition
	{
		get
		{
			return this.offset;
		}
		set
		{
			this.offset = value;
		}
	}

	public String Phrase
	{
		get
		{
			return this.phrase;
		}
		set
		{
			String text = this.OverwritePrerenderText(value);
            DialogBoxConstructor.PhrasePreOpcodeSymbol(text, this);
			this.PrepareNextPage();
		}
	}

	public String Caption
	{
		get
		{
			return this.caption;
		}
		set
		{
			if (this.captionLabel.text != value && !String.IsNullOrEmpty(value))
			{
				this.caption = value;
				this.captionLabel.text = value;
				this.captionWidth = NGUIText.GetTextWidthFromFF9Font(this.captionLabel, value);
			}
		}
	}

	public Dialog.CaptionType CapType
	{
		get
		{
			return this.capType;
		}
		set
		{
			this.capType = value;
		}
	}

	public Single CaptionWidth
	{
		get
		{
			return this.captionWidth;
		}
	}

	public Single Width
	{
		get
		{
			return this.size.x;
		}
		set
		{
			this.originalWidth = value;
			value *= UIManager.ResourceXMultipier;
			this.size.x = Mathf.Max(value, (Single)(Dialog.WindowMinWidth - Dialog.AdjustWidth));
			this.bodySprite.width = (Int32)this.size.x;
			this.phraseWidget.width = (Int32)this.size.x - (Int32)(Dialog.DialogPhraseXPadding * 2f);
			this.phraseWidget.transform.localPosition = new Vector3((Single)(-(Single)this.phraseWidget.width) / 2f, this.phraseWidget.transform.localPosition.y, this.phraseWidget.transform.localPosition.z);
			this.clipPanel.baseClipRegion = new Vector4(this.clipPanel.baseClipRegion.x, this.clipPanel.baseClipRegion.y, this.ClipSize.x, this.ClipSize.y);
		}
	}

	public Single OriginalWidth
	{
		get
		{
			return this.originalWidth;
		}
	}

	public Single LineNumber
	{
		get
		{
			return (Single)this.lineNumber;
		}
		set
		{
			this.lineNumber = (Int32)value;
			this.size.y = value * Dialog.DialogLineHeight + Dialog.DialogPhraseYPadding;
			this.bodySprite.height = (Int32)this.size.y;
			this.phraseWidget.height = (Int32)this.size.y - (Int32)Dialog.DialogPhraseYPadding;
			this.phraseWidget.transform.localPosition = new Vector3(this.phraseWidget.transform.localPosition.x, (Single)this.phraseWidget.height / 2f, this.phraseWidget.transform.localPosition.z);
			this.clipPanel.baseClipRegion = new Vector4(this.clipPanel.baseClipRegion.x, this.clipPanel.baseClipRegion.y, this.ClipSize.x, this.ClipSize.y);
		}
	}

	public Int32 EndMode
	{
		get
		{
			return this.endMode;
		}
		set
		{
			this.endMode = value;
		}
	}

	public Dictionary<Int32, Int32> MessageSpeedDict
	{
		get
		{
			return this.messageSpeed;
		}
	}

	public Dictionary<Int32, Single> MessageWaitDict
	{
		get
		{
			return this.messageWait;
		}
	}

	public Boolean TypeEffect
	{
		get
		{
			return this.typeAnimationEffect;
		}
		set
		{
			this.typeAnimationEffect = value;
		}
	}

	public Boolean FlagButtonInh
	{
		get
		{
			return this.ignoreInputFlag;
		}
		set
		{
			this.ignoreInputFlag = value;
		}
	}

	public Boolean FlagResetChoice
	{
		get
		{
			return this.isNeedResetChoice;
		}
		set
		{
			this.isNeedResetChoice = value;
		}
	}

	public List<Dialog.DialogImage> ImageList
	{
		get
		{
			return this.imageList;
		}
		set
		{
			this.imageList = value;
		}
	}

	public PosObj Po
	{
		get
		{
			return this.targetPos;
		}
		set
		{
			this.targetPos = value;
			if (this.targetPos != null)
			{
				this.sid = (Int32)this.targetPos.sid;
				this.followObject = ((!(this.targetPos.go != (UnityEngine.Object)null)) ? null : this.targetPos.go);
			}
			else
			{
				this.sid = -1;
				this.followObject = (GameObject)null;
			}
		}
	}

	public Vector2 FF9Position
	{
		get
		{
			return this.ff9Position;
		}
		set
		{
			this.ff9Position = value;
		}
	}

	public UIPanel Panel
	{
		get
		{
			if (this.panel == (UnityEngine.Object)null)
			{
				this.panel = base.GetComponent<UIPanel>();
			}
			return this.panel;
		}
	}

	public Boolean IsActive
	{
		get
		{
			return this.isActive;
		}
	}

	public Boolean FocusToActor
	{
		get
		{
			return this.focusToActor;
		}
		set
		{
			this.focusToActor = value;
		}
	}

	public Boolean IsReadyToFollow
	{
		get
		{
			return this.isReadyToFollow;
		}
	}

	public Dialog.State CurrentState
	{
		get
		{
			return this.currentState;
		}
		set
		{
			this.currentState = value;
		}
	}

	public UILabel PhraseLabel
	{
		get
		{
			return this.phraseLabel;
		}
	}

	public Int32 TextId
	{
		get
		{
			return this.textId;
		}
		set
		{
			this.textId = value;
		}
	}

	public Int32 SignalNumber
	{
		get
		{
			return this.signalNumber;
		}
		set
		{
			this.signalNumber = value;
		}
	}

	public Int32 SignalMode
	{
		get
		{
			return this.signalMode;
		}
		set
		{
			this.signalMode = value;
		}
	}

	public Dictionary<Int32, Int32> MessageValues
	{
		get
		{
			return this.messageValues;
		}
	}

	public Boolean MessageNeedUpdate
	{
		get
		{
			return this.messageNeedUpdate;
		}
		set
		{
			this.messageNeedUpdate = value;
		}
	}

	public List<String> SubPage
	{
		get
		{
			return this.subPage;
		}
	}

	public Single DialogShowTime
	{
		get
		{
			return this.dialogShowTime;
		}
	}

	public Single DialogHideTime
	{
		get
		{
			return this.dialogHideTime;
		}
	}

	public Boolean IsOverlayDialog
	{
		get
		{
			if (this.isOverlayChecked)
			{
				return this.isOverlayDialog;
			}
			EventEngine instance = PersistenSingleton<EventEngine>.Instance;
			if (instance == (UnityEngine.Object)null)
			{
				return this.isOverlayDialog;
			}
			if (instance.gMode == 1)
			{
				if (FF9TextTool.FieldZoneId == 23)
				{
					String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
				    switch (currentLanguage)
				    {
                        case "Japanese":
                        case "French":
                            this.isOverlayDialog = (this.textId == 154 || this.textId == 155);
                            goto IL_136;
                        case "Italian":
                            this.isOverlayDialog = (this.textId == 149 || this.textId == 150);
                            goto IL_136;
                    }
                    
					this.isOverlayDialog = (this.textId == 134 || this.textId == 135);
					IL_136:;
				}
				else if (FF9TextTool.FieldZoneId == 70 || FF9TextTool.FieldZoneId == 741)
				{
					String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
                    switch (currentLanguage)
                    {
                        case "English(US)":
                        case "English(UK)":
                            this.isOverlayDialog = (this.textId == 204 || this.textId == 205 || this.textId == 206);
                            goto IL_22A;
                    }
                    
					this.isOverlayDialog = (this.textId == 205 || this.textId == 206 || this.textId == 207);
					IL_22A:;
				}
				else if (FF9TextTool.FieldZoneId == 166)
				{
					this.isOverlayDialog = (this.textId == 106 || this.textId == 107);
				}
				else if (FF9TextTool.FieldZoneId == 358)
				{
					String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
					switch (currentLanguage)
					{
					case "Japanese":
					case "French":
						this.isOverlayDialog = (this.textId == 874 || this.textId == 875);
						goto IL_3DB;
					case "Spanish":
						this.isOverlayDialog = (this.textId == 859 || this.textId == 860);
						goto IL_3DB;
					case "German":
						this.isOverlayDialog = (this.textId == 875 || this.textId == 876);
						goto IL_3DB;
					case "Italian":
						this.isOverlayDialog = (this.textId == 889 || this.textId == 890);
						goto IL_3DB;
					}
					this.isOverlayDialog = (this.textId == 861 || this.textId == 862);
					IL_3DB:;
				}
				else if (FF9TextTool.FieldZoneId == 945)
				{
					String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
				    if (currentLanguage == "Japanese")
				    {
                        this.isOverlayDialog = (this.textId == 251 || this.textId == 252);
                        goto IL_497;
                    }
                    
					this.isOverlayDialog = (this.textId == 252 || this.textId == 253);
				}
			}
			IL_497:
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
	}

	public void Show()
	{
		this.OverwriteDialogParameter();
		this.dialogShowTime = RealTime.time;
		this.AutomaticWidth();
		this.InitializeDialogTransition();
		this.InitializeWindowType();
		this.SetMessageSpeed(-1, 0);
		this.currentState = Dialog.State.OpenAnimation;
		this.dialogAnimator.ShowDialog();
        PersistenSingleton<UIManager>.Instance.Dialogs.CurMesId = this.textId;
        this.StartSignalProcess();
	}

	public void Hide()
	{
		this.dialogHideTime = RealTime.time;
		base.StopAllCoroutines();
		if (this.subPage.Count > this.currentPage)
		{
			this.PrepareNextPage();
			this.dialogAnimator.ShowNewPage();
			this.StartSignalProcess();
			this.currentState = Dialog.State.OpenAnimation;
			return;
		}
		this.messageSpeed.Clear();
		this.messageWait.Clear();
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
        {
            UIManager.Input.ResetTriggerEvent();
        }
    }

	public void AfterShown()
	{
		this.InitializeChoice();
		if (!this.typeAnimationEffect)
		{
			NGUIText.ProcessFF9Signal(ref this.signalMode, ref this.signalNumber);
			this.ShowAllIcon();
			this.currentState = Dialog.State.CompleteAnimation;
			if (base.gameObject.activeInHierarchy && this.endMode > 0)
			{
				base.StartCoroutine("AutoHide");
			}
		}
		if (this.AfterDialogShown != null)
		{
			this.AfterDialogShown(this.id);
		}
		if (this.targetPos != null)
		{
			this.isReadyToFollow = true;
		}
	}

	public void AfterSentenseShown()
	{
		if (this.currentState != Dialog.State.TextAnimation)
		{
			return;
		}
		this.currentState = Dialog.State.CompleteAnimation;
		this.phraseEffect.enabled = false;
		UIDebugMarker.DebugLog(String.Concat(new Object[]
		{
			"AfterSentenseShown Id:",
			this.Id,
			" Animation State:",
			this.currentState
		}));
		if (this.endMode > 0 && base.gameObject.activeInHierarchy)
		{
			base.StartCoroutine("AutoHide");
		}
		if (this.AfterDialogSentenseShown != null)
		{
			this.AfterDialogSentenseShown();
		}
	}

	public void AfterHidden()
	{
		EventHUD.CheckSpecialHUDFromMesId(this.textId, false);
        ETb.ProcessDialog(this);
        ETb.ProcessATEDialog(this);
		Singleton<DialogManager>.Instance.ReleaseDialogToPool(this);
		if (this.AfterDialogHidden != null)
		{
			if (this.startChoiceRow > -1)
			{
				this.AfterDialogHidden(this.SelectChoice);
			}
			else
			{
				this.AfterDialogHidden(-1);
			}
			this.AfterDialogHidden = (Dialog.DialogIntDelegate)null;
		}
		this.Reset();
	}

	public void OnKeyConfirm(GameObject go)
	{
        if (FF9StateSystem.Common.FF9.fldMapNo == 2951 || FF9StateSystem.Common.FF9.fldMapNo == 2952)
        {
            string symbol = Localization.GetSymbol();
            if (symbol == "JP" && Singleton<DialogManager>.Instance.PressMesId == 245 && Singleton<DialogManager>.Instance.ReleaseMesId == 226)
            {
                return;
            }
            if (Singleton<DialogManager>.Instance.PressMesId == 246 && Singleton<DialogManager>.Instance.ReleaseMesId == 227)
            {
                return;
            }
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 2950)
        {
            string symbol2 = Localization.GetSymbol();
            if (symbol2 == "JP" && Singleton<DialogManager>.Instance.PressMesId == 245 && Singleton<DialogManager>.Instance.ReleaseMesId == 225)
            {
                return;
            }
            if (Singleton<DialogManager>.Instance.PressMesId == 246 && Singleton<DialogManager>.Instance.ReleaseMesId == 226)
            {
                return;
            }
        }
        if (this.currentState == Dialog.State.CompleteAnimation)
		{
			if (this.startChoiceRow > -1)
			{
				this.SelectChoice = this.choiceList.IndexOf(ButtonGroupState.ActiveButton);
			}
			if (!this.ignoreInputFlag)
			{
				if (this.startChoiceRow > -1)
				{
					if (this.isChoiceReady)
					{
						this.Hide();
					}
				}
				else
				{
					this.Hide();
				}
			}
		}
		if (this.currentState == Dialog.State.TextAnimation && this.typeAnimationEffect)
		{
			this.phraseLabel.text = this.phrase;
			this.ShowAllIcon();
			this.AfterSentenseShown();
		}
	}

	public void OnKeyCancel(GameObject go)
	{
		if (this.startChoiceRow > -1 && this.cancelChoice > -1 && this.currentState == Dialog.State.CompleteAnimation)
		{
			this.isMuteSelectSound = true;
			this.SetCurrentChoice(this.cancelChoice);
			ETb.SndCancel();
		}
	}

	public void OnItemSelect(GameObject go)
	{
		if (this.currentState == Dialog.State.CompleteAnimation)
		{
			this.SelectChoice = this.choiceList.IndexOf(go);
			if (!this.isMuteSelectSound)
			{
				ETb.SndMove();
			}
			else
			{
				this.isMuteSelectSound = false;
			}
		}
	}

	private IEnumerator AutoHide()
	{
		Single waitTime = (Single)this.endMode / 30f;
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
			String localSymbol3 = Localization.GetSymbol();
			if (localSymbol3 != "US" && localSymbol3 != "JP")
			{
				waitTime -= -0.56f;
			}
			else
			{
				waitTime -= 0.22f;
			}
		}
		while (waitTime > 0f)
		{
			waitTime -= ((!HonoBehaviorSystem.Instance.IsFastForwardModeActive()) ? Time.deltaTime : (Time.deltaTime * (Single)HonoBehaviorSystem.Instance.GetFastForwardFactor()));
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
		Single num = this.position.x;
		Single num2 = this.position.y;
		Boolean flag = false;
		if (this.id == 9)
		{
			this.Panel.depth = (Int32)(Dialog.DialogMaximumDepth + Dialog.DialogAdditionalRaiseDepth + 2);
			this.phrasePanel.depth = this.Panel.depth + 1;
		}
		else if (this.id != -1)
		{
			this.Panel.depth = (Int32)Dialog.DialogMaximumDepth - this.id * 2;
			this.phrasePanel.depth = this.Panel.depth + 1;
		}
		if (this.position != Vector2.zero)
		{
			this.Po = (PosObj)null;
		}
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
            ETb.GetMesPos(this.Po, out float num3, out float num4);
            num = num3;
			num2 = num4;
			Boolean flag2;
			if (this.tailPosition == Dialog.TailPosition.AutoPosition)
			{
				EventEngine instance = PersistenSingleton<EventEngine>.Instance;
				Obj obj = instance.FindObjByUID((Int32)((this.Po.uid != instance.GetControlUID()) ? instance.GetControlUID() : ((Actor)this.Po).listener));
				PosObj posObj = (PosObj)null;
				if (obj != null)
				{
					posObj = ((!instance.isPosObj(obj)) ? null : ((PosObj)obj));
				}
				if (posObj != null)
				{
					Single num5;
					Single num6;
					ETb.GetMesPos(posObj, out num5, out num6);
					flag = (num4 < num6);
					if (this.ForceUpperTail())
					{
						flag = true;
					}
					flag2 = (num3 < num5);
				}
				else
				{
					flag = true;
					flag2 = false;
				}
			}
			else
			{
                flag = (((TailPosition)(((Int32)this.tailPosition) >> 1) & Dialog.TailPosition.LowerLeft) == Dialog.TailPosition.LowerLeft);
                flag2 = ((this.tailPosition & Dialog.TailPosition.LowerLeft) == Dialog.TailPosition.LowerLeft);
			}
			if (!this.isForceTailPosition)
			{
				Single num7 = (Single)Dialog.DialogLimitLeft + Dialog.InitialMagicNum;
				Single num8 = (Single)Dialog.DialogLimitRight - Dialog.InitialMagicNum;
				flag2 ^= ((!flag2) ? (num > num8) : (num < num7));
			}
			num += Dialog.PosXOffset;
			num2 += Dialog.PosYOffset;
			num = this.setPositionX(num, this.size.x, flag2, false);
			if (this.isForceTailPosition)
			{
				num2 = this.forceSetPositionY(num2, this.size.y, flag);
			}
			else
			{
				num2 = this.setPositionY(num2, this.size.y, ref flag);
			}
			this.ff9Position = new Vector2(num, num2);
			if (!this.isForceTailPosition && Singleton<DialogManager>.Instance.CheckDialogOverlap(this))
			{
				flag ^= true;
				num2 = this.setPositionY(num4, this.size.y, ref flag);
				this.ff9Position = new Vector2(num, num2);
			}
			Dialog.CalculateDialogCenter(ref num, ref num2, this.ClipSize);
			this.tailMargin -= num;
			this.tailPosition = (Dialog.TailPosition)(Convert.ToInt32(flag) << 1 | Convert.ToInt32(flag2));
			this.HideUnusedSprite();
			num2 = this.CalculateYPositionAfterHideTail(num2, flag);
			this.setAutoPosition(num, num2);
			this.setTailAutoPosition(this.tailPosition, false);
			this.isActive = true;
		}
		else
		{
			if (this.windowStyle == Dialog.WindowStyle.WindowStyleAuto)
			{
				this.windowStyle = Dialog.WindowStyle.WindowStylePlain;
			}
			if (num == 0f && num2 == 0f)
			{
				switch (this.tailPosition)
				{
				case Dialog.TailPosition.LowerRight:
					num = (Single)Dialog.DialogLimitRight - Dialog.kMargin - this.size.x / 2f;
					num2 = (Single)Dialog.DialogLimitTop + Dialog.kMargin + this.size.y / 2f;
					goto IL_5B5;
				case Dialog.TailPosition.LowerLeft:
					num = (Single)Dialog.DialogLimitLeft + Dialog.kMargin + this.size.x / 2f;
					num2 = (Single)Dialog.DialogLimitTop + Dialog.kMargin + this.size.y / 2f;
					goto IL_5B5;
				case Dialog.TailPosition.UpperRight:
					num = (Single)Dialog.DialogLimitRight - Dialog.kMargin - this.size.x / 2f;
					num2 = (Single)Dialog.DialogLimitBottom - Dialog.kMargin - this.size.y / 2f;
					goto IL_5B5;
				case Dialog.TailPosition.UpperLeft:
					num = (Single)Dialog.DialogLimitLeft + Dialog.kMargin + this.size.x / 2f;
					num2 = (Single)Dialog.DialogLimitBottom - Dialog.kMargin - this.size.y / 2f;
					if (PersistenSingleton<UIManager>.Instance.IsPauseControlEnable && PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.FieldHUD)
					{
						num += (Single)UIManager.Field.PauseWidth;
					}
					goto IL_5B5;
				case Dialog.TailPosition.LowerCenter:
					num = (Single)Dialog.kCenterX;
					num2 = (Single)Dialog.DialogLimitTop + Dialog.kMargin + this.size.y / 2f;
					goto IL_5B5;
				case Dialog.TailPosition.UpperCenter:
					num = (Single)Dialog.kCenterX;
					num2 = (Single)Dialog.DialogLimitBottom - Dialog.kMargin - this.size.y / 2f;
					goto IL_5B5;
				case Dialog.TailPosition.DialogPosition:
					num = (Single)Dialog.kCenterX;
					num2 = (Single)Dialog.kDialogY;
					this.tailPosition = Dialog.TailPosition.Center;
					goto IL_5B5;
				}
				num = (Single)Dialog.kCenterX;
				num2 = (Single)Dialog.kCenterY;
				this.tailPosition = Dialog.TailPosition.Center;
				IL_5B5:
				Single x = num - this.size.x / 2f;
				Single y = UIManager.UIContentSize.y - num2 - this.size.y / 2f;
				this.ff9Position = new Vector2(x, y);
				if (this.windowStyle == Dialog.WindowStyle.WindowStylePlain && this.capType != Dialog.CaptionType.None)
				{
					this.tailPosition = Dialog.TailPosition.Center;
				}
			}
			else
			{
				this.ff9Position = new Vector2(num, num2);
				num += this.size.x / 2f;
				num2 = UIManager.UIContentSize.y - num2 - this.size.y / 2f;
			}
			this.HideUnusedSprite();
			this.setAutoPosition(num, num2);
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
		Dialog.WindowStyle windowStyle = this.windowStyle;
		if (windowStyle == Dialog.WindowStyle.WindowStyleTransparent || windowStyle == Dialog.WindowStyle.WindowStyleNoTail)
		{
			Single num = (Single)this.tailSprite.height;
			currentY += ((!isTailUpper) ? num : (-num));
		}
		return currentY;
	}

	private void FollowTarget()
	{
		Single num;
		Single num2;
		ETb.GetMesPos(this.Po, out num, out num2);
		Single num3 = num + Dialog.PosXOffset;
		Single num4 = num2 + Dialog.PosYOffset;
        Boolean flag = ((TailPosition)((Int32)this.tailPosition >> 1) & Dialog.TailPosition.LowerLeft) == Dialog.TailPosition.LowerLeft;
        Boolean flag2 = (this.tailPosition & Dialog.TailPosition.LowerLeft) == Dialog.TailPosition.LowerLeft;
		num3 = this.setPositionX(num3, this.size.x, flag2, true);
		num4 = this.forceSetPositionY(num4, this.size.y, flag);
		Dialog.CalculateDialogCenter(ref num3, ref num4, this.ClipSize);
		this.tailMargin -= num3;
		this.tailPosition = (Dialog.TailPosition)(Convert.ToInt32(flag) << 1 | Convert.ToInt32(flag2));
		num4 = this.CalculateYPositionAfterHideTail(num4, flag);
		this.setAutoPosition(num3, num4);
		this.setTailAutoPosition(this.tailPosition, true);
	}

	private Boolean IsAutoPositionMode()
	{
		return this.targetPos != null && this.targetPos.go != (UnityEngine.Object)null && this.windowStyle != Dialog.WindowStyle.WindowStylePlain;
	}

	private static void CalculateDialogCenter(ref Single x0, ref Single y0, Vector2 ClipSize)
	{
		x0 += ClipSize.x / 2f;
		y0 += ClipSize.y / 2f;
		y0 = UIManager.UIContentSize.y - y0;
	}

	private Single setPositionX(Single posX, Single width, Boolean isLeft, Boolean isUpdate)
	{
		Single num;
		Single num2;
		Single num3;
		if (isLeft)
		{
			num = posX - (Single)Dialog.DialogTailLeftRightOffset;
			if (num < (Single)Dialog.DialogLimitLeft + Dialog.TailMagicNumber1)
			{
				num = (Single)Dialog.DialogLimitLeft + Dialog.TailMagicNumber1;
			}
			if (num > (Single)Dialog.DialogLimitRight - Dialog.TailMagicNumber2)
			{
				num = (Single)Dialog.DialogLimitRight - Dialog.TailMagicNumber2;
			}
			num2 = num + Dialog.TailMagicNumber2 - width;
			num3 = num - Dialog.TailMagicNumber1;
		}
		else
		{
			num = posX + (Single)Dialog.DialogTailLeftRightOffset;
			if (num > (Single)Dialog.DialogLimitRight - Dialog.TailMagicNumber1)
			{
				num = (Single)Dialog.DialogLimitRight - Dialog.TailMagicNumber1;
			}
			if (num < (Single)Dialog.DialogLimitLeft + Dialog.TailMagicNumber2)
			{
				num = (Single)Dialog.DialogLimitLeft + Dialog.TailMagicNumber2;
			}
			num2 = num + Dialog.TailMagicNumber1 - width;
			num3 = num - Dialog.TailMagicNumber2;
		}
		this.tailMargin = num;
		if (num2 < (Single)Dialog.DialogLimitLeft)
		{
			num2 = (Single)Dialog.DialogLimitLeft;
		}
		if (num3 > (Single)Dialog.DialogLimitRight - width)
		{
			num3 = (Single)Dialog.DialogLimitRight - width;
		}
		return (num2 + num3) / 2f;
	}

	private Single setPositionY(Single posY, Single height, ref Boolean isUpper)
	{
		Single num;
		if (isUpper)
		{
			num = posY - height - Dialog.kUpperOffset;
			if (!this.isForceTailPosition && num < Dialog.kLimitTop)
			{
				isUpper ^= true;
				num = posY + Dialog.kLowerOffset;
			}
		}
		else
		{
			num = posY + Dialog.kLowerOffset;
			if (!this.isForceTailPosition && num > Dialog.kLimitBottom - height)
			{
				isUpper ^= true;
				num = posY - height - Dialog.kUpperOffset;
			}
		}
		if (num < Dialog.kLimitTop)
		{
			num = Dialog.kLimitTop;
		}
		else if (num > Dialog.kLimitBottom - height)
		{
			num = Dialog.kLimitBottom - height;
		}
		return num;
	}

	private Single forceSetPositionY(Single posY, Single height, Boolean isUpper)
	{
		posY = ((!isUpper) ? (posY + Dialog.kLowerOffset) : (posY - this.size.y - Dialog.kUpperOffset));
		if (posY < Dialog.kLimitTop)
		{
			posY = Dialog.kLimitTop;
		}
		else if (posY > Dialog.kLimitBottom - this.size.y)
		{
			posY = Dialog.kLimitBottom - this.size.y;
		}
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
		this.Po = (PosObj)null;
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
		this.captionLabel.text = this.caption;
		this.captionWidth = 0f;
		this.lineNumber = 0;
		this.id = -1;
		this.textId = -1;
		this.endMode = -1;
		this.signalMode = 0;
		this.signalNumber = 0;
		this.dialogAnimator.PhraseTextEffect.Restart();
		base.transform.localPosition = Vector3.zero;
		this.tailPosition = Dialog.TailPosition.AutoPosition;
		this.dialogAnimator.PhraseTextEffect.enabled = false;
		this.phraseLabel.text = String.Empty;
		this.phraseLabel.ImageList.Clear();
		this.ClearIcon();
		this.messageValues.Clear();
		this.messageNeedUpdate = false;
		this.subPage.Clear();
		this.currentPage = 0;
		this.dialogAnimator.Pause = false;
		this.dialogAnimator.ShowWithoutAnimation = false;
		this.ResetChoose();
		this.isOverlayDialog = false;
		this.isOverlayChecked = false;
		this.overlayMessageNumber = -1;
		this.isChoiceReady = false;
	}

	private void setTailAutoPosition(Dialog.TailPosition _position, Boolean isUpdate)
	{
		if (!isUpdate)
		{
			this.setTailPosition(_position);
		}
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

	private void setTailPosition(Dialog.TailPosition _position)
	{
		this.tailPosition = _position;
		UIWidget.Pivot pivot;
		switch (_position)
		{
		case Dialog.TailPosition.LowerRight:
		case Dialog.TailPosition.LowerLeft:
		case Dialog.TailPosition.LowerCenter:
		case Dialog.TailPosition.LowerRightForce:
		case Dialog.TailPosition.LowerLeftForce:
			pivot = UIWidget.Pivot.Top;
			goto IL_5B;
		case Dialog.TailPosition.UpperRight:
		case Dialog.TailPosition.UpperLeft:
		case Dialog.TailPosition.UpperCenter:
		case Dialog.TailPosition.UpperRightForce:
		case Dialog.TailPosition.UpperLeftForce:
			pivot = UIWidget.Pivot.Bottom;
			goto IL_5B;
		}
		pivot = UIWidget.Pivot.Center;
		IL_5B:
		this.bodySprite.pivot = pivot;
		this.borderSprite.pivot = pivot;
		Vector2 v = new Vector2(0f, 0f);
		switch (_position)
		{
		case Dialog.TailPosition.LowerRight:
		case Dialog.TailPosition.LowerLeft:
		case Dialog.TailPosition.LowerCenter:
		case Dialog.TailPosition.LowerRightForce:
		case Dialog.TailPosition.LowerLeftForce:
			v.y = this.size.y / 2f - (Dialog.DialogYPadding / 2f - Dialog.BorderPadding);
			break;
		case Dialog.TailPosition.UpperRight:
		case Dialog.TailPosition.UpperLeft:
		case Dialog.TailPosition.UpperCenter:
		case Dialog.TailPosition.UpperRightForce:
		case Dialog.TailPosition.UpperLeftForce:
			v.y = -this.size.y / 2f + (Dialog.DialogYPadding / 2f - Dialog.BorderPadding);
			break;
		}
		this.BodyGameObject.transform.localPosition = v;
		Vector2 birthPosition = new Vector2(0f, 0f);
		switch (_position)
		{
		case Dialog.TailPosition.LowerRight:
		case Dialog.TailPosition.LowerLeft:
		case Dialog.TailPosition.LowerCenter:
		case Dialog.TailPosition.LowerRightForce:
		case Dialog.TailPosition.LowerLeftForce:
			birthPosition = new Vector2(this.size.x / 2f, this.size.y);
			goto IL_205;
		case Dialog.TailPosition.UpperRight:
		case Dialog.TailPosition.UpperLeft:
		case Dialog.TailPosition.UpperCenter:
		case Dialog.TailPosition.UpperRightForce:
		case Dialog.TailPosition.UpperLeftForce:
			birthPosition = new Vector2(this.size.x / 2f, 0f);
			goto IL_205;
		}
		birthPosition = new Vector2(this.size.x / 2f, this.size.y / 2f);
		IL_205:
		this.bodySprite.birthPosition = birthPosition;
		Vector2 v2 = this.phraseWidget.transform.localPosition;
		v2.y = (Single)(this.phraseWidget.height / 2);
		switch (_position)
		{
		case Dialog.TailPosition.LowerRight:
		case Dialog.TailPosition.LowerLeft:
		case Dialog.TailPosition.LowerCenter:
		case Dialog.TailPosition.LowerRightForce:
		case Dialog.TailPosition.LowerLeftForce:
			v2.y -= 36f;
			goto IL_2C3;
		case Dialog.TailPosition.UpperRight:
		case Dialog.TailPosition.UpperLeft:
		case Dialog.TailPosition.UpperCenter:
		case Dialog.TailPosition.UpperRightForce:
		case Dialog.TailPosition.UpperLeftForce:
			v2.y += 6f;
			goto IL_2C3;
		}
		v2.y -= 18f;
		IL_2C3:
		this.phraseWidget.transform.localPosition = v2;
		UIWidget.Pivot pivot2;
		switch (_position)
		{
		case Dialog.TailPosition.LowerRight:
		case Dialog.TailPosition.LowerRightForce:
			pivot2 = UIWidget.Pivot.TopLeft;
			goto IL_350;
		case Dialog.TailPosition.LowerLeft:
		case Dialog.TailPosition.LowerLeftForce:
			pivot2 = UIWidget.Pivot.TopRight;
			goto IL_350;
		case Dialog.TailPosition.UpperRight:
		case Dialog.TailPosition.UpperRightForce:
			pivot2 = UIWidget.Pivot.BottomLeft;
			goto IL_350;
		case Dialog.TailPosition.UpperLeft:
		case Dialog.TailPosition.UpperLeftForce:
			pivot2 = UIWidget.Pivot.BottomRight;
			goto IL_350;
		case Dialog.TailPosition.LowerCenter:
			pivot2 = UIWidget.Pivot.Top;
			goto IL_350;
		case Dialog.TailPosition.UpperCenter:
			pivot2 = UIWidget.Pivot.Bottom;
			goto IL_350;
		}
		pivot2 = UIWidget.Pivot.Center;
		IL_350:
		this.tailSprite.pivot = pivot2;
		switch (_position)
		{
		case Dialog.TailPosition.LowerRight:
		case Dialog.TailPosition.LowerRightForce:
			this.tailSprite.spriteName = "dialog_pointer_topleft";
			goto IL_405;
		case Dialog.TailPosition.LowerLeft:
		case Dialog.TailPosition.LowerCenter:
		case Dialog.TailPosition.LowerLeftForce:
			this.tailSprite.spriteName = "dialog_pointer_topright";
			goto IL_405;
		case Dialog.TailPosition.UpperRight:
		case Dialog.TailPosition.UpperRightForce:
			this.tailSprite.spriteName = "dialog_pointer_downleft";
			goto IL_405;
		case Dialog.TailPosition.UpperLeft:
		case Dialog.TailPosition.UpperCenter:
		case Dialog.TailPosition.UpperLeftForce:
			this.tailSprite.spriteName = "dialog_pointer_downright";
			goto IL_405;
		}
		this.tailSprite.spriteName = String.Empty;
		IL_405:
		Single x = this.tailSprite.localSize.x / 2f;
		Single y;
		switch (_position)
		{
		case Dialog.TailPosition.LowerRight:
		case Dialog.TailPosition.LowerLeft:
		case Dialog.TailPosition.LowerCenter:
		case Dialog.TailPosition.LowerRightForce:
		case Dialog.TailPosition.LowerLeftForce:
			y = this.size.y / 2f + Dialog.DialogYPadding / 2f;
			goto IL_4C2;
		case Dialog.TailPosition.UpperRight:
		case Dialog.TailPosition.UpperLeft:
		case Dialog.TailPosition.UpperCenter:
		case Dialog.TailPosition.UpperRightForce:
		case Dialog.TailPosition.UpperLeftForce:
			y = -(this.size.y / 2f) - Dialog.DialogYPadding / 2f + 1f;
			goto IL_4C2;
		}
		y = 0f;
		IL_4C2:
		this.tailTransform.localPosition = new Vector3(x, y, 0f);
	}

	private void Update()
	{
		this.UpdatePosition();
		this.UpdateMessageValue();
	}

	private void StartSignalProcess()
	{
		if (this.typeAnimationEffect && this.signalMode != 0)
		{
			Singleton<DialogManager>.Instance.StartSignalProcess(this.phraseLabel, this.phrase, this.signalNumber, this.messageSpeed, this.messageWait);
		}
	}

	public void SetMessageSpeed(Int32 speed, Int32 index)
	{
		if (speed != -1)
		{
			this.messageSpeed[index] = speed * Dialog.FF9TextSpeedRatio;
		}
		else
		{
			this.messageSpeed[index] = (Int32)Dialog.DialogTextAnimationTick[(Int32)(checked((IntPtr)FF9StateSystem.Settings.cfg.fld_msg))] * Dialog.FF9TextSpeedRatio;
		}
	}

	public void SetMessageWait(Int32 ff9Frames, Int32 index)
	{
		this.messageWait[index] = (Single)ff9Frames / 30f;
	}

	public void OnCharacterShown(GameObject go, Int32 index)
	{
		for (Int32 i = 0; i < this.ImageList.Count; i++)
		{
			Dialog.DialogImage dialogImage = this.imageList[i];
			if (index == dialogImage.TextPosition && !dialogImage.IsShown)
			{
				this.phraseLabel.Update();
				base.StartCoroutine("ShowIconProcess", i);
				dialogImage.IsShown = true;
				break;
			}
		}
	}

	private IEnumerator ShowIconProcess(Int32 index)
	{
		if (this.phraseLabel.ImageList == null)
		{
			yield break;
		}
		while (this.phraseLabel.ImageList.size == 0)
		{
			yield return new WaitForEndOfFrame();
		}
		GameObject iconObject = Singleton<BitmapIconManager>.Instance.InsertBitmapIcon(this.phraseLabel.ImageList[index], this);
		base.StartCoroutine("SetIconDepth", iconObject);
		yield break;
	}

	public void ShowAllIcon()
	{
		base.StartCoroutine("ShowAllIconProcess");
	}

	private IEnumerator ShowAllIconProcess()
	{
		yield return new WaitForEndOfFrame();
		if (this.phraseLabel.ImageList == null)
		{
			yield return new WaitForEndOfFrame();
		}
		if (this.phraseLabel.ImageList.size == 0)
		{
			yield return new WaitForEndOfFrame();
		}
		Int32 index = 0;
		foreach (Dialog.DialogImage image in this.phraseLabel.ImageList)
		{
			if (!this.imageList[index].IsShown && image != null)
			{
				GameObject iconObject = Singleton<BitmapIconManager>.Instance.InsertBitmapIcon(image, this);
				base.StartCoroutine("SetIconDepth", iconObject);
				this.imageList[index].IsShown = true;
			}
			index++;
		}
		yield break;
	}

	private IEnumerator SetIconDepth(GameObject iconObject)
	{
		yield return new WaitForEndOfFrame();
		NGUIText.SetIconDepth(this.phraseLabel.gameObject, iconObject, true);
		yield break;
	}

	private void UpdateMessageValue()
	{
		if (this.currentState == Dialog.State.CompleteAnimation && this.messageNeedUpdate && !this.IsOverlayDialog && (this.HasMessageValueChanged() || this.HasOverlayChanged()))
		{
			this.ReplaceMessageValue();
		}
	}

	private Boolean HasMessageValueChanged()
	{
		Boolean result = false;
		foreach (KeyValuePair<Int32, Int32> keyValuePair in this.messageValues)
		{
			if (PersistenSingleton<EventEngine>.Instance.eTb.gMesValue[keyValuePair.Key] != keyValuePair.Value)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private void ReplaceMessageValue()
	{
		Int32[] gMesValue = PersistenSingleton<EventEngine>.Instance.eTb.gMesValue;
		String text = this.phraseMessageValue;
		String formattedValue = String.Empty;
		for (Int32 i = 0; i < (Int32)gMesValue.Length; i++)
		{
			if (this.messageValues.ContainsKey(i))
			{
				formattedValue = gMesValue[i].ToString();
				if (this.overlayMessageNumber == i)
					formattedValue = NGUIText.FF9PinkColor + formattedValue + NGUIText.FF9WhiteColor;

                text = text.ReplaceAll(
                new[]
                {
                    new KeyValuePair<String, TextReplacement>($"[NUMB={i}]", formattedValue),
                    new KeyValuePair<String, TextReplacement>($"{{Variable {i}}}", formattedValue)
                });

				this.messageValues[i] = gMesValue[i];
			}
		}
		this.phraseLabel.text = text;
	}

	private Boolean HasOverlayChanged()
	{
		if (this.IsOverlayDialog)
		{
			return false;
		}
		Dialog overlayDialog = Singleton<DialogManager>.Instance.GetOverlayDialog();
		Int32 num = -1;
		if (overlayDialog != (UnityEngine.Object)null)
		{
			using (Dictionary<Int32, Int32>.Enumerator enumerator = overlayDialog.MessageValues.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					KeyValuePair<Int32, Int32> keyValuePair = enumerator.Current;
					num = keyValuePair.Key;
				}
			}
		}
		if (num != this.overlayMessageNumber)
		{
			this.overlayMessageNumber = num;
			return true;
		}
		return false;
	}

	public void UpdatePosition()
	{
		if (this.isReadyToFollow && this.focusToActor)
		{
			if (this.IsAutoPositionMode())
			{
				this.FollowTarget();
			}
			else
			{
				this.isReadyToFollow = false;
			}
		}
	}

	public void PauseDialog(Boolean isPause)
	{
		if (isPause)
		{
			if (this.currentState != Dialog.State.CompleteAnimation)
			{
				this.dialogAnimator.Pause = true;
			}
			if (this.currentState == Dialog.State.TextAnimation)
			{
				this.phraseEffect.Pause();
			}
		}
		else
		{
			if (this.currentState != Dialog.State.CompleteAnimation)
			{
				this.dialogAnimator.Pause = false;
			}
			if (this.currentState == Dialog.State.TextAnimation)
			{
				this.phraseEffect.Resume();
			}
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
		{
			this.Hide();
		}
	}

	private void PrepareNextPage()
	{
		this.phrase = ((this.subPage.Count <= this.currentPage) ? String.Empty : this.subPage[this.currentPage++]);
		this.phrase = this.RewriteSentenceForExclamation(this.phrase);
		this.ClearIcon();
        DialogBoxController.PhraseOpcodeSymbol(this.phrase, this);
		this.phraseMessageValue = this.phrase;
		this.phrase = NGUIText.ReplaceNumberValue(this.phrase, this);
	}

	private String RewriteSentenceForExclamation(String inputPharse)
	{
		if (inputPharse.Length > 0 && this.lineNumber == 1)
		{
			String text = NGUIText.StripSymbols(inputPharse);
			if (text.Length < 6 && text.Contains("!"))
			{
				inputPharse = "[CENT]" + inputPharse;
			}
		}
		return inputPharse;
	}

	private void ClearIcon()
	{
		for (Int32 i = this.phraseLabel.transform.childCount - 1; i >= 0; i--)
		{
			GameObject gameObject = this.phraseLabel.transform.GetChild(i).gameObject;
			gameObject.SetActive(false);
			Singleton<BitmapIconManager>.Instance.RemoveBitmapIcon(gameObject);
		}
		if (this.phraseLabel.ImageList != null)
		{
			this.phraseLabel.ImageList.Clear();
		}
		this.imageList.Clear();
	}

	private void AutomaticWidth()
	{
		if (this.CanResizeWidth())
		{
			Int32 width = this.phraseLabel.width;
			Int32 height = this.phraseLabel.height;
			this.phraseLabel.width = (Int32)UIManager.UIContentSize.x;
			this.phraseLabel.height = (Int32)UIManager.UIContentSize.y;
			this.phraseLabel.ProcessText();
			this.phraseLabel.UpdateNGUIText();
			Single num = 0f;
			foreach (String text in this.subPage)
			{
				String text2 = text;
				if (this.messageNeedUpdate)
				{
					text2 = NGUIText.ReplaceNumberValue(text2, this);
				}
				Single num2 = (NGUIText.CalculatePrintedSize2(text2).x + Dialog.DialogPhraseXPadding * 2f) / UIManager.ResourceXMultipier;
				if (num2 > num)
				{
					num = num2;
				}
			}
			if (num < this.originalWidth / 2f)
			{
				num = this.originalWidth - 1f;
			}
			else
			{
				foreach (Dialog.DialogImage dialogImage in this.imageList)
				{
					if (dialogImage.Id == 27)
					{
						num += 8f;
					}
				}
			}
			Single num3 = Dialog.DialogPhraseXPadding * 2f / UIManager.ResourceXMultipier;
			if (num < this.captionWidth + num3)
			{
				num = this.captionWidth + num3;
			}
			this.Width = num + 1f;
			this.phraseLabel.height = height;
		}
	}

	private Boolean CanResizeWidth()
	{
		if (this.id == 9)
		{
			return false;
		}
		if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
		{
			Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
			if (fldMapNo >= 1400 && fldMapNo <= 1425 && (this.tailPosition == Dialog.TailPosition.UpperLeft || this.tailPosition == Dialog.TailPosition.UpperRight))
			{
				if (this.windowStyle == Dialog.WindowStyle.WindowStyleTransparent)
				{
					return false;
				}
				if (this.windowStyle == Dialog.WindowStyle.WindowStylePlain && this.startChoiceRow == -1)
				{
					return false;
				}
			}
			if (EventHUD.CurrentHUD == MinigameHUD.Auction && this.windowStyle == Dialog.WindowStyle.WindowStyleTransparent)
			{
				return false;
			}
			if (EventHUD.CurrentHUD == MinigameHUD.ChocoHot && this.windowStyle == Dialog.WindowStyle.WindowStylePlain)
			{
				String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
			    switch (currentLanguage)
			    {
                    case "Japanese":
                    case "English(UK)":
                    case "English(US)":
                        return this.textId != 275;
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
			Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
			if (fldMapNo == 1608)
			{
				Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
				if (varManually == 6840)
				{
					FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
					Camera mainCamera = fieldmap.GetMainCamera();
					Vector3 localPosition = mainCamera.transform.localPosition;
					result = (localPosition.x == 32f && localPosition.y == 0f && localPosition.z == 0f && this.targetPos.sid != 11 && this.targetPos.sid != 9);
				}
				else if (varManually == 6850)
				{
					result = (this.targetPos.sid != 9);
				}
			}
		}
		return result;
	}

	private void OverwriteDialogParameter()
	{
		if (FF9StateSystem.MobilePlatform && this.id < 9)
		{
			Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
			Int32 num = fldMapNo;
			if (num == 2951)
			{
				if (this.targetPos != null && this.targetPos.uid == 13 && this.startChoiceRow > -1)
				{
					this.Po = (PosObj)null;
					this.windowStyle = Dialog.WindowStyle.WindowStyleNoTail;
					this.tailPosition = Dialog.TailPosition.UpperCenter;
				}
			}
		}
	}

	private String OverwritePrerenderText(String text)
	{
		if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.FieldHUD && FF9TextTool.FieldZoneId == 2 && this.textId > 0)
		{
			text = TextOpCodeModifier.ReplaceChanbaraArrow(text);
		}
		return text;
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

	public static String DialogGroupButton;

	public static Vector2 DefaultOffset;

	[SerializeField]
	private List<Single> choiceYPositions;

	private Boolean isChoiceReady;

	public static Byte DialogMaximumDepth = 68;

	public static Byte DialogAdditionalRaiseDepth = 22;

	private static Byte[] DialogTextAnimationTick = new Byte[]
	{
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

	public static readonly Int32 DialogLimitRight = (Int32)(UIManager.ResourceXMultipier * 312f);

	public static readonly Int32 DialogLimitTop = (Int32)(UIManager.ResourceYMultipier * 8f);

	public static readonly Int32 DialogLimitBottom = (Int32)(UIManager.ResourceYMultipier * 208f);

	public static readonly Int32 DialogTailLeftRightOffset = (Int32)(UIManager.ResourceXMultipier * 6f);

	public static readonly Int32 DialogTailTopOffset = (Int32)(UIManager.ResourceYMultipier * 16f);

	public static readonly Int32 DialogTailBottomOffset = (Int32)(UIManager.ResourceYMultipier * 32f);

	public static readonly Int32 kCenterX = (Int32)(UIManager.ResourceXMultipier * FieldMap.HalfScreenWidth);

	public static readonly Int32 kCenterY = (Int32)(UIManager.ResourceYMultipier * FieldMap.HalfScreenHeight);

	public static readonly Int32 kDialogY = (Int32)(UIManager.ResourceYMultipier * 150f);

	public static readonly Single TailMagicNumber1 = (Single)((Int32)(24f * UIManager.ResourceXMultipier));

	public static readonly Single TailMagicNumber2 = (Single)((Int32)(8f * UIManager.ResourceXMultipier));

	public static readonly Single PosXOffset = -2f;

	public static readonly Single kUpperOffset = (Single)Dialog.DialogTailTopOffset;

	public static readonly Single kLimitTop = 24f;

	public static readonly Single kLowerOffset = (Single)(Dialog.DialogTailBottomOffset * 3 / 4);

	public static readonly Single kLimitBottom = (Single)(Dialog.DialogLimitBottom - 4);

	public static readonly Single kMargin = UIManager.ResourceXMultipier * 5f;

	public static readonly Single PosYOffset = -15f;

	public static readonly Single InitialMagicNum = Mathf.Ceil(32f * UIManager.ResourceXMultipier);

	public static readonly Single DialogLineHeight = 68f;

	public static readonly Single DialogYPadding = 80f;

	public static readonly Single DialogXPadding = 18f;

	public static readonly Single BorderPadding = 18f;

	public static readonly Single DialogPhraseXPadding = 16f;

	public static readonly Single DialogPhraseYPadding = 20f;

	public static readonly Int32 FF9TextSpeedRatio = 2;

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

	private Int32 lineNumber;

	private Vector2 size = Vector2.zero;

	[SerializeField]
	private Vector2 position = Vector2.zero;

	[SerializeField]
	private Vector3 offset = Vector3.zero;

	[SerializeField]
	private String phrase = String.Empty;

	private String phraseMessageValue = String.Empty;

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

	private Boolean isMuteSelectSound;

	private Boolean typeAnimationEffect = true;

	private Dictionary<Int32, Int32> messageSpeed = new Dictionary<Int32, Int32>();

	private Dictionary<Int32, Single> messageWait = new Dictionary<Int32, Single>();

	[SerializeField]
	private List<Dialog.DialogImage> imageList = new List<Dialog.DialogImage>();

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

	private Int32 signalNumber;

	private Int32 signalMode;

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

	public class DialogImage
	{
		public Int32 Id;

		public Vector2 Size;

		public Int32 TextPosition;

		public Int32 PrintedLine;

		public Vector3 LocalPosition;

		public Vector3 Offset;

		public Boolean IsShown;

		public Boolean checkFromConfig = true;

		public Boolean IsButton = true;

		public String tag = String.Empty;
	}

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
}
