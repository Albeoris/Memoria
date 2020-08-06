/*
 * [The "BSD licence"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Pixel Mine, Inc.
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
    using ArgumentNullException = System.ArgumentNullException;

    /** <summary>
     *  A tree node that is wrapper for a Token object.  After 3.0 release
     *  while building tree rewrite stuff, it became clear that computing
     *  parent and child index is very difficult and cumbersome.  Better to
     *  spend the space in every tree node.  If you don't want these extra
     *  fields, it's easy to cut them out in your own BaseTree subclass.
     *  </summary>
     */
    [System.Serializable]
    public class CommonTree : BaseTree
    {
        /** <summary>A single token is the payload</summary> */
        private IToken _token;

        /** <summary>
         *  What token indexes bracket all tokens associated with this node
         *  and below?
         *  </summary>
         */
        protected int startIndex = -1;
        protected int stopIndex = -1;

        /** <summary>Who is the parent node of this node; if null, implies node is root</summary> */
        CommonTree parent;

        /** <summary>What index is this node in the child list? Range: 0..n-1</summary> */
        int childIndex = -1;

        public CommonTree()
        {
        }

        public CommonTree( CommonTree node )
            : base( node )
        {
            if (node == null)
                throw new ArgumentNullException("node");

            this.Token = node.Token;
            this.startIndex = node.startIndex;
            this.stopIndex = node.stopIndex;
        }

        public CommonTree( IToken t )
        {
            this.Token = t;
        }

        #region Properties

        public override int CharPositionInLine
        {
            get
            {
                if ( Token == null || Token.CharPositionInLine == -1 )
                {
                    if ( ChildCount > 0 )
                        return Children[0].CharPositionInLine;

                    return 0;
                }
                return Token.CharPositionInLine;
            }

            set
            {
                base.CharPositionInLine = value;
            }
        }

        public override int ChildIndex
        {
            get
            {
                return childIndex;
            }

            set
            {
                childIndex = value;
            }
        }

        public override bool IsNil
        {
            get
            {
                return Token == null;
            }
        }

        public override int Line
        {
            get
            {
                if ( Token == null || Token.Line == 0 )
                {
                    if ( ChildCount > 0 )
                        return Children[0].Line;

                    return 0;
                }

                return Token.Line;
            }

            set
            {
                base.Line = value;
            }
        }

        public override ITree Parent
        {
            get
            {
                return parent;
            }

            set
            {
                parent = (CommonTree)value;
            }
        }

        public override string Text
        {
            get
            {
                if ( Token == null )
                    return null;

                return Token.Text;
            }

            set
            {
            }
        }

        public IToken Token
        {
            get
            {
                return _token;
            }

            set
            {
                _token = value;
            }
        }

        public override int TokenStartIndex
        {
            get
            {
                if ( startIndex == -1 && Token != null )
                    return Token.TokenIndex;

                return startIndex;
            }

            set
            {
                startIndex = value;
            }
        }

        public override int TokenStopIndex
        {
            get
            {
                if ( stopIndex == -1 && Token != null )
                {
                    return Token.TokenIndex;
                }
                return stopIndex;
            }

            set
            {
                stopIndex = value;
            }
        }

        public override int Type
        {
            get
            {
                if ( Token == null )
                    return TokenTypes.Invalid;

                return Token.Type;
            }

            set
            {
            }
        }

        #endregion

        public override ITree DupNode()
        {
            return new CommonTree( this );
        }

        /** <summary>
         *  For every node in this subtree, make sure it's start/stop token's
         *  are set.  Walk depth first, visit bottom up.  Only updates nodes
         *  with at least one token index &lt; 0.
         *  </summary>
         */
        public virtual void SetUnknownTokenBoundaries()
        {
            if ( Children == null )
            {
                if ( startIndex < 0 || stopIndex < 0 )
                    startIndex = stopIndex = Token.TokenIndex;

                return;
            }

            foreach (ITree childTree in Children)
            {
                CommonTree commonTree = childTree as CommonTree;
                if (commonTree == null)
                    continue;

                commonTree.SetUnknownTokenBoundaries();
            }

            if ( startIndex >= 0 && stopIndex >= 0 )
                return; // already set

            if ( Children.Count > 0 )
            {
                ITree firstChild = Children[0];
                ITree lastChild = Children[Children.Count - 1];
                startIndex = firstChild.TokenStartIndex;
                stopIndex = lastChild.TokenStopIndex;
            }
        }

        public override string ToString()
        {
            if (IsNil)
                return "nil";

            if (Type == TokenTypes.Invalid)
                return "<errornode>";

            if (Token == null)
                return string.Empty;

            return Token.Text;
        }
    }
}
