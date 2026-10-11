using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Debug
        {
            public static readonly Debugging.GameLaunchOptions LaunchOptions = Debugging.GameLaunchOptions.Parse(Environment.GetCommandLineArgs());

            public static Boolean SigningEventObjects => Instance._debug.SigningEventObjects;
            public static Boolean StartModelViewer => LaunchOptions.ModelViewer || Instance._debug.StartModelViewer;
            public static Boolean StartFieldCreator => LaunchOptions.FieldCreator || Instance._debug.StartFieldCreator;
            public static Boolean StartBattle => LaunchOptions.Battle.HasValue;
            public static Boolean RenderWalkmeshes => Instance._debug.RenderWalkmeshes;
            public static Int32 Test => Instance._debug.Test;
        }
    }
}
