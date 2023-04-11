using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using Memoria;
using Memoria.Assets;
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
            public readonly GONavigationButton AccessMenu;
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
                if (Configuration.Battle.AccessMenus > 0)
                {
                    GameObject itemDuplicate = UnityEngine.Object.Instantiate(obj.GetChild(4));
                    itemDuplicate.transform.parent = obj.transform;
                    AccessMenu = new GONavigationButton(itemDuplicate);
                    AccessMenu.Transform.localScale = Vector3.one;
                    AccessMenu.Widget.SetAnchor(Transform);
                    AccessMenu.Name.Label.SetAnchor(AccessMenu.Transform);
                    AccessMenu.Highlight.Sprite.SetAnchor(AccessMenu.Transform);
                    AccessMenu.Background.Widget.SetAnchor(AccessMenu.Transform);
                    AccessMenu.Background.Border.Sprite.SetAnchor(AccessMenu.Background.Transform);
                    AccessMenu.IsActive = false;
                }
                else
				{
                    AccessMenu = null;
                }

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
                    case CommandMenu.AccessMenu:
                        return AccessMenu;
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
                if (AccessMenu != null)
                    AccessMenu.EventListener.Click += _scene.onClick;
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