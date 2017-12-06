using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using UnityEngine;
using Object = System.Object;

// ReSharper disable UnusedParameter.Global

public partial class BattleHUD : UIScene
{
    public Boolean BtlWorkLibra => _currentLibraMessageNumber > 0;
    public Boolean BtlWorkPeep => _currentPeepingMessageCount > 0;
    public GameObject PlayerTargetPanel => TargetPanel.GetChild(0);
    public GameObject EnemyTargetPanel => TargetPanel.GetChild(1);
    public List<Int32> ReadyQueue { get; }
    public List<Int32> InputFinishList { get; }
    public Int32 CurrentPlayerIndex { get; private set; }
    public Boolean IsDoubleCast => (Int32)_currentCommandId == 23 || (Int32)_currentCommandId == 21;
    
    public BattleHUD()
    {
        _abilityDetailDict = new Dictionary<Int32, AbilityPlayerDetail>();
        _magicSwordCond = new MagicSwordCondition();
        _enemyCount = -1;
        _playerCount = -1;
        _currentCharacterHp = new List<ParameterStatus>();
        _currentEnemyDieState = new List<Boolean>();
        _hpInfoVal = new List<DamageAnimationInfo>();
        _mpInfoVal = new List<DamageAnimationInfo>();
        _currentMpValue = -1;

        _currentButtonGroup = String.Empty;
        CurrentPlayerIndex = -1;
        _currentTargetIndex = -1;
        _targetIndexList = new List<Int32>();
        ReadyQueue = new List<Int32>();
        InputFinishList = new List<Int32>();
        _unconsciousStateList = new List<Int32>();
        _firstCommand = new CommandDetail();
        _commandCursorMemorize = new Dictionary<Int32, CommandMenu>();
        _ability1CursorMemorize = new Dictionary<Int32, Int32>();
        _ability2CursorMemorize = new Dictionary<Int32, Int32>();
        _itemCursorMemorize = new Dictionary<Int32, Int32>();
        _matchBattleIdPlayerList = new List<Int32>();
        _matchBattleIdEnemyList = new List<Int32>();
        _itemIdList = new List<Byte>();
        _oneTime = true;
    }
    
    public void SetBattleFollowMessage(Int32 pMesNo, params Object[] args)
    {
        String str1 = FF9TextTool.BattleFollowText(pMesNo + 7);
        if (String.IsNullOrEmpty(str1))
            return;

        Byte priority = (Byte)Char.GetNumericValue(str1[0]);
        String str2 = str1.Substring(1);

        if (args.Length > 0)
        {
            String str3 = args[0].ToString();
            Int32 result;
            str2 = !Int32.TryParse(str3, out result) ? str2.Replace("%", str3) : str2.Replace("&", str3);
        }

        SetBattleMessage(str2, priority);
    }

    public void SetBattleFollowMessage(Byte priority, String formatMessage, params Object[] args)
    {
        if (String.IsNullOrEmpty(formatMessage))
            return;

        String message;
        try
        {
            message = String.Format(formatMessage, args);
        }
        catch (FormatException ex)
        {
            Log.Error(ex, "Cannot format a battle follow message.");
            message = formatMessage;
        }

        SetBattleMessage(message, priority);
    }

    public void SetBattleCommandTitle(CMD_DATA pCmd)
    {
        String str1 = String.Empty;
        switch (pCmd.cmd_no)
        {
            case BattleCommandId.Item:
            case BattleCommandId.Throw:
                str1 = FF9TextTool.ItemName(pCmd.sub_no);
                break;
            case BattleCommandId.MagicCounter:
                str1 = pCmd.aa.Name;
                break;
            default:
                if (pCmd.sub_no < 192)
                {
                    Int32 id = CmdTitleTable[pCmd.sub_no].MappedId;
                    switch (id)
                    {
                        case 254: // Magic sword
                            str1 = FormatMagicSwordAbility(pCmd);
                            break;
                        case 255:
                            str1 = FF9TextTool.ActionAbilityName(pCmd.sub_no);
                            break;
                        case 0:
                            break;
                        default:
                            str1 = id >= 192 ? FF9TextTool.BattleCommandTitleText((id & 63) + 1) : FF9TextTool.ActionAbilityName(id);
                            break;
                    }
                }
                break;
        }

        if (String.IsNullOrEmpty(str1) ||( pCmd.cmd_no == BattleCommandId.Change && pCmd.sub_no == 96))
            return;

        SetBattleTitle(str1, 1);
    }

