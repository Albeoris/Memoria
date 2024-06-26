using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Debug
        {
            public static Boolean SigningEventObjects => Instance._debug.SigningEventObjects;
            public static Boolean StartModelViewer => Instance._debug.StartModelViewer;
            public static Boolean StartFieldCreator => Instance._debug.StartFieldCreator;
            public static Boolean RenderWalkmeshes => Instance._debug.RenderWalkmeshes;
        }
    }
}
