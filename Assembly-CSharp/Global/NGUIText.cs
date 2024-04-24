using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using Memoria.Assets;
using Memoria.Prime.Text;
using UnityEngine;
using Object = System.Object;

public static class NGUIText
{
    static NGUIText()
    {
        // Note: this type is marked as 'beforefieldinit'.
        NGUIText.RenderOpcodeSymbols = new String[]
        {
            NGUIText.StartSentense,
            NGUIText.DialogId,
            NGUIText.Choose,
            NGUIText.AnimationTime,
            NGUIText.FlashInh,
            NGUIText.NoAnimation,
            NGUIText.NoTypeEffect,
            NGUIText.MessageSpeed,
            NGUIText.Zidane,
            NGUIText.Vivi,
            NGUIText.Dagger,
            NGUIText.Steiner,
            NGUIText.Freya,
            NGUIText.Quina,
            NGUIText.Eiko,
            NGUIText.Amarant,
            NGUIText.Party1,
            NGUIText.Party2,
            NGUIText.Party3,
            NGUIText.Party4,
            NGUIText.Shadow,
            NGUIText.NoShadow,
            NGUIText.ButtonIcon,
            NGUIText.NoFocus,
            NGUIText.IncreaseSignal,
            NGUIText.CustomButtonIcon,
            NGUIText.NewIcon,
            NGUIText.TextOffset,
            NGUIText.EndSentence,
            NGUIText.TextVar,
            NGUIText.ItemNameVar,
            NGUIText.SignalVar,
            NGUIText.NumberVar,
            NGUIText.MessageDelay,
            NGUIText.MessageFeed,
            NGUIText.MessageTab,
            NGUIText.YAddOffset,
            NGUIText.YSubOffset,
            NGUIText.IconVar,
            NGUIText.PreChoose,
            NGUIText.PreChooseMask,
            NGUIText.DialogAbsPosition,
            NGUIText.DialogOffsetPositon,
            NGUIText.DialogTailPositon,
            NGUIText.TableStart,
            NGUIText.WidthInfo,
            NGUIText.Center,
            NGUIText.Signal,
            NGUIText.NewPage,
            NGUIText.MobileIcon,
            NGUIText.SpacingY,
            NGUIText.JoyStickButtonIcon,
            NGUIText.KeyboardButtonIcon
        };
        NGUIText.TextOffsetOpcodeSymbols = new String[]
        {
            NGUIText.TextOffset,
            NGUIText.MessageFeed,
            NGUIText.MessageTab
        };
        NGUIText.IconIdException = new List<Int32>
        {
            255,
            254,
            20,
            19,
            192
        };
        NGUIText.CharException = new List<Char>
        {
            ' ',
            'p',
            '-',
            'y',
            ',',
            '一'
        };
        NGUIText.nameKeywordList = new List<String>(new String[]
        {
            NGUIText.Zidane,
            NGUIText.Vivi,
            NGUIText.Dagger,
            NGUIText.Steiner,
            NGUIText.Freya,
            NGUIText.Quina,
            NGUIText.Eiko,
            NGUIText.Amarant,
            NGUIText.Party1,
            NGUIText.Party2,
            NGUIText.Party3,
            NGUIText.Party4
        });
        NGUIText.nameCustomKeywords = new Dictionary<String, CharacterId>();
        NGUIText.FF9WhiteColor = "[C8C8C8]";
        NGUIText.FF9YellowColor = "[C8B040]";
        NGUIText.FF9PinkColor = "[B880E0]";
        NGUIText.FF9BlueColor = "[2870FB]";
        NGUIText.MobileTouchToConfirmJP = 322;
        NGUIText.MobileTouchToConfirmUS = 323;
        NGUIText.forceShowButton = false;
    }

    public static void RegisterCustomNameKeywork(String keyword, CharacterId charId)
    {
        NGUIText.nameCustomKeywords[keyword] = charId;
        if (!NGUIText.nameKeywordList.Contains(keyword))
            NGUIText.nameKeywordList.Add(keyword);
    }

    public static Boolean ForceShowButton
    {
        get
        {
            return NGUIText.forceShowButton;
        }
        set
        {
            NGUIText.forceShowButton = value;
        }
    }

    public static Single GetTextWidthFromFF9Font(UILabel phraseLabel, String text)
    {
        phraseLabel.ProcessText();
        phraseLabel.UpdateNGUIText();
        NGUIText.Prepare(text);
        Single num = NGUIText.finalSpacingX;
        Single num2 = (Single)phraseLabel.fontSize * NGUIText.fontScale;
        Single num3 = 0f;
        foreach (Char ch in text)
        {
            Single glyphWidth = NGUIText.GetGlyphWidth((Int32)ch, 0);
            num3 += ((glyphWidth <= 0f) ? (num2 + num) : (glyphWidth + num));
        }
        return num3 / UIManager.ResourceXMultipier + 1f;
    }

