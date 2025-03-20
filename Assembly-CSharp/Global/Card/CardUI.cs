using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Scenes;
using Memoria.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardUI : UIScene
{
    public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate afterShowDelegate = delegate
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.SubMenuPanel.SetActive(false);
            ButtonGroupState.SetPointerDepthToGroup(2, CardUI.CardGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(60f, 10f), CardUI.DiscardDialogButtonGroup);
            ButtonGroupState.ActiveGroup = CardUI.CardGroupButton;
        };
        if (afterFinished != null)
            afterShowDelegate += afterFinished;
        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(afterShowDelegate);
        UpdateUserInterface();
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

    private void UpdateUserInterface()
    {
        String colon = NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft ? "" : ":";
        infoPanel.CollectorLevelSprite.alpha = NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft ? 0f : 1f;
        infoPanel.CollectorLevelColon.rawText = colon;
        infoPanel.WinColon.rawText = colon;
        infoPanel.LoseColon.rawText = colon;
        infoPanel.DrawColon.rawText = colon;
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
                        _uiDiscardDialog.ShowMessage(true);
                    });
                    _uiDiscardDialog.ShowMessage(false);
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
                    return true;
                OnDiscardDialogKeyConfirm(go);
                base.Loading = true;
                deleteDialogTransition.TweenOut(delegate
                {
                    base.Loading = false;
                    ButtonGroupState.ActiveGroup = CardUI.CardGroupButton;
                });
                _uiDiscardDialog.ShowMessage(false);
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
                DiscardSelectedCard();
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

    private void DiscardSelectedCard()
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
                    return true;
                FF9Sfx.FF9SFX_Play(101);
                base.Loading = true;
                deleteDialogTransition.TweenOut(delegate
                {
                    base.Loading = false;
                    ButtonGroupState.ActiveGroup = CardUI.CardGroupButton;
                });
                _uiDiscardDialog.ShowMessage(false);
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
            Byte selectedCardCount = count[currentCardId];
            if (selectedCardCount > 1)
            {
                FF9Sfx.FF9SFX_Play(1047);
                offset[currentCardId] = (offset[currentCardId] + selectedCardCount - 1) % selectedCardCount;
                Byte[] dialogIndexes = (from i in Enumerable.Range(0, Mathf.Min(selectedCardCount - 1, 4))
                                        select (Byte)i).ToArray();
                base.ShowPointerWhenLoading = true;
                base.Loading = true;
                cardDetailTransition.TweenPingPong(dialogIndexes, null, delegate
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
            Byte selectedCardCount = count[currentCardId];
            if (selectedCardCount > 1)
            {
                FF9Sfx.FF9SFX_Play(1047);
                offset[currentCardId] = (offset[currentCardId] + selectedCardCount + 1) % selectedCardCount;
                Byte[] dialogIndexes = (from i in Enumerable.Range(0, Mathf.Min(selectedCardCount - 1, 4))
                                        select (Byte)i).ToArray();
                base.ShowPointerWhenLoading = true;
                base.Loading = true;
                cardDetailTransition.TweenPingPong(dialogIndexes, null, delegate
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
            CardUI.CardListHUD newSelectionHUD = cardHudList[go.transform.GetSiblingIndex()];
            CardUI.CardListHUD oldSelectionHUD = currentCardId == -1 ? null : cardHudList[currentCardId];
            Int32 id = newSelectionHUD.Id;
            if (currentCardId != id)
            {
                currentCardId = id;
                DisplayCardDetail();
                newSelectionHUD.CardHighlightAnimation.enabled = true;
                if (oldSelectionHUD != null || newSelectionHUD.Id == oldSelectionHUD.Id)
                    oldSelectionHUD.CardHighlightAnimation.enabled = false;
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
        for (Int32 id = 0; id < CardPool.TOTAL_CARDS; id++)
        {
            Byte selectedCardCount = count[id];
            CardUI.CardListHUD cardListHUD = cardHudList.First((CardUI.CardListHUD hud) => hud.Id == id);
            if (selectedCardCount > 0)
            {
                CardIcon.Attribute attribute = QuadMistDatabase.MiniGame_GetCardAttribute(id);
                String spriteName = $"card_type{(Int32)attribute}_" + (selectedCardCount <= 1 ? "normal" : "select");
                cardListHUD.CardIconSprite.spriteName = spriteName;
                if (selectedCardCount > 1)
                {
                    cardListHUD.CardAmountLabel.gameObject.SetActive(true);
                    cardListHUD.CardAmountLabel.rawText = selectedCardCount.ToString();
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
        stockCountLabel.rawText = QuadMistDatabase.MiniGame_GetAllCardCount().ToString();
        typeCountLabel.rawText = QuadMistDatabase.MiniGame_GetCardKindCount().ToString();
    }

    private void DisplayCardDetail()
    {
        Int32 selectedCardCount = count[currentCardId];
        if (selectedCardCount > 0)
        {
            cardInfoContentGameObject.SetActive(true);
            ShowCardDetailHudNumber(selectedCardCount);
            FF9UIDataTool.DisplayCard(QuadMistDatabase.MiniGame_GetCardInfoPtr((TetraMasterCardId)currentCardId, offset[currentCardId]), cardDetailHudList[0], false);
            cardNameLabel.rawText = FF9TextTool.CardName((TetraMasterCardId)currentCardId);
            if (selectedCardCount > 1)
            {
                cardNumberGameObject.SetActive(true);
                currentCardNumberLabel.rawText = (offset[currentCardId] + 1).ToString();
                totalCardNumberLabel.rawText = selectedCardCount.ToString();
                for (Int32 i = 1; i < Math.Min(selectedCardCount, 5); i++)
                    FF9UIDataTool.DisplayCard(QuadMistDatabase.MiniGame_GetCardInfoPtr((TetraMasterCardId)currentCardId, 0), cardDetailHudList[i], true);
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
        Int32 winCount = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetWinCount());
        Int32 loseCount = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetLoseCount());
        Int32 drawCount = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetDrawCount());
        FF9FCard_GetPoint();
        levelLabel.rawText = Localization.GetWithDefault("CardPoints").Replace("%", point.ToString());
        classNameLabel.rawText = FF9TextTool.CardLevelName(lv_collector);
        winCountLabel.rawText = winCount.ToString();
        loseCountLabel.rawText = loseCount.ToString();
        drawCountLabel.rawText = drawCount.ToString();
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
        for (Int32 i = 0; i < cardDetailHudList.Count; i++)
            cardDetailHudList[i].Self.SetActive(i < number);
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
        cardDetailTransition = TransitionPanel.GetChild(0).GetComponent<HonoTweenPosition>();
        deleteDialogTransition = TransitionPanel.GetChild(1).GetComponent<HonoTweenClipping>();
        foreach (Transform transf in CardInfoPanel.GetChild(0).GetChild(0).transform)
            cardDetailHudList.Add(new CardDetailHUD(transf.gameObject));
        UIEventListener.Get(DeleteSubmenuButton).Click += onClick;
        _uiDiscardDialog = new DiscardDialogHUD(DeleteDialogGameObject);
        foreach (var button in _uiDiscardDialog.Choices)
            button.EventListener.onClick += onClick;

        infoPanel = new GOCardPlayerInfoPanel(PlayerInfoPanel);
        background = new GOMenuBackground(transform.GetChild(6).gameObject, "card_bg");

        cardNameLabel.rightAnchor.Set(CardInfoPanel.transform, 1f, -40);
        cardNameLabel.fixedAlignment = true;
        cardNameLabel.overflowMethod = UILabel.Overflow.ShrinkContent;
        totalCardNumberLabel.fixedAlignment = true;
        CardInfoPanel.GetChild(1).GetChild(2).GetComponent<UILabel>().rightAnchor.Set(1f, -40);
    }

    private const String CardGroupButton = "Card.Card";
    private const String DiscardDialogButtonGroup = "Card.Choice";
    private const Int32 FF9FCARD_ARROW_TYPE_MAX = 256;
    private const Int32 FF9FCAZRD_LV_MAX = 32;

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
    [NonSerialized]
    private GOMenuBackground background;
    [NonSerialized]
    private GOCardPlayerInfoPanel infoPanel;

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

    private DiscardDialogHUD _uiDiscardDialog;

    private ButtonGroupState DiscardConfirmButton => _uiDiscardDialog.Choices[0].ButtonGroup;
    private ButtonGroupState DiscardCancelButton => _uiDiscardDialog.Choices[1].ButtonGroup;
    private ButtonGroupState DiscardAutoButton => _uiDiscardDialog.Choices.Count >= 3 ? _uiDiscardDialog.Choices[2].ButtonGroup : null;

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

    private class GOCardPlayerInfoPanel : GOWidget
    {
        public GOLocalizableLabel CollectorLevelLabel;
        public UISprite CollectorLevelSprite;
        public UILabel CollectorLevelColon;
        public UILabel CollectorLevelValue;
        public UILabel CollectorClass;
        public UISprite CollectorSpliter;
        public GOLocalizableLabel WinLabel;
        public UILabel WinColon;
        public UILabel WinValue;
        public GOLocalizableLabel LoseLabel;
        public UILabel LoseColon;
        public UILabel LoseValue;
        public GOLocalizableLabel DrawLabel;
        public UILabel DrawColon;
        public UILabel DrawValue;
        public GOFrameBackground Background;

        public GOCardPlayerInfoPanel(GameObject go) : base(go)
        {
            CollectorLevelLabel = new GOLocalizableLabel(go.GetChild(0).GetChild(0).GetChild(0));
            CollectorLevelSprite = go.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UISprite>();
            CollectorLevelColon = go.GetChild(0).GetChild(0).GetChild(2).GetComponent<UILabel>();
            CollectorLevelValue = go.GetChild(0).GetChild(0).GetChild(3).GetComponent<UILabel>();
            CollectorClass = go.GetChild(0).GetChild(1).GetComponent<UILabel>();
            CollectorSpliter = go.GetChild(0).GetChild(2).GetComponent<UISprite>();
            WinLabel = new GOLocalizableLabel(go.GetChild(1).GetChild(0).GetChild(0));
            WinColon = go.GetChild(1).GetChild(0).GetChild(1).GetComponent<UILabel>();
            WinValue = go.GetChild(1).GetChild(0).GetChild(2).GetComponent<UILabel>();
            LoseLabel = new GOLocalizableLabel(go.GetChild(1).GetChild(1).GetChild(0));
            LoseColon = go.GetChild(1).GetChild(1).GetChild(1).GetComponent<UILabel>();
            LoseValue = go.GetChild(1).GetChild(1).GetChild(2).GetComponent<UILabel>();
            DrawLabel = new GOLocalizableLabel(go.GetChild(1).GetChild(2).GetChild(0));
            DrawColon = go.GetChild(1).GetChild(2).GetChild(1).GetComponent<UILabel>();
            DrawValue = go.GetChild(1).GetChild(2).GetChild(2).GetComponent<UILabel>();
            Background = new GOFrameBackground(go.GetChild(2));
            CollectorLevelLabel.Label.SetAnchor((Transform)null);
            CollectorLevelLabel.Label.width = 332;
            CollectorLevelLabel.Label.height = 40;
            Background.Caption.Label.rightAnchor.Set(1f, -40);
        }
    }

    private sealed class DiscardDialogHUD : GOPanel
    {
        public readonly UIPanel TextPanel;
        public readonly GOLocalizableLabel Label;
        public readonly List<GOSimpleButton<GOLocalizableLabel>> Choices;
        public readonly GOFrameBackground Background;

        public DiscardDialogHUD(GameObject go) : base(go)
        {
            TextPanel = go.GetChild(0).GetComponent<UIPanel>();
            Label = new GOLocalizableLabel(go.GetChild(0).GetChild(0));
            Int32 choiceCount = go.GetChild(0).transform.childCount - 1;
            Choices = new List<GOSimpleButton<GOLocalizableLabel>>(choiceCount);
            for (Int32 i = 0; i < choiceCount; i++)
                Choices.Add(new GOSimpleButton<GOLocalizableLabel>(go.GetChild(0).GetChild(i + 1)));
            Background = new GOFrameBackground(go.GetChild(1));

            if (Configuration.TetraMaster.DiscardAutoButton)
                AddAutoButton();

            foreach (var button in Choices)
            {
                button.Content.Localize.TextOverwriting += PrependCenterText;
                button.Transform.localPosition = new Vector3(0, button.Transform.localPosition.y, button.Transform.localPosition.z);
            }
        }

        public void AddAutoButton()
        {
            Panel.clipping = UIDrawCall.Clipping.None;
            Background.Widget.height = 408;
            TextPanel.transform.localPosition = new Vector3(-13, 40, 0);
            Label.Transform.localPosition = new Vector3(-90, Label.Transform.localPosition.y, Label.Transform.localPosition.z);
            GameObject autoGo = UnityEngine.Object.Instantiate(Choices[Choices.Count - 1].GameObject);
            GOSimpleButton<GOLocalizableLabel> autoButton = new GOSimpleButton<GOLocalizableLabel>(autoGo);
            autoGo.name = "Auto";
            autoGo.transform.SetParent(TextPanel.transform, false);
            autoGo.transform.localPosition = new Vector3(0, -386f, 0);
            autoButton.Content.Localize.key = "AutoBattleCamera";
            Choices.Add(autoButton);
        }

        public void ShowMessage(Boolean show)
        {
            TextPanel.gameObject.SetActive(show);
        }

        private static void PrependCenterText(String key, ref String text)
        {
            if (!text.StartsWith("[CENT]"))
                text = "[CENT]" + text;
        }
    }
}
