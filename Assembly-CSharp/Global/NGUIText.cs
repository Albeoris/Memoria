using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Object = System.Object;

public static class NGUIText
{
    static NGUIText()
    {
        // Note: this type is marked as 'beforefieldinit'.
        NGUIText.RenderOpcodeSymbols =
        [
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
            NGUIText.KeyboardButtonIcon,
            NGUIText.IconSprite
        ];
        NGUIText.TextOffsetOpcodeSymbols =
        [
            NGUIText.TextOffset,
            NGUIText.MessageFeed,
            NGUIText.MessageTab
        ];
        NGUIText.IconIdException =
        [
            19,  // arrow_up
            20,  // arrow_down
            192, // ap_bar_complete_star
            254, // ap_bar_full
            255  // ap_bar_half
        ];
        NGUIText.CharException = [' ', 'p', '-', 'y', ',', '一' ];
        NGUIText.nameKeywordList =
        [
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
        ];
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
        NGUIText.nameKeywordList.Add(keyword);
    }

    public static Boolean ForceShowButton
    {
        get => NGUIText.forceShowButton;
        set => NGUIText.forceShowButton = value;
    }

    public static Single GetTextWidthFromFF9Font(UILabel phraseLabel, String text)
    {
        phraseLabel.ProcessText();
        phraseLabel.UpdateNGUIText();
        NGUIText.Prepare(text);
        Single defaultCharacterSize = phraseLabel.fontSize * NGUIText.fontScale;
        Single textWidth = 0f;
        foreach (Char ch in text)
        {
            Single glyphWidth = NGUIText.GetGlyphWidth(ch, 0);
            textWidth += (glyphWidth <= 0f ? defaultCharacterSize : glyphWidth) + NGUIText.finalSpacingX;
        }
        return textWidth / UIManager.ResourceXMultipier + 1f;
    }

