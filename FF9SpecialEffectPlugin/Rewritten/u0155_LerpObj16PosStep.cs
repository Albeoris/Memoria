namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Table handlers that move an object along a GTE INTPL interpolation between a start vector and a target position
/// over a4 steps; both inline GteIntplVector (GTE-044) and the second inlines HostQueryObjCmd02ById (HOST-011).
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>
    /// Step a3 of a4 for object a1: on step 1 queries the object position into a6; then interpolates a6 towards
    /// a2 with IR0 = (a3 &lt;&lt; 12) / a4 (signed idiv) and sends the result with HostQueryObjCmd02ById.
    /// Returns a3 &gt;= a4.
    /// </summary>
    public static long LerpObjPosStep(long a1, short* a2, int a3, int a4, int a5, short* a6)
    {
        Recompiler.Runtime.Tracer.Enter("LerpObjPosStep(a1, a2, a3, a4, a5, a6)", a1, (nint)a2, a3, a4, a5, (nint)a6);
        if (a3 == 1)
            HostQueryObjPositionById((int)a1, (long)a6);

        ushort* result = stackalloc ushort[4];
        GteIntplVector(a6, a2, (a3 << 12) / a4, result);
        HostSetObjectPositionById((int)a1, (long)result);
        return a3 >= a4 ? 1 : 0;
    }

    /// <summary>
    /// Step a3 of a4 (0 treated as 1): on step 0 reads the start position into a5 (a6 == 0: HostQueryObjCmd01ById
    /// of object a1, a6 == 1: its position; any other a6 returns 1 at once); interpolates a5 towards a2 with
    /// IR0 = min(((a3 + 1) &lt;&lt; 12) / a4, 4096) and sends the result to object 16 through HostQueryObjCmd02ById.
    /// Returns a3 &gt;= a4 - 1.
    /// </summary>
    public static long LerpObj16PosStep(long a1, short* a2, int a3, int a4, short* a5, int a6)
    {
        Recompiler.Runtime.Tracer.Enter("LerpObj16PosStep(a1, a2, a3, a4, a5, a6)", a1, (nint)a2, a3, a4, (nint)a5, a6);
        int steps = a4 != 0 ? a4 : 1;
        if (a3 == 0)
        {
            if (a6 == 0)
            {
                HostGetObjectPositionById((int)a1, (long)a5);
            }
            else if (a6 == 1)
            {
                HostQueryObjPositionById((int)a1, (long)a5);
            }
            else
            {
                return 1;
            }
        }

        int ratio = ((a3 + 1) << 12) / steps;
        if (ratio > 4096)
            ratio = 4096;

        ushort* result = stackalloc ushort[4];
        GteIntplVector(a5, a2, ratio, result);
        HostSetObjectPositionById(16, (long)result);
        return a3 >= steps - 1 ? 1 : 0;
    }
}
