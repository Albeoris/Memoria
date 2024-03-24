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

			if (Configuration.TetraMaster.TripleTriad > 0)
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
            }
            //if (Configuration.TetraMaster.TripleTriad == 3) // [DV] TODO => Not finished yet... otherwise, i will remove it for the release.
            //{
            //    GameObject CardElement = UnityEngine.Object.Instantiate(AtkParamSprite.gameObject);
            //    CardElement.transform.SetParent(Self.GetChild(1).transform);
            //    CardElementSprite = Self.GetChild(1).GetChild(4).GetComponent<UISprite>();
            //    CardElementSprite.GetComponent<UISprite>().atlas = FF9UIDataTool.IconAtlas;
            //    CardElementSprite.GetComponent<UISprite>().spriteName = FF9UIDataTool.IconSpriteName[141]; // Heat
            //    CardElementSprite.SetAnchor(CardImageSprite.gameObject, 0.96f, 0.97f, 0.59f, 0.59f);
            //    CardElementSprite.transform.localScale /= 20; // [DV] TODO - Use a precise size instead but i don't know the function...
            //    CardElementSprite.GetComponent<UISprite>().depth = 100;
                // Memoria.Scenes.ControlPanel.DebugLogComponents(go, true, true, c => $" (child of {c.transform}) has component of type {c.GetType()}");
            //}
        }

		public GameObject Self;

		public UISprite CardImageSprite;
		public UISprite AtkParamSprite;
		public UISprite AtkTypeParamSprite;
		public UISprite PhysicDefParamSprite;
		public UISprite MagicDefParamSprite;
		public UISprite CardBorderSprite;
        //public UISprite CardElementSprite;

        public GameObject[] CardArrowList;
    }
}
