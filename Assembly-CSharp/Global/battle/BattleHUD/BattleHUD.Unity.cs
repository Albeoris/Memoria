using System;
using System.Collections.Generic;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Database;
using Memoria.Prime;
using Memoria.Scenes;
using Memoria.Test;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Global

public partial class BattleHUD : UIScene
{
    public ModelButtonManager modelButtonManager;
    public GameObject AutoBattleHud;
    public GameObject AutoBattleButton;
    public GameObject AllTargetButton;
    public GameObject RunButton;
    public GameObject BackButton;
    public GameObject PauseButtonGameObject;
    public GameObject HelpButtonGameObject;
    public GameObject HideHudHitAreaGameObject;
    public GameObject AllMenuPanel;
    public GameObject TargetPanel;
    public GameObject AbilityPanel;
    public GameObject ItemPanel;
    public GameObject CommandPanel;
    public GameObject PartyDetailPanel;
    public GameObject BattleDialogGameObject;
    public GameObject StatusContainer;
    public GameObject TransitionGameObject;
    public GameObject ScreenFadeGameObject;

    private void Update()
    {
        if (PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI || PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.BattleHUD)
            return;
        UpdatePlayer();
        UpdateMessage();
        if (!_commandEnable)
            return;
        if (UIManager.Input.GetKey(Control.LeftBumper) && UIManager.Input.GetKey(Control.RightBumper) || _isTryingToRun)
        {
            _runCounter += RealTime.deltaTime;
            FF9StateSystem.Battle.FF9Battle.btl_escape_key = 1;
            if (_runCounter > 1.0)
            {
                _runCounter = 0.0f;
                btl_sys.CheckEscape(true);
            }
            else
                btl_sys.CheckEscape(false);
        }
        else
        {
            _runCounter = 0.0f;
            FF9StateSystem.Battle.FF9Battle.btl_escape_key = 0;
        }

        SetHudVisibility(!UIManager.Input.GetKey(Control.Special));
        UpdateAndroidTV();

        // TODO
        //if (true)
        //{
        //    _partyDetail.Transform.SetY(-450);
        //    _partyDetail.Captions.TopBorder.Sprite.width = 360;
        //    _partyDetail.Captions.TopBorder.Transform.SetX(80);
        //    _partyDetail.Characters.Table.padding.y -= 15;
        //    _partyDetail.Characters.Transform.SetY(150);
        //    foreach (UI.PanelParty.Character character in _partyDetail.Characters.Entries)
        //    {
        //        character.Name.Transform.SetX(-220);
        //        //character.Highlight.Transform.AddY(3);
        //        character.Highlight.Sprite.height = 41;
        //        character.Highlight.Sprite.width = 460;
        //    }
        //}
    }
    
    private void Awake()
    {
        FadingComponent = ScreenFadeGameObject.GetExactComponent<HonoFading>();

        _partyDetail = new UI.PanelParty(this, PartyDetailPanel);
        _autoBattleButtonComponent = AutoBattleButton.GetComponent<UIButton>();
        _allTargetButtonComponent = AllTargetButton.GetComponent<UIButton>();
        _autoBattleToggle = AutoBattleButton.GetComponent<UIToggle>();
        _allTargetToggle = AllTargetButton.GetComponent<UIToggle>();
        _autoBattleToggle.validator = OnAutoToggleValidate;
        _allTargetToggle.validator = OnAllTargetToggleValidate;
        _commandPanel = new UI.PanelCommand(this, CommandPanel);
        _targetPanel = new UI.PanelTarget(this, TargetPanel);

        if (FF9StateSystem.MobilePlatform)
        {
            RunButton.SetActive(true);
            UIEventListener eventListener = UIEventListener.Get(RunButton);
            eventListener.Press += OnRunPress;
        }
        else
            RunButton.SetActive(false);

        _battleDialogWidget = BattleDialogGameObject.GetComponent<UIWidget>();
        _battleDialogLabel = BattleDialogGameObject.GetChild(1).GetComponent<UILabel>();

        _targetPanel.Buttons.Player.EventListener.Click += OnAllTargetClick;
        _targetPanel.Buttons.Player.EventListener.Hover += OnAllTargetHover;
        _targetPanel.Buttons.Enemy.EventListener.Click += OnAllTargetClick;
        _targetPanel.Buttons.Enemy.EventListener.Hover += OnAllTargetHover;

        _statusPanel = new UI.ContainerStatus(this, StatusContainer);
        _itemScrollList = ItemPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        _abilityScrollList = AbilityPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        //_itemTransition = TransitionGameObject.GetChild(0).GetComponent<HonoTweenClipping>();
        //_abilityTransition = TransitionGameObject.GetChild(1).GetComponent<HonoTweenClipping>();
        //_targetTransition = TransitionGameObject.GetChild(2).GetComponent<HonoTweenClipping>();
        _onResumeFromQuit = GeneratedAwake;
    }

