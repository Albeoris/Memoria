namespace Antlr.Runtime.Misc
{
    using System.Text.RegularExpressions;

#if PORTABLE
    using System;
#endif

    internal static class RegexOptionsHelper
    {
        public static readonly RegexOptions Compiled;

        static RegexOptionsHelper()
        {
#if !PORTABLE
            //Compiled = RegexOptions.Compiled;
            Compiled = RegexOptions.None;
#else
            if (!Enum.TryParse("Compiled", out Compiled))
                Compiled = RegexOptions.None;
#endif
        }
    }
}
