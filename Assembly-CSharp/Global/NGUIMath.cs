using System;
using System.Diagnostics;
using System.Collections.Generic;
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
        return val < 0 ? 0 : (val >= max ? max - 1 : val);
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static Int32 RepeatIndex(Int32 val, Int32 max)
    {
        if (max < 1)
            return 0;
        while (val < 0)
            val += max;
        while (val >= max)
            val -= max;
        return val;
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static Single WrapAngle(Single angle)
    {
        while (angle > 180f)
            angle -= 360f;
        while (angle < -180f)
            angle += 360f;
        return angle;
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static Single Wrap01(Single val)
    {
        return val - Mathf.FloorToInt(val);
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
            case 'a':
            case 'A':
                return 10;
            case 'b':
            case 'B':
                return 11;
            case 'c':
            case 'C':
                return 12;
            case 'd':
            case 'D':
                return 13;
            case 'e':
            case 'E':
                return 14;
            case 'f':
            case 'F':
                return 15;
        }
        return 15;
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static Char DecimalToHexChar(Int32 num)
    {
        if (num > 15)
            return 'F';
        if (num < 10)
            return (Char)('0' + num);
        return (Char)('A' + num - 10);
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static String DecimalToHex8(Int32 num)
    {
        num &= 0xFF;
        return num.ToString("X2");
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static String DecimalToHex24(Int32 num)
    {
        num &= 0xFFFFFF;
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
        Single factor = 0.003921569f; // 1/255
        Color col = Color.black;
        col.r = factor * (val >> 24 & 255);
        col.g = factor * (val >> 16 & 255);
        col.b = factor * (val >> 8 & 255);
        col.a = factor * (val & 255);
        return col;
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
                text += " ";
            text += ((val & 1 << --i) == 0) ? '0' : '1';
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
        if (width != 0 && height != 0)
        {
            result.xMin = rect.xMin / width;
            result.xMax = rect.xMax / width;
            result.yMin = 1f - rect.yMax / height;
            result.yMax = 1f - rect.yMin / height;
        }
        return result;
    }

    public static Rect ConvertToPixels(Rect rect, Int32 width, Int32 height, Boolean round)
    {
        Rect result = rect;
        if (round)
        {
            result.xMin = Mathf.RoundToInt(rect.xMin * width);
            result.xMax = Mathf.RoundToInt(rect.xMax * width);
            result.yMin = Mathf.RoundToInt((1f - rect.yMax) * height);
            result.yMax = Mathf.RoundToInt((1f - rect.yMin) * height);
        }
        else
        {
            result.xMin = rect.xMin * width;
            result.xMax = rect.xMax * width;
            result.yMin = (1f - rect.yMax) * height;
            result.yMax = (1f - rect.yMin) * height;
        }
        return result;
    }

    public static Rect MakePixelPerfect(Rect rect)
    {
        rect.xMin = Mathf.RoundToInt(rect.xMin);
        rect.yMin = Mathf.RoundToInt(rect.yMin);
        rect.xMax = Mathf.RoundToInt(rect.xMax);
        rect.yMax = Mathf.RoundToInt(rect.yMax);
        return rect;
    }

    public static Rect MakePixelPerfect(Rect rect, Int32 width, Int32 height)
    {
        rect = NGUIMath.ConvertToPixels(rect, width, height, true);
        rect.xMin = Mathf.RoundToInt(rect.xMin);
        rect.yMin = Mathf.RoundToInt(rect.yMin);
        rect.xMax = Mathf.RoundToInt(rect.xMax);
        rect.yMax = Mathf.RoundToInt(rect.yMax);
        return NGUIMath.ConvertToTexCoords(rect, width, height);
    }

    public static Vector2 ConstrainRect(Vector2 minRect, Vector2 maxRect, Vector2 minArea, Vector2 maxArea)
    {
        Vector2 result = Vector2.zero;
        Single rectWidth = maxRect.x - minRect.x;
        Single rectHeight = maxRect.y - minRect.y;
        Single areaWidth = maxArea.x - minArea.x;
        Single areaHeight = maxArea.y - minArea.y;
        if (rectWidth > areaWidth)
        {
            Single widthDiff = rectWidth - areaWidth;
            minArea.x -= widthDiff;
            maxArea.x += widthDiff;
        }
        if (rectHeight > areaHeight)
        {
            Single heightDiff = rectHeight - areaHeight;
            minArea.y -= heightDiff;
            maxArea.y += heightDiff;
        }
        if (minRect.x < minArea.x)
            result.x += minArea.x - minRect.x;
        if (maxRect.x > maxArea.x)
            result.x -= maxRect.x - maxArea.x;
        if (minRect.y < minArea.y)
            result.y += minArea.y - minRect.y;
        if (maxRect.y > maxArea.y)
            result.y -= maxRect.y - maxArea.y;
        return result;
    }

    public static Rect GetBoundingBox(IEnumerable<Vector3> vCollection)
    {
        Vector2 min = new Vector2(Single.MaxValue, Single.MaxValue);
        Vector2 max = new Vector2(Single.MinValue, Single.MinValue);
        foreach (Vector2 v in vCollection)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }
        if (min.x == Single.MaxValue)
            return new Rect();
        return new Rect(min, max - min);
    }

    public static Bounds CalculateAbsoluteWidgetBounds(Transform transform)
    {
        if (transform == null)
            return new Bounds(Vector3.zero, Vector3.zero);
        UIWidget[] allWidgets = transform.GetComponentsInChildren<UIWidget>();
        if (allWidgets.Length == 0)
            return new Bounds(transform.position, Vector3.zero);
        Vector3 center = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
        Vector3 point = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);
        for (Int32 i = 0; i < allWidgets.Length; i++)
        {
            UIWidget widget = allWidgets[i];
            if (widget.enabled)
            {
                Vector3[] corners = widget.worldCorners;
                for (Int32 j = 0; j < 4; j++)
                {
                    Vector3 corner = corners[j];
                    if (corner.x > point.x)
                        point.x = corner.x;
                    if (corner.y > point.y)
                        point.y = corner.y;
                    if (corner.z > point.z)
                        point.z = corner.z;
                    if (corner.x < center.x)
                        center.x = corner.x;
                    if (corner.y < center.y)
                        center.y = corner.y;
                    if (corner.z < center.z)
                        center.z = corner.z;
                }
            }
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
        if (content != null && relativeTo != null)
        {
            Boolean isSet = false;
            Matrix4x4 worldToLocalMatrix = relativeTo.worldToLocalMatrix;
            Vector3 center = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
            Vector3 point = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);
            NGUIMath.CalculateRelativeWidgetBounds(content, considerInactive, true, ref worldToLocalMatrix, ref center, ref point, ref isSet, considerChildren);
            if (isSet)
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
        if (content == null)
            return;
        if (!considerInactive && !NGUITools.GetActive(content.gameObject))
            return;
        UIPanel panel = isRoot ? null : content.GetComponent<UIPanel>();
        if (panel != null && !panel.enabled)
            return;
        if (panel != null && panel.clipping != UIDrawCall.Clipping.None)
        {
            Vector3[] worldCorners = panel.worldCorners;
            for (Int32 i = 0; i < 4; i++)
            {
                Vector3 localCorner = toLocal.MultiplyPoint3x4(worldCorners[i]);
                if (localCorner.x > vMax.x)
                    vMax.x = localCorner.x;
                if (localCorner.y > vMax.y)
                    vMax.y = localCorner.y;
                if (localCorner.z > vMax.z)
                    vMax.z = localCorner.z;
                if (localCorner.x < vMin.x)
                    vMin.x = localCorner.x;
                if (localCorner.y < vMin.y)
                    vMin.y = localCorner.y;
                if (localCorner.z < vMin.z)
                    vMin.z = localCorner.z;
                isSet = true;
            }
        }
        else
        {
            UIWidget widget = content.GetComponent<UIWidget>();
            if (widget != null && widget.enabled)
            {
                Vector3[] worldCorners = widget.worldCorners;
                for (Int32 i = 0; i < 4; i++)
                {
                    Vector3 localCorner = toLocal.MultiplyPoint3x4(worldCorners[i]);
                    if (localCorner.x > vMax.x)
                        vMax.x = localCorner.x;
                    if (localCorner.y > vMax.y)
                        vMax.y = localCorner.y;
                    if (localCorner.z > vMax.z)
                        vMax.z = localCorner.z;
                    if (localCorner.x < vMin.x)
                        vMin.x = localCorner.x;
                    if (localCorner.y < vMin.y)
                        vMin.y = localCorner.y;
                    if (localCorner.z < vMin.z)
                        vMin.z = localCorner.z;
                    isSet = true;
                }
                if (!considerChildren)
                    return;
            }
            for (Int32 i = 0; i < content.childCount; i++)
                NGUIMath.CalculateRelativeWidgetBounds(content.GetChild(i), considerInactive, false, ref toLocal, ref vMin, ref vMax, ref isSet, true);
        }
    }

    public static Vector3 SpringDampen(ref Vector3 velocity, Single strength, Single deltaTime)
    {
        if (deltaTime > 1f)
            deltaTime = 1f;
        Single f = 1f - strength * 0.001f;
        Int32 iteration = Mathf.RoundToInt(deltaTime * 1000f);
        Single shrinkFactor = Mathf.Pow(f, iteration);
        velocity *= shrinkFactor;
        return velocity * ((shrinkFactor - 1f) / Mathf.Log(f)) * 0.06f;
    }

    public static Vector2 SpringDampen(ref Vector2 velocity, Single strength, Single deltaTime)
    {
        if (deltaTime > 1f)
            deltaTime = 1f;
        Single f = 1f - strength * 0.001f;
        Int32 iteration = Mathf.RoundToInt(deltaTime * 1000f);
        Single shrinkFactor = Mathf.Pow(f, (Single)iteration);
        velocity *= shrinkFactor;
        return velocity * ((shrinkFactor - 1f) / Mathf.Log(f)) * 0.06f;
    }

    public static Single SpringLerp(Single strength, Single deltaTime)
    {
        if (deltaTime > 1f)
            deltaTime = 1f;
        Int32 iteration = Mathf.RoundToInt(deltaTime * 1000f);
        deltaTime = 0.001f * strength;
        Single result = 0f;
        for (Int32 i = 0; i < iteration; i++)
            result = Mathf.Lerp(result, 1f, deltaTime);
        return result;
    }

    public static Single SpringLerp(Single from, Single to, Single strength, Single deltaTime)
    {
        if (deltaTime > 1f)
            deltaTime = 1f;
        Int32 iteration = Mathf.RoundToInt(deltaTime * 1000f);
        deltaTime = 0.001f * strength;
        for (Int32 i = 0; i < iteration; i++)
            from = Mathf.Lerp(from, to, deltaTime);
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
        Single deltaAngle = NGUIMath.WrapAngle(to - from);
        if (Mathf.Abs(deltaAngle) > maxAngle)
            deltaAngle = maxAngle * Mathf.Sign(deltaAngle);
        return from + deltaAngle;
    }

    private static Single DistancePointToLineSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        Single sqrMagnitude = (b - a).sqrMagnitude;
        if (sqrMagnitude == 0f)
            return (point - a).magnitude;
        Single k = Vector2.Dot(point - a, b - a) / sqrMagnitude;
        if (k < 0f)
            return (point - a).magnitude;
        if (k > 1f)
            return (point - b).magnitude;
        Vector2 proj = a + k * (b - a);
        return (point - proj).magnitude;
    }

    public static Single DistanceToRectangle(Vector2[] screenPoints, Vector2 mousePos)
    {
        Boolean isInside = true;
        for (Int32 i = 0; i < 4; i++)
        {
            Int32 j = i > 0 ? i - 1 : 3;
            if (Vector3.Cross(mousePos - screenPoints[i], screenPoints[j] - screenPoints[i]).z >= 0)
                isInside = false;
        }
        if (!isInside)
        {
            Single dist = -1f;
            for (Int32 i = 0; i < 4; i++)
            {
                Int32 j = i < 3 ? i + 1 : 0;
                Single lineDist = NGUIMath.DistancePointToLineSegment(mousePos, screenPoints[i], screenPoints[j]);
                if (lineDist < dist || dist < 0f)
                    dist = lineDist;
            }
            return dist;
        }
        return 0f;
    }

    public static Single DistanceToRectangle(Vector3[] worldPoints, Vector2 mousePos, Camera cam)
    {
        Vector2[] screenPoints = new Vector2[4];
        for (Int32 i = 0; i < 4; i++)
            screenPoints[i] = cam.WorldToScreenPoint(worldPoints[i]);
        return NGUIMath.DistanceToRectangle(screenPoints, mousePos);
    }

    public static Vector2 GetPivotOffset(UIWidget.Pivot pv)
    {
        Vector2 offset = Vector2.zero;
        if (pv == UIWidget.Pivot.Top || pv == UIWidget.Pivot.Center || pv == UIWidget.Pivot.Bottom)
            offset.x = 0.5f;
        else if (pv == UIWidget.Pivot.TopRight || pv == UIWidget.Pivot.Right || pv == UIWidget.Pivot.BottomRight)
            offset.x = 1f;
        if (pv == UIWidget.Pivot.Left || pv == UIWidget.Pivot.Center || pv == UIWidget.Pivot.Right)
            offset.y = 0.5f;
        else if (pv == UIWidget.Pivot.TopLeft || pv == UIWidget.Pivot.Top || pv == UIWidget.Pivot.TopRight)
            offset.y = 1f;
        return offset;
    }

    public static UIWidget.Pivot GetPivot(Vector2 offset)
    {
        if (offset.x == 0f)
        {
            if (offset.y == 0f)
                return UIWidget.Pivot.BottomLeft;
            if (offset.y == 1f)
                return UIWidget.Pivot.TopLeft;
            return UIWidget.Pivot.Left;
        }
        else if (offset.x == 1f)
        {
            if (offset.y == 0f)
                return UIWidget.Pivot.BottomRight;
            if (offset.y == 1f)
                return UIWidget.Pivot.TopRight;
            return UIWidget.Pivot.Right;
        }
        if (offset.y == 0f)
            return UIWidget.Pivot.Bottom;
        if (offset.y == 1f)
            return UIWidget.Pivot.Top;
        return UIWidget.Pivot.Center;
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
