using System;
using System.IO;
using UnityEngine;

namespace Memoria.Test
{
	public class UITableMessage : UIWidgetContainerMessage
	{
		public Int32 Columns;
		public UITable.Direction Direction;
		public UITable.Sorting Sorting;
		public UIWidget.Pivot Pivot;
		public UIWidget.Pivot CellAlignment;
		public Boolean HideInactive = true;
		public Boolean KeepWithinPanel;
		public Vector2 Padding;

		public UITableMessage()
		{
		}

		public UITableMessage(UITable uiTable)
			: base(uiTable)
		{
			Columns = uiTable.columns;
			Direction = uiTable.direction;
			Sorting = uiTable.sorting;
			Pivot = uiTable.pivot;
			CellAlignment = uiTable.cellAlignment;
			HideInactive = uiTable.hideInactive;
			KeepWithinPanel = uiTable.keepWithinPanel;
			Padding = uiTable.padding;
		}

		public override void Serialize(BinaryWriter bw)
		{
			base.Serialize(bw);

			bw.Write(Columns);
			bw.Write((Int32)Direction);
			bw.Write((Int32)Sorting);
			bw.Write((Int32)Pivot);
			bw.Write((Int32)CellAlignment);
			bw.Write(HideInactive);
			bw.Write(KeepWithinPanel);
			bw.Write(Padding);
		}

		public override void Deserialize(BinaryReader br)
		{
			base.Deserialize(br);

			Columns = br.ReadInt32();
			Direction = (UITable.Direction)br.ReadInt32();
			Sorting = (UITable.Sorting)br.ReadInt32();
			Pivot = (UIWidget.Pivot)br.ReadInt32();
			CellAlignment = (UIWidget.Pivot)br.ReadInt32();
			HideInactive = br.ReadBoolean();
			KeepWithinPanel = br.ReadBoolean();
			Padding = br.ReadVector2();
		}
	}
}
