using System;
using System.Runtime.CompilerServices;
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
            public sealed class Comparison : IValueExpression
            {
                private readonly IValueExpression _value1;
                private readonly IValueExpression _value2;
                private readonly Type _type;

                private Comparison(IValueExpression value1, IValueExpression value2, Type type)
                {
                    _value1 = value1;
                    _value2 = value2;
                    _type = type;
                }

                private static void Evaluate(JsmExpressionEvaluator ev, Type type)
                {
                    IValueExpression value2 = (IValueExpression) ev.Pop();
                    IValueExpression value1 = (IValueExpression) ev.Pop();

                    if (value1 is IConstExpression c1 && value2 is IConstExpression c2)
                    {
                        Boolean result = Compare(c1.Value, c2.Value, type);
                        ev.Push(ValueExpression.Boolean(result));
                    }
                    else
                    {
                        ev.Push(new Comparison(value1, value2, type));
                    }
                }

                public override String ToString()
                {
                    return _type switch
                    {
                        Type.Equal => $"{GetType().Name}({_value1} == {_value2})",
                        Type.NotEqual => $"{GetType().Name}({_value1} != {_value2})",
                        Type.LessStrict => $"{GetType().Name}({_value1} < {_value2})",
                        Type.LessOrEqual => $"{GetType().Name}({_value1} <= {_value2})",
                        Type.GreatStrict => $"{GetType().Name}({_value1} > {_value2})",
                        Type.GreatOrEqual => $"{GetType().Name}({_value1} >= {_value2})",
                        _ => throw new ArgumentOutOfRangeException(nameof(_type), _type, null)
                    };
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    sw.Append("(");
                    _value1.Format(sw, formatterContext, services);
                    
                    String op = _type switch
                    {
                        Type.Equal => " == ",
                        Type.NotEqual => " != ",
                        Type.LessStrict => " < ",
                        Type.LessOrEqual => " <= ",
                        Type.GreatStrict => " > ",
                        Type.GreatOrEqual => " >= ",
                        _ => throw new ArgumentOutOfRangeException(nameof(_type), _type, null)
                    };
                    sw.Append(op);
                    
                    _value2.Format(sw, formatterContext, services);
                    sw.Append(")");
                }

                private static Boolean Compare(in Int64 v1, in Int64 v2, in Type type)
                {
                    return type switch
                    {
                        Type.Equal => v1 == v2,
                        Type.NotEqual => v1 != v2,
                        Type.LessStrict => v1 < v2,
                        Type.LessOrEqual => v1 <= v2,
                        Type.GreatStrict => v1 > v2,
                        Type.GreatOrEqual => v1 >= v2,
                        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                    };
                }

                public static void Equal       (JsmExpressionEvaluator ev) => Evaluate(ev, Type.Equal);
                public static void NotEqual    (JsmExpressionEvaluator ev) => Evaluate(ev, Type.NotEqual);
                public static void LessStrict  (JsmExpressionEvaluator ev) => Evaluate(ev, Type.LessStrict);
                public static void LessOrEqual (JsmExpressionEvaluator ev) => Evaluate(ev, Type.LessOrEqual);
                public static void GreatStrict (JsmExpressionEvaluator ev) => Evaluate(ev, Type.GreatStrict);
                public static void GreatOrEqual(JsmExpressionEvaluator ev) => Evaluate(ev, Type.GreatOrEqual);

                private enum Type
                {
                    Equal,
                    NotEqual,
                    LessStrict,
                    LessOrEqual,
                    GreatStrict,
                    GreatOrEqual,
                }
            }
        }
    }
}