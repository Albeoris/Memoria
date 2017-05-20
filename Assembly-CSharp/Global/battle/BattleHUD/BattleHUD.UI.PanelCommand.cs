using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using Memoria.Scenes;
using UnityEngine;
using Object = System.Object;

public partial class BattleHUD : UIScene
{
    private partial class UI
    {
        internal sealed class PanelCommand : GOWidget
        {
            public readonly GONavigationButton Attack;
            public readonly GONavigationButton Defend;
            public readonly GONavigationButton Skill1;
            public readonly GONavigationButton Skill2;
            public readonly GONavigationButton Item;
            public readonly GONavigationButton Change;
            public readonly CaptionBackground Caption;

            private readonly BattleHUD _scene;

            public PanelCommand(BattleHUD scene, GameObject obj)
                : base(obj)
            {
                _scene = scene;

                Attack = new GONavigationButton(obj.GetChild(0));
                Defend = new GONavigationButton(obj.GetChild(1));
                Skill1 = new GONavigationButton(obj.GetChild(2));
                Skill2 = new GONavigationButton(obj.GetChild(3));
                Item = new GONavigationButton(obj.GetChild(4));
                Change = new GONavigationButton(obj.GetChild(5));
                Caption = new CaptionBackground(obj.GetChild(6));

                SubscribeOnClick();
            }

            public void SetCaptionColor(Color color)
            {
                Caption.Caption.Label.color = color;
            }

            public void SetCaptionText(String text)
            {
                Caption.Caption.Label.text = text;
            }

            public GameObject GetCommandButton(CommandMenu menu)
            {
                switch (menu)
                {
                    case CommandMenu.Attack:
                        return Attack;
                    case CommandMenu.Defend:
                        return Defend;
                    case CommandMenu.Ability1:
                        return Skill1;
                    case CommandMenu.Ability2:
                        return Skill2;
                    case CommandMenu.Item:
                        return Item;
                    case CommandMenu.Change:
                        return Change;
                    default:
                        return Attack;
                }
            }

            private void SubscribeOnClick()
            {
                Attack.EventListener.Click += _scene.onClick;
                Defend.EventListener.Click += _scene.onClick;
                Skill1.EventListener.Click += _scene.onClick;
                Skill2.EventListener.Click += _scene.onClick;
                Item.EventListener.Click += _scene.onClick;
                Change.EventListener.Click += _scene.onClick;
            }

            internal sealed class CaptionBackground : GOThinSpriteBackground
            {
                public readonly GOLabel Caption;

                public CaptionBackground(GameObject obj)
                    : base(obj)
                {
                    Caption = new GOLabel(obj.GetChild(2));
                }
            }
        }
    }
}