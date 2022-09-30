using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.SiliconSocial;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Data;
using Memoria.Assets;
using UnityEngine;
using Object = System.Object;

public class SaveLoadUI : UIScene
{
	public SaveLoadUI.SerializeType Type
	{
		get
		{
			return this.type;
		}
		set
		{
			this.type = value;
		}
	}

	private void LoadDataStartDelgate(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID)
	{
		global::Debug.Log("LoadDataStartDelgate()");
		PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, (Action)null);
	}

	private void LoadDataFinishDelegate(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID, Boolean isSuccess)
	{
		global::Debug.Log("LoadDataFinishDelegate()");
		PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, (Action)null);
		if (!PersistenSingleton<EventEngine>.Instance.GetUserControl())
		{
			PersistenSingleton<UIManager>.Instance.IsPlayerControlEnable = false;
		}
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		instance.ReplaceFieldMap();
		PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.FieldHUD);
	}

	private void SaveDataStartDelegate(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID)
	{
		global::Debug.Log("SaveDataStartDelegate()");
		PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, (Action)null);
	}

	private void SaveDataFinishDelegate(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID, Boolean isSuccess, SharedDataPreviewSlot preview)
	{
		global::Debug.Log("SaveDataFinishDelegate()");
		PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, (Action)null);
		if (!PersistenSingleton<EventEngine>.Instance.GetUserControl())
		{
			PersistenSingleton<UIManager>.Instance.IsPlayerControlEnable = false;
		}
	}

	public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
	{
		if (FF9StateSystem.aaaaPlatform)
		{
			if (this.type == SaveLoadUI.SerializeType.Load)
			{
				FF9StateSystem.Serializer.Load(0, 0, new ISharedDataSerializer.OnSaveLoadStart(this.LoadDataStartDelgate), new ISharedDataSerializer.OnLoadFinish(this.LoadDataFinishDelegate));
			}
			else
			{
				FF9StateSystem.Serializer.Save(0, 0, new ISharedDataSerializer.OnSaveLoadStart(this.SaveDataStartDelegate), new ISharedDataSerializer.OnSaveFinish(this.SaveDataFinishDelegate));
			}
		}
		else
		{
			UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
			{
				ButtonGroupState.SetPointerDepthToGroup(11, SaveLoadUI.FileGroupButton);
				ButtonGroupState.SetPointerOffsetToGroup(new Vector2(48f, 0f), SaveLoadUI.SlotGroupButton);
				ButtonGroupState.SetPointerOffsetToGroup(new Vector2(48f, 0f), SaveLoadUI.FileGroupButton);
				ButtonGroupState.SetPointerOffsetToGroup(new Vector2(180f, 10f), SaveLoadUI.ConfirmDialogGroupButton);
				ButtonGroupState.SetPointerLimitRectToGroup(this.FileListPanel.GetComponent<UIWidget>(), (Single)this.fileScrollList.ItemHeight, SaveLoadUI.FileGroupButton);
				ButtonGroupState.SetScrollButtonToGroup(this.FileListPanel.GetChild(0).GetComponent<ScrollButton>(), SaveLoadUI.FileGroupButton);
				Int32 index = (Int32)((FF9StateSystem.Settings.LatestSlot <= -1) ? 0 : FF9StateSystem.Settings.LatestSlot);
				ButtonGroupState.SetCursorStartSelect(this.slotNameButtonList[index].gameObject, SaveLoadUI.SlotGroupButton);
				ButtonGroupState.ActiveGroup = SaveLoadUI.SlotGroupButton;
			};
			if (afterFinished != null)
			{
				sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
			}
			SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
			base.Show(sceneVoidDelegate);
			this.currentSlot = -1;
			this.currentFile = -1;
			this.setSerialzeType(this.Type);
			this.screenFadePanel.depth = 7;
			this.DisplaySlot();
			this.DisplayHelp();
		}
	}

	public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
	{
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
		{
			FF9StateSystem.Common.FF9.attr &= 4294967293u;
			if (this.type == SaveLoadUI.SerializeType.Save)
			{
				SceneDirector.FF9Wipe_FadeInEx(24);
			}
		};
        if (PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.WorldHUD && ButtonGroupState.HelpEnabled)
        {
            ButtonGroupState.ToggleHelp(false);
        }
        if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		base.Hide(sceneVoidDelegate);
		this.screenFadePanel.depth = 10;
	}

	public override Boolean OnKeyConfirm(GameObject go)
	{
		if (base.OnKeyConfirm(go))
		{
			if (ButtonGroupState.ActiveGroup == SaveLoadUI.SlotGroupButton)
			{
				FF9Sfx.FF9SFX_Play(103);
				this.currentSlot = go.transform.GetSiblingIndex();
				ButtonGroupState.DisableAllGroup(true);
				this.LoadingPreviewDialog.SetActive(true);
				FF9UIDataTool.DisplayTextLocalize(this.HelpTitleLabel, "SaveHelpCheck");
				base.Loading = true;
				this.timeCounter = Time.time;
				FF9StateSystem.Serializer.LoadSlotPreview(this.currentSlot, new ISharedDataSerializer.OnLoadSlotFinish(this.OnFinishedLoadPreview));
			}
			else if (ButtonGroupState.ActiveGroup == SaveLoadUI.FileGroupButton)
			{
				this.currentFile = go.GetComponent<ScrollItemKeyNavigation>().ID;
				if (this.type == SaveLoadUI.SerializeType.Save)
				{
					FF9Sfx.FF9SFX_Play(103);
					if (this.isFileExistList[this.currentFile])
					{
						this.DisplayOverWriteDialog();
						ButtonGroupState.ActiveGroup = SaveLoadUI.ConfirmDialogGroupButton;
					}
					else
					{
						ButtonGroupState.DisableAllGroup(true);
						FF9UIDataTool.DisplayTextLocalize(this.HelpTitleLabel, "SaveHelpSave");
						FF9UIDataTool.DisplayTextLocalize(this.loadingAccessText, "WarningSaveData");
						this.LoadingAccessPanel.SetActive(true);
						base.StartCoroutine("RunProgressBar");
						this.timeCounter = Time.time;
						base.Loading = true;
						FF9StateSystem.Settings.UpdateTickTime();
						FF9StateSystem.Serializer.Save(this.currentSlot, this.currentFile, (ISharedDataSerializer.OnSaveLoadStart)null, new ISharedDataSerializer.OnSaveFinish(this.OnFinishedSaveFile));
					}
				}
				else if (this.isFileExistList[this.currentFile])
				{
					FF9Sfx.FF9SFX_Play(103);
					ButtonGroupState.DisableAllGroup(true);
					FF9UIDataTool.DisplayTextLocalize(this.HelpTitleLabel, "SaveHelpLoad");
					FF9UIDataTool.DisplayTextLocalize(this.loadingAccessText, "WarningAccessData");
					this.LoadingAccessPanel.SetActive(true);
					base.StartCoroutine("RunProgressBar");
					this.timeCounter = Time.time;
					base.Loading = true;
					FF9StateSystem.Serializer.Load(this.currentSlot, this.currentFile, (ISharedDataSerializer.OnSaveLoadStart)null, new ISharedDataSerializer.OnLoadFinish(this.OnFinishedLoadFile));
				}
				else
				{
					FF9Sfx.FF9SFX_Play(102);
				}
			}
			else if (ButtonGroupState.ActiveGroup == SaveLoadUI.ConfirmDialogGroupButton)
			{
				this.OverWriteDialog.SetActive(false);
				if (go.transform.GetSiblingIndex() == 1)
				{
					FF9Sfx.FF9SFX_Play(103);
					ButtonGroupState.DisableAllGroup(true);
					FF9UIDataTool.DisplayTextLocalize(this.HelpTitleLabel, "SaveHelpSave");
					FF9UIDataTool.DisplayTextLocalize(this.loadingAccessText, "WarningSaveData");
					this.LoadingAccessPanel.SetActive(true);
					base.StartCoroutine("RunProgressBar");
					this.timeCounter = Time.time;
					base.Loading = true;
					FF9StateSystem.Settings.UpdateTickTime();
					FF9StateSystem.Serializer.Save(this.currentSlot, this.currentFile, (ISharedDataSerializer.OnSaveLoadStart)null, new ISharedDataSerializer.OnSaveFinish(this.OnFinishedSaveFile));
				}
				else
				{
					FF9Sfx.FF9SFX_Play(101);
					ButtonGroupState.ActiveGroup = SaveLoadUI.FileGroupButton;
				}
			}
		}
		return true;
	}

	public override Boolean OnKeyCancel(GameObject go)
	{
		if (base.OnKeyCancel(go))
		{
			if (ButtonGroupState.ActiveGroup == SaveLoadUI.SlotGroupButton)
			{
				FF9Sfx.FF9SFX_Play(101);
				this.Hide(delegate
				{
					PersistenSingleton<UIManager>.Instance.ChangeUIState(PersistenSingleton<UIManager>.Instance.HUDState);
				});
			}
			else if (ButtonGroupState.ActiveGroup == SaveLoadUI.FileGroupButton)
			{
				FF9Sfx.FF9SFX_Play(101);
				base.Loading = true;
				base.FadingComponent.FadePingPong(delegate
				{
					FF9UIDataTool.DisplayTextLocalize(this.HelpTitleLabel, "SaveHelpSlot");
					this.SlotListPanel.SetActive(true);
					this.FileListPanel.SetActive(false);
					ButtonGroupState.ActiveGroup = SaveLoadUI.SlotGroupButton;
				}, delegate
				{
					base.Loading = false;
				});
			}
			else if (ButtonGroupState.ActiveGroup == SaveLoadUI.ConfirmDialogGroupButton)
			{
				FF9Sfx.FF9SFX_Play(101);
				this.OverWriteDialog.SetActive(false);
				ButtonGroupState.ActiveGroup = SaveLoadUI.FileGroupButton;
			}
		}
		return true;
	}

	public override Boolean OnItemSelect(GameObject go)
	{
		if (base.OnItemSelect(go))
		{
			if (ButtonGroupState.ActiveGroup == SaveLoadUI.SlotGroupButton)
			{
				if (this.currentSlot != go.transform.GetSiblingIndex())
				{
					this.currentSlot = go.transform.GetSiblingIndex();
					this.helpSlotLabel.text = String.Format(Localization.Get("SlotNo"), this.currentSlot + 1);
				}
			}
			else if (ButtonGroupState.ActiveGroup == SaveLoadUI.FileGroupButton)
			{
				ButtonGroupState.SetCursorStartSelect(go, SaveLoadUI.FileGroupButton);
			}
		}
		return true;
	}

	private void DisplayHelp()
	{
		Int32 num = 0;
		foreach (ButtonGroupState buttonGroupState in this.slotNameButtonList)
		{
			if (this.type == SaveLoadUI.SerializeType.Save)
			{
				buttonGroupState.Help.Text = String.Format(Localization.Get("SaveSlotHelp"), num + 1);
			}
			else
			{
				buttonGroupState.Help.Text = String.Format(Localization.Get("LoadSlotHelp"), num + 1);
			}
			num++;
		}
		if (FF9StateSystem.PCPlatform)
		{
			this.HelpDespLabelGameObject.SetActive(true);
		}
		else
		{
			this.HelpDespLabelGameObject.SetActive(false);
		}
	}

	private void DisplayFile(List<SharedDataPreviewSlot> data)
	{
		ScrollButton component = this.FileListPanel.GetChild(0).GetComponent<ScrollButton>();
		UIPanel component2 = this.FileListPanel.GetChild(1).GetComponent<UIPanel>();
		Int32 num = 0;
		Double num2 = 0.0;
		this.currentFile = 0;
		this.isFileExistList.Clear();
		this.isFileCorrupt.Clear();
		foreach (SharedDataPreviewSlot sharedDataPreviewSlot in data)
		{
			if (sharedDataPreviewSlot != null && sharedDataPreviewSlot.Timestamp > num2)
			{
				num2 = sharedDataPreviewSlot.Timestamp;
				this.currentFile = num;
			}
			this.isFileExistList.Add(sharedDataPreviewSlot != (SharedDataPreviewSlot)null);
			this.isFileCorrupt.Add(false);
			this.DisplayFileInfo(num++, sharedDataPreviewSlot);
		}
		component.CheckScrollPosition();
		ButtonGroupState.SetCursorStartSelect(this.fileInfoHudList[this.currentFile].Self, SaveLoadUI.FileGroupButton);
		ButtonGroupState.RemoveCursorMemorize(SaveLoadUI.FileGroupButton);
		ButtonGroupState.ActiveGroup = SaveLoadUI.FileGroupButton;
		this.fileScrollList.ScrollToIndex(this.currentFile);
	}

	private void DisplayFileInfo(Int32 index, SharedDataPreviewSlot file)
	{
		SaveLoadUI.FileInfoHUD fileInfoHUD = this.fileInfoHudList[index];
		fileInfoHUD.Self.SetActive(true);
		fileInfoHUD.FileNoLabel.text = String.Format(Localization.Get("FileNo"), (index + 1).ToString("0#"));
		if (file != null && !this.isFileCorrupt[index] && !file.IsPreviewCorrupted)
		{
			fileInfoHUD.Container.SetActive(true);
			fileInfoHUD.EmptySlotTextGameObject.SetActive(false);
			Int32 num = 1;
			String text = String.Empty;
			String text2 = String.Empty;
			Boolean flag = false;
			Int32 num2 = 0;
			foreach (SharedDataPreviewCharacterInfo sharedDataPreviewCharacterInfo in file.CharacterInfoList)
			{
				fileInfoHUD.CharacterAvatarList[num2].alpha = 0f;
				fileInfoHUD.CharacterAvatarList[num2].spriteName = String.Empty;
				if (sharedDataPreviewCharacterInfo != null)
				{
					if (!flag)
					{
						num = sharedDataPreviewCharacterInfo.Level;
						text = sharedDataPreviewCharacterInfo.Name;
						flag = true;
					}
					fileInfoHUD.CharacterAvatarList[num2].alpha = 1f;
					FF9UIDataTool.DisplayCharacterAvatar((CharacterSerialNumber)sharedDataPreviewCharacterInfo.SerialID, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), fileInfoHUD.CharacterAvatarList[num2], false);
					String text3 = text2;
					text2 = String.Concat(new String[]
					{
						text3,
						sharedDataPreviewCharacterInfo.Name,
						"[XTAB=80][YADD=4]",
						Localization.Get("LvDialogIcon"),
						"[YSUB=2][FEED=1] ",
						sharedDataPreviewCharacterInfo.Level.ToString(),
						"\n"
					});
				}
				num2++;
			}
			fileInfoHUD.LeaderNameLabel.text = text;
			fileInfoHUD.LeaderLvLabel.text = num.ToString();
			fileInfoHUD.GilLabel.text = file.Gil.ToString() + "[YSUB=1.3][sub]G";
			fileInfoHUD.LocationNameLabel.text = file.Location;
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
			String text4 = ((Int32)(num3 / 3600.0)).ToString("0#");
			String text5 = ((Int32)(num3 / 60.0) % 60).ToString("0#");
			String text6 = ((Int32)num3 % 60).ToString("0#");
			fileInfoHUD.TimeLabel.color = color;
			fileInfoHUD.TimeLabel.text = String.Concat(new String[]
			{
				text4,
				" : ",
				text5,
				" : ",
				text6
			});
			this.DisplayWindowBackground(fileInfoHUD.Self, (file.win_type != 0UL) ? FF9UIDataTool.BlueAtlas : FF9UIDataTool.GrayAtlas);
			fileInfoHUD.Button.Help.TextKey = String.Empty;
			fileInfoHUD.Button.Help.Text = text2.Remove(text2.Length - 1);
		}
		else if (file == null)
		{
			fileInfoHUD.Container.SetActive(false);
			fileInfoHUD.EmptySlotTextGameObject.SetActive(true);
			if (this.isFileCorrupt[index])
			{
				fileInfoHUD.Button.Help.TextKey = "CorruptFile";
				fileInfoHUD.EmptySlotTextLabel.text = Localization.Get("CorruptFile");
				fileInfoHUD.EmptySlotTextLabel.color = FF9TextTool.Red;
			}
			else
			{
				fileInfoHUD.Button.Help.TextKey = "NoSaveHelp";
				fileInfoHUD.EmptySlotTextLabel.text = Localization.Get("EmptyFile");
				fileInfoHUD.EmptySlotTextLabel.color = FF9TextTool.White;
			}
			this.DisplayWindowBackground(fileInfoHUD.Self, (UIAtlas)null);
		}
		else if (file.IsPreviewCorrupted)
		{
			fileInfoHUD.Container.SetActive(false);
			fileInfoHUD.EmptySlotTextGameObject.SetActive(true);
			fileInfoHUD.Button.Help.TextKey = "CorruptFile";
			fileInfoHUD.EmptySlotTextLabel.text = Localization.Get("CorruptFile");
			fileInfoHUD.EmptySlotTextLabel.color = FF9TextTool.Red;
			this.DisplayWindowBackground(fileInfoHUD.Self, (UIAtlas)null);
		}
	}

	private void DisplaySuccessfulAccessDialog()
	{
		if (this.type == SaveLoadUI.SerializeType.Save)
		{
			this.SuccessfulAccessPanel.transform.position = this.fileInfoHudList[this.currentFile].Self.transform.position - new Vector3(0.8f, 0.05f);
			this.SuccessfulAccessPanel.SetActive(true);
			FF9UIDataTool.DisplayTextLocalize(this.successfulAccessGameObject, "SaveSuccess");
			base.Loading = true;
			base.StartCoroutine("HideSuccessfulAccessDialog");
		}
	}

	private IEnumerator HideSuccessfulAccessDialog()
	{
		yield return new WaitForSeconds(1.5f);
		this.SuccessfulAccessPanel.SetActive(false);
		FF9UIDataTool.DisplayTextLocalize(this.HelpTitleLabel, "SaveHelpBlock");
		if (this.type == SaveLoadUI.SerializeType.Save)
		{
			base.Loading = false;
			ButtonGroupState.ActiveGroup = SaveLoadUI.FileGroupButton;
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

	private void DisplayCorruptAccessDialog(String group, SaveLoadUI.SerializeType serializeType, DataSerializerErrorCode errorCode)
	{
		String key;
        switch (errorCode)
        {
            case DataSerializerErrorCode.FileCorruption:
            case DataSerializerErrorCode.DataCorruption:
                key = "LocalDecryptFailed";
                break;
            case DataSerializerErrorCode.CloudDataCorruption:
                key = "CloudDataCorrupt";
                break;
            case DataSerializerErrorCode.CloudConnectionTimeout:
                key = "CloudConnectionTimeout";
                break;
            case DataSerializerErrorCode.CloudFileNotFound:
                key = "CloudFileNotFound";
                break;
            case DataSerializerErrorCode.CloudConnectionError:
                key = "CloudDataUnknownError";
                break;
            default:
                key = "CloudDataUnknownError";
                break;
        }
        this.noSaveDataDialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get(key), 0, 0, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, Vector2.zero, Dialog.CaptionType.Notice);
		ButtonGroupState.DisableAllGroup(true);
		base.Loading = true;
		base.StartCoroutine(this.HideSaveInfoDialog(group));
	}

	private void DisplayNoSaveFoundDialog()
	{
		this.noSaveDataDialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get("NoSaveData"), 0, 0, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, Vector2.zero, Dialog.CaptionType.Notice);
		ButtonGroupState.DisableAllGroup(true);
		base.Loading = true;
		base.StartCoroutine(this.HideSaveInfoDialog(SaveLoadUI.SlotGroupButton));
	}

	private IEnumerator HideSaveInfoDialog(String group)
	{
		yield return new WaitForSeconds(1.5f);
		this.noSaveDataDialog.Hide();
		base.Loading = false;
		ButtonGroupState.ActiveGroup = group;
		yield break;
	}

	private void DisplaySlot()
	{
		Int32 num = 0;
		foreach (UILabel uilabel in this.slotNameLabelList)
		{
			uilabel.text = String.Format(Localization.Get("SlotNo"), num + 1);
			uilabel.color = FF9TextTool.White;
			num++;
		}
		this.currentSlot = FF9StateSystem.Settings.LatestSlot;
		this.helpSlotLabel.text = String.Format(Localization.Get("SlotNo"), this.currentSlot + 1);
		this.SlotListPanel.SetActive(true);
		this.FileListPanel.SetActive(false);
		FF9UIDataTool.DisplayTextLocalize(this.HelpTitleLabel, "SaveHelpSlot");
	}

	private void DisplayOverWriteDialog()
	{
		this.OverWriteDialog.SetActive(true);
	}

	private void OnFinishedLoadPreview(DataSerializerErrorCode errNo, Int32 slotID, List<SharedDataPreviewSlot> data)
	{
		base.StartCoroutine(this.OnFinishedLoadPreview_delay(errNo, slotID, data));
	}

	private IEnumerator OnFinishedLoadPreview_delay(DataSerializerErrorCode errNo, Int32 slotID, List<SharedDataPreviewSlot> data)
	{
		Single remainTime = Mathf.Max(2f - (Time.time - this.timeCounter), 0f);
		yield return new WaitForSeconds(remainTime);
		base.Loading = false;
		if (errNo == DataSerializerErrorCode.Success)
		{
			FF9Sfx.FF9SFX_Play(1044);
			this.LoadingPreviewDialog.SetActive(false);
			if (this.type == SaveLoadUI.SerializeType.Load)
			{
				Boolean isNodata = true;
				foreach (SharedDataPreviewSlot slot in data)
				{
					if (slot != null)
					{
						isNodata = false;
						break;
					}
				}
				if (isNodata)
				{
					this.DisplayNoSaveFoundDialog();
					FF9UIDataTool.DisplayTextLocalize(this.HelpTitleLabel, "SaveHelpSlot");
					yield break;
				}
			}
			else if (data == null)
			{
				data = new List<SharedDataPreviewSlot>();
				for (Int32 index = 0; index < 15; index++)
				{
					data.Add((SharedDataPreviewSlot)null);
				}
			}
			ButtonGroupState.MuteActiveSound = true;
			base.Loading = true;
			base.FadingComponent.FadePingPong(delegate
			{
				this.SlotListPanel.SetActive(false);
				this.FileListPanel.SetActive(true);
				FF9UIDataTool.DisplayTextLocalize(this.HelpTitleLabel, "SaveHelpBlock");
				this.DisplayFile(data);
			}, delegate
			{
				ButtonGroupState.MuteActiveSound = false;
				this.Loading = false;
			});
			yield break;
		}
		FF9Sfx.FF9SFX_Play(1046);
        global::Debug.Log("DISPLAYING CORRUPT DIALOG 1");
        FF9UIDataTool.DisplayTextLocalize(this.HelpTitleLabel, "SaveHelpSlot");
		this.LoadingPreviewDialog.SetActive(false);
		this.DisplayCorruptAccessDialog(SaveLoadUI.SlotGroupButton, SaveLoadUI.SerializeType.Load, errNo);
		this.slotNameLabelList[slotID].color = FF9TextTool.Red;
		yield break;
	}

	private void OnFinishedLoadFile(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID, Boolean isSuccess)
	{
		base.StartCoroutine(this.OnFinishedLoadFile_delay(errNo, slotID, saveID, isSuccess));
	}

	private IEnumerator OnFinishedLoadFile_delay(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID, Boolean isSuccess)
	{
		Single remainTime = Mathf.Max(2f - (Time.time - this.timeCounter), 0f);
		yield return new WaitForSeconds(remainTime);
		this.progressBar.value = 1f;
		yield return new WaitForSeconds(0.1f);
		this.LoadingAccessPanel.SetActive(false);
		if (isSuccess && errNo == DataSerializerErrorCode.Success)
		{
			FF9Sfx.FF9SFX_Play(1261);
			this.Hide(delegate
			{
				SoundLib.StopMovieMusic("FMV000", true);
				this.Loading = true;
				FF9StateSystem.Settings.SetSound();
				FF9StateSystem.Settings.SetSoundEffect();
				EventEngine instance = PersistenSingleton<EventEngine>.Instance;
				instance.ReplaceLoadMap();
				PersistenSingleton<UIManager>.Instance.TitleScene.SplashScreenEnabled = true;
				AchievementManager.ResyncNormalAchievements();
				FF9StateSystem.Settings.UpdateSetting();
				ff9.w_frameNewSession();
				SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.white);
			});
		}
		else
		{
			FF9Sfx.FF9SFX_Play(1046);
			FF9UIDataTool.DisplayTextLocalize(this.HelpTitleLabel, "SaveHelpBlock");
			global::Debug.LogError("Cannot load file");
			this.DisplayCorruptAccessDialog(SaveLoadUI.FileGroupButton, SaveLoadUI.SerializeType.Load, errNo);
			this.isFileCorrupt[saveID] = true;
			this.DisplayFileInfo(this.currentFile, (SharedDataPreviewSlot)null);
		}
		yield break;
	}

	private void OnFinishedSaveFile(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID, Boolean isSuccess, SharedDataPreviewSlot data)
	{
		if (Configuration.SaveFile.SaveOnCloud && isSuccess && errNo == DataSerializerErrorCode.Success)
			FF9StateSystem.Serializer.UploadToCloud(null, OnFinishedUploadToCloud);
		else
			base.StartCoroutine(this.OnFinishedSaveFile_delay(errNo, slotID, saveID, isSuccess, data));
	}

	private void OnFinishedUploadToCloud(DataSerializerErrorCode errNo, Boolean isSuccess, SharedDataPreviewSlot localData, SharedDataPreviewSlot cloudData)
	{
		base.StartCoroutine(this.OnFinishedUploadToCloud_delay(errNo, isSuccess, localData, cloudData));
	}

	private IEnumerator OnFinishedSaveFile_delay(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID, Boolean isSuccess, SharedDataPreviewSlot data)
	{
		Single remainTime = Mathf.Max(2f - (Time.time - this.timeCounter), 0f);
		yield return new WaitForSeconds(remainTime);
		this.progressBar.value = 1f;
		yield return new WaitForSeconds(0.1f);
		base.Loading = false;
		this.LoadingAccessPanel.SetActive(false);
		if (isSuccess && errNo == DataSerializerErrorCode.Success)
		{
			FF9Sfx.FF9SFX_Play(1261);
			this.isFileExistList[this.currentFile] = true;
			this.isFileCorrupt[this.currentFile] = false;
			this.DisplayFileInfo(this.currentFile, data);
			this.DisplaySuccessfulAccessDialog();
		}
		else
		{
			FF9Sfx.FF9SFX_Play(1046);
            global::Debug.Log("DISPLAYING CORRUPT DIALOG 2");
            FF9UIDataTool.DisplayTextLocalize(this.HelpTitleLabel, "SaveHelpBlock");
			global::Debug.LogError("Cannot save file");
			this.DisplayCorruptAccessDialog(SaveLoadUI.FileGroupButton, SaveLoadUI.SerializeType.Save, errNo);
			this.isFileCorrupt[saveID] = true;
		}
		yield break;
	}

	private IEnumerator OnFinishedUploadToCloud_delay(DataSerializerErrorCode errNo, Boolean isSuccess, SharedDataPreviewSlot localData, SharedDataPreviewSlot cloudData)
	{
		Single remainTime = Mathf.Max(2f - (Time.time - this.timeCounter), 0f);
		yield return new WaitForSeconds(remainTime);
		this.progressBar.value = 1f;
		yield return new WaitForSeconds(0.1f);
		base.Loading = false;
		this.LoadingAccessPanel.SetActive(false);
		if (isSuccess && errNo == DataSerializerErrorCode.Success)
			FF9Sfx.FF9SFX_Play(1261);
		else
			FF9Sfx.FF9SFX_Play(1046);
		this.isFileExistList[this.currentFile] = true;
		this.isFileCorrupt[this.currentFile] = false;
		this.DisplayFileInfo(this.currentFile, localData);
		this.DisplaySuccessfulAccessDialog();
		yield break;
	}

	private void setSerialzeType(SaveLoadUI.SerializeType serailizeType)
	{
		if (serailizeType != SaveLoadUI.SerializeType.Save)
		{
			if (serailizeType == SaveLoadUI.SerializeType.Load)
			{
				FF9UIDataTool.DisplayTextLocalize(this.SerailizeTitleLabel, "Load");
			}
		}
		else
		{
			FF9UIDataTool.DisplayTextLocalize(this.SerailizeTitleLabel, "Save");
		}
	}

	private void Awake()
	{
		base.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
		this.helpSlotLabel = this.HelpPanel.GetChild(1).GetChild(0).GetComponent<UILabel>();
		this.successfulAccessGameObject = this.SuccessfulAccessPanel.GetChild(0);
		this.fileScrollList = this.FileListPanel.GetChild(1).GetComponent<SnapDragScrollView>();
		this.progressBar = this.LoadingAccessPanel.GetChild(2).GetComponent<UISlider>();
		this.loadingAccessText = this.LoadingAccessPanel.GetChild(1);
		foreach (Object obj in this.SlotListPanel.transform)
		{
			Transform transform = (Transform)obj;
			Int32 siblingIndex = transform.GetSiblingIndex();
			if (siblingIndex != 10)
			{
				UILabel component = transform.gameObject.GetChild(0).GetComponent<UILabel>();
				this.slotNameLabelList.Add(component);
				this.slotNameButtonList.Add(transform.gameObject.GetComponent<ButtonGroupState>());
				UIEventListener uieventListener = UIEventListener.Get(transform.gameObject);
				uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.onClick));
			}
		}
		foreach (Object obj2 in this.FileListPanel.GetChild(1).GetChild(0).transform)
		{
			Transform transform2 = (Transform)obj2;
			SaveLoadUI.FileInfoHUD item = new SaveLoadUI.FileInfoHUD(transform2.gameObject);
			this.fileInfoHudList.Add(item);
			UIEventListener uieventListener2 = UIEventListener.Get(transform2.gameObject);
			uieventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener2.onClick, new UIEventListener.VoidDelegate(this.onClick));
		}
		this.screenFadePanel = this.ScreenFadeGameObject.GetParent().GetComponent<UIPanel>();
		UIEventListener uieventListener3 = UIEventListener.Get(this.OverWriteDialog.GetChild(1));
		uieventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener3.onClick, new UIEventListener.VoidDelegate(this.onClick));
		UIEventListener uieventListener4 = UIEventListener.Get(this.OverWriteDialog.GetChild(2));
		uieventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener4.onClick, new UIEventListener.VoidDelegate(this.onClick));
	}

	private static String SlotGroupButton = "Save.Slot";

	private static String FileGroupButton = "Save.File";

	private static String ConfirmDialogGroupButton = "Save.Choice";

	public GameObject SerailizeTitleLabel;

	public GameObject HelpTitleLabel;

	public GameObject HelpDespLabelGameObject;

	public GameObject SlotListPanel;

	public GameObject FileListPanel;

	public GameObject HelpPanel;

	public GameObject LoadingPreviewDialog;

	public GameObject OverWriteDialog;

	public GameObject LoadingAccessPanel;

	public GameObject SuccessfulAccessPanel;

	public GameObject ScreenFadeGameObject;

	private UISlider progressBar;

	private SnapDragScrollView fileScrollList;

	private GameObject successfulAccessGameObject;

	private GameObject loadingAccessText;

	private UILabel helpSlotLabel;

	private List<SaveLoadUI.FileInfoHUD> fileInfoHudList = new List<SaveLoadUI.FileInfoHUD>();

	private List<UILabel> slotNameLabelList = new List<UILabel>();

	private List<ButtonGroupState> slotNameButtonList = new List<ButtonGroupState>();

	private UIPanel screenFadePanel;

	private List<Boolean> isFileExistList = new List<Boolean>();

	private List<Boolean> isFileCorrupt = new List<Boolean>();

	private SaveLoadUI.SerializeType type;

	private Int32 currentSlot;

	private Int32 currentFile;

	private Single timeCounter;

	private Dialog noSaveDataDialog;

	private struct FileInfoHUD
	{
		public FileInfoHUD(GameObject go)
		{
			this.Self = go;
			this.Container = go.GetChild(0);
			this.Button = go.GetComponent<ButtonGroupState>();
			this.LeaderNameLabel = go.GetChild(0).GetChild(0).GetComponent<UILabel>();
			this.TimeLabel = go.GetChild(0).GetChild(2).GetComponent<UILabel>();
			this.LeaderLvLabel = go.GetChild(0).GetChild(4).GetComponent<UILabel>();
			this.GilLabel = go.GetChild(0).GetChild(6).GetComponent<UILabel>();
			this.FileNoLabel = go.GetChild(1).GetComponent<UILabel>();
			this.EmptySlotTextGameObject = go.GetChild(2);
			this.EmptySlotTextLabel = go.GetChild(2).GetComponent<UILabel>();
			this.LocationNameLabel = go.GetChild(0).GetChild(8).GetChild(0).GetComponent<UILabel>();
			this.CharacterAvatarList = new UISprite[]
			{
				go.GetChild(0).GetChild(7).GetChild(0).GetComponent<UISprite>(),
				go.GetChild(0).GetChild(7).GetChild(1).GetComponent<UISprite>(),
				go.GetChild(0).GetChild(7).GetChild(2).GetComponent<UISprite>(),
				go.GetChild(0).GetChild(7).GetChild(3).GetComponent<UISprite>()
			};
		}

		public GameObject Self;

		public GameObject Container;

		public ButtonGroupState Button;

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

	public enum SerializeType
	{
		Save,
		Load,
		None
	}
}
