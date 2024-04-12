using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
	public class EquipmentDetailHud
	{
		public EquipmentDetailHud(GameObject go)
		{
			this.Self = go;
			this.Weapon = new ItemListDetailWithIconHUD(go.GetChild(0), false);
			this.Head = new ItemListDetailWithIconHUD(go.GetChild(1), false);
			this.Wrist = new ItemListDetailWithIconHUD(go.GetChild(2), false);
			this.Body = new ItemListDetailWithIconHUD(go.GetChild(3), false);
			this.Accessory = new ItemListDetailWithIconHUD(go.GetChild(4), false);
		}

		public GameObject Self;

		public ItemListDetailWithIconHUD Weapon;

		public ItemListDetailWithIconHUD Head;

		public ItemListDetailWithIconHUD Wrist;

		public ItemListDetailWithIconHUD Body;

		public ItemListDetailWithIconHUD Accessory;
	}
}
