using System;

public class BTL_SCENE_INFO
{
    public battle_start_type_tags StartType;

    [Memoria.PatchableFieldAttribute]
    public Boolean SpecialStart;

    [Memoria.PatchableFieldAttribute]
    public Boolean BackAttack;

    [Memoria.PatchableFieldAttribute]
    public Boolean NoGameOver;

    [Memoria.PatchableFieldAttribute]
    public Boolean NoExp;

    [Memoria.PatchableFieldAttribute]
    public Boolean WinPose;

    [Memoria.PatchableFieldAttribute]
    public Boolean Runaway;

    [Memoria.PatchableFieldAttribute]
    public Boolean NoNeighboring;

    [Memoria.PatchableFieldAttribute]
    public Boolean NoMagical;

    [Memoria.PatchableFieldAttribute]
    public Boolean ReverseAttack;

    [Memoria.PatchableFieldAttribute]
    public Boolean FixedCamera1;

    [Memoria.PatchableFieldAttribute]
    public Boolean FixedCamera2;

    [Memoria.PatchableFieldAttribute]
    public Boolean AfterEvent;

    [Memoria.PatchableFieldAttribute]
    public Boolean FieldBGM;

    [Memoria.PatchableFieldAttribute]
    public Boolean Preemptive;

    [Memoria.PatchableFieldAttribute]
    public String BattleBackground;
}
