using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using Memoria.Prime.Text;

namespace Memoria.Assets
{
    public static class DialogBoxSymbols
    {
        public static List<String> ParseTextSplitTags(String text)
        {
            List<String> pages = new List<String>();
            Int32 startPos = 0;
            Int32 textPos = 0;
            while (textPos < text.Length)
            {
                Int32 tagEndPos = textPos;
                FFIXTextTag tag = FFIXTextTag.TryRead(text, ref tagEndPos);
                if (tag != null)
                {
                    switch (tag.Code)
                    {
                        case FFIXTextTagCode.End:
                            pages.Add(text.Substring(startPos, textPos - startPos));
                            return pages;
                        case FFIXTextTagCode.NewPage:
                            pages.Add(text.Substring(startPos, textPos - startPos));
                            startPos = tagEndPos;
                            break;
                        case FFIXTextTagCode.Table:
                            if (text.EndsWith("[ENDN]"))
                                text = text.Remove(text.Length - "[ENDN]".Length);
                            return text.Substring(tagEndPos).Split('\n').ToList();
                    }
                    textPos = tagEndPos;
                }
                else
                {
                    textPos++;
                }
            }
            pages.Add(text.Substring(startPos, textPos - startPos));
            return pages;
        }

        public static String ParseSingleConstantTextReplaceTag(FFIXTextTag tag)
        {
            switch (tag.Code)
            {
                case FFIXTextTagCode.CharacterName:
                    return FF9StateSystem.Common.FF9.GetPlayer((CharacterId)tag.IntParam(0))?.Name;
                case FFIXTextTagCode.Party:
                    return FF9StateSystem.Common.FF9.GetPlayer(PersistenSingleton<EventEngine>.Instance.GetEventPartyPlayer(tag.IntParam(0)))?.Name;
                case FFIXTextTagCode.Text:
                    return ETb.GetStringFromTable(tag.UIntParam(0), tag.UIntParam(1));
                case FFIXTextTagCode.Zidane:
                    return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Zidane).Name;
                case FFIXTextTagCode.Vivi:
                    return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Vivi).Name;
                case FFIXTextTagCode.Dagger:
                    return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Garnet).Name;
                case FFIXTextTagCode.Steiner:
                    return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Steiner).Name;
                case FFIXTextTagCode.Freya:
                case FFIXTextTagCode.Fraya:
                    return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Freya).Name;
                case FFIXTextTagCode.Quina:
                    return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Quina).Name;
                case FFIXTextTagCode.Eiko:
                    return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Eiko).Name;
                case FFIXTextTagCode.Amarant:
                    return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Amarant).Name;
            }
            return String.Empty;
        }

        public static void ParseInitialAndConstantTextTags(TextParser parser)
        {
            if (!parser.LabelContainer.supportEncoding || parser.ParsedText.Length == 0)
                return;
            StringBuilder sb = new StringBuilder(parser.ParsedText.Length + 10);
            Dialog dialog = parser.LabelContainer.DialogWindow;
            List<String> parsingText = [parser.ParsedText];
            List<Int32> parsingTextPos = [0];
            while (parsingText.Count > 0)
            {
                Int32 stackIndex = parsingText.Count - 1;
                Int32 tagEndPos = parsingTextPos[stackIndex];
                FFIXTextTag tag = FFIXTextTag.TryRead(parsingText[stackIndex], ref tagEndPos);
                if (tag != null)
                {
                    if (FFIXTextTag.ConstantTextReplaceTags.Contains(tag.Code))
                    {
                        if (tag.Code == FFIXTextTagCode.Icon)
                        {
                            if (!PatchIconTag(tag, parser, sb))
                            {
                                tag.TextOffset = sb.Length;
                                parser.ParsedTagList.Add(tag);
                            }
                        }
                        else if (tag.Code == FFIXTextTagCode.Text)
                        {
                            parsingTextPos.Add(0);
                            parsingText.Add(DialogBoxSymbols.ParseSingleConstantTextReplaceTag(tag));
                        }
                        else
                        {
                            sb.Append(DialogBoxSymbols.ParseSingleConstantTextReplaceTag(tag));
                        }
                        parsingTextPos[stackIndex] = tagEndPos;
                    }
                    else
                    {
                        if (!ApplyFormatTag(tag, dialog))
                        {
                            tag.TextOffset = sb.Length;
                            parser.ParsedTagList.Add(tag);
                        }
                        else if (tag.Code == FFIXTextTagCode.TagAnimation)
                        {
                            if (FFIXTextTag.OriginalTagNames.TryGetValue(tag.StringParam(0), out FFIXTextTagCode animatedCode) || tag.StringParam(0).TryEnumParse(out animatedCode))
                            {
                                FFIXTextTag animatedTag = new FFIXTextTag(animatedCode);
                                TextAnimatedTag animTag = new TextAnimatedTag(tag, animatedTag);
                                animatedTag.TextOffset = sb.Length;
                                parser.ParsedTagList.Add(animatedTag);
                                parser.AnimatedTags.Add(animTag);
                            }
                        }
                        parsingTextPos[stackIndex] = tagEndPos;
                    }
                }
                else
                {
                    sb.Append(parsingText[stackIndex][parsingTextPos[stackIndex]++]);
                }
                stackIndex = parsingText.Count - 1;
                while (stackIndex >= 0 && parsingTextPos[stackIndex] >= parsingText[stackIndex].Length)
                {
                    parsingText.RemoveAt(stackIndex);
                    parsingTextPos.RemoveAt(stackIndex);
                    stackIndex--;
                }
            }
            parser.ParsedText = sb.ToString();
        }

        public static void ParseVariableTextReplaceTags(TextParser parser)
        {
            Dialog dialog = parser.LabelContainer.DialogWindow;
            for (Int32 i = 0; i < parser.ParsedTagList.Count; i++)
            {
                FFIXTextTag tag = parser.ParsedTagList[i];
                if (tag.Code == FFIXTextTagCode.Variable)
                {
                    Int32 mesScriptId = tag.IntParam(0);
                    Int32 numbOffset = parser.ParsedTagList[i].TextOffset;
                    Boolean isSelectedOverlay = tag.ParamCount >= 2 ? ETb.gMesValue[tag.IntParam(1)] == mesScriptId : dialog != null && dialog.OverlayMessageNumber == mesScriptId;
                    parser.VariableMessageValues[mesScriptId] = ETb.gMesValue[mesScriptId];
                    if (isSelectedOverlay)
                        parser.ReplaceTag(i--, ETb.gMesValue[mesScriptId].ToString(), [new FFIXTextTag(FFIXTextTagCode.Pink), new FFIXTextTag(FFIXTextTagCode.ShadowToggle)], [new FFIXTextTag(FFIXTextTagCode.White), new FFIXTextTag(FFIXTextTagCode.ShadowToggle)]);
                    else
                        parser.ReplaceTag(i--, ETb.gMesValue[mesScriptId].ToString());
                }
                else if (tag.Code == FFIXTextTagCode.Item)
                {
                    Int32 mesScriptId = tag.IntParam(0);
                    Int32 itemOffset = parser.ParsedTagList[i].TextOffset;
                    String itemName = ETb.GetItemName(ETb.gMesValue[mesScriptId]);
                    parser.VariableMessageValues[mesScriptId] = ETb.gMesValue[mesScriptId];
                    parser.ReplaceTag(i--, itemName, [new FFIXTextTag(FFIXTextTagCode.Yellow), new FFIXTextTag(FFIXTextTagCode.ShadowToggle)], [new FFIXTextTag(FFIXTextTagCode.White), new FFIXTextTag(FFIXTextTagCode.ShadowToggle)]);
                }
            }
        }

        public static void ParseChoiceTags(TextParser parser)
        {
            Dialog dialog = parser.LabelContainer.DialogWindow;
            if (dialog == null)
                return;
            Int32 choiceIndex = parser.ParsedTagList.FindIndex(tag => tag.Code == FFIXTextTagCode.Choice);
            if (choiceIndex < 0)
                return;
            Int32 choicePos = parser.ParsedTagList[choiceIndex].TextOffset;
            FFIXTextTag preChoice = parser.ParsedTagList.FirstOrDefault(tag => tag.Code == FFIXTextTagCode.PreChoose); // can be null
            FFIXTextTag preChoiceMask = parser.ParsedTagList.FirstOrDefault(tag => tag.Code == FFIXTextTagCode.PreChooseMask); // can be null
            Int32 choiceCount = parser.ParsedText.Substring(choicePos).Count(c => c == '\n') + 1;
            dialog.StartChoiceRow = parser.ParsedText.Substring(0, choicePos).Count(c => c == '\n');
            SetupChoose(preChoice, preChoiceMask, dialog, choiceCount);
            if (dialog.DisableIndexes.Count > 0)
            {
                List<KeyValuePair<Int32, Int32>> linesToRemove = new List<KeyValuePair<Int32, Int32>>();
                if (dialog.StartChoiceRow == 0)
                    choicePos = 0;
                else
                    choicePos = parser.ParsedText.Substring(0, choicePos).LastIndexOf('\n');
                for (Int32 i = 0; i < dialog.ChoiceNumber; i++)
                {
                    Int32 endChoicePos = parser.ParsedText.IndexOf('\n', choicePos + 1);
                    if (endChoicePos < 0)
                        endChoicePos = parser.ParsedText.Length;
                    if (dialog.DisableIndexes.Contains(i))
                        linesToRemove.Add(new KeyValuePair<Int32, Int32>(choicePos, endChoicePos - choicePos));
                    choicePos = endChoicePos;
                    if (endChoicePos == parser.ParsedText.Length)
                        break;
                }
                for (Int32 i = linesToRemove.Count - 1; i >= 0; i--)
                    parser.RemovePart(linesToRemove[i].Key, linesToRemove[i].Value);
            }
        }

        public static Boolean ParseSingleGenericTag(FFIXTextTag tag, Dialog dialog, UILabel label, FFIXTextModifier modifiers)
        {
            switch (tag.Code)
            {
                case FFIXTextTagCode.Wait:
                    NGUIText.progressStep = Math.Max(0f, NGUIText.progressStep + tag.SingleParam(0) / 30f);
                    break;
                case FFIXTextTagCode.Speed:
                    modifiers.SetAppearanceSpeed(tag.SingleParam(0));
                    break;
                case FFIXTextTagCode.Tab:
                    OnTab(modifiers, 18f, 0f);
                    return true;
                case FFIXTextTagCode.MoveCaret:
                    OnTab(modifiers, tag.SingleParam(0), tag.SingleParam(1));
                    return true;
                case FFIXTextTagCode.Choice:
                    modifiers.choice = true;
                    if (dialog == null || dialog.IsETbDialog)
                        modifiers.frameOffset.x += NGUIText.ChoiceIndent;
                    break;
                case FFIXTextTagCode.ShadowToggle:
                    if (tag.ParamCount > 0)
                        tag.StringParam(0).TryEnumParse(out NGUIText.shadowEffect);
                    else
                        modifiers.highShadow = !modifiers.highShadow;
                    return true;
                case FFIXTextTagCode.ShadowOff:
                    modifiers.highShadow = false;
                    return true;
                case FFIXTextTagCode.TextRGBA:
                    OnTextColorRGBA(modifiers, tag);
                    return true;
                case FFIXTextTagCode.White:
                    OnTextColor(modifiers, FF9TextTool.White);
                    return true;
                case FFIXTextTagCode.Pink:
                    OnTextColor(modifiers, FF9TextTool.Magenta);
                    return true;
                case FFIXTextTagCode.Cyan:
                    OnTextColor(modifiers, FF9TextTool.Cyan);
                    return true;
                case FFIXTextTagCode.Brown:
                    OnTextColor(modifiers, FF9TextTool.Red);
                    return true;
                case FFIXTextTagCode.Yellow:
                    OnTextColor(modifiers, FF9TextTool.Yellow);
                    return true;
                case FFIXTextTagCode.Green:
                    OnTextColor(modifiers, FF9TextTool.Green);
                    return true;
                case FFIXTextTagCode.Grey:
                    OnTextColor(modifiers, FF9TextTool.Gray);
                    return true;
                case FFIXTextTagCode.BackgroundRGBA:
                    OnBackgroundColorRGBA(modifiers, tag);
                    return true;
                case FFIXTextTagCode.ChangeFont:
                    OnFontChange(modifiers, tag);
                    return true;
                case FFIXTextTagCode.Time:
                    OnTime(dialog, tag.IntParam(0));
                    return true;
                case FFIXTextTagCode.IncreaseSignal:
                    OnTime(dialog, -1);
                    return true;
                case FFIXTextTagCode.DialogY:
                    modifiers.extraOffset.y += tag.SingleParam(0) * UIManager.ResourceYMultipier;
                    return true;
                case FFIXTextTagCode.DialogX:
                    modifiers.tabX = tag.SingleParam(0) * UIManager.ResourceXMultipier;
                    NGUIText.fixedLineAlignment = true;
                    return true;
                case FFIXTextTagCode.DialogF:
                    OnFeed(modifiers, tag.SingleParam(0));
                    return true;
                case FFIXTextTagCode.SpacingY:
                    if (label != null)
                        label.spacingY = tag.IntParam(0);
                    return true;
                case FFIXTextTagCode.TextFrame:
                    // TODO: might want to add optional parameters, such as width/height, maybe a frame outline or internal color...
                    // Also, it currently doesn't work correctly with RTL reading direction
                    modifiers.frameOffset.x += tag.SingleParam(0) * UIManager.ResourceXMultipier;
                    modifiers.frameOffset.y += tag.SingleParam(1) * UIManager.ResourceYMultipier;
                    modifiers.tabX = 0f;
                    return true;
                case FFIXTextTagCode.TurboOff:
                    UIKeyTrigger.preventTurboKey = true;
                    return true;
                case FFIXTextTagCode.Center:
                    modifiers.center = true;
                    return true;
                case FFIXTextTagCode.Justified:
                    modifiers.justified = !modifiers.justified;
                    return true;
                case FFIXTextTagCode.Mirrored:
                    modifiers.mirror = !modifiers.mirror;
                    return true;
                case FFIXTextTagCode.Superscript:
                    modifiers.sub = tag.StringParam(0) != "Off" ? 2 : 0;
                    return true;
                case FFIXTextTagCode.Subscript:
                    modifiers.sub = tag.StringParam(0) != "Off" ? 1 : 0;
                    return true;
                case FFIXTextTagCode.Hyperlink:
                    // Should that really be supported?
                    return true;
                case FFIXTextTagCode.Bold:
                    modifiers.bold = tag.StringParam(0) != "Off";
                    return true;
                case FFIXTextTagCode.Italic:
                    modifiers.italic = tag.StringParam(0) != "Off";
                    return true;
                case FFIXTextTagCode.Underline:
                    modifiers.underline = tag.StringParam(0) != "Off";
                    return true;
                case FFIXTextTagCode.Strikethrough:
                    modifiers.strike = tag.StringParam(0) != "Off";
                    return true;
                case FFIXTextTagCode.IgnoreColor:
                    modifiers.ignoreColor = tag.StringParam(0) != "Off";
                    return true;
            }
            return ParseImageTag(tag, ref modifiers.insertImage);
        }

        public static Boolean ParseImageTag(FFIXTextTag tag, ref DialogImage insertImage)
        {
            switch (tag.Code)
            {
                case FFIXTextTagCode.Sprite:
                    insertImage = NGUIText.CreateSpriteImage(tag.Param);
                    return true;
                case FFIXTextTagCode.Icon:
                    insertImage = NGUIText.CreateIconImage(tag.IntParam(0));
                    return true;
                case FFIXTextTagCode.IconEx:
                    if ((ETb.gMesValue[0] & (1 << tag.IntParam(0))) != 0) // [DBG] icon is shifted in dialogs?! Moguo tutorials
                        insertImage = NGUIText.CreateIconImage(FF9UIDataTool.NewIconId);
                    return true;
                case FFIXTextTagCode.Mobile:
                    if (NGUIText.ShowMobileButtons)
                        insertImage = NGUIText.CreateIconImage(tag.IntParam(0));
                    return true;
                case FFIXTextTagCode.DefaultButton:
                    insertImage = NGUIText.CreateButtonImage(tag.StringParam(0), false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.CustomButton:
                    insertImage = NGUIText.CreateButtonImage(tag.StringParam(0), true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.KeyboardButton:
                    insertImage = NGUIText.CreateButtonImage(tag.StringParam(0), false, NGUIText.KeyboardButtonIcon);
                    return true;
                case FFIXTextTagCode.JoyStickButton:
                    insertImage = NGUIText.CreateButtonImage(tag.StringParam(0), true, NGUIText.JoyStickButtonIcon);
                    return true;
                case FFIXTextTagCode.Up:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Up], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.Down:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Down], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.Left:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Left], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.Right:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Right], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.Circle:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Cancel], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.Cross:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Confirm], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.Triangle:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Menu], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.Square:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Special], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.R1:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.RightBumper], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.R2:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.RightTrigger], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.L1:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.LeftBumper], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.L2:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.LeftTrigger], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.Select:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Select], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.Start:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Pause], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.Pad:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.DPad], false, NGUIText.ButtonIcon);
                    return true;
                case FFIXTextTagCode.UpEx:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Up], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.DownEx:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Down], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.LeftEx:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Left], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.RightEx:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Right], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.CircleEx:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Cancel], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.CrossEx:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Confirm], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.TriangleEx:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Menu], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.SquareEx:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Special], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.R1Ex:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.RightBumper], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.R2Ex:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.RightTrigger], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.L1Ex:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.LeftBumper], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.L2Ex:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.LeftTrigger], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.SelectEx:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Select], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.StartEx:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.Pause], true, NGUIText.CustomButtonIcon);
                    return true;
                case FFIXTextTagCode.PadEx:
                    insertImage = NGUIText.CreateButtonImage(NGUIText.ButtonNames[(Int32)Control.DPad], true, NGUIText.CustomButtonIcon);
                    return true;
            }
            return false;
        }

        public static void OnAppearTag(FFIXTextTag tag, Dialog dialog, UILabel label)
        {
            switch (tag.Code)
            {
                case FFIXTextTagCode.IncreaseSignal:
                    OnSignal(dialog, -1);
                    break;
                case FFIXTextTagCode.IncreaseSignalEx:
                    OnSignal(dialog, -1);
                    break;
                case FFIXTextTagCode.Signal:
                    OnSignal(dialog, tag.IntParam(0));
                    break;
                case FFIXTextTagCode.Sound:
                    OnSound(tag);
                    break;
            }
        }

        private static Boolean PatchIconTag(FFIXTextTag tag, TextParser parser, StringBuilder sb)
        {
            switch (tag.IntParam(0))
            {
                case 34:
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Subscript, null, sb.Length));
                    sb.Append("0");
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Subscript, ["Off"], sb.Length));
                    return true;
                case 35:
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Subscript, null, sb.Length));
                    sb.Append("1");
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Subscript, ["Off"], sb.Length));
                    return true;
                case 39:
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Subscript, null, sb.Length));
                    sb.Append("5");
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Subscript, ["Off"], sb.Length));
                    return true;
                case 45:
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Subscript, null, sb.Length));
                    sb.Append("/");
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Subscript, ["Off"], sb.Length));
                    return true;
                case 159:
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Superscript, null, sb.Length));
                    sb.Append(Localization.Get("Miss"));
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Superscript, ["Off"], sb.Length));
                    return true;
                case 160:
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Superscript, null, sb.Length));
                    sb.Append(Localization.Get("Death"));
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Superscript, ["Off"], sb.Length));
                    return true;
                case 161:
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Superscript, null, sb.Length));
                    sb.Append(Localization.Get("Guard"));
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Superscript, ["Off"], sb.Length));
                    return true;
                case 163:
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Superscript, null, sb.Length));
                    sb.Append(Localization.Get("MPCaption"));
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Superscript, ["Off"], sb.Length));
                    return true;
                case 173:
                    sb.Append("9");
                    return true;
                case 174:
                    sb.Append("/");
                    return true;
                case 179:
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Yellow, null, sb.Length));
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Superscript, null, sb.Length));
                    sb.Append(Localization.Get("Critical"));
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.Superscript, ["Off"], sb.Length));
                    parser.ParsedTagList.Add(new FFIXTextTag(FFIXTextTagCode.White, null, sb.Length));
                    return true;
            }
            return false;
        }

        private static Boolean ApplyFormatTag(FFIXTextTag tag, Dialog dialog)
        {
            switch (tag.Code)
            {
                case FFIXTextTagCode.Table:
                case FFIXTextTagCode.NewPage:
                case FFIXTextTagCode.End:
                    // No need to keep these tags registered any longer
                    return true;
                case FFIXTextTagCode.Instantly:
                    if (dialog != null)
                        dialog.TypeEffect = false;
                    return true;
                case FFIXTextTagCode.Flash:
                    if (dialog != null)
                        dialog.TypeEffect = true;
                    return true;
                case FFIXTextTagCode.NoFocus:
                    if (dialog != null)
                    {
                        dialog.FlagButtonInh = true;
                        dialog.FlagResetChoice = false;
                    }
                    return true;
                case FFIXTextTagCode.NoAnimation:
                    if (dialog != null)
                        dialog.DialogAnimate.ShowWithoutAnimation = true;
                    return true;
                case FFIXTextTagCode.TailPosition:
                    OnTailPosition(dialog, FFIXTextTag.GetTailPosition(tag.StringParam(0)));
                    return true;
                case FFIXTextTagCode.LowerRight:
                    OnTailPosition(dialog, Dialog.TailPosition.LowerRight);
                    return true;
                case FFIXTextTagCode.LowerLeft:
                    OnTailPosition(dialog, Dialog.TailPosition.LowerLeft);
                    return true;
                case FFIXTextTagCode.UpperRight:
                    OnTailPosition(dialog, Dialog.TailPosition.UpperRight);
                    return true;
                case FFIXTextTagCode.UpperLeft:
                    OnTailPosition(dialog, Dialog.TailPosition.UpperLeft);
                    return true;
                case FFIXTextTagCode.LowerCenter:
                    OnTailPosition(dialog, Dialog.TailPosition.LowerCenter);
                    return true;
                case FFIXTextTagCode.UpperCenter:
                    OnTailPosition(dialog, Dialog.TailPosition.UpperCenter);
                    return true;
                case FFIXTextTagCode.LowerRightForce:
                    OnTailPosition(dialog, Dialog.TailPosition.LowerRightForce);
                    return true;
                case FFIXTextTagCode.LowerLeftForce:
                    OnTailPosition(dialog, Dialog.TailPosition.LowerLeftForce);
                    return true;
                case FFIXTextTagCode.UpperRightForce:
                    OnTailPosition(dialog, Dialog.TailPosition.UpperRightForce);
                    return true;
                case FFIXTextTagCode.UpperLeftForce:
                    OnTailPosition(dialog, Dialog.TailPosition.UpperLeftForce);
                    return true;
                case FFIXTextTagCode.DialogPosition:
                    OnTailPosition(dialog, Dialog.TailPosition.DialogPosition);
                    return true;
                case FFIXTextTagCode.DialogSize:
                    OnDialogSize(dialog, tag.SingleParam(0), tag.SingleParam(1), tag.IntParam(2) > 0);
                    return true;
                case FFIXTextTagCode.Position:
                    OnDialogPosition(dialog, tag);
                    return true;
                case FFIXTextTagCode.Offset:
                    if (dialog != null)
                        dialog.OffsetPosition = new Vector3(tag.SingleParam(0), tag.SingleParam(1), tag.SingleParam(2));
                    return true;
                case FFIXTextTagCode.Widths:
                    // Unused anymore (it was complicated and uses the PSX binary text tag format, only for variable width that is now automatically handled)
                    //OnWidths(dialog, ...);
                    return true;
                case FFIXTextTagCode.TagAnimation:
                    return true;
            }
            return false;
        }

        private static void SetupChoose(FFIXTextTag preTag, FFIXTextTag maskTag, Dialog dialog, Int32 choiceExactCount)
        {
            if (dialog != null)
            {
                if (preTag != null && maskTag != null)
                {
                    dialog.ChoiceNumber = preTag.IntParam(0);
                    dialog.ChoiceNumber = maskTag.IntParam(0);
                    dialog.CancelChoice = maskTag.IntParam(1);
                    dialog.DefaultChoice = ETb.sChoose < 0 ? 0 : ETb.sChoose;
                }
                else if (preTag != null)
                {
                    dialog.ChoiceNumber = preTag.IntParam(0);
                    dialog.DefaultChoice = ETb.sChoose < 0 ? 0 : ETb.sChoose;
                    dialog.DefaultChoice = dialog.DefaultChoice < dialog.ChoiceNumber ? dialog.DefaultChoice : dialog.ChoiceNumber - 1;
                    dialog.CancelChoice = preTag.IntParam(1);
                    if (dialog.CancelChoice < 0)
                        dialog.CancelChoice = dialog.ChoiceNumber - 1;
                }
                else if (maskTag != null)
                {
                    dialog.ChoiceNumber = maskTag.IntParam(0);
                    dialog.CancelChoice = maskTag.IntParam(1);
                    dialog.DefaultChoice = ETb.sChoose < 0 ? 0 : ETb.sChoose;
                }
                else
                {
                    dialog.ChoiceNumber = choiceExactCount;
                    dialog.CancelChoice = choiceExactCount - 1;
                    dialog.DefaultChoice = 0;
                }
                dialog.SetupChooseMask(ETb.sChooseMask, choiceExactCount);
                if (dialog.DisableIndexes.Count > 0)
                {
                    if (dialog.DisableIndexes.Contains(dialog.DefaultChoice) || !dialog.ActiveIndexes.Contains(dialog.DefaultChoice))
                        dialog.DefaultChoice = dialog.ActiveIndexes.Min();
                    if (dialog.DisableIndexes.Contains(dialog.CancelChoice) || !dialog.ActiveIndexes.Contains(dialog.CancelChoice))
                        dialog.CancelChoice = dialog.ActiveIndexes.Max();
                    if (dialog.LineNumberHint > dialog.ChoiceNumber)
                        dialog.LineNumberHint -= dialog.DisableIndexes.Count;
                }
                else
                {
                    dialog.DefaultChoice = Math.Min(dialog.DefaultChoice, choiceExactCount - 1);
                    dialog.CancelChoice = Math.Min(dialog.CancelChoice, choiceExactCount - 1);
                }
            }
        }

        private static void OnTab(FFIXTextModifier modifiers, Single dx, Single dy)
        {
            if (!modifiers.choice)
            {
                modifiers.extraOffset.x += (dx - 4f) * UIManager.ResourceXMultipier;
                modifiers.extraOffset.y += dy * UIManager.ResourceYMultipier;
            }
        }

        private static void OnFeed(FFIXTextModifier modifiers, Single feedParam)
        {
            if (!modifiers.choice)
                modifiers.extraOffset.x += feedParam * UIManager.ResourceXMultipier;
        }

        private static void OnTextColorRGBA(FFIXTextModifier modifiers, FFIXTextTag tag)
        {
            if (tag.ParamCount == 0)
            {
                if (modifiers.colors != null && modifiers.colors.size > 1)
                    modifiers.colors.RemoveAt(modifiers.colors.size - 1);
            }
            else if (tag.ParamCount == 1)
            {
                NGUIText.mAlpha = tag.SingleParam(0);
            }
            else if (tag.ParamCount == 3)
            {
                OnTextColor(modifiers, new Color(tag.SingleParam(0), tag.SingleParam(1), tag.SingleParam(2)));
            }
            else if (tag.ParamCount == 4)
            {
                if (modifiers.colors != null)
                {
                    Color textColor = new Color(tag.SingleParam(0), tag.SingleParam(1), tag.SingleParam(2), tag.SingleParam(3));
                    if (NGUIText.premultiply && textColor.a != 1f)
                        textColor = Color.Lerp(NGUIText.mInvisible, textColor, textColor.a);
                    modifiers.colors.Add(textColor);
                }
            }
        }

        private static void OnTextColor(FFIXTextModifier modifiers, Color textColor)
        {
            if (modifiers.colors != null)
            {
                if (modifiers.colors.size > 0)
                    textColor.a = modifiers.colors[modifiers.colors.size - 1].a;
                if (NGUIText.premultiply && textColor.a != 1f)
                    textColor = Color.Lerp(NGUIText.mInvisible, textColor, textColor.a);
                modifiers.colors.Add(textColor);
            }
        }

        private static void OnBackgroundColorRGBA(FFIXTextModifier modifiers, FFIXTextTag tag)
        {
            if ((tag.ParamCount == 0 || (tag.ParamCount == 1 && tag.StringParam(0) == "Off")) && modifiers.backgroundColors.Count > 0)
                modifiers.backgroundColors.RemoveAt(modifiers.backgroundColors.Count - 1);
            else if (tag.ParamCount == 1)
                modifiers.backgroundColors.Add(new FFIXTextModifier.Background(new Color(1f, 1f, 1f, tag.SingleParam(0) * NGUIText.tint.a)));
            else if (tag.ParamCount == 3)
                modifiers.backgroundColors.Add(new FFIXTextModifier.Background(new Color(tag.SingleParam(0), tag.SingleParam(1), tag.SingleParam(2), NGUIText.tint.a)));
            else if (tag.ParamCount == 4)
                modifiers.backgroundColors.Add(new FFIXTextModifier.Background(new Color(tag.SingleParam(0), tag.SingleParam(1), tag.SingleParam(2), tag.SingleParam(3) * NGUIText.tint.a)));
            else if (tag.ParamCount == 8)
                modifiers.backgroundColors.Add(new FFIXTextModifier.Background(new Color(tag.SingleParam(0), tag.SingleParam(1), tag.SingleParam(2), tag.SingleParam(3) * NGUIText.tint.a), new Rect(0f, 0f, 1f, 1f), tag.RectMinMaxParam(4)));
            else if (tag.ParamCount == 12)
                modifiers.backgroundColors.Add(new FFIXTextModifier.Background(new Color(tag.SingleParam(0), tag.SingleParam(1), tag.SingleParam(2), tag.SingleParam(3) * NGUIText.tint.a), tag.RectMinMaxParam(4), tag.RectMinMaxParam(8)));
        }

        private static void OnFontChange(FFIXTextModifier modifiers, FFIXTextTag tag)
        {
            Single fontScale = NGUIText.fontScale;
            String param = tag.StringParam(0);
            if (param == "RESET")
            {
                fontScale = 1f;
            }
            else
            {
                Char operation = param.Length > 0 ? param[0] : ' ';
                if (!Single.TryParse(param.Substring(Math.Min(1, param.Length)), out Single diff) || diff < 0f)
                    return;
                if (operation == '+')
                    fontScale += diff;
                else if (operation == '-')
                    fontScale = Math.Max(0.1f, fontScale - diff);
                else if (operation == '*')
                    fontScale = Math.Max(0.1f, fontScale * diff);
                else if (operation == '/' && diff > 0f)
                    fontScale = Math.Max(0.1f, fontScale / diff);
            }
            if (fontScale != NGUIText.fontScale)
            {
                Single oldHeight = NGUIText.GetGlyphHeight('A');
                NGUIText.fontScale = fontScale;
                NGUIText.UpdateFontSizes(false);
                Single newHeight = NGUIText.GetGlyphHeight('A');
                modifiers.extraOffset.y = oldHeight - newHeight;
            }
        }

        private static void OnTime(Dialog dialog, Int32 timeParam)
        {
            if (dialog != null)
            {
                if (timeParam > 0)
                {
                    dialog.EndMode = timeParam;
                    dialog.FlagButtonInh = true;
                }
                else if (timeParam == -1)
                {
                    dialog.FlagButtonInh = true;
                }
                else
                {
                    dialog.FlagButtonInh = false;
                }
            }
        }

        private static void OnSignal(Dialog dialog, Int32 signal)
        {
            if (signal < 0)
                ETb.gMesSignal++;
            else
                ETb.gMesSignal = signal;
        }

        private static void OnSound(FFIXTextTag tag)
        {
            // TODO: allow sound parameter to be given as an asset path
            Single volume = tag.ParamCount > 1 ? tag.SingleParam(1) : 1f;
            Single pitch = tag.ParamCount > 3 ? tag.SingleParam(3) : 1f;
            SoundLib.PlaySoundEffect(tag.IntParam(0), volume, tag.SingleParam(2), pitch);
        }

        private static void OnTailPosition(Dialog dialog, Dialog.TailPosition tailPos)
        {
            if (dialog != null)
                dialog.Tail = tailPos;
        }

        private static void OnDialogSize(Dialog dialog, Single width, Single lineNumber, Boolean forceSize)
        {
            if (dialog != null)
            {
                if (width > 0f)
                    width += 3f;
                dialog.WidthHint = width;
                dialog.LineNumberHint = lineNumber;
            }
        }

        private static void OnDialogPosition(Dialog dialog, FFIXTextTag tag)
        {
            // [DBG] Maybe shift position in widescreen (check Festival of the Hunt)
            if (dialog != null)
            {
                dialog.Position = new Vector2(tag.SingleParam(0), tag.SingleParam(1));
                if (tag.ParamCount > 2)
                    dialog.FollowDialog = tag.IntParam(2);
            }
        }

        private static void OnWidths(Dialog dialog, Int32[] tagParameters)
        {
            // Dummied
            if (dialog != null)
            {
                Int32 paramIndex = 0;
                while (paramIndex + 2 < tagParameters.Length)
                {
                    Int32 subParamIndex = 0;
                    Int32 lineIndex = tagParameters[paramIndex];
                    Int32 lineWidth = tagParameters[paramIndex + 1];
                    if (dialog.DisableIndexes.Contains(lineIndex - 1))
                        lineWidth = 0;
                    List<Int32> subParams = new List<Int32>();
                    Boolean hasSubParam = false;
                    while (tagParameters[paramIndex + 2 + subParamIndex] != -1)
                    {
                        subParams.Add(tagParameters[paramIndex + 2 + subParamIndex]);
                        hasSubParam = true;
                        subParamIndex++;
                    }
                    if (subParamIndex == 0)
                        subParamIndex = 1;
                    paramIndex += 2 + subParamIndex;
                    if (hasSubParam)
                        paramIndex++;
                    lineWidth += (Int32)NGUIText.GetDialogWidthFromSpecialOpcode(subParams, dialog.PhraseLabel);
                    dialog.WidthHint = Math.Max(dialog.WidthHint, lineWidth);
                }
            }
        }

        private static void SeekToNextLine(String text, ref Int32 offset)
        {
            Int32 lineIndex = text.IndexOf('\n', offset);
            offset = lineIndex >= 0 ? lineIndex : text.Length;
        }
    }
}
