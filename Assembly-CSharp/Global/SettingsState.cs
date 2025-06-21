using Assets.Scripts.Common;
using Assets.SiliconSocial;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Speedrun;
using System;
using UnityEngine;

#pragma warning disable 169
#pragma warning disable 414
#pragma warning disable 649

// ReSharper disable ArrangeThisQualifier
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable NotAccessedField.Local
// ReSharper disable ConvertToConstant.Global
// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable InconsistentNaming

public class SettingsState : MonoBehaviour
{
    public FF9CFG cfg;
    public Double time;
    public Double StartGameTime;
    public String CurrentLanguage;
    public Byte[] SystemAchievementStatuses;
    public Byte ScreenRotation;
    public Int32 LatestSlot;
    public Int32 LatestSave;
    public Boolean[] IsBoosterButtonActive;

    public Boolean IsATBFull => IsBoosterButtonActive[0];
    public Boolean IsHpMpFull => IsBoosterButtonActive[0];
    public Boolean IsTranceFull => IsBoosterButtonActive[0];
    public Boolean IsDmg9999 => IsBoosterButtonActive[3];
    public Boolean IsNoEncounter => IsBoosterButtonActive[4];
    public Boolean IsFastForward => IsBoosterButtonActive[1];
    public Boolean IsMasterSkill => IsBoosterButtonActive[7];
    public Boolean IsAutoRotation => IsBoosterButtonActive[2];
    public Boolean IsPerspectCamera => IsBoosterButtonActive[5];
    public Boolean IsFastTrophyMode => false;
    public static Int32 FastForwardGameSpeed => Configuration.Cheats.SpeedFactor;

    public static Boolean IsRapidEncounter = false;

    public static Int32 IsFriendlyBattleOnly = 0;

    public Int32 FastForwardFactor
    {
        get
        {
            String str = PersistenSingleton<SceneDirector>.Instance.CurrentScene;
            if (str == "Movie" || IsFastForward && str != "QuadMist")
                return FastForwardGameSpeed;
            return 1;
        }
    }

    public SettingsState()
    {
        Boolean[] flagArray = new Boolean[8];
        flagArray[2] = true;
        IsBoosterButtonActive = flagArray;
    }

    private void Awake()
    {
        cfg = new FF9CFG();
    }

    public void ReadSystemData(Action callback)
    {
        ISharedDataSerializer.OnReadSystemData func = (errNo, metaData) =>
        {
            if (errNo == DataSerializerErrorCode.Success)
            {
                SystemAchievementStatuses = metaData.SystemAchievementStatuses;
                ScreenRotation = metaData.ScreenRotation;
                if (ScreenRotation == 0)
                {
                    Debug.Log("serializer.ReadSystemData.callback 0.5 ReadSystemData : ScreenRotation == 0. Old save file.");
                    ScreenRotation = 3;
                }
                Debug.Log("serializer.ReadSystemData.callback 1 ReadSystemData : ScreenRotation = " + ScreenRotation);
                LatestSlot = metaData.LatestSlot;
                LatestSave = metaData.LatestSave;
                CurrentLanguage = LanguageName.ConvertToLanguageName(metaData.SelectedLanguage);
            }
            else
            {
                SystemAchievementStatuses = null;
                ScreenRotation = 3;
                CurrentLanguage = GetSystemLanguage();
                Debug.Log("serializer.ReadSystemData.callback 2 ReadSystemData : fail");
            }
            if (Configuration.VoiceActing.ForceLanguage >= 0)
            {
                CurrentLanguage = LanguageName.ConvertToLanguageName(Configuration.VoiceActing.ForceLanguage);
                Log.Message($"[VoiceActing] Language forced to '{CurrentLanguage}'");
            }
            PersistenSingleton<UIManager>.Instance.TitleScene.SetRotateScreen();
            Localization.SetCurrentLanguage(CurrentLanguage, this, callback);
            EventInput.UpdateInputLayout();
        };

        FF9StateSystem.Serializer.ReadSystemData(func);
    }

    public void Initial()
    {
        StartGameTime = Time.time;
        time = 0.0;
        cfg = new FF9CFG();
        Boolean[] flagArray = new Boolean[8];
        flagArray[2] = true;
        IsBoosterButtonActive = flagArray;
        SetSound();
        SetSoundEffect();
        SetFastForward(IsFastForward);
        SetBoosterHudToCurrentState();
        PersistenSingleton<HonoInputManager>.Instance.InitialInput();
    }

    public void UpdateSetting()
    {
        StartGameTime = Time.time;
        PersistenSingleton<HonoInputManager>.Instance.SetPrimaryKeys();
        SetSound();
        SetSoundEffect();
        SetBoosterHudToCurrentState();
    }

    public void UpdateTickTime()
    {
        Double num = Time.time;
        time = Math.Min(time + (num - StartGameTime), 2160001.0);
        StartGameTime = num;
        SpeedrunSettings.LogGameTime();
    }

