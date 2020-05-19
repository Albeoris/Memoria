using System;
using FF8.Core;
using FF8.JSM.Format;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public sealed class PSHM_B : IJsmExpression
            {
                private GlobalVariableId<Byte> _globalVariable;

                /// <summary>
                /// Push first byte from Argument onto the stack. 
                /// </summary>
                /// <param name="globalVariable"></param>
                public PSHM_B(GlobalVariableId<Byte> globalVariable)
                {
                    _globalVariable = globalVariable;
                }

                public override String ToString()
                {
                    return $"{nameof(PSHM_B)}({nameof(_globalVariable)}: {_globalVariable})";
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    throw new NotImplementedException();
                    // FormatHelper.FormatGlobalGet(_globalVariable, Jsm.GlobalUInt8, sw, formatterContext, services);
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