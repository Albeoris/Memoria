using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;
using UnityEngine;
using Object = System.Object;

public static class NGUIText
{
	static NGUIText()
	{
		// Note: this type is marked as 'beforefieldinit'.
		NGUIText.StartSentense = "STRT";
		NGUIText.DialogId = "ID";
		NGUIText.Choose = "CHOO";
		NGUIText.AnimationTime = "TIME";
		NGUIText.FlashInh = "FLIM";
		NGUIText.NoAnimation = "NANI";
		NGUIText.NoTypeEffect = "IMME";
		NGUIText.MessageSpeed = "SPED";
		NGUIText.Zidane = "ZDNE";
		NGUIText.Vivi = "VIVI";
		NGUIText.Dagger = "DGGR";
		NGUIText.Steiner = "STNR";
		NGUIText.Fraya = "FRYA";
		NGUIText.Quina = "QUIN";
		NGUIText.Eiko = "EIKO";
		NGUIText.Amarant = "AMRT";
		NGUIText.Party1 = "PTY1";
		NGUIText.Party2 = "PTY2";
		NGUIText.Party3 = "PTY3";
		NGUIText.Party4 = "PTY4";
		NGUIText.Shadow = "HSHD";
		NGUIText.NoShadow = "NSHD";
		NGUIText.ButtonIcon = "DBTN";
		NGUIText.NoFocus = "NFOC";
		NGUIText.IncreaseSignal = "INCS";
		NGUIText.CustomButtonIcon = "CBTN";
		NGUIText.NewIcon = "PNEW";
		NGUIText.TextOffset = "MOVE";
		NGUIText.EndSentence = "ENDN";
		NGUIText.TextVar = "TEXT";
		NGUIText.ItemNameVar = "ITEM";
		NGUIText.SignalVar = "SIGL";
		NGUIText.NumberVar = "NUMB";
		NGUIText.MessageDelay = "WAIT";
		NGUIText.MessageFeed = "FEED";
		NGUIText.MessageTab = "XTAB";
		NGUIText.YAddOffset = "YADD";
		NGUIText.YSubOffset = "YSUB";
		NGUIText.IconVar = "ICON";
		NGUIText.PreChoose = "PCHC";
		NGUIText.PreChooseMask = "PCHM";
		NGUIText.DialogAbsPosition = "MPOS";
		NGUIText.DialogOffsetPositon = "OFFT";
		NGUIText.DialogTailPositon = "TAIL";
		NGUIText.TableStart = "TBLE";
		NGUIText.WidthInfo = "WDTH";
		NGUIText.Center = "CENT";
		NGUIText.Signal = "SIGL";
		NGUIText.NewPage = "PAGE";
		NGUIText.MobileIcon = "MOBI";
		NGUIText.SpacingY = "SPAY";
		NGUIText.KeyboardButtonIcon = "KCBT";
		NGUIText.JoyStickButtonIcon = "JCBT";
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
			NGUIText.Fraya,
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
			NGUIText.Fraya,
			NGUIText.Quina,
			NGUIText.Eiko,
			NGUIText.Amarant,
			NGUIText.Party1,
			NGUIText.Party2,
			NGUIText.Party3,
			NGUIText.Party4
		});
		NGUIText.FF9WhiteColor = "[C8C8C8]";
		NGUIText.FF9YellowColor = "[C8B040]";
		NGUIText.FF9PinkColor = "[B880E0]";
		NGUIText.MobileTouchToConfirmJP = 322;
		NGUIText.MobileTouchToConfirmUS = 323;
		NGUIText.forceShowButton = false;
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

	public static Boolean PhraseOpcodeSymbol(String text, ref Int32 index, ref Boolean highShadow, ref Boolean center, ref Int32 ff9Signal, ref Vector3 extraOffset, ref Single tabX, ref Dialog.DialogImage insertImage)
	{
		if (index + 6 > text.Length || text[index] != '[')
		{
			return false;
		}
		Int32 num = index;
		String a = text.Substring(index, 5);
		String[] renderOpcodeSymbols = NGUIText.RenderOpcodeSymbols;
		for (Int32 i = 0; i < (Int32)renderOpcodeSymbols.Length; i++)
		{
			String text2 = renderOpcodeSymbols[i];
			if (a == "[" + text2)
			{
				NGUIText.PhraseRenderOpcodeSymbol(text, index, ref num, text2, ref highShadow, ref center, ref ff9Signal, ref extraOffset, ref tabX, ref insertImage);
				break;
			}
		}
		if (num == index)
		{
			return false;
		}
		if (num != -1)
		{
			index = num + 1;
			return true;
		}
		index = text.Length;
		return true;
	}

	private static void PhraseRenderOpcodeSymbol(String text, Int32 index, ref Int32 closingBracket, String tag, ref Boolean highShadow, ref Boolean center, ref Int32 ff9Signal, ref Vector3 extraOffset, ref Single tabX, ref Dialog.DialogImage insertImage)
	{
		if (tag == NGUIText.Center)
		{
			closingBracket = text.IndexOf(']', index + 4);
			center = true;
		}
		else if (tag == NGUIText.Shadow)
		{
			closingBracket = text.IndexOf(']', index + 4);
			highShadow = true;
		}
		else if (tag == NGUIText.NoShadow)
		{
			closingBracket = text.IndexOf(']', index + 4);
			highShadow = false;
		}
		else if (tag == NGUIText.Signal)
		{
			Int32 oneParameterFromTag = NGUIText.GetOneParameterFromTag(text, index, ref closingBracket);
			ff9Signal = 10 + oneParameterFromTag;
		}
		else if (tag == NGUIText.IncreaseSignal)
		{
			closingBracket = text.IndexOf(']', index + 4);
			ff9Signal = 2;
		}
		else if (tag == NGUIText.MessageFeed)
		{
			Single[] allParameters = NGUIText.GetAllParameters(text, index, ref closingBracket);
			extraOffset.x += ((allParameters[0] >= 160f) ? 0f : (allParameters[0] * UIManager.ResourceXMultipier));
		}
		else if (tag == NGUIText.YSubOffset)
		{
			Single[] allParameters2 = NGUIText.GetAllParameters(text, index, ref closingBracket);
			extraOffset.y += allParameters2[0] * UIManager.ResourceYMultipier;
		}
		else if (tag == NGUIText.YAddOffset)
		{
			Single[] allParameters3 = NGUIText.GetAllParameters(text, index, ref closingBracket);
			extraOffset.y -= allParameters3[0] * UIManager.ResourceYMultipier;
		}
		else if (tag == NGUIText.MessageTab)
		{
			Single[] allParameters4 = NGUIText.GetAllParameters(text, index, ref closingBracket);
			if (allParameters4[0] == 224f)
			{
				allParameters4[0] = 0f;
			}
			tabX = allParameters4[0] * UIManager.ResourceYMultipier;
		}
		else if (tag == NGUIText.CustomButtonIcon || tag == NGUIText.ButtonIcon || tag == NGUIText.JoyStickButtonIcon || tag == NGUIText.KeyboardButtonIcon)
		{
			closingBracket = text.IndexOf(']', index + 4);
			String parameterStr = text.Substring(index + 6, closingBracket - index - 6);
			Boolean checkConfig = tag == NGUIText.CustomButtonIcon || tag == NGUIText.JoyStickButtonIcon;
			insertImage = NGUIText.CreateButtonImage(parameterStr, checkConfig, tag);
			insertImage.TextPosition = index;
		}
		else if (tag == NGUIText.IconVar)
		{
			Int32 oneParameterFromTag2 = NGUIText.GetOneParameterFromTag(text, index, ref closingBracket);
			insertImage = NGUIText.CreateIconImage(oneParameterFromTag2);
			insertImage.TextPosition = index;
		}
		else if (tag == NGUIText.NewIcon)
		{
			Int32 oneParameterFromTag3 = NGUIText.GetOneParameterFromTag(text, index, ref closingBracket);
			insertImage = NGUIText.CreateIconImage(FF9UIDataTool.NewIconId);
			insertImage.TextPosition = index;
		}
		else if (tag == NGUIText.MobileIcon)
		{
			Int32 oneParameterFromTag4 = NGUIText.GetOneParameterFromTag(text, index, ref closingBracket);
			if (FF9StateSystem.MobilePlatform)
			{
				insertImage = NGUIText.CreateIconImage(oneParameterFromTag4);
				insertImage.TextPosition = index;
			}
		}
		else if (tag == NGUIText.TextOffset)
		{
			Single[] allParameters5 = NGUIText.GetAllParameters(text, index, ref closingBracket);
			extraOffset.x += (allParameters5[0] - 4f) * UIManager.ResourceXMultipier;
			extraOffset.y -= allParameters5[1] * UIManager.ResourceYMultipier;
		}
		else
		{
			closingBracket = text.IndexOf(']', index + 4);
		}
	}

	public static void PhrasePreOpcodeSymbol(String text, Dialog dlg)
	{
		String text2 = String.Empty;
		Int32 length = text.Length;
		Int32 num = 0;
		Boolean flag = FF9StateSystem.Settings.CurrentLanguage == "Japanese";
		FF9StateGlobal ff = FF9StateSystem.Common.FF9;
		ETb etb = (!(PersistenSingleton<EventEngine>.Instance == (UnityEngine.Object)null)) ? PersistenSingleton<EventEngine>.Instance.eTb : ((ETb)null);
		dlg.SignalNumber = ETb.gMesSignal;
		for (Int32 i = 0; i < text.Length; i++)
		{
			length = text.Length;
			if (i + 6 > length || text[i] != '[')
			{
				text2 += text[i];
			}
			else
			{
				if (text[i + 5] == ']')
				{
					String a = text.Substring(i, 6);
					if (a == "[" + NGUIText.NoAnimation + "]")
					{
						dlg.DialogAnimate.ShowWithoutAnimation = true;
						i += 5;
						goto IL_1280;
					}
					if (a == "[" + NGUIText.NoFocus + "]")
					{
						dlg.FlagButtonInh = true;
						dlg.FlagResetChoice = false;
						i += 5;
						goto IL_1280;
					}
					if (a == "[" + NGUIText.FlashInh + "]")
					{
						dlg.TypeEffect = true;
						i += 5;
						goto IL_1280;
					}
					if (a == "[" + NGUIText.EndSentence + "]")
					{
						dlg.EndMode = -1;
						i += 5;
						goto IL_1280;
					}
				}
				Int32 num2 = i;
				String text3 = text.Substring(i, 5);
				if (text3 == "[" + NGUIText.StartSentense)
				{
					Int32[] allParametersFromTag = NGUIText.GetAllParametersFromTag(text, i, ref num2);
					Single num3 = Convert.ToSingle(allParametersFromTag[0]);
					if (num3 > 0f)
					{
						num3 += 3f;
					}
					if (num3 > dlg.CaptionWidth)
					{
						dlg.Width = num3;
					}
					else
					{
						dlg.Width = dlg.CaptionWidth;
					}
					dlg.LineNumber = Convert.ToSingle(allParametersFromTag[1]);
				}
				else if (text3 == "[" + NGUIText.DialogTailPositon)
				{
					num2 = text.IndexOf(']', i + 4);
					String text4 = text.Substring(i + 6, num2 - i - 6);
					String text5 = text4;
					switch (text5)
					{
					case "LOR":
						dlg.Tail = Dialog.TailPosition.LowerRight;
						break;
					case "LOL":
						dlg.Tail = Dialog.TailPosition.LowerLeft;
						break;
					case "UPR":
						dlg.Tail = Dialog.TailPosition.UpperRight;
						break;
					case "UPL":
						dlg.Tail = Dialog.TailPosition.UpperLeft;
						break;
					case "LOC":
						dlg.Tail = Dialog.TailPosition.LowerCenter;
						break;
					case "UPC":
						dlg.Tail = Dialog.TailPosition.UpperCenter;
						break;
					case "LORF":
						dlg.Tail = Dialog.TailPosition.LowerRightForce;
						break;
					case "LOLF":
						dlg.Tail = Dialog.TailPosition.LowerLeftForce;
						break;
					case "UPRF":
						dlg.Tail = Dialog.TailPosition.UpperRightForce;
						break;
					case "UPLF":
						dlg.Tail = Dialog.TailPosition.UpperLeftForce;
						break;
					case "DEFT":
						dlg.Tail = Dialog.TailPosition.DialogPosition;
						break;
					}
				}
				else if (text3 == "[" + NGUIText.WidthInfo)
				{
					Int32[] allParametersFromTag2 = NGUIText.GetAllParametersFromTag(text, i, ref num2);
					Int32 num5 = 0;
					while (num5 + 2 < (Int32)allParametersFromTag2.Length)
					{
						Int32 num6 = 0;
						Int32 num7 = allParametersFromTag2[num5];
						Int32 num8 = allParametersFromTag2[num5 + 1];
						if (dlg.DisableIndexes.Contains(num7 - 1))
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
						String[] array = (from val in list
						select val.ToString()).ToArray<String>();
						num8 += (Int32)NGUIText.GetDialogWidthFromSpecialOpcode(list, etb, dlg.PhraseLabel);
						if (dlg.OriginalWidth < (Single)num8)
						{
							dlg.Width = (Single)num8;
						}
					}
				}
				else if (text3 == "[" + NGUIText.PreChoose)
				{
					Int32[] allParametersFromTag3 = NGUIText.GetAllParametersFromTag(text, i, ref num2);
					ETb.sChooseMask = -1;
					dlg.ChoiceNumber = Convert.ToInt32(allParametersFromTag3[0]);
					dlg.DefaultChoice = (Int32)((ETb.sChoose < 0) ? 0 : ETb.sChoose);
					dlg.DefaultChoice = (Int32)((dlg.DefaultChoice < dlg.ChoiceNumber) ? dlg.DefaultChoice : (dlg.ChoiceNumber - 1));
					Int32 num9 = Convert.ToInt32(allParametersFromTag3[1]);
					dlg.CancelChoice = (Int32)((num9 <= -1) ? (dlg.ChoiceNumber - 1) : num9);
				}
				else if (text3 == "[" + NGUIText.PreChooseMask)
				{
					Int32[] allParametersFromTag4 = NGUIText.GetAllParametersFromTag(text, i, ref num2);
					ETb.sChooseMask = ETb.sChooseMaskInit;
					dlg.ChoiceNumber = Convert.ToInt32(allParametersFromTag4[0]);
					dlg.CancelChoice = Convert.ToInt32(allParametersFromTag4[1]);
					dlg.DefaultChoice = (Int32)((ETb.sChoose < 0) ? 0 : ETb.sChoose);
					dlg.LineNumber = ((dlg.LineNumber >= (Single)dlg.ChoiceNumber) ? dlg.LineNumber : (dlg.LineNumber + (Single)dlg.ChoiceNumber));
					dlg.ChooseMask = ETb.sChooseMask;
					if (dlg.DisableIndexes.Count > 0)
					{
						if (dlg.DisableIndexes.Contains(dlg.DefaultChoice) || !dlg.ActiveIndexes.Contains(dlg.DefaultChoice))
						{
							dlg.DefaultChoice = dlg.ActiveIndexes.Min();
						}
						if (dlg.DisableIndexes.Contains(dlg.CancelChoice) || !dlg.ActiveIndexes.Contains(dlg.CancelChoice))
						{
							dlg.CancelChoice = dlg.ActiveIndexes.Max();
						}
					}
					else
					{
						dlg.DefaultChoice = (Int32)((dlg.DefaultChoice < dlg.ChoiceNumber) ? dlg.DefaultChoice : (dlg.ChoiceNumber - 1));
					}
					num = 0;
				}
				else if (text3 == "[" + NGUIText.AnimationTime)
				{
					Int32 oneParameterFromTag = NGUIText.GetOneParameterFromTag(text, i, ref num2);
					if (oneParameterFromTag > 0)
					{
						dlg.EndMode = oneParameterFromTag;
						dlg.FlagButtonInh = true;
					}
					else if (oneParameterFromTag == -1)
					{
						dlg.FlagButtonInh = true;
					}
					else
					{
						dlg.FlagButtonInh = false;
					}
				}
				else if (text3 == "[" + NGUIText.TextOffset)
				{
					Int32[] allParametersFromTag5 = NGUIText.GetAllParametersFromTag(text, i, ref num2);
					if (dlg.SkipThisChoice(num))
					{
						num2 = text.IndexOf('[' + NGUIText.TextOffset, num2);
						if (num2 >= 0)
						{
							num2--;
						}
					}
					else if (allParametersFromTag5[0] == 18 && allParametersFromTag5[1] == 0)
					{
						text2 += "    ";
					}
					else
					{
						String text5 = text2;
						text2 = String.Concat(new Object[]
						{
							text5,
							text3,
							"=",
							allParametersFromTag5[0],
							",",
							allParametersFromTag5[1],
							"]"
						});
					}
					num++;
				}
				else if (text3 == "[" + NGUIText.CustomButtonIcon || text3 == "[" + NGUIText.ButtonIcon || text3 == "[" + NGUIText.JoyStickButtonIcon || text3 == "[" + NGUIText.KeyboardButtonIcon)
				{
					num2 = text.IndexOf(']', i + 4);
					String text6 = text.Substring(i + 6, num2 - i - 6);
					if (!FF9StateSystem.MobilePlatform || text3 == "[" + NGUIText.JoyStickButtonIcon || text3 == "[" + NGUIText.KeyboardButtonIcon || NGUIText.ForceShowButton)
					{
						String text5 = text2;
						text2 = String.Concat(new String[]
						{
							text5,
							text3,
							"=",
							text6,
							"] "
						});
					}
				}
				else if (text3 == "[" + NGUIText.IconVar)
				{
					Int32 oneParameterFromTag2 = NGUIText.GetOneParameterFromTag(text, i, ref num2);
					String str = String.Empty;
					Int32 num4 = oneParameterFromTag2;
					switch (num4)
					{
					case 34:
						str = "[sub]0[/sub]";
						break;
					case 35:
						str = "[sub]1[/sub]";
						break;
					case 36:
					case 37:
					case 38:
						IL_9B2:
						switch (num4)
						{
						case 159:
							str = "[sup]" + Localization.Get("Miss") + "[/sup]";
							break;
						case 160:
							str = "[sup]" + Localization.Get("Death") + "[/sup]";
							break;
						case 161:
							str = "[sup]" + Localization.Get("Guard") + "[/sup]";
							break;
						case 162:
							IL_9D3:
							if (num4 != 173)
							{
								if (num4 != 174)
								{
									if (num4 != 45)
									{
										if (num4 != 179)
										{
											str = String.Concat(new Object[]
											{
												text3,
												"=",
												oneParameterFromTag2,
												"] "
											});
										}
										else
										{
											str = String.Concat(new String[]
											{
												NGUIText.FF9YellowColor,
												"[sup]",
												Localization.Get("Critical"),
												"[/sup]",
												NGUIText.FF9WhiteColor
											});
										}
									}
									else
									{
										str = "[sub]/[/sub]";
									}
								}
								else
								{
									str = "/";
								}
							}
							else
							{
								str = "9";
							}
							break;
						case 163:
							str = "[sup]" + Localization.Get("MPCaption") + "[/sup]";
							break;
						default:
							goto IL_9D3;
						}
						break;
					case 39:
						str = "[sub]5[/sub]";
						break;
					default:
						goto IL_9B2;
					}
					text2 += str;
				}
				else if (text3 == "[" + NGUIText.NewIcon)
				{
					Int32 oneParameterFromTag3 = NGUIText.GetOneParameterFromTag(text, i, ref num2);
					if ((etb.gMesValue[0] & 1 << oneParameterFromTag3) > 0)
					{
						String text5 = text2;
						text2 = String.Concat(new Object[]
						{
							text5,
							text3,
							"=",
							oneParameterFromTag3,
							"] "
						});
					}
				}
				else if (text3 == "[" + NGUIText.MobileIcon)
				{
					Int32 oneParameterFromTag4 = NGUIText.GetOneParameterFromTag(text, i, ref num2);
					if (FF9StateSystem.MobilePlatform && !NGUIText.ForceShowButton)
                    {
						String text5 = text2;
						text2 = String.Concat(new Object[]
						{
							text5,
							text3,
							"=",
							oneParameterFromTag4,
							"] "
						});
					}
				}
				else if (text3 == "[" + NGUIText.DialogOffsetPositon)
				{
					Int32[] allParametersFromTag6 = NGUIText.GetAllParametersFromTag(text, i, ref num2);
					dlg.OffsetPosition = new Vector3((Single)allParametersFromTag6[0], (Single)allParametersFromTag6[1], (Single)allParametersFromTag6[2]);
				}
				else if (NGUIText.nameKeywordList.Contains(text3.Remove(0, 1)))
				{
					String a2 = text3.Remove(0, 1);
					String text7 = String.Empty;
					String str2 = String.Empty;
					num2 = text.IndexOf(']', i + 4);
					if (a2 == NGUIText.Zidane)
					{
						text2 += FF9StateSystem.Common.FF9.player[0].name;
					}
					else if (a2 == NGUIText.Vivi)
					{
						text2 += FF9StateSystem.Common.FF9.player[1].name;
					}
					else if (a2 == NGUIText.Dagger)
					{
						text2 += FF9StateSystem.Common.FF9.player[2].name;
					}
					else if (a2 == NGUIText.Steiner)
					{
						text2 += FF9StateSystem.Common.FF9.player[3].name;
					}
					else if (a2 == NGUIText.Fraya)
					{
						text2 += FF9StateSystem.Common.FF9.player[4].name;
					}
					else if (a2 == NGUIText.Quina)
					{
						text2 += FF9StateSystem.Common.FF9.player[5].name;
					}
					else if (a2 == NGUIText.Eiko)
					{
						text2 += FF9StateSystem.Common.FF9.player[6].name;
					}
					else if (a2 == NGUIText.Amarant)
					{
						text2 += FF9StateSystem.Common.FF9.player[7].name;
					}
					else if (a2 == NGUIText.Party1)
					{
						Int32 partyPlayer = PersistenSingleton<EventEngine>.Instance.GetPartyPlayer(0);
						PLAYER player = ff.player[partyPlayer];
						str2 = player.name;
						text2 += str2;
						text7 = "Party 1:" + str2;
					}
					else if (a2 == NGUIText.Party2)
					{
						Int32 partyPlayer = PersistenSingleton<EventEngine>.Instance.GetPartyPlayer(1);
						PLAYER player = ff.player[partyPlayer];
						str2 = player.name;
						text2 += str2;
						text7 = "Party 2:" + str2;
					}
					else if (a2 == NGUIText.Party3)
					{
						Int32 partyPlayer = PersistenSingleton<EventEngine>.Instance.GetPartyPlayer(2);
						PLAYER player = ff.player[partyPlayer];
						str2 = player.name;
						text2 += str2;
						text7 = "Party 3:" + str2;
					}
					else if (a2 == NGUIText.Party4)
					{
						Int32 partyPlayer = PersistenSingleton<EventEngine>.Instance.GetPartyPlayer(3);
						PLAYER player = ff.player[partyPlayer];
						str2 = player.name;
						text2 += str2;
						text7 = "Party 4:" + str2;
					}
				}
				else if (text3 == "[" + NGUIText.NumberVar)
				{
					Int32 num10 = 0;
					Int32 oneParameterFromTag5 = NGUIText.GetOneParameterFromTag(text, i, ref num10);
					Int32 value = etb.gMesValue[oneParameterFromTag5];
					if (!dlg.MessageValues.ContainsKey(oneParameterFromTag5))
					{
						dlg.MessageValues.Add(oneParameterFromTag5, value);
					}
					dlg.MessageNeedUpdate = true;
				}
				else if (text3 == "[" + NGUIText.ItemNameVar)
				{
					Int32 oneParameterFromTag6 = NGUIText.GetOneParameterFromTag(text, i, ref num2);
					text2 = text2 + "[C8B040][HSHD]" + ETb.GetItemName(etb.gMesValue[oneParameterFromTag6]) + "[C8C8C8]";
				}
				else if (text3 == "[" + NGUIText.Signal)
				{
					Int32 num11 = text.IndexOf(']', i + 4);
					String value2 = text.Substring(i + 6, num11 - i - 6);
					Int32 signalNumber = Convert.ToInt32(value2);
					dlg.SignalNumber = signalNumber;
					dlg.SignalMode = 1;
				}
				else if (text3 == "[" + NGUIText.IncreaseSignal)
				{
					dlg.SignalNumber++;
					dlg.SignalMode = 2;
				}
				else if (text3 == "[" + NGUIText.DialogAbsPosition)
				{
					Single[] allParameters = NGUIText.GetAllParameters(text, i, ref num2);
					dlg.Position = new Vector2(allParameters[0], allParameters[1]);
				}
				else if (text3 == "[" + NGUIText.TextVar)
				{
					Int32[] allParametersFromTag7 = NGUIText.GetAllParametersFromTag(text, i, ref num2);
					text2 += etb.GetStringFromTable(Convert.ToUInt32(allParametersFromTag7[0]), Convert.ToUInt32(allParametersFromTag7[1]));
				}
				else if (text3 == "[" + NGUIText.Choose)
				{
					Int32 startIndex = text.IndexOf(']', i + 4);
					if (flag)
					{
						Int32[] array2 = new Int32[]
						{
							-1
						};
						if (dlg.DisableIndexes != null)
						{
							array2 = ((dlg.DisableIndexes.Count <= 0) ? array2 : dlg.DisableIndexes.ToArray());
						}
						global::Debug.Log(text2);
						text = NGUIText.ProcessJapaneseChoose(text, startIndex, array2);
					}
				}
				else if (text3 == "[" + NGUIText.NewPage)
				{
					Int32 num12 = text.IndexOf(']', i + 4);
					dlg.SubPage.Add(NGUIText.FF9WhiteColor + text2);
					text2 = String.Empty;
				}
				if (num2 == i)
				{
					text2 += text[i];
				}
				else if (num2 != -1)
				{
					i = num2;
				}
				else
				{
					i = length;
				}
			}
			IL_1280:;
		}
		dlg.SubPage.Add(NGUIText.FF9WhiteColor + text2);
	}

	public static void PhraseOpcodeSymbol(String text, Dialog dialog)
	{
		Int32 num = 0;
		for (Int32 i = 0; i < text.Length; i++)
		{
			Int32 length = text.Length;
			if (text[i] == '\n')
			{
				num++;
			}
			if (i + 6 <= length && text[i] == '[')
			{
				Int32 num2 = i;
				String a = text.Substring(i, 5);
				if (a == "[" + NGUIText.MessageSpeed)
				{
					Int32 oneParameterFromTag = NGUIText.GetOneParameterFromTag(text, i, ref num2);
					dialog.SetMessageSpeed(oneParameterFromTag, i);
				}
				if (a == "[" + NGUIText.MessageDelay)
				{
					Int32 oneParameterFromTag2 = NGUIText.GetOneParameterFromTag(text, i, ref num2);
					dialog.SetMessageWait(oneParameterFromTag2, i);
				}
				else if (a == "[" + NGUIText.NoTypeEffect)
				{
					num2 = text.IndexOf(']', i + 4);
					dialog.TypeEffect = false;
				}
				else if (a == "[" + NGUIText.Choose)
				{
					num2 = text.IndexOf(']', i + 4);
					dialog.StartChoiceRow = num;
				}
				else if (a == "[" + NGUIText.CustomButtonIcon || a == "[" + NGUIText.ButtonIcon || a == "[" + NGUIText.JoyStickButtonIcon || a == "[" + NGUIText.KeyboardButtonIcon)
				{
					num2 = text.IndexOf(']', i + 4);
					String parameterStr = text.Substring(i + 6, num2 - i - 6);
					Boolean checkConfig = a == "[" + NGUIText.CustomButtonIcon || a == "[" + NGUIText.JoyStickButtonIcon;
					String tag = text.Substring(i + 1, 4);
					Dialog.DialogImage dialogImage = NGUIText.CreateButtonImage(parameterStr, checkConfig, tag);
					dialogImage.TextPosition = i;
					dialog.ImageList.Add(dialogImage);
				}
				else if (a == "[" + NGUIText.IconVar)
				{
					Int32 oneParameterFromTag3 = NGUIText.GetOneParameterFromTag(text, i, ref num2);
					Dialog.DialogImage dialogImage2 = NGUIText.CreateIconImage(oneParameterFromTag3);
					dialogImage2.TextPosition = i;
					dialog.ImageList.Add(dialogImage2);
				}
				else if (a == "[" + NGUIText.NewIcon)
				{
					Int32 oneParameterFromTag4 = NGUIText.GetOneParameterFromTag(text, i, ref num2);
					Dialog.DialogImage dialogImage3 = NGUIText.CreateIconImage(FF9UIDataTool.NewIconId);
					dialogImage3.TextPosition = i;
					dialog.ImageList.Add(dialogImage3);
				}
				else if (a == "[" + NGUIText.MobileIcon)
				{
					Int32 oneParameterFromTag5 = NGUIText.GetOneParameterFromTag(text, i, ref num2);
					Dialog.DialogImage dialogImage4 = NGUIText.CreateIconImage(oneParameterFromTag5);
					dialogImage4.TextPosition = i;
					dialog.ImageList.Add(dialogImage4);
				}
				if (num2 == i)
				{
					if (num2 != -1)
					{
						i = num2;
					}
					else
					{
						i = text.Length;
					}
				}
			}
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
		PLAYER[] player = FF9StateSystem.Common.FF9.player;
		PLAYER[] member = FF9StateSystem.Common.FF9.party.member;
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
				num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, player[0].name);
				break;
			case 17:
				num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, player[1].name);
				break;
			case 18:
				num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, player[2].name);
				break;
			case 19:
				num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, player[3].name);
				break;
			case 20:
				num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, player[4].name);
				break;
			case 21:
				num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, player[5].name);
				break;
			case 22:
				num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, player[6].name);
				break;
			case 23:
				num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, player[7].name);
				break;
			case 24:
			case 25:
			case 26:
			case 27:
			{
				FF9StateGlobal ff = FF9StateSystem.Common.FF9;
				Int32 partyPlayer = PersistenSingleton<EventEngine>.Instance.GetPartyPlayer(num2 - 24);
				PLAYER player2 = ff.player[partyPlayer];
				num += NGUIText.GetTextWidthFromFF9Font(phraseLabel, player2.name);
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

	public static String ProcessJapaneseChoose(String text, Int32 startIndex, Int32[] disableChoice)
	{
		Int32 num = text.IndexOf('[' + NGUIText.EndSentence, startIndex + 1) - startIndex;
		num = (Int32)((num > 0) ? num : (text.IndexOf("[TIME=-1]", startIndex + 1) - startIndex));
		String text2 = text.Substring(startIndex + 1, num);
		String[] array = text2.Split(new Char[]
		{
			'\n'
		});
		String text3 = String.Empty;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			String text4 = array[i];
			Boolean flag = true;
			for (Int32 j = 0; j < (Int32)disableChoice.Length; j++)
			{
				Int32 num2 = disableChoice[j];
				if (i == num2)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				text3 += text4.Replace("  ", "    ");
				if (i + 1 < (Int32)array.Length)
				{
					text3 += '\n';
				}
			}
		}
		return text.Replace(text2, text3);
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
			goto IL_200;
		case "DOWN":
			control = Control.Down;
			goto IL_200;
		case "LEFT":
			control = Control.Left;
			goto IL_200;
		case "RIGHT":
			control = Control.Right;
			goto IL_200;
		case "CIRCLE":
			if (checkConfig)
			{
				control = (Control)((!EventInput.IsJapaneseLayout) ? Control.Cancel : Control.Confirm);
			}
			else
			{
				control = Control.Confirm;
			}
			goto IL_200;
		case "CROSS":
			if (checkConfig)
			{
				control = (Control)((!EventInput.IsJapaneseLayout) ? Control.Confirm : Control.Cancel);
			}
			else
			{
				control = Control.Cancel;
			}
			goto IL_200;
		case "TRIANGLE":
			control = Control.Menu;
			goto IL_200;
		case "SQUARE":
			control = Control.Special;
			goto IL_200;
		case "R1":
			control = Control.RightBumper;
			goto IL_200;
		case "R2":
			control = Control.RightTrigger;
			goto IL_200;
		case "L1":
			control = Control.LeftBumper;
			goto IL_200;
		case "L2":
			control = Control.LeftTrigger;
			goto IL_200;
		case "SELECT":
			control = Control.Select;
			goto IL_200;
		case "START":
			control = Control.Pause;
			goto IL_200;
		case "PAD":
			control = Control.DPad;
			goto IL_200;
		}
		control = Control.None;
		IL_200:
		dialogImage.Size = FF9UIDataTool.GetButtonSize(control, checkConfig, tag);
		dialogImage.Id = (Int32)control;
		dialogImage.tag = tag;
		return dialogImage;
	}

    public static Dialog.DialogImage CreateIconImage(int iconId)
    {
        Dialog.DialogImage dialogImage = new Dialog.DialogImage();
        dialogImage.Size = FF9UIDataTool.GetIconSize(iconId);
        if (iconId == 180)
        {
            dialogImage.Offset = new Vector3(0f, -15.2f);
        }
        else
        {
            dialogImage.Offset = new Vector3(0f, -10f);
        }
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

	public static Int32[] GetAllParametersFromTag(String fullText, Int32 currentIndex, ref Int32 closingBracket)
	{
		closingBracket = fullText.IndexOf(']', currentIndex + 4);
		String text = fullText.Substring(currentIndex + 6, closingBracket - currentIndex - 6);
		String[] array = text.Split(new Char[]
		{
			','
		});
		return Array.ConvertAll<String, Int32>(array, new Converter<String, Int32>(Int32.Parse));
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

	public static String ReplaceNumberValue(String phrase, Dialog dialog)
	{
		String text = phrase;
		foreach (KeyValuePair<Int32, Int32> keyValuePair in dialog.MessageValues)
		{
			text = text.Replace(String.Concat(new Object[]
			{
				"[",
				NGUIText.NumberVar,
				"=",
				keyValuePair.Key,
				"]"
			}), keyValuePair.Value.ToString());
		}
		return text;
	}

	public static Boolean ContainsTextOffset(Dialog dialog)
	{
		foreach (String text in dialog.SubPage)
		{
			if (NGUIText.ContainsTextOffset(text))
			{
				return true;
			}
		}
		return false;
	}

	public static Boolean ContainsTextOffset(String text)
	{
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
					if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref num, ref flag, ref flag2, ref flag3, ref flag4, ref flag5, ref flag6, ref flag7, ref num2, ref zero2, ref num3, ref dialogImage))
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
				if (c == '[')
				{
					Int32 num = 0;
					Boolean flag = false;
					Boolean flag2 = false;
					Boolean flag3 = false;
					Boolean flag4 = false;
					Boolean flag5 = false;
					Boolean flag6 = false;
					Boolean flag7 = false;
					Int32 num2 = i;
					Int32 num3 = 0;
					Vector3 zero = Vector3.zero;
					Single num4 = 0f;
					Dialog.DialogImage dialogImage = (Dialog.DialogImage)null;
					if (NGUIText.ParseSymbol(text, ref num2, null, false, ref num, ref flag, ref flag2, ref flag3, ref flag4, ref flag5, ref flag6, ref flag7, ref num3, ref zero, ref num4, ref dialogImage))
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
		Vector3 zero = Vector3.zero;
		Single num2 = 0f;
		return NGUIText.ParseSymbol(text, ref index, null, false, ref num, ref flag, ref flag2, ref flag3, ref flag4, ref flag5, ref flag6, ref flag7, ref ff9Signal, ref zero, ref num2, ref insertImage);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static Boolean IsHex(Char ch)
	{
		return (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
	}

	public static Boolean ParseSymbol(String text, ref Int32 index, BetterList<Color> colors, Boolean premultiply, ref Int32 sub, ref Boolean bold, ref Boolean italic, ref Boolean underline, ref Boolean strike, ref Boolean ignoreColor, ref Boolean highShadow, ref Boolean center, ref Int32 ff9Signal, ref Vector3 extraOffset, ref Single tabX, ref Dialog.DialogImage insertImage)
	{
		Int32 length = text.Length;
		if (index + 3 > length || text[index] != '[')
		{
			return false;
		}
		if (text[index + 2] == ']')
		{
			if (text[index + 1] == '-')
			{
				if (colors != null && colors.size > 1)
				{
					colors.RemoveAt(colors.size - 1);
				}
				index += 3;
				return true;
			}
			String text2 = text.Substring(index, 3);
			String text3 = text2;
			switch (text3)
			{
			case "[b]":
				bold = true;
				index += 3;
				return true;
			case "[i]":
				italic = true;
				index += 3;
				return true;
			case "[u]":
				underline = true;
				index += 3;
				return true;
			case "[s]":
				strike = true;
				index += 3;
				return true;
			case "[c]":
				ignoreColor = true;
				index += 3;
				return true;
			}
		}
		if (index + 4 > length)
		{
			return false;
		}
		if (text[index + 3] == ']')
		{
			String text4 = text.Substring(index, 4);
			String text3 = text4;
			switch (text3)
			{
			case "[/b]":
				bold = false;
				index += 4;
				return true;
			case "[/i]":
				italic = false;
				index += 4;
				return true;
			case "[/u]":
				underline = false;
				index += 4;
				return true;
			case "[/s]":
				strike = false;
				index += 4;
				return true;
			case "[/c]":
				ignoreColor = false;
				index += 4;
				return true;
			}
			Char ch = text[index + 1];
			Char ch2 = text[index + 2];
			if (NGUIText.IsHex(ch) && NGUIText.IsHex(ch2))
			{
				Int32 num2 = NGUIMath.HexToDecimal(ch) << 4 | NGUIMath.HexToDecimal(ch2);
				NGUIText.mAlpha = (Single)num2 / 255f;
				index += 4;
				return true;
			}
		}
		if (index + 5 > length)
		{
			return false;
		}
		if (text[index + 4] == ']')
		{
			String text5 = text.Substring(index, 5);
            switch (text5)
            {
                case "[sub]":
                    sub = 1;
                    index += 5;
                    return true;
                case "[sup]":
                    sub = 2;
                    index += 5;
                    return true;

            }
        }
		if (index + 6 > length)
		{
			return false;
		}
		if (NGUIText.PhraseOpcodeSymbol(text, ref index, ref highShadow, ref center, ref ff9Signal, ref extraOffset, ref tabX, ref insertImage))
		{
			return true;
		}
		if (text[index + 5] == ']')
		{
			String text6 = text.Substring(index, 6);
			String text3 = text6;
			switch (text3)
			{
			case "[/sub]":
				sub = 0;
				index += 6;
				return true;
			case "[/sup]":
				sub = 0;
				index += 6;
				return true;
			case "[/url]":
				index += 6;
				return true;
			}
		}
		if (text[index + 1] == 'u' && text[index + 2] == 'r' && text[index + 3] == 'l' && text[index + 4] == '=')
		{
			Int32 num3 = text.IndexOf(']', index + 4);
			if (num3 != -1)
			{
				index = num3 + 1;
				center = true;
				return true;
			}
			index = text.Length;
			center = false;
			return true;
		}
		else
		{
			if (index + 8 > length)
			{
				return false;
			}
			if (text[index + 7] == ']')
			{
				Color color = NGUIText.ParseColor24(text, index + 1);
				if (NGUIText.EncodeColor24(color) != text.Substring(index + 1, 6).ToUpper())
				{
					return false;
				}
				if (colors != null)
				{
					color.a = colors[colors.size - 1].a;
					if (premultiply && color.a != 1f)
					{
						color = Color.Lerp(NGUIText.mInvisible, color, color.a);
					}
					colors.Add(color);
				}
				index += 8;
				return true;
			}
			else
			{
				if (index + 10 > length)
				{
					return false;
				}
				if (text[index + 9] != ']')
				{
					return false;
				}
				Color color2 = NGUIText.ParseColor32(text, index + 1);
				if (NGUIText.EncodeColor32(color2) != text.Substring(index + 1, 8).ToUpper())
				{
					return false;
				}
				if (colors != null)
				{
					if (premultiply && color2.a != 1f)
					{
						color2 = Color.Lerp(NGUIText.mInvisible, color2, color2.a);
					}
					colors.Add(color2);
				}
				index += 10;
				return true;
			}
		}
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
				if (c == '[')
				{
					Int32 num = 0;
					Boolean flag = false;
					Boolean flag2 = false;
					Boolean flag3 = false;
					Boolean flag4 = false;
					Boolean flag5 = false;
					Boolean flag6 = false;
					Boolean flag7 = false;
					Int32 num2 = i;
					Int32 num3 = 0;
					Vector3 zero = Vector3.zero;
					Single num4 = 0f;
					Dialog.DialogImage dialogImage = (Dialog.DialogImage)null;
					if (NGUIText.ParseSymbol(text, ref num2, null, false, ref num, ref flag, ref flag2, ref flag3, ref flag4, ref flag5, ref flag6, ref flag7, ref num3, ref zero, ref num4, ref dialogImage))
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
			Single num13 = ((Single)NGUIText.rectWidth - printedWidth) * 0.5f;
			if (num13 < 1f)
			{
				return;
			}
			Int32 num14 = (verts.size - indexOffset) / elements;
			if (num14 < 1)
			{
				return;
			}
			Single num15 = 1f / (Single)(num14 - 1);
			Single num16 = (Single)NGUIText.rectWidth / printedWidth;
			Int32 l = indexOffset + elements;
			Int32 num17 = 1;
			while (l < verts.size)
			{
				Single num18 = verts.buffer[l].x;
				Single num19 = verts.buffer[l + elements / 2].x;
				Single num20 = num19 - num18;
				Single num21 = num18 * num16;
				Single a = num21 + num20;
				Single num22 = num19 * num16;
				Single b = num22 - num20;
				Single t = (Single)num17 * num15;
				num19 = Mathf.Lerp(a, num22, t);
				num18 = Mathf.Lerp(num21, b, t);
				num18 = Mathf.Round(num18);
				num19 = Mathf.Round(num19);
				if (elements == 4)
				{
					verts.buffer[l++].x = num18;
					verts.buffer[l++].x = num18;
					verts.buffer[l++].x = num19;
					verts.buffer[l++].x = num19;
				}
				else if (elements == 2)
				{
					verts.buffer[l++].x = num18;
					verts.buffer[l++].x = num19;
				}
				else if (elements == 1)
				{
					verts.buffer[l++].x = num18;
				}
				num17++;
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
					else if (NGUIText.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref num6, ref flag4, ref flag5, ref flag6, ref flag7, ref flag8, ref flag9, ref flag10, ref num7, ref zero, ref num8, ref dialogImage))
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
		Int32 prev = 0;
		Int32 num = 0;
		Single num2 = 0f;
		Single num3 = 0f;
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
		Int32 num9 = 0;
		Boolean flag2 = false;
		Boolean flag3 = false;
		Boolean flag4 = false;
		Boolean flag5 = false;
		Boolean flag6 = false;
		Boolean flag7 = false;
		Boolean flag8 = false;
		Boolean flag9 = false;
		Int32 num10 = 0;
		Vector3 zero = Vector3.zero;
		Dialog.DialogImage dialogImage = (Dialog.DialogImage)null;
		Int32 num11 = 0;
		Single num12 = 0f;
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
			Int32 num13 = (Int32)text[i];
			Single num14 = num2;
			if (num13 == 10)
			{
				if (num2 > num4)
				{
					num4 = num2;
				}
				if (verts.size > 0 && flag9)
				{
					NGUIText.AlignImageWithLastChar(ref specialImages, betterList, verts, num11);
				}
				betterList.Clear();
				NGUIText.AlignImage(verts, size, num2 - NGUIText.finalSpacingX, specialImages, num11);
				NGUIText.Align(verts, size, num2 - NGUIText.finalSpacingX, 4);
				size = verts.size;
				vertsLineOffsets.Add(size);
				NGUIText.alignment = alignment;
				flag8 = false;
				flag9 = false;
				zero = Vector3.zero;
				num11++;
				num2 = 0f;
				num3 += NGUIText.finalLineHeight;
				prev = 0;
			}
			else if (num13 < 32)
			{
				prev = num13;
			}
			else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref num9, ref flag2, ref flag3, ref flag4, ref flag5, ref flag6, ref flag7, ref flag8, ref num10, ref zero, ref num12, ref dialogImage))
			{
				Color color2;
				if (flag6)
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
				if (flag8)
				{
					NGUIText.alignment = NGUIText.Alignment.Center;
				}
				else
				{
					NGUIText.alignment = alignment;
				}
				if (num12 != 0f)
				{
					zero.x = 0f;
					num2 = num12;
					num12 = 0f;
				}
				NGUIText.AddSpecialIconToList(ref specialImages, ref betterList, ref dialogImage, zero, ref num2, num3, num11);
				BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
				if (bmsymbol != null)
				{
					Single num16 = num2 + (Single)bmsymbol.offsetX * NGUIText.fontScale;
					Single num17 = num16 + (Single)bmsymbol.width * NGUIText.fontScale;
					Single num18 = -(num3 + (Single)bmsymbol.offsetY * NGUIText.fontScale);
					Single num19 = num18 - (Single)bmsymbol.height * NGUIText.fontScale;
					if (Mathf.RoundToInt(num2 + (Single)bmsymbol.advance * NGUIText.fontScale) > NGUIText.regionWidth)
					{
						if (num2 == 0f)
						{
							return;
						}
						if (size < verts.size)
						{
							NGUIText.AlignImage(verts, size, num2 - NGUIText.finalSpacingX, specialImages, num11);
							NGUIText.Align(verts, size, num2 - NGUIText.finalSpacingX, 4);
							size = verts.size;
							vertsLineOffsets.Add(size);
						}
						NGUIText.alignment = alignment;
						flag8 = false;
						zero = Vector3.zero;
						num11++;
						num16 -= num2;
						num17 -= num2;
						num19 -= NGUIText.finalLineHeight;
						num18 -= NGUIText.finalLineHeight;
						num2 = 0f;
						num3 += NGUIText.finalLineHeight;
					}
					verts.Add(new Vector3(num16, num19));
					verts.Add(new Vector3(num16, num18));
					verts.Add(new Vector3(num17, num18));
					verts.Add(new Vector3(num17, num19));
					num2 += NGUIText.finalSpacingX + (Single)bmsymbol.advance * NGUIText.fontScale;
					i += bmsymbol.length - 1;
					prev = 0;
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
					NGUIText.GlyphInfo glyphInfo = NGUIText.GetGlyph(num13, prev);
					if (glyphInfo != null)
					{
						if (!flag9)
						{
							flag9 = NGUIText.ContainCharAlignment(num13);
						}
						prev = num13;
						num = num13;
						if (num9 != 0)
						{
							NGUIText.GlyphInfo glyphInfo2 = glyphInfo;
							glyphInfo2.v0.x = glyphInfo2.v0.x * 0.75f;
							NGUIText.GlyphInfo glyphInfo3 = glyphInfo;
							glyphInfo3.v0.y = glyphInfo3.v0.y * 0.75f;
							NGUIText.GlyphInfo glyphInfo4 = glyphInfo;
							glyphInfo4.v1.x = glyphInfo4.v1.x * 0.75f;
							NGUIText.GlyphInfo glyphInfo5 = glyphInfo;
							glyphInfo5.v1.y = glyphInfo5.v1.y * 0.75f;
							if (num9 == 1)
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
						Single num16 = glyphInfo.v0.x + num2;
						Single num19 = glyphInfo.v0.y - num3;
						Single num17 = glyphInfo.v1.x + num2;
						Single num18 = glyphInfo.v1.y - num3;
						Single num20 = glyphInfo.advance;
						if (NGUIText.finalSpacingX < 0f)
						{
							num20 += NGUIText.finalSpacingX;
						}
						if (Mathf.RoundToInt(num2 + num20) > NGUIText.regionWidth)
						{
							if (num2 == 0f)
							{
								return;
							}
							if (size < verts.size)
							{
								NGUIText.Align(verts, size, num2 - NGUIText.finalSpacingX, 4);
								size = verts.size;
								vertsLineOffsets.Add(size);
							}
							zero = Vector3.zero;
							num16 -= num2;
							num17 -= num2;
							num19 -= NGUIText.finalLineHeight;
							num18 -= NGUIText.finalLineHeight;
							num2 = 0f;
							num3 += NGUIText.finalLineHeight;
							num14 = 0f;
							NGUIText.alignment = alignment;
							flag8 = false;
						}
						if (NGUIText.IsSpace(num13))
						{
							if (flag4)
							{
								num13 = 95;
							}
							else if (flag5)
							{
								num13 = 45;
							}
						}
						num2 += ((num9 != 0) ? ((NGUIText.finalSpacingX + glyphInfo.advance) * 0.75f) : (NGUIText.finalSpacingX + glyphInfo.advance));
						if (!NGUIText.IsSpace(num13))
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
								Int32 num21 = (Int32)((!flag2) ? 1 : 4);
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
										Int32 num24 = (Int32)((!flag2) ? 1 : 4);
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
										Int32 num26 = (Int32)((!flag2) ? 4 : 16);
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
									Int32 num28 = (Int32)((!flag2) ? 4 : 16);
									while (num27 < num28)
									{
										cols.Add(item2);
										num27++;
									}
								}
							}
							if (!flag2)
							{
								if (!flag3)
								{
									verts.Add(new Vector3(num16, num19));
									verts.Add(new Vector3(num16, num18));
									verts.Add(new Vector3(num17, num18));
									verts.Add(new Vector3(num17, num19));
								}
								else
								{
									Single num29 = (Single)NGUIText.fontSize * 0.1f * ((num18 - num19) / (Single)NGUIText.fontSize);
									verts.Add(new Vector3(num16 - num29, num19));
									verts.Add(new Vector3(num16 + num29, num18));
									verts.Add(new Vector3(num17 + num29, num18));
									verts.Add(new Vector3(num17 - num29, num19));
								}
							}
							else
							{
								for (Int32 num30 = 0; num30 < 4; num30++)
								{
									Single num31 = NGUIText.mBoldOffset[num30 * 2];
									Single num32 = NGUIText.mBoldOffset[num30 * 2 + 1];
									Single num33 = (!flag3) ? 0f : ((Single)NGUIText.fontSize * 0.1f * ((num18 - num19) / (Single)NGUIText.fontSize));
									verts.Add(new Vector3(num16 + num31 - num33, num19 + num32));
									verts.Add(new Vector3(num16 + num31 + num33, num18 + num32));
									verts.Add(new Vector3(num17 + num31 + num33, num18 + num32));
									verts.Add(new Vector3(num17 + num31 - num33, num19 + num32));
								}
							}
							if (flag7)
							{
								for (Int32 num34 = verts.size - 4; num34 < verts.size; num34++)
								{
									highShadowVertIndexes.Add(num34);
								}
							}
							if (zero != Vector3.zero)
							{
								for (Int32 num35 = verts.size - 4; num35 < verts.size; num35++)
								{
									Int32 i3;
									Int32 i2 = i3 = num35;
									Vector3 a2 = verts[i3];
									verts[i2] = a2 + zero;
								}
							}
							if (NGUIText.ContainCharAlignment(num13))
							{
								NGUIText.AlignImageWithLastChar(ref specialImages, betterList, verts, num11);
								betterList.Clear();
							}
							if (flag4 || flag5)
							{
								NGUIText.GlyphInfo glyphInfo10 = NGUIText.GetGlyph((Int32)((!flag5) ? 95 : 45), prev);
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
										Int32 num37 = (Int32)((!flag2) ? 1 : 4);
										while (num36 < num37)
										{
											uvs.Add(new Vector2(x, glyphInfo10.u0.y));
											uvs.Add(new Vector2(x, glyphInfo10.u2.y));
											uvs.Add(new Vector2(x, glyphInfo10.u2.y));
											uvs.Add(new Vector2(x, glyphInfo10.u0.y));
											num36++;
										}
									}
									if (flag && flag5)
									{
										num19 = (-num3 + glyphInfo10.v0.y) * 0.75f;
										num18 = (-num3 + glyphInfo10.v1.y) * 0.75f;
									}
									else
									{
										num19 = -num3 + glyphInfo10.v0.y;
										num18 = -num3 + glyphInfo10.v1.y;
									}
									if (flag2)
									{
										for (Int32 num38 = 0; num38 < 4; num38++)
										{
											Single num39 = NGUIText.mBoldOffset[num38 * 2];
											Single num40 = NGUIText.mBoldOffset[num38 * 2 + 1];
											verts.Add(new Vector3(num14 + num39, num19 + num40));
											verts.Add(new Vector3(num14 + num39, num18 + num40));
											verts.Add(new Vector3(num2 + num39, num18 + num40));
											verts.Add(new Vector3(num2 + num39, num19 + num40));
										}
									}
									else
									{
										verts.Add(new Vector3(num14, num19));
										verts.Add(new Vector3(num14, num18));
										verts.Add(new Vector3(num2, num18));
										verts.Add(new Vector3(num2, num19));
									}
									if (flag7)
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
										Int32 num45 = (Int32)((!flag2) ? 1 : 4);
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
										Int32 num47 = (Int32)((!flag2) ? 4 : 16);
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
		NGUIText.AddSpecialIconToList(ref specialImages, ref betterList, ref dialogImage, zero, ref num2, num3, num11);
		if (size < verts.size)
		{
			NGUIText.AlignImage(verts, size, num2 - NGUIText.finalSpacingX, specialImages, num11);
			NGUIText.Align(verts, size, num2 - NGUIText.finalSpacingX, 4);
			size = verts.size;
			vertsLineOffsets.Add(size);
			NGUIText.alignment = alignment;
			flag8 = false;
		}
		if (betterList.size > 0 && flag9 && num != 45)
		{
			NGUIText.AlignImageWithLastChar(ref specialImages, betterList, verts, num11);
			betterList.Clear();
		}
		zero = Vector3.zero;
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

	public static readonly String StartSentense;

	public static readonly String DialogId;

	public static readonly String Choose;

	public static readonly String AnimationTime;

	public static readonly String FlashInh;

	public static readonly String NoAnimation;

	public static readonly String NoTypeEffect;

	public static readonly String MessageSpeed;

	public static readonly String Zidane;

	public static readonly String Vivi;

	public static readonly String Dagger;

	public static readonly String Steiner;

	public static readonly String Fraya;

	public static readonly String Quina;

	public static readonly String Eiko;

	public static readonly String Amarant;

	public static readonly String Party1;

	public static readonly String Party2;

	public static readonly String Party3;

	public static readonly String Party4;

	public static readonly String Shadow;

	public static readonly String NoShadow;

	public static readonly String ButtonIcon;

	public static readonly String NoFocus;

	public static readonly String IncreaseSignal;

	public static readonly String CustomButtonIcon;

	public static readonly String NewIcon;

	public static readonly String TextOffset;

	public static readonly String EndSentence;

	public static readonly String TextVar;

	public static readonly String ItemNameVar;

	public static readonly String SignalVar;

	public static readonly String NumberVar;

	public static readonly String MessageDelay;

	public static readonly String MessageFeed;

	public static readonly String MessageTab;

	public static readonly String YAddOffset;

	public static readonly String YSubOffset;

	public static readonly String IconVar;

	public static readonly String PreChoose;

	public static readonly String PreChooseMask;

	public static readonly String DialogAbsPosition;

	public static readonly String DialogOffsetPositon;

	public static readonly String DialogTailPositon;

	public static readonly String TableStart;

	public static readonly String WidthInfo;

	public static readonly String Center;

	public static readonly String Signal;

	public static readonly String NewPage;

	public static readonly String MobileIcon;

	public static readonly String SpacingY;

	public static readonly String KeyboardButtonIcon;

	public static readonly String JoyStickButtonIcon;

	public static readonly String[] RenderOpcodeSymbols;

	public static readonly String[] TextOffsetOpcodeSymbols;

	public static readonly List<Int32> IconIdException;

	public static readonly List<Char> CharException;

	public static readonly List<String> nameKeywordList;

	public static readonly String FF9WhiteColor;

	public static readonly String FF9YellowColor;

	public static readonly String FF9PinkColor;

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

	private static Color mInvisible = new Color(0f, 0f, 0f, 0f);

	private static BetterList<Color> mColors = new BetterList<Color>();

	private static Single mAlpha = 1f;

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
