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
        public static ServiceId<IVariablesService> Variables { get; } = new VariablesServiceId();
        
        private sealed class VariablesServiceId : ServiceId<IVariablesService>, IVariablesService
        {
            public Int32 Get(GlobalVariableId id) => throw new NotSupportedException();
            public void Set(GlobalVariableId id, Int32 value) => throw new NotSupportedException();
            
            public Int32 Get(MapVariableId id) => throw new NotSupportedException();
            public void Set(MapVariableId id, Int32 value) => throw new NotSupportedException();
            public Int32 Get(SystemVariableId id) => throw new NotSupportedException();
            public void Set(SystemVariableId id, Int32 value) => throw new NotSupportedException();
        }
    }
}