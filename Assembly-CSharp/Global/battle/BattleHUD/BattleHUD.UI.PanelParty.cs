using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Memoria.Scenes;
using UnityEngine;
using Object = System.Object;

public partial class BattleHUD : UIScene
{
    private partial class UI
    {
        internal sealed class PanelParty : GOWidget
        {
            private readonly BattleHUD _scene;

            public readonly GOTable<Character> Characters;
            public readonly CaptionBackground Captions;

            public PanelParty(BattleHUD scene, GameObject obj)
                : base(obj)
            {
                _scene = scene;

                Characters = new GOTable<Character>(obj.GetChild(0));
                Captions = new CaptionBackground(obj.GetChild(1));

                foreach (Character character in Characters.Entries)
                    character.EventListener.Click += OnCharacterClick;
            }

            public void SetActive(Boolean value)
            {
                Captions.HP.IsActive = value;
                Captions.MP.IsActive = value;
                Captions.ATB.IsActive = value;
            }

            public void SetBlink(Boolean value)
            {
                foreach (Character character in Characters.Entries)
                {
                    character.ATBBlink = value;
                    character.TranceBlink = value;
                }
            }

            public void SetBlink(Int32 characterIndex, Boolean value)
            {
                Character character = FindCharacter(characterIndex);
                if (character != null)
                {
                    character.ATBBlink = false;
                    character.TranceBlink = false;
                }
            }

            public Character GetCharacter(Int32 characterIndex)
            {
                return Characters.Entries.First(c => c.PlayerId == characterIndex);
            }

            public Character FindCharacter(Int32 characterIndex)
            {
                return Characters.Entries.FirstOrDefault(c => c.PlayerId == characterIndex);
            }

            public void SetDetailButtonState(UIButtonColor.State state, Boolean instant)
            {
                foreach (Character character in Characters.Entries)
                    character.ButtonColor.SetState(state, instant);
            }

            public void SetPartySwapButtonActive(Boolean isActive)
            {
                foreach (Character character in Characters.Entries)
                {
                    if (_scene.CurrentPlayerIndex == character.PlayerId)
                    {
                        character.BoxCollider.enabled = false;
                        character.ButtonColor.disabledColor = character.ButtonColor.pressed;
                    }
                    else
                    {
                        character.BoxCollider.enabled = isActive;
                        character.ButtonColor.disabledColor = character.ButtonColor.defaultColor;
                    }
                }
            }

            private void OnCharacterClick(GameObject go)
            {
                if (go.GetParent() != Characters.GameObject)
                {
                    _scene.onClick(go);
                    return;
                }

                Int32 playerId = Characters[go.transform.GetSiblingIndex()].PlayerId;
                if (playerId == _scene.CurrentPlayerIndex
                    ||!_scene.ReadyQueue.Contains(playerId)
                    || _scene.InputFinishList.Contains(playerId)
                    || _scene._unconsciousStateList.Contains(playerId))
                    return;

                _scene.SwitchPlayer(playerId);
            }

            internal sealed class Character : GOWidgetButton
            {
                public readonly GOLabel Name;
                public readonly GOLabel HP;
                public readonly GOLabel MP;
                public readonly GOProgressBar ATBBar;
                public readonly GOProgressBar TranceBar;
                public readonly GOSprite Highlight;
                public readonly UIEventListener EventListener;

                public readonly UILabel Label;
                public readonly ButtonGroupState ButtonGroup;
                public readonly UIButtonColor ButtonColor;

                public Int32 PlayerId { get; set; }

                private Boolean _atbBlink;
                private Boolean _tranceBlink;

                public Character(GameObject obj)
                    : base(obj)
                {
                    Name = new GOLabel(obj.GetChild(0));
                    HP = new GOLabel(obj.GetChild(1));
                    MP = new GOLabel(obj.GetChild(2));
                    ATBBar = new GOProgressBar(obj.GetChild(3));
                    TranceBar = new GOProgressBar(obj.GetChild(4));
                    Highlight = new GOSprite(obj.GetChild(5));
                    HP.Label.overflowMethod = UILabel.Overflow.ResizeFreely;
                    MP.Label.overflowMethod = UILabel.Overflow.ResizeFreely;

                    Label = obj.GetComponent<UILabel>();
                    ButtonGroup = obj.GetComponent<ButtonGroupState>();
                    ButtonColor = obj.GetComponent<UIButtonColor>();
                    EventListener = obj.EnsureExactComponent<UIEventListener>();
                }

                public Boolean ATBBlink
                {
                    get { return _atbBlink; }
                    set
                    {
                        ATBBar.Foreground.Highlight.Sprite.alpha = value ? 0.6f : 0.0f;
                        ATBBar.Foreground.Widget.alpha = value ? 0.0f : 1f;
                        _atbBlink = value;
                    }
                }

                public Boolean TranceBlink
                {
                    get { return _tranceBlink; }
                    set
                    {
                        TranceBar.Foreground.Highlight.Sprite.alpha = value ? 0.6f : 0.0f;
                        TranceBar.Foreground.Widget.alpha = value ? 0.0f : 1f;
                        _tranceBlink = value;
                    }
                }
            }

            internal sealed class CaptionBackground : GOWidget
            {
                public readonly GOSprite TopBorder;
                public readonly GOLocalizableLabel Name;
                public readonly GOLocalizableLabel HP;
                public readonly GOLocalizableLabel MP;
                public readonly GOLocalizableLabel ATB;

                public CaptionBackground(GameObject obj)
                    : base(obj)
                {
                    TopBorder = new GOSprite(obj.GetChild(0));
                    Name = new GOLocalizableLabel(obj.GetChild(1));
                    HP = new GOLocalizableLabel(obj.GetChild(2));
                    MP = new GOLocalizableLabel(obj.GetChild(3));
                    ATB = new GOLocalizableLabel(obj.GetChild(4));
                }
            }
        }
    }
}