    private void UpdateAndroidTV()
    {
        HonoInputManager instance = PersistenSingleton<HonoInputManager>.Instance;
        if (!FF9StateSystem.AndroidTVPlatform || !instance.IsControllerConnect || !FF9StateSystem.EnableAndroidTVJoystickMode)
            return;
        Single axisRaw = Input.GetAxisRaw(instance.SpecificPlatformRightTriggerKey);
        Boolean button = Input.GetButton(instance.DefaultJoystickInputKeys[2]);
        Boolean flag = axisRaw > 0.189999997615814 && button;
        if (flag && _lastFrameRightTriggerAxis > 0.189999997615814 && _lastFramePressOnMenu)
            flag = false;
        if (flag && !_hidingHud)
            ProcessAutoBattleInput();
        _lastFrameRightTriggerAxis = axisRaw;
        _lastFramePressOnMenu = button;
    }

    private void UpdateMessage()
    {
        if (!BattleDialogGameObject.activeSelf || PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.BattleHUD)
            return;

        List<String> queueRemove = new List<String>();
        foreach (Message mess in _messageQueue.Values)
        {
            mess.counter += Time.deltaTime * FF9StateSystem.Settings.FastForwardFactor;
            if (mess.counter >= BattleMessageTimeTick[FF9StateSystem.Settings.cfg.btl_msg] / 15.0)
                queueRemove.Add(mess.message);
        }
        foreach (String mess in queueRemove)
            _messageQueue.Remove(mess);

        _battleMessageCounter += Time.deltaTime * FF9StateSystem.Settings.FastForwardFactor;
        if (_battleMessageCounter < BattleMessageTimeTick[FF9StateSystem.Settings.cfg.btl_msg] / 15.0)
            return;

        BattleDialogGameObject.SetActive(false);
        _currentMessagePriority = 0;

        if (_currentLibraMessageNumber > 0)
        {
            DisplayMessageLibra();
            return;
        }
        if (_currentPeepingMessageCount > 0)
        {
            DisplayMessagePeeping();
            return;
        }

        Byte maxPriority = 0;
        String maxMess = null;
        Single maxCounter = 0f;
        Boolean maxIsRect = false;
        foreach (Message mess in _messageQueue.Values)
        {
            if (mess.priority >= maxPriority)
            {
                maxPriority = mess.priority;
                maxMess = mess.message;
                maxCounter = mess.counter;
                maxIsRect = mess.isRect;
            }
        }
        if (!String.IsNullOrEmpty(maxMess))
        {
            _currentMessagePriority = maxPriority;
            _battleMessageCounter = maxCounter;
            DisplayBattleMessage(maxMess, maxIsRect);
        }
    }

