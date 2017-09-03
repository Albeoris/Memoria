using Assets.Sources.Scripts.UI.Common;
using FF9;
using System;
using System.Collections.Generic;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Database;
using Memoria.Scenes;
using UnityEngine;

public partial class BattleHUD : UIScene
{
    private readonly Dictionary<Int32, AbilityPlayerDetail> _abilityDetailDict;
    private readonly MagicSwordCondition _magicSwordCond;
    private readonly List<ParameterStatus> _currentCharacterHp;
    private readonly List<Boolean> _currentEnemyDieState;
    private readonly List<DamageAnimationInfo> _hpInfoVal;
    private readonly List<DamageAnimationInfo> _mpInfoVal;
    private readonly Dictionary<Int32, CommandMenu> _commandCursorMemorize;
    private readonly Dictionary<Int32, Int32> _ability1CursorMemorize;
    private readonly Dictionary<Int32, Int32> _ability2CursorMemorize;
    private readonly Dictionary<Int32, Int32> _itemCursorMemorize;
    private readonly List<Int32> _matchBattleIdPlayerList;
    private readonly List<Int32> _matchBattleIdEnemyList;
    private readonly List<Int32> _itemIdList;

    private Single _lastFrameRightTriggerAxis;
    private Boolean _lastFramePressOnMenu;
    private Byte _currentLibraMessageNumber;
    private Byte _currentLibraMessageCount;
    private BattleUnit _libraBtlData;
    private Byte _currentPeepingMessageCount;
    private BattleEnemy _peepingEnmData;
    private Byte _currentMessagePriority;
    private Single _battleMessageCounter;
    private UI.PanelCommand _commandPanel;
    private Int32 _enemyCount;
    private Int32 _playerCount;
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
    private Boolean _commandEnable;
    private Boolean _beforePauseCommandEnable;
    private Boolean _isFromPause;
    private Boolean _isNeedToInit;
    private CommandMenu _currentCommandIndex;
    private UInt32 _currentCommandId;
    private String _currentButtonGroup;
    private Int32 _currentSubMenuIndex;
    private Int32 _currentTargetIndex;
    private List<Int32> _targetIndexList;
    private SubMenuType _subMenuType;
    private List<Int32> _unconsciousStateList;
    private Single _runCounter;
    private Boolean _hidingHud;
    private CursorGroup _cursorType;
    private Boolean _defaultTargetCursor;
    private Boolean _defaultTargetDead;
    private Boolean _targetDead;
    private TargetType _targetCursor;
    private Boolean _isTryingToRun;
    private Boolean _isAutoAttack;
    private Boolean _isAllTarget;
    private Byte _doubleCastCount;
    private CommandDetail _firstCommand;
    private Action _onResumeFromQuit;
    private Boolean _oneTime;
    private Int32 CurrentBattlePlayerIndex => _matchBattleIdPlayerList.IndexOf(CurrentPlayerIndex);

