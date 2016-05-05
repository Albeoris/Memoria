using System;
using System.Reflection;

// ReSharper disable InconsistentNaming

namespace Memoria
{
    public static class EventEngineUtilsAccessor
    {
        private const BindingFlags SearchFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly Type Type = Type.GetType("EventEngineUtils, Assembly-CSharp", true, false);

        public delegate byte[] LoadEventDataDelegate(string ebFileName, string ebSubFolder);
        public static readonly LoadEventDataDelegate LoadEventData = loadEventData();

        private static LoadEventDataDelegate loadEventData()
        {
            return new LoadEventDataDelegate(Expressions.MakeStaticFunction<String, String, Byte[]>(Type.GetMethod(nameof(loadEventData), SearchFlags)));
        }
    }
}