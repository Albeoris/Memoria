using Memoria.Data;
using Memoria.DefaultScripts;

namespace Memoria.Scripts
{
    public static class DefaultStatusDatabase
    {
        public static StatusScriptBase Get(BattleStatusId statusId)
        {
            switch (statusId)
            {
                case BattleStatusId.Venom: return new VenomStatusScript();
                case BattleStatusId.Petrify: return new PetrifyStatusScript();
                case BattleStatusId.Virus: return new VirusStatusScript();
                case BattleStatusId.Silence: return new SilenceStatusScript();
                case BattleStatusId.Blind: return new BlindStatusScript();
                case BattleStatusId.Trouble: return new TroubleStatusScript();
                case BattleStatusId.Zombie: return new ZombieStatusScript();
                case BattleStatusId.EasyKill: return new EasyKillStatusScript();
                case BattleStatusId.Death: return new DeathStatusScript();
                case BattleStatusId.LowHP: return new LowHPStatusScript();
                case BattleStatusId.Confuse: return new ConfuseStatusScript();
                case BattleStatusId.Berserk: return new BerserkStatusScript();
                case BattleStatusId.Stop: return new StopStatusScript();
                case BattleStatusId.AutoLife: return new AutoLifeStatusScript();
                case BattleStatusId.Trance: return new TranceStatusScript();
                case BattleStatusId.Defend: return new DefendStatusScript();
                case BattleStatusId.Poison: return new PoisonStatusScript();
                case BattleStatusId.Sleep: return new SleepStatusScript();
                case BattleStatusId.Regen: return new RegenStatusScript();
                case BattleStatusId.Haste: return new HasteStatusScript();
                case BattleStatusId.Slow: return new SlowStatusScript();
                case BattleStatusId.Float: return new FloatStatusScript();
                case BattleStatusId.Shell: return new ShellStatusScript();
                case BattleStatusId.Protect: return new ProtectStatusScript();
                case BattleStatusId.Heat: return new HeatStatusScript();
                case BattleStatusId.Freeze: return new FreezeStatusScript();
                case BattleStatusId.Vanish: return new VanishStatusScript();
                case BattleStatusId.Doom: return new DoomStatusScript();
                case BattleStatusId.Mini: return new MiniStatusScript();
                case BattleStatusId.Reflect: return new ReflectStatusScript();
                case BattleStatusId.Jump: return new JumpStatusScript();
                case BattleStatusId.GradualPetrify: return new GradualPetrifyStatusScript();
                case BattleStatusId.ChangeStat: return new ChangeStatStatusScript();
            }
            return null;
        }
    }
}
