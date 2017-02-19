using System;
using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
	public class APBarHUD
	{
		public APBarHUD(GameObject go)
		{
			this.Self = go;
			this.Slider = go.GetComponent<UISlider>();
			this.TextPanel = go.GetChild(1);
			this.ForegroundSprite = go.GetChild(0).GetComponent<UISprite>();
			this.APLable = go.GetChild(1).GetChild(0).GetComponent<UILabel>();
			this.APMaxLable = go.GetChild(1).GetChild(2).GetComponent<UILabel>();
			this.MasterSprite = go.GetChild(2).GetComponent<UISprite>();
		}

		public GameObject Self;

		public UISlider Slider;

		public UISprite ForegroundSprite;

		public UISprite MasterSprite;

		public GameObject TextPanel;

		public UILabel APLable;

		public UILabel APMaxLable;
	}
}
