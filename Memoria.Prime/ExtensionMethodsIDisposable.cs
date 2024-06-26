using System;
using System.Collections.Generic;

namespace Memoria.Prime
{
    internal static class ExtensionMethodsIDisposable
    {
        public static void SafeDispose(this IDisposable self)
        {
            try
            {
                if (!ReferenceEquals(self, null))
                    self.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public static void SafeDispose(this IEnumerable<IDisposable> self)
        {
            if (ReferenceEquals(self, null))
                return;

            foreach (IDisposable item in self)
                SafeDispose(item);
        }
    }
}