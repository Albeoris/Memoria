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

                private readonly VariableType _typeCode;

                public static ValueExpression Create(Int64 value) => new ValueExpression(value, VariableType.Int24);
                public static ValueExpression Boolean(Boolean value) => new ValueExpression(value ? 1 : 0, VariableType.Int24);

                public ValueExpression(Int64 value, VariableType typeCode)
                {
                    Value = value;
                    _typeCode = typeCode;

                    if (_typeCode < VariableType.Default || typeCode > VariableType.UInt16)
                        throw new ArgumentOutOfRangeException($"Type {typeCode} isn't supported.", nameof(typeCode));
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    switch (_typeCode)
                    {
                        case VariableType.Default:
                            sw.Append("(SByte)");
                            break;
                        case VariableType.Bit:
                            sw.Append("(Boolean)");
                            break;
                        case VariableType.Int24:
                            sw.Append("(Int32)");
                            break;
                        case VariableType.UInt24:
                            sw.Append("(UInt32)");
                            break;
                        case VariableType.SByte:
                            sw.Append("(SByte)");
                            break;
                        case VariableType.Byte:
                            sw.Append("(Byte)");
                            break;
                        case VariableType.Int16:
                            sw.Append("(Int16)");
                            break;
                        case VariableType.UInt16:
                            sw.Append("(UInt16)");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    sw.Append(Value.ToString(CultureInfo.InvariantCulture));

                    if (_typeCode == VariableType.UInt24)
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
                    return new ValueExpression(Value == 0 ? 1 : 0, VariableType.Default);
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