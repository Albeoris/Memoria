﻿using Assets.Scripts.Common;
using Memoria;
using System;
using UnityEngine;

public partial class EventEngine
{
    private void InitEncount()
    {
        if (!Configuration.Battle.PersistentDangerValue)
        {
            this._encountTimer = Configuration.Battle.EncounterInitial;
            this._encountBase = 0;
        }
        this._context.encratio = 0;
        this._encountReserved = false;
    }

    public void InitEvents()
    {
        this.InitEvents(1);
    }

    public void InitEvents(Int32 gModeVal)
    {
        this.sEventContext0 = new EventContext();
        this.sEventContext1 = new EventContext();
        this.eBin = new EBin(this);
        this.gMode = gModeVal;
        ETb.gMesValue = new Int32[8];
        this._objPtrList = new Obj[8];
        this._sysList = new UInt16[8];
        this._enCountData = new EncountData();
        ETb.InitKeyEvents();
        this.InitEventsSub();
        ETb.InitKeyEvents();
        this.InitEventsSub();
    }

    public void NewGame()
    {
        FF9StateSystem.EventState.gStepCount = 0;
        FF9StateSystem.EventState.gEventGlobal = new Byte[2048];
        FF9StateSystem.EventState.gAbilityUsage.Clear();
        FF9StateSystem.EventState.gScriptVector.Clear();
        FF9StateSystem.EventState.gScriptDictionary.Clear();
        FF9StateSystem.Common.FF9.fldMapNo = 70; // Opening-For FMV
        FF9StateSystem.Common.FF9.fldLocNo = (Int16)EventEngineUtils.eventIDToMESID[70];
        FF9StateSystem.Settings.time = 0.0;
        this._encountTimer = Configuration.Battle.EncounterInitial;
        this._encountBase = 0;
        this.ReplaceFieldMap();
    }

    public void ReplaceFieldMap()
    {
        PersistenSingleton<FF9StateSystem>.Instance.mode = 1;
        FF9StateSystem.Settings.StartGameTime = Time.time;
        this.InitializeFlags();
        this.InitEvents();
    }

    public void ReplaceLoadMap()
    {
        if (PersistenSingleton<FF9StateSystem>.Instance.mode == 1)
            SceneDirector.Replace("FieldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
        else
            SceneDirector.Replace("WorldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
        FF9StateSystem.Settings.StartGameTime = Time.time;
        this.InitializeFlags();
        this.InitEvents(PersistenSingleton<FF9StateSystem>.Instance.mode);
    }

    public void InitializeFlags()
    {
        FF9StateSystem.Field.isDebug = false;
        FF9StateSystem.Field.isDebugWalkMesh = false;
        FF9StateSystem.World.IsBeeScene = false;
    }

    public void InitEventsSub()
    {
        this._context = null;
        this._lastScene = 0;
        this.sEventContext0.inited = 0;
        this.sEventContext1.inited = 0;
    }
}
