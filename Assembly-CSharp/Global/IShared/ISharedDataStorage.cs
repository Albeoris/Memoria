using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISharedDataStorage : MonoBehaviour
{
    public abstract void SaveSlotPreview(Int32 slotID, Int32 saveID, ISharedDataStorage.OnSaveSlotFinish onFinishDelegate);

    public abstract void LoadSlotPreview(Int32 slotID, ISharedDataStorage.OnLoadSlotFinish onFinishDelegate);

    public abstract void Load(Int32 slotID, Int32 saveID, ISharedDataStorage.OnLoadFinish onFinishDelegate);

    public abstract void Save(Int32 slotID, Int32 saveID, JSONClass rootNode, ISharedDataStorage.OnSaveFinish onFinishDelegate);

    public abstract void Autosave(JSONClass rootNode, ISharedDataStorage.OnAutosaveFinish onFinishDelegate);

    public abstract void Autoload(ISharedDataStorage.OnAutoloadFinish onFinishDelegate);

    public abstract void HasAutoload(ISharedDataStorage.OnHasAutoloadFinish onFinishDelegate);

    public abstract void LoadRawData(ISharedDataStorage.OnLoadRawDataFinish onFinishDelegate);

    public abstract void SaveRawData(Byte[] rawData, ISharedDataStorage.OnSaveRawDataFinish onFinishDelegate);

    public abstract void GetLatestSlotAndSave(ISharedDataStorage.OnGetLatestSlotAndSaveFinish onFinishDelegate);

    public abstract void GetLatestTimestamp(ISharedDataStorage.OnGetLatestSaveTimestamp onFinishDelegate);

    public abstract void GetGameFinishFlag(ISharedDataStorage.OnGetGameFinishFlag onFinishDelegate);

    public abstract void SetGameFinishFlagWithTrue(ISharedDataStorage.OnSetGameFinishFlag onFinishDelegate);

    public abstract void GetSelectedLanguage(ISharedDataStorage.OnGetSelectedLanguage onFinishDelegate);

    public abstract void SetSelectedLanguage(Int32 selectedLanguage, ISharedDataStorage.OnSetSelectedLanguage onFinishDelegate);

    public abstract void GetIsAutoLogin(ISharedDataStorage.OnGetIsAutoLogin onFinishDelegate);

    public abstract void SetIsAutoLogin(SByte isAutoLogin, ISharedDataStorage.OnSetIsAutoLogin onFinishDelegate);

    public abstract void GetSystemAchievementStatuses(ISharedDataStorage.OnGetSystemAchievementStatuses onFinishDelegate);

    public abstract void SetSystemAchievementStatuses(Byte[] systemAchievementStatuses, ISharedDataStorage.OnSetSystemAchievementStatuses onFinishDelegate);

    public abstract void GetScreenRotation(ISharedDataStorage.OnGetScreenRotation onFinishDelegate);

    public abstract void SetScreenRotation(Byte screenRotation, ISharedDataStorage.OnSetScreenRotation onFinishDelegate);

    public abstract void ReadSystemData(ISharedDataStorage.OnReadSystemData onFinishDelegate);

    public abstract void ClearAllData();

    public abstract void GetDataSize(ISharedDataStorage.OnGetDataSizeFinish onFinishDelegate);

    public ISharedDataEncryption Encryption;

    public delegate void OnLoadFinish(Int32 slotID, Int32 saveID, JSONClass rootNode);

    public delegate void OnSaveFinish(Int32 slotID, Int32 saveID, Boolean isSuccess);

    public delegate void OnLoadSlotFinish(Int32 slotID, List<SharedDataPreviewSlot> data);

    public delegate void OnSaveSlotFinish(Int32 slotID, Int32 saveID, SharedDataPreviewSlot data);

    public delegate void OnAutosaveAutoloadStart();

    public delegate void OnAutosaveFinish(Boolean isSuccess);

    public delegate void OnAutoloadFinish(JSONClass rootNode);

    public delegate void OnHasAutoloadFinish(Boolean hasAutoload);

    public delegate void OnLoadRawDataFinish(Byte[] rawData);

    public delegate void OnSaveRawDataFinish(Boolean isSuccess);

    public delegate void OnGetLatestSlotAndSaveFinish(Int32 latestSlot, Int32 latestSave);

    public delegate void OnGetLatestSaveTimestamp(Boolean isSuccess, Double timestamp);

    public delegate void OnGetGameFinishFlag(Boolean isLoadSuccess, Boolean isFinished);

    public delegate void OnSetGameFinishFlag(Boolean isSuccess);

    public delegate void OnGetSelectedLanguage(Int32 selectedLanguage);

    public delegate void OnSetSelectedLanguage();

    public delegate void OnGetIsAutoLogin(SByte isAutoLogin);

    public delegate void OnSetIsAutoLogin();

    public delegate void OnGetSystemAchievementStatuses(Byte[] systemAchievementStatuses);

    public delegate void OnSetSystemAchievementStatuses();

    public delegate void OnGetScreenRotation(Byte screenRotation);

    public delegate void OnSetScreenRotation();

    public delegate void OnReadSystemData(SharedDataBytesStorage.MetaData metaData);

    public delegate void OnGetDataSizeFinish(Boolean isSuccess, Int32 dataSize);
}
