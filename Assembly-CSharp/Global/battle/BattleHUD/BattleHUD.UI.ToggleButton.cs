using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using Memoria.Scenes;
using UnityEngine;

public partial class BattleHUD : UIScene
{
    private partial class UI
    {
        internal sealed class ToggleButton : GOWidget
        {
            public readonly UIButton Button;
            public readonly BoxCollider BoxCollider;
            public readonly OnScreenButton OnScreenButton;
            public readonly UIToggle Toggle;

            public readonly GOThinSpriteBackground Background;
            public readonly GOSprite Highlight;
            public readonly GOSprite Icon;
            public readonly GOSprite IconActive;

            public ToggleButton(GameObject obj)
                : base(obj)
            {
                Button = obj.GetExactComponent<UIButton>();
                BoxCollider = obj.GetExactComponent<BoxCollider>();
                OnScreenButton = obj.GetExactComponent<OnScreenButton>();
                Toggle = obj.GetExactComponent<UIToggle>();

                Background = new GOThinSpriteBackground(obj.GetChild(0));
                Highlight = new GOSprite(obj.GetChild(1));
                Icon = new GOSprite(obj.GetChild(2));
                IconActive = new GOSprite(obj.GetChild(3));
            }
        }
    }
}