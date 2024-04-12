using AOT;
using Assets.SiliconSocial;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Object = System.Object;

namespace SiliconStudio
{
	public static class Social
	{
		private static Boolean _isAuthenticated
		{
			get
			{
				return Social.m_isAuthenticated;
			}
		}

		public static void InitializeSocialPlatform()
		{
			global::Debug.Log(String.Concat(new Object[]
			{
				"KF: InitializeSocialPlatform _hasCreated = ",
				Social._hasCreated,
				", IsEnabled = ",
				true
			}));
			if (Social._hasCreated || Application.isEditor || false)
			{
				return;
			}
			SteamSdkWrapper steamSdkWrapper = SteamSdkWrapper.Create();
			steamSdkWrapper.Activate();
			Social.steamAchievementData = new SteamAchievementData[87];
			Social._hasCreated = true;
		}

		[MonoPInvokeCallback(typeof(Social.SendAchievementToCSCallback))]
		public unsafe static Int32 SendAchievementToCS(void* p)
		{
			global::Debug.Log("SendAchievementToCS marshalling");
			Int32 num = Marshal.SizeOf(typeof(SteamAchievementData));
			global::Debug.Log("structSize = " + num);
			IntPtr intPtr = (IntPtr)p;
			for (Int32 i = 0; i < (Int32)Social.steamAchievementData.Length; i++)
			{
				IntPtr ptr = new IntPtr(intPtr.ToInt64() + (Int64)(num * i));
				Social.steamAchievementData[i] = (SteamAchievementData)Marshal.PtrToStructure(ptr, typeof(SteamAchievementData));
				global::Debug.Log(String.Concat(new Object[]
				{
					"SendAchievementToCS marshalling data [",
					i,
					"] id = ",
					Social.steamAchievementData[i].id,
					", completed = ",
					Social.steamAchievementData[i].completed
				}));
			}
			return 0;
		}

		public static void Authenticate(Boolean needToResyncSystemAchievement = false)
		{
			global::Debug.Log("KF: Social.Authenticate _hasCreated = " + Social._hasCreated);
			if (!Social._hasCreated || Application.isEditor)
			{
				return;
			}
			global::Debug.Log("BEFORE calling SteamSdkWrapper.SetSendAchievementToCS(SendAchievementToCS)");
			unsafe
			{
				SteamSdkWrapper.SetSendAchievementToCS(new Social.SendAchievementToCSCallback(Social.SendAchievementToCS));
			}
			global::Debug.Log("AFTER calling SteamSdkWrapper.SetSendAchievementToCS(SendAchievementToCS)");
			UnityEngine.Social.localUser.Authenticate(delegate (Boolean success)
			{
				global::Debug.Log("KF: authenticate's callback success = " + success);
				if (success)
				{
					global::Debug.Log("KF: PAUSE: authen succuessful");
					Social.m_isAuthenticated = true;
					if (needToResyncSystemAchievement)
					{
						global::Debug.Log("Calling ResyncSystemAchievements after the first authentication");
						AchievementManager.ResyncSystemAchievements();
					}
				}
				else
				{
					Social.m_isAuthenticated = false;
				}
			});
		}

		public static Boolean IsSocialPlatformAuthenticated()
		{
			return !Application.isEditor && Social._isAuthenticated;
		}

		public static void ProcessCallbacks()
		{
			if (!Social._hasCreated || Application.isEditor)
			{
				return;
			}
			UnityEngine.Social.localUser.ProcessCallbacks();
		}

		public static void ShowAchievement()
		{
			global::Debug.Log("KF: ShowAchievement");
			if (!Social.IsSocialPlatformAuthenticated())
			{
				global::Debug.Log("KF: ShowAchievement in IF");
				Social.Authenticate(false);
			}
			else
			{
				global::Debug.Log("KF: ShowAchievement in ELSE");
				UnityEngine.Social.ShowAchievementsUI();
			}
		}

