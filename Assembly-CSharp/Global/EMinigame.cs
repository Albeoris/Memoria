using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.SiliconSocial;
using Memoria;

public class EMinigame
{
	public static void ChanbaraBonusPoints(Obj s1, EBin eBin)
	{
		if (FF9StateSystem.Common.FF9.fldMapNo == 64 && s1.sid == 4 && s1.ip == 223)
		{
			Int32 num = eBin.getVarManually(12505);
			num += num / 10 * 3;
			EMinigame.GetEncoreChanbaraAchievement(num);
			eBin.setVarManually(12505, num);
		}
	}

	public static void GetEncoreChanbaraAchievement(Int32 score)
	{
		if (score >= 75)
		{
			AchievementManager.ReportAchievement(AcheivementKey.Encore, 1);
		}
	}

	public static Int32 StiltzkinBuy
	{
		get
		{
			return EMinigame.stiltzkinBuy;
		}
		set
		{
			EMinigame.stiltzkinBuy = value;
		}
	}

	public static void StiltzkinAchievement(PosObj gCur, UInt32 gilDecrease)
	{
		if (gCur.model == 212)
		{
			if (gilDecrease == 333 || gilDecrease == 444 || gilDecrease == 555 || gilDecrease == 666 || gilDecrease == 777 || gilDecrease == 888 || gilDecrease == 2222 || gilDecrease == 5555)
				FF9StateSystem.Achievement.StiltzkinBuy++;
			AchievementManager.ReportAchievement(AcheivementKey.AllStiltzkinItem, FF9StateSystem.Achievement.StiltzkinBuy);
		}
	}

