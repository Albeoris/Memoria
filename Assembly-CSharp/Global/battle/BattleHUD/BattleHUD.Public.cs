using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using FF9;
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
    public static HashSet<BattleCommandId> DoubleCastSet = new HashSet<BattleCommandId> {
        BattleCommandId.DoubleBlackMagic,
        BattleCommandId.DoubleWhiteMagic
    };
    public Boolean IsDoubleCast => DoubleCastSet.Contains(_currentCommandId);
    
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
        _abilityCursorMemorize = new Dictionary<PairCharCommand, Int32>();
        _matchBattleIdPlayerList = new List<Int32>();
        _matchBattleIdEnemyList = new List<Int32>();
        _itemIdList = new List<Byte>();
        _oneTime = true;
    }
    
    public void SetBattleFollowMessage(BattleMesages pMes, params Object[] args)
    {
        Int32 pMesNo = (Int32)pMes;
        String fmtMessage = FF9TextTool.BattleFollowText(pMesNo + 7);
        if (String.IsNullOrEmpty(fmtMessage))
            return;

        Byte priority = (Byte)Char.GetNumericValue(fmtMessage[0]);
        String parsedMessage = fmtMessage.Substring(1);

        if (args.Length > 0)
        {
            String str3 = args[0].ToString();
            Int32 result;
            parsedMessage = !Int32.TryParse(str3, out result) ? parsedMessage.Replace("%", str3) : parsedMessage.Replace("&", str3);
        }

        SetBattleMessage(parsedMessage, priority);
        VoicePlayer.PlayBattleVoice(pMesNo + 7, fmtMessage, true);
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

    public String GetBattleCommandTitle(CMD_DATA pCmd)
	{
        switch (pCmd.cmd_no)
        {
            case BattleCommandId.Item:
            case BattleCommandId.Throw:
                return FF9TextTool.ItemName(pCmd.sub_no);
            case BattleCommandId.AutoPotion:
                return String.Empty;
            case BattleCommandId.MagicCounter:
                return pCmd.aa.Name;
            default:
                if (pCmd.sub_no < 192)
                {
                    Int32 id = CmdTitleTable[pCmd.sub_no].MappedId;
                    switch (id)
                    {
                        case 254: // Magic sword
                            return FormatMagicSwordAbility(pCmd);
                        case 255:
                            return FF9TextTool.ActionAbilityName(pCmd.sub_no);
                        case 0:
                            break;
                        default:
                            return id >= 192 ? FF9TextTool.BattleCommandTitleText((id & 63) + 1) : FF9TextTool.ActionAbilityName(id);
                    }
                }
                else
                {
                    return FF9TextTool.ActionAbilityName(pCmd.sub_no);
                }
                break;
        }
        return String.Empty;
    }

    public void SetBattleCommandTitle(CMD_DATA pCmd)
    {
        String str1 = GetBattleCommandTitle(pCmd);

        if (String.IsNullOrEmpty(str1) || (pCmd.cmd_no == BattleCommandId.Change && pCmd.sub_no == 96))
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
            SetBattleMessage(FF9TextTool.BattleLibraText(9), 3);
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
        
        CorrectPartyPanelPosition(partyIndex);

        for (; partyIndex < _partyDetail.Characters.Count ; ++partyIndex)
        {
            _partyDetail.Characters[partyIndex].IsActive = false;
            _partyDetail.Characters[partyIndex].PlayerId = -1;
        }
    }

    private void CorrectPartyPanelPosition(Int32 partyIndex)
    {
        // TODO Check Native: #147, Didn't notice any changes
        var y = this.PartyDetailPanel.transform.localPosition.y;

        var hp = _statusPanel.HP;
        var mp = _statusPanel.MP;
        var good = _statusPanel.GoodStatus;
        var bad = _statusPanel.BadStatus;

        hp.Transform.SetY(y);
        mp.Transform.SetY(y);
        good.Transform.SetY(y);
        bad.Transform.SetY(y);

        hp.Caption.Content.GameObject.transform.localScale = new Vector3(1f, 0.25f * partyIndex, 1f);
        mp.Caption.Content.GameObject.transform.localScale = new Vector3(1f, 0.25f * partyIndex, 1f);
        good.Caption.Content.GameObject.transform.localScale = new Vector3(1f, 0.25f * partyIndex, 1f);
        bad.Caption.Content.GameObject.transform.localScale = new Vector3(1f, 0.25f * partyIndex, 1f);
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
        FPSManager.SetTargetFPS(Configuration.Graphics.MenuFPS);
        FPSManager.SetMainLoopSpeed(Configuration.Graphics.MenuTPS);

        UInt32 gil = (UInt32)battle.btl_bonus.gil;
        for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
            if (next.bi.player == 0)
                gil += btl_util.getEnemyTypePtr(next).bonus.gil;
        if (FF9StateSystem.Common.FF9.btl_result == 4)
            btl_sys.ClearBattleBonus();
        for (Int32 i = 0; i < 4; i++)
        {
            PLAYER player = FF9StateSystem.Common.FF9.party.member[i];
            if (player != null)
                foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(player.sa))
                    saFeature.TriggerOnBattleResult(player, battle.btl_bonus, new List<FF9ITEM>(), "BattleEnd", gil / 10U);
        }
        if (FF9StateSystem.Common.FF9.btl_result == 4 && (battle.btl_bonus.gil != 0 || battle.btl_bonus.exp != 0 || battle.btl_bonus.ap != 0 || battle.btl_bonus.card != Byte.MaxValue))
            battle.btl_bonus.escape_gil = true;

        Hide(() => PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.BattleResult));
    }

    public void GoToGameOver()
    {
        if (!_oneTime)
            return;
        _oneTime = false;
        FPSManager.SetTargetFPS(Configuration.Graphics.MenuFPS);
        FPSManager.SetMainLoopSpeed(Configuration.Graphics.MenuTPS);
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

    public void ClearCursorMemorize(Int32 playerIndex, BattleCommandId commandId)
	{
        _abilityCursorMemorize.Remove(new PairCharCommand(playerIndex, commandId));
    }
}

