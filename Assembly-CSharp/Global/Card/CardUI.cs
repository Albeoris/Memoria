using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Prime;
using Memoria.Prime.Threading;
using Memoria.Scenes;
using Memoria.Test;
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
						ButtonGroupState.RemoveCursorMemorize(CardUI.DiscardDialogButtonGroup);
						ButtonGroupState.ActiveGroup = CardUI.DiscardDialogButtonGroup;
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
			else if (ButtonGroupState.ActiveGroup == CardUI.DiscardDialogButtonGroup)
			{
				if (go == this.DeleteSubmenuButton)
				{
					return true;
				}

			    OnDiscardDialogKeyConfirm(go);

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

    private void OnDiscardDialogKeyConfirm(GameObject go)
    {
        switch (go.transform.GetSiblingIndex())
        {
            case 1: // Confirm
            {
                if (this.count[this.deleteCardId] < 1)
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
        FF9Sfx.FF9SFX_Play(103);
        QuadMistDatabase.MiniGame_AwayCard(this.deleteCardId, this.offset[this.deleteCardId]);
        count[deleteCardId] = (Byte)(count[deleteCardId] - 1);
        this.offset[this.deleteCardId] = Math.Min(this.offset[this.deleteCardId], (Int32)(this.count[this.deleteCardId] - 1));
        this.DisplayHelp();
        this.DisplayInfo();
        this.DisplayCardList();
        this.DisplayCardDetail();
    }

    private void DiscardUnnecessaryCards()
    {
        FF9Sfx.FF9SFX_Play(103);
        QuadMistDatabase.DiscardUnnecessaryCards();

        for (Int32 i = 0; i < 100; i++)
        {
            this.count[i] = 0;
            this.offset[i] = 0;
        }

        foreach (QuadMistCard quadMistCard in FF9StateSystem.MiniGame.SavedData.MiniGameCard)
        {
            this.count[quadMistCard.id]++;
        }

        this.DisplayHelp();
        this.DisplayCardList();
        this.DisplayCardDetail();
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
			else if (ButtonGroupState.ActiveGroup == CardUI.DiscardDialogButtonGroup)
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
			this.DiscardConfirmButton.Help.Enable = true;
			this.DiscardConfirmButton.Help.TextKey = "ConfirmDiscardHelp";
			this.DiscardConfirmButton.Help.Tail = true;

            this.DiscardCancelButton.Help.Enable = true;
			this.DiscardCancelButton.Help.TextKey = "CancelDiscardHelp";
			this.DiscardCancelButton.Help.Tail = true;

		    if (this.DiscardAutoButton != null)
		    {
		        this.DiscardAutoButton.Help.Enable = true;
		        this.DiscardAutoButton.Help.TextKey = String.Empty;
		        this.DiscardAutoButton.Help.Text = "Discard all unnecessary cards.";
		        this.DiscardAutoButton.Help.Tail = true;
		    }

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
		this.FF9FCard_GetPoint();
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
		    this.count[i] = 0;
			this.offset[i] = 0;
		}

        foreach (QuadMistCard quadMistCard in FF9StateSystem.MiniGame.SavedData.MiniGameCard)
        {
            this.count[quadMistCard.id]++;
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
		bool[] array = new bool[CardUI.FF9FCARD_ARROW_TYPE_MAX];
		int num = 0;
		int num2 = 0;
		byte[] array2 = new byte[]
		{
			0,
			0,
			1,
			2
		};
		int num3 = QuadMistDatabase.MiniGame_GetCardKindCount() * 10;
		for (int i = 0; i < 100; i++)
		{
			for (int j = 0; j < (int)this.count[i]; j++)
			{
				QuadMistCard quadMistCard = QuadMistDatabase.MiniGame_GetCardInfoPtr(i, j);
				array[(int)quadMistCard.arrow] = true;
				num += (int)array2[(int)quadMistCard.type];
			}
		}
		for (int k = 0; k < CardUI.FF9FCARD_ARROW_TYPE_MAX; k++)
		{
			if (array[k])
			{
				num2 += 5;
			}
		}
		this.point = num3 + num + num2;
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
	    uieventListener2.Click += onClick;

        _uiDiscardDialog = new UiDiscardDialog(DeleteDialogGameObject);
	    _uiDiscardDialog.Content.Confirm.UiEventListener.onClick += onClick;
        _uiDiscardDialog.Content.Cancel.UiEventListener.onClick += onClick;

	    if (_uiDiscardDialog.Content.Auto != null)
	        _uiDiscardDialog.Content.Auto.UiEventListener.onClick += onClick;

        this.cardDetailTransition = this.TransitionPanel.GetChild(0).GetComponent<HonoTweenPosition>();
		this.deleteDialogTransition = this.TransitionPanel.GetChild(1).GetComponent<HonoTweenClipping>();
        UILabel component = this.PlayerInfoPanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<UILabel>();
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
    //
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

	private Int32[] offset = new Int32[100];

	private Byte[] count = new Byte[100];

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
