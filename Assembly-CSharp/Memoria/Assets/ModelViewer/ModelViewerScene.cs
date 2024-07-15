using Assets.Scripts.Common;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Scenes;
using Memoria.Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Memoria.Assets
{
    public static class ModelViewerScene
    {
        public static Boolean initialized = false;
        private static Boolean displayBones = false;
        private static Boolean displayBoneConnections = false;
        private static Boolean displayBoneNames = false;
        private static Boolean displayUI = true;
        private static Boolean toggleAnim = true;
        private static Boolean displayCurrentModel = true;
        private static Boolean orthoView = false;
        private static List<ModelObject> geoList;
        private static List<ModelObject> weapongeoList;
        private static List<KeyValuePair<Int32, String>> animList;
        private static HashSet<Int32> geoArchetype;
        private static Int32 currentGeoIndex;
        private static Int32 currentAnimIndex;
        private static Int32 currentWeaponGeoIndex;
        private static Int32 currentWeaponBoneIndex;
        private static List<Int32> currentBonesID;
        private static String currentAnimName;
        private static GameObject currentModel;
        private static GameObject currentWeaponModel;
        private static CommonSPSSystem spsUtility;
        private static SPSEffect spsEffect;
        private static Vector3 scaleFactor;
        private static Single speedFactor;
        private static String savedAnimationPath;

        private static Int32 inputDelay;
        private static Boolean isLoadingModel;
        private static Boolean isLoadingWeaponModel;
        private static Int32 replaceOnce = 0;
        private static Boolean mouseLeftPressed;
        private static Boolean mouseRightPressed;
        private static Boolean ControlWeapon = false;
        private static Boolean DontSpamMessage = false;
        private static Vector3 mousePreviousPosition;
        private static BoneHierarchyNode currentModelBones;
        private static List<GameObject> boneModels = new List<GameObject>();
        private static List<GameObject> boneConnectModels = new List<GameObject>();
        private static List<Dialog> boneDialogs = new List<Dialog>();
        private static ControlPanel infoPanel;
        private static ControlPanel controlPanel;
        private static ControlPanel extraInfoPanel;
        private static UILabel infoLabel;
        private static UILabel controlLabel;
        private static UILabel extraInfoLabel;
        private static Int32 controlLabelPosX = 0;

        public static void Init()
        {
            Camera camera = GetCamera();
            if (camera == null)
                return;
            isLoadingModel = false;
            isLoadingWeaponModel = false;
            currentWeaponBoneIndex = 0;
            scaleFactor = new Vector3(0.5f, 0.5f, 0.5f);
            geoList = new List<ModelObject>();
            weapongeoList = new List<ModelObject>();
            geoArchetype = new HashSet<Int32>();
            currentBonesID = new List<Int32>();
            speedFactor = 1f;
            savedAnimationPath = null;
            spsUtility = new CommonSPSSystem();
            GameObject spsGo = new GameObject($"ModelViewer_SPS");
            MeshRenderer meshRenderer = spsGo.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = spsGo.AddComponent<MeshFilter>();
            spsEffect = spsGo.AddComponent<SPSEffect>();
            spsEffect.Init(0);
            spsEffect.spsIndex = 0;
            spsEffect.spsTransform = spsGo.transform;
            spsEffect.meshRenderer = meshRenderer;
            spsEffect.meshFilter = meshFilter;
            infoPanel = new ControlPanel(PersistenSingleton<UIManager>.Instance.transform, "");
            infoLabel = infoPanel.AddSimpleLabel("", NGUIText.Alignment.Left, 7);
            infoPanel.EndInitialization(UIWidget.Pivot.BottomRight);
            infoPanel.BasePanel.SetRect(-50f, 0f, 1000f, 580f);
            controlPanel = new ControlPanel(PersistenSingleton<UIManager>.Instance.transform, "");
            controlLabel = controlPanel.AddSimpleLabel("", NGUIText.Alignment.Right, 10);
            controlPanel.EndInitialization(UIWidget.Pivot.BottomRight);
            controlPanel.BasePanel.SetRect(-50f, 0f, 1000f, 580f);
            extraInfoPanel = new ControlPanel(PersistenSingleton<UIManager>.Instance.transform, "");
            extraInfoLabel = extraInfoPanel.AddSimpleLabel("", NGUIText.Alignment.Center, 1);
            extraInfoPanel.EndInitialization(UIWidget.Pivot.BottomRight);
            extraInfoPanel.BasePanel.SetRect(-50f, 0f, 1000f, 580f);
            foreach (UISprite sprite in infoPanel.BasePanel.GetComponentsInChildren<UISprite>(true))
            {
                sprite.spriteName = String.Empty;
                sprite.alpha = 0f;
            }
            foreach (UISprite sprite in controlPanel.BasePanel.GetComponentsInChildren<UISprite>(true))
            {
                sprite.spriteName = String.Empty;
                sprite.alpha = 0f;
            }
            foreach (UISprite sprite in extraInfoPanel.BasePanel.GetComponentsInChildren<UISprite>(true))
            {
                sprite.spriteName = String.Empty;
                sprite.alpha = 0f;
            }
            // Usual models, of type ACC, MAIN, MON, NPC, SUB and WEP
            foreach (KeyValuePair<Int32, String> geo in FF9BattleDB.GEO)
            {
                geoList.Add(new ModelObject() { Id = geo.Key, Name = geo.Value, Kind = MODEL_KIND_NORMAL });
                if (geo.Value.StartsWith("GEO_WEP"))
                    weapongeoList.Add(new ModelObject() { Id = geo.Key, Name = geo.Value, Kind = MODEL_KIND_NORMAL });
            }
            geoArchetype.Add(0);
            String lastArchetype = geoList[0].Name.Substring(0, 8);
            for (Int32 i = 0; i < geoList.Count; i++)
            {
                if (!geoList[i].Name.StartsWith(lastArchetype))
                {
                    geoArchetype.Add(i);
                    lastArchetype = geoList[i].Name.Substring(0, 8);
                }
            }
            geoArchetype.Add(geoList.Count);
            // Battle scenes (and their animated objects)
            List<String> bbgNames = new HashSet<String>(FF9BattleDB.MapModel.Values).ToList();
            bbgNames.Sort();
            foreach (String bbgName in bbgNames)
            {
                if (!Int32.TryParse(bbgName.Substring(5), out Int32 bbgId))
                    continue;
                geoList.Add(new ModelObject() { Id = bbgId, Name = bbgName, Kind = MODEL_KIND_BBG });
                BBGINFO bbgInfo = new BBGINFO();
                bbgInfo.ReadBattleInfo(bbgName);
                for (Int32 i = 0; i < bbgInfo.objanim; i++)
                    geoList.Add(new ModelObject() { Id = bbgId, Name = $"{bbgName}_OBJ{i + 1}", Kind = MODEL_KIND_BBG_OBJ });
            }
            geoArchetype.Add(geoList.Count);
            // SPS effects
            for (Int32 bundleId = 0; bundleId <= 9; bundleId++)
            {
                foreach (AssetManager.AssetFolder modFolder in AssetManager.FolderLowToHigh)
                {
                    if (!modFolder.DictAssetBundleRefs.TryGetValue($"data1{bundleId}", out AssetManager.AssetBundleRef bundle) || bundle.assetBundle == null)
                        continue;
                    List<String> assetsInBundle = new List<String>(bundle.assetBundle.GetAllAssetNames());
                    assetsInBundle.Sort();
                    foreach (String assetName in assetsInBundle)
                    {
                        if (!assetName.StartsWith($"Assets/Resources/FieldMaps/", StringComparison.OrdinalIgnoreCase) || !assetName.EndsWith($".sps.bytes", StringComparison.OrdinalIgnoreCase))
                            continue;
                        String[] path = assetName.Split('/');
                        if (path.Length != 5)
                            continue;
                        String mapName = path[3];
                        if (!Int32.TryParse(path[4].Remove(path[4].Length - 10), out Int32 spsId))
                            continue;
                        geoList.Add(new ModelObject() { Id = spsId, Name = mapName, Kind = MODEL_KIND_SPS });
                    }
                }
                geoArchetype.Add(geoList.Count);
            }
            for (Int32 spsNo = 0; spsNo < SPSConst.WORLD_DEFAULT_OBJLOAD; spsNo++)
                geoList.Add(new ModelObject() { Id = spsNo, Name = "WorldMap", Kind = MODEL_KIND_SPS });
            geoArchetype.Add(geoList.Count);
            foreach (SPSPrototype sps in CommonSPSSystem.SPSPrototypes.Values)
                geoList.Add(new ModelObject() { Id = sps.Id, Name = "FromPrototype", Kind = MODEL_KIND_SPS });
            geoArchetype.Add(geoList.Count);
            ChangeModel(0);
            SceneDirector.ClearFadeColor();
            camera.transform.position = new Vector3(0f, 0f, -1000f);
            camera.transform.LookAt(Vector3.zero, Vector3.down);
            FPSManager.SetTargetFPS(Configuration.Graphics.MenuFPS);
            FPSManager.SetMainLoopSpeed(Configuration.Graphics.MenuTPS);
            initialized = true;
        }

        public static void Update()
        {
            try
            {
                if (isLoadingModel || isLoadingWeaponModel)
                    return;
                if (replaceOnce > 0 && currentModel != null)
                {
                    currentModel.transform.localScale = scaleFactor;
                    currentModel.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
                    replaceOnce--;
                }
                Boolean mouseLeftWasPressed = mouseLeftPressed;
                Boolean mouseRightWasPressed = mouseRightPressed;
                Boolean downUpProcessed = false;
                mouseLeftPressed = false;
                mouseRightPressed = false;
                Boolean ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                Boolean shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                Boolean altgr = Input.GetKey(KeyCode.AltGr);

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Int32 nextIndex = currentGeoIndex + 1;
                    if (shift)
                        while (!geoArchetype.Contains(nextIndex) && nextIndex != geoList.Count)
                            nextIndex++;
                    else if (ctrl)
                        nextIndex = currentGeoIndex + 10;
                    if (nextIndex == geoList.Count)
                        nextIndex -= geoList.Count;
                    if (geoList[nextIndex].Id == 276 || geoList[nextIndex].Id == 393 || geoList[nextIndex].Id == 394) // models with no texture bugged
                        ChangeModel(nextIndex + 1);
                    else
                        ChangeModel(nextIndex);
                    while (currentModel == null)
                        ChangeModel(++nextIndex);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Int32 prevIndex = currentGeoIndex - 1;
                    if (prevIndex < 0)
                        prevIndex += geoList.Count;
                    if (shift)
                        while (!geoArchetype.Contains(prevIndex))
                            prevIndex--;
                    else if (ctrl)
                        prevIndex = currentGeoIndex - 10;
                    if (geoList[prevIndex].Id == 276 || geoList[prevIndex].Id == 393 || geoList[prevIndex].Id == 394) // models with no texture bugged
                        ChangeModel(prevIndex - 1);
                    else
                        ChangeModel(prevIndex);
                    while (currentModel == null)
                        ChangeModel(--prevIndex);
                }
                if (shift && Input.GetKeyDown(KeyCode.B))
                {
                    displayBoneConnections = !displayBoneConnections;
                }
                else if (Input.GetKeyDown(KeyCode.B))
                {
                    displayBones = !displayBones;
                    displayBoneNames = !displayBoneNames;
                }
                if (Input.GetKeyDown(KeyCode.I))
                {
                    displayUI = !displayUI;
                }
                if (Input.GetKeyDown(KeyCode.O))
                {
                    orthoView = !orthoView;
                }
                if (Input.GetKeyDown(KeyCode.H)) // TODO - Replace it by changing the color instead, to hide the model
                {
                    displayCurrentModel = !displayCurrentModel;
                    Renderer[] componentsInChildren = currentModel.GetComponentsInChildren<Renderer>();
                    for (Int32 i = 0; i < componentsInChildren.Length; i++)
                    {
                        Renderer renderer = componentsInChildren[i];
                        renderer.enabled = displayCurrentModel;
                    }
                }
                if (currentModel == null)
                    return;
                if (geoList[currentGeoIndex].Kind == MODEL_KIND_SPS) // SPS SPECIFIC
                {
                    if (shift && Input.GetKey(KeyCode.UpArrow)) // Fade ++
                    {
                        spsEffect.fade++;
                        if (spsEffect.fade > 255)
                            spsEffect.fade = 255;
                        downUpProcessed = true;
                    }
                    else if (shift && Input.GetKey(KeyCode.DownArrow)) // Fade --
                    {
                        spsEffect.fade--;
                        if (spsEffect.fade < 0)
                            spsEffect.fade = 0;
                        downUpProcessed = true;
                    }
                    if (Input.GetKey(KeyCode.Space)) // Next frame
                    {
                        ChangeAnimation(currentAnimIndex + 1);
                    }
                    if (Input.GetKeyDown(KeyCode.S)) // Next shader
                    {
                        spsEffect.abr++;
                        if (spsEffect.abr >= spsEffect.materials.Length)
                            spsEffect.abr = 0;
                    }
                }
                else if (geoList[currentGeoIndex].Kind < MODEL_KIND_SPS && animList.Count > 0) // Anim specific
                {
                    if (Input.GetKeyDown(KeyCode.Space)) // play/pause anim
                    {
                        toggleAnim = !toggleAnim;
                    }
                    if (Input.GetKeyDown(KeyCode.S)) // change anim speed
                    {
                        if (speedFactor == 1f)
                            speedFactor = 0.5f;
                        else if (speedFactor == 0.5f)
                            speedFactor = 0.1f;
                        else if (speedFactor == 0.1f)
                            speedFactor = 1f;
                    }
                }
                if (!downUpProcessed) // Browse anims
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                        ChangeAnimation(currentAnimIndex + (ctrl ? 5 : 1));
                    else if (Input.GetKeyDown(KeyCode.DownArrow))
                        ChangeAnimation(currentAnimIndex - (ctrl ? 5 : 1));
                }
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    Animation anim = currentModel.GetComponent<Animation>();
                    if (anim != null && !String.IsNullOrEmpty(savedAnimationPath))
                    {
                        AnimationClip clip = AnimationClipReader.ReadAnimationClipFromDisc(savedAnimationPath);
                        if (clip != null)
                        {
                            currentAnimName = "CUSTOM_CLIP";
                            anim.RemoveClip("CUSTOM_CLIP");
                            anim.AddClip(clip, "CUSTOM_CLIP");
                            anim.Play("CUSTOM_CLIP");
                            anim["CUSTOM_CLIP"].speed = speedFactor;
                        }
                        else
                            FF9Sfx.FF9SFX_Play(102);
                    }
                    else
                        FF9Sfx.FF9SFX_Play(102);
                }
                if (Input.GetKey(KeyCode.V))
                {
                    infoPanel.BasePanel.transform.localPosition = infoPanel.BasePanel.transform.localPosition + new Vector3(shift ? -5 : 5, 0, 0);
                }
                if (Input.GetKey(KeyCode.N))
                {
                    controlLabelPosX += shift ? 5 : -5;
                }
                GameObject targetModel = ControlWeapon ? currentWeaponModel : currentModel;
                if (ctrl)
                {
                    if (Input.GetKey(KeyCode.Keypad6))
                        targetModel.transform.localRotation *= Quaternion.Euler(0f, 1f, 0f);
                    if (Input.GetKey(KeyCode.Keypad4))
                        targetModel.transform.localRotation *= Quaternion.Euler(0f, -1f, 0f);
                    if (Input.GetKey(KeyCode.Keypad8))
                        targetModel.transform.localRotation *= Quaternion.Euler(-1f, 0f, 0f);
                    if (Input.GetKey(KeyCode.Keypad2))
                        targetModel.transform.localRotation *= Quaternion.Euler(1f, 0f, 0f);
                    if (Input.GetKey(KeyCode.Keypad7) || Input.GetKey(KeyCode.Keypad9))
                        targetModel.transform.localRotation *= Quaternion.Euler(0f, 0f, 1f);
                    if (Input.GetKey(KeyCode.Keypad1) || Input.GetKey(KeyCode.Keypad3))
                        targetModel.transform.localRotation *= Quaternion.Euler(0f, 0f, -1f);
                }
                else
                {
                    Single moveSpeed = 0.5f;
                    if (Input.GetKey(KeyCode.Keypad6))
                        targetModel.transform.localPosition += moveSpeed * Vector3.left;
                    if (Input.GetKey(KeyCode.Keypad4))
                        targetModel.transform.localPosition += moveSpeed * Vector3.right;
                    if (Input.GetKey(KeyCode.Keypad8))
                        targetModel.transform.localPosition += moveSpeed * Vector3.down;
                    if (Input.GetKey(KeyCode.Keypad2))
                        targetModel.transform.localPosition += moveSpeed * Vector3.up;
                    if (Input.GetKey(KeyCode.Keypad7) || Input.GetKey(KeyCode.Keypad9))
                        targetModel.transform.localPosition += moveSpeed * Vector3.back;
                    if (Input.GetKey(KeyCode.Keypad1) || Input.GetKey(KeyCode.Keypad3))
                        targetModel.transform.localPosition += moveSpeed * Vector3.forward;
                }
                if (Input.GetKey(KeyCode.Keypad5) && !DontSpamMessage)
                {
                    string ModelTargetPos = currentModel.transform.localPosition.ToString("F9");
                    string ModelTargetRot = currentModel.transform.localRotation.eulerAngles.ToString("F9");
                    string WeaponTargetPos = currentWeaponModel.transform.localPosition.ToString("F9");
                    string WeaponTargetRot = currentWeaponModel.transform.localRotation.eulerAngles.ToString("F9");
                    Log.Message("[MODEL] " + geoList[currentGeoIndex].Name + ".offset = " + ModelTargetPos.Remove(ModelTargetPos.Length - 1) + ", " + ModelTargetRot.Remove(0, 1));
                    Log.Message("[WEAPON] " + weapongeoList[currentWeaponGeoIndex].Name + ".offset = " + WeaponTargetPos.Remove(WeaponTargetPos.Length - 1) + ", " + WeaponTargetRot.Remove(0, 1));
                    DontSpamMessage = true;
                }
                if (Input.GetKeyUp(KeyCode.Keypad5))
                    DontSpamMessage = false;
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ChangeModel(GetFirstModelOfCategory(0));
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ChangeModel(GetFirstModelOfCategory(1));
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    ChangeModel(GetFirstModelOfCategory(2));
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    ChangeModel(GetFirstModelOfCategory(3));
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    ChangeModel(GetFirstModelOfCategory(4));
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    ChangeModel(GetFirstModelOfCategory(5));
                }
                else if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    ChangeModel(GetFirstModelOfCategory(6));
                }
                else if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    ChangeModel(GetFirstModelOfCategory(7));
                }
                if (Input.GetKeyDown(KeyCode.P) && currentBonesID.Count > 0)
                {
                    if (shift)
                    {
                        if (ControlWeapon)
                        {
                            ControlWeapon = false;
                        }
                        else
                        {
                            ControlWeapon = true;
                        }
                    }
                    else
                    {
                        ChangeWeaponModel(currentWeaponGeoIndex);
                    }
                }
                if (Input.mouseScrollDelta.y != 0f) // Scroll wheel on mouse (zoom in/out)
                {
                    if (currentWeaponModel && (shift || ctrl))
                    {
                        if (shift)
                        {
                            currentWeaponBoneIndex += Input.mouseScrollDelta.y < 0f ? -1 : 1;
                            if (currentWeaponBoneIndex < 0)
                                currentWeaponBoneIndex = currentBonesID.Count - 1;
                            else if (currentWeaponBoneIndex > currentBonesID.Count)
                                currentWeaponBoneIndex = 0;
                            if (currentWeaponModel != null && currentModel != null)
                                WeaponAttach(currentWeaponModel, currentModel, currentBonesID[currentWeaponBoneIndex]);
                        }
                        else if (ctrl)
                        {
                            Int32 nextIndex = currentWeaponGeoIndex;
                            nextIndex += Input.mouseScrollDelta.y < 0f ? -1 : 1;
                            if (nextIndex < 0)
                                nextIndex = (weapongeoList.Count - 1);
                            else if (nextIndex > weapongeoList.Count)
                                nextIndex = 0;
                            ChangeWeaponModel(nextIndex);
                        }
                    }
                    else
                    {
                        Single scrollSpeed = 0.1f; // was 0.05 before
                        if (Input.mouseScrollDelta.y > 0f)
                            scaleFactor *= 1f + scrollSpeed * Input.mouseScrollDelta.y;
                        else
                            scaleFactor /= 1f - scrollSpeed * Input.mouseScrollDelta.y;
                        currentModel.transform.localScale = scaleFactor;
                    }
                }
                if (Input.GetMouseButton(0) && geoList[currentGeoIndex].Kind < MODEL_KIND_SPS) // Left Click
                {
                    if (mouseLeftWasPressed)
                    {
                        Vector3 mouseDelta = Input.mousePosition - mousePreviousPosition;
                        if (Math.Abs(mouseDelta.x) >= Math.Abs(mouseDelta.y))
                        {
                            targetModel.transform.localRotation *= Quaternion.Euler(0f, mouseDelta.x, 0f);
                        }
                        else
                        {
                            Quaternion angles = targetModel.transform.localRotation;
                            Single angley = angles.eulerAngles[1];
                            Single factorx = -(Single)Math.Cos(Math.PI * angley / 180f);
                            Single factorz = -(Single)Math.Sin(Math.PI * angley / 180f);
                            Quaternion performedRot = Quaternion.Euler(factorx * mouseDelta.y, 0f, factorz * mouseDelta.y);
                            Single horizontalFactor = (angles * performedRot * Vector3.up).y;
                            if (horizontalFactor > 0.5f)
                                targetModel.transform.localRotation *= performedRot;
                        }
                    }
                    mouseLeftPressed = true;
                }
                if (Input.GetMouseButton(1)) // Right Click
                {
                    if (mouseRightWasPressed)
                    {
                        Vector3 mouseDelta = Input.mousePosition - mousePreviousPosition;
                        Single mouseSensibility = 1f; // was 0.5f
                        targetModel.transform.localPosition -= mouseSensibility * new Vector3(mouseDelta.x, mouseDelta.y, mouseDelta.z);
                        if (geoList[currentGeoIndex].Kind == MODEL_KIND_SPS)
                            spsEffect.pos = currentModel.transform.localPosition;
                    }
                    mouseRightPressed = true;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (animList.Count > 0)
                    {
                        List<String> exportedAnims;
                        if (shift)
                            exportedAnims = animList.Select(a => a.Value).ToList();
                        else
                            exportedAnims = new List<String>() { animList[currentAnimIndex].Value };
                        ExportAnimation(exportedAnims);
                    }
                    else if (geoList[currentGeoIndex].Kind == MODEL_KIND_SPS && currentModel != null)
                    {
                        String directoryName = "StreamingAssets/EffectSPS/";
                        String fileName;
                        if (String.Equals(geoList[currentGeoIndex].Name, "WorldMap"))
                        {
                            directoryName += "WorldSPS";
                            fileName = $"fx{geoList[currentGeoIndex].Id:D2}.png";
                        }
                        else if (String.Equals(geoList[currentGeoIndex].Name, "FromPrototype"))
                        {
                            directoryName += "Status";
                            fileName = Path.GetFileName(CommonSPSSystem.SPSPrototypes[geoList[currentGeoIndex].Id].TexturePath);
                        }
                        else
                        {
                            directoryName += geoList[currentGeoIndex].Name;
                            fileName = $"{geoList[currentGeoIndex].Id}.png";
                        }
                        Texture2D spstexture = spsEffect.pngTexture;
                        if (spstexture == null || (String.Equals(geoList[currentGeoIndex].Name, "FromPrototype") && spstexture.width == 256 && spstexture.height == 256))
                        {
                            Rect area = spsEffect.GetRelevantPartOfTCB();
                            spstexture = TextureHelper.GetFragment(TextureHelper.CopyAsReadable(spstexture), (Int32)area.x, (Int32)area.y, (Int32)area.width, (Int32)area.height);
                        }
                        if (spstexture != null)
                        {
                            Directory.CreateDirectory(directoryName);
                            File.WriteAllBytes($"{directoryName}/{fileName}", spstexture.EncodeToPNG());
                            Log.Message($"[ModelViewerScene] Export SPS texture: {directoryName}/{fileName}");
                            FF9Sfx.FF9SFX_Play(1261);
                        }
                        else
                        {
                            Log.Message($"[ModelViewerScene] Failed to export SPS texture: {directoryName}/{fileName}");
                            FF9Sfx.FF9SFX_Play(102);
                        }
                    }
                    else
                    {
                        FF9Sfx.FF9SFX_Play(102);
                    }
                }
                Animation animation = currentModel.GetComponent<Animation>();
                if (Input.GetKeyDown(KeyCode.C))
                {
                    Camera camera = GetCamera();
                    if (camera.backgroundColor == Color.grey) camera.backgroundColor = Color.black;
                    else if (camera.backgroundColor == Color.black) camera.backgroundColor = Color.green;
                    else camera.backgroundColor = Color.grey;
                }
                // make animation a loop by default
                if (animation != null && !animation.IsPlaying(currentAnimName) && toggleAnim)
                {
                    animation.Play(currentAnimName);
                    if (animation[currentAnimName] != null)
                        animation[currentAnimName].speed = speedFactor;
                }
                else if (animation != null && animation.IsPlaying(currentAnimName) && (!toggleAnim || Input.GetKeyDown(KeyCode.S)))
                {
                    animation.Stop();
                }

                UpdateRender();
                mousePreviousPosition = Input.mousePosition;
            }
            catch (Exception err)
            {
                isLoadingModel = false;
                Log.Error(err);
            }
        }

        public static void UpdateRender()
        {
            Int32 boneCount = 0;
            Int32 boneConnectionCount = 0;
            Int32 boneDialogCount = 0;
            Camera camera = GetCamera();
            if (currentModel != null && geoList[currentGeoIndex].Kind == MODEL_KIND_SPS)
            {
                spsEffect.GenerateSPS();
                spsEffect.lastFrame = spsEffect.curFrame;
                Matrix4x4 cameraMatrix = camera.worldToCameraMatrix.inverse;
                Vector3 directionForward = cameraMatrix.MultiplyVector(Vector3.forward);
                Vector3 directionRight = cameraMatrix.MultiplyVector(Vector3.right);
                Vector3 directionDown = Vector3.Cross(directionForward, directionRight);
                currentModel.transform.localScale = scaleFactor;
                currentModel.transform.LookAt(currentModel.transform.position + directionForward, -directionDown);
            }
            if (currentModel != null && currentModelBones != null && !isLoadingModel)
            {
                foreach (BoneHierarchyNode bone in currentModelBones)
                {
                    if (!currentBonesID.Contains(bone.Id))
                        currentBonesID.Add(bone.Id);
                    if (displayBoneNames)
                    {
                        while (boneDialogCount >= boneDialogs.Count)
                        {
                            boneDialogs.Add(Singleton<DialogManager>.Instance.AttachDialog($"[IMME][NFOC][b]{bone.Id}[/b][ENDN]", 10, 1, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStyleTransparent, bone.Position, Dialog.CaptionType.None));
                            Vector3 BonePos = -bone.Position;
                            string ID = $"[IMME][NFOC][b]{bone.Id}[/b]";
                            for (Int32 i = 0; i < (boneDialogs.Count - 1); i++)
                            {
                                if ((BonePos - boneDialogs[i].transform.localPosition).sqrMagnitude < 1 && boneDialogs[i].Phrase.Length > 8)
                                {
                                    String IDBone = boneDialogs[i].Phrase.Remove(0, 14);
                                    ID += $",{IDBone}[ENDN]";
                                    boneDialogs[i].Phrase = "";
                                    boneDialogs[boneDialogCount].Phrase = "";
                                    boneDialogs[i] = Singleton<DialogManager>.Instance.AttachDialog(ID, ID.Length, 1, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStyleTransparent, bone.Position, Dialog.CaptionType.None);
                                    boneDialogs[i].transform.localPosition = BonePos;
                                    break;
                                }
                            }
                        }
                        boneDialogs[boneDialogCount].transform.localPosition = -bone.Position;
                        boneDialogs[boneDialogCount].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //scaleFactor;
                        boneDialogCount++;
                    }
                    if (displayBones)
                    {
                        while (boneCount >= boneModels.Count)
                            boneModels.Add(CreateModelForBone());
                        boneModels[boneCount].transform.position = bone.Position;
                        boneModels[boneCount].transform.localScale = scaleFactor;
                        boneCount++;
                    }
                    if (displayBoneConnections && bone.Parent != null)
                    {
                        while (boneConnectionCount >= boneConnectModels.Count)
                            boneConnectModels.Add(CreateModelForBoneConnection());
                        //Log.Message("Test " + boneConnectModels.Count);
                        MoveBoneConnection(boneConnectModels[boneConnectionCount], bone.Parent.Position, bone.Position);
                        boneConnectionCount++;
                    }
                }
            }
            for (Int32 i = boneCount; i < boneModels.Count; i++)
                UnityEngine.Object.Destroy(boneModels[i]);
            for (Int32 i = boneConnectionCount; i < boneConnectModels.Count; i++)
                UnityEngine.Object.Destroy(boneConnectModels[i]);
            for (Int32 i = boneDialogCount; i < boneDialogs.Count; i++)
                boneDialogs[i].ForceClose();
            if (boneCount < boneModels.Count)
                boneModels.RemoveRange(boneCount, boneModels.Count - boneCount);
            if (boneConnectionCount < boneConnectModels.Count)
                boneConnectModels.RemoveRange(boneConnectionCount, boneConnectModels.Count - boneConnectionCount);
            if (boneDialogCount < boneDialogs.Count)
                boneDialogs.RemoveRange(boneDialogCount, boneDialogs.Count - boneDialogCount);
            if (displayUI)
            {
                String label = $"[FFFF00][⇧↔/1-8][E5E5FF] {GetCategoryEnumeration(currentGeoIndex, true)}[FFFFFF]"; //[222222]{GetCategoryEnumeration(currentGeoIndex, true, -1)} [FFFFFF]{GetCategoryEnumeration(currentGeoIndex, true)} [222222]{GetCategoryEnumeration(currentGeoIndex, true, 1)}[FFFFFF]
                label += "\n";
                label += $"[FFFF00][↔][FFFFFF] Model {GetCategoryEnumeration(currentGeoIndex)}: {geoList[currentGeoIndex].Name} ({geoList[currentGeoIndex].Id})";
                label += "\n";
                if (geoList[currentGeoIndex].Kind == MODEL_KIND_SPS)
                {
                    label += $"[FFFF00][␣][FFFFFF] {currentAnimName}";
                    label += "\n";
                    label += $"[FFFF00][S][FFFFFF] Shader: {spsEffect.materials[Math.Min((Int32)spsEffect.abr, 4)].shader.name} [FFFF00][^↓↑][FFFFFF] Fade: {spsEffect.fade}";
                    label += "\n";
                }
                else if (animList.Count > 0)
                {
                    label += $"[FFFF00][↕][FFFFFF] Anim {currentAnimIndex + 1}/{animList.Count} [FFFF00][S][FFFFFF] Speed: {speedFactor} [FFFF00][␣][FFFFFF] {((toggleAnim) ? "[00FF00]▶" : "[FF0000]ıı")}";
                    label += "\n";
                    label += $"[CCCCCC]  - Anim name: {currentAnimName} ({animList[currentAnimIndex].Key})[FFFFFF]";
                    label += "\n";
                }
                else
                {
                    label += "\n\n";
                }
                if (currentWeaponModel)
                {
                    label += $"[FFFF00][^Scroll][FFFFFF] Weapon: {weapongeoList[currentWeaponGeoIndex].Name}\n";
                    label += $"[FFFF00][⇧Scroll][FFFFFF] Bone: {currentWeaponBoneIndex}\n";
                    if (!ControlWeapon)
                        label += $"[FFFF00][⇧P][FFFFFF] Selected: Model\n";
                    else
                        label += $"[FFFF00][⇧P][FFFFFF] Selected: [00FF00]Weapon\n";
                }
                else
                    label += "\n\n\n";
                if (!String.Equals(infoLabel.text, label))
                    infoLabel.text = label;
                if (!infoPanel.Show)
                    infoPanel.Show = true;
                infoLabel.fontSize = 25;

                String controlist = "Hide UI [FFFF00][I][FFFFFF]\r\n";
                foreach (KeyValuePair<String, String> entry in ControlsKeys)
                    controlist += $"{entry.Value} [FFFF00][{entry.Key}][FFFFFF]\r\n";
                controlLabel.text = controlist;

                GameObject targetModel = ControlWeapon ? currentWeaponModel : currentModel;
                String extraInfo = "";
                if (targetModel != null)
                {
                    Transform transform = targetModel.transform;
                    extraInfo += $"Pos: [x]{transform.localPosition.x} [y]{transform.localPosition.y}";
                    extraInfo += $" | Rot(Quat): [x]{Math.Round(transform.localRotation.x, 2)} [y]{Math.Round(transform.localRotation.y, 2)} [z]{Math.Round(transform.localRotation.z, 2)} [w]{Math.Round(transform.localRotation.w, 2)}";
                    extraInfo += $" | Rot(Eul): {Math.Round(transform.localRotation.eulerAngles.x, 0)}/{Math.Round(transform.localRotation.eulerAngles.y, 0)}/{Math.Round(transform.localRotation.eulerAngles.z, 0)}";
                }
                extraInfoLabel.text = extraInfo;
                extraInfoLabel.fontSize = 16;
                extraInfoLabel.effectDistance = new Vector2(2f,2f);
                extraInfoLabel.alignment = NGUIText.Alignment.Center;
                extraInfoPanel.BasePanel.transform.localPosition = new Vector3(500, 0, 0);
                extraInfoPanel.Show = true;
            }
            else
            {
                infoPanel.Show = false;
                extraInfoPanel.Show = false;

                String controlist = "Show UI [FFFF00][I][FFFFFF]\r\n";
                foreach (KeyValuePair<String, String> entry in ControlsKeys)
                    controlist += $"\r\n";
                controlLabel.text = controlist;
            }
            controlPanel.BasePanel.transform.localPosition = new Vector3(1000 + controlLabelPosX, 0, 0);
            controlPanel.Show = true;
            controlLabel.fontSize = 25;
            //Log.Message("boneConnectModels.Count " + boneConnectModels.Count);
        }

        /// <summary>Returns "model#/maxmodel#" from a category, or the category name if "categoryName" is true </summary>
        private static String GetCategoryEnumeration(Int32 modelNum, Boolean categoryName = false, Int32 offset = 0)
        {
            List<int> categoriesThresholds = new List<int>(geoArchetype);
            categoriesThresholds.Add(geoList.Count);
            categoriesThresholds.Sort();
            int categoryNum = -1;
            foreach (int threshold in categoriesThresholds)
            {
                if (!(threshold > modelNum))
                    categoryNum++;
                
            }
            if (offset != 0)
            {
                categoryNum += offset;
                if (categoryNum < 0) categoryNum += categoriesThresholds.Count -1;
                if (categoryNum >= categoriesThresholds.Count -1) categoryNum -= categoriesThresholds.Count -1;
            }
            if (categoryName)
            {
                if (!(categoryNum >= categoryNames.Count))
                    return categoryNames[categoryNum];
                else
                    return $"{categoryNum}";
            }
            else
                return $"{modelNum + 1 - categoriesThresholds[categoryNum]}/{categoriesThresholds[categoryNum + 1] - categoriesThresholds[categoryNum]}";
        }

        private static Int32 GetFirstModelOfCategory(Int32 categoryNum)
        {
            List<int> categoriesThresholds = new List<int>(geoArchetype);
            categoriesThresholds.Sort();
            categoryNum = Mathf.Clamp(categoryNum, 0, categoriesThresholds.Count -1);
            return categoriesThresholds[categoryNum];
        }

        private static List<string> categoryNames = new List<string>
        {
            "FIELD ITEMS",
            "ACTORS (MAIN)",
            "MONSTERS",
            "NPC",
            "ACTORS/WM",
            "WEAPONS",
            "BATTLE MAPS",
            "SPS (11)",
            "SPS (12)",
            "SPS (13)",
            "SPS (14)",
            "SPS (15)",
            "SPS (16)",
            "SPS (17)",
            "SPS (18)",
            "SPS (19)",
            "SPS (WM)",
            "SPS (PROTO)",
        };

        private static readonly Dictionary<String, String> ControlsKeys = new Dictionary<String, String>
        {
            {"B", "Show bones"},
            {"⇧B", "Bone lines"},
            {"P", "Attach weapon"},
            {"O", "Ortho view"},
            {"C", "BG color"},
            {"◐", "Angle"},
            {"◑", "Position"},
            {"Scroll", "Zoom"},
            {"^✥", "Fast browse"},
            {"E", "Export anim"},
            {"L", "Read last exp."},
        };

        private static Camera GetCamera()
        {
            Camera camera = GameObject.Find("FieldMap Camera")?.GetComponent<Camera>();
            if (camera != null)
            {
                if (displayBones || orthoView)
                {
                    camera.orthographic = true;
                    camera.orthographicSize = 350f;
                }
                else
                {
                    camera.orthographic = false;
                    camera.fieldOfView = 40f;
                    camera.nearClipPlane = 0.1f;
                    camera.farClipPlane = 10000f;
                }
            }
            return camera;
        }

        private static List<KeyValuePair<Int32, String>> GetAnimationsOfModel(ModelObject model)
        {
            List<KeyValuePair<Int32, String>> result = new List<KeyValuePair<Int32, String>>();
            if (model.Kind == MODEL_KIND_NORMAL)
            {
                String identifier = model.Name.Substring(4);
                foreach (KeyValuePair<Int32, String> anim in FF9DBAll.AnimationDB)
                    if (anim.Value.Substring(4).StartsWith(identifier))
                        result.Add(new KeyValuePair<Int32, String>(anim.Key, anim.Value));
                Log.Message($"[ModelViewerScene] Animation set: {String.Join(", ", result.Select(a => a.Value).ToArray())}");
            }
            return result;
        }

        private static void ChangeModel(Int32 index)
        {
            currentAnimIndex = 0;
            isLoadingModel = true;
            if (currentModel != null && geoList[currentGeoIndex].Kind != MODEL_KIND_SPS)
                UnityEngine.Object.Destroy(currentModel);
            if (currentWeaponModel != null)
            {
                ControlWeapon = false;
                UnityEngine.Object.Destroy(currentWeaponModel);
            }
            else if (geoList[currentGeoIndex].Kind == MODEL_KIND_SPS)
                spsEffect.Unload();
            while (index < 0)
                index += geoList.Count;
            while (index >= geoList.Count)
                index -= geoList.Count;
            currentGeoIndex = index;
            animList = GetAnimationsOfModel(geoList[index]);
            Log.Message($"[ModelViewerScene] Change model: {geoList[index].Name}");
            if (geoList[index].Kind != MODEL_KIND_SPS)
            {
                UpdateRender(); // Force refresh bones between different models
                if (geoList[index].Kind == MODEL_KIND_NORMAL)
                    currentModel = ModelFactory.CreateModel(geoList[index].Name);
                else if (geoList[index].Kind == MODEL_KIND_BBG)
                    currentModel = ModelFactory.CreateModel($"BattleMap/BattleModel/battleMap_all/{geoList[index].Name}/{geoList[index].Name}", true);
                else if (geoList[index].Kind == MODEL_KIND_BBG_OBJ)
                    currentModel = ModelFactory.CreateModel($"BattleMap/BattleModel/battleMap_all/{geoList[index].Name}/{geoList[index].Name}");
                else
                    currentModel = null;
                if (currentModel != null && (geoList[index].Kind == MODEL_KIND_BBG || geoList[index].Kind == MODEL_KIND_BBG_OBJ))
                {
                    battlebg.SetDefaultShader(currentModel);
                    if (String.Equals(geoList[index].Name, "BBG_B171_OBJ2")) // Crystal World, Crystal
                        battlebg.SetMaterialShader(currentModel, "PSX/BattleMap_Cystal");
                }
            }
            else
            {
                if (spsUtility.SetupSPSBinary(spsEffect, new KeyValuePair<String, Int32>(geoList[index].Name, geoList[index].Id), true))
                {
                    currentModel = spsEffect.gameObject;
                    currentModel.transform.localPosition = Vector3.zero;
                    spsEffect.curFrame = 0;
                    spsEffect.fade = 128;
                    spsEffect.meshRenderer.enabled = true;
                }
                else
                {
                    currentModel = null;
                }
            }
            Single scaleAbs = scaleFactor.z;
            scaleFactor.x = geoList[index].Kind == MODEL_KIND_NORMAL ? scaleAbs : -scaleAbs;
            scaleFactor.y = geoList[index].Kind == MODEL_KIND_NORMAL ? scaleAbs : -scaleAbs;
            currentModelBones = null;
            currentBonesID.Clear();
            currentWeaponBoneIndex = 0;
            if (currentModel == null)
            {
                currentAnimIndex = 0;
                currentAnimName = "";
                currentModelBones = null;
                isLoadingModel = false;
                return;
            }
            if (geoList[index].Kind == MODEL_KIND_NORMAL)
            {
                if (ModelFactory.garnetShortHairTable.Contains(geoList[index].Name))
                {
                    Boolean garnetShortHair = geoList[index].Name == "GEO_MAIN_F1_GRN"
                                           || geoList[index].Name == "GEO_MAIN_B0_004"
                                           || geoList[index].Name == "GEO_MAIN_B0_005"
                                           || geoList[index].Name == "GEO_MON_B3_169"
                                           || geoList[index].Name == "GEO_MAIN_B0_026"
                                           || geoList[index].Name == "GEO_MAIN_B0_027";
                    Renderer[] longHairRenderers = currentModel.transform.GetChildByName("long_hair").GetComponentsInChildren<Renderer>();
                    Renderer[] shortHairRenderers = currentModel.transform.GetChildByName("short_hair").GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in longHairRenderers)
                        renderer.enabled = !garnetShortHair;
                    foreach (Renderer renderer in shortHairRenderers)
                        renderer.enabled = garnetShortHair;
                }
                currentModel.transform.position = Vector3.zero;
                currentModel.transform.localScale = scaleFactor;
                currentModel.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
                foreach (KeyValuePair<Int32, String> anim in animList)
                    AnimationFactory.AddAnimWithAnimatioName(currentModel, anim.Value);
                currentAnimIndex = 0;
                currentAnimName = animList.Count > 0 ? animList[0].Value : "";
                currentModelBones = BoneHierarchyNode.CreateFromModel(currentModel);
                replaceOnce = 4;
            }
            else if (geoList[index].Kind == MODEL_KIND_BBG)
            {
                currentModel.transform.position = Vector3.zero;
                currentModel.transform.localScale = scaleFactor;
                currentModel.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
                currentAnimIndex = 0;
                currentAnimName = "";
                currentModelBones = null;
                replaceOnce = 4;
            }
            else if (geoList[index].Kind == MODEL_KIND_BBG_OBJ)
            {
                currentModel.transform.position = Vector3.zero;
                currentModel.transform.localScale = scaleFactor;
                currentModel.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
                currentAnimIndex = 0;
                currentAnimName = "";
                currentModelBones = null;
            }
            else
            {
                currentAnimIndex = 0;
                currentAnimName = $"Frame: {(spsEffect.curFrame >> 4) + 1}/{spsEffect.frameCount >> 4}";
                currentModelBones = null;
            }
            isLoadingModel = false;
        }

        private static void ChangeWeaponModel(Int32 index)
        {
            if (currentWeaponModel != null && index == currentWeaponGeoIndex)
            {
                UnityEngine.Object.Destroy(currentWeaponModel);
            }
            else
            {
                if (currentBonesID.Count != 0 && currentModel != null)
                {
                    isLoadingWeaponModel = true;
                    if (currentWeaponModel != null)
                        UnityEngine.Object.Destroy(currentWeaponModel);
                    while (index < 0)
                        index += weapongeoList.Count;
                    while (index >= weapongeoList.Count)
                        index -= weapongeoList.Count;
                    currentWeaponGeoIndex = index;
                    Log.Message($"[ModelViewerScene] Change weapon model: {weapongeoList[index].Name}");
                    currentWeaponModel = ModelFactory.CreateModel(weapongeoList[index].Name);
                    WeaponAttach(currentWeaponModel, currentModel, currentBonesID[currentWeaponBoneIndex]);
                    isLoadingWeaponModel = false;
                }
            }
        }

        public static void WeaponAttach(GameObject sourceObject, GameObject targetObject, Int32 bone_index)
        {
            Transform childByName = targetObject.transform.GetChildByName("bone" + bone_index.ToString("D3"));
            sourceObject.transform.parent = childByName;
            sourceObject.transform.localPosition = Vector3.zero;
            sourceObject.transform.localRotation = Quaternion.identity;
            sourceObject.transform.localScale = Vector3.one;
        }

        private static void ChangeAnimation(Int32 index)
        {
            Int32 count = geoList[currentGeoIndex].Kind != MODEL_KIND_SPS ? animList.Count : spsEffect.frameCount >> 4;
            // TODO: use battlebg.getBbgObjAnimation for objects of type MODEL_KIND_BBG_OBJ
            // It only consists of rotation (and hovering movement for BBG_B112)
            if (count == 0)
                return;
            while (index < 0)
                index += count;
            while (index >= count)
                index -= count;
            currentAnimIndex = index;
            if (geoList[currentGeoIndex].Kind == MODEL_KIND_SPS)
            {
                spsEffect.curFrame = index << 4;
                currentAnimName = $"Frame: {index + 1}/{count}";
            }
            else
            {
                currentAnimName = animList[index].Value;
                Animation anim = currentModel.GetComponent<Animation>();
                if (anim != null)
                {
                    anim.Play(currentAnimName);
                    anim[currentAnimName].speed = speedFactor;
                }
            }
        }

        private static GameObject CreateModelForBone()
        {
            Mesh mesh = new Mesh();
            Single radius = 2f;
            Int32 vertCount = 20;
            Vector3[] meshVert = new Vector3[vertCount];
            for (Int32 i = 0; i < vertCount; i++)
            {
                Double angle = Math.PI * 2 * i / vertCount;
                meshVert[i] = new Vector3((Single)(radius * Math.Cos(angle)), (Single)(radius * Math.Sin(angle)), 0f);
            }
            Int32[] meshIndex = new Int32[3 * (vertCount - 2)];
            for (Int32 i = 0; i + 2 < vertCount; i++)
            {
                meshIndex[3 * i] = 0;
                meshIndex[3 * i + 1] = i + 1;
                meshIndex[3 * i + 2] = i + 2;
            }
            mesh.vertices = meshVert;
            mesh.uv = null;
            mesh.triangles = meshIndex;
            GameObject go = ModelFactory.CreateModel("GEO_ACC_F0_BON");
            go.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = mesh;
            go.GetComponentInChildren<SkinnedMeshRenderer>().material.shader = ShadersLoader.Find("SFX_RUSH_ADD");
            return go;
        }

        private static GameObject CreateModelForBoneConnection()
        {
            Mesh mesh = new Mesh();
            Vector3[] meshVert = new Vector3[4];
            Int32[] meshIndex =
            [
                0,
                1,
                2,
                1,
                2,
                3
            ];
            mesh.vertices = meshVert;
            mesh.uv = null;
            mesh.triangles = meshIndex;
            GameObject go = ModelFactory.CreateModel("GEO_ACC_F0_BON");
            go.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = mesh;
            go.GetComponentInChildren<SkinnedMeshRenderer>().material.shader = ShadersLoader.Find("SFX_RUSH_ADD");
            go.transform.localRotation = Quaternion.Euler(0f, 180f, -90f);
            return go;
        }

        private static void MoveBoneConnection(GameObject connectionMesh, Vector3 parentPos, Vector3 childPos)
        {
            Mesh mesh = connectionMesh.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
            Vector3[] meshVert = new Vector3[4];
            meshVert[0] = parentPos + new Vector3(0, 1f, 0f);
            meshVert[1] = parentPos + new Vector3(0, -1f, 0f);
            meshVert[2] = childPos + new Vector3(0, 2f, 0f);
            meshVert[3] = childPos + new Vector3(0, -2f, 0f);
            mesh.vertices = meshVert;
        }

        private static void ExportAnimation(List<String> animNameList)
        {
            String exportConf = "MemoriaExportAnim.txt";
            if (File.Exists(exportConf))
            {
                if (ProcessExportAnimation(File.ReadAllText(exportConf)))
                    File.Delete(exportConf);
            }
            else
            {
                if (GenerateExportAnimationFile(exportConf, animNameList))
                    Process.Start(Path.GetFullPath(exportConf));
            }
        }

        private static Boolean GenerateExportAnimationFile(String filepath, List<String> animNameList)
        {
            String[] animNameToken = animNameList[0].Split('_');
            String animModelName = "GEO_" + animNameToken[1] + "_" + animNameToken[2] + "_" + animNameToken[3];
            String assetPath = "Animations/" + animModelName + "/" + animNameList[0];
            assetPath = AnimationFactory.GetRenameAnimationPath(assetPath);
            AnimationClipReader clipIO;
            AnimationClipReader.ReadAnimationClipFromDisc("StreamingAssets/Assets/Resources/" + assetPath + ".anim", out clipIO);
            if (clipIO == null)
            {
                FF9Sfx.FF9SFX_Play(102);
                return false;
            }
            String config = "// EXPORT ANIMATION\n";
            config += "// Write the configurations for exporting an animation, then save the file and press \"E\" again on the model viewer\n\n";
            foreach (String animName in animNameList)
                config += $"Animation:{animName}\n";
            foreach (String animName in animNameList)
            {
                animNameToken = animName.Split('_');
                animModelName = "GEO_" + animNameToken[1] + "_" + animNameToken[2] + "_" + animNameToken[3];
                assetPath = "Animations/" + animModelName + "/" + animName;
                assetPath = AnimationFactory.GetRenameAnimationPath(assetPath);
                config += $"ExportPath:StreamingAssets/Assets/Resources/{assetPath}.anim\n";
            }
            config += $"DeleteThisOnSuccess:true\n";
            config += $"Reverse:false\n";
            config += $"//AnimPatch:bone000;localRotation;Euler(0, 0, 0)\n";
            config += $"//AnimPatch:bone000;localRotation;Quaternion(0, 0, 0, 1)\n";
            config += $"//AnimPatch:bone000;localPosition;(0, 0, 0)\n";
            config += $"//AnimPatch:bone000;localScale;(1, 1, 1)\n";
            config += $"FrameRate:{clipIO.frameRate}\n";
            config += $"BoneHierarchy:\n";
            foreach (var bone in clipIO.boneAnimList)
                config += $"{bone.boneNameInHierarchy}\n";
            File.WriteAllText(filepath, config);
            return true;
        }

        private static Boolean ProcessExportAnimation(String config)
        {
            Dictionary<String, Dictionary<String, Vector4>> animPatch = new Dictionary<String, Dictionary<String, Vector4>>();
            List<String> animNameList = new List<String>();
            List<String> outputPathList = new List<String>();
            Boolean deleteConfig = true;
            Boolean reverseAnim = false;
            Single frameRate = 60f;
            List<String> boneHierarchy = null;
            Boolean readingBoneHierarchy = false;
            String[] lines = config.Split('\n');
            foreach (String ntline in lines)
            {
                String line = ntline.Trim();
                if (readingBoneHierarchy)
                {
                    if (String.IsNullOrEmpty(line))
                        boneHierarchy.Add("");
                    else if (!line.StartsWith("bone"))
                        readingBoneHierarchy = false;
                    else
                        boneHierarchy.Add(line);
                }
                if (!readingBoneHierarchy)
                {
                    if (line.StartsWith("Animation:"))
                        animNameList.Add(line.Substring("Animation:".Length));
                    else if (line.StartsWith("ExportPath:"))
                        outputPathList.Add(line.Substring("ExportPath:".Length));
                    else if (line.StartsWith("DeleteThisOnSuccess:"))
                        deleteConfig = Boolean.Parse(line.Substring("DeleteThisOnSuccess:".Length));
                    else if (line.StartsWith("FrameRate:"))
                        frameRate = Single.Parse(line.Substring("FrameRate:".Length));
                    else if (line.StartsWith("Reverse:"))
                        reverseAnim = Boolean.Parse(line.Substring("Reverse:".Length));
                    else if (line.StartsWith("AnimPatch:"))
                    {
                        String[] param = line.Substring("AnimPatch:".Length).Split(';');
                        if (param.Length != 3)
                            continue;
                        Dictionary<String, Vector4> baPatch;
                        if (!animPatch.TryGetValue(param[0], out baPatch))
                        {
                            baPatch = new Dictionary<String, Vector4>();
                            animPatch.Add(param[0], baPatch);
                        }
                        if (param[1] == "localRotation")
                        {
                            Quaternion arg;
                            if (param[2].StartsWith("Euler"))
                                arg = Quaternion.Euler(StringToVector(param[2].Substring("Euler".Length)));
                            else if (param[2].StartsWith("Quaternion"))
                                arg = VectorToQuaternion(StringToVector(param[2].Substring("Quaternion".Length)));
                            else
                                arg = VectorToQuaternion(StringToVector(param[2]));
                            baPatch[param[1]] = QuaternionToVector(arg);
                        }
                        else
                        {
                            baPatch[param[1]] = StringToVector(param[2]);
                        }
                    }
                    else if (line.StartsWith("BoneHierarchy:"))
                    {
                        readingBoneHierarchy = true;
                        boneHierarchy = new List<String>();
                    }
                }
            }
            if (animNameList.Count == 0 || outputPathList.Count == 0)
            {
                FF9Sfx.FF9SFX_Play(102);
                return false;
            }
            Boolean allGood = true;
            for (Int32 i = 0; i < animNameList.Count; i++)
            {
                if (i >= outputPathList.Count)
                    continue;
                String[] animNameToken = animNameList[i].Split('_');
                String animModelName = "GEO_" + animNameToken[1] + "_" + animNameToken[2] + "_" + animNameToken[3];
                String assetPath = "Animations/" + animModelName + "/" + animNameList[i];
                assetPath = AnimationFactory.GetRenameAnimationPath(assetPath);
                AnimationClipReader clipIO;
                AnimationClipReader.ReadAnimationClipFromDisc("StreamingAssets/Assets/Resources/" + assetPath + ".anim", out clipIO);
                if (clipIO == null)
                {
                    allGood = false;
                    continue;
                }
                if (reverseAnim)
                {
                    Single duration = 0f;
                    foreach (var ba in clipIO.boneAnimList)
                        foreach (var ta in ba.transformAnimList)
                            foreach (var fa in ta.frameAnimList)
                                if (duration < fa.time)
                                    duration = fa.time;
                    foreach (var ba in clipIO.boneAnimList)
                        foreach (var ta in ba.transformAnimList)
                            foreach (var fa in ta.frameAnimList)
                                fa.time = duration - fa.time;
                }
                clipIO.frameRate = frameRate;
                if (boneHierarchy != null)
                {
                    Int32 lineIndex = 0;
                    for (Int32 j = 0; j < clipIO.boneAnimList.Count; j++)
                    {
                        if (lineIndex >= boneHierarchy.Count || String.IsNullOrEmpty(boneHierarchy[lineIndex]))
                            clipIO.boneAnimList.RemoveAt(j--);
                        else
                            clipIO.boneAnimList[j].boneNameInHierarchy = boneHierarchy[lineIndex];
                        lineIndex++;
                    }
                }
                foreach (var bonePatch in animPatch)
                {
                    var ba = clipIO.boneAnimList.Find(b => b.boneNameInHierarchy == bonePatch.Key);
                    if (ba == null)
                    {
                        ba = new AnimationClipReader.BoneAnimation();
                        ba.boneNameInHierarchy = bonePatch.Key;
                        clipIO.boneAnimList.Add(ba);
                    }
                    foreach (var transfPatch in bonePatch.Value)
                    {
                        var ta = ba.transformAnimList.Find(t => t.transformType == transfPatch.Key);
                        if (ta == null)
                        {
                            ta = new AnimationClipReader.BoneAnimation.TransformAnimation();
                            ta.transformType = transfPatch.Key;
                            ba.transformAnimList.Add(ta);
                        }
                        if (ta.frameAnimList.Count == 0)
                        {
                            var fa = new AnimationClipReader.BoneAnimation.TransformAnimation.FrameAnimation();
                            fa.time = 0f;
                            fa.pos = transfPatch.Value;
                            fa.posInnerTangent = Vector4.zero;
                            fa.posOuterTangent = Vector4.zero;
                            ta.frameAnimList.Add(fa);
                        }
                        else
                        {
                            foreach (var fa in ta.frameAnimList)
                            {
                                if (transfPatch.Key == "localRotation")
                                    fa.pos = QuaternionToVector(VectorToQuaternion(fa.pos) * VectorToQuaternion(transfPatch.Value));
                                else if (transfPatch.Key == "localPosition")
                                    fa.pos += transfPatch.Value;
                                else if (transfPatch.Key == "localScale")
                                    fa.pos = new Vector3(fa.pos.x * transfPatch.Value.x, fa.pos.y * transfPatch.Value.y, fa.pos.z * transfPatch.Value.z);
                            }
                        }
                    }
                }
                String outputPath = outputPathList[i];
                if (!String.IsNullOrEmpty(Path.GetDirectoryName(outputPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                File.WriteAllText(outputPath, clipIO.ParseToJSON());
                AnimationClipReader.LoadedClips.Remove("StreamingAssets/Assets/Resources/" + assetPath + ".anim");
                AnimationClipReader.LoadedClips.Remove(outputPath);
                savedAnimationPath = outputPath;
            }
            if (!allGood)
            {
                FF9Sfx.FF9SFX_Play(102);
                return false;
            }
            return deleteConfig;
        }

        // TODO: maybe add that kind of API somewhere else (ExtensionMethodsVector3?)
        private static Quaternion VectorToQuaternion(Vector4 val)
        {
            return new Quaternion(val.x, val.y, val.z, val.w);
        }
        private static Vector4 QuaternionToVector(Quaternion val)
        {
            return new Vector4(val.x, val.y, val.z, val.w);
        }
        private static Vector4 StringToVector(String val)
        {
            if (val.StartsWith("(") && val.EndsWith(")"))
                val = val.Substring(1, val.Length - 2);
            String[] coordStr = val.Split(',');
            if (coordStr.Length == 0)
                return Vector4.zero;
            Single[] coordNum = new Single[coordStr.Length];
            for (Int32 i = 0; i < coordStr.Length; i++)
                if (!Single.TryParse(coordStr[i], out coordNum[i]))
                    return Vector4.zero;
            if (coordNum.Length == 1)
                return new Vector4(coordNum[0], coordNum[0], coordNum[0], coordNum[0]);
            if (coordNum.Length == 2)
                return new Vector4(coordNum[0], coordNum[1], 0f, 0f);
            if (coordNum.Length == 3)
                return new Vector4(coordNum[0], coordNum[1], coordNum[2], 0f);
            return new Vector4(coordNum[0], coordNum[1], coordNum[2], coordNum[3]);
        }

        private class ModelObject
        {
            public Int32 Id;
            public String Name;
            public Int32 Kind;
        }

        private const Int32 MODEL_KIND_NORMAL = 0;
        private const Int32 MODEL_KIND_BBG = 1;
        private const Int32 MODEL_KIND_BBG_OBJ = 2;
        private const Int32 MODEL_KIND_SPS = 3;
    }
}
