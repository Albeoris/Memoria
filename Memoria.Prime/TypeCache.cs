using System;

namespace Memoria.Prime
{
	public static class TypeCache<T>
	{
		public static readonly Type Type = typeof(T);
		public static readonly TypeCode TypeCode = Type.GetTypeCode(Type);
	}
}
