using System;
using FF8.JSM;

namespace FF8.Core
{
    public struct MapVariableId
    {
        public Jsm.Expression.VariableType Type { get; }
        public Int32 VariableId { get; }

        public MapVariableId(Int32 variableId, Jsm.Expression.VariableType type)
        {
            if (variableId < 0)
                throw new ArgumentOutOfRangeException(nameof(variableId), $"Invalid map variable id: {variableId}");
            
            Type = type;
            VariableId = variableId;
        }

        public String TypeName => Type.ToString();

        public override String ToString()
        {
            return $"Map({Type}): {VariableId}";
        }
    }
}