using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;
using Assets.Scripts.Common;
using UnityEngine;
using Object = System.Object;

public class WorldHUD : UIScene
{
	public WorldHUD.State CurrentState
	{
		get
		{
			return this.currentState;
		}
	}

	public Int32 CurrentCharacterStateIndex
	{
		get
		{
			return this.currentCharacterStateIndex;
		}
		set
		{
			this.currentCharacterStateIndex = value;
		}
	}

	public Boolean EnableMapButton
	{
		set
		{
			this.enableMapButton = value;
		}
	}

	public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
	{
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
		{
			this.currentCharacterStateIndex = -1;
			ButtonGroupState.HelpEnabled = false;
			this.PauseButtonGameObject.SetActive(PersistenSingleton<UIManager>.Instance.IsPauseControlEnable && FF9StateSystem.MobilePlatform);
			this.locationName = FF9TextTool.GetTableText(0u);
			if (this.currentState == WorldHUD.State.FullMap)
			{
				if (PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.FieldHUD)
				{
					this.ForceCloseFullMap();
				}
				else
				{
					if (!this.ignorePointerProcess)
					{
						this.DisplayLocationName(this.currentLocationIndex);
					}
					if (FF9StateSystem.MobilePlatform)
					{
						base.StartCoroutine(this.ForceOpenVirtual());
					}
				}
			}
			else if (this.currentState == WorldHUD.State.HUDNoMiniMap && this.MiniMapPanel.activeSelf)
			{
				this.currentState = WorldHUD.State.HUD;
				this.MapButtonGameObject.SetActive(false);
			}
			if (PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.Pause && this.currentState != WorldHUD.State.FullMap && PersistenSingleton<EventEngine>.Instance.GetUserControl())
			{
				base.StartCoroutine(this.DisableEventInputForAWhile());
			}
		};
		if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		base.Show(sceneVoidDelegate);
		PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
		PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(ff9.GetUserControl());
		PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(ff9.GetUserControl(), (Action)null);
		this.InitialHUD();
		this.DisplayButtonHud();
		this.DisplayChocographLocation(true);
		VirtualAnalog.Init(base.gameObject);
		VirtualAnalog.FallbackTouchWidgetList.Add(PersistenSingleton<UIManager>.Instance.gameObject);
		VirtualAnalog.FallbackTouchWidgetList.Add(PersistenSingleton<UIManager>.Instance.Dialogs.gameObject);
		VirtualAnalog.FallbackTouchWidgetList.Add(PersistenSingleton<UIManager>.Instance.Booster.OutsideBoosterHitPoint);
		VirtualAnalog.FallbackTouchWidgetList.Add(this.moveLeftGameObject);
		VirtualAnalog.FallbackTouchWidgetList.Add(this.moveRightGameObject);
		VirtualAnalog.FallbackTouchWidgetList.Add(this.mapWidget.gameObject);
		PersistenSingleton<UIManager>.Instance.SetGameCameraEnable(true);
	}

