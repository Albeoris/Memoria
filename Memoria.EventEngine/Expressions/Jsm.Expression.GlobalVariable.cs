using System;
using FF8.Core;
using FF8.JSM.Format;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public sealed class GlobalVariable : IVariableExpression
            {
                private GlobalVariableId _globalVariable;

                public GlobalVariable(GlobalVariableId globalVariable)
                {
                    _globalVariable = globalVariable;
                }

                public override String ToString()
                {
                    return $"{nameof(GlobalVariable)}({nameof(_globalVariable)}: {_globalVariable})";
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    throw new NotImplementedException();
                    // FormatHelper.FormatGlobalGet(_globalVariable, Jsm.GlobalUInt8, sw, formatterContext, services);
                }

                public IJsmExpression Evaluate(IServices services)
                {
                    IVariablesService global = ServiceId.Variables[services];
                    if (global.IsSupported)
                    {
                        var value = global.Get(_globalVariable);
                        return ValueExpression.Create(value);
                    }
                    return this;
                }

                public Int64 Calculate(IServices services)
                {
                    return ServiceId.Variables[services].Get(_globalVariable);
                }
            }
        }
    }
}