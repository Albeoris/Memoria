using Memoria.Prime.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Memoria.Prime.Ini
{
    public abstract class IniSection
    {
        public readonly String Name;

        public readonly IniValue<Boolean> Enabled;

        private readonly List<ValueBinding> _bindings = new List<ValueBinding>();

        protected IniSection(String name, Boolean isEnabled)
        {
            Name = name.TrimEnd("Section", StringComparison.InvariantCulture);
            Enabled = BindBoolean(nameof(Enabled), isEnabled);
        }

        protected IniValue<Boolean> BindBoolean(String name, Boolean defaultValue)
        {
            IniValue<Boolean> handler = IniValue.Boolean(name);
            handler.Value = defaultValue;
            _bindings.Add(new ValueBinding<Boolean>(handler));
            return handler;
        }

        protected IniValue<Int32> BindInt32(String name, Int32 defaultValue)
        {
            IniValue<Int32> handler = IniValue.Int32(name);
            handler.Value = defaultValue;
            _bindings.Add(new ValueBinding<Int32>(handler));
            return handler;
        }

        protected IniSet<Int32> BindInt32Set(String name, HashSet<Int32> defaultValue)
        {
            IniSet<Int32> handler = IniValue.Int32Set(name);
            handler.Value = defaultValue;
            _bindings.Add(new SetBinding<Int32>(handler));
            return handler;
        }

        protected IniValue<String> BindPath(String name, String defaultValue)
        {
            if (IniValue.TryParsePath(defaultValue, out var path))
                defaultValue = path;

            IniValue<String> handler = IniValue.Path(name);
            handler.Value = defaultValue;
            _bindings.Add(new ValueBinding<String>(handler));
            return handler;
        }

        protected IniValue<String> BindString(String name, String defaultValue)
        {
            IniValue<String> handler = IniValue.String(name);
            handler.Value = defaultValue;
            _bindings.Add(new ValueBinding<String>(handler));
            return handler;
        }

        protected IniArray<String> BindStringArray(String name, String[] defaultValue)
        {
            IniArray<String> handler = IniValue.StringArray(name);
            handler.Value = defaultValue;
            _bindings.Add(new ArrayBinding<String>(handler));
            return handler;
        }

        internal void Reset()
        {
            IEnumerable<ValueBinding> skipEnabled = _bindings.Skip(1);
            foreach (ValueBinding handler in skipEnabled)
                handler.Reset();
        }

        internal IEnumerable<IniValue> GetValues()
        {
            return _bindings.Select(b => b.Value);
        }

        private abstract class ValueBinding
        {
            public readonly IniValue Value;

            protected ValueBinding(IniValue handler)
            {
                Value = handler;
            }

            public abstract void Reset();
        }

        private sealed class ValueBinding<T> : ValueBinding
        {
            private readonly T _initialValue;
            private readonly IniValue<T> _handler;

            public ValueBinding(IniValue<T> handler)
                : base(handler)
            {
                _handler = handler;
                _initialValue = handler.Value;
            }

            public override void Reset()
            {
                _handler.Value = _initialValue;
            }
        }

        private sealed class ArrayBinding<T> : ValueBinding
        {
            private readonly T[] _initialValue;
            private readonly IniArray<T> _handler;

            public ArrayBinding(IniArray<T> handler)
                : base(handler)
            {
                _handler = handler;
                _initialValue = handler.Value;
            }

            public override void Reset()
            {
                _handler.Value = _initialValue;
            }
        }

        private sealed class SetBinding<T> : ValueBinding
        {
            private readonly HashSet<T> _initialValue;
            private readonly IniSet<T> _handler;

            public SetBinding(IniSet<T> handler)
                : base(handler)
            {
                _handler = handler;
                _initialValue = handler.Value;
            }

            public override void Reset()
            {
                _handler.Value = _initialValue;
            }
        }
    }
}
