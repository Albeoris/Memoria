using System;
using System.Collections.Generic;
using UnityEngine;

public class EndGame
{
    public EndGame()
    {
        this.count = -1;
        this.wx = new Int64[3];
        this.wy = new Int64[3];
        this.dealer = new EndGameDef.HAND_DEF();
        this.player = new EndGameDef.HAND_DEF();
        this.split = new EndGameDef.HAND_DEF();
    }

    public void Start(EndGameMain endGameMain)
    {
        this.endGameMain = endGameMain;
        EndGame.ff9endingGameDeck = new EndGameDef.DECK_DEF();
        this.ff9endingGameInit();
        this.ff9endingGameWager = 10L;
        this.ff9endingGameHandInit(this.player, 30, 120);
        this.ff9endingGameHandInit(this.dealer, 30, 10);
        this.handPtr = this.player;
        this.brightness = 0;
        this.resultType = EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_WIN;
        this.dealNdx = 0;
        this.isSplit = 0;
        this.cx = 240f;
        this.cy = 10f;
        this.dx = 1f;
        this.dy = 1f;
        this.wx[0] = (this.wx[1] = (this.wx[2] = 0L));
        this.wy[0] = (this.wy[1] = (this.wy[2] = 0L));
        this._gameState = EndGameState.OnUpdate;
        this.InitRenderer();
    }

