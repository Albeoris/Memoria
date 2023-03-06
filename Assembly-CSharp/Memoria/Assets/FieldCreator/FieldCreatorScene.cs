using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Memoria.Prime;
using Memoria.Scenes;
using Memoria.Scripts;
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

		private static Boolean initialized = false;
		private static Boolean isUpdatingControl = false;
		private static FileSystemWatcher fileControlWatcher;
		private static GameObject fieldMapGo;

		private static ControlPanel controlPanel;
		private static Int32 controlTimeoutAutoShow;
		private static Int32 controlInformationPanel;
		private static UILabel controlInformationLabel;
		private static ControlList<String> controlLoadInternal;
		private static ControlList<String> controlLoadCustom;
		private static ControlRoll<String> controlSelectCharacter;
		private static ControlRoll<Int32> controlSelectWalkpath;
		private static ControlRoll<Int32> controlSelectCamera;
		private static ControlToggle controlShowWalkmesh;
		private static ControlToggle controlShowCharacter;
		private static ControlToggle controlCameraModeOption;
		private static ControlToggle controlSelectWalkmeshOption;
		private static ControlToggle controlSelectCameraOption;
		private static ControlInput controlSaveFileName;
		private static ControlHitBox controlSaveOption;
		private static ControlHelp controlHelpOption;

		private static BGI_DEF bgi;
		private static Boolean shouldRefreshWalkmesh;
		private static Boolean walkmeshIsShown = true;
		private static Int32 walkpathSelection = 0;
		private static String walkmeshFilePath = String.Empty;

		private static Camera fieldCamera;
		private static BGSCENE_DEF scene;
		private static String backgroundFileName;
		private static Int32 cameraSelection = 0;
		private static Vector2 cameraPosition = Vector2.zero;
		private static Vector3 cameraTarget = Vector3.zero;

		private static String dummyModelName;
		private static GameObject dummyGo;
		private static Vector3 dummyPosition;

		private static Int32 selectionMode = 0;
		private static Boolean freeCameraMode = false;
		private static Boolean mouseLeftPressed;
		private static Boolean mouseRightPressed;
		private static Vector3 mousePreviousPosition;

		public static void Init()
		{
			bgi = new BGI_DEF();
			scene = new BGSCENE_DEF(true);
			GameObject mapRootGo = GameObject.Find("FieldMap Root");
			fieldMapGo = new GameObject("FieldMap");
			fieldMapGo.transform.parent = mapRootGo.transform;

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

			controlPanel = new ControlPanel(PersistenSingleton<UIManager>.Instance.transform, "Control Panel");
			controlInformationPanel = controlPanel.CreateSubPanel("Information");
			controlInformationLabel = controlPanel.CreateUIElementForPanel<UILabel>(controlPanel.GetPanel(controlInformationPanel));
			controlInformationLabel.alignment = NGUIText.Alignment.Center;
			controlInformationLabel.overflowMethod = UILabel.Overflow.ClampContent;
			controlInformationLabel.bottomAnchor.Set(0f, 50);
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
			if (externalFields.Count > 0)
			{
				controlLoadInternal.ListOpener.Label.rightAnchor.Set(controlPanel.BasePanel.transform, 0.5f, -controlPanel.elementSeparatorWidth / 2);
				controlLoadCustom = controlPanel.AddListOneTimeSelection("Load Custom Field", externalFields, str => str, str => true, str => ControlChangePath(str, false));
			}
			controlPanel.PanelAddRow();
			controlShowWalkmesh = controlPanel.AddToggleOption("Show walkmesh", walkmeshIsShown, b => ShowWalkmesh(b));
			controlShowWalkmesh.Label.rightAnchor.Set(controlPanel.BasePanel.transform, 0.5f, -controlPanel.elementSeparatorWidth / 2);
			controlShowCharacter = controlPanel.AddToggleOption("Show character", true, b => dummyGo?.SetActive(b));
			controlPanel.PanelAddRow();
			controlCameraModeOption = controlPanel.AddToggleOption("Free-camera mode", freeCameraMode, b => SetFreeCameraMode(b));
			controlPanel.PanelAddRow();
			controlSelectCharacter = controlPanel.AddRollOption(dummyModels, str => str, str => ControlChangeDummyModel(str));
			controlSelectCharacter.Loop = false;
			controlPanel.PanelAddRow();
			controlSelectWalkpath = controlPanel.AddRollOption(GetNonEmptyIndexList(bgi.floorList.Count), sel => $"Walkpath {sel}", sel => ChangeWalkpathSelection(sel));
			controlPanel.PanelAddRow();
			controlSelectCamera = controlPanel.AddRollOption(GetNonEmptyIndexList(scene.cameraList.Count), sel => $"Camera {sel}", sel => ChangeCameraSelection(sel));
			controlPanel.PanelAddRow();
			controlSelectWalkmeshOption = controlPanel.AddToggleOption("Select walkpath", selectionMode == 1, b => SetSelectionMode(b ? 1 : 0));
			controlPanel.PanelAddRow();
			controlSelectCameraOption = controlPanel.AddToggleOption("Select field camera", selectionMode == 2, b => SetSelectionMode(b ? 2 : 0));
			controlPanel.PanelAddRow();
			controlSaveFileName = controlPanel.AddInputField("CUSTOM_FIELD_000", UIInput.Validation.Filename);
			controlSaveOption = controlPanel.AddHitBoxOption("Save", ExportField);
			controlPanel.PanelAddRow();
			controlHelpOption = controlPanel.AddHelpSubPanel("Help", ControlPanelHelp);
			controlCameraModeOption.IsEnabled = false; // TODO: add free-camera mode
			controlPanel.EndInitialization();
			controlTimeoutAutoShow = 2;
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
			if (walkmeshIsShown)
			{
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
			Boolean partlyDisableMouse = controlPanel.ActivePanelIndex != 0 && controlPanel.Show;
			Boolean mouseLeftWasPressed = mouseLeftPressed;
			Boolean mouseRightWasPressed = mouseRightPressed;
			mouseLeftPressed = false;
			mouseRightPressed = false;
			if (Input.GetKeyDown(KeyCode.Space) && UICamera.selectedObject?.GetComponent<UIInput>() == null)
				controlPanel.Show = !controlPanel.Show;
			if (Input.GetKeyDown(KeyCode.Escape) && controlPanel.Show)
				controlPanel.SetActivePanel(false);
			if (!partlyDisableMouse && Input.mouseScrollDelta.y != 0f)
			{
				Single scaleFactor = 1f;
				if (Input.mouseScrollDelta.y > 0f)
					scaleFactor *= 1f + 0.05f * Input.mouseScrollDelta.y;
				else
					scaleFactor /= 1f - 0.05f * Input.mouseScrollDelta.y;
				if (selectionMode == 0 || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					ScaleCameraView(cameraTarget, scaleFactor);
				else if (selectionMode == 1)
					ScaleWalkmesh(cameraTarget, scaleFactor);
				else if (selectionMode == 2)
					ScaleCamera(cameraTarget, scaleFactor);
			}
			if (!partlyDisableMouse && Input.GetMouseButton(0))
			{
				if (mouseLeftWasPressed)
				{
					Vector3 mouseDelta = Input.mousePosition - mousePreviousPosition;
					if (Math.Abs(mouseDelta.x) > Math.Abs(mouseDelta.y))
					{
						if (selectionMode == 0 || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
							RotateCameraView(cameraTarget, Vector3.up, -mouseDelta.x);
						else if (selectionMode == 1)
							RotateWalkmesh(cameraTarget, Vector3.up, mouseDelta.x);
						else if (selectionMode == 2)
							RotateCamera(cameraTarget, Vector3.up, -mouseDelta.x);
					}
					else
					{
						// TODO
						//Quaternion angles = currentModel.transform.localRotation;
						//Single angley = angles.eulerAngles[1];
						//Single factorx = -(Single)Math.Cos(Math.PI * angley / 180f);
						//Single factorz = -(Single)Math.Sin(Math.PI * angley / 180f);
						//Quaternion performedRot = Quaternion.Euler(factorx * mouseDelta.y, 0f, factorz * mouseDelta.y);
						//Single horizontalFactor = (angles * performedRot * Vector3.up).y;
						//if (horizontalFactor > 0.5f)
						//	currentModel.transform.localRotation *= performedRot;
					}
				}
				mouseLeftPressed = true;
			}
			if (Input.GetMouseButton(1))
			{
				if (mouseRightWasPressed)
				{
					Vector3 mouseDelta = Input.mousePosition - mousePreviousPosition;
					if (selectionMode == 0 || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
						MoveCameraView(-0.5f * (mouseDelta.y * Vector3.up + mouseDelta.x * Vector3.right));
					else if (selectionMode == 1)
						MoveWalkmesh(-0.5f * mouseDelta.y * Vector3.up);
					else if (selectionMode == 2)
						MoveCamera(-0.5f * (mouseDelta.y * Vector3.up + mouseDelta.x * Vector3.right));
				}
				mouseRightPressed = true;
			}
			UpdateRender();
			mousePreviousPosition = Input.mousePosition;
		}

		public static void UpdateRender()
		{
			GenerateWalkmesh();
			if (walkpathSelection < bgi.floorList.Count)
			{
				BGI_FLOOR_DEF floor = bgi.floorList[walkpathSelection];
				if (floor.triNdxList.Count > 0)
				{
					BGI_TRI_DEF tri = bgi.triList[floor.triNdxList[0]];
					BGI_VEC_DEF v = bgi.vertexList[tri.vertexNdx[0]];
					dummyPosition = bgi.orgPos.ToVector3() + floor.orgPos.ToVector3() + v.ToVector3();
					dummyPosition.y *= -1;
				}
			}
			if (dummyGo != null)
				dummyGo.transform.localPosition = dummyPosition;
		}

		private static void ExportField()
		{
			try
			{
				String fileName = controlSaveFileName.Input.value;
				if (String.IsNullOrEmpty(fileName))
					throw new Exception($"File name must not be empty");
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
			if (!Directory.Exists($"{EditorDirectory}/{CustomFieldsDirectory}"))
				return new List<String>();
			return new List<String>(Directory.GetDirectories($"{EditorDirectory}/{CustomFieldsDirectory}")).ConvertAll(path => Path.GetFileName(path));
		}

		private static List<Int32> GetNonEmptyIndexList(Int32 count)
		{
			if (count <= 0)
				return new List<Int32>() { 0 };
			List<Int32> list = new List<Int32>();
			for (Int32 i = 0; i < count; i++)
				list.Add(i);
			return list;
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

		private static void ControlChangePath(String newPath, Boolean isInternalPath)
		{
			ControlChangeWalkmesh(newPath, isInternalPath);
			ControlChangeBackground(newPath, isInternalPath);
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
				if (File.Exists(objPath))
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
						{
							bgi.ReadData(binaryReader);
						}
					}
				}
			}
			if (initialized)
			{
				controlSelectWalkpath.ChangeList(GetNonEmptyIndexList(bgi.floorList.Count), walkpathSelection);
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
				scene.CreateMemoriaScene(fieldMapGo.transform);
				ChangeCameraSelection(cameraSelection);
			}
			if (initialized)
			{
				controlSelectCamera.ChangeList(GetNonEmptyIndexList(scene.cameraList.Count), cameraSelection);
				controlSelectCamera.IsEnabled = scene.cameraList.Count > 0;
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
						material.shader = ShadersLoader.Find("PSX/FieldMapActor");
						material.SetColor("_Color", new Color32(128, 128, 128, 255));
					}
				}
				// Prevent mesh culling, since in PSX render mode the position depends not only on UnityEngine.Camera but also on BGCAM_DEF.GetMatrixRT
				foreach (MeshFilter renderer in dummyGo.GetComponentsInChildren<MeshFilter>())
					renderer.sharedMesh.bounds = new Bounds(Vector3.zero, Vector3.one * Single.MaxValue * 0.01f);
				foreach (SkinnedMeshRenderer renderer in dummyGo.GetComponentsInChildren<SkinnedMeshRenderer>())
					renderer.localBounds = new Bounds(Vector3.zero, Vector3.one * Single.MaxValue * 0.01f);
			}
		}

		private static void CreateNewControlFile()
		{
			//Stream input = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Ini.Memoria.ini");
			String controlFile =	"Background: Background.bgx\n" +
									"Walkmesh: Walkmesh.obj\n" +
									"DummyModel: GEO_MAIN_F4_ZDN";
			File.WriteAllText(ControlFilePath, controlFile);
		}

		private static void ShowWalkmesh(Boolean show)
		{
			shouldRefreshWalkmesh = true;
			walkmeshIsShown = show;
			controlSelectWalkmeshOption.IsEnabled = show;
			if (!show && selectionMode == 1)
			{
				controlSelectWalkmeshOption.IsToggled = false;
				selectionMode = 0;
			}
		}

		private static void SetFreeCameraMode(Boolean free)
		{
			if (freeCameraMode == free)
				return;
			freeCameraMode = free;
			// TODO
		}

		private static void SetSelectionMode(Int32 selMode)
		{
			if (selectionMode == 1 || selMode == 1)
				shouldRefreshWalkmesh = true;
			selectionMode = selMode;
			controlSelectWalkmeshOption.IsToggled = selectionMode == 1;
			controlSelectCameraOption.IsToggled = selectionMode == 2;
		}

		private static void ChangeWalkpathSelection(Int32 newSelection)
		{
			if (bgi.floorList.Count == 0)
				return;
			while (newSelection < 0)
				newSelection += bgi.floorList.Count;
			while (newSelection >= bgi.floorList.Count)
				newSelection -= bgi.floorList.Count;
			walkpathSelection = newSelection;
			UpdateSelection();
		}

		private static void ChangeCameraSelection(Int32 newSelection)
		{
			if (scene.cameraList.Count == 0)
				return;
			while (newSelection < 0)
				newSelection += scene.cameraList.Count;
			while (newSelection >= scene.cameraList.Count)
				newSelection -= scene.cameraList.Count;
			cameraSelection = newSelection;
			BGCAM_DEF bgCamera = scene.cameraList[newSelection];
			Vector2 centerOffset = bgCamera.GetCenterOffset();
			Single halfFieldWidth = FieldMap.PsxFieldWidthNative / 2f;
			Single halfFieldHeight = FieldMap.PsxFieldHeightNative / 2f;
			Single offsetX = centerOffset.x + bgCamera.w / 2 - halfFieldWidth;
			Single offsetY = -centerOffset.y - bgCamera.h / 2 + halfFieldHeight;
			Shader.SetGlobalFloat("_OffsetX", offsetX);
			Shader.SetGlobalFloat("_OffsetY", offsetY);
			Shader.SetGlobalFloat("_MulX", 1f / halfFieldWidth);
			Shader.SetGlobalFloat("_MulY", 1f / halfFieldHeight);
			Shader.SetGlobalMatrix("_MatrixRT", bgCamera.GetMatrixRT());
			Shader.SetGlobalFloat("_ViewDistance", bgCamera.GetViewDistance());
			Shader.SetGlobalFloat("_DepthOffset", bgCamera.depthOffset);
			cameraPosition = centerOffset;
			GetCamera().transform.position = cameraPosition;
			UpdateSelection();
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
					Color walkpathColor = selectionMode == 1 && j == walkpathSelection ? new Color(0.2f, 0.8f, 0.2f, 0.5f) : new Color(0.5f, 0f, 0.8f, 0.5f);
					floorGo.GetComponent<MeshRenderer>().material.SetColor("_TintColor", walkpathColor);
				}
				bgCamera.projectedWalkMesh.SetActive(i == cameraSelection);
			}
		}

		private static void MoveWalkmesh(Vector3 offset)
		{
			shouldRefreshWalkmesh = true;
			bgi.orgPos = BGI_VEC_DEF.FromVector3(bgi.orgPos.ToVector3() + offset);
		}

		private static void ScaleWalkmesh(Vector3 center, Single factor)
		{
			// TODO
			//shouldRefreshWalkmesh = true;
			//HashSet<Int32> vertexDone = new HashSet<Int32>();
			//Vector3 oldBPos = bgi.orgPos.ToVector3();
			//Vector3 newBPos = BGI_VEC_DEF.FromVector3(center + factor * (oldBPos - center)).ToVector3();
			//for (Int32 i = 0; i < bgi.floorList.Count; i++)
			//{
			//	BGI_FLOOR_DEF floor = bgi.floorList[i];
			//	Vector3 oldFPos = oldBPos + floor.orgPos.ToVector3();
			//	Vector3 newFPos = BGI_VEC_DEF.FromVector3(center + factor * (oldFPos - center)).ToVector3();
			//	for (Int32 j = 0; j < floor.triNdxList.Count; j++)
			//	{
			//		for (Int32 k = 0; k < 3; k++)
			//		{
			//			Int32 vId = bgi.triList[floor.triNdxList[j]].vertexNdx[k];
			//			if (!vertexDone.Contains(vId))
			//			{
			//				vertexDone.Add(vId);
			//				Vector3 oldVPos = oldFPos + bgi.vertexList[vId].ToVector3();
			//				Vector3 newVPos = center + factor * (oldVPos - center);
			//				bgi.vertexList[vId] = BGI_VEC_DEF.FromVector3(newVPos - newFPos - newBPos);
			//			}
			//		}
			//	}
			//	floor.orgPos = BGI_VEC_DEF.FromVector3(newFPos - newBPos);
			//}
			//bgi.orgPos = BGI_VEC_DEF.FromVector3(newBPos);
		}

		private static void RotateWalkmesh(Vector3 axisOrigin, Vector3 axis, Single angle)
		{
			// TODO
			//shouldRefreshWalkmesh = true;
			//Quaternion q = Quaternion.AngleAxis(angle, axis);
			//HashSet<Int32> vertexDone = new HashSet<Int32>();
			//Vector3 oldBPos = bgi.orgPos.ToVector3();
			//Vector3 newBPos = BGI_VEC_DEF.FromVector3(axisOrigin + q * (oldBPos - axisOrigin)).ToVector3();
			//for (Int32 i = 0; i < bgi.floorList.Count; i++)
			//{
			//	BGI_FLOOR_DEF floor = bgi.floorList[i];
			//	Vector3 oldFPos = oldBPos + floor.orgPos.ToVector3();
			//	Vector3 newFPos = BGI_VEC_DEF.FromVector3(axisOrigin + q * (oldFPos - axisOrigin)).ToVector3();
			//	for (Int32 j = 0; j < floor.triNdxList.Count; j++)
			//	{
			//		for (Int32 k = 0; k < 3; k++)
			//		{
			//			Int32 vId = bgi.triList[floor.triNdxList[j]].vertexNdx[k];
			//			if (!vertexDone.Contains(vId))
			//			{
			//				vertexDone.Add(vId);
			//				Vector3 oldVPos = oldFPos + bgi.vertexList[vId].ToVector3();
			//				Vector3 newVPos = axisOrigin + q * (oldVPos - axisOrigin);
			//				bgi.vertexList[vId] = BGI_VEC_DEF.FromVector3(newVPos - newFPos - newBPos);
			//			}
			//		}
			//	}
			//	floor.orgPos = BGI_VEC_DEF.FromVector3(newFPos - newBPos);
			//}
			//bgi.orgPos = BGI_VEC_DEF.FromVector3(newBPos);
		}

		private static void MoveCameraView(Vector2 offset)
		{
			cameraPosition += offset;
			Camera camera = GetCamera();
			camera.transform.position = cameraPosition;
		}

		private static void ScaleCameraView(Vector3 center, Single factor)
		{
			// TODO
		}

		private static void RotateCameraView(Vector3 axisOrigin, Vector3 axis, Single angle)
		{
			// TODO
		}

		private static void MoveCamera(Vector2 offset)
		{
			// TODO
		}

		private static void ScaleCamera(Vector3 center, Single factor)
		{
			//cameraViewDistance *= factor;
			//Shader.SetGlobalFloat("_ViewDistance", cameraViewDistance);
			if (cameraSelection >= scene.cameraList.Count)
				return;
			BGCAM_DEF bgCamera = scene.cameraList[cameraSelection];
			Vector3 newCamPos = center + factor * (bgCamera.GetCamPos() - center);
			bgCamera.t[0] = (Int32)newCamPos[0];
			bgCamera.t[1] = (Int32)newCamPos[1];
			bgCamera.t[2] = (Int32)newCamPos[2];
			Shader.SetGlobalMatrix("_MatrixRT", bgCamera.GetMatrixRT());
			//bgCamera.proj = (UInt16)(bgCamera.proj * factor);
			shouldRefreshWalkmesh = true;
		}

		private static void RotateCamera(Vector3 axisOrigin, Vector3 axis, Single angle)
		{
			// TODO
			if (cameraSelection >= scene.cameraList.Count)
				return;
			//BGCAM_DEF bgCamera = scene.cameraList[cameraSelection];
			//Quaternion q = Quaternion.AngleAxis(angle, axis);
			//Vector3 newCamPos = axisOrigin + q * (bgCamera.GetCamPos() - axisOrigin);
			//bgCamera.t[0] = (Int32)newCamPos[0];
			//bgCamera.t[1] = (Int32)newCamPos[1];
			//bgCamera.t[2] = (Int32)newCamPos[2];
			//Shader.SetGlobalMatrix("_MatrixRT", bgCamera.GetMatrixRT());
			//shouldRefreshWalkmesh = true;
		}

		private static Camera GetCamera()
		{
			if (fieldCamera == null)
				fieldCamera = GameObject.Find("FieldMap Camera").GetComponent<Camera>();
			return fieldCamera;
		}

		private const String ControlPanelHelp =
@"SPACE: show/hide menu
ESCAPE: exit sub-menus

____________________________

Hold SHIFT: viewpoint only

LEFT MOUSE: rotate
SCROLL MOUSE: zoom/resize
RIGHT MOUSE: move";
	}
}
