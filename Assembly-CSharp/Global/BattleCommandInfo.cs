using Memoria.Data;
using System;

public class BattleCommandInfo
{
    [Memoria.PatchableFieldAttribute]
    public TargetType Target;
    [Memoria.PatchableFieldAttribute]
    public Boolean DefaultAlly;
    [Memoria.PatchableFieldAttribute]
    public TargetDisplay DisplayStats;
    [Memoria.PatchableFieldAttribute]
    public Int16 VfxIndex;
    [Memoria.PatchableFieldAttribute]
    public Boolean ForDead;
    [Memoria.PatchableFieldAttribute]
    public Boolean DefaultCamera;
    [Memoria.PatchableFieldAttribute]
    public Boolean DefaultOnDead;

    [Memoria.PatchableFieldAttribute]
    public String SequenceFile;
    public UnifiedBattleSequencer.BattleAction VfxAction;

    public BattleCommandInfo()
    {
        Target = 0;
        DefaultAlly = false;
        DisplayStats = 0;
        VfxIndex = 0;
        ForDead = false;
        DefaultCamera = false;
        DefaultOnDead = false;
        SequenceFile = null;
        VfxAction = null;
    }

    public BattleCommandInfo(TargetType target, Boolean defaultAlly, TargetDisplay displayStats, Int16 vfxIndex, Boolean forDead, Boolean defaultCamera, Boolean defaultOnDead)
    {
        Target = target;
        DefaultAlly = defaultAlly;
        DisplayStats = displayStats;
        VfxIndex = vfxIndex;
        ForDead = forDead;
        DefaultCamera = defaultCamera;
        DefaultOnDead = defaultOnDead;
        SequenceFile = null;
        VfxAction = null;
    }
}
