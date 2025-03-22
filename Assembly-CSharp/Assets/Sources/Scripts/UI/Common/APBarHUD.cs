using System;
using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
    public class APBarHUD
    {
        public APBarHUD(GameObject go)
        {
            this.Self = go;
            this.SliderSprite = go.GetComponent<UISprite>();
            this.Slider = go.GetComponent<UISlider>();
            this.TextPanel = go.GetChild(1);
            this.ForegroundSprite = go.GetChild(0).GetComponent<UISprite>();
            this.APLabel = go.GetChild(1).GetChild(0).GetComponent<UILabel>();
            this.APMaxLabel = go.GetChild(1).GetChild(2).GetComponent<UILabel>();
            this.MasterSprite = go.GetChild(2).GetComponent<UISprite>();
            this.APMaxLabel.fixedAlignment = true;
        }

        public GameObject Self;
        public UISprite SliderSprite;
        public UISlider Slider;
        public UISprite ForegroundSprite;
        public UISprite MasterSprite;
        public GameObject TextPanel;
        public UILabel APLabel;
        public UILabel APMaxLabel;
    }
}
