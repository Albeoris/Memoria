/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Antlr.Runtime
{
    using ArgumentNullException = System.ArgumentNullException;
    using Exception = System.Exception;

#if !PORTABLE
    using SecurityCriticalAttribute = System.Security.SecurityCriticalAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
#endif

    [System.Serializable]
    public class MismatchedRangeException : RecognitionException
    {
        private readonly int _a;
        private readonly int _b;

        public MismatchedRangeException()
        {
        }

        public MismatchedRangeException(string message)
            : base(message)
        {
        }

        public MismatchedRangeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public MismatchedRangeException(int a, int b, IIntStream input)
            : base(input)
        {
            this._a = a;
            this._b = b;
        }

        public MismatchedRangeException(string message, int a, int b, IIntStream input)
            : base(message, input)
        {
            this._a = a;
            this._b = b;
        }

        public MismatchedRangeException(string message, int a, int b, IIntStream input, Exception innerException)
            : base(message, input, innerException)
        {
            this._a = a;
            this._b = b;
        }

#if !PORTABLE
        protected MismatchedRangeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            this._a = info.GetInt32("A");
            this._b = info.GetInt32("B");
        }
#endif

        public int A
        {
            get
            {
                return _a;
            }
        }

        public int B
        {
            get
            {
                return _b;
            }
        }

#if !PORTABLE
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue("A", _a);
            info.AddValue("B", _b);
        }
#endif

        public override string ToString()
        {
            return "MismatchedRangeException(" + UnexpectedType + " not in [" + A + "," + B + "])";
        }
    }
}
