namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Initializers <see cref="RunModuleInitializers"/> calls (1 of 1 recorded).</summary>
    public const int ModuleInitializerCount = 1;

    /// <summary>Runs the rewritten CRT initializers in the original's order; ModuleData calls it once.</summary>
    internal static void RunModuleInitializers()
    {
        _ = InitMapperTable(); // 6. qword_18004A1C0..qword_18004A1F0 slot 0x4a1e0: InitMapperTable
    }
}
