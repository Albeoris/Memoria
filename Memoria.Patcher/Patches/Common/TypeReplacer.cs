using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Memoria.Patcher
{
    public sealed class TypeReplacer
    {
        public readonly TypeReference NewReference;
        private readonly TypeDefinition _newDefination;
        private readonly List<ArrayType> _newArrayReferences;
        private readonly Dictionary<TypeReference, TypeDefinition> _arrayDefinitions;

        public TypeReplacer(TypeReference newReference)
        {
            NewReference = newReference;
            _newDefination = newReference.Resolve();
            _newArrayReferences = new List<ArrayType>();
            _arrayDefinitions = new Dictionary<TypeReference, TypeDefinition>();
        }

        public Instruction RecreateMethodOperand(ModuleDefinition module, Instruction inst, TypeReference newType)
        {
            MethodReference operandMethod = (MethodReference)inst.Operand;
            GenericInstanceMethod operandGeneric = operandMethod as GenericInstanceMethod;
            if (operandGeneric != null)
            {
                MethodDefinition method = ResolveType(newType).GetMethod(operandGeneric.ElementMethod);
                MethodReference methodReference = module.Import(method);

                GenericInstanceMethod methodInstance = new GenericInstanceMethod(methodReference);
                foreach (TypeReference genericArg in operandGeneric.GenericArguments)
                    methodInstance.GenericArguments.Add(module.Import(genericArg));

                return Instruction.Create(inst.OpCode, methodInstance);
            }
            else
            {
                MethodDefinition method = ResolveType(newType).GetMethod((MethodReference)inst.Operand);
                MethodReference methodReference = module.Import(method);
                return Instruction.Create(inst.OpCode, methodReference);
            }
        }

        public Instruction RecreateFieldOperand(ModuleDefinition module, Instruction inst, TypeReference newType)
        {
            String oldName = ((FieldReference)inst.Operand).Name;
            FieldDefinition field = ResolveType(newType).GetField(oldName);
            FieldReference fieldReference = module.Import(field);
            return Instruction.Create(inst.OpCode, fieldReference);
        }

        public Instruction RecreateTypeOperand(Instruction inst, TypeReference newType)
        {
            return Instruction.Create(inst.OpCode, newType);
        }

        private TypeDefinition ResolveType(TypeReference type)
        {
            if (type == NewReference)
                return _newDefination;

            TypeDefinition definition;
            if (!_arrayDefinitions.TryGetValue(type, out definition))
            {
                definition = type.Resolve();
                _arrayDefinitions.Add(type, definition);
            }

            return definition;
        }

        public ArrayType GetArrayType(int rank)
        {
            for (int i = _newArrayReferences.Count; i <= rank; i++)
                _newArrayReferences.Add(new ArrayType(NewReference, i + 1));

            return _newArrayReferences[rank];
        }
    }
}