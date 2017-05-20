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
            public readonly GOPanel<GOLocalizableLabel> Panel;

            public ScrollablePanel(GameObject obj)
                : base(obj)
            {
                ScrollButton = new GOScrollButton(obj.GetChild(0));
                SubPanel = new GOSubPanel(obj.GetChild(1));
                Panel = new GOPanel<GOLocalizableLabel>(obj.GetChild(2));
            }
        }
    }
}