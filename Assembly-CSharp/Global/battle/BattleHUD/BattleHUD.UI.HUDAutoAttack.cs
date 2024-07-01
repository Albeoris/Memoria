using Memoria.Scenes;
using System;
using UnityEngine;

public partial class BattleHUD : UIScene
{
    private partial class UI
    {
        internal sealed class HUDAutoAttack : GOBase
        {
            public readonly GOAnimatedSprite Sprite;
            public readonly GOBlinkedLabel BlinkedLabel;

            public HUDAutoAttack(GameObject obj)
                : base(obj)
            {
                Sprite = new GOAnimatedSprite(obj.GetChild(0));
                BlinkedLabel = new GOBlinkedLabel(obj.GetChild(1));
            }
        }
    }
}
