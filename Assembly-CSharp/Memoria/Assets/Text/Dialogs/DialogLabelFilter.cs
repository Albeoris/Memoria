using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Memoria.Assets
{
    public sealed class DialogLabelFilter
    {
        public static String PhrasePreOpcodeSymbol(UILabel label, Char[] text, ref Single additionalWidth)
        {
            DialogLabelFilter filter = new DialogLabelFilter(label, text, additionalWidth);
            try
            {
                filter.PhrasePreOpcodeSymbol();
                return filter._sb.ToString();
            }
            finally
            {
                additionalWidth = filter._additionalWidth;
            }
        }

        private readonly Char[] _chars;
        private readonly StringBuilder _sb;
        private readonly UILabel _label;
        private Single _additionalWidth;
        private Single _currentWidth;

        public DialogLabelFilter(UILabel label, Char[] chars, Single additionalWidth)
        {
            _label = label;
            _chars = chars;
            _additionalWidth = additionalWidth;

            _sb = new StringBuilder(chars.Length);
        }

        public void PhrasePreOpcodeSymbol()
        {
            for (Int32 index = 0; index < _chars.Length; index++)
            {
                Char ch = _chars[index];
                if (ch == '\n')
                {
                    if (_currentWidth > _additionalWidth)
                        _additionalWidth = _currentWidth;

                    _currentWidth = 0f;
                    _sb.Append(ch);
                    continue;
                }

                Int32 left = _chars.Length - index;
                FFIXTextTag tag = FFIXTextTag.TryRead(_chars, ref index, ref left);
                if (tag != null)
                {
                    index--;
                    ProcessMemoriaTag(tag, ref index);
                }
                else if (ch == '[' && (index + 6 <= _chars.Length))
                {
                    ProcessOriginalTag(ref index);
                }
                else
                {
                    _sb.Append(ch);
                }
            }
        }

        private void ProcessMemoriaTag(FFIXTextTag tag, ref Int32 index)
        {
            switch (tag.Code)
            {
                case FFIXTextTagCode.Icon:
                    OnIcon(tag.Param[0]);
                    break;
                case FFIXTextTagCode.Mobile:
                    OnMobileIcon(tag.Param[0]);
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
                    OnVariable(tag.Param[0]);
                    break;
                case FFIXTextTagCode.DialogX:
                    OnMessageX(tag.Param[0]);
                    break;
                case FFIXTextTagCode.DialogF:
                    OnMessageFeed(tag.Param[0]);
                    break;
                default:
                    StringBuilder sb;
                    if (NGUIText.ForceShowButton || !FF9StateSystem.MobilePlatform)
                        sb = _sb;
                    else
                        sb = new StringBuilder(16);

                    if (DialogBoxConstructor.KeepKeyIcon(sb, tag.Code) || DialogBoxConstructor.KeepKeyExIcon(sb, tag))
                        return;

                    _sb.Append(tag);
                    break;
            }
        }

        private void ProcessOriginalTag(ref Int32 index)
        {
            Int32 num2 = index;
            String text3 = new String(_chars, index, 5);
            if (text3 == "[" + NGUIText.IconVar)
            {
                Int32 oneParameterFromTag = NGUIText.GetOneParameterFromTag(_chars, index, ref num2);
                OnIcon(oneParameterFromTag);
            }
            else if (text3 == "[" + NGUIText.CustomButtonIcon || text3 == "[" + NGUIText.ButtonIcon || text3 == "[" + NGUIText.JoyStickButtonIcon || text3 == "[" + NGUIText.KeyboardButtonIcon)
            {
                num2 = Array.IndexOf(_chars, ']', index + 4);
                String text5 = new String(_chars, index + 6, num2 - index - 6);
                if (!FF9StateSystem.MobilePlatform || NGUIText.ForceShowButton)
                {
                    _sb.Append(text3);
                    _sb.Append("=");
                    _sb.Append(text5);
                    _sb.Append("] ");
                }
            }
            else if (text3 == "[" + NGUIText.MobileIcon)
            {
                Int32 oneParameterFromTag2 = NGUIText.GetOneParameterFromTag(_chars, index, ref num2);
                OnMobileIcon(oneParameterFromTag2);
            }
            else if (NGUIText.nameKeywordList.Contains(text3.Remove(0, 1)))
            {
                String a = text3.Remove(0, 1);
                num2 = Array.IndexOf(_chars, ']', index + 4);
                if (a == NGUIText.Zidane)
                    OnCharacterName(CharacterId.Zidane);
                else if (a == NGUIText.Vivi)
                    OnCharacterName(CharacterId.Vivi);
                else if (a == NGUIText.Dagger)
                    OnCharacterName(CharacterId.Garnet);
                else if (a == NGUIText.Steiner)
                    OnCharacterName(CharacterId.Steiner);
                else if (a == NGUIText.Freya)
                    OnCharacterName(CharacterId.Freya);
                else if (a == NGUIText.Quina)
                    OnCharacterName(CharacterId.Quina);
                else if (a == NGUIText.Eiko)
                    OnCharacterName(CharacterId.Eiko);
                else if (a == NGUIText.Amarant)
                    OnCharacterName(CharacterId.Amarant);
                else if (a == NGUIText.Party1)
                    OnPartyMemberName(0);
                else if (a == NGUIText.Party2)
                    OnPartyMemberName(1);
                else if (a == NGUIText.Party3)
                    OnPartyMemberName(2);
                else if (a == NGUIText.Party4)
                    OnPartyMemberName(3);
                else
                {
                    foreach (KeyValuePair<String, CharacterId> kv in NGUIText.nameCustomKeywords)
                    {
                        if (a == kv.Key)
                        {
                            OnCharacterName(kv.Value);
                            break;
                        }
                    }
                }
            }
            else if (text3 == "[" + NGUIText.NumberVar)
            {
                Int32 num4 = 0;
                Int32 oneParameterFromTag3 = NGUIText.GetOneParameterFromTag(_chars, index, ref num4);
                num2 = Array.IndexOf(_chars, ']', index + 4);
                OnVariable(oneParameterFromTag3);
            }
            else if (text3 == "[" + NGUIText.MessageTab)
            {
                Int32 oneParameterFromTag4 = NGUIText.GetOneParameterFromTag(_chars, index, ref num2);
                OnMessageX(oneParameterFromTag4);
            }
            else if (text3 == "[" + NGUIText.MessageFeed)
            {
                Int32 oneParameterFromTag5 = NGUIText.GetOneParameterFromTag(_chars, index, ref num2);
                OnMessageFeed(oneParameterFromTag5);
            }
            else if (text3 == "[" + NGUIText.SpacingY)
            {
                _label.spacingY = NGUIText.GetOneParameterFromTag(_chars, index, ref num2);
            }
            if (num2 == index)
            {
                _sb.Append(_chars[index]);
            }
            else if (num2 != -1)
            {
                index = num2;
            }
            else
            {
                index = _chars.Length;
            }
        }

        private void OnMessageFeed(Int32 oneParameterFromTag5)
        {
            _currentWidth += oneParameterFromTag5 * 3;
            _sb.Append("[FEED=");
            _sb.Append(oneParameterFromTag5);
            _sb.Append(']');
        }

        private void OnMessageX(Int32 oneParameterFromTag4)
        {
            _currentWidth += oneParameterFromTag4 * 3;
            _sb.Append("[XTAB=");
            _sb.Append(oneParameterFromTag4);
            _sb.Append(']');
        }

        private void OnVariable(Int32 oneParameterFromTag3)
        {
            Int32 num5 = PersistenSingleton<EventEngine>.Instance.eTb.gMesValue[oneParameterFromTag3];
            _sb.Append(num5.ToString());
            _currentWidth += NGUIText.GetTextWidthFromFF9Font(_label, num5.ToString());
        }

        private void OnCharacterName(CharacterId charId)
        {
            _sb.Append(FF9StateSystem.Common.FF9.GetPlayer(charId).Name);
        }

        private void OnPartyMemberName(Int32 index)
        {
            CharacterId partyPlayer = PersistenSingleton<EventEngine>.Instance.GetEventPartyPlayer(index);
            PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(partyPlayer);
            if (player != null)
            {
                _sb.Append(player.Name);
                _currentWidth += NGUIText.GetTextWidthFromFF9Font(_label, player.Name);
            }
        }

        private void OnMobileIcon(Int32 oneParameterFromTag2)
        {
            DialogBoxConstructor.KeepMobileIcon(_sb, oneParameterFromTag2);
            _currentWidth += FF9UIDataTool.GetIconSize(oneParameterFromTag2).x;
        }

        private void OnIcon(Int32 oneParameterFromTag)
        {
            String parsed;
            Boolean isIcon = true;
            switch (oneParameterFromTag)
            {
                case 34:
                    parsed = "[sub]0[/sub]";
                    break;
                case 35:
                    parsed = "[sub]1[/sub]";
                    break;
                case 39:
                    parsed = "[sub]5[/sub]";
                    break;
                case 45:
                    parsed = "[sub]/[/sub]";
                    break;
                case 159:
                    parsed = "[sup]" + Localization.Get("Miss") + "[/sup]";
                    break;
                case 160:
                    parsed = "[sup]" + Localization.Get("Death") + "[/sup]";
                    break;
                case 161:
                    parsed = "[sup]" + Localization.Get("Guard") + "[/sup]";
                    break;
                case 162:
                    parsed = "[B880E0][sup]" + Localization.Get("Critical") + "[/sup][C8C8C8]";
                    break;
                case 163:
                    parsed = "[sup]MP[/sup]";
                    break;
                case 173:
                    parsed = "9";
                    break;
                case 174:
                    parsed = "/";
                    break;
                default:
                    parsed = String.Concat("[ICON", "=", oneParameterFromTag, "] ");
                    isIcon = false;
                    break;
            }
            _sb.Append(parsed);
            if (isIcon)
                _currentWidth += NGUIText.GetTextWidthFromFF9Font(_label, FF9TextTool.RemoveOpCode(parsed));
            else
                _currentWidth += FF9UIDataTool.GetIconSize(oneParameterFromTag).x;
        }
    }
}
