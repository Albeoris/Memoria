using Assets.Sources.Scripts.UI.Common;
using FF9;
using System;
using System.Linq;
using System.Collections.Generic;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Database;
using Memoria.Prime;
using Memoria.Scenes;
using NCalc;
using UnityEngine;
using static Memoria.Assets.DataResources;
using Memoria.Prime.PsdFile;
using System.Collections;

public partial class BattleHUD : UIScene
{
    private readonly Dictionary<Int32, AbilityPlayerDetail> _abilityDetailDict;
    private readonly MagicSwordCondition _magicSwordCond;
    private readonly CommandMPCondition _commandMPCond;
    private readonly List<ParameterStatus> _currentCharacterHp;
    private readonly List<Boolean> _currentEnemyDieState;
    private readonly List<DamageAnimationInfo> _hpInfoVal;
    private readonly List<DamageAnimationInfo> _mpInfoVal;
    private readonly Dictionary<Int32, CommandMenu> _commandCursorMemorize;
    private readonly Dictionary<PairCharCommand, Int32> _abilityCursorMemorize;
    private readonly List<Int32> _matchBattleIdPlayerList;
    private readonly List<Int32> _matchBattleIdEnemyList;
    private readonly List<RegularItem> _itemIdList;
    private readonly Dictionary<String, Message> _messageQueue;

    private Single _lastFrameRightTriggerAxis;
    private Boolean _lastFramePressOnMenu;
    private Byte _currentLibraMessageNumber;
    private Byte _currentLibraMessageCount;
    private BattleUnit _libraBtlData;
    private LibraInformation _libraEnabledMessage;
    private Byte _currentPeepingMessageCount;
    private Boolean _currentPeepingReverseOrder;
    private BattleEnemy _peepingEnmData;
    private Byte _currentMessagePriority;
    private Single _battleMessageCounter;
    private UI.PanelCommand _commandPanel;
    private GOScrollablePanel _abilityPanel;
    private GOScrollablePanel _itemPanel;
    private Int32 _enemyCount;
    private Int32 _playerCount;
    private Int32 _playerDetailCount;
    private RecycleListPopulator _itemScrollList;
    private RecycleListPopulator _abilityScrollList;
    private Boolean _isTranceMenu;
    private Boolean _isManualTrance;
    private Boolean _needItemUpdate;
    private Boolean _currentSilenceStatus;
    private Int32 _currentMpValue;
    private Single _blinkAlphaCounter;
    private Int32 _tranceColorCounter;
    private UIWidget _battleDialogWidget;
    private UILabel _battleDialogLabel;
    private UIToggle _autoBattleToggle;
    private UIToggle _allTargetToggle;
    private UIButton _autoBattleButtonComponent;
    private UIButton _allTargetButtonComponent;
    private UI.PanelTarget _targetPanel;
    private UI.ContainerStatus _statusPanel;
    private UI.PanelParty _partyDetail;
    private Boolean _usingMainMenu;
    private PLAYER _mainMenuSinglePlayer;
    private List<PlayerMemo> _mainMenuPlayerMemo = new List<PlayerMemo>();
    private Boolean _commandEnable;
    private Boolean _beforePauseCommandEnable;
    private Boolean _isFromPause;
    private Boolean _isNeedToInit;
    private CommandMenu _currentCommandIndex;
    private BattleCommandId _currentCommandId;
    private String _currentButtonGroup;
    private Int32 _currentSubMenuIndex;
    private Int32 _currentTargetIndex;
    private List<Int32> _targetIndexList;
    private SubMenuType _subMenuType;
    private List<Int32> _unconsciousStateList;
    private Single _runCounter;
    private Boolean _hidingHud;
    private CursorGroup _cursorType;
    private Boolean _defaultTargetAlly;
    private Boolean _defaultTargetHealingAttack;
    private Boolean _defaultTargetDead;
    private Boolean _targetDead;
    private Int32 _bestTargetIndex;
    private TargetType _targetCursor;
    private Boolean _isTryingToRun;
    private Boolean _isAutoAttack;
    private Boolean _isAllTarget;
    private Byte _doubleCastCount;
    private CommandDetail _firstCommand;
    private Action _onResumeFromQuit;
    private Boolean _oneTime;
    private Single _buttonSlideFactor;
    private Vector2 _buttonSlidePos;
    private CommandMenu _buttonSlideInitial;
    private GONavigationButton _buttonSliding;
    private Int32 CurrentBattlePlayerIndex => _matchBattleIdPlayerList.IndexOf(CurrentPlayerIndex);

    private void DisplayBattleMessage(Message message)
    {
        String str = message.message;
        Boolean isRect = message.isRect;
        CMD_DATA cmd = message.titleCmd;
        BattleDialogGameObject.SetActive(false);
        if (isRect)
        {
            _battleDialogWidget.width = (Int32)(128.0 * UIManager.ResourceXMultipier);
            _battleDialogWidget.height = 120;
            _battleDialogWidget.transform.localPosition = new Vector3(0.0f, 445f, 0.0f);
            if (cmd != null && cmd.regist != null && !String.IsNullOrEmpty(Configuration.Interface.BattleCommandTitleFormat))
            {
                try
                {
                    Expression expr = new Expression(Configuration.Interface.BattleCommandTitleFormat);
                    NCalcUtility.InitializeExpressionUnit(ref expr, new BattleUnit(cmd.regist), "Caster");
                    NCalcUtility.InitializeExpressionCommand(ref expr, new BattleCommand(cmd));
                    expr.Parameters["CommandTitle"] = str;
                    expr.Parameters["IsEnemyCounter"] = cmd.cmd_no == BattleCommandId.EnemyCounter;
                    expr.Parameters["IsPlayerCounter"] = cmd.cmd_no == BattleCommandId.Counter || cmd.cmd_no == BattleCommandId.MagicCounter;
                    expr.Parameters["IsCounter"] = cmd.cmd_no == BattleCommandId.EnemyCounter || cmd.cmd_no == BattleCommandId.Counter || cmd.cmd_no == BattleCommandId.MagicCounter;
                    expr.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                    expr.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                    str = NCalcUtility.EvaluateNCalcString(expr.Evaluate(), str);
                }
                catch (Exception err)
                {
                    Log.Error(err);
                }
            }
        }
        else
        {
            _battleDialogWidget.width = (Int32)(240.0 * UIManager.ResourceXMultipier);
            if (str.Contains("\n"))
            {
                _battleDialogWidget.height = 200;
                _battleDialogWidget.transform.localPosition = new Vector3(-10f, 405f, 0.0f);
            }
            else
            {
                _battleDialogWidget.height = 120;
                _battleDialogWidget.transform.localPosition = new Vector3(-10f, 445f, 0.0f);
            }

            if (!String.IsNullOrEmpty(Configuration.Interface.BattleCommandTitleFormat))
            {
                try
                {
                    Expression expr = new Expression(Configuration.Interface.BattleCommandTitleFormat);
                    expr.Parameters["CommandTitle"] = str;
                    expr.Parameters["IsEnemyCounter"] = false;
                    expr.Parameters["IsPlayerCounter"] = str == FF9TextTool.BattleFollowText(14).Substring(1) || str == FF9TextTool.BattleFollowText(15).Substring(1);
                    expr.Parameters["IsCounter"] = str == FF9TextTool.BattleFollowText(14).Substring(1) || str == FF9TextTool.BattleFollowText(15).Substring(1); // BattleMesages.CounterAttack || BattleMesages.ReturnMagic
                    expr.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                    expr.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                    str = NCalcUtility.EvaluateNCalcString(expr.Evaluate(), str);
                }
                catch (Exception err)
                {
                    Log.Error(err);
                }
            }
        }

        Single additionalWidth = 0.0f;
        _battleDialogLabel.text = _battleDialogLabel.PhrasePreOpcodeSymbol(str, ref additionalWidth);
        BattleDialogGameObject.SetActive(true);
    }

    private void AdvanceLibraMessageNumber()
    {
        _currentLibraMessageCount = 0;
        _currentLibraMessageNumber++;
        while (_currentLibraMessageNumber < 7)
        {
            if (_currentLibraMessageNumber == 1 && (_libraEnabledMessage & LibraInformation.NameLevel) != 0)
                return;
            if (_currentLibraMessageNumber == 2 && (_libraEnabledMessage & LibraInformation.HPMP) != 0)
                return;
            if (_currentLibraMessageNumber == 3 && (_libraEnabledMessage & LibraInformation.Category) != 0)
                return;
            if (_currentLibraMessageNumber == 4 && (_libraEnabledMessage & LibraInformation.ElementWeak) != 0)
                return;
            if (_currentLibraMessageNumber == 5 && (_libraEnabledMessage & LibraInformation.ItemSteal) != 0)
                return;
            if (_currentLibraMessageNumber == 6 && (_libraEnabledMessage & LibraInformation.BlueLearn) != 0)
                return;
            _currentLibraMessageNumber++;
        }
    }

    private Boolean DisplayMessageLibra()
    {
        if (_libraBtlData == null)
            return false;

        if (_currentLibraMessageNumber == 1)
        {
            String str = String.Empty;
            if ((_libraEnabledMessage & LibraInformation.Name) != 0)
                str += _libraBtlData.Name;
            if ((_libraEnabledMessage & LibraInformation.Level) != 0)
                str += FF9TextTool.BattleLibraText(10) + _libraBtlData.Level.ToString();
            SetBattleMessage(str, 3);
            AdvanceLibraMessageNumber();
            return true;
        }
        if (_currentLibraMessageNumber == 2)
        {
            String str = String.Empty;
            if ((_libraEnabledMessage & LibraInformation.HP) != 0)
            {
                str += FF9TextTool.BattleLibraText(11);
                str += _libraBtlData.CurrentHp;
                str += FF9TextTool.BattleLibraText(13);
                str += _libraBtlData.MaximumHp;
            }
            if ((_libraEnabledMessage & LibraInformation.MP) != 0)
            {
                str += FF9TextTool.BattleLibraText(12);
                str += _libraBtlData.CurrentMp;
                str += FF9TextTool.BattleLibraText(13);
                str += _libraBtlData.MaximumMp;
            }
            SetBattleMessage(str, 3);
            AdvanceLibraMessageNumber();
            return true;
        }
        if (_currentLibraMessageNumber == 3)
        {
            if (!_libraBtlData.IsPlayer)
            {
                Int32 enemyCategory = _libraBtlData.Enemy.Data.et.category;
                while (_currentLibraMessageCount < 8 && (enemyCategory & 1 << _currentLibraMessageCount) == 0)
                    _currentLibraMessageCount++;
                if (_currentLibraMessageCount < 8)
                {
                    SetBattleMessage(FF9TextTool.BattleLibraText(_currentLibraMessageCount), 3);
                    _currentLibraMessageCount++;
                    return true;
                }
            }
            AdvanceLibraMessageNumber();
        }
        if (_currentLibraMessageNumber == 4)
        {
            EffectElement element = _libraBtlData.WeakElement & ~_libraBtlData.GuardElement;
            while (_currentLibraMessageCount < 8 && (element & (EffectElement)(1 << _currentLibraMessageCount)) == 0)
                _currentLibraMessageCount++;
            if (_currentLibraMessageCount < 8)
            {
                SetBattleMessage(Localization.GetSymbol() != "JP" ? FF9TextTool.BattleLibraText(14 + _currentLibraMessageCount) : BtlGetAttrName(1 << _currentLibraMessageCount) + FF9TextTool.BattleLibraText(14), 3);
                _currentLibraMessageCount++;
                return true;
            }
            AdvanceLibraMessageNumber();
        }
        if (_currentLibraMessageNumber == 5)
        {
            if (!_libraBtlData.IsPlayer)
            {
                RegularItem itemId = RegularItem.NoItem;
                BattleEnemy enemy = _libraBtlData.Enemy;
                while (_currentLibraMessageCount < enemy.StealableItems.Length && (itemId = enemy.StealableItems[_currentLibraMessageCount]) == RegularItem.NoItem)
                    _currentLibraMessageCount++;
                if (itemId != RegularItem.NoItem)
                {
                    SetBattleMessage(Localization.GetSymbol() != "JP" ? FF9TextTool.BattleLibraText(8) + FF9TextTool.ItemName(itemId) : FF9TextTool.ItemName(itemId) + FF9TextTool.BattleLibraText(8), 2);
                    _currentLibraMessageCount++;
                    return true;
                }
            }
            AdvanceLibraMessageNumber();
        }
        if (_currentLibraMessageNumber == 6)
        {
            if (!_libraBtlData.IsPlayer)
            {
                BattleEnemyPrototype enemyPrototype = BattleEnemyPrototype.Find(_libraBtlData);
                Int32 blueMagicId = enemyPrototype.BlueMagicId;
                String str;
                if (blueMagicId != 0)
                {
                    str = FF9TextTool.CommandName(BattleCommandId.BlueMagic) + ": ";
                    if (ff9abil.IsAbilityActive(blueMagicId))
                        str += FF9TextTool.ActionAbilityName(ff9abil.GetActiveAbilityFromAbilityId(blueMagicId));
                    else if (ff9abil.IsAbilitySupport(blueMagicId))
                        str += FF9TextTool.SupportAbilityName(ff9abil.GetSupportAbilityFromAbilityId(blueMagicId));
                }
                else
				{
                    str = FF9TextTool.CommandName(BattleCommandId.BlueMagic) + ": -";
                }
                SetBattleMessage(str, 3);
                AdvanceLibraMessageNumber();
                return true;
            }
            AdvanceLibraMessageNumber();
        }
        if (_currentLibraMessageNumber >= 7)
        {
            _libraBtlData = null;
            _currentLibraMessageCount = 0;
            _currentLibraMessageNumber = 0;
        }
        return false;
    }

