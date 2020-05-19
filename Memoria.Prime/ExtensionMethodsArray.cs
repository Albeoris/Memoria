using System;

namespace Memoria.Prime
{
    public static class ExtensionMethodsArray
    {
        public static ArraySegment<T> Segment<T>(this T[] array, Int32 offset)
        {
            return new ArraySegment<T>(array, offset, array.Length - offset);
        }
        
        public static ArraySegment<T> Segment<T>(this T[] array, Int32 offset, Int32 size)
        {
            return new ArraySegment<T>(array, offset, size);
        }
    }
}