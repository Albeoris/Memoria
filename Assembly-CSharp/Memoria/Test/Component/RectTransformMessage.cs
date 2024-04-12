using System.IO;
using UnityEngine;

namespace Memoria.Test
{
	public class RectTransformMessage : TransformMessage
	{
		public Vector2 AnchorMin;
		public Vector2 AnchorMax;
		public Vector3 AnchoredPosition3D;
		public Vector2 AnchoredPosition;
		public Vector2 SizeDelta;
		public Vector2 Pivot;
		public Vector2 OffsetMin;
		public Vector2 OffsetMax;

		public RectTransformMessage()
		{
		}

		public RectTransformMessage(RectTransform rectTransform)
			: base(rectTransform)
		{
			AnchorMin = rectTransform.anchorMin;
			AnchorMax = rectTransform.anchorMax;
			AnchoredPosition3D = rectTransform.anchoredPosition3D;
			AnchoredPosition = rectTransform.anchoredPosition;
			SizeDelta = rectTransform.sizeDelta;
			Pivot = rectTransform.pivot;
			OffsetMin = rectTransform.offsetMin;
			OffsetMax = rectTransform.offsetMax;
		}

		public override void Serialize(BinaryWriter bw)
		{
			base.Serialize(bw);

			bw.Write(AnchorMin);
			bw.Write(AnchorMax);
			bw.Write(AnchoredPosition3D);
			bw.Write(AnchoredPosition);
			bw.Write(SizeDelta);
			bw.Write(Pivot);
			bw.Write(OffsetMin);
			bw.Write(OffsetMax);
		}

		public override void Deserialize(BinaryReader br)
		{
			base.Deserialize(br);

			AnchorMin = br.ReadVector2();
			AnchorMax = br.ReadVector2();
			AnchoredPosition3D = br.ReadVector3();
			AnchoredPosition = br.ReadVector2();
			SizeDelta = br.ReadVector2();
			Pivot = br.ReadVector2();
			OffsetMin = br.ReadVector2();
			OffsetMax = br.ReadVector2();
		}
	}
}
