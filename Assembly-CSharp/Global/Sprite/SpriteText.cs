﻿using System;
using UnityEngine;

public class SpriteText : MonoBehaviour
{
    public String Text
    {
        get
        {
            return _text;
        }
        set
        {
            if (alignment == TextAlignment.Left)
            {
                for (Int32 i = 0; i < (Int32)letters.Length; i++)
                {
                    if (i < value.Length)
                    {
                        letters[i].sprite = font[(Int32)value[i]];
                    }
                    else
                    {
                        letters[i].sprite = (Sprite)null;
                    }
                }
            }
            if (alignment == TextAlignment.Right)
            {
                String text = String.Empty;
                for (Int32 j = value.Length - 1; j >= 0; j--)
                {
                    text += value[j];
                }
                for (Int32 k = 0; k < (Int32)letters.Length; k++)
                {
                    if (k < text.Length)
                    {
                        letters[(Int32)letters.Length - k - 1].sprite = font[(Int32)text[k]];
                    }
                    else
                    {
                        letters[(Int32)letters.Length - k - 1].sprite = (Sprite)null;
                    }
                }
            }
            _text = value;
        }
    }

    public SpriteFont font;

    public SpriteRenderer[] letters;

    public TextAlignment alignment;

    private String _text;
}
