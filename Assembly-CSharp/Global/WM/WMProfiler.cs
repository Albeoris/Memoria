using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class WMProfiler
{
    public static void Begin(String message)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        WMProfiler.Data t = new WMProfiler.Data
        {
            Message = message,
            Stopwatch = stopwatch
        };
        WMProfiler.DataStack.Push(t);
    }

    public static void End()
    {
        WMProfiler.Data data = WMProfiler.DataStack.Pop();
        String message = data.Message;
        Stopwatch stopwatch = data.Stopwatch;
        stopwatch.Stop();
        Double totalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
        Double num = totalMilliseconds / 1000.0;
    }

    private static readonly Stack<WMProfiler.Data> DataStack = new Stack<WMProfiler.Data>();

    private class Data
    {
        public String Message;

        public Stopwatch Stopwatch;
    }
}
