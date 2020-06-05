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
            public sealed class Math : IValueExpression
            {
                private readonly IValueExpression _value1;
                private readonly IValueExpression _value2;
                private readonly Type _type;

                public Math(IValueExpression value1, IValueExpression value2, Type type)
                {
                    _value1 = value1;
                    _value2 = value2;
                    _type = type;
                }

                public static void Evaluate(JsmExpressionEvaluator ev, Type type)
                {
                    IValueExpression value2 = (IValueExpression) ev.Pop();
                    IValueExpression value1 = (IValueExpression) ev.Pop();

                    if (value1 is IConstExpression c1 && value2 is IConstExpression c2)
                    {
                        Int64 result = Calc(c1.Value, c2.Value, type);
                        ev.Push(ValueExpression.Create(result));
                    }
                    else
                    {
                        ev.Push(new Math(value1, value2, type));
                    }
                }

                public override String ToString()
                {
                    return $"{GetType().Name}({_value1} {FormatOperand()} {_value2})";
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    sw.Append("(");
                    _value1.Format(sw, formatterContext, services);
                    sw.Append(" ");
                    sw.Append(FormatOperand());
                    sw.Append(" ");
                    _value2.Format(sw, formatterContext, services);
                    sw.Append(")");
                }
                
                public String FormatOperand()
                {
                    return _type switch
                    {
                        Type.Mul => "*",
                        Type.Div => "/",
                        Type.Mod => "%",
                        Type.Add => "+",
                        Type.Sub => "-",
                        Type.BitAnd => "&",
                        Type.BitXor => "^",
                        Type.BitOr => "|",
                        Type.BitLeft => "<<",
                        Type.BitRight => ">>",
                        _ => throw new ArgumentOutOfRangeException(nameof(_type), _type, null)
                    };
                }

                private static Int64 Calc(in Int64 v1, in Int64 v2, in Type type)
                {
                    return type switch
                    {
                        Type.Mul => v1 * v2,
                        Type.Div => v1 / v2,
                        Type.Mod => v1 % v2,
                        Type.Add => v1 + v2,
                        Type.Sub => v1 - v2,
                        Type.BitAnd => v1 & v2,
                        Type.BitXor => v1 ^ v2,
                        Type.BitOr => v1 | v2,
                        Type.BitLeft => v1 << BitNumber(v2),
                        Type.BitRight => v1 >> BitNumber(v2),
                        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                    };
                }

                private static Int32 BitNumber(in Int64 bitNumber)
                {
                    if (bitNumber < 0 || bitNumber > 23)
                        throw new ArgumentOutOfRangeException(nameof(bitNumber), bitNumber, "if (bitNumber < 0 || bitNumber > 23)");

                    return (Int32) bitNumber;
                }
                
                public static void Mul(JsmExpressionEvaluator ev) => Evaluate(ev, Type.Mul);
                public static void Div(JsmExpressionEvaluator ev) => Evaluate(ev, Type.Div);
                public static void Mod(JsmExpressionEvaluator ev) => Evaluate(ev, Type.Mod);
                public static void Add(JsmExpressionEvaluator ev) => Evaluate(ev, Type.Add);
                public static void Sub(JsmExpressionEvaluator ev) => Evaluate(ev, Type.Sub);

                public static void BitAnd  (JsmExpressionEvaluator ev) => Evaluate(ev, Type.BitAnd);
                public static void BitXor  (JsmExpressionEvaluator ev) => Evaluate(ev, Type.BitXor);
                public static void BitOr   (JsmExpressionEvaluator ev) => Evaluate(ev, Type.BitOr);
                public static void BitLeft (JsmExpressionEvaluator ev) => Evaluate(ev, Type.BitLeft);
                public static void BitRight(JsmExpressionEvaluator ev) => Evaluate(ev, Type.BitRight);

                public enum Type
                {
                    Mul,
                    Div,
                    Mod,
                    Add,
                    Sub,
                    BitAnd,
                    BitXor,
                    BitOr,
                    BitLeft,
                    BitRight
                }
            }
        }
    }
}