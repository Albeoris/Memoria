// using System;
// using FF8.Core;
// using FF8.JSM.Format;
// using Memoria.Prime.Collections;
//
// namespace FF8.JSM
// {
//     public static partial class Jsm
//     {
//         public static partial class Expression
//         {
//             public static class CAL
//             {
//                 public static IJsmExpression Read(Int32 typeValue, IStack<IJsmExpression> stack)
//                 {
//                     Operation type = (Operation)typeValue;
//
//                     IJsmExpression last = stack.Pop();
//                     switch (type)
//                     {
//                         case Operation.Minus:
//                             return new Minus(last);
//                         case Operation.BitInverse:
//                             return new BitInverse(last);
//                         case Operation.LogNot:
//                             return new LogNot((ILogicalExpression)last);
//
//                         case Operation.Add:
//                             return new Add(stack.Pop(), last);
//                         case Operation.Sub:
//                             return new Sub(stack.Pop(), last);
//                         case Operation.Mul:
//                             return new Mul(stack.Pop(), last);
//                         case Operation.Mod:
//                             return new Mod(stack.Pop(), last);
//                         case Operation.Div:
//                             return new Div(stack.Pop(), last);
//                         case Operation.Eq:
//                             return new Eq(stack.Pop(), last);
//                         case Operation.Great:
//                             return new Great(stack.Pop(), last);
//                         case Operation.GreatOrEquals:
//                             return new GreatOrEquals(stack.Pop(), last);
//                         case Operation.Less:
//                             return new Less(stack.Pop(), last);
//                         case Operation.LessOrEquals:
//                             return new LessOrEquals(stack.Pop(), last);
//                         case Operation.NotEquals:
//                             return new NotEquals(stack.Pop(), last);
//                         case Operation.And:
//                             return new BitAnd(stack.Pop(), last);
//                         case Operation.Or:
//                             return new BitOr(stack.Pop(), last);
//                         case Operation.Xor:
//                             return new BitXor(stack.Pop(), last);
//                         case Operation.LogAnd:
//                             return new LogAnd((ILogicalExpression)stack.Pop(), (ILogicalExpression)last);
//                         case Operation.LogOr:
//                             return new LogOr((ILogicalExpression)stack.Pop(), (ILogicalExpression)last);
//                         default:
//                             throw new NotSupportedException(type.ToString());
//                     }
//                 }
//
//                 public enum Operation
//                 {
//                     // Unary
//                     Minus = 5,
//                     BitInverse = 15,
//                     LogNot = 0x1000002,
//
//                     // Binary
//                     Add = 0,
//                     Sub = 1,
//                     Mul = 2,
//                     Mod = 3,
//                     Div = 4,
//                     Eq = 6,
//                     Great = 7,
//                     GreatOrEquals = 8,
//                     Less = 9,
//                     LessOrEquals = 10,
//                     NotEquals = 11,
//                     And = 12,
//                     Or = 13,
//                     Xor = 14,
//                     LogAnd = 0x1000000,
//                     LogOr = 0x1000001
//                 };
//
//                 public class Minus : IJsmExpression
//                 {
//                     private readonly IJsmExpression _value;
//
//                     public Minus(IJsmExpression value)
//                     {
//                         _value = value;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval(-{_value})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("-");
//                         _value.Format(sw, formatterContext, services);
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression result = _value.Evaluate(services);
//                         if (result is IConstExpression number)
//                             return new ValueExpression((Int32) (-number.Value), VariableType.Int24);
//                         return result;
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return -(_value.Calculate(services));
//                     }
//                 }
//
//                 public class BitInverse : IJsmExpression
//                 {
//                     private readonly IJsmExpression _value;
//
//                     public BitInverse(IJsmExpression value)
//                     {
//                         _value = value;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval(~{_value})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("~");
//                         _value.Format(sw, formatterContext, services);
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression result = _value.Evaluate(services);
//                         if (result is IConstExpression number)
//                             return new ValueExpression((Int32)~number.Value, VariableType.Int24);
//                         return result;
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return ~(_value.Calculate(services));
//                     }
//                 }
//
//                 public class LogNot : ILogicalExpression
//                 {
//                     private readonly IJsmExpression _value;
//
//                     public LogNot(IJsmExpression value)
//                     {
//                         _value = value;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval(!{_value})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("!");
//                         _value.Format(sw, formatterContext, services);
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression result = _value.Evaluate(services);
//                         if (result is IConstExpression number)
//                             return new ValueExpression(number.Value == 0 ? 1 : 0, VariableType.Bit);
//                         return result;
//                     }
//
//                     public ILogicalExpression LogicalInverse()
//                     {
//                         throw new NotImplementedException();
//                         //return _value;
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _value.Calculate(services) == 0 ? 1 : 0;
//                     }
//                 }
//
//                 public class Add : IJsmExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public Add(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} + {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" + ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create((Int32)(n1.Value + n2.Value));
//                         return new BitAnd(first, last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) + _last.Calculate(services);
//                     }
//                 }
//
//                 public class Sub : IJsmExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public Sub(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} - {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" - ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create((Int32)(n1.Value - n2.Value));
//                         return new Sub(first, last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) - _last.Calculate(services);
//                     }
//                 }
//
//                 public class Mul : IJsmExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public Mul(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} * {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" * ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create((Int32)(n1.Value * n2.Value));
//                         return new Mul(first, last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) * _last.Calculate(services);
//                     }
//                 }
//
//                 public class Mod : IJsmExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public Mod(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} % {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" % ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create((Int32)(n1.Value % n2.Value));
//                         return new Mod(first, last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) % _last.Calculate(services);
//                     }
//                 }
//
//                 public class Div : IJsmExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public Div(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} / {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" / ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create((Int32)(n1.Value / n2.Value));
//                         return new Div(first, last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) / _last.Calculate(services);
//                     }
//                 }
//
//                 public class Eq : ILogicalExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public Eq(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} == {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" == ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create(n1.Value == n2.Value ? 1 : 0);
//                         
//                         return new Eq(first, last);
//                     }
//
//                     public ILogicalExpression LogicalInverse()
//                     {
//                         return new NotEquals(_first, _last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) == _last.Calculate(services) ? 1 : 0;
//                     }
//                 }
//
//                 public class Great : ILogicalExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public Great(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} > {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" > ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create(n1.Value > n2.Value ? 1 : 0);
//                         return new Great(first, last);
//                     }
//
//                     public ILogicalExpression LogicalInverse()
//                     {
//                         return new LessOrEquals(_first, _last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) > _last.Calculate(services) ? 1 : 0;
//                     }
//                 }
//
//                 public class GreatOrEquals : ILogicalExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public GreatOrEquals(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} >= {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" >= ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create(n1.Value >= n2.Value ? 1 : 0);
//                         return new GreatOrEquals(first, last);
//                     }
//
//                     public ILogicalExpression LogicalInverse()
//                     {
//                         return new Less(_first, _last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) >= _last.Calculate(services) ? 1 : 0;
//                     }
//                 }
//
//                 public class Less : ILogicalExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public Less(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} < {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" < ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create(n1.Value < n2.Value ? 1 : 0);
//                         return new Less(first, last);
//                     }
//
//                     public ILogicalExpression LogicalInverse()
//                     {
//                         return new GreatOrEquals(_first, _last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) < _last.Calculate(services) ? 1 : 0;
//                     }
//                 }
//
//                 public class LessOrEquals : ILogicalExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public LessOrEquals(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} <= {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" <= ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create(n1.Value <= n2.Value ? 1 : 0);
//                         return new LessOrEquals(first, last);
//                     }
//
//                     public ILogicalExpression LogicalInverse()
//                     {
//                         return new Great(_first, _last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) <= _last.Calculate(services) ? 1 : 0;
//                     }
//                 }
//
//                 public class NotEquals : ILogicalExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public NotEquals(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} != {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" != ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create(n1.Value != n2.Value ? 1 : 0);
//                         return new NotEquals(first, last);
//                     }
//
//                     public ILogicalExpression LogicalInverse()
//                     {
//                         return new Eq(_first, _last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) != _last.Calculate(services) ? 1 : 0;
//                     }
//                 }
//
//                 public class BitAnd : IJsmExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public BitAnd(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} & {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" & ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create((Int32)(n1.Value & n2.Value));
//                         return new BitAnd(first, last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) & _last.Calculate(services);
//                     }
//                 }
//
//                 public class BitOr : IJsmExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public BitOr(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} | {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" | ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create((Int32)(n1.Value | n2.Value));
//                         return new BitOr(first, last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) | _last.Calculate(services);
//                     }
//                 }
//
//                 public class BitXor : IJsmExpression
//                 {
//                     private readonly IJsmExpression _first;
//                     private readonly IJsmExpression _last;
//
//                     public BitXor(IJsmExpression first, IJsmExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} ^ {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" ^ ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         IJsmExpression first = _first.Evaluate(services);
//                         IJsmExpression last = _last.Evaluate(services);
//                         if (first is IConstExpression n1 && last is IConstExpression n2)
//                             return ValueExpression.Create((Int32)(n1.Value ^ n2.Value));
//                         return new BitXor(first, last);
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return _first.Calculate(services) ^ _last.Calculate(services);
//                     }
//                 }
//
//                 public class LogAnd : ILogicalExpression
//                 {
//                     private readonly ILogicalExpression _first;
//                     private readonly ILogicalExpression _last;
//
//                     public LogAnd(ILogicalExpression first, ILogicalExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} && {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" && ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         ILogicalExpression first = _first.LogicalEvaluate(services);
//                         ILogicalExpression last = _last.LogicalEvaluate(services);
//                         if (first is IConstExpression n1)
//                         {
//                             if (n1.Value == 0)
//                             {
//                                 // (false && ??) => false
//                                 return ValueExpression.Create(1);
//                             }
//
//                             if (last is IConstExpression n2)
//                             {
//                                 if (n2.Value == 0)
//                                 {
//                                     // (true && false) => false
//                                     return ValueExpression.Create(1);
//                                 }
//                                 else
//                                 {
//                                     // (true && true) => true
//                                     return ValueExpression.Create(0);
//                                 }
//                             }
//                             else
//                             {
//                                 // (true && ??) => ??
//                                 return _last;
//                             }
//                         }
//                         else if (last is IConstExpression n2)
//                         {
//                             if (n2.Value == 0)
//                             {
//                                 // (?? && false) => false
//                                 return ValueExpression.Create(1);
//                             }
//
//                             // (?? && true) => ??
//                             return first;
//                         }
//
//                         // (?? && ??)
//                         return new LogAnd(first, last);
//                     }
//
//                     public ILogicalExpression LogicalInverse()
//                     {
//                         return new LogOr(_first.LogicalInverse(), _last.LogicalInverse());
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return (_first.Calculate(services) != 0 && _last.Calculate(services) != 0) ? 1 : 0;
//                     }
//                 }
//
//                 public class LogOr : ILogicalExpression
//                 {
//                     private readonly ILogicalExpression _first;
//                     private readonly ILogicalExpression _last;
//
//                     public LogOr(ILogicalExpression first, ILogicalExpression last)
//                     {
//                         _first = first;
//                         _last = last;
//                     }
//
//                     public override String ToString()
//                     {
//                         return $"Eval({_first} || {_last})";
//                     }
//
//                     public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                     {
//                         sw.Append("(");
//                         _first.Format(sw, formatterContext, services);
//                         sw.Append(" || ");
//                         _last.Format(sw, formatterContext, services);
//                         sw.Append(")");
//                     }
//
//                     public IJsmExpression Evaluate(IServices services)
//                     {
//                         ILogicalExpression first = _first.LogicalEvaluate(services);
//                         ILogicalExpression last = _last.LogicalEvaluate(services);
//
//                         if (first is IConstExpression n1)
//                         {
//                             if (n1.Value != 0)
//                             {
//                                 // (true || ??) => true
//                                 return ValueExpression.Create(0);
//                             }
//
//                             if (last is IConstExpression n2)
//                             {
//                                 if (n2.Value != 0)
//                                 {
//                                     // (false || true) => true
//                                     return ValueExpression.Create(0);
//                                 }
//                                 else
//                                 {
//                                     // (false || false) => false
//                                     return ValueExpression.Create(1);
//                                 }
//                             }
//                             else
//                             {
//                                 // (false || ??) => ??
//                                 return _last;
//                             }
//                         }
//                         else if (last is IConstExpression n2)
//                         {
//                             if (n2.Value != 0)
//                             {
//                                 // (?? || true) => true
//                                 return ValueExpression.Create(0);
//                             }
//
//                             // (?? || true) => ??
//                             return first;
//                         }
//
//                         // (?? || ??)
//                         return new LogOr(first, last);
//                     }
//
//                     public ILogicalExpression LogicalInverse()
//                     {
//                         return new LogAnd(_first.LogicalInverse(), _last.LogicalInverse());
//                     }
//
//                     public Int64 Calculate(IServices services)
//                     {
//                         return (_first.Calculate(services) != 0 || _last.Calculate(services) != 0) ? 1 : 0;
//                     }
//                 }
//             }
//         }
//     }
// }