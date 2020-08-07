using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.Common;
using Memoria;
using Memoria.Data;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
	private void UpdateSphereIndicator()
	{
		this.sphere.transform.position = FF9StateSystem.Battle.FF9Battle.btl_data[FF9StateSystem.Battle.selectCharPosID].gameObject.transform.position;
	}

	private void Start()
	{
		if (FF9StateSystem.Battle.selectCharPosID >= FF9StateSystem.Battle.selectPlayerCount)
		{
			FF9StateSystem.Battle.selectCharPosID = FF9StateSystem.Battle.selectPlayerCount - 1;
		}
		this.battleMain = base.GetComponent<HonoluluBattleMain>();
		FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum = FF9StateSystem.Battle.patternIndex;
		this.patternIndex = String.Concat("Pattern ", (Int32)(this.battleMain.btlScene.PatNum + 1), "/", this.battleMain.btlScene.header.PatCount);
		this.cameraController = GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>();
		this.effNum = 126;
		this.strEffNum = this.effNum.ToString();
		this.mapIDs = FF9StateSystem.Battle.mapName.Keys.ToArray<Int32>();
		this.battleMapString = FF9StateSystem.Battle.battleMapIndex.ToString();
		this.mapIndex = Array.IndexOf<Int32>(this.mapIDs, FF9StateSystem.Battle.battleMapIndex);
		this.btlSeq = base.GetComponent<HonoluluBattleMain>().btlSeq;
		this.btlScene = base.GetComponent<HonoluluBattleMain>().btlScene;
		this.seqNo = 0;
		this.seqList = base.GetComponent<HonoluluBattleMain>().seqList;
		Boolean[] isTrance = FF9StateSystem.Battle.isTrance;
		for (Int32 i = 0; i < (Int32)isTrance.Length; i++)
		{
			isTrance[i] = false;
		}
	}

	private void setMaterialTrans(Material mat)
	{
		mat.SetFloat("_Mode", 3f);
		mat.SetInt("_SrcBlend", 1);
		mat.SetInt("_DstBlend", 10);
		mat.SetInt("_ZWrite", 0);
		mat.DisableKeyword("_ALPHATEST_ON");
		mat.DisableKeyword("_ALPHABLEND_ON");
		mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
		mat.renderQueue = 3000;
	}

	private void OnGUI()
	{
		if (!EventEngineUtils.showDebugUI)
		{
			return;
		}
		Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
		DebugGuiSkin.ApplySkin();
		if (!FF9StateSystem.Battle.isDebug)
		{
			GUILayout.BeginArea(fullscreenRect);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Back"))
			{
				SceneDirector.Replace(PersistenSingleton<SceneDirector>.Instance.LastScene, SceneTransition.FadeOutToBlack_FadeIn, true);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			return;
		}
		GUILayout.BeginArea(fullscreenRect);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Back"))
		{
			SceneDirector.Replace("MainMenu", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.FlexibleSpace();
		GUILayout.FlexibleSpace();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		GUILayout.BeginVertical("Box");
		GUILayout.BeginHorizontal();
		if (this.isOpenBattleMapDebugPanel)
		{
			if (GUILayout.Button("Effect UI"))
			{
				this.isOpenEffectDebugPanel = !this.isOpenEffectDebugPanel;
				if (this.isOpenEffectDebugPanel)
				{
					this.isOpenAttackDebugPanel = false;
				}
			}
			if (GUILayout.Button("Attack UI"))
			{
				this.isOpenAttackDebugPanel = !this.isOpenAttackDebugPanel;
				if (this.isOpenAttackDebugPanel)
				{
					this.isOpenEffectDebugPanel = false;
				}
			}
			GUILayout.FlexibleSpace();
		}
		if (GUILayout.Button("Debug UI"))
		{
			this.isOpenBattleMapDebugPanel = !this.isOpenBattleMapDebugPanel;
		}
		GUILayout.EndHorizontal();
		if (this.isOpenBattleMapDebugPanel)
		{
			this.BuildBattleMapDebugPanel();
		}
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		if (this.isOpenEffectDebugPanel)
		{
			this.OnUiSpecialEffect();
		}
		if (this.isOpenAttackDebugPanel)
		{
			this.OnUiAttackDebug();
		}
	}

	private void BuildBattleMapDebugPanel()
	{
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(" < "))
		{
			this.mapIndex--;
			if (this.mapIndex < 0)
			{
				this.mapIndex = (Int32)this.mapIDs.Length - 1;
			}
			FF9StateSystem.Battle.battleMapIndex = this.mapIDs[this.mapIndex];
			FF9StateSystem.Battle.patternIndex = 0;
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		this.battleMapString = GUILayout.TextField(this.battleMapString, GUILayout.Width(200f));
		if (GUILayout.Button(" > "))
		{
			this.mapIndex++;
			if (this.mapIndex > (Int32)this.mapIDs.Length)
			{
				this.mapIndex = 0;
			}
			FF9StateSystem.Battle.battleMapIndex = this.mapIDs[this.mapIndex];
			FF9StateSystem.Battle.patternIndex = 0;
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		if (GUILayout.Button("Jump"))
		{
			Int32 num = 0;
			Int32.TryParse(this.battleMapString, out num);
			if (!this.mapIDs.Contains(num))
			{
				return;
			}
			FF9StateSystem.Battle.battleMapIndex = num;
			FF9StateSystem.Battle.patternIndex = 0;
			SoundLib.StopAllSounds(true);
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(" < "))
		{
			if (this.battleMain.btlScene.header.PatCount == 1)
			{
				return;
			}
			BTL_SCENE btl_scene = FF9StateSystem.Battle.FF9Battle.btl_scene;
			btl_scene.PatNum = (Byte)(btl_scene.PatNum - 1);
			if (FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum < 0 || FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum == 255)
			{
				FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum = (Byte)(this.battleMain.btlScene.header.PatCount - 1);
			}
			this.patternIndex = String.Concat("Pattern ", (Int32)(FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum + 1), " / ", this.battleMain.btlScene.header.PatCount);
			FF9StateSystem.Battle.patternIndex = FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum;
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.Label(this.patternIndex, GUILayout.Width(200f));
		if (GUILayout.Button(" > "))
		{
			if (this.battleMain.btlScene.header.PatCount == 1)
			{
				return;
			}
			BTL_SCENE btl_scene2 = FF9StateSystem.Battle.FF9Battle.btl_scene;
			btl_scene2.PatNum = (Byte)(btl_scene2.PatNum + 1);
			if (FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum >= this.battleMain.btlScene.header.PatCount)
			{
				FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum = 0;
			}
			this.patternIndex = String.Concat("Pattern ", (Int32)(FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum + 1), " / ", this.battleMain.btlScene.header.PatCount);
			FF9StateSystem.Battle.patternIndex = FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum;
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUI.enabled = (btlseq.seq_work_set.SeqWork[0] != null && btlseq.seq_work_set.SeqWork[0].CmdPtr == null);
		if (GUILayout.Button(" < "))
		{
			this.seqNo--;
			if (this.seqNo < 0)
			{
				this.seqNo = this.seqList.Count - 1;
			}
		}
		GUILayout.Label("sequence: " + (this.seqNo + 1), GUILayout.Width(150f));
		if (GUILayout.Button(" > "))
		{
			this.seqNo++;
			if (this.seqNo >= this.seqList.Count)
			{
				this.seqNo = 0;
			}
		}
		if (GUILayout.Button(" Play "))
		{
			btl_cmd.SetEnemyCommandBySequence(1, BattleCommandId.EnemyAtk, (UInt32)this.seqList[this.seqNo]);
		}
		GUI.enabled = true;
		GUILayout.EndHorizontal();
		this.OnUiCamera();
		Boolean flag = GUILayout.Toggle(FF9StateSystem.Battle.isFrontRow, "Front Row");
		if (flag != FF9StateSystem.Battle.isFrontRow)
		{
			FF9StateSystem.Battle.isFrontRow = flag;
			this.battleMain.SetFrontRow();
		}
		Boolean flag2 = GUILayout.Toggle(FF9StateSystem.Battle.isLevitate, "Levitate");
		if (flag2 != FF9StateSystem.Battle.isLevitate)
		{
			FF9StateSystem.Battle.isLevitate = flag2;
			if (!flag2)
			{
				for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
				{
					if (next.bi.player != 0)
					{
						Vector3 pos = next.pos;
						pos.y = 0f;
						next.pos = pos;
					}
				}
			}
		}
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Normal Attack"))
		{
			FF9StateSystem.Battle.debugStartType = battle_start_type_tags.BTL_START_NORMAL_ATTACK;
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		if (GUILayout.Button("First Attack"))
		{
			FF9StateSystem.Battle.debugStartType = battle_start_type_tags.BTL_START_FIRST_ATTACK;
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		if (GUILayout.Button("Back Attack"))
		{
			FF9StateSystem.Battle.debugStartType = battle_start_type_tags.BTL_START_BACK_ATTACK;
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.EndHorizontal();
	}

	private void OnUiCamera()
	{
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(" < "))
		{
			this.cameraController.SetPrevDefaultCamera();
		}
		GUILayout.Label("Cam ID: " + (this.cameraController.GetCurrDefaultCamID() + 1), GUILayout.Width(200f));
		if (GUILayout.Button(" > "))
		{
			this.cameraController.SetNextDefaultCamera();
		}
		GUILayout.EndHorizontal();
	}

	private void OnUiSpecialEffect()
	{
		Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
		Rect screenRect = fullscreenRect;
		screenRect.height *= 0.375f;
		screenRect.y = fullscreenRect.height - screenRect.height;
		GUILayout.BeginArea(screenRect);
		this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, GUILayout.Width(screenRect.width), GUILayout.Height(screenRect.height));
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		this.OnUiSpecialEffectBottom1();
		this.OnUiSpecialEffectBottom0();
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void OnUiSpecialEffectBottom1()
	{
		GUILayout.BeginHorizontal("box");
		if (GUILayout.Button("Line " + ((!SFX.isDebugLine) ? "Off" : "On")))
		{
			SFX.isDebugLine = !SFX.isDebugLine;
		}
		if (GUILayout.Button("Cam " + ((!SFX.isDebugCam) ? "On" : "Off")))
		{
			SFX.isDebugCam = !SFX.isDebugCam;
		}
		if (GUILayout.Button("Sub " + SFX.defaultSubOrder))
		{
			SFX.defaultSubOrder = (SFX.defaultSubOrder + 1) % 3;
			SFX.subOrder = SFX.defaultSubOrder;
		}
		if (GUILayout.Button("Col " + SFX.defaultColIntensity))
		{
			SFX.defaultColIntensity = (SFX.defaultColIntensity + 1) % 3;
			SFX.colIntensity = SFX.defaultColIntensity;
		}
		if (GUILayout.Button("Ofs " + SFX.pixelOffset))
		{
			SFX.pixelOffset = (SFX.pixelOffset + 1 & 1);
		}
		if (GUILayout.Button("Alp " + SFX.colThreshold))
		{
			SFX.colThreshold = (SFX.colThreshold + 1 & 1);
		}
		if (GUILayout.Button("Bilinear " + ((!SFX.isDebugFillter) ? "SPRT" : "All")))
		{
			SFX.isDebugFillter = !SFX.isDebugFillter;
		}
		GUILayout.Label(String.Concat("Sub:", SFX.subOrder, " /Col:", SFX.colIntensity, " /Prim:", SFXRender.primCount, " / Frame:", SFX.frameIndex));
		GUILayout.EndHorizontal();
	}

	private void OnUiSpecialEffectBottom0()
	{
		GUILayout.BeginHorizontal("box");
		if (GUILayout.Button("<"))
		{
			this.effNum--;
			if (this.effNum < 0)
			{
				this.effNum = 510;
			}
			this.strEffNum = this.effNum.ToString();
		}
		String text = this.strEffNum;
		this.strEffNum = GUILayout.TextField(text);
		if (this.strEffNum != text)
		{
			Int32 num = 0;
			if (Int32.TryParse(this.strEffNum, out num))
			{
				if (num >= 0 && num < 511)
				{
					this.effNum = num;
					this.strEffNum = this.effNum.ToString();
				}
			}
			else if (this.strEffNum.Length == 0)
			{
				this.effNum = 0;
			}
		}
		if (GUILayout.Button(">"))
		{
			this.effNum++;
			if (this.effNum >= 511)
			{
				this.effNum = 0;
			}
			this.strEffNum = this.effNum.ToString();
		}
		Boolean enabled = true;
		GUI.enabled = enabled;
		if (GUILayout.Button(" P2E "))
		{
			this.SFXPlay(8, FF9StateSystem.Battle.selectCharPosID, 4);
		}
		if (GUILayout.Button(" E2P "))
		{
			this.SFXPlay(1, 4, 0);
		}
		if (GUILayout.Button("P(Me)"))
		{
			this.SFXPlay(8, FF9StateSystem.Battle.selectCharPosID, FF9StateSystem.Battle.selectCharPosID);
		}
		if (GUILayout.Button("E(Me)"))
		{
			this.SFXPlay(1, 4, 4);
		}
		if (GUILayout.Button("Summon"))
		{
			Byte[] monbone = new Byte[2];
			PSX_LIBGTE.VECTOR trgcpos = default(PSX_LIBGTE.VECTOR);
			trgcpos.vx = (trgcpos.vy = (trgcpos.vz = 0));
			SFX.Begin(4, 0, monbone, trgcpos);
			SFX.SetExe(FF9StateSystem.Battle.FF9Battle.btl_data[0]);
			SFX.SetTrg(FF9StateSystem.Battle.FF9Battle.btl_data, 4);
			SFX.Play(this.effNum);
		}
		if (GUILayout.Button(" Item "))
		{
			this.SFXPlay(2, FF9StateSystem.Battle.selectCharPosID, 1);
		}
		if (GUILayout.Button("Sword"))
		{
			Byte[] monbone2 = new Byte[2];
			PSX_LIBGTE.VECTOR trgcpos2 = default(PSX_LIBGTE.VECTOR);
			trgcpos2.vx = (trgcpos2.vy = (trgcpos2.vz = 0));
			SFX.Begin(4, 0, monbone2, trgcpos2);
			SFX.SetExe(FF9StateSystem.Battle.FF9Battle.btl_data[3]);
			SFX.SetMExe(FF9StateSystem.Battle.FF9Battle.btl_data[1]);
			SFX.SetTrg(FF9StateSystem.Battle.FF9Battle.btl_data[4]);
			SFX.Play(this.effNum);
		}
		if (GUILayout.Button(" Ref "))
		{
			Byte[] monbone3 = new Byte[2];
			PSX_LIBGTE.VECTOR trgcpos3 = default(PSX_LIBGTE.VECTOR);
			trgcpos3.vx = (trgcpos3.vy = (trgcpos3.vz = 0));
			SFX.Begin(16, 0, monbone3, trgcpos3);
			SFX.SetExe(FF9StateSystem.Battle.FF9Battle.btl_data[4]);
			SFX.SetTrg(FF9StateSystem.Battle.FF9Battle.btl_data[4]);
			SFX.SetRTrgTest(FF9StateSystem.Battle.FF9Battle.btl_data[2]);
			SFX.Play(this.effNum);
		}
		if (GUILayout.Button("Auto " + ((!SFX.isDebugAutoPlay) ? "Off" : "On")))
		{
			SFX.isDebugAutoPlay = !SFX.isDebugAutoPlay;
		}
		if (SFX.isDebugAutoPlay && !SFX.isRunning)
		{
			this.effNum = BattleUI.repTable[this.repIndex];
			this.repIndex++;
			if (this.repIndex > (Int32)BattleUI.repTable.Length)
			{
				this.repIndex = 0;
			}
			this.strEffNum = this.effNum.ToString();
			this.SFXPlay(8, FF9StateSystem.Battle.selectCharPosID, 4);
		}
		GUI.enabled = true;
		GUILayout.EndHorizontal();
	}

	private void SFXPlay(UInt16 flag, Int32 exe, Int32 trg)
	{
		Byte[] monbone = new Byte[2];
		PSX_LIBGTE.VECTOR trgcpos = default(PSX_LIBGTE.VECTOR);
		trgcpos.vx = (trgcpos.vy = (trgcpos.vz = 0));
		SFX.Begin(flag, 0, monbone, trgcpos);
		SFX.SetExe(FF9StateSystem.Battle.FF9Battle.btl_data[exe]);
		SFX.SetTrg(FF9StateSystem.Battle.FF9Battle.btl_data[trg]);
		SFX.Play(this.effNum);
		if (exe == 4)
		{
			SFX.SetTaskMonsteraStart();
		}
	}

	private void Update()
	{
	}

	private void OnUiAttackDebug()
	{
		Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
		Rect screenRect = fullscreenRect;
		screenRect.height *= 0.375f;
		screenRect.y = fullscreenRect.height - screenRect.height;
		GUILayout.BeginArea(screenRect);
		GUILayout.BeginVertical();
		this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, GUILayout.Width(screenRect.width), GUILayout.Height(screenRect.height));
		GUILayout.BeginHorizontal("Box");
		Boolean isTrance = GUILayout.Toggle(FF9StateSystem.Battle.isTrance[FF9StateSystem.Battle.selectCharPosID], "Trance");
		if (isTrance != FF9StateSystem.Battle.isTrance[FF9StateSystem.Battle.selectCharPosID])
		{
		    BattleUnit character = btl_scrp.FindBattleUnit((UInt16)(1 << FF9StateSystem.Battle.selectCharPosID));
			FF9StateSystem.Battle.isTrance[FF9StateSystem.Battle.selectCharPosID] = isTrance;
			if (isTrance)
			{
				character.Trance = Byte.MaxValue;
			    character.AlterStatus(BattleStatus.Trance);
			}
			else
			{
				character.Trance = 0;
			    character.RemoveStatus(BattleStatus.Trance);
			}
		}
		if (GUILayout.Button("Attack"))
		{
			HonoluluBattleMain.playCommand(FF9StateSystem.Battle.selectCharPosID, 0, 16, isTrance);
		}
		else if (GUILayout.Button("Skill1"))
		{
			HonoluluBattleMain.playCommand(FF9StateSystem.Battle.selectCharPosID, 1, 16, isTrance);
		}
		else if (GUILayout.Button("Skill2"))
		{
			HonoluluBattleMain.playCommand(FF9StateSystem.Battle.selectCharPosID, 2, 16, isTrance);
		}
		else if (GUILayout.Button("Item"))
		{
			HonoluluBattleMain.playCommand(FF9StateSystem.Battle.selectCharPosID, 3, 16, isTrance);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("Box");
		GUILayout.Label("PlayerCount:" + FF9StateSystem.Battle.selectPlayerCount);
		if (GUILayout.Button("<"))
		{
			FF9StateSystem.Battle.selectPlayerCount--;
			if (FF9StateSystem.Battle.selectPlayerCount <= 0)
			{
				FF9StateSystem.Battle.selectPlayerCount = 4;
				ff9play.FF9Play_SetParty(0, 0);
				ff9play.FF9Play_SetParty(1, 1);
				ff9play.FF9Play_SetParty(2, 2);
				ff9play.FF9Play_SetParty(3, 3);
			}
			else
			{
				ff9play.FF9Play_SetParty(FF9StateSystem.Battle.selectPlayerCount, -1);
			}
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		if (GUILayout.Button(">"))
		{
			Int32 num = FF9StateSystem.Battle.selectPlayerCount++;
			if (FF9StateSystem.Battle.selectPlayerCount > 4)
			{
				FF9StateSystem.Battle.selectPlayerCount = 1;
				ff9play.FF9Play_SetParty(3, -1);
				ff9play.FF9Play_SetParty(2, -1);
				ff9play.FF9Play_SetParty(1, -1);
			}
			else
			{
				ff9play.FF9Play_SetParty(num, num);
			}
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("Box");
		GUILayout.Label("CharPosID:" + FF9StateSystem.Battle.selectCharPosID);
		if (GUILayout.Button("<"))
		{
			FF9StateSystem.Battle.selectCharPosID--;
			if (FF9StateSystem.Battle.selectCharPosID < 0)
			{
				FF9StateSystem.Battle.selectCharPosID = FF9StateSystem.Battle.selectPlayerCount - 1;
			}
		}
		if (GUILayout.Button(">"))
		{
			FF9StateSystem.Battle.selectCharPosID++;
			if (FF9StateSystem.Battle.selectCharPosID >= FF9StateSystem.Battle.selectPlayerCount)
			{
				FF9StateSystem.Battle.selectCharPosID = 0;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("Box");
		GUILayout.Label("CharModelID:" + FF9StateSystem.Common.FF9.party.member[FF9StateSystem.Battle.selectCharPosID].info.slot_no);
		if (GUILayout.Button("<"))
		{
			Int32 num2 = BattleUI.currentDebugSerialCharacter[FF9StateSystem.Battle.selectCharPosID];
			do
			{
				if (num2 != 0)
				{
					num2--;
				}
				else
				{
					num2 = 11;
				}
			}
			while (Array.IndexOf<Int32>(BattleUI.currentDebugSerialCharacter, num2) != -1);
			ff9play.FF9Dbg_SetCharacter(num2, FF9StateSystem.Battle.selectCharPosID);
			BattleUI.currentDebugSerialCharacter[FF9StateSystem.Battle.selectCharPosID] = num2;
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		if (GUILayout.Button(">"))
		{
			Int32 num3 = BattleUI.currentDebugSerialCharacter[FF9StateSystem.Battle.selectCharPosID];
			do
			{
				if (num3 < 12)
				{
					num3++;
				}
				else
				{
					num3 = 0;
				}
			}
			while (Array.IndexOf<Int32>(BattleUI.currentDebugSerialCharacter, num3) != -1);
			ff9play.FF9Dbg_SetCharacter(num3, FF9StateSystem.Battle.selectCharPosID);
			BattleUI.currentDebugSerialCharacter[FF9StateSystem.Battle.selectCharPosID] = num3;
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("Box");
		GUILayout.Label("WeaponID:" + this.battleMain.GetWeaponID(FF9StateSystem.Battle.selectCharPosID));
		if (GUILayout.Button("<"))
		{
			HonoluluBattleMain.CurPlayerWeaponIndex[FF9StateSystem.Battle.selectCharPosID]--;
			ff9feqp.FF9FEqp_Equip((Byte)FF9StateSystem.Battle.selectCharPosID, ref HonoluluBattleMain.CurPlayerWeaponIndex[FF9StateSystem.Battle.selectCharPosID]);
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		if (GUILayout.Button(">"))
		{
			HonoluluBattleMain.CurPlayerWeaponIndex[FF9StateSystem.Battle.selectCharPosID]++;
			ff9feqp.FF9FEqp_Equip((Byte)FF9StateSystem.Battle.selectCharPosID, ref HonoluluBattleMain.CurPlayerWeaponIndex[FF9StateSystem.Battle.selectCharPosID]);
			SceneDirector.Replace("BattleMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private Boolean isOpenBattleMapDebugPanel = true;

	private Boolean isOpenEffectDebugPanel;

	private Boolean isOpenAttackDebugPanel;

	private String patternIndex;

	private String battleMapString;

	private static Int32[] currentDebugSerialCharacter = new Int32[]
	{
		0,
		1,
		2,
		3
	};

	private HonoluluBattleMain battleMain;

	private BattleMapCameraController cameraController;

	public Int32 effNum;

	private String strEffNum;

	private GameObject sphere;

	private btlseq btlSeq;

	private GameObject goDebugMonRadius;

	private BTL_SCENE btlScene;

	private Int32 seqNo;

	private List<Int32> seqList;

	private Single elapsedTime;

	private Int32 mapIndex;

	private Int32[] mapIDs;

	private Int32 repIndex;

	private static Int32[] repTable = new Int32[]
	{
		3,
		3,
		250,
		199,
		8,
		212,
		326,
		229,
		0,
		250,
		26,
		212,
		3,
		326,
		229,
		26,
		8,
		212,
		3,
		231,
		0,
		231,
		0,
		212,
		231,
		0,
		326,
		326,
		231,
		0,
		92,
		326,
		250,
		57,
		407,
		312,
		57,
		407,
		312,
		199,
		326,
		326,
		250,
		57,
		212,
		326,
		312,
		199,
		250,
		3,
		199,
		92,
		231,
		0,
		312,
		57,
		38,
		231,
		0,
		212,
		326,
		229,
		0,
		312,
		199,
		8,
		212,
		326,
		326,
		231,
		3,
		57,
		212,
		326,
		312,
		326,
		57,
		8,
		229,
		3,
		326,
		57,
		212,
		231,
		3,
		0,
		250,
		57,
		38,
		250,
		3,
		199,
		407,
		312,
		26,
		407,
		250,
		199,
		8,
		212,
		326,
		312,
		57,
		3,
		229,
		57,
		92,
		231,
		326,
		57,
		8,
		212,
		229,
		57,
		8,
		326,
		3,
		3,
		326,
		229,
		199,
		3,
		326,
		326,
		250,
		8,
		0,
		326,
		3,
		312,
		26,
		92,
		326,
		3,
		250,
		326,
		57,
		3,
		326,
		229,
		3,
		199,
		8,
		326,
		231,
		326,
		199,
		326,
		38,
		250,
		57,
		3,
		3,
		229,
		0,
		326,
		312,
		57,
		231,
		3,
		57,
		38,
		250,
		326,
		199,
		38
	};

	private Vector2 scrollPosition;
}
