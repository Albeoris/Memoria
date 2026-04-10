using System;

namespace Memoria.MSBuild
{
    internal enum DeploymentItemKind
    {
        Folder,
        ManagedDll,
        File
    }

    internal sealed class DeploymentFileDefinition
    {
        public DeploymentFileDefinition(DeploymentItemKind kind, String sourceRelativePath, String targetRelativePath)
        {
            Kind = kind;
            SourceRelativePath = sourceRelativePath;
            TargetRelativePath = targetRelativePath;
        }

        public DeploymentItemKind Kind { get; }
        public String SourceRelativePath { get; }
        public String TargetRelativePath { get; }
    }

    internal static class DeploymentFileSet
    {
        public static readonly DeploymentFileDefinition[] Files =
        {
            new DeploymentFileDefinition(DeploymentItemKind.Folder, @"StreamingAssets", @"StreamingAssets"),
            new DeploymentFileDefinition(DeploymentItemKind.Folder, @"FF9_Data", @"FF9_Data"),
            new DeploymentFileDefinition(DeploymentItemKind.Folder, @"Debugger", @"Debugger"),
            new DeploymentFileDefinition(DeploymentItemKind.ManagedDll, @"Assembly-CSharp.dll", @"{PLATFORM}\FF9_Data\Managed\Assembly-CSharp.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.ManagedDll, @"Memoria.Prime.dll", @"{PLATFORM}\FF9_Data\Managed\Memoria.Prime.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.ManagedDll, @"UnityEngine.UI.dll", @"{PLATFORM}\FF9_Data\Managed\UnityEngine.UI.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"Launcher\Memoria.Launcher.exe", @"FF9_Launcher.exe"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"Launcher\Memoria.Launcher.exe.config", @"FF9_Launcher.exe.config"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"Launcher\Memoria.SteamFix.exe", @"Memoria.SteamFix.exe"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"Launcher\Memoria.ini", @"Memoria.ini"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"Launcher\Settings.ini", @"Settings.ini"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"XInputDotNetPure.dll", @"{PLATFORM}\FF9_Data\Managed\XInputDotNetPure.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"Newtonsoft.Json.dll", @"{PLATFORM}\FF9_Data\Managed\Newtonsoft.Json.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"System.Runtime.Serialization.dll", @"{PLATFORM}\FF9_Data\Managed\System.Runtime.Serialization.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"JoyShockLibrary\x64\JoyShockLibrary.dll", @"x64\FF9_Data\Plugins\JoyShockLibrary.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"JoyShockLibrary\x86\JoyShockLibrary.dll", @"x86\FF9_Data\Plugins\JoyShockLibrary.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"Global\Sound\SoLoud\x64\soloud.dll", @"x64\FF9_Data\Plugins\soloud.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"Global\Sound\SoLoud\x86\soloud.dll", @"x86\FF9_Data\Plugins\soloud.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"Global\Sound\SaXAudio\x64\SaXAudio.dll", @"x64\FF9_Data\Plugins\SaXAudio.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"Global\Sound\SaXAudio\x86\SaXAudio.dll", @"x86\FF9_Data\Plugins\SaXAudio.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"Global\Sound\SaXAudio\x64\XAudio2_9.dll", @"x64\XAudio2_9.dll"),
            new DeploymentFileDefinition(DeploymentItemKind.File, @"Global\Sound\SaXAudio\x86\XAudio2_9.dll", @"x86\XAudio2_9.dll"),
        };
    }
}