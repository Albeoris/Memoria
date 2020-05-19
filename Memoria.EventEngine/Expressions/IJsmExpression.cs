using System;
using FF8.Core;
using FF8.JSM.Format;

namespace FF8.JSM
{
    public interface IJsmExpression : IFormattableScript
    {
        IJsmExpression Evaluate(IServices services);
        Int64 Calculate(IServices services);
    }

    public interface IConstExpression : ILogicalExpression
    {
        Int64 Value { get; }
    }

    public interface ILogicalExpression : IJsmExpression
    {
        ILogicalExpression LogicalInverse();
    }

    public static class ExtensionMethods
    {
        public static ILogicalExpression LogicalEvaluate(this ILogicalExpression expression, IServices services)
        {
            IJsmExpression jsm = expression.Evaluate(services);
            return (ILogicalExpression)jsm;
        }

        public static Int32 Int32(this IJsmExpression expression, IServices services)
        {
            return checked((Int32)expression.Calculate(services));
        }

        public static Boolean Boolean(this IJsmExpression expression, IServices services)
        {
            Int32 value = Int32(expression, services);
            if (value == 0)
                return false;
            if (value == 1)
                return true;

            throw new NotSupportedException($"Cannot convert value {value} to Boolean.");
        }

        public static Int32 Int32(this IConstExpression expression)
        {
            return checked((Int32)expression.Value);
        }

        public static Boolean Boolean(this IConstExpression expression)
        {
            if (expression.Value == 0)
                return false;
            if (expression.Value == 1)
                return true;

            throw new NotSupportedException($"Cannot convert value {expression.Value} to Boolean.");
        }
    }
}