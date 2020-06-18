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
		String[] animInfo;
		String str = "Animations/GEO_SUB_W0_001/";
		this.ZidaneIdleClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_001_STAND", out animInfo, false);
		this.ZidaneRunClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_001_RUN", out animInfo, false);
		str = "Animations/GEO_SUB_W0_002/";
		this.DaggerIdleClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_002_STAND", out animInfo, false);
		this.DaggerRunClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_002_RUN", out animInfo, false);
		str = "Animations/GEO_SUB_W0_003/";
		this.ChocoboIdleClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_003_IDLE1_CHO", out animInfo, false);
		this.ChocoboRunClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_003_RUN_CHO", out animInfo, false);
		this.ChocoboFlyClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_003_FLY_CHO", out animInfo, false);
		str = "Animations/GEO_SUB_W0_008/";
		this.BluenalusisuIdleClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_008_IDLE1_BN", out animInfo, false);
		this.BluenalusisuTakeOffClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_008_TAKEOFF_BN", out animInfo, false);
		str = "Animations/GEO_SUB_W0_009/";
		this.HirudagarudeIdleClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_009_IDLE1_H3", out animInfo, false);
		this.HirudagarudeTakeOffClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_009_TAKEOFF_H3", out animInfo, false);
		str = "Animations/GEO_SUB_W0_010/";
		this.InvincibleIdleClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_010_IDLE1_IV", out animInfo, false);
		this.InvincibleTakeOffClip = AssetManager.Load<AnimationClip>(str + "ANH_SUB_W0_010_TAKEOFF_IV", out animInfo, false);
		if (this.ZidaneIdleClip != (UnityEngine.Object)null)
		{
			return;
		}
		str = "Animations/310/";
		this.ZidaneIdleClip = AssetManager.Load<AnimationClip>(str + "4719", out animInfo, false);
		this.ZidaneRunClip = AssetManager.Load<AnimationClip>(str + "4722", out animInfo, false);
		str = "Animations/309/";
		this.DaggerIdleClip = AssetManager.Load<AnimationClip>(str + "6143", out animInfo, false);
		this.DaggerRunClip = AssetManager.Load<AnimationClip>(str + "6146", out animInfo, false);
		str = "Animations/308/";
		this.ChocoboIdleClip = AssetManager.Load<AnimationClip>(str + "4707", out animInfo, false);
		this.ChocoboRunClip = AssetManager.Load<AnimationClip>(str + "4710", out animInfo, false);
		this.ChocoboFlyClip = AssetManager.Load<AnimationClip>(str + "4991", out animInfo, false);
		str = "Animations/321/";
		this.BluenalusisuIdleClip = AssetManager.Load<AnimationClip>(str + "5144", out animInfo, false);
		this.BluenalusisuTakeOffClip = AssetManager.Load<AnimationClip>(str + "5142", out animInfo, false);
		str = "Animations/320/";
		this.HirudagarudeIdleClip = AssetManager.Load<AnimationClip>(str + "5134", out animInfo, false);
		this.HirudagarudeTakeOffClip = AssetManager.Load<AnimationClip>(str + "5138", out animInfo, false);
		str = "Animations/317/";
		this.InvincibleIdleClip = AssetManager.Load<AnimationClip>(str + "5127", out animInfo, false);
		this.InvincibleTakeOffClip = AssetManager.Load<AnimationClip>(str + "5127", out animInfo, false);
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
