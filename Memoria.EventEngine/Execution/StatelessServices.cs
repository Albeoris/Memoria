using System;
using FF8.Core;

namespace Memoria.EventEngine.Execution
{
    public sealed class StatelessServices : IServices
    {
        public static IServices Instance { get; } = new StatelessServices();

        private StatelessServices()
        {
        }

        public T Service<T>(ServiceId<T> id)
        {
            return (T)(Object)id;
        }
    }
}