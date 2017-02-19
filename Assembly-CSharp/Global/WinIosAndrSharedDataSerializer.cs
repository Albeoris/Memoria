using System;
using System.Collections;
using System.Collections.Generic;
using SiliconStudio;
using SimpleJSON;
using UnityEngine;
using Object = System.Object;

public class WinIosAndrSharedDataSerializer : ISharedDataSerializer
{
	public override void Save(Int32 slotID, Int32 saveID, ISharedDataSerializer.OnSaveLoadStart onStartDelegate, ISharedDataSerializer.OnSaveFinish onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.previewSlotCache = (SharedDataPreviewSlot)null;
		this.slotIDCache = slotID;
		this.saveIDCache = saveID;
		this.onSaveLoadStartDelegate = onStartDelegate;
		this.onSaveFinishDelegate = onFinishDelegate;
		base.StartCoroutine(this.SaveWithCoroutine(slotID, saveID));
	}

	private IEnumerator SaveWithCoroutine(Int32 slotID, Int32 saveID)
	{
		if (this.onSaveLoadStartDelegate != null)
		{
			this.onSaveLoadStartDelegate(ISharedDataSerializer.LastErrno, slotID, saveID);
		}
		this.Storage.SaveSlotPreview(slotID, saveID, new ISharedDataStorage.OnSaveSlotFinish(this.OnSaveSlotFinish));
		yield break;
	}

	private void OnSaveSlotFinish(Int32 slotID, Int32 saveID, SharedDataPreviewSlot data)
	{
		this.previewSlotCache = data;
		this.Parser.ParseFromFF9StateSystem();
		this.Storage.Save(slotID, this.saveIDCache, this.Parser.RootNodeInParser, new ISharedDataStorage.OnSaveFinish(this.OnDataSaveFinish));
	}

	private void OnDataSaveFinish(Int32 slotID, Int32 saveID, Boolean isSuccess)
	{
		if (isSuccess)
		{
			this.onSaveFinishDelegate(ISharedDataSerializer.LastErrno, slotID, saveID, true, this.previewSlotCache);
		}
		else
		{
			ISharedDataLog.LogError("Storage.Save() failure!");
			this.onSaveFinishDelegate(ISharedDataSerializer.LastErrno, -1, -1, false, (SharedDataPreviewSlot)null);
		}
	}

	public override void Load(Int32 slotID, Int32 saveID, ISharedDataSerializer.OnSaveLoadStart onStartDelegate, ISharedDataSerializer.OnLoadFinish onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.previewSlotCache = (SharedDataPreviewSlot)null;
		this.slotIDCache = slotID;
		this.saveIDCache = saveID;
		this.onSaveLoadStartDelegate = onStartDelegate;
		this.onLoadFinishDelegate = onFinishDelegate;
		base.StartCoroutine(this.LoadWithCoroutine(slotID, saveID));
	}

	private IEnumerator LoadWithCoroutine(Int32 slotID, Int32 saveID)
	{
		if (this.onSaveLoadStartDelegate != null)
		{
			this.onSaveLoadStartDelegate(ISharedDataSerializer.LastErrno, slotID, saveID);
		}
		this.Storage.Load(slotID, saveID, new ISharedDataStorage.OnLoadFinish(this.OnDataLoadFinish));
		yield break;
	}

	private void OnDataLoadFinish(Int32 slotID, Int32 saveID, JSONClass rootNode)
	{
		if (rootNode == null)
		{
			this.onLoadFinishDelegate(ISharedDataSerializer.LastErrno, slotID, saveID, false);
		}
		else
		{
			this.Parser.ParseToFF9StateSystem(rootNode);
			this.onLoadFinishDelegate(ISharedDataSerializer.LastErrno, slotID, saveID, true);
		}
	}

	public override void Autosave(ISharedDataSerializer.OnAutosaveAutoloadStart onStartDelegate, ISharedDataSerializer.OnAutosaveFinish onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.onAutosaveAutoloadStartDelegate = onStartDelegate;
		this.onAutosaveFinishDelegate = onFinishDelegate;
		if (this.onAutosaveAutoloadStartDelegate != null)
		{
			this.onAutosaveAutoloadStartDelegate(ISharedDataSerializer.LastErrno);
		}
		SharedSerializerEvent.WillAutosave();
		this.Parser.ParseFromFF9StateSystem();
		SharedSerializerEvent.WillAutosaveDidParse();
		this.Storage.Autosave(this.Parser.RootNodeInParser, new ISharedDataStorage.OnAutosaveFinish(this.OnDataAutosaveFinish));
	}

