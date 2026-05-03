using Assets.Scripts.Common;
using Assets.SiliconSocial;
using Memoria;
using System;
using System.Collections.Generic;

public class EMinigame
{
    public static void ChanbaraBonusPoints(Obj s1, EBin eBin)
    {
        // This mini-game's script can be read there: https://www.dropbox.com/scl/fi/7iiziktw78ijuc4wgnobz/Script_BlankMinigame.txt?rlkey=e5jx7739rz6c6dwlh2h0wgu24&st=twind9av&dl=0
        if (FF9StateSystem.Common.FF9.fldMapNo == 64) // A. Castle/Public Seats
        {
            if (s1.sid == 4 && s1.ip == 223)
            {
                // Somewhere in a "Code4" function (only "Code4_Loop" is long enough)
                Int32 score = eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Int16, 48)); // VAR_GlobInt16_48, aka. TotalScore
                if (Configuration.Hacks.SwordplayAssistance >= 1)
                    score += score / 10 * 3; // +30% bonus granted by the Steam version
                EMinigame.GetEncoreChanbaraAchievement(score);
                eBin.setVarManually(EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Int16, 48), score);
            }
            else if (Configuration.Hacks.SwordplayAssistance >= 2 && s1.sid == 4)
            {
                // Anywhere in any "Code4" function
                // 0x3409 = VAR_GlobUInt8_52, aka. TimeLeft
                // 0x22D9 = VAR_GlobInt16_34, aka. HitCount
                if (eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Byte, 52)) > 0 && eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Int16, 34)) < 50)
                    eBin.setVarManually(EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Byte, 52), 50);
            }
        }
    }

    public static void GetEncoreChanbaraAchievement(Int32 score)
    {
        if (score >= 75)
            AchievementManager.ReportAchievement(AcheivementKey.Encore, 1);
    }

    public static Int32 StiltzkinBuy
    {
        get => EMinigame.stiltzkinBuy;
        set => EMinigame.stiltzkinBuy = value;
    }

    public static void StiltzkinAchievement(PosObj gCur, UInt32 gilDecrease)
    {
        if (gCur.model == EMinigame.StiltzkinMogModelId)
        {
            if (gilDecrease == 333 || gilDecrease == 444 || gilDecrease == 555 || gilDecrease == 666 || gilDecrease == 777 || gilDecrease == 888 || gilDecrease == 2222 || gilDecrease == 5555)
                FF9StateSystem.Achievement.StiltzkinBuy++;
            AchievementManager.ReportAchievement(AcheivementKey.AllStiltzkinItem, FF9StateSystem.Achievement.StiltzkinBuy);
        }
    }

    public static void EidolonMuralAchievement(String langSymbol, Int32 messageId)
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if (fldMapNo == 1704) // Madain Sari/Eidolon Wall
        {
            Boolean nameRead;
            switch (langSymbol)
            {
                case "FR":
                case "IT":
                    nameRead = messageId == 119;
                    break;
                case "GR":
                    nameRead = messageId == 120;
                    break;
                case "JP":
                    nameRead = messageId == 122;
                    break;
                default:
                    nameRead = messageId == 118; // Message giving Garnet's birthname and her mother's name
                    break;
            }
            if (nameRead)
                AchievementManager.ReportAchievement(AcheivementKey.EidolonMural, 1);
        }
    }

    public static void ExcellentLuckColorFortuneTellingAchievement(String langSymbol, Int32 messageId)
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if (fldMapNo == 352) // Dali/Inn
        {
            Boolean bestLuck;
            switch (langSymbol)
            {
                case "UK":
                case "US":
                    bestLuck = messageId == 222; // "Very Good Omen [...]"
                    break;
                case "ES":
                    bestLuck = messageId == 230;
                    break;
                default:
                    bestLuck = messageId == 233;
                    break;
            }
            if (bestLuck)
                AchievementManager.ReportAchievement(AcheivementKey.ExcellentLuck, 1);
        }
    }

    public static void JumpingRopeAchievement(String langSymbol, Int32 mesId)
    {
        Int32 varOperation = 0;
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        Int32 scenarioCounter = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
        if (fldMapNo == 103) // Early game
            varOperation = EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Int24, 43);
        if (fldMapNo == 2456 && scenarioCounter >= 10300) // Late game
            varOperation = EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Int24, 59);
        if (varOperation == 0)
            return;
        Int32 jumpCount = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(varOperation);
        if (FF9StateSystem.Settings.IsFastTrophyMode)
        {
            if (jumpCount == 5)
                AchievementManager.ReportAchievement(AcheivementKey.Rope100, 1);
            if (jumpCount == 10)
                AchievementManager.ReportAchievement(AcheivementKey.Rope1000, 1);
            return;
        }
        Boolean success = EMinigame.IsCongratMessage(langSymbol, mesId, fldMapNo);
        if (jumpCount == 100 && success)
            AchievementManager.ReportAchievement(AcheivementKey.Rope100, 1);
        if (jumpCount >= 1000 && success)
            AchievementManager.ReportAchievement(AcheivementKey.Rope1000, 1);
    }

    private static Boolean IsCongratMessage(String langSymbol, Int32 mesId, Int32 fieldId)
    {
        if (fieldId == 103) // Alexandria/Square
            return mesId == 241 || mesId == 69;
        if (fieldId == 2456 && !String.IsNullOrEmpty(langSymbol)) // Alexandria/Steeple
        {
            switch (langSymbol)
            {
                case "FR":
                case "ES":
                case "GR":
                case "IT":
                case "JP":
                    return mesId == 266 || mesId == 69;
                default:
                    return mesId == 259 || mesId == 69;
            }
        }
        return false;
    }

    public static void ProvokeMogAchievement(String langSymbol, Int32 messageId)
    {
        if (PersistenSingleton<SceneDirector>.Instance.CurrentScene != SceneDirector.WorldMapSceneName)
            return;
        if (messageId == 49)
            AchievementManager.ReportAchievement(AcheivementKey.ProvokeMoogle, 1);
    }

    public static void Catching99FrogAchievement(Int32 numFrog)
    {
        AchievementManager.ReportAchievement(AcheivementKey.Frog99, numFrog);
    }

    public static void CatchingGoldenFrogAchievement(Obj frogObj)
    {
        if (((PosObj)frogObj).model == EMinigame.GoldenFrogModelId)
            AchievementManager.ReportAchievement(AcheivementKey.GoldenFrog, 1);
    }

    public static void GetRewardFromQueenStellaAchievement()
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if (fldMapNo == 911 || fldMapNo == 1911) // Treno/Queen's House
        {
            Int32 stellazzioBitFlags = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.UInt16, 355));
            Int32 stellazzioCount = 0;
            for (Int32 i = 0; i < 13; i++)
            {
                stellazzioCount += stellazzioBitFlags & 1;
                stellazzioBitFlags >>= 1;
            }
            AchievementManager.ReportAchievement(AcheivementKey.QueenReward10, stellazzioCount);
        }
    }

    public static void GetTheAirShipAchievement()
    {
        if (PersistenSingleton<EventEngine>.Instance.gMode == 3)
        {
            Int32 scenarioCounter = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
            Int32 mapIndex = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
            if (scenarioCounter == 10400 && mapIndex == 60)
                AchievementManager.ReportAchievement(AcheivementKey.Airship, 1);
        }
    }

    public static void TreasureHunterSAchievement(String langSymbol, Int32 mesId)
    {
        // Treno/Pub or Daguerreo/Right Hall
        if ((FF9StateSystem.Common.FF9.fldMapNo == 1900 && mesId == 289) || (FF9StateSystem.Common.FF9.fldMapNo == 2801 && mesId == 236))
            AchievementManager.ReportAchievement(AcheivementKey.TreasureHuntS, 1);
    }

    public static void ShuffleGameAchievement(String langSymbol, Int32 mesId)
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if (fldMapNo == 1858) // Alexandria/Weapon Shop
        {
            Boolean isCongratulation;
            switch (langSymbol)
            {
                case "US":
                case "UK":
                    isCongratulation = mesId == 265; // Zenero "Ahh! You're too good!"
                    break;
                default:
                    isCongratulation = mesId == 264;
                    break;
            }
            if (isCongratulation)
                AchievementManager.ReportAchievement(AcheivementKey.Shuffle9, 1);
        }
    }

    public static void GetTheaterShipMaquetteAchievement()
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        Int32 scenarioCounter = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
        if ((fldMapNo == 457 || (fldMapNo == 455 && scenarioCounter < 11090)) && EMinigame.g_tship_model_get_flg == 0)
        {
            // Mountain/Shack or Mountain/Base
            EMinigame.g_tship_model_get_flg = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 2419));
            if (EMinigame.g_tship_model_get_flg == 1)
                AchievementManager.ReportAchievement(AcheivementKey.ShipMaquette, 1);
        }
    }

    public static void GetHelpAllVictimsInCleyraTown()
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        Int32 scenarioCounter = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
        if (fldMapNo == 1109 && scenarioCounter == 4980) // Cleyra/Cathedral
        {
            Int32 motherHelped1 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 3881));
            Int32 motherHelped2 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 3882));
            Int32 priestessHelped = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 3883));
            if (motherHelped1 == 1 && motherHelped2 == 1 && priestessHelped == 1)
                AchievementManager.ReportAchievement(AcheivementKey.CleyraVictimAll, 1);
        }
    }

    public static void GetPlayBlackjackAchievement()
    {
        AchievementManager.ReportAchievement(AcheivementKey.Blackjack, 1);
    }

    public static void ChocoboBeakLV99Achievement(String langSymbol, Int32 mesId)
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if (fldMapNo == 2950 || fldMapNo == 2951 || fldMapNo == 2952) // Chocobo's Forest, Lagoon or Air Garden
        {
            Boolean checkBeakUpdate;
            switch (langSymbol)
            {
                case "JP":
                    checkBeakUpdate = mesId == 325 || mesId == 277;
                    break;
                case "US":
                case "UK":
                    checkBeakUpdate = mesId == 320 || mesId == 277; // "Choco's beak became stronger!" or "Time's Up"
                    break;
                default:
                    checkBeakUpdate = mesId == 326 || mesId == 278;
                    break;
            }
            if (checkBeakUpdate)
            {
                Int32 beakLevel = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Byte, 139));
                AchievementManager.ReportAchievement(AcheivementKey.ChocoboLv99, beakLevel);
            }
        }
    }

    public static void Auction10TimesAchievement(Obj gCur)
    {
        if (gCur == null)
            return;
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if ((fldMapNo == 909 && gCur.sid == 7) || (fldMapNo == 1909 && gCur.sid == 3)) // Treno/Auction Site
            AchievementManager.ReportAchievement(AcheivementKey.Auction10, ++FF9StateSystem.Achievement.AuctionTime);
    }

    public static void ViviWinHuntAchievement()
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if (fldMapNo == 600) // Lindblum Castle/Royal Chamber
        {
            Int32 huntWinner = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Byte, 313));
            if (huntWinner == 2)
                AchievementManager.ReportAchievement(AcheivementKey.ViviWinHunt, 1);
        }
    }

    public static void GetWinQuadmistAchievement()
    {
        AchievementManager.ReportAchievement(AcheivementKey.CardWin1, FF9StateSystem.Achievement.QuadmistWinList.Count);
        AchievementManager.ReportAchievement(AcheivementKey.CardWin10, FF9StateSystem.Achievement.QuadmistWinList.Count);
        AchievementManager.ReportAchievement(AcheivementKey.CardWin100, FF9StateSystem.Achievement.QuadmistWinList.Count);
    }

    public static Int32 CreateNPCID(Int32 fldNo, Int32 uid)
    {
        return fldNo * 1000 + uid;
    }

    public static void SetQuadmistOpponentId(Obj gCur)
    {
        EMinigame.quadmistOpponentId = EMinigame.CreateNPCID(FF9StateSystem.Common.FF9.fldMapNo, gCur.uid);
        EMinigame.SetGroupingOpponentId();
    }

    private static void SetGroupingOpponentId()
    {
        Int32 opponentId = EMinigame.quadmistOpponentId;
        if (opponentId == 558002 || opponentId == 1306002 || opponentId == 2106002)
        {
            EMinigame.quadmistOpponentId = 558002;
        }
        else if (opponentId == 908009 || opponentId == 1908006)
        {
            Int32 coinThrowed = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Byte, 369));
            EMinigame.quadmistOpponentId = 908009;
            if (coinThrowed >= EMinigame.ThiefNPCCoinCondition)
                EMinigame.quadmistOpponentId += 255;
        }
    }

    public static void SetQuadmistStadiumOpponentId(Obj gCur, Int32 minigameFlag)
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if ((fldMapNo == 903 && gCur.sid == 11) || (fldMapNo == 1903 && gCur.sid == 15)) // Treno/Card Stadium
            EMinigame.quadmistOpponentId = fldMapNo * 1000 + minigameFlag + 100;
        else if (fldMapNo == 112 && gCur.sid == 7) // Alexandria/Pub
            EMinigame.SetQuadmistOpponentId(gCur);
    }

    public static void SetThiefId(Obj gCur)
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if (fldMapNo == 908 || fldMapNo == 1908) // Treno/Gate
        {
            Int32 opponentId = EMinigame.CreateNPCID(fldMapNo, gCur.uid);
            if (opponentId == 908009 || opponentId == 1908006)
                EMinigame.SetGroupingOpponentId();
        }
    }

    public static void SetFatChocoboId(Obj gCur)
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if (fldMapNo == 2955 && gCur.uid == 9) // Chocobo's Paradise
            EMinigame.SetQuadmistOpponentId(gCur);
    }

    public static void QuadmistWinAllNPCAchievement()
    {
        if (EMinigame.quadmistOpponentId <= 0)
            return;
        if (FF9StateSystem.Achievement.QuadmistWinList.Add(EMinigame.quadmistOpponentId))
            AchievementManager.ReportAchievement(AcheivementKey.CardWinAll, FF9StateSystem.Achievement.QuadmistWinList.Count);
        EMinigame.quadmistOpponentId = 0;
    }

    public static void DigUpMadainRingAchievement()
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if (fldMapNo == 1421) // Fossil Roo/Mining Site
            AchievementManager.ReportAchievement(AcheivementKey.MadainRing, 1);
    }

    public static void DigUpMadianRingCheating()
    {
        EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
        if (FF9StateSystem.Common.FF9.fldMapNo == 1421 && EBin.s1.sid == 0 && EBin.s1.ip == 813)  // Fossil Roo/Mining Site
        {
            eBin.setVarManually(EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Byte, 47), 2);
            eBin.setVarManually(EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Int16, 56), 203);
        }
    }

    public static void MognetCentralAchievement()
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        if (fldMapNo == 3100) // Mognet Central
            AchievementManager.ReportAchievement(AcheivementKey.MognetCentral, 1);
    }

    public static void AtleteQueenAchievement_ByMessage(String langSymbol, Int32 mesId)
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 1850 // Alexandria/Main Street
         && mesId == 152 // "Vivi wins!"
         && FF9StateSystem.Settings.IsFastTrophyMode
         && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Byte, 491)) < 80)
            PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Byte, 491), 80);
    }

    public static void AtleteQueenAchievement_ByReward()
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 1850) // Alexandria/Main Street
            AchievementManager.ReportAchievement(AcheivementKey.AthleteQueen, 1);
    }

    public static void SuperSlickOilAchievement()
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 2457) // Alexandria/Mini-Theater
            AchievementManager.ReportAchievement(AcheivementKey.SuperSlickOil, 1);
    }

    public static void SuperSlickOilCheating()
    {
        EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
        eBin.setVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 8495), 1);
        eBin.setVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 7627), 0);
    }

    public static void AllTreasureAchievement()
    {
        if (PersistenSingleton<SceneDirector>.Instance.CurrentScene != SceneDirector.WorldMapSceneName)
            return;
        EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
        Int32 chocographFlags = eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Int24, 184));
        chocographFlags &= 0xFFFFFF;
        if (EMinigame.lastGWldItemGet0 != chocographFlags && chocographFlags == 0xFFFFFF)
            AchievementManager.ReportAchievement(AcheivementKey.AllTreasure, EMinigame.numOfTreasures);
        EMinigame.lastGWldItemGet0 = chocographFlags;
    }

    public static void InitializeAllTreasureAchievement()
    {
        EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
        EMinigame.lastGWldItemGet0 = eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Int24, 184));
        EMinigame.lastGWldItemGet0 &= 0xFFFFFF;
    }

    public static void AllTreasureCheating()
    {
        EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
        eBin.setVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Int24, 184), 0xFFFFFE);
        eBin.setVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Int24, 187), 0xFFFFFF);
        FF9StateSystem.EventState.gEventGlobal[191] = 5;
    }

    public static void AllSandyBeachAchievement()
    {
        if (PersistenSingleton<SceneDirector>.Instance.CurrentScene != SceneDirector.WorldMapSceneName)
            return;
        Int32 beachCount = EMinigame.CountVisitedSandyBeach();
        if (EMinigame.lastNumOfVisitedSandyBeach != beachCount)
        {
            AchievementManager.ReportAchievement(AcheivementKey.AllSandyBeach, beachCount);
            EMinigame.lastNumOfVisitedSandyBeach = beachCount;
        }
    }

    public static void InitializeAllSandyBeachAchievement()
    {
        EMinigame.lastNumOfVisitedSandyBeach = EMinigame.CountVisitedSandyBeach();
    }

    private static Int32 CountVisitedSandyBeach()
    {
        EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
        Int32 beachCount = 0;
        for (Int32 i = 0; i < EMinigame.numOfSandyBeach; i++)
        {
            Int32 beachVisited = eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 856 + i));
            if (beachVisited == 1)
                beachCount++;
        }
        return beachCount;
    }

    public static void AllSandyBeachCheating()
    {
        EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
        eBin.setVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 1042), 1);
        eBin.setVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Int16, 107), 0xFFFE);
        eBin.setVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Int16, 109), 31);
        FF9StateSystem.EventState.gEventGlobal[191] = 5;
    }

    public static void DigUpKupoAchievement()
    {
        EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
        Int32 kupoFound = eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 3255));
        if (FF9StateSystem.Common.FF9.fldMapNo == 1421 && EMinigame.lastGFFossilMog != kupoFound && kupoFound == 1) // Fossil Roo/Mining Site
        {
            AchievementManager.ReportAchievement(AcheivementKey.Kuppo, 1);
            EMinigame.lastGFFossilMog = kupoFound;
        }
    }

    public static void InitializeDigUpKupoAchievement()
    {
        EMinigame.lastGFFossilMog = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 3255));
    }

    public static void ATE80Achievement(Int32 ateID)
    {
        if (ateID == -1)
            return;
        EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
        Int32[] ateCheck = FF9StateSystem.Achievement.AteCheck;
        Debug.Log($"{AcheivementKey.ATE80} : ateCheck[{ateID}] = {ateCheck[ateID]}");
        if (ateCheck[ateID] == 0)
            ateCheck[ateID] = 1;
        Int32 ateCount = 0;
        for (Int32 i = 0; i < 83; i++)
            if (ateCheck[i] == 1 && i != 6 && i != 7 && i != 14)
                ateCount++;
        Debug.Log(AcheivementKey.ATE80 + " : numOfSeenATE = " + ateCount);
        AchievementManager.ReportAchievement(AcheivementKey.ATE80, ateCount);
    }

    public static Int32 MappingATEID(Dialog dialog, Int32 selectedChoice, Boolean isCompulsory)
    {
        Int32 result = -1;
        if (FF9StateSystem.Common.FF9.fldLocNo == 40)
        {
            if (FF9StateSystem.Common.FF9.fldMapNo == 206 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 1900 && selectedChoice == 0)
                result = 0; // Prima Vista/Crash Site
            else
                result = 1 + selectedChoice;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 4)
        {
            if (selectedChoice == 4 && !isCompulsory)
                result = 4;
            else if ((FF9StateSystem.Common.FF9.fldMapNo == 253 || FF9StateSystem.Common.FF9.fldMapNo == 204) && isCompulsory)
                result = 5;
            else if (FF9StateSystem.Common.FF9.fldMapNo == 262 && isCompulsory)
                result = 6;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 8)
        {
            if (FF9StateSystem.Common.FF9.fldMapNo == 306 && isCompulsory)
                result = 7;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 47 || FF9StateSystem.Common.FF9.fldLocNo == 53)
        {
            if (isCompulsory)
                result = 8;
            else
                result = 8 + selectedChoice;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 276)
        {
            if (FF9StateSystem.Common.FF9.fldMapNo == 554 && isCompulsory)
                result = 14;
            else if (FF9StateSystem.Common.FF9.fldMapNo == 552 && isCompulsory)
                result = 16;
            else if (FF9StateSystem.Common.FF9.fldMapNo == 565 && isCompulsory)
                result = 18;
            else
                result = selectedChoice == 3 ? 19 : 15 + selectedChoice;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 70)
        {
            if (isCompulsory)
                result = 24;
            else
                result = 20 + selectedChoice;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 44)
        {
            result = 27 + selectedChoice;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 289)
        {
            result = 31;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 485)
        {
            if (!isCompulsory)
                result = 32;
            else if (FF9StateSystem.Common.FF9.fldMapNo == 1307)
                result = 33;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 525)
        {
            if (isCompulsory && FF9StateSystem.Common.FF9.fldMapNo == 1353)
                result = 34;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 32)
        {
            if (isCompulsory)
                result = 40;
            else
                result = 35 + selectedChoice;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 37)
        {
            if (isCompulsory)
                result = 42;
            else
                result = 41 + selectedChoice;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 358)
        {
            if (isCompulsory)
                result = 49;
            else
                result = 47 + selectedChoice;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 88 || FF9StateSystem.Common.FF9.fldLocNo == 90)
        {
            result = 52 + selectedChoice;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 359)
        {
            if (FF9StateSystem.Common.FF9.fldMapNo == 956 && isCompulsory)
                result = 57;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 741)
        {
            result = 58 + selectedChoice;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 595 || FF9StateSystem.Common.FF9.fldLocNo == 943)
        {
            if (isCompulsory)
            {
                if (FF9StateSystem.Common.FF9.fldMapNo == 2169)
                {
                    Int32 scenarioCounter = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
                    if (scenarioCounter == 9100)
                        result = 70;
                    else if (scenarioCounter == 9200)
                        result = 71;
                    else if (scenarioCounter == 10010)
                        result = 76;
                    else if (scenarioCounter == 10030)
                        result = 77;
                }
                else if (FF9StateSystem.Common.FF9.fldMapNo == 2113)
                {
                    result = 73;
                }
                else if (FF9StateSystem.Common.FF9.fldMapNo == 2173)
                {
                    result = 75;
                }
            }
            else
            {
                result = 68 + selectedChoice;
            }
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 52)
        {
            if (isCompulsory)
                result = 81;
            else
                result = 78 + selectedChoice;
        }
        else if (FF9StateSystem.Common.FF9.fldLocNo == 344)
        {
            result = 82;
        }
        return result;
    }

    public static Boolean CheckChocoboVirtual()
    {
        if (FF9StateSystem.MobilePlatform)
        {
            if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
            {
                if (EventHUD.CurrentHUD == MinigameHUD.ChocoHot)
                    return true;
            }
            else if (PersistenSingleton<EventEngine>.Instance.gMode == 3)
            {
                if (EventCollision.IsRidingChocobo() && EventInput.IsPressedDig)
                    return true;
                EventInput.IsPressedDig = false;
            }
        }
        return false;
    }

    public static void SetViviSpeed(Obj s1, EBin eBin)
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 1850 && s1.sid == 15) // Alexandria/Main Street
        {
            Int32 varOp = EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Byte, 36);
            if (s1.ip == 1445 || s1.ip == 1497)
            {
                Int32 speed = eBin.getVarManually(varOp);
                eBin.setVarManually(varOp, Configuration.Hacks.HippaulRacingViviSpeed);
            }
            else if (s1.ip == 1595)
            {
                Int32 speed = eBin.getVarManually(varOp);
                if (speed != 0 && speed < 5)
                {
                    speed = 5;
                    eBin.setVarManually(varOp, speed);
                }
            }
        }
    }

    public static Boolean CheckBeachMinigame()
    {
        Boolean result = false;
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (instance.gMode == 3 && WMUIData.ControlNo == 0)
        {
            Int32 varManually = instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 1042));
            if (varManually > 0)
            {
                Int32 areaId = ff9.w_frameGetParameter(192);
                Int32 count = EMinigame.BeachData.Count;
                Int32 curBeachOp = 0;
                Int32 beachFlags = 0;
                for (Int32 i = 0; i < count; i++)
                {
                    Int32 beachOp = EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 856 + i);
                    if (areaId == EMinigame.BeachData[i])
                        curBeachOp = beachOp;
                    if (instance.eBin.getVarManually(beachOp) > 0)
                        beachFlags++;
                    if (i < count - 1)
                        beachFlags <<= 1;
                }
                if (curBeachOp != 0)
                {
                    Int32 wasVisited = instance.eBin.getVarManually(curBeachOp);
                    result = wasVisited == 0 || (wasVisited == 1 && beachFlags == 0x1FFFFF);
                }
            }
        }
        return result;
    }

    private const Boolean showLog = false;
    private const Boolean debugMode = false;

    public const Int32 StiltzkinMogModelId = 212;
    public const Int32 GoldenFrogModelId = 423;
    public const Int32 ThiefNPCCoinCondition = 51;
    public const Int32 ViviRunningVelocity = 33;

    private static Int32 stiltzkinBuy = 0;
    private static Int32 g_tship_model_get_flg = 0;
    private static Int32 quadmistOpponentId = 0;
    private static Int32 lastGWldItemGet0 = 0;
    private static Int32 numOfTreasures = 24;
    private static Int32 numOfSandyBeach = 21;
    private static Int32 lastNumOfVisitedSandyBeach = 0;

    public static Int32 lastGFFossilMog;

    public static List<Int32> BeachData = new List<Int32>
    {
        4,
        5,
        13,
        16,
        17,
        18,
        19,
        25,
        29,
        30,
        31,
        33,
        37,
        38,
        46,
        47,
        49,
        50,
        51,
        52,
        58
    };
}
