using System;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;

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
                FF9StateSystem.Settings.SetFastForward(FF9StateSystem.Settings.IsFastForward);
            else if (value == MinigameHUD.Auction || value == MinigameHUD.PandoniumElevator)
                HonoBehaviorSystem.Instance.StopFastForwardMode();
        }
    }

    public static void CheckSpecialHUDFromMesId(Int32 mesId, Boolean open)
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (instance == null)
            return;
        if (instance.gMode == 1)
        {
            Boolean isSpecialHUD = false;
            MinigameHUD hudtype = MinigameHUD.None;
            if (FF9TextTool.FieldZoneId == 2) // Prima Vista
            {
                isSpecialHUD = mesId == 35; // "Guess nobody's here yet..."
                if (isSpecialHUD && !open)
                {
                    EventService.OpenBasicControlTutorial();
                    return;
                }
            }
            else if (FF9TextTool.FieldZoneId == 7) // Cleyra Trunk
            {
                isSpecialHUD = mesId == 113; // "It’s pulling me in! Mash the X button!"
                if (isSpecialHUD)
                    hudtype = MinigameHUD.JumpingRope;
            }
            else if (FF9TextTool.FieldZoneId == 22) // Lindblum Castle (1)
            {
                switch (Localization.GetSymbol())
                {
                    case "JP":
                    case "IT":
                        isSpecialHUD = mesId == 402;
                        break;
                    case "US":
                    case "UK":
                        isSpecialHUD = mesId == 401; // "Use PAD to move the telescope."
                        break;
                    case "FR":
                    case "GR":
                        isSpecialHUD = mesId == 400;
                        break;
                    case "ES":
                        isSpecialHUD = mesId == 395;
                        break;
                }
                if (isSpecialHUD)
                    hudtype = MinigameHUD.Telescope;
            }
            else if (FF9TextTool.FieldZoneId == 23) // Mist Gates
            {
                switch (Localization.GetSymbol())
                {
                    case "JP":
                    case "FR":
                        isSpecialHUD = mesId == 153;
                        break;
                    case "IT":
                        isSpecialHUD = mesId == 148;
                        break;
                    default:
                        isSpecialHUD = mesId == 133; // "How many Potions (50 Gil each) do you want? [...]"
                        break;
                }
                if (isSpecialHUD)
                    hudtype = MinigameHUD.Auction;
            }
            else if (FF9TextTool.FieldZoneId == 33) // Alexandria (1)
            {
                isSpecialHUD = mesId == 233 || mesId == 246;
                if (isSpecialHUD)
                {
                    if (mesId == 233 && !open) // "You wanna try?"
                        open = true;
                    else if (mesId == 246 && open) // "Come play with us again!"
                        open = false;
                }
                if (isSpecialHUD)
                    hudtype = MinigameHUD.JumpingRope;
            }
            else if (FF9TextTool.FieldZoneId == 70 || FF9TextTool.FieldZoneId == 741) // Treno (1) and Treno (2)
            {
                switch (Localization.GetSymbol())
                {
                    case "US":
                    case "UK":
                        isSpecialHUD = mesId == 203; // Bidding dialog "Bid: 0000 Gil [...]"
                        break;
                    default:
                        isSpecialHUD = mesId == 204;
                        break;
                }
                if (isSpecialHUD)
                    hudtype = MinigameHUD.Auction;
            }
            else if (FF9TextTool.FieldZoneId == 71) // Qu's Marsh
            {
                switch (Localization.GetSymbol())
                {
                    case "US":
                    case "UK":
                        isSpecialHUD = mesId == 216; // "Press O to cancel."
                        break;
                    default:
                        isSpecialHUD = mesId == 217;
                        break;
                }
                if (isSpecialHUD)
                    hudtype = MinigameHUD.MogTutorial;
            }
            else if (FF9TextTool.FieldZoneId == 90) // Alexandria (2)
            {
                if (open)
                    isSpecialHUD = mesId == 147 || mesId == 148; // Hippolady "Okay, let's start!"
                else
                    isSpecialHUD = mesId == 148; // "Press Square and Circle alternately!"
                if (isSpecialHUD)
                    hudtype = MinigameHUD.RacingHippaul;
            }
            else if (FF9TextTool.FieldZoneId == 166) // Daguerreo
            {
                isSpecialHUD = mesId == 105; // "Place how many Ore? [...]"
                if (isSpecialHUD)
                    hudtype = MinigameHUD.Auction;
            }
            else if (FF9TextTool.FieldZoneId == 358) // Madain Sari (1)
            {
                switch (Localization.GetSymbol())
                {
                    case "JP":
                    case "FR":
                        isSpecialHUD = mesId == 873;
                        break;
                    case "ES":
                        isSpecialHUD = mesId == 858;
                        break;
                    case "GR":
                        isSpecialHUD = mesId == 874;
                        break;
                    case "IT":
                        isSpecialHUD = mesId == 888;
                        break;
                    default:
                        isSpecialHUD = mesId == 860; // "Um... How many people do I need to account for, kupo? [...]"
                        break;
                }
                if (isSpecialHUD)
                    hudtype = MinigameHUD.Auction;
            }
            else if (FF9TextTool.FieldZoneId == 740) // Desert Palace
            {
                isSpecialHUD = mesId == 249 || mesId == 250;
                if (isSpecialHUD)
                {
                    if (mesId == 249 && !open) // "Get the key! Press the O button to go forward."
                        open = true;
                    else if (mesId == 250 && open) // "You receive the Hourglass Key!"
                        open = false;
                }
                if (isSpecialHUD)
                    hudtype = MinigameHUD.GetTheKey;
            }
            else if (FF9TextTool.FieldZoneId == 945) // Chocobo Places
            {
                isSpecialHUD = mesId == 34 || mesId == 35; // "-How to play Chocobo Hot & Cold- [...]" and "-Choco’s Cries- [...]"
                if (isSpecialHUD)
                {
                    hudtype = MinigameHUD.ChocoHotInstruction;
                    open = true;
                }
                else
                {
                    if (Localization.GetSymbol() == "JP")
                        isSpecialHUD = mesId == 250;
                    else
                        isSpecialHUD = mesId == 251; // "I'll give you one for 50 Gil (Stock: 0) [...]"
                    if (isSpecialHUD)
                        hudtype = MinigameHUD.Auction;
                }
            }
            else if (FF9TextTool.FieldZoneId == 946) // Alexandria (Ruin)
            {
                switch (Localization.GetSymbol())
                {
                    case "US":
                    case "UK":
                        isSpecialHUD = mesId == 250 || mesId == 251 || mesId == 264;
                        if (isSpecialHUD)
                        {
                            if ((mesId == 250 || mesId == 251) && !open) // "Sure, Vivi can play!" or "Sure, Eiko can play!"
                                open = true;
                            else if (mesId == 264 && open) // "Come play with us again!"
                                open = false;
                        }
                        break;
                    default:
                        isSpecialHUD = mesId == 257 || mesId == 258 || mesId == 271;
                        if (isSpecialHUD)
                        {
                            if ((mesId == 257 || mesId == 258) && !open)
                                open = true;
                            else if (mesId == 271 && open)
                                open = false;
                        }
                        break;
                }
                if (isSpecialHUD)
                    hudtype = MinigameHUD.JumpingRope;
            }
            if (isSpecialHUD)
            {
                if (open)
                    EventHUD.OpenSpecialHUD(hudtype);
                else
                    EventHUD.CloseSpecialHUD(hudtype);
            }
        }
    }

    public static void OpenSpecialHUD(MinigameHUD HUDType)
    {
        Boolean startHUD = false;
        switch (HUDType)
        {
            case MinigameHUD.Chanbara:
                startHUD = !UIManager.Field.IsDisplayChanbaraHUD();
                break;
            case MinigameHUD.Auction:
                startHUD = !UIManager.Field.IsDisplayAuctionHUD();
                break;
            case MinigameHUD.MogTutorial:
                startHUD = !UIManager.Field.IsDisplayTutorialHUD();
                break;
            case MinigameHUD.JumpingRope:
                startHUD = !UIManager.Field.IsDisplayJumpRopeHUD();
                break;
            case MinigameHUD.Telescope:
                startHUD = !UIManager.Field.IsDisplayTelescopeHUD();
                break;
            case MinigameHUD.RacingHippaul:
                startHUD = !UIManager.Field.IsDisplayRacingHippaulHUD();
                break;
            case MinigameHUD.SwingACage:
                startHUD = !UIManager.Field.IsDisplaySwingACageHUD();
                break;
            case MinigameHUD.GetTheKey:
                startHUD = !UIManager.Field.IsDisplayGetTheKeyHUD();
                break;
            case MinigameHUD.ChocoHot:
                startHUD = !UIManager.Field.IsDisplayChocoHot();
                break;
            case MinigameHUD.ChocoHotInstruction:
                startHUD = !UIManager.Field.IsDisplayChocoHotInstruction();
                break;
            case MinigameHUD.PandoniumElevator:
                startHUD = !UIManager.Field.IsDisplayPandoniumElevator();
                break;
        }
        if (startHUD)
        {
            EventHUD.CurrentHUD = HUDType;
            UIManager.Field.DisplaySpecialHUD(HUDType);
            PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(HUDType == MinigameHUD.Telescope || HUDType == MinigameHUD.ChocoHot || PersistenSingleton<EventEngine>.Instance.GetUserControl(), null);
        }
        else if (!FF9StateSystem.MobilePlatform)
        {
            EventHUD.CurrentHUD = HUDType;
        }
    }

    public static void CloseSpecialHUD(MinigameHUD HUDType)
    {
        Boolean closeHUD = false;
        switch (HUDType)
        {
            case MinigameHUD.Chanbara:
                closeHUD = UIManager.Field.IsDisplayChanbaraHUD();
                break;
            case MinigameHUD.Auction:
                closeHUD = UIManager.Field.IsDisplayAuctionHUD();
                break;
            case MinigameHUD.MogTutorial:
                closeHUD = UIManager.Field.IsDisplayTutorialHUD();
                break;
            case MinigameHUD.JumpingRope:
                closeHUD = UIManager.Field.IsDisplayJumpRopeHUD();
                break;
            case MinigameHUD.Telescope:
                closeHUD = UIManager.Field.IsDisplayTelescopeHUD();
                break;
            case MinigameHUD.RacingHippaul:
                closeHUD = UIManager.Field.IsDisplayRacingHippaulHUD();
                break;
            case MinigameHUD.SwingACage:
                closeHUD = UIManager.Field.IsDisplaySwingACageHUD();
                break;
            case MinigameHUD.GetTheKey:
                closeHUD = UIManager.Field.IsDisplayGetTheKeyHUD();
                break;
            case MinigameHUD.ChocoHot:
                closeHUD = UIManager.Field.IsDisplayChocoHot();
                break;
            case MinigameHUD.ChocoHotInstruction:
                closeHUD = UIManager.Field.IsDisplayChocoHotInstruction();
                break;
            case MinigameHUD.PandoniumElevator:
                closeHUD = UIManager.Field.IsDisplayPandoniumElevator();
                break;
        }
        if (closeHUD)
        {
            EventHUD.CurrentHUD = MinigameHUD.None;
            UIManager.Field.DestroySpecialHUD();
            PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(HUDType != MinigameHUD.Telescope && HUDType != MinigameHUD.ChocoHot && PersistenSingleton<EventEngine>.Instance.GetUserControl(), null);
        }
        else if (!FF9StateSystem.MobilePlatform)
        {
            EventHUD.CurrentHUD = MinigameHUD.None;
        }
    }

    public static void CheckUIMiniGameForMobile()
    {
        Int32 globDialogProgression = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(6357);
        Int32 genFieldEntrance = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(728);
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
        {
            if (fldMapNo == 64) // A. Castle/Public Seats
            {
                if (genFieldEntrance != 327 && genFieldEntrance != 315 && genFieldEntrance != 316)
                {
                    if (globDialogProgression == 3)
                        EventHUD.OpenSpecialHUD(MinigameHUD.Chanbara);
                    else
                        EventHUD.CloseSpecialHUD(MinigameHUD.Chanbara);
                }
            }
            else if (fldMapNo == 1208) // A. Castle/Dungeon
            {
                if (genFieldEntrance != 0)
                {
                    if (globDialogProgression == 13)
                        EventHUD.OpenSpecialHUD(MinigameHUD.SwingACage);
                    else
                        EventHUD.CloseSpecialHUD(MinigameHUD.SwingACage);
                }
            }
            else if (fldMapNo == 1704) // Mdn. Sari/Eidolon Wall
            {
                if (!PersistenSingleton<EventEngine>.Instance.GetUserControl() && FF9StateSystem.MobilePlatform)
                    PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(!Singleton<DialogManager>.Instance.Visible, null);
            }
            else if (fldMapNo == 2204) // Palace/Odyssey
            {
                if (TimerUI.Time == 0f)
                    EventHUD.CloseSpecialHUD(MinigameHUD.GetTheKey);
            }
            else if (fldMapNo == 2921) // Memoria/To the Origin
            {
                if (!PersistenSingleton<EventEngine>.Instance.GetUserControl() && FF9StateSystem.MobilePlatform)
                    PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(!Singleton<DialogManager>.Instance.Visible, null);
            }
            else if (fldMapNo == 2950 || fldMapNo == 2951 || fldMapNo == 2952) // Chocobo's Forest / Lagoon / Air Garden
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
            else if (fldMapNo == 2711) // Pand./Control Room
            {
                Dialog dialogByWindowID = Singleton<DialogManager>.Instance.GetDialogByWindowID(Convert.ToInt32(Dialog.WindowID.ID7));
                if (dialogByWindowID != null && dialogByWindowID.TextId == 324) // Elevator control: "Current Altitude: X / Current Heading: Y / Standard Heading: [...]"
                    EventHUD.OpenSpecialHUD(MinigameHUD.PandoniumElevator);
                else
                    EventHUD.CloseSpecialHUD(MinigameHUD.PandoniumElevator);
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
