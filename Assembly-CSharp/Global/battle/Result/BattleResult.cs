using System;
using System.Collections.Generic;

public class BattleResult
{
    public BattleResult()
    {
        this.type = BattleResult.Type.NOTHING;
        this.calculation = new BattleCalculation();
    }

    public BattleResult.Type type;

    public BattleCalculation calculation;

    public QuadMistCard attacker;

    public QuadMistCard defender;

    public List<QuadMistCard> beats;

    public QuadMistCard[] combos;

    public enum Type
    {
        WIN,
        LOSE,
        NOTHING
    }
}
