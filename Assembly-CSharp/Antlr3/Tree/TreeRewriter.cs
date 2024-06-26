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

#if !PORTABLE
    using Console = System.Console;
#endif

    public class TreeRewriter : TreeParser
    {
        protected bool showTransformations;

        protected ITokenStream originalTokenStream;
        protected ITreeAdaptor originalAdaptor;

        Func<IAstRuleReturnScope> topdown_func;
        Func<IAstRuleReturnScope> bottomup_func;

        public TreeRewriter( ITreeNodeStream input )
            : this( input, new RecognizerSharedState() )
        {
        }
        public TreeRewriter( ITreeNodeStream input, RecognizerSharedState state )
            : base( input, state )
        {
            originalAdaptor = input.TreeAdaptor;
            originalTokenStream = input.TokenStream;
            topdown_func = () => Topdown();
            bottomup_func = () => Bottomup();
        }

        public virtual object ApplyOnce( object t, Func<IAstRuleReturnScope> whichRule )
        {
            if ( t == null )
                return null;

            try
            {
                // share TreeParser object but not parsing-related state
                SetState(new RecognizerSharedState());
                SetTreeNodeStream(new CommonTreeNodeStream(originalAdaptor, t));
                ( (CommonTreeNodeStream)input ).TokenStream = originalTokenStream;
                BacktrackingLevel = 1;
                IAstRuleReturnScope r = whichRule();
                BacktrackingLevel = 0;
                if ( Failed )
                    return t;

                if (showTransformations && r != null && !t.Equals(r.Tree) && r.Tree != null)
                    ReportTransformation(t, r.Tree);

                if ( r != null && r.Tree != null )
                    return r.Tree;
                else
                    return t;
            }
            catch ( RecognitionException )
            {
            }

            return t;
        }

        public virtual object ApplyRepeatedly( object t, Func<IAstRuleReturnScope> whichRule )
        {
            bool treeChanged = true;
            while ( treeChanged )
            {
                object u = ApplyOnce( t, whichRule );
                treeChanged = !t.Equals( u );
                t = u;
            }
            return t;
        }

        public virtual object Downup( object t )
        {
            return Downup( t, false );
        }

        public virtual object Downup( object t, bool showTransformations )
        {
            this.showTransformations = showTransformations;
            TreeVisitor v = new TreeVisitor( new CommonTreeAdaptor() );
            t = v.Visit( t, ( o ) => ApplyOnce( o, topdown_func ), ( o ) => ApplyRepeatedly( o, bottomup_func ) );
            return t;
        }

        // methods the downup strategy uses to do the up and down rules.
        // to override, just define tree grammar rule topdown and turn on
        // filter=true.
        protected virtual IAstRuleReturnScope Topdown()
        {
            return null;
        }

        protected virtual IAstRuleReturnScope Bottomup()
        {
            return null;
        }

        /** Override this if you need transformation tracing to go somewhere
         *  other than stdout or if you're not using ITree-derived trees.
         */
        protected virtual void ReportTransformation(object oldTree, object newTree)
        {
            ITree old = oldTree as ITree;
            ITree @new = newTree as ITree;
            string oldMessage = old != null ? old.ToStringTree() : "??";
            string newMessage = @new != null ? @new.ToStringTree() : "??";
#if !PORTABLE
            Console.WriteLine("{0} -> {1}", oldMessage, newMessage);
#endif
        }
    }
}
