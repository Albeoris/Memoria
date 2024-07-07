using System;
using UnityEngine;
using Memoria.Data;
using Memoria.Scripts;

namespace Memoria
{
    public interface IOverloadUnitCheckPointScript
    {
        /// <summary>Update the current state of a unit based on its HP</summary>
        public BattleStatus UpdatePointStatus(BattleUnit unit);
    }

    public interface IOverloadPlayerUIScript
    {
        /// <summary>Update some of the UI aspects in the menus</summary>
        public Result UpdatePointStatus(PLAYER player);

        public class Result
        {
            public Color ColorHP;
            public Color ColorMP;
            public Color ColorMagicStone;
        }
    }

    public interface IOverloadOnBattleScriptStartScript
    {
        /// <summary>Run a code before any battle script, right after the SA features "WhenBattleScriptStart" (typically handles killing frozen targets)</summary>
        /// <returns>Whether the battle script should be skipped directly to "BattleScriptEnd"</returns>
        public Boolean OnBattleScriptStart(BattleCalculator calc);
    }

    public interface IOverloadOnCommandRunScript
    {
        /// <summary>Run a code when a command starts (typically handles killing acting characters with Heat)</summary>
        /// <returns>Whether the command should be canceled</returns>
        public Boolean OnCommandRun(CMD_DATA cmd);
    }

    // TODO
    public interface IOverloadDamageModifierScript
    {
        /// <summary>The effect of increasing/decreasing DamageModifierCount</summary>
        public void OnDamageModifierChange(BattleCalculator v, Int32 previousValue, Int32 bonus);

        /// <summary>The effect of drastically reducing damage (typically a physical attack under Mini)</summary>
        public void OnDamageDrasticReduction(BattleCalculator v);

        /// <summary>The last modifications applied to target HPDamage, between WhenBattleScriptEnd and WhenEffectDone</summary>
        public void OnDamageFinalChanges(BattleCalculator v);

        /// <summary>The effect of decreasing the attack's hit rate</summary>
        public void OnHitRateDecrease(BattleCalculator v, Boolean isDrasticDecrease = false);
    }
}
