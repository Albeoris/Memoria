using System;
using UnityEngine;

public class SpriteFont : ScriptableObject
{
    public Sprite this[Int32 i]
    {
        get
        {
            QuadMistResourceManager instance = QuadMistResourceManager.Instance;
            return instance.GetFont(base.name, (Char)i).Sprite;
        }
    }

    public Int32 mode;

    public Sprite[] charMap = new Sprite[256];
}