	public void OnDataAutosaveFinish(Boolean isSuccess)
	{
		this.onAutosaveFinishDelegate(ISharedDataSerializer.LastErrno, isSuccess);
	}

	public override void Autoload(ISharedDataSerializer.OnAutosaveAutoloadStart onStartDelegate, ISharedDataSerializer.OnAutoloadFinish onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.onAutosaveAutoloadStartDelegate = onStartDelegate;
		this.onAutoloadFinishDelegate = onFinishDelegate;
		if (this.onAutosaveAutoloadStartDelegate != null)
		{
			this.onAutosaveAutoloadStartDelegate(ISharedDataSerializer.LastErrno);
		}
		base.StartCoroutine(this.AutoloadWithCoroutine());
	}

	private IEnumerator AutoloadWithCoroutine()
	{
		this.Storage.Autoload(new ISharedDataStorage.OnAutoloadFinish(this.OnDataAutoloadFinish));
		yield break;
	}

	public void OnDataAutoloadFinish(JSONClass rootNode)
	{
		if (rootNode == null)
		{
			this.onAutoloadFinishDelegate(ISharedDataSerializer.LastErrno, false);
		}
		else
		{
			this.Parser.ParseToFF9StateSystem(rootNode);
			SharedSerializerEvent.DidAutoload();
			this.onAutoloadFinishDelegate(ISharedDataSerializer.LastErrno, true);
		}
	}

	public override void HasAutoload(ISharedDataSerializer.OnHasAutoloadFinish onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.onHasAutoloadFinishDelegate = onFinishDelegate;
		this.Storage.HasAutoload(new ISharedDataStorage.OnHasAutoloadFinish(this.OnDataHasAutoloadFinish));
	}

	private void OnDataHasAutoloadFinish(Boolean isSuccess)
	{
		this.onHasAutoloadFinishDelegate(ISharedDataSerializer.LastErrno, isSuccess);
	}

	public override void LoadSlotPreview(Int32 slotID, ISharedDataSerializer.OnLoadSlotFinish onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.onLoadSlotFinishDelegate = onFinishDelegate;
		this.Storage.LoadSlotPreview(slotID, new ISharedDataStorage.OnLoadSlotFinish(this.OnDataLoadSlotFinish));
	}

	private void OnDataLoadSlotFinish(Int32 slotID, List<SharedDataPreviewSlot> data)
	{
		if (ISharedDataSerializer.LastErrno != DataSerializerErrorCode.Success)
		{
			ISharedDataLog.LogWarning("LastErrno is NOT success in OnDataLoadSlotFinish(), return as success!");
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		}
		this.onLoadSlotFinishDelegate(ISharedDataSerializer.LastErrno, slotID, data);
	}

	private void ValidateCloudSaveData(Byte[] data, WinIosAndrSharedDataSerializer.OnValidateSaveDataFinish onFinishDelegate)
	{
		this.RawStorage.RawData = data;
		this.RawStorage.GetDataSize(delegate(Boolean isSuccess, Int32 dataSize)
		{
			this.RawStorage.RawData = null;
			if (!isSuccess)
			{
				onFinishDelegate(false);
			}
			else
			{
				onFinishDelegate(true);
			}
		});
	}

	private void ClearCloudSyncPreviewCache()
	{
		this.latestSlotID = -1;
		this.latestSaveID = -1;
		this.loadCloudSyncPreviewLocalPreview = (SharedDataPreviewSlot)null;
		this.loadCloudSyncPreviewRemotePreview = (SharedDataPreviewSlot)null;
		this.onSyncCloudSlotFinishDelegate = (ISharedDataSerializer.OnSyncCloudSlotFinish)null;
	}

