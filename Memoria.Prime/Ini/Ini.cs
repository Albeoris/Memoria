using System;
using System.Collections.Generic;

namespace Memoria.Prime.Ini
{
	public abstract class Ini
	{
		private readonly List<SectionBinding> _bindings = new List<SectionBinding>();

		internal IEnumerable<SectionBinding> GetBindings()
		{
			return _bindings;
		}

		protected void BindingSection<T>(out T section, Action<T> setter) where T : IniSection, new()
		{
			Func<T> ctor = () => new T();
			section = ctor();

			SectionBinding<T> binding = new SectionBinding<T>(section.Name, ctor, setter);

			_bindings.Add(binding);
		}
	}
}
