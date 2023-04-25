using Memoria;
using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
	public class CardDetailHUD
	{
		public CardDetailHUD(GameObject go)
		{
			Self = go;
			CardImageSprite = Self.GetChild(3).GetComponent<UISprite>();
			AtkParamSprite = Self.GetChild(1).GetChild(0).GetComponent<UISprite>();
			AtkTypeParamSprite = Self.GetChild(1).GetChild(1).GetComponent<UISprite>();
			PhysicDefParamSprite = Self.GetChild(1).GetChild(2).GetComponent<UISprite>();
			MagicDefParamSprite = Self.GetChild(1).GetChild(3).GetComponent<UISprite>();
			CardBorderSprite = Self.GetChild(2).GetComponent<UISprite>();
			CardArrowList = new GameObject[]
			{
				Self.GetChild(0).GetChild(0),
				Self.GetChild(0).GetChild(1),
				Self.GetChild(0).GetChild(2),
				Self.GetChild(0).GetChild(3),
				Self.GetChild(0).GetChild(4),
				Self.GetChild(0).GetChild(5),
				Self.GetChild(0).GetChild(6),
				Self.GetChild(0).GetChild(7)
			};

			if (Configuration.Mod.TranceSeek || Configuration.TetraMaster.TripleTriad > 0)
			{
                AtkParamSprite.BottomAnchorPosition = new UIRect.Position(0f, 190);
                AtkParamSprite.TopAnchorPosition = new UIRect.Position(0f, 220);
                AtkParamSprite.LeftAnchorPosition = new UIRect.Position(0f, 30);
                AtkParamSprite.RightAnchorPosition = new UIRect.Position(0f, 60);
                AtkTypeParamSprite.BottomAnchorPosition = new UIRect.Position(0f, -60);
                AtkTypeParamSprite.TopAnchorPosition = new UIRect.Position(1f, -60);
                AtkTypeParamSprite.LeftAnchorPosition = new UIRect.Position(0f, 0);
                AtkTypeParamSprite.RightAnchorPosition = new UIRect.Position(1f, 0);
                PhysicDefParamSprite.BottomAnchorPosition = new UIRect.Position(0f, 30);
                PhysicDefParamSprite.TopAnchorPosition = new UIRect.Position(1f, 30);
                PhysicDefParamSprite.LeftAnchorPosition = new UIRect.Position(0f, -15);
                PhysicDefParamSprite.RightAnchorPosition = new UIRect.Position(1f, -15);
                MagicDefParamSprite.BottomAnchorPosition = new UIRect.Position(0f, 0);
                MagicDefParamSprite.TopAnchorPosition = new UIRect.Position(1f, 0);
                MagicDefParamSprite.LeftAnchorPosition = new UIRect.Position(0f, 30);
                MagicDefParamSprite.RightAnchorPosition = new UIRect.Position(1f, 30);
                if ((CardBorderSprite.atlas.spriteList.Find(sprt => sprt.name == "goldenbluecardframe") == null) && Configuration.Mod.TranceSeek)
				{
                    CardBorderSprite.atlas.spriteList.Add(new UISpriteData
                    {
                        name = "goldenbluecardframe",
                        x = 1842,
                        y = 1798,
                        width = 204,
                        height = 247,
                    });
                }
            }

		}

		public GameObject Self;

		public UISprite CardImageSprite;

		public UISprite AtkParamSprite;

		public UISprite AtkTypeParamSprite;

		public UISprite PhysicDefParamSprite;

		public UISprite MagicDefParamSprite;

		public UISprite CardBorderSprite;

		public GameObject[] CardArrowList;
	}
}
