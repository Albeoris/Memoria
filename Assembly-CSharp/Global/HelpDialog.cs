﻿using System;
using System.Collections;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Assets;
using UnityEngine;

public class HelpDialog : Singleton<HelpDialog>
{
    public Vector2 PointerOffset
    {
        set => this.pointerOffset = value;
    }

    public Vector4 PointerLimitRect
    {
        set => this.pointerLimitRect = value;
    }

    public String Phrase
    {
        set => this.phrase = value;
    }

    public Vector2 Position
    {
        set
        {
            this.position = value;
            this.position.x = this.position.x + this.pointerOffset.x;
            this.position.y = this.position.y + this.pointerOffset.y;
            this.position.x = Mathf.Clamp(this.position.x, this.pointerLimitRect.x, this.pointerLimitRect.z);
            this.position.y = Mathf.Clamp(this.position.y, this.pointerLimitRect.y, this.pointerLimitRect.w);
            if (this.position.y < -480f)
                this.position.y = this.position.y + 20f;
            this.position.x = this.position.x + UIManager.UIContentSize.x / 2f;
            this.position.y = -this.position.y + UIManager.UIContentSize.y / 2f;
        }
    }

    public Boolean Tail
    {
        set => this.tail = value;
    }

    public Vector4 ClipRect
    {
        set => this.clipRect = value;
    }

    public Vector2 BodySize => new Vector2(this.bodyWidget.width, this.bodyWidget.height);

    public Int32 Depth
    {
        set => this.depth = value;
    }

    public UILabel PhraseLabel => this.phraseLabel;

    public Boolean IsShown => base.gameObject.activeSelf;

    public void ShowDialog()
    {
        base.gameObject.SetActive(true);
        this.phraseLabel.rawText = this.phrase;
        this.InitializeDialog();
    }

    public void OnLocalize()
    {
        if (this.isActiveAndEnabled)
            base.StartCoroutine(OnLocalizeDelayed());
    }

    private IEnumerator OnLocalizeDelayed()
    {
        yield return new WaitForEndOfFrame();
        ButtonGroupState.RefreshHelpDialog();
        yield break;
    }

    public void HideDialog()
    {
        base.gameObject.SetActive(false);
    }

    public void SetDialogVisibility(Boolean isVisible)
    {
        if (isVisible && ButtonGroupState.HelpEnabled && ButtonGroupState.ActiveGroup != String.Empty)
        {
            if (ButtonGroupState.ActiveButton != null)
            {
                ButtonGroupState buttonGroup = ButtonGroupState.ActiveButton.GetComponent<ButtonGroupState>();
                if (buttonGroup != null && buttonGroup.Help.Enable)
                    ButtonGroupState.ShowHelpDialog(ButtonGroupState.ActiveButton);
            }
        }
        else
        {
            this.HideDialog();
        }
    }

