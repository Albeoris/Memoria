using System;
using System.Collections;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using Memoria.Assets;
using SiliconStudio;
using UnityEngine;

public class CloudUI : UIScene
{
	public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
	{
		SiliconStudio.Social.InitializeSocialPlatform();
		SiliconStudio.Social.Authenticate(false);
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
		{
			base.Loading = true;
			this.timeCounter = Time.time;
			this.currentState = CloudUI.State.LoadPreview;
			FF9StateSystem.Serializer.LoadCloudSyncPreview((ISharedDataSerializer.OnSyncCloudSlotStart)null, new ISharedDataSerializer.OnSyncCloudSlotFinish(this.OnFinishedLoadPreview));
			this.LoadingPreviewDialog.SetActive(true);
			this.helpTitleLabel.text = Localization.Get("ConnectingHelpInfo");
			ButtonGroupState.SetPointerOffsetToGroup(new Vector2(26f, 134f), CloudUI.LocalFileGroupButton);
			ButtonGroupState.SetPointerOffsetToGroup(new Vector2(26f, 134f), CloudUI.CloudFileGroupButton);
			ButtonGroupState.SetPointerOffsetToGroup(new Vector2(220f, 10f), CloudUI.ConfirmDialogGroupButton);
		};
		if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
		base.Show(sceneVoidDelegate);
		this.FileListPanel.SetActive(false);
		this.DisplayInfo();
	}

	public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
	{
		base.Hide(afterFinished);
		if (this.isLocalFileExist)
		{
			PersistenSingleton<UIManager>.Instance.TitleScene.ForceCheckingAutoSave = true;
		}
	}

	public override Boolean OnKeyConfirm(GameObject go)
	{
		if (base.OnKeyConfirm(go))
		{
			if (ButtonGroupState.ActiveGroup == CloudUI.SubMenuGroupButton)
			{
				FF9Sfx.FF9SFX_Play(103);
				if (go == this.UploadButton)
				{
					this.syncState = CloudUI.Sync.Upload;
					ButtonGroupState.ActiveGroup = CloudUI.CloudFileGroupButton;
					ButtonGroupState.SetSecondaryOnGroup(CloudUI.SubMenuGroupButton);
					ButtonGroupState.HoldActiveStateOnGroup(CloudUI.SubMenuGroupButton);
				}
				else if (go == this.DownloadButton)
				{
					this.syncState = CloudUI.Sync.Download;
					ButtonGroupState.ActiveGroup = CloudUI.LocalFileGroupButton;
					ButtonGroupState.SetSecondaryOnGroup(CloudUI.SubMenuGroupButton);
					ButtonGroupState.HoldActiveStateOnGroup(CloudUI.SubMenuGroupButton);
				}
			}
			else if (ButtonGroupState.ActiveGroup == CloudUI.CloudFileGroupButton)
			{
				if (ButtonGroupState.ContainButtonInGroup(go, CloudUI.CloudFileGroupButton))
				{
					FF9Sfx.FF9SFX_Play(103);
					if (this.isCloudFileExist)
					{
						this.overWriteDialogDespLabel.text = Localization.Get("UploadOverwrite");
						this.DisplayOverWriteDialog();
						ButtonGroupState.ActiveGroup = CloudUI.ConfirmDialogGroupButton;
						ButtonGroupState.HoldActiveStateOnGroup(CloudUI.CloudFileGroupButton);
					}
					else
					{
						this.SyncFileToCloud();
					}
				}
				else
				{
					this.OnSecondaryGroupClick(go);
				}
			}
			else if (ButtonGroupState.ActiveGroup == CloudUI.LocalFileGroupButton)
			{
				if (ButtonGroupState.ContainButtonInGroup(go, CloudUI.LocalFileGroupButton))
				{
					FF9Sfx.FF9SFX_Play(103);
					if (this.isLocalFileExist)
					{
						this.overWriteDialogDespLabel.text = Localization.Get("DownloadOverwrite");
						this.DisplayOverWriteDialog();
						ButtonGroupState.ActiveGroup = CloudUI.ConfirmDialogGroupButton;
						ButtonGroupState.HoldActiveStateOnGroup(CloudUI.LocalFileGroupButton);
					}
					else
					{
						this.SyncFileToCloud();
					}
				}
				else
				{
					this.OnSecondaryGroupClick(go);
				}
			}
			else if (ButtonGroupState.ActiveGroup == CloudUI.ConfirmDialogGroupButton && ButtonGroupState.ContainButtonInGroup(go, CloudUI.ConfirmDialogGroupButton))
			{
				this.OverWriteDialog.SetActive(false);
				if (go.transform.GetSiblingIndex() == 1)
				{
					FF9Sfx.FF9SFX_Play(103);
					this.SyncFileToCloud();
				}
				else
				{
					FF9Sfx.FF9SFX_Play(101);
					if (this.syncState == CloudUI.Sync.Upload)
					{
						ButtonGroupState.ActiveGroup = CloudUI.CloudFileGroupButton;
					}
					else
					{
						ButtonGroupState.ActiveGroup = CloudUI.LocalFileGroupButton;
					}
				}
			}
		}
		return true;
	}

