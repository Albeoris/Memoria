using System;

namespace FF8.Core
{
    public interface IServices
    {
        T Service<T>(ServiceId<T> id);
    }

    public abstract class ServiceId<T>
    {
        public Boolean IsSupported => false;
        public T this[IServices services] => services.Service(this);
    }

    public static class ServiceId
    {
        public static ServiceId<IGlobalVariableService> Global { get; } = new GlobalVariableServiceId();
        
        private sealed class GlobalVariableServiceId : ServiceId<IGlobalVariableService>, IGlobalVariableService
        {
            public T Get<T>(GlobalVariableId<T> id) where T : unmanaged => throw new NotSupportedException();
            public void Set<T>(GlobalVariableId<T> id, T value) where T : unmanaged => throw new NotSupportedException();
        }
    }
}