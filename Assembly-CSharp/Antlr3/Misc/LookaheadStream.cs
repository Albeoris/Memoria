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

namespace Antlr.Runtime.Misc
{
    using ArgumentException = System.ArgumentException;
    using Debug = System.Diagnostics.Debug;
    using InvalidOperationException = System.InvalidOperationException;
    using NotSupportedException = System.NotSupportedException;
    using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

    /** <summary>
     * A lookahead queue that knows how to mark/release locations in the buffer for
     * backtracking purposes. Any markers force the {@link FastQueue} superclass to
     * keep all elements until no more markers; then can reset to avoid growing a
     * huge buffer.
     *  </summary>
     */
    public abstract class LookaheadStream<T>
        : FastQueue<T>
        where T : class
    {
        /** Absolute token index. It's the index of the symbol about to be
         *  read via {@code LT(1)}. Goes from 0 to numtokens.
         */
        private int _currentElementIndex = 0;

        /**
         * This is the {@code LT(-1)} element for the first element in {@link #data}.
         */
        private T _previousElement;

        /** Track object returned by nextElement upon end of stream;
         *  Return it later when they ask for LT passed end of input.
         */
        T _eof = null;

        /** <summary>Track the last mark() call result value for use in rewind().</summary> */
        int _lastMarker;

        /** <summary>tracks how deep mark() calls are nested</summary> */
        int _markDepth;

        public T EndOfFile
        {
            get
            {
                return _eof;
            }
            protected set
            {
                _eof = value;
            }
        }

        public T PreviousElement
        {
            get
            {
                return _previousElement;
            }
        }

        public virtual void Reset()
        {
            Clear();
            _currentElementIndex = 0;
            _p = 0;
            _previousElement = null;
        }

        /** <summary>
         *  Implement nextElement to supply a stream of elements to this
         *  lookahead buffer.  Return EOF upon end of the stream we're pulling from.
         *  </summary>
         */
        public abstract T NextElement();

        public abstract bool IsEndOfFile(T o);

        /** <summary>
         * Get and remove first element in queue; override
         * {@link FastQueue#remove()}; it's the same, just checks for backtracking.
         * </summary> */
        public override T Dequeue()
        {
            T o = this[0];
            _p++;
            // have we hit end of buffer and not backtracking?
            if ( _p == _data.Count && _markDepth == 0 )
            {
                _previousElement = o;
                // if so, it's an opportunity to start filling at index 0 again
                Clear(); // size goes to 0, but retains memory
            }
            return o;
        }

        /** <summary>Make sure we have at least one element to remove, even if EOF</summary> */
        public virtual void Consume()
        {
            SyncAhead(1);
            Dequeue();
            _currentElementIndex++;
        }

        /** <summary>
         *  Make sure we have 'need' elements from current position p. Last valid
         *  p index is data.size()-1.  p+need-1 is the data index 'need' elements
         *  ahead.  If we need 1 element, (p+1-1)==p must be &lt; data.size().
         *  </summary>
         */
        protected virtual void SyncAhead( int need )
        {
            int n = ( _p + need - 1 ) - _data.Count + 1; // how many more elements we need?
            if ( n > 0 )
                Fill( n );                 // out of elements?
        }

        /** <summary>add n elements to buffer</summary> */
        public virtual void Fill( int n )
        {
            for ( int i = 0; i < n; i++ )
            {
                T o = NextElement();
                if ( IsEndOfFile(o) )
                    _eof = o;

                _data.Add( o );
            }
        }

        /** <summary>Size of entire stream is unknown; we only know buffer size from FastQueue</summary> */
        public override int Count
        {
            get
            {
                throw new System.NotSupportedException( "streams are of unknown size" );
            }
        }

        public virtual T LT( int k )
        {
            if ( k == 0 )
            {
                return null;
            }
            if ( k < 0 )
            {
                return LB(-k);
            }

            SyncAhead( k );
            if ((_p + k - 1) > _data.Count)
                return _eof;

            return this[k - 1];
        }

        public virtual int Index
        {
            get
            {
                return _currentElementIndex;
            }
        }

        public virtual int Mark()
        {
            _markDepth++;
            _lastMarker = _p; // track where we are in buffer, not absolute token index
            return _lastMarker;
        }

        public virtual void Release( int marker )
        {
            if (_markDepth == 0)
                throw new InvalidOperationException();

            _markDepth--;
        }

        public virtual void Rewind( int marker )
        {
            _markDepth--;
            int delta = _p - marker;
            _currentElementIndex -= delta;
            _p = marker;
        }

        public virtual void Rewind()
        {
            // rewind but do not release marker
            int delta = _p - _lastMarker;
            _currentElementIndex -= delta;
            _p = _lastMarker;
        }

        /** <summary>
         * Seek to a 0-indexed absolute token index. Normally used to seek backwards
         * in the buffer. Does not force loading of nodes.
         *  </summary>
         *  <remarks>
         * To preserve backward compatibility, this method allows seeking past the
         * end of the currently buffered data. In this case, the input pointer will
         * be moved but the data will only actually be loaded upon the next call to
         * {@link #consume} or {@link #LT} for {@code k>0}.
         *  </remarks>
         */
        public virtual void Seek( int index )
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            int delta = _currentElementIndex - index;
            if (_p - delta < 0)
                throw new NotSupportedException("can't seek before the beginning of this stream's buffer");

            _p -= delta;
            _currentElementIndex = index;
        }

        protected virtual T LB(int k)
        {
            Debug.Assert(k > 0);

            int index = _p - k;
            if (index == -1)
                return _previousElement;

            // if k>0 then we know index < data.size(). avoid the double-check for
            // performance.
            if (index >= 0 /*&& index < data.size()*/)
                return _data[index];

            if (index < -1)
                throw new NotSupportedException("can't look more than one token before the beginning of this stream's buffer");

            throw new NotSupportedException("can't look past the end of this stream's buffer using LB(int)");
        }
    }
}
