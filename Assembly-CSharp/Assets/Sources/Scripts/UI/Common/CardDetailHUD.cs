using System;
using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
	public class CardDetailHUD
	{
		public CardDetailHUD(GameObject go)
		{
			this.Self = go;
			this.CardImageSprite = this.Self.GetChild(3).GetComponent<UISprite>();
			this.AtkParamSprite = this.Self.GetChild(1).GetChild(0).GetComponent<UISprite>();
			this.AtkTypeParamSprite = this.Self.GetChild(1).GetChild(1).GetComponent<UISprite>();
			this.PhysicDefParamSprite = this.Self.GetChild(1).GetChild(2).GetComponent<UISprite>();
			this.MagicDefParamSprite = this.Self.GetChild(1).GetChild(3).GetComponent<UISprite>();
			this.CardArrowList = new GameObject[]
			{
				this.Self.GetChild(0).GetChild(0),
				this.Self.GetChild(0).GetChild(1),
				this.Self.GetChild(0).GetChild(2),
				this.Self.GetChild(0).GetChild(3),
				this.Self.GetChild(0).GetChild(4),
				this.Self.GetChild(0).GetChild(5),
				this.Self.GetChild(0).GetChild(6),
				this.Self.GetChild(0).GetChild(7)
			};
		}

		public GameObject Self;

		public UISprite CardImageSprite;

		public UISprite AtkParamSprite;

		public UISprite AtkTypeParamSprite;

		public UISprite PhysicDefParamSprite;

		public UISprite MagicDefParamSprite;

		public GameObject[] CardArrowList;
	}
}
