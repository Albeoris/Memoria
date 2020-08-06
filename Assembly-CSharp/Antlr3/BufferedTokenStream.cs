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
    using CLSCompliant = System.CLSCompliantAttribute;
    using IndexOutOfRangeException = System.IndexOutOfRangeException;
    using StringBuilder = System.Text.StringBuilder;

    /** Buffer all input tokens but do on-demand fetching of new tokens from
     *  lexer. Useful when the parser or lexer has to set context/mode info before
     *  proper lexing of future tokens. The ST template parser needs this,
     *  for example, because it has to constantly flip back and forth between
     *  inside/output templates. E.g., <c>&lt;names:{hi, &lt;it&gt;}&gt;</c> has to parse names
     *  as part of an expression but "hi, &lt;it&gt;" as a nested template.
     *
     *  You can't use this stream if you pass whitespace or other off-channel
     *  tokens to the parser. The stream can't ignore off-channel tokens.
     *  (UnbufferedTokenStream is the same way.)
     *
     *  This is not a subclass of UnbufferedTokenStream because I don't want
     *  to confuse small moving window of tokens it uses for the full buffer.
     */
    [System.Serializable]
    public class BufferedTokenStream : ITokenStream, ITokenStreamInformation
    {
        private ITokenSource _tokenSource;

        /** Record every single token pulled from the source so we can reproduce
         *  chunks of it later.  The buffer in LookaheadStream overlaps sometimes
         *  as its moving window moves through the input.  This list captures
         *  everything so we can access complete input text.
         */
        [CLSCompliant(false)]
        protected List<IToken> _tokens = new List<IToken>(100);

        /** Track the last mark() call result value for use in rewind(). */
        private int _lastMarker;

        /** The index into the tokens list of the current token (next token
         *  to consume).  tokens[p] should be LT(1).  p=-1 indicates need
         *  to initialize with first token.  The ctor doesn't get a token.
         *  First call to LT(1) or whatever gets the first token and sets p=0;
         */
        [CLSCompliant(false)]
        protected int _p = -1;

        public BufferedTokenStream()
        {
        }

        public BufferedTokenStream(ITokenSource tokenSource)
        {
            this._tokenSource = tokenSource;
        }

        public virtual ITokenSource TokenSource
        {
            get
            {
                return _tokenSource;
            }
            set
            {
                this._tokenSource = value;
                _tokens.Clear();
                _p = -1;
            }
        }

        public virtual int Index
        {
            get
            {
                return _p;
            }
        }

        /// <summary>
        /// How deep have we gone?
        /// </summary>
        public virtual int Range
        {
            get;
            protected set;
        }

        public virtual int Count
        {
            get
            {
                return _tokens.Count;
            }
        }

        public virtual string SourceName
        {
            get
            {
                return _tokenSource.SourceName;
            }
        }

        public virtual IToken LastToken
        {
            get
            {
                return LB(1);
            }
        }

        public virtual IToken LastRealToken
        {
            get
            {
                int i = 0;
                IToken token;
                do
                {
                    i++;
                    token = LB(i);
                } while (token != null && token.Line <= 0);

                return token;
            }
        }

        public virtual int MaxLookBehind
        {
            get
            {
                return int.MaxValue;
            }
        }

        public virtual int Mark()
        {
            if (_p == -1)
                Setup();
            _lastMarker = Index;
            return _lastMarker;
        }

        public virtual void Release(int marker)
        {
            // no resources to release
        }

        public virtual void Rewind(int marker)
        {
            Seek(marker);
        }

        public virtual void Rewind()
        {
            Seek(_lastMarker);
        }

        public virtual void Reset()
        {
            _p = 0;
            _lastMarker = 0;
        }

        public virtual void Seek(int index)
        {
            _p = index;
        }

        /** Move the input pointer to the next incoming token.  The stream
         *  must become active with LT(1) available.  consume() simply
         *  moves the input pointer so that LT(1) points at the next
         *  input symbol. Consume at least one token.
         *
         *  Walk past any token not on the channel the parser is listening to.
         */
        public virtual void Consume()
        {
            if (_p == -1)
                Setup();
            _p++;
            Sync(_p);
        }

        /** Make sure index i in tokens has a token. */
        protected virtual void Sync(int i)
        {
            int n = i - _tokens.Count + 1; // how many more elements we need?
            if (n > 0)
                Fetch(n);
        }

        /** add n elements to buffer */
        protected virtual void Fetch(int n)
        {
            for (int i = 0; i < n; i++)
            {
                IToken t = TokenSource.NextToken();
                t.TokenIndex = _tokens.Count;
                _tokens.Add(t);
                if (t.Type == CharStreamConstants.EndOfFile)
                    break;
            }
        }

        public virtual IToken Get(int i)
        {
            if (i < 0 || i >= _tokens.Count)
            {
                throw new IndexOutOfRangeException("token index " + i + " out of range 0.." + (_tokens.Count - 1));
            }
            return _tokens[i];
        }

#if false // why is this different from GetTokens(start, count) ?
        /// <summary>
        /// Get all tokens from start..(start+count-1) inclusively
        /// </summary>
        public virtual List<IToken> Get(int start, int count)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException("start");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            if (start + count >= _tokens.Count)
                throw new ArgumentException();

            if (_p == -1)
                Setup();

            List<IToken> subset = new List<IToken>(count);
            for (int i = 0; i < count; i++)
            {
                IToken token = _tokens[i];
                if (token.Type == TokenTypes.EndOfFile)
                    break;

                subset.Add(token);
            }

            return subset;
        }
