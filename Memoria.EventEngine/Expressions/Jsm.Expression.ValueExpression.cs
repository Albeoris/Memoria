using System;
using System.Globalization;
using FF8.Core;
using FF8.JSM.Format;
using Memoria.EventEngine.Execution;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public sealed class ValueExpression : IConstExpression
            {
                public Int64 Value { get; }

                private readonly TypeCode _typeCode;

                public static ValueExpression Create(Int32 value) => new ValueExpression(value, TypeCode.Int32);
                public static ValueExpression Create(UInt32 value) => new ValueExpression(value, TypeCode.UInt32);
                public static ValueExpression Create(Int16 value) => new ValueExpression(value, TypeCode.Int16);
                public static ValueExpression Create(UInt16 value) => new ValueExpression(value, TypeCode.UInt16);
                public static ValueExpression Create(Byte value) => new ValueExpression(value, TypeCode.Byte);
                public static ValueExpression Create(SByte value) => new ValueExpression(value, TypeCode.SByte);

                private ValueExpression(Int64 value, TypeCode typeCode)
                {
                    Value = value;
                    _typeCode = typeCode;

                    if (_typeCode < TypeCode.SByte || typeCode > TypeCode.UInt32)
                        throw new ArgumentOutOfRangeException($"Type {typeCode} isn't supported.", nameof(typeCode));
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    switch (_typeCode)
                    {
                        case TypeCode.SByte:
                            sw.Append("(SByte)");
                            break;
                        case TypeCode.Byte:
                            sw.Append("(Byte)");
                            break;
                        case TypeCode.Int16:
                            sw.Append("(Int16)");
                            break;
                        case TypeCode.UInt16:
                            sw.Append("(UInt16)");
                            break;
                    }

                    sw.Append(Value.ToString(CultureInfo.InvariantCulture));

                    if (_typeCode == TypeCode.UInt32)
                        sw.Append("u");
                }

                public IJsmExpression Evaluate(IServices services)
                {
                    return this;
                }

                public Int64 Calculate(IServices services)
                {
                    return Value;
                }

                public ILogicalExpression LogicalInverse()
                {
                    return new ValueExpression(Value == 0 ? 1 : 0, TypeCode.Int32);
                }

                public override String ToString()
                {
                    ScriptWriter sw = new ScriptWriter(capacity: 16);
                    Format(sw, DummyFormatterContext.Instance, StatelessServices.Instance);
                    return sw.Release();
                }
            }
        }
    }
}