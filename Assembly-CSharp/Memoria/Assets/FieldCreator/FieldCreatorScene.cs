using Assets.Scripts.Common;
using Memoria.Prime;
using Memoria.Scenes;
using Memoria.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using Object = System.Object;

namespace Memoria.Assets
{
    public static class FieldCreatorScene
    {
        private const String EditorDirectory = "MemoriaFieldCreator";
        private const String InternalFieldsDirectory = "InternalFields";
        private const String CustomFieldsDirectory = "CustomFields";
        private const String ControlFileName = "Configuration.txt";
        private const String ControlFilePath = EditorDirectory + "/" + ControlFileName;

        public static Boolean initialized = false;
        private static Boolean isUpdatingControl = false;
        private static FileSystemWatcher fileControlWatcher;
        private static GameObject fieldMapGo;

        private static ControlPanel controlPanel;
        private static Int32 controlTimeoutAutoShow;
        private static Int32 controlLastActivePanel;
        private static Int32 controlInformationPanel;
        private static Int32 controlSetupCameraPanel;
        private static Int32 controlSetupOverlayPanel;
        private static Int32 controlSetupAnimationPanel;
        private static Int32 controlSetupWalkmeshPanel;
        private static UILabel controlInformationLabel;
        private static UILabel controlSetupCameraHelp;
        private static UILabel controlSetupOverlayHelp;
        private static ControlList<String> controlLoadInternal;
        private static ControlList<String> controlLoadCustom;
        private static ControlRoll<String> controlSelectCharacter;
        private static ControlRoll<Int32> controlSelectWalkpath;
        private static List<ControlRoll<Int32>> controlListSelectCamera;
        private static ControlRoll<Int32> controlSelectOverlay;
        private static ControlToggle controlShowWalkmesh;
        private static ControlToggle controlShowCharacter;
        private static ControlToggle controlCameraModeOption;
        private static ControlToggle controlAnchorCameraOption;
        private static ControlToggle controlWalkpathActiveOption;
        private static ControlToggle controlWalkpathFootstepOption;
        private static ControlToggle controlWalkpathNoNPCOption;
        private static ControlToggle controlWalkpathNoPCOption;
        private static ControlInput controlSaveFileName;
        private static ControlSlider controlCameraDistance;
        private static ControlSlider controlOverlayDepth;
        private static ControlHitBox controlSetupCamera;
        private static ControlHitBox controlSetupOverlay;
        private static ControlHitBox controlSetupAnimation;
        private static ControlHitBox controlSetupWalkmesh;
        private static ControlHitBox controlSaveOption;
        private static ControlHitBox controlCameraDistanceReset;
        private static ControlHitBox controlOverlayDepthReset;
        private static ControlHelp controlHelpOption;

        private static BGI_DEF bgi;
        private static Boolean shouldRefreshWalkmesh;
        private static Boolean walkmeshIsShown = true;
        private static Int32 walkpathSelection = 0;
        private static String walkmeshFilePath = String.Empty;
        private static GameObject walkmeshFreeCameraGo;

        private static Camera fieldCamera;
        private static BGSCENE_DEF scene;
        private static String backgroundFileName;
        private static Int32 cameraSelection = 0;
        private static Int32 overlaySelection = 0;
        private static Vector3 cameraPosition = Vector3.zero;
        private static Vector3 cameraTarget = Vector3.zero;
        private static GameObject overlayHighlightGo;

        private static String dummyModelName;
        private static GameObject dummyGo;
        private static Vector3 dummyPosition;
        private static Single scalingFactor = 1f;

        private static List<GameObject> anchorWalkmeshGo;
        private static List<GameObject> anchorScreenGo;
        private static List<GameObject> anchorLinkGo;

        private static Boolean freeCameraMode = false;
        private static Boolean mouseLeftPressed;
        private static Boolean mouseRightPressed;
        private static Vector3 mousePreviousPosition;
        private static PointScreenAnchor cameraAnchor;
        private static Int32 cameraAnchorStep = -1;