#endif

        public virtual int LA(int i)
        {
            return LT(i).Type;
        }

        protected virtual IToken LB(int k)
        {
            if ((_p - k) < 0)
                return null;

            return _tokens[_p - k];
        }

        public virtual IToken LT(int k)
        {
            if (_p == -1)
                Setup();
            if (k == 0)
                return null;
            if (k < 0)
                return LB(-k);

            int i = _p + k - 1;
            Sync(i);
            if (i >= _tokens.Count)
            {
                // EOF must be last token
                return _tokens[_tokens.Count - 1];
            }

            if (i > Range)
                Range = i;

            return _tokens[_p + k - 1];
        }

        protected virtual void Setup()
        {
            Sync(0);
            _p = 0;
        }

        public virtual List<IToken> GetTokens()
        {
            return _tokens;
        }

        public virtual List<IToken> GetTokens(int start, int stop)
        {
            return GetTokens(start, stop, default(BitSet));
        }

        /** Given a start and stop index, return a List of all tokens in
         *  the token type BitSet.  Return null if no tokens were found.  This
         *  method looks at both on and off channel tokens.
         */
        public virtual List<IToken> GetTokens(int start, int stop, BitSet types)
        {
            if (_p == -1)
                Setup();
            if (stop >= _tokens.Count)
                stop = _tokens.Count - 1;
            if (start < 0)
                start = 0;
            if (start > stop)
                return null;

            // list = tokens[start:stop]:{Token t, t.getType() in types}
            List<IToken> filteredTokens = new List<IToken>();
            for (int i = start; i <= stop; i++)
            {
                IToken t = _tokens[i];
                if (types == null || types.Member(t.Type))
                {
                    filteredTokens.Add(t);
                }
            }
            if (filteredTokens.Count == 0)
            {
                filteredTokens = null;
            }
            return filteredTokens;
        }

        public virtual List<IToken> GetTokens(int start, int stop, IEnumerable<int> types)
        {
            return GetTokens(start, stop, new BitSet(types));
        }

        public virtual List<IToken> GetTokens(int start, int stop, int ttype)
        {
            return GetTokens(start, stop, BitSet.Of(ttype));
        }

        public override string ToString()
        {
            if (_p == -1)
                Setup();

            Fill();
            return ToString(0, _tokens.Count - 1);
        }

        public virtual string ToString(int start, int stop)
        {
            if (start < 0 || stop < 0)
                return null;
            if (_p == -1)
                Setup();
            if (stop >= _tokens.Count)
                stop = _tokens.Count - 1;

            StringBuilder buf = new StringBuilder();
            for (int i = start; i <= stop; i++)
            {
                IToken t = _tokens[i];
                if (t.Type == CharStreamConstants.EndOfFile)
                    break;
                buf.Append(t.Text);
            }

            return buf.ToString();
        }

        public virtual string ToString(IToken start, IToken stop)
        {
            if (start != null && stop != null)
            {
                return ToString(start.TokenIndex, stop.TokenIndex);
            }
            return null;
        }

        public virtual void Fill()
        {
            if (_p == -1)
                Setup();

            if (_tokens[_p].Type == CharStreamConstants.EndOfFile)
                return;

            int i = _p + 1;
            Sync(i);
            while (_tokens[i].Type != CharStreamConstants.EndOfFile)
            {
                i++;
                Sync(i);
            }
        }
    }
}
