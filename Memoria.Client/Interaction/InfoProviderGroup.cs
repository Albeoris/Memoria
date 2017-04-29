using System;
using System.Collections.Generic;

namespace Memoria.Client.Interaction
{
    public class InfoProviderGroup<T> : List<IInfoProvider<T>>, IInfoProvider<T> where T : class
    {
        public String Title { get; private set; }
        public String Description { get; private set; }

        private readonly Object _lock = new Object();
        private T _current;

        public event Action<T> InfoProvided;
        public event Action<T> InfoLost;

        public InfoProviderGroup(String title, String description)
        {
            Title = title;
            Description = description;
        }

        public T Provide()
        {
            lock (_lock)
            {
                if (_current != null)
                    return _current;

                List<Exception> exceptions = new List<Exception>();
                foreach (IInfoProvider<T> provider in this)
                {
                    try
                    {
                        SetValue(provider.Provide());
                        return _current;
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
                throw new AggregateException(exceptions);
            }
        }

        public T Refresh(IInfoProvider<T> provider)
        {
            lock (_lock)
            {
                ClearValue();
                T result = provider.Provide();

                if (Count == 0)
                {
                    Add(provider);
                }
                else if (this[0] != provider)
                {
                    Remove(provider);
                    Insert(0, provider);
                }

                return SetValue(result);
            }
        }

        public T Refresh()
        {
            lock (_lock)
            {
                ClearValue();
                return Provide();
            }
        }

        public void ClearValue()
        {
            if (!ReferenceEquals(_current, null))
                InfoLost?.Invoke(_current);
            _current = null;
        }

        public T SetValue(T value)
        {
            _current = value;
            InfoProvided?.Invoke(value);
            return value;
        }
    }
}