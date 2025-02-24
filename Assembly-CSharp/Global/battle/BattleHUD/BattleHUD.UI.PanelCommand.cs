using Memoria;
using Memoria.Data;
using Memoria.Scenes;
using System;
using System.Collections.Generic;
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

            public IEnumerable<GONavigationButton> EnumerateButtons(Boolean includeMenuIfPossible = true)
            {
                yield return Attack;
                yield return Defend;
                yield return Skill1;
                yield return Skill2;
                yield return Item;
                yield return Change;
                if (includeMenuIfPossible && AccessMenu != null)
                    yield return AccessMenu;
                yield break;
            }

            public GameObject GetCommandButton(BattleCommandMenu menu)
            {
                switch (menu)
                {
                    case BattleCommandMenu.Attack:
                        return Attack;
                    case BattleCommandMenu.Defend:
                        return Defend;
                    case BattleCommandMenu.Ability1:
                        return Skill1;
                    case BattleCommandMenu.Ability2:
                        return Skill2;
                    case BattleCommandMenu.Item:
                        return Item;
                    case BattleCommandMenu.Change:
                        return Change;
                    case BattleCommandMenu.AccessMenu:
                        return AccessMenu;
                    default:
                        return Attack;
                }
            }

            private void SubscribeOnClick()
            {
                foreach (GONavigationButton button in EnumerateButtons())
                    button.EventListener.Click += _scene.onClick;
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
