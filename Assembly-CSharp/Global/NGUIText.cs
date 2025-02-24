using System;
using System.Diagnostics;
using System.Collections.Generic;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using UnityEngine;

public static class NGUIText
{
    public static Boolean ShowMobileButtons => FF9StateSystem.MobilePlatform && !NGUIText.ForceShowButton;
    public static Boolean ForceShowButton
    {
        get => NGUIText.forceShowButton;
        set => NGUIText.forceShowButton = value;
    }

    public static Single GetTextWidthFromFF9Font(UILabel phraseLabel, String text)
    {
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

    public static Single GetDialogWidthFromSpecialOpcode(List<Int32> specialCodeList, UILabel phraseLabel)
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
                    UInt32 tableId = Convert.ToUInt32(specialCodeList[i + 1]);
                    if (tableId > Byte.MaxValue)
                    {
                        UInt32 textId = Convert.ToUInt32(specialCodeList[i + 2]);
                        String[] tableText = FF9TextTool.GetTableText(tableId - Byte.MaxValue - 1);
                        extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, tableText[textId]);
                        i += 2;
                    }
                    else
                    {
                        extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ETb.GetStringFromTable(tableId >> 4 & 3u, tableId & 7u));
                        i++;
                    }
                    break;
                }
                case 14: // [ITEM=SCRIPTID]
                {
                    Int32 scriptId = specialCodeList[i + 1];
                    String itemName = ETb.GetItemName(ETb.gMesValue[scriptId]);
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
                    if (member[specialCode - 24] != null)
                        extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, member[specialCode - 24].Name);
                    break;
                }
                case 64: // [NUMB=SCRIPTID]
                {
                    Int32 scriptId = specialCodeList[i + 1];
                    extraWidth += NGUIText.GetTextWidthFromFF9Font(phraseLabel, ETb.gMesValue[scriptId].ToString());
                    i++;
                    break;
                }
                case 112: // [PNEW=SCRIPTID]
                    if ((ETb.gMesValue[0] & (1 << specialCodeList[++i])) != 0)
                        extraWidth += 30f;
                    break;
            }
        }
        return extraWidth;
    }

    public static DialogImage CreateButtonImage(String parameterStr, Boolean checkConfig, String tag)
    {
        if (NGUIText.ShowMobileButtons)
            return null;
        DialogImage dialogImage = new DialogImage();
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
                control = EventInput.IsJapaneseLayout ? Control.Confirm : Control.Cancel;
                break;
            case "CROSS":
                control = EventInput.IsJapaneseLayout ? Control.Cancel : Control.Confirm;
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
                Log.Warning($"[NGUIText] Unsupported button \"{parameterStr}\" for the text tag \"{tag}\"");
                return null;
        }
        dialogImage.Size = FF9UIDataTool.GetButtonSize(control, checkConfig, tag);
        dialogImage.Id = (Int32)control;
        dialogImage.tag = tag;
        dialogImage.checkFromConfig = checkConfig;
        dialogImage.AppearStep = NGUIText.progressStep;
        return dialogImage;
    }

    public static DialogImage CreateIconImage(Int32 iconId)
    {
        DialogImage dialogImage = new DialogImage();
        dialogImage.Size = FF9UIDataTool.GetIconSize(iconId);
        if (iconId == 180) // text_lv_us_uk_jp_gr_it
            dialogImage.Offset = new Vector3(0f, -15.2f);
        else
            dialogImage.Offset = new Vector3(0f, -10f);
        dialogImage.Id = iconId;
        dialogImage.IsButton = false;
        dialogImage.AppearStep = NGUIText.progressStep;
        return dialogImage;
    }

    public static DialogImage CreateSpriteImage(String[] args)
    {
        if (args == null)
            return null;
        DialogImage dialogImage = new DialogImage();
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
        else
        {
            Log.Warning($"[NGUIText] Unsupported parameters for \"{NGUIText.IconSprite}\": expect between 1 and 4 parameters, got {args.Length}: {String.Join(", ", args)}");
            return null;
        }
        dialogImage.Offset = new Vector3(0f, -10f);
        dialogImage.Id = -1;
        dialogImage.IsButton = false;
        dialogImage.AppearStep = NGUIText.progressStep;
        return dialogImage;
    }

    public static Single[] CalculateAllCharacterAdvances(String text)
    {
        NGUIText.Prepare(text);
        Int32 textLength = text.Length;
        Int32 prevCh = 0;
        Single[] charAdvances = new Single[textLength];
        for (Int32 texti = 0; texti < textLength; texti++)
        {
            Int32 ch = text[texti];
            if (ch >= 32)
            {
                BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
                if (bmsymbol == null)
                {
                    Single glyphWidth = NGUIText.GetGlyphWidth(ch, prevCh);
                    if (glyphWidth != 0f)
                    {
                        glyphWidth += NGUIText.finalSpacingX;
                        charAdvances[texti] = glyphWidth; // NGUIText.mTextModifiers.sub not taken into account
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
        return charAdvances;
    }

    public static void SetIconDepth(GameObject phaseLabel, GameObject iconObject, Boolean isLowerPhrase = true)
    {
        Int32 depth = phaseLabel.GetComponent<UIWidget>().depth;
        depth = isLowerPhrase ? depth - iconObject.transform.childCount - 1 : depth + 1;
        iconObject.GetComponent<UIWidget>().depth = depth++;
        foreach (Transform transform in iconObject.transform)
            transform.GetComponent<UIWidget>().depth = depth++;
    }

    public static Boolean ShouldAlignImageVertically(DialogImage img)
    {
        return img.IsButton || !NGUIText.IconIdException.Contains(img.Id);
    }

    private static void AlignImageWithPadding(BetterList<DialogImage> specialImages, Vector3 padding, Int32 printedLine)
    {
        foreach (DialogImage dialogImage in specialImages)
            if (dialogImage.PrintedLine == printedLine)
                dialogImage.LocalPosition += padding;
    }

    public static void UpdateFontSizes(Boolean requestBaseLine)
    {
        NGUIText.finalSize = Mathf.RoundToInt(NGUIText.fontSize / NGUIText.pixelDensity);
        NGUIText.finalSpacingX = NGUIText.spacingX * NGUIText.fontScale;
        NGUIText.finalLineHeight = (NGUIText.fontSize + NGUIText.spacingY) * NGUIText.fontScale;
        NGUIText.useSymbols = NGUIText.bitmapFont != null && NGUIText.bitmapFont.hasSymbols && NGUIText.encoding && NGUIText.symbolStyle != NGUIText.SymbolStyle.None;
        if (requestBaseLine && NGUIText.dynamicFont != null)
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

    private static void Prepare(String text)
    {
        if (NGUIText.dynamicFont != null && !String.IsNullOrEmpty(text))
            NGUIText.dynamicFont.RequestCharactersInTexture(text, NGUIText.finalSize, NGUIText.fontStyle);
    }

    public static BMSymbol GetSymbol(String text, Int32 index, Int32 textLength)
    {
        return NGUIText.bitmapFont == null ? null : NGUIText.bitmapFont.MatchSymbol(text, index, textLength);
    }

    public static Single GetGlyphHeight(Int32 ch)
    {
        Single height = 0f;
        if (NGUIText.bitmapFont != null)
        {
            BMGlyph bmglyph = NGUIText.bitmapFont.bmFont.GetGlyph(ch);
            if (bmglyph != null)
                height = bmglyph.offsetY * NGUIText.fontScale;
        }
        else if (NGUIText.dynamicFont != null && NGUIText.dynamicFont.GetCharacterInfo((Char)ch, out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle))
        {
            height = Mathf.Round(NGUIText.baseline - NGUIText.mTempChar.minY) * NGUIText.fontScale * NGUIText.pixelDensity;
        }
        return height;
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

    private static Color ComputeGradientColor(Color gradientColorBottom, Color gradientColorTop, Single glyphY, Single ftSize)
    {
        Single factor = (ftSize + glyphY / NGUIText.fontScale) / ftSize;
        return Color.Lerp(gradientColorBottom, gradientColorTop, factor);
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static Single ParseAlpha(String text, Int32 index)
    {
        Int32 alpha = NGUIMath.HexToDecimal(text[index]) << 4 | NGUIMath.HexToDecimal(text[index + 1]);
        return Mathf.Clamp01(alpha * 0.003921569f); // 1/255
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
        return "[" + NGUIText.EncodeColor24(c) + "]";
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

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static Boolean IsHex(Char ch)
    {
        return (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
    }

    private static void Align(TextParser.Line lineInfo, BetterList<Vector3> verts, Int32 indexOffset, Int32 printedLine, Single printedWidth, Int32 vertCountByCharacter, BetterList<DialogImage> imageList, Dialog dialog)
    {
        if ((verts.size == 0 || verts.size <= indexOffset) && (imageList == null || imageList.size == 0))
            return;
        Alignment finalAlignement = NGUIText.alignment;
        if (dialog != null && !dialog.IsETbDialog && NGUIText.mTextModifiers.choice)
            finalAlignement = Alignment.Center;
        else if (NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft && !NGUIText.fixedAlignment && finalAlignement == Alignment.Left)
            finalAlignement = Alignment.Right;
        lineInfo.Alignment = finalAlignement;
        switch (finalAlignement)
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
                    {
                        for (Int32 i = indexOffset; i < verts.size; i++)
                            verts.buffer[i].x += positiveEmptySpace;
                        if (imageList != null)
                            NGUIText.AlignImageWithPadding(ref imageList, new Vector3(positiveEmptySpace, 0f), printedLine);
                    }
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
                if (imageList != null)
                    NGUIText.AlignImageWithPadding(imageList, new Vector3(halfEmptySpace, 0f), printedLine);
                break;
            }
            case NGUIText.Alignment.Right:
            {
                Single emptySpace = NGUIText.rectWidth - printedWidth;
                if (emptySpace < 0f)
                    return;
                for (Int32 i = indexOffset; i < verts.size; i++)
                    verts.buffer[i].x += emptySpace;
                if (imageList != null)
                    NGUIText.AlignImageWithPadding(imageList, new Vector3(emptySpace, 0f), printedLine);
                break;
            }
            case NGUIText.Alignment.Justified:
            {
                if (verts.size == 0 || verts.size <= indexOffset)
                    return;
                if (printedWidth < NGUIText.rectWidth * 0.65f)
                {
                    lineInfo.Alignment = Alignment.Right;
                    if (NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft)
                        goto case NGUIText.Alignment.Right;
                    lineInfo.Alignment = Alignment.Left;
                    goto case NGUIText.Alignment.Left;
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
    private static Boolean EndLine(ref String str, Int32 pos)
    {
        if (pos < 0 || pos >= str.Length)
            return false;
        if (str[pos] == '\n')
            return false;
        if (NGUIText.IsSpace(str[pos]))
            str = str.Substring(0, pos) + "\n" + str.Substring(pos + 1);
        else
            str = str.Substring(0, pos) + "\n" + str.Substring(pos);
        return true;
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    private static Boolean ReplaceSpaceWithNewline(ref String str, Int32 pos)
    {
        if (pos < 0 || pos >= str.Length)
            return false;
        if (!NGUIText.IsSpace(str[pos]))
            return false;
        str = str.Substring(0, pos) + "\n" + str.Substring(pos + 1);
        return true;
    }

    public static Int32 EnsureEvenSize(Int32 size)
    {
        return (size & 1) == 1 ? size + 1 : size;
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

    public static Boolean WrapText(TextParser parser, Boolean cancelOnFail)
    {
        Log.Message($"[DBG] Wrap text for {parser.ParsedText.Replace('\n', '+')} --- {NGUIText.finalLineHeight}");
        List<TextParser.Line> lineList = new List<TextParser.Line>();
        TextParser.Line currentLine = new TextParser.Line();
        lineList.Add(currentLine);
        if (NGUIText.regionWidth < 1 || NGUIText.regionHeight < 1 || NGUIText.finalLineHeight < 1f)
        {
            if (!cancelOnFail)
            {
                parser.ParsedText = String.Empty;
                parser.LineInfo = lineList;
            }
            return false;
        }
        Single maxHeight = NGUIText.maxLines <= 0 ? NGUIText.regionHeight : Mathf.Min(NGUIText.regionHeight, NGUIText.finalLineHeight * NGUIText.maxLines);
        Int32 maxLineCount = NGUIText.maxLines <= 0 ? 1000000 : NGUIText.maxLines;
        maxLineCount = Mathf.FloorToInt(Mathf.Min(maxLineCount, maxHeight / NGUIText.finalLineHeight) + 0.01f);
        if (maxLineCount == 0)
        {
            if (!cancelOnFail)
            {
                parser.ParsedText = String.Empty;
                parser.LineInfo = lineList;
            }
            return false;
        }
        List<FFIXTextTag> tabTags = new List<FFIXTextTag>();
        String wrappedText = parser.ParsedText; // wrappedText and parser.ParsedText may differ only in spaces replaced by newlines
        NGUIText.Prepare(wrappedText);
        NGUIText.mTextModifiers.Reset();
        Int32 textLength = wrappedText.Length;
        Int32 lastPossibleBreakPos = 0;
        Int32 currentLineCount = 1;
        Int32 prevCh = 0;
        Boolean hasSpecialCharacter = false;
        Boolean noBreakableSpaceYet = true;
        Boolean preventWordBreak = false;
        Boolean wordsPreserved = true;
        Boolean afterImage = false;
        Single lastPossibleBreakX = 0f;
        Single currentTabX = 0f;
        Single currentX = 0f;
        Single offsetY = 0f;
        Int32 tagIndex = 0;
        Int32 texti;
        for (texti = 0; texti <= textLength; texti++)
        {
            afterImage = false;
            while (tagIndex < parser.ParsedTagList.Count && texti == parser.ParsedTagList[tagIndex].TextOffset)
            {
                if (parser.ParsedTagList[tagIndex].Code == FFIXTextTagCode.Choice)
                    preventWordBreak = true;
                DialogBoxSymbols.ParseSingleGenericTag(parser.ParsedTagList[tagIndex], null, null, NGUIText.mTextModifiers);
                if (NGUIText.mTextModifiers.extraOffset != Vector2.zero)
                {
                    currentX += NGUIText.mTextModifiers.extraOffset.x;
                    offsetY += NGUIText.mTextModifiers.extraOffset.y;
                    NGUIText.mTextModifiers.extraOffset = Vector2.zero;
                    lastPossibleBreakPos = texti;
                    lastPossibleBreakX = currentX;
                    afterImage = false;
                }
                else if (NGUIText.mTextModifiers.insertImage != null)
                {
                    currentLine.BaseHeight = Math.Max(currentLine.BaseHeight, NGUIText.mTextModifiers.insertImage.Size.y);
                    currentX += NGUIText.mTextModifiers.insertImage.Size.x;
                    NGUIText.mTextModifiers.insertImage = null;
                    lastPossibleBreakPos = texti;
                    lastPossibleBreakX = currentX;
                    afterImage = true;
                }
                else if (NGUIText.mTextModifiers.tabX.HasValue)
                {
                    currentX = NGUIText.mTextModifiers.tabX.Value;
                    NGUIText.mTextModifiers.tabX = null;
                    currentTabX = currentX;
                    lastPossibleBreakPos = texti;
                    lastPossibleBreakX = currentX;
                    afterImage = false;
                }
                currentLine.Width = Math.Max(currentLine.Width, currentX);
                tagIndex++;
            }
            if (texti == textLength)
                break;
            Char ch = wrappedText[texti];
            // Note: This was used, maybe for characters like【】(0x3010 and 0x3011) or『』(0x300E and 0x300F), to avoid text wrapping
            //if (ch > 0x2FFF)
            //    hasSpecialCharacter = true;
            if (ch == '\n')
            {
                if (currentLineCount == maxLineCount)
                    break;
                currentLine = new TextParser.Line();
                lineList.Add(currentLine);
                currentLine.BaseY = currentLineCount * NGUIText.finalLineHeight;
                noBreakableSpaceYet = true;
                currentLineCount++;
                lastPossibleBreakPos = texti;
                lastPossibleBreakX = 0f;
                currentTabX = 0f;
                currentX = 0f;
                offsetY = 0f;
                prevCh = 0;
            }
            else
            {
                Single advanceX, charBottom;
                BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(wrappedText, texti, textLength) : null;
                if (bmsymbol != null)
                {
                    advanceX = NGUIText.finalSpacingX + bmsymbol.advance * NGUIText.fontScale;
                    charBottom = (bmsymbol.offsetY + bmsymbol.height) * NGUIText.fontScale;
                }
                else
                {
                    NGUIText.GlyphInfo glyphInfo = NGUIText.GetGlyph(ch, prevCh);
                    if (glyphInfo == null || glyphInfo.advance == 0f)
                        continue;
                    advanceX = NGUIText.finalSpacingX + glyphInfo.advance;
                    charBottom = glyphInfo.v1.y;
                }
                Boolean isSpace = NGUIText.IsSpace(ch);
                if (isSpace && !hasSpecialCharacter && lastPossibleBreakPos + 1 < texti)
                {
                    noBreakableSpaceYet = false;
                    lastPossibleBreakPos = texti;
                    lastPossibleBreakX = currentX;
                }
                if (!isSpace || !afterImage)
                {
                    currentX += advanceX;
                    if (Mathf.RoundToInt(currentX) > NGUIText.regionWidth)
                    {
                        if (preventWordBreak)
                        {
                            wordsPreserved = false;
                            prevCh = ch;
                        }
                        else if (noBreakableSpaceYet || currentLineCount == maxLineCount)
                        {
                            if (!isSpace && !hasSpecialCharacter)
                                wordsPreserved = false;
                            if (currentLineCount++ == maxLineCount)
                                break;
                            lastPossibleBreakPos = Math.Max(lastPossibleBreakPos, texti - 1);
                            if (NGUIText.ReplaceSpaceWithNewline(ref wrappedText, lastPossibleBreakPos))
                            {
                                currentLine = new TextParser.Line();
                                lineList.Add(currentLine);
                                currentLine.BaseY = (currentLineCount - 1) * NGUIText.finalLineHeight;
                                if (currentTabX != 0f)
                                    tabTags.Add(new FFIXTextTag(FFIXTextTagCode.DialogX, [currentTabX.ToString()], lastPossibleBreakPos + 1));
                            }
                            noBreakableSpaceYet = true;
                            if (isSpace)
                            {
                                lastPossibleBreakPos = texti;
                                currentX = currentTabX;
                                lastPossibleBreakX = currentX;
                            }
                            else
                            {
                                lastPossibleBreakPos = texti - 1;
                                currentX = currentTabX + advanceX;
                                lastPossibleBreakX = currentX;
                            }
                            offsetY = 0f;
                            prevCh = 0;
                        }
                        else
                        {
                            noBreakableSpaceYet = true;
                            texti = lastPossibleBreakPos;
                            prevCh = 0;
                            if (currentLineCount++ == maxLineCount)
                                break;
                            if (NGUIText.ReplaceSpaceWithNewline(ref wrappedText, lastPossibleBreakPos))
                            {
                                currentLine.Width = lastPossibleBreakX;
                                currentLine = new TextParser.Line();
                                lineList.Add(currentLine);
                                currentLine.BaseY = (currentLineCount - 1) * NGUIText.finalLineHeight;
                                if (currentTabX != 0f)
                                    tabTags.Add(new FFIXTextTag(FFIXTextTagCode.DialogX, [currentTabX.ToString()], lastPossibleBreakPos + 1));
                            }
                            currentX = currentTabX;
                            offsetY = 0f;
                        }
                    }
                    else
                    {
                        prevCh = ch;
                    }
                }
                currentLine.Width = Math.Max(currentLine.Width, currentX);
                currentLine.BaseHeight = Math.Max(currentLine.BaseHeight, -charBottom + offsetY);
                if (bmsymbol != null)
                {
                    texti += bmsymbol.length - 1;
                    prevCh = 0;
                }
            }
        }
        Boolean success = wordsPreserved && (texti == textLength || currentLineCount <= Mathf.Min(NGUIText.maxLines, maxLineCount));
        if (success || !cancelOnFail)
        {
            if (!success)
                Log.Message($"[DBG] Failed wrapping {parser.ParsedText.Replace('\n', '+')} --- {wrappedText.Replace('\n', '+')} --- {texti},{textLength - texti} & {lineList.Count}");
            foreach (FFIXTextTag tag in tabTags)
                parser.InsertTag(tag, tag.TextOffset);
            parser.ParsedText = wrappedText;
            parser.RemovePart(texti, textLength - texti);
            parser.LineInfo = lineList;
        }
        return success;
    }

    public static void GenerateTextRender(TextParser parser)
    {
        Log.Message($"[DBG] Generate render for {parser.ParsedText.Replace('\n', '+')} --- {parser.LineInfo.Count} --- {NGUIText.finalLineHeight}");
        Dialog dialog = parser.LabelContainer.DialogWindow;
        NGUIText.SetupShadowEffect(parser);
        NGUIText.choiceIndent = dialog != null && !dialog.IsETbDialog ? 0f : 14f * UIManager.ResourceXMultipier;
        NGUIText.progressStep = 0f;
        NGUIText.mAlpha = 1f;
        NGUIText.mTextModifiers.Reset();
        NGUIText.Prepare(parser.ParsedText);
        Int32 lineFirstVIndex = 0;
        Int32 charFirstVIndex = 0;
        Int32 prevCh = 0;
        Int32 tagIndex = 0;
        Single currentX = 0f;
        Single currentY = parser.LineInfo[0].BaseY;
        Single typicalCharacterHeight = NGUIText.GetGlyphHeight('A');
        Color gradientColorBottom = NGUIText.tint * NGUIText.gradientBottom;
        Color gradientColorTop = NGUIText.tint * NGUIText.gradientTop;
        Color32 textColor = NGUIText.tint;
        Int32 textLength = parser.ParsedText.Length;
        Rect bmUvRect = default;
        Single bmTextureFactorX = 0f;
        Single bmTextureFactorY = 0f;
        Single ftSize = NGUIText.finalSize * NGUIText.pixelDensity;
        Boolean displacedStrike = false;
        Boolean afterImage = false;
        Int32 printedLine = 0;
        NGUIText.Alignment defaultAlignment = NGUIText.alignment;
        BetterList<Int32> imgNotYetAligned = new BetterList<Int32>();
        if (NGUIText.bitmapFont != null)
        {
            bmUvRect = NGUIText.bitmapFont.uvRect;
            bmTextureFactorX = bmUvRect.width / NGUIText.bitmapFont.texWidth;
            bmTextureFactorY = bmUvRect.height / NGUIText.bitmapFont.texHeight;
        }

        Boolean useBIDI = parser.Bidi != null;
        Single[] allCharAdvances = null;
        if (useBIDI)
        {
            allCharAdvances = NGUIText.CalculateAllCharacterAdvances(parser.ParsedText); // Sorted according to memory position
            Single[] reorderedAdvances = new Single[textLength + 1];
            for (Int32 i = 0; i < textLength; i++)
                reorderedAdvances[parser.Bidi.Reposition[i] + 1] = allCharAdvances[i];
            allCharAdvances = reorderedAdvances; // Sorted according to display position (+ shifted by 1)
            for (Int32 i = 2; i <= textLength; i++)
                if (parser.ParsedText[i - 1] != '\n')
                    allCharAdvances[i] += allCharAdvances[i - 1]; // Cumulative advances per line (ie. vertex left position)
        }
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Color32> cols = new List<Color32>();
        NGUIText.mTextModifiers.colors.Add(Color.white); // [DBG] use a label's "clear color"

        Int32 textIndexAdvance = 1;
        for (Int32 texti = 0; texti <= textLength; texti += textIndexAdvance)
        {
            afterImage = false;
            textIndexAdvance = 1;
            while (tagIndex < parser.ParsedTagList.Count && texti == parser.ParsedTagList[tagIndex].TextOffset)
            {
                // Process tags at that position
                parser.ParsedTagList[tagIndex].AppearStep = NGUIText.progressStep;
                DialogBoxSymbols.ParseSingleGenericTag(parser.ParsedTagList[tagIndex], dialog, parser.LabelContainer, NGUIText.mTextModifiers);
                NGUIText.mTextModifiers.UpdateSettingsAfterTag(ref currentX, ref currentY, ref afterImage, ref parser.SpecialImages, ref textColor, ref gradientColorBottom, ref gradientColorTop, defaultAlignment, printedLine, typicalCharacterHeight);
                tagIndex++;
            }
            if (texti == textLength)
                break;
            Int32 ch = parser.ParsedText[texti];
            if (useBIDI)
                currentX = allCharAdvances[parser.Bidi.Reposition[texti]];
            Single previousX = currentX;
            if (ch == '\n')
            {
                // Line return
                NGUIText.Align(parser.LineInfo[printedLine], parser.Vertices, lineFirstVIndex, printedLine, currentX - NGUIText.finalSpacingX, 4, parser.SpecialImages, dialog);
                NGUIText.alignment = defaultAlignment;
                NGUIText.mTextModifiers.ResetLine();
                NGUIText.progressStep += NGUIText.mTextModifiers.appearanceSpeed;
                lineFirstVIndex = parser.Vertices.size;
                parser.LineInfo[printedLine].EndVertexIndex = lineFirstVIndex;
                printedLine++;
                parser.LineInfo[printedLine].FirstVertexIndex = lineFirstVIndex;
                currentX = NGUIText.mTextModifiers.choice ? NGUIText.choiceIndent : 0f;
                currentY = parser.LineInfo[printedLine].BaseY;
                prevCh = 0;
                continue;
            }
            else if (ch < 32)
            {
                // Control characters
                prevCh = ch;
                continue;
            }
            if (!afterImage || !NGUIText.IsSpace(ch))
            {
                // Normal character
                Single vMinX, vMaxX, vMinY, vMaxY;
                Single uvMinX, uvMaxX, uvMinY, uvMaxY;
                Color32 colBottom, colTop;
                Single glyphAdvance;
                BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(parser.ParsedText, texti, textLength) : null;
                if (bmsymbol != null)
                {
                    // As a bitmap symbol
                    vMinX = bmsymbol.offsetX * NGUIText.fontScale;
                    vMaxX = vMinX + bmsymbol.width * NGUIText.fontScale;
                    vMaxY = -bmsymbol.offsetY * NGUIText.fontScale;
                    vMinY = vMaxY - bmsymbol.height * NGUIText.fontScale;
                    uvMinX = bmsymbol.uvRect.xMin;
                    uvMaxX = bmsymbol.uvRect.xMax;
                    uvMinY = bmsymbol.uvRect.yMin;
                    uvMaxY = bmsymbol.uvRect.yMax;
                    if (NGUIText.symbolStyle == NGUIText.SymbolStyle.Colored)
                    {
                        colBottom = textColor;
                        colTop = textColor;
                    }
                    else
                    {
                        colBottom = Color.white;
                        colBottom.a = textColor.a;
                        colTop = colBottom;
                    }
                    glyphAdvance = bmsymbol.advance * NGUIText.fontScale;
                    textIndexAdvance = bmsymbol.length;
                    prevCh = 0;
                }
                else
                {
                    NGUIText.GlyphInfo glyphInfo = NGUIText.GetGlyph(ch, prevCh);
                    if (glyphInfo == null)
                        continue;
                    // As a glyph (the usual situation)
                    prevCh = ch;
                    vMinX = glyphInfo.v0.x;
                    vMaxX = glyphInfo.v1.x;
                    vMinY = glyphInfo.v1.y;
                    vMaxY = glyphInfo.v0.y;
                    if (NGUIText.bitmapFont != null)
                    {
                        uvMinX = bmUvRect.xMin + bmTextureFactorX * glyphInfo.u0.x;
                        uvMaxX = bmUvRect.xMin + bmTextureFactorX * glyphInfo.u2.x;
                        uvMinY = bmUvRect.yMax - bmTextureFactorY * glyphInfo.u0.y;
                        uvMaxY = bmUvRect.yMax - bmTextureFactorY * glyphInfo.u2.y;
                    }
                    else
                    {
                        uvMinX = glyphInfo.u0.x;
                        uvMaxX = glyphInfo.u2.x;
                        uvMinY = glyphInfo.u0.y;
                        uvMaxY = glyphInfo.u2.y;
                    }
                    if (glyphInfo.channel == 0 || glyphInfo.channel == 15)
                    {
                        if (NGUIText.gradient)
                        {
                            colBottom = NGUIText.ComputeGradientColor(gradientColorBottom, gradientColorTop, glyphInfo.v0.y, ftSize);
                            colTop = NGUIText.ComputeGradientColor(gradientColorBottom, gradientColorTop, glyphInfo.v1.y, ftSize);
                        }
                        else
                        {
                            colBottom = textColor;
                            colTop = textColor;
                        }
                    }
                    else
                    {
                        Color adjustedColor = textColor;
                        adjustedColor *= 0.49f;
                        switch (glyphInfo.channel)
                        {
                            case 1: adjustedColor.b += 0.51f; break;
                            case 2: adjustedColor.g += 0.51f; break;
                            case 4: adjustedColor.r += 0.51f; break;
                            case 8: adjustedColor.a += 0.51f; break;
                        }
                        colBottom = adjustedColor;
                        colTop = adjustedColor;
                    }
                    glyphAdvance = glyphInfo.advance;
                }
                if (NGUIText.mTextModifiers.sub != 0)
                {
                    // Superscript or subscript
                    vMinX *= 0.75f;
                    vMaxX *= 0.75f;
                    vMinY *= 0.75f;
                    vMaxY *= 0.75f;
                    if (NGUIText.mTextModifiers.sub == 1)
                    {
                        vMinY -= NGUIText.fontScale * NGUIText.fontSize * 0.4f;
                        vMaxY -= NGUIText.fontScale * NGUIText.fontSize * 0.4f;
                    }
                    else
                    {
                        vMinY += NGUIText.fontScale * NGUIText.fontSize * 0.05f;
                        vMaxY += NGUIText.fontScale * NGUIText.fontSize * 0.05f;
                    }
                }
                vMinX += currentX;
                vMaxX += currentX;
                vMinY -= currentY;
                vMaxY -= currentY;
                currentX += NGUIText.mTextModifiers.sub != 0 ? (NGUIText.finalSpacingX + glyphAdvance) * 0.75f : (NGUIText.finalSpacingX + glyphAdvance);
                if (NGUIText.mTextModifiers.mirror ^ (useBIDI && parser.Bidi.MustMirror.Contains(texti)))
                {
                    Single tmp = uvMinX;
                    uvMinX = uvMaxX;
                    uvMaxX = tmp;
                }
                Int32 charCopyCount = 1;
                if (!NGUIText.IsSpace(ch))
                {
                    // Generate the character's render
                    if (NGUIText.mTextModifiers.bold)
                    {
                        charCopyCount = 4;
                        Single slantOffset = NGUIText.mTextModifiers.italic ? 0.1f * (vMinY - vMaxY) : 0f;
                        for (Int32 i = 0; i < 4; i++)
                        {
                            Single boldOffsetX = NGUIText.mBoldOffset[i * 2];
                            Single boldOffsetY = NGUIText.mBoldOffset[i * 2 + 1];
                            verts.Add(new Vector3(vMinX + boldOffsetX - slantOffset, vMaxY + boldOffsetY));
                            verts.Add(new Vector3(vMinX + boldOffsetX + slantOffset, vMinY + boldOffsetY));
                            verts.Add(new Vector3(vMaxX + boldOffsetX + slantOffset, vMinY + boldOffsetY));
                            verts.Add(new Vector3(vMaxX + boldOffsetX - slantOffset, vMaxY + boldOffsetY));
                        }
                    }
                    else if (NGUIText.mTextModifiers.italic)
                    {
                        Single slantOffset = 0.1f * (vMinY - vMaxY);
                        verts.Add(new Vector3(vMinX - slantOffset, vMaxY));
                        verts.Add(new Vector3(vMinX + slantOffset, vMinY));
                        verts.Add(new Vector3(vMaxX + slantOffset, vMinY));
                        verts.Add(new Vector3(vMaxX - slantOffset, vMaxY));
                    }
                    else
                    {
                        verts.Add(new Vector3(vMinX, vMaxY));
                        verts.Add(new Vector3(vMinX, vMinY));
                        verts.Add(new Vector3(vMaxX, vMinY));
                        verts.Add(new Vector3(vMaxX, vMaxY));
                    }
                    for (Int32 i = 0; i < charCopyCount; i++)
                    {
                        uvs.Add(new Vector2(uvMinX, uvMinY));
                        uvs.Add(new Vector2(uvMinX, uvMaxY));
                        uvs.Add(new Vector2(uvMaxX, uvMaxY));
                        uvs.Add(new Vector2(uvMaxX, uvMinY));
                        cols.Add(colBottom);
                        cols.Add(colTop);
                        cols.Add(colTop);
                        cols.Add(colBottom);
                    }
                }
                if (NGUIText.mTextModifiers.underline || NGUIText.mTextModifiers.strike)
                {
                    // Generate a line striking through the character
                    NGUIText.GlyphInfo lineGlyph = NGUIText.GetGlyph(NGUIText.mTextModifiers.strike ? '-' : '_', prevCh);
                    if (lineGlyph != null)
                    {
                        if (NGUIText.mTextModifiers.strike)
                        {
                            if (displacedStrike) // Maybe for overlining?
                            {
                                vMinY = (-currentY + lineGlyph.v1.y) * 0.75f;
                                vMaxY = (-currentY + lineGlyph.v0.y) * 0.75f;
                            }
                            else // strikethrough
                            {
                                vMinY = -currentY + lineGlyph.v1.y;
                                vMaxY = -currentY + lineGlyph.v0.y;
                            }
                        }
                        else // underline
                        {
                            vMinY = -currentY + lineGlyph.v1.y - NGUIText.fontScale * NGUIText.fontSize * 0.3f;
                            vMaxY = -currentY + lineGlyph.v0.y - NGUIText.fontScale * NGUIText.fontSize * 0.3f;
                        }
                        if (NGUIText.mTextModifiers.bold)
                        {
                            for (Int32 i = 0; i < 4; i++)
                            {
                                Single boldOffsetX = NGUIText.mBoldOffset[i * 2];
                                Single boldOffsetY = NGUIText.mBoldOffset[i * 2 + 1];
                                verts.Add(new Vector3(previousX + boldOffsetX, vMaxY + boldOffsetY));
                                verts.Add(new Vector3(previousX + boldOffsetX, vMinY + boldOffsetY));
                                verts.Add(new Vector3(currentX + boldOffsetX, vMinY + boldOffsetY));
                                verts.Add(new Vector3(currentX + boldOffsetX, vMaxY + boldOffsetY));
                            }
                        }
                        else
                        {
                            verts.Add(new Vector3(previousX, vMaxY));
                            verts.Add(new Vector3(previousX, vMinY));
                            verts.Add(new Vector3(currentX, vMinY));
                            verts.Add(new Vector3(currentX, vMaxY));
                        }
                        if (NGUIText.bitmapFont != null)
                        {
                            lineGlyph.u0.x = bmUvRect.xMin + bmTextureFactorX * lineGlyph.u0.x;
                            lineGlyph.u2.x = bmUvRect.xMin + bmTextureFactorX * lineGlyph.u2.x;
                            lineGlyph.u0.y = bmUvRect.yMax - bmTextureFactorY * lineGlyph.u0.y;
                            lineGlyph.u2.y = bmUvRect.yMax - bmTextureFactorY * lineGlyph.u2.y;
                        }
                        Single lineTextureCoordX = (lineGlyph.u0.x + lineGlyph.u2.x) * 0.5f;
                        for (Int32 i = 0; i < charCopyCount; i++)
                        {
                            uvs.Add(new Vector2(lineTextureCoordX, lineGlyph.u0.y));
                            uvs.Add(new Vector2(lineTextureCoordX, lineGlyph.u2.y));
                            uvs.Add(new Vector2(lineTextureCoordX, lineGlyph.u2.y));
                            uvs.Add(new Vector2(lineTextureCoordX, lineGlyph.u0.y));
                        }
                        if (NGUIText.gradient)
                        {
                            colBottom = NGUIText.ComputeGradientColor(gradientColorBottom, gradientColorTop, lineGlyph.v0.y, ftSize);
                            colTop = NGUIText.ComputeGradientColor(gradientColorBottom, gradientColorTop, lineGlyph.v1.y, ftSize);
                            for (Int32 i = 0; i < charCopyCount; i++)
                            {
                                cols.Add(colBottom);
                                cols.Add(colTop);
                                cols.Add(colTop);
                                cols.Add(colBottom);
                            }
                        }
                        else
                        {
                            Int32 colIndexCount = charCopyCount * 4;
                            for (Int32 i = 0; i < colIndexCount; i++)
                                cols.Add(textColor);
                        }
                    }
                }
            }
            // Update the render and generate character shadows
            charFirstVIndex = parser.Vertices.size;
            for (Int32 i = 0; i < verts.Count; i++)
                parser.AddVertex(verts[i], uvs[i], cols[i], NGUIText.progressStep);
            verts.Clear();
            uvs.Clear();
            cols.Clear();
            NGUIText.GenerateShadowsOnLastChar(parser, charFirstVIndex, NGUIText.mTextModifiers.highShadow);
            NGUIText.progressStep += NGUIText.mTextModifiers.appearanceSpeed;
        }
        if (lineFirstVIndex < parser.Vertices.size)
        {
            if (useBIDI)
                currentX = allCharAdvances[textLength];
            NGUIText.Align(parser.LineInfo[printedLine], parser.Vertices, lineFirstVIndex, printedLine, currentX - NGUIText.finalSpacingX, 4, parser.SpecialImages, dialog);
        }
        parser.LineInfo[printedLine].EndVertexIndex = parser.Vertices.size;
        //DrawBoundingFrames(parser, 0, Color.red); // [DBG]
        //DrawBoundingFrames(parser, 1, Color.white);
        //DrawBoundingFrames(parser, 2, Color.green);
        parser.ApplyOffset(parser.LabelContainer.GetApplyOffset());
        parser.ComputeAppearProgressMax();
        return;
    }

    private static void DrawBoundingFrames(TextParser parser, Int32 kind, Color color)
    {
        if (kind == 0) // Render rect: encapsulate the text vertices
        {
            Rect renderRect = parser.RenderRect;
            parser.AddSegment(new Vector3(renderRect.xMin, renderRect.yMin), new Vector3(renderRect.xMax, renderRect.yMin), Color.red, Color.red, 0f);
            parser.AddSegment(new Vector3(renderRect.xMax, renderRect.yMin), new Vector3(renderRect.xMax, renderRect.yMax), Color.red, Color.red, 0f);
            parser.AddSegment(new Vector3(renderRect.xMax, renderRect.yMax), new Vector3(renderRect.xMin, renderRect.yMax), Color.red, Color.red, 0f);
            parser.AddSegment(new Vector3(renderRect.xMin, renderRect.yMax), new Vector3(renderRect.xMin, renderRect.yMin), Color.red, Color.red, 0f);
        }
        if (kind == 1) // Full size: width/height computed at WrapText step (should be a good approximate of the above)
        {
            Vector2 fullsize = parser.FullSize;
            fullsize.y *= -1;
            parser.AddSegment(Vector3.zero, new Vector3(fullsize.x, 0f), Color.white, Color.white, 0f);
            parser.AddSegment(new Vector3(fullsize.x, 0f), fullsize, Color.white, Color.white, 0f);
            parser.AddSegment(fullsize, new Vector3(0f, fullsize.y), Color.white, Color.white, 0f);
            parser.AddSegment(new Vector3(0f, fullsize.y), Vector3.zero, Color.white, Color.white, 0f);
            if (parser.LineInfo[0].Alignment == Alignment.Center)
                for (Int32 i = parser.Vertices.size - 16; i < parser.Vertices.size; i++)
                    parser.Vertices.buffer[i].x += Math.Max(0f, (NGUIText.rectWidth - fullsize.x) / 2f);
            else if (parser.LineInfo[0].Alignment == Alignment.Right)
                for (Int32 i = parser.Vertices.size - 16; i < parser.Vertices.size; i++)
                    parser.Vertices.buffer[i].x += Math.Max(0f, NGUIText.rectWidth - fullsize.x);
        }
        if (kind == 2) // Overlay size: width/height of the containing UILabel
        {
            Vector2 overlaySize = new Vector2(NGUIText.rectWidth, -NGUIText.rectHeight);
            parser.AddSegment(Vector3.zero, new Vector3(overlaySize.x, 0f), Color.green, Color.green, 0f);
            parser.AddSegment(new Vector3(overlaySize.x, 0f), overlaySize, Color.green, Color.green, 0f);
            parser.AddSegment(overlaySize, new Vector3(0f, overlaySize.y), Color.green, Color.green, 0f);
            parser.AddSegment(new Vector3(0f, overlaySize.y), Vector3.zero, Color.green, Color.green, 0f);
            Vector3 shift = new Vector3(0f, Mathf.Lerp(-parser.LabelContainer.printedSize.y, 0f, parser.LabelContainer.pivotOffset.y) - overlaySize.y / 2f);
            for (Int32 i = parser.Vertices.size - 16; i < parser.Vertices.size; i++)
                parser.Vertices[i] += shift;
        }
    }

    private static void SetupShadowEffect(TextParser parser)
    {
        NGUIText.shadowEffect = parser.LabelContainer.effectStyle;
        NGUIText.shadowDistance = parser.LabelContainer.effectDistance;
        NGUIText.shadowColor = parser.LabelContainer.effectColor;
        NGUIText.shadowColor.a *= parser.LabelContainer.finalAlpha;
        if (parser.LabelContainer.bitmapFont != null && parser.LabelContainer.bitmapFont.premultipliedAlphaShader)
            NGUIText.shadowColor = NGUITools.ApplyPMA(NGUIText.shadowColor);
    }

    private static void GenerateShadowsOnLastChar(TextParser parser, Int32 vStart, Boolean highShadow)
    {
        if ((parser.LabelContainer.bitmapFont == null || !parser.LabelContainer.bitmapFont.packedFontShader) && NGUIText.shadowEffect != UILabel.Effect.None)
        {
            Int32 vEnd = parser.Vertices.size;
            Int32 vCharEnd = vEnd;
            Int32 vCharStart = vStart;
            GenerateSingleShadow(parser, ref vStart, ref vEnd, shadowDistance.x, -shadowDistance.y);
            if (highShadow)
            {
                Single textDx = shadowDistance.x / 3f;
                Single textDy = -shadowDistance.y / 3f;
                for (Int32 i = vCharStart; i < vCharEnd; i++)
                {
                    parser.Vertices.buffer[i].x += textDx;
                    parser.Vertices.buffer[i].y += textDy;
                }
            }
            if (NGUIText.shadowEffect == UILabel.Effect.Outline || NGUIText.shadowEffect == UILabel.Effect.Outline8)
            {
                GenerateSingleShadow(parser, ref vStart, ref vEnd, -shadowDistance.x, shadowDistance.y);
                GenerateSingleShadow(parser, ref vStart, ref vEnd, shadowDistance.x, shadowDistance.y);
                GenerateSingleShadow(parser, ref vStart, ref vEnd, -shadowDistance.x, -shadowDistance.y);
                if (NGUIText.shadowEffect == UILabel.Effect.Outline8)
                {
                    GenerateSingleShadow(parser, ref vStart, ref vEnd, -shadowDistance.x, 0f);
                    GenerateSingleShadow(parser, ref vStart, ref vEnd, shadowDistance.x, 0f);
                    GenerateSingleShadow(parser, ref vStart, ref vEnd, 0f, shadowDistance.y);
                    GenerateSingleShadow(parser, ref vStart, ref vEnd, 0f, -shadowDistance.y);
                }
            }
        }
    }

    private static void GenerateSingleShadow(TextParser parser, ref Int32 vStart, ref Int32 vEnd, Single dx, Single dy)
    {
        for (Int32 i = vStart; i < vEnd; i++)
        {
            parser.AddVertex(parser.Vertices[i], parser.UVs[i], parser.Colors[i], parser.VertexAppearStep[i]);
            parser.Vertices.buffer[i].x += dx;
            parser.Vertices.buffer[i].y += dy;
            Byte bufferColorAlpha = parser.Colors[i].a;
            if (bufferColorAlpha == Byte.MaxValue)
            {
                parser.Colors[i] = NGUIText.shadowColor;
            }
            else
            {
                Color vertColor = NGUIText.shadowColor;
                vertColor.a *= bufferColorAlpha / 255f;
                if (parser.LabelContainer.bitmapFont != null && parser.LabelContainer.bitmapFont.premultipliedAlphaShader)
                    vertColor = NGUITools.ApplyPMA(vertColor);
                parser.Colors[i] = vertColor;
            }
        }
        vStart = vEnd;
        vEnd = parser.Vertices.size;
    }

    public static void PrintApproximateCharacterPositions(String text, BetterList<Vector3> verts, BetterList<Int32> indices)
    {
        //if (String.IsNullOrEmpty(text))
        //    text = " ";
        //NGUIText.Prepare(text);
        //Single currentX = 0f;
        //Single textHeight = 0f;
        //Single maxLineWidth = 0f;
        //Single halfLineHeight = NGUIText.fontSize * NGUIText.fontScale * 0.5f;
        //Int32 textLength = text.Length;
        //Int32 vIndex = verts.size;
        //Int32 prevCh = 0;
        //NGUIText.mTextModifiers.Reset();
        //for (Int32 texti = 0; texti < textLength; texti++)
        //{
        //    Int32 ch = text[texti];
        //    verts.Add(new Vector3(currentX, -textHeight - halfLineHeight));
        //    indices.Add(texti);
        //    if (ch == '\n')
        //    {
        //        if (currentX > maxLineWidth)
        //            maxLineWidth = currentX;
        //        if (NGUIText.alignment != NGUIText.Alignment.Left)
        //        {
        //            NGUIText.Align(verts, vIndex, -1, currentX - NGUIText.finalSpacingX, 1, null);
        //            vIndex = verts.size;
        //        }
        //        currentX = 0f;
        //        textHeight += NGUIText.finalLineHeight;
        //        prevCh = 0;
        //    }
        //    else if (ch < 32)
        //    {
        //        prevCh = 0;
        //    }
        //    else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref texti, NGUIText.mTextModifiers))
        //    {
        //        texti--;
        //    }
        //    else
        //    {
        //        BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
        //        if (bmsymbol == null)
        //        {
        //            Single glyphWidth = NGUIText.GetGlyphWidth(ch, prevCh);
        //            if (glyphWidth != 0f)
        //            {
        //                glyphWidth += NGUIText.finalSpacingX;
        //                if (Mathf.RoundToInt(currentX + glyphWidth) > NGUIText.regionWidth)
        //                {
        //                    if (currentX == 0f)
        //                        return;
        //                    if (NGUIText.alignment != NGUIText.Alignment.Left && vIndex < verts.size)
        //                    {
        //                        NGUIText.Align(verts, vIndex, -1, currentX - NGUIText.finalSpacingX, 1, null);
        //                        vIndex = verts.size;
        //                    }
        //                    currentX = glyphWidth;
        //                    textHeight += NGUIText.finalLineHeight;
        //                }
        //                else
        //                {
        //                    currentX += glyphWidth;
        //                }
        //                verts.Add(new Vector3(currentX, -textHeight - halfLineHeight));
        //                indices.Add(texti + 1);
        //                prevCh = ch;
        //            }
        //        }
        //        else
        //        {
        //            Single advanceX = bmsymbol.advance * NGUIText.fontScale + NGUIText.finalSpacingX;
        //            if (Mathf.RoundToInt(currentX + advanceX) > NGUIText.regionWidth)
        //            {
        //                if (currentX == 0f)
        //                    return;
        //                if (NGUIText.alignment != NGUIText.Alignment.Left && vIndex < verts.size)
        //                {
        //                    NGUIText.Align(verts, vIndex, -1, currentX - NGUIText.finalSpacingX, 1, null);
        //                    vIndex = verts.size;
        //                }
        //                currentX = advanceX;
        //                textHeight += NGUIText.finalLineHeight;
        //            }
        //            else
        //            {
        //                currentX += advanceX;
        //            }
        //            verts.Add(new Vector3(currentX, -textHeight - halfLineHeight));
        //            indices.Add(texti + 1);
        //            texti += bmsymbol.sequence.Length - 1;
        //            prevCh = 0;
        //        }
        //    }
        //}
        //if (NGUIText.alignment != NGUIText.Alignment.Left && vIndex < verts.size)
        //    NGUIText.Align(verts, vIndex, -1, currentX - NGUIText.finalSpacingX, 1, null);
    }

    public static void PrintExactCharacterPositions(String text, BetterList<Vector3> verts, BetterList<Int32> indices)
    {
        //if (String.IsNullOrEmpty(text))
        //    text = " ";
        //NGUIText.Prepare(text);
        //Single lineHeight = NGUIText.fontSize * NGUIText.fontScale;
        //Single currentX = 0f;
        //Single textHeight = 0f;
        //Single maxLineWidth = 0f;
        //Int32 textLength = text.Length;
        //Int32 vIndex = verts.size;
        //Int32 prevCh = 0;
        //NGUIText.mTextModifiers.Reset();
        //for (Int32 texti = 0; texti < textLength; texti++)
        //{
        //    Int32 ch = text[texti];
        //    if (ch == '\n')
        //    {
        //        if (currentX > maxLineWidth)
        //            maxLineWidth = currentX;
        //        if (NGUIText.alignment != NGUIText.Alignment.Left)
        //        {
        //            NGUIText.Align(verts, vIndex, -1, currentX - NGUIText.finalSpacingX, 2, null);
        //            vIndex = verts.size;
        //        }
        //        currentX = 0f;
        //        textHeight += NGUIText.finalLineHeight;
        //        prevCh = 0;
        //    }
        //    else if (ch < 32)
        //    {
        //        prevCh = 0;
        //    }
        //    else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref texti, NGUIText.mTextModifiers))
        //    {
        //        texti--;
        //    }
        //    else
        //    {
        //        BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
        //        if (bmsymbol == null)
        //        {
        //            Single glyphWidth = NGUIText.GetGlyphWidth(ch, prevCh);
        //            if (glyphWidth != 0f)
        //            {
        //                Single advanceX = glyphWidth + NGUIText.finalSpacingX;
        //                if (Mathf.RoundToInt(currentX + advanceX) > NGUIText.regionWidth)
        //                {
        //                    if (currentX == 0f)
        //                        return;
        //                    if (NGUIText.alignment != NGUIText.Alignment.Left && vIndex < verts.size)
        //                    {
        //                        NGUIText.Align(verts, vIndex, -1, currentX - NGUIText.finalSpacingX, 2, null);
        //                        vIndex = verts.size;
        //                    }
        //                    currentX = 0f;
        //                    textHeight += NGUIText.finalLineHeight;
        //                    prevCh = 0;
        //                    texti--;
        //                }
        //                else
        //                {
        //                    indices.Add(texti);
        //                    verts.Add(new Vector3(currentX, -textHeight - lineHeight));
        //                    verts.Add(new Vector3(currentX + advanceX, -textHeight));
        //                    prevCh = ch;
        //                    currentX += advanceX;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Single advanceX = bmsymbol.advance * NGUIText.fontScale + NGUIText.finalSpacingX;
        //            if (Mathf.RoundToInt(currentX + advanceX) > NGUIText.regionWidth)
        //            {
        //                if (currentX == 0f)
        //                    return;
        //                if (NGUIText.alignment != NGUIText.Alignment.Left && vIndex < verts.size)
        //                {
        //                    NGUIText.Align(verts, vIndex, -1, currentX - NGUIText.finalSpacingX, 2, null);
        //                    vIndex = verts.size;
        //                }
        //                currentX = 0f;
        //                textHeight += NGUIText.finalLineHeight;
        //                prevCh = 0;
        //                texti--;
        //            }
        //            else
        //            {
        //                indices.Add(texti);
        //                verts.Add(new Vector3(currentX, -textHeight - lineHeight));
        //                verts.Add(new Vector3(currentX + advanceX, -textHeight));
        //                texti += bmsymbol.sequence.Length - 1;
        //                currentX += advanceX;
        //                prevCh = 0;
        //            }
        //        }
        //    }
        //}
        //if (NGUIText.alignment != NGUIText.Alignment.Left && vIndex < verts.size)
        //    NGUIText.Align(verts, vIndex, -1, currentX - NGUIText.finalSpacingX, 2, null);
    }

    public static void PrintCaretAndSelection(String text, Int32 start, Int32 end, BetterList<Vector3> caret, BetterList<Vector3> highlight)
    {
        //if (String.IsNullOrEmpty(text))
        //    text = " ";
        //NGUIText.Prepare(text);
        //Int32 caretPos = end;
        //if (start > end)
        //{
        //    end = start;
        //    start = caretPos;
        //}
        //Single currentX = 0f;
        //Single textHeight = 0f;
        //Single maxLineWidth = 0f;
        //Single lineHeight = NGUIText.fontSize * NGUIText.fontScale;
        //Int32 caretStartIndex = caret == null ? 0 : caret.size;
        //Int32 highlightIndex = highlight == null ? 0 : highlight.size;
        //Int32 textLength = text.Length;
        //Int32 prevCh = 0;
        //Boolean highlightStarted = false;
        //Boolean caretInitialized = false;
        //Vector2 charEndPos1 = Vector2.zero;
        //Vector2 charEndPos2 = Vector2.zero;
        //Int32 texti;
        //NGUIText.mTextModifiers.Reset();
        //for (texti = 0; texti < textLength; texti++)
        //{
        //    if (caret != null && !caretInitialized && caretPos <= texti)
        //    {
        //        caretInitialized = true;
        //        caret.Add(new Vector3(currentX - 1f, -textHeight - lineHeight - 6f));
        //        caret.Add(new Vector3(currentX - 1f, -textHeight + 6f));
        //        caret.Add(new Vector3(currentX + 1f, -textHeight + 6f));
        //        caret.Add(new Vector3(currentX + 1f, -textHeight - lineHeight - 6f));
        //    }
        //    Int32 ch = text[texti];
        //    if (ch == '\n')
        //    {
        //        if (currentX > maxLineWidth)
        //            maxLineWidth = currentX;
        //        if (caret != null && caretInitialized)
        //        {
        //            if (NGUIText.alignment != NGUIText.Alignment.Left)
        //                NGUIText.Align(caret, caretStartIndex, -1, currentX - NGUIText.finalSpacingX, 4, null);
        //            caret = null;
        //        }
        //        if (highlight != null)
        //        {
        //            if (highlightStarted)
        //            {
        //                highlightStarted = false;
        //                highlight.Add(charEndPos2);
        //                highlight.Add(charEndPos1);
        //            }
        //            else if (start <= texti && end > texti)
        //            {
        //                highlight.Add(new Vector3(currentX, -textHeight - lineHeight));
        //                highlight.Add(new Vector3(currentX, -textHeight));
        //                highlight.Add(new Vector3(currentX + 2f, -textHeight));
        //                highlight.Add(new Vector3(currentX + 2f, -textHeight - lineHeight));
        //            }
        //            if (NGUIText.alignment != NGUIText.Alignment.Left && highlightIndex < highlight.size)
        //            {
        //                NGUIText.Align(highlight, highlightIndex, -1, currentX - NGUIText.finalSpacingX, 4, null);
        //                highlightIndex = highlight.size;
        //            }
        //        }
        //        currentX = 0f;
        //        textHeight += NGUIText.finalLineHeight;
        //        prevCh = 0;
        //    }
        //    else if (ch < 32)
        //    {
        //        prevCh = 0;
        //    }
        //    else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref texti, NGUIText.mTextModifiers))
        //    {
        //        texti--;
        //    }
        //    else
        //    {
        //        BMSymbol bmsymbol = NGUIText.useSymbols ? NGUIText.GetSymbol(text, texti, textLength) : null;
        //        Single glyphWidth = bmsymbol == null ? NGUIText.GetGlyphWidth(ch, prevCh) : bmsymbol.advance * NGUIText.fontScale;
        //        if (glyphWidth != 0f)
        //        {
        //            Single charStartX = currentX;
        //            Single charEndX = currentX + glyphWidth;
        //            Single charStartY = -textHeight - lineHeight - 6f;
        //            Single charEndY = -textHeight + 6f;
        //            if (Mathf.RoundToInt(charEndX + NGUIText.finalSpacingX) > NGUIText.regionWidth)
        //            {
        //                if (currentX == 0f)
        //                    return;
        //                if (currentX > maxLineWidth)
        //                    maxLineWidth = currentX;
        //                if (caret != null && caretInitialized)
        //                {
        //                    if (NGUIText.alignment != NGUIText.Alignment.Left)
        //                        NGUIText.Align(caret, caretStartIndex, -1, currentX - NGUIText.finalSpacingX, 4, null);
        //                    caret = null;
        //                }
        //                if (highlight != null)
        //                {
        //                    if (highlightStarted)
        //                    {
        //                        highlightStarted = false;
        //                        highlight.Add(charEndPos2);
        //                        highlight.Add(charEndPos1);
        //                    }
        //                    else if (start <= texti && end > texti)
        //                    {
        //                        highlight.Add(new Vector3(currentX, -textHeight - lineHeight));
        //                        highlight.Add(new Vector3(currentX, -textHeight));
        //                        highlight.Add(new Vector3(currentX + 2f, -textHeight));
        //                        highlight.Add(new Vector3(currentX + 2f, -textHeight - lineHeight));
        //                    }
        //                    if (NGUIText.alignment != NGUIText.Alignment.Left && highlightIndex < highlight.size)
        //                    {
        //                        NGUIText.Align(highlight, highlightIndex, -1, currentX - NGUIText.finalSpacingX, 4, null);
        //                        highlightIndex = highlight.size;
        //                    }
        //                }
        //                charStartX -= currentX;
        //                charEndX -= currentX;
        //                charStartY -= NGUIText.finalLineHeight;
        //                charEndY -= NGUIText.finalLineHeight;
        //                currentX = 0f;
        //                textHeight += NGUIText.finalLineHeight;
        //            }
        //            currentX += glyphWidth + NGUIText.finalSpacingX;
        //            if (highlight != null)
        //            {
        //                if (start > texti || end <= texti)
        //                {
        //                    if (highlightStarted)
        //                    {
        //                        highlightStarted = false;
        //                        highlight.Add(charEndPos2);
        //                        highlight.Add(charEndPos1);
        //                    }
        //                }
        //                else if (!highlightStarted)
        //                {
        //                    highlightStarted = true;
        //                    highlight.Add(new Vector3(charStartX, charStartY));
        //                    highlight.Add(new Vector3(charStartX, charEndY));
        //                }
        //            }
        //            charEndPos1 = new Vector2(charEndX, charStartY);
        //            charEndPos2 = new Vector2(charEndX, charEndY);
        //            prevCh = ch;
        //        }
        //    }
        //}
        //if (caret != null)
        //{
        //    if (!caretInitialized)
        //    {
        //        caret.Add(new Vector3(currentX - 1f, -textHeight - lineHeight - 6f));
        //        caret.Add(new Vector3(currentX - 1f, -textHeight + 6f));
        //        caret.Add(new Vector3(currentX + 1f, -textHeight + 6f));
        //        caret.Add(new Vector3(currentX + 1f, -textHeight - lineHeight - 6f));
        //    }
        //    if (NGUIText.alignment != NGUIText.Alignment.Left)
        //        NGUIText.Align(caret, caretStartIndex, -1, currentX - NGUIText.finalSpacingX, 4, null);
        //}
        //if (highlight != null)
        //{
        //    if (highlightStarted)
        //    {
        //        highlight.Add(charEndPos2);
        //        highlight.Add(charEndPos1);
        //    }
        //    else if (start < texti && end == texti)
        //    {
        //        highlight.Add(new Vector3(currentX, -textHeight - lineHeight));
        //        highlight.Add(new Vector3(currentX, -textHeight));
        //        highlight.Add(new Vector3(currentX + 2f, -textHeight));
        //        highlight.Add(new Vector3(currentX + 2f, -textHeight - lineHeight));
        //    }
        //    if (NGUIText.alignment != NGUIText.Alignment.Left && highlightIndex < highlight.size)
        //        NGUIText.Align(highlight, highlightIndex, -1, currentX - NGUIText.finalSpacingX, 4, null);
        //}
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
    public const String Justified = "JSTF";
    public const String Mirrored = "MIRR";
    public const String Superscript = "sup";
    public const String Subscript = "sub";
    public const String Hyperlink = "url";
    public const String Bold = "b";
    public const String Italic = "i";
    public const String Underline = "u";
    public const String Strikethrough = "s";
    public const String IgnoreColor = "c";

    public const String FF9WhiteColor = "[C8C8C8]";
    public const String FF9YellowColor = "[C8B040]";
    public const String FF9PinkColor = "[B880E0]";
    public const String FF9BlueColor = "[2870FB]";
    public const String FF9DarkBlueColor = "[383840]";

    public const Int32 MobileTouchToConfirmJP = 322;
    public const Int32 MobileTouchToConfirmUS = 323;

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
    public static Boolean fixedAlignment = false;

    public static UILabel.Effect shadowEffect = UILabel.Effect.None;
    public static Vector2 shadowDistance = Vector2.zero;
    public static Color shadowColor = Color.white;

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

    public static Single progressStep = 0f;
    public static Single choiceIndent = 0f;
    public static Int32 finalSize = 0;
    public static Single finalSpacingX = 0f;
    public static Single finalLineHeight = 0f;
    public static Single baseline = 0f;
    public static Boolean useSymbols = false;

    /// <summary>Sorted like enum Control</summary>
    public static String[] ButtonNames =
    [
        "CROSS",
        "CIRCLE",
        "TRIANGLE",
        "SQUARE",
        "L1",
        "R1",
        "L2",
        "R2",
        "START",
        "SELECT",
        "UP",
        "DOWN",
        "LEFT",
        "RIGHT",
        "PAD"
    ];

    internal static Color mInvisible = new Color(0f, 0f, 0f, 0f);
    internal static Single mAlpha = 1f;

    private static Boolean forceShowButton = false;

    private static FFIXTextModifier mTextModifiers = new FFIXTextModifier();

    private static CharacterInfo mTempChar;

    private static BetterList<Single> mSizes = new BetterList<Single>();

    private static Single[] mBoldOffset = new Single[]
    {
        -0.25f, 0f,
        0.25f,  0f,
        0f,     -0.25f,
        0f,     0.25f
    };

    private static readonly HashSet<Int32> IconIdException =
    [
        19,  // arrow_up
        20,  // arrow_down
        192, // ap_bar_complete_star
        254, // ap_bar_full
        255  // ap_bar_half
    ];

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
