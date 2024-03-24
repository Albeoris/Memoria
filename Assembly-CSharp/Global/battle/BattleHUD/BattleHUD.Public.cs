using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Scenes;
using Memoria.Assets;
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
        _commandMPCond = new CommandMPCondition();
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
        _itemIdList = new List<RegularItem>();
        _messageQueue = new Dictionary<String, Message>();
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

        VoicePlayer.PlayBattleVoice(pMesNo + 7, fmtMessage, true);
        SetBattleMessage(parsedMessage, priority);
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
        if (pCmd.regist != null && pCmd.regist.bi.player == 0)
            return pCmd.aa.Name;
        if (btl_util.IsCommandMonsterTransform(pCmd))
        {
            AA_DATA aaData = btl_util.GetCommandMonsterAttack(pCmd);
            if (aaData != null)
                return aaData.Name;
            return String.Empty;
        }
        switch (pCmd.cmd_no)
        {
            case BattleCommandId.Item:
            case BattleCommandId.Throw:
                return FF9TextTool.ItemName((RegularItem)pCmd.sub_no);
            case BattleCommandId.AutoPotion:
                return String.Empty;
            case BattleCommandId.MagicCounter:
                return pCmd.aa.Name;
            default:
                BattleAbilityId abilId = btl_util.GetCommandMainActionIndex(pCmd);
                if (abilId != BattleAbilityId.Void)
                {
                    Byte type = FF9StateSystem.Battle.FF9Battle.aa_data[abilId].CastingTitleType;
                    switch (type)
                    {
                        case 254: // Magic sword
                            return FormatMagicSwordAbility(pCmd);
                        case 255:
                            return FF9TextTool.ActionAbilityName(abilId);
                        case 0:
                            break;
                        default:
                            return type < 192 ? FF9TextTool.ActionAbilityName((BattleAbilityId)type) : FF9TextTool.BattleCommandTitleText((type & 63) + 1);
                    }
                }
                else
                {
                    return FF9TextTool.ActionAbilityName(abilId);
                }
                break;
        }
        return String.Empty;
    }

    public void SetBattleCommandTitle(CMD_DATA pCmd)
    {
        String title = GetBattleCommandTitle(pCmd);

        if (String.IsNullOrEmpty(title))
            return;

        SetBattleTitle(pCmd, title, 1);
    }

    public String BtlGetAttrName(Int32 pAttr)
    {
        Int32 id = 0;
        while ((pAttr >>= 1) != 0)
            ++id;

        return FF9TextTool.BattleFollowText(id);
    }

    public void SetBattleLibra(BattleUnit pBtl, LibraInformation infos = LibraInformation.Default)
    {
        if (Configuration.Interface.ScanDisplay)
        {
            Single additionalWidth = 0.0f;
            String libraMessage = $"[{NGUIText.Center}]";
            if ((infos & LibraInformation.Name) != 0)
                libraMessage += Singleton<HelpDialog>.Instance.PhraseLabel.PhrasePreOpcodeSymbol(pBtl.Enemy.Name, ref additionalWidth);
            if ((infos & LibraInformation.Level) != 0)
                libraMessage += FF9TextTool.BattleLibraText(10) + pBtl.Level.ToString();
            if ((infos & LibraInformation.NameLevel) != 0)
                libraMessage += "\n\n";
            if ((infos & LibraInformation.HP) != 0)
            {
                libraMessage += FF9TextTool.BattleLibraText(11);
                libraMessage += pBtl.CurrentHp;
                libraMessage += FF9TextTool.BattleLibraText(13);
                libraMessage += pBtl.MaximumHp;
            }
            if ((infos & LibraInformation.MP) != 0)
            {
                libraMessage += FF9TextTool.BattleLibraText(12);
                libraMessage += pBtl.CurrentMp;
                libraMessage += FF9TextTool.BattleLibraText(13);
                libraMessage += pBtl.MaximumMp;
            }
            if ((infos & LibraInformation.HPMP) != 0)
                libraMessage += "\n\n";
            if ((infos & LibraInformation.Category) != 0 && !pBtl.IsPlayer)
            {
                Int32 enemyCategory = pBtl.Enemy.Data.et.category;
                for (Int32 category = 0; category < 8; category++)
                    if ((enemyCategory & (1 << category)) != 0)
                        libraMessage += FF9TextTool.BattleLibraText(category) + "\n";
            }
            if ((infos & LibraInformation.ElementWeak) != 0)
            {
                EffectElement weakElement = pBtl.WeakElement & ~pBtl.GuardElement;
                for (Int32 element = 0; element < 8; element++)
                    if ((weakElement & (EffectElement)(1 << element)) != 0)
                        libraMessage += (Localization.GetSymbol() != "JP" ? FF9TextTool.BattleLibraText(14 + element) : BtlGetAttrName(1 << element) + FF9TextTool.BattleLibraText(14)) + "\n";
            }
            if ((infos & LibraInformation.ItemSteal) != 0 && !pBtl.IsPlayer)
            {
                BattleEnemy enemy = pBtl.Enemy;
                for (Int32 i = 0; i < enemy.StealableItems.Length; i++)
                    if (enemy.StealableItems[i] != RegularItem.NoItem)
                        libraMessage += (Localization.GetSymbol() != "JP" ? FF9TextTool.BattleLibraText(8) + FF9TextTool.ItemName(enemy.StealableItems[i]) : FF9TextTool.ItemName(enemy.StealableItems[i]) + FF9TextTool.BattleLibraText(8)) + "\n";
            }
            if ((infos & LibraInformation.BlueLearn) != 0 && !pBtl.IsPlayer)
            {
                Int32 blueMagicId = BattleEnemyPrototype.Find(pBtl).BlueMagicId;
                libraMessage += FF9TextTool.CommandName(BattleCommandId.BlueMagic) + (Localization.GetSymbol() == "FR" ? " : " : ": ");
                if (blueMagicId != 0)
                {
                    if (ff9abil.IsAbilityActive(blueMagicId))
                        libraMessage += FF9TextTool.ActionAbilityName(ff9abil.GetActiveAbilityFromAbilityId(blueMagicId));
                    else if (ff9abil.IsAbilitySupport(blueMagicId))
                        libraMessage += FF9TextTool.SupportAbilityName(ff9abil.GetSupportAbilityFromAbilityId(blueMagicId));
                }
                else
                {
                    libraMessage += "-";
                }
                libraMessage += "\n";
            }
            Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
            RenderTexture photoRender = RenderTexture.GetTemporary(Screen.width, Screen.height);
            Vector2 photoSize = new Vector2(Math.Min(Screen.width, 400f), Math.Min(Screen.height, 600f));
            btl2d.GetIconPosition(pBtl.Data, out Byte[] iconBone, out _, out _);
            Vector3 btlPos = pBtl.Data.gameObject.transform.GetChildByName($"bone{iconBone[3]:D3}").position + 50f * Vector3.down;
            camera.ResetWorldToCameraMatrix();
            camera.transform.position = btlPos - 1500f * pBtl.Data.gameObject.transform.forward + 500f * Vector3.up;
            camera.transform.LookAt(btlPos);
            camera.orthographic = true;
            camera.orthographicSize = pBtl.Data.height * (Screen.height / photoSize.y);
            camera.targetTexture = photoRender;
            camera.Render();
            camera.targetTexture = null;
            SFXDataCamera.UpdateCamera();
            RenderTexture.active = photoRender;
            Texture2D photo = new Texture2D((Int32)photoSize.x, (Int32)photoSize.y, TextureFormat.ARGB32, false);
            photo.ReadPixels(new Rect((Screen.width - (Int32)photoSize.x) / 2, (Screen.height - (Int32)photoSize.y) / 2, photoRender.width, photoRender.height), 0, 0);
            photo.Apply();
            RenderTexture.active = null;
            battle.isSpecialTutorialWindow = true;
            _isFromPause = true;
            _beforePauseCommandEnable = _commandEnable;
            _currentButtonGroup = !_hidingHud ? ButtonGroupState.ActiveGroup : _currentButtonGroup;
            FF9BMenu_EnableMenu(false);
            TutorialUI tutorialUI = PersistenSingleton<UIManager>.Instance.TutorialScene;
            tutorialUI.libraTitle = Singleton<HelpDialog>.Instance.PhraseLabel.PhrasePreOpcodeSymbol(pBtl.Enemy.Name, ref additionalWidth);
            tutorialUI.libraMessage = libraMessage;
            tutorialUI.libraPhoto = photo;
            tutorialUI.DisplayMode = TutorialUI.Mode.Libra;
            PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Tutorial);
        }
        else
        {
            _currentLibraMessageNumber = 1;
            _libraBtlData = pBtl;
            _libraEnabledMessage = infos;
            DisplayMessageLibra();
        }
    }

    public void SetBattlePeeping(BattleUnit pBtl, Boolean reverseOrder = true)
    {
        if (pBtl.IsPlayer)
            return;

        _peepingEnmData = pBtl.Enemy;

        Boolean flag = false;
        for (Int32 index = 0; index < 4; ++index)
        {
            if (_peepingEnmData.StealableItems[index] == RegularItem.NoItem)
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
            _currentPeepingReverseOrder = reverseOrder;
            _currentPeepingMessageCount = 1;
            DisplayMessagePeeping();
        }
    }

    public void SetBattleTitle(CMD_DATA cmd, String str, Byte strPriority)
    {
        Message asMessage = new Message()
        {
            message = str,
            priority = strPriority,
            counter = 0f,
            isRect = true,
            titleCmd = cmd
        };
        _messageQueue[str] = asMessage;

        if (_currentMessagePriority > strPriority)
            return;

        _currentMessagePriority = strPriority;
        _battleMessageCounter = 0.0f;
        DisplayBattleMessage(asMessage);
    }

    public void SetBattleMessage(String str, Byte strPriority)
    {
        Message asMessage = new Message()
        {
            message = str,
            priority = strPriority,
            counter = 0f,
            isRect = false,
            titleCmd = null
        };
        _messageQueue[str] = asMessage;

        if (_currentMessagePriority > strPriority)
            return;

        _currentMessagePriority = strPriority;
        _battleMessageCounter = 0.0f;
        DisplayBattleMessage(asMessage);
    }

    public Boolean IsMessageQueued(String str)
    {
        return _messageQueue.ContainsKey(str);
    }

    public Single GetQueuedMessageCounter(String str)
    {
        Message mess;
        if (!_messageQueue.TryGetValue(str, out mess))
            return -1f;
        return mess.counter;
    }

    public Byte GetQueuedMessagePriority(String str)
    {
        Message mess;
        if (!_messageQueue.TryGetValue(str, out mess))
            return 0;
        return mess.priority;
    }

    public void DisplayParty(Boolean resetPointAnimations = false)
    {
        Int32 hudIndex = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;

            Int32 playerId = unit.GetIndex();
            UI.PanelParty.Character character = _partyDetail.Characters[hudIndex];
            DamageAnimationInfo hp = _hpInfoVal[hudIndex];
            DamageAnimationInfo mp = _mpInfoVal[hudIndex];
            if (resetPointAnimations)
            {
                hp.RequiredValue = hp.CurrentValue = (Int32)unit.CurrentHp;
                mp.RequiredValue = mp.CurrentValue = (Int32)unit.CurrentMp;
                hp.FrameLeft = mp.FrameLeft = 0;
                hp.IncrementStep = mp.IncrementStep = 0;
            }
            character.PlayerId = playerId;
            character.ATBBlink = ReadyQueue.Contains(playerId) && !InputFinishList.Contains(playerId);
            character.IsActive = true;
            DisplayCharacterParameter(character, unit, hp, mp);
            character.TranceBar.IsActive = unit.HasTrance;
            hudIndex++;
        }
        while (hudIndex < _partyDetail.Characters.Count)
        {
            _partyDetail.Characters[hudIndex].IsActive = false;
            _partyDetail.Characters[hudIndex].PlayerId = -1;
            hudIndex++;
        }
        UpdateUserInterface();
    }

    public void UpdateUserInterface(Boolean forceUpdate = false)
    {
        Int32 partyCount = FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits().Count(unit => unit.IsPlayer);
        if (partyCount == _playerDetailCount && !forceUpdate)
            return;
        _playerDetailCount = partyCount;
        var hp = _statusPanel.HP;
        var mp = _statusPanel.MP;
        var good = _statusPanel.GoodStatus;
        var bad = _statusPanel.BadStatus;
        if (!Configuration.Interface.IsEnabled)
        {
            Single detailY = DefaultPartyPanelPosY - PartyItemHeight * (_partyDetail.Characters.Count - partyCount);
            _partyDetail.Transform.SetY(detailY);
            hp.Transform.SetY(detailY);
            mp.Transform.SetY(detailY);
            good.Transform.SetY(detailY);
            bad.Transform.SetY(detailY);
            hp.Caption.Content.Transform.localScale = new Vector3(1f, 0.25f * partyCount, 1f);
            mp.Caption.Content.Transform.localScale = new Vector3(1f, 0.25f * partyCount, 1f);
            good.Caption.Content.Transform.localScale = new Vector3(1f, 0.25f * partyCount, 1f);
            bad.Caption.Content.Transform.localScale = new Vector3(1f, 0.25f * partyCount, 1f);
            return;
        }

        Vector2 menuPos = Configuration.Interface.BattleMenuPos;
        Vector2 menuSize = Configuration.Interface.BattleMenuSize;
        Vector2 detailPos = Configuration.Interface.BattleDetailPos;
        Vector2 detailSize = Configuration.Interface.BattleDetailSize;
        Single partyItemHeight = detailSize.y / _partyDetail.Characters.Count;
        detailPos.y -= partyItemHeight * (_partyDetail.Characters.Count - partyCount);
        Int32 columnPerPage = Configuration.Interface.BattleColumnCount;
        Int32 linePerPage = Configuration.Interface.BattleRowCount;
        Int32 lineHeight = (Int32)Math.Round(menuSize.y / linePerPage);
        Single lineFontFactor = (1f + (linePerPage - 5) * 0.15f) / columnPerPage;
        Single cellWidth = (menuSize.x + 32f) / columnPerPage;
        _partyDetail.Widget.SetRawRect(detailPos.x, detailPos.y, detailSize.x, detailSize.y);
        _partyDetail.Captions.Widget.topAnchor.absolute = 20;
        _partyDetail.Captions.Name.Label.SetAnchor(target: _partyDetail.Captions.Transform, relLeft: 0.01f, relRight: 0.38f, relBottom: 1f, bottom: -20f, top: 20f);
        _partyDetail.Captions.HP.Label.SetAnchor(target: _partyDetail.Captions.Transform, relLeft: 0.4f, relRight: 0.565f, relBottom: 1f, bottom: -20f, top: 20f);
        _partyDetail.Captions.MP.Label.SetAnchor(target: _partyDetail.Captions.Transform, relLeft: 0.59f, relRight: 0.71f, relBottom: 1f, bottom: -20f, top: 20f);
        _partyDetail.Captions.ATB.Label.SetAnchor(target: _partyDetail.Captions.Transform, relLeft: 0.725f, relRight: 1f, relBottom: 1f, bottom: -20f, top: 20f);
        for (Int32 i = 0; i < _partyDetail.Characters.Count; i++)
        {
            var charDetail = _partyDetail.Characters[i];
            if (charDetail.PlayerId < 0)
                continue;
            charDetail.Transform.parent = _partyDetail.Transform;
            charDetail.Widget.SetAnchor(target: _partyDetail.Transform, relBottom: 1f, bottom: (i + 1) * -partyItemHeight, top: i * -partyItemHeight);
            charDetail.Name.Label.SetAnchor(target: charDetail.Transform, relLeft: 0.01f, relRight: 0.38f);
            charDetail.HP.Label.SetAnchor(target: charDetail.Transform, relLeft: 0.4f, relRight: 0.565f);
            charDetail.MP.Label.SetAnchor(target: charDetail.Transform, relLeft: 0.59f, relRight: 0.71f);
            charDetail.ATBBar.Sprite.SetAnchor(target: charDetail.Transform, relLeft: 0.725f, relRight: 0.977f, relBottom: 0.55f, relTop: 0.78f);
            charDetail.TranceBar.Sprite.SetAnchor(target: charDetail.Transform, relLeft: 0.748f, relRight: 1f, relBottom: 0.28f, relTop: 0.52f);
        }
        _statusPanel.Transform.SetXY(detailPos.x, detailPos.y);
        foreach (var statusSubPanel in new[] { hp, mp })
        {
            statusSubPanel.Widget.SetAnchor(target: null);
            statusSubPanel.Widget.SetRawRect(0f, 0f, detailSize.x, detailSize.y);
            statusSubPanel.Caption.Widget.SetAnchor(target: statusSubPanel.Transform, relLeft: 0.4f);
            statusSubPanel.Caption.Body.Sprite.SetAnchor(target: statusSubPanel.Caption.Transform, relBottom: 1f - (Single)partyCount / _partyDetail.Characters.Count);
            statusSubPanel.Caption.Content.Label.SetAnchor(target: statusSubPanel.Caption.Transform, relBottom: 1f, bottom: -13f, top: 50f);
            for (Int32 i = 0; i < statusSubPanel.Array.Count; i++)
            {
                var statusDetail = statusSubPanel.Array[i];
                statusDetail.Transform.parent = statusSubPanel.Transform;
                statusDetail.Widget.SetAnchor(target: statusSubPanel.Transform, relLeft: 0.4f, relBottom: 1f, bottom: (i + 1) * -partyItemHeight, top: i * -partyItemHeight);
                statusDetail.Value.Label.SetAnchor(target: statusDetail.Transform, relLeft: 0.04f, relRight: 0.32f);
                statusDetail.Slash.Label.SetAnchor(target: statusDetail.Transform, relLeft: 0.32f, relRight: 0.38f);
                statusDetail.MaxValue.Label.SetAnchor(target: statusDetail.Transform, relLeft: 0.38f, relRight: 0.64f);
                statusDetail.Background.Widget.SetAnchor(target: statusDetail.Transform);
                statusDetail.Background.Border.Sprite.SetAnchor(target: statusDetail.Background.Transform);
            }
        }
        foreach (var statusSubPanel in new[] { good, bad })
        {
            statusSubPanel.Widget.SetAnchor(target: null);
            statusSubPanel.Widget.SetRawRect(0f, 0f, detailSize.x, detailSize.y);
            statusSubPanel.Caption.Widget.SetAnchor(target: statusSubPanel.Transform, relLeft: 0.4f);
            statusSubPanel.Caption.Body.Sprite.SetAnchor(target: statusSubPanel.Caption.Transform, relBottom: 1f - (Single)partyCount / _partyDetail.Characters.Count);
            statusSubPanel.Caption.Content.Label.SetAnchor(target: statusSubPanel.Caption.Transform, relBottom: 1f, bottom: -13f, top: 50f);
            for (Int32 i = 0; i < statusSubPanel.Array.Count; i++)
            {
                var statusDetail = statusSubPanel.Array[i];
                statusDetail.Transform.parent = statusSubPanel.Transform;
                statusDetail.Widget.SetAnchor(target: statusSubPanel.Transform, relLeft: 0.4f, relBottom: 1f, bottom: (i + 1) * -partyItemHeight, top: i * -partyItemHeight);
                for (Int32 j = 0; j < statusDetail.Icons.Count; j++)
                    statusDetail.Icons[j].Sprite.SetAnchor(target: statusDetail.Transform, relLeft: (Single)j / statusDetail.Icons.Count, relRight: (Single)(j + 1) / statusDetail.Icons.Count);
                statusDetail.Background.Widget.SetAnchor(target: statusDetail.Transform);
                statusDetail.Background.Border.Sprite.SetAnchor(target: statusDetail.Background.Transform);
            }
        }
        _commandPanel.Widget.SetAnchor(target: null);
        if (Configuration.Interface.PSXBattleMenu)
            _commandPanel.Widget.SetRawRect(menuPos.x - menuSize.x * 0.25f, menuPos.y, menuSize.x * 0.5f, menuSize.y);
        else
            _commandPanel.Widget.SetRawRect(menuPos.x, menuPos.y, menuSize.x, menuSize.y);
        _commandPanel.Caption.Border.Sprite.SetAnchor(target: _commandPanel.Caption.Transform, left: -3f, bottom: -3f, right: 3f, top: 3f);
        Transform commandPanelTransform = _commandPanel.Transform;
        Boolean hasAccessMenuButton = _commandPanel.AccessMenu != null && Configuration.Battle.AccessMenus > 0;
        Boolean wrapMenus = Configuration.Interface.PSXBattleMenu && Configuration.Control.WrapSomeMenus;
        Single menuButtonHeight;
        if (Configuration.Interface.PSXBattleMenu)
        {
            Single buttonHeight = hasAccessMenuButton ? 0.2f : 0.25f;
            menuButtonHeight = buttonHeight * menuSize.y;
            _commandPanel.Change.KeyNavigation.constraint = UIKeyNavigation.Constraint.Vertical;
            _commandPanel.Defend.KeyNavigation.constraint = UIKeyNavigation.Constraint.Vertical;
            _commandPanel.Change.IsActive = _buttonSliding == _commandPanel.Change;
            _commandPanel.Defend.IsActive = _buttonSliding == _commandPanel.Defend;
            if (_commandPanel.AccessMenu != null)
                _commandPanel.AccessMenu.IsActive = hasAccessMenuButton;
            _commandPanel.Attack.Widget.SetAnchor(target: commandPanelTransform, relBottom: 1f - buttonHeight, relTop: 1f);
            _commandPanel.Skill1.Widget.SetAnchor(target: commandPanelTransform, relBottom: 1f - 2f * buttonHeight, relTop: 1f - buttonHeight);
            _commandPanel.Skill2.Widget.SetAnchor(target: commandPanelTransform, relBottom: 1f - 3f * buttonHeight, relTop: 1f - 2f * buttonHeight);
            _commandPanel.Item.Widget.SetAnchor(target: commandPanelTransform, relBottom: 1f - 4f * buttonHeight, relTop: 1f - 3f * buttonHeight);
            if (hasAccessMenuButton)
                _commandPanel.AccessMenu.Widget.SetAnchor(target: commandPanelTransform, relBottom: 0f, relTop: buttonHeight);
        }
        else
        {
            menuButtonHeight = menuSize.y / 3f;
            _commandPanel.Defend.KeyNavigation.constraint = UIKeyNavigation.Constraint.None;
            _commandPanel.Change.KeyNavigation.constraint = UIKeyNavigation.Constraint.None;
            _commandPanel.Change.IsActive = true;
            _commandPanel.Defend.IsActive = true;
            if (_commandPanel.AccessMenu != null)
                _commandPanel.AccessMenu.IsActive = false;
            _commandPanel.Attack.Widget.SetAnchor(target: commandPanelTransform, relBottom: 0.6666667f, relTop: 1f, relLeft: 0f, relRight: 0.5f);
            _commandPanel.Defend.Widget.SetAnchor(target: commandPanelTransform, relBottom: 0.6666667f, relTop: 1f, relLeft: 0.5f, relRight: 1f);
            _commandPanel.Skill1.Widget.SetAnchor(target: commandPanelTransform, relBottom: 0.3333333f, relTop: 0.6666667f, relLeft: 0f, relRight: 0.5f);
            _commandPanel.Skill2.Widget.SetAnchor(target: commandPanelTransform, relBottom: 0.3333333f, relTop: 0.6666667f, relLeft: 0.5f, relRight: 1f);
            _commandPanel.Item.Widget.SetAnchor(target: commandPanelTransform, relBottom: 0f, relTop: 0.3333333f, relLeft: 0f, relRight: 0.5f);
            _commandPanel.Change.Widget.SetAnchor(target: commandPanelTransform, relBottom: 0f, relTop: 0.3333333f, relLeft: 0.5f, relRight: 1f);
        }
        foreach (GONavigationButton button in _commandPanel.EnumerateButtons(hasAccessMenuButton))
        {
            button.KeyNavigation.wrapUpDown = wrapMenus;
            button.Name.Label.fontSize = (Int32)Math.Round(36f * menuButtonHeight / 79f);
        }
        _abilityPanel.Widget.SetAnchor(target: null);
        _abilityPanel.Widget.SetRawRect(menuPos.x - 1f, menuPos.y - 8f, cellWidth * columnPerPage, lineHeight * linePerPage);
        _abilityPanel.SubPanel.ChangeDims(columnPerPage, linePerPage, cellWidth, lineHeight);
        _abilityPanel.Background.Panel.Name.Label.SetAnchor(target: _abilityPanel.Background.Transform, relLeft: 0.1f, relBottom: 1f, bottom: -5f, top: 20f);
        _abilityPanel.Background.Panel.Info.Label.SetAnchor(target: _abilityPanel.Background.Transform, relRight: 0.9f, relBottom: 1f, bottom: -5f, top: 20f);
        _abilityPanel.SubPanel.ButtonPrefab.NameLabel.SetAnchor(target: _abilityPanel.SubPanel.ButtonPrefab.Transform, relLeft: 0.12f, relRight: 0.9f);
        _abilityPanel.SubPanel.ButtonPrefab.NumberLabel.SetAnchor(target: _abilityPanel.SubPanel.ButtonPrefab.Transform, relLeft: 0f, relRight: 0.9f);
        _abilityPanel.SubPanel.ButtonPrefab.NameLabel.fontSize = (Int32)Math.Round(32f * lineHeight / 50f * lineFontFactor);
        _abilityPanel.SubPanel.ButtonPrefab.NumberLabel.fontSize = (Int32)Math.Round(32f * lineHeight / 50f * lineFontFactor);
        _itemPanel.Widget.SetAnchor(target: null);
        _itemPanel.Widget.SetRawRect(menuPos.x - 1f, menuPos.y - 8f, cellWidth * columnPerPage, lineHeight * linePerPage);
        _itemPanel.SubPanel.ChangeDims(columnPerPage, linePerPage, cellWidth, lineHeight);
        _itemPanel.Background.Panel.Name.Label.SetAnchor(target: _itemPanel.Background.Transform, relLeft: 0.1f, relBottom: 1f, bottom: -5f, top: 20f);
        _itemPanel.Background.Panel.Info.Label.SetAnchor(target: _itemPanel.Background.Transform, relRight: 0.9f, relBottom: 1f, bottom: -5f, top: 20f);
        _itemPanel.SubPanel.ButtonPrefab.IconSprite.SetAnchor(target: _itemPanel.SubPanel.ButtonPrefab.Transform, relLeft: 0.1f, relRight: 0.1f, right: lineHeight);
        _itemPanel.SubPanel.ButtonPrefab.NameLabel.SetAnchor(target: _itemPanel.SubPanel.ButtonPrefab.Transform, relLeft: 0.1f, relRight: 0.9f, left: lineHeight);
        _itemPanel.SubPanel.ButtonPrefab.NumberLabel.SetAnchor(target: _itemPanel.SubPanel.ButtonPrefab.Transform, relRight: 0.9f);
        _itemPanel.SubPanel.ButtonPrefab.NameLabel.fontSize = (Int32)Math.Round(32f * lineHeight / 50f * lineFontFactor);
        _itemPanel.SubPanel.ButtonPrefab.NumberLabel.fontSize = (Int32)Math.Round(32f * lineHeight / 50f * lineFontFactor);
        _targetPanel.Widget.SetAnchor(target: null);
        _targetPanel.Widget.SetRawRect(menuPos.x, menuPos.y, menuSize.x, menuSize.y);
        _targetPanel.Captions.Border.Sprite.SetAnchor(target: _targetPanel.Captions.Transform, left: -3f, bottom: -3f, right: 3f, top: 3f);
        _targetPanel.Captions.Caption1.Label.SetAnchor(target: _targetPanel.Captions.Transform, relLeft: 0.075f, relRight: 0.5f, relBottom: 1f, bottom: -26f, top: 14f);
        _targetPanel.Captions.Caption2.Label.SetAnchor(target: _targetPanel.Captions.Transform, relLeft: 0.585f, relRight: 1f, relBottom: 1f, bottom: -26f, top: 14f);
        Transform targetPanelTransform = _targetPanel.Transform;
        for (Int32 entryIndex = 0; entryIndex < _targetPanel.Enemies.Count; entryIndex++)
        {
            if (!_targetPanel.Enemies[entryIndex].IsActive)
                continue;
            _targetPanel.Enemies[entryIndex].Widget.SetAnchor(target: targetPanelTransform, relBottom: 0.75f - entryIndex * 0.25f, relTop: 1f - entryIndex * 0.25f, relLeft: 0f, relRight: 0.525f);
            _targetPanel.Enemies[entryIndex].Name.Label.fontSize = (Int32)Math.Round(32f * menuSize.y / 228f);
        }
        for (Int32 entryIndex = 0; entryIndex < _targetPanel.Players.Count; entryIndex++)
        {
            if (!_targetPanel.Players[entryIndex].IsActive)
                continue;
            _targetPanel.Players[entryIndex].Widget.SetAnchor(target: targetPanelTransform, relBottom: 0.75f - entryIndex * 0.25f, relTop: 1f - entryIndex * 0.25f, relLeft: 0.525f, relRight: 1f);
            _targetPanel.Players[entryIndex].Highlight.Sprite.SetAnchor(target: _targetPanel.Players[entryIndex].Transform);
            _targetPanel.Players[entryIndex].Background.Widget.SetAnchor(target: _targetPanel.Players[entryIndex].Transform);
            _targetPanel.Players[entryIndex].Background.Border.Sprite.SetAnchor(target: _targetPanel.Players[entryIndex].Background.Transform);
            _targetPanel.Players[entryIndex].Name.Label.fontSize = (Int32)Math.Round(32f * menuSize.y / 228f);
        }
        ButtonGroupState.SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior.None, CommandGroupButton);
        ButtonGroupState.SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior.None, TargetGroupButton);
        ButtonGroupState.SetPointerLimitRectToGroup(_abilityPanel.Widget, lineHeight, AbilityGroupButton);
        ButtonGroupState.SetPointerLimitRectToGroup(_itemPanel.Widget, lineHeight, ItemGroupButton);
        if (Singleton<HelpDialog>.Instance.IsShown)
            Singleton<HelpDialog>.Instance.ShowDialog();
    }

    public void AddPlayerToReady(Int32 playerId)
    {
        if (_unconsciousStateList.Contains(playerId))
            return;

        // ATB bar is full
        ReadyQueue.Add(playerId);
        _partyDetail.GetCharacter(playerId).ATBBlink = true;
    }

    public void RemovePlayerFromAction(Int32 btlId, Boolean isNeedToClearCommand)
    {
        if (!isNeedToClearCommand)
            return;

        Int32 btlIndex = 0;
        while (1 << btlIndex != btlId)
            ++btlIndex;

        InputFinishList.Remove(btlIndex);
        ReadyQueue.Remove(btlIndex);
        _partyDetail.SetBlink(btlIndex, false);
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
                gil += btl_util.getEnemyPtr(next).bonus_gil;
        if (FF9StateSystem.Common.FF9.btl_result == 4)
            btl_sys.ClearBattleBonus();
        for (Int32 i = 0; i < 4; i++)
        {
            PLAYER player = FF9StateSystem.Common.FF9.party.member[i];
            if (player != null)
                foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(player))
                    saFeature.TriggerOnBattleResult(player, battle.btl_bonus, new List<FF9ITEM>(), "BattleEnd", gil / 10U);
        }
        if (FF9StateSystem.Common.FF9.btl_result == 4 && (battle.btl_bonus.gil != 0 || battle.btl_bonus.exp != 0 || battle.btl_bonus.ap != 0 || battle.btl_bonus.card != TetraMasterCardId.NONE))
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

    public Boolean FF9BMenu_IsEnableAtb()
    {
        if (!IsNativeEnableAtb())
            return false;

        if (Configuration.Battle.Speed != 2 || FF9StateSystem.Battle.FF9Battle.btl_escape_key != 0)
            return true;

        // Stops the ATB if any of these are true
        Boolean isMenuing = _commandPanel.IsActive || _targetPanel.IsActive || _itemPanel.IsActive || _abilityPanel.IsActive;
        Boolean isEnemyActing = FF9StateSystem.Battle.FF9Battle.cur_cmd != null && FF9StateSystem.Battle.FF9Battle.cur_cmd.regist?.bi.player == 0;
        Boolean hasQueue = btl_cmd.GetFirstCommandReadyToDequeue(FF9StateSystem.Battle.FF9Battle) != null;
        return !(isMenuing || hasQueue || isEnemyActing);
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

    public void ItemRequest(RegularItem id)
    {
        _needItemUpdate = true;
    }

    public void ItemUse(RegularItem id)
    {
        if (ff9item.FF9Item_Remove(id, 1) == 0)
            return;
        _needItemUpdate = true;
    }

    public void ItemUnuse(RegularItem id)
    {
        _needItemUpdate = true;
    }

    public void ItemRemove(RegularItem id)
    {
        if (ff9item.FF9Item_Remove(id, 1) == 0)
            return;
        _needItemUpdate = true;
    }

    public void ItemAdd(RegularItem id)
    {
        if (ff9item.FF9Item_Add(id, 1) == 0)
            return;
        _needItemUpdate = true;
    }

    public void SwitchAvailablePlayerOrIdle()
    {
        if (ReadyQueue.Count > 0)
        {
            for (Int32 i = 0; i < ReadyQueue.Count; i++)
            {
                Int32 current = ReadyQueue[i];
                if (!InputFinishList.Contains(current) && !_unconsciousStateList.Contains(current))
                {
                    if (ReadyQueue.IndexOf(current) > 0)
                    {
                        ReadyQueue.Remove(current);
                        ReadyQueue.Insert(0, current);
                    }
                    SwitchPlayer(current);
                    return;
                }
            }
        }
        SetIdle();
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

    public Boolean IsAbilityAvailable(BattleUnit unit, Int32 abilId)
	{
        if (!unit.IsPlayer)
            return true;
        return GetAbilityState(abilId, unit.GetIndex()) == AbilityStatus.Enable;
    }
}

