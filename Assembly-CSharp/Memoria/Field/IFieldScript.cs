using Memoria.Data;
using System;

namespace Memoria
{
    /// <summary>The attribute required for all the field ability scripts</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class FieldAbilityScriptAttribute : Attribute
    {
        public Int32 Id { get; }
        public FieldAbilityScriptAttribute(Int32 id) { Id = id; }
    }

    /// <summary>The base class required for all the field ability scripts</summary>
    public abstract class FieldAbilityScriptBase
    {
        public virtual void Apply(FieldCalculator context)
        {
        }
    }
}
