using System;
using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
    public class ItemListDetailWithIconHUD
    {
        public ItemListDetailWithIconHUD()
        {
        }

        public ItemListDetailWithIconHUD(GameObject go, Boolean haveNumberLabel)
        {
            this.Self = go;
            this.Button = go.GetComponent<ButtonGroupState>();
            Int32 childCount = go.GetChild(0).transform.childCount;
            if (childCount > 0)
            {
                this.Content = go.GetChild(0);
                this.IconSprite = this.Content.GetChild(0).GetComponent<UISprite>();
                this.IconSpriteAnimation = this.Content.GetChild(0).GetComponent<UISpriteAnimation>();
                this.NameLabel = this.Content.GetChild(1).GetComponent<UILabel>();
                if (haveNumberLabel)
                {
                    this.NumberLabel = this.Content.GetChild(2).GetComponent<UILabel>();
                }
                else
                {
                    this.NumberLabel = (UILabel)null;
                }
            }
            else
            {
                this.Content = this.Self;
                this.IconSprite = this.Content.GetChild(0).GetComponent<UISprite>();
                this.IconSpriteAnimation = this.Content.GetChild(0).GetComponent<UISpriteAnimation>();
                this.NameLabel = this.Content.GetChild(1).GetComponent<UILabel>();
                if (haveNumberLabel)
                {
                    this.NumberLabel = this.Content.GetChild(2).GetComponent<UILabel>();
                }
                else
                {
                    this.NumberLabel = (UILabel)null;
                }
            }
        }

        public GameObject Self;

        public GameObject Content;

        public ButtonGroupState Button;

        public UISprite IconSprite;

        public UISpriteAnimation IconSpriteAnimation;

        public UILabel NameLabel;

        public UILabel NumberLabel;
    }
}
