using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Memoria.Assets;
using Memoria.Prime;
using Memoria.Prime.IL;
using Memoria.Prime.Threading;

namespace Memoria.Scripts
{
    public static class ScriptsLoader
    {
        private static volatile Task s_initializationTask;
        private static volatile Result s_result;

        public static void InitializeAsync()
        {
            s_initializationTask = Task.Run(Initialize);
        }

        public static BattleScriptFactory[] GetBaseScripts()
        {
            return GetResult(r => r.BattleBaseScripts);
        }

        public static Dictionary<Int32, BattleScriptFactory> GetExtendedScripts()
        {
            return GetResult(r => r.BattleExtendedScripts);
        }

        private static void Initialize()
        {
            try
            {
                String inputPath = DataResources.ScriptsDirectory + "Memoria.Scripts.dll";
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"[ScriptsLoader] Cannot load Memoria.Scripts.dll because a file does not exist: [{inputPath}].", inputPath);

                Assembly assembly = Assembly.LoadFile(inputPath);
                Result result = new Result();
                TypeOrderer orderer = new TypeOrderer();
                foreach (Type type in assembly.GetTypes().OrderBy(t => t, orderer))
                    ProcessType(type, result);
                s_result = result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ScriptsLoader] Failed to initialize battle calculator's script.");
                UIManager.Input.ConfirmQuit();
            }
        }

        private static void ProcessType(Type type, Result result)
        {
            foreach (Object attribute in type.GetCustomAttributes(false))
            {
                Type attributeType = attribute.GetType();
                if (attributeType == TypeCache<BattleScriptAttribute>.Type)
                    ProcessBattleScript(type, result, attribute);
            }
        }

        private static void ProcessBattleScript(Type type, Result result, Object attribute)
        {
            BattleScriptAttribute bsa = (BattleScriptAttribute)attribute;
            ConstructorInfo constructor = type.GetConstructor(new[] {TypeCache<BattleCalculator>.Type});
            DynamicMethod dm = Expressions.MakeConstructor<BattleCalculator>(type, constructor);
            BattleScriptFactory factory = (BattleScriptFactory)dm.CreateDelegate(TypeCache<BattleScriptFactory>.Type);

            if (bsa.Id < result.BattleBaseScripts.Length && bsa.Id >= 0)
                result.BattleBaseScripts[bsa.Id] = factory;
            else
                result.BattleExtendedScripts[bsa.Id] = factory;
        }

        private static T GetResult<T>(Func<Result, T> selector) where T : class
        {
            try
            {
                if (s_result != null)
                    return selector(s_result);

                if (s_initializationTask == null)
                    InitializeAsync();

                s_initializationTask.Wait();
                return selector(s_result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ScriptsLoader] Failed to get battle calculator's script.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        private sealed class Result
        {
            public readonly BattleScriptFactory[] BattleBaseScripts;
            public readonly Dictionary<Int32, BattleScriptFactory> BattleExtendedScripts;

            public Result()
            {
                BattleBaseScripts = new BattleScriptFactory[256];
                BattleExtendedScripts = new Dictionary<Int32, BattleScriptFactory>();
            }
        }

        private sealed class TypeOrderer : IComparer<Type>
        {
            public Int32 Compare(Type x, Type y)
            {
                String xn = x.Namespace ?? String.Empty;
                String yn = y.Namespace ?? String.Empty;

                if (String.IsNullOrEmpty(xn))
                {
                    if (!String.IsNullOrEmpty(yn))
                        return -1;
                }
                else if (String.IsNullOrEmpty(yn))
                {
                    return 1;
                }

                const String memoriaName = "Memoria";
                if (xn.StartsWith(memoriaName))
                {
                    if (!yn.StartsWith(memoriaName))
                        return -1;
                }
                else if (yn.StartsWith(memoriaName))
                {
                    return 1;
                }

                return String.Compare(x.FullName, y.FullName, StringComparison.Ordinal);

            }
        }
    }
}