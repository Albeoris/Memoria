using System;
using FF8.Core;
using FF8.JSM.Format;
using FF8.JSM.Instructions;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public sealed class GetAngle : JsmInstruction, IValueExpression
            {
                private readonly IValueExpression _value1;
                private readonly IValueExpression _value2;

                public GetAngle(IValueExpression value1, IValueExpression value2)
                {
                    _value1 = value1;
                    _value2 = value2;
                }

                public static void Evaluate(JsmExpressionEvaluator evaluator)
                {
                    IValueExpression value1 = (IValueExpression) evaluator.Pop();
                    IValueExpression value2 = (IValueExpression) evaluator.Pop();

                    evaluator.Push(new GetAngle(value1, value2));
                }

                public override String ToString()
                {
                    return $"{nameof(GetAngle)}({_value1}, {_value2})";
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    sw.Format(formatterContext, services)
                        .Method(nameof(GetAngle))
                        .Argument("value1", _value1)
                        .Argument("value2", _value2)
                        .Comment(nameof(GetAngle));
                }
            }
        }
    }
}