using System.Collections.Generic;

namespace Memoria.Prime
{
	public static class ExtensionMethodsIEnumerable
	{
		public static IEnumerable<T> Append<T>(this IEnumerable<T> self, params T[] values)
		{
			foreach (T item in self)
				yield return item;
			foreach (T item in values)
				yield return item;
		}
	}
}
