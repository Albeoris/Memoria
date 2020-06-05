using System;
using FF8.Core;
using FF8.JSM.Format;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public sealed class SystemVariable : IVariableExpression
            {
                private SystemVariableId _systemVariable;

                public SystemVariable(SystemVariableId systemVariable)
                {
                    _systemVariable = systemVariable;
                }

                public override String ToString()
                {
                    return $"{nameof(SystemVariable)}({nameof(_systemVariable)}: {_systemVariable})";
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
                        var value = variableService.Get(_systemVariable);
                        return ValueExpression.Create(value);
                    }
                    return this;
                }

                public Int64 Calculate(IServices services)
                {
                    return ServiceId.Variables[services].Get(_systemVariable);
                }
            }
        }
    }
}