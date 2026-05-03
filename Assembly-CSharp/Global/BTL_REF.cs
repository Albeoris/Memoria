using System;

public class BTL_REF
{
    [Memoria.PatchableFieldAttribute]
    public Int32 ScriptId;
    [Memoria.PatchableFieldAttribute]
    public Int32 Power;
    [Memoria.PatchableFieldAttribute]
    public Byte Elements;
    [Memoria.PatchableFieldAttribute]
    public Int32 Rate;

    public BTL_REF()
    {
        ScriptId = 0;
        Power = 0;
        Elements = 0;
        Rate = 0;
    }

    public BTL_REF(Int32 scriptId, Int32 power, Byte elements, Int32 rate)
    {
        ScriptId = scriptId;
        Power = power;
        Elements = elements;
        Rate = rate;
    }
}
