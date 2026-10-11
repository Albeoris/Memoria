namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// For the context object of a1: query HostCommand.GetSlave|id (sub 0); when the answer is zero or the object's byte +16
    /// is set, send HostCommand.ShowMesh|id (sub 3, a2) and return its result. Otherwise returns the query result, or 0 when no
    /// host callback is installed. Byte +16 is read without a null check after a nonzero answer, as in the original.
    /// </summary>
    public static long HostFadeObjectMeshIfAllowed(int objectIndex, uint fade)
    {
        Recompiler.Runtime.Tracer.Enter("HostFadeObjectMeshIfAllowed(objectIndex, fade)", objectIndex, fade);
        long obj = PsxGetContextQword(objectIndex);
        if (BattleCallbackAddress == null)
        {
            return 0;
        }

        long answer = HostObjCallback(ObjCommand(obj, HostCommand.GetSlave), 0, 0, 0, 0, 0);
        if ((uint)answer != 0 && *(byte*)(obj + 16) == 0)
        {
            return answer;
        }

        if (BattleCallbackAddress == null)
        {
            return 0;
        }

        return HostObjCallback(ObjCommand(obj, HostCommand.ShowMesh), 3, fade, 0, 0, 0);
    }

    /// <summary>
    /// Walks the object list at qword_1800694F0 (next pointer at +0) and calls HostSetObjectMeshFade(obj, a2) on the
    /// objects selected by a1: 0 = flag +16 set, 1 = flag clear, 2 = all, 8 = by the flag of the current object
    /// (qword +80 of the block at dword 3 of off_180066C68): clear -> flag clear, set -> flag set except the context
    /// object 16; 9 = only the current object. Returns the number of calls.
    /// </summary>
    public static long SetMeshFadeForObjectList(int selection, int fade)
    {
        Recompiler.Runtime.Tracer.Enter("SetMeshFadeForObjectList(selection, fade)", selection, fade);
        uint count = 0;
        for (long obj = BattleObjectList; obj != 0; obj = *(long*)obj)
        {
            bool apply;
            switch (selection)
            {
                case 0:
                    apply = *(byte*)(obj + 16) != 0;
                    break;
                case 1:
                    apply = *(byte*)(obj + 16) == 0;
                    break;
                case 2:
                    apply = true;
                    break;
                case 8:
                {
                    long current = ((Site_ResolveGlobalPsxAddress5*)SetMeshFadeForObjectListBlock())->field_80;
                    if (*(byte*)(current + 16) == 0)
                    {
                        apply = *(byte*)(obj + 16) == 0;
                    }
                    else
                    {
                        apply = obj != PsxGetContextQword(16) && *(byte*)(obj + 16) != 0;
                    }

                    break;
                }

                case 9:
                    apply = obj == ((Site_ResolveGlobalPsxAddress5*)SetMeshFadeForObjectListBlock())->field_80;
                    break;
                default:
                    apply = false;
                    break;
            }

            if (apply)
            {
                HostSetObjectMeshFade(obj, fade);
                ++count;
            }
        }

        return count;
    }

    /// <summary>
    /// For the context object of a1: sends HostCommand.CheckStatus|id with sub-id 3 when the context object's byte +16 is set,
    /// else 2 (other arguments 0; the object is fetched twice and the flag read without a null check). Returns the
    /// callback result, or 0 without a callback.
    /// </summary>
    public static long HostResetObjectStatuses(int objectIndex)
    {
        Recompiler.Runtime.Tracer.Enter("HostResetObjectStatuses(objectIndex)", objectIndex);
        long obj = PsxGetContextQword(objectIndex);
        ulong sub = *(byte*)(PsxGetContextQword(objectIndex) + 16) != 0 ? 3UL : 2UL;
        if (BattleCallbackAddress == null)
        {
            return 0;
        }

        return HostObjCallback(ObjCommand(obj, HostCommand.CheckStatus), sub, 0, 0, 0, 0);
    }

    /// <summary>Host pointer of the PSX block whose address is dword 3 of off_180066C68 (0 when unmapped).</summary>
    private static byte* SetMeshFadeForObjectListBlock()
    {
        Recompiler.Runtime.Tracer.Enter("SetMeshFadeForObjectListBlock()");
        return ResolveGlobalPsxAddress(((uint*)off_180066C68)[3]);
    }
}
