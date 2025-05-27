using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Database;
using Memoria.Prime;
using Memoria.Scenes;
using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleHUD : UIScene
{
    private readonly Dictionary<Int32, AbilityPlayerDetail> _abilityDetailDict;
    private readonly List<ParameterStatus> _currentCharacterHp;
    private readonly List<Boolean> _currentEnemyDieState;
    private readonly List<DamageAnimationInfo> _hpInfoVal;
    private readonly List<DamageAnimationInfo> _mpInfoVal;
    private readonly Dictionary<Int32, BattleCommandMenu> _commandCursorMemorize;
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
    private Dictionary<BattleCommandMenu, Int32> _commandEnabledState;
    private Boolean _needItemUpdate;
    private Boolean _currentSilenceStatus;
    private Boolean _currentMagicSwordState;
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
    private BattleCommandMenu _currentCommandIndex;
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
    private BattleCommandMenu _buttonSlideInitial;
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
        }

        _battleDialogLabel.rawText = str;
        BattleDialogGameObject.SetActive(true);
    }

    private List<String> GetLibraMessages(BattleUnit unit, LibraInformation info)
    {
        List<String> messages = new List<String>();
        switch (info)
        {
            case LibraInformation.Name:
                return [unit.NameTag];
            case LibraInformation.Level:
                return [FF9TextTool.BattleLibraText(10) + unit.Level.ToString()];
            case LibraInformation.HP:
                return [FF9TextTool.BattleLibraText(11) + unit.CurrentHp + FF9TextTool.BattleLibraText(13) + unit.MaximumHp];
            case LibraInformation.MP:
                return [FF9TextTool.BattleLibraText(12) + unit.CurrentMp + FF9TextTool.BattleLibraText(13) + unit.MaximumMp];
            case LibraInformation.Category:
                if (!unit.IsPlayer)
                {
                    Int32 enemyCategory = unit.Enemy.Data.et.category;
                    for (Int32 i = 0; i < 8; i++)
                        if ((enemyCategory & (1 << i)) != 0)
                            messages.Add(FF9TextTool.BattleLibraText(i));
                }
                return messages;
            case LibraInformation.ElementWeak:
            case LibraInformation.ElementResist:
            case LibraInformation.ElementImmune:
            case LibraInformation.ElementAbsorb:
            {
                EffectElement element = info == LibraInformation.ElementWeak ? unit.WeakElement & ~unit.GuardElement :
                                        info == LibraInformation.ElementResist ? unit.HalfElement & ~unit.GuardElement :
                                        info == LibraInformation.ElementImmune ? unit.GuardElement :
                                        unit.AbsorbElement & ~unit.GuardElement;
                for (Int32 i = 0; i < 8; i++)
                    if ((element & (EffectElement)(1 << i)) != 0)
                        messages.Add(Localization.GetWithDefault(info.ToString()).Replace("%", BtlGetAttrName(1 << i)));
                return messages;
            }
            case LibraInformation.StatusAuto:
            case LibraInformation.StatusImmune:
            {
                BattleStatus status = info == LibraInformation.StatusAuto ? unit.PermanentStatus : unit.ResistStatus;
                Dictionary<BattleStatusId, String> icons = info == LibraInformation.StatusAuto ? BattleHUD.BuffIconNames : BattleHUD.DebuffIconNames;
                foreach (BattleStatusId statusId in status.ToStatusList())
                    if (icons.TryGetValue(statusId, out String spriteName))
                        messages.Add($"[SPRT={spriteName},48,48]");
                if (messages.Count == 0)
                    return [];
                if (messages.Count <= 10)
                    return [Localization.GetWithDefault(info.ToString()).Replace("%", String.Join("  ", messages.ToArray())) + " "];
                List<String> result = new List<String>();
                result.Add(Localization.GetWithDefault(info.ToString()).Replace("%", String.Join("  ", messages.Take(10).ToArray())) + " ");
                messages.RemoveRange(0, 10);
                while (messages.Count > 0)
                {
                    result.Add("-" + String.Join("  ", messages.Take(10).ToArray()) + " -");
                    messages.RemoveRange(0, Math.Min(10, messages.Count));
                }
                return result;
            }
            case LibraInformation.StatusResist:
            {
                String spriteName;
                foreach (KeyValuePair<BattleStatusId, Single> resist in unit.PartialResistStatus)
                    if (resist.Value > 0f && (BattleHUD.BuffIconNames.TryGetValue(resist.Key, out spriteName) || BattleHUD.DebuffIconNames.TryGetValue(resist.Key, out spriteName)))
                        messages.Add($"[SPRT={spriteName},48,48]  ({(Int32)Math.Min(100, resist.Value * 100)}%)");
                if (messages.Count == 0)
                    return [];
                if (messages.Count <= 10)
                    return [Localization.GetWithDefault(info.ToString()).Replace("%", String.Join(", ", messages.ToArray())) + " "];
                List<String> result = new List<String>();
                result.Add(Localization.GetWithDefault(info.ToString()).Replace("%", String.Join("  ", messages.Take(10).ToArray())) + " ");
                messages.RemoveRange(0, 10);
                while (messages.Count > 0)
                {
                    result.Add("-" + String.Join("  ", messages.Take(10).ToArray()) + " -");
                    messages.RemoveRange(0, Math.Min(10, messages.Count));
                }
                return result;
            }
            case LibraInformation.ItemSteal:
                if (!unit.IsPlayer)
                {
                    BattleEnemy enemy = unit.Enemy;
                    foreach (RegularItem itemId in enemy.StealableItems)
                        if (itemId != RegularItem.NoItem)
                            messages.Add(Localization.CurrentDisplaySymbol != "JP" ? FF9TextTool.BattleLibraText(8) + "[FFCC00]" + FF9TextTool.ItemName(itemId) + "[FFFFFF]" : "[FFCC00]" + FF9TextTool.ItemName(itemId) + "[FFFFFF]" + FF9TextTool.BattleLibraText(8));
                }
                return messages;
            case LibraInformation.BlueLearn:
                if (!unit.IsPlayer)
                {
                    BattleEnemyPrototype enemyPrototype = BattleEnemyPrototype.Find(unit);
                    Int32 blueMagicId = enemyPrototype.BlueMagicId;
                    if (blueMagicId == 0)
                        return [FF9TextTool.CommandName(BattleCommandId.BlueMagic) + ": [FFCC00]-[FFFFFF]"];
                    String str = FF9TextTool.CommandName(BattleCommandId.BlueMagic) + ": [FFCC00]";
                    if (ff9abil.IsAbilityActive(blueMagicId))
                        str += FF9TextTool.ActionAbilityName(ff9abil.GetActiveAbilityFromAbilityId(blueMagicId));
                    else if (ff9abil.IsAbilitySupport(blueMagicId))
                        str += FF9TextTool.SupportAbilityName(ff9abil.GetSupportAbilityFromAbilityId(blueMagicId));
                    str += "[FFFFFF]";
                    return [str];
                }
                return messages;
            case LibraInformation.AttackList:
                if (!unit.IsPlayer)
                {
                    Int32 atkCount = 0;
                    messages.Add(Localization.GetWithDefault(info.ToString()));
                    foreach (Int32 atkIndex in BTL_SCENE.EnemyGetAttackList(unit.Data.bi.slot_no))
                    {
                        String atkTitle = btlseq.instance.GetAttackTitleOfSequence(unit, atkIndex);
                        if (!String.IsNullOrEmpty(atkTitle))
                        {
                            if ((atkCount % 3) == 0)
                                messages.Add(atkTitle);
                            else
                                messages[messages.Count - 1] += ", " + atkTitle;
                            atkCount++;
                        }
                    }
                    if (messages.Count <= 1)
                        return [];
                    return messages;
                }
                return messages;
        }
        return messages;
    }

    private void AdvanceLibraMessageNumber()
    {
        _currentLibraMessageCount = 0;
        _currentLibraMessageNumber++;
        while (_currentLibraMessageNumber < 13)
        {
            if (_currentLibraMessageNumber == 1 && (_libraEnabledMessage & LibraInformation.NameLevel) != 0)
                return;
            if (_currentLibraMessageNumber == 2 && (_libraEnabledMessage & LibraInformation.HPMP) != 0)
                return;
            if (_currentLibraMessageNumber == 3 && (_libraEnabledMessage & LibraInformation.Category) != 0)
                return;
            if (_currentLibraMessageNumber == 4 && (_libraEnabledMessage & LibraInformation.ElementWeak) != 0)
                return;
            if (_currentLibraMessageNumber == 5 && (_libraEnabledMessage & LibraInformation.ElementResist) != 0)
                return;
            if (_currentLibraMessageNumber == 6 && (_libraEnabledMessage & LibraInformation.ElementImmune) != 0)
                return;
            if (_currentLibraMessageNumber == 7 && (_libraEnabledMessage & LibraInformation.ElementAbsorb) != 0)
                return;
            if (_currentLibraMessageNumber == 8 && (_libraEnabledMessage & LibraInformation.StatusAuto) != 0)
                return;
            if (_currentLibraMessageNumber == 9 && (_libraEnabledMessage & LibraInformation.StatusImmune) != 0)
                return;
            if (_currentLibraMessageNumber == 10 && (_libraEnabledMessage & LibraInformation.ItemSteal) != 0)
                return;
            if (_currentLibraMessageNumber == 11 && (_libraEnabledMessage & LibraInformation.BlueLearn) != 0)
                return;
            if (_currentLibraMessageNumber == 12 && (_libraEnabledMessage & LibraInformation.AttackList) != 0)
                return;
            _currentLibraMessageNumber++;
        }
    }

    private Boolean DisplayMessageLibra()
    {
        if (_libraBtlData == null)
            return false;

        List<String> multiMessages = null;
        if (_currentLibraMessageNumber == 1)
        {
            String str = String.Empty;
            if ((_libraEnabledMessage & LibraInformation.Name) != 0)
                str += GetLibraMessages(_libraBtlData, LibraInformation.Name)[0];
            if ((_libraEnabledMessage & LibraInformation.Level) != 0)
                str += GetLibraMessages(_libraBtlData, LibraInformation.Level)[0];
            SetBattleMessage(str, 3);
            AdvanceLibraMessageNumber();
            return true;
        }
        if (_currentLibraMessageNumber == 2)
        {
            String str = String.Empty;
            if ((_libraEnabledMessage & LibraInformation.HP) != 0)
                str += GetLibraMessages(_libraBtlData, LibraInformation.HP)[0];
            if ((_libraEnabledMessage & LibraInformation.MP) != 0)
                str += GetLibraMessages(_libraBtlData, LibraInformation.MP)[0];
            SetBattleMessage(str, 3);
            AdvanceLibraMessageNumber();
            return true;
        }
        if (_currentLibraMessageNumber == 3)
            multiMessages = GetLibraMessages(_libraBtlData, LibraInformation.Category);
        else if (_currentLibraMessageNumber == 4)
            multiMessages = GetLibraMessages(_libraBtlData, LibraInformation.ElementWeak);
        else if (_currentLibraMessageNumber == 5)
            multiMessages = GetLibraMessages(_libraBtlData, LibraInformation.ElementResist);
        else if (_currentLibraMessageNumber == 6)
            multiMessages = GetLibraMessages(_libraBtlData, LibraInformation.ElementImmune);
        else if (_currentLibraMessageNumber == 7)
            multiMessages = GetLibraMessages(_libraBtlData, LibraInformation.ElementAbsorb);
        else if (_currentLibraMessageNumber == 8)
            multiMessages = GetLibraMessages(_libraBtlData, LibraInformation.StatusAuto);
        else if (_currentLibraMessageNumber == 9)
            multiMessages = GetLibraMessages(_libraBtlData, LibraInformation.StatusImmune);
        else if (_currentLibraMessageNumber == 10)
            multiMessages = GetLibraMessages(_libraBtlData, LibraInformation.ItemSteal);
        else if (_currentLibraMessageNumber == 11)
            multiMessages = GetLibraMessages(_libraBtlData, LibraInformation.BlueLearn);
        else if (_currentLibraMessageNumber == 12)
            multiMessages = GetLibraMessages(_libraBtlData, LibraInformation.AttackList);
        if (multiMessages != null && _currentLibraMessageCount < multiMessages.Count)
        {
            SetBattleMessage(multiMessages[_currentLibraMessageCount++], 3);
            return true;
        }
        AdvanceLibraMessageNumber();
        if (_currentLibraMessageNumber >= 13)
        {
            _libraBtlData = null;
            _currentLibraMessageCount = 0;
            _currentLibraMessageNumber = 0;
        }
        else
        {
            DisplayMessageLibra();
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

        SetBattleMessage(Localization.CurrentDisplaySymbol != "JP" ? FF9TextTool.BattleLibraText(8) + FF9TextTool.ItemName(id) : FF9TextTool.ItemName(id) + FF9TextTool.BattleLibraText(8), 3);
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

    private void DisplayCommand()
    {
        BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
        BTL_DATA.MONSTER_TRANSFORM transform = btl.Data.is_monster_transform ? btl.Data.monster_transform : null;
        CharacterPresetId presetId = FF9StateSystem.Common.FF9.party.member[btl.Position].PresetId;
        BattleCommandId[] commands = new BattleCommandId[CharacterCommandSet.SupportedMenus.Count];
        _isTranceMenu = btl.IsUnderAnyStatus(BattleStatus.Trance);

        if (_isTranceMenu)
        {
            _commandPanel.SetCaptionText(Localization.Get("TranceCaption"));
        }
        else
        {
            _commandPanel.SetCaptionText(Localization.Get("CommandCaption"));
            _commandPanel.SetCaptionColor(FF9TextTool.White);
        }

        for (Int32 i = 0; i < CharacterCommandSet.SupportedMenus.Count; i++)
        {
            commands[i] = CharacterCommands.CommandSets[presetId].Get(_isTranceMenu, CharacterCommandSet.SupportedMenus[i]);
            commands[i] = BattleCommandHelper.Patch(commands[i], CharacterCommandSet.SupportedMenus[i], btl.Player, btl);
            _commandEnabledState[CharacterCommandSet.SupportedMenus[i]] = BattleCommandHelper.GetCommandEnabledState(commands[i], CharacterCommandSet.SupportedMenus[i], btl.Player, btl);
        }
        BattleCommandId accessMenuCommand = BattleCommandHelper.Patch(BattleCommandId.AccessMenu, BattleCommandMenu.AccessMenu, btl.Player, btl);
        _commandEnabledState[BattleCommandMenu.AccessMenu] = BattleCommandHelper.GetCommandEnabledState(accessMenuCommand, BattleCommandMenu.AccessMenu, btl.Player, btl);

        if (Configuration.Battle.NoAutoTrance && btl.Trance == Byte.MaxValue && !_isTranceMenu)
        {
            if (!_isManualTrance)
            {
                _commandPanel.Change.SetLabelText(Localization.Get("Trance"));
                _commandPanel.Change.SetLabelColor(FF9TextTool.Yellow);
                _commandPanel.Change.ButtonGroup.Help.Text = Localization.GetWithDefault("TranceCommandHelp");
                _isManualTrance = true;
            }
        }
        else
        {
            SetupCommandButton(_commandPanel.Change, commands[5], _commandEnabledState[BattleCommandMenu.Change] == 2, _commandEnabledState[BattleCommandMenu.Change] == 0);
            _isManualTrance = false;
        }
        
        SetupCommandButton(_commandPanel.Attack, commands[0], _commandEnabledState[BattleCommandMenu.Attack] == 2, _commandEnabledState[BattleCommandMenu.Attack] == 0);
        SetupCommandButton(_commandPanel.Defend, commands[1], _commandEnabledState[BattleCommandMenu.Defend] == 2, _commandEnabledState[BattleCommandMenu.Defend] == 0);
        SetupCommandButton(_commandPanel.Skill1, commands[2], _commandEnabledState[BattleCommandMenu.Ability1] == 2, _commandEnabledState[BattleCommandMenu.Ability1] == 0);
        SetupCommandButton(_commandPanel.Skill2, commands[3], _commandEnabledState[BattleCommandMenu.Ability2] == 2, _commandEnabledState[BattleCommandMenu.Ability2] == 0);
        SetupCommandButton(_commandPanel.Item, commands[4], _commandEnabledState[BattleCommandMenu.Item] == 2, _commandEnabledState[BattleCommandMenu.Item] == 0);
        if (_commandPanel.AccessMenu != null)
            SetupCommandButton(_commandPanel.AccessMenu, accessMenuCommand, Configuration.Battle.AccessMenus > 0 && _commandEnabledState[BattleCommandMenu.AccessMenu] == 2);

        if (ButtonGroupState.ActiveGroup == CommandGroupButton)
            SetCommandVisibility(true, false);
    }

    private void DisplayStatus(TargetDisplay subMode)
    {
        StatusContainer.SetActive(true);
        _statusPanel.SetActive(false);
        _partyDetail.SetActive(true);

        List<Int32> list = [0, 1, 2, 3];
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
            numberSubModeHud.Value.SetText(String.IsNullOrEmpty(player.UILabelHP) ? player.CurrentHp.ToString() : player.UILabelHP);
            numberSubModeHud.MaxValue.SetText(player.MaximumHp.ToString());
            if (!player.IsTargetable)
                numberSubModeHud.SetColor(FF9TextTool.Gray);
            else if (CheckHPState(player) == ParameterStatus.Dead)
                numberSubModeHud.SetColor(FF9TextTool.Red);
            else
                numberSubModeHud.SetColor(player.UIColorHP);
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
            numberSubModeHud.Value.SetText(String.IsNullOrEmpty(player.UILabelMP) ? player.CurrentMp.ToString() : player.UILabelMP);
            numberSubModeHud.MaxValue.SetText(player.MaximumMp.ToString());
            numberSubModeHud.SetColor(player.IsTargetable ? player.UIColorMP : FF9TextTool.Gray);
            list.Remove(index);
        }

        foreach (Int32 index in list)
            _statusPanel.MP.Array[index].IsActive = false;
    }

    private void DisplayTargetStatus(List<Int32> list, UI.ContainerStatus.PanelDetail<UI.ContainerStatus.IconsWidget> statusPanel, Dictionary<BattleStatusId, String> iconNames)
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
            foreach (KeyValuePair<BattleStatusId, String> status in iconNames)
            {
                if (iconIndex >= uiStatus.Icons.Count)
                    break;
                if (!player.IsUnderAnyStatus(status.Key.ToBattleStatus()))
                    continue;

                UISprite sprite = uiStatus.Icons[iconIndex].Sprite;
                sprite.alpha = 1f;
                sprite.spriteName = status.Value;
                iconIndex++;
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
        if (_currentSilenceStatus != unit.IsUnderAnyStatus(BattleStatusConst.CannotUseMagic))
        {
            _currentSilenceStatus = !_currentSilenceStatus;
            DisplayAbility();
            return;
        }

        if (_currentMpValue != unit.CurrentMp)
        {
            _currentMpValue = (Int32)unit.CurrentMp;
            DisplayAbility();
            return;
        }

        HashSet<BattleMagicSwordSet> unitMagicSet = new HashSet<BattleMagicSwordSet>(_abilityDetailDict[CurrentPlayerIndex].AbilityMagicSet.Values);
        Boolean newMagicSwordState = true;
        foreach (BattleMagicSwordSet magicSet in unitMagicSet)
        {
            BattleUnit supporter = FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits().FirstOrDefault(u => u.PlayerIndex == magicSet.Supporter);
            newMagicSwordState = newMagicSwordState && supporter != null && !unit.IsUnderAnyStatus(BattleStatusConst.Immobilized | magicSet.BeneficiaryBlockingStatus) && !supporter.IsUnderAnyStatus(BattleStatusConst.Immobilized | magicSet.SupporterBlockingStatus);
        }
        if (_currentMagicSwordState != newMagicSwordState)
        {
            _currentMagicSwordState = newMagicSwordState;
            DisplayAbility();
            return;
        }
    }

    private void DisplayItemRealTime()
    {
        if (!_needItemUpdate)
            return;

        _needItemUpdate = false;
        DisplayItem(CharacterCommands.Commands[_currentCommandId]);
    }

    private void DisplayItem(CharacterCommand ff9Command)
    {
        _itemIdList.Clear();
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();
        Boolean isThrow = ff9Command.Type == CharacterCommandType.Throw;
        foreach (FF9ITEM ff9Item in FF9StateSystem.Common.FF9.item)
        {
            if (ff9Item.count <= 0)
                continue;
            FF9ITEM_DATA itemData = ff9item._FF9Item_Data[ff9Item.id];
            Boolean canBeUsed;
            if (isThrow)
                canBeUsed = ff9item.CanThrowItem(itemData);
            else
                canBeUsed = ff9item.HasItemEffect(ff9Item.id) && (itemData.type & ItemType.Item) != 0;
            if (canBeUsed)
            {
                if (CurrentPlayerIndex >= 0 && !String.IsNullOrEmpty(itemData.use_condition))
                {
                    if (ff9Command.OnlySpecificItem && !itemData.use_condition.Contains("CommandId"))
                    {
                        canBeUsed = false;
                    }
                    else
                    {
                        Expression c = new Expression(itemData.use_condition);
                        NCalcUtility.InitializeExpressionUnit(ref c, FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex));
                        c.Parameters["CommandId"] = (Int32)_currentCommandId;
                        c.Parameters["CommandMenu"] = (Int32)_currentCommandIndex;
                        c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        canBeUsed = NCalcUtility.EvaluateNCalcCondition(c.Evaluate());
                    }
                }
                else if (ff9Command.OnlySpecificItem)
                {
                    canBeUsed = false;
                }
            }
            if (canBeUsed)
            {
                _itemIdList.Add(ff9Item.id);
                BattleItemListData battleItemListData = new BattleItemListData
                {
                    Count = ff9Item.count,
                    Id = ff9Item.id
                };
                if (_doubleCastCount == 2 && battleItemListData.Id == (RegularItem)_firstCommand.SubId && IsMixCast)
                {
                    battleItemListData.Count--;
                }
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

    private AA_DATA GetSelectedActiveAbility(Int32 playerIndex, BattleCommandId cmdId, Int32 abilityIndex, out Int32 subNo, out BattleAbilityId abilId)
    {
        CharacterCommand ff9Command = CharacterCommands.Commands[cmdId];
        if (CommandIsMonsterTransformCommand(playerIndex, cmdId, out BTL_DATA.MONSTER_TRANSFORM transform))
        {
            subNo = ff9Command.ListEntry[abilityIndex];
            abilId = BattleAbilityId.Void;
            return transform.spell[subNo];
        }
        abilId = PatchAbility(ff9Command.GetAbilityId(abilityIndex));
        subNo = (Int32)abilId;
        return FF9StateSystem.Battle.FF9Battle.aa_data[abilId];
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
                itemListDetailHud.NameLabel.rawText = aaData.Name;
                itemListDetailHud.Button.Help.Text = String.Empty;
            }
            else
            {
                BattleAbilityId patchedID = PatchAbility(ff9abil.GetActiveAbilityFromAbilityId(battleAbilityListData.Id));
                mp = GetActionMpCost(FF9StateSystem.Battle.FF9Battle.aa_data[patchedID], curUnit, patchedID);
                itemListDetailHud.NameLabel.rawText = FF9TextTool.ActionAbilityName(patchedID);
                itemListDetailHud.Button.Help.Text = FF9TextTool.ActionAbilityHelpDescription(patchedID);
            }
            itemListDetailHud.NumberLabel.rawText = mp == 0 ? String.Empty : mp.ToString();

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
        List<Int32> matchBattleIdPlayerCurrentList = new List<Int32>();
        List<Int32> matchBattleIdEnemyCurrentList = new List<Int32>();
        Int32 enemyCount = 0;
        Int32 playerCount = 0;

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            Int32 index = unit.GetIndex();

            if (unit.IsTargetable)
            {
                if (unit.IsPlayer)
                {
                    playerCount++;
                    matchBattleIdPlayerCurrentList.Add(index);
                }
                else
                {
                    enemyCount++;
                    matchBattleIdEnemyCurrentList.Add(index);
                }
            }
        }

        if (enemyCount != enemyCountOld || playerCount != playerCountOld || !Enumerable.SequenceEqual(matchBattleIdPlayerCurrentList, _matchBattleIdPlayerList) || !Enumerable.SequenceEqual(matchBattleIdEnemyCurrentList, _matchBattleIdEnemyList))
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
                else
                {
                    _targetPanel.Players[playerIndex].Name.Label.rawText = unit.NameTag;
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
                nameLabel.SetText(unit.Player.NameTag);
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
                else
                {
                    if (_cursorType == CursorGroup.Individual)
                        ButtonGroupState.SetButtonEnable(labelObj, true);
                    nameLabel.color = unit.UIColorHP;
                }
                if (_targetCursor == TargetType.Self && unit.GetIndex() == CurrentPlayerIndex && currentTargetLabel != labelObj)
                {
                    Singleton<PointerManager>.Instance.RemovePointerFromGameObject(currentTargetLabel);
                    _currentTargetIndex = unit.GetIndex();
                    currentTargetLabel = labelObj;
                }
                ++playerIndex;
            }
            else
            {
                GONavigationButton enemyHUD = _targetPanel.Enemies[enemyIndex];
                GameObject labelObj = enemyHUD.GameObject;
                UILabel nameLabel = enemyHUD.Name.Label;
                labelObj.SetActive(true);
                nameLabel.rawText = GetEnemyDisplayName(unit);
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
                                    if (_currentCommandIndex == BattleCommandMenu.Attack && FF9StateSystem.PCPlatform && _enemyCount > 1)
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

    private String GetEnemyDisplayName(BattleUnit enemy)
    {
        if (!Localization.UseSecondaryLanguage || enemy.Data.typeNo == Byte.MaxValue)
            return enemy.Name;
        return FF9TextTool.BattleText(enemy.Data.typeNo);
    }

    private String GetEnemyCommandDisplayName(AA_DATA enemyAbility)
    {
        if (!Localization.UseSecondaryLanguage)
            return enemyAbility.Name;
        for (Int32 i = 0; i < FF9StateSystem.Battle.FF9Battle.enemy_attack.Count; i++)
            if (FF9StateSystem.Battle.FF9Battle.enemy_attack[i] == enemyAbility)
                return FF9TextTool.BattleText(FF9StateSystem.Battle.FF9Battle.btl_scene.header.TypCount + i);
        return enemyAbility.Name;
    }

    private void DisplayCharacterParameter(UI.PanelParty.Character playerHud, BattleUnit bd, DamageAnimationInfo hp, DamageAnimationInfo mp)
    {
        playerHud.Name.Label.SetText(bd.NameTag);
        playerHud.HP.SetText(String.IsNullOrEmpty(bd.UILabelHP) ? hp.CurrentValue.ToString() : bd.UILabelHP);
        playerHud.MP.SetText(String.IsNullOrEmpty(bd.UILabelMP) ? mp.CurrentValue.ToString() : bd.UILabelMP);
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
            default:
                playerHud.ATBBar.SetProgress(bd.CurrentAtb / (Single)bd.MaximumAtb);
                playerHud.HP.SetColor(bd.UIColorHP);
                playerHud.Name.SetColor(bd.UIColorHP);
                break;
        }

        playerHud.MP.SetColor(bd.UIColorMP);
        playerHud.ATBBar.Foreground.Foreground.Sprite.spriteName = bd.UISpriteATB;
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

        if (btl_para.CheckPointDataStatus(bd) != 0)
            return ParameterStatus.Critical;

        return ParameterStatus.Normal;
    }

    private static ParameterStatus CheckMPState(BattleUnit bd)
    {
        // Dummied (see btl_para.CheckPointDataStatus instead)
        return bd.CurrentMp <= bd.MaximumMp / 6.0 ? ParameterStatus.Critical : ParameterStatus.Normal;
    }

    private Boolean CheckDoubleCast(Int32 battleIndex, CursorGroup cursorGroup)
    {
        if (!IsDoubleCast || (IsDoubleCast && _doubleCastCount == 2))
        {
            _doubleCastCount = 0;
            return SetTarget(battleIndex);
        }
        if (_doubleCastCount > 2) // TODO: handle more than 2 commands (triple cast etc...)
            return false;

        ++_doubleCastCount;
        _firstCommand = ProcessCommand(battleIndex, cursorGroup);
        CharacterCommand ff9Command = CharacterCommands.Commands[_firstCommand.CommandId];
        CharacterCommandType commandType = ff9Command.Type;

        if (commandType == CharacterCommandType.Item || commandType == CharacterCommandType.Throw)
        {
            _subMenuType = commandType == CharacterCommandType.Item ? SubMenuType.Item : SubMenuType.Throw;
            DisplayItem(ff9Command);
            SetTargetVisibility(false);
            SetItemPanelVisibility(true, true);
        }
        else
        {
            _subMenuType = SubMenuType.Ability;
            DisplayAbility();
            SetTargetVisibility(false);
            SetAbilityPanelVisibility(true, true);
        }
        FF9Sfx.FF9SFX_Play(103);
        BackButton.SetActive(FF9StateSystem.MobilePlatform);
        return false;
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

    private Int32 GetActionMpCost(AA_DATA aaData, BattleUnit unit, BattleAbilityId abilId = BattleAbilityId.Void, Boolean considerCommandMenu = true)
    {
        Int32 mpCost = aaData.MP;
        if (considerCommandMenu)
            BattleAbilityHelper.GetPatchedMPCost(ref mpCost, abilId, unit, _currentCommandId, _currentCommandIndex, aaData);
        else
            BattleAbilityHelper.GetPatchedMPCost(ref mpCost, abilId, unit, BattleCommandId.None, BattleCommandMenu.None, aaData);

        if ((aaData.Type & 4) != 0 && battle.GARNET_SUMMON_FLAG != 0)
            mpCost *= 4;

        mpCost = mpCost * unit.Player.mpCostFactor / 100;

        return mpCost;
    }

    private AbilityStatus GetMonsterTransformAbilityState(Int32 abilId, Int32 playerIndex = -1)
    {
        Boolean checkCurrentPlayer = playerIndex < 0;
        if (checkCurrentPlayer)
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

            if (unit.IsUnderAnyStatus(BattleStatusConst.CannotUseMagic))
                return AbilityStatus.Disable;
        }

        if (GetActionMpCost(aaData, unit, BattleAbilityId.Void, checkCurrentPlayer) > unit.CurrentMp)
            return AbilityStatus.Disable;

        return AbilityStatus.Enable;
    }

    private AbilityStatus GetAbilityState(Int32 abilId, Int32 playerIndex = -1)
    {
        Boolean checkCurrentPlayer = playerIndex < 0;
        if (checkCurrentPlayer)
            playerIndex = CurrentPlayerIndex;
        AbilityPlayerDetail abilityPlayerDetail = _abilityDetailDict[playerIndex];
        BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(playerIndex);
        BattleAbilityId patchedId = BattleAbilityHelper.Patch(ff9abil.GetActiveAbilityFromAbilityId(abilId), unit.Player);
        AA_DATA patchedAbil = FF9StateSystem.Battle.FF9Battle.aa_data[patchedId];

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

        if ((patchedAbil.Category & 2) != 0)
        {
            if (FF9StateSystem.Battle.FF9Battle.btl_scene.Info.NoMagical)
                return AbilityStatus.Disable;

            if (unit.IsUnderAnyStatus(BattleStatusConst.CannotUseMagic))
                return AbilityStatus.Disable;
        }

        if (abilityPlayerDetail.AbilityMagicSet.TryGetValue(abilId, out BattleMagicSwordSet magicSet))
        {
            if (unit.IsUnderAnyStatus(BattleStatusConst.Immobilized | magicSet.BeneficiaryBlockingStatus))
                return AbilityStatus.Disable;
            BattleUnit supporter = FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits().FirstOrDefault(u => u.PlayerIndex == magicSet.Supporter);
            if (supporter == null || supporter.IsUnderAnyStatus(BattleStatusConst.Immobilized | magicSet.SupporterBlockingStatus))
                return AbilityStatus.Disable;
        }

        if (BattleAbilityHelper.IsAbilityDisabled(patchedId, unit, checkCurrentPlayer ? _currentCommandId : BattleCommandId.None, checkCurrentPlayer ? _currentCommandIndex : BattleCommandMenu.None))
            return AbilityStatus.Disable;
        if (GetActionMpCost(patchedAbil, unit, patchedId, checkCurrentPlayer) > unit.CurrentMp)
            return AbilityStatus.Disable;

        return AbilityStatus.Enable;
    }

    private void SetAbilityAp(AbilityPlayerDetail abilityPlayer)
    {
        PLAYER player = abilityPlayer.Player;
        if (!abilityPlayer.HasAp)
            return;

        CharacterAbility[] abilArray = ff9abil._FF9Abil_PaData[player.PresetId];
        for (Int32 i = 0; i < abilArray.Length; i++)
        {
            if (abilArray[i].Id == 0)
                continue;

            abilityPlayer.AbilityPaList[abilArray[i].Id] = player.pa[i];
            abilityPlayer.AbilityMaxPaList[abilArray[i].Id] = abilArray[i].Ap;
        }
    }

    private static void SetAbilityEquip(AbilityPlayerDetail abilityPlayer)
    {
        PLAYER player = abilityPlayer.Player;
        for (Int32 i = 0; i < 5; ++i)
        {
            RegularItem itemId = player.equip[i];
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
        PLAYER player = abilityPlayer.Player;
        CharacterPresetId presetId = player.PresetId;
        if (!ff9abil.FF9Abil_HasAp(player))
            return;

        for (Int32 i = 0; i < CharacterCommandSet.SupportedMenus.Count; i++)
        {
            BattleCommandId normalCommandId = CharacterCommands.CommandSets[presetId].GetRegular(CharacterCommandSet.SupportedMenus[i]);
            BattleCommandId tranceCommandId = CharacterCommands.CommandSets[presetId].GetTrance(CharacterCommandSet.SupportedMenus[i]);
            if (normalCommandId == tranceCommandId)
                continue;
            CharacterCommand normalCommand = CharacterCommands.Commands[normalCommandId];
            CharacterCommand tranceCommand = CharacterCommands.Commands[tranceCommandId];
            if (normalCommand.Type != CharacterCommandType.Ability || tranceCommand.Type != CharacterCommandType.Ability)
                continue;

            Int32 count = Math.Min(normalCommand.ListEntry.Length, tranceCommand.ListEntry.Length);
            for (Int32 j = 0; j < count; ++j)
            {
                Int32 normalId = normalCommand.ListEntry[j];
                Int32 tranceId = tranceCommand.ListEntry[j];
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
                abilityPlayer.AbilityMagicSet[magicSet.UnlockedAbilities[i]] = magicSet;
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
        _currentCommandIndex = BattleCommandMenu.Attack;
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
            Menu = _currentCommandIndex,
            CommandId = _currentCommandId,
            SubId = 0
        };

        BattleCommandId cmdId = commandDetail.CommandId;

        CharacterCommandType commandType = CharacterCommands.Commands[cmdId].Type;
        if (commandType == CharacterCommandType.Normal || commandType == CharacterCommandType.Ability || commandType == CharacterCommandType.Instant)
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
            commandDetail.Menu = BattleCommandMenu.None;
            commandDetail.CommandId = BattleCommandId.BlackMagic;
            commandDetail.SubId = (Int32)bestAbility;
            caster.Data.cmd[0].info.IsZeroMP = true;
        }
    }

    private Boolean SendCommand(CommandDetail command)
    {
        BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex];
        CMD_DATA cmd = btl.cmd[0];
        cmd.regist.sel_mode = 1;
        btl_cmd.SetCommand(cmd, command.CommandId, command.SubId, command.TargetId, command.TargetType, cmdMenu: command.Menu);
        SetPartySwapButtonActive(false);
        InputFinishList.Add(CurrentPlayerIndex);

        _partyDetail.SetBlink(CurrentPlayerIndex, false);
        return true;
    }

    private Boolean SendDoubleCastCommand(CommandDetail first, CommandDetail second)
    {
        CMD_DATA cmd1 = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].cmd[3];
        CMD_DATA cmd2 = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].cmd[0]; // The second command sent/performed is the main one (it resets the ATB correctly amongst other things)
        cmd1.regist.sel_mode = 1;
        btl_cmd.SetCommand(cmd1, first.CommandId, first.SubId, first.TargetId, first.TargetType, cmdMenu: first.Menu);
        btl_cmd.SetCommand(cmd2, second.CommandId, second.SubId, second.TargetId, second.TargetType, cmdMenu: second.Menu);
        SetPartySwapButtonActive(false);
        InputFinishList.Add(CurrentPlayerIndex);

        _partyDetail.SetBlink(CurrentPlayerIndex, false);
        return true;
    }

    private Boolean SendMixCommand(CommandDetail[] allInputs)
    {
        CommandDetail lastInput = allInputs[allInputs.Length - 1];
        CharacterCommand charCmd = CharacterCommands.Commands[lastInput.CommandId];
        MixItems mixRequest = new MixItems()
        {
            Id = -1,
            Result = RegularItem.NoItem,
            Ingredients = allInputs.Select(detail => (RegularItem)detail.SubId).ToList()
        };
        Dictionary<RegularItem, Int32> requestIngredients = mixRequest.GetIngredientsAsDict();
        foreach (MixItems mixCandidate in ff9mixitem.MixItemsData.Values)
        {
            if (mixCandidate.Id < 0)
                continue;
            if (mixCandidate.Length != mixRequest.Length)
                continue;
            Dictionary<RegularItem, Int32> candidateIngredients = mixCandidate.GetIngredientsAsDict();
            if (candidateIngredients.All(kvp => requestIngredients.TryGetValue(kvp.Key, out Int32 cnt) && cnt == kvp.Value))
            {
                mixRequest.Id = mixCandidate.Id;
                break;
            }
        }
        CMD_DATA cmd = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].cmd[0];
        if (mixRequest.Id >= 0)
        {
            cmd.regist.sel_mode = 1;
            if (charCmd.Type == CharacterCommandType.Item)
                lastInput.TargetId = GetTargetFromType(lastInput, cmd, ff9item.GetItemEffect(ff9mixitem.MixItemsData[mixRequest.Id].Result).info.Target);
            btl_cmd.SetCommand(cmd, lastInput.CommandId, mixRequest.Id, lastInput.TargetId, lastInput.TargetType, cmdMenu: lastInput.Menu);
        }
        else
        {
            MixCommandType mixInfo = MixCommandSet[lastInput.CommandId];
            if (mixInfo.failType == FailedMixType.CANCEL_MENU)
            {
                // No command sent, return to a menu (first item pick)
                if (charCmd.Type == CharacterCommandType.Throw)
                {
                    _doubleCastCount = 1;
                    _subMenuType = SubMenuType.Throw;
                    DisplayItem(charCmd);
                    SetTargetVisibility(false);
                    SetItemPanelVisibility(true, false);
                }
                else if (charCmd.Type == CharacterCommandType.Item)
                {
                    _doubleCastCount = 1;
                    _subMenuType = SubMenuType.Item;
                    DisplayItem(charCmd);
                    SetTargetVisibility(false);
                    SetItemPanelVisibility(true, false);
                }
                if (mixInfo.consumeOnFail)
                    foreach (RegularItem ingredient in mixRequest.Ingredients)
                        UIManager.Battle.ItemUse(ingredient);
                FF9Sfx.FF9SFX_Play(1046);
                return false;
            }
            cmd.regist.sel_mode = 1;
            if (mixInfo.failType == FailedMixType.FIRST_ITEM || mixInfo.failType == FailedMixType.SECOND_ITEM || mixInfo.failType == FailedMixType.USE_ITEMS)
            {
                // Don't use the mix command
                BattleCommandId commandReplacement = charCmd.Type == CharacterCommandType.Item ? BattleCommandId.Item : BattleCommandId.Throw;
                if (mixInfo.failType == FailedMixType.USE_ITEMS)
                {
                    CMD_DATA cmd2 = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].cmd[3]; // TODO: handle more than 2 items
                    if (charCmd.Type == CharacterCommandType.Item)
                    {
                        allInputs[0].TargetId = GetTargetFromType(lastInput, cmd, ff9item.GetItemEffect((RegularItem)allInputs[0].SubId).info.Target);
                        allInputs[1].TargetId = GetTargetFromType(lastInput, cmd, ff9item.GetItemEffect((RegularItem)allInputs[1].SubId).info.Target);
                    }
                    btl_cmd.SetCommand(cmd2, commandReplacement, allInputs[0].SubId, allInputs[0].TargetId, lastInput.TargetType, cmdMenu: lastInput.Menu);
                    btl_cmd.SetCommand(cmd, commandReplacement, allInputs[1].SubId, allInputs[1].TargetId, lastInput.TargetType, cmdMenu: lastInput.Menu);
                }
                else
                {
                    Int32 itemIdUsed = mixInfo.failType == FailedMixType.SECOND_ITEM ? allInputs[1].SubId : allInputs[0].SubId;
                    btl_cmd.SetCommand(cmd, commandReplacement, itemIdUsed, lastInput.TargetId, lastInput.TargetType, cmdMenu: lastInput.Menu);
                }
            }
            else
            {
                // Use the mix command with the non-registered request (it is added to the database with a negative ID)
                mixRequest.Id = -1 - CurrentPlayerIndex;
                ff9mixitem.MixItemsData[mixRequest.Id] = mixRequest;
                if (mixInfo.failType == FailedMixType.FAIL_ITEM)
                {
                    mixRequest.Result = mixInfo.failItem;
                    if (charCmd.Type == CharacterCommandType.Item)
                        lastInput.TargetId = GetTargetFromType(lastInput, cmd, ff9item.GetItemEffect(mixInfo.failItem).info.Target);
                }
                btl_cmd.SetCommand(cmd, lastInput.CommandId, mixRequest.Id, lastInput.TargetId, lastInput.TargetType, cmdMenu: lastInput.Menu);
            }
        }
        SetPartySwapButtonActive(false);
        InputFinishList.Add(CurrentPlayerIndex);
        _partyDetail.SetBlink(CurrentPlayerIndex, false);
        return true;
    }

    private static ushort GetTargetFromType(CommandDetail cmddetail, CMD_DATA cmd, TargetType TargetType)
    {
        switch (TargetType)
        {
            case TargetType.AllAlly:
                return 0x0F;
            case TargetType.AllEnemy:
                return 0xF0;
            case TargetType.Everyone:
                return 0xFF;
            case TargetType.Self:
                return cmd.regist.btl_id;
            default:
                return cmddetail.TargetId;
        }
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
            if (ButtonGroupState.ActiveButton == null || (_buttonSliding == null && !ButtonGroupState.ActiveButton.GetComponent<ButtonGroupState>().enabled))
                ButtonGroupState.ActiveButton = commandObject;
        }
        else
        {
            _commandPanel.IsActive = true;
            ButtonGroupState.RemoveCursorMemorize(CommandGroupButton);

            GameObject previousSelection = commandObject;
            if (commandObject.GetComponent<ButtonGroupState>().enabled)
                TryGetMemorizedCommandObject(ref previousSelection, forceCursorMemo);
            if (previousSelection.GetComponent<ButtonGroupState>().enabled)
                commandObject = previousSelection;

            ButtonGroupState.SetCursorMemorize(commandObject, CommandGroupButton);
        }

        if (_hidingHud)
            _currentButtonGroup = CommandGroupButton;
        else
            ButtonGroupState.ActiveGroup = CommandGroupButton;
    }

    private void TryMemorizeCommand()
    {
        if (Configuration.Interface.PSXBattleMenu && (_currentCommandIndex == BattleCommandMenu.Defend || _currentCommandIndex == BattleCommandMenu.Change))
            return;
        _commandCursorMemorize[CurrentPlayerIndex] = _currentCommandIndex;
    }

    private void TryGetMemorizedCommandObject(ref GameObject commandObject, Boolean forceCursorMemo)
    {
        if (!forceCursorMemo && (Int64)FF9StateSystem.Settings.cfg.cursor == 0L)
            return;
        if (_commandCursorMemorize.TryGetValue(CurrentPlayerIndex, out BattleCommandMenu memorizedCommand))
            commandObject = _commandPanel.GetCommandButton(memorizedCommand);
    }

    private void SetItemPanelVisibility(Boolean isVisible, Boolean forceCursorMemo)
    {
        if (isVisible)
        {
            if (!ItemPanel.activeSelf)
            {
                ItemPanel.SetActive(true);
                ButtonGroupState.RemoveCursorMemorize(ItemGroupButton);
                PairCharCommand cursorKey = new PairCharCommand(CurrentPlayerIndex, _currentCommandId);
                if (_abilityCursorMemorize.ContainsKey(cursorKey) && FF9StateSystem.Settings.cfg.cursor != 0 || forceCursorMemo)
                    _itemScrollList.JumpToIndex(_abilityCursorMemorize[cursorKey], true);
                else
                    _itemScrollList.JumpToIndex(0, false);
            }
            if (IsDoubleCast && _doubleCastCount == 1)
                ButtonGroupState.SetPointerNumberToGroup(1, ItemGroupButton);
            else if (IsDoubleCast && _doubleCastCount == 2)
                ButtonGroupState.SetPointerNumberToGroup(2, ItemGroupButton);
            else
                ButtonGroupState.SetPointerNumberToGroup(-1, ItemGroupButton);
            ButtonGroupState.ActiveGroup = ItemGroupButton;
            ButtonGroupState.UpdateActiveButton();
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
                ButtonGroupState.SetPointerNumberToGroup(-1, AbilityGroupButton);
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

            if (_currentCommandId == BattleCommandId.Attack)
            {
                BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
                if (btl.IsHealingRod || btl.HasSupportAbility(SupportAbility1.Healer)) // TODO: should be coded as a SA feature instead of being hard-coded
                    _defaultTargetHealingAttack = true;
            }
            else if (CharacterCommands.Commands[_currentCommandId].Type == CharacterCommandType.Item)
            {
                RegularItem itemId = _itemIdList[_currentSubMenuIndex];
                ITEM_DATA itemData = ff9item.GetItemEffect(itemId);
                targetType = itemData.info.Target;
                _defaultTargetAlly = itemData.info.DefaultAlly;
                _defaultTargetDead = itemData.info.ForDead;
                _targetDead = itemData.info.ForDead;
                subMode = itemData.info.DisplayStats;
                if (!MixCommandSet.ContainsKey(_currentCommandId))
                {
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
                else
                    _targetDead = true; // Always true to target any player, whatever the result. (with a Mix Command)
            }
            else
            {
                AA_DATA aaData = GetSelectedActiveAbility(CurrentPlayerIndex, _currentCommandId, _currentSubMenuIndex, out Int32 subNo, out _);
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
                if (CharacterCommands.Commands[_currentCommandId].Type == CharacterCommandType.Throw && ff9item.HasItemEffect(_itemIdList[_currentSubMenuIndex]))
                {
                    ITEM_DATA effect = ff9item.GetItemEffect(_itemIdList[_currentSubMenuIndex]);
                    if (targetType == TargetType.SingleAny || targetType == TargetType.SingleAlly || targetType == TargetType.SingleEnemy)
                        targetType = effect.info.Target;
                    _defaultTargetAlly = effect.info.DefaultAlly;
                    _defaultTargetDead = effect.info.DefaultOnDead;
                    _targetDead = effect.info.ForDead;
                }
                SelectBestTarget(targetType, testCommand);
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
            _cursorType = CursorGroup.Individual;
            ChangeTargetAvailability(player: true, enemy: true, all: FF9StateSystem.MobilePlatform, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.ManyEnemy)
        {
            _cursorType = CursorGroup.Individual;
            ChangeTargetAvailability(player: false, enemy: true, all: FF9StateSystem.MobilePlatform, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.ManyAlly)
        {
            _cursorType = CursorGroup.Individual;
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
                _cursorType = _defaultTargetAlly ? CursorGroup.AllPlayer : CursorGroup.AllEnemy;
            else
                _cursorType = CursorGroup.All;
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
                    if (_currentCommandIndex == BattleCommandMenu.Attack && FF9StateSystem.PCPlatform)
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
                        if (_currentCommandIndex == BattleCommandMenu.Attack && FF9StateSystem.PCPlatform)
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
                String targetName = displayName ? unit.Player.NameTag : String.Empty;
                targetHud.ButtonGroup.Help.Enable = true;
                targetHud.ButtonGroup.Help.Text = cursorHelp + "\n" + targetName;
                ++playerIndex;
            }
            else
            {
                GONavigationButton targetHud = _targetPanel.Enemies[enemyIndex];
                String targetName = displayName ? GetEnemyDisplayName(unit) : String.Empty;
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
        if (!Configuration.Cheats.AutoBattle) return;

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

    public void DisableAutoBattle()
    {
        _isAutoAttack = false;
    }


    private BattleAbilityId PatchAbility(BattleAbilityId id)
    {
        BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
        return BattleAbilityHelper.Patch(id, unit.Player);
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


    private Boolean SetTarget(Int32 battleIndex)
    {
        // This could be moved to AbilityFeatures.txt but whatever...
        if (_currentCommandId == BattleCommandId.Attack && CurrentBattlePlayerIndex > -1)
        {
            BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
            if (btl.PlayerIndex == CharacterId.Beatrix)
            {
                PLAYER player = btl.Player;
                if (player.equip.Weapon == RegularItem.SaveTheQueen && player.equip.Accessory == RegularItem.SaveTheQueen)
                {
                    CommandDetail first = ProcessCommand(battleIndex, _cursorType);
                    SendDoubleCastCommand(first, first);
                    return true;
                }
            }
        }

        Boolean success;
        if (IsMixCast)
            success = SendMixCommand([_firstCommand, ProcessCommand(battleIndex, _cursorType)]);
        else if (IsDoubleCast)
            success = SendDoubleCastCommand(_firstCommand, ProcessCommand(battleIndex, _cursorType));
        else
            success = SendCommand(ProcessCommand(battleIndex, _cursorType));

        return success;
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
        Boolean FirstIngredientMixed = _doubleCastCount == 2 && battleItemListData.Id == (RegularItem)_firstCommand.SubId;

        if (isInit)
            DisplayWindowBackground(item.gameObject, null);

        if (battleItemListData.Id == RegularItem.NoItem)
        {
            detailWithIconHud.IconSprite.alpha = 0.0f;
            detailWithIconHud.NameLabel.rawText = String.Empty;
            detailWithIconHud.NumberLabel.rawText = String.Empty;
            detailWithIconHud.Button.Help.Enable = false;
            detailWithIconHud.Button.Help.TextKey = String.Empty;
            detailWithIconHud.Button.Help.Text = String.Empty;
        }
        else
        {
            FF9UIDataTool.DisplayItem(battleItemListData.Id, detailWithIconHud.IconSprite, detailWithIconHud.NameLabel, true);
            detailWithIconHud.NumberLabel.rawText = battleItemListData.Count.ToString();
            detailWithIconHud.NameLabel.color = FirstIngredientMixed ? (battleItemListData.Count == 0 ? FF9TextTool.DarkYellow : FF9TextTool.Yellow) : (battleItemListData.Count == 0 ? FF9TextTool.Gray : FF9TextTool.White);
            detailWithIconHud.NumberLabel.color = FirstIngredientMixed ? (battleItemListData.Count == 0 ? FF9TextTool.DarkYellow : FF9TextTool.Yellow) : (battleItemListData.Count == 0 ? FF9TextTool.Gray : FF9TextTool.White);
            detailWithIconHud.Button.Help.Enable = true;
            detailWithIconHud.Button.Help.TextKey = String.Empty;
            detailWithIconHud.Button.Help.Text = FF9TextTool.ItemBattleDescription(battleItemListData.Id);
        }
    }

    private void UpdatePlayersForMainMenu()
    {
        _mainMenuPlayerMemo.Clear();
        for (Int32 i = 0; i < 4; i++)
            _mainMenuPlayerMemo.Add(new PlayerMemo(null, true));
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;
            PLAYER player = unit.Player;
            BattleStatus playerPermanentStatus = player.permanent_status;
            player.permanent_status |= unit.PermanentStatus & BattleStatusConst.OutOfBattle;
            player.trance = unit.Trance;
            btl_init.CopyPoints(player.cur, unit.Data.cur);
            btl_stat.SaveStatus(player, unit.Data);
            _mainMenuPlayerMemo[unit.Position] = new PlayerMemo(player, true);
            _mainMenuPlayerMemo[unit.Position].playerPermanentStatus = playerPermanentStatus;
        }
        for (Int32 i = 0; i < 4; i++)
            if ((_mainMenuSinglePlayer != null && _mainMenuSinglePlayer != FF9StateSystem.Common.FF9.party.member[i]) || _mainMenuPlayerMemo[i].original == null)
                FF9StateSystem.Common.FF9.party.member[i] = null;
    }

    private void UpdateBattleAfterMainMenu()
    {
        Boolean menuHadImpact = PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount > 0;
        PARTY_DATA party = FF9StateSystem.Common.FF9.party;
        if (menuHadImpact)
        {
            List<BattleUnit> oldSwappedOut = new List<BattleUnit>();
            List<CharacterId> newSwappedIn = new List<CharacterId>();
            foreach (PLAYER player in party.member)
                if (player != null)
                    newSwappedIn.Add(player.Index);
            foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
            {
                if (!unit.IsPlayer)
                    continue;
                if (_mainMenuSinglePlayer != null && unit.Player != _mainMenuSinglePlayer)
                {
                    newSwappedIn.Remove(unit.PlayerIndex);
                    continue;
                }
                if (!party.IsInParty(unit.PlayerIndex))
                {
                    oldSwappedOut.Add(unit);
                    continue;
                }
                newSwappedIn.Remove(unit.PlayerIndex);
                PLAYER player = unit.Player;
                PlayerMemo beforeMenu = _mainMenuPlayerMemo.Find(memo => memo.original == player);
                BTL_DATA btl = unit.Data;
                unit.Trance = player.trance;
                btl_init.CopyPoints(btl.cur, player.cur);
                player.permanent_status = beforeMenu.playerPermanentStatus;
                BattleStatus statusesToRemove = unit.CurrentStatus & BattleStatusConst.OutOfBattle & ~player.status;
                btl_stat.RemoveStatuses(unit, statusesToRemove);
                if (player.cur.hp > 0 && unit.IsUnderAnyStatus(BattleStatus.Death))
                    btl_stat.RemoveStatus(unit, BattleStatusId.Death);
                else if (player.cur.hp == 0 && !unit.IsUnderAnyStatus(BattleStatus.Death))
                    btl_stat.AlterStatus(unit, BattleStatusId.Death);

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
                btl_stat.MakeStatusesPermanent(unit, oldPermanent & ~newPermanent, false);
                unit.ResistStatus &= ~(oldResist & ~newResist);
                btl_stat.MakeStatusesPermanent(unit, newPermanent & ~oldPermanent, true);
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

                if (beforeMenu.serialNo != player.info.serial_no)
                {
                    if (unit.IsMonsterTransform)
                        unit.ReleaseChangeToMonster();
                    BattlePlayerCharacter.CreatePlayer(btl, player);
                    btl_vfx.SetTranceModel(btl, unit.IsUnderAnyStatus(BattleStatus.Trance));
                }
                if (btl.weapon != ff9item.GetItemWeapon(player.equip[0]))
                {
                    btl_eqp.InitWeapon(player, btl);
                    btl.weaponModels[0].offset_rot = btl_mot.BattleParameterList[player.info.serial_no].GetWeaponRotationFixed(btl.weapon.ModelId, false);
                    SFX.InitBattleParty();
                }
                btl_eqp.InitEquipPrivilegeAttrib(player, btl);
            }
            if (newSwappedIn.Count != oldSwappedOut.Count)
                Log.Warning($"[BattleHUD] Error for character swap: {String.Join(", ", oldSwappedOut.ConvertAll(unit => unit.Name).ToArray())} -> {String.Join(", ", newSwappedIn.ConvertAll(charId => charId.ToString()).ToArray())}");
            for (Int32 i = 0; i < newSwappedIn.Count && i < oldSwappedOut.Count; i++)
            {
                BattleUnit swappedOut = oldSwappedOut[i];
                PLAYER swappedIn = FF9StateSystem.Common.FF9.player[newSwappedIn[i]];
                Boolean isSinglePlayer = _mainMenuSinglePlayer == swappedOut.Player;
                if (swappedOut.IsMonsterTransform)
                    swappedOut.ReleaseChangeToMonster();
                swappedOut.ResistStatus = 0;
                btl_stat.MakeStatusesPermanent(swappedOut, FF9BattleDB.AllStatuses, false);
                btl_stat.RemoveStatuses(swappedOut, FF9BattleDB.AllStatuses);
                btl_sys.DelCharacter(swappedOut);
                RemovePlayerFromAction(swappedOut.Id, true);
                btl_cmd.KillAllCommands(swappedOut);
                btl_init.SwapPlayerCharacter(swappedOut, swappedIn);
                AbilityPlayerDetail abilityPlayer = _abilityDetailDict[swappedOut.GetIndex()];
                abilityPlayer.Player = swappedIn;
                abilityPlayer.HasAp = ff9abil.FF9Abil_HasAp(swappedIn);
                abilityPlayer.AbilityPaList.Clear();
                abilityPlayer.AbilityMaxPaList.Clear();
                SetAbilityAp(abilityPlayer);
                if (isSinglePlayer)
                    _mainMenuSinglePlayer = swappedIn;
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
            PLAYER player = unit.Player;
            PlayerMemo beforeMenu = _mainMenuPlayerMemo.Find(memo => memo.original == player);
            if (beforeMenu != null)
                player.info.row = beforeMenu.row;
        }
        for (Int32 i = 0; i < 4; i++)
            party.member[i] = null;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
            if (unit.IsPlayer)
                party.member[unit.Position] = unit.Player;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateDeletedUnits())
            if (unit.IsPlayer)
                party.member[unit.Position] = unit.Player;
        if (_mainMenuSinglePlayer != null && Configuration.Battle.AccessMenus <= 1 && menuHadImpact)
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
        if (CommandPanel.activeSelf)
            DisplayCommand();
        if (AbilityPanel.activeSelf)
            DisplayAbility();
        if (ItemPanel.activeSelf)
            DisplayItem(CharacterCommands.Commands[_currentCommandId]);
    }
}
