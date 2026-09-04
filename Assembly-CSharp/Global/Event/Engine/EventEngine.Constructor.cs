using System;
using System.Collections.Generic;
using System.Linq;

public partial class EventEngine
{
    public EventEngine()
    {
        this.nil = -1;
        this.nilFloat = -1f;
        this.POS_COMMAND_DEFAULTY = 32768f;
        this.toBeAddedObjUIDList = new List<Int32>();
        this._requestCommandTrigger = new CMD_DATA[8, 8]; // [btl.bi.line_no, call.level]
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
    }

    protected override void Awake()
    {
        EventEngine.testEventIDs = EventEngineUtils.eventIDToFBGID.Keys.ToArray<Int32>();
        this._ff9fieldDisc = new FF9FIELD_DISC();
    }

}