    private void InitializeDialog()
    {
        this.phraseLabel.width = (Int32)UIManager.UIContentSize.x;
        this.phraseLabel.height = (Int32)UIManager.UIContentSize.y;
        this.phraseLabel.fontSize = 42;
        Vector4 rectVector;
        Single tailX = 0f;
        if (PersistenSingleton<UIManager>.Instance.UnityScene != UIManager.Scene.Battle || UIManager.IsUIStateMenu(PersistenSingleton<UIManager>.Instance.State))
        {
            tailX = this.FF9Help_ComputeWindow();
            rectVector = this.dialogRect;
        }
        else
        {
            this.dialogTail = HelpDialog.FF9HELP_NONE;
            if (Configuration.Interface.IsEnabled)
            {
                this.dialogRect.x = Configuration.Interface.BattleDetailPos.x + UIManager.UIContentSize.x / 2f;
                this.dialogRect.y = UIManager.UIContentSize.y / 2f - Configuration.Interface.BattleDetailPos.y - 22f;
                this.dialogRect.z = Configuration.Interface.BattleDetailSize.x - 2f * HelpDialog.HelpDialogXPadding + 48f;
                this.dialogRect.w = Configuration.Interface.BattleDetailSize.y - 2f * HelpDialog.HelpDialogYPadding + 158f;
                this.phraseLabel.fontSize = (Int32)Math.Round(42f * (Configuration.Interface.BattleDetailSize.y / 230f));
            }
            else if (ButtonGroupState.ActiveGroup == BattleHUD.TargetGroupButton)
            {
                // Official patch https://store.steampowered.com/news/app/377840/view/2735326880600635615 resized the battle help
                // New:
                this.dialogRect.x = 1282f;
                this.dialogRect.y = 898f;
                this.dialogRect.z = 478f;
                this.dialogRect.w = 272f;

                // Old:
                // this.dialogRect.x = 1306f;
                // this.dialogRect.y = 880f;
                // this.dialogRect.z = 420f;
                // this.dialogRect.w = 332f;
                if (Localization.CurrentDisplaySymbol != "JP")
                    this.phraseLabel.fontSize = 32;
            }
            else
            {
                // New:
                this.dialogRect.x = 1116.5f;
                this.dialogRect.y = 898f;
                this.dialogRect.z = 808f;
                this.dialogRect.w = 264f;

                // Old:
                // this.dialogRect.x = 1116.5f;
                // this.dialogRect.y = 880f;
                // this.dialogRect.z = 798f;
                // this.dialogRect.w = 332f;
            }
            rectVector = this.dialogRect;
        }
        this.dialogRect.z = this.dialogRect.z + 2f * HelpDialog.HelpDialogXPadding;
        this.dialogRect.w = this.dialogRect.w + 2f * HelpDialog.HelpDialogYPadding;
        this.dialogRect.x = this.dialogRect.x - UIManager.UIContentSize.x / 2f;
        this.dialogRect.y = UIManager.UIContentSize.y / 2f - this.dialogRect.y;
        base.transform.localPosition = new Vector3(this.dialogRect.x, this.dialogRect.y, 0f);
        this.panel.baseClipRegion = new Vector4(0f, 0f, this.dialogRect.z, this.dialogRect.w);
        this.panel.depth = this.depth;
        this.bodyWidget.width = (Int32)rectVector.z;
        this.bodyWidget.height = (Int32)rectVector.w;
        this.borderWidget.width = (Int32)(rectVector.z + 36f);
        this.borderWidget.height = (Int32)(rectVector.w + 36f);
        this.captionWidget.transform.localPosition = new Vector3(22f - rectVector.z / 2f + this.captionWidget.width / 2, rectVector.w / 2f - this.captionWidget.height / 2 + 2f, 0f);
        this.phraseLabel.width = (Int32)(rectVector.z - HelpDialog.HelpDialogTextXPadding * 2 + 4);
        this.phraseLabel.height = (Int32)(rectVector.w - HelpDialog.HelpDialogTextYPadding + 4);
        this.phraseLabel.transform.localPosition = new Vector3(-rectVector.z / 2f + HelpDialog.HelpDialogTextXPadding, rectVector.w / 2f - HelpDialog.HelpDialogTextYPadding, 0f);
        this.DisplayDialogTail(tailX);
    }

