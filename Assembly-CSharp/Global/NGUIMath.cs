using System;
using System.Diagnostics;
using UnityEngine;

public static class NGUIMath
{
	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Single Lerp(Single from, Single to, Single factor)
	{
		return from * (1f - factor) + to * factor;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Int32 ClampIndex(Int32 val, Int32 max)
	{
		return (Int32)((val >= 0) ? ((Int32)((val >= max) ? (max - 1) : val)) : 0);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Int32 RepeatIndex(Int32 val, Int32 max)
	{
		if (max < 1)
		{
			return 0;
		}
		while (val < 0)
		{
			val += max;
		}
		while (val >= max)
		{
			val -= max;
		}
		return val;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Single WrapAngle(Single angle)
	{
		while (angle > 180f)
		{
			angle -= 360f;
		}
		while (angle < -180f)
		{
			angle += 360f;
		}
		return angle;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Single Wrap01(Single val)
	{
		return val - (Single)Mathf.FloorToInt(val);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Int32 HexToDecimal(Char ch)
	{
		switch (ch)
		{
			case '0':
				return 0;
			case '1':
				return 1;
			case '2':
				return 2;
			case '3':
				return 3;
			case '4':
				return 4;
			case '5':
				return 5;
			case '6':
				return 6;
			case '7':
				return 7;
			case '8':
				return 8;
			case '9':
				return 9;
			case ':':
			case ';':
			case '<':
			case '=':
			case '>':
			case '?':
			case '@':
			IL_67:
				switch (ch)
				{
					case 'a':
						break;
					case 'b':
						return 11;
					case 'c':
						return 12;
					case 'd':
						return 13;
					case 'e':
						return 14;
					case 'f':
						return 15;
					default:
						return 15;
				}
				break;
			case 'A':
				break;
			case 'B':
				return 11;
			case 'C':
				return 12;
			case 'D':
				return 13;
			case 'E':
				return 14;
			case 'F':
				return 15;
			default:
				goto IL_67;
		}
		return 10;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Char DecimalToHexChar(Int32 num)
	{
		if (num > 15)
		{
			return 'F';
		}
		if (num < 10)
		{
			return (Char)(48 + num);
		}
		return (Char)(65 + num - 10);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static String DecimalToHex8(Int32 num)
	{
		num &= 255;
		return num.ToString("X2");
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static String DecimalToHex24(Int32 num)
	{
		num &= 16777215;
		return num.ToString("X6");
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static String DecimalToHex32(Int32 num)
	{
		return num.ToString("X8");
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Int32 ColorToInt(Color c)
	{
		Int32 num = 0;
		num |= Mathf.RoundToInt(c.r * 255f) << 24;
		num |= Mathf.RoundToInt(c.g * 255f) << 16;
		num |= Mathf.RoundToInt(c.b * 255f) << 8;
		return num | Mathf.RoundToInt(c.a * 255f);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Color IntToColor(Int32 val)
	{
		Single num = 0.003921569f;
		Color black = Color.black;
		black.r = num * (Single)(val >> 24 & 255);
		black.g = num * (Single)(val >> 16 & 255);
		black.b = num * (Single)(val >> 8 & 255);
		black.a = num * (Single)(val & 255);
		return black;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static String IntToBinary(Int32 val, Int32 bits)
	{
		String text = String.Empty;
		Int32 i = bits;
		while (i > 0)
		{
			if (i == 8 || i == 16 || i == 24)
			{
				text += " ";
			}
			text += (Char)(((val & 1 << --i) == 0) ? '0' : '1');
		}
		return text;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Color HexToColor(UInt32 val)
	{
		return NGUIMath.IntToColor((Int32)val);
	}

	public static Rect ConvertToTexCoords(Rect rect, Int32 width, Int32 height)
	{
		Rect result = rect;
		if ((Single)width != 0f && (Single)height != 0f)
		{
			result.xMin = rect.xMin / (Single)width;
			result.xMax = rect.xMax / (Single)width;
			result.yMin = 1f - rect.yMax / (Single)height;
			result.yMax = 1f - rect.yMin / (Single)height;
		}
		return result;
	}

	public static Rect ConvertToPixels(Rect rect, Int32 width, Int32 height, Boolean round)
	{
		Rect result = rect;
		if (round)
		{
			result.xMin = (Single)Mathf.RoundToInt(rect.xMin * (Single)width);
			result.xMax = (Single)Mathf.RoundToInt(rect.xMax * (Single)width);
			result.yMin = (Single)Mathf.RoundToInt((1f - rect.yMax) * (Single)height);
			result.yMax = (Single)Mathf.RoundToInt((1f - rect.yMin) * (Single)height);
		}
		else
		{
			result.xMin = rect.xMin * (Single)width;
			result.xMax = rect.xMax * (Single)width;
			result.yMin = (1f - rect.yMax) * (Single)height;
			result.yMax = (1f - rect.yMin) * (Single)height;
		}
		return result;
	}

	public static Rect MakePixelPerfect(Rect rect)
	{
		rect.xMin = (Single)Mathf.RoundToInt(rect.xMin);
		rect.yMin = (Single)Mathf.RoundToInt(rect.yMin);
		rect.xMax = (Single)Mathf.RoundToInt(rect.xMax);
		rect.yMax = (Single)Mathf.RoundToInt(rect.yMax);
		return rect;
	}

	public static Rect MakePixelPerfect(Rect rect, Int32 width, Int32 height)
	{
		rect = NGUIMath.ConvertToPixels(rect, width, height, true);
		rect.xMin = (Single)Mathf.RoundToInt(rect.xMin);
		rect.yMin = (Single)Mathf.RoundToInt(rect.yMin);
		rect.xMax = (Single)Mathf.RoundToInt(rect.xMax);
		rect.yMax = (Single)Mathf.RoundToInt(rect.yMax);
		return NGUIMath.ConvertToTexCoords(rect, width, height);
	}

	public static Vector2 ConstrainRect(Vector2 minRect, Vector2 maxRect, Vector2 minArea, Vector2 maxArea)
	{
		Vector2 zero = Vector2.zero;
		Single num = maxRect.x - minRect.x;
		Single num2 = maxRect.y - minRect.y;
		Single num3 = maxArea.x - minArea.x;
		Single num4 = maxArea.y - minArea.y;
		if (num > num3)
		{
			Single num5 = num - num3;
			minArea.x -= num5;
			maxArea.x += num5;
		}
		if (num2 > num4)
		{
			Single num6 = num2 - num4;
			minArea.y -= num6;
			maxArea.y += num6;
		}
		if (minRect.x < minArea.x)
		{
			zero.x += minArea.x - minRect.x;
		}
		if (maxRect.x > maxArea.x)
		{
			zero.x -= maxRect.x - maxArea.x;
		}
		if (minRect.y < minArea.y)
		{
			zero.y += minArea.y - minRect.y;
		}
		if (maxRect.y > maxArea.y)
		{
			zero.y -= maxRect.y - maxArea.y;
		}
		return zero;
	}

	public static Bounds CalculateAbsoluteWidgetBounds(Transform trans)
	{
		if (!(trans != (UnityEngine.Object)null))
		{
			return new Bounds(Vector3.zero, Vector3.zero);
		}
		UIWidget[] componentsInChildren = trans.GetComponentsInChildren<UIWidget>();
		if (componentsInChildren.Length == 0)
		{
			return new Bounds(trans.position, Vector3.zero);
		}
		Vector3 center = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
		Vector3 point = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);
		Int32 i = 0;
		Int32 num = (Int32)componentsInChildren.Length;
		while (i < num)
		{
			UIWidget uiwidget = componentsInChildren[i];
			if (uiwidget.enabled)
			{
				Vector3[] worldCorners = uiwidget.worldCorners;
				for (Int32 j = 0; j < 4; j++)
				{
					Vector3 vector = worldCorners[j];
					if (vector.x > point.x)
					{
						point.x = vector.x;
					}
					if (vector.y > point.y)
					{
						point.y = vector.y;
					}
					if (vector.z > point.z)
					{
						point.z = vector.z;
					}
					if (vector.x < center.x)
					{
						center.x = vector.x;
					}
					if (vector.y < center.y)
					{
						center.y = vector.y;
					}
					if (vector.z < center.z)
					{
						center.z = vector.z;
					}
				}
			}
			i++;
		}
		Bounds result = new Bounds(center, Vector3.zero);
		result.Encapsulate(point);
		return result;
	}

	public static Bounds CalculateRelativeWidgetBounds(Transform trans)
	{
		return NGUIMath.CalculateRelativeWidgetBounds(trans, trans, false, true);
	}

	public static Bounds CalculateRelativeWidgetBounds(Transform trans, Boolean considerInactive)
	{
		return NGUIMath.CalculateRelativeWidgetBounds(trans, trans, considerInactive, true);
	}

	public static Bounds CalculateRelativeWidgetBounds(Transform relativeTo, Transform content)
	{
		return NGUIMath.CalculateRelativeWidgetBounds(relativeTo, content, false, true);
	}

	public static Bounds CalculateRelativeWidgetBounds(Transform relativeTo, Transform content, Boolean considerInactive, Boolean considerChildren = true)
	{
		if (content != (UnityEngine.Object)null && relativeTo != (UnityEngine.Object)null)
		{
			Boolean flag = false;
			Matrix4x4 worldToLocalMatrix = relativeTo.worldToLocalMatrix;
			Vector3 center = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
			Vector3 point = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);
			NGUIMath.CalculateRelativeWidgetBounds(content, considerInactive, true, ref worldToLocalMatrix, ref center, ref point, ref flag, considerChildren);
			if (flag)
			{
				Bounds result = new Bounds(center, Vector3.zero);
				result.Encapsulate(point);
				return result;
			}
		}
		return new Bounds(Vector3.zero, Vector3.zero);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	private static void CalculateRelativeWidgetBounds(Transform content, Boolean considerInactive, Boolean isRoot, ref Matrix4x4 toLocal, ref Vector3 vMin, ref Vector3 vMax, ref Boolean isSet, Boolean considerChildren)
	{
		if (content == (UnityEngine.Object)null)
		{
			return;
		}
		if (!considerInactive && !NGUITools.GetActive(content.gameObject))
		{
			return;
		}
		UIPanel uipanel = (!isRoot) ? content.GetComponent<UIPanel>() : ((UIPanel)null);
		if (uipanel != (UnityEngine.Object)null && !uipanel.enabled)
		{
			return;
		}
		if (uipanel != (UnityEngine.Object)null && uipanel.clipping != UIDrawCall.Clipping.None)
		{
			Vector3[] worldCorners = uipanel.worldCorners;
			for (Int32 i = 0; i < 4; i++)
			{
				Vector3 vector = toLocal.MultiplyPoint3x4(worldCorners[i]);
				if (vector.x > vMax.x)
				{
					vMax.x = vector.x;
				}
				if (vector.y > vMax.y)
				{
					vMax.y = vector.y;
				}
				if (vector.z > vMax.z)
				{
					vMax.z = vector.z;
				}
				if (vector.x < vMin.x)
				{
					vMin.x = vector.x;
				}
				if (vector.y < vMin.y)
				{
					vMin.y = vector.y;
				}
				if (vector.z < vMin.z)
				{
					vMin.z = vector.z;
				}
				isSet = true;
			}
		}
		else
		{
			UIWidget component = content.GetComponent<UIWidget>();
			if (component != (UnityEngine.Object)null && component.enabled)
			{
				Vector3[] worldCorners2 = component.worldCorners;
				for (Int32 j = 0; j < 4; j++)
				{
					Vector3 vector2 = toLocal.MultiplyPoint3x4(worldCorners2[j]);
					if (vector2.x > vMax.x)
					{
						vMax.x = vector2.x;
					}
					if (vector2.y > vMax.y)
					{
						vMax.y = vector2.y;
					}
					if (vector2.z > vMax.z)
					{
						vMax.z = vector2.z;
					}
					if (vector2.x < vMin.x)
					{
						vMin.x = vector2.x;
					}
					if (vector2.y < vMin.y)
					{
						vMin.y = vector2.y;
					}
					if (vector2.z < vMin.z)
					{
						vMin.z = vector2.z;
					}
					isSet = true;
				}
				if (!considerChildren)
				{
					return;
				}
			}
			Int32 k = 0;
			Int32 childCount = content.childCount;
			while (k < childCount)
			{
				NGUIMath.CalculateRelativeWidgetBounds(content.GetChild(k), considerInactive, false, ref toLocal, ref vMin, ref vMax, ref isSet, true);
				k++;
			}
		}
	}

	public static Vector3 SpringDampen(ref Vector3 velocity, Single strength, Single deltaTime)
	{
		if (deltaTime > 1f)
		{
			deltaTime = 1f;
		}
		Single f = 1f - strength * 0.001f;
		Int32 num = Mathf.RoundToInt(deltaTime * 1000f);
		Single num2 = Mathf.Pow(f, (Single)num);
		Vector3 a = velocity * ((num2 - 1f) / Mathf.Log(f));
		velocity *= num2;
		return a * 0.06f;
	}

	public static Vector2 SpringDampen(ref Vector2 velocity, Single strength, Single deltaTime)
	{
		if (deltaTime > 1f)
		{
			deltaTime = 1f;
		}
		Single f = 1f - strength * 0.001f;
		Int32 num = Mathf.RoundToInt(deltaTime * 1000f);
		Single num2 = Mathf.Pow(f, (Single)num);
		Vector2 a = velocity * ((num2 - 1f) / Mathf.Log(f));
		velocity *= num2;
		return a * 0.06f;
	}

	public static Single SpringLerp(Single strength, Single deltaTime)
	{
		if (deltaTime > 1f)
		{
			deltaTime = 1f;
		}
		Int32 num = Mathf.RoundToInt(deltaTime * 1000f);
		deltaTime = 0.001f * strength;
		Single num2 = 0f;
		for (Int32 i = 0; i < num; i++)
		{
			num2 = Mathf.Lerp(num2, 1f, deltaTime);
		}
		return num2;
	}

	public static Single SpringLerp(Single from, Single to, Single strength, Single deltaTime)
	{
		if (deltaTime > 1f)
		{
			deltaTime = 1f;
		}
		Int32 num = Mathf.RoundToInt(deltaTime * 1000f);
		deltaTime = 0.001f * strength;
		for (Int32 i = 0; i < num; i++)
		{
			from = Mathf.Lerp(from, to, deltaTime);
		}
		return from;
	}

	public static Vector2 SpringLerp(Vector2 from, Vector2 to, Single strength, Single deltaTime)
	{
		return Vector2.Lerp(from, to, NGUIMath.SpringLerp(strength, deltaTime));
	}

	public static Vector3 SpringLerp(Vector3 from, Vector3 to, Single strength, Single deltaTime)
	{
		return Vector3.Lerp(from, to, NGUIMath.SpringLerp(strength, deltaTime));
	}

	public static Quaternion SpringLerp(Quaternion from, Quaternion to, Single strength, Single deltaTime)
	{
		return Quaternion.Slerp(from, to, NGUIMath.SpringLerp(strength, deltaTime));
	}

	public static Single RotateTowards(Single from, Single to, Single maxAngle)
	{
		Single num = NGUIMath.WrapAngle(to - from);
		if (Mathf.Abs(num) > maxAngle)
		{
			num = maxAngle * Mathf.Sign(num);
		}
		return from + num;
	}

	private static Single DistancePointToLineSegment(Vector2 point, Vector2 a, Vector2 b)
	{
		Single sqrMagnitude = (b - a).sqrMagnitude;
		if (sqrMagnitude == 0f)
		{
			return (point - a).magnitude;
		}
		Single num = Vector2.Dot(point - a, b - a) / sqrMagnitude;
		if (num < 0f)
		{
			return (point - a).magnitude;
		}
		if (num > 1f)
		{
			return (point - b).magnitude;
		}
		Vector2 b2 = a + num * (b - a);
		return (point - b2).magnitude;
	}

	public static Single DistanceToRectangle(Vector2[] screenPoints, Vector2 mousePos)
	{
		Boolean flag = false;
		Int32 val = 4;
		for (Int32 i = 0; i < 5; i++)
		{
			Vector3 vector = screenPoints[NGUIMath.RepeatIndex(i, 4)];
			Vector3 vector2 = screenPoints[NGUIMath.RepeatIndex(val, 4)];
			if (vector.y > mousePos.y != vector2.y > mousePos.y && mousePos.x < (vector2.x - vector.x) * (mousePos.y - vector.y) / (vector2.y - vector.y) + vector.x)
			{
				flag = !flag;
			}
			val = i;
		}
		if (!flag)
		{
			Single num = -1f;
			for (Int32 j = 0; j < 4; j++)
			{
				Vector3 v = screenPoints[j];
				Vector3 v2 = screenPoints[NGUIMath.RepeatIndex(j + 1, 4)];
				Single num2 = NGUIMath.DistancePointToLineSegment(mousePos, v, v2);
				if (num2 < num || num < 0f)
				{
					num = num2;
				}
			}
			return num;
		}
		return 0f;
	}

	public static Single DistanceToRectangle(Vector3[] worldPoints, Vector2 mousePos, Camera cam)
	{
		Vector2[] array = new Vector2[4];
		for (Int32 i = 0; i < 4; i++)
		{
			array[i] = cam.WorldToScreenPoint(worldPoints[i]);
		}
		return NGUIMath.DistanceToRectangle(array, mousePos);
	}

	public static Vector2 GetPivotOffset(UIWidget.Pivot pv)
	{
		Vector2 zero = Vector2.zero;
		if (pv == UIWidget.Pivot.Top || pv == UIWidget.Pivot.Center || pv == UIWidget.Pivot.Bottom)
		{
			zero.x = 0.5f;
		}
		else if (pv == UIWidget.Pivot.TopRight || pv == UIWidget.Pivot.Right || pv == UIWidget.Pivot.BottomRight)
		{
			zero.x = 1f;
		}
		else
		{
			zero.x = 0f;
		}
		if (pv == UIWidget.Pivot.Left || pv == UIWidget.Pivot.Center || pv == UIWidget.Pivot.Right)
		{
			zero.y = 0.5f;
		}
		else if (pv == UIWidget.Pivot.TopLeft || pv == UIWidget.Pivot.Top || pv == UIWidget.Pivot.TopRight)
		{
			zero.y = 1f;
		}
		else
		{
			zero.y = 0f;
		}
		return zero;
	}

	public static UIWidget.Pivot GetPivot(Vector2 offset)
	{
		if (offset.x == 0f)
		{
			if (offset.y == 0f)
			{
				return UIWidget.Pivot.BottomLeft;
			}
			if (offset.y == 1f)
			{
				return UIWidget.Pivot.TopLeft;
			}
			return UIWidget.Pivot.Left;
		}
		else if (offset.x == 1f)
		{
			if (offset.y == 0f)
			{
				return UIWidget.Pivot.BottomRight;
			}
			if (offset.y == 1f)
			{
				return UIWidget.Pivot.TopRight;
			}
			return UIWidget.Pivot.Right;
		}
		else
		{
			if (offset.y == 0f)
			{
				return UIWidget.Pivot.Bottom;
			}
			if (offset.y == 1f)
			{
				return UIWidget.Pivot.Top;
			}
			return UIWidget.Pivot.Center;
		}
	}

	public static void MoveWidget(UIRect w, Single x, Single y)
	{
		NGUIMath.MoveRect(w, x, y);
	}

	public static void MoveRect(UIRect rect, Single x, Single y)
	{
		Int32 num = Mathf.FloorToInt(x + 0.5f);
		Int32 num2 = Mathf.FloorToInt(y + 0.5f);
		Transform cachedTransform = rect.cachedTransform;
		cachedTransform.localPosition += new Vector3((Single)num, (Single)num2);
		Int32 num3 = 0;
		if (rect.leftAnchor.target)
		{
			num3++;
			rect.leftAnchor.absolute += num;
		}
		if (rect.rightAnchor.target)
		{
			num3++;
			rect.rightAnchor.absolute += num;
		}
		if (rect.bottomAnchor.target)
		{
			num3++;
			rect.bottomAnchor.absolute += num2;
		}
		if (rect.topAnchor.target)
		{
			num3++;
			rect.topAnchor.absolute += num2;
		}
		if (num3 != 0)
		{
			rect.UpdateAnchors();
		}
	}

	public static void ResizeWidget(UIWidget w, UIWidget.Pivot pivot, Single x, Single y, Int32 minWidth, Int32 minHeight)
	{
		NGUIMath.ResizeWidget(w, pivot, x, y, 2, 2, 100000, 100000);
	}

	public static void ResizeWidget(UIWidget w, UIWidget.Pivot pivot, Single x, Single y, Int32 minWidth, Int32 minHeight, Int32 maxWidth, Int32 maxHeight)
	{
		if (pivot == UIWidget.Pivot.Center)
		{
			Int32 num = Mathf.RoundToInt(x - (Single)w.width);
			Int32 num2 = Mathf.RoundToInt(y - (Single)w.height);
			num -= (num & 1);
			num2 -= (num2 & 1);
			if ((num | num2) != 0)
			{
				num >>= 1;
				num2 >>= 1;
				NGUIMath.AdjustWidget(w, (Single)(-(Single)num), (Single)(-(Single)num2), (Single)num, (Single)num2, minWidth, minHeight);
			}
			return;
		}
		Vector3 point = new Vector3(x, y);
		point = Quaternion.Inverse(w.cachedTransform.localRotation) * point;
		switch (pivot)
		{
			case UIWidget.Pivot.TopLeft:
				NGUIMath.AdjustWidget(w, point.x, 0f, 0f, point.y, minWidth, minHeight, maxWidth, maxHeight);
				break;
			case UIWidget.Pivot.Top:
				NGUIMath.AdjustWidget(w, 0f, 0f, 0f, point.y, minWidth, minHeight, maxWidth, maxHeight);
				break;
			case UIWidget.Pivot.TopRight:
				NGUIMath.AdjustWidget(w, 0f, 0f, point.x, point.y, minWidth, minHeight, maxWidth, maxHeight);
				break;
			case UIWidget.Pivot.Left:
				NGUIMath.AdjustWidget(w, point.x, 0f, 0f, 0f, minWidth, minHeight, maxWidth, maxHeight);
				break;
			case UIWidget.Pivot.Right:
				NGUIMath.AdjustWidget(w, 0f, 0f, point.x, 0f, minWidth, minHeight, maxWidth, maxHeight);
				break;
			case UIWidget.Pivot.BottomLeft:
				NGUIMath.AdjustWidget(w, point.x, point.y, 0f, 0f, minWidth, minHeight, maxWidth, maxHeight);
				break;
			case UIWidget.Pivot.Bottom:
				NGUIMath.AdjustWidget(w, 0f, point.y, 0f, 0f, minWidth, minHeight, maxWidth, maxHeight);
				break;
			case UIWidget.Pivot.BottomRight:
				NGUIMath.AdjustWidget(w, 0f, point.y, point.x, 0f, minWidth, minHeight, maxWidth, maxHeight);
				break;
		}
	}

	public static void AdjustWidget(UIWidget w, Single left, Single bottom, Single right, Single top)
	{
		NGUIMath.AdjustWidget(w, left, bottom, right, top, 2, 2, 100000, 100000);
	}

	public static void AdjustWidget(UIWidget w, Single left, Single bottom, Single right, Single top, Int32 minWidth, Int32 minHeight)
	{
		NGUIMath.AdjustWidget(w, left, bottom, right, top, minWidth, minHeight, 100000, 100000);
	}

	public static void AdjustWidget(UIWidget w, Single left, Single bottom, Single right, Single top, Int32 minWidth, Int32 minHeight, Int32 maxWidth, Int32 maxHeight)
	{
		Vector2 pivotOffset = w.pivotOffset;
		Transform transform = w.cachedTransform;
		Quaternion localRotation = transform.localRotation;
		Int32 num = Mathf.FloorToInt(left + 0.5f);
		Int32 num2 = Mathf.FloorToInt(bottom + 0.5f);
		Int32 num3 = Mathf.FloorToInt(right + 0.5f);
		Int32 num4 = Mathf.FloorToInt(top + 0.5f);
		if (pivotOffset.x == 0.5f && (num == 0 || num3 == 0))
		{
			num = num >> 1 << 1;
			num3 = num3 >> 1 << 1;
		}
		if (pivotOffset.y == 0.5f && (num2 == 0 || num4 == 0))
		{
			num2 = num2 >> 1 << 1;
			num4 = num4 >> 1 << 1;
		}
		Vector3 vector = localRotation * new Vector3((Single)num, (Single)num4);
		Vector3 vector2 = localRotation * new Vector3((Single)num3, (Single)num4);
		Vector3 vector3 = localRotation * new Vector3((Single)num, (Single)num2);
		Vector3 vector4 = localRotation * new Vector3((Single)num3, (Single)num2);
		Vector3 vector5 = localRotation * new Vector3((Single)num, 0f);
		Vector3 vector6 = localRotation * new Vector3((Single)num3, 0f);
		Vector3 vector7 = localRotation * new Vector3(0f, (Single)num4);
		Vector3 vector8 = localRotation * new Vector3(0f, (Single)num2);
		Vector3 zero = Vector3.zero;
		if (pivotOffset.x == 0f && pivotOffset.y == 1f)
		{
			zero.x = vector.x;
			zero.y = vector.y;
		}
		else if (pivotOffset.x == 1f && pivotOffset.y == 0f)
		{
			zero.x = vector4.x;
			zero.y = vector4.y;
		}
		else if (pivotOffset.x == 0f && pivotOffset.y == 0f)
		{
			zero.x = vector3.x;
			zero.y = vector3.y;
		}
		else if (pivotOffset.x == 1f && pivotOffset.y == 1f)
		{
			zero.x = vector2.x;
			zero.y = vector2.y;
		}
		else if (pivotOffset.x == 0f && pivotOffset.y == 0.5f)
		{
			zero.x = vector5.x + (vector7.x + vector8.x) * 0.5f;
			zero.y = vector5.y + (vector7.y + vector8.y) * 0.5f;
		}
		else if (pivotOffset.x == 1f && pivotOffset.y == 0.5f)
		{
			zero.x = vector6.x + (vector7.x + vector8.x) * 0.5f;
			zero.y = vector6.y + (vector7.y + vector8.y) * 0.5f;
		}
		else if (pivotOffset.x == 0.5f && pivotOffset.y == 1f)
		{
			zero.x = vector7.x + (vector5.x + vector6.x) * 0.5f;
			zero.y = vector7.y + (vector5.y + vector6.y) * 0.5f;
		}
		else if (pivotOffset.x == 0.5f && pivotOffset.y == 0f)
		{
			zero.x = vector8.x + (vector5.x + vector6.x) * 0.5f;
			zero.y = vector8.y + (vector5.y + vector6.y) * 0.5f;
		}
		else if (pivotOffset.x == 0.5f && pivotOffset.y == 0.5f)
		{
			zero.x = (vector5.x + vector6.x + vector7.x + vector8.x) * 0.5f;
			zero.y = (vector7.y + vector8.y + vector5.y + vector6.y) * 0.5f;
		}
		minWidth = Mathf.Max(minWidth, w.minWidth);
		minHeight = Mathf.Max(minHeight, w.minHeight);
		Int32 num5 = w.width + num3 - num;
		Int32 num6 = w.height + num4 - num2;
		Vector3 zero2 = Vector3.zero;
		Int32 num7 = num5;
		if (num5 < minWidth)
		{
			num7 = minWidth;
		}
		else if (num5 > maxWidth)
		{
			num7 = maxWidth;
		}
		if (num5 != num7)
		{
			if (num != 0)
			{
				zero2.x -= Mathf.Lerp((Single)(num7 - num5), 0f, pivotOffset.x);
			}
			else
			{
				zero2.x += Mathf.Lerp(0f, (Single)(num7 - num5), pivotOffset.x);
			}
			num5 = num7;
		}
		Int32 num8 = num6;
		if (num6 < minHeight)
		{
			num8 = minHeight;
		}
		else if (num6 > maxHeight)
		{
			num8 = maxHeight;
		}
		if (num6 != num8)
		{
			if (num2 != 0)
			{
				zero2.y -= Mathf.Lerp((Single)(num8 - num6), 0f, pivotOffset.y);
			}
			else
			{
				zero2.y += Mathf.Lerp(0f, (Single)(num8 - num6), pivotOffset.y);
			}
			num6 = num8;
		}
		if (pivotOffset.x == 0.5f)
		{
			num5 = num5 >> 1 << 1;
		}
		if (pivotOffset.y == 0.5f)
		{
			num6 = num6 >> 1 << 1;
		}
		Vector3 localPosition = transform.localPosition + zero + localRotation * zero2;
		transform.localPosition = localPosition;
		w.SetDimensions(num5, num6);
		if (w.isAnchored)
		{
			transform = transform.parent;
			Single num9 = localPosition.x - pivotOffset.x * (Single)num5;
			Single num10 = localPosition.y - pivotOffset.y * (Single)num6;
			if (w.leftAnchor.target)
			{
				w.leftAnchor.SetHorizontal(transform, num9);
			}
			if (w.rightAnchor.target)
			{
				w.rightAnchor.SetHorizontal(transform, num9 + (Single)num5);
			}
			if (w.bottomAnchor.target)
			{
				w.bottomAnchor.SetVertical(transform, num10);
			}
			if (w.topAnchor.target)
			{
				w.topAnchor.SetVertical(transform, num10 + (Single)num6);
			}
		}
	}

	public static Int32 AdjustByDPI(Single height)
	{
		Single num = Screen.dpi;
		RuntimePlatform platform = Application.platform;
		if (num == 0f)
		{
			num = ((platform != RuntimePlatform.Android && platform != RuntimePlatform.IPhonePlayer) ? 96f : 160f);
		}
		Int32 num2 = Mathf.RoundToInt(height * (96f / num));
		if ((num2 & 1) == 1)
		{
			num2++;
		}
		return num2;
	}

	public static Vector2 ScreenToPixels(Vector2 pos, Transform relativeTo)
	{
		Int32 layer = relativeTo.gameObject.layer;
		Camera camera = NGUITools.FindCameraForLayer(layer);
		if (camera == (UnityEngine.Object)null)
		{
			global::Debug.LogWarning("No camera found for layer " + layer);
			return pos;
		}
		Vector3 position = camera.ScreenToWorldPoint(pos);
		return relativeTo.InverseTransformPoint(position);
	}

	public static Vector2 ScreenToParentPixels(Vector2 pos, Transform relativeTo)
	{
		Int32 layer = relativeTo.gameObject.layer;
		if (relativeTo.parent != (UnityEngine.Object)null)
		{
			relativeTo = relativeTo.parent;
		}
		Camera camera = NGUITools.FindCameraForLayer(layer);
		if (camera == (UnityEngine.Object)null)
		{
			global::Debug.LogWarning("No camera found for layer " + layer);
			return pos;
		}
		Vector3 vector = camera.ScreenToWorldPoint(pos);
		return (!(relativeTo != (UnityEngine.Object)null)) ? vector : relativeTo.InverseTransformPoint(vector);
	}

	public static Vector3 WorldToLocalPoint(Vector3 worldPos, Camera worldCam, Camera uiCam, Transform relativeTo)
	{
		worldPos = worldCam.WorldToViewportPoint(worldPos);
		worldPos = uiCam.ViewportToWorldPoint(worldPos);
		if (relativeTo == (UnityEngine.Object)null)
		{
			return worldPos;
		}
		relativeTo = relativeTo.parent;
		if (relativeTo == (UnityEngine.Object)null)
		{
			return worldPos;
		}
		return relativeTo.InverseTransformPoint(worldPos);
	}

	public static void OverlayPosition(this Transform trans, Vector3 worldPos, Camera worldCam, Camera myCam)
	{
		worldPos = worldCam.WorldToViewportPoint(worldPos);
		worldPos = myCam.ViewportToWorldPoint(worldPos);
		Transform parent = trans.parent;
		trans.localPosition = ((!(parent != (UnityEngine.Object)null)) ? worldPos : parent.InverseTransformPoint(worldPos));
	}

	public static void OverlayPosition(this Transform trans, Vector3 worldPos, Camera worldCam)
	{
		Camera camera = NGUITools.FindCameraForLayer(trans.gameObject.layer);
		if (camera != (UnityEngine.Object)null)
		{
			trans.OverlayPosition(worldPos, worldCam, camera);
		}
	}

	public static void OverlayPosition(this Transform trans, Transform target)
	{
		Camera camera = NGUITools.FindCameraForLayer(trans.gameObject.layer);
		Camera camera2 = NGUITools.FindCameraForLayer(target.gameObject.layer);
		if (camera != (UnityEngine.Object)null && camera2 != (UnityEngine.Object)null)
		{
			trans.OverlayPosition(target.position, camera2, camera);
		}
	}
}
