using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Scenes;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

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
    public BattleUIControlPanel UIControlPanel { get; set; }

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
        _abilityPanel = new GOScrollablePanel(AbilityPanel);
        _itemPanel = new GOScrollablePanel(ItemPanel);
        _abilityScrollList = _abilityPanel.SubPanel.RecycleListPopulator;
        _itemScrollList = _itemPanel.SubPanel.RecycleListPopulator;

        if (FF9StateSystem.MobilePlatform)
        {
            RunButton.SetActive(true);
            UIEventListener.Get(RunButton).Press += OnRunPress;
        }
        else
        {
            RunButton.SetActive(false);
        }

        _battleDialogWidget = BattleDialogGameObject.GetComponent<UIWidget>();
        _battleDialogLabel = BattleDialogGameObject.GetChild(1).GetComponent<UILabel>();

        _targetPanel.Buttons.Player.EventListener.Click += OnAllTargetClick;
        _targetPanel.Buttons.Player.EventListener.Hover += OnAllTargetHover;
        _targetPanel.Buttons.Enemy.EventListener.Click += OnAllTargetClick;
        _targetPanel.Buttons.Enemy.EventListener.Hover += OnAllTargetHover;

        _statusPanel = new UI.ContainerStatus(this, StatusContainer);
        //_itemTransition = TransitionGameObject.GetChild(0).GetComponent<HonoTweenClipping>();
        //_abilityTransition = TransitionGameObject.GetChild(1).GetComponent<HonoTweenClipping>();
        //_targetTransition = TransitionGameObject.GetChild(2).GetComponent<HonoTweenClipping>();
        _onResumeFromQuit = GeneratedAwake;

        if (Configuration.Control.WrapSomeMenus)
            foreach (GONavigationButton button in _targetPanel.AllTargets)
                button.KeyNavigation.wrapUpDown = true;

        _abilityPanel.Background.Panel.Name.Label.fixedAlignment = true;
        _itemPanel.Background.Panel.Name.Label.fixedAlignment = true;
    }

    private void UpdateSprites()
    {
        UISprite[] relevantUI = gameObject.GetComponentsInChildren<UISprite>(true);
        BetterList<String> atlasSpriteList = FF9UIDataTool.WindowAtlas.GetListOfSprites();
        foreach (UISprite ui in relevantUI)
        {
            if (ui.atlas == FF9UIDataTool.WindowAtlas)
            {
                if (!ui.spriteName.StartsWith("battle_") && atlasSpriteList.Contains("battle_" + ui.spriteName))
                    ui.spriteName = "battle_" + ui.spriteName;
                else if (ui.spriteName.StartsWith("battle_") && !atlasSpriteList.Contains(ui.spriteName))
                    ui.spriteName = ui.spriteName.Substring("battle_".Length);
            }
        }
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

        if (_currentLibraMessageNumber > 0 && DisplayMessageLibra())
            return;
        if (_currentPeepingMessageCount > 0 && DisplayMessagePeeping())
            return;

        Byte maxPriority = 0;
        Message maxMess = null;
        foreach (Message mess in _messageQueue.Values)
        {
            if (mess.priority >= maxPriority)
            {
                maxPriority = mess.priority;
                maxMess = mess;
            }
        }
        if (maxMess != null)
        {
            _currentMessagePriority = maxPriority;
            _battleMessageCounter = maxMess.counter;
            DisplayBattleMessage(maxMess);
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
            if (player.Data.stat.HasAutoAttackEffect && character.ATBBlink)
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

                if ((!_isTranceMenu && unit.IsUnderAnyStatus(BattleStatus.Trance)) || _commandEnabledState[_currentCommandIndex] < 2)
                {
                    FF9Sfx.FF9SFX_Play(101);
                    ResetToReady();
                    return;
                }
            }
        }

        if (switchBtlId >= 0)
        {
            // Switch to player ready when ForceNextTurn (turn-based)
            Int32 playerId = 0;
            while (1 << playerId != switchBtlId)
                ++playerId;
            SwitchPlayer(playerId);
            switchBtlId = -1;
        }

        if (ReadyQueue.Count <= 0 || CurrentPlayerIndex != -1)
            return;

        for (Int32 index = ReadyQueue.Count - 1; index >= 0; --index)
        {
            if (!_unconsciousStateList.Contains(ReadyQueue[index]))
                continue;

            BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(ReadyQueue[index]);
            Boolean needToClearCommand = unit.IsUnderAnyStatus(BattleStatusConst.CmdCancel);
            RemovePlayerFromAction(unit.Id, needToClearCommand);
        }

        for (Int32 index = 0; index < ReadyQueue.Count; ++index)
        {
            Int32 current = ReadyQueue[index];
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

    private void YMenu_ManagerHpMp()
    {
        Int32 charIndex = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;

            DamageAnimationInfo hpAnimation = _hpInfoVal[charIndex];
            DamageAnimationInfo mpAnimation = _mpInfoVal[charIndex];
            for (Int32 pointNum = 0; pointNum < 2; ++pointNum)
            {
                DamageAnimationInfo curAnimation = pointNum != 0 ? mpAnimation : hpAnimation;
                Int32 maxPoint = pointNum != 0 ? (Int32)unit.MaximumMp : (Int32)unit.MaximumHp;
                if (curAnimation.FrameLeft != 0 && curAnimation.RequiredValue <= maxPoint)
                {
                    if (curAnimation.IncrementStep >= 0)
                    {
                        if (curAnimation.CurrentValue + curAnimation.IncrementStep >= curAnimation.RequiredValue)
                        {
                            curAnimation.CurrentValue = curAnimation.RequiredValue;
                            curAnimation.FrameLeft = 0;
                        }
                        else
                        {
                            curAnimation.CurrentValue += curAnimation.IncrementStep;
                            --curAnimation.FrameLeft;
                        }
                    }
                    else if (curAnimation.CurrentValue + curAnimation.IncrementStep <= curAnimation.RequiredValue)
                    {
                        curAnimation.CurrentValue = curAnimation.RequiredValue;
                        curAnimation.FrameLeft = 0;
                    }
                    else
                    {
                        curAnimation.CurrentValue += curAnimation.IncrementStep;
                        --curAnimation.FrameLeft;
                    }
                }
                else
                {
                    Int32 curPoint = pointNum != 0 ? (Int32)unit.CurrentMp : (Int32)unit.CurrentHp;
                    Int32 diff = curPoint - curAnimation.CurrentValue;
                    if (diff == 0)
                        continue;
                    if (curAnimation.CurrentValue > maxPoint)
                    {
                        curAnimation.CurrentValue = maxPoint;
                        continue;
                    }

                    Int32 absDiff = Mathf.Abs(diff);
                    curAnimation.RequiredValue = curPoint;
                    if (absDiff < YINFO_ANIM_HPMP_MIN)
                    {
                        curAnimation.FrameLeft = absDiff;
                    }
                    else
                    {
                        curAnimation.FrameLeft = absDiff * YINFO_ANIM_HPMP_MAX / maxPoint;
                        if (YINFO_ANIM_HPMP_MIN > curAnimation.FrameLeft)
                            curAnimation.FrameLeft = YINFO_ANIM_HPMP_MIN;
                    }
                    curAnimation.IncrementStep = diff < 0 ? (diff - (curAnimation.FrameLeft - 1)) / curAnimation.FrameLeft : (diff + (curAnimation.FrameLeft - 1)) / curAnimation.FrameLeft;
                }
            }
            ++charIndex;
        }
    }

    private void ManagerInfo()
    {
        if (CurrentPlayerIndex < 0)
            return;
        BattleUnit unit = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);
        if ((BattleHUD.EffectedBtlId & unit.Id) != 0)
        {
            if (ButtonGroupState.ActiveGroup == CommandGroupButton)
                TryMemorizeCommand();
            DisplayCommand();
        }
        BattleHUD.EffectedBtlId = 0;
    }

    private Boolean ManageTargetCommand()
    {
        BattleUnit btl = FF9StateSystem.Battle.FF9Battle.GetUnit(CurrentPlayerIndex);

        if ((!_isTranceMenu && btl.IsUnderAnyStatus(BattleStatus.Trance)) || _commandEnabledState[_currentCommandIndex] < 2)
        {
            FF9Sfx.FF9SFX_Play(101);
            ResetToReady();
            return false;
        }

        if (_subMenuType == SubMenuType.Ability)
        {
            AA_DATA aaData = GetSelectedActiveAbility(CurrentPlayerIndex, _currentCommandId, _currentSubMenuIndex, out _, out BattleAbilityId abilId);

            AbilityPlayerDetail detail = _abilityDetailDict[CurrentPlayerIndex];
            if (detail.AbilityMagicSet.TryGetValue(ff9abil.GetAbilityIdFromActiveAbility(abilId), out BattleMagicSwordSet magicSet))
            {
                BattleUnit supporter = BattleState.EnumerateUnits().FirstOrDefault(u => u.PlayerIndex == magicSet.Supporter);
                if (supporter == null || btl.IsUnderAnyStatus(BattleStatusConst.Immobilized | magicSet.BeneficiaryBlockingStatus) || supporter.IsUnderAnyStatus(BattleStatusConst.Immobilized | magicSet.SupporterBlockingStatus))
                {
                    FF9Sfx.FF9SFX_Play(101);
                    ResetToReady();
                    return false;
                }
            }

            if (btl.CurrentMp < GetActionMpCost(aaData, btl, abilId))
            {
                FF9Sfx.FF9SFX_Play(101);
                DisplayAbility();
                SetTargetVisibility(false);
                ClearModelPointer();
                SetAbilityPanelVisibility(true, true);
                return false;
            }

            if ((aaData.Category & 2) != 0 && btl.IsUnderAnyStatus(BattleStatusConst.CannotUseMagic))
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
        Boolean commandSent = false;
        if (_cursorType == CursorGroup.All)
        {
            FF9Sfx.FF9SFX_Play(103);
            commandSent = CheckDoubleCast(-1, _cursorType);
        }
        else if (UICamera.currentTouchID == 0 || UICamera.currentTouchID == 1)
        {
            FF9Sfx.FF9SFX_Play(103);
            if (go == _targetPanel.Buttons.Player.GameObject)
            {
                if (_cursorType == CursorGroup.AllPlayer)
                    commandSent = CheckDoubleCast(-1, _cursorType);
                else
                    OnTargetNavigate(go, KeyCode.RightArrow);
            }
            else if (go == _targetPanel.Buttons.Enemy.GameObject)
            {
                if (_cursorType == CursorGroup.AllEnemy)
                    commandSent = CheckDoubleCast(-1, _cursorType);
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
            commandSent = CheckDoubleCast(-1, _cursorType);
        }
        if (commandSent)
        {
            SetTargetVisibility(false);
            SetIdle();
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
        _currentCommandIndex = BattleCommandMenu.Attack;

        BattleUnit enemy = GetFirstAliveEnemy();
        if (enemy != null)
        {
            Boolean cmdSent = false;
            if (Configuration.Battle.ViviAutoAttack && player.PlayerIndex == CharacterId.Vivi)
                cmdSent = SelectViviMagicInsteadOfAttack_AutoAttack(player, enemy);
            if (!cmdSent)
                btl_cmd.SetCommand(player.Data.cmd[0], BattleCommandId.Attack, (Int32)BattleAbilityId.Attack, enemy.Id, 0U);
            InputFinishList.Add(CurrentPlayerIndex);
        }
        CurrentPlayerIndex = -1;
    }

    private Boolean SelectViviMagicInsteadOfAttack_AutoAttack(BattleUnit caster, BattleUnit target)
    {
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
            caster.Data.cmd[0].info.IsZeroMP = true;
            btl_cmd.SetCommand(caster.Data.cmd[0], BattleCommandId.BlackMagic, (Int32)bestAbility, target.Id, 0U);
            return true;
        }
        return false;
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
