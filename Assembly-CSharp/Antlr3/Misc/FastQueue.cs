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
    using System.Collections.Generic;
    using ArgumentException = System.ArgumentException;
    using InvalidOperationException = System.InvalidOperationException;

    /** A queue that can dequeue and get(i) in O(1) and grow arbitrarily large.
     *  A linked list is fast at dequeue but slow at get(i).  An array is
     *  the reverse.  This is O(1) for both operations.
     *
     *  List grows until you dequeue last element at end of buffer. Then
     *  it resets to start filling at 0 again.  If adds/removes are balanced, the
     *  buffer will not grow too large.
     *
     *  No iterator stuff as that's not how we'll use it.
     */
    public class FastQueue<T>
    {
        /** <summary>dynamically-sized buffer of elements</summary> */
        internal List<T> _data = new List<T>();
        /** <summary>index of next element to fill</summary> */
        internal int _p = 0;

        public virtual int Count
        {
            get
            {
                return _data.Count - _p;
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

        /** <summary>
         * Return element {@code i} elements ahead of current element. {@code i==0}
         * gets current element. This is not an absolute index into {@link #data}
         * since {@code p} defines the start of the real list.
         *  </summary>
         */
        public virtual T this[int i]
        {
            get
            {
                int absIndex = _p + i;
                if (absIndex >= _data.Count)
                    throw new ArgumentException(string.Format("queue index {0} > last index {1}", absIndex, _data.Count - 1));
                if (absIndex < 0)
                    throw new ArgumentException(string.Format("queue index {0} < 0", absIndex));

                if (absIndex > Range)
                    Range = absIndex;

                return _data[absIndex];
            }
        }

        /** <summary>Get and remove first element in queue</summary> */
        public virtual T Dequeue()
        {
            if (Count == 0)
                throw new InvalidOperationException();

            T o = this[0];
            _p++;
            // have we hit end of buffer?
            if ( _p == _data.Count )
            {
                // if so, it's an opportunity to start filling at index 0 again
                Clear(); // size goes to 0, but retains memory
            }
            return o;
        }

        public virtual void Enqueue( T o )
        {
            _data.Add( o );
        }

        public virtual T Peek()
        {
            return this[0];
        }

        public virtual void Clear()
        {
            _p = 0;
            _data.Clear();
        }

        /** <summary>Return string of current buffer contents; non-destructive</summary> */
        public override string ToString()
        {
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            int n = Count;
            for ( int i = 0; i < n; i++ )
            {
                buf.Append( this[i] );
                if ( ( i + 1 ) < n )
                    buf.Append( " " );
            }
            return buf.ToString();
        }
    }
}
