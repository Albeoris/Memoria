/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
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
    using System.Collections.Generic;
    using Exception = System.Exception;

#if !PORTABLE
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
#endif

    /** <summary>
     *  We were expecting a token but it's not found.  The current token
     *  is actually what we wanted next.  Used for tree node errors too.
     *  </summary>
     */
    [System.Serializable]
    public class MissingTokenException : MismatchedTokenException
    {
        private readonly object _inserted;

        public MissingTokenException()
        {
        }

        public MissingTokenException(string message)
            : base(message)
        {
        }

        public MissingTokenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public MissingTokenException(int expecting, IIntStream input, object inserted)
            : this(expecting, input, inserted, null)
        {
        }

        public MissingTokenException(int expecting, IIntStream input, object inserted, IList<string> tokenNames)
            : base(expecting, input, tokenNames)
        {
            this._inserted = inserted;
        }

        public MissingTokenException(string message, int expecting, IIntStream input, object inserted, IList<string> tokenNames)
            : base(message, expecting, input, tokenNames)
        {
            this._inserted = inserted;
        }

        public MissingTokenException(string message, int expecting, IIntStream input, object inserted, IList<string> tokenNames, Exception innerException)
            : base(message, expecting, input, tokenNames, innerException)
        {
            this._inserted = inserted;
        }

#if !PORTABLE
        protected MissingTokenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        public virtual int MissingType
        {
            get
            {
                return Expecting;
            }
        }

        public override string ToString()
        {
            if (_inserted != null && Token != null)
            {
                return "MissingTokenException(inserted " + _inserted + " at " + Token.Text + ")";
            }
            if (Token != null)
            {
                return "MissingTokenException(at " + Token.Text + ")";
            }
            return "MissingTokenException";
        }
    }
}