    private void UpdatePlayer()
    {
        _blinkAlphaCounter += RealTime.deltaTime * 3f;
        _blinkAlphaCounter = _blinkAlphaCounter <= 2.0 ? _blinkAlphaCounter : 0.0f;
        Single alpha = _blinkAlphaCounter > 1.0 ? 2f - _blinkAlphaCounter : _blinkAlphaCounter;
        if (!_commandEnable)
            return;

        foreach (UI.PanelParty.Character character in _partyDetail.Characters.Entries)
        {
            if (character.PlayerId == -1)
                continue;

            BattleUnit player = FF9StateSystem.Battle.FF9Battle.GetUnit(character.PlayerId);
            if ((player.IsUnderAnyStatus(BattleStatus.Confuse) || player.IsUnderAnyStatus(BattleStatus.Berserk)) && character.ATBBlink)
                character.ATBBlink = false;
            if (IsEnableInput(player) && !_isAutoAttack)
            {
                if (character.ATBBlink)
                    character.ATBBar.Foreground.Widget.alpha = alpha;

                if (character.TranceBlink && player.HasTrance)
                    character.TranceBar.Foreground.Widget.alpha = alpha;
            }
            else
            {
                if (character.ATBBlink)
                {
                    character.ATBBar.Foreground.Widget.alpha = 1f;
                    character.ATBBar.Foreground.Highlight.Sprite.alpha = 0.0f;
                }
                if (character.TranceBlink && player.HasTrance)
                {
                    character.TranceBar.Foreground.Widget.alpha = 1f;
                    character.TranceBar.Foreground.Highlight.Sprite.alpha = 0.0f;
                }
            }
        }

        YMenu_ManagerHpMp();
        CheckPlayerState();
        DisplayPartyRealtime();

        if (TargetPanel.activeSelf)
        {
            UpdateTargetStates();
            _statusPanel.DisplayStatusRealtime();
        }

        ManagerTarget();
        ManagerInfo();

        if (CurrentPlayerIndex > -1)
        {
            BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
            if (ButtonGroupState.ActiveGroup == CommandGroupButton && _isTranceMenu)
            {
                _tranceColorCounter = (_tranceColorCounter + 1) % TranceTextColor.Length;
                _commandPanel.SetCaptionColor(TranceTextColor[_tranceColorCounter]);
            }

            if (!IsEnableInput(unit))
            {
                SetIdle();
                return;
            }

            if (TargetPanel.activeSelf)
            {
                if (!ManageTargetCommand())
                    return;
            }
            else if (AbilityPanel.activeSelf || ItemPanel.activeSelf)
            {
                if (AbilityPanel.activeSelf)
                    DisplayAbilityRealTime();

                if (ItemPanel.activeSelf)
                    DisplayItemRealTime();

                if (_currentCommandId == BattleCommandId.MagicSword && (!_magicSwordCond.IsViviExist || _magicSwordCond.IsViviDead || _magicSwordCond.IsSteinerMini))
                {
                    FF9Sfx.FF9SFX_Play(101);
                    ResetToReady();
                    return;
                }

                if (!_isTranceMenu && unit.IsUnderAnyStatus(BattleStatus.Trance))
                {
                    FF9Sfx.FF9SFX_Play(101);
                    ResetToReady();
                    return;
                }
            }
        }

        if (ReadyQueue.Count <= 0 || CurrentPlayerIndex != -1)
            return;

        for (Int32 index = ReadyQueue.Count - 1; index >= 0; --index)
        {
            if (!_unconsciousStateList.Contains(ReadyQueue[index]))
                continue;

            BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(ReadyQueue[index]);
            Boolean needToClearCommand = unit.IsUnderAnyStatus(BattleStatus.CmdCancel);
            RemovePlayerFromAction(unit.Id, needToClearCommand);
        }

        using (List<Int32>.Enumerator enumerator = ReadyQueue.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                Int32 current = enumerator.Current;
                if (InputFinishList.Contains(current) || _unconsciousStateList.Contains(current))
                    continue;

                if (_isAutoAttack)
                {
                    SendAutoAttackCommand(current);
                    break;
                }

                SwitchPlayer(current);
                break;
            }
        }
    }

    private void YMenu_ManagerHpMp()
    {
        Int32 index1 = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;

            DamageAnimationInfo infoVal1 = _hpInfoVal[index1];
            DamageAnimationInfo infoVal2 = _mpInfoVal[index1];
            for (Int32 index2 = 0; index2 < 2; ++index2)
            {
                DamageAnimationInfo infoVal3 = index2 != 0 ? infoVal2 : infoVal1;
                if (infoVal3.FrameLeft != 0)
                {
                    if (infoVal3.IncrementStep >= 0)
                    {
                        if (infoVal3.CurrentValue + infoVal3.IncrementStep >= infoVal3.RequiredValue)
                        {
                            infoVal3.CurrentValue = infoVal3.RequiredValue;
                            infoVal3.FrameLeft = 0;
                        }
                        else
                        {
                            infoVal3.CurrentValue += infoVal3.IncrementStep;
                            --infoVal3.FrameLeft;
                        }
                    }
                    else if (infoVal3.CurrentValue + infoVal3.IncrementStep <= infoVal3.RequiredValue)
                    {
                        infoVal3.CurrentValue = infoVal3.RequiredValue;
                        infoVal3.FrameLeft = 0;
                    }
                    else
                    {
                        infoVal3.CurrentValue += infoVal3.IncrementStep;
                        --infoVal3.FrameLeft;
                    }
                }
                else
                {
                    Int32 num1 = index2 != 0 ? (Int32)unit.CurrentMp : (Int32)unit.CurrentHp;
                    Int32 num2 = index2 != 0 ? (Int32)unit.MaximumMp : (Int32)unit.MaximumHp;
                    Int32 num3;
                    if ((num3 = num1 - infoVal3.CurrentValue) == 0)
                        continue;

                    Int32 num4 = Mathf.Abs(num3);
                    infoVal3.RequiredValue = num1;
                    if (num4 < YINFO_ANIM_HPMP_MIN)
                    {
                        infoVal3.FrameLeft = num4;
                    }
                    else
                    {
                        infoVal3.FrameLeft = num4 * YINFO_ANIM_HPMP_MAX / num2;
                        if (YINFO_ANIM_HPMP_MIN > infoVal3.FrameLeft)
                            infoVal3.FrameLeft = YINFO_ANIM_HPMP_MIN;
                    }
                    infoVal3.IncrementStep = 0 > num3 ? (num3 - (infoVal3.FrameLeft - 1)) / infoVal3.FrameLeft : (num3 + (infoVal3.FrameLeft - 1)) / infoVal3.FrameLeft;
                }
            }
            ++index1;
        }
    }

    private void ManagerInfo()
    {
        MagicSwordCondition magicSwordCondition1 = new MagicSwordCondition();
        MagicSwordCondition magicSwordCondition2 = new MagicSwordCondition();
        magicSwordCondition1.IsViviExist = _magicSwordCond.IsViviExist;
        magicSwordCondition1.IsViviDead = _magicSwordCond.IsViviDead;
        magicSwordCondition1.IsSteinerMini = _magicSwordCond.IsSteinerMini;

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                break;

            if (unit.PlayerIndex == CharacterId.Vivi)
            {
                magicSwordCondition2.IsViviExist = true;
                if (unit.CurrentHp == 0)
                    magicSwordCondition2.IsViviDead = true;
                else if (unit.IsUnderAnyStatus((BattleStatus)318905611U))
                    magicSwordCondition2.IsViviDead = true;
            }
            else if (unit.PlayerIndex == CharacterId.Steiner)
            {
                magicSwordCondition2.IsSteinerMini = unit.IsUnderAnyStatus(BattleStatus.Mini);
            }
        }
        if (magicSwordCondition1.Changed(magicSwordCondition2))
        {
            _magicSwordCond.IsViviExist = magicSwordCondition2.IsViviExist;
            _magicSwordCond.IsViviDead = magicSwordCondition2.IsViviDead;
            _magicSwordCond.IsSteinerMini = magicSwordCondition2.IsSteinerMini;
            if (CurrentPlayerIndex == -1)
                return;

            DisplayCommand();
        }
        else
        {
            if (_isTranceMenu || CurrentPlayerIndex == -1)
                return;

            BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
            if (!unit.IsUnderAnyStatus(BattleStatus.Trance))
                return;

            DisplayCommand();
        }
    }

    private Boolean ManageTargetCommand()
    {
        BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
        if (_currentCommandId == BattleCommandId.MagicSword && (!_magicSwordCond.IsViviExist || _magicSwordCond.IsViviDead || _magicSwordCond.IsSteinerMini))
        {
            FF9Sfx.FF9SFX_Play(101);
            ResetToReady();
            return false;
        }

        if (!_isTranceMenu && btl.IsUnderAnyStatus(BattleStatus.Trance))
        {
            FF9Sfx.FF9SFX_Play(101);
            ResetToReady();
            return false;
        }

        if (_subMenuType == SubMenuType.Ability)
        {
            AA_DATA aaData = GetSelectedActiveAbility(CurrentPlayerIndex, _currentCommandId, _currentSubMenuIndex, out _);

            if (btl.CurrentMp < GetActionMpCost(aaData))
            {
                FF9Sfx.FF9SFX_Play(101);
                DisplayAbility();
                SetTargetVisibility(false);
                ClearModelPointer();
                SetAbilityPanelVisibility(true, true);
                return false;
            }

            if ((aaData.Category & 2) != 0 && btl.IsUnderAnyStatus(BattleStatus.Silence))
            {
                FF9Sfx.FF9SFX_Play(101);
                DisplayAbility();
                SetTargetVisibility(false);
                ClearModelPointer();
                SetAbilityPanelVisibility(true, true);
                return false;
            }
        }
        else if ((_subMenuType == SubMenuType.Item || _subMenuType == SubMenuType.Throw) && ff9item.FF9Item_GetCount(_itemIdList[_currentSubMenuIndex]) == 0)
        {
            FF9Sfx.FF9SFX_Play(101);
            DisplayItem(_subMenuType == SubMenuType.Throw);
            SetTargetVisibility(false);
            ClearModelPointer();
            SetItemPanelVisibility(true, true);
            return false;
        }

        return true;
    }

    private static void ManagerTarget()
    {
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            BTL_DATA btlData = unit.Data;
            if (btlData.tar_mode < 2)
                continue;

            if (btlData.tar_mode == 2)
            {
                btlData.bi.target = btlData.bi.atb = 0;
                btlData.tar_mode = 0;
            }
            else if (btlData.tar_mode == 3)
            {
                btlData.bi.target = btlData.bi.atb = 1;
                btlData.tar_mode = 1;
            }
        }
    }

    private void OnAllTargetHover(GameObject go, Boolean isHover)
    {
        if (!isHover || _cursorType != CursorGroup.AllEnemy && _cursorType != CursorGroup.AllPlayer)
            return;

        if (go == _targetPanel.Buttons.Player.GameObject)
        {
            if (_cursorType == CursorGroup.AllPlayer)
                return;

            FF9Sfx.FF9SFX_Play(103);
            _cursorType = CursorGroup.AllPlayer;
            DisplayTargetPointer();
        }
        else if (go == _targetPanel.Buttons.Enemy.GameObject)
        {
            if (_cursorType == CursorGroup.AllEnemy)
                return;

            FF9Sfx.FF9SFX_Play(103);
            _cursorType = CursorGroup.AllEnemy;
            DisplayTargetPointer();
        }
    }

    private void OnAllTargetClick(GameObject go)
    {
        if (_cursorType == CursorGroup.All)
        {
            FF9Sfx.FF9SFX_Play(103);
            CheckDoubleCast(-1, _cursorType);
        }
        else if (UICamera.currentTouchID == 0 || UICamera.currentTouchID == 1)
        {
            FF9Sfx.FF9SFX_Play(103);
            if (go == _targetPanel.Buttons.Player.GameObject)
            {
                if (_cursorType == CursorGroup.AllPlayer)
                    CheckDoubleCast(-1, _cursorType);
                else
                    OnTargetNavigate(go, KeyCode.RightArrow);
            }
            else if (go == _targetPanel.Buttons.Enemy.GameObject)
            {
                    if (_cursorType == CursorGroup.AllEnemy)
                        CheckDoubleCast(-1, _cursorType);
                    else
                        OnTargetNavigate(go, KeyCode.LeftArrow);
            }
        }
        else
        {
            if (UICamera.currentTouchID != -1)
                return;

            FF9Sfx.FF9SFX_Play(103);
            if (go == _targetPanel.Buttons.Player.GameObject)
                _cursorType = CursorGroup.AllPlayer;
            else if (go == _targetPanel.Buttons.Enemy.GameObject)
                _cursorType = CursorGroup.AllEnemy;
            CheckDoubleCast(-1, _cursorType);
        }
    }

    private void OnRunPress(GameObject go, Boolean isDown)
    {
        _runCounter = 0.0f;
        _isTryingToRun = isDown;
    }

    private Boolean OnAllTargetToggleValidate(Boolean choice)
    {
        if (_isAllTarget != _allTargetToggle.value)
            return true;

        _allTargetButtonComponent.SetState(UIButtonColor.State.Normal, false);
        return false;
    }

    private Boolean OnAutoToggleValidate(Boolean choice)
    {
        if (_isAutoAttack != _autoBattleToggle.value)
            return true;

        _autoBattleButtonComponent.SetState(UIButtonColor.State.Normal, false);
        return false;
    }

    private void SendAutoAttackCommand(Int32 playerIndex)
    {
        BattleUnit player = FF9StateSystem.Battle.FF9Battle.GetUnit(playerIndex);
        CMD_DATA cmd = player.Data.cmd[0];
        if (cmd != null && btl_cmd.CheckUsingCommand(cmd))
            return;
        CurrentPlayerIndex = playerIndex;
        _currentCommandIndex = CommandMenu.Attack;
        
        BattleUnit enemy = GetFirstAliveEnemy();
        if (enemy != null)
        {
            btl_cmd.SetCommand(player.Data.cmd[0], BattleCommandId.Attack, (Int32)BattleAbilityId.Attack, enemy.Id, 0U);
            InputFinishList.Add(CurrentPlayerIndex);
        }
        CurrentPlayerIndex = -1;
    }

    private void ResetToReady()
    {
        SetItemPanelVisibility(false, false);
        SetAbilityPanelVisibility(false, false);
        SetTargetVisibility(false);
        ClearModelPointer();
        DisplayCommand();
        SetCommandVisibility(true, false);
    }

    private void GeneratedAwake()
    {
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, null);
        PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(true);
        PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(true);
        _commandEnable = _beforePauseCommandEnable;
        if (!_commandEnable)
            return;

        _isFromPause = true;
        FF9BMenu_EnableMenu(true);
        DisplayTargetPointer();
        _isFromPause = false;
    }

    private void DisplayPartyRealtime()
    {
        Int32 index = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;

            UI.PanelParty.Character character = _partyDetail.Characters[index];
            DamageAnimationInfo hp = _hpInfoVal[index];
            DamageAnimationInfo mp = _mpInfoVal[index];
            index++;
            DisplayCharacterParameter(character, unit, hp, mp);
        }
    }
}