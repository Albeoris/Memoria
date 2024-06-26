using System;
using UnityEngine;

namespace Memoria.Assets
{
    public static class DialogBoxSymbols
    {
        public static Boolean ParseSymbol(String text, ref Int32 index, BetterList<Color> colors, Boolean premultiply, ref Int32 sub, ref Boolean bold, ref Boolean italic, ref Boolean underline, ref Boolean strike, ref Boolean ignoreColor, ref Boolean highShadow, ref Boolean center, ref Boolean justified, ref Int32 ff9Signal, ref Vector3 extraOffset, ref Single tabX, ref Dialog.DialogImage insertImage)
        {
            Int32 length = text.Length;
            if (IsMemoriaTag(text, index))
            {
                Char[] chars = text.ToCharArray();
                if (DialogBoxRenderer.ProcessMemoriaTag(chars, ref index, colors, ref highShadow, ref center, ref justified, ref ff9Signal, ref extraOffset, ref tabX, ref insertImage))
                    return true;

                return false;
            }

            if (!IsOriginalTag(text, index))
                return false;

            if (text[index + 2] == ']')
            {
                if (text[index + 1] == '-')
                {
                    if (colors != null && colors.size > 1)
                    {
                        colors.RemoveAt(colors.size - 1);
                    }
                    index += 3;
                    return true;
                }
                String text2 = text.Substring(index, 3);
                String text3 = text2;
                switch (text3)
                {
                    case "[b]":
                        bold = true;
                        index += 3;
                        return true;
                    case "[i]":
                        italic = true;
                        index += 3;
                        return true;
                    case "[u]":
                        underline = true;
                        index += 3;
                        return true;
                    case "[s]":
                        strike = true;
                        index += 3;
                        return true;
                    case "[c]":
                        ignoreColor = true;
                        index += 3;
                        return true;
                }
            }
            if (index + 4 > length)
            {
                return false;
            }
            if (text[index + 3] == ']')
            {
                String text4 = text.Substring(index, 4);
                String text3 = text4;
                switch (text3)
                {
                    case "[/b]":
                        bold = false;
                        index += 4;
                        return true;
                    case "[/i]":
                        italic = false;
                        index += 4;
                        return true;
                    case "[/u]":
                        underline = false;
                        index += 4;
                        return true;
                    case "[/s]":
                        strike = false;
                        index += 4;
                        return true;
                    case "[/c]":
                        ignoreColor = false;
                        index += 4;
                        return true;
                }
                Char ch = text[index + 1];
                Char ch2 = text[index + 2];
                if (NGUIText.IsHex(ch) && NGUIText.IsHex(ch2))
                {
                    Int32 num2 = NGUIMath.HexToDecimal(ch) << 4 | NGUIMath.HexToDecimal(ch2);
                    NGUIText.mAlpha = (Single)num2 / 255f;
                    index += 4;
                    return true;
                }
            }
            if (index + 5 > length)
            {
                return false;
            }
            if (text[index + 4] == ']')
            {
                String text5 = text.Substring(index, 5);
                switch (text5)
                {
                    case "[sub]":
                        sub = 1;
                        index += 5;
                        return true;
                    case "[sup]":
                        sub = 2;
                        index += 5;
                        return true;

                }
            }
            if (index + 6 > length)
            {
                return false;
            }
            if (DialogBoxRenderer.ProcessOriginalTag(text.ToCharArray(), ref index, ref highShadow, ref center, ref ff9Signal, ref extraOffset, ref tabX, ref insertImage))
            {
                return true;
            }
            if (text[index + 5] == ']')
            {
                String text6 = text.Substring(index, 6);
                String text3 = text6;
                switch (text3)
                {
                    case "[/sub]":
                        sub = 0;
                        index += 6;
                        return true;
                    case "[/sup]":
                        sub = 0;
                        index += 6;
                        return true;
                    case "[/url]":
                        index += 6;
                        return true;
                }
            }
            if (text[index + 1] == 'u' && text[index + 2] == 'r' && text[index + 3] == 'l' && text[index + 4] == '=')
            {
                Int32 num3 = text.IndexOf(']', index + 4);
                if (num3 != -1)
                {
                    index = num3 + 1;
                    center = true;
                    return true;
                }
                index = text.Length;
                center = false;
                return true;
            }
            if (index + 8 > length)
            {
                return false;
            }
            if (text[index + 7] == ']')
            {
                Color color = NGUIText.ParseColor24(text, index + 1);
                if (NGUIText.EncodeColor24(color) != text.Substring(index + 1, 6).ToUpper())
                {
                    return false;
                }
                if (colors != null)
                {
                    color.a = colors[colors.size - 1].a;
                    if (premultiply && color.a != 1f)
                    {
                        color = Color.Lerp(NGUIText.mInvisible, color, color.a);
                    }
                    colors.Add(color);
                }
                index += 8;
                return true;
            }

            if (index + 10 > length)
            {
                return false;
            }
            if (text[index + 9] != ']')
            {
                return false;
            }
            Color color2 = NGUIText.ParseColor32(text, index + 1);
            if (NGUIText.EncodeColor32(color2) != text.Substring(index + 1, 8).ToUpper())
            {
                return false;
            }
            if (colors != null)
            {
                if (premultiply && color2.a != 1f)
                {
                    color2 = Color.Lerp(NGUIText.mInvisible, color2, color2.a);
                }
                colors.Add(color2);
            }
            index += 10;
            return true;
        }

        private static Boolean IsOriginalTag(String text, Int32 index)
        {
            return index + 3 <= text.Length && text[index] == '[';
        }

        private static Boolean IsMemoriaTag(String text, Int32 index)
        {
            return index + 3 <= text.Length && text[index] == '{';
        }
    }
}