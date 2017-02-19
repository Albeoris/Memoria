using System;
using System.Collections.Generic;
using SiliconStudio;
using UnityEngine;

public abstract class ISharedDataSerializer : MonoBehaviour
{
	public abstract void LoadSlotPreview(Int32 slotID, ISharedDataSerializer.OnLoadSlotFinish onFinishDelegate);

	public abstract void Save(Int32 slotID, Int32 saveID, ISharedDataSerializer.OnSaveLoadStart onStartDelegate, ISharedDataSerializer.OnSaveFinish onFinishDelegate);

	public abstract void Load(Int32 slotID, Int32 saveID, ISharedDataSerializer.OnSaveLoadStart onStartDelegate, ISharedDataSerializer.OnLoadFinish onFinishDelegate);

	public abstract void Autosave(ISharedDataSerializer.OnAutosaveAutoloadStart onStartDelegate, ISharedDataSerializer.OnAutosaveFinish onFinishDelegate);

	public abstract void Autoload(ISharedDataSerializer.OnAutosaveAutoloadStart onStartDelegate, ISharedDataSerializer.OnAutoloadFinish onFinishDelegate);

	public abstract void HasAutoload(ISharedDataSerializer.OnHasAutoloadFinish onFinishDelegate);

	public abstract void LoadCloudSyncPreview(ISharedDataSerializer.OnSyncCloudSlotStart onStartDelegate, ISharedDataSerializer.OnSyncCloudSlotFinish onFinishDelegate);

	public abstract void UploadToCloud(ISharedDataSerializer.OnSyncCloudSlotStart onStartDelegate, ISharedDataSerializer.OnSyncCloudSlotFinish onFinishDelegate);

	public abstract void DownloadFromCloud(ISharedDataSerializer.OnSyncCloudSlotStart onStartDelegate, ISharedDataSerializer.OnSyncCloudSlotFinish onFinishDelegate);

	public abstract void GetLocalSaveLatestTimestamp(ISharedDataSerializer.OnGetLatestSaveTimestamp onFinishDelegate);

	public abstract void GetCloudSaveLatestTimestamp(ISharedDataSerializer.OnGetLatestSaveTimestamp onFinishDelegate);

	public abstract void GetGameFinishFlag(ISharedDataSerializer.OnGetGameFinishFlag onFinishDelegate);

	public abstract void SetGameFinishFlagWithTrue(ISharedDataSerializer.OnSetGameFinishFlag onFinishDelegate);

	public abstract void GetSelectedLanguage(ISharedDataSerializer.OnGetSelectedLanguage onFinishDelegate);

	public abstract void SetSelectedLanguage(Int32 selectedLanguage, ISharedDataSerializer.OnSetSelectedLanguage onFinishDelegate);

	public abstract void GetIsAutoLogin(ISharedDataSerializer.OnGetIsAutoLogin onFinishDelegate);

	public abstract void SetIsAutoLogin(SByte isAutoLogin, ISharedDataSerializer.OnSetIsAutoLogin onFinishDelegate);

	public abstract void GetSystemAchievementStatuses(ISharedDataSerializer.OnGetSystemAchievementStatuses onFinishDelegate);

	public abstract void SetSystemAchievementStatuses(Byte[] systemAchievementStatuses, ISharedDataSerializer.OnSetSystemAchievementStatuses onFinishDelegate);

	public abstract void GetScreenRotation(ISharedDataSerializer.OnGetScreenRotation onFinishDelegate);

	public abstract void SetScreenRotation(Byte screenRotation, ISharedDataSerializer.OnSetScreenRotation onFinishDelegate);

	public abstract void ReadSystemData(ISharedDataSerializer.OnReadSystemData onFinishDelegate);

	public abstract void ClearAllData();

	public static DataSerializerErrorCode ConvertCloudStatusToDataSerializerErrorCode(SiliconStudio.Social.ResponseData.Status input)
	{
		DataSerializerErrorCode result;
		switch (input)
		{
		case SiliconStudio.Social.ResponseData.Status.UnknownError:
			result = DataSerializerErrorCode.CloudDataUnknownError;
			break;
		case SiliconStudio.Social.ResponseData.Status.DocumentNotFound:
			result = DataSerializerErrorCode.CloudFileNotFound;
			break;
		case SiliconStudio.Social.ResponseData.Status.DownloadTimeout:
			result = DataSerializerErrorCode.CloudConnectionTimeout;
			break;
		default:
			result = DataSerializerErrorCode.Success;
			break;
		}
		return result;
	}

	public static DataSerializerErrorCode LastErrno;

	public ISharedDataParser Parser;

	public ISharedDataStorage Storage;

	public ISharedDataEncryption Encryption;

	public delegate void OnLoadSlotFinish(DataSerializerErrorCode errNo, Int32 slotID, List<SharedDataPreviewSlot> data);

	public delegate void OnSaveLoadStart(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID);

	public delegate void OnSaveFinish(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID, Boolean isSuccess, SharedDataPreviewSlot preview);

	public delegate void OnLoadFinish(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID, Boolean isSuccess);

	public delegate void OnAutosaveAutoloadStart(DataSerializerErrorCode errNo);

	public delegate void OnAutosaveFinish(DataSerializerErrorCode errNo, Boolean isSuccess);

	public delegate void OnAutoloadFinish(DataSerializerErrorCode errNo, Boolean isSuccess);

	public delegate void OnHasAutoloadFinish(DataSerializerErrorCode errNo, Boolean isSuccess);

	public delegate void OnSyncCloudSlotStart(DataSerializerErrorCode errNo);

	public delegate void OnSyncCloudSlotFinish(DataSerializerErrorCode errNo, Boolean isSuccess, SharedDataPreviewSlot localData, SharedDataPreviewSlot cloudData);

	public delegate void OnGetLatestSaveTimestamp(DataSerializerErrorCode errNo, Boolean isSuccess, Double timestamp);

	public delegate void OnGetGameFinishFlag(DataSerializerErrorCode errNo, Boolean isLoadSuccess, Boolean isFinished);

	public delegate void OnSetGameFinishFlag(DataSerializerErrorCode errNo, Boolean isSuccess);

	public delegate void OnGetSelectedLanguage(DataSerializerErrorCode errNo, Int32 selectedLanguage);

	public delegate void OnSetSelectedLanguage(DataSerializerErrorCode errNo);

	public delegate void OnGetIsAutoLogin(DataSerializerErrorCode errNo, SByte isAutoLogin);

	public delegate void OnSetIsAutoLogin(DataSerializerErrorCode errNo);

	public delegate void OnGetSystemAchievementStatuses(DataSerializerErrorCode errNo, Byte[] systemAchievementStatuses);

	public delegate void OnSetSystemAchievementStatuses(DataSerializerErrorCode errNo);

	public delegate void OnGetScreenRotation(DataSerializerErrorCode errNo, Byte screenRotation);

	public delegate void OnSetScreenRotation(DataSerializerErrorCode errNo);

	public delegate void OnReadSystemData(DataSerializerErrorCode errNo, SharedDataBytesStorage.MetaData metaData);
}
