using System;
using System.IO;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using UnityEngine;
using Object = System.Object;

public class btlseq
{
	public btlseq()
	{
		btlseq.enemy = FF9StateSystem.Battle.FF9Battle.enemy;
	}

	public void ReadBattleSequence(String name)
	{
		String name2 = String.Concat(new Object[]
		{
			"BattleMap/BattleScene/EVT_BATTLE_",
			name,
			"/",
			FF9BattleDB.SceneData["BSC_" + name],
			".raw17"
		});
		TextAsset textAsset = AssetManager.Load<TextAsset>(name2, false);
		btlseq.data = textAsset.bytes;
		btlseq.seq_work_set = FF9StateSystem.Battle.FF9Battle.seq_work_set;
		using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(btlseq.data)))
		{
			Int16 num = binaryReader.ReadInt16();
			btlseq.camOffset = (Int32)binaryReader.ReadInt16();
			Int16 num2 = binaryReader.ReadInt16();
			Int16 num3 = binaryReader.ReadInt16();
			Int32[] array = new Int32[18];
			for (Int32 i = 0; i < 18; i++)
			{
				if (i < (Int32)num2)
				{
					array[i] = (Int32)binaryReader.ReadInt16();
				}
				else
				{
					array[i] = -1;
				}
			}
			Int32[] array2 = new Int32[(Int32)num3];
			for (Int32 j = 0; j < (Int32)num3; j++)
			{
				array2[j] = binaryReader.ReadInt32();
			}
			Byte[] array3 = new Byte[(Int32)num2];
			for (Int32 k = 0; k < (Int32)num2; k++)
			{
				array3[k] = binaryReader.ReadByte();
			}
			btlseq.seq_work_set.SeqData = array;
			btlseq.seq_work_set.AnmAddrList = array2;
			btlseq.seq_work_set.AnmOfsList = array3;
			Byte[] array4 = array3.Distinct<Byte>().ToArray<Byte>();
			this.ChangeToSequenceListNumber(array4);
			Byte[] array5 = new Byte[(Int32)array3.Length];
			Array.Copy(array3, array5, (Int32)array3.Length);
			this.ChangeToSequenceListNumber(array5);
			this.sequenceProperty = new SequenceProperty[(Int32)array4.Length];
			for (Int32 l = 0; l < (Int32)array4.Length; l++)
			{
				this.sequenceProperty[l] = new SequenceProperty();
				this.sequenceProperty[l].Montype = (Int32)array4[l];
			}
			for (Int32 m = 0; m < (Int32)num2; m++)
			{
				binaryReader.BaseStream.Seek((Int64)(array[m] + 4), SeekOrigin.Begin);
				Byte b = binaryReader.ReadByte();
				Byte b2 = binaryReader.ReadByte();
				if ((b != 24 && b != 7) || b2 != 0)
				{
					for (Int32 n = 0; n < (Int32)array4.Length; n++)
					{
						if (this.sequenceProperty[n].Montype == (Int32)array5[m])
						{
							this.sequenceProperty[n].PlayableSequence.Add(m);
						}
					}
				}
			}
		}
	}

	private void ChangeToSequenceListNumber(Byte[] list)
	{
		Byte[] array = list.Distinct<Byte>().ToArray<Byte>();
		for (Int32 i = 0; i < (Int32)list.Length; i++)
		{
			for (Int32 j = 0; j < (Int32)array.Length; j++)
			{
				if (list[i] == array[j])
				{
					list[i] = (Byte)j;
				}
			}
		}
	}

	public static void InitSequencer()
	{
		SEQ_WORK_SET seq_WORK_SET = btlseq.seq_work_set;
		SEQ_WORK[] seqWork = seq_WORK_SET.SeqWork;
		for (Int32 i = 0; i < 4; i++)
		{
			seqWork[i] = new SEQ_WORK();
			seqWork[i].CmdPtr = (CMD_DATA)null;
		}
	}

	public static void StartBtlSeq(Int32 pBtlID, Int32 pTarID, Int32 pSeqNo)
	{
		if (btlseq.seq_work_set.SeqData[pSeqNo] == 0)
		{
			return;
		}
		BTL_DATA next;
		for (next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
		{
			if (((Int32)next.btl_id & pBtlID) != 0)
			{
				break;
			}
		}
		if (next == null)
		{
			return;
		}
		CMD_DATA cmd_DATA = next.cmd[0];
		cmd_DATA.cmd_no = 0;
		cmd_DATA.sub_no = (Byte)pSeqNo;
		cmd_DATA.tar_id = (UInt16)pTarID;
		cmd_DATA.regist = next;
		cmd_DATA.aa = FF9StateSystem.Battle.FF9Battle.enemy_attack[pSeqNo];
		cmd_DATA.info = new CMD_DATA.SELECT_INFO();
		SEQ_WORK seq_WORK;
		if ((seq_WORK = btlseq.EntrySequence(cmd_DATA)) == null)
		{
			return;
		}
		seq_WORK.Flags.EventMode = true;
	}

	public static Boolean BtlSeqBusy()
	{
		SEQ_WORK[] seqWork = FF9StateSystem.Battle.FF9Battle.seq_work_set.SeqWork;
		for (Int16 num = 0; num < 4; num = (Int16)(num + 1))
		{
			if (seqWork[(Int32)num].CmdPtr != null)
			{
				return true;
			}
		}
		return false;
	}

	public static void RunSequence(CMD_DATA pCmd)
	{
		if (FF9StateSystem.Battle.FF9Battle.seq_work_set.SeqData[(Int32)pCmd.sub_no] == 0)
		{
			return;
		}
		if (btlseq.EntrySequence(pCmd) == null)
		{
			return;
		}
		if (pCmd.regist.bi.player == 0)
		{
			Vector3 eulerAngles = pCmd.regist.rot.eulerAngles;
			pCmd.regist.rot.eulerAngles = new Vector3(eulerAngles.x, 0f, eulerAngles.z);
		}
	}

	public static SEQ_WORK EntrySequence(CMD_DATA pCmd)
	{
		SEQ_WORK[] seqWork = btlseq.seq_work_set.SeqWork;
		Int16 num;
		for (num = 0; num < 4; num = (Int16)(num + 1))
		{
			if (seqWork[(Int32)num].CmdPtr == null)
			{
				break;
			}
		}
		if (num < 0)
		{
			return (SEQ_WORK)null;
		}
		seqWork[(Int32)num].Flags = new SeqFlag();
		seqWork[(Int32)num].CmdPtr = pCmd;
		seqWork[(Int32)num].CurPtr = btlseq.seq_work_set.SeqData[(Int32)pCmd.sub_no];
		seqWork[(Int32)num].OldPtr = 0;
		seqWork[(Int32)num].IncCnt = 0;
		seqWork[(Int32)num].DecCnt = 0;
		seqWork[(Int32)num].AnmCnt = 0;
		seqWork[(Int32)num].AnmIDOfs = btlseq.seq_work_set.AnmOfsList[(Int32)pCmd.sub_no];
		seqWork[(Int32)num].SfxTime = 0;
		seqWork[(Int32)num].TurnTime = 0;
		seqWork[(Int32)num].SVfxTime = 0;
		seqWork[(Int32)num].FadeTotal = 0;
		FF9StateSystem.Battle.FF9Battle.seq_work_set.CameraNo = 0;
		return seqWork[(Int32)num];
	}

	public static void Sequencer()
	{
		btlseq.wSeqCode = 0;
		SEQ_WORK[] seqWork = btlseq.seq_work_set.SeqWork;
		for (Int32 i = 0; i < 4; i++)
		{
			if (seqWork[i].CmdPtr != null)
			{
				SEQ_WORK seq_WORK = seqWork[i];
				seq_WORK.IncCnt = (Int16)(seq_WORK.IncCnt + 1);
				SEQ_WORK seq_WORK2 = seqWork[i];
				seq_WORK2.DecCnt = (Int16)(seq_WORK2.DecCnt - 1);
				SEQ_WORK seq_WORK3 = seqWork[i];
				seq_WORK3.AnmCnt = (Int16)(seq_WORK3.AnmCnt + 1);
				BTL_DATA regist = seqWork[i].CmdPtr.regist;
				Int32 num = 1;
				using (btlseq.sequenceReader = new BinaryReader(new MemoryStream(btlseq.data)))
				{
					while (num != 0)
					{
						btlseq.sequenceReader.BaseStream.Seek((Int64)(seqWork[i].CurPtr + 4), SeekOrigin.Begin);
						btlseq.wSeqCode = (Int32)btlseq.sequenceReader.ReadByte();
						if (btlseq.wSeqCode > (Int32)btlseq.gSeqProg.Length)
						{
							btlseq.wSeqCode = 0;
						}
						if (seqWork[i].Flags.WaitLoadVfx && SFX.GetTaskMonsteraStartOK() != 0)
						{
							seqWork[i].Flags.DoneLoadVfx = true;
							seqWork[i].Flags.WaitLoadVfx = false;
						}
						if (seqWork[i].CurPtr != seqWork[i].OldPtr && btlseq.gSeqProg[btlseq.wSeqCode].Init != null)
						{
							btlseq.gSeqProg[btlseq.wSeqCode].Init(seqWork[i], regist);
						}
						seqWork[i].OldPtr = seqWork[i].CurPtr;
						num = btlseq.gSeqProg[btlseq.wSeqCode].Exec(seqWork[i], regist);
					}
				}
				if (seqWork[i].TurnTime != 0)
				{
					Vector3 eulerAngles = regist.rot.eulerAngles;
					eulerAngles.y = (Single)(seqWork[i].TurnOrg + seqWork[i].TurnRot * (Int16)seqWork[i].TurnCnt / (Int16)seqWork[i].TurnTime);
					regist.rot = Quaternion.Euler(eulerAngles);
					SEQ_WORK seq_WORK4 = seqWork[i];
					Byte turnCnt;
					seq_WORK4.TurnCnt = (Byte)((turnCnt = seq_WORK4.TurnCnt) + 1);
					if (turnCnt >= seqWork[i].TurnTime)
					{
						seqWork[i].TurnTime = 0;
					}
				}
				if (seqWork[i].SfxTime != 0)
				{
					SEQ_WORK seq_WORK5 = seqWork[i];
					if ((seq_WORK5.SfxTime = (Byte)(seq_WORK5.SfxTime - 1)) == 0)
					{
						btl_util.SetBattleSfx(regist, seqWork[i].SfxNum, seqWork[i].SfxVol);
					}
				}
				if (seqWork[i].SVfxTime != 0)
				{
					SEQ_WORK seq_WORK6 = seqWork[i];
					if ((seq_WORK6.SVfxTime = (Byte)(seq_WORK6.SVfxTime - 1)) == 0)
					{
						Int16[] arg = new Int16[]
						{
							(Int16)seqWork[i].SVfxParam,
							0,
							0,
							0
						};
						btl_vfx.SetBattleVfx(seqWork[i].CmdPtr, (UInt32)seqWork[i].SVfxNum, arg);
					}
				}
				if (seqWork[i].FadeTotal != 0)
				{
					SEQ_WORK seq_WORK7 = seqWork[i];
					seq_WORK7.FadeStep = (Byte)(seq_WORK7.FadeStep - 1);
					btl_util.SetEnemyFadeToPacket(regist, (Int32)(seqWork[i].FadeStep * 32 / seqWork[i].FadeTotal));
					if (seqWork[i].FadeStep == 0)
					{
						seqWork[i].FadeTotal = 0;
					}
				}
			}
		}
	}

	public static void DispCharacter(BTL_DATA btl)
	{
		PosObj evt = btl.evt;
		if (btl.bi.slave == 0)
		{
			if (btl.bi.player != 0 && btl_mot.checkMotion(btl, 17))
			{
				Vector3 eulerAngles = btl.rot.eulerAngles;
				eulerAngles.y = 180f;
				btl.gameObject.transform.localRotation = Quaternion.Euler(eulerAngles);
			}
			else
			{
				btl.gameObject.transform.localRotation = btl.rot;
			}
			if ((!(HonoluluBattleMain.battleSceneName == "EF_E006") && !(HonoluluBattleMain.battleSceneName == "EF_E007")) || btl != FF9StateSystem.Battle.FF9Battle.btl_data[5])
			{
				btl.gameObject.transform.localPosition = btl.pos;
			}
			btl_mot.PlayAnim(btl);
			if ((UInt16)evt.animFrame >= GeoAnim.geoAnimGetNumFrames(btl))
			{
				if (!btl_mot.SetDefaultIdle(btl))
				{
					btl.evt.animFrame = 0;
				}
				else if (Status.checkCurStat(btl, 33558531u))
				{
					PosObj evt2 = btl.evt;
					evt2.animFrame = (Byte)(evt2.animFrame - 1);
				}
			}
			if (!Status.checkCurStat(btl, 33558531u) && btl.bi.stop_anim == 0)
			{
				if (btl.animation != (UnityEngine.Object)null)
				{
					btl.animation.enabled = true;
				}
				PosObj posObj = evt;
				posObj.animFrame = (Byte)(posObj.animFrame + 1);
			}
			else if (btl.animation != (UnityEngine.Object)null)
			{
				btl.animation.enabled = false;
			}
			Int32 num = btl.meshCount;
			Int32 num2 = 0;
			Int32 num3 = 0;
			btl.flags = (UInt16)(btl.flags & (UInt16)(~geo.GEO_FLAGS_RENDER));
			for (Int32 i = 0; i < num; i++)
			{
				if (geo.geoMeshChkFlags(btl, i) == 0)
				{
					btl.flags = (UInt16)(btl.flags | geo.GEO_FLAGS_RENDER);
					btl.SetIsEnabledMeshRenderer(i, true);
					num3++;
				}
				else
				{
					btl.SetIsEnabledMeshRenderer(i, false);
					num2++;
				}
			}
			if (num2 == num)
			{
				btl.SetIsEnabledBattleModelRenderer(false);
				if ((btl.bi.slot_no == 2 && btl.bi.player != 0) || (btl.bi.player == 0 && btl.dms_geo_id == 671))
				{
					Renderer[] componentsInChildren = btl.gameObject.transform.GetChildByName("long_hair").GetComponentsInChildren<Renderer>();
					Renderer[] array = componentsInChildren;
					for (Int32 j = 0; j < (Int32)array.Length; j++)
					{
						Renderer renderer = array[j];
						renderer.enabled = false;
					}
					Renderer[] componentsInChildren2 = btl.gameObject.transform.GetChildByName("short_hair").GetComponentsInChildren<Renderer>();
					Renderer[] array2 = componentsInChildren2;
					for (Int32 k = 0; k < (Int32)array2.Length; k++)
					{
						Renderer renderer2 = array2[k];
						renderer2.enabled = false;
					}
				}
			}
			if (num3 == num)
			{
				btl.SetIsEnabledBattleModelRenderer(true);
				if ((btl.bi.slot_no == 2 && btl.bi.player != 0) || (btl.bi.player == 0 && btl.dms_geo_id == 671))
				{
					Byte serialNumber = btl_util.getSerialNumber(btl);
					if (Configuration.Graphics.GarnetHair != 2 && (serialNumber == 4 || serialNumber == 3 || Configuration.Graphics.GarnetHair == 1))
					{
						Renderer[] componentsInChildren3 = btl.gameObject.transform.GetChildByName("long_hair").GetComponentsInChildren<Renderer>();
						Renderer[] array3 = componentsInChildren3;
						for (Int32 l = 0; l < (Int32)array3.Length; l++)
						{
							Renderer renderer3 = array3[l];
							renderer3.enabled = true;
						}
					}
					else
					{
						Renderer[] componentsInChildren4 = btl.gameObject.transform.GetChildByName("short_hair").GetComponentsInChildren<Renderer>();
						Renderer[] array4 = componentsInChildren4;
						for (Int32 m = 0; m < (Int32)array4.Length; m++)
						{
							Renderer renderer4 = array4[m];
							renderer4.enabled = true;
						}
					}
				}
				else if (btl.bi.slot_no == 0 && btl.bi.player != 0)
				{
					Byte serialNumber2 = btl_util.getSerialNumber(btl);
					if (serialNumber2 == 1)
					{
						btl.SetIsEnabledBattleModelRenderer(false);
					}
				}
			}
			if (!Status.checkCurStat(btl, 1073741824u))
			{
				GeoTexAnim.geoTexAnimService(btl.texanimptr);
				GeoTexAnim.geoTexAnimService(btl.tranceTexanimptr);
			}
			if (btl.weapon_geo != (UnityEngine.Object)null)
			{
				num = btl.weaponMeshCount;
				btl.weaponFlags = (UInt16)(btl.weaponFlags & (UInt16)(~geo.GEO_FLAGS_RENDER));
				for (Int32 n = 0; n < num; n++)
				{
					if (geo.geoWeaponMeshChkFlags(btl, n) == 0)
					{
						btl.weaponFlags = (UInt16)(btl.weaponFlags | geo.GEO_FLAGS_RENDER);
						btl.weaponRenderer[n].enabled = true;
					}
					else
					{
						btl.weaponRenderer[n].enabled = false;
					}
				}
			}
			return;
		}
		BTL_DATA masterEnemyBtlPtr = btl_util.GetMasterEnemyBtlPtr();
		if (masterEnemyBtlPtr == null)
		{
			return;
		}
		btl.rot = masterEnemyBtlPtr.rot;
		btl_mot.setSlavePos(btl, ref btl.base_pos);
		btl_mot.setBasePos(btl);
	}

	public static void FF9DrawShadowCharBattle(GameObject[] shadowArray, Int32 charNo, Int32 posY, Int32 BoneNo)
	{
		GameObject gameObject = shadowArray[charNo];
		GameObject gameObject2 = (GameObject)null;
		if (gameObject == (UnityEngine.Object)null)
		{
			return;
		}
		gameObject.SetActive(true);
		if (charNo < 9)
		{
			for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
			{
				if (next.bi.player != 0 && (Int32)next.bi.slot_no == charNo)
				{
					gameObject2 = next.gameObject;
				}
			}
			if (gameObject2 == (UnityEngine.Object)null)
			{
				global::Debug.LogError("gameObject is NULL");
			}
		}
		else
		{
			gameObject2 = FF9StateSystem.Battle.FF9Battle.btl_data[4 + charNo - 9].gameObject;
		}
		btlseq.ff9battleShadowCalculateMatrix(gameObject, gameObject2, posY, ff9btl.ff9btl_get_bonestart(BoneNo), ff9btl.ff9btl_get_boneend(BoneNo));
	}

	public static void ff9battleShadowCalculateMatrix(GameObject ObjPtr, GameObject CharPtr, Int32 PosY, Int32 BoneStartNo, Int32 BoneEndNo)
	{
		Vector3 localPosition = ObjPtr.transform.localPosition;
		Transform childByName = CharPtr.transform.GetChildByName("bone" + BoneStartNo.ToString("D3"));
		if (childByName == (UnityEngine.Object)null)
		{
			if (ObjPtr.activeSelf)
			{
				ObjPtr.SetActive(false);
			}
			return;
		}
		localPosition.x = childByName.position.x;
		localPosition.y = (Single)PosY + 2.5f;
		localPosition.z = childByName.position.z;
		ObjPtr.transform.localPosition = localPosition;
	}

	public static void SetupBattleScene()
	{
		SB2_HEAD header = FF9StateSystem.Battle.FF9Battle.btl_scene.header;
		BTL_SCENE btl_scene = FF9StateSystem.Battle.FF9Battle.btl_scene;
		UInt16 flags = header.Flags;
		btl_scene.Info = new BTL_SCENE_INFO();
		if ((flags & 1) != 0)
		{
			btl_scene.Info.SpecialStart = 1;
		}
		if ((flags & 2) != 0)
		{
			btl_scene.Info.BackAttack = 1;
		}
		if ((flags & 4) != 0)
		{
			btl_scene.Info.NoGameOver = 1;
		}
		if ((flags & 8) != 0)
		{
			btl_scene.Info.NoExp = 1;
		}
		if ((flags & 16) == 0)
		{
			btl_scene.Info.WinPose = 1;
		}
		if ((flags & 32) == 0)
		{
			btl_scene.Info.Runaway = 1;
		}
		if ((flags & 64) != 0)
		{
			btl_scene.Info.NoNeighboring = 1;
		}
		if ((flags & 128) != 0)
		{
			btl_scene.Info.NoMagical = true;
		}
		if ((flags & 256) != 0)
		{
			btl_scene.Info.ReverseAttack = 1;
		}
		if ((flags & 512) != 0)
		{
			btl_scene.Info.FixedCamera1 = 1;
		}
		if ((flags & 1024) != 0)
		{
			btl_scene.Info.FixedCamera2 = 1;
		}
		if ((flags & 2048) != 0)
		{
			btl_scene.Info.AfterEvent = 1;
		}
		if (FF9StateSystem.Battle.IsPlayFieldBGMInCurrentBattle)
		{
			btl_scene.Info.FieldBGM = 1;
		}
		else
		{
			btl_scene.Info.FieldBGM = 0;
		}
		battle.btl_bonus.ap = (UInt16)FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[(Int32)FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].AP;
	}

	private static Int32 SeqExecEnd(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecEnd!!!!!!!!!!!!");
		if (!pSeqWork.Flags.FinishIdle && pSeqWork.AnmCnt != 0 && (UInt16)pMe.evt.animFrame >= GeoAnim.geoAnimGetNumFrames(pMe))
		{
			btl_mot.setMotion(pMe, pMe.bi.def_idle);
			pMe.evt.animFrame = 0;
			pSeqWork.Flags.FinishIdle = true;
		}
		if ((pMe.stat.cur & 33558530u) == 0u && !pSeqWork.Flags.FinishIdle)
		{
			return 0;
		}
		if (SFX.isRunning)
		{
			return 0;
		}
		if (pSeqWork.TurnTime != 0)
		{
			return 0;
		}
		if (UIManager.Battle.BtlWorkLibra)
		{
			return 0;
		}
		if (!pSeqWork.Flags.EventMode)
		{
			btl_cmd.ReqFinishCommand();
		}
		pSeqWork.CmdPtr = (CMD_DATA)null;
		if (FF9StateSystem.Battle.isDebug)
		{
			pMe.pos = (pMe.base_pos = pMe.original_pos);
			pMe.gameObject.transform.localPosition = pMe.pos;
		}
		return 0;
	}

	public static void SeqInitWait(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqInitWait");
		pSeqWork.DecCnt = (Int16)btlseq.sequenceReader.ReadByte();
	}

	public static Int32 SeqExecWait(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecWait");
		if (pSeqWork.DecCnt <= 0)
		{
			pSeqWork.CurPtr += 2;
			return 1;
		}
		return 0;
	}

	public static Int32 SeqExecCalc(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecCalc");
		CMD_DATA cmdPtr = pSeqWork.CmdPtr;
		UInt16 tar_id = cmdPtr.tar_id;
		for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
		{
			if ((next.btl_id & tar_id) != 0)
			{
				btl_cmd.ExecVfxCommand(next);
			}
		}
		pSeqWork.CurPtr++;
		return 1;
	}

	public static void SeqInitMoveToTarget(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqInitMoveToTarget");
		WK_MOVE wk_MOVE = new WK_MOVE();
		wk_MOVE.Next = 4;
		pSeqWork.IncCnt = 1;
		wk_MOVE.Frames = (Int16)btlseq.sequenceReader.ReadByte();
		for (Int32 i = 0; i < 3; i++)
		{
			wk_MOVE.Org[i] = (Int16)pMe.pos[i];
		}
		wk_MOVE.Dst[1] = wk_MOVE.Org[1];
		btlseq.SeqSubTargetAveragePos(pSeqWork.CmdPtr.tar_id, out wk_MOVE.Dst[0], out wk_MOVE.Dst[2]);
		Int16 num = btlseq.sequenceReader.ReadInt16();
		if (wk_MOVE.Org[0] != wk_MOVE.Dst[0] || wk_MOVE.Org[2] != wk_MOVE.Dst[2])
		{
			if (btlseq.wSeqCode == 30)
			{
				Int16[] dst = wk_MOVE.Dst;
				Int32 num2 = 2;
				dst[num2] = (Int16)(dst[num2] + (Int16)(-num));
			}
			else
			{
				Double num3 = (Double)Mathf.Atan2((Single)(wk_MOVE.Dst[0] - wk_MOVE.Org[0]), (Single)(wk_MOVE.Dst[2] - wk_MOVE.Org[2]));
				Double num4 = (Double)Mathf.Sin((Single)num3);
				Double num5 = (Double)Mathf.Cos((Single)num3);
				Int16[] dst2 = wk_MOVE.Dst;
				Int32 num6 = 0;
				dst2[num6] = (Int16)(dst2[num6] + (Int16)(num4 * (Double)num));
				Int16[] dst3 = wk_MOVE.Dst;
				Int32 num7 = 2;
				dst3[num7] = (Int16)(dst3[num7] + (Int16)(num5 * (Double)num));
			}
		}
		pSeqWork.Work = btlseq.SequenceConverter.WkMoveToWork(wk_MOVE);
	}

	public static Int32 SeqExecMoveToTarget(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecMoveToTarget");
		WK_MOVE wk_MOVE = new WK_MOVE();
		wk_MOVE = btlseq.SequenceConverter.WorkToWkMove(pSeqWork.Work);
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		for (Int16 num = 0; num < 3; num = (Int16)(num + 1))
		{
			Int16 num2 = wk_MOVE.Org[(Int32)num];
			Int16 num3 = wk_MOVE.Dst[(Int32)num];
			zero[(Int32)num] = pMe.pos[(Int32)num];
			zero2[(Int32)num] = (Single)num3;
			pMe.pos[(Int32)num] = (Single)(num3 - num2) * ((Single)pSeqWork.IncCnt * 1f) / (Single)wk_MOVE.Frames + (Single)num2;
		}
		if (pSeqWork.IncCnt >= wk_MOVE.Frames)
		{
			pSeqWork.CurPtr += (Int32)wk_MOVE.Next;
		}
		return 0;
	}

	public static void SeqInitMoveToTurn(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqInitMoveToTurn");
		WK_MOVE wk_MOVE = new WK_MOVE();
		wk_MOVE.Next = 2;
		pSeqWork.IncCnt = 1;
		wk_MOVE.Frames = (Int16)btlseq.sequenceReader.ReadByte();
		for (Int16 num = 0; num < 3; num = (Int16)(num + 1))
		{
			wk_MOVE.Org[(Int32)num] = (Int16)pMe.gameObject.transform.localPosition[(Int32)num];
			wk_MOVE.Dst[(Int32)num] = (Int16)pMe.base_pos[(Int32)num];
		}
		pSeqWork.Work = btlseq.SequenceConverter.WkMoveToWork(wk_MOVE);
	}

	public static Int32 SeqExecAnim(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecAnim");
		Byte b = btlseq.sequenceReader.ReadByte();
		if (b == 255)
		{
			Int32 num = (Int32)((pMe.bi.def_idle == 0) ? 0 : 1);
			String name = FF9StateSystem.Battle.FF9Battle.enemy[(Int32)pMe.bi.slot_no].et.mot[num];
			btl_mot.setMotion(pMe, name);
		}
		else
		{
			Int32 num = (Int32)(pSeqWork.AnmIDOfs + b);
			String name2 = FF9BattleDB.Animation[btlseq.seq_work_set.AnmAddrList[num]];
			btl_mot.setMotion(pMe, name2);
		}
		pMe.evt.animFrame = 0;
		pSeqWork.AnmCnt = 0;
		pSeqWork.CurPtr += 2;
		return 1;
	}

	public static Int32 SeqExecSVfx(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecSVfx");
		pSeqWork.SVfxNum = btlseq.sequenceReader.ReadUInt16();
		pSeqWork.SVfxParam = btlseq.sequenceReader.ReadByte();
		pSeqWork.SVfxTime = (Byte)(btlseq.sequenceReader.ReadByte() + 1);
		pSeqWork.CurPtr += 5;
		return 1;
	}

	public static Int32 SeqExecWaitAnim(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecWaitAnim");
		if (pSeqWork.AnmCnt != 0 && ((UInt16)pMe.evt.animFrame >= GeoAnim.geoAnimGetNumFrames(pMe) || (pMe.stat.cur & 33558530u) != 0u))
		{
			SFX.SetEffCamTrigger();
			pSeqWork.CurPtr++;
			return 1;
		}
		return 0;
	}

	public static Int32 SeqExecVfx(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecVfx");
		Int16[] array = new Int16[4];
		if (SFX.GetEffectOvRun() != 0)
		{
			return 0;
		}
		UInt16 fx_no = btlseq.sequenceReader.ReadUInt16();
		array[0] = btlseq.sequenceReader.ReadInt16();
		array[1] = btlseq.sequenceReader.ReadInt16();
		array[2] = btlseq.sequenceReader.ReadInt16();
		array[3] = (Int16)((btlseq.wSeqCode != 26) ? 0 : 1);
		btl_vfx.SetBattleVfx(pSeqWork.CmdPtr, (UInt32)fx_no, array);
		pSeqWork.Flags.WaitLoadVfx = true;
		pSeqWork.CurPtr += 9;
		return 1;
	}

	public static Int32 SeqExecWaitLoadVfx(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecWaitLoadVfx");
		if (pSeqWork.Flags.DoneLoadVfx)
		{
			pSeqWork.CurPtr++;
			return 1;
		}
		return 0;
	}

	public static Int32 SeqExecStartVfx(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecStartVfx");
		SFX.SetTaskMonsteraStart();
		pSeqWork.CurPtr++;
		return 1;
	}

	public static Int32 SeqExecWaitVfx(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecWaitVfx");
		if (SFX.GetEffectOvRun() == 0)
		{
			pSeqWork.CurPtr++;
			return 1;
		}
		return 0;
	}

	public static void SeqInitScale(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqInitScale");
		WK_SCALE wk_SCALE = new WK_SCALE();
		wk_SCALE.Org = (Int16)(pMe.gameObject.transform.localScale.x * 4096f);
		Int16 num = btlseq.sequenceReader.ReadInt16();
		if (num == -1)
		{
			num = 4096;
		}
		else
		{
			num = (Int16)(wk_SCALE.Org * num / 4096);
		}
		wk_SCALE.Scl = (Int16)(num - wk_SCALE.Org);
		pSeqWork.IncCnt = 1;
		wk_SCALE.Frames = (Int16)btlseq.sequenceReader.ReadByte();
		pSeqWork.Work = btlseq.SequenceConverter.WkScaleToWork(wk_SCALE);
	}

	public static Int32 SeqExecScale(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecScale");
		WK_SCALE wk_SCALE = btlseq.SequenceConverter.WorkToWkScale(pSeqWork.Work);
		UInt16 num = (UInt16)(wk_SCALE.Scl * pSeqWork.IncCnt / wk_SCALE.Frames + wk_SCALE.Org);
		geo.geoScaleSet(pMe, (Int32)num);
		btl_scrp.SetCharacterData(pMe, 55u, (UInt32)num);
		if (num == 4096)
		{
			geo.geoScaleReset(pMe);
		}
		if (pSeqWork.IncCnt >= wk_SCALE.Frames)
		{
			pSeqWork.CurPtr += 4;
			return 1;
		}
		return 0;
	}

	public static Int32 SeqExecMeshHide(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecMeshHide");
		UInt16 num = btlseq.sequenceReader.ReadUInt16();
		pMe.meshflags |= (UInt32)num;
		pMe.mesh_current = (UInt16)(pMe.mesh_current | num);
		btl_mot.HideMesh(pMe, num, false);
		pSeqWork.CurPtr += 3;
		return 1;
	}

	public static Int32 SeqExecMessage(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecMessage");
		UInt16 num = (UInt16)btlseq.sequenceReader.ReadByte();
		if ((num & 128) != 0)
		{
			btlseq.BattleLog("wOfs " + pSeqWork.CmdPtr.aa.Name);
			UIManager.Battle.SetBattleTitle(pSeqWork.CmdPtr.aa.Name, 1);
		}
		else
		{
			num = (UInt16)(num + (UInt16)FF9StateSystem.Battle.FF9Battle.enemy[(Int32)pMe.bi.slot_no].et.mes);
			btlseq.BattleLog("wMsg " + num);
			if (btlseq.wSeqCode == 33)
			{
				String str = FF9TextTool.BattleText((Int32)num);
				UIManager.Battle.SetBattleTitle(str, 3);
			}
			else
			{
				String str2 = FF9TextTool.BattleText((Int32)num);
				UIManager.Battle.SetBattleMessage(str2, 3);
			}
		}
		pSeqWork.CurPtr += 2;
		return 1;
	}

	public static Int32 SeqExecMeshShow(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecMeshShow");
		UInt16 num = btlseq.sequenceReader.ReadUInt16();
		pMe.meshflags &= (UInt32)(~num);
		pMe.mesh_current = (UInt16)(pMe.mesh_current & (UInt16)(~num));
		btl_mot.ShowMesh(pMe, num, false);
		pSeqWork.CurPtr += 3;
		return 1;
	}

	public static Int32 SeqExecSetCamera(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecSetCamera");
		btlseq.seq_work_set.CameraNo = btlseq.sequenceReader.ReadByte();
		pSeqWork.CurPtr += 2;
		return 1;
	}

	public static Int32 SeqExecDefaultIdle(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecDefaultIdle");
		pMe.bi.def_idle = btlseq.sequenceReader.ReadByte();
		pSeqWork.CurPtr += 2;
		return 1;
	}

	private static BTL_DATA SeqSubGetTarget(UInt16 pTarID)
	{
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		BTL_DATA next;
		for (next = ff9Battle.btl_list.next; next != null; next = next.next)
		{
			if ((next.btl_id & pTarID) != 0)
			{
				break;
			}
		}
		return next;
	}

	public static Int32 SeqExecRunCamera(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecRunCamera");
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		SEQ_WORK_SET seq_WORK_SET = ff9Battle.seq_work_set;
		if (btlseq.wSeqCode != 32)
		{
			if (pSeqWork.CmdPtr.aa.Info.DefaultCamera == false)
			{
				if (FF9StateSystem.Settings.cfg.camera == 1UL)
				{
					goto IL_16C;
				}
				UInt32 num = pMe.stat.cur;
				for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
				{
					if ((next.btl_id & pSeqWork.CmdPtr.tar_id) != 0)
					{
						num |= next.stat.cur;
					}
				}
				if ((num & 335544320u) != 0u)
				{
					goto IL_16C;
				}
				if (Comn.random8() >= 128)
				{
					goto IL_16C;
				}
			}
		}
		Int16[] array = new Int16[3];
		btlseq.SeqSubTargetAveragePos(pSeqWork.CmdPtr.tar_id, out array[0], out array[2]);
		array[1] = 0;
		seq_WORK_SET.CamTrgCPos = new Vector3((Single)array[0], (Single)array[1], (Single)array[2]);
		seq_WORK_SET.CamExe = pMe;
		seq_WORK_SET.CamTrg = btlseq.SeqSubGetTarget(pSeqWork.CmdPtr.tar_id);
		SFX.SetCameraTarget(seq_WORK_SET.CamTrgCPos, seq_WORK_SET.CamExe, seq_WORK_SET.CamTrg);
		ff9Battle.seq_work_set.CameraNo = btlseq.sequenceReader.ReadByte();
		SFX.SetEnemyCamera(pMe);
		IL_16C:
		pSeqWork.CurPtr += 2;
		return 1;
	}

	public static void SeqInitMoveToPoint(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqInitMoveToPoint");
		WK_MOVE wk_MOVE = btlseq.SequenceConverter.WorkToWkMove(pSeqWork.Work);
		wk_MOVE.Next = 8;
		pSeqWork.IncCnt = 1;
		wk_MOVE.Frames = (Int16)btlseq.sequenceReader.ReadByte();
		for (Int16 num = 0; num < 3; num = (Int16)(num + 1))
		{
			wk_MOVE.Org[(Int32)num] = (Int16)pMe.gameObject.transform.localPosition[(Int32)num];
			Int16 num2 = btlseq.sequenceReader.ReadInt16();
			if (num == 1)
			{
				num2 = (Int16)(num2 * -1);
			}
			wk_MOVE.Dst[(Int32)num] = num2;
		}
		pSeqWork.Work = btlseq.SequenceConverter.WkMoveToWork(wk_MOVE);
	}

	public static Int32 SeqExecMoveToPoint(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecMoveToPoint");
		btlseq.SeqExecMoveToTarget(pSeqWork, pMe);
		for (Int16 num = 0; num < 3; num = (Int16)(num + 1))
		{
			pMe.base_pos[(Int32)num] = pMe.pos[(Int32)num];
		}
		return 0;
	}

	public static Int32 SeqExecTurn(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecTurn");
		Int16 num = 0;
		Int16 num2 = 0;
		Int16 num3 = btlseq.sequenceReader.ReadInt16();
		Int16 num4 = btlseq.sequenceReader.ReadInt16();
		num4 = (Int16)((Single)num4 / 4096f * 360f);
		pSeqWork.TurnTime = btlseq.sequenceReader.ReadByte();
		pSeqWork.TurnOrg = (Int16)pMe.rot.eulerAngles.y;
		pSeqWork.TurnCnt = 1;
		if (((Int32)num3 & 32768) != 0)
		{
			Int16 num5 = (Int16)(num3 & Int16.MaxValue);
			if (num5 != 0)
			{
				if (num5 != 1)
				{
					num3 = (Int16)pMe.rot.eulerAngles[1];
				}
				else
				{
					btlseq.SeqSubTargetAveragePos(pSeqWork.CmdPtr.tar_id, out num, out num2);
					num = (Int16)(pMe.gameObject.transform.localPosition.x - (Single)num);
					num2 = (Int16)(pMe.gameObject.transform.localPosition.z - (Single)num2);
					num3 = (Int16)((num != 0 || num2 != 0) ? ((Int16)((Double)Mathf.Atan2((Single)num, (Single)num2) * 57.295779513082323)) : ((Int16)pMe.rot.eulerAngles.y));
				}
			}
			else
			{
				num3 = (Int16)pMe.evt.rotBattle.eulerAngles.y;
			}
		}
		else
		{
			num3 = (Int16)((Single)num3 / 4096f * 360f);
		}
		num3 = (Int16)(num3 + num4);
		num3 = (Int16)(pMe.rot.eulerAngles.y - (Single)num3);
		pSeqWork.TurnRot = (Int16)((num3 <= 180) ? ((Int16)(-num3)) : ((Int16)(360 - num3)));
		pSeqWork.CurPtr += 6;
		return 1;
	}

	public static void SeqSubTargetAveragePos(UInt16 pTarID, out Int16 px, out Int16 pz)
	{
		Int32 num = 0;
		px = 0;
		pz = 0;
		for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
		{
			if ((next.btl_id & pTarID) != 0)
			{
				px = (Int16)(px + (Int16)next.pos[0]);
				pz = (Int16)(pz + (Int16)next.pos[2]);
				num++;
			}
		}
		if (num > 1)
		{
			px = (Int16)(px / (Int16)num);
			pz = (Int16)(pz / (Int16)num);
		}
	}

	public static Int32 SeqExecTexAnimPlay(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecTexAnimPlay");
		GeoTexAnim.geoTexAnimPlay(pMe.texanimptr, (Int32)btlseq.sequenceReader.ReadByte());
		pSeqWork.CurPtr += 2;
		return 1;
	}

	public static Int32 SeqExecTexAnimOnce(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecTexAnimOnce");
		GeoTexAnim.geoTexAnimPlayOnce(pMe.texanimptr, (Int32)btlseq.sequenceReader.ReadByte());
		pSeqWork.CurPtr += 2;
		return 1;
	}

	public static Int32 SeqExecTexAnimStop(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecTexAnimStop");
		GeoTexAnim.geoTexAnimStop(pMe.texanimptr, (Int32)btlseq.sequenceReader.ReadByte());
		pSeqWork.CurPtr += 2;
		return 1;
	}

	public static Int32 SeqExecFastEnd(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecFastEnd");
		if (SFX.isRunning)
		{
			return 0;
		}
		if (pSeqWork.TurnTime != 0)
		{
			return 0;
		}
		if (!pSeqWork.Flags.EventMode)
		{
			btl_cmd.ReqFinishCommand();
		}
		pSeqWork.CmdPtr = (CMD_DATA)null;
		return 0;
	}

	public static Int32 SeqExecSfx(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecSfx");
		pSeqWork.SfxNum = btlseq.sequenceReader.ReadUInt16();
		pSeqWork.SfxTime = (Byte)(btlseq.sequenceReader.ReadByte() + 1);
		btlseq.sequenceReader.Read();
		pSeqWork.SfxVol = btlseq.sequenceReader.ReadByte();
		pSeqWork.CurPtr += 6;
		return 1;
	}

	public static void SeqInitMoveToOffset(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqInitMoveToOffset");
		WK_MOVE wk_MOVE = new WK_MOVE();
		wk_MOVE.Next = 8;
		pSeqWork.IncCnt = 1;
		wk_MOVE.Frames = (Int16)btlseq.sequenceReader.ReadByte();
		for (Int16 num = 0; num < 3; num = (Int16)(num + 1))
		{
			wk_MOVE.Org[(Int32)num] = (Int16)pMe.gameObject.transform.localPosition[(Int32)num];
			Int16 num2 = btlseq.sequenceReader.ReadInt16();
			if (num == 1)
			{
				num2 = (Int16)(num2 * -1);
			}
			wk_MOVE.Dst[(Int32)num] = (Int16)(wk_MOVE.Org[(Int32)num] + num2);
		}
		pSeqWork.Work = btlseq.SequenceConverter.WkMoveToWork(wk_MOVE);
	}

	public static Int32 SeqExecTargetBone(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecTargetBone");
		pMe.tar_bone = btlseq.sequenceReader.ReadByte();
		pSeqWork.CurPtr += 2;
		return 1;
	}

	public static Int32 SeqExecFadeOut(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecFadeOut");
		pSeqWork.FadeTotal = (pSeqWork.FadeStep = btlseq.sequenceReader.ReadByte());
		pSeqWork.CurPtr += 2;
		return 1;
	}

	public static Int32 SeqExecShadow(SEQ_WORK pSeqWork, BTL_DATA pMe)
	{
		btlseq.BattleLog("SeqExecShadow");
		pMe.bi.shadow = btlseq.sequenceReader.ReadByte();
		if (pMe.bi.shadow != 0)
		{
			pMe.getShadow().SetActive(true);
		}
		else
		{
			pMe.getShadow().SetActive(false);
		}
		pSeqWork.CurPtr += 2;
		return 1;
	}

	public static void BattleLog(String str)
	{
	}

	public const Int32 BTL_LIST_PLAYER_0 = 0;

	public const Int32 BTL_LIST_PLAYER_1 = 1;

	public const Int32 BTL_LIST_PLAYER_2 = 2;

	public const Int32 BTL_LIST_PLAYER_3 = 3;

	public const Int32 BTL_LIST_ENERMY_0 = 4;

	public const Int32 BTL_LIST_ENERMY_1 = 5;

	public const Int32 BTL_LIST_ENERMY_2 = 6;

	public const Int32 BTL_LIST_ENERMY_3 = 7;

	public const Int32 RET_BLOCK = 0;

	public const Int32 RET_CONTINUE = 1;

	public const UInt32 ANIM_STOP_STATUS = 33558530u;

	public static Byte[] data;

	public static BinaryReader sequenceReader;

	public static SEQ_WORK_SET seq_work_set;

	public static ENEMY[] enemy;

	public static Int32 wSeqCode;

	public static BTL_DATA[] btl_list;

	public SequenceProperty[] sequenceProperty;

	public static Int32 camOffset;

	public static btlseq.SequenceProgram[] gSeqProg = new btlseq.SequenceProgram[]
	{
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecEnd)),
		new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitWait), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecWait)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecCalc)),
		new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitMoveToTarget), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMoveToTarget)),
		new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitMoveToTurn), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMoveToTarget)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecAnim)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecSVfx)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecWaitAnim)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecVfx)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecWaitLoadVfx)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecStartVfx)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecWaitVfx)),
		new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitScale), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecScale)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMeshHide)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMessage)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMeshShow)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecSetCamera)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecDefaultIdle)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecRunCamera)),
		new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitMoveToPoint), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMoveToPoint)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecTurn)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecTexAnimPlay)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecTexAnimOnce)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecTexAnimStop)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecFastEnd)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecSfx)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecVfx)),
		new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitMoveToOffset), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMoveToPoint)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecTargetBone)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecFadeOut)),
		new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitMoveToTarget), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMoveToTarget)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecShadow)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecRunCamera)),
		new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMessage))
	};

	public static class SequenceConverter
	{
		public static WK_MOVE WorkToWkMove(Byte[] work)
		{
			WK_MOVE wk_MOVE = new WK_MOVE();
			wk_MOVE.Org[0] = (Int16)(((Int32)work[1] << 8) + (Int32)(work[0] & Byte.MaxValue));
			wk_MOVE.Org[1] = (Int16)(((Int32)work[3] << 8) + (Int32)(work[2] & Byte.MaxValue));
			wk_MOVE.Org[2] = (Int16)(((Int32)work[5] << 8) + (Int32)(work[4] & Byte.MaxValue));
			wk_MOVE.Dst[0] = (Int16)(((Int32)work[7] << 8) + (Int32)(work[6] & Byte.MaxValue));
			wk_MOVE.Dst[1] = (Int16)(((Int32)work[9] << 8) + (Int32)(work[8] & Byte.MaxValue));
			wk_MOVE.Dst[2] = (Int16)(((Int32)work[11] << 8) + (Int32)(work[10] & Byte.MaxValue));
			wk_MOVE.Frames = (Int16)(((Int32)work[13] << 8) + (Int32)(work[12] & Byte.MaxValue));
			wk_MOVE.Next = (UInt16)(((Int32)work[15] << 8) + (Int32)(work[14] & Byte.MaxValue));
			return wk_MOVE;
		}

		public static Byte[] WkMoveToWork(WK_MOVE wMove)
		{
			return new Byte[]
			{
				(Byte)(wMove.Org[0] & 255),
				(Byte)(wMove.Org[0] >> 8),
				(Byte)(wMove.Org[1] & 255),
				(Byte)(wMove.Org[1] >> 8),
				(Byte)(wMove.Org[2] & 255),
				(Byte)(wMove.Org[2] >> 8),
				(Byte)(wMove.Dst[0] & 255),
				(Byte)(wMove.Dst[0] >> 8),
				(Byte)(wMove.Dst[1] & 255),
				(Byte)(wMove.Dst[1] >> 8),
				(Byte)(wMove.Dst[2] & 255),
				(Byte)(wMove.Dst[2] >> 8),
				(Byte)(wMove.Frames & 255),
				(Byte)(wMove.Frames >> 8),
				(Byte)(wMove.Next & 255),
				(Byte)(wMove.Next >> 8)
			};
		}

		public static WK_SCALE WorkToWkScale(Byte[] work)
		{
			return new WK_SCALE
			{
				Org = (Int16)(((Int32)work[1] << 8) + (Int32)(work[0] & Byte.MaxValue)),
				Scl = (Int16)(((Int32)work[3] << 8) + (Int32)(work[2] & Byte.MaxValue)),
				Frames = (Int16)(((Int32)work[5] << 8) + (Int32)(work[4] & Byte.MaxValue))
			};
		}

		public static Byte[] WkScaleToWork(WK_SCALE wScale)
		{
			return new Byte[]
			{
				(Byte)(wScale.Org & 255),
				(Byte)(wScale.Org >> 8),
				(Byte)(wScale.Scl & 255),
				(Byte)(wScale.Scl >> 8),
				(Byte)(wScale.Frames & 255),
				(Byte)(wScale.Frames >> 8)
			};
		}
	}

	public class SequenceProgram
	{
		public SequenceProgram(btlseq.SequenceProgram.InitEvent init, btlseq.SequenceProgram.ExecEvent exec)
		{
			this.Init = init;
			this.Exec = exec;
		}

		public btlseq.SequenceProgram.InitEvent Init;

		public btlseq.SequenceProgram.ExecEvent Exec;

		public delegate void InitEvent(SEQ_WORK pSeqWork, BTL_DATA pMe);

		public delegate Int32 ExecEvent(SEQ_WORK pSeqWork, BTL_DATA pMe);
	}
}
