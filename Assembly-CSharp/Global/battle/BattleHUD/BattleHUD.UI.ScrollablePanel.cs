using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using Memoria.Scenes;
using UnityEngine;

public partial class BattleHUD : UIScene
{
    private partial class UI
    {
        internal sealed class ScrollablePanel : GOWidget
        {
            public readonly GOScrollButton ScrollButton;
            public readonly GOSubPanel SubPanel;
            public readonly CaptionBackground Background;

            public ScrollablePanel(GameObject obj)
                : base(obj)
            {
                ScrollButton = new GOScrollButton(obj.GetChild(0));
                SubPanel = new GOSubPanel(obj.GetChild(1));
                Background = new CaptionBackground(obj.GetChild(2));
            }

            internal sealed class CaptionBackground : GOWidget
            {
                public readonly GOSprite Body;
                public readonly GOSprite TopBorder;
                public readonly GOSprite Border;
                public readonly CaptionPanel Panel;

                public CaptionBackground(GameObject obj)
                    : base(obj)
                {
                    TopBorder = new GOSprite(obj.GetChild(0));
                    Border = new GOSprite(obj.GetChild(1));
                    Body = new GOSprite(obj.GetChild(2));
                    Panel = new CaptionPanel(obj.GetChild(3));
                }
            }

            internal sealed class CaptionPanel : GOPanel
            {
                public readonly GOLocalizableLabel Name;
                public readonly GOLocalizableLabel Info;

                public CaptionPanel(GameObject obj)
                    : base(obj)
                {
                    Name = new GOLocalizableLabel(obj.GetChild(0));
                    Info = new GOLocalizableLabel(obj.GetChild(1));
                }
            }
        }
    }
}