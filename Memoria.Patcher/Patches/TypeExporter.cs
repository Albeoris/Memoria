using Mono.Cecil;

namespace Memoria.Patcher
{
    public sealed class TypeExporter
    {
        public TypeDefinition ExportedType { get; }
        public TypeHash ExportedHash { get; }
        public TypeDefinition VictimType { get; set; }
        public TypeHash VictimHash { get; set; }

        public TypeExporter(TypeDefinition exportedType, TypeHash exportedHash)
        {
            ExportedType = exportedType;
            ExportedHash = exportedHash;
        }
    }
}