        public static void Init()
        {
            GameObject mapRootGo = GameObject.Find("FieldMap Root");
            if (mapRootGo == null)
                return;
            bgi = new BGI_DEF();
            scene = new BGSCENE_DEF(true);
            fieldMapGo = new GameObject("FieldMap");
            fieldMapGo.transform.parent = mapRootGo.transform;
            controlListSelectCamera = new List<ControlRoll<Int32>>();
            cameraAnchor = new PointScreenAnchor();
            anchorWalkmeshGo = new List<GameObject>();
            anchorScreenGo = new List<GameObject>();
            anchorLinkGo = new List<GameObject>();
            controlPanel = new ControlPanel(PersistenSingleton<UIManager>.Instance.transform, "Control Panel");

            Camera camera = GetCamera();
            camera.transform.position = cameraPosition;

            if (!Directory.Exists(EditorDirectory))
                Directory.CreateDirectory(EditorDirectory);
            //if (!File.Exists(ControlFilePath))
            //	CreateNewControlFile();
            //ReadControlFile();
            //fileControlWatcher = CreateWatcher();

            SceneDirector.ClearFadeColor();
            FPSManager.SetTargetFPS(Configuration.Graphics.MenuFPS);
            FPSManager.SetMainLoopSpeed(Configuration.Graphics.MenuTPS);
            //Process.Start(Path.GetFullPath(ControlFilePath));

            ControlChangeDummyModel("GEO_MAIN_F4_ZDN");
            ControlChangePath("FBG_N00_TSHP_MAP002_TH_CGR_1", true);

            // Main panel
            controlInformationPanel = controlPanel.CreateSubPanel("Information");
            controlInformationLabel = controlPanel.AddSimpleLabel("", NGUIText.Alignment.Center, -1, controlInformationPanel);
            List<String> internalFields = new List<String>(EventEngineUtils.eventIDToFBGID.Values);
            List<String> externalFields = FindExternalFields();
            List<String> dummyModels = new List<String>()
            {
                "GEO_MAIN_F4_ZDN",
                "GEO_MAIN_F0_VIV",
                "GEO_MAIN_F0_GRN",
                "GEO_MAIN_F0_STN",
                "GEO_MAIN_F0_FRJ",
                "GEO_MAIN_F0_KUI",
                "GEO_MAIN_F0_EIK",
                "GEO_MAIN_F0_SLM",
                "GEO_MON_B3_124",
                "GEO_MON_F0_CDR",
                "GEO_MON_B3_188",
                "GEO_NPC_F3_MOG"
            };
            controlLoadInternal = controlPanel.AddListOneTimeSelection("Load Internal Field", internalFields, str => str, str => str != "invalidFieldMapID", str => ControlChangePath(str, true));
            controlLoadInternal.ListOpener.Label.rightAnchor.Set(controlPanel.BasePanel.transform, 0.5f, -controlPanel.elementSeparatorWidth / 2);
            controlLoadCustom = controlPanel.AddListOneTimeSelection("Load Custom Field", externalFields, str => str, str => true, str => ControlChangePath(str, false));
            controlLoadCustom.ListOpener.IsEnabled = !String.IsNullOrEmpty(externalFields[0]);
            controlPanel.PanelAddRow();
            controlShowWalkmesh = controlPanel.AddToggleOption("Show walkmesh", walkmeshIsShown, ShowWalkmesh);
            controlShowWalkmesh.Label.rightAnchor.Set(controlPanel.BasePanel.transform, 0.5f, -controlPanel.elementSeparatorWidth / 2);
            controlShowCharacter = controlPanel.AddToggleOption("Show character", true, b => dummyGo?.SetActive(b));
            controlPanel.PanelAddRow();

            // Setup Cameras panel
            controlSetupCameraPanel = controlPanel.CreateSubPanel("Setup Cameras");
            controlListSelectCamera.Add(controlPanel.AddRollOption(ControlRoll<Int32>.GetNonEmptyIndexList(scene.cameraList.Count), sel => $"Camera {sel}", sel => ChangeCameraSelection(sel, false), controlSetupCameraPanel));
            controlPanel.PanelAddRow(controlSetupCameraPanel);
            controlAnchorCameraOption = controlPanel.AddToggleOption("Select Anchors", false, StartAnchors, controlSetupCameraPanel);
            controlPanel.PanelAddRow(controlSetupCameraPanel);
            controlCameraDistance = controlPanel.AddSlider("Distance Factor", 1f, val => ControlSlider.LogScaleIn(val, 10f), t => ControlSlider.LogScaleOut(t, 10f), SetScalingFactor, controlSetupCameraPanel);
            controlCameraDistanceReset = controlPanel.AddHitBoxOption("Reset", () => SetScalingFactor(1f), controlSetupCameraPanel);
            controlPanel.PanelAddRow(controlSetupCameraPanel);
            controlSetupCameraHelp = controlPanel.AddSimpleLabel(SetupCameraHelp, NGUIText.Alignment.Left, 9, controlSetupCameraPanel);
            controlSetupCamera = controlPanel.AddHitBoxOption("Setup Cameras", () => controlPanel.SetActivePanel(true, controlSetupCameraPanel));
            controlPanel.PanelAddRow();
            // Setup Overlays panel
            controlSetupOverlayPanel = controlPanel.CreateSubPanel("Setup Overlays");
            controlListSelectCamera.Add(controlPanel.AddRollOption(ControlRoll<Int32>.GetNonEmptyIndexList(scene.cameraList.Count), sel => $"Camera {sel}", sel => ChangeCameraSelection(sel, false), controlSetupOverlayPanel));
            controlPanel.PanelAddRow(controlSetupOverlayPanel);
            controlSelectOverlay = controlPanel.AddRollOption(ControlRoll<Int32>.GetNonEmptyIndexList(scene.overlayList.Count), sel => $"Overlay {sel}", ChangeOverlaySelection, controlSetupOverlayPanel);
            controlPanel.PanelAddRow(controlSetupOverlayPanel);
            controlOverlayDepth = controlPanel.AddSlider("Depth", 0f, val => ControlSlider.LinearScaleIn(val, 0f, 5000f), t => ControlSlider.LinearScaleOut(t, 0f, 5000f), val => SetOverlayDepth(overlaySelection, val), controlSetupOverlayPanel);
            controlOverlayDepthReset = controlPanel.AddHitBoxOption("Reset", () => ResetOverlayDepth(overlaySelection), controlSetupOverlayPanel);
            controlPanel.PanelAddRow(controlSetupOverlayPanel);
            controlSetupOverlayHelp = controlPanel.AddSimpleLabel(SetupOverlayHelp, NGUIText.Alignment.Left, 4, controlSetupOverlayPanel);
            controlSetupOverlay = controlPanel.AddHitBoxOption("Setup Overlays", () => controlPanel.SetActivePanel(true, controlSetupOverlayPanel));
            controlPanel.PanelAddRow();
            // TODO: Setup Animations panel
            //controlSetupAnimationPanel = controlPanel.CreateSubPanel("Setup Animations");
            //controlPanel.AddHitBoxOption("Do nothing", () => controlPanel.SetActivePanel(false), controlSetupAnimationPanel);
            //controlSetupAnimation = controlPanel.AddHitBoxOption("Setup Animations", () => controlPanel.SetActivePanel(true, controlSetupAnimationPanel));
            //controlPanel.PanelAddRow();
            // Setup Walkmesh panel
            controlSetupWalkmeshPanel = controlPanel.CreateSubPanel("Setup Walkmesh");
            controlSelectWalkpath = controlPanel.AddRollOption(ControlRoll<Int32>.GetNonEmptyIndexList(bgi.floorList.Count), sel => $"Walkpath {sel}", ChangeWalkpathSelection, controlSetupWalkmeshPanel);
            controlPanel.PanelAddRow(controlSetupWalkmeshPanel);
            controlWalkpathActiveOption = controlPanel.AddToggleOption("Active by default", false, b => SetWalkpathFlag(walkpathSelection, 1, b), controlSetupWalkmeshPanel);
            controlPanel.PanelAddRow(controlSetupWalkmeshPanel);
            controlWalkpathFootstepOption = controlPanel.AddToggleOption("Alternate footstep", false, b => SetWalkpathFlag(walkpathSelection, 0x1000, b), controlSetupWalkmeshPanel);
            controlPanel.PanelAddRow(controlSetupWalkmeshPanel);
            controlWalkpathNoNPCOption = controlPanel.AddToggleOption("Prevent NPC pathing", false, b => SetWalkpathFlag(walkpathSelection, 0x4000, b), controlSetupWalkmeshPanel);
            controlPanel.PanelAddRow(controlSetupWalkmeshPanel);
            controlWalkpathNoPCOption = controlPanel.AddToggleOption("Prevent PC pathing", false, b => SetWalkpathFlag(walkpathSelection, 0x8000, b), controlSetupWalkmeshPanel);
            controlSetupWalkmesh = controlPanel.AddHitBoxOption("Setup Walkmesh", () => controlPanel.SetActivePanel(true, controlSetupWalkmeshPanel));
            controlPanel.PanelAddRow();

            controlCameraModeOption = controlPanel.AddToggleOption("Free-camera mode (experimental)", freeCameraMode, SetFreeCameraMode);
            controlPanel.PanelAddRow();
            controlSelectCharacter = controlPanel.AddRollOption(dummyModels, str => str, ControlChangeDummyModel);
            controlSelectCharacter.Loop = false;
            controlPanel.PanelAddRow();
            controlListSelectCamera.Add(controlPanel.AddRollOption(ControlRoll<Int32>.GetNonEmptyIndexList(scene.cameraList.Count), sel => $"Camera {sel}", sel => ChangeCameraSelection(sel, false)));
            controlPanel.PanelAddRow();
            controlSaveFileName = controlPanel.AddInputField("CUSTOM_FIELD_000", UIInput.Validation.Filename);
            controlSaveOption = controlPanel.AddHitBoxOption("Save", ExportField);
            controlPanel.PanelAddRow();
            controlHelpOption = controlPanel.AddHelpSubPanel("Help", ControlPanelHelp);
            controlPanel.EndInitialization();
            controlTimeoutAutoShow = 2;
            controlLastActivePanel = 0;
            initialized = true;
        }