    public String GetSystemLanguage()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;
        switch (systemLanguage)
        {
            case SystemLanguage.English:
                return LanguageName.EnglishUS;
            case SystemLanguage.French:
                return LanguageName.French;
            case SystemLanguage.German:
                return LanguageName.German;
            case SystemLanguage.Italian:
                return LanguageName.Italian;
            case SystemLanguage.Japanese:
                return LanguageName.Japanese;
            case SystemLanguage.Spanish:
                return LanguageName.Spanish;
            default:
                return LanguageName.EnglishUS;
        }
    }

    public void SetMenuLanguage(String language, Action callback)
    {
        ISharedDataSerializer.OnSetSelectedLanguage func = errNo =>
        {
            Localization.SetCurrentLanguage(language, this, callback);
        };

        FF9StateSystem.Serializer.SetSelectedLanguage(LanguageName.ConvertToLanguageCode(language), func);
    }

    public void SetScreenRotation(Byte screenRotation, Action callback)
    {
        ISharedDataSerializer.OnSetScreenRotation func = errNo =>
        {
            ScreenRotation = screenRotation;
            Debug.Log("FF9StateSystem.Serializer.SetScreenRotation: errNo = " + errNo + ", screenRotation = " + screenRotation + ", ScreenRotation = " + ScreenRotation);
            if (callback != null)
                callback();
        };

        Debug.Log("SettingsState.SetScreenRotation screenRotation = " + screenRotation);
        FF9StateSystem.Serializer.SetScreenRotation(screenRotation, func);
    }

    public void SetSound()
    {
        if ((Int64)cfg.sound == 0L)
            SoundLib.EnableMusic();
        else
            SoundLib.DisableMusic();
    }

    public void SetSoundEffect()
    {
        if ((Int64)cfg.sound_effect == 0L)
            SoundLib.EnableSoundEffect();
        else
            SoundLib.DisableSoundEffect();
    }

    public void SetBoosterHudToCurrentState()
    {
        PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.HighSpeedMode, FF9StateSystem.Settings.IsBoosterButtonActive[1]);
        PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.BattleAssistance, FF9StateSystem.Settings.IsBoosterButtonActive[0]);
        PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.Attack9999, FF9StateSystem.Settings.IsBoosterButtonActive[3]);
        PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.NoRandomEncounter, FF9StateSystem.Settings.IsBoosterButtonActive[4]);
        PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.MasterSkill, FF9StateSystem.Settings.IsBoosterButtonActive[7]);
    }

    public void CallBoosterButtonFuntion(BoosterType buttonType, Boolean setActive = true)
    {
        switch (buttonType)
        {
            case BoosterType.BattleAssistance:
                FF9StateSystem.Settings.IsBoosterButtonActive[(Int32)buttonType] = setActive;
                SetATBFull();
                SetHPFull();
                SetTranceBarFull();
                break;
            case BoosterType.HighSpeedMode:
                SetFastForward(setActive);
                break;
            case BoosterType.Rotation:
                FF9StateSystem.Settings.IsBoosterButtonActive[(Int32)buttonType] = setActive;
                UIManager.Input.SendKeyCode(Control.LeftTrigger, true);
                break;
            case BoosterType.Attack9999:
                FF9StateSystem.Settings.IsBoosterButtonActive[(Int32)buttonType] = setActive;
                break;
            case BoosterType.NoRandomEncounter:
                FF9StateSystem.Settings.IsBoosterButtonActive[(Int32)buttonType] = setActive;
                break;
            case BoosterType.Perspective:
                FF9StateSystem.Settings.IsBoosterButtonActive[(Int32)buttonType] = setActive;
                UIManager.Input.SendKeyCode(Control.RightTrigger, true);
                break;
            case BoosterType.MasterSkill:
                FF9StateSystem.Settings.IsBoosterButtonActive[(Int32)buttonType] = setActive;
                SetMasterSkill();
                break;
            case BoosterType.LvMax:
                SetLvMax();
                break;
            case BoosterType.GilMax:
                SetGilMax();
                break;
        }
    }

    public void SetATBFull()
    {
        if (!IsATBFull || !SceneDirector.IsBattleScene())
            return;
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
            if (btl.bi.player != 0 && !btl_stat.CheckStatus(btl, BattleStatus.Death))
                btl.cur.at = btl.max.at;
    }

    public void SetHPFull()
    {
        if (!IsHpMpFull || !SceneDirector.IsBattleScene())
            return;
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.bi.player != 0 && !btl_stat.CheckStatus(btl, BattleStatus.Death))
            {
                btl.cur.hp = btl.max.hp;
                btl.cur.mp = btl.max.mp;
            }
        }
    }

    public void SetTranceFull()
    {
        if (!IsTranceFull || !SceneDirector.IsBattleScene())
            return;

        foreach (BattleUnit btl in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (btl.HasTrance && btl.IsPlayer && !btl.IsUnderAnyStatus(BattleStatusConst.CannotTrance) && btl.Data.cmd[4] != btl_util.getCurCmdPtr() && !SFX.IsRunning())
            {
                btl.Trance = Byte.MaxValue;
                if (!btl.IsUnderAnyStatus(BattleStatus.Trance))
                    btl.AlterStatus(BattleStatusId.Trance);
            }
        }
    }

    public void SetTranceBarFull()
    {
        if (!this.IsTranceFull || !SceneDirector.IsBattleScene())
            return;

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
            if (unit.IsPlayer && unit.IsUnderAnyStatus(BattleStatus.Trance))
                unit.Trance = Byte.MaxValue;
    }

    public void SetFastForward(Boolean isFastForward)
    {
        FF9StateSystem.Settings.IsBoosterButtonActive[1] = isFastForward;
        if (isFastForward)
        {
            if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Field)
            {
                if (EventHUD.CurrentHUD != MinigameHUD.Auction && EventHUD.CurrentHUD != MinigameHUD.PandoniumElevator)
                {
                    HonoBehaviorSystem.Instance.StartFastForwardMode();
                    if (!MBG.IsNull)
                        MBG.Instance.SetFastForward(true);
                }
            }
            else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.World)
                HonoBehaviorSystem.Instance.StartFastForwardMode();
            else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Battle)
                HonoBehaviorSystem.Instance.StartFastForwardMode();
            else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Ending && !MBG.IsNull)
                MBG.Instance.SetFastForward(true);
        }
        else
        {
            if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Field)
            {
                HonoBehaviorSystem.Instance.StopFastForwardMode();
                if (!MBG.IsNull)
                    MBG.Instance.SetFastForward(false);
            }
            else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.World)
                HonoBehaviorSystem.Instance.StopFastForwardMode();
            else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Battle)
                HonoBehaviorSystem.Instance.StopFastForwardMode();
            else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Ending && !MBG.IsNull)
                MBG.Instance.SetFastForward(false);
        }
        SoundLib.UpdatePlayingSoundEffectPitchByGameSpeed();
    }

    public void SetMasterSkill()
    {
        if (!IsMasterSkill)
            return;
        Boolean gotAchievement = false;
        foreach (PLAYER player in FF9StateSystem.Common.FF9.PlayerList)
        {
            foreach (FF9ITEM ff9Item in FF9StateSystem.Common.FF9.item)
            {
                FF9ITEM_DATA itemData = ff9item._FF9Item_Data[ff9Item.id];
                if (ff9Item.count > 0 && (itemData.type & ItemType.AnyEquipment) != 0)
                {
                    foreach (Int32 abilId in itemData.ability)
                    {
                        Int32 abilIndex = ff9abil.FF9Abil_GetIndex(player, abilId);
                        if (abilIndex > -1)
                        {
                            player.pa[abilIndex] = ff9abil.FF9Abil_GetMax(player, abilId);
                            if (BattleAchievement.UpdateAbilitiesAchievement(abilId, false))
                                gotAchievement = true;
                        }
                    }
                }
            }
            for (Int32 equipIndex = 0; equipIndex < 5; ++equipIndex)
            {
                RegularItem itemId = player.equip[equipIndex];
                if (itemId != RegularItem.NoItem)
                {
                    foreach (Int32 abilId in ff9item._FF9Item_Data[itemId].ability)
                    {
                        Int32 abilIndex = ff9abil.FF9Abil_GetIndex(player, abilId);
                        if (abilIndex > -1)
                        {
                            player.pa[abilIndex] = ff9abil.FF9Abil_GetMax(player, abilId);
                            if (BattleAchievement.UpdateAbilitiesAchievement(abilId, false))
                                gotAchievement = true;
                        }
                    }
                }
            }
            if (player.info.serial_no == CharacterSerialNumber.KUINA && ff9abil._FF9Abil_PaData.ContainsKey(player.PresetId))
            {
                foreach (CharacterAbility paData in ff9abil._FF9Abil_PaData[player.PresetId])
                {
                    if (paData.Id != 0 && ff9abil.IsAbilityActive(paData.Id))
                    {
                        Int32 abilIndex = ff9abil.FF9Abil_GetIndex(player, paData.Id);
                        player.pa[abilIndex] = paData.Ap;
                        if (BattleAchievement.UpdateAbilitiesAchievement(paData.Id, false))
                            gotAchievement = true;
                    }
                }
            }
        }
        if (gotAchievement)
            BattleAchievement.SendAbilitiesAchievement();
    }

    private void SetLvMax()
    {
        foreach (PLAYER player in FF9StateSystem.Common.FF9.PlayerList)
        {
            player.SetMaxBonusBasisStatus();
            ff9play.FF9Play_ChangeLevel(player, ff9level.LEVEL_COUNT, false);
            UInt32 gemUsage = player.max.capa - player.cur.capa;
            player.max.capa = UInt32.MaxValue;
            player.cur.capa = UInt32.MaxValue - gemUsage;
        }
        AchievementManager.ReportAchievement(AcheivementKey.CharLv99, 1);
    }

    private void SetGilMax()
    {
        FF9StateSystem.Common.FF9.party.gil = 9999999U;
    }
}
