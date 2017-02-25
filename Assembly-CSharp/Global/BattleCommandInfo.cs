using System;
using Memoria.Data;

public class BattleCommandInfo
{
    public TargetType Target;
    public Boolean DefaultAlly;
    public TargetDisplay DisplayStats;
    public Int16 VfxIndex;
    public Boolean ForDead;
    public Boolean DefaultCamera;
    public Boolean DefaultOnDead;

    public BattleCommandInfo()
	{
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
	}
}
