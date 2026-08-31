using Memoria.Launcher.Controller;
using Xunit;

namespace Memoria.Launcher.Tests.Controller;

public sealed class SpatialNavigationTests
{
    [Fact]
    public void FindNext_PrefersAlignedControlOverCloserDiagonalControl()
    {
        NavigationRectangle origin = Rect(100, 100);
        SpatialNavigationCandidate<String> aligned = Candidate("aligned", 100, 220);
        SpatialNavigationCandidate<String> diagonal = Candidate("diagonal", 220, 150);

        SpatialNavigationCandidate<String>? result = SpatialNavigation.FindNext(origin, new[] { diagonal, aligned }, NavigationDirection.Down);

        Assert.NotNull(result);
        Assert.Equal("aligned", result.Value);
    }

    [Fact]
    public void FindNext_OnlyConsidersControlsInRequestedHalfPlane()
    {
        NavigationRectangle origin = Rect(100, 100);
        SpatialNavigationCandidate<String> left = Candidate("left", 0, 100);
        SpatialNavigationCandidate<String> right = Candidate("right", 200, 100);

        SpatialNavigationCandidate<String>? result = SpatialNavigation.FindNext(origin, new[] { left, right }, NavigationDirection.Right);

        Assert.NotNull(result);
        Assert.Equal("right", result.Value);
    }

    [Fact]
    public void FindNext_ReturnsNullWhenThereIsNoControlInDirection()
    {
        NavigationRectangle origin = Rect(100, 100);
        SpatialNavigationCandidate<String> above = Candidate("above", 100, 0);

        Assert.Null(SpatialNavigation.FindNext(origin, new[] { above }, NavigationDirection.Down));
    }

    [Fact]
    public void FindNext_HorizontalNavigationIgnoresOffsetControlInSameColumn()
    {
        NavigationRectangle origin = new NavigationRectangle(100, 100, 120, 30);
        SpatialNavigationCandidate<String> sameColumnBelow = new SpatialNavigationCandidate<string>(
            "same-column",
            new NavigationRectangle(110, 160, 140, 30));
        
        SpatialNavigationCandidate<String> actualRightNeighbour = new SpatialNavigationCandidate<string>(
            "right",
            new NavigationRectangle(280, 105, 80, 30));

        SpatialNavigationCandidate<String>? result = SpatialNavigation.FindNext(
            origin,
            new[] { sameColumnBelow, actualRightNeighbour },
            NavigationDirection.Right);

        Assert.NotNull(result);
        Assert.Equal("right", result.Value);
    }

    [Fact]
    public void FindNext_HorizontalNavigationDoesNothingWithoutEdgeSeparatedNeighbour()
    {
        NavigationRectangle origin = new NavigationRectangle(100, 100, 120, 30);
        SpatialNavigationCandidate<String> overlappingBelow = new SpatialNavigationCandidate<string>(
            "overlapping",
            new NavigationRectangle(180, 160, 80, 30));

        Assert.Null(SpatialNavigation.FindNext(
            origin,
            new[] { overlappingBelow },
            NavigationDirection.Right));
    }

    [Fact]
    public void FindNext_VerticalNavigationIgnoresOverlappingControlInSameRow()
    {
        NavigationRectangle origin = new NavigationRectangle(100, 100, 80, 50);
        SpatialNavigationCandidate<String> overlappingSameRow = new SpatialNavigationCandidate<string>(
            "same-row",
            new NavigationRectangle(0, 110, 80, 50));
        
        SpatialNavigationCandidate<String> actualLowerNeighbour = new SpatialNavigationCandidate<string>(
            "below",
            new NavigationRectangle(100, 200, 80, 30));

        SpatialNavigationCandidate<String>? result = SpatialNavigation.FindNext(
            origin,
            new[] { overlappingSameRow, actualLowerNeighbour },
            NavigationDirection.Down);

        Assert.NotNull(result);
        Assert.Equal("below", result.Value);
    }

    private static SpatialNavigationCandidate<string> Candidate(string value, double x, double y) =>
        new(value, Rect(x, y));

    private static NavigationRectangle Rect(double x, double y) => new(x, y, 80, 30);
}
