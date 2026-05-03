using System;
using UnityEngine;

public static class WMGUIScreen
{
    public static Vector3 Scale
    {
        get
        {
            return WMGUIScreen._scale;
        }
    }

    public static Vector2 Scale2D
    {
        get
        {
            return new Vector2(WMGUIScreen._scale.x, WMGUIScreen._scale.y);
        }
    }

    public static void RotateAroundPivot(Single angle, Vector2 pivot)
    {
        Vector2 pivotPoint = pivot;
        pivotPoint.x *= WMGUIScreen._scale.x;
        pivotPoint.y *= WMGUIScreen._scale.y;
        GUIUtility.RotateAroundPivot(angle, pivotPoint);
    }

    public static void StartScale()
    {
        WMGUIScreen._scale.x = (Single)Screen.width / WMGUIScreen.Width;
        WMGUIScreen._scale.y = (Single)Screen.height / WMGUIScreen.Height;
        WMGUIScreen._scale.z = 1f;
        WMGUIScreen.savedMatrix = GUI.matrix;
        GUI.skin.button.fontSize = 30;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, WMGUIScreen.Scale);
    }

    public static void EndScale()
    {
        GUI.matrix = WMGUIScreen.savedMatrix;
    }

    public static Single Width = 1543f;

    public static Single Height = 1080f;

    private static Matrix4x4 savedMatrix;

    private static Vector3 _scale;
}
