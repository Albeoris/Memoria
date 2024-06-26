using System;
using System.Runtime.InteropServices;
using Memoria.Prime;
using SiliconStudio;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SteamSdkWrapper : ISocialPlatform
{
	private SteamSdkWrapper()
	{
		this.localUser = new SteamLocalUser(this);
	}

	public static event Action<Boolean> OnStatsReceived;

	public static event Action<Boolean> OnStatsStored;

	public static event Action<Boolean> OnAchievementStored;

	public ILocalUser localUser { get; private set; }

	public void Activate()
	{
		UnityEngine.Social.Active = this;
	}

	public void Authenticate(ILocalUser unused, Action<Boolean> callback)
	{
		this.Authenticate(callback);
	}

	public void Authenticate(Action<Boolean> callback)
	{
		SteamSdkWrapper.OnStatsReceived = callback;
		SteamSdkWrapper.RegisterStatsReceivedCallback(new SteamSdkWrapper.ResultCallbackDelegate(SteamSdkWrapper.DispatchStatsReceivedEvent));
		SteamSdkWrapper.RegisterStatsStoredCallback(new SteamSdkWrapper.ResultCallbackDelegate(SteamSdkWrapper.DispatchStatsStoredEvent));
		SteamSdkWrapper.RegisterAchievementStoredCallback(new SteamSdkWrapper.VoidCallbackDelegate(SteamSdkWrapper.DispatchAchievementStoredEvent));
		this.Initialized = TryInitialize();
	}

	public static void RequestStats(Action<Boolean> callback)
	{
		SteamSdkWrapper.OnStatsReceived = callback;
		SteamSdkWrapper.RequestStats();
	}

	public void ReportProgress(String achievementId, Double unusedProgress, Action<Boolean> callback)
	{
		SteamSdkWrapper.OnAchievementStored = callback;
		this.TriggerAchievement(achievementId);
	}

	public static void UpdateStatProgress(String statToUpdate, Int32 progress, Action<Boolean> callback)
	{
		SteamSdkWrapper.OnStatsStored = callback;
		SteamSdkWrapper.UpdateStatProgress(statToUpdate, progress);
	}

	private static void DispatchStatsReceivedEvent(Boolean result)
	{
		if (SteamSdkWrapper.OnStatsReceived != null)
		{
			SteamSdkWrapper.AchievementReady = result;
			SteamSdkWrapper.OnStatsReceived(result);
			SteamSdkWrapper.OnStatsReceived = null;
		}
	}

	private static void DispatchStatsStoredEvent(Boolean result)
	{
		if (SteamSdkWrapper.OnStatsStored != null)
		{
			SteamSdkWrapper.OnStatsStored(result);
			SteamSdkWrapper.OnStatsStored = null;
		}
	}

	private static void DispatchAchievementStoredEvent()
	{
		if (SteamSdkWrapper.OnAchievementStored != null)
		{
			SteamSdkWrapper.OnAchievementStored(true);
			SteamSdkWrapper.OnAchievementStored = null;
		}
	}

	public void LoadAchievements(Action<IAchievement[]> callback)
	{
		throw new NotImplementedException();
	}

	public IAchievement CreateAchievement()
	{
		throw new NotImplementedException();
	}

	public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
	{
		throw new NotImplementedException();
	}

	public void LoadUsers(String[] userIDs, Action<IUserProfile[]> callback)
	{
		throw new NotImplementedException();
	}

	public void ReportScore(Int64 score, String board, Action<Boolean> callback)
	{
		throw new NotImplementedException();
	}

	public void LoadScores(String leaderboardID, Action<IScore[]> callback)
	{
		throw new NotImplementedException();
	}

	public ILeaderboard CreateLeaderboard()
	{
		throw new NotImplementedException();
	}

	public void ShowAchievementsUI()
	{
	}

	public void ShowLeaderboardUI()
	{
		throw new NotImplementedException();
	}

	public void LoadFriends(ILocalUser user, Action<Boolean> callback)
	{
		throw new NotImplementedException();
	}

	public void LoadScores(ILeaderboard board, Action<Boolean> callback)
	{
		throw new NotImplementedException();
	}

	public Boolean GetLoading(ILeaderboard board)
	{
		throw new NotImplementedException();
	}
	
	public static Boolean IsInitialized { get; private set; }

	public static Boolean TryInitialize()
	{
		if (!IsInitialized)
		{
			IsInitialized = Init();
			Log.Message($"[{nameof(SteamSdkWrapper)}] Initialized: {IsInitialized}");
		}
		else
		{
			Boolean stats = RequestStats();
			Log.Message($"[{nameof(SteamSdkWrapper)}] Request stats: {stats}");
		}

		return IsInitialized;
	}

	[DllImport("steamwrapper")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern Boolean Init();

	[DllImport("steamwrapper")]
	private static extern void Shutdown_Internal();

	[DllImport("steamwrapper")]
	private static extern void ProcessCallbacks_Internal();

	[DllImport("steamwrapper")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern Boolean TriggerAchievement_Internal(IntPtr achievementIdPointer);

	[DllImport("steamwrapper")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern Boolean UpdateStatProgress_Internal(IntPtr statIdPointer, Int32 rawProgress);

	[DllImport("steamwrapper")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern Boolean ClearAchievement_Internal(IntPtr achievementIdPointer);

	[DllImport("steamwrapper")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern Boolean IsCloudEnabled_Internal();

	[DllImport("steamwrapper")]
	private static extern void RegisterStatsReceivedCallback(SteamSdkWrapper.ResultCallbackDelegate statsReceivedCallback);

	[DllImport("steamwrapper")]
	private static extern void RegisterStatsStoredCallback(SteamSdkWrapper.ResultCallbackDelegate statsStoredCallback);

	[DllImport("steamwrapper")]
	private static extern void RegisterAchievementStoredCallback(SteamSdkWrapper.VoidCallbackDelegate achievementStoredCallback);

	[DllImport("steamwrapper")]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern Boolean RequestStats();

	[DllImport("steamwrapper")]
	public static extern void SetSendAchievementToCS(SiliconStudio.Social.SendAchievementToCSCallback method);

	[DllImport("steamwrapper")]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern Boolean SteamAPIRestartAppIfNecessary();

	public Boolean Initialized { get; private set; }

	private static Boolean AchievementReady { get; set; }

	public static SteamSdkWrapper Create()
	{
		return new SteamSdkWrapper();
	}

	public Boolean TriggerAchievement(String achievementId)
	{
		if (!this.Initialized)
		{
			global::Debug.LogWarning("Invalid Operation Trigger Achievement: please initialize SDK");
			return false;
		}
		if (!SteamSdkWrapper.AchievementReady)
		{
			global::Debug.LogWarning("Invalid Operation Trigger Achievement: Achievement system is not ready");
			return false;
		}
		IntPtr intPtr = Marshal.StringToHGlobalAnsi(achievementId);
		Boolean result = SteamSdkWrapper.TriggerAchievement_Internal(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public static Boolean UpdateStatProgress(String statId, Int32 rawProgress)
	{
		if (!SteamSdkWrapper.AchievementReady)
		{
			global::Debug.LogWarning("Invalid Operation Trigger Achievement: Achievement system is not ready");
			return false;
		}
		IntPtr intPtr = Marshal.StringToHGlobalAnsi(statId);
		Boolean result = SteamSdkWrapper.UpdateStatProgress_Internal(intPtr, rawProgress);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public Boolean ClearAchievement(String achievementId)
	{
		if (!this.Initialized)
		{
			global::Debug.LogWarning("Invalid Operation Clear Achievement: please initialize SDK");
			return false;
		}
		if (!SteamSdkWrapper.AchievementReady)
		{
			global::Debug.LogWarning("Invalid Operation Clear Achievement: Achievement system is not ready");
			return false;
		}
		IntPtr intPtr = Marshal.StringToHGlobalAnsi(achievementId);
		Boolean result = SteamSdkWrapper.ClearAchievement_Internal(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public Boolean IsCloudEnabled()
	{
		if (!this.Initialized)
		{
			global::Debug.LogWarning("Invalid Operation Clear Achievement: please initialize SDK");
			return false;
		}
		if (!SteamSdkWrapper.AchievementReady)
		{
			global::Debug.LogWarning("Invalid Operation Clear Achievement: Achievement system is not ready");
			return false;
		}
		Boolean flag = SteamSdkWrapper.IsCloudEnabled_Internal();
		global::Debug.Log("IsCloudEnabled_Internal() => " + flag);
		return flag;
	}

	public void ProcessCallbacks()
	{
		if (this.Initialized)
		{
			SteamSdkWrapper.ProcessCallbacks_Internal();
		}
	}

	public void Shutdown()
	{
		if (this.Initialized)
		{
			SteamSdkWrapper.Shutdown_Internal();
		}
	}

	public const String LibraryName = "steamwrapper";

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private delegate void ResultCallbackDelegate([MarshalAs(UnmanagedType.I1)] Boolean result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private delegate void VoidCallbackDelegate();
}
