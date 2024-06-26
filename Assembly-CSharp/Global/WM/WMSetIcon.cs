using System;
using UnityEngine;

public class WMSetIcon
{
    public static void SetIcon(GameObject gObj, WMSetIcon.LabelIcon icon)
    {
        if (WMSetIcon.labelIcons == null)
        {
            WMSetIcon.labelIcons = WMSetIcon.GetTextures("sv_label_", String.Empty, 0, 8);
        }
        WMSetIcon.SetIcon(gObj, WMSetIcon.labelIcons[(Int32)icon].image as Texture2D);
    }

    public static void SetIcon(GameObject gObj, WMSetIcon.Icon icon)
    {
        if (WMSetIcon.largeIcons == null)
        {
            WMSetIcon.largeIcons = WMSetIcon.GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);
        }
        WMSetIcon.SetIcon(gObj, WMSetIcon.largeIcons[(Int32)icon].image as Texture2D);
    }

    private static void SetIcon(GameObject gObj, Texture2D texture)
    {
    }

    private static GUIContent[] GetTextures(String baseName, String postFix, Int32 startIndex, Int32 count)
    {
        return new GUIContent[count];
    }

    private static GUIContent[] labelIcons;

    private static GUIContent[] largeIcons;

    public enum LabelIcon
    {
        Gray,
        Blue,
        Teal,
        Green,
        Yellow,
        Orange,
        Red,
        Purple
    }

    public enum Icon
    {
        CircleGray,
        CircleBlue,
        CircleTeal,
        CircleGreen,
        CircleYellow,
        CircleOrange,
        CircleRed,
        CirclePurple,
        DiamondGray,
        DiamondBlue,
        DiamondTeal,
        DiamondGreen,
        DiamondYellow,
        DiamondOrange,
        DiamondRed,
        DiamondPurple
    }
}
