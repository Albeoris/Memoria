using System;
using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
    public class CharacterDetailHUD
    {
        public CharacterDetailHUD(GameObject go, Boolean isTargetHud)
        {
            this.Self = go;
            this.Content = go.GetChild(0);
            this.HPPanel = this.Content.GetChild(2);
            this.MPPanel = this.Content.GetChild(3);
            this.AvatarSprite = this.Content.GetChild(0).GetComponent<UISprite>();
            this.NameLabel = this.Content.GetChild(1).GetChild(0).GetComponent<UILabel>();
            this.LvLabel = this.Content.GetChild(1).GetChild(2).GetComponent<UILabel>();
            this.HPLabel = this.HPPanel.GetChild(1).GetComponent<UILabel>();
            this.hpSlashLabel = this.HPPanel.GetChild(2).GetComponent<UILabel>();
            this.HPMaxLabel = this.HPPanel.GetChild(3).GetComponent<UILabel>();
            this.MPLabel = this.MPPanel.GetChild(1).GetComponent<UILabel>();
            this.mpSlashLabel = this.MPPanel.GetChild(2).GetComponent<UILabel>();
            this.MPMaxLabel = this.MPPanel.GetChild(3).GetComponent<UILabel>();
            if (isTargetHud)
            {
                this.MagicStoneLabel = null;
                this.MagicStoneMaxLabel = null;
                this.StatusesPanel = this.Content.GetChild(4);
                this.StatusesSpriteList = new UISprite[]
                {
                    this.StatusesPanel.GetChild(0).GetChild(0).GetComponent<UISprite>(),
                    this.StatusesPanel.GetChild(0).GetChild(1).GetComponent<UISprite>(),
                    this.StatusesPanel.GetChild(0).GetChild(2).GetComponent<UISprite>(),
                    this.StatusesPanel.GetChild(0).GetChild(3).GetComponent<UISprite>(),
                    this.StatusesPanel.GetChild(0).GetChild(4).GetComponent<UISprite>(),
                    this.StatusesPanel.GetChild(0).GetChild(5).GetComponent<UISprite>(),
                    this.StatusesPanel.GetChild(0).GetChild(6).GetComponent<UISprite>()
                };
            }
            else
            {
                if (this.Content.GetChild(4).name == "Empty")
                {
                    this.MagicStoneLabel = null;
                    this.MagicStoneMaxLabel = null;
                }
                else
                {
                    this.MagicStoneLabel = this.Content.GetChild(4).GetChild(1).GetComponent<UILabel>();
                    this.magicStoneSlashLabel = this.Content.GetChild(4).GetChild(2).GetComponent<UILabel>();
                    this.MagicStoneMaxLabel = this.Content.GetChild(4).GetChild(3).GetComponent<UILabel>();
                }
                if (this.Content.GetChild(5).name == "Empty")
                {
                    this.StatusesPanel = null;
                    this.StatusesSpriteList = null;
                }
                else
                {
                    this.StatusesPanel = this.Content.GetChild(5);
                    this.StatusesSpriteList = new UISprite[]
                    {
                        this.StatusesPanel.GetChild(0).GetChild(0).GetComponent<UISprite>(),
                        this.StatusesPanel.GetChild(0).GetChild(1).GetComponent<UISprite>(),
                        this.StatusesPanel.GetChild(0).GetChild(2).GetComponent<UISprite>(),
                        this.StatusesPanel.GetChild(0).GetChild(3).GetComponent<UISprite>(),
                        this.StatusesPanel.GetChild(0).GetChild(4).GetComponent<UISprite>(),
                        this.StatusesPanel.GetChild(0).GetChild(5).GetComponent<UISprite>(),
                        this.StatusesPanel.GetChild(0).GetChild(6).GetComponent<UISprite>()
                    };
                }
            }
            this.HPLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
            this.HPMaxLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
            this.MPLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
            this.MPMaxLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
        }

        public Color HPTextColor
        {
            set
            {
                this.HPLabel.color = value;
                this.hpSlashLabel.color = value;
                this.HPMaxLabel.color = value;
            }
        }

        public Color MPTextColor
        {
            set
            {
                this.MPLabel.color = value;
                this.mpSlashLabel.color = value;
                this.MPMaxLabel.color = value;
            }
        }

        public Color MagicStoneTextColor
        {
            set
            {
                this.MagicStoneLabel.color = value;
                this.magicStoneSlashLabel.color = value;
                this.MagicStoneMaxLabel.color = value;
            }
        }

        public GameObject Self;
        public GameObject Content;
        public GameObject HPPanel;
        public GameObject MPPanel;
        public GameObject StatusesPanel;
        public UISprite AvatarSprite;
        public UILabel NameLabel;
        public UILabel LvLabel;
        public UILabel HPLabel;
        public UILabel HPMaxLabel;
        public UILabel MPLabel;
        public UILabel MPMaxLabel;
        public UILabel MagicStoneLabel;
        public UILabel MagicStoneMaxLabel;
        public UISprite[] StatusesSpriteList;
        private UILabel hpSlashLabel;
        private UILabel mpSlashLabel;
        private UILabel magicStoneSlashLabel;
    }
}
