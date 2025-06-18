using Memoria.Data;
using System;
using UnityEngine;

namespace Memoria
{
    public interface IOverloadUnitCheckPointScript
    {
        /// <summary>Update the current state of a unit based on its HP (typically handles the LowHP status)</summary>
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

    public interface IOverloadOnBattleInitScript
    {
        /// <summary>Run a code at battle initialisation time, right after the BattleUnits have all been initialised</summary>
        public void OnBattleInit();
    }

    public interface IOverloadOnBattleScriptStartScript
    {
        /// <summary>Run a code before any battle script, right after the SA features "WhenBattleScriptStart" (typically handles killing frozen targets)</summary>
        /// <returns>Whether the battle script should be skipped directly to "BattleScriptEnd"</returns>
        public Boolean OnBattleScriptStart(BattleCalculator calc);
    }

    public interface IOverloadOnBattleScriptEndScript
    {
        /// <summary>Run a code at the end of a battle script, at the end of SBattleCalculator.CalcResult(v)</summary>
        public void OnBattleScriptEnd(BattleCalculator v);
    }

    public interface IOverloadOnCommandRunScript
    {
        /// <summary>Run a code when a command starts (typically handles killing acting characters with Heat)</summary>
        /// <returns>Whether the command should be canceled</returns>
        public Boolean OnCommandRun(BattleCommand cmd);
    }

    public interface IOverloadOnGameOverScript
    {
        /// <summary>Run a code in a Game Over situation (typically handles Eiko's automatic Rebirth Flame)</summary>
        /// <param name="dyingPC">The player character for which the situation just changed (KO, petrified...)</param>
        /// <returns>Whether the game over should be canceled</returns>
        public Boolean OnGameOver(FF9StateBattleSystem state, BattleUnit dyingPC);
    }

    public interface IOverloadOnFleeScript
    {
        /// <summary>Run a code when successfully fleeing, either with an ability or with the bumper buttons (typically handles gil loss when fleeing with an ability)</summary>
        public void OnFlee(FF9StateGlobal state);
    }

    public interface IOverloadDamageModifierScript
    {
        /// <summary>The effect of increasing/decreasing DamageModifierCount</summary>
        public void OnDamageModifierChange(BattleCalculator v, Int32 previousValue, Int32 bonus);

        /// <summary>The effect of drastically reducing damage (typically a physical attack under Mini)</summary>
        public void OnDamageDrasticReduction(BattleCalculator v);

        /// <summary>The last modifications applied to target HPDamage, between WhenBattleScriptEnd and WhenEffectDone</summary>
        public void OnDamageFinalChanges(BattleCalculator v);
    }

    public interface IOverloadVABattleScript
    {
        /// <summary>Called to initialize the voice acting battle script (subscribe to BattleVoice events there)</summary>
        public void Initialize();
    }
}
