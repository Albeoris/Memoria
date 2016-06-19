using Assets.Scripts.Common;
using UnityEngine;

public partial class EventEngine
{
    private void InitEncount()
    {
        this._encountTimer = -1440f;
        this._context.encratio = (byte)0;
        this._encountBase = 0;
        this._encountReserved = false;
    }

    public void InitEvents()
    {
        this.InitEvents(1);
    }

    public void InitEvents(int gModeVal)
    {
        this.eTb = new ETb();
        this.sEventContext0 = new EventContext();
        this.sEventContext1 = new EventContext();
        this.eBin = new EBin(this);
        this.gMode = gModeVal;
        this.eTb.gMesValue = new int[8];
        this._objPtrList = new Obj[8];
        this._sysList = new ushort[8];
        this._enCountData = new EncountData();
        this.eTb.InitKeyEvents();
        this.InitEventsSub();
        this.eTb.InitKeyEvents();
        this.InitEventsSub();
    }

    public void NewGame()
    {
        FF9StateSystem.EventState.gStepCount = 0;
        FF9StateSystem.EventState.gEventGlobal = new byte[2048];
        FF9StateSystem.Common.FF9.fldMapNo = (short)70;
        FF9StateSystem.Common.FF9.fldLocNo = (short)EventEngineUtils.eventIDToMESID[70];
        FF9StateSystem.Settings.time = 0.0;
        this.ReplaceFieldMap();
    }

    public void ReplaceFieldMap()
    {
        PersistenSingleton<FF9StateSystem>.Instance.mode = (byte)1;
        FF9StateSystem.Settings.StartGameTime = (double)Time.time;
        this.InitializeFlags();
        PersistenSingleton<EventEngine>.Instance.InitEvents();
    }

    public void ReplaceLoadMap()
    {
        if ((int)PersistenSingleton<FF9StateSystem>.Instance.mode == 1)
            SceneDirector.Replace("FieldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
        else
            SceneDirector.Replace("WorldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
        FF9StateSystem.Settings.StartGameTime = (double)Time.time;
        this.InitializeFlags();
        PersistenSingleton<EventEngine>.Instance.InitEvents((int)PersistenSingleton<FF9StateSystem>.Instance.mode);
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