	public override Boolean OnKeyCancel(GameObject go)
	{
		if (base.OnKeyCancel(go))
		{
			if (ButtonGroupState.ActiveGroup == CloudUI.SubMenuGroupButton)
			{
				FF9Sfx.FF9SFX_Play(101);
				this.Hide(delegate
				{
					PersistenSingleton<UIManager>.Instance.ChangeUIState(PersistenSingleton<UIManager>.Instance.HUDState);
				});
			}
			else if (ButtonGroupState.ActiveGroup == CloudUI.CloudFileGroupButton || ButtonGroupState.ActiveGroup == CloudUI.LocalFileGroupButton)
			{
				FF9Sfx.FF9SFX_Play(101);
				ButtonGroupState.ActiveGroup = CloudUI.SubMenuGroupButton;
				this.CheckData();
			}
			else if (ButtonGroupState.ActiveGroup == CloudUI.ConfirmDialogGroupButton)
			{
				FF9Sfx.FF9SFX_Play(101);
				this.OverWriteDialog.SetActive(false);
				if (this.syncState == CloudUI.Sync.Upload)
				{
					ButtonGroupState.ActiveGroup = CloudUI.CloudFileGroupButton;
				}
				else
				{
					ButtonGroupState.ActiveGroup = CloudUI.LocalFileGroupButton;
				}
			}
		}
		return true;
	}

	public override Boolean OnKeySelect(GameObject go)
	{
		return false;
	}

	public override Boolean OnItemSelect(GameObject go)
	{
		if (base.OnItemSelect(go) && ButtonGroupState.ActiveGroup == CloudUI.SubMenuGroupButton)
		{
			if (go == this.InfoButton)
			{
				this.FileListPanel.SetActive(false);
				this.DisplayInfo();
			}
			else
			{
				if (go == this.UploadButton)
				{
					this.syncState = CloudUI.Sync.Upload;
				}
				else if (go == this.DownloadButton)
				{
					this.syncState = CloudUI.Sync.Download;
				}
				this.InfoPanel.SetActive(false);
				this.DisplayFile();
			}
		}
		return true;
	}

	private void OnSecondaryGroupClick(GameObject go)
	{
		ButtonGroupState.HoldActiveStateOnGroup(go, CloudUI.SubMenuGroupButton);
		if (ButtonGroupState.ActiveGroup == CloudUI.CloudFileGroupButton)
		{
			FF9Sfx.muteSfx = true;
			this.OnKeyCancel(this.FileListPanel.GetChild(0));
			FF9Sfx.muteSfx = false;
			this.OnKeyConfirm(go);
		}
		else if (ButtonGroupState.ActiveGroup == CloudUI.LocalFileGroupButton)
		{
			FF9Sfx.muteSfx = true;
			this.OnKeyCancel(this.FileListPanel.GetChild(2));
			FF9Sfx.muteSfx = false;
			this.OnKeyConfirm(go);
		}
	}

	private void DisplayInfo()
	{
		this.InfoPanel.SetActive(true);
		this.slotGameObject.SetActive(false);
		this.helpTitleLabel.text = Localization.Get("SelectSyncHelpInfo");
		String text = String.Empty;
		if (FF9StateSystem.MobilePlatform)
		{
			text += Localization.Get("MobileCloudInfoPart1");
			text += Localization.Get("MobileNoticeConnectDesc");
			text += Localization.Get("MobileCloudInfoPart2");
		}
		else if (FF9StateSystem.PCPlatform)
		{
			text += Localization.Get("PCCloudInfoPart1");
			text += Localization.Get("PCNoticeConnectDesc");
			text += Localization.Get("PCCloudInfoPart2");
		}
		this.infoLabel.text = text;
	}

	private void DisplayFile()
	{
		this.FileListPanel.SetActive(true);
		this.slotGameObject.SetActive(true);
		this.helpTitleLabel.text = Localization.Get("SelectDataHelpInfo");
		if (this.syncState == CloudUI.Sync.Upload)
		{
			this.syncStatusLabel.text = Localization.Get("UploadCaption");
			this.syncStatusLabel.color = FF9TextTool.Cyan;
			this.helpSlotLabel.text = Localization.Get("Upload");
			UISprite[] array = this.arrowSprites;
			for (Int32 i = 0; i < (Int32)array.Length; i++)
			{
				UISprite uisprite = array[i];
				uisprite.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			}
		}
		else
		{
			this.syncStatusLabel.text = Localization.Get("DownloadCaption");
			this.syncStatusLabel.color = FF9TextTool.Green;
			this.helpSlotLabel.text = Localization.Get("Download");
			UISprite[] array2 = this.arrowSprites;
			for (Int32 j = 0; j < (Int32)array2.Length; j++)
			{
				UISprite uisprite2 = array2[j];
				uisprite2.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
			}
		}
	}

