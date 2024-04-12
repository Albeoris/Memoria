using System;
using System.Collections;
using System.IO;

namespace Memoria.Prime.Exceptions
{
	public static class Exceptions
	{
		public static Exception CreateException(String message, params Object[] args)
		{
			return new Exception(String.Format(message, args));
		}

		public static Exception CreateArgumentException(String paramName, String message, params Object[] args)
		{
			return new ArgumentException(String.Format(message, args), paramName);
		}

		public static T CheckArgumentNull<T>(T arg, String name) where T : class
		{
			if (ReferenceEquals(arg, null))
				throw new ArgumentNullException(name);

			return arg;
		}

		public static String CheckArgumentNullOrEmprty(String arg, String name)
		{
			if (ReferenceEquals(arg, null))
				throw new ArgumentNullException(name);
			if (arg == String.Empty)
				throw new ArgumentEmptyException(name);

			return arg;
		}

		public static T CheckArgumentNullOrEmprty<T>(T arg, String name) where T : IList
		{
			if (ReferenceEquals(arg, null))
				throw new ArgumentNullException(name);
			if (arg.Count == 0)
				throw new ArgumentEmptyException(name);

			return arg;
		}

		public static String CheckFileNotFoundException(String fullName)
		{
			CheckArgumentNullOrEmprty(fullName, "fullName");
			if (!File.Exists(fullName))
				throw new FileNotFoundException(fullName);

			return fullName;
		}

		public static String CheckDirectoryNotFoundException(String fullName)
		{
			CheckArgumentNullOrEmprty(fullName, "fullName");
			if (!Directory.Exists(fullName))
				throw new DirectoryNotFoundException(fullName);

			return fullName;
		}

		public static T CheckArgumentOutOfRangeException<T>(T value, String name, T minValue, T maxValue) where T : IComparable<T>
		{
			if (value.CompareTo(minValue) < 0 || value.CompareTo(maxValue) > 0)
				throw new ArgumentOutOfRangeException(name, value, $"Значение аргумента ({name} = {value}) выходит за пределы допустимого диапазона: ({minValue}~{maxValue}).");
			return value;
		}

		public static T CheckReadableStream<T>(T stream, String name) where T : Stream
		{
			CheckArgumentNull(stream, name);

			if (!stream.CanRead)
			{
				if (!stream.CanWrite)
					throw new ObjectDisposedException("stream", "Stream closed.");
				throw new NotSupportedException("Unreadable stream.");
			}

			return stream;
		}

		public static T CheckWritableStream<T>(T stream, String name) where T : Stream
		{
			CheckArgumentNull(stream, name);

			if (!stream.CanWrite)
			{
				if (!stream.CanRead)
					throw new ObjectDisposedException("output", "Stream closed.");
				throw new NotSupportedException("Unwritable stream.");
			}

			return stream;
		}
	}
}