    public String BtlGetAttrName(Int32 pAttr)
    {
        Int32 id = 0;
        while ((pAttr >>= 1) != 0)
            ++id;

        return FF9TextTool.BattleFollowText(id);
    }

    public void SetBattleLibra(BattleUnit pBtl)
    {
        _currentLibraMessageNumber = 1;
        _libraBtlData = pBtl;
        DisplayMessageLibra();
    }

    public void SetBattlePeeping(BattleUnit pBtl)
    {
        if (pBtl.IsPlayer)
            return;

        _peepingEnmData = pBtl.Enemy;

        Boolean flag = false;
        for (Int32 index = 0; index < 4; ++index)
        {
            if (_peepingEnmData.StealableItems[index] == Byte.MaxValue)
                continue;

            flag = true;
            break;
        }

        if (!flag)
        {
            SetBattleMessage(FF9TextTool.BattleLibraText(9), 2);
            _currentPeepingMessageCount = 5;
        }
        else
        {
            _currentPeepingMessageCount = 1;
            DisplayMessagePeeping();
        }
    }

    public void SetBattleTitle(String str, Byte priority)
    {
        if (_currentMessagePriority > priority)
            return;

        _currentMessagePriority = priority;
        _battleMessageCounter = 0.0f;
        DisplayBattleMessage(str, true);
    }

    public void SetBattleMessage(String str, Byte priority)
    {
        if (_currentMessagePriority > priority)
            return;

        _currentMessagePriority = priority;
        _battleMessageCounter = 0.0f;
        DisplayBattleMessage(str, false);
    }
    
    public void DisplayParty()
    {
        Int32 partyIndex = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;

            Int32 playerId = unit.GetIndex();
            UI.PanelParty.Character character = _partyDetail.Characters[partyIndex];
            DamageAnimationInfo hp = _hpInfoVal[partyIndex];
            DamageAnimationInfo mp = _mpInfoVal[partyIndex];
            character.PlayerId = playerId;
            character.IsActive = true;
            DisplayCharacterParameter(character, unit, hp, mp);
            character.TranceBar.IsActive = unit.HasTrance;
            partyIndex++;
        }

