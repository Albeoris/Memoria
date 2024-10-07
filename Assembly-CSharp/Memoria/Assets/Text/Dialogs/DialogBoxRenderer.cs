using Assets.Sources.Scripts.UI.Common;
using System;
using UnityEngine;

namespace Memoria.Assets
{
    public class DialogBoxRenderer
    {
        public static Boolean ProcessMemoriaTag(Char[] toCharArray, ref Int32 index, FFIXTextModifier modifiers)
        {
            Int32 afterTag = index;
            Int32 left = toCharArray.Length - index;
            FFIXTextTag tag = FFIXTextTag.TryRead(toCharArray, ref afterTag, ref left);
            if (tag == null)
                return false;

            switch (tag.Code)
            {
                case FFIXTextTagCode.Justified:
                    modifiers.justified = true;
                    break;
                case FFIXTextTagCode.Center:
                    modifiers.center = true;
                    break;
                case FFIXTextTagCode.Signal:
                    modifiers.ff9Signal = 10 + tag.Param[0];
                    break;
                case FFIXTextTagCode.IncreaseSignal:
                    modifiers.ff9Signal = 2;
                    break;
                case FFIXTextTagCode.IncreaseSignalEx:
                    modifiers.ff9Signal = 2;
                    break;
                case FFIXTextTagCode.DialogF:
                    modifiers.extraOffset.x += (tag.Param[0] >= FieldMap.HalfFieldWidth) ? 0f : (tag.Param[0] * UIManager.ResourceXMultipier);
                    break;
                case FFIXTextTagCode.DialogY:
                    modifiers.extraOffset.y -= tag.Param[0] * UIManager.ResourceYMultipier;
                    break;
                case FFIXTextTagCode.DialogX:
                    if (tag.Param[0] == 224)
                        tag.Param[0] = 0;
                    modifiers.tabX = tag.Param[0] * UIManager.ResourceYMultipier;
                    break;
                case FFIXTextTagCode.Up:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "UP");
                    break;
                case FFIXTextTagCode.Down:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "DOWN");
                    break;
                case FFIXTextTagCode.Left:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "LEFT");
                    break;
                case FFIXTextTagCode.Right:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "RIGHT");
                    break;
                case FFIXTextTagCode.Circle:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "CIRCLE");
                    break;
                case FFIXTextTagCode.Cross:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "CROSS");
                    break;
                case FFIXTextTagCode.Triangle:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "TRIANGLE");
                    break;
                case FFIXTextTagCode.Square:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "SQUARE");
                    break;
                case FFIXTextTagCode.R1:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "R1");
                    break;
                case FFIXTextTagCode.R2:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "R2");
                    break;
                case FFIXTextTagCode.L1:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "L1");
                    break;
                case FFIXTextTagCode.L2:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "L2");
                    break;
                case FFIXTextTagCode.Select:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "SELECT");
                    break;
                case FFIXTextTagCode.Start:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "START");
                    break;
                case FFIXTextTagCode.Pad:
                    OnButton(out modifiers.insertImage, index, false, "DBTN", "PAD");
                    break;
                case FFIXTextTagCode.UpEx:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "UP");
                    break;
                case FFIXTextTagCode.DownEx:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "DOWN");
                    break;
                case FFIXTextTagCode.LeftEx:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "LEFT");
                    break;
                case FFIXTextTagCode.RightEx:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "RIGHT");
                    break;
                case FFIXTextTagCode.CircleEx:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "CIRCLE");
                    break;
                case FFIXTextTagCode.CrossEx:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "CROSS");
                    break;
                case FFIXTextTagCode.TriangleEx:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "TRIANGLE");
                    break;
                case FFIXTextTagCode.SquareEx:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "SQUARE");
                    break;
                case FFIXTextTagCode.R1Ex:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "R1");
                    break;
                case FFIXTextTagCode.R2Ex:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "R2");
                    break;
                case FFIXTextTagCode.L1Ex:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "L1");
                    break;
                case FFIXTextTagCode.L2Ex:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "L2");
                    break;
                case FFIXTextTagCode.SelectEx:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "SELECT");
                    break;
                case FFIXTextTagCode.StartEx:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "START");
                    break;
                case FFIXTextTagCode.PadEx:
                    OnButton(out modifiers.insertImage, index, true, "CBTN", "PAD");
                    break;
                case FFIXTextTagCode.Icon:
                    OnIcon(index, out modifiers.insertImage, tag.Param[0]);
                    break;
                case FFIXTextTagCode.IconEx:
                    OnIconEx(index, out modifiers.insertImage);
                    break;
                case FFIXTextTagCode.Mobile:
                    OnMobileIcon(index, ref modifiers.insertImage, tag.Param[0]);
                    break;
                case FFIXTextTagCode.Tab:
                    modifiers.extraOffset.x += (18 - 4f) * UIManager.ResourceXMultipier;
                    break;
                case FFIXTextTagCode.White:
                    OnColor(modifiers.colors, "C8C8C8");
                    modifiers.highShadow = true;
                    break;
                case FFIXTextTagCode.Pink:
                    OnColor(modifiers.colors, "B880E0");
                    modifiers.highShadow = true;
                    break;
                case FFIXTextTagCode.Cyan:
                    OnColor(modifiers.colors, "68C0D8");
                    modifiers.highShadow = true;
                    break;
                case FFIXTextTagCode.Brown:
                    OnColor(modifiers.colors, "D06050");
                    modifiers.highShadow = true;
                    break;
                case FFIXTextTagCode.Yellow:
                    OnColor(modifiers.colors, "C8B040");
                    modifiers.highShadow = true;
                    break;
                case FFIXTextTagCode.Green:
                    OnColor(modifiers.colors, "78C840");
                    modifiers.highShadow = true;
                    break;
                case FFIXTextTagCode.Grey:
                    OnColor(modifiers.colors, "909090");
                    modifiers.highShadow = true;
                    break;
                case FFIXTextTagCode.DialogSize:
                case FFIXTextTagCode.Choice:
                case FFIXTextTagCode.Time:
                case FFIXTextTagCode.Flash:
                case FFIXTextTagCode.NoAnimation:
                case FFIXTextTagCode.Instantly:
                case FFIXTextTagCode.Speed:
                case FFIXTextTagCode.Zidane:
                case FFIXTextTagCode.Vivi:
                case FFIXTextTagCode.Dagger:
                case FFIXTextTagCode.Steiner:
                case FFIXTextTagCode.Freya:
                case FFIXTextTagCode.Fraya:
                case FFIXTextTagCode.Quina:
                case FFIXTextTagCode.Eiko:
                case FFIXTextTagCode.Amarant:
                case FFIXTextTagCode.Party:
                case FFIXTextTagCode.NoFocus:
                case FFIXTextTagCode.End:
                case FFIXTextTagCode.Text:
                case FFIXTextTagCode.Item:
                case FFIXTextTagCode.Variable:
                case FFIXTextTagCode.Wait:
                case FFIXTextTagCode.PreChoose:
                case FFIXTextTagCode.PreChooseMask:
                case FFIXTextTagCode.Position:
                case FFIXTextTagCode.Offset:
                case FFIXTextTagCode.LowerRight:
                case FFIXTextTagCode.LowerLeft:
                case FFIXTextTagCode.UpperRight:
                case FFIXTextTagCode.UpperLeft:
                case FFIXTextTagCode.LowerCenter:
                case FFIXTextTagCode.UpperCenter:
                case FFIXTextTagCode.LowerRightForce:
                case FFIXTextTagCode.LowerLeftForce:
                case FFIXTextTagCode.UpperRightForce:
                case FFIXTextTagCode.UpperLeftForce:
                case FFIXTextTagCode.DialogPosition:
                case FFIXTextTagCode.Table:
                case FFIXTextTagCode.Widths:
                case FFIXTextTagCode.NewPage:
                    index = afterTag;
                    return true;

                default:
                    return false;
            }

            index = afterTag;
            return true;
        }

        private static void OnColor(BetterList<Color> colors, String colorText)
        {
            Color color = NGUIText.ParseColor24(colorText, 0);
            if (colors != null)
            {
                color.a = colors[colors.size - 1].a;
                if (NGUIText.premultiply && color.a != 1f)
                    color = Color.Lerp(NGUIText.mInvisible, color, color.a);
                colors.Add(color);
            }
        }

        private static void PhraseRenderOpcodeSymbol(Char[] text, Int32 index, ref Int32 closingBracket, String tag, FFIXTextModifier modifiers)
        {
            if (tag == NGUIText.Center)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                modifiers.center = true;
            }
            else if (tag == NGUIText.Shadow)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                modifiers.highShadow = !modifiers.highShadow; // The tag seems to be used as a toggle (see issue #397)
            }
            else if (tag == NGUIText.NoShadow)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                modifiers.highShadow = false;
            }
            else if (tag == NGUIText.Signal)
            {
                Int32 absoluteSignal = NGUIText.GetOneParameterFromTag(text, index, ref closingBracket);
                modifiers.ff9Signal = 10 + absoluteSignal;
            }
            else if (tag == NGUIText.IncreaseSignal)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                modifiers.ff9Signal = 2;
            }
            else if (tag == NGUIText.MessageFeed)
            {
                Single[] offX = NGUIText.GetAllParameters(text, index, ref closingBracket);
                modifiers.extraOffset.x += offX[0] >= FieldMap.HalfFieldWidth ? 0f : offX[0] * UIManager.ResourceXMultipier;
            }
            else if (tag == NGUIText.YSubOffset)
            {
                Single[] offY = NGUIText.GetAllParameters(text, index, ref closingBracket);
                modifiers.extraOffset.y += offY[0] * UIManager.ResourceYMultipier;
            }
            else if (tag == NGUIText.YAddOffset)
            {
                Single[] offY = NGUIText.GetAllParameters(text, index, ref closingBracket);
                modifiers.extraOffset.y -= offY[0] * UIManager.ResourceYMultipier;
            }
            else if (tag == NGUIText.MessageTab)
            {
                Single[] tabCount = NGUIText.GetAllParameters(text, index, ref closingBracket);
                if (tabCount[0] == 224f)
                    tabCount[0] = 0f;
                modifiers.tabX = tabCount[0] * UIManager.ResourceYMultipier;
            }
            else if (tag == NGUIText.CustomButtonIcon || tag == NGUIText.ButtonIcon || tag == NGUIText.JoyStickButtonIcon || tag == NGUIText.KeyboardButtonIcon)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                String btnCode = new String(text, index + 6, closingBracket - index - 6);
                Boolean checkConfig = tag == NGUIText.CustomButtonIcon || tag == NGUIText.JoyStickButtonIcon;
                OnButton(out modifiers.insertImage, index, checkConfig, tag, btnCode);
            }
            else if (tag == NGUIText.IconVar)
            {
                Int32 iconID = NGUIText.GetOneParameterFromTag(text, index, ref closingBracket);
                OnIcon(index, out modifiers.insertImage, iconID);
            }
            else if (tag == NGUIText.NewIcon)
            {
                Int32 scriptID = NGUIText.GetOneParameterFromTag(text, index, ref closingBracket);
                OnIconEx(index, out modifiers.insertImage);
            }
            else if (tag == NGUIText.MobileIcon)
            {
                Int32 iconID = NGUIText.GetOneParameterFromTag(text, index, ref closingBracket);
                OnMobileIcon(index, ref modifiers.insertImage, iconID);
            }
            else if (tag == NGUIText.TextOffset)
            {
                Single[] offXY = NGUIText.GetAllParameters(text, index, ref closingBracket);
                modifiers.extraOffset.x += (offXY[0] - 4f) * UIManager.ResourceXMultipier;
                modifiers.extraOffset.y -= offXY[1] * UIManager.ResourceYMultipier;
            }
            else if (tag == NGUIText.IconSprite)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                String spriteCode = new String(text, index + 6, closingBracket - index - 6);
                modifiers.insertImage = NGUIText.CreateSpriteImage(spriteCode);
                modifiers.insertImage.TextPosition = index;
            }
            else if (tag == NGUIText.Justified)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                modifiers.justified = true;
            }
            else if (tag == NGUIText.Mirrored)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                modifiers.mirror = !modifiers.mirror;
            }
            else
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
            }
        }

        private static void OnMobileIcon(Int32 index, ref Dialog.DialogImage insertImage, Int32 oneParameterFromTag4)
        {
            if (FF9StateSystem.MobilePlatform)
            {
                insertImage = NGUIText.CreateIconImage(oneParameterFromTag4);
                insertImage.TextPosition = index;
            }
        }

        private static void OnIconEx(Int32 index, out Dialog.DialogImage insertImage)
        {
            insertImage = NGUIText.CreateIconImage(FF9UIDataTool.NewIconId);
            insertImage.TextPosition = index;
        }

        private static void OnIcon(Int32 index, out Dialog.DialogImage insertImage, Int32 oneParameterFromTag2)
        {
            insertImage = NGUIText.CreateIconImage(oneParameterFromTag2);
            insertImage.TextPosition = index;
        }

        private static void OnButton(out Dialog.DialogImage insertImage, Int32 index, Boolean checkConfig, String tag, String parameterStr)
        {
            insertImage = NGUIText.CreateButtonImage(parameterStr, checkConfig, tag);
            insertImage.TextPosition = index;
        }

        public static Boolean ProcessOriginalTag(Char[] text, ref Int32 index, FFIXTextModifier modifiers)
        {
            if (index + 6 > text.Length || text[index] != '[')
                return false;

            Int32 processedIndex = index;
            String tag = new String(text, index + 1, 4);
            if (NGUIText.RenderOpcodeSymbols.Contains(tag))
                PhraseRenderOpcodeSymbol(text, index, ref processedIndex, tag, modifiers);
            if (processedIndex == index)
                return false;
            if (processedIndex != -1)
            {
                index = processedIndex + 1;
                return true;
            }
            index = text.Length;
            return true;
        }
    }
}
