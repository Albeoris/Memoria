using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using FF9;
using System.Reflection;
using System.Linq;

public class QuadMistGame : MonoBehaviour
{
    private readonly List<QuadMistCard> _selectableTargets = new List<QuadMistCard>();
    private readonly List<QuadMistCard> _beatableTargets = new List<QuadMistCard>();
    private readonly List<QuadMistCard> _adjacentTargets_rules_same = new List<QuadMistCard>();
    private readonly List<QuadMistCard> _adjacentTargets_rules_plus = new List<QuadMistCard>();
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

    // [DV] Triple Triad special rules
    public static Boolean HasTripleTrialRule_One; // [Trade Rule] Pick one card from the opponent card, regardless of his side.
    public static Boolean HasTripleTrialRule_Direct; // [Trade Rule] Player and opponent win the cards they have turned over.
    public static Boolean HasTripleTrialRule_Diff; // [Trade Rule] Recover a certain number of cards according to the score difference.
    public static Boolean HasTripleTrialRule_All; // [Trade Rule] Earn every cards from the opponent card, regardless of the score.
    public static Boolean HasTripleTrialRule_Open; // [Misc Rule] See the opponent's deck.
    public static Boolean HasTripleTrialRule_Random; // [Misc Rule] The game choose randomly cards for the player.
    public static Boolean HasTripleTrialRule_SuddenDeath; // [Misc Rule] When draw, start a new game with new hands depending the cards' side, from the previous game.
    public static Boolean HasTripleTrialRule_Same; // [Gameplay Rule] When a card is placed touching two or more other cards and the touching sides are equal, activate a combo.
    public static Boolean HasTripleTrialRule_Plus; // [Gameplay Rule] When a card is placed touching two or more other cards and the sum of touching sides are equal, activate a combo.
    public static Boolean HasTripleTrialRule_SameWall; // [Gameplay Rule] The edges of the board count as A (value = 10) for the same rule
    public static Boolean HasTripleTrialRule_Elemental; // [Gameplay Rule] Gives +1 stats on the card if the case has the same element as the card, otherwise give -1 stats.
    private TripleTriadRulesID TripleTrialRuleActivated;
    public static Boolean SkipTripleTriadRules;
    public static Int32 remainingcardsdiff;
    public static Boolean TriggerSuddenDeath;

    public PreBoard preBoard => preGame.preBoard;
    public Board board => playGame.board;
    public Coin coin => playGame.coin;
    public Combo combo => playGame.combo;
    public Score score => playGame.score;
    public ResultText resultText => playGame.result;
    public SpriteDisplay bomb => playGame.bomb;
    public SpriteText[] battleNumber => playGame.battleNumber;

    public Int32 PlayerScore
    {
        get { return playerScore; }
        set
        {
            score.PlayerScore = value;
            playerScore = value;
            if (Board.USE_TRIPLETRIAD_BOARD)
                score.player.transform.localPosition = new Vector3(2.75f, -2f, -2f);
        }
    }

