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
        public readonly String[] Param;

        public Int32 TextOffset = 0;
        public Single AppearStep = 0f;

        public FFIXTextTag(FFIXTextTagCode code, String[] param = null, Int32 textOff = 0)
        {
            Code = code;
            Param = param;
            TextOffset = textOff;
        }

        public String StringParam(Int32 index)
        {
            if (Param != null && Param.Length > index)
                return Param[index];
            return String.Empty;
        }

        public Single SingleParam(Int32 index)
        {
            if (Param != null && Param.Length > index && Single.TryParse(Param[index], out Single result))
                return result;
            return 0f;
        }

        public Int32 IntParam(Int32 index)
        {
            if (Param != null && Param.Length > index && Int32.TryParse(Param[index], out Int32 result))
                return result;
            return 0;
        }

        public UInt32 UIntParam(Int32 index)
        {
            if (Param != null && Param.Length > index && UInt32.TryParse(Param[index], out UInt32 result))
                return result;
            return 0;
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
                if (fullTag[0] == 'W' && Char.IsNumber(fullTag[1]))
                {
                    parameters = fullTag.Substring(1).Split('H');
                    if (parameters.Length != 2)
                        return null;
                    return new FFIXTextTag(FFIXTextTagCode.DialogSize, parameters);
                }
                if (fullTag[0] == 'y' && (fullTag[1] == '-' || Char.IsNumber(fullTag[1])))
                    return new FFIXTextTag(FFIXTextTagCode.DialogY, [fullTag.Substring(1)]);
                if (fullTag[0] == 'x' && (fullTag[1] == '-' || Char.IsNumber(fullTag[1])))
                    return new FFIXTextTag(FFIXTextTagCode.DialogX, [fullTag.Substring(1)]);
                if (fullTag[0] == 'f' && (fullTag[1] == '-' || Char.IsNumber(fullTag[1])))
                    return new FFIXTextTag(FFIXTextTagCode.DialogF, [fullTag.Substring(1)]);
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
                return tag;
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
                if (Param?.Length > 0)
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
                    if (Param.Length == 0)
                        return $"[-]";
                    if (Param.Length == 1)
                        return $"[{NGUIText.EncodeAlpha(SingleParam(0))}]";
                    if (Param.Length == 3)
                        return $"[{NGUIText.EncodeColor24(new Color(SingleParam(0), SingleParam(1), SingleParam(2)))}]";
                    if (Param.Length == 4)
                        return $"[{NGUIText.EncodeColor32(new Color(SingleParam(0), SingleParam(1), SingleParam(2), SingleParam(3)))}]";
                }
                else if (Code == FFIXTextTagCode.CharacterName)
                {
                    if (Param.Length == 1 && CharacterNameTags.TryGetKey((CharacterId)IntParam(0), out String charTagName))
                        return $"[{charTagName}]";
                }
                else if (Code == FFIXTextTagCode.Party)
                {
                    if (Param.Length == 1)
                        return $"[PTY{Param[0]}]";
                }

                Boolean isClosingTag = Param.Length == 1 && Param[0] == "Off";
                sb.Append('[');
                if (isClosingTag)
                    sb.Append('/');
                if (OriginalTagNames.TryGetKey(Code, out String tagName))
                    sb.Append(tagName);
                else
                    sb.Append("Unknown").Append(((Int32)Code).ToString("X2"));
                if (Param.Length > 0 && !isClosingTag)
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
            { NGUIText.Shadow, FFIXTextTagCode.ShadowToggle },
            { NGUIText.NoShadow, FFIXTextTagCode.ShadowOff },
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
            { NGUIText.IconVar, FFIXTextTagCode.Icon },
            { NGUIText.PreChoose, FFIXTextTagCode.PreChoose },
            { NGUIText.PreChooseMask, FFIXTextTagCode.PreChooseMask },
            { NGUIText.DialogAbsPosition, FFIXTextTagCode.Position },
            { NGUIText.DialogOffsetPositon, FFIXTextTagCode.Offset },
            { NGUIText.DialogTailPositon, FFIXTextTagCode.TailPosition },
            { NGUIText.TableStart, FFIXTextTagCode.Table },
            { NGUIText.WidthInfo, FFIXTextTagCode.Widths },
            { NGUIText.Center, FFIXTextTagCode.Center },
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

        public static readonly TwoWayDictionary<String, CharacterId> CharacterNameTags = new TwoWayDictionary<String, CharacterId>();
    }
}
