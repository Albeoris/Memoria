using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Memoria.Assets;
using Memoria.Prime;
using Memoria.Prime.IL;
using Memoria.Prime.Threading;
using UnityEngine;
using Object = System.Object;

namespace Memoria.Scripts
{
    public static class ShadersLoader
    {
        private static volatile Task s_initializationTask;
        private static volatile Dictionary<String, Shader> s_shaders;
        private static volatile FileSystemWatcher s_watcher;

        public static void InitializeAsync()
        {
            s_initializationTask = Task.Run(Initialize);
        }

        public static Material CreateShaderMaterial(String shaderName)
        {
            Shader shader = FindShaderSafe(shaderName);
            if (shader == null)
                shader = Shader.Find(shaderName);

            if (shader != null)
                return new Material(shader);

            return null;
        }

        public static Shader Find(String shaderName)
        {
            return FindShader(shaderName);
        }

        public static Shader FindShader(String shaderName)
        {
            if (shaderName != "Unlit/Transparent Colored")
            {
                Shader shaderMaterial = FindShaderSafe(shaderName);
                if (shaderMaterial != null)
                    return shaderMaterial;
            }
            else
            {
                Shader shader = Shader.Find(shaderName);
                if (shader != null)
                    return shader;
            }

            return null;
        }

        private static void Initialize()
        {
            try
            {
                String rootPath = DataResources.ShadersDirectory;
                if (!Directory.Exists(rootPath))
                    throw new DirectoryNotFoundException($"[ShadersLoader] Cannot load external shaders because a directory does not exist: [{rootPath}].");

                String[] shaderFiles = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);
                Dictionary<String, Shader> shaders = new Dictionary<String, Shader>(shaderFiles.Length);

                foreach (String shaderPath in shaderFiles)
                    InitializeMaterial(shaderPath, rootPath, shaders);

                // ReSharper disable once InconsistentlySynchronizedField
                s_shaders = shaders;

                s_watcher = new FileSystemWatcher(rootPath, "*");
                GameLoopManager.Quit += s_watcher.Dispose;

                s_watcher.IncludeSubdirectories = true;
                s_watcher.Changed += OnChangedFileInDirectory;
                s_watcher.Created += OnChangedFileInDirectory;
                s_watcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ShadersLoader] Failed to load external shaders.");
                UIManager.Input.ConfirmQuit();
            }
        }

        private static void InitializeMaterial(String shaderPath, String rootPath, Dictionary<String, Shader> shaders)
        {
            String shaderName = Path.ChangeExtension(shaderPath.Substring(rootPath.Length), extension: null).Replace('\\', '/');
            String shaderCode = File.ReadAllText(shaderPath);
            Material newShader = new Material(shaderCode);

            if (newShader.shader.isSupported)
            {
                shaders[shaderName] = newShader.shader;
            }
            else
            {
                shaders[shaderName] = null;
                Log.Warning("[ShadersLoader] Shader isn't supported: " + shaderPath);
            }
        }

        private static void OnChangedFileInDirectory(Object sender, FileSystemEventArgs e)
        {
            Task.Run(DoWatch, e);
        }

        private static void DoWatch(FileSystemEventArgs e)
        {
            Int32 retryCount = 10;

            try
            {
                while (retryCount > 0)
                {
                    try
                    {
                        DateTime beginTime = DateTime.UtcNow;
                        Log.Message($"[ShadersLoader] An external shader was changed. Reloading... [{e.FullPath}]");

                        String rootPath = DataResources.ShadersDirectory;
                        rootPath = Path.GetFullPath(rootPath);

                        lock (s_shaders)
                            InitializeMaterial(e.FullPath, rootPath, s_shaders);

                        Log.Message($"[ShadersLoader] Reloaded for {DateTime.UtcNow - beginTime}");
                        return;
                    }
                    catch (IOException ex)
                    {
                        retryCount--;
                        Log.Error(ex, $"[ShadersLoader] Failed to reload an external shader {e.FullPath}.");
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ShadersLoader] Failed to realod an external shader.");
            }
        }

        private static Shader FindShaderSafe(String shaderName)
        {
            try
            {
                if (s_shaders == null)
                {
                    if (s_initializationTask == null)
                        InitializeAsync();

                    s_initializationTask.Wait();
                }

                lock (s_shaders)
                {
                    Shader shaderMaterial;
                    if (s_shaders.TryGetValue(shaderName, out shaderMaterial))
                        return shaderMaterial;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ShadersLoader] Failed to get battle calculator's script.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }
    }

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