    public void OnUpdate()
    {
        if (this.ff9endingGameState >= EndGameDef.FF9EndingGameState.ENDGAME_STATE_END)
        {
            this._gameState = EndGameState.Finish;
            return;
        }
        this.ff9endingGameLoopStart();
        switch (this.ff9endingGameState)
        {
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_INIT:
                this.count = -1;
                this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_SHUFFLE;
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_SHUFFLE:
                if (this.count == -1)
                {
                    this.ff9endingGameDeckInit(EndGame.ff9endingGameDeck);
                    this.ff9endingGameDeckShuffle(EndGame.ff9endingGameDeck);
                    this.StartWipe(1, 1, 60, EndGameDef.FF9WIPE_SETRGB(0L, 0L, 0L));
                    this.count = 78;
                    this.dealNdx = 0;
                }
                else if (this.count != 0)
                {
                    this.ff9endingGameCardRender(EndGameDef.ENDGAME_CARD(EndGame.ff9endingGameDeck.ndxList[this.dealNdx]), EndGameDef.ENDGAME_SUIT(EndGame.ff9endingGameDeck.ndxList[this.dealNdx]), 240, 10);
                    this.dealNdx += 4;
                    this.count--;
                }
                else
                {
                    this.count = -1;
                    this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_WAGER;
                    EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_BET);
                }
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_WAGER:
                this.ff9endingGameHandInit(this.player, 30, 120);
                this.ff9endingGameHandInit(this.dealer, 30, 10);
                if (EndSys.Endsys_GetMode() == EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_QUIT)
                {
                    this.count = -1;
                    this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_DEAL;
                    this.DBG_PRINT("ff9endingGameBankroll: " + EndGame.ff9endingGameBankroll);
                }
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_DEAL:
                if (this.count == -1)
                {
                    this.dealer.flags |= 16u;
                    this.ff9endingGameDeckNextCard(this.player, EndGame.ff9endingGameDeck);
                    this.ff9endingGameDeckNextCard(this.dealer, EndGame.ff9endingGameDeck);
                    this.ff9endingGameDeckNextCard(this.player, EndGame.ff9endingGameDeck);
                    this.ff9endingGameDeckNextCard(this.dealer, EndGame.ff9endingGameDeck);
                    this.handPtr = this.player;
                    this.dealNdx = 0;
                    this.cx = 240f;
                    this.cy = 10f;
                    this.dx = EndGameDef.ENDGAME_FIXEDDIV((Single)this.player.pos[0] - 240f, 30f);
                    this.dy = EndGameDef.ENDGAME_FIXEDDIV((Single)this.player.pos[1] - 10f, 30f);
                    this.count = 120;
                    this.DBG_PRINT("player.cardTotal: " + this.player.cardTotal);
                    if (this.dealCardInterpolation.Count > 0)
                    {
                        this.dealCardInterpolation.Dequeue();
                    }
                    EndGame.CardInterpolationData item = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.player.pos[0], (Single)this.player.pos[1], 0f));
                    this.dealCardInterpolation.Enqueue(item);
                }
                else if (this.count > 0)
                {
                    this.cx += this.dx;
                    this.cy += this.dy;
                    if ((this.count - 1) % 30 == 0)
                    {
                        this.dealNdx++;
                        if (this.dealNdx < 4)
                        {
                            this.cx = 240f;
                            this.cy = 10f;
                            switch (this.dealNdx)
                            {
                                case 1:
                                {
                                    this.dx = EndGameDef.ENDGAME_FIXEDDIV((Single)this.dealer.pos[0] - 240f, 30f);
                                    this.dy = EndGameDef.ENDGAME_FIXEDDIV((Single)this.dealer.pos[1] - 10f, 30f);
                                    if (this.dealCardInterpolation.Count > 0)
                                    {
                                        this.dealCardInterpolation.Dequeue();
                                    }
                                    EndGame.CardInterpolationData item2 = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.dealer.pos[0], (Single)this.dealer.pos[1], 0f));
                                    this.dealCardInterpolation.Enqueue(item2);
                                    break;
                                }
                                case 2:
                                {
                                    this.dx = EndGameDef.ENDGAME_FIXEDDIV((Single)this.player.pos[0] + 13f - 240f, 30f);
                                    this.dy = EndGameDef.ENDGAME_FIXEDDIV((Single)this.player.pos[1] - 10f, 30f);
                                    if (this.dealCardInterpolation.Count > 0)
                                    {
                                        this.dealCardInterpolation.Dequeue();
                                    }
                                    EndGame.CardInterpolationData item2 = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.player.pos[0] + 13f, (Single)this.player.pos[1], 0f));
                                    this.dealCardInterpolation.Enqueue(item2);
                                    break;
                                }
                                case 3:
                                {
                                    this.dx = EndGameDef.ENDGAME_FIXEDDIV((Single)this.dealer.pos[0] + 13f - 240f, 30f);
                                    this.dy = EndGameDef.ENDGAME_FIXEDDIV((Single)this.dealer.pos[1] - 10f, 30f);
                                    if (this.dealCardInterpolation.Count > 0)
                                    {
                                        this.dealCardInterpolation.Dequeue();
                                    }
                                    EndGame.CardInterpolationData item2 = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.dealer.pos[0] + 13f, (Single)this.dealer.pos[1], 0f));
                                    this.dealCardInterpolation.Enqueue(item2);
                                    break;
                                }
                            }
                        }
                    }
                    this.count--;
                    this.ff9endingGameHandRender(this.dealer, 0, 0, (Int32)((this.dealNdx <= 1) ? 0 : 1));
                    this.ff9endingGameHandRender(this.player, 0, 0, (Int32)((this.dealNdx <= 0) ? 0 : ((Int32)((this.dealNdx >= 3) ? 2 : 1))));
                }
                else
                {
                    if (this.dealCardInterpolation.Count > 0)
                    {
                        this.dealCardInterpolation.Dequeue();
                    }
                    if (this.ff9endingGameHandBlackJack(this.player) != 0)
                    {
                        if (this.ff9endingGameHandBlackJack(this.dealer) != 0)
                        {
                            this.count = -1;
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PUSH;
                        }
                        else
                        {
                            this.count = -1;
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_BLACKJACK;
                        }
                    }
                    else if (this.ff9endingGameHandBlackJack(this.dealer) != 0)
                    {
                        if (this.ff9endingGameHandBlackJack(this.player) != 0)
                        {
                            this.count = -1;
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PUSH;
                        }
                        else
                        {
                            this.count = -1;
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_BLACKJACK;
                        }
                    }
                    else
                    {
                        this.count = -1;
                        this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSPLAYER;
                        EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION);
                    }
                }
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSPLAYER:
                this.optionFlags = this.ff9endingGameHandOptions(this.handPtr);
                this.ff9endingGameDoubleAllowed = false;
                this.ff9endingGameSplitAllowed = false;
                if ((this.optionFlags & 2UL) != 0UL && this.ff9endingGameWager * 2L <= EndGame.ff9endingGameBankroll)
                {
                    this.ff9endingGameDoubleAllowed = true;
                }
                if ((this.optionFlags & 1UL) != 0UL && this.isSplit == 0 && this.ff9endingGameWager * 2L <= EndGame.ff9endingGameBankroll)
                {
                    this.ff9endingGameSplitAllowed = true;
                }
                if (this.count == -1)
                {
                    if (EndSys.Endsys_GetMode() == EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_QUIT)
                    {
                        if (EndAct.Endact_GetMode() == EndAct.ENDACT_MODE_ENUM.ENDACT_MODE_STAND)
                        {
                            this.count = -1;
                            if (this.isSplit == 1)
                            {
                                this.handPtr = this.split;
                                this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSPLAYER;
                                EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION);
                                this.cx = 240f;
                                this.cy = 10f;
                                this.dx = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount - 240f, 30f);
                                this.dy = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount - 10f, 30f);
                                this.count = 30;
                                this.isSplit = 2;
                                if (this.dealCardInterpolation.Count > 0)
                                {
                                    this.dealCardInterpolation.Dequeue();
                                }
                                EndGame.CardInterpolationData item3 = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount, (Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount, 0f));
                                this.dealCardInterpolation.Enqueue(item3);
                            }
                            else
                            {
                                this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSDEALER;
                                this.dealer.flags &= 4294967279u;
                                this.DBG_PRINT("Player STAND");
                            }
                        }
                        else if (EndAct.Endact_GetMode() == EndAct.ENDACT_MODE_ENUM.ENDACT_MODE_HIT)
                        {
                            this.cx = 240f;
                            this.cy = 10f;
                            this.dx = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount - 240f, 30f);
                            this.dy = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount - 10f, 30f);
                            this.count = 30;
                            this.DBG_PRINT("Player HIT");
                            if (this.dealCardInterpolation.Count > 0)
                            {
                                this.dealCardInterpolation.Dequeue();
                            }
                            EndGame.CardInterpolationData item4 = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount, (Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount, 0f));
                            this.dealCardInterpolation.Enqueue(item4);
                        }
                        else if (EndAct.Endact_GetMode() == EndAct.ENDACT_MODE_ENUM.ENDACT_MODE_DOUBLE)
                        {
                            this.cx = 240f;
                            this.cy = 10f;
                            this.dx = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount - 240f, 30f);
                            this.dy = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount - 10f, 30f);
                            this.count = 30;
                            this.handPtr.flags |= 4u;
                            this.DBG_PRINT("Player DOUBLE");
                            if (this.dealCardInterpolation.Count > 0)
                            {
                                this.dealCardInterpolation.Dequeue();
                            }
                            EndGame.CardInterpolationData item5 = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount, (Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount, 0f));
                            this.dealCardInterpolation.Enqueue(item5);
                        }
                        else if (EndAct.Endact_GetMode() == EndAct.ENDACT_MODE_ENUM.ENDACT_MODE_SPLIT)
                        {
                            this.optionFlags = this.ff9endingGameHandOptions(this.handPtr);
                            if ((this.optionFlags & 1UL) != 0UL)
                            {
                                this.ff9endingGameHandSplit(this.player, this.split);
                                this.cx = 240f;
                                this.cy = 10f;
                                this.dx = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount - 240f, 30f);
                                this.dy = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount - 10f, 30f);
                                this.count = 30;
                                this.isSplit = 1;
                                this.DBG_PRINT("Player SPLIT");
                                if (this.dealCardInterpolation.Count > 0)
                                {
                                    this.dealCardInterpolation.Dequeue();
                                }
                                EndGame.CardInterpolationData item6 = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount, (Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount, 0f));
                                this.dealCardInterpolation.Enqueue(item6);
                            }
                        }
                    }
                }
                else if (this.count > 0)
                {
                    this.cx += this.dx;
                    this.cy += this.dy;
                    this.count--;
                }
                else
                {
                    if (this.dealCardInterpolation.Count > 0)
                    {
                        this.dealCardInterpolation.Dequeue();
                    }
                    this.ff9endingGameDeckNextCard(this.handPtr, EndGame.ff9endingGameDeck);
                    this.DBG_PRINT("player.cardTotal: " + this.player.cardTotal);
                    this.count = -1;
                    if (this.handPtr.cardTotal > 21u)
                    {
                        this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_BUST;
                    }
                    else if (this.handPtr.cardTotal == 21u)
                    {
                        if (this.isSplit == 1)
                        {
                            this.handPtr = this.split;
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSPLAYER;
                            EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION);
                            this.cx = 240f;
                            this.cy = 10f;
                            this.dx = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount - 240f, 30f);
                            this.dy = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount - 10f, 30f);
                            this.count = 30;
                            this.isSplit = 2;
                            if (this.dealCardInterpolation.Count > 0)
                            {
                                this.dealCardInterpolation.Dequeue();
                            }
                            EndGame.CardInterpolationData item7 = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount, (Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount, 0f));
                            this.dealCardInterpolation.Enqueue(item7);
                        }
                        else
                        {
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSDEALER;
                        }
                    }
                    else if ((this.handPtr.flags & 8u) != 0u)
                    {
                        if (this.isSplit == 1)
                        {
                            this.handPtr = this.split;
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSPLAYER;
                            EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION);
                            this.cx = 240f;
                            this.cy = 10f;
                            this.dx = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount - 240f, 30f);
                            this.dy = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount - 10f, 30f);
                            this.count = 30;
                            this.isSplit = 2;
                            if (this.dealCardInterpolation.Count > 0)
                            {
                                this.dealCardInterpolation.Dequeue();
                            }
                            EndGame.CardInterpolationData item8 = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount, (Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount, 0f));
                            this.dealCardInterpolation.Enqueue(item8);
                        }
                        else
                        {
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSDEALER;
                        }
                    }
                    else if ((this.handPtr.flags & 4u) != 0u)
                    {
                        if (this.isSplit == 1)
                        {
                            this.handPtr = this.split;
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSPLAYER;
                            EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION);
                            this.cx = 240f;
                            this.cy = 10f;
                            this.dx = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount - 240f, 30f);
                            this.dy = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount - 10f, 30f);
                            this.count = 30;
                            this.isSplit = 2;
                            if (this.dealCardInterpolation.Count > 0)
                            {
                                this.dealCardInterpolation.Dequeue();
                            }
                            EndGame.CardInterpolationData item9 = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount, (Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount, 0f));
                            this.dealCardInterpolation.Enqueue(item9);
                        }
                        else
                        {
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSDEALER;
                        }
                    }
                    else
                    {
                        this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSPLAYER;
                        EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION);
                    }
                }
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSDEALER:
                if (this.count == -1)
                {
                    if (this.dealer.cardTotal < 17u)
                    {
                        this.cx = 240f;
                        this.cy = 10f;
                        this.dx = EndGameDef.ENDGAME_FIXEDDIV(30f + 13f * this.dealer.cardCount - 240f, 30f);
                        this.dy = EndGameDef.ENDGAME_FIXEDDIV(10f + 0f * this.dealer.cardCount - 10f, 30f);
                        this.count = 30;
                        if (this.dealCardInterpolation.Count > 0)
                        {
                            this.dealCardInterpolation.Dequeue();
                        }
                        EndGame.CardInterpolationData item10 = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.dealer.pos[0] + 13f * this.dealer.cardCount, (Single)this.dealer.pos[1] + 0f * this.dealer.cardCount, 0f));
                        this.dealCardInterpolation.Enqueue(item10);
                    }
                    else if ((this.dealer.flags & 2u) != 0u)
                    {
                        this.count = -1;
                        this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_BUST;
                    }
                    else
                    {
                        this.count = -1;
                        if (this.isSplit > 0)
                        {
                            this.isSplit = 1;
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_SPLITCHECK;
                        }
                        else if (this.player.cardTotal == this.dealer.cardTotal)
                        {
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PUSH;
                        }
                        else if (this.player.cardTotal > this.dealer.cardTotal)
                        {
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_WIN;
                        }
                        else
                        {
                            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_LOSS;
                        }
                    }
                }
                else if (this.count > 0)
                {
                    this.cx += this.dx;
                    this.cy += this.dy;
                    this.count--;
                }
                else
                {
                    if (this.dealCardInterpolation.Count > 0)
                    {
                        this.dealCardInterpolation.Dequeue();
                    }
                    this.count = -1;
                    this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSDEALER;
                    this.ff9endingGameDeckNextCard(this.dealer, EndGame.ff9endingGameDeck);
                }
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_SPLITCHECK:
                if (this.count == -1)
                {
                    if (this.isSplit == 1)
                    {
                        if (this.player.cardTotal > 21u)
                        {
                            this.resultType = EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_LOSE;
                            this.FF9SFX_Play(107u);
                            EndGame.ff9endingGameBankroll -= this.ff9endingGameWager * (Int64)(((this.player.flags & 4u) == 0u) ? 1L : 2L);
                        }
                        else if (this.dealer.cardTotal > 21u || this.player.cardTotal > this.dealer.cardTotal)
                        {
                            this.resultType = EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_WIN;
                            this.FF9SFX_Play(108u);
                            EndGame.ff9endingGameBankroll += this.ff9endingGameWager * (Int64)(((this.player.flags & 4u) == 0u) ? 1L : 2L);
                        }
                        else if (this.player.cardTotal == this.dealer.cardTotal)
                        {
                            this.resultType = EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_DRAW;
                        }
                        else
                        {
                            this.resultType = EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_LOSE;
                            this.FF9SFX_Play(107u);
                            EndGame.ff9endingGameBankroll -= this.ff9endingGameWager * (Int64)(((this.player.flags & 4u) == 0u) ? 1L : 2L);
                        }
                    }
                    else if (this.split.cardTotal > 21u)
                    {
                        this.resultType = EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_LOSE;
                        this.FF9SFX_Play(107u);
                        EndGame.ff9endingGameBankroll -= this.ff9endingGameWager * (Int64)(((this.split.flags & 4u) == 0u) ? 1L : 2L);
                    }
                    else if (this.dealer.cardTotal > 21u || this.split.cardTotal > this.dealer.cardTotal)
                    {
                        this.resultType = EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_WIN;
                        this.FF9SFX_Play(108u);
                        EndGame.ff9endingGameBankroll += this.ff9endingGameWager * (Int64)(((this.split.flags & 4u) == 0u) ? 1L : 2L);
                    }
                    else if (this.split.cardTotal == this.dealer.cardTotal)
                    {
                        this.resultType = EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_DRAW;
                    }
                    else
                    {
                        this.resultType = EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_LOSE;
                        this.FF9SFX_Play(107u);
                        EndGame.ff9endingGameBankroll -= this.ff9endingGameWager * (Int64)(((this.split.flags & 4u) == 0u) ? 1L : 2L);
                    }
                    this.count = 148;
                    this.dealer.flags &= 4294967279u;
                }
                else if (this.count > 0)
                {
                    if (this.count < 16)
                    {
                        this.brightness = this.count * 16;
                    }
                    else if (this.count > 132)
                    {
                        this.brightness = (148 - this.count) * 16;
                    }
                    else
                    {
                        this.brightness = 255;
                    }
                    this.ff9endingGameResultRender(this.resultType, this.brightness, 108, (Int32)((this.isSplit != 1) ? (this.split.pos[1] + 12) : (this.player.pos[1] + 12)));
                    this.count--;
                }
                else if (this.isSplit == 1)
                {
                    this.count = -1;
                    this.isSplit = 2;
                }
                else
                {
                    this.count = -1;
                    this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_NEXTHAND;
                }
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_BUST:
                if (this.handPtr.cardTotal > 21u)
                {
                    if (this.isSplit == 2)
                    {
                        this.dealNdx = 2;
                    }
                    else
                    {
                        this.dealNdx = 1;
                    }
                }
                else
                {
                    this.dealNdx = 0;
                }
                if (this.count == -1)
                {
                    this.count = 10;
                    this.wx[this.dealNdx] = (Int64)(EndGame.Random() % 3);
                    this.wy[this.dealNdx] = (Int64)(EndGame.Random() % 3);
                }
                else if (this.count > 0)
                {
                    if (this.wx[this.dealNdx] < 0L)
                    {
                        this.wx[this.dealNdx] = (Int64)(EndGame.Random() % 3);
                    }
                    else
                    {
                        this.wx[this.dealNdx] = (Int64)(-(Int64)(EndGame.Random() % 3));
                    }
                    if (this.wy[this.dealNdx] < 0L)
                    {
                        this.wy[this.dealNdx] = (Int64)(EndGame.Random() % 3);
                    }
                    else
                    {
                        this.wy[this.dealNdx] = (Int64)(-(Int64)(EndGame.Random() % 3));
                    }
                    this.count--;
                }
                else
                {
                    this.wx[0] = (this.wx[1] = (this.wx[2] = 0L));
                    this.wy[0] = (this.wy[1] = (this.wy[2] = 0L));
                    this.count = -1;
                    if (this.isSplit == 1)
                    {
                        this.handPtr = this.split;
                        this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSPLAYER;
                        EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION);
                        this.cx = 240f;
                        this.cy = 10f;
                        this.dx = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount - 240f, 30f);
                        this.dy = EndGameDef.ENDGAME_FIXEDDIV((Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount - 10f, 30f);
                        this.count = 30;
                        this.isSplit = 2;
                        if (this.dealCardInterpolation.Count > 0)
                        {
                            this.dealCardInterpolation.Dequeue();
                        }
                        EndGame.CardInterpolationData item11 = new EndGame.CardInterpolationData(0.5f, new Vector3(240f, 10f, 0f), new Vector3((Single)this.handPtr.pos[0] + 13f * this.handPtr.cardCount, (Single)this.handPtr.pos[1] + 0f * this.handPtr.cardCount, 0f));
                        this.dealCardInterpolation.Enqueue(item11);
                    }
                    else if (this.isSplit == 2)
                    {
                        this.isSplit = 1;
                        this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_SPLITCHECK;
                    }
                    else if (this.handPtr.cardTotal > 21u)
                    {
                        this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_LOSS;
                    }
                    else
                    {
                        this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_WIN;
                    }
                }
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_BLACKJACK:
                if (this.count == -1)
                {
                    if (this.player.cardTotal > this.dealer.cardTotal)
                    {
                        this.FF9SFX_Play(108u);
                        EndGame.ff9endingGameBankroll += this.ff9endingGameWager + (this.ff9endingGameWager >> 1);
                    }
                    else
                    {
                        this.FF9SFX_Play(107u);
                        EndGame.ff9endingGameBankroll -= this.ff9endingGameWager;
                    }
                    this.count = 148;
                    this.dealer.flags &= 4294967279u;
                }
                else if (this.count > 0)
                {
                    if (this.count < 16)
                    {
                        this.brightness = this.count * 16;
                    }
                    else if (this.count > 132)
                    {
                        this.brightness = (148 - this.count) * 16;
                    }
                    else
                    {
                        this.brightness = 255;
                    }
                    if (this.player.cardTotal > this.dealer.cardTotal)
                    {
                        this.ff9endingGameResultRender(EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_WIN, this.brightness, 108, this.player.pos[1] + 12);
                    }
                    else
                    {
                        this.ff9endingGameResultRender(EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_LOSE, this.brightness, 108, this.player.pos[1] + 12);
                    }
                    this.count--;
                }
                else
                {
                    this.count = -1;
                    this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_NEXTHAND;
                }
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_WIN:
                if (this.count == -1)
                {
                    this.FF9SFX_Play(108u);
                    EndGame.ff9endingGameBankroll += this.ff9endingGameWager * (Int64)(((this.player.flags & 4u) == 0u) ? 1L : 2L);
                    this.count = 148;
                    this.dealer.flags &= 4294967279u;
                }
                else if (this.count > 0)
                {
                    if (this.count < 16)
                    {
                        this.brightness = this.count * 16;
                    }
                    else if (this.count > 132)
                    {
                        this.brightness = (148 - this.count) * 16;
                    }
                    else
                    {
                        this.brightness = 255;
                    }
                    this.ff9endingGameResultRender(EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_WIN, this.brightness, 108, this.player.pos[1] + 12);
                    this.count--;
                }
                else
                {
                    this.count = -1;
                    this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_NEXTHAND;
                    this.DBG_PRINT("Player WIN");
                    this.DBG_PRINT("dealer.cardTotal: " + this.dealer.cardTotal);
                    this.DBG_PRINT("bankroll: " + EndGame.ff9endingGameBankroll);
                }
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_PUSH:
                if (this.count == -1)
                {
                    this.count = 148;
                    this.dealer.flags &= 4294967279u;
                }
                else if (this.count > 0)
                {
                    if (this.count < 16)
                    {
                        this.brightness = this.count * 16;
                    }
                    else if (this.count > 132)
                    {
                        this.brightness = (148 - this.count) * 16;
                    }
                    else
                    {
                        this.brightness = 255;
                    }
                    this.ff9endingGameResultRender(EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_DRAW, this.brightness, 108, this.player.pos[1] + 12);
                    this.count--;
                }
                else
                {
                    this.count = -1;
                    this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_NEXTHAND;
                    this.DBG_PRINT("Player PUSH");
                    this.DBG_PRINT("dealer.cardTotal: " + this.dealer.cardTotal);
                    this.DBG_PRINT("bankroll: " + EndGame.ff9endingGameBankroll);
                }
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_LOSS:
                if (this.count == -1)
                {
                    this.FF9SFX_Play(107u);
                    EndGame.ff9endingGameBankroll -= this.ff9endingGameWager * (Int64)(((this.player.flags & 4u) == 0u) ? 1L : 2L);
                    this.count = 148;
                    this.dealer.flags &= 4294967279u;
                }
                else if (this.count > 0)
                {
                    if (this.count < 16)
                    {
                        this.brightness = this.count * 16;
                    }
                    else if (this.count > 132)
                    {
                        this.brightness = (148 - this.count) * 16;
                    }
                    else
                    {
                        this.brightness = 255;
                    }
                    this.ff9endingGameResultRender(EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_LOSE, this.brightness, 108, this.player.pos[1] + 12);
                    this.count--;
                }
                else
                {
                    this.count = -1;
                    this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_NEXTHAND;
                    this.DBG_PRINT("Player LOSS");
                    this.DBG_PRINT("dealer.cardTotal: " + this.dealer.cardTotal);
                    this.DBG_PRINT("bankroll: " + EndGame.ff9endingGameBankroll);
                }
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_NEXTHAND:
                EMinigame.GetPlayBlackjackAchievement();
                if ((EndGame.ff9endingGameDeck.flags & 4) != 0)
                {
                    this.count = -1;
                    this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_LEAVEGAME;
                }
                else
                {
                    this.isSplit = 0;
                    if (this.ff9endingGameWager > EndGame.ff9endingGameBankroll)
                    {
                        this.ff9endingGameWager = EndGame.ff9endingGameBankroll / 10L * 10L;
                    }
                    this.count = -1;
                    this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_WAGER;
                    EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_BET);
                }
                break;
            case EndGameDef.FF9EndingGameState.ENDGAME_STATE_LEAVEGAME:
                if (this.count == -1)
                {
                    this.count = 60;
                    this.StartWipe(1, 1, this.count, EndGameDef.FF9WIPE_SETRGB(255L, 255L, 255L));
                    if (EndGame.ff9endingGameBankroll < 10L)
                    {
                        EndGame.ff9endingGameBankroll = 1000L;
                    }
                }
                else if (this.count > 0)
                {
                    this.count--;
                }
                else
                {
                    this.count = -1;
                    this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_END;
                    this.DBG_PRINT("Leavegame");
                    this.DBG_PRINT("Bankroll: " + EndGame.ff9endingGameBankroll);
                }
                break;
        }
        this.endGameMain.endGameScore.ClearScores();
        if (this.ff9endingGameState > EndGameDef.FF9EndingGameState.ENDGAME_STATE_DEAL)
        {
            this.ff9endingGameHandRender(this.dealer, (Int32)this.wx[0], (Int32)this.wy[0], -1);
            if (this.dealer.cardCount > 0u && this.ff9endingGameState >= EndGameDef.FF9EndingGameState.ENDGAME_STATE_PROCESSDEALER)
            {
                this.endGameMain.endGameScore.dealerCardTotal = this.dealer.cardTotal.ToString();
                this.FF9Font_Draw08(10, this.dealer.pos[1] + 32, 16, this._str, EndGameDef.FF9FONT_COL_ENUM.FF9FONT_COL_WHITE);
            }
            if (this.isSplit == 2)
            {
                this.ff9endingGameHandRender(this.split, (Int32)this.wx[2], (Int32)this.wy[2], -1);
                if (this.split.cardCount > 0u)
                {
                    this.endGameMain.endGameScore.splitCardTotal = this.split.cardTotal.ToString();
                    this.FF9Font_Draw08(10, this.split.pos[1] + 32, 16, this._str, EndGameDef.FF9FONT_COL_ENUM.FF9FONT_COL_WHITE);
                    if (this.split.cardTotal != 21u && (this.split.flags & 1u) != 0u)
                    {
                        this.endGameMain.endGameScore.splitMinTotal = this.split.minTotal.ToString();
                        this.FF9Font_Draw08(10, this.split.pos[1] + 32 + 10, 16, this._str, EndGameDef.FF9FONT_COL_ENUM.FF9FONT_COL_CYAN);
                    }
                }
            }
            this.ff9endingGameHandRender(this.player, (Int32)this.wx[1], (Int32)this.wy[1], -1);
            if (this.player.cardCount > 0u)
            {
                this.endGameMain.endGameScore.playerCardTotal = this.player.cardTotal.ToString();
                this.FF9Font_Draw08(10, this.player.pos[1] + 32, 16, this._str, EndGameDef.FF9FONT_COL_ENUM.FF9FONT_COL_WHITE);
                if (this.player.cardTotal != 21u && (this.player.flags & 1u) != 0u)
                {
                    this.endGameMain.endGameScore.playerMinTotal = this.player.minTotal.ToString();
                    this.FF9Font_Draw08(10, this.player.pos[1] + 32 + 10, 16, this._str, EndGameDef.FF9FONT_COL_ENUM.FF9FONT_COL_CYAN);
                }
            }
            if (this.isSplit == 1)
            {
                this.ff9endingGameHandRender(this.split, (Int32)this.wx[2], (Int32)this.wy[2], -1);
                if (this.split.cardCount > 0u)
                {
                    this.endGameMain.endGameScore.splitCardTotal = this.split.cardTotal.ToString();
                    this.FF9Font_Draw08(10, this.split.pos[1] + 32, 16, this._str, EndGameDef.FF9FONT_COL_ENUM.FF9FONT_COL_WHITE);
                    if (this.split.cardTotal != 21u && (this.split.flags & 1u) != 0u)
                    {
                        this.endGameMain.endGameScore.splitMinTotal = this.split.minTotal.ToString();
                        this.FF9Font_Draw08(10, this.split.pos[1] + 32 + 10, 16, this._str, EndGameDef.FF9FONT_COL_ENUM.FF9FONT_COL_CYAN);
                    }
                }
            }
        }
        this.ff9endingGameShoeRender(240, 10);
        if (this.ff9endingGameState < EndGameDef.FF9EndingGameState.ENDGAME_STATE_END && EndGame.ff9endingGameBankroll < 10L)
        {
            this.count = -1;
            this.ff9endingGameState = EndGameDef.FF9EndingGameState.ENDGAME_STATE_LEAVEGAME;
        }
        this.ff9endingGameLoopEnd();
    }

    public void Finish()
    {
        this.DBG_PRINT("Finish()");
        this._gameState = EndGameState.None;
    }

    public EndGameState GetGameState()
    {
        return this._gameState;
    }

    public EndGameDef.FF9EndingGameState GetEndingGameState()
    {
        return this.ff9endingGameState;
    }

    public static Int32 Random()
    {
        return UnityEngine.Random.Range(1, 10000);
    }

    private Int32 ff9endingGameInit()
    {
        if (SoundLib.GetAllSoundDispatchPlayer().GetCurrentMusicId() == -1)
        {
            FF9Snd.ff9endsnd_song_play(156);
            FF9Snd.ff9endsnd_song_vol_fade(156, 90, 0, 127);
        }
        return 0;
    }

    private Int32 ff9endingGameShutdown()
    {
        return 0;
    }

    private void ff9endingGameLoopStart()
    {
    }

    private void ff9endingGameLoopEnd()
    {
    }

    private Int32 ff9endingGameHandInit(EndGameDef.HAND_DEF hand, Int32 px, Int32 py)
    {
        hand.flags = 0u;
        hand.cardCount = 0u;
        hand.cardTotal = 0u;
        hand.minTotal = 0u;
        hand.pos[0] = px;
        hand.pos[1] = py;
        for (Int32 i = 0; i < 22; i++)
        {
            hand.cardNdx[i] = 0UL;
        }
        return 0;
    }

    private Int32 ff9endingGameHandSplit(EndGameDef.HAND_DEF org, EndGameDef.HAND_DEF split)
    {
        org.cardCount = 1u;
        org.pos[0] = 30;
        org.pos[1] = 140;
        this.ff9endingGameHandInit(split, 30, 96);
        split.cardCount = 1u;
        split.cardNdx[0] = org.cardNdx[1];
        Int32 num = EndGameDef.ENDGAME_CARD((Int32)org.cardNdx[1]);
        if (num < 9)
        {
            org.cardTotal = (UInt32)(num + 2);
            org.minTotal = (UInt32)(num + 2);
            split.cardTotal = (UInt32)(num + 2);
            split.minTotal = (UInt32)(num + 2);
        }
        else if (num == 12)
        {
            org.cardTotal = 11u;
            org.minTotal = 1u;
            org.flags |= 9u;
            split.cardTotal = 11u;
            split.minTotal = 1u;
            split.flags |= 9u;
        }
        else
        {
            org.cardTotal = 10u;
            org.minTotal = 10u;
            split.cardTotal = 10u;
            split.minTotal = 10u;
        }
        return 0;
    }

    private UInt64 ff9endingGameHandOptions(EndGameDef.HAND_DEF hand)
    {
        UInt64 num = 0UL;
        if (hand.cardCount == 2u)
        {
            num |= 2UL;
            if (EndGameDef.ENDGAME_CARD((Int32)hand.cardNdx[0]) == EndGameDef.ENDGAME_CARD((Int32)hand.cardNdx[1]))
            {
                num |= 1UL;
            }
        }
        return num;
    }

    private Int32 ff9endingGameHandBlackJack(EndGameDef.HAND_DEF hand)
    {
        if (hand.cardTotal == 21u && hand.cardCount == 2u)
        {
            return 1;
        }
        return 0;
    }

    private Int32 ff9endingGameDeckInit(EndGameDef.DECK_DEF deck)
    {
        for (Int32 i = 0; i < 6; i++)
        {
            for (Int32 j = 0; j < 52; j++)
            {
                deck.ndxList[i * 52 + j] = j;
            }
        }
        deck.flags = 1;
        deck.cardNdx = 0;
        return 0;
    }

    private Int32 ff9endingGameDeckShuffle(EndGameDef.DECK_DEF deck)
    {
        for (Int32 i = 0; i < 5; i++)
        {
            Int32 num = 0;
            for (Int32 j = 312; j > 0; j--)
            {
                Int32 num2 = num + (EndGame.Random() << 1) % j;
                Int32 num3 = deck.ndxList[num];
                deck.ndxList[num] = deck.ndxList[num2];
                deck.ndxList[num2] = num3;
                num++;
            }
        }
        deck.flags = (UInt16)(deck.flags | 2);
        return 0;
    }

    private Int32 ff9endingGameDeckNextCard(EndGameDef.HAND_DEF hand, EndGameDef.DECK_DEF deck)
    {
        Int32[] ndxList = deck.ndxList;
        UInt16 cardNdx;
        deck.cardNdx = (UInt16)((cardNdx = deck.cardNdx) + 1);
        Int32 num = ndxList[(Int32)cardNdx];
        hand.cardNdx[(Int32)((UIntPtr)(hand.cardCount++))] = (UInt64)((Int64)num);
        Int32 num2 = EndGameDef.ENDGAME_CARD(num);
        if (num2 < 9)
        {
            hand.cardTotal += (UInt32)(num2 + 2);
            hand.minTotal += (UInt32)(num2 + 2);
        }
        else if (num2 == 12)
        {
            hand.cardTotal += 11u;
            hand.minTotal += 1u;
            hand.flags |= 1u;
        }
        else
        {
            hand.cardTotal += 10u;
            hand.minTotal += 10u;
        }
        if (hand.cardTotal > 21u)
        {
            if ((hand.flags & 1u) != 0u)
            {
                hand.cardTotal -= 10u;
                if (hand.minTotal >= hand.cardTotal)
                {
                    hand.flags &= 4294967294u;
                }
            }
            else
            {
                hand.flags |= 2u;
            }
        }
        if (deck.cardNdx >= 249)
        {
            deck.flags = (UInt16)(deck.flags | 4);
        }
        return 0;
    }

    public void FF9SFX_Play(UInt32 soundEffectID)
    {
        FF9Snd.ff9snd_sndeffect_play((Int32)soundEffectID, 8388608, 127, 128);
    }

    public void DBG_PRINT(String msg)
    {
    }

    private void InitRenderer()
    {
        this.ResetFrameBasedRenderingObject();
        this.ResetTimeBasedAnimation();
    }

    public void ResetFrameBasedRenderingObject()
    {
        this.mostBackward = this.endGameMain.CanvasPlane.transform.position.z;
        this.mostForward = this.endGameMain.CanvasCamera.transform.position.z;
        this.currentDepth = this.mostBackward;
        this.deltaDepth = (this.mostForward - this.mostBackward) / 32f;
        this.currentDepth += this.deltaDepth * 5f;
        this.ResetFrameBasedRenderObject();
    }

    private void ResetFrameBasedRenderObject()
    {
        for (Int32 i = 0; i < this.endGameMain.InHandGameCardCounts.Count; i++)
        {
            this.endGameMain.InHandGameCardCounts[i] = 0;
        }
        this.cardFaceDownCounter = 0;
        foreach (List<GameObject> list in this.endGameMain.GameCards)
        {
            foreach (GameObject gameObject in list)
            {
                gameObject.transform.position = this.cardOriginalPosition;
            }
        }
        foreach (GameObject gameObject2 in this.endGameMain.CardFaceDown)
        {
            gameObject2.transform.position = this.cardOriginalPosition;
        }
        this.endGameMain.WinResultShadow.transform.position = this.resultOriginalPosition;
        this.endGameMain.DrawResultShadow.transform.position = this.resultOriginalPosition;
        this.endGameMain.LoseResultShadow.transform.position = this.resultOriginalPosition;
        this.endGameMain.WinResult.transform.position = this.resultOriginalPosition;
        this.endGameMain.DrawResult.transform.position = this.resultOriginalPosition;
        this.endGameMain.LoseResult.transform.position = this.resultOriginalPosition;
    }

    public void ResetTimeBasedAnimation()
    {
        foreach (GameObject gameObject in this.endGameMain.CardFaceDownMove)
        {
            gameObject.transform.position = this.cardOriginalPosition;
        }
    }

    public void UpdateTimeBasedAnimation(Single deltaTime)
    {
        if (this.dealCardInterpolation.Count > 0)
        {
            EndGame.CardInterpolationData cardInterpolationData = this.dealCardInterpolation.Peek();
            cardInterpolationData.currentInterpolationTime += deltaTime;
            if (cardInterpolationData.currentInterpolationTime > cardInterpolationData.period)
            {
                cardInterpolationData.currentInterpolationTime = cardInterpolationData.period;
            }
            else
            {
                Single normalizeProgress = cardInterpolationData.currentInterpolationTime / cardInterpolationData.period;
                Vector2 v = cardInterpolationData.destinationPoint - cardInterpolationData.initialPoint;
                Vector3 b = this.ManipulateCardPosition(v, normalizeProgress);
                if (b.magnitude > v.magnitude)
                {
                    b = v;
                }
                cardInterpolationData.currentPoint = cardInterpolationData.initialPoint + b;
            }
            this.RenderWithHighScaleCoordinate(this.endGameMain.CardFaceDownMove[0], (Int32)cardInterpolationData.currentPoint[0], (Int32)cardInterpolationData.currentPoint[1], false);
        }
    }

    private Vector3 ManipulateCardPosition(Vector3 displacement, Single normalizeProgress)
    {
        if (this.isEnableEaseAnimation)
        {
            Single num = 0.63f;
            Single num2 = 1.45f;
            Single num3 = 4.69f;
            Single num4 = 4.69f;
            Single num5 = 1f - normalizeProgress;
            Single num6 = num * num5 * num5 * num5 + 3f * (num2 * num5 * num5 * normalizeProgress) + 3f * (num3 * num5 * normalizeProgress * normalizeProgress) + num4 * normalizeProgress * normalizeProgress * normalizeProgress;
            return displacement * (num6 / num4);
        }
        return displacement * normalizeProgress;
    }

    private void FF9Font_Draw08(Int32 x, Int32 y, Int32 z, String str, EndGameDef.FF9FONT_COL_ENUM color)
    {
    }

    private void StartWipe(Int32 type, Int32 mode, Int32 frame, Int64 color)
    {
    }

    private Int32 ff9endingGameResultRender(EndGameDef.ENDGAME_RESULT_ENUM resultType, Int32 brightness, Int32 sx, Int32 sy)
    {
        GameObject gameObject = (GameObject)null;
        GameObject gameObject2 = (GameObject)null;
        Int32 num = 0;
        Int32 num2 = 0;
        if (resultType == EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_WIN)
        {
            gameObject = this.endGameMain.WinResult;
            gameObject2 = this.endGameMain.WinResultShadow;
        }
        else if (resultType == EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_DRAW)
        {
            gameObject = this.endGameMain.DrawResult;
            gameObject2 = this.endGameMain.DrawResultShadow;
        }
        else if (resultType == EndGameDef.ENDGAME_RESULT_ENUM.ENDGAME_RESULT_LOSE)
        {
            gameObject = this.endGameMain.LoseResult;
            gameObject2 = this.endGameMain.LoseResultShadow;
        }
        Color color = gameObject.GetComponent<Renderer>().material.color;
        color.a = (Single)brightness / 255f;
        gameObject.GetComponent<Renderer>().material.color = color;
        color = gameObject2.GetComponent<Renderer>().material.color;
        color.a = (Single)brightness / 255f;
        gameObject2.GetComponent<Renderer>().material.color = color;
        this.RenderWithHighScaleCoordinate(gameObject, sx + num, sy + num2, true);
        this.RenderWithHighScaleCoordinate(gameObject2, sx + num, sy + num2, true);
        return 0;
    }

    private void RenderWithHighScaleCoordinate(GameObject gameObject, Int32 sx, Int32 sy, Boolean useBatchDepth = true)
    {
        Single num = (Single)sx / FieldMap.PsxFieldWidth;
        Single num2 = (Single)sy / FieldMap.PsxFieldHeightNative;
        Single x = 5f + num * -10f;
        Single z = -5f + num2 * 10f;
        Vector3 localPosition;
        if (useBatchDepth)
        {
            this.currentDepth += this.deltaDepth;
            localPosition = new Vector3(x, this.currentDepth, z);
        }
        else
        {
            localPosition = gameObject.transform.localPosition;
            localPosition = new Vector3(x, localPosition.y, z);
        }
        gameObject.transform.localPosition = localPosition;
    }

    private Int32 ff9endingGameCardRenderFaceDown(Int32 sx, Int32 sy)
    {
        this.RenderWithHighScaleCoordinate(this.endGameMain.CardFaceDown[this.cardFaceDownCounter], sx, sy, true);
        this.cardFaceDownCounter++;
        return 0;
    }

    private Int32 ff9endingGameShoeRender(Int32 sx, Int32 sy)
    {
        for (Int32 i = 0; i < 3; i++)
        {
            GameObject gameObject = this.endGameMain.CardShoe[i];
            this.RenderWithHighScaleCoordinate(gameObject, sx - (i << 1), sy - i, true);
        }
        return 0;
    }

    private Int32 ff9endingGameCardRender(Int32 cardNdx, Int32 suitNdx, Int32 sx, Int32 sy)
    {
        Int32 num = suitNdx + cardNdx * 4;
        GameObject gameObject = this.endGameMain.GameCards[num][this.endGameMain.InHandGameCardCounts[num]];
        this.RenderWithHighScaleCoordinate(gameObject, sx, sy, true);
        if (this.endGameMain.InHandGameCardCounts[num] < 5)
        {
            List<Int32> inHandGameCardCounts;
            List<Int32> list = inHandGameCardCounts = this.endGameMain.InHandGameCardCounts;
            Int32 num2;
            Int32 index = num2 = num;
            num2 = inHandGameCardCounts[num2];
            list[index] = num2 + 1;
        }
        return 0;
    }

    private Int32 ff9endingGameHandRender(EndGameDef.HAND_DEF hand, Int32 ox, Int32 oy, Int32 cardCount)
    {
        if (cardCount == 0)
        {
            return 1;
        }
        Int32 num;
        if (cardCount < 0)
        {
            if (hand.cardCount == 0u)
            {
                return 1;
            }
            num = (Int32)(hand.cardCount - 1u);
        }
        else
        {
            num = cardCount - 1;
        }
        Int32 num2 = hand.pos[0] + ox + num * 13;
        Int32 num3 = hand.pos[1] + oy + num * 0;
        Int32 num4 = 1;
        for (Int32 i = num; i >= 0; i--)
        {
            Int32 n = (Int32)hand.cardNdx[i];
            if (num4 != 0 && (hand.flags & 16u) != 0u && num == 1)
            {
                this.ff9endingGameCardRenderFaceDown(num2, num3);
                num4 = 0;
            }
            else
            {
                this.ff9endingGameCardRender(EndGameDef.ENDGAME_CARD(n), EndGameDef.ENDGAME_SUIT(n), num2, num3);
            }
            num2 -= 13;
            //num3 = num3;
        }
        return 0;
    }

    private const Int64 DefaultGameBankroll = 1000L;

    private const Single depthIndex = 32f;

    private const Int32 reservedDepthCount = 5;

    private EndGameState _gameState;

    private static EndGameDef.DECK_DEF ff9endingGameDeck;

    public static Int64 ff9endingGameBankroll = 1000L;

    public Int64 ff9endingGameWager;

    public Boolean ff9endingGameDoubleAllowed;

    public Boolean ff9endingGameSplitAllowed;

    private Int32 count;

    private Int32 buzzCount;

    private Int32 dealNdx;

    private Int32 isSplit;

    private EndGameDef.ENDGAME_RESULT_ENUM resultType;

    private Int32 brightness;

    private String _str;

    private Single cx;

    private Single cy;

    private Single dx;

    private Single dy;

    private Int64[] wx;

    private Int64[] wy;

    private UInt64 optionFlags;

    private EndGameDef.HAND_DEF handPtr;

    private EndGameDef.HAND_DEF dealer;

    private EndGameDef.HAND_DEF player;

    private EndGameDef.HAND_DEF split;

    private EndGameDef.FF9EndingGameState ff9endingGameState;

    private EndGameMain endGameMain;

    private Int32 cardFaceDownCounter;

    private Single mostBackward;

    private Single mostForward;

    private Single deltaDepth;

    private Single currentDepth;

    private Vector3 cardOriginalPosition = new Vector3(-5819f, 5f, 0f);

    private Vector2 resultOriginalPosition = new Vector3(-5819f, 200f, 0f);

    public Boolean isEnableEaseAnimation = true;

    private Queue<EndGame.CardInterpolationData> dealCardInterpolation = new Queue<EndGame.CardInterpolationData>();

    public class CardInterpolationData
    {
        public CardInterpolationData(Single period, Vector3 initialPoint, Vector3 destinationPoint)
        {
            this.period = period;
            this.initialPoint = initialPoint;
            this.destinationPoint = destinationPoint;
            this.currentInterpolationTime = 0f;
            this.currentPoint = this.initialPoint;
        }

        public Single period;

        public Vector3 initialPoint;

        public Vector3 destinationPoint;

        public Single currentInterpolationTime;

        public Vector3 currentPoint;
    }
}