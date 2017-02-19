using System;
using System.Collections.Generic;
using Assets.Sources.Scripts.UI.Common;
using UnityEngine;
using Object = System.Object;

public class ETb
{
	public void InitMessage()
	{
		if (this.sInitMesInh)
		{
			this.sInitMesInh = false;
		}
		else
		{
			this.YWindow_CloseAll();
		}
		this.InitMesWin();
	}

	public void InitMesWin()
	{
		ETb.sChoose = 0;
		DialogManager.SelectChoice = ETb.sChoose;
		ETb.sChooseMaskInit = -1;
		ETb.sChooseInit = 0;
		this.gMesCount = 0;
		EventInput.IsNeedAddStartSignal = false;
	}

	public void InitMovieHitPoint(Int32 MapNo)
	{
		if (PersistenSingleton<UIManager>.Instance.FieldHUDScene.MovieHitArea.activeSelf)
		{
			global::Debug.LogWarning("InitMovieHitPoint() => FieldHUD.MovieHitArea has not been deactivated after played movie in MapNo. " + MapNo + " So, I will deactivate it.");
			PersistenSingleton<UIManager>.Instance.FieldHUDScene.MovieHitArea.SetActive(false);
		}
	}

	public void InhInitMes()
	{
		this.sInitMesInh = true;
	}

	public void InitKeyEvents()
	{
		ETb.sKey0 = 0u;
		this.sEvent0 = new ETbEvent();
		this.sEvent0.what = this.eventNull;
		this.sEvent0.msg = 0;
	}

	public void ProcessKeyEvents()
	{
		this.GenerateKeyEvent();
	}

	private void GenerateKeyEvent()
	{
		UInt32 num = this.PadReadE();
		ETb.sKeyOn = (num & ~ETb.sKey0);
		ETb.sKeyOff = (~num & ETb.sKey0);
		ETb.sKey0 = num;
	}

	public UInt32 PadReadE()
	{
		return this.getPad() & 67108863u;
	}

	private UInt32 getPad()
	{
		return EventInput.ReadInput();
	}

	public void NewMesWin(Int32 mes, Int32 num, Int32 flags, PosObj targetPo)
	{
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		if (this.IsSkipped(instance, mes, num, flags, targetPo))
		{
			return;
		}
		this.DisposWindowByID(num);
		Dialog.CaptionType captionType = Dialog.CaptionType.None;
		Dialog.WindowStyle windowStyle;
		if ((flags & 128) > 0)
		{
			windowStyle = Dialog.WindowStyle.WindowStyleAuto;
		}
		else
		{
			windowStyle = Dialog.WindowStyle.WindowStylePlain;
			if ((flags & 8) > 0)
			{
				captionType = Dialog.CaptionType.Mognet;
			}
			else if ((flags & 64) > 0)
			{
				captionType = Dialog.CaptionType.ActiveTimeEvent;
			}
		}
		if (windowStyle == Dialog.WindowStyle.WindowStylePlain)
		{
			targetPo = (PosObj)null;
		}
		if ((flags & 16) > 0)
		{
			windowStyle = Dialog.WindowStyle.WindowStyleTransparent;
		}
		else if ((flags & 4) > 0)
		{
			windowStyle = Dialog.WindowStyle.WindowStyleNoTail;
		}
		if ((flags & 1) <= 0)
		{
			ETb.sChoose = ETb.sChooseInit;
			ETb.sChooseInit = 0;
		}
		if (instance.gMode == 3)
		{
			targetPo = (PosObj)null;
			if (mes != 40)
			{
				if (mes == 41)
				{
					EIcon.ShowDialogBubble(true);
				}
			}
			else
			{
				EIcon.ShowDialogBubble(false);
			}
		}
		EventHUD.CheckSpecialHUDFromMesId(mes, true);
		Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(num, windowStyle, mes, targetPo, new Dialog.DialogIntDelegate(this.OnDialogFinish), captionType);
		if (FF9StateSystem.Common.FF9.fldMapNo == 1657 && (mes == 166 || mes == 183))
		{
			dialog.FocusToActor = false;
		}
		if (dialog == (UnityEngine.Object)null)
		{
			return;
		}
		if ((flags & 32) > 0)
		{
			dialog.FocusToActor = false;
		}
		if (ETb.isMessageDebug)
		{
			global::Debug.Log(String.Concat(new Object[]
			{
				"NewMesWin => sid:",
				instance.gCur.sid,
				", mes: ",
				mes,
				", field id:",
				FF9TextTool.FieldZoneId,
				", num: ",
				num,
				", flags: ",
				flags,
				", text:",
				dialog.Phrase
			}));
		}
		this.gMesCount++;
		EIcon.SetHereIcon(0);
		String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
		EMinigame.EidolonMuralAchievement(currentLanguage, mes);
		EMinigame.ExcellentLuckColorFortuneTellingAchievement(currentLanguage, mes);
		EMinigame.ProvokeMogAchievement(currentLanguage, mes);
		EMinigame.JumpingRopeAchievement(currentLanguage, mes);
		EMinigame.GetRewardFromQueenStellaAchievement();
		EMinigame.ShuffleGameAchievement(currentLanguage, mes);
		EMinigame.ChocoboBeakLV99Achievement(currentLanguage, mes);
		EMinigame.AtleteQueenAchievement_Debug(currentLanguage, mes);
		EMinigame.TreasureHunterSAchievement(currentLanguage, mes);
		ETb.FixChocoAccidenlyFly(dialog);
	}

