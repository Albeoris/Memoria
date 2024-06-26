//using UnityEngine;

//// ReSharper disable NotAccessedField.Global

//namespace Memoria.Scenes.BattleHUD
//{
//    internal sealed class SOBattleHUD
//    {
//        public readonly global::BattleHUD Scene;

//        public readonly SpriteButton PauseButton;
//        public readonly SpriteButton HelpButton;
//        public readonly WidgetButton HitArea;

//        public readonly GOAllMenuPanel AllMenuPanel;
//        public readonly PanelBattleInformation BattleInformationPanel;
//        public readonly Transition Transitions;
//        public readonly SpriteFading Fading;

//        public SOBattleHUD(global::BattleHUD scene)
//        {
//            Scene = scene;

//            PauseButton = new SpriteButton(scene.PauseButtonGameObject);
//            HelpButton = new SpriteButton(scene.HelpButtonGameObject);
//            HitArea = new WidgetButton(scene.HideHudHitAreaGameObject);
//            AllMenuPanel = new GOAllMenuPanel(scene.AllMenuPanel);
//            BattleInformationPanel = new PanelBattleInformation(scene.BattleDialogGameObject);
//            Transitions = new Transition(scene.TransitionGameObject);
//            Fading = new SpriteFading(scene.ScreenFadeGameObject);
//        }

//        internal sealed class SpriteButton : GOSpriteButton
//        {
//            public readonly OnScreenButton OnScreenButton;

//            public SpriteButton(GameObject obj)
//                : base(obj)
//            {
//                OnScreenButton = obj.GetExactComponent<OnScreenButton>();
//            }
//        }

//        internal sealed class WidgetButton : GOWidgetButton
//        {
//            public readonly OnScreenButton OnScreenButton;

//            public WidgetButton(GameObject obj)
//                : base(obj)
//            {
//                OnScreenButton = obj.GetExactComponent<OnScreenButton>();
//            }
//        }

//        internal sealed class PanelBattleInformation : GOWidget
//        {
//            public readonly GOThinSpriteBackground Background;
//            public readonly GOLabel Label;

//            public PanelBattleInformation(GameObject obj)
//                : base(obj)
//            {
//                Background = new GOThinSpriteBackground(obj.GetChild(0));
//                Label = new GOLabel(obj.GetChild(1));
//            }
//        }

//        internal sealed class Transition : GOBase
//        {
//            public readonly ClippingPanel ItemPanel;
//            public readonly ClippingPanel AbilityPanel;
//            public readonly ClippingPanel TargetPanel;

//            public Transition(GameObject obj)
//                : base(obj)
//            {
//                ItemPanel = new ClippingPanel(obj.GetChild(0));
//                AbilityPanel = new ClippingPanel(obj.GetChild(1));
//                TargetPanel = new ClippingPanel(obj.GetChild(2));
//            }

//            internal sealed class ClippingPanel : GOBase
//            {
//                public readonly HonoTweenClipping Clipping;

//                public ClippingPanel(GameObject obj)
//                    : base(obj)
//                {
//                    Clipping = obj.GetExactComponent<HonoTweenClipping>();
//                }
//            }
//        }

//        internal sealed class SpriteFading : GOBase
//        {
//            public readonly UISprite Sprite;
//            public readonly TweenAlpha TweenAlpha;
//            public readonly HonoFading HonoFading;

//            public SpriteFading(GameObject obj)
//                : base(obj)
//            {
//                Sprite = obj.GetExactComponent<UISprite>();
//                TweenAlpha = obj.GetExactComponent<TweenAlpha>();
//                HonoFading = obj.GetExactComponent<HonoFading>();
//            }
//        }
//    }
//}