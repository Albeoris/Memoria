using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Assets;
using Memoria.Prime.Text;
using UnityEngine;

public class DialogManager : Singleton<DialogManager>
{
    public static Int32 SelectChoice
    {
        get => DialogManager.selectChoice;
        set => DialogManager.selectChoice = value;
    }

    public Queue<Dialog> DialogPool => this.dialogPool;
    public List<Dialog> ActiveDialogList => this.activeDialogList;

    public Camera CurrentUICamera
    {
        get
        {
            if (this.uiCamera == null)
                this.uiCamera = base.transform.parent.FindChild("Camera").GetComponent<Camera>();
            return this.uiCamera;
        }
    }

    public UIPanel PointerPanel
    {
        get
        {
            if (this.pointerPanel == null)
                this.pointerPanel = base.transform.parent.FindChild("Pointer Container").GetComponent<UIPanel>();
            return this.pointerPanel;
        }
    }

    public Boolean Activate => this.isActivate;
    public Boolean Visible => this.activeDialogList.Count() > 0;

    public Boolean CompletlyVisible
    {
        get
        {
            foreach (Dialog dialog in this.activeDialogList)
                if (dialog.CurrentState != Dialog.State.CompleteAnimation)
                    return false;
            return this.Visible;
        }
    }

    public UIWidget Widget => this.widget;

    public Boolean HasChocoboMenu => EventCollision.IsRidingChocobo() && (this.GetChoiceDialog() != null || this.IsDialogNeedControl());

    public int CurMesId
    {
        get => this.curMesId;
        set => this.curMesId = value;
    }

    public int PressMesId
    {
        get => this.pressMesId;
        set => this.pressMesId = value;
    }

    public int ReleaseMesId
    {
        get => this.releaseMesId;
        set => this.releaseMesId = value;
    }

    public Dialog AttachDialog(String phrase, Int32 width, Int32 lineCount, Dialog.TailPosition tailPos, Dialog.WindowStyle style, Vector2 pos, Dialog.CaptionType captionType = Dialog.CaptionType.None)
    {
        Dialog dialogFromPool = this.GetDialogFromPool();
        if (dialogFromPool != null)
        {
            dialogFromPool.Reset();
            dialogFromPool.IsETbDialog = false;
            dialogFromPool.Width = width;
            dialogFromPool.LineNumber = lineCount;
            dialogFromPool.Style = style;
            dialogFromPool.Tail = tailPos;
            dialogFromPool.Position = pos;
            dialogFromPool.Id = DialogManager.UIDialogId;
            dialogFromPool.Caption = FF9TextTool.GetDialogCaptionText(captionType);
            dialogFromPool.Phrase = phrase;
            dialogFromPool.Show();
            if (!this.isActivate)
                this.ActivateDialogScene();
        }
        return dialogFromPool;
    }

