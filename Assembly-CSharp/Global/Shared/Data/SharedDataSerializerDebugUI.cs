using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.Common;
using UnityEngine;

public class SharedDataSerializerDebugUI : MonoBehaviour
{
	private void Start()
	{
		this.scrollPosition = default(Vector2);
		this.message = "Please select LOAD or SAVE";
		this.slotID = 0;
		this.saveID = 0;
		this.previewSlotID = 0;
		this.isLoading = false;
	}

	private void OnGUI()
	{
		Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
		DebugGuiSkin.ApplySkin();
		GUILayout.BeginArea(fullscreenRect);
		if (!this.isShowUI)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("On/Off", new GUILayoutOption[0]))
			{
				this.isShowUI = !this.isShowUI;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Back", new GUILayoutOption[0]))
			{
				SceneDirector.Replace("MainMenu", SceneTransition.FadeOutToBlack_FadeIn, true);
			}
			if (GUILayout.Button("On/Off", new GUILayoutOption[0]))
			{
				this.isShowUI = !this.isShowUI;
			}
			GUILayout.FlexibleSpace();
			this.OnMobileAndWindowsGUI(fullscreenRect);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
	}

	private void OnaaaaGUI(Rect areaRect)
	{
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("LOAD", new GUILayoutOption[0]) && !this.isLoading)
		{
			this.LoadData();
		}
		if (GUILayout.Button("SAVE", new GUILayoutOption[0]) && !this.isLoading)
		{
			this.SaveData();
		}
		if (GUILayout.Button("HAS AUTOLOAD", new GUILayoutOption[0]) && !this.isLoading)
		{
			this.HasAutoLoadData();
		}
		if (GUILayout.Button("AUTOLOAD", new GUILayoutOption[0]) && !this.isLoading)
		{
			this.AutoLoadData();
		}
		if (GUILayout.Button("AUTOSAVE", new GUILayoutOption[0]) && !this.isLoading)
		{
			this.AutoSaveData();
		}
		if (GUILayout.Button("IS GAME FINISH ?", new GUILayoutOption[0]) && !this.isLoading)
		{
			this.IsGameFinish();
		}
		if (GUILayout.Button("FINISH GAME", new GUILayoutOption[0]) && !this.isLoading)
		{
			this.FinishGame();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}