	public static void EidolonMuralAchievement(String currentLanguange, Int32 messageId)
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (fldMapNo == 1704)
		{
			Boolean flag;
			switch (currentLanguange)
			{
			case "French":
			case "Italian":
				flag = (messageId == 119);
				goto IL_C2;
			case "German":
				flag = (messageId == 120);
				goto IL_C2;
			case "Japanese":
				flag = (messageId == 122);
				goto IL_C2;
			}
			flag = (messageId == 118);
			IL_C2:
			if (flag)
			{
				AchievementManager.ReportAchievement(AcheivementKey.EidolonMural, 1);
			}
		}
	}

	public static void ExcellentLuckColorFortuneTellingAchievement(String currentLanguage, Int32 messageId)
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (fldMapNo == 352)
		{
			Boolean flag;
            switch (currentLanguage)
            {
                case "English(UK)":
                case "English(US)":
                    flag = (messageId == 222);
                    goto IL_B0;
                case "Spanish":
                    flag = (messageId == 230);
                    goto IL_B0;
            }
            
			flag = (messageId == 233);
			IL_B0:
			if (flag)
			{
				AchievementManager.ReportAchievement(AcheivementKey.ExcellentLuck, 1);
			}
		}
	}

	public static void JumpingRopeAchievement(String lang, Int32 mesId)
	{
		Int32 num = 0;
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(220);
		if (fldMapNo == 103)
		{
			num = 11209;
		}
		if (fldMapNo == 2456 && varManually >= 10300)
		{
			num = 15305;
		}
		if (num == 0)
		{
			return;
		}
		Int32 varManually2 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(num);
		if (FF9StateSystem.Settings.IsFastTrophyMode)
		{
			if (varManually2 == 5)
			{
				AchievementManager.ReportAchievement(AcheivementKey.Rope100, 1);
			}
			if (varManually2 == 10)
			{
				AchievementManager.ReportAchievement(AcheivementKey.Rope1000, 1);
			}
			return;
		}
		Boolean flag = EMinigame.IsCongratMessage(lang, mesId, fldMapNo);
		if (varManually2 == 100 && flag)
		{
			AchievementManager.ReportAchievement(AcheivementKey.Rope100, 1);
		}
		if (varManually2 >= 1000 && flag)
		{
			AchievementManager.ReportAchievement(AcheivementKey.Rope1000, 1);
		}
	}

	private static Boolean IsCongratMessage(String lang, Int32 mesId, Int32 fieldId)
	{
		Boolean result = false;
		if (fieldId == 103)
		{
			result = (mesId == 241 || mesId == 69);
		}
		if (fieldId == 2456 && lang != null)
		{
            switch (lang)
            {
                case "English(UK)":
                case "English(US)":
                default:
                    result = (mesId == 259 || mesId == 69);
                    break;
                case "French":
				case "Spanish":
				case "German":
				case "Italian":
				case "Japanese":
                    result = (mesId == 266 || mesId == 69);
                    break;
            }
		}
		return result;
	}

	public static void ProvokeMogAchievement(String currentLanguange, Int32 messageId)
    {
        if (PersistenSingleton<SceneDirector>.Instance.CurrentScene != SceneDirector.WorldMapSceneName)
        {
            return;
        }
        Boolean flag = false;
        switch (currentLanguange)
        {
            case "English(US)":
            case "English(UK)":
            case "French":
            case "Spanish":
            case "German":
            case "Italian":
            case "Japanese":
                flag = (messageId == 49);
                break;
        }
        if (flag)
        {
            AchievementManager.ReportAchievement(AcheivementKey.ProvokeMoogle, 1);
        }
    }

    public static void Catching99FrogAchievement(Int32 numFrog)
	{
		AchievementManager.ReportAchievement(AcheivementKey.Frog99, numFrog);
		if (numFrog == 99)
		{
		}
	}

    public const Int32 GoldenFrogModelId = 423;

    public static void CatchingGoldenFrogAchievement(Obj frogObj)
	{
		if (((PosObj)frogObj).model == GoldenFrogModelId)
			AchievementManager.ReportAchievement(AcheivementKey.GoldenFrog, 1);
	}

	public static void GetRewardFromQueenStellaAchievement()
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (fldMapNo == 911 || fldMapNo == 1911)
		{
			Int32 num = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(91132);
			Int32 num2 = 0;
			for (Int32 i = 0; i < 12; i++)
			{
				num2 += (Int32)((num % 2 != 1) ? 0 : 1);
				num >>= 1;
			}
			AchievementManager.ReportAchievement(AcheivementKey.QueenReward10, num2);
		}
	}

	public static void GetTheAirShipAchievement()
	{
		if (PersistenSingleton<EventEngine>.Instance.gMode == 3)
		{
			Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(220);
			Int32 varManually2 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(728);
			if (varManually == 10400 && varManually2 == 60)
			{
				AchievementManager.ReportAchievement(AcheivementKey.Airship, 1);
			}
		}
	}

	public static void TreasureHunterSAchievement(String lang, Int32 mesId)
	{
		Boolean flag = false;
		if (FF9StateSystem.Common.FF9.fldMapNo == 1900 && mesId == 289)
		{
			flag = true;
		}
		else if (FF9StateSystem.Common.FF9.fldMapNo == 2801 && mesId == 236)
		{
			flag = true;
		}
		if (flag)
		{
			AchievementManager.ReportAchievement(AcheivementKey.TreasureHuntS, 1);
		}
	}

	public static void ShuffleGameAchievement(String lang, Int32 mesId)
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (fldMapNo == 1858)
		{
			Boolean flag;
            switch (lang)
            {
                case "English(US)":
                case "English(UK)":
                    flag = (mesId == 265);
                    goto IL_8E;
            }

			flag = (mesId == 264);
			IL_8E:
			if (flag)
			{
				AchievementManager.ReportAchievement(AcheivementKey.Shuffle9, 1);
			}
		}
	}

	public static void GetTheaterShipMaquetteAchievement()
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(220);
		if ((fldMapNo == 457 || (fldMapNo == 455 && varManually < 11090)) && EMinigame.g_tship_model_get_flg == 0)
		{
			EMinigame.g_tship_model_get_flg = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(619492);
			if (EMinigame.g_tship_model_get_flg == 1)
			{
				AchievementManager.ReportAchievement(AcheivementKey.ShipMaquette, 1);
			}
		}
	}

	public static void GetHelpAllVictimsInCleyraTown()
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(220);
		if (fldMapNo == 1109 && varManually == 4980)
		{
			Int32 varManually2 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(993764);
			Int32 varManually3 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(994020);
			Int32 varManually4 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(994276);
			if (varManually2 == 1 && varManually3 == 1 && varManually4 == 1)
			{
				AchievementManager.ReportAchievement(AcheivementKey.CleyraVictimAll, 1);
			}
		}
	}

	public static void GetPlayBlackjackAchievement()
	{
		AchievementManager.ReportAchievement(AcheivementKey.Blackjack, 1);
	}

	public static void ChocoboBeakLV99Achievement(String language, Int32 mesId)
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (fldMapNo == 2950 || fldMapNo == 2951 || fldMapNo == 2952)
		{
			Boolean flag;
            switch (language)
            {
                case "Japanese":
                    flag = (mesId == 325 || mesId == 277);
                    goto IL_F5;
                case "English(US)":
                case "English(UK)":
                    flag = (mesId == 320 || mesId == 277);
                    goto IL_F5;
            }
            
			flag = (mesId == 326 || mesId == 278);
			IL_F5:
			if (flag)
			{
				Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(35796);
				AchievementManager.ReportAchievement(AcheivementKey.ChocoboLv99, varManually);
			}
		}
	}

	public static void Auction10TimesAchievement(Obj gCur)
	{
		if (gCur == null)
		{
			return;
		}
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if ((fldMapNo == 909 && gCur.sid == 7) || (fldMapNo == 1909 && gCur.sid == 3))
		{
			AchievementManager.ReportAchievement(AcheivementKey.Auction10, ++FF9StateSystem.Achievement.AuctionTime);
		}
	}

	public static void ViviWinHuntAchievement()
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (fldMapNo == 600)
		{
			Boolean flag = false;
			Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(80372);
			if (varManually == 2)
			{
				flag = true;
			}
			if (flag)
			{
				AchievementManager.ReportAchievement(AcheivementKey.ViviWinHunt, 1);
			}
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
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		Int32 uid = (Int32)gCur.uid;
		EMinigame.quadmistOpponentId = EMinigame.CreateNPCID(fldMapNo, uid);
		EMinigame.SetGroupingOpponentId();
	}

	private static void SetGroupingOpponentId()
	{
		Int32 num = EMinigame.quadmistOpponentId;
		if (num != 558002)
		{
			if (num != 908009)
			{
				if (num == 1306002)
				{
					goto IL_42;
				}
				if (num != 1908006)
				{
					if (num != 2106002)
					{
						return;
					}
					goto IL_42;
				}
			}
			EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
			Int32 varManually = eBin.getVarManually(94708);
			EMinigame.quadmistOpponentId = 908009;
			if (varManually >= 51)
			{
				EMinigame.quadmistOpponentId += 255;
			}
			return;
		}
		IL_42:
		EMinigame.quadmistOpponentId = 558002;
	}

	public static void SetQuadmistStadiumOpponentId(Obj gCur, Int32 minigameFlag)
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if ((fldMapNo == 903 && gCur.sid == 11) || (fldMapNo == 1903 && gCur.sid == 15))
		{
			EMinigame.quadmistOpponentId = fldMapNo * 1000 + minigameFlag + 100;
		}
		else if (fldMapNo == 112 && gCur.sid == 7)
		{
			EMinigame.SetQuadmistOpponentId(gCur);
		}
	}

	public static void SetThiefId(Obj gCur)
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (fldMapNo == 908 || fldMapNo == 1908)
		{
			Int32 num = EMinigame.CreateNPCID(fldMapNo, (Int32)gCur.uid);
			Int32 num2 = num;
			if (num2 == 908009 || num2 == 1908006)
			{
				EMinigame.SetGroupingOpponentId();
			}
		}
	}

	public static void SetFatChocoboId(Obj gCur)
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (fldMapNo == 2955 && gCur.uid == 9)
		{
			EMinigame.SetQuadmistOpponentId(gCur);
		}
	}

	public static void QuadmistWinAllNPCAchievement()
	{
		if (EMinigame.quadmistOpponentId <= 0)
		{
			return;
		}
		if (FF9StateSystem.Achievement.QuadmistWinList.Add(EMinigame.quadmistOpponentId))
		{
			AchievementManager.ReportAchievement(AcheivementKey.CardWinAll, FF9StateSystem.Achievement.QuadmistWinList.Count);
		}
		EMinigame.quadmistOpponentId = 0;
	}

	public static void DigUpMadainRingAchievement()
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (fldMapNo == 1421)
		{
			Boolean flag = true;
			if (flag)
			{
				AchievementManager.ReportAchievement(AcheivementKey.MadainRing, 1);
			}
		}
	}

	public static void DigUpMadianRingCheating()
	{
		EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
		if (FF9StateSystem.Common.FF9.fldMapNo == 1421 && EBin.s1.sid == 0 && EBin.s1.ip == 813)
		{
			eBin.setVarManually(12245, 2);
			eBin.setVarManually(14553, 203);
		}
	}

	public static void MognetCentralAchievement()
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (fldMapNo == 3100)
		{
			Boolean flag = true;
			if (flag)
			{
				AchievementManager.ReportAchievement(AcheivementKey.MognetCentral, 1);
			}
		}
	}

	public static void AtleteQueenAchievement_Debug(String lang, Int32 mesId)
	{
		if (FF9StateSystem.Common.FF9.fldMapNo != 1850)
		{
			return;
		}
		if (mesId == 152 && FF9StateSystem.Settings.IsFastTrophyMode && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(125940) < 80)
		{
			PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(125940, 80);
		}
	}

	public static void AtleteQueenAchievement()
	{
		if (FF9StateSystem.Common.FF9.fldMapNo != 1850)
		{
			return;
		}
		AchievementManager.ReportAchievement(AcheivementKey.AthleteQueen, 1);
	}

	public static void SuperSlickOilAchievement()
	{
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (fldMapNo == 2457)
		{
			Boolean flag = true;
			if (flag)
			{
				AchievementManager.ReportAchievement(AcheivementKey.SuperSlickOil, 1);
			}
		}
	}

	public static void SuperSlickOilCheating()
	{
		EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
		eBin.setVarManually(2174948, 1);
		eBin.setVarManually(1952740, 0);
	}

	public static void AllTreasureAchievement()
	{
		if (PersistenSingleton<SceneDirector>.Instance.CurrentScene != SceneDirector.WorldMapSceneName)
		{
			return;
		}
		Boolean flag = false;
		EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
		Int32 num = eBin.getVarManually(47304);
		num &= 16777215;
		Int32 num2 = EMinigame.CountOpenedTreasure();
		if (EMinigame.lastGWldItemGet0 != num && num == 16777215)
		{
			flag = true;
		}
		if (flag)
		{
			AchievementManager.ReportAchievement(AcheivementKey.AllTreasure, EMinigame.numOfTreasures);
		}
		EMinigame.lastGWldItemGet0 = num;
	}

	public static void InitializeAllTreasureAchievement()
	{
		EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
		EMinigame.lastGWldItemGet0 = eBin.getVarManually(47304);
		EMinigame.lastGWldItemGet0 &= 16777215;
	}

	private static Int32 CountOpenedTreasure()
	{
		EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
		Int32 varManually = eBin.getVarManually(47304);
		Int32 num = 0;
		for (Int32 i = 0; i < num; i++)
		{
			Int32 num2 = varManually >> i & 1;
			if (num2 == 1)
			{
				num++;
			}
		}
		return num;
	}

	public static void AllTreasureCheating()
	{
		EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
		eBin.setVarManually(47304, 16777214);
		eBin.setVarManually(48072, 16777215);
		FF9StateSystem.EventState.gEventGlobal[191] = 5;
	}

	public static void AllSandyBeachAchievement()
	{
		if (PersistenSingleton<SceneDirector>.Instance.CurrentScene != SceneDirector.WorldMapSceneName)
		{
			return;
		}
		Int32 num = EMinigame.CountVisitedSandyBeach();
		if (EMinigame.lastNumOfVisitedSandyBeach != num)
		{
			AchievementManager.ReportAchievement(AcheivementKey.AllSandyBeach, num);
			EMinigame.lastNumOfVisitedSandyBeach = num;
		}
	}

	public static void InitializeAllSandyBeachAchievement()
	{
		EMinigame.lastNumOfVisitedSandyBeach = EMinigame.CountVisitedSandyBeach();
	}

	private static Int32 CountVisitedSandyBeach()
	{
		EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
		Int32 num = 0;
		for (Int32 i = 0; i < EMinigame.numOfSandyBeach; i++)
		{
			Int32 varManually = eBin.getVarManually(219364 + i * 256);
			if (varManually == 1)
			{
				num++;
			}
		}
		return num;
	}

	public static void AllSandyBeachCheating()
	{
		EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
		eBin.setVarManually(266980, 1);
		eBin.setVarManually(27608, 65534);
		eBin.setVarManually(28120, 31);
		FF9StateSystem.EventState.gEventGlobal[191] = 5;
	}

	public static void DigUpKupoAchievement()
	{
		EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
		Int32 varManually = eBin.getVarManually(833508);
		if (FF9StateSystem.Common.FF9.fldMapNo == 1421 && EMinigame.lastGFFossilMog != varManually && varManually == 1)
		{
			AchievementManager.ReportAchievement(AcheivementKey.Kuppo, 1);
			EMinigame.lastGFFossilMog = varManually;
		}
	}

	public static void InitializeDigUpKupoAchievement()
	{
		EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
		EMinigame.lastGFFossilMog = eBin.getVarManually(833508);
	}

	public static void ATE80Achievement(Int32 ateID)
	{
		if (ateID == -1)
		{
			return;
		}
		EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
		Int32[] ateCheck = FF9StateSystem.Achievement.AteCheck;
		Boolean flag = true;
		Debug.Log(String.Concat(new Object[]
		{
			AcheivementKey.ATE80,
			" : ateCheck[",
			ateID,
			"] = ",
			ateCheck[ateID]
		}));
		if (ateCheck[ateID] == 0)
		{
			ateCheck[ateID] = 1;
		}
		Int32 num = 0;
		for (Int32 i = 0; i < 83; i++)
		{
			if (ateCheck[i] == 1 && i != 6 && i != 7 && i != 14)
			{
				num++;
			}
		}
		if (flag)
		{
			Debug.Log(AcheivementKey.ATE80 + " : numOfSeenATE = " + num);
			AchievementManager.ReportAchievement(AcheivementKey.ATE80, num);
		}
	}

	public static Int32 MappingATEID(Dialog dialog, Int32 selectedChoice, Boolean isCompulsory)
	{
		Int32 result = -1;
		if (FF9StateSystem.Common.FF9.fldLocNo == 40)
		{
			if (FF9StateSystem.Common.FF9.fldMapNo == 206 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 1900 && selectedChoice == 0)
			{
				result = 0;
			}
			else
			{
				result = 1 + selectedChoice;
			}
		}
		else if (FF9StateSystem.Common.FF9.fldLocNo == 4)
		{
			if (selectedChoice == 4 && !isCompulsory)
			{
				result = 4;
			}
			else if ((FF9StateSystem.Common.FF9.fldMapNo == 253 || FF9StateSystem.Common.FF9.fldMapNo == 204) && isCompulsory)
			{
				result = 5;
			}
			else if (FF9StateSystem.Common.FF9.fldMapNo == 262 && isCompulsory)
			{
				result = 6;
			}
		}
		else if (FF9StateSystem.Common.FF9.fldLocNo == 8)
		{
			if (FF9StateSystem.Common.FF9.fldMapNo == 306 && isCompulsory)
			{
				result = 7;
			}
		}
		else if (FF9StateSystem.Common.FF9.fldLocNo == 47 || FF9StateSystem.Common.FF9.fldLocNo == 53)
		{
			if (isCompulsory)
			{
				result = 8;
			}
			else
			{
				result = 8 + selectedChoice;
			}
		}
		else if (FF9StateSystem.Common.FF9.fldLocNo == 276)
		{
			if (FF9StateSystem.Common.FF9.fldMapNo == 554 && isCompulsory)
			{
				result = 14;
			}
			else if (FF9StateSystem.Common.FF9.fldMapNo == 552 && isCompulsory)
			{
				result = 16;
			}
			else if (FF9StateSystem.Common.FF9.fldMapNo == 565 && isCompulsory)
			{
				result = 18;
			}
			else
			{
				result = 15 + selectedChoice;
				if (selectedChoice == 3)
				{
					result = 19;
				}
			}
		}
		else if (FF9StateSystem.Common.FF9.fldLocNo == 70)
		{
			if (isCompulsory)
			{
				result = 24;
			}
			else
			{
				result = 20 + selectedChoice;
			}
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
			{
				result = 32;
			}
			else if (FF9StateSystem.Common.FF9.fldMapNo == 1307)
			{
				result = 33;
			}
		}
		else if (FF9StateSystem.Common.FF9.fldLocNo == 525)
		{
			if (isCompulsory && FF9StateSystem.Common.FF9.fldMapNo == 1353)
			{
				result = 34;
			}
		}
		else if (FF9StateSystem.Common.FF9.fldLocNo == 32)
		{
			if (isCompulsory)
			{
				result = 40;
			}
			else
			{
				result = 35 + selectedChoice;
			}
		}
		else if (FF9StateSystem.Common.FF9.fldLocNo == 37)
		{
			if (isCompulsory)
			{
				result = 42;
			}
			else
			{
				result = 41 + selectedChoice;
			}
		}
		else if (FF9StateSystem.Common.FF9.fldLocNo == 358)
		{
			if (isCompulsory)
			{
				result = 49;
			}
			else
			{
				result = 47 + selectedChoice;
			}
		}
		else if (FF9StateSystem.Common.FF9.fldLocNo == 88 || FF9StateSystem.Common.FF9.fldLocNo == 90)
		{
			result = 52 + selectedChoice;
		}
		else if (FF9StateSystem.Common.FF9.fldLocNo == 359)
		{
			if (FF9StateSystem.Common.FF9.fldMapNo == 956 && isCompulsory)
			{
				result = 57;
			}
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
					Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
					if (varManually == 9100)
					{
						result = 70;
					}
					else if (varManually == 9200)
					{
						result = 71;
					}
					else if (varManually == 10010)
					{
						result = 76;
					}
					else if (varManually == 10030)
					{
						result = 77;
					}
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
			{
				result = 81;
			}
			else
			{
				result = 78 + selectedChoice;
			}
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
				{
					return true;
				}
			}
			else if (PersistenSingleton<EventEngine>.Instance.gMode == 3)
			{
				if (EventCollision.IsRidingChocobo() && EventInput.IsPressedDig)
				{
					return true;
				}
				EventInput.IsPressedDig = false;
			}
		}
		return false;
	}

	public static void SetHippaulLevel(Obj s1, EBin eBin, Int32 level)
	{
	}

	public static void SetViviSpeed(Obj s1, EBin eBin)
	{
		if (FF9StateSystem.Common.FF9.fldMapNo == 1850 && s1.sid == 15)
		{
			if (s1.ip == 1445 || s1.ip == 1497)
			{
				Int32 varManually = eBin.getVarManually(9429);
				eBin.setVarManually(9429, Configuration.Hacks.HippaulRacingViviSpeed);
			}
			else if (s1.ip == 1595)
			{
				Int32 num = eBin.getVarManually(9429);
				if (num != 0 && num < 5)
				{
					num = 5;
					eBin.setVarManually(9429, num);
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
			Int32 varManually = instance.eBin.getVarManually(266980);
			if (varManually > 0)
			{
				Int32 num = ff9.w_frameGetParameter(192);
				Int32 count = EMinigame.BeachData.Count;
				Int32 num2 = 0;
				Int32 num3 = 0;
				for (Int32 i = 0; i < count; i++)
				{
					Int32 num4 = 219364 + (i << 8);
					if (num == EMinigame.BeachData[i])
					{
						num2 = num4;
					}
					if (instance.eBin.getVarManually(num4) > 0)
					{
						num3++;
					}
					if (i < count - 1)
					{
						num3 <<= 1;
					}
				}
				if (num2 != 0)
				{
					Int32 varManually2 = instance.eBin.getVarManually(num2);
					result = (varManually2 == 0 || (varManually2 == 1 && num3 == 2097151));
				}
			}
		}
		return result;
	}

	private const Boolean showLog = false;

	private const Boolean debugMode = false;

	private const Int32 ChanbaraPointCondition = 10;

	private const Int32 ChanbaraBonusPoint = 3;

	private const Int32 StiltzkinMogModelId = 212;

	public const Int32 GTrenoIzumiAddress = 94708;

	public const Int32 ThiefNPCCoinCondition = 51;

	public const Int32 GKabaoLevelAddress = 125940;

	public const Int32 RaceVecAddress = 9429;

	public const Int32 ViviRunningVelocity = 33;

	public const Int32 WM_AREA_04 = 4;

	public const Int32 WM_AREA_05 = 5;

	public const Int32 WM_AREA_13 = 13;

	public const Int32 WM_AREA_16 = 16;

	public const Int32 WM_AREA_17 = 17;

	public const Int32 WM_AREA_18 = 18;

	public const Int32 WM_AREA_19 = 19;

	public const Int32 WM_AREA_25 = 25;

	public const Int32 WM_AREA_29 = 29;

	public const Int32 WM_AREA_30 = 30;

	public const Int32 WM_AREA_31 = 31;

	public const Int32 WM_AREA_33 = 33;

	public const Int32 WM_AREA_37 = 37;

	public const Int32 WM_AREA_38 = 38;

	public const Int32 WM_AREA_46 = 46;

	public const Int32 WM_AREA_47 = 47;

	public const Int32 WM_AREA_49 = 49;

	public const Int32 WM_AREA_50 = 50;

	public const Int32 WM_AREA_51 = 51;

	public const Int32 WM_AREA_52 = 52;

	public const Int32 WM_AREA_58 = 58;

	public const Int32 BeachFlagStartOffset = 219364;

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
