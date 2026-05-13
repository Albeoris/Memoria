using Memoria.Prime;
using Memoria.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Memoria
{
    public class MergingDamageModifierScript : IOverloadDamageModifierScript
    {
        private readonly IEnumerable<IOverloadDamageModifierScript> _scripts;

        public MergingDamageModifierScript(IEnumerable<IOverloadDamageModifierScript> scripts)
        {
            _scripts = scripts.ToArray();
        }
        public Boolean HasLoaded => _scripts.Any();
        public Int32 ScriptCount => _scripts.Count();

        public void OnDamageModifierChange(BattleCalculator v, Int32 previousValue, Int32 bonus)
        {
            foreach (var script in _scripts)
                script.OnDamageModifierChange(v, previousValue, bonus);
        }

        public void OnDamageDrasticReduction(BattleCalculator v)
        {
            foreach (var script in _scripts)
                script.OnDamageDrasticReduction(v);
        }

        public void OnDamageFinalChanges(BattleCalculator v)
        {
            foreach (var script in _scripts)
                script.OnDamageFinalChanges(v);
        }
    }

    public class MergingOnBattleInitScript : IOverloadOnBattleInitScript
    {
        private readonly IEnumerable<IOverloadOnBattleInitScript> _scripts;

        public MergingOnBattleInitScript(IEnumerable<IOverloadOnBattleInitScript> scripts)
        {
            _scripts = scripts.ToArray();
        }
        public Boolean HasLoaded => _scripts.Any();
        public Int32 ScriptCount => _scripts.Count();

        public void OnBattleInit()
        {
            foreach (var script in _scripts)
                script.OnBattleInit();
        }
    }

    public class MergingOnBattleScriptEndScript : IOverloadOnBattleScriptEndScript
    {
        private readonly IEnumerable<IOverloadOnBattleScriptEndScript> _scripts;

        public MergingOnBattleScriptEndScript(IEnumerable<IOverloadOnBattleScriptEndScript> scripts)
        {
            _scripts = scripts.ToArray();
        }
        public Boolean HasLoaded => _scripts.Any();
        public Int32 ScriptCount => _scripts.Count();

        public void OnBattleScriptEnd(BattleCalculator v)
        {
            foreach (var script in _scripts)
                script.OnBattleScriptEnd(v);
        }
    }

    public class MergingOnFleeScript : IOverloadOnFleeScript
    {
        private readonly IEnumerable<IOverloadOnFleeScript> _scripts;

        public MergingOnFleeScript(IEnumerable<IOverloadOnFleeScript> scripts)
        {
            _scripts = scripts.ToArray();
        }

        public Boolean HasLoaded => _scripts.Any();
        public Int32 ScriptCount => _scripts.Count();

        public void OnFlee(FF9StateGlobal state)
        {
            foreach (var script in _scripts)
                script.OnFlee(state);
        }
    }

    public class MergingOnBattleScriptStartScript : IOverloadOnBattleScriptStartScript
    {
        private readonly IEnumerable<IOverloadOnBattleScriptStartScript> _scripts;

        public MergingOnBattleScriptStartScript(IEnumerable<IOverloadOnBattleScriptStartScript> scripts)
        {
            _scripts = scripts.ToArray();
        }
        public Boolean HasLoaded => _scripts.Any();
        public Int32 ScriptCount => _scripts.Count();

        public Boolean OnBattleScriptStart(BattleCalculator calc)
        {
            bool shouldSkip = false;
            foreach (var script in _scripts)
                shouldSkip |= script.OnBattleScriptStart(calc);

            return shouldSkip;
        }
    }
    public class MergingOnCommandRunScript : IOverloadOnCommandRunScript
    {
        private readonly IEnumerable<IOverloadOnCommandRunScript> _scripts;

        public MergingOnCommandRunScript(IEnumerable<IOverloadOnCommandRunScript> scripts)
        {
            _scripts = scripts.ToArray();
        }
        public Boolean HasLoaded => _scripts.Any();
        public Int32 ScriptCount => _scripts.Count();

        public Boolean OnCommandRun(BattleCommand cmd)
        {
            bool cancelCommand = false;
            foreach (var script in _scripts)
                cancelCommand |= script.OnCommandRun(cmd);

            return cancelCommand;
        }
    }

    public class MergingOnGameOverScript : IOverloadOnGameOverScript
    {
        private readonly IEnumerable<IOverloadOnGameOverScript> _scripts;

        public MergingOnGameOverScript(IEnumerable<IOverloadOnGameOverScript> scripts)
        {
            _scripts = scripts.ToArray();
        }
        public Boolean HasLoaded => _scripts.Any();
        public Int32 ScriptCount => _scripts.Count();

        public Boolean OnGameOver(FF9StateBattleSystem state, BattleUnit dyingPC)
        {
            bool cancelGameOver = false;
            foreach (var script in _scripts)
                cancelGameOver |= script.OnGameOver(state, dyingPC);

            return cancelGameOver;
        }
    }

    public static class MergingScriptsCache
    {
        public static void Initialize()
        {
        }

        public static readonly MergingDamageModifierScript DamageModifier = new MergingDamageModifierScript(
            ScriptsLoader.GetOverloadedMethods(typeof(IOverloadDamageModifierScript)).Cast<IOverloadDamageModifierScript>()
        );

        public static readonly MergingOnBattleInitScript OnBattleInit = new MergingOnBattleInitScript(
            ScriptsLoader.GetOverloadedMethods(typeof(IOverloadOnBattleInitScript)).Cast<IOverloadOnBattleInitScript>()
        );

        public static readonly MergingOnBattleScriptStartScript OnBattleScriptStart = new MergingOnBattleScriptStartScript(
            ScriptsLoader.GetOverloadedMethods(typeof(IOverloadOnBattleScriptStartScript)).Cast<IOverloadOnBattleScriptStartScript>()
        );

        public static readonly MergingOnBattleScriptEndScript OnBattleScriptEnd = new MergingOnBattleScriptEndScript(
            ScriptsLoader.GetOverloadedMethods(typeof(IOverloadOnBattleScriptEndScript)).Cast<IOverloadOnBattleScriptEndScript>()
        );

        public static readonly MergingOnCommandRunScript OnCommandRun = new MergingOnCommandRunScript(
            ScriptsLoader.GetOverloadedMethods(typeof(IOverloadOnCommandRunScript)).Cast<IOverloadOnCommandRunScript>()
        );

        public static readonly MergingOnGameOverScript OnGameOver = new MergingOnGameOverScript(
            ScriptsLoader.GetOverloadedMethods(typeof(IOverloadOnGameOverScript)).Cast<IOverloadOnGameOverScript>()
        );

        public static readonly MergingOnFleeScript OnFlee = new MergingOnFleeScript(
            ScriptsLoader.GetOverloadedMethods(typeof(IOverloadOnFleeScript)).Cast<IOverloadOnFleeScript>()
        );
    }
}
