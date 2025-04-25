using System;
using UnityEngine;

public class DialogImage
{
    public Int32 Id;
    public Vector2 Size;
    public Single AppearStep;
    public Int32 PrintedLine;
    public Vector3 LocalPosition;
    public Vector3 Offset;
    public Boolean IsShown;
    public Boolean checkFromConfig = true;
    public Boolean IsButton = true;
    public String tag = String.Empty;

    [NonSerialized]
    public Boolean IsRegistered = false;
    [NonSerialized]
    public GameObject SpriteGo = null;
    [NonSerialized]
    public String AtlasName = null;
    [NonSerialized]
    public String SpriteName = null;
    [NonSerialized]
    public Boolean Rescale = false;
    [NonSerialized]
    public Boolean Mirror = false;
    [NonSerialized]
    public Single Alpha = 1f;

    public static Boolean CompareImages(DialogImage first, DialogImage second)
    {
        return first.Id == second.Id && first.AtlasName == second.AtlasName && first.SpriteName == second.SpriteName && first.checkFromConfig == second.checkFromConfig && first.IsButton == second.IsButton && first.tag == second.tag;
    }
}