    private void DisplayBattleMessage(String str, Boolean isRect)
    {
        BattleDialogGameObject.SetActive(false);
        if (isRect)
        {
            _battleDialogWidget.width = (Int32)(128.0 * UIManager.ResourceXMultipier);
            _battleDialogWidget.height = 120;
            _battleDialogWidget.transform.localPosition = new Vector3(0.0f, 445f, 0.0f);
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

        Single additionalWidth = 0.0f;
        _battleDialogLabel.text = _battleDialogLabel.PhrasePreOpcodeSymbol(str, ref additionalWidth);
        BattleDialogGameObject.SetActive(true);
    }

    private void DisplayMessageLibra()
    {
        if (_libraBtlData == null)
            return;

        String str = String.Empty;
        if (_currentLibraMessageNumber == 1)
        {
            str = _libraBtlData.Name + FF9TextTool.BattleLibraText(10) + _libraBtlData.Level.ToString();
            _currentLibraMessageNumber = 2;
        }
        else if (_currentLibraMessageNumber == 2)
        {
            str = String.Format("{0}{1}{2}{3}{4}{5}{2}{6}",
                FF9TextTool.BattleLibraText(11),
                _libraBtlData.CurrentHp,
                FF9TextTool.BattleLibraText(13),
                _libraBtlData.MaximumHp,
                FF9TextTool.BattleLibraText(12),
                _libraBtlData.CurrentMp,
                _libraBtlData.MaximumMp);

            _currentLibraMessageCount = 0;
            _currentLibraMessageNumber = 3;
        }
        else if (_currentLibraMessageNumber == 3)
        {
            if (!_libraBtlData.IsPlayer)
            {
                Int32 enemyCategory = _libraBtlData.Enemy.Data.et.category;
                Int32 id;
                do
                {
                    Byte num2 = _currentLibraMessageCount++;
                    if ((id = num2) >= 8)
                        goto label_11;
                } while ((enemyCategory & 1 << id) == 0);
                SetBattleMessage(FF9TextTool.BattleLibraText(id), 2);
                return;
            }
            label_11:
            _currentLibraMessageCount = 0;
            _currentLibraMessageNumber = 4;
        }

        if (_currentLibraMessageNumber == 4)
        {
            EffectElement num1 = _libraBtlData.WeakElement & ~_libraBtlData.GuardElement;
            Int32 num2;
            do
            {
                Byte num3 = _currentLibraMessageCount++;
                if ((num2 = num3) >= 8)
                    goto label_17;
            } while ((num1 & (EffectElement)(1 << num2)) == 0);
            SetBattleMessage(Localization.GetSymbol() != "JP" ? str + FF9TextTool.BattleLibraText(14 + num2) : BtlGetAttrName(1 << num2) + FF9TextTool.BattleLibraText(14), 2);
            return;
            label_17:
            _currentLibraMessageCount = 0;
            _currentLibraMessageNumber = 5;
        }

        if (_currentLibraMessageNumber == 5)
        {
            _libraBtlData = null;
            _currentLibraMessageCount = 0;
            _currentLibraMessageNumber = 0;
        }
        else
        {
            SetBattleMessage(str, 2);
        }
    }

    private void DisplayMessagePeeping()
    {
        if (_peepingEnmData == null)
            return;

        Int32 id;
        do
        {
            Byte num1 = _currentPeepingMessageCount++;
            Int32 num2;
            if ((num2 = num1) < _peepingEnmData.StealableItems.Length + 1)
                id = _peepingEnmData.StealableItems[_peepingEnmData.StealableItems.Length - num2];
            else
                goto label_5;
        } while (id == Byte.MaxValue);

        SetBattleMessage(Localization.GetSymbol() != "JP"
            ? FF9TextTool.BattleLibraText(8) + FF9TextTool.ItemName(id)
            : FF9TextTool.ItemName(id) + FF9TextTool.BattleLibraText(8), 2);

        return;
        label_5:
        _peepingEnmData = null;
        _currentPeepingMessageCount = 0;
    }



    private void DisplayCommand()
    {
        BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
        CharacterPresetId presetId = FF9StateSystem.Common.FF9.party.GetCharacter(btl.Position).PresetId;
        command_tags command1;
        command_tags command2;

        if (btl.IsUnderStatus(BattleStatus.Trance))
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
            _commandPanel.Change.SetLabelText(FF9TextTool.CommandName(7));
            _commandPanel.Change.SetLabelColor(FF9TextTool.White);
            _commandPanel.Change.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription(7);
            _isManualTrance = false;
        }

        String command1Name = FF9TextTool.CommandName((Byte)command1);
        String command2Name = FF9TextTool.CommandName((Byte)command2);
        Boolean flag1 = command1 != 0;
        Boolean command2IsEnable = command2 != 0;

        if (command2 == command_tags.CMD_MAGIC_SWORD)
        {
            if (!_magicSwordCond.IsViviExist)
            {
                command2Name = String.Empty;
                command2IsEnable = false;
            }
            else if (_magicSwordCond.IsViviDead || _magicSwordCond.IsSteinerMini)
            {
                command2IsEnable = false;
            }
        }

        _commandPanel.Skill1.SetLabelText(command1Name);
        ButtonGroupState.SetButtonEnable(_commandPanel.Skill1, flag1);
        ButtonGroupState.SetButtonAnimation(_commandPanel.Skill1, flag1);

        if (flag1)
        {
            _commandPanel.Skill1.SetLabelColor(FF9TextTool.White);
            _commandPanel.Skill1.ButtonGroup.Help.Enable = true;
            _commandPanel.Skill1.ButtonGroup.Help.TextKey = String.Empty;
            _commandPanel.Skill1.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription((Byte)command1);
        }
        else
        {
            _commandPanel.Skill1.SetLabelColor(FF9TextTool.Gray);
            _commandPanel.Skill1.BoxCollider.enabled = false;
            _commandPanel.Skill1.ButtonGroup.Help.Enable = false;
        }

        _commandPanel.Skill2.SetLabelText(command2Name);
        ButtonGroupState.SetButtonEnable(_commandPanel.Skill2, command2IsEnable);
        ButtonGroupState.SetButtonAnimation(_commandPanel.Skill2, command2IsEnable);

        if (command2IsEnable)
        {
            _commandPanel.Skill2.SetLabelColor(FF9TextTool.White);
            _commandPanel.Skill2.ButtonGroup.Help.Enable = true;
            _commandPanel.Skill2.ButtonGroup.Help.TextKey = String.Empty;
            _commandPanel.Skill2.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription((Byte)command2);
        }
        else
        {
            _commandPanel.Skill2.SetLabelColor(FF9TextTool.Gray);
            _commandPanel.Skill2.BoxCollider.enabled = false;
            _commandPanel.Skill2.ButtonGroup.Help.Enable = false;
        }

        _commandPanel.Attack.SetLabelText(FF9TextTool.CommandName(1));
        _commandPanel.Defend.SetLabelText(FF9TextTool.CommandName(4));
        _commandPanel.Item.SetLabelText(FF9TextTool.CommandName(14));
        _commandPanel.Attack.ButtonGroup.Help.TextKey = String.Empty;
        _commandPanel.Attack.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription(1);
        _commandPanel.Defend.ButtonGroup.Help.TextKey = String.Empty;
        _commandPanel.Defend.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription(4);
        _commandPanel.Item.ButtonGroup.Help.TextKey = String.Empty;
        _commandPanel.Item.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription(14);
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
            switch (CheckHPState(player))
            {
                case ParameterStatus.Empty:
                    numberSubModeHud.SetColor(FF9TextTool.Red);
                    break;
                case ParameterStatus.Critical:
                    numberSubModeHud.SetColor(FF9TextTool.Yellow);
                    break;
                default:
                    numberSubModeHud.SetColor(FF9TextTool.White);
                    break;
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

            if (CheckMPState(player) == ParameterStatus.Empty)
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
                if (!player.IsUnderAnyStatus(status.Key))
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

    private IEnumerable<KnownUnit> EnumerateKnownPlayers()
    {
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;

            Int32 num = 0;
            while (1 << num != unit.Id)
                ++num;

            Int32 index = _matchBattleIdPlayerList.IndexOf(num);
            if (index >= 0)
                yield return new KnownUnit(index, unit);
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
            _currentMpValue = unit.CurrentMp;
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
            if (!isThrow)
            {
                if (!citem.YCITEM_IS_ITEM(ff9Item.id) || ff9Item.count <= 0)
                    continue;

                _itemIdList.Add(ff9Item.id);
                BattleItemListData battleItemListData = new BattleItemListData
                {
                    Count = ff9Item.count,
                    Id = ff9Item.id
                };
                inDataList.Add(battleItemListData);
            }
            else if (citem.YCITEM_IS_THROW(ff9Item.id) && ff9Item.count > 0)
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
            _itemIdList.Add(Byte.MaxValue);
            BattleItemListData battleItemListData = new BattleItemListData
            {
                Count = 0,
                Id = Byte.MaxValue
            };
            inDataList.Add(battleItemListData);
        }

        if (_itemScrollList.ItemsPool.Count == 0)
        {
            _itemScrollList.PopulateListItemWithData = DisplayItemDetail;
            RecycleListPopulator recycleListPopulator = _itemScrollList;
            BattleHUD battleHud = this;
            RecycleListPopulator.RecycleListItemClick recycleListItemClick = battleHud.OnListItemClick;
            recycleListPopulator.OnRecycleListItemClick += recycleListItemClick;
            _itemScrollList.InitTableView(inDataList, 0);
        }
        else
        {
            _itemScrollList.SetOriginalData(inDataList);
        }
    }


    private void DisplayAbility()
    {
        CharacterCommand ff9Command = CharacterCommands.Commands[_currentCommandId];
        SetAbilityAp(_abilityDetailDict[CurrentPlayerIndex]);
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();

        foreach (Byte id in ff9Command.Abilities)
        {
            BattleAbilityListData battleAbilityListData = new BattleAbilityListData {Id = id};
            inDataList.Add(battleAbilityListData);
        }

        if (_abilityScrollList.ItemsPool.Count == 0)
        {
            _abilityScrollList.PopulateListItemWithData = DisplayAbilityDetail;
            RecycleListPopulator recycleListPopulator = _abilityScrollList;
            BattleHUD battleHud = this;

            RecycleListPopulator.RecycleListItemClick recycleListItemClick = battleHud.OnListItemClick;
            recycleListPopulator.OnRecycleListItemClick += recycleListItemClick;
            _abilityScrollList.InitTableView(inDataList, 0);
        }
        else
        {
            _abilityScrollList.SetOriginalData(inDataList);
        }
    }

    private void DisplayAbilityDetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        BattleAbilityListData battleAbilityListData = (BattleAbilityListData)data;

        ItemListDetailHUD itemListDetailHud = new ItemListDetailHUD(item.gameObject);
        if (isInit)
            DisplayWindowBackground(item.gameObject, null);

        Int32 abilityId = battleAbilityListData.Id;
        AbilityStatus abilityState = GetAbilityState(abilityId);
        AA_DATA aaData = FF9StateSystem.Battle.FF9Battle.aa_data[abilityId];

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
            itemListDetailHud.NameLabel.text = FF9TextTool.ActionAbilityName(abilityId);
            Int32 mp = GetActionMpCost(aaData);
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

            itemListDetailHud.Button.Help.TextKey = String.Empty;
            itemListDetailHud.Button.Help.Text = FF9TextTool.ActionAbilityHelpDescription(abilityId);
        }
    }

    private void DisplayTarget()
    {
        Boolean flag1 = false;
        Int32 enCount = _enemyCount;
        Int32 plCount = _playerCount;
        Int32 enemyCount = 0;
        Int32 playerCount = 0;

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (unit.IsPlayer)
            {
                if (unit.IsSelected)
                    playerCount++;
            }
            else
            {
                enemyCount++;
            }
        }

        if (enemyCount != enCount || playerCount != plCount)
        {
            flag1 = true;
            _matchBattleIdPlayerList.Clear();
            _currentCharacterHp.Clear();
            _matchBattleIdEnemyList.Clear();
            _currentEnemyDieState.Clear();
            _enemyCount = enemyCount;
            _playerCount = playerCount;
        }

        Int32 index1 = 0;
        Int32 index2 = 0;

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (unit.Id == 0 || !unit.IsSelected)
                continue;

            Int32 unitIndex = unit.GetIndex();

            if (unit.IsPlayer)
            {
                ParameterStatus parameterStatus = CheckHPState(unit);
                if (index1 >= _currentCharacterHp.Count)
                {
                    _currentCharacterHp.Add(parameterStatus);
                    _matchBattleIdPlayerList.Add(unitIndex);
                    flag1 = true;
                }
                else if (parameterStatus != _currentCharacterHp[index1])
                {
                    _currentCharacterHp[index1] = parameterStatus;
                    flag1 = true;
                }
                ++index1;
            }
            else
            {
                Boolean isDead = unit.IsUnderStatus(BattleStatus.Death);
                if (index2 >= _currentEnemyDieState.Count)
                {
                    _currentEnemyDieState.Add(isDead);
                    _matchBattleIdEnemyList.Add(unitIndex);
                    flag1 = true;
                }
                else if (isDead != _currentEnemyDieState[index2])
                {
                    _currentEnemyDieState[index2] = isDead;
                    flag1 = true;
                }
                ++index2;
            }
        }

        if (!flag1)
            return;

        foreach (GONavigationButton current in _targetPanel.EnumerateTargets())
        {
            current.KeyNavigation.startsSelected = false;
            current.IsActive = false;
        }

        GameObject obj = null;
        Int32 playerIndex = 0;
        Int32 enemyIndex = 0;
        if (_cursorType == CursorGroup.Individual)
            obj = ButtonGroupState.ActiveButton;

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (unit.Id == 0 || !unit.IsSelected)
                continue;

            if (unit.IsPlayer)
            {
                GONavigationButton targetHud = _targetPanel.Players[playerIndex];
                GameObject go = targetHud.GameObject;
                UILabel uiLabel = targetHud.Name.Label;
                go.SetActive(true);
                uiLabel.text = unit.Player.Name;
                if (_currentCharacterHp[playerIndex] == ParameterStatus.Empty)
                {
                    if (_cursorType == CursorGroup.Individual)
                    {
                        if (_targetDead == false)
                        {
                            ButtonGroupState.SetButtonEnable(go, false);
                            if (go == obj)
                            {
                                Int32 firstPlayer = GetFirstAlivePlayerIndex();
                                if (firstPlayer != -1)
                                {
                                    _currentTargetIndex = firstPlayer;
                                    obj = _targetPanel.Players[firstPlayer].GameObject;
                                }
                                else
                                {
                                    Debug.LogError("NO player active !!");
                                }
                                Singleton<PointerManager>.Instance.RemovePointerFromGameObject(go);
                            }
                        }
                        else
                            ButtonGroupState.SetButtonEnable(go, true);
                    }
                    uiLabel.color = FF9TextTool.Red;
                }
                else if (_currentCharacterHp[playerIndex] == ParameterStatus.Critical)
                {
                    if (_cursorType == CursorGroup.Individual)
                        ButtonGroupState.SetButtonEnable(go, true);
                    uiLabel.color = FF9TextTool.Yellow;
                }
                else
                {
                    if (_cursorType == CursorGroup.Individual)
                        ButtonGroupState.SetButtonEnable(go, true);
                    uiLabel.color = FF9TextTool.White;
                }
                ++playerIndex;
            }
            else
            {
                GONavigationButton targetHud = _targetPanel.Enemies[enemyIndex];
                GameObject go = targetHud.GameObject;
                UILabel uiLabel = targetHud.Name.Label;
                Single additionalWidth = 0.0f;
                go.SetActive(true);
                uiLabel.text = uiLabel.PhrasePreOpcodeSymbol(unit.Enemy.Name, ref additionalWidth);
                if (_currentEnemyDieState[enemyIndex])
                {
                    if (_cursorType == CursorGroup.Individual)
                    {
                        ButtonGroupState.SetButtonEnable(go, false);
                        if (_targetDead == false)
                        {
                            if (go == obj)
                            {
                                Int32 enemyIndexEx = GetFirstAliveEnemyIndex() + HonoluluBattleMain.EnemyStartIndex;
                                if (enemyIndexEx != -1)
                                {
                                    if (_currentCommandIndex == CommandMenu.Attack && FF9StateSystem.PCPlatform && _enemyCount > 1)
                                    {
                                        Int32 nextEnemyIndexEx = _currentTargetIndex == enemyIndexEx ? enemyIndexEx + 1 : enemyIndexEx;
                                        Int32 firstIndex = nextEnemyIndexEx >= _targetPanel.AllTargets.Length ? enemyIndexEx : nextEnemyIndexEx;
                                        ValidateDefaultTarget(ref firstIndex);
                                        enemyIndexEx = firstIndex;
                                    }
                                    _currentTargetIndex = enemyIndexEx;
                                    obj = _targetPanel.AllTargets[enemyIndexEx].GameObject;
                                }
                                else
                                    Debug.LogError("NO enemy active !!");
                                Singleton<PointerManager>.Instance.RemovePointerFromGameObject(go);
                            }
                        }
                        else
                            ButtonGroupState.SetButtonEnable(go, true);
                    }
                    uiLabel.color = FF9TextTool.Gray;
                }
                else
                {
                    if (_cursorType == CursorGroup.Individual)
                        ButtonGroupState.SetButtonEnable(go, true);
                    uiLabel.color = FF9TextTool.White;
                }
                ++enemyIndex;
            }
        }

        if ((enCount != enemyCount || plCount != playerCount) && ButtonGroupState.ActiveGroup == TargetGroupButton)
        {
            SetTargetDefault();
            modelButtonManager.Reset();
            EnableTargetArea();
            SetTargetHelp();
            ButtonGroupState.DisableAllGroup(true);
            ButtonGroupState.ActiveGroup = TargetGroupButton;
        }

        if (obj != null && _cursorType == CursorGroup.Individual && obj.activeSelf)
            ButtonGroupState.ActiveButton = obj;
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
            case ParameterStatus.Empty:
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

        if (bd.IsUnderAnyStatus(BattleStatus.Slow | BattleStatus.Stop))
            spriteName = ATEGray;
        else if (bd.IsUnderAnyStatus(BattleStatus.Haste))
            spriteName = ATEOrange;

        playerHud.ATBBar.Foreground.Foreground.Sprite.spriteName = spriteName;
        if (!bd.HasTrance)
            return;

        playerHud.TranceBar.SetProgress(bd.Trance / 256f);
        if (parameterStatus == ParameterStatus.Empty)
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
            AbilityPlayerDetail abilityPlayer = new AbilityPlayerDetail {Player = unit.Player};
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
        if (bd.CurrentHp == 0)
            return ParameterStatus.Empty;

        if (bd.IsPlayer)
        {
            if (bd.CurrentHp <= bd.MaximumHp / 6.0)
                return ParameterStatus.Critical;
        }

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
            infoVal1.RequiredValue = infoVal1.CurrentValue = unit.CurrentHp;
            infoVal2.RequiredValue = infoVal2.CurrentValue = unit.CurrentMp;
            infoVal1.FrameLeft = infoVal2.FrameLeft = 0;
            infoVal1.IncrementStep = infoVal2.IncrementStep = 0;
            _hpInfoVal.Add(infoVal1);
            _mpInfoVal.Add(infoVal2);
        }
    }

    private Int32 GetActionMpCost(AA_DATA aaData)
    {
        Int32 num = aaData.MP;
        BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
        CharacterIndex slotId = FF9StateSystem.Common.FF9.party.GetCharacter(unit.Position).Index;
        if ((aaData.Type & 4) != 0 && FF9StateSystem.EventState.gEventGlobal[18] != 0)
            num <<= 2;

        if (ff9abil.FF9Abil_GetEnableSA(slotId, AbilSaMpHalf))
            num >>= 1;

        return num;
    }

    private AbilityStatus GetAbilityState(Int32 abilId)
    {
        AbilityPlayerDetail abilityPlayerDetail = _abilityDetailDict[CurrentPlayerIndex];
        BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
        AA_DATA aaData = FF9StateSystem.Battle.FF9Battle.aa_data[abilId];

        if (abilityPlayerDetail.HasAp && !abilityPlayerDetail.AbilityEquipList.ContainsKey(abilId) && (!abilityPlayerDetail.AbilityPaList.ContainsKey(abilId) || abilityPlayerDetail.AbilityPaList[abilId] < abilityPlayerDetail.AbilityMaxPaList[abilId]))
            return AbilityStatus.None;

        if ((aaData.Category & 2) != 0)
        {
            if (FF9StateSystem.Battle.FF9Battle.btl_scene.Info.NoMagical)
                return AbilityStatus.Disable;

            if (unit.IsUnderAnyStatus(BattleStatus.Silence))
                return AbilityStatus.Disable;
        }

        if (GetActionMpCost(aaData) > unit.CurrentMp)
            return AbilityStatus.Disable;

        return AbilityStatus.Enable;
    }

    private void SetAbilityAp(AbilityPlayerDetail abilityPlayer)
    {
        Character player = abilityPlayer.Player;
        if (!abilityPlayer.HasAp)
            return;

        CharacterAbility[] paDataArray = ff9abil._FF9Abil_PaData[player.PresetId];
        for (Int32 abilId = 0; abilId < 192; ++abilId)
        {
            Int32 index;
            if ((index = ff9abil.FF9Abil_GetIndex(player.Index, abilId)) < 0)
                continue;

            abilityPlayer.AbilityPaList[abilId] = player.Data.pa[index];
            abilityPlayer.AbilityMaxPaList[abilId] = paDataArray[index].Ap;
        }
    }

    private static void SetAbilityEquip(AbilityPlayerDetail abilityPlayer)
    {
        Character player = abilityPlayer.Player;
        for (Int32 index1 = 0; index1 < 5; ++index1)
        {
            Int32 index2 = player.Equipment[index1];
            if (index2 == Byte.MaxValue)
                continue;

            FF9ITEM_DATA ff9ItemData = ff9item._FF9Item_Data[index2];
            for (Int32 index3 = 0; index3 < 3; ++index3)
            {
                Int32 index4 = ff9ItemData.ability[index3];
                if (index4 != 0 && 192 > index4)
                    abilityPlayer.AbilityEquipList[index4] = true;
            }
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
            command_tags normalCommandId = CharacterCommands.CommandSets[presetId].GetRegular(k);
            command_tags tranceCommandId = CharacterCommands.CommandSets[presetId].GetTrance(k);
            if (normalCommandId == tranceCommandId)
                continue;

            CharacterCommand normalCommand = CharacterCommands.Commands[(Byte)normalCommandId];
            CharacterCommand tranceCommand = CharacterCommands.Commands[(Byte)tranceCommandId];
            Int32 count = Math.Min(normalCommand.Abilities.Length, tranceCommand.Abilities.Length);
            for (Int32 i = 0; i < count; ++i)
            {
                Int32 normalId = normalCommand.Abilities[i];
                Int32 tranceId = tranceCommand.Abilities[i];
                if (normalId == tranceId)
                    continue;

                if (abilityPlayer.AbilityPaList.ContainsKey(normalId))
                {
                    abilityPlayer.AbilityPaList[tranceId] = abilityPlayer.AbilityPaList[normalId];
                    abilityPlayer.AbilityMaxPaList[tranceId] = abilityPlayer.AbilityMaxPaList[normalId];
                }

                if (abilityPlayer.AbilityEquipList.ContainsKey(normalId))
                    abilityPlayer.AbilityEquipList[tranceId] = abilityPlayer.AbilityEquipList[normalId];
            }
        }
    }

    private static void SetAbilityMagic(AbilityPlayerDetail abilityPlayer)
    {
        Character character = abilityPlayer.Player;
        if (character.Index != CharacterIndex.Steiner)
            return;

        CharacterCommand magicSwordCommand = CharacterCommands.Commands[(Int32)command_tags.CMD_MAGIC_SWORD];
        PLAYER player2 = FF9StateSystem.Common.FF9.player[CharacterPresetId.Vivi];
        CharacterAbility[] paDataArray = ff9abil._FF9Abil_PaData[CharacterPresetId.Vivi];
        Int32[] abilities = {25, 26, 27, 29, 30, 31, 33, 34, 35, 38, 45, 47, 48}; // TODO: Move to the resource file

        Int32 count = Math.Min(magicSwordCommand.Abilities.Length, abilities.Length);
        for (Int32 i = 0; i < count; ++i)
        {
            Int32 abilityId = magicSwordCommand.Abilities[i];
            Int32 index = ff9abil.FF9Abil_GetIndex(1, abilities[i]);
            if (index > -1)
            {
                abilityPlayer.AbilityPaList[abilityId] = player2.pa[index];
                abilityPlayer.AbilityMaxPaList[abilityId] = paDataArray[index].Ap;
            }
        }

        for (Int32 equipSlot = 0; equipSlot < 5; ++equipSlot)
        {
            Int32 equipId = player2.equip[equipSlot];
            if (equipId == Byte.MaxValue)
                continue;

            FF9ITEM_DATA ff9ItemData = ff9item._FF9Item_Data[equipId];
            for (Int32 equipAbilitySlot = 0; equipAbilitySlot < 3; ++equipAbilitySlot)
            {
                Int32 equipAbilityId = ff9ItemData.ability[equipAbilitySlot];
                if (equipAbilityId == 0 || equipAbilityId >= 192)
                    continue;

                for (Int32 i = 0; i < count; ++i)
                {
                    if (equipAbilityId != abilities[i])
                        continue;

                    Int32 abilityId = magicSwordCommand.Abilities[i];
                    abilityPlayer.AbilityEquipList[abilityId] = true;
                }
            }
        }
    }

    private void OnTargetNavigate(GameObject go, KeyCode key)
    {
        if (_cursorType == CursorGroup.AllEnemy)
        {
            if (_targetCursor != TargetType.ManyAny || key != KeyCode.RightArrow)
                return;
            FF9Sfx.FF9SFX_Play(103);
            _cursorType = CursorGroup.AllPlayer;
            DisplayTargetPointer();
        }
        else
        {
            if (_cursorType != CursorGroup.AllPlayer || _targetCursor != TargetType.ManyAny || key != KeyCode.LeftArrow)
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

        _partyDetail.SetBlink(false);

        AutoBattleHud.SetActive(_isAutoAttack);
        Singleton<HUDMessage>.Instance.WorldCamera = PersistenSingleton<UIManager>.Instance.BattleCamera;
        modelButtonManager.WorldCamera = PersistenSingleton<UIManager>.Instance.BattleCamera;
        ManageAbility();
        InitHpMp();

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            Int32 index = unit.GetIndex();

            if (unit.IsSelected)
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



    private CommandDetail ProcessCommand(Int32 target, CursorGroup cursor)
    {
        CommandDetail commandDetail = new CommandDetail
        {
            CommandId = _currentCommandId,
            SubId = 0U
        };

        CharacterCommandType commandType = CharacterCommands.Commands[commandDetail.CommandId].Type;
        if (commandType == CharacterCommandType.Normal)
            commandDetail.SubId = CharacterCommands.Commands[commandDetail.CommandId].Ability;

        if (commandType == CharacterCommandType.Ability)
        {
            Int32 abilityId = CharacterCommands.Commands[commandDetail.CommandId].Abilities[_currentSubMenuIndex];
            commandDetail.SubId = (UInt32)PatchAbility(abilityId);
        }
        else if (commandType == CharacterCommandType.Item || commandType == CharacterCommandType.Throw)
        {
            Int32 num2 = _itemIdList[_currentSubMenuIndex];
            commandDetail.SubId = (UInt32)num2;
        }

        commandDetail.TargetId = 0;

        switch (cursor)
        {
            case CursorGroup.Individual:
                commandDetail.TargetId = (UInt16)(1 << target);
                break;
            case CursorGroup.AllPlayer:
                commandDetail.TargetId = 15;
                break;
            case CursorGroup.AllEnemy:
                commandDetail.TargetId = 240;
                break;
            case CursorGroup.All:
                commandDetail.TargetId = Byte.MaxValue;
                break;
        }

        commandDetail.TargetType = (UInt32)GetSelectMode(cursor);
        return commandDetail;
    }

    private void SendCommand(CommandDetail command)
    {
        CMD_DATA cmd = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].cmd[0];
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
            _commandCursorMemorize[CurrentPlayerIndex] = _currentCommandIndex;
            _commandPanel.IsActive = false;
            return;
        }

        GameObject commandObject = _commandPanel.Attack;

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
            if (_itemCursorMemorize.ContainsKey(CurrentPlayerIndex) && (Int64)FF9StateSystem.Settings.cfg.cursor != 0L || forceCursorMemo)
                _itemScrollList.JumpToIndex(_itemCursorMemorize[CurrentPlayerIndex], true);
            else
                _itemScrollList.JumpToIndex(0, false);
            ButtonGroupState.RemoveCursorMemorize(ItemGroupButton);
            ButtonGroupState.ActiveGroup = ItemGroupButton;
        }
        else
        {
            if (_currentCommandIndex == CommandMenu.Item && _currentSubMenuIndex != -1)
                _itemCursorMemorize[CurrentPlayerIndex] = _currentSubMenuIndex;
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
                Dictionary<Int32, Int32> dictionary = _currentCommandIndex != CommandMenu.Ability1 ? _ability2CursorMemorize : _ability1CursorMemorize;
                ButtonGroupState.RemoveCursorMemorize(AbilityGroupButton);
                if (dictionary.ContainsKey(CurrentPlayerIndex) && (Int64)FF9StateSystem.Settings.cfg.cursor != 0L || forceCursorMemo)
                    _abilityScrollList.JumpToIndex(dictionary[CurrentPlayerIndex], true);
                else
                    _abilityScrollList.JumpToIndex(0, true);
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
            if (_currentCommandIndex == CommandMenu.Ability1 && _currentSubMenuIndex != -1)
                _ability1CursorMemorize[CurrentPlayerIndex] = _currentSubMenuIndex;
            else if (_currentCommandIndex == CommandMenu.Ability2 && _currentSubMenuIndex != -1)
                _ability2CursorMemorize[CurrentPlayerIndex] = _currentSubMenuIndex;
            AbilityPanel.SetActive(false);
        }
    }

    private void SetTargetVisibility(Boolean isVisible)
    {
        if (isVisible)
        {
            TargetType cursor = 0;
            TargetDisplay subMode = 0;
            _defaultTargetCursor = false;
            _defaultTargetDead = false;
            _targetDead = false;
            if (_currentCommandIndex == CommandMenu.Ability1 || _currentCommandIndex == CommandMenu.Ability2)
            {
                CharacterCommand ff9Command = CharacterCommands.Commands[_currentCommandId];
                AA_DATA aaData = FF9StateSystem.Battle.FF9Battle.aa_data[ff9Command.Type != CharacterCommandType.Ability ? ff9Command.Ability : ff9Command.Abilities[_currentSubMenuIndex]];
                cursor = aaData.Info.Target;
                _defaultTargetCursor = aaData.Info.DefaultAlly;
                _defaultTargetDead = aaData.Info.DefaultOnDead;
                _targetDead = aaData.Info.ForDead;
                subMode = aaData.Info.DisplayStats;
            }
            else if (_currentCommandIndex != CommandMenu.Attack && _currentCommandIndex == CommandMenu.Item)
            {
                ITEM_DATA itemData = ff9item._FF9Item_Info[_itemIdList[_currentSubMenuIndex] - 224];
                cursor = itemData.info.Target;
                _defaultTargetCursor = itemData.info.DefaultAlly;
                _defaultTargetDead = itemData.info.ForDead;
                _targetDead = itemData.info.ForDead;
                subMode = itemData.info.DisplayStats;
            }
            _isAllTarget = false;
            TargetPanel.SetActive(true);
            EnableTargetArea();
            DisplayTarget();
            DisplayStatus(subMode);
            SetTargetAvalability(cursor);
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

    private void SetTargetAvalability(TargetType cursor)
    {
        _targetCursor = cursor;
        if (cursor == TargetType.SingleAny)
        {
            _cursorType = CursorGroup.Individual;

            ChangeTargetAvalability(player: true, enemy: true, all: false, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.SingleEnemy)
        {
            _cursorType = CursorGroup.Individual;

            ChangeTargetAvalability(player: false, enemy: true, all: false, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.SingleAlly)
        {
            _cursorType = CursorGroup.Individual;

            ChangeTargetAvalability(player: true, enemy: false, all: false, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.ManyAny)
        {
            ChangeTargetAvalability(player: true, enemy: true, all: FF9StateSystem.MobilePlatform, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.ManyEnemy)
        {
            ChangeTargetAvalability(player: false, enemy: true, all: FF9StateSystem.MobilePlatform, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.ManyAlly)
        {
            ChangeTargetAvalability(player: true, enemy: false, all: FF9StateSystem.MobilePlatform, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.AllEnemy || cursor == TargetType.RandomEnemy)
        {
            _cursorType = CursorGroup.AllEnemy;

            ChangeTargetAvalability(player: false, enemy: true, all: false, allPlayers: false, allEnemies: true);

            _isAllTarget = true;
        }
        else if (cursor == TargetType.AllAlly || cursor == TargetType.RandomAlly)
        {
            _cursorType = CursorGroup.AllPlayer;

            ChangeTargetAvalability(player: true, enemy: false, all: false, allPlayers: true, allEnemies: false);

            _isAllTarget = true;
        }
        else if (cursor == TargetType.All || cursor == TargetType.Everyone || cursor == TargetType.Random)
        {
            _cursorType = CursorGroup.All;

            ChangeTargetAvalability(player: true, enemy: true, all: false, allPlayers: true, allEnemies: true);

            _isAllTarget = true;
        }
        else if (cursor == TargetType.Self)
        {
            _cursorType = CursorGroup.Individual;

            ChangeTargetAvalability(player: false, enemy: false, all: false, allPlayers: false, allEnemies: false);

            GONavigationButton currentPlayer = _targetPanel.Players[CurrentBattlePlayerIndex];
            ButtonGroupState.SetButtonEnable(currentPlayer, true);
        }
    }

    private void ChangeTargetAvalability(Boolean player, Boolean enemy, Boolean all, Boolean allPlayers, Boolean allEnemies)
    {
        foreach (GONavigationButton button in _targetPanel.Players.Entries)
            ButtonGroupState.SetButtonEnable(button, player);

        foreach (GONavigationButton button in _targetPanel.Enemies.Entries)
            ButtonGroupState.SetButtonEnable(button, enemy);

        AllTargetButton.SetActive(all);
        _targetPanel.Buttons.Player.IsActive = allPlayers;
        _targetPanel.Buttons.Enemy.IsActive = allEnemies;
    }

    private void SetTargetDefault()
    {
        if (_targetDead == false)
        {
            Int32 playerIndex = 0;
            Int32 enemyIndex = 0;

            foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
            {
                if (unit.Id == 0 || !unit.IsSelected)
                    continue;

                if (unit.IsPlayer)
                {
                    if (unit.IsUnderAnyStatus(BattleStatus.Death))
                        ButtonGroupState.SetButtonEnable(_targetPanel.Players[playerIndex], false);
                    playerIndex++;
                }
                else
                {
                    if (unit.IsUnderAnyStatus(BattleStatus.Death))
                        ButtonGroupState.SetButtonEnable(_targetPanel.Enemies[enemyIndex], false);
                    enemyIndex++;
                }
            }
        }

        if (_targetCursor == TargetType.SingleAny
            || _targetCursor == TargetType.SingleAlly
            || _targetCursor == TargetType.SingleEnemy
            || _targetCursor == TargetType.ManyAny
            || _targetCursor == TargetType.ManyAlly
            || _targetCursor == TargetType.ManyEnemy)
        {
            if (_defaultTargetCursor)
            {
                if (_defaultTargetDead)
                {
                    UInt16 deadPlayerIndex = GetDeadOrCurrentPlayer(true);
                    GONavigationButton deadPlayer = _targetPanel.Players[deadPlayerIndex];
                    ButtonGroupState.SetCursorStartSelect(deadPlayer, TargetGroupButton);
                }
                else
                {
                    GONavigationButton currentPlayer = _targetPanel.Players[CurrentBattlePlayerIndex];
                    ButtonGroupState.SetCursorStartSelect(currentPlayer, TargetGroupButton);
                }

                _currentTargetIndex = 0;
                ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
            }
            else
            {
                //int num = HonoluluBattleMain.EnemyStartIndex;
                Int32 firstIndex;
                if (_defaultTargetDead)
                {
                    firstIndex = GetDeadOrCurrentPlayer(false);
                    GONavigationButton deadPlayer = _targetPanel.AllTargets[firstIndex];
                    ButtonGroupState.SetCursorStartSelect(deadPlayer, TargetGroupButton);
                }
                else
                {
                    firstIndex = GetFirstAliveEnemyIndex() + HonoluluBattleMain.EnemyStartIndex;
                    if (firstIndex != -1)
                    {
                        if (_currentCommandIndex == CommandMenu.Attack && FF9StateSystem.PCPlatform)
                            ValidateDefaultTarget(ref firstIndex);
                        GONavigationButton currentEnemy = _targetPanel.AllTargets[firstIndex];
                        ButtonGroupState.SetCursorStartSelect(currentEnemy, TargetGroupButton);
                    }
                }
                _currentTargetIndex = firstIndex;
                ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
            }
        }
        else if (_targetCursor == TargetType.Self)
        {
            Int32 currentPlayerIndex = CurrentBattlePlayerIndex;
            GONavigationButton currentPlayer = _targetPanel.Players[currentPlayerIndex];
            ButtonGroupState.SetCursorStartSelect(currentPlayer, TargetGroupButton);
            _currentTargetIndex = currentPlayerIndex;
            ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
        }
    }

    private void SetTargetHelp()
    {
        String str1 = String.Empty;
        Boolean flag = (Int32)_targetCursor < 6 || (Int32)_targetCursor > 12;
        switch (_targetCursor)
        {
            case TargetType.SingleAny:
                str1 = Localization.Get("BattleTargetHelpIndividual");
                break;
            case TargetType.SingleAlly:
                str1 = Localization.Get("BattleTargetHelpIndividualPC");
                break;
            case TargetType.SingleEnemy:
                str1 = Localization.Get("BattleTargetHelpIndividualNPC");
                break;
            case TargetType.ManyAny:
                str1 = Localization.Get("BattleTargetHelpMultiS");
                break;
            case TargetType.ManyAlly:
                str1 = Localization.Get("BattleTargetHelpMultiPCS");
                break;
            case TargetType.ManyEnemy:
                str1 = Localization.Get("BattleTargetHelpMultiNPCS");
                break;
            case TargetType.All:
                str1 = Localization.Get("BattleTargetHelpAll");
                break;
            case TargetType.AllAlly:
                str1 = Localization.Get("BattleTargetHelpAllPC");
                break;
            case TargetType.AllEnemy:
                str1 = Localization.Get("BattleTargetHelpAllNPC");
                break;
            case TargetType.Random:
                str1 = Localization.Get("BattleTargetHelpRand");
                break;
            case TargetType.RandomAlly:
                str1 = Localization.Get("BattleTargetHelpRandPC");
                break;
            case TargetType.RandomEnemy:
                str1 = Localization.Get("BattleTargetHelpRandNPC");
                break;
            case TargetType.Everyone:
                str1 = Localization.Get("BattleTargetHelpWhole");
                break;
            case TargetType.Self:
                str1 = Localization.Get("BattleTargetHelpSelf");
                break;
        }

        if (_isAllTarget)
        {
            flag = false;
            switch (_targetCursor)
            {
                case TargetType.ManyAny:
                    str1 = Localization.Get("BattleTargetHelpMultiM");
                    break;
                case TargetType.ManyAlly:
                    str1 = Localization.Get("BattleTargetHelpMultiPCM");
                    break;
                case TargetType.ManyEnemy:
                    str1 = Localization.Get("BattleTargetHelpMultiNPCM");
                    break;
            }
        }

        Int32 playerIndex = 0;
        Int32 enemyIndex = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (unit.Id == 0 || !unit.IsSelected)
                continue;

            if (unit.IsPlayer)
            {
                GONavigationButton targetHud = _targetPanel.Players[playerIndex];
                String str2 = !flag ? String.Empty : unit.Player.Name;
                targetHud.ButtonGroup.Help.Enable = true;
                targetHud.ButtonGroup.Help.Text = str1 + "\n" + str2;
                ++playerIndex;
            }
            else
            {
                GONavigationButton targetHud = _targetPanel.Enemies[enemyIndex];
                Single additionalWidth = 0.0f;
                String str2 = !flag ? String.Empty : Singleton<HelpDialog>.Instance.PhraseLabel.PhrasePreOpcodeSymbol(unit.Enemy.Name, ref additionalWidth);
                targetHud.ButtonGroup.Help.Enable = true;
                targetHud.ButtonGroup.Help.Text = str1 + "\n" + str2;
                ++enemyIndex;
            }
        }
    }

    private void SetHelpMessageVisibility(Boolean active)
    {
        if (!ButtonGroupState.HelpEnabled)
            return;
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


    private Int32 PatchAbility(Int32 id)
    {
        if (AbilCarbuncle == id)
        {
            BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
            Character character = FF9StateSystem.Common.FF9.party.GetCharacter(unit.Position);
            switch (character.Equipment.Accessory)
            {
                case 228:
                    id++;
                    break;
                case 229:
                    id += 2;
                    break;
                case 227:
                    id += 3;
                    break;
            }
        }
        else if (AbilFenril == id)
        {
            BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
            Character character = FF9StateSystem.Common.FF9.party.GetCharacter(unit.Position);
            if (character.Equipment.Accessory == 222)
                id++;
        }
        return id;
    }

    private UInt16 GetDeadOrCurrentPlayer(Boolean player)
    {
        UInt16 index = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsSelected || !unit.IsUnderStatus(BattleStatus.Death))
                continue;

            if (player && unit.IsPlayer || !player && !unit.IsPlayer)
                return index;

            index++;
        }
        return (UInt16)CurrentBattlePlayerIndex;
    }


    private static Boolean IsEnableInput(BattleUnit unit)
    {
        return unit != null && unit.CurrentHp != 0 && !unit.IsUnderAnyStatus((BattleStatus)1107434755U) && (battle.btl_bonus.member_flag & 1 << unit.Position) != 0;
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
            if (!unit.IsSelected)
                continue;

            Int32 index = unit.GetIndex();

            modelButtonManager.Show(
                unit.Data.gameObject.transform.GetChildByName("bone" + unit.Data.tar_bone.ToString("D3")).gameObject.transform,
                index,
                !unit.IsPlayer,
                unit.Data.radius,
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
                List<GameObject> goList = new List<GameObject>();
                for (Int32 index = 0; index < _playerCount; ++index)
                {
                    if (_currentCharacterHp[index] != ParameterStatus.Empty || _targetDead)
                        goList.Add(_targetPanel.Players[index]);
                }
                ButtonGroupState.SetMultipleTarget(goList, true);
            }
            else if (_cursorType == CursorGroup.AllEnemy)
            {
                List<GameObject> goList = new List<GameObject>();
                for (Int32 index = 0; index < _enemyCount; ++index)
                {
                    if (!_currentEnemyDieState[index] || _targetDead)
                        goList.Add(_targetPanel.Enemies[index]);
                }
                ButtonGroupState.SetMultipleTarget(goList, true);
            }
            else
                ButtonGroupState.SetAllTarget(true);
        }
    }


    private void SetTarget(Int32 battleIndex)
    {
        const Byte saveTheQueenId = (Byte)WeaponItem.SaveTheQueen;

        if (_currentCommandId == 1 && CurrentBattlePlayerIndex > -1)
        {
            BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
            if (btl.PlayerIndex == CharacterIndex.Beatrix)
            {
                Character player = btl.Player;
                Byte weapon = player.Equipment.Weapon;
                Byte accessory = player.Equipment.Accessory;
                if (weapon == saveTheQueenId && accessory == saveTheQueenId)
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

        if (battleItemListData.Id == Byte.MaxValue)
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
}