//using UnityEngine;

//// ReSharper disable ClassNeverInstantiated.Global
//// ReSharper disable NotAccessedField.Global

//namespace Memoria.Scenes.BattleHUD
//{
//    internal sealed class GOAllMenuPanel : GOBase
//    {
//        public readonly ButtonBack BackButton;
//        public readonly ToggleButton AllTargetButton;
//        public readonly ToggleButton AutoAttackButton;
//        public readonly ButtonRun RunButton;
//        public readonly ContainerStatus StatusContainer;
//        public readonly PanelCommand CommandPanel;
//        public readonly PanelTarget TargetPanel;
//        public readonly ScrollablePanel AbilityPanel;
//        public readonly ScrollablePanel ItemPanel;
//        public readonly PanelParty PartyPanel;
//        public readonly HUDAutoAttack AutoAttackHUD;

//        public GOAllMenuPanel(GameObject obj)
//            : base(obj)
//        {
//            BackButton = new ButtonBack(obj.GetChild(0));
//            AllTargetButton = new ToggleButton(obj.GetChild(1));
//            AutoAttackButton = new ToggleButton(obj.GetChild(2));
//            RunButton = new ButtonRun(obj.GetChild(3));
//            StatusContainer = new ContainerStatus(obj.GetChild(4));
//            CommandPanel = new PanelCommand(obj.GetChild(5));
//            TargetPanel = new PanelTarget(obj.GetChild(6));
//            AbilityPanel = new ScrollablePanel(obj.GetChild(7));
//            ItemPanel = new ScrollablePanel(obj.GetChild(8));
//            PartyPanel = new PanelParty(obj.GetChild(9));
//            AutoAttackHUD = new HUDAutoAttack(obj.GetChild(10));
//        }

//        internal sealed class ButtonBack : GOWidget
//        {
//            public readonly UIButton Button;
//            public readonly BoxCollider BoxCollider;
//            public readonly OnScreenButton OnScreenButton;

//            public readonly GOThinSpriteBackground Background;
//            public readonly GOSprite Highlight;
//            public readonly GOSprite Icon;

//            public ButtonBack(GameObject obj)
//                : base(obj)
//            {
//                Button = obj.GetExactComponent<UIButton>();
//                BoxCollider = obj.GetExactComponent<BoxCollider>();
//                OnScreenButton = obj.GetExactComponent<OnScreenButton>();

//                Background = new GOThinSpriteBackground(obj.GetChild(0));
//                Highlight = new GOSprite(obj.GetChild(1));
//                Icon = new GOSprite(obj.GetChild(2));
//            }
//        }

//        internal sealed class ToggleButton : GOWidget
//        {
//            public readonly UIButton Button;
//            public readonly BoxCollider BoxCollider;
//            public readonly OnScreenButton OnScreenButton;
//            public readonly UIToggle Toggle;

//            public readonly GOThinSpriteBackground Background;
//            public readonly GOSprite Highlight;
//            public readonly GOSprite Icon;
//            public readonly GOSprite IconActive;

//            public ToggleButton(GameObject obj)
//                : base(obj)
//            {
//                Button = obj.GetExactComponent<UIButton>();
//                BoxCollider = obj.GetExactComponent<BoxCollider>();
//                OnScreenButton = obj.GetExactComponent<OnScreenButton>();
//                Toggle = obj.GetExactComponent<UIToggle>();

//                Background = new GOThinSpriteBackground(obj.GetChild(0));
//                Highlight = new GOSprite(obj.GetChild(1));
//                Icon = new GOSprite(obj.GetChild(2));
//                IconActive = new GOSprite(obj.GetChild(3));
//            }
//        }

//        internal sealed class ButtonRun : GOWidget
//        {
//            public readonly UIButton Button1, Button2;
//            public readonly BoxCollider BoxCollider;

//            public readonly GOThinSpriteBackground Background;
//            public readonly GOSprite Highlight;
//            public readonly GOSprite Icon;

//            public ButtonRun(GameObject obj)
//                : base(obj)
//            {
//                UIButton[] buttons = obj.GetExactComponents<UIButton>();
//                Button1 = buttons[0];
//                Button2 = buttons[1];
//                BoxCollider = obj.GetExactComponent<BoxCollider>();

//                Background = new GOThinSpriteBackground(obj.GetChild(0));
//                Highlight = new GOSprite(obj.GetChild(1));
//                Icon = new GOSprite(obj.GetChild(2));
//            }
//        }

