using System;
using System.IO;
using UnityEngine;

namespace Memoria.Test
{
	public class UIPanelMessage : UIRectMessage
	{
		public Single Width;
		public Single Height;
		public Int32 Depth;
		public Int32 SortingOrder;
		public UIDrawCall.Clipping Clipping;
		public Vector2 ClipOffset;
		public Vector4 ClipRegion;
		public Vector2 ClipSoftness;

		public UIPanelMessage()
		{
		}

		public UIPanelMessage(UIPanel uiPanel)
			: base(uiPanel)
		{
			Width = uiPanel.width;
			Height = uiPanel.height;
			Depth = uiPanel.depth;
			SortingOrder = uiPanel.sortingOrder;
			Clipping = uiPanel.clipping;
			ClipOffset = uiPanel.clipOffset;
			ClipRegion = uiPanel.baseClipRegion;
			ClipSoftness = uiPanel.clipSoftness;
		}

		public override void Serialize(BinaryWriter bw)
		{
			base.Serialize(bw);

			bw.Write(Width);
			bw.Write(Height);
			bw.Write(Depth);
			bw.Write(SortingOrder);
			bw.Write((Int32)Clipping);
			bw.Write(ClipOffset);
			bw.Write(ClipRegion);
			bw.Write(ClipSoftness);
		}

		public override void Deserialize(BinaryReader br)
		{
			base.Deserialize(br);

			Width = br.ReadInt32();
			Height = br.ReadInt32();
			Depth = br.ReadInt32();
			SortingOrder = br.ReadInt32();
			Clipping = (UIDrawCall.Clipping)br.ReadInt32();
			ClipOffset = br.ReadVector2();
			ClipRegion = br.ReadVector4();
			ClipSoftness = br.ReadVector2();
		}
	}
}
