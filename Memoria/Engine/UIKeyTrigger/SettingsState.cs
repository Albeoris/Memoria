using Assets.Scripts.Common;
using Assets.SiliconSocial;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using System;
using Memoria;
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

[ExportedType("®Ēßđ.!!!xĥQ¡ÊSeÛºý=±ł8J§âŁŃD«9Þð_ęĆ^ģÝáĈX¼öfãďþ0Q¹·Zè6³ĺ+!!!¦Mŀ6®ÌĿÕ=ÇðøĠVáýĮ8ËÍĴĪÔĥ{¢ûºMTÜ¸ĸlĬěC!!!į­ÍJìP'Ġd:ĕÎjzĐĐģČõĬ/¤^C~vīĬĪ­ł¦ĉõ°ēÂĈ9»LtđüóÑĀGý>«Ģī2LzoÝÊ!ĥ#ĭvó[äÄhäü²Òě¢įKğĭluönÇăBġV¦Ynÿöêv$(ĖĦĽĥO!¡ĂqbV¢ħ@¦*Ëø/ĳĨįèrP¶lļd3àĴkºA%!!!ZăÐ¬ńńńń$!!!ĔóĖ°ĜıĞ·$!!!ĄÀļě6Łµèńńńń5Ð½ôńńńń%!!!|¦_æĔóĖ°ĜıĞ·$!!!ĄÀļěĂt5ĴńńńńĔA2ińńńń%!!!ī¸QÚĔóĖ°ĜıĞ·$!!!ĄÀļěò0e1ńńńń")]
public class SettingsState : MonoBehaviour
{
    public FF9CFG cfg;
    public double time;
    public double StartGameTime;
    public string CurrentLanguage;
    public byte[] SystemAchievementStatuses;
    public byte ScreenRotation;
    public int LatestSlot;
    public int LatestSave;
    public bool[] IsBoosterButtonActive;

    public bool IsATBFull => IsBoosterButtonActive[0];

    public bool IsHpMpFull => IsBoosterButtonActive[0];

    public bool IsTranceFull => IsBoosterButtonActive[0];

    public bool IsDmg9999 => IsBoosterButtonActive[3];

    public bool IsNoEncounter => IsBoosterButtonActive[4];

    public bool IsFastForward => IsBoosterButtonActive[1];

    public bool IsMasterSkill => IsBoosterButtonActive[7];

    public bool IsAutoRotation => IsBoosterButtonActive[2];

    public bool IsPerspectCamera => IsBoosterButtonActive[5];

    public bool IsFastTrophyMode => false;

    public static int FastForwardGameSpeed => Configuration.Cheats.SpeedFactor;

    public int FastForwardFactor
    {
        get
        {
            string str = PersistenSingleton<SceneDirector>.Instance.CurrentScene;
            if (str == "Movie" || IsFastForward && str != "QuadMist")
                return FastForwardGameSpeed;
            return 1;
        }
    }

    public SettingsState()
    {
        bool[] flagArray = new bool[8];
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
        bool[] flagArray = new bool[8];
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
        double num = Time.time;
        time = Math.Min(time + (num - StartGameTime), 2160001.0);
        StartGameTime = num;
    }

    public string GetSystemLanguage()
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

    public void SetMenuLanguage(string language, Action callback)
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

