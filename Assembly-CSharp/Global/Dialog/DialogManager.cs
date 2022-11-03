using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;
using UnityEngine;
using Assets.Scripts.Common;
using Memoria;
using Memoria.Assets;
using Memoria.Scenes;
using Memoria.Scripts;
using Object = System.Object;

public class DialogManager : Singleton<DialogManager>
{
	public void StartSignalProcess(UILabel label, String fullText, Int32 targetSignal, Dictionary<Int32, Int32> dynamicCharsPerSecond, Dictionary<Int32, Single> waitList)
	{
		base.StartCoroutine(this.SignalProcess(label, fullText, targetSignal, dynamicCharsPerSecond, waitList));
	}

	private IEnumerator SignalProcess(UILabel label, String fullText, Int32 targetSignal, Dictionary<Int32, Int32> dynamicCharsPerSecond, Dictionary<Int32, Single> waitList)
	{
		Int32 currentOffset = 0;
		Int32 length = fullText.Length;
		Single nextCharTime = 0f;
		Int32 ff9Signal = 0;
		Dialog.DialogImage insertImage = (Dialog.DialogImage)null;
		Single charsPerSecond = 0f;
		while (currentOffset < length)
		{
			while (nextCharTime <= RealTime.time)
			{
				charsPerSecond = ((!dynamicCharsPerSecond.ContainsKey(currentOffset)) ? Mathf.Max(1f, charsPerSecond) : ((Single)dynamicCharsPerSecond[currentOffset]));
				if (waitList.ContainsKey(currentOffset) && waitList[currentOffset] > 0f)
				{
					Int32 key2;
					Int32 key = key2 = currentOffset;
					Single num = waitList[key2];
					waitList[key] = num - ((!HonoBehaviorSystem.Instance.IsFastForwardModeActive()) ? Time.deltaTime : (Time.deltaTime * (Single)FF9StateSystem.Settings.FastForwardFactor));
					break;
				}
				if (label.supportEncoding)
				{
					while (NGUIText.ParseSymbol(fullText, ref currentOffset, ref ff9Signal, ref insertImage))
					{
					}
				}
				currentOffset++;
				Single delay = (!HonoBehaviorSystem.Instance.IsFastForwardModeActive()) ? (1f / charsPerSecond) : (1f / (charsPerSecond * (Single)FF9StateSystem.Settings.FastForwardFactor));
				if (nextCharTime == 0f || nextCharTime + delay > RealTime.time)
				{
					nextCharTime = RealTime.time + delay;
				}
				else
				{
					nextCharTime += delay;
				}
				NGUIText.ProcessFF9Signal(ref ff9Signal);
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	public static Int32 SelectChoice
	{
		get
		{
			return DialogManager.selectChoice;
		}
		set
		{
			DialogManager.selectChoice = value;
		}
	}

	public Queue<Dialog> DialogPool
	{
		get
		{
			return this.dialogPool;
		}
	}

	public List<Dialog> ActiveDialogList
	{
		get
		{
			return this.activeDialogList;
		}
	}

	public Camera CurrentUICamera
	{
		get
		{
			if (this.uiCamera == (UnityEngine.Object)null)
			{
				this.uiCamera = base.transform.parent.FindChild("Camera").GetComponent<Camera>();
			}
			return this.uiCamera;
		}
	}

	public UIPanel PointerPanel
	{
		get
		{
			if (this.pointerPanel == (UnityEngine.Object)null)
			{
				this.pointerPanel = base.transform.parent.FindChild("Pointer Container").GetComponent<UIPanel>();
			}
			return this.pointerPanel;
		}
	}

	public Boolean Activate
	{
		get
		{
			return this.isActivate;
		}
	}

	public Boolean Visible
	{
		get
		{
			return this.activeDialogList.Count<Dialog>() > 0;
		}
	}

	public Boolean CompletlyVisible
	{
		get
		{
			foreach (Dialog dialog in this.activeDialogList)
			{
				if (dialog.CurrentState != Dialog.State.CompleteAnimation)
				{
					return false;
				}
			}
			return this.Visible;
		}
	}

	public UIWidget Widget
	{
		get
		{
			return this.widget;
		}
	}

	public Boolean HasChocoboMenu
	{
		get
		{
			return EventCollision.IsRidingChocobo() && (this.GetChoiceDialog() != (UnityEngine.Object)null || this.IsDialogNeedControl());
		}
	}

    public int CurMesId
    {
        get { return this.curMesId; }
        set { this.curMesId = value; }
    }

    public int PressMesId
    {
        get { return this.pressMesId; }
        set { this.pressMesId = value; }
    }

    public int ReleaseMesId
    {
        get { return this.releaseMesId; }
        set { this.releaseMesId = value; }
    }

    public Dialog AttachDialog(String phrase, Int32 width, Int32 lineCount, Dialog.TailPosition tailPos, Dialog.WindowStyle style, Vector2 pos, Dialog.CaptionType captionType = Dialog.CaptionType.None)
	{
		Dialog dialogFromPool = this.GetDialogFromPool();
		if (dialogFromPool != (UnityEngine.Object)null)
		{
			dialogFromPool.Reset();
			dialogFromPool.Width = (Single)width;
			dialogFromPool.LineNumber = (Single)lineCount;
			dialogFromPool.Style = style;
			dialogFromPool.Tail = tailPos;
			dialogFromPool.Position = pos;
			dialogFromPool.Id = DialogManager.UIDialogId;
			dialogFromPool.Caption = FF9TextTool.GetDialogCaptionText(captionType);
			dialogFromPool.Phrase = phrase;
			dialogFromPool.Show();
			if (!this.isActivate)
			{
				this.ActivateDialogScene();
			}
		}
		return dialogFromPool;
	}

	public Dialog AttachDialog(Int32 dialogId, Dialog.WindowStyle style, Int32 textId, PosObj po, Dialog.DialogIntDelegate listener, Dialog.CaptionType captionType)
	{
		Dialog dialogFromPool = this.GetDialogFromPool();
		if (dialogFromPool != null)
		{
			dialogFromPool.Reset();
			dialogFromPool.Id = dialogId;
			dialogFromPool.Style = style;
			dialogFromPool.Po = po;
			dialogFromPool.TextId = textId;
			dialogFromPool.Caption = FF9TextTool.GetDialogCaptionText(captionType);
			dialogFromPool.CapType = captionType;
			if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Battle)
			{
				if (FF9TextTool.IsBattleTextLoaded)
				{
					dialogFromPool.Phrase = FF9TextTool.BattleText(textId);
				}
				else
				{
					dialogFromPool.Phrase = String.Empty;
					dialogFromPool.Style = Dialog.WindowStyle.WindowStyleTransparent;
				}
			}
			else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Field || PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.World)
			{
				dialogFromPool.Phrase = FF9TextTool.FieldText(textId);

                // Subscribe
			    Action onFieldTextUpdated = () =>
			    {
                    dialogFromPool.Phrase = FF9TextTool.FieldText(textId);
			        dialogFromPool.Show();
			    };
                FF9TextTool.FieldTextUpdated += onFieldTextUpdated;

                // Unsubscribe
                Dialog.DialogIntDelegate unsubscribe = (c) => FF9TextTool.FieldTextUpdated -= onFieldTextUpdated;
                listener = (Dialog.DialogIntDelegate)Delegate.Combine(unsubscribe, listener);
            }
            dialogFromPool.Show();
			dialogFromPool.AfterDialogHidden = listener;
			if (!this.isActivate)
			{
				this.ActivateDialogScene();
			}
		}
		return dialogFromPool;
	}

	public Boolean CheckDialogOverlap(Dialog dialog)
	{
		foreach (Dialog dialog2 in this.activeDialogList)
		{
			if (dialog2.gameObject.activeSelf)
			{
				Dialog dialog3 = dialog2;
				if (dialog3.IsActive)
				{
					Boolean flag = dialog.FF9Position.x < dialog3.FF9Position.x + dialog3.Size.x && dialog.FF9Position.x + dialog.Size.x > dialog3.FF9Position.x;
					Boolean flag2 = dialog.FF9Position.y < dialog3.FF9Position.y + dialog3.Size.y && dialog.FF9Position.y + dialog.Size.y > dialog3.FF9Position.y;
					if (flag && flag2)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public Boolean CheckDialogShowing(Int32 dialogId)
	{
		foreach (Dialog dialog in this.activeDialogList)
		{
			if (dialog.Id == dialogId)
			{
				return true;
			}
		}
		return false;
	}

	public Dialog GetDialogFromPool()
	{
		Dialog dialog = (Dialog)null;
		try
		{

			if (this.DialogPool.Count > 0)
			{
				dialog = this.DialogPool.Dequeue();
			}
			else
			{
				dialog = NGUITools.AddChild(base.gameObject, this.DialogPrefab).GetComponent<Dialog>();
			}
		}
		catch
		{
		}
		if (dialog == (UnityEngine.Object)null)
		{
			return dialog;
		}
		this.activeDialogList.Add(dialog);
		dialog.gameObject.SetActive(true);
		return dialog;
	}

	public void Close(Int32 dialogId)
	{
		foreach (Dialog dialog in this.activeDialogList.ToList()) // Copy the list in case closing a dialog immediately releases it
		{
			if (dialog.gameObject.activeInHierarchy && dialog.Id == dialogId)
			{
				dialog.ForceClose();
				if (FF9StateSystem.Common.FF9.fldMapNo == 100) // Alexandria/Main Street
				{
					Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(8149);
					if (varManually == 3) // When Puck hits Vivi
						dialog.AfterHidden();
				}
			}
		}
	}

	public void CloseAll()
	{
		foreach (Dialog dialog in this.activeDialogList.ToList())
			if (dialog.gameObject.activeInHierarchy)
				dialog.ForceClose();
	}

	public void ShowChoiceHud()
	{
		if (FF9StateSystem.MobilePlatform && (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.FieldHUD || PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.Pause || PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD))
		{
			Singleton<BubbleUI>.Instance.HideAllHud();
			if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD)
			{
				UIManager.World.SetChocoboHudVisible(false);
				UIManager.World.SetPlaneHudVisible(false);
			}
			this.DialogChoiceConfirmHud.SetActive(true);
			this.DialogChoiceCancelHud.SetActive(true);
			this.isHudActive = true;
		}
	}

	public void HideChoiceHud()
	{
		if (FF9StateSystem.MobilePlatform)
		{
			Singleton<BubbleUI>.Instance.ShowAllHud();
			if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD)
			{
				UIManager.World.SetChocoboHudVisible(true);
				UIManager.World.SetPlaneHudVisible(true);
			}
			this.DialogChoiceConfirmHud.SetActive(false);
			this.DialogChoiceCancelHud.SetActive(false);
			this.isHudActive = false;
		}
	}

	public void ReleaseDialogToPool(Dialog dialog)
	{
		dialog.gameObject.SetActive(false);
		this.activeDialogList.Remove(dialog);
		this.dialogPool.Enqueue(dialog);
		this.DeactivateDialogScene();
	}

	public void ReleaseAllDialogsToPool()
	{
		foreach (Dialog dialog in this.activeDialogList)
		{
			dialog.gameObject.SetActive(false);
			dialog.Reset();
			this.dialogPool.Enqueue(dialog);
		}
		this.activeDialogList.Clear();
		this.DeactivateDialogScene();
	}

	protected override void Awake()
	{
		base.Awake();
		this.isActivate = false;
		this.dialogPool = new Queue<Dialog>();
		this.activeDialogList = new List<Dialog>();
	}

	private void Start()
	{
		ButtonGroupState.SetPointerOffsetToGroup(Dialog.DefaultOffset, Dialog.DialogGroupButton);
		this.widget = base.GetComponent<UIWidget>();
		this.PreloadDialog();
	}

	private void PreloadDialog()
	{
		for (Int32 i = 0; i < (Int32)(DialogManager.MaximumDialogCount - 1); i++)
		{
			this.AttachDialog("[STRT=10,1][IMME]Load[TIME=1]", 10, 1, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStyleTransparent, new Vector2(10000f, 10000f), Dialog.CaptionType.None);
		}
	}

	private void OnClick()
	{
		if (this.isHudActive)
		{
			return;
		}
		if (this.GetChoiceDialog() == (UnityEngine.Object)null)
		{
			this.OnKeyConfirm(base.gameObject);
			EventInput.RecieveDialogConfirm();
		}
	}

	public void OnDrag(Vector2 delta)
	{
		if (PersistenSingleton<UIManager>.Instance.IsPause)
		{
			return;
		}
		Dialog choiceDialog = this.GetChoiceDialog();
		if (choiceDialog != (UnityEngine.Object)null)
		{
			if (delta.y < -50f)
			{
				choiceDialog.SetCurrentChoiceRef(1);
			}
			else if (delta.y > 50f)
			{
				choiceDialog.SetCurrentChoiceRef(-1);
			}
		}
	}

	public void OnKeyConfirm(GameObject go)
	{
		if (PersistenSingleton<UIManager>.Instance.IsPause)
			return;
		foreach (Dialog dialog in this.activeDialogList.ToList())
			dialog.OnKeyConfirm(go);
	}

	public void OnKeyCancel(GameObject go)
	{
		if (PersistenSingleton<UIManager>.Instance.IsPause)
			return;
		foreach (Dialog dialog in this.activeDialogList.ToList())
			dialog.OnKeyCancel(go);
	}

	public void OnItemSelect(GameObject go)
	{
		if (PersistenSingleton<UIManager>.Instance.IsPause)
		{
			return;
		}
		Dialog choiceDialog = this.GetChoiceDialog();
		if (choiceDialog != (UnityEngine.Object)null)
		{
			choiceDialog.OnItemSelect(go);
		}
	}

	private void ActivateDialogScene()
	{
		Boolean flag = true;
		if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD)
		{
			ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[(Int32)ff9.w_moveActorPtr.originalActor.index];
			if (ff9.m_GetIDEvent(s_moveCHRStatus.id) != 0 && UIManager.World.CurrentState != WorldHUD.State.FullMap && !this.HasChocoboMenu && !ff9.w_isMogActive)
			{
				Boolean flag2 = false;
				foreach (Dialog dialog in this.activeDialogList)
				{
					if (dialog.Id == DialogManager.UIDialogId)
					{
						flag2 = true;
						break;
					}
				}
				flag = flag2;
			}
		}
		if (flag)
		{
			base.gameObject.GetComponent<BoxCollider>().enabled = true;
			this.isActivate = true;
		}
	}

	private void DeactivateDialogScene()
	{
		Boolean flag = this.activeDialogList.Count == 0;
		if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD)
		{
			ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[(Int32)ff9.w_moveActorPtr.originalActor.index];
			if (ff9.m_GetIDEvent(s_moveCHRStatus.id) != 0 && UIManager.World.CurrentState != WorldHUD.State.FullMap && !this.HasChocoboMenu && !ff9.w_isMogActive)
			{
				flag = true;
			}
		}
		if (flag)
		{
			this.DialogChoiceConfirmHud.SetActive(false);
			this.DialogChoiceCancelHud.SetActive(false);
			base.gameObject.GetComponent<BoxCollider>().enabled = false;
			this.isHudActive = false;
			this.isActivate = false;
		}
	}

	public void PauseAllDialog(Boolean isPause)
	{
		foreach (Dialog dialog in this.activeDialogList)
		{
			dialog.PauseDialog(isPause);
		}
	}

	public Dialog GetDialogByWindowID(Int32 follow)
	{
		foreach (Dialog dialog in this.activeDialogList)
		{
			if (dialog.Id == follow)
			{
				return dialog;
			}
		}
		return (Dialog)null;
	}

	public Dialog GetChoiceDialog()
	{
		foreach (Dialog dialog in this.activeDialogList)
		{
			if (dialog.StartChoiceRow > -1)
			{
				return dialog;
			}
		}
		return (Dialog)null;
	}

	public Boolean IsDialogNeedControl()
	{
		foreach (Dialog dialog in this.activeDialogList)
		{
			if (!dialog.FlagButtonInh)
			{
				return true;
			}
		}
		return false;
	}

	public void ForceControlByEvent(Boolean control)
	{
		foreach (Dialog dialog in this.activeDialogList)
		{
			dialog.FlagButtonInh = control;
		}
	}

	public void RiseAll()
	{
		foreach (Dialog dialog in this.activeDialogList)
		{
			if (dialog.Panel.depth < (Int32)(Dialog.DialogAdditionalRaiseDepth + Dialog.DialogMaximumDepth))
			{
				dialog.Panel.depth += (Int32)Dialog.DialogAdditionalRaiseDepth;
				dialog.phrasePanel.depth += (Int32)Dialog.DialogAdditionalRaiseDepth;
			}
		}
	}

	public UILabel GetDialogLabel()
	{
		foreach (Object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			Dialog component = transform.GetComponent<Dialog>();
			if (component != (UnityEngine.Object)null)
			{
				return component.PhraseLabel;
			}
		}
		return (UILabel)null;
	}

	public Dialog GetDialogByTextId(Int32 textId)
	{
		foreach (Dialog dialog in this.activeDialogList)
		{
			if (dialog.TextId == textId)
			{
				return dialog;
			}
		}
		return (Dialog)null;
	}

	public Dialog GetOverlayDialog()
	{
		foreach (Dialog dialog in this.activeDialogList)
		{
			if (dialog.IsOverlayDialog)
			{
				return dialog;
			}
		}
		return (Dialog)null;
	}

	public Dialog GetMognetDialog()
	{
		foreach (Dialog dialog in this.activeDialogList)
		{
			if (dialog.CapType == Dialog.CaptionType.Mognet && dialog.StartChoiceRow > -1)
			{
				return dialog;
			}
		}
		return (Dialog)null;
	}

	public void EnableCollider(Boolean value)
	{
		base.gameObject.GetComponent<BoxCollider>().enabled = value;
	}

	public static Byte MaximumDialogCount = 10;

	private static Int32 selectChoice;

	private static Int32 UIDialogId = 9;

	public GameObject DialogChoiceConfirmHud;

	public GameObject DialogChoiceCancelHud;

	public GameObject DialogPrefab;

	private Queue<Dialog> dialogPool = new Queue<Dialog>();

	private List<Dialog> activeDialogList = new List<Dialog>();

	private Boolean isActivate;

	private Boolean isHudActive;

	private Camera uiCamera;

	private UIPanel pointerPanel;

	private UIWidget widget;

    private int curMesId = -1;

    private int pressMesId = -1;

    private int releaseMesId = -1;
}
