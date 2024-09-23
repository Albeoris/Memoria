using System;
using UnityEngine;

namespace Memoria.Assets
{
    public static class DialogBoxSymbols
    {
        public static Boolean ParseSymbol(String text, ref Int32 index, Boolean premultiplyColor, FFIXTextModifier modifiers)
        {
            Int32 textLength = text.Length;
            if (IsMemoriaTag(text, index))
            {
                Char[] chars = text.ToCharArray();
                if (DialogBoxRenderer.ProcessMemoriaTag(chars, ref index, modifiers))
                    return true;

                return false;
            }

            if (!IsOriginalTag(text, index))
                return false;

            if (text[index + 2] == ']')
            {
                if (text[index + 1] == '-')
                {
                    // [-] used by color encoding (eg. NGUIText.EncodeColor)
                    if (modifiers.colors != null && modifiers.colors.size > 1)
                        modifiers.colors.RemoveAt(modifiers.colors.size - 1);
                    index += 3;
                    return true;
                }
                switch (text.Substring(index, 3))
                {
                    // Short tags
                    case "[b]":
                        modifiers.bold = true;
                        index += 3;
                        return true;
                    case "[i]":
                        modifiers.italic = true;
                        index += 3;
                        return true;
                    case "[u]":
                        modifiers.underline = true;
                        index += 3;
                        return true;
                    case "[s]":
                        modifiers.strike = true;
                        index += 3;
                        return true;
                    case "[c]":
                        modifiers.ignoreColor = true;
                        index += 3;
                        return true;
                }
            }
            if (index + 4 > textLength)
                return false;
            if (text[index + 3] == ']')
            {
                // Short tag ends
                switch (text.Substring(index, 4))
                {
                    case "[/b]":
                        modifiers.bold = false;
                        index += 4;
                        return true;
                    case "[/i]":
                        modifiers.italic = false;
                        index += 4;
                        return true;
                    case "[/u]":
                        modifiers.underline = false;
                        index += 4;
                        return true;
                    case "[/s]":
                        modifiers.strike = false;
                        index += 4;
                        return true;
                    case "[/c]":
                        modifiers.ignoreColor = false;
                        index += 4;
                        return true;
                }
                Char tagChar1 = text[index + 1];
                Char tagChar2 = text[index + 2];
                if (NGUIText.IsHex(tagChar1) && NGUIText.IsHex(tagChar2))
                {
                    // 2-byte color tag (alpha)
                    NGUIText.mAlpha = (NGUIMath.HexToDecimal(tagChar1) << 4 | NGUIMath.HexToDecimal(tagChar2)) / 255f;
                    index += 4;
                    return true;
                }
            }
            if (index + 5 > textLength)
                return false;
            if (text[index + 4] == ']')
            {
                // 3-letter tags (except "url" which has parameters)
                switch (text.Substring(index, 5))
                {
                    case "[sub]":
                        modifiers.sub = 1;
                        index += 5;
                        return true;
                    case "[sup]":
                        modifiers.sub = 2;
                        index += 5;
                        return true;

                }
            }
            if (index + 6 > textLength)
                return false;
            if (DialogBoxRenderer.ProcessOriginalTag(text.ToCharArray(), ref index, modifiers))
                return true; // Usual tags
            if (text[index + 5] == ']')
            {
                switch (text.Substring(index, 6))
                {
                    // 3-letter tag ends
                    case "[/sub]":
                        modifiers.sub = 0;
                        index += 6;
                        return true;
                    case "[/sup]":
                        modifiers.sub = 0;
                        index += 6;
                        return true;
                    case "[/url]":
                        index += 6;
                        return true;
                }
            }
            if (text[index + 1] == 'u' && text[index + 2] == 'r' && text[index + 3] == 'l' && text[index + 4] == '=')
            {
                // url tag
                Int32 urlEnd = text.IndexOf(']', index + 4);
                if (urlEnd != -1)
                {
                    index = urlEnd + 1;
                    modifiers.center = true;
                    return true;
                }
                index = text.Length;
                modifiers.center = false;
                return true;
            }
            if (index + 8 > textLength)
                return false;
            if (text[index + 7] == ']')
            {
                // 6-byte color tags (RGB)
                Color textColor = NGUIText.ParseColor24(text, index + 1);
                if (NGUIText.EncodeColor24(textColor) != text.Substring(index + 1, 6).ToUpper())
                    return false;
                if (modifiers.colors != null)
                {
                    if (modifiers.colors.size > 0)
                        textColor.a = modifiers.colors[modifiers.colors.size - 1].a;
                    if (premultiplyColor && textColor.a != 1f)
                        textColor = Color.Lerp(NGUIText.mInvisible, textColor, textColor.a);
                    modifiers.colors.Add(textColor);
                }
                index += 8;
                return true;
            }

            if (index + 10 > textLength)
                return false;
            if (text[index + 9] == ']')
            {
                // 8-byte color tags (RGBA)
                Color textColor = NGUIText.ParseColor32(text, index + 1);
                if (NGUIText.EncodeColor32(textColor) != text.Substring(index + 1, 8).ToUpper())
                    return false;
                if (modifiers.colors != null)
                {
                    if (premultiplyColor && textColor.a != 1f)
                        textColor = Color.Lerp(NGUIText.mInvisible, textColor, textColor.a);
                    modifiers.colors.Add(textColor);
                }
                index += 10;
                return true;
            }
            return false;
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
