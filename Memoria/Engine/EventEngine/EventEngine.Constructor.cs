using System.Collections.Generic;
using System.Linq;

public partial class EventEngine
{
    public EventEngine()
    {
        this.nil = -1;
        this.nilFloat = -1f;
        this.POS_COMMAND_DEFAULTY = 32768f;
        this.toBeAddedObjUIDList = new List<int>();
        Dictionary<int, int> dictionary1 = new Dictionary<int, int>
        {
            {246, 254},
            {247, (int)byte.MaxValue},
            {248, 256},
            {249, 257},
            {250, 258},
            {251, 259},
            {252, 260},
            {253, 261},
            {254, 262},
            {(int)byte.MaxValue, 263},
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
        Dictionary<int, int> dictionary2 = new Dictionary<int, int>
        {
            {246, 254},
            {247, (int)byte.MaxValue},
            {248, 256},
            {249, 257},
            {250, 258},
            {251, 259},
            {252, 260},
            {253, 261},
            {254, 262},
            {(int)byte.MaxValue, 263},
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
        Dictionary<int, int> dictionary3 = new Dictionary<int, int>
        {
            {246, 254},
            {247, (int)byte.MaxValue},
            {248, 256},
            {249, 257},
            {250, 258},
            {251, 259},
            {252, 260},
            {253, 261},
            {254, 262},
            {(int)byte.MaxValue, 263},
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
        int[] array = EventEngineUtils.eventIDToFBGID.Keys.ToArray<int>();
        EventEngine.testEventIDs = (int[])null;
        EventEngine.testEventIDs = array;
        this.fieldCalc = new fld_calc();
        this._ff9fieldDisc = new FF9FIELD_DISC();
    }

}