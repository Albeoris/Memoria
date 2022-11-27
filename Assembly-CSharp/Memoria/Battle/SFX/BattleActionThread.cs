using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Memoria;
using Memoria.Prime;
using Memoria.Data;
using Memoria.Assets;

public class BattleActionThread
{
	public LinkedList<BattleActionCode> code;
	public Boolean active;
	public UInt16 targetId;
	public Boolean isReflectThread;
	public Int32 defaultSFXIndex; // -1 until LoadSFX/LoadMonsterSFX is used, then point to the last one (inherit through RunThread)
	public SFXData parentSFX; // null except for threads started with PlaySFX/PlayMonsterSFX (inherit through RunThread)

	public Int32 waitFrame;
	public UInt16 waitAnimId;
	public UInt16 waitMoveId;
	public UInt16 waitTurnId;
	public UInt16 waitScaleId;
	public Int32 waitSFX;
	public List<BattleActionThread> waitThread;

	public delegate String TextGetter(Int32 id);

	public BattleActionThread()
	{
		code = new LinkedList<BattleActionCode>();
		active = false;
		targetId = 0;
		isReflectThread = false;
		defaultSFXIndex = -1;
		parentSFX = null;
		waitFrame = 0;
		waitAnimId = 0;
		waitMoveId = 0;
		waitTurnId = 0;
		waitScaleId = 0;
		waitSFX = -1;
		waitThread = new List<BattleActionThread>();
	}

	public BattleActionThread(BattleActionThread thread, Boolean asReflectThread = false)
	{
		code = new LinkedList<BattleActionCode>(thread.code);
		active = false;
		targetId = 0;
		isReflectThread = asReflectThread;
		defaultSFXIndex = -1;
		parentSFX = thread.parentSFX;
		waitFrame = 0;
		waitAnimId = 0;
		waitMoveId = 0;
		waitTurnId = 0;
		waitScaleId = 0;
		waitSFX = -1;
		waitThread = new List<BattleActionThread>();
	}

	public Boolean IsRunningNoWait()
	{
		return active && code.Count > 0 && waitFrame == 0 && waitAnimId == 0 && waitMoveId == 0 && waitTurnId == 0 && waitScaleId == 0 && waitSFX < 0 && waitThread.Count == 0;
	}

	public void UpdateWaits(UInt16 animatedChar, UInt16 movingChar, UInt16 turningChar, UInt16 scalingChar)
	{
		if (waitFrame > 0)
			waitFrame--;
		waitAnimId &= animatedChar;
		waitMoveId &= movingChar;
		waitTurnId &= turningChar;
		waitScaleId &= scalingChar;
		if (waitThread.Count > 0 && waitThread.FindIndex(th => th.active) < 0)
			waitThread.Clear();
	}

	public void UpdateActiveFlag()
	{
		active = code.Count > 0 || waitFrame > 0 || waitAnimId != 0 || waitMoveId != 0 || waitTurnId != 0 || waitScaleId != 0 || waitSFX >= 0 || waitThread.Count > 0;
	}

	public void SkipFrames(Int32 frameCount)
	{
		Int32 frameWait;
		if (frameCount < 0)
			code.AddFirst(new BattleActionCode("Wait", "Time", (-frameCount).ToString()));
		while (frameCount > 0 && code.Count > 0)
		{
			BattleActionCode action = code.First.Value;
			switch (action.operation)
			{
				case "Wait":
					action.TryGetArgInt32("Time", out frameWait);
					if (frameCount > frameWait)
					{
						frameCount -= frameWait;
						code.RemoveFirst();
					}
					else if (frameCount == frameWait)
					{
						code.RemoveFirst();
						return;
					}
					else
					{
						frameWait -= frameCount;
						action.argument["Time"] = frameWait.ToString();
						return;
					}
					break;
				case "WaitAnimation":
				case "WaitMove":
				case "WaitTurn":
				case "WaitSize":
				case "WaitMonsterSFXLoaded":
				case "WaitMonsterSFXDone":
				case "WaitSFXLoaded":
				case "WaitSFXDone":
				case "WaitReflect":
					return;
				default:
					code.RemoveFirst();
					break;
			}
		}
	}

