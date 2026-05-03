using AOT;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class SFXData
{
    public SpecialEffect id;
    public CMD_DATA cmdRef;
    public BTL_VFX_REQ sfxRequest;
    public Boolean useCamera;
    public Int32 firstMeshFrame;
    public Boolean loadHasEnded;
    public Boolean isEventSFX;

    public List<BattleActionThread> sfxthread;
    public List<SFXDataCamera> camera;
    public SFXDataMesh mesh;

    public List<RunningInstance> runningSFX;

    private Boolean cancel;

    public static SFXData LoadCur;
    public static Boolean lockLoading;
    public static Dictionary<Int32, SFXData> EventSFX = new Dictionary<Int32, SFXData>(); // TODO: Event SFX would be SFX tied to event scripts (battle scripts but also field/wm scripts). Loading these SFX and rendering them in fields is not possible yet.
    private static Queue<SFXData> loadingQueue = new Queue<SFXData>();
    private static HashSet<SFXData> sfxLoadedNotPlayed = new HashSet<SFXData>();

    private const Single LoadingTimeAllocatedPerFrame = 1000f * 0.5f; // 500: half of the frame's duration

    public static void Reinit()
    {
        SFXData.LoadCur = null;
        SFXData.lockLoading = false;
        SFXData.EventSFX.Clear();
        SFXData.loadingQueue.Clear();
        SFXData.sfxLoadedNotPlayed.Clear();
        SFXDataMesh.Raw.RenderingCount = 0;
        SFXDataMesh.Runtime.RenderingCount = 0;
        SFXDataMesh.JSON.RenderingCount = 0;
    }

    public static void LoadLoop()
    {
        if (LoadCur == null && loadingQueue.Count == 0)
            return;
        Stopwatch watch = new Stopwatch();
        watch.Start();
        Int32 maxLoadingTime = Math.Max(5, (Int32)Mathf.Floor(LoadingTimeAllocatedPerFrame / FPSManager.GetEstimatedFps()));
        while (!SFXData.lockLoading && watch.ElapsedMilliseconds < maxLoadingTime)
        {
            if (LoadCur == null)
            {
                if (loadingQueue.Count == 0)
                    break;
                LoadCur = loadingQueue.Dequeue();
                if (LoadCur.cancel)
                {
                    LoadCur.loadHasEnded = true;
                    LoadCur = null;
                    continue;
                }
                if (!LoadCur.useCamera)
                    LoadCur.LoadSFXRawInit();
            }
            if (LoadCur.useCamera)
            {
                // An SFX loaded must be played at least once...
                // An SFX loaded shouldn't wait for another SFX to load before playing, except if UseCamera is set to false
                if (SFXDataMesh.Raw.RenderingCount > 0 || SFXData.sfxLoadedNotPlayed.Count > 0)
                    break;
                LoadCur.mesh = SFXDataMesh.Runtime.SetupRuntimeMesh(LoadCur.id, LoadCur.cmdRef, LoadCur.sfxRequest, out LoadCur.firstMeshFrame);
                LoadCur.loadHasEnded = true;
                SFXData.lockLoading = true;
                SFXData.sfxLoadedNotPlayed.Add(LoadCur);
            }
            else if (LoadCur.LoadSFXRawLoop())
            {
                LoadCur.LoadSFXRawEnd();
                LoadCur.loadHasEnded = true;
                SFXData.sfxLoadedNotPlayed.Add(LoadCur);
                LoadCur = null;
            }
        }
    }

    public static void AdvanceEventSFXFrame()
    {
        try
        {
            foreach (SFXData sfx in SFXData.EventSFX.Values)
                foreach (SFXData.RunningInstance run in sfx.runningSFX)
                    run.frame++;
            SFXData.LoadLoop();
        }
        catch (Exception err)
        {
            Log.Error(err);
        }
    }

    public static void RenderEventSFX()
    {
        try
        {
            foreach (SFXData sfx in SFXData.EventSFX.Values)
            {
                for (Int32 i = 0; i < sfx.runningSFX.Count; i++)
                {
                    SFXData.RunningInstance run = sfx.runningSFX[i];
                    if (sfx.mesh.Render(run.frame, run))
                    {
                        sfx.runningSFX.RemoveAt(i);
                        if (sfx.runningSFX.Count == 0)
                            sfx.mesh.End();
                        i--;
                    }
                }
            }
        }
        catch (Exception err)
        {
            Log.Error(err);
        }
    }

    public void PlaySFX(Int32 frameStart, List<UInt32> preventMeshByKey = null, List<UInt32> preventMeshByIndex = null, Dictionary<UInt32, Color> colorsByKey = null, Dictionary<UInt32, Color> colorsByIndex = null)
    {
        SFXData.sfxLoadedNotPlayed.Remove(this);
        if (cancel)
            return;
        if (preventMeshByKey == null)
            preventMeshByKey = new List<UInt32>();
        if (preventMeshByIndex == null)
            preventMeshByIndex = new List<UInt32>();
        if (colorsByKey == null)
            colorsByKey = new Dictionary<UInt32, Color>();
        if (colorsByIndex == null)
            colorsByIndex = new Dictionary<UInt32, Color>();
        mesh.SetupPositions(sfxRequest.exe, sfxRequest.trgno == 1 ? sfxRequest.trg[0] : null, new Vector3(sfxRequest.trgcpos.vx, sfxRequest.trgcpos.vy, sfxRequest.trgcpos.vz));
        if (runningSFX.Count == 0)
            mesh.Begin();
        frameStart += firstMeshFrame;
        runningSFX.Add(new RunningInstance(frameStart, preventMeshByKey, preventMeshByIndex, colorsByKey, colorsByIndex));
    }

    public void LoadSFX(SpecialEffect effNum, CMD_DATA cmd, BTL_VFX_REQ request, Boolean applyCamera)
    {
        id = effNum;
        cmdRef = cmd;
        sfxRequest = request;
        useCamera = applyCamera;
        firstMeshFrame = -1;
        loadHasEnded = false;
        isEventSFX = false;
        cancel = false;
        sfxthread = new List<BattleActionThread>();
        camera = new List<SFXDataCamera>();
        mesh = null;
        runningSFX = new List<RunningInstance>();
        String defaultFolder = DataResources.PureDataDirectory + $"SpecialEffects/ef{(Int32)effNum:D3}/";
        String sfxInfo = AssetManager.LoadString(defaultFolder + UnifiedBattleSequencer.INFORMATION_FILE, true);
        if (sfxInfo != null)
            LoadSFXFromInfo(sfxInfo, defaultFolder);
        LoadSequenceFromFile(defaultFolder + UnifiedBattleSequencer.SEQUENCE_FILE);
        if (mesh != null)
        {
            loadHasEnded = true;
            return;
        }
        loadingQueue.Enqueue(this);
    }

    public void LoadEventSFX(Int32 eventId, SpecialEffect effNum, BTL_VFX_REQ request)
    {
        if (SFXData.EventSFX.TryGetValue(eventId, out SFXData oldSFX))
        {
            oldSFX.Cancel();
            if (oldSFX.runningSFX.Count > 0)
            {
                oldSFX.runningSFX.Clear();
                oldSFX.mesh.End();
            }
        }
        id = effNum;
        cmdRef = null;
        sfxRequest = request;
        useCamera = false;
        firstMeshFrame = -1;
        loadHasEnded = false;
        isEventSFX = true;
        cancel = false;
        sfxthread = new List<BattleActionThread>();
        camera = new List<SFXDataCamera>();
        mesh = null;
        runningSFX = new List<RunningInstance>();
        String defaultFolder = DataResources.PureDataDirectory + $"SpecialEffects/ef{(Int32)effNum:D3}/";
        String sfxInfo = AssetManager.LoadString(defaultFolder + UnifiedBattleSequencer.INFORMATION_FILE, true);
        if (sfxInfo != null)
            LoadSFXFromInfo(sfxInfo, defaultFolder);
        LoadSequenceFromFile(defaultFolder + UnifiedBattleSequencer.SEQUENCE_FILE);
        if (mesh != null)
        {
            loadHasEnded = true;
            return;
        }
        SFXData.loadingQueue.Enqueue(this);
        SFXData.EventSFX[eventId] = this;
    }

    public void Cancel()
    {
        cancel = true;
        SFXData.sfxLoadedNotPlayed.Remove(this);
        foreach (RunningInstance inst in runningSFX)
            inst.cancel = true;
    }

    public Boolean IsCancelled()
    {
        return cancel;
    }

    private void LoadSequenceFromFile(String seqPath)
    {
        String sequenceText = AssetManager.LoadString(seqPath, true);
        if (sequenceText != null)
        {
            sfxthread = BattleActionThread.LoadFromTextSequence(sequenceText);
            foreach (BattleActionThread thread in sfxthread)
                thread.parentSFX = this;
        }
    }

    private void LoadSFXFromInfo(String sfxInfo, String defaultFolder)
    {
        try
        {
            String[] sfxInfoLines = sfxInfo.Split('\n');
            List<UInt32> keyList = new List<UInt32>();
            foreach (String linent in sfxInfoLines)
            {
                String line = linent.Trim();
                String[] arguments = line.Split(' ');
                if (arguments[0] == "Model" && arguments.Length >= 2)
                {
                    String meshPath = AssetManager.UsePathWithDefaultFolder(defaultFolder, arguments[1]);
                    SFXDataMesh.ModelSequence modelJSON = SFXDataMesh.ModelSequence.Load(meshPath);
                    if (modelJSON == null)
                        continue;
                    if (mesh == null)
                        mesh = new SFXDataMesh.JSON();
                    (mesh as SFXDataMesh.JSON).model.Add(modelJSON);
                    firstMeshFrame = 0;
                }
                if (arguments[0] == "Camera" && arguments.Length >= 2 && useCamera)
                {
                    String cameraPath = AssetManager.UsePathWithDefaultFolder(defaultFolder, arguments[1]);
                    SFXDataCamera camJSON = SFXDataCamera.LoadFromJSON(cameraPath);
                    if (camJSON == null)
                        continue;
                    camera.Add(camJSON);
                }
            }
        }
        catch (Exception err)
        {
            Log.Error(err);
        }
    }

    public void LoadSFXRawInit()
    {
        if (SFXData.BattleCallbackReaderExportSequence)
        {
            LoadThread = new List<BattleActionThread>();
            LoadThread.Add(new BattleActionThread());
            LoadThId = 0;
        }
        ShowCursor = 1;
        BbgIntensity = 128;
        SoundPitch = 4;
        TakeBackgroundCapture = -1;
        IsDisappear.Clear();
        ChannelingStep = 0;
        BtlPos.Clear();
        BtlRot.Clear();
        BtlScl.Clear();
        BtlFade.Clear();
        BbgFade.Clear();
        unsafe
        {
            SFX.hijackedCallback = SFXData.BattleCallbackReader;
        }
        SFXDataCamera.currentCameraEngine = SFXDataCamera.CameraEngine.NONE;
        sfxRequest.UpdateTargetAveragePosition();
        sfxRequest.PlaySFX(id);
        LoadMatList.Clear();
        mesh = new SFXDataMesh.Raw();
    }

    public Boolean LoadSFXRawLoop()
    {
        SFXDataMesh.Raw sfxRawMesh = LoadCur.mesh as SFXDataMesh.Raw;
        if (sfxRawMesh == null)
            return true;
        if (sfxRequest.exe.bi.player == 0 && SFX.frameIndex == 0 && (sfxRequest.flgs & 1) == 1)
            SFX.SetTaskMonsteraStart();
        if (SFXData.BattleCallbackReaderTrackBtlMovement)
        {
            Dictionary<UInt16, BtlDataTempSave> btlSave = new Dictionary<UInt16, BtlDataTempSave>();
            List<BTL_DATA> allBtl = new List<BTL_DATA>();
            for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
            {
                // Switch between actual datas and datas modified by the battle callback, for accuracy
                allBtl.Add(btl);
                btlSave[btl.btl_id] = new BtlDataTempSave();
                btlSave[btl.btl_id].Store(btl);
                if (LoadBtlSave.ContainsKey(btl.btl_id))
                    LoadBtlSave[btl.btl_id].Restore(btl);
            }
            SFX.isRunning = SFX.SFX_Update(ref SFX.frameIndex);
            foreach (BTL_DATA btl in allBtl)
            {
                if (LoadBtlSave.ContainsKey(btl.btl_id))
                {
                    BtlDataTempSave sfxBtlSave = LoadBtlSave[btl.btl_id];
                    sfxBtlSave.Store(btl);
                    sfxBtlSave.animFrame++;
                    if (sfxBtlSave.animFrame > GeoAnim.geoAnimGetNumFrames(btl, sfxBtlSave.animName))
                        sfxBtlSave.animFrame = 0;
                }
                btlSave[btl.btl_id].Restore(btl);
            }
        }
        else
        {
            SFX.isRunning = SFX.SFX_Update(ref SFX.frameIndex);
        }
        if (!SFX.isRunning)
            return true;
        if (SFXData.BattleCallbackReaderExportSequence && SFX.frameIndex > 1 && ChannelingStep != 3)
            LoadThread[LoadThId].code.AddLast(new BattleActionCode("Wait", "Time", "1"));
        PSXTextureMgr.isCaptureBlur = true;
        if (id == SpecialEffect.Ark__Full)
        {
            if (SFX.frameIndex == 1004)
                SFX.subOrder = 2;
            if (SFX.frameIndex == 1193)
                SFX.subOrder = 0;
        }
        //SFX.SFX_UpdateCamera(0);
        SFX.SFX_LateUpdate();
        SFXRender.Update();
        PSXTextureMgr.isCreateGenTexture = true;
        Boolean meshIsRenderTexture = false;
        if (id == SpecialEffect.Boomerang && SFX.frameIndex == 34)
            firstMeshFrame = SFX.frameIndex - 1;
        if (id == SpecialEffect.Special_Necron_Engage && SFX.frameIndex == 26)
            firstMeshFrame = SFX.frameIndex - 1;
        if (SFXRender.commandBuffer.Count != 0 && firstMeshFrame < 0)
            firstMeshFrame = SFX.frameIndex - 1;
        for (Int32 i = 0; i < SFXRender.commandBuffer.Count; i++)
        {
            if (SFXRender.commandBuffer[i] is SFXMesh)
            {
                SFXMesh sfxmesh = SFXRender.commandBuffer[i] as SFXMesh;
                SFXDataMesh.EffectMaterial sfxmat;
                if (!LoadMatList.TryGetValue(sfxmesh._key, out sfxmat))
                {
                    sfxmat = new SFXDataMesh.EffectMaterial();
                    sfxmat.ConvertFromSFXMesh(sfxmesh);
                    LoadMatList[sfxmesh._key] = sfxmat;
                }
                Boolean isNecronSpecialTexture = id == SpecialEffect.Special_Necron_Death && sfxmat.textureParam.w < 0;
                SFXDataMesh.Raw.RMesh frameMesh;
                if (meshIsRenderTexture)
                {
                    Int32 genMeshIndex = sfxRawMesh.genTextureMesh.FindIndex(p => p.Key == sfxmat && !p.Value.raw.ContainsKey(SFX.frameIndex));
                    if (genMeshIndex >= 0)
                    {
                        frameMesh = sfxRawMesh.genTextureMesh[genMeshIndex].Value;
                    }
                    else
                    {
                        frameMesh = new SFXDataMesh.Raw.RMesh();
                        sfxRawMesh.genTextureMesh.Add(new KeyValuePair<SFXDataMesh.EffectMaterial, SFXDataMesh.Raw.RMesh>(sfxmat, frameMesh));
                    }
                }
                else if (!sfxRawMesh.data.TryGetValue(sfxmat, out frameMesh))
                {
                    frameMesh = new SFXDataMesh.Raw.RMesh();
                    sfxRawMesh.data[sfxmat] = frameMesh;
                }
                if (sfxmat.textureKind == PSXTextureMgr.Kind.IMAGE && !SFXScreenShot.IsSpecialSlowTexture(sfxmesh._key) && !isNecronSpecialTexture && !PSXTextureMgr.HasTextureKey(sfxmesh._key))
                    sfxmat.AddTextureChanger(SFX.frameIndex, PSXTextureMgr.Kind.IMAGE, "", UnityEngine.Object.Instantiate(PSXTextureMgr.GetTexture(sfxmesh._key).texture));
                else if (sfxmat.textureKind == PSXTextureMgr.Kind.BACKGROUND)
                    sfxmat.PushBackgroundCapture(TakeBackgroundCapture);
                else if (sfxmat.textureKind == PSXTextureMgr.Kind.SCREENSHOT)
                    sfxmat.PushScreenshot(SFX.frameIndex);
                frameMesh.ConvertFromSFXMesh(SFX.frameIndex, sfxmesh);
                if (sfxmat.textureKind == PSXTextureMgr.Kind.SCREENSHOT)
                    frameMesh.raw[SFX.frameIndex].renderPriority = 9;
                sfxRawMesh.firstFrame = Math.Min(sfxRawMesh.firstFrame, SFX.frameIndex);
                sfxRawMesh.lastFrame = Math.Max(sfxRawMesh.lastFrame, SFX.frameIndex);
            }
            else if (SFXRender.commandBuffer[i] is SFXRenderTextureBegin || SFXRender.commandBuffer[i] is SFXRenderTextureEnd)
            {
                meshIsRenderTexture = SFXRender.commandBuffer[i] is SFXRenderTextureBegin;
            }
            else if (SFXRender.commandBuffer[i] is SFXMoveImage)
            {
                SFXMoveImage sfximgmove = SFXRender.commandBuffer[i] as SFXMoveImage;
                PSXTextureMgr.MoveImage(sfximgmove.rx, sfximgmove.ry, sfximgmove.rw, sfximgmove.rh, sfximgmove.x, sfximgmove.y);
            }
        }
        return false;
    }

    public void LoadSFXRawEnd()
    {
        if (SFXData.BattleCallbackReaderExportSequence && cmdRef != null)
        {
            List<BattleActionThread> reworkedThreads = new List<BattleActionThread>();
            Single BbgIntensityFactor = 1f;
            for (Int32 i = 0; i < LoadThread.Count; i++)
            {
                BattleActionThread rwThread = new BattleActionThread();
                reworkedThreads.Add(rwThread);
                Int32 waitCount = 0;
                Int32 frameCount = 0;
                while (LoadThread[i].code.Count > 0)
                {
                    BattleActionCode brutCode = LoadThread[i].code.First.Value;
                    LoadThread[i].code.RemoveFirst();
                    if (brutCode.operation == "Wait")
                    {
                        waitCount++;
                        frameCount++;
                    }
                    else
                    {
                        if (waitCount > 0)
                        {
                            rwThread.code.AddLast(new BattleActionCode("Wait", "Time", waitCount.ToString()));
                            waitCount = 0;
                        }
                        rwThread.code.AddLast(brutCode);
                        if (brutCode.argument.ContainsKey("Char"))
                        {
                            UInt16 charId;
                            UInt16.TryParse(brutCode.argument["Char"], out charId);
                            while (LoadThread[i].code.Count > 0 && LoadThread[i].code.First.Value.operation == brutCode.operation)
                            {
                                BattleActionCode groupCode = LoadThread[i].code.First.Value;
                                LoadThread[i].code.RemoveFirst();
                                UInt16 otherCharId;
                                UInt16.TryParse(groupCode.argument["Char"], out otherCharId);
                                charId |= otherCharId;
                            }
                            ChangeCharacterArgument(brutCode, charId);
                        }
                    }
                    if (waitCount > 0 && (BtlPos.ContainsKey(frameCount) || BtlRot.ContainsKey(frameCount) || BtlScl.ContainsKey(frameCount) || BtlFade.ContainsKey(frameCount) || BbgFade.ContainsKey(frameCount)))
                    {
                        rwThread.code.AddLast(new BattleActionCode("Wait", "Time", waitCount.ToString()));
                        waitCount = 0;
                    }
                    foreach (Dictionary<Int32, BtlPosWatcher> watcher in new Dictionary<Int32, BtlPosWatcher>[] { BtlPos, BtlRot, BtlScl, BtlFade })
                        if (watcher.ContainsKey(frameCount))
                        {
                            for (Int32 j = 0; j < watcher[frameCount].pos.Count; j++)
                            {
                                UInt16 curBtlid = watcher[frameCount].btlid[j];
                                Int32 opTime = 1;
                                Vector3 opEnd = watcher[frameCount].pos[j];
                                //while (watcher.ContainsKey(frameCount + opTime) && watcher[frameCount + opTime].btlid.Contains(curBtlid))
                                //{
                                //    BtlPosWatcher curWatch = watcher[frameCount + opTime];
                                //    Int32 k = curWatch.btlid.FindIndex(btlid => btlid == curBtlid);
                                //    opEnd = curWatch.pos[k];
                                //    curWatch.pos.RemoveAt(k);
                                //    curWatch.btlid.RemoveAt(k);
                                //    if (curWatch.btlid.Count == 0)
                                //        watcher.Remove(frameCount + opTime);
                                //    opTime++;
                                //}
                                Boolean relPos = false;
                                //if (watcher == BtlPos && watcher.ContainsKey(frameCount - 1) && watcher[frameCount - 1].btlid.Contains(curBtlid))
                                //{
                                //    BtlPosWatcher curWatch = watcher[frameCount - 1];
                                //    Int32 k = curWatch.btlid.FindIndex(btlid => btlid == curBtlid);
                                //    opEnd -= curWatch.pos[k];
                                //    relPos = true;
                                //}
                                BattleActionCode watchCode;
                                if (watcher == BtlPos)
                                    watchCode = new BattleActionCode("MoveToPosition", (relPos ? "RelativePosition" : "AbsolutePosition"), opEnd.ToString(), "Time", (opTime - 1).ToString(), "MoveHeight", true.ToString());
                                else if (watcher == BtlRot)
                                    watchCode = new BattleActionCode("Turn", "Angle", opEnd.ToString(), "Time", (opTime - 1).ToString());
                                else if (watcher == BtlScl)
                                    watchCode = new BattleActionCode("ChangeSize", "Size", opEnd.ToString(), "Time", (opTime - 1).ToString(), "ScaleShadow", true.ToString());
                                else
                                    watchCode = new BattleActionCode("ShowMesh", "Enable", (opEnd.x > 64).ToString(), "Time", (opTime - 1).ToString());
                                ChangeCharacterArgument(watchCode, curBtlid);
                                rwThread.code.AddLast(watchCode);
                            }
                            watcher.Remove(frameCount);
                        }
                    if (BbgFade.ContainsKey(frameCount))
                    {
                        Int32 opTime = 1;
                        Single opStart = BbgFade[frameCount];
                        Single opEnd = opStart;
                        BbgFade.Remove(frameCount);
                        while (BbgFade.ContainsKey(frameCount + opTime))
                        {
                            opEnd = BbgFade[frameCount + opTime];
                            BbgFade.Remove(frameCount + opTime);
                            opTime++;
                        }
                        if (opEnd != BbgIntensityFactor || opTime > 1)
                            rwThread.code.AddLast(new BattleActionCode("SetBackgroundIntensity", "Intensity", opEnd.ToString(), "Time", (opTime - 1).ToString()));
                        BbgIntensityFactor = opEnd;
                    }
                }
            }
            // Generate a sequence file
            Directory.CreateDirectory("SpecialEffects/ef" + ((Int32)id).ToString("D3"));
            if (IsShortSpecialEffect(id))
                File.WriteAllText("SpecialEffects/ef" + ((Int32)id).ToString("D3") + "/" + UnifiedBattleSequencer.PLAYER_SEQUENCE_FILE, $"// Player sequence of SFX {id}\n\nLoadSFX: SFX={id}\nPlayAnimation: Char=Caster ; Anim=MP_SET\nWaitAnimation: Char=Caster\nMoveToTarget: Char=Caster ; Target=AllTargets ; Dist=-2155 ; Anim=MP_RUN\nTurn: Char=Caster ; BaseAngle=AllTargets ; Time=4\nWaitMove: Char=Caster\nMoveToTarget: Char=Caster ; Target=AllTargets ; Dist=-1885 ; Anim=MP_RUN_TO_ATTACK\nWaitMove: Char=Caster\nStartThread\n\tWaitSFXLoaded: SFX={id}\n\tPlaySFX: SFX={id}\n\tWaitSFXDone: SFX={id}\nEndThread\nPlayAnimation: Char=Caster ; Anim=MP_ATTACK\nPlaySound: Sound=WeaponAttack\nPlaySound: Sound=WeaponHit\nWaitAnimation: Char=Caster\nMoveToPosition: Char=Caster ; AbsPos=Default ; Anim=MP_BACK\nTurn: Char=Caster ; BaseAngle=Default ; Time=2\nWaitMove: Caster\nPlayAnimation: Char=Caster ; Anim=MP_ATK_TO_NORMAL\nWaitAnimation: Char=Caster\nPlayAnimation: Char=Caster ; Anim=MP_IDLE_NORMAL ; Loop=True\nWaitTurn: Char=Caster\n");
            else
                File.WriteAllText("SpecialEffects/ef" + ((Int32)id).ToString("D3") + "/" + UnifiedBattleSequencer.PLAYER_SEQUENCE_FILE, $"// Player sequence of SFX {id}\n\n" + BattleActionThread.GetSequenceStringCode(reworkedThreads));
        }
        SFX.currentEffectID = SpecialEffect.Special_No_Effect;
        SFX.hijackedCallback = null;
    }

    private void ChangeCharacterArgument(BattleActionCode baCode, UInt16 charId)
    {
        Boolean includeCaster = (charId & cmdRef.regist.btl_id) != 0;
        Boolean includePlayers = (charId & 0xF) == 0xF;
        Boolean includeATarget = (charId & cmdRef.tar_id) != 0;
        if (includePlayers && includeATarget)
            baCode.argument["Char"] = "Everyone";
        else if (includePlayers)
            baCode.argument["Char"] = "AllPlayers";
        else if (includeATarget)
            baCode.argument["Char"] = "AllTargets";
        else if (includeCaster)
            baCode.argument["Char"] = "Caster";
        else
            baCode.argument["Char"] = charId.ToString();
    }

    public static List<BattleActionThread> LoadThread;
    public static Int32 LoadThId;
    private static Dictionary<UInt16, BtlDataTempSave> LoadBtlSave = new Dictionary<UInt16, BtlDataTempSave>();
    private static Dictionary<UInt32, SFXDataMesh.EffectMaterial> LoadMatList = new Dictionary<UInt32, SFXDataMesh.EffectMaterial>();
    private static Int32 ShowCursor;
    private static Int32 BbgIntensity;
    private static Int32 SoundPitch;
    private static Int32 TakeBackgroundCapture;
    private static HashSet<UInt16> IsDisappear = new HashSet<UInt16>();
    private static Int32 ChannelingStep;

    private static Dictionary<Int32, BtlPosWatcher> BtlPos = new Dictionary<Int32, BtlPosWatcher>();
    private static Dictionary<Int32, BtlPosWatcher> BtlRot = new Dictionary<Int32, BtlPosWatcher>();
    private static Dictionary<Int32, BtlPosWatcher> BtlScl = new Dictionary<Int32, BtlPosWatcher>();
    private static Dictionary<Int32, BtlPosWatcher> BtlFade = new Dictionary<Int32, BtlPosWatcher>();
    private static Dictionary<Int32, Single> BbgFade = new Dictionary<Int32, Single>();

    private class BtlDataTempSave
    {
        public Vector3 pos;
        public Quaternion rot;
        public Int32 scale_x;
        public Int32 scale_y;
        public Int32 scale_z;
        public UInt16 flags;
        public String animName;
        public Byte animFrame;

        public void Store(BTL_DATA btl)
        {
            pos = btl.pos;
            rot = btl.rot;
            scale_x = btl.geo_scale_x;
            scale_y = btl.geo_scale_y;
            scale_z = btl.geo_scale_z;
            flags = btl.flags;
            animName = btl.currentAnimationName;
            animFrame = btl.evt.animFrame;
        }

        public void Restore(BTL_DATA btl)
        {
            btl.pos = pos;
            btl.rot = rot;
            btl.geo_scale_x = scale_x;
            btl.geo_scale_y = scale_y;
            btl.geo_scale_z = scale_z;
            btl.flags = flags;
            btl.currentAnimationName = animName;
            btl.evt.animFrame = animFrame;
        }
    }

    private class BtlPosWatcher
    {
        public List<UInt16> btlid = new List<UInt16>();
        public List<Vector3> pos = new List<Vector3>();
    }

    [MonoPInvokeCallback(typeof(SFX.Callback))]
    public static unsafe Int32 BattleCallbackReader(Int32 fullCode, Int32 arg0, Int32 arg1, Int32 arg2, Int32 arg3, void* pv)
    {
        Int32* p = (Int32*)pv;
        Int16* ps = (Int16*)pv;
        Int32 code = fullCode >> 24;
        Int32 btlid = fullCode & 255;
        if (SFX.isDebugPrintCode)
            Log.Message("[SFXData] Callback " + (SFX.COMMAND)code + " " + arg0 + " " + arg1 + " " + arg2 + " " + arg3 + " " + btlid);
        if (SFXData.BattleCallbackReaderExportSequence && (ChannelingStep == 1 || ChannelingStep == 3))
        {
            if (ChannelingStep == 1 && code == 12 && (BattlePlayerCharacter.PlayerMotionIndex)arg0 == BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD)
                ChannelingStep = 2;
            if (code == 113 || code == 2 || code == 4 || code == 12)
                return 0;
        }
        switch (code)
        {
            case 100: // Load the rectangle [x, y, w, h] = [arg0, arg1, arg2, arg3] from a PSX-like Vram (TIM format)
                PSXTextureMgr.LoadImage(arg0, arg1, arg2, arg3, (UInt16*)p);
                return 0;
            case 32: // Play/Stop Sound
                if (SFXData.BattleCallbackReaderExportSequence)
                {
                    switch (arg0)
                    {
                        case 0:
                            SFX.SoundPlay(arg1, arg2, arg3);
                            break;
                        case 1:
                            SFX.SoundPlayChant(arg1, arg2, arg3);
                            break;
                        case 2:
                            SFX.SoundStop(arg1, arg2);
                            break;
                        case 3:
                            SFX.StreamPlay(arg1);
                            break;
                    }
                }
                return 0;
            case 101: // Pass the Vram rectangle back to FF9SpecialEffectPlugin.dll
                PSXTextureMgr.StoreImage(arg0, arg1, arg2, arg3, (UInt16*)p);
                return 0;
            case 102: // Update the rectangle [arg0, arg1, p[2], p[3]] with the image of the rectangle [p[0], p[1], p[2], p[3]]
                PSXTextureMgr.MoveImage(arg0, arg1, (Int16*)p);
                return 0;
            case 110: // btl_seq controls the end of battles (game over, victory animation, fleeing...)
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("SetVariable", "Variable", "btl_seq", "Value", "+1"));
                return 0;
            case 111: // "Battle Camera: Auto / Fixed"
                return (Int32)FF9StateSystem.Settings.cfg.camera;
            case 112: // Some flag used in AI scripts (Steiner's Moonlight Slash, Kuja's Transform, Trance Kuja's Ultima (Crystal) and Necron's death)
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("SetVariable", "Variable", "gEventGlobal", "Index", "199", "Value", "|16"));
                return 0;
            case 113: // Display Ability Casting Name
            {
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("Message", "Text", "[CastName]", "Priority", "1", "Title", true.ToString(), "Reflect", true.ToString()));
                return 0;
            }
            case 114: // Show/Hide Cursor
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("SetVariable", "Variable", "cmd_status", "Value", arg0 != 0 ? "|2" : "&65533", "Reflect", true.ToString()));
                ShowCursor = arg0 != 0 ? 1 : 0;
                return 0;
            case 115: // Is Cursor Shown
                return ShowCursor;
            case 116:
                return 0;
            case 117: // Get Background Intensity, for fading backgrounds (also used to fake a light dim)
                return BbgIntensity;
            case 118: // Set Background Intensity
                if (SFXData.BattleCallbackReaderExportSequence)
                    BbgFade[SFX.frameIndex] = (Single)arg0 / 128f;
                BbgIntensity = arg0;
                return 0;
            case 119: // Controller Vibration
                return 0;
            case 120: // Take a screenshot in the Vram
                Marshal.Copy((IntPtr)p, PSXTextureMgr.bgParam, 0, 7);
                PSXTextureMgr.bgKey = (UInt32)(PSXTextureMgr.bgParam[1] >> 8 << 20 | PSXTextureMgr.bgParam[0] >> 6 << 16);
                TakeBackgroundCapture = SFX.frameIndex;
                return 0;
            case 121: // Return btl_id of player characters that were not removed from the battle (with Snort / Swallow)
                return battle.btl_bonus.member_flag;
            case 122: // Back Attack, Preemptive, Normal
                return (Byte)FF9StateSystem.Battle.FF9Battle.btl_scene.Info.StartType;
            case 123: // Return the number of targetable player characters
            {
                Int32 validPlayerTarget = 0;
                for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
                    if (next.bi.player != 0 && !btl_stat.CheckStatus(next, BattleStatus.Death | BattleStatus.Jump))
                        validPlayerTarget++;
                return validPlayerTarget;
            }
            case 124: // Return the current battle camera index (these cameras are predefined per BTL_SCENE)
                return FF9StateSystem.Battle.FF9Battle.seq_work_set.CameraNo;
            case 125: // Set Sound Pitch (default is 4 for a pitch of 1f)
                SoundPitch = arg0;
                return 0;
        }
        BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next;
        while (btl != null)
        {
            if (btl.btl_id == btlid)
                break;
            btl = btl.next;
        }
        if (btl == null)
            return (code != 9) ? 0 : 1;
        switch (code)
        {
            case 1: // Get Position
                switch (arg0)
                {
                    case 0: // Current 3D
                        *(Int16*)p = (Int16)btl.pos.x;
                        *(Int16*)((Byte*)p + 2) = (Int16)(-btl.pos.y);
                        *(Int16*)((Byte*)p + 4) = (Int16)btl.pos.z;
                        break;
                    case 1: // Base pos 3D
                        *(Int16*)p = (Int16)btl.base_pos.x;
                        *(Int16*)((Byte*)p + 2) = (Int16)(-btl.base_pos.y);
                        *(Int16*)((Byte*)p + 4) = (Int16)btl.base_pos.z;
                        break;
                    case 2: // Current 2D
                        *(Int16*)p = (Int16)btl.pos.x;
                        *(Int16*)((Byte*)p + 4) = (Int16)btl.pos.z;
                        break;
                    case 3: // Base pos 2D
                        *(Int16*)p = (Int16)btl.base_pos.x;
                        *(Int16*)((Byte*)p + 4) = (Int16)btl.base_pos.z;
                        break;
                }
                break;
            case 2: // Set Position
                if (SFXData.BattleCallbackReaderTrackBtlMovement)
                    btl.pos = new Vector3(ps[0], -ps[1], ps[2]);
                if (SFXData.BattleCallbackReaderExportSequence)
                {
                    if (!BtlPos.ContainsKey(SFX.frameIndex))
                        BtlPos[SFX.frameIndex] = new BtlPosWatcher();
                    BtlPos[SFX.frameIndex].pos.Add(new Vector3(ps[0], -ps[1], ps[2]));
                    BtlPos[SFX.frameIndex].btlid.Add((UInt16)btlid);
                }
                break;
            case 3: // Get Angles
                switch (arg0)
                {
                    case 0: // Difference between base and current
                        *(Int16*)p = (Int16)((btl.evt.rotBattle.eulerAngles.x - btl.rot.eulerAngles.x) * 11.3777781f);
                        *(Int16*)((Byte*)p + 2) = (Int16)((btl.evt.rotBattle.eulerAngles.y - btl.rot.eulerAngles.y) * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((btl.evt.rotBattle.eulerAngles.z - btl.rot.eulerAngles.z) * 11.3777781f);
                        break;
                    case 1: // Current
                        *(Int16*)p = (Int16)(btl.rot.eulerAngles.x * 11.3777781f);
                        *(Int16*)((Byte*)p + 2) = (Int16)(btl.rot.eulerAngles.y * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((btl.rot.eulerAngles.z - 180f) * 11.3777781f);
                        break;
                    case 2: // Base
                        *(Int16*)p = (Int16)(btl.evt.rotBattle.eulerAngles.x * 11.3777781f);
                        *(Int16*)((Byte*)p + 2) = (Int16)(btl.evt.rotBattle.eulerAngles.y * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((btl.evt.rotBattle.eulerAngles.z - 180f) * 11.3777781f);
                        break;
                    case 3: // Base, no orientation (horizontal angle)
                        *(Int16*)p = (Int16)(btl.evt.rotBattle.eulerAngles.x * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((btl.evt.rotBattle.eulerAngles.z - 180f) * 11.3777781f);
                        break;
                }
                break;
            case 4: // Set Angles
                switch (arg0)
                {
                    case 0: // To base angle
                        if (SFXData.BattleCallbackReaderExportSequence)
                            LoadThread[LoadThId].code.AddLast(new BattleActionCode("Turn", "Char", btlid.ToString(), "BaseAngle", "Default"));
                        break;
                    case 1: // To base angle + 180 degree
                        if (SFXData.BattleCallbackReaderExportSequence)
                            LoadThread[LoadThId].code.AddLast(new BattleActionCode("Turn", "Char", btlid.ToString(), "BaseAngle", "Default", "Angle", "180"));
                        break;
                    case 2: // To value
                        if (SFXData.BattleCallbackReaderTrackBtlMovement)
                            btl.rot.eulerAngles = new Vector3(ps[0] * 0.087890625f, ps[1] * 0.087890625f - 180f, ps[2] * 0.087890625f + 180f);
                        if (SFXData.BattleCallbackReaderExportSequence)
                        {
                            if (!BtlRot.ContainsKey(SFX.frameIndex))
                                BtlRot[SFX.frameIndex] = new BtlPosWatcher();
                            BtlRot[SFX.frameIndex].pos.Add(new Vector3(ps[0] * 0.087890625f, ps[1] * 0.087890625f - 180f, ps[2] * 0.087890625f + 180f));
                            BtlRot[SFX.frameIndex].btlid.Add((UInt16)btlid);
                        }
                        break;
                }
                break;
            case 5: // Get Size
                *(Int32*)p = btl.geo_scale_x;
                *(Int32*)((Byte*)p + 4) = btl.geo_scale_y;
                *(Int32*)((Byte*)p + 8) = btl.geo_scale_z;
                break;
            case 6: // Set Size
                if ((arg0 & 128) == 0 || (arg0 & 1) == 1)
                {
                    if (SFXData.BattleCallbackReaderExportSequence)
                    {
                        if (!BtlScl.ContainsKey(SFX.frameIndex))
                            BtlScl[SFX.frameIndex] = new BtlPosWatcher();
                        BtlScl[SFX.frameIndex].btlid.Add((UInt16)btlid);
                        if ((arg0 & 128) == 0)
                            BtlScl[SFX.frameIndex].pos.Add(new Vector3(1f, 1f, 1f));
                        if ((arg0 & 1) == 1)
                            BtlScl[SFX.frameIndex].pos.Add(new Vector3(arg1 / 4096f, arg2 / 4096f, arg3 / 4096f));
                    }
                    if (SFXData.BattleCallbackReaderTrackBtlMovement)
                    {
                        btl.flags |= geo.GEO_FLAGS_SCALE;
                        btl.geo_scale_x = arg1;
                        btl.geo_scale_y = arg2;
                        btl.geo_scale_z = arg3;
                    }
                }
                break;
            case 7: // Get geo flags
                return btl.flags;
            case 8: // Is using Auto-Potion
                return 0;
            case 9: // Get Current Animation's frame count
                if (SFXData.BattleCallbackReaderTrackBtlMovement)
                {
                    UInt16 frameCount = GeoAnim.geoAnimGetNumFrames(btl);
                    if (frameCount == 0)
                        frameCount = 1;
                    return frameCount;
                }
                return 1;
            case 10: // Get Current Animation's current frame
                if (SFXData.BattleCallbackReaderTrackBtlMovement)
                    return btl.evt.animFrame;
                return 1;
            case 11: // Set Current Animation's current frame
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("PlayAnimation", "Char", btlid.ToString(), "Frame", arg0.ToString()));
                if (SFXData.BattleCallbackReaderTrackBtlMovement)
                {
                    UInt16 frameMax = GeoAnim.geoAnimGetNumFrames(btl);
                    btl.evt.animFrame = frameMax <= arg0 ? (Byte)(frameMax - 1) : (Byte)arg0;
                }
                break;
            case 12: // Set Current Animation
                if (SFXData.BattleCallbackReaderExportSequence)
                {
                    if (arg0 == -1)
                        LoadThread[LoadThId].code.AddLast(new BattleActionCode("PlayAnimation", "Char", btlid.ToString(), "Anim", "Idle"));
                    else
                    {
                        BattlePlayerCharacter.PlayerMotionIndex animCode = (BattlePlayerCharacter.PlayerMotionIndex)arg0;
                        if (ChannelingStep == 0 && animCode == BattlePlayerCharacter.PlayerMotionIndex.MP_STEP_FORWARD)
                        {
                            ChannelingStep = 1;
                            LoadThread.Add(new BattleActionThread());
                            LoadThread.Add(new BattleActionThread());
                            LoadThread[0].code.AddLast(new BattleActionCode("RunThread", "Thread", "1", "Condition", "CasterRow == 0 && AreCasterAndTargetEnemies", "Sync", true.ToString()));
                            LoadThread[1].code.AddLast(new BattleActionCode("MoveToPosition", "Char", btlid.ToString(), "RelativePosition", "(0, 0, 400)", "Anim", "MP_STEP_FORWARD"));
                            LoadThread[1].code.AddLast(new BattleActionCode("WaitMove", "Char", btlid.ToString()));
                            LoadThread[0].code.AddLast(new BattleActionCode("RunThread", "Thread", "2", "Condition", "IsSingleTarget"));
                            LoadThread[2].code.AddLast(new BattleActionCode("Turn", "Char", btlid.ToString(), "BaseAngle", "AllTargets", "Time", "5"));
                            LoadThread[0].code.AddLast(new BattleActionCode("LoadSFX", "SFX", SFX.currentEffectID.ToString(), "Reflect", true.ToString()));
                            LoadThread[0].code.AddLast(new BattleActionCode("PlayAnimation", "Char", btlid.ToString(), "Anim", "MP_IDLE_TO_CHANT"));
                            LoadThread[0].code.AddLast(new BattleActionCode("WaitAnimation", "Char", btlid.ToString()));
                            LoadThread[0].code.AddLast(new BattleActionCode("PlayAnimation", "Char", btlid.ToString(), "Anim", "MP_CHANT", "Loop", true.ToString()));
                            LoadThread[0].code.AddLast(new BattleActionCode("Channel", "Type", "Spell"));
                            LoadThread[0].code.AddLast(new BattleActionCode("Message", "Text", "[CastName]", "Priority", "1", "Title", true.ToString(), "Reflect", true.ToString()));
                            LoadThread[0].code.AddLast(new BattleActionCode("WaitSFXLoaded", "SFX", SFX.currentEffectID.ToString(), "Reflect", true.ToString()));
                            LoadThread[0].code.AddLast(new BattleActionCode("WaitAnimation", "Char", btlid.ToString()));
                            LoadThread[0].code.AddLast(new BattleActionCode("StopChannel"));
                            LoadThread[0].code.AddLast(new BattleActionCode("PlayAnimation", "Char", btlid.ToString(), "Anim", "MP_MAGIC"));
                            LoadThread[0].code.AddLast(new BattleActionCode("WaitAnimation", "Char", btlid.ToString()));
                            LoadThread[0].code.AddLast(new BattleActionCode("PlaySFX", "SFX", SFX.currentEffectID.ToString(), "Reflect", true.ToString()));
                            LoadThread[0].code.AddLast(new BattleActionCode("WaitSFXDone", "SFX", SFX.currentEffectID.ToString(), "Reflect", true.ToString()));
                        }
                        else if (ChannelingStep == 2 && animCode == BattlePlayerCharacter.PlayerMotionIndex.MP_CMD_TO_NORMAL)
                        {
                            ChannelingStep = 3;
                            LoadThread.Add(new BattleActionThread());
                            LoadThread[0].code.AddLast(new BattleActionCode("ActivateReflect"));
                            LoadThread[0].code.AddLast(new BattleActionCode("WaitReflect"));
                            LoadThread[0].code.AddLast(new BattleActionCode("RunThread", "Thread", "3", "Condition", "CasterRow == 0 && AreCasterAndTargetEnemies", "Sync", true.ToString()));
                            LoadThread[3].code.AddLast(new BattleActionCode("MoveToPosition", "Char", btlid.ToString(), "RelativePosition", "(0, 0, -400)", "Anim", "MP_STEP_BACK"));
                            LoadThread[3].code.AddLast(new BattleActionCode("WaitMove", "Char", btlid.ToString()));
                            LoadThread[0].code.AddLast(new BattleActionCode("PlayAnimation", "Char", btlid.ToString(), "Anim", "MP_CMD_TO_NORMAL"));
                            LoadThread[0].code.AddLast(new BattleActionCode("Turn", "Char", btlid.ToString(), "BaseAngle", "Default", "Time", "5"));
                            LoadThread[0].code.AddLast(new BattleActionCode("WaitAnimation", "Char", btlid.ToString()));
                            LoadThread[0].code.AddLast(new BattleActionCode("PlayAnimation", "Char", btlid.ToString(), "Anim", "MP_IDLE_NORMAL", "Loop", true.ToString()));
                            LoadThread[0].code.AddLast(new BattleActionCode("WaitTurn", "Char", btlid.ToString()));
                        }
                        else
                        {
                            LoadThread[LoadThId].code.AddLast(new BattleActionCode("PlayAnimation", "Char", btlid.ToString(), "Anim", animCode.ToString()));
                        }
                    }
                }
                if (SFXData.BattleCallbackReaderTrackBtlMovement)
                    btl.evt.animFrame = 0;
                break;
            case 13: // Stop Animation
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("PlayAnimation", "Char", btlid.ToString(), "Speed", (arg0 != 0 ? 0f : 1f).ToString()));
                return arg0;
            case 14: // Get Bone Stance
                Matrix4x4 matrix4x;
                try
                {
                    if (SFXData.BattleCallbackReaderTrackBtlMovement)
                        btl_mot.PlayAnim(btl);
                    matrix4x = btl.gameObject.transform.GetChildByName("bone" + arg1.ToString("D3")).localToWorldMatrix;
                }
                catch (NullReferenceException)
                {
                    matrix4x = Matrix4x4.identity;
                }
                switch (arg0)
                {
                    case 0: // Get Bone Position
                        *(Int16*)p = (Int16)matrix4x.m03;
                        *(Int16*)((Byte*)p + 2) = (Int16)(-matrix4x.m13);
                        *(Int16*)((Byte*)p + 4) = (Int16)matrix4x.m23;
                        break;
                    case 1: // Get Bone Height
                        return (Int32)(-matrix4x.m13);
                    case 2: // Get Bone Orientation & Position
                        SFX.PSXMAT* matPtr = (SFX.PSXMAT*)p;
                        Int16* rptr = matPtr->r;
                        Int32* tptr = matPtr->t;
                        matPtr->pad = 0;
                        rptr[0] = (Int16)(matrix4x.m00 * -4096f);
                        rptr[1] = (Int16)(matrix4x.m01 * -4096f);
                        rptr[2] = (Int16)(matrix4x.m02 * 4096f);
                        rptr[3] = (Int16)(matrix4x.m10 * -4096f);
                        rptr[4] = (Int16)(matrix4x.m11 * 4096f);
                        rptr[5] = (Int16)(matrix4x.m12 * -4096f);
                        rptr[6] = (Int16)(matrix4x.m20 * 4096f);
                        rptr[7] = (Int16)(matrix4x.m21 * 4096f);
                        rptr[8] = (Int16)(matrix4x.m22 * 4096f);
                        tptr[0] = (Int32)matrix4x.m03;
                        tptr[1] = (Int32)(-matrix4x.m13);
                        tptr[2] = (Int32)matrix4x.m23;
                        break;
                }
                break;
            case 15: // Is Targetable
                return 1;
            case 16: // Reset Stand Animation
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("PlayAnimation", "Char", btlid.ToString(), "Anim", "Idle"));
                break;
            case 17: // Is Hidden (no model, no targeting)
                return IsDisappear.Contains((UInt16)btlid) ? 1 : 0;
            case 18: // Set Hidden On/Off
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("ShowMesh", "Char", btlid.ToString(), "Enable", (arg0 == 0).ToString(), "IsDisappear", true.ToString()));
                if (SFXData.BattleCallbackReaderTrackBtlMovement)
                {
                    if (arg0 == 0)
                        IsDisappear.Remove((UInt16)btlid);
                    else
                        IsDisappear.Add((UInt16)btlid);
                }
                break;
            case 19: // Show/Hide Weapon
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("ShowMesh", "Char", btlid.ToString(), "Mesh", "Weapon", "Enable", (arg0 != 0).ToString()));
                break;
            case 20: // Status
                return 0;
            case 21: // Has a main command queued (Ready)
                return 0;
            case 22: // Is attached to another enemy
                return 0;
            case 23: // Effect Point
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("EffectPoint", "Char", btlid.ToString(), "Type", "Effect"));
                break;
            case 24: // Figure Point
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("EffectPoint", "Char", btlid.ToString(), "Type", "Figure"));
                break;
            case 25: // Show/Hide mesh
                switch (arg0)
                {
                    case 0: // Hide all
                        if (SFXData.BattleCallbackReaderExportSequence)
                            LoadThread[LoadThId].code.AddLast(new BattleActionCode("ShowMesh", "Char", btlid.ToString(), "Mesh", "65535", "Enable", false.ToString()));
                        if (SFXData.BattleCallbackReaderTrackBtlMovement)
                            btl.flags &= (UInt16)~geo.GEO_FLAGS_RENDER;
                        break;
                    case 1: // Hide for Vanish
                        if (SFXData.BattleCallbackReaderExportSequence)
                            LoadThread[LoadThId].code.AddLast(new BattleActionCode("ShowMesh", "Char", btlid.ToString(), "Mesh", "Vanish", "Enable", false.ToString()));
                        break;
                    case 2: // Show all
                        if (SFXData.BattleCallbackReaderExportSequence)
                            LoadThread[LoadThId].code.AddLast(new BattleActionCode("ShowMesh", "Char", btlid.ToString(), "Mesh", "65535", "Enable", true.ToString()));
                        if (SFXData.BattleCallbackReaderTrackBtlMovement)
                            btl.flags |= geo.GEO_FLAGS_RENDER;
                        break;
                    case 3: // Semi-transparent fade, light
                    case 4: // Semi-transparent fade, severe
                        if (SFXData.BattleCallbackReaderExportSequence)
                        {
                            if (!BtlFade.ContainsKey(SFX.frameIndex))
                                BtlFade[SFX.frameIndex] = new BtlPosWatcher();
                            BtlFade[SFX.frameIndex].btlid.Add((UInt16)btlid);
                            BtlFade[SFX.frameIndex].pos.Add(new Vector3(arg1, arg0, 0f));
                        }
                        break;
                }
                break;
            case 26: // Number of meshes
                return btl.meshCount;
            case 27: // Update color
                break;
            case 28: // Play/Stop Texture Animation
                break;
            case 29: // Play Sound (Jump)
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("PlaySound", "Sound", "1110"));
                break;
            case 30: // Play Weapon Sound (see FF9Snd.ff9battleSoundWeaponSndEffect02, first entry)
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("PlaySound", "Sound", btlsnd.ff9btlsnd_weapon_sfx(btl.bi.line_no, FF9BatteSoundWeaponSndEffectType.FF9BTLSND_WEAPONSNDEFFECTTYPE_ATTACK).ToString()));
                break;
            case 31: // Play Weapon Sound (see FF9Snd.ff9battleSoundWeaponSndEffect02, second entry)
                if (SFXData.BattleCallbackReaderExportSequence)
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("PlaySound", "Sound", btlsnd.ff9btlsnd_weapon_sfx(btl.bi.line_no, FF9BatteSoundWeaponSndEffectType.FF9BTLSND_WEAPONSNDEFFECTTYPE_HIT).ToString()));
                break;
            case 33: // Play Casting Animation (Dragon)
                if (SFXData.BattleCallbackReaderExportSequence)
                {
                    // Freya's Dragon casting animation (looping or launch)
                    switch (arg1)
                    {
                        case 61: // Luna
                            arg0 = ((arg0 != 20) ? 1 : 0);
                            break;
                        case 83: // White Draw
                            arg0 = ((arg0 != 9) ? 1 : 0);
                            break;
                        case 168: // Reis' Wind
                            arg0 = ((arg0 != 11) ? 1 : 0);
                            break;
                        case 296: // Dragon Breath
                            arg0 = ((arg0 != 19) ? 1 : 0);
                            break;
                        case 387: // Cherry Blossom
                            arg0 = ((arg0 != 18) ? 1 : 0);
                            break;
                        case 490: // Dragon's Crest
                            arg0 = ((arg0 != 14) ? 1 : 0);
                            break;
                        case 491: // Six Dragons
                            arg0 = ((arg0 != 18) ? 1 : 0);
                            break;
                    }
                    LoadThread[LoadThId].code.AddLast(new BattleActionCode("PlayAnimation", "Char", btlid.ToString(), "Anim", arg0 != 0 ? "ANH_MAIN_B0_011_202" : "ANH_MAIN_B0_011_201"));
                }
                if (SFXData.BattleCallbackReaderTrackBtlMovement)
                    btl.evt.animFrame = 0;
                break;
        }
        return 0;
    }

    [MonoPInvokeCallback(typeof(SFX.Callback))]
    public static unsafe Int32 BattleCallbackDummy(Int32 fullCode, Int32 arg0, Int32 arg1, Int32 arg2, Int32 arg3, void* pv)
    {
        Int32* p = (Int32*)pv;
        Int16* ps = (Int16*)pv;
        Int32 code = fullCode >> 24;
        Int32 btlid = fullCode & 255;
        if (SFX.isDebugPrintCode)
            Log.Message("[SFXData] Callback " + (SFX.COMMAND)code + " " + arg0 + " " + arg1 + " " + arg2 + " " + arg3 + " " + btlid);
        switch (code)
        {
            case 32: // Play/Stop Sound
            case 110: // btl_seq controls the end of battles (game over, victory animation, fleeing...)
            case 112: // Some flag used in AI scripts (Steiner's Moonlight Slash, Kuja's Transform, Trance Kuja's Ultima (Crystal) and Necron's death)
            case 113: // Display Ability Casting Name
            case 114: // Show/Hide Cursor
            case 116: // Show/Hide Cursor
            case 118: // Set Background Intensity
            case 125: // Set Sound Pitch (default is 4 for a pitch of 1f)
                return 0;
            case 100: // Load the rectangle [x, y, w, h] = [arg0, arg1, arg2, arg3] from a PSX-like Vram (TIM format)
                if (SFXData.BattleCallbackDummyLoadImages)
                    PSXTextureMgr.LoadImage(arg0, arg1, arg2, arg3, (UInt16*)p);
                return 0;
            case 101: // Pass the Vram rectangle back to FF9SpecialEffectPlugin.dll
                PSXTextureMgr.StoreImage(arg0, arg1, arg2, arg3, (UInt16*)p);
                return 0;
            case 102: // Update the rectangle [arg0, arg1, p[2], p[3]] with the image of the rectangle [p[0], p[1], p[2], p[3]]
                if (SFXData.BattleCallbackDummyLoadImages)
                    PSXTextureMgr.MoveImage(arg0, arg1, (Int16*)p);
                return 0;
            case 111: // "Battle Camera: Auto / Fixed"
                return (Int32)FF9StateSystem.Settings.cfg.camera;
            case 115: // Is Cursor Shown
                return ((FF9StateSystem.Battle.FF9Battle.cmd_status & 2) == 0) ? 0 : 1;
            case 117: // Get Background Intensity, for fading backgrounds (also used to fake a light dim)
                return battlebg.nf_GetBbgIntensity();
            case 119: // Controller Vibration
                if (SFXData.BattleCallbackDummyUpdateVib)
                {
                    switch (arg0)
                    {
                        case 0: // Stop Vibration
                            vib.VIB_purge();
                            break;
                        case 1: // Vibrate (full parameters)
                        {
                            Byte[] array = new Byte[1800];
                            Marshal.Copy((IntPtr)p, array, 0, 1800);
                            MemoryStream input = new MemoryStream(array);
                            BinaryReader binaryReader = new BinaryReader(input);
                            vib.VIB_init(binaryReader);
                            vib.VIB_setTrackActive(0, vib.VIB_SAMPLE_LO, true);
                            vib.VIB_setTrackActive(0, vib.VIB_SAMPLE_HI, true);
                            vib.VIB_vibrate(1);
                            binaryReader.Close();
                            break;
                        }
                        case 2: // Vibrate
                            vib.VIB_setActive(false);
                            vib.VIB_setTrackActive(1, vib.VIB_SAMPLE_LO, true);
                            vib.VIB_setTrackActive(1, vib.VIB_SAMPLE_HI, true);
                            vib.VIB_vibrate(1);
                            break;
                    }
                }
                return 0;
            case 120: // Take a screenshot of the background's ground in the Vram
                PSXTextureMgr.isBgCapture = true;
                Marshal.Copy((IntPtr)p, PSXTextureMgr.bgParam, 0, 7);
                return 0;
            case 121: // Return btl_id of player characters that were not removed from the battle (with Snort / Swallow)
                return battle.btl_bonus.member_flag;
            case 122: // Back Attack, Preemptive, Normal
                return (Byte)FF9StateSystem.Battle.FF9Battle.btl_scene.Info.StartType;
            case 123: // Return the number of targetable player characters
            {
                Int32 validPlayerTarget = 0;
                for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
                    if (next.bi.player != 0 && !btl_stat.CheckStatus(next, BattleStatus.Death | BattleStatus.Jump))
                        validPlayerTarget++;
                return validPlayerTarget;
            }
            case 124: // Return the current battle camera index (these cameras are predefined per BTL_SCENE)
                return FF9StateSystem.Battle.FF9Battle.seq_work_set.CameraNo;
        }
        BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next;
        while (btl != null)
        {
            if (btl.btl_id == btlid)
                break;
            btl = btl.next;
        }
        if (btl == null)
            return (code != 9) ? 0 : 1;
        switch (code)
        {
            case 2: // Set Position
            case 4: // Set Angles
            case 6: // Set Size
            case 11: // Set Current Animation's current frame
            case 12: // Set Current Animation
            case 16: // Reset Stand Animation
            case 17: // Is Hidden (no model, no targeting)
            case 18: // Set Hidden On/Off
            case 19: // Show/Hide Weapon
            case 20: // Status
            case 21: // Has a main command queued (Ready)
            case 22: // Is attached to another enemy
            case 23: // Effect Point
            case 24: // Figure Point
            case 25: // Show/Hide mesh
            case 27: // Update color
            case 28: // Play/Stop Texture Animation
            case 29: // Play Sound (Jump)
            case 30: // Play Weapon Sound (see FF9Snd.ff9battleSoundWeaponSndEffect02, first entry)
            case 31: // Play Weapon Sound (see FF9Snd.ff9battleSoundWeaponSndEffect02, second entry)
            case 33: // Play Casting Animation (Dragon)
                return 0;
            case 1: // Get Position
                switch (arg0)
                {
                    case 0: // Current 3D
                        *(Int16*)p = (Int16)btl.pos.x;
                        *(Int16*)((Byte*)p + 2) = (Int16)(-btl.pos.y);
                        *(Int16*)((Byte*)p + 4) = (Int16)btl.pos.z;
                        break;
                    case 1: // Base pos 3D
                        *(Int16*)p = (Int16)btl.base_pos.x;
                        *(Int16*)((Byte*)p + 2) = (Int16)(-btl.base_pos.y);
                        *(Int16*)((Byte*)p + 4) = (Int16)btl.base_pos.z;
                        break;
                    case 2: // Current 2D
                        *(Int16*)p = (Int16)btl.pos.x;
                        *(Int16*)((Byte*)p + 4) = (Int16)btl.pos.z;
                        break;
                    case 3: // Base pos 2D
                        *(Int16*)p = (Int16)btl.base_pos.x;
                        *(Int16*)((Byte*)p + 4) = (Int16)btl.base_pos.z;
                        break;
                }
                break;
            case 3: // Get Angles
                switch (arg0)
                {
                    case 0: // Difference between base and current
                        *(Int16*)p = (Int16)((btl.evt.rotBattle.eulerAngles.x - btl.rot.eulerAngles.x) * 11.3777781f);
                        *(Int16*)((Byte*)p + 2) = (Int16)((btl.evt.rotBattle.eulerAngles.y - btl.rot.eulerAngles.y) * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((btl.evt.rotBattle.eulerAngles.z - btl.rot.eulerAngles.z) * 11.3777781f);
                        break;
                    case 1: // Current
                        *(Int16*)p = (Int16)(btl.rot.eulerAngles.x * 11.3777781f);
                        *(Int16*)((Byte*)p + 2) = (Int16)(btl.rot.eulerAngles.y * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((btl.rot.eulerAngles.z - 180f) * 11.3777781f);
                        break;
                    case 2: // Base
                        *(Int16*)p = (Int16)(btl.evt.rotBattle.eulerAngles.x * 11.3777781f);
                        *(Int16*)((Byte*)p + 2) = (Int16)(btl.evt.rotBattle.eulerAngles.y * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((btl.evt.rotBattle.eulerAngles.z - 180f) * 11.3777781f);
                        break;
                    case 3: // Base, no orientation (horizontal angle)
                        *(Int16*)p = (Int16)(btl.evt.rotBattle.eulerAngles.x * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((btl.evt.rotBattle.eulerAngles.z - 180f) * 11.3777781f);
                        break;
                }
                break;
            case 5: // Get Size
                *(Int32*)p = btl.geo_scale_x;
                *(Int32*)((Byte*)p + 4) = btl.geo_scale_y;
                *(Int32*)((Byte*)p + 8) = btl.geo_scale_z;
                break;
            case 7: // Get geo flags
                return btl.flags;
            case 8: // Is using Auto-Potion
                return 0;
            case 9: // Get Current Animation's frame count
                return 1;
            case 10: // Get Current Animation's current frame
                return 1;
            case 13: // Stop Animation
                return arg0;
            case 14: // Get Bone Stance
                Matrix4x4 matrix4x;
                try
                {
                    matrix4x = btl.gameObject.transform.GetChildByName("bone" + arg1.ToString("D3")).localToWorldMatrix;
                }
                catch (NullReferenceException)
                {
                    matrix4x = Matrix4x4.identity;
                }
                switch (arg0)
                {
                    case 0: // Get Bone Position
                        *(Int16*)p = (Int16)matrix4x.m03;
                        *(Int16*)((Byte*)p + 2) = (Int16)(-matrix4x.m13);
                        *(Int16*)((Byte*)p + 4) = (Int16)matrix4x.m23;
                        break;
                    case 1: // Get Bone Height
                        return (Int32)(-matrix4x.m13);
                    case 2: // Get Bone Orientation & Position
                        SFX.PSXMAT* matPtr = (SFX.PSXMAT*)p;
                        Int16* rptr = matPtr->r;
                        Int32* tptr = matPtr->t;
                        matPtr->pad = 0;
                        rptr[0] = (Int16)(matrix4x.m00 * -4096f);
                        rptr[1] = (Int16)(matrix4x.m01 * -4096f);
                        rptr[2] = (Int16)(matrix4x.m02 * 4096f);
                        rptr[3] = (Int16)(matrix4x.m10 * -4096f);
                        rptr[4] = (Int16)(matrix4x.m11 * 4096f);
                        rptr[5] = (Int16)(matrix4x.m12 * -4096f);
                        rptr[6] = (Int16)(matrix4x.m20 * 4096f);
                        rptr[7] = (Int16)(matrix4x.m21 * 4096f);
                        rptr[8] = (Int16)(matrix4x.m22 * 4096f);
                        tptr[0] = (Int32)matrix4x.m03;
                        tptr[1] = (Int32)(-matrix4x.m13);
                        tptr[2] = (Int32)matrix4x.m23;
                        break;
                }
                break;
            case 15: // Is Targetable
                return 1;
            case 26: // Number of meshes
                return btl.meshCount;
        }
        return 0;
    }

    public static Boolean BattleCallbackReaderExportSequence = false;
    public static Boolean BattleCallbackReaderTrackBtlMovement = false;
    public static Boolean BattleCallbackDummyLoadImages = false;
    public static Boolean BattleCallbackDummyUpdateVib = false;

    public static Boolean IsShortSpecialEffect(SpecialEffect eff)
    {
        //return AssetManager.LoadBytes("SpecialEffects/ef" + ((Int32)eff).ToString("D3"), out _, false) == null;
        return (eff >= SpecialEffect.Player_Attack_Zidane_Dagger && eff <= SpecialEffect.Player_Attack_Beatrix)
            || (eff >= SpecialEffect.SFX_Attack_Rod && eff <= SpecialEffect.SFX_Attack_Excalibur2)
            || (eff >= SpecialEffect.SFX_Attack_Javelin && eff <= SpecialEffect.SFX_Attack_Blade_Long);
    }

    public static HashSet<SpecialEffect> FixedCameraEffects = new HashSet<SpecialEffect> {
        SpecialEffect.Shiva__Full,
        SpecialEffect.Ifrit__Full,
        SpecialEffect.Ramuh__Full,
        //SpecialEffect.Atomos__Short,
        SpecialEffect.Atomos__Full,
        SpecialEffect.Odin__Full,
        SpecialEffect.Leviathan__Full,
        //SpecialEffect.Bahamut__Short,
        SpecialEffect.Bahamut__Full,
        SpecialEffect.Ark__Short,
        SpecialEffect.Ark__Full,
        //SpecialEffect.Carbuncle_Ruby__Full,
        //SpecialEffect.Carbuncle_Pearl__Full,
        //SpecialEffect.Carbuncle_Emerald__Full,
        //SpecialEffect.Carbuncle_Diamond__Full,
        //SpecialEffect.Fenrir_Earth__Short,
        SpecialEffect.Fenrir_Earth__Full,
        SpecialEffect.Fenrir_Wind__Full,
        SpecialEffect.Phoenix__Full,
        SpecialEffect.Phoenix_Rebirth_Flame,
        SpecialEffect.Madeen__Short,
        SpecialEffect.Madeen__Full,
        SpecialEffect.Meteor__Success,
        SpecialEffect.Meteor__Fail,
        SpecialEffect.Doomsday,
        SpecialEffect.Doomsday_Sword,
        SpecialEffect.Holy,
        SpecialEffect.LV4_Holy,
        SpecialEffect.Night,
        SpecialEffect.Whats_That,
        SpecialEffect.Grand_Cross
    };

    public class RunningInstance
    {
        public Int32 frame;
        public HashSet<UInt32> preventedMeshKeys;
        public List<UInt32> preventedMeshIndices;
        public Dictionary<UInt32, Color> coloredMeshes;
        public Dictionary<UInt32, Color> coloredMeshIndices;
        public List<UInt32> meshKeyList;
        public Boolean cancel;

        public RunningInstance(Int32 f, List<UInt32> pmk, List<UInt32> pmi, Dictionary<UInt32, Color> cbk, Dictionary<UInt32, Color> cbi)
        {
            frame = f;
            preventedMeshKeys = new HashSet<UInt32>(pmk);
            preventedMeshIndices = pmi;
            coloredMeshes = cbk;
            coloredMeshIndices = cbi;
            meshKeyList = new List<UInt32>();
            cancel = false;
        }

        public Color? TryGetCustomColor(UInt32 meshKey)
        {
            if (coloredMeshes.TryGetValue(meshKey, out Color col))
                return col;
            if (coloredMeshes.TryGetValue(0, out col))
                return col;
            return null;
        }
    }
}
