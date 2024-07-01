using System;
using UnityEngine;

public class OverlayCanvas : PersistenSingleton<OverlayCanvas>
{
    public void Restart()
    {
        if (!this.isRestart)
        {
            this.isRestart = true;
            this.overlayBoosterUI.Restart();
            this.pillarBoxOverrideManager.Restart();
            this.topClock.Restart();
            this.leftClock.Restart();
            this.topBattery.Restart();
            this.rightBattery.Restart();
            Singleton<VirtualAnalog>.Instance.Restart();
        }
    }

    public static Vector2 ReferenceScreenSize = new Vector2(1024f, 768f);

    public OverlayBoosterUI overlayBoosterUI;

    public PillarBoxOverrideManager pillarBoxOverrideManager;

    public ClockUI topClock;

    public ClockUI leftClock;

    public BatteryUI topBattery;

    public BatteryUI rightBattery;

    private Boolean isRestart;
}
