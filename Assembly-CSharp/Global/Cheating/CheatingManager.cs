using System;

public class CheatingManager : PersistenSingleton<CheatingManager>
{
    public void ApplyDataWhenEventStart()
    {
        this.CheatQuadmistWin();
        this.CheatQuadmistNpcWon();
        this.CheatBeakLevel();
        this.CheatAuction();
        this.CheatBattle();
        this.CheatCatchingFrog();
        this.CheatSeenATE();
    }

    public void CheatQuadmistWin()
    {
    }

    public void CheatQuadmistNpcWon()
    {
    }

    public void CheatBeakLevel()
    {
    }

    public void CheatJumpingRobe()
    {
    }

    public void CheatCatchingFrog()
    {
    }

    public void CheatSeenATE()
    {
    }

    public void CheatAuction()
    {
    }

    public void CheatBattle()
    {
    }

    public Boolean IsCheatingMode;

    public Int32 LevelBeak;

    public Int32 JumpRobeTime;

    public Int16 QuadMistWin;

    public Int32 QuadMistWonNPC;

    public Int32 AuctionTime;

    public Int32 DefeatEnemy;

    public Int32 CatchedFrog;

    public Int32 BlkMgcUsage;

    public Int32 WhtMgcUsage;

    public Int32 BluMgcUsage;

    public Int32 SummonUsage;

    public Int32 TranceCount;
}
