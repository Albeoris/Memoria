using System;
using Mono.Cecil;

namespace Memoria.Patcher
{
    public abstract class TypePatch : IPatch
    {
        public abstract String SourceName { get; }
        public virtual String TargetName { get; }
        public TypeDefinition TargetType { get; set; }
        protected abstract MethodPatch[] InitializeMethodPatches();

        public PatchCollection<MethodPatch> GetMethodPatches()
        {
            MethodPatch[] result = InitializeMethodPatches();
            if (TargetType != null)
            {
                foreach (MethodPatch patch in result)
                    SetTargetMethod(patch);
            }
            else if (TargetName != null)
            {
                throw Exceptions.CreateException("Target type wasn't found {0}", TargetName);
            }
            return new PatchCollection<MethodPatch>(result);
        }

        private void SetTargetMethod(MethodPatch patch)
        {
            if (patch.TargetName != null)
                patch.TargetMethod = TargetType.GetMethod(patch.TargetName);
        }
    }
}