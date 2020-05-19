using System;
using System.Globalization;
using FF8.Core;
using FF8.JSM.Format;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public sealed class PSHN_L : IConstExpression
            {
                public Int32 Value { get; }
                Int64 IConstExpression.Value => Value;

                public PSHN_L(Int32 value)
                {
                    Value = value;
                }

                public override String ToString()
                {
                    return $"{nameof(PSHN_L)}({nameof(Value)}: {Value})";
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    sw.Append(Value.ToString(CultureInfo.InvariantCulture));
                }

                public IJsmExpression Evaluate(IServices services)
                {
                    return this;
                }

                public ILogicalExpression LogicalInverse()
                {
                    return ValueExpression.Create(Value == 0 ? 1 : 0);
                }

                public Int64 Calculate(IServices services)
                {
                    return Value;
                }
            }
        }
    }
}