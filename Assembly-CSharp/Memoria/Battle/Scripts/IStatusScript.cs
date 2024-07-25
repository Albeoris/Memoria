using System;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Text;

namespace Memoria
{
    public abstract class StatusScriptBase
    {
        /// <summary>The unit on which the status has been inflicted, provided that base.Apply is called correctly</summary>
        public BattleUnit Target = null;

        /// <summary>Retrieve any public non-static field value specific to the status script code</summary>
        public T GetFieldValue<T>(String fieldName)
        {
            if (!fieldName.TryPublicFieldParse(this, out Object obj))
            {
                Log.Warning($"[StatusScriptBase] Trying to retrieve the field {fieldName} of {this} but there is no field with that name");
                return default;
            }
            if (obj is T)
                return (T)obj;
            Log.Warning($"[StatusScriptBase] The field {fieldName} of {this}, of type {obj?.GetType().Name ?? "null"}, is wrongly requested as type {typeof(T)}");
            return default;
        }

        /// <summary>The code executed when applying or re-applying the status</summary>
        /// <returns>Whether the status has been applied</returns>
        public virtual UInt32 Apply(BattleUnit target, BattleUnit inflicter = null, params Object[] parameters)
        {
            if (Target == null)
                Target = target;
            return btl_stat.ALTER_SUCCESS;
        }

        /// <summary>The code executed when removing a status</summary>
        /// <returns>A confirmation of the status removal</returns>
        public virtual Boolean Remove()
        {
            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class StatusScriptAttribute : Attribute
    {
        public BattleStatusId Id { get; }

        public StatusScriptAttribute(BattleStatusId id)
        {
            Id = id;
        }
    }

    public interface IOprStatusScript
    {
        /// <summary>A code executed at every Opr ticks (eg. Regen or Poison ticks)</summary>
        /// <returns>Whether the status should be removed</returns>
        public Boolean OnOpr();

        /// <summary>An optional method that determines the cnt.opr for the status (in case "StatusTickFormula" is not flexible enough)</summary>
        public SetupOprMethod SetupOpr { get; }
        public delegate Int32 SetupOprMethod();
    }

    public interface IDeathChangerStatusScript
    {
        /// <summary>A code executed when a player character touches the ground, right before a Game Over may trigger</summary>
        /// <returns>Whether the status Death has been removed</returns>
        public Boolean OnDeath();
    }

    public interface IAutoAttackStatusScript
    {
        /// <summary>A code executed when the ATB is filled</summary>
        /// <returns>Whether an auto-attack has been issued</returns>
        public Boolean OnATB();
    }

    public interface IFinishCommandScript
    {
        /// <summary>A code executed when a command used by the character under status ailment ends</summary>
        public void OnFinishCommand(CMD_DATA cmd, Int32 tranceDecrease);
    }

    public interface ITroubleStatusScript
    {
        /// <summary>A code executed at the figure point (ie. when damage is displayed) if the flag FIG_INFO_TROUBLE was raised</summary>
        public void OnTroubleDamage(UInt16 fig_info, Int32 fig, Int32 m_fig);
    }
}