		public static void ReportAchievement(AcheivementKey key, String achievementId, Int32 percentageProgress, Action<Boolean> callback)
		{
			if (!Social.IsSocialPlatformAuthenticated())
			{
				return;
			}
			UnityEngine.Social.ReportProgress(achievementId, (Double)percentageProgress, callback);
		}

		public static void UpdateStatSteam(String statId, Int32 rawProgress, Action<Boolean> callback)
		{
			SteamSdkWrapper.UpdateStatProgress(statId, rawProgress, callback);
		}

		public static void ResetAchievements()
		{
		}

		private static Boolean IsSteamCloudEnabled()
		{
			if (Application.isEditor)
			{
				return true;
			}
			if (!Social._isAuthenticated)
			{
				return false;
			}
			ILocalUser localUser = UnityEngine.Social.localUser;
			if (localUser != null)
			{
				Boolean flag = ((SteamLocalUser)localUser).IsCloudEnabled();
				global::Debug.Log("Is Steam Cloud available?:" + flag);
				return flag;
			}
			return false;
		}

		public static Boolean IsCloudAvailable()
		{
			return Social.IsSteamCloudEnabled();
		}

		public static IEnumerator Cloud_Save(MonoBehaviour owner, byte[] data, TimeSpan playTime, Action<bool, Social.ResponseData.Status> callback)
		{
			bool isSuccess = false;
			Social.ResponseData.Status status = Social.ResponseData.Status.UnknownError;
			object result = null;
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				if (Application.platform != RuntimePlatform.Android)
				{
					if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
					{
						Social.CoroutineWithData cd = new Social.CoroutineWithData(owner, Social.Steam_Save(data));
						yield return cd.coroutine;
						result = cd.result;
					}
				}
			}
			if (result != null)
			{
				Social.ResponseData resultData = (Social.ResponseData)result;
				if (resultData.m_data != null)
				{
					isSuccess = (bool)resultData.m_data;
				}
				status = resultData.m_status;
			}
			callback(isSuccess, status);
			yield break;
		}

