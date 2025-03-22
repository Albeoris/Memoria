using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadMistUI : UIScene
{
    public Boolean AllowGameCancel => FF9StateSystem.Common.FF9.miniGameArg != 124 && FF9StateSystem.Common.FF9.miniGameArg != 125 && FF9StateSystem.Common.FF9.miniGameArg != 126 && FF9StateSystem.Common.FF9.miniGameArg != 127;

    public QuadMistUI.CardState State
    {
        set => currentState = value;
    }

    public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate afterFinishedDelegate = delegate
        {
            if (currentState == QuadMistUI.CardState.CardSelection)
            {
                if (PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.Tutorial)
                    ButtonGroupState.ActiveGroup = QuadMistUI.CardGroupButton;
            }
            else
            {
                String phrase = Localization.Get("QuadMistDiscardNotice");
                Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(phrase, 100, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(105f, 90f), Dialog.CaptionType.None);
                dialog.AfterDialogHidden = onDiscardNoticeHidden;
                isDialogShowing = true;
            }
        };
        if (afterFinished != null)
            afterFinishedDelegate += afterFinished;
        base.NextSceneIsModal = !isNeedToBuildCard;
        base.Show(afterFinishedDelegate);
        if (isNeedToBuildCard)
        {
            currentCardId = 0;
            currentCardOffset = 0;
            isNeedToBuildCard = false;
            BuildCard();
            UpdateAmountLabel();
        }
        textAtlas = AssetManager.Load<UIAtlas>("EmbeddedAsset/QuadMist/Atlas/QuadMist Text " + Localization.GetSymbol() + " Atlas", false);
        PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(false);
        DisplayInfo();
        DisplayCardList();
        if (currentState == QuadMistUI.CardState.CardSelection)
        {
            DisplayCardDetail();
            DiscardTitle.SetActive(false);
            TitleSprite.atlas = textAtlas;
            TitleSprite.gameObject.SetActive(true);
            CardSelectedPanel.SetActive(true);
            BackButton.SetActive(AllowGameCancel);
        }
        else
        {
            TitleSprite.gameObject.SetActive(false);
            DiscardTitle.SetActive(true);
            discardTitleSprite.atlas = textAtlas;
            UISprite[] array = discardTitleBulletSprite;
            foreach (UISprite sprite in discardTitleBulletSprite)
                sprite.atlas = textAtlas;
            switch (Localization.GetSymbol())
            {
                case "US":
                case "UK":
                    discardTitleBulletWidget[0].leftAnchor.absolute = 128;
                    discardTitleBulletWidget[0].rightAnchor.absolute = 186;
                    discardTitleBulletWidget[1].leftAnchor.absolute = -186;
                    discardTitleBulletWidget[1].rightAnchor.absolute = -128;
                    break;
                case "JP":
                    discardTitleBulletWidget[0].leftAnchor.absolute = 0;
                    discardTitleBulletWidget[0].rightAnchor.absolute = 58;
                    discardTitleBulletWidget[1].leftAnchor.absolute = -58;
                    discardTitleBulletWidget[1].rightAnchor.absolute = 0;
                    break;
                case "ES":
                case "GR":
                case "IT":
                    discardTitleBulletWidget[0].leftAnchor.absolute = -58;
                    discardTitleBulletWidget[0].rightAnchor.absolute = 0;
                    discardTitleBulletWidget[1].leftAnchor.absolute = 0;
                    discardTitleBulletWidget[1].rightAnchor.absolute = 58;
                    break;
                case "FR":
                    discardTitleBulletWidget[0].leftAnchor.absolute = -28;
                    discardTitleBulletWidget[0].rightAnchor.absolute = 30;
                    discardTitleBulletWidget[1].leftAnchor.absolute = -30;
                    discardTitleBulletWidget[1].rightAnchor.absolute = 28;
                    break;
            }
            discardTitleBulletWidget[0].UpdateAnchors();
            discardTitleBulletWidget[1].UpdateAnchors();
            CardSelectedPanel.SetActive(false);
            BackButton.SetActive(false);
        }
    }

    public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate afterFinishedDelegate = delegate
        {
            PersistenSingleton<UIManager>.Instance.State = UIManager.UIState.QuadMistBattle;
        };
        if (afterFinished != null)
            afterFinishedDelegate += afterFinished;
        base.NextSceneIsModal = false;
        base.Hide(afterFinishedDelegate);
        RemoveCursorMemorize();
        cardInfoContentGameObject.SetActive(false);
    }

    private void RemoveCursorMemorize()
    {
        ButtonGroupState.RemoveCursorMemorize(QuadMistUI.CardGroupButton);
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go) && ButtonGroupState.ActiveGroup == QuadMistUI.CardGroupButton)
        {
            if (currentState == QuadMistUI.CardState.CardSelection)
            {
                if (count[currentCardId] > 0)
                {
                    FF9Sfx.FF9SFX_Play(1047);
                    SelectCard(currentCardId, currentCardOffset);
                    currentCardOffset = 0;
                    DisplayCardList();
                    DisplayCardDetail();
                    if (selectedCardList.Count > 4)
                    {
                        cardInfoContentGameObject.SetActive(false);
                        String phrase = Localization.Get("QuadMistConfirmSelection");
                        Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(phrase, 100, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(105f, 90f), Dialog.CaptionType.None);
                        dialog.AfterDialogHidden = onConfirmDialogHidden;
                    }
                }
            }
            else if (count[currentCardId] > 0)
            {
                deleteCardId = currentCardId;
                ETb.sChoose = 1;
                String phrase = Localization.Get("QuadMistDiscardConfirm");
                Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(phrase, 100, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(194f, 38f), Dialog.CaptionType.None);
                dialog.AfterDialogHidden = onDiscardConfirmHidden;
            }
        }
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (base.OnKeyCancel(go) && ButtonGroupState.ActiveGroup == QuadMistUI.CardGroupButton && currentState == QuadMistUI.CardState.CardSelection)
        {
            FF9Sfx.FF9SFX_Play(101);
            if (selectedCardList.Count > 0)
            {
                DeselectCard(selectedCardList.Count - 1);
                DisplayCardList();
                DisplayCardDetail();
            }
            else if (AllowGameCancel)
            {
                ETb.sChoose = 1;
                String phrase = Localization.Get("QuadMistEndCardGame");
                Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(phrase, 100, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(105f, 90f), Dialog.CaptionType.None);
                dialog.AfterDialogHidden = onQuitDialogHidden;
            }
        }
        return true;
    }

    public override Boolean OnKeySelect(GameObject go)
    {
        return false;
    }

    private void OnPressSelectedCard(GameObject go, Boolean isPressed)
    {
        if (!isPressed)
        {
            Int32 siblingIndex = go.transform.GetSiblingIndex();
            if (siblingIndex < selectedCardList.Count)
            {
                FF9Sfx.FF9SFX_Play(101);
                QuadMistCard quadMistCard = selectedCardList[siblingIndex];
                selectedCardList.RemoveAt(siblingIndex);
                QuadMistUI.allCardList.Add(quadMistCard);
                count[(Int32)quadMistCard.id]++;
                QuadMistGame.UpdateSelectedCardList(selectedCardList);
                DisplayCardList();
            }
        }
    }

    private void OnClickSelectedCard(GameObject go)
    {
        Int32 siblingIndex = go.transform.GetSiblingIndex();
        if (siblingIndex < selectedCardList.Count)
        {
            FF9Sfx.FF9SFX_Play(101);
            QuadMistCard quadMistCard = selectedCardList[siblingIndex];
            selectedCardList.RemoveAt(siblingIndex);
            QuadMistUI.allCardList.Add(quadMistCard);
            count[(Int32)quadMistCard.id]++;
            QuadMistGame.UpdateSelectedCardList(selectedCardList);
            DisplayCardList();
            DisplayCardDetail();
        }
    }

    public override Boolean OnKeyLeftBumper(GameObject go)
    {
        if (base.OnKeyLeftBumper(go) && ButtonGroupState.ActiveGroup == QuadMistUI.CardGroupButton)
        {
            Byte ccount = count[currentCardId];
            if (ccount > 1)
            {
                FF9Sfx.FF9SFX_Play(1047);
                currentCardOffset = (currentCardOffset + ccount - 1) % ccount;
                Byte[] dialogIndexes = (from i in Enumerable.Range(0, Mathf.Min(ccount - 1, 4))
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
        if (base.OnKeyRightBumper(go) && ButtonGroupState.ActiveGroup == QuadMistUI.CardGroupButton)
        {
            Byte ccount = count[currentCardId];
            if (ccount > 1)
            {
                FF9Sfx.FF9SFX_Play(1047);
                currentCardOffset = (currentCardOffset + ccount + 1) % ccount;
                Byte[] dialogIndexes = (from i in Enumerable.Range(0, Mathf.Min(ccount - 1, 4))
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

    public override Boolean OnItemSelect(GameObject go)
    {
        if (base.OnItemSelect(go) && ButtonGroupState.ActiveGroup == QuadMistUI.CardGroupButton)
        {
            Int32 id = cardHudList[go.transform.GetSiblingIndex()].Id;
            if (currentCardId != id)
            {
                currentCardId = id;
                currentCardOffset = 0;
                DisplayCardDetail();
            }
        }
        return true;
    }

    private void onConfirmDialogHidden(Int32 choice)
    {
        if (choice == 0)
        {
            Hide(delegate
            {
                QuadMistGame.OnFinishSelectCardUI(selectedCardList);
                isNeedToBuildCard = true;
            });
        }
        else
        {
            DeselectCard(4);
            currentCardOffset = 0;
            cardInfoContentGameObject.SetActive(true);
            DisplayCardList();
            DisplayCardDetail();
            ButtonGroupState.ActiveGroup = QuadMistUI.CardGroupButton;
        }
    }

    private void onQuitDialogHidden(Int32 choice)
    {
        if (choice == 0)
        {
            isNeedToBuildCard = true;
            base.Loading = true;
            QuadMistGame.main.QuitQuadMist();
        }
        else
        {
            ButtonGroupState.ActiveGroup = QuadMistUI.CardGroupButton;
        }
    }

    private void onDiscardNoticeHidden(Int32 choice)
    {
        isDialogShowing = false;
        DisplayCardDetail();
        ButtonGroupState.ActiveGroup = QuadMistUI.CardGroupButton;
    }

    private void onDiscardConfirmHidden(Int32 choice)
    {
        if (choice == 0)
        {
            QuadMistDatabase.MiniGame_AwayCard((TetraMasterCardId)deleteCardId, currentCardOffset);
            if (QuadMistDatabase.MiniGame_GetAllCardCount() > Configuration.TetraMaster.MaxCardCount)
            {
                QuadMistUI.allCardList.Remove(GetCardInfo(currentCardId, currentCardOffset));
                count[deleteCardId]--;
                currentCardOffset = 0;
                DisplayCardList();
                DisplayCardDetail();
                ButtonGroupState.ActiveGroup = QuadMistUI.CardGroupButton;
                UpdateAmountLabel();
            }
            else
            {
                Hide(delegate
                {
                    QuadMistGame.OnDiscardFinish();
                    isNeedToBuildCard = true;
                });
            }
        }
        else
        {
            ButtonGroupState.ActiveGroup = QuadMistUI.CardGroupButton;
        }
    }

    private void DisplayCardList()
    {
        for (Int32 id = 0; id < CardPool.TOTAL_CARDS; id++)
        {
            Byte kindCount = count[id];
            QuadMistUI.CardListHUD cardListHUD = cardHudList.First(hud => hud.Id == id);
            if (kindCount > 0)
            {
                CardIcon.Attribute attribute = QuadMistDatabase.MiniGame_GetCardAttribute(id);
                String spriteName = $"card_type{(Int32)attribute}_{(kindCount <= 1 ? "normal" : "select")}";
                cardListHUD.CardIconSprite.spriteName = spriteName;
                if (kindCount > 1)
                {
                    cardListHUD.CardAmountLabel.gameObject.SetActive(true);
                    cardListHUD.CardAmountLabel.rawText = kindCount.ToString();
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
    }

    private void DisplayCardDetail()
    {
        Int32 ccount = count[currentCardId];
        if (ccount > 0)
        {
            cardInfoContentGameObject.SetActive(true);
            ShowCardDetailHudAmount(ccount);
            cardIdLabel.rawText = $"No{currentCardId + 1:0#}";
            FF9UIDataTool.DisplayCard(GetCardInfo(currentCardId, currentCardOffset), cardDetailHudList[0], false);
            cardNameLabel.rawText = FF9TextTool.CardName((TetraMasterCardId)currentCardId);
            if (ccount > 1)
            {
                cardNumberGameObject.SetActive(true);
                currentCardNumberLabel.rawText = (currentCardOffset + 1).ToString();
                totalCardNumberLabel.rawText = ccount.ToString();
                for (Int32 i = 1; i < Math.Min(ccount, 5); i++)
                    FF9UIDataTool.DisplayCard(GetCardInfo(currentCardId, 0), cardDetailHudList[i], true);
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
        if (currentState == QuadMistUI.CardState.CardSelection)
        {
            PlayerInfoPanel.SetActive(true);
            winCountLabel.rawText = Math.Min(99999, QuadMistDatabase.MiniGame_GetWinCount()).ToString();
            loseCountLabel.rawText = Math.Min(99999, QuadMistDatabase.MiniGame_GetLoseCount()).ToString();
            drawCountLabel.rawText = Math.Min(99999, QuadMistDatabase.MiniGame_GetDrawCount()).ToString();
        }
        else
        {
            PlayerInfoPanel.SetActive(false);
        }
    }

    private void BuildCard()
    {
        List<QuadMistCard> fullList = new List<QuadMistCard>();
        QuadMistGame.main.preBoard.collection.CreateCards();
        QuadMistGame.main.preBoard.UpdateCollection(-1);
        List<QuadMistCard>[] cards = QuadMistGame.main.preBoard.collection.cards;
        for (Int32 i = 0; i < cards.Length; i++)
            foreach (QuadMistCard quadMistCard in cards[i])
                if (quadMistCard != null)
                    fullList.Add(quadMistCard);
        QuadMistUI.allCardList = fullList;
        selectedCardList.Clear();
        for (Int32 i = 0; i < CardPool.TOTAL_CARDS; i++)
            count[i] = (Byte)QuadMistDatabase.MiniGame_GetCardCount((TetraMasterCardId)i);
    }

    private QuadMistCard GetCardInfo(Int32 id, Int32 offset)
    {
        Int32 idIndex = 0;
        foreach (QuadMistCard quadMistCard in QuadMistUI.allCardList)
        {
            if ((Int32)quadMistCard.id == id)
            {
                if (offset == idIndex)
                    return quadMistCard;
                idIndex++;
            }
        }
        return null;
    }

    private void ShowCardDetailHudAmount(Int32 number)
    {
        Int32 num = 0;
        foreach (CardDetailHUD cardDetailHUD in cardDetailHudList)
        {
            cardDetailHUD.Self.SetActive(num < number);
            num++;
        }
    }

    private void SelectCard(Int32 id, Int32 offset)
    {
        QuadMistCard cardInfo = GetCardInfo(id, offset);
        selectedCardList.Add(cardInfo);
        QuadMistUI.allCardList.Remove(cardInfo);
        count[id]--;
        QuadMistGame.UpdateSelectedCardList(selectedCardList);
        DisplayCardList();
    }

    private void DeselectCard(Int32 index)
    {
        QuadMistCard quadMistCard = selectedCardList[index];
        selectedCardList.Remove(quadMistCard);
        QuadMistUI.allCardList.Add(quadMistCard);
        count[(Int32)quadMistCard.id]++;
        QuadMistGame.UpdateSelectedCardList(selectedCardList);
        DisplayCardList();
    }

    private void UpdateAmountLabel()
    {
        stockCountLabel.rawText = QuadMistDatabase.MiniGame_GetAllCardCount().ToString();
        typeCountLabel.rawText = QuadMistDatabase.MiniGame_GetCardKindCount().ToString();
    }

    private void Awake()
    {
        base.FadingComponent = ScreenFadeGameObject.GetComponent<HonoFading>();
        winCountLabel = PlayerInfoPanel.GetChild(0).GetChild(1).GetComponent<UILabel>();
        loseCountLabel = PlayerInfoPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
        drawCountLabel = PlayerInfoPanel.GetChild(2).GetChild(1).GetComponent<UILabel>();
        stockCountLabel = CardSelectionListPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
        typeCountLabel = CardSelectionListPanel.GetChild(1).GetChild(3).GetComponent<UILabel>();
        cardInfoContentGameObject = CardInfoPanel.GetChild(0);
        cardNumberGameObject = CardInfoPanel.GetChild(0).GetChild(2);
        cardIdLabel = CardInfoPanel.GetChild(0).GetChild(0).GetComponent<UILabel>();
        currentCardNumberLabel = CardInfoPanel.GetChild(0).GetChild(2).GetChild(1).GetComponent<UILabel>();
        totalCardNumberLabel = CardInfoPanel.GetChild(0).GetChild(2).GetChild(3).GetComponent<UILabel>();
        cardNameLabel = CardInfoPanel.GetChild(0).GetChild(3).GetComponent<UILabel>();
        prevOffsetButton = CardInfoPanel.GetChild(0).GetChild(1).GetChild(0).GetComponent<BoxCollider>();
        nextOffsetButton = CardInfoPanel.GetChild(0).GetChild(1).GetChild(4).GetComponent<BoxCollider>();
        discardTitleSprite = DiscardTitle.GetComponent<UISprite>();
        discardTitleBulletSprite = new UISprite[]
        {
            DiscardTitle.GetChild(0).GetComponent<UISprite>(),
            DiscardTitle.GetChild(1).GetComponent<UISprite>()
        };
        discardTitleBulletWidget = new UIWidget[]
        {
            DiscardTitle.GetChild(0).GetComponent<UIWidget>(),
            DiscardTitle.GetChild(1).GetComponent<UIWidget>()
        };
        Int32 tableIndex = 0;
        foreach (Transform transform in CardSelectionListPanel.transform.GetChild(0))
        {
            Int32 invTableIndex = tableIndex % 10 * 10;
            invTableIndex += tableIndex / 10;
            tableIndex++;
            QuadMistUI.CardListHUD cardListHUD = new QuadMistUI.CardListHUD(transform.gameObject, invTableIndex);
            cardHudList.Add(cardListHUD);
            UIEventListener.Get(cardListHUD.Self).onClick += onClick;
        }
        foreach (Transform transform in CardInfoPanel.GetChild(0).GetChild(1).transform)
            cardDetailHudList.Add(new CardDetailHUD(transform.gameObject));
        foreach (Transform transform in CardSelectedPanel.transform)
            UIEventListener.Get(transform.gameObject).onClick += OnClickSelectedCard;
        cardDetailTransition = TransitionPanel.GetChild(0).GetComponent<HonoTweenPosition>();
        cardNameLabel.leftAnchor.Set(0f, 0);
        cardNameLabel.rightAnchor.Set(1f, 0);
        cardNameLabel.overflowMethod = UILabel.Overflow.ShrinkContent;
    }

    private const String CardGroupButton = "QuadMist.Card";
    private const Int32 FF9FCARD_ARROW_TYPE_MAX = 256;
    private const Int32 FF9FCAZRD_LV_MAX = 32;

    public GameObject CardSelectionListPanel;
    public GameObject CardSelectedPanel;
    public GameObject PlayerInfoPanel;
    public GameObject CardInfoPanel;
    public GameObject TransitionPanel;

    public UISprite TitleSprite;

    public GameObject DiscardTitle;
    public GameObject ScreenFadeGameObject;
    public GameObject BackButton;

    private UISprite discardTitleSprite;

    private UISprite[] discardTitleBulletSprite;
    private UIWidget[] discardTitleBulletWidget;

    private UILabel winCountLabel;
    private UILabel loseCountLabel;
    private UILabel drawCountLabel;
    private UILabel stockCountLabel;
    private UILabel typeCountLabel;

    private GameObject cardInfoContentGameObject;
    private GameObject cardNumberGameObject;

    private UILabel cardIdLabel;
    private UILabel currentCardNumberLabel;
    private UILabel totalCardNumberLabel;
    private UILabel cardNameLabel;

    private BoxCollider prevOffsetButton;
    private BoxCollider nextOffsetButton;

    private HonoTweenPosition cardDetailTransition;

    private List<QuadMistUI.CardListHUD> cardHudList = new List<QuadMistUI.CardListHUD>();
    private List<CardDetailHUD> cardDetailHudList = new List<CardDetailHUD>();

    private UIAtlas textAtlas;

    private Boolean isNeedToBuildCard = true;

    private QuadMistUI.CardState currentState;

    private Int32 currentCardId;
    private Int32 currentCardOffset;
    private Int32 deleteCardId;

    private Boolean isDialogShowing;

    private List<QuadMistCard> selectedCardList = new List<QuadMistCard>();
    public static List<QuadMistCard> allCardList = new List<QuadMistCard>();

    private Byte[] count = new Byte[CardPool.TOTAL_CARDS];

    public class CardListHUD
    {
        public CardListHUD(GameObject go, Int32 id)
        {
            Self = go;
            Id = id;
            CardIconSprite = go.GetChild(0).GetComponent<UISprite>();
            CardAmountLabel = go.GetChild(1).GetComponent<UILabel>();
        }

        public GameObject Self;
        public Int32 Id;
        public UISprite CardIconSprite;
        public UILabel CardAmountLabel;
    }

    public enum CardState
    {
        CardSelection,
        CardDestroy
    }
}
