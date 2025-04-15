using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Data;
using Memoria.Prime.Text;
using Memoria.Prime.Collections;

namespace Memoria.Assets
{
    public sealed class FFIXTextTag
    {
        public const Int32 MaxTagLength = 32;

        public readonly FFIXTextTagCode Code;
        public String[] Param;

        public Int32 TextOffset = 0;
        public Single AppearStep = 0f;

        public TextAnimatedTag LinkedAnimation = null;

        public Int32 ParamCount => Param?.Length ?? 0;

        public FFIXTextTag(FFIXTextTagCode code, String[] param = null, Int32 textOff = 0)
        {
            Code = code;
            Param = param;
            TextOffset = textOff;
        }

        public FFIXTextTag(FFIXTextTag copy)
        {
            Code = copy.Code;
            Param = copy.Param;
            TextOffset = copy.TextOffset;
            AppearStep = copy.AppearStep;
            LinkedAnimation = copy.LinkedAnimation;
        }

        public static List<FFIXTextTag> DeepListCopy(List<FFIXTextTag> from)
        {
            List<FFIXTextTag> copy = new List<FFIXTextTag>(from.Count);
            foreach (FFIXTextTag tag in from)
                copy.Add(new FFIXTextTag(tag));
            return copy;
        }

        public String StringParam(Int32 index)
        {
            if (Param != null && Param.Length > index)
                return Param[index];
            return String.Empty;
        }

        public Single SingleParam(Int32 index)
        {
            if (ParamCount > index && Single.TryParse(Param[index], out Single result))
                return result;
            return 0f;
        }

        public Int32 IntParam(Int32 index)
        {
            if (ParamCount > index && Single.TryParse(Param[index], out Single result))
                return (Int32)result;
            return 0;
        }

        public UInt32 UIntParam(Int32 index)
        {
            if (ParamCount > index && Single.TryParse(Param[index], out Single result))
                return (UInt32)result;
            return 0;
        }

        public Rect RectMinMaxParam(Int32 index)
        {
            Single xMin = SingleParam(index);
            Single yMin = SingleParam(index + 1);
            Single xMax = SingleParam(index + 2);
            Single yMax = SingleParam(index + 3);
            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        public static FFIXTextTag TryRead(String txt, ref Int32 offset)
        {
            if (txt[offset] == '{')
            {
                Int32 endOffset = txt.IndexOf('}', offset + 1);
                if (endOffset < 0)
                    return null;
                String fullTag = txt.Substring(offset + 1, endOffset - offset - 1);
                String[] parameters = null;
                if (fullTag.Length >= 2)
                {
                    FFIXTextTag tag = null;
                    if (fullTag[0] == 'W' && Char.IsNumber(fullTag[1]))
                    {
                        parameters = fullTag.Substring(1).Split('H');
                        if (parameters.Length == 2)
                            tag = new FFIXTextTag(FFIXTextTagCode.DialogSize, parameters);
                    }
                    if (fullTag[0] == 'y' && (fullTag[1] == '-' || Char.IsNumber(fullTag[1])))
                        tag = new FFIXTextTag(FFIXTextTagCode.DialogY, [fullTag.Substring(1)]);
                    else if (fullTag[0] == 'x' && (fullTag[1] == '-' || Char.IsNumber(fullTag[1])))
                        tag = new FFIXTextTag(FFIXTextTagCode.DialogX, [fullTag.Substring(1)]);
                    else if (fullTag[0] == 'f' && (fullTag[1] == '-' || Char.IsNumber(fullTag[1])))
                        tag = new FFIXTextTag(FFIXTextTagCode.DialogF, [fullTag.Substring(1)]);
                    if (tag != null)
                    {
                        offset = endOffset + 1;
                        return tag;
                    }
                }
                Int32 spaceIndex = fullTag.IndexOf(' ');
                if (spaceIndex >= 0)
                {
                    String pars = fullTag.Substring(spaceIndex + 1);
                    fullTag = fullTag.Substring(0, spaceIndex);
                    parameters = pars.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries);
                }
                FFIXTextTagCode code;
                if (fullTag[fullTag.Length - 1] == '+')
                    fullTag = fullTag.Substring(0, fullTag.Length - 1) + "Ex";
                if (!fullTag.TryEnumParse(out code))
                    return null;
                offset = endOffset + 1;
                return new FFIXTextTag(code, parameters);
            }
            else if (txt[offset] == '[')
            {
                Int32 endOffset = txt.IndexOf(']', offset + 1);
                if (endOffset < 0)
                    return null;
                String fullTag = txt.Substring(offset + 1, endOffset - offset - 1);
                Int32 equalIndex = fullTag.IndexOf('=');
                String[] parameters = null;
                if (equalIndex >= 0)
                {
                    String pars = fullTag.Substring(equalIndex + 1);
                    fullTag = fullTag.Substring(0, equalIndex);
                    parameters = pars.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries);
                }
                FFIXTextTagCode? code = ConvertOriginalTag(fullTag, ref parameters);
                if (!code.HasValue)
                    return null;
                offset = endOffset + 1;
                return new FFIXTextTag(code.Value, parameters);
            }
            return null;
        }

