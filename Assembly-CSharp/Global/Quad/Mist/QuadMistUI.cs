using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;
using UnityEngine;
using Object = System.Object;

public class QuadMistUI : UIScene
{
	public QuadMistUI.CardState State
	{
		set
		{
			this.currentState = value;
		}
	}

	public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
	{
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
		{
			if (this.currentState == QuadMistUI.CardState.CardSelection)
			{
				if (PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.Tutorial)
				{
					ButtonGroupState.ActiveGroup = QuadMistUI.CardGroupButton;
				}
			}
			else
			{
				String phrase = Localization.Get("QuadMistDiscardNotice");
				Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(phrase, 100, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(105f, 90f), Dialog.CaptionType.None);
				dialog.AfterDialogHidden = new Dialog.DialogIntDelegate(this.onDiscardNoticeHidden);
				this.isDialogShowing = true;
			}
		};
		if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		base.NextSceneIsModal = !this.isNeedToBuildCard;
		base.Show(sceneVoidDelegate);
		if (this.isNeedToBuildCard)
		{
			this.currentCardId = 0;
			this.currentCardOffset = 0;
			this.isNeedToBuildCard = false;
			this.BuildCard();
			this.UpdateAmountLabel();
		}
		this.textAtlas = (Resources.Load("EmbeddedAsset/QuadMist/Atlas/QuadMist Text " + Localization.GetSymbol() + " Atlas", typeof(UIAtlas)) as UIAtlas);
		PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(false);
		this.DisplayInfo();
		this.DisplayCardList();
		if (this.currentState == QuadMistUI.CardState.CardSelection)
		{
			this.DisplayCardDetail();
			this.DiscardTitle.SetActive(false);
			this.TitleSprite.atlas = this.textAtlas;
			this.TitleSprite.gameObject.SetActive(true);
			this.CardSelectedPanel.SetActive(true);
			Boolean flag = false;
			if (FF9StateSystem.Common.FF9.miniGameArg == 124 || FF9StateSystem.Common.FF9.miniGameArg == 125 || FF9StateSystem.Common.FF9.miniGameArg == 126 || FF9StateSystem.Common.FF9.miniGameArg == 127)
			{
				flag = true;
			}
			this.BackButton.SetActive(!flag);
		}
		else
		{
			this.TitleSprite.gameObject.SetActive(false);
			this.DiscardTitle.SetActive(true);
			this.discardTitleSprite.atlas = this.textAtlas;
			UISprite[] array = this.discardTitleBulletSprite;
			for (Int32 i = 0; i < (Int32)array.Length; i++)
			{
				UISprite uisprite = array[i];
				uisprite.atlas = this.textAtlas;
			}
			String language = Localization.CurrentLanguage;
			switch (language)
			{
			case "English(US)":
			case "English(UK)":
				this.discardTitleBulletWidget[0].leftAnchor.absolute = 128;
				this.discardTitleBulletWidget[0].rightAnchor.absolute = 186;
				this.discardTitleBulletWidget[1].leftAnchor.absolute = -186;
				this.discardTitleBulletWidget[1].rightAnchor.absolute = -128;
				break;
			case "Japanese":
				this.discardTitleBulletWidget[0].leftAnchor.absolute = 0;
				this.discardTitleBulletWidget[0].rightAnchor.absolute = 58;
				this.discardTitleBulletWidget[1].leftAnchor.absolute = -58;
				this.discardTitleBulletWidget[1].rightAnchor.absolute = 0;
				break;
			case "Spanish":
			case "German":
			case "Italian":
				this.discardTitleBulletWidget[0].leftAnchor.absolute = -58;
				this.discardTitleBulletWidget[0].rightAnchor.absolute = 0;
				this.discardTitleBulletWidget[1].leftAnchor.absolute = 0;
				this.discardTitleBulletWidget[1].rightAnchor.absolute = 58;
				break;
			case "French":
				this.discardTitleBulletWidget[0].leftAnchor.absolute = -28;
				this.discardTitleBulletWidget[0].rightAnchor.absolute = 30;
				this.discardTitleBulletWidget[1].leftAnchor.absolute = -30;
				this.discardTitleBulletWidget[1].rightAnchor.absolute = 28;
				break;
			}
			this.discardTitleBulletWidget[0].UpdateAnchors();
			this.discardTitleBulletWidget[1].UpdateAnchors();
			this.CardSelectedPanel.SetActive(false);
			this.BackButton.SetActive(false);
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
		this.RemoveCursorMemorize();
		this.cardInfoContentGameObject.SetActive(false);
	}

	private void RemoveCursorMemorize()
	{
		ButtonGroupState.RemoveCursorMemorize(QuadMistUI.CardGroupButton);
	}

	public override Boolean OnKeyConfirm(GameObject go)
	{
		if (base.OnKeyConfirm(go) && ButtonGroupState.ActiveGroup == QuadMistUI.CardGroupButton)
		{
			if (this.currentState == QuadMistUI.CardState.CardSelection)
			{
				if (this.count[this.currentCardId] > 0)
				{
					FF9Sfx.FF9SFX_Play(1047);
					this.SelectCard(this.currentCardId, this.currentCardOffset);
					this.currentCardOffset = 0;
					this.DisplayCardList();
					this.DisplayCardDetail();
					if (this.selectedCardList.Count > 4)
					{
						this.cardInfoContentGameObject.SetActive(false);
						String phrase = Localization.Get("QuadMistConfirmSelection");
						Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(phrase, 100, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(105f, 90f), Dialog.CaptionType.None);
						ButtonGroupState.SetPointerOffsetToGroup(new Vector2(140f, 0f), Dialog.DialogGroupButton);
						dialog.AfterDialogHidden = new Dialog.DialogIntDelegate(this.onConfirmDialogHidden);
					}
				}
			}
			else if (this.count[this.currentCardId] > 0)
			{
				this.deleteCardId = this.currentCardId;
				ETb.sChoose = 1;
				String phrase2 = Localization.Get("QuadMistDiscardConfirm");
				Dialog dialog2 = Singleton<DialogManager>.Instance.AttachDialog(phrase2, 100, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(194f, 38f), Dialog.CaptionType.None);
				ButtonGroupState.SetPointerOffsetToGroup(new Vector2(100f, 0f), Dialog.DialogGroupButton);
				dialog2.AfterDialogHidden = new Dialog.DialogIntDelegate(this.onDiscardConfirmHidden);
			}
		}
		return true;
	}

	public override Boolean OnKeyCancel(GameObject go)
	{
		if (base.OnKeyCancel(go) && ButtonGroupState.ActiveGroup == QuadMistUI.CardGroupButton && this.currentState == QuadMistUI.CardState.CardSelection)
		{
			FF9Sfx.FF9SFX_Play(101);
			if (this.selectedCardList.Count > 0)
			{
				this.DeselectCard(this.selectedCardList.Count - 1);
				this.DisplayCardList();
				this.DisplayCardDetail();
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
					dialog.AfterDialogHidden = new Dialog.DialogIntDelegate(this.onQuitDialogHidden);
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
			if (siblingIndex < this.selectedCardList.Count)
			{
				FF9Sfx.FF9SFX_Play(101);
				QuadMistCard quadMistCard = this.selectedCardList[siblingIndex];
				this.selectedCardList.RemoveAt(siblingIndex);
				QuadMistUI.allCardList.Add(quadMistCard);
				Byte[] array = this.count;
				Byte id = quadMistCard.id;
				array[(Int32)id] = (Byte)(array[(Int32)id] + 1);
				QuadMistGame.UpdateSelectedCardList(this.selectedCardList);
				this.DisplayCardList();
			}
		}
	}

	private void OnClickSelectedCard(GameObject go)
	{
		Int32 siblingIndex = go.transform.GetSiblingIndex();
		if (siblingIndex < this.selectedCardList.Count)
		{
			FF9Sfx.FF9SFX_Play(101);
			QuadMistCard quadMistCard = this.selectedCardList[siblingIndex];
			this.selectedCardList.RemoveAt(siblingIndex);
			QuadMistUI.allCardList.Add(quadMistCard);
			Byte[] array = this.count;
			Byte id = quadMistCard.id;
			array[(Int32)id] = (Byte)(array[(Int32)id] + 1);
			QuadMistGame.UpdateSelectedCardList(this.selectedCardList);
			this.DisplayCardList();
			this.DisplayCardDetail();
		}
	}

	public override Boolean OnKeyLeftBumper(GameObject go)
	{
		if (base.OnKeyLeftBumper(go) && ButtonGroupState.ActiveGroup == QuadMistUI.CardGroupButton)
		{
			Byte b = this.count[this.currentCardId];
			if (b > 1)
			{
				FF9Sfx.FF9SFX_Play(1047);
				this.currentCardOffset = (this.currentCardOffset + (Int32)b - 1) % (Int32)b;
				Byte[] dialogIndexes = (from i in Enumerable.Range(0, Mathf.Min((Int32)(b - 1), 4))
				select (Byte)i).ToArray<Byte>();
				base.ShowPointerWhenLoading = true;
				base.Loading = true;
				this.cardDetailTransition.TweenPingPong(dialogIndexes, (UIScene.SceneVoidDelegate)null, delegate
				{
					base.Loading = false;
					base.ShowPointerWhenLoading = false;
					this.DisplayCardDetail();
				});
			}
		}
		return true;
	}

	public override Boolean OnKeyRightBumper(GameObject go)
	{
		if (base.OnKeyRightBumper(go) && ButtonGroupState.ActiveGroup == QuadMistUI.CardGroupButton)
		{
			Byte b = this.count[this.currentCardId];
			if (b > 1)
			{
				FF9Sfx.FF9SFX_Play(1047);
				this.currentCardOffset = (this.currentCardOffset + (Int32)b + 1) % (Int32)b;
				Byte[] dialogIndexes = (from i in Enumerable.Range(0, Mathf.Min((Int32)(b - 1), 4))
				select (Byte)i).ToArray<Byte>();
				base.ShowPointerWhenLoading = true;
				base.Loading = true;
				this.cardDetailTransition.TweenPingPong(dialogIndexes, (UIScene.SceneVoidDelegate)null, delegate
				{
					base.Loading = false;
					base.ShowPointerWhenLoading = false;
					this.DisplayCardDetail();
				});
			}
		}
		return true;
	}

	public override Boolean OnItemSelect(GameObject go)
	{
		if (base.OnItemSelect(go) && ButtonGroupState.ActiveGroup == QuadMistUI.CardGroupButton)
		{
			Int32 id = this.cardHudList[go.transform.GetSiblingIndex()].Id;
			if (this.currentCardId != id)
			{
				this.currentCardId = id;
				this.currentCardOffset = 0;
				this.DisplayCardDetail();
			}
		}
		return true;
	}

	private void onConfirmDialogHidden(Int32 choice)
	{
		ButtonGroupState.SetPointerOffsetToGroup(Dialog.DefaultOffset, Dialog.DialogGroupButton);
		if (choice == 0)
		{
			this.Hide(delegate
			{
				QuadMistGame.OnFinishSelectCardUI(this.selectedCardList);
				this.isNeedToBuildCard = true;
			});
		}
		else
		{
			this.DeselectCard(4);
			this.currentCardOffset = 0;
			this.cardInfoContentGameObject.SetActive(true);
			this.DisplayCardList();
			this.DisplayCardDetail();
			ButtonGroupState.ActiveGroup = QuadMistUI.CardGroupButton;
		}
	}

	private void onQuitDialogHidden(Int32 choice)
	{
		ButtonGroupState.SetPointerOffsetToGroup(Dialog.DefaultOffset, Dialog.DialogGroupButton);
		if (choice == 0)
		{
			this.isNeedToBuildCard = true;
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
		this.isDialogShowing = false;
		this.DisplayCardDetail();
		ButtonGroupState.ActiveGroup = QuadMistUI.CardGroupButton;
	}

	private void onDiscardConfirmHidden(Int32 choice)
	{
		ButtonGroupState.SetPointerOffsetToGroup(Dialog.DefaultOffset, Dialog.DialogGroupButton);
		if (choice == 0)
		{
			QuadMistDatabase.MiniGame_AwayCard(this.deleteCardId, this.currentCardOffset);
			if (QuadMistDatabase.MiniGame_GetAllCardCount() > 100)
			{
				QuadMistUI.allCardList.Remove(this.GetCardInfo(this.currentCardId, this.currentCardOffset));
				Byte[] array = this.count;
				Int32 num = this.deleteCardId;
				array[num] = (Byte)(array[num] - 1);
				this.currentCardOffset = 0;
				this.DisplayCardList();
				this.DisplayCardDetail();
				ButtonGroupState.ActiveGroup = QuadMistUI.CardGroupButton;
				this.UpdateAmountLabel();
			}
			else
			{
				this.Hide(delegate
				{
					QuadMistGame.OnDiscardFinish();
					this.isNeedToBuildCard = true;
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
		for (id = 0; id < 100; id++)
		{
			Byte b = this.count[id];
			QuadMistUI.CardListHUD cardListHUD = this.cardHudList.First((QuadMistUI.CardListHUD hud) => hud.Id == id);
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
	}

	private void DisplayCardDetail()
	{
		Int32 num = (Int32)this.count[this.currentCardId];
		if (num > 0)
		{
			this.cardInfoContentGameObject.SetActive(true);
			this.ShowCardDetailHudAmount(num);
			this.cardIdLabel.text = "No" + (this.currentCardId + 1).ToString("0#");
			FF9UIDataTool.DisplayCard(this.GetCardInfo(this.currentCardId, this.currentCardOffset), this.cardDetailHudList[0], false);
			this.cardNameLabel.text = FF9TextTool.CardName(this.currentCardId);
			if (num > 1)
			{
				this.cardNumberGameObject.SetActive(true);
				this.currentCardNumberLabel.text = (this.currentCardOffset + 1).ToString();
				this.totalCardNumberLabel.text = num.ToString();
				for (Int32 i = 1; i < Math.Min(num, 5); i++)
				{
					FF9UIDataTool.DisplayCard(this.GetCardInfo(this.currentCardId, 0), this.cardDetailHudList[i], true);
				}
			}
			else
			{
				this.cardNumberGameObject.SetActive(false);
			}
		}
		else
		{
			this.cardInfoContentGameObject.SetActive(false);
		}
	}

	private void DisplayInfo()
	{
		if (this.currentState == QuadMistUI.CardState.CardSelection)
		{
			this.PlayerInfoPanel.SetActive(true);
			Int32 num = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetWinCount());
			Int32 num2 = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetLoseCount());
			Int32 num3 = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetDrawCount());
			this.winCountLabel.text = num.ToString();
			this.loseCountLabel.text = num2.ToString();
			this.drawCountLabel.text = num3.ToString();
		}
		else
		{
			this.PlayerInfoPanel.SetActive(false);
		}
	}

	private void BuildCard()
	{
		List<QuadMistCard> list = new List<QuadMistCard>();
		QuadMistGame.main.preBoard.collection.CreateCards();
		QuadMistGame.main.preBoard.UpdateCollection(-1);
		List<QuadMistCard>[] cards = QuadMistGame.main.preBoard.collection.cards;
		for (Int32 i = 0; i < (Int32)cards.Length; i++)
		{
			List<QuadMistCard> list2 = cards[i];
			foreach (QuadMistCard quadMistCard in list2)
			{
				if (quadMistCard != null)
				{
					list.Add(quadMistCard);
				}
			}
		}
		QuadMistUI.allCardList = list;
		this.selectedCardList.Clear();
		for (Int32 j = 0; j < 100; j++)
		{
			this.count[j] = (Byte)QuadMistDatabase.MiniGame_GetCardCount(j);
		}
	}

	private QuadMistCard GetCardInfo(Int32 id, Int32 offset)
	{
		Int32 num = 0;
		foreach (QuadMistCard quadMistCard in QuadMistUI.allCardList)
		{
			if ((Int32)quadMistCard.id == id)
			{
				if (offset == num)
				{
					return quadMistCard;
				}
				num++;
			}
		}
		return (QuadMistCard)null;
	}

	private void ShowCardDetailHudAmount(Int32 number)
	{
		Int32 num = 0;
		foreach (CardDetailHUD cardDetailHUD in this.cardDetailHudList)
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

	private void SelectCard(Int32 id, Int32 offset)
	{
		QuadMistCard cardInfo = this.GetCardInfo(id, offset);
		this.selectedCardList.Add(cardInfo);
		QuadMistUI.allCardList.Remove(cardInfo);
		Byte[] array = this.count;
		array[id] = (Byte)(array[id] - 1);
		QuadMistGame.UpdateSelectedCardList(this.selectedCardList);
		this.DisplayCardList();
	}

	private void DeselectCard(Int32 index)
	{
		QuadMistCard quadMistCard = this.selectedCardList[index];
		this.selectedCardList.Remove(quadMistCard);
		QuadMistUI.allCardList.Add(quadMistCard);
		Byte[] array = this.count;
		Byte id = quadMistCard.id;
		array[(Int32)id] = (Byte)(array[(Int32)id] + 1);
		QuadMistGame.UpdateSelectedCardList(this.selectedCardList);
		this.DisplayCardList();
	}

	private void UpdateAmountLabel()
	{
		this.stockCountLabel.text = QuadMistDatabase.MiniGame_GetAllCardCount().ToString();
		this.typeCountLabel.text = QuadMistDatabase.MiniGame_GetCardKindCount().ToString();
	}

	private void Awake()
	{
		base.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
		this.winCountLabel = this.PlayerInfoPanel.GetChild(0).GetChild(1).GetComponent<UILabel>();
		this.loseCountLabel = this.PlayerInfoPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
		this.drawCountLabel = this.PlayerInfoPanel.GetChild(2).GetChild(1).GetComponent<UILabel>();
		this.stockCountLabel = this.CardSelectionListPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
		this.typeCountLabel = this.CardSelectionListPanel.GetChild(1).GetChild(3).GetComponent<UILabel>();
		this.cardInfoContentGameObject = this.CardInfoPanel.GetChild(0);
		this.cardNumberGameObject = this.CardInfoPanel.GetChild(0).GetChild(2);
		this.cardIdLabel = this.CardInfoPanel.GetChild(0).GetChild(0).GetComponent<UILabel>();
		this.currentCardNumberLabel = this.CardInfoPanel.GetChild(0).GetChild(2).GetChild(1).GetComponent<UILabel>();
		this.totalCardNumberLabel = this.CardInfoPanel.GetChild(0).GetChild(2).GetChild(3).GetComponent<UILabel>();
		this.cardNameLabel = this.CardInfoPanel.GetChild(0).GetChild(3).GetComponent<UILabel>();
		this.prevOffsetButton = this.CardInfoPanel.GetChild(0).GetChild(1).GetChild(0).GetComponent<BoxCollider>();
		this.nextOffsetButton = this.CardInfoPanel.GetChild(0).GetChild(1).GetChild(4).GetComponent<BoxCollider>();
		this.discardTitleSprite = this.DiscardTitle.GetComponent<UISprite>();
		this.discardTitleBulletSprite = new UISprite[]
		{
			this.DiscardTitle.GetChild(0).GetComponent<UISprite>(),
			this.DiscardTitle.GetChild(1).GetComponent<UISprite>()
		};
		this.discardTitleBulletWidget = new UIWidget[]
		{
			this.DiscardTitle.GetChild(0).GetComponent<UIWidget>(),
			this.DiscardTitle.GetChild(1).GetComponent<UIWidget>()
		};
		Int32 num = 0;
		foreach (Object obj in this.CardSelectionListPanel.transform.GetChild(0))
		{
			Transform transform = (Transform)obj;
			Int32 num2 = num % 10 * 10;
			num2 += num / 10;
			num++;
			QuadMistUI.CardListHUD cardListHUD = new QuadMistUI.CardListHUD(transform.gameObject, num2);
			this.cardHudList.Add(cardListHUD);
			UIEventListener uieventListener = UIEventListener.Get(cardListHUD.Self);
			uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.onClick));
		}
		foreach (Object obj2 in this.CardInfoPanel.GetChild(0).GetChild(1).transform)
		{
			Transform transform2 = (Transform)obj2;
			CardDetailHUD item = new CardDetailHUD(transform2.gameObject);
			this.cardDetailHudList.Add(item);
		}
		foreach (Object obj3 in this.CardSelectedPanel.transform)
		{
			Transform transform3 = (Transform)obj3;
			UIEventListener uieventListener2 = UIEventListener.Get(transform3.gameObject);
			uieventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener2.onClick, new UIEventListener.VoidDelegate(this.OnClickSelectedCard));
		}
		this.cardDetailTransition = this.TransitionPanel.GetChild(0).GetComponent<HonoTweenPosition>();
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

	private Byte[] count = new Byte[100];

	public class CardListHUD
	{
		public CardListHUD(GameObject go, Int32 id)
		{
			this.Self = go;
			this.Id = id;
			this.CardIconSprite = go.GetChild(0).GetComponent<UISprite>();
			this.CardAmountLabel = go.GetChild(1).GetComponent<UILabel>();
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
