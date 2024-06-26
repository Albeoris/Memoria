﻿/*
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

    /** <summary>A source of characters for an ANTLR lexer</summary> */
    public interface ICharStream : IIntStream
    {

        /** <summary>
         *  For infinite streams, you don't need this; primarily I'm providing
         *  a useful interface for action code.  Just make sure actions don't
         *  use this on streams that don't support it.
         *  </summary>
         */
        string Substring(int start, int length);

        /** <summary>
         *  Get the ith character of lookahead.  This is the same usually as
         *  LA(i).  This will be used for labels in the generated
         *  lexer code.  I'd prefer to return a char here type-wise, but it's
         *  probably better to be 32-bit clean and be consistent with LA.
         *  </summary>
         */
        int LT(int i);

        /** <summary>ANTLR tracks the line information automatically</summary> */
        /** <summary>Because this stream can rewind, we need to be able to reset the line</summary> */
        int Line
        {
            get;
            set;
        }

        /** <summary>The index of the character relative to the beginning of the line 0..n-1</summary> */
        int CharPositionInLine
        {
            get;
            set;
        }
    }
}