        public static FFIXTextTagCode? ConvertOriginalTag(String tagName, ref String[] tagParams)
        {
            if (CharacterNameTags.TryGetValue(tagName, out CharacterId charId))
            {
                tagParams = [((Int32)charId).ToString()];
                return FFIXTextTagCode.CharacterName;
            }
            else if (tagName.StartsWith("PTY") && tagName.Length >= 4 && Int32.TryParse(tagName.Substring(3), out Int32 partyIndex))
            {
                tagParams = [partyIndex.ToString()];
                return FFIXTextTagCode.Party;
            }
            else if (tagName.Length == 1 && tagParams == null && tagName[0] == '-')
            {
                tagParams = [];
                return FFIXTextTagCode.TextRGBA;
            }
            else if (tagName.Length == 2 && tagParams == null && NGUIText.IsHex(tagName[0]) && NGUIText.IsHex(tagName[1]))
            {
                tagParams = [NGUIText.ParseAlpha(tagName, 0).ToString()];
                return FFIXTextTagCode.TextRGBA;
            }
            else if (tagName.Length == 6 && tagParams == null)
            {
                Color textColor = NGUIText.ParseColor24(tagName, 0);
                if (NGUIText.EncodeColor24(textColor) == tagName.ToUpper())
                {
                    tagParams = [textColor.r.ToString(), textColor.g.ToString(), textColor.b.ToString()];
                    return FFIXTextTagCode.TextRGBA;
                }
            }
            else if (tagName.Length == 8 && tagParams == null)
            {
                Color textColor = NGUIText.ParseColor32(tagName, 0);
                if (NGUIText.EncodeColor32(textColor) == tagName.ToUpper())
                {
                    tagParams = [textColor.r.ToString(), textColor.g.ToString(), textColor.b.ToString(), textColor.a.ToString()];
                    return FFIXTextTagCode.TextRGBA;
                }
            }
            if (tagName.StartsWith("/"))
            {
                tagName = tagName.Substring(1);
                tagParams = ["Off"];
            }
            if (OriginalTagNames.TryGetValue(tagName, out FFIXTextTagCode tag))
            {
                if (tag == FFIXTextTagCode.DialogF && tagParams != null && tagParams.Length == 1 && Int32.TryParse(tagParams[0], out Int32 feedOffset) && feedOffset > SByte.MaxValue)
                    tagParams[0] = "0"; // Negative offsets [FEED=255] (mostly used in key item descriptions) are prevented. Note that many of the [FEED] offsets of key items are different from the PSX version
                return tag;
            }
            if (tagName == NGUIText.YSubOffset)
            {
                if (tagParams.Length > 0)
                    tagParams[0] = "-" + tagParams[0];
                return FFIXTextTagCode.DialogY;
            }
            if (tagName == "CENTER") // Used in Localization.txt (ControlPressStart, etc.) for some reason
                return FFIXTextTagCode.Center;
            return null;
        }

