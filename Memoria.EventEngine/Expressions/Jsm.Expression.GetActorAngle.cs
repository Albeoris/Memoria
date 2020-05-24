using System;
using FF8.Core;
using FF8.JSM.Format;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public sealed class GetActorAngle : IValueExpression
            {
                private readonly IValueExpression _value1;
                private readonly IValueExpression _value2;

                public GetActorAngle(IValueExpression value1, IValueExpression value2)
                {
                    _value1 = value1;
                    _value2 = value2;
                }

                public static void Evaluate(JsmExpressionEvaluator evaluator)
                {
                    IValueExpression value1 = (IValueExpression) evaluator.Pop();
                    IValueExpression value2 = (IValueExpression) evaluator.Pop();

                    evaluator.Push(new GetActorAngle(value1, value2));
                }

                public override String ToString()
                {
                    return $"{nameof(GetActorAngle)}({_value1}, {_value2})";
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    sw.Append(ToString());
                }
            }
        }
    }
}