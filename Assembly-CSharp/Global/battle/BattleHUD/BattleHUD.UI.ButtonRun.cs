using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using Memoria.Scenes;
using UnityEngine;

public partial class BattleHUD : UIScene
{
    private partial class UI
    {
        internal sealed class ButtonRun : GOWidget
        {
            public readonly UIButton Button1, Button2;
            public readonly BoxCollider BoxCollider;

            public readonly GOThinSpriteBackground Background;
            public readonly GOSprite Highlight;
            public readonly GOSprite Icon;

            public ButtonRun(GameObject obj)
                : base(obj)
            {
                UIButton[] buttons = obj.GetExactComponents<UIButton>();
                Button1 = buttons[0];
                Button2 = buttons[1];
                BoxCollider = obj.GetExactComponent<BoxCollider>();

                Background = new GOThinSpriteBackground(obj.GetChild(0));
                Highlight = new GOSprite(obj.GetChild(1));
                Icon = new GOSprite(obj.GetChild(2));
            }
        }
    }
}