	private void OnMobileAndWindowsGUI(Rect areaRect)
	{
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[]
		{
			GUILayout.Width(areaRect.width * 0.7f),
			GUILayout.Height(areaRect.height * 0.7f)
		});
		GUILayout.BeginVertical("box", new GUILayoutOption[0]);
		GUILayout.Label(this.message, new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal("box", new GUILayoutOption[0]);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("<", new GUILayoutOption[0]))
		{
			this.slotID--;
		}
		GUILayout.Label("Slot ID: " + this.slotID.ToString(), new GUILayoutOption[0]);
		if (GUILayout.Button(">", new GUILayoutOption[0]))
		{
			this.slotID++;
		}
		if (this.slotID < 0)
		{
			this.slotID = 0;
		}
		if (this.slotID > 9)
		{
			this.slotID = 9;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("<", new GUILayoutOption[0]))
		{
			this.saveID--;
		}
		GUILayout.Label("Save ID: " + this.saveID.ToString(), new GUILayoutOption[0]);
		if (GUILayout.Button(">", new GUILayoutOption[0]))
		{
			this.saveID++;
		}
		if (this.saveID < 0)
		{
			this.saveID = 0;
		}
		if (this.saveID > 14)
		{
			this.saveID = 14;
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("LOAD", new GUILayoutOption[0]) && !this.isLoading)
		{
			this.LoadData();
		}
		if (GUILayout.Button("SAVE", new GUILayoutOption[0]) && !this.isLoading)
		{
			this.SaveData();
		}
		if (this.savedPreviewSlot != null)
		{
			GUILayout.Label("---------------BEGIN SLOT-------------", new GUILayoutOption[0]);
			SharedDataPreviewSlot sharedDataPreviewSlot = this.savedPreviewSlot;
			GUILayout.Label("Preview debug data", new GUILayoutOption[0]);
			GUILayout.Label("Gil: " + sharedDataPreviewSlot.Gil, new GUILayoutOption[0]);
			GUILayout.Label("PlayDuration: " + sharedDataPreviewSlot.PlayDuration, new GUILayoutOption[0]);
			GUILayout.Label("Location: " + sharedDataPreviewSlot.Location, new GUILayoutOption[0]);
			if (sharedDataPreviewSlot.CharacterInfoList != null)
			{
				GUILayout.Label("Character Info -------------------", new GUILayoutOption[0]);
				foreach (SharedDataPreviewCharacterInfo sharedDataPreviewCharacterInfo in sharedDataPreviewSlot.CharacterInfoList)
				{
					if (sharedDataPreviewCharacterInfo != null)
					{
						GUILayout.Label("SlotID: " + sharedDataPreviewCharacterInfo.SerialID, new GUILayoutOption[0]);
						GUILayout.Label("Level: " + sharedDataPreviewCharacterInfo.Level, new GUILayoutOption[0]);
						GUILayout.Label("Name: " + sharedDataPreviewCharacterInfo.Name, new GUILayoutOption[0]);
						GUILayout.Label("---------------------------------", new GUILayoutOption[0]);
					}
				}
			}
			GUILayout.Label("---------------END SLOT-------------", new GUILayoutOption[0]);
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.BeginVertical("box", new GUILayoutOption[0]);
		if (GUILayout.Button("HAS AUTOLOAD", new GUILayoutOption[0]))
		{
			this.HasAutoLoadData();
		}
		else if (GUILayout.Button("AUTOLOAD", new GUILayoutOption[0]))
		{
			this.AutoLoadData();
		}
		else if (GUILayout.Button("AUTOSAVE", new GUILayoutOption[0]))
		{
			this.AutoSaveData();
		}
		GUILayout.EndVertical();
		GUILayout.Space(10f);
		GUILayout.BeginVertical("box", new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("<", new GUILayoutOption[0]))
		{
			this.previewSlotID--;
		}
		GUILayout.Label("Slot ID: " + this.previewSlotID.ToString(), new GUILayoutOption[0]);
		if (GUILayout.Button(">", new GUILayoutOption[0]))
		{
			this.previewSlotID++;
		}
		if (this.previewSlotID < 0)
		{
			this.previewSlotID = 0;
		}
		if (this.previewSlotID > 9)
		{
			this.previewSlotID = 9;
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("LOAD PREVIEW", new GUILayoutOption[0]) && !this.isLoading)
		{
			this.LoadPreview(this.previewSlotID);
		}
		if (this.previewSlotList != null)
		{
			GUILayout.Label("---------------BEGIN DATA-------------", new GUILayoutOption[0]);
			for (Int32 i = 0; i < this.previewSlotList.Count; i++)
			{
				SharedDataPreviewSlot sharedDataPreviewSlot2 = this.previewSlotList[i];
				if (sharedDataPreviewSlot2 != null)
				{
					GUILayout.Label("Preview # " + i + " -------------------", new GUILayoutOption[0]);
					GUILayout.Label("Preview debug data", new GUILayoutOption[0]);
					GUILayout.Label("Gil: " + sharedDataPreviewSlot2.Gil, new GUILayoutOption[0]);
					GUILayout.Label("PlayDuration: " + sharedDataPreviewSlot2.PlayDuration, new GUILayoutOption[0]);
					GUILayout.Label("Location: " + sharedDataPreviewSlot2.Location, new GUILayoutOption[0]);
					if (sharedDataPreviewSlot2.CharacterInfoList != null)
					{
						GUILayout.Label("Character Info -------------------", new GUILayoutOption[0]);
						foreach (SharedDataPreviewCharacterInfo sharedDataPreviewCharacterInfo2 in sharedDataPreviewSlot2.CharacterInfoList)
						{
							if (sharedDataPreviewCharacterInfo2 != null)
							{
								GUILayout.Label("SlotID: " + sharedDataPreviewCharacterInfo2.SerialID, new GUILayoutOption[0]);
								GUILayout.Label("Level: " + sharedDataPreviewCharacterInfo2.Level, new GUILayoutOption[0]);
								GUILayout.Label("Name: " + sharedDataPreviewCharacterInfo2.Name, new GUILayoutOption[0]);
								GUILayout.Label("---------------------------------", new GUILayoutOption[0]);
							}
						}
					}
					GUILayout.Label("---------------END SLOT-------------", new GUILayoutOption[0]);
				}
			}
			GUILayout.Label("---------------END DATA-------------", new GUILayoutOption[0]);
		}
		GUILayout.EndVertical();
		GUILayout.Space(10f);
		GUILayout.BeginVertical("box", new GUILayoutOption[0]);
		if (GUILayout.Button("Get latest timestamp", new GUILayoutOption[0]))
		{
			this.GetLatestTimestamp();
		}
		GUILayout.EndVertical();
		GUILayout.Space(10f);
		GUILayout.BeginVertical("box", new GUILayoutOption[0]);
		if (GUILayout.Button("Clear UI", new GUILayoutOption[0]))
		{
			this.ClearUI();
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical("box", new GUILayoutOption[0]);
		if (GUILayout.Button("Is Game Finish ?", new GUILayoutOption[0]))
		{
			this.IsGameFinish();
		}
		if (GUILayout.Button("Finish Game", new GUILayoutOption[0]))
		{
			this.FinishGame();
		}
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}

	private void LoadPreview(Int32 previewSlotID)
	{
		this.isLoading = true;
		FF9StateSystem.Serializer.LoadSlotPreview(previewSlotID, new ISharedDataSerializer.OnLoadSlotFinish(this.LoadSlotPreviewDelgate));
	}

	private void LoadSlotPreviewDelgate(DataSerializerErrorCode errNo, Int32 slotID, List<SharedDataPreviewSlot> data)
	{
		if (data != null)
		{
			this.message = "Loaded. Success. Have fun!";
		}
		else
		{
			this.message = "Fail!";
		}
		this.previewSlotList = data;
		this.isLoading = false;
	}

	private void AutoLoadData()
	{
		FF9StateSystem.Serializer.Autoload(new ISharedDataSerializer.OnAutosaveAutoloadStart(this.AutoloadDataStartDelegate), new ISharedDataSerializer.OnAutoloadFinish(this.AutoloadDataFinishDelegate));
	}

	private void HasAutoLoadData()
	{
		FF9StateSystem.Serializer.HasAutoload(new ISharedDataSerializer.OnHasAutoloadFinish(this.HasAutoloadDataFinishDelegate));
	}

	private void HasAutoloadDataFinishDelegate(DataSerializerErrorCode errNo, Boolean hasAutoloadData)
	{
		OnScreenLog.Add((!hasAutoloadData) ? "No, you haven't" : "Yes, you have");
	}

	private void AutoloadDataStartDelegate(DataSerializerErrorCode errNo)
	{
		this.isLoading = true;
		this.message = "Start autoload";
	}

	private void AutoloadDataFinishDelegate(DataSerializerErrorCode errNo, Boolean isSuccess)
	{
		if (isSuccess)
		{
			OnScreenLog.Add("Autoloaded. Success. Have fun!");
		}
		else
		{
			OnScreenLog.Add("Fail! Data not found?");
		}
		this.isLoading = false;
	}

	private void AutoSaveData()
	{
		FF9StateSystem.Serializer.Autosave(new ISharedDataSerializer.OnAutosaveAutoloadStart(this.AutosaveDataStartDelegate), new ISharedDataSerializer.OnAutosaveFinish(this.AutosaveDataFinishDelegate));
	}

	private void AutosaveDataStartDelegate(DataSerializerErrorCode errNo)
	{
		this.isLoading = true;
		this.message = "Start autosave";
	}

	private void AutosaveDataFinishDelegate(DataSerializerErrorCode errNo, Boolean isSuccess)
	{
		if (isSuccess)
		{
			this.message = "Autosaved. Success. Have fun!";
		}
		else
		{
			this.message = "Fail! Data not found?";
		}
		this.isLoading = false;
	}

	private void LoadData()
	{
		FF9StateSystem.Serializer.Load(this.slotID, this.saveID, new ISharedDataSerializer.OnSaveLoadStart(this.LoadDataStartDelgate), new ISharedDataSerializer.OnLoadFinish(this.LoadDataFinishDelegate));
	}

	private void LoadDataStartDelgate(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID)
	{
		this.isLoading = true;
		this.message = "Start to read data";
	}

	private void LoadDataFinishDelegate(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID, Boolean isSuccess)
	{
		if (isSuccess)
		{
			this.message = "Loaded. Success. Have fun!";
		}
		else
		{
			this.message = "Fail! Data not found?";
		}
		this.isLoading = false;
	}

	private void SaveData()
	{
		FF9StateSystem.Serializer.Save(this.slotID, this.saveID, new ISharedDataSerializer.OnSaveLoadStart(this.SaveDataStartDelegate), new ISharedDataSerializer.OnSaveFinish(this.SaveDataFinishDelegate));
	}

	private void SaveDataStartDelegate(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID)
	{
		this.isLoading = true;
		this.message = "Start to write data";
	}

	private void SaveDataFinishDelegate(DataSerializerErrorCode errNo, Int32 slotID, Int32 saveID, Boolean isSuccess, SharedDataPreviewSlot preview)
	{
		if (isSuccess)
		{
			this.message = "Saved. Success. Have fun!";
		}
		else
		{
			this.message = "Fail. Something wrong?";
		}
		this.isLoading = false;
		this.savedPreviewSlot = preview;
	}

	private void GetLatestTimestamp()
	{
		FF9StateSystem.Serializer.GetLocalSaveLatestTimestamp(delegate(DataSerializerErrorCode errNo, Boolean isSuccess, Double timestamp)
		{
			OnScreenLog.Add("Local save timestamp " + ((!isSuccess) ? " not exist" : (" exist with timestamp: " + timestamp)));
		});
		FF9StateSystem.Serializer.GetCloudSaveLatestTimestamp(delegate(DataSerializerErrorCode errNo, Boolean isSuccess, Double timestamp)
		{
			OnScreenLog.Add("Cloud save timestamp " + ((!isSuccess) ? " not exist" : (" exist with timestamp: " + timestamp)));
		});
	}

	private void IsGameFinish()
	{
		FF9StateSystem.Serializer.GetGameFinishFlag(delegate(DataSerializerErrorCode errNo, Boolean isLoadSuccess, Boolean isFinished)
		{
			if (errNo == DataSerializerErrorCode.Success)
			{
				if (!isLoadSuccess)
				{
					OnScreenLog.Add("Mayday, GetGameFinishFlag loading failure!");
					return;
				}
				OnScreenLog.Add((!isFinished) ? "Game is NOT finished" : "Game is finished");
			}
			else
			{
				OnScreenLog.Add("GetGameFinnish flag has invalid data!");
			}
		});
	}

	private void FinishGame()
	{
		FF9StateSystem.Serializer.SetGameFinishFlagWithTrue(delegate(DataSerializerErrorCode errNo, Boolean isSuccess)
		{
			if (!isSuccess)
			{
				OnScreenLog.Add("May day, SetGameFinishFlagWithTrue operation failure!");
				return;
			}
			OnScreenLog.Add("SetGameFinishFlagWithTrue success");
		});
	}

	private void ClearAllData()
	{
		FF9StateSystem.Serializer.ClearAllData();
		this.ClearUI();
	}

	private void ClearUI()
	{
		this.savedPreviewSlot = (SharedDataPreviewSlot)null;
		this.previewSlotList = null;
	}

	private Vector2 scrollPosition;

	private String message;

	private Boolean isLoading;

	private Int32 slotID;

	private Int32 saveID;

	private Int32 previewSlotID;

	private List<SharedDataPreviewSlot> previewSlotList;

	private SharedDataPreviewSlot savedPreviewSlot;

	private Boolean isShowUI = true;
}
