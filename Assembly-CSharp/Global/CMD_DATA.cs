using FF9;
using Memoria.Data;
using System;

public class CMD_DATA
{
    public CMD_DATA()
    {
        this.regist = new BTL_DATA();
        this.info = new CMD_DATA.SELECT_INFO();
        this.aa = new AA_DATA();
        this.vfxRequest = new BTL_VFX_REQ();
    }

    public CMD_DATA next;
    public BTL_DATA regist;
    public AA_DATA aa;
    public UInt16 tar_id;
    public BattleCommandId cmd_no;
    public Int32 sub_no;
    public CMD_DATA.SELECT_INFO info;

    // Having duplicates allow to modify these fields (eg. SA features) without modifying the base AA's fields
    public Boolean IsShortRange;
    public Int32 HitRate;
    public Int32 Power;
    public Int32 ScriptId;
    public EffectElement Element;
    public EffectElement ElementForBonus;
    public BattleStatus AbilityStatus;
    public Byte AbilityCategory;
    public Byte AbilityType;
    public SpecialEffect PatchedVfx;

    public BTL_VFX_REQ vfxRequest;
    public REFLEC_DATA reflec = new REFLEC_DATA();

    public void SetAAData(AA_DATA value)
    {
        aa = value;
        if (aa != null)
        {
            HitRate = aa.Ref.Rate;
            Power = aa.Ref.Power;
            ScriptId = aa.Ref.ScriptId;
            Element = (EffectElement)aa.Ref.Elements;
            ElementForBonus = Element;
            AbilityStatus = FF9StateSystem.Battle.FF9Battle.add_status[aa.AddStatusNo].Value;
            AbilityCategory = aa.Category;
            AbilityType = aa.Type;
            PatchedVfx = SpecialEffect.Special_No_Effect;
            IsShortRange = false;
        }
    }

    public Int32 GetCommandMPCost()
    {
        return info.CustomMPCost ?? aa?.MP ?? 0;
    }

    public class SELECT_INFO
    {
        public Byte cursor;
        public Byte stat;
        public Byte priority;
        public Byte cover;
        public Byte dodge;
        public Byte reflec;
        public Byte meteor_miss;
        public Byte short_summon;
        public Byte mon_reflec;

        // Custom fields
        public BattleCommandMenu cmdMenu;
        public command_mode_index mode;
        public Boolean cmd_motion;
        // For multi-hit attacks (this counter allows to keep track of the hit number, for having different effects)
        public Int32 effect_counter;
        public Int32? CustomMPCost { get; set; }
        public Boolean ReflectNull { get; set; }
        public Boolean HasCheckedReflect { get; set; }

        public Boolean IsZeroMP
        {
            get => CustomMPCost == 0;
            set => CustomMPCost = value ? 0 : null;
        }

        public void Reset()
        {
            cursor = 0;
            stat = 0;
            priority = 0;
            cover = 0;
            dodge = 0;
            reflec = 0;
            meteor_miss = 0;
            short_summon = 0;
            mon_reflec = 0;
            cmdMenu = BattleCommandMenu.None;
            mode = command_mode_index.CMD_MODE_INSPECTION;
            cmd_motion = true;
            effect_counter = 0;
            IsZeroMP = false;
            CustomMPCost = null;
            ReflectNull = false;
            HasCheckedReflect = false;
        }
    }
}
