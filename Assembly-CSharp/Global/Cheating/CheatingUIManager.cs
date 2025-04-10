using Assets.Scripts.Common;
using System;
using UnityEngine;

public class CheatingUIManager : MonoBehaviour
{
    private void Start()
    {
        if (PersistenSingleton<CheatingManager>.Instance.IsCheatingMode)
        {
            this.resultText.rawText = "Cheating mode is on for now.";
        }
        else
        {
            this.resultText.rawText = "Please press on button.";
        }
    }

    public void CheatAchievementLevel1()
    {
        this.UpdateAchivement(1);
    }

    public void CheatAchievementLevel2()
    {
        this.UpdateAchivement(2);
    }

    public void CheatAchievementLevel3()
    {
        this.UpdateAchivement(3);
    }

    public void Back()
    {
        SceneDirector.Replace("MainMenu", SceneTransition.FadeOutToBlack_FadeIn, true);
    }

    private void UpdateAchivement(Int32 level)
    {
        this.UpdateAuctionItem(level);
        this.UpdateChocoboBeak(level);
        this.UpdateCardGame(level);
        this.UpdateJumpingRobe();
        this.UpdateCatchedFrog();
        this.UpdateDefeatEnemy(level);
        this.UpdateUsingAbility(level);
        this.UpdateTurnToTrance(level);
        PersistenSingleton<CheatingManager>.Instance.IsCheatingMode = true;
        this.resultText.rawText = "Enabled cheat mode level " + level + ".";
    }

    private void UpdateDefeatEnemy(Int32 level)
    {
        Int16 defeatEnemy;
        if (level != 1)
        {
            if (level != 2)
            {
                defeatEnemy = 9999;
            }
            else
            {
                defeatEnemy = 999;
            }
        }
        else
        {
            defeatEnemy = 99;
        }
        PersistenSingleton<CheatingManager>.Instance.DefeatEnemy = (Int32)defeatEnemy;
    }

    private void UpdateCardGame(Int32 level)
    {
        Int16 num;
        if (level != 1)
        {
            num = 237;
        }
        else
        {
            num = 99;
        }
        PersistenSingleton<CheatingManager>.Instance.QuadMistWin = num;
        PersistenSingleton<CheatingManager>.Instance.QuadMistWonNPC = (Int32)num;
    }

    private void UpdateUsingAbility(Int32 level)
    {
        PersistenSingleton<CheatingManager>.Instance.BlkMgcUsage = 99;
        PersistenSingleton<CheatingManager>.Instance.WhtMgcUsage = 199;
        PersistenSingleton<CheatingManager>.Instance.BluMgcUsage = 99;
        PersistenSingleton<CheatingManager>.Instance.SummonUsage = 49;
    }

    private void UpdateChocoboBeak(Int32 level)
    {
        PersistenSingleton<CheatingManager>.Instance.LevelBeak = 98;
    }

    private void UpdateCatchedFrog()
    {
        PersistenSingleton<CheatingManager>.Instance.CatchedFrog = 98;
    }

    private void UpdateAuctionItem(Int32 level)
    {
        PersistenSingleton<CheatingManager>.Instance.AuctionTime = 9;
    }

    private void UpdateJumpingRobe()
    {
        PersistenSingleton<CheatingManager>.Instance.JumpRobeTime = 998;
    }

    private void UpdateTurnToTrance(Int32 level)
    {
        PersistenSingleton<CheatingManager>.Instance.TranceCount = 49;
    }

    public UILabel resultText;
}
