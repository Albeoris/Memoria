using Assets.Sources.Scripts.UI.Common;
using Memoria.Prime;
using System;

namespace Memoria.Assets
{
	public class DialogBoxController
	{
		public static void PhraseOpcodeSymbol(String text, Dialog dlg)
		{
			DialogBoxController constructor = new DialogBoxController(dlg, text);
			constructor.Control();
		}

		private readonly Dialog _dlg;
		private readonly Char[] _chars;
		private Int32 _linesNumber;

		private DialogBoxController(Dialog dlg, String text)
		{
			_dlg = dlg;
			_chars = text.ToCharArray();
		}

		public void Control()
		{
			for (Int32 index = 0; index < _chars.Length; index++)
			{
				Char ch = _chars[index];
				if (ch == '\n')
					_linesNumber++;

				if (index + 6 <= _chars.Length && ch == '[')
				{
					PerformOriginalTag(ref index);
				}
				else
				{
					Int32 currentIndex = index;
					Int32 left = _chars.Length - index;
					FFIXTextTag tag = FFIXTextTag.TryRead(_chars, ref index, ref left);
					if (tag == null)
						continue;

					index--;
					try
					{
						PerformMemoriaTag(currentIndex, tag);
					}
					catch (IndexOutOfRangeException ex)
					{
						Log.Error(ex, "Not enought arguments: {0}", tag);
					}
					catch (Exception ex)
					{
						Log.Error(ex, "Unexpected error: {0}", tag);
					}
				}
			}
		}

		private void PerformMemoriaTag(Int32 characterIndex, FFIXTextTag tag)
		{
			switch (tag.Code)
			{
				case FFIXTextTagCode.Speed:
					OnMessageSpeed(characterIndex, tag.Param[0]);
					break;
				case FFIXTextTagCode.Wait:
					OnMessageDelay(characterIndex, tag.Param[0]);
					break;
				case FFIXTextTagCode.Instantly:
					OnNoTypeEffect();
					break;
				case FFIXTextTagCode.Choice:
					OnChoise();
					break;
				case FFIXTextTagCode.Up:
					OnButton(characterIndex, false, "DBTN", "UP");
					return;
				case FFIXTextTagCode.Down:
					OnButton(characterIndex, false, "DBTN", "DOWN");
					return;
				case FFIXTextTagCode.Left:
					OnButton(characterIndex, false, "DBTN", "LEFT");
					return;
				case FFIXTextTagCode.Right:
					OnButton(characterIndex, false, "DBTN", "RIGHT");
					return;
				case FFIXTextTagCode.Circle:
					OnButton(characterIndex, false, "DBTN", "CIRCLE");
					return;
				case FFIXTextTagCode.Cross:
					OnButton(characterIndex, false, "DBTN", "CROSS");
					return;
				case FFIXTextTagCode.Triangle:
					OnButton(characterIndex, false, "DBTN", "TRIANGLE");
					return;
				case FFIXTextTagCode.Square:
					OnButton(characterIndex, false, "DBTN", "SQUARE");
					return;
				case FFIXTextTagCode.R1:
					OnButton(characterIndex, false, "DBTN", "R1");
					return;
				case FFIXTextTagCode.R2:
					OnButton(characterIndex, false, "DBTN", "R2");
					return;
				case FFIXTextTagCode.L1:
					OnButton(characterIndex, false, "DBTN", "L1");
					return;
				case FFIXTextTagCode.L2:
					OnButton(characterIndex, false, "DBTN", "L2");
					return;
				case FFIXTextTagCode.Select:
					OnButton(characterIndex, false, "DBTN", "SELECT");
					return;
				case FFIXTextTagCode.Start:
					OnButton(characterIndex, false, "DBTN", "START");
					return;
				case FFIXTextTagCode.Pad:
					OnButton(characterIndex, false, "DBTN", "PAD");
					return;
				case FFIXTextTagCode.UpEx:
					OnButton(characterIndex, true, "CBTN", "UP");
					break;
				case FFIXTextTagCode.DownEx:
					OnButton(characterIndex, true, "CBTN", "DOWN");
					break;
				case FFIXTextTagCode.LeftEx:
					OnButton(characterIndex, true, "CBTN", "LEFT");
					break;
				case FFIXTextTagCode.RightEx:
					OnButton(characterIndex, true, "CBTN", "RIGHT");
					break;
				case FFIXTextTagCode.CircleEx:
					OnButton(characterIndex, true, "CBTN", "CIRCLE");
					break;
				case FFIXTextTagCode.CrossEx:
					OnButton(characterIndex, true, "CBTN", "CROSS");
					break;
				case FFIXTextTagCode.TriangleEx:
					OnButton(characterIndex, true, "CBTN", "TRIANGLE");
					break;
				case FFIXTextTagCode.SquareEx:
					OnButton(characterIndex, true, "CBTN", "SQUARE");
					break;
				case FFIXTextTagCode.R1Ex:
					OnButton(characterIndex, true, "CBTN", "R1");
					break;
				case FFIXTextTagCode.R2Ex:
					OnButton(characterIndex, true, "CBTN", "R2");
					break;
				case FFIXTextTagCode.L1Ex:
					OnButton(characterIndex, true, "CBTN", "L1");
					break;
				case FFIXTextTagCode.L2Ex:
					OnButton(characterIndex, true, "CBTN", "L2");
					break;
				case FFIXTextTagCode.SelectEx:
					OnButton(characterIndex, true, "CBTN", "SELECT");
					break;
				case FFIXTextTagCode.StartEx:
					OnButton(characterIndex, true, "CBTN", "START");
					break;
				case FFIXTextTagCode.PadEx:
					OnButton(characterIndex, true, "CBTN", "PAD");
					break;
				case FFIXTextTagCode.Icon:
					OnIcon(characterIndex, tag.Param[0]);
					break;
				case FFIXTextTagCode.IconEx:
					OnNewIcon(characterIndex);
					break;
				case FFIXTextTagCode.Mobile:
					OnMobileIcon(characterIndex, tag.Param[0]);
					break;
			}
		}

