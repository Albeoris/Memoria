using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Memoria.Patcher
{
    public sealed class TypeReplacer
    {
        private readonly String _fullName;
        private readonly TypeReference _newReference;
        private readonly TypeDefinition _newDefination;
        private readonly List<ArrayType> _newArrayReferences;
        private readonly Dictionary<TypeReference, TypeDefinition> _arrayDefinitions;

        private ModuleDefinition _module;
        private MethodDefinition _method;

        public TypeReplacer(String fullName, TypeReference newReference)
        {
            _fullName = fullName;
            _newReference = newReference;
            _newDefination = newReference.Resolve();
            _newArrayReferences = new List<ArrayType>();
            _arrayDefinitions = new Dictionary<TypeReference, TypeDefinition>();
        }

        public void Replace(ModuleDefinition module)
        {
            _module = module;
            foreach (TypeDefinition type in _module.Types)
                ProcessType(type);
        }

        private void ProcessType(TypeDefinition type)
        {
            if (type.FullName == _fullName)
                return;

            ProcessProperties(type);
            ProcessFields(type);
            ProcessMethods(type);
            ProcessNestedTypes(type);
        }

        private void ProcessNestedTypes(TypeDefinition type)
        {
            if (!type.HasNestedTypes)
                return;

            foreach (TypeDefinition nested in type.NestedTypes)
                ProcessType(nested);
        }

        private void ProcessProperties(TypeDefinition type)
        {
            foreach (PropertyDefinition p in type.Properties)
            {
                TypeReference newType;
                if (TryGetNewType(p.PropertyType, out newType))
                    p.PropertyType = newType;
            }
        }

        private void ProcessFields(TypeDefinition type)
        {
            foreach (FieldDefinition f in type.Fields)
            {
                TypeReference newType;
                if (TryGetNewType(f.FieldType, out newType))
                    f.FieldType = newType;
            }
        }

        private void ProcessMethods(TypeDefinition type)
        {
            foreach (MethodDefinition m in type.Methods)
            {
                _method = m;
                ProcessMethod();
            }
        }

        private void ProcessMethod()
        {
            TypeReference newType;
            if (TryGetNewType(_method.ReturnType, out newType))
                _method.ReturnType = newType;

            ProcessMethodParameters();
            ProcessMethodBody();
        }

        private void ProcessMethodParameters()
        {
            if (!_method.HasParameters)
                return;

            foreach (ParameterDefinition p in _method.Parameters)
            {
                TypeReference newType;
                if (TryGetNewType(p.ParameterType, out newType))
                    p.ParameterType = newType;
            }
        }

        private void ProcessMethodBody()
        {
            if (!_method.HasBody)
                return;

            ProcessMethodBodyVariables();
            ProcessMethodBodyInstructions();
        }

        private void ProcessMethodBodyVariables()
        {
            if (!_method.Body.HasVariables)
                return;

            foreach (VariableDefinition v in _method.Body.Variables)
            {
                TypeReference newType;
                if (TryGetNewType(v.VariableType, out newType))
                    v.VariableType = newType;
            }
        }

        private void ProcessMethodBodyInstructions()
        {
            TypeReference newType;
            MethodBody body = _method.Body;
            Collection<Instruction> instructions = body.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction inst = instructions[i];
                switch (inst.OpCode.OperandType)
                {
                    case OperandType.InlineField:
                    case OperandType.InlineMethod:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                        break;
                    default:
                        continue;
                }

                IGenericInstance genericInstance = inst.Operand as IGenericInstance;
                if (genericInstance != null && genericInstance.HasGenericArguments)
                {
                    for (int g = 0; g < genericInstance.GenericArguments.Count; g++)
                    {
                        if (TryGetNewType(genericInstance.GenericArguments[g], out newType))
                            genericInstance.GenericArguments[g] = newType;
                    }
                }

                TypeReference typeReference = inst.Operand as TypeReference;
                if (typeReference != null)
                {
                    if (TryGetNewType(typeReference, out newType))
                        instructions.Replace(i, RecreateTypeOperand(inst, newType));
                    continue;
                }

                MethodReference methodReference = inst.Operand as MethodReference;
                if (methodReference != null)
                {
                    if (TryGetNewType(methodReference.DeclaringType, out newType))
                        instructions.Replace(i, RecreateMethodOperand(inst, newType));
                    continue;
                }

                FieldReference fieldReference = inst.Operand as FieldReference;
                if (fieldReference != null)
                {
                    if (TryGetNewType(fieldReference.DeclaringType, out newType))
                        instructions.Replace(i, RecreateFieldOperand(inst, newType));

                    continue;
                }

            }
        }

        private Instruction RecreateMethodOperand(Instruction inst, TypeReference newType)
        {
            MethodDefinition method = ResolveType(newType).GetMethod((MethodReference)inst.Operand);
            MethodReference methodReference = _module.Import(method);
            return Instruction.Create(inst.OpCode, methodReference);
        }

        private Instruction RecreateFieldOperand(Instruction inst, TypeReference newType)
        {
            String oldName = ((FieldReference)inst.Operand).Name;
            FieldDefinition field = ResolveType(newType).GetField(oldName);
            FieldReference fieldReference = _module.Import(field);
            return Instruction.Create(inst.OpCode, fieldReference);
        }

        private Instruction RecreateTypeOperand(Instruction inst, TypeReference newType)
        {
            return Instruction.Create(inst.OpCode, newType);
        }

        private Boolean TryGetNewType(TypeReference oldType, out TypeReference newType)
        {
            Int32 rank = 0;
            while (oldType.IsArray)
            {
                rank++;
                oldType = oldType.GetElementType();
            }

            if (oldType.FullName == _fullName)
            {
                newType = rank > 0 ? GetArrayType(rank) : _newReference;
                return true;
            }

            newType = null;
            return false;
        }

        private TypeDefinition ResolveType(TypeReference type)
        {
            if (type == _newReference)
                return _newDefination;

            TypeDefinition definition;
            if (!_arrayDefinitions.TryGetValue(type, out definition))
            {
                definition = type.Resolve();
                _arrayDefinitions.Add(type, definition);
            }
            return definition;
        }

        private ArrayType GetArrayType(int rank)
        {
            for (int i = _newArrayReferences.Count; i <= rank; i++)
                _newArrayReferences.Add(new ArrayType(_newReference, i + 1));

            return _newArrayReferences[rank];
        }
    }
}