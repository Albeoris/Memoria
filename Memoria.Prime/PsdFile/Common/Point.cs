using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Memoria.Prime.PsdFile
{
    /// <devdoc>
    ///    Represents an ordered pair of x and y coordinates that
    ///    define a point in a two-dimensional plane. 
    /// </devdoc>
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
    public struct Point
    {

        /// <devdoc> 
        ///    Creates a new instance of the <see cref='Point'/> class 
        ///    with member data left uninitialized.
        /// </devdoc> 
        public static readonly Point Empty = new Point();

        private Int32 _x;
        private Int32 _y;

        /// <devdoc> 
        ///    Initializes a new instance of the <see cref='Point'/> class
        ///    with the specified coordinates. 
        /// </devdoc>
        public Point(Int32 x, Int32 y)
        {
            _x = x;
            _y = y;
        }

        /// <devdoc>
        ///    <para> 
        ///       Initializes a new instance of the <see cref='Point'/> class
        ///       from a <see cref='Size'/> .
        ///    </para>
        /// </devdoc> 
        public Point(Size sz)
        {
            _x = sz.Width;
            _y = sz.Height;
        }

        /// <devdoc>
        ///    Initializes a new instance of the Point class using
        ///    coordinates specified by an integer value. 
        /// </devdoc>
        public Point(Int32 dw)
        {
            _x = (Int16)Loword(dw);
            _y = (Int16)Hiword(dw);
        }

        /// <devdoc>
        ///    <para> 
        ///       Gets a value indicating whether this <see cref='Point'/> is empty.
        ///    </para> 
        /// </devdoc> 
        [Browsable(false)]
        public Boolean IsEmpty => _x == 0 && _y == 0;

        /// <devdoc> 
        ///    Gets the x-coordinate of this <see cref='Point'/>.
        /// </devdoc> 
        public Int32 X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the y-coordinate of this <see cref='Point'/>. 
        ///    </para>
        /// </devdoc> 
        public Int32 Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <devdoc>
        ///    <para>
        ///       Creates a <see cref='Size'/> with the coordinates of the specified <see cref='Point'/> . 
        ///    </para>
        /// </devdoc> 
        public static explicit operator Size(Point p)
        {
            return new Size(p.X, p.Y);
        }

        /// <devdoc>
        ///    <para> 
        ///       Translates a <see cref='Point'/> by a given <see cref='Size'/> .
        ///    </para> 
        /// </devdoc> 
        public static Point operator +(Point pt, Size sz)
        {
            return Add(pt, sz);
        }

        /// <devdoc> 
        ///    <para>
        ///       Translates a <see cref='Point'/> by the negative of a given <see cref='Size'/> . 
        ///    </para> 
        /// </devdoc>
        public static Point operator -(Point pt, Size sz)
        {
            return Subtract(pt, sz);
        }

        /// <devdoc>
        ///    <para> 
        ///       Compares two <see cref='Point'/> objects. The result specifies 
        ///       whether the values of the <see cref='X'/> and <see cref='Y'/> properties of the two <see cref='Point'/>
        ///       objects are equal. 
        ///    </para>
        /// </devdoc>
        public static Boolean operator ==(Point left, Point right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        /// <devdoc>
        ///    <para> 
        ///       Compares two <see cref='Point'/> objects. The result specifies whether the values
        ///       of the <see cref='X'/> or <see cref='Y'/> properties of the two
        ///    <see cref='Point'/>
        ///    objects are unequal. 
        /// </para>
        /// </devdoc> 
        public static Boolean operator !=(Point left, Point right)
        {
            return !(left == right);
        }

        /// <devdoc>
        ///    <para> 
        ///       Translates a <see cref='Point'/> by a given <see cref='Size'/> .
        ///    </para> 
        /// </devdoc> 
        public static Point Add(Point pt, Size sz)
        {
            return new Point(pt.X + sz.Width, pt.Y + sz.Height);
        }

        /// <devdoc>
        ///    <para> 
        ///       Translates a <see cref='Point'/> by the negative of a given <see cref='Size'/> .
        ///    </para> 
        /// </devdoc> 
        public static Point Subtract(Point pt, Size sz)
        {
            return new Point(pt.X - sz.Width, pt.Y - sz.Height);
        }

        /// <devdoc>
        ///    <para> 
        ///       Specifies whether this <see cref='Point'/> contains 
        ///       the same coordinates as the specified <see cref='System.Object'/>.
        ///    </para> 
        /// </devdoc>
        public override Boolean Equals(Object obj)
        {
            if (!(obj is Point)) return false;
            Point comp = (Point)obj;
            // Note value types can't have derived classes, so we don't need
            // to check the types of the objects here.  -- [....], 2/21/2001 
            return comp.X == X && comp.Y == Y;
        }

        /// <devdoc>
        ///    <para>
        ///       Returns a hash code. 
        ///    </para>
        /// </devdoc> 
        public override Int32 GetHashCode()
        {
            return _x ^ _y;
        }

        /**
         * Offset the current Point object by the given amount
         */

        /// <devdoc> 
        ///    Translates this <see cref='Point'/> by the specified amount. 
        /// </devdoc>
        public void Offset(Int32 dx, Int32 dy)
        {
            X += dx;
            Y += dy;
        }

        /// <devdoc> 
        ///    Translates this <see cref='Point'/> by the specified amount.
        /// </devdoc> 
        public void Offset(Point p)
        {
            Offset(p.X, p.Y);
        }

        /// <devdoc> 
        ///    <para> 
        ///       Converts this <see cref='Point'/>
        ///       to a human readable 
        ///       string.
        ///    </para>
        /// </devdoc>
        public override String ToString()
        {
            return "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) + "}";
        }

        private static Int32 Hiword(Int32 n)
        {
            return (n >> 16) & 0xffff;
        }

        private static Int32 Loword(Int32 n)
        {
            return n & 0xffff;
        }
    }
}