	public override void LoadCloudSyncPreview(ISharedDataSerializer.OnSyncCloudSlotStart onStartDelegate, ISharedDataSerializer.OnSyncCloudSlotFinish onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.ClearCloudSyncPreviewCache();
		this.onSyncCloudSlotFinishDelegate = onFinishDelegate;
		this.Storage.GetDataSize(delegate(Boolean isSuccess, Int32 dataSize)
		{
			if (isSuccess)
			{
				this.Storage.GetLatestSlotAndSave(delegate(Int32 latestSlot, Int32 latestSave)
				{
					this.latestSlotID = latestSlot;
					this.latestSaveID = latestSave;
					if (onStartDelegate != null)
					{
						onStartDelegate(ISharedDataSerializer.LastErrno);
					}
					if (this.latestSlotID != -1)
					{
						this.LoadSlotPreview(this.latestSlotID, delegate(DataSerializerErrorCode errNo, Int32 slotID, List<SharedDataPreviewSlot> data)
						{
							if (data != null)
							{
								this.loadCloudSyncPreviewLocalPreview = data[this.latestSaveID];
								this.StartCoroutine(SiliconStudio.Social.Cloud_IsFileExist(this, new Action<Boolean, SiliconStudio.Social.ResponseData.Status>(this.LoadCloudSyncPreviewOnFileExistCallBack)));
							}
							else
							{
								ISharedDataLog.LogError("latestSlotID is: " + this.latestSlotID + " but preview data NOT found!");
								ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
								this.onSyncCloudSlotFinishDelegate(ISharedDataSerializer.LastErrno, false, (SharedDataPreviewSlot)null, (SharedDataPreviewSlot)null);
								this.ClearCloudSyncPreviewCache();
							}
						});
					}
					else
					{
						this.StartCoroutine(SiliconStudio.Social.Cloud_IsFileExist(this, new Action<Boolean, SiliconStudio.Social.ResponseData.Status>(this.LoadCloudSyncPreviewOnFileExistCallBack)));
					}
				});
			}
			else
			{
				ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
				ISharedDataLog.LogError("Save data is invalid!");
				this.StartCoroutine(SiliconStudio.Social.Cloud_IsFileExist(this, new Action<Boolean, SiliconStudio.Social.ResponseData.Status>(this.LoadCloudSyncPreviewOnFileExistCallBack)));
			}
		});
	}

	private void LoadCloudSyncPreviewOnFileExistCallBack(Boolean isExist, SiliconStudio.Social.ResponseData.Status status)
	{
		if (isExist)
		{
			base.StartCoroutine(SiliconStudio.Social.Cloud_Load(this, new Action<Byte[], SiliconStudio.Social.ResponseData.Status>(this.LoadCloudSyncPreviewOnCloudLoadCallBack)));
		}
		else
		{
			ISharedDataLog.LogWarning("Cloud data does not exist!. Return success status.");
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.CloudFileNotFound;
			this.onSyncCloudSlotFinishDelegate(ISharedDataSerializer.LastErrno, true, this.loadCloudSyncPreviewLocalPreview, (SharedDataPreviewSlot)null);
			this.ClearCloudSyncPreviewCache();
		}
	}

	private void LoadCloudSyncPreviewOnCloudLoadCallBack(Byte[] data, SiliconStudio.Social.ResponseData.Status status)
	{
		if (data != null)
		{
			this.ValidateCloudSaveData(data, delegate(Boolean isSuccess)
			{
				if (isSuccess)
				{
					this.RawStorage.RawData = data;
					this.RawStorage.GetLatestSlotAndSave(delegate(Int32 latestCloudSlotID, Int32 latestCloudSaveID)
					{
						if (latestCloudSlotID != -1 && latestCloudSaveID != -1)
						{
							this.RawStorage.LoadSlotPreview(latestCloudSlotID, delegate(Int32 slotID, List<SharedDataPreviewSlot> previewData)
							{
								this.loadCloudSyncPreviewRemotePreview = previewData[latestCloudSaveID];
								this.onSyncCloudSlotFinishDelegate(ISharedDataSerializer.LastErrno, true, this.loadCloudSyncPreviewLocalPreview, this.loadCloudSyncPreviewRemotePreview);
								this.ClearCloudSyncPreviewCache();
								this.RawStorage.RawData = null;
							});
						}
						else
						{
							this.onSyncCloudSlotFinishDelegate(ISharedDataSerializer.LastErrno, true, this.loadCloudSyncPreviewLocalPreview, this.loadCloudSyncPreviewRemotePreview);
							this.ClearCloudSyncPreviewCache();
						}
					});
				}
				else
				{
					ISharedDataSerializer.LastErrno = DataSerializerErrorCode.CloudDataCorruption;
					ISharedDataLog.LogError("Cloud data is invalid!");
					this.onSyncCloudSlotFinishDelegate(ISharedDataSerializer.LastErrno, true, this.loadCloudSyncPreviewLocalPreview, this.loadCloudSyncPreviewRemotePreview);
				}
			});
		}
		else
		{
			ISharedDataLog.LogWarning("Cloud data is null!");
			ISharedDataSerializer.LastErrno = ISharedDataSerializer.ConvertCloudStatusToDataSerializerErrorCode(status);
			this.onSyncCloudSlotFinishDelegate(ISharedDataSerializer.LastErrno, true, this.loadCloudSyncPreviewLocalPreview, this.loadCloudSyncPreviewRemotePreview);
			this.ClearCloudSyncPreviewCache();
		}
	}

