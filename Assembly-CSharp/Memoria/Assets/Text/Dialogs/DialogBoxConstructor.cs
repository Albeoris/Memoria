using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Exceptions;
using Memoria.Prime.Text;
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
        private readonly ETb _textEngine;
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
            _textEngine = PersistenSingleton<EventEngine>.Instance?.eTb;

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
                String a = new String(_chars, index, 6);
                if (a == "[" + NGUIText.NoAnimation + "]")
                {
                    _dlg.DialogAnimate.ShowWithoutAnimation = true;
                    index += 5;
                    return;
                }
                if (a == "[" + NGUIText.NoFocus + "]")
                {
                    _dlg.FlagButtonInh = true;
                    _dlg.FlagResetChoice = false;
                    index += 5;
                    return;
                }
                if (a == "[" + NGUIText.FlashInh + "]")
                {
                    _dlg.TypeEffect = true;
                    index += 5;
                    return;
                }
                if (a == "[" + NGUIText.EndSentence + "]")
                {
                    _dlg.EndMode = -1;
                    index += 5;
                    return;
                }
                if (a == "[" + NGUIText.NoTurboDialog + "]")
                {
                    UIKeyTrigger.preventTurboKey = true;  // Disable turbo dialog manually. (for Trance Seek purpose)
                    index += 5;
                    return;
                }
            }

            Int32 newIndex = index;
            String text3 = new String(_chars, index, 5);
            if (text3 == "[" + NGUIText.StartSentense)
            {
                Int32[] allParametersFromTag = GetAllParametersFromTag(_chars, index, ref newIndex);
                OnStartSentense(allParametersFromTag);
            }
            else if (text3 == "[" + NGUIText.DialogTailPositon)
            {
                newIndex = Array.IndexOf(_chars, ']', index + 4);
                String text4 = new String(_chars, index + 6, newIndex - index - 6);
                String text5 = text4;
                switch (text5)
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
            else if (text3 == "[" + NGUIText.WidthInfo)
            {
                Int32[] allParametersFromTag2 = GetAllParametersFromTag(_chars, index, ref newIndex);
                OnWidthsTag(allParametersFromTag2);
            }
            else if (text3 == "[" + NGUIText.PreChoose)
            {
                Int32[] allParametersFromTag3 = GetAllParametersFromTag(_chars, index, ref newIndex);
                OnPreChoose(allParametersFromTag3);
            }
            else if (text3 == "[" + NGUIText.PreChooseMask)
            {
                Int32[] allParametersFromTag4 = GetAllParametersFromTag(_chars, index, ref newIndex);
                OnPreChooseMask(allParametersFromTag4);
            }
            else if (text3 == "[" + NGUIText.AnimationTime)
            {
                Int32 oneParameterFromTag = NGUIText.GetOneParameterFromTag(_chars, index, ref newIndex);
                OnTime(oneParameterFromTag);
            }
            else if (text3 == "[" + NGUIText.TextOffset)
            {
                Int32[] allParametersFromTag5 = GetAllParametersFromTag(_chars, index, ref newIndex);
                if (_dlg.SkipThisChoice(_choiseIndex))
                {
                    newIndex = _text.IndexOf('[' + NGUIText.TextOffset, newIndex, StringComparison.Ordinal);
                    if (newIndex >= 0)
                    {
                        newIndex--;
                    }
                }
                else if (allParametersFromTag5[0] == 18 && allParametersFromTag5[1] == 0)
                {
                    _sb.Append("    ");
                }
                else
                {
                    _sb.Append(text3);
                    _sb.Append('=');
                    _sb.Append(allParametersFromTag5[0]);
                    _sb.Append(',');
                    _sb.Append(allParametersFromTag5[1]);
                    _sb.Append(']');
                }
                _choiseIndex++;
            }
            else if (
                text3 == "[" + NGUIText.CustomButtonIcon ||
                text3 == "[" + NGUIText.ButtonIcon ||
                text3 == "[" + NGUIText.JoyStickButtonIcon ||
                text3 == "[" + NGUIText.KeyboardButtonIcon)
            {
                UIKeyTrigger.preventTurboKey = true;  // Disable turbo dialog when a special box appear (Gysahl Greens shop from Chocobo Forest, Eiko when she's cooking, ...)
                newIndex = Array.IndexOf(_chars, ']', index + 4);
                String text6 = new String(_chars, index + 6, newIndex - index - 6);
                if (!FF9StateSystem.MobilePlatform ||
                    text3 == "[" + NGUIText.JoyStickButtonIcon ||
                    text3 == "[" + NGUIText.KeyboardButtonIcon ||
                    NGUIText.ForceShowButton)
                {
                    _sb.Append(text3);
                    _sb.Append('=');
                    _sb.Append(text6);
                    _sb.Append("] ");
                }
            }
            else if (text3 == "[" + NGUIText.IconVar)
            {
                Int32 oneParameterFromTag2 = NGUIText.GetOneParameterFromTag(_chars, index, ref newIndex);
                OnIcon(oneParameterFromTag2);
            }
            else if (text3 == "[" + NGUIText.NewIcon)
            {
                Int32 oneParameterFromTag3 = NGUIText.GetOneParameterFromTag(_chars, index, ref newIndex);
                KeepIconEx(oneParameterFromTag3);
            }
            else if (text3 == "[" + NGUIText.MobileIcon)
            {
                Int32 oneParameterFromTag4 = NGUIText.GetOneParameterFromTag(_chars, index, ref newIndex);
                KeepMobileIcon(_sb, oneParameterFromTag4);
            }
            else if (text3 == "[" + NGUIText.DialogOffsetPositon)
            {
                Int32[] allParametersFromTag6 = GetAllParametersFromTag(_chars, index, ref newIndex);
                OnDialogOffsetPositon(allParametersFromTag6);
            }
            else if (NGUIText.nameKeywordList.Contains(text3.Remove(0, 1)))
            {
                String a2 = text3.Remove(0, 1);
                newIndex = Array.IndexOf(_chars, ']', index + 4);
                if (a2 == NGUIText.Zidane)
                    OnCharacterName(CharacterId.Zidane);
                else if (a2 == NGUIText.Vivi)
                    OnCharacterName(CharacterId.Vivi);
                else if (a2 == NGUIText.Dagger)
                    OnCharacterName(CharacterId.Garnet);
                else if (a2 == NGUIText.Steiner)
                    OnCharacterName(CharacterId.Steiner);
                else if (a2 == NGUIText.Freya)
                    OnCharacterName(CharacterId.Freya);
                else if (a2 == NGUIText.Quina)
                    OnCharacterName(CharacterId.Quina);
                else if (a2 == NGUIText.Eiko)
                    OnCharacterName(CharacterId.Eiko);
                else if (a2 == NGUIText.Amarant)
                    OnCharacterName(CharacterId.Amarant);
                else if (a2 == NGUIText.Party1)
                    OnPartyMemberName(0);
                else if (a2 == NGUIText.Party2)
                    OnPartyMemberName(1);
                else if (a2 == NGUIText.Party3)
                    OnPartyMemberName(2);
                else if (a2 == NGUIText.Party4)
                    OnPartyMemberName(3);
                else
                {
                    foreach (KeyValuePair<String, CharacterId> kv in NGUIText.nameCustomKeywords)
                    {
                        if (a2 == kv.Key)
                        {
                            OnCharacterName(kv.Value);
                            break;
                        }
                    }
                }
            }
            else if (text3 == "[" + NGUIText.NumberVar)
            {
                Int32 num10 = 0;
                Int32 oneParameterFromTag5 = NGUIText.GetOneParameterFromTag(_chars, index, ref num10);
                OnVariable(oneParameterFromTag5);
            }
            else if (text3 == "[" + NGUIText.ItemNameVar)
            {
                Int32 oneParameterFromTag6 = NGUIText.GetOneParameterFromTag(_chars, index, ref newIndex);
                OnItemName(oneParameterFromTag6);
            }
            else if (text3 == "[" + NGUIText.Signal)
            {
                Int32 num11 = Array.IndexOf(_chars, ']', index + 4);
                String value2 = new String(_chars, index + 6, num11 - index - 6);
                Int32 signalNumber = Convert.ToInt32(value2);
                OnSignal(signalNumber);
            }
            else if (text3 == "[" + NGUIText.IncreaseSignal)
            {
                OnIncreaseSignal();
            }
            else if (text3 == "[" + NGUIText.DialogAbsPosition)
            {
                Single[] allParameters = NGUIText.GetAllParameters(_chars, index, ref newIndex);
                _dlg.Position = new Vector2(allParameters[0], allParameters[1]);
            }
            else if (text3 == "[" + NGUIText.TextVar)
            {
                Int32[] allParametersFromTag7 = GetAllParametersFromTag(_chars, index, ref newIndex);
                OnTextVariable(allParametersFromTag7);
            }
            else if (text3 == "[" + NGUIText.Choose)
            {
                UIKeyTrigger.preventTurboKey = true; // Disable turbo dialog when a choice appear
                Int32 startIndex = Array.IndexOf(_chars, ']', index + 4);
                OnChoice(startIndex);
            }
            else if (text3 == "[" + NGUIText.NewPage)
            {
                OnNewPage();
            }
            if (newIndex == index)
            {
                _sb.Append(_chars[index]);
            }
            else if (newIndex != -1)
            {
                index = newIndex;
            }
            else
            {
                index = length;
            }
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
                Int32[] array2 = { -1 };
                if (_dlg.DisableIndexes != null)
                {
                    array2 = (_dlg.DisableIndexes.Count <= 0) ? array2 : _dlg.DisableIndexes.ToArray();
                }

                _text = ProcessJapaneseChoose(_text, startIndex, array2);
                _chars = _text.ToArray();
            }
        }

        private void OnTextVariable(Int32[] allParametersFromTag7)
        {
            _sb.Append(_textEngine.GetStringFromTable(Convert.ToUInt32(allParametersFromTag7[0]), Convert.ToUInt32(allParametersFromTag7[1])));
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

        private void OnItemName(Int32 oneParameterFromTag6)
        {
            _sb.Append("[C8B040][HSHD]");
            _sb.Append(ETb.GetItemName(_textEngine.gMesValue[oneParameterFromTag6]));
            _sb.Append("[C8C8C8]");
        }

        private void OnVariable(Int32 oneParameterFromTag5)
        {
            Int32 value = _textEngine.gMesValue[oneParameterFromTag5];
            if (!_dlg.MessageValues.ContainsKey(oneParameterFromTag5))
            {
                _dlg.MessageValues.Add(oneParameterFromTag5, value);
            }
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

        private void OnDialogOffsetPositon(Int32[] allParametersFromTag6)
        {
            _dlg.OffsetPosition = new Vector3(allParametersFromTag6[0], allParametersFromTag6[1], allParametersFromTag6[2]);
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

        internal static void KeepMobileIcon(StringBuilder sb, Int32 oneParameterFromTag4)
        {
            if (FF9StateSystem.MobilePlatform && !NGUIText.ForceShowButton)
            {
                sb.Append("[MOBI=");
                sb.Append(oneParameterFromTag4);
                sb.Append("] ");
            }
        }

        private void KeepIconEx(Int32 oneParameterFromTag3)
        {
            if ((_textEngine.gMesValue[0] & 1 << oneParameterFromTag3) > 0)
            {
                _sb.Append("[PNEW=");
                _sb.Append(oneParameterFromTag3);
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

        private void OnTime(Int32 oneParameterFromTag)
        {
            if (oneParameterFromTag > 0)
            {
                _dlg.EndMode = oneParameterFromTag;
                _dlg.FlagButtonInh = true;
            }
            else if (oneParameterFromTag == -1)
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

        private void OnPreChooseMask(Int32[] allParametersFromTag4)
        {
            ETb.sChooseMask = ETb.sChooseMaskInit;
            _dlg.ChoiceNumber = Convert.ToInt32(allParametersFromTag4[0]);
            _dlg.CancelChoice = Convert.ToInt32(allParametersFromTag4[1]);
            _dlg.DefaultChoice = (ETb.sChoose < 0) ? 0 : ETb.sChoose;
            _dlg.LineNumber = ((_dlg.LineNumber >= (Single)_dlg.ChoiceNumber) ? _dlg.LineNumber : (_dlg.LineNumber + _dlg.ChoiceNumber));
            _dlg.ChooseMask = ETb.sChooseMask;
            if (_dlg.DisableIndexes.Count > 0)
            {
                if (_dlg.DisableIndexes.Contains(_dlg.DefaultChoice) || !_dlg.ActiveIndexes.Contains(_dlg.DefaultChoice))
                {
                    _dlg.DefaultChoice = _dlg.ActiveIndexes.Min();
                }
                if (_dlg.DisableIndexes.Contains(_dlg.CancelChoice) || !_dlg.ActiveIndexes.Contains(_dlg.CancelChoice))
                {
                    _dlg.CancelChoice = _dlg.ActiveIndexes.Max();
                }
            }
            else
            {
                _dlg.DefaultChoice = (_dlg.DefaultChoice < _dlg.ChoiceNumber) ? _dlg.DefaultChoice : (_dlg.ChoiceNumber - 1);
            }
            _choiseIndex = 0;
        }

        private void OnPreChoose(Int32[] allParametersFromTag3)
        {
            ETb.sChooseMask = -1;
            _dlg.ChoiceNumber = Convert.ToInt32(allParametersFromTag3[0]);
            _dlg.DefaultChoice = (ETb.sChoose < 0) ? 0 : ETb.sChoose;
            _dlg.DefaultChoice = (_dlg.DefaultChoice < _dlg.ChoiceNumber) ? _dlg.DefaultChoice : (_dlg.ChoiceNumber - 1);
            Int32 num9 = Convert.ToInt32(allParametersFromTag3[1]);
            _dlg.CancelChoice = (num9 <= -1) ? (_dlg.ChoiceNumber - 1) : num9;
        }

        private void OnWidthsTag(Int32[] allParametersFromTag2)
        {
            Int32 num5 = 0;
            while (num5 + 2 < allParametersFromTag2.Length)
            {
                Int32 num6 = 0;
                Int32 num7 = allParametersFromTag2[num5];
                Int32 num8 = allParametersFromTag2[num5 + 1];
                if (_dlg.DisableIndexes.Contains(num7 - 1))
                {
                    num8 = 0;
                }
                List<Int32> list = new List<Int32>();
                Boolean flag2 = false;
                while (allParametersFromTag2[num5 + 2 + num6] != -1)
                {
                    list.Add(allParametersFromTag2[num5 + 2 + num6]);
                    flag2 = true;
                    num6++;
                }
                if (num6 == 0)
                {
                    num6 = 1;
                }
                num5 += 2 + num6;
                if (flag2)
                {
                    num5++;
                }

                num8 += (Int32)NGUIText.GetDialogWidthFromSpecialOpcode(list, _textEngine, _dlg.PhraseLabel);
                if (_dlg.OriginalWidth < num8)
                {
                    _dlg.Width = num8;
                }
            }
        }

        public static Int32[] GetAllParametersFromTag(Char[] fullText, Int32 currentIndex, ref Int32 closingBracket)
        {
            closingBracket = Array.IndexOf(fullText, ']', currentIndex + 4);
            String text = new String(fullText, currentIndex + 6, closingBracket - currentIndex - 6);
            String[] array = text.Split(',');
            return Array.ConvertAll(array, Int32.Parse);
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

                for (Int32 k = 0; k < disableChoice.Length; k++)
                {
                    if (i == disableChoice[k])
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