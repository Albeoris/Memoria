using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Memoria.Patcher
{
    internal sealed class FF9TextToolPatch : TypePatch
    {
        //private const String FieldTextFieldName = "fieldText";
        //private const String FieldZoneFieldName = "fieldZoneId";

        public override String SourceName => "Assets.Sources.Scripts.UI.Common.FF9TextTool";
        public override String TargetName => "Memoria." + nameof(FF9TextToolInterceptor);

        protected override MethodPatch[] InitializeMethodPatches()
        {
            return new MethodPatch[]
            {
                new RedirectMethodPatch("InitializeFieldText", nameof(FF9TextToolInterceptor.InitializeFieldText), "System.Collections.IEnumerator"),
                new RedirectMethodPatch("InitializeItemText", nameof(FF9TextToolInterceptor.InitializeItemText), "System.Collections.IEnumerator"),
                new RedirectMethodPatch("InitializeImportantItemText", nameof(FF9TextToolInterceptor.InitializeImportantItemText), "System.Collections.IEnumerator"),
                new RedirectMethodPatch("InitializeAbilityText", nameof(FF9TextToolInterceptor.InitializeAbilityText), "System.Collections.IEnumerator"),
                new RedirectMethodPatch("InitializeCommandText", nameof(FF9TextToolInterceptor.InitializeCommandText), "System.Collections.IEnumerator"),
                new RedirectMethodPatch("InitializeBattleText", nameof(FF9TextToolInterceptor.InitializeBattleText), "System.Collections.IEnumerator"),
                new RedirectMethodPatch("InitializeLocationText", nameof(FF9TextToolInterceptor.InitializeLocationText), "System.Collections.IEnumerator"),
                new RedirectMethodPatch("InitializeEtcText", nameof(FF9TextToolInterceptor.InitializeEtcText), "System.Collections.IEnumerator"),
            };
        }

        private sealed class RedirectMethodPatch : MethodPatch
        {
            public override String SourceName { get; }
            public override String TargetName { get; }
            public override String ExpectedReturnType { get; }
            public override String[] ExpectedParameterTypes { get; }

            public RedirectMethodPatch(String sourceMethodName, String targetMethodName, String expectedReturnType, params String[] expectedParameterTypes)
            {
                SourceName = sourceMethodName;
                TargetName = targetMethodName;
                ExpectedReturnType = expectedReturnType;
                ExpectedParameterTypes = expectedParameterTypes;
            }

            public override void Patch(MethodDefinition source)
            {
                ILProcessor il = source.Body.GetILProcessor();
                il.ClearInstructions();
                il.ClearVariables();
                il.Append(il.CreateCallInstruction(TargetMethod));
                il.Emit(OpCodes.Ret);
            }
        }

        //private sealed class InitializeFieldTextMethod : MethodPatch
        //{
        //    public override String SourceMethodName => "InitializeFieldText";
        //    public override String ExpectedReturnType => "System.Collections.IEnumerator";
        //    public override String TargetMethodName => nameof(FF9TextToolInterceptor.InitializeFieldText);

        //    public override void Patch(MethodDefinition source)
        //    {
        //        ILProcessor il = source.Body.GetILProcessor();
        //        il.ClearInstructions();
        //        il.ClearVariables();
        //        il.Append(il.CreateCallInstruction(TargetMethod));
        //        il.Emit(OpCodes.Ret);
        //    }
        //}

        //private sealed class InitializeItemTextMethod : MethodPatch
        //{
        //    public String Label => SourceMethodName + "_PATCHED";
        //    public String SourceMethodName => "InitializeItemText";
        //    public String ExpectedReturnType => "System.Collections.IEnumerator";
        //    public String[] ExpectedParameterTypes => null;

        //    public void Patch(MethodDefinition source)
        //    {
        //        ILProcessor il = source.Body.GetILProcessor();
        //        il.ClearInstructions();
        //        il.Append(il.CreateCallInstruction(typeof(FF9TextToolInterceptor), nameof(FF9TextToolInterceptor.InitializeItemText)));
        //        il.Emit(OpCodes.Ret);
        //    }
        //}

        //private sealed class InitializeImportantItemTextMethod : MethodPatch
        //{
        //    public String Label => SourceMethodName + "_PATCHED";
        //    public String SourceMethodName => "InitializeImportantItemText";
        //    public String ExpectedReturnType => "System.Collections.IEnumerator";
        //    public String[] ExpectedParameterTypes => null;

        //    public void Patch(MethodDefinition source)
        //    {
        //        ILProcessor il = source.Body.GetILProcessor();
        //        il.ClearInstructions();
        //        il.Append(il.CreateCallInstruction(typeof(FF9TextToolInterceptor), nameof(FF9TextToolInterceptor.InitializeImportantItemText)));
        //        il.Emit(OpCodes.Ret);
        //    }
        //}

        //private sealed class InitializeAbilityTextMethod : MethodPatch
        //{
        //    public String Label => SourceMethodName + "_PATCHED";
        //    public String SourceMethodName => "InitializeAbilityText";
        //    public String ExpectedReturnType => "System.Collections.IEnumerator";
        //    public String[] ExpectedParameterTypes => null;

        //    public void Patch(MethodDefinition source)
        //    {
        //        ILProcessor il = source.Body.GetILProcessor();
        //        il.ClearInstructions();
        //        il.Append(il.CreateCallInstruction(typeof(FF9TextToolInterceptor), nameof(FF9TextToolInterceptor.InitializeAbilityText)));
        //        il.Emit(OpCodes.Ret);
        //    }
        //}

        //private sealed class InitializeCommandTextMethod : MethodPatch
        //{
        //    public String Label => SourceMethodName + "_PATCHED";
        //    public String SourceMethodName => "InitializeCommandText";
        //    public String ExpectedReturnType => "System.Collections.IEnumerator";
        //    public String[] ExpectedParameterTypes => null;

        //    public void Patch(MethodDefinition source)
        //    {
        //        ILProcessor il = source.Body.GetILProcessor();
        //        il.ClearInstructions();
        //        il.Append(il.CreateCallInstruction(typeof(FF9TextToolInterceptor), nameof(FF9TextToolInterceptor.InitializeCommandText)));
        //        il.Emit(OpCodes.Ret);
        //    }
        //}

        //private sealed class InitializeBattleTextMethod : MethodPatch
        //{
        //    public String Label => SourceMethodName + "_PATCHED";
        //    public String SourceMethodName => "InitializeBattleText";
        //    public String ExpectedReturnType => "System.Collections.IEnumerator";
        //    public String[] ExpectedParameterTypes => null;

        //    public void Patch(MethodDefinition source)
        //    {
        //        ILProcessor il = source.Body.GetILProcessor();
        //        il.ClearInstructions();
        //        il.Append(il.CreateCallInstruction(typeof(FF9TextToolInterceptor), nameof(FF9TextToolInterceptor.InitializeBattleText)));
        //        il.Emit(OpCodes.Ret);
        //    }
        //}

        //private sealed class InitializeLocationTextMethod : MethodPatch
        //{
        //    public String Label => SourceMethodName + "_PATCHED";
        //    public String SourceMethodName => "InitializeLocationText";
        //    public String ExpectedReturnType => "System.Collections.IEnumerator";
        //    public String[] ExpectedParameterTypes => null;

        //    public void Patch(MethodDefinition source)
        //    {
        //        ILProcessor il = source.Body.GetILProcessor();
        //        il.ClearInstructions();
        //        il.Append(il.CreateCallInstruction(typeof(FF9TextToolInterceptor), nameof(FF9TextToolInterceptor.InitializeLocationText)));
        //        il.Emit(OpCodes.Ret);
        //    }
        //}

        //private sealed class InitializeEtcTextMethod : MethodPatch
        //{
        //    public String Label => SourceMethodName + "_PATCHED";
        //    public String SourceMethodName => "InitializeEtcText";
        //    public String ExpectedReturnType => "System.Collections.IEnumerator";
        //    public String[] ExpectedParameterTypes => null;

        //    public void Patch(MethodDefinition source)
        //    {
        //        ILProcessor il = source.Body.GetILProcessor();
        //        il.ClearInstructions();
        //        il.Append(il.CreateCallInstruction(typeof(FF9TextToolInterceptor), nameof(FF9TextToolInterceptor.InitializeEtcText)));
        //        il.Emit(OpCodes.Ret);
        //    }
        //}

        //private sealed class FieldTextMethod : MethodPatch
        //{
        //    public String Label => SourceMethodName + "_PATCHED1";
        //    public String SourceMethodName => "FieldText";
        //    public String ExpectedReturnType => "System.String";
        //    public String[] ExpectedParameterTypes => new[] {"System.Int32"};

        //    public void Patch(MethodDefinition source)
        //    {
        //        FieldDefinition fieldText = source.DeclaringType.GetField(FieldTextFieldName);
        //        FieldDefinition fieldZone = source.DeclaringType.GetField(FieldZoneFieldName);

        //        ILProcessor il = source.Body.GetILProcessor();
        //        il.ClearInstructions();
        //        il.Emit(OpCodes.Ldsflda, fieldText);
        //        il.Emit(OpCodes.Ldsfld, fieldZone);
        //        il.Emit(OpCodes.Ldarg_0);
        //        il.Append(il.CreateCallInstruction(typeof(FF9TextToolInterceptor), nameof(FF9TextToolInterceptor.FieldText)));
        //        il.Emit(OpCodes.Ret);
        //    }
        //}
    }
}