    public void SetScreenRotation(byte screenRotation, Action callback)
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
        if ((long)cfg.sound == 0L)
            SoundLib.EnableMusic();
        else
            SoundLib.DisableMusic();
    }

    public void SetSoundEffect()
    {
        if ((long)cfg.sound_effect == 0L)
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

    public void CallBoosterButtonFuntion(BoosterType buttonType, bool setActive = true)
    {
        switch (buttonType)
        {
            case BoosterType.BattleAssistance:
                FF9StateSystem.Settings.IsBoosterButtonActive[(int)buttonType] = setActive;
                SetATBFull();
                SetHPFull();
                SetTranceBarFull();
                break;
            case BoosterType.HighSpeedMode:
                SetFastForward(setActive);
                break;
            case BoosterType.Rotation:
                FF9StateSystem.Settings.IsBoosterButtonActive[(int)buttonType] = setActive;
                UIManager.Input.SendKeyCode(Control.LeftTrigger, true);
                break;
            case BoosterType.Attack9999:
                FF9StateSystem.Settings.IsBoosterButtonActive[(int)buttonType] = setActive;
                break;
            case BoosterType.NoRandomEncounter:
                FF9StateSystem.Settings.IsBoosterButtonActive[(int)buttonType] = setActive;
                break;
            case BoosterType.Perspective:
                FF9StateSystem.Settings.IsBoosterButtonActive[(int)buttonType] = setActive;
                UIManager.Input.SendKeyCode(Control.RightTrigger, true);
                break;
            case BoosterType.MasterSkill:
                FF9StateSystem.Settings.IsBoosterButtonActive[(int)buttonType] = setActive;
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
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.bi.t_gauge != 0 && btl.bi.player != 0 && !Status.checkCurStat(btl, 33575235U) && btl.cmd[4] != btl_util.getCurCmdPtr() && !SFX.isRunning)
            {
                if (!Status.checkCurStat(btl, 16384U))
                {
                    btl.trance = byte.MaxValue;
                    btl_stat.AlterStatus(btl, 16384U);
                }
                else
                    btl.trance = byte.MaxValue;
            }
        }
    }

    public void SetTranceBarFull()
    {
        if (!this.IsTranceFull || !SceneDirector.IsBattleScene())
            return;

        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (Status.checkCurStat(btl, 16384U))
                btl.trance = byte.MaxValue;
        }
    }

    public void SetFastForward(bool isFastForward)
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
        bool flag = false;
        for (int index1 = 0; index1 < 9; ++index1)
        {
            PLAYER player = FF9StateSystem.Common.FF9.player[index1];
            foreach (FF9ITEM ff9Item in FF9StateSystem.Common.FF9.item)
            {
                if (ff9Item.count > 0 && ff9item.FF9Item_GetEquipPart(ff9Item.id) > -1)
                {
                    foreach (byte num in ff9item._FF9Item_Data[ff9Item.id].ability)
                    {
                        int index2 = ff9abil.FF9Abil_GetIndex(player.info.slot_no, num);
                        if (index2 > -1)
                        {
                            player.pa[index2] = (byte)ff9abil.FF9Abil_GetMax(player.info.slot_no, num);
                            if (BattleAchievement.UpdateAbilitiesAchievement(num, false))
                                flag = true;
                        }
                    }
                }
            }
            for (int index2 = 0; index2 < 5; ++index2)
            {
                byte num1 = player.equip[index2];
                if (num1 != byte.MaxValue)
                {
                    foreach (byte num2 in ff9item._FF9Item_Data[num1].ability)
                    {
                        int index3 = ff9abil.FF9Abil_GetIndex(player.info.slot_no, num2);
                        if (index3 > -1)
                        {
                            player.pa[index3] = (byte)ff9abil.FF9Abil_GetMax(player.info.slot_no, num2);
                            if (BattleAchievement.UpdateAbilitiesAchievement(num2, false))
                                flag = true;
                        }
                    }
                }
            }
            if (player.info.serial_no == 9)
            {
                foreach (PA_DATA paData in ff9abil._FF9Abil_PaData[player.info.menu_type])
                {
                    if (paData.id != 0 && paData.id < 192)
                    {
                        int index2 = ff9abil.FF9Abil_GetIndex(player.info.slot_no, paData.id);
                        player.pa[index2] = paData.max_ap;
                        if (BattleAchievement.UpdateAbilitiesAchievement(paData.id, false))
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
        for (int slot_id = 0; slot_id < 9; ++slot_id)
        {
            PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
            player.SetMaxBonusBasisStatus();
            ff9play.FF9Play_ChangeLevel(slot_id, 99, false);
            int num = player.max.capa - player.cur.capa;
            player.max.capa = 99;
            player.cur.capa = (byte)(99 - num);
        }
        AchievementManager.ReportAchievement(AcheivementKey.CharLv99, 1);
    }

    private void SetGilMax()
    {
        FF9StateSystem.Common.FF9.party.gil = 9999999U;
    }
}