	private void ClearUploadToCloudCache()
	{
		this.latestSlotID = -1;
		this.latestSaveID = -1;
		this.uploadToCloudFinishDelegate = (ISharedDataSerializer.OnSyncCloudSlotFinish)null;
		this.uploadToCloudLocalDataPreviewSlot = (SharedDataPreviewSlot)null;
		this.uploadToCloudRemoteDataPreviewSlot = (SharedDataPreviewSlot)null;
		this.uploadToCloudRawData = null;
	}

	public override void UploadToCloud(ISharedDataSerializer.OnSyncCloudSlotStart onStartDelegate, ISharedDataSerializer.OnSyncCloudSlotFinish onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.ClearUploadToCloudCache();
		if (onStartDelegate != null)
		{
			onStartDelegate(ISharedDataSerializer.LastErrno);
		}
		this.uploadToCloudFinishDelegate = onFinishDelegate;
		this.Storage.GetLatestSlotAndSave(delegate(Int32 latestSlot, Int32 latestSave)
		{
			this.latestSlotID = latestSlot;
			this.latestSaveID = latestSave;
			this.Storage.LoadRawData(delegate(Byte[] rawData)
			{
				if (rawData != null)
				{
					this.uploadToCloudRawData = rawData;
					this.LoadSlotPreview(this.latestSlotID, delegate(DataSerializerErrorCode errNo, Int32 slotID, List<SharedDataPreviewSlot> data)
					{
						if (data != null)
						{
							this.uploadToCloudLocalDataPreviewSlot = data[this.latestSaveID];
							UInt64 playDuration = this.uploadToCloudLocalDataPreviewSlot.PlayDuration;
							Int32 num = (Int32)playDuration % 60;
							Int32 num2 = num / 60;
							Int32 num3 = num2 / 60;
							num2 %= 60;
							Int32 days = num3 / 24;
							num3 %= 24;
							TimeSpan playTime = new TimeSpan(days, num3, num2, num);
							base.StartCoroutine(SiliconStudio.Social.Cloud_Save(this, this.uploadToCloudRawData, playTime, delegate(Boolean isSuccess, SiliconStudio.Social.ResponseData.Status status)
							{
								if (isSuccess)
								{
									this.uploadToCloudRemoteDataPreviewSlot = this.uploadToCloudLocalDataPreviewSlot;
								}
								else
								{
									ISharedDataSerializer.LastErrno = ISharedDataSerializer.ConvertCloudStatusToDataSerializerErrorCode(status);
								}
								this.uploadToCloudFinishDelegate(ISharedDataSerializer.LastErrno, isSuccess, this.uploadToCloudLocalDataPreviewSlot, this.uploadToCloudRemoteDataPreviewSlot);
								this.ClearUploadToCloudCache();
							}));
						}
						else
						{
							this.uploadToCloudFinishDelegate(ISharedDataSerializer.LastErrno, false, (SharedDataPreviewSlot)null, (SharedDataPreviewSlot)null);
							this.ClearUploadToCloudCache();
						}
					});
				}
				else
				{
					this.uploadToCloudFinishDelegate(ISharedDataSerializer.LastErrno, false, (SharedDataPreviewSlot)null, (SharedDataPreviewSlot)null);
					this.ClearUploadToCloudCache();
				}
			});
		});
	}

	private void ClearDownloadFromCloudCache()
	{
		this.latestSlotID = -1;
		this.latestSaveID = -1;
	}

