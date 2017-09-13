using System;

namespace Memoria.MSBuild
{
    internal static class BuildEnvironment
    {
        public static Boolean IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
    }
}