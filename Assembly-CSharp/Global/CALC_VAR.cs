using System;

public class CALC_VAR
{
    public CALC_VAR()
    {
        this.caster = new BTL_DATA();
        this.target = new BTL_DATA();
        this.cmd = new CMD_DATA();
    }

    public BTL_DATA caster;

    public BTL_DATA target;

    public CMD_DATA cmd;

    public Int16 add_rate;

    public Int16 at_pow;

    public Int16 df_pow;

    public Int16 ev;

    public Int16 at_num;

    public Int16 hit_rate;

    public UInt16 flags;

    public Byte ct_flags;

    public Byte tg_flags;

    public Int16 ct_hp;

    public Int16 tg_hp;

    public Int16 ct_mp;

    public Int16 tg_mp;
}