        public static void RegisterCustomNameKeywork(String keyword, CharacterId charId)
        {
            CharacterNameTags[keyword] = charId;
        }

        public static Dialog.TailPosition GetTailPosition(String param)
        {
            switch (param)
            {
                case "LOR": return Dialog.TailPosition.LowerRight;
                case "LOL": return Dialog.TailPosition.LowerLeft;
                case "UPR": return Dialog.TailPosition.UpperRight;
                case "UPL": return Dialog.TailPosition.UpperLeft;
                case "LOC": return Dialog.TailPosition.LowerCenter;
                case "UPC": return Dialog.TailPosition.UpperCenter;
                case "LORF": return Dialog.TailPosition.LowerRightForce;
                case "LOLF": return Dialog.TailPosition.LowerLeftForce;
                case "UPRF": return Dialog.TailPosition.UpperRightForce;
                case "UPLF": return Dialog.TailPosition.UpperLeftForce;
                case "DEFT": return Dialog.TailPosition.DialogPosition;
            }
            return Dialog.TailPosition.DialogPosition;
        }

        public override String ToString()
        {
            return ToString(true);
        }

        public String ToString(Boolean useMemoriaTags)
        {
            StringBuilder sb = new StringBuilder(MaxTagLength);
            if (useMemoriaTags)
            {
                switch (Code)
                {
                    case FFIXTextTagCode.DialogSize:
                        return $"{{W{Param[0]}H{Param[1]}}}";
                    case FFIXTextTagCode.DialogY:
                        return $"{{y{Param[0]}}}";
                    case FFIXTextTagCode.DialogX:
                        return $"{{x{Param[0]}}}";
                    case FFIXTextTagCode.DialogF:
                        return $"{{f{Param[0]}}}";
                }

                sb.Append('{');
                if (Enum.IsDefined(typeof(FFIXTextTagCode), Code))
                {
                    if (Code == FFIXTextTagCode.Fraya)
                        sb.Append(FFIXTextTagCode.Freya);
                    else
                        sb.Append(Code);
                }
                else
                {
                    sb.Append("Unknown ").Append(((Int32)Code).ToString("X2"));
                }
                if (ParamCount > 0)
                {
                    sb.Append(' ');
                    sb.Append(String.Join(",", Param));
                }
                sb.Append('}');
            }
            else
            {
                if (Code == FFIXTextTagCode.TextRGBA)
                {
                    if (ParamCount == 0)
                        return $"[-]";
                    if (ParamCount == 1)
                        return $"[{NGUIText.EncodeAlpha(SingleParam(0))}]";
                    if (ParamCount == 3)
                        return $"[{NGUIText.EncodeColor24(new Color(SingleParam(0), SingleParam(1), SingleParam(2)))}]";
                    if (ParamCount == 4)
                        return $"[{NGUIText.EncodeColor32(new Color(SingleParam(0), SingleParam(1), SingleParam(2), SingleParam(3)))}]";
                }
                else if (Code == FFIXTextTagCode.CharacterName)
                {
                    if (ParamCount == 1 && CharacterNameTags.TryGetKey((CharacterId)IntParam(0), out String charTagName))
                        return $"[{charTagName}]";
                }
                else if (Code == FFIXTextTagCode.Party)
                {
                    if (ParamCount == 1)
                        return $"[PTY{Param[0]}]";
                }

                Boolean isClosingTag = ParamCount == 1 && Param[0] == "Off";
                sb.Append('[');
                if (isClosingTag)
                    sb.Append('/');
                if (OriginalTagNames.TryGetKey(Code, out String tagName))
                    sb.Append(tagName);
                else
                    sb.Append("Unknown").Append(((Int32)Code).ToString("X2"));
                if (ParamCount > 0 && !isClosingTag)
                {
                    sb.Append('=');
                    sb.Append(String.Join(",", Param));
                }
                sb.Append(']');
            }
            return sb.ToString();
        }

