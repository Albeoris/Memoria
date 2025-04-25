using System;
using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
    public class EquipmentDetailHud
    {
        public EquipmentDetailHud(GameObject go)
        {
            this.Self = go;
            this.Weapon = new EquipmentItemListHUD(go.GetChild(0));
            this.Head = new EquipmentItemListHUD(go.GetChild(1));
            this.Wrist = new EquipmentItemListHUD(go.GetChild(2));
            this.Body = new EquipmentItemListHUD(go.GetChild(3));
            this.Accessory = new EquipmentItemListHUD(go.GetChild(4));
        }

        public GameObject Self;
        public EquipmentItemListHUD Weapon;
        public EquipmentItemListHUD Head;
        public EquipmentItemListHUD Wrist;
        public EquipmentItemListHUD Body;
        public EquipmentItemListHUD Accessory;
    }

    public class EquipmentItemListHUD
    {
        public EquipmentItemListHUD(GameObject go)
        {
            this.Self = go;
            this.PartIconSprite = go.FindChild("Part Icon").GetComponent<UISprite>();
            this.IconSprite = go.FindChild("Equipment Icon").GetComponent<UISprite>();
            this.NameLabel = go.FindChild("Equipment Text").GetComponent<UILabel>();
            this.ColonLabel = go.FindChild("Colon").GetComponent<UILabel>();
            this.Button = go.GetComponent<ButtonGroupState>();
            this.NameLabel.overflowMethod = UILabel.Overflow.ShrinkContent;
            this.NameLabel.fixedAlignment = true;
        }

        public GameObject Self;
        public UISprite PartIconSprite;
        public UISprite IconSprite;
        public UILabel NameLabel;
        public UILabel ColonLabel;

        public ButtonGroupState Button; // null for non-selectable equip part
    }
}