	public override void DownloadFromCloud(ISharedDataSerializer.OnSyncCloudSlotStart onStartDelegate, ISharedDataSerializer.OnSyncCloudSlotFinish onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		if (onStartDelegate != null)
		{
			onStartDelegate(ISharedDataSerializer.LastErrno);
		}
		this.ClearDownloadFromCloudCache();
		if (onStartDelegate != null)
		{
			onStartDelegate(ISharedDataSerializer.LastErrno);
		}
		this.downloadFromCloudFinishDelegate = onFinishDelegate;
		this.Storage.GetLatestSlotAndSave(delegate(Int32 latestSlot, Int32 latestSave)
		{
			this.latestSlotID = latestSlot;
			this.latestSaveID = latestSave;
			if (this.latestSlotID != -1 && this.latestSaveID != -1)
			{
				this.LoadSlotPreview(this.latestSlotID, delegate(DataSerializerErrorCode errNo, Int32 slotID, List<SharedDataPreviewSlot> data)
				{
					if (data != null)
					{
						this.downloadFromCloudLocalDataPreviewSlot = data[this.latestSaveID];
						base.StartCoroutine(SiliconStudio.Social.Cloud_Load(this, new Action<Byte[], SiliconStudio.Social.ResponseData.Status>(this.DownloadFromCloudOnCloudLoadCallBack)));
					}
					else
					{
						this.downloadFromCloudFinishDelegate(ISharedDataSerializer.LastErrno, false, (SharedDataPreviewSlot)null, (SharedDataPreviewSlot)null);
						this.ClearCloudSyncPreviewCache();
					}
				});
			}
			else
			{
				base.StartCoroutine(SiliconStudio.Social.Cloud_Load(this, new Action<Byte[], SiliconStudio.Social.ResponseData.Status>(this.DownloadFromCloudOnCloudLoadCallBack)));
			}
		});
	}

	private void DownloadFromCloudOnCloudLoadCallBack(Byte[] data, SiliconStudio.Social.ResponseData.Status status)
	{
		if (data != null)
		{
			this.ValidateCloudSaveData(data, delegate(Boolean isValidate)
			{
				if (isValidate)
				{
					this.RawStorage.RawData = data;
					this.Storage.SaveRawData(data, delegate(Boolean isSuccess)
					{
						this.RawStorage.GetLatestSlotAndSave(delegate(Int32 latestSlotID, Int32 latestSaveID)
						{
							this.latestCloudSlotID = latestSlotID;
							this.latestCloudSaveID = latestSaveID;
							if (this.latestCloudSlotID != -1 && this.latestCloudSaveID != -1)
							{
								this.RawStorage.LoadSlotPreview(this.latestCloudSlotID, delegate(Int32 slotID, List<SharedDataPreviewSlot> previewData)
								{
									this.downloadFromCloudRemoteDataPreviewSlot = previewData[this.latestCloudSaveID];
									this.downloadFromCloudLocalDataPreviewSlot = this.downloadFromCloudRemoteDataPreviewSlot;
									this.downloadFromCloudFinishDelegate(ISharedDataSerializer.LastErrno, true, this.downloadFromCloudLocalDataPreviewSlot, this.downloadFromCloudRemoteDataPreviewSlot);
									this.ClearCloudSyncPreviewCache();
									this.RawStorage.RawData = null;
								});
							}
							else
							{
								this.downloadFromCloudFinishDelegate(ISharedDataSerializer.LastErrno, true, this.downloadFromCloudLocalDataPreviewSlot, this.downloadFromCloudRemoteDataPreviewSlot);
								this.ClearCloudSyncPreviewCache();
							}
						});
					});
				}
				else
				{
					ISharedDataSerializer.LastErrno = DataSerializerErrorCode.CloudDataCorruption;
					ISharedDataLog.LogError("Cloud data is invalid!");
					this.downloadFromCloudFinishDelegate(ISharedDataSerializer.LastErrno, true, this.downloadFromCloudLocalDataPreviewSlot, this.downloadFromCloudRemoteDataPreviewSlot);
					this.ClearCloudSyncPreviewCache();
				}
			});
		}
		else
		{
			ISharedDataSerializer.LastErrno = ISharedDataSerializer.ConvertCloudStatusToDataSerializerErrorCode(status);
			this.downloadFromCloudFinishDelegate(ISharedDataSerializer.LastErrno, false, (SharedDataPreviewSlot)null, (SharedDataPreviewSlot)null);
			this.ClearCloudSyncPreviewCache();
		}
	}

