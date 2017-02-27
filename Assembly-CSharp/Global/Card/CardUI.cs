using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
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
			ButtonGroupState.SetPointerOffsetToGroup(new Vector2(134f, 10f), CardUI.ChoiceGroupButton);
			ButtonGroupState.ActiveGroup = CardUI.CardGroupButton;
		};
		if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
		base.Show(sceneVoidDelegate);
		this.FF9FCard_Build();
		this.DisplayInfo();
		this.DisplayHelp();
		this.DisplayCardList();
		this.DisplayCardDetail();
		this.DeleteSubmenuButton.SetActive(FF9StateSystem.MobilePlatform);
	}

	public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
	{
		base.Hide(afterFinished);
		if (!this.fastSwitch)
		{
			PersistenSingleton<UIManager>.Instance.MainMenuScene.StartSubmenuTweenIn();
			this.RemoveCursorMemorize();
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
				if (go != this.DeleteSubmenuButton)
				{
					this.currentCardId = this.cardHudList[go.transform.GetSiblingIndex()].Id;
				}
				if (this.count[this.currentCardId] > 0)
				{
					FF9Sfx.FF9SFX_Play(103);
					base.Loading = true;
					this.deleteDialogTransition.TweenIn(delegate
					{
						base.Loading = false;
						ButtonGroupState.RemoveCursorMemorize(CardUI.ChoiceGroupButton);
						ButtonGroupState.ActiveGroup = CardUI.ChoiceGroupButton;
						ButtonGroupState.HoldActiveStateOnGroup(CardUI.CardGroupButton);
					});
					this.prevOffsetButton.enabled = false;
					this.nextOffsetButton.enabled = false;
					this.deleteCardId = this.currentCardId;
				}
				else
				{
					FF9Sfx.FF9SFX_Play(102);
				}
			}
			else if (ButtonGroupState.ActiveGroup == CardUI.ChoiceGroupButton)
			{
				if (go == this.DeleteSubmenuButton)
				{
					return true;
				}
				if (this.count[this.deleteCardId] > 0)
				{
					if (go.transform.GetSiblingIndex() == 1)
					{
						FF9Sfx.FF9SFX_Play(103);
						QuadMistDatabase.MiniGame_AwayCard(this.deleteCardId, this.offset[this.deleteCardId]);
						Byte[] array = this.count;
						Int32 num = this.deleteCardId;
						array[num] = (Byte)(array[num] - 1);
						this.offset[this.deleteCardId] = Math.Min(this.offset[this.deleteCardId], (Int32)(this.count[this.deleteCardId] - 1));
						this.DisplayHelp();
						this.DisplayCardList();
						this.DisplayCardDetail();
					}
					else
					{
						FF9Sfx.FF9SFX_Play(101);
					}
				}
				else
				{
					FF9Sfx.FF9SFX_Play(101);
				}
				base.Loading = true;
				this.deleteDialogTransition.TweenOut(delegate
				{
					base.Loading = false;
					ButtonGroupState.ActiveGroup = CardUI.CardGroupButton;
				});
				this.prevOffsetButton.enabled = true;
				this.nextOffsetButton.enabled = true;
				ButtonGroupState.DisableAllGroup(true);
			}
		}
		return true;
	}

	public override Boolean OnKeyCancel(GameObject go)
	{
		if (base.OnKeyCancel(go))
		{
			if (ButtonGroupState.ActiveGroup == CardUI.CardGroupButton)
			{
				FF9Sfx.FF9SFX_Play(101);
				this.fastSwitch = false;
				this.Hide(delegate
				{
					PersistenSingleton<UIManager>.Instance.MainMenuScene.NeedTweenAndHideSubMenu = false;
					PersistenSingleton<UIManager>.Instance.MainMenuScene.CurrentSubMenu = MainMenuUI.SubMenu.Card;
					PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.MainMenu);
				});
			}
			else if (ButtonGroupState.ActiveGroup == CardUI.ChoiceGroupButton)
			{
				if (go == this.DeleteSubmenuButton)
				{
					return true;
				}
				FF9Sfx.FF9SFX_Play(101);
				base.Loading = true;
				this.deleteDialogTransition.TweenOut(delegate
				{
					base.Loading = false;
					ButtonGroupState.ActiveGroup = CardUI.CardGroupButton;
				});
				this.prevOffsetButton.enabled = true;
				this.nextOffsetButton.enabled = true;
			}
		}
		return true;
	}

	public override Boolean OnKeyLeftBumper(GameObject go)
	{
		if (base.OnKeyLeftBumper(go) && ButtonGroupState.ActiveGroup == CardUI.CardGroupButton)
		{
			Byte b = this.count[this.currentCardId];
			if (b > 1)
			{
				FF9Sfx.FF9SFX_Play(1047);
				this.offset[this.currentCardId] = (this.offset[this.currentCardId] + (Int32)b - 1) % (Int32)b;
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
		if (base.OnKeyRightBumper(go) && ButtonGroupState.ActiveGroup == CardUI.CardGroupButton)
		{
			Byte b = this.count[this.currentCardId];
			if (b > 1)
			{
				FF9Sfx.FF9SFX_Play(1047);
				this.offset[this.currentCardId] = (this.offset[this.currentCardId] + (Int32)b + 1) % (Int32)b;
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

	public override Boolean OnKeySelect(GameObject go)
	{
		return FF9StateSystem.PCPlatform && base.OnKeySelect(go);
	}

	public override Boolean OnItemSelect(GameObject go)
	{
		if (base.OnItemSelect(go) && ButtonGroupState.ActiveGroup == CardUI.CardGroupButton)
		{
			CardUI.CardListHUD cardListHUD = this.cardHudList[go.transform.GetSiblingIndex()];
			CardUI.CardListHUD cardListHUD2 = (this.currentCardId == -1) ? null : this.cardHudList[this.currentCardId];
			Int32 id = cardListHUD.Id;
			if (this.currentCardId != id)
			{
				this.currentCardId = id;
				this.DisplayCardDetail();
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
		this.HelpDespLabelGameObject.SetActive(FF9StateSystem.PCPlatform);
		if (FF9StateSystem.PCPlatform)
		{
			this.discardConfirmButton.Help.Enable = true;
			this.discardConfirmButton.Help.TextKey = "ConfirmDiscardHelp";
			this.discardConfirmButton.Help.Tail = true;
			this.discardCancelButton.Help.Enable = true;
			this.discardCancelButton.Help.TextKey = "CancelDiscardHelp";
			this.discardCancelButton.Help.Tail = true;
			foreach (CardUI.CardListHUD cardListHUD in this.cardHudList)
			{
				if (this.count[cardListHUD.Id] > 1)
				{
					cardListHUD.CardButtonGroup.Help.Enable = true;
					cardListHUD.CardButtonGroup.Help.TextKey = "SelectCard2MoreHelp";
					cardListHUD.CardButtonGroup.Help.Tail = true;
				}
				else if (this.count[cardListHUD.Id] > 0)
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
			this.discardConfirmButton.Help.Enable = false;
			this.discardConfirmButton.Help.TextKey = String.Empty;
			this.discardConfirmButton.Help.Tail = false;
			this.discardCancelButton.Help.Enable = false;
			this.discardCancelButton.Help.TextKey = String.Empty;
			this.discardCancelButton.Help.Tail = false;
			foreach (CardUI.CardListHUD cardListHUD2 in this.cardHudList)
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
		for (id = 0; id < 100; id++)
		{
			Byte b = this.count[id];
			CardUI.CardListHUD cardListHUD = this.cardHudList.First((CardUI.CardListHUD hud) => hud.Id == id);
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
		this.stockCountLabel.text = QuadMistDatabase.MiniGame_GetAllCardCount().ToString();
		this.typeCountLabel.text = QuadMistDatabase.MiniGame_GetCardKindCount().ToString();
	}

	private void DisplayCardDetail()
	{
		Int32 num = (Int32)this.count[this.currentCardId];
		if (num > 0)
		{
			this.cardInfoContentGameObject.SetActive(true);
			this.ShowCardDetailHudNumber(num);
			FF9UIDataTool.DisplayCard(QuadMistDatabase.MiniGame_GetCardInfoPtr(this.currentCardId, this.offset[this.currentCardId]), this.cardDetailHudList[0], false);
			this.cardNameLabel.text = FF9TextTool.CardName(this.currentCardId);
			if (num > 1)
			{
				this.cardNumberGameObject.SetActive(true);
				this.currentCardNumberLabel.text = (this.offset[this.currentCardId] + 1).ToString();
				this.totalCardNumberLabel.text = num.ToString();
				for (Int32 i = 1; i < Math.Min(num, 5); i++)
				{
					FF9UIDataTool.DisplayCard(QuadMistDatabase.MiniGame_GetCardInfoPtr(this.currentCardId, 0), this.cardDetailHudList[i], true);
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
		Int32 num = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetWinCount());
		Int32 num2 = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetLoseCount());
		Int32 num3 = Mathf.Min(99999, QuadMistDatabase.MiniGame_GetDrawCount());
		this.levelLabel.text = this.point.ToString() + "p";
		this.classNameLabel.text = FF9TextTool.CardLevelName(this.lv_collector);
		this.winCountLabel.text = num.ToString();
		this.loseCountLabel.text = num2.ToString();
		this.drawCountLabel.text = num3.ToString();
	}

	private void FF9FCard_Build()
	{
		Int16[] array = new Int16[]
		{
			0,
			300,
			400,
			500,
			600,
			700,
			800,
			900,
			1000,
			1100,
			1200,
			1250,
			1300,
			1320,
			1330,
			1340,
			1350,
			1360,
			1370,
			1380,
			1390,
			1400,
			1420,
			1470,
			1510,
			1550,
			1600,
			1650,
			1680,
			1690,
			1698,
			1700
		};
		for (Int32 i = 0; i < 100; i++)
		{
			this.count[i] = (Byte)QuadMistDatabase.MiniGame_GetCardCount(i);
			this.offset[i] = 0;
		}
		this.FF9FCard_GetPoint();
        this.lv_collector = CardUI.FF9FCAZRD_LV_MAX - 1;
        for (Int32 j = 1; j < CardUI.FF9FCAZRD_LV_MAX; j++)
		{
			if (this.point < (Int32)array[j])
			{
				this.lv_collector = j - 1;
				break;
			}
		}
	}

	private void FF9FCard_GetPoint()
	{
		List<QuadMistCard> list = QuadMistDatabase.MiniGame_GetCardBinPtr();
		Boolean[] array = new Boolean[CardUI.FF9FCARD_ARROW_TYPE_MAX];
		Int32 num = 0;
		Int32 num2 = 0;
		Int32 num3 = 0;
		Byte[] array2 = new Byte[]
		{
			0,
			0,
			1,
			2
		};
		foreach (QuadMistCard quadMistCard in list)
		{
			num += (Int32)quadMistCard.cpoint;
		}
		for (Int32 i = 0; i < 100; i++)
		{
			for (Int32 j = 0; j < (Int32)this.count[i]; j++)
			{
				QuadMistCard quadMistCard2 = QuadMistDatabase.MiniGame_GetCardInfoPtr(i, j);
				array[(Int32)quadMistCard2.arrow] = true;
				num2 += (Int32)array2[(Int32)quadMistCard2.type];
			}
		}
		for (Int32 k = 0; k < CardUI.FF9FCARD_ARROW_TYPE_MAX; k++)
		{
			if (array[k])
			{
				num3 += 5;
			}
		}
		this.point = num + num2 + num3;
	}

	private void ShowCardDetailHudNumber(Int32 number)
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

	private void Awake()
	{
		base.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
		this.levelLabel = this.PlayerInfoPanel.GetChild(0).GetChild(0).GetChild(3).GetComponent<UILabel>();
		this.classNameLabel = this.PlayerInfoPanel.GetChild(0).GetChild(1).GetComponent<UILabel>();
		this.winCountLabel = this.PlayerInfoPanel.GetChild(1).GetChild(0).GetChild(2).GetComponent<UILabel>();
		this.loseCountLabel = this.PlayerInfoPanel.GetChild(1).GetChild(1).GetChild(2).GetComponent<UILabel>();
		this.drawCountLabel = this.PlayerInfoPanel.GetChild(1).GetChild(2).GetChild(2).GetComponent<UILabel>();
		this.stockCountLabel = this.CardSelectionListPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
		this.typeCountLabel = this.CardSelectionListPanel.GetChild(1).GetChild(3).GetComponent<UILabel>();
		this.cardInfoContentGameObject = this.CardInfoPanel.GetChild(0);
		this.cardNumberGameObject = this.CardInfoPanel.GetChild(0).GetChild(1);
		this.currentCardNumberLabel = this.CardInfoPanel.GetChild(0).GetChild(1).GetChild(1).GetComponent<UILabel>();
		this.totalCardNumberLabel = this.CardInfoPanel.GetChild(0).GetChild(1).GetChild(3).GetComponent<UILabel>();
		this.cardNameLabel = this.CardInfoPanel.GetChild(0).GetChild(2).GetChild(1).GetComponent<UILabel>();
		this.prevOffsetButton = this.CardInfoPanel.GetChild(0).GetChild(1).GetChild(0).GetComponent<BoxCollider>();
		this.nextOffsetButton = this.CardInfoPanel.GetChild(0).GetChild(1).GetChild(4).GetComponent<BoxCollider>();
		Int32 num = 0;
		foreach (Object obj in this.CardSelectionListPanel.transform.GetChild(0))
		{
			Transform transform = (Transform)obj;
			Int32 num2 = num % 10 * 10;
			num2 += num / 10;
			num++;
			CardUI.CardListHUD cardListHUD = new CardUI.CardListHUD(transform.gameObject, num2);
			this.cardHudList.Add(cardListHUD);
			cardListHUD.CardHighlightAnimation.enabled = false;
			UIEventListener uieventListener = UIEventListener.Get(cardListHUD.Self);
			uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.onClick));
		}
		foreach (Object obj2 in this.CardInfoPanel.GetChild(0).GetChild(0).transform)
		{
			Transform transform2 = (Transform)obj2;
			CardDetailHUD item = new CardDetailHUD(transform2.gameObject);
			this.cardDetailHudList.Add(item);
		}
		UIEventListener uieventListener2 = UIEventListener.Get(this.DeleteSubmenuButton);
		uieventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener2.onClick, new UIEventListener.VoidDelegate(this.onClick));
		this.discardConfirmButton = this.DeleteDialogGameObject.GetChild(0).GetChild(1).GetComponent<ButtonGroupState>();
		this.discardCancelButton = this.DeleteDialogGameObject.GetChild(0).GetChild(2).GetComponent<ButtonGroupState>();
		UIEventListener uieventListener3 = UIEventListener.Get(this.discardConfirmButton.gameObject);
		uieventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener3.onClick, new UIEventListener.VoidDelegate(this.onClick));
		UIEventListener uieventListener4 = UIEventListener.Get(this.discardCancelButton.gameObject);
		uieventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener4.onClick, new UIEventListener.VoidDelegate(this.onClick));
		this.cardDetailTransition = this.TransitionPanel.GetChild(0).GetComponent<HonoTweenPosition>();
		this.deleteDialogTransition = this.TransitionPanel.GetChild(1).GetComponent<HonoTweenClipping>();
        UILabel component = this.PlayerInfoPanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<UILabel>();
        component.SetAnchor((Transform)null);
        component.width = 332;
        component.height = 40;
    }

	private static String CardGroupButton = "Card.Card";

	private static String ChoiceGroupButton = "Card.Choice";

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

	private ButtonGroupState discardConfirmButton;

	private ButtonGroupState discardCancelButton;

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

	private Int32[] offset = new Int32[100];

	private Byte[] count = new Byte[100];

	private Int32 lv_collector;

	private Int32 point;

	public class CardListHUD
	{
		public CardListHUD(GameObject go, Int32 id)
		{
			this.Self = go;
			this.Id = id;
			this.CardButtonGroup = go.GetComponent<ButtonGroupState>();
			this.CardIconSprite = go.GetChild(0).GetComponent<UISprite>();
			this.CardAmountLabel = go.GetChild(1).GetComponent<UILabel>();
			this.CardHighlightAnimation = go.GetChild(2).GetComponent<UISpriteAnimation>();
		}

		public GameObject Self;

		public Int32 Id;

		public UISprite CardIconSprite;

		public UILabel CardAmountLabel;

		public UISpriteAnimation CardHighlightAnimation;

		public ButtonGroupState CardButtonGroup;
	}
}