	public static List<BattleActionThread> LoadFromTextSequence(String textCode)
	{
		List<BattleActionThread> result = new List<BattleActionThread>();
		Stack<Int32> threadStack = new Stack<Int32>();
		Int32 current = 0;
		result.Add(new BattleActionThread());
		threadStack.Push(current);
		BattleActionCode action;
		String[] lines = textCode.Split(new Char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
		for (Int32 i = 0; i < lines.Length; i++)
		{
			Int32 commentIndex = lines[i].IndexOf("//");
			String nonCommentedline = commentIndex < 0 ? lines[i] : lines[i].Substring(0, commentIndex);
			Int32 argIndex = nonCommentedline.IndexOf(":");
			String opCode = argIndex < 0 ? nonCommentedline : nonCommentedline.Substring(0, argIndex);
			opCode = opCode.Trim();
			if (opCode == "EndThread" && threadStack.Count > 1)
			{
				threadStack.Pop();
				current = threadStack.Peek();
			}
			String[] defaultArgList;
			if (!BattleActionCode.operationArguments.TryGetValue(opCode, out defaultArgList))
				continue;
			action = new BattleActionCode();
			action.operation = opCode;
			if (argIndex >= 0)
			{
				String argCode = nonCommentedline.Substring(argIndex + 1);
				String[] arg = argCode.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				for (Int32 j = 0; j < arg.Length; j++)
				{
					Int32 keyIndex = arg[j].IndexOf('=');
					String argKey = keyIndex >= 0 ? arg[j].Substring(0, keyIndex) : (j < defaultArgList?.Length ? defaultArgList[j] : "");
					String argValue = keyIndex >= 0 ? arg[j].Substring(keyIndex + 1) : arg[j];
					argKey = argKey.Trim();
					argValue = argValue.Trim();
					if (keyIndex < 0)
						foreach (String k in defaultArgList)
							if (k == argValue)
							{
								argKey = argValue;
								argValue = true.ToString();
								break;
							}
					if (!String.IsNullOrEmpty(argKey))
						action.argument[argKey] = argValue;
				}
			}
			if (opCode == "StartThread")
			{
				BattleActionThread newThread = new BattleActionThread();
				action.operation = "RunThread";
				action.argument["Thread"] = result.Count.ToString();
				result[current].code.AddLast(action);
				current = result.Count;
				threadStack.Push(current);
				result.Add(newThread);
			}
			else
			{
				result[current].code.AddLast(action);
			}
		}
		return result;
	}

	public static List<BattleActionThread> LoadFromBtlSeq(BTL_SCENE scene, btlseq.btlseqinstance seq, TextGetter battleText, Int32 atkNo, Dictionary<String, TextGetter> langBattleText = null)
	{
		List<BattleActionThread> result = new List<BattleActionThread>();
		BattleActionThread mainThread = new BattleActionThread();
		result.Add(mainThread);
		if (atkNo < 0 || atkNo >= seq.seq_work_set.SeqData.Length || seq.seq_work_set.SeqData[atkNo] == 0)
		{
			Log.Warning($"[{nameof(BattleActionThread)}] Invalid enemy sequence {atkNo} (battle has {scene.header.AtkCount} attacks)");
			return result;
		}
		Int32 seqCodePtr = seq.seq_work_set.SeqData[atkNo] + 4;
		Int32 seqAnimPtr = seq.seq_work_set.AnmOfsList[atkNo];
		Int32 messPtr = scene.header.TypCount + scene.header.AtkCount;
		Int32 sceneTypeId = seq.GetEnemyIndexOfSequence(atkNo);
		if (sceneTypeId >= scene.MonAddr.Length)
			Log.Warning($"[{nameof(BattleActionThread)}] Sequence says that {atkNo} belongs to enemy {sceneTypeId} that doesn't exist in scene");
		else
			for (Int32 i = 0; i < sceneTypeId; i++)
				messPtr += scene.MonAddr[i].MesCnt;
		Int32 sfxNum = -1;
		Int16 sfxBone0, sfxBone1, sfxArg;
		String sfxName;
		Boolean allowCamera = true;
		using (seq.sequenceReader = new BinaryReader(new MemoryStream(seq.data)))
		{
			BinaryReader r = seq.sequenceReader;
			r.BaseStream.Seek(seqCodePtr, SeekOrigin.Begin);
			while (true)
			{
				seq.wSeqCode = (Int32)r.ReadByte();
				if (seq.wSeqCode > (Int32)btlseq.gSeqProg.Length)
					seq.wSeqCode = 0;
				switch (seq.wSeqCode)
				{
					case 0: // END
					case 24: // Fast End
						if (seq.wSeqCode == 0)
						{
							mainThread.code.AddLast(new BattleActionCode("WaitAnimation", "Char", "Caster"));
							mainThread.code.AddLast(new BattleActionCode("PlayAnimation", "Char", "Caster", "Anim", "Idle"));
						}
						mainThread.code.AddLast(new BattleActionCode("WaitMonsterSFXDone", "Reflect", true.ToString()));
						mainThread.code.AddLast(new BattleActionCode("WaitTurn", "Char", "Caster"));
						mainThread.code.AddLast(new BattleActionCode("ActivateReflect"));
						mainThread.code.AddLast(new BattleActionCode("WaitReflect"));
						return result;
					case 1: // Wait
						mainThread.code.AddLast(new BattleActionCode("Wait", "Time", r.ReadByte().ToString()));
						break;
					case 2: // Effect Point
						mainThread.code.AddLast(new BattleActionCode("EffectPoint", "Char", "AllTargets", "Type", "Effect"));
						break;
					case 3: // Walk Target
					case 30: // Walk Target Front
						mainThread.code.AddLast(new BattleActionCode("MoveToTarget", "Char", "Caster", "Target", "AllTargets", "Time", r.ReadByte().ToString(), seq.wSeqCode == 3 ? "Distance" : "Offset", seq.wSeqCode == 3 ? (-r.ReadInt16()).ToString() : new Vector2(0, -r.ReadInt16()).ToString()));
						mainThread.code.AddLast(new BattleActionCode("WaitMove", "Char", "Caster"));
						break;
					case 4: // Return
						mainThread.code.AddLast(new BattleActionCode("MoveToPosition", "Char", "Caster", "Time", r.ReadByte().ToString(), "AbsolutePosition", "Default", "MoveHeight", "true"));
						mainThread.code.AddLast(new BattleActionCode("WaitMove", "Char", "Caster"));
						break;
					case 5: // Play Animation
						Byte animId = r.ReadByte();
						String animName = animId == 255 ? "Idle" : FF9BattleDB.Animation[seq.seq_work_set.AnmAddrList[seqAnimPtr + animId]];
						mainThread.code.AddLast(new BattleActionCode("PlayAnimation", "Char", "Caster", "Anim", animName));
						break;
					case 6: // SFX
					{
						BattleActionThread sfxThread = new BattleActionThread();
						sfxNum = r.ReadInt16();
						sfxBone0 = r.ReadByte();
						sfxName = Enum.IsDefined(typeof(SpecialEffect), sfxNum) ? ((SpecialEffect)sfxNum).ToString() : sfxNum.ToString();
						sfxThread.code.AddLast(new BattleActionCode("SetupReflect"));
						sfxThread.code.AddLast(new BattleActionCode("LoadSFX", "SFX", sfxName, "FirstBone", sfxBone0.ToString(), "Reflect", true.ToString()));
						sfxThread.code.AddLast(new BattleActionCode("Wait", "Time", (r.ReadByte() + 4).ToString(), "Reflect", true.ToString()));
						sfxThread.code.AddLast(new BattleActionCode("WaitSFXLoaded", "Reflect", true.ToString()));
						sfxThread.code.AddLast(new BattleActionCode("PlaySFX", "SkipSequence", (sfxBone0 != 0).ToString(), "Reflect", true.ToString()));
						sfxThread.code.AddLast(new BattleActionCode("WaitSFXDone", "Reflect", true.ToString()));
						mainThread.code.AddLast(new BattleActionCode("RunThread", "Thread", result.Count.ToString()));
						result.Add(sfxThread);
						break;
					}
					case 10: // Run Spell Animation
					{
						BattleActionThread sfxThread = mainThread;
						if ((SpecialEffect)sfxNum == SpecialEffect.Special_Silver_Dragon_Death)
						{
							sfxThread = new BattleActionThread();
							sfxThread.code.AddLast(new BattleActionCode("Wait", "Time", "44"));
							mainThread.code.AddLast(new BattleActionCode("RunThread", "Thread", result.Count.ToString()));
							result.Add(sfxThread);
						}
						sfxThread.code.AddLast(new BattleActionCode("PlayMonsterSFX", "Reflect", true.ToString()));
						break;
					}
					case 7: // Wait Animation
						mainThread.code.AddLast(new BattleActionCode("WaitAnimation", "Char", "Caster"));
						break;
					case 8: // Set Spell Animation + Channel
					case 26: // Set Spell Animation
						sfxNum = r.ReadInt16();
						sfxBone0 = r.ReadInt16();
						sfxBone1 = r.ReadInt16();
						sfxArg = r.ReadInt16();
						sfxName = Enum.IsDefined(typeof(SpecialEffect), sfxNum) ? ((SpecialEffect)sfxNum).ToString() : sfxNum.ToString();
						if (seq.wSeqCode == 8 && (SpecialEffect)sfxNum != SpecialEffect.Special_Necron_Engage && (SpecialEffect)sfxNum != SpecialEffect.Neutron_Ring)
							mainThread.code.AddLast(new BattleActionCode("Channel"));
						mainThread.code.AddLast(new BattleActionCode("SetupReflect", "Delay", "SFXLoaded"));
						if (SFXData.FixedCameraEffects.Contains((SpecialEffect)sfxNum))
							mainThread.code.AddLast(new BattleActionCode("LoadMonsterSFX", "SFX", sfxName, "FirstBone", sfxBone0.ToString(), "SecondBone", sfxBone1.ToString(), "Args", sfxArg.ToString(), "UseCamera", true.ToString(), "Reflect", true.ToString()));
						else
							mainThread.code.AddLast(new BattleActionCode("LoadMonsterSFX", "SFX", sfxName, "FirstBone", sfxBone0.ToString(), "SecondBone", sfxBone1.ToString(), "Args", sfxArg.ToString(), "Reflect", true.ToString()));
						allowCamera = false;
						break;
					case 9: // Wait Spell Loaded
						mainThread.code.AddLast(new BattleActionCode("WaitMonsterSFXLoaded", "Reflect", true.ToString()));
						break;
					case 11: // Wait Spell Executed
						mainThread.code.AddLast(new BattleActionCode("WaitMonsterSFXDone", "Reflect", true.ToString()));
						allowCamera = true;
						break;
					case 12: // Resize
						Int16 factor = r.ReadInt16();
						if (factor == -1)
							mainThread.code.AddLast(new BattleActionCode("ChangeSize", "Char", "Caster", "Size", "Reset", "Time", r.ReadByte().ToString()));
						else
							mainThread.code.AddLast(new BattleActionCode("ChangeSize", "Char", "Caster", "Size", (factor / 4096f).ToString(), "IsRelative", true.ToString(), "Time", r.ReadByte().ToString()));
						mainThread.code.AddLast(new BattleActionCode("WaitSize", "Char", "Caster"));
						break;
					case 13: // Hide Mesh
					case 15: // Show Mesh
						mainThread.code.AddLast(new BattleActionCode("ShowMesh", "Char", "Caster", "Mesh", r.ReadUInt16().ToString(), "Enable", (seq.wSeqCode == 15).ToString(), "IsPermanent", true.ToString()));
						break;
					case 14: // Battle Text
					case 33: // Battle Title
						Byte messId = r.ReadByte();
						Int32 messExactId = (messId & 128) != 0 ? scene.header.TypCount + atkNo : messPtr + messId;
						String text = battleText(messExactId);
						Int32 btlId;
						if (Configuration.VoiceActing.Enabled && FF9BattleDB.SceneData.TryGetValue(scene.nameIdentifier, out btlId))
						{
							String vaPath = String.Format("Voices/{0}/battle/{2}/va_{1}", Localization.GetSymbol(), messExactId, btlId);
							if (AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false))
								mainThread.code.AddLast(new BattleActionCode("PlaySound", "SoundType", "Voice", "Sound", btlId + ":" + messExactId));
							else
								SoundLib.VALog(String.Format("field:battle/{0}, msg:{1}, text:{2} path:{3} (not found)", btlId, messExactId, text, vaPath));
						}
						if (langBattleText == null || (messId & 128) != 0)
						{
							mainThread.code.AddLast(new BattleActionCode("Message", "Text", (messId & 128) != 0 ? "[CastName]" : text, "Priority", (messId & 128) != 0 ? "1" : "4", "Title", (seq.wSeqCode == 33 || (messId & 128) != 0).ToString(), "Reflect", (seq.wSeqCode == 33).ToString()));
						}
						else
						{
							BattleActionCode actionCode = new BattleActionCode("Message", "Priority", "4", "Title", (seq.wSeqCode == 33).ToString(), "Reflect", (seq.wSeqCode == 33).ToString());
							foreach (KeyValuePair<String, TextGetter> langText in langBattleText)
								actionCode.argument["Text" + langText.Key] = langText.Value(messExactId);
							mainThread.code.AddLast(actionCode);
						}
						break;
					case 16: // Run Camera
					case 18: // Run Camera Target Alternate
					case 32: // Run Camera Target
						if (allowCamera)
							mainThread.code.AddLast(new BattleActionCode("PlayCamera", "Camera", r.ReadByte().ToString(), "Char", seq.wSeqCode == 16 ? "0" : "AllTargets", "Alternate", (seq.wSeqCode == 18 ? true : false).ToString()));
						else
							r.ReadByte();
						break;
					case 17: // Change Stand Animation
						mainThread.code.AddLast(new BattleActionCode("ToggleStandAnimation", "Char", "Caster", "Alternate", (r.ReadByte() != 0).ToString()));
						break;
					case 19: // Walk Absolute
					{
						Byte time = r.ReadByte();
						Vector3 pos = new Vector3(r.ReadInt16(), -r.ReadInt16(), r.ReadInt16());
						mainThread.code.AddLast(new BattleActionCode("MoveToPosition", "Char", "Caster", "Time", time.ToString(), "AbsolutePosition", pos.ToString(), "MoveHeight", "true"));
						mainThread.code.AddLast(new BattleActionCode("ChangeCharacterProperty", "Char", "Caster", "Property", "base_pos", "Value", pos.ToString()));
						mainThread.code.AddLast(new BattleActionCode("WaitMove", "Char", "Caster"));
						break;
					}
					case 20: // Turn
						UInt16 baseAngle = r.ReadUInt16();
						String baseAngleName = (baseAngle & 32768) == 0 ? (baseAngle * 360f / 4096f).ToString()
							: baseAngle == 32768 ? "Default"
							: baseAngle == 32769 ? "AllTargets"
							: baseAngle == 32770 ? "Current" : "AllTargets";
						mainThread.code.AddLast(new BattleActionCode("Turn", "Char", "Caster", "BaseAngle", baseAngleName, "Angle", (r.ReadInt16() * 360f / 4096f).ToString(), "Time", r.ReadByte().ToString()));
						break;
					case 21: // Play Texture Animation
					case 22: // Play Texture Animation Once
					case 23: // Stop Texture Animation
						mainThread.code.AddLast(new BattleActionCode("PlayTextureAnimation", "Char", "Caster", "Anim", r.ReadByte().ToString(), "Once", (seq.wSeqCode == 22).ToString(), "Stop", (seq.wSeqCode == 23).ToString()));
						break;
					case 25: // Play Sound
						BattleActionThread soundThread = new BattleActionThread();
						UInt16 soundId = r.ReadUInt16();
						soundThread.code.AddLast(new BattleActionCode("Wait", "Time", (r.ReadByte() + 1).ToString()));
						r.Read();
						soundThread.code.AddLast(new BattleActionCode("PlaySound", "Sound", soundId.ToString(), "Volume", AllSoundDispatchPlayer.NormalizeVolume(r.ReadByte()).ToString(), "Once", (seq.wSeqCode == 21).ToString()));
						mainThread.code.AddLast(new BattleActionCode("RunThread", "Thread", result.Count.ToString()));
						result.Add(soundThread);
						break;
					case 27: // Walk Relative
					{
						Byte time = r.ReadByte();
						Vector3 pos = new Vector3(r.ReadInt16(), -r.ReadInt16(), r.ReadInt16());
						mainThread.code.AddLast(new BattleActionCode("MoveToPosition", "Char", "Caster", "Time", time.ToString(), "RelativePosition", pos.ToString(), "MoveHeight", "true"));
						mainThread.code.AddLast(new BattleActionCode("ChangeCharacterProperty", "Char", "Caster", "Property", "base_pos", "Value", "+" + pos.ToString()));
						mainThread.code.AddLast(new BattleActionCode("WaitMove", "Char", "Caster"));
						break;
					}
					case 28: // Change Target Bone
						mainThread.code.AddLast(new BattleActionCode("ChangeCharacterProperty", "Char", "Caster", "Property", "tar_bone", "Value", r.ReadByte().ToString()));
						break;
					case 29: // Fade Out
						mainThread.code.AddLast(new BattleActionCode("ShowMesh", "Char", "Caster", "Enable", false.ToString(), "Time", r.ReadByte().ToString(), "IsDisappear", true.ToString()));
						break;
					case 31: // Enable Shadow
						mainThread.code.AddLast(new BattleActionCode("ShowShadow", "Char", "Caster", "Enable", (r.ReadByte() != 0).ToString()));
						break;
				}
			}
		}
	}

