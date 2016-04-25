using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Memoria.Patcher
{
    public static class ILProcessorExm
    {
        public static void ClearInstructions(this ILProcessor il)
        {
            il.Body.Instructions.Clear();
        }

        public static void ClearVariables(this ILProcessor il)
        {
            il.Body.Variables.Clear();
        }

        public static Instruction GetFirstInstruction(this ILProcessor il)
        {
            return il.Body.Instructions.First();
        }

        public static Instruction GetStsfldInstruction(this ILProcessor il, String staticFieldName)
        {
            return il.Body.Instructions.First(i => i.OpCode.Code == Code.Stsfld && ((FieldReference)i.Operand).Name == staticFieldName);
        }

        public static Instruction CreateCallInstruction(this ILProcessor il, MethodDefinition targetMethod)
        {
            MethodReference reference = il.Body.Method.Module.Import(targetMethod);
            return Instruction.Create(OpCodes.Call, reference);
        }
    }
}

