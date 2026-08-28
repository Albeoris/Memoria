using System;

namespace Memoria.Launcher.Utils
{
    public readonly struct DisplayBounds : IEquatable<DisplayBounds>
    {
        public Int32 Left { get; }
        public Int32 Top { get; }
        public Int32 Width { get; }
        public Int32 Height { get; }
        public Int32 Right => Left + Width;
        public Int32 Bottom => Top + Height;

        public DisplayBounds(Int32 left, Int32 top, Int32 width, Int32 height)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public Boolean Equals(DisplayBounds other)
        {
            return Left == other.Left
                && Top == other.Top
                && Width == other.Width
                && Height == other.Height;
        }

        public override Boolean Equals(Object obj)
        {
            return obj is DisplayBounds other && Equals(other);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = Left;
                hashCode = (hashCode * 397) ^ Top;
                hashCode = (hashCode * 397) ^ Width;
                return (hashCode * 397) ^ Height;
            }
        }

        public static Boolean operator ==(DisplayBounds left, DisplayBounds right) => left.Equals(right);
        public static Boolean operator !=(DisplayBounds left, DisplayBounds right) => !left.Equals(right);
    }
}
