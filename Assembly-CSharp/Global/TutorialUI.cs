using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Memoria.Assets;
using Memoria.Scenes;

public class TutorialUI : UIScene
{
    public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
        {
            if (!this.isFromPause)
            {
                switch (this.DisplayMode)
                {
                    case TutorialUI.Mode.Battle:
                        this.DisplayBattleTutorial();
                        break;
                    case TutorialUI.Mode.QuadMist:
                        this.DisplayQuadmistTutorial();
                        break;
                    case TutorialUI.Mode.BasicControl:
                        this.DisplayBasicControlTutorial();
                        break;
                    case TutorialUI.Mode.Libra:
                        this.DisplayLibra();
                        break;
                }
            }
            this.isFromPause = false;
        };
        if (afterFinished != null)
            sceneVoidDelegate += afterFinished;
        base.Show(sceneVoidDelegate);
        if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Battle)
            FF9StateSystem.Battle.isTutorial = true;
    }

    public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
        {
            if (!this.isFromPause)
            {
                if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Battle)
                {
                    UIManager.UIState previousState = PersistenSingleton<UIManager>.Instance.PreviousState;
                    if (previousState != UIManager.UIState.Config)
                    {
                        battle.isSpecialTutorialWindow = false;
                        FF9StateSystem.Battle.isTutorial = false;
                        UIManager.Battle.NextSceneIsModal = true;
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.BattleHUD);
                    }
                    else
                    {
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Config);
                    }
                }
                else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Field)
                {
                    UIManager.UIState previousState = PersistenSingleton<UIManager>.Instance.PreviousState;
                    if (previousState != UIManager.UIState.Config)
                    {
                        UIManager.Field.NextSceneIsModal = true;
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.FieldHUD);
                    }
                    else
                    {
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Config);
                    }
                }
                else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.QuadMist)
                {
                    PersistenSingleton<UIManager>.Instance.ChangeUIState(this.QuadmistTutorialID == 1 ? UIManager.UIState.QuadMist : UIManager.UIState.QuadMistBattle);
                }
                else
                {
                    PersistenSingleton<UIManager>.Instance.ChangeUIState(PersistenSingleton<UIManager>.Instance.HUDState);
                }
            }
        };
        if (afterFinished != null)
            sceneVoidDelegate += afterFinished;
        base.Hide(sceneVoidDelegate);
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go))
        {
            FF9Sfx.FF9SFX_Play(103);
            if (this.DisplayMode == TutorialUI.Mode.Battle || this.DisplayMode == TutorialUI.Mode.Libra)
                this.OnOKButtonClick();
        }
        return true;
    }

    public override Boolean OnKeyPause(GameObject go)
    {
        if (PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.Config || PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.QuadMistBattle || PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.QuadMist)
            return false;
        base.OnKeyPause(go);
        base.NextSceneIsModal = true;
        this.isFromPause = true;
        this.Hide(delegate
        {
            PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Pause);
        });
        return true;
    }

    private void Awake()
    {
        this.ContentPanel.transform.localScale = new Vector3(0f, 0f, 0f);
        this.battleTutorialImage1 = this.ContentPanel.GetChild(0).GetComponent<UISprite>();
        this.battleTutorialImage2 = this.ContentPanel.GetChild(1).GetComponent<UISprite>();
        this.battleTutorialDialogImage2 = this.ContentPanel.GetChild(1).GetChild(0).GetChild(0).GetComponent<UISprite>();
        this.battleLeftLabel = this.ContentPanel.GetChild(0).GetChild(1).GetComponent<UILabel>();
        this.battleRightLabel = this.ContentPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
        this.battleBottomLabel = this.ContentPanel.GetChild(3).GetComponent<UILabel>();
        this.headerLabel = this.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UILabel>();
        this.headerLocalize = this.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UILocalize>();
        this.battleTutorialImage1Dialog = this.ContentPanel.GetChild(0).GetChild(0).GetComponent<UISprite>();
        this.battleTutorialImage1ATB = this.ContentPanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<UISprite>();
        this.battleTutorialImage1ATBLocalize = this.ContentPanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<UILocalize>();
        this.battleTutorialImage1Pointer = this.ContentPanel.GetChild(0).GetChild(2).GetComponent<UISprite>();
        this.battleTutorialImage2Dialog = this.ContentPanel.GetChild(1).GetChild(0).GetComponent<UISprite>();
        this.battleTutorialImage2Pointer = this.ContentPanel.GetChild(1).GetChild(2).GetComponent<UISprite>();
        this.battleBottomLocalize = this.ContentPanel.GetChild(3).GetComponent<UILocalize>();
        this.battleOkButton = this.ContentPanel.GetChild(4).GetComponent<UIButton>();
        this.OkButton = new GOIsolatedButton(this.ContentPanel.GetChild(4));
    }

    private void HideTutorial()
    {
        base.Loading = false;
        if (PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.Config)
        {
            this.DisplayMode = TutorialUI.Mode.Battle;
            if (this.AfterFinished != null)
                this.AfterFinished();
        }
    }

    private void AnimatePanel(Vector3 scale)
    {
        this.ContentPanel.GetParent().SetActive(true);
        EventDelegate.Add(TweenScale.Begin(this.ContentPanel, this.duration, scale).onFinished, this.AfterShowBattleTutorial);
    }

    public void OnOKButtonClick()
    {
        if (this.DisplayMode == TutorialUI.Mode.Libra && ++this.libraPage < this.libraMessages.Count)
        {
            this.battleBottomLabel.rawText = this.libraMessages[this.libraPage];
        }
        else
        {
            this.AnimatePanel(new Vector3(0f, 0f, 0f));
            base.StartCoroutine(this.WaitAndHide());
        }
    }

    private IEnumerator WaitAndHide()
    {
        yield return new WaitForSeconds(this.duration);
        this.Hide(delegate
        {
            this.ContentPanel.GetParent().SetActive(false);
            this.HideTutorial();
        });
        yield break;
    }

    private void AfterShowBattleTutorial()
    {
        this.OkButton.Label.Label.Parser.ResetBeforeVariableTags();
        this.battleBottomLabel.Parser.ResetBeforeVariableTags();
        base.Loading = false;
    }

    private void DisplayBattleTutorial()
    {
        String suffix = Localization.CurrentLanguage == "Japanese" ? "_jp" : String.Empty;
        String platform = FF9StateSystem.MobilePlatform ? "mobile" : "pc";
        String platformUpper = FF9StateSystem.MobilePlatform ? "Mobile" : "PC";
        this.headerLocalize.enabled = true;
        this.battleTutorialImage1.SetAnchor(target: this.ContentPanel.transform, relRight: 0.5f, relBottom: 0.5f, left: 103, right: -60, top: -41, bottom: -6); // TODO
        this.battleTutorialImage1.spriteName = $"tutorial_{platform}_01{suffix}";
        this.battleTutorialImage2.spriteName = $"tutorial_{platform}_02{suffix}";
        this.battleTutorialImage1Dialog.spriteName = "tutorial_help_01";
        this.battleTutorialImage1ATB.spriteName = "tutorial_help_01_us_uk_jp_fr_gr_it";
        this.battleTutorialImage1ATBLocalize.enabled = true;
        this.battleTutorialImage1Pointer.spriteName = "tutorial_help_cursor";
        this.battleTutorialDialogImage2.spriteName = Localization.Get($"TutorialTapOnCharacterIcon{platformUpper}");
        this.battleTutorialImage2Dialog.spriteName = "tutorial_help_02";
        this.battleTutorialImage2Pointer.spriteName = "tutorial_help_cursor";
        this.battleLeftLabel.rawText = Localization.Get($"TutorialLeftParagraph{platformUpper}");
        this.battleRightLabel.rawText = Localization.Get($"TutorialRightParagraph{platformUpper}");
        this.headerLabel.fontSize = 36;
        this.battleBottomLabel.SetAnchor(target: this.ContentPanel.transform, relTop: 0.33f);
        this.battleBottomLabel.fontSize = 24;
        this.battleBottomLabel.overflowMethod = UILabel.Overflow.ClampContent;
        this.battleBottomLocalize.enabled = true;
        base.Loading = true;
        this.AnimatePanel(new Vector3(1f, 1f, 1f));
        this.battleBottomLabel.gameObject.SetActive(!FF9StateSystem.PCPlatform);
    }

    private void DisplayQuadmistTutorial()
    {
        if (this.QuadmistTutorialID > 3)
            return;
        base.Loading = true;
        String key = TutorialUI.QuadMistLocalizeKey + this.QuadmistTutorialID;
        Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get(key), 0, 0, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, Vector2.zero, Dialog.CaptionType.None);
        dialog.AfterDialogShown = delegate (Int32 choice)
        {
            base.Loading = false;
        };
        dialog.AfterDialogHidden = this.AfterHideQuadmistTutorial;
        TweenPosition tweenPos = dialog.GetComponent<TweenPosition>();
        if (tweenPos != null)
            tweenPos.enabled = false;
    }

    private void AfterHideQuadmistTutorial(Int32 choice)
    {
        this.Hide(delegate
        {
            this.HideTutorial();
        });
    }

    private void DisplayBasicControlTutorial()
    {
        base.Loading = true;
        String prefix;
        if (FF9StateSystem.MobilePlatform)
        {
            if (FF9StateSystem.AndroidTVPlatform && PersistenSingleton<HonoInputManager>.Instance.IsControllerConnect)
            {
                prefix = this.BasicControlTutorialID <= 0 ? "AndroidTV" : "PC";
                this.lastPage = 2;
            }
            else
            {
                prefix = "Mobile";
                this.lastPage = 1;
            }
        }
        else
        {
            prefix = "PC";
            this.lastPage = 2;
        }
        if (this.BasicControlTutorialID > this.lastPage)
            return;
        String key = prefix + TutorialUI.BasicControlLocalizeKey + this.BasicControlTutorialID;
        Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get(key), 0, 0, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, Vector2.zero, Dialog.CaptionType.None);
        dialog.AfterDialogShown = delegate (Int32 choice)
        {
            base.Loading = false;
        };
        dialog.AfterDialogHidden = this.AfterHideBasicControlTutorial;
    }

    private void AfterHideBasicControlTutorial(Int32 choice)
    {
        if (this.BasicControlTutorialID < this.lastPage)
        {
            this.BasicControlTutorialID++;
            this.DisplayBasicControlTutorial();
        }
        else
        {
            this.Hide(this.HideTutorial);
        }
    }

    private void DisplayLibra()
    {
        List<UISpriteData> spriteList = this.battleTutorialImage1.atlas.spriteList;
        UISpriteData photoSprite = spriteList.FirstOrDefault(sprt => sprt.name == "libra_photo");
        if (photoSprite == null)
        {
            photoSprite = new UISpriteData();
            photoSprite.name = "libra_photo";
            spriteList.Add(photoSprite);
            this.battleTutorialImage1.atlas.spriteList = spriteList;
            this.battleTutorialImage1.atlas.MarkSpriteListAsChanged();
        }
        photoSprite.width = this.libraPhoto.width;
        photoSprite.height = this.libraPhoto.height;
        photoSprite.texture = this.libraPhoto;
        this.headerLocalize.enabled = false;
        this.headerLabel.rawText = this.libraTitle;
        this.headerLabel.fontSize = 44;
        this.battleTutorialImage1.spriteName = String.Empty;
        this.battleTutorialImage1.spriteName = "libra_photo";
        this.battleTutorialImage1.SetAnchor(target: this.ContentPanel.transform, relBottom: 1f, relRight: 0f, left: 100f, right: 100f + photoSprite.width, bottom: -100f - photoSprite.height, top: -100f);
        this.battleTutorialImage2.spriteName = String.Empty;
        this.battleTutorialImage1Dialog.spriteName = String.Empty;
        this.battleTutorialImage1ATB.spriteName = String.Empty;
        this.battleTutorialImage1ATBLocalize.enabled = false;
        this.battleTutorialImage1Pointer.spriteName = String.Empty;
        this.battleTutorialDialogImage2.spriteName = String.Empty;
        this.battleTutorialImage2Dialog.spriteName = String.Empty;
        this.battleTutorialImage2Pointer.spriteName = String.Empty;
        this.battleLeftLabel.rawText = String.Empty;
        this.battleRightLabel.rawText = String.Empty;
        this.battleBottomLocalize.enabled = false;
        this.battleBottomLabel.SetAnchor(target: this.ContentPanel.transform, left: 100f + photoSprite.width);
        this.battleBottomLabel.rawText = this.libraMessages[0];
        this.battleBottomLabel.fontSize = 42;
        this.battleBottomLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
        this.battleBottomLabel.gameObject.SetActive(true);
        this.libraPage = 0;
        base.Loading = true;
        this.AnimatePanel(new Vector3(1f, 1f, 1f));
    }

    private const String QuadMistLocalizeKey = "QuadMistTutorial";
    private const String BasicControlLocalizeKey = "BasicControlTutorial";

    public GameObject ContentPanel;

    public TutorialUI.Mode DisplayMode;

    public Int32 QuadmistTutorialID;
    public Int32 BasicControlTutorialID;

    public UIScene.SceneVoidDelegate AfterFinished;

    private UISprite battleTutorialImage1;
    private UISprite battleTutorialImage2;
    private UISprite battleTutorialDialogImage2;

    private UILabel battleLeftLabel;
    private UILabel battleRightLabel;
    private UILabel battleBottomLabel;

    private Single duration = 0.16f;

    private String currentButtonGroup = String.Empty;

    private Boolean isFromPause;

    private Int32 lastPage = 1;

    [NonSerialized]
    private UILabel headerLabel;
    [NonSerialized]
    private UILocalize headerLocalize;
    [NonSerialized]
    private UISprite battleTutorialImage1Dialog;
    [NonSerialized]
    private UISprite battleTutorialImage1ATB;
    [NonSerialized]
    private UILocalize battleTutorialImage1ATBLocalize;
    [NonSerialized]
    private UISprite battleTutorialImage1Pointer;
    [NonSerialized]
    private UISprite battleTutorialImage2Dialog;
    [NonSerialized]
    private UISprite battleTutorialImage2Pointer;
    [NonSerialized]
    private UILocalize battleBottomLocalize;
    [NonSerialized]
    private UIButton battleOkButton;
    [NonSerialized]
    private GOIsolatedButton OkButton;

    [NonSerialized]
    public String libraTitle;
    [NonSerialized]
    public List<String> libraMessages;
    [NonSerialized]
    public Texture2D libraPhoto;
    [NonSerialized]
    public Int32 libraPage;

    private class GOIsolatedButton : GOWidget
    {
        public readonly UIButton Button;
        public readonly BoxCollider BoxCollider;
        public readonly OnScreenButton OnScreenButton;
        public readonly UISprite Highlight;
        public readonly GOLocalizableLabel Label;
        public readonly GOThinBackground Background;

        public GOIsolatedButton(GameObject go) : base(go)
        {
            Button = go.GetComponent<UIButton>();
            BoxCollider = go.GetComponent<BoxCollider>();
            OnScreenButton = go.GetComponent<OnScreenButton>();
            Highlight = go.GetChild(0).GetComponent<UISprite>();
            Label = new GOLocalizableLabel(go.GetChild(1));
            Background = new GOThinBackground(go.GetChild(2));
        }
    }

    public enum Mode
    {
        Battle,
        QuadMist,
        BasicControl,
        Libra
    }
}
