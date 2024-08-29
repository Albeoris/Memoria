﻿using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Database;
using Memoria.Prime;
using Memoria.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

#pragma warning disable 169
#pragma warning disable 414
#pragma warning disable 649

// ReSharper disable ArrangeThisQualifier
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable NotAccessedField.Local
// ReSharper disable ConvertToConstant.Global
// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable InconsistentNaming

public class HonoluluBattleMain : PersistenSingleton<MonoBehaviour>
{
    public static String battleSceneName;
    public static Int32[] CurPlayerWeaponIndex;
    public static Int32 EnemyStartIndex;
    public static Boolean playerEnterCommand;
    public static BattleSPSSystem battleSPS;
    public static Int32 counterATB;
    private static List<KeyValuePair<BTL_DATA, BTL_DATA>> attachModel;

    public BTL_SCENE btlScene;
    public BattleMapCameraController cameraController;
    public String[] animationName;
    public Boolean playerCastingSkill;
    public Boolean enemyEnterCommand;
    public List<Int32> seqList;
    private Boolean isSetup;
    private List<Material> playerMaterials;
    private List<Material> monsterMaterials;
    private BattleState battleState;
    private Byte[] btlIDList;
    private UInt32 battleResult;
    private Single debugUILastTouchTime;
    private Single showHideDebugUICoolDown;
    private String scaleEdit;
    private String distanceEdit;
    private Boolean needClampTime;
    private Single cumulativeTime;
    private Boolean isKeyFrame;
    private readonly Int32[][] playerXPos;

    static HonoluluBattleMain()
    {
        CurPlayerWeaponIndex = new Int32[4];
        EnemyStartIndex = 4;
        attachModel = new List<KeyValuePair<BTL_DATA, BTL_DATA>>();
    }

    public HonoluluBattleMain()
    {
        this.playerXPos = new Int32[][]
        {
            new Int32[]{ 0 },
            new Int32[]{ 316, -316 },
            new Int32[]{ 632, 0, -632 },
            new Int32[]{ 948, 316, -316, -948 }
        };
        this.showHideDebugUICoolDown = 0.5f;
        this.scaleEdit = String.Empty;
        this.distanceEdit = String.Empty;
    }

    protected override void Awake()
    {
        try
        {
            base.Awake();
            FPSManager.SetTargetFPS(Configuration.Graphics.BattleFPS);
            FPSManager.SetMainLoopSpeed(Configuration.Graphics.BattleTPS);
            this.playerMaterials = new List<Material>();
            this.monsterMaterials = new List<Material>();
            FF9StateSystem.Battle.isFade = false;
            this.animationName = new String[8];
            this.needClampTime = false;
            if (Application.platform == RuntimePlatform.Android)
            {
                String deviceModel = SystemInfo.deviceModel;
                if (String.Compare("Asus Nexus Player", deviceModel, true) == 0)
                    this.needClampTime = true;
            }
            FF9StateSystem instance = PersistenSingleton<FF9StateSystem>.Instance;
            FF9StateSystem.Battle.FF9Battle.map.nextMode = instance.prevMode;
            if (instance.prevMode == 1)
            {
                FF9StateSystem.Battle.FF9Battle.map.nextMapNo = FF9StateSystem.Common.FF9.fldMapNo;
            }
            else
            {
                if (instance.prevMode != 3)
                    return;
                FF9StateSystem.Battle.FF9Battle.map.nextMapNo = FF9StateSystem.Common.FF9.wldMapNo;
            }
        }
        catch (Exception err)
        {
            Log.Error(err);
        }
    }

