using System;
using System.IO;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Memoria.Patcher
{
    public sealed class TypeHash
    {
        private Int32 _hash;
        private Int32[] _properties;
        private Int32[] _fields;
        private Int32[] _methods;
        private TypeHash[] _nestedTypes;

        private TypeHash()
        {
        }

        public TypeHash(TypeDefinition type)
        {
            _hash = type.FullName.GetHashCode();
            _hash = (_hash * 397) ^ type.Attributes.GetHashCode();

            CalcPropertiesHash(type);
            CalcFieldsHash(type);
            CalcMethodsHash(type);
            CalcProcessNestedTypesHash(type);
        }

        #region Compare

        public String Compare(TypeHash other)
        {
            StringBuilder sb = new StringBuilder();
            Compare(other, sb);
            return sb.ToString();
        }

        private void Compare(TypeHash other, StringBuilder sb)
        {
            if (ReferenceEquals(other, null))
            {
                sb.AppendLine("Null type occured.");
                return;
            }

            if (_hash != other._hash)
                sb.AppendLine("Base hash was changed.");

            CompareArray("Properties", _properties, other._properties, sb);
            CompareArray("Fields", _fields, other._fields, sb);
            CompareArray("Methods", _methods, other._methods, sb);

            CompareNestedTypes(_nestedTypes, other._nestedTypes, sb);
        }

        private void CompareArray(String prefix, Int32[] self, Int32[] other, StringBuilder sb)
        {
            if (ReferenceEquals(self, null))
            {
                if (!ReferenceEquals(other, null))
                    sb.AppendLine($"[{prefix}] Unexpected list occured.");
            }
            else if (ReferenceEquals(other, null))
            {
                sb.AppendLine($"[{prefix}] Expected list not present.");
            }
            else
            {
                if (self.Length != other.Length)
                {
                    sb.AppendLine($"[{prefix}] Different number of elements: " + (self.Length - other.Length));
                }
                else
                {
                    for (int i = 0; i < self.Length; i++)
                    {
                        if (self[i] != other[i])
                            sb.AppendLine($"[{prefix}] Different hash of element: " + i);
                    }
                }
            }
        }

        private void CompareNestedTypes(TypeHash[] self, TypeHash[] other, StringBuilder sb)
        {
            if (ReferenceEquals(self, null))
            {
                if (!ReferenceEquals(other, null))
                    sb.AppendLine("[Nested] Unexpected list occured.");
            }
            else if (ReferenceEquals(other, null))
            {
                sb.AppendLine("[Nested] Expected list not present.");
            }
            else
            {
                if (self.Length != other.Length)
                {
                    sb.AppendLine("[Nested] Different number of elements: " + (self.Length - other.Length));
                }
                else
                {
                    for (int i = 0; i < self.Length; i++)
                        self[i].Compare(other[i], sb);
                }
            }
        }

        #endregion

        #region Serialization

        public String ToBase64()
        {
            using (MemoryStream ms = new MemoryStream(4 + 4 * (_properties?.Length ?? 0) + 4 * (_fields?.Length ?? 0) + 4 * (_methods?.Length ?? 0)))
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                Serialize(bw);
                bw.Flush();

                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }

        public static TypeHash FromBase64(String base64String)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64String)))
            using (BinaryReader br = new BinaryReader(ms))
            {
                TypeHash hash = new TypeHash();
                hash.Deserialize(br);
                return hash;
            }
        }

        private void Serialize(BinaryWriter bw)
        {
            bw.Write(_hash);
            if (_properties == null)
                bw.Write(-1);
            else
            {
                bw.Write(_properties.Length);
                foreach (int hash in _properties)
                    bw.Write(hash);
            }

            if (_fields == null)
                bw.Write(-1);
            else
            {
                bw.Write(_fields.Length);
                foreach (int hash in _fields)
                    bw.Write(hash);
            }

            if (_methods == null)
                bw.Write(-1);
            else
            {
                bw.Write(_methods.Length);
                foreach (int hash in _methods)
                    bw.Write(hash);
            }

            if (_nestedTypes == null)
                bw.Write(-1);
            else
            {
                bw.Write(_nestedTypes.Length);
                foreach (TypeHash hash in _nestedTypes)
                    hash.Serialize(bw);
            }
        }

        private void Deserialize(BinaryReader br)
        {
            _hash = br.ReadInt32();

            Int32 propertiesLength = br.ReadInt32();
            if (propertiesLength > -1)
            {
                _properties = new Int32[propertiesLength];
                for (int i = 0; i < propertiesLength; i++)
                    _properties[i] = br.ReadInt32();
            }

            Int32 fieldsLength = br.ReadInt32();
            if (fieldsLength > -1)
            {
                _fields = new Int32[fieldsLength];
                for (int i = 0; i < fieldsLength; i++)
                    _fields[i] = br.ReadInt32();
            }

            Int32 methodsLength = br.ReadInt32();
            if (methodsLength > -1)
            {
                _methods = new Int32[methodsLength];
                for (int i = 0; i < methodsLength; i++)
                    _methods[i] = br.ReadInt32();
            }

            Int32 nestedLength = br.ReadInt32();
            if (nestedLength > -1)
            {
                _nestedTypes = new TypeHash[nestedLength];
                for (int i = 0; i < nestedLength; i++)
                {
                    TypeHash nested = new TypeHash();
                    nested.Deserialize(br);
                    _nestedTypes[i] = nested;
                }
            }
        }

        #endregion

        #region Calc

        private void CalcProcessNestedTypesHash(TypeDefinition type)
        {
            if (!type.HasNestedTypes)
                return;

            _nestedTypes = new TypeHash[type.NestedTypes.Count];
            for (int index = 0; index < type.NestedTypes.Count; index++)
            {
                TypeDefinition nested = type.NestedTypes[index];
                _nestedTypes[index] = new TypeHash(nested);
            }
        }

        private void CalcPropertiesHash(TypeDefinition type)
        {
            if (!type.HasProperties)
                return;

            _properties = new Int32[type.Properties.Count];
            for (int index = 0; index < type.Properties.Count; index++)
            {
                PropertyDefinition p = type.Properties[index];
                _properties[index] = CalcProcessPropertyHash(p);
            }
        }

        private static Int32 CalcProcessPropertyHash(PropertyDefinition propertyDefinition)
        {
            Int32 hash = propertyDefinition.Name.GetHashCode();
            hash = (hash * 397) ^ propertyDefinition.Attributes.GetHashCode();
            hash = (hash * 397) ^ propertyDefinition.PropertyType.FullName.GetHashCode();
            return hash;
        }

        private void CalcFieldsHash(TypeDefinition type)
        {
            if (!type.HasFields)
                return;

            _fields = new Int32[type.Fields.Count];
            for (int index = 0; index < type.Fields.Count; index++)
            {
                FieldDefinition f = type.Fields[index];
                _fields[index] = CalcFieldHash(f);
            }
        }

        private static int CalcFieldHash(FieldDefinition fieldDefinition)
        {
            Int32 hash = fieldDefinition.Name.GetHashCode();
            hash = (hash * 397) ^ fieldDefinition.Attributes.GetHashCode();
            hash = (hash * 397) ^ fieldDefinition.FieldType.FullName.GetHashCode();
            return hash;
        }

        private void CalcMethodsHash(TypeDefinition type)
        {
            if (!type.HasMethods)
                return;

            _methods = new Int32[type.Methods.Count];
            for (int index = 0; index < type.Methods.Count; index++)
            {
                MethodDefinition m = type.Methods[index];
                _methods[index] = CalcMethodHash(m);
            }
        }

        private static int CalcMethodHash(MethodDefinition m)
        {
            Int32 hash = m.Name.GetHashCode();
            hash = (hash * 397) ^ m.Attributes.GetHashCode();
            hash = (hash * 397) ^ m.ReturnType.FullName.GetHashCode();
            hash = (hash * 397) ^ CalcMethodParametersHash(m);
            hash = (hash * 397) ^ CalcMethodBodyHash(m);
            return hash;
        }

        private static int CalcMethodParametersHash(MethodDefinition m)
        {
            if (!m.HasParameters)
                return 0;

            Int32 hash = m.Parameters.Count;
            foreach (ParameterDefinition p in m.Parameters)
                hash = (hash * 397) ^ p.ParameterType.FullName.GetHashCode();

            return hash;
        }

        private static int CalcMethodBodyHash(MethodDefinition m)
        {
            if (!m.HasBody)
                return 0;

            Int32 hash = 17;
            hash = (hash * 397) ^ CalcMethodBodyVariablesHash(m);
            hash = (hash * 397) ^ CalcMethodBodyInstructionsHash(m);
            return hash;
        }

        private static int CalcMethodBodyVariablesHash(MethodDefinition m)
        {
            if (!m.Body.HasVariables)
                return 0;

            Int32 hash = m.Parameters.Count;
            foreach (VariableDefinition v in m.Body.Variables)
                hash = (hash * 397) ^ v.VariableType.FullName.GetHashCode();
            return hash;
        }

        private static int CalcMethodBodyInstructionsHash(MethodDefinition m)
        {
            MethodBody body = m.Body;
            Collection<Instruction> instructions = body.Instructions;
            Int32 hash = instructions.Count;
            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction inst = instructions[i];
                hash = (hash * 397) ^ inst.OpCode.GetHashCode();
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
                    hash = (hash * 397) ^ genericInstance.GenericArguments.Count;
                    for (int g = 0; g < genericInstance.GenericArguments.Count; g++)
                        hash = (hash * 397) ^ genericInstance.GenericArguments[g].FullName.GetHashCode();
                }

                MemberReference memberReference = inst.Operand as MemberReference;
                if (memberReference != null)
                    hash = (hash * 397) ^ memberReference.FullName.GetHashCode();
            }
            return hash;
        }

        #endregion
    }
}