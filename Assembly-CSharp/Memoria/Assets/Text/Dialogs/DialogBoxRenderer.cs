using System;
using Assets.Sources.Scripts.UI.Common;
using UnityEngine;

namespace Memoria.Assets
{
    public class DialogBoxRenderer
    {
        public static Boolean ProcessMemoriaTag(Char[] toCharArray, ref Int32 index, BetterList<Color> colors, ref Boolean highShadow, ref Boolean center, ref Boolean justified, ref Int32 ff9Signal, ref Vector3 extraOffset, ref Single tabX, ref Dialog.DialogImage insertImage)
        {
            Int32 num = index;
            Int32 left = toCharArray.Length - index;
            FFIXTextTag tag = FFIXTextTag.TryRead(toCharArray, ref num, ref left);
            if (tag == null)
                return false;

            switch (tag.Code)
            {
                case FFIXTextTagCode.Justified:
                    justified = true;
                    break;
                case FFIXTextTagCode.Center:
                    center = true;
                    break;
                case FFIXTextTagCode.Signal:
                    ff9Signal = 10 + tag.Param[0];
                    break;
                case FFIXTextTagCode.IncreaseSignal:
                    ff9Signal = 2;
                    break;
                case FFIXTextTagCode.IncreaseSignalEx:
                    ff9Signal = 2;
                    break;
                case FFIXTextTagCode.DialogF:
                    extraOffset.x += (tag.Param[0] >= FieldMap.HalfFieldWidth) ? 0f : (tag.Param[0] * UIManager.ResourceXMultipier);
                    break;
                case FFIXTextTagCode.DialogY:
                    extraOffset.y -= tag.Param[0] * UIManager.ResourceYMultipier;
                    break;
                case FFIXTextTagCode.DialogX:
                    if (tag.Param[0] == 224)
                        tag.Param[0] = 0;
                    tabX = tag.Param[0] * UIManager.ResourceYMultipier;
                    break;
                case FFIXTextTagCode.Up:
                    OnButton(out insertImage, index, false, "DBTN", "UP");
                    break;
                case FFIXTextTagCode.Down:
                    OnButton(out insertImage, index, false, "DBTN", "DOWN");
                    break;
                case FFIXTextTagCode.Left:
                    OnButton(out insertImage, index, false, "DBTN", "LEFT");
                    break;
                case FFIXTextTagCode.Right:
                    OnButton(out insertImage, index, false, "DBTN", "RIGHT");
                    break;
                case FFIXTextTagCode.Circle:
                    OnButton(out insertImage, index, false, "DBTN", "CIRCLE");
                    break;
                case FFIXTextTagCode.Cross:
                    OnButton(out insertImage, index, false, "DBTN", "CROSS");
                    break;
                case FFIXTextTagCode.Triangle:
                    OnButton(out insertImage, index, false, "DBTN", "TRIANGLE");
                    break;
                case FFIXTextTagCode.Square:
                    OnButton(out insertImage, index, false, "DBTN", "SQUARE");
                    break;
                case FFIXTextTagCode.R1:
                    OnButton(out insertImage, index, false, "DBTN", "R1");
                    break;
                case FFIXTextTagCode.R2:
                    OnButton(out insertImage, index, false, "DBTN", "R2");
                    break;
                case FFIXTextTagCode.L1:
                    OnButton(out insertImage, index, false, "DBTN", "L1");
                    break;
                case FFIXTextTagCode.L2:
                    OnButton(out insertImage, index, false, "DBTN", "L2");
                    break;
                case FFIXTextTagCode.Select:
                    OnButton(out insertImage, index, false, "DBTN", "SELECT");
                    break;
                case FFIXTextTagCode.Start:
                    OnButton(out insertImage, index, false, "DBTN", "START");
                    break;
                case FFIXTextTagCode.Pad:
                    OnButton(out insertImage, index, false, "DBTN", "PAD");
                    break;
                case FFIXTextTagCode.UpEx:
                    OnButton(out insertImage, index, true, "CBTN", "UP");
                    break;
                case FFIXTextTagCode.DownEx:
                    OnButton(out insertImage, index, true, "CBTN", "DOWN");
                    break;
                case FFIXTextTagCode.LeftEx:
                    OnButton(out insertImage, index, true, "CBTN", "LEFT");
                    break;
                case FFIXTextTagCode.RightEx:
                    OnButton(out insertImage, index, true, "CBTN", "RIGHT");
                    break;
                case FFIXTextTagCode.CircleEx:
                    OnButton(out insertImage, index, true, "CBTN", "CIRCLE");
                    break;
                case FFIXTextTagCode.CrossEx:
                    OnButton(out insertImage, index, true, "CBTN", "CROSS");
                    break;
                case FFIXTextTagCode.TriangleEx:
                    OnButton(out insertImage, index, true, "CBTN", "TRIANGLE");
                    break;
                case FFIXTextTagCode.SquareEx:
                    OnButton(out insertImage, index, true, "CBTN", "SQUARE");
                    break;
                case FFIXTextTagCode.R1Ex:
                    OnButton(out insertImage, index, true, "CBTN", "R1");
                    break;
                case FFIXTextTagCode.R2Ex:
                    OnButton(out insertImage, index, true, "CBTN", "R2");
                    break;
                case FFIXTextTagCode.L1Ex:
                    OnButton(out insertImage, index, true, "CBTN", "L1");
                    break;
                case FFIXTextTagCode.L2Ex:
                    OnButton(out insertImage, index, true, "CBTN", "L2");
                    break;
                case FFIXTextTagCode.SelectEx:
                    OnButton(out insertImage, index, true, "CBTN", "SELECT");
                    break;
                case FFIXTextTagCode.StartEx:
                    OnButton(out insertImage, index, true, "CBTN", "START");
                    break;
                case FFIXTextTagCode.PadEx:
                    OnButton(out insertImage, index, true, "CBTN", "PAD");
                    break;
                case FFIXTextTagCode.Icon:
                    OnIcon(index, out insertImage, tag.Param[0]);
                    break;
                case FFIXTextTagCode.IconEx:
                    OnIconEx(index, out insertImage);
                    break;
                case FFIXTextTagCode.Mobile:
                    OnMobileIcon(index, ref insertImage, tag.Param[0]);
                    break;
                case FFIXTextTagCode.Tab:
                    extraOffset.x += (18 - 4f) * UIManager.ResourceXMultipier;
                    break;
                case FFIXTextTagCode.White:
                    OnColor(colors, "C8C8C8");
                    highShadow = true;
                    break;
                case FFIXTextTagCode.Pink:
                    OnColor(colors, "B880E0");
                    highShadow = true;
                    break;
                case FFIXTextTagCode.Cyan:
                    OnColor(colors, "68C0D8");
                    highShadow = true;
                    break;
                case FFIXTextTagCode.Brown:
                    OnColor(colors, "D06050");
                    highShadow = true;
                    break;
                case FFIXTextTagCode.Yellow:
                    OnColor(colors, "C8B040");
                    highShadow = true;
                    break;
                case FFIXTextTagCode.Green:
                    OnColor(colors, "78C840");
                    highShadow = true;
                    break;
                case FFIXTextTagCode.Grey:
                    OnColor(colors, "909090");
                    highShadow = true;
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
                    index = num;
                    return true;

                default:
                    return false;
            }

            index = num;
            return true;
        }

