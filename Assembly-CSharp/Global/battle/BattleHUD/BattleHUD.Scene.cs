using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Database;
using Memoria.Scenes;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public partial class BattleHUD : UIScene
{
    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        SceneVoidDelegate afterShowAction = GeneratedShow;
        if (afterFinished != null)
            afterShowAction += afterFinished;

        if (!_isFromPause)
        {
            base.Show(afterShowAction);
            PersistenSingleton<UIManager>.Instance.Booster.SetBoosterState(PersistenSingleton<UIManager>.Instance.UnityScene);
            FF9StateSystem.Settings.SetMasterSkill();
            this._doubleCastCount = 0;
            AllMenuPanel.SetActive(false);
        }
        else
        {
            _commandEnable = _beforePauseCommandEnable;
            _isTryingToRun = false;
            Singleton<HUDMessage>.Instance.Pause(false);
            base.Show(afterShowAction);
            if (_commandEnable && !_hidingHud)
            {
                FF9BMenu_EnableMenu(true);
                ButtonGroupState.ActiveGroup = _currentButtonGroup;
                DisplayTargetPointer();
            }
        }
        if (!_isFromPause || _usingMainMenu)
            UpdateSprites();

        if (_usingMainMenu)
        {
            try
            {
                UpdateBattleAfterMainMenu();
                btl2d.ShowMessages(true);
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
        }

        _isFromPause = false;
        _oneTime = true;
    }

    public override void Hide(SceneVoidDelegate afterFinished = null)
    {
        base.Hide(afterFinished);
        PauseButtonGameObject.SetActive(false);
        HelpButtonGameObject.SetActive(false);
        if ((!_isFromPause || _usingMainMenu) && UIControlPanel != null && UIControlPanel.Show)
            UIControlPanel.Show = false;

        if (!_isFromPause)
            RemoveCursorMemorize();
    }

    public void OnLocalize()
    {
        if (!isActiveAndEnabled)
            return;
        if (_subMenuType == SubMenuType.Ability && AbilityPanel.activeSelf)
            _abilityScrollList.UpdateTableViewImp();
        if ((_subMenuType == SubMenuType.Item || _subMenuType == SubMenuType.Throw) && ItemPanel.activeSelf)
            _itemScrollList.UpdateTableViewImp();
        if (CommandPanel.activeSelf)
            DisplayCommand();
        if (TargetPanel.activeSelf)
        {
            Int32 enemyIndex = 0;
            foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
                if (unit.Id != 0 && unit.IsTargetable && !unit.IsPlayer)
                    _targetPanel.Enemies[enemyIndex++].Name.Label.rawText = GetEnemyDisplayName(unit);
        }
    }

    public void UpdateSlidingButtonState()
    {
        if (!Configuration.Interface.PSXBattleMenu || ButtonGroupState.ActiveGroup != CommandGroupButton)
            return;
        Single axis = HonoInputManager.Instance.GetHorizontalNavigation();
        // Threshold: We don't want the stick to be too sensitive for UI navigation
        Single threshold = Mathf.Max(0.7f, HonoInputManager.AnalogThreadhold);
        Boolean leftPressed = axis < -threshold;
        Boolean rightPressed = axis > threshold;
        if (_buttonSliding == null)
        {
            GONavigationButton nextSlidingButton = null;
            if (leftPressed && _commandEnabledState[BattleCommandMenu.Change] == 2)
                nextSlidingButton = _commandPanel.Change;
            else if (rightPressed && _commandEnabledState[BattleCommandMenu.Defend] == 2)
                nextSlidingButton = _commandPanel.Defend;
            if (nextSlidingButton != null)
            {
                Single rowCount = _commandPanel.AccessMenu == null ? 4f : 5f;
                Single row;
                if (_currentCommandIndex == BattleCommandMenu.Attack)
                    row = 0f;
                else if (_currentCommandIndex == BattleCommandMenu.Ability1)
                    row = 1f;
                else if (_currentCommandIndex == BattleCommandMenu.Ability2)
                    row = 2f;
                else if (_currentCommandIndex == BattleCommandMenu.Item)
                    row = 3f;
                else if (_currentCommandIndex == BattleCommandMenu.AccessMenu)
                    row = 4f;
                else
                    return;
                _buttonSliding = nextSlidingButton;
                _buttonSlidePos = new Vector2(1f - (row + 1f) / rowCount, 1f - row / rowCount);
                _buttonSlideInitial = _currentCommandIndex;
                _buttonSliding.IsActive = true;
                _commandPanel.GetCommandButton(_buttonSlideInitial).SetActive(false);
                ButtonGroupState.MuteActiveSound = true;
                ButtonGroupState.ActiveButton = _buttonSliding;
                FF9Sfx.FF9SFX_Play(1047);
                ButtonGroupState.MuteActiveSound = false;
                _currentCommandIndex = (BattleCommandMenu)_buttonSliding.Transform.GetSiblingIndex();
            }
        }
        if (_buttonSliding != null)
        {
            ButtonGroupState.MuteActiveSound = true;
            if (_buttonSliding == _commandPanel.Change && leftPressed)
                _buttonSlideFactor += 0.3f;
            else if (_buttonSliding == _commandPanel.Defend && rightPressed)
                _buttonSlideFactor += 0.3f;
            else
                _buttonSlideFactor -= 0.4f;
            if (ButtonGroupState.ActiveButton != _buttonSliding)
                _buttonSlideFactor = 0f;
            _buttonSlideFactor = Mathf.Clamp01(_buttonSlideFactor);
            if (_buttonSlideFactor > 0f)
            {
                Single slideX = _buttonSliding == _commandPanel.Defend ? 0f : 1f - _buttonSlideFactor;
                Single slideY = _buttonSliding == _commandPanel.Change ? 1f : _buttonSlideFactor;
                _buttonSliding.Widget.SetAnchor(target: _commandPanel.Transform, relLeft: slideX, relRight: slideY, relBottom: _buttonSlidePos.x, relTop: _buttonSlidePos.y);
                _buttonSliding.Name.Label.ResetAndUpdateAnchors();
                _buttonSliding.Highlight.Sprite.ResetAndUpdateAnchors();
                _buttonSliding.Background.Widget.ResetAndUpdateAnchors();
                _buttonSliding.Background.Border.Sprite.ResetAndUpdateAnchors();
                _buttonSliding.Name.Label.Parser.ResetBeforeVariableTags();
            }
            else
            {
                ResetSlidingButton();
            }
            ButtonGroupState.MuteActiveSound = false;
        }
    }

    private void ResetSlidingButton(Boolean selectInitialButton = true)
    {
        if (_buttonSliding == null)
            return;
        _buttonSlideFactor = 0f;
        _commandPanel.GetCommandButton(_buttonSlideInitial).SetActive(true);
        _buttonSliding.IsActive = false;
        _buttonSliding = null;
        if (selectInitialButton && ButtonGroupState.ActiveGroup == CommandGroupButton)
        {
            ButtonGroupState.ActiveButton = _commandPanel.GetCommandButton(_buttonSlideInitial);
            _currentCommandIndex = _buttonSlideInitial;
        }
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (!base.OnKeyConfirm(go) || _hidingHud || _usingMainMenu)
            return true;

        if (ButtonGroupState.ActiveGroup == CommandGroupButton)
        {
            FF9Sfx.FF9SFX_Play(103);
            _currentCommandIndex = (BattleCommandMenu)go.transform.GetSiblingIndex();
            Boolean triggerManualTrance = _isManualTrance && _currentCommandIndex == BattleCommandMenu.Change;
            _currentCommandId = triggerManualTrance ? BattleCommandId.None : GetCommandFromCommandIndex(_currentCommandIndex, CurrentPlayerIndex);
            ResetSlidingButton();
            TryMemorizeCommand();
            if (!triggerManualTrance && IsDoubleCast)
                _doubleCastCount = 1;

            if (triggerManualTrance)
            {
                _subMenuType = SubMenuType.Instant;
                _targetCursor = TargetType.SingleAny;
                BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
                btl.Trance = Byte.MaxValue;
                btl.AlterStatus(BattleStatusId.Trance);
                SetIdle();
            }
            else if (_currentCommandId == BattleCommandId.Defend || _currentCommandId == BattleCommandId.Change)
            {
                _subMenuType = SubMenuType.Instant;
                _targetCursor = TargetType.SingleAny;
                SendCommand(ProcessCommand(CurrentPlayerIndex, CursorGroup.Individual));
                SetIdle();
            }
            else if (_currentCommandId == BattleCommandId.AccessMenu)
            {
                OpenMainMenu(Configuration.Battle.AccessMenus <= 2 ? FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex)?.Player : null);
            }
            else
            {
                CharacterCommand ff9Command = CharacterCommands.Commands[_currentCommandId];
                if (ff9Command.Type == CharacterCommandType.Normal)
                {
                    _subMenuType = SubMenuType.Normal;
                    SetCommandVisibility(false, false);
                    SetTargetVisibility(true);
                }
                else if (ff9Command.Type == CharacterCommandType.Ability)
                {
                    _subMenuType = SubMenuType.Ability;
                    DisplayAbility();
                    SetCommandVisibility(false, false);
                    SetAbilityPanelVisibility(true, false);
                }
                else if (ff9Command.Type == CharacterCommandType.Throw)
                {
                    _subMenuType = SubMenuType.Throw;
                    DisplayItem(ff9Command);
                    SetCommandVisibility(false, false);
                    SetItemPanelVisibility(true, false);
                }
                else if (ff9Command.Type == CharacterCommandType.Item)
                {
                    _subMenuType = SubMenuType.Item;
                    DisplayItem(ff9Command);
                    SetCommandVisibility(false, false);
                    SetItemPanelVisibility(true, false);
                }
                else if (ff9Command.Type == CharacterCommandType.Instant)
                {
                    _subMenuType = SubMenuType.Instant;
                    AA_DATA aaData = GetSelectedActiveAbility(CurrentPlayerIndex, _currentCommandId, -1, out _, out _);
                    TargetType targetType = aaData.Info.Target;
                    if (targetType == TargetType.Self)
                        SendCommand(ProcessCommand(CurrentPlayerIndex, CursorGroup.Individual));
                    else if (targetType == TargetType.Everyone)
                        SendCommand(ProcessCommand(-1, CursorGroup.All));
                    else if (targetType == TargetType.AllEnemy)
                        SendCommand(ProcessCommand(-1, CursorGroup.AllEnemy));
                    else if (targetType == TargetType.AllAlly)
                        SendCommand(ProcessCommand(-1, CursorGroup.AllPlayer));
                    else
                        throw new Exception($"[BattleHUD] The command {_currentCommandId} uses an instant command type {(Int32)CharacterCommandType.Instant}, but its ability {aaData.Name} has a target type {targetType} which cannot be automatised");
                    SetIdle();
                }
            }
        }
        else if (ButtonGroupState.ActiveGroup == TargetGroupButton)
        {
            Boolean commandSent = false;
            if (_cursorType == CursorGroup.Individual)
            {
                for (Int32 i = 0; i < _matchBattleIdEnemyList.Count; i++)
                {
                    if (i < _targetPanel.Enemies.Count && _targetPanel.Enemies[i].GameObject == go)
                    {
                        commandSent = CheckDoubleCast(_matchBattleIdEnemyList[i], _cursorType);
                        break;
                    }
                }
                for (Int32 i = 0; i < _matchBattleIdPlayerList.Count; i++)
                {
                    if (i < _targetPanel.Players.Count && _targetPanel.Players[i].GameObject == go)
                    {
                        commandSent = CheckDoubleCast(_matchBattleIdPlayerList[i], _cursorType);
                        break;
                    }
                }
            }
            else if (_cursorType == CursorGroup.AllPlayer || _cursorType == CursorGroup.AllEnemy || _cursorType == CursorGroup.All)
            {
                commandSent = CheckDoubleCast(-1, _cursorType);
            }
            if (commandSent)
            {
                FF9Sfx.FF9SFX_Play(103);
                SetTargetVisibility(false);
                SetIdle();
            }
        }
        else if (ButtonGroupState.ActiveGroup == AbilityGroupButton)
        {
            if (CheckAbilityStatus(go.GetComponent<RecycleListItem>().ItemDataIndex) == AbilityStatus.Enable)
            {
                FF9Sfx.FF9SFX_Play(103);
                _currentSubMenuIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                _abilityCursorMemorize[new PairCharCommand(CurrentPlayerIndex, _currentCommandId)] = _currentSubMenuIndex;

                SetAbilityPanelVisibility(false, false);
                SetTargetVisibility(true);
            }
            else
            {
                FF9Sfx.FF9SFX_Play(102);
            }
        }
        else if (ButtonGroupState.ActiveGroup == ItemGroupButton)
        {
            if (_itemIdList[_currentSubMenuIndex] != RegularItem.NoItem)
            {
                ItemListDetailWithIconHUD detailHUD = new ItemListDetailWithIconHUD(go, true);
                if (detailHUD.NameLabel.color == FF9TextTool.Gray || detailHUD.NameLabel.color == FF9TextTool.DarkYellow)
                {
                    FF9Sfx.FF9SFX_Play(102);
                    return true;
                }
                FF9Sfx.FF9SFX_Play(103);
                _currentSubMenuIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                _abilityCursorMemorize[new PairCharCommand(CurrentPlayerIndex, _currentCommandId)] = _currentSubMenuIndex;
                if (IsMixCast && _doubleCastCount < 2)
                {
                    ++_doubleCastCount;
                    _firstCommand = ProcessCommand(1, _cursorType);
                    DisplayItem(CharacterCommands.Commands[_firstCommand.CommandId]);
                    SetItemPanelVisibility(true, true);
                }
                else
                {
                    SetItemPanelVisibility(false, false);
                    SetTargetVisibility(true);
                }
            }
            else
            {
                FF9Sfx.FF9SFX_Play(102);
            }
        }
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (UIManager.Input.GetKey(Control.Special))
            return true;

        if (base.OnKeyCancel(go) && !_hidingHud && !_usingMainMenu && ButtonGroupState.ActiveGroup != CommandGroupButton)
        {
            if (ButtonGroupState.ActiveGroup == TargetGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                SetTargetVisibility(false);
                ClearModelPointer();
                if (_subMenuType == SubMenuType.Normal)
                    SetCommandVisibility(true, true);
                else if (_subMenuType == SubMenuType.Ability)
                    SetAbilityPanelVisibility(true, true);
                else
                    SetItemPanelVisibility(true, true);
            }
            else if (ButtonGroupState.ActiveGroup == AbilityGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                if (IsDoubleCast && _doubleCastCount > 0)
                    --_doubleCastCount;
                if (_doubleCastCount == 0)
                {
                    SetAbilityPanelVisibility(false, false);
                    SetCommandVisibility(true, true);
                }
                else
                {
                    SetAbilityPanelVisibility(true, false);
                }
            }
            else if (ButtonGroupState.ActiveGroup == ItemGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                if (IsDoubleCast && _doubleCastCount > 0)
                    --_doubleCastCount;
                if (_doubleCastCount == 0)
                {
                    SetItemPanelVisibility(false, false);
                    SetCommandVisibility(true, true);
                }
                else
                {
                    SetItemPanelVisibility(true, false);
                    DisplayItem(CharacterCommands.Commands[_currentCommandId]);
                }
            }
            else if (ButtonGroupState.ActiveGroup == String.Empty && UIManager.Input.ContainsAndroidQuitKey())
            {
                OnKeyQuit();
            }
        }
        return true;
    }

    public override Boolean OnKeyMenu(GameObject go)
    {
        if (base.OnKeyMenu(go))
        {
            if (_usingMainMenu)
                return true;
            if (!_hidingHud && ButtonGroupState.ActiveGroup == CommandGroupButton)
            {
                if (ReadyQueue.Count > 1)
                {
                    Int32 postponed = ReadyQueue[0];
                    ReadyQueue.RemoveAt(0);
                    ReadyQueue.Add(postponed);
                    for (Int32 i = 0; i < ReadyQueue.Count; i++)
                    {
                        Int32 current = ReadyQueue[i];
                        if (!InputFinishList.Contains(current) && !_unconsciousStateList.Contains(current) && current != CurrentPlayerIndex)
                        {
                            if (i > 0)
                            {
                                ReadyQueue.RemoveAt(i);
                                ReadyQueue.Insert(0, current);
                            }
                            SwitchPlayer(current);
                            return true;
                        }
                    }
                    if (Configuration.Battle.Speed == 2)
                    {
                        // If we end up with the same player
                        BattleHUD.ForceNextTurn = true;
                        return true;
                    }
                }
                else if (ReadyQueue.Count == 1)
                {
                    if (Configuration.Battle.Speed == 2)
                    {
                        // If no other players are ready
                        BattleHUD.ForceNextTurn = true;
                        return true;
                    }
                    else
                    {
                        SwitchPlayer(ReadyQueue[0]);
                    }
                }
            }
            if (Configuration.Battle.AccessMenus <= 0)
                return true;
            Boolean hasAccessMenuButton = _commandPanel.AccessMenu != null && _commandPanel.AccessMenu.IsActive;
            Boolean canOpen = true;
            BattleUnit selectedChar = CurrentPlayerIndex >= 0 ? FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex) : null;
            if (Configuration.Battle.AccessMenus == 1 || Configuration.Battle.AccessMenus == 2)
                canOpen = !hasAccessMenuButton && !_hidingHud && ButtonGroupState.ActiveGroup == ItemGroupButton && _currentCommandIndex == BattleCommandMenu.Item && (selectedChar == null || !selectedChar.IsMonsterTransform || !selectedChar.Data.monster_transform.disable_commands.Contains(BattleCommandId.AccessMenu));
            else if (Configuration.Battle.AccessMenus == 3)
                canOpen = FF9BMenu_IsEnable() && ((!hasAccessMenuButton && ButtonGroupState.ActiveGroup != CommandGroupButton && ButtonGroupState.ActiveGroup != TargetGroupButton) || (hasAccessMenuButton && selectedChar == null));
            if (canOpen)
                OpenMainMenu(Configuration.Battle.AccessMenus <= 2 ? selectedChar?.Player : null);
        }
        return true;
    }

    public override Boolean OnKeyPause(GameObject go)
    {
        if (base.OnKeyPause(go) && FF9StateSystem.Battle.FF9Battle.btl_seq != 2 && FF9StateSystem.Battle.FF9Battle.btl_seq != 1 && !battle.isSpecialTutorialWindow)
        {
            NextSceneIsModal = true;
            _isFromPause = true;
            _beforePauseCommandEnable = _commandEnable;
            _currentButtonGroup = !_hidingHud ? ButtonGroupState.ActiveGroup : _currentButtonGroup;
            FF9BMenu_EnableMenu(false);
            Singleton<HUDMessage>.Instance.Pause(true);
            Hide(() => PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Pause));
        }
        return base.OnKeyPause(go);
    }

    public override void OnKeyQuit()
    {
        if (Loading || FF9StateSystem.Battle.FF9Battle.btl_seq == 2 || FF9StateSystem.Battle.FF9Battle.btl_seq == 1)
            return;
        _beforePauseCommandEnable = _commandEnable;
        _currentButtonGroup = ButtonGroupState.ActiveGroup;
        FF9BMenu_EnableMenu(false);
        ShowQuitUI(_onResumeFromQuit);
    }

    public override Boolean OnKeyLeftBumper(GameObject go)
    {
        if (base.OnKeyLeftBumper(go) && !_hidingHud && ButtonGroupState.ActiveGroup == TargetGroupButton && (_targetCursor == TargetType.ManyAny || _targetCursor == TargetType.ManyEnemy || _targetCursor == TargetType.ManyAlly))
        {
            FF9Sfx.FF9SFX_Play(103);
            _isAllTarget = !_isAllTarget;
            _allTargetToggle.value = _isAllTarget;
            _allTargetButtonComponent.SetState(UIButtonColor.State.Normal, false);
            ToggleAllTarget();
        }
        return true;
    }

    public override Boolean OnKeyRightBumper(GameObject go)
    {
        if (base.OnKeyRightBumper(go) && !_hidingHud && ButtonGroupState.ActiveGroup == TargetGroupButton && (_targetCursor == TargetType.ManyAny || _targetCursor == TargetType.ManyEnemy || _targetCursor == TargetType.ManyAlly))
        {
            FF9Sfx.FF9SFX_Play(103);
            _isAllTarget = !_isAllTarget;
            _allTargetToggle.value = _isAllTarget;
            _allTargetButtonComponent.SetState(UIButtonColor.State.Normal, false);
            ToggleAllTarget();
        }
        return true;
    }

    public override Boolean OnKeyRightTrigger(GameObject go)
    {
        if (base.OnKeyRightTrigger(go) && !_hidingHud && !AndroidTvOnKeyRightTrigger(go))
            ProcessAutoBattleInput();
        return true;
    }

    public override Boolean OnItemSelect(GameObject go)
    {
        if (base.OnItemSelect(go))
        {
            if (ButtonGroupState.ActiveGroup == CommandGroupButton)
            {
                _currentCommandIndex = (BattleCommandMenu)go.transform.GetSiblingIndex();
                if (_currentCommandIndex != BattleCommandMenu.Defend && _currentCommandIndex != BattleCommandMenu.Change)
                    ResetSlidingButton(false);
            }
            else if (ButtonGroupState.ActiveGroup == AbilityGroupButton || ButtonGroupState.ActiveGroup == ItemGroupButton)
            {
                _currentSubMenuIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
            }
            if (ButtonGroupState.ActiveGroup == TargetGroupButton && _cursorType == CursorGroup.Individual)
            {
                if (go.transform.parent == modelButtonManager.transform)
                {
                    Int32 targetIndex = go.GetComponent<ModelButton>().index;
                    Int32 targetLabelIndex = targetIndex >= HonoluluBattleMain.EnemyStartIndex ? _matchBattleIdEnemyList.IndexOf(targetIndex) : _matchBattleIdPlayerList.IndexOf(targetIndex);
                    if (targetLabelIndex != -1)
                    {
                        if (targetIndex >= HonoluluBattleMain.EnemyStartIndex)
                            targetLabelIndex += HonoluluBattleMain.EnemyStartIndex;
                        GONavigationButton targetHud = _targetPanel.AllTargets[targetLabelIndex];
                        if (targetHud.ButtonGroup.enabled)
                            ButtonGroupState.ActiveButton = targetHud.GameObject;
                    }
                }
                else if (go.transform.parent.parent == TargetPanel.transform)
                {
                    Int32 targetIndex = go.transform.GetSiblingIndex();
                    if (go.GetParent().transform.GetSiblingIndex() == 1)
                        targetIndex += HonoluluBattleMain.EnemyStartIndex;
                    if (_currentTargetIndex != targetIndex)
                    {
                        _currentTargetIndex = targetIndex;
                        DisplayTargetPointer();
                    }
                }
            }
        }
        return true;
    }

    private void OpenMainMenu(PLAYER singlePlayerMenu = null)
    {
        _usingMainMenu = true;
        _mainMenuSinglePlayer = singlePlayerMenu;
        _isFromPause = true;
        _beforePauseCommandEnable = _commandEnable;
        _currentButtonGroup = !_hidingHud ? ButtonGroupState.ActiveGroup : _currentButtonGroup;
        FF9BMenu_EnableMenu(false);
        UpdatePlayersForMainMenu();
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, null);
        Hide(OpenMainMenuAfterHide);
    }

    private static void OpenMainMenuAfterHide()
    {
        btl2d.ShowMessages(false);
        PersistenSingleton<UIManager>.Instance.MainMenuScene.EnabledSubMenus = new HashSet<String>(Configuration.Battle.AvailableMenus);
        PersistenSingleton<UIManager>.Instance.MainMenuScene.CurrentSubMenu = PersistenSingleton<UIManager>.Instance.MainMenuScene.GetFirstAvailableSubMenu();
        PersistenSingleton<UIManager>.Instance.MainMenuScene.NeedTweenAndHideSubMenu = true;
        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.MainMenu);
    }

    private void RemoveCursorMemorize()
    {
        _commandCursorMemorize.Clear();
        _abilityCursorMemorize.Clear();

        ButtonGroupState.RemoveCursorMemorize(CommandGroupButton);
        ButtonGroupState.RemoveCursorMemorize(ItemGroupButton);
        ButtonGroupState.RemoveCursorMemorize(AbilityGroupButton);
        ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
    }

    private Boolean AndroidTvOnKeyRightTrigger(GameObject go)
    {
        return CheckAndroidTVModule(Control.RightTrigger);
    }

    private AbilityStatus CheckAbilityStatus(Int32 subMenuIndex)
    {
        CharacterCommand command = CharacterCommands.Commands[_currentCommandId];
        if (CommandIsMonsterTransformCommand(CurrentPlayerIndex, _currentCommandId, out _))
            return GetMonsterTransformAbilityState(command.ListEntry[subMenuIndex]);
        BattleAbilityId abilId = command.GetAbilityId(subMenuIndex);
        if (abilId == BattleAbilityId.Void)
            return AbilityStatus.None;
        return GetAbilityState(ff9abil.GetAbilityIdFromActiveAbility(abilId));
    }

    private void ToggleAllTarget()
    {
        if (_cursorType == CursorGroup.AllEnemy || _cursorType == CursorGroup.AllPlayer)
        {
            if (ButtonGroupState.ActiveButton)
            {
                ButtonGroupState.SetButtonAnimation(ButtonGroupState.ActiveButton, true);
            }
            else
            {
                foreach (GONavigationButton button in _targetPanel.AllTargets)
                    ButtonGroupState.SetButtonAnimation(button, true);

                ButtonGroupState.ActiveButton = ButtonGroupState.GetCursorStartSelect(TargetGroupButton);
            }
            _cursorType = CursorGroup.Individual;
            _targetPanel.Buttons.Player.IsActive = false;
            _targetPanel.Buttons.Enemy.IsActive = false;
        }
        else
        {
            ButtonGroupState.SetButtonAnimation(ButtonGroupState.ActiveButton, false);
            Singleton<PointerManager>.Instance.RemovePointerFromGameObject(ButtonGroupState.ActiveButton);
            _cursorType = _currentTargetIndex >= HonoluluBattleMain.EnemyStartIndex ? CursorGroup.AllEnemy : CursorGroup.AllPlayer;
            _targetPanel.Buttons.Player.IsActive = _targetCursor != TargetType.ManyEnemy;
            _targetPanel.Buttons.Enemy.IsActive = _targetCursor != TargetType.ManyAlly;
        }
        UpdateTargetStates();
        SetTargetHelp();
        DisplayTargetPointer();
    }

    [CompilerGenerated]
    private void GeneratedShow()
    {
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, null);
        PersistenSingleton<UIManager>.Instance.SetGameCameraEnable(true);
        PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(true);
        PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(true);
        PauseButtonGameObject.SetActive(PersistenSingleton<UIManager>.Instance.IsPauseControlEnable && FF9StateSystem.MobilePlatform);
        HelpButtonGameObject.SetActive(PersistenSingleton<UIManager>.Instance.IsPauseControlEnable && FF9StateSystem.MobilePlatform);
        ButtonGroupState.SetScrollButtonToGroup(_abilityScrollList.ScrollButton, AbilityGroupButton);
        ButtonGroupState.SetScrollButtonToGroup(_itemScrollList.ScrollButton, ItemGroupButton);
        ButtonGroupState.SetPointerOffsetToGroup(new Vector2(34f, 0.0f), AbilityGroupButton);
        ButtonGroupState.SetPointerOffsetToGroup(new Vector2(34f, 0.0f), ItemGroupButton);
        ButtonGroupState.SetPointerOffsetToGroup(new Vector2(16f, 0.0f), TargetGroupButton);
        ButtonGroupState.SetPointerOffsetToGroup(new Vector2(10f, 0.0f), CommandGroupButton);
        ButtonGroupState.SetPointerLimitRectToGroup(AbilityPanel.GetComponent<UIWidget>(), _abilityScrollList.cellHeight, AbilityGroupButton);
        ButtonGroupState.SetPointerLimitRectToGroup(ItemPanel.GetComponent<UIWidget>(), _itemScrollList.cellHeight, ItemGroupButton);

        _usingMainMenu = false;
    }
}