    private Boolean DisplayMessagePeeping()
    {
        if (_peepingEnmData == null)
            return false;

        RegularItem id;
        do
        {
            Byte stealIndex = _currentPeepingMessageCount++;
            if (stealIndex > _peepingEnmData.StealableItems.Length)
            {
                _peepingEnmData = null;
                _currentPeepingMessageCount = 0;
                return false;
            }
            id = _peepingEnmData.StealableItems[_currentPeepingReverseOrder ? _peepingEnmData.StealableItems.Length - stealIndex : stealIndex - 1];
        } while (id == RegularItem.NoItem);

        SetBattleMessage(Localization.GetSymbol() != "JP"
            ? FF9TextTool.BattleLibraText(8) + FF9TextTool.ItemName(id)
            : FF9TextTool.ItemName(id) + FF9TextTool.BattleLibraText(8), 3);
        return true;
    }

    private void SetupCommandButton(GONavigationButton button, BattleCommandId cmdId, Boolean enabled, Boolean hardDisable = false)
    {
        enabled = enabled && !hardDisable;
        button.SetLabelText(hardDisable ? String.Empty : FF9TextTool.CommandName(cmdId));
        ButtonGroupState.SetButtonEnable(button, enabled);
        ButtonGroupState.SetButtonAnimation(button, enabled);
        if (enabled)
        {
            button.SetLabelColor(FF9TextTool.White);
            button.ButtonGroup.Help.Enable = true;
            button.ButtonGroup.Help.TextKey = String.Empty;
            button.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription(cmdId);
        }
        else
        {
            button.SetLabelColor(FF9TextTool.Gray);
            button.BoxCollider.enabled = false;
            button.ButtonGroup.Help.Enable = false;
        }
    }

    private Boolean CommandIsEnabled(BTL_DATA btl, BattleCommandId cmdId)
	{
        CharacterCommandType commandType = CharacterCommands.Commands[cmdId].Type;
        if (cmdId == BattleCommandId.None)
            return false;
        if (commandType == CharacterCommandType.Normal && CharacterCommands.Commands[cmdId].MainEntry > 0)
        {
            BattleUnit unit = new BattleUnit(btl);
            BattleAbilityId patchedID = PatchAbility(ff9abil.GetActiveAbilityFromAbilityId(CharacterCommands.Commands[cmdId].MainEntry));
            if (GetActionMpCost(FF9StateSystem.Battle.FF9Battle.aa_data[patchedID], unit) > btl.cur.mp)
                return false;
        }
        if (!btl.is_monster_transform)
            return true;
        BTL_DATA.MONSTER_TRANSFORM transform = btl.monster_transform;
        if (transform.base_command == cmdId)
            return true;
        if (cmdId == BattleCommandId.Attack && transform.attack[btl.bi.def_idle] == null)
            return false;
        return !transform.disable_commands.Contains(cmdId);
    }

    private void DisplayCommand()
    {
        BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
        BTL_DATA.MONSTER_TRANSFORM transform = btl.Data.is_monster_transform ? btl.Data.monster_transform : null;
        CharacterPresetId presetId = FF9StateSystem.Common.FF9.party.GetCharacter(btl.Position).PresetId;
        BattleCommandId command1;
        BattleCommandId command2;

        if (btl.IsUnderAnyStatus(BattleStatus.Trance))
        {
            command1 = CharacterCommands.CommandSets[presetId].Trance1;
            command2 = CharacterCommands.CommandSets[presetId].Trance2;
            _commandPanel.SetCaptionText(Localization.Get("TranceCaption"));
            _isTranceMenu = true;
        }
        else
        {
            command1 = CharacterCommands.CommandSets[presetId].Regular1;
            command2 = CharacterCommands.CommandSets[presetId].Regular2;
            _commandPanel.SetCaptionText(Localization.Get("CommandCaption"));
            _commandPanel.SetCaptionColor(FF9TextTool.White);
            _isTranceMenu = false;
        }

        if (btl.Data.is_monster_transform && transform.base_command == command1)
            command1 = transform.new_command;
        if (btl.Data.is_monster_transform && transform.base_command == command2)
            command2 = transform.new_command;
        Boolean attackIsEnable = CommandIsEnabled(btl.Data, BattleCommandId.Attack);
        Boolean changeIsEnable = CommandIsEnabled(btl.Data, BattleCommandId.Change);
        Boolean itemIsEnable = CommandIsEnabled(btl.Data, BattleCommandId.Item);
        Boolean defendIsEnable = CommandIsEnabled(btl.Data, BattleCommandId.Defend);
        Boolean menuIsEnable = CommandIsEnabled(btl.Data, BattleCommandId.AccessMenu);
        BattleCommandId attackCmdId = !btl.Data.is_monster_transform || transform.base_command != BattleCommandId.Attack ? BattleCommandId.Attack : transform.new_command;
        BattleCommandId changeCmdId = !btl.Data.is_monster_transform || transform.base_command != BattleCommandId.Change ? BattleCommandId.Change : transform.new_command;
        BattleCommandId itemCmdId = !btl.Data.is_monster_transform || transform.base_command != BattleCommandId.Item ? BattleCommandId.Item : transform.new_command;
        BattleCommandId defendCmdId = !btl.Data.is_monster_transform || transform.base_command != BattleCommandId.Defend ? BattleCommandId.Defend : transform.new_command;
        Boolean command1IsEnable = CommandIsEnabled(btl.Data, command1);
        Boolean command2IsEnable = CommandIsEnabled(btl.Data, command2);
        Boolean noMagicSword = false;
        if (command2 == BattleCommandId.MagicSword)
        {
            if (!_magicSwordCond.IsViviExist)
            {
                noMagicSword = true;
                command2IsEnable = false;
            }
            else if (_magicSwordCond.IsViviDead || _magicSwordCond.IsSteinerMini)
            {
                command2IsEnable = false;
            }
        }

        if (Configuration.Battle.NoAutoTrance && btl.Trance == Byte.MaxValue && !btl.IsUnderAnyStatus(BattleStatus.Trance))
        {
            if (!_isManualTrance)
            {
                _commandPanel.Change.SetLabelText(Localization.Get("Trance"));
                _commandPanel.Change.SetLabelColor(FF9TextTool.Yellow);
                _commandPanel.Change.ButtonGroup.Help.Text = "Activates the trance mode.";
                _isManualTrance = true;
            }
        }
        else
        {
            SetupCommandButton(_commandPanel.Change, changeCmdId, changeIsEnable);
            _isManualTrance = false;
        }

        if (Configuration.Mod.TranceSeek) // TRANCE SEEK - Special commands
        {
            if (presetId == CharacterPresetId.Zidane)
            {
                CharacterCommands.CommandSets[presetId].Regular2 = btl_util.getSerialNumber(btl) == CharacterSerialNumber.ZIDANE_DAGGER ? BattleCommandId.SecretTrick : (BattleCommandId)10001;
                command2 = btl.IsUnderAnyStatus(BattleStatus.Trance) ? CharacterCommands.CommandSets[presetId].Trance2 : btl_util.getSerialNumber(btl) == CharacterSerialNumber.ZIDANE_DAGGER ? BattleCommandId.SecretTrick : (BattleCommandId)10001;
            }
            else if (presetId == CharacterPresetId.Steiner)
                defendCmdId = (BattleCommandId)10015; // Gardien
            else if (presetId == CharacterPresetId.Amarant)
                defendCmdId = (BattleCommandId)10016; // Duel
        }

        SetupCommandButton(_commandPanel.Skill1, command1, command1IsEnable);
        SetupCommandButton(_commandPanel.Skill2, command2, command2IsEnable, noMagicSword);
        SetupCommandButton(_commandPanel.Attack, attackCmdId, attackIsEnable);
        SetupCommandButton(_commandPanel.Defend, defendCmdId, defendIsEnable);
        SetupCommandButton(_commandPanel.Item, itemCmdId, itemIsEnable);
        if (_commandPanel.AccessMenu != null)
            SetupCommandButton(_commandPanel.AccessMenu, BattleCommandId.AccessMenu, menuIsEnable && Configuration.Battle.AccessMenus > 0);

        if (ButtonGroupState.ActiveGroup != CommandGroupButton)
            return;

        SetCommandVisibility(true, false);
    }

    private void DisplayStatus(TargetDisplay subMode)
    {
        StatusContainer.SetActive(true);
        _statusPanel.SetActive(false);
        _partyDetail.SetActive(true);

        List<Int32> list = new List<Int32>(new[] {0, 1, 2, 3});
        switch (subMode)
        {
            case TargetDisplay.Hp:
                DisplayTargetHp(list);
                break;
            case TargetDisplay.Mp:
                DisplayTargetMp(list);
                break;
            case TargetDisplay.Debuffs:
                DisplayTargetStatus(list, _statusPanel.BadStatus, DebuffIconNames);
                break;
            case TargetDisplay.Buffs:
                DisplayTargetStatus(list, _statusPanel.GoodStatus, BuffIconNames);
                break;
        }
    }

    private void DisplayTargetHp(List<Int32> list)
    {
        _statusPanel.HP.IsActive = true;
        _partyDetail.SetActive(false);

        foreach (KnownUnit ku in EnumerateKnownPlayers())
        {
            Int32 index = ku.Index;
            BattleUnit player = ku.Unit;

            UI.ContainerStatus.ValueWidget numberSubModeHud = _statusPanel.HP.Array[index];
            numberSubModeHud.IsActive = true;
            numberSubModeHud.Value.SetText(player.CurrentHp.ToString());
            numberSubModeHud.MaxValue.SetText(player.MaximumHp.ToString());
            if (!player.IsTargetable)
            {
                numberSubModeHud.SetColor(FF9TextTool.Gray);
            }
            else
            {
                switch (CheckHPState(player))
                {
                    case ParameterStatus.Dead:
                        numberSubModeHud.SetColor(FF9TextTool.Red);
                        break;
                    case ParameterStatus.Critical:
                        numberSubModeHud.SetColor(FF9TextTool.Yellow);
                        break;
                    default:
                        numberSubModeHud.SetColor(FF9TextTool.White);
                        break;
                }
            }
            list.Remove(index);
        }

        foreach (Int32 index in list)
            _statusPanel.HP.Array[index].IsActive = false;
    }

    private void DisplayTargetMp(List<Int32> list)
    {
        _statusPanel.MP.IsActive = true;
        _partyDetail.SetActive(false);

        foreach (KnownUnit ku in EnumerateKnownPlayers())
        {
            Int32 index = ku.Index;
            BattleUnit player = ku.Unit;

            UI.ContainerStatus.ValueWidget numberSubModeHud = _statusPanel.MP.Array[index];
            numberSubModeHud.IsActive = true;
            numberSubModeHud.Value.SetText(player.CurrentMp.ToString());
            numberSubModeHud.MaxValue.SetText(player.MaximumMp.ToString());

            if (!player.IsTargetable)
                numberSubModeHud.SetColor(FF9TextTool.Gray);
            else if (CheckMPState(player) == ParameterStatus.Dead)
                numberSubModeHud.SetColor(FF9TextTool.Yellow);
            else
                numberSubModeHud.SetColor(FF9TextTool.White);

            list.Remove(index);
        }

        foreach (Int32 index in list)
            _statusPanel.MP.Array[index].IsActive = false;
    }

    private void DisplayTargetStatus(List<Int32> list, UI.ContainerStatus.PanelDetail<UI.ContainerStatus.IconsWidget> statusPanel, Dictionary<BattleStatus, String> iconNames)
    {
        statusPanel.IsActive = true;
        _partyDetail.SetActive(false);

        foreach (KnownUnit ku in EnumerateKnownPlayers())
        {
            Int32 index1 = ku.Index;
            BattleUnit player = ku.Unit;

            UI.ContainerStatus.IconsWidget uiStatus = statusPanel.Array[index1];
            uiStatus.IsActive = true;
            foreach (GOSprite uiWidget in uiStatus.Icons.Entries)
                uiWidget.Sprite.alpha = 0.0f;

            Int32 iconIndex = 0;
            foreach (KeyValuePair<BattleStatus, String> status in iconNames)
            {
                if (!player.IsUnderAnyStatus(status.Key) || iconIndex >= 8)
                    continue;

                UISprite sprite = uiStatus.Icons[iconIndex].Sprite;
                sprite.alpha = 1f;
                sprite.spriteName = status.Value;
                iconIndex++;

                if (iconIndex > uiStatus.Icons.Count)
                    break;
            }
            list.Remove(index1);
        }

        foreach (Int32 index in list)
            statusPanel.Array[index].IsActive = false;
    }

