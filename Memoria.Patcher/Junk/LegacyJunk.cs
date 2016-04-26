//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Memoria.Patcher
//{
//    class LegacyJunk
//    {

//public static MethodInfo GetChangeEBGMethodInfo()
//{
//    return typeof(TextSource).GetMethod(nameof(ChangeEBG));
//}
//
//public static Texture2D ChangeEBG(String originalPath, Texture2D originalSource)
//{
//    //Log.Message("ChangeEBG" + Environment.StackTrace);
//    //Log.Message("{0} {1}", originalPath, originalSource);
//    return originalSource;
//}
//        //private static void PatchBGSCENE_DEF(TypeDefinition type)
//        //{
//        //    foreach (MethodDefinition method in type.Methods)
//        //    {
//        //        switch (method.Name)
//        //        {
//        //            case "LoadEBG":
//        //                PatchLoadEBG(method);
//        //                break;
//        //        }
//        //    }
//        //}

//        //private static void PatchLoadEBG(MethodDefinition method)
//        //{
//        //    if (!ExpectParameters(method, 3))
//        //        return;
//        //    if (method.Body.Instructions[0].OpCode == OpCodes.Nop)
//        //        return;

//        //    Instruction firstInstruction = method.Body.Instructions.First();
//        //    Instruction formatInstruction = method.Body.Instructions.First(i => i.OpCode.Code == Code.Call && ((MethodReference)i.Operand).Name == "Combine");
//        //    Instruction callInstruction = method.Body.Instructions.First(i => i.OpCode.Code == Code.Call && ((MethodReference)i.Operand).FullName == "!!0 AssetManager::Load<UnityEngine.Texture2D>(System.String,System.Boolean)");

//        //    ILProcessor il = method.Body.GetILProcessor();

//        //    Instruction nop = Instruction.Create(OpCodes.Nop);
//        //    Instruction dup = Instruction.Create(OpCodes.Dup);
//        //    Instruction call = Instruction.Create(OpCodes.Call, method.Module.Import(TextSource.GetChangeFontMethodInfo()));

//        //    il.InsertBefore(firstInstruction, nop);
//        //    il.InsertAfter(formatInstruction, dup);
//        //    il.InsertAfter(callInstruction, call);

//        //    Changed = true;
//        //}

//        //private static void PatchQuadMistResourceManager(TypeDefinition type)
//        //{
//        //    foreach (MethodDefinition method in type.Methods)
//        //    {
//        //        switch (method.Name)
//        //        {
//        //            case "LoadSprite":
//        //                PatchLoadSprite(method);
//        //                break;
//        //        }
//        //    }
//        //}

//        //private static void PatchLoadSprite(MethodDefinition method)
//        //{
//        //    if (!ExpectParameters(method, 0))
//        //        return;
//        //    if (method.Body.Instructions[0].OpCode == OpCodes.Nop)
//        //        return;

//        //    Instruction firstInstruction = method.Body.Instructions.First();
//        //    Instruction callInstruction = method.Body.Instructions.First(i => i.OpCode.Code == Code.Call);

//        //    ILProcessor il = method.Body.GetILProcessor();

//        //    Instruction nop = Instruction.Create(OpCodes.Nop);
//        //    Instruction ldloc0 = Instruction.Create(OpCodes.Ldloc_0);
//        //    Instruction call = Instruction.Create(OpCodes.Call, method.Module.Import(TextSource.GetChangeSpritesMethodInfo()));

//        //    il.InsertBefore(firstInstruction, nop);
//        //    il.InsertAfter(callInstruction, ldloc0);
//        //    il.InsertAfter(ldloc0, call);

//        //    Changed = true;
//        //}
//    }
//}



// ==================/

//String p = Path.Combine(directory, "UnityEngine.dll");
//AssemblyDefinition unity = AssemblyDefinition.ReadAssembly(p);
//TypeDefinition type = unity.MainModule.Types.First(t => t.FullName == "UnityEngine.Resources");
//MethodDefinition method = type.Methods.First(m => m.Name == "Load" && m.IsInternalCall);
//MethodDefinition loadInternal = new MethodDefinition("LoadInternal", method.Attributes, method.ReturnType) { DeclaringType = type };
//PInvokeInfo attribure = mod.MainModule.Types.First(t => t.Name == "Class1").Methods.First(m => m.Name == "LoadInternal").PInvokeInfo;
//loadInternal.IsPInvokeImpl = true;
//            loadInternal.PInvokeInfo = new PInvokeInfo(PInvokeAttributes.CallConvStdCall, "LoadInternal", attribure.Module);
//            foreach (ParameterDefinition par in method.Parameters)
//                loadInternal.Parameters.Add(par);
//            foreach (CustomAttribute attr in method.CustomAttributes)
//                loadInternal.CustomAttributes.Add(attr);

//            method.CustomAttributes.Clear();
//            method.IsInternalCall = false;
//            method.Attributes = MethodAttributes.Public | MethodAttributes.Static;
//            type.Module.Import(loadInternal);
//            type.Methods.Add(loadInternal);
//            var il = method.Body.GetILProcessor();
//il.ClearInstructions();
//            il.Emit(OpCodes.Ldarg_0);
//            il.Emit(OpCodes.Ldarg_1);
//            il.Append(Instruction.Create(OpCodes.Call, loadInternal));
//            il.Emit(OpCodes.Ret);
//            unity.Write(p + ".Patched.dll");
//            Environment.Exit(1);

//            // ==================