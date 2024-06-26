using System;
using System.Collections.Generic;
using Assets.Sources.Scripts.UI.Common;
using UnityEngine;

public class EventHUD
{
	public static MinigameHUD CurrentHUD
	{
		get
		{
			return EventHUD.currentHUD;
		}
		private set
		{
			EventHUD.currentHUD = value;
			if (value == MinigameHUD.None)
			{
				FF9StateSystem.Settings.SetFastForward(FF9StateSystem.Settings.IsFastForward);
			}
			else if (value == MinigameHUD.Auction || value == MinigameHUD.PandoniumElevator)
			{
				HonoBehaviorSystem.Instance.StopFastForwardMode();
			}
		}
	}

	public static void CheckSpecialHUDFromMesId(Int32 mesId, Boolean open)
	{
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		if (instance == (UnityEngine.Object)null)
		{
			return;
		}
		if (instance.gMode == 1)
		{
			Boolean flag = false;
			MinigameHUD hudtype = MinigameHUD.None;
			if (FF9TextTool.FieldZoneId == 2)
			{
				flag = (mesId == 35);
				if (flag && !open)
				{
					EventService.OpenBasicControlTutorial();
					return;
				}
			}
			else if (FF9TextTool.FieldZoneId == 7)
			{
				flag = (mesId == 113);
				if (flag)
				{
					hudtype = MinigameHUD.JumpingRope;
				}
			}
			else if (FF9TextTool.FieldZoneId == 22)
			{
				String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
				switch (currentLanguage)
				{
				case "Japanese":
				case "Italian":
					flag = (mesId == 402);
					break;
				case "English(US)":
				case "English(UK)":
					flag = (mesId == 401);
					break;
				case "French":
				case "German":
					flag = (mesId == 400);
					break;
				case "Spanish":
					flag = (mesId == 395);
					break;
				}
				if (flag)
				{
					hudtype = MinigameHUD.Telescope;
				}
			}
			else if (FF9TextTool.FieldZoneId == 23)
			{
				String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
			    switch (currentLanguage)
			    {
                    case "Japanese":
                    case "French":
                        flag = (mesId == 153);
                        goto IL_21A;
                    case "Italian":
                        flag = (mesId == 148);
                        goto IL_21A;
                }
                
				flag = (mesId == 133);
				IL_21A:
				if (flag)
				{
					hudtype = MinigameHUD.Auction;
				}
			}
			else if (FF9TextTool.FieldZoneId == 33)
			{
				flag = (mesId == 233);
				if (!flag)
				{
					flag = (mesId == 246);
				}
				if (flag)
				{
					if (mesId == 233 && !open)
					{
						open = true;
					}
					else if (mesId == 246 && open)
					{
						open = false;
					}
				}
				if (flag)
				{
					hudtype = MinigameHUD.JumpingRope;
				}
			}
			else if (FF9TextTool.FieldZoneId == 70 || FF9TextTool.FieldZoneId == 741)
			{
				String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
			    switch (currentLanguage)
			    {
                    case "English(US)":
                    case "English(UK)":
                        flag = (mesId == 203);
                        goto IL_324;
                }
			    
				flag = (mesId == 204);
				IL_324:
				if (flag)
				{
					hudtype = MinigameHUD.Auction;
				}
			}
			else if (FF9TextTool.FieldZoneId == 71)
			{
				String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
                switch (currentLanguage)
                {
                    case "English(US)":
                    case "English(UK)":
                        flag = (mesId == 216);
                        goto IL_3BB;
                }
                
				flag = (mesId == 217);
				IL_3BB:
				if (flag)
				{
					hudtype = MinigameHUD.MogTutorial;
				}
			}
			else if (FF9TextTool.FieldZoneId == 90)
			{
				if (open)
				{
					flag = (mesId == 147 || mesId == 148);
				}
				else
				{
					flag = (mesId == 148);
				}
				if (flag)
				{
					hudtype = MinigameHUD.RacingHippaul;
				}
			}
			else if (FF9TextTool.FieldZoneId == 166)
			{
				flag = (mesId == 105);
				if (flag)
				{
					hudtype = MinigameHUD.Auction;
				}
			}
			else if (FF9TextTool.FieldZoneId == 358)
			{
				String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
				switch (currentLanguage)
				{
				case "Japanese":
				case "French":
					flag = (mesId == 873);
					goto IL_51C;
				case "Spanish":
					flag = (mesId == 858);
					goto IL_51C;
				case "German":
					flag = (mesId == 874);
					goto IL_51C;
				case "Italian":
					flag = (mesId == 888);
					goto IL_51C;
				}
				flag = (mesId == 860);
				IL_51C:
				if (flag)
				{
					hudtype = MinigameHUD.Auction;
				}
			}
			else if (FF9TextTool.FieldZoneId == 740)
			{
				flag = (mesId == 249);
				if (!flag)
				{
					flag = (mesId == 250);
				}
				if (flag)
				{
					if (mesId == 249 && !open)
					{
						open = true;
					}
					else if (mesId == 250 && open)
					{
						open = false;
					}
				}
				if (flag)
				{
					hudtype = MinigameHUD.GetTheKey;
				}
			}
			else if (FF9TextTool.FieldZoneId == 945)
			{
				flag = (mesId == 34);
				if (!flag)
				{
					flag = (mesId == 35);
				}
				if (flag)
				{
					hudtype = MinigameHUD.ChocoHotInstruction;
					if (!open)
					{
						open = true;
					}
				}
				else
				{
					String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
				    if (currentLanguage == "Japanese")
				    {
				        flag = (mesId == 250);
				        goto IL_639;
				    }
                    
					flag = (mesId == 251);
					IL_639:
					if (flag)
					{
						hudtype = MinigameHUD.Auction;
					}
				}
			}
			else if (FF9TextTool.FieldZoneId == 946)
			{
				String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
                switch (currentLanguage)
                {
                    case "English(US)":
                    case "English(UK)":
                        flag = (mesId == 250 || mesId == 251);
                        if (!flag)
                        {
                            flag = (mesId == 264);
                        }
                        if (flag)
                        {
                            if ((mesId == 250 || mesId == 251) && !open)
                            {
                                open = true;
                            }
                            else if (mesId == 264 && open)
                            {
                                open = false;
                            }
                        }
                        goto IL_789;
                }
                
				flag = (mesId == 257 || mesId == 258);
				if (!flag)
				{
					flag = (mesId == 271);
				}
				if (flag)
				{
					if ((mesId == 257 || mesId == 258) && !open)
					{
						open = true;
					}
					else if (mesId == 271 && open)
					{
						open = false;
					}
				}
				IL_789:
				if (flag)
				{
					hudtype = MinigameHUD.JumpingRope;
				}
			}
			if (flag)
			{
				if (open)
				{
					EventHUD.OpenSpecialHUD(hudtype);
				}
				else
				{
					EventHUD.CloseSpecialHUD(hudtype);
				}
			}
		}
	}

