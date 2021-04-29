using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Memoria;
using Memoria.Data;
using Memoria.Database;
using UnityEngine;

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
    public BTL_SCENE btlScene;
    public static String battleSceneName;
    public BattleMapCameraController cameraController;
    private Boolean isSetup;
    public String[] animationName;
    private static readonly Int32[] CurPlayerSerialNum;
    public static Int32[] CurPlayerWeaponIndex;
    public static Int32 EnemyStartIndex;
    private readonly Int32[][] playerXPos;
    private List<Material> playerMaterials;
    private List<Material> monsterMaterials;
    private BattleState battleState;
    public static Boolean playerEnterCommand;
    public Boolean playerCastingSkill;
    public Boolean enemyEnterCommand;
    public List<Int32> seqList;
    private Byte[] btlIDList;
    private UInt64 battleResult;
    private Single debugUILastTouchTime;
    private Single showHideDebugUICoolDown;
    private Transform rootBone;
    public static BattleSPSSystem battleSPS;
    private String scaleEdit;
    private String distanceEdit;
    private Boolean needClampTime;
    private UInt32 counter;
    private Single cumulativeTime;
    public static Int32 Speed;
    private static Int32 fps;
    private static Single frameTime;
    private Boolean isKeyFrame;

    public static Single FrameTime => frameTime;
    public static Single FPS => fps;

    static HonoluluBattleMain()
    {
        CurPlayerSerialNum = new Int32[4];
        CurPlayerWeaponIndex = new Int32[4];
        EnemyStartIndex = 4;
    }

    public HonoluluBattleMain()
    {
        Int32[][] numArray1 = new Int32[4][];
        numArray1[0] = new Int32[1];
        Int32 index1 = 1;
        Int32[] numArray2 = { 316, -316 };
        numArray1[index1] = numArray2;
        Int32 index2 = 2;
        Int32[] numArray3 = { 632, 0, -632 };
        numArray1[index2] = numArray3;
        Int32 index3 = 3;
        Int32[] numArray4 =
        {
            948,
            316,
            -316,
            -948
        };
        numArray1[index3] = numArray4;
        this.playerXPos = numArray1;
        this.showHideDebugUICoolDown = 0.5f;
        this.scaleEdit = String.Empty;
        this.distanceEdit = String.Empty;
    }

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 30;
        this.playerMaterials = new List<Material>();
        this.monsterMaterials = new List<Material>();
        FF9StateSystem.Battle.isFade = false;
        this.animationName = new String[8];
        this.needClampTime = false;
        if (Application.platform == RuntimePlatform.Android)
        {
            string deviceModel = SystemInfo.deviceModel;
            if (string.Compare("Asus Nexus Player", deviceModel, true) == 0)
            {
                this.needClampTime = true;
            }
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

    private void Start()
    {
        this.cameraController = GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>();
        this.InitBattleScene();
        UpdateFrameTime(FF9StateSystem.Settings.FastForwardFactor);
        GameObject gameObject1 = GameObject.Find("BattleMap Root");
        GameObject gameObject2 = new GameObject("BattleMap SPS");
        gameObject2.transform.parent = gameObject1.transform;
        battleSPS = gameObject2.AddComponent<BattleSPSSystem>();
        battleSPS.Init();
        Byte num = FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].Camera;
        FF9StateSystem.Battle.FF9Battle.seq_work_set.CameraNo = (Int32)num >= 3 ? (Byte)UnityEngine.Random.Range(0, 3) : num;
        SFX.StartBattle();

        if ((Int64)FF9StateSystem.Settings.cfg.skip_btl_camera == 0L && FF9StateSystem.Battle.isRandomEncounter)
            SFX.SkipCameraAnimation(-1);

        if (!FF9StateSystem.Battle.isNoBoosterMap())
            return;

        FF9StateSystem.Settings.IsBoosterButtonActive[0] = false;
        FF9StateSystem.Settings.SetBoosterHudToCurrentState();
        PersistenSingleton<UIManager>.Instance.Booster.SetBoosterButton(BoosterType.BattleAssistance, false);
    }

    public void InitBattleScene()
    {
        FF9StateGlobal FF9 = FF9StateSystem.Common.FF9;
        FF9.charArray.Clear();
        this.btlScene = FF9StateSystem.Battle.FF9Battle.btl_scene = new BTL_SCENE();
        Debug.Log("battleID = " + FF9StateSystem.Battle.battleMapIndex);
        Dictionary<String, Int32> source = FF9BattleDB.SceneData;

        battleSceneName = source.FirstOrDefault(p => p.Value == FF9StateSystem.Battle.battleMapIndex).Key;
        battleSceneName = battleSceneName.Substring(4);
        Debug.Log("battleSceneName = " + battleSceneName);
        this.btlScene.ReadBattleScene(battleSceneName);
        this.StartCoroutine(PersistenSingleton<FF9TextTool>.Instance.UpdateBattleText(FF9BattleDB.SceneData["BSC_" + battleSceneName]));
        WMProfiler.Begin("Start Load Text");
        String battleModelPath = FF9BattleDB.MapModel["BSC_" + battleSceneName];
        FF9StateSystem.Battle.FF9Battle.map.btlBGPtr = ModelFactory.CreateModel("BattleMap/BattleModel/battleMap_all/" + battleModelPath + "/" + battleModelPath, Vector3.zero, Vector3.zero, true);
        FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum = FF9StateSystem.Battle.isDebug ? FF9StateSystem.Battle.patternIndex : (Byte)this.ChoicePattern();
        btlseq.ReadBattleSequence(battleSceneName);
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
        if (battleSceneName == "EF_E006" || battleSceneName == "EF_E007")
        {
            BTL_DATA btlData1 = FF9StateSystem.Battle.FF9Battle.btl_data[4];
            BTL_DATA btlData2 = FF9StateSystem.Battle.FF9Battle.btl_data[5];
            this.GeoBattleAttach(btlData2.gameObject, btlData1.gameObject, 55);
            btlData2.attachOffset = 100;
        }
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        GEOTEXHEADER geotexheader = new GEOTEXHEADER();
        geotexheader.ReadBGTextureAnim(battleModelPath);
        stateBattleSystem.map.btlBGTexAnimPtr = geotexheader;
        BBGINFO bbginfo = new BBGINFO();
        bbginfo.ReadBattleInfo(battleModelPath);
        FF9StateSystem.Battle.FF9Battle.map.btlBGInfoPtr = bbginfo;
        btlshadow.ff9battleShadowInit(13);
        battle.InitBattleMap();
        this.seqList = new List<Int32>();
        SB2_PATTERN sb2Pattern = this.btlScene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum];
        Int32[] numArray = new Int32[sb2Pattern.MonCount];
        for (Int32 index = 0; index < (Int32)sb2Pattern.MonCount; ++index)
            numArray[index] = sb2Pattern.Put[index].TypeNo;
        foreach (Int32 num in numArray.Distinct().ToArray())
        {
            for (Int32 index1 = 0; index1 < btlseq.instance.sequenceProperty.Length; ++index1)
            {
                SequenceProperty sequenceProperty = btlseq.instance.sequenceProperty[index1];
                if (sequenceProperty.Montype == num)
                {
                    for (Int32 index2 = 0; index2 < sequenceProperty.PlayableSequence.Count; ++index2)
                        this.seqList.Add(sequenceProperty.PlayableSequence[index2]);
                }
            }
        }
        this.btlIDList = FF9StateSystem.Battle.FF9Battle.seq_work_set.AnmOfsList.Distinct().ToArray();
        this.battleState = BattleState.PlayerTurn;
        playerEnterCommand = false;
        this.playerCastingSkill = false;
        this.enemyEnterCommand = false;
    }

    private void CreateBattleData(FF9StateGlobal FF9)
    {
        BTL_DATA[] btlDataArray = btlseq.instance.btl_list = FF9StateSystem.Battle.FF9Battle.btl_data;
        Int32 index1 = 0;
        for (Int32 index2 = index1; index2 < 4; ++index2)
        {
            btlDataArray[index2] = new BTL_DATA();
            if (FF9.party.member[index2] != null)
            {
                Byte num = FF9.party.member[index2].info.serial_no;
                BattlePlayerCharacter.CreatePlayer(btlDataArray[index1], (BattlePlayerCharacter.PlayerSerialNumber)num);
                Int32 length = 0;
                IEnumerator enumerator = btlDataArray[index1].gameObject.transform.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        if ((enumerator.Current as UnityEngine.Object)?.name.Contains("mesh") == true)
                            ++length;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    disposable?.Dispose();
                }
                btlDataArray[index1].meshIsRendering = new Boolean[length];
                for (Int32 index3 = 0; index3 < length; ++index3)
                    btlDataArray[index1].meshIsRendering[index3] = true;
                btlDataArray[index1].meshCount = length;
                btlDataArray[index1].animation = btlDataArray[index1].gameObject.GetComponent<Animation>();
                ++index1;
            }
            btlDataArray[index2].typeNo = 5;
            btlDataArray[index2].idleAnimationName = this.animationName[index2];
        }
        for (Int32 index2 = 4; index2 < 4 + (Int32)this.btlScene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].MonCount; ++index2)
        {
            SB2_PATTERN sb2Pattern = this.btlScene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum];
            Byte num = sb2Pattern.Put[index2 - 4].TypeNo;
            SB2_MON_PARM sb2MonParm = this.btlScene.MonAddr[num];
            String path = FF9BattleDB.GEO.GetValue(sb2MonParm.Geo);
            //var vector3 = new Vector3(sb2Pattern.Put[index2 - 4].Xpos, sb2Pattern.Put[index2 - 4].Ypos * -1, sb2Pattern.Put[index2 - 4].Zpos);
            btlDataArray[index2] = new BTL_DATA { gameObject = ModelFactory.CreateModel(path, true) };
            if (ModelFactory.IsUseAsEnemyCharacter(path))
            {
                if (path.Contains("GEO_MON_B3_168"))
                    btlDataArray[index2].gameObject.transform.FindChild("mesh5").gameObject.SetActive(false);
                btlDataArray[index2].weapon_geo = ModelFactory.CreateDefaultWeaponForCharacterWhenUseAsEnemy(path);
                MeshRenderer[] componentsInChildren = btlDataArray[index2].weapon_geo.GetComponentsInChildren<MeshRenderer>();
                btlDataArray[index2].weaponMeshCount = componentsInChildren.Length;
                btlDataArray[index2].weaponRenderer = new Renderer[btlDataArray[index2].weaponMeshCount];
                for (Int32 index3 = 0; index3 < btlDataArray[index2].weaponMeshCount; ++index3)
                    btlDataArray[index2].weaponRenderer[index3] = componentsInChildren[index3].GetComponent<Renderer>();
                geo.geoAttach(btlDataArray[index2].weapon_geo, btlDataArray[index2].gameObject, ModelFactory.GetDefaultWeaponBoneIdForCharacterWhenUseAsEnemy(path));
            }
            Int32 length = 0;
            IEnumerator enumerator = btlDataArray[index2].gameObject.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if ((enumerator.Current as UnityEngine.Object)?.name.Contains("mesh") == true)
                        ++length;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                disposable?.Dispose();
            }
            btlDataArray[index2].meshIsRendering = new Boolean[length];
            for (Int32 index3 = 0; index3 < length; ++index3)
                btlDataArray[index2].meshIsRendering[index3] = true;
            btlDataArray[index2].meshCount = length;
            btlDataArray[index2].animation = btlDataArray[index2].gameObject.GetComponent<Animation>();
            btlDataArray[index2].animation = btlDataArray[index2].gameObject.GetComponent<Animation>();
            btlDataArray[index2].typeNo = num;
            btlDataArray[index2].idleAnimationName = this.animationName[index2];
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

    public static void UpdateFrameTime(Int32 newSpeed)
    {
        Speed = newSpeed;
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            if (btlData.gameObject == null)
                return;

            //foreach (object item in btlData.gameObject.GetComponent<Animation>())
            //{
            //    // do nothing?
            //}
        }
        fps = Configuration.Graphics.BattleFPS * newSpeed;
        frameTime = 1f / fps;
    }

    public static void playCommand(Int32 characterNo, Int32 slotNo, Int32 target, Boolean isTrance = false)
    {
        if (slotNo < 0 || slotNo > 6)
        {
            Debug.LogError("slot number value can be only 0 to 5");
        }
        else if (characterNo < 0 || characterNo > 4)
        {
            Debug.LogError("character number value can be only 1 to 4");
        }
        else
        {
            BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_data[characterNo];
            Byte presetId = FF9StateSystem.Common.FF9.party.member[characterNo].info.menu_type;
            BattleCommandId commandId = 0;
            Int32 commandIndex = (Int32)commandId;
            UInt32 sub_no = 0;
            switch (slotNo)
            {
                case 0:
                    commandId = BattleCommandId.Attack;
                    sub_no = CharacterCommands.Commands[commandIndex].Ability;
                    break;
                case 1:
                    commandId = CharacterCommands.CommandSets[presetId].Get(isTrance, 0);
                    sub_no = CharacterCommands.Commands[commandIndex].Ability;
                    break;
                case 2:
                    commandId = CharacterCommands.CommandSets[presetId].Get(isTrance, 1);
                    sub_no = CharacterCommands.Commands[commandIndex].Ability;
                    break;
                case 3:
                    commandId = BattleCommandId.Item;
                    sub_no = 236U;
                    break;
                case 4:
                    commandId = BattleCommandId.Defend;
                    break;
                case 5:
                    commandId = BattleCommandId.Change;
                    sub_no = CharacterCommands.Commands[commandIndex].Ability;
                    break;
            }

            if (CharacterCommands.Commands[commandIndex].Type == CharacterCommandType.Throw)
                sub_no = 1U;

            if (commandId == BattleCommandId.None)
                return;

            CMD_DATA cmd = new CMD_DATA { regist = btlData };
            btl_cmd.SetCommand(cmd, commandId, sub_no, (UInt16)target, 0U);
        }
    }

    private void YMenu_ManagerActiveTime()
    {
        BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next;
        if (UIManager.Battle.FF9BMenu_IsEnableAtb())
            ProcessActiveTime(btl);
    }

    private void ProcessActiveTime(BTL_DATA btl)
    {
        Int32 battleSpeed = Configuration.Battle.Speed;

        BTL_DATA source = btl;
        Boolean canContinute = false;
        Boolean needContinue = battleSpeed > 0;
        do
        {
            for (btl = source; btl != null; btl = btl.next)
            {
                if (btl.cur.hp == 0)
                    continue;

                if (btl.sel_mode != 0 || btl.sel_menu != 0 || btl.bi.atb == 0)
                {
                    // We need to refresh status for any alive characters in the fast or the turn-based mode
                    if (needContinue)
                    {
                        // ============ Warning ============
                        btl_para.CheckPointData(btl);
                        btl_stat.CheckStatusLoop(btl, true);
                        // =================================
                    }

                    continue;
                }

                POINTS current = btl.cur;
                POINTS maximum = btl.max;

                Boolean changed = false;
                if (current.at < maximum.at)
                {
                    if (btl.bi.player != 0)
                        canContinute = true;

                    changed = true;
                    current.at += (Int16)Math.Max(1, current.at_coef * 4 );
                }

                if (needContinue)
                {
                    // ============ Warning ============
                    btl_para.CheckPointData(btl);
                    btl_stat.CheckStatusLoop(btl, true);
                    // =================================
                }

                if (current.at < maximum.at)
                    continue;

                if (battleSpeed == 2)
                {
                    if (btl.bi.player != 0)
                    {
                        if (changed)
                        {
                            BattleHUD.ForceNextTurn = false;
                            needContinue = false;
                        }
                    }
                    else
                    {
                        if (!btl_stat.CheckStatus(btl, BattleStatus.Venom | BattleStatus.Sleep | BattleStatus.Freeze))
                            needContinue = false;
                    }
                }
                else
                {
                    needContinue = false;
                }

                if (!Configuration.Fixes.IsKeepRestTimeInBattle)
                    current.at = maximum.at;

                if (btl_stat.CheckStatus(btl, BattleStatus.Venom | BattleStatus.Sleep | BattleStatus.Freeze))
                    continue;

                if (btl.bi.player != 0)
                {
                    if (btl_stat.CheckStatus(btl, BattleStatus.Confuse | BattleStatus.Berserk))
                    {
                        Int32 num = 0;
                        while (1 << num != btl.btl_id)
                            ++num;
                        if (!UIManager.Battle.InputFinishList.Contains(num))
                        {
                            btl.sel_mode = 1;
                            btl_cmd.SetAutoCommand(btl);
                        }
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
                else if (!FF9StateSystem.Battle.isDebug)
                {
                    if (PersistenSingleton<EventEngine>.Instance.RequestAction(BattleCommandId.EnemyAtk, btl.btl_id, 0, 0))
                        btl.sel_mode = 1;
                }
                else
                {
                    if (Array.IndexOf(FF9StateSystem.Battle.FF9Battle.seq_work_set.AnmOfsList, this.btlIDList[btl_scrp.FindBattleUnit(btl.btl_id).Data.typeNo]) < 0)
                        Debug.LogError("Index out of range");

                    UnityEngine.Random.Range(0, 4);
                    if (FF9StateSystem.Battle.FF9Battle.btl_phase != 4)
                        break;
                }
            }
        } while (canContinute && needContinue);
    }

    private void Update()
    {
        this.UpdateAttachModel();
        UpdateFrames();
        UpdateData();
    }

    private void UpdateFrames()
    {
        this.cumulativeTime += Time.deltaTime;
        if (this.needClampTime)
            this.cumulativeTime = Mathf.Min(this.cumulativeTime, HonoluluBattleMain.frameTime * (float)SettingsState.FastForwardGameSpeed * 1.2f);

        while (this.cumulativeTime >= (Double)frameTime)
        {
            this.cumulativeTime -= frameTime;
            battleSPS.Service();
            if (!IsOver)
                UpdateBattleFrame();
            else
                UpdateOverFrame();
        }
    }

    private void UpdateBattleFrame()
    {
        if (IsPaused)
            return;

        this.battleResult = battle.BattleMain();
        if (!FF9StateSystem.Battle.isDebug)
        {
            if (UIManager.Battle.FF9BMenu_IsEnable())
                this.YMenu_ManagerActiveTime();

            if ((Int64)this.battleResult == 1L)
            {
                PersistenSingleton<FF9StateSystem>.Instance.mode = 8;
                IsOver = true;
            }
        }
        SceneDirector.ServiceFade();
    }

    private static void UpdateOverFrame()
    {
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        switch (ff9StateGlobal.btl_result)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 7:
                if (ff9StateGlobal.btl_result == 2)
                    ff9StateGlobal.btl_result = 1;
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
            case 6:
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
                btlseq.FF9DrawShadowCharBattle(stateBattleSystem.map.shadowArray, btl.bi.slot_no, 0, BoneNo);
            }
            else if (btl.die_seq < 4)
            {
                btlseq.FF9DrawShadowCharBattle(stateBattleSystem.map.shadowArray, 9 + btl.bi.slot_no, 0, BoneNo);
            }
        }
    }

    private void UpdateAttachModel()
    {
        if (battleSceneName != "EF_E006" && battleSceneName != "EF_E007")
            return;
        //GameObject gameObject1 = FF9StateSystem.Battle.FF9Battle.btl_data[4].gameObject;
        GameObject gameObject2 = FF9StateSystem.Battle.FF9Battle.btl_data[5].gameObject;
        gameObject2.transform.localPosition = Vector3.zero;
        gameObject2.transform.localRotation = Quaternion.identity;
        gameObject2.transform.localScale = Vector3.one;
        this.rootBone.transform.localPosition = Vector3.zero;
        this.rootBone.transform.localRotation = Quaternion.identity;
        this.rootBone.transform.localScale = Vector3.one;
    }

    public void GeoBattleAttach(GameObject src, GameObject target, Int32 boneIndex)
    {
        this.rootBone = src.transform.GetChildByName("bone000");
        Transform childByName = target.transform.GetChildByName("bone" + boneIndex.ToString("D3"));
        src.transform.parent = childByName.transform;
        src.transform.localPosition = Vector3.zero;
        src.transform.localRotation = Quaternion.identity;
        src.transform.localScale = Vector3.one;
        this.rootBone.transform.localPosition = Vector3.zero;
        this.rootBone.transform.localRotation = Quaternion.identity;
        this.rootBone.transform.localScale = Vector3.one;
    }

    private void LateUpdate()
    {
        this.UpdateAttachModel();
        UIManager.Battle.modelButtonManager.UpdateModelButtonPosition();
        Singleton<HUDMessage>.Instance.UpdateChildPosition();
    }

    public Int32 GetPlayerSerialNum(Int32 battlePlayerPosID)
    {
        return CurPlayerSerialNum[battlePlayerPosID];
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
        {
            if (btlData.animation != null)
                btlData.animation.enabled = false;
        }
    }

    private void ResumeBattle()
    {
        IsPaused = false;
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            if (btlData.animation != null)
                btlData.animation.enabled = true;
        }
    }

    public enum BattleState
    {
        PlayerTurn,
        EnemyTurn,
    }

    #region Attributes

    private static void BeginInitialize()
    {
        FF9StateSystem.Battle.FF9Battle.attr = (UInt16)Attributes.None;
    }

    private static void EndInitialize()
    {
        IsInitialized = true;
    }

    private static Boolean IsInitialized
    {
        get { return (FF9StateSystem.Battle.FF9Battle.attr & (UInt16)Attributes.Initialized) == (UInt16)Attributes.Initialized; }
        set
        {
            if (value)
                FF9StateSystem.Battle.FF9Battle.attr |= (UInt16)Attributes.Initialized;
            else
                FF9StateSystem.Battle.FF9Battle.attr &= (UInt16)~Attributes.Initialized;
        }
    }

    private static Boolean IsPaused
    {
        get { return (FF9StateSystem.Battle.FF9Battle.attr & (UInt16)Attributes.Paused) == (UInt16)Attributes.Paused; }
        set
        {
            if (value)
                FF9StateSystem.Battle.FF9Battle.attr |= (UInt16)Attributes.Paused;
            else
                FF9StateSystem.Battle.FF9Battle.attr &= (UInt16)~Attributes.Paused;
        }
    }

    private static Boolean IsOver
    {
        get { return (FF9StateSystem.Battle.FF9Battle.attr & (UInt16)Attributes.Over) == (UInt16)Attributes.Over; }
        set
        {
            if (value)
                FF9StateSystem.Battle.FF9Battle.attr |= (UInt16)Attributes.Over;
            else
                FF9StateSystem.Battle.FF9Battle.attr &= (UInt16)~Attributes.Over;
        }
    }

    [Flags]
    private enum Attributes : ushort
    {
        None = 0,
        Initialized = 1,
        Paused = 256,
        Over = 4096
    }

    #endregion
}