	public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
	{
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
		{
			if (!base.NextSceneIsModal)
			{
				PersistenSingleton<UIManager>.Instance.SetGameCameraEnable(false);
			}
		};
		if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		base.Hide(sceneVoidDelegate);
		this.PauseButtonGameObject.SetActive(false);
		PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, (Action)null);
		PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
	}

	public void onMouseMove(Vector2 delta)
	{
		if (FF9StateSystem.PCPlatform && this.controlPointEnable && HonoInputManager.MouseEnabled && !this.ignorePointerProcess)
		{
			Vector2 v = Input.mousePosition;
			Camera camera = NGUITools.FindCameraForLayer(base.gameObject.layer);
			Vector3 v2 = camera.ScreenToWorldPoint(v);
			Vector3 vector = base.transform.worldToLocalMatrix.MultiplyPoint3x4(v2);
			vector.x += (Single)this.mapWidget.width / 2f;
			vector.y += (Single)this.mapWidget.height / 2f;
			vector.x -= (Single)this.mapPointerWidget.width / 2f - 8f;
			vector.y -= 14f;
			vector.x = Mathf.Clamp(vector.x, this.FullMapBorder.x, this.FullMapBorder.y);
			vector.y = Mathf.Clamp(vector.y, this.FullMapBorder.z, this.FullMapBorder.w);
			Int32 num = this.DetectLocation(vector);
			if (this.currentLocationIndex != num)
			{
				this.currentLocationIndex = num;
				this.DisplayLocationName(this.currentLocationIndex);
			}
			if (num != -1)
			{
				vector = this.GetPositionToSnap(this.currentLocationIndex);
			}
			this.mapPointerWidget.transform.localPosition = vector;
		}
	}

	public override Boolean OnKeyConfirm(GameObject go)
	{
		if (base.OnKeyConfirm(go) && this.currentState == WorldHUD.State.FullMap)
		{
			BubbleUI instance = Singleton<BubbleUI>.Instance;
			Boolean flag;
			if (FF9StateSystem.MobilePlatform)
			{
				SourceControl source = PersistenSingleton<HonoInputManager>.Instance.GetSource(Control.Confirm);
				flag = (instance.helpButton.gameObject.Equals(go) || instance.primaryButton.Equals(go) || source == SourceControl.Joystick || source == SourceControl.KeyBoard);
			}
			else
			{
				flag = true;
			}
			if (flag && this.currentLocationIndex != -1 && this.CheckUsingAirShip() && !Singleton<VirtualAnalog>.Instance.IsShown && !this.ignorePointerProcess)
			{
				this.ProcessTouchedLocation();
			}
		}
		return true;
	}

	public override Boolean OnKeyCancel(GameObject go)
	{
		if (base.OnKeyCancel(go) && this.currentCharacterStateIndex == 0 && this.currentState != WorldHUD.State.FullMap && !Singleton<BubbleUI>.Instance.beachButton.gameObject.activeInHierarchy && UIManager.Input.ContainsAndroidQuitKey())
		{
			UIManager.Input.OnQuitCommandDetected();
		}
		return true;
	}

	public override Boolean OnKeyMenu(GameObject go)
	{
		if (base.OnKeyMenu(go))
		{
			if (this.currentState == WorldHUD.State.HUD || this.currentState == WorldHUD.State.HUDNoMiniMap)
			{
				if (PersistenSingleton<UIManager>.Instance.Dialogs.Visible && !EventCollision.IsRidingChocobo())
				{
					PersistenSingleton<UIManager>.Instance.HideAllHUD();
				}
				if (this.currentCharacterStateIndex == 0)
				{
					this.Hide(delegate
					{
						PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.MainMenu);
					});
				}
			}
			else if (this.currentState == WorldHUD.State.FullMap)
			{
			}
		}
		return true;
	}

	public override Boolean OnKeyPause(GameObject go)
	{
		if (base.OnKeyPause(go))
		{
			base.NextSceneIsModal = true;
			this.Hide(delegate
			{
				PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Pause);
			});
		}
		return true;
	}

	public override Boolean OnKeyLeftBumper(GameObject go)
	{
		if (base.OnKeyLeftBumper(go) && this.currentState == WorldHUD.State.FullMap && !this.ignorePointerProcess)
		{
			this.FindNextLocation(true);
			this.DisplayLocationName(this.currentLocationIndex);
			this.pointerPosition = this.SnapPointerToLocation(this.currentLocationIndex);
		}
		return true;
	}

	public override Boolean OnKeyRightBumper(GameObject go)
	{
		if (base.OnKeyRightBumper(go) && this.currentState == WorldHUD.State.FullMap && !this.ignorePointerProcess)
		{
			this.FindNextLocation(false);
			this.DisplayLocationName(this.currentLocationIndex);
			this.pointerPosition = this.SnapPointerToLocation(this.currentLocationIndex);
		}
		return true;
	}

	public override Boolean OnKeySelect(GameObject go)
	{
		if (base.OnKeySelect(go))
		{
			if (ff9.w_moveMogPtr != (UnityEngine.Object)null && PersistenSingleton<EventEngine>.Instance.objIsVisible(ff9.w_moveMogPtr.originalActor))
			{
				return false;
			}
            if (!PersistenSingleton<SceneDirector>.Instance.PendingNextScene.Equals(string.Empty) || !PersistenSingleton<SceneDirector>.Instance.PendingCurrentScene.Equals(string.Empty))
            {
                return false;
            }
            if (this.isSelectEnable)
			{
				if (!FF9StateSystem.Battle.isEncount && this.currentState == WorldHUD.State.HUD && this.enableMapButton)
				{
					this.currentState = WorldHUD.State.FullMap;
					PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, delegate
					{
						PersistenSingleton<HonoInputManager>.Instance.SetVirtualAnalogEnable(true);
					});
					ff9.w_naviMode = 2;
					this.currentLocationIndex = -1;
					this.SetButtonVisible(false);
					this.MiniMapPanel.SetActive(false);
					this.FullMapPanel.SetActive(true);
					this.mapBackButtonGameObject.SetActive(FF9StateSystem.MobilePlatform);
					this.mapHelpButtonGameObject.SetActive(FF9StateSystem.MobilePlatform);
					this.mapRightBumperGameObject.SetActive(FF9StateSystem.MobilePlatform);
					this.mapLeftBumperGameObject.SetActive(FF9StateSystem.MobilePlatform);
					this.SetupBubble();
					this.ShowMapPointer(true);
					this.DisplayFullmap();
					Singleton<DialogManager>.Instance.CloseAll();
				}
				else if (this.currentState == WorldHUD.State.FullMap)
				{
					this.currentState = WorldHUD.State.HUDNoMiniMap;
					PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, (Action)null);
					ff9.w_naviMode = 0;
					this.SetButtonVisible(true);
					this.FullMapPanel.SetActive(false);
					UIManager.Input.ResetKeyCode();
					this.MapButtonGameObject.SetActive(FF9StateSystem.MobilePlatform);
					this.FallbackBubbleToEvent();
					this.DisplayChocographLocation(false);
				}
				else if (this.currentState == WorldHUD.State.HUDNoMiniMap && this.enableMapButton)
				{
					this.currentState = WorldHUD.State.HUD;
					PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, (Action)null);
					ff9.w_naviMode = 1;
					this.SetButtonVisible(true);
					this.MiniMapPanel.SetActive(true);
					this.MapButtonGameObject.SetActive(false);
					this.DisplayChocographLocation(true);
				}
			}
		}
		return true;
	}

	public void OnPressButton(GameObject go, Boolean isPress)
	{
		if (!VirtualAnalog.HasInput())
		{
			if (isPress)
			{
				PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, (Action)null);
				if (go == this.moveLeftGameObject)
				{
					this.moveRightGameObject.GetComponent<UIButton>().enabled = false;
					this.moveRightGameObject.GetComponent<BoxCollider>().enabled = false;
				}
				else if (go == this.moveRightGameObject)
				{
					this.moveLeftGameObject.GetComponent<UIButton>().enabled = false;
					this.moveLeftGameObject.GetComponent<BoxCollider>().enabled = false;
				}
			}
			else
			{
				PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, (Action)null);
				if (go == this.moveLeftGameObject)
				{
					this.moveRightGameObject.GetComponent<UIButton>().enabled = true;
					this.moveRightGameObject.GetComponent<BoxCollider>().enabled = true;
					this.moveRightGameObject.GetComponent<UIButtonColor>().SetState(UIButtonColor.State.Normal, true);
				}
				else if (go == this.moveRightGameObject)
				{
					this.moveLeftGameObject.GetComponent<UIButton>().enabled = true;
					this.moveLeftGameObject.GetComponent<BoxCollider>().enabled = true;
					this.moveLeftGameObject.GetComponent<UIButtonColor>().SetState(UIButtonColor.State.Normal, true);
				}
			}
		}
	}

	private void OnFullMapHover(GameObject go, Boolean isHover)
	{
		if (this.currentState == WorldHUD.State.FullMap)
		{
			if (isHover)
			{
				this.controlPointEnable = true;
			}
			else
			{
				this.controlPointEnable = false;
			}
		}
	}

	private void OnFullMapNavigator(GameObject go, KeyCode key)
	{
	}

	private void MovePointer()
	{
		if ((HonoInputManager.JoystickEnabled || HonoInputManager.VirtualAnalogEnabled) && PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD)
		{
			if (this.ignorePointerProcess)
			{
				return;
			}
			this.inputAxis = PersistenSingleton<HonoInputManager>.Instance.GetAxis();
			if (this.inputAxis.magnitude > this.directionAnalogThreshold)
			{
				this.pointerPosition += this.inputAxis * this.pointerSpeed * Time.deltaTime;
				UICamera.EventWaitTime = Time.deltaTime;
				if (this.pointerPosition.x > this.FullMapBorder.y)
				{
					this.pointerPosition.x = this.FullMapBorder.x;
				}
				else if (this.pointerPosition.x < this.FullMapBorder.x)
				{
					this.pointerPosition.x = this.FullMapBorder.y;
				}
				if (this.pointerPosition.y > this.FullMapBorder.w)
				{
					this.pointerPosition.y = this.FullMapBorder.z;
				}
				else if (this.pointerPosition.y < this.FullMapBorder.z)
				{
					this.pointerPosition.y = this.FullMapBorder.w;
				}
				Vector3 positionToSnap = this.pointerPosition;
				Int32 num = this.DetectLocation(positionToSnap);
				if (this.currentLocationIndex != num)
				{
					this.currentLocationIndex = num;
					this.DisplayLocationName(this.currentLocationIndex);
				}
				if (num != -1)
				{
					positionToSnap = this.GetPositionToSnap(this.currentLocationIndex);
				}
				this.mapPointerWidget.transform.localPosition = positionToSnap;
			}
		}
	}

	private Int32 DetectLocation(Vector3 onScreenPosition)
	{
		Int32 result = -1;
		Single num = Single.PositiveInfinity;
		foreach (WorldHUD.LocationPostionHudData locationPostionHudData in this.locationPointerList)
		{
			Single num2 = locationPostionHudData.DistanceTo(onScreenPosition - this.referencePosition);
			if (num2 < num)
			{
				num = num2;
				Int32 siblingIndex = locationPostionHudData.gameobject.transform.GetSiblingIndex();
			}
			if (num2 < this.snapRadius)
			{
				result = locationPostionHudData.gameobject.transform.GetSiblingIndex();
			}
		}
		return result;
	}

	private void OnLocationHover(GameObject go, Boolean isHover)
	{
		if (FF9StateSystem.PCPlatform && HonoInputManager.MouseEnabled && PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD && !this.ignorePointerProcess && isHover)
		{
			Int32 siblingIndex = go.transform.GetSiblingIndex();
			if (this.currentLocationIndex != siblingIndex)
			{
				this.currentLocationIndex = siblingIndex;
				this.DisplayLocationName(siblingIndex);
				this.SnapPointerToLocation(siblingIndex);
			}
		}
	}

	private void OnLocaltionClick(GameObject go)
	{
		if (this.currentState == WorldHUD.State.FullMap && (FF9StateSystem.MobilePlatform || FF9StateSystem.PCPlatform))
		{
			Int32 num = 0;
			Int32 num2 = -1;
			foreach (WorldHUD.LocationPostionHudData locationPostionHudData in this.locationPointerList)
			{
				if (locationPostionHudData.gameobject.transform.position.Equals(go.transform.position))
				{
					num2 = num;
					break;
				}
				num++;
			}
			if (num2 != -1 && this.currentLocationIndex != num2)
			{
				this.currentLocationIndex = num2;
				this.mapPointerWidget.transform.localPosition = this.GetPositionToSnap(this.currentLocationIndex);
				this.DisplayLocationName(this.currentLocationIndex);
			}
		}
	}

	private void onConfirmNavi(Int32 choice)
	{
		if (choice == 0)
		{
			ff9.w_frameAutoid = (Int32)((Byte)this.navigateLocationIndex);
			ff9.w_frameSetParameter(34, 0);
			ff9.w_naviMode = 0;
			this.currentState = WorldHUD.State.HUD;
			this.SetButtonVisible(true);
			this.FullMapPanel.SetActive(false);
			UIManager.Input.ResetKeyCode();
			this.MiniMapPanel.SetActive(true);
			this.FallbackBubbleToEvent();
			this.DisplayChocographLocation(true);
		}
		else
		{
			this.ShowMapPointer(true);
		}
		EventInput.IsProcessingInput = true;
	}

	private void SetupBubble()
	{
		EIcon.IsProcessingFIcon = false;
		PersistenSingleton<EventEngine>.Instance.SetUserControl(false);
		PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(9173, 1);
		Singleton<BubbleUI>.Instance.SetGameObjectActive(false);
	}

	private void FallbackBubbleToEvent()
	{
		EIcon.IsProcessingFIcon = true;
		PersistenSingleton<EventEngine>.Instance.SetUserControl(true);
		PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(9173, 0);
		Singleton<BubbleUI>.Instance.SetGameObjectActive(false);
		PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, (Action)null);
		PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(true);
	}

	public void ShowMapPointer(Boolean isActive)
	{
		this.ignorePointerProcess = !isActive;
		this.mapPointerWidget.gameObject.SetActive(isActive);
		this.mapBackButtonGameObject.SetActive(isActive && FF9StateSystem.MobilePlatform);
		this.mapHelpButtonGameObject.SetActive(isActive && FF9StateSystem.MobilePlatform);
		this.mapRightBumperGameObject.SetActive(isActive && FF9StateSystem.MobilePlatform);
		this.mapLeftBumperGameObject.SetActive(isActive && FF9StateSystem.MobilePlatform);
		if (!isActive)
		{
			this.DisplayLocationName(-1);
		}
		else
		{
			this.DisplayLocationName(this.currentLocationIndex);
		}
	}

	private Int32 GetConfirmDialogStartSize()
	{
		String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
		switch (currentLanguage)
		{
		case "Japanese":
			return 70;
		case "English(UK)":
		case "English(US)":
		case "Italian":
			return 53;
		case "Spanish":
			return 85;
		}
		return 77;
	}

	private void FindNextLocation(Boolean isPressedLeftBumper)
	{
		if (this.locationPointerList.Count == 1)
		{
			this.currentLocationIndex = 0;
		}
		else
		{
			Vector3 vector = (this.currentLocationIndex == -1) ? Vector3.zero : this.locationPointerList[this.currentLocationIndex].gameobject.transform.position;
			Vector3 rhs = vector;
			Int32 num = 0;
			while (vector == rhs)
			{
				if (isPressedLeftBumper)
				{
					if (this.currentLocationIndex >= this.locationPointerList.Count - 1)
					{
						this.currentLocationIndex = 0;
					}
					else
					{
						this.currentLocationIndex++;
					}
				}
				else if (this.currentLocationIndex <= 0)
				{
					this.currentLocationIndex = this.locationPointerList.Count - 1;
				}
				else
				{
					this.currentLocationIndex--;
				}
				rhs = this.locationPointerList[this.currentLocationIndex].gameobject.transform.position;
				num++;
				if (num >= this.locationPointerList.Count)
				{
					break;
				}
			}
		}
	}

	private void ProcessTouchedLocation()
	{
		Int32 num = this.locationPointerList[this.currentLocationIndex].locationId;
		this.navigateLocationIndex = num;
		if (num == 63)
		{
			num = 49;
		}
		this.ShowMapPointer(false);
		String text = Localization.Get("NavigateDialog");
		String text2 = this.locationName[num + 1];
		text = text.Replace("{0}", text2);
		Int32 fontSize = this.mapLocationLabel.fontSize;
		this.mapLocationLabel.fontSize = 50;
		Single textWidthFromFF9Font = NGUIText.GetTextWidthFromFF9Font(this.mapLocationLabel, text2);
		this.mapLocationLabel.fontSize = fontSize;
		Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(String.Concat(new Object[]
		{
			"[",
			NGUIText.StartSentense,
			"=",
			Convert.ToInt32(textWidthFromFF9Font + (Single)this.GetConfirmDialogStartSize()),
			",3][",
			NGUIText.Center,
			"=0]",
			text
		}), 0, 0, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, Vector2.zero, Dialog.CaptionType.None);
		dialog.AfterDialogHidden = new Dialog.DialogIntDelegate(this.onConfirmNavi);
		EventInput.IsProcessingInput = false;
	}

	private IEnumerator ForceOpenVirtual()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		PersistenSingleton<HonoInputManager>.Instance.SetVirtualAnalogEnable(true);
		yield break;
	}

	private void ForceCloseFullMap()
	{
		this.currentState = WorldHUD.State.HUD;
		ff9.w_naviMode = 1;
		this.SetButtonVisible(true);
		this.FullMapPanel.SetActive(false);
		UIManager.Input.ResetKeyCode();
	}

	public void InitialHUD()
	{
		String[] spriteInfo;
		if (WMUIData.ActiveMapNo == 1)
		{
			Sprite sprite = AssetManager.Load<Sprite>("EmbeddedAsset/UI/Sprites/world_map_full_all", out spriteInfo, false);
			this.miniMapSprite.spriteName = "world_map_mini_all";
			this.miniMapButton.normalSprite = "world_map_mini_all";
			this.mapSprite.sprite2D = sprite;
			this.mapButton.normalSprite2D = sprite;
		}
		else
		{
			Sprite sprite2 = AssetManager.Load<Sprite>("EmbeddedAsset/UI/Sprites/world_map_full_mistcontinent", out spriteInfo, false);
			this.miniMapSprite.spriteName = "world_map_mini_mistcontinent";
			this.miniMapButton.normalSprite = "world_map_mini_mistcontinent";
			this.mapSprite.sprite2D = sprite2;
			this.mapButton.normalSprite2D = sprite2;
		}
	}

	public void DisplayButtonHud()
	{
		if (FF9StateSystem.MobilePlatform)
		{
			switch (this.currentCharacterStateIndex)
			{
			case 0:
				FF9UIDataTool.DisplayTextLocalize(this.menuButtonLabelGameObject, "Menu");
				if (!this.isTitleShowing)
				{
					this.specialButtonSprite.spriteName = "button_mog";
					this.specialButton.KeyCommand = Control.Special;
					this.SpecialButtonGameObject.SetActive(true);
				}
				this.RotationLockButtonGameObject.SetActive(true);
				this.PerspectiveButtonGameObject.SetActive(true);
				this.CommonButtonPanel.SetActive(true);
				this.ChocoboButtonPanel.SetActive(false);
				this.PlaneButtonPanel.SetActive(false);
				break;
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
				FF9UIDataTool.DisplayTextLocalize(this.menuButtonLabelGameObject, "Menu");
				this.specialButtonSprite.spriteName = "button_dismount";
				this.specialButton.KeyCommand = Control.Cancel;
				this.SpecialButtonGameObject.SetActive(true);
				this.RotationLockButtonGameObject.SetActive(true);
				this.PerspectiveButtonGameObject.SetActive(true);
				this.CommonButtonPanel.SetActive(true);
				this.ChocoboButtonPanel.SetActive(true);
				this.PlaneButtonPanel.SetActive(false);
				break;
			case 6:
				FF9UIDataTool.DisplayTextLocalize(this.menuButtonLabelGameObject, "Menu");
				this.SpecialButtonGameObject.SetActive(false);
				this.RotationLockButtonGameObject.SetActive(true);
				this.PerspectiveButtonGameObject.SetActive(false);
				this.CommonButtonPanel.SetActive(true);
				this.ChocoboButtonPanel.SetActive(false);
				this.PlaneButtonPanel.SetActive(true);
				break;
			case 7:
				FF9UIDataTool.DisplayTextLocalize(this.menuButtonLabelGameObject, "Deck");
				this.specialButtonSprite.spriteName = "button_dismount";
				this.specialButton.KeyCommand = Control.Cancel;
				this.SpecialButtonGameObject.SetActive(true);
				this.RotationLockButtonGameObject.SetActive(true);
				this.PerspectiveButtonGameObject.SetActive(true);
				this.CommonButtonPanel.SetActive(true);
				this.ChocoboButtonPanel.SetActive(false);
				this.PlaneButtonPanel.SetActive(false);
				break;
			case 8:
			case 9:
				FF9UIDataTool.DisplayTextLocalize(this.menuButtonLabelGameObject, "Deck");
				this.specialButtonSprite.spriteName = "button_dismount";
				this.specialButton.KeyCommand = Control.Cancel;
				this.SpecialButtonGameObject.SetActive(true);
				this.RotationLockButtonGameObject.SetActive(true);
				this.PerspectiveButtonGameObject.SetActive(false);
				this.CommonButtonPanel.SetActive(true);
				this.ChocoboButtonPanel.SetActive(false);
				this.PlaneButtonPanel.SetActive(true);
				break;
			}
			this.DisplayCameraOptionHud();
		}
		else
		{
			this.ChocoboButtonPanel.SetActive(false);
			this.PlaneButtonPanel.SetActive(false);
		}
	}

	public void DisplayMinimap()
	{
		Vector2 mainCharacterPosition = WMUIData.MainCharacterPosition;
		Single mainCharacterRotationY = WMUIData.MainCharacterRotationY;
		Vector2 cameraPosition = WMUIData.CameraPosition;
		Single cameraRotationY = WMUIData.CameraRotationY;
		Vector2 chocoboPosition = WMUIData.ChocoboPosition;
		Vector2 planePosition = WMUIData.PlanePosition;
		Vector2 vector = new Vector2((Single)this.miniMapWidget.width, (Single)this.miniMapWidget.height);
		this.miniMapPlayerPointer.transform.localPosition = new Vector3(vector.x * mainCharacterPosition.x, vector.y * mainCharacterPosition.y, 0f);
		this.miniMapPlayerPointer.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -mainCharacterRotationY - 90f));
		this.miniMapPlayerVision.transform.localPosition = new Vector3(vector.x * cameraPosition.x, vector.y * cameraPosition.y, 0f);
		this.miniMapPlayerVision.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -cameraRotationY - 90f));
		if (WMUIData.ChocoboIsAvailable)
		{
			this.miniMapChocoboPointer.gameObject.SetActive(true);
			this.miniMapChocoboPointer.transform.localPosition = new Vector3(vector.x * chocoboPosition.x, vector.y * chocoboPosition.y, 0f);
		}
		else
		{
			this.miniMapChocoboPointer.gameObject.SetActive(false);
		}
		if (WMUIData.PlaneIsAvailable)
		{
			this.miniMapPlanePointer.gameObject.SetActive(true);
			this.miniMapPlanePointer.transform.localPosition = new Vector3(vector.x * planePosition.x, vector.y * planePosition.y, 0f);
		}
		else
		{
			this.miniMapPlanePointer.gameObject.SetActive(false);
		}
	}

	public void DisplayFullmap()
	{
		Vector2 mainCharacterPosition = WMUIData.MainCharacterPosition;
		Single mainCharacterRotationY = WMUIData.MainCharacterRotationY;
		Vector2 cameraPosition = WMUIData.CameraPosition;
		Single cameraRotationY = WMUIData.CameraRotationY;
		Vector2 chocoboPosition = WMUIData.ChocoboPosition;
		Vector2 planePosition = WMUIData.PlanePosition;
		Int32 activeMapNo = WMUIData.ActiveMapNo;
		this.referencePosition = new Vector3(0f, 0f, 0f);
		Vector3 b = new Vector3(0f, 0f, 0f);
		Vector2 a = new Vector2((Single)this.mapWidget.width, (Single)this.mapWidget.height);
		if (activeMapNo == 1)
		{
			this.referencePosition = new Vector3(0f, 0f, 0f);
			b = new Vector3(-18f, 14f, 0f);
			a -= new Vector2(96f, 80f);
		}
		else
		{
			this.referencePosition = new Vector3(10f, 0f, 0f);
			b = new Vector3(0f, 32f, 0f);
			a -= new Vector2(100f, 90f);
		}
		this.mapPlayerPointer.transform.localPosition = new Vector3(a.x * mainCharacterPosition.x, a.y * mainCharacterPosition.y, 0f);
		this.mapPlayerPointer.transform.localPosition += b;
		this.mapPlayerPointer.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -mainCharacterRotationY - 90f));
		this.mapPointerWidget.transform.localPosition = new Vector3(a.x * mainCharacterPosition.x, a.y * mainCharacterPosition.y, 0f);
		this.mapPointerWidget.transform.localPosition += b;
		this.mapPlayerVision.transform.localPosition = new Vector3(a.x * cameraPosition.x, a.y * cameraPosition.y, 0f);
		this.mapPlayerVision.transform.localPosition += b;
		this.mapPlayerVision.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -cameraRotationY - 90f));
		if (WMUIData.ChocoboIsAvailable)
		{
			this.mapChocoboPointer.gameObject.SetActive(true);
			this.mapChocoboPointer.transform.localPosition = new Vector3(a.x * chocoboPosition.x, a.y * chocoboPosition.y, 0f);
			this.mapChocoboPointer.transform.localPosition += b;
		}
		else
		{
			this.mapChocoboPointer.gameObject.SetActive(false);
		}
		if (WMUIData.PlaneIsAvailable)
		{
			this.mapPlanePointer.gameObject.SetActive(true);
			this.mapPlanePointer.transform.localPosition = new Vector3(a.x * planePosition.x, a.y * planePosition.y, 0f);
			this.mapPlanePointer.transform.localPosition += b;
		}
		else
		{
			this.mapPlanePointer.gameObject.SetActive(false);
		}
		ff9.navipos[,] navigationLocaition = WMUIData.NavigationLocaition;
		this.mapLocationPointerPanel.transform.localPosition = this.referencePosition;
		this.referencePosition += this.pointerOffset;
		this.pointerPosition = this.mapPointerWidget.transform.localPosition;
		Int32 num = 0;
		Vector2 vector = new Vector2(12.5f, -12.5f);
		for (Int32 i = 0; i < 64; i++)
		{
			ff9.navipos navipos = navigationLocaition[activeMapNo, i];
			if (WMUIData.LocationAvailable(i))
			{
				if (this.locationPointerList.Count <= num)
				{
					GameObject gameObject = NGUITools.AddChild(this.mapLocationPointerPanel, this.LocationPointerPrefab);
					gameObject.name = "Location Pointer#" + i;
					gameObject.transform.localPosition = new Vector3(vector.x + (float)navipos.vx * UIManager.ResourceXMultipier, vector.y + ((float)(-(float)navipos.vy) * UIManager.ResourceYMultipier + a.y), 0f);
					if (HonoInputManager.MouseEnabled)
					{
						UIEventListener uieventListener = UIEventListener.Get(gameObject);
						uieventListener.onHover = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener.onHover, new UIEventListener.BoolDelegate(this.OnLocationHover));
					}
					UIEventListener uieventListener2 = UIEventListener.Get(gameObject);
					uieventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener2.onClick, new UIEventListener.VoidDelegate(this.OnLocaltionClick));
					this.locationPointerList.Add(new WorldHUD.LocationPostionHudData(gameObject, i));
				}
				else
				{
					WorldHUD.LocationPostionHudData locationPostionHudData = this.locationPointerList[num];
					locationPostionHudData.gameobject.name = "Location Pointer#" + i;
					locationPostionHudData.locationId = i;
					locationPostionHudData.transform.localPosition = new Vector3(vector.x + (float)navipos.vx * UIManager.ResourceXMultipier, vector.y + ((float)(-(float)navipos.vy) * UIManager.ResourceYMultipier + a.y), 0f);
				}
				num++;
			}
		}
	}

	private void DisplayLocationName(Int32 indexId)
	{
		if (indexId != -1)
		{
			Int32 num = this.locationPointerList[indexId].locationId;
			Vector3 position = this.locationPointerList[indexId].gameobject.transform.position;
			if (num == 63)
			{
				num = 49;
			}
			this.mapLocationLabel.text = NGUIText.FF9WhiteColor + this.locationName[num + 1];
			this.mapLocationPanel.SetActive(true);
			this.mapLocationBody.UpdateAnchors();
			this.mapLocationBorder.UpdateAnchors();
			if (this.CheckUsingAirShip())
			{
				if (!Singleton<BubbleUI>.Instance.gameObject.activeSelf)
				{
					Singleton<BubbleUI>.Instance.SetGameObjectActive(true);
				}
				Singleton<BubbleUI>.Instance.Show(position, new BubbleUI.Flag[]
				{
					BubbleUI.Flag.QUESTION
				}, null);
				Singleton<BubbleUI>.Instance.transform.position = position;
				Singleton<BubbleUI>.Instance.transform.localPosition += WorldHUD.navBubbleOffset;
			}
		}
		else if (this.mapLocationPanel.activeSelf)
		{
			this.mapLocationPanel.SetActive(false);
			if (this.CheckUsingAirShip())
			{
				Singleton<BubbleUI>.Instance.Hide();
			}
		}
	}

	private void DisplayChocographLocation(Boolean isEnable)
	{
		if (isEnable)
		{
			ChocographUI.UpdateEquipedHintMap();
			if (ChocographUI.CurrentSelectedChocograph != -1)
			{
				this.chocographLocationSprite.spriteName = "chocograph_map_" + ChocographUI.CurrentSelectedChocograph.ToString("0#");
				this.ChocographLocationPanel.SetActive(true);
			}
			else
			{
				this.ChocographLocationPanel.SetActive(false);
			}
		}
		else
		{
			this.ChocographLocationPanel.SetActive(false);
		}
	}

	private void DisplayCameraOptionHud()
	{
		if (this.RotationLockButtonGameObject.activeInHierarchy)
		{
			this.SetRotationLockToggle(FF9StateSystem.Settings.IsAutoRotation);
		}
		if (this.PerspectiveButtonGameObject.activeInHierarchy)
		{
			this.SetPerspectiveToggle(FF9StateSystem.Settings.IsPerspectCamera);
		}
	}

    public void ClearFullMapLocations()
    {
        if (this.mapLocationPointerPanel != (UnityEngine.Object)null)
        {
            this.mapLocationPointerPanel.transform.DestroyChildren();
        }
        this.locationPointerList.Clear();
    }

    public void SetContinentTitleSprite(SByte titleId)
	{
		this.continentTitleText.sprite2D = FF9UIDataTool.LoadWorldTitle(titleId, false);
		this.continentTitleShadow.sprite2D = FF9UIDataTool.LoadWorldTitle(titleId, true);
		this.continentTitleText.MakePixelPerfect();
		this.continentTitleShadow.MakePixelPerfect();
	}

	public void ShowContinentTitle(Int32 fadeInFrames)
	{
		Single fadeInDuration = this.CalculateTitleFadeDuration(fadeInFrames);
		this.continentTitleTextFader.fadeInDuration = fadeInDuration;
		this.continentTitleShadowFader.fadeInDuration = fadeInDuration;
		this.continentTitleTextFader.FadeIn((UIScene.SceneVoidDelegate)null);
		this.continentTitleShadowFader.FadeIn((UIScene.SceneVoidDelegate)null);
	}

	public void HideContinentTitle(Int32 fadeOutFrames)
	{
		Single fadeOutDuration = this.CalculateTitleFadeDuration(fadeOutFrames);
		this.continentTitleTextFader.fadeOutDuration = fadeOutDuration;
		this.continentTitleShadowFader.fadeOutDuration = fadeOutDuration;
		this.continentTitleTextFader.FadeOut((UIScene.SceneVoidDelegate)null);
		this.continentTitleShadowFader.FadeOut((UIScene.SceneVoidDelegate)null);
	}

	public void EnableContinentTitle(Boolean isActive)
	{
		if (this.ContinentTitlePanel.activeSelf != isActive)
		{
			this.ContinentTitlePanel.SetActive(isActive);
			this.isTitleShowing = isActive;
		}
	}

	private Single CalculateTitleFadeDuration(Int32 frames)
	{
		Single num = (Single)frames / 30f;
		return (!HonoBehaviorSystem.Instance.IsFastForwardModeActive()) ? num : (num / (Single)FF9StateSystem.Settings.FastForwardFactor);
	}

	public void SetButtonVisible(Boolean isVisible)
	{
		if (FF9StateSystem.MobilePlatform)
		{
			if (!this.isTitleShowing)
			{
				this.MenuButtonGameObject.SetActive(isVisible);
			}
			if (this.currentCharacterStateIndex >= 1 && this.currentCharacterStateIndex <= 6)
			{
				this.MenuButtonGameObject.SetActive(true);
			}
			if (this.currentState == WorldHUD.State.HUDNoMiniMap)
			{
				if (isVisible)
				{
					this.DisplayButtonHud();
					if (!this.MiniMapPanel.activeSelf)
					{
						this.MapButtonGameObject.SetActive(true);
					}
				}
				else if (this.currentCharacterStateIndex < 1 || this.currentCharacterStateIndex > 6)
				{
					this.MapButtonGameObject.SetActive(isVisible);
					this.RotationLockButtonGameObject.SetActive(false);
					this.PerspectiveButtonGameObject.SetActive(false);
					this.SpecialButtonGameObject.SetActive(false);
					this.CommonButtonPanel.SetActive(false);
					this.ChocoboButtonPanel.SetActive(false);
					this.PlaneButtonPanel.SetActive(false);
				}
			}
			else if (this.currentState == WorldHUD.State.HUD)
			{
				if (isVisible)
				{
					this.DisplayButtonHud();
				}
				else if (this.currentCharacterStateIndex < 1 || this.currentCharacterStateIndex > 6)
				{
					this.RotationLockButtonGameObject.SetActive(false);
					this.PerspectiveButtonGameObject.SetActive(false);
					this.SpecialButtonGameObject.SetActive(false);
					this.CommonButtonPanel.SetActive(false);
					this.ChocoboButtonPanel.SetActive(false);
					this.PlaneButtonPanel.SetActive(false);
				}
			}
			else if (this.currentState == WorldHUD.State.FullMap)
			{
				if (isVisible)
				{
					this.DisplayButtonHud();
				}
				else
				{
					this.RotationLockButtonGameObject.SetActive(false);
					this.PerspectiveButtonGameObject.SetActive(false);
					this.SpecialButtonGameObject.SetActive(false);
					this.CommonButtonPanel.SetActive(false);
					this.ChocoboButtonPanel.SetActive(false);
					this.PlaneButtonPanel.SetActive(false);
				}
			}
		}
	}

	public void SetPauseVisible(Boolean isVisible)
	{
		if (base.gameObject.activeSelf && FF9StateSystem.MobilePlatform)
		{
			this.PauseButtonGameObject.SetActive(isVisible);
		}
	}

	public void SetMinimapVisible(Boolean isVisible)
	{
		this.MiniMapPanel.SetActive(isVisible);
		this.SetMinimapPressable(isVisible);
	}

	public void SetMinimapPressable(Boolean isEnable)
	{
		this.isSelectEnable = isEnable;
	}

	public void SetChocoboHudVisible(Boolean isVisible)
	{
		if (FF9StateSystem.MobilePlatform)
		{
			if (isVisible && this.currentCharacterStateIndex > 0 && this.currentCharacterStateIndex < 6)
			{
				this.ChocoboButtonPanel.SetActive(true);
			}
			else if (!isVisible)
			{
				this.ChocoboButtonPanel.SetActive(false);
			}
		}
	}

	public void SetPlaneHudVisible(Boolean isVisible)
	{
		if (FF9StateSystem.MobilePlatform)
		{
			if (isVisible && (this.currentCharacterStateIndex == 6 || this.currentCharacterStateIndex == 8 || this.currentCharacterStateIndex == 9))
			{
				this.PlaneButtonPanel.SetActive(true);
			}
			else if (!isVisible)
			{
				this.PlaneButtonPanel.SetActive(false);
			}
		}
	}

	public void SetRotationLockToggle(Boolean isToggle)
	{
		if (FF9StateSystem.MobilePlatform)
		{
			switch (this.currentCharacterStateIndex)
			{
			case 0:
				this.rotationLockButtonSprite.spriteName = ((!isToggle) ? "button_rotate" : "button_rotate_act");
				break;
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
				this.rotationLockButtonSprite.spriteName = ((!isToggle) ? "button_rotate" : "button_rotate_act");
				break;
			case 6:
				this.rotationLockButtonSprite.spriteName = ((!isToggle) ? "button_align" : "button_align_act");
				break;
			case 7:
				this.rotationLockButtonSprite.spriteName = ((!isToggle) ? "button_rotate" : "button_rotate_act");
				break;
			case 8:
			case 9:
				this.rotationLockButtonSprite.spriteName = ((!isToggle) ? "button_align" : "button_align_act");
				break;
			}
		}
		FF9StateSystem.Settings.IsBoosterButtonActive[2] = isToggle;
	}

	public void SetPerspectiveToggle(Boolean isToggle)
	{
		if (FF9StateSystem.MobilePlatform)
		{
			this.perspectiveButtonSprite.spriteName = ((!isToggle) ? "button_perspective" : "button_perspective_act");
		}
		FF9StateSystem.Settings.IsBoosterButtonActive[5] = isToggle;
	}

	public void OnChocographClick(GameObject go, Boolean isClicked)
	{
		PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, delegate
		{
			this.Hide(delegate
			{
				PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Chocograph);
			});
		});
	}

	public void ForceShowButton()
	{
		base.StartCoroutine(this.ForceShowButtonProcess());
	}

	private IEnumerator ForceShowButtonProcess()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		PersistenSingleton<UIManager>.Instance.IsMenuControlEnable = true;
		this.ShowButton();
		yield break;
	}

	private IEnumerator DisableEventInputForAWhile()
	{
		EventInput.IsProcessingInput = false;
		yield return new WaitForEndOfFrame();
		EventInput.IsProcessingInput = true;
		yield break;
	}

	private Vector3 SnapPointerToLocation(Int32 indexId)
	{
		Vector3 positionToSnap = this.GetPositionToSnap(indexId);
		this.mapPointerWidget.transform.localPosition = positionToSnap;
		return positionToSnap;
	}

	private Vector3 GetPositionToSnap(Int32 locationIndex)
	{
		if (locationIndex >= this.mapLocationPointerPanel.transform.childCount)
		{
			locationIndex = this.mapLocationPointerPanel.transform.childCount - 1;
		}
		Vector3 localPosition = this.mapLocationPointerPanel.GetChild(locationIndex).transform.localPosition;
		return localPosition + this.referencePosition;
	}

	private Boolean CheckUsingAirShip()
	{
		return WMUIData.StatusNo == 10 || WMUIData.StatusNo == 9;
	}

	private void ShowButton()
	{
		this.currentCharacterStateIndex = WMUIData.ControlNo;
		this.SetButtonVisible(true);
		PersistenSingleton<UIManager>.Instance.Booster.SetBoosterState(this.currentCharacterStateIndex);
	}

	private void Update()
	{
		if ((this.currentState == WorldHUD.State.HUD || this.currentState == WorldHUD.State.HUDNoMiniMap) && this.currentCharacterStateIndex != WMUIData.ControlNo && PersistenSingleton<UIManager>.Instance.IsPlayerControlEnable)
		{
			this.ShowButton();
		}
		if (this.currentState == WorldHUD.State.HUD)
		{
			this.DisplayMinimap();
		}
		else if (this.currentState == WorldHUD.State.FullMap)
		{
			if (!this.ignorePointerProcess)
			{
				if (UIManager.Input.GetKey(Control.Cancel))
				{
					if (this.mapContent.activeSelf)
					{
						this.mapLocationPanel.SetActive(false);
						this.mapContent.SetActive(false);
						this.controlPointEnable = false;
					}
				}
				else if (!this.mapContent.activeSelf)
				{
					this.mapLocationPanel.SetActive(this.currentLocationIndex != -1);
					this.mapContent.SetActive(true);
					this.controlPointEnable = true;
				}
			}
			this.MovePointer();
		}
	}

	private void Awake()
	{
		if (HonoInputManager.MouseEnabled)
		{
			UICamera.onMouseMove = (UICamera.MoveDelegate)Delegate.Combine(UICamera.onMouseMove, new UICamera.MoveDelegate(this.onMouseMove));
		}
		base.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
		this.chocographLocationSprite = this.ChocographLocationPanel.GetChild(0).GetComponent<UISprite>();
		this.menuButtonLabelGameObject = this.MenuButtonGameObject.GetChild(2);
		this.specialButtonSprite = this.SpecialButtonGameObject.GetChild(2).GetComponent<UISprite>();
		this.specialButton = this.SpecialButtonGameObject.GetComponent<OnScreenButton>();
		this.perspectiveButtonSprite = this.PerspectiveButtonGameObject.GetChild(0).GetComponent<UISprite>();
		this.rotationLockButtonSprite = this.RotationLockButtonGameObject.GetChild(0).GetComponent<UISprite>();
		this.moveLeftGameObject = this.CommonButtonPanel.GetChild(0);
		this.moveRightGameObject = this.CommonButtonPanel.GetChild(1);
		UIEventListener uieventListener = UIEventListener.Get(this.moveLeftGameObject);
		uieventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener.onPress, new UIEventListener.BoolDelegate(this.OnPressButton));
		UIEventListener uieventListener2 = UIEventListener.Get(this.moveRightGameObject);
		uieventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onPress, new UIEventListener.BoolDelegate(this.OnPressButton));
		this.miniMapButton = this.MiniMapPanel.GetComponent<UIButton>();
		this.miniMapSprite = this.MiniMapPanel.GetChild(4).GetComponent<UISprite>();
		this.miniMapWidget = this.MiniMapPanel.GetChild(4).GetComponent<UIWidget>();
		this.miniMapPlayerPointer = this.MiniMapPanel.GetChild(1).GetComponent<UISprite>();
		this.miniMapPlayerVision = this.MiniMapPanel.GetChild(0).GetComponent<UISprite>();
		this.miniMapChocoboPointer = this.MiniMapPanel.GetChild(2).GetComponent<UISprite>();
		this.miniMapPlanePointer = this.MiniMapPanel.GetChild(3).GetComponent<UISprite>();
		this.mapButton = this.FullMapPanel.GetChild(1).GetComponent<UIButton>();
		this.mapSprite = this.FullMapPanel.GetChild(1).GetComponent<UI2DSprite>();
		this.mapWidget = this.FullMapPanel.GetChild(1).GetComponent<UIWidget>();
		this.mapContent = this.FullMapPanel.GetChild(1).GetChild(0);
		this.mapLocationPointerPanel = this.FullMapPanel.GetChild(1).GetChild(0).GetChild(0);
		this.mapPlayerPointer = this.FullMapPanel.GetChild(1).GetChild(0).GetChild(3).GetComponent<UISprite>();
		this.mapPlayerVision = this.FullMapPanel.GetChild(1).GetChild(0).GetChild(2).GetComponent<UISprite>();
		this.mapChocoboPointer = this.FullMapPanel.GetChild(1).GetChild(0).GetChild(4).GetComponent<UISprite>();
		this.mapPlanePointer = this.FullMapPanel.GetChild(1).GetChild(0).GetChild(5).GetComponent<UISprite>();
		this.mapPointerWidget = this.FullMapPanel.GetChild(1).GetChild(0).GetChild(1).GetComponent<UISprite>();
		this.mapLocationPanel = this.FullMapPanel.GetChild(0);
		this.mapHelpButtonGameObject = this.FullMapPanel.GetChild(2).gameObject;
		this.mapBackButtonGameObject = this.FullMapPanel.GetChild(3).gameObject;
		this.mapLeftBumperGameObject = this.FullMapPanel.GetChild(4).gameObject;
		this.mapRightBumperGameObject = this.FullMapPanel.GetChild(5).gameObject;
		this.mapLocationBody = this.mapLocationPanel.GetChild(0).GetComponent<UISprite>();
		this.mapLocationBorder = this.mapLocationPanel.GetChild(1).GetComponent<UISprite>();
		this.mapLocationLabel = this.mapLocationPanel.GetChild(2).GetComponent<UILabel>();
		this.continentTitleText = this.ContinentTitlePanel.GetChild(0).GetComponent<UI2DSprite>();
		this.continentTitleShadow = this.ContinentTitlePanel.GetChild(1).GetComponent<UI2DSprite>();
		this.continentTitleTextFader = this.continentTitleText.GetComponent<HonoFading>();
		this.continentTitleShadowFader = this.continentTitleShadow.GetComponent<HonoFading>();
		UIEventListener uieventListener3 = UIEventListener.Get(this.mapWidget.gameObject);
		uieventListener3.onHover = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener3.onHover, new UIEventListener.BoolDelegate(this.OnFullMapHover));
		UIEventListener uieventListener4 = UIEventListener.Get(this.mapWidget.gameObject);
		uieventListener4.onNavigate = (UIEventListener.KeyCodeDelegate)Delegate.Combine(uieventListener4.onNavigate, new UIEventListener.KeyCodeDelegate(this.OnFullMapNavigator));
		UIEventListener uieventListener5 = UIEventListener.Get(this.mapWidget.gameObject);
		uieventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener5.onClick, new UIEventListener.VoidDelegate(this.onClick));
		UIEventListener uieventListener6 = UIEventListener.Get(this.MiniMapPanel);
		uieventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener6.onClick, new UIEventListener.VoidDelegate(this.onClick));
		this.PlaneButtonPanel.GetChild(0).GetComponent<OnScreenButton>().SetHighlightKeyCommand(Control.Down);
		this.PlaneButtonPanel.GetChild(1).GetComponent<OnScreenButton>().SetHighlightKeyCommand(Control.Up);
	}

	public GameObject LocationPointerPrefab;

	public GameObject MenuButtonGameObject;

	public GameObject SpecialButtonGameObject;

	public GameObject RotationLockButtonGameObject;

	public GameObject PerspectiveButtonGameObject;

	public GameObject MapButtonGameObject;

	public GameObject PauseButtonGameObject;

	public GameObject ChocographLocationPanel;

	public GameObject CommonButtonPanel;

	public GameObject ChocoboButtonPanel;

	public GameObject PlaneButtonPanel;

	public GameObject MiniMapPanel;

	public GameObject FullMapPanel;

	public GameObject ScreenFadeGameObject;

	public GameObject ContinentTitlePanel;

	private Vector4 FullMapBorder = new Vector4(10f, 1190f, 20f, 1020f);

	private GameObject menuButtonLabelGameObject;

	private UISprite specialButtonSprite;

	private OnScreenButton specialButton;

	private UISprite rotationLockButtonSprite;

	private UISprite perspectiveButtonSprite;

	private UISprite miniMapSprite;

	private UIButton miniMapButton;

	private UIWidget miniMapWidget;

	private UISprite miniMapPlayerPointer;

	private UISprite miniMapPlayerVision;

	private UISprite miniMapChocoboPointer;

	private UISprite miniMapPlanePointer;

	private UI2DSprite mapSprite;

	private UIButton mapButton;

	private UIWidget mapWidget;

	private GameObject mapContent;

	private UIWidget mapPointerWidget;

	private GameObject mapLocationPanel;

	private UILabel mapLocationLabel;

	private UISprite mapLocationBody;

	private UISprite mapLocationBorder;

	private UISprite mapPlayerPointer;

	private UISprite mapPlayerVision;

	private UISprite mapChocoboPointer;

	private UISprite mapPlanePointer;

	private GameObject mapLocationPointerPanel;

	private GameObject mapBackButtonGameObject;

	private GameObject mapHelpButtonGameObject;

	private GameObject mapRightBumperGameObject;

	private GameObject mapLeftBumperGameObject;

	private HonoFading continentTitleTextFader;

	private HonoFading continentTitleShadowFader;

	private UI2DSprite continentTitleText;

	private UI2DSprite continentTitleShadow;

	private GameObject moveLeftGameObject;

	private GameObject moveRightGameObject;

	private UISprite chocographLocationSprite;

	private Boolean isSelectEnable = true;

	private Boolean controlPointEnable;

	private Int32 currentLocationIndex = -1;

	private Int32 navigateLocationIndex = -1;

	private WorldHUD.State currentState = WorldHUD.State.HUD;

	private Int32 currentCharacterStateIndex = -1;

	private List<WorldHUD.LocationPostionHudData> locationPointerList = new List<WorldHUD.LocationPostionHudData>();

	private String[] locationName;

	private Single pointerSpeed = 434.782623f;

	private Vector3 pointerPosition = Vector3.zero;

	private Vector3 referencePosition;

	private Vector3 pointerOffset = new Vector3(8f, 14f, 0f);

	private Single snapRadius = 15f;

	private Single directionAnalogThreshold = 0.2f;

	private Boolean ignorePointerProcess;

	private static readonly Vector3 navBubbleOffset = new Vector3(0f, 55f, 0f);

	private Boolean enableMapButton = true;

	private Vector3 inputAxis;

	private Boolean isTitleShowing;

	public class LocationPostionHudData
	{
		public LocationPostionHudData(GameObject go, Int32 id)
		{
			this.gameobject = go;
			this.transform = go.transform;
			this.bounds = NGUIMath.CalculateAbsoluteWidgetBounds(go.transform);
			this.locationId = id;
		}

		public Single DistanceTo(Vector3 localPosition)
		{
			return Vector3.Distance(this.gameobject.transform.localPosition, localPosition);
		}

		public GameObject gameobject;

		public Transform transform;

		public Bounds bounds;

		public Int32 locationId;
	}

	public enum State
	{
		HUDNoMiniMap,
		HUD,
		FullMap,
		ControlHelp
	}
}
