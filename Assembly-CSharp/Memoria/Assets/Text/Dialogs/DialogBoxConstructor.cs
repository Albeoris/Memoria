using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Memoria.Assets
{
    public sealed class DialogBoxConstructor
    {
        public static void PhrasePreOpcodeSymbol(String text, Dialog dlg)
        {
            DialogBoxConstructor constructor = new DialogBoxConstructor(dlg, text);
            constructor.Construct();
        }

        private readonly Dialog _dlg;
        private readonly StringBuilder _sb;
        private readonly Boolean _isJapanese;
        private readonly FF9StateGlobal _gameState;
        private String _text;
        private Char[] _chars;
        private Int32 _choiseIndex;

        private DialogBoxConstructor(Dialog dlg, String text)
        {
            _dlg = dlg;
            _text = text;
            _chars = text.ToCharArray();
            _sb = new StringBuilder(_chars.Length);
            _sb.Append(NGUIText.FF9WhiteColor); // Clear color
            _isJapanese = FF9StateSystem.Settings.CurrentLanguage == "Japanese";
            _gameState = FF9StateSystem.Common.FF9;
            _dlg.SignalNumber = ETb.gMesSignal;
        }

        private void Construct()
        {
            for (Int32 index = 0; index < _chars.Length; index++)
            {
                Int32 length = _chars.Length;
                Int32 left = length - index;

                Char ch = _chars[index];
                if (ch == '[')
                {
                    ParseOriginalTag(ref index, length);
                }
                else if (ch == '{')
                {
                    FFIXTextTag tag = FFIXTextTag.TryRead(_chars, ref index, ref left);
                    if (tag == null)
                    {
                        _sb.Append(ch);
                    }
                    else
                    {
                        index--;
                        try
                        {
                            PerformMemoriaTag(ref index, tag);
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            Log.Error(ex, "Not enought arguments: {0}", tag);
                            _sb.Append(tag);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Unexpected error: {0}", tag);
                            _sb.Append(tag);
                        }
                    }
                }
                else
                {
                    _sb.Append(ch);
                }
            }

            _dlg.SubPage.Add(_sb.ToString());
        }

        private void PerformMemoriaTag(ref Int32 index, FFIXTextTag tag)
        {
            switch (tag.Code)
            {
                case FFIXTextTagCode.NoAnimation:
                    _dlg.DialogAnimate.ShowWithoutAnimation = true;
                    break;
                case FFIXTextTagCode.NoFocus:
                    _dlg.FlagButtonInh = true;
                    _dlg.FlagResetChoice = false;
                    break;
                case FFIXTextTagCode.Flash:
                    _dlg.TypeEffect = true;
                    break;
                case FFIXTextTagCode.End:
                    _dlg.EndMode = -1;
                    break;
                case FFIXTextTagCode.DialogSize:
                    OnStartSentense(tag.Param);
                    break;
                case FFIXTextTagCode.LowerRight:
                    _dlg.Tail = Dialog.TailPosition.LowerRight;
                    break;
                case FFIXTextTagCode.LowerLeft:
                    _dlg.Tail = Dialog.TailPosition.LowerLeft;
                    break;
                case FFIXTextTagCode.UpperRight:
                    _dlg.Tail = Dialog.TailPosition.UpperRight;
                    break;
                case FFIXTextTagCode.UpperLeft:
                    _dlg.Tail = Dialog.TailPosition.UpperLeft;
                    break;
                case FFIXTextTagCode.LowerCenter:
                    _dlg.Tail = Dialog.TailPosition.LowerCenter;
                    break;
                case FFIXTextTagCode.UpperCenter:
                    _dlg.Tail = Dialog.TailPosition.UpperCenter;
                    break;
                case FFIXTextTagCode.LowerRightForce:
                    _dlg.Tail = Dialog.TailPosition.LowerRightForce;
                    break;
                case FFIXTextTagCode.LowerLeftForce:
                    _dlg.Tail = Dialog.TailPosition.LowerLeftForce;
                    break;
                case FFIXTextTagCode.UpperRightForce:
                    _dlg.Tail = Dialog.TailPosition.UpperRightForce;
                    break;
                case FFIXTextTagCode.UpperLeftForce:
                    _dlg.Tail = Dialog.TailPosition.UpperLeftForce;
                    break;
                case FFIXTextTagCode.DialogPosition:
                    _dlg.Tail = Dialog.TailPosition.DialogPosition;
                    break;
                case FFIXTextTagCode.Widths:
                    OnWidthsTag(tag.Param);
                    break;
                case FFIXTextTagCode.PreChoose:
                    OnPreChoose(tag.Param);
                    break;
                case FFIXTextTagCode.PreChooseMask:
                    OnPreChooseMask(tag.Param);
                    break;
                case FFIXTextTagCode.Time:
                    OnTime(tag.Param[0]);
                    break;
                case FFIXTextTagCode.Tab:
                    OnTab(ref index);
                    break;
                case FFIXTextTagCode.Icon:
                    OnIcon(tag.Param[0]);
                    break;
                case FFIXTextTagCode.IconEx:
                    KeepIconEx(tag.Param[0]);
                    break;
                case FFIXTextTagCode.Mobile:
                    KeepMobileIcon(_sb, tag.Param[0]);
                    break;
                case FFIXTextTagCode.Offset:
                    OnDialogOffsetPositon(tag.Param);
                    break;
                case FFIXTextTagCode.Zidane:
                    OnCharacterName(CharacterId.Zidane);
                    break;
                case FFIXTextTagCode.Vivi:
                    OnCharacterName(CharacterId.Vivi);
                    break;
                case FFIXTextTagCode.Dagger:
                    OnCharacterName(CharacterId.Garnet);
                    break;
                case FFIXTextTagCode.Steiner:
                    OnCharacterName(CharacterId.Steiner);
                    break;
                case FFIXTextTagCode.Freya:
                case FFIXTextTagCode.Fraya:
                    OnCharacterName(CharacterId.Freya);
                    break;
                case FFIXTextTagCode.Quina:
                    OnCharacterName(CharacterId.Quina);
                    break;
                case FFIXTextTagCode.Eiko:
                    OnCharacterName(CharacterId.Eiko);
                    break;
                case FFIXTextTagCode.Amarant:
                    OnCharacterName(CharacterId.Amarant);
                    break;
                case FFIXTextTagCode.Party:
                    OnPartyMemberName(tag.Param[0] - 1);
                    break;
                case FFIXTextTagCode.Variable:
                    _sb.Append(tag);
                    OnVariable(tag.Param[0]);
                    break;
                case FFIXTextTagCode.Item:
                    OnItemName(tag.Param[0]);
                    break;
                case FFIXTextTagCode.Signal:
                    _sb.Append(tag);
                    OnSignal(tag.Param[0]);
                    break;
                case FFIXTextTagCode.IncreaseSignal:
                    _sb.Append(tag);
                    OnIncreaseSignal();
                    OnTime(-1);
                    break;
                case FFIXTextTagCode.IncreaseSignalEx:
                    _sb.Append(tag);
                    OnIncreaseSignal();
                    break;
                case FFIXTextTagCode.Position:
                    _dlg.Position = new Vector2(tag.Param[0], tag.Param[1]);
                    break;
                case FFIXTextTagCode.Text:
                    OnTextVariable(tag.Param);
                    break;
                case FFIXTextTagCode.Choice:
                    _sb.Append(tag);
                    OnTab(ref index);
                    break;
                case FFIXTextTagCode.NewPage:
                    _sb.Append(tag);
                    OnNewPage();
                    break;
                default:
                {
                    StringBuilder sb;
                    if (NGUIText.ForceShowButton || !FF9StateSystem.MobilePlatform)
                        sb = _sb;
                    else
                        sb = new StringBuilder(16);

                    if (KeepKeyIcon(sb, tag.Code) || KeepKeyExIcon(sb, tag))
                        return;

                    _sb.Append(tag);
                    break;
                }
            }
        }

        private void ParseOriginalTag(ref Int32 index, Int32 length)
        {
            if (_chars[index + 5] == ']')
            {
                String textTagShort = new String(_chars, index + 1, 4);
                if (textTagShort == NGUIText.NoAnimation)
                {
                    _dlg.DialogAnimate.ShowWithoutAnimation = true;
                    index += 5;
                    return;
                }
                if (textTagShort == NGUIText.NoFocus)
                {
                    _dlg.FlagButtonInh = true;
                    _dlg.FlagResetChoice = false;
                    index += 5;
                    return;
                }
                if (textTagShort == NGUIText.FlashInh)
                {
                    _dlg.TypeEffect = true;
                    index += 5;
                    return;
                }
                if (textTagShort == NGUIText.EndSentence)
                {
                    _dlg.EndMode = -1;
                    index += 5;
                    return;
                }
                if (textTagShort == NGUIText.NoTurboDialog)
                {
                    UIKeyTrigger.preventTurboKey = true;  // Disable turbo dialog manually. (for Trance Seek purpose)
                    index += 5;
                    return;
                }
            }

            Int32 nextIndex = index;
            String textTag = new String(_chars, index + 1, 4);
            if (textTag == NGUIText.StartSentense)
            {
                OnStartSentense(GetAllParametersFromTag(_chars, index, ref nextIndex));
            }
            else if (textTag == NGUIText.DialogTailPositon)
            {
                nextIndex = Array.IndexOf(_chars, ']', index + 4);
                switch (new String(_chars, index + 6, nextIndex - index - 6))
                {
                    case "LOR":
                        _dlg.Tail = Dialog.TailPosition.LowerRight;
                        break;
                    case "LOL":
                        _dlg.Tail = Dialog.TailPosition.LowerLeft;
                        break;
                    case "UPR":
                        _dlg.Tail = Dialog.TailPosition.UpperRight;
                        break;
                    case "UPL":
                        _dlg.Tail = Dialog.TailPosition.UpperLeft;
                        break;
                    case "LOC":
                        _dlg.Tail = Dialog.TailPosition.LowerCenter;
                        break;
                    case "UPC":
                        _dlg.Tail = Dialog.TailPosition.UpperCenter;
                        break;
                    case "LORF":
                        _dlg.Tail = Dialog.TailPosition.LowerRightForce;
                        break;
                    case "LOLF":
                        _dlg.Tail = Dialog.TailPosition.LowerLeftForce;
                        break;
                    case "UPRF":
                        _dlg.Tail = Dialog.TailPosition.UpperRightForce;
                        break;
                    case "UPLF":
                        _dlg.Tail = Dialog.TailPosition.UpperLeftForce;
                        break;
                    case "DEFT":
                        _dlg.Tail = Dialog.TailPosition.DialogPosition;
                        break;
                }
            }
            else if (textTag == NGUIText.WidthInfo)
            {
                OnWidthsTag(GetAllParametersFromTag(_chars, index, ref nextIndex));
            }
            else if (textTag == NGUIText.PreChoose)
            {
                OnPreChoose(GetAllParametersFromTag(_chars, index, ref nextIndex));
            }
            else if (textTag == NGUIText.PreChooseMask)
            {
                OnPreChooseMask(GetAllParametersFromTag(_chars, index, ref nextIndex));
            }
            else if (textTag == NGUIText.AnimationTime)
            {
                OnTime(NGUIText.GetOneParameterFromTag(_chars, index, ref nextIndex));
            }
            else if (textTag == NGUIText.TextOffset)
            {
                Int32[] tagParameters = GetAllParametersFromTag(_chars, index, ref nextIndex);
                if (_dlg.SkipThisChoice(_choiseIndex))
                {
                    nextIndex = _text.IndexOf('[' + NGUIText.TextOffset, nextIndex, StringComparison.Ordinal);
                    if (nextIndex >= 0)
                        nextIndex--;
                }
                else if (tagParameters[0] == 18 && tagParameters[1] == 0)
                {
                    _sb.Append("    ");
                }
                else
                {
                    _sb.Append($"[{textTag}={tagParameters[0]},{tagParameters[1]}]");
                }
                _choiseIndex++;
            }
            else if (
                textTag == NGUIText.CustomButtonIcon ||
                textTag == NGUIText.ButtonIcon ||
                textTag == NGUIText.JoyStickButtonIcon ||
                textTag == NGUIText.KeyboardButtonIcon)
            {
                UIKeyTrigger.preventTurboKey = true;  // Disable turbo dialog when a special box appear (Gysahl Greens shop from Chocobo Forest, Eiko when she's cooking, ...)
                nextIndex = Array.IndexOf(_chars, ']', index + 4);
                String iconName = new String(_chars, index + 6, nextIndex - index - 6);
                if (!FF9StateSystem.MobilePlatform ||
                    textTag == NGUIText.JoyStickButtonIcon ||
                    textTag == NGUIText.KeyboardButtonIcon ||
                    NGUIText.ForceShowButton)
                {
                    _sb.Append('[');
                    _sb.Append(textTag);
                    _sb.Append('=');
                    _sb.Append(iconName);
                    _sb.Append("] ");
                }
            }
            else if (textTag == NGUIText.IconVar)
            {
                OnIcon(NGUIText.GetOneParameterFromTag(_chars, index, ref nextIndex));
            }
            else if (textTag == NGUIText.NewIcon)
            {
                KeepIconEx(NGUIText.GetOneParameterFromTag(_chars, index, ref nextIndex));
            }
            else if (textTag == NGUIText.MobileIcon)
            {
                KeepMobileIcon(_sb, NGUIText.GetOneParameterFromTag(_chars, index, ref nextIndex));
            }
            else if (textTag == NGUIText.DialogOffsetPositon)
            {
                OnDialogOffsetPositon(GetAllParametersFromTag(_chars, index, ref nextIndex));
            }
            else if (NGUIText.nameKeywordList.Contains(textTag))
            {
                nextIndex = Array.IndexOf(_chars, ']', index + 4);
                if (textTag == NGUIText.Zidane)
                    OnCharacterName(CharacterId.Zidane);
                else if (textTag == NGUIText.Vivi)
                    OnCharacterName(CharacterId.Vivi);
                else if (textTag == NGUIText.Dagger)
                    OnCharacterName(CharacterId.Garnet);
                else if (textTag == NGUIText.Steiner)
                    OnCharacterName(CharacterId.Steiner);
                else if (textTag == NGUIText.Freya)
                    OnCharacterName(CharacterId.Freya);
                else if (textTag == NGUIText.Quina)
                    OnCharacterName(CharacterId.Quina);
                else if (textTag == NGUIText.Eiko)
                    OnCharacterName(CharacterId.Eiko);
                else if (textTag == NGUIText.Amarant)
                    OnCharacterName(CharacterId.Amarant);
                else if (textTag == NGUIText.Party1)
                    OnPartyMemberName(0);
                else if (textTag == NGUIText.Party2)
                    OnPartyMemberName(1);
                else if (textTag == NGUIText.Party3)
                    OnPartyMemberName(2);
                else if (textTag == NGUIText.Party4)
                    OnPartyMemberName(3);
                else
                {
                    foreach (KeyValuePair<String, CharacterId> kv in NGUIText.nameCustomKeywords)
                    {
                        if (textTag == kv.Key)
                        {
                            OnCharacterName(kv.Value);
                            break;
                        }
                    }
                }
            }
            else if (textTag == NGUIText.NumberVar)
            {
                Int32 bracketEnd = 0;
                OnVariable(NGUIText.GetOneParameterFromTag(_chars, index, ref bracketEnd));
            }
            else if (textTag == NGUIText.ItemNameVar)
            {
                OnItemName(NGUIText.GetOneParameterFromTag(_chars, index, ref nextIndex));
            }
            else if (textTag == NGUIText.Signal)
            {
                Int32 bracketEnd = Array.IndexOf(_chars, ']', index + 4);
                String numStr = new String(_chars, index + 6, bracketEnd - index - 6);
                if (Int32.TryParse(numStr, out Int32 signalNumber))
                    OnSignal(signalNumber);
            }
            else if (textTag == NGUIText.IncreaseSignal)
            {
                OnIncreaseSignal();
            }
            else if (textTag == NGUIText.DialogAbsPosition)
            {
                Single[] tagParameters = NGUIText.GetAllParameters(_chars, index, ref nextIndex);
                _dlg.Position = new Vector2(tagParameters[0], tagParameters[1]);
            }
            else if (textTag == NGUIText.TextVar)
            {
                OnTextVariable(GetAllParametersFromTag(_chars, index, ref nextIndex));
            }
            else if (textTag == NGUIText.Choose)
            {
                UIKeyTrigger.preventTurboKey = true; // Disable turbo dialog when a choice appearc
                Int32 startIndex = Array.IndexOf(_chars, ']', index + 4);
                OnChoice(startIndex);
            }
            else if (textTag == NGUIText.NewPage)
            {
                OnNewPage();
            }
            if (nextIndex == index)
                _sb.Append(_chars[index]);
            else if (nextIndex != -1)
                index = nextIndex;
            else
                index = length;
        }

        private void OnNewPage()
        {
            _dlg.SubPage.Add(_sb.Evict());
            _sb.Append(NGUIText.FF9WhiteColor);
        }

        private void OnChoice(Int32 startIndex)
        {
            if (_isJapanese)
            {
                Int32[] disabled = _dlg.DisableIndexes != null && _dlg.DisableIndexes.Count > 0 ? _dlg.DisableIndexes.ToArray() : [-1];
                _text = ProcessJapaneseChoose(_text, startIndex, disabled);
                _chars = _text.ToArray();
            }
        }

        private void OnTextVariable(Int32[] tagParameters)
        {
            _sb.Append(ETb.GetStringFromTable(Convert.ToUInt32(tagParameters[0]), Convert.ToUInt32(tagParameters[1])));
        }

        private void OnIncreaseSignal()
        {
            _dlg.SignalNumber++;
            _dlg.SignalMode = 2;
        }

        private void OnSignal(Int32 signalNumber)
        {
            _dlg.SignalNumber = signalNumber;
            _dlg.SignalMode = 1;
        }

        private void OnItemName(Int32 tagParameter)
        {
            _sb.Append("[C8B040][HSHD]");
            _sb.Append(ETb.GetItemName(ETb.gMesValue[tagParameter]));
            _sb.Append("[C8C8C8][HSHD]");
        }

        private void OnVariable(Int32 tagParameter)
        {
            Int32 value = ETb.gMesValue[tagParameter];
            if (!_dlg.MessageValues.ContainsKey(tagParameter))
                _dlg.MessageValues.Add(tagParameter, value);
            _dlg.MessageNeedUpdate = true;
        }

        private void OnStartSentense(Int32[] args)
        {
            Single width = args[0];
            Int32 lineNumber = args[1];
            if (width > 0f)
                width += 3f;
            if (width > _dlg.CaptionWidth)
                _dlg.Width = width;
            else
                _dlg.Width = _dlg.CaptionWidth;
            _dlg.LineNumber = lineNumber;
        }

        private void OnPartyMemberName(Int32 index)
        {
            CharacterId partyPlayer = PersistenSingleton<EventEngine>.Instance.GetEventPartyPlayer(index);
            PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(partyPlayer);
            _sb.Append(player?.Name);
        }

        private void OnCharacterName(CharacterId charId)
        {
            _sb.Append(FF9StateSystem.Common.FF9.GetPlayer(charId).Name);
        }

        private void OnDialogOffsetPositon(Int32[] tagParameters)
        {
            _dlg.OffsetPosition = new Vector3(tagParameters[0], tagParameters[1], tagParameters[2]);
        }

        internal static Boolean KeepKeyIcon(StringBuilder sb, FFIXTextTagCode tagCode)
        {
            switch (tagCode)
            {
                case FFIXTextTagCode.Up:
                    sb.Append("[DBTN=UP] ");
                    break;
                case FFIXTextTagCode.Down:
                    sb.Append("[DBTN=DOWN] ");
                    break;
                case FFIXTextTagCode.Left:
                    sb.Append("[DBTN=LEFT] ");
                    break;
                case FFIXTextTagCode.Right:
                    sb.Append("[DBTN=RIGHT] ");
                    break;
                case FFIXTextTagCode.Circle:
                    sb.Append("[DBTN=CIRCLE] ");
                    break;
                case FFIXTextTagCode.Cross:
                    sb.Append("[DBTN=CROSS] ");
                    break;
                case FFIXTextTagCode.Triangle:
                    sb.Append("[DBTN=TRIANGLE] ");
                    break;
                case FFIXTextTagCode.Square:
                    sb.Append("[DBTN=SQUARE] ");
                    break;
                case FFIXTextTagCode.R1:
                    sb.Append("[DBTN=R1] ");
                    break;
                case FFIXTextTagCode.R2:
                    sb.Append("[DBTN=R2] ");
                    break;
                case FFIXTextTagCode.L1:
                    sb.Append("[DBTN=L1] ");
                    break;
                case FFIXTextTagCode.L2:
                    sb.Append("[DBTN=L2] ");
                    break;
                case FFIXTextTagCode.Select:
                    sb.Append("[DBTN=SELECT] ");
                    break;
                case FFIXTextTagCode.Start:
                    sb.Append("[DBTN=START] ");
                    break;
                case FFIXTextTagCode.Pad:
                    sb.Append("[DBTN=PAD] ");
                    break;
                default:
                    return false;
            }
            return true;
        }

        internal static Boolean KeepKeyExIcon(StringBuilder sb, FFIXTextTag tag)
        {
            switch (tag.Code)
            {
                case FFIXTextTagCode.UpEx:
                    sb.Append("[CBTN=UP] ");
                    break;
                case FFIXTextTagCode.DownEx:
                    sb.Append("[CBTN=DOWN] ");
                    break;
                case FFIXTextTagCode.LeftEx:
                    sb.Append("[CBTN=LEFT] ");
                    break;
                case FFIXTextTagCode.RightEx:
                    sb.Append("[CBTN=RIGHT] ");
                    break;
                case FFIXTextTagCode.CircleEx:
                    sb.Append("[CBTN=CIRCLE] ");
                    break;
                case FFIXTextTagCode.CrossEx:
                    sb.Append("[CBTN=CROSS] ");
                    break;
                case FFIXTextTagCode.TriangleEx:
                    sb.Append("[CBTN=TRIANGLE] ");
                    break;
                case FFIXTextTagCode.SquareEx:
                    sb.Append("[CBTN=SQUARE] ");
                    break;
                case FFIXTextTagCode.R1Ex:
                    sb.Append("[CBTN=R1] ");
                    break;
                case FFIXTextTagCode.R2Ex:
                    sb.Append("[CBTN=R2] ");
                    break;
                case FFIXTextTagCode.L1Ex:
                    sb.Append("[CBTN=L1] ");
                    break;
                case FFIXTextTagCode.L2Ex:
                    sb.Append("[CBTN=L2] ");
                    break;
                case FFIXTextTagCode.SelectEx:
                    sb.Append("[CBTN=SELECT] ");
                    break;
                case FFIXTextTagCode.StartEx:
                    sb.Append("[CBTN=START] ");
                    break;
                case FFIXTextTagCode.PadEx:
                    sb.Append("[CBTN=PAD] ");
                    break;
                default:
                    return false;
            }
            return true;
        }

        internal static void KeepMobileIcon(StringBuilder sb, Int32 tagParameter)
        {
            if (FF9StateSystem.MobilePlatform && !NGUIText.ForceShowButton)
            {
                sb.Append("[MOBI=");
                sb.Append(tagParameter);
                sb.Append("] ");
            }
        }

        private void KeepIconEx(Int32 tagParameter)
        {
            if ((ETb.gMesValue[0] & 1 << tagParameter) > 0)
            {
                _sb.Append("[PNEW=");
                _sb.Append(tagParameter);
                _sb.Append("] ");
            }
        }

        private void OnIcon(Int32 iconNumber)
        {
            switch (iconNumber)
            {
                case 34:
                    _sb.Append("[sub]0[/sub]");
                    return;
                case 35:
                    _sb.Append("[sub]1[/sub]");
                    return;
                case 39:
                    _sb.Append("[sub]5[/sub]");
                    return;
                case 45:
                    _sb.Append("[sub]/[/sub]");
                    break;
                case 159:
                    _sb.Append("[sup]");
                    _sb.Append(Localization.Get("Miss"));
                    _sb.Append("[/sup]");
                    return;
                case 160:
                    _sb.Append("[sup]");
                    _sb.Append(Localization.Get("Death"));
                    _sb.Append("[/sup]");
                    return;
                case 161:
                    _sb.Append("[sup]");
                    _sb.Append(Localization.Get("Guard"));
                    _sb.Append("[/sup]");
                    return;
                case 163:
                    _sb.Append("[sup]");
                    _sb.Append(Localization.Get("MPCaption"));
                    _sb.Append("[/sup]");
                    break;
                case 173:
                    _sb.Append("9");
                    break;
                case 174:
                    _sb.Append("/");
                    break;
                case 179:
                    _sb.Append(NGUIText.FF9YellowColor);
                    _sb.Append("[sup]");
                    _sb.Append(Localization.Get("Critical"));
                    _sb.Append("[/sup]");
                    _sb.Append(NGUIText.FF9WhiteColor);
                    break;
                default:
                    _sb.Append("[ICON=");
                    _sb.Append(iconNumber);
                    _sb.Append("] ");
                    break;
            }
        }

        private void OnTime(Int32 tagParameter)
        {
            if (tagParameter > 0)
            {
                _dlg.EndMode = tagParameter;
                _dlg.FlagButtonInh = true;
            }
            else if (tagParameter == -1)
            {
                _dlg.FlagButtonInh = true;
            }
            else
            {
                _dlg.FlagButtonInh = false;
            }
        }

        private void OnTab(ref Int32 index)
        {
            if (_dlg.SkipThisChoice(_choiseIndex))
            {
                Int32 newIndex = _text.IndexOf("{Tab", index, StringComparison.Ordinal);
                if (newIndex >= 0)
                    index = newIndex - 1;
            }
            else
            {
                _sb.Append("    ");
            }
            _choiseIndex++;
        }

        private void OnPreChooseMask(Int32[] tagParameters)
        {
            ETb.sChooseMask = ETb.sChooseMaskInit;
            _dlg.ChoiceNumber = Convert.ToInt32(tagParameters[0]);
            _dlg.CancelChoice = Convert.ToInt32(tagParameters[1]);
            _dlg.DefaultChoice = (ETb.sChoose < 0) ? 0 : ETb.sChoose;
            _dlg.LineNumber = ((_dlg.LineNumber >= (Single)_dlg.ChoiceNumber) ? _dlg.LineNumber : (_dlg.LineNumber + _dlg.ChoiceNumber));
            _dlg.ChooseMask = ETb.sChooseMask;
            if (_dlg.DisableIndexes.Count > 0)
            {
                if (_dlg.DisableIndexes.Contains(_dlg.DefaultChoice) || !_dlg.ActiveIndexes.Contains(_dlg.DefaultChoice))
                    _dlg.DefaultChoice = _dlg.ActiveIndexes.Min();
                if (_dlg.DisableIndexes.Contains(_dlg.CancelChoice) || !_dlg.ActiveIndexes.Contains(_dlg.CancelChoice))
                    _dlg.CancelChoice = _dlg.ActiveIndexes.Max();
            }
            else
            {
                _dlg.DefaultChoice = _dlg.DefaultChoice < _dlg.ChoiceNumber ? _dlg.DefaultChoice : _dlg.ChoiceNumber - 1;
            }
            _choiseIndex = 0;
        }

        private void OnPreChoose(Int32[] tagParameters)
        {
            ETb.sChooseMask = -1;
            _dlg.ChoiceNumber = tagParameters[0];
            _dlg.DefaultChoice = ETb.sChoose < 0 ? 0 : ETb.sChoose;
            _dlg.DefaultChoice = _dlg.DefaultChoice < _dlg.ChoiceNumber ? _dlg.DefaultChoice : _dlg.ChoiceNumber - 1;
            _dlg.CancelChoice = tagParameters[1] < 0 ? _dlg.ChoiceNumber - 1 : tagParameters[1];
        }

        private void OnWidthsTag(Int32[] tagParameters)
        {
            Int32 paramIndex = 0;
            while (paramIndex + 2 < tagParameters.Length)
            {
                Int32 subParamIndex = 0;
                Int32 lineIndex = tagParameters[paramIndex];
                Int32 lineWidth = tagParameters[paramIndex + 1];
                if (_dlg.DisableIndexes.Contains(lineIndex - 1))
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
                lineWidth += (Int32)NGUIText.GetDialogWidthFromSpecialOpcode(subParams, _dlg.PhraseLabel);
                if (_dlg.OriginalWidth < lineWidth)
                    _dlg.Width = lineWidth;
            }
        }

        public static Int32[] GetAllParametersFromTag(Char[] fullText, Int32 currentIndex, ref Int32 closingBracket)
        {
            closingBracket = Array.IndexOf(fullText, ']', currentIndex + 4);
            return Array.ConvertAll(new String(fullText, currentIndex + 6, closingBracket - currentIndex - 6).Split(','), Int32.Parse);
        }

        // Not supported for Memoria Tags
        public static String ProcessJapaneseChoose(String text, Int32 startIndex, Int32[] disableChoice)
        {
            Int32 endSentenceIndex = text.IndexOf('[' + NGUIText.EndSentence, startIndex + 1, StringComparison.Ordinal);
            Int32 sentenceLength = endSentenceIndex - startIndex;

            if (sentenceLength <= 0)
            {
                Int32 timeIndex = text.IndexOf("[TIME=-1]", startIndex + 1, StringComparison.Ordinal);
                sentenceLength = timeIndex - startIndex;
            }

            String sentence = text.Substring(startIndex + 1, sentenceLength);
            String[] lines = sentence.Split('\n');

            StringBuilder sb = new StringBuilder(sentence.Length);

            for (Int32 i = 0; i < lines.Length; i++)
            {
                String line = lines[i];

                Boolean replacePadding = true;

                for (Int32 j = 0; j < disableChoice.Length; j++)
                {
                    if (i == disableChoice[j])
                    {
                        replacePadding = false;
                        break;
                    }
                }

                if (replacePadding)
                {
                    sb.Append(line.Replace("  ", "    "));
                    if (i + 1 < lines.Length)
                        sb.Append('\n');
                }
            }

            return text.Replace(sentence, sb.ToString());
        }
    }
}
