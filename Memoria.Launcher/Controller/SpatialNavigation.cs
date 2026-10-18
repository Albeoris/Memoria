#nullable enable

using System;
using System.Collections.Generic;

namespace Memoria.Launcher.Controller
{
    internal enum NavigationDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    internal readonly struct NavigationRectangle(Double x, Double y, Double width, Double height)
    {
        public Double X { get; } = x;
        public Double Y { get; } = y;
        public Double Width { get; } = width;
        public Double Height { get; } = height;
        public Double Left => X;
        public Double Top => Y;
        public Double Right => X + Width;
        public Double Bottom => Y + Height;
        public Double CenterX => X + Width / 2.0;
        public Double CenterY => Y + Height / 2.0;
    }

    internal sealed class SpatialNavigationCandidate<T>(T value, NavigationRectangle bounds)
    {
        public T Value { get; } = value;
        public NavigationRectangle Bounds { get; } = bounds;
    }

    /// <summary>
    /// Finds the geometrically most natural neighbour. Alignment is weighted
    /// more heavily than raw distance so navigation remains within visual rows
    /// and columns whenever possible.
    /// </summary>
    internal static class SpatialNavigation
    {
        private const Double DirectionTolerance = 1.0;

        public static SpatialNavigationCandidate<T>? FindNext<T>(
            NavigationRectangle origin,
            IEnumerable<SpatialNavigationCandidate<T>> candidates,
            NavigationDirection direction)
        {
            SpatialNavigationCandidate<T>? best = null;
            Double bestScore = Double.PositiveInfinity;

            foreach (SpatialNavigationCandidate<T> candidate in candidates)
            {
                if (!IsStrictlyInDirection(origin, candidate.Bounds, direction))
                    continue;

                Double primaryCenter = GetPrimaryCenterDistance(origin, candidate.Bounds, direction);
                if (primaryCenter <= 1.0)
                    continue;

                Double primaryGap = GetPrimaryGap(origin, candidate.Bounds, direction);
                Double perpendicularGap = GetPerpendicularGap(origin, candidate.Bounds, direction);
                Double perpendicularCenter = GetPerpendicularCenterDistance(origin, candidate.Bounds, direction);
                Double score = Math.Max(0.0, primaryGap)
                             + perpendicularGap * 5.0
                             + perpendicularCenter * 0.01;

                if (score < bestScore)
                {
                    best = candidate;
                    bestScore = score;
                }
            }

            return best;
        }

        private static Boolean IsStrictlyInDirection(
            NavigationRectangle origin,
            NavigationRectangle candidate,
            NavigationDirection direction)
        {
            switch (direction)
            {
                // Horizontal navigation must cross the corresponding edge.
                // Comparing centres incorrectly treats differently-sized
                // controls in the same column as left/right neighbours.
                case NavigationDirection.Left:
                    return candidate.Right <= origin.Left + DirectionTolerance;
                case NavigationDirection.Right:
                    return candidate.Left >= origin.Right - DirectionTolerance;
                case NavigationDirection.Up:
                    return candidate.Bottom <= origin.Top + DirectionTolerance;
                case NavigationDirection.Down:
                    return candidate.Top >= origin.Bottom - DirectionTolerance;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }

        private static Double GetPrimaryCenterDistance(NavigationRectangle origin, NavigationRectangle candidate, NavigationDirection direction)
        {
            switch (direction)
            {
                case NavigationDirection.Up: return origin.CenterY - candidate.CenterY;
                case NavigationDirection.Down: return candidate.CenterY - origin.CenterY;
                case NavigationDirection.Left: return origin.CenterX - candidate.CenterX;
                case NavigationDirection.Right: return candidate.CenterX - origin.CenterX;
                default: throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }

        private static Double GetPrimaryGap(NavigationRectangle origin, NavigationRectangle candidate, NavigationDirection direction)
        {
            switch (direction)
            {
                case NavigationDirection.Up: return origin.Top - candidate.Bottom;
                case NavigationDirection.Down: return candidate.Top - origin.Bottom;
                case NavigationDirection.Left: return origin.Left - candidate.Right;
                case NavigationDirection.Right: return candidate.Left - origin.Right;
                default: throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }

        private static Double GetPerpendicularGap(NavigationRectangle origin, NavigationRectangle candidate, NavigationDirection direction)
        {
            Boolean vertical = direction == NavigationDirection.Up || direction == NavigationDirection.Down;
            Double firstStart = vertical ? origin.Left : origin.Top;
            Double firstEnd = vertical ? origin.Right : origin.Bottom;
            Double secondStart = vertical ? candidate.Left : candidate.Top;
            Double secondEnd = vertical ? candidate.Right : candidate.Bottom;

            if (firstEnd < secondStart)
                return secondStart - firstEnd;
            
            if (secondEnd < firstStart)
                return firstStart - secondEnd;
            
            return 0.0;
        }

        private static Double GetPerpendicularCenterDistance(NavigationRectangle origin, NavigationRectangle candidate, NavigationDirection direction)
        {
            return direction == NavigationDirection.Up || direction == NavigationDirection.Down
                ? Math.Abs(origin.CenterX - candidate.CenterX)
                : Math.Abs(origin.CenterY - candidate.CenterY);
        }
    }
}
