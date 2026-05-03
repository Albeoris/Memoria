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

    using InvalidOperationException = System.InvalidOperationException;
    using StringBuilder = System.Text.StringBuilder;

    /** <summary>
     *  The most common stream of tokens is one where every token is buffered up
     *  and tokens are prefiltered for a certain channel (the parser will only
     *  see these tokens and cannot change the filter channel number during the
     *  parse).
     *  </summary>
     *
     *  <remarks>TODO: how to access the full token stream?  How to track all tokens matched per rule?</remarks>
     */
    [System.Serializable]
    public class CommonTokenStream : BufferedTokenStream
    {
        /** Skip tokens on any channel but this one; this is how we skip whitespace... */
        private int _channel;

        public CommonTokenStream()
        {
        }

        public CommonTokenStream(ITokenSource tokenSource)
            : this(tokenSource, TokenChannels.Default)
        {
        }

        public CommonTokenStream(ITokenSource tokenSource, int channel)
            : base(tokenSource)
        {
            this._channel = channel;
        }

        public int Channel
        {
            get
            {
                return _channel;
            }
        }

        /** Reset this token stream by setting its token source. */
        public override ITokenSource TokenSource
        {
            get
            {
                return base.TokenSource;
            }
            set
            {
                base.TokenSource = value;
                _channel = TokenChannels.Default;
            }
        }

        /** Always leave p on an on-channel token. */
        public override void Consume()
        {
            if (_p == -1)
                Setup();
            _p++;
            _p = SkipOffTokenChannels(_p);
        }

        protected override IToken LB(int k)
        {
            if (k == 0 || (_p - k) < 0)
                return null;

            int i = _p;
            int n = 1;
            // find k good tokens looking backwards
            while (n <= k)
            {
                // skip off-channel tokens
                i = SkipOffTokenChannelsReverse(i - 1);
                n++;
            }
            if (i < 0)
                return null;
            return _tokens[i];
        }

        public override IToken LT(int k)
        {
            if (_p == -1)
                Setup();
            if (k == 0)
                return null;
            if (k < 0)
                return LB(-k);
            int i = _p;
            int n = 1; // we know tokens[p] is a good one
            // find k good tokens
            while (n < k)
            {
                // skip off-channel tokens
                i = SkipOffTokenChannels(i + 1);
                n++;
            }

            if (i > Range)
                Range = i;

            return _tokens[i];
        }

        /** Given a starting index, return the index of the first on-channel
         *  token.
         */
        protected virtual int SkipOffTokenChannels(int i)
        {
            Sync(i);
            while (_tokens[i].Channel != _channel)
            {
                // also stops at EOF (it's on channel)
                i++;
                Sync(i);
            }
            return i;
        }

        protected virtual int SkipOffTokenChannelsReverse(int i)
        {
            while (i >= 0 && ((IToken)_tokens[i]).Channel != _channel)
            {
                i--;
            }

            return i;
        }

        public override void Reset()
        {
            base.Reset();
            _p = SkipOffTokenChannels(0);
        }

        protected override void Setup()
        {
            _p = 0;
            _p = SkipOffTokenChannels(_p);
        }
    }
}
