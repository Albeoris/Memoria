namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Memoria SFX.Callback: five signed 32-bit values and a host pointer, returning Int32.
    /// The command is in bits 24..31 of code; the battle id occupies the low byte.
    /// Narrow register-shaped operands at the boundary and preserve EAX zero-extension for existing internal callers.
    /// Evidence: ref:R001/Assembly-CSharp/Global/SFX/SFX.cs#L832-L841 and #L2537.
    /// </summary>
    internal static long InvokeBattleCallback(long callback, ulong code, ulong arg0, ulong arg1, ulong arg2, ulong arg3, ulong payload)
    {
        Recompiler.Runtime.Tracer.Enter("InvokeBattleCallback(callback, code, arg0, arg1, arg2, arg3, payload)", callback, code, arg0, arg1, arg2, arg3, payload);
        var method = (BattleCallbackFn)callback;
        return unchecked((uint)method((int)code, (int)arg0, (int)arg1, (int)arg2, (int)arg3, (void*)payload));
    }
}