        private static readonly Char[] CommaSeparator = [','];

        public static readonly TwoWayDictionary<String, FFIXTextTagCode> OriginalTagNames = new TwoWayDictionary<String, FFIXTextTagCode>()
        {
            { NGUIText.StartSentense, FFIXTextTagCode.DialogSize },
            { NGUIText.Choose, FFIXTextTagCode.Choice },
            { NGUIText.AnimationTime, FFIXTextTagCode.Time },
            { NGUIText.FlashInh, FFIXTextTagCode.Flash },
            { NGUIText.NoAnimation, FFIXTextTagCode.NoAnimation },
            { NGUIText.NoTypeEffect, FFIXTextTagCode.Instantly },
            { NGUIText.MessageSpeed, FFIXTextTagCode.Speed },
            { NGUIText.TagAnimation, FFIXTextTagCode.TagAnimation },
            { NGUIText.Shadow, FFIXTextTagCode.ShadowToggle },
            { NGUIText.NoShadow, FFIXTextTagCode.ShadowOff },
            { NGUIText.BackgroundColor, FFIXTextTagCode.BackgroundRGBA },
            { NGUIText.ChangeFont, FFIXTextTagCode.ChangeFont },
            { NGUIText.ButtonIcon, FFIXTextTagCode.DefaultButton },
            { NGUIText.CustomButtonIcon, FFIXTextTagCode.CustomButton },
            { NGUIText.NoFocus, FFIXTextTagCode.NoFocus },
            { NGUIText.IncreaseSignal, FFIXTextTagCode.IncreaseSignalEx },
            { NGUIText.NewIcon, FFIXTextTagCode.IconEx },
            { NGUIText.TextOffset, FFIXTextTagCode.MoveCaret },
            { NGUIText.EndSentence, FFIXTextTagCode.End },
            { NGUIText.TextVar, FFIXTextTagCode.Text },
            { NGUIText.ItemNameVar, FFIXTextTagCode.Item },
            { NGUIText.NumberVar, FFIXTextTagCode.Variable },
            { NGUIText.MessageDelay, FFIXTextTagCode.Wait },
            { NGUIText.MessageFeed, FFIXTextTagCode.DialogF },
            { NGUIText.MessageTab, FFIXTextTagCode.DialogX },
            { NGUIText.YAddOffset, FFIXTextTagCode.DialogY },
            { NGUIText.TextFrame, FFIXTextTagCode.TextFrame },
            { NGUIText.IconVar, FFIXTextTagCode.Icon },
            { NGUIText.PreChoose, FFIXTextTagCode.PreChoose },
            { NGUIText.PreChooseMask, FFIXTextTagCode.PreChooseMask },
            { NGUIText.DialogAbsPosition, FFIXTextTagCode.Position },
            { NGUIText.DialogOffsetPositon, FFIXTextTagCode.Offset },
            { NGUIText.DialogTailPositon, FFIXTextTagCode.TailPosition },
            { NGUIText.TableStart, FFIXTextTagCode.Table },
            { NGUIText.WidthInfo, FFIXTextTagCode.Widths },
            { NGUIText.Center, FFIXTextTagCode.Center },
            { NGUIText.Sound, FFIXTextTagCode.Sound },
            { NGUIText.Signal, FFIXTextTagCode.Signal },
            { NGUIText.NewPage, FFIXTextTagCode.NewPage },
            { NGUIText.MobileIcon, FFIXTextTagCode.Mobile },
            { NGUIText.SpacingY, FFIXTextTagCode.SpacingY },
            { NGUIText.KeyboardButtonIcon, FFIXTextTagCode.KeyboardButton },
            { NGUIText.JoyStickButtonIcon, FFIXTextTagCode.JoyStickButton },
            { NGUIText.NoTurboDialog, FFIXTextTagCode.TurboOff },
            { NGUIText.IconSprite, FFIXTextTagCode.Sprite },
            { NGUIText.Justified, FFIXTextTagCode.Justified },
            { NGUIText.Mirrored, FFIXTextTagCode.Mirrored },
            { NGUIText.Superscript, FFIXTextTagCode.Superscript },
            { NGUIText.Subscript, FFIXTextTagCode.Subscript },
            { NGUIText.Hyperlink, FFIXTextTagCode.Hyperlink },
            { NGUIText.Bold, FFIXTextTagCode.Bold },
            { NGUIText.Italic, FFIXTextTagCode.Italic },
            { NGUIText.Underline, FFIXTextTagCode.Underline },
            { NGUIText.Strikethrough, FFIXTextTagCode.Strikethrough },
            { NGUIText.IgnoreColor, FFIXTextTagCode.IgnoreColor }
        };