    private void Start()
    {
        try
        {
            this.cameraController = GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>();
            this.InitBattleScene();
            FPSManager.SetTargetFPS(Configuration.Graphics.BattleFPS);
            FPSManager.SetMainLoopSpeed(Configuration.Graphics.BattleTPS);
            GameObject battleMapGo = GameObject.Find("BattleMap Root");
            GameObject spsSystemGo = new GameObject("BattleMap SPS");
            spsSystemGo.transform.parent = battleMapGo.transform;
            if (battleSPS == null)
                battleSPS = spsSystemGo.AddComponent<BattleSPSSystem>();
            battleSPS.Init();
            Byte cameraNo = FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].Camera;
            FF9StateSystem.Battle.FF9Battle.seq_work_set.CameraNo = cameraNo >= 3 ? (Byte)UnityEngine.Random.Range(0, 3) : cameraNo;
            SFX.StartBattle();
            BattleVoice.InitBattle();
            SmoothFrameUpdater_Battle.OnBattleMapChange();

            if ((Int64)FF9StateSystem.Settings.cfg.skip_btl_camera == 0L && FF9StateSystem.Battle.isRandomEncounter)
                SFX.SkipCameraAnimation(-1);

            if (!FF9StateSystem.Battle.isNoBoosterMap())
                return;

            FF9StateSystem.Settings.IsBoosterButtonActive[0] = false;
            FF9StateSystem.Settings.SetBoosterHudToCurrentState();
            PersistenSingleton<UIManager>.Instance.Booster.SetBoosterButton(BoosterType.BattleAssistance, false);
        }
        catch (Exception err)
        {
            Log.Error(err);
        }
    }

    public void InitBattleScene()
    {
        FF9StateGlobal FF9 = FF9StateSystem.Common.FF9;
        FF9.charArray.Clear();
        attachModel.Clear();
        this.btlScene = FF9StateSystem.Battle.FF9Battle.btl_scene = new BTL_SCENE();
        Debug.Log("battleID = " + FF9StateSystem.Battle.battleMapIndex);

        battlebg.CreateBattleRoot();
        FF9BattleDB.SceneData.TryGetKey(FF9StateSystem.Battle.battleMapIndex, out battleSceneName);
        battleSceneName = battleSceneName.Substring(4);
        Debug.Log("battleSceneName = " + battleSceneName);
        this.btlScene.ReadBattleScene(battleSceneName);
        this.StartCoroutine(PersistenSingleton<FF9TextTool>.Instance.UpdateBattleText(FF9BattleDB.SceneData["BSC_" + battleSceneName]));
        WMProfiler.Begin("Start Load Text");
        String battleModelPath = String.IsNullOrEmpty(this.btlScene.Info.BattleBackground) ? FF9BattleDB.MapModel["BSC_" + battleSceneName] : this.btlScene.Info.BattleBackground;
        FF9StateSystem.Battle.FF9Battle.map.btlBGPtr = ModelFactory.CreateModel("BattleMap/BattleModel/battleMap_all/" + battleModelPath + "/" + battleModelPath, Vector3.zero, Vector3.zero, true);
        FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum = FF9StateSystem.Battle.isDebug ? FF9StateSystem.Battle.patternIndex : (Byte)this.ChoicePattern();
        btlseq.ReadBattleSequence(battleSceneName);
        btlseq.instance.FixBuggedAnimations(this.btlScene);
        BeginInitialize();
        battle.InitBattle();
        EndInitialize();
        if (!FF9StateSystem.Battle.isDebug)
        {
            String ebFileName = "EVT_BATTLE_" + battleSceneName;
            FF9StateBattleMap ff9StateBattleMap = FF9StateSystem.Battle.FF9Battle.map;
            ff9StateBattleMap.evtPtr = EventEngineUtils.loadEventData(ebFileName, "Battle/");
            PersistenSingleton<EventEngine>.Instance.StartEvents(ff9StateBattleMap.evtPtr);
            PersistenSingleton<EventEngine>.Instance.eTb.InitMessage();
        }
        this.CreateBattleData(FF9);
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        GEOTEXHEADER geotexheader = new GEOTEXHEADER();
        geotexheader.ReadBGTextureAnim(battleModelPath);
        stateBattleSystem.map.btlBGTexAnimPtr = geotexheader;
        BBGINFO bbginfo = new BBGINFO();
        bbginfo.ReadBattleInfo(battleModelPath);
        FF9StateSystem.Battle.FF9Battle.map.btlBGInfoPtr = bbginfo;
        battle.InitBattleMap();
        this.seqList = new List<Int32>();
        SB2_PATTERN sb2Pattern = this.btlScene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum];
        Int32[] monsterType = new Int32[sb2Pattern.MonsterCount];
        for (Int32 i = 0; i < sb2Pattern.MonsterCount; ++i)
            monsterType[i] = sb2Pattern.Monster[i].TypeNo;
        foreach (Int32 typeNo in monsterType.Distinct().ToArray())
        {
            for (Int32 i = 0; i < btlseq.instance.sequenceProperty.Length; ++i)
            {
                SequenceProperty sequenceProperty = btlseq.instance.sequenceProperty[i];
                if (sequenceProperty.Montype == typeNo)
                    for (Int32 j = 0; j < sequenceProperty.PlayableSequence.Count; ++j)
                        this.seqList.Add(sequenceProperty.PlayableSequence[j]);
            }
        }
        this.btlIDList = FF9StateSystem.Battle.FF9Battle.seq_work_set.AnmOfsList.Distinct().ToArray();
        this.battleState = BattleState.PlayerTurn;
        playerEnterCommand = false;
        this.playerCastingSkill = false;
        this.enemyEnterCommand = false;
        
        
        if (Configuration.Shaders.CustomShaderEnabled == 1 && _updateAmbientRoutine == null)
        {
            _updateAmbientRoutine = this.StartCoroutine(UpdateAmbientLight());
        }
    }

    private void CreateBattleData(FF9StateGlobal FF9)
    {
        BTL_DATA[] btl = btlseq.instance.btl_list = FF9StateSystem.Battle.FF9Battle.btl_data;
        Int32 pindex = 0;
        for (Int32 i = 0; i < 4; ++i)
        {
            btl[i] = new BTL_DATA();
            btl[i].typeNo = Byte.MaxValue;
            if (FF9.party.member[i] != null)
            {
                BattlePlayerCharacter.CreatePlayer(btl[pindex], FF9.party.member[i]);
                Int32 meshCount = 0;
                foreach (Transform transform in btl[pindex].gameObject.transform)
                    if (transform.name.Contains("mesh"))
                        meshCount++;
                btl[pindex].meshCount = meshCount;
                btl[pindex].meshIsRendering = new Boolean[meshCount];
                for (Int32 j = 0; j < meshCount; ++j)
                    btl[pindex].meshIsRendering[j] = true;
                btl[pindex].animation = btl[pindex].gameObject.GetComponent<Animation>();
                ++pindex;
            }
        }
        for (Int32 i = 4; i < 4 + this.btlScene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].MonsterCount; ++i)
        {
            SB2_PATTERN sb2Pattern = this.btlScene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum];
            Byte monType = sb2Pattern.Monster[i - 4].TypeNo;
            SB2_MON_PARM sb2MonParm = this.btlScene.MonAddr[monType];
            String path = FF9BattleDB.GEO.GetValue(sb2MonParm.Geo);
            //var vector3 = new Vector3(sb2Pattern.Put[index2 - 4].Xpos, sb2Pattern.Put[index2 - 4].Ypos * -1, sb2Pattern.Put[index2 - 4].Zpos);
            btl[i] = new BTL_DATA { gameObject = ModelFactory.CreateModel(path, true, true, Configuration.Graphics.ElementsSmoothTexture) };
            btl[i].typeNo = monType;
            if (!String.IsNullOrEmpty(sb2MonParm.WeaponModel))
            {
                if (sb2MonParm.WeaponModel.Contains("GEO_WEP"))
                    btl[i].weapon_geo = ModelFactory.CreateModel("BattleMap/BattleModel/battle_weapon/" + sb2MonParm.WeaponModel + "/" + sb2MonParm.WeaponModel, true, true, Configuration.Graphics.ElementsSmoothTexture);
                else
                    btl[i].weapon_geo = ModelFactory.CreateModel(sb2MonParm.WeaponModel, true, true, Configuration.Graphics.ElementsSmoothTexture);
                geo.geoAttach(btl[i].weapon_geo, btl[i].gameObject, sb2MonParm.WeaponAttachment);
                if (btl_eqp.EnemyBuiltInWeaponTable.ContainsKey(sb2MonParm.Geo))
                    btl[i].builtin_weapon_mode = true;
            }
            else if (ModelFactory.IsUseAsEnemyCharacter(path))
            {
                if (path.Contains("GEO_MON_B3_168"))
                    btl[i].gameObject.transform.FindChild("mesh5").gameObject.SetActive(false);
                btl[i].weapon_geo = ModelFactory.CreateDefaultWeaponForCharacterWhenUseAsEnemy(path);
                geo.geoAttach(btl[i].weapon_geo, btl[i].gameObject, ModelFactory.GetDefaultWeaponBoneIdForCharacterWhenUseAsEnemy(path));
            }
            else
            {
                btl[i].weapon_geo = null;
            }
            if (sb2MonParm.TextureFiles != null)
                ModelFactory.ChangeModelTexture(btl[i].gameObject, sb2MonParm.TextureFiles);
            if (btl[i].weapon_geo != null)
            {
                MeshRenderer[] weaponRenderers = btl[i].weapon_geo.GetComponentsInChildren<MeshRenderer>();
                btl[i].weaponMeshCount = weaponRenderers.Length;
                btl[i].weaponRenderer = new Renderer[btl[i].weaponMeshCount];
                if (btl[i].weaponMeshCount > 0)
                {
                    for (Int32 j = 0; j < btl[i].weaponMeshCount; j++)
                    {
                        btl[i].weaponRenderer[j] = weaponRenderers[j].GetComponent<Renderer>();
                        if (sb2MonParm.WeaponTextureFiles != null && sb2MonParm.WeaponTextureFiles.Length > j && !String.IsNullOrEmpty(sb2MonParm.WeaponTextureFiles[j]))
                            btl[i].weaponRenderer[j].material.mainTexture = AssetManager.Load<Texture2D>(sb2MonParm.WeaponTextureFiles[j], false);
                    }
                }
                else if (sb2MonParm.WeaponTextureFiles != null) // Other kind of model have no btl.weaponMeshCount
                {
                    ModelFactory.ChangeModelTexture(btl[i].weapon_geo, sb2MonParm.WeaponTextureFiles);
                }
            }
            Int32 meshCount = 0;
            foreach (Transform transform in btl[i].gameObject.transform)
                if (transform.name.Contains("mesh"))
                    meshCount++;
            btl[i].meshCount = meshCount;
            btl[i].meshIsRendering = new Boolean[meshCount];
            for (Int32 j = 0; j < meshCount; ++j)
                btl[i].meshIsRendering[j] = true;
            btl[i].animation = btl[i].gameObject.GetComponent<Animation>();
        }
    }

    public void SetFrontRow()
    {
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            if (btlData.bi.player != 0)
            {
                Vector3 vector3 = btlData.pos;
                vector3.z = !FF9StateSystem.Battle.isFrontRow ? -1960f : -1560f;
                btlData.pos = vector3;
            }
        }
    }

    private Int32 ChoicePattern()
    {
        Int32 num1 = UnityEngine.Random.Range(0, 100);
        Int32 num2 = this.btlScene.PatAddr[0].Rate;
        Int32 num3 = 0;
        while (num1 >= num2)
        {
            num2 += this.btlScene.PatAddr[num3 + 1].Rate;
            ++num3;
        }
        if (FF9StateSystem.Common.FF9.btlSubMapNo != -1)
            num3 = FF9StateSystem.Common.FF9.btlSubMapNo;
        if (num3 < 0 || num3 >= this.btlScene.header.PatCount)
            num3 = 0;
        FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum = (Byte)num3;
        return num3;
    }

    // Dummied
    public static void UpdateFrameTime(Int32 newSpeed)
    {
        //Speed = newSpeed;
        //for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        //{
        //    if (btlData.gameObject == null)
        //        return;
        //
        //    foreach (object item in btlData.gameObject.GetComponent<Animation>())
        //    {
        //        // do nothing?
        //    }
        //}
        //fps = Configuration.Graphics.BattleFPS * newSpeed;
        //frameTime = 1f / fps;
    }

    private void YMenu_ManagerActiveTime()
    {
        BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next;
        UIManager.Battle.UpdateSlidingButtonState();
        if (UIManager.Battle.IsNativeEnableAtb())
            ProcessActiveTime(btl);
    }

    private void ProcessActiveTime(BTL_DATA btl)
    {
        Int32 battleSpeed = Configuration.Battle.Speed;

        BTL_DATA source = btl;
        Boolean canContinue = false;
        Boolean needContinue = false;
        Int32 maxLoop = 1000;

        do
        {
            if (battleSpeed == 1 || battleSpeed == 2)
            {
                // Check if someone has some atb, we don't want to advance atb instantly in some cases
                // This is necessary for some fights starting with a conversation where atb is forced to 0 (see issue #430)
                needContinue = false;
                foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
                {
                    if (!unit.IsPlayer) continue;
                    if (unit.CurrentAtb > 0)
                    {
                        needContinue = true;
                        break;
                    }
                }
            }

            Boolean advanceAtb = UIManager.Battle.FF9BMenu_IsEnableAtb();
            if (advanceAtb) HonoluluBattleMain.counterATB++;

            for (btl = source; btl != null; btl = btl.next)
            {
                if (btl.cur.hp == 0 || btl.sel_mode != 0 || btl.bi.atb == 0)
                    continue;

                POINTS current = btl.cur;
                POINTS maximum = btl.max;

                Boolean changed = false;
                if (current.at < maximum.at)
                {
                    if (btl.bi.player != 0 || (battleSpeed == 2 && BattleHUD.ForceNextTurn))
                        canContinue = true;

                    changed = true;
                    if (advanceAtb)
                        current.at += (Int16)Math.Max(1, current.at_coef * 4);
                    else
                        needContinue = false;
                }

                if (current.at < maximum.at)
                    continue;

                if (battleSpeed == 2)
                {
                    if (btl.bi.player != 0)
                    {
                        if (changed)
                        {
                            if (BattleHUD.ForceNextTurn)
                            {
                                BattleHUD.ForceNextTurn = false;
                                BattleHUD.switchBtlId = btl.btl_id;
                            }
                            needContinue = false;
                        }
                    }
                    else if (!btl_stat.CheckStatus(btl, BattleStatusConst.PreventATBConfirm))
                    {
                        if (changed) BattleHUD.ForceNextTurn = false;
                        needContinue = false;
                    }
                }
                else if (battleSpeed != 1 || !btl_stat.CheckStatus(btl, BattleStatusConst.PreventATBConfirm))
                {
                    needContinue = false;
                }

                if (!Configuration.Fixes.IsKeepRestTimeInBattle)
                    current.at = maximum.at;

                if (btl_stat.CheckStatus(btl, BattleStatusConst.PreventATBConfirm))
                    continue;

                if (btl.bi.player != 0)
                {
                    if (!btl_cmd.CheckUsingCommand(btl.cmd[0]))
                    {
                        Boolean autoAttack = false;
                        foreach (BattleStatusId statusId in btl.stat.cur.ToStatusList())
                        {
                            if (!btl.stat.effects.TryGetValue(statusId, out StatusScriptBase effect))
                                continue;
                            if ((effect as IAutoAttackStatusScript)?.OnATB() ?? false)
                            {
                                autoAttack = true;
                                break;
                            }
                        }
                        if (autoAttack)
                        {
                            btl.sel_mode = 1;
                        }
                        else
                        {
                            Int32 playerId = 0;
                            while (1 << playerId != btl.btl_id)
                                ++playerId;
                            if (!UIManager.Battle.ReadyQueue.Contains(playerId))
                                UIManager.Battle.AddPlayerToReady(playerId);
                        }
                    }
                }
                else if (!FF9StateSystem.Battle.isDebug)
                {
                    if (PersistenSingleton<EventEngine>.Instance.RequestAction(BattleCommandId.EnemyAtk, btl.btl_id, 0, 0, 0))
                        btl.sel_mode = 1;
                }
                else
                {
                    if (Array.IndexOf(FF9StateSystem.Battle.FF9Battle.seq_work_set.AnmOfsList, this.btlIDList[btl_scrp.FindBattleUnit(btl.btl_id).Data.typeNo]) < 0)
                        Debug.LogError("Index out of range");

                    if (FF9StateSystem.Battle.FF9Battle.btl_phase != FF9StateBattleSystem.PHASE_NORMAL)
                        return;
                }
            }

            // Failsafe - some events might produce infinite loops (i.e. atb never advance)
            if (--maxLoop <= 0)
            {
                Log.Warning($"[ProcessActiveTime] Failsafe activated - btlMapNo: {FF9StateSystem.Common.FF9.btlMapNo} fldMapNo: {FF9StateSystem.Common.FF9.fldMapNo} wldMapNo: {FF9StateSystem.Common.FF9.wldMapNo}");
                return;
            }

            if (canContinue && needContinue)
            {
                // Update statuses before looping
                for (btl = source; btl != null; btl = btl.next)
                {
                    BattleUnit unit = new BattleUnit(btl);
                    btl_para.CheckPointData(unit);
                    btl_stat.CheckStatusLoop(unit);
                }
                // Events need to be processed (i.e. atb reset to 0, see issue #430)
                PersistenSingleton<EventEngine>.Instance.ServiceEvents();
            }
        } while (canContinue && needContinue);
    }

    private void Update()
    {
        try
        {
            UpdateAttachModel();
            UpdateFrames();
            UpdateData();
        }
        catch (Exception err)
        {
            Log.Error(err);
        }
    }

    private void UpdateFrames()
    {
        //this.cumulativeTime += Time.deltaTime;
        //if (this.needClampTime)
        //    this.cumulativeTime = Mathf.Min(this.cumulativeTime, HonoluluBattleMain.frameTime * (float)SettingsState.FastForwardGameSpeed * 1.2f);
        //
        //while (this.cumulativeTime >= (Double)frameTime)
        //{
        //    this.cumulativeTime -= frameTime;
        //    battleSPS.Service();
        //    if (!IsOver)
        //        UpdateBattleFrame();
        //    else
        //        UpdateOverFrame();
        //}
        for (Int32 updateCount = 0; updateCount < FPSManager.MainLoopUpdateCount; updateCount++)
        {
            if (!IsPaused)
                SmoothFrameUpdater_Battle.ResetState();
            if (IsOver)
                UpdateOverFrame();
            else
                UpdateBattleFrame();
            if (!IsPaused)
                SmoothFrameUpdater_Battle.RegisterState();
        }
        if (!IsPaused)
            FPSManager.AddSmoothEffect(SmoothFrameUpdater_Battle.Apply);
    }
    
    // runtime game object and material for creating ambient lighting
    private ReflectionProbe _reflectionProbe;
    private bool _hasUpdateProbeCapture = false;
    private bool _hasUpdateAmbient = false;
    private Material _skyBox;

    private IEnumerator UpdateAmbientLight()
    {
        // TODO: I think BTL_DATA are always initialised at this point because of the call at the end of "InitBattleScene"
        Boolean hasValidCharacter = false;
        while (!hasValidCharacter)
        {
            hasValidCharacter = FF9StateSystem.Battle.FF9Battle.btl_list.next != null;
            yield return null;
        }

        //for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        //    if (btl.bi.player == 1)
        
        if (_reflectionProbe == null)
        {
            GameObject obj = new GameObject("EnvironmentCapture");
            // the position is somewhere above one of the character's head in the battle scene....
            obj.transform.position = new Vector3(632.0f, 500.0f, -1560.0f);
            _reflectionProbe = obj.AddComponent<ReflectionProbe>();
            _reflectionProbe.mode = ReflectionProbeMode.Realtime;
            _reflectionProbe.cullingMask = -1;
            _reflectionProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
            _reflectionProbe.size = new Vector3(10000, 10000, 10000);
            _reflectionProbe.resolution = 128;
            _reflectionProbe.clearFlags = ReflectionProbeClearFlags.SolidColor;
            _reflectionProbe.backgroundColor = Color.black;
        }
        
        if (_skyBox == null)
            _skyBox = new Material(ShadersLoader.Find("PSX/Skybox_Cubemap"));
        
        if (_reflectionProbe != null)
        {
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientIntensity = Configuration.Shaders.Shader_Battle_Toon == 1 ? 0.8f : 1.5f;
            RenderSettings.skybox = _skyBox;
            _reflectionProbe.RenderProbe();
        }
        // this simply mean the reflection probe's texture are not yet update internally.
        // so we need to wait for it.
        while (_reflectionProbe.texture.name.Contains("Black"))
        {
            yield return new WaitForEndOfFrame();
        }
        _skyBox.SetTexture("_Tex", _reflectionProbe.texture);
        _skyBox.SetFloat("_Exposure", Configuration.Shaders.Shader_Battle_Toon == 1 ? 0.5f : 1.0f);
        
        // This is a slow operation, make sure this code only run once.
        DynamicGI.UpdateEnvironment();
    }

    private Coroutine _updateAmbientRoutine = null;
    private float _debugNormal = -1;
    private float _debugSH = -1;
    
    private void UpdateBattleFrame()
    {
        if (IsPaused)
            return;

        HonoluluBattleMain.counterATB = 0;
        battleSPS.EffectUpdate();
        this.battleResult = battle.BattleMain();
        if (!FF9StateSystem.Battle.isDebug)
        {
            if (UIManager.Battle.FF9BMenu_IsEnable())
                this.YMenu_ManagerActiveTime();

            for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
                btl.CheckDelayedModifier();

            if (this.battleResult == 1)
            {
                PersistenSingleton<FF9StateSystem>.Instance.mode = 8;
                IsOver = true;
            }
        }
        SceneDirector.ServiceFade();
        foreach (BattleCalculator calc in BattleCalculator.FrameAppliedEffectList)
            BattleHUD.EffectedBtlId |= calc.Target.Id;
        BattleCalculator.FrameAppliedEffectList.Clear();
    }

    private static void UpdateOverFrame()
    {
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        switch (ff9StateGlobal.btl_result)
        {
            case FF9StateGlobal.BTL_RESULT_VICTORY:
            case FF9StateGlobal.BTL_RESULT_VICTORY_NO_POSE:
            case FF9StateGlobal.BTL_RESULT_DEFEAT:
            case FF9StateGlobal.BTL_RESULT_ESCAPE:
            case FF9StateGlobal.BTL_RESULT_INTERRUPTION:
            case FF9StateGlobal.BTL_RESULT_ENEMY_FLEE:
                if (ff9StateGlobal.btl_result == FF9StateGlobal.BTL_RESULT_VICTORY_NO_POSE)
                    ff9StateGlobal.btl_result = FF9StateGlobal.BTL_RESULT_VICTORY;
                if (FF9StateSystem.Battle.FF9Battle.map.nextMode == 1 || FF9StateSystem.Battle.FF9Battle.map.nextMode == 5)
                    FF9StateSystem.Common.FF9.fldMapNo = FF9StateSystem.Battle.FF9Battle.map.nextMapNo;
                else if (FF9StateSystem.Battle.FF9Battle.map.nextMode == 3)
                    FF9StateSystem.Common.FF9.wldMapNo = FF9StateSystem.Battle.FF9Battle.map.nextMapNo;
                UIManager.Battle.GoToBattleResult();
                if (!FF9StateSystem.Battle.isDebug)
                {
                    PersistenSingleton<EventEngine>.Instance.ServiceEvents();
                    SceneDirector.ServiceFade();
                }
                return;
            case FF9StateGlobal.BTL_RESULT_GAMEOVER:
                UIManager.Battle.GoToGameOver();
                return;
            default:
                return;
        }
    }

    private static void UpdateData()
    {
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.bi.slave != 0 || btl.bi.disappear != 0 || btl.bi.shadow == 0)
                continue;

            FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
            Int32 BoneNo = ff9btl.ff9btl_set_bone(btl.shadow_bone[0], btl.shadow_bone[1]);
            if (btl.bi.player != 0)
            {
                if ((stateBattleSystem.cmd_status & 1) == 0)
                {
                    if ((btl.escape_key ^ stateBattleSystem.btl_escape_key) != 0)
                        btl_mot.SetDefaultIdle(btl);
                    btl.escape_key = stateBattleSystem.btl_escape_key;
                }
            }
            btlseq.FF9DrawShadowCharBattle(stateBattleSystem.map.shadowArray[btl], btl, 0, BoneNo);
        }

        // Smooth all battle character once
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.bi.slave != 0 || btl.bi.disappear != 0 || btl.bi.shadow == 0)
                continue;
            if (btl._hasMeshSmoothed)
                continue;
            NormalSolver.SmoothCharacterMesh(btl.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>());
            NormalSolver.SmoothCharacterMesh(btl.gameObject.GetComponentsInChildren<MeshRenderer>());
            btl._hasMeshSmoothed = true;
        }
    }

    public static void UpdateAttachModel()
    {
        foreach (KeyValuePair<BTL_DATA, BTL_DATA> kvp in attachModel)
        {
            BTL_DATA carryingBtl = kvp.Key;
            BTL_DATA attachedBtl = kvp.Value;
            Transform attachedTransform = attachedBtl.gameObject.transform;
            Transform rootTransform = attachedTransform.GetChildByName("bone000").transform;
            attachedTransform.localPosition = Vector3.zero;
            attachedTransform.localRotation = Quaternion.identity;
            attachedTransform.localScale = Vector3.one;
            rootTransform.localPosition = Vector3.zero;
            rootTransform.localRotation = Quaternion.identity;
            rootTransform.localScale = Vector3.one;
            attachedBtl.pos = carryingBtl.pos;
        }
    }

    public static void SetupAttachModel(BTL_DATA carryingBtl, BTL_DATA attachedBtl, Int32 boneIndex, Int32 offset)
    {
        if (carryingBtl.gameObject == null || attachedBtl.gameObject == null)
            return;
        Transform attachedTransform = attachedBtl.gameObject.transform;
        Transform rootTransform = attachedTransform.GetChildByName("bone000").transform;
        Transform carryingBone = carryingBtl.gameObject.transform.GetChildByName($"bone{boneIndex:D3}");
        attachedTransform.parent = carryingBone.transform;
        attachedTransform.localPosition = Vector3.zero;
        attachedTransform.localRotation = Quaternion.identity;
        attachedTransform.localScale = Vector3.one;
        rootTransform.localPosition = Vector3.zero;
        rootTransform.localRotation = Quaternion.identity;
        rootTransform.localScale = Vector3.one;
        attachedBtl.attachOffset = offset;
        attachModel.Add(new KeyValuePair<BTL_DATA, BTL_DATA>(carryingBtl, attachedBtl));
    }

    public static void ClearAttachModel(BTL_DATA attachedBtl)
    {
        attachModel.RemoveAll(kvp => kvp.Value == attachedBtl);
        attachedBtl.attachOffset = 0;
        if (attachedBtl.gameObject != null)
            attachedBtl.gameObject.transform.parent = battlebg.BattleRoot.transform;
    }

    public static Boolean IsAttachedModel(BTL_DATA btl)
    {
        return attachModel.Any(kvp => kvp.Value == btl);
    }

    private void LateUpdate()
    {
        UpdateAttachModel();
        UIManager.Battle.modelButtonManager.UpdateModelButtonPosition();
        Singleton<HUDMessage>.Instance.UpdateChildPosition();
        btl_eqp.UpdateWeaponOffsets();
    }

    public Int32 GetWeaponID(Int32 battlePlayerPosID)
    {
        return CurPlayerWeaponIndex[battlePlayerPosID];
    }

    private void OnGUI()
    {
        //if (!EventEngineUtils.showDebugUI)
        //    return;
        //Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
        //DebugGuiSkin.ApplySkin();
        //if (FF9StateSystem.Battle.isDebug)
        //    return;
        //GUILayout.BeginArea(fullscreenRect);
        //GUILayout.BeginHorizontal();
        //GUILayout.EndHorizontal();
        //GUILayout.EndArea();
    }

    private void OnDestroy()
    {
        SFX.EndBattle();
        if (Configuration.Shaders.CustomShaderEnabled == 1)
        {
            // Make sure we destroy the object we created for ambient lighting during init phase
            if (_reflectionProbe != null)
            {
                Destroy(_reflectionProbe.gameObject);
                _hasUpdateProbeCapture = false;
                _hasUpdateAmbient = false;
            }
            StopCoroutine(_updateAmbientRoutine);
            _updateAmbientRoutine = null;   
        }

        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            if (btlData.texanimptr != null)
            {
                foreach (RenderTexture value in btlData.texanimptr.RenderTexMapping.Values)
                    value.Release();
                btlData.texanimptr.RenderTexMapping.Clear();
            }
            if (btlData.bi.player != 0 && btlData.tranceTexanimptr != null)
            {
                foreach (RenderTexture value in btlData.tranceTexanimptr.RenderTexMapping.Values)
                    value.Release();
                btlData.tranceTexanimptr.RenderTexMapping.Clear();
            }
        }

        if (battlebg.nf_BbgTabAddress != null)
        {
            foreach (RenderTexture value in battlebg.nf_BbgTabAddress.RenderTexMapping.Values)
                value.Release();
        }

        if (FF9StateSystem.Battle.FF9Battle.map.btlBGTexAnimPtr != null)
            FF9StateSystem.Battle.FF9Battle.map.btlBGTexAnimPtr.RenderTexMapping.Clear();
    }

    public void SetActive(Boolean active)
    {
        if (active)
            ResumeBattle();
        else
            PauseBattle();
    }

    private void PauseBattle()
    {
        IsPaused = true;
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
            if (btlData.animation != null)
                btlData.animation.enabled = false;
    }

    private void ResumeBattle()
    {
        IsPaused = false;
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
            if (btlData.animation != null)
                btlData.animation.enabled = true;
    }

    public enum BattleState
    {
        PlayerTurn,
        EnemyTurn,
    }

    private static void BeginInitialize()
    {
        FF9StateSystem.Battle.FF9Battle.attr = 0;
    }

    private static void EndInitialize()
    {
        IsInitialized = true;
    }

    private static Boolean IsInitialized
    {
        get => (FF9StateSystem.Battle.FF9Battle.attr & ff9btl.ATTR.LOADBBG) == ff9btl.ATTR.LOADBBG;
        set
        {
            if (value)
                FF9StateSystem.Battle.FF9Battle.attr |= ff9btl.ATTR.LOADBBG;
            else
                FF9StateSystem.Battle.FF9Battle.attr &= ~ff9btl.ATTR.LOADBBG;
        }
    }

    private static Boolean IsPaused
    {
        get => (FF9StateSystem.Battle.FF9Battle.attr & ff9btl.ATTR.NOPUTDISPENV) == ff9btl.ATTR.NOPUTDISPENV || battle.isSpecialTutorialWindow;
        set
        {
            if (value)
                FF9StateSystem.Battle.FF9Battle.attr |= ff9btl.ATTR.NOPUTDISPENV;
            else
                FF9StateSystem.Battle.FF9Battle.attr &= ~ff9btl.ATTR.NOPUTDISPENV;
        }
    }

    private static Boolean IsOver
    {
        get => (FF9StateSystem.Battle.FF9Battle.attr & ff9btl.ATTR.EXITBATTLE) == ff9btl.ATTR.EXITBATTLE;
        set
        {
            if (value)
                FF9StateSystem.Battle.FF9Battle.attr |= ff9btl.ATTR.EXITBATTLE;
            else
                FF9StateSystem.Battle.FF9Battle.attr &= ~ff9btl.ATTR.EXITBATTLE;
        }
    }
}
