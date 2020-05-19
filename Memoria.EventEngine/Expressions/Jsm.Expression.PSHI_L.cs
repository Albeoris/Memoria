// using System;
// using FF8.Core;
// using FF8.JSM.Format;
//
// namespace FF8.JSM
// {
//     public static partial class Jsm
//     {
//         public static partial class Expression
//         {
//             public sealed class PSHI_L : IJsmExpression
//             {
//                 private ScriptResultId _index;
//
//                 /// <summary>
//                 /// Push the value (Int32) at index position in the Temp List onto the stack. 
//                 /// </summary>
//                 /// <param name="index">Index position in the Temp List (0...7).</param>
//                 public PSHI_L(ScriptResultId index)
//                 {
//                     _index = index;
//                 }
//
//                 public override String ToString()
//                 {
//                     return $"{nameof(PSHI_L)}({nameof(_index)}: {_index})";
//                 }
//
//                 public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
//                 {
//                     sw.Append($"R{_index.ResultId}");
//                 }
//
//                 public IJsmExpression Evaluate(IServices services)
//                 {
//                     IInteractionService interaction = ServiceId.Interaction[services];
//                     if (interaction.IsSupported)
//                     {
//                         Int32 value = interaction[_index];
//                         return ValueExpression.Create(value);
//                     }
//
//                     return this;
//                 }
//
//                 public Int64 Calculate(IServices services)
//                 {
//                     return ServiceId.Interaction[services][_index];
//                 }
//             }
//         }
//     }
// }