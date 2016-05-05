using System;
using System.Linq;
using Mono.Cecil;

namespace Memoria.Patcher
{
    public static class TypeDefinitionExm
    {
        public static FieldDefinition GetField(this TypeDefinition td, String fieldName)
        {
            return td.Fields.First(f => f.Name == fieldName);
        }

        public static MethodDefinition GetMethod(this TypeDefinition td, String methodName)
        {
            return td.Methods.Single(f => f.Name == methodName);
        }

        public static MethodDefinition GetMethod(this TypeDefinition td, MethodReference descriptor)
        {
            foreach (MethodDefinition method in td.Methods)
            {
                if (method.Name != descriptor.Name)
                    continue;

                if (method.ReturnType.IsGenericParameter)
                {
                    if (!descriptor.ReturnType.IsGenericParameter)
                        continue;
                }
                else if (descriptor.ReturnType.IsGenericParameter)
                {
                    continue;
                }
                else
                {
                    if (method.ReturnType.IsArray)
                    {
                        if (!descriptor.ReturnType.IsArray)
                            continue;
                    }
                    else if (descriptor.ReturnType.IsArray)
                    {
                        continue;
                    }
                    else
                    {
                        if (method.ReturnType.FullName != descriptor.ReturnType.FullName)
                            continue;
                    }
                }

                if (method.HasGenericParameters)
                {
                    if (!descriptor.HasGenericParameters)
                        continue;

                    if (method.GenericParameters.Count != descriptor.GenericParameters.Count)
                        continue;
                }
                else if (descriptor.HasGenericParameters)
                {
                    continue;
                }

                if (method.HasParameters)
                {
                    if (!descriptor.HasParameters)
                        continue;

                    if (method.Parameters.Count != descriptor.Parameters.Count)
                        continue;

                    for (int i = 0; i < method.Parameters.Count; i++)
                    {
                        ParameterDefinition p1 = method.Parameters[i];
                        ParameterDefinition p2 = descriptor.Parameters[i];
                        if (p1.ParameterType.FullName != p2.ParameterType.FullName)
                            continue;

                        if (p1.Attributes != p2.Attributes)
                            continue;
                    }
                }
                else if (descriptor.HasParameters)
                {
                    continue;
                }

                return method;
            }

            throw new InvalidOperationException("Sequence contains more than one matching element.");
        }
    }
}