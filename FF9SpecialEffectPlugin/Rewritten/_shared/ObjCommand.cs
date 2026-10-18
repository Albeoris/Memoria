using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    // Shared helper (recomp fn dedupe-helpers): the same body was also HostObjectCode (Rewritten/u0097_HostObjectsMessage17.cs).
    /// <summary>command | object id (u16 at +24), or the bare command for a null object.</summary>
    private static ulong ObjCommand(long obj, uint command)
    {
        Recompiler.Runtime.Tracer.Enter("ObjCommand(obj, command)", obj, command);
        return obj != 0 ? *(ushort*)(obj + 24) | command : command;
    }
}
