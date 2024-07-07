using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class BattleResultUI : UIScene
{
    public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
        {
            this.screenFadePanel.depth = 0;
        };
        if (afterFinished != null)
        {
            sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        }
        base.Show(sceneVoidDelegate);
        SceneDirector.FF9Wipe_FadeInEx(12);
        PersistenSingleton<UIManager>.Instance.SetGameCameraEnable(false);
        this.isTimerDisplay = TimerUI.GetDisplay();
        if (FF9StateSystem.Common.FF9.btl_result == 4)
        {
            if (battle.btl_bonus.escape_gil)
            {
                this.InitialNormal();
            }
            else
            {
                this.InitialNone();
            }
        }
        else if (FF9StateSystem.Common.FF9.btl_result != 7)
        {
            if (battle.btl_bonus.Event)
            {
                this.InitialEvent();
            }
            else
            {
                this.InitialNormal();
            }
        }
        else
        {
            this.InitialNone();
        }
    }

    public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
        {
            SceneDirector.FF9Wipe_FadeInEx(256);
            battle.ff9ShutdownStateBattleResult();
            this.ItemListPanel.SetActive(false);
            PersistenSingleton<FF9StateSystem>.Instance.mode = PersistenSingleton<FF9StateSystem>.Instance.prevMode;
            Byte mode = PersistenSingleton<FF9StateSystem>.Instance.mode;
            if (mode == 3)
            {
                SceneDirector.Replace("WorldMap", SceneTransition.FadeOutToBlack, true);
            }
            else if (mode == 5 || mode == 1)
            {
                SceneDirector.Replace("FieldMap", SceneTransition.FadeOutToBlack, true);
            }
            if (this.isTimerDisplay && TimerUI.Enable)
            {
                TimerUI.SetDisplay(true);
            }
        };
        if (afterFinished != null)
        {
            sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        }
        base.Hide(sceneVoidDelegate);
        SceneDirector.FF9Wipe_FadeInEx(12);
        this.screenFadePanel.depth = 5;
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go))
        {
            switch (this.currentState)
            {
                case BattleResultUI.ResultState.Start:
                    FF9Sfx.FF9SFX_PlayLoop(105);
                    this.currentState = BattleResultUI.ResultState.EXPAndAPTick;
                    break;
                case BattleResultUI.ResultState.EXPAndAPTick:
                {
                    FF9Sfx.FF9SFX_StopLoop(105);
                    this.currentState = BattleResultUI.ResultState.EndEXPAndAP;
                    for (Int32 i = 0; i < this.expValue.Length; i++)
                    {
                        BattleEndValue endValue = this.expValue[i];
                        endValue.step = endValue.value;
                    }
                    this.UpdateExp();
                    for (Int32 i = 0; i < this.apValue.Length; i++)
                    {
                        BattleEndValue endValue = this.apValue[i];
                        endValue.step = endValue.value;
                    }
                    this.UpdateAp();
                    this.DisplayEXPAndAPInfo();
                    this.DisplayCharacterInfo();
                    this.ApplyTweenAndFade();
                    break;
                }
                case BattleResultUI.ResultState.EndEXPAndAP:
                    this.ApplyTweenAndFade();
                    break;
                case BattleResultUI.ResultState.StartGilAndItem:
                    if (!this.UpdateItem())
                    {
                        this.ItemOverflowPanelTween.TweenIn((Action)null);
                        this.currentState = BattleResultUI.ResultState.ItemFullDialog;
                    }
                    else if (this.gilValue.value == 0u)
                    {
                        this.currentState = BattleResultUI.ResultState.End;
                    }
                    else
                    {
                        FF9Sfx.FF9SFX_PlayLoop(109);
                        this.currentState = BattleResultUI.ResultState.GilTick;
                    }
                    break;
                case BattleResultUI.ResultState.ItemFullDialog:
                    this.ItemOverflowPanelTween.TweenOut(delegate
                    {
                        if (this.gilValue.value == 0u)
                        {
                            this.currentState = BattleResultUI.ResultState.End;
                        }
                        else
                        {
                            FF9Sfx.FF9SFX_PlayLoop(109);
                            this.currentState = BattleResultUI.ResultState.GilTick;
                        }
                    });
                    break;
                case BattleResultUI.ResultState.GilTick:
                    this.currentState = BattleResultUI.ResultState.End;
                    this.gilValue.step = this.gilValue.value;
                    FF9Sfx.FF9SFX_StopLoop(109);
                    this.UpdateGil();
                    this.DisplayGilAndItemInfo();
                    break;
                case BattleResultUI.ResultState.End:
                    this.currentState = BattleResultUI.ResultState.Hide;
                    this.Hide((UIScene.SceneVoidDelegate)null);
                    break;
            }
        }
        return true;
    }

    public override void OnKeyQuit()
    {
        if (this.currentState != BattleResultUI.ResultState.Hide && !base.Loading)
        {
            base.OnKeyQuit();
        }
    }

    private void DisplayEXPAndAPInfo()
    {
        this.EXPAndAPPhrasePanel.SetActive(true);
        this.GilAndItemPhrasePanel.SetActive(false);
        this.expReceiveLabel.text = this.defaultExp.ToString();
        this.apReceiveLabel.text = this.defaultAp.ToString();
        FF9UIDataTool.DisplayTextLocalize(this.infoLabelGameObject, "BattleResultInfoEXPAP");
    }

    private void DisplayGilAndItemInfo()
    {
        this.EXPAndAPPhrasePanel.SetActive(false);
        this.GilAndItemPhrasePanel.SetActive(true);
        this.receiveGilLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", (this.gilValue.value - this.gilValue.current).ToString());
        this.currentGilLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", Mathf.Min(FF9StateSystem.Common.FF9.party.gil, 9999999f).ToString());
        FF9UIDataTool.DisplayTextLocalize(this.infoLabelGameObject, "BattleResultInfoGilItem");
        if (this.itemList.Count > 0)
        {
            this.ItemDetailPanel.SetActive(true);
            this.noItemLabel.SetActive(false);
            Int32 num = 0;
            foreach (FF9ITEM ff9ITEM in this.itemList)
            {
                ItemListDetailWithIconHUD itemListDetailWithIconHUD = this.itemHudList[num];
                itemListDetailWithIconHUD.Self.SetActive(true);
                FF9UIDataTool.DisplayItem(ff9ITEM.id, itemListDetailWithIconHUD.IconSprite, itemListDetailWithIconHUD.NameLabel, true);
                itemListDetailWithIconHUD.NumberLabel.text = ff9ITEM.count.ToString();
                num++;
            }
        }
        else
        {
            this.ItemDetailPanel.SetActive(false);
            this.noItemLabel.SetActive(true);
        }
        if (this.defaultCard != TetraMasterCardId.NONE)
        {
            this.ItemDetailPanel.SetActive(true);
            this.cardHud.Self.SetActive(true);
            this.cardHud.NameLabel.text = FF9TextTool.CardName(this.defaultCard);
        }
    }

    private void DisplayCharacterInfo()
    {
        for (Int32 i = 0; i < 4; i++)
        {
            PLAYER player = FF9StateSystem.Common.FF9.party.member[i];
            if (player != null)
            {
                UInt64 nextLvl = (player.level >= ff9level.LEVEL_COUNT) ? player.exp : ff9level.CharacterLevelUps[player.level].ExperienceToLevel;
                BattleResultUI.CharacterBattleResultInfoHUD characterBattleResultInfoHUD = this.characterBRInfoHudList[i];
                characterBattleResultInfoHUD.Content.SetActive(true);
                characterBattleResultInfoHUD.NameLabel.text = player.Name;
                characterBattleResultInfoHUD.LevelLabel.text = player.level.ToString();
                characterBattleResultInfoHUD.ExpLabel.text = player.exp.ToString();
                characterBattleResultInfoHUD.NextLvLabel.text = (nextLvl - player.exp).ToString();
                FF9UIDataTool.DisplayCharacterAvatar(player, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), characterBattleResultInfoHUD.AvatarSprite, false);
                UISprite[] statusesSpriteList = characterBattleResultInfoHUD.StatusesSpriteList;
                for (Int32 j = 0; j < statusesSpriteList.Length; j++)
                    statusesSpriteList[j].alpha = 0f;
                Int32 spriteSlot = 0;
                foreach (KeyValuePair<BattleStatus, Byte> kvp in BattleResultUI.BadIconDict)
                {
                    if (spriteSlot >= characterBattleResultInfoHUD.StatusesSpriteList.Length)
                        break;
                    if ((player.status & kvp.Key) != 0u)
                    {
                        characterBattleResultInfoHUD.StatusesSpriteList[spriteSlot].alpha = 1f;
                        characterBattleResultInfoHUD.StatusesSpriteList[spriteSlot].spriteName = FF9UIDataTool.IconSpriteName[kvp.Value];
                        spriteSlot++;
                    }
                }
                if (!this.IsEnableDraw(player, i))
                {
                    this.isRecieveExpList.Add(false);
                    characterBattleResultInfoHUD.DimPanel.SetActive(true);
                    characterBattleResultInfoHUD.AvatarSprite.alpha = 0.5f;
                }
                else
                {
                    this.isRecieveExpList.Add(true);
                    characterBattleResultInfoHUD.DimPanel.SetActive(false);
                    characterBattleResultInfoHUD.AvatarSprite.alpha = 1f;
                }
            }
        }
    }

    private void DisplayLevelup(Int32 index)
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[index];
        ff9play.FF9Play_GrowLevel(player, player.level + 1);
        this.totalLevelUp[index]++;
        this.ShowLevelUpAnimation(index);
    }

    private void ShowLevelUpAnimation(Int32 index)
    {
        if (this.finishedLevelUpAnimation[index] >= this.totalLevelUp[index])
            return;
        if (!this.isLevelUpSoundPlayed)
            FF9Sfx.FF9SFX_Play(683);
        this.isLevelUpSoundPlayed = true;
        this.levelUpSpriteTween[index].TweenIn(new Byte[1], delegate
        {
            this.levelUpSpriteTween[index].dialogList[0].SetActive(false);
            this.StartCoroutine(this.WaitAndPlayNextAnimation(index));
        });
    }

    private IEnumerator WaitAndPlayNextAnimation(Int32 index)
    {
        yield return new WaitForSeconds(0.1f);
        this.finishedLevelUpAnimation[index]++;
        this.ShowLevelUpAnimation(index);
        yield break;
    }

    private void ClearUI()
    {
        foreach (BattleResultUI.CharacterBattleResultInfoHUD characterBattleResultInfoHUD in this.characterBRInfoHudList)
            characterBattleResultInfoHUD.Content.SetActive(false);
        foreach (ItemListDetailWithIconHUD itemListDetailWithIconHUD in this.itemHudList)
            itemListDetailWithIconHUD.Self.SetActive(false);
        HonoTweenPosition[] array = this.levelUpSpriteTween;
        foreach (HonoTweenPosition honoTweenPosition in this.levelUpSpriteTween)
            honoTweenPosition.dialogList[0].SetActive(false);
        HonoTweenClipping[] array2 = this.abilityLearnedPanelTween;
        foreach (HonoTweenClipping honoTweenClipping in this.abilityLearnedPanelTween)
            honoTweenClipping.ClipGameObject.SetActive(false);
        this.cardHud.Self.SetActive(false);
    }

    private void AfterShowGilAndItem()
    {
        this.DisplayGilAndItemInfo();
        this.ItemPanelTween.TweenIn(null);
        this.currentState = BattleResultUI.ResultState.StartGilAndItem;
    }

    private void AnalyzeParty()
    {
        this.remainPlayerCounter = 0;
        for (Int32 i = 0; i < 4; i++)
        {
            if ((battle.btl_bonus.member_flag & 1 << i) != 0 && FF9StateSystem.Common.FF9.party.member[i] != null)
            {
                Character player = FF9StateSystem.Common.FF9.party.GetCharacter(i);

                this.isNeedExp[i] = this.IsNeedExp(player);
                this.isNeedAp[i] = this.IsNeedAp(player);
                if (this.isNeedExp[i])
                    this.remainPlayerCounter++;
            }
        }
    }

    private Boolean IsNeedExp(Character play)
    {
        return this.IsEnable(play.Data) && play.Data.exp < 9999999u;
    }

    private Boolean IsNeedAp(Character play)
    {
        return this.IsEnable(play.Data) && ff9abil.FF9Abil_HasAp(play);
    }

    private Boolean IsEnableDraw(PLAYER play, Int32 id)
    {
        return this.IsEnable(play) && (battle.btl_bonus.member_flag & 1 << id) != 0;
    }

    private Boolean IsEnable(PLAYER play)
    {
        return play != null && play.cur.hp > 0 && (play.status & BattleStatusConst.DisableRewards) == 0;
    }

    private void InitialNormal()
    {
        this.AllPanel.SetActive(true);
        for (Int32 i = 0; i < 4; i++)
        {
            this.expValue[i] = new BattleEndValue();
            this.apValue[i] = new BattleEndValue();
            this.isNeedAp[i] = false;
            this.isNeedExp[i] = false;
        }
        this.currentState = BattleResultUI.ResultState.Start;
        this.InitialInfo();
        this.ClearUI();
        this.DisplayEXPAndAPInfo();
        this.DisplayCharacterInfo();
        this.infoPanelTween.TweenIn(new Byte[1], (UIScene.SceneVoidDelegate)null);
        this.helpPanelTween.TweenIn(new Byte[1], (UIScene.SceneVoidDelegate)null);
        this.expLeftSideTween.TweenIn(new Byte[]
        {
            0,
            1,
            2
        }, null);
        this.expRightSideTween.TweenIn(new Byte[]
        {
            0,
            1,
            2
        }, null);
        this.totalLevelUp = new Int32[4];
        this.finishedLevelUpAnimation = new Int32[4];
        this.abilityLearned[0] = new List<Int32>();
        this.abilityLearned[1] = new List<Int32>();
        this.abilityLearned[2] = new List<Int32>();
        this.abilityLearned[3] = new List<Int32>();
        this.isReadyToShowNextAbil = new Boolean[] { true, true, true, true };
        Boolean skipEXPAndAP = true;
        for (Int32 i = 0; i < this.expValue.Length && skipEXPAndAP; i++)
            if (this.expValue[i].value != 0u)
                skipEXPAndAP = false;
        for (Int32 i = 0; i < this.apValue.Length && skipEXPAndAP; i++)
            if (this.apValue[i].value != 0u)
                skipEXPAndAP = false;
        if (!skipEXPAndAP)
            this.currentState = BattleResultUI.ResultState.Start;
        else
            this.currentState = BattleResultUI.ResultState.EndEXPAndAP;
    }

    private void InitialEvent()
    {
        this.currentState = BattleResultUI.ResultState.None;
        FF9StateBattleMap map = FF9StateSystem.Battle.FF9Battle.map;
        if (map.evtPtr != null)
        {
            this.AllPanel.SetActive(false);
            PersistenSingleton<EventEngine>.Instance.StartEvents(map.evtPtr);
            if (this.isTimerDisplay && TimerUI.Enable)
                TimerUI.SetDisplay(false);
        }
    }

    private void InitialNone()
    {
        this.currentState = BattleResultUI.ResultState.None;
        base.Loading = true;
        this.AllPanel.SetActive(false);
        base.StartCoroutine(this.InitialNone_delay());
    }

    private IEnumerator InitialNone_delay()
    {
        this.currentState = BattleResultUI.ResultState.Hide;
        yield return new WaitForSeconds(0.3f);
        base.Loading = false;
        this.Hide(null);
        yield break;
    }

    private void InitialInfo()
    {
        this.AnalyzeBonusItem();
        PLAYER[] party = FF9StateSystem.Common.FF9.party.member;
        HashSet<SupportingAbilityFeature> triggeredSA = new HashSet<SupportingAbilityFeature>();
        for (Int32 playIndex = 0; playIndex < 4; playIndex++)
        {
            if (party[playIndex] == null)
                continue;
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(party[playIndex]))
            {
                if (triggeredSA.Contains(saFeature))
                    continue;
                if (saFeature.TriggerOnBattleResult(party[playIndex], battle.btl_bonus, this.itemList, "RewardAll", 0U))
                    triggeredSA.Add(saFeature);
            }
        }
        this.AnalyzeArgument();
        for (Int32 i = 0; i < 4; i++)
        {
            PLAYER player = party[i];
            if (player != null)
            {
                BONUS individualBonus = new BONUS();
                individualBonus.ap = (UInt16)this.defaultAp;
                individualBonus.card = this.defaultCard;
                individualBonus.exp = this.defaultExp;
                individualBonus.gil = this.defaultGil;
                foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(player))
                    saFeature.TriggerOnBattleResult(player, individualBonus, this.itemList, "RewardSingle", 0U);
                this.defaultCard = individualBonus.card;
                this.defaultGil = individualBonus.gil;
                if (this.isNeedExp[i])
                {
                    BattleEndValue battleEndValue = new BattleEndValue();
                    battleEndValue.value = individualBonus.exp;
                    if (battleEndValue.value < (UInt64)EXPMinTick)
                    {
                        battleEndValue.step = 1u;
                    }
                    else if (battleEndValue.value < (UInt64)(EXPMinTick * EXPDefaultAdd))
                    {
                        battleEndValue.step = (UInt32)((battleEndValue.value + (UInt64)EXPMinTick - 1UL) / (UInt64)EXPMinTick);
                    }
                    else
                    {
                        battleEndValue.step = (UInt32)BattleResultUI.EXPDefaultAdd;
                        UInt32 expTick = battleEndValue.value / battleEndValue.step;
                        if (expTick > BattleResultUI.EXPTickMax)
                            battleEndValue.step = (UInt32)((battleEndValue.value + (UInt64)BattleResultUI.EXPTickMax - 1UL) / (UInt64)BattleResultUI.EXPTickMax);
                    }
                    battleEndValue.current = 0u;
                    this.expValue[i] = battleEndValue;
                }
                if (this.isNeedAp[i])
                {
                    BattleEndValue battleEndValue2 = new BattleEndValue();
                    battleEndValue2.value = individualBonus.ap;
                    battleEndValue2.step = 1u;
                    battleEndValue2.current = 0u;
                    this.apValue[i] = battleEndValue2;
                }
            }
        }
        this.gilValue.value = (UInt32)this.defaultGil;
        if (this.gilValue.value < (UInt64)BattleResultUI.GilCountTick)
            this.gilValue.step = 1u;
        else if ((UInt64)this.gilValue.value < (UInt64)(BattleResultUI.GilCountTick * BattleResultUI.GilDefaultAdd))
            this.gilValue.step = (UInt32)((this.gilValue.value + (UInt64)BattleResultUI.GilCountTick - 1UL) / (UInt64)BattleResultUI.GilCountTick);
        else
            this.gilValue.step = (UInt32)BattleResultUI.GilDefaultAdd;
        this.gilValue.current = 0u;
        this.PostProcessItems();
    }

    public void ShutdownBattleResultUI()
    {
        base.StartCoroutine(this.InitialNone_delay());
    }

    private void AnalyzeBonusItem()
    {
        this.itemList.Clear();
        for (Int32 i = 0; i < battle.btl_bonus.item.Count; i++)
        {
            if (battle.btl_bonus.item[i] != RegularItem.NoItem)
            {
                Boolean hasItem = false;
                for (Int32 j = 0; j < this.itemList.Count; j++)
                {
                    if (battle.btl_bonus.item[i] == this.itemList[j].id)
                    {
                        FF9ITEM ff9ITEM = this.itemList[j];
                        ff9ITEM.count++;
                        hasItem = true;
                        break;
                    }
                }
                if (!hasItem)
                {
                    if (this.itemList.Count >= BattleResultUI.ItemMax)
                        return;
                    FF9ITEM item = new FF9ITEM(battle.btl_bonus.item[i], 1);
                    this.itemList.Add(item);
                }
            }
        }
    }

    private void AnalyzeArgument()
    {
        this.AnalyzeParty();
        this.defaultExp = (UInt32)(this.remainPlayerCounter == 0 ? 0u : battle.btl_bonus.exp / this.remainPlayerCounter);
        this.defaultAp = battle.btl_bonus.ap;
        this.defaultGil = battle.btl_bonus.gil;
        this.defaultCard = QuadMistDatabase.MiniGame_GetAllCardCount() < Configuration.TetraMaster.MaxCardCount ? battle.btl_bonus.card : TetraMasterCardId.NONE;
    }

    private void PostProcessItems()
    {
        for (Int32 i = 0; i < this.itemList.Count; i++)
        {
            for (Int32 j = i + 1; j < this.itemList.Count; j++)
            {
                if (this.itemList[i].id == this.itemList[j].id)
                {
                    this.itemList[i].count = (Byte)Math.Min(this.itemList[i].count + this.itemList[j].count, Byte.MaxValue);
                    this.itemList[j].count = 0;
                }
            }
        }
        this.itemList.RemoveAll(it => it.id == RegularItem.NoItem || it.count == 0);
    }

    private void UpdateExp()
    {
        this.expEndTick = true;
        for (Int32 i = 0; i < 4; i++)
        {
            PLAYER player = FF9StateSystem.Common.FF9.party.member[i];
            BattleEndValue battleEndValue = this.expValue[i];
            if (this.isNeedExp[i])
            {
                if (player != null)
                {
                    UInt32 tickGain = (UInt32)((battleEndValue.current + battleEndValue.step > battleEndValue.value) ? (battleEndValue.value - battleEndValue.current) : battleEndValue.step);
                    if (battleEndValue.current < battleEndValue.value)
                    {
                        this.expEndTick = false;
                        if (tickGain != 0u)
                        {
                            if (9999999u > player.exp)
                            {
                                battleEndValue.current += tickGain;
                                player.exp += tickGain;
                                if (9999999u <= player.exp)
                                    player.exp = 9999999u;
                                if (player.level < ff9level.LEVEL_COUNT)
                                {
                                    for (UInt32 exp = ff9level.CharacterLevelUps[player.level].ExperienceToLevel; exp <= player.exp; exp = ff9level.CharacterLevelUps[player.level].ExperienceToLevel)
                                    {
                                        this.DisplayLevelup(i);
                                        if (player.level >= ff9level.LEVEL_COUNT)
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void UpdateAp()
    {
        this.apEndTick = true;
        for (Int32 i = 0; i < 4; i++)
        {
            if (this.isNeedAp[i])
            {
                BattleEndValue battleEndValue = this.apValue[i];
                if (battleEndValue.current < battleEndValue.value)
                {
                    this.apEndTick = false;
                    UInt32 ap = (UInt32)((battleEndValue.current + battleEndValue.step > battleEndValue.value) ? (battleEndValue.value - battleEndValue.current) : battleEndValue.step);
                    this.AddAp(i, ap);
                }
            }
        }
    }

    private void AddAp(Int32 id, UInt32 ap)
    {
        Character player = FF9StateSystem.Common.FF9.party.GetCharacter(id);
        if (ap == 0u || player == null || !ff9abil.FF9Abil_HasAp(player))
            return;
        this.apValue[id].current += ap;
        for (Int32 i = 0; i < 5; i++)
        {
            if (player.Equipment[i] != RegularItem.NoItem)
            {
                FF9ITEM_DATA itemData = ff9item._FF9Item_Data[player.Equipment[i]];
                foreach (Int32 abil in itemData.ability)
                {
                    if (abil != 0)
                    {
                        Int32 abilIndex = ff9abil.FF9Abil_GetIndex(player.Data, abil);
                        if (abilIndex >= 0)
                        {
                            Int32 max_ap = ff9abil._FF9Abil_PaData[player.PresetId][abilIndex].Ap;
                            Int32 cur_ap = player.Data.pa[abilIndex];
                            if (max_ap > cur_ap)
                            {
                                if (max_ap <= cur_ap + ap)
                                {
                                    player.Data.pa[abilIndex] = (Byte)max_ap;
                                    this.ApLearned(id, abil);
                                }
                                else
                                {
                                    player.Data.pa[abilIndex] += (Byte)ap;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void ApLearned(Int32 id, Int32 abilId)
    {
        if (!this.isAbilityLearnSoundPlayed)
            FF9Sfx.FF9SFX_Play(1043);
        this.isAbilityLearnSoundPlayed = true;
        this.abilityLearned[id].Add(abilId);
        BattleAchievement.UpdateAbilitiesAchievement(abilId, true);
    }

    private void DisplayApWin()
    {
        for (Int32 i = 0; i < 4; i++)
        {
            if (this.isReadyToShowNextAbil[i] && this.abilityLearned[i].Count != 0)
            {
                global::Debug.Log(String.Concat(new Object[]
                {
                    "index: ",
                    i,
                    " abilityLearned[index].Count: ",
                    this.abilityLearned[i].Count
                }));
                this.isReadyToShowNextAbil[i] = false;
                base.StartCoroutine(this.ApDraw(i));
            }
        }
    }

    private IEnumerator ApDraw(Int32 id)
    {
        Int32 abilId = this.abilityLearned[id][0];
        this.abilityLearned[id].RemoveAt(0);
        String abilName;
        String spriteName;
        if (ff9abil.IsAbilityActive(abilId))
        {
            abilName = FF9TextTool.ActionAbilityName(ff9abil.GetActiveAbilityFromAbilityId(abilId));
            spriteName = "ability_stone";
        }
        else
        {
            SupportAbility saIndex = ff9abil.GetSupportAbilityFromAbilityId(abilId);
            abilName = FF9TextTool.SupportAbilityName(saIndex);
            spriteName = ff9abil.FF9Abil_IsEnableSA(FF9StateSystem.Common.FF9.party.member[id], saIndex) ? "skill_stone_on" : "skill_stone_off";
        }
        this.characterBRInfoHudList[id].AbiltySprite.spriteName = spriteName;
        this.characterBRInfoHudList[id].AbilityLabel.text = abilName;
        this.abilityLearnedPanelTween[id].TweenIn(null);
        yield return new WaitForSeconds(1f);
        this.abilityLearnedPanelTween[id].TweenOut(delegate
        {
            this.isReadyToShowNextAbil[id] = true;
        });
        yield break;
    }

    private void UpdateGil()
    {
        Boolean keepUpdate = true;
        if (this.gilValue.current < this.gilValue.value)
        {
            UInt32 gilAdd = (this.gilValue.current + this.gilValue.step > this.gilValue.value) ? (this.gilValue.value - this.gilValue.current) : this.gilValue.step;
            if (gilAdd != 0u)
            {
                keepUpdate = false;
                this.gilValue.current += gilAdd;
                if (9999999u < FF9StateSystem.Common.FF9.party.gil + gilAdd)
                {
                    gilAdd = 9999999u - FF9StateSystem.Common.FF9.party.gil;
                    this.gilValue.current = this.gilValue.value;
                }
                FF9StateSystem.Common.FF9.party.gil += gilAdd;
            }
        }
        if (keepUpdate)
        {
            FF9Sfx.FF9SFX_StopLoop(109);
            this.currentState = BattleResultUI.ResultState.End;
        }
    }

    private Boolean UpdateItem()
    {
        Boolean gainedItem = false;
        for (Int32 i = 0; i < this.itemList.Count; i++)
        {
            if (this.itemList[i].count != ff9item.FF9Item_Add(this.itemList[i].id, this.itemList[i].count))
            {
                FF9Sfx.FF9SFX_Play(1046);
                gainedItem = true;
            }
        }
        if (this.defaultCard != TetraMasterCardId.NONE)
            QuadMistDatabase.MiniGame_SetCard(this.defaultCard);
        if (gainedItem && (this.itemList.Count != 0 || this.defaultCard != TetraMasterCardId.NONE))
            FF9Sfx.FF9SFX_Play(108);
        return !gainedItem;
    }

    private void UpdateState()
    {
        if (this.expEndTick && this.apEndTick)
        {
            for (Int32 i = 0; i < 4; i++)
                if (this.abilityLearned[i].Count != 0)
                    return;
            FF9Sfx.FF9SFX_StopLoop(105);
            FF9Sfx.FF9SFX_Play(103);
            this.currentState = BattleResultUI.ResultState.EndEXPAndAP;
        }
    }

    private void ApplyTweenAndFade()
    {
        this.expLeftSideTween.TweenOut(new Byte[]
        {
            0,
            1,
            2
        }, null);
        this.expRightSideTween.TweenOut(new Byte[]
        {
            0,
            1,
            2
        }, null);
        base.Loading = true;
        base.FadingComponent.FadePingPong(new UIScene.SceneVoidDelegate(this.AfterShowGilAndItem), delegate
        {
            base.Loading = false;
        });
    }

    private void Update()
    {
        if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.BattleResult)
        {
            this.isLevelUpSoundPlayed = false;
            this.isAbilityLearnSoundPlayed = false;
            BattleResultUI.ResultState resultState = this.currentState;
            if (resultState == BattleResultUI.ResultState.GilTick)
            {
                this.UpdateGil();
                this.DisplayGilAndItemInfo();
            }
            else if (resultState == BattleResultUI.ResultState.EXPAndAPTick)
            {
                this.UpdateExp();
                this.UpdateAp();
                this.DisplayApWin();
                this.DisplayCharacterInfo();
                this.UpdateState();
            }
        }
    }

    private void Awake()
    {
        base.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
        foreach (Object obj in this.CharacterInfoPanel.GetChild(0).transform)
        {
            Transform transform = (Transform)obj;
            BattleResultUI.CharacterBattleResultInfoHUD item = new BattleResultUI.CharacterBattleResultInfoHUD(transform.gameObject);
            this.characterBRInfoHudList.Add(item);
        }
        this.expReceiveLabel = this.EXPPanel.GetChild(1).GetComponent<UILabel>();
        this.apReceiveLabel = this.APPanel.GetChild(1).GetComponent<UILabel>();
        this.receiveGilLabel = this.ReceiveGilPanel.GetChild(1).GetComponent<UILabel>();
        this.currentGilLabel = this.CurrentGilPanel.GetChild(1).GetComponent<UILabel>();
        this.infoLabelGameObject = this.InfoPanel.GetChild(0);
        Int32 childCount = this.ItemListPanel.GetChild(0).GetChild(0).transform.childCount;
        this.ItemDetailPanel = this.ItemListPanel.GetChild(0).GetChild(0);
        this.noItemLabel = this.ItemListPanel.GetChild(0).GetChild(1);
        for (Int32 i = 0; i < childCount; i++)
        {
            GameObject child = this.ItemListPanel.GetChild(0).GetChild(0).GetChild(i);
            if (i == childCount - 1)
            {
                this.cardHud = new ItemListDetailWithIconHUD(child, false);
            }
            else if (i != childCount - 2)
            {
                this.itemHudList.Add(new ItemListDetailWithIconHUD(child, true));
            }
        }
        this.screenFadePanel = this.ScreenFadeGameObject.GetParent().GetComponent<UIPanel>();
        this.expLeftSideTween = this.TransitionPanel.GetChild(0).GetComponent<HonoTweenPosition>();
        this.expRightSideTween = this.TransitionPanel.GetChild(1).GetComponent<HonoTweenPosition>();
        this.infoPanelTween = this.TransitionPanel.GetChild(2).GetComponent<HonoTweenPosition>();
        this.helpPanelTween = this.TransitionPanel.GetChild(3).GetComponent<HonoTweenPosition>();
        this.ItemOverflowPanelTween = this.TransitionPanel.GetChild(4).GetComponent<HonoTweenClipping>();
        this.ItemPanelTween = this.TransitionPanel.GetChild(5).GetComponent<HonoTweenClipping>();
        this.levelUpSpriteTween = new HonoTweenPosition[4];
        this.levelUpSpriteTween[0] = this.TransitionPanel.GetChild(6).GetComponent<HonoTweenPosition>();
        this.levelUpSpriteTween[1] = this.TransitionPanel.GetChild(7).GetComponent<HonoTweenPosition>();
        this.levelUpSpriteTween[2] = this.TransitionPanel.GetChild(8).GetComponent<HonoTweenPosition>();
        this.levelUpSpriteTween[3] = this.TransitionPanel.GetChild(9).GetComponent<HonoTweenPosition>();
        this.abilityLearnedPanelTween = new HonoTweenClipping[4];
        this.abilityLearnedPanelTween[0] = this.TransitionPanel.GetChild(10).GetComponent<HonoTweenClipping>();
        this.abilityLearnedPanelTween[1] = this.TransitionPanel.GetChild(11).GetComponent<HonoTweenClipping>();
        this.abilityLearnedPanelTween[2] = this.TransitionPanel.GetChild(12).GetComponent<HonoTweenClipping>();
        this.abilityLearnedPanelTween[3] = this.TransitionPanel.GetChild(13).GetComponent<HonoTweenClipping>();
        if (FF9StateSystem.MobilePlatform)
        {
            this.AllPanel.GetChild(3).GetChild(0).GetComponent<UILocalize>().key = "TouchToConfirm";
        }
        GameObject child2 = this.GilAndItemPhrasePanel.GetChild(0);
        GameObject child3 = this.GilAndItemPhrasePanel.GetChild(0).GetChild(0);
        child2.GetComponent<UIPanel>().depth = 2;
        child3.GetComponent<UIPanel>().depth = 3;
    }

    public GameObject AllPanel;
    public GameObject EXPAndAPPhrasePanel;
    public GameObject GilAndItemPhrasePanel;
    public GameObject CharacterInfoPanel;
    public GameObject EXPPanel;
    public GameObject APPanel;
    public GameObject ReceiveGilPanel;
    public GameObject CurrentGilPanel;
    public GameObject ItemListPanel;
    public GameObject InfoPanel;
    public GameObject TransitionPanel;
    public GameObject ScreenFadeGameObject;

    public static Int32 ItemMax = 6;
    private static Int32 ItemArgMax = 16;
    private static Int32 EXPMinTick = 16;
    private static Int32 EXPDefaultAdd = 32;
    private static Int32 EXPTickMax = 192;
    private static Int32 GilCountTick = 32;
    private static Int32 GilDefaultAdd = 1;

    private List<BattleResultUI.CharacterBattleResultInfoHUD> characterBRInfoHudList = new List<BattleResultUI.CharacterBattleResultInfoHUD>();

    private UILabel expReceiveLabel;
    private UILabel apReceiveLabel;
    private UILabel receiveGilLabel;
    private UILabel currentGilLabel;

    private List<ItemListDetailWithIconHUD> itemHudList = new List<ItemListDetailWithIconHUD>();
    private ItemListDetailWithIconHUD cardHud = new ItemListDetailWithIconHUD();

    private GameObject infoLabelGameObject;
    private UIPanel screenFadePanel;
    private GameObject noItemLabel;
    private GameObject ItemDetailPanel;

    private HonoTweenPosition expLeftSideTween;
    private HonoTweenPosition expRightSideTween;
    private HonoTweenPosition infoPanelTween;
    private HonoTweenPosition helpPanelTween;
    private HonoTweenClipping ItemOverflowPanelTween;
    private HonoTweenClipping ItemPanelTween;
    private HonoTweenPosition[] levelUpSpriteTween;
    private HonoTweenClipping[] abilityLearnedPanelTween;

    private UInt32 defaultExp;
    private UInt32 defaultAp;
    private Int32 defaultGil;
    private TetraMasterCardId defaultCard;

    private Int32 remainPlayerCounter;
    private BattleResultUI.ResultState currentState;

    private BattleEndValue[] expValue = new BattleEndValue[4];
    private BattleEndValue[] apValue = new BattleEndValue[4];
    private BattleEndValue gilValue = new BattleEndValue();

    private List<FF9ITEM> itemList = new List<FF9ITEM>();
    private List<Boolean> isRecieveExpList = new List<Boolean>();

    private Boolean[] isNeedExp = new Boolean[4];
    private Boolean[] isNeedAp = new Boolean[4];
    private Boolean isItemOverflow;

    private Int32[] totalLevelUp = new Int32[4];
    private Int32[] finishedLevelUpAnimation = new Int32[4];
    private List<Int32>[] abilityLearned = new List<Int32>[4];

    private Boolean[] isReadyToShowNextAbil = new Boolean[] { true, true, true, true };

    private Boolean expEndTick;
    private Boolean apEndTick;

    private Boolean isTimerDisplay;

    [NonSerialized]
    private Boolean isLevelUpSoundPlayed = false;
    [NonSerialized]
    private Boolean isAbilityLearnSoundPlayed = false;

    public static Dictionary<BattleStatus, Byte> BadIconDict = new Dictionary<BattleStatus, Byte>
    {
        { BattleStatus.Petrify, 154 },
        { BattleStatus.Venom, 153 },
        { BattleStatus.Virus, 152 },
        { BattleStatus.Silence, 151 },
        { BattleStatus.Blind, 150 },
        { BattleStatus.Trouble, 149 },
        { BattleStatus.Zombie, 148 }
    };

    private class CharacterBattleResultInfoHUD
    {
        public CharacterBattleResultInfoHUD(GameObject go)
        {
            this.Self = go;
            this.Content = go.GetChild(0);
            this.DimPanel = this.Content.GetChild(5);
            this.AvatarSprite = this.Content.GetChild(0).GetComponent<UISprite>();
            this.NameLabel = this.Content.GetChild(1).GetChild(0).GetComponent<UILabel>();
            this.LevelLabel = this.Content.GetChild(1).GetChild(2).GetComponent<UILabel>();
            this.ExpLabel = this.Content.GetChild(3).GetChild(1).GetComponent<UILabel>();
            this.NextLvLabel = this.Content.GetChild(4).GetChild(2).GetComponent<UILabel>();
            this.StatusesSpriteList = new UISprite[]
            {
                this.Content.GetChild(2).GetChild(0).GetChild(0).GetComponent<UISprite>(),
                this.Content.GetChild(2).GetChild(0).GetChild(1).GetComponent<UISprite>(),
                this.Content.GetChild(2).GetChild(0).GetChild(2).GetComponent<UISprite>(),
                this.Content.GetChild(2).GetChild(0).GetChild(3).GetComponent<UISprite>(),
                this.Content.GetChild(2).GetChild(0).GetChild(4).GetComponent<UISprite>(),
                this.Content.GetChild(2).GetChild(0).GetChild(5).GetComponent<UISprite>(),
                this.Content.GetChild(2).GetChild(0).GetChild(6).GetComponent<UISprite>()
            };
            this.AbiltySprite = this.Content.GetChild(7).GetChild(0).GetChild(0).GetComponent<UISprite>();
            this.AbilityLabel = this.Content.GetChild(7).GetChild(0).GetChild(1).GetComponent<UILabel>();
        }

        public GameObject Self;
        public GameObject Content;
        public GameObject DimPanel;

        public UILabel NameLabel;
        public UILabel LevelLabel;
        public UILabel ExpLabel;
        public UILabel NextLvLabel;
        public UISprite AvatarSprite;
        public UISprite[] StatusesSpriteList;
        public UISprite AbiltySprite;
        public UILabel AbilityLabel;
    }

    private class BattleEndValue
    {
        public UInt32 value;
        public UInt32 step;
        public UInt32 current;
    }

    private enum ResultState
    {
        Start,
        EXPAndAPTick,
        EndEXPAndAP,
        StartGilAndItem,
        ItemFullDialog,
        GilTick,
        End,
        Hide,
        None
    }
}
