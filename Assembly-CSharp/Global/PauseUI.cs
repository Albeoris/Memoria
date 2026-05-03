using System;
using UnityEngine;

public class PauseUI : UIScene
{
    public Boolean AndroidTVOnKeyCancel(GameObject go)
    {
        Boolean result = false;
        if (base.CheckAndroidTVModule(Control.Cancel))
        {
            UIManager.Input.HandleBoosterButton(BoosterType.NoRandomEncounter);
            result = true;
        }
        return result;
    }

    public Boolean AndroidTVOnKeyOnKeyLeftBumper(GameObject go)
    {
        Boolean result = false;
        if (base.CheckAndroidTVModule(Control.LeftBumper))
        {
            result = true;
        }
        return result;
    }

    public Boolean AndroidTVOnKeyOnKeyRightBumper(GameObject go)
    {
        Boolean result = false;
        if (base.CheckAndroidTVModule(Control.RightBumper))
        {
            result = true;
        }
        return result;
    }

    public Boolean AndroidTVOnKeyOnKeyLeftTrigger(GameObject go)
    {
        Boolean result = false;
        if (base.CheckAndroidTVModule(Control.LeftTrigger))
        {
            result = true;
        }
        return result;
    }

    public Boolean AndroidTVOnKeyOnKeyRightTrigger(GameObject go)
    {
        Boolean result = false;
        if (base.CheckAndroidTVModule(Control.RightTrigger))
        {
            result = true;
        }
        return result;
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go) && base.CheckAndroidTVModule(Control.Confirm))
        {
            UIManager.Input.HandleBoosterButton(BoosterType.HighSpeedMode);
        }
        return true;
    }

    public override Boolean OnKeySpecial(GameObject go)
    {
        if (base.OnKeySpecial(go) && base.CheckAndroidTVModule(Control.Special))
        {
            UIManager.Input.HandleBoosterButton(BoosterType.Attack9999);
        }
        return true;
    }

    public override Boolean OnKeyMenu(GameObject go)
    {
        if (base.OnKeyMenu(go) && base.CheckAndroidTVModule(Control.Menu))
        {
            UIManager.Input.HandleBoosterButton(BoosterType.BattleAssistance);
        }
        return true;
    }

    public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
        {
            FF9StateSystem.Settings.UpdateTickTime();
        };
        if (afterFinished != null)
        {
            sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        }
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, (Action)null);
        PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(false);
        PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
        if (PersistenSingleton<UIManager>.Instance.Dialogs != (UnityEngine.Object)null)
        {
            PersistenSingleton<UIManager>.Instance.Dialogs.PauseAllDialog(true);
        }
        base.Show(sceneVoidDelegate);
        if (FF9StateSystem.MobileAndaaaaPlatform)
        {
            PersistenSingleton<UIManager>.Instance.Booster.OpenBoosterPanelImmediately();
        }
        this.previousVibLeft = vib.CurrentVibrateLeft;
        this.previousVibRight = vib.CurrentVibrateRight;
        vib.VIB_actuatorReset(0);
        vib.VIB_actuatorReset(1);
        this.PauseDespText.SetActive(FF9StateSystem.MobileAndaaaaPlatform);
    }

    public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
    {
        if (FF9StateSystem.MobileAndaaaaPlatform)
        {
            PersistenSingleton<UIManager>.Instance.Booster.CloseBoosterPanelImmediately();
        }
        UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
        {
            FF9StateSystem.Settings.StartGameTime = (Double)Time.time;
            PersistenSingleton<UIManager>.Instance.SetEventEnable(PersistenSingleton<UIManager>.Instance.IsEventEnable);
            PersistenSingleton<UIManager>.Instance.ChangeUIState(PersistenSingleton<UIManager>.Instance.PreviousState);
            if (PersistenSingleton<UIManager>.Instance.Dialogs != (UnityEngine.Object)null)
            {
                PersistenSingleton<UIManager>.Instance.Dialogs.PauseAllDialog(false);
            }
            vib.VIB_actuatorSet(0, this.previousVibLeft, this.previousVibRight);
            vib.VIB_actuatorSet(1, this.previousVibLeft, this.previousVibRight);
        };
        if (afterFinished != null)
        {
            sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        }
        base.Hide(sceneVoidDelegate);
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (base.OnKeyCancel(go) && !this.AndroidTVOnKeyCancel(go))
        {
            this.Hide((UIScene.SceneVoidDelegate)null);
        }
        return true;
    }

    public override Boolean OnKeyPause(GameObject go)
    {
        if (base.OnKeyPause(go))
        {
            this.Hide((UIScene.SceneVoidDelegate)null);
        }
        return true;
    }

    public override Boolean OnKeyLeftBumper(GameObject go)
    {
        if (base.OnKeyLeftBumper(go) && !this.AndroidTVOnKeyOnKeyLeftBumper(go))
        {
            UIManager.Input.HandleBoosterButton(BoosterType.BattleAssistance);
        }
        return true;
    }

    public override Boolean OnKeyRightBumper(GameObject go)
    {
        if (base.OnKeyRightBumper(go) && !this.AndroidTVOnKeyOnKeyRightBumper(go))
        {
            UIManager.Input.HandleBoosterButton(BoosterType.HighSpeedMode);
        }
        return true;
    }

    public override Boolean OnKeyLeftTrigger(GameObject go)
    {
        if (base.OnKeyLeftTrigger(go) && !this.AndroidTVOnKeyOnKeyLeftTrigger(go))
        {
            UIManager.Input.HandleBoosterButton(BoosterType.Attack9999);
        }
        return true;
    }

    public override Boolean OnKeyRightTrigger(GameObject go)
    {
        if (base.OnKeyRightTrigger(go) && !this.AndroidTVOnKeyOnKeyRightTrigger(go))
        {
            UIManager.Input.HandleBoosterButton(BoosterType.NoRandomEncounter);
        }
        return true;
    }

    public GameObject PauseDespText;

    private Single previousVibLeft;

    private Single previousVibRight;
}
