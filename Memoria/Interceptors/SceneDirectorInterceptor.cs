using System;
using System.Threading;

namespace Memoria
{
    public static class SceneDirectorInterceptor
    {
        private static Int32 _initialized = 0;

        public static void ReplaceNow(ref String nextScene)
        {
            Log.Message("[SceneDirectorInterceptor] ReplaceNow: {0}", nextScene);
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 0)
                Initialize();
        }

        private static void Initialize()
        {
            ResourceExporter.ExportSafe();
        }
    }
}