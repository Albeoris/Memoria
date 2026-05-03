using System;
using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
    public class ItemListDetailHUD
    {
        public ItemListDetailHUD(GameObject go)
        {
            Self = go;
            Content = go.GetChild(0);
            Button = go.GetComponent<ButtonGroupState>();
            NameLabel = Content.GetChild(0).GetComponent<UILabel>();
            NumberLabel = Content.GetChild(1).GetComponent<UILabel>();
            NameLabel.fixedAlignment = true;
        }

        public GameObject Self;
        public GameObject Content;
        public ButtonGroupState Button;
        public UILabel NameLabel;
        public UILabel NumberLabel;
    }
}