	public override void GetLocalSaveLatestTimestamp(ISharedDataSerializer.OnGetLatestSaveTimestamp onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.Storage.GetLatestTimestamp(delegate(Boolean isSuccess, Double timestamp)
		{
			onFinishDelegate(ISharedDataSerializer.LastErrno, isSuccess, timestamp);
		});
	}

	public override void GetCloudSaveLatestTimestamp(ISharedDataSerializer.OnGetLatestSaveTimestamp onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.getCloudSaveLatestTimestampFinishDelegate = onFinishDelegate;
		base.StartCoroutine(SiliconStudio.Social.Cloud_Load(this, new Action<Byte[], SiliconStudio.Social.ResponseData.Status>(this.GetCloudSaveLatestTimestampCallback)));
	}

	private void GetCloudSaveLatestTimestampCallback(Byte[] data, SiliconStudio.Social.ResponseData.Status status)
	{
		if (data != null)
		{
			this.ValidateCloudSaveData(data, delegate(Boolean isValidate)
			{
				if (isValidate)
				{
					this.RawStorage.RawData = data;
					this.RawStorage.GetLatestTimestamp(delegate(Boolean isSuccess, Double timestamp)
					{
						this.getCloudSaveLatestTimestampFinishDelegate(ISharedDataSerializer.LastErrno, isSuccess, timestamp);
						this.RawStorage.RawData = null;
					});
				}
				else
				{
					ISharedDataSerializer.LastErrno = DataSerializerErrorCode.CloudDataCorruption;
					ISharedDataLog.LogError("Cloud data is invalid!");
					this.getCloudSaveLatestTimestampFinishDelegate(ISharedDataSerializer.LastErrno, false, 0.0);
				}
			});
		}
		else
		{
			ISharedDataSerializer.LastErrno = ISharedDataSerializer.ConvertCloudStatusToDataSerializerErrorCode(status);
			this.getCloudSaveLatestTimestampFinishDelegate(ISharedDataSerializer.LastErrno, false, 0.0);
		}
	}

	public override void GetGameFinishFlag(ISharedDataSerializer.OnGetGameFinishFlag onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.Storage.GetGameFinishFlag(delegate(Boolean isLoadSuccess, Boolean isFinished)
		{
			onFinishDelegate(ISharedDataSerializer.LastErrno, isLoadSuccess, isFinished);
		});
	}

	public override void SetGameFinishFlagWithTrue(ISharedDataSerializer.OnSetGameFinishFlag onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.Storage.SetGameFinishFlagWithTrue(delegate(Boolean isSuccess)
		{
			onFinishDelegate(ISharedDataSerializer.LastErrno, isSuccess);
		});
	}

	public override void GetSelectedLanguage(ISharedDataSerializer.OnGetSelectedLanguage onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.Storage.GetSelectedLanguage(delegate(Int32 selectedLanguage)
		{
			onFinishDelegate(ISharedDataSerializer.LastErrno, selectedLanguage);
		});
	}

	public override void SetSelectedLanguage(Int32 selectedLanguage, ISharedDataSerializer.OnSetSelectedLanguage onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.Storage.SetSelectedLanguage(selectedLanguage, delegate
		{
			onFinishDelegate(ISharedDataSerializer.LastErrno);
		});
	}

	public override void GetIsAutoLogin(ISharedDataSerializer.OnGetIsAutoLogin onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.Storage.GetIsAutoLogin(delegate(SByte isAutoLogin)
		{
			onFinishDelegate(ISharedDataSerializer.LastErrno, isAutoLogin);
		});
	}

	public override void SetIsAutoLogin(SByte isAutoLogin, ISharedDataSerializer.OnSetIsAutoLogin onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.Storage.SetIsAutoLogin(isAutoLogin, delegate
		{
			onFinishDelegate(ISharedDataSerializer.LastErrno);
		});
	}

	public override void GetSystemAchievementStatuses(ISharedDataSerializer.OnGetSystemAchievementStatuses onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.Storage.GetSystemAchievementStatuses(delegate(Byte[] systemAchievementStatues)
		{
			onFinishDelegate(ISharedDataSerializer.LastErrno, systemAchievementStatues);
		});
	}