    private Single FF9Help_ComputeWindow()
    {
        this.phraseLabel.ProcessText();
        this.dialogRect.z = Math.Min(this.phraseLabel.Parser.MaxWidth + HelpDialog.FF9HELP_X_SPACE, this.clipRect.z);
        this.dialogRect.w = Math.Min(this.phraseLabel.Parser.RawHeight + HelpDialog.FF9HELP_Y_SPACE, this.clipRect.w);
        if (!this.tail)
        {
            this.dialogTail = HelpDialog.FF9HELP_NONE;
            this.dialogRect.x = this.position.x;
            this.dialogRect.y = this.position.y;
            return 0f;
        }
        Int32 freeSpaceBelow = (Int32)(this.position.y + HelpDialog.FF9HELP_GAP_BOTTOM - this.clipRect.y);
        Int32 freeSpaceAbove = (Int32)(this.clipRect.y + this.clipRect.w - (this.position.y + HelpDialog.FF9HELP_GAP_TOP));
        if (freeSpaceBelow > freeSpaceAbove)
        {
            this.dialogTail = HelpDialog.FF9HELP_BOTTOM;
            this.dialogRect.y = this.position.y + HelpDialog.FF9HELP_GAP_BOTTOM - this.dialogRect.w / 2f;
        }
        else
        {
            this.dialogTail = HelpDialog.FF9HELP_TOP;
            this.dialogRect.y = this.position.y + HelpDialog.FF9HELP_GAP_TOP + this.dialogRect.w / 2f;
        }
        Int32 freeSpaceRight = (Int32)(this.position.x + HelpDialog.FF9HELP_GAP_RIGHT - this.clipRect.x);
        Int32 freeSpaceLeft = (Int32)(this.clipRect.x + this.clipRect.z / 2f - (this.position.x + HelpDialog.FF9HELP_GAP_LEFT));
        Int32 posX;
        if (freeSpaceRight > freeSpaceLeft)
        {
            this.dialogTail |= HelpDialog.FF9HELP_RIGHT;
            posX = (Int32)this.position.x + HelpDialog.FF9HELP_GAP_RIGHT;
        }
        else
        {
            this.dialogTail |= HelpDialog.FF9HELP_LEFT;
            posX = (Int32)this.position.x;
        }
        this.dialogRect.x = posX - ((Int32)this.dialogRect.z >> 1);
        if (this.dialogRect.x - this.dialogRect.z / 2f < this.clipRect.x)
            this.dialogRect.x = this.clipRect.x + this.dialogRect.z / 2f;
        if (this.dialogRect.x + this.dialogRect.z / 2f > this.clipRect.x + this.clipRect.z)
            this.dialogRect.x = this.clipRect.x + this.clipRect.z - this.dialogRect.z / 2f;
        if ((this.dialogTail & HelpDialog.FF9HELP_RIGHT) != 0)
            return posX - this.dialogRect.x;
        return Mathf.Max(HelpDialog.FF9HELP_WIN_MINX, posX - (this.dialogRect.x - this.dialogRect.z / 2f));
    }

