using System;
using System.Runtime.Serialization;

namespace Memoria
{
    public class ArgumentEmptyException : ArgumentException
    {
        public ArgumentEmptyException()
        {
        }

        public ArgumentEmptyException(String message)
            : base(message)
        {
        }

        public ArgumentEmptyException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ArgumentEmptyException(String message, String paramName)
            : base(message, paramName)
        {
        }

        public ArgumentEmptyException(String message, String paramName, Exception innerException)
            : base(message, paramName, innerException)
        {
        }

        protected ArgumentEmptyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}