    public Dialog AttachDialog(Int32 dialogId, Dialog.WindowStyle style, Int32 textId, PosObj po, Dialog.DialogIntDelegate listener, Dialog.CaptionType captionType)
    {
        Dialog dialogFromPool = this.GetDialogFromPool();
        if (dialogFromPool != null)
        {
            dialogFromPool.Reset();
            dialogFromPool.IsETbDialog = true;
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
                String phrase = TextPatcher.PatchDialogString(FF9TextTool.FieldText(textId), dialogFromPool);
                if (Configuration.Lang.DualLanguageMode == 2 && po != null)
                {
                    Localization.UseSecondaryLanguage = true;
                    String translation = TextPatcher.PatchDialogString(FF9TextTool.FieldText(textId), dialogFromPool);
                    Localization.UseSecondaryLanguage = false;
                    if (phrase.EndsWith("[ENDN]"))
                        phrase = phrase.Substring(0, phrase.Length - "[ENDN]".Length);
                    if (phrase.OccurenceCount("\n") < 8)
                        phrase += "\n{ResetTags}[WAIT=-1000]" + translation;
                    else
                        phrase += "[PAGE]" + translation;
                }
                dialogFromPool.Phrase = phrase;
                Action<Int32> onFieldTextUpdated = (id) =>
                {
                    if (id == textId)
                    {
                        dialogFromPool.Phrase = TextPatcher.PatchDialogString(FF9TextTool.FieldText(textId), dialogFromPool);
                        dialogFromPool.Show();
                    }
                };
                FF9TextTool.FieldTextUpdated += onFieldTextUpdated;
                // Unsubscribe
                Dialog.DialogIntDelegate unsubscribe = (c) => FF9TextTool.FieldTextUpdated -= onFieldTextUpdated;
                listener = listener != null ? unsubscribe + listener : unsubscribe;
            }
            dialogFromPool.Show();
            dialogFromPool.AfterDialogHidden = listener;
            if (!this.isActivate)
                this.ActivateDialogScene();
        }
        return dialogFromPool;
    }

    public Boolean CheckDialogOverlap(Dialog dialog)
    {
        foreach (Dialog otherDialog in this.activeDialogList)
        {
            if (otherDialog.gameObject.activeSelf && otherDialog.IsActive)
            {
                Boolean overlapX = dialog.FF9Position.x < otherDialog.FF9Position.x + otherDialog.Size.x && dialog.FF9Position.x + dialog.Size.x > otherDialog.FF9Position.x;
                Boolean overlapY = dialog.FF9Position.y < otherDialog.FF9Position.y + otherDialog.Size.y && dialog.FF9Position.y + dialog.Size.y > otherDialog.FF9Position.y;
                if (overlapX && overlapY)
                    return true;
            }
        }
        return false;
    }

    public Boolean CheckDialogShowing(Int32 dialogId)
    {
        foreach (Dialog dialog in this.activeDialogList)
            if (dialog.Id == dialogId)
                return true;
        return false;
    }

    public Dialog GetDialogFromPool()
    {
        Dialog dialog = null;
        try
        {

            if (this.DialogPool.Count > 0)
                dialog = this.DialogPool.Dequeue();
            else
                dialog = NGUITools.AddChild(base.gameObject, this.DialogPrefab).GetComponent<Dialog>();
        }
        catch
        {
        }
        if (dialog == null)
            return null;
        this.activeDialogList.Add(dialog);
        dialog.gameObject.SetActive(true);
        return dialog;
    }

    public void Close(Int32 dialogId, Boolean scriptedClose = false)
    {
        foreach (Dialog dialog in this.activeDialogList.ToList()) // Copy the list in case closing a dialog immediately releases it
        {
            if (dialog.gameObject.activeInHierarchy && dialog.Id == dialogId)
            {
                if (scriptedClose)
                    dialog.IsClosedByScript = true;
                dialog.ForceClose();
                if (FF9StateSystem.Common.FF9.fldMapNo == 100) // Alexandria/Main Street
                {
                    Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Byte, 31));
                    if (varManually == 3) // When Puck hits Vivi
                        dialog.AfterHidden();
                }
            }
        }
    }

    public void CloseAll(Boolean scriptedClose = false)
    {
        foreach (Dialog dialog in this.activeDialogList.ToList())
        {
            if (dialog.gameObject.activeInHierarchy)
            {
                if (scriptedClose)
                    dialog.IsClosedByScript = true;
                dialog.ForceClose();
            }
        }
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
        for (Int32 i = 0; i < DialogManager.InitialDialogCount - 1; i++)
            this.AttachDialog("[STRT=10,1][IMME]Load[TIME=1]", 10, 1, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStyleTransparent, new Vector2(10000f, 10000f), Dialog.CaptionType.None);
    }

    private void OnClick()
    {
        if (this.isHudActive || Configuration.Control.DisableMouseForMenus)
            return;
        if (this.GetChoiceDialog() == null)
        {
            this.OnKeyConfirm(base.gameObject);
            EventInput.RecieveDialogConfirm();
        }
    }

    public void OnDrag(Vector2 delta)
    {
        if (PersistenSingleton<UIManager>.Instance.IsPause)
            return;
        Dialog choiceDialog = this.GetChoiceDialog();
        if (choiceDialog != null)
        {
            if (delta.y < -50f)
                choiceDialog.MoveCurrentChoice(1);
            else if (delta.y > 50f)
                choiceDialog.MoveCurrentChoice(-1);
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
            return;
        Dialog choiceDialog = this.GetChoiceDialog();
        if (choiceDialog != null)
            choiceDialog.OnItemSelect(go);
    }

    private void ActivateDialogScene()
    {
        Boolean shouldActivate = true;
        if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD)
        {
            ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[ff9.w_moveActorPtr.originalActor.index];
            if (ff9.m_GetIDEvent(s_moveCHRStatus.id) != 0 && UIManager.World.CurrentState != WorldHUD.State.FullMap && !this.HasChocoboMenu && !ff9.w_isMogActive)
            {
                Boolean closableDialogIsShown = this.activeDialogList.Any(dial => dial.TextId == 87); // "Watched the waves around the world and relaxed!"
                Boolean hasUIDialog = this.activeDialogList.Any(dial => dial.Id == DialogManager.UIDialogId);
                shouldActivate = hasUIDialog || closableDialogIsShown;
            }
        }
        if (shouldActivate)
        {
            base.gameObject.GetComponent<BoxCollider>().enabled = true;
            this.isActivate = true;
        }
    }

    private void DeactivateDialogScene()
    {
        Boolean shouldDeactivate = this.activeDialogList.Count == 0;
        if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD)
        {
            Boolean closableDialogIsShown = this.activeDialogList.Any(dial => dial.TextId == 87); // "Watched the waves around the world and relaxed!"
            ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[ff9.w_moveActorPtr.originalActor.index];
            if (!closableDialogIsShown && ff9.m_GetIDEvent(s_moveCHRStatus.id) != 0 && UIManager.World.CurrentState != WorldHUD.State.FullMap && !this.HasChocoboMenu && !ff9.w_isMogActive)
                shouldDeactivate = true;
        }
        if (shouldDeactivate)
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
            dialog.PauseDialog(isPause);
    }

    public Dialog GetDialogByWindowID(Int32 winId)
    {
        foreach (Dialog dialog in this.activeDialogList)
            if (dialog.Id == winId)
                return dialog;
        return null;
    }

    public Dialog GetChoiceDialog()
    {
        foreach (Dialog dialog in this.activeDialogList)
            if (dialog.HasChoices)
                return dialog;
        return null;
    }

    public Boolean IsDialogNeedControl()
    {
        foreach (Dialog dialog in this.activeDialogList)
            if (!dialog.FlagButtonInh)
                return true;
        return false;
    }

    public void ForceControlByEvent(Boolean control)
    {
        foreach (Dialog dialog in this.activeDialogList)
            dialog.FlagButtonInh = control;
    }

    public void RiseAll()
    {
        foreach (Dialog dialog in this.activeDialogList)
        {
            if (dialog.Panel.depth < Dialog.DialogAdditionalRaiseDepth + Dialog.DialogMaximumDepth)
            {
                dialog.Panel.depth += Dialog.DialogAdditionalRaiseDepth;
                dialog.phrasePanel.depth += Dialog.DialogAdditionalRaiseDepth;
            }
        }
    }

    public UILabel GetDialogLabel()
    {
        foreach (Transform transform in base.transform)
        {
            Dialog dialog = transform.GetComponent<Dialog>();
            if (dialog != null)
                return dialog.PhraseLabel;
        }
        return null;
    }

    public Dialog GetDialogByTextId(Int32 textId)
    {
        foreach (Dialog dialog in this.activeDialogList)
            if (dialog.TextId == textId)
                return dialog;
        return null;
    }

    public Dialog GetOverlayDialog()
    {
        foreach (Dialog dialog in this.activeDialogList)
            if (dialog.IsOverlayDialog)
                return dialog;
        return null;
    }

    public Dialog GetMognetDialog()
    {
        foreach (Dialog dialog in this.activeDialogList)
            if (dialog.CapType == Dialog.CaptionType.Mognet && dialog.HasChoices)
                return dialog;
        return null;
    }

    public void EnableCollider(Boolean value)
    {
        base.gameObject.GetComponent<BoxCollider>().enabled = value;
    }

    public const Int32 UIDialogId = 9;

    private const Byte InitialDialogCount = 10;

    private static Int32 selectChoice;

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

    private Int32 curMesId = -1;
    private Int32 pressMesId = -1;
    private Int32 releaseMesId = -1;
}
