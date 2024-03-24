using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Data;
using Memoria.Assets;
using Memoria.Prime;
using UnityEngine;
using Object = System.Object;
using static BubbleUI;

public class QuadMistUI : UIScene
{
	public QuadMistUI.CardState State
	{
		set
		{
			currentState = value;
		}
	}

	public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
	{
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
		{
			if (currentState == QuadMistUI.CardState.CardSelection)
			{
				if (PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.Tutorial)
				{
					ButtonGroupState.ActiveGroup = QuadMistUI.CardGroupButton;
				}
                if (Configuration.TetraMaster.TripleTriad == 3)
                {
					if (QuadMistGame.SkipTripleTriadRules && QuadMistGame.HasTripleTrialRule_Random)
					{
						GeneratePlayerCardRandom();
                    }
					if (!QuadMistGame.SkipTripleTriadRules)
					{
                        String phrase = "[IMME]Liste des règles :";
                        int numberline = 1;
                        if (QuadMistGame.HasTripleTrialRule_One)
                        {
                            phrase += "\n[MOVE=18,0]One";
                            numberline++;
                        }
                        if (QuadMistGame.HasTripleTrialRule_Direct)
                        {
                            phrase += "\n[MOVE=18,0]Direct";
                            numberline++;
                        }
                        if (QuadMistGame.HasTripleTrialRule_Diff)
                        {
                            phrase += "\n[MOVE=18,0]Diff";
                            numberline++;
                        }
                        if (QuadMistGame.HasTripleTrialRule_All)
                        {
                            phrase += "\n[MOVE=18,0]All";
                            numberline++;
                        }
                        if (QuadMistGame.HasTripleTrialRule_Open)
                        {
                            phrase += "\n[MOVE=18,0]Open";
                            numberline++;
                        }
                        if (QuadMistGame.HasTripleTrialRule_Random)
                        {
                            phrase += "\n[MOVE=18,0]Random";
                            numberline++;
                        }
                        if (QuadMistGame.HasTripleTrialRule_SuddenDeath)
                        {
                            phrase += "\n[MOVE=18,0]SuddenDeath";
                            numberline++;
                        }
                        if (QuadMistGame.HasTripleTrialRule_Same)
                        {
                            phrase += "\n[MOVE=18,0]Same";
                            numberline++;
                        }
                        if (QuadMistGame.HasTripleTrialRule_Plus)
                        {
                            phrase += "\n[MOVE=18,0]Plus";
                            numberline++;
                        }
                        if (QuadMistGame.HasTripleTrialRule_SameWall)
                        {
                            phrase += "\n[MOVE=18,0]SameWall";
                            numberline++;
                        }
                        if (QuadMistGame.HasTripleTrialRule_Elemental)
                        {
                            phrase += "\n[MOVE=18,0]Elemental";
                            numberline++;
                        }
                        if (numberline == 1)
                        {
                            phrase += "\n[MOVE=18,0]Aucune";
                            numberline++;
                        }
                        phrase += "\n\n[CHOO][FEED=20]Zé parti !\n[FEED=20]Quitter[ENDN]";
                        numberline += 3;
                        cardInfoContentGameObject.SetActive(false);
                        Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(phrase, 100, numberline, Dialog.TailPosition.UpperCenter, Dialog.WindowStyle.WindowStylePlain, new Vector2(105f, 40f), Dialog.CaptionType.None);
                        ButtonGroupState.SetPointerOffsetToGroup(new Vector2(0f, 0f), Dialog.DialogGroupButton);
                        dialog.AfterDialogHidden = new Dialog.DialogIntDelegate(onConfirmDialogTripleTriadHidden);
                    }
                }
            }
            else
			{
				String phrase = Localization.Get("QuadMistDiscardNotice");
				Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(phrase, 100, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(105f, 90f), Dialog.CaptionType.None);
				dialog.AfterDialogHidden = new Dialog.DialogIntDelegate(onDiscardNoticeHidden);
				isDialogShowing = true;
			}
		};
		if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		base.NextSceneIsModal = !isNeedToBuildCard;
		base.Show(sceneVoidDelegate);
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
			Boolean flag = false;
			if (FF9StateSystem.Common.FF9.miniGameArg == 124 || FF9StateSystem.Common.FF9.miniGameArg == 125 || FF9StateSystem.Common.FF9.miniGameArg == 126 || FF9StateSystem.Common.FF9.miniGameArg == 127)
			{
				flag = true;
			}
			BackButton.SetActive(!flag);
		}
        else
		{
			TitleSprite.gameObject.SetActive(false);
			DiscardTitle.SetActive(true);
			discardTitleSprite.atlas = textAtlas;
			UISprite[] array = discardTitleBulletSprite;
			for (Int32 i = 0; i < (Int32)array.Length; i++)
			{
				UISprite uisprite = array[i];
				uisprite.atlas = textAtlas;
			}
			String language = Localization.CurrentLanguage;
			switch (language)
			{
				case "English(US)":
				case "English(UK)":
					discardTitleBulletWidget[0].leftAnchor.absolute = 128;
					discardTitleBulletWidget[0].rightAnchor.absolute = 186;
					discardTitleBulletWidget[1].leftAnchor.absolute = -186;
					discardTitleBulletWidget[1].rightAnchor.absolute = -128;
					break;
				case "Japanese":
					discardTitleBulletWidget[0].leftAnchor.absolute = 0;
					discardTitleBulletWidget[0].rightAnchor.absolute = 58;
					discardTitleBulletWidget[1].leftAnchor.absolute = -58;
					discardTitleBulletWidget[1].rightAnchor.absolute = 0;
					break;
				case "Spanish":
				case "German":
				case "Italian":
					discardTitleBulletWidget[0].leftAnchor.absolute = -58;
					discardTitleBulletWidget[0].rightAnchor.absolute = 0;
					discardTitleBulletWidget[1].leftAnchor.absolute = 0;
					discardTitleBulletWidget[1].rightAnchor.absolute = 58;
					break;
				case "French":
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
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
		{
			PersistenSingleton<UIManager>.Instance.State = UIManager.UIState.QuadMistBattle;
		};
		if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		base.NextSceneIsModal = false;
		base.Hide(sceneVoidDelegate);
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
						ButtonGroupState.SetPointerOffsetToGroup(new Vector2(140f, 0f), Dialog.DialogGroupButton);
						dialog.AfterDialogHidden = new Dialog.DialogIntDelegate(onConfirmDialogHidden);
					}
				}
			}
			else if (count[currentCardId] > 0)
			{
				deleteCardId = currentCardId;
				ETb.sChoose = 1;
				String phrase2 = Localization.Get("QuadMistDiscardConfirm");
				Dialog dialog2 = Singleton<DialogManager>.Instance.AttachDialog(phrase2, 100, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(194f, 38f), Dialog.CaptionType.None);
				ButtonGroupState.SetPointerOffsetToGroup(new Vector2(100f, 0f), Dialog.DialogGroupButton);
				dialog2.AfterDialogHidden = new Dialog.DialogIntDelegate(onDiscardConfirmHidden);
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
			else
			{
				Boolean flag = false;
				if (FF9StateSystem.Common.FF9.miniGameArg == 124 || FF9StateSystem.Common.FF9.miniGameArg == 125 || FF9StateSystem.Common.FF9.miniGameArg == 126 || FF9StateSystem.Common.FF9.miniGameArg == 127)
				{
					flag = true;
				}
				if (!flag)
				{
					ETb.sChoose = 1;
					String phrase = Localization.Get("QuadMistEndCardGame");
					Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(phrase, 100, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(105f, 90f), Dialog.CaptionType.None);
					ButtonGroupState.SetPointerOffsetToGroup(new Vector2(230f, 0f), Dialog.DialogGroupButton);
					dialog.AfterDialogHidden = new Dialog.DialogIntDelegate(onQuitDialogHidden);
				}
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
			Byte b = count[currentCardId];
			if (b > 1)
			{
				FF9Sfx.FF9SFX_Play(1047);
				currentCardOffset = (currentCardOffset + (Int32)b - 1) % (Int32)b;
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
		if (base.OnKeyRightBumper(go) && ButtonGroupState.ActiveGroup == QuadMistUI.CardGroupButton)
		{
			Byte b = count[currentCardId];
			if (b > 1)
			{
				FF9Sfx.FF9SFX_Play(1047);
				currentCardOffset = (currentCardOffset + (Int32)b + 1) % (Int32)b;
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
		ButtonGroupState.SetPointerOffsetToGroup(Dialog.DefaultOffset, Dialog.DialogGroupButton);
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

    private void onConfirmDialogTripleTriadHidden(Int32 choice)
    {
        ButtonGroupState.SetPointerOffsetToGroup(Dialog.DefaultOffset, Dialog.DialogGroupButton);
        if (choice == 0)
        {
            QuadMistGame.SkipTripleTriadRules = true;
            if (QuadMistGame.HasTripleTrialRule_Random)
            {
				GeneratePlayerCardRandom();
            }
			else
			{
                currentCardOffset = 0;
                cardInfoContentGameObject.SetActive(true);
                DisplayCardList();
                DisplayCardDetail();
                ButtonGroupState.ActiveGroup = QuadMistUI.CardGroupButton;
            }
        }
        else
        {
            isNeedToBuildCard = true;
            base.Loading = true;
            QuadMistGame.main.QuitQuadMist();
        }
    }

    private void onQuitDialogHidden(Int32 choice)
	{
		ButtonGroupState.SetPointerOffsetToGroup(Dialog.DefaultOffset, Dialog.DialogGroupButton);
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
		ButtonGroupState.SetPointerOffsetToGroup(Dialog.DefaultOffset, Dialog.DialogGroupButton);
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
		Int32 id;
		for (id = 0; id < CardPool.TOTAL_CARDS; id++)
		{
			Byte kindCount = count[id];
			QuadMistUI.CardListHUD cardListHUD = cardHudList.First((QuadMistUI.CardListHUD hud) => hud.Id == id);
			if (kindCount > 0)
			{
				CardIcon.Attribute attribute = QuadMistDatabase.MiniGame_GetCardAttribute(id);
				String spriteName = $"card_type{(Int32)attribute}_{(kindCount <= 1 ? "normal" : "select")}";
				cardListHUD.CardIconSprite.spriteName = spriteName;
				if (kindCount > 1)
				{
					cardListHUD.CardAmountLabel.gameObject.SetActive(true);
					cardListHUD.CardAmountLabel.text = kindCount.ToString();
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
		Int32 num = count[currentCardId];
		if (num > 0)
		{
			cardInfoContentGameObject.SetActive(true);
			ShowCardDetailHudAmount(num);
			cardIdLabel.text = "No" + (currentCardId + 1).ToString("0#");
			FF9UIDataTool.DisplayCard(GetCardInfo(currentCardId, currentCardOffset), cardDetailHudList[0], false);
			cardNameLabel.text = FF9TextTool.CardName((TetraMasterCardId)currentCardId);
			if (num > 1)
			{
				cardNumberGameObject.SetActive(true);
				currentCardNumberLabel.text = (currentCardOffset + 1).ToString();
				totalCardNumberLabel.text = num.ToString();
				for (Int32 i = 1; i < Math.Min(num, 5); i++)
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
			Int32 num = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetWinCount());
			Int32 num2 = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetLoseCount());
			Int32 num3 = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetDrawCount());
			winCountLabel.text = num.ToString();
			loseCountLabel.text = num2.ToString();
			drawCountLabel.text = num3.ToString();
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
		for (Int32 j = 0; j < CardPool.TOTAL_CARDS; j++)
			count[j] = (Byte)QuadMistDatabase.MiniGame_GetCardCount((TetraMasterCardId)j);
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

    private void GeneratePlayerCardRandom() // For the rule "Random" from Triple Triad
    {
        int remainingcard = 5 - selectedCardList.Count < 0 ? 0 : 5 - selectedCardList.Count;
        for (Int32 i = 0; i < remainingcard; ++i)
        {
            int randomindex = GameRandom.Next16() % (allCardList.Count);
            QuadMistCard cardInfo = allCardList[randomindex];
            selectedCardList.Add(cardInfo);
            QuadMistUI.allCardList.Remove(cardInfo);
            count[(Int32)allCardList[randomindex].id]--;
            QuadMistGame.UpdateSelectedCardList(selectedCardList);
            currentCardOffset = 0;
        }
        cardInfoContentGameObject.SetActive(false);
        Hide(delegate
        {
            QuadMistGame.OnFinishSelectCardUI(selectedCardList);
            isNeedToBuildCard = true;
        });
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
		stockCountLabel.text = QuadMistDatabase.MiniGame_GetAllCardCount().ToString();
		typeCountLabel.text = QuadMistDatabase.MiniGame_GetCardKindCount().ToString();
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
		foreach (Object obj in CardSelectionListPanel.transform.GetChild(0))
		{
			Transform transform = (Transform)obj;
			Int32 invTableIndex = tableIndex % 10 * 10;
			invTableIndex += tableIndex / 10;
			tableIndex++;
			QuadMistUI.CardListHUD cardListHUD = new QuadMistUI.CardListHUD(transform.gameObject, invTableIndex);
			cardHudList.Add(cardListHUD);
			UIEventListener uieventListener = UIEventListener.Get(cardListHUD.Self);
			uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(onClick));
		}
		foreach (Object obj2 in CardInfoPanel.GetChild(0).GetChild(1).transform)
		{
			Transform transform2 = (Transform)obj2;
			CardDetailHUD item = new CardDetailHUD(transform2.gameObject);
			cardDetailHudList.Add(item);
		}
		foreach (Object obj3 in CardSelectedPanel.transform)
		{
			Transform transform3 = (Transform)obj3;
			UIEventListener uieventListener2 = UIEventListener.Get(transform3.gameObject);
			uieventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener2.onClick, new UIEventListener.VoidDelegate(OnClickSelectedCard));
		}
		cardDetailTransition = TransitionPanel.GetChild(0).GetComponent<HonoTweenPosition>();
	}

	private static String CardGroupButton = "QuadMist.Card";
	private static Int32 FF9FCARD_ARROW_TYPE_MAX = 256;
	private static Int32 FF9FCAZRD_LV_MAX = 32;

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