	public override void SetSystemAchievementStatuses(Byte[] systemAchievementStatuses, ISharedDataSerializer.OnSetSystemAchievementStatuses onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.Storage.SetSystemAchievementStatuses(systemAchievementStatuses, delegate
		{
			onFinishDelegate(ISharedDataSerializer.LastErrno);
		});
	}

	public override void GetScreenRotation(ISharedDataSerializer.OnGetScreenRotation onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.Storage.GetScreenRotation(delegate(Byte screenRotation)
		{
			onFinishDelegate(ISharedDataSerializer.LastErrno, screenRotation);
		});
	}

	public override void SetScreenRotation(Byte screenRotation, ISharedDataSerializer.OnSetScreenRotation onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		global::Debug.Log("Serializer.SetScreenRotation : 1 rotation = " + screenRotation);
		this.Storage.SetScreenRotation(screenRotation, delegate
		{
			global::Debug.Log("Serializer.SetScreenRotation : 2 rotation = " + screenRotation);
			onFinishDelegate(ISharedDataSerializer.LastErrno);
		});
	}

	public override void ReadSystemData(ISharedDataSerializer.OnReadSystemData onFinishDelegate)
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		global::Debug.Log(String.Concat(new Object[]
		{
			"Serializer.ReadSystemData : 1 settings.screenRotation = ",
			FF9StateSystem.Settings.ScreenRotation,
			", screen.orientation = ",
			Screen.orientation
		}));
		this.Storage.ReadSystemData(delegate(SharedDataBytesStorage.MetaData metaData)
		{
			global::Debug.Log(String.Concat(new Object[]
			{
				"Serializer.ReadSystemData : 2 serializer.callback par[0] = ",
				metaData.SelectedLanguage,
				", par[1] = ",
				metaData.SystemAchievementStatuses[0],
				", par[2] = ",
				metaData.ScreenRotation,
				", settings.screenRotation = ",
				FF9StateSystem.Settings.ScreenRotation
			}));
			onFinishDelegate(ISharedDataSerializer.LastErrno, metaData);
		});
	}

	public override void ClearAllData()
	{
		ISharedDataSerializer.LastErrno = DataSerializerErrorCode.Success;
		this.Storage.ClearAllData();
	}

	private ISharedDataSerializer.OnSaveLoadStart onSaveLoadStartDelegate;

	private ISharedDataSerializer.OnSaveFinish onSaveFinishDelegate;

	private ISharedDataSerializer.OnLoadFinish onLoadFinishDelegate;

	private ISharedDataSerializer.OnAutosaveAutoloadStart onAutosaveAutoloadStartDelegate;

	private ISharedDataSerializer.OnAutosaveFinish onAutosaveFinishDelegate;

	private ISharedDataSerializer.OnAutoloadFinish onAutoloadFinishDelegate;

	private ISharedDataSerializer.OnHasAutoloadFinish onHasAutoloadFinishDelegate;

	private ISharedDataSerializer.OnLoadSlotFinish onLoadSlotFinishDelegate;

	private SharedDataPreviewSlot previewSlotCache;

	private Int32 slotIDCache;

	private Int32 saveIDCache;

	public SharedDataRawStorage RawStorage;

	private Int32 latestSlotID;

	private Int32 latestSaveID;

	private Int32 latestCloudSlotID;

	private Int32 latestCloudSaveID;

	private SharedDataPreviewSlot loadCloudSyncPreviewLocalPreview;

	private SharedDataPreviewSlot loadCloudSyncPreviewRemotePreview;

	private ISharedDataSerializer.OnSyncCloudSlotFinish onSyncCloudSlotFinishDelegate;

	private ISharedDataSerializer.OnSyncCloudSlotFinish uploadToCloudFinishDelegate;

	private SharedDataPreviewSlot uploadToCloudLocalDataPreviewSlot;

	private SharedDataPreviewSlot uploadToCloudRemoteDataPreviewSlot;

	private Byte[] uploadToCloudRawData;

	private ISharedDataSerializer.OnSyncCloudSlotFinish downloadFromCloudFinishDelegate;

	private SharedDataPreviewSlot downloadFromCloudLocalDataPreviewSlot;

	private SharedDataPreviewSlot downloadFromCloudRemoteDataPreviewSlot;

	private ISharedDataSerializer.OnGetLatestSaveTimestamp getCloudSaveLatestTimestampFinishDelegate;

	private delegate void OnValidateSaveDataFinish(Boolean isSuccess);
}
