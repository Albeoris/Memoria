using System;
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

        private ModuleDefinition _module;
        private MethodDefinition _method;

        public TypeReplacer(String fullName, TypeReference newReference)
        {
            _fullName = fullName;
            _newReference = newReference;
            _newDefination = newReference.Resolve();
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
                if (p.PropertyType.FullName == _fullName)
                    p.PropertyType = _newReference;
            }
        }

        private void ProcessFields(TypeDefinition type)
        {
            foreach (FieldDefinition f in type.Fields)
            {
                if (f.FieldType.FullName == _fullName)
                    f.FieldType = _newReference;
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
            if (_method.ReturnType.FullName == _fullName)
                _method.ReturnType = _newReference;

            ProcessMethodParameters();
            ProcessMethodBody();
        }

        private void ProcessMethodParameters()
        {
            if (!_method.HasParameters)
                return;

            foreach (ParameterDefinition p in _method.Parameters)
            {
                if (p.ParameterType.FullName == _fullName)
                    p.ParameterType = _newReference;
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
                if (v.VariableType.FullName == _fullName)
                    v.VariableType = _newReference;
            }
        }

        private void ProcessMethodBodyInstructions()
        {
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
                        if (genericInstance.GenericArguments[g].FullName == _fullName)
                            genericInstance.GenericArguments[g] = _newReference;
                    }
                }

                TypeReference typeReference = inst.Operand as TypeReference;
                if (typeReference != null)
                {
                    if (typeReference.FullName == _fullName)
                        instructions.Replace(i, RecreateTypeOperand(inst));
                    continue;
                }

                MethodReference methodReference = inst.Operand as MethodReference;
                if (methodReference != null)
                {
                    if (methodReference.DeclaringType.FullName == _fullName)
                        instructions.Replace(i, RecreateMethodOperand(inst));
                    continue;
                }

                FieldReference fieldReference = inst.Operand as FieldReference;
                if (fieldReference != null)
                {
                    if (fieldReference.DeclaringType.FullName == _fullName)
                        instructions.Replace(i, RecreateFieldOperand(inst));
                    continue;
                }

            }
        }

        private Instruction RecreateMethodOperand(Instruction inst)
        {
            MethodDefinition method = _newDefination.GetMethod((MethodReference)inst.Operand);
            MethodReference methodReference = _module.Import(method);
            return Instruction.Create(inst.OpCode, methodReference);
        }

        private Instruction RecreateFieldOperand(Instruction inst)
        {
            String oldName = ((FieldReference)inst.Operand).Name;
            FieldDefinition field = _newDefination.GetField(oldName);
            FieldReference fieldReference = _module.Import(field);
            return Instruction.Create(inst.OpCode, fieldReference);
        }

        private Instruction RecreateTypeOperand(Instruction inst)
        {
            return Instruction.Create(inst.OpCode, _newReference);
        }
    }
}