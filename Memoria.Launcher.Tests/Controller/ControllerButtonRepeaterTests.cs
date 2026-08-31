using Memoria.Launcher.Controller;
using Xunit;

namespace Memoria.Launcher.Tests.Controller;

public sealed class ControllerButtonRepeaterTests
{
    private static readonly TimeSpan InitialDelay = TimeSpan.FromMilliseconds(350);
    private static readonly TimeSpan RepeatInterval = TimeSpan.FromMilliseconds(90);

    [Fact]
    public void Update_EmitsButtonOnlyOnPressEdge()
    {
        ControllerButtonRepeater repeater = new ControllerButtonRepeater(InitialDelay, RepeatInterval);

        Assert.Equal(ControllerButton.Confirm, repeater.Update(ControllerButton.Confirm, TimeSpan.Zero));
        Assert.Equal(ControllerButton.None, repeater.Update(ControllerButton.Confirm, TimeSpan.FromSeconds(1)));
        Assert.Equal(ControllerButton.None, repeater.Update(ControllerButton.None, TimeSpan.FromSeconds(2)));
        Assert.Equal(ControllerButton.Confirm, repeater.Update(ControllerButton.Confirm, TimeSpan.FromSeconds(3)));
    }

    [Fact]
    public void Update_RepeatsDirectionsAfterDelay()
    {
        ControllerButtonRepeater repeater = new ControllerButtonRepeater(InitialDelay, RepeatInterval);

        Assert.Equal(ControllerButton.Down, repeater.Update(ControllerButton.Down, TimeSpan.Zero));
        Assert.Equal(ControllerButton.None, repeater.Update(ControllerButton.Down, InitialDelay - TimeSpan.FromMilliseconds(1)));
        Assert.Equal(ControllerButton.Down, repeater.Update(ControllerButton.Down, InitialDelay));
        Assert.Equal(ControllerButton.None, repeater.Update(ControllerButton.Down, InitialDelay + RepeatInterval - TimeSpan.FromMilliseconds(1)));
        Assert.Equal(ControllerButton.Down, repeater.Update(ControllerButton.Down, InitialDelay + RepeatInterval));
    }
}
