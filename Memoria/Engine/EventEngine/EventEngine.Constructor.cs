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
        Dictionary<Int32, Int32> dictionary1 = new Dictionary<Int32, Int32>
        {
            {246, 254},
            {247, (Int32)Byte.MaxValue},
            {248, 256},
            {249, 257},
            {250, 258},
            {251, 259},
            {252, 260},
            {253, 261},
            {254, 262},
            {(Int32)Byte.MaxValue, 263},
            {256, 264},
            {257, 265},
            {258, 266},
            {259, 267},
            {260, 268},
            {261, 269},
            {263, 270},
            {262, 271},
            {264, 272},
            {265, 273}
        };
        this._mesIdES_FR = dictionary1;
        Dictionary<Int32, Int32> dictionary2 = new Dictionary<Int32, Int32>
        {
            {246, 254},
            {247, (Int32)Byte.MaxValue},
            {248, 256},
            {249, 257},
            {250, 258},
            {251, 259},
            {252, 260},
            {253, 261},
            {254, 262},
            {(Int32)Byte.MaxValue, 263},
            {256, 264},
            {257, 265},
            {258, 266},
            {259, 267},
            {260, 268},
            {261, 270},
            {263, 273},
            {262, 271},
            {264, 274},
            {265, 275}
        };
        this._mesIdGR = dictionary2;
        Dictionary<Int32, Int32> dictionary3 = new Dictionary<Int32, Int32>
        {
            {246, 254},
            {247, (Int32)Byte.MaxValue},
            {248, 256},
            {249, 257},
            {250, 258},
            {251, 259},
            {252, 260},
            {253, 261},
            {254, 262},
            {(Int32)Byte.MaxValue, 263},
            {256, 264},
            {257, 265},
            {258, 266},
            {259, 267},
            {260, 268},
            {261, 269},
            {263, 271},
            {262, 270},
            {264, 272},
            {265, 273}
        };
        this._mesIdIT = dictionary3;
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
    }

    protected override void Awake()
    {
        Int32[] array = EventEngineUtils.eventIDToFBGID.Keys.ToArray<Int32>();
        EventEngine.testEventIDs = (Int32[])null;
        EventEngine.testEventIDs = array;
        this.fieldCalc = new fld_calc();
        this._ff9fieldDisc = new FF9FIELD_DISC();
    }

}