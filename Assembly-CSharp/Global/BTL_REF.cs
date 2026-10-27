using System;
using Memoria.Data;

public class BTL_REF
{
    [Memoria.PatchableFieldAttribute]
    public Int32 ScriptId;
    [Memoria.PatchableFieldAttribute]
    public Int32 Power;
    [Memoria.PatchableFieldAttribute]
    public ElementSet Elements;
    [Memoria.PatchableFieldAttribute]
    public Int32 Rate;

    public BTL_REF()
    {
        ScriptId = 0;
        Power = 0;
        Elements = new ElementSet();
        Rate = 0;
    }

    public BTL_REF(Int32 scriptId, Int32 power, ElementSet elements, Int32 rate)
    {
        ScriptId = scriptId;
        Power = power;
        Elements = elements;
        Rate = rate;
    }
}