	public void OnDialogFinish(Int32 choice)
	{
		if (choice > -1)
		{
		}
	}

	public Boolean MesWinActive(Int32 num)
	{
		return Singleton<DialogManager>.Instance.CheckDialogShowing(num);
	}

	public static UInt32 KeyOn()
	{
		return ETb.sKeyOn & 67108863u;
	}

	public static UInt32 KeyOff()
	{
		return ETb.sKeyOff & 67108863u;
	}

	public Int32 GetPartyMember(Int32 index)
	{
		FF9StateGlobal ff = FF9StateSystem.Common.FF9;
		PLAYER player = ff.party.member[index];
		return (Int32)((player == null) ? -1 : ((Int32)player.info.slot_no));
	}

	public void YWindow_CloseAll()
	{
		Singleton<DialogManager>.Instance.CloseAll();
	}

	public void DisposWindowByID(Int32 p)
	{
		Singleton<DialogManager>.Instance.Close(p);
	}

	public void RaiseAllWindow()
	{
		Singleton<DialogManager>.Instance.RiseAll();
	}

	public void SetMesValue(Int32 n, Int32 v)
	{
		if (n >= 0 && n < 8)
		{
			this.gMesValue[n] = v;
		}
	}

	public static String GetItemName(Int32 index)
	{
		if (index < EventEngine.kSItemOfs)
		{
			return FF9TextTool.ItemName(index);
		}
		if (index < EventEngine.kCItemOfs)
		{
			return FF9TextTool.ImportantItemName(index - EventEngine.kSItemOfs);
		}
		return FF9TextTool.CardName(index - EventEngine.kCItemOfs);
	}

	public void SetChooseParam(Int32 mask, Int32 pos)
	{
		ETb.sChooseMaskInit = mask;
		Int32 num = -1;
		while (pos >= 0 && mask > 0)
		{
			num += (mask & 1);
			pos--;
			mask >>= 1;
		}
		ETb.sChooseInit = (Int32)((num < 0) ? 0 : num);
	}

	public Int32 GetChoose()
	{
		ETb.sChoose = DialogManager.SelectChoice;
		if (ETb.isMessageDebug)
		{
			global::Debug.Log("Event choice value:" + ETb.sChoose);
		}
		return ETb.sChoose;
	}

	public String GetStringFromTable(UInt32 bank, UInt32 index)
	{
		String result = String.Empty;
		if (bank < 4u && index < 8u)
		{
			String[] tableText = FF9TextTool.GetTableText(bank);
			if (tableText != null)
			{
				Int32 num = this.gMesValue[(Int32)((UIntPtr)index)];
				if (num < (Int32)tableText.Length)
				{
					result = tableText[num];
				}
			}
		}
		return result;
	}

