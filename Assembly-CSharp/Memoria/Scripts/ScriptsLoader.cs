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
        
        private const string DefaultBattleCharacterShader = "PSX/BattleMap_StatusEffect";
        private const string ToonBattleCharacterShader = "PSX/BattleMap_StatusEffect_Toon";
        private const string RealismBattleCharacterShader = "PSX/BattleMap_StatusEffect_RealLighting";
        
        private const string DefaultFieldCharacterShader = "PSX/FieldMapActor";
        private const string ToonFieldCharacterShader = "PSX/FieldMapActor_Toon";
        private const string RealismFieldCharacterShader = "PSX/FieldMapActor_RealLighting";

        public static string GetCurrentBattleCharcterShader
        {
            get
            {
                if (Configuration.Graphics.CustomShader == 1)
                {
                    if (Configuration.Graphics.EnableToonShadingBattle == 1)
                        return ToonBattleCharacterShader;
                    if (Configuration.Graphics.EnableRealismShadingBattle == 1)
                        return RealismBattleCharacterShader;
                }

                return DefaultBattleCharacterShader;
            }
        }
        
        public static string GetCurrentFieldMapCharcterShader
        {
            get
            {
                if (Configuration.Graphics.CustomShader == 1)
                {
                    if (Configuration.Graphics.EnableToonShadingField == 1)
                        return ToonFieldCharacterShader;
                    if (Configuration.Graphics.EnableRealismShadingField == 1)
                        return RealismFieldCharacterShader;
                }
                return DefaultFieldCharacterShader;
            }
        }

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
                String[] dir = Configuration.Mod.AllFolderNames;
                Boolean foundOneDir = false;
                s_shaders = new Dictionary<String, Shader>();
                for (Int32 i = 0; i < dir.Length; i++)
                {
                    rootPath = DataResources.ShadersModDirectory(dir[i]);
                    if (Directory.Exists(rootPath))
                    {
                        String[] shaderFiles = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);

                        foreach (String shaderPath in shaderFiles)
                            InitializeMaterial(shaderPath, rootPath, s_shaders);

                        // Create a watcher only for the shader folder that has priority
                        if (!foundOneDir)
                        {
                            // ReSharper disable once InconsistentlySynchronizedField
                            s_watcher = new FileSystemWatcher(rootPath, "*");
                            GameLoopManager.Quit += s_watcher.Dispose;

                            s_watcher.IncludeSubdirectories = true;
                            s_watcher.Changed += OnChangedFileInDirectory;
                            s_watcher.Created += OnChangedFileInDirectory;
                            s_watcher.EnableRaisingEvents = true;
                        }
                        foundOneDir = true;
                    }
                }
                if (!foundOneDir)
                    throw new DirectoryNotFoundException($"[ShadersLoader] Cannot load external shaders because a directory does not exist: [{rootPath}].");
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
            if (shaders.ContainsKey(shaderName))
                return;

            String shaderCode = File.ReadAllText(shaderPath);
            Material newShader = new Material(shaderCode);
            if (newShader.shader.isSupported)
                shaders[shaderName] = newShader.shader;
            else
                Log.Warning("[ShadersLoader] Shader isn't supported: " + shaderPath);
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

                        String rootPath = s_watcher.Path;
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
                Log.Error(ex, $"[ShadersLoader] Failed to find shader {shaderName}.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }
    }

    public static class ScriptsLoader
    {
        private static volatile Task s_initializationTask;
        private static volatile List<Result> s_result = new List<Result>();

        public static void InitializeAsync()
        {
            s_initializationTask = Task.Run(Initialize);
        }

        public static BattleScriptFactory GetBattleScript(Int32 scriptId)
        {
            Result result = GetScriptResult(scriptId);
            if (result == null)
                return null;

            if (scriptId >= 0 && scriptId < result.BattleBaseScripts.Length)
                return result.BattleBaseScripts[scriptId];
            return result.BattleExtendedScripts.TryGetValue(scriptId, out BattleScriptFactory script) ? script : null;
        }

        public static String GetScriptDLL(Int32 scriptId)
		{
            Result result = GetScriptResult(scriptId);
            if (result != null)
                return result.DLLPath;
            return String.Empty;
        }

        private static Result GetScriptResult(Int32 scriptId)
        {
            if (s_result.Count == 1)
                return s_result[0];
            if (s_result.Count == 0)
            {
                try
                {
                    if (s_initializationTask == null)
                        InitializeAsync();

                    s_initializationTask.Wait();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[ScriptsLoader] Failed to get battle calculator's script.");
                    UIManager.Input.ConfirmQuit();
                    return null;
                }
            }
            foreach (Result result in s_result)
            {
                if (scriptId >= 0 && scriptId < result.BattleBaseScripts.Length && result.BattleBaseScripts[scriptId] != null)
                    return result;
                if (result.BattleExtendedScripts.ContainsKey(scriptId))
                    return result;
            }
            return null;
        }

        private static void Initialize()
        {
            try
            {
                s_result.Clear();
                String mainDllPath = DataResources.PureScriptsDirectory + "Memoria.Scripts.dll";
                TypeOrderer orderer = new TypeOrderer();
                Int32 dllCount = 0;
                foreach (AssetManager.AssetFolder folder in AssetManager.FolderHighToLow)
                {
                    // Assume that the DLL file name matches the internal DLL name (ie. "Memoria.Scripts.MyMod.dll" is not just a renamed "Memoria.Scripts.dll")
                    String fullPath;
                    if (!String.IsNullOrEmpty(folder.FolderPath))
                    {
                        String partialDllName = $"Memoria.Scripts.{folder.FolderPath.Trim('/').Replace('/', '.')}.dll";
                        if (folder.TryFindAssetInModOnDisc(DataResources.PureScriptsDirectory + partialDllName, out fullPath, AssetManagerUtil.GetStreamingAssetsPath() + "/"))
                        {
                            Assembly assembly = Assembly.LoadFile(fullPath);
                            Result result = new Result(fullPath);
                            foreach (Type type in assembly.GetTypes().OrderBy(t => t, orderer))
                                ProcessType(type, result);
                            s_result.Add(result);
                            dllCount++;
                        }
                    }
                    if (folder.TryFindAssetInModOnDisc(mainDllPath, out fullPath, AssetManagerUtil.GetStreamingAssetsPath() + "/"))
                    {
                        Assembly assembly = Assembly.LoadFile(fullPath);
                        Result result = new Result(fullPath);
                        foreach (Type type in assembly.GetTypes().OrderBy(t => t, orderer))
                            ProcessType(type, result);
                        s_result.Add(result);
                        return;
                    }
                }
                if (dllCount == 0)
                {
                    mainDllPath = DataResources.ScriptsDirectory + "Memoria.Scripts.dll";
                    throw new FileNotFoundException($"[ScriptsLoader] Cannot load Memoria.Scripts.dll because a file does not exist: [{mainDllPath}].", mainDllPath);
                }
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

        private sealed class Result
        {
            public readonly BattleScriptFactory[] BattleBaseScripts;
            public readonly Dictionary<Int32, BattleScriptFactory> BattleExtendedScripts;
            public readonly String DLLPath;

            public Result(String dllPath)
            {
                BattleBaseScripts = new BattleScriptFactory[256];
                BattleExtendedScripts = new Dictionary<Int32, BattleScriptFactory>();
                DLLPath = dllPath;
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