    private void DisplayDialogTail(Single x)
    {
        if (this.dialogTail != HelpDialog.FF9HELP_NONE)
        {
            this.tailSprite.alpha = 1f;
            Single y = (HelpDialog.FF9HELP_BOTTOM <= this.dialogTail ? 1 : -1) * (this.tailSprite.height / 2f - this.dialogRect.w / 2f);
            x += (this.dialogTail % 2 == HelpDialog.FF9HELP_LEFT) ? this.tailSprite.width / 2f - this.dialogRect.z / 2f : -this.tailSprite.width / 2f;
            Vector3 localPosition = new Vector3((Int32)x, (Int32)y, 0f);
            this.tailSprite.transform.localPosition = localPosition;
            switch (this.dialogTail)
            {
                case 0:
                    this.tailSprite.spriteName = "dialog_pointer_topleft";
                    break;
                case 1:
                    this.tailSprite.spriteName = "dialog_pointer_topright";
                    break;
                case 2:
                    this.tailSprite.spriteName = "dialog_pointer_downleft";
                    break;
                case 3:
                    this.tailSprite.spriteName = "dialog_pointer_downright";
                    break;
            }
        }
        else
        {
            this.tailSprite.alpha = 0f;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        this.panel = base.gameObject.GetComponent<UIPanel>();
        this.bodyWidget = base.gameObject.GetChild(0).GetComponent<UIWidget>();
        this.borderWidget = base.gameObject.GetChild(1).GetComponent<UIWidget>();
        this.phraseLabel = base.gameObject.GetChild(3).GetComponent<UILabel>();
        this.tailSprite = base.gameObject.GetChild(4).GetComponent<UISprite>();
        this.captionWidget = base.gameObject.GetChild(2).GetComponent<UIWidget>();
        this.clipRect = HelpDialog.DefaultClipRect;
        this.phraseLabel.DefaultTextColor = FF9TextTool.DarkBlue;
    }

    public static Int32 FF9HELP_X_SPACE = (Int32)(UIManager.ResourceXMultipier * 12f);
    public static Int32 FF9HELP_Y_SPACE = (Int32)(UIManager.ResourceYMultipier * 12f);
    public static Int32 FF9HELP_MOG_X = (Int32)(UIManager.ResourceXMultipier * 5f);
    public static Int32 FF9HELP_MOG_Y = (Int32)(UIManager.ResourceYMultipier * 2f);
    public static Int32 FF9HELP_GAP_TOP = (Int32)(UIManager.ResourceYMultipier * 10f);
    public static Int32 FF9HELP_GAP_BOTTOM = (Int32)(UIManager.ResourceYMultipier * -16f);
    public static Int32 FF9HELP_GAP_LEFT = (Int32)(UIManager.ResourceXMultipier * 18f);
    public static Int32 FF9HELP_GAP_RIGHT = (Int32)(UIManager.ResourceXMultipier * -19f);
    public static Int32 FF9HELP_WIN_MINX = (Int32)(UIManager.ResourceXMultipier * 8f);
    public static Int32 FF9HELP_TALK_WD = (Int32)(UIManager.ResourceXMultipier * 16f);
    public static Int32 FF9HELP_TALK_HG = (Int32)(UIManager.ResourceYMultipier * 14f);
    public static Int32 FF9HELP_MINX = (Int32)(UIManager.ResourceXMultipier * 8f);
    public static Int32 FF9HELP_MINY = (Int32)(UIManager.ResourceYMultipier * 8f);
    public static Int32 FF9HELP_MAXX = (Int32)(UIManager.UIContentSize.x - HelpDialog.FF9HELP_MINX);
    public static Int32 FF9HELP_MAXY = (Int32)(UIManager.UIContentSize.y - HelpDialog.FF9HELP_MINY);
    public static Int32 FF9HELP_MAXW = HelpDialog.FF9HELP_MAXX - HelpDialog.FF9HELP_MINX + 1 * (Int32)UIManager.ResourceXMultipier;
    public static Int32 FF9HELP_MAXH = HelpDialog.FF9HELP_MAXY - HelpDialog.FF9HELP_MINY + 1 * (Int32)UIManager.ResourceYMultipier;
    public static Int32 FF9HELP_DEFMAXX = (Int32)(UIManager.ResourceXMultipier * FieldMap.PsxFieldWidth - HelpDialog.FF9HELP_MINX);
    public static Int32 FF9HELP_DEFMAXY = (Int32)(UIManager.ResourceYMultipier * FieldMap.PsxFieldHeightNative - HelpDialog.FF9HELP_MINY);
    public static Int32 FF9HELP_DEFMAXW = HelpDialog.FF9HELP_DEFMAXX - HelpDialog.FF9HELP_MINX + 1 * (Int32)UIManager.ResourceXMultipier;
    public static Int32 FF9HELP_DEFMAXH = HelpDialog.FF9HELP_DEFMAXY - HelpDialog.FF9HELP_MINY + 1 * (Int32)UIManager.ResourceYMultipier;

    public static Int32 FF9HELP_TOP = 0;
    public static Int32 FF9HELP_LEFT = 0;
    public static Int32 FF9HELP_BOTTOM = 2;
    public static Int32 FF9HELP_RIGHT = 1;
    public static Int32 FF9HELP_NONE = 255;

    public static Int32 HelpDialogXPadding = 18;
    public static Int32 HelpDialogYPadding = 62;
    public static Int32 HelpDialogTextXPadding = 30;
    public static Int32 HelpDialogTextYPadding = 58;

    public static Vector4 DefaultClipRect = new Vector4(HelpDialog.FF9HELP_MINX, HelpDialog.FF9HELP_MINY, HelpDialog.FF9HELP_DEFMAXW, HelpDialog.FF9HELP_DEFMAXH);

    [SerializeField]
    private Vector3 position;

    [SerializeField]
    private Int32 dialogTail;

    private Vector2 pointerOffset;
    private Vector4 pointerLimitRect;

    private String phrase;

    private Boolean tail;

    private Vector4 clipRect;
    private Vector4 dialogRect;

    private Int32 depth;

    private UIPanel panel;
    private UIWidget bodyWidget;
    private UIWidget borderWidget;
    private UILabel phraseLabel;
    private UISprite tailSprite;
    private UIWidget captionWidget;
}
