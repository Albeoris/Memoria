/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2009 Sam Harwell
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
    using Antlr.Runtime.Misc;
    using CLSCompliant = System.CLSCompliantAttribute;
    using NotSupportedException = System.NotSupportedException;
    using IndexOutOfRangeException = System.IndexOutOfRangeException;

    /** A token stream that pulls tokens from the code source on-demand and
     *  without tracking a complete buffer of the tokens. This stream buffers
     *  the minimum number of tokens possible.  It's the same as
     *  OnDemandTokenStream except that OnDemandTokenStream buffers all tokens.
     *
     *  You can't use this stream if you pass whitespace or other off-channel
     *  tokens to the parser. The stream can't ignore off-channel tokens.
     * 
     *  You can only look backwards 1 token: LT(-1).
     *
     *  Use this when you need to read from a socket or other infinite stream.
     *
     *  @see BufferedTokenStream
     *  @see CommonTokenStream
     */
    public class UnbufferedTokenStream : LookaheadStream<IToken>, ITokenStream, ITokenStreamInformation
    {
        [CLSCompliant(false)]
        protected ITokenSource tokenSource;
        protected int tokenIndex; // simple counter to set token index in tokens

        /** Skip tokens on any channel but this one; this is how we skip whitespace... */
        protected int channel = TokenChannels.Default;

        private readonly ListStack<IToken> _realTokens = new ListStack<IToken>() { null };

        public UnbufferedTokenStream(ITokenSource tokenSource)
        {
            this.tokenSource = tokenSource;
        }

        public ITokenSource TokenSource
        {
            get
            {
                return this.tokenSource;
            }
        }

        public string SourceName
        {
            get
            {
                return TokenSource.SourceName;
            }
        }

        #region ITokenStreamInformation Members

        public IToken LastToken
        {
            get
            {
                return LB(1);
            }
        }

        public IToken LastRealToken
        {
            get
            {
                return _realTokens.Peek();
            }
        }

        public int MaxLookBehind
        {
            get
            {
                return 1;
            }
        }

        public override int Mark()
        {
            _realTokens.Push(_realTokens.Peek());
            return base.Mark();
        }

        public override void Release(int marker)
        {
            base.Release(marker);
            _realTokens.Pop();
        }

        public override void Clear()
        {
            _realTokens.Clear();
            _realTokens.Push(null);
        }

        public override void Consume()
        {
            base.Consume();
            if (PreviousElement != null && PreviousElement.Line > 0)
                _realTokens[_realTokens.Count - 1] = PreviousElement;
        }

        #endregion

        public override IToken NextElement()
        {
            IToken t = this.tokenSource.NextToken();
            t.TokenIndex = this.tokenIndex++;
            return t;
        }

        public override bool IsEndOfFile(IToken o)
        {
            return o.Type == CharStreamConstants.EndOfFile;
        }

        public IToken Get(int i)
        {
            throw new NotSupportedException("Absolute token indexes are meaningless in an unbuffered stream");
        }

        public int LA(int i)
        {
            return LT(i).Type;
        }

        public string ToString(int start, int stop)
        {
            return "n/a";
        }

        public string ToString(IToken start, IToken stop)
        {
            return "n/a";
        }
    }
}
