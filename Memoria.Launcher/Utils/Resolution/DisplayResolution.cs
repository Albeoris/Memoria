using System;
using System.Globalization;

namespace Memoria.Launcher.Utils
{
    public readonly struct DisplayResolution : IComparable<DisplayResolution>, IEquatable<DisplayResolution>
    {
        public static DisplayResolution Empty { get; } = default;

        public Int32 Width { get; }
        public Int32 Height { get; }
        public Boolean IsValid => Width > 0 && Height > 0;

        public DisplayResolution(Int32 width, Int32 height)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
        }

        public static Boolean TryParse(String value, out DisplayResolution resolution)
        {
            resolution = Empty;
            if (String.IsNullOrWhiteSpace(value))
                return false;

            String dimensions = value.Split([' ', '|'], StringSplitOptions.RemoveEmptyEntries)[0];
            String[] components = dimensions.Split('x');
            if (components.Length != 2
                || !Int32.TryParse(components[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out Int32 width)
                || !Int32.TryParse(components[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out Int32 height)
                || width <= 0
                || height <= 0)
            {
                return false;
            }

            resolution = new DisplayResolution(width, height);
            return true;
        }

        public Int32 CompareTo(DisplayResolution other)
        {
            Int32 widthComparison = Width.CompareTo(other.Width);
            return widthComparison != 0 ? widthComparison : Height.CompareTo(other.Height);
        }

        public Boolean Equals(DisplayResolution other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override Boolean Equals(Object obj)
        {
            return obj is DisplayResolution other && Equals(other);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return (Width * 397) ^ Height;
            }
        }

        public override String ToString()
        {
            return $"{Width.ToString(CultureInfo.InvariantCulture)}x{Height.ToString(CultureInfo.InvariantCulture)}";
        }

        public static Boolean operator ==(DisplayResolution left, DisplayResolution right) => left.Equals(right);
        public static Boolean operator !=(DisplayResolution left, DisplayResolution right) => !left.Equals(right);
    }
}
