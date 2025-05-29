using Assets.Scripts.Common;
using Assets.Sources.Graphics.Movie;
using Memoria;
using Memoria.Scripts;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class MBG : HonoBehavior
{
    public static MBG Instance
    {
        get
        {
            if (MBG.instance == null)
                MBG.instance = UnityEngine.Object.Instantiate<GameObject>(AssetManager.Load<GameObject>("CommonAsset/MBGData/MBG", false)).GetComponent<MBG>();
            return MBG.instance;
        }
    }

    public static Boolean IsNull => MBG.instance == null;

    public static Boolean IsSkip
    {
        set
        {
            if (!MBG.IsNull)
                MBG.instance.isSkip = value;
        }
    }

    public override void HonoAwake()
    {
        base.HonoAwake();
        this.MBGInitialized = 1;
        this.mbgCamera = this.cameraObject.GetComponent<Camera>();
        this.process = this.cameraObject.GetComponent<MovieMaterialProcessor>();
        this.movieMaterial = MovieMaterial.New(this.process);
        this.moviePlane.GetComponent<Renderer>().material = this.movieMaterial.Material;
        this.moviePlane.transform.localScale = Vector3.Scale(new Vector3(32f, 1f, 22.4f), MovieMaterial.ScaleVector);
        this.mbgCamera.depth = -4096f;
        this.movieMaterial.FastForward = (MovieMaterial.FastForwardMode)((!FF9StateSystem.Settings.IsFastForward) ? MovieMaterial.FastForwardMode.Normal : MovieMaterial.FastForwardMode.HighSpeed);
        this.shader = ShadersLoader.Find("PSX/FieldMapActorMBGMask");
        this.SetFastForward(HonoBehaviorSystem.Instance.IsFastForwardModeActive());
        this.isFastForwardOnBeforePlayingMBG = false;
        this.played = false;
    }

    public override void HonoOnDestroy()
    {
        MBG.MarkCharacterDepth = false;
        //QualitySettings.vSyncCount = 0;
        this.movieMaterial.Destroy();
        base.HonoOnDestroy();
    }

    public void SetFinishCallback(Action callback)
    {
        this.movieMaterial.OnFinished = callback;
    }

    public void Seek(Int32 discNo, Int32 fmvNo)
    {
        this.played = false;
        MBG.MarkCharacterDepth = false;
        this.fadeDuration = 0f;
        MBG_DEF mbg_DEF = MBG.MBGDiscTable[discNo][fmvNo];
        this.MBGType = mbg_DEF.type;
        MBG.MBGParms.pauseRequest = false;
        MBG.MBGParms.firstCall = true;
        MBG.MBGParms.soundCallCount = 0;
        String name = MBG.MBGDiscTable[discNo][fmvNo].name;
        String binaryName = name;
        if (discNo == 4 && fmvNo == 3)
        {
            this.isFMV55D = true;
        }
        else
        {
            this.isFMV55D = false;
        }
        if (discNo == 4 && fmvNo == 9)
        {
            this.isMBG116 = true;
        }
        else
        {
            this.isMBG116 = false;
        }
        if (discNo == 3 && fmvNo == 13)
        {
            this.isFMV045 = true;
        }
        else
        {
            this.isFMV045 = false;
        }
        if (discNo == 4 && fmvNo == 0)
        {
            this.isFMV055A = true;
        }
        else
        {
            this.isFMV055A = false;
        }
        MBGDataReader mbgdataReader = MBGDataReader.Load(binaryName);
        if (mbgdataReader != null)
        {
            this.audioDef = mbgdataReader.audioRefList;
        }
        this.LoadMaskData(name.ToUpper());
        this.LoadMovie(name);
    }

    private void LoadMaskData(String fileName)
    {
        this.maskSheet.Clear();
        this.maskData.Clear();
        this.haveMask = false;
        this.currentMaskID = -1;
        this.loadedMask = 0;
        this.maskCount = 0;
        if (this.maskSheetData.ContainsKey(fileName))
        {
            this.haveMask = true;
            this.maskName = fileName;
            this.maskCount = this.maskSheetData[this.maskName];
            for (Int32 i = 0; i < this.maskCount; i++)
            {
                String name = String.Concat(new Object[]
                {
                    "EmbeddedAsset/Movie/Mask/",
                    this.maskName,
                    "/",
                    this.maskName,
                    "_",
                    i,
                    ".txt"
                });
                String textAsset = AssetManager.LoadString(name);
                JSONNode jsonnode = JSONNode.Parse(textAsset);
                JSONClass asObject = jsonnode["frames"].AsObject;
                foreach (Object obj in asObject)
                {
                    String key = ((KeyValuePair<String, JSONNode>)obj).Key;
                    Int32 key2 = Int32.Parse(key);
                    MaskFrame maskFrame = new MaskFrame();
                    JSONClass asObject2 = asObject[key].AsObject;
                    JSONClass asObject3 = asObject2["frame"].AsObject;
                    Vector4 frame = new Vector4((Single)asObject3["x"].AsInt, (Single)asObject3["y"].AsInt, (Single)asObject3["w"].AsInt, (Single)asObject3["h"].AsInt);
                    JSONClass asObject4 = asObject2["spriteSourceSize"].AsObject;
                    Vector4 sourceSize = new Vector4((Single)asObject4["x"].AsInt, (Single)asObject4["y"].AsInt, (Single)asObject4["w"].AsInt, (Single)asObject4["h"].AsInt);
                    maskFrame.frame = frame;
                    maskFrame.sourceSize = sourceSize;
                    maskFrame.sheetID = i;
                    this.maskData.Add(key2, maskFrame);
                }
            }
            if (this.maskCount > 0)
            {
                this.UpdateMarkSheet();
            }
        }
    }

    public void LoadMovie(String fileName)
    {
        this.MBGInitialized++;
        this.movieMaterial.Load(fileName);
        SoundLib.UnloadMovieResources();
        SoundLib.LoadMovieResources("MovieAudio/", new String[]
        {
            fileName
        });
    }

    public void SetFieldMap(FieldMap fieldMap)
    {
        this.mbgCamera.depth = -4096f;
        this.currentFieldMap = fieldMap;
        MBG.MBGParms.oldBGCamNdx = this.currentFieldMap.camIdx;
        Camera mainCamera = fieldMap.GetMainCamera();
        this.SetMovieCamera(mainCamera);
    }

    public void Play()
    {
        this.tempTargetFrameRate = FPSManager.GetTargetFPS();
        this.tempVirtualAnalogStatus = VirtualAnalog.IsEnable;
        PersistenSingleton<HonoInputManager>.Instance.SetVirtualAnalogEnable(false);
        this.isSkip = false;
        this.MBGInitialized++;
        if (PersistenSingleton<FF9StateSystem>.Instance.mode == 1 || PersistenSingleton<FF9StateSystem>.Instance.mode == 5)
        {
            MBG.MBGParms.oldCharOTOffset = FF9StateSystem.Field.FF9Field.loc.map.charOTOffset;
            FF9StateSystem.Field.FF9Field.loc.map.charOTOffset = 0;
            Shader.SetGlobalFloat("_DepthOffset", (Single)FF9StateSystem.Field.FF9Field.loc.map.charOTOffset);
            this.background = GameObject.Find("Background");
            this.enableBackground = false;
        }
        PersistenSingleton<UIManager>.Instance.Booster.CloseBoosterPanel();
        if (this.MBGType != 1 && !this.isEnding && !this.isTitle && !this.isMovieGallery)
        {
            UIManager.Field.MovieHitArea.SetActive(true);
        }
        if (this.MBGType == 1 && FF9StateSystem.Settings.IsFastForward)
        {
            this.isFastForwardOnBeforePlayingMBG = true;
            FF9StateSystem.Settings.CallBoosterButtonFuntion(BoosterType.HighSpeedMode, false);
            PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.HighSpeedMode, false);
        }
        this.isWaitForPause = false;
        this.played = true;
        FPSManager.SetTargetFPS(Mathf.RoundToInt((Single)this.movieMaterial.FPS));
        PlayerWindow.Instance.SetTitle($"FMV: {this.movieMaterial.movieKey} | Map: {FF9StateSystem.Common.FF9.fldMapNo} ({FF9StateSystem.Common.FF9.mapNameStr}) | Index/Counter: {PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR)}/{PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR)} | Loc: {FF9StateSystem.Common.FF9.fldLocNo}");
        this.movieMaterial.Play();
    }

    public void Pause(Boolean doPause)
    {
        if (this.movieMaterial.PlayPosition < this.movieMaterial.Duration && this.played)
        {
            if (doPause)
            {
                vib.VIB_actuatorReset(0);
                this.movieMaterial.Pause();
            }
            else
            {
                this.isWaitForPause = false;
                this.movieMaterial.Resume();
            }
        }
        else if (this.played)
        {
            if (doPause)
            {
                if (!this.isWaitForPause)
                {
                    this.isWaitForPause = true;
                    base.StartCoroutine(this.WaitForPause());
                }
            }
            else
            {
                this.isWaitForPause = false;
            }
        }
    }

    private IEnumerator WaitForPause()
    {
        while ((MBG.Instance.IsPlaying() & 2UL) == 0UL)
        {
            yield return null;
        }
        if (this.isWaitForPause)
        {
            vib.VIB_actuatorReset(0);
            this.movieMaterial.Pause();
        }
        yield break;
    }

    public void Stop()
    {
        if (PersistenSingleton<FF9StateSystem>.Instance.mode == 1 || PersistenSingleton<FF9StateSystem>.Instance.mode == 5)
        {
            FF9StateSystem.Field.FF9Field.loc.map.charOTOffset = MBG.MBGParms.oldCharOTOffset;
            Shader.SetGlobalFloat("_DepthOffset", FF9StateSystem.Field.FF9Field.loc.map.charOTOffset);
            if (this.currentFieldMap != null)
            {
                this.currentFieldMap.camIdx = -1;
                this.currentFieldMap.SetCurrentCameraIndex(MBG.MBGParms.oldBGCamNdx);
            }
            this.enableBackground = true;
            if (this.background != (UnityEngine.Object)null)
            {
                this.background.SetActive(this.enableBackground);
            }
        }
        ObjList activeObjList = PersistenSingleton<EventEngine>.Instance.GetActiveObjList();
        for (ObjList objList = activeObjList; objList != null; objList = objList.next)
        {
            if (objList.obj.cid == 4 && !(objList.obj.go == (UnityEngine.Object)null))
            {
                SkinnedMeshRenderer[] componentsInChildren = objList.obj.go.GetComponentsInChildren<SkinnedMeshRenderer>();
                SkinnedMeshRenderer[] array = componentsInChildren;
                for (Int32 i = 0; i < (Int32)array.Length; i++)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = array[i];
                    Material[] materials = skinnedMeshRenderer.materials;
                    for (Int32 j = 0; j < (Int32)materials.Length; j++)
                    {
                        Material material = materials[j];
                        material.renderQueue = 2000;
                    }
                }
            }
        }
        PersistenSingleton<HonoInputManager>.Instance.SetVirtualAnalogEnable(this.tempVirtualAnalogStatus);
        SceneDirector.ToggleFadeAll(true);
        FPSManager.SetTargetFPS(this.tempTargetFrameRate);
        //PlayerWindow.Instance.SetTitle(String.Empty);
        PlayerWindow.Instance.SetTitle($"Map: {FF9StateSystem.Common.FF9.fldMapNo} ({FF9StateSystem.Common.FF9.mapNameStr}) | Index/Counter: {PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR)}/{PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR)} | Loc: {FF9StateSystem.Common.FF9.fldLocNo}");

        vib.VIB_actuatorReset(0);
        this.movieMaterial.Transparency = 0f;
        this.movieMaterial.Stop();
        this.mbgCamera.depth = -4096f;
        this.MBGInitialized--;
        MBG.MarkCharacterDepth = false;
        this.played = false;
        PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(true);
    }

    public void Purge()
    {
        this.Stop();
        this.MBGInitialized = 0;
    }

    public void UpdateCamera()
    {
        Int32 num = this.GetFrame;
        if (this.audioDef != null && num < this.audioDef.Count)
        {
            MBG.MarkCharacterDepth = true;
            AUDIO_HDR_DEF audio_HDR_DEF = this.audioDef[num];
            if (this.isMBG116 && num >= 1152 && num <= 1155)
            {
                audio_HDR_DEF = this.audioDef[1151];
            }
            MBG_CAM_DEF mbgCameraA = audio_HDR_DEF.mbgCameraA;
            Vector2 centerOffset = mbgCameraA.GetCenterOffset();
            if (this.currentFieldMap != (UnityEngine.Object)null && FF9StateSystem.Common.FF9.fldMapNo != 2752)
            {
                Vector2 offset = this.currentFieldMap.offset;
                Vector2 zero = Vector2.zero;
                zero.x = (Single)((Int16)(this.currentFieldMap.offset.x - (Single)mbgCameraA.centerOffset[0]));
                zero.y = (Single)((Int16)(this.currentFieldMap.offset.y + (Single)mbgCameraA.centerOffset[1]));
                this.currentFieldMap.mainCamera.transform.localPosition = zero;
                Shader.SetGlobalMatrix("_MatrixRT", mbgCameraA.GetMatrixRT());
                Shader.SetGlobalFloat("_ViewDistance", mbgCameraA.GetViewDistance());
                FF9StateSystem.Common.FF9.cam = mbgCameraA.GetMatrixRT();
                FF9StateSystem.Common.FF9.proj = mbgCameraA.proj;
            }
            if ((FF9StateSystem.Settings.cfg.vibe == (UInt64)FF9CFG.FF9CFG_VIBE_ON || PersistenSingleton<FF9StateSystem>.Instance.mode == 5) && !PersistenSingleton<UIManager>.Instance.IsPause && UIManager.Field != (UnityEngine.Object)null && !UIManager.Field.isShowSkipMovieDialog)
            {
                vib.VIB_actuatorSet(0, audio_HDR_DEF.vibrate[0] / 255f, audio_HDR_DEF.vibrate[1] / 255f);
            }
        }
        if (FF9StateSystem.Common.FF9.fldMapNo != 2933)
        {
            num++;
        }
        if (this.haveMask && this.maskData.ContainsKey(num))
        {
            ObjList activeObjList = PersistenSingleton<EventEngine>.Instance.GetActiveObjList();
            for (ObjList objList = activeObjList; objList != null; objList = objList.next)
            {
                if (objList.obj.cid == 4)
                {
                    SkinnedMeshRenderer[] componentsInChildren = objList.obj.go.GetComponentsInChildren<SkinnedMeshRenderer>();
                    SkinnedMeshRenderer[] array = componentsInChildren;
                    for (Int32 i = 0; i < (Int32)array.Length; i++)
                    {
                        SkinnedMeshRenderer skinnedMeshRenderer = array[i];
                        Material[] materials = skinnedMeshRenderer.materials;
                        for (Int32 j = 0; j < (Int32)materials.Length; j++)
                        {
                            Material material = materials[j];
                            if (skinnedMeshRenderer.material.shader != this.shader)
                            {
                                skinnedMeshRenderer.material.shader = this.shader;
                                skinnedMeshRenderer.material.renderQueue = this.renderQueue;
                            }
                        }
                    }
                }
            }
            MaskFrame maskFrame = this.maskData[num];
            if (this.isMBG116 && num >= 1150 && num <= 1154)
            {
                maskFrame = this.maskData[1149];
            }
            Shader.SetGlobalVector("_Frame", maskFrame.frame);
            Shader.SetGlobalVector("_SpriteSourceSize", maskFrame.sourceSize);
            Int32 sheetID = maskFrame.sheetID;
            Vector2 zero2 = Vector2.zero;
            zero2.x = maskFrame.frame.z;
            zero2.y = maskFrame.frame.w;
            if (this.maskSheet.ContainsKey(sheetID) && sheetID != this.currentMaskID && zero2 != Vector2.one)
            {
                Texture2D tex = this.maskSheet[sheetID];
                Shader.SetGlobalTexture("_MBGMask", tex);
                if (this.maskSheet.ContainsKey(this.currentMaskID))
                {
                    Resources.UnloadAsset(this.maskSheet[this.currentMaskID]);
                    this.maskSheet.Remove(this.currentMaskID);
                    this.UpdateMarkSheet();
                }
                this.currentMaskID = sheetID;
            }
        }
        if (this.currentFieldMap != (UnityEngine.Object)null && FF9StateSystem.Common.FF9.fldMapNo == 2933)
        {
            ObjList activeObjList2 = PersistenSingleton<EventEngine>.Instance.GetActiveObjList();
            for (ObjList objList2 = activeObjList2; objList2 != null; objList2 = objList2.next)
            {
                if (objList2.obj.uid == 4 && num == 1154)
                {
                    Actor actor = (Actor)objList2.obj;
                    actor.flags = 0;
                }
                if (objList2.obj.uid == 4)
                {
                    Actor actor2 = (Actor)objList2.obj;
                    if (num > 1045)
                    {
                        Vector3 curPos = actor2.fieldMapActorController.curPos;
                        curPos.y = -30f;
                        actor2.fieldMapActorController.curPos = curPos;
                        actor2.fieldMapActorController.SyncPosToTransform();
                    }
                }
                if (objList2.obj.uid == 1)
                {
                    Actor actor3 = (Actor)objList2.obj;
                    if (num == 366 || num == 392)
                    {
                        actor3.flags = 0;
                    }
                    else if (num == 393)
                    {
                        actor3.flags = 1;
                    }
                    else if ((num > 628 && num < 636) || num == 392 || num == 1156 || num == 89)
                    {
                        SkinnedMeshRenderer[] componentsInChildren2 = objList2.obj.go.GetComponentsInChildren<SkinnedMeshRenderer>();
                        SkinnedMeshRenderer[] array2 = componentsInChildren2;
                        for (Int32 k = 0; k < (Int32)array2.Length; k++)
                        {
                            SkinnedMeshRenderer skinnedMeshRenderer2 = array2[k];
                            skinnedMeshRenderer2.enabled = true;
                        }
                        Transform transform = objList2.obj.go.transform.FindChild("battle_model");
                        if (transform == (UnityEngine.Object)null)
                        {
                            return;
                        }
                        Renderer[] componentsInChildren3 = transform.GetComponentsInChildren<Renderer>();
                        Renderer[] array3 = componentsInChildren3;
                        for (Int32 l = 0; l < (Int32)array3.Length; l++)
                        {
                            Renderer renderer = array3[l];
                            renderer.enabled = false;
                        }
                    }
                    else if (num == 88 || num == 391 || num == 1154 || num == 1155)
                    {
                        SkinnedMeshRenderer[] componentsInChildren4 = objList2.obj.go.GetComponentsInChildren<SkinnedMeshRenderer>();
                        SkinnedMeshRenderer[] array4 = componentsInChildren4;
                        for (Int32 m = 0; m < (Int32)array4.Length; m++)
                        {
                            SkinnedMeshRenderer skinnedMeshRenderer3 = array4[m];
                            skinnedMeshRenderer3.enabled = false;
                        }
                        Transform transform2 = objList2.obj.go.transform.FindChild("battle_model");
                        if (transform2 == (UnityEngine.Object)null)
                        {
                            return;
                        }
                        Renderer[] componentsInChildren5 = transform2.GetComponentsInChildren<Renderer>();
                        Renderer[] array5 = componentsInChildren5;
                        for (Int32 n = 0; n < (Int32)array5.Length; n++)
                        {
                            Renderer renderer2 = array5[n];
                            renderer2.enabled = false;
                        }
                    }
                    else if (num == 980)
                    {
                        actor3.flags = 0;
                    }
                }
                else if (objList2.obj.uid == 5)
                {
                    Actor actor4 = (Actor)objList2.obj;
                    if (num == 42)
                    {
                        actor4.flags = 0;
                    }
                }
            }
        }
    }

    private void UpdateMarkSheet()
    {
        while (this.loadedMask < this.maskCount && this.maskSheet.Count < 3)
        {
            this.corutineLoadMask.Add(this.loadedMask, base.StartCoroutine(this.LoadMaskSheet(this.loadedMask)));
            this.loadedMask++;
        }
    }

    private IEnumerator LoadMaskSheet(Int32 maskID)
    {
        String filePath = String.Concat(new Object[]
        {
            "EmbeddedAsset/Movie/Mask/",
            this.maskName,
            "/",
            this.maskName,
            "_",
            this.loadedMask
        });
        AssetManagerRequest resourceRequest = AssetManager.LoadAsync<Texture2D>(filePath);
        while (!resourceRequest.isDone)
        {
            yield return 0;
        }
        Texture2D maskResource = resourceRequest.asset as Texture2D;
        this.maskSheet.Add(maskID, maskResource);
        base.StopCoroutine(this.corutineLoadMask[maskID]);
        this.corutineLoadMask.Remove(maskID);
        yield break;
    }

    private void Update()
    {
        Boolean flag = PersistenSingleton<UIManager>.Instance.IsPause || (UIManager.Field != null && UIManager.Field.isShowSkipMovieDialog) || this.isSkip;
        if (this.isPause != flag)
        {
            this.isPause = flag;
            this.Pause(this.isPause);
        }
    }

    public override void HonoUpdate()
    {
        base.HonoUpdate();
        if (FF9StateSystem.Common.FF9.fldMapNo != 2933) // last/cw mbg 1
            this.MBGUpdate();
    }

    private void LateUpdate()
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 2933) // last/cw mbg 1
            this.MBGUpdate();
    }

    public void MBGUpdate()
    {
        if (this.MBGInitialized <= 1)
            return;
        if (this.currentFadeDuration < this.fadeDuration)
        {
            this.currentFadeDuration += Time.deltaTime;
            if (this.currentFadeDuration >= this.fadeDuration)
                this.currentFadeDuration = this.fadeDuration;
            this.movieMaterial.Transparency = this.alphaFrom + (this.alphaTo - this.alphaFrom) * this.currentFadeDuration / this.fadeDuration;
            if (this.currentFadeDuration >= this.fadeDuration && this.fadeCallback != null)
                this.fadeCallback();
        }
        if (this.movieMaterial.GetFirstFrame)
        {
            if (this.updateSwitchCamera)
            {
                this.updateSwitchCamera = false;
                this.Set24BitMode(this.MBGIsRGB24);
            }
            if (this.background != null && this.background.activeSelf != this.enableBackground)
                this.background.SetActive(this.enableBackground);
            if (this.movieMaterial.PlayPosition < this.movieMaterial.Duration)
            {
                this.UpdateCamera();
                if (!this.isEnding)
                    SceneDirector.ToggleFadeAll(false);
            }
        }
    }

    public void Fade(Single alphaFrom, Single alphaTo, Single duration, Action callback = null)
    {
        this.alphaFrom = alphaFrom;
        this.alphaTo = alphaTo;
        this.currentFadeDuration = 0f;
        this.fadeDuration = duration;
        this.fadeCallback = callback;
    }

    public Boolean IsFinished()
    {
        return this.movieMaterial == null || !this.movieMaterial.GetFirstFrame || (this.movieMaterial.GetFirstFrame && this.movieMaterial.Frame >= this.movieMaterial.TotalFrame) || this.isSkip;
    }

    public Boolean HasJustFinished()
    {
        return this.IsFinished() && this.played;
    }

    public bool IsFinishedForDisableBooster()
    {
        return !this.played || this.isSkip;
    }

    public Int32 GetFrame => this.movieMaterial.Frame;

    public void SetFadeInParameters(Int32 threshold, Int32 tickCount, Int32 targetVolume)
    {
        if (threshold < 0)
        {
            MBG.MBGParms.threshold = 3UL;
        }
        else
        {
            MBG.MBGParms.threshold = (UInt64)((Int64)threshold);
        }
        if (tickCount < 0)
        {
            MBG.MBGParms.tickCount = 4UL;
        }
        else
        {
            MBG.MBGParms.tickCount = (UInt64)((Int64)tickCount);
        }
        if (targetVolume < 0)
        {
            MBG.MBGParms.targetVolume = 127UL;
        }
        else
        {
            MBG.MBGParms.targetVolume = (UInt64)((Int64)targetVolume);
        }
    }

    public unsafe Int32 Set24BitMode(Int32 isOn)
    {
        UInt64 num = this.IsPlaying();
        if (isOn == 1)
        {
            this.MBGIsRGB24 = 1;
            if (this.movieMaterial.GetFirstFrame)
            {
                this.mbgCamera.depth = 1f;
                this.SetMovieCamera(this.mbgCamera);
                SceneDirector.ToggleFadeAll(true);
            }
            else
            {
                this.updateSwitchCamera = true;
            }
            FF9StateSystem.Common.FF9.attr |= 20u;
            if (PersistenSingleton<FF9StateSystem>.Instance.mode == 1 || PersistenSingleton<FF9StateSystem>.Instance.mode == 5)
            {
                FF9StateSystem.Field.FF9Field.attr |= 9u;
            }
            this.renderQueue = 2000;
        }
        else
        {
            this.updateSwitchCamera = false;
            this.MBGIsRGB24 = 0;
            FieldMap component = GameObject.Find("FieldMap").GetComponent<FieldMap>();
            if (component != (UnityEngine.Object)null)
            {
                this.SetFieldMap(component);
            }
            if (PersistenSingleton<FF9StateSystem>.Instance.mode == 1 || PersistenSingleton<FF9StateSystem>.Instance.mode == 5)
            {
                FieldMap.FF9FieldAttr.ff9[1, 0] |= (UInt16)20;
                FieldMap.FF9FieldAttr.field[1, 0] = (UInt16)9;
            }
            this.renderQueue = 3001;
        }
        ObjList activeObjList = PersistenSingleton<EventEngine>.Instance.GetActiveObjList();
        for (ObjList objList = activeObjList; objList != null; objList = objList.next)
        {
            if (objList.obj.cid == 4)
            {
                if (objList.obj.go != (UnityEngine.Object)null)
                {
                    SkinnedMeshRenderer[] componentsInChildren = objList.obj.go.GetComponentsInChildren<SkinnedMeshRenderer>();
                    SkinnedMeshRenderer[] array = componentsInChildren;
                    for (Int32 i = 0; i < (Int32)array.Length; i++)
                    {
                        SkinnedMeshRenderer skinnedMeshRenderer = array[i];
                        Material[] materials = skinnedMeshRenderer.materials;
                        for (Int32 j = 0; j < (Int32)materials.Length; j++)
                        {
                            Material material = materials[j];
                            material.renderQueue = this.renderQueue;
                        }
                    }
                }
            }
        }
        return 1;
    }

    public void SetModeEnding()
    {
        this.isEnding = true;
        this.Set24BitMode(1);
    }

    public void SetFastForward(Boolean IsFastFprward)
    {
        if (this.movieMaterial != null)
        {
            this.movieMaterial.FastForward = (MovieMaterial.FastForwardMode)((!IsFastFprward) ? MovieMaterial.FastForwardMode.Normal : MovieMaterial.FastForwardMode.HighSpeed);
        }
    }

    public void ResetFlags()
    {
        this.isEnding = false;
        this.isTitle = false;
        this.isMovieGallery = false;
        this.isSkip = false;
    }

    public Int32 GetFrameCount => this.movieMaterial.TotalFrame;
    public Boolean is24BitMode => this.MBGIsRGB24 == 1;

    public UInt64 IsPlaying()
    {
        UInt64 num = 0UL;
        if ((Int32)this.MBGInitialized >= 1)
        {
            num |= 1UL;
            if (this.GetFrame > 0)
            {
                num |= 2UL;
            }
            if (this.MBGIsRGB24 != 0)
            {
                num |= 4UL;
            }
            if (this.MBGType == 1)
            {
                num |= 8UL;
            }
        }
        return num;
    }

    private Int32 MBG_isInitialized()
    {
        return (Int32)this.MBGInitialized;
    }

    private void SetMovieCamera(Camera movieCamera)
    {
        this.moviePlane.transform.SetParent(movieCamera.transform);
        this.moviePlane.transform.localPosition = Vector3.forward * 2f;
        this.moviePlane.transform.localScale = Vector3.Scale(new Vector3(32f, 1f, 22.4f), MovieMaterial.ScaleVector);
        this.moviePlane.transform.localRotation = Quaternion.Euler(90f, 180f, 0f);
    }

    public void SetDepthForTitle()
    {
        this.isTitle = true;
        this.mbgCamera.depth = 0f;
    }

    public void SetDepthForMovieGallery()
    {
        this.isMovieGallery = true;
        this.mbgCamera.depth = 2f;
    }

    public const Byte MBG_DEFAULT_THRESHOLD = 3;

    public const Byte MBG_DEFAULT_TICKS = 4;

    public const Byte MBG_DEFAULT_TARGET = 127;

    public const Byte MBG_INITIALIZED = 1;

    public const Byte MBG_PLAYING = 2;

    public const Byte MBG_24BIT = 4;

    public const Byte MBG_DATA_INTERLEAVE = 8;

    public const Byte MBG_PAUSED = 16;

    private const Int32 maxStoreSheet = 3;

    private const Int32 geometryQueue = 2000;

    private const Int32 transparentQueue = 3001;

    public static readonly MBG_DEF[][] MBGDiscTable = new MBG_DEF[][]
    {
        new MBG_DEF[]
        {
            new MBG_DEF("\\SPARE\\FMV090.STR;1", 1, 0),
            new MBG_DEF("\\SPARE\\FMV091.STR;1", 1, 0),
            new MBG_DEF("\\SPARE\\FMV092.STR;1", 1, 0),
            new MBG_DEF("\\SPARE\\FMV093.STR;1", 1, 0),
            new MBG_DEF("\\SPARE\\FMV094.STR;1", 1, 0),
            new MBG_DEF("\\TEST\\FMV910.STR;1", 1, 0),
            new MBG_DEF("\\TEST\\FMV920.STR;1", 1, 0),
            new MBG_DEF("\\TEST\\FMV930.STR;1", 1, 0),
            new MBG_DEF("\\TEST\\FMV940.STR;1", 1, 0),
            new MBG_DEF("\\TEST\\FMV950.STR;1", 1, 0),
            new MBG_DEF("\\TEST\\FMV960.STR;1", 1, 0),
            new MBG_DEF("\\TEST\\FMV970.STR;1", 1, 0),
            new MBG_DEF("\\TEST\\FMV980.STR;1", 1, 0),
            new MBG_DEF("\\TEST\\FMV990.STR;1", 1, 0)
        },
        new MBG_DEF[]
        {
            new MBG_DEF("\\OPENING\\FMVD001.STR;1", 1, 0),
            new MBG_DEF("\\OPENING\\FMVD002.STR;1", 1, 0),
            new MBG_DEF("\\OPENING\\FMVD003.STR;1", 1, 0),
            new MBG_DEF("FMV001", 1, 0),
            new MBG_DEF("FMV002", 1, 0),
            new MBG_DEF("mbg101", 0, 1),
            new MBG_DEF("\\SEQ01\\FMV002X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ01\\FMV002Y.STR;1", 1, 0),
            new MBG_DEF("FMV003", 1, 0),
            new MBG_DEF("FMV004", 1, 0),
            new MBG_DEF("FMV005", 1, 0),
            new MBG_DEF("FMV006A", 1, 0),
            new MBG_DEF("FMV006B", 1, 0),
            new MBG_DEF("mbg102", 0, 1),
            new MBG_DEF("\\SEQ02\\FMV007X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ02\\FMV007Y.STR;1", 1, 0),
            new MBG_DEF("FMV008", 1, 0),
            new MBG_DEF("mbg105", 0, 1),
            new MBG_DEF("\\SEQ02\\FMV010X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ02\\FMV010Y.STR;1", 1, 0),
            new MBG_DEF("FMV011", 1, 0),
            new MBG_DEF("FMV012", 1, 0),
            new MBG_DEF("FMV013", 1, 0),
            new MBG_DEF("mbg103", 0, 1),
            new MBG_DEF("\\SEQ02\\FMV014X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ02\\FMV014Y.STR;1", 1, 0),
            new MBG_DEF("FMV015", 1, 0),
            new MBG_DEF("FMV016", 1, 0),
            new MBG_DEF("FMV017", 1, 0)
        },
        new MBG_DEF[]
        {
            new MBG_DEF("mbg106", 0, 1),
            new MBG_DEF("\\SEQ04\\FMV018X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ04\\FMV018Y.STR;1", 1, 0),
            new MBG_DEF("FMV019", 1, 0),
            new MBG_DEF("FMV021", 1, 0),
            new MBG_DEF("mbg107", 0, 1),
            new MBG_DEF("\\SEQ05\\FMV022X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ05\\FMV022Y.STR;1", 1, 0),
            new MBG_DEF("FMV023", 1, 0),
            new MBG_DEF("FMV024", 1, 0),
            new MBG_DEF("mbg108", 0, 1),
            new MBG_DEF("\\SEQ06\\FMV025X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ06\\FMV025Y.STR;1", 1, 0),
            new MBG_DEF("mbg109", 0, 1),
            new MBG_DEF("\\SEQ06\\FMV026X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ06\\FMV026Y.STR;1", 1, 0),
            new MBG_DEF("FMV027", 1, 0),
            new MBG_DEF("FMV029", 1, 0),
            new MBG_DEF("mbg110", 0, 1),
            new MBG_DEF("\\SEQ06\\FMV030X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ06\\FMV030Y.STR;1", 1, 0),
            new MBG_DEF("FMV031", 1, 0),
            new MBG_DEF("FMV032", 1, 0),
            new MBG_DEF("FMV033", 1, 0)
        },
        new MBG_DEF[]
        {
            new MBG_DEF("FMV034", 1, 0),
            new MBG_DEF("FMV035", 1, 0),
            new MBG_DEF("FMV036", 1, 0),
            new MBG_DEF("mbg111", 0, 1),
            new MBG_DEF("\\SEQ08\\FMV037X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ08\\FMV037Y.STR;1", 1, 0),
            new MBG_DEF("FMV038", 1, 0),
            new MBG_DEF("FMV039", 1, 0),
            new MBG_DEF("FMV040", 1, 0),
            new MBG_DEF("mbg112", 0, 1),
            new MBG_DEF("\\SEQ09\\FMV041X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ09\\FMV041Y.STR;1", 1, 0),
            new MBG_DEF("FMV042", 1, 0),
            new MBG_DEF("FMV045", 1, 0),
            new MBG_DEF("FMV046", 1, 0),
            new MBG_DEF("mbg113", 0, 1),
            new MBG_DEF("\\SEQ10\\FMV048X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ10\\FMV048Y.STR;1", 1, 0),
            new MBG_DEF("FMV052", 1, 0),
            new MBG_DEF("FMV053", 1, 0)
        },
        new MBG_DEF[]
        {
            new MBG_DEF("FMV055A", 1, 0),
            new MBG_DEF("FMV055B", 1, 0),
            new MBG_DEF("FMV055C", 1, 0),
            new MBG_DEF("FMV055D", 1, 0),
            new MBG_DEF("mbg114", 0, 1),
            new MBG_DEF("\\SEQ11\\FMV055X.STR;1", 1, 0),
            new MBG_DEF("\\SEQ11\\FMV055Y.STR;1", 1, 0),
            new MBG_DEF("FMV056", 1, 0),
            new MBG_DEF("mbg115", 0, 1),
            new MBG_DEF("mbg116", 0, 1),
            new MBG_DEF("mbg117", 0, 1),
            new MBG_DEF("mbg118", 0, 1),
            new MBG_DEF("mbg119", 0, 1),
            new MBG_DEF("\\SEQ11\\FMV057Q.STR;1", 1, 0),
            new MBG_DEF("\\SEQ11\\FMV057R.STR;1", 1, 0),
            new MBG_DEF("\\SEQ11\\FMV057S.STR;1", 1, 0),
            new MBG_DEF("\\SEQ11\\FMV057T.STR;1", 1, 0),
            new MBG_DEF("\\SEQ11\\FMV057U.STR;1", 1, 0),
            new MBG_DEF("FMV059", 1, 0),
            new MBG_DEF("FMV060", 1, 0)
        }
    };

    private readonly Dictionary<String, Int32> maskSheetData = new Dictionary<String, Int32>
    {
        {
            "MBG105",
            2
        },
        {
            "MBG108",
            1
        },
        {
            "MBG116",
            13
        },
        {
            "MBG117",
            5
        }
    };

    public static MBG_PARMS_DEF MBGParms;

    public MovieMaterial movieMaterial;

    private List<AUDIO_HDR_DEF> audioDef;

    private Byte MBGIsRGB24;

    private SByte MBGInitialized;

    public Byte MBGType;

    public Boolean isFastForwardOnBeforePlayingMBG;

    private Int32 sortingOrder;

    private MovieMaterialProcessor process;

    public GameObject moviePlane;

    public GameObject cameraObject;

    private Camera mbgCamera;

    private FieldMap currentFieldMap;

    private Vector2 startCamOffset = Vector2.zero;

    private Single alphaFrom;

    private Single alphaTo;

    private Single fadeDuration;

    private Single currentFadeDuration;

    private Action fadeCallback;

    private Int32 defaultMBGDepth = -8;

    private static MBG instance = (MBG)null;

    private Shader shader;

    private Dictionary<Int32, Texture2D> maskSheet = new Dictionary<Int32, Texture2D>();

    private Dictionary<Int32, Coroutine> corutineLoadMask = new Dictionary<Int32, Coroutine>();

    private Dictionary<Int32, MaskFrame> maskData = new Dictionary<Int32, MaskFrame>();

    private Boolean haveMask;

    private String maskName = String.Empty;

    private Int32 loadedMask;

    private Int32 maskCount;

    private Int32 currentMaskID;

    private Int32 renderQueue = 2000;

    private Int32 tempTargetFrameRate;

    private Boolean updateSwitchCamera;

    private GameObject background;

    private Boolean enableBackground = true;

    private Boolean isSkip;

    private Boolean isEnding;

    private Boolean isTitle;

    private Boolean isMovieGallery;

    public Boolean isFMV55D;

    public Boolean isFMV045;

    public Boolean isMBG116;

    public Boolean isFMV055A;

    private Boolean played;

    private Boolean isWaitForPause;

    public static Boolean MarkCharacterDepth = false;

    private Boolean tempVirtualAnalogStatus;

    private Boolean isPause;

    internal class MaskFrame
    {
        public Vector4 frame;
        public Vector4 sourceSize;
        public Int32 sheetID;
    }
}
