using System;

namespace Memoria
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class BattleScriptAttribute : Attribute
	{
		public Int32 Id { get; }

		public BattleScriptAttribute(Int32 id)
		{
			Id = id;
		}
	}
}
