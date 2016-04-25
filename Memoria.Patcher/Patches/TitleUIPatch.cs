using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Memoria.Patcher
{
    internal sealed class TitleUIPatch : TypePatch
    {
        private const String LogoSpriteFieldName = "LogoSprite";

        public override String SourceName => "TitleUI";
        public override String TargetName => "Memoria." + nameof(TitleUIInterceptor);

        protected override MethodPatch[] InitializeMethodPatches()
        {
            return new MethodPatch[] { new StartMethod() };
        }

        private sealed class StartMethod : MethodPatch
        {
            public override String SourceName => "Start";
            public override String TargetName => nameof(TitleUIInterceptor.Start);

            public override void Patch(MethodDefinition source)
            {
                FieldDefinition fieldText = source.DeclaringType.GetField(LogoSpriteFieldName);

                ILProcessor il = source.Body.GetILProcessor();
                Instruction target = il.GetFirstInstruction();
                il.InsertBefore(target, Instruction.Create(OpCodes.Ldarg_0));
                il.InsertBefore(target, Instruction.Create(OpCodes.Ldflda, fieldText));
                il.InsertBefore(target, il.CreateCallInstruction(TargetMethod));
            }
        }
    }
}