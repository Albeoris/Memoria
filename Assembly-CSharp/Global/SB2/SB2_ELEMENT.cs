using System;

public class SB2_ELEMENT
{
    [Memoria.PatchableFieldAttribute]
    public Byte Speed;

    [Memoria.PatchableFieldAttribute]
    public Byte Strength;

    [Memoria.PatchableFieldAttribute]
    public Byte Magic;

    [Memoria.PatchableFieldAttribute]
    public Byte Spirit;

    public Byte pad;

    public Byte trans;

    public Byte cur_capa;

    public Byte max_capa;
}
