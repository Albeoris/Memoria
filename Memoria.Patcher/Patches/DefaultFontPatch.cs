using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Memoria.Patcher
{
    internal sealed class DefaultFontPatch : TypePatch
    {
        private const String FontFieldName = "defaultFont";

        public override String SourceName => "EncryptFontManager";
        public override String TargetName => "Memoria." + nameof(FontInterceptor);

        protected override MethodPatch[] InitializeMethodPatches()
        {
            return new MethodPatch[] {new LoadFontMethod()};
        }

        private sealed class LoadFontMethod : MethodPatch
        {
            public override String SourceName => "LoadFont";
            public override String TargetName => nameof(FontInterceptor.LoadFont);

            public override void Patch(MethodDefinition source)
            {
                ILProcessor il = source.Body.GetILProcessor();
                Instruction oldSet = il.GetStsfldInstruction(FontFieldName);
                Instruction newCall = il.CreateCallInstruction(TargetMethod);
                il.InsertBefore(oldSet, newCall);
            }
        }
    }
}