        public static void GenerateWalkmesh()
        {
            if (!shouldRefreshWalkmesh)
                return;
            shouldRefreshWalkmesh = false;
            foreach (BGCAM_DEF bgCamera in scene.cameraList)
            {
                if (bgCamera.projectedWalkMesh != null)
                {
                    UnityEngine.Object.Destroy(bgCamera.projectedWalkMesh);
                    bgCamera.projectedWalkMesh = null;
                }
            }
            if (walkmeshFreeCameraGo != null)
            {
                UnityEngine.Object.Destroy(walkmeshFreeCameraGo);
                walkmeshFreeCameraGo = null;
            }
            if (walkmeshIsShown)
            {
                if (freeCameraMode)
                    walkmeshFreeCameraGo = WalkMesh.CreateWalkMesh(fieldMapGo.transform, bgi, new Color(0.5f, 0f, 0.8f, 0.5f));
                else
                    WalkMesh.CreateProjectedWalkMesh(fieldMapGo.transform, scene, bgi, cameraSelection, new Color(0.5f, 0f, 0.8f, 0.5f));
                UpdateSelection();
            }
        }

        public static void Update()
        {
            if (isUpdatingControl)
                return;
            if (controlTimeoutAutoShow > 0 && --controlTimeoutAutoShow == 0)
                controlPanel.Show = true;
            Boolean mouseLeftWasPressed = mouseLeftPressed;
            Boolean mouseRightWasPressed = mouseRightPressed;
            mouseLeftPressed = false;
            mouseRightPressed = false;
            if (Input.GetKeyDown(KeyCode.Space) && UICamera.selectedObject?.GetComponent<UIInput>() == null)
                controlPanel.Show = !controlPanel.Show;
            if (Input.GetKeyDown(KeyCode.Escape) && controlPanel.Show)
            {
                if (controlPanel.ActivePanelIndex == controlSetupCameraPanel)
                    StartAnchors(false);
                controlPanel.SetActivePanel(false);
            }
            if (Input.mouseScrollDelta.y != 0f)
            {
                ScaleCameraView(cameraTarget, Input.mouseScrollDelta.y);
            }
            if (Input.GetMouseButton(0))
            {
                if (mouseLeftWasPressed)
                {
                    Vector3 mouseDelta = Input.mousePosition - mousePreviousPosition;
                    if (controlPanel.ActivePanelIndex == 0)
                    {
                        if (Math.Abs(mouseDelta.x) > Math.Abs(mouseDelta.y))
                            RotateCameraView(cameraTarget, Vector3.up, mouseDelta.x);
                        else
                            RotateCameraView(cameraTarget, GetCamera().transform.right, -mouseDelta.y);
                    }
                }
                else
                {
                    if (controlPanel.ActivePanelIndex == controlSetupCameraPanel && cameraAnchorStep >= 0)
                    {
                        Int32 anchorIndex = cameraAnchorStep / 2;
                        if ((cameraAnchorStep % 2) == 0)
                            cameraAnchor.WorldPoint[anchorIndex] = cameraAnchor.GetClosestWalkmeshPoint(cameraAnchor.GetMouseScreenPoint());
                        else
                            cameraAnchor.ScreenPoint[anchorIndex] = cameraAnchor.GetMouseScreenPoint();
                        cameraAnchorStep++;
                        if (cameraAnchorStep >= 2 * cameraAnchor.WorldPoint.Length)
                        {
                            cameraAnchor.PerformAnchorOnCamera();
                            StartAnchors(false);
                            shouldRefreshWalkmesh = true;
                        }
                    }
                }
                mouseLeftPressed = true;
            }
            if (Input.GetMouseButton(1))
            {
                if (mouseRightWasPressed)
                {
                    Vector3 mouseDelta = Input.mousePosition - mousePreviousPosition;
                    if (controlPanel.ActivePanelIndex != controlSetupOverlayPanel || (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)))
                        MoveCameraView(-0.5f * (mouseDelta.y * GetCamera().transform.up + mouseDelta.x * GetCamera().transform.right));
                    else
                        MoveOverlayXY(overlaySelection, (Int16)(0.5f * mouseDelta.x), (Int16)(-0.5f * mouseDelta.y));
                }
                mouseRightPressed = true;
            }
            mousePreviousPosition = Input.mousePosition;
            if (controlLastActivePanel != controlPanel.ActivePanelIndex)
            {
                OnActivePanelChange(controlLastActivePanel, controlPanel.ActivePanelIndex);
                controlLastActivePanel = controlPanel.ActivePanelIndex;
            }
            UpdateRender();
        }

