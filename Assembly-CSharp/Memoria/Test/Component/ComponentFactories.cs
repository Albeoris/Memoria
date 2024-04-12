using System;
using System.Collections;
using System.Collections.Generic;

namespace Memoria.Test
{
	internal sealed partial class ComponentFactories : IEnumerable
	{
		private readonly List<ComponentFactoryCreateDelegate> _creators = new List<ComponentFactoryCreateDelegate>();
		private readonly List<ComponentFactoryWrapDelegate> _wrappers = new List<ComponentFactoryWrapDelegate>();
		private readonly Dictionary<Type, Int32> _map = new Dictionary<Type, Int32>();

		public void Add(Type type, ComponentFactoryCreateDelegate createFactory, ComponentFactoryWrapDelegate wrapFactory)
		{
			Int32 index = _creators.Count;
			_creators.Add(createFactory);
			_wrappers.Add(wrapFactory);
			_map.Add(type, index);
		}

		public void Link(Type type, Int32 index)
		{
			_map.Add(type, index);
		}

		public Boolean TryGetIndex(Type type, out Int32 index)
		{
			return _map.TryGetValue(type, out index);
		}

		public Boolean TryGetCreator(Int32 index, out ComponentFactoryCreateDelegate factory)
		{
			if (index >= 0 && index < _creators.Count)
			{
				factory = _creators[index];
				return true;
			}

			factory = null;
			return false;
		}

		public Boolean TryGetWrapper(Type type, out Int32 index, out ComponentFactoryWrapDelegate factory)
		{
			if (_map.TryGetValue(type, out index))
			{
				factory = _wrappers[index];
				return true;
			}

			factory = null;
			return false;
		}

		public ComponentFactoryWrapDelegate GetWrapper(Int32 index)
		{
			return _wrappers[index];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _map.Keys.GetEnumerator();
		}
	}
}