		public static IEnumerator Cloud_Load(MonoBehaviour owner, Action<byte[], Social.ResponseData.Status> callback)
		{
			global::Debug.Log("in Cloud_Load 1");
			global::Debug.Log("in Cloud_Load 2");
			object result = null;
			Social.ResponseData.Status status = Social.ResponseData.Status.UnknownError;
			byte[] readBytes = null;
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				if (Application.platform != RuntimePlatform.Android)
				{
					if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
					{
						Social.CoroutineWithData cd = new Social.CoroutineWithData(owner, Social.Steam_Load());
						yield return cd.coroutine;
						result = cd.result;
					}
				}
			}
			global::Debug.Log("in Cloud_Load 5");
			if (result != null)
			{
				Social.ResponseData resultData = (Social.ResponseData)result;
				if (resultData.m_data != null)
				{
					readBytes = (byte[])resultData.m_data;
				}
				status = resultData.m_status;
			}
			global::Debug.Log("in Cloud_Load 6");
			callback(readBytes, status);
			yield break;
		}

		public static IEnumerator Cloud_IsFileExist(MonoBehaviour owner, Action<bool, Social.ResponseData.Status> callback)
		{
			global::Debug.Log("IN CLOUD_ISFILEEXIST1");
			bool isSuccess = false;
			Social.ResponseData.Status status = Social.ResponseData.Status.UnknownError;
			object result = null;
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				if (Application.platform != RuntimePlatform.Android)
				{
					if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
					{
						Social.CoroutineWithData cd = new Social.CoroutineWithData(owner, Social.Steam_FileExist());
						yield return cd.coroutine;
						result = cd.result;
					}
				}
			}
			global::Debug.Log("IN CLOUD_ISFILEEXIST3");
			if (result != null)
			{
				Social.ResponseData resultData = (Social.ResponseData)result;
				if (resultData.m_data != null)
				{
					isSuccess = (bool)resultData.m_data;
				}
				status = resultData.m_status;
			}
			global::Debug.Log("IN CLOUD_ISFILEEXIST4");
			callback(isSuccess, status);
			yield break;
		}

		private static void ClearTempData()
		{
			Array.Clear(Social._saveData, 0, (Int32)Social._saveData.Length);
			Array.Clear(Social._loadData, 0, (Int32)Social._loadData.Length);
			Social._isFileExist = false;
			Social._totalPlayTime = default(TimeSpan);
		}

		private static String GetCloudSaveFileName()
		{
			if (SharedDataBytesStorage.MetaData.FilePath == String.Empty)
				throw new Exception("SharedDataBytesStorage not initialized.");

			if (SharedDataBytesStorage.MetaData.FilePath.EndsWith("_jp.dat"))
				return "FF9_CloudSaveFile_jp.sav";

			return "FF9_CloudSaveFile_ww.sav";
		}

		//   private static IEnumerator iCloud_Save(Byte[] data)
		//{
		//	JCloudDocumentOperation operation = JCloudDocument.DirectoryExists("FF9_SaveDocument");
		//	while (!operation.finished)
		//	{
		//		yield return null;
		//	}
		//	if (operation.error != null)
		//	{
		//		Social.HandleDocumentError(operation.error.Value);
		//		yield return new Social.ResponseData(false, operation.error.Value);
		//		yield break;
		//	}
		//	if (!(Boolean)operation.result)
		//	{
		//		operation = JCloudDocument.DirectoryCreate("FF9_SaveDocument");
		//		while (!operation.finished)
		//		{
		//			yield return null;
		//		}
		//		if (operation.error != null)
		//		{
		//			Social.HandleDocumentError(operation.error.Value);
		//			yield return new Social.ResponseData(false, operation.error.Value);
		//			yield break;
		//		}
		//	}
		//    operation = JCloudDocument.FileWriteAllBytes("FF9_SaveDocument/" + GetCloudSaveFileName(), data);
		//	while (!operation.finished)
		//	{
		//		yield return null;
		//	}
		//	if (operation.error != null)
		//	{
		//		Social.HandleDocumentError(operation.error.Value);
		//		yield return new Social.ResponseData(false, operation.error.Value);
		//		yield break;
		//	}
		//	yield return new Social.ResponseData(true, null);
		//	yield break;
		//}

		//private static IEnumerator iCloud_Load()
		//{
		//	JCloudDocumentOperation operation = JCloudDocument.FileReadAllBytes("FF9_SaveDocument/" + GetCloudSaveFileName());
		//	while (!operation.finished)
		//	{
		//		yield return null;
		//	}
		//	if (operation.error != null)
		//	{
		//		Social.HandleDocumentError(operation.error.Value);
		//		yield return new Social.ResponseData(null, operation.error.Value);
		//		yield break;
		//	}
		//	Social._icloudError = (String)null;
		//	Byte[] gameBytes = operation.result as Byte[];
		//	yield return new Social.ResponseData(gameBytes, null);
		//	yield break;
		//}

		//private static IEnumerator iCloud_FileExist()
		//{
		//	JCloudDocumentOperation operation = JCloudDocument.FileExists("FF9_SaveDocument/" + GetCloudSaveFileName());
		//	while (!operation.finished)
		//	{
		//		yield return null;
		//	}
		//	if (operation.error != null)
		//	{
		//		Social.HandleDocumentError(operation.error.Value);
		//		yield return new Social.ResponseData(null, operation.error.Value);
		//		yield break;
		//	}
		//	Social._icloudError = (String)null;
		//	yield return new Social.ResponseData(operation.result, null);
		//	yield break;
		//}

		private static IEnumerator Steam_Save(Byte[] data)
		{
			Boolean success = false;
			Social.ResponseData returnData = new Social.ResponseData(null, null);
			try
			{
				String rootDir = Social.GetRootDir_SteamCloud();
				if (!Directory.Exists(rootDir))
				{
					Directory.CreateDirectory(rootDir);
				}
				String filePath = Social.GetFilePath_SteamCloud();
				File.WriteAllBytes(filePath, data);
				success = true;
			}
			catch (Exception)
			{
				success = false;
				returnData.m_status = Social.ResponseData.Status.UnknownError;
			}
			returnData.m_data = success;
			yield return returnData;
			yield break;
		}

		private static IEnumerator Steam_Load()
		{
			String filePath = Social.GetFilePath_SteamCloud();
			if (!File.Exists(filePath))
			{
				yield return new Social.ResponseData(null, null)
				{
					m_status = Social.ResponseData.Status.DocumentNotFound
				};
				yield break;
			}
			Byte[] gameBytes = File.ReadAllBytes(filePath);
			yield return new Social.ResponseData(gameBytes, null);
			yield break;
		}

		private static IEnumerator Steam_FileExist()
		{
			yield return new Social.ResponseData(File.Exists(Social.GetFilePath_SteamCloud()), null);
			yield break;
		}

		private static String GetRootDir_SteamCloud()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Square Enix\\FINAL FANTASY IX";
		}

		private static String GetFilePath_SteamCloud()
		{
			return Social.GetRootDir_SteamCloud() + "\\" + GetCloudSaveFileName();
		}

		public const Boolean IsEnabled = true;

		private const UInt32 MaxSaveSlot = 1u;

		private const Boolean AllowCreateNew = false;

		private const Boolean AllowDelete = true;

		private const String SteamCloudRootDir = "Square Enix\\FINAL FANTASY IX";

		private static Boolean m_isAuthenticated;

		private static Boolean _hasCreated = false;

		public static SteamAchievementData[] steamAchievementData;

		private static Byte[] _saveData;

		private static Byte[] _loadData;

		private static Boolean _isFileExist;

		private static TimeSpan _totalPlayTime;

		private static Social.SavedGamesCloudStatus _saveStatus;

		private static Social.SavedGamesCloudStatus _loadStatus;

		private static Social.SavedGamesCloudStatus _checkFileStatus;

		private enum SavedGamesCloudStatus
		{
			None,
			Saving,
			Loading,
			CheckingFileExist,
			FinishWithSuccess,
			FinishWithError
		}

		public class CoroutineWithData
		{
			public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
			{
				global::Debug.Log("IN COUROUTINEWITHDATA1");
				this.target = target;
				this.coroutine = owner.StartCoroutine(this.Run());
			}

			public Coroutine coroutine { get; private set; }

			private IEnumerator Run()
			{
				Single timeCounter = 0f;
				while (this.target.MoveNext())
				{
					timeCounter += Time.fixedDeltaTime;
					if (timeCounter >= 90f)
					{
						this.result = new Social.ResponseData(null, null)
						{
							m_status = Social.ResponseData.Status.DownloadTimeout
						};
						yield break;
					}
					this.result = this.target.Current;
					yield return this.result;
				}
				yield break;
			}

			public const Single ConnectionTimeout = 90f;

			public Object result;

			private IEnumerator target;
		}

		public class ResponseData
		{
			public ResponseData(Object result, Object status)
			{
				this.m_data = result;
				if (status == null)
				{
					this.m_status = Social.ResponseData.Status.Success;
				}
				else if (Application.platform != RuntimePlatform.Android)
				{
					if (Application.platform != RuntimePlatform.IPhonePlayer)
						this.m_status = Social.ResponseData.Status.UnknownError;
				}
			}

			public Object m_data;

			public Social.ResponseData.Status m_status;

			public enum Status
			{
				Success,
				UnknownError,
				DocumentNotFound,
				DownloadTimeout,
				AutenticationError,
				ConnectionError
			}
		}

		public unsafe delegate Int32 SendAchievementToCSCallback(void* p);
	}
}