//        internal sealed class ContainerStatus : GOBase
//        {
//            public readonly PanelDetail<ValueWidget> HP;
//            public readonly PanelDetail<ValueWidget> MP;
//            public readonly PanelDetail<IconsWidget> GoodStatus;
//            public readonly PanelDetail<IconsWidget> BadStatus;

//            public ContainerStatus(GameObject obj)
//                : base(obj)
//            {
//                HP = new PanelDetail<ValueWidget>(obj.GetChild(0));
//                MP = new PanelDetail<ValueWidget>(obj.GetChild(1));
//                GoodStatus = new PanelDetail<IconsWidget>(obj.GetChild(2));
//                BadStatus = new PanelDetail<IconsWidget>(obj.GetChild(3));
//            }

//            internal sealed class PanelDetail<T> : GOWidget where T : GOBase
//            {
//                public readonly GOArray<T> Array;
//                public readonly CaptionBackground<GOLocalizableLabel> Caption;

//                public PanelDetail(GameObject obj)
//                    : base(obj)
//                {
//                    Array = new GOArray<T>(obj.GetChild(0));
//                    Caption = new CaptionBackground<GOLocalizableLabel>(obj.GetChild(1));
//                }

//                internal sealed class CaptionBackground<T1> : GOWidget where T1 : GOBase
//                {
//                    public readonly GOSprite TopBorder;
//                    public readonly GOSprite Border;
//                    public readonly GOSprite Body;
//                    public readonly T1 Content;

//                    public CaptionBackground(GameObject obj)
//                        : base(obj)
//                    {
//                        TopBorder = new GOSprite(obj.GetChild(0));
//                        Border = new GOSprite(obj.GetChild(1));
//                        Body = new GOSprite(obj.GetChild(2));
//                        Content = Create<T1>(obj.GetChild(3));
//                    }
//                }
//            }

//            internal sealed class ValueWidget : GOWidget
//            {
//                public readonly GOLabel Value;
//                public readonly GOLabel Slash;
//                public readonly GOLabel MaxValue;
//                public readonly GOThinBackground Background;

//                public ValueWidget(GameObject obj)
//                    : base(obj)
//                {
//                    Value = new GOLabel(obj.GetChild(0));
//                    Slash = new GOLabel(obj.GetChild(1));
//                    MaxValue = new GOLabel(obj.GetChild(2));
//                    Background = new GOThinBackground(obj.GetChild(3));
//                }
//            }

//            internal sealed class IconsWidget : GOWidget
//            {
//                public readonly GOTable<GOSprite> Icons;
//                public readonly GOThinBackground Background;

//                public IconsWidget(GameObject obj)
//                    : base(obj)
//                {
//                    Icons = new GOTable<GOSprite>(obj.GetChild(0));
//                    Background = new GOThinBackground(obj.GetChild(1));
//                }
//            }
//        }

//        internal sealed class PanelCommand : GOWidget
//        {
//            public readonly GONavigationButton Attack;
//            public readonly GONavigationButton Defend;
//            public readonly GONavigationButton Skill1;
//            public readonly GONavigationButton Skill2;
//            public readonly GONavigationButton Item;
//            public readonly GONavigationButton Change;
//            public readonly CaptionBackground Caption;

//            public PanelCommand(GameObject obj)
//                : base(obj)
//            {
//                Attack = new GONavigationButton(obj.GetChild(0));
//                Defend = new GONavigationButton(obj.GetChild(1));
//                Skill1 = new GONavigationButton(obj.GetChild(2));
//                Skill2 = new GONavigationButton(obj.GetChild(3));
//                Item = new GONavigationButton(obj.GetChild(4));
//                Change = new GONavigationButton(obj.GetChild(5));
//                Caption = new CaptionBackground(obj.GetChild(6));
//            }

//            internal sealed class CaptionBackground : GOThinSpriteBackground
//            {
//                public readonly GOLabel Caption;

//                public CaptionBackground(GameObject obj)
//                    : base(obj)
//                {
//                    Caption = new GOLabel(obj.GetChild(2));
//                }
//            }
//        }

//        internal sealed class PanelTarget : GOWidget
//        {
//            public readonly GOTable<GONavigationButton> PlayerTable;
//            public readonly GOTable<GONavigationButton> EnemyTable;
//            public readonly ButtonPair Buttons;
//            public readonly GOWidgetButton PreventArea;
//            public readonly CaptionBackground Captions;

