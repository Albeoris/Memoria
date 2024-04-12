using UnityEngine.SocialPlatforms;

public static class SocialExtension
{
	public static void ProcessCallbacks(this ILocalUser socialUser)
	{
		((SteamLocalUser)socialUser).ProcessCallbacks();
	}
}