        private static void OnActivePanelChange(Int32 previousPanel, Int32 currentPanel)
        {
            if (previousPanel == 0)
            {
                SetFreeCameraMode(false);
                controlCameraModeOption.IsToggled = false;
            }
            if (previousPanel == controlSetupWalkmeshPanel)
                ShowWalkmesh(controlShowWalkmesh.IsToggled);
            else if (currentPanel == controlSetupWalkmeshPanel)
                ShowWalkmesh(true);
            if (previousPanel == controlSetupOverlayPanel || currentPanel == controlSetupOverlayPanel)
                ChangeOverlaySelection(overlaySelection);
            if (previousPanel == controlSetupWalkmeshPanel || currentPanel == controlSetupWalkmeshPanel)
                ChangeWalkpathSelection(walkpathSelection);
        }

        private static void UpdateRender()
        {
            GenerateWalkmesh();
            if (cameraAnchor.CanBeUsed)
            {
                dummyPosition = scalingFactor * cameraAnchor.GetClosestWalkmeshPoint(cameraAnchor.GetMouseScreenPoint());
                if (controlPanel.ActivePanelIndex == controlSetupCameraPanel && cameraAnchorStep >= 0)
                {
                    Int32 walkmeshGoCount = (cameraAnchorStep + 1) / 2;
                    Int32 screenGoCount = cameraAnchorStep / 2;
                    while (anchorWalkmeshGo.Count < walkmeshGoCount)
                        anchorWalkmeshGo.Add(ModelFactory.CreateUIModel(PrimitiveType.Sphere, Color.green, 5f, cameraAnchor.GetWorldPointScreenPos(anchorWalkmeshGo.Count)));
                    while (anchorScreenGo.Count < screenGoCount)
                        anchorScreenGo.Add(ModelFactory.CreateUIModel(PrimitiveType.Sphere, Color.blue, 5f, cameraAnchor.ScreenPoint[anchorScreenGo.Count]));
                    while (anchorLinkGo.Count < screenGoCount)
                    {
                        Vector3 origin = cameraAnchor.ScreenPoint[anchorLinkGo.Count];
                        Vector3 dest = cameraAnchor.GetWorldPointScreenPos(anchorLinkGo.Count);
                        anchorLinkGo.Add(ModelFactory.CreateUIModel(PrimitiveType.Cylinder, Color.white, 1f, origin + Vector3.forward, dest + Vector3.forward));
                    }
                }
                else
                {
                    foreach (GameObject go in anchorWalkmeshGo)
                        UnityEngine.Object.Destroy(go);
                    foreach (GameObject go in anchorScreenGo)
                        UnityEngine.Object.Destroy(go);
                    foreach (GameObject go in anchorLinkGo)
                        UnityEngine.Object.Destroy(go);
                    anchorWalkmeshGo.Clear();
                    anchorScreenGo.Clear();
                    anchorLinkGo.Clear();
                }
            }
            else if (walkpathSelection < bgi.floorList.Count)
            {
                BGI_FLOOR_DEF floor = bgi.floorList[walkpathSelection];
                if (floor.triNdxList.Count > 0)
                {
                    BGI_TRI_DEF tri = bgi.triList[floor.triNdxList[0]];
                    BGI_VEC_DEF v = bgi.vertexList[tri.vertexNdx[0]];
                    dummyPosition = scalingFactor * (bgi.orgPos.ToVector3() + floor.orgPos.ToVector3() + v.ToVector3());
                    dummyPosition.y *= -1;
                }
            }
            if (dummyGo != null)
                dummyGo.transform.localPosition = dummyPosition;
        }

        private static void CreateNewCamera()
        {
            if (cameraSelection >= scene.cameraList.Count)
                return;
            BGCAM_DEF bgCamera = scene.cameraList[cameraSelection].Copy();
            scene.cameraList.Add(bgCamera);
            cameraSelection = scene.cameraList.Count - 1;
            foreach (ControlRoll<Int32> cameraSelector in controlListSelectCamera)
            {
                cameraSelector.ChangeList(ControlRoll<Int32>.GetNonEmptyIndexList(scene.cameraList.Count), cameraSelection);
                cameraSelector.IsEnabled = true;
            }
        }

        private static void StartAnchors(Boolean start = true)
        {
            if (start)
            {
                if (cameraSelection >= scene.cameraList.Count)
                    start = false;
                else
                    cameraAnchor.Init(scene.cameraList[cameraSelection], bgi);
                if (!cameraAnchor.CanBeUsed)
                    start = false;
            }
            if (start)
            {
                cameraAnchorStep = 0;
            }
            else
            {
                cameraAnchorStep = -1;
                controlAnchorCameraOption.IsToggled = false;
            }
            foreach (ControlRoll<Int32> cameraSelector in controlListSelectCamera)
                cameraSelector.IsEnabled = !start && scene.cameraList.Count > 0;
        }

        private static void ExportField()
        {
            try
            {
                String fileName = controlSaveFileName.Input.value;
                if (String.IsNullOrEmpty(fileName))
                    throw new Exception($"File name must not be empty");

                // Apply the scalingFactor for real before saving
                foreach (BGCAM_DEF bgCamera in scene.cameraList)
                    for (Int32 i = 0; i < 3; i++)
                        bgCamera.t[i] = (Int32)(scalingFactor * bgCamera.t[i]);
                bgi.orgPos = BGI_VEC_DEF.FromVector3(scalingFactor * bgi.orgPos.ToVector3());
                bgi.curPos = BGI_VEC_DEF.FromVector3(scalingFactor * bgi.curPos.ToVector3());
                bgi.charPos = BGI_VEC_DEF.FromVector3(scalingFactor * bgi.charPos.ToVector3());
                foreach (BGI_FLOOR_DEF floor in bgi.floorList)
                {
                    floor.orgPos = BGI_VEC_DEF.FromVector3(scalingFactor * floor.orgPos.ToVector3());
                    floor.curPos = BGI_VEC_DEF.FromVector3(scalingFactor * floor.curPos.ToVector3());
                }
                for (Int32 i = 0; i < bgi.vertexList.Count; i++)
                    bgi.vertexList[i] = BGI_VEC_DEF.FromVector3(scalingFactor * bgi.vertexList[i].ToVector3());
                scalingFactor = 1f;
                controlCameraDistance.Value = scalingFactor;

                // Update the overlay Org positions with their Cur positions
                foreach (BGOVERLAY_DEF bgOverlay in scene.overlayList)
                {
                    bgOverlay.orgX = (short)bgOverlay.curX;
                    bgOverlay.orgY = (short)bgOverlay.curY;
                    bgOverlay.orgZ = bgOverlay.curZ;
                }

                String fileBaseDirectory = $"{EditorDirectory}/{CustomFieldsDirectory}/{fileName}";
                String fileBasePath = $"{fileBaseDirectory}/{fileName}";
                Directory.CreateDirectory(fileBaseDirectory);
                WavefrontObject obj = new WavefrontObject();
                obj.LoadFromBGI(bgi);
                File.WriteAllText($"{fileBasePath}.obj", obj.GenerateFile(fileName));
                using (FileStream walkmeshStream = File.Open($"{fileBasePath}.bgi.bytes", FileMode.Create, FileAccess.Write))
                using (BinaryWriter writer = new BinaryWriter(walkmeshStream))
                    bgi.WriteData(writer);
                scene.ExportMemoriaBGX($"{fileBasePath}{BGSCENE_DEF.MemoriaBGXExtension}");
                // TODO: updating controlLoadCustom.ObjectList doesn't work very well for now
                //List<String> externalFields = FindExternalFields();
                //controlLoadCustom.ObjectList = externalFields;
                //controlLoadCustom.ListOpener.IsEnabled = !String.IsNullOrEmpty(externalFields[0]);
                controlInformationLabel.text = $"Saved successfully\nPress ESCAPE";
                controlPanel.SetActivePanel(true, controlInformationPanel);
            }
            catch (Exception err)
            {
                controlInformationLabel.text = $"Save failed:\n{err}";
                controlPanel.SetActivePanel(true, controlInformationPanel);
            }
        }