    public static Single GetDialogWidthFromSpecialOpcode(List<Int32> specialCodeList, ETb eTb, UILabel phraseLabel)
    {
        Single extraWidth = 0f;
        FF9StateGlobal ff = FF9StateSystem.Common.FF9;
        PLAYER[] member = ff.party.member;
        for (Int32 i = 0; i < specialCodeList.Count; i++)
        {
            Int32 specialCode = specialCodeList[i];
            switch (specialCode)
            {
                case 6: // [TEXT=TABLE,SCRIPTID]
                {
                    // TODO: that code seems fishy
                    UInt32 tableId = Convert.ToUInt32(specialCodeList[i + 1]);
                    if (tableId > Byte.MaxValue)
                    {
                        UInt32 textId = Convert.ToUInt32(specialCodeList[i + 2]);
                        String[] tableText = FF9TextTool.GetTableText(0u);
                        extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, tableText[textId]);
                        i += 2;
                    }
                    else
                    {
                        extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, eTb.GetStringFromTable(tableId >> 4 & 3u, tableId & 7u));
                        i++;
                    }
                    break;
                }
                case 14: // [ITEM=SCRIPTID]
                {
                    Int32 scriptId = specialCodeList[i + 1];
                    String itemName = ETb.GetItemName(eTb.gMesValue[scriptId]);
                    extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, itemName);
                    i++;
                    break;
                }
                case 16: // [ZDNE]
                    extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Zidane).Name);
                    break;
                case 17: // [VIVI]
                    extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Vivi).Name);
                    break;
                case 18: // [DGGR]
                    extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Garnet).Name);
                    break;
                case 19: // [STNR]
                    extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Steiner).Name);
                    break;
                case 20: // [FRYA]
                    extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Freya).Name);
                    break;
                case 21: // [QUIN]
                    extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Quina).Name);
                    break;
                case 22: // [EIKO]
                    extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Eiko).Name);
                    break;
                case 23: // [AMRT]
                    extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ff.GetPlayer(CharacterId.Amarant).Name);
                    break;
                case 24: // [PTY1]
                case 25: // [PTY2]
                case 26: // [PTY3]
                case 27: // [PTY4]
                {
                    extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, member[specialCode - 24]?.Name);
                    break;
                }
                case 64: // [NUMB=SCRIPTID]
                {
                    Int32 scriptId = specialCodeList[i + 1];
                    extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, eTb.gMesValue[scriptId].ToString());
                    i++;
                    break;
                }
                case 112: // [PNEW=SCRIPTID]
                    if ((eTb.gMesValue[0] & (1 << specialCodeList[++i])) != 0)
                        extraWidth += 30f;
                    break;
            }
        }
        return extraWidth;
    }

    public static void ProcessFF9Signal(ref Int32 ff9Signal, ref Int32 newSignal)
    {
        if (ff9Signal == 0)
            return;
        if (ff9Signal == 1 || (ff9Signal == 2 && newSignal != ETb.gMesSignal))
            ETb.gMesSignal = newSignal;
        else
            ETb.gMesSignal++;
        ff9Signal = 0;
        newSignal = 0;
    }

    public static void ProcessFF9Signal(Int32 ff9Signal)
    {
        if (ff9Signal >= 10)
            ETb.gMesSignal = ff9Signal % 10;
        else if (ff9Signal == 2)
            ETb.gMesSignal++;
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

    public static Dialog.DialogImage CreateSpriteImage(String spriteCode)
    {
        Dialog.DialogImage dialogImage = new Dialog.DialogImage();
        String[] args = spriteCode.Split(',');
        if (args.Length == 1)
        {
            dialogImage.AtlasName = "IconAtlas";
            dialogImage.SpriteName = args[0];
            dialogImage.Size = FF9UIDataTool.GetSpriteSize(dialogImage.AtlasName, dialogImage.SpriteName);
        }
        else if (args.Length == 2)
        {
            dialogImage.AtlasName = args[0];
            dialogImage.SpriteName = args[1];
            dialogImage.Size = FF9UIDataTool.GetSpriteSize(dialogImage.AtlasName, dialogImage.SpriteName);
        }
        else if (args.Length == 3)
        {
            dialogImage.AtlasName = "IconAtlas";
            dialogImage.SpriteName = args[0];
            dialogImage.Size = new Vector2();
            Single.TryParse(args[1], out dialogImage.Size.x);
            Single.TryParse(args[2], out dialogImage.Size.y);
            dialogImage.Rescale = true;
        }
        else if (args.Length == 4)
        {
            dialogImage.AtlasName = args[0];
            dialogImage.SpriteName = args[1];
            dialogImage.Size = new Vector2();
            Single.TryParse(args[2], out dialogImage.Size.x);
            Single.TryParse(args[3], out dialogImage.Size.y);
            dialogImage.Rescale = true;
        }
        dialogImage.Offset = new Vector3(0f, -10f);
        dialogImage.Id = -1;
        dialogImage.IsButton = false;
        return dialogImage;
    }

    public static Int32 GetOneParameterFromTag(String fullText, Int32 currentIndex, ref Int32 closingBracket)
    {
        return GetOneParameterFromTag(fullText.ToCharArray(), currentIndex, ref closingBracket);
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
        return GetAllParameters(fullText.ToCharArray(), currentIndex, ref closingBracket);
    }

    public static Single[] GetAllParameters(Char[] fullText, Int32 currentIndex, ref Int32 closingBracket)
    {
        closingBracket = Array.IndexOf(fullText, ']', currentIndex + 4);
        String paramText = new String(fullText, currentIndex + 6, closingBracket - currentIndex - 6);
        return Array.ConvertAll(paramText.Split(','), Single.Parse);
    }

    public static String ReplaceNumberValue(String phrase, Dialog dialog)
    {
        foreach (KeyValuePair<Int32, Int32> keyValuePair in dialog.MessageValues)
        {
            String value = keyValuePair.Value.ToString();
            phrase = phrase.ReplaceAll(
                [
                    new KeyValuePair<String, TextReplacement>($"[NUMB={keyValuePair.Key}]", value),
                    new KeyValuePair<String, TextReplacement>($"{{Variable {keyValuePair.Key}}}", value)
                ]);
        }
        return phrase;
    }

    public static Boolean ContainsTextOffset(String text)
    {
        Int32 offset = 0;
        Int32 left = text.Length;
        FFIXTextTag memoriaTag = FFIXTextTag.TryRead(text.ToCharArray(), ref offset, ref left);
        switch (memoriaTag?.Code)
        {
            case FFIXTextTagCode.DialogX:
            case FFIXTextTagCode.DialogY:
            case FFIXTextTagCode.DialogF:
                return true;
        }

        if (text[0] != '[' || text.Length < 5)
            return false;
        return NGUIText.TextOffsetOpcodeSymbols.Contains(text.Substring(1, 4));
    }

    public static Vector2 CalculatePrintedSize2(String text)
    {
        Vector2 printedSize = Vector2.zero;
        if (!String.IsNullOrEmpty(text))
        {
            if (NGUIText.encoding)
                text = NGUIText.StripSymbols2(text);
            NGUIText.mTextModifiers.Reset();
            NGUIText.Prepare(text);
            Single currentX = 0f;
            Single textHeight = 0f;
            Single maxLineWidth = 0f;
            Int32 textLength = text.Length;
            Int32 prevCh = 0;
            for (Int32 texti = 0; texti < textLength; texti++)
            {
                Int32 ch = text[texti];
                if (ch == '\n')
                {
                    if (currentX > maxLineWidth)
                        maxLineWidth = currentX;
                    currentX = 0f;
                    textHeight += NGUIText.finalLineHeight;
                    NGUIText.mTextModifiers.extraOffset = Vector3.zero;
                    NGUIText.mTextModifiers.insertImage = null;
                }
                else if (ch >= 32)
                {
                    if (NGUIText.encoding && DialogBoxSymbols.ParseSymbol(text, ref texti, NGUIText.premultiply, NGUIText.mTextModifiers))
                    {
                        texti--;
                    }
                    else
                    {
                        if (NGUIText.mTextModifiers.tabX != 0f)
                        {
                            NGUIText.mTextModifiers.extraOffset.x = 0f;
                            currentX = NGUIText.mTextModifiers.tabX;
                            NGUIText.mTextModifiers.tabX = 0f;
                        }
                        if (NGUIText.mTextModifiers.insertImage != null)
                        {
                            currentX += NGUIText.mTextModifiers.insertImage.Size.x - 20f;
                            NGUIText.mTextModifiers.insertImage = null;
                        }
                        if (NGUIText.mTextModifiers.extraOffset != Vector3.zero)
                        {
                            currentX += NGUIText.mTextModifiers.extraOffset.x;
                            NGUIText.mTextModifiers.extraOffset = Vector3.zero;
                        }
                        BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
                        if (bmsymbol == null)
                        {
                            Single glyphWidth = NGUIText.GetGlyphWidth(ch, prevCh);
                            if (glyphWidth != 0f)
                            {
                                glyphWidth += NGUIText.finalSpacingX;
                                if (Mathf.RoundToInt(currentX + glyphWidth) > NGUIText.regionWidth)
                                {
                                    if (currentX > maxLineWidth)
                                        maxLineWidth = currentX - NGUIText.finalSpacingX;
                                    currentX = glyphWidth;
                                    textHeight += NGUIText.finalLineHeight;
                                    NGUIText.mTextModifiers.extraOffset = Vector3.zero;
                                }
                                else
                                {
                                    currentX += glyphWidth;
                                }
                                prevCh = ch;
                            }
                        }
                        else
                        {
                            Single advanceX = NGUIText.finalSpacingX + bmsymbol.advance * NGUIText.fontScale;
                            if (Mathf.RoundToInt(currentX + advanceX) > NGUIText.regionWidth)
                            {
                                if (currentX > maxLineWidth)
                                    maxLineWidth = currentX - NGUIText.finalSpacingX;
                                currentX = advanceX;
                                textHeight += NGUIText.finalLineHeight;
                                NGUIText.mTextModifiers.extraOffset = Vector3.zero;
                            }
                            else
                            {
                                currentX += advanceX;
                            }
                            texti += bmsymbol.sequence.Length - 1;
                            prevCh = 0;
                        }
                    }
                }
            }
            printedSize.x = currentX <= maxLineWidth ? maxLineWidth : currentX - NGUIText.finalSpacingX;
            printedSize.y = textHeight + NGUIText.finalLineHeight;
        }
        return printedSize;
    }

    public static Single[] CalculateAllCharacterAdvances(String text)
    {
        NGUIText.Prepare(text);
        NGUIText.mTextModifiers.Reset();
        Int32 textLength = text.Length;
        Int32 prevCh = 0;
        Int32 nexti;
        Single[] charAdvances = new Single[textLength];
        for (Int32 texti = 0; texti < textLength; texti++)
        {
            Int32 ch = text[texti];
            if (ch == '\n')
            {
                NGUIText.mTextModifiers.extraOffset = Vector3.zero;
                NGUIText.mTextModifiers.insertImage = null;
            }
            else if (ch >= 32)
            {
                nexti = texti;
                if (NGUIText.encoding && DialogBoxSymbols.ParseSymbol(text, ref nexti, NGUIText.premultiply, NGUIText.mTextModifiers))
                {
                    charAdvances[texti] = NGUIText.mTextModifiers.extraOffset.x;
                    if (NGUIText.mTextModifiers.insertImage != null)
                        charAdvances[texti] += NGUIText.mTextModifiers.insertImage.Size.x - 20f;
                    texti = nexti - 1;
                }
                else
                {
                    BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
                    if (bmsymbol == null)
                    {
                        Single glyphWidth = NGUIText.GetGlyphWidth(ch, prevCh);
                        if (glyphWidth != 0f)
                        {
                            glyphWidth += NGUIText.finalSpacingX;
                            charAdvances[texti] = NGUIText.mTextModifiers.sub != 0 ? glyphWidth * 0.75f : glyphWidth;
                            prevCh = ch;
                        }
                    }
                    else
                    {
                        charAdvances[texti] = NGUIText.finalSpacingX + bmsymbol.advance * NGUIText.fontScale;
                        texti += bmsymbol.sequence.Length - 1;
                        prevCh = 0;
                    }
                }
            }
        }
        return charAdvances;
    }

    public static String StripSymbols2(String text)
    {
        if (text != null)
        {
            Int32 textLength = text.Length;
            for (Int32 texti = 0; texti < textLength; texti++)
            {
                Char ch = text[texti];
                if (ch == '[' || ch == '{')
                {
                    NGUIText.mTextModifiers.Reset();
                    Int32 endOfSymbol = texti;
                    if (DialogBoxSymbols.ParseSymbol(text, ref endOfSymbol, false, NGUIText.mTextModifiers))
                    {
                        String opcodeText = text.Substring(texti, endOfSymbol - texti);
                        if (!NGUIText.ContainsTextOffset(opcodeText) && NGUIText.mTextModifiers.insertImage == null)
                        {
                            text = text.Remove(texti, endOfSymbol - texti);
                            textLength = text.Length;
                        }
                        else
                        {
                            texti = endOfSymbol - 1;
                        }
                    }
                }
            }
        }
        return text;
    }

    public static void SetIconDepth(GameObject phaseLabel, GameObject iconObject, Boolean isLowerPhrase = true)
    {
        Int32 depth = phaseLabel.GetComponent<UIWidget>().depth;
        depth = isLowerPhrase ? depth - iconObject.transform.childCount - 1 : depth + 1;
        iconObject.GetComponent<UIWidget>().depth = depth++;
        foreach (Transform transform in iconObject.transform)
            transform.GetComponent<UIWidget>().depth = depth++;
    }

    private static void AlignImageWithLastChar(ref BetterList<Dialog.DialogImage> specialImages, BetterList<Int32> imageAlignmentList, BetterList<Vector3> verts, Int32 printedLine)
    {
        if (imageAlignmentList.size == 0)
            return;
        Single charPos = verts.size >= 2 ? verts[verts.size - 2].y : 0f;
        foreach (Int32 i in imageAlignmentList)
        {
            Dialog.DialogImage dialogImage = specialImages[i];
            if (NGUIText.AlignImageCondition(dialogImage) && printedLine == dialogImage.PrintedLine)
                dialogImage.LocalPosition.y = charPos + dialogImage.Size.y + dialogImage.Offset.y;
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
            if (dialogImage.PrintedLine == printedLine)
                dialogImage.LocalPosition += padding;
    }

    private static void AlignImage(BetterList<Vector3> verts, Int32 indexOffset, Single printedWidth, BetterList<Dialog.DialogImage> imageList, Int32 printedLine)
    {
        Alignment bidiAlignement = NGUIText.alignment;
        if (NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft && bidiAlignement == Alignment.Left)
            bidiAlignement = Alignment.Right;
        switch (bidiAlignement)
        {
            case NGUIText.Alignment.Left:
            {
                /*if (verts.size == 0 || verts.size <= indexOffset)
                    return;
                if (verts.buffer != null)
                {
                    Single halfEmptySpace = (NGUIText.rectWidth - printedWidth) * 0.5f;
                    Int32 emptySpace = Mathf.RoundToInt(NGUIText.rectWidth - printedWidth);
                    Int32 allowedWidth = Mathf.RoundToInt(NGUIText.rectWidth);
                    Boolean emptySpaceOdd = (emptySpace & 1) == 1;
                    Boolean allowedWidthOdd = (allowedWidth & 1) == 1;
                    if (emptySpaceOdd != allowedWidthOdd)
                        halfEmptySpace += 0.5f * NGUIText.fontScale;
                    Single positiveEmptySpace = Math.Max(0f, emptySpace);
                    if (positiveEmptySpace < 0f)
                        NGUIText.AlignImageWithPadding(ref imageList, new Vector3(positiveEmptySpace, 0f), printedLine);
                }*/
                break;
            }
            case NGUIText.Alignment.Center:
            {
                Single halfEmptySpace = (NGUIText.rectWidth - printedWidth) * 0.5f;
                if (halfEmptySpace < 0f)
                    return;
                Int32 emptySpace = Mathf.RoundToInt(NGUIText.rectWidth - printedWidth);
                Int32 allowedWidth = Mathf.RoundToInt(NGUIText.rectWidth);
                Boolean emptySpaceOdd = (emptySpace & 1) == 1;
                Boolean allowedWidthOdd = (allowedWidth & 1) == 1;
                if (emptySpaceOdd != allowedWidthOdd)
                    halfEmptySpace += 0.5f * NGUIText.fontScale;
                NGUIText.AlignImageWithPadding(ref imageList, new Vector3(halfEmptySpace, 0f), printedLine);
                break;
            }
        }
    }

    private static void AddSpecialIconToList(ref BetterList<Dialog.DialogImage> specialImages, ref BetterList<Int32> imageAlignmentList, ref Dialog.DialogImage insertImage, Vector3 extraOffset, ref Single currentX, Single currentY, Int32 printedLine)
    {
        if (insertImage != null)
        {
            insertImage.LocalPosition = new Vector3(currentX + extraOffset.x, extraOffset.y - currentY);
            insertImage.PrintedLine = printedLine;
            imageAlignmentList.Add(specialImages.size);
            specialImages.Add(insertImage);
            currentX += insertImage.Size.x - 20f;
            insertImage = null;
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
        NGUIText.finalSize = Mathf.RoundToInt(NGUIText.fontSize / NGUIText.pixelDensity);
        NGUIText.finalSpacingX = NGUIText.spacingX * NGUIText.fontScale;
        NGUIText.finalLineHeight = (NGUIText.fontSize + NGUIText.spacingY) * NGUIText.fontScale;
        NGUIText.useSymbols = NGUIText.bitmapFont != null && NGUIText.bitmapFont.hasSymbols && NGUIText.encoding && NGUIText.symbolStyle != NGUIText.SymbolStyle.None;
        if (request && NGUIText.dynamicFont != null)
        {
            NGUIText.dynamicFont.RequestCharactersInTexture(")_-", NGUIText.finalSize, NGUIText.fontStyle);
            if (!NGUIText.dynamicFont.GetCharacterInfo(')', out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle) || NGUIText.mTempChar.maxY == 0f)
            {
                NGUIText.dynamicFont.RequestCharactersInTexture("A", NGUIText.finalSize, NGUIText.fontStyle);
                if (!NGUIText.dynamicFont.GetCharacterInfo('A', out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle))
                {
                    NGUIText.baseline = 0f;
                    return;
                }
            }
            NGUIText.baseline = Mathf.Round((NGUIText.mTempChar.minY + NGUIText.mTempChar.maxY + NGUIText.finalSize) * 0.5f);
        }
    }

    public static void Prepare(String text)
    {
        if (NGUIText.dynamicFont != null)
            NGUIText.dynamicFont.RequestCharactersInTexture(text, NGUIText.finalSize, NGUIText.fontStyle);
    }

    public static BMSymbol GetSymbol(String text, Int32 index, Int32 textLength)
    {
        return NGUIText.bitmapFont == null ? null : NGUIText.bitmapFont.MatchSymbol(text, index, textLength);
    }

    public static Single GetGlyphWidth(Int32 ch, Int32 prev)
    {
        if (NGUIText.bitmapFont != null)
        {
            Boolean isThinSpace = false;
            if (ch == 0x2009) // Thin space
            {
                isThinSpace = true;
                ch = ' ';
            }
            BMGlyph bmglyph = NGUIText.bitmapFont.bmFont.GetGlyph(ch);
            if (bmglyph != null)
            {
                Int32 chAdvance = bmglyph.advance;
                if (isThinSpace)
                    chAdvance >>= 1;
                return NGUIText.fontScale * (prev == 0 ? bmglyph.advance : chAdvance + bmglyph.GetKerning(prev));
            }
        }
        else if (NGUIText.dynamicFont != null && NGUIText.dynamicFont.GetCharacterInfo((Char)ch, out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle))
        {
            return NGUIText.mTempChar.advance * NGUIText.fontScale * NGUIText.pixelDensity;
        }
        return 0f;
    }

    public static NGUIText.GlyphInfo GetGlyph(Int32 ch, Int32 prev)
    {
        if (NGUIText.bitmapFont != null)
        {
            Boolean isThinSpace = false;
            if (ch == 0x2009) // Thin space
            {
                isThinSpace = true;
                ch = ' ';
            }
            BMGlyph bmglyph = NGUIText.bitmapFont.bmFont.GetGlyph(ch);
            if (bmglyph != null)
            {
                Int32 prevAdvance = prev == 0 ? 0 : bmglyph.GetKerning(prev);
                NGUIText.glyph.v0.x = prev == 0 ? bmglyph.offsetX : bmglyph.offsetX + prevAdvance;
                NGUIText.glyph.v1.y = -bmglyph.offsetY;
                NGUIText.glyph.v1.x = NGUIText.glyph.v0.x + bmglyph.width;
                NGUIText.glyph.v0.y = NGUIText.glyph.v1.y - bmglyph.height;
                NGUIText.glyph.u0.x = bmglyph.x;
                NGUIText.glyph.u0.y = bmglyph.y + bmglyph.height;
                NGUIText.glyph.u2.x = bmglyph.x + bmglyph.width;
                NGUIText.glyph.u2.y = bmglyph.y;
                NGUIText.glyph.u1.x = NGUIText.glyph.u0.x;
                NGUIText.glyph.u1.y = NGUIText.glyph.u2.y;
                NGUIText.glyph.u3.x = NGUIText.glyph.u2.x;
                NGUIText.glyph.u3.y = NGUIText.glyph.u0.y;
                Int32 chAdvance = bmglyph.advance;
                if (isThinSpace)
                    chAdvance >>= 1;
                NGUIText.glyph.advance = chAdvance + prevAdvance;
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
        else if (NGUIText.dynamicFont != null && NGUIText.dynamicFont.GetCharacterInfo((Char)ch, out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle))
        {
            NGUIText.glyph.v0.x = NGUIText.mTempChar.minX;
            NGUIText.glyph.v1.x = NGUIText.mTempChar.maxX;
            NGUIText.glyph.v0.y = NGUIText.mTempChar.maxY - NGUIText.baseline;
            NGUIText.glyph.v1.y = NGUIText.mTempChar.minY - NGUIText.baseline;
            NGUIText.glyph.u0 = NGUIText.mTempChar.uvTopLeft;
            NGUIText.glyph.u1 = NGUIText.mTempChar.uvBottomLeft;
            NGUIText.glyph.u2 = NGUIText.mTempChar.uvBottomRight;
            NGUIText.glyph.u3 = NGUIText.mTempChar.uvTopRight;
            NGUIText.glyph.advance = NGUIText.mTempChar.advance;
            NGUIText.glyph.channel = 0;
            NGUIText.glyph.v0.x = Mathf.Round(NGUIText.glyph.v0.x);
            NGUIText.glyph.v0.y = Mathf.Round(NGUIText.glyph.v0.y);
            NGUIText.glyph.v1.x = Mathf.Round(NGUIText.glyph.v1.x);
            NGUIText.glyph.v1.y = Mathf.Round(NGUIText.glyph.v1.y);
            Single pixelScale = NGUIText.fontScale * NGUIText.pixelDensity;
            if (pixelScale != 1f)
            {
                NGUIText.glyph.v0 *= pixelScale;
                NGUIText.glyph.v1 *= pixelScale;
                NGUIText.glyph.advance *= pixelScale;
            }
            return NGUIText.glyph;
        }
        return null;
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static Single ParseAlpha(String text, Int32 index)
    {
        Int32 alpha = NGUIMath.HexToDecimal(text[index + 1]) << 4 | NGUIMath.HexToDecimal(text[index + 2]);
        return Mathf.Clamp01(alpha / 255f);
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
        Int32 r = NGUIMath.HexToDecimal(text[offset]) << 4 | NGUIMath.HexToDecimal(text[offset + 1]);
        Int32 g = NGUIMath.HexToDecimal(text[offset + 2]) << 4 | NGUIMath.HexToDecimal(text[offset + 3]);
        Int32 b = NGUIMath.HexToDecimal(text[offset + 4]) << 4 | NGUIMath.HexToDecimal(text[offset + 5]);
        Single f = 0.003921569f; // 1/255
        return new Color(f * r, f * g, f * b);
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static Color ParseColor32(String text, Int32 offset)
    {
        Int32 r = NGUIMath.HexToDecimal(text[offset]) << 4 | NGUIMath.HexToDecimal(text[offset + 1]);
        Int32 g = NGUIMath.HexToDecimal(text[offset + 2]) << 4 | NGUIMath.HexToDecimal(text[offset + 3]);
        Int32 b = NGUIMath.HexToDecimal(text[offset + 4]) << 4 | NGUIMath.HexToDecimal(text[offset + 5]);
        Int32 a = NGUIMath.HexToDecimal(text[offset + 6]) << 4 | NGUIMath.HexToDecimal(text[offset + 7]);
        Single f = 0.003921569f; // 1/255
        return new Color(f * r, f * g, f * b, f * a);
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
        return $"[c][{NGUIText.EncodeColor24(c)}]{text}[-][/c]";
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static String EncodeAlpha(Single a)
    {
        Int32 alpha = Mathf.Clamp(Mathf.RoundToInt(a * 255f), 0, 255);
        return NGUIMath.DecimalToHex8(alpha);
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static String EncodeColor24(Color c)
    {
        Int32 colorCode = 0xFFFFFF & NGUIMath.ColorToInt(c) >> 8;
        return NGUIMath.DecimalToHex24(colorCode);
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static String EncodeColor32(Color c)
    {
        Int32 colorCode = NGUIMath.ColorToInt(c);
        return NGUIMath.DecimalToHex32(colorCode);
    }

    private static Boolean ParseSymbol(String text, ref Int32 index, FFIXTextModifier modifiers)
    {
        return DialogBoxSymbols.ParseSymbol(text, ref index, false, modifiers);
    }

    public static Dialog.DialogImage GetNextDialogImage(String text, ref Int32 index)
    {
        NGUIText.mTextModifiers.Reset();
        while (DialogBoxSymbols.ParseSymbol(text, ref index, false, NGUIText.mTextModifiers)) { }
        return NGUIText.mTextModifiers.insertImage;
    }

    public static Int32 GetNextFF9Signal(String text, ref Int32 index)
    {
        NGUIText.mTextModifiers.Reset();
        while (DialogBoxSymbols.ParseSymbol(text, ref index, false, NGUIText.mTextModifiers)) { }
        return NGUIText.mTextModifiers.ff9Signal;
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
            Int32 textLength = text.Length;
            for (Int32 texti = 0; texti < textLength; texti++)
            {
                Char ch = text[texti];
                if (ch == '[' || ch == '{')
                {
                    Int32 endOfSymbol = texti;
                    NGUIText.mTextModifiers.Reset();
                    if (DialogBoxSymbols.ParseSymbol(text, ref endOfSymbol, false, NGUIText.mTextModifiers))
                    {
                        text = text.Remove(texti, endOfSymbol - texti);
                        textLength = text.Length;
                    }
                }
            }
        }
        return text;
    }

    public static void Align(BetterList<Vector3> verts, Int32 indexOffset, Single printedWidth, Int32 vertCountByCharacter = 4)
    {
        if (verts.size == 0 || verts.size <= indexOffset)
            return;
        Alignment bidiAlignement = NGUIText.alignment;
        if (NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft && bidiAlignement == Alignment.Left)
            bidiAlignement = Alignment.Right;
        switch (bidiAlignement)
        {
            case NGUIText.Alignment.Left:
            {
                /*if (verts.buffer != null)
                {
                    Single halfEmptySpace = (NGUIText.rectWidth - printedWidth) * 0.5f;
                    Int32 emptySpace = Mathf.RoundToInt(NGUIText.rectWidth - printedWidth);
                    Int32 allowedWidth = Mathf.RoundToInt(NGUIText.rectWidth);
                    Boolean emptySpaceOdd = (emptySpace & 1) == 1;
                    Boolean allowedWidthOdd = (allowedWidth & 1) == 1;
                    if (emptySpaceOdd != allowedWidthOdd)
                        halfEmptySpace += 0.5f * NGUIText.fontScale;
                    Single positiveEmptySpace = Math.Max(0f, emptySpace);
                    if (positiveEmptySpace < 0f)
                        for (Int32 i = indexOffset; i < verts.size; i++)
                            verts.buffer[i].x += positiveEmptySpace;
                }*/
                break;
            }
            case NGUIText.Alignment.Center:
            {
                Single halfEmptySpace = (NGUIText.rectWidth - printedWidth) * 0.5f;
                if (halfEmptySpace < 0f)
                    return;
                Int32 emptySpace = Mathf.RoundToInt(NGUIText.rectWidth - printedWidth);
                Int32 allowedWidth = Mathf.RoundToInt(NGUIText.rectWidth);
                Boolean emptySpaceOdd = (emptySpace & 1) == 1;
                Boolean allowedWidthOdd = (allowedWidth & 1) == 1;
                if (emptySpaceOdd != allowedWidthOdd)
                    halfEmptySpace += 0.5f * NGUIText.fontScale;
                for (Int32 i = indexOffset; i < verts.size; i++)
                    verts.buffer[i].x += halfEmptySpace;
                break;
            }
            case NGUIText.Alignment.Right:
            {
                Single emptySpace = NGUIText.rectWidth - printedWidth;
                if (emptySpace < 0f)
                    return;
                for (Int32 i = indexOffset; i < verts.size; i++)
                    verts.buffer[i].x += emptySpace;
                break;
            }
            case NGUIText.Alignment.Justified:
            {
                if (printedWidth < NGUIText.rectWidth * 0.65f)
                {
                    if (NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft)
                        goto case NGUIText.Alignment.Right;
                    return;
                }
                Single margin = (NGUIText.rectWidth - printedWidth) * 0.5f;
                if (margin < 1f)
                    return;

                Int32 padding = (verts.size - indexOffset) / vertCountByCharacter;
                if (padding < 2)
                    return;

                Single paddingFactor = 1f / (padding - 1);
                Single widthRatio = NGUIText.rectWidth / printedWidth;
                Int32 counter = 1;
                Int32 i = indexOffset + vertCountByCharacter;
                while (i < verts.size)
                {
                    Single currentX1 = verts.buffer[i].x;
                    Single currentX2 = verts.buffer[i + vertCountByCharacter / 2].x;
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
                    if (vertCountByCharacter == 4)
                    {
                        verts.buffer[i++].x = currentX1;
                        verts.buffer[i++].x = currentX1;
                        verts.buffer[i++].x = currentX2;
                        verts.buffer[i++].x = currentX2;
                    }
                    else if (vertCountByCharacter == 2)
                    {
                        verts.buffer[i++].x = currentX1;
                        verts.buffer[i++].x = currentX2;
                    }
                    else if (vertCountByCharacter == 1)
                    {
                        verts.buffer[i++].x = currentX1;
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
            Int32 vmin = i << 1;
            Int32 vmax = vmin + 1;
            if (pos.x >= verts[vmin].x && pos.x <= verts[vmax].x && pos.y >= verts[vmin].y && pos.y <= verts[vmax].y)
                return indices[i];
        }
        return 0;
    }

    public static Int32 GetApproximateCharacterIndex(BetterList<Vector3> verts, BetterList<Int32> indices, Vector2 pos)
    {
        Single closestRowDist = Single.MaxValue;
        Single closestLineDist = Single.MaxValue;
        Int32 closestIndex = 0;
        for (Int32 i = 0; i < verts.size; i++)
        {
            Single lineDist = Mathf.Abs(pos.y - verts[i].y);
            if (lineDist <= closestLineDist)
            {
                Single rowDist = Mathf.Abs(pos.x - verts[i].x);
                if (lineDist < closestLineDist)
                {
                    closestLineDist = lineDist;
                    closestRowDist = rowDist;
                    closestIndex = i;
                }
                else if (rowDist < closestRowDist)
                {
                    closestRowDist = rowDist;
                    closestIndex = i;
                }
            }
        }
        return indices[closestIndex];
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    private static Boolean IsSpace(Int32 ch)
    {
        return ch == ' ' || ch == 0x2009 || ch == 0x200A || ch == 0x200B; // Thin space, Hair space and Zero-width space
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void EndLine(ref StringBuilder s)
    {
        Int32 lastPos = s.Length - 1;
        if (lastPos > 0 && NGUIText.IsSpace(s[lastPos]))
            s[lastPos] = '\n';
        else
            s.Append('\n');
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    private static void ReplaceSpaceWithNewline(ref StringBuilder s)
    {
        Int32 lastPos = s.Length - 1;
        if (lastPos > 0 && NGUIText.IsSpace(s[lastPos]))
            s[lastPos] = '\n';
    }

    public static Vector2 CalculatePrintedSize(String text)
    {
        Vector2 printedSize = Vector2.zero;
        if (!String.IsNullOrEmpty(text))
        {
            if (NGUIText.encoding)
                text = NGUIText.StripSymbols(text);
            NGUIText.Prepare(text);
            Single currentX = 0f;
            Single textHeight = 0f;
            Single maxLineWidth = 0f;
            Int32 textLength = text.Length;
            Int32 prevCh = 0;
            for (Int32 texti = 0; texti < textLength; texti++)
            {
                Int32 ch = text[texti];
                if (ch == '\n')
                {
                    if (currentX > maxLineWidth)
                        maxLineWidth = currentX;
                    currentX = 0f;
                    textHeight += NGUIText.finalLineHeight;
                }
                else if (ch >= 32)
                {
                    BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
                    if (bmsymbol == null)
                    {
                        Single glyphWidth = NGUIText.GetGlyphWidth(ch, prevCh);
                        if (glyphWidth != 0f)
                        {
                            glyphWidth += NGUIText.finalSpacingX;
                            if (Mathf.RoundToInt(currentX + glyphWidth) > NGUIText.regionWidth)
                            {
                                if (currentX > maxLineWidth)
                                    maxLineWidth = currentX - NGUIText.finalSpacingX;
                                currentX = glyphWidth;
                                textHeight += NGUIText.finalLineHeight;
                            }
                            else
                            {
                                currentX += glyphWidth;
                            }
                            prevCh = ch;
                        }
                    }
                    else
                    {
                        Single advanceX = NGUIText.finalSpacingX + bmsymbol.advance * NGUIText.fontScale;
                        if (Mathf.RoundToInt(currentX + advanceX) > NGUIText.regionWidth)
                        {
                            if (currentX > maxLineWidth)
                                maxLineWidth = currentX - NGUIText.finalSpacingX;
                            currentX = advanceX;
                            textHeight += NGUIText.finalLineHeight;
                        }
                        else
                        {
                            currentX += advanceX;
                        }
                        texti += bmsymbol.sequence.Length - 1;
                        prevCh = 0;
                    }
                }
            }
            printedSize.x = currentX <= maxLineWidth ? maxLineWidth : currentX - NGUIText.finalSpacingX;
            printedSize.y = textHeight + NGUIText.finalLineHeight;
        }
        return printedSize;
    }

    public static Int32 CalculateOffsetToFit(String text)
    {
        if (String.IsNullOrEmpty(text) || NGUIText.regionWidth < 1)
            return 0;
        NGUIText.Prepare(text);
        Int32 textLength = text.Length;
        Int32 prevCh = 0;
        for (Int32 texti = 0; texti < textLength; texti++)
        {
            BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
            if (bmsymbol == null)
            {
                Int32 ch = text[texti];
                Single glyphWidth = NGUIText.GetGlyphWidth(ch, prevCh);
                if (glyphWidth != 0f)
                    NGUIText.mSizes.Add(NGUIText.finalSpacingX + glyphWidth);
                prevCh = ch;
            }
            else
            {
                NGUIText.mSizes.Add(NGUIText.finalSpacingX + bmsymbol.advance * NGUIText.fontScale);
                Int32 symbolSeqLength = bmsymbol.sequence.Length - 1;
                for (Int32 i = 0; i < symbolSeqLength; i++)
                    NGUIText.mSizes.Add(0f);
                texti += bmsymbol.sequence.Length - 1;
                prevCh = 0;
            }
        }
        Single availableWidth = NGUIText.regionWidth;
        Int32 overlimitCharacters = NGUIText.mSizes.size;
        while (overlimitCharacters > 0 && availableWidth > 0f)
            availableWidth -= NGUIText.mSizes[--overlimitCharacters];
        NGUIText.mSizes.Clear();
        if (availableWidth < 0f)
            overlimitCharacters++;
        return overlimitCharacters;
    }

    public static String GetEndOfLineThatFits(String text)
    {
        Int32 charToRemove = NGUIText.CalculateOffsetToFit(text);
        return text.Substring(charToRemove, text.Length - charToRemove);
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
        Single maxHeight = NGUIText.maxLines <= 0 ? NGUIText.regionHeight : Mathf.Min(NGUIText.regionHeight, NGUIText.finalLineHeight * NGUIText.maxLines);
        Int32 maxLineCount = NGUIText.maxLines <= 0 ? 1000000 : NGUIText.maxLines;
        maxLineCount = Mathf.FloorToInt(Mathf.Min(maxLineCount, maxHeight / NGUIText.finalLineHeight) + 0.01f);
        if (maxLineCount == 0)
        {
            finalText = String.Empty;
            return false;
        }
        if (String.IsNullOrEmpty(text))
            text = " ";
        NGUIText.Prepare(text);
        StringBuilder finalTextBuilder = new StringBuilder();
        Int32 textLength = text.Length;
        Single availableWidth = NGUIText.regionWidth;
        Int32 lastPossibleBreakPos = 0;
        Int32 currentLineCount = 1;
        Int32 prevCh = 0;
        Boolean canBreakNow = true;
        Boolean wordsPreserved = true;
        Boolean isSpecialCharacter = false;
        Color textColor = NGUIText.tint;
        NGUIText.mTextModifiers.Reset();
        BetterList<Color> colorList = NGUIText.mTextModifiers.colors;
        if (!NGUIText.useSymbols)
            wrapLineColors = false;
        if (wrapLineColors)
            colorList.Add(textColor);
        Int32 texti;
        for (texti = 0; texti < textLength; texti++)
        {
            Char ch = text[texti];
            if (ch > 0x2FFF)
                isSpecialCharacter = true;
            if (ch == '\n')
            {
                if (currentLineCount == maxLineCount)
                    break;
                availableWidth = NGUIText.regionWidth;
                if (lastPossibleBreakPos < texti)
                    finalTextBuilder.Append(text.Substring(lastPossibleBreakPos, texti - lastPossibleBreakPos + 1));
                else
                    finalTextBuilder.Append(ch);
                if (wrapLineColors)
                {
                    for (Int32 i = 0; i < colorList.size; i++)
                        finalTextBuilder.Insert(finalTextBuilder.Length - 1, "[-]");
                    for (Int32 i = 0; i < colorList.size; i++)
                    {
                        finalTextBuilder.Append("[");
                        finalTextBuilder.Append(NGUIText.EncodeColor(colorList[i]));
                        finalTextBuilder.Append("]");
                    }
                }
                canBreakNow = true;
                currentLineCount++;
                lastPossibleBreakPos = texti + 1;
                prevCh = 0;
            }
            else
            {
                if (NGUIText.encoding)
                {
                    if (!wrapLineColors)
                    {
                        if (NGUIText.ParseSymbol(text, ref texti, NGUIText.mTextModifiers))
                        {
                            texti--;
                            continue;
                        }
                    }
                    else if (DialogBoxSymbols.ParseSymbol(text, ref texti, NGUIText.premultiply, NGUIText.mTextModifiers))
                    {
                        if (NGUIText.mTextModifiers.ignoreColor)
                        {
                            textColor = colorList[colorList.size - 1];
                            textColor.a *= NGUIText.mAlpha * NGUIText.tint.a;
                        }
                        else
                        {
                            textColor = NGUIText.tint * colorList[colorList.size - 1];
                            textColor.a *= NGUIText.mAlpha;
                        }
                        for (Int32 i = 0; i < colorList.size - 2; i++)
                            textColor.a *= colorList[i].a;
                        texti--;
                        continue;
                    }
                }
                BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
                Single advanceX;
                if (bmsymbol == null)
                {
                    Single glyphWidth = NGUIText.GetGlyphWidth(ch, prevCh);
                    if (glyphWidth == 0f)
                        continue;
                    advanceX = NGUIText.finalSpacingX + glyphWidth;
                }
                else
                {
                    advanceX = NGUIText.finalSpacingX + bmsymbol.advance * NGUIText.fontScale;
                }
                availableWidth -= advanceX;
                Boolean isSpace = NGUIText.IsSpace(ch);
                if (isSpace && !isSpecialCharacter && lastPossibleBreakPos < texti)
                {
                    Int32 copyLength = texti - lastPossibleBreakPos + 1;
                    if (currentLineCount == maxLineCount && availableWidth <= 0f && texti < textLength && (ch < ' ' || isSpace))
                        copyLength--;
                    finalTextBuilder.Append(text.Substring(lastPossibleBreakPos, copyLength));
                    canBreakNow = false;
                    lastPossibleBreakPos = texti + 1;
                }
                if (Mathf.RoundToInt(availableWidth) < 0)
                {
                    if (canBreakNow || currentLineCount == maxLineCount)
                    {
                        finalTextBuilder.Append(text.Substring(lastPossibleBreakPos, Mathf.Max(0, texti - lastPossibleBreakPos)));
                        if (!isSpace && !isSpecialCharacter)
                            wordsPreserved = false;
                        if (wrapLineColors && colorList.size > 0)
                            finalTextBuilder.Append("[-]");
                        if (currentLineCount++ == maxLineCount)
                        {
                            lastPossibleBreakPos = texti;
                            break;
                        }
                        if (keepCharCount)
                            NGUIText.ReplaceSpaceWithNewline(ref finalTextBuilder);
                        else
                            NGUIText.EndLine(ref finalTextBuilder);
                        if (wrapLineColors)
                        {
                            for (Int32 i = 0; i < colorList.size; i++)
                                finalTextBuilder.Insert(finalTextBuilder.Length - 1, "[-]");
                            for (Int32 i = 0; i < colorList.size; i++)
                            {
                                finalTextBuilder.Append("[");
                                finalTextBuilder.Append(NGUIText.EncodeColor(colorList[i]));
                                finalTextBuilder.Append("]");
                            }
                        }
                        canBreakNow = true;
                        if (isSpace)
                        {
                            lastPossibleBreakPos = texti + 1;
                            availableWidth = NGUIText.regionWidth;
                        }
                        else
                        {
                            lastPossibleBreakPos = texti;
                            availableWidth = NGUIText.regionWidth - advanceX;
                        }
                        prevCh = 0;
                    }
                    else
                    {
                        canBreakNow = true;
                        availableWidth = NGUIText.regionWidth;
                        texti = lastPossibleBreakPos - 1;
                        prevCh = 0;
                        if (currentLineCount++ == maxLineCount)
                            break;
                        if (keepCharCount)
                            NGUIText.ReplaceSpaceWithNewline(ref finalTextBuilder);
                        else
                            NGUIText.EndLine(ref finalTextBuilder);
                        if (wrapLineColors)
                        {
                            for (Int32 i = 0; i < colorList.size; i++)
                                finalTextBuilder.Insert(finalTextBuilder.Length - 1, "[-]");
                            for (Int32 i = 0; i < colorList.size; i++)
                            {
                                finalTextBuilder.Append("[");
                                finalTextBuilder.Append(NGUIText.EncodeColor(colorList[i]));
                                finalTextBuilder.Append("]");
                            }
                        }
                        continue;
                    }
                }
                else
                {
                    prevCh = ch;
                }
                if (bmsymbol != null)
                {
                    texti += bmsymbol.length - 1;
                    prevCh = 0;
                }
            }
        }
        if (lastPossibleBreakPos < texti)
            finalTextBuilder.Append(text.Substring(lastPossibleBreakPos, texti - lastPossibleBreakPos));
        if (wrapLineColors && colorList.size > 0)
            finalTextBuilder.Append("[-]");
        finalText = finalTextBuilder.ToString();
        colorList.Clear();
        return wordsPreserved && (texti == textLength || currentLineCount <= Mathf.Min(NGUIText.maxLines, maxLineCount));
    }

    public static void Print(String text, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, out BetterList<Int32> highShadowVertIndexes, out BetterList<Dialog.DialogImage> specialImages, out BetterList<Int32> vertsLineOffsets)
    {
        highShadowVertIndexes = new BetterList<Int32>();
        specialImages = new BetterList<Dialog.DialogImage>();
        vertsLineOffsets = new BetterList<Int32>();
        if (String.IsNullOrEmpty(text))
            return;
        Int32 vIndex = verts.size;
        NGUIText.Prepare(text);
        Int32 prevCh = 0;
        Int32 currCh = 0;
        Single currentX = 0f;
        Single textHeight = 0f;
        Single maxLineWidth = 0f;
        Color gradientColorBottom = NGUIText.tint * NGUIText.gradientBottom;
        Color gradientColorTop = NGUIText.tint * NGUIText.gradientTop;
        Color32 textColor = NGUIText.tint;
        Int32 textLength = text.Length;
        Rect bmUvRect = default;
        Single bmTextureFactorX = 0f;
        Single bmTextureFactorY = 0f;
        Single ftSize = NGUIText.finalSize * NGUIText.pixelDensity;
        Boolean displacedStrike = false;
        Boolean containCharAlignment = false;
        Int32 printedLine = 0;
        NGUIText.Alignment defaultAlignment = NGUIText.alignment;
        BetterList<Int32> imgNotYetAligned = new BetterList<Int32>();
        if (NGUIText.bitmapFont != null)
        {
            bmUvRect = NGUIText.bitmapFont.uvRect;
            bmTextureFactorX = bmUvRect.width / NGUIText.bitmapFont.texWidth;
            bmTextureFactorY = bmUvRect.height / NGUIText.bitmapFont.texHeight;
        }
        if (textLength > 0)
            vertsLineOffsets.Add(0);

        Boolean useBIDI = NGUIText.ShouldUseBIDI(text);
        UnicodeBIDI bidi = null;
        Single[] allCharAdvances = null;
        if (useBIDI)
        {
            bidi = new UnicodeBIDI(text.ToCharArray(), readingDirection);
            text = new String(bidi.FullText);
            allCharAdvances = NGUIText.CalculateAllCharacterAdvances(text); // Sorted according to memory position
            Single[] reorderedAdvances = new Single[textLength + 1];
            for (Int32 i = 0; i < textLength; i++)
                reorderedAdvances[bidi.Reposition[i] + 1] = allCharAdvances[i];
            allCharAdvances = reorderedAdvances; // Sorted according to display position (+ shifted by 1)
            for (Int32 i = 2; i <= textLength; i++)
                if (text[i - 1] != '\n')
                    allCharAdvances[i] += allCharAdvances[i - 1]; // Cumulative advances per line (ie. vertex left position)
        }
        NGUIText.mTextModifiers.Reset();
        BetterList<Color> colorList = NGUIText.mTextModifiers.colors;
        colorList.Add(Color.white);
        NGUIText.mAlpha = 1f;

        for (Int32 texti = 0; texti < textLength; texti++)
        {
            Int32 ch = text[texti];
            if (useBIDI)
                currentX = allCharAdvances[bidi.Reposition[texti]];
            Single previousX = currentX;
            if (ch == '\n')
            {
                // Line forced return
                if (currentX > maxLineWidth)
                    maxLineWidth = currentX;
                if (verts.size > 0 && containCharAlignment)
                    NGUIText.AlignImageWithLastChar(ref specialImages, imgNotYetAligned, verts, printedLine);
                imgNotYetAligned.Clear();
                NGUIText.AlignImage(verts, vIndex, currentX - NGUIText.finalSpacingX, specialImages, printedLine);
                NGUIText.Align(verts, vIndex, currentX - NGUIText.finalSpacingX, 4);
                vIndex = verts.size;
                vertsLineOffsets.Add(vIndex);
                NGUIText.alignment = defaultAlignment;
                NGUIText.mTextModifiers.center = false;
                containCharAlignment = false;
                NGUIText.mTextModifiers.extraOffset = Vector3.zero;
                printedLine++;
                currentX = 0f;
                textHeight += NGUIText.finalLineHeight;
                prevCh = 0;
            }
            else if (ch < 32)
            {
                // Control characters
                prevCh = ch;
            }
            else if (NGUIText.encoding && DialogBoxSymbols.ParseSymbol(text, ref texti, NGUIText.premultiply, NGUIText.mTextModifiers))
            {
                // Opcode / tag
                Color colorFromOpcode;
                if (NGUIText.mTextModifiers.ignoreColor)
                {
                    colorFromOpcode = colorList[colorList.size - 1];
                    colorFromOpcode.a *= NGUIText.mAlpha * NGUIText.tint.a;
                }
                else
                {
                    colorFromOpcode = NGUIText.tint * colorList[colorList.size - 1];
                    colorFromOpcode.a *= NGUIText.mAlpha;
                }
                textColor = colorFromOpcode;
                for (Int32 i = 0; i < colorList.size - 2; i++)
                    colorFromOpcode.a *= colorList[i].a;
                if (NGUIText.gradient)
                {
                    gradientColorBottom = NGUIText.gradientBottom * colorFromOpcode;
                    gradientColorTop = NGUIText.gradientTop * colorFromOpcode;
                }
                texti--;
            }
            else
            {
                // Normal character
                if (NGUIText.mTextModifiers.justified)
                    NGUIText.alignment = NGUIText.Alignment.Justified;
                else if (NGUIText.mTextModifiers.center)
                    NGUIText.alignment = NGUIText.Alignment.Center;
                else
                    NGUIText.alignment = defaultAlignment;
                if (NGUIText.mTextModifiers.tabX != 0f)
                {
                    NGUIText.mTextModifiers.extraOffset.x = 0f;
                    currentX = NGUIText.mTextModifiers.tabX;
                    NGUIText.mTextModifiers.tabX = 0f;
                }
                NGUIText.AddSpecialIconToList(ref specialImages, ref imgNotYetAligned, ref NGUIText.mTextModifiers.insertImage, NGUIText.mTextModifiers.extraOffset, ref currentX, textHeight, printedLine);
                BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
                if (bmsymbol != null)
                {
                    // As a bitmap symbol
                    Single v0x = currentX + bmsymbol.offsetX * NGUIText.fontScale;
                    Single v1x = v0x + bmsymbol.width * NGUIText.fontScale;
                    Single v0y = -(textHeight + bmsymbol.offsetY * NGUIText.fontScale);
                    Single v1y = v0y - bmsymbol.height * NGUIText.fontScale;
                    if (Mathf.RoundToInt(currentX + bmsymbol.advance * NGUIText.fontScale) > NGUIText.regionWidth)
                    {
                        if (currentX == 0f)
                            return;
                        if (vIndex < verts.size)
                        {
                            NGUIText.AlignImage(verts, vIndex, currentX - NGUIText.finalSpacingX, specialImages, printedLine);
                            NGUIText.Align(verts, vIndex, currentX - NGUIText.finalSpacingX, 4);
                            vIndex = verts.size;
                            vertsLineOffsets.Add(vIndex);
                        }
                        NGUIText.alignment = defaultAlignment;
                        NGUIText.mTextModifiers.center = false;
                        NGUIText.mTextModifiers.extraOffset = Vector3.zero;
                        printedLine++;
                        v0x -= currentX;
                        v1x -= currentX;
                        v1y -= NGUIText.finalLineHeight;
                        v0y -= NGUIText.finalLineHeight;
                        currentX = 0f;
                        textHeight += NGUIText.finalLineHeight;
                    }
                    verts.Add(new Vector3(v0x, v1y));
                    verts.Add(new Vector3(v0x, v0y));
                    verts.Add(new Vector3(v1x, v0y));
                    verts.Add(new Vector3(v1x, v1y));
                    currentX += NGUIText.finalSpacingX + bmsymbol.advance * NGUIText.fontScale;
                    texti += bmsymbol.length - 1;
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
                            for (Int32 i = 0; i < 4; i++)
                                cols.Add(textColor);
                        }
                        else
                        {
                            Color32 whiteWithAlpha = Color.white;
                            whiteWithAlpha.a = textColor.a;
                            for (Int32 i = 0; i < 4; i++)
                                cols.Add(whiteWithAlpha);
                        }
                    }
                }
                else
                {
                    NGUIText.GlyphInfo glyphInfo = NGUIText.GetGlyph(ch, prevCh);
                    if (glyphInfo != null)
                    {
                        // As a glyph (the usual situation)
                        if (!containCharAlignment)
                            containCharAlignment = NGUIText.ContainCharAlignment(ch);
                        prevCh = ch;
                        currCh = ch;
                        if (NGUIText.mTextModifiers.sub != 0)
                        {
                            glyphInfo.v0 *= 0.75f;
                            glyphInfo.v1 *= 0.75f;
                            if (NGUIText.mTextModifiers.sub == 1)
                            {
                                glyphInfo.v0.y -= NGUIText.fontScale * NGUIText.fontSize * 0.4f;
                                glyphInfo.v1.y -= NGUIText.fontScale * NGUIText.fontSize * 0.4f;
                            }
                            else
                            {
                                glyphInfo.v0.y += NGUIText.fontScale * NGUIText.fontSize * 0.05f;
                                glyphInfo.v1.y += NGUIText.fontScale * NGUIText.fontSize * 0.05f;
                            }
                        }
                        Single v0x = glyphInfo.v0.x + currentX;
                        Single v0y = glyphInfo.v0.y - textHeight;
                        Single v1x = glyphInfo.v1.x + currentX;
                        Single v1y = glyphInfo.v1.y - textHeight;
                        Single glyphAdvance = glyphInfo.advance;
                        if (NGUIText.finalSpacingX < 0f)
                            glyphAdvance += NGUIText.finalSpacingX;
                        if (Mathf.RoundToInt(currentX + glyphAdvance) > NGUIText.regionWidth)
                        {
                            if (currentX == 0f)
                                return;
                            if (vIndex < verts.size)
                            {
                                NGUIText.Align(verts, vIndex, currentX - NGUIText.finalSpacingX, 4);
                                vIndex = verts.size;
                                vertsLineOffsets.Add(vIndex);
                            }
                            NGUIText.mTextModifiers.extraOffset = Vector3.zero;
                            v0x -= currentX;
                            v1x -= currentX;
                            v0y -= NGUIText.finalLineHeight;
                            v1y -= NGUIText.finalLineHeight;
                            currentX = 0f;
                            textHeight += NGUIText.finalLineHeight;
                            previousX = 0f;
                            NGUIText.alignment = defaultAlignment;
                            NGUIText.mTextModifiers.center = false;
                        }
                        if (NGUIText.IsSpace(ch))
                        {
                            if (NGUIText.mTextModifiers.underline)
                                ch = '_';
                            else if (NGUIText.mTextModifiers.strike)
                                ch = '-';
                        }
                        currentX += NGUIText.mTextModifiers.sub != 0 ? (NGUIText.finalSpacingX + glyphInfo.advance) * 0.75f : (NGUIText.finalSpacingX + glyphInfo.advance);
                        if (!NGUIText.IsSpace(ch))
                        {
                            if (uvs != null)
                            {
                                if (NGUIText.bitmapFont != null)
                                {
                                    glyphInfo.u0.x = bmUvRect.xMin + bmTextureFactorX * glyphInfo.u0.x;
                                    glyphInfo.u2.x = bmUvRect.xMin + bmTextureFactorX * glyphInfo.u2.x;
                                    glyphInfo.u0.y = bmUvRect.yMax - bmTextureFactorY * glyphInfo.u0.y;
                                    glyphInfo.u2.y = bmUvRect.yMax - bmTextureFactorY * glyphInfo.u2.y;
                                    glyphInfo.u1.x = glyphInfo.u0.x;
                                    glyphInfo.u1.y = glyphInfo.u2.y;
                                    glyphInfo.u3.x = glyphInfo.u2.x;
                                    glyphInfo.u3.y = glyphInfo.u0.y;
                                }
                                Int32 uvCopyCount = NGUIText.mTextModifiers.bold ? 4 : 1;
                                for (Int32 i = 0; i < uvCopyCount; i++)
                                {
                                    uvs.Add(glyphInfo.u0);
                                    uvs.Add(glyphInfo.u1);
                                    uvs.Add(glyphInfo.u2);
                                    uvs.Add(glyphInfo.u3);
                                }
                            }
                            if (cols != null)
                            {
                                if (glyphInfo.channel == 0 || glyphInfo.channel == 15)
                                {
                                    if (NGUIText.gradient)
                                    {
                                        Single gradientPos0 = ftSize + glyphInfo.v0.y / NGUIText.fontScale;
                                        Single gradientPos1 = ftSize + glyphInfo.v1.y / NGUIText.fontScale;
                                        gradientPos0 /= ftSize;
                                        gradientPos1 /= ftSize;
                                        NGUIText.s_c0 = Color.Lerp(gradientColorBottom, gradientColorTop, gradientPos0);
                                        NGUIText.s_c1 = Color.Lerp(gradientColorBottom, gradientColorTop, gradientPos1);
                                        Int32 colCopyCount = NGUIText.mTextModifiers.bold ? 4 : 1;
                                        for (Int32 i = 0; i < colCopyCount; i++)
                                        {
                                            cols.Add(NGUIText.s_c0);
                                            cols.Add(NGUIText.s_c1);
                                            cols.Add(NGUIText.s_c1);
                                            cols.Add(NGUIText.s_c0);
                                        }
                                    }
                                    else
                                    {
                                        Int32 colIndexCount = NGUIText.mTextModifiers.bold ? 16 : 4;
                                        for (Int32 i = 0; i < colIndexCount; i++)
                                            cols.Add(textColor);
                                    }
                                }
                                else
                                {
                                    Color adjustedColor = textColor;
                                    adjustedColor *= 0.49f;
                                    switch (glyphInfo.channel)
                                    {
                                        case 1: adjustedColor.b += 0.51f;   break;
                                        case 2: adjustedColor.g += 0.51f;   break;
                                        case 4: adjustedColor.r += 0.51f;   break;
                                        case 8: adjustedColor.a += 0.51f;   break;
                                    }
                                    Int32 colIndexCount = NGUIText.mTextModifiers.bold ? 16 : 4;
                                    for (Int32 i = 0; i < colIndexCount; i++)
                                        cols.Add(adjustedColor);
                                }
                            }
                            if (NGUIText.mTextModifiers.bold)
                            {
                                Single slantOffset = NGUIText.mTextModifiers.italic ? 0.1f * (v1y - v0y) : 0f;
                                for (Int32 i = 0; i < 4; i++)
                                {
                                    Single boldOffsetX = NGUIText.mBoldOffset[i * 2];
                                    Single boldOffsetY = NGUIText.mBoldOffset[i * 2 + 1];
                                    verts.Add(new Vector3(v0x + boldOffsetX - slantOffset, v0y + boldOffsetY));
                                    verts.Add(new Vector3(v0x + boldOffsetX + slantOffset, v1y + boldOffsetY));
                                    verts.Add(new Vector3(v1x + boldOffsetX + slantOffset, v1y + boldOffsetY));
                                    verts.Add(new Vector3(v1x + boldOffsetX - slantOffset, v0y + boldOffsetY));
                                }
                            }
                            else if (NGUIText.mTextModifiers.italic)
                            {
                                Single slantOffset = 0.1f * (v1y - v0y);
                                verts.Add(new Vector3(v0x - slantOffset, v0y));
                                verts.Add(new Vector3(v0x + slantOffset, v1y));
                                verts.Add(new Vector3(v1x + slantOffset, v1y));
                                verts.Add(new Vector3(v1x - slantOffset, v0y));
                            }
                            else
                            {
                                verts.Add(new Vector3(v0x, v0y));
                                verts.Add(new Vector3(v0x, v1y));
                                verts.Add(new Vector3(v1x, v1y));
                                verts.Add(new Vector3(v1x, v0y));
                            }
                            if (NGUIText.mTextModifiers.highShadow)
                                for (Int32 verti = verts.size - 4; verti < verts.size; verti++)
                                    highShadowVertIndexes.Add(verti);
                            if (NGUIText.mTextModifiers.extraOffset != Vector3.zero)
                                for (Int32 i = verts.size - 4; i < verts.size; i++)
                                    verts[i] += NGUIText.mTextModifiers.extraOffset;
                            if (NGUIText.ContainCharAlignment(ch))
                            {
                                NGUIText.AlignImageWithLastChar(ref specialImages, imgNotYetAligned, verts, printedLine);
                                imgNotYetAligned.Clear();
                            }
                            if (NGUIText.mTextModifiers.underline || NGUIText.mTextModifiers.strike)
                            {
                                NGUIText.GlyphInfo lineGlyph = NGUIText.GetGlyph(NGUIText.mTextModifiers.strike ? '-' : '_', prevCh);
                                if (lineGlyph != null)
                                {
                                    if (uvs != null)
                                    {
                                        if (NGUIText.bitmapFont != null)
                                        {
                                            lineGlyph.u0.x = bmUvRect.xMin + bmTextureFactorX * lineGlyph.u0.x;
                                            lineGlyph.u2.x = bmUvRect.xMin + bmTextureFactorX * lineGlyph.u2.x;
                                            lineGlyph.u0.y = bmUvRect.yMax - bmTextureFactorY * lineGlyph.u0.y;
                                            lineGlyph.u2.y = bmUvRect.yMax - bmTextureFactorY * lineGlyph.u2.y;
                                        }
                                        Single lineTextureCoordX = (lineGlyph.u0.x + lineGlyph.u2.x) * 0.5f;
                                        Int32 lineCopyCount = NGUIText.mTextModifiers.bold ? 4 : 1;
                                        for (Int32 i = 0; i < lineCopyCount; i++)
                                        {
                                            uvs.Add(new Vector2(lineTextureCoordX, lineGlyph.u0.y));
                                            uvs.Add(new Vector2(lineTextureCoordX, lineGlyph.u2.y));
                                            uvs.Add(new Vector2(lineTextureCoordX, lineGlyph.u2.y));
                                            uvs.Add(new Vector2(lineTextureCoordX, lineGlyph.u0.y));
                                        }
                                    }
                                    if (NGUIText.mTextModifiers.strike)
                                    {
                                        if (displacedStrike) // Maybe for overlining?
                                        {
                                            v0y = (-textHeight + lineGlyph.v0.y) * 0.75f;
                                            v1y = (-textHeight + lineGlyph.v1.y) * 0.75f;
                                        }
                                        else
                                        {
                                            v0y = -textHeight + lineGlyph.v0.y;
                                            v1y = -textHeight + lineGlyph.v1.y;
                                        }
                                    }
                                    else // underline
                                    {
                                        v0y = -textHeight + lineGlyph.v0.y - NGUIText.fontScale * NGUIText.fontSize * 0.3f;
                                        v1y = -textHeight + lineGlyph.v1.y - NGUIText.fontScale * NGUIText.fontSize * 0.3f;
                                    }
                                    if (NGUIText.mTextModifiers.bold)
                                    {
                                        for (Int32 i = 0; i < 4; i++)
                                        {
                                            Single boldOffsetX = NGUIText.mBoldOffset[i * 2];
                                            Single boldOffsetY = NGUIText.mBoldOffset[i * 2 + 1];
                                            verts.Add(new Vector3(previousX + boldOffsetX, v0y + boldOffsetY));
                                            verts.Add(new Vector3(previousX + boldOffsetX, v1y + boldOffsetY));
                                            verts.Add(new Vector3(currentX + boldOffsetX, v1y + boldOffsetY));
                                            verts.Add(new Vector3(currentX + boldOffsetX, v0y + boldOffsetY));
                                        }
                                    }
                                    else
                                    {
                                        verts.Add(new Vector3(previousX, v0y));
                                        verts.Add(new Vector3(previousX, v1y));
                                        verts.Add(new Vector3(currentX, v1y));
                                        verts.Add(new Vector3(currentX, v0y));
                                    }
                                    if (NGUIText.mTextModifiers.highShadow)
                                        for (Int32 verti = verts.size - 4; verti < verts.size; verti++)
                                            highShadowVertIndexes.Add(verti);
                                    if (NGUIText.gradient)
                                    {
                                        Single gradientPos0 = ftSize + lineGlyph.v0.y / NGUIText.fontScale;
                                        Single gradientPos1 = ftSize + lineGlyph.v1.y / NGUIText.fontScale;
                                        gradientPos0 /= ftSize;
                                        gradientPos1 /= ftSize;
                                        NGUIText.s_c0 = Color.Lerp(gradientColorBottom, gradientColorTop, gradientPos0);
                                        NGUIText.s_c1 = Color.Lerp(gradientColorBottom, gradientColorTop, gradientPos1);
                                        Int32 colCopyCount = NGUIText.mTextModifiers.bold ? 4 : 1;
                                        for (Int32 i = 0; i < colCopyCount; i++)
                                        {
                                            cols.Add(NGUIText.s_c0);
                                            cols.Add(NGUIText.s_c1);
                                            cols.Add(NGUIText.s_c1);
                                            cols.Add(NGUIText.s_c0);
                                        }
                                    }
                                    else
                                    {
                                        Int32 colIndexCount = NGUIText.mTextModifiers.bold ? 16 : 4;
                                        for (Int32 i = 0; i < colIndexCount; i++)
                                            cols.Add(textColor);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        NGUIText.AddSpecialIconToList(ref specialImages, ref imgNotYetAligned, ref NGUIText.mTextModifiers.insertImage, NGUIText.mTextModifiers.extraOffset, ref currentX, textHeight, printedLine);
        if (vIndex < verts.size)
        {
            if (useBIDI)
                currentX = allCharAdvances[textLength];
            NGUIText.AlignImage(verts, vIndex, currentX - NGUIText.finalSpacingX, specialImages, printedLine);
            NGUIText.Align(verts, vIndex, currentX - NGUIText.finalSpacingX, 4);
            vIndex = verts.size;
            vertsLineOffsets.Add(vIndex);
            NGUIText.alignment = defaultAlignment;
        }
        if (imgNotYetAligned.size > 0 && containCharAlignment && currCh != '-')
        {
            NGUIText.AlignImageWithLastChar(ref specialImages, imgNotYetAligned, verts, printedLine);
            imgNotYetAligned.Clear();
        }
        colorList.Clear();
        NGUIText.alignment = defaultAlignment;
    }

    public static void PrintApproximateCharacterPositions(String text, BetterList<Vector3> verts, BetterList<Int32> indices)
    {
        if (String.IsNullOrEmpty(text))
            text = " ";
        NGUIText.Prepare(text);
        Single currentX = 0f;
        Single textHeight = 0f;
        Single maxLineWidth = 0f;
        Single halfLineHeight = NGUIText.fontSize * NGUIText.fontScale * 0.5f;
        Int32 textLength = text.Length;
        Int32 vIndex = verts.size;
        Int32 prevCh = 0;
        NGUIText.mTextModifiers.Reset();
        for (Int32 texti = 0; texti < textLength; texti++)
        {
            Int32 ch = text[texti];
            verts.Add(new Vector3(currentX, -textHeight - halfLineHeight));
            indices.Add(texti);
            if (ch == '\n')
            {
                if (currentX > maxLineWidth)
                    maxLineWidth = currentX;
                if (NGUIText.alignment != NGUIText.Alignment.Left)
                {
                    NGUIText.Align(verts, vIndex, currentX - NGUIText.finalSpacingX, 1);
                    vIndex = verts.size;
                }
                currentX = 0f;
                textHeight += NGUIText.finalLineHeight;
                prevCh = 0;
            }
            else if (ch < 32)
            {
                prevCh = 0;
            }
            else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref texti, NGUIText.mTextModifiers))
            {
                texti--;
            }
            else
            {
                BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
                if (bmsymbol == null)
                {
                    Single glyphWidth = NGUIText.GetGlyphWidth(ch, prevCh);
                    if (glyphWidth != 0f)
                    {
                        glyphWidth += NGUIText.finalSpacingX;
                        if (Mathf.RoundToInt(currentX + glyphWidth) > NGUIText.regionWidth)
                        {
                            if (currentX == 0f)
                                return;
                            if (NGUIText.alignment != NGUIText.Alignment.Left && vIndex < verts.size)
                            {
                                NGUIText.Align(verts, vIndex, currentX - NGUIText.finalSpacingX, 1);
                                vIndex = verts.size;
                            }
                            currentX = glyphWidth;
                            textHeight += NGUIText.finalLineHeight;
                        }
                        else
                        {
                            currentX += glyphWidth;
                        }
                        verts.Add(new Vector3(currentX, -textHeight - halfLineHeight));
                        indices.Add(texti + 1);
                        prevCh = ch;
                    }
                }
                else
                {
                    Single advanceX = bmsymbol.advance * NGUIText.fontScale + NGUIText.finalSpacingX;
                    if (Mathf.RoundToInt(currentX + advanceX) > NGUIText.regionWidth)
                    {
                        if (currentX == 0f)
                            return;
                        if (NGUIText.alignment != NGUIText.Alignment.Left && vIndex < verts.size)
                        {
                            NGUIText.Align(verts, vIndex, currentX - NGUIText.finalSpacingX, 1);
                            vIndex = verts.size;
                        }
                        currentX = advanceX;
                        textHeight += NGUIText.finalLineHeight;
                    }
                    else
                    {
                        currentX += advanceX;
                    }
                    verts.Add(new Vector3(currentX, -textHeight - halfLineHeight));
                    indices.Add(texti + 1);
                    texti += bmsymbol.sequence.Length - 1;
                    prevCh = 0;
                }
            }
        }
        if (NGUIText.alignment != NGUIText.Alignment.Left && vIndex < verts.size)
            NGUIText.Align(verts, vIndex, currentX - NGUIText.finalSpacingX, 1);
    }

    public static void PrintExactCharacterPositions(String text, BetterList<Vector3> verts, BetterList<Int32> indices)
    {
        if (String.IsNullOrEmpty(text))
            text = " ";
        NGUIText.Prepare(text);
        Single lineHeight = NGUIText.fontSize * NGUIText.fontScale;
        Single currentX = 0f;
        Single textHeight = 0f;
        Single maxLineWidth = 0f;
        Int32 textLength = text.Length;
        Int32 vIndex = verts.size;
        Int32 prevCh = 0;
        NGUIText.mTextModifiers.Reset();
        for (Int32 texti = 0; texti < textLength; texti++)
        {
            Int32 ch = text[texti];
            if (ch == '\n')
            {
                if (currentX > maxLineWidth)
                    maxLineWidth = currentX;
                if (NGUIText.alignment != NGUIText.Alignment.Left)
                {
                    NGUIText.Align(verts, vIndex, currentX - NGUIText.finalSpacingX, 2);
                    vIndex = verts.size;
                }
                currentX = 0f;
                textHeight += NGUIText.finalLineHeight;
                prevCh = 0;
            }
            else if (ch < 32)
            {
                prevCh = 0;
            }
            else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref texti, NGUIText.mTextModifiers))
            {
                texti--;
            }
            else
            {
                BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
                if (bmsymbol == null)
                {
                    Single glyphWidth = NGUIText.GetGlyphWidth(ch, prevCh);
                    if (glyphWidth != 0f)
                    {
                        Single advanceX = glyphWidth + NGUIText.finalSpacingX;
                        if (Mathf.RoundToInt(currentX + advanceX) > NGUIText.regionWidth)
                        {
                            if (currentX == 0f)
                                return;
                            if (NGUIText.alignment != NGUIText.Alignment.Left && vIndex < verts.size)
                            {
                                NGUIText.Align(verts, vIndex, currentX - NGUIText.finalSpacingX, 2);
                                vIndex = verts.size;
                            }
                            currentX = 0f;
                            textHeight += NGUIText.finalLineHeight;
                            prevCh = 0;
                            texti--;
                        }
                        else
                        {
                            indices.Add(texti);
                            verts.Add(new Vector3(currentX, -textHeight - lineHeight));
                            verts.Add(new Vector3(currentX + advanceX, -textHeight));
                            prevCh = ch;
                            currentX += advanceX;
                        }
                    }
                }
                else
                {
                    Single advanceX = bmsymbol.advance * NGUIText.fontScale + NGUIText.finalSpacingX;
                    if (Mathf.RoundToInt(currentX + advanceX) > NGUIText.regionWidth)
                    {
                        if (currentX == 0f)
                            return;
                        if (NGUIText.alignment != NGUIText.Alignment.Left && vIndex < verts.size)
                        {
                            NGUIText.Align(verts, vIndex, currentX - NGUIText.finalSpacingX, 2);
                            vIndex = verts.size;
                        }
                        currentX = 0f;
                        textHeight += NGUIText.finalLineHeight;
                        prevCh = 0;
                        texti--;
                    }
                    else
                    {
                        indices.Add(texti);
                        verts.Add(new Vector3(currentX, -textHeight - lineHeight));
                        verts.Add(new Vector3(currentX + advanceX, -textHeight));
                        texti += bmsymbol.sequence.Length - 1;
                        currentX += advanceX;
                        prevCh = 0;
                    }
                }
            }
        }
        if (NGUIText.alignment != NGUIText.Alignment.Left && vIndex < verts.size)
            NGUIText.Align(verts, vIndex, currentX - NGUIText.finalSpacingX, 2);
    }

    public static void PrintCaretAndSelection(String text, Int32 start, Int32 end, BetterList<Vector3> caret, BetterList<Vector3> highlight)
    {
        if (String.IsNullOrEmpty(text))
            text = " ";
        NGUIText.Prepare(text);
        Int32 caretPos = end;
        if (start > end)
        {
            end = start;
            start = caretPos;
        }
        Single currentX = 0f;
        Single textHeight = 0f;
        Single maxLineWidth = 0f;
        Single lineHeight = NGUIText.fontSize * NGUIText.fontScale;
        Int32 caretStartIndex = caret == null ? 0 : caret.size;
        Int32 highlightIndex = highlight == null ? 0 : highlight.size;
        Int32 textLength = text.Length;
        Int32 prevCh = 0;
        Boolean highlightStarted = false;
        Boolean caretInitialized = false;
        Vector2 charEndPos1 = Vector2.zero;
        Vector2 charEndPos2 = Vector2.zero;
        Int32 texti;
        NGUIText.mTextModifiers.Reset();
        for (texti = 0; texti < textLength; texti++)
        {
            if (caret != null && !caretInitialized && caretPos <= texti)
            {
                caretInitialized = true;
                caret.Add(new Vector3(currentX - 1f, -textHeight - lineHeight - 6f));
                caret.Add(new Vector3(currentX - 1f, -textHeight + 6f));
                caret.Add(new Vector3(currentX + 1f, -textHeight + 6f));
                caret.Add(new Vector3(currentX + 1f, -textHeight - lineHeight - 6f));
            }
            Int32 ch = text[texti];
            if (ch == '\n')
            {
                if (currentX > maxLineWidth)
                    maxLineWidth = currentX;
                if (caret != null && caretInitialized)
                {
                    if (NGUIText.alignment != NGUIText.Alignment.Left)
                        NGUIText.Align(caret, caretStartIndex, currentX - NGUIText.finalSpacingX, 4);
                    caret = null;
                }
                if (highlight != null)
                {
                    if (highlightStarted)
                    {
                        highlightStarted = false;
                        highlight.Add(charEndPos2);
                        highlight.Add(charEndPos1);
                    }
                    else if (start <= texti && end > texti)
                    {
                        highlight.Add(new Vector3(currentX, -textHeight - lineHeight));
                        highlight.Add(new Vector3(currentX, -textHeight));
                        highlight.Add(new Vector3(currentX + 2f, -textHeight));
                        highlight.Add(new Vector3(currentX + 2f, -textHeight - lineHeight));
                    }
                    if (NGUIText.alignment != NGUIText.Alignment.Left && highlightIndex < highlight.size)
                    {
                        NGUIText.Align(highlight, highlightIndex, currentX - NGUIText.finalSpacingX, 4);
                        highlightIndex = highlight.size;
                    }
                }
                currentX = 0f;
                textHeight += NGUIText.finalLineHeight;
                prevCh = 0;
            }
            else if (ch < 32)
            {
                prevCh = 0;
            }
            else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref texti, NGUIText.mTextModifiers))
            {
                texti--;
            }
            else
            {
                BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
                Single glyphWidth = bmsymbol == null ? NGUIText.GetGlyphWidth(ch, prevCh) : bmsymbol.advance * NGUIText.fontScale;
                if (glyphWidth != 0f)
                {
                    Single charStartX = currentX;
                    Single charEndX = currentX + glyphWidth;
                    Single charStartY = -textHeight - lineHeight - 6f;
                    Single charEndY = -textHeight + 6f;
                    if (Mathf.RoundToInt(charEndX + NGUIText.finalSpacingX) > NGUIText.regionWidth)
                    {
                        if (currentX == 0f)
                            return;
                        if (currentX > maxLineWidth)
                            maxLineWidth = currentX;
                        if (caret != null && caretInitialized)
                        {
                            if (NGUIText.alignment != NGUIText.Alignment.Left)
                                NGUIText.Align(caret, caretStartIndex, currentX - NGUIText.finalSpacingX, 4);
                            caret = null;
                        }
                        if (highlight != null)
                        {
                            if (highlightStarted)
                            {
                                highlightStarted = false;
                                highlight.Add(charEndPos2);
                                highlight.Add(charEndPos1);
                            }
                            else if (start <= texti && end > texti)
                            {
                                highlight.Add(new Vector3(currentX, -textHeight - lineHeight));
                                highlight.Add(new Vector3(currentX, -textHeight));
                                highlight.Add(new Vector3(currentX + 2f, -textHeight));
                                highlight.Add(new Vector3(currentX + 2f, -textHeight - lineHeight));
                            }
                            if (NGUIText.alignment != NGUIText.Alignment.Left && highlightIndex < highlight.size)
                            {
                                NGUIText.Align(highlight, highlightIndex, currentX - NGUIText.finalSpacingX, 4);
                                highlightIndex = highlight.size;
                            }
                        }
                        charStartX -= currentX;
                        charEndX -= currentX;
                        charStartY -= NGUIText.finalLineHeight;
                        charEndY -= NGUIText.finalLineHeight;
                        currentX = 0f;
                        textHeight += NGUIText.finalLineHeight;
                    }
                    currentX += glyphWidth + NGUIText.finalSpacingX;
                    if (highlight != null)
                    {
                        if (start > texti || end <= texti)
                        {
                            if (highlightStarted)
                            {
                                highlightStarted = false;
                                highlight.Add(charEndPos2);
                                highlight.Add(charEndPos1);
                            }
                        }
                        else if (!highlightStarted)
                        {
                            highlightStarted = true;
                            highlight.Add(new Vector3(charStartX, charStartY));
                            highlight.Add(new Vector3(charStartX, charEndY));
                        }
                    }
                    charEndPos1 = new Vector2(charEndX, charStartY);
                    charEndPos2 = new Vector2(charEndX, charEndY);
                    prevCh = ch;
                }
            }
        }
        if (caret != null)
        {
            if (!caretInitialized)
            {
                caret.Add(new Vector3(currentX - 1f, -textHeight - lineHeight - 6f));
                caret.Add(new Vector3(currentX - 1f, -textHeight + 6f));
                caret.Add(new Vector3(currentX + 1f, -textHeight + 6f));
                caret.Add(new Vector3(currentX + 1f, -textHeight - lineHeight - 6f));
            }
            if (NGUIText.alignment != NGUIText.Alignment.Left)
                NGUIText.Align(caret, caretStartIndex, currentX - NGUIText.finalSpacingX, 4);
        }
        if (highlight != null)
        {
            if (highlightStarted)
            {
                highlight.Add(charEndPos2);
                highlight.Add(charEndPos1);
            }
            else if (start < texti && end == texti)
            {
                highlight.Add(new Vector3(currentX, -textHeight - lineHeight));
                highlight.Add(new Vector3(currentX, -textHeight));
                highlight.Add(new Vector3(currentX + 2f, -textHeight));
                highlight.Add(new Vector3(currentX + 2f, -textHeight - lineHeight));
            }
            if (NGUIText.alignment != NGUIText.Alignment.Left && highlightIndex < highlight.size)
                NGUIText.Align(highlight, highlightIndex, currentX - NGUIText.finalSpacingX, 4);
        }
    }

    private static Boolean ShouldUseBIDI(String text)
    {
        // Currently, BIDI is used only if the base language is not left-to-right for speed performance
        return NGUIText.readingDirection != UnicodeBIDI.LanguageReadingDirection.LeftToRight;
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
    public const String IconSprite = "SPRT";

    public static readonly HashSet<String> RenderOpcodeSymbols;
    public static readonly HashSet<String> TextOffsetOpcodeSymbols;

    public static readonly HashSet<Int32> IconIdException;
    public static readonly HashSet<Char> CharException;

    public static readonly HashSet<String> nameKeywordList;
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
    public static UnicodeBIDI.LanguageReadingDirection readingDirection = UnicodeBIDI.LanguageReadingDirection.LeftToRight;

    public static Int32 rectWidth = 1000000;    // Text area width (for alignment)
    public static Int32 rectHeight = 1000000;   // Not useful
    public static Int32 regionWidth = 1000000;  // Available text width
    public static Int32 regionHeight = 1000000; // Available text height

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

    private static FFIXTextModifier mTextModifiers = new FFIXTextModifier();

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
