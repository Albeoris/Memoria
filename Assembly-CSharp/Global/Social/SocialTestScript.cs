using Assets.Scripts.Common;
using Assets.SiliconSocial;
using System;
using UnityEngine;
using Object = System.Object;

public class SocialTestScript : MonoBehaviour
{
	private void Start()
	{
		this.InitializeUI();
		global::Debug.Log("KF: before calling InitializeSocialPlatform()");
		SiliconStudio.Social.InitializeSocialPlatform();
		global::Debug.Log("KF: before calling authenticate()");
		SiliconStudio.Social.Authenticate(false);
	}

	private void InitializeUI()
	{
		GameObject.Find("Show Achievement Button").SetActive(false);
	}

	public void Update()
	{
		SiliconStudio.Social.ProcessCallbacks();
	}

	public void Unlock1()
	{
		global::Debug.Log("Unlock Defeat100");
		Int32 num = UnityEngine.Random.Range(1, 100);
		this.test002Progress += num;
		AchievementManager.ReportAchievement(AcheivementKey.Defeat100, this.test002Progress);
	}

	public void Unlock2()
	{
		global::Debug.Log("Unlock Excalibur");
		AchievementManager.ReportAchievement(AcheivementKey.Excalibur, 1);
	}

	public void Unlock3()
	{
		global::Debug.Log("Unlock Blackjack");
		AchievementManager.ReportAchievement(AcheivementKey.Blackjack, 1);
	}

	public void Unlock4()
	{
		global::Debug.Log("Unlock PartyMen");
		AchievementManager.ReportAchievement(AcheivementKey.PartyMen, 1);
	}

	public void Unlock5()
	{
		for (Int32 i = 0; i < 87; i++)
		{
			AchievementManager.ProcessAchievementReport((AcheivementKey)i, 10000, 100, 5);
		}
	}

	public void ShowAchievement()
	{
		SiliconStudio.Social.ShowAchievement();
	}

	public void ResetAchievements()
	{
		this.ResetSteamAchievements();
	}

	private void ResetSteamAchievements()
	{
		SteamLocalUser steamLocalUser = UnityEngine.Social.Active.localUser as SteamLocalUser;
		if (steamLocalUser != null)
		{
			foreach (Object obj in Enum.GetValues(typeof(AcheivementKey)))
			{
				AcheivementKey key = (AcheivementKey)((Int32)obj);
				steamLocalUser.ClearAchievement(AchievementManager.GetRefKeyForPlatform(key));
			}
		}
	}

	public void Back()
	{
		SceneDirector.Replace("MainMenu", SceneTransition.FadeOutToBlack_FadeIn, true);
	}

	private Int32 test002Progress;
}
