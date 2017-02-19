using System;
using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
	public class ItemListDetailHUD
	{
		public ItemListDetailHUD(GameObject go)
		{
			this.Self = go;
			this.Content = go.GetChild(0);
			this.Button = go.GetComponent<ButtonGroupState>();
			this.NameLabel = this.Content.GetChild(0).GetComponent<UILabel>();
			this.NumberLabel = this.Content.GetChild(1).GetComponent<UILabel>();
		}

		public GameObject Self;

		public GameObject Content;

		public ButtonGroupState Button;

		public UILabel NameLabel;

		public UILabel NumberLabel;
	}
}
