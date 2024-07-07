using System;
using System.Linq;
using System.Collections.Generic;
using Memoria;
using Memoria.Data;

public class STAT_INFO
{
    public STAT_INFO()
    {
        this.effects = new Dictionary<BattleStatusId, StatusScriptBase>();
        this.opr = new Dictionary<BattleStatusId, Int32>();
        this.conti = new Dictionary<BattleStatusId, Int32>();
    }

    public Boolean HasDeathChangerEffect => effects.Values.Any(eff => eff is IDeathChangerStatusScript);
    public Boolean HasAutoAttackEffect => effects.Values.Any(eff => eff is IAutoAttackStatusScript);
    public Boolean HasTroubleEffect => effects.Values.Any(eff => eff is ITroubleStatusScript);

    public BattleStatus invalid;
    public BattleStatus permanent;
    public BattleStatus cur;
    public Dictionary<BattleStatusId, StatusScriptBase> effects;
    public Dictionary<BattleStatusId, Int32> opr;
    public Dictionary<BattleStatusId, Int32> conti;
}
