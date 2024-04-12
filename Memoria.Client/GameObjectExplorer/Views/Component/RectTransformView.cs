using Memoria.Test;
using System.ComponentModel;
using UnityEngine;

namespace Memoria.Client
{
	public class RectTransformView<T> : TransformView<T> where T : RectTransformMessage
	{
		public RectTransformView(T message, RemoteGameObjects context)
			: base(message, context)
		{
		}

		[Category(nameof(RectTransform))]
		[DisplayName(nameof(AnchorMin))]
		[Description("The normalized position in the parent RectTransform that the lower left corner is anchored to.")]
		public Vector2 AnchorMin
		{
			get { return Native.AnchorMin; }
			set
			{
				{
					Native.AnchorMin = value;
					SendPropertyChanged(nameof(RectTransform.anchorMin), new Vector2Message(value));
				}
			}
		}

		[Category(nameof(RectTransform))]
		[DisplayName(nameof(AnchorMax))]
		[Description("The normalized position in the parent RectTransform that the upper right corner is anchored to.")]
		public Vector2 AnchorMax
		{
			get { return Native.AnchorMax; }
			set
			{
				{
					Native.AnchorMax = value;
					SendPropertyChanged(nameof(RectTransform.anchorMax), new Vector2Message(value));
				}
			}
		}

		[Category(nameof(RectTransform))]
		[DisplayName(nameof(AnchoredPosition3D))]
		[Description("The 3D position of the pivot of this RectTransform relative to the anchor reference point.")]
		public Vector3 AnchoredPosition3D
		{
			get { return Native.AnchoredPosition3D; }
			set
			{
				{
					Native.AnchoredPosition3D = value;
					SendPropertyChanged(nameof(RectTransform.anchoredPosition3D), new Vector3Message(value));
				}
			}
		}

		[Category(nameof(RectTransform))]
		[DisplayName(nameof(AnchoredPosition))]
		[Description("The position of the pivot of this RectTransform relative to the anchor reference point.")]
		public Vector2 AnchoredPosition
		{
			get { return Native.AnchoredPosition; }
			set
			{
				{
					Native.AnchoredPosition = value;
					SendPropertyChanged(nameof(RectTransform.anchoredPosition), new Vector2Message(value));
				}
			}
		}

		[Category(nameof(RectTransform))]
		[DisplayName(nameof(SizeDelta))]
		[Description("The size of this RectTransform relative to the distances between the anchors.")]
		public Vector2 SizeDelta
		{
			get { return Native.SizeDelta; }
			set
			{
				{
					Native.SizeDelta = value;
					SendPropertyChanged(nameof(RectTransform.sizeDelta), new Vector2Message(value));
				}
			}
		}

		[Category(nameof(RectTransform))]
		[DisplayName(nameof(Pivot))]
		[Description("The normalized position in this RectTransform that it rotates around.")]
		public Vector2 Pivot
		{
			get { return Native.Pivot; }
			set
			{
				{
					Native.Pivot = value;
					SendPropertyChanged(nameof(RectTransform.pivot), new Vector2Message(value));
				}
			}
		}

		[Category(nameof(RectTransform))]
		[DisplayName(nameof(OffsetMin))]
		[Description("The offset of the lower left corner of the rectangle relative to the lower left anchor.")]
		public Vector2 OffsetMin
		{
			get { return Native.OffsetMin; }
			set
			{
				{
					Native.OffsetMin = value;
					SendPropertyChanged(nameof(RectTransform.offsetMin), new Vector2Message(value));
				}
			}
		}

		[Category(nameof(RectTransform))]
		[DisplayName(nameof(OffsetMax))]
		[Description("The offset of the upper right corner of the rectangle relative to the upper right anchor.")]
		public Vector2 OffsetMax
		{
			get { return Native.OffsetMax; }
			set
			{
				{
					Native.OffsetMax = value;
					SendPropertyChanged(nameof(RectTransform.offsetMax), new Vector2Message(value));
				}
			}
		}
	}
}