    /// <summary>
    /// </summary>
    /// <remarks>There may be invisible players here, we must ignore them.</remarks>
    private IEnumerable<KnownUnit> EnumerateKnownPlayers()
    {
        Int32 index = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;

            // Issue #173: include non-targetable players (display their status in grey in DisplayTargetHP etc.)
            yield return new KnownUnit(index++, unit);
        }
    }

    /// <summary>
    /// </summary>
    /// <remarks>There may be invisible enemies here, we must ignore them.</remarks>
    private IEnumerable<KnownUnit> EnumerateKnownEnemies()
    {
        foreach (KnownUnit unit in EnumerateKnownUnits())
        {
            if (!unit.Unit.IsPlayer)
                yield return new KnownUnit(unit.Index - HonoluluBattleMain.EnemyStartIndex, unit.Unit);
        }
    }

    /// <summary>
    /// </summary>
    /// <remarks>There may be invisible units here, we must ignore them.</remarks>
    private IEnumerable<KnownUnit> EnumerateKnownUnits()
    {
        Int32 index = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsTargetable)
                continue;

            yield return new KnownUnit(index++, unit);
        }
    }

    private void DisplayAbilityRealTime()
    {
        BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
        if (_currentSilenceStatus != unit.IsUnderAnyStatus(BattleStatus.Silence))
        {
            _currentSilenceStatus = !_currentSilenceStatus;
            DisplayAbility();
        }

        if (_currentMpValue != unit.CurrentMp)
        {
            _currentMpValue = (Int32)unit.CurrentMp;
            DisplayAbility();
        }
    }

    private void DisplayItemRealTime()
    {
        if (!_needItemUpdate)
            return;

        _needItemUpdate = false;
        DisplayItem(CharacterCommands.Commands[_currentCommandId].Type == CharacterCommandType.Throw);
    }

    private void DisplayItem(Boolean isThrow)
    {
        _itemIdList.Clear();
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();
        foreach (FF9ITEM ff9Item in FF9StateSystem.Common.FF9.item)
        {
            if (ff9Item.count <= 0)
                continue;
            if ((isThrow && ff9item.CanThrowItem(ff9Item.id)) || (!isThrow && ff9item.HasItemEffect(ff9Item.id)))
            {
                _itemIdList.Add(ff9Item.id);
                BattleItemListData battleItemListData = new BattleItemListData
                {
                    Count = ff9Item.count,
                    Id = ff9Item.id
                };
                inDataList.Add(battleItemListData);
            }
        }

        if (inDataList.Count == 0)
        {
            _itemIdList.Add(RegularItem.NoItem);
            BattleItemListData battleItemListData = new BattleItemListData
            {
                Count = 0,
                Id = RegularItem.NoItem
            };
            inDataList.Add(battleItemListData);
        }

        if (_itemScrollList.ItemsPool.Count == 0)
        {
            _itemScrollList.PopulateListItemWithData = DisplayItemDetail;
            _itemScrollList.OnRecycleListItemClick += OnListItemClick;
            _itemScrollList.InitTableView(inDataList, 0);
        }
        else
        {
            _itemScrollList.SetOriginalData(inDataList);
        }
    }

    private Boolean CommandIsMonsterTransformCommand(Int32 playerIndex, BattleCommandId cmdId, out BTL_DATA.MONSTER_TRANSFORM transform)
    {
        if (playerIndex < 0)
        {
            transform = null;
            return false;
        }
        BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(playerIndex);
        transform = btl.Data.monster_transform;
        return btl.Data.is_monster_transform && btl.Data.monster_transform.new_command == cmdId;
    }

    private AA_DATA GetSelectedActiveAbility(Int32 playerIndex, BattleCommandId cmdId, Int32 abilityIndex, out Int32 subNo)
    {
        CharacterCommand ff9Command = CharacterCommands.Commands[cmdId];
        if (CommandIsMonsterTransformCommand(playerIndex, cmdId, out BTL_DATA.MONSTER_TRANSFORM transform))
        {
            subNo = ff9Command.ListEntry[abilityIndex];
            return transform.spell[subNo];
        }
        BattleAbilityId abilityId = PatchAbility(ff9Command.GetAbilityId(abilityIndex));
        subNo = (Int32)abilityId;
        return FF9StateSystem.Battle.FF9Battle.aa_data[abilityId];
    }

    private void DisplayAbility()
    {
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();
        CharacterCommand ff9Command = CharacterCommands.Commands[_currentCommandId];
        if (CommandIsMonsterTransformCommand(CurrentPlayerIndex, _currentCommandId, out BTL_DATA.MONSTER_TRANSFORM transform))
        {
            for (Int32 i = 0; i < ff9Command.ListEntry.Length; i++)
                inDataList.Add(new BattleAbilityListData { Id = ff9Command.ListEntry[i] });
        }
        else
        {
            SetAbilityAp(_abilityDetailDict[CurrentPlayerIndex]);
            foreach (BattleAbilityId abilId in ff9Command.EnumerateAbilities())
                inDataList.Add(new BattleAbilityListData { Id = ff9abil.GetAbilityIdFromActiveAbility(abilId) });
        }

        if (_abilityScrollList.ItemsPool.Count == 0)
        {
            _abilityScrollList.PopulateListItemWithData = DisplayAbilityDetail;
            _abilityScrollList.OnRecycleListItemClick += OnListItemClick;
            _abilityScrollList.InitTableView(inDataList, 0);
        }
        else
        {
            _abilityScrollList.SetOriginalData(inDataList);
        }
    }

    private void DisplayAbilityDetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        if (CurrentPlayerIndex < 0) // Note: I think this situation may happen for a frame when a character dies
            return;

        BattleAbilityListData battleAbilityListData = (BattleAbilityListData)data;
        BattleUnit curUnit = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);

        ItemListDetailHUD itemListDetailHud = new ItemListDetailHUD(item.gameObject);
        if (isInit)
            DisplayWindowBackground(item.gameObject, null);

        Boolean isMonsterTransformCommand = CommandIsMonsterTransformCommand(CurrentPlayerIndex, _currentCommandId, out BTL_DATA.MONSTER_TRANSFORM transform);
        AbilityStatus abilityState = isMonsterTransformCommand ? GetMonsterTransformAbilityState(battleAbilityListData.Id) : GetAbilityState(battleAbilityListData.Id);

        if (abilityState == AbilityStatus.None)
        {
            itemListDetailHud.Content.SetActive(false);
            ButtonGroupState.SetButtonAnimation(itemListDetailHud.Self, false);
            itemListDetailHud.Button.Help.TextKey = String.Empty;
            itemListDetailHud.Button.Help.Text = String.Empty;
        }
        else
        {
            itemListDetailHud.Content.SetActive(true);
            itemListDetailHud.Button.Help.TextKey = String.Empty;

            Int32 mp;
            if (isMonsterTransformCommand)
            {
                AA_DATA aaData = transform.spell[battleAbilityListData.Id];
                mp = GetActionMpCost(aaData, curUnit);
                itemListDetailHud.NameLabel.text = aaData.Name;
                itemListDetailHud.Button.Help.Text = String.Empty;
            }
            else
            {
                BattleAbilityId patchedID = PatchAbility(ff9abil.GetActiveAbilityFromAbilityId(battleAbilityListData.Id));
                mp = GetActionMpCost(FF9StateSystem.Battle.FF9Battle.aa_data[patchedID], curUnit);
                itemListDetailHud.NameLabel.text = FF9TextTool.ActionAbilityName(patchedID);
                itemListDetailHud.Button.Help.Text = FF9TextTool.ActionAbilityHelpDescription(patchedID);
            }
            itemListDetailHud.NumberLabel.text = mp == 0 ? String.Empty : mp.ToString();

            if (abilityState == AbilityStatus.Disable)
            {
                itemListDetailHud.NameLabel.color = FF9TextTool.Gray;
                itemListDetailHud.NumberLabel.color = FF9TextTool.Gray;
                ButtonGroupState.SetButtonAnimation(itemListDetailHud.Self, false);
            }
            else
            {
                itemListDetailHud.NameLabel.color = FF9TextTool.White;
                itemListDetailHud.NumberLabel.color = FF9TextTool.White;
                ButtonGroupState.SetButtonAnimation(itemListDetailHud.Self, true);
            }
        }
    }

    private void UpdateTargetStates()
    {
        Boolean shouldUpdatePointer = false;
        Int32 enemyCountOld = _enemyCount;
        Int32 playerCountOld = _playerCount;
        Int32 enemyCount = 0;
        Int32 playerCount = 0;
        List<Int32> currentBattleIdEnemyList = new List<Int32>();

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsTargetable)
                continue;

            if (unit.IsPlayer)
                playerCount++;
            else
            {
                currentBattleIdEnemyList.Add(unit.GetIndex());
                enemyCount++;
            }       
        }

        if (enemyCount != enemyCountOld || playerCount != playerCountOld || !Enumerable.SequenceEqual(currentBattleIdEnemyList, _matchBattleIdEnemyList))
        {
            shouldUpdatePointer = true;
            _matchBattleIdPlayerList.Clear();
            _currentCharacterHp.Clear();
            _matchBattleIdEnemyList.Clear();
            _currentEnemyDieState.Clear();
            _enemyCount = enemyCount;
            _playerCount = playerCount;
        }

        Int32 playerIndex = 0;
        Int32 enemyIndex = 0;

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (unit.Id == 0 || !unit.IsTargetable)
                continue;

            Int32 unitIndex = unit.GetIndex();

            if (unit.IsPlayer)
            {
                ParameterStatus playerHpState = CheckHPState(unit);
                if (playerIndex >= _currentCharacterHp.Count)
                {
                    _currentCharacterHp.Add(playerHpState);
                    _matchBattleIdPlayerList.Add(unitIndex);
                    shouldUpdatePointer = true;
                }
                else if (playerHpState != _currentCharacterHp[playerIndex])
                {
                    _currentCharacterHp[playerIndex] = playerHpState;
                    shouldUpdatePointer = true;
                }
                ++playerIndex;
            }
            else
            {
                Boolean enemyIsDead = unit.IsUnderStatus(BattleStatus.Death);               
                if (enemyIndex >= _currentEnemyDieState.Count)
                {
                    _currentEnemyDieState.Add(enemyIsDead);
                    _matchBattleIdEnemyList.Add(unitIndex);
                    shouldUpdatePointer = true;
                }
                else if (enemyIsDead != _currentEnemyDieState[enemyIndex])
                {
                    _currentEnemyDieState[enemyIndex] = enemyIsDead;
                    shouldUpdatePointer = true;
                }
                ++enemyIndex;
            }
        }

        if (!shouldUpdatePointer)
            return;

        foreach (GONavigationButton targetHud in _targetPanel.EnumerateTargets())
        {
            targetHud.KeyNavigation.startsSelected = false;
            targetHud.IsActive = false;
        }

        GameObject currentTargetLabel = null;
        playerIndex = 0;
        enemyIndex = 0;
        if (_cursorType == CursorGroup.Individual)
            currentTargetLabel = ButtonGroupState.ActiveButton;

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (unit.Id == 0 || !unit.IsTargetable)
                continue;

            if (unit.IsPlayer)
            {
                GONavigationButton playerHUD = _targetPanel.Players[playerIndex];
                GameObject labelObj = playerHUD.GameObject;
                UILabel nameLabel = playerHUD.Name.Label;
                labelObj.SetActive(true);
                nameLabel.text = unit.Player.Name;
                if (_currentCharacterHp[playerIndex] == ParameterStatus.Dead)
                {
                    if (_cursorType == CursorGroup.Individual)
                    {
                        if (!_targetDead)
                        {
                            ButtonGroupState.SetButtonEnable(labelObj, false);
                            if (labelObj == currentTargetLabel)
                            {
                                Int32 firstPlayer = GetFirstAlivePlayerIndex();
                                if (firstPlayer != -1)
                                {
                                    _currentTargetIndex = firstPlayer;
                                    currentTargetLabel = _targetPanel.Players[firstPlayer].GameObject;
                                }
                                else
                                {
                                    Debug.LogError("NO player active !!");
                                }
                                Singleton<PointerManager>.Instance.RemovePointerFromGameObject(labelObj);
                            }
                        }
                        else
                        {
                            ButtonGroupState.SetButtonEnable(labelObj, true);
                        }
                    }
                    nameLabel.color = FF9TextTool.Red;
                }
                else if (_currentCharacterHp[playerIndex] == ParameterStatus.Critical)
                {
                    if (_cursorType == CursorGroup.Individual)
                        ButtonGroupState.SetButtonEnable(labelObj, true);
                    nameLabel.color = FF9TextTool.Yellow;
                }
                else
                {
                    if (_cursorType == CursorGroup.Individual)
                        ButtonGroupState.SetButtonEnable(labelObj, true);
                    nameLabel.color = FF9TextTool.White;
                }
                ++playerIndex;
            }
            else
            {
                GONavigationButton enemyHUD = _targetPanel.Enemies[enemyIndex];
                GameObject labelObj = enemyHUD.GameObject;
                UILabel nameLabel = enemyHUD.Name.Label;
                Single additionalWidth = 0.0f;
                labelObj.SetActive(true);
                nameLabel.text = nameLabel.PhrasePreOpcodeSymbol(unit.Enemy.Name, ref additionalWidth);
                if (_currentEnemyDieState[enemyIndex])
                {
                    if (_cursorType == CursorGroup.Individual)
                    {
                        ButtonGroupState.SetButtonEnable(labelObj, false);
                        if (_targetDead == false)
                        {
                            if (labelObj == currentTargetLabel)
                            {
                                Int32 nextValidTarget = GetFirstAliveEnemyIndex();
                                if (nextValidTarget != -1)
                                {
                                    if (_currentCommandIndex == CommandMenu.Attack && FF9StateSystem.PCPlatform && _enemyCount > 1)
                                    {
                                        if (_currentTargetIndex == nextValidTarget && nextValidTarget + 1 < _targetPanel.AllTargets.Length)
                                            nextValidTarget++;
                                        ValidateDefaultTarget(ref nextValidTarget);
                                    }
                                    _currentTargetIndex = nextValidTarget;
                                    currentTargetLabel = _targetPanel.AllTargets[nextValidTarget].GameObject;
                                }
                                else
                                {
                                    Debug.LogError("NO enemy active !!");
                                }
                                Singleton<PointerManager>.Instance.RemovePointerFromGameObject(labelObj);
                            }
                        }
                        else
                        {
                            ButtonGroupState.SetButtonEnable(labelObj, true);
                        }
                    }
                    nameLabel.color = FF9TextTool.Gray;
                }
                else
                {
                    if (_cursorType == CursorGroup.Individual)
                        ButtonGroupState.SetButtonEnable(labelObj, true);
                    nameLabel.color = FF9TextTool.White;
                }
                ++enemyIndex;
            }
        }

        if (ButtonGroupState.ActiveGroup == TargetGroupButton)
            SetTargetAvailability(_targetCursor);
        if ((enemyCountOld != enemyCount || playerCountOld != playerCount) && ButtonGroupState.ActiveGroup == TargetGroupButton)
        {
            SetTargetDefault();
            modelButtonManager.Reset();
            EnableTargetArea();
            SetTargetHelp();
            ButtonGroupState.DisableAllGroup(true);
            ButtonGroupState.ActiveGroup = TargetGroupButton;
        }

        if (_cursorType == CursorGroup.Individual && currentTargetLabel != null && currentTargetLabel.activeSelf)
            ButtonGroupState.ActiveButton = currentTargetLabel;
        else
            DisplayTargetPointer();
    }

    private void DisplayCharacterParameter(UI.PanelParty.Character playerHud, BattleUnit bd, DamageAnimationInfo hp, DamageAnimationInfo mp)
    {
        playerHud.Name.SetText(bd.Player.Name);
        playerHud.HP.SetText(hp.CurrentValue.ToString());
        playerHud.MP.SetText(mp.CurrentValue.ToString());
        ParameterStatus parameterStatus = CheckHPState(bd);

        switch (parameterStatus)
        {
            case ParameterStatus.Dead:
                playerHud.ATBBar.SetProgress(0.0f);
                playerHud.HP.SetColor(FF9TextTool.Red);
                playerHud.Name.SetColor(FF9TextTool.Red);
                playerHud.ATBBlink = false;
                playerHud.TranceBlink = false;
                break;
            case ParameterStatus.Critical:
                playerHud.ATBBar.SetProgress(bd.CurrentAtb / (Single)bd.MaximumAtb);
                playerHud.HP.SetColor(FF9TextTool.Yellow);
                playerHud.Name.SetColor(FF9TextTool.Yellow);
                break;
            default:
                playerHud.ATBBar.SetProgress(bd.CurrentAtb / (Single)bd.MaximumAtb);
                playerHud.HP.SetColor(FF9TextTool.White);
                playerHud.Name.SetColor(FF9TextTool.White);
                break;
        }

        playerHud.MP.SetColor(CheckMPState(bd) == ParameterStatus.Critical ? FF9TextTool.Yellow : FF9TextTool.White);
        String spriteName = ATENormal;

        if (bd.IsUnderAnyStatus(BattleStatusConst.ATBGrey))
            spriteName = ATEGray;
        else if (bd.IsUnderAnyStatus(BattleStatusConst.ATBOrange))
            spriteName = ATEOrange;

        playerHud.ATBBar.Foreground.Foreground.Sprite.spriteName = spriteName;
        if (!bd.HasTrance)
            return;

        playerHud.TranceBar.SetProgress(bd.Trance / 256f);
        if (parameterStatus == ParameterStatus.Dead)
            return;

        if (bd.Trance == Byte.MaxValue && !playerHud.TranceBlink)
        {
            playerHud.TranceBlink = true;
        }
        else if (bd.Trance != Byte.MaxValue)
        {
            playerHud.TranceBlink = false;
        }
    }


    private void ManageAbility()
    {
        _abilityDetailDict.Clear();

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;

            Int32 index = unit.GetIndex();
            AbilityPlayerDetail abilityPlayer = new AbilityPlayerDetail { Player = unit.Player };
            abilityPlayer.HasAp = ff9abil.FF9Abil_HasAp(abilityPlayer.Player);
            SetAbilityAp(abilityPlayer);
            SetAbilityEquip(abilityPlayer);
            SetAbilityTrance(abilityPlayer);
            SetAbilityMagic(abilityPlayer);
            _abilityDetailDict[index] = abilityPlayer;
        }
    }

    private static ParameterStatus CheckHPState(BattleUnit bd)
    {
        if (bd.IsUnderStatus(BattleStatus.Death))
            return ParameterStatus.Dead;

        if (bd.IsPlayer && bd.CurrentHp <= bd.MaximumHp / 6.0)
            return ParameterStatus.Critical;

        return ParameterStatus.Normal;
    }

    private static ParameterStatus CheckMPState(BattleUnit bd)
    {
        return bd.CurrentMp <= bd.MaximumMp / 6.0 ? ParameterStatus.Critical : ParameterStatus.Normal;
    }

    private void CheckDoubleCast(Int32 battleIndex, CursorGroup cursorGroup)
    {
        if (IsDoubleCast && _doubleCastCount == 2 || !IsDoubleCast)
        {
            _doubleCastCount = 0;
            SetTarget(battleIndex);
        }
        else
        {
            if (!IsDoubleCast || _doubleCastCount >= 2)
                return;

            ++_doubleCastCount;
            _firstCommand = ProcessCommand(battleIndex, cursorGroup);
            _subMenuType = SubMenuType.Ability;

            DisplayAbility();
            SetTargetVisibility(false);
            SetAbilityPanelVisibility(true, true);
            BackButton.SetActive(FF9StateSystem.MobilePlatform);
        }
    }

    private void CheckPlayerState()
    {
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;

            Int32 index = unit.GetIndex();

            if (!IsEnableInput(unit))
            {
                if (!_unconsciousStateList.Contains(index))
                    _unconsciousStateList.Add(index);
            }
            else if (_unconsciousStateList.Contains(index))
            {
                _unconsciousStateList.Remove(index);
            }
        }
    }

    private void SwitchPlayer(Int32 playerId)
    {
        SetIdle();
        FF9Sfx.FF9SFX_Play(1044);
        CurrentPlayerIndex = playerId;

        UI.PanelParty.Character playerDetailHud = _partyDetail.FindCharacter(playerId);
        playerDetailHud.BoxCollider.enabled = false;
        playerDetailHud.ButtonColor.SetState(UIButtonColor.State.Pressed, false);
        DisplayCommand();
        SetCommandVisibility(true, false);
    }

    private void InitHpMp()
    {
        _hpInfoVal.Clear();
        _mpInfoVal.Clear();
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            DamageAnimationInfo infoVal1 = new DamageAnimationInfo();
            DamageAnimationInfo infoVal2 = new DamageAnimationInfo();
            infoVal1.RequiredValue = infoVal1.CurrentValue = (Int32)unit.CurrentHp;
            infoVal2.RequiredValue = infoVal2.CurrentValue = (Int32)unit.CurrentMp;
            infoVal1.FrameLeft = infoVal2.FrameLeft = 0;
            infoVal1.IncrementStep = infoVal2.IncrementStep = 0;
            _hpInfoVal.Add(infoVal1);
            _mpInfoVal.Add(infoVal2);
        }
    }

    private Int32 GetActionMpCost(AA_DATA aaData, BattleUnit unit)
    {
        Int32 mpCost = aaData.MP;
        CharacterCommandSet command = CharacterCommands.CommandSets[FF9StateSystem.Common.FF9.party.GetCharacter(unit.Position).PresetId];
        if ((aaData.Type & 4) != 0 && FF9StateSystem.EventState.gEventGlobal[18] != 0)
            mpCost <<= 2;

        if ((_currentCommandId == command.Regular1 || _currentCommandId == command.Trance1) && unit.Player.Data.mpCostFactorSkill1 != 100)
            mpCost = mpCost * unit.Player.Data.mpCostFactorSkill1 / 100;
        else if ((_currentCommandId == command.Regular2 || _currentCommandId == command.Trance2) && unit.Player.Data.mpCostFactorSkill2 != 100)
            mpCost = mpCost * unit.Player.Data.mpCostFactorSkill2 / 100;

        mpCost = mpCost * unit.Player.Data.mpCostFactor / 100;

        return mpCost;
    }

    private AbilityStatus GetMonsterTransformAbilityState(Int32 abilId, Int32 playerIndex = -1)
    {
        if (playerIndex < 0)
            playerIndex = CurrentPlayerIndex;
        BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(playerIndex);
        if (!unit.IsMonsterTransform)
            return AbilityStatus.None;
        AA_DATA aaData = unit.Data.monster_transform.spell[abilId];

        if (aaData.AlternateIdleAccess != (unit.Data.bi.def_idle == 1))
            return AbilityStatus.Disable;

        if ((aaData.Category & 2) != 0)
        {
            if (FF9StateSystem.Battle.FF9Battle.btl_scene.Info.NoMagical)
                return AbilityStatus.Disable;

            if (unit.IsUnderAnyStatus(BattleStatus.Silence))
                return AbilityStatus.Disable;
        }

        if (GetActionMpCost(aaData, unit) > unit.CurrentMp)
            return AbilityStatus.Disable;

        return AbilityStatus.Enable;
    }

    private AbilityStatus GetAbilityState(Int32 abilId, Int32 playerIndex = -1)
    {
        if (playerIndex < 0)
            playerIndex = CurrentPlayerIndex;
        AbilityPlayerDetail abilityPlayerDetail = _abilityDetailDict[playerIndex];
        BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(playerIndex);
        BattleAbilityId patchedID = PatchAbility(ff9abil.GetActiveAbilityFromAbilityId(abilId));
        AA_DATA aaData = FF9StateSystem.Battle.FF9Battle.aa_data[patchedID];

        if ((Configuration.Battle.LockEquippedAbilities == 2 || Configuration.Battle.LockEquippedAbilities == 3) && abilityPlayerDetail.Player.Index != CharacterId.Quina && abilityPlayerDetail.HasAp && !abilityPlayerDetail.AbilityEquipList.ContainsKey(abilId) && ff9abil.IsAbilityActive(abilId))
            return AbilityStatus.None;
        if (abilityPlayerDetail.HasAp && !abilityPlayerDetail.AbilityEquipList.ContainsKey(abilId) && ff9abil.IsAbilityActive(abilId))
		{
            if (unit.InTrance && abilityPlayerDetail.AbilityTranceList.ContainsKey(abilId))
            {
                if (!abilityPlayerDetail.AbilityTranceList[abilId])
                    return AbilityStatus.None;
            }
            else if (!abilityPlayerDetail.AbilityPaList.ContainsKey(abilId) || abilityPlayerDetail.AbilityPaList[abilId] < abilityPlayerDetail.AbilityMaxPaList[abilId])
            {
                return AbilityStatus.None;
            }
        }

        if ((aaData.Category & 2) != 0)
        {
            if (FF9StateSystem.Battle.FF9Battle.btl_scene.Info.NoMagical)
                return AbilityStatus.Disable;

            if (unit.IsUnderAnyStatus(BattleStatus.Silence))
                return AbilityStatus.Disable;
        }

        if ((aaData.Type & 16) != 0) // [DV] Unused (5) - To disable "Bandit !" if Zidane equip a Dagger, Mage Masher or Mythril Dagger.
        {
            return AbilityStatus.Disable;
        }

        if (GetActionMpCost(aaData, unit) > unit.CurrentMp)
            return AbilityStatus.Disable;

        return AbilityStatus.Enable;
    }

    private void SetAbilityAp(AbilityPlayerDetail abilityPlayer)
    {
        Character player = abilityPlayer.Player;
        if (!abilityPlayer.HasAp)
            return;

        CharacterAbility[] abilArray = ff9abil._FF9Abil_PaData[player.PresetId];
        for (Int32 i = 0; i < abilArray.Length; i++)
        {
            if (abilArray[i].Id == 0)
                continue;

            abilityPlayer.AbilityPaList[abilArray[i].Id] = player.Data.pa[i];
            abilityPlayer.AbilityMaxPaList[abilArray[i].Id] = abilArray[i].Ap;
        }
    }

    private static void SetAbilityEquip(AbilityPlayerDetail abilityPlayer)
    {
        Character player = abilityPlayer.Player;
        for (Int32 i = 0; i < 5; ++i)
        {
            RegularItem itemId = player.Equipment[i];
            if (itemId == RegularItem.NoItem)
                continue;

            FF9ITEM_DATA itemData = ff9item._FF9Item_Data[itemId];
            foreach (Int32 abilId in itemData.ability)
                if (abilId != 0 && ff9abil.IsAbilityActive(abilId))
                    abilityPlayer.AbilityEquipList[abilId] = true;
        }
    }

    private static void SetAbilityTrance(AbilityPlayerDetail abilityPlayer)
    {
        Character play = abilityPlayer.Player;
        CharacterPresetId presetId = play.PresetId;
        if (!ff9abil.FF9Abil_HasAp(play))
            return;

        for (Int32 k = 0; k < 2; k++)
        {
            BattleCommandId normalCommandId = CharacterCommands.CommandSets[presetId].GetRegular(k);
            BattleCommandId tranceCommandId = CharacterCommands.CommandSets[presetId].GetTrance(k);
            if (normalCommandId == tranceCommandId)
                continue;
            CharacterCommand normalCommand = CharacterCommands.Commands[normalCommandId];
            CharacterCommand tranceCommand = CharacterCommands.Commands[tranceCommandId];
            if (normalCommand.Type != CharacterCommandType.Ability || tranceCommand.Type != CharacterCommandType.Ability)
                continue;

            Int32 count = Math.Min(normalCommand.ListEntry.Length, tranceCommand.ListEntry.Length);
            for (Int32 i = 0; i < count; ++i)
            {
                Int32 normalId = normalCommand.ListEntry[i];
                Int32 tranceId = tranceCommand.ListEntry[i];
                if (normalId == tranceId)
                    continue;

                normalId = ff9abil.GetAbilityIdFromActiveAbility((BattleAbilityId)normalId);
                tranceId = ff9abil.GetAbilityIdFromActiveAbility((BattleAbilityId)tranceId);
                abilityPlayer.AbilityTranceList[tranceId] = abilityPlayer.AbilityEquipList.ContainsKey(normalId) || (abilityPlayer.AbilityPaList.ContainsKey(normalId) && abilityPlayer.AbilityPaList[normalId] >= abilityPlayer.AbilityMaxPaList[normalId]);
            }
        }
    }

    private static void SetAbilityMagic(AbilityPlayerDetail abilityPlayer)
    {
        foreach (BattleMagicSwordSet magicSet in FF9BattleDB.MagicSwordData.Values)
		{
            if (magicSet.Beneficiary != abilityPlayer.Player.Index)
                continue;
            if (!FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits().Any(btl => btl.IsPlayer && btl.PlayerIndex == magicSet.Supporter))
                continue;

            PLAYER supporter = FF9StateSystem.Common.FF9.GetPlayer(magicSet.Supporter);
            CharacterAbility[] paDataArray = ff9abil._FF9Abil_PaData[supporter.PresetId];
            Int32 count = Math.Min(magicSet.BaseAbilities.Length, magicSet.UnlockedAbilities.Length);
            for (Int32 i = 0; i < count; ++i)
            {
                Int32 index = ff9abil.FF9Abil_GetIndex(supporter, magicSet.BaseAbilities[i]);
                if (index >= 0)
                {
                    abilityPlayer.AbilityPaList[magicSet.UnlockedAbilities[i]] = supporter.pa[index];
                    abilityPlayer.AbilityMaxPaList[magicSet.UnlockedAbilities[i]] = paDataArray[index].Ap;
                }
            }
            for (Int32 equipSlot = 0; equipSlot < 5; ++equipSlot)
            {
                RegularItem equipId = supporter.equip[equipSlot];
                if (equipId == RegularItem.NoItem)
                    continue;
                FF9ITEM_DATA ff9ItemData = ff9item._FF9Item_Data[equipId];
                foreach (Int32 equipAbil in ff9ItemData.ability)
                {
                    if (equipAbil == 0)
                        continue;
                    for (Int32 i = 0; i < count; ++i)
                        if (equipAbil == magicSet.BaseAbilities[i])
                            abilityPlayer.AbilityEquipList[magicSet.UnlockedAbilities[i]] = true;
                }
            }
        }
    }

    private void OnTargetNavigate(GameObject go, KeyCode key)
    {
        if (_cursorType == CursorGroup.AllEnemy)
        {
            if ((_targetCursor != TargetType.ManyAny && _targetCursor != TargetType.All && _targetCursor != TargetType.Random) || key != KeyCode.RightArrow)
                return;
            FF9Sfx.FF9SFX_Play(103);
            _cursorType = CursorGroup.AllPlayer;
            DisplayTargetPointer();
        }
        else if (_cursorType == CursorGroup.AllPlayer)
        {
            if ((_targetCursor != TargetType.ManyAny && _targetCursor != TargetType.All && _targetCursor != TargetType.Random) || key != KeyCode.LeftArrow)
                return;
            FF9Sfx.FF9SFX_Play(103);
            _cursorType = CursorGroup.AllEnemy;
            DisplayTargetPointer();
        }
    }

    private void InitialBattle()
    {
        _currentCommandIndex = CommandMenu.Attack;
        _currentSubMenuIndex = 0;
        CurrentPlayerIndex = -1;
        _subMenuType = SubMenuType.Normal;
        _runCounter = 0.0f;
        _isTryingToRun = false;
        _unconsciousStateList.Clear();
        ReadyQueue.Clear();
        InputFinishList.Clear();
        _matchBattleIdPlayerList.Clear();
        _matchBattleIdEnemyList.Clear();
        _itemIdList.Clear();

        foreach (AbilityPlayerDetail value in _abilityDetailDict.Values)
            value.Clear();

        _currentCharacterHp.Clear();
        _enemyCount = 0;
        _playerCount = 0;
        _playerDetailCount = 0;

        _partyDetail.SetBlink(false);

        AutoBattleHud.SetActive(_isAutoAttack);
        Singleton<HUDMessage>.Instance.WorldCamera = PersistenSingleton<UIManager>.Instance.BattleCamera;
        modelButtonManager.WorldCamera = PersistenSingleton<UIManager>.Instance.BattleCamera;
        ManageAbility();
        InitHpMp();

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            Int32 index = unit.GetIndex();

            if (unit.IsTargetable)
            {
                if (unit.IsPlayer)
                    _matchBattleIdPlayerList.Add(index);
                else
                    _matchBattleIdEnemyList.Add(index);
            }
        }

        Int32 orderedIndex = 0;
        foreach (Int32 matchIndex in _matchBattleIdPlayerList)
        {
            if (orderedIndex != matchIndex)
            {
                Debug.LogWarning("This Battle, player index and id not the same. Please be careful.");
                break;
            }
            orderedIndex++;
        }
    }

    private CommandDetail ProcessCommand(Int32 targetIndex, CursorGroup cursor)
    {
        CommandDetail commandDetail = new CommandDetail
        {
            CommandId = _currentCommandId,
            SubId = 0
        };

        BattleCommandId cmdId = commandDetail.CommandId;

        CharacterCommandType commandType = CharacterCommands.Commands[cmdId].Type;
        if (commandType == CharacterCommandType.Normal || commandType == CharacterCommandType.Ability)
            commandDetail.SubId = (Int32)PatchAbility(CharacterCommands.Commands[cmdId].GetAbilityId(_currentSubMenuIndex));
        else if (commandType == CharacterCommandType.Item || commandType == CharacterCommandType.Throw)
            commandDetail.SubId = (Int32)_itemIdList[_currentSubMenuIndex];

        commandDetail.TargetId = 0;

        switch (cursor)
        {
            case CursorGroup.Individual:
                commandDetail.TargetId = (UInt16)(1 << targetIndex);
                break;
            case CursorGroup.AllPlayer:
                commandDetail.TargetId = 0x0F;
                break;
            case CursorGroup.AllEnemy:
                commandDetail.TargetId = 0xF0;
                break;
            case CursorGroup.All:
                commandDetail.TargetId = 0xFF;
                break;
        }

        commandDetail.TargetType = (UInt32)GetSelectMode(cursor);

        SelectViviMagicInsteadOfAttack(targetIndex, commandDetail);

        return commandDetail;
    }

    private void SelectViviMagicInsteadOfAttack(Int32 targetIndex, CommandDetail commandDetail)
    {
        if (!Configuration.Battle.ViviAutoAttack || _currentCommandId != BattleCommandId.Attack)
            return;

        BattleUnit caster = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
        if (caster.PlayerIndex != CharacterId.Vivi)
            return;

        BattleUnit target = FF9StateSystem.Battle.FF9Battle.GetUnit(targetIndex);
        if (target.IsPlayer)
            return;

        CMD_DATA testCommand = new CMD_DATA
        {
            regist = caster.Data,
            cmd_no = BattleCommandId.BlackMagic,
            tar_id = target.Id
        };

        Single bestRating = 0;
        BattleAbilityId bestAbility = 0;

        BattleAbilityId[] abilityIds = { BattleAbilityId.Fire, BattleAbilityId.Blizzard, BattleAbilityId.Thunder };
        abilityIds.Shuffle();

        foreach (BattleAbilityId abilityId in abilityIds)
        {
            if (GetAbilityState(ff9abil.GetAbilityIdFromActiveAbility(abilityId)) != AbilityStatus.Enable)
                continue;

            testCommand.sub_no = (Int32)abilityId;
            testCommand.SetAAData(FF9StateSystem.Battle.FF9Battle.aa_data[abilityId]);
            testCommand.ScriptId = btl_util.GetCommandScriptId(testCommand);

            BattleScriptFactory factory = SBattleCalculator.FindScriptFactory(testCommand.ScriptId);
            if (factory == null)
                continue;

            BattleCalculator v = new BattleCalculator(caster.Data, target.Data, new BattleCommand(testCommand));
            IEstimateBattleScript script = factory(v) as IEstimateBattleScript;
            if (script == null)
                continue;

            Single rating = script.RateTarget();
            if (rating > bestRating)
            {
                bestRating = rating;
                bestAbility = abilityId;
            }
        }

        if (bestRating > 0)
        {
            commandDetail.CommandId = BattleCommandId.BlackMagic;
            commandDetail.SubId = (Int32)bestAbility;
            caster.Data.cmd[0].info.IsZeroMP = true;
        }
    }

    private void SendCommand(CommandDetail command)
    {
        BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex];
        CMD_DATA cmd = btl.cmd[0];
        cmd.regist.sel_mode = 1;
        btl_cmd.SetCommand(cmd, command.CommandId, command.SubId, command.TargetId, command.TargetType);
        SetPartySwapButtonActive(false);
        InputFinishList.Add(CurrentPlayerIndex);

        _partyDetail.SetBlink(CurrentPlayerIndex, false);
    }

    private void SendDoubleCastCommand(CommandDetail first, CommandDetail secondCommand)
    {
        CMD_DATA cmd = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].cmd[3];
        cmd.regist.sel_mode = 1;
        btl_cmd.SetCommand(cmd, first.CommandId, first.SubId, first.TargetId, first.TargetType);
        btl_cmd.SetCommand(FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].cmd[0], secondCommand.CommandId, secondCommand.SubId, secondCommand.TargetId, secondCommand.TargetType);
        SetPartySwapButtonActive(false);
        InputFinishList.Add(CurrentPlayerIndex);

        _partyDetail.SetBlink(CurrentPlayerIndex, false);
    }

    private void SetCommandVisibility(Boolean isVisible, Boolean forceCursorMemo)
    {
        SetPartySwapButtonActive(isVisible);
        BackButton.SetActive(!isVisible && FF9StateSystem.MobilePlatform);

        if (!isVisible)
        {
            ResetSlidingButton();
            TryMemorizeCommand();
            _commandPanel.IsActive = false;
            return;
        }

        GameObject commandObject = _commandPanel.Attack.ButtonGroup.Help.Enable ? _commandPanel.Attack
            : _commandPanel.Skill1.ButtonGroup.Help.Enable ? _commandPanel.Skill1
            : _commandPanel.Skill2.ButtonGroup.Help.Enable ? _commandPanel.Skill2
            : _commandPanel.Item;

        if (_commandPanel.IsActive)
        {
            if (commandObject.GetComponent<ButtonGroupState>().enabled)
                TryGetMemorizedCommandObject(ref commandObject, forceCursorMemo);

            ButtonGroupState.ActiveButton = commandObject;
        }
        else
        {
            _commandPanel.IsActive = true;
            ButtonGroupState.RemoveCursorMemorize(CommandGroupButton);

            if (commandObject.GetComponent<ButtonGroupState>().enabled)
                TryGetMemorizedCommandObject(ref commandObject, forceCursorMemo);

            ButtonGroupState.SetCursorMemorize(commandObject, CommandGroupButton);
        }

        if (_hidingHud)
            _currentButtonGroup = CommandGroupButton;
        else
            ButtonGroupState.ActiveGroup = CommandGroupButton;
    }

    private void TryMemorizeCommand()
    {
        if (Configuration.Interface.PSXBattleMenu && (_currentCommandIndex == CommandMenu.Defend || _currentCommandIndex == CommandMenu.Change))
            return;
        _commandCursorMemorize[CurrentPlayerIndex] = _currentCommandIndex;
    }

    private void TryGetMemorizedCommandObject(ref GameObject commandObject, Boolean forceCursorMemo)
    {
        if (!forceCursorMemo && (Int64)FF9StateSystem.Settings.cfg.cursor == 0L)
            return;

        CommandMenu memorizedCommand;
        if (_commandCursorMemorize.TryGetValue(CurrentPlayerIndex, out memorizedCommand))
            commandObject = _commandPanel.GetCommandButton(memorizedCommand);
    }

    private void SetItemPanelVisibility(Boolean isVisible, Boolean forceCursorMemo)
    {
        if (isVisible)
        {
            ItemPanel.SetActive(true);
            ButtonGroupState.RemoveCursorMemorize(ItemGroupButton);
            PairCharCommand cursorKey = new PairCharCommand(CurrentPlayerIndex, _currentCommandId);
            if (_abilityCursorMemorize.ContainsKey(cursorKey) && FF9StateSystem.Settings.cfg.cursor != 0 || forceCursorMemo)
                _itemScrollList.JumpToIndex(_abilityCursorMemorize[cursorKey], true);
            else
                _itemScrollList.JumpToIndex(0, false);
            ButtonGroupState.RemoveCursorMemorize(ItemGroupButton);
            ButtonGroupState.ActiveGroup = ItemGroupButton;
        }
        else
        {
            if (_currentSubMenuIndex != -1)
                _abilityCursorMemorize[new PairCharCommand(CurrentPlayerIndex, _currentCommandId)] = _currentSubMenuIndex;
            ItemPanel.SetActive(false);
        }
    }

    private void SetAbilityPanelVisibility(Boolean isVisible, Boolean forceCursorMemo)
    {
        if (isVisible)
        {
            if (!AbilityPanel.activeSelf)
            {
                AbilityPanel.SetActive(true);
                Int32 defaultIndex = 0;
                ButtonGroupState.RemoveCursorMemorize(AbilityGroupButton);
                PairCharCommand cursorKey = new PairCharCommand(CurrentPlayerIndex, _currentCommandId);
                if (_abilityCursorMemorize.ContainsKey(cursorKey) && FF9StateSystem.Settings.cfg.cursor != 0 || forceCursorMemo)
                    defaultIndex = _abilityCursorMemorize[cursorKey];
                _abilityScrollList.JumpToIndex(defaultIndex, true);
            }
            if (IsDoubleCast && _doubleCastCount == 1)
                ButtonGroupState.SetPointerNumberToGroup(1, AbilityGroupButton);
            else if (IsDoubleCast && _doubleCastCount == 2)
                ButtonGroupState.SetPointerNumberToGroup(2, AbilityGroupButton);
            else
                ButtonGroupState.SetPointerNumberToGroup(0, AbilityGroupButton);
            ButtonGroupState.ActiveGroup = AbilityGroupButton;
            ButtonGroupState.UpdateActiveButton();
        }
        else
        {
            if (_currentSubMenuIndex != -1)
                _abilityCursorMemorize[new PairCharCommand(CurrentPlayerIndex, _currentCommandId)] = _currentSubMenuIndex;
            AbilityPanel.SetActive(false);
        }
    }

    private void SetTargetVisibility(Boolean isVisible)
    {
        if (isVisible)
        {
            TargetType targetType = 0;
            TargetDisplay subMode = 0;
            _defaultTargetAlly = false;
            _defaultTargetHealingAttack = false;
            _defaultTargetDead = false;
            _targetDead = false;
            _bestTargetIndex = -1;
            if (_currentCommandIndex == CommandMenu.Ability1 || _currentCommandIndex == CommandMenu.Ability2 || CommandIsMonsterTransformCommand(CurrentPlayerIndex, _currentCommandId, out _))
            {
                AA_DATA aaData = GetSelectedActiveAbility(CurrentPlayerIndex, _currentCommandId, _currentSubMenuIndex, out Int32 subNo);
                targetType = aaData.Info.Target;
                _defaultTargetAlly = aaData.Info.DefaultAlly;
                _defaultTargetDead = aaData.Info.DefaultOnDead;
                _targetDead = aaData.Info.ForDead;
                subMode = aaData.Info.DisplayStats;

                CMD_DATA testCommand = new CMD_DATA
                {
                    regist = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex],
                    cmd_no = _currentCommandId,
                    sub_no = subNo
                };
                testCommand.SetAAData(aaData);
                testCommand.ScriptId = btl_util.GetCommandScriptId(testCommand);

                if (_currentCommandId == BattleCommandId.Throw) // [DV] Change TargetType for throwing items (magic scrolls for Trance Seek)
                { // Or i can make it with the DictionaryPatch.txt instead ?
                    ItemAttack weapon = ff9item.GetItemWeapon(_itemIdList[_currentSubMenuIndex]);
                    if (((weapon.Category & WeaponCategory.Throw) != 0) && (weapon.ModelId == 65535 || weapon.ModelId == 0)) 
                    { 
                        switch (weapon.Offset2)
                        {
                            case 1:
                                targetType = TargetType.SingleAlly;
                                break;
                            case 6:
                                targetType = TargetType.All;
                                break;
                            case 7:
                                targetType = TargetType.AllAlly;
                                break;
                            case 8:
                                targetType = TargetType.AllEnemy;
                                break;
                        }
                    }
                }

                SelectBestTarget(targetType, testCommand);
            }
            else if (_currentCommandIndex == CommandMenu.Item)
            {
                RegularItem itemId = _itemIdList[_currentSubMenuIndex];
                ITEM_DATA itemData = ff9item.GetItemEffect(itemId);
                targetType = itemData.info.Target;
                _defaultTargetAlly = itemData.info.DefaultAlly;
                _defaultTargetDead = itemData.info.ForDead;
                _targetDead = itemData.info.ForDead;
                subMode = itemData.info.DisplayStats;

                CMD_DATA testCommand = new CMD_DATA
                {
                    regist = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex],
                    cmd_no = _currentCommandId,
                    sub_no = (Int32)itemId
                };
                testCommand.SetAAData(FF9StateSystem.Battle.FF9Battle.aa_data[BattleAbilityId.Void]);
                testCommand.ScriptId = btl_util.GetCommandScriptId(testCommand);

                SelectBestTarget(targetType, testCommand);
            }
            else if (_currentCommandIndex == CommandMenu.Attack && CurrentPlayerIndex > -1)
            {
                BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
                if (btl.IsHealingRod || btl.HasSupportAbility(SupportAbility1.Healer)) // Todo: should be coded as a SA feature instead of being hard-coded
                    _defaultTargetHealingAttack = true;
            }

            _isAllTarget = false;
            TargetPanel.SetActive(true);
            EnableTargetArea();
            UpdateTargetStates();
            DisplayStatus(subMode);
            SetTargetAvailability(targetType);
            SetTargetDefault();
            SetTargetHelp();
            ButtonGroupState.ActiveGroup = TargetGroupButton;
            _allTargetToggle.Set(_isAllTarget);
            DisplayTargetPointer();
        }
        else
        {
            DisableTargetArea();
            ClearModelPointer();
            ButtonGroupState.SetAllTarget(false);
            _cursorType = CursorGroup.Individual;
            _allTargetToggle.value = false;
            ButtonGroupState.DisableAllGroup(true);
            AllTargetButton.SetActive(false);
            _targetPanel.ActivateButtons(false);
            StatusContainer.SetActive(false);
            _partyDetail.SetActive(true);
            TargetPanel.SetActive(false);
        }
    }

    private void SelectBestTarget(TargetType targetType, CMD_DATA testCommand)
    {
        if (!Configuration.Battle.SelectBestTarget)
            return;
        if ((targetType != TargetType.SingleAny && targetType != TargetType.SingleAlly && targetType != TargetType.SingleEnemy) || CurrentPlayerIndex < 0)
            return;

        Boolean allowAllies = targetType != TargetType.SingleEnemy;
        Boolean allowEnemies = targetType != TargetType.SingleAlly;
        Single bestRating = 0;
        if (testCommand.ScriptId == 9) // Magic attack, not yet supported
            return;

        try
        {
            BattleScriptFactory factory = SBattleCalculator.FindScriptFactory(testCommand.ScriptId);
            if (factory == null)
                return;

            BTL_DATA caster = testCommand.regist;
            foreach (KnownUnit knownTarget in EnumerateKnownUnits())
            {
                BattleUnit target = knownTarget.Unit;
                if (!allowAllies && target.IsPlayer)
                    continue;
                if (!allowEnemies && !target.IsPlayer)
                    continue;
                if (!_targetDead && target.IsUnderAnyStatus(BattleStatus.Death))
                    continue;

                testCommand.tar_id = target.Id;

                BattleCalculator v = new BattleCalculator(caster, target.Data, new BattleCommand(testCommand));
                IEstimateBattleScript script = factory(v) as IEstimateBattleScript;
                if (script == null)
                    break;

                Single rating = script.RateTarget();
                if (rating > bestRating)
                {
                    bestRating = rating;
                    _bestTargetIndex = knownTarget.Index;
                }
            }
        }
        catch (Exception err)
        {
            Log.Error(err);
        }
    }

    private void SetTargetAvailability(TargetType cursor)
    {
        _targetCursor = cursor;
        if (cursor == TargetType.SingleAny)
        {
            _cursorType = CursorGroup.Individual;

            ChangeTargetAvailability(player: true, enemy: true, all: false, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.SingleEnemy)
        {
            _cursorType = CursorGroup.Individual;

            ChangeTargetAvailability(player: false, enemy: true, all: false, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.SingleAlly)
        {
            _cursorType = CursorGroup.Individual;

            ChangeTargetAvailability(player: true, enemy: false, all: false, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.ManyAny)
        {
            ChangeTargetAvailability(player: true, enemy: true, all: FF9StateSystem.MobilePlatform, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.ManyEnemy)
        {
            ChangeTargetAvailability(player: false, enemy: true, all: FF9StateSystem.MobilePlatform, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.ManyAlly)
        {
            ChangeTargetAvailability(player: true, enemy: false, all: FF9StateSystem.MobilePlatform, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.AllEnemy || cursor == TargetType.RandomEnemy)
        {
            _cursorType = CursorGroup.AllEnemy;

            ChangeTargetAvailability(player: false, enemy: true, all: false, allPlayers: false, allEnemies: true);

            _isAllTarget = true;
        }
        else if (cursor == TargetType.AllAlly || cursor == TargetType.RandomAlly)
        {
            _cursorType = CursorGroup.AllPlayer;

            ChangeTargetAvailability(player: true, enemy: false, all: false, allPlayers: true, allEnemies: false);

            _isAllTarget = true;
        }
        else if (cursor == TargetType.All || cursor == TargetType.Everyone || cursor == TargetType.Random)
        {
            if (cursor == TargetType.All || cursor == TargetType.Random)
            {
                if (_defaultTargetAlly)
                    _cursorType = CursorGroup.AllPlayer;
                else
                    _cursorType = CursorGroup.AllEnemy;
            }
            else
            {
                _cursorType = CursorGroup.All;
            }

            ChangeTargetAvailability(player: true, enemy: true, all: false, allPlayers: true, allEnemies: true);

            _isAllTarget = true;
        }
        else if (cursor == TargetType.Self)
        {
            _cursorType = CursorGroup.Individual;

            ChangeTargetAvailability(player: false, enemy: false, all: false, allPlayers: false, allEnemies: false);

            GONavigationButton currentPlayer = _targetPanel.Players[CurrentBattlePlayerIndex];
            ButtonGroupState.SetButtonEnable(currentPlayer, true);
        }
    }

    private void ChangeTargetAvailability(Boolean player, Boolean enemy, Boolean all, Boolean allPlayers, Boolean allEnemies)
    {
        foreach (GONavigationButton button in _targetPanel.Players.Entries)
            ButtonGroupState.SetButtonEnable(button, player);

        foreach (GONavigationButton button in _targetPanel.Enemies.Entries)
            ButtonGroupState.SetButtonEnable(button, enemy);

        AllTargetButton.SetActive(all);
        _targetPanel.Buttons.Player.IsActive = allPlayers;
        _targetPanel.Buttons.Enemy.IsActive = allEnemies;

        if (!_targetDead)
        {
            Int32 playerIndex = 0;
            Int32 enemyIndex = 0;
            foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
            {
                if (unit.Id == 0 || !unit.IsTargetable)
                    continue;

                if (unit.IsPlayer)
                {
                    if (_currentCharacterHp[playerIndex] == ParameterStatus.Dead)
                        ButtonGroupState.SetButtonEnable(_targetPanel.Players[playerIndex], false);
                    playerIndex++;
                }
                else
                {
                    if (_currentEnemyDieState[enemyIndex])
                        ButtonGroupState.SetButtonEnable(_targetPanel.Enemies[enemyIndex], false);
                    enemyIndex++;
                }
            }
        }
    }

    private void SetTargetDefault()
    {
        if (_targetCursor == TargetType.SingleAny
            || _targetCursor == TargetType.SingleAlly
            || _targetCursor == TargetType.SingleEnemy
            || _targetCursor == TargetType.ManyAny
            || _targetCursor == TargetType.ManyAlly
            || _targetCursor == TargetType.ManyEnemy)
        {
            if (_bestTargetIndex > -1)
            {
                GONavigationButton target = _targetPanel.AllTargets[_bestTargetIndex];
                ButtonGroupState.SetCursorStartSelect(target, TargetGroupButton);
                _currentTargetIndex = _bestTargetIndex;
            }
            else if (_defaultTargetHealingAttack)
            {
                Int32 targetIndex = GetFirstAliveZombieEnemyIndex();
                if (targetIndex > -1)
                {
                    targetIndex += HonoluluBattleMain.EnemyStartIndex;
                    if (_currentCommandIndex == CommandMenu.Attack && FF9StateSystem.PCPlatform)
                        ValidateDefaultTarget(ref targetIndex);
                    GONavigationButton targetEnemy = _targetPanel.AllTargets[targetIndex];
                    ButtonGroupState.SetCursorStartSelect(targetEnemy, TargetGroupButton);
                }
                else
                {
                    targetIndex = GetAlivePlayerIndexForHealingAttack();
                    GONavigationButton ally = _targetPanel.Players[targetIndex];
                    ButtonGroupState.SetCursorStartSelect(ally, TargetGroupButton);
                }
                _currentTargetIndex = targetIndex;
            }
            else if (_defaultTargetAlly)
            {
                if (_defaultTargetDead)
                    _currentTargetIndex = GetDeadOrCurrentPlayer(true);
                else
                    _currentTargetIndex = CurrentBattlePlayerIndex;
                GONavigationButton currentPlayer = _targetPanel.Players[_currentTargetIndex];
                ButtonGroupState.SetCursorStartSelect(currentPlayer, TargetGroupButton);
            }
            else
            {
                Int32 targetIndex;
                if (_defaultTargetDead)
                {
                    targetIndex = GetDeadOrCurrentPlayer(false);
                    GONavigationButton deadPlayer = _targetPanel.AllTargets[targetIndex];
                    ButtonGroupState.SetCursorStartSelect(deadPlayer, TargetGroupButton);
                }
                else
                {
                    targetIndex = GetFirstAliveEnemyIndex();
                    if (targetIndex != -1)
                    {
                        if (_currentCommandIndex == CommandMenu.Attack && FF9StateSystem.PCPlatform)
                            ValidateDefaultTarget(ref targetIndex);
                        GONavigationButton currentEnemy = _targetPanel.AllTargets[targetIndex];
                        ButtonGroupState.SetCursorStartSelect(currentEnemy, TargetGroupButton);
                    }
                }
                _currentTargetIndex = targetIndex;
            }
        }
        else if (_targetCursor == TargetType.Self)
        {
            Int32 currentPlayerIndex = CurrentBattlePlayerIndex;
            GONavigationButton currentPlayer = _targetPanel.Players[currentPlayerIndex];
            ButtonGroupState.SetCursorStartSelect(currentPlayer, TargetGroupButton);
            _currentTargetIndex = currentPlayerIndex;
        }

        ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
    }

    private void SetTargetHelp()
    {
        String cursorHelp = String.Empty;
        Boolean displayName = _targetCursor <= TargetType.ManyEnemy || _targetCursor == TargetType.Self;
        switch (_targetCursor)
        {
            case TargetType.SingleAny:
                cursorHelp = Localization.Get("BattleTargetHelpIndividual");
                break;
            case TargetType.SingleAlly:
                cursorHelp = Localization.Get("BattleTargetHelpIndividualPC");
                break;
            case TargetType.SingleEnemy:
                cursorHelp = Localization.Get("BattleTargetHelpIndividualNPC");
                break;
            case TargetType.ManyAny:
                cursorHelp = Localization.Get("BattleTargetHelpMultiS");
                break;
            case TargetType.ManyAlly:
                cursorHelp = Localization.Get("BattleTargetHelpMultiPCS");
                break;
            case TargetType.ManyEnemy:
                cursorHelp = Localization.Get("BattleTargetHelpMultiNPCS");
                break;
            case TargetType.All:
                cursorHelp = Localization.Get("BattleTargetHelpAll");
                break;
            case TargetType.AllAlly:
                cursorHelp = Localization.Get("BattleTargetHelpAllPC");
                break;
            case TargetType.AllEnemy:
                cursorHelp = Localization.Get("BattleTargetHelpAllNPC");
                break;
            case TargetType.Random:
                cursorHelp = Localization.Get("BattleTargetHelpRand");
                break;
            case TargetType.RandomAlly:
                cursorHelp = Localization.Get("BattleTargetHelpRandPC");
                break;
            case TargetType.RandomEnemy:
                cursorHelp = Localization.Get("BattleTargetHelpRandNPC");
                break;
            case TargetType.Everyone:
                cursorHelp = Localization.Get("BattleTargetHelpWhole");
                break;
            case TargetType.Self:
                cursorHelp = Localization.Get("BattleTargetHelpSelf");
                break;
        }

        if (_isAllTarget)
        {
            displayName = false;
            switch (_targetCursor)
            {
                case TargetType.ManyAny:
                    cursorHelp = Localization.Get("BattleTargetHelpMultiM");
                    break;
                case TargetType.ManyAlly:
                    cursorHelp = Localization.Get("BattleTargetHelpMultiPCM");
                    break;
                case TargetType.ManyEnemy:
                    cursorHelp = Localization.Get("BattleTargetHelpMultiNPCM");
                    break;
            }
        }

        Int32 playerIndex = 0;
        Int32 enemyIndex = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (unit.Id == 0 || !unit.IsTargetable)
                continue;

            if (unit.IsPlayer)
            {
                GONavigationButton targetHud = _targetPanel.Players[playerIndex];
                String targetName = displayName ? unit.Player.Name : String.Empty;
                targetHud.ButtonGroup.Help.Enable = true;
                targetHud.ButtonGroup.Help.Text = cursorHelp + "\n" + targetName;
                ++playerIndex;
            }
            else
            {
                GONavigationButton targetHud = _targetPanel.Enemies[enemyIndex];
                Single additionalWidth = 0.0f;
                String targetName = displayName ? Singleton<HelpDialog>.Instance.PhraseLabel.PhrasePreOpcodeSymbol(unit.Enemy.Name, ref additionalWidth) : String.Empty;
                targetHud.ButtonGroup.Help.Enable = true;
                targetHud.ButtonGroup.Help.Text = cursorHelp + "\n" + targetName;
                ++enemyIndex;
            }
        }
    }

    private void SetHelpMessageVisibility(Boolean active)
    {
        if (ButtonGroupState.HelpEnabled)
            Singleton<HelpDialog>.Instance.gameObject.SetActive(active && (CommandPanel.activeSelf || ItemPanel.activeSelf || AbilityPanel.activeSelf || TargetPanel.activeSelf));
    }

    private void SetHudVisibility(Boolean active)
    {
        if (_hidingHud != active)
            return;

        _hidingHud = !active;
        AllMenuPanel.SetActive(active);

        SetHelpMessageVisibility(active);

        if (!active)
        {
            _currentButtonGroup = ButtonGroupState.ActiveGroup;
            ButtonGroupState.DisableAllGroup(false);
            ButtonGroupState.SetPointerVisibilityToGroup(false, _currentButtonGroup);
        }
        else
        {
            if (_currentButtonGroup == CommandGroupButton && !CommandPanel.activeSelf)
                _currentButtonGroup = String.Empty;

            _isTryingToRun = false;
            ButtonGroupState.ActiveGroup = _currentButtonGroup;
            DisplayTargetPointer();
        }
    }

    private void ProcessAutoBattleInput()
    {
        _isAutoAttack = !_isAutoAttack;
        _autoBattleToggle.value = _isAutoAttack;
        AutoBattleHud.SetActive(_isAutoAttack);
        _autoBattleButtonComponent.SetState(UIButtonColor.State.Normal, false);
        if (_isAutoAttack)
        {
            SetIdle();
            SetPartySwapButtonActive(false);
        }
        else
        {
            SetPartySwapButtonActive(true);
            foreach (UI.PanelParty.Character character in _partyDetail.Characters.Entries)
                character.ATBBlink = ReadyQueue.Contains(character.PlayerId) && !InputFinishList.Contains(character.PlayerId);
        }
    }


    private BattleAbilityId PatchAbility(BattleAbilityId id)
    {
        BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
        return BattleAbilityHelper.Patch(id, unit.Player.Data);
    }

    private UInt16 GetDeadOrCurrentPlayer(Boolean player)
    {
        UInt16 index = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsTargetable || !unit.IsUnderStatus(BattleStatus.Death))
                continue;

            if (player && unit.IsPlayer || !player && !unit.IsPlayer)
                return index;

            index++;
        }
        return (UInt16)CurrentBattlePlayerIndex;
    }


    private static Boolean IsEnableInput(BattleUnit unit)
    {
        return unit != null && unit.CurrentHp != 0 && !unit.IsUnderAnyStatus(BattleStatusConst.NoInput) && (battle.btl_bonus.member_flag & 1 << unit.Position) != 0;
    }

    private Int32 GetSelectMode(CursorGroup cursor)
    {
        if (_targetCursor == TargetType.Random || _targetCursor == TargetType.RandomAlly || _targetCursor == TargetType.RandomEnemy)
            return 2;
        return cursor == CursorGroup.Individual ? 0 : 1;
    }

    private void EnableTargetArea()
    {
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsTargetable)
                continue;

            Int32 index = unit.GetIndex();

            modelButtonManager.Show(
                unit.Data.gameObject.transform.GetChildByName("bone" + unit.Data.tar_bone.ToString("D3")).gameObject.transform,
                index,
                !unit.IsPlayer,
                unit.Data.radius_effect,
                unit.Data.height);
        }
    }

    private void DisableTargetArea()
    {
        modelButtonManager.Reset();
        _targetIndexList.Clear();
    }

    private void ClearModelPointer()
    {
        using (List<Int32>.Enumerator enumerator = _targetIndexList.GetEnumerator())
        {
            while (enumerator.MoveNext())
                Singleton<PointerManager>.Instance.RemovePointerFromGameObject(modelButtonManager.GetGameObject(enumerator.Current));
        }
        _targetIndexList.Clear();
    }

    private void PointToModel(CursorGroup selectType, Int32 targetIndex = 0)
    {
        ClearModelPointer();
        Boolean isBlink = false;
        switch (selectType)
        {
            case CursorGroup.Individual:
                if (targetIndex < HonoluluBattleMain.EnemyStartIndex)
                {
                    if (targetIndex < _matchBattleIdPlayerList.Count)
                        _targetIndexList.Add(_matchBattleIdPlayerList[targetIndex]);
                }
                else if (targetIndex - HonoluluBattleMain.EnemyStartIndex < _matchBattleIdEnemyList.Count)
                {
                    _targetIndexList.Add(_matchBattleIdEnemyList[targetIndex - HonoluluBattleMain.EnemyStartIndex]);
                }
                break;
            case CursorGroup.AllPlayer:
                _targetIndexList = modelButtonManager.GetAllPlayerIndex();
                isBlink = true;
                break;
            case CursorGroup.AllEnemy:
                _targetIndexList = modelButtonManager.GetAllEnemyIndex();
                isBlink = true;
                break;
            case CursorGroup.All:
                _targetIndexList = modelButtonManager.GetAllIndex();
                isBlink = true;
                break;
        }

        foreach (Int32 index in _targetIndexList)
        {
            GameObject obj = modelButtonManager.GetGameObject(index);
            Singleton<PointerManager>.Instance.PointerDepth = 0;
            Singleton<PointerManager>.Instance.AttachPointerToGameObject(obj, true);
            Singleton<PointerManager>.Instance.SetPointerBlinkAt(obj, isBlink);
            Singleton<PointerManager>.Instance.SetPointerLimitRectBehavior(obj, PointerManager.LimitRectBehavior.Hide);
            Singleton<PointerManager>.Instance.PointerDepth = 5;
        }
    }

    private void DisplayTargetPointer()
    {
        if (ButtonGroupState.ActiveGroup != TargetGroupButton)
            return;

        if (_cursorType == CursorGroup.Individual)
        {
            PointToModel(_cursorType, _currentTargetIndex);
            ButtonGroupState.SetAllTarget(false);
        }
        else
        {
            PointToModel(_cursorType, 0);
            foreach (GONavigationButton target in _targetPanel.AllTargets)
                Singleton<PointerManager>.Instance.SetPointerVisibility(target, false);

            if (_cursorType == CursorGroup.AllPlayer)
            {
                List<GameObject> targetList = new List<GameObject>();
                for (Int32 playerIndex = 0; playerIndex < _playerCount; ++playerIndex)
                    if (_targetDead || _currentCharacterHp[playerIndex] != ParameterStatus.Dead)
                        targetList.Add(_targetPanel.Players[playerIndex]);
                ButtonGroupState.SetMultipleTarget(targetList, true);
            }
            else if (_cursorType == CursorGroup.AllEnemy)
            {
                List<GameObject> targetList = new List<GameObject>();
                for (Int32 enemyIndex = 0; enemyIndex < _enemyCount; ++enemyIndex)
                    if (_targetDead || enemyIndex < _currentEnemyDieState.Count && !_currentEnemyDieState[enemyIndex])
                        targetList.Add(_targetPanel.Enemies[enemyIndex]);
                ButtonGroupState.SetMultipleTarget(targetList, true);
            }
            else
            {
                ButtonGroupState.SetAllTarget(true);
            }
        }
    }


    private void SetTarget(Int32 battleIndex)
    {
        // This could be moved to AbilityFeatures.txt but whatever...
        if (_currentCommandId == BattleCommandId.Attack && CurrentBattlePlayerIndex > -1)
        {
            BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
            if (btl.PlayerIndex == CharacterId.Beatrix)
            {
                Character player = btl.Player;
                if (player.Equipment.Weapon == RegularItem.SaveTheQueen && player.Equipment.Accessory == RegularItem.SaveTheQueen)
                {
                    CommandDetail first = ProcessCommand(battleIndex, _cursorType);
                    SendDoubleCastCommand(first, first);

                    SetTargetVisibility(false);
                    SetIdle();
                    return;
                }
            }
        }

        if (IsDoubleCast)
            SendDoubleCastCommand(_firstCommand, ProcessCommand(battleIndex, _cursorType));
        else
            SendCommand(ProcessCommand(battleIndex, _cursorType));

        SetTargetVisibility(false);
        SetIdle();
    }

    private void ValidateDefaultTarget(ref Int32 firstIndex)
    {
        for (Int32 index = firstIndex; index < _targetPanel.AllTargets.Length; ++index)
        {
            GONavigationButton targetHud = _targetPanel.AllTargets[index];
            if (targetHud.IsActive && targetHud.Name.Label.color != FF9TextTool.Gray)
            {
                firstIndex = index;
                break;
            }
        }
    }

    private void DisplayItemDetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        BattleItemListData battleItemListData = (BattleItemListData)data;
        ItemListDetailWithIconHUD detailWithIconHud = new ItemListDetailWithIconHUD(item.gameObject, true);
        if (isInit)
            DisplayWindowBackground(item.gameObject, null);

        if (battleItemListData.Id == RegularItem.NoItem)
        {
            detailWithIconHud.IconSprite.alpha = 0.0f;
            detailWithIconHud.NameLabel.text = String.Empty;
            detailWithIconHud.NumberLabel.text = String.Empty;
            detailWithIconHud.Button.Help.Enable = false;
            detailWithIconHud.Button.Help.TextKey = String.Empty;
            detailWithIconHud.Button.Help.Text = String.Empty;
        }
        else
        {
            FF9UIDataTool.DisplayItem(battleItemListData.Id, detailWithIconHud.IconSprite, detailWithIconHud.NameLabel, true);
            detailWithIconHud.NumberLabel.text = battleItemListData.Count.ToString();
            detailWithIconHud.Button.Help.Enable = true;
            detailWithIconHud.Button.Help.TextKey = String.Empty;
            detailWithIconHud.Button.Help.Text = FF9TextTool.ItemBattleDescription(battleItemListData.Id);
        }
    }

    private void UpdatePlayersForMainMenu()
    {
        Dictionary<PLAYER, BattleStatus> statusLockDict = new Dictionary<PLAYER, BattleStatus>();
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;
            PLAYER player = unit.Player.Data;
            BattleStatus battlePermanent = unit.PermanentStatus & BattleStatusConst.OutOfBattle & ~player.permanent_status;
            statusLockDict[player] = battlePermanent;
            player.permanent_status |= battlePermanent;
            player.trance = unit.Trance;
            btl_init.CopyPoints(player.cur, unit.Data.cur);
            btl_stat.SaveStatus(player, unit.Data);
        }
        _mainMenuPlayerMemo.Clear();
        for (Int32 i = 0; i < 4; i++)
        {
            _mainMenuPlayerMemo.Add(new PlayerMemo(FF9StateSystem.Common.FF9.party.member[i], true));
            if (FF9StateSystem.Common.FF9.party.member[i] != null)
                statusLockDict.TryGetValue(FF9StateSystem.Common.FF9.party.member[i], out _mainMenuPlayerMemo[i].battlePermanentStatus);
        }
        if (_mainMenuSinglePlayer != null)
            for (Int32 i = 0; i < 4; i++)
                if (_mainMenuSinglePlayer != FF9StateSystem.Common.FF9.party.member[i])
                    FF9StateSystem.Common.FF9.party.member[i] = null;
    }

    private void UpdateBattleAfterMainMenu()
    {
        Boolean menuHadImpact = PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount > 0;
        if (menuHadImpact)
        {
            foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
            {
                if (!unit.IsPlayer)
                    continue;
                if (_mainMenuSinglePlayer != null && unit.Player.Data != _mainMenuSinglePlayer)
                    continue;
                Character playerPtr = unit.Player;
                BTL_DATA btl = unit.Data;
                PLAYER player = playerPtr.Data;
                PlayerMemo beforeMenu = _mainMenuPlayerMemo.Find(memo => memo.original == player);
                unit.Trance = player.trance;
                btl_init.CopyPoints(btl.cur, player.cur);
                player.permanent_status &= ~beforeMenu.battlePermanentStatus;
                BattleStatus statusesToRemove = unit.CurrentStatus & BattleStatusConst.OutOfBattle & ~player.status;
                btl_stat.RemoveStatuses(btl, statusesToRemove);
                if ((unit.CurrentStatus & BattleStatus.Death) != 0 && player.cur.hp > 0)
                    btl_stat.RemoveStatus(btl, BattleStatus.Death);

                BattleStatus oldPermanent = 0, oldResist = 0, newPermanent = 0, newResist = 0;
                foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(beforeMenu.saExtended))
                {
                    saFeature.GetStatusInitQuietly(unit, out BattleStatus permanent, out BattleStatus initial, out BattleStatus resist, out StatusModifier partial, out StatusModifier duration, out Int16 atb);
                    oldPermanent |= permanent;
                    oldResist |= resist;
                }
                foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(player))
                {
                    saFeature.GetStatusInitQuietly(unit, out BattleStatus permanent, out BattleStatus initial, out BattleStatus resist, out StatusModifier partial, out StatusModifier duration, out Int16 atb);
                    newPermanent |= permanent;
                    newResist |= resist;
                }
                btl.sa = player.sa;
                btl.saExtended = player.saExtended;
                btl_stat.MakeStatusesPermanent(btl, oldPermanent & ~newPermanent, false);
                unit.ResistStatus &= ~(oldResist & ~newResist);
                btl_stat.MakeStatusesPermanent(btl, newPermanent & ~oldPermanent, true);
                unit.ResistStatus |= newResist & ~oldResist;

                btl.max.hp = Math.Max(1, btl.max.hp + player.max.hp - beforeMenu.max.hp);
                btl.max.mp = Math.Max(0, btl.max.mp + player.max.mp - beforeMenu.max.mp);
                btl.elem.dex = (Byte)Mathf.Clamp(player.elem.dex + player.elem.dex - beforeMenu.elem.dex, 0, Byte.MaxValue);
                btl.elem.str = (Byte)Mathf.Clamp(player.elem.str + player.elem.str - beforeMenu.elem.str, 0, Byte.MaxValue);
                btl.elem.mgc = (Byte)Mathf.Clamp(player.elem.mgc + player.elem.mgc - beforeMenu.elem.mgc, 0, Byte.MaxValue);
                btl.elem.wpr = (Byte)Mathf.Clamp(player.elem.wpr + player.elem.wpr - beforeMenu.elem.wpr, 0, Byte.MaxValue);
                btl.defence.PhysicalDefence = Math.Max(0, btl.defence.PhysicalDefence + player.defence.PhysicalDefence - beforeMenu.defence.PhysicalDefence);
                btl.defence.PhysicalEvade = Math.Max(0, btl.defence.PhysicalEvade + player.defence.PhysicalEvade - beforeMenu.defence.PhysicalEvade);
                btl.defence.MagicalDefence = Math.Max(0, btl.defence.MagicalDefence + player.defence.MagicalDefence - beforeMenu.defence.MagicalDefence);
                btl.defence.MagicalEvade = Math.Max(0, btl.defence.MagicalEvade + player.defence.MagicalEvade - beforeMenu.defence.MagicalEvade);
                if (beforeMenu.battleRow != player.info.row)
                    btl_para.SwitchPlayerRow(btl, !btl_util.IsBtlBusy(btl, btl_util.BusyMode.CASTER));

                // Todo: Serial number changed
                //BattlePlayerCharacter.CreatePlayer(btl, player.info.serial_no);
                //btl_mot.SetPlayerDefMotion(btl, player.info.serial_no, (UInt32)unit.GetIndex());
                //BattlePlayerCharacter.InitAnimation(btl);
                if (btl.weapon_geo != null && btl.weapon != ff9item.GetItemWeapon(player.equip[0]))
                {
                    UnityEngine.Object.Destroy(btl.weapon_geo);
                    btl_eqp.InitWeapon(player, btl);
                }
                btl_eqp.InitEquipPrivilegeAttrib(player, btl);
            }
            foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
            {
                if (!unit.IsPlayer)
                    continue;
                AbilityPlayerDetail abilityPlayer = _abilityDetailDict[unit.GetIndex()];
                abilityPlayer.AbilityEquipList.Clear();
                abilityPlayer.AbilityTranceList.Clear();
                SetAbilityEquip(abilityPlayer);
                SetAbilityTrance(abilityPlayer);
                SetAbilityMagic(abilityPlayer);
            }
        }
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;
            PLAYER player = unit.Player.Data;
            PlayerMemo beforeMenu = _mainMenuPlayerMemo.Find(memo => memo.original == player);
            player.info.row = beforeMenu.row;
        }
        if (_mainMenuSinglePlayer != null)
        {
            for (Int32 i = 0; i < 4; i++)
                FF9StateSystem.Common.FF9.party.member[i] = _mainMenuPlayerMemo[i].original;
            if (Configuration.Battle.AccessMenus <= 1 && menuHadImpact)
            {
                BTL_DATA playerBtl = btl_util.getBattlePtr(_mainMenuSinglePlayer);
                if (playerBtl != null)
                {
                    playerBtl.cur.at = 0;
                    RemovePlayerFromAction(playerBtl.btl_id, true);
                    if (1 << CurrentPlayerIndex == playerBtl.btl_id)
                        SwitchAvailablePlayerOrIdle();
                }
            }
        }
        if (AbilityPanel.activeSelf)
            DisplayAbility();
        if (ItemPanel.activeSelf)
            DisplayItem(_subMenuType == SubMenuType.Throw);
    }
}