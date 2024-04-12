using System;
using System.ComponentModel;
using System.Globalization;

namespace Memoria.Prime.PsdFile
{
	/**
     * Represents a dimension in 2D coordinate space
     */

	/// <devdoc>
	///    Represents the size of a rectangular region
	///    with an ordered pair of width and height.
	/// </devdoc>
	[Serializable]
	public struct Size
	{
		/// <devdoc>
		///    Initializes a new instance of the <see cref='Size'/> class.
		/// </devdoc>
		public static readonly Size Empty = new Size();

		private Int32 _width;
		private Int32 _height;

		/**
         * Create a new Size object of the specified dimension
         */

		/// <devdoc>
		///    Initializes a new instance of the <see cref='Size'/> class from
		///    the specified dimensions.
		/// </devdoc>
		public Size(Int32 width, Int32 height)
		{
			_width = width;
			_height = height;
		}

		/// <devdoc>
		///    <para>
		///       Performs vector addition of two <see cref='Size'/> objects.
		///    </para>
		/// </devdoc>
		public static Size operator +(Size sz1, Size sz2)
		{
			return Add(sz1, sz2);
		}

		/// <devdoc>
		///    <para>
		///       Contracts a <see cref='Size'/> by another <see cref='Size'/>
		///       .
		///    </para>
		/// </devdoc>
		public static Size operator -(Size sz1, Size sz2)
		{
			return Subtract(sz1, sz2);
		}

		/// <devdoc>
		///    Tests whether two <see cref='Size'/> objects
		///    are identical.
		/// </devdoc>
		public static Boolean operator ==(Size sz1, Size sz2)
		{
			return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
		}

		/// <devdoc>
		///    <para>
		///       Tests whether two <see cref='Size'/> objects are different.
		///    </para>
		/// </devdoc>
		public static Boolean operator !=(Size sz1, Size sz2)
		{
			return !(sz1 == sz2);
		}

		/// <devdoc>
		///    Tests whether this <see cref='Size'/> has zero
		///    width and height.
		/// </devdoc>
		[Browsable(false)]
		public Boolean IsEmpty => _width == 0 && _height == 0;

		/**
         * Horizontal dimension
         */

		/// <devdoc>
		///    <para>
		///       Represents the horizontal component of this
		///    <see cref='Size'/>.
		///    </para>
		/// </devdoc>
		public Int32 Width
		{
			get { return _width; }
			set { _width = value; }
		}

		/**
         * Vertical dimension
         */

		/// <devdoc>
		///    Represents the vertical component of this
		/// <see cref='Size'/>.
		/// </devdoc>
		public Int32 Height
		{
			get { return _height; }
			set { _height = value; }
		}

		/// <devdoc>
		///    <para>
		///       Performs vector addition of two <see cref='Size'/> objects.
		///    </para>
		/// </devdoc>
		public static Size Add(Size sz1, Size sz2)
		{
			return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		/// <devdoc>
		///    <para>
		///       Contracts a <see cref='Size'/> by another <see cref='Size'/> .
		///    </para>
		/// </devdoc>
		public static Size Subtract(Size sz1, Size sz2)
		{
			return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		/// <devdoc>
		///    <para>
		///       Tests to see whether the specified object is a
		///    <see cref='Size'/>
		///    with the same dimensions as this <see cref='Size'/>.
		/// </para>
		/// </devdoc>
		public override Boolean Equals(Object obj)
		{
			if (!(obj is Size))
				return false;

			Size comp = (Size)obj;
			// Note value types can't have derived classes, so we don't need to
			// check the types of the objects here.  -- [....], 2/21/2001
			return (comp._width == _width) &&
				   (comp._height == _height);
		}

		/// <devdoc>
		///    <para>
		///       Returns a hash code.
		///    </para>
		/// </devdoc>
		public override Int32 GetHashCode()
		{
			return _width ^ _height;
		}

		/// <devdoc>
		///    <para>
		///       Creates a human-readable string that represents this
		///    <see cref='Size'/>.
		///    </para>
		/// </devdoc>
		public override String ToString()
		{
			return "{Width=" + _width.ToString(CultureInfo.CurrentCulture) + ", Height=" + _height.ToString(CultureInfo.CurrentCulture) + "}";
		}
	}
}