    public static Single GetDialogWidthFromSpecialOpcode(List<Int32> specialCodeList, ETb eTb, UILabel phraseLabel)
    {
        Single num = 0f;
        FF9StateGlobal ff = FF9StateSystem.Common.FF9;
        PLAYER[] member = ff.party.member;
        for (Int32 i = 0; i < specialCodeList.Count; i++)
        {
            Int32 num2 = specialCodeList[i];
            Int32 num3 = num2;
            switch (num3)
            {
                case 6:
                {
                    UInt32 num4 = Convert.ToUInt32(specialCodeList[i + 1]);
                    if (num4 > 255u)
                    {
                        UInt32 num5 = Convert.ToUInt32(specialCodeList[i + 2]);
                        String[] tableText = FF9TextTool.GetTableText(0u);
                        String text = tableText[(Int32)((UIntPtr)num5)];
                        num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, text);
                        i += 2;
                    }
                    else
                    {
                        num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, eTb.GetStringFromTable(num4 >> 4 & 3u, num4 & 7u));
                        i++;
                    }
                    break;
                }
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 15:
                IL_A0:
                    if (num3 != 64)
                    {
                        if (num3 == 112)
                        {
                            if ((eTb.gMesValue[0] & 1 << specialCodeList[++i]) > 0)
                            {
                                num += 30f;
                            }
                        }
                    }
                    else
                    {
                        Int32 num6 = specialCodeList[i + 1];
                        Int32 num7 = eTb.gMesValue[num6];
                        num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, num7.ToString());
                        i++;
                    }
                    break;
                case 14:
                {
                    Int32 num8 = specialCodeList[i + 1];
                    String itemName = ETb.GetItemName(eTb.gMesValue[num8]);
                    num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, itemName);
                    i++;
                    break;
                }
                case 16:
                    num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Zidane).Name);
                    break;
                case 17:
                    num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Vivi).Name);
                    break;
                case 18:
                    num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Garnet).Name);
                    break;
                case 19:
                    num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Steiner).Name);
                    break;
                case 20:
                    num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Freya).Name);
                    break;
                case 21:
                    num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Quina).Name);
                    break;
                case 22:
                    num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Eiko).Name);
                    break;
                case 23:
                    num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Amarant).Name);
                    break;
                case 24:
                case 25:
                case 26:
                case 27:
                {
                    PLAYER player2 = member[num2 - 24];
                    num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, player2.Name);
                    break;
                }
                default:
                    goto IL_A0;
            }
        }
        return num;
    }

    public static void ProcessFF9Signal(ref Int32 ff9Signal, ref Int32 newSignal)
    {
        if (ff9Signal == 1)
        {
            ETb.gMesSignal = newSignal;
        }
        else if (ff9Signal == 2)
        {
            if (newSignal != ETb.gMesSignal)
            {
                ETb.gMesSignal = newSignal;
            }
            else
            {
                ETb.gMesSignal++;
            }
        }
        ff9Signal = 0;
        newSignal = 0;
    }

    public static void ProcessFF9Signal(ref Int32 ff9Signal)
    {
        if (ff9Signal >= 10)
        {
            ETb.gMesSignal = ff9Signal % 10;
        }
        else if (ff9Signal == 2)
        {
            ETb.gMesSignal++;
        }
        ff9Signal = 0;
    }

    public static Dialog.DialogImage CreateButtonImage(String parameterStr, Boolean checkConfig, String tag)
    {
        Dialog.DialogImage dialogImage = new Dialog.DialogImage();
        dialogImage.Offset = new Vector3(0f, -10f);
        Control control;
        switch (parameterStr)
        {
            case "UP":
                control = Control.Up;
                break;
            case "DOWN":
                control = Control.Down;
                break;
            case "LEFT":
                control = Control.Left;
                break;
            case "RIGHT":
                control = Control.Right;
                break;
            case "CIRCLE":
                if (checkConfig)
                    control = EventInput.IsJapaneseLayout ? Control.Confirm : Control.Cancel;
                else
                    control = Control.Confirm;
                break;
            case "CROSS":
                if (checkConfig)
                    control = EventInput.IsJapaneseLayout ? Control.Cancel : Control.Confirm;
                else
                    control = Control.Cancel;
                break;
            case "TRIANGLE":
                control = Control.Menu;
                break;
            case "SQUARE":
                control = Control.Special;
                break;
            case "R1":
                control = Control.RightBumper;
                break;
            case "R2":
                control = Control.RightTrigger;
                break;
            case "L1":
                control = Control.LeftBumper;
                break;
            case "L2":
                control = Control.LeftTrigger;
                break;
            case "SELECT":
                control = Control.Select;
                break;
            case "START":
                control = Control.Pause;
                break;
            case "PAD":
                control = Control.DPad;
                break;
            default:
                control = Control.None;
                break;
        }
        dialogImage.Size = FF9UIDataTool.GetButtonSize(control, checkConfig, tag);
        dialogImage.Id = (Int32)control;
        dialogImage.tag = tag;
        dialogImage.checkFromConfig = checkConfig;
        return dialogImage;
    }

    public static Dialog.DialogImage CreateIconImage(Int32 iconId)
    {
        Dialog.DialogImage dialogImage = new Dialog.DialogImage();
        dialogImage.Size = FF9UIDataTool.GetIconSize(iconId);
        if (iconId == 180) // text_lv_us_uk_jp_gr_it
            dialogImage.Offset = new Vector3(0f, -15.2f);
        else
            dialogImage.Offset = new Vector3(0f, -10f);
        dialogImage.Id = iconId;
        dialogImage.IsButton = false;
        return dialogImage;
    }

    public static Int32 GetOneParameterFromTag(String fullText, Int32 currentIndex, ref Int32 closingBracket)
    {
        Int32 result = 0;
        try
        {
            closingBracket = fullText.IndexOf(']', currentIndex + 4);
            String value = fullText.Substring(currentIndex + 6, closingBracket - currentIndex - 6);
            result = Convert.ToInt32(value);
        }
        catch
        {
        }
        return result;
    }

    public static Int32 GetOneParameterFromTag(Char[] fullText, Int32 currentIndex, ref Int32 closingBracket)
    {
        Int32 result = 0;
        try
        {
            closingBracket = Array.IndexOf(fullText, ']', currentIndex + 4);
            String value = new String(fullText, currentIndex + 6, closingBracket - currentIndex - 6);
            result = Convert.ToInt32(value);
        }
        catch
        {
        }
        return result;
    }

    public static Single[] GetAllParameters(String fullText, Int32 currentIndex, ref Int32 closingBracket)
    {
        closingBracket = fullText.IndexOf(']', currentIndex + 4);
        String text = fullText.Substring(currentIndex + 6, closingBracket - currentIndex - 6);
        String[] array = text.Split(new Char[]
        {
            ','
        });
        return Array.ConvertAll<String, Single>(array, new Converter<String, Single>(Single.Parse));
    }

    public static Single[] GetAllParameters(Char[] fullText, Int32 currentIndex, ref Int32 closingBracket)
    {
        closingBracket = Array.IndexOf(fullText, ']', currentIndex + 4);
        String text = new string(fullText, currentIndex + 6, closingBracket - currentIndex - 6);
        String[] array = text.Split(new Char[]
        {
            ','
        });
        return Array.ConvertAll<String, Single>(array, new Converter<String, Single>(Single.Parse));
    }

    public static String ReplaceNumberValue(String phrase, Dialog dialog)
    {
        String text = phrase;
        foreach (KeyValuePair<Int32, Int32> keyValuePair in dialog.MessageValues)
        {
            String value = keyValuePair.Value.ToString();

            text = text.ReplaceAll(
                new[]
                {
                    new KeyValuePair<String, TextReplacement>($"[NUMB={keyValuePair.Key}]", value),
                    new KeyValuePair<String, TextReplacement>($"{{Variable {keyValuePair.Key}}}", value)
                });
        }
        return text;
    }

    public static Boolean ContainsTextOffset(String text)
    {
        Int32 offset = 0;
        Int32 left = text.Length;

        FFIXTextTag tag = FFIXTextTag.TryRead(text.ToCharArray(), ref offset, ref left);
        switch (tag?.Code)
        {
            case FFIXTextTagCode.DialogX:
            case FFIXTextTagCode.DialogY:
            case FFIXTextTagCode.DialogF:
                return true;
        }

        String[] textOffsetOpcodeSymbols = NGUIText.TextOffsetOpcodeSymbols;
        for (Int32 i = 0; i < (Int32)textOffsetOpcodeSymbols.Length; i++)
        {
            String value = textOffsetOpcodeSymbols[i];
            if (text.Contains(value))
            {
                return true;
            }
        }

        return false;
    }

    public static Vector2 CalculatePrintedSize2(String text)
    {
        Vector2 zero = Vector2.zero;
        Int32 num = 0;
        Boolean flag = false;
        Boolean flag2 = false;
        Boolean flag3 = false;
        Boolean flag4 = false;
        Boolean flag5 = false;
        Boolean flag6 = false;
        Boolean flag7 = false;
        Boolean flag8 = false;
        Int32 num2 = 0;
        Vector3 zero2 = Vector3.zero;
        Dialog.DialogImage dialogImage = (Dialog.DialogImage)null;
        Single num3 = 0f;
        if (!String.IsNullOrEmpty(text))
        {
            if (NGUIText.encoding)
            {
                text = NGUIText.StripSymbols2(text);
            }
            NGUIText.Prepare(text);
            Single num4 = 0f;
            Single num5 = 0f;
            Single num6 = 0f;
            Int32 length = text.Length;
            Int32 prev = 0;
            for (Int32 i = 0; i < length; i++)
            {
                Int32 num7 = (Int32)text[i];
                if (num7 == 10)
                {
                    if (num4 > num6)
                    {
                        num6 = num4;
                    }
                    num4 = 0f;
                    num5 += NGUIText.finalLineHeight;
                    zero2 = Vector3.zero;
                    dialogImage = (Dialog.DialogImage)null;
                }
                else if (num7 >= 32)
                {
                    if (NGUIText.encoding && DialogBoxSymbols.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref num, ref flag, ref flag2, ref flag3, ref flag4, ref flag5, ref flag6, ref flag7, ref flag8, ref num2, ref zero2, ref num3, ref dialogImage))
                    {
                        i--;
                    }
                    else
                    {
                        if (num3 != 0f)
                        {
                            zero2.x = 0f;
                            num4 = num3;
                            num3 = 0f;
                        }
                        if (dialogImage != null)
                        {
                            num4 += dialogImage.Size.x - 20f;
                            dialogImage = (Dialog.DialogImage)null;
                        }
                        if (zero2 != Vector3.zero)
                        {
                            num4 += zero2.x;
                            zero2 = Vector3.zero;
                        }
                        BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
                        if (bmsymbol == null)
                        {
                            Single num8 = NGUIText.GetGlyphWidth(num7, prev);
                            if (num8 != 0f)
                            {
                                num8 += NGUIText.finalSpacingX;
                                if (Mathf.RoundToInt(num4 + num8) > NGUIText.regionWidth)
                                {
                                    if (num4 > num6)
                                    {
                                        num6 = num4 - NGUIText.finalSpacingX;
                                    }
                                    num4 = num8;
                                    num5 += NGUIText.finalLineHeight;
                                    zero2 = Vector3.zero;
                                }
                                else
                                {
                                    num4 += num8;
                                }
                                prev = num7;
                            }
                        }
                        else
                        {
                            Single num9 = NGUIText.finalSpacingX + (Single)bmsymbol.advance * NGUIText.fontScale;
                            if (Mathf.RoundToInt(num4 + num9) > NGUIText.regionWidth)
                            {
                                if (num4 > num6)
                                {
                                    num6 = num4 - NGUIText.finalSpacingX;
                                }
                                num4 = num9;
                                num5 += NGUIText.finalLineHeight;
                                zero2 = Vector3.zero;
                            }
                            else
                            {
                                num4 += num9;
                            }
                            i += bmsymbol.sequence.Length - 1;
                            prev = 0;
                        }
                    }
                }
            }
            zero.x = ((num4 <= num6) ? num6 : (num4 - NGUIText.finalSpacingX));
            zero.y = num5 + NGUIText.finalLineHeight;
        }
        return zero;
    }

    public static String StripSymbols2(String text)
    {
        if (text != null)
        {
            Int32 i = 0;
            Int32 length = text.Length;
            while (i < length)
            {
                Char c = text[i];
                if (c == '[' || c == '{')
                {
                    Int32 num = 0;
                    Boolean flag = false;
                    Boolean flag2 = false;
                    Boolean flag3 = false;
                    Boolean flag4 = false;
                    Boolean flag5 = false;
                    Boolean flag6 = false;
                    Boolean flag7 = false;
                    Boolean flag8 = false;
                    Int32 num2 = i;
                    Int32 num3 = 0;
                    Vector3 zero = Vector3.zero;
                    Single num4 = 0f;
                    Dialog.DialogImage dialogImage = (Dialog.DialogImage)null;
                    if (DialogBoxSymbols.ParseSymbol(text, ref num2, null, false, ref num, ref flag, ref flag2, ref flag3, ref flag4, ref flag5, ref flag6, ref flag7, ref flag8, ref num3, ref zero, ref num4, ref dialogImage))
                    {
                        String text2 = text.Substring(i, num2 - i);
                        if (!NGUIText.ContainsTextOffset(text2) && dialogImage == null)
                        {
                            text = text.Remove(i, num2 - i);
                            length = text.Length;
                        }
                        else
                        {
                            i = num2 - 1;
                        }
                        continue;
                    }
                }
                i++;
            }
        }
        return text;
    }

    public static void SetIconDepth(GameObject phaseLabel, GameObject iconObject, Boolean isLowerPhrase = true)
    {
        Int32 depth = phaseLabel.GetComponent<UIWidget>().depth;
        Int32 num = (Int32)((!isLowerPhrase) ? (depth + 1) : (depth - iconObject.transform.childCount - 1));
        iconObject.GetComponent<UIWidget>().depth = num++;
        foreach (Object obj in iconObject.transform)
        {
            Transform transform = (Transform)obj;
            transform.GetComponent<UIWidget>().depth = num++;
        }
    }

    private static void AlignImageWithLastChar(ref BetterList<Dialog.DialogImage> specialImages, BetterList<Int32> imageAlignmentList, BetterList<Vector3> verts, Int32 printedLine)
    {
        foreach (Int32 i in imageAlignmentList)
        {
            Dialog.DialogImage dialogImage = specialImages[i];
            if (NGUIText.AlignImageCondition(dialogImage) && printedLine == dialogImage.PrintedLine)
            {
                Dialog.DialogImage dialogImage2 = dialogImage;
                dialogImage2.LocalPosition.y = dialogImage2.LocalPosition.y + (verts[verts.size - 2].y - (dialogImage.LocalPosition.y - dialogImage.Size.y));
                Dialog.DialogImage dialogImage3 = dialogImage;
                dialogImage3.LocalPosition.y = dialogImage3.LocalPosition.y + dialogImage.Offset.y;
            }
        }
    }

    private static Boolean AlignImageCondition(Dialog.DialogImage img)
    {
        return !NGUIText.IconIdException.Contains(img.Id);
    }

    private static Boolean ContainCharAlignment(Int32 ch)
    {
        return !NGUIText.CharException.Contains((Char)ch);
    }

    private static void AlignImageWithPadding(ref BetterList<Dialog.DialogImage> specialImages, Vector3 padding, Int32 printedLine)
    {
        foreach (Dialog.DialogImage dialogImage in specialImages)
        {
            if (dialogImage.PrintedLine == printedLine)
            {
                dialogImage.LocalPosition += padding;
            }
        }
    }

    private static void AlignImage(BetterList<Vector3> verts, Int32 indexOffset, Single printedWidth, BetterList<Dialog.DialogImage> imageList, Int32 printedLine)
    {
        NGUIText.Alignment alignment = NGUIText.alignment;
        if (alignment != NGUIText.Alignment.Left)
        {
            if (alignment == NGUIText.Alignment.Center)
            {
                Single num = ((Single)NGUIText.rectWidth - printedWidth) * 0.5f;
                if (num < 0f)
                {
                    return;
                }
                Int32 num2 = Mathf.RoundToInt((Single)NGUIText.rectWidth - printedWidth);
                Int32 num3 = Mathf.RoundToInt((Single)NGUIText.rectWidth);
                Boolean flag = (num2 & 1) == 1;
                Boolean flag2 = (num3 & 1) == 1;
                if ((flag && !flag2) || (!flag && flag2))
                {
                    num += 0.5f * NGUIText.fontScale;
                }
                NGUIText.AlignImageWithPadding(ref imageList, new Vector3(num, 0f), printedLine);
            }
        }
        else
        {
            if (verts.size == 0 || verts.size <= indexOffset)
            {
                return;
            }
            if (verts.buffer != null)
            {
                Single num4 = ((Single)NGUIText.rectWidth - printedWidth) * 0.5f;
                Int32 num5 = Mathf.RoundToInt((Single)NGUIText.rectWidth - printedWidth);
                Int32 num6 = Mathf.RoundToInt((Single)NGUIText.rectWidth);
                Boolean flag3 = (num5 & 1) == 1;
                Boolean flag4 = (num6 & 1) == 1;
                if ((flag3 && !flag4) || (!flag3 && flag4))
                {
                    num4 += 0.5f * NGUIText.fontScale;
                }
                Single num7 = verts.buffer[indexOffset].x - num4;
                Single num8 = (Single)((num5 <= 0) ? 0 : num5);
                if (num8 < 0f)
                {
                    NGUIText.AlignImageWithPadding(ref imageList, new Vector3(num8, 0f), printedLine);
                }
            }
        }
    }

    private static void AddSpecialIconToList(ref BetterList<Dialog.DialogImage> specialImages, ref BetterList<Int32> imageAlignmentList, ref Dialog.DialogImage insertImage, Vector3 extraOffset, ref Single currentX, Single currentY, Int32 printedLine)
    {
        if (insertImage != null)
        {
            insertImage.LocalPosition = new Vector3(currentX, currentY);
            Dialog.DialogImage dialogImage = insertImage;
            dialogImage.LocalPosition.x = dialogImage.LocalPosition.x + extraOffset.x;
            Dialog.DialogImage dialogImage2 = insertImage;
            dialogImage2.LocalPosition.y = dialogImage2.LocalPosition.y - extraOffset.y;
            insertImage.LocalPosition.y = -insertImage.LocalPosition.y;
            insertImage.PrintedLine = printedLine;
            specialImages.Add(insertImage);
            imageAlignmentList.Add(specialImages.size - 1);
            currentX += insertImage.Size.x - 20f;
            insertImage = (Dialog.DialogImage)null;
        }
    }

    public static String GetTestingResource()
    {
        return String.Empty;
    }

    public static void Update()
    {
        NGUIText.Update(true);
    }

    public static void Update(Boolean request)
    {
        NGUIText.finalSize = Mathf.RoundToInt((Single)NGUIText.fontSize / NGUIText.pixelDensity);
        NGUIText.finalSpacingX = NGUIText.spacingX * NGUIText.fontScale;
        NGUIText.finalLineHeight = ((Single)NGUIText.fontSize + NGUIText.spacingY) * NGUIText.fontScale;
        NGUIText.useSymbols = (NGUIText.bitmapFont != (UnityEngine.Object)null && NGUIText.bitmapFont.hasSymbols && NGUIText.encoding && NGUIText.symbolStyle != NGUIText.SymbolStyle.None);
        if (NGUIText.dynamicFont != (UnityEngine.Object)null && request)
        {
            NGUIText.dynamicFont.RequestCharactersInTexture(")_-", NGUIText.finalSize, NGUIText.fontStyle);
            if (!NGUIText.dynamicFont.GetCharacterInfo(')', out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle) || (Single)NGUIText.mTempChar.maxY == 0f)
            {
                NGUIText.dynamicFont.RequestCharactersInTexture("A", NGUIText.finalSize, NGUIText.fontStyle);
                if (!NGUIText.dynamicFont.GetCharacterInfo('A', out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle))
                {
                    NGUIText.baseline = 0f;
                    return;
                }
            }
            Single num = (Single)NGUIText.mTempChar.maxY;
            Single num2 = (Single)NGUIText.mTempChar.minY;
            NGUIText.baseline = Mathf.Round(num + ((Single)NGUIText.finalSize - num + num2) * 0.5f);
        }
    }

    public static void Prepare(String text)
    {
        if (NGUIText.dynamicFont != (UnityEngine.Object)null)
        {
            NGUIText.dynamicFont.RequestCharactersInTexture(text, NGUIText.finalSize, NGUIText.fontStyle);
        }
    }

    public static BMSymbol GetSymbol(String text, Int32 index, Int32 textLength)
    {
        return (!(NGUIText.bitmapFont != (UnityEngine.Object)null)) ? null : NGUIText.bitmapFont.MatchSymbol(text, index, textLength);
    }

    public static Single GetGlyphWidth(Int32 ch, Int32 prev)
    {
        if (NGUIText.bitmapFont != (UnityEngine.Object)null)
        {
            Boolean flag = false;
            if (ch == 8201)
            {
                flag = true;
                ch = 32;
            }
            BMGlyph bmglyph = NGUIText.bitmapFont.bmFont.GetGlyph(ch);
            if (bmglyph != null)
            {
                Int32 num = bmglyph.advance;
                if (flag)
                {
                    num >>= 1;
                }
                return NGUIText.fontScale * (Single)((prev == 0) ? bmglyph.advance : (num + bmglyph.GetKerning(prev)));
            }
        }
        else if (NGUIText.dynamicFont != (UnityEngine.Object)null && NGUIText.dynamicFont.GetCharacterInfo((Char)ch, out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle))
        {
            return (Single)NGUIText.mTempChar.advance * NGUIText.fontScale * NGUIText.pixelDensity;
        }
        return 0f;
    }

    public static NGUIText.GlyphInfo GetGlyph(Int32 ch, Int32 prev)
    {
        if (NGUIText.bitmapFont != (UnityEngine.Object)null)
        {
            Boolean flag = false;
            if (ch == 8201)
            {
                flag = true;
                ch = 32;
            }
            BMGlyph bmglyph = NGUIText.bitmapFont.bmFont.GetGlyph(ch);
            if (bmglyph != null)
            {
                Int32 num = (Int32)((prev == 0) ? 0 : bmglyph.GetKerning(prev));
                NGUIText.glyph.v0.x = (Single)((prev == 0) ? bmglyph.offsetX : (bmglyph.offsetX + num));
                NGUIText.glyph.v1.y = (Single)(-(Single)bmglyph.offsetY);
                NGUIText.glyph.v1.x = NGUIText.glyph.v0.x + (Single)bmglyph.width;
                NGUIText.glyph.v0.y = NGUIText.glyph.v1.y - (Single)bmglyph.height;
                NGUIText.glyph.u0.x = (Single)bmglyph.x;
                NGUIText.glyph.u0.y = (Single)(bmglyph.y + bmglyph.height);
                NGUIText.glyph.u2.x = (Single)(bmglyph.x + bmglyph.width);
                NGUIText.glyph.u2.y = (Single)bmglyph.y;
                NGUIText.glyph.u1.x = NGUIText.glyph.u0.x;
                NGUIText.glyph.u1.y = NGUIText.glyph.u2.y;
                NGUIText.glyph.u3.x = NGUIText.glyph.u2.x;
                NGUIText.glyph.u3.y = NGUIText.glyph.u0.y;
                Int32 num2 = bmglyph.advance;
                if (flag)
                {
                    num2 >>= 1;
                }
                NGUIText.glyph.advance = (Single)(num2 + num);
                NGUIText.glyph.channel = bmglyph.channel;
                if (NGUIText.fontScale != 1f)
                {
                    NGUIText.glyph.v0 *= NGUIText.fontScale;
                    NGUIText.glyph.v1 *= NGUIText.fontScale;
                    NGUIText.glyph.advance *= NGUIText.fontScale;
                }
                return NGUIText.glyph;
            }
        }
        else if (NGUIText.dynamicFont != (UnityEngine.Object)null && NGUIText.dynamicFont.GetCharacterInfo((Char)ch, out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle))
        {
            NGUIText.glyph.v0.x = (Single)NGUIText.mTempChar.minX;
            NGUIText.glyph.v1.x = (Single)NGUIText.mTempChar.maxX;
            NGUIText.glyph.v0.y = (Single)NGUIText.mTempChar.maxY - NGUIText.baseline;
            NGUIText.glyph.v1.y = (Single)NGUIText.mTempChar.minY - NGUIText.baseline;
            NGUIText.glyph.u0 = NGUIText.mTempChar.uvTopLeft;
            NGUIText.glyph.u1 = NGUIText.mTempChar.uvBottomLeft;
            NGUIText.glyph.u2 = NGUIText.mTempChar.uvBottomRight;
            NGUIText.glyph.u3 = NGUIText.mTempChar.uvTopRight;
            NGUIText.glyph.advance = (Single)NGUIText.mTempChar.advance;
            NGUIText.glyph.channel = 0;
            NGUIText.glyph.v0.x = Mathf.Round(NGUIText.glyph.v0.x);
            NGUIText.glyph.v0.y = Mathf.Round(NGUIText.glyph.v0.y);
            NGUIText.glyph.v1.x = Mathf.Round(NGUIText.glyph.v1.x);
            NGUIText.glyph.v1.y = Mathf.Round(NGUIText.glyph.v1.y);
            Single num3 = NGUIText.fontScale * NGUIText.pixelDensity;
            if (num3 != 1f)
            {
                NGUIText.glyph.v0 *= num3;
                NGUIText.glyph.v1 *= num3;
                NGUIText.glyph.advance *= num3;
            }
            return NGUIText.glyph;
        }
        return (NGUIText.GlyphInfo)null;
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static Single ParseAlpha(String text, Int32 index)
    {
        Int32 num = NGUIMath.HexToDecimal(text[index + 1]) << 4 | NGUIMath.HexToDecimal(text[index + 2]);
        return Mathf.Clamp01((Single)num / 255f);
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static Color ParseColor(String text, Int32 offset)
    {
        return NGUIText.ParseColor24(text, offset);
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static Color ParseColor24(String text, Int32 offset)
    {
        Int32 num = NGUIMath.HexToDecimal(text[offset]) << 4 | NGUIMath.HexToDecimal(text[offset + 1]);
        Int32 num2 = NGUIMath.HexToDecimal(text[offset + 2]) << 4 | NGUIMath.HexToDecimal(text[offset + 3]);
        Int32 num3 = NGUIMath.HexToDecimal(text[offset + 4]) << 4 | NGUIMath.HexToDecimal(text[offset + 5]);
        Single num4 = 0.003921569f;
        return new Color(num4 * (Single)num, num4 * (Single)num2, num4 * (Single)num3);
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static Color ParseColor32(String text, Int32 offset)
    {
        Int32 num = NGUIMath.HexToDecimal(text[offset]) << 4 | NGUIMath.HexToDecimal(text[offset + 1]);
        Int32 num2 = NGUIMath.HexToDecimal(text[offset + 2]) << 4 | NGUIMath.HexToDecimal(text[offset + 3]);
        Int32 num3 = NGUIMath.HexToDecimal(text[offset + 4]) << 4 | NGUIMath.HexToDecimal(text[offset + 5]);
        Int32 num4 = NGUIMath.HexToDecimal(text[offset + 6]) << 4 | NGUIMath.HexToDecimal(text[offset + 7]);
        Single num5 = 0.003921569f;
        return new Color(num5 * (Single)num, num5 * (Single)num2, num5 * (Single)num3, num5 * (Single)num4);
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static String EncodeColor(Color c)
    {
        return NGUIText.EncodeColor24(c);
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static String EncodeColor(String text, Color c)
    {
        return String.Concat(new String[]
        {
            "[c][",
            NGUIText.EncodeColor24(c),
            "]",
            text,
            "[-][/c]"
        });
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static String EncodeAlpha(Single a)
    {
        Int32 num = Mathf.Clamp(Mathf.RoundToInt(a * 255f), 0, 255);
        return NGUIMath.DecimalToHex8(num);
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static String EncodeColor24(Color c)
    {
        Int32 num = 16777215 & NGUIMath.ColorToInt(c) >> 8;
        return NGUIMath.DecimalToHex24(num);
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static String EncodeColor32(Color c)
    {
        Int32 num = NGUIMath.ColorToInt(c);
        return NGUIMath.DecimalToHex32(num);
    }

    public static Boolean ParseSymbol(String text, ref Int32 index, ref Int32 ff9Signal, ref Dialog.DialogImage insertImage)
    {
        Int32 num = 1;
        Boolean flag = false;
        Boolean flag2 = false;
        Boolean flag3 = false;
        Boolean flag4 = false;
        Boolean flag5 = false;
        Boolean flag6 = false;
        Boolean flag7 = false;
        Boolean flag8 = false;
        Vector3 zero = Vector3.zero;
        Single num2 = 0f;
        return DialogBoxSymbols.ParseSymbol(text, ref index, null, false, ref num, ref flag, ref flag2, ref flag3, ref flag4, ref flag5, ref flag6, ref flag7, ref flag8, ref ff9Signal, ref zero, ref num2, ref insertImage);
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static Boolean IsHex(Char ch)
    {
        return (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
    }



    public static String StripSymbols(String text)
    {
        if (text != null)
        {
            Int32 i = 0;
            Int32 length = text.Length;
            while (i < length)
            {
                Char c = text[i];
                if (c == '[' || c == '{')
                {
                    Int32 num = 0;
                    Boolean flag = false;
                    Boolean flag2 = false;
                    Boolean flag3 = false;
                    Boolean flag4 = false;
                    Boolean flag5 = false;
                    Boolean flag6 = false;
                    Boolean flag7 = false;
                    Boolean flag8 = false;
                    Int32 num2 = i;
                    Int32 num3 = 0;
                    Vector3 zero = Vector3.zero;
                    Single num4 = 0f;
                    Dialog.DialogImage dialogImage = (Dialog.DialogImage)null;
                    if (DialogBoxSymbols.ParseSymbol(text, ref num2, null, false, ref num, ref flag, ref flag2, ref flag3, ref flag4, ref flag5, ref flag6, ref flag7, ref flag8, ref num3, ref zero, ref num4, ref dialogImage))
                    {
                        text = text.Remove(i, num2 - i);
                        length = text.Length;
                        continue;
                    }
                }
                i++;
            }
        }
        return text;
    }

    public static void Align(BetterList<Vector3> verts, Int32 indexOffset, Single printedWidth, Int32 elements = 4)
    {
        if (verts.size == 0 || verts.size <= indexOffset)
        {
            return;
        }
        switch (NGUIText.alignment)
        {
            case NGUIText.Alignment.Left:
            {
                if (verts.buffer != null)
                {
                    Single num = ((Single)NGUIText.rectWidth - printedWidth) * 0.5f;
                    Int32 num2 = Mathf.RoundToInt((Single)NGUIText.rectWidth - printedWidth);
                    Int32 num3 = Mathf.RoundToInt((Single)NGUIText.rectWidth);
                    Boolean flag = (num2 & 1) == 1;
                    Boolean flag2 = (num3 & 1) == 1;
                    if ((flag && !flag2) || (!flag && flag2))
                    {
                        num += 0.5f * NGUIText.fontScale;
                    }
                    Single num4 = verts.buffer[indexOffset].x - num;
                    Single num5 = (Single)((num2 <= 0) ? 0 : num2);
                    if (num5 < 0f)
                    {
                        for (Int32 i = indexOffset; i < verts.size; i++)
                        {
                            Vector3[] buffer = verts.buffer;
                            Int32 num6 = i;
                            buffer[num6].x = buffer[num6].x + num5;
                        }
                    }
                }
                break;
            }
            case NGUIText.Alignment.Center:
            {
                Single num7 = ((Single)NGUIText.rectWidth - printedWidth) * 0.5f;
                if (num7 < 0f)
                {
                    return;
                }
                Int32 num8 = Mathf.RoundToInt((Single)NGUIText.rectWidth - printedWidth);
                Int32 num9 = Mathf.RoundToInt((Single)NGUIText.rectWidth);
                Boolean flag3 = (num8 & 1) == 1;
                Boolean flag4 = (num9 & 1) == 1;
                if ((flag3 && !flag4) || (!flag3 && flag4))
                {
                    num7 += 0.5f * NGUIText.fontScale;
                }
                for (Int32 j = indexOffset; j < verts.size; j++)
                {
                    Vector3[] buffer2 = verts.buffer;
                    Int32 num10 = j;
                    buffer2[num10].x = buffer2[num10].x + num7;
                }
                break;
            }
            case NGUIText.Alignment.Right:
            {
                Single num11 = (Single)NGUIText.rectWidth - printedWidth;
                if (num11 < 0f)
                {
                    return;
                }
                for (Int32 k = indexOffset; k < verts.size; k++)
                {
                    Vector3[] buffer3 = verts.buffer;
                    Int32 num12 = k;
                    buffer3[num12].x = buffer3[num12].x + num11;
                }
                break;
            }
            case NGUIText.Alignment.Justified:
            {
                if (printedWidth < (Single)NGUIText.rectWidth * 0.65f)
                {
                    return;
                }
                Single margin = (NGUIText.rectWidth - printedWidth) * 0.5f;
                if (margin < 1f)
                    return;

                Int32 padding = (verts.size - indexOffset) / elements;
                if (padding < 2)
                    return;

                Single paddingFactor = 1f / (padding - 1);
                Single widthRatio = NGUIText.rectWidth / printedWidth;
                Int32 l = indexOffset + elements;
                Int32 counter = 1;
                while (l < verts.size)
                {
                    Single currentX1 = verts.buffer[l].x;
                    Single currentX2 = verts.buffer[l + elements / 2].x;
                    Single deltaX = currentX2 - currentX1;
                    Single scaledX1 = currentX1 * widthRatio;
                    Single a = scaledX1 + deltaX;
                    Single scaledX2 = currentX2 * widthRatio;
                    Single b = scaledX2 - deltaX;
                    Single t = counter * paddingFactor;
                    currentX2 = Mathf.Lerp(a, scaledX2, t);
                    currentX1 = Mathf.Lerp(scaledX1, b, t);
                    currentX1 = Mathf.Round(currentX1);
                    currentX2 = Mathf.Round(currentX2);
                    if (elements == 4)
                    {
                        verts.buffer[l++].x = currentX1;
                        verts.buffer[l++].x = currentX1;
                        verts.buffer[l++].x = currentX2;
                        verts.buffer[l++].x = currentX2;
                    }
                    else if (elements == 2)
                    {
                        verts.buffer[l++].x = currentX1;
                        verts.buffer[l++].x = currentX2;
                    }
                    else if (elements == 1)
                    {
                        verts.buffer[l++].x = currentX1;
                    }
                    counter++;
                }
                break;
            }
        }
    }

    public static Int32 GetExactCharacterIndex(BetterList<Vector3> verts, BetterList<Int32> indices, Vector2 pos)
    {
        for (Int32 i = 0; i < indices.size; i++)
        {
            Int32 num = i << 1;
            Int32 i2 = num + 1;
            Single x = verts[num].x;
            if (pos.x >= x)
            {
                Single x2 = verts[i2].x;
                if (pos.x <= x2)
                {
                    Single y = verts[num].y;
                    if (pos.y >= y)
                    {
                        Single y2 = verts[i2].y;
                        if (pos.y <= y2)
                        {
                            return indices[i];
                        }
                    }
                }
            }
        }
        return 0;
    }

    public static Int32 GetApproximateCharacterIndex(BetterList<Vector3> verts, BetterList<Int32> indices, Vector2 pos)
    {
        Single num = Single.MaxValue;
        Single num2 = Single.MaxValue;
        Int32 i = 0;
        for (Int32 j = 0; j < verts.size; j++)
        {
            Single num3 = Mathf.Abs(pos.y - verts[j].y);
            if (num3 <= num2)
            {
                Single num4 = Mathf.Abs(pos.x - verts[j].x);
                if (num3 < num2)
                {
                    num2 = num3;
                    num = num4;
                    i = j;
                }
                else if (num4 < num)
                {
                    num = num4;
                    i = j;
                }
            }
        }
        return indices[i];
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    private static Boolean IsSpace(Int32 ch)
    {
        return ch == 32 || ch == 8202 || ch == 8203 || ch == 8201;
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void EndLine(ref StringBuilder s)
    {
        Int32 num = s.Length - 1;
        if (num > 0 && NGUIText.IsSpace((Int32)s[num]))
        {
            s[num] = '\n';
        }
        else
        {
            s.Append('\n');
        }
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    private static void ReplaceSpaceWithNewline(ref StringBuilder s)
    {
        Int32 num = s.Length - 1;
        if (num > 0 && NGUIText.IsSpace((Int32)s[num]))
        {
            s[num] = '\n';
        }
    }

    public static Vector2 CalculatePrintedSize(String text)
    {
        Vector2 zero = Vector2.zero;
        if (!String.IsNullOrEmpty(text))
        {
            if (NGUIText.encoding)
            {
                text = NGUIText.StripSymbols(text);
            }
            NGUIText.Prepare(text);
            Single num = 0f;
            Single num2 = 0f;
            Single num3 = 0f;
            Int32 length = text.Length;
            Int32 prev = 0;
            for (Int32 i = 0; i < length; i++)
            {
                Int32 num4 = (Int32)text[i];
                if (num4 == 10)
                {
                    if (num > num3)
                    {
                        num3 = num;
                    }
                    num = 0f;
                    num2 += NGUIText.finalLineHeight;
                }
                else if (num4 >= 32)
                {
                    BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
                    if (bmsymbol == null)
                    {
                        Single num5 = NGUIText.GetGlyphWidth(num4, prev);
                        if (num5 != 0f)
                        {
                            num5 += NGUIText.finalSpacingX;
                            if (Mathf.RoundToInt(num + num5) > NGUIText.regionWidth)
                            {
                                if (num > num3)
                                {
                                    num3 = num - NGUIText.finalSpacingX;
                                }
                                num = num5;
                                num2 += NGUIText.finalLineHeight;
                            }
                            else
                            {
                                num += num5;
                            }
                            prev = num4;
                        }
                    }
                    else
                    {
                        Single num6 = NGUIText.finalSpacingX + (Single)bmsymbol.advance * NGUIText.fontScale;
                        if (Mathf.RoundToInt(num + num6) > NGUIText.regionWidth)
                        {
                            if (num > num3)
                            {
                                num3 = num - NGUIText.finalSpacingX;
                            }
                            num = num6;
                            num2 += NGUIText.finalLineHeight;
                        }
                        else
                        {
                            num += num6;
                        }
                        i += bmsymbol.sequence.Length - 1;
                        prev = 0;
                    }
                }
            }
            zero.x = ((num <= num3) ? num3 : (num - NGUIText.finalSpacingX));
            zero.y = num2 + NGUIText.finalLineHeight;
        }
        return zero;
    }

    public static Int32 CalculateOffsetToFit(String text)
    {
        if (String.IsNullOrEmpty(text) || NGUIText.regionWidth < 1)
        {
            return 0;
        }
        NGUIText.Prepare(text);
        Int32 length = text.Length;
        Int32 prev = 0;
        Int32 i = 0;
        Int32 length2 = text.Length;
        while (i < length2)
        {
            BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
            if (bmsymbol == null)
            {
                Int32 num = (Int32)text[i];
                Single glyphWidth = NGUIText.GetGlyphWidth(num, prev);
                if (glyphWidth != 0f)
                {
                    NGUIText.mSizes.Add(NGUIText.finalSpacingX + glyphWidth);
                }
                prev = num;
            }
            else
            {
                NGUIText.mSizes.Add(NGUIText.finalSpacingX + (Single)bmsymbol.advance * NGUIText.fontScale);
                Int32 j = 0;
                Int32 num2 = bmsymbol.sequence.Length - 1;
                while (j < num2)
                {
                    NGUIText.mSizes.Add(0f);
                    j++;
                }
                i += bmsymbol.sequence.Length - 1;
                prev = 0;
            }
            i++;
        }
        Single num3 = (Single)NGUIText.regionWidth;
        Int32 num4 = NGUIText.mSizes.size;
        while (num4 > 0 && num3 > 0f)
        {
            num3 -= NGUIText.mSizes[--num4];
        }
        NGUIText.mSizes.Clear();
        if (num3 < 0f)
        {
            num4++;
        }
        return num4;
    }

    public static String GetEndOfLineThatFits(String text)
    {
        Int32 length = text.Length;
        Int32 num = NGUIText.CalculateOffsetToFit(text);
        return text.Substring(num, length - num);
    }

    public static Boolean WrapText(String text, out String finalText, Boolean wrapLineColors = false)
    {
        return NGUIText.WrapText(text, out finalText, false, wrapLineColors);
    }

    public static Boolean WrapText(String text, out String finalText, Boolean keepCharCount, Boolean wrapLineColors)
    {
        if (NGUIText.regionWidth < 1 || NGUIText.regionHeight < 1 || NGUIText.finalLineHeight < 1f)
        {
            finalText = String.Empty;
            return false;
        }
        Single num = (NGUIText.maxLines <= 0) ? ((Single)NGUIText.regionHeight) : Mathf.Min((Single)NGUIText.regionHeight, NGUIText.finalLineHeight * (Single)NGUIText.maxLines);
        Int32 num2 = (Int32)((NGUIText.maxLines <= 0) ? 1000000 : NGUIText.maxLines);
        num2 = Mathf.FloorToInt(Mathf.Min((Single)num2, num / NGUIText.finalLineHeight) + 0.01f);
        if (num2 == 0)
        {
            finalText = String.Empty;
            return false;
        }
        if (String.IsNullOrEmpty(text))
        {
            text = " ";
        }
        NGUIText.Prepare(text);
        StringBuilder stringBuilder = new StringBuilder();
        Int32 length = text.Length;
        Single num3 = (Single)NGUIText.regionWidth;
        Int32 num4 = 0;
        Int32 i = 0;
        Int32 num5 = 1;
        Int32 prev = 0;
        Boolean flag = true;
        Boolean flag2 = true;
        Boolean flag3 = false;
        Color item = NGUIText.tint;
        Int32 num6 = 0;
        Boolean flag4 = false;
        Boolean flag5 = false;
        Boolean flag6 = false;
        Boolean flag7 = false;
        Boolean flag8 = false;
        Boolean flag9 = false;
        Boolean flag10 = false;
        Boolean flag15 = false;
        Int32 num7 = 0;
        Vector3 zero = Vector3.zero;
        Single num8 = 0f;
        Dialog.DialogImage dialogImage = (Dialog.DialogImage)null;
        if (!NGUIText.useSymbols)
        {
            wrapLineColors = false;
        }
        if (wrapLineColors)
        {
            NGUIText.mColors.Add(item);
        }
        while (i < length)
        {
            Char c = text[i];
            if (c > '⿿')
            {
                flag3 = true;
            }
            if (c == '\n')
            {
                if (num5 == num2)
                {
                    break;
                }
                num3 = (Single)NGUIText.regionWidth;
                if (num4 < i)
                {
                    stringBuilder.Append(text.Substring(num4, i - num4 + 1));
                }
                else
                {
                    stringBuilder.Append(c);
                }
                if (wrapLineColors)
                {
                    for (Int32 j = 0; j < NGUIText.mColors.size; j++)
                    {
                        stringBuilder.Insert(stringBuilder.Length - 1, "[-]");
                    }
                    for (Int32 k = 0; k < NGUIText.mColors.size; k++)
                    {
                        stringBuilder.Append("[");
                        stringBuilder.Append(NGUIText.EncodeColor(NGUIText.mColors[k]));
                        stringBuilder.Append("]");
                    }
                }
                flag = true;
                num5++;
                num4 = i + 1;
                prev = 0;
            }
            else
            {
                if (NGUIText.encoding)
                {
                    if (!wrapLineColors)
                    {
                        if (NGUIText.ParseSymbol(text, ref i, ref num7, ref dialogImage))
                        {
                            i--;
                            goto IL_69E;
                        }
                    }
                    else if (DialogBoxSymbols.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref num6, ref flag4, ref flag5, ref flag6, ref flag7, ref flag8, ref flag9, ref flag10, ref flag15, ref num7, ref zero, ref num8, ref dialogImage))
                    {
                        if (flag8)
                        {
                            item = NGUIText.mColors[NGUIText.mColors.size - 1];
                            item.a *= NGUIText.mAlpha * NGUIText.tint.a;
                        }
                        else
                        {
                            item = NGUIText.tint * NGUIText.mColors[NGUIText.mColors.size - 1];
                            item.a *= NGUIText.mAlpha;
                        }
                        Int32 l = 0;
                        Int32 num9 = NGUIText.mColors.size - 2;
                        while (l < num9)
                        {
                            item.a *= NGUIText.mColors[l].a;
                            l++;
                        }
                        i--;
                        goto IL_69E;
                    }
                }
                BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
                Single num10;
                if (bmsymbol == null)
                {
                    Single glyphWidth = NGUIText.GetGlyphWidth((Int32)c, prev);
                    if (glyphWidth == 0f)
                    {
                        goto IL_69E;
                    }
                    num10 = NGUIText.finalSpacingX + glyphWidth;
                }
                else
                {
                    num10 = NGUIText.finalSpacingX + (Single)bmsymbol.advance * NGUIText.fontScale;
                }
                num3 -= num10;
                if (NGUIText.IsSpace((Int32)c) && !flag3 && num4 < i)
                {
                    Int32 num11 = i - num4 + 1;
                    if (num5 == num2 && num3 <= 0f && i < length)
                    {
                        Char c2 = text[i];
                        if (c2 < ' ' || NGUIText.IsSpace((Int32)c2))
                        {
                            num11--;
                        }
                    }
                    stringBuilder.Append(text.Substring(num4, num11));
                    flag = false;
                    num4 = i + 1;
                }
                if (Mathf.RoundToInt(num3) < 0)
                {
                    if (flag || num5 == num2)
                    {
                        stringBuilder.Append(text.Substring(num4, Mathf.Max(0, i - num4)));
                        Boolean flag11 = NGUIText.IsSpace((Int32)c);
                        if (!flag11 && !flag3)
                        {
                            flag2 = false;
                        }
                        if (wrapLineColors && NGUIText.mColors.size > 0)
                        {
                            stringBuilder.Append("[-]");
                        }
                        if (num5++ == num2)
                        {
                            num4 = i;
                            break;
                        }
                        if (keepCharCount)
                        {
                            NGUIText.ReplaceSpaceWithNewline(ref stringBuilder);
                        }
                        else
                        {
                            NGUIText.EndLine(ref stringBuilder);
                        }
                        if (wrapLineColors)
                        {
                            for (Int32 m = 0; m < NGUIText.mColors.size; m++)
                            {
                                stringBuilder.Insert(stringBuilder.Length - 1, "[-]");
                            }
                            for (Int32 n = 0; n < NGUIText.mColors.size; n++)
                            {
                                stringBuilder.Append("[");
                                stringBuilder.Append(NGUIText.EncodeColor(NGUIText.mColors[n]));
                                stringBuilder.Append("]");
                            }
                        }
                        flag = true;
                        if (flag11)
                        {
                            num4 = i + 1;
                            num3 = (Single)NGUIText.regionWidth;
                        }
                        else
                        {
                            num4 = i;
                            num3 = (Single)NGUIText.regionWidth - num10;
                        }
                        prev = 0;
                    }
                    else
                    {
                        flag = true;
                        num3 = (Single)NGUIText.regionWidth;
                        i = num4 - 1;
                        prev = 0;
                        if (num5++ == num2)
                        {
                            break;
                        }
                        if (keepCharCount)
                        {
                            NGUIText.ReplaceSpaceWithNewline(ref stringBuilder);
                        }
                        else
                        {
                            NGUIText.EndLine(ref stringBuilder);
                        }
                        if (wrapLineColors)
                        {
                            for (Int32 num12 = 0; num12 < NGUIText.mColors.size; num12++)
                            {
                                stringBuilder.Insert(stringBuilder.Length - 1, "[-]");
                            }
                            for (Int32 num13 = 0; num13 < NGUIText.mColors.size; num13++)
                            {
                                stringBuilder.Append("[");
                                stringBuilder.Append(NGUIText.EncodeColor(NGUIText.mColors[num13]));
                                stringBuilder.Append("]");
                            }
                        }
                        goto IL_69E;
                    }
                }
                else
                {
                    prev = (Int32)c;
                }
                if (bmsymbol != null)
                {
                    i += bmsymbol.length - 1;
                    prev = 0;
                }
            }
        IL_69E:
            i++;
        }
        if (num4 < i)
        {
            stringBuilder.Append(text.Substring(num4, i - num4));
        }
        if (wrapLineColors && NGUIText.mColors.size > 0)
        {
            stringBuilder.Append("[-]");
        }
        finalText = stringBuilder.ToString();
        NGUIText.mColors.Clear();
        return flag2 && (i == length || num5 <= Mathf.Min(NGUIText.maxLines, num2));
    }

    public static void Print(String text, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, out BetterList<Int32> highShadowVertIndexes, out BetterList<Dialog.DialogImage> specialImages, out BetterList<Int32> vertsLineOffsets)
    {
        highShadowVertIndexes = new BetterList<Int32>();
        specialImages = new BetterList<Dialog.DialogImage>();
        vertsLineOffsets = new BetterList<Int32>();
        if (String.IsNullOrEmpty(text))
        {
            return;
        }
        Int32 size = verts.size;
        NGUIText.Prepare(text);
        NGUIText.mColors.Add(Color.white);
        NGUIText.mAlpha = 1f;
        Int32 prevCh = 0;
        Int32 currCh = 0;
        Single currentX = 0f;
        Single lineHeight = 0f;
        Single num4 = 0f;
        Single num5 = (Single)NGUIText.finalSize;
        Color a = NGUIText.tint * NGUIText.gradientBottom;
        Color b = NGUIText.tint * NGUIText.gradientTop;
        Color32 color = NGUIText.tint;
        Int32 length = text.Length;
        Rect rect = default(Rect);
        Single num6 = 0f;
        Single num7 = 0f;
        Single num8 = num5 * NGUIText.pixelDensity;
        Boolean flag = false;
        Int32 sub = 0;
        Boolean bold = false;
        Boolean italic = false;
        Boolean underline = false;
        Boolean strike = false;
        Boolean ignoreColor = false;
        Boolean highShadow = false;
        Boolean center = false;
        Boolean justified = false;
        Boolean containCharAlignment = false;
        Int32 ff9Signal = 0;
        Vector3 extraOffset = Vector3.zero;
        Dialog.DialogImage dialogImage = (Dialog.DialogImage)null;
        Int32 printedLine = 0;
        Single tabX = 0f;
        NGUIText.Alignment alignment = NGUIText.alignment;
        BetterList<Int32> betterList = new BetterList<Int32>();
        if (NGUIText.bitmapFont != (UnityEngine.Object)null)
        {
            rect = NGUIText.bitmapFont.uvRect;
            num6 = rect.width / (Single)NGUIText.bitmapFont.texWidth;
            num7 = rect.height / (Single)NGUIText.bitmapFont.texHeight;
        }
        if (length > 0)
        {
            vertsLineOffsets.Add(0);
        }
        for (Int32 i = 0; i < length; i++)
        {
            Int32 ch = (Int32)text[i];
            Single num14 = currentX;
            if (ch == 10)
            {
                if (currentX > num4)
                {
                    num4 = currentX;
                }
                if (verts.size > 0 && containCharAlignment)
                {
                    NGUIText.AlignImageWithLastChar(ref specialImages, betterList, verts, printedLine);
                }
                betterList.Clear();
                NGUIText.AlignImage(verts, size, currentX - NGUIText.finalSpacingX, specialImages, printedLine);
                NGUIText.Align(verts, size, currentX - NGUIText.finalSpacingX, 4);
                size = verts.size;
                vertsLineOffsets.Add(size);
                NGUIText.alignment = alignment;
                center = false;
                containCharAlignment = false;
                extraOffset = Vector3.zero;
                printedLine++;
                currentX = 0f;
                lineHeight += NGUIText.finalLineHeight;
                prevCh = 0;
            }
            else if (ch < 32)
            {
                prevCh = ch;
            }
            else if (NGUIText.encoding && DialogBoxSymbols.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref sub, ref bold, ref italic, ref underline, ref strike, ref ignoreColor, ref highShadow, ref center, ref justified, ref ff9Signal, ref extraOffset, ref tabX, ref dialogImage))
            {
                Color color2;
                if (ignoreColor)
                {
                    color2 = NGUIText.mColors[NGUIText.mColors.size - 1];
                    color2.a *= NGUIText.mAlpha * NGUIText.tint.a;
                }
                else
                {
                    color2 = NGUIText.tint * NGUIText.mColors[NGUIText.mColors.size - 1];
                    color2.a *= NGUIText.mAlpha;
                }
                color = color2;
                Int32 j = 0;
                Int32 num15 = NGUIText.mColors.size - 2;
                while (j < num15)
                {
                    color2.a *= NGUIText.mColors[j].a;
                    j++;
                }
                if (NGUIText.gradient)
                {
                    a = NGUIText.gradientBottom * color2;
                    b = NGUIText.gradientTop * color2;
                }
                i--;
            }
            else
            {
                if (justified)
                {
                    NGUIText.alignment = NGUIText.Alignment.Justified;
                }
                else if (center)
                {
                    NGUIText.alignment = NGUIText.Alignment.Center;
                }
                else
                {
                    NGUIText.alignment = alignment;
                }
                if (tabX != 0f)
                {
                    extraOffset.x = 0f;
                    currentX = tabX;
                    tabX = 0f;
                }
                NGUIText.AddSpecialIconToList(ref specialImages, ref betterList, ref dialogImage, extraOffset, ref currentX, lineHeight, printedLine);
                BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
                if (bmsymbol != null)
                {
                    Single num16 = currentX + (Single)bmsymbol.offsetX * NGUIText.fontScale;
                    Single num17 = num16 + (Single)bmsymbol.width * NGUIText.fontScale;
                    Single num18 = -(lineHeight + (Single)bmsymbol.offsetY * NGUIText.fontScale);
                    Single num19 = num18 - (Single)bmsymbol.height * NGUIText.fontScale;
                    if (Mathf.RoundToInt(currentX + (Single)bmsymbol.advance * NGUIText.fontScale) > NGUIText.regionWidth)
                    {
                        if (currentX == 0f)
                        {
                            return;
                        }
                        if (size < verts.size)
                        {
                            NGUIText.AlignImage(verts, size, currentX - NGUIText.finalSpacingX, specialImages, printedLine);
                            NGUIText.Align(verts, size, currentX - NGUIText.finalSpacingX, 4);
                            size = verts.size;
                            vertsLineOffsets.Add(size);
                        }
                        NGUIText.alignment = alignment;
                        center = false;
                        extraOffset = Vector3.zero;
                        printedLine++;
                        num16 -= currentX;
                        num17 -= currentX;
                        num19 -= NGUIText.finalLineHeight;
                        num18 -= NGUIText.finalLineHeight;
                        currentX = 0f;
                        lineHeight += NGUIText.finalLineHeight;
                    }
                    verts.Add(new Vector3(num16, num19));
                    verts.Add(new Vector3(num16, num18));
                    verts.Add(new Vector3(num17, num18));
                    verts.Add(new Vector3(num17, num19));
                    currentX += NGUIText.finalSpacingX + (Single)bmsymbol.advance * NGUIText.fontScale;
                    i += bmsymbol.length - 1;
                    prevCh = 0;
                    if (uvs != null)
                    {
                        Rect uvRect = bmsymbol.uvRect;
                        Single xMin = uvRect.xMin;
                        Single yMin = uvRect.yMin;
                        Single xMax = uvRect.xMax;
                        Single yMax = uvRect.yMax;
                        uvs.Add(new Vector2(xMin, yMin));
                        uvs.Add(new Vector2(xMin, yMax));
                        uvs.Add(new Vector2(xMax, yMax));
                        uvs.Add(new Vector2(xMax, yMin));
                    }
                    if (cols != null)
                    {
                        if (NGUIText.symbolStyle == NGUIText.SymbolStyle.Colored)
                        {
                            for (Int32 k = 0; k < 4; k++)
                            {
                                cols.Add(color);
                            }
                        }
                        else
                        {
                            Color32 item = Color.white;
                            item.a = color.a;
                            for (Int32 l = 0; l < 4; l++)
                            {
                                cols.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    NGUIText.GlyphInfo glyphInfo = NGUIText.GetGlyph(ch, prevCh);
                    if (glyphInfo != null)
                    {
                        if (!containCharAlignment)
                        {
                            containCharAlignment = NGUIText.ContainCharAlignment(ch);
                        }
                        prevCh = ch;
                        currCh = ch;
                        if (sub != 0)
                        {
                            NGUIText.GlyphInfo glyphInfo2 = glyphInfo;
                            glyphInfo2.v0.x = glyphInfo2.v0.x * 0.75f;
                            NGUIText.GlyphInfo glyphInfo3 = glyphInfo;
                            glyphInfo3.v0.y = glyphInfo3.v0.y * 0.75f;
                            NGUIText.GlyphInfo glyphInfo4 = glyphInfo;
                            glyphInfo4.v1.x = glyphInfo4.v1.x * 0.75f;
                            NGUIText.GlyphInfo glyphInfo5 = glyphInfo;
                            glyphInfo5.v1.y = glyphInfo5.v1.y * 0.75f;
                            if (sub == 1)
                            {
                                NGUIText.GlyphInfo glyphInfo6 = glyphInfo;
                                glyphInfo6.v0.y = glyphInfo6.v0.y - NGUIText.fontScale * (Single)NGUIText.fontSize * 0.4f;
                                NGUIText.GlyphInfo glyphInfo7 = glyphInfo;
                                glyphInfo7.v1.y = glyphInfo7.v1.y - NGUIText.fontScale * (Single)NGUIText.fontSize * 0.4f;
                            }
                            else
                            {
                                NGUIText.GlyphInfo glyphInfo8 = glyphInfo;
                                glyphInfo8.v0.y = glyphInfo8.v0.y + NGUIText.fontScale * (Single)NGUIText.fontSize * 0.05f;
                                NGUIText.GlyphInfo glyphInfo9 = glyphInfo;
                                glyphInfo9.v1.y = glyphInfo9.v1.y + NGUIText.fontScale * (Single)NGUIText.fontSize * 0.05f;
                            }
                        }
                        Single v0x = glyphInfo.v0.x + currentX;
                        Single v0y = glyphInfo.v0.y - lineHeight;
                        Single v1x = glyphInfo.v1.x + currentX;
                        Single v1y = glyphInfo.v1.y - lineHeight;
                        Single num20 = glyphInfo.advance;
                        if (NGUIText.finalSpacingX < 0f)
                        {
                            num20 += NGUIText.finalSpacingX;
                        }
                        if (Mathf.RoundToInt(currentX + num20) > NGUIText.regionWidth)
                        {
                            if (currentX == 0f)
                            {
                                return;
                            }
                            if (size < verts.size)
                            {
                                NGUIText.Align(verts, size, currentX - NGUIText.finalSpacingX, 4);
                                size = verts.size;
                                vertsLineOffsets.Add(size);
                            }
                            extraOffset = Vector3.zero;
                            v0x -= currentX;
                            v1x -= currentX;
                            v0y -= NGUIText.finalLineHeight;
                            v1y -= NGUIText.finalLineHeight;
                            currentX = 0f;
                            lineHeight += NGUIText.finalLineHeight;
                            num14 = 0f;
                            NGUIText.alignment = alignment;
                            center = false;
                        }
                        if (NGUIText.IsSpace(ch))
                        {
                            if (underline)
                            {
                                ch = 95;
                            }
                            else if (strike)
                            {
                                ch = 45;
                            }
                        }
                        currentX += ((sub != 0) ? ((NGUIText.finalSpacingX + glyphInfo.advance) * 0.75f) : (NGUIText.finalSpacingX + glyphInfo.advance));
                        if (!NGUIText.IsSpace(ch))
                        {
                            if (uvs != null)
                            {
                                if (NGUIText.bitmapFont != (UnityEngine.Object)null)
                                {
                                    glyphInfo.u0.x = rect.xMin + num6 * glyphInfo.u0.x;
                                    glyphInfo.u2.x = rect.xMin + num6 * glyphInfo.u2.x;
                                    glyphInfo.u0.y = rect.yMax - num7 * glyphInfo.u0.y;
                                    glyphInfo.u2.y = rect.yMax - num7 * glyphInfo.u2.y;
                                    glyphInfo.u1.x = glyphInfo.u0.x;
                                    glyphInfo.u1.y = glyphInfo.u2.y;
                                    glyphInfo.u3.x = glyphInfo.u2.x;
                                    glyphInfo.u3.y = glyphInfo.u0.y;
                                }
                                Int32 m = 0;
                                Int32 num21 = (Int32)((!bold) ? 1 : 4);
                                while (m < num21)
                                {
                                    uvs.Add(glyphInfo.u0);
                                    uvs.Add(glyphInfo.u1);
                                    uvs.Add(glyphInfo.u2);
                                    uvs.Add(glyphInfo.u3);
                                    m++;
                                }
                            }
                            if (cols != null)
                            {
                                if (glyphInfo.channel == 0 || glyphInfo.channel == 15)
                                {
                                    if (NGUIText.gradient)
                                    {
                                        Single num22 = num8 + glyphInfo.v0.y / NGUIText.fontScale;
                                        Single num23 = num8 + glyphInfo.v1.y / NGUIText.fontScale;
                                        num22 /= num8;
                                        num23 /= num8;
                                        NGUIText.s_c0 = Color.Lerp(a, b, num22);
                                        NGUIText.s_c1 = Color.Lerp(a, b, num23);
                                        Int32 n = 0;
                                        Int32 num24 = (Int32)((!bold) ? 1 : 4);
                                        while (n < num24)
                                        {
                                            cols.Add(NGUIText.s_c0);
                                            cols.Add(NGUIText.s_c1);
                                            cols.Add(NGUIText.s_c1);
                                            cols.Add(NGUIText.s_c0);
                                            n++;
                                        }
                                    }
                                    else
                                    {
                                        Int32 num25 = 0;
                                        Int32 num26 = (Int32)((!bold) ? 4 : 16);
                                        while (num25 < num26)
                                        {
                                            cols.Add(color);
                                            num25++;
                                        }
                                    }
                                }
                                else
                                {
                                    Color color3 = color;
                                    color3 *= 0.49f;
                                    switch (glyphInfo.channel)
                                    {
                                        case 1:
                                            color3.b += 0.51f;
                                            break;
                                        case 2:
                                            color3.g += 0.51f;
                                            break;
                                        case 4:
                                            color3.r += 0.51f;
                                            break;
                                        case 8:
                                            color3.a += 0.51f;
                                            break;
                                    }
                                    Color32 item2 = color3;
                                    Int32 num27 = 0;
                                    Int32 num28 = (Int32)((!bold) ? 4 : 16);
                                    while (num27 < num28)
                                    {
                                        cols.Add(item2);
                                        num27++;
                                    }
                                }
                            }
                            if (!bold)
                            {
                                if (!italic)
                                {
                                    verts.Add(new Vector3(v0x, v0y));
                                    verts.Add(new Vector3(v0x, v1y));
                                    verts.Add(new Vector3(v1x, v1y));
                                    verts.Add(new Vector3(v1x, v0y));
                                }
                                else
                                {
                                    Single num29 = (Single)NGUIText.fontSize * 0.1f * ((v1y - v0y) / (Single)NGUIText.fontSize);
                                    verts.Add(new Vector3(v0x - num29, v0y));
                                    verts.Add(new Vector3(v0x + num29, v1y));
                                    verts.Add(new Vector3(v1x + num29, v1y));
                                    verts.Add(new Vector3(v1x - num29, v0y));
                                }
                            }
                            else
                            {
                                for (Int32 num30 = 0; num30 < 4; num30++)
                                {
                                    Single num31 = NGUIText.mBoldOffset[num30 * 2];
                                    Single num32 = NGUIText.mBoldOffset[num30 * 2 + 1];
                                    Single num33 = (!italic) ? 0f : ((Single)NGUIText.fontSize * 0.1f * ((v1y - v0y) / (Single)NGUIText.fontSize));
                                    verts.Add(new Vector3(v0x + num31 - num33, v0y + num32));
                                    verts.Add(new Vector3(v0x + num31 + num33, v1y + num32));
                                    verts.Add(new Vector3(v1x + num31 + num33, v1y + num32));
                                    verts.Add(new Vector3(v1x + num31 - num33, v0y + num32));
                                }
                            }
                            if (highShadow)
                            {
                                for (Int32 num34 = verts.size - 4; num34 < verts.size; num34++)
                                {
                                    highShadowVertIndexes.Add(num34);
                                }
                            }
                            if (extraOffset != Vector3.zero)
                            {
                                for (Int32 num35 = verts.size - 4; num35 < verts.size; num35++)
                                {
                                    Int32 i3;
                                    Int32 i2 = i3 = num35;
                                    Vector3 a2 = verts[i3];
                                    verts[i2] = a2 + extraOffset;
                                }
                            }
                            if (NGUIText.ContainCharAlignment(ch))
                            {
                                NGUIText.AlignImageWithLastChar(ref specialImages, betterList, verts, printedLine);
                                betterList.Clear();
                            }
                            if (underline || strike)
                            {
                                NGUIText.GlyphInfo glyphInfo10 = NGUIText.GetGlyph((Int32)((!strike) ? 95 : 45), prevCh);
                                if (glyphInfo10 != null)
                                {
                                    if (uvs != null)
                                    {
                                        if (NGUIText.bitmapFont != (UnityEngine.Object)null)
                                        {
                                            glyphInfo10.u0.x = rect.xMin + num6 * glyphInfo10.u0.x;
                                            glyphInfo10.u2.x = rect.xMin + num6 * glyphInfo10.u2.x;
                                            glyphInfo10.u0.y = rect.yMax - num7 * glyphInfo10.u0.y;
                                            glyphInfo10.u2.y = rect.yMax - num7 * glyphInfo10.u2.y;
                                        }
                                        Single x = (glyphInfo10.u0.x + glyphInfo10.u2.x) * 0.5f;
                                        Int32 num36 = 0;
                                        Int32 num37 = (Int32)((!bold) ? 1 : 4);
                                        while (num36 < num37)
                                        {
                                            uvs.Add(new Vector2(x, glyphInfo10.u0.y));
                                            uvs.Add(new Vector2(x, glyphInfo10.u2.y));
                                            uvs.Add(new Vector2(x, glyphInfo10.u2.y));
                                            uvs.Add(new Vector2(x, glyphInfo10.u0.y));
                                            num36++;
                                        }
                                    }
                                    if (flag && strike)
                                    {
                                        v0y = (-lineHeight + glyphInfo10.v0.y) * 0.75f;
                                        v1y = (-lineHeight + glyphInfo10.v1.y) * 0.75f;
                                    }
                                    else
                                    {
                                        v0y = -lineHeight + glyphInfo10.v0.y;
                                        v1y = -lineHeight + glyphInfo10.v1.y;
                                    }
                                    if (bold)
                                    {
                                        for (Int32 num38 = 0; num38 < 4; num38++)
                                        {
                                            Single num39 = NGUIText.mBoldOffset[num38 * 2];
                                            Single num40 = NGUIText.mBoldOffset[num38 * 2 + 1];
                                            verts.Add(new Vector3(num14 + num39, v0y + num40));
                                            verts.Add(new Vector3(num14 + num39, v1y + num40));
                                            verts.Add(new Vector3(currentX + num39, v1y + num40));
                                            verts.Add(new Vector3(currentX + num39, v0y + num40));
                                        }
                                    }
                                    else
                                    {
                                        verts.Add(new Vector3(num14, v0y));
                                        verts.Add(new Vector3(num14, v1y));
                                        verts.Add(new Vector3(currentX, v1y));
                                        verts.Add(new Vector3(currentX, v0y));
                                    }
                                    if (highShadow)
                                    {
                                        for (Int32 num41 = verts.size - 4; num41 < verts.size; num41++)
                                        {
                                            highShadowVertIndexes.Add(num41);
                                        }
                                    }
                                    if (NGUIText.gradient)
                                    {
                                        Single num42 = num8 + glyphInfo10.v0.y / NGUIText.fontScale;
                                        Single num43 = num8 + glyphInfo10.v1.y / NGUIText.fontScale;
                                        num42 /= num8;
                                        num43 /= num8;
                                        NGUIText.s_c0 = Color.Lerp(a, b, num42);
                                        NGUIText.s_c1 = Color.Lerp(a, b, num43);
                                        Int32 num44 = 0;
                                        Int32 num45 = (Int32)((!bold) ? 1 : 4);
                                        while (num44 < num45)
                                        {
                                            cols.Add(NGUIText.s_c0);
                                            cols.Add(NGUIText.s_c1);
                                            cols.Add(NGUIText.s_c1);
                                            cols.Add(NGUIText.s_c0);
                                            num44++;
                                        }
                                    }
                                    else
                                    {
                                        Int32 num46 = 0;
                                        Int32 num47 = (Int32)((!bold) ? 4 : 16);
                                        while (num46 < num47)
                                        {
                                            cols.Add(color);
                                            num46++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        NGUIText.AddSpecialIconToList(ref specialImages, ref betterList, ref dialogImage, extraOffset, ref currentX, lineHeight, printedLine);
        if (size < verts.size)
        {
            NGUIText.AlignImage(verts, size, currentX - NGUIText.finalSpacingX, specialImages, printedLine);
            NGUIText.Align(verts, size, currentX - NGUIText.finalSpacingX, 4);
            size = verts.size;
            vertsLineOffsets.Add(size);
            NGUIText.alignment = alignment;
            center = false;
        }
        if (betterList.size > 0 && containCharAlignment && currCh != 45)
        {
            NGUIText.AlignImageWithLastChar(ref specialImages, betterList, verts, printedLine);
            betterList.Clear();
        }
        extraOffset = Vector3.zero;
        NGUIText.mColors.Clear();
        NGUIText.alignment = alignment;
    }

    public static void PrintApproximateCharacterPositions(String text, BetterList<Vector3> verts, BetterList<Int32> indices)
    {
        if (String.IsNullOrEmpty(text))
        {
            text = " ";
        }
        NGUIText.Prepare(text);
        Single num = 0f;
        Single num2 = 0f;
        Single num3 = 0f;
        Single num4 = (Single)NGUIText.fontSize * NGUIText.fontScale * 0.5f;
        Int32 length = text.Length;
        Int32 size = verts.size;
        Int32 prev = 0;
        Int32 num5 = 0;
        Dialog.DialogImage dialogImage = (Dialog.DialogImage)null;
        for (Int32 i = 0; i < length; i++)
        {
            Int32 num6 = (Int32)text[i];
            verts.Add(new Vector3(num, -num2 - num4));
            indices.Add(i);
            if (num6 == 10)
            {
                if (num > num3)
                {
                    num3 = num;
                }
                if (NGUIText.alignment != NGUIText.Alignment.Left)
                {
                    NGUIText.Align(verts, size, num - NGUIText.finalSpacingX, 1);
                    size = verts.size;
                }
                num = 0f;
                num2 += NGUIText.finalLineHeight;
                prev = 0;
            }
            else if (num6 < 32)
            {
                prev = 0;
            }
            else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref i, ref num5, ref dialogImage))
            {
                i--;
            }
            else
            {
                BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
                if (bmsymbol == null)
                {
                    Single num7 = NGUIText.GetGlyphWidth(num6, prev);
                    if (num7 != 0f)
                    {
                        num7 += NGUIText.finalSpacingX;
                        if (Mathf.RoundToInt(num + num7) > NGUIText.regionWidth)
                        {
                            if (num == 0f)
                            {
                                return;
                            }
                            if (NGUIText.alignment != NGUIText.Alignment.Left && size < verts.size)
                            {
                                NGUIText.Align(verts, size, num - NGUIText.finalSpacingX, 1);
                                size = verts.size;
                            }
                            num = num7;
                            num2 += NGUIText.finalLineHeight;
                        }
                        else
                        {
                            num += num7;
                        }
                        verts.Add(new Vector3(num, -num2 - num4));
                        indices.Add(i + 1);
                        prev = num6;
                    }
                }
                else
                {
                    Single num8 = (Single)bmsymbol.advance * NGUIText.fontScale + NGUIText.finalSpacingX;
                    if (Mathf.RoundToInt(num + num8) > NGUIText.regionWidth)
                    {
                        if (num == 0f)
                        {
                            return;
                        }
                        if (NGUIText.alignment != NGUIText.Alignment.Left && size < verts.size)
                        {
                            NGUIText.Align(verts, size, num - NGUIText.finalSpacingX, 1);
                            size = verts.size;
                        }
                        num = num8;
                        num2 += NGUIText.finalLineHeight;
                    }
                    else
                    {
                        num += num8;
                    }
                    verts.Add(new Vector3(num, -num2 - num4));
                    indices.Add(i + 1);
                    i += bmsymbol.sequence.Length - 1;
                    prev = 0;
                }
            }
        }
        if (NGUIText.alignment != NGUIText.Alignment.Left && size < verts.size)
        {
            NGUIText.Align(verts, size, num - NGUIText.finalSpacingX, 1);
        }
    }

    public static void PrintExactCharacterPositions(String text, BetterList<Vector3> verts, BetterList<Int32> indices)
    {
        if (String.IsNullOrEmpty(text))
        {
            text = " ";
        }
        NGUIText.Prepare(text);
        Single num = (Single)NGUIText.fontSize * NGUIText.fontScale;
        Single num2 = 0f;
        Single num3 = 0f;
        Single num4 = 0f;
        Int32 length = text.Length;
        Int32 size = verts.size;
        Int32 prev = 0;
        Int32 num5 = 0;
        Dialog.DialogImage dialogImage = (Dialog.DialogImage)null;
        for (Int32 i = 0; i < length; i++)
        {
            Int32 num6 = (Int32)text[i];
            if (num6 == 10)
            {
                if (num2 > num4)
                {
                    num4 = num2;
                }
                if (NGUIText.alignment != NGUIText.Alignment.Left)
                {
                    NGUIText.Align(verts, size, num2 - NGUIText.finalSpacingX, 2);
                    size = verts.size;
                }
                num2 = 0f;
                num3 += NGUIText.finalLineHeight;
                prev = 0;
            }
            else if (num6 < 32)
            {
                prev = 0;
            }
            else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref i, ref num5, ref dialogImage))
            {
                i--;
            }
            else
            {
                BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
                if (bmsymbol == null)
                {
                    Single glyphWidth = NGUIText.GetGlyphWidth(num6, prev);
                    if (glyphWidth != 0f)
                    {
                        Single num7 = glyphWidth + NGUIText.finalSpacingX;
                        if (Mathf.RoundToInt(num2 + num7) > NGUIText.regionWidth)
                        {
                            if (num2 == 0f)
                            {
                                return;
                            }
                            if (NGUIText.alignment != NGUIText.Alignment.Left && size < verts.size)
                            {
                                NGUIText.Align(verts, size, num2 - NGUIText.finalSpacingX, 2);
                                size = verts.size;
                            }
                            num2 = 0f;
                            num3 += NGUIText.finalLineHeight;
                            prev = 0;
                            i--;
                        }
                        else
                        {
                            indices.Add(i);
                            verts.Add(new Vector3(num2, -num3 - num));
                            verts.Add(new Vector3(num2 + num7, -num3));
                            prev = num6;
                            num2 += num7;
                        }
                    }
                }
                else
                {
                    Single num8 = (Single)bmsymbol.advance * NGUIText.fontScale + NGUIText.finalSpacingX;
                    if (Mathf.RoundToInt(num2 + num8) > NGUIText.regionWidth)
                    {
                        if (num2 == 0f)
                        {
                            return;
                        }
                        if (NGUIText.alignment != NGUIText.Alignment.Left && size < verts.size)
                        {
                            NGUIText.Align(verts, size, num2 - NGUIText.finalSpacingX, 2);
                            size = verts.size;
                        }
                        num2 = 0f;
                        num3 += NGUIText.finalLineHeight;
                        prev = 0;
                        i--;
                    }
                    else
                    {
                        indices.Add(i);
                        verts.Add(new Vector3(num2, -num3 - num));
                        verts.Add(new Vector3(num2 + num8, -num3));
                        i += bmsymbol.sequence.Length - 1;
                        num2 += num8;
                        prev = 0;
                    }
                }
            }
        }
        if (NGUIText.alignment != NGUIText.Alignment.Left && size < verts.size)
        {
            NGUIText.Align(verts, size, num2 - NGUIText.finalSpacingX, 2);
        }
    }

    public static void PrintCaretAndSelection(String text, Int32 start, Int32 end, BetterList<Vector3> caret, BetterList<Vector3> highlight)
    {
        if (String.IsNullOrEmpty(text))
        {
            text = " ";
        }
        NGUIText.Prepare(text);
        Int32 num = end;
        if (start > end)
        {
            end = start;
            start = num;
        }
        Single num2 = 0f;
        Single num3 = 0f;
        Single num4 = 0f;
        Single num5 = (Single)NGUIText.fontSize * NGUIText.fontScale;
        Int32 indexOffset = (Int32)((caret == null) ? 0 : caret.size);
        Int32 num6 = (Int32)((highlight == null) ? 0 : highlight.size);
        Int32 length = text.Length;
        Int32 i = 0;
        Int32 prev = 0;
        Boolean flag = false;
        Boolean flag2 = false;
        Int32 num7 = 0;
        Dialog.DialogImage dialogImage = (Dialog.DialogImage)null;
        Vector2 zero = Vector2.zero;
        Vector2 zero2 = Vector2.zero;
        while (i < length)
        {
            if (caret != null && !flag2 && num <= i)
            {
                flag2 = true;
                caret.Add(new Vector3(num2 - 1f, -num3 - num5 - 6f));
                caret.Add(new Vector3(num2 - 1f, -num3 + 6f));
                caret.Add(new Vector3(num2 + 1f, -num3 + 6f));
                caret.Add(new Vector3(num2 + 1f, -num3 - num5 - 6f));
            }
            Int32 num8 = (Int32)text[i];
            if (num8 == 10)
            {
                if (num2 > num4)
                {
                    num4 = num2;
                }
                if (caret != null && flag2)
                {
                    if (NGUIText.alignment != NGUIText.Alignment.Left)
                    {
                        NGUIText.Align(caret, indexOffset, num2 - NGUIText.finalSpacingX, 4);
                    }
                    caret = null;
                }
                if (highlight != null)
                {
                    if (flag)
                    {
                        flag = false;
                        highlight.Add(zero2);
                        highlight.Add(zero);
                    }
                    else if (start <= i && end > i)
                    {
                        highlight.Add(new Vector3(num2, -num3 - num5));
                        highlight.Add(new Vector3(num2, -num3));
                        highlight.Add(new Vector3(num2 + 2f, -num3));
                        highlight.Add(new Vector3(num2 + 2f, -num3 - num5));
                    }
                    if (NGUIText.alignment != NGUIText.Alignment.Left && num6 < highlight.size)
                    {
                        NGUIText.Align(highlight, num6, num2 - NGUIText.finalSpacingX, 4);
                        num6 = highlight.size;
                    }
                }
                num2 = 0f;
                num3 += NGUIText.finalLineHeight;
                prev = 0;
            }
            else if (num8 < 32)
            {
                prev = 0;
            }
            else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref i, ref num7, ref dialogImage))
            {
                i--;
            }
            else
            {
                BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
                Single num9 = (bmsymbol == null) ? NGUIText.GetGlyphWidth(num8, prev) : ((Single)bmsymbol.advance * NGUIText.fontScale);
                if (num9 != 0f)
                {
                    Single num10 = num2;
                    Single num11 = num2 + num9;
                    Single num12 = -num3 - num5 - 6f;
                    Single num13 = -num3 + 6f;
                    if (Mathf.RoundToInt(num11 + NGUIText.finalSpacingX) > NGUIText.regionWidth)
                    {
                        if (num2 == 0f)
                        {
                            return;
                        }
                        if (num2 > num4)
                        {
                            num4 = num2;
                        }
                        if (caret != null && flag2)
                        {
                            if (NGUIText.alignment != NGUIText.Alignment.Left)
                            {
                                NGUIText.Align(caret, indexOffset, num2 - NGUIText.finalSpacingX, 4);
                            }
                            caret = null;
                        }
                        if (highlight != null)
                        {
                            if (flag)
                            {
                                flag = false;
                                highlight.Add(zero2);
                                highlight.Add(zero);
                            }
                            else if (start <= i && end > i)
                            {
                                highlight.Add(new Vector3(num2, -num3 - num5));
                                highlight.Add(new Vector3(num2, -num3));
                                highlight.Add(new Vector3(num2 + 2f, -num3));
                                highlight.Add(new Vector3(num2 + 2f, -num3 - num5));
                            }
                            if (NGUIText.alignment != NGUIText.Alignment.Left && num6 < highlight.size)
                            {
                                NGUIText.Align(highlight, num6, num2 - NGUIText.finalSpacingX, 4);
                                num6 = highlight.size;
                            }
                        }
                        num10 -= num2;
                        num11 -= num2;
                        num12 -= NGUIText.finalLineHeight;
                        num13 -= NGUIText.finalLineHeight;
                        num2 = 0f;
                        num3 += NGUIText.finalLineHeight;
                    }
                    num2 += num9 + NGUIText.finalSpacingX;
                    if (highlight != null)
                    {
                        if (start > i || end <= i)
                        {
                            if (flag)
                            {
                                flag = false;
                                highlight.Add(zero2);
                                highlight.Add(zero);
                            }
                        }
                        else if (!flag)
                        {
                            flag = true;
                            highlight.Add(new Vector3(num10, num12));
                            highlight.Add(new Vector3(num10, num13));
                        }
                    }
                    zero = new Vector2(num11, num12);
                    zero2 = new Vector2(num11, num13);
                    prev = num8;
                }
            }
            i++;
        }
        if (caret != null)
        {
            if (!flag2)
            {
                caret.Add(new Vector3(num2 - 1f, -num3 - num5 - 6f));
                caret.Add(new Vector3(num2 - 1f, -num3 + 6f));
                caret.Add(new Vector3(num2 + 1f, -num3 + 6f));
                caret.Add(new Vector3(num2 + 1f, -num3 - num5 - 6f));
            }
            if (NGUIText.alignment != NGUIText.Alignment.Left)
            {
                NGUIText.Align(caret, indexOffset, num2 - NGUIText.finalSpacingX, 4);
            }
        }
        if (highlight != null)
        {
            if (flag)
            {
                highlight.Add(zero2);
                highlight.Add(zero);
            }
            else if (start < i && end == i)
            {
                highlight.Add(new Vector3(num2, -num3 - num5));
                highlight.Add(new Vector3(num2, -num3));
                highlight.Add(new Vector3(num2 + 2f, -num3));
                highlight.Add(new Vector3(num2 + 2f, -num3 - num5));
            }
            if (NGUIText.alignment != NGUIText.Alignment.Left && num6 < highlight.size)
            {
                NGUIText.Align(highlight, num6, num2 - NGUIText.finalSpacingX, 4);
            }
        }
    }

    [Conditional("NGUI_TEXT_DEBUG")]
    public static void DebugLog(params Object[] objs)
    {
    }

    public const Int32 FF9TIM_ID_APNUM_0 = 34;

    public const Int32 FF9TIM_ID_APNUM_1 = 35;

    public const Int32 FF9TIM_ID_APNUM_5 = 39;

    public const Int32 FF9TIM_ID_APNUM_SLUSH = 45;

    public const Int32 FF9TIM_ID_DMG_MISS = 159;

    public const Int32 FF9TIM_ID_DMG_DEATH = 160;

    public const Int32 FF9TIM_ID_DMG_GUARD = 161;

    public const Int32 FF9TIM_ID_DMG_CRITICAL = 162;

    public const Int32 FF9TIM_ID_DMG_MP = 163;

    public const Int32 FF9TIM_ID_DMG_9 = 173;

    public const Int32 FF9TIM_ID_DMG_SLASH = 174;

    public const Int32 FF9TIM_ID_DMG_CRITICAL_YELLOW = 179;

    public const String StartSentense = "STRT";
    public const String DialogId = "ID";
    public const String Choose = "CHOO";
    public const String AnimationTime = "TIME";
    public const String FlashInh = "FLIM";
    public const String NoAnimation = "NANI";
    public const String NoTypeEffect = "IMME";
    public const String MessageSpeed = "SPED";
    public const String Zidane = "ZDNE";
    public const String Vivi = "VIVI";
    public const String Dagger = "DGGR";
    public const String Steiner = "STNR";
    public const String Freya = "FRYA";
    public const String Quina = "QUIN";
    public const String Eiko = "EIKO";
    public const String Amarant = "AMRT";
    public const String Party1 = "PTY1";
    public const String Party2 = "PTY2";
    public const String Party3 = "PTY3";
    public const String Party4 = "PTY4";
    public const String Shadow = "HSHD";
    public const String NoShadow = "NSHD";
    public const String ButtonIcon = "DBTN";
    public const String NoFocus = "NFOC";
    public const String IncreaseSignal = "INCS";
    public const String CustomButtonIcon = "CBTN";
    public const String NewIcon = "PNEW";
    public const String TextOffset = "MOVE";
    public const String EndSentence = "ENDN";
    public const String TextVar = "TEXT";
    public const String ItemNameVar = "ITEM";
    public const String SignalVar = "SIGL";
    public const String NumberVar = "NUMB";
    public const String MessageDelay = "WAIT";
    public const String MessageFeed = "FEED";
    public const String MessageTab = "XTAB";
    public const String YAddOffset = "YADD";
    public const String YSubOffset = "YSUB";
    public const String IconVar = "ICON";
    public const String PreChoose = "PCHC";
    public const String PreChooseMask = "PCHM";
    public const String DialogAbsPosition = "MPOS";
    public const String DialogOffsetPositon = "OFFT";
    public const String DialogTailPositon = "TAIL";
    public const String TableStart = "TBLE";
    public const String WidthInfo = "WDTH";
    public const String Center = "CENT";
    public const String Signal = "SIGL";
    public const String NewPage = "PAGE";
    public const String MobileIcon = "MOBI";
    public const String SpacingY = "SPAY";
    public const String KeyboardButtonIcon = "KCBT";
    public const String JoyStickButtonIcon = "JCBT";
    public const String NoTurboDialog = "NTUR";

    public static readonly String[] RenderOpcodeSymbols;

    public static readonly String[] TextOffsetOpcodeSymbols;

    public static readonly List<Int32> IconIdException;

    public static readonly List<Char> CharException;

    public static readonly List<String> nameKeywordList;

    public static readonly Dictionary<String, CharacterId> nameCustomKeywords;

    public static readonly String FF9WhiteColor;

    public static readonly String FF9YellowColor;

    public static readonly String FF9PinkColor;

    public static readonly String FF9BlueColor;

    public static readonly Int32 MobileTouchToConfirmJP;

    public static readonly Int32 MobileTouchToConfirmUS;

    private static Boolean forceShowButton;

    public static UIFont bitmapFont;

    public static Font dynamicFont;

    public static NGUIText.GlyphInfo glyph = new NGUIText.GlyphInfo();

    public static Int32 fontSize = 16;

    public static Single fontScale = 1f;

    public static Single pixelDensity = 1f;

    public static FontStyle fontStyle = FontStyle.Normal;

    public static NGUIText.Alignment alignment = NGUIText.Alignment.Left;

    public static Color tint = Color.white;

    public static Int32 rectWidth = 1000000;

    public static Int32 rectHeight = 1000000;

    public static Int32 regionWidth = 1000000;

    public static Int32 regionHeight = 1000000;

    public static Int32 maxLines = 0;

    public static Boolean gradient = false;

    public static Color gradientBottom = Color.white;

    public static Color gradientTop = Color.white;

    public static Boolean encoding = false;

    public static Single spacingX = 0f;

    public static Single spacingY = 0f;

    public static Boolean premultiply = false;

    public static NGUIText.SymbolStyle symbolStyle;

    public static Int32 finalSize = 0;

    public static Single finalSpacingX = 0f;

    public static Single finalLineHeight = 0f;

    public static Single baseline = 0f;

    public static Boolean useSymbols = false;

    internal static Color mInvisible = new Color(0f, 0f, 0f, 0f);

    private static BetterList<Color> mColors = new BetterList<Color>();

    internal static Single mAlpha = 1f;

    private static CharacterInfo mTempChar;

    private static BetterList<Single> mSizes = new BetterList<Single>();

    private static Color32 s_c0;

    private static Color32 s_c1;

    private static Single[] mBoldOffset = new Single[]
    {
        -0.25f,
        0f,
        0.25f,
        0f,
        0f,
        -0.25f,
        0f,
        0.25f
    };

    public enum SignalMode
    {
        None,
        Set,
        Increase
    }

    public enum Alignment
    {
        Automatic,
        Left,
        Center,
        Right,
        Justified
    }

    public enum SymbolStyle
    {
        None,
        Normal,
        Colored
    }

    public class GlyphInfo
    {
        public Vector2 v0;

        public Vector2 v1;

        public Vector2 u0;

        public Vector2 u1;

        public Vector2 u2;

        public Vector2 u3;

        public Single advance;

        public Int32 channel;
    }
}