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
            return td.Methods.First(f => f.Name == methodName);
        }
    }
}