        private static List<String> FindExternalFields()
        {
            List<String> list;
            if (!Directory.Exists($"{EditorDirectory}/{CustomFieldsDirectory}"))
                list = new List<String>();
            else
                list = new List<String>(Directory.GetDirectories($"{EditorDirectory}/{CustomFieldsDirectory}")).ConvertAll(path => Path.GetFileName(path));
            if (list.Count == 0)
                list.Add(String.Empty);
            return list;
        }

        private static void ControlChangePath(String newPath, Boolean isInternalPath)
        {
            ControlChangeWalkmesh(newPath, isInternalPath);
            ControlChangeBackground(newPath, isInternalPath);
            scalingFactor = 1f;
            if (initialized)
                controlCameraDistance.Value = scalingFactor;
        }

        private static void ControlChangeWalkmesh(String newPath, Boolean isInternalPath)
        {
            if (newPath == walkmeshFilePath)
                return;
            walkmeshFilePath = newPath;
            walkpathSelection = 0;
            if (isInternalPath)
            {
                bgi = new BGI_DEF();
                bgi.LoadBGI(null, FieldMap.GetMapResourcePath(newPath), newPath);
            }
            else
            {
                String objPath = $"{EditorDirectory}/{CustomFieldsDirectory}/{newPath}/{newPath}.obj";
                String bgiPath = $"{EditorDirectory}/{CustomFieldsDirectory}/{newPath}/{newPath}.bgi.bytes";
                if (File.Exists(objPath) && !File.Exists(bgiPath)) // Priority to .bgi.bytes
                {
                    WavefrontObject walkmeshObj = new WavefrontObject();
                    walkmeshObj.LoadFromFile(objPath);
                    bgi = walkmeshObj.ConvertToBGI();
                }
                else
                {
                    bgi = new BGI_DEF();
                    if (File.Exists(bgiPath))
                    {
                        Byte[] binAsset = File.ReadAllBytes(bgiPath);
                        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(binAsset)))
                            bgi.ReadData(binaryReader);
                    }
                }
            }
            if (initialized)
            {
                controlSelectWalkpath.ChangeList(ControlRoll<Int32>.GetNonEmptyIndexList(bgi.floorList.Count), walkpathSelection);
                controlSelectWalkpath.IsEnabled = bgi.floorList.Count > 0;
            }
            shouldRefreshWalkmesh = true;
        }

        private static void ControlChangeBackground(String newName, Boolean isInternalPath)
        {
            if (newName == backgroundFileName)
                return;
            backgroundFileName = newName;
            cameraSelection = 0;
            overlaySelection = 0;
            DestroyMemoriaScene();
            shouldRefreshWalkmesh = true;
            scene = new BGSCENE_DEF(true);
            String filePath;
            if (isInternalPath)
            {
                filePath = $"{EditorDirectory}/{InternalFieldsDirectory}/{newName}/{newName}{BGSCENE_DEF.MemoriaBGXExtension}";
                if (!File.Exists(filePath))
                {
                    scene.LoadResources(FieldMap.GetMapResourcePath(newName), newName);
                    scene.ExportMemoriaBGX(filePath);
                    scene = new BGSCENE_DEF(true);
                }
            }
            else
            {
                filePath = $"{EditorDirectory}/{CustomFieldsDirectory}/{newName}/{newName}{BGSCENE_DEF.MemoriaBGXExtension}";
            }
            if (File.Exists(filePath))
            {
                scene.name = newName;
                scene.ReadMemoriaBGS(filePath);
                scene.CreateMemoriaScene(fieldMapGo.transform, freeCameraMode);
                ChangeCameraSelection(cameraSelection, true);
                ChangeOverlaySelection(overlaySelection);
            }
            foreach (ControlRoll<Int32> cameraSelector in controlListSelectCamera)
            {
                cameraSelector.ChangeList(ControlRoll<Int32>.GetNonEmptyIndexList(scene.cameraList.Count), cameraSelection);
                cameraSelector.IsEnabled = scene.cameraList.Count > 0;
            }
            if (initialized)
            {
                controlSelectOverlay.ChangeList(ControlRoll<Int32>.GetNonEmptyIndexList(scene.overlayList.Count), overlaySelection);
                controlSelectOverlay.IsEnabled = scene.overlayList.Count > 0;
                controlOverlayDepth.IsEnabled = scene.overlayList.Count > 0;
                controlOverlayDepthReset.IsEnabled = scene.overlayList.Count > 0;
            }
        }

        private static void ControlChangeDummyModel(String newModel)
        {
            if (newModel == dummyModelName)
                return;
            dummyModelName = newModel;
            if (dummyGo != null)
                UnityEngine.Object.Destroy(dummyGo);
            dummyGo = ModelFactory.CreateModel(newModel);
            SetupDummy();
            dummyGo?.SetActive(controlShowCharacter?.IsToggled ?? true);
        }

        private static void SetupDummy()
        {
            if (dummyGo != null)
            {
                //GeoTexAnim.addTexAnim(dummyGo, newModel);
                dummyGo.transform.parent = fieldMapGo.transform;
                dummyGo.transform.localScale = new Vector3(-1f, -1f, 1f);
                dummyGo.transform.localPosition = dummyPosition;
                foreach (Renderer renderer in dummyGo.GetComponentsInChildren<Renderer>())
                {
                    foreach (Material material in renderer.materials)
                    {
                        if (freeCameraMode)
                        {
                            material.shader = ShadersLoader.Find("Unlit/Transparent Cutout");
                        }
                        else
                        {
                            material.shader = ShadersLoader.Find("PSX/FieldMapActor");
                            material.SetColor("_Color", new Color32(128, 128, 128, 255));
                        }
                    }
                }
                // Prevent mesh culling, since in PSX render mode the position depends not only on UnityEngine.Camera but also on BGCAM_DEF.GetMatrixRT
                foreach (MeshFilter renderer in dummyGo.GetComponentsInChildren<MeshFilter>())
                    renderer.sharedMesh.bounds = new Bounds(Vector3.zero, Vector3.one * Single.MaxValue * 0.01f);
                foreach (SkinnedMeshRenderer renderer in dummyGo.GetComponentsInChildren<SkinnedMeshRenderer>())
                    renderer.localBounds = new Bounds(Vector3.zero, Vector3.one * Single.MaxValue * 0.01f);
            }
        }

        private static void DestroyMemoriaScene()
        {
            GameObject backgroundGo = fieldMapGo.transform.FindChild("Background")?.gameObject;
            if (backgroundGo != null)
                UnityEngine.Object.Destroy(backgroundGo);
            foreach (BGCAM_DEF bgCamera in scene.cameraList)
            {
                if (bgCamera.projectedWalkMesh != null)
                {
                    UnityEngine.Object.Destroy(bgCamera.projectedWalkMesh);
                    bgCamera.projectedWalkMesh = null;
                }
            }
        }

        private static void CreateNewControlFile()
        {
            String controlFile = "Background: Background.bgx\n" +
                                    "Walkmesh: Walkmesh.obj\n" +
                                    "DummyModel: GEO_MAIN_F4_ZDN";
            File.WriteAllText(ControlFilePath, controlFile);
        }

        private static void ShowWalkmesh(Boolean show)
        {
            if (show == walkmeshIsShown)
                return;
            shouldRefreshWalkmesh = true;
            walkmeshIsShown = show;
        }

        private static void SetFreeCameraMode(Boolean free)
        {
            if (freeCameraMode == free)
                return;
            freeCameraMode = free;
            shouldRefreshWalkmesh = true;
            DestroyMemoriaScene();
            scene.CreateMemoriaScene(fieldMapGo.transform, freeCameraMode);
            SetupDummy();
            Camera camera = GetCamera();
            cameraPosition = free ? new Vector3(0f, 0f, -10000f) : Vector3.zero;
            cameraTarget = Vector3.zero;
            if (cameraSelection < scene.cameraList.Count)
            {
                ChangeCameraSelection(cameraSelection, true);
            }
            else
            {
                camera.transform.position = cameraPosition;
                if (free)
                    camera.transform.LookAt(cameraTarget);
            }
            // TODO: fix free-camera mode (overlay positions should be computed better + setup correctly the initial camera position/orientation)
        }

        private static void ChangeWalkpathSelection(Int32 newSelection)
        {
            if (bgi.floorList.Count == 0)
                return;
            while (newSelection < 0)
                newSelection += bgi.floorList.Count;
            while (newSelection >= bgi.floorList.Count)
                newSelection -= bgi.floorList.Count;
            BGI_FLOOR_DEF floor = bgi.floorList[newSelection];
            walkpathSelection = newSelection;
            if (floor.triNdxList.Count > 0)
            {
                BGI_TRI_DEF floorTriangle = bgi.triList[bgi.floorList[newSelection].triNdxList[0]];
                controlWalkpathActiveOption.IsToggled = (floorTriangle.triFlags & 1) != 0;
                controlWalkpathFootstepOption.IsToggled = (floorTriangle.triFlags & 0x1000) != 0;
                controlWalkpathNoNPCOption.IsToggled = (floorTriangle.triFlags & 0x4000) != 0;
                controlWalkpathNoPCOption.IsToggled = (floorTriangle.triFlags & 0x8000) != 0;
                controlWalkpathActiveOption.IsEnabled = true;
            }
            else
            {
                controlWalkpathActiveOption.IsEnabled = false;
            }
            UpdateSelection();
        }

        private static void ChangeCameraSelection(Int32 newSelection, Boolean refreshPosition = true)
        {
            if (scene.cameraList.Count == 0)
                return;
            while (newSelection < 0)
                newSelection += scene.cameraList.Count;
            while (newSelection >= scene.cameraList.Count)
                newSelection -= scene.cameraList.Count;
            if (cameraSelection != newSelection)
                refreshPosition = true;
            cameraSelection = newSelection;
            BGCAM_DEF bgCamera = scene.cameraList[newSelection];
            Vector2 centerOffset = bgCamera.GetCenterOffset();
            Single halfFieldWidth = FieldMap.PsxFieldWidthNative / 2f;
            Single halfFieldHeight = FieldMap.PsxFieldHeightNative / 2f;
            Single offsetX = centerOffset.x + bgCamera.w / 2 - halfFieldWidth;
            Single offsetY = -centerOffset.y - bgCamera.h / 2 + halfFieldHeight;
            Matrix4x4 matrix = bgCamera.GetMatrixRT();
            matrix.m03 *= scalingFactor;
            matrix.m13 *= scalingFactor;
            matrix.m23 *= scalingFactor;
            Shader.SetGlobalFloat("_OffsetX", offsetX);
            Shader.SetGlobalFloat("_OffsetY", offsetY);
            Shader.SetGlobalFloat("_MulX", 1f / halfFieldWidth);
            Shader.SetGlobalFloat("_MulY", 1f / halfFieldHeight);
            Shader.SetGlobalMatrix("_MatrixRT", matrix);
            Shader.SetGlobalFloat("_ViewDistance", bgCamera.GetViewDistance());
            Shader.SetGlobalFloat("_DepthOffset", bgCamera.depthOffset);
            if (refreshPosition)
            {
                if (freeCameraMode)
                {
                    cameraPosition = 10 * bgCamera.GetCamPos();
                    cameraPosition.z *= -1;
                    //cameraPosition = new Vector3(0f, 0f, -10000f);
                    Camera camera = GetCamera();
                    camera.orthographicSize = bgCamera.GetViewDistance();
                    camera.farClipPlane = 100000f;
                    camera.transform.position = cameraPosition;
                    camera.transform.LookAt(cameraTarget);
                }
                else
                {
                    cameraPosition = centerOffset;
                    Camera camera = GetCamera();
                    camera.transform.position = cameraPosition;
                    camera.transform.localRotation = Quaternion.identity;
                    camera.orthographicSize = FieldMap.HalfFieldHeight;
                }
            }
            UpdateSelection();
            cameraAnchor.Init(scene.cameraList[cameraSelection], bgi);
            foreach (ControlRoll<Int32> cameraSelector in controlListSelectCamera)
                cameraSelector.Selection = cameraSelection;
            ChangeOverlaySelection(overlaySelection);
        }

        private static void ChangeOverlaySelection(Int32 newSelection)
        {
            if (scene.overlayList.Count == 0)
                return;
            while (newSelection < 0)
                newSelection += scene.overlayList.Count;
            while (newSelection >= scene.overlayList.Count)
                newSelection -= scene.overlayList.Count;
            overlaySelection = newSelection;
            BGOVERLAY_DEF bgOverlay = scene.overlayList[newSelection];
            if (overlayHighlightGo != null)
            {
                UnityEngine.Object.Destroy(overlayHighlightGo);
                overlayHighlightGo = null;
            }
            if (controlPanel.ActivePanelIndex == controlSetupOverlayPanel && bgOverlay.camNdx == cameraSelection)
            {
                overlayHighlightGo = new GameObject($"Overlay_{overlaySelection:D2}_Highlight");
                overlayHighlightGo.transform.parent = bgOverlay.transform;
                overlayHighlightGo.transform.localPosition = Vector3.zero;
                overlayHighlightGo.transform.localScale = Vector3.one;
                List<Vector3> vertexList = new List<Vector3>();
                List<Vector2> uvList = new List<Vector2>();
                List<Int32> triangleList = new List<Int32>();
                vertexList.Add(new Vector3(0f, 0f, -1f));
                vertexList.Add(new Vector3(bgOverlay.memoriaSize[0], 0f, -1f));
                vertexList.Add(new Vector3(bgOverlay.memoriaSize[0], bgOverlay.memoriaSize[1], -1f));
                vertexList.Add(new Vector3(0f, bgOverlay.memoriaSize[1], -1f));
                uvList.Add(new Vector2(0f, 1f));
                uvList.Add(new Vector2(1f, 1f));
                uvList.Add(new Vector2(1f, 0f));
                uvList.Add(new Vector2(0f, 0f));
                triangleList.Add(2);
                triangleList.Add(1);
                triangleList.Add(0);
                triangleList.Add(3);
                triangleList.Add(2);
                triangleList.Add(0);
                Mesh mesh = new Mesh
                {
                    vertices = vertexList.ToArray(),
                    uv = uvList.ToArray(),
                    triangles = triangleList.ToArray()
                };
                MeshRenderer meshRenderer = overlayHighlightGo.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = overlayHighlightGo.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                meshRenderer.material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_1"));
                Texture2D colorTexture = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
                colorTexture.SetPixel(0, 0, new Color(0.2f, 0.8f, 0.2f, 0.5f));
                colorTexture.Apply();
                meshRenderer.material.mainTexture = colorTexture;
            }
            if (initialized)
                controlOverlayDepth.Value = bgOverlay.curZ;
        }

        private static void UpdateSelection()
        {
            for (Int32 i = 0; i < scene.cameraList.Count; i++)
            {
                BGCAM_DEF bgCamera = scene.cameraList[i];
                bgCamera.transform.gameObject.SetActive(i == cameraSelection);
                if (bgCamera.projectedWalkMesh == null)
                    continue;
                for (Int32 j = 0; j < bgi.floorList.Count; j++)
                {
                    GameObject floorGo = bgCamera.projectedWalkMesh.transform.FindChild($"Projected_Floor_{j:D2}")?.gameObject;
                    if (floorGo == null)
                        continue;
                    Color walkpathColor = controlPanel.ActivePanelIndex == controlSetupWalkmeshPanel && j == walkpathSelection ? new Color(0.2f, 0.8f, 0.2f, 0.5f) : new Color(0.5f, 0f, 0.8f, 0.5f);
                    floorGo.GetComponent<MeshRenderer>().material.SetColor("_TintColor", walkpathColor);
                }
                bgCamera.projectedWalkMesh.SetActive(i == cameraSelection);
            }
            if (walkmeshFreeCameraGo != null)
            {
                for (Int32 j = 0; j < bgi.floorList.Count; j++)
                {
                    GameObject floorGo = walkmeshFreeCameraGo.transform.FindChild($"Floor_{j:D2}")?.gameObject;
                    if (floorGo == null)
                        continue;
                    Color walkpathColor = controlPanel.ActivePanelIndex == controlSetupWalkmeshPanel && j == walkpathSelection ? new Color(0.2f, 0.8f, 0.2f, 0.5f) : new Color(0.5f, 0f, 0.8f, 0.5f);
                    Texture2D colorTexture = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
                    colorTexture.SetPixel(0, 0, walkpathColor);
                    colorTexture.Apply();
                    floorGo.GetComponent<MeshRenderer>().material.mainTexture = colorTexture;
                }
            }
        }

        private static void MoveCameraView(Vector3 offset)
        {
            cameraPosition += offset;
            if (controlPanel.ActivePanelIndex == controlSetupCameraPanel && cameraSelection < scene.cameraList.Count)
            {
                BGCAM_DEF bgCamera = scene.cameraList[cameraSelection];
                Single halfFieldWidth = FieldMap.PsxFieldWidthNative / 2f;
                Single halfFieldHeight = FieldMap.PsxFieldHeightNative / 2f;
                cameraPosition.x = Mathf.Clamp(cameraPosition.x, bgCamera.vrpMinX - halfFieldWidth, bgCamera.vrpMaxX - halfFieldWidth);
                cameraPosition.y = Mathf.Clamp(cameraPosition.y, -bgCamera.vrpMaxY + halfFieldHeight, -bgCamera.vrpMinY + halfFieldHeight);
            }
            Camera camera = GetCamera();
            camera.transform.position = cameraPosition;
            if (freeCameraMode)
                camera.transform.LookAt(cameraTarget);
        }

        private static void ScaleCameraView(Vector3 center, Single delta)
        {
            Single scaleFactor = 1f;
            if (delta > 0f)
                scaleFactor *= 1f + 0.05f * delta;
            else
                scaleFactor /= 1f - 0.05f * delta;
            if (controlPanel.ActivePanelIndex == controlSetupCameraPanel && cameraSelection < scene.cameraList.Count)
            {
                SetScalingFactor(scalingFactor * scaleFactor);
            }
            else if (controlPanel.ActivePanelIndex == controlSetupOverlayPanel && overlaySelection < scene.overlayList.Count)
            {
                SetOverlayDepth(overlaySelection, scene.overlayList[overlaySelection].curZ + (Single)Math.Round(delta));
            }
            else if (freeCameraMode)
            {
                Camera camera = GetCamera();
                camera.orthographicSize /= scaleFactor;
            }
        }

        private static void RotateCameraView(Vector3 axisOrigin, Vector3 axis, Single angle)
        {
            if (!freeCameraMode)
                return;
            Quaternion q = Quaternion.AngleAxis(angle, axis);
            cameraPosition = axisOrigin + q * (cameraPosition - axisOrigin);
            Camera camera = GetCamera();
            camera.transform.position = cameraPosition;
            camera.transform.LookAt(cameraTarget);
        }

        private static void SetScalingFactor(Single factor)
        {
            scalingFactor = factor; // Using a factor instead of modifying positions directly prevents rounding errors to accumulate
            controlCameraDistance.Value = scalingFactor;
            ChangeCameraSelection(cameraSelection, false);
            shouldRefreshWalkmesh = true;
        }

        private static void MoveOverlayXY(Int32 overlayIndex, Int16 dx, Int16 dy)
        {
            if (overlayIndex >= scene.overlayList.Count)
                return;
            BGOVERLAY_DEF bgOverlay = scene.overlayList[overlayIndex];
            bgOverlay.curX += dx;
            bgOverlay.curY += dy;
            bgOverlay.transform.localPosition = new Vector3(bgOverlay.curX, bgOverlay.curY, bgOverlay.curZ);
        }

        private static void SetOverlayDepth(Int32 overlayIndex, Single depth)
        {
            if (overlayIndex >= scene.overlayList.Count)
                return;
            BGOVERLAY_DEF bgOverlay = scene.overlayList[overlayIndex];
            bgOverlay.curZ = (UInt16)Math.Round(Math.Max(0, depth));
            bgOverlay.transform.localPosition = new Vector3(bgOverlay.transform.localPosition.x, bgOverlay.transform.localPosition.y, bgOverlay.curZ);
            controlOverlayDepth.Value = bgOverlay.curZ;
        }

        private static void ResetOverlayDepth(Int32 overlayIndex)
        {
            if (overlayIndex >= scene.overlayList.Count)
                return;
            BGOVERLAY_DEF bgOverlay = scene.overlayList[overlayIndex];
            SetOverlayDepth(overlayIndex, bgOverlay.orgZ);
        }

        private static void SetWalkpathFlag(Int32 pathIndex, UInt16 flag, Boolean active)
        {
            if (pathIndex >= bgi.floorList.Count)
                return;
            BGI_FLOOR_DEF floor = bgi.floorList[pathIndex];
            if (active)
                foreach (Int32 triId in floor.triNdxList)
                    bgi.triList[triId].triFlags |= flag;
            else
                foreach (Int32 triId in floor.triNdxList)
                    bgi.triList[triId].triFlags &= (UInt16)~flag;
        }

        private static Camera GetCamera()
        {
            if (fieldCamera == null)
                fieldCamera = GameObject.Find("FieldMap Camera").GetComponent<Camera>();
            return fieldCamera;
        }

        private static FileSystemWatcher CreateWatcher()
        {
            // Dummied
            FileSystemWatcher watcher = new FileSystemWatcher(EditorDirectory);
            GameLoopManager.Quit += watcher.Dispose;
            watcher.Filter = ControlFileName;
            watcher.IncludeSubdirectories = false;
            watcher.Changed += OnControlChanged;
            watcher.Created += OnControlChanged;
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        private static void OnControlChanged(Object sender, FileSystemEventArgs e)
        {
            // Dummied
            if (!Monitor.TryEnter(fileControlWatcher))
                return;
            try
            {
                isUpdatingControl = true;
                ReadControlFile();
            }
            finally
            {
                isUpdatingControl = false;
                Monitor.Exit(fileControlWatcher);
            }
        }

        private static void ReadControlFile()
        {
            // Dummied
            Char[] operationSeparator = new Char[] { ':' };
            Char[] argumentSeparator = new Char[] { ',' };
            String[] allLines = File.ReadAllLines(ControlFilePath);
            foreach (String line in allLines)
            {
                String trimmedLine = line.Trim();
                if (String.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#") || trimmedLine.StartsWith("//"))
                    continue;
                String[] split = trimmedLine.Split(operationSeparator, 2);
                String operation = split[0].Trim();
                String[] args = split.Length > 1 ? split[1].Split(argumentSeparator) : new String[0];
                for (Int32 i = 0; i < args.Length; i++)
                    args[i] = args[i].Trim();
                if (operation == "Walkmesh")
                    ControlChangeWalkmesh(EditorDirectory + "/" + (args.Length >= 1 ? args[0] : String.Empty), false);
                else if (operation == "Background")
                    ControlChangeBackground(EditorDirectory + "/" + (args.Length >= 1 ? args[0] : String.Empty), false);
                else if (operation == "DummyModel")
                    ControlChangeDummyModel(args.Length >= 1 ? args[0] : String.Empty);
            }
        }

        private const String ControlPanelHelp =
@"SPACE: show/hide menu
ESCAPE: exit sub-menus
RIGHT MOUSE: move view";

        private const String SetupCameraHelp =
@"Adjust camera by anchoring walkmesh vertices to screen points:
(1) Enable 'Select Anchors'
(2) Select a walkmesh vertex
(3) Select the corresponding background point
(4) Do over for 5 points
(5) Adjust the camera distance";

        private const String SetupOverlayHelp =
@"RIGHT MOUSE: move view
SHIFT + RIGHT MOUSE: move overlay";
    }
}