        public static readonly HashSet<FFIXTextTagCode> ConstantTextReplaceTags =
        [
            FFIXTextTagCode.Zidane,
            FFIXTextTagCode.Vivi,
            FFIXTextTagCode.Dagger,
            FFIXTextTagCode.Steiner,
            FFIXTextTagCode.Freya,
            FFIXTextTagCode.Fraya,
            FFIXTextTagCode.Quina,
            FFIXTextTagCode.Eiko,
            FFIXTextTagCode.Amarant,
            FFIXTextTagCode.CharacterName,
            FFIXTextTagCode.Party,
            FFIXTextTagCode.Text,
            FFIXTextTagCode.Icon
        ];

        public static readonly HashSet<FFIXTextTagCode> VariableTextReplaceTags =
        [
            FFIXTextTagCode.Variable,
            FFIXTextTagCode.Item,
        ];

        public static HashSet<FFIXTextTagCode> TabCodes =
        [
            FFIXTextTagCode.DialogX,
            FFIXTextTagCode.TextFrame
        ];

        public static readonly HashSet<FFIXTextTagCode> RemoveAtEndOfDeletionTags =
        [
            FFIXTextTagCode.Icon,
            FFIXTextTagCode.IconEx,
            FFIXTextTagCode.Signal,
            FFIXTextTagCode.IncreaseSignal,
            FFIXTextTagCode.IncreaseSignalEx,
            FFIXTextTagCode.Mobile,
            FFIXTextTagCode.Sound,
            FFIXTextTagCode.Sprite,
            FFIXTextTagCode.DefaultButton,
            FFIXTextTagCode.CustomButton,
            FFIXTextTagCode.KeyboardButton,
            FFIXTextTagCode.JoyStickButton,
            FFIXTextTagCode.Up,
            FFIXTextTagCode.Down,
            FFIXTextTagCode.Left,
            FFIXTextTagCode.Right,
            FFIXTextTagCode.Circle,
            FFIXTextTagCode.Cross,
            FFIXTextTagCode.Triangle,
            FFIXTextTagCode.Square,
            FFIXTextTagCode.R1,
            FFIXTextTagCode.R2,
            FFIXTextTagCode.L1,
            FFIXTextTagCode.L2,
            FFIXTextTagCode.Select,
            FFIXTextTagCode.Start,
            FFIXTextTagCode.Pad,
            FFIXTextTagCode.UpEx,
            FFIXTextTagCode.DownEx,
            FFIXTextTagCode.LeftEx,
            FFIXTextTagCode.RightEx,
            FFIXTextTagCode.CircleEx,
            FFIXTextTagCode.CrossEx,
            FFIXTextTagCode.TriangleEx,
            FFIXTextTagCode.SquareEx,
            FFIXTextTagCode.R1Ex,
            FFIXTextTagCode.R2Ex,
            FFIXTextTagCode.L1Ex,
            FFIXTextTagCode.L2Ex,
            FFIXTextTagCode.SelectEx,
            FFIXTextTagCode.StartEx,
            FFIXTextTagCode.PadEx
        ];

        public static readonly TwoWayDictionary<String, CharacterId> CharacterNameTags = new TwoWayDictionary<String, CharacterId>();
    }
}
