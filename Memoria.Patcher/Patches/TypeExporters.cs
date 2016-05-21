using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Memoria.Patcher
{
    public sealed class TypeExporters
    {
        private readonly Dictionary<String, TypeExporter> _exporters = new Dictionary<String, TypeExporter>();
        private readonly Dictionary<String, TypeReplacer> _replacers = new Dictionary<String, TypeReplacer>();

        private readonly AssemblyDefinition _exportedAssembly;
        private readonly AssemblyDefinition _victimAssembly;

        private ModuleDefinition _module;
        private MethodDefinition _method;

        public TypeExporters(AssemblyDefinition exportedAssembly, AssemblyDefinition victimAssembly)
        {
            _exportedAssembly = exportedAssembly;
            _victimAssembly = victimAssembly;

            InitializeExportedTypes();
            InitializeVictimTypes();
            ValidateExporters();

            InitializeReplacers();
        }

        public void Export()
        {
            foreach (ModuleDefinition module in _victimAssembly.Modules)
                ProcessModule(module);

            Commit();
        }

        private void InitializeExportedTypes()
        {
            foreach (TypeDefinition type in _exportedAssembly.MainModule.Types)
            {
                if (!type.HasCustomAttributes)
                    continue;

                CustomAttribute attribute = type.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == "Memoria." + nameof(ExportedTypeAttribute));
                if (attribute == null)
                    continue;

                TypeHash hash = TypeHash.FromBase256((String)attribute.ConstructorArguments[0].Value);
                _exporters.Add(type.FullName, new TypeExporter(type, hash));
            }
        }

        private void InitializeVictimTypes()
        {
            foreach (TypeDefinition type in _victimAssembly.MainModule.Types)
            {
                TypeExporter exporter;
                if (!_exporters.TryGetValue(type.FullName, out exporter))
                    continue;

                exporter.VictimType = type;
                exporter.VictimHash = new TypeHash(type);
            }
        }

        private void ValidateExporters()
        {
            Boolean errorOccured = false;
            foreach (TypeExporter exporter in _exporters.Values)
            {
                String error = exporter.VictimHash.Compare(exporter.ExportedHash);
                if (String.IsNullOrEmpty(error))
                    continue;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Victim type was changed. Update the Game and Memoria to the last version.");
                sb.Append("Fullname: ").AppendLine(exporter.ExportedType.FullName);
                sb.Append("Current hash: ").Append(new String(exporter.VictimHash.ToBase256())).AppendLine();
                sb.Append("Error: ").AppendLine(error);
                String message = sb.ToString();

                Log.Warning(message);
                Console.Error.WriteLine(message);
                Console.Error.WriteLine();
                errorOccured = true;
            }

            if (errorOccured)
                throw new Exception("Failed to export types. See Memoria.log for details.");
        }

        private void InitializeReplacers()
        {
            foreach (TypeExporter exporter in _exporters.Values)
            {
                TypeDefinition oldType = exporter.VictimType;
                TypeDefinition newType = exporter.ExportedType;
                TypeReference newReference = oldType.Module.Import(newType);

                _replacers.Add(oldType.FullName, new TypeReplacer(newReference));

                if (oldType.HasNestedTypes)
                {
                    foreach (TypeDefinition oldNested in oldType.NestedTypes)
                    {
                        if (oldNested.IsNestedPrivate)
                            continue;

                        TypeDefinition newNested = newType.NestedTypes.First(t => t.Name == oldNested.Name);
                        newReference = oldNested.Module.Import(newNested);
                        _replacers.Add(oldNested.FullName, new TypeReplacer(newReference));
                    }
                }
            }
        }

        private void Commit()
        {
            ModuleDefinition victimModule = _victimAssembly.MainModule;
            ModuleDefinition exportedModule = _exportedAssembly.MainModule;
            AssemblyNameReference modReference = victimModule.AssemblyReferences.First(r => r.FullName == _exportedAssembly.MainModule.Assembly.Name.FullName);

            foreach (TypeExporter exporter in _exporters.Values)
            {
                TypeDefinition oldType = exporter.VictimType;

                ExportedType exportedType = new ExportedType(oldType.Namespace, oldType.Name, exportedModule, modReference);
                victimModule.Types.Remove(oldType);
                victimModule.ExportedTypes.Add(exportedType);
            }
        }

        private void ProcessModule(ModuleDefinition module)
        {
            _module = module;
            foreach (TypeDefinition type in _module.Types)
                ProcessType(type);
        }

        private void ProcessType(TypeDefinition type)
        {
            if (_replacers.ContainsKey(type.FullName))
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
                TypeReference newType;
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
                    TypeReplacer replacer;
                    if (TryGetNewType(typeReference, out replacer, out newType))
                        instructions.Replace(i, replacer.RecreateTypeOperand(inst, newType));
                    continue;
                }

                MethodReference methodReference = inst.Operand as MethodReference;
                if (methodReference != null)
                {
                    TypeReplacer replacer;
                    if (TryGetNewType(methodReference.DeclaringType, out replacer, out newType))
                        instructions.Replace(i, replacer.RecreateMethodOperand(_module, inst, newType));
                    continue;
                }

                FieldReference fieldReference = inst.Operand as FieldReference;
                if (fieldReference != null)
                {
                    TypeReplacer replacer;
                    if (TryGetNewType(fieldReference.DeclaringType, out replacer, out newType))
                        instructions.Replace(i, replacer.RecreateFieldOperand(_module, inst, newType));
                }
            }
        }

        private Boolean TryGetNewType(TypeReference oldType, out TypeReference newType)
        {
            TypeReplacer replacer;
            return TryGetNewType(oldType, out replacer, out newType);
        }

        private Boolean TryGetNewType(TypeReference oldType, out TypeReplacer replacer, out TypeReference newType)
        {
            Int32 rank = 0;
            while (oldType.IsArray)
            {
                rank++;
                oldType = oldType.GetElementType();
            }

            if (_replacers.TryGetValue(oldType.FullName, out replacer))
            {
                newType = rank > 0 ? replacer.GetArrayType(rank) : replacer.NewReference;
                return true;
            }

            GenericInstanceType genericType = oldType as GenericInstanceType;
            if (genericType != null)
            {
                for (int index = 0; index < genericType.GenericArguments.Count; index++)
                {
                    TypeReference argument = genericType.GenericArguments[index];
                    if (TryGetNewType(argument, out replacer, out newType))
                        genericType.GenericArguments[index] = newType;
                }
            }

            newType = null;
            return false;
        }

        public static void Export(AssemblyDefinition mod, AssemblyDefinition victim)
        {
            TypeExporters exporters = new TypeExporters(mod, victim);
            exporters.Export();
        }
    }
}