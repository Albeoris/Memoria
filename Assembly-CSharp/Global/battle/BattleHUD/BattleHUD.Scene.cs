using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Database;
using Memoria.Scenes;
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

        if (_usingMainMenu)
            UpdateBattleAfterMainMenu();

        _isFromPause = false;
        _oneTime = true;
    }

    public override void Hide(SceneVoidDelegate afterFinished = null)
    {
        base.Hide(afterFinished);
        PauseButtonGameObject.SetActive(false);
        HelpButtonGameObject.SetActive(false);
        if ((!_isFromPause || _usingMainMenu) && UIControlPanel != null && UIControlPanel.Show)
        {
            Configuration.Interface.SaveValues();
            UIControlPanel.Show = false;
        }

        if (!_isFromPause)
            RemoveCursorMemorize();
    }

    public void UpdateSlidingButtonState()
	{
        if (!Configuration.Interface.PSXBattleMenu || ButtonGroupState.ActiveGroup != CommandGroupButton)
            return;
        Boolean leftPressed = (ETb.sKey0 & (EventInput.Lleft | EventInput.Pleft)) != 0;
        Boolean rightPressed = (ETb.sKey0 & (EventInput.Lright | EventInput.Pright)) != 0;
        if (_buttonSliding == null)
		{
            GONavigationButton nextSlidingButton = null;
            if (leftPressed)
                nextSlidingButton = _commandPanel.Change;
            else if (rightPressed)
                nextSlidingButton = _commandPanel.Defend;
            if (nextSlidingButton != null)
            {
                Single rowCount = _commandPanel.AccessMenu == null ? 4f : 5f;
                Single row = 0f;
                if (_currentCommandIndex == CommandMenu.Attack)
                    row = 0f;
                else if (_currentCommandIndex == CommandMenu.Ability1)
                    row = 1f;
                else if (_currentCommandIndex == CommandMenu.Ability2)
                    row = 2f;
                else if (_currentCommandIndex == CommandMenu.Item)
                    row = 3f;
                else if (_currentCommandIndex == CommandMenu.AccessMenu)
                    row = 4f;
                else
                    return;
                _buttonSliding = nextSlidingButton;
                _buttonSlidePos = new Vector2(1f - (row + 1f) / rowCount, 1f - row / rowCount);
                _buttonSlideInitial = _currentCommandIndex;
                _buttonSliding.IsActive = true;
                _commandPanel.GetCommandButton(_buttonSlideInitial).SetActive(false);
                ButtonGroupState.ActiveButton = _buttonSliding;
                _currentCommandIndex = (CommandMenu)_buttonSliding.Transform.GetSiblingIndex();
            }
        }
        if (_buttonSliding != null)
		{
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
            }
            else
            {
                ResetSlidingButton();
            }
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
            _currentCommandIndex = (CommandMenu)go.transform.GetSiblingIndex();
            CommandMenu menuType = _currentCommandIndex;
            _currentCommandId = GetCommandFromCommandIndex(ref menuType, CurrentPlayerIndex);
            ResetSlidingButton();
            TryMemorizeCommand();
            _subMenuType = SubMenuType.Normal;
            if (IsDoubleCast && _doubleCastCount < 2)
                ++_doubleCastCount;

            switch (menuType)
            {
                case CommandMenu.Attack:
                    SetCommandVisibility(false, false);
                    SetTargetVisibility(true);
                    break;
                case CommandMenu.Defend:
                    _targetCursor = 0;
                    SendCommand(ProcessCommand(CurrentPlayerIndex, CursorGroup.Individual));
                    SetIdle();
                    break;
                case CommandMenu.Ability1:
                case CommandMenu.Ability2:
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
                        DisplayItem(true);
                        SetCommandVisibility(false, false);
                        SetItemPanelVisibility(true, false);
                    }
                    break;
                case CommandMenu.Item:
                    DisplayItem(false);
                    SetCommandVisibility(false, false);
                    SetItemPanelVisibility(true, false);
                    break;
                case CommandMenu.Change:
                    _targetCursor = 0;
                    if (_isManualTrance)
                    {
                        BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
                        btl.Trance = Byte.MaxValue;
                        btl.AlterStatus(BattleStatus.Trance);
                    }
                    else
                    {
                        CommandDetail command = ProcessCommand(CurrentPlayerIndex, CursorGroup.Individual);
                        SendCommand(command);
                        SetIdle();
                    }
                    break;
                case CommandMenu.AccessMenu:
                    OpenMainMenu(FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex)?.Player?.Data);
                    break;
            }
        }
        else if (ButtonGroupState.ActiveGroup == TargetGroupButton)
        {
            FF9Sfx.FF9SFX_Play(103);
            if (_cursorType == CursorGroup.Individual)
            {
                for (Int32 i = 0; i < _matchBattleIdEnemyList.Count; i++)
                {
                    if (i < _targetPanel.Enemies.Count && _targetPanel.Enemies[i].GameObject == go)
                    {
                        CheckDoubleCast(_matchBattleIdEnemyList[i], _cursorType);
                        return true;
                    }
                }

                for (Int32 i = 0; i < _matchBattleIdPlayerList.Count; i++)
                {
                    if (i < _targetPanel.Players.Count && _targetPanel.Players[i].GameObject == go)
                    {
                        CheckDoubleCast(_matchBattleIdPlayerList[i], _cursorType);
                        return true;
                    }
                }
            }
            else if (_cursorType == CursorGroup.AllPlayer || _cursorType == CursorGroup.AllEnemy || _cursorType == CursorGroup.All)
            {
                CheckDoubleCast(-1, _cursorType);
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
                FF9Sfx.FF9SFX_Play(103);
                _currentSubMenuIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                _abilityCursorMemorize[new PairCharCommand(CurrentPlayerIndex, _currentCommandId)] = _currentSubMenuIndex;
                SetItemPanelVisibility(false, false);
                SetTargetVisibility(true);
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
                CommandMenu menuType = _currentCommandIndex;
                GetCommandFromCommandIndex(ref menuType, CurrentPlayerIndex);
                FF9Sfx.FF9SFX_Play(101);
                SetTargetVisibility(false);
                ClearModelPointer();
                switch (menuType)
                {
                    case CommandMenu.Attack:
                        SetCommandVisibility(true, true);
                        break;
                    case CommandMenu.Ability1:
                    case CommandMenu.Ability2:
                        if (_subMenuType == SubMenuType.Ability)
                        {
                            SetAbilityPanelVisibility(true, true);
                            break;
                        }
                        if (_subMenuType == SubMenuType.Throw)
                        {
                            SetItemPanelVisibility(true, true);
                            break;
                        }
                        SetCommandVisibility(true, true);
                        break;
                    case CommandMenu.Item:
                        SetItemPanelVisibility(true, true);
                        break;
                }
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
                SetItemPanelVisibility(false, false);
                SetCommandVisibility(true, true);
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
                    using (List<Int32>.Enumerator enumerator = ReadyQueue.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Int32 current = enumerator.Current;
                            if (!InputFinishList.Contains(current) && !_unconsciousStateList.Contains(current) && current != CurrentPlayerIndex)
                            {
                                if (ReadyQueue.IndexOf(current) > 0)
                                {
                                    ReadyQueue.Remove(current);
                                    ReadyQueue.Insert(0, current);
                                }
                                SwitchPlayer(current);
                                break;
                            }
                        }
                    }
                }
                else if (ReadyQueue.Count == 1)
                {
                    SwitchPlayer(ReadyQueue[0]);
                }
            }
            if (Configuration.Battle.AccessMenus <= 0)
                return true;
            Boolean hasAccessMenuButton = _commandPanel.AccessMenu != null;
            Boolean canOpen = true;
            BattleUnit selectedChar = CurrentPlayerIndex >= 0 ? FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex) : null;
            if (Configuration.Battle.AccessMenus == 1 || Configuration.Battle.AccessMenus == 2)
                canOpen = !hasAccessMenuButton && !_hidingHud && ButtonGroupState.ActiveGroup == ItemGroupButton && _currentCommandId == BattleCommandId.Item && (selectedChar == null || !selectedChar.IsMonsterTransform || !selectedChar.Data.monster_transform.disable_commands.Contains(BattleCommandId.AccessMenu));
            else if (Configuration.Battle.AccessMenus == 3)
                canOpen = FF9BMenu_IsEnable() && ((!hasAccessMenuButton && ButtonGroupState.ActiveGroup != CommandGroupButton && ButtonGroupState.ActiveGroup != TargetGroupButton) || (hasAccessMenuButton && selectedChar == null));
            if (canOpen)
                OpenMainMenu(Configuration.Battle.AccessMenus <= 2 ? selectedChar?.Player?.Data : null);
        }
        return true;
    }

    public override Boolean OnKeyPause(GameObject go)
    {
        if (base.OnKeyPause(go) && FF9StateSystem.Battle.FF9Battle.btl_seq != 2 && FF9StateSystem.Battle.FF9Battle.btl_seq != 1)
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
                _currentCommandIndex = (CommandMenu)go.transform.GetSiblingIndex();
                if (_currentCommandIndex != CommandMenu.Defend && _currentCommandIndex != CommandMenu.Change)
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
        PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(false);
        Hide(OpenMainMenuAfterHide);
    }

    private static void OpenMainMenuAfterHide()
    {
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
        if (CommandIsMonsterTransformCommand(CurrentPlayerIndex, _currentCommandId, out _))
            return AbilityStatus.Enable;
        CharacterCommand command = CharacterCommands.Commands[_currentCommandId];
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
            _targetPanel.ActivateButtons(false);
        }
        else
        {
            ButtonGroupState.SetButtonAnimation(ButtonGroupState.ActiveButton, false);
            Singleton<PointerManager>.Instance.RemovePointerFromGameObject(ButtonGroupState.ActiveButton);
            _cursorType = _currentTargetIndex >= HonoluluBattleMain.EnemyStartIndex ? CursorGroup.AllEnemy : CursorGroup.AllPlayer;
            _targetPanel.ActivateButtons(true);
        }
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