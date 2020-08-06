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

namespace Antlr.Runtime.Tree
{
    using Antlr.Runtime.Misc;

    public class TreeFilter : TreeParser
    {
        protected ITokenStream originalTokenStream;
        protected ITreeAdaptor originalAdaptor;

        public TreeFilter( ITreeNodeStream input )
            : this( input, new RecognizerSharedState() )
        {
        }
        public TreeFilter( ITreeNodeStream input, RecognizerSharedState state )
            : base( input, state )
        {
            originalAdaptor = input.TreeAdaptor;
            originalTokenStream = input.TokenStream;
        }

        public virtual void ApplyOnce( object t, Action whichRule )
        {
            if ( t == null )
                return;

            try
            {
                // share TreeParser object but not parsing-related state
                SetState(new RecognizerSharedState());
                SetTreeNodeStream(new CommonTreeNodeStream(originalAdaptor, t));
                ( (CommonTreeNodeStream)input ).TokenStream = originalTokenStream;
                BacktrackingLevel = 1;
                whichRule();
                BacktrackingLevel = 0;
            }
            catch ( RecognitionException )
            {
            }
        }

        public virtual void Downup( object t )
        {
            TreeVisitor v = new TreeVisitor( new CommonTreeAdaptor() );
            Func<object, object> pre = ( o ) =>
            {
                ApplyOnce( o, Topdown );
                return o;
            };
            Func<object, object> post = ( o ) =>
            {
                ApplyOnce( o, Bottomup );
                return o;
            };
            v.Visit( t, pre, post );
        }

        // methods the downup strategy uses to do the up and down rules.
        // to override, just define tree grammar rule topdown and turn on
        // filter=true.
        protected virtual void Topdown()
        {
        }
        protected virtual void Bottomup()
        {
        }
    }
}