//            public PanelTarget(GameObject obj)
//                : base(obj)
//            {
//                PlayerTable = new GOTable<GONavigationButton>(obj.GetChild(0));
//                EnemyTable = new GOTable<GONavigationButton>(obj.GetChild(1));
//                Buttons = new ButtonPair(obj.GetChild(2));
//                PreventArea = new GOWidgetButton(obj.GetChild(3));
//                Captions = new CaptionBackground(obj.GetChild(4));
//            }

//            internal sealed class ButtonPair : GOBase
//            {
//                public readonly GOWidgetButton Player;
//                public readonly GOWidgetButton Enemy;

//                public ButtonPair(GameObject obj)
//                    : base(obj)
//                {
//                    Player = new GOWidgetButton(obj.GetChild(0));
//                    Enemy = new GOWidgetButton(obj.GetChild(1));
//                }
//            }

//            internal sealed class CaptionBackground : GOThinSpriteBackground
//            {
//                public readonly GOLocalizableLabel Caption1;
//                public readonly GOLocalizableLabel Caption2;

//                public CaptionBackground(GameObject obj)
//                    : base(obj)
//                {
//                    Caption1 = new GOLocalizableLabel(obj.GetChild(2));
//                    Caption2 = new GOLocalizableLabel(obj.GetChild(3));
//                }
//            }
//        }

//        internal sealed class ScrollablePanel : GOWidget
//        {
//            public readonly GOScrollButton ScrollButton;
//            public readonly GOSubPanel SubPanel;
//            public readonly GOPanel<GOLocalizableLabel> Panel;

//            public ScrollablePanel(GameObject obj)
//                : base(obj)
//            {
//                ScrollButton = new GOScrollButton(obj.GetChild(0));
//                SubPanel = new GOSubPanel(obj.GetChild(1));
//                Panel = new GOPanel<GOLocalizableLabel>(obj.GetChild(2));
//            }
//        }

//        internal sealed class PanelParty : GOWidget
//        {
//            public readonly GOTable<Character> Table;
//            public readonly CaptionBackground Captions;

//            public PanelParty(GameObject obj)
//                : base(obj)
//            {
//                Table = new GOTable<Character>(obj.GetChild(0));
//                Captions = new CaptionBackground(obj.GetChild(1));
//            }

//            internal sealed class Character : GOWidgetButton
//            {
//                public readonly GOLabel Name;
//                public readonly GOLabel HP;
//                public readonly GOLabel MP;
//                public readonly GOProgressBar ATBBar;
//                public readonly GOProgressBar TranceBar;
//                public readonly GOSprite Highlight;

//                public Character(GameObject obj)
//                    : base(obj)
//                {
//                    Name = new GOLabel(obj.GetChild(0));
//                    HP = new GOLabel(obj.GetChild(1));
//                    MP = new GOLabel(obj.GetChild(2));
//                    ATBBar = new GOProgressBar(obj.GetChild(3));
//                    TranceBar = new GOProgressBar(obj.GetChild(4));
//                    Highlight = new GOSprite(obj.GetChild(5));
//                }
//            }

//            internal sealed class CaptionBackground : GOWidget
//            {
//                public readonly GOSprite TopBorder;
//                public readonly GOLocalizableLabel Caption1;
//                public readonly GOLocalizableLabel Caption2;
//                public readonly GOLocalizableLabel Caption3;
//                public readonly GOLocalizableLabel Caption4;

//                public CaptionBackground(GameObject obj)
//                    : base(obj)
//                {
//                    TopBorder = new GOSprite(obj.GetChild(0));
//                    Caption1 = new GOLocalizableLabel(obj.GetChild(1));
//                    Caption2 = new GOLocalizableLabel(obj.GetChild(2));
//                    Caption3 = new GOLocalizableLabel(obj.GetChild(3));
//                    Caption4 = new GOLocalizableLabel(obj.GetChild(4));
//                }
//            }
//        }

//        internal sealed class HUDAutoAttack : GOBase
//        {
//            public readonly GOAnimatedSprite Sprite;
//            public readonly GOBlinkedLabel BlinkedLabel;

//            public HUDAutoAttack(GameObject obj)
//                : base(obj)
//            {
//                Sprite = new GOAnimatedSprite(obj.GetChild(0));
//                BlinkedLabel = new GOBlinkedLabel(obj.GetChild(1));
//            }
//        }
//    }
//}