	public static void OpenSpecialHUD(MinigameHUD HUDType)
	{
		Boolean flag = false;
		switch (HUDType)
		{
		case MinigameHUD.Chanbara:
			flag = !UIManager.Field.IsDisplayChanbaraHUD();
			break;
		case MinigameHUD.Auction:
			flag = !UIManager.Field.IsDisplayAuctionHUD();
			break;
		case MinigameHUD.MogTutorial:
			flag = !UIManager.Field.IsDisplayTutorialHUD();
			break;
		case MinigameHUD.JumpingRope:
			flag = !UIManager.Field.IsDisplayJumpRopeHUD();
			break;
		case MinigameHUD.Telescope:
			flag = !UIManager.Field.IsDisplayTelescopeHUD();
			break;
		case MinigameHUD.RacingHippaul:
			flag = !UIManager.Field.IsDisplayRacingHippaulHUD();
			break;
		case MinigameHUD.SwingACage:
			flag = !UIManager.Field.IsDisplaySwingACageHUD();
			break;
		case MinigameHUD.GetTheKey:
			flag = !UIManager.Field.IsDisplayGetTheKeyHUD();
			break;
		case MinigameHUD.ChocoHot:
			flag = !UIManager.Field.IsDisplayChocoHot();
			break;
		case MinigameHUD.ChocoHotInstruction:
			flag = !UIManager.Field.IsDisplayChocoHotInstruction();
			break;
		case MinigameHUD.PandoniumElevator:
			flag = !UIManager.Field.IsDisplayPandoniumElevator();
			break;
		}
		Boolean isEnable = HUDType == MinigameHUD.Telescope || HUDType == MinigameHUD.ChocoHot || PersistenSingleton<EventEngine>.Instance.GetUserControl();
		if (flag)
		{
			EventHUD.CurrentHUD = HUDType;
			UIManager.Field.DisplaySpecialHUD(HUDType);
			PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(isEnable, (Action)null);
		}
		else if (!FF9StateSystem.MobilePlatform)
		{
			EventHUD.CurrentHUD = HUDType;
		}
	}

