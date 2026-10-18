namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>When flag +16 of object a1 is set: host 0x1C000000 | id (a2, a3); returns the callback result or the object.</summary>
    public static long HostObjCmd1C(long a1, uint a2, uint a3)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd1C(a1, a2, a3)", a1, a2, a3);
        uint index = (uint)a1;
        long result = PsxGetContextQword((int)a1);
        if (*(byte*)(result + 16) != 0)
        {
            result = PsxGetContextQword((int)index);
            if (BattleCallbackAddress != null)
            {
                return HostObjCallback(ObjCommand(result, HostCommand.EyeBlink), a2, a3, 0, 0, 0);
            }
        }

        return result;
    }

    /// <summary>
    /// Sets the byte at 0x180323195 (high byte of dword_180323192) to 1 when the signed count byte at +39 of the context
    /// at PSX address off_180066C68[3] is 0; returns the resulting byte.
    /// </summary>
    public static long MarkNoExtraObjects()
    {
        Recompiler.Runtime.Tracer.Enter("MarkNoExtraObjects()");
        byte* context = ResolveGlobalPsxAddress(ContextAddress(3));
        long result = *unk_180323195;
        if (context[39] == 0)
        {
            result = 1;
        }

        *unk_180323195 = (byte)result;
        return result;
    }

    /// <summary>When flag +16 of context object 16 is set: host 0x71000000 (no arguments); returns its result or the object.</summary>
    public static byte* HostCmd71IfObj16Flag()
    {
        Recompiler.Runtime.Tracer.Enter("HostCmd71IfObj16Flag()");
        var result = (byte*)PsxGetContextQword(16);
        if (result[16] != 0)
        {
            result = (byte*)BattleCallbackAddress;
            if (BattleCallbackAddress != null)
            {
                return (byte*)HostObjCallback(HostCommand.EffectTitle, 0, 0, 0, 0, 0);
            }
        }

        return result;
    }
}
