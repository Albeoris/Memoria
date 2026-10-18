using Memoria.Launcher.Utils.Mods;
using Xunit;

namespace Memoria.Launcher.Tests.Mods;

public sealed class ModUninstallFailureMessageBuilderTests
{
    [Fact]
    public void Build_includes_the_mod_path_and_complete_exception_chain()
    {
        IOException cause = new("Access denied for 10_0.png.");
        InvalidOperationException failure = new("Directory cleanup failed.", cause);

        String message = ModUninstallFailureMessageBuilder.Build("Moguri Mod (2020)", "MoguriFiles", failure);

        Assert.Contains("Moguri Mod (2020)", message, StringComparison.Ordinal);
        Assert.Contains("MoguriFiles", message, StringComparison.Ordinal);
        Assert.Contains("InvalidOperationException: Directory cleanup failed.", message, StringComparison.Ordinal);
        Assert.Contains("IOException: Access denied for 10_0.png.", message, StringComparison.Ordinal);
    }
}
