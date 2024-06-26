using System;
using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
	public class AbilityItemHUD
	{
		public AbilityItemHUD(GameObject go)
		{
			this.Self = go;
			this.IconSprite = go.GetChild(0).GetComponent<UISprite>();
			this.NameLabel = go.GetChild(1).GetComponent<UILabel>();
			this.APBar = new APBarHUD(go.GetChild(2));
		}

		public GameObject Self;

		public UISprite IconSprite;

		public UILabel NameLabel;

		public APBarHUD APBar;
	}
}
