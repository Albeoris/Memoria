﻿using System;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using UnityEngine;
using Object = System.Object;

public class HonoluluFieldMain : HonoBehavior
{
	public override void HonoAwake()
	{
		EMinigame.InitializeDigUpKupoAchievement();
		this.FF9Sys = PersistenSingleton<FF9StateSystem>.Instance;
		this.FF9 = FF9StateSystem.Common.FF9;
		this.FF9Field = FF9StateSystem.Field.FF9Field;
		this.FF9FieldMap = FF9StateSystem.Field.FF9Field.loc.map;
		this.FF9.fldLocNo = (Int16)EventEngineUtils.eventIDToMESID[(Int32)this.FF9.fldMapNo];
		PersistenSingleton<FF9StateSystem>.Instance.mode = 1;
		String text = FF9DBAll.EventDB[(Int32)this.FF9.fldMapNo];
		FF9StateSystem.Field.SceneName = EventEngineUtils.eventIDToFBGID[(Int32)this.FF9.fldMapNo];
		this.ee = PersistenSingleton<EventEngine>.Instance;
		HonoluluFieldMain.eventEngineRunningCount = 0;
		this.cumulativeTime = 0f;
		this.isInside = false;
		this.stringToEdit = this.FF9.fldMapNo.ToString();
		this.scString = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR).ToString();
		this.mapIDString = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR).ToString();
		this.ff9InitStateFieldLocation((Int32)this.FF9.fldLocNo);
	}

	public override void HonoStart()
	{
	}

	public override void HonoOnDestroy()
	{
	}

	private void shutdownField()
	{
		UInt32 num = this.FF9Sys.attr & 15u;
		if (num != 0u)
		{
			this.ff9ShutdownStateFieldMap();
		}
		num = (this.FF9Sys.attr & 7u);
		if (num != 0u)
		{
			this.ff9ShutdownStateFieldLocation();
		}
		num = (this.FF9Sys.attr & 3u);
		if (num != 0u)
		{
			this.ff9ShutdownStateFieldSystem();
		}
	}

	private void ff9InitStateFieldSystem()
	{
		this.FF9Field.ff9ResetStateFieldSystem();
		this.FF9Sys.attr &= 4294967288u;
		FF9StateSystem.Common.FF9.npcCount = (FF9StateSystem.Common.FF9.npcUsed = 0);
	}

	private void ff9InitStateFieldLocation(Int32 LocNo)
	{
		FF9StateFieldLocation loc = this.FF9Field.loc;
		loc.ff9ResetStateFieldLocation();
		this.FF9.fldLocNo = (Int16)EventEngineUtils.eventIDToMESID[(Int32)this.FF9.fldMapNo];
		base.StartCoroutine(PersistenSingleton<FF9TextTool>.Instance.UpdateFieldText((Int32)this.FF9.fldLocNo));
	}

	private void ff9InitStateFieldMap(Int32 MapNo)
	{
		FF9StateFieldMap map = this.FF9Field.loc.map;
		map.ff9ResetStateFieldMap();
		this.FF9Sys.attr &= 4294966512u;
		this.FF9Field.attr |= 4u;
		FF9StateFieldSystem ff9Field = FF9StateSystem.Field.FF9Field;
		this.FF9Field.playerID = 0;
		map.nextMapNo = (this.FF9.fldMapNo = (Int16)MapNo);
		for (Int32 i = 1; i >= 0; i--)
		{
			for (Int32 j = 3; j >= 0; j--)
			{
				FieldMap.FF9FieldAttr.ff9[i, j] = 0;
				FieldMap.FF9FieldAttr.field[i, j] = 0;
			}
		}
		String text = FF9DBAll.EventDB[MapNo];
		map.evtPtr = EventEngineUtils.loadEventData(text, EventEngineUtils.ebSubFolderField);
		AnimationFactory.LoadAnimationUseInEvent(text);
		vib.LoadVibData(text);
		map.mcfPtr = MapConfiguration.LoadMapConfigData(text);
		GameObject gameObject = GameObject.Find("FieldMap Root");
		GameObject gameObject2 = new GameObject("FieldMap");
		gameObject2.transform.parent = gameObject.transform;
		PersistenSingleton<EventEngine>.Instance.fieldmap = gameObject2.AddComponent<FieldMap>();
		GameObject gameObject3 = new GameObject("FieldMap SPS");
		gameObject3.transform.parent = gameObject.transform;
		PersistenSingleton<EventEngine>.Instance.fieldSps = gameObject3.AddComponent<FieldSPSSystem>();
		PersistenSingleton<EventEngine>.Instance.fieldSps.Init(PersistenSingleton<EventEngine>.Instance.fieldmap);
		if (MapNo >= 3000 && MapNo <= 3012)
		{
			FF9StateSystem.Settings.CallBoosterButtonFuntion(BoosterType.BattleAssistance, false);
			FF9StateSystem.Settings.CallBoosterButtonFuntion(BoosterType.HighSpeedMode, false);
			FF9StateSystem.Settings.CallBoosterButtonFuntion(BoosterType.Attack9999, false);
			FF9StateSystem.Settings.CallBoosterButtonFuntion(BoosterType.NoRandomEncounter, false);
			PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.BattleAssistance, false);
			PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.HighSpeedMode, false);
			PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.Attack9999, false);
			PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.NoRandomEncounter, false);
			PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(false);
			PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.PreEnding);
		}
		else
		{
			PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(true);
		}
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		Int32 suspendSongID = allSoundDispatchPlayer.GetSuspendSongID();
		Int32 sndEffectResSoundID = allSoundDispatchPlayer.GetSndEffectResSoundID(0);
		Int32 sndEffectResSoundID2 = allSoundDispatchPlayer.GetSndEffectResSoundID(1);
		FF9Snd.BGMFieldSongCounter = 0;
		this.ee.StartEvents(map.evtPtr);
		FF9StateSystem.Field.SetTwistAD((Int32)this.ee.GetTwistA(), (Int32)this.ee.GetTwistD());
		PersistenSingleton<EventEngine>.Instance.eTb.InitMessage();
		PersistenSingleton<EventEngine>.Instance.eTb.InitMovieHitPoint(MapNo);
		this.FF9.npcCount = (Byte)this.ee.GetNumberNPC();
		this.fieldmap = GameObject.Find("FieldMap").GetComponent<FieldMap>();
		this.ee.updateModelsToBeAdded();
		Int32 suspendSongID2 = allSoundDispatchPlayer.GetSuspendSongID();
		Int32 sndEffectResSoundID3 = allSoundDispatchPlayer.GetSndEffectResSoundID(0);
		Int32 sndEffectResSoundID4 = allSoundDispatchPlayer.GetSndEffectResSoundID(1);
		if (suspendSongID != -1 && suspendSongID2 != -1 && suspendSongID == suspendSongID2)
		{
			FF9Snd.ff9fldsnd_song_restore();
		}
		else
		{
			SoundLib.GetAllSoundDispatchPlayer().StopAndClearSuspendBGM(-1);
		}
		if (FF9Snd.LatestWorldPlayedSong != -1 && FF9Snd.LatestWorldPlayedSong == SoundLib.GetAllSoundDispatchPlayer().GetCurrentMusicId() && FF9Snd.BGMFieldSongCounter == 0)
		{
			SoundLib.GetAllSoundDispatchPlayer().FF9SOUND_SONG_STOPCURRENT();
		}
		FF9Snd.LatestWorldPlayedSong = -1;
		FF9Snd.BGMFieldSongCounter = 0;
		if ((sndEffectResSoundID != -1 || sndEffectResSoundID2 != -1) && (sndEffectResSoundID3 != -1 || sndEffectResSoundID4 != -1) && sndEffectResSoundID == sndEffectResSoundID3 && sndEffectResSoundID2 == sndEffectResSoundID4)
		{
			FF9Snd.ff9fieldSoundRestoreAllResidentSndEffect();
		}
	}

	public override void HonoUpdate()
	{
		this.frameTime = 1f / (30f * (Single)FF9StateSystem.Settings.FastForwardFactor);
		if ((MBG.Instance.IsPlaying() & 2UL) != 0UL)
		{
			this.frameTime = 1f / (15f * (Single)FF9StateSystem.Settings.FastForwardFactor);
		}
		Single deltaTime = Time.deltaTime;
		Single num = this.cumulativeTime + deltaTime;
		this.cumulativeTime = num;
		if (this.cumulativeTime >= this.frameTime)
		{
			if (!FF9StateSystem.Settings.IsFastForward)
			{
				this.cumulativeTime -= this.frameTime;
			}
			else
			{
				this.cumulativeTime = 0f;
			}
			SceneTransition transition = PersistenSingleton<SceneDirector>.Instance.Transition;
			if (!PersistenSingleton<SceneDirector>.Instance.IsReady || (PersistenSingleton<SceneDirector>.Instance.IsFading && transition != SceneTransition.FadeInFromBlack && transition != SceneTransition.FadeInFromWhite))
			{
				return;
			}
			if (this.firstFrame)
			{
				this.firstFrame = false;
				this.ff9InitStateFieldSystem();
				this.ff9InitStateFieldMap((Int32)this.FF9.fldMapNo);
				UIScene sceneFromState = PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State);
				if (sceneFromState != (UnityEngine.Object)null)
				{
					sceneFromState.Show((UIScene.SceneVoidDelegate)null);
				}
			}
			this.FF9FieldLocationMain((Int32)this.FF9.fldLocNo);
			EMinigame.GetTheaterShipMaquetteAchievement();
		}
	}

	private void FF9FieldLocationMain(Int32 LocNo)
	{
		this.FF9FieldMapMain((Int32)this.FF9.fldMapNo);
	}

	private void FF9FieldMapMain(Int32 MapNo)
	{
		EBin eBin = this.ee.eBin;
		Int32 varManually = eBin.getVarManually(6357);
		if ((this.FF9.attr & 256u) == 0u)
		{
			if (!MBG.IsNull)
			{
				Singleton<fldfmv>.Instance.ff9fieldFMVService();
			}
			if ((this.FF9.attr & 2u) == 0u)
			{
				Int32 varManually2 = eBin.getVarManually(6357);
				if (varManually2 != this.prevPrg)
				{
					this.prevPrg = varManually2;
					if (varManually2 != 1 || this.FF9.fldMapNo != 50)
					{
						if (this.FF9.fldMapNo != 150 || varManually2 != 5)
						{
							if (this.FF9.fldMapNo != 404)
							{
								if (this.FF9.fldMapNo == 404)
								{
								}
							}
						}
					}
				}
				Int32 num = this.ee.ServiceEvents();
				HonoluluFieldMain.eventEngineRunningCount++;
				this.updatePlayerObj();
				switch (num)
				{
				case 3:
					this.FF9Sys.attr |= 8u;
					this.FF9FieldMap.nextMode = 2;
					this.fieldmap.ff9fieldInternalBattleEncountStart();
					this.ee.BackupPosObjData();
					FF9StateSystem.Battle.isDebug = false;
					FF9StateSystem.Battle.mappingBattleIDWithMapList = false;
					NGUIDebug.Clear();
					break;
				case 4:
					if (this.FF9FieldMap.nextMapNo == 16000)
					{
						this.FF9FieldMap.nextMode = 4;
						this.FF9Sys.attr |= 2u;
					}
					else
					{
						this.FF9FieldMap.nextMode = 1;
						this.FF9Sys.attr |= 8u;
					}
					break;
				case 5:
					this.FF9FieldMap.nextMode = 3;
					this.FF9Sys.attr |= 2u;
					break;
				case 7:
					this.FF9FieldMap.nextMode = 9;
					this.FF9Sys.attr |= 2u;
					break;
				case 8:
					this.FF9FieldMap.nextMode = 7;
					this.FF9Sys.attr |= 2u;
					this.FF9.attr |= 2u;
					PersistenSingleton<EventEngine>.Instance.eTb.InitMessage();
					PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(false);
					this.shutdownField();
					EventService.OpenGameOver();
					break;
				}
			}
			if ((this.FF9.attr & 4u) == 0u && this.fieldmap != (UnityEngine.Object)null)
			{
				this.fieldmap.ff9fieldCharService();
			}
			if ((this.FF9.attr & 8u) == 0u && FF9StateSystem.Common.FF9.fldMapNo != 70 && this.fieldmap.walkMesh != null)
			{
				this.fieldmap.walkMesh.BGI_simService();
			}
			if ((this.FF9Field.attr & 16u) == 0u)
			{
				vib.VIB_service();
			}
			if ((this.FF9Field.attr & 8u) == 0u)
			{
				this.fieldmap.rainRenderer.ServiceRain();
			}
			Boolean flag = !MBG.IsNull && MBG.Instance.isFMV55D;
			if ((this.FF9Field.attr & 2048u) == 0u || flag)
			{
				SceneDirector.ServiceFade();
			}
			if ((this.FF9Field.attr & 4u) == 0u)
			{
				this.fieldmap.ff9fieldInternalBattleEncountService();
			}
		}
		this.ff9fieldInternalLoopEnd();
		UInt32 num2 = this.FF9Sys.attr & 15u;
		if (num2 != 0u)
		{
			if (this.ff9fieldDiscCondition())
			{
				this.FF9FieldMap.nextMode = 1;
				this.FF9FieldMap.nextMapNo = (Int16)PersistenSingleton<EventEngine>.Instance.GetFldMapNoAfterChangeDisc();
			}
			this.shutdownField();
			switch (this.FF9FieldMap.nextMode)
			{
			case 1:
				SceneDirector.Replace("FieldMap", SceneTransition.FadeOutToBlack_FadeIn, false);
				break;
			case 2:
				if (FF9StateSystem.Common.FF9.fldMapNo == 1663)
				{
					Int32 varManually3 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
					Int32 varManually4 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
					if (varManually3 == 6950 && varManually4 == 40)
					{
						global::Debug.Log("Force close all dialog for <SQEX> #3105");
						PersistenSingleton<UIManager>.Instance.Dialogs.CloseAll();
					}
				}
				SFX_Rush.SetCenterPosition(0);
				SceneDirector.Replace("BattleMap", SceneTransition.SwirlInBlack, true);
				break;
			case 3:
				SceneDirector.Replace("WorldMap", SceneTransition.FadeOutToBlack_FadeIn, false);
				break;
			case 4:
				SceneDirector.Replace("Ending", SceneTransition.FadeOutToBlack_FadeIn, false);
				break;
			case 9:
				SceneDirector.Replace("QuadMist", SceneTransition.FadeOutToBlack_FadeIn, true);
				break;
			}
		}
	}

	private Boolean ff9fieldDiscCondition()
	{
		return (this.FF9Field.attr & 1048576u) != 0u;
	}

	private void addFieldCharsFromList()
	{
		if (this.ee.requiredAddActor)
		{
			Int32 controlUID = (Int32)this.ee.GetControlUID();
			this.ee.requiredAddActor = false;
			foreach (Int32 num in this.ee.toBeAddedObjUIDList)
			{
				Obj objUID = this.ee.GetObjUID(num);
				Boolean flag = num == controlUID;
				if (flag)
				{
					this.player = objUID.go;
					objUID.go.name = "Player";
				}
				else
				{
					objUID.go.name = "obj" + num;
				}
				this.fieldmap.AddFieldChar(objUID.go, ((PosObj)objUID).posField, ((PosObj)objUID).rotField, flag, (Actor)objUID, false);
			}
		}
	}

	private void updatePlayerObj()
	{
		Obj objUID = this.ee.GetObjUID(250);
		String text = "Player";
		if (objUID != null && objUID.go != (UnityEngine.Object)null && objUID.go.name != text)
		{
			this.player = objUID.go;
			objUID.go.name = text;
			this.fieldmap.updatePlayer(objUID.go);
			((Actor)objUID).fieldMapActorController.LoadResources();
		}
	}

	public override void HonoLateUpdate()
	{
	}

	private void ff9fieldInternalLoopEnd()
	{
		FF9StateGlobal ff = FF9StateSystem.Common.FF9;
		if ((ff.attr & 256u) == 0u)
		{
			FF9FieldAttrState ff9FieldAttr = FieldMap.FF9FieldAttr;
			ff.attr |= (UInt32)ff9FieldAttr.ff9[0, 0];
			ff.attr &= (UInt32)(~(UInt32)ff9FieldAttr.ff9[1, 0]);
			this.FF9Field.attr |= (UInt32)ff9FieldAttr.field[0, 0];
			this.FF9Field.attr &= (UInt32)(~(UInt32)ff9FieldAttr.field[1, 0]);
			fldfmv.ff9fieldFMVAttr |= (Int32)ff9FieldAttr.fmv[0, 0];
			fldfmv.ff9fieldFMVAttr &= (Int32)(~(Int32)ff9FieldAttr.fmv[1, 0]);
			for (Int32 i = 0; i < 2; i++)
			{
				for (Int32 j = 0; j < 3; j++)
				{
					ff9FieldAttr.ff9[i, j] = ff9FieldAttr.ff9[i, j + 1];
					ff9FieldAttr.field[i, j] = ff9FieldAttr.field[i, j + 1];
					ff9FieldAttr.fmv[i, j] = ff9FieldAttr.fmv[i, j + 1];
				}
				ff9FieldAttr.ff9[i, 3] = 0;
				ff9FieldAttr.field[i, 3] = 0;
				ff9FieldAttr.fmv[i, 3] = 0;
			}
		}
	}

	private void ff9ShutdownStateFieldMap()
	{
		FF9StateFieldMap map = FF9StateSystem.Field.FF9Field.loc.map;
		FF9Snd.ff9fieldsound_stopall_mapsndeffect((Int32)this.FF9.fldMapNo);
		EBin eBin = this.ee.eBin;
		Int32 varManually = eBin.getVarManually(6357);
		switch (map.nextMode)
		{
		case 1:
			this.FF9.fldMapNo = map.nextMapNo;
			break;
		case 2:
			this.FF9.btlMapNo = map.nextMapNo;
			FF9StateSystem.Battle.battleMapIndex = (Int32)this.FF9.btlMapNo;
			this.FF9Sys.mode = 2;
			this.FF9Sys.prevMode = 1;
			break;
		case 3:
			this.FF9.wldMapNo = map.nextMapNo;
			this.FF9.wldLocNo = (Int16)EventEngineUtils.eventIDToMESID[(Int32)this.FF9.wldMapNo];
			this.FF9Sys.mode = 3;
			this.FF9Sys.prevMode = 1;
			break;
		case 4:
		{
			AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
			allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP_ALL(null);
			allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_STOPCURRENT();
			allSoundDispatchPlayer.FF9SOUND_STREAM_STOP();
			break;
		}
		case 7:
		{
			AllSoundDispatchPlayer allSoundDispatchPlayer2 = SoundLib.GetAllSoundDispatchPlayer();
			allSoundDispatchPlayer2.FF9SOUND_SNDEFFECT_STOP_ALL(null);
			allSoundDispatchPlayer2.FF9SOUND_SNDEFFECTRES_STOPCURRENT();
			allSoundDispatchPlayer2.FF9SOUND_STREAM_STOP();
			this.FF9Sys.mode = 7;
			this.FF9Sys.prevMode = 1;
			break;
		}
		case 9:
		{
			Int32 currentMusicId = FF9Snd.GetCurrentMusicId();
			if (currentMusicId != -1)
			{
				FF9Snd.ff9fldsnd_song_suspend(currentMusicId);
			}
			FF9Snd.ff9fieldSoundSuspendAllResidentSndEffect();
			AllSoundDispatchPlayer allSoundDispatchPlayer3 = SoundLib.GetAllSoundDispatchPlayer();
			allSoundDispatchPlayer3.FF9SOUND_STREAM_STOP();
			break;
		}
		}
	}

	private void ff9ShutdownStateFieldLocation()
	{
	}

	private void ff9ShutdownStateFieldSystem()
	{
	}

	private void OnGUI()
	{
		if (!EventEngineUtils.showDebugUI)
		{
			return;
		}
		EBin eBin = this.ee.eBin;
		ETb eTb = this.ee.eTb;
	}

	private void changeScenePanel()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("(<)", new GUILayoutOption[0]))
		{
			Int32 num = EventEngine.FindArrayIDFromEventID((Int32)this.FF9.fldMapNo);
			num--;
			if (num < 0)
			{
				num = EventEngine.testEventIDs.GetUpperBound(0);
			}
			this.FF9.fldMapNo = (Int16)EventEngine.testEventIDs[num];
			this.shutdownField();
			SoundLib.StopAllSounds(true);
			SceneDirector.Replace("FieldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		this.stringToEdit = GUILayout.TextField(this.stringToEdit, new GUILayoutOption[]
		{
			GUILayout.Width(90f)
		});
		if (GUILayout.Button("(>)", new GUILayoutOption[0]))
		{
			Int32 num2 = EventEngine.FindArrayIDFromEventID((Int32)this.FF9.fldMapNo);
			num2++;
			if (num2 > EventEngine.testEventIDs.GetUpperBound(0))
			{
				num2 = 0;
			}
			this.FF9.fldMapNo = (Int16)EventEngine.testEventIDs[num2];
			this.shutdownField();
			SoundLib.StopAllSounds(true);
			SceneDirector.Replace("FieldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("Jump", new GUILayoutOption[0]))
		{
			Int32 num3;
			Int32.TryParse(this.stringToEdit, out num3);
			num3 = EventEngine.FindArrayIDFromEventID(num3);
			if (num3 == -1)
			{
				this.stringToEdit = this.FF9.fldMapNo.ToString();
				return;
			}
			if (num3 < 0 || num3 > EventEngine.testEventIDs.GetUpperBound(0))
			{
				num3 = 0;
			}
			this.FF9.fldMapNo = (Int16)EventEngine.testEventIDs[num3];
			this.shutdownField();
			SoundLib.StopAllSounds(true);
			SceneDirector.Replace("FieldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private void jumpingScenePanel()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("(<)", new GUILayoutOption[0]))
		{
			Int32 num = EventEngine.FindArrayIDFromEventID((Int32)this.FF9.fldMapNo);
			num--;
			if (num < 0)
			{
				num = EventEngine.testEventIDs.GetUpperBound(0);
			}
			this.FF9.fldMapNo = (Int16)EventEngine.testEventIDs[num];
			this.shutdownField();
			SoundLib.StopAllSounds(true);
			SceneDirector.FF9Wipe_FadeIn();
			SceneDirector.Replace("FieldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		this.stringToEdit = GUILayout.TextField(this.stringToEdit, new GUILayoutOption[]
		{
			GUILayout.Width(90f)
		});
		if (GUILayout.Button("(>)", new GUILayoutOption[0]))
		{
			Int32 num2 = EventEngine.FindArrayIDFromEventID((Int32)this.FF9.fldMapNo);
			num2++;
			if (num2 > EventEngine.testEventIDs.GetUpperBound(0))
			{
				num2 = 0;
			}
			this.FF9.fldMapNo = (Int16)EventEngine.testEventIDs[num2];
			this.shutdownField();
			SoundLib.StopAllSounds(true);
			SceneDirector.FF9Wipe_FadeIn();
			SceneDirector.Replace("FieldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		if (GUILayout.Button("Jump", new GUILayoutOption[0]))
		{
			Int32 num3;
			Int32.TryParse(this.stringToEdit, out num3);
			num3 = EventEngine.FindArrayIDFromEventID(num3);
			if (num3 == -1)
			{
				this.stringToEdit = this.FF9.fldMapNo.ToString();
				return;
			}
			EBin eBin = PersistenSingleton<EventEngine>.Instance.eBin;
			Int32 num4 = 0;
			if (Int32.TryParse(this.scString, out num4))
			{
				eBin.setVarManually(EBin.SC_COUNTER_SVR, num4);
			}
			Int32 num5;
			if (Int32.TryParse(this.mapIDString, out num5))
			{
				eBin.setVarManually(EBin.MAP_INDEX_SVR, num5);
			}
			if (num3 < 0 || num3 > EventEngine.testEventIDs.GetUpperBound(0))
			{
				num3 = 0;
			}
			this.FF9.fldMapNo = (Int16)EventEngine.testEventIDs[num3];
			this.shutdownField();
			SoundLib.StopAllSounds(true);
			SceneDirector.FF9Wipe_FadeIn();
			SceneDirector.Replace("FieldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
			global::Debug.Log(String.Concat(new Object[]
			{
				"----- Jump to : ",
				this.FF9.fldMapNo,
				"/",
				num4,
				"/",
				num5,
				" -----"
			}));
		}
		GUILayout.FlexibleSpace();
	}

	private void settingScenerioPanel()
	{
		GUILayout.BeginHorizontal("box", new GUILayoutOption[0]);
		GUILayout.Label("SC:", new GUILayoutOption[0]);
		this.scString = GUILayout.TextField(this.scString, new GUILayoutOption[]
		{
			GUILayout.Width(90f)
		});
		GUILayout.BeginHorizontal("box", new GUILayoutOption[0]);
		GUILayout.Label("mapID:", new GUILayoutOption[0]);
		this.mapIDString = GUILayout.TextField(this.mapIDString, new GUILayoutOption[]
		{
			GUILayout.Width(90f)
		});
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
	}

	public static void FadeOutMusic()
	{
		if (FF9Snd.GetCurrentMusicId() != -1)
		{
			FF9Snd.ff9fldsnd_song_vol_intpl(FF9Snd.GetCurrentMusicId(), 30, 0);
		}
	}

	private FieldMap fieldmap;

	private GameObject player;

	private Boolean isInside;

	private FF9StateSystem FF9Sys;

	private FF9StateFieldSystem FF9Field;

	private FF9StateGlobal FF9;

	private FF9StateFieldMap FF9FieldMap;

	private EventEngine ee;

	private EMain eMain;

	private static Int32 eventEngineRunningCount;

	private static Int32 fldMapNoOffset;

	private Int32 prevPrg = -1;

	private Single cumulativeTime;

	private Single frameTime = 0.0333333351f;

	private Single debugUILastTouchTime;

	private Single showHideDebugUICoolDown = 0.5f;

	private Boolean firstFrame = true;

	private String stringToEdit = String.Empty;

	private String scString = String.Empty;

	private String mapIDString = String.Empty;
}