		private void PerformOriginalTag(ref Int32 characterIndex)
		{
			Int32 num2 = characterIndex;
			String a = new String(_chars, characterIndex, 5);
			if (a == "[" + NGUIText.MessageSpeed)
			{
				Int32 oneParameterFromTag = NGUIText.GetOneParameterFromTag(_chars, characterIndex, ref num2);
				OnMessageSpeed(characterIndex, oneParameterFromTag);
			}
			if (a == "[" + NGUIText.MessageDelay)
			{
				Int32 oneParameterFromTag2 = NGUIText.GetOneParameterFromTag(_chars, characterIndex, ref num2);
				OnMessageDelay(characterIndex, oneParameterFromTag2);
			}
			else if (a == "[" + NGUIText.NoTypeEffect)
			{
				num2 = Array.IndexOf(_chars, ']', characterIndex + 4);
				OnNoTypeEffect();
			}
			else if (a == "[" + NGUIText.Choose)
			{
				num2 = Array.IndexOf(_chars, ']', characterIndex + 4);
				OnChoise();
			}
			else if (a == "[" + NGUIText.CustomButtonIcon || a == "[" + NGUIText.ButtonIcon || a == "[" + NGUIText.JoyStickButtonIcon || a == "[" + NGUIText.KeyboardButtonIcon)
			{
				num2 = Array.IndexOf(_chars, ']', characterIndex + 4);
				String parameterStr = new String(_chars, characterIndex + 6, num2 - characterIndex - 6);
				Boolean checkConfig = a == "[" + NGUIText.CustomButtonIcon || a == "[" + NGUIText.JoyStickButtonIcon;
				String tag = new String(_chars, characterIndex + 1, 4);
				OnButton(characterIndex, checkConfig, tag, parameterStr);
			}
			else if (a == "[" + NGUIText.IconVar)
			{
				Int32 oneParameterFromTag3 = NGUIText.GetOneParameterFromTag(_chars, characterIndex, ref num2);
				OnIcon(characterIndex, oneParameterFromTag3);
			}
			else if (a == "[" + NGUIText.NewIcon)
			{
				Int32 oneParameterFromTag4 = NGUIText.GetOneParameterFromTag(_chars, characterIndex, ref num2);
				OnNewIcon(characterIndex);
			}
			else if (a == "[" + NGUIText.MobileIcon)
			{
				Int32 oneParameterFromTag5 = NGUIText.GetOneParameterFromTag(_chars, characterIndex, ref num2);
				OnMobileIcon(characterIndex, oneParameterFromTag5);
			}
			if (num2 == characterIndex)
			{
				if (num2 != -1)
				{
					characterIndex = num2;
				}
				else
				{
					characterIndex = _chars.Length;
				}
			}
		}

		private void OnMobileIcon(Int32 characterIndex, Int32 oneParameterFromTag5)
		{
			Dialog.DialogImage dialogImage4 = NGUIText.CreateIconImage(oneParameterFromTag5);
			dialogImage4.TextPosition = characterIndex;
			_dlg.ImageList.Add(dialogImage4);
		}

		private void OnNewIcon(Int32 characterIndex)
		{
			Dialog.DialogImage dialogImage3 = NGUIText.CreateIconImage(FF9UIDataTool.NewIconId);
			dialogImage3.TextPosition = characterIndex;
			_dlg.ImageList.Add(dialogImage3);
		}

		private void OnIcon(Int32 characterIndex, Int32 oneParameterFromTag3)
		{
			Dialog.DialogImage dialogImage2 = NGUIText.CreateIconImage(oneParameterFromTag3);
			dialogImage2.TextPosition = characterIndex;
			_dlg.ImageList.Add(dialogImage2);
		}

		private void OnButton(Int32 characterIndex, Boolean checkConfig, String tag, String parameterStr)
		{
			Dialog.DialogImage dialogImage = NGUIText.CreateButtonImage(parameterStr, checkConfig, tag);
			dialogImage.TextPosition = characterIndex;
			_dlg.ImageList.Add(dialogImage);
		}

		private void OnChoise()
		{
			_dlg.StartChoiceRow = _linesNumber;
		}

		private void OnNoTypeEffect()
		{
			_dlg.TypeEffect = false;
		}

		private void OnMessageDelay(Int32 characterIndex, Int32 oneParameterFromTag2)
		{
			_dlg.SetMessageWait(oneParameterFromTag2, characterIndex);
		}

		private void OnMessageSpeed(Int32 characterIndex, Int32 oneParameterFromTag)
		{
			_dlg.SetMessageSpeed(oneParameterFromTag, characterIndex);
		}
	}
}