	public static void CloseSpecialHUD(MinigameHUD HUDType)
	{
		Boolean flag = false;
		switch (HUDType)
		{
		case MinigameHUD.Chanbara:
			flag = UIManager.Field.IsDisplayChanbaraHUD();
			break;
		case MinigameHUD.Auction:
			flag = UIManager.Field.IsDisplayAuctionHUD();
			break;
		case MinigameHUD.MogTutorial:
			flag = UIManager.Field.IsDisplayTutorialHUD();
			break;
		case MinigameHUD.JumpingRope:
			flag = UIManager.Field.IsDisplayJumpRopeHUD();
			break;
		case MinigameHUD.Telescope:
			flag = UIManager.Field.IsDisplayTelescopeHUD();
			break;
		case MinigameHUD.RacingHippaul:
			flag = UIManager.Field.IsDisplayRacingHippaulHUD();
			break;
		case MinigameHUD.SwingACage:
			flag = UIManager.Field.IsDisplaySwingACageHUD();
			break;
		case MinigameHUD.GetTheKey:
			flag = UIManager.Field.IsDisplayGetTheKeyHUD();
			break;
		case MinigameHUD.ChocoHot:
			flag = UIManager.Field.IsDisplayChocoHot();
			break;
		case MinigameHUD.ChocoHotInstruction:
			flag = UIManager.Field.IsDisplayChocoHotInstruction();
			break;
		case MinigameHUD.PandoniumElevator:
			flag = UIManager.Field.IsDisplayPandoniumElevator();
			break;
		}
		Boolean isEnable = HUDType != MinigameHUD.Telescope && HUDType != MinigameHUD.ChocoHot && PersistenSingleton<EventEngine>.Instance.GetUserControl();
		if (flag)
		{
			EventHUD.CurrentHUD = MinigameHUD.None;
			UIManager.Field.DestroySpecialHUD();
			PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(isEnable, (Action)null);
		}
		else if (!FF9StateSystem.MobilePlatform)
		{
			EventHUD.CurrentHUD = MinigameHUD.None;
		}
	}

	public static void CheckUIMiniGameForMobile()
	{
		Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(6357);
		Int32 varManually2 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(728);
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
		{
			if (fldMapNo == 64)
			{
				if (varManually2 != 327 && varManually2 != 315 && varManually2 != 316)
				{
					if (varManually == 3)
					{
						EventHUD.OpenSpecialHUD(MinigameHUD.Chanbara);
					}
					else
					{
						EventHUD.CloseSpecialHUD(MinigameHUD.Chanbara);
					}
				}
			}
			else if (fldMapNo == 1208)
			{
				if (varManually2 != 0)
				{
					if (varManually == 13)
					{
						EventHUD.OpenSpecialHUD(MinigameHUD.SwingACage);
					}
					else
					{
						EventHUD.CloseSpecialHUD(MinigameHUD.SwingACage);
					}
				}
			}
			else if (fldMapNo == 1704)
			{
				if (!PersistenSingleton<EventEngine>.Instance.GetUserControl() && FF9StateSystem.MobilePlatform)
				{
					Boolean isEnable = !Singleton<DialogManager>.Instance.Visible;
					PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(isEnable, (Action)null);
				}
			}
			else if (fldMapNo == 2204)
			{
				if (TimerUI.Time == 0f)
				{
					EventHUD.CloseSpecialHUD(MinigameHUD.GetTheKey);
				}
			}
			else if (fldMapNo == 2921)
			{
				if (!PersistenSingleton<EventEngine>.Instance.GetUserControl() && FF9StateSystem.MobilePlatform)
				{
					Boolean isEnable2 = !Singleton<DialogManager>.Instance.Visible;
					PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(isEnable2, (Action)null);
				}
			}
			else if (fldMapNo == 2950 || fldMapNo == 2951 || fldMapNo == 2952)
			{
				if (TimerUI.Play && TimerUI.Time > 0f)
				{
					EventHUD.CloseSpecialHUD(MinigameHUD.ChocoHotInstruction);
					EventHUD.OpenSpecialHUD(MinigameHUD.ChocoHot);
				}
				else if (TimerUI.Enable)
				{
					EventHUD.CloseSpecialHUD(MinigameHUD.ChocoHot);
				}
			}
			else if (fldMapNo == 2711)
			{
				Dialog dialogByWindowID = Singleton<DialogManager>.Instance.GetDialogByWindowID(Convert.ToInt32(Dialog.WindowID.ID7));
				Boolean flag = false;
				if (dialogByWindowID != (UnityEngine.Object)null)
				{
					flag = (dialogByWindowID.TextId == 324);
				}
				if (flag)
				{
					EventHUD.OpenSpecialHUD(MinigameHUD.PandoniumElevator);
				}
				else
				{
					EventHUD.CloseSpecialHUD(MinigameHUD.PandoniumElevator);
				}
			}
		}
	}

	public static void Cleanup()
	{
		UIManager.Field.DestroySpecialHUD();
		EventHUD.currentHUD = MinigameHUD.None;
	}

	private static MinigameHUD currentHUD;
}
