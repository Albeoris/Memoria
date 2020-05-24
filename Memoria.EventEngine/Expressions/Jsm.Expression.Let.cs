using System;
using FF8.Core;
using FF8.JSM.Format;
using FF8.JSM.Instructions;
using Memoria.EventEngine.EV;
using Memoria.Prime.Collections;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public sealed class Let : JsmInstruction, IValueExpression
            {
                private readonly IVariableExpression _variable;
                private readonly IValueExpression _value;

                public Let(IVariableExpression variable, IValueExpression value)
                {
                    _variable = variable;
                    _value = value;
                }

                public static void Evaluate(JsmExpressionEvaluator ev)
                {
                    IValueExpression value = ev.PopValue();
                    IVariableExpression variable = (IVariableExpression) ev.Pop();
                    ev.Push(new Let(variable, value));
                }

                public override String ToString()
                {
                    return $"{GetType().Name}({_variable} = {_value})";
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    sw.Format(formatterContext, services)
                        .StaticType(nameof(IVariablesService))
                        .Method(nameof(IVariablesService.Set))
                        .Argument("variable", _variable)
                        .Argument("value", _value)
                        .Comment(nameof(Let));
                }
            }
        }
    }
}