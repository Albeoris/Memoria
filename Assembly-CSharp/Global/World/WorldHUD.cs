﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Assets;
using UnityEngine;

public class WorldHUD : UIScene
{
    public WorldHUD.State CurrentState => this.currentState;

    public Int32 CurrentCharacterStateIndex
    {
        get => this.currentCharacterStateIndex;
        set => this.currentCharacterStateIndex = value;
    }

    public Boolean EnableMapButton
    {
        set => this.enableMapButton = value;
    }

    public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate afterShowAction = delegate
        {
            this.currentCharacterStateIndex = -1;
            ButtonGroupState.HelpEnabled = false;
            this.PauseButtonGameObject.SetActive(PersistenSingleton<UIManager>.Instance.IsPauseControlEnable && FF9StateSystem.MobilePlatform);
            if (this.currentState == WorldHUD.State.FullMap)
            {
                if (PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.FieldHUD)
                {
                    this.ForceCloseFullMap();
                }
                else
                {
                    if (!this.ignorePointerProcess)
                        this.DisplayLocationName(this.currentLocationIndex);
                    if (FF9StateSystem.MobilePlatform)
                        base.StartCoroutine(this.ForceOpenVirtual());
                }
                this.DisplayChocographLocation(false);
            }
            else if (ff9.w_naviMode == 0)
            {
                this.currentState = WorldHUD.State.HUDNoMiniMap;
                this.MiniMapPanel.SetActive(false);
                this.MapButtonGameObject.SetActive(FF9StateSystem.MobilePlatform);
                this.DisplayChocographLocation(false);
            }
            else if (ff9.w_naviMode == 1)
            {
                this.currentState = WorldHUD.State.HUD;
                this.MiniMapPanel.SetActive(true);
                this.MapButtonGameObject.SetActive(false);
                this.DisplayChocographLocation(true);
            }
            if (PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.Pause && this.currentState != WorldHUD.State.FullMap && PersistenSingleton<EventEngine>.Instance.GetUserControl())
                base.StartCoroutine(this.DisableEventInputForAWhile());
        };
        if (afterFinished != null)
            afterShowAction += afterFinished;
        base.Show(afterShowAction);
        PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
        PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(ff9.GetUserControl());
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(ff9.GetUserControl(), null);
        this.EnableContinentTitle(false);
        this.InitialHUD();
        this.DisplayButtonHud();
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
        UIScene.SceneVoidDelegate afterHideAction = delegate
        {
            if (!base.NextSceneIsModal)
                PersistenSingleton<UIManager>.Instance.SetGameCameraEnable(false);
        };
        if (afterFinished != null)
            afterHideAction += afterFinished;
        base.Hide(afterHideAction);
        this.PauseButtonGameObject.SetActive(false);
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, null);
        PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
    }

    public void OnLocalize()
    {
        if (!isActiveAndEnabled)
            return;
        if (this.navigateDialog != null && this.navigateLocationIndex >= 0)
        {
            Int32 locId = this.navigateLocationIndex;
            if (locId == 63)
                locId = 49;
            this.navigateDialog.ChangePhraseSoft($"[STRT=0,3][CENT=0]{Localization.Get("NavigateDialog").Replace("{0}", FF9TextTool.GetTableText(0u)[locId + 1])}");
        }
        if (this.currentLocationIndex >= 0)
        {
            Int32 locId = this.locationPointerList[this.currentLocationIndex].locationId;
            if (locId == 63)
                locId = 49; // Chocobo's Paradise
            this.mapLocationLabel.rawText = FF9TextTool.GetTableText(0u)[locId + 1];
            this.mapLocationBody.UpdateAnchors();
            this.mapLocationBorder.UpdateAnchors();
        }
    }

    public void OnMouseMove(Vector2 delta)
    {
        if (FF9StateSystem.PCPlatform && this.controlPointEnable && HonoInputManager.MouseEnabled && !this.ignorePointerProcess)
        {
            Camera camera = NGUITools.FindCameraForLayer(base.gameObject.layer);
            Vector3 mousePos = base.transform.worldToLocalMatrix.MultiplyPoint3x4(camera.ScreenToWorldPoint(Input.mousePosition));
            mousePos.x += this.mapWidget.width / 2f;
            mousePos.y += this.mapWidget.height / 2f;
            mousePos.x -= this.mapPointerWidget.width / 2f - 8f;
            mousePos.y -= 14f;
            mousePos.x = Mathf.Clamp(mousePos.x, this.FullMapBorder.x, this.FullMapBorder.y);
            mousePos.y = Mathf.Clamp(mousePos.y, this.FullMapBorder.z, this.FullMapBorder.w);
            Int32 hoverLocation = this.DetectLocation(mousePos);
            if (this.currentLocationIndex != hoverLocation)
            {
                this.currentLocationIndex = hoverLocation;
                this.DisplayLocationName(this.currentLocationIndex);
            }
            if (hoverLocation >= 0)
                mousePos = this.GetPositionToSnap(this.currentLocationIndex);
            this.mapPointerWidget.transform.localPosition = mousePos;
        }
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go) && this.currentState == WorldHUD.State.FullMap)
        {
            BubbleUI instance = Singleton<BubbleUI>.Instance;
            Boolean allowAutoPilot = true;
            if (FF9StateSystem.MobilePlatform)
            {
                SourceControl source = PersistenSingleton<HonoInputManager>.Instance.GetSource(Control.Confirm);
                allowAutoPilot = instance.helpButton.gameObject.Equals(go) || instance.primaryButton.Equals(go) || source == SourceControl.Joystick || source == SourceControl.KeyBoard;
            }
            if (allowAutoPilot && this.currentLocationIndex >= 0 && this.CheckUsingAirShip() && !Singleton<VirtualAnalog>.Instance.IsShown && !this.ignorePointerProcess)
                this.ProcessTouchedLocation();
        }
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (base.OnKeyCancel(go) && this.currentCharacterStateIndex == 0 && this.currentState != WorldHUD.State.FullMap && !Singleton<BubbleUI>.Instance.beachButton.gameObject.activeInHierarchy && UIManager.Input.ContainsAndroidQuitKey())
            UIManager.Input.OnQuitCommandDetected();
        return true;
    }

    public override Boolean OnKeyMenu(GameObject go)
    {
        if (base.OnKeyMenu(go))
        {
            if (this.currentState == WorldHUD.State.HUD || this.currentState == WorldHUD.State.HUDNoMiniMap)
            {
                if (PersistenSingleton<UIManager>.Instance.Dialogs.Visible && !EventCollision.IsRidingChocobo())
                    PersistenSingleton<UIManager>.Instance.HideAllHUD();
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
            if (ff9.w_moveMogPtr != null && PersistenSingleton<EventEngine>.Instance.objIsVisible(ff9.w_moveMogPtr.originalActor))
                return false;
            if (!PersistenSingleton<SceneDirector>.Instance.PendingNextScene.Equals(String.Empty) || !PersistenSingleton<SceneDirector>.Instance.PendingCurrentScene.Equals(String.Empty))
                return false;
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
                    PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, null);
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
                    PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, null);
                    ff9.w_naviMode = 1;
                    this.SetButtonVisible(true);
                    this.MiniMapPanel.SetActive(true);
                    this.MapButtonGameObject.SetActive(false);
                    this.DisplayChocographLocation(true);
                }
                ff9.byte_gEventGlobal_updateNaviMode();
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
                PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, null);
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
                PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, null);
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
            this.controlPointEnable = isHover;
    }

    private void OnFullMapNavigator(GameObject go, KeyCode key)
    {
    }

    private void MovePointer()
    {
        if ((HonoInputManager.JoystickEnabled || HonoInputManager.VirtualAnalogEnabled) && PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD)
        {
            if (this.ignorePointerProcess)
                return;
            this.inputAxis = PersistenSingleton<HonoInputManager>.Instance.GetAxis();
            if (this.inputAxis.magnitude > this.directionAnalogThreshold)
            {
                this.pointerPosition += this.inputAxis * this.pointerSpeed * Time.deltaTime;
                UICamera.EventWaitTime = Time.deltaTime;
                if (this.pointerPosition.x > this.FullMapBorder.y)
                    this.pointerPosition.x = this.FullMapBorder.x;
                else if (this.pointerPosition.x < this.FullMapBorder.x)
                    this.pointerPosition.x = this.FullMapBorder.y;
                if (this.pointerPosition.y > this.FullMapBorder.w)
                    this.pointerPosition.y = this.FullMapBorder.z;
                else if (this.pointerPosition.y < this.FullMapBorder.z)
                    this.pointerPosition.y = this.FullMapBorder.w;
                Vector3 positionToSnap = this.pointerPosition;
                Int32 locationIndex = this.DetectLocation(positionToSnap);
                if (this.currentLocationIndex != locationIndex)
                {
                    this.currentLocationIndex = locationIndex;
                    this.DisplayLocationName(this.currentLocationIndex);
                }
                if (locationIndex != -1)
                    positionToSnap = this.GetPositionToSnap(this.currentLocationIndex);
                this.mapPointerWidget.transform.localPosition = positionToSnap;
            }
        }
    }

    private Int32 DetectLocation(Vector3 onScreenPosition)
    {
        Int32 locationIndex = -1;
        Single bestDist = Single.PositiveInfinity;
        foreach (WorldHUD.LocationPostionHudData locationPostionHudData in this.locationPointerList)
        {
            Single dist = locationPostionHudData.DistanceTo(onScreenPosition - this.referencePosition);
            if (dist < this.snapRadius && dist < bestDist)
            {
                locationIndex = locationPostionHudData.gameobject.transform.GetSiblingIndex();
                bestDist = dist;
            }
        }
        return locationIndex;
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

    private void OnLocationClick(GameObject go)
    {
        if (this.currentState == WorldHUD.State.FullMap && (FF9StateSystem.MobilePlatform || FF9StateSystem.PCPlatform))
        {
            Int32 index = 0;
            Int32 locationIndex = -1;
            foreach (WorldHUD.LocationPostionHudData locationPostionHudData in this.locationPointerList)
            {
                if (locationPostionHudData.gameobject.transform.position == go.transform.position)
                {
                    locationIndex = index;
                    break;
                }
                index++;
            }
            if (locationIndex >= 0 && this.currentLocationIndex != locationIndex)
            {
                this.currentLocationIndex = locationIndex;
                this.mapPointerWidget.transform.localPosition = this.GetPositionToSnap(this.currentLocationIndex);
                this.DisplayLocationName(this.currentLocationIndex);
            }
        }
    }

    private void OnConfirmNavi(Int32 choice)
    {
        this.navigateDialog = null;
        if (choice == 0)
        {
            ff9.w_frameAutoid = (Byte)this.navigateLocationIndex;
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
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, null);
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
        this.DisplayLocationName(isActive ? this.currentLocationIndex : -1);
    }

    private void FindNextLocation(Boolean isPressedLeftBumper)
    {
        if (this.locationPointerList.Count == 1)
        {
            this.currentLocationIndex = 0;
        }
        else
        {
            Vector3 currentLocationPos = this.currentLocationIndex < 0 ? Vector3.zero : this.locationPointerList[this.currentLocationIndex].gameobject.transform.position;
            Vector3 nextLocationPos = currentLocationPos;
            Int32 loopCounter = 0;
            while (currentLocationPos == nextLocationPos)
            {
                if (isPressedLeftBumper)
                {
                    if (this.currentLocationIndex >= this.locationPointerList.Count - 1)
                        this.currentLocationIndex = 0;
                    else
                        this.currentLocationIndex++;
                }
                else if (this.currentLocationIndex <= 0)
                {
                    this.currentLocationIndex = this.locationPointerList.Count - 1;
                }
                else
                {
                    this.currentLocationIndex--;
                }
                nextLocationPos = this.locationPointerList[this.currentLocationIndex].gameobject.transform.position;
                loopCounter++;
                if (loopCounter >= this.locationPointerList.Count)
                    break;
            }
        }
    }

    private void ProcessTouchedLocation()
    {
        Int32 locId = this.locationPointerList[this.currentLocationIndex].locationId;
        this.navigateLocationIndex = locId;
        if (locId == 63)
            locId = 49; // Chocobo's Paradise
        this.ShowMapPointer(false);
        String message = Localization.Get("NavigateDialog").Replace("{0}", FF9TextTool.GetTableText(0u)[locId + 1]);
        this.navigateDialog = Singleton<DialogManager>.Instance.AttachDialog($"[STRT=0,3][CENT=0]{message}", 0, 0, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, Vector2.zero, Dialog.CaptionType.None);
        this.navigateDialog.AfterDialogHidden = this.OnConfirmNavi;
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
        ff9.byte_gEventGlobal_updateNaviMode();
        this.SetButtonVisible(true);
        this.FullMapPanel.SetActive(false);
        UIManager.Input.ResetKeyCode();
    }

    public void InitialHUD()
    {
        if (WMUIData.ActiveMapNo == 1)
        {
            String externalPath = AssetManager.SearchAssetOnDisc("EmbeddedAsset/UI/Sprites/world_map_full_all.png", true, false);
            if (!String.IsNullOrEmpty(externalPath))
                externalPath = "EmbeddedAsset/UI/Sprites/world_map_full_all.png";
            else
                externalPath = "EmbeddedAsset/UI/Sprites/world_map_full_all";
            Sprite fullMapSprite = AssetManager.Load<Sprite>(externalPath, false);
            this.miniMapSprite.spriteName = "world_map_mini_all";
            this.miniMapButton.normalSprite = "world_map_mini_all";
            this.mapSprite.sprite2D = fullMapSprite;
            this.mapButton.normalSprite2D = fullMapSprite;
        }
        else
        {
            String externalPath = AssetManager.SearchAssetOnDisc("EmbeddedAsset/UI/Sprites/world_map_full_mistcontinent.png", true, false);
            if (!String.IsNullOrEmpty(externalPath))
                externalPath = "EmbeddedAsset/UI/Sprites/world_map_full_mistcontinent.png";
            else
                externalPath = "EmbeddedAsset/UI/Sprites/world_map_full_mistcontinent";
            Sprite mistContinentMapSprite = AssetManager.Load<Sprite>(externalPath, false);
            this.miniMapSprite.spriteName = "world_map_mini_mistcontinent";
            this.miniMapButton.normalSprite = "world_map_mini_mistcontinent";
            this.mapSprite.sprite2D = mistContinentMapSprite;
            this.mapButton.normalSprite2D = mistContinentMapSprite;
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
        this.SetMiniMapAndChocoPosition();
    }

    private void SetMiniMapAndChocoPosition()
    {
        Camera camera = NGUITools.FindCameraForLayer(base.gameObject.layer);
        Vector3 camVector = camera.ScreenToWorldPoint(new Vector3((Int32)camera.pixelRect.width, (Int32)camera.pixelRect.height, 0));
        Vector2 minimapSize = new Vector2(miniMapSprite.width, miniMapSprite.height);
        Vector2 chocoSize = new Vector2((Int32)((chocographLocationSprite.width + 36f) * chocographLocationSprite.pixelSize), (Int32)((chocographLocationSprite.height + 36f) * chocographLocationSprite.pixelSize));

        Transform minimap = this.miniMapButton.transform;
        Transform choco = this.ChocographLocationPanel.transform;

        Vector2 offsetBlackBars = Vector2.zero;
        if (!Configuration.Graphics.WidescreenSupport) // offset of black bars, as screen reference is actual window
        {
            if (320f / 220f < (Single)Screen.width / (Single)Screen.height) // screen wider than world
            {
                Single realWidth = (Single)(Screen.height * 320f / 220f);
                offsetBlackBars.x = realWidth > 0 ? (Int32)((Screen.width - realWidth) / 2) : 0;
            }
            else
            {
                Single realHeight = (Single)(Screen.width / 320f * 220f);
                offsetBlackBars.y = realHeight > 0 ? (Int32)((Screen.height - realHeight) / 2) : 0;
            }
        }

        if (Configuration.Interface.MinimapPreset == 0) // left position
        {
            minimap.position = new Vector3(-camVector.x, -camVector.y, 0); // bottom left of the screen
            minimap.localPosition += new Vector3(20, 20, 0); // offset from border

            choco.position = this.miniMapButton.transform.position; // center of chocograph = bottom left of minimap
            choco.localPosition += new Vector3(chocoSize.x / 2, chocoSize.y / 2, 0); // bottom left of chocograph = bottom left of minimap
            choco.localPosition += new Vector3(0f, minimapSize.y + 15, 0); // bottom left of chocograph = top left of minimap + 15f

            minimap.localPosition += new Vector3(offsetBlackBars.x, offsetBlackBars.y, 0); // apply offset if not widescreen
            choco.localPosition += new Vector3(offsetBlackBars.x, offsetBlackBars.y, 0);
        }
        else // right position
        {
            minimap.position = new Vector3(camVector.x, -camVector.y, 0); // bottom left of the screen
            minimap.localPosition += new Vector3(-minimapSize.x, 0, 0); // take bottom right as corner point
            minimap.localPosition += new Vector3(-20, 20, 0); // offset from border

            choco.position = this.miniMapButton.transform.position; // center of chocograph = bottom left of minimap
            choco.localPosition += new Vector3(-(Int32)(chocoSize.x / 2) + minimapSize.x, chocoSize.y / 2, 0); // bottom right of chocograph = bottom right of minimap
            choco.localPosition += new Vector3(0, minimapSize.y + 15 , 0); // bottom right of chocograph = top left of minimap

            minimap.localPosition += new Vector3(-offsetBlackBars.x, offsetBlackBars.y, 0); // apply offset if not widescreen
            choco.localPosition += new Vector3(-offsetBlackBars.x, offsetBlackBars.y, 0);
        }
        minimap.localPosition += new Vector3(Configuration.Interface.MinimapOffset.x, Configuration.Interface.MinimapOffset.y, 0); // apply user set offset
        choco.localPosition += new Vector3(Configuration.Interface.MinimapOffset.x, Configuration.Interface.MinimapOffset.y, 0);
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
        Vector3 mapWidgetOffset = new Vector3(0f, 0f, 0f);
        Vector2 mapWidgetSize = new Vector2(this.mapWidget.width, this.mapWidget.height);
        if (activeMapNo == 1)
        {
            this.referencePosition = new Vector3(0f, 0f, 0f);
            mapWidgetOffset = new Vector3(-18f, 14f, 0f);
            mapWidgetSize -= new Vector2(96f, 80f);
        }
        else
        {
            this.referencePosition = new Vector3(10f, 0f, 0f);
            mapWidgetOffset = new Vector3(0f, 32f, 0f);
            mapWidgetSize -= new Vector2(100f, 90f);
        }
        this.mapPlayerPointer.transform.localPosition = new Vector3(mapWidgetSize.x * mainCharacterPosition.x, mapWidgetSize.y * mainCharacterPosition.y, 0f);
        this.mapPlayerPointer.transform.localPosition += mapWidgetOffset;
        this.mapPlayerPointer.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -mainCharacterRotationY - 90f));
        this.mapPointerWidget.transform.localPosition = new Vector3(mapWidgetSize.x * mainCharacterPosition.x, mapWidgetSize.y * mainCharacterPosition.y, 0f);
        this.mapPointerWidget.transform.localPosition += mapWidgetOffset;
        this.mapPlayerVision.transform.localPosition = new Vector3(mapWidgetSize.x * cameraPosition.x, mapWidgetSize.y * cameraPosition.y, 0f);
        this.mapPlayerVision.transform.localPosition += mapWidgetOffset;
        this.mapPlayerVision.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -cameraRotationY - 90f));
        if (WMUIData.ChocoboIsAvailable)
        {
            this.mapChocoboPointer.gameObject.SetActive(true);
            this.mapChocoboPointer.transform.localPosition = new Vector3(mapWidgetSize.x * chocoboPosition.x, mapWidgetSize.y * chocoboPosition.y, 0f);
            this.mapChocoboPointer.transform.localPosition += mapWidgetOffset;
        }
        else
        {
            this.mapChocoboPointer.gameObject.SetActive(false);
        }
        if (WMUIData.PlaneIsAvailable)
        {
            this.mapPlanePointer.gameObject.SetActive(true);
            this.mapPlanePointer.transform.localPosition = new Vector3(mapWidgetSize.x * planePosition.x, mapWidgetSize.y * planePosition.y, 0f);
            this.mapPlanePointer.transform.localPosition += mapWidgetOffset;
        }
        else
        {
            this.mapPlanePointer.gameObject.SetActive(false);
        }
        ff9.navipos[,] navigationLocaition = WMUIData.NavigationLocaition;
        this.mapLocationPointerPanel.transform.localPosition = this.referencePosition;
        this.referencePosition += this.pointerOffset;
        this.pointerPosition = this.mapPointerWidget.transform.localPosition;
        Int32 locationIndex = 0;
        Vector2 locationObjectOffset = new Vector2(12.5f, -12.5f);
        for (Int32 i = 0; i < 64; i++)
        {
            ff9.navipos navipos = navigationLocaition[activeMapNo, i];
            if (WMUIData.LocationAvailable(i))
            {
                if (this.locationPointerList.Count <= locationIndex)
                {
                    GameObject locationPositionGo = NGUITools.AddChild(this.mapLocationPointerPanel, this.LocationPointerPrefab);
                    locationPositionGo.name = "Location Pointer#" + i;
                    locationPositionGo.transform.localPosition = new Vector3(locationObjectOffset.x + navipos.vx * UIManager.ResourceXMultipier, locationObjectOffset.y - navipos.vy * UIManager.ResourceYMultipier + mapWidgetSize.y, 0f);
                    if (HonoInputManager.MouseEnabled)
                        UIEventListener.Get(locationPositionGo).onHover += this.OnLocationHover;
                    UIEventListener.Get(locationPositionGo).onClick += this.OnLocationClick;
                    this.locationPointerList.Add(new WorldHUD.LocationPostionHudData(locationPositionGo, i));
                }
                else
                {
                    WorldHUD.LocationPostionHudData locationPostionHudData = this.locationPointerList[locationIndex];
                    locationPostionHudData.gameobject.name = "Location Pointer#" + i;
                    locationPostionHudData.locationId = i;
                    locationPostionHudData.transform.localPosition = new Vector3(locationObjectOffset.x + navipos.vx * UIManager.ResourceXMultipier, locationObjectOffset.y - navipos.vy * UIManager.ResourceYMultipier + mapWidgetSize.y, 0f);
                }
                locationIndex++;
            }
        }
    }

    private void DisplayLocationName(Int32 indexId)
    {
        if (indexId != -1)
        {
            Int32 locId = this.locationPointerList[indexId].locationId;
            Vector3 position = this.locationPointerList[indexId].gameobject.transform.position;
            if (locId == 63)
                locId = 49; // Chocobo's Paradise
            this.mapLocationLabel.rawText = FF9TextTool.GetTableText(0u)[locId + 1];
            this.mapLocationPanel.SetActive(true);
            this.mapLocationBody.UpdateAnchors();
            this.mapLocationBorder.UpdateAnchors();
            if (this.CheckUsingAirShip())
            {
                if (!Singleton<BubbleUI>.Instance.gameObject.activeSelf)
                    Singleton<BubbleUI>.Instance.SetGameObjectActive(true);
                Singleton<BubbleUI>.Instance.Show(position, [BubbleUI.Flag.QUESTION], null);
                Singleton<BubbleUI>.Instance.transform.position = position;
                Singleton<BubbleUI>.Instance.transform.localPosition += WorldHUD.navBubbleOffset;
            }
        }
        else if (this.mapLocationPanel.activeSelf)
        {
            this.mapLocationPanel.SetActive(false);
            if (this.CheckUsingAirShip())
                Singleton<BubbleUI>.Instance.Hide();
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
            this.SetRotationLockToggle(FF9StateSystem.Settings.IsAutoRotation);
        if (this.PerspectiveButtonGameObject.activeInHierarchy)
            this.SetPerspectiveToggle(FF9StateSystem.Settings.IsPerspectCamera);
    }

    public void ClearFullMapLocations()
    {
        if (this.mapLocationPointerPanel != null)
            this.mapLocationPointerPanel.transform.DestroyChildren();
        this.locationPointerList.Clear();
    }

    public void SetContinentTitleSprite(SByte titleId)
    {
        this.continentTitleText.sprite2D = FF9UIDataTool.LoadWorldTitle(titleId, false);
        this.continentTitleShadow.sprite2D = FF9UIDataTool.LoadWorldTitle(titleId, true);
        this.continentTitleText.MakePixelPerfect();
        this.continentTitleShadow.MakePixelPerfect();
        Rect spriteRect = WorldConfiguration.GetTitleSpriteRect(titleId);
        this.continentTitleText.transform.localPosition = spriteRect.min;
        this.continentTitleShadow.transform.localPosition = spriteRect.min;
        if (spriteRect.x != this.continentTitleText.width || spriteRect.y != this.continentTitleText.height)
        {
            this.continentTitleText.SetDimensions((Int32)spriteRect.width, (Int32)spriteRect.height);
            this.continentTitleShadow.SetDimensions((Int32)spriteRect.width, (Int32)spriteRect.height);
        }
    }

    public void ShowContinentTitle(Int32 fadeInFrames)
    {
        Single fadeInDuration = this.CalculateTitleFadeDuration(fadeInFrames);
        this.continentTitleTextFader.fadeInDuration = fadeInDuration;
        this.continentTitleShadowFader.fadeInDuration = fadeInDuration;
        this.continentTitleTextFader.FadeIn(null);
        this.continentTitleShadowFader.FadeIn(null);
    }

    public void HideContinentTitle(Int32 fadeOutFrames)
    {
        Single fadeOutDuration = this.CalculateTitleFadeDuration(fadeOutFrames);
        this.continentTitleTextFader.fadeOutDuration = fadeOutDuration;
        this.continentTitleShadowFader.fadeOutDuration = fadeOutDuration;
        this.continentTitleTextFader.FadeOut(null);
        this.continentTitleShadowFader.FadeOut(null);
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
        Single seconds = frames / 30f;
        return HonoBehaviorSystem.Instance.IsFastForwardModeActive() ? seconds / FF9StateSystem.Settings.FastForwardFactor : seconds;
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
        if (isVisible && FF9StateSystem.Common.FF9.wldMapNo == 9005)
        {
            // Fix #670: World Map - Hilda Garde 1. Maybe it should be always done like that when SetMinimapVisible is called
            this.currentState = isVisible ? WorldHUD.State.HUD : WorldHUD.State.HUDNoMiniMap;
            this.DisplayChocographLocation(isVisible);
        }
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

    /*
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
    */

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
            UICamera.onMouseMove += this.OnMouseMove;
        base.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
        this.chocographLocationSprite = this.ChocographLocationPanel.GetChild(0).GetComponent<UISprite>();
        this.menuButtonLabelGameObject = this.MenuButtonGameObject.GetChild(2);
        this.specialButtonSprite = this.SpecialButtonGameObject.GetChild(2).GetComponent<UISprite>();
        this.specialButton = this.SpecialButtonGameObject.GetComponent<OnScreenButton>();
        this.perspectiveButtonSprite = this.PerspectiveButtonGameObject.GetChild(0).GetComponent<UISprite>();
        this.rotationLockButtonSprite = this.RotationLockButtonGameObject.GetChild(0).GetComponent<UISprite>();
        this.moveLeftGameObject = this.CommonButtonPanel.GetChild(0);
        this.moveRightGameObject = this.CommonButtonPanel.GetChild(1);
        UIEventListener.Get(this.moveLeftGameObject).onPress += this.OnPressButton;
        UIEventListener.Get(this.moveRightGameObject).onPress += this.OnPressButton;
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
        UIEventListener.Get(this.mapWidget.gameObject).onHover += this.OnFullMapHover;
        UIEventListener.Get(this.mapWidget.gameObject).onNavigate += this.OnFullMapNavigator;
        UIEventListener.Get(this.mapWidget.gameObject).onClick += this.onClick;
        UIEventListener.Get(this.MiniMapPanel).onClick += this.onClick;
        this.mapLocationLabel.DefaultTextColor = FF9TextTool.White;
        this.PlaneButtonPanel.GetChild(0).GetComponent<OnScreenButton>().SetHighlightKeyCommand(Control.Down);
        this.PlaneButtonPanel.GetChild(1).GetComponent<OnScreenButton>().SetHighlightKeyCommand(Control.Up);
        this.ChocographLocationPanel.transform.localPosition += new Vector3(158f, -708f, 0f);
        //this.ChocographLocationPanel.transform.localPosition = new Vector3(600f, 0f, 0f);
        //this.miniMapWidget.transform.parent.localPosition = new Vector3(250f, -500f, 0f);
        // Todo: fix Title displaying permanently if the World Map was exited during its first appearance
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

    private String[] locationName; // Dummied, use "FF9TextTool.GetTableText(0u)" directly instead

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

    [NonSerialized]
    private Dialog navigateDialog;

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
