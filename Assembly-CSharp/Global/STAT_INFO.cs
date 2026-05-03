using System;
using System.Linq;
using System.Collections.Generic;
using Memoria;
using Memoria.Data;

public class STAT_INFO
{
    public Boolean HasDeathChangerEffect => effects.Values.Any(eff => eff is IDeathChangerStatusScript);
    public Boolean HasAutoAttackEffect => effects.Values.Any(eff => eff is IAutoAttackStatusScript);
    public BattleStatus CurrentIncludeOnHold => cur | permanent_on_hold;

    public BattleStatus invalid;
    public BattleStatus permanent;
    public BattleStatus cur;
    public Dictionary<BattleStatusId, StatusScriptBase> effects = new Dictionary<BattleStatusId, StatusScriptBase>();
    public Dictionary<BattleStatusId, Int32> opr = new Dictionary<BattleStatusId, Int32>();
    public Dictionary<BattleStatusId, Int32> conti = new Dictionary<BattleStatusId, Int32>();
    public BattleStatus permanent_on_hold; // Statuses that cannot be applied yet but will be applied (as permanent) as soon as possible

    public StatusModifier partial_resist = new StatusModifier(0f);
    public StatusModifier duration_factor = new StatusModifier(1f);
}