	private void DisplayFileInfo(CloudUI.FileCloudInfoHUD fileHud, SharedDataPreviewSlot file, Boolean isCorrupt, DataSerializerErrorCode status)
	{
		fileHud.Self.SetActive(true);
		if (file != null)
		{
			fileHud.Container.SetActive(true);
			fileHud.EmptySlotTextGameObject.SetActive(false);
			Int32 num = 1;
			String text = String.Empty;
			String empty = String.Empty;
			Boolean flag = false;
			Int32 num2 = 0;
			foreach (SharedDataPreviewCharacterInfo sharedDataPreviewCharacterInfo in file.CharacterInfoList)
			{
				fileHud.CharacterAvatarList[num2].alpha = 0f;
				fileHud.CharacterAvatarList[num2].spriteName = String.Empty;
				if (sharedDataPreviewCharacterInfo != null)
				{
					if (!flag)
					{
						num = sharedDataPreviewCharacterInfo.Level;
						text = sharedDataPreviewCharacterInfo.Name;
						flag = true;
					}
					fileHud.CharacterAvatarList[num2].alpha = 1f;
					FF9UIDataTool.DisplayCharacterAvatar((CharacterSerialNumber)sharedDataPreviewCharacterInfo.SerialID, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), fileHud.CharacterAvatarList[num2], false);
				}
				num2++;
			}
			fileHud.LeaderNameLabel.text = text;
			fileHud.LeaderLvLabel.text = num.ToString();
			fileHud.GilLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", file.Gil.ToString());
			fileHud.LocationNameLabel.text = file.Location;
			Color color = FF9TextTool.White;
			Double num3 = (Double)(file.PlayDuration % 360000f);
			switch ((Int32)(file.PlayDuration / 360000f))
			{
			case 0:
				color = FF9TextTool.White;
				break;
			case 1:
				color = FF9TextTool.Red;
				break;
			case 2:
				color = FF9TextTool.Yellow;
				break;
			case 3:
				color = FF9TextTool.Cyan;
				break;
			case 4:
				color = FF9TextTool.Magenta;
				break;
			case 5:
				color = FF9TextTool.Green;
				break;
			default:
				num3 = 359999.0;
				color = FF9TextTool.Green;
				break;
			}
			String text2 = ((Int32)(num3 / 3600.0)).ToString("0#");
			String text3 = ((Int32)(num3 / 60.0) % 60).ToString("0#");
			String text4 = ((Int32)num3 % 60).ToString("0#");
			fileHud.TimeLabel.color = color;
			fileHud.TimeLabel.text = String.Concat(new String[]
			{
				text2,
				" : ",
				text3,
				" : ",
				text4
			});
			this.DisplayWindowBackground(fileHud.Self, (file.win_type != 0UL) ? FF9UIDataTool.BlueAtlas : FF9UIDataTool.GrayAtlas);
		}
		else
		{
			fileHud.Container.SetActive(false);
			fileHud.EmptySlotTextGameObject.SetActive(true);
			fileHud.EmptySlotTextLabel.color = FF9TextTool.White;
			if (isCorrupt)
			{
				switch (status)
				{
				case DataSerializerErrorCode.FileCorruption:
				case DataSerializerErrorCode.DataCorruption:
					fileHud.EmptySlotTextLabel.text = Localization.Get("LocalDecryptFailed");
					fileHud.EmptySlotTextLabel.color = FF9TextTool.Red;
					goto IL_3E7;
				case DataSerializerErrorCode.CloudDataCorruption:
					fileHud.EmptySlotTextLabel.text = Localization.Get("CloudDataCorrupt");
					fileHud.EmptySlotTextLabel.color = FF9TextTool.Red;
					goto IL_3E7;
				case DataSerializerErrorCode.CloudConnectionTimeout:
					fileHud.EmptySlotTextLabel.text = Localization.Get("CloudConnectionTimeout");
					goto IL_3E7;
				case DataSerializerErrorCode.CloudDataUnknownError:
					fileHud.EmptySlotTextLabel.text = Localization.Get("CloudDataUnknownError");
					goto IL_3E7;
				}
				fileHud.EmptySlotTextLabel.text = Localization.Get("EmptyFile");
				IL_3E7:;
			}
			else
			{
				fileHud.EmptySlotTextLabel.text = Localization.Get("EmptyFile");
			}
			this.UpdateEmptySlotLabelSize(fileHud.EmptySlotTextLabel);
		}
	}

	private void UpdateEmptySlotLabelSize(UILabel emptySlotLabel)
	{
		emptySlotLabel.SetAnchor((Transform)null);
		emptySlotLabel.updateAnchors = UIRect.AnchorUpdate.OnStart;
		Int32 width = emptySlotLabel.width;
		Int32 height = emptySlotLabel.height;
		emptySlotLabel.width = (Int32)UIManager.UIContentSize.x;
		emptySlotLabel.height = (Int32)UIManager.UIContentSize.y;
		emptySlotLabel.ProcessText();
		emptySlotLabel.UpdateNGUIText();
		Int32 num = (Int32)NGUIText.CalculatePrintedSize(emptySlotLabel.text).x + 3;
		emptySlotLabel.width = (Int32)((width >= num) ? width : num);
		emptySlotLabel.height = height;
	}

	private void DisplayTimeStamp()
	{
		String format = String.Empty;
		String language = Localization.CurrentLanguage;
		switch (language)
		{
		case "English(US)":
			format = "MM/dd/yyyy";
			break;
		case "English(UK)":
		case "Spanish":
		case "French":
		case "German":
		case "Italian":
			format = "dd/MM/yyyy";
			break;
		case "Japanese":
			format = "yyyy/MM/dd";
			break;
		}
		if (this.localFileHud.Self.activeSelf)
		{
			if (this.latestLocalTime > 0.0)
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
				String str = dateTime.AddSeconds(Math.Round(this.latestLocalTime)).ToString(format);
				this.localFileHud.LatestTimeLabel.text = Localization.Get("LatestSave") + str;
			}
			else
			{
				this.localFileHud.LatestTimeLabel.text = Localization.Get("LatestSave") + "------------";
			}
		}
		if (this.cloudFileHud.Self.activeSelf)
		{
			if (this.latestCloudTime > 0.0)
			{
				DateTime dateTime2 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
				String str2 = dateTime2.AddSeconds(Math.Round(this.latestCloudTime)).ToString(format);
				this.cloudFileHud.LatestTimeLabel.text = Localization.Get("LatestSave") + str2;
			}
			else
			{
				this.cloudFileHud.LatestTimeLabel.text = Localization.Get("LatestSave") + "------------";
			}
		}
	}

	private void DisplayOverWriteDialog()
	{
		this.OverWriteDialog.SetActive(true);
	}

	private void DisplaySuccessfulAccessDialog(Boolean isSuccess)
	{
		if (isSuccess)
		{
			if (this.syncState == CloudUI.Sync.Upload)
			{
				this.successfulAccessLabel.text = Localization.Get("SuccessUploadDesc");
			}
			else
			{
				this.successfulAccessLabel.text = Localization.Get("SuccessDownloadDesc");
			}
		}
		else
		{
			this.successfulAccessLabel.text = Localization.Get("FailedConnectDesc");
		}
		this.SuccessfulAccessPanel.SetActive(true);
		base.StartCoroutine(this.HideSuccessfulAccessDialog());
	}

	private void DisplayCorruptAccessDialog(Action callback)
	{
		String key;
		switch (ISharedDataSerializer.LastErrno)
		{
		case DataSerializerErrorCode.FileCorruption:
		case DataSerializerErrorCode.DataCorruption:
			key = "LocalDecryptFailed";
			goto IL_66;
		case DataSerializerErrorCode.CloudDataCorruption:
			key = "CloudDataCorrupt";
			goto IL_66;
		case DataSerializerErrorCode.CloudConnectionTimeout:
			key = "CloudConnectionTimeout";
			goto IL_66;
		case DataSerializerErrorCode.CloudFileNotFound:
			key = "CloudFileNotFound";
			goto IL_66;
		}
		key = "CloudDataUnknownError";
		IL_66:
		this.saveCorruptDialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get(key), 0, 0, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, Vector2.zero, Dialog.CaptionType.Notice);
		ButtonGroupState.DisableAllGroup(true);
		base.StartCoroutine(this.HideCorruptAccessDialog(callback));
	}

	private void UpdateLabelLanguage()
	{
		this.helpSlotLabel.text = Localization.Get("Download");
		this.helpTitleLabel.text = Localization.Get("SelectDataHelpInfo");
		this.syncStatusLabel.text = Localization.Get("DownloadCaption");
	}

	private IEnumerator HideSuccessfulAccessDialog()
	{
		if (this.syncState == CloudUI.Sync.Download)
		{
			yield return new WaitForSeconds(0.5f);
			FF9StateSystem.Serializer.GetSelectedLanguage(delegate(DataSerializerErrorCode errNo, Int32 selectedLanguage)
			{
				if (errNo == DataSerializerErrorCode.Success)
				{
					FF9StateSystem.Settings.CurrentLanguage = LanguageCode.ConvertToLanguageName(selectedLanguage);
					Localization.CurrentLanguage = LanguageCode.ConvertToLanguageName(selectedLanguage);
					UIManager.Field.InitializeATEText();
					this.StartCoroutine(PersistenSingleton<FF9TextTool>.Instance.UpdateTextLocalization(delegate
					{
						this.Loading = false;
						this.SuccessfulAccessPanel.SetActive(false);
						this.UpdateLabelLanguage();
						this.DisplayFileInfo(this.localFileHud, this.localPreview, this.corruptFile == CloudUI.Sync.Upload, ISharedDataSerializer.LastErrno);
						this.DisplayFileInfo(this.cloudFileHud, this.cloudPreview, this.corruptFile == CloudUI.Sync.Download, ISharedDataSerializer.LastErrno);
						this.DisplayTimeStamp();
						this.CheckData();
						ButtonGroupState.ActiveGroup = CloudUI.LocalFileGroupButton;
						ButtonGroupState.HoldActiveStateOnGroup(CloudUI.SubMenuGroupButton);
					}));
				}
				else
				{
					this.Loading = false;
					this.SuccessfulAccessPanel.SetActive(false);
					this.DisplayFileInfo(this.localFileHud, this.localPreview, this.corruptFile == CloudUI.Sync.Upload, ISharedDataSerializer.LastErrno);
					this.DisplayFileInfo(this.cloudFileHud, this.cloudPreview, this.corruptFile == CloudUI.Sync.Download, ISharedDataSerializer.LastErrno);
					this.DisplayTimeStamp();
					this.CheckData();
					ButtonGroupState.ActiveGroup = CloudUI.LocalFileGroupButton;
					ButtonGroupState.HoldActiveStateOnGroup(CloudUI.SubMenuGroupButton);
				}
			});
		}
		else
		{
			yield return new WaitForSeconds(1.5f);
			base.Loading = false;
			this.SuccessfulAccessPanel.SetActive(false);
			this.DisplayFileInfo(this.localFileHud, this.localPreview, this.corruptFile == CloudUI.Sync.Upload, ISharedDataSerializer.LastErrno);
			this.DisplayFileInfo(this.cloudFileHud, this.cloudPreview, this.corruptFile == CloudUI.Sync.Download, ISharedDataSerializer.LastErrno);
			this.DisplayTimeStamp();
			this.CheckData();
			ButtonGroupState.ActiveGroup = CloudUI.CloudFileGroupButton;
			ButtonGroupState.HoldActiveStateOnGroup(CloudUI.SubMenuGroupButton);
		}
		yield break;
	}

	private IEnumerator HideCorruptAccessDialog(Action callback)
	{
		yield return new WaitForSeconds(1.5f);
		this.saveCorruptDialog.Hide();
		if (callback != null)
		{
			callback();
		}
		yield break;
	}

	private IEnumerator RunProgressBar()
	{
		this.progressBar.value = 0f;
		while ((Double)this.progressBar.value < 0.95)
		{
			yield return new WaitForSeconds(0.01f);
			this.progressBar.value += 0.01f;
		}
		yield break;
	}

	private void OnFinishedLoadPreview(DataSerializerErrorCode errNo, Boolean isSuccess, SharedDataPreviewSlot localData, SharedDataPreviewSlot cloudData)
	{
		if (errNo == DataSerializerErrorCode.Success)
		{
			this.corruptFile = CloudUI.Sync.None;
			if (isSuccess)
			{
				this.isLocalFileExist = (localData != (SharedDataPreviewSlot)null);
				this.isCloudFileExist = (cloudData != (SharedDataPreviewSlot)null);
				this.localPreview = localData;
				this.cloudPreview = cloudData;
			}
			else
			{
				this.isLocalFileExist = false;
				this.isCloudFileExist = false;
				this.localPreview = (SharedDataPreviewSlot)null;
				this.cloudPreview = (SharedDataPreviewSlot)null;
			}
			if (this.isLocalFileExist)
			{
				FF9StateSystem.Serializer.GetLocalSaveLatestTimestamp(new ISharedDataSerializer.OnGetLatestSaveTimestamp(this.OnFinishGetTimeLocal));
			}
			else
			{
				this.OnFinishGetTimeLocal(DataSerializerErrorCode.Success, true, 0.0);
			}
		}
		else
		{
			if (localData == null)
			{
				this.corruptFile = CloudUI.Sync.Upload;
			}
			else if (cloudData == null)
			{
				this.corruptFile = CloudUI.Sync.Download;
			}
			this.isLocalFileExist = (localData != (SharedDataPreviewSlot)null);
			this.isCloudFileExist = (cloudData != (SharedDataPreviewSlot)null);
			this.localPreview = localData;
			this.cloudPreview = cloudData;
			if (this.currentState == CloudUI.State.LoadPreview)
			{
				base.StartCoroutine(this.OnFinishedLoadPreview_delay(false));
			}
			else if (this.currentState == CloudUI.State.Sync)
			{
				base.StartCoroutine(this.OnFinishedSync_delay(false));
			}
		}
	}

	private void OnFinishedSync(DataSerializerErrorCode errNo, Boolean isSuccess, SharedDataPreviewSlot localData, SharedDataPreviewSlot cloudData)
	{
		this.isSyncSuccess = (isSuccess && errNo == DataSerializerErrorCode.Success);
		if (this.isSyncSuccess)
		{
			this.corruptFile = CloudUI.Sync.None;
			this.isLocalFileExist = (localData != (SharedDataPreviewSlot)null);
			this.isCloudFileExist = (cloudData != (SharedDataPreviewSlot)null);
			this.localPreview = localData;
			this.cloudPreview = cloudData;
			if (this.isLocalFileExist)
			{
				FF9StateSystem.Serializer.GetLocalSaveLatestTimestamp(new ISharedDataSerializer.OnGetLatestSaveTimestamp(this.OnFinishGetTimeLocal));
			}
			else
			{
				this.OnFinishGetTimeLocal(DataSerializerErrorCode.Success, true, 0.0);
			}
		}
		else
		{
			if (localData == null)
			{
				this.corruptFile = CloudUI.Sync.Upload;
			}
			else if (cloudData == null)
			{
				this.corruptFile = CloudUI.Sync.Download;
			}
			this.isLocalFileExist = (localData != (SharedDataPreviewSlot)null);
			this.isCloudFileExist = (cloudData != (SharedDataPreviewSlot)null);
			this.localPreview = localData;
			this.cloudPreview = cloudData;
			if (this.currentState == CloudUI.State.LoadPreview)
			{
				base.StartCoroutine(this.OnFinishedLoadPreview_delay(false));
			}
			else if (this.currentState == CloudUI.State.Sync)
			{
				base.StartCoroutine(this.OnFinishedSync_delay(false));
			}
		}
	}

	private void OnFinishGetTimeLocal(DataSerializerErrorCode errNo, Boolean isSuccess, Double timestamp)
	{
		if (isSuccess && errNo == DataSerializerErrorCode.Success)
		{
			this.latestLocalTime = timestamp;
			if (this.isCloudFileExist)
			{
				FF9StateSystem.Serializer.GetCloudSaveLatestTimestamp(new ISharedDataSerializer.OnGetLatestSaveTimestamp(this.OnFinishGetTimeCloud));
			}
			else
			{
				this.OnFinishGetTimeCloud(DataSerializerErrorCode.Success, true, 0.0);
			}
		}
		else
		{
			this.isLocalFileExist = false;
			this.isCloudFileExist = false;
			this.localPreview = (SharedDataPreviewSlot)null;
			this.cloudPreview = (SharedDataPreviewSlot)null;
			if (this.currentState == CloudUI.State.LoadPreview)
			{
				base.StartCoroutine(this.OnFinishedLoadPreview_delay(false));
			}
			else if (this.currentState == CloudUI.State.Sync)
			{
				base.StartCoroutine(this.OnFinishedSync_delay(false));
			}
		}
	}

	private void OnFinishGetTimeCloud(DataSerializerErrorCode errNo, Boolean isSuccess, Double timestamp)
	{
		if (isSuccess && errNo == DataSerializerErrorCode.Success)
		{
			this.latestCloudTime = timestamp;
			if (this.currentState == CloudUI.State.LoadPreview)
			{
				base.StartCoroutine(this.OnFinishedLoadPreview_delay(true));
			}
			else if (this.currentState == CloudUI.State.Sync)
			{
				base.StartCoroutine(this.OnFinishedSync_delay(true));
			}
		}
		else
		{
			this.isLocalFileExist = false;
			this.isCloudFileExist = false;
			this.localPreview = (SharedDataPreviewSlot)null;
			this.cloudPreview = (SharedDataPreviewSlot)null;
			if (this.currentState == CloudUI.State.LoadPreview)
			{
				base.StartCoroutine(this.OnFinishedLoadPreview_delay(false));
			}
			else if (this.currentState == CloudUI.State.Sync)
			{
				base.StartCoroutine(this.OnFinishedSync_delay(false));
			}
		}
	}

	private IEnumerator OnFinishedLoadPreview_delay(Boolean isSuccess)
	{
		Single remainTime = Mathf.Max(2f - (Time.time - this.timeCounter), 0f);
		yield return new WaitForSeconds(remainTime);
		this.LoadingPreviewDialog.SetActive(false);
		this.helpTitleLabel.text = Localization.Get("SelectSyncHelpInfo");
		if (isSuccess)
		{
			base.Loading = false;
			this.currentState = CloudUI.State.None;
			FF9Sfx.FF9SFX_Play(1044);
			this.DisplayFileInfo(this.localFileHud, this.localPreview, this.corruptFile == CloudUI.Sync.Upload, ISharedDataSerializer.LastErrno);
			this.DisplayFileInfo(this.cloudFileHud, this.cloudPreview, this.corruptFile == CloudUI.Sync.Download, ISharedDataSerializer.LastErrno);
			this.DisplayTimeStamp();
			ButtonGroupState.RemoveCursorMemorize(CloudUI.SubMenuGroupButton);
			ButtonGroupState.ActiveGroup = CloudUI.SubMenuGroupButton;
			this.CheckData();
		}
		else
		{
			FF9Sfx.FF9SFX_Play(1046);
			this.DisplayCorruptAccessDialog(delegate
			{
				this.Loading = false;
				this.currentState = CloudUI.State.None;
				this.DisplayFileInfo(this.localFileHud, this.localPreview, this.corruptFile == CloudUI.Sync.Upload, ISharedDataSerializer.LastErrno);
				this.DisplayFileInfo(this.cloudFileHud, this.cloudPreview, this.corruptFile == CloudUI.Sync.Download, ISharedDataSerializer.LastErrno);
				this.DisplayTimeStamp();
				ButtonGroupState.RemoveCursorMemorize(CloudUI.SubMenuGroupButton);
				ButtonGroupState.ActiveGroup = CloudUI.SubMenuGroupButton;
				this.CheckData();
			});
		}
		yield break;
	}

	private IEnumerator OnFinishedSync_delay(Boolean isSuccess)
	{
		Single remainTime = Mathf.Max(2f - (Time.time - this.timeCounter), 0f);
		yield return new WaitForSeconds(remainTime);
		this.progressBar.value = 1f;
		yield return new WaitForSeconds(0.1f);
		this.LoadingAccessDialog.SetActive(false);
		if (isSuccess)
		{
			FF9Sfx.FF9SFX_Play(103);
			this.currentState = CloudUI.State.None;
			this.DisplaySuccessfulAccessDialog(this.isSyncSuccess);
		}
		else
		{
			FF9Sfx.FF9SFX_Play(1046);
			this.DisplayCorruptAccessDialog(delegate
			{
				this.Loading = false;
				this.currentState = CloudUI.State.None;
				this.DisplayFileInfo(this.localFileHud, this.localPreview, this.corruptFile == CloudUI.Sync.Upload, ISharedDataSerializer.LastErrno);
				this.DisplayFileInfo(this.cloudFileHud, this.cloudPreview, this.corruptFile == CloudUI.Sync.Download, ISharedDataSerializer.LastErrno);
				this.DisplayTimeStamp();
				this.CheckData();
				if (this.syncState == CloudUI.Sync.Upload)
				{
					ButtonGroupState.ActiveGroup = CloudUI.CloudFileGroupButton;
				}
				else
				{
					ButtonGroupState.ActiveGroup = CloudUI.LocalFileGroupButton;
				}
				ButtonGroupState.HoldActiveStateOnGroup(CloudUI.SubMenuGroupButton);
			});
		}
		yield break;
	}

	private void CheckData()
	{
		if (this.isLocalFileExist)
		{
			if (ButtonGroupState.ActiveGroup == CloudUI.SubMenuGroupButton)
			{
				ButtonGroupState.SetButtonEnable(this.UploadButton, true);
			}
			this.uploadButtonLabel.color = FF9TextTool.White;
		}
		else
		{
			if (ButtonGroupState.ActiveGroup == CloudUI.SubMenuGroupButton)
			{
				ButtonGroupState.SetButtonEnable(this.UploadButton, false);
			}
			this.uploadButtonLabel.color = FF9TextTool.Gray;
		}
		if (this.isCloudFileExist)
		{
			if (ButtonGroupState.ActiveGroup == CloudUI.SubMenuGroupButton)
			{
				ButtonGroupState.SetButtonEnable(this.DownloadButton, true);
			}
			this.downloadButtonLabel.color = FF9TextTool.White;
		}
		else
		{
			if (ButtonGroupState.ActiveGroup == CloudUI.SubMenuGroupButton)
			{
				ButtonGroupState.SetButtonEnable(this.DownloadButton, false);
			}
			this.downloadButtonLabel.color = FF9TextTool.Gray;
		}
	}

	private void SyncFileToCloud()
	{
		base.Loading = true;
		this.timeCounter = Time.time;
		this.currentState = CloudUI.State.Sync;
		base.StartCoroutine("RunProgressBar");
		if (this.syncState == CloudUI.Sync.Upload)
		{
			FF9StateSystem.Serializer.UploadToCloud((ISharedDataSerializer.OnSyncCloudSlotStart)null, new ISharedDataSerializer.OnSyncCloudSlotFinish(this.OnFinishedSync));
			this.loadingAccessLocalize.key = "WarningUploadData";
		}
		else
		{
			FF9StateSystem.Serializer.DownloadFromCloud((ISharedDataSerializer.OnSyncCloudSlotStart)null, new ISharedDataSerializer.OnSyncCloudSlotFinish(this.OnFinishedSync));
			this.loadingAccessLocalize.key = "WarningDownloadData";
		}
		this.LoadingAccessDialog.SetActive(true);
	}

	private void Awake()
	{
		base.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
		this.helpTitleLabel = this.HelpPanel.GetChild(0).GetComponent<UILabel>();
		this.helpSlotLabel = this.HelpPanel.GetChild(1).GetChild(0).GetComponent<UILabel>();
		this.slotGameObject = this.HelpPanel.GetChild(1);
		this.infoLabel = this.InfoPanel.GetChild(0).GetComponent<UILabel>();
		this.loadingAccessLocalize = this.LoadingAccessDialog.GetChild(1).GetComponent<UILocalize>();
		this.progressBar = this.LoadingAccessDialog.GetChild(2).GetComponent<UISlider>();
		this.syncStatusLabel = this.FileListPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
		this.arrowSprites = new UISprite[]
		{
			this.FileListPanel.GetChild(1).GetChild(0).GetComponent<UISprite>(),
			this.FileListPanel.GetChild(1).GetChild(2).GetComponent<UISprite>()
		};
		UIEventListener uieventListener = UIEventListener.Get(this.InfoButton);
		uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.onClick));
		UIEventListener uieventListener2 = UIEventListener.Get(this.UploadButton);
		uieventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener2.onClick, new UIEventListener.VoidDelegate(this.onClick));
		UIEventListener uieventListener3 = UIEventListener.Get(this.DownloadButton);
		uieventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener3.onClick, new UIEventListener.VoidDelegate(this.onClick));
		this.localFileHud = new CloudUI.FileCloudInfoHUD(this.FileListPanel.GetChild(2));
		this.cloudFileHud = new CloudUI.FileCloudInfoHUD(this.FileListPanel.GetChild(0));
		UIEventListener uieventListener4 = UIEventListener.Get(this.localFileHud.Self);
		uieventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener4.onClick, new UIEventListener.VoidDelegate(this.onClick));
		UIEventListener uieventListener5 = UIEventListener.Get(this.cloudFileHud.Self);
		uieventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener5.onClick, new UIEventListener.VoidDelegate(this.onClick));
		this.uploadButtonLabel = this.UploadButton.GetChild(1).GetComponent<UILabel>();
		this.downloadButtonLabel = this.DownloadButton.GetChild(1).GetComponent<UILabel>();
		this.overWriteDialogDespLabel = this.OverWriteDialog.GetChild(0).GetComponent<UILabel>();
		UIEventListener uieventListener6 = UIEventListener.Get(this.OverWriteDialog.GetChild(1));
		uieventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener6.onClick, new UIEventListener.VoidDelegate(this.onClick));
		UIEventListener uieventListener7 = UIEventListener.Get(this.OverWriteDialog.GetChild(2));
		uieventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener7.onClick, new UIEventListener.VoidDelegate(this.onClick));
		this.successfulAccessLabel = this.SuccessfulAccessPanel.GetChild(0).GetComponent<UILabel>();
	}

	private static String SubMenuGroupButton = "Cloud.SubMenu";

	private static String LocalFileGroupButton = "Cloud.LocalFile";

	private static String CloudFileGroupButton = "Cloud.CloudFile";

	private static String ConfirmDialogGroupButton = "Cloud.Choice";

	public GameObject HelpPanel;

	public GameObject FileListPanel;

	public GameObject InfoPanel;

	public GameObject LoadingPreviewDialog;

	public GameObject OverWriteDialog;

	public GameObject LoadingAccessDialog;

	public GameObject SuccessfulAccessPanel;

	public GameObject InfoButton;

	public GameObject UploadButton;

	public GameObject DownloadButton;

	public GameObject ScreenFadeGameObject;

	private GameObject slotGameObject;

	private UISlider progressBar;

	private UILabel helpTitleLabel;

	private UILabel helpSlotLabel;

	private UILabel syncStatusLabel;

	private UILabel successfulAccessLabel;

	private UISprite[] arrowSprites;

	private CloudUI.FileCloudInfoHUD localFileHud;

	private CloudUI.FileCloudInfoHUD cloudFileHud;

	private UILabel infoLabel;

	private UILabel uploadButtonLabel;

	private UILabel downloadButtonLabel;

	private UILabel overWriteDialogDespLabel;

	private UILocalize loadingAccessLocalize;

	private Dialog saveCorruptDialog;

	private Single timeCounter;

	private CloudUI.Sync syncState;

	private CloudUI.State currentState;

	private Boolean isSyncSuccess;

	private CloudUI.Sync corruptFile;

	private Boolean isLocalFileExist;

	private Boolean isCloudFileExist;

	private Double latestLocalTime;

	private Double latestCloudTime;

	private SharedDataPreviewSlot localPreview;

	private SharedDataPreviewSlot cloudPreview;

	private struct FileCloudInfoHUD
	{
		public FileCloudInfoHUD(GameObject go)
		{
			this.Self = go;
			this.Container = go.GetChild(0);
			this.Button = go.GetComponent<ButtonGroupState>();
			this.LatestTimeLabel = go.GetChild(0).GetChild(0).GetComponent<UILabel>();
			this.LeaderNameLabel = go.GetChild(0).GetChild(1).GetComponent<UILabel>();
			this.TimeLabel = go.GetChild(0).GetChild(3).GetComponent<UILabel>();
			this.LeaderLvLabel = go.GetChild(0).GetChild(5).GetComponent<UILabel>();
			this.GilLabel = go.GetChild(0).GetChild(7).GetComponent<UILabel>();
			this.FileNoLabel = go.GetChild(1).GetComponent<UILabel>();
			this.EmptySlotTextGameObject = go.GetChild(2);
			this.EmptySlotTextLabel = go.GetChild(2).GetComponent<UILabel>();
			this.LocationNameLabel = go.GetChild(0).GetChild(9).GetChild(0).GetComponent<UILabel>();
			this.CharacterAvatarList = new UISprite[]
			{
				go.GetChild(0).GetChild(8).GetChild(0).GetComponent<UISprite>(),
				go.GetChild(0).GetChild(8).GetChild(1).GetComponent<UISprite>(),
				go.GetChild(0).GetChild(8).GetChild(2).GetComponent<UISprite>(),
				go.GetChild(0).GetChild(8).GetChild(3).GetComponent<UISprite>()
			};
		}

		public GameObject Self;

		public GameObject Container;

		public ButtonGroupState Button;

		public UILabel LatestTimeLabel;

		public UILabel LeaderNameLabel;

		public UILabel LeaderLvLabel;

		public UILabel GilLabel;

		public UILabel TimeLabel;

		public UILabel LocationNameLabel;

		public GameObject EmptySlotTextGameObject;

		public UILabel EmptySlotTextLabel;

		public UILabel FileNoLabel;

		public UISprite[] CharacterAvatarList;
	}

	public enum Sync
	{
		Upload,
		Download,
		None
	}

	public enum State
	{
		LoadPreview,
		Sync,
		None
	}
}
