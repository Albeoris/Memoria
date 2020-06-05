using System;
using FF8.Core;
using FF8.JSM.Format;
using Memoria.EventEngine.EV;
using Memoria.Prime.Collections;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public sealed class BooleanAnd : IValueExpression
            {
                private readonly IValueExpression _value1;
                private readonly IValueExpression _value2;

                private BooleanAnd(IValueExpression value1, IValueExpression value2)
                {
                    _value1 = value1;
                    _value2 = value2;
                }

                public static void Evaluate(JsmExpressionEvaluator ev)
                {
                    IValueExpression value2 = (IValueExpression) ev.Pop();
                    IValueExpression value1 = (IValueExpression) ev.Pop();

                    if (value1 is IConstExpression c1)
                    {
                        if (!c1.IsTrue())
                            ev.Push(ValueExpression.Boolean(false));
                        else if (value2 is IConstExpression c2)
                            ev.Push(ValueExpression.Boolean(c2.IsTrue()));
                        else
                            ev.Push(value2);
                    }
                    else
                    {
                        ev.Push(new BooleanAnd(value1, value2));
                    }
                }

                public override String ToString()
                {
                    return $"{GetType().Name}({_value1} && {_value2})";
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    sw.Append("(");
                    _value1.Format(sw, formatterContext, services);
                    sw.Append(" && ");
                    _value2.Format(sw, formatterContext, services);
                    sw.Append(")");
                }
            }
        }
    }
}