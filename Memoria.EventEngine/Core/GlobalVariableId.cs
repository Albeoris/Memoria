using System;

namespace FF8.Core
{
    public struct GlobalVariableId<T> where T : unmanaged
    {
        public static String TypeName { get; } = typeof(T).Name;
        public static Boolean IsValid = Validate();

        public Int32 VariableId { get; }

        public GlobalVariableId(Int32 variableId)
        {
            if (variableId < 0)
                throw new ArgumentOutOfRangeException(nameof(variableId), $"Invalid global variable id: {variableId}");

            VariableId = variableId;
        }

        public override String ToString()
        {
            return $"Global({TypeName}): {VariableId}";
        }
        
        public static Boolean Validate()
        {
            unsafe
            {
                if (sizeof(T) > 8)
                    throw new NotSupportedException($"The global variable must be an integer and cannot exceed 8 bytes. {TypeName} isn't supported.");
            }
            
            return true;
        }
    }
}