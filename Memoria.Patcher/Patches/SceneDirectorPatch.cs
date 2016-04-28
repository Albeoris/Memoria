using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Memoria.Patcher
{
    internal sealed class SceneDirectorPatch : TypePatch
    {
        public override String SourceName => "Assets.Scripts.Common.SceneDirector";
        public override String TargetName => "Memoria." + nameof(SceneDirectorInterceptor);

        protected override MethodPatch[] InitializeMethodPatches()
        {
            return new MethodPatch[] { new ReplaceNowMethod() };
        }

        private sealed class ReplaceNowMethod : MethodPatch
        {
            public override String SourceName => "ReplaceNow";
            public override String TargetName => nameof(SceneDirectorInterceptor.ReplaceNow);
            public override String[] ExpectedParameterTypes => new[] {"System.String"};

            public override void Patch(MethodDefinition source)
            {
                ILProcessor il = source.Body.GetILProcessor();
                Instruction target = il.GetFirstInstruction();
                il.InsertBefore(target, Instruction.Create(OpCodes.Ldarga, source.Parameters[0]));
                il.InsertBefore(target, il.CreateCallInstruction(TargetMethod));
            }
        }
    }
}