        private static void OnColor(BetterList<Color> colors, String colorText)
        {
            Color color = NGUIText.ParseColor24(colorText, 0);
            if (colors != null)
            {
                color.a = colors[colors.size - 1].a;
                if (NGUIText.premultiply && color.a != 1f)
                {
                    color = Color.Lerp(NGUIText.mInvisible, color, color.a);
                }
                colors.Add(color);
            }
        }

        private static void PhraseRenderOpcodeSymbol(Char[] text, Int32 index, ref Int32 closingBracket, String tag, ref Boolean highShadow, ref Boolean center, ref Int32 ff9Signal, ref Vector3 extraOffset, ref Single tabX, ref Dialog.DialogImage insertImage)
        {
            if (tag == NGUIText.Center)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                center = true;
            }
            else if (tag == NGUIText.Shadow)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                highShadow = true;
            }
            else if (tag == NGUIText.NoShadow)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                highShadow = false;
            }
            else if (tag == NGUIText.Signal)
            {
                Int32 oneParameterFromTag = NGUIText.GetOneParameterFromTag(text, index, ref closingBracket);
                ff9Signal = 10 + oneParameterFromTag;
            }
            else if (tag == NGUIText.IncreaseSignal)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                ff9Signal = 2;
            }
            else if (tag == NGUIText.MessageFeed)
            {
                Single[] allParameters = NGUIText.GetAllParameters(text, index, ref closingBracket);
                extraOffset.x += ((allParameters[0] >= FieldMap.HalfFieldWidth) ? 0f : (allParameters[0] * UIManager.ResourceXMultipier));
            }
            else if (tag == NGUIText.YSubOffset)
            {
                Single[] allParameters2 = NGUIText.GetAllParameters(text, index, ref closingBracket);
                extraOffset.y += allParameters2[0] * UIManager.ResourceYMultipier;
            }
            else if (tag == NGUIText.YAddOffset)
            {
                Single[] allParameters3 = NGUIText.GetAllParameters(text, index, ref closingBracket);
                extraOffset.y -= allParameters3[0] * UIManager.ResourceYMultipier;
            }
            else if (tag == NGUIText.MessageTab)
            {
                Single[] allParameters4 = NGUIText.GetAllParameters(text, index, ref closingBracket);
                if (allParameters4[0] == 224f)
                {
                    allParameters4[0] = 0f;
                }
                tabX = allParameters4[0] * UIManager.ResourceYMultipier;
            }
            else if (tag == NGUIText.CustomButtonIcon || tag == NGUIText.ButtonIcon || tag == NGUIText.JoyStickButtonIcon || tag == NGUIText.KeyboardButtonIcon)
            {
                closingBracket = Array.IndexOf(text, ']', index + 4);
                String parameterStr = new String(text, index + 6, closingBracket - index - 6);
                Boolean checkConfig = tag == NGUIText.CustomButtonIcon || tag == NGUIText.JoyStickButtonIcon;
                OnButton(out insertImage, index, checkConfig, tag, parameterStr);
            }
            else if (tag == NGUIText.IconVar)
            {
                Int32 oneParameterFromTag2 = NGUIText.GetOneParameterFromTag(text, index, ref closingBracket);
                OnIcon(index, out insertImage, oneParameterFromTag2);
            }
            else if (tag == NGUIText.NewIcon)
            {
                Int32 oneParameterFromTag3 = NGUIText.GetOneParameterFromTag(text, index, ref closingBracket);
                OnIconEx(index, out insertImage);
            }
            else if (tag == NGUIText.MobileIcon)
            {
                Int32 oneParameterFromTag4 = NGUIText.GetOneParameterFromTag(text, index, ref closingBracket);
                OnMobileIcon(index, ref insertImage, oneParameterFromTag4);
            }
            else if (tag == NGUIText.TextOffset)
            {
                Single[] allParameters5 = NGUIText.GetAllParameters(text, index, ref closingBracket);
                extraOffset.x += (allParameters5[0] - 4f) * UIManager.ResourceXMultipier;
                extraOffset.y -= allParameters5[1] * UIManager.ResourceYMultipier;
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

        public static Boolean ProcessOriginalTag(Char[] text, ref Int32 index, ref Boolean highShadow, ref Boolean center, ref Int32 ff9Signal, ref Vector3 extraOffset, ref Single tabX, ref Dialog.DialogImage insertImage)
        {
            if (index + 6 > text.Length || text[index] != '[')
                return false;

            Int32 num = index;
            String a = new String(text, index, 5);
            String[] renderOpcodeSymbols = NGUIText.RenderOpcodeSymbols;
            for (Int32 i = 0; i < (Int32)renderOpcodeSymbols.Length; i++)
            {
                String text2 = renderOpcodeSymbols[i];
                if (a == "[" + text2)
                {
                    PhraseRenderOpcodeSymbol(text, index, ref num, text2, ref highShadow, ref center, ref ff9Signal, ref extraOffset, ref tabX, ref insertImage);
                    break;
                }
            }
            if (num == index)
            {
                return false;
            }
            if (num != -1)
            {
                index = num + 1;
                return true;
            }
            index = text.Length;
            return true;
        }
    }
}