    public Int32 EnemyScore
    {
        get { return enemyScore; }
        set
        {
            score.EnemyScore = value;
            enemyScore = value;
            if (Board.USE_TRIPLETRIAD_BOARD)
                score.enemy.transform.localPosition = new Vector3(0.3f, -2f, -8f);
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
        InitResources();
        preBoard.InitResources();
        board.InitResources();
        ClearHands();
        ClearGameObjects();
        ClearStates();
        ClearInput();
        InitTripleTriadResources();
        FF9Snd.ff9minisnd_song_play(66);
        StackCardInfo = new QuadMistCard();
        StackCardCount = 0;
    }

    private void InitResources()
    {
        GameObject go = Object.Instantiate(cardPrefab);
        movedCard = go.GetComponent<QuadMistCardUI>();
        go.name = "movedCard";
        go.transform.parent = transform;
        go.transform.localPosition = new Vector3(1.6f, -1.12f, 0.21865f);
        go.SetActive(false);
    }

    private void InitTripleTriadResources()
    {
        TriggerSuddenDeath = false;
        SkipTripleTriadRules = false;
        if (Configuration.TetraMaster.TripleTriad == 3 && TripleTriad.TripleTriadRules.Count > 0 && (TripleTriad.TripleTriadRules.ContainsKey((Int32)FF9StateSystem.Common.FF9.fldMapNo) || TripleTriad.TripleTriadRules.ContainsKey(0)))
        {
            int RandomTradesRules = Comn.random8() % 4;
            byte RandomOtherRules = (byte)(1 + Comn.random8() % 7);
            byte RandomGameplayRules = (byte)(1 + Comn.random8() % 15);
            int fieldid = TripleTriad.TripleTriadRules.ContainsKey((Int32)FF9StateSystem.Common.FF9.fldMapNo) ? FF9StateSystem.Common.FF9.fldMapNo : (TripleTriad.TripleTriadRules.ContainsKey(0) ? 0 : -1);
            if (fieldid > -1)
            {
                if (TripleTriad.TripleTriadRules[fieldid].RandomRules)
                {
                    HasTripleTrialRule_One = RandomTradesRules == 0 ? true : false;
                    HasTripleTrialRule_Direct = RandomTradesRules == 1 ? true : false;
                    HasTripleTrialRule_Diff = RandomTradesRules == 2 ? true : false;
                    HasTripleTrialRule_All = RandomTradesRules == 3 ? true : false;
                    HasTripleTrialRule_Open = (RandomOtherRules & 1) != 0 ? true : false;
                    HasTripleTrialRule_Random = (RandomOtherRules & 2) != 0 ? true : false;
                    HasTripleTrialRule_SuddenDeath = (RandomOtherRules & 4) != 0 ? true : false;
                    HasTripleTrialRule_Same = (RandomGameplayRules & 1) != 0 ? true : false;
                    HasTripleTrialRule_Plus = (RandomGameplayRules & 2) != 0 ? true : false;
                    HasTripleTrialRule_SameWall = (RandomGameplayRules & 4) != 0 ? true : false;
                    HasTripleTrialRule_Elemental = (RandomGameplayRules & 8) != 0 ? true : false;
                }
                else
                {
                    if (TripleTriad.TripleTriadRules[fieldid].One)
                        HasTripleTrialRule_One = TripleTriad.TripleTriadRules[fieldid].One;
                    else if (TripleTriad.TripleTriadRules[fieldid].Direct)
                        HasTripleTrialRule_Direct = TripleTriad.TripleTriadRules[fieldid].Direct;
                    else if (TripleTriad.TripleTriadRules[fieldid].Diff)
                        HasTripleTrialRule_Diff = TripleTriad.TripleTriadRules[fieldid].Diff;
                    else if (TripleTriad.TripleTriadRules[fieldid].All)
                        HasTripleTrialRule_All = TripleTriad.TripleTriadRules[fieldid].All;
                    HasTripleTrialRule_Open = TripleTriad.TripleTriadRules[fieldid].Open;
                    HasTripleTrialRule_Random = TripleTriad.TripleTriadRules[fieldid].Random;
                    HasTripleTrialRule_SuddenDeath = TripleTriad.TripleTriadRules[fieldid].SuddenDeath;
                    HasTripleTrialRule_Same = TripleTriad.TripleTriadRules[fieldid].Same;
                    HasTripleTrialRule_Plus = TripleTriad.TripleTriadRules[fieldid].Plus;
                    HasTripleTrialRule_SameWall = TripleTriad.TripleTriadRules[fieldid].SameWall;
                    HasTripleTrialRule_Elemental = TripleTriad.TripleTriadRules[fieldid].Elemental;
                }
            }
        }
    }

    public void Pause()
    {
        IsPause = true;
        TimeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0.0f;
        playerHand.Select = -1;
        IsSeizingCard = false;
    }

    public void Resume()
    {
        IsPause = false;
        Time.timeScale = TimeScaleBeforePause;
    }

    private void Update()
    {
        if (!isAnimating && !IsPause)
        {
            switch (GameState)
            {
                case GAME_STATE.PREGAME:
                    PreGame();
                    break;
                case GAME_STATE.START:
                    StartGame();
                    break;
                case GAME_STATE.PLAY:
                    PlayGame();
                    break;
                case GAME_STATE.END:
                    EndGame();
                    break;
                case GAME_STATE.POSTGAME:
                    PostGame();
                    break;
            }
            SpriteRenderer component = CardNameToggleButton.GetComponent<SpriteRenderer>();
            if (component != null)
                component.sprite = CardNameDialogSlider.IsShowCardName ? CardNameToggleButtonRendererHilight.sprite : CardNameToggleButtonRenderer.sprite;
        }
        SceneDirector.ServiceFade();
    }

    private Boolean ShouldShowTutorial()
    {
        return QuadMistDatabase.WinCount == 0 && QuadMistDatabase.LoseCount == 0 && QuadMistDatabase.DrawCount == 0 && Configuration.TetraMaster.TripleTriad <= 1;
    }

    private void onFinishTutorial01()
    {
        PreGameState = PREGAME_STATE.SELECT_COLLECTION;
    }

    private void onFinishTutorial02()
    {
        PlayState = PLAY_STATE.INPUT_PLAYER;
    }

    private void onFinishTutorial03()
    {
        PlayState = PLAY_STATE.INPUT_PLAYER;
    }

    private void PreGame()
    {
        inputResult.Clear();
        switch (PreGameState)
        {
            case PREGAME_STATE.SETUP:
                PreGameState = PREGAME_STATE.SETUP_DONE;
                enemyHand.State = Hand.STATE.ENEMY_HIDE;
                playerHand.State = Hand.STATE.PLAYER_PREGAME;
                QuadMistDatabase.LoadData();
                QuadMistDatabase.CreateDataIfLessThanFive();
                reservedCardList = QuadMistDatabase.GetCardList();
                preBoard.collection = collection;
                preBoard.collection.CreateCards();
                preBoard.UpdateCollection(-1);
                preBoard.SetPreviewCardID(0);
                winScore.text = QuadMistDatabase.WinCount.ToString();
                loseScore.text = QuadMistDatabase.LoseCount.ToString();
                drawScore.text = QuadMistDatabase.DrawCount.ToString();
                Int32 totalCount = 0;
                Int32 typeCount = 0;
                foreach (List<QuadMistCard> card in preBoard.collection.cards)
                {
                    if (card.Count > 0)
                    {
                        typeCount++;
                        totalCount += card.Count;
                    }
                }
                cardTypeCount.text = String.Empty + typeCount;
                cardStockCount.text = String.Empty + totalCount;
                PersistenSingleton<UIManager>.Instance.QuadMistScene.State = QuadMistUI.CardState.CardSelection;
                PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.QuadMist);
                if (ShouldShowTutorial())
                    PreGameState = PREGAME_STATE.SHOW_TUTORIAL;
                QuadMistDatabase.MiniGame_ContinueInit();
                SceneDirector.FF9Wipe_FadeInEx(30);
                break;
            case PREGAME_STATE.SHOW_TUTORIAL:
                TutorialUI tutorialScene = PersistenSingleton<UIManager>.Instance.TutorialScene;
                tutorialScene.DisplayMode = TutorialUI.Mode.QuadMist;
                tutorialScene.QuadmistTutorialID = 1;
                tutorialScene.AfterFinished = onFinishTutorial01;
                PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Tutorial);
                PreGameState = PREGAME_STATE.TUTORIAL_01;
                break;
            case PREGAME_STATE.SELECT_COLLECTION:
                if (!inputResult.IsValid())
                    break;
                QuadMistConfirmDialog.MessageShow(new Vector3(0.0f, 0.0f, 0.0f), "Confirm Selection", true, true);
                ++PreGameState;
                break;
            case PREGAME_STATE.CONFIRM_DIALOG:
                playerInput.HandleDialog(ref inputResult);
                if (!inputResult.IsValid())
                    break;
                if (QuadMistConfirmDialog.IsOK)
                {
                    preGame.gameObject.SetActive(false);
                    GameState = GAME_STATE.START;
                }
                else
                    PreGameState = PREGAME_STATE.SELECT_COLLECTION;
                QuadMistConfirmDialog.MessageHide();
                break;
        }          
    }

    private void StartGame()
    {
        if (PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI)
            return;
        switch (StartState)
        {
            case START_STATE.SETUP:
                Int32 rnd = Random.Range(0, 100);
                numberOfBlocks = rnd < 0 || rnd >= 3 ? (rnd < 3 || rnd >= 7 ? (rnd < 7 || rnd >= 12 ? (rnd < 12 || rnd >= 17 ? (rnd < 17 || rnd >= 27 ? (rnd < 27 || rnd >= 97 ? 6 : 5) : 4) : 3) : 2) : 1) : 0;
                if (!TriggerSuddenDeath)
                {
                    EnemyData.Setup(enemyHand);
                    if (StackCardCount != 0)
                        EnemyData.RestorePlayerLostCard(enemyHand, Random.Range(0, 4), StackCardInfo);
                }
                enemyHand.HideCardCursor();
                matchCards = new QuadMistCard[10];
                for (Int32 i = 0; i < matchCards.Length; ++i)
                    matchCards[i] = i >= 5 ? enemyHand[i - 5] : playerHand[i];
                UpdateScore();
                board.Clear();
                playGame.gameObject.SetActive(true);
                ++StartState;
                // Child(1) is "divider" and has components SpriteRenderer / SpriteDisplay
                SpriteRenderer dividerSprite = score.transform.GetChild(1).GetComponent<SpriteRenderer>();
                if (Configuration.TetraMaster.TripleTriad <= 1)
                {
                    PlayerScore = 0;
                    EnemyScore = 0;
                    dividerSprite?.gameObject?.SetActive(true);
                }
                else
                {
                    PlayerScore = playerHand.Count;
                    EnemyScore = enemyHand.Count;
                    dividerSprite?.gameObject?.SetActive(false);
                }
                break;
            case START_STATE.SETUP_BOARD:
                AnimCoroutine(Anim.Enable(board.gameObject), board.FadeInBoard());
                ++StartState;
                break;
            case START_STATE.SETUP_FIELD:
                enemyHand.State = Hand.STATE.ENEMY_SHOW;
                if (Configuration.TetraMaster.TripleTriad <= 1)
                    AnimCoroutine(board.ScaleInBlocks(numberOfBlocks));
                ++StartState;
                break;
            case START_STATE.COIN:
                Int32 side = Random.Range(0, 2);
                PlayState = side != 0 ? PLAY_STATE.INPUT_ENEMY : PLAY_STATE.INPUT_PLAYER;
                yourTurn = side == 0;
                AnimCoroutine(Anim.Enable(coin.gameObject), coin.Toss(side), Anim.Disable(coin.gameObject));
                playerTurnCount = 0;
                hasShowTutorial02 = false;
                hasShowTutorial03 = false;
                GameState = GAME_STATE.PLAY;
                enemyHand.State = Hand.STATE.ENEMY_WAIT;
                playerHand.State = Hand.STATE.PLAYER_WAIT;
                playerHand.HideShadowCard();
                QuadMistGame.main.CardNameDialogSlider.IsShowCardName = false;
                QuadMistGame.main.CardNameDialogSlider.HideCardNameDialog(playerHand);
                if (yourTurn)
                {
                    playerHand.State = Hand.STATE.PLAYER_SELECT_CARD;
                    break;
                }
                enemyHand.State = Hand.STATE.ENEMY_PLAY;
                break;
        }
    }

    private void PlayGame()
    {
        if (PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI)
            return;
        if (UIManager.Input.GetKeyTrigger(Control.RightBumper) && CardNameDialogSlider.IsReady)
        {
            CardNameDialogSlider.IsShowCardName = !CardNameDialogSlider.IsShowCardName;
            if (CardNameDialogSlider.IsShowCardName)
                CardNameDialogSlider.ShowCardNameDialog(playerHand);
            else
                CardNameDialogSlider.HideCardNameDialog(playerHand);
            SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
        }
        switch (PlayState)
        {
            case PLAY_STATE.INPUT_PLAYER:
                if (playerTurnCount == 0 && !hasShowTutorial02 && InputState == INPUT_STATE.SELECT_CARD && ShouldShowTutorial())
                {
                    TutorialUI tutorialScene = PersistenSingleton<UIManager>.Instance.TutorialScene;
                    tutorialScene.DisplayMode = TutorialUI.Mode.QuadMist;
                    tutorialScene.QuadmistTutorialID = 2;
                    tutorialScene.AfterFinished = onFinishTutorial02;
                    PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Tutorial);
                    PlayState = PLAY_STATE.TUTORIAL_02;
                    hasShowTutorial02 = true;
                    break;
                }
                if (playerTurnCount == 1 && !hasShowTutorial03 && InputState == INPUT_STATE.SELECT_CARD && ShouldShowTutorial())
                {
                    TutorialUI tutorialScene = PersistenSingleton<UIManager>.Instance.TutorialScene;
                    tutorialScene.DisplayMode = TutorialUI.Mode.QuadMist;
                    tutorialScene.QuadmistTutorialID = 3;
                    tutorialScene.AfterFinished = onFinishTutorial03;
                    PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Tutorial);
                    PlayState = PLAY_STATE.TUTORIAL_03;
                    hasShowTutorial03 = true;
                    break;
                }
                InputPlayer();
                break;
            case PLAY_STATE.INPUT_ENEMY:
                if (UIManager.Input.GetKeyTrigger(Control.Confirm))
                    SoundEffect.Play(QuadMistSoundID.MINI_SE_WARNING);
                InputEnemy();
                break;
            case PLAY_STATE.CALCULATE_BATTLE:
                enemyHand.State = Hand.STATE.ENEMY_WAIT;
                playerHand.State = Hand.STATE.PLAYER_WAIT;
                if (_battleResult.defender != null)
                {
                    QuadMistCard attacker = _battleResult.attacker;
                    QuadMistCard defender = _battleResult.defender;
                    if (Configuration.TetraMaster.TripleTriad < 2)
                    {
                        _battleResult.calculation = Calculate(attacker, defender);
                        _battleResult.type = _battleResult.calculation.atkFinish <= _battleResult.calculation.defFinish ? BattleResult.Type.LOSE : BattleResult.Type.WIN;
                        if (_battleResult.type == BattleResult.Type.WIN)
                        {
                            _battleResult.combos = GenerateCombo(_battleResult.defender, attacker.side);
                            RemoveComboFromBeatable(_battleResult.combos);
                        }
                        else
                        {
                            _battleResult.combos = GenerateCombo(_battleResult.attacker, defender.side);
                            _beatableTargets.Clear();
                        }
                    }
                    else if (Configuration.TetraMaster.TripleTriad == 3)
                    {
                        if (_selectableTargets.Count > 0)
                        {
                            _battleResult.type = BattleResult.Type.WIN;
                            _battleResult.calculation.atkFinish = 1;
                            _battleResult.calculation.defFinish = 0;
                            _battleResult.combos = GenerateComboTripleTriad(_battleResult.defender, attacker.side);
                            if (TripleTrialRuleActivated != TripleTriadRulesID.COMBO)
                                TripleTriadText(TripleTrialRuleActivated);
                            RemoveComboFromBeatable(_battleResult.combos);
                        }
                    }
                }
                else
                {
                    _battleResult.type = BattleResult.Type.NOTHING;
                }
                _battleResult.beats = _beatableTargets;
                AnimCoroutine(BattleAnimation(_battleResult));
                PlayState = PLAY_STATE.ANIMATE_BATTLE;
                break;
            case PLAY_STATE.ANIMATE_BATTLE:
                if (_battleResult.type == BattleResult.Type.WIN && _selectableTargets.Count > 0)
                {
                    foreach (QuadMistCard card in _battleResult.combos)
                        _selectableTargets.Remove(card);
                    _selectableTargets.Remove(_battleResult.defender);
                    PlayState = PLAY_STATE.INPUT_PLAYER;
                    break;
                }
                InputState = INPUT_STATE.SELECT_CARD;
                yourTurn = !yourTurn;
                PlayState = !yourTurn ? PLAY_STATE.INPUT_ENEMY : PLAY_STATE.INPUT_PLAYER;
                _battleResult.defender = null;
                if (Configuration.TetraMaster.TripleTriad >= 2 && enemyHand.Count == 0) // Fix a minor issue when player starts (his last card placed on board automatically)
                {
                    ++GameState;
                    playerHand.HideShadowCard();
                    return;
                }
                if (yourTurn)
                {
                    playerHand.State = Hand.STATE.PLAYER_SELECT_CARD;
                    if (QuadMistGame.main.CardNameDialogSlider.IsShowCardName)
                        QuadMistGame.main.CardNameDialogSlider.ShowCardNameDialog(playerHand);
                    else
                        QuadMistGame.main.CardNameDialogSlider.HideCardNameDialog(playerHand);
                }
                else
                    enemyHand.State = Hand.STATE.ENEMY_PLAY;
                if (Configuration.TetraMaster.TripleTriad == 3)
                {
                    resultText.gameObject.SetActive(false);
                    TripleTrialRuleActivated = TripleTriadRulesID.NONE;
                }
                if (Configuration.TetraMaster.TripleTriad <= 1 ? EnemyScore + PlayerScore == 10 : enemyHand.Count + playerHand.Count == 1)
                    ++GameState;
                playerHand.HideShadowCard();
                break;
        }
    }

    private void EndGame()
    {
        inputResult.Clear();
        switch (EndState)
        {
            case END_STATE.RESULT:
                Int32 perfectScore = Configuration.TetraMaster.TripleTriad <= 1 ? 10 : 9;
                matchResult = new MatchResult { perfect = PlayerScore == perfectScore || EnemyScore == perfectScore || HasTripleTrialRule_All };
                PostState = (matchResult.perfect || HasTripleTrialRule_All) ? POSTGAME_STATE.PERFECT_SELECT_CARD : POSTGAME_STATE.SELECT_CARD;
                if (PlayerScore > EnemyScore)
                {
                    matchResult.type = MatchResult.Type.WIN;
                    for (Int32 index = 5; index < 10; ++index)
                    {
                        if (HasTripleTrialRule_One || HasTripleTrialRule_Diff)
                        {
                            matchResult.selectable.Add(index - 5);
                        }
                        else
                        {
                            if (matchCards[index].side == 0)
                                matchResult.selectable.Add(index - 5);
                        }
                    }
                    QuadMistDatabase.WinCount++;
                }
                else if (EnemyScore > PlayerScore)
                {
                    matchResult.type = MatchResult.Type.LOSE;
                    for (Int32 index = 0; index < 5; ++index)
                    {
                        if (HasTripleTrialRule_One || HasTripleTrialRule_Diff)
                        {
                            matchResult.selectable.Add(index);
                        }
                        else
                        {
                            if (matchCards[index].side == 1)
                                matchResult.selectable.Add(index);
                        }
                    }
                    QuadMistDatabase.LoseCount++;
                }
                else
                {
                    matchResult.type = MatchResult.Type.DRAW;
                    PostState = POSTGAME_STATE.PRE_REMATCH;
                    QuadMistDatabase.DrawCount++;
                }
                if (matchResult.type == MatchResult.Type.WIN)
                    QuadMistDatabase.MiniGame_SetLastBattleResult(QuadMistDatabase.MINIGAME_LASTBATTLE_WIN);
                else if (matchResult.type == MatchResult.Type.LOSE)
                    QuadMistDatabase.MiniGame_SetLastBattleResult(QuadMistDatabase.MINIGAME_LASTBATTLE_LOSE);
                else if (matchResult.type == MatchResult.Type.DRAW)
                    QuadMistDatabase.MiniGame_SetLastBattleResult(QuadMistDatabase.MINIGAME_LASTBATTLE_DRAW);
                AnimCoroutine(ResultText(matchResult));
                if (HasTripleTrialRule_Diff && matchResult.type != MatchResult.Type.DRAW)
                {
                    PostState = POSTGAME_STATE.TRIPLETRIAD_DIFF;
                    remainingcardsdiff = Math.Abs(PlayerScore - EnemyScore);
                    if (remainingcardsdiff > 5)
                    {
                        matchResult.perfect = true;
                        PostState = POSTGAME_STATE.PERFECT_SELECT_CARD;
                    }
                }
                else if (HasTripleTrialRule_Direct)
                {
                    PostState = POSTGAME_STATE.TRIPLETRIAD_DIRECT;
                    matchResult.perfect = false;
                }
                EndState = END_STATE.CONFIRM;
                break;
            case END_STATE.CONFIRM:
                if (HasTripleTrialRule_SuddenDeath && matchResult.type == MatchResult.Type.DRAW)
                {
                    if (!isAnimating)
                    {
                        ClearHands();
                        ClearGameObjects();
                        ClearInput();
                        ClearStates();
                        for (Int32 i = 0; i < 10; i++)
                        {
                            QuadMistCard card = matchCards[i];
                            if (card.side == 0)
                            {
                                playerHand.Add(card);
                            }
                            else
                            {
                                enemyHand.Add(card);
                            }
                        }
                        TriggerSuddenDeath = true;
                        preGame.gameObject.SetActive(false);
                        GameState = GAME_STATE.START;
                        resultText.gameObject.SetActive(false);
                    }
                    break;
                }
                playerInput.HandleConfirmation(ref inputResult);
                if (!inputResult.IsValid())
                    break;
                if (QuadMistGame.main.CardNameDialogSlider.IsShowCardName)
                    QuadMistGame.main.CardNameDialogSlider.ShowCardNameDialog(playerHand);
                else
                    QuadMistGame.main.CardNameDialogSlider.HideCardNameDialog(playerHand);
                resultText.gameObject.SetActive(false);
                if (PostState != POSTGAME_STATE.PRE_REMATCH)
                    AnimCoroutine(ResultRestoreHands());
                playGame.gameObject.SetActive(false);
                playGame.background.color = new Color(1f, 1f, 1f, 0.0f);
                ++GameState;
                break;
        }
    }

    private void CheckAndClearStackCard()
    {
        //Debug.Log((object)("CheckAndClearStackCard: StackCardCount = " + (object)StackCardCount + ", Stack"));
        if (StackCardCount == 1 && StackCardInfo.isTheSameCard(matchResult.selectedCard))
        {
            //Debug.Log((object)"CheckAndClearStackCard: RESET StackCardCount(taken card) here.");
            StackCardCount = 0;
        }
        else
        {
            //Debug.Log((object)"CheckAndClearStackCard: NO NEED to reset StackCardCount(taken card) here.");
        }
    }

    private void PostGame()
    {
        inputResult.Clear();
        switch (PostState)
        {
            case POSTGAME_STATE.SELECT_CARD:
                if (matchResult.type == MatchResult.Type.WIN)
                {
                    playerInput.HandlePostSelection(enemyHand, matchResult.selectable, ref inputResult);
                    enemyHand.ShowCardCursor();
                    if (!inputResult.IsValid() && matchResult.selectable.Count != 1)
                        break;
                    if (matchResult.selectable.Count == 1)
                    {
                        inputResult.selectedCard = enemyHand[matchResult.selectable[0]];
                        inputResult.selectedHandIndex = matchResult.selectable[0];
                    }
                    matchResult.selectedCard = inputResult.selectedCard;
                    if (_t_selectedCard != null)
                        enemyHand.GetCardUI(enemyHandSelectedCardIndex).transform.localPosition -= _selectedCardDeltaPos;
                    enemyHandSelectedCardIndex = inputResult.selectedHandIndex;
                    QuadMistCardUI cardUi = enemyHand.GetCardUI(enemyHandSelectedCardIndex);
                    cardUi.transform.localPosition += _selectedCardDeltaPos;
                    enemyHand.UpdateEnemyCardCursorToPosition(cardUi.transform.localPosition);
                    if (_t_selectedCard != null && _t_selectedCard == inputResult.selectedCard)
                    {
                        enemyHand.HideCardCursor();
                        AnimCoroutine(ChangeCardToCenter(matchResult));
                        QuadMistDatabase.Add(inputResult.selectedCard);
                        QuadMistStockDialog.Hide();
                        _t_selectedCard = null;
                        PostState = POSTGAME_STATE.CONFIRM;
                        CheckAndClearStackCard();
                        break;
                    }
                    _t_selectedCard = inputResult.selectedCard;
                    Int32 cardCount = QuadMistDatabase.GetCardCount(matchResult.selectedCard);
                    Vector3 position = cardUi.transform.position;
                    QuadMistStockDialog.Show(new Vector3(position.x + 0.9482538f, position.y - 0.2696013f, 0.0f), Localization.Get("QuadMistStock").Replace("[NUMBER]", cardCount.ToString()));
                    break;
                }
                enemyInput.HandlePostSelection(playerHand, matchResult.selectable, ref inputResult);
                matchResult.selectedCard = inputResult.selectedCard;
                AnimCoroutine(ChangeCardToCenter(matchResult));
                PostState = POSTGAME_STATE.CONFIRM;
                QuadMistDatabase.Remove(matchResult.selectedCard);
                QuadMistCard lostCard = matchResult.selectedCard;
                StackCardInfo.id = lostCard.id;
                StackCardInfo.atk = lostCard.atk;
                StackCardInfo.arrow = lostCard.arrow;
                StackCardInfo.type = lostCard.type;
                StackCardInfo.pdef = lostCard.pdef;
                StackCardInfo.mdef = lostCard.mdef;
                StackCardCount = 1;
                break;
            case POSTGAME_STATE.PERFECT_SELECT_CARD:
                if (matchResult.type == MatchResult.Type.WIN)
                {
                    if (enemyHand.Count != 0)
                    {
                        matchResult.selectedCard = enemyHand[0];
                        AnimCoroutine(ChangeCardToCenter2(matchResult));
                        QuadMistDatabase.Add(matchResult.selectedCard);
                        CheckAndClearStackCard();
                        PostState = POSTGAME_STATE.CONFIRM;
                        break;
                    }
                    PostState = POSTGAME_STATE.PRE_REMATCH;
                    break;
                }
                if (playerHand.Count != 0)
                {
                    matchResult.selectedCard = playerHand[0];
                    AnimCoroutine(ChangeCardToCenter2(matchResult));
                    PostState = POSTGAME_STATE.CONFIRM;
                    QuadMistDatabase.Remove(matchResult.selectedCard);
                    QuadMistCard lostCardLoop = matchResult.selectedCard;
                    StackCardInfo.id = lostCardLoop.id;
                    StackCardInfo.atk = lostCardLoop.atk;
                    StackCardInfo.arrow = lostCardLoop.arrow;
                    StackCardInfo.type = lostCardLoop.type;
                    StackCardInfo.pdef = lostCardLoop.pdef;
                    StackCardInfo.mdef = lostCardLoop.mdef;
                    StackCardCount = 1;
                    break;
                }
                PostState = POSTGAME_STATE.PRE_REMATCH;
                break;
            case POSTGAME_STATE.TRIPLETRIAD_DIFF:
                if (remainingcardsdiff > 0)
                {
                    if (matchResult.type == MatchResult.Type.WIN)
                    {
                        playerInput.HandlePostSelection(enemyHand, matchResult.selectable, ref inputResult);
                        enemyHand.ShowCardCursor();
                        if (!inputResult.IsValid() && matchResult.selectable.Count != 1)
                            break;
                        if (matchResult.selectable.Count == 1)
                        {
                            inputResult.selectedCard = enemyHand[matchResult.selectable[0]];
                            inputResult.selectedHandIndex = matchResult.selectable[0];
                        }
                        matchResult.selectedCard = inputResult.selectedCard;
                        if (_t_selectedCard != null)
                            enemyHand.GetCardUI(enemyHandSelectedCardIndex).transform.localPosition -= _selectedCardDeltaPos;
                        enemyHandSelectedCardIndex = inputResult.selectedHandIndex;
                        QuadMistCardUI cardUi = enemyHand.GetCardUI(enemyHandSelectedCardIndex);
                        cardUi.transform.localPosition += _selectedCardDeltaPos;
                        enemyHand.UpdateEnemyCardCursorToPosition(cardUi.transform.localPosition);
                        if (_t_selectedCard != null && _t_selectedCard == inputResult.selectedCard)
                        {
                            enemyHand.HideCardCursor();
                            AnimCoroutine(ChangeCardToCenter(matchResult));
                            QuadMistDatabase.Add(inputResult.selectedCard);
                            QuadMistStockDialog.Hide();
                            _t_selectedCard = null;
                            PostState = POSTGAME_STATE.CONFIRM;
                            CheckAndClearStackCard();
                            remainingcardsdiff--;
                            break;
                        }
                        _t_selectedCard = inputResult.selectedCard;
                        Int32 cardCount = QuadMistDatabase.GetCardCount(matchResult.selectedCard);
                        Vector3 position = cardUi.transform.position;
                        QuadMistStockDialog.Show(new Vector3(position.x + 0.9482538f, position.y - 0.2696013f, 0.0f), Localization.Get("QuadMistStock").Replace("[NUMBER]", cardCount.ToString()));  
                        break;
                    }
                    enemyInput.HandlePostSelection(playerHand, matchResult.selectable, ref inputResult);
                    matchResult.selectedCard = inputResult.selectedCard;
                    AnimCoroutine(ChangeCardToCenter(matchResult));
                    PostState = POSTGAME_STATE.CONFIRM;
                    QuadMistDatabase.Remove(matchResult.selectedCard);
                    QuadMistCard lostCard2 = matchResult.selectedCard;
                    StackCardInfo.id = lostCard2.id;
                    StackCardInfo.atk = lostCard2.atk;
                    StackCardInfo.arrow = lostCard2.arrow;
                    StackCardInfo.type = lostCard2.type;
                    StackCardInfo.pdef = lostCard2.pdef;
                    StackCardInfo.mdef = lostCard2.mdef;
                    StackCardCount = 1;
                    remainingcardsdiff--;
                }                
                break;
            case POSTGAME_STATE.TRIPLETRIAD_DIRECT:
                Boolean NoMorePlayerCards = true;
                Boolean NoMoreEnemyCards = true;
                int indexcard = 0;
                for (Int32 index = 0; index < playerHand.Count; index++)
                {
                    if (playerHand[index].side == 1)
                    {
                        NoMoreEnemyCards = false;
                        indexcard = index;
                        break;
                    }
                }
                if (!NoMoreEnemyCards)
                {
                    matchResult.type = MatchResult.Type.LOSE;
                    matchResult.selectable.Add(indexcard);
                    matchResult.selectedCard = playerHand[indexcard];
                    AnimCoroutine(ChangeCardToCenter2(matchResult));
                    PostState = POSTGAME_STATE.CONFIRM;
                    QuadMistDatabase.Remove(matchResult.selectedCard);
                    QuadMistCard lostCardLoop = matchResult.selectedCard;
                    StackCardInfo.id = lostCardLoop.id;
                    StackCardInfo.atk = lostCardLoop.atk;
                    StackCardInfo.arrow = lostCardLoop.arrow;
                    StackCardInfo.type = lostCardLoop.type;
                    StackCardInfo.pdef = lostCardLoop.pdef;
                    StackCardInfo.mdef = lostCardLoop.mdef;
                    StackCardCount = 1;
                    break;
                }
                for (Int32 index = 0; index < enemyHand.Count; index++)
                {
                    if (enemyHand[index].side == 0)
                    {
                        NoMorePlayerCards = false;
                        indexcard = index;
                        break;
                    }
                }
                if (!NoMorePlayerCards)
                {
                    matchResult.type = MatchResult.Type.WIN;
                    matchResult.selectedCard = enemyHand[indexcard];
                    AnimCoroutine(ChangeCardToCenter2(matchResult));
                    QuadMistDatabase.Add(matchResult.selectedCard);
                    CheckAndClearStackCard();
                    PostState = POSTGAME_STATE.CONFIRM;
                    break;
                }
                PostState = POSTGAME_STATE.PRE_REMATCH;
                break;
            case POSTGAME_STATE.CONFIRM:
                playerInput.HandleConfirmation(ref inputResult);
                if (!inputResult.IsValid())
                    break;
                if (HasTripleTrialRule_Diff && remainingcardsdiff > 0 && !matchResult.perfect)
                {
                    AnimCoroutine(ChangeCardToHand(matchResult));
                    PostState = POSTGAME_STATE.TRIPLETRIAD_DIFF;
                }
                else if (HasTripleTrialRule_Direct)
                {
                    AnimCoroutine(ChangeCardToHand(matchResult));
                    PostState = POSTGAME_STATE.TRIPLETRIAD_DIRECT;
                }
                else if (matchResult.perfect)
                {
                    AnimCoroutine(ChangeCardToHand(matchResult));
                    PostState = POSTGAME_STATE.PERFECT_SELECT_CARD;
                }
                else
                {
                    AnimCoroutine(ChangeCardToHand(matchResult));
                    PostState = POSTGAME_STATE.PRE_REMATCH;
                }
                QuadMistGetCardDialog.Hide();
                break;
            case POSTGAME_STATE.PRE_REMATCH:
                if (matchResult.type == MatchResult.Type.DRAW)
                    SaveReservedCardToDatabase();
                else
                    SaveCardToDatabase();
                PostState = POSTGAME_STATE.REMATCH;
                break;
            case POSTGAME_STATE.REMATCH:
                Rematch();
                break;
        }
    }

    private void onRematchDialogHidden(Int32 choice)
    {
        PostState = POSTGAME_STATE.REMATCH_CONFIRM;
        ButtonGroupState.SetPointerOffsetToGroup(Dialog.DefaultOffset, Dialog.DialogGroupButton);
        Boolean isTournamentMatch = FF9StateSystem.Common.FF9.miniGameArg == 124 || FF9StateSystem.Common.FF9.miniGameArg == 125 || FF9StateSystem.Common.FF9.miniGameArg == 126 || FF9StateSystem.Common.FF9.miniGameArg == 127;
        if (!isTournamentMatch && choice == 0 || isTournamentMatch && choice == -1)
        {
            board.BoardCursor.ForceHide();
            RestoreCollection();
            ClearHands();
            ClearGameObjects();
            ClearInput();
            ClearStates();
        }
        else
        {
            QuitQuadMist();
        }
    }

    public void QuitQuadMist()
    {
        SceneDirector.FF9Wipe_FadeOutEx(30);
        StartCoroutine(QuitQuadMistTransition(30f / FPSManager.GetTargetFPS()));
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
        PostState = POSTGAME_STATE.REMATCH_CONFIRM;
        if (matchResult.type == MatchResult.Type.DRAW)
            SaveReservedCardToDatabase();
        else
            SaveCardToDatabase();
        QuitQuadMist();
    }

    public static void OnDiscardFinish()
    {
        QuadMistGame.main.playerHand.gameObject.SetActive(true);
        QuadMistGame.main.enemyHand.gameObject.SetActive(true);
        QuadMistGame.main.PostState = POSTGAME_STATE.REMATCH;
    }

    private void Rematch()
    {
        if (matchResult.type == MatchResult.Type.DRAW && (FF9StateSystem.Common.FF9.miniGameArg == 124 || FF9StateSystem.Common.FF9.miniGameArg == 125 || (FF9StateSystem.Common.FF9.miniGameArg == 126 || FF9StateSystem.Common.FF9.miniGameArg == SByte.MaxValue)))
        {
            if (PersistenSingleton<UIManager>.Instance.Dialogs.CheckDialogShowing(1))
                return;
            Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get("QuadMistTournamentDraw"), 110, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(0.0f, 0.0f), Dialog.CaptionType.None);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(265f, 0.0f), Dialog.DialogGroupButton);
            dialog.AfterDialogHidden = onRematchDialogHidden;
            dialog.Id = 1;
        }
        else if (QuadMistDatabase.MiniGame_GetAllCardCount() > Configuration.TetraMaster.MaxCardCount)
        {
            PersistenSingleton<UIManager>.Instance.QuadMistScene.State = QuadMistUI.CardState.CardDestroy;
            PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.QuadMist);
            playerHand.gameObject.SetActive(false);
            enemyHand.gameObject.SetActive(false);
            PostState = POSTGAME_STATE.DISCARD;
        }
        else
        {
            if (PersistenSingleton<UIManager>.Instance.Dialogs.CheckDialogShowing(1))
                return;
            if (FF9StateSystem.Common.FF9.miniGameArg == 124 || FF9StateSystem.Common.FF9.miniGameArg == 125 || (FF9StateSystem.Common.FF9.miniGameArg == 126 || FF9StateSystem.Common.FF9.miniGameArg == SByte.MaxValue))
                onRematchDialogHidden(1);
            else if ((matchResult.type != MatchResult.Type.DRAW ? playerHand.GetQuadMistCards().Count + QuadMistUI.allCardList.Count : reservedCardList.Count) < 5)
            {
                Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get("QuadMistYouDontHave5Cards"), 124, 2, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(0.0f, 0.0f), Dialog.CaptionType.None);
                dialog.AfterDialogHidden = OnDontHave5CardsDialog;
                dialog.Id = 1;
                SoundEffect.Play(QuadMistSoundID.MINI_SE_WARNING);
            }
            else
            {
                Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get("QuadMistRematch"), 110, 3, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(0.0f, 0.0f), Dialog.CaptionType.None);
                ButtonGroupState.SetPointerOffsetToGroup(new Vector2(265f, 0.0f), Dialog.DialogGroupButton);
                dialog.AfterDialogHidden = onRematchDialogHidden;
                dialog.Id = 1;
            }
        }
    }

    private void SaveReservedCardToDatabase()
    {
        QuadMistDatabase.SetCardList(reservedCardList);
        QuadMistDatabase.SaveData();
    }

    private void SaveCardToDatabase()
    {
        List<QuadMistCard> quadMistCards = playerHand.GetQuadMistCards();
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
        if (matchResult.type != MatchResult.Type.WIN)
            return;
        EMinigame.QuadmistWinAllNPCAchievement();
        EMinigame.GetWinQuadmistAchievement();
    }

    private void RestoreCollection()
    {
        if (matchResult.type == MatchResult.Type.DRAW)
        {
            for (Int32 index = 0; index < 5; ++index)
                collection.Add(matchCards[index]);
        }
        else
        {
            foreach (QuadMistCard c in playerHand)
                collection.Add(c);
        }
    }

    private void InputPlayer()
    {
        inputResult.Clear();
        switch (InputState)
        {
            case INPUT_STATE.SELECT_CARD:
                playerInput.HandleYourCardSelection(board, playerHand, ref inputResult);
                if (!inputResult.IsValid())
                    break;
                PlaceCard(inputResult.x, inputResult.y, inputResult.selectedCard);
                if (Configuration.TetraMaster.TripleTriad >= 2)
                    GenerateTargetableTripleTriad(inputResult.x, inputResult.y);
                else
                    GenerateTargetable(inputResult.x, inputResult.y);
                _battleResult.attacker = inputResult.selectedCard;
                ++InputState;
                ++playerTurnCount;
                break;
            case INPUT_STATE.SELECT_BATTLE_TARGET:
                if (Configuration.TetraMaster.TripleTriad == 3 && _selectableTargets.Count > 0)
                {
                    _battleResult.defender = _selectableTargets[0];
                    PlayState = PLAY_STATE.CALCULATE_BATTLE;
                    break;
                }
                if (_selectableTargets.Count > 1)
                {
                    playerInput.HandleTargetCardSelection(board, _battleResult.attacker, _selectableTargets, ref inputResult);
                    if (!inputResult.IsValid())
                        break;
                    _battleResult.defender = _selectableTargets[inputResult.index];
                    PlayState = PLAY_STATE.CALCULATE_BATTLE;
                    break;
                }
                if (_selectableTargets.Count == 1)
                {
                    _battleResult.defender = _selectableTargets[0];
                    PlayState = PLAY_STATE.CALCULATE_BATTLE;
                    break;
                }
                _battleResult.defender = null;
                PlayState = PLAY_STATE.CALCULATE_BATTLE;
                break;
        }
    }

    private void InputEnemy()
    {
        inputResult.Clear();
        switch (InputState)
        {
            case INPUT_STATE.SELECT_CARD:
                enemyInput.HandleYourCardSelection(board, enemyHand, ref inputResult);
                if (!inputResult.IsValid())
                    break;
                PlaceCard(inputResult.x, inputResult.y, inputResult.selectedCard);
                if (Configuration.TetraMaster.TripleTriad >= 2)
                    GenerateTargetableTripleTriad(inputResult.x, inputResult.y);
                else
                    GenerateTargetable(inputResult.x, inputResult.y);
                _battleResult.attacker = inputResult.selectedCard;
                ++InputState;
                break;
            case INPUT_STATE.SELECT_BATTLE_TARGET:
                enemyInput.HandleTargetCardSelection(board, _battleResult.attacker, _selectableTargets, ref inputResult);
                if (!inputResult.IsValid())
                    break;
                _battleResult.defender = _selectableTargets.Count != 0 ? _selectableTargets[inputResult.index] : null;
                PlayState = PLAY_STATE.CALCULATE_BATTLE;
                break;
        }
    }

    public void GenerateTargetable(Int32 x, Int32 y)
    {
        _adjacentTargets = board.GetAdjacentCards(x, y);
        _selectableTargets.Clear();
        _beatableTargets.Clear();
        for (Int32 index = 0; index < CardArrow.MAX_ARROWNUM; ++index)
        {
            CardArrow.Type direction = (CardArrow.Type)index;
            QuadMistCard adjacentTarget = _adjacentTargets[index];
            if (adjacentTarget != null && adjacentTarget.side != inputResult.selectedCard.side)
            {
                Int32 num = CardArrow.CheckDirection(inputResult.selectedCard.arrow, adjacentTarget.arrow, direction);

                if (num == 2)
                    _selectableTargets.Add(_adjacentTargets[index]);
                if (num == 1)
                    _beatableTargets.Add(_adjacentTargets[index]);
            }
        }
    }

    public void GenerateTargetableTripleTriad(Int32 x, Int32 y)
    {
        _adjacentTargets = board.GetAdjacentCards(x, y);
        _selectableTargets.Clear();
        _beatableTargets.Clear();
        _adjacentTargets_rules_same.Clear();
        _adjacentTargets_rules_plus.Clear();
        int sumUP = 0;
        int sumRIGHT = 0;
        int sumDOWN = 0;
        int sumLEFT = 0;
        int TriggerRulePlus = 0;
        for (Int32 index = 0; index < CardArrow.MAX_ARROWNUM; ++index)
        {
            QuadMistCard adjacentTarget = _adjacentTargets[index];           
            if (adjacentTarget != null)
            {
                TripleTriadCard baseCardAttacker = TripleTriad.TripleTriadCardStats[inputResult.selectedCard.id];
                TripleTriadCard baseCardDefender = TripleTriad.TripleTriadCardStats[adjacentTarget.id];
                if (Configuration.TetraMaster.TripleTriad == 3)
                {            
                    if (HasTripleTrialRule_Plus)
                    {           
                        if (index == 0)
                        {
                            sumUP = baseCardAttacker.atk + baseCardDefender.matk;
                            TriggerRulePlus++;
                        }
                        if (index == 2)
                        {
                            sumRIGHT = baseCardAttacker.mdef + baseCardDefender.pdef;
                            TriggerRulePlus++;
                        }
                        if (index == 4)
                        {
                            sumDOWN = baseCardAttacker.matk + baseCardDefender.atk;
                            TriggerRulePlus++;
                        }
                        if (index == 6)
                        {
                            sumLEFT = baseCardAttacker.pdef + baseCardDefender.mdef;
                            TriggerRulePlus++;
                        }

                    }
                    if (HasTripleTrialRule_Same)
                    {
                        if (index == 0 && (baseCardAttacker.atk == baseCardDefender.matk) || (HasTripleTrialRule_SameWall && inputResult.x == 0 && baseCardAttacker.atk == 10))
                            _adjacentTargets_rules_same.Add(_adjacentTargets[index]);
                        if (index == 2 && (baseCardAttacker.mdef == baseCardDefender.pdef) || (HasTripleTrialRule_SameWall && inputResult.y == 2 && baseCardAttacker.mdef == 10))
                            _adjacentTargets_rules_same.Add(_adjacentTargets[index]);
                        if (index == 4 && (baseCardAttacker.matk == baseCardDefender.atk) || (HasTripleTrialRule_SameWall && inputResult.x == 2 && baseCardAttacker.matk == 10))
                            _adjacentTargets_rules_same.Add(_adjacentTargets[index]);
                        if (index == 6 && (baseCardAttacker.pdef == baseCardDefender.mdef) || (HasTripleTrialRule_SameWall && inputResult.y == 0 && baseCardAttacker.pdef == 10))
                            _adjacentTargets_rules_same.Add(_adjacentTargets[index]);
                    }
                    if (TriggerRulePlus > 1)
                    {
                        if ((sumUP == sumRIGHT || sumUP == sumDOWN || sumUP == sumLEFT) && sumUP > 0 && !_adjacentTargets_rules_plus.Contains(_adjacentTargets[0])) 
                            _adjacentTargets_rules_plus.Add(_adjacentTargets[0]);
                        if ((sumRIGHT == sumUP || sumRIGHT == sumDOWN || sumRIGHT == sumLEFT) && sumRIGHT > 0 && !_adjacentTargets_rules_plus.Contains(_adjacentTargets[2]))
                            _adjacentTargets_rules_plus.Add(_adjacentTargets[2]);
                        if ((sumDOWN == sumUP || sumDOWN == sumRIGHT || sumDOWN == sumLEFT) && sumDOWN > 0 && !_adjacentTargets_rules_plus.Contains(_adjacentTargets[4]))
                            _adjacentTargets_rules_plus.Add(_adjacentTargets[4]);
                        if ((sumLEFT == sumUP || sumLEFT == sumRIGHT || sumLEFT == sumDOWN) && sumLEFT > 0 && !_adjacentTargets_rules_plus.Contains(_adjacentTargets[6]))
                            _adjacentTargets_rules_plus.Add(_adjacentTargets[6]);
                    }
                }
                if (adjacentTarget.side != inputResult.selectedCard.side)
                {
                    if (index == 0 && baseCardAttacker.atk > baseCardDefender.matk)
                        _beatableTargets.Add(_adjacentTargets[index]);
                    else if (index == 2 && baseCardAttacker.mdef > baseCardDefender.pdef)
                        _beatableTargets.Add(_adjacentTargets[index]);
                    else if (index == 4 && baseCardAttacker.matk > baseCardDefender.atk)
                        _beatableTargets.Add(_adjacentTargets[index]);
                    else if (index == 6 && baseCardAttacker.pdef > baseCardDefender.mdef)
                        _beatableTargets.Add(_adjacentTargets[index]);
                }
            }
        }
        if (_adjacentTargets_rules_plus.Count >= 2)
        {
            if (TripleTrialRuleActivated != TripleTriadRulesID.COMBO)
                TripleTrialRuleActivated = TripleTriadRulesID.PLUS;
            for (Int32 cardindex = 0; cardindex < _adjacentTargets_rules_plus.Count; ++cardindex)
            {
                if (_adjacentTargets_rules_plus[cardindex].side != inputResult.selectedCard.side)
                {
                    _selectableTargets.Add(_adjacentTargets_rules_plus[cardindex]);
                }
            }
        }
        if (_adjacentTargets_rules_same.Count >= 2)
        {
            if (TripleTrialRuleActivated != TripleTriadRulesID.COMBO)
                TripleTrialRuleActivated = TripleTriadRulesID.SAME;
            for (Int32 cardindex = 0; cardindex < _adjacentTargets_rules_same.Count; ++cardindex)
            {
                if (_adjacentTargets_rules_same[cardindex].side != inputResult.selectedCard.side)
                {
                    _selectableTargets.Add(_adjacentTargets_rules_same[cardindex]);
                }                   
            }
        }
    }

    public void RemoveComboFromBeatable(QuadMistCard[] removal)
    {
        foreach (QuadMistCard quadMistCard in removal)
        {
            if (quadMistCard != null)
                _beatableTargets.Remove(quadMistCard);
        }
    }

    public QuadMistCard[] GenerateCombo(QuadMistCard card, Int32 sideOf)
    {
        QuadMistCard[] adjacentCards = board.GetAdjacentCards(card);
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

    public QuadMistCard[] GenerateComboTripleTriad(QuadMistCard card, Int32 sideOf)
    {
        QuadMistCard[] adjacentCards = board.GetAdjacentCardsTripleTriad(card);
        for (Int32 index = 0; index < adjacentCards.Length; ++index)
        {
            QuadMistCard quadMistCard2 = adjacentCards[index];
            if (quadMistCard2 == null || quadMistCard2.side == sideOf)
            {
                adjacentCards[index] = null;
            }
        }
        return adjacentCards;
    }

    public void PlaceCard(Int32 x, Int32 y, QuadMistCard card)
    {
        board[x, y] = card;
        UpdateScore();
    }

    public void UpdateScore()
    {
        Int32 playerBoardCnt = 0;
        Int32 enemyBoardCnt = 0;
        for (Int32 index = 0; index < Board.SIZE_Y * Board.SIZE_X; ++index)
        {
            if (board[index] != null && !board[index].IsBlock)
            {
                if (board[index].side == QuadMistCardUI.PLAYER_SIDE)
                    ++playerBoardCnt;
                else
                    ++enemyBoardCnt;
            }
        }
        PlayerScore = Configuration.TetraMaster.TripleTriad <= 1 ? playerBoardCnt : playerHand.Count + playerBoardCnt;
        EnemyScore = Configuration.TetraMaster.TripleTriad <= 1 ? enemyBoardCnt : enemyHand.Count + enemyBoardCnt;
    }

    public BattleCalculation Calculate(QuadMistCard attacker, QuadMistCard defender, Int32 boardX = -1, Int32 boardY = -1)
    {
        if (Configuration.TetraMaster.TripleTriad > 0)
            return CalculateTripleTriad(attacker, defender, boardX, boardY);
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
            Single ak = 1f - attacker.ArrowNumber / 10f; // The attack is 20% less susceptible to randomness.
            Single dk = 1f - defender.ArrowNumber / 8f;

            Int32 lowestAttack = (Int32)(battleCalculation.atkStart * ak);
            Int32 lowestDefense = (Int32)(battleCalculation.defStart * dk);

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

    public BattleCalculation CalculateTripleTriad(QuadMistCard attacker, QuadMistCard defender, Int32 boardX = -1, Int32 boardY = -1)
    {
        BattleCalculation battleCalculation = new BattleCalculation();
        TripleTriadCard baseCardAttacker = TripleTriad.TripleTriadCardStats[attacker.id];
        TripleTriadCard baseCardDefender = TripleTriad.TripleTriadCardStats[defender.id];
        Vector2 attackerLocation = new Vector2(boardX, boardY);
        if (boardX < 0 || boardY < 0)
            attackerLocation = board.GetCardLocation(attacker);
        Vector2 defenderLocation = board.GetCardLocation(defender);
        Vector2 defToAtk = attackerLocation - defenderLocation;
        if (defToAtk.x == 0f && defToAtk.y == 1f)
        {
            battleCalculation.atkStart = baseCardAttacker.atk;
            battleCalculation.defStart = baseCardDefender.matk;
        }
        else if (defToAtk.x == -1f && defToAtk.y == 1f)
        {
            battleCalculation.atkStart = (baseCardAttacker.atk + baseCardAttacker.mdef) / 2;
            battleCalculation.defStart = (baseCardDefender.matk + baseCardDefender.pdef) / 2;
        }
        else if (defToAtk.x == -1f && defToAtk.y == 0f)
        {
            battleCalculation.atkStart = baseCardAttacker.mdef;
            battleCalculation.defStart = baseCardDefender.pdef;
        }
        else if (defToAtk.x == -1f && defToAtk.y == -1f)
        {
            battleCalculation.atkStart = (baseCardAttacker.matk + baseCardAttacker.mdef) / 2;
            battleCalculation.defStart = (baseCardDefender.atk + baseCardDefender.pdef) / 2;
        }
        else if (defToAtk.x == 0f && defToAtk.y == -1f)
        {
            battleCalculation.atkStart = baseCardAttacker.matk;
            battleCalculation.defStart = baseCardDefender.atk;
        }
        else if (defToAtk.x == 1f && defToAtk.y == -1f)
        {
            battleCalculation.atkStart = (baseCardAttacker.matk + baseCardAttacker.pdef) / 2;
            battleCalculation.defStart = (baseCardDefender.atk + baseCardDefender.mdef) / 2;
        }
        else if (defToAtk.x == 1f && defToAtk.y == 0f)
        {
            battleCalculation.atkStart = baseCardAttacker.pdef;
            battleCalculation.defStart = baseCardDefender.mdef;
        }
        else
        {
            battleCalculation.atkStart = (baseCardAttacker.atk + baseCardAttacker.pdef) / 2;
            battleCalculation.defStart = (baseCardDefender.matk + baseCardDefender.mdef) / 2;
        }

        if (battleCalculation.atkStart == battleCalculation.defStart)
        {
            if ((GameRandom.Next16() % 2) == 0)
            {
                battleCalculation.atkFinish = 1;
                battleCalculation.defFinish = battleCalculation.atkStart - battleCalculation.defStart;
            }
            else
            {
                battleCalculation.atkFinish = battleCalculation.atkStart - battleCalculation.defStart;
                battleCalculation.defFinish = 1;
            }
        }
        else if (battleCalculation.atkStart > battleCalculation.defStart)
        {
            battleCalculation.atkFinish = battleCalculation.atkStart - battleCalculation.defStart;
            battleCalculation.defFinish = 0;
        }
        else
        {
            battleCalculation.atkFinish = 0;
            battleCalculation.defFinish = battleCalculation.defStart - battleCalculation.atkStart;
        }
        return battleCalculation;
    }

    public void SetGetCardMessage(Int32 type, Boolean active)
    {
        getCardMessage.ID = type;
        getCardMessage.gameObject.SetActive(active);
    }

    private void ClearHands()
    {
        enemyHand.Clear();
        playerHand.Clear();
    }

    private void ClearGameObjects()
    {
        preGame.gameObject.SetActive(false);
        playGame.gameObject.SetActive(false);
    }

    private void ClearStates()
    {
        GameState = GAME_STATE.PREGAME;
        PreGameState = PREGAME_STATE.SETUP;
        StartState = START_STATE.SETUP;
        PlayState = PLAY_STATE.INPUT_PLAYER;
        EndState = END_STATE.RESULT;
        PostState = POSTGAME_STATE.SELECT_CARD;
        InputState = INPUT_STATE.SELECT_CARD;
    }

    private void ClearInput()
    {
        inputResult = new InputResult();
    }

    public void AnimCoroutine(IEnumerator func)
    {
        StartCoroutine(InvokeCoroutine(func));
    }

    public void AnimCoroutine(params IEnumerator[] funcs)
    {
        StartCoroutine(InvokeCoroutine(Anim.Sequence(funcs)));
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
            SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_GET);
            enemyHand.Select = -1;
        }
        else if (result.type == MatchResult.Type.LOSE)
        {
            playerHand.Remove(selected);
            SoundEffect.Play(QuadMistSoundID.MINI_SE_WINDOW);
            playerHand.Select = -1;
        }
        StartCoroutine(RestoreCards(result));

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
            String monsterName = FF9TextTool.CardName(result.selectedCard.id);
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
            SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_GET);
        }
        else if (result.type == MatchResult.Type.LOSE)
        {
            playerHand.Remove(selected);
            SoundEffect.Play(QuadMistSoundID.MINI_SE_WINDOW);
        }

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
            String monsterName = FF9TextTool.CardName(result.selectedCard.id);
            resultTypeText = resultTypeText.Replace("[CARD]", monsterName);
        }
        QuadMistGetCardDialog.Show(new Vector3(0f, -0.6f, 0f), resultTypeText);
    }

    [DebuggerHidden]
    private IEnumerator RestoreCards(MatchResult result)
    {
        Action<QuadMistCardUI> swapToPlayer = c =>
        {
            c.Side = QuadMistCardUI.PLAYER_SIDE;
            if (Configuration.TetraMaster.TripleTriad == 0 && result.type == MatchResult.Type.WIN)
                c.Data.LevelUpInMatch();
        };
        Action<QuadMistCardUI> swapToEnemy = c => c.Side = QuadMistCardUI.ENEMY_SIDE;

        for (Int32 i = 0; i < playerHand.Count; i++)
            StartCoroutine(playerHand.GetCardUI(i).FlashNormal(swapToPlayer));
        for (Int32 i = 0; i < enemyHand.Count; i++)
            StartCoroutine(enemyHand.GetCardUI(i).FlashNormal(swapToEnemy));
        yield return StartCoroutine(Anim.Tick(CardEffect.FLASH_TICK_DURATION));
    }

    [DebuggerHidden]
    private IEnumerator ResultRestoreHands()
    {
        Int32 index = 0;
        while (index < matchCards.Length)
        {
            QuadMistCardUI cardindex = board.GetCardUI(matchCards[index]);
            if (cardindex != null)
            {
                if (index < 5)
                {
                    playerHand.GetCardUI(index).transform.position = cardindex.transform.position;
                    playerHand.AddWithoutChanged(matchCards[index]);
                }
                else
                {
                    enemyHand.GetCardUI(index - 5).transform.position = cardindex.transform.position;
                    enemyHand.AddWithoutChanged(matchCards[index]);
                }
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
                if (result.perfect && !HasTripleTrialRule_All)
                {
                    sound = QuadMistSoundID.MINI_SE_PERFECT;
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

    private void TripleTriadText(TripleTriadRulesID rules)
    {
        Int32 id = 0;
        switch (rules)
        {
            case TripleTriadRulesID.SAME:
                id = 4;
                break;
            case TripleTriadRulesID.PLUS:
                id = 5;
                break;
            case TripleTriadRulesID.COMBO:
                id = 6;
                break;
        }
        resultText.ID = id;

        if (rules != TripleTriadRulesID.COMBO)
        {
            Single brightness = 0f;
            resultText.Alpha = brightness;
            resultText.gameObject.SetActive(true);

            for (Int32 tick = 0; tick < 64; tick++)
            {
                brightness = (tick * 4) / 255f;
                if (brightness > 1f)
                {
                    brightness = 1f;
                }
                if (tick == 31)
                {
                    SoundEffect.Play(QuadMistSoundID.MINI_SE_WIN);
                }
                resultText.Alpha = brightness;
            }
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
            if (Configuration.TetraMaster.TripleTriad != 3)
            {
                StartCoroutine(BattleNumberTextAnimation(result, BATTLE_NUMBER_TIME + 32, BATTLE_COUNTDOWN_TIME + BATTLE_NUMBER_TIME, 45));
                yield return StartCoroutine(Anim.Tick(BATTLE_NUMBER_TIME));

                StartCoroutine(BattleMotionAnimation(atkUi, defUi, 7, 15));
                yield return StartCoroutine(BombAnimation(atkUi, defUi, 32));

            }

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
            if (Configuration.TetraMaster.TripleTriad == 3)
            {
                TripleTrialRuleActivated = TripleTriadRulesID.COMBO;
                TripleTriadText(TripleTrialRuleActivated);
            }
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
        SpriteText spriteText = battleNumber[i];
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
        SetBattleNumber(i, a, battleNumber[i].Text);
    }
}