using System;
using FF8.Core;
using FF8.JSM.Format;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public sealed class MapVariable : IVariableExpression
            {
                private MapVariableId _mapVariable;

                public MapVariable(MapVariableId mapVariable)
                {
                    _mapVariable = mapVariable;
                }

                public override String ToString()
                {
                    return $"{nameof(MapVariable)}({nameof(_mapVariable)}: {_mapVariable})";
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    throw new NotImplementedException();
                    // FormatHelper.FormatMapGet(_MapVariable, Jsm.MapUInt8, sw, formatterContext, services);
                }

                public IJsmExpression Evaluate(IServices services)
                {
                    var variableService = ServiceId.Variables[services];
                    if (variableService.IsSupported)
                    {
                        var value = variableService.Get(_mapVariable);
                        return ValueExpression.Create(value);
                    }
                    return this;
                }

                public Int64 Calculate(IServices services)
                {
                    return ServiceId.Variables[services].Get(_mapVariable);
                }
            }
        }
    }
}