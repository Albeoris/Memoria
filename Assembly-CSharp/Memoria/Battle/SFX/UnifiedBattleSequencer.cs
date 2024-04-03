using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Assets;
using NCalc;

public static class UnifiedBattleSequencer
{
	public static List<BattleAction> runningActions = new List<BattleAction>();

	public static void EndBattle()
	{
		while (runningActions.Count > 0)
			UnifiedBattleSequencer.ReleaseRunningAction(0);
		BattleAction.bbgIntensity.Clear();
		battlebg.nf_SetBbgIntensity(128);
	}

	public static void Loop()
	{
		if (!Configuration.Battle.SFXRework)
			return;
		try
		{
			SFXChannel.ExecuteLoop();
			for (Int32 i = 0; i < runningActions.Count; i++)
			{
				if (runningActions[i].ExecuteLoop())
				{
					if (FF9StateSystem.Battle.FF9Battle.btl_phase == 4 && runningActions[i].useCameraTarget)
					{
						//SFX.SFX_SendIntData(4, 0, 0, 0);
						btlseq.instance.seq_work_set.CameraNo = 0;
					}
					UnifiedBattleSequencer.ReleaseRunningAction(i);
					i--;
				}
			}
			SFXData.AdvanceEventSFXFrame();
			SequenceBBGIntensity.Apply(true);
		}
		catch (Exception err)
		{
			Log.Error(err);
		}
	}

	public static void LoopRender()
	{
		SFXChannel.Render();
		for (Int32 i = 0; i < runningActions.Count; i++)
			runningActions[i].Render();
		SFXData.RenderEventSFX();
	}

	private static void ReleaseRunningAction(Int32 index)
	{
		foreach (SFXData sfx in runningActions[index].sfxList)
		{
			if (sfx.runningSFX.Count > 0)
			{
				sfx.mesh.End();
				sfx.runningSFX.Clear();
			}
		}
		runningActions[index].cmd = null;
		runningActions.RemoveAt(index);
	}

	public class BattleAction
	{
		public CMD_DATA cmd;
		public Int32 effectNum;
		public EffectType effectType;
		public Int32 frameIndex;
		public List<BattleActionThread> threadList;
		public Int32 reflectStartThreadIndex;
		public Boolean reflectEffectLoaded;
		public Boolean reflectTriggered;
		public Boolean useCameraTarget;
		public UInt16 originalTargetId;

		public List<SFXData> sfxList = new List<SFXData>();
		public List<SequenceMove> move = new List<SequenceMove>();
		public List<SequenceTurn> turn = new List<SequenceTurn>();
		public List<SequenceScale> scale = new List<SequenceScale>();
		public List<SequenceFade> fade = new List<SequenceFade>();
		public Dictionary<BTL_DATA, Btl2dParam> btl2dParam = new Dictionary<BTL_DATA, Btl2dParam>();

		public static List<SequenceBBGIntensity> bbgIntensity = new List<SequenceBBGIntensity>();
		public static SoundProfile hackySoundProfile = null; // TODO: register sounds properly for stop/unload

		private UInt16 animatedChar;
		private UInt16 movingChar;
		private UInt16 turningChar;
		private UInt16 scalingChar;
		private Boolean cancel;

		public BattleAction(EffectType type, Int32 eff)
		{
			effectNum = eff;
			effectType = type;
			switch (effectType)
			{
				case EffectType.EnemySequence:
					threadList = BattleActionThread.LoadFromBtlSeq(FF9StateSystem.Battle.FF9Battle.btl_scene, btlseq.instance, FF9TextTool.BattleText, effectNum);
					break;
				case EffectType.SpecialEffect:
					String path = DataResources.PureDataDirectory + $"SpecialEffects/ef{effectNum:D3}/" + PLAYER_SEQUENCE_FILE;
					String sequenceText = AssetManager.LoadString(path);
					if (sequenceText != null)
					{
						threadList = BattleActionThread.LoadFromTextSequence(sequenceText);
					}
					else
					{
						threadList = new List<BattleActionThread>();
						Log.Warning($"[{nameof(UnifiedBattleSequencer)}] Trying to use the {effectType} {effectNum} ({(SpecialEffect)effectNum}) but {path} is missing");
					}
					break;
			}
		}

		public BattleAction(BTL_SCENE scene, btlseq.btlseqinstance seq, BattleActionThread.TextGetter battleText, Int32 atkNo)
		{
			effectNum = atkNo;
			effectType = EffectType.EnemySequence;
			threadList = BattleActionThread.LoadFromBtlSeq(scene, seq, battleText, effectNum, null, true);
		}

		public BattleAction(String sequenceText)
		{
			effectNum = 0;
			effectType = EffectType.SpecialEffect;
			threadList = BattleActionThread.LoadFromTextSequence(sequenceText);
		}

		public BattleAction(BattleAction basis)
		{
			effectNum = basis.effectNum;
			effectType = basis.effectType;
			threadList = new List<BattleActionThread>(basis.threadList.Count);
			for (Int32 i = 0; i < basis.threadList.Count; i++)
				threadList.Add(new BattleActionThread(basis.threadList[i]));
		}

		public void Execute(CMD_DATA c)
		{
			if (threadList.Count == 0)
				return;
			cmd = c;
			frameIndex = 0;
			threadList[0].active = true;
			threadList[0].targetId = cmd.tar_id;
			reflectStartThreadIndex = threadList.Count;
			threadList.Add(new BattleActionThread(threadList[0], true));
			reflectEffectLoaded = false;
			reflectTriggered = false;
			useCameraTarget = false;
			originalTargetId = cmd.tar_id;
			cancel = false;
			runningActions.Add(this);
		}

		public void Cancel() // Might be used to cancel a command eg. when the caster dies?
		{
			foreach (SFXData sfx in sfxList)
				sfx.Cancel();
			foreach (BattleActionThread th in threadList)
				if (th.waitSFX == 1 || (th.waitSFX >= 1000 && th.waitSFX < 2000))
					th.waitSFX = 0;
			cancel = true;
		}