        PartyDetailPanel.transform.localPosition = new Vector3(PartyDetailPanel.transform.localPosition.x, DefaultPartyPanelPosY - PartyItemHeight * (_partyDetail.Characters.Count - partyIndex), PartyDetailPanel.transform.localPosition.z);
        for (; partyIndex < _partyDetail.Characters.Count ; ++partyIndex)
        {
            _partyDetail.Characters[partyIndex].IsActive = false;
            _partyDetail.Characters[partyIndex].PlayerId = -1;
        }
    }

    public void AddPlayerToReady(Int32 playerId)
    {
        if (_unconsciousStateList.Contains(playerId))
            return;

        ReadyQueue.Add(playerId);
        _partyDetail.GetCharacter(playerId).ATBBlink = true;
    }

    public void RemovePlayerFromAction(Int32 btlId, Boolean isNeedToClearCommand)
    {
        Int32 num = 0;
        while (1 << num != btlId)
            ++num;

        if (InputFinishList.Contains(num) && isNeedToClearCommand)
            InputFinishList.Remove(num);

        if (!ReadyQueue.Contains(num) || !isNeedToClearCommand)
            return;

        ReadyQueue.Remove(num);
    }

    public void GoToBattleResult()
    {
        if (!_oneTime)
            return;
        _oneTime = false;
        Application.targetFrameRate = 60;
        Hide(() => PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.BattleResult));
    }

    public void GoToGameOver()
    {
        if (!_oneTime)
            return;
        _oneTime = false;
        Application.targetFrameRate = 60;
        Hide(() => PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.GameOver));
    }

    public Boolean FF9BMenu_IsEnable()
    {
        return _commandEnable;
    }

    public static Boolean ForceNextTurn;

    public Boolean FF9BMenu_IsEnableAtb()
    {
        if (!IsNativeEnableAtb())
            return false;

        if (Configuration.Battle.Speed != 2)
            return true;

        if (FF9StateSystem.Battle.FF9Battle.btl_escape_key != 0)
            return true;

        if (FF9StateSystem.Battle.FF9Battle.cur_cmd != null)
            return false;

        for (CMD_DATA cmd = FF9StateSystem.Battle.FF9Battle.cmd_queue.next; cmd != null; cmd = cmd.next)
        {
            BTL_DATA btl = cmd.regist;
            if (btl == null)
                continue;

            BattleUnit unit = new BattleUnit(btl);
            if (unit.CurrentHp == 0 || !unit.CanMove || !unit.IsPlayer)
                continue;

            if (unit.IsUnderAnyStatus(BattleStatus.Freeze))
                continue;

            return false;
        }

        if (UIManager.Battle.CurrentPlayerIndex == -1)
            return true;

        if (!ForceNextTurn)
            return false;

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            BTL_DATA btl = unit.Data;

            if (btl.sel_mode != 0 || btl.sel_menu != 0 || unit.CurrentHp == 0 || btl.bi.atb == 0 || !unit.IsPlayer)
                continue;

            if (unit.CurrentAtb < unit.MaximumAtb)
                return true;
        }

        return false;
    }

    internal Boolean IsNativeEnableAtb()
    {
        if (!_commandEnable)
            return false;

        if ((Int64)FF9StateSystem.Settings.cfg.atb != 1L)
            return true;

        if (_hidingHud)
            return CurrentPlayerIndex == -1 || _currentButtonGroup == CommandGroupButton || _currentButtonGroup == String.Empty;

        return CurrentPlayerIndex == -1 || ButtonGroupState.ActiveGroup == CommandGroupButton || ButtonGroupState.ActiveGroup == String.Empty;
    }

    public void FF9BMenu_EnableMenu(Boolean active)
    {
        if (PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI)
            return;

        if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.BattleHUD)
        {
            _commandEnable = active;
            AllMenuPanel.SetActive(active);
            HideHudHitAreaGameObject.SetActive(active);
            if (!active)
            {
                ButtonGroupState.DisableAllGroup(true);
            }
            else
            {
                if ((_isFromPause || ButtonGroupState.ActiveGroup != String.Empty) && !_isNeedToInit)
                    return;

                _isNeedToInit = false;
                InitialBattle();
                DisplayParty();
                SetIdle();
            }
        }
        else
        {
            _beforePauseCommandEnable = active;
            _isNeedToInit = active;
        }
    }

    public void ItemRequest(Int32 id)
    {
        _needItemUpdate = true;
    }

    public void ItemUse(Int32 id)
    {
        if (ff9item.FF9Item_Remove(id, 1) == 0)
            return;
        _needItemUpdate = true;
    }

    public void ItemUnuse(Int32 id)
    {
        _needItemUpdate = true;
    }

    public void ItemRemove(Int32 id)
    {
        if (ff9item.FF9Item_Remove(id, 1) == 0)
            return;
        _needItemUpdate = true;
    }

    public void ItemAdd(Int32 id)
    {
        if (ff9item.FF9Item_Add(id, 1) == 0)
            return;
        _needItemUpdate = true;
    }

    public void SetIdle()
    {
        SetCommandVisibility(false, false);
        SetTargetVisibility(false);
        SetItemPanelVisibility(false, false);
        SetAbilityPanelVisibility(false, false);
        BackButton.SetActive(false);
        _currentSilenceStatus = false;
        _currentMpValue = -1;
        _currentCommandIndex = CommandMenu.Attack;
        _currentSubMenuIndex = -1;
        CurrentPlayerIndex = -1;
        //currentTranceTrigger = false;
        ButtonGroupState.DisableAllGroup(true);

        _partyDetail.SetDetailButtonState(UIButtonColor.State.Normal, false);
    }

    public void SetPartySwapButtonActive(Boolean isActive)
    {
        _partyDetail.SetPartySwapButtonActive(isActive);
    }

    public void VerifyTarget(Int32 modelIndex)
    {
        if (_hidingHud || !_commandEnable || _cursorType != CursorGroup.Individual)
            return;
        Int32 index = modelIndex >= HonoluluBattleMain.EnemyStartIndex ? _matchBattleIdEnemyList.IndexOf(modelIndex) + 4 : _matchBattleIdPlayerList.IndexOf(modelIndex);
        if (index == -1)
            return;
        FF9Sfx.FF9SFX_Play(103);

        if (_targetPanel.AllTargets[index].ButtonGroup.enabled)
            CheckDoubleCast(modelIndex, CursorGroup.Individual);
    }
}

