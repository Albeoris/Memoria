using System;
using System.ComponentModel;
using System.Globalization;

namespace Memoria.Prime.PsdFile
{
    /// <devdoc>
    ///    <para>
    ///       Stores the location and size of a rectangular region. For
    ///       more advanced region functions use a <see cref='System.Drawing.Region'/> 
    ///       object.
    ///    </para> 
    /// </devdoc> 
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public struct Rectangle
    {

        /// <devdoc> 
        ///    <para>
        ///       Stores the location and size of a rectangular region. For 
        ///       more advanced region functions use a <see cref='System.Drawing.Region'/>
        ///       object.
        ///    </para>
        /// </devdoc> 
        public static readonly Rectangle Empty = new Rectangle();

        private Int32 _x;
        private Int32 _y;
        private Int32 _width;
        private Int32 _height;

        /// <devdoc> 
        ///    <para>
        ///       Initializes a new instance of the <see cref='Rectangle'/> 
        ///       class with the specified location and size. 
        ///    </para>
        /// </devdoc> 
        public Rectangle(Int32 x, Int32 y, Int32 width, Int32 height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        /// <devdoc> 
        ///    <para>
        ///       Initializes a new instance of the Rectangle class with the specified location
        ///       and size.
        ///    </para> 
        /// </devdoc>
        public Rectangle(Point location, Size size)
        {
            _x = location.X;
            _y = location.Y;
            _width = size.Width;
            _height = size.Height;
        }

        /// <devdoc>
        ///    Creates a new <see cref='Rectangle'/> with 
        ///    the specified location and size. 
        /// </devdoc>
        // !! Not in C++ version 
        public static Rectangle FromLtrb(Int32 left, Int32 top, Int32 right, Int32 bottom)
        {
            return new Rectangle(left,
                top,
                right - left,
                bottom - top);
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the coordinates of the
        ///       upper-left corner of the rectangular region represented by this <see cref='Rectangle'/>. 
        ///    </para>
        /// </devdoc> 
        [Browsable(false)]
        public Point Location
        {
            get { return new Point(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <devdoc>
        ///    Gets or sets the size of this <see cref='Rectangle'/>. 
        /// </devdoc>
        [Browsable(false)]
        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <devdoc>
        ///    Gets or sets the x-coordinate of the
        ///    upper-left corner of the rectangular region defined by this <see cref='Rectangle'/>. 
        /// </devdoc>
        public Int32 X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <devdoc> 
        ///    Gets or sets the y-coordinate of the
        ///    upper-left corner of the rectangular region defined by this <see cref='Rectangle'/>. 
        /// </devdoc>
        public Int32 Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <devdoc>
        ///    Gets or sets the width of the rectangular 
        ///    region defined by this <see cref='Rectangle'/>.
        /// </devdoc> 
        public Int32 Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <devdoc>
        ///    Gets or sets the width of the rectangular 
        ///    region defined by this <see cref='Rectangle'/>.
        /// </devdoc>
        public Int32 Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <devdoc> 
        ///    <para>
        ///       Gets the x-coordinate of the upper-left corner of the 
        ///       rectangular region defined by this <see cref='Rectangle'/> . 
        ///    </para>
        /// </devdoc> 
        [Browsable(false)]
        public Int32 Left => X;

        /// <devdoc>
        ///    <para> 
        ///       Gets the y-coordinate of the upper-left corner of the
        ///       rectangular region defined by this <see cref='Rectangle'/>.
        ///    </para>
        /// </devdoc> 
        [Browsable(false)]
        public Int32 Top => Y;

        /// <devdoc> 
        ///    <para>
        ///       Gets the x-coordinate of the lower-right corner of the 
        ///       rectangular region defined by this <see cref='Rectangle'/>. 
        ///    </para>
        /// </devdoc> 
        [Browsable(false)]
        public Int32 Right => X + Width;

        /// <devdoc>
        ///    <para>
        ///       Gets the y-coordinate of the lower-right corner of the
        ///       rectangular region defined by this <see cref='Rectangle'/>. 
        ///    </para>
        /// </devdoc> 
        [Browsable(false)]
        public Int32 Bottom => Y + Height;

        /// <devdoc> 
        ///    <para>
        ///       Tests whether this <see cref='Rectangle'/> has a <see cref='Width'/> 
        ///       or a <see cref='Height'/> of 0.
        ///    </para>
        /// </devdoc>
        [Browsable(false)]
        public Boolean IsEmpty => _height == 0 && _width == 0 && _x == 0 && _y == 0;

        /// <devdoc>
        ///    <para> 
        ///       Tests whether <paramref name="obj"/> is a <see cref='Rectangle'/> with 
        ///       the same location and size of this Rectangle.
        ///    </para> 
        /// </devdoc>
        public override Boolean Equals(Object obj)
        {
            if (!(obj is Rectangle))
                return false;

            Rectangle comp = (Rectangle)obj;

            return (comp.X == X) &&
                   (comp.Y == Y) &&
                   (comp.Width == Width) &&
                   (comp.Height == Height);
        }

        /// <devdoc> 
        ///    <para> 
        ///       Tests whether two <see cref='Rectangle'/>
        ///       objects have equal location and size. 
        ///    </para>
        /// </devdoc>
        public static Boolean operator ==(Rectangle left, Rectangle right)
        {
            return (left.X == right.X
                    && left.Y == right.Y
                    && left.Width == right.Width
                    && left.Height == right.Height);
        }

        /// <devdoc>
        ///    <para>
        ///       Tests whether two <see cref='Rectangle'/> 
        ///       objects differ in location or size.
        ///    </para> 
        /// </devdoc> 
        public static Boolean operator !=(Rectangle left, Rectangle right)
        {
            return !(left == right);
        }

        /// <devdoc>
        ///    <para> 
        ///       Determines if the specfied point is contained within the 
        ///       rectangular region defined by this <see cref='Rectangle'/> .
        ///    </para> 
        /// </devdoc>
        public Boolean Contains(Int32 x, Int32 y)
        {
            return X <= x &&
                   x < X + Width &&
                   Y <= y &&
                   y < Y + Height;
        }

        /// <devdoc>
        ///    <para>
        ///       Determines if the specfied point is contained within the
        ///       rectangular region defined by this <see cref='Rectangle'/> . 
        ///    </para>
        /// </devdoc> 
        public Boolean Contains(Point pt)
        {
            return Contains(pt.X, pt.Y);
        }

        /// <devdoc>
        ///    <para> 
        ///       Determines if the rectangular region represented by
        ///    <paramref name="rect"/> is entirely contained within the rectangular region represented by 
        ///       this <see cref='Rectangle'/> . 
        ///    </para>
        /// </devdoc> 
        public Boolean Contains(Rectangle rect)
        {
            return (X <= rect.X) &&
                   ((rect.X + rect.Width) <= (X + Width)) &&
                   (Y <= rect.Y) &&
                   ((rect.Y + rect.Height) <= (Y + Height));
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override Int32 GetHashCode()
        {
            return (Int32)((UInt32)X ^
                           (((UInt32)Y << 13) | ((UInt32)Y >> 19)) ^
                           (((UInt32)Width << 26) | ((UInt32)Width >> 6)) ^
                           (((UInt32)Height << 7) | ((UInt32)Height >> 25)));
        }

        /// <devdoc>
        ///    <para> 
        ///       Inflates this <see cref='Rectangle'/>
        ///       by the specified amount. 
        ///    </para> 
        /// </devdoc>
        public void Inflate(Int32 width, Int32 height)
        {
            X -= width;
            Y -= height;
            Width += 2 * width;
            Height += 2 * height;
        }

        /// <devdoc> 
        ///    Inflates this <see cref='Rectangle'/> by the specified amount.
        /// </devdoc>
        public void Inflate(Size size)
        {

            Inflate(size.Width, size.Height);
        }

        /// <devdoc> 
        ///    <para>
        ///       Creates a <see cref='Rectangle'/>
        ///       that is inflated by the specified amount.
        ///    </para> 
        /// </devdoc>
        // !! Not in C++ 
        public static Rectangle Inflate(Rectangle rect, Int32 x, Int32 y)
        {
            Rectangle r = rect;
            r.Inflate(x, y);
            return r;
        }

        /// <devdoc> Creates a Rectangle that represents the intersection between this Rectangle and rect.
        /// </devdoc> 
        public void Intersect(Rectangle rect)
        {
            Rectangle result = Intersect(rect, this);

            X = result.X;
            Y = result.Y;
            Width = result.Width;
            Height = result.Height;
        }

        /// <devdoc> 
        ///    Creates a rectangle that represents the intersetion between a and
        ///    b. If there is no intersection, null is returned. 
        /// </devdoc>
        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            Int32 x1 = Math.Max(a.X, b.X);
            Int32 x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            Int32 y1 = Math.Max(a.Y, b.Y);
            Int32 y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1
                && y2 >= y1)
            {

                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            }
            return Empty;
        }

        /// <devdoc>
        ///     Determines if this rectangle intersets with rect. 
        /// </devdoc>
        public Boolean IntersectsWith(Rectangle rect)
        {
            return (rect.X < X + Width) &&
                   (X < (rect.X + rect.Width)) &&
                   (rect.Y < Y + Height) &&
                   (Y < rect.Y + rect.Height);
        }

        /// <devdoc>
        ///    <para>
        ///       Creates a rectangle that represents the union between a and
        ///       b. 
        ///    </para>
        /// </devdoc> 
        public static Rectangle Union(Rectangle a, Rectangle b)
        {
            Int32 x1 = Math.Min(a.X, b.X);
            Int32 x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            Int32 y1 = Math.Min(a.Y, b.Y);
            Int32 y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        /// <devdoc>
        ///    <para> 
        ///       Adjusts the location of this rectangle by the specified amount.
        ///    </para>
        /// </devdoc>
        public void Offset(Point pos)
        {
            Offset(pos.X, pos.Y);
        }

        /// <devdoc> 
        ///    Adjusts the location of this rectangle by the specified amount.
        /// </devdoc>
        public void Offset(Int32 x, Int32 y)
        {
            X += x;
            Y += y;
        }

        /// <devdoc>
        ///    <para>
        ///       Converts the attributes of this <see cref='Rectangle'/> to a
        ///       human readable string. 
        ///    </para>
        /// </devdoc> 
        public override String ToString()
        {
            return "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) +
                   ",Width=" + Width.ToString(CultureInfo.CurrentCulture) +
                   ",Height=" + Height.ToString(CultureInfo.CurrentCulture) + "}";
        }
    }
}