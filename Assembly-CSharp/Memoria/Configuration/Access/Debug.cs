using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Debug
        {
            public static Boolean SigningEventObjects => Instance._debug.SigningEventObjects;
            public static Boolean StartModelViewer => Instance._debug.StartModelViewer;
        }
    }
}