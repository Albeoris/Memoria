using System;
using System.Collections.Generic;

public static class ListExtension
{
    public static T Pop<T>(this List<T> theList)
    {
        T result = theList[theList.Count - 1];
        theList.RemoveAt(theList.Count - 1);
        return result;
    }

    public static void Push<T>(this List<T> theList, T item)
    {
        theList.Add(item);
    }
}
