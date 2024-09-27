using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

public class CardUI : UIScene
{
    public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.SubMenuPanel.SetActive(false);
            ButtonGroupState.SetPointerDepthToGroup(2, CardUI.CardGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(134f, 10f), CardUI.DiscardDialogButtonGroup);
            ButtonGroupState.ActiveGroup = CardUI.CardGroupButton;
        };
        if (afterFinished != null)
        {
            sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        }
        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(sceneVoidDelegate);
        FF9FCard_Build();
        DisplayInfo();
        DisplayHelp();
        DisplayCardList();
        DisplayCardDetail();
        DeleteSubmenuButton.SetActive(FF9StateSystem.MobilePlatform);
    }

    public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
    {
        base.Hide(afterFinished);
        if (!fastSwitch)
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.StartSubmenuTweenIn();
            RemoveCursorMemorize();
        }
    }

    private void RemoveCursorMemorize()
    {
        ButtonGroupState.RemoveCursorMemorize(CardUI.CardGroupButton);
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go))
        {
            if (ButtonGroupState.ActiveGroup == CardUI.CardGroupButton)
            {
                if (go != DeleteSubmenuButton)
                    currentCardId = cardHudList[go.transform.GetSiblingIndex()].Id;
                if (count[currentCardId] > 0)
                {
                    FF9Sfx.FF9SFX_Play(103);
                    base.Loading = true;
                    deleteDialogTransition.TweenIn(delegate
                    {
                        base.Loading = false;
                        ButtonGroupState.RemoveCursorMemorize(CardUI.DiscardDialogButtonGroup);
                        ButtonGroupState.ActiveGroup = CardUI.DiscardDialogButtonGroup;
                        ButtonGroupState.HoldActiveStateOnGroup(CardUI.CardGroupButton);
                    });
                    prevOffsetButton.enabled = false;
                    nextOffsetButton.enabled = false;
                    deleteCardId = currentCardId;
                }
                else
                {
                    FF9Sfx.FF9SFX_Play(102);
                }
            }
            else if (ButtonGroupState.ActiveGroup == CardUI.DiscardDialogButtonGroup)
            {
                if (go == DeleteSubmenuButton)
                {
                    return true;
                }

                OnDiscardDialogKeyConfirm(go);

                base.Loading = true;
                deleteDialogTransition.TweenOut(delegate
                {
                    base.Loading = false;
                    ButtonGroupState.ActiveGroup = CardUI.CardGroupButton;
                });
                prevOffsetButton.enabled = true;
                nextOffsetButton.enabled = true;
                ButtonGroupState.DisableAllGroup(true);
            }
        }
        return true;
    }

    private void OnDiscardDialogKeyConfirm(GameObject go)
    {
        switch (go.transform.GetSiblingIndex())
        {
            case 1: // Confirm
            {
                if (count[deleteCardId] < 1)
                    goto case 2;

                DiscardSeletctedCard();
                break;
            }
            case 2: // Cancel
                FF9Sfx.FF9SFX_Play(101);
                break;
            case 3: // Auto
                DiscardUnnecessaryCards();
                break;
            default:
                goto case 2;
        }
    }

    private void DiscardSeletctedCard()
    {
        PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
        FF9Sfx.FF9SFX_Play(103);
        QuadMistDatabase.MiniGame_AwayCard((TetraMasterCardId)deleteCardId, offset[deleteCardId]);
        count[deleteCardId]--;
        offset[deleteCardId] = Math.Min(offset[deleteCardId], count[deleteCardId] - 1);
        DisplayHelp();
        DisplayInfo();
        DisplayCardList();
        DisplayCardDetail();
    }

    private void DiscardUnnecessaryCards()
    {
        PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
        FF9Sfx.FF9SFX_Play(103);
        QuadMistDatabase.DiscardUnnecessaryCards();

        for (Int32 i = 0; i < CardPool.TOTAL_CARDS; i++)
        {
            count[i] = 0;
            offset[i] = 0;
        }

        foreach (QuadMistCard quadMistCard in FF9StateSystem.MiniGame.SavedData.MiniGameCard)
            count[(Int32)quadMistCard.id]++;

        DisplayHelp();
        DisplayCardList();
        DisplayCardDetail();
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (base.OnKeyCancel(go))
        {
            if (ButtonGroupState.ActiveGroup == CardUI.CardGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                fastSwitch = false;
                Hide(delegate
                {
                    PersistenSingleton<UIManager>.Instance.MainMenuScene.NeedTweenAndHideSubMenu = false;
                    PersistenSingleton<UIManager>.Instance.MainMenuScene.CurrentSubMenu = MainMenuUI.SubMenu.Card;
                    PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.MainMenu);
                });
            }
            else if (ButtonGroupState.ActiveGroup == CardUI.DiscardDialogButtonGroup)
            {
                if (go == DeleteSubmenuButton)
                {
                    return true;
                }
                FF9Sfx.FF9SFX_Play(101);
                base.Loading = true;
                deleteDialogTransition.TweenOut(delegate
                {
                    base.Loading = false;
                    ButtonGroupState.ActiveGroup = CardUI.CardGroupButton;
                });
                prevOffsetButton.enabled = true;
                nextOffsetButton.enabled = true;
            }
        }
        return true;
    }

    public override Boolean OnKeyLeftBumper(GameObject go)
    {
        if (base.OnKeyLeftBumper(go) && ButtonGroupState.ActiveGroup == CardUI.CardGroupButton)
        {
            Byte b = count[currentCardId];
            if (b > 1)
            {
                FF9Sfx.FF9SFX_Play(1047);
                offset[currentCardId] = (offset[currentCardId] + (Int32)b - 1) % (Int32)b;
                Byte[] dialogIndexes = (from i in Enumerable.Range(0, Mathf.Min((Int32)(b - 1), 4))
                                        select (Byte)i).ToArray<Byte>();
                base.ShowPointerWhenLoading = true;
                base.Loading = true;
                cardDetailTransition.TweenPingPong(dialogIndexes, (UIScene.SceneVoidDelegate)null, delegate
                {
                    base.Loading = false;
                    base.ShowPointerWhenLoading = false;
                    DisplayCardDetail();
                });
            }
        }
        return true;
    }

    public override Boolean OnKeyRightBumper(GameObject go)
    {
        if (base.OnKeyRightBumper(go) && ButtonGroupState.ActiveGroup == CardUI.CardGroupButton)
        {
            Byte b = count[currentCardId];
            if (b > 1)
            {
                FF9Sfx.FF9SFX_Play(1047);
                offset[currentCardId] = (offset[currentCardId] + (Int32)b + 1) % (Int32)b;
                Byte[] dialogIndexes = (from i in Enumerable.Range(0, Mathf.Min((Int32)(b - 1), 4))
                                        select (Byte)i).ToArray<Byte>();
                base.ShowPointerWhenLoading = true;
                base.Loading = true;
                cardDetailTransition.TweenPingPong(dialogIndexes, (UIScene.SceneVoidDelegate)null, delegate
                {
                    base.Loading = false;
                    base.ShowPointerWhenLoading = false;
                    DisplayCardDetail();
                });
            }
        }
        return true;
    }

    public override Boolean OnKeySelect(GameObject go)
    {
        return FF9StateSystem.PCPlatform && base.OnKeySelect(go);
    }

    public override Boolean OnItemSelect(GameObject go)
    {
        if (base.OnItemSelect(go) && ButtonGroupState.ActiveGroup == CardUI.CardGroupButton)
        {
            CardUI.CardListHUD cardListHUD = cardHudList[go.transform.GetSiblingIndex()];
            CardUI.CardListHUD cardListHUD2 = (currentCardId == -1) ? null : cardHudList[currentCardId];
            Int32 id = cardListHUD.Id;
            if (currentCardId != id)
            {
                currentCardId = id;
                DisplayCardDetail();
                cardListHUD.CardHighlightAnimation.enabled = true;
                if (cardListHUD2 != null || cardListHUD.Id == cardListHUD2.Id)
                {
                    cardListHUD2.CardHighlightAnimation.enabled = false;
                }
            }
        }
        return true;
    }

    private void DisplayHelp()
    {
        HelpDespLabelGameObject.SetActive(FF9StateSystem.PCPlatform);
        if (FF9StateSystem.PCPlatform)
        {
            DiscardConfirmButton.Help.Enable = true;
            DiscardConfirmButton.Help.TextKey = "ConfirmDiscardHelp";
            DiscardConfirmButton.Help.Tail = true;

            DiscardCancelButton.Help.Enable = true;
            DiscardCancelButton.Help.TextKey = "CancelDiscardHelp";
            DiscardCancelButton.Help.Tail = true;

            if (DiscardAutoButton != null)
            {
                DiscardAutoButton.Help.Enable = true;
                DiscardAutoButton.Help.TextKey = String.Empty;
                DiscardAutoButton.Help.Text = "Discard all unnecessary cards.";
                DiscardAutoButton.Help.Tail = true;
            }

            foreach (CardUI.CardListHUD cardListHUD in cardHudList)
            {
                if (count[cardListHUD.Id] > 1)
                {
                    cardListHUD.CardButtonGroup.Help.Enable = true;
                    cardListHUD.CardButtonGroup.Help.TextKey = "SelectCard2MoreHelp";
                    cardListHUD.CardButtonGroup.Help.Tail = true;
                }
                else if (count[cardListHUD.Id] > 0)
                {
                    cardListHUD.CardButtonGroup.Help.Enable = true;
                    cardListHUD.CardButtonGroup.Help.TextKey = "SelectCard1Help";
                    cardListHUD.CardButtonGroup.Help.Tail = true;
                }
                else
                {
                    cardListHUD.CardButtonGroup.Help.Enable = false;
                    cardListHUD.CardButtonGroup.Help.TextKey = String.Empty;
                    cardListHUD.CardButtonGroup.Help.Tail = false;
                }
            }
        }
        else
        {
            DiscardConfirmButton.Help.Enable = false;
            DiscardConfirmButton.Help.TextKey = String.Empty;
            DiscardConfirmButton.Help.Tail = false;

            DiscardCancelButton.Help.Enable = false;
            DiscardCancelButton.Help.TextKey = String.Empty;
            DiscardCancelButton.Help.Tail = false;

            if (DiscardAutoButton != null)
            {
                DiscardAutoButton.Help.Enable = false;
                DiscardAutoButton.Help.TextKey = String.Empty;
                DiscardAutoButton.Help.Text = String.Empty;
                DiscardAutoButton.Help.Tail = false;
            }

            foreach (CardUI.CardListHUD cardListHUD2 in cardHudList)
            {
                cardListHUD2.CardButtonGroup.Help.Enable = false;
                cardListHUD2.CardButtonGroup.Help.TextKey = String.Empty;
                cardListHUD2.CardButtonGroup.Help.Tail = false;
            }
        }
    }

    private void DisplayCardList()
    {
        Int32 id;
        for (id = 0; id < CardPool.TOTAL_CARDS; id++)
        {
            Byte b = count[id];
            CardUI.CardListHUD cardListHUD = cardHudList.First((CardUI.CardListHUD hud) => hud.Id == id);
            if (b > 0)
            {
                CardIcon.Attribute attribute = QuadMistDatabase.MiniGame_GetCardAttribute(id);
                String spriteName = String.Concat(new Object[]
                {
                    "card_type",
                    (Int32)attribute,
                    "_",
                    (b <= 1) ? "normal" : "select"
                });
                cardListHUD.CardIconSprite.spriteName = spriteName;
                if (b > 1)
                {
                    cardListHUD.CardAmountLabel.gameObject.SetActive(true);
                    cardListHUD.CardAmountLabel.text = b.ToString();
                }
                else
                {
                    cardListHUD.CardAmountLabel.gameObject.SetActive(false);
                }
            }
            else
            {
                cardListHUD.CardAmountLabel.gameObject.SetActive(false);
                cardListHUD.CardIconSprite.spriteName = "card_slot";
            }
        }
        stockCountLabel.text = QuadMistDatabase.MiniGame_GetAllCardCount().ToString();
        typeCountLabel.text = QuadMistDatabase.MiniGame_GetCardKindCount().ToString();
    }

    private void DisplayCardDetail()
    {
        Int32 num = (Int32)count[currentCardId];
        if (num > 0)
        {
            cardInfoContentGameObject.SetActive(true);
            ShowCardDetailHudNumber(num);
            FF9UIDataTool.DisplayCard(QuadMistDatabase.MiniGame_GetCardInfoPtr((TetraMasterCardId)currentCardId, offset[currentCardId]), cardDetailHudList[0], false);
            cardNameLabel.text = FF9TextTool.CardName((TetraMasterCardId)currentCardId);
            if (num > 1)
            {
                cardNumberGameObject.SetActive(true);
                currentCardNumberLabel.text = (offset[currentCardId] + 1).ToString();
                totalCardNumberLabel.text = num.ToString();
                for (Int32 i = 1; i < Math.Min(num, 5); i++)
                {
                    FF9UIDataTool.DisplayCard(QuadMistDatabase.MiniGame_GetCardInfoPtr((TetraMasterCardId)currentCardId, 0), cardDetailHudList[i], true);
                }
            }
            else
            {
                cardNumberGameObject.SetActive(false);
            }
        }
        else
        {
            cardInfoContentGameObject.SetActive(false);
        }
    }

    private void DisplayInfo()
    {
        Int32 num = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetWinCount());
        Int32 num2 = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetLoseCount());
        Int32 num3 = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetDrawCount());
        FF9FCard_GetPoint();
        levelLabel.text = point.ToString() + "p";
        classNameLabel.text = FF9TextTool.CardLevelName(lv_collector);
        winCountLabel.text = num.ToString();
        loseCountLabel.text = num2.ToString();
        drawCountLabel.text = num3.ToString();
    }

    private void FF9FCard_Build()
    {
        for (Int32 i = 0; i < CardPool.TOTAL_CARDS; i++)
        {
            count[i] = 0;
            offset[i] = 0;
        }

        foreach (QuadMistCard quadMistCard in FF9StateSystem.MiniGame.SavedData.MiniGameCard)
            count[(Int32)quadMistCard.id]++;

        FF9FCard_GetPoint();
    }

    private void FF9FCard_GetPoint()
    {
        point = QuadMistDatabase.MiniGame_GetPlayerPoints();
        lv_collector = QuadMistDatabase.MiniGame_GetCollectorLevel();
    }

    private void ShowCardDetailHudNumber(Int32 number)
    {
        Int32 num = 0;
        foreach (CardDetailHUD cardDetailHUD in cardDetailHudList)
        {
            if (num < number)
            {
                cardDetailHUD.Self.SetActive(true);
            }
            else
            {
                cardDetailHUD.Self.SetActive(false);
            }
            num++;
        }
    }

    private void Awake()
    {
        base.FadingComponent = ScreenFadeGameObject.GetComponent<HonoFading>();
        levelLabel = PlayerInfoPanel.GetChild(0).GetChild(0).GetChild(3).GetComponent<UILabel>();
        classNameLabel = PlayerInfoPanel.GetChild(0).GetChild(1).GetComponent<UILabel>();
        winCountLabel = PlayerInfoPanel.GetChild(1).GetChild(0).GetChild(2).GetComponent<UILabel>();
        loseCountLabel = PlayerInfoPanel.GetChild(1).GetChild(1).GetChild(2).GetComponent<UILabel>();
        drawCountLabel = PlayerInfoPanel.GetChild(1).GetChild(2).GetChild(2).GetComponent<UILabel>();
        stockCountLabel = CardSelectionListPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
        typeCountLabel = CardSelectionListPanel.GetChild(1).GetChild(3).GetComponent<UILabel>();
        cardInfoContentGameObject = CardInfoPanel.GetChild(0);
        cardNumberGameObject = CardInfoPanel.GetChild(0).GetChild(1);
        currentCardNumberLabel = CardInfoPanel.GetChild(0).GetChild(1).GetChild(1).GetComponent<UILabel>();
        totalCardNumberLabel = CardInfoPanel.GetChild(0).GetChild(1).GetChild(3).GetComponent<UILabel>();
        cardNameLabel = CardInfoPanel.GetChild(0).GetChild(2).GetChild(1).GetComponent<UILabel>();
        prevOffsetButton = CardInfoPanel.GetChild(0).GetChild(1).GetChild(0).GetComponent<BoxCollider>();
        nextOffsetButton = CardInfoPanel.GetChild(0).GetChild(1).GetChild(4).GetComponent<BoxCollider>();
        Int32 cardIndex = 0;
        foreach (Transform transf in CardSelectionListPanel.transform.GetChild(0))
        {
            Int32 invertIndex = cardIndex % 10 * 10;
            invertIndex += cardIndex / 10;
            cardIndex++;
            CardUI.CardListHUD cardListHUD = new CardUI.CardListHUD(transf.gameObject, invertIndex);
            cardHudList.Add(cardListHUD);
            cardListHUD.CardHighlightAnimation.enabled = false;
            UIEventListener.Get(cardListHUD.Self).onClick += onClick;
        }
        foreach (Transform transf in CardInfoPanel.GetChild(0).GetChild(0).transform)
        {
            CardDetailHUD item = new CardDetailHUD(transf.gameObject);
            cardDetailHudList.Add(item);
        }
        UIEventListener.Get(DeleteSubmenuButton).Click += onClick;

        _uiDiscardDialog = new UiDiscardDialog(DeleteDialogGameObject);
        _uiDiscardDialog.Content.Confirm.UiEventListener.onClick += onClick;
        _uiDiscardDialog.Content.Cancel.UiEventListener.onClick += onClick;

        if (_uiDiscardDialog.Content.Auto != null)
            _uiDiscardDialog.Content.Auto.UiEventListener.onClick += onClick;

        cardDetailTransition = TransitionPanel.GetChild(0).GetComponent<HonoTweenPosition>();
        deleteDialogTransition = TransitionPanel.GetChild(1).GetComponent<HonoTweenClipping>();
        UILabel component = PlayerInfoPanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<UILabel>();
        component.SetAnchor((Transform)null);
        component.width = 332;
        component.height = 40;
    }

    private static String CardGroupButton = "Card.Card";

    private static String DiscardDialogButtonGroup = "Card.Choice";

    private static Int32 FF9FCARD_ARROW_TYPE_MAX = 256;

    private static Int32 FF9FCAZRD_LV_MAX = 32;

    public GameObject CardSelectionListPanel;

    public GameObject PlayerInfoPanel;

    public GameObject CardInfoPanel;

    public GameObject DeleteDialogGameObject;

    public GameObject DeleteSubmenuButton;

    public GameObject HelpDespLabelGameObject;

    public GameObject TransitionPanel;

    public GameObject ScreenFadeGameObject;

    private UILabel levelLabel;

    private UILabel classNameLabel;

    private UILabel winCountLabel;

    private UILabel loseCountLabel;

    private UILabel drawCountLabel;

    private UILabel stockCountLabel;

    private UILabel typeCountLabel;

    private GameObject cardInfoContentGameObject;

    private GameObject cardNumberGameObject;

    //private ButtonGroupState discardConfirmButton;

    //private ButtonGroupState discardCancelButton;

    private UILabel currentCardNumberLabel;

    private UILabel totalCardNumberLabel;

    private UILabel cardNameLabel;

    private BoxCollider prevOffsetButton;

    private BoxCollider nextOffsetButton;

    private HonoTweenPosition cardDetailTransition;

    private HonoTweenClipping deleteDialogTransition;

    private List<CardUI.CardListHUD> cardHudList = new List<CardUI.CardListHUD>();

    private List<CardDetailHUD> cardDetailHudList = new List<CardDetailHUD>();

    private Boolean fastSwitch;

    private Int32 currentCardId;

    private Int32 deleteCardId;

    private Int32[] offset = new Int32[CardPool.TOTAL_CARDS];

    private Byte[] count = new Byte[CardPool.TOTAL_CARDS];

    private Int32 lv_collector;

    private Int32 point;

    private UiDiscardDialog _uiDiscardDialog;

    private ButtonGroupState DiscardConfirmButton => _uiDiscardDialog.Content.Confirm.GroupState;
    private ButtonGroupState DiscardCancelButton => _uiDiscardDialog.Content.Cancel.GroupState;
    private ButtonGroupState DiscardAutoButton => _uiDiscardDialog.Content.Auto?.GroupState;

    public class CardListHUD
    {
        public CardListHUD(GameObject go, Int32 id)
        {
            Self = go;
            Id = id;
            CardButtonGroup = go.GetComponent<ButtonGroupState>();
            CardIconSprite = go.GetChild(0).GetComponent<UISprite>();
            CardAmountLabel = go.GetChild(1).GetComponent<UILabel>();
            CardHighlightAnimation = go.GetChild(2).GetComponent<UISpriteAnimation>();
        }

        public GameObject Self;

        public Int32 Id;

        public UISprite CardIconSprite;

        public UILabel CardAmountLabel;

        public UISpriteAnimation CardHighlightAnimation;

        public ButtonGroupState CardButtonGroup;
    }

    private sealed class UiDiscardDialog
    {
        public readonly TextPanel Content;
        public readonly FrameBackground Background;
        public readonly UIPanel Panel;

        public UiDiscardDialog(GameObject obj)
        {
            Content = new TextPanel(obj.GetChild(0));
            Background = new FrameBackground(obj.GetChild(1));
            Panel = obj.GetExactComponent<UIPanel>();

            if (Configuration.TetraMaster.DiscardAutoButton)
                AddAutoButton();
        }

        public void AddAutoButton()
        {
            Panel.clipping = UIDrawCall.Clipping.None;

            Background.OnAutoButtonAdded();
            Content.OnAutoButtonAdded();
        }

        public sealed class TextPanel : GOBase
        {
            public readonly GOLocalizableLabel Question;
            public readonly Button Confirm;
            public readonly Button Cancel;
            public Button Auto;

            public TextPanel(GameObject obj)
                : base(obj)
            {
                Question = new GOLocalizableLabel(obj.GetChild(0));
                Confirm = new Button(obj.GetChild(1));
                Cancel = new Button(obj.GetChild(2));
            }

            public void OnAutoButtonAdded()
            {
                Transform.localPosition = new Vector3(-13, 40, 0);
                Question.Transform.localPosition = new Vector3(-90, Question.Transform.localPosition.y, Question.Transform.localPosition.z);

                GameObject autoGo = Instantiate(Cancel.GameObject);
                autoGo.name = "Auto";
                autoGo.transform.SetParent(Transform, false);

                Auto = new Button(autoGo);
                Auto.Transform.localPosition = new Vector3(0, -386f, 0);
                Auto.UiLocalize.key = null;
                Auto.UiLabel.text = "Auto";

                Confirm.OnAutoButtonAdded();
                Cancel.OnAutoButtonAdded();
                Auto.OnAutoButtonAdded();
            }

            public sealed class Button : GOBase
            {
                public readonly ButtonGroupState GroupState;
                public readonly UIButton UiButton;
                public readonly UILocalize UiLocalize;
                public readonly UILabel UiLabel;
                public readonly UIEventListener UiEventListener;

                public Button(GameObject obj)
                    : base(obj)
                {
                    GroupState = obj.GetExactComponent<ButtonGroupState>();
                    UiButton = obj.GetExactComponent<UIButton>();
                    UiLocalize = obj.GetExactComponent<UILocalize>();
                    UiLabel = obj.GetExactComponent<UILabel>();
                    UiEventListener = obj.EnsureExactComponent<UIEventListener>();
                    if (Configuration.Control.WrapSomeMenus)
                        obj.GetExactComponent<UIKeyNavigation>().wrapUpDown = true;
                }

                public void OnAutoButtonAdded()
                {
                    UiLocalize.TextOverwriting += OnTextOverwriting;
                    Transform.localPosition = new Vector3(0, Transform.localPosition.y, Transform.localPosition.z);
                }

                private String OnTextOverwriting(String key, String text)
                {
                    if (text.StartsWith("[CENT]"))
                        return text;

                    return "[CENT]" + text;
                }
            }
        }

        public sealed class FrameBackground : GOWidget
        {
            public FrameBackground(GameObject obj)
                : base(obj)
            {
            }

            public void OnAutoButtonAdded()
            {
                Widget.height = 408;
            }
        }
    }
}
