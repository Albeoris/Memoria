using System;
using FF8.Core;
using FF8.JSM.Format;
using Memoria.EventEngine.Execution;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public sealed class VariableExpression : IVariableExpression
            {
                public Int26 Value { get; }

                public VariableExpression(Int26 value)
                {
                    Value = value;
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    switch (Value.Type)
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

                    switch (Value.Source)
                    {
                        case VariableSource.Global:
                            sw.Append($"Global[{Value.Value}]");
                            break;
                        case VariableSource.Map:
                            sw.Append($"Global[{Value.Value}]");
                            break;
                        case VariableSource.Instance:
                            sw.Append($"Instance[{Value.Value}]");
                            break;
                        case VariableSource.Object:
                            sw.Append($"Object[{Value.Value}]");
                            break;
                        case VariableSource.System:
                            sw.Append($"System[{Value.Value}]");
                            break;
                        case VariableSource.Member:
                            sw.Append($"Member[{Value.Value}]");
                            break;
                        case VariableSource.Const:
                            sw.Append($"{Value.Value}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public override String ToString()
                {
                    ScriptWriter sw = new ScriptWriter(capacity: 16);
                    Format(sw, DummyFormatterContext.Instance, StatelessServices.Instance);
                    return sw.Release();
                }

                public IJsmExpression Evaluate()
                {
                    if (Value.Source == VariableSource.Const)
                        return new ValueExpression(Value.Value, Value.Type);

                    return new Evaluated(Value);
                }

                private sealed class Evaluated : IValueExpression
                {
                    private readonly Int26 _value;

                    public Evaluated(Int26 value)
                    {
                        _value = value;
                    }

                    public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                    {
                        switch (_value.Type)
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

                        switch (_value.Source)
                        {
                            case VariableSource.Global:
                                sw.Append($"Global[{_value.Value}]");
                                break;
                            case VariableSource.Map:
                                sw.Append($"Global[{_value.Value}]");
                                break;
                            case VariableSource.Instance:
                                sw.Append($"Instance[{_value.Value}]");
                                break;
                            case VariableSource.Object:
                                sw.Append($"Object[{_value.Value}]");
                                break;
                            case VariableSource.System:
                                sw.Append($"System[{_value.Value}]");
                                break;
                            case VariableSource.Member:
                                sw.Append($"Member[{_value.Value}]");
                                break;
                            case VariableSource.Const:
                                sw.Append($"{_value.Value}");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }
    }
}