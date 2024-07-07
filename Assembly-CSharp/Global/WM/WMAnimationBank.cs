using System;
using UnityEngine;

public class WMAnimationBank : Singleton<WMAnimationBank>
{
    public void Initialize()
    {
        this.LoadResources();
    }

    public void LoadResources()
    {
        String folder = "Animations/GEO_SUB_W0_001/";
        this.ZidaneIdleClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_001_STAND", false);
        this.ZidaneRunClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_001_RUN", false);
        folder = "Animations/GEO_SUB_W0_002/";
        this.DaggerIdleClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_002_STAND", false);
        this.DaggerRunClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_002_RUN", false);
        folder = "Animations/GEO_SUB_W0_003/";
        this.ChocoboIdleClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_003_IDLE1_CHO", false);
        this.ChocoboRunClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_003_RUN_CHO", false);
        this.ChocoboFlyClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_003_FLY_CHO", false);
        folder = "Animations/GEO_SUB_W0_008/";
        this.BluenalusisuIdleClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_008_IDLE1_BN", false);
        this.BluenalusisuTakeOffClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_008_TAKEOFF_BN", false);
        folder = "Animations/GEO_SUB_W0_009/";
        this.HirudagarudeIdleClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_009_IDLE1_H3", false);
        this.HirudagarudeTakeOffClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_009_TAKEOFF_H3", false);
        folder = "Animations/GEO_SUB_W0_010/";
        this.InvincibleIdleClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_010_IDLE1_IV", false);
        this.InvincibleTakeOffClip = AssetManager.Load<AnimationClip>(folder + "ANH_SUB_W0_010_TAKEOFF_IV", false);
        if (this.ZidaneIdleClip != null)
            return;
        folder = "Animations/310/";
        this.ZidaneIdleClip = AssetManager.Load<AnimationClip>(folder + "4719", false);
        this.ZidaneRunClip = AssetManager.Load<AnimationClip>(folder + "4722", false);
        folder = "Animations/309/";
        this.DaggerIdleClip = AssetManager.Load<AnimationClip>(folder + "6143", false);
        this.DaggerRunClip = AssetManager.Load<AnimationClip>(folder + "6146", false);
        folder = "Animations/308/";
        this.ChocoboIdleClip = AssetManager.Load<AnimationClip>(folder + "4707", false);
        this.ChocoboRunClip = AssetManager.Load<AnimationClip>(folder + "4710", false);
        this.ChocoboFlyClip = AssetManager.Load<AnimationClip>(folder + "4991", false);
        folder = "Animations/321/";
        this.BluenalusisuIdleClip = AssetManager.Load<AnimationClip>(folder + "5144", false);
        this.BluenalusisuTakeOffClip = AssetManager.Load<AnimationClip>(folder + "5142", false);
        folder = "Animations/320/";
        this.HirudagarudeIdleClip = AssetManager.Load<AnimationClip>(folder + "5134", false);
        this.HirudagarudeTakeOffClip = AssetManager.Load<AnimationClip>(folder + "5138", false);
        folder = "Animations/317/";
        this.InvincibleIdleClip = AssetManager.Load<AnimationClip>(folder + "5127", false);
        this.InvincibleTakeOffClip = AssetManager.Load<AnimationClip>(folder + "5127", false);
    }

    private const String resources = "Animations/";

    public AnimationClip ZidaneIdleClip;

    public AnimationClip ZidaneRunClip;

    public AnimationClip ZidaneGetOnChocoboClip;

    public AnimationClip ZidaneIdleChocoboClip;

    public AnimationClip ZidaneRunChocoboClip;

    public AnimationClip DaggerIdleClip;

    public AnimationClip DaggerRunClip;

    public AnimationClip ChocoboIdleClip;

    public AnimationClip ChocoboRunClip;

    public AnimationClip ChocoboFlyClip;

    public AnimationClip BluenalusisuIdleClip;

    public AnimationClip BluenalusisuTakeOffClip;

    public AnimationClip HirudagarudeIdleClip;

    public AnimationClip HirudagarudeTakeOffClip;

    public AnimationClip InvincibleIdleClip;

    public AnimationClip InvincibleTakeOffClip;
}