		public void ExecuteSingleCode(Int32 runningThreadId)
		{
			BattleActionThread runningThread = threadList[runningThreadId];
			BattleActionCode code = runningThread.code.First.Value;
			runningThread.code.RemoveFirst();
			Boolean isSFXThread = runningThread.parentSFX != null;
			Boolean skipNextElse = false;
			if (runningThread.isReflectThread && !isSFXThread && code.operation != "RunThread") // SFX thread code is always used for reflect even if not flagged "Reflect"
			{
				Boolean executeReflect;
				if (!code.TryGetArgBoolean("Reflect", out executeReflect) || !executeReflect)
				{
					runningThread.skipNextElseThread = skipNextElse;
					return;
				}
			}
			Single tmpSingle;
			Boolean tmpBool;
			UInt16 tmpChar;
			UInt16 tmpTarg;
			Int32 tmpInt;
			Int32 tmpInt2;
			String tmpStr;
			Vector3 tmpVec;
			SpecialEffect tmpSfx;
			Int16[] sfxArg;
			switch (code.operation)
			{
				case "Wait":
					code.TryGetArgInt32("Time", out runningThread.waitFrame);
					break;
				case "WaitAnimation":
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					runningThread.waitAnimId |= (UInt16)(tmpChar & animatedChar);
					break;
				case "WaitMove":
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					runningThread.waitMoveId |= (UInt16)(tmpChar & movingChar);
					break;
				case "WaitTurn":
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					runningThread.waitTurnId |= (UInt16)(tmpChar & turningChar);
					break;
				case "WaitSize":
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					runningThread.waitScaleId |= (UInt16)(tmpChar & scalingChar);
					break;
				case "WaitMonsterSFXLoaded":
				case "WaitSFXLoaded":
					if (cancel)
						break;
					if (code.TryGetArgSFXInstance("SFX", "Instance", cmd.regist, sfxList, runningThread.defaultSFXIndex, out tmpInt))
					{
						if (tmpInt < 0)
							runningThread.waitSFX = 1;
						else
							runningThread.waitSFX = 1000 + tmpInt;
						UpdateSFXWait(runningThread);
					}
					break;
				case "WaitMonsterSFXDone":
				case "WaitSFXDone":
					if (code.TryGetArgSFXInstance("SFX", "Instance", cmd.regist, sfxList, runningThread.defaultSFXIndex, out tmpInt))
					{
						if (tmpInt < 0)
							runningThread.waitSFX = 2;
						else
							runningThread.waitSFX = 2000 + tmpInt;
						UpdateSFXWait(runningThread);
					}
					break;
				case "WaitReflect":
					runningThread.waitSFX = 10;
					UpdateSFXWait(runningThread);
					break;
				case "Channel":
					if (cancel)
						break;
					if (!code.argument.TryGetValue("Type", out tmpStr))
					{
						if (cmd.regist.bi.player == 0 || cmd.cmd_no == BattleCommandId.BlueMagic)
							tmpStr = "Enemy";
						else if (cmd.cmd_no == BattleCommandId.BlackMagic || cmd.cmd_no == BattleCommandId.DoubleBlackMagic || cmd.cmd_no == BattleCommandId.MagicSword)
							tmpStr = "Black";
						else if (cmd.cmd_no == BattleCommandId.SummonEiko || cmd.cmd_no == BattleCommandId.SummonGarnet || cmd.cmd_no == BattleCommandId.Phantom || cmd.cmd_no == BattleCommandId.SysPhantom)
							tmpStr = "Summon";
						else
							tmpStr = "Spell";
					}
					if (!code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar))
						tmpChar = cmd.regist.btl_id;
					foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
					{
						Boolean useFluteSound = btl.bi.player != 0 && btl_util.getPlayerPtr(btl).info.serial_no == CharacterSerialNumber.EIKO_FLUTE && (!code.TryGetArgBoolean("SkipFlute", out tmpBool) || !tmpBool);
						SFXChannel.Play(tmpStr, btl, !useFluteSound);
						if (useFluteSound)
						{
							SoundLib.PlaySoundEffect(1507);
							SoundLib.PlaySoundEffect(1508);
							SoundLib.PlaySoundEffect(1501);
						}
					}
					break;
				case "StopChannel":
					if (!code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar))
						tmpChar = cmd.regist.btl_id;
					foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
						SFXChannel.Stop(btl);
					break;
				case "LoadMonsterSFX":
				case "LoadSFX":
					if (cancel)
						break;
					if (code.TryGetArgSFX("SFX", cmd.regist, out tmpSfx))
					{
						//Boolean isMonsterSFX = code.operation == "LoadMonsterSFX";
						SFXData sfxData = new SFXData();
						BTL_VFX_REQ customRequest = new BTL_VFX_REQ();
						BTL_DATA caster = cmd.regist;
						UInt16 mcasterId;
						sfxArg = new Int16[4];
						code.TryGetArgInt32("FirstBone", out tmpInt2);
						sfxArg[0] = (Int16)tmpInt2;
						code.TryGetArgInt32("SecondBone", out tmpInt2);
						sfxArg[1] = (Int16)tmpInt2;
						code.TryGetArgInt32("Args", out tmpInt2);
						sfxArg[2] = (Int16)tmpInt2;
						sfxArg[3] = 1;
						//if ((!code.TryGetArgBoolean("SkipChannel", out tmpBool) && (cmd.cmd_no == BattleCommandId.SysPhantom || cmd.cmd_no == BattleCommandId.SysLastPhoenix)) || tmpBool || runningThread.isReflectThread)
						//	sfxArg[3] |= 1;
						//if ((!code.TryGetArgBoolean("IsItem", out tmpBool) && (cmd.cmd_no == BattleCommandId.Item || cmd.cmd_no == BattleCommandId.AutoPotion)) || tmpBool)
						//	sfxArg[3] |= 2;
						//if (code.TryGetArgBoolean("ForcePlayerCast", out tmpBool) && tmpBool)
						//	sfxArg[3] |= 8;
						//if ((code.TryGetArgBoolean("IsFullReflect", out tmpBool) && tmpBool) || runningThread.isReflectThread)
						//	sfxArg[3] |= 16;
						if (code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar))
						{
							List<BTL_DATA> btlList = btl_util.findAllBtlData(tmpChar);
							if (btlList.Count > 0)
								caster = btlList[Comn.random8() % btlList.Count];
						}
						if (!code.TryGetArgCharacter("Target", cmd.regist.btl_id, runningThread.targetId, out tmpChar))
							tmpChar = runningThread.targetId;
						if (code.TryGetArgCharacter("MagicCaster", cmd.regist.btl_id, runningThread.targetId, out mcasterId))
							sfxArg[3] |= 4;
						customRequest.SetupVfxRequest(cmd, sfxArg, caster, tmpChar);
						if (mcasterId != 0)
							customRequest.mexe = btl_scrp.FindBattleUnit(mcasterId).Data;
						if (code.TryGetArgVector("TargetPosition", out tmpVec))
						{
							customRequest.trgcpos.vx = (Int32)tmpVec.x;
							customRequest.trgcpos.vy = (Int32)tmpVec.y;
							customRequest.trgcpos.vz = (Int32)tmpVec.z;
							customRequest.useTargetAveragePosition = false;
						}
						if (!code.TryGetArgBoolean("UseCamera", out tmpBool))
							tmpBool = Configuration.Battle.Speed < 3 || FF9StateSystem.Battle.FF9Battle.btl_phase != 4 || !UIManager.Battle.FF9BMenu_IsEnable();
						if (SFXData.IsShortSpecialEffect(tmpSfx))
							tmpBool = false;
						else if (useCameraTarget)
							tmpBool = true;
						sfxData.LoadSFX(tmpSfx, cmd, customRequest, tmpBool);
						runningThread.defaultSFXIndex = sfxList.Count;
						sfxList.Add(sfxData);
					}
					break;
				case "PlayMonsterSFX":
				case "PlaySFX":
					if (cancel)
						break;
					if (code.TryGetArgSFXInstance("SFX", "Instance", cmd.regist, sfxList, runningThread.defaultSFXIndex, out tmpInt))
						foreach (SFXData sfx in (tmpInt < 0 ? sfxList.ToArray() : new SFXData[] { sfxList[tmpInt] }))
						{
							List<UInt32> meshKeyList, meshIndexList;
							Dictionary<UInt32, Color> meshKeyColors, meshIndexColors;
							Int32 frameStart = 0;
							if (code.TryGetArgInt32("JumpToFrame", out tmpInt))
								frameStart += tmpInt;
							code.TryGetArgMeshList("HideMeshes", out meshKeyList, out meshIndexList);
							code.TryGetArgMeshColors("MeshColors", out meshKeyColors, out meshIndexColors);
							sfx.PlaySFX(frameStart, meshKeyList, meshIndexList, meshKeyColors, meshIndexColors);
							if (cancel)
								continue;
							if (sfx.sfxthread.Count > 0 && (!code.TryGetArgBoolean("SkipSequence", out tmpBool) || !tmpBool))
							{
								BattleActionThread copy = new BattleActionThread(sfx.sfxthread[0], runningThread.isReflectThread);
								copy.SkipFrames(tmpInt);
								copy.targetId = runningThread.targetId;
								copy.active = true;
								threadList.Add(copy);
							}
						}
					break;
				case "Turn":
					Single tmpBaseAngle;
					Boolean hasSingleAngle = code.TryGetArgSingle("Angle", out tmpSingle);
					Boolean hasVectorAngle = code.TryGetArgVector("Angle", out tmpVec);
					code.TryGetArgBoolean("UsePitch", out tmpBool);
					code.TryGetArgInt32("Time", out tmpInt);
					if (!hasSingleAngle && !hasVectorAngle)
						hasSingleAngle = true;
					else if (hasSingleAngle && hasVectorAngle)
						hasVectorAngle = false;
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
					{
						if (btl.bi.slave != 0)
							continue;
						Vector3 destAngle = tmpVec;
						code.TryGetArgBaseAngle("BaseAngle", btl, cmd.regist.btl_id, runningThread.targetId, tmpBool, out tmpBaseAngle, out tmpTarg);
						if (hasSingleAngle && !hasVectorAngle)
						{
							destAngle = btl.rot.eulerAngles;
							destAngle[tmpBool ? 0 : 1] = tmpBaseAngle + tmpSingle;
						}
						else if (hasVectorAngle)
						{
							destAngle[tmpBool ? 0 : 1] += tmpBaseAngle;
						}
						if (tmpTarg != btl.btl_id)
						{
							SequenceTurn seqt = new SequenceTurn(btl, tmpTarg, destAngle, tmpInt);
							if (tmpInt == 0)
							{
								seqt.frameCur = seqt.frameEnd;
								seqt.Apply();
							}
							else
							{
								turn.Add(seqt);
								turningChar |= btl.btl_id;
							}
						}
					}
					break;
				case "PlayAnimation":
					Boolean palindrome, hold;
					Boolean setFrame = code.TryGetArgInt32("Frame", out tmpInt);
					code.TryGetArgBoolean("Loop", out tmpBool);
					code.TryGetArgBoolean("Palindrome", out palindrome);
					code.TryGetArgBoolean("Hold", out hold);
					if (!code.TryGetArgSingle("Speed", out tmpSingle))
						tmpSingle = 1f;
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
					{
						if (btl.bi.slave != 0)
							continue;
						Boolean releaseCmdIdle = btl == cmd.regist && code.argument.TryGetValue("Anim", out tmpStr) && tmpStr == "Idle";
						Boolean changeAnim = false;
						if (releaseCmdIdle)
						{
							animatedChar |= btl.btl_id;
							if (cmd.info.cmd_motion)
							{
								cmd.info.cmd_motion = false;
								btl_mot.EndCommandMotion(cmd);
							}
							btl_mot.SetDefaultIdle(cmd.regist);
						}
						else
						{
							changeAnim = code.TryGetArgAnimation("Anim", btl, out tmpStr);
							if (changeAnim)
								btl_mot.setMotion(btl, tmpStr);
						}
						if (changeAnim || setFrame)
						{
							animatedChar |= btl.btl_id;
							if (tmpInt < 0)
								btl.evt.animFrame = (Byte)Math.Max(0, GeoAnim.geoAnimGetNumFrames(btl) + tmpInt);
							else
								btl.evt.animFrame = (Byte)Math.Min(GeoAnim.geoAnimGetNumFrames(btl), tmpInt);
						}
						if (tmpSingle == 0f)
							btl.animFlag |= (UInt16)EventEngine.afFreeze;
						else
							btl.animFlag &= (UInt16)~EventEngine.afFreeze;
						if (tmpBool)
							btl.animFlag |= (UInt16)EventEngine.afLoop;
						else
							btl.animFlag &= (UInt16)~EventEngine.afLoop;
						if (palindrome)
							btl.animFlag |= (UInt16)EventEngine.afPalindrome;
						else
							btl.animFlag &= (UInt16)~EventEngine.afPalindrome;
						if (hold)
							btl.animFlag |= (UInt16)EventEngine.afHold;
						else
							btl.animFlag &= (UInt16)~EventEngine.afHold;
						btl.animSpeed = tmpSingle != 0f ? tmpSingle : 1f;
					}
					break;
				case "PlayTextureAnimation":
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					code.TryGetArgInt32("Anim", out tmpInt);
					code.TryGetArgBoolean("Stop", out tmpBool);
					if (tmpBool)
					{
						foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
							GeoTexAnim.geoTexAnimStop(btl.texanimptr, tmpInt);
						break;
					}
					code.TryGetArgBoolean("Once", out tmpBool);
					if (tmpBool)
						foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
							GeoTexAnim.geoTexAnimPlayOnce(btl.texanimptr, tmpInt);
					else
						foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
							GeoTexAnim.geoTexAnimPlay(btl.texanimptr, tmpInt);
					break;
				case "ToggleStandAnimation":
					code.TryGetArgBoolean("Alternate", out tmpBool);
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
						btl_mot.ToggleIdleAnimation(btl, tmpBool);
					break;
				case "MoveToTarget":
					Boolean relativeDistance, useTargetRadius;
					code.TryGetArgVector("Offset", out tmpVec);
					code.TryGetArgSingle("Distance", out tmpSingle);
					code.TryGetArgInt32("Time", out tmpInt);
					code.TryGetArgBoolean("MoveHeight", out tmpBool);
					code.TryGetArgBoolean("IsRelativeDistance", out relativeDistance);
					code.TryGetArgBoolean("UseCollisionRadius", out useTargetRadius);
					code.TryGetArgCharacter("Target", cmd.regist.btl_id, runningThread.targetId, out tmpTarg);
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
					{
						if (btl.bi.slave != 0)
							continue;
						if (code.TryGetArgAnimation("Anim", btl, out tmpStr))
						{
							tmpInt = GeoAnim.getAnimationLoopFrame(btl, tmpStr);
							btl_mot.setMotion(btl, tmpStr);
							btl.evt.animFrame = 0;
						}
						SequenceMove seqm = new SequenceMove(btl, tmpTarg, tmpVec, tmpSingle, tmpInt, tmpBool, relativeDistance, useTargetRadius);
						if (tmpInt == 0)
						{
							seqm.frameCur = seqm.frameEnd;
							seqm.Apply();
						}
						else
						{
							move.Add(seqm);
							movingChar |= btl.btl_id;
						}
					}
					break;
				case "MoveToPosition":
					Vector3 absPos = default(Vector3), dest;
					Boolean useDefaultPos = code.argument.TryGetValue("AbsolutePosition", out tmpStr) && tmpStr == "Default";
					Boolean useAbsPos = useDefaultPos || code.TryGetArgVector("AbsolutePosition", out absPos);
					code.TryGetArgInt32("Time", out tmpInt);
					code.TryGetArgBoolean("MoveHeight", out tmpBool);
					code.TryGetArgVector("RelativePosition", out tmpVec);
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
					{
						if (btl.bi.slave != 0)
							continue;
						if (useDefaultPos)
							dest = btl.base_pos + tmpVec;
						else if (useAbsPos)
							dest = absPos + tmpVec;
						else
							dest = btl.pos + tmpVec;
						if (code.TryGetArgAnimation("Anim", btl, out tmpStr))
						{
							tmpInt = GeoAnim.getAnimationLoopFrame(btl, tmpStr);
							btl_mot.setMotion(btl, tmpStr);
							btl.evt.animFrame = 0;
						}
						SequenceMove seqm = new SequenceMove(btl, 0, dest, 0f, tmpInt, tmpBool);
						if (tmpInt == 0)
						{
							seqm.frameCur = seqm.frameEnd;
							seqm.Apply();
						}
						else
						{
							move.Add(seqm);
							movingChar |= btl.btl_id;
						}
					}
					break;
				case "MOVE_WATER":
					// TODO: Maybe improve "MoveToPosition" etc... to allow formulas for destination position
					if (!code.argument.TryGetValue("Type", out tmpStr))
						tmpStr = "Single";
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					code.TryGetArgInt32("Time", out tmpInt);
					foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
					{
						if (btl.bi.slave != 0)
							continue;
						if (tmpStr == "Single")
						{
							if (btl.radius_collision < 180)
								tmpVec = btl.pos + new Vector3(0, 145, 0);
							else if (btl.radius_collision <= 1150)
								tmpVec = btl.pos + new Vector3(0, btl.radius_collision * 106 / 225, 0);
							else
								tmpVec = btl.pos + new Vector3(0, 500, 0);
						}
						else if (tmpStr == "Sword")
						{
							if (btl.radius_collision < 180)
								tmpVec = btl.pos + new Vector3(0, 200, 0);
							else if (btl.radius_collision <= 650)
								tmpVec = btl.pos + new Vector3(0, btl.radius_collision * 106 / 225 + 50, 0);
							else if (btl.radius_collision <= 1150)
								tmpVec = btl.pos + new Vector3(0, btl.radius_collision * 106 / 225 + 160, 0);
							else
								tmpVec = btl.pos + new Vector3(0, 650, 0);
						}
						else if (tmpStr == "Multi")
						{
							tmpVec = btl.pos + new Vector3(0, 460, 0);
						}
						else
						{
							tmpVec = btl.pos;
						}
						SequenceMove seqm = new SequenceMove(btl, 0, tmpVec, 0f, tmpInt, true);
						if (tmpInt == 0)
						{
							seqm.frameCur = seqm.frameEnd;
							seqm.Apply();
						}
						else
						{
							move.Add(seqm);
							movingChar |= btl.btl_id;
						}
					}
					break;
				case "ChangeSize":
					Boolean isRelative;
					code.TryGetArgInt32("Time", out tmpInt);
					code.TryGetArgVector("Size", out tmpVec);
					if (!code.argument.TryGetValue("Size", out tmpStr))
						tmpStr = "Reset";
					if (tmpStr == "Reset")
						tmpVec = new Vector3(1f, 1f, 1f);
					code.TryGetArgBoolean("ScaleShadow", out tmpBool);
					code.TryGetArgBoolean("IsRelative", out isRelative);
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
					{
						if (btl.bi.slave != 0)
							continue;
						Vector3 scaleVec = isRelative ?
							4096f * new Vector3(btl.gameObject.transform.localScale.x * tmpVec.x, btl.gameObject.transform.localScale.y * tmpVec.y, btl.gameObject.transform.localScale.z * tmpVec.z) :
							btl.geo_scale_default * tmpVec;
						if (tmpInt == 0)
						{
							geo.geoScaleSetXYZ(btl, (Int32)scaleVec.x, (Int32)scaleVec.y, (Int32)scaleVec.z, tmpBool);
						}
						else
						{
							scale.Add(new SequenceScale(btl, scaleVec, tmpBool, tmpInt));
							scalingChar |= btl.btl_id;
						}
					}
					break;
				case "ShowMesh":
					Boolean disappear, permanent;
					code.TryGetArgInt32("Time", out tmpInt);
					code.TryGetArgBoolean("Enable", out tmpBool);
					code.TryGetArgBoolean("IsDisappear", out disappear);
					code.TryGetArgBoolean("IsPermanent", out permanent);
					if (code.argument.ContainsKey("Mesh"))
					{
						code.TryGetArgInt32("Mesh", out tmpInt2);
						code.argument.TryGetValue("Mesh", out tmpStr);
					}
					else
					{
						tmpInt2 = UInt16.MaxValue;
						tmpStr = "All";
					}
					if (tmpStr == "All")
						tmpInt2 = UInt16.MaxValue;
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					Boolean includeWeapon = tmpStr == "Weapon" || tmpInt2 == UInt16.MaxValue;
					Boolean includeShadow = tmpStr == "Shadow" || tmpInt2 == UInt16.MaxValue;
					Boolean isVanish = tmpStr == "Vanish";
					if (tmpStr == "Main")
						tmpInt2 = UInt16.MaxValue;
					Int32 priority;
					if (!code.TryGetArgInt32("Priority", out priority))
						priority = 1;
					foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
					{
						Int32 meshList = tmpInt2;
						if (btl.bi.slave != 0)
							continue;
						if (isVanish)
							meshList = btl.mesh_banish;
						if (btl_stat.CheckStatus(btl, BattleStatus.Vanish))
							meshList &= ~btl.mesh_banish;
						if (!permanent)
							meshList &= ~btl.mesh_current;
						if (tmpInt > 0)
						{
							if (disappear && tmpBool)
								btl.SetDisappear(false, (Byte)priority);
							if (tmpBool)
							{
								if (permanent)
								{
									btl.meshflags &= (UInt32)~meshList;
									btl.mesh_current = (UInt16)(btl.mesh_current & (UInt16)~meshList);
								}
								btl_mot.ShowMesh(btl, (UInt16)meshList, isVanish);
								if (includeWeapon)
									for (Int32 i = 0; i < btl.weaponMeshCount; i++)
										geo.geoWeaponMeshShow(btl, i);
							}
							fade.Add(new SequenceFade(btl, (UInt16)meshList, disappear && !tmpBool, permanent, includeWeapon, includeShadow, tmpBool ? 0f : 128f, tmpBool ? 128f : 0f, tmpInt, (Byte)priority));
						}
						else
						{
							if (disappear)
								btl.SetDisappear(!tmpBool, (Byte)priority);
							if (tmpBool)
							{
								if (permanent)
								{
									btl.meshflags &= (UInt32)~meshList;
									btl.mesh_current = (UInt16)(btl.mesh_current & (UInt16)~meshList);
								}
								btl_mot.ShowMesh(btl, (UInt16)meshList, isVanish);
								if (includeWeapon)
									for (Int32 i = 0; i < btl.weaponMeshCount; i++)
										geo.geoWeaponMeshShow(btl, i);
							}
							else if (!disappear)
							{
								if (permanent)
								{
									btl.meshflags |= (UInt32)meshList;
									btl.mesh_current = (UInt16)(btl.mesh_current | meshList);
								}
								btl_mot.HideMesh(btl, (UInt16)meshList, isVanish);
								if (includeWeapon)
									for (Int32 i = 0; i < btl.weaponMeshCount; i++)
										geo.geoWeaponMeshHide(btl, i);
							}
						}
					}
					break;
				case "ShowShadow":
					code.TryGetArgBoolean("Enable", out tmpBool);
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
					{
						if (btl.bi.slave != 0)
							continue;
						btl.bi.shadow = tmpBool ? (Byte)1 : (Byte)0;
						btl.getShadow().SetActive(tmpBool);
					}
					break;
				case "ChangeCharacterProperty":
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
						code.TryChangeCharacterProperty("Property", "Value", btl);
					break;
				case "PlayCamera":
					if (cancel)
						break;
					if (Configuration.Battle.Speed >= 3 && FF9StateSystem.Battle.FF9Battle.btl_phase == 4)
						break;
					code.TryGetArgBoolean("Alternate", out tmpBool);
					if (tmpBool && !SFX.ShouldPlayAlternateCamera(cmd))
						break;
					code.TryGetArgCamera("Camera", out tmpInt);
					code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar);
					if (!code.TryGetArgInt32("Start", out tmpInt2))
						tmpInt2 = 1;
					//SFX.SetEffCamTrigger();
					if (tmpChar != 0)
					{
						Int16 px, pz;
						btlseq.SeqSubTargetAveragePos(tmpChar, out px, out pz);
						Vector3 trgCPos = new Vector3(px, 0f, pz);
						BTL_DATA camTrg = btlseq.SeqSubGetTarget(tmpChar);
						SFX.SetCameraTarget(trgCPos, cmd.regist, camTrg);
						btlseq.instance.seq_work_set.CameraNo = (Byte)tmpInt;
						if (!tmpBool || cmd.aa.Info.DefaultCamera)
							SFX.SetEnemyCamera(cmd.regist); // DEBUG: in order to avoid getting cameras stuck in some situations, don't call the optional enemy cameras
						useCameraTarget = true;
					}
					else
					{
						btlseq.instance.seq_work_set.CameraNo = (Byte)tmpInt;
					}
					break;
				case "ResetCamera":
					SFX.ResetViewPort();
					break;
				case "PlaySound":
					// TODO: add a starting frame argument
					SoundProfileType soundType;
					String soundAsName;
					Single pitch;
					Single panning;
					code.argument.TryGetValue("Sound", out soundAsName);
					code.TryGetArgSound("Sound", cmd.regist, out tmpInt);
					code.TryGetArgSoundType("SoundType", out soundType);
					if (!code.TryGetArgSingle("Volume", out tmpSingle))
						tmpSingle = 1f;
					code.TryGetArgSingle("Panning", out panning);
					if (!code.TryGetArgSingle("Pitch", out pitch))
						pitch = 1f;
					switch (soundType)
					{
						case SoundProfileType.Default: // TODO (properly)
							try
							{
								if (hackySoundProfile != null)
								{
									SoundPlayer.StaticUnregisterBank(hackySoundProfile);
								}
								hackySoundProfile = new SoundProfile
								{
									Code = soundAsName,
									Name = soundAsName,
									SoundIndex = 0,
									ResourceID = soundAsName,
									SoundProfileType = soundType,
									SoundVolume = tmpSingle,
									Panning = panning,
									Pitch = pitch
								};
								SoundLoaderProxy.Instance.Load(hackySoundProfile,
									(pr, db) =>
									{
										if (pr == null)
											throw new NullReferenceException();
										SoundPlayer.StaticCreateSound(pr);
										SoundPlayer.StaticStartSound(pr, SoundLib.SoundEffectPlayer.Volume);
									}, null);
							}
							catch (Exception err)
							{
								Log.Error(err, $"[{nameof(BattleAction)}] Failed to play custom sound {soundAsName}");
							}
							break;
						case SoundProfileType.Music:
							SoundLib.PlayMusic(tmpInt);
							break;
						case SoundProfileType.SoundEffect:
							SoundLib.PlaySoundEffect(tmpInt, tmpSingle, panning, pitch);
							break;
						case SoundProfileType.MovieAudio:
							SoundLib.PlayMovieMusic(soundAsName);
							break;
						case SoundProfileType.Song:
							SoundLib.PlaySong(tmpInt, tmpSingle, panning, pitch);
							break;
						case SoundProfileType.Sfx:
							//SoundLib.PlaySfxSound(tmpInt, tmpSingle, panning, pitch);
							break;
						case SoundProfileType.Voice:
							if (!Configuration.VoiceActing.Enabled)
								break;
							if (soundAsName.Contains(":"))
							{
								Int32 sepPos = soundAsName.IndexOf(':');
								Int32 messId;
								Int32 battleId;
								if (Int32.TryParse(soundAsName.Substring(0, sepPos), out battleId) && Int32.TryParse(soundAsName.Substring(sepPos + 1), out messId))
									VoicePlayer.PlayBattleVoice(messId, FF9TextTool.BattleText(messId), false, battleId);
							}
							else
							{
								VoicePlayer.PlayBattleVoice(tmpInt, FF9TextTool.BattleFollowText(tmpInt), true);
							}
							break;
					}
					break;
				case "StopSound":
					code.argument.TryGetValue("Sound", out soundAsName);
					code.TryGetArgSound("Sound", cmd.regist, out tmpInt);
					code.TryGetArgSoundType("SoundType", out soundType);
					switch (soundType)
					{
						case SoundProfileType.Default:
							if (hackySoundProfile != null)
								SoundPlayer.StaticStopSound(hackySoundProfile);
							break;
						case SoundProfileType.Music:
							code.TryGetArgInt32("FadeOut", out tmpInt);
							SoundLib.StopMusic(tmpInt);
							break;
						case SoundProfileType.SoundEffect:
							SoundLib.StopSoundEffect(tmpInt);
							break;
						case SoundProfileType.MovieAudio:
							SoundLib.StopMovieMusic(soundAsName);
							break;
						case SoundProfileType.Song:
							SoundLib.StopSong(tmpInt);
							break;
						case SoundProfileType.Sfx:
							//SoundLib.StopSfxSound(tmpInt);
							break;
					}
					break;
				case "EffectPoint":
					if (cancel)
						break;
					if (!code.argument.TryGetValue("Type", out tmpStr))
						tmpStr = "Both";
					if (!code.TryGetArgCharacter("Char", cmd.regist.btl_id, runningThread.targetId, out tmpChar))
						tmpChar = runningThread.targetId;
                    if (tmpStr == "Effect" || tmpStr == "Both")
					{
						cmd.info.effect_counter++;
						foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
						{
							if (btl.bi.target == 0 && cmd.cmd_no != BattleCommandId.Jump && cmd.cmd_no != BattleCommandId.Jump2 && cmd.cmd_no != BattleCommandId.SysEscape && cmd.cmd_no != BattleCommandId.SysTrans)
								continue;
							List<BTL_DATA> allBtl = btl_util.findAllBtlData(0xFF);
							foreach (BTL_DATA c in allBtl)
							{
								c.fig_info = 0;
								c.fig = c.m_fig = 0;
							}
							btl_cmd.ExecVfxCommand(btl, cmd, runningThread);
							foreach (BTL_DATA c in allBtl)
							{
								if (c.fig_info != 0)
								{
									Btl2dParam figParam;
									if (!btl2dParam.TryGetValue(c, out figParam))
										figParam = new Btl2dParam();
									figParam.info |= c.fig_info;
									figParam.hp += c.fig;
									figParam.mp += c.m_fig;
									btl2dParam[c] = figParam;
								}
								c.fig_info = 0;
								c.fig = c.m_fig = 0;
							}
						}
					}
					if (tmpStr == "Figure" || tmpStr == "Both")
					{
						foreach (BTL_DATA btl in btl_util.findAllBtlData(tmpChar))
						{
							Btl2dParam figParam;
							if (!btl2dParam.TryGetValue(btl, out figParam))
								continue;
							btl2d.Btl2dReq(btl, ref figParam.info, ref figParam.hp, ref figParam.mp);
							btl2dParam.Remove(btl);
						}
					}
					break;
				case "Message":
					code.TryGetArgBoolean("Title", out tmpBool);
					if (!code.TryGetArgInt32("Priority", out tmpInt))
						tmpInt = 3;
					if (code.TryGetArgMessage("Text", cmd, out tmpStr) && tmpStr.Length > 0)
					{
						if (tmpBool)
							UIManager.Battle.SetBattleTitle(cmd, tmpStr, (Byte)tmpInt);
						else
							UIManager.Battle.SetBattleMessage(tmpStr, (Byte)tmpInt);
					}
					break;
				case "SetBackgroundIntensity":
					if (!code.TryGetArgSingle("Intensity", out tmpSingle))
						tmpSingle = 1f;
					if (code.TryGetArgInt32("Time", out tmpInt) && tmpInt == 0)
						tmpInt = 1;
					code.TryGetArgInt32("HoldDuration", out tmpInt2);
					SequenceBBGIntensity seqbbg = new SequenceBBGIntensity((Int32)(tmpSingle * 128), tmpInt, tmpInt2 >= 0 ? tmpInt2 - tmpInt : -1);
					BattleAction.bbgIntensity.Add(seqbbg);
					if (tmpInt == 0)
					{
						seqbbg.frameCur = seqbbg.frameEnd;
						SequenceBBGIntensity.Apply(false);
					}
					break;
				case "SetVariable":
					code.TrySetVariable("Variable", "Value", "Index");
					break;
				case "SetupReflect":
					if (cancel)
						break;
					if (!cmd.info.HasCheckedReflect)
					{
						code.argument.TryGetValue("Delay", out tmpStr);
						if (tmpStr == "SFXLoaded")
							tmpInt = 30; // TODO
						else if (tmpStr == "SFXPlay")
							tmpInt = 30;
						else if (tmpStr == "SFXDone")
							tmpInt = 30;
						else
							code.TryGetArgInt32("Delay", out tmpInt);
						UInt16 reflectingBtl = btl_cmd.CheckReflec(cmd);
						runningThread.targetId = cmd.tar_id;
						SFXChannel.PlayReflectEffect(reflectingBtl, tmpInt);
					}
					break;
				case "ActivateReflect":
					if (cancel)
						break;
					ActivateReflect();
					break;
				case "RunThread":
					if (code.TryGetArgBoolean("AsElseThread", out tmpBool) && tmpBool && runningThread.skipNextElseThread)
					{
						skipNextElse = true;
						break;
					}
					if (code.TryGetArgInt32("Thread", out tmpInt) && tmpInt >= 0)
					{
						Boolean dontCopyThread;
						code.TryGetArgBoolean("DoNotCopyThread", out dontCopyThread);
						Boolean useMainThreadList = dontCopyThread || !isSFXThread;
						if (useMainThreadList && tmpInt >= threadList.Count)
							break;
						if (!useMainThreadList && tmpInt >= runningThread.parentSFX.sfxthread.Count)
							break;
						if (code.argument.TryGetValue("Condition", out tmpStr))
						{
							Expression c = new Expression(tmpStr);
							BattleUnit caster = new BattleUnit(cmd.regist);
							BattleUnit target = btl_scrp.FindBattleUnitUnlimited((UInt16)Comn.firstBitSet(runningThread.targetId));
							NCalcUtility.InitializeExpressionUnit(ref c, caster, "Caster");
							NCalcUtility.InitializeExpressionUnit(ref c, target, "Target");
							NCalcUtility.InitializeExpressionCommand(ref c, new BattleCommand(cmd));
							c.Parameters["IsSingleTarget"] = Comn.countBits(runningThread.targetId) == 1;
							c.Parameters["AreCasterAndTargetsEnemies"] = caster.IsPlayer && (runningThread.targetId & 0xF0) == runningThread.targetId || !caster.IsPlayer && (runningThread.targetId & 0xF) == runningThread.targetId;
							c.Parameters["AreCasterAndTargetsAllies"] = caster.IsPlayer && (runningThread.targetId & 0xF) == runningThread.targetId || !caster.IsPlayer && (runningThread.targetId & 0x0F) == runningThread.targetId;
							c.Parameters["IsSingleSelectedTarget"] = Comn.countBits(originalTargetId) == 1;
							c.Parameters["AreCasterAndSelectedTargetsEnemies"] = caster.IsPlayer && (originalTargetId & 0xF0) == originalTargetId || !caster.IsPlayer && (originalTargetId & 0xF) == originalTargetId;
							c.Parameters["AreCasterAndSelectedTargetsAllies"] = caster.IsPlayer && (originalTargetId & 0xF) == originalTargetId || !caster.IsPlayer && (originalTargetId & 0x0F) == originalTargetId;
							c.Parameters["SFXUseCamera"] = isSFXThread ? runningThread.parentSFX.useCamera : false;
							List<BTL_DATA> allTrgt = btl_util.findAllBtlData(runningThread.targetId);
							Btl2dParam figParam;
							c.Parameters["IsAttackHeal"] = false;
							c.Parameters["IsAttackCritical"] = false;
							c.Parameters["IsAttackMiss"] = false;
							c.Parameters["IsAttackKill"] = false;
							c.Parameters["IsAttackGuard"] = false;
							c.Parameters["AttackDamage"] = 0;
							c.Parameters["AttackMPDamage"] = 0;
							foreach (BTL_DATA btl in allTrgt)
							{
								if (btl2dParam.TryGetValue(btl, out figParam))
								{
									if ((figParam.info & Param.FIG_INFO_HP_RECOVER) != 0)
										c.Parameters["IsAttackHeal"] = true;
									if ((figParam.info & Param.FIG_INFO_HP_CRITICAL) != 0)
										c.Parameters["IsAttackCritical"] = true;
									if ((figParam.info & Param.FIG_INFO_MISS) != 0)
										c.Parameters["IsAttackMiss"] = true;
									if ((figParam.info & Param.FIG_INFO_DEATH) != 0)
										c.Parameters["IsAttackKill"] = true;
									if ((figParam.info & Param.FIG_INFO_GUARD) != 0)
										c.Parameters["IsAttackGuard"] = true;
									if (figParam.hp > (Int32)c.Parameters["AttackDamage"])
										c.Parameters["AttackDamage"] = figParam.hp;
									if (figParam.mp > (Int32)c.Parameters["AttackMPDamage"])
										c.Parameters["AttackMPDamage"] = figParam.mp;
								}
							}
							c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
							c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
							if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
								break;
						}
						skipNextElse = true;
						Boolean loopTarget, chain, sync;
						Int32 loopCount;
						if (!code.TryGetArgInt32("LoopCount", out loopCount))
							loopCount = 1;
						if (!code.TryGetArgCharacter("Target", cmd.regist.btl_id, runningThread.targetId, out tmpChar))
							tmpChar = runningThread.targetId;
						code.TryGetArgBoolean("TargetLoop", out loopTarget);
						code.TryGetArgBoolean("Chain", out chain);
						code.TryGetArgBoolean("Sync", out sync);
						List<BTL_DATA> targList = btl_util.findAllBtlData(tmpChar);
						if (targList.Count == 0)
							break;						
                        Int32 loopTargetCount = loopTarget ? targList.Count : 1;
						for (Int32 i = 0; i < loopCount; i++)
							for (Int32 j = 0; j < loopTargetCount; j++)
							{
								BattleActionThread copy = dontCopyThread ? threadList[tmpInt] : new BattleActionThread(useMainThreadList ? threadList[tmpInt] : runningThread.parentSFX.sfxthread[tmpInt], runningThread.isReflectThread);
								copy.targetId = loopTarget ? targList[j].btl_id : tmpChar;
								if ((i == 0 && j == 0) || !chain)
									copy.active = true;
								if (!dontCopyThread)
								{
									if (chain)
									{
										UInt16 nextTargetId = !loopTarget ? tmpChar
															: j + 1 < loopTargetCount ? targList[j + 1].btl_id
															: i + 1 < loopCount ? targList[0].btl_id
															: (UInt16)0;
										if (nextTargetId != 0)
											copy.code.AddLast(new BattleActionCode("RunThread", "Thread", (threadList.Count + 1).ToString(), "Target", nextTargetId.ToString(), "DoNotCopyThread", true.ToString()));
									}
									threadList.Add(copy);
									copy.defaultSFXIndex = runningThread.defaultSFXIndex;
								}
								if (sync)
									runningThread.waitThread.Add(copy);
							}
					}
					break;
			}
			runningThread.skipNextElseThread = skipNextElse;
		}

		public Boolean ExecuteLoop()
		{
			// Update waits and progressive changes
			frameIndex++;
			animatedChar = 0;
			movingChar = 0;
			turningChar = 0;
			scalingChar = 0;
			for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
				if (next.bi.disappear == 0 && !next.animEndFrame)
					animatedChar |= next.btl_id;
			foreach (SequenceMove seqm in move)
			{
				seqm.frameCur++;
				movingChar |= seqm.Apply();
			}
			foreach (SequenceTurn seqt in turn)
			{
				seqt.frameCur++;
				turningChar |= seqt.Apply();
			}
			foreach (SequenceScale seqs in scale)
			{
				seqs.frameCur++;
				if (seqs.frameCur < seqs.frameEnd)
				{
					geo.geoScaleSetXYZ(seqs.character,
						(Int32)ParametricMovement.Interpolate(seqs.frameCur, seqs.frameEnd, seqs.origin.x, seqs.dest.x),
						(Int32)ParametricMovement.Interpolate(seqs.frameCur, seqs.frameEnd, seqs.origin.y, seqs.dest.y),
						(Int32)ParametricMovement.Interpolate(seqs.frameCur, seqs.frameEnd, seqs.origin.z, seqs.dest.z), seqs.shadow);
					scalingChar |= seqs.character.btl_id;
				}
				else
				{
					geo.geoScaleSetXYZ(seqs.character, (Int32)seqs.dest.x, (Int32)seqs.dest.y, (Int32)seqs.dest.z, seqs.shadow);
				}
			}
			foreach (SequenceFade seqf in fade)
			{
				seqf.frameCur++;
				Single alpha = ParametricMovement.Interpolate(seqf.frameCur, seqf.frameEnd, seqf.origin, seqf.dest);
				if (seqf.includeWeapon && seqf.character.bi.player != 0 && seqf.character.weapon_geo != null && (seqf.character.weaponFlags & geo.GEO_FLAGS_RENDER) != 0 && (seqf.character.weaponFlags & geo.GEO_FLAGS_CLIP) == 0)
				{
					btl_stat.GeoAddColor2DrawPacket(seqf.character.weapon_geo, (Int16)(alpha - 128), (Int16)(alpha - 128), (Int16)(alpha - 128));
					if (alpha < 70)
						btl_util.GeoSetABR(seqf.character.weapon_geo, "GEO_POLYFLAGS_TRANS_100_PLUS_25");
				}
				if (seqf.includeShadow && seqf.character.bi.shadow != 0)
				{
					btl_stat.GeoAddColor2DrawPacket(seqf.character.getShadow(), (Int16)(alpha - 128), (Int16)(alpha - 128), (Int16)(alpha - 128));
					if (alpha < 70)
						btl_util.GeoSetABR(seqf.character.getShadow(), "GEO_POLYFLAGS_TRANS_100_PLUS_25");
					else
						btl_util.GeoSetABR(seqf.character.getShadow(), "SHADOW");
				}
				if ((seqf.character.flags & geo.GEO_FLAGS_RENDER) != 0 && (seqf.character.flags & geo.GEO_FLAGS_CLIP) == 0)
				{
					btl_stat.GeoAddColor2DrawPacket(seqf.character.gameObject, (Int16)(alpha - 128), (Int16)(alpha - 128), (Int16)(alpha - 128));
					if (alpha < 70)
						btl_util.GeoSetABR(seqf.character.gameObject, "GEO_POLYFLAGS_TRANS_100_PLUS_25");
				}
				if (seqf.frameCur >= seqf.frameEnd)
				{
					if (seqf.disappear)
						seqf.character.SetDisappear(true, seqf.priority);
					else if (seqf.dest == 0f)
					{
						if (seqf.permanent)
						{
							seqf.character.meshflags |= (UInt32)seqf.meshId;
							seqf.character.mesh_current = (UInt16)(seqf.character.mesh_current | seqf.meshId);
						}
						btl_mot.HideMesh(seqf.character, (UInt16)seqf.meshId, false);
						if (seqf.includeWeapon)
							for (Int32 i = 0; i < seqf.character.weaponMeshCount; i++)
								geo.geoWeaponMeshHide(seqf.character, i);
					}
				}
			}
			move.RemoveAll(m => m.frameCur >= m.frameEnd);
			turn.RemoveAll(t => t.frameCur >= t.frameEnd);
			scale.RemoveAll(s => s.frameCur >= s.frameEnd);
			fade.RemoveAll(f => f.frameCur >= f.frameEnd);
			foreach (BattleActionThread th in threadList)
				if (th.active)
				{
					UpdateSFXWait(th);
					th.UpdateWaits(animatedChar, movingChar, turningChar, scalingChar);
				}
			// Execute action codes
			Boolean hasThreadToRun = true;
			while (hasThreadToRun)
			{
				Int32 runningThreadId;
				hasThreadToRun = false;
				for (runningThreadId = 0; runningThreadId < threadList.Count; runningThreadId++)
					if (threadList[runningThreadId].IsRunningNoWait())
						break;
				if (runningThreadId < threadList.Count)
				{
					ExecuteSingleCode(runningThreadId);
					hasThreadToRun = true;
				}
			}
			// Check if BattleAction is over
			Boolean isOver = true;
			Boolean hasRunningSFX = false;
			foreach (BattleActionThread th in threadList)
				if (th.active)
				{
					th.UpdateActiveFlag();
					if (th.active)
						isOver = false;
				}
			//if (isOver && ActivateReflect())
			//	isOver = false;
			if (move.Count > 0 || turn.Count > 0 || scale.Count > 0 || fade.Count > 0)
				isOver = false;
			if (!isOver)
			{
				foreach (SFXData sfx in sfxList)
				{
					if (sfx.runningSFX.Count > 0)
						hasRunningSFX = true;
					foreach (SFXData.RunningInstance run in sfx.runningSFX)
						run.frame++;
				}
				if (hasRunningSFX)
					PSXTextureMgr.isCaptureBlur = true;
			}
			if (cancel)
			{
				if (cmd.regist.bi.disappear != 0 && !hasRunningSFX)
				{
					cmd.regist.pos = cmd.regist.base_pos;
					return true;
				}
				if (!isOver)
				{
					// Make sure that cancelled sequence take the caster back to the base position
					Single dist = (cmd.regist.pos - cmd.regist.base_pos).magnitude;
					if (dist < 10)
					{
						cmd.regist.pos = cmd.regist.base_pos;
						return true;
					}
					cmd.regist.pos = cmd.regist.pos + Math.Min(100f, dist) * BattleActionCode.PolarVector(cmd.regist.base_pos - cmd.regist.pos);
					return false;
				}
			}
			return isOver;
		}

		public void Render()
		{
			try
			{
				foreach (SFXData sfx in sfxList)
				{
					for (Int32 i = 0; i < sfx.runningSFX.Count; i++)
					{
						SFXData.RunningInstance run = sfx.runningSFX[i];
						if (sfx.mesh.Render(run.frame, run))
						{
							sfx.runningSFX.RemoveAt(i);
							if (sfx.runningSFX.Count == 0)
								sfx.mesh.End();
							if (sfx.IsCancelled())
								foreach (BattleActionThread th in threadList)
									if (th.parentSFX == sfx) // TODO: consider when there are several RunningInstance of the same SFXData (or remove the possibility to have multiple RunningInstance...)
										th.active = false;
							i--;
						}
					}
				}
			}
			catch (Exception err)
			{
				Log.Error(err);
			}
		}

		public Boolean ActivateReflect()
		{
			if (reflectTriggered || cmd.info.reflec != 2)
				return false;
			if (FF9StateSystem.Battle.FF9Battle.btl_phase != 4 || !UIManager.Battle.FF9BMenu_IsEnable())
				return false;
			List<BTL_DATA> newTargList = btl_util.findAllBtlData(btl_cmd.MergeReflecTargetID(cmd.reflec));
			if (newTargList.Count == 0)
				return false;
			UInt16 newTarg = 0;
			foreach (BTL_DATA btl in newTargList)
				newTarg |= btl.btl_id;
			cmd.tar_id = newTarg;
			cmd.info.reflec = 1;
			if (cmd.regist.bi.player == 0)
				cmd.info.mon_reflec = 1;
			cmd.vfxRequest.SetupVfxRequest(cmd, new Int16[] { cmd.vfxRequest.monbone[0], cmd.vfxRequest.monbone[1], cmd.vfxRequest.arg0, (Int16)cmd.vfxRequest.flgs });
			cmd.vfxRequest.flgs |= 17;
			cmd.info.mode = command_mode_index.CMD_MODE_LOOP;
			threadList[reflectStartThreadIndex].active = true;
			threadList[reflectStartThreadIndex].targetId = newTarg;
			reflectTriggered = true;
			return true;
		}

		private void UpdateSFXWait(BattleActionThread th)
		{
			Boolean reflectIsRunning = false;
			foreach (BattleActionThread th2 in threadList)
				if (th2.active && th2.isReflectThread)
					reflectIsRunning = true;
			if (th.waitSFX >= 0)
			{
				if (th.waitSFX == 10 && !reflectIsRunning)
					th.waitSFX = -1;
				else if (th.waitSFX == 1 || (th.waitSFX >= 1000 && th.waitSFX < 2000))
				{
					Boolean stopWait = true;
					foreach (SFXData sfx in (th.waitSFX == 1 ? sfxList.ToArray() : new SFXData[] { sfxList[th.waitSFX - 1000] }))
						if (!sfx.loadHasEnded)
							stopWait = false;
					if (stopWait)
						th.waitSFX = -1;
				}
				else if (th.waitSFX == 2 || th.waitSFX >= 2000)
				{
					Boolean stopWait = true;
					foreach (SFXData sfx in (th.waitSFX == 2 ? sfxList.ToArray() : new SFXData[] { sfxList[th.waitSFX - 2000] }))
					{
						if (sfx.runningSFX.Count > 0)
							stopWait = false;
						if (threadList.FindIndex(th2 => th2.active && th2.parentSFX == sfx) >= 0)
							stopWait = false;
					}
					if (stopWait)
						th.waitSFX = -1;
				}
			}
		}
	}


	public class SequenceMove
	{
		public BTL_DATA character;
		public UInt16 targetId;
		public Vector3 origin;
		public Vector3 dest;
		public Single dist;
		public Boolean relDist;
		public Boolean useHeight;
		public Int32 frameCur;
		public Int32 frameEnd;

		public SequenceMove(BTL_DATA c, UInt16 tId, Vector3 d, Single di, Int32 f, Boolean h = false, Boolean rd = false, Boolean addTargetRadius = false)
		{
			character = c;
			targetId = tId;
			origin = c.pos;
			dest = d;
			dist = di;
			if (addTargetRadius)
			{
				List<BTL_DATA> targets = btl_util.findAllBtlData(tId);
				if (targets.Count == 1)
					dist += targets[0].radius_collision;
			}
			relDist = rd;
			useHeight = h;
			frameCur = 0;
			frameEnd = Math.Max(1, f);
		}

		public UInt16 Apply()
		{
			Vector3 targetPos = BattleActionCode.TargetAveragePos(targetId) + dest;
			UInt16 movingChar = 0;
			if (relDist)
				targetPos = character.pos + dist * BattleActionCode.PolarVector(targetPos - character.pos);
			else
				targetPos += dist * BattleActionCode.PolarVector(character.pos - targetPos);
			if (targetId == character.btl_id)
				targetPos = character.pos;
			if (!useHeight)
			{
				origin.y = character.pos.y;
				targetPos.y = character.pos.y;
			}
			if (frameCur < frameEnd)
			{
				character.pos = ParametricMovement.Interpolate(frameCur, frameEnd, origin, targetPos);
				movingChar |= character.btl_id;
			}
			else
			{
				character.pos = targetPos;
			}
			return movingChar;
		}
	}
	public class SequenceTurn
	{
		public BTL_DATA character;
		public UInt16 targetId;
		public Vector3 origin; // Euler angle
		public Vector3 dest;
		public Int32 frameCur;
		public Int32 frameEnd;

		public SequenceTurn(BTL_DATA c, UInt16 tId, Vector3 d, Int32 f)
		{
			character = c;
			targetId = tId;
			origin = c.rot.eulerAngles;
			dest = d;
			frameCur = 0;
			frameEnd = Math.Max(1, f);
		}

		public UInt16 Apply()
		{
			Vector3 euler = character.rot.eulerAngles;
			UInt16 turningChar = 0;
			Single destAngle;
			for (Int32 i = 0; i < 3; i++)
			{
				destAngle = 0;
				if (targetId != 0 && i == 1)
				{
					Vector3 targetDir = BattleActionCode.TargetAveragePos(targetId) - character.pos;
					if (targetDir.x != 0 || targetDir.z != 0)
						destAngle = ff9.ratan2(-targetDir.x, -targetDir.z);
					else
						destAngle = euler.y;
				}
				destAngle += dest[i];
				destAngle %= 360f;
				if (origin[i] - destAngle < -180f)
					destAngle -= 360f;
				if (destAngle - origin[i] < -180f)
					destAngle += 360f;
				if (frameCur < frameEnd)
				{
					euler[i] = ParametricMovement.Interpolate(frameCur, frameEnd, origin[i], destAngle);
					turningChar |= character.btl_id;
				}
				else
				{
					euler[i] = destAngle;
				}
			}
			character.rot.eulerAngles = euler;
			return turningChar;
		}
	}
	public class SequenceScale
	{
		public BTL_DATA character;
		public Boolean shadow;
		public Vector3 origin;
		public Vector3 dest;
		public Int32 frameCur;
		public Int32 frameEnd;

		public SequenceScale(BTL_DATA c, Vector3 d, Boolean s, Int32 f)
		{
			character = c;
			shadow = s;
			origin = new Vector3(c.geo_scale_x, c.geo_scale_y, c.geo_scale_z);
			dest = d;
			frameCur = 0;
			frameEnd = Math.Max(1, f);
		}
	}
	public class SequenceFade
	{
		public BTL_DATA character;
		public UInt16 meshId;
		public Boolean disappear;
		public Boolean permanent;
		public Boolean includeWeapon;
		public Boolean includeShadow;
		public Byte priority;
		public Single origin;
		public Single dest;
		public Int32 frameCur;
		public Int32 frameEnd;

		public SequenceFade(BTL_DATA c, UInt16 m, Boolean dis, Boolean per, Boolean w, Boolean sh, Single o, Single d, Int32 f, Byte pr)
		{
			character = c;
			meshId = m;
			disappear = dis;
			permanent = per;
			includeWeapon = w;
			includeShadow = sh;
			priority = pr;
			origin = o;
			dest = d;
			frameCur = 0;
			frameEnd = Math.Max(1, f);
		}
	}

	public class SequenceBBGIntensity
	{
		public Int32 origin;
		public Int32 dest;
		public Int32 frameCur;
		public Int32 frameEnd;
		public Int32 holdDuration;

		public SequenceBBGIntensity(Int32 d, Int32 f, Int32 hd)
		{
			origin = battlebg.nf_GetBbgIntensity();
			dest = d;
			frameCur = 0;
			frameEnd = Math.Max(1, f);
			holdDuration = hd;
		}

		public static void Apply(Boolean increaseFrame)
		{
			if (BattleAction.bbgIntensity.Count > 0)
			{
				Int32 bbgimin = 128;
				Int32 bbgimax = 128;
				List<SequenceBBGIntensity> holdAddition = new List<SequenceBBGIntensity>();
				foreach (SequenceBBGIntensity seqb in BattleAction.bbgIntensity)
				{
					if (increaseFrame)
						seqb.frameCur++;
					Int32 intensity = (Int32)ParametricMovement.Interpolate(seqb.frameCur, seqb.frameEnd, seqb.origin, seqb.dest);
					bbgimin = Math.Min(bbgimin, intensity);
					bbgimax = Math.Max(bbgimax, intensity);
					if (seqb.frameCur >= seqb.frameEnd && seqb.holdDuration > 0)
						holdAddition.Add(new SequenceBBGIntensity(seqb.dest, seqb.holdDuration, -1));
				}
				if (battlebg.nf_GetBbgIntensity() > 128) // Not useful
					battlebg.nf_SetBbgIntensity((Byte)bbgimax);
				else
					battlebg.nf_SetBbgIntensity((Byte)bbgimin);
				foreach (SequenceBBGIntensity newb in holdAddition)
				{
					newb.origin = newb.dest;
					BattleAction.bbgIntensity.Add(newb);
				}
				BattleAction.bbgIntensity.RemoveAll(f => f.frameCur >= f.frameEnd);
			}
		}
	}

	public class Btl2dParam
	{
		public UInt16 info = 0;
		public Int32 hp = 0;
		public Int32 mp = 0;
	}

	public enum EffectType
	{
		SpecialEffect, // text-based sequence & SFX.cs,
		EnemySequence // btlseq.cs & SFX.cs,
	}

	public const String INFORMATION_FILE = "FileList.txt";
	public const String SEQUENCE_FILE = "Sequence.seq";
	public const String PLAYER_SEQUENCE_FILE = "PlayerSequence.seq";
	public const String EXTENSION_SFXMESH_RAW = ".sfxmesh";
	public const String EXTENSION_SFXMESH_MODEL = ".sfxmodel";
	public const String EXTENSION_SFXCAMERA = ".sfxcamera";
	public const String EXTENSION_SEQ = ".seq";
}