	private Boolean IsSkipped(EventEngine eventEngine, Int32 mes, Int32 num, Int32 flags, PosObj targetPo)
	{
		if (eventEngine.gMode == 1)
		{
			Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
			if (fldMapNo == 1652)
			{
				String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
			    switch (currentLanguage)
			    {
                    case "Japanese":
                        return mes == 146;
                    case "French":
                        return mes == 144;

                }
			    
				return mes == 142;
			}
			if (fldMapNo == 1659)
			{
				if (targetPo.sid == 1)
				{
					Dialog dialogByTextId = Singleton<DialogManager>.Instance.GetDialogByTextId(mes);
					return dialogByTextId != (UnityEngine.Object)null;
				}
			}
			else if (fldMapNo == 2209 && targetPo.sid == 9)
			{
				return mes == 393;
			}
		}
		return false;
	}

	public static void World2Screen(Vector3 v, out Single x, out Single y)
	{
		FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
		Camera mainCamera = fieldmap.GetMainCamera();
		BGCAM_DEF currentBgCamera = fieldmap.GetCurrentBgCamera();
		Vector3 vector = PSX.CalculateGTE_RTPT(v, Matrix4x4.identity, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), fieldmap.GetProjectionOffset());
		Vector2 cameraOffset = fieldmap.GetCameraOffset();
		Single num = vector.x - cameraOffset.x;
		Single num2 = vector.y - cameraOffset.y;
		ETb.ConvertGTEToUIScreenPosition(ref num, ref num2);
		x = num;
		y = num2;
	}

	public static void ConvertGTEToUIScreenPosition(ref Single x, ref Single y)
	{
		x = x * UIManager.ResourceXMultipier + UIManager.UIContentSize.x / 2f;
		y = y * UIManager.ResourceYMultipier + UIManager.UIContentSize.y / 2f;
	}

	public static void GetMesPos(PosObj po, out Single x, out Single y)
	{
		x = 0f;
		y = 0f;
		if (po.go == (UnityEngine.Object)null)
		{
			return;
		}
		Vector3 vector = new Vector3(po.pos[0], po.pos[1], po.pos[2]);
		Vector3 zero = Vector3.zero;
		zero.x = vector.x;
		Byte scaley = po.scaley;
		Single num = (Single)(-po.eye * (Int16)scaley >> 6);
		zero.y = vector.y + num + 50f;
		zero.z = vector.z;
		if (po.cid == 4)
		{
			Actor actor = (Actor)po;
			zero.x += (Single)actor.mesofsX;
			zero.y -= (Single)actor.mesofsY;
			zero.z += (Single)actor.mesofsZ;
		}
		ETb.World2Screen(zero, out x, out y);
		y = UIManager.UIContentSize.y - y;
	}

	public static void SndMove()
	{
		if (RealTime.time - ETb.lastPlaySound < 0.01f)
		{
			return;
		}
		ETb.lastPlaySound = RealTime.time;
		FF9Sfx.FF9SFX_Play(103);
	}

	public static void SndCancel()
	{
		if (RealTime.time - ETb.lastPlaySound < 0.01f)
		{
			return;
		}
		ETb.lastPlaySound = RealTime.time;
		FF9Sfx.FF9SFX_Play(101);
	}

	public static void SndOK()
	{
		ETb.SndMove();
	}

	public static void SndConfirm(UIScene scene)
	{
		Type type = scene.GetType();
		if (type != typeof(FieldHUD) && type != typeof(WorldHUD))
		{
			ETb.SndOK();
		}
	}

	public static void SndCancel(UIScene scene)
	{
		Type type = scene.GetType();
		if (type != typeof(FieldHUD) && type != typeof(WorldHUD))
		{
			ETb.SndCancel();
		}
	}

	public static void ProcessWorldDialog(Dialog dialog)
	{
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		if (instance == (UnityEngine.Object)null)
		{
			return;
		}
		if (instance.gMode == 3)
		{
			if (dialog.TextId == 40 || dialog.TextId == 41)
			{
				EIcon.HideDialogBubble();
			}
			ETb.CheckVehicleTutorial(dialog);
		}
	}

	public static void ProcessATEDialog(Dialog dialog)
	{
		if (dialog.CapType == Dialog.CaptionType.ActiveTimeEvent)
		{
			global::Debug.Log(String.Concat(new Object[]
			{
				"ProcessATBDialog: DialogManager.SelectChoice ",
				DialogManager.SelectChoice,
				", numOfChoices ",
				dialog.ChoiceNumber
			}));
			Boolean isCompulsory = false;
			if (ETb.LastATEDialogID == -1 && dialog.Id == 0)
			{
				isCompulsory = true;
			}
			if (dialog.Id != 1 || DialogManager.SelectChoice != dialog.ChoiceNumber - 1 || dialog.ChoiceNumber <= 0)
			{
				if (dialog.Id != 0 || ETb.LastATEDialogID != 1)
				{
					Int32 num = EMinigame.MappingATEID(dialog, DialogManager.SelectChoice, isCompulsory);
					EMinigame.ATE80Achievement(num);
					global::Debug.Log("ATEID = " + num);
				}
			}
			ETb.LastATEDialogID = dialog.Id;
			if (FF9StateSystem.Common.FF9.fldLocNo == 40 && FF9StateSystem.Common.FF9.fldMapNo == 206 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 1900)
			{
				if (DialogManager.SelectChoice == 1)
				{
					ETb.LastATEDialogID = -1;
				}
			}
			else if (dialog.Id == 0)
			{
				ETb.LastATEDialogID = -1;
			}
		}
	}

	private static void CheckVehicleTutorial(Dialog dialog)
	{
		if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD && UIManager.World.CurrentState != WorldHUD.State.FullMap)
		{
			switch (dialog.TextId)
			{
			case 70:
			case 71:
			case 72:
			case 73:
			case 74:
			case 75:
				EventInput.IsDialogConfirm = true;
				UIManager.World.ForceShowButton();
				break;
			}
		}
	}

	private static void FixChocoAccidenlyFly(Dialog dialog)
	{
		if (PersistenSingleton<EventEngine>.Instance.gMode == 3 && EventCollision.IsChocoboWalkingInForestArea() && (dialog.TextId == 54 || dialog.TextId == 55 || dialog.TextId == 56 || dialog.TextId == 57 || dialog.TextId == 58 || dialog.TextId == 59 || dialog.TextId == 60 || dialog.TextId == 61 || dialog.TextId == 62))
		{
			PersistenSingleton<UIManager>.Instance.Dialogs.EnableCollider(false);
		}
	}

	public const Int32 kMesValueN = 8;

	public const Int32 kMesOfsY = 50;

	public const Int32 kMesItem = 14;

	public const Int32 kMesValue = 64;

	public const Int32 kMesPlayer0 = 16;

	public const Int32 kMesPlayer1 = 17;

	public const Int32 kMesPlayer2 = 18;

	public const Int32 kMesPlayer3 = 19;

	public const Int32 kMesPlayer4 = 20;

	public const Int32 kMesPlayer5 = 21;

	public const Int32 kMesPlayer6 = 22;

	public const Int32 kMesPlayer7 = 23;

	public const Int32 kMesParty0 = 24;

	public const Int32 kMesParty1 = 25;

	public const Int32 kMesParty2 = 26;

	public const Int32 kMesParty3 = 27;

	public const Int32 kMesString = 6;

	public const Int32 kMesNew = 112;

	public const Int32 kStringTableN = 4;

	public const Int32 winMOG = 8;

	public const Int32 winATE = 64;

	public const Int32 WindowChatStyle = 128;

	public const Int32 WindowTransparentStyle1 = 32;

	public const Int32 WindowTransparentStyle2 = 16;

	public const Int32 WindowChatStyleWithoutTail = 4;

	public const Int32 WindowNotFollowActor = 32;

	public const Int32 EnterTownMesId = 40;

	public const Int32 EnterDungeonMesId = 41;

	public const Int32 ResetChooseMask = 1;

	private static readonly Boolean isMessageDebug;

	private Boolean sInitMesInh;

	public Int32 eventNull;

	public Int32 eventKeyDown = 1;

	public Int32 eventAutoKey = 2;

	public Int32 eventKeyUp = 4;

	public static UInt32 sKey0;

	public static UInt32 sKeyOn;

	public static UInt32 sKeyOff;

	private ETbEvent sEvent0;

	public Int32 gMesCount;

	public Int32[] gMesValue;

	public static Int32 gMesSignal;

	public static Int32 sChoose;

	public static Int32 sChooseMaskInit = -1;

	public static Int32 sChooseInit;

	public static Int32 sChooseMask = -1;

	private static Single lastPlaySound;

	private static Int32 LastATEDialogID = -1;
}