	public static String GetSequenceStringCode(List<BattleActionThread> threadList)
	{
		if (threadList == null || threadList.Count == 0)
			return "";
		String seqCode = "";
		String indent = "";
		Int32 threadCallId;
		Stack<BattleActionThread> threadStack = new Stack<BattleActionThread>();
		Stack<String> endThreadLine = new Stack<String>();
		threadStack.Push(new BattleActionThread(threadList[0]));
		while (threadStack.Count > 0)
		{
			BattleActionThread curThread = threadStack.Peek();
			while (curThread.code.Count > 0)
			{
				BattleActionCode curCode = curThread.code.First.Value;
				curThread.code.RemoveFirst();
				Boolean isRunThread = curCode.operation == "RunThread";
				if (isRunThread)
					seqCode += indent + "StartThread";
				else
					seqCode += indent + curCode.operation;
				if (curCode.argument.Count > 0 && (!isRunThread || curCode.argument.Count > 1 || !curCode.argument.ContainsKey("Thread")))
					seqCode += ": ";
				Boolean addSemicolon = false;
				foreach (KeyValuePair<String, String> p in curCode.argument)
				{
					if (p.Key == "Thread")
						continue;
					if (addSemicolon)
						seqCode += " ; ";
					seqCode += p.Key + "=" + p.Value;
					addSemicolon = true;
				}
				seqCode += "\n";
				if (isRunThread)
				{
					endThreadLine.Push("EndThread\n");
					indent += "\t";
					if (curCode.TryGetArgInt32("Thread", out threadCallId) && threadCallId > 0 && threadCallId < threadList.Count)
						threadStack.Push(new BattleActionThread(threadList[threadCallId]));
					else
						threadStack.Push(new BattleActionThread());
					curThread = threadStack.Peek();
				}
			}
			threadStack.Pop();
			if (endThreadLine.Count > 0)
			{
				indent = indent.Substring(1);
				seqCode += indent + endThreadLine.Pop();
			}
		}
		return seqCode;
	}
}
