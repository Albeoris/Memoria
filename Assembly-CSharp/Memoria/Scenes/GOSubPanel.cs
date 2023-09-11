using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Memoria.Scenes
{
	internal sealed class GOSubPanel : GOBase
	{
		public readonly UIPanel Panel;
		public readonly UIScrollView ScrollView;
		public readonly Rigidbody Rigidbody;
		public readonly SnapDragScrollView SnapDragScrollView;

		public readonly GOTable<GOButtonPrefab> Table;

		public Boolean HasListPopulator => RecycleListPopulator != null;
		public readonly RecycleListPopulator RecycleListPopulator = null;
		public readonly GOButtonPrefab ButtonPrefab = null;

		public GOSubPanel(GameObject obj)
			: base(obj)
		{
			Panel = obj.GetExactComponent<UIPanel>();
			ScrollView = obj.GetExactComponent<UIScrollView>();
			Rigidbody = obj.GetExactComponent<Rigidbody>();
			SnapDragScrollView = obj.GetExactComponent<SnapDragScrollView>();
			Table = new GOTable<GOButtonPrefab>(obj.GetChild(0));

			if (obj.GetComponent<RecycleListPopulator>() != null)
			{
				RecycleListPopulator = obj.GetExactComponent<RecycleListPopulator>();
				ButtonPrefab = new GOButtonPrefab(RecycleListPopulator.itemPrefab.gameObject);
			}
			else
			{
				RecycleListPopulator = null;
				ButtonPrefab = null;
			}
		}

		public void ChangeDims(Int32 colCount, Int32 rowCount, Single colWidth, Single rowHeight)
		{
			colCount = Math.Max(colCount, 1);
			rowCount = Math.Max(rowCount, 1);
			SnapDragScrollView.ItemHeight = (Int32)rowHeight;
			SnapDragScrollView.VisibleItem = colCount * rowCount;
			Table.Table.columns = colCount;
			Panel.SetAnchor(target: null);
			Panel.baseClipRegion = new Vector4(Panel.baseClipRegion.x, Panel.baseClipRegion.y, colCount * colWidth, rowCount * rowHeight);
			if (HasListPopulator)
			{
				RecycleListPopulator.cellHeight = rowHeight;
				ButtonPrefab.Widget.SetRawRect(0f, 0f, colWidth, rowHeight);
				ButtonPrefab.ScrollKeyNavigation.ItemHeight = rowHeight;
				if (GameObject.activeInHierarchy)
					RecycleListPopulator.RefreshTableView();
			}
			else
			{
				for (Int32 i = 0; i < Table.Count; i++)
				{
					GOButtonPrefab button = Table.Entries[i];
					button.Widget.SetDimensions((Int32)colWidth, (Int32)rowHeight);
				}
				if (GameObject.activeInHierarchy)
					Table.Table.StartCoroutine(RepositionStaticList(colCount, rowCount, colWidth, rowHeight));
			}
		}

		private IEnumerator RepositionStaticList(Int32 colCount, Int32 rowCount, Single colWidth, Single rowHeight)
		{
			yield return new WaitForEndOfFrame();
			// The "pivotOffset" pick is hacky there; it applies to the list of Chocographs, which is currently the only GOSubPanel without list populator
			Vector2 pivotOffset = new Vector2(0f, 0.5f);
			for (Int32 i = 0; i < Table.Count; i++)
			{
				GOButtonPrefab button = Table.Entries[i];
				Int32 lineNo = i / colCount;
				Int32 columnNo = i % colCount;
				Single posX = Table.Table.padding.x + pivotOffset.x * button.Widget.width + columnNo * (2 * Table.Table.padding.x + button.Widget.width);
				Single posY = Table.Table.padding.y + pivotOffset.y * rowHeight + lineNo * (2 * Table.Table.padding.y + rowHeight);
				button.Widget.SetAnchor(target: null);
				button.Widget.SetRawRect(posX, -posY, (Int32)colWidth, (Int32)rowHeight);
				button.ScrollKeyNavigation.ItemHeight = rowHeight;
			}
		}
	}
}