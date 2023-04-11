using System;
using UnityEngine;

namespace Memoria.Scenes
{
    internal sealed class GOSubPanel : GOBase
    {
        public readonly UIPanel Panel;
        public readonly UIScrollView ScrollView;
        public readonly Rigidbody Rigidbody;
        public readonly SnapDragScrollView SnapDragScrollView;
        public readonly RecycleListPopulator RecycleListPopulator;

        public readonly GOButtonPrefab ButtonPrefab;
        public readonly GOTable<GOBase> Table;

        public GOSubPanel(GameObject obj)
            : base(obj)
        {
            Panel = obj.GetExactComponent<UIPanel>();
            ScrollView = obj.GetExactComponent<UIScrollView>();
            Rigidbody = obj.GetExactComponent<Rigidbody>();
            SnapDragScrollView = obj.GetExactComponent<SnapDragScrollView>();
            RecycleListPopulator = obj.GetExactComponent<RecycleListPopulator>();

            ButtonPrefab = new GOButtonPrefab(RecycleListPopulator.itemPrefab.gameObject);
            Table = new GOTable<GOBase>(obj.GetChild(0));
        }

        public void ChangeDims(Int32 colCount, Int32 rowCount, Single colWidth, Single rowHeight)
        {
            SnapDragScrollView.ItemHeight = (Int32)rowHeight;
            RecycleListPopulator.cellHeight = rowHeight;
            Table.Table.columns = colCount;
            ButtonPrefab.Widget.SetRawRect(0f, 0f, colWidth, rowHeight);
            ButtonPrefab.ScrollKeyNavigation.ItemHeight = rowHeight;
            Panel.baseClipRegion = new Vector4(Panel.baseClipRegion.x, Panel.baseClipRegion.y, colCount * colWidth, rowCount * rowHeight);
            if (GameObject.activeInHierarchy)
                RecycleListPopulator.RefreshTableView();
        }
    }
}