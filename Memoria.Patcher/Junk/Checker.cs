using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Memoria.Patcher
{
    public static class JunkChecker
    {
        public static void Check(AssemblyDefinition victim, String backupPath)
        {
            AssemblyDefinition backup = AssemblyDefinition.ReadAssembly(backupPath);
            var m1 = backup.MainModule.GetType("ConfigUI").GetMethod("OnKeyConfirm");

            var m2 = victim.MainModule.GetType("ConfigUI").GetMethod("OnKeyConfirm");
            for (int i = 0; i < Math.Max(m1.Body.Instructions.Count, m2.Body.Instructions.Count); i++)
            {
                var i1 = m1.Body.Instructions[i];
                var i2 = m2.Body.Instructions[i];
                if (GetPopDelta(i1, m1) != GetPopDelta(i2, m2))
                    throw new InvalidOperationException();
            }
        }

        public static int GetPushDelta(Instruction instruction)
        {
            OpCode opCode = instruction.OpCode;
            switch (opCode.StackBehaviourPush)
            {
                case StackBehaviour.Push0:
                    return 0;
                case StackBehaviour.Push1:
                case StackBehaviour.Pushi:
                case StackBehaviour.Pushi8:
                case StackBehaviour.Pushr4:
                case StackBehaviour.Pushr8:
                case StackBehaviour.Pushref:
                    return 1;
                case StackBehaviour.Push1_push1:
                    return 2;
                case StackBehaviour.Varpush:
                    if (opCode.FlowControl == FlowControl.Call)
                        return ((IMethodSignature)instruction.Operand).ReturnType.Name == "Void" ? 0 : 1;
                    break;
            }
            throw new NotSupportedException();
        }

        public static int? GetPopDelta(Instruction instruction, MethodDefinition methodDef)
        {
            OpCode opCode = instruction.OpCode;
            switch (opCode.StackBehaviourPop)
            {
                case StackBehaviour.Pop0:
                    return new int?(0);
                case StackBehaviour.Pop1:
                case StackBehaviour.Popi:
                case StackBehaviour.Popref:
                    return new int?(1);
                case StackBehaviour.Pop1_pop1:
                case StackBehaviour.Popi_pop1:
                case StackBehaviour.Popi_popi:
                case StackBehaviour.Popi_popi8:
                case StackBehaviour.Popi_popr4:
                case StackBehaviour.Popi_popr8:
                case StackBehaviour.Popref_pop1:
                case StackBehaviour.Popref_popi:
                    return new int?(2);
                case StackBehaviour.Popi_popi_popi:
                case StackBehaviour.Popref_popi_popi:
                case StackBehaviour.Popref_popi_popi8:
                case StackBehaviour.Popref_popi_popr4:
                case StackBehaviour.Popref_popi_popr8:
                case StackBehaviour.Popref_popi_popref:
                    return new int?(3);
                case StackBehaviour.PopAll:
                    return new int?();
                case StackBehaviour.Varpop:
                    if (opCode == OpCodes.Ret)
                        return new int?(methodDef.ReturnType.Name == "Void" ? 0 : 1);
                    if (opCode.FlowControl == FlowControl.Call)
                    {
                        IMethodSignature methodSignature = (IMethodSignature)instruction.Operand;
                        int num = methodSignature.HasParameters ? methodSignature.Parameters.Count : 0;
                        if (methodSignature.HasThis && opCode != OpCodes.Newobj)
                            ++num;
                        if (opCode == OpCodes.Calli)
                            ++num;
                        return new int?(num);
                    }
                    break;
            }
            throw new NotSupportedException();
        }
    }
}