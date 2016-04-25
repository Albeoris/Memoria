using System;
using Mono.Cecil;

namespace Memoria.Patcher
{
    public abstract class MethodPatch : IPatch
    {
        public abstract String SourceName { get; }
        public virtual String Label => SourceName + "_PATCHED";
        public virtual String TargetName { get; }
        public virtual String ExpectedReturnType => "System.Void";
        public virtual String[] ExpectedParameterTypes { get; }
        public MethodDefinition TargetMethod { get; set; }

        public abstract void Patch(MethodDefinition source);
    }
}