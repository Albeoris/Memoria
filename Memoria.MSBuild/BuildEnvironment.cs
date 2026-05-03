using System;

namespace Memoria.MSBuild
{
    internal static class BuildEnvironment
    {
        public static Boolean IsDebug
        {
            get
            {
                return false;
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
    }
}
