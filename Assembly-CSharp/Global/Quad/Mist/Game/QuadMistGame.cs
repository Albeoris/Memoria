using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.Common;
using Memoria;
using Memoria.Assets;
using Memoria.Prime;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class QuadMistGame : MonoBehaviour
{
    private readonly List<QuadMistCard> _selectableTargets = new List<QuadMistCard>();
    private readonly List<QuadMistCard> _beatableTargets = new List<QuadMistCard>();
    private readonly BattleResult _battleResult = new BattleResult();
    private readonly Vector3 _selectedCardDeltaPos = new Vector3(0.12f, 0.0f, -1.5f);
    private QuadMistCard[] _adjacentTargets = new QuadMistCard[0];

    public const Int32 MINIGAME_MAP_TORENO_0 = 124;
    public const Int32 MINIGAME_MAP_TORENO_1 = 125;
    public const Int32 MINIGAME_MAP_TORENO_2 = 126;
    public const Int32 MINIGAME_MAP_TORENO_3 = 127;
    public const Int32 MINIINT_SONG_DEFAULT = 66;
    private const Int32 QuitFrame = 30;

    public TextMesh winScore;
    public TextMesh loseScore;
    public TextMesh drawScore;
    public TextMesh cardTypeCount;
    public TextMesh cardStockCount;
    public static QuadMistGame main;
    public GAME_STATE GameState;
    public PREGAME_STATE PreGameState;
    public START_STATE StartState;
    public PLAY_STATE PlayState;
    public END_STATE EndState;
    public POSTGAME_STATE PostState;
    public INPUT_STATE InputState;
    public InputResult inputResult;
    public InputHandler playerInput;
    public InputHandler enemyInput;
    public PreGame preGame;
    public PlayGame playGame;
    public Collection collection;
    private QuadMistCard StackCardInfo;
    private Int32 StackCardCount;
    public Hand playerHand;
    public Hand enemyHand;
    public MatchResult matchResult;
    public QuadMistCardUI movedCard;
    public SpriteDisplay getCardMessage;
    public GameObject CardNameToggleButton;
    public SpriteRenderer CardNameToggleButtonRenderer;
    public SpriteRenderer CardNameToggleButtonRendererHilight;
    public Boolean isAnimating;
    public Boolean yourTurn;
    public Int32 numberOfBlocks;
    private QuadMistCard[] matchCards;
    private Int32 playerScore;
    private Int32 enemyScore;
    public GameObject cardPrefab;
    private Int32 enemyHandSelectedCardIndex;
    public QuadMistCardNameDialogSlider CardNameDialogSlider;
    public Boolean IsPause;
    public Single TimeScaleBeforePause;
    public Boolean IsSeizingCard;
    private List<QuadMistCard> reservedCardList;
    private Int32 playerTurnCount;
    private Boolean hasShowTutorial02;
    private Boolean hasShowTutorial03;
    private QuadMistCard _t_selectedCard;

    public PreBoard preBoard => this.preGame.preBoard;
    public Board board => this.playGame.board;
    public Coin coin => this.playGame.coin;
    public Combo combo => this.playGame.combo;
    public Score score => this.playGame.score;
    public ResultText resultText => this.playGame.result;
    public SpriteDisplay bomb => this.playGame.bomb;
    public SpriteText[] battleNumber => this.playGame.battleNumber;

    public Int32 PlayerScore
    {
        get { return this.playerScore; }
        set
        {
            this.score.PlayerScore = value;
            this.playerScore = value;
        }
    }

    public Int32 EnemyScore
    {
        get { return this.enemyScore; }
        set
        {
            this.score.EnemyScore = value;
            this.enemyScore = value;
        }
    }

    public static void UpdateSelectedCardList(List<QuadMistCard> selectedCard)
    {
        QuadMistGame.main.playerHand.Clear();
        using (List<QuadMistCard>.Enumerator enumerator = selectedCard.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                QuadMistCard current = enumerator.Current;
                QuadMistGame.main.playerHand.Add(current);
            }
        }
    }

    public static void OnFinishSelectCardUI(List<QuadMistCard> selectedCard)
    {
        QuadMistGame.main.GameState = GAME_STATE.START;
    }

    private void Start()
    {
        QuadMistGame.main = this;
        this.InitResources();
        this.preBoard.InitResources();
        this.board.InitResources();
        this.ClearHands();
        this.ClearGameObjects();
        this.ClearStates();
        this.ClearInput();
        FF9Snd.ff9minisnd_song_play(66);
        this.StackCardInfo = new QuadMistCard();
        this.StackCardCount = 0;
    }

    private void InitResources()
    {
        GameObject go = Object.Instantiate(this.cardPrefab);
        this.movedCard = go.GetComponent<QuadMistCardUI>();
        go.name = "movedCard";
        go.transform.parent = this.transform;
        go.transform.localPosition = new Vector3(1.6f, -1.12f, 0.21865f);
        go.SetActive(false);
    }

    public void Pause()
    {
        this.IsPause = true;
        this.TimeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0.0f;
        this.playerHand.Select = -1;
        this.IsSeizingCard = false;
    }

    public void Resume()
    {
        this.IsPause = false;
        Time.timeScale = this.TimeScaleBeforePause;
    }

    private void Update()
    {
        if (!this.isAnimating && !this.IsPause)
        {
            switch (this.GameState)
            {
                case GAME_STATE.PREGAME:
                    this.PreGame();
                    break;
                case GAME_STATE.START:
                    this.StartGame();
                    break;
                case GAME_STATE.PLAY:
                    this.PlayGame();
                    break;
                case GAME_STATE.END:
                    this.EndGame();
                    break;
                case GAME_STATE.POSTGAME:
                    this.PostGame();
                    break;
            }
            SpriteRenderer component = this.CardNameToggleButton.GetComponent<SpriteRenderer>();
            if (component != null)
                component.sprite = !this.CardNameDialogSlider.IsShowCardName ? this.CardNameToggleButtonRenderer.sprite : this.CardNameToggleButtonRendererHilight.sprite;
        }
        SceneDirector.ServiceFade();
    }

    private Boolean ShouldShowTutorial()
    {
        return QuadMistDatabase.GetWinCount() == 0 && QuadMistDatabase.GetLoseCount() == 0 && QuadMistDatabase.GetDrawCount() == 0;
    }

    private void onFinishTutorial01()
    {
        this.PreGameState = PREGAME_STATE.SELECT_COLLECTION;
    }

    private void onFinishTutorial02()
    {
        this.PlayState = PLAY_STATE.INPUT_PLAYER;
    }

    private void onFinishTutorial03()
    {
        this.PlayState = PLAY_STATE.INPUT_PLAYER;
    }

    private void PreGame()
    {
        this.inputResult.Clear();
        switch (this.PreGameState)
        {
            case PREGAME_STATE.SETUP:
                this.PreGameState = PREGAME_STATE.SETUP_DONE;
                this.enemyHand.State = Hand.STATE.ENEMY_HIDE;
                this.playerHand.State = Hand.STATE.PLAYER_PREGAME;
                QuadMistDatabase.LoadData();
                QuadMistDatabase.CreateDataIfLessThanFive();
                this.reservedCardList = QuadMistDatabase.GetCardList();
                this.preBoard.collection = this.collection;
                this.preBoard.collection.CreateCards();
                this.preBoard.UpdateCollection(-1);
                this.preBoard.SetPreviewCardID(0);
                this.winScore.text = String.Empty + QuadMistDatabase.GetWinCount();
                this.loseScore.text = String.Empty + QuadMistDatabase.GetLoseCount();
                this.drawScore.text = String.Empty + QuadMistDatabase.GetDrawCount();
                Int32 totalCount = 0;
                Int32 typeCount = 0;
                foreach (List<QuadMistCard> card in this.preBoard.collection.cards)
                {
                    if (card.Count > 0)
                    {
                        typeCount++;
                        totalCount += card.Count;
                    }
                }
                this.cardTypeCount.text = String.Empty + typeCount;
                this.cardStockCount.text = String.Empty + totalCount;
                PersistenSingleton<UIManager>.Instance.QuadMistScene.State = QuadMistUI.CardState.CardSelection;
                PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.QuadMist);
                if (this.ShouldShowTutorial())
                    this.PreGameState = PREGAME_STATE.SHOW_TUTORIAL;
                QuadMistDatabase.MiniGame_ContinueInit();
                SceneDirector.FF9Wipe_FadeInEx(30);
                break;
            case PREGAME_STATE.SHOW_TUTORIAL:
                TutorialUI tutorialScene = PersistenSingleton<UIManager>.Instance.TutorialScene;
                tutorialScene.DisplayMode = TutorialUI.Mode.QuadMist;
                tutorialScene.QuadmistTutorialID = 1;
                tutorialScene.AfterFinished = this.onFinishTutorial01;
                PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Tutorial);
                this.PreGameState = PREGAME_STATE.TUTORIAL_01;
                break;
            case PREGAME_STATE.SELECT_COLLECTION:
                if (!this.inputResult.IsValid())
                    break;
                QuadMistConfirmDialog.MessageShow(new Vector3(0.0f, 0.0f, 0.0f), "Confirm Selection", true, true);
                ++this.PreGameState;
                break;
            case PREGAME_STATE.CONFIRM_DIALOG:
                this.playerInput.HandleDialog(ref this.inputResult);
                if (!this.inputResult.IsValid())
                    break;
                if (QuadMistConfirmDialog.IsOK)
                {
                    this.preGame.gameObject.SetActive(false);
                    this.GameState = GAME_STATE.START;
                }
                else
                    this.PreGameState = PREGAME_STATE.SELECT_COLLECTION;
                QuadMistConfirmDialog.MessageHide();
                break;
        }
    }

    private void StartGame()
    {
        if (PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI)
            return;
        switch (this.StartState)
        {
            case START_STATE.SETUP:
                Int32 num = Random.Range(0, 100);
                this.numberOfBlocks = num < 0 || num >= 3 ? (num < 3 || num >= 7 ? (num < 7 || num >= 12 ? (num < 12 || num >= 17 ? (num < 17 || num >= 27 ? (num < 27 || num >= 97 ? 6 : 5) : 4) : 3) : 2) : 1) : 0;
                EnemyData.Setup(this.enemyHand);
                if (this.StackCardCount != 0)
                    EnemyData.RestorePlayerLostCard(this.enemyHand, Random.Range(0, 4), this.StackCardInfo);
                this.enemyHand.HideCardCursor();
                this.matchCards = new QuadMistCard[10];
                for (Int32 index = 0; index < this.matchCards.Length; ++index)
                    this.matchCards[index] = index >= 5 ? this.enemyHand[index - 5] : this.playerHand[index];
                this.UpdateScore();
                this.board.Clear();
                this.playGame.gameObject.SetActive(true);
                ++this.StartState;
                this.PlayerScore = 0;
                this.EnemyScore = 0;
                break;
            case START_STATE.SETUP_BOARD:
                this.AnimCoroutine(Anim.Enable(this.board.gameObject), this.board.FadeInBoard());
                ++this.StartState;
                break;
            case START_STATE.SETUP_FIELD:
                this.enemyHand.State = Hand.STATE.ENEMY_SHOW;
                this.AnimCoroutine(this.board.ScaleInBlocks(this.numberOfBlocks));
                ++this.StartState;
                break;
            case START_STATE.COIN:
                Int32 side = Random.Range(0, 2);
                this.PlayState = side != 0 ? PLAY_STATE.INPUT_ENEMY : PLAY_STATE.INPUT_PLAYER;
                this.yourTurn = side == 0;
                this.AnimCoroutine(Anim.Enable(this.coin.gameObject), this.coin.Toss(side), Anim.Disable(this.coin.gameObject));
                this.playerTurnCount = 0;
                this.hasShowTutorial02 = false;
                this.hasShowTutorial03 = false;
                this.GameState = GAME_STATE.PLAY;
                this.enemyHand.State = Hand.STATE.ENEMY_WAIT;
                this.playerHand.State = Hand.STATE.PLAYER_WAIT;
                this.playerHand.HideShadowCard();
                QuadMistGame.main.CardNameDialogSlider.IsShowCardName = false;
                QuadMistGame.main.CardNameDialogSlider.HideCardNameDialog(this.playerHand);
                if (this.yourTurn)
                {
                    this.playerHand.State = Hand.STATE.PLAYER_SELECT_CARD;
                    break;
                }
                this.enemyHand.State = Hand.STATE.ENEMY_PLAY;
                break;
        }
    }

    private void PlayGame()
    {
        if (PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI)
            return;
        if (UIManager.Input.GetKeyTrigger(Control.RightBumper) && this.CardNameDialogSlider.IsReady)
        {
            this.CardNameDialogSlider.IsShowCardName = !this.CardNameDialogSlider.IsShowCardName;
            if (this.CardNameDialogSlider.IsShowCardName)
                this.CardNameDialogSlider.ShowCardNameDialog(this.playerHand);
            else
                this.CardNameDialogSlider.HideCardNameDialog(this.playerHand);
            SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
        }
        switch (this.PlayState)
        {
            case PLAY_STATE.INPUT_PLAYER:
                if (this.playerTurnCount == 0 && !this.hasShowTutorial02 && (this.InputState == INPUT_STATE.SELECT_CARD && this.ShouldShowTutorial()))
                {
                    TutorialUI tutorialScene = PersistenSingleton<UIManager>.Instance.TutorialScene;
                    tutorialScene.DisplayMode = TutorialUI.Mode.QuadMist;
                    tutorialScene.QuadmistTutorialID = 2;
                    tutorialScene.AfterFinished = this.onFinishTutorial02;
                    PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Tutorial);
                    this.PlayState = PLAY_STATE.TUTORIAL_02;
                    this.hasShowTutorial02 = true;
                    break;
                }
                if (this.playerTurnCount == 1 && !this.hasShowTutorial03 && (this.InputState == INPUT_STATE.SELECT_CARD && this.ShouldShowTutorial()))
                {
                    TutorialUI tutorialScene = PersistenSingleton<UIManager>.Instance.TutorialScene;
                    tutorialScene.DisplayMode = TutorialUI.Mode.QuadMist;
                    tutorialScene.QuadmistTutorialID = 3;
                    tutorialScene.AfterFinished = this.onFinishTutorial03;
                    PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Tutorial);
                    this.PlayState = PLAY_STATE.TUTORIAL_03;
                    this.hasShowTutorial03 = true;
                    break;
                }
                this.InputPlayer();
                break;
            case PLAY_STATE.INPUT_ENEMY:
                if (UIManager.Input.GetKeyTrigger(Control.Confirm))
                    SoundEffect.Play(QuadMistSoundID.MINI_SE_WARNING);
                this.InputEnemy();
                break;
            case PLAY_STATE.CALCULATE_BATTLE:
                this.enemyHand.State = Hand.STATE.ENEMY_WAIT;
                this.playerHand.State = Hand.STATE.PLAYER_WAIT;
                if (this._battleResult.defender != null)
                {
                    QuadMistCard attacker = this._battleResult.attacker;
                    QuadMistCard defender = this._battleResult.defender;
                    this._battleResult.calculation = this.Calculate(attacker, defender);
                    this._battleResult.type = this._battleResult.calculation.atkFinish <= this._battleResult.calculation.defFinish ? BattleResult.Type.LOSE : BattleResult.Type.WIN;
                    if (this._battleResult.type == BattleResult.Type.WIN)
                    {
                        this._battleResult.combos = this.GenerateCombo(this._battleResult.defender, attacker.side);
                        this.RemoveComboFromBeatable(this._battleResult.combos);
                    }
                    else
                    {
                        this._battleResult.combos = this.GenerateCombo(this._battleResult.attacker, defender.side);
                        this._beatableTargets.Clear();
                    }
                }
                else
                    this._battleResult.type = BattleResult.Type.NOTHING;
                this._battleResult.beats = this._beatableTargets;
                this.AnimCoroutine(this.BattleAnimation(this._battleResult));
                this.PlayState = PLAY_STATE.ANIMATE_BATTLE;
                break;
            case PLAY_STATE.ANIMATE_BATTLE:
                if (this._battleResult.type == BattleResult.Type.WIN && this._selectableTargets.Count > 1)
                {
                    foreach (QuadMistCard card in this._battleResult.combos)
                        this._selectableTargets.Remove(card);
                    this._selectableTargets.Remove(this._battleResult.defender);
                    this.PlayState = PLAY_STATE.INPUT_PLAYER;
                    break;
                }
                this.InputState = INPUT_STATE.SELECT_CARD;
                this.yourTurn = !this.yourTurn;
                this.PlayState = !this.yourTurn ? PLAY_STATE.INPUT_ENEMY : PLAY_STATE.INPUT_PLAYER;
                this._battleResult.defender = null;
                if (this.yourTurn)
                {
                    this.playerHand.State = Hand.STATE.PLAYER_SELECT_CARD;
                    if (QuadMistGame.main.CardNameDialogSlider.IsShowCardName)
                        QuadMistGame.main.CardNameDialogSlider.ShowCardNameDialog(this.playerHand);
                    else
                        QuadMistGame.main.CardNameDialogSlider.HideCardNameDialog(this.playerHand);
                }
                else
                    this.enemyHand.State = Hand.STATE.ENEMY_PLAY;
                if (this.EnemyScore + this.PlayerScore == 10)
                    ++this.GameState;
                this.playerHand.HideShadowCard();
                break;
        }
    }

    private void EndGame()
    {
        this.inputResult.Clear();
        switch (this.EndState)
        {
            case END_STATE.RESULT:
                this.matchResult = new MatchResult {perfect = this.PlayerScore == 10 || this.EnemyScore == 10};
                this.PostState = !this.matchResult.perfect ? POSTGAME_STATE.SELECT_CARD : POSTGAME_STATE.PERFECT_SELECT_CARD;
                if (this.PlayerScore > this.EnemyScore)
                {
                    this.matchResult.type = MatchResult.Type.WIN;
                    for (Int32 index = 5; index < 10; ++index)
                    {
                        if (this.matchCards[index].side == 0)
                            this.matchResult.selectable.Add(index - 5);
                    }
                    QuadMistDatabase.SetWinCount((Int16)(QuadMistDatabase.GetWinCount() + 1));
                }
                else if (this.EnemyScore > this.PlayerScore)
                {
                    this.matchResult.type = MatchResult.Type.LOSE;
                    for (Int32 index = 0; index < 5; ++index)
                    {
                        if (this.matchCards[index].side == 1)
                            this.matchResult.selectable.Add(index);
                    }
                    QuadMistDatabase.SetLoseCount((Int16)(QuadMistDatabase.GetLoseCount() + 1));
                }
                else
                {
                    this.matchResult.type = MatchResult.Type.DRAW;
                    this.PostState = POSTGAME_STATE.PRE_REMATCH;
                    QuadMistDatabase.SetDrawCount((Int16)(QuadMistDatabase.GetDrawCount() + 1));
                }
                if (this.matchResult.type == MatchResult.Type.WIN)
                    QuadMistDatabase.MiniGame_SetLastBattleResult(0);
                else if (this.matchResult.type == MatchResult.Type.LOSE)
                    QuadMistDatabase.MiniGame_SetLastBattleResult(1);
                else if (this.matchResult.type == MatchResult.Type.DRAW)
                    QuadMistDatabase.MiniGame_SetLastBattleResult(2);
                this.AnimCoroutine(this.ResultText(this.matchResult));
                this.EndState = END_STATE.CONFIRM;
                break;
            case END_STATE.CONFIRM:
                this.playerInput.HandleConfirmation(ref this.inputResult);
                if (!this.inputResult.IsValid())
                    break;
                if (QuadMistGame.main.CardNameDialogSlider.IsShowCardName)
                    QuadMistGame.main.CardNameDialogSlider.ShowCardNameDialog(this.playerHand);
                else
                    QuadMistGame.main.CardNameDialogSlider.HideCardNameDialog(this.playerHand);
                this.resultText.gameObject.SetActive(false);
                if (this.PostState != POSTGAME_STATE.PRE_REMATCH)
                    this.AnimCoroutine(this.ResultRestoreHands());
                this.playGame.gameObject.SetActive(false);
                this.playGame.background.color = new Color(1f, 1f, 1f, 0.0f);
                ++this.GameState;
                break;
        }
    }

    private void CheckAndClearStackCard()
    {
        //Debug.Log((object)("CheckAndClearStackCard: StackCardCount = " + (object)this.StackCardCount + ", Stack"));
        if (this.StackCardCount == 1 && this.StackCardInfo.isTheSameCard(this.matchResult.selectedCard))
        {
            //Debug.Log((object)"CheckAndClearStackCard: RESET StackCardCount(taken card) here.");
            this.StackCardCount = 0;
        }
        else
        {
            //Debug.Log((object)"CheckAndClearStackCard: NO NEED to reset StackCardCount(taken card) here.");
        }
    }

    private void PostGame()
    {
        this.inputResult.Clear();
        switch (this.PostState)
        {
            case POSTGAME_STATE.SELECT_CARD:
                if (this.matchResult.type == MatchResult.Type.WIN)
                {
                    this.playerInput.HandlePostSelection(this.enemyHand, this.matchResult.selectable, ref this.inputResult);
                    this.enemyHand.ShowCardCursor();
                    if (!this.inputResult.IsValid() && this.matchResult.selectable.Count != 1)
                        break;
                    if (this.matchResult.selectable.Count == 1)
                    {
                        this.inputResult.selectedCard = this.enemyHand[this.matchResult.selectable[0]];
                        this.inputResult.selectedHandIndex = this.matchResult.selectable[0];
                    }
                    this.matchResult.selectedCard = this.inputResult.selectedCard;
                    if (this._t_selectedCard != null)
                        this.enemyHand.GetCardUI(this.enemyHandSelectedCardIndex).transform.localPosition -= this._selectedCardDeltaPos;
                    this.enemyHandSelectedCardIndex = this.inputResult.selectedHandIndex;
                    QuadMistCardUI cardUi = this.enemyHand.GetCardUI(this.enemyHandSelectedCardIndex);
                    cardUi.transform.localPosition += this._selectedCardDeltaPos;
                    this.enemyHand.UpdateEnemyCardCursorToPosition(cardUi.transform.localPosition);
                    if (this._t_selectedCard != null && this._t_selectedCard == this.inputResult.selectedCard)
                    {
                        this.enemyHand.HideCardCursor();
                        this.AnimCoroutine(this.ChangeCardToCenter(this.matchResult));
                        QuadMistDatabase.Add(this.inputResult.selectedCard);
                        QuadMistStockDialog.Hide();
                        this._t_selectedCard = null;
                        this.PostState = POSTGAME_STATE.CONFIRM;
                        this.CheckAndClearStackCard();
                        break;
                    }
                    this._t_selectedCard = this.inputResult.selectedCard;
                    Int32 cardCount = QuadMistDatabase.GetCardCount(this.matchResult.selectedCard);
                    Vector3 position = cardUi.transform.position;
                    QuadMistStockDialog.Show(new Vector3(position.x + 0.9482538f, position.y - 0.2696013f, 0.0f), Localization.Get("QuadMistStock").Replace("[NUMBER]", cardCount.ToString()));
                    break;
                }
                this.enemyInput.HandlePostSelection(this.playerHand, this.matchResult.selectable, ref this.inputResult);
                this.matchResult.selectedCard = this.inputResult.selectedCard;
                this.AnimCoroutine(this.ChangeCardToCenter(this.matchResult));
                this.PostState = POSTGAME_STATE.CONFIRM;
                QuadMistDatabase.Remove(this.matchResult.selectedCard);
                QuadMistCard selectedCard1 = this.matchResult.selectedCard;
                this.StackCardInfo.id = selectedCard1.id;
                this.StackCardInfo.atk = selectedCard1.atk;
                this.StackCardInfo.arrow = selectedCard1.arrow;
                this.StackCardInfo.type = selectedCard1.type;
                this.StackCardInfo.pdef = selectedCard1.pdef;
                this.StackCardInfo.mdef = selectedCard1.mdef;
                this.StackCardCount = 1;
                break;
            case POSTGAME_STATE.PERFECT_SELECT_CARD:
                if (this.matchResult.type == MatchResult.Type.WIN)
                {
                    if (this.enemyHand.Count != 0)
                    {
                        this.matchResult.selectedCard = this.enemyHand[0];
                        this.AnimCoroutine(this.ChangeCardToCenter2(this.matchResult));
                        QuadMistDatabase.Add(this.matchResult.selectedCard);
                        this.CheckAndClearStackCard();
                        this.PostState = POSTGAME_STATE.CONFIRM;
                        break;
                    }
                    this.PostState = POSTGAME_STATE.PRE_REMATCH;
                    break;
                }
                if (this.playerHand.Count != 0)
                {
                    this.matchResult.selectedCard = this.playerHand[0];
                    this.AnimCoroutine(this.ChangeCardToCenter2(this.matchResult));
                    this.PostState = POSTGAME_STATE.CONFIRM;
                    QuadMistDatabase.Remove(this.matchResult.selectedCard);
                    QuadMistCard selectedCard2 = this.matchResult.selectedCard;
                    this.StackCardInfo.id = selectedCard2.id;
                    this.StackCardInfo.atk = selectedCard2.atk;
                    this.StackCardInfo.arrow = selectedCard2.arrow;
                    this.StackCardInfo.type = selectedCard2.type;
                    this.StackCardInfo.pdef = selectedCard2.pdef;
                    this.StackCardInfo.mdef = selectedCard2.mdef;
                    this.StackCardCount = 1;
                    break;
                }
                this.PostState = POSTGAME_STATE.PRE_REMATCH;
                break;
            case POSTGAME_STATE.CONFIRM:
                this.playerInput.HandleConfirmation(ref this.inputResult);
                if (!this.inputResult.IsValid())
                    break;
                if (this.matchResult.perfect)
                {
                    this.AnimCoroutine(this.ChangeCardToHand(this.matchResult));
                    this.PostState = POSTGAME_STATE.PERFECT_SELECT_CARD;
                }
                else
                {
                    this.AnimCoroutine(this.ChangeCardToHand(this.matchResult));
                    this.PostState = POSTGAME_STATE.PRE_REMATCH;
                }
                QuadMistGetCardDialog.Hide();
                break;
            case POSTGAME_STATE.PRE_REMATCH:
                if (this.matchResult.type == MatchResult.Type.DRAW)
                    this.SaveReservedCardToDatabase();
                else
                    this.SaveCardToDatabase();
                this.PostState = POSTGAME_STATE.REMATCH;
                break;
            case POSTGAME_STATE.REMATCH:
                this.Rematch();
                break;
        }
    }

    private void onRematchDialogHidden(Int32 choice)
    {
        this.PostState = POSTGAME_STATE.REMATCH_CONFIRM;
        ButtonGroupState.SetPointerOffsetToGroup(Dialog.DefaultOffset, Dialog.DialogGroupButton);
        Boolean flag = FF9StateSystem.Common.FF9.miniGameArg == 124 || FF9StateSystem.Common.FF9.miniGameArg == 125 || FF9StateSystem.Common.FF9.miniGameArg == 126 || FF9StateSystem.Common.FF9.miniGameArg == SByte.MaxValue;
        if (!flag && choice == 0 || flag && choice == -1)
        {
            this.board.BoardCursor.ForceHide();
            this.RestoreCollection();
            this.ClearHands();
            this.ClearGameObjects();
            this.ClearInput();
            this.ClearStates();
        }
        else
            this.QuitQuadMist();
    }

    public void QuitQuadMist()
    {
        SceneDirector.FF9Wipe_FadeOutEx(30);
        this.StartCoroutine(this.QuitQuadMistTransition(30f / Application.targetFrameRate));
    }

    [DebuggerHidden]
    private IEnumerator QuitQuadMistTransition(Single duration)
    {
        Single curTime = 0f;

        while (curTime < duration)
        {
            curTime += Time.deltaTime;
            yield return null;
        }

        FF9Snd.ff9minisnd_song_stop(66);
        SceneDirector.Replace(PersistenSingleton<SceneDirector>.Instance.LastScene, SceneTransition.FadeOutToBlack_FadeIn, true);
    }

    private void OnDontHave5CardsDialog(Int32 choice)
    {
        this.PostState = POSTGAME_STATE.REMATCH_CONFIRM;
        if (this.matchResult.type == MatchResult.Type.DRAW)
            this.SaveReservedCardToDatabase();
        else
            this.SaveCardToDatabase();
        this.QuitQuadMist();
    }

    public static void OnDiscardFinish()
    {
        QuadMistGame.main.playerHand.gameObject.SetActive(true);
        QuadMistGame.main.enemyHand.gameObject.SetActive(true);
        QuadMistGame.main.PostState = POSTGAME_STATE.REMATCH;
    }

    private void Rematch()
    {
        if (this.matchResult.type == MatchResult.Type.DRAW && (FF9StateSystem.Common.FF9.miniGameArg == 124 || FF9StateSystem.Common.FF9.miniGameArg == 125 || (FF9StateSystem.Common.FF9.miniGameArg == 126 || FF9StateSystem.Common.FF9.miniGameArg == SByte.MaxValue)))
        {
            if (PersistenSingleton<UIManager>.Instance.Dialogs.CheckDialogShowing(1))
                return;
            Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get("QuadMistTournamentDraw"), 110, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(0.0f, 0.0f), Dialog.CaptionType.None);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(265f, 0.0f), Dialog.DialogGroupButton);
            dialog.AfterDialogHidden = this.onRematchDialogHidden;
            dialog.Id = 1;
        }
        else if (QuadMistDatabase.MiniGame_GetAllCardCount() > 100)
        {
            PersistenSingleton<UIManager>.Instance.QuadMistScene.State = QuadMistUI.CardState.CardDestroy;
            PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.QuadMist);
            this.playerHand.gameObject.SetActive(false);
            this.enemyHand.gameObject.SetActive(false);
            this.PostState = POSTGAME_STATE.DISCARD;
        }
        else
        {
            if (PersistenSingleton<UIManager>.Instance.Dialogs.CheckDialogShowing(1))
                return;
            if (FF9StateSystem.Common.FF9.miniGameArg == 124 || FF9StateSystem.Common.FF9.miniGameArg == 125 || (FF9StateSystem.Common.FF9.miniGameArg == 126 || FF9StateSystem.Common.FF9.miniGameArg == SByte.MaxValue))
                this.onRematchDialogHidden(1);
            else if ((this.matchResult.type != MatchResult.Type.DRAW ? this.playerHand.GetQuadMistCards().Count + QuadMistUI.allCardList.Count : this.reservedCardList.Count) < 5)
            {
                Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get("QuadMistYouDontHave5Cards"), 124, 2, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(0.0f, 0.0f), Dialog.CaptionType.None);
                dialog.AfterDialogHidden = this.OnDontHave5CardsDialog;
                dialog.Id = 1;
                SoundEffect.Play(QuadMistSoundID.MINI_SE_WARNING);
            }
            else
            {
                Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get("QuadMistRematch"), 110, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(0.0f, 0.0f), Dialog.CaptionType.None);
                ButtonGroupState.SetPointerOffsetToGroup(new Vector2(265f, 0.0f), Dialog.DialogGroupButton);
                dialog.AfterDialogHidden = this.onRematchDialogHidden;
                dialog.Id = 1;
            }
        }
    }

    private void SaveReservedCardToDatabase()
    {
        QuadMistDatabase.SetCardList(this.reservedCardList);
        QuadMistDatabase.SaveData();
    }

    private void SaveCardToDatabase()
    {
        List<QuadMistCard> quadMistCards = this.playerHand.GetQuadMistCards();
        List<QuadMistCard> cards = new List<QuadMistCard>();
        using (List<QuadMistCard>.Enumerator enumerator = quadMistCards.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                QuadMistCard current = enumerator.Current;
                cards.Add(current);
            }
        }
        using (List<QuadMistCard>.Enumerator enumerator = QuadMistUI.allCardList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                QuadMistCard current = enumerator.Current;
                cards.Add(current);
            }
        }
        QuadMistDatabase.SetCardList(cards);
        QuadMistDatabase.SaveData();
        if (this.matchResult.type != MatchResult.Type.WIN)
            return;
        EMinigame.QuadmistWinAllNPCAchievement();
        EMinigame.GetWinQuadmistAchievement();
    }

    private void RestoreCollection()
    {
        if (this.matchResult.type == MatchResult.Type.DRAW)
        {
            for (Int32 index = 0; index < 5; ++index)
                this.collection.Add(this.matchCards[index]);
        }
        else
        {
            foreach (QuadMistCard c in this.playerHand)
                this.collection.Add(c);
        }
    }

    private void InputPlayer()
    {
        this.inputResult.Clear();
        switch (this.InputState)
        {
            case INPUT_STATE.SELECT_CARD:
                this.playerInput.HandleYourCardSelection(this.board, this.playerHand, ref this.inputResult);
                if (!this.inputResult.IsValid())
                    break;
                this.PlaceCard(this.inputResult.x, this.inputResult.y, this.inputResult.selectedCard);
                this.GenerateTargetable(this.inputResult.x, this.inputResult.y);
                this._battleResult.attacker = this.inputResult.selectedCard;
                ++this.InputState;
                ++this.playerTurnCount;
                break;
            case INPUT_STATE.SELECT_BATTLE_TARGET:
                if (this._selectableTargets.Count > 1)
                {
                    this.playerInput.HandleTargetCardSelection(this.board, this._battleResult.attacker, this._selectableTargets, ref this.inputResult);
                    if (!this.inputResult.IsValid())
                        break;
                    this._battleResult.defender = this._selectableTargets[this.inputResult.index];
                    this.PlayState = PLAY_STATE.CALCULATE_BATTLE;
                    break;
                }
                if (this._selectableTargets.Count == 1)
                {
                    this._battleResult.defender = this._selectableTargets[0];
                    this.PlayState = PLAY_STATE.CALCULATE_BATTLE;
                    break;
                }
                this._battleResult.defender = null;
                this.PlayState = PLAY_STATE.CALCULATE_BATTLE;
                break;
        }
    }

    private void InputEnemy()
    {
        this.inputResult.Clear();
        switch (this.InputState)
        {
            case INPUT_STATE.SELECT_CARD:
                this.enemyInput.HandleYourCardSelection(this.board, this.enemyHand, ref this.inputResult);
                if (!this.inputResult.IsValid())
                    break;
                this.PlaceCard(this.inputResult.x, this.inputResult.y, this.inputResult.selectedCard);
                this.GenerateTargetable(this.inputResult.x, this.inputResult.y);
                this._battleResult.attacker = this.inputResult.selectedCard;
                ++this.InputState;
                break;
            case INPUT_STATE.SELECT_BATTLE_TARGET:
                this.enemyInput.HandleTargetCardSelection(this.board, this._battleResult.attacker, this._selectableTargets, ref this.inputResult);
                if (!this.inputResult.IsValid())
                    break;
                this._battleResult.defender = this._selectableTargets.Count != 0 ? this._selectableTargets[this.inputResult.index] : null;
                this.PlayState = PLAY_STATE.CALCULATE_BATTLE;
                break;
        }
    }

    public void GenerateTargetable(Int32 x, Int32 y)
    {
        this._adjacentTargets = this.board.GetAdjacentCards(x, y);
        this._selectableTargets.Clear();
        this._beatableTargets.Clear();
        for (Int32 index = 0; index < CardArrow.MAX_ARROWNUM; ++index)
        {
            CardArrow.Type direction = (CardArrow.Type)index;
            QuadMistCard adjacentTarget = this._adjacentTargets[index];
            if (adjacentTarget != null && adjacentTarget.side != this.inputResult.selectedCard.side)
            {
                Int32 num = CardArrow.CheckDirection(this.inputResult.selectedCard.arrow, adjacentTarget.arrow, direction);
                if (num == 2)
                    this._selectableTargets.Add(this._adjacentTargets[index]);
                if (num == 1)
                    this._beatableTargets.Add(this._adjacentTargets[index]);
            }
        }
    }

    public void RemoveComboFromBeatable(QuadMistCard[] removal)
    {
        foreach (QuadMistCard quadMistCard in removal)
        {
            if (quadMistCard != null)
                this._beatableTargets.Remove(quadMistCard);
        }
    }

    public QuadMistCard[] GenerateCombo(QuadMistCard card, Int32 sideOf)
    {
        QuadMistCard[] adjacentCards = this.board.GetAdjacentCards(card);
        QuadMistCard quadMistCard1 = card;
        for (Int32 index = 0; index < adjacentCards.Length; ++index)
        {
            QuadMistCard quadMistCard2 = adjacentCards[index];
            if (quadMistCard2 != null && quadMistCard2.side != sideOf)
            {
                if (CardArrow.CheckDirection(quadMistCard1.arrow, quadMistCard2.arrow, (CardArrow.Type)index) <= 0)
                    adjacentCards[index] = null;
            }
            else
                adjacentCards[index] = null;
        }
        return adjacentCards;
    }

    public void PlaceCard(Int32 x, Int32 y, QuadMistCard card)
    {
        this.board[x, y] = card;
        this.UpdateScore();
    }

    public void UpdateScore()
    {
        Int32 num1 = 0;
        Int32 num2 = 0;
        for (Int32 index = 0; index < Board.SIZE_Y * Board.SIZE_X; ++index)
        {
            if (this.board[index] != null && !this.board[index].IsBlock)
            {
                if (this.board[index].side == QuadMistCardUI.PLAYER_SIDE)
                    ++num1;
                else
                    ++num2;
            }
        }
        this.PlayerScore = num1;
        this.EnemyScore = num2;
    }

    public BattleCalculation Calculate(QuadMistCard attacker, QuadMistCard defender)
    {
        BattleCalculation battleCalculation = new BattleCalculation();
        switch (attacker.type)
        {
            case QuadMistCard.Type.PHYSICAL:
                battleCalculation.atkStart = attacker.atk;
                battleCalculation.defStart = defender.pdef;
                break;
            case QuadMistCard.Type.MAGIC:
                battleCalculation.atkStart = attacker.atk;
                battleCalculation.defStart = defender.mdef;
                break;
            case QuadMistCard.Type.FLEXIABLE:
                battleCalculation.atkStart = attacker.atk;
                battleCalculation.defStart = Mathf.Min(defender.pdef, defender.mdef);
                break;
            case QuadMistCard.Type.ASSAULT:
                battleCalculation.atkStart = Mathf.Max(attacker.atk, attacker.pdef, attacker.mdef);
                battleCalculation.defStart = Mathf.Min(defender.atk, defender.pdef, defender.mdef);
                break;
        }

        if (battleCalculation.atkStart == 0)
            battleCalculation.atkStart = 1;
        if (battleCalculation.defStart == 0)
            battleCalculation.defStart = 1;

        if (Configuration.TetraMaster.IsEasyWin)
        {
            if (attacker.side == 0) // Player
            {
                battleCalculation.atkFinish = 100;
                battleCalculation.defFinish = 0;
            }
            else
            {
                battleCalculation.atkFinish = 0;
                battleCalculation.defFinish = 100;
            }
        }
        else if (Configuration.TetraMaster.IsReduceRandom)
        {
            Single ak = 1 - attacker.ArrowNumber / 10.0f; // The attack is 20% less susceptible to randomness.
            Single dk = 1 - defender.ArrowNumber / 8.0f;

            Int32 lowestAttack = (Int32) (battleCalculation.atkStart * ak);
            Int32 lowestDefense = (Int32) (battleCalculation.defStart * dk);
            
            battleCalculation.atkFinish = Random.Range(lowestAttack, battleCalculation.atkStart);
            battleCalculation.defFinish = Random.Range(lowestDefense, battleCalculation.defStart);

            // Set the life of the loser to 0.
            if (battleCalculation.atkFinish > battleCalculation.defFinish)
                battleCalculation.defFinish = 0;
            else
                battleCalculation.atkFinish = 0;
        }
        else
        {
            battleCalculation.atkFinish = Random.Range(0, battleCalculation.atkStart);
            battleCalculation.defFinish = Random.Range(0, battleCalculation.defStart);
        }
        return battleCalculation;
    }

    public void SetGetCardMessage(Int32 type, Boolean active)
    {
        this.getCardMessage.ID = type;
        this.getCardMessage.gameObject.SetActive(active);
    }

    private void ClearHands()
    {
        this.enemyHand.Clear();
        this.playerHand.Clear();
    }

    private void ClearGameObjects()
    {
        this.preGame.gameObject.SetActive(false);
        this.playGame.gameObject.SetActive(false);
    }

    private void ClearStates()
    {
        this.GameState = GAME_STATE.PREGAME;
        this.PreGameState = PREGAME_STATE.SETUP;
        this.StartState = START_STATE.SETUP;
        this.PlayState = PLAY_STATE.INPUT_PLAYER;
        this.EndState = END_STATE.RESULT;
        this.PostState = POSTGAME_STATE.SELECT_CARD;
        this.InputState = INPUT_STATE.SELECT_CARD;
    }

    private void ClearInput()
    {
        this.inputResult = new InputResult();
    }

    public void AnimCoroutine(IEnumerator func)
    {
        this.StartCoroutine(this.InvokeCoroutine(func));
    }

    public void AnimCoroutine(params IEnumerator[] funcs)
    {
        this.StartCoroutine(this.InvokeCoroutine(Anim.Sequence(funcs)));
    }

    [DebuggerHidden]
    private IEnumerator InvokeCoroutine(IEnumerator func)
    {
        isAnimating = true;
        yield return StartCoroutine(func);
        isAnimating = false;
    }

    [DebuggerHidden]
    private IEnumerator ChangeCardToHand(MatchResult result)
    {
        QuadMistCard selected = result.selectedCard;
        SetGetCardMessage(0, false);

        if (result.type == MatchResult.Type.WIN)
        {
            Vector3 dest = playerHand.GetCardUI(playerHand.Count - 1).transform.position;
            yield return StartCoroutine(Anim.MoveLerp(movedCard.transform, dest, Anim.TickToTime(20), false));
            playerHand.Add(selected);
        }
        else if (result.type == MatchResult.Type.LOSE)
        {
            Vector3 dest = enemyHand.GetCardUI(enemyHand.Count - 1).transform.position;
            yield return StartCoroutine(Anim.MoveLerp(movedCard.transform, dest, Anim.TickToTime(20), false));
            enemyHand.Add(selected);
        }

        movedCard.gameObject.SetActive(false);
    }

    [DebuggerHidden]
    private IEnumerator ChangeCardToCenter(MatchResult result)
    {
        QuadMistCard selected = result.selectedCard;
        QuadMistCardUI cardUi = null;
        if (result.type == MatchResult.Type.WIN)
        {
            cardUi = enemyHand.GetCardUI(selected);
        }
        else if (result.type == MatchResult.Type.LOSE)
        {
            cardUi = playerHand.GetCardUI(selected);
        }
        movedCard.Data = cardUi.Data;
        movedCard.transform.position = cardUi.transform.position + new Vector3(0f, 0f, -4f);
        movedCard.gameObject.SetActive(true);
        if (result.type == MatchResult.Type.WIN)
        {
            enemyHand.Remove(selected);
            enemyHand.Select = -1;
        }
        else if (result.type == MatchResult.Type.LOSE)
        {
            playerHand.Remove(selected);
            playerHand.Select = -1;
        }
        StartCoroutine(RestoreCards(result));
        SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_GET);
        yield return StartCoroutine(Anim.MoveLerp(movedCard.transform, transform.TransformPoint(new Vector3(1.6f - movedCard.Size.x / 2f, -1.12f + movedCard.Size.y / 2f, -1f)), Anim.TickToTime(20), false));

        if (collection.GetCardsWithID(movedCard.Data.id).Count == 0)
        {
            if (result.type == MatchResult.Type.WIN)
            {
                SetGetCardMessage(0, true);
            }
            else if (result.type == MatchResult.Type.LOSE)
            {
                SetGetCardMessage(1, true);
            }
        }
        String resultTypeText = String.Empty;
        if (result.type == MatchResult.Type.WIN)
        {
            resultTypeText = Localization.Get("QuadMistReceived");
        }
        else if (result.type == MatchResult.Type.LOSE)
        {
            resultTypeText = Localization.Get("QuadMistLost");
        }
        if (resultTypeText != String.Empty)
        {
            String monsterName = Assets.Sources.Scripts.UI.Common.FF9TextTool.CardName(result.selectedCard.id);
            resultTypeText = resultTypeText.Replace("[CARD]", monsterName);
        }
        QuadMistGetCardDialog.Show(new Vector3(0f, -0.6f, 0f), resultTypeText);
    }

    [DebuggerHidden]
    private IEnumerator ChangeCardToCenter2(MatchResult result)
    {
        QuadMistCard selected = result.selectedCard;
        QuadMistCardUI cardUi = null;
        if (result.type == MatchResult.Type.WIN)
        {
            cardUi = enemyHand.GetCardUI(selected);
        }
        else if (result.type == MatchResult.Type.LOSE)
        {
            cardUi = playerHand.GetCardUI(selected);
        }
        movedCard.Data = cardUi.Data;
        movedCard.transform.position = cardUi.transform.position + new Vector3(0f, 0f, -4f);
        movedCard.gameObject.SetActive(true);
        if (result.type == MatchResult.Type.WIN)
        {
            enemyHand.Remove(selected);
        }
        else if (result.type == MatchResult.Type.LOSE)
        {
            playerHand.Remove(selected);
        }
        SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_GET);
        yield return StartCoroutine(Anim.MoveLerp(movedCard.transform, transform.TransformPoint(new Vector3(1.6f - movedCard.Size.x / 2f, -1.12f + movedCard.Size.y / 2f, -1f)), Anim.TickToTime(20), false));

        if (collection.GetCardsWithID(movedCard.Data.id).Count == 0)
        {
            if (result.type == MatchResult.Type.WIN)
            {
                SetGetCardMessage(0, true);
            }
            else if (result.type == MatchResult.Type.LOSE)
            {
                SetGetCardMessage(1, true);
            }
        }

        String resultTypeText = String.Empty;
        if (result.type == MatchResult.Type.WIN)
        {
            resultTypeText = Localization.Get("QuadMistReceived");
        }
        else if (result.type == MatchResult.Type.LOSE)
        {
            resultTypeText = Localization.Get("QuadMistLost");
        }
        if (resultTypeText != String.Empty)
        {
            String monsterName = Assets.Sources.Scripts.UI.Common.FF9TextTool.CardName(result.selectedCard.id);
            resultTypeText = resultTypeText.Replace("[CARD]", monsterName);
        }
        QuadMistGetCardDialog.Show(new Vector3(0f, -0.6f, 0f), resultTypeText);
    }

    [DebuggerHidden]
    private IEnumerator RestoreCards(MatchResult result)
    {
        Action<QuadMistCardUI> functor0 = c =>
        {
            c.Side = 0;
            if (result.type == MatchResult.Type.WIN)
            {
                c.Data.LevelUpInMatch();
            }
        };

        Action<QuadMistCardUI> functor1 = c => c.Side = 1;
        Int32 index = 0;
        while (index < playerHand.Count)
        {
            QuadMistCardUI cardUi = playerHand.GetCardUI(index);
            StartCoroutine(cardUi.FlashNormal(functor0));
            index++;
        }
        Int32 index2 = 0;
        while (index2 < enemyHand.Count)
        {
            QuadMistCardUI cardUi = enemyHand.GetCardUI(index2);
            StartCoroutine(cardUi.FlashNormal(functor1));
            index2++;
        }
        yield return StartCoroutine(Anim.Tick(CardEffect.FLASH_TICK_DURATION));
    }

    [DebuggerHidden]
    private IEnumerator ResultRestoreHands()
    {
        Int32 index = 0;
        while (index < matchCards.Length)
        {
            if (index < 5)
            {
                playerHand.GetCardUI(index).transform.position = board.GetCardUI(matchCards[index]).transform.position;
                playerHand.AddWithoutChanged(matchCards[index]);
            }
            else
            {
                enemyHand.GetCardUI(index - 5).transform.position = board.GetCardUI(matchCards[index]).transform.position;
                enemyHand.AddWithoutChanged(matchCards[index]);
            }
            index++;
        }
        enemyHand.State = Hand.STATE.ENEMY_POSTGAME;
        playerHand.State = Hand.STATE.PLAYER_POSTGAME;
        yield return StartCoroutine(Anim.Tick(30));
    }

    [DebuggerHidden]
    private IEnumerator ResultText(MatchResult result)
    {
        Single brightness = 0f;
        Int32 id = 0;
        QuadMistSoundID sound = QuadMistSoundID.MINI_SE_WIN;
        switch (result.type)
        {
            case MatchResult.Type.WIN:
                if (result.perfect)
                {
                    sound = QuadMistSoundID.MINI_SE_PARFECT;
                    id = 3;
                }
                else
                {
                    sound = QuadMistSoundID.MINI_SE_WIN;
                    id = 0;
                }
                break;
            case MatchResult.Type.LOSE:
                sound = QuadMistSoundID.MINI_SE_LOSE;
                id = 1;
                break;
            case MatchResult.Type.DRAW:
                sound = QuadMistSoundID.MINI_SE_DRAW;
                id = 2;
                break;
        }
        resultText.ID = id;
        resultText.Alpha = brightness;
        resultText.gameObject.SetActive(true);

        for (Int32 tick = 0; tick < 32; tick++)
        {
            brightness = tick * 8 / 255f;
            if (brightness > 1f)
            {
                brightness = 1f;
            }
            if (tick == 31)
            {
                SoundEffect.Play(sound);
            }
            resultText.Alpha = brightness;
            yield return StartCoroutine(Anim.Tick());
        }
    }

    [DebuggerHidden]
    private IEnumerator BattleAnimation(BattleResult result)
    {
        Int32 BATTLE_NUMBER_TIME = 35;
        Int32 BATTLE_COUNTDOWN_TIME = 55;
        if (result.type != BattleResult.Type.NOTHING)
        {
            QuadMistCardUI atkUi = board.GetCardUI(result.attacker);
            QuadMistCardUI defUi = board.GetCardUI(result.defender);
            StartCoroutine(BattleNumberTextAnimation(result, BATTLE_NUMBER_TIME + 32, BATTLE_COUNTDOWN_TIME + BATTLE_NUMBER_TIME, 45));
            yield return StartCoroutine(Anim.Tick(BATTLE_NUMBER_TIME));

            StartCoroutine(BattleMotionAnimation(atkUi, defUi, 7, 15));
            yield return StartCoroutine(BombAnimation(atkUi, defUi, 32));

            yield return StartCoroutine(Anim.Tick(53));

            if (result.type == BattleResult.Type.WIN)
            {
                Action<QuadMistCardUI> functor = d =>
                {
                    d.Side = atkUi.Side;
                    UpdateScore();
                };
                SoundEffect.Play(QuadMistSoundID.MINI_SE_FLASH);
                yield return StartCoroutine(defUi.FlashBattle(functor));

                atkUi.Data.LevelUpInBattle();
            }
            else
            {
                Action<QuadMistCardUI> functor = d =>
                {
                    d.Side = defUi.Side;
                    UpdateScore();
                };
                SoundEffect.Play(QuadMistSoundID.MINI_SE_FLASH);
                yield return StartCoroutine(atkUi.FlashBattle(functor));

                defUi.Data.LevelUpInBattle();
            }

            yield return StartCoroutine(ComboAnimation(result));
        }

        yield return StartCoroutine(BeatAnimation(result));
    }

    [DebuggerHidden]
    private IEnumerator ComboAnimation(BattleResult result)
    {
        Int32 count = 0;
        Int32 winSide;
        Int32 arrowMask = 0;
        QuadMistCardUI atkUI = board.GetCardUI(result.attacker);
        QuadMistCardUI defUI = board.GetCardUI(result.defender);
        Int32 i = 0;
        while (i < result.combos.Length)
        {
            if (result.combos[i] != null)
            {
                arrowMask |= 1 << i;
                count++;
            }
            i++;
        }
        if (count > 0)
        {
            QuadMistCardUI loser;
            if (result.type == BattleResult.Type.WIN)
            {
                winSide = atkUI.Side;
                loser = defUI;
            }
            else
            {
                winSide = defUI.Side;
                loser = atkUI;
            }
            yield return StartCoroutine(loser.FlashArrow((Byte)arrowMask));

            i = 0;
            while (i < result.combos.Length)
            {
                if (result.combos[i] != null)
                {
                    QuadMistCardUI cardUI = board.GetCardUI(result.combos[i]);
                    Action<QuadMistCardUI> functor = c =>
                    {
                        c.Side = winSide;
                        UpdateScore();
                    };
                    SoundEffect.Play(QuadMistSoundID.MINI_SE_COMB);
                    StartCoroutine(cardUI.FlashCombo((CardArrow.Type)i, functor));
                }
                i++;
            }

            yield return StartCoroutine(Anim.Tick(CardEffect.FLASH_TICK_DURATION / 2));

            StartCoroutine(ComboText(loser, count));
            yield return StartCoroutine(Anim.Tick(CardEffect.FLASH_TICK_DURATION / 2));
        }
    }

    [DebuggerHidden]
    private IEnumerator BeatAnimation(BattleResult result)
    {
        Byte winSide = result.attacker.side;
        Int32 i = 0;
        while (i < result.beats.Count)
        {
            QuadMistCardUI cardUI = board.GetCardUI(result.beats[i]);
            if (winSide != cardUI.Side)
            {
                Action<QuadMistCardUI> functor = c =>
                {
                    c.Side = winSide;
                    UpdateScore();
                };
                SoundEffect.Play(QuadMistSoundID.MINI_SE_FLASH);
                StartCoroutine(cardUI.FlashNormal(functor));
            }
            i++;
        }
        yield return StartCoroutine(Anim.Tick(CardEffect.FLASH_TICK_DURATION));
    }

    [DebuggerHidden]
    private IEnumerator BombAnimation(QuadMistCardUI a, QuadMistCardUI b, Int32 tickDuration)
    {
        Vector3 v0 = a.transform.position + new Vector3(a.Size.x, -a.Size.y, 0f) / 2f;
        Vector3 v1 = b.transform.position + new Vector3(b.Size.x, -b.Size.y, 0f) / 2f;
        Vector3 v = (v0 + v1) / 2f;
        v.z = -5f;
        bomb.transform.position = v;
        bomb.gameObject.SetActive(true);
        SoundEffect.Play(QuadMistSoundID.MINI_SE_BOMB);

        for (Int32 tick = 0; tick <= tickDuration; tick++)
        {
            bomb.ID = (tick & 15) / 2;
            bomb.gameObject.SetActive(true);
            yield return StartCoroutine(Anim.Tick());
        }

        bomb.gameObject.SetActive(false);
    }

    [DebuggerHidden]
    private IEnumerator BattleNumberTextAnimation(BattleResult result, Int32 startTick, Int32 valueChangeOffsetTick, Int32 endTick)
    {
        QuadMistCardUI atkUI = board.GetCardUI(result.attacker);
        QuadMistCardUI defUI = board.GetCardUI(result.defender);
        SetBattleNumber(0, atkUI, result.calculation.atkStart.ToString());
        SetBattleNumber(1, defUI, result.calculation.defStart.ToString());
        battleNumber[0].gameObject.SetActive(true);
        battleNumber[1].gameObject.SetActive(true);
        yield return StartCoroutine(Anim.Tick(startTick));

        Int32 tickOffset = valueChangeOffsetTick - startTick;
        for (Int32 tick = 0; tick <= tickOffset; tick++)
        {
            Int32 n0 = result.calculation.atkStart - (result.calculation.atkStart - result.calculation.atkFinish) * tick / tickOffset;
            Int32 n1 = result.calculation.defStart - (result.calculation.defStart - result.calculation.defFinish) * tick / tickOffset;
            SetBattleNumber(0, atkUI, n0.ToString());
            SetBattleNumber(1, defUI, n1.ToString());
            yield return StartCoroutine(Anim.Tick());
        }

        SetBattleNumber(0, atkUI, result.calculation.atkFinish.ToString());
        SetBattleNumber(1, defUI, result.calculation.defFinish.ToString());
        yield return StartCoroutine(Anim.Tick(endTick));

        battleNumber[0].Text = String.Empty;
        battleNumber[1].Text = String.Empty;
        battleNumber[0].gameObject.SetActive(false);
        battleNumber[1].gameObject.SetActive(false);
    }

    [DebuggerHidden]
    private IEnumerator BattleMotionAnimation(QuadMistCardUI a, QuadMistCardUI b, Int32 tickOffset, Int32 tickDuration)
    {
        Single moveX = (a.transform.position.x - b.transform.position.x) * a.Size.x * 256f / 270f;
        Single moveY = (a.transform.position.y - b.transform.position.y) * a.Size.y * 256f / 270f;
        Vector3 initPosA = a.transform.position;
        Vector3 initPosB = b.transform.position;

        for (Int32 tick = 0; tick <= tickDuration; tick++)
        {
            Int32 moveOffset = tick < tickOffset ? tick : tickDuration - tick;
            moveOffset *= moveOffset;
            a.transform.position = new Vector3(initPosA.x - moveX * moveOffset / 256f, initPosA.y - moveY * moveOffset / 256f, initPosA.z - 1f);
            b.transform.position = new Vector3(initPosB.x + moveX * moveOffset / 256f, initPosB.y + moveY * moveOffset / 256f, initPosB.z);
            UpdateBattleNumberPosition(0, a);
            UpdateBattleNumberPosition(1, b);
            yield return StartCoroutine(Anim.Tick());
        }

        a.transform.position = initPosA;
        b.transform.position = initPosB;
    }

    [DebuggerHidden]
    private IEnumerator ComboText(QuadMistCardUI a, Int32 num)
    {
        combo.Number = num;
        combo.transform.position = a.transform.position + new Vector3(0f, 0f, -2f);
        combo.gameObject.SetActive(true);
        yield return StartCoroutine(Anim.Tick(50));

        combo.gameObject.SetActive(false);
    }

    private void SetBattleNumber(Int32 i, QuadMistCardUI a, String text)
    {
        Single num = 0.09f;
        SpriteText spriteText = this.battleNumber[i];
        Vector3 vector3 = new Vector3((Single)(a.transform.position.x + (a.Size.x - num * 3.0) / 2.0 + 0.00999999977648258), a.transform.position.y - 0.18f, a.transform.position.z - 2f);
        spriteText.Text = text;
        if (text.Length < 2)
            vector3.x += num;
        else if (text.Length < 3)
            vector3.x += num / 2f;
        spriteText.transform.position = vector3;
    }

    private void UpdateBattleNumberPosition(Int32 i, QuadMistCardUI a)
    {
        this.SetBattleNumber(i, a, this.battleNumber[i].Text);
    }
}