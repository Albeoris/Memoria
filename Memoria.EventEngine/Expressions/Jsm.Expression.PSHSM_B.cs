using System;
using FF8.Core;
using FF8.JSM.Format;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public sealed class PSHSM_B : IJsmExpression
            {
                private GlobalVariableId<SByte> _globalVariable;

                public PSHSM_B(GlobalVariableId<SByte> globalVariable)
                {
                    _globalVariable = globalVariable;
                }

                public override String ToString()
                {
                    return $"{nameof(PSHSM_B)}({nameof(_globalVariable)}: {_globalVariable})";
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    FormatHelper.FormatGlobalGet(_globalVariable, null, sw, formatterContext, services);
                }

                public IJsmExpression Evaluate(IServices services)
                {
                    IGlobalVariableService global = ServiceId.Global[services];
                    if (global.IsSupported)
                    {
                        var value = global.Get(_globalVariable);
                        return ValueExpression.Create(value);
                    }
                    return this;
                }

                public Int64 Calculate(IServices services)
                {
                    return ServiceId.Global[services].Get(_globalVariable);
                }
            }
        }
    }
}