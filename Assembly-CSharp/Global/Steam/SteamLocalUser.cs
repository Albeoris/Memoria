using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SteamLocalUser : ILocalUser, IUserProfile
{
	public SteamLocalUser(SteamSdkWrapper steamSdkWrapper)
	{
		this.steamSdkWrapper = steamSdkWrapper;
	}

	public String userName { get; private set; }

	public String id { get; private set; }

	public Boolean isFriend { get; private set; }

	public UserState state { get; private set; }

	public Texture2D image { get; private set; }

	public IUserProfile[] friends { get; private set; }

	public Boolean authenticated { get; private set; }

	public Boolean underage { get; private set; }

	public void Authenticate(Action<Boolean> callback)
	{
		this.steamSdkWrapper.Authenticate(callback);
	}

	public void ProcessCallbacks()
	{
		this.steamSdkWrapper.ProcessCallbacks();
	}

	public void LoadFriends(Action<Boolean> callback)
	{
		throw new NotImplementedException();
	}

	public Boolean ClearAchievement(String achievementId)
	{
		return this.steamSdkWrapper.ClearAchievement(achievementId);
	}

	public Boolean IsCloudEnabled()
	{
		return this.steamSdkWrapper.IsCloudEnabled();
	}

	private readonly SteamSdkWrapper steamSdkWrapper;
}
