using System;
using FF8.Core;
using FF8.JSM.Format;

namespace FF8.JSM
{
    public interface IJsmExpression : IFormattableScript
    {
    }

    public interface IValueExpression : IJsmExpression
    {
    }
    
    public interface IVariableExpression : IValueExpression
    {
    }

    public interface IConstExpression : IValueExpression, ILogicalExpression
    {
        Int64 Value { get; }
    }

    public interface ILogicalExpression : IJsmExpression
    {
        ILogicalExpression LogicalInverse();
    }

    public static class ExtensionMethods
    {
        public static Int32 Int32(this IJsmExpression expression, IServices services)
        {
            throw new NotImplementedException();
            //return checked((Int32)expression.Calculate(services));
        }

        public static Boolean IsTrue(this IJsmExpression expression, IServices services)
        {
            Int32 value = Int32(expression, services);
            return value != 0;
        }

        public static Int32 Int32(this IConstExpression expression)
        {
            return checked((Int32)expression.Value);
        }

        public static Boolean IsTrue(this IConstExpression expression)
        {
            return expression.Value != 0;
        }
    }
}