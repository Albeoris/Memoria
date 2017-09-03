using Assets.Scripts.Common;
using Assets.SiliconSocial;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using System;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
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
    public static Boolean IsRapidEncounter;

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
                switch (metaData.SelectedLanguage)
                {
                    case 0:
                        CurrentLanguage = "English(US)";
                        break;
                    case 1:
                        CurrentLanguage = "English(UK)";
                        break;
                    case 2:
                        CurrentLanguage = "English(US)";
                        break;
                    case 3:
                        CurrentLanguage = "German";
                        break;
                    case 4:
                        CurrentLanguage = "French";
                        break;
                    case 5:
                        CurrentLanguage = "Italian";
                        break;
                    case 6:
                        CurrentLanguage = "Spanish";
                        break;
                    default:
                        CurrentLanguage = GetSystemLanguage();
                        break;
                }
            }
            else
            {
                SystemAchievementStatuses = null;
                ScreenRotation = 3;
                CurrentLanguage = GetSystemLanguage();
                Debug.Log("serializer.ReadSystemData.callback 2 ReadSystemData : fail");
            }
            PersistenSingleton<UIManager>.Instance.TitleScene.SetRotateScreen();
            Localization.localizationHasBeenSet = false;
            Localization.language = CurrentLanguage;
            UIManager.Field.InitializeATEText();
            StartCoroutine(PersistenSingleton<FF9TextTool>.Instance.UpdateTextLocalization(callback));
            EventInput.ChangeInputLayout(CurrentLanguage);
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
    }

    public String GetSystemLanguage()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;
        switch (systemLanguage)
        {
            case SystemLanguage.English:
                return "English(US)";
            case SystemLanguage.French:
                return "French";
            case SystemLanguage.German:
                return "German";
            default:
                if (systemLanguage == SystemLanguage.Italian)
                    return "Italian";
                return systemLanguage == SystemLanguage.Japanese || systemLanguage != SystemLanguage.Spanish ? "English(US)" : "Spanish";
        }
    }

    public void SetMenuLanguage(String language, Action callback)
    {
        ISharedDataSerializer.OnSetSelectedLanguage func = errNo =>
        {
            //if (errNo != DataSerializerErrorCode.Success)
            //    ;
            CurrentLanguage = language;
            Localization.language = language;
            UIManager.Field.InitializeATEText();
            StartCoroutine(PersistenSingleton<FF9TextTool>.Instance.UpdateTextLocalization(callback));
        };

        FF9StateSystem.Serializer.SetSelectedLanguage(LanguageName.ConvertToLanguageCode(language), func);
    }

    public void SetScreenRotation(Byte screenRotation, Action callback)
    {
        ISharedDataSerializer.OnSetScreenRotation func = errNo =>
        {
            //if (errNo != DataSerializerErrorCode.Success)
            //    ;
            ScreenRotation = screenRotation;
            Debug.Log("FF9StateSystem.Serializer.SetScreenRotation: errNo = " + errNo + ", screenRotation = " + screenRotation + ", ScreenRotation = " + ScreenRotation);
            if (callback == null)
                return;
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
        {
            if (btl.bi.player != 0 && !Status.checkCurStat(btl, 256U))
                btl.cur.at = btl.max.at;
        }
    }

    public void SetHPFull()
    {
        if (!IsHpMpFull || !SceneDirector.IsBattleScene())
            return;
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.bi.player != 0 && !Status.checkCurStat(btl, 256U))
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
            if (btl.HasTrance && btl.IsPlayer && !btl.IsUnderStatus((BattleStatus)33575235U) && btl.Data.cmd[4] != btl_util.getCurCmdPtr() && !SFX.isRunning)
            {
                btl.Trance = Byte.MaxValue;
                if (!btl.IsUnderStatus(BattleStatus.Trance))
                    btl.AlterStatus(BattleStatus.Trance);
            }
        }
    }

    public void SetTranceBarFull()
    {
        if (!this.IsTranceFull || !SceneDirector.IsBattleScene())
            return;

        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (unit.IsUnderStatus(BattleStatus.Trance))
                unit.Trance = Byte.MaxValue;
        }
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
                HonoluluBattleMain.UpdateFrameTime(HonoBehaviorSystem.Instance.GetFastForwardFactor());
            else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Ending && !MBG.IsNull)
                MBG.Instance.SetFastForward(true);
        }
        else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Field)
        {
            HonoBehaviorSystem.Instance.StopFastForwardMode();
            if (!MBG.IsNull)
                MBG.Instance.SetFastForward(false);
        }
        else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.World)
            HonoBehaviorSystem.Instance.StopFastForwardMode();
        else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Battle)
            HonoluluBattleMain.UpdateFrameTime(1);
        else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Ending && !MBG.IsNull)
            MBG.Instance.SetFastForward(false);
        SoundLib.UpdatePlayingSoundEffectPitchByGameSpeed();
    }

    public void SetMasterSkill()
    {
        if (!IsMasterSkill)
            return;
        Boolean flag = false;
        for (Int32 index1 = 0; index1 < 9; ++index1)
        {
            PLAYER player = FF9StateSystem.Common.FF9.player[index1];
            foreach (FF9ITEM ff9Item in FF9StateSystem.Common.FF9.item)
            {
                if (ff9Item.count > 0 && ff9item.FF9Item_GetEquipPart(ff9Item.id) > -1)
                {
                    foreach (Byte num in ff9item._FF9Item_Data[ff9Item.id].ability)
                    {
                        Int32 index2 = ff9abil.FF9Abil_GetIndex(player.info.slot_no, num);
                        if (index2 > -1)
                        {
                            player.pa[index2] = (Byte)ff9abil.FF9Abil_GetMax(player.info.slot_no, num);
                            if (BattleAchievement.UpdateAbilitiesAchievement(num, false))
                                flag = true;
                        }
                    }
                }
            }
            for (Int32 index2 = 0; index2 < 5; ++index2)
            {
                Byte num1 = player.equip[index2];
                if (num1 != Byte.MaxValue)
                {
                    foreach (Byte num2 in ff9item._FF9Item_Data[num1].ability)
                    {
                        Int32 index3 = ff9abil.FF9Abil_GetIndex(player.info.slot_no, num2);
                        if (index3 > -1)
                        {
                            player.pa[index3] = (Byte)ff9abil.FF9Abil_GetMax(player.info.slot_no, num2);
                            if (BattleAchievement.UpdateAbilitiesAchievement(num2, false))
                                flag = true;
                        }
                    }
                }
            }
            if (player.info.serial_no == 9)
            {
                foreach (CharacterAbility paData in ff9abil._FF9Abil_PaData[player.info.menu_type])
                {
                    if (paData.Id != 0 && paData.Id < 192)
                    {
                        Int32 index2 = ff9abil.FF9Abil_GetIndex(player.info.slot_no, paData.Id);
                        player.pa[index2] = paData.Ap;
                        if (BattleAchievement.UpdateAbilitiesAchievement(paData.Id, false))
                            flag = true;
                    }
                }
            }
        }
        if (!flag)
            return;
        BattleAchievement.SendAbilitiesAchievement();
    }

    private void SetLvMax()
    {
        for (Int32 slot_id = 0; slot_id < 9; ++slot_id)
        {
            PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
            player.SetMaxBonusBasisStatus();
            ff9play.FF9Play_ChangeLevel(slot_id, 99, false);
            Int32 num = player.max.capa - player.cur.capa;
            player.max.capa = 99;
            player.cur.capa = (Byte)(99 - num);
        }
        AchievementManager.ReportAchievement(AcheivementKey.CharLv99, 1);
    }

    private void SetGilMax()
    {
        FF9StateSystem.Common.FF9.party.gil = 9999999U;
    }
}