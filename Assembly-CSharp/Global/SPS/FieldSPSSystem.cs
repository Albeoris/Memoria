using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FieldSPSSystem : HonoBehavior
{
    public override void HonoUpdate()
    {
        this.EffectUpdate();
    }

    private void LateUpdate()
    {
        if (!PersistenSingleton<UIManager>.Instance.IsPause)
            this.GenerateSPS();
    }

    public void Init(FieldMap fieldMap)
    {
        Utility = new CommonSPSSystem();
        this._fieldMap = fieldMap;
        for (Int32 i = 0; i < SPSConst.FIELD_DEFAULT_OBJCOUNT; i++)
            this.InitSPSInstance(i);
        FieldMapInfo.fieldmapSPSExtraOffset.SetSPSOffset(FF9StateSystem.Field.SceneName, Utility.SpsList);
        Utility.LoadMapTextureInVram(FF9StateSystem.Field.SceneName);
    }

    private void InitSPSInstance(Int32 index)
    {
        if (index < 0 || index > Utility.SpsList.Count)
            return;
        if (index < Utility.SpsList.Count)
        {
            Utility.SpsList[index].Init(1);
            Utility.SpsList[index].fieldMap = this._fieldMap;
            return;
        }
        GameObject spsGo = new GameObject($"SPS_{index:D4}");
        spsGo.transform.parent = base.transform;
        spsGo.transform.localScale = Vector3.one;
        spsGo.transform.localPosition = Vector3.zero;
        MeshRenderer meshRenderer = spsGo.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = spsGo.AddComponent<MeshFilter>();
        SPSEffect sps = spsGo.AddComponent<SPSEffect>();
        sps.Init(1);
        sps.fieldMap = this._fieldMap;
        sps.spsIndex = index;
        sps.spsTransform = spsGo.transform;
        sps.meshRenderer = meshRenderer;
        sps.meshFilter = meshFilter;
        FieldSPSActor fieldSPSActor = spsGo.AddComponent<FieldSPSActor>();
        fieldSPSActor.sps = sps;
        sps.spsActor = fieldSPSActor;
        Utility.SpsList.Add(sps);
    }

    public void EffectUpdate()
    {
        if (!Utility.IsTCBReady)
            return;
        foreach (SPSEffect sps in Utility.SpsList)
        {
            if (sps.spsBin != null && (sps.attr & SPSConst.ATTR_VISIBLE) != 0 && sps.lastFrame != -1)
            {
                sps.lastFrame = sps.curFrame;
                sps.curFrame += sps.frameRate;
                if (sps.curFrame >= sps.frameCount)
                {
                    sps.curFrame = 0;
                    if ((sps.attr & SPSConst.ATTR_UNLOAD_ON_FINISH) != 0)
                        sps.Unload();
                }
                else if (sps.curFrame < 0)
                {
                    sps.curFrame = (sps.frameCount >> 4) - 1 << 4;
                }
                if (sps.duration > 0)
                    sps.duration--;
                if (sps.duration == 0)
                    sps.Unload();
            }
        }
    }

    public void GenerateSPS()
    {
        if (!Utility.IsTCBReady)
            return;
        foreach (SPSEffect sps in Utility.SpsList)
        {
            if (sps.spsBin != null && (sps.attr & SPSConst.ATTR_VISIBLE) != 0)
            {
                if ((sps.attr & (SPSConst.ATTR_UPDATE_THIS_FRAME | SPSConst.ATTR_UPDATE_ANY_FRAME)) == 0)
                {
                    sps.meshRenderer.enabled = false;
                    continue;
                }
                sps.attr &= unchecked((Byte)~SPSConst.ATTR_UPDATE_THIS_FRAME);
                if (sps.charTran != null && sps.boneTran != null)
                {
                    FieldMapActor component = sps.charTran.GetComponent<FieldMapActor>();
                    if (component != null)
                        component.UpdateGeoAttach();
                    sps.pos = sps.boneTran.position + sps.posOffset;
                }
                sps.GenerateSPS();
                sps.lastFrame = sps.curFrame;
                sps.meshRenderer.enabled = true;
            }
        }
    }

    public void SetObjParm(Int32 ObjNo, Int32 ParmType, Int32 Arg0, Int32 Arg1, Int32 Arg2)
    {
        if (ObjNo == Utility.SpsList.Count)
            this.InitSPSInstance(ObjNo);
        if (ObjNo < 0 || ObjNo >= Utility.SpsList.Count)
            return;
        Utility.SetObjParm(Utility.SpsList[ObjNo], ParmType, Arg0, Arg1, Arg2);
    }

    [NonSerialized]
    private CommonSPSSystem Utility;

    private FieldMap _fieldMap;
}
