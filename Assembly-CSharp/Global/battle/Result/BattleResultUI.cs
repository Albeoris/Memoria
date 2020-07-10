using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Data;
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
				BattleEndValue[] array = this.expValue;
				for (Int32 i = 0; i < (Int32)array.Length; i++)
				{
					BattleEndValue battleEndValue = array[i];
					battleEndValue.step = battleEndValue.value;
				}
				this.UpdateExp();
				BattleEndValue[] array2 = this.apValue;
				for (Int32 j = 0; j < (Int32)array2.Length; j++)
				{
					BattleEndValue battleEndValue2 = array2[j];
					battleEndValue2.step = battleEndValue2.value;
				}
				this.UpdateAp();
				this.DisplayEXPAndAPInfo();
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
		this.receiveGilLabel.text = (this.gilValue.value - this.gilValue.current).ToString() + "[YSUB=1.3][sub]G";
		this.currentGilLabel.text = Mathf.Min(FF9StateSystem.Common.FF9.party.gil, 9999999f).ToString() + "[YSUB=1.3][sub]G";
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
				FF9UIDataTool.DisplayItem((Int32)ff9ITEM.id, itemListDetailWithIconHUD.IconSprite, itemListDetailWithIconHUD.NameLabel, true);
				itemListDetailWithIconHUD.NumberLabel.text = ff9ITEM.count.ToString();
				num++;
			}
		}
		else
		{
			this.ItemDetailPanel.SetActive(false);
			this.noItemLabel.SetActive(true);
		}
		if (this.defaultCard != 255)
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
				UInt64 num = (player.level >= 99) ? player.exp : ff9level.CharacterLevelUps[player.level].ExperienceToLevel;
				BattleResultUI.CharacterBattleResultInfoHUD characterBattleResultInfoHUD = this.characterBRInfoHudList[i];
				characterBattleResultInfoHUD.Content.SetActive(true);
				characterBattleResultInfoHUD.NameLabel.text = player.name;
				characterBattleResultInfoHUD.LevelLabel.text = player.level.ToString();
				characterBattleResultInfoHUD.ExpLabel.text = player.exp.ToString();
				characterBattleResultInfoHUD.NextLvLabel.text = (num - player.exp).ToString();
				FF9UIDataTool.DisplayCharacterAvatar(player, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), characterBattleResultInfoHUD.AvatarSprite, false);
				UISprite[] statusesSpriteList = characterBattleResultInfoHUD.StatusesSpriteList;
				for (Int32 j = 0; j < (Int32)statusesSpriteList.Length; j++)
				{
					UISprite uisprite = statusesSpriteList[j];
					uisprite.alpha = 0f;
				}
				Int32 num2 = 0;
				foreach (KeyValuePair<UInt32, Byte> keyValuePair in BattleResultUI.BadIconDict)
				{
					if (((UInt32)player.status & keyValuePair.Key) != 0u)
					{
						characterBattleResultInfoHUD.StatusesSpriteList[num2].alpha = 1f;
						characterBattleResultInfoHUD.StatusesSpriteList[num2].spriteName = FF9UIDataTool.IconSpriteName[(Int32)keyValuePair.Value];
						num2++;
						if (num2 > (Int32)characterBattleResultInfoHUD.StatusesSpriteList.Length)
						{
							break;
						}
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
		BattleAchievement.GetReachLv99Achievement((Int32)(player.level + 1));
		ff9play.FF9Play_GrowLevel((Int32)player.info.slot_no, (Int32)(player.level + 1));
		this.totalLevelUp[index]++;
		this.ShowLevelUpAnimation(index);
	}

	private void ShowLevelUpAnimation(Int32 index)
	{
		if (this.finishedLevelUpAnimation[index] >= this.totalLevelUp[index])
		{
			return;
		}
		FF9Sfx.FF9SFX_Play(683);
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
		{
			characterBattleResultInfoHUD.Content.SetActive(false);
		}
		foreach (ItemListDetailWithIconHUD itemListDetailWithIconHUD in this.itemHudList)
		{
			itemListDetailWithIconHUD.Self.SetActive(false);
		}
		HonoTweenPosition[] array = this.levelUpSpriteTween;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			HonoTweenPosition honoTweenPosition = array[i];
			honoTweenPosition.dialogList[0].SetActive(false);
		}
		HonoTweenClipping[] array2 = this.abilityLearnedPanelTween;
		for (Int32 j = 0; j < (Int32)array2.Length; j++)
		{
			HonoTweenClipping honoTweenClipping = array2[j];
			honoTweenClipping.ClipGameObject.SetActive(false);
		}
		this.cardHud.Self.SetActive(false);
	}

	private void AfterShowGilAndItem()
	{
		this.DisplayGilAndItemInfo();
		this.ItemPanelTween.TweenIn((Action)null);
		this.currentState = BattleResultUI.ResultState.StartGilAndItem;
	}

	private void AnalyzeParty()
	{
		this.remainPlayerCounter = 0;
		for (Int32 i = 0; i < 4; i++)
		{
			if (((Int32)battle.btl_bonus.member_flag & 1 << i) != 0 && FF9StateSystem.Common.FF9.party.member[i] != null)
			{
			    Character player = FF9StateSystem.Common.FF9.party.GetCharacter(i);

                this.isNeedExp[i] = this.IsNeedExp(player);
				this.isNeedAp[i] = this.IsNeedAp(player);
				if (this.isNeedExp[i])
				{
					this.remainPlayerCounter++;
				}
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
		return this.IsEnable(play) && ((Int32)battle.btl_bonus.member_flag & 1 << id) != 0;
	}

	private Boolean IsEnable(PLAYER play)
	{
		return play != null && play.cur.hp > 0 && (play.status & 69) == 0;
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
		}, (UIScene.SceneVoidDelegate)null);
		this.expRightSideTween.TweenIn(new Byte[]
		{
			0,
			1,
			2
		}, (UIScene.SceneVoidDelegate)null);
		this.totalLevelUp = new Int32[4];
		this.finishedLevelUpAnimation = new Int32[4];
		this.abilityLearned[0] = new List<Int32>();
		this.abilityLearned[1] = new List<Int32>();
		this.abilityLearned[2] = new List<Int32>();
		this.abilityLearned[3] = new List<Int32>();
		this.isReadyToShowNextAbil = new Boolean[]
		{
			true,
			true,
			true,
			true
		};
		Boolean flag = true;
		BattleEndValue[] array = this.expValue;
		for (Int32 j = 0; j < (Int32)array.Length; j++)
		{
			BattleEndValue battleEndValue = array[j];
			if (battleEndValue.value != 0u)
			{
				flag = false;
				break;
			}
		}
		for (Int32 k = 0; k < (Int32)this.apValue.Length; k++)
		{
			if (this.apValue[k].value != 0u)
			{
				flag = false;
				break;
			}
		}
		for (Int32 k = 0; k < (Int32)this.expValue.Length && flag; k++)
			if (this.expValue[k].value != 0u)
				flag = false;
		if (!flag)
		{
			this.currentState = BattleResultUI.ResultState.Start;
		}
		else
		{
			this.currentState = BattleResultUI.ResultState.EndEXPAndAP;
		}
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
			{
				TimerUI.SetDisplay(false);
			}
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
		this.Hide((UIScene.SceneVoidDelegate)null);
		yield break;
	}

	private void InitialInfo()
	{
		this.AnalyzeBonusItem();
		PLAYER[] party = FF9StateSystem.Common.FF9.party.member;
		for (Int32 saIndex = 0; saIndex < 64; saIndex++)
		{
			Boolean triggered = false;
			for (Int32 playIndex = 0; playIndex < 4 && !triggered; playIndex++)
				if (party[playIndex] != null && ff9abil.FF9Abil_IsEnableSA(party[playIndex].sa, 192 + saIndex))
					triggered = ff9abil._FF9Abil_SaFeature[saIndex].TriggerOnBattleResult(party[playIndex], battle.btl_bonus, this.itemList, "RewardAll", 0U);
		}
		this.AnalyzeArgument();
		for (Int32 i = 0; i < 4; i++)
		{
			PLAYER player = party[i];
			if (player != null)
			{
				BONUS individualBonus = new BONUS();
				individualBonus.ap = (UInt16)this.defaultAp;
				individualBonus.card = (Byte)this.defaultCard;
				individualBonus.exp = this.defaultExp;
				individualBonus.gil = this.defaultGil;
				foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(player.sa))
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
						UInt32 num = battleEndValue.value / battleEndValue.step;
						if ((UInt64)num > (UInt64)((Int64)BattleResultUI.EXPTickMax))
						{
							battleEndValue.step = (UInt32)(((UInt64)battleEndValue.value + (UInt64)((Int64)BattleResultUI.EXPTickMax) - 1UL) / (UInt64)((Int64)BattleResultUI.EXPTickMax));
						}
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
		if ((UInt64)this.gilValue.value < (UInt64)((Int64)BattleResultUI.GilCountTick))
		{
			this.gilValue.step = 1u;
		}
		else if ((UInt64)this.gilValue.value < (UInt64)((Int64)(BattleResultUI.GilCountTick * BattleResultUI.GilDefaultAdd)))
		{
			this.gilValue.step = (UInt32)(((UInt64)this.gilValue.value + (UInt64)((Int64)BattleResultUI.GilCountTick) - 1UL) / (UInt64)((Int64)BattleResultUI.GilCountTick));
		}
		else
		{
			this.gilValue.step = (UInt32)BattleResultUI.GilDefaultAdd;
		}
		this.gilValue.current = 0u;
	}

	public void ShutdownBattleResultUI()
	{
		base.StartCoroutine(this.InitialNone_delay());
	}

	private void AnalyzeBonusItem()
	{
		this.itemList.Clear();
		for (Int32 i = 0; i < BattleResultUI.ItemArgMax; i++)
		{
			if (battle.btl_bonus.item[i] != 255)
			{
				Boolean flag = false;
				for (Int32 j = 0; j < this.itemList.Count; j++)
				{
					if (battle.btl_bonus.item[i] == this.itemList[j].id)
					{
						FF9ITEM ff9ITEM = this.itemList[j];
						ff9ITEM.count = (Byte)(ff9ITEM.count + 1);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (this.itemList.Count >= BattleResultUI.ItemMax)
					{
						return;
					}
					FF9ITEM item = new FF9ITEM(battle.btl_bonus.item[i], 1);
					this.itemList.Add(item);
				}
			}
		}
	}

	private void AnalyzeArgument()
	{
		this.AnalyzeParty();
		this.defaultExp = (UInt32)((this.remainPlayerCounter == 0) ? 0UL : ((UInt64)battle.btl_bonus.exp / (UInt64)((Int64)this.remainPlayerCounter)));
		this.defaultAp = (UInt32)battle.btl_bonus.ap;
		this.defaultGil = battle.btl_bonus.gil;
		this.defaultCard = (Int32)((QuadMistDatabase.MiniGame_GetAllCardCount() < 100) ? battle.btl_bonus.card : Byte.MaxValue);
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
					UInt32 num = (UInt32)((battleEndValue.current + battleEndValue.step > battleEndValue.value) ? (battleEndValue.value - battleEndValue.current) : battleEndValue.step);
					if (battleEndValue.current < battleEndValue.value)
					{
						this.expEndTick = false;
						if (num != 0u)
						{
							if (9999999u > player.exp)
							{
								battleEndValue.current += num;
								player.exp += num;
								if (9999999u <= player.exp)
								{
									player.exp = 9999999u;
								}
								if (99 > player.level)
								{
									for (UInt32 exp = ff9level.CharacterLevelUps[player.level].ExperienceToLevel; exp <= player.exp; exp = ff9level.CharacterLevelUps[player.level].ExperienceToLevel)
									{
										this.DisplayLevelup(i);
										if (player.level >= 99)
										{
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
		if (ap == 0u)
		{
			return;
		}
		if (player == null)
		{
			return;
		}
		if (!ff9abil.FF9Abil_HasAp(player))
		{
			return;
		}
		this.apValue[id].current += ap;
		for (Int32 i = 0; i < 5; i++)
		{
			if (player.Equipment[i] != 255)
			{
				FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[(Int32)player.Equipment[i]];
				for (Int32 j = 0; j < 3; j++)
				{
					if (ff9ITEM_DATA.ability[j] != 0)
					{
						Int32 num = ff9abil.FF9Abil_GetIndex(player.Index, ff9ITEM_DATA.ability[j]);
						if (num >= 0)
						{
							Int32 max_ap = ff9abil._FF9Abil_PaData[player.PresetId][num].Ap;
							Int32 num2 = (Int32)player.Data.pa[num];
							if (max_ap > num2)
							{
								if ((Int64)(max_ap - num2) <= (Int64)((UInt64)ap))
								{
									player.Data.pa[num] = (Byte)max_ap;
									this.ApLearned(id, (Int32)ff9ITEM_DATA.ability[j]);
								}
								else
								{
									Byte[] pa = player.Data.pa;
									Int32 num3 = num;
									pa[num3] = (Byte)(pa[num3] + (Byte)ap);
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
		FF9Sfx.FF9SFX_Play(1043);
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
		if (abilId < 192)
		{
			abilName = FF9TextTool.ActionAbilityName(abilId);
			spriteName = "ability_stone";
		}
		else
		{
			abilName = FF9TextTool.SupportAbilityName(abilId - 192);
			PLAYER player = FF9StateSystem.Common.FF9.party.member[id];
			spriteName = ((!ff9abil.FF9Abil_IsEnableSA(player.sa, abilId)) ? "skill_stone_off" : "skill_stone_on");
		}
		this.characterBRInfoHudList[id].AbiltySprite.spriteName = spriteName;
		this.characterBRInfoHudList[id].AbilityLabel.text = abilName;
		this.abilityLearnedPanelTween[id].TweenIn((Action)null);
		yield return new WaitForSeconds(1f);
		this.abilityLearnedPanelTween[id].TweenOut(delegate
		{
			this.isReadyToShowNextAbil[id] = true;
		});
		yield break;
	}

	private void UpdateGil()
	{
		Boolean flag = true;
		if (this.gilValue.current < this.gilValue.value)
		{
			UInt32 num = (UInt32)((this.gilValue.current + this.gilValue.step > this.gilValue.value) ? (this.gilValue.value - this.gilValue.current) : this.gilValue.step);
			if (num != 0u)
			{
				flag = false;
				this.gilValue.current += num;
				if (9999999u < FF9StateSystem.Common.FF9.party.gil + num)
				{
					num = 9999999u - FF9StateSystem.Common.FF9.party.gil;
					this.gilValue.current = this.gilValue.value;
				}
				FF9StateSystem.Common.FF9.party.gil += num;
			}
		}
		if (flag)
		{
			FF9Sfx.FF9SFX_StopLoop(109);
			this.currentState = BattleResultUI.ResultState.End;
		}
	}

	private Boolean UpdateItem()
	{
		Boolean flag = false;
		for (Int32 i = 0; i < this.itemList.Count; i++)
		{
			if ((Int32)this.itemList[i].count != ff9item.FF9Item_Add((Int32)this.itemList[i].id, (Int32)this.itemList[i].count))
			{
				FF9Sfx.FF9SFX_Play(1046);
				flag = true;
			}
		}
		if (this.defaultCard != 255)
		{
			QuadMistDatabase.MiniGame_SetCard(this.defaultCard);
		}
		if (flag && (this.itemList.Count != 0 || this.defaultCard != 255))
		{
			FF9Sfx.FF9SFX_Play(108);
		}
		return !flag;
	}

	private void UpdateState()
	{
		if (this.expEndTick && this.apEndTick)
		{
			for (Int32 i = 0; i < 4; i++)
			{
				if (this.abilityLearned[i].Count != 0)
				{
					return;
				}
			}
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
		}, (UIScene.SceneVoidDelegate)null);
		this.expRightSideTween.TweenOut(new Byte[]
		{
			0,
			1,
			2
		}, (UIScene.SceneVoidDelegate)null);
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
			BattleResultUI.ResultState resultState = this.currentState;
			if (resultState != BattleResultUI.ResultState.EXPAndAPTick)
			{
				if (resultState == BattleResultUI.ResultState.GilTick)
				{
					this.UpdateGil();
					this.DisplayGilAndItemInfo();
				}
			}
			else
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

	private Int32 defaultCard;

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

	private Boolean[] isReadyToShowNextAbil = new Boolean[]
	{
		true,
		true,
		true,
		true
	};

	private Boolean expEndTick;

	private Boolean apEndTick;

	private Boolean isTimerDisplay;

	public static Dictionary<UInt32, Byte> BadIconDict = new Dictionary<UInt32, Byte>
	{
		{
			1u,
			154
		},
		{
			2u,
			153
		},
		{
			4u,
			152
		},
		{
			8u,
			151
		},
		{
			16u,
			150
		},
		{
			32u,
			149
		},
		{
			64u,
			148
		}
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
