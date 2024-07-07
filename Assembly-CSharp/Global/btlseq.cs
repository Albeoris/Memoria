using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = System.Object;

public class btlseq
{
    public static void ReadBattleSequence(String name)
    {
        ReadBattleSequence(name, ref instance, true);
    }

    public static void ReadBattleSequence(String name, ref btlseqinstance inst, Boolean useGlobalWorkSet = false)
    {
        inst.enemy = FF9StateSystem.Battle.FF9Battle.enemy;
        String path = $"BattleMap/BattleScene/EVT_BATTLE_{name}/{FF9BattleDB.SceneData["BSC_" + name]}.raw17";
        inst.data = AssetManager.LoadBytes(path);
        if (inst.data == null)
            return;
        if (useGlobalWorkSet)
            inst.seq_work_set = FF9StateSystem.Battle.FF9Battle.seq_work_set;
        else
            inst.seq_work_set = new SEQ_WORK_SET();
        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(inst.data)))
        {
            Int16 seqBlockOffset = binaryReader.ReadInt16();
            inst.camOffset = binaryReader.ReadInt16();
            Int16 seqCount = binaryReader.ReadInt16();
            Int16 animCount = binaryReader.ReadInt16();
            Int32[] seqOffset = new Int32[seqCount];
            for (Int32 i = 0; i < seqCount; i++)
                seqOffset[i] = binaryReader.ReadInt16();
            Int32[] animList = new Int32[animCount];
            for (Int32 i = 0; i < animCount; i++)
                animList[i] = binaryReader.ReadInt32();
            Byte[] seqBaseAnim = new Byte[seqCount];
            for (Int32 i = 0; i < seqCount; i++)
                seqBaseAnim[i] = binaryReader.ReadByte();
            inst.seq_work_set.SeqData = seqOffset;
            inst.seq_work_set.AnmAddrList = animList;
            inst.seq_work_set.AnmOfsList = seqBaseAnim;
            Byte[] seqDistinctIndices = seqBaseAnim.Distinct<Byte>().ToArray<Byte>();
            ChangeToSequenceListNumber(seqDistinctIndices);
            Byte[] seqDistinctMonType = new Byte[seqBaseAnim.Length]; // Surely this is convoluted
            Array.Copy(seqBaseAnim, seqDistinctMonType, seqBaseAnim.Length);
            ChangeToSequenceListNumber(seqDistinctMonType);
            inst.sequenceProperty = new SequenceProperty[seqDistinctIndices.Length];
            for (Int32 i = 0; i < seqDistinctIndices.Length; i++)
            {
                inst.sequenceProperty[i] = new SequenceProperty();
                inst.sequenceProperty[i].Montype = seqDistinctIndices[i];
            }
            for (Int32 i = 0; i < seqCount; i++)
            {
                binaryReader.BaseStream.Seek(seqOffset[i] + 4, SeekOrigin.Begin);
                for (Int32 j = 0; j < seqDistinctIndices.Length; j++)
                    if (inst.sequenceProperty[j].Montype == seqDistinctMonType[i])
                        inst.sequenceProperty[j].PlayableSequence.Add(i);
            }
        }
    }

    private static void ChangeToSequenceListNumber(Byte[] list)
    {
        Byte[] distinct = list.Distinct<Byte>().ToArray<Byte>();
        for (Int32 i = 0; i < list.Length; i++)
            for (Int32 j = 0; j < distinct.Length; j++)
                if (list[i] == distinct[j])
                    list[i] = (Byte)j;
    }

    public static void InitSequencer()
    {
        SEQ_WORK[] seqWork = instance.seq_work_set.SeqWork;
        for (Int32 i = 0; i < 4; i++)
        {
            seqWork[i] = new SEQ_WORK();
            seqWork[i].CmdPtr = (CMD_DATA)null;
        }
    }

    public static void StartBtlSeq(Int32 pBtlID, Int32 pTarID, Int32 pSeqNo)
    {
        if (instance.seq_work_set.SeqData[pSeqNo] == 0)
            return;
        BTL_DATA next;
        for (next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
            if ((next.btl_id & pBtlID) != 0)
                break;
        if (next == null)
            return;
        CMD_DATA cmd_DATA = next.cmd[0];
        cmd_DATA.cmd_no = BattleCommandId.None;
        cmd_DATA.sub_no = pSeqNo;
        cmd_DATA.tar_id = (UInt16)pTarID;
        cmd_DATA.regist = next;
        cmd_DATA.SetAAData(FF9StateSystem.Battle.FF9Battle.enemy_attack[pSeqNo]);
        cmd_DATA.ScriptId = btl_util.GetCommandScriptId(cmd_DATA);
        cmd_DATA.info.Reset();
        cmd_DATA.IsShortRange = btl_util.IsAttackShortRange(cmd_DATA);
        if (Configuration.Battle.SFXRework)
        {
            UnifiedBattleSequencer.BattleAction action = new UnifiedBattleSequencer.BattleAction(UnifiedBattleSequencer.EffectType.EnemySequence, pSeqNo);
            action.Execute(cmd_DATA);
        }
        else
        {
            SEQ_WORK seq_WORK;
            if ((seq_WORK = btlseq.EntrySequence(cmd_DATA)) == null)
                return;
            seq_WORK.Flags.EventMode = true;
        }
    }

    public static Boolean BtlSeqBusy()
    {
        if (Configuration.Battle.SFXRework)
        {
            for (Int32 i = 0; i < UnifiedBattleSequencer.runningActions.Count; i++)
                if (UnifiedBattleSequencer.runningActions[i].cmd.regist.bi.player == 0)
                    return true;
            return false;
        }
        SEQ_WORK[] seqWork = FF9StateSystem.Battle.FF9Battle.seq_work_set.SeqWork;
        for (Int16 i = 0; i < 4; i++)
            if (seqWork[i].CmdPtr != null)
                return true;
        return false;
    }

    public static void RunSequence(CMD_DATA pCmd)
    {
        if (FF9StateSystem.Battle.FF9Battle.seq_work_set.SeqData[pCmd.sub_no] == 0)
            return;
        if (btlseq.EntrySequence(pCmd) == null)
            return;
        if (pCmd.regist.bi.player == 0)
        {
            Vector3 eulerAngles = pCmd.regist.rot.eulerAngles;
            pCmd.regist.rot.eulerAngles = new Vector3(eulerAngles.x, pCmd.regist.evt.rotBattle.eulerAngles.y, eulerAngles.z);
        }
    }

    public static SEQ_WORK EntrySequence(CMD_DATA pCmd)
    {
        SEQ_WORK[] seqWork = instance.seq_work_set.SeqWork;
        Int16 num;
        for (num = 0; num < 4; num++)
            if (seqWork[num].CmdPtr == null)
                break;
        if (num >= 4)
            return (SEQ_WORK)null;
        seqWork[num].Flags = new SeqFlag();
        seqWork[num].CmdPtr = pCmd;
        seqWork[num].CurPtr = instance.seq_work_set.SeqData[pCmd.sub_no];
        seqWork[num].OldPtr = 0;
        seqWork[num].IncCnt = 0;
        seqWork[num].DecCnt = 0;
        seqWork[num].AnmCnt = 0;
        seqWork[num].AnmIDOfs = instance.seq_work_set.AnmOfsList[pCmd.sub_no];
        seqWork[num].SfxTime = 0;
        seqWork[num].TurnTime = 0;
        seqWork[num].SVfxTime = 0;
        seqWork[num].FadeTotal = 0;
        FF9StateSystem.Battle.FF9Battle.seq_work_set.CameraNo = 0;
        return seqWork[num];
    }

    public static void Sequencer()
    {
        instance.wSeqCode = 0;
        SEQ_WORK[] seqWork = instance.seq_work_set.SeqWork;
        for (Int32 i = 0; i < 4; i++)
        {
            if (seqWork[i].CmdPtr != null)
            {
                seqWork[i].IncCnt++;
                seqWork[i].DecCnt--;
                seqWork[i].AnmCnt++;
                BTL_DATA regist = seqWork[i].CmdPtr.regist;
                Int32 continueSeq = 1;
                using (instance.sequenceReader = new BinaryReader(new MemoryStream(instance.data)))
                {
                    while (continueSeq != 0)
                    {
                        instance.sequenceReader.BaseStream.Seek(seqWork[i].CurPtr + 4, SeekOrigin.Begin);
                        instance.wSeqCode = instance.sequenceReader.ReadByte();
                        if (instance.wSeqCode > btlseq.gSeqProg.Length)
                            instance.wSeqCode = 0;
                        if (seqWork[i].Flags.WaitLoadVfx && SFX.GetTaskMonsteraStartOK() != 0)
                        {
                            seqWork[i].Flags.DoneLoadVfx = true;
                            seqWork[i].Flags.WaitLoadVfx = false;
                        }
                        if (seqWork[i].CurPtr != seqWork[i].OldPtr && btlseq.gSeqProg[instance.wSeqCode].Init != null)
                            btlseq.gSeqProg[instance.wSeqCode].Init(seqWork[i], regist);
                        seqWork[i].OldPtr = seqWork[i].CurPtr;
                        continueSeq = btlseq.gSeqProg[instance.wSeqCode].Exec(seqWork[i], regist);
                    }
                }
                if (seqWork[i].TurnTime != 0)
                {
                    Vector3 eulerAngles = regist.rot.eulerAngles;
                    eulerAngles.y = seqWork[i].TurnOrg + seqWork[i].TurnRot * (Single)seqWork[i].TurnCnt / (Single)seqWork[i].TurnTime;
                    regist.rot = Quaternion.Euler(eulerAngles);
                    if (seqWork[i].TurnCnt++ >= seqWork[i].TurnTime)
                        seqWork[i].TurnTime = 0;
                }
                if (seqWork[i].SfxTime != 0)
                    if (--seqWork[i].SfxTime == 0)
                        btl_util.SetBattleSfx(regist, seqWork[i].SfxNum, seqWork[i].SfxVol);
                if (seqWork[i].SVfxTime != 0)
                {
                    if (--seqWork[i].SVfxTime == 0)
                    {
                        Int16[] arg = new Int16[]
                        {
                            seqWork[i].SVfxParam,
                            0,
                            0,
                            0
                        };
                        btl_vfx.SetBattleVfx(seqWork[i].CmdPtr, (SpecialEffect)seqWork[i].SVfxNum, arg);
                    }
                }
                if (seqWork[i].FadeTotal != 0)
                {
                    seqWork[i].FadeStep--;
                    btl_util.SetEnemyFadeToPacket(regist, seqWork[i].FadeStep * 32 / seqWork[i].FadeTotal);
                    if (seqWork[i].FadeStep == 0)
                        seqWork[i].FadeTotal = 0;
                }
            }
        }
    }

    public static void MonsterTransformFading(BTL_DATA btl) // Maybe have a smooth fading in/out
    {
        if (btl.monster_transform.fade_counter == 0)
        {
            btl_mot.ShowMesh(btl, UInt16.MaxValue);
        }
        else
        {
            Single alpha = 128 - 16 * btl.monster_transform.fade_counter;
            if (btl.bi.player != 0 && btl.weapon_geo != null && (btl.weaponFlags & geo.GEO_FLAGS_RENDER) != 0 && (btl.weaponFlags & geo.GEO_FLAGS_CLIP) == 0)
            {
                btl_stat.GeoAddColor2DrawPacket(btl.weapon_geo, (Int16)(alpha - 128), (Int16)(alpha - 128), (Int16)(alpha - 128));
                if (alpha < 70)
                    btl_util.GeoSetABR(btl.weapon_geo, "GEO_POLYFLAGS_TRANS_100_PLUS_25");
            }
            if ((btl.flags & geo.GEO_FLAGS_RENDER) != 0 && (btl.flags & geo.GEO_FLAGS_CLIP) == 0)
            {
                btl_stat.GeoAddColor2DrawPacket(btl.gameObject, (Int16)(alpha - 128), (Int16)(alpha - 128), (Int16)(alpha - 128));
                if (alpha < 70)
                    btl_util.GeoSetABR(btl.gameObject, "GEO_POLYFLAGS_TRANS_100_PLUS_25");
            }
        }
    }

    public static void DispCharacter(BTL_DATA btl, Boolean advanceAnim = true)
    {
        if (btl.bi.slave != 0)
        {
            btl_mot.DieSequence(btl);
            BattleUnit masterEnemyBtlPtr = btl_util.GetMasterEnemyBtlPtr();
            if (masterEnemyBtlPtr == null)
                return;
            btl.rot = masterEnemyBtlPtr.Data.rot;
            btl_mot.setSlavePos(btl, ref btl.base_pos);
            btl_mot.setBasePos(btl);
            return;
        }
        if (btl.bi.player != 0 && btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE))
        {
            Vector3 eulerAngles = btl.rot.eulerAngles;
            eulerAngles.y = 180f;
            btl.gameObject.transform.localRotation = Quaternion.Euler(eulerAngles);
        }
        else
        {
            btl.gameObject.transform.localRotation = btl.rot;
        }
        if (!HonoluluBattleMain.IsAttachedModel(btl))
            btl.gameObject.transform.localPosition = btl.pos;
        btl_mot.PlayAnim(btl);
        Boolean reverseSpeed = btl.animSpeed < 0f;
        Int32 animLoopFrame = GeoAnim.getAnimationLoopFrame(btl);
        if (!btl_mot.IsAnimationFrozen(btl))
        {
            if (btl.animation != null)
                btl.animation.enabled = true;
            if (advanceAnim)
                btl.animFrameFrac += Math.Abs(btl.animSpeed);

            while (btl.animFrameFrac >= 1f)
            {
                btl.animFrameFrac--;
                if (reverseSpeed && btl.evt.animFrame >= 0)
                    btl.evt.animFrame--;
                else if (!reverseSpeed && btl.evt.animFrame <= animLoopFrame)
                    btl.evt.animFrame++;
            }
        }
        else if (btl.animation != null)
        {
            btl.animation.enabled = false;
        }
        btl.animEndFrame = false;

        if (btl_mot.IsAnimationFrozen(btl) || btl.evt.animFrame > animLoopFrame)
        {
            btl.endedAnimationName = btl.currentAnimationName;
            if (btl.bi.dmg_mot_f != 0)
            {
                btl.pos[2] = btl.base_pos[2];
                btl.bi.dmg_mot_f = 0;
            }
            if (btl_stat.CheckStatus(btl, BattleStatus.Death) && btl.die_seq == 0 && (btl.bi.player != 0 || btl_util.getEnemyPtr(btl).info.die_atk == 0 || !btl_util.IsBtlBusy(btl, btl_util.BusyMode.CASTER | btl_util.BusyMode.QUEUED_CASTER)))
                btl.die_seq = 1;
            if ((btl.animFlag & EventEngine.afHold) != 0)
                btl.animFlag = (UInt16)EventEngine.afFreeze;
            if (!btl_mot.IsAnimationFrozen(btl))
            {
                if ((btl.animFlag & EventEngine.afPalindrome) != 0)
                {
                    btl.animSpeed = -btl.animSpeed;
                    btl.evt.animFrame = (Byte)(btl.animSpeed < 0 ? animLoopFrame + btl.animSpeed : btl.animSpeed);
                }
                else if ((btl.animFlag & EventEngine.afLoop) != 0)
                    btl.evt.animFrame = (Byte)(reverseSpeed ? animLoopFrame : 0);
                else
                    btl_mot.SetDefaultIdle(btl, true);
                if ((btl.animFlag & EventEngine.afLoop) == 0)
                    btl.animFlag &= (UInt16)~EventEngine.afPalindrome;
            }
            btl.evt.animFrame = Math.Max(btl.evt.animFrame, (Byte)0);
            btl.evt.animFrame = Math.Min(btl.evt.animFrame, (Byte)animLoopFrame);
            btl.animEndFrame = true;
        }
        if (btl.monster_transform != null && btl.monster_transform.fade_counter > 0)
        {
            btl.monster_transform.fade_counter--;
            MonsterTransformFading(btl);
        }
        Int32 meshcnt = btl.meshCount;
        Int32 meshoffcnt = 0;
        Int32 meshoncnt = 0;
        btl.flags &= (UInt16)~geo.GEO_FLAGS_RENDER;
        for (Int32 i = 0; i < meshcnt; i++)
        {
            if (geo.geoMeshChkFlags(btl, i) == 0)
            {
                btl.flags |= geo.GEO_FLAGS_RENDER;
                btl.SetIsEnabledMeshRenderer(i, true);
                meshoncnt++;
            }
            else
            {
                btl.SetIsEnabledMeshRenderer(i, false);
                meshoffcnt++;
            }
        }
        if (meshoffcnt == meshcnt)
        {
            btl.SetIsEnabledBattleModelRenderer(false);
            if (FF9BattleDB.GEO.TryGetValue(btl.dms_geo_id, out String modelName) && ModelFactory.garnetShortHairTable.Contains(modelName))
            {
                Renderer[] longHairRenderers = btl.gameObject.transform.GetChildByName("long_hair").GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in longHairRenderers)
                    renderer.enabled = false;
                Renderer[] shortHairRenderers = btl.gameObject.transform.GetChildByName("short_hair").GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in shortHairRenderers)
                    renderer.enabled = false;
            }
        }
        if (meshoncnt == meshcnt)
        {
            btl.SetIsEnabledBattleModelRenderer(true);
            CharacterSerialNumber serialNumber = btl_util.getSerialNumber(btl);
            if (FF9BattleDB.GEO.TryGetValue(btl.dms_geo_id, out String modelName) && ModelFactory.garnetShortHairTable.Contains(modelName))
            {
                if (Configuration.Graphics.GarnetHair != 2 && (serialNumber == CharacterSerialNumber.GARNET_LH_ROD || serialNumber == CharacterSerialNumber.GARNET_LH_KNIFE || Configuration.Graphics.GarnetHair == 1))
                {
                    Renderer[] longHairRenderers = btl.gameObject.transform.GetChildByName("long_hair").GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in longHairRenderers)
                        renderer.enabled = true;
                }
                else
                {
                    Renderer[] shortHairRenderers = btl.gameObject.transform.GetChildByName("short_hair").GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in shortHairRenderers)
                        renderer.enabled = true;
                }
            }
            else if (btl.bi.player != 0 && btl.bi.slot_no == (Byte)CharacterId.Zidane && serialNumber == CharacterSerialNumber.ZIDANE_SWORD)
            {
                btl.SetIsEnabledBattleModelRenderer(false);
            }
        }
        // TODO (was check !BattleStatus.Jump, so verify it is OK to check disappear instead)
        if (btl.bi.disappear == 0)
        {
            GeoTexAnim.geoTexAnimService(btl.texanimptr);
            GeoTexAnim.geoTexAnimService(btl.tranceTexanimptr);
        }
        if (btl.weapon_geo != null)
        {
            btl.weaponFlags &= (UInt16)~geo.GEO_FLAGS_RENDER;
            for (Int32 n = 0; n < btl.weaponMeshCount; n++)
            {
                if (geo.geoWeaponMeshChkFlags(btl, n) == 0)
                {
                    btl.weaponFlags |= geo.GEO_FLAGS_RENDER;
                    btl.weaponRenderer[n].enabled = true;
                }
                else
                {
                    btl.weaponRenderer[n].enabled = false;
                }
            }
        }
    }

    public static void FF9DrawShadowCharBattle(GameObject shadowObj, BTL_DATA btl, Int32 posY, Int32 BoneNo)
    {
        GameObject btlObj = btl.gameObject;
        if (shadowObj == null)
            return;
        shadowObj.SetActive(true);
        if (btlObj == null)
            global::Debug.LogError("gameObject is NULL");
        btlseq.ff9battleShadowCalculateMatrix(shadowObj, btlObj, posY, ff9btl.ff9btl_get_bonestart(BoneNo), ff9btl.ff9btl_get_boneend(BoneNo));
    }

    public static void ff9battleShadowCalculateMatrix(GameObject ObjPtr, GameObject CharPtr, Int32 PosY, Int32 BoneStartNo, Int32 BoneEndNo)
    {
        Vector3 localPosition = ObjPtr.transform.localPosition;
        Transform childByName = CharPtr.transform.GetChildByName("bone" + BoneStartNo.ToString("D3"));
        if (childByName == null)
        {
            if (ObjPtr.activeSelf)
                ObjPtr.SetActive(false);
            return;
        }
        localPosition.x = childByName.position.x;
        localPosition.y = PosY + 2.5f;
        localPosition.z = childByName.position.z;
        ObjPtr.transform.localPosition = localPosition;
    }

    public static void SetupBattleScene()
    {
        BTL_SCENE btl_scene = FF9StateSystem.Battle.FF9Battle.btl_scene;
        btl_scene.Info.FieldBGM = FF9StateSystem.Battle.IsPlayFieldBGMInCurrentBattle;
        battle.btl_bonus.ap = (UInt16)FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[btl_scene.PatNum].AP;
    }

    private static Int32 SeqExecEnd(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecEnd!!!!!!!!!!!!");
        if (!pSeqWork.Flags.FinishIdle && pSeqWork.AnmCnt != 0 && pMe.animEndFrame)
        {
            btl_mot.setMotion(pMe, pMe.bi.def_idle);
            pMe.evt.animFrame = 0;
            pSeqWork.Flags.FinishIdle = true;
        }
        if (!btl_mot.IsAnimationFrozen(pMe) && !pSeqWork.Flags.FinishIdle)
            return 0;
        if (SFX.isRunning)
            return 0;
        if (pSeqWork.TurnTime != 0)
            return 0;
        if (UIManager.Battle.BtlWorkLibra)
            return 0;
        if (!pSeqWork.Flags.EventMode)
            btl_cmd.ReqFinishCommand(pSeqWork.CmdPtr);
        pSeqWork.CmdPtr = null;
        if (FF9StateSystem.Battle.isDebug)
        {
            pMe.pos = (pMe.base_pos = pMe.original_pos);
            pMe.gameObject.transform.localPosition = pMe.pos;
        }
        return 0;
    }

    public static void SeqInitWait(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqInitWait");
        pSeqWork.DecCnt = (Int16)instance.sequenceReader.ReadByte();
    }

    public static Int32 SeqExecWait(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecWait");
        if (pSeqWork.DecCnt <= 0)
        {
            pSeqWork.CurPtr += 2;
            return 1;
        }
        return 0;
    }

    public static Int32 SeqExecCalc(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecCalc");
        CMD_DATA cmdPtr = pSeqWork.CmdPtr;
        UInt16 tar_id = cmdPtr.tar_id;
        cmdPtr.info.effect_counter++;
        for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
            if ((next.btl_id & tar_id) != 0)
                btl_cmd.ExecVfxCommand(next, cmdPtr);
        pSeqWork.CurPtr++;
        return 1;
    }

    public static void SeqInitMoveToTarget(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqInitMoveToTarget");
        WK_MOVE wk_MOVE = new WK_MOVE();
        wk_MOVE.Next = 4;
        pSeqWork.IncCnt = 1;
        wk_MOVE.Frames = instance.sequenceReader.ReadByte();
        for (Int32 i = 0; i < 3; i++)
            wk_MOVE.Org[i] = (Int16)pMe.pos[i];
        wk_MOVE.Dst[1] = wk_MOVE.Org[1];
        btlseq.SeqSubTargetAveragePos(pSeqWork.CmdPtr.tar_id, out wk_MOVE.Dst[0], out wk_MOVE.Dst[2]);
        Int16 distance = instance.sequenceReader.ReadInt16();
        if (wk_MOVE.Org[0] != wk_MOVE.Dst[0] || wk_MOVE.Org[2] != wk_MOVE.Dst[2])
        {
            if (instance.wSeqCode == 30)
            {
                wk_MOVE.Dst[2] -= distance;
            }
            else
            {
                Single angle = Mathf.Atan2(wk_MOVE.Dst[0] - wk_MOVE.Org[0], wk_MOVE.Dst[2] - wk_MOVE.Org[2]);
                Single sin = Mathf.Sin(angle);
                Single cos = Mathf.Cos(angle);
                wk_MOVE.Dst[0] += (Int16)(sin * distance);
                wk_MOVE.Dst[2] += (Int16)(cos * distance);
            }
        }
        pSeqWork.Work = btlseq.SequenceConverter.WkMoveToWork(wk_MOVE);
    }

    public static Int32 SeqExecMoveToTarget(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecMoveToTarget");
        WK_MOVE wk_MOVE = btlseq.SequenceConverter.WorkToWkMove(pSeqWork.Work);
        for (Int16 i = 0; i < 3; i++)
        {
            Int16 org = wk_MOVE.Org[i];
            Int16 dst = wk_MOVE.Dst[i];
            pMe.pos[i] = (dst - org) * (Single)pSeqWork.IncCnt / (Single)wk_MOVE.Frames + org;
        }
        if (pSeqWork.IncCnt >= wk_MOVE.Frames)
            pSeqWork.CurPtr += wk_MOVE.Next;
        return 0;
    }

    public static void SeqInitMoveToTurn(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqInitMoveToTurn");
        WK_MOVE wk_MOVE = new WK_MOVE();
        wk_MOVE.Next = 2;
        pSeqWork.IncCnt = 1;
        wk_MOVE.Frames = instance.sequenceReader.ReadByte();
        for (Int16 i = 0; i < 3; i++)
        {
            wk_MOVE.Org[i] = (Int16)pMe.gameObject.transform.localPosition[i];
            wk_MOVE.Dst[i] = (Int16)pMe.base_pos[i];
        }
        pSeqWork.Work = btlseq.SequenceConverter.WkMoveToWork(wk_MOVE);
    }

    public static Int32 SeqExecAnim(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecAnim");
        Byte animCode = instance.sequenceReader.ReadByte();
        if (animCode == 255)
        {
            Int32 motId = (pMe.bi.def_idle == 0) ? 0 : 1;
            String animName = FF9StateSystem.Battle.FF9Battle.enemy[pMe.bi.slot_no].et.mot[motId];
            btl_mot.setMotion(pMe, animName);
        }
        else
        {
            Int32 animId = pSeqWork.AnmIDOfs + animCode;
            String animName = FF9BattleDB.Animation[instance.seq_work_set.AnmAddrList[animId]];
            btl_mot.setMotion(pMe, animName);
        }
        pMe.evt.animFrame = 0;
        pSeqWork.AnmCnt = 0;
        pSeqWork.CurPtr += 2;
        return 1;
    }

    public static Int32 SeqExecSVfx(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecSVfx");
        pSeqWork.SVfxNum = instance.sequenceReader.ReadUInt16();
        pSeqWork.SVfxParam = instance.sequenceReader.ReadByte();
        pSeqWork.SVfxTime = (Byte)(instance.sequenceReader.ReadByte() + 1);
        pSeqWork.CurPtr += 5;
        return 1;
    }

    public static Int32 SeqExecWaitAnim(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecWaitAnim");
        if (pSeqWork.AnmCnt != 0 && (pMe.animEndFrame || btl_mot.IsAnimationFrozen(pMe)))
        {
            SFX.SetEffCamTrigger();
            pSeqWork.CurPtr++;
            return 1;
        }
        return 0;
    }

    public static Int32 SeqExecVfx(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecVfx");
        Int16[] array = new Int16[4];
        if (SFX.GetEffectOvRun() != 0)
            return 0;
        UInt16 fx_no = instance.sequenceReader.ReadUInt16();
        array[0] = instance.sequenceReader.ReadInt16();
        array[1] = instance.sequenceReader.ReadInt16();
        array[2] = instance.sequenceReader.ReadInt16();
        array[3] = (Int16)((instance.wSeqCode != 26) ? 0 : 1);
        btl_vfx.SetBattleVfx(pSeqWork.CmdPtr, (SpecialEffect)fx_no, array);
        pSeqWork.Flags.WaitLoadVfx = true;
        pSeqWork.CurPtr += 9;
        return 1;
    }

    public static Int32 SeqExecWaitLoadVfx(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecWaitLoadVfx");
        if (pSeqWork.Flags.DoneLoadVfx)
        {
            pSeqWork.CurPtr++;
            return 1;
        }
        return 0;
    }

    public static Int32 SeqExecStartVfx(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecStartVfx");
        SFX.SetTaskMonsteraStart();
        pSeqWork.CurPtr++;
        return 1;
    }

    public static Int32 SeqExecWaitVfx(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecWaitVfx");
        if (SFX.GetEffectOvRun() == 0)
        {
            pSeqWork.CurPtr++;
            return 1;
        }
        return 0;
    }

    public static void SeqInitScale(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqInitScale");
        WK_SCALE wk_SCALE = new WK_SCALE();
        wk_SCALE.Org = (Int16)(pMe.gameObject.transform.localScale.x * 4096f);
        Int16 scaleFactor = instance.sequenceReader.ReadInt16();
        if (scaleFactor == -1)
            scaleFactor = 4096;
        else
            scaleFactor = (Int16)(wk_SCALE.Org * scaleFactor / 4096);
        wk_SCALE.Scl = (Int16)(scaleFactor - wk_SCALE.Org);
        pSeqWork.IncCnt = 1;
        wk_SCALE.Frames = instance.sequenceReader.ReadByte();
        pSeqWork.Work = btlseq.SequenceConverter.WkScaleToWork(wk_SCALE);
    }

    public static Int32 SeqExecScale(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecScale");
        WK_SCALE wk_SCALE = btlseq.SequenceConverter.WorkToWkScale(pSeqWork.Work);
        UInt16 scaleFactor = (UInt16)(wk_SCALE.Scl * pSeqWork.IncCnt / wk_SCALE.Frames + wk_SCALE.Org);
        geo.geoScaleSet(pMe, scaleFactor);
        btl_scrp.SetCharacterData(new BattleUnit(pMe), 55u, scaleFactor);
        if (scaleFactor == 4096)
            geo.geoScaleReset(pMe);
        if (pSeqWork.IncCnt >= wk_SCALE.Frames)
        {
            pSeqWork.CurPtr += 4;
            return 1;
        }
        return 0;
    }

    public static Int32 SeqExecMeshHide(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecMeshHide");
        UInt16 hiddenMesh = instance.sequenceReader.ReadUInt16();
        pMe.meshflags |= hiddenMesh;
        pMe.mesh_current |= hiddenMesh;
        btl_mot.HideMesh(pMe, hiddenMesh, false);
        pSeqWork.CurPtr += 3;
        return 1;
    }

    public static Int32 SeqExecMessage(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecMessage");
        Int32 messId = instance.sequenceReader.ReadByte();
        if ((messId & 0x80) != 0)
        {
            btlseq.BattleLog("wOfs " + pSeqWork.CmdPtr.aa.Name);
            Int32 cmdNameIndex = FF9StateSystem.Battle.FF9Battle.btl_scene.header.TypCount + pSeqWork.CmdPtr.sub_no;
            VoicePlayer.PlayBattleVoice(cmdNameIndex, pSeqWork.CmdPtr.aa.Name);
            UIManager.Battle.SetBattleTitle(pSeqWork.CmdPtr, pSeqWork.CmdPtr.aa.Name, 1);
        }
        else
        {
            messId += FF9StateSystem.Battle.FF9Battle.enemy[pMe.bi.slot_no].et.mes;
            String btlMessage = FF9TextTool.BattleText(messId);
            VoicePlayer.PlayBattleVoice(messId, btlMessage);
            if (instance.wSeqCode == 33)
                UIManager.Battle.SetBattleTitle(pSeqWork.CmdPtr, btlMessage, 4);
            else
                UIManager.Battle.SetBattleMessage(btlMessage, 4);
        }
        pSeqWork.CurPtr += 2;
        return 1;
    }

    public static Int32 SeqExecMeshShow(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecMeshShow");
        UInt16 shownMesh = instance.sequenceReader.ReadUInt16();
        pMe.meshflags &= (UInt32)~shownMesh;
        pMe.mesh_current &= (UInt16)~shownMesh;
        btl_mot.ShowMesh(pMe, shownMesh, false);
        pSeqWork.CurPtr += 3;
        return 1;
    }

    public static Int32 SeqExecSetCamera(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecSetCamera");
        instance.seq_work_set.CameraNo = instance.sequenceReader.ReadByte();
        pSeqWork.CurPtr += 2;
        return 1;
    }

    public static Int32 SeqExecDefaultIdle(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecDefaultIdle");
        btl_mot.ToggleIdleAnimation(pMe, instance.sequenceReader.ReadByte() != 0);
        pSeqWork.CurPtr += 2;
        return 1;
    }

    public static BTL_DATA SeqSubGetTarget(UInt16 pTarID)
    {
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        BTL_DATA next;
        for (next = ff9Battle.btl_list.next; next != null; next = next.next)
            if ((next.btl_id & pTarID) != 0)
                break;
        return next;
    }

    public static Int32 SeqExecRunCamera(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecRunCamera");
        SEQ_WORK_SET seq_WORK_SET = FF9StateSystem.Battle.FF9Battle.seq_work_set;
        if (instance.wSeqCode == 32 || SFX.ShouldPlayAlternateCamera(pSeqWork.CmdPtr))
        {
            Int16[] array = new Int16[3];
            btlseq.SeqSubTargetAveragePos(pSeqWork.CmdPtr.tar_id, out array[0], out array[2]);
            array[1] = 0;
            seq_WORK_SET.CamTrgCPos = new Vector3(array[0], array[1], array[2]);
            seq_WORK_SET.CamExe = pMe;
            seq_WORK_SET.CamTrg = btlseq.SeqSubGetTarget(pSeqWork.CmdPtr.tar_id);
            SFX.SetCameraTarget(seq_WORK_SET.CamTrgCPos, seq_WORK_SET.CamExe, seq_WORK_SET.CamTrg);
            seq_WORK_SET.CameraNo = instance.sequenceReader.ReadByte();
            SFX.SetEnemyCamera(pMe);
        }
        pSeqWork.CurPtr += 2;
        return 1;
    }

    public static void SeqInitMoveToPoint(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqInitMoveToPoint");
        WK_MOVE wk_MOVE = btlseq.SequenceConverter.WorkToWkMove(pSeqWork.Work);
        wk_MOVE.Next = 8;
        pSeqWork.IncCnt = 1;
        wk_MOVE.Frames = instance.sequenceReader.ReadByte();
        for (Int16 i = 0; i < 3; i++)
        {
            wk_MOVE.Org[i] = (Int16)pMe.gameObject.transform.localPosition[i];
            wk_MOVE.Dst[i] = instance.sequenceReader.ReadInt16();
            if (i == 1)
                wk_MOVE.Dst[i] *= -1;
        }
        pSeqWork.Work = btlseq.SequenceConverter.WkMoveToWork(wk_MOVE);
    }

    public static Int32 SeqExecMoveToPoint(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecMoveToPoint");
        btlseq.SeqExecMoveToTarget(pSeqWork, pMe);
        for (Int16 i = 0; i < 3; i++)
            pMe.base_pos[i] = pMe.pos[i];
        return 0;
    }

    public static Int32 SeqExecTurn(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecTurn");
        Int16 num = 0;
        Int16 num2 = 0;
        Int16 num3 = instance.sequenceReader.ReadInt16();
        Int16 num4 = instance.sequenceReader.ReadInt16();
        num4 = (Int16)((Single)num4 / 4096f * 360f);
        pSeqWork.TurnTime = instance.sequenceReader.ReadByte();
        pSeqWork.TurnOrg = (Int16)pMe.rot.eulerAngles.y;
        pSeqWork.TurnCnt = 1;
        if (((Int32)num3 & 0x8000) != 0)
        {
            Int16 num5 = (Int16)(num3 & Int16.MaxValue);
            if (num5 != 0)
            {
                if (num5 != 1)
                {
                    num3 = (Int16)pMe.rot.eulerAngles[1];
                }
                else
                {
                    btlseq.SeqSubTargetAveragePos(pSeqWork.CmdPtr.tar_id, out num, out num2);
                    num = (Int16)(pMe.gameObject.transform.localPosition.x - (Single)num);
                    num2 = (Int16)(pMe.gameObject.transform.localPosition.z - (Single)num2);
                    num3 = (Int16)((num != 0 || num2 != 0) ? ((Int16)((Double)Mathf.Atan2((Single)num, (Single)num2) * 57.295779513082323)) : ((Int16)pMe.rot.eulerAngles.y));
                }
            }
            else
            {
                num3 = (Int16)pMe.evt.rotBattle.eulerAngles.y;
            }
        }
        else
        {
            num3 = (Int16)((Single)num3 / 4096f * 360f);
        }
        num3 = (Int16)(num3 + num4);
        num3 = (Int16)(pMe.rot.eulerAngles.y - (Single)num3);
        pSeqWork.TurnRot = (Int16)((num3 <= 180) ? ((Int16)(-num3)) : ((Int16)(360 - num3)));
        pSeqWork.CurPtr += 6;
        return 1;
    }

    public static void SeqSubTargetAveragePos(UInt16 pTarID, out Int16 px, out Int16 pz)
    {
        Int32 num = 0;
        px = 0;
        pz = 0;
        for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
        {
            if ((next.btl_id & pTarID) != 0)
            {
                px = (Int16)(px + (Int16)next.pos[0]);
                pz = (Int16)(pz + (Int16)next.pos[2]);
                num++;
            }
        }
        if (num > 1)
        {
            px = (Int16)(px / (Int16)num);
            pz = (Int16)(pz / (Int16)num);
        }
    }

    public static Int32 SeqExecTexAnimPlay(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecTexAnimPlay");
        GeoTexAnim.geoTexAnimPlay(pMe.texanimptr, instance.sequenceReader.ReadByte());
        pSeqWork.CurPtr += 2;
        return 1;
    }

    public static Int32 SeqExecTexAnimOnce(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecTexAnimOnce");
        GeoTexAnim.geoTexAnimPlayOnce(pMe.texanimptr, instance.sequenceReader.ReadByte());
        pSeqWork.CurPtr += 2;
        return 1;
    }

    public static Int32 SeqExecTexAnimStop(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecTexAnimStop");
        GeoTexAnim.geoTexAnimStop(pMe.texanimptr, instance.sequenceReader.ReadByte());
        pSeqWork.CurPtr += 2;
        return 1;
    }

    public static Int32 SeqExecFastEnd(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecFastEnd");
        if (SFX.isRunning)
            return 0;
        if (pSeqWork.TurnTime != 0)
            return 0;
        if (!pSeqWork.Flags.EventMode)
            btl_cmd.ReqFinishCommand(pSeqWork.CmdPtr);
        pSeqWork.CmdPtr = (CMD_DATA)null;
        return 0;
    }

    public static Int32 SeqExecSfx(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecSfx");
        pSeqWork.SfxNum = instance.sequenceReader.ReadUInt16();
        pSeqWork.SfxTime = (Byte)(instance.sequenceReader.ReadByte() + 1);
        instance.sequenceReader.Read();
        pSeqWork.SfxVol = instance.sequenceReader.ReadByte();
        pSeqWork.CurPtr += 6;
        return 1;
    }

    public static void SeqInitMoveToOffset(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqInitMoveToOffset");
        WK_MOVE wk_MOVE = new WK_MOVE();
        wk_MOVE.Next = 8;
        pSeqWork.IncCnt = 1;
        wk_MOVE.Frames = instance.sequenceReader.ReadByte();
        for (Int16 num = 0; num < 3; num++)
        {
            wk_MOVE.Org[num] = (Int16)pMe.gameObject.transform.localPosition[num];
            Int16 num2 = instance.sequenceReader.ReadInt16();
            if (num == 1)
                num2 *= -1;
            wk_MOVE.Dst[num] = (Int16)(wk_MOVE.Org[num] + num2);
        }
        pSeqWork.Work = btlseq.SequenceConverter.WkMoveToWork(wk_MOVE);
    }

    public static Int32 SeqExecTargetBone(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecTargetBone");
        pMe.tar_bone = instance.sequenceReader.ReadByte();
        pSeqWork.CurPtr += 2;
        return 1;
    }

    public static Int32 SeqExecFadeOut(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecFadeOut");
        pSeqWork.FadeTotal = (pSeqWork.FadeStep = instance.sequenceReader.ReadByte());
        pSeqWork.CurPtr += 2;
        return 1;
    }

    public static Int32 SeqExecShadow(SEQ_WORK pSeqWork, BTL_DATA pMe)
    {
        btlseq.BattleLog("SeqExecShadow");
        pMe.bi.shadow = instance.sequenceReader.ReadByte();
        if (pMe.bi.shadow != 0)
            pMe.getShadow().SetActive(true);
        else
            pMe.getShadow().SetActive(false);
        pSeqWork.CurPtr += 2;
        return 1;
    }

    public static void BattleLog(String str)
    {
    }

    public const Int32 BTL_LIST_PLAYER_0 = 0;
    public const Int32 BTL_LIST_PLAYER_1 = 1;
    public const Int32 BTL_LIST_PLAYER_2 = 2;
    public const Int32 BTL_LIST_PLAYER_3 = 3;
    public const Int32 BTL_LIST_ENERMY_0 = 4;
    public const Int32 BTL_LIST_ENERMY_1 = 5;
    public const Int32 BTL_LIST_ENERMY_2 = 6;
    public const Int32 BTL_LIST_ENERMY_3 = 7;

    public const Int32 RET_BLOCK = 0;
    public const Int32 RET_CONTINUE = 1;

    public class btlseqinstance
    {
        public Byte[] data;

        public BinaryReader sequenceReader;

        public SEQ_WORK_SET seq_work_set;

        public ENEMY[] enemy;

        public Int32 wSeqCode;

        public BTL_DATA[] btl_list;

        public Int32 camOffset;

        public SequenceProperty[] sequenceProperty;

        public Int32 GetEnemyIndexOfSequence(Int32 pSeqNo)
        {
            for (UInt32 i = 0; i < sequenceProperty.Length; i++)
                if (sequenceProperty[i].PlayableSequence.Contains(pSeqNo))
                    return sequenceProperty[i].Montype;
            return -1;
        }

        public Int32 GetSFXOfSequence(Int32 pSeqNo, out Boolean isChanneling, out Boolean isContact)
        {
            isChanneling = false;
            isContact = false;
            if (seq_work_set.SeqData[pSeqNo] == 0)
                return -1;
            using (sequenceReader = new BinaryReader(new MemoryStream(data)))
            {
                sequenceReader.BaseStream.Seek(seq_work_set.SeqData[pSeqNo] + 4, SeekOrigin.Begin);
                wSeqCode = sequenceReader.ReadByte();
                while (wSeqCode != 0 && wSeqCode != 0x18)
                {
                    if (wSeqCode > btlseq.gSeqProg.Length)
                        wSeqCode = 0;
                    switch (wSeqCode)
                    {
                        case 0x6:
                        case 0x8:
                        case 0x1A:
                            isChanneling = wSeqCode == 0x8;
                            isContact = wSeqCode == 0x6;
                            return sequenceReader.ReadByte() | (sequenceReader.ReadByte() << 8);
                        case 2:
                        case 7:
                        case 9:
                        case 0xA:
                        case 0xB:
                        case 0x18:
                            break;
                        case 1:
                        case 5:
                        case 4:
                        case 0xE:
                        case 0x10:
                        case 0x11:
                        case 0x12:
                        case 0x15:
                        case 0x16:
                        case 0x17:
                        case 0x1C:
                        case 0x1D:
                        case 0x1F:
                        case 0x20:
                        case 0x21:
                            sequenceReader.BaseStream.Seek(1, SeekOrigin.Current);
                            break;
                        case 0xD:
                        case 0xF:
                            sequenceReader.BaseStream.Seek(2, SeekOrigin.Current);
                            break;
                        case 3:
                        case 0xC:
                        case 0x1E:
                            sequenceReader.BaseStream.Seek(3, SeekOrigin.Current);
                            break;
                        case 0x14:
                        case 0x19:
                            sequenceReader.BaseStream.Seek(5, SeekOrigin.Current);
                            break;
                        case 0x13:
                        case 0x1B:
                            sequenceReader.BaseStream.Seek(7, SeekOrigin.Current);
                            break;
                    }
                    wSeqCode = sequenceReader.ReadByte();
                }
            }
            return -1;
        }
        public Int32 GetSFXOfSequence(Int32 pSeqNo)
        {
            return GetSFXOfSequence(pSeqNo, out _, out _);
        }

        public List<Int32> GetAnimationsOfSequence(Int32 pSeqNo)
        {
            List<Int32> animList = new List<Int32>();
            if (seq_work_set.SeqData[pSeqNo] == 0)
                return animList;
            using (sequenceReader = new BinaryReader(new MemoryStream(data)))
            {
                sequenceReader.BaseStream.Seek(seq_work_set.SeqData[pSeqNo] + 4, SeekOrigin.Begin);
                wSeqCode = sequenceReader.ReadByte();
                while (wSeqCode != 0 && wSeqCode != 0x18)
                {
                    if (wSeqCode > btlseq.gSeqProg.Length)
                        wSeqCode = 0;
                    switch (wSeqCode)
                    {
                        case 5:
                            animList.Add(sequenceReader.ReadByte());
                            break;
                        case 2:
                        case 7:
                        case 9:
                        case 0xA:
                        case 0xB:
                        case 0x18:
                            break;
                        case 1:
                        case 4:
                        case 0xE:
                        case 0x10:
                        case 0x11:
                        case 0x12:
                        case 0x15:
                        case 0x16:
                        case 0x17:
                        case 0x1C:
                        case 0x1D:
                        case 0x1F:
                        case 0x20:
                        case 0x21:
                            sequenceReader.BaseStream.Seek(1, SeekOrigin.Current);
                            break;
                        case 0xD:
                        case 0xF:
                            sequenceReader.BaseStream.Seek(2, SeekOrigin.Current);
                            break;
                        case 3:
                        case 0xC:
                        case 0x1E:
                            sequenceReader.BaseStream.Seek(3, SeekOrigin.Current);
                            break;
                        case 0x6:
                            sequenceReader.BaseStream.Seek(4, SeekOrigin.Current);
                            break;
                        case 0x14:
                        case 0x19:
                            sequenceReader.BaseStream.Seek(5, SeekOrigin.Current);
                            break;
                        case 0x13:
                        case 0x1B:
                            sequenceReader.BaseStream.Seek(7, SeekOrigin.Current);
                            break;
                        case 0x8:
                        case 0x1A:
                            sequenceReader.BaseStream.Seek(8, SeekOrigin.Current);
                            break;
                    }
                    wSeqCode = sequenceReader.ReadByte();
                }
            }
            return animList;
        }

        public void FixBuggedAnimations(BTL_SCENE scene)
        {
            for (Int32 i = 0; i < scene.MonAddr.Length; i++)
            {
                if (scene.MonAddr[i].Geo == 149) // Wraith (Ice)
                {
                    Int32 lastAnim = i + 1 < scene.MonAddr.Length ? seq_work_set.AnmOfsList[scene.MonAddr[i + 1].Konran] : seq_work_set.AnmAddrList.Length;
                    for (Int32 j = seq_work_set.AnmOfsList[scene.MonAddr[i].Konran]; j < lastAnim; j++)
                        if (j < seq_work_set.AnmAddrList.Length)
                        {
                            if (seq_work_set.AnmAddrList[j] == 6707) // Cast Init (Fire)
                                seq_work_set.AnmAddrList[j] = 3961; // Cast Init (Ice)
                            else if (seq_work_set.AnmAddrList[j] == 6713) // Cast Loop (Fire)
                                seq_work_set.AnmAddrList[j] = 3967; // Cast Loop (Ice)
                            else if (seq_work_set.AnmAddrList[j] == 6695) // Cast End (Fire)
                                seq_work_set.AnmAddrList[j] = 3949; // Cast End (Ice)
                        }
                }
            }
        }
    }

    public static btlseqinstance instance = new btlseqinstance();

    public static btlseq.SequenceProgram[] gSeqProg = new btlseq.SequenceProgram[]
    {
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecEnd)),
        new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitWait), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecWait)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecCalc)),
        new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitMoveToTarget), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMoveToTarget)),
        new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitMoveToTurn), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMoveToTarget)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecAnim)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecSVfx)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecWaitAnim)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecVfx)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecWaitLoadVfx)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecStartVfx)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecWaitVfx)),
        new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitScale), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecScale)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMeshHide)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMessage)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMeshShow)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecSetCamera)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecDefaultIdle)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecRunCamera)),
        new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitMoveToPoint), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMoveToPoint)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecTurn)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecTexAnimPlay)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecTexAnimOnce)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecTexAnimStop)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecFastEnd)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecSfx)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecVfx)),
        new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitMoveToOffset), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMoveToPoint)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecTargetBone)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecFadeOut)),
        new btlseq.SequenceProgram(new btlseq.SequenceProgram.InitEvent(btlseq.SeqInitMoveToTarget), new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMoveToTarget)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecShadow)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecRunCamera)),
        new btlseq.SequenceProgram((btlseq.SequenceProgram.InitEvent)null, new btlseq.SequenceProgram.ExecEvent(btlseq.SeqExecMessage))
    };

    public static class SequenceConverter
    {
        public static WK_MOVE WorkToWkMove(Byte[] work)
        {
            WK_MOVE wk_MOVE = new WK_MOVE();
            wk_MOVE.Org[0] = (Int16)(((Int32)work[1] << 8) + (Int32)(work[0] & Byte.MaxValue));
            wk_MOVE.Org[1] = (Int16)(((Int32)work[3] << 8) + (Int32)(work[2] & Byte.MaxValue));
            wk_MOVE.Org[2] = (Int16)(((Int32)work[5] << 8) + (Int32)(work[4] & Byte.MaxValue));
            wk_MOVE.Dst[0] = (Int16)(((Int32)work[7] << 8) + (Int32)(work[6] & Byte.MaxValue));
            wk_MOVE.Dst[1] = (Int16)(((Int32)work[9] << 8) + (Int32)(work[8] & Byte.MaxValue));
            wk_MOVE.Dst[2] = (Int16)(((Int32)work[11] << 8) + (Int32)(work[10] & Byte.MaxValue));
            wk_MOVE.Frames = (Int16)(((Int32)work[13] << 8) + (Int32)(work[12] & Byte.MaxValue));
            wk_MOVE.Next = (UInt16)(((Int32)work[15] << 8) + (Int32)(work[14] & Byte.MaxValue));
            return wk_MOVE;
        }

        public static Byte[] WkMoveToWork(WK_MOVE wMove)
        {
            return new Byte[]
            {
                (Byte)(wMove.Org[0] & 255),
                (Byte)(wMove.Org[0] >> 8),
                (Byte)(wMove.Org[1] & 255),
                (Byte)(wMove.Org[1] >> 8),
                (Byte)(wMove.Org[2] & 255),
                (Byte)(wMove.Org[2] >> 8),
                (Byte)(wMove.Dst[0] & 255),
                (Byte)(wMove.Dst[0] >> 8),
                (Byte)(wMove.Dst[1] & 255),
                (Byte)(wMove.Dst[1] >> 8),
                (Byte)(wMove.Dst[2] & 255),
                (Byte)(wMove.Dst[2] >> 8),
                (Byte)(wMove.Frames & 255),
                (Byte)(wMove.Frames >> 8),
                (Byte)(wMove.Next & 255),
                (Byte)(wMove.Next >> 8)
            };
        }

        public static WK_SCALE WorkToWkScale(Byte[] work)
        {
            return new WK_SCALE
            {
                Org = (Int16)(((Int32)work[1] << 8) + (Int32)(work[0] & Byte.MaxValue)),
                Scl = (Int16)(((Int32)work[3] << 8) + (Int32)(work[2] & Byte.MaxValue)),
                Frames = (Int16)(((Int32)work[5] << 8) + (Int32)(work[4] & Byte.MaxValue))
            };
        }

        public static Byte[] WkScaleToWork(WK_SCALE wScale)
        {
            return new Byte[]
            {
                (Byte)(wScale.Org & 255),
                (Byte)(wScale.Org >> 8),
                (Byte)(wScale.Scl & 255),
                (Byte)(wScale.Scl >> 8),
                (Byte)(wScale.Frames & 255),
                (Byte)(wScale.Frames >> 8)
            };
        }
    }

    public class SequenceProgram
    {
        public SequenceProgram(btlseq.SequenceProgram.InitEvent init, btlseq.SequenceProgram.ExecEvent exec)
        {
            this.Init = init;
            this.Exec = exec;
        }

        public btlseq.SequenceProgram.InitEvent Init;

        public btlseq.SequenceProgram.ExecEvent Exec;

        public delegate void InitEvent(SEQ_WORK pSeqWork, BTL_DATA pMe);

        public delegate Int32 ExecEvent(SEQ_WORK pSeqWork, BTL_DATA pMe);
    }
}
