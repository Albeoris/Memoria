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
    using System.Collections.Generic;

    using ArgumentNullException = System.ArgumentNullException;
    using Exception = System.Exception;
    using IDictionary = System.Collections.IDictionary;
    using NotSupportedException = System.NotSupportedException;

    /** <summary>A TreeAdaptor that works with any Tree implementation.</summary> */
    public abstract class BaseTreeAdaptor : ITreeAdaptor
    {
        /** <summary>
         *  System.identityHashCode() is not always unique; we have to
         *  track ourselves.  That's ok, it's only for debugging, though it's
         *  expensive: we have to create a hashtable with all tree nodes in it.
         *  </summary>
         */
        protected IDictionary<object, int> treeToUniqueIDMap;
        protected int uniqueNodeID = 1;

        public virtual object Nil()
        {
            return Create( null );
        }

        /** <summary>
         *  Create tree node that holds the start and stop tokens associated
         *  with an error.
         *  </summary>
         *
         *  <remarks>
         *  If you specify your own kind of tree nodes, you will likely have to
         *  override this method. CommonTree returns Token.INVALID_TOKEN_TYPE
         *  if no token payload but you might have to set token type for diff
         *  node type.
         *
         *  You don't have to subclass CommonErrorNode; you will likely need to
         *  subclass your own tree node class to avoid class cast exception.
         *  </remarks>
         */
        public virtual object ErrorNode( ITokenStream input, IToken start, IToken stop,
                                RecognitionException e )
        {
            CommonErrorNode t = new CommonErrorNode( input, start, stop, e );
            //System.out.println("returning error node '"+t+"' @index="+input.index());
            return t;
        }

        public virtual bool IsNil( object tree )
        {
            return ( (ITree)tree ).IsNil;
        }

        public virtual object DupNode(int type, object treeNode)
        {
            object t = DupNode(treeNode);
            SetType(t, type);
            return t;
        }

        public virtual object DupNode(object treeNode, string text)
        {
            object t = DupNode(treeNode);
            SetText(t, text);
            return t;
        }

        public virtual object DupNode(int type, object treeNode, string text)
        {
            object t = DupNode(treeNode);
            SetType(t, type);
            SetText(t, text);
            return t;
        }

        public virtual object DupTree( object tree )
        {
            return DupTree( tree, null );
        }

        /** <summary>
         *  This is generic in the sense that it will work with any kind of
         *  tree (not just ITree interface).  It invokes the adaptor routines
         *  not the tree node routines to do the construction.  
         *  </summary>
         */
        public virtual object DupTree( object t, object parent )
        {
            if ( t == null )
            {
                return null;
            }
            object newTree = DupNode( t );
            // ensure new subtree root has parent/child index set
            SetChildIndex( newTree, GetChildIndex( t ) ); // same index in new tree
            SetParent( newTree, parent );
            int n = GetChildCount( t );
            for ( int i = 0; i < n; i++ )
            {
                object child = GetChild( t, i );
                object newSubTree = DupTree( child, t );
                AddChild( newTree, newSubTree );
            }
            return newTree;
        }

        /** <summary>
         *  Add a child to the tree t.  If child is a flat tree (a list), make all
         *  in list children of t.  Warning: if t has no children, but child does
         *  and child isNil then you can decide it is ok to move children to t via
         *  t.children = child.children; i.e., without copying the array.  Just
         *  make sure that this is consistent with have the user will build
         *  ASTs.
         *  </summary>
         */
        public virtual void AddChild( object t, object child )
        {
            if ( t != null && child != null )
            {
                ( (ITree)t ).AddChild( (ITree)child );
            }
        }

        /** <summary>
         *  If oldRoot is a nil root, just copy or move the children to newRoot.
         *  If not a nil root, make oldRoot a child of newRoot.
         *  </summary>
         *
         *  <remarks>
         *    old=^(nil a b c), new=r yields ^(r a b c)
         *    old=^(a b c), new=r yields ^(r ^(a b c))
         *
         *  If newRoot is a nil-rooted single child tree, use the single
         *  child as the new root node.
         *
         *    old=^(nil a b c), new=^(nil r) yields ^(r a b c)
         *    old=^(a b c), new=^(nil r) yields ^(r ^(a b c))
         *
         *  If oldRoot was null, it's ok, just return newRoot (even if isNil).
         *
         *    old=null, new=r yields r
         *    old=null, new=^(nil r) yields ^(nil r)
         *
         *  Return newRoot.  Throw an exception if newRoot is not a
         *  simple node or nil root with a single child node--it must be a root
         *  node.  If newRoot is ^(nil x) return x as newRoot.
         *
         *  Be advised that it's ok for newRoot to point at oldRoot's
         *  children; i.e., you don't have to copy the list.  We are
         *  constructing these nodes so we should have this control for
         *  efficiency.
         *  </remarks>
         */
        public virtual object BecomeRoot( object newRoot, object oldRoot )
        {
            //System.out.println("becomeroot new "+newRoot.toString()+" old "+oldRoot);
            ITree newRootTree = (ITree)newRoot;
            ITree oldRootTree = (ITree)oldRoot;
            if ( oldRoot == null )
            {
                return newRoot;
            }
            // handle ^(nil real-node)
            if ( newRootTree.IsNil )
            {
                int nc = newRootTree.ChildCount;
                if ( nc == 1 )
                    newRootTree = (ITree)newRootTree.GetChild( 0 );
                else if ( nc > 1 )
                {
                    // TODO: make tree run time exceptions hierarchy
                    throw new Exception( "more than one node as root (TODO: make exception hierarchy)" );
                }
            }
            // add oldRoot to newRoot; addChild takes care of case where oldRoot
            // is a flat list (i.e., nil-rooted tree).  All children of oldRoot
            // are added to newRoot.
            newRootTree.AddChild( oldRootTree );
            return newRootTree;
        }

        /** <summary>Transform ^(nil x) to x and nil to null</summary> */
        public virtual object RulePostProcessing( object root )
        {
            //System.out.println("rulePostProcessing: "+((Tree)root).toStringTree());
            ITree r = (ITree)root;
            if ( r != null && r.IsNil )
            {
                if ( r.ChildCount == 0 )
                {
                    r = null;
                }
                else if ( r.ChildCount == 1 )
                {
                    r = (ITree)r.GetChild( 0 );
                    // whoever invokes rule will set parent and child index
                    r.Parent = null;
                    r.ChildIndex = -1;
                }
            }
            return r;
        }

        public virtual object BecomeRoot( IToken newRoot, object oldRoot )
        {
            return BecomeRoot( Create( newRoot ), oldRoot );
        }

        public virtual object Create( int tokenType, IToken fromToken )
        {
            fromToken = CreateToken( fromToken );
            fromToken.Type = tokenType;
            object t = Create( fromToken );
            return t;
        }

        public virtual object Create( int tokenType, IToken fromToken, string text )
        {
            if ( fromToken == null )
                return Create( tokenType, text );

            fromToken = CreateToken( fromToken );
            fromToken.Type = tokenType;
            fromToken.Text = text;
            object result = Create(fromToken);
            return result;
        }

        public virtual object Create(IToken fromToken, string text)
        {
            if (fromToken == null)
                throw new ArgumentNullException("fromToken");

            fromToken = CreateToken(fromToken);
            fromToken.Text = text;
            object result = Create(fromToken);
            return result;
        }

        public virtual object Create( int tokenType, string text )
        {
            IToken fromToken = CreateToken( tokenType, text );
            object t = Create( fromToken );
            return t;
        }

        public virtual int GetType( object t )
        {
            ITree tree = GetTree(t);
            if (tree == null)
                return TokenTypes.Invalid;

            return tree.Type;
        }

        public virtual void SetType( object t, int type )
        {
            throw new NotSupportedException( "don't know enough about Tree node" );
        }

        public virtual string GetText( object t )
        {
            ITree tree = GetTree(t);
            if (tree == null)
                return null;

            return tree.Text;
        }

        public virtual void SetText( object t, string text )
        {
            throw new NotSupportedException( "don't know enough about Tree node" );
        }

        public virtual object GetChild( object t, int i )
        {
            ITree tree = GetTree(t);
            if (tree == null)
                return null;

            return tree.GetChild(i);
        }

        public virtual void SetChild( object t, int i, object child )
        {
            ITree tree = GetTree(t);
            if (tree == null)
                return;

            ITree childTree = GetTree(child);
            tree.SetChild(i, childTree);
        }

        public virtual object DeleteChild( object t, int i )
        {
            return ( (ITree)t ).DeleteChild( i );
        }

        public virtual int GetChildCount( object t )
        {
            ITree tree = GetTree(t);
            if (tree == null)
                return 0;

            return tree.ChildCount;
        }

        public virtual int GetUniqueID( object node )
        {
            if ( treeToUniqueIDMap == null )
            {
                treeToUniqueIDMap = new Dictionary<object, int>();
            }
            int id;
            if ( treeToUniqueIDMap.TryGetValue( node, out id ) )
                return id;

            id = uniqueNodeID;
            treeToUniqueIDMap[node] = id;
            uniqueNodeID++;
            return id;
            // GC makes these nonunique:
            // return System.identityHashCode(node);
        }

        /** <summary>
         *  Tell me how to create a token for use with imaginary token nodes.
         *  For example, there is probably no input symbol associated with imaginary
         *  token DECL, but you need to create it as a payload or whatever for
         *  the DECL node as in ^(DECL type ID).
         *  </summary>
         *
         *  <remarks>
         *  If you care what the token payload objects' type is, you should
         *  override this method and any other createToken variant.
         *  </remarks>
         */
        public abstract IToken CreateToken( int tokenType, string text );

        /** <summary>
         *  Tell me how to create a token for use with imaginary token nodes.
         *  For example, there is probably no input symbol associated with imaginary
         *  token DECL, but you need to create it as a payload or whatever for
         *  the DECL node as in ^(DECL type ID).
         *  </summary>
         *
         *  <remarks>
         *  This is a variant of createToken where the new token is derived from
         *  an actual real input token.  Typically this is for converting '{'
         *  tokens to BLOCK etc...  You'll see
         *
         *    r : lc='{' ID+ '}' -> ^(BLOCK[$lc] ID+) ;
         *
         *  If you care what the token payload objects' type is, you should
         *  override this method and any other createToken variant.
         *  </remarks>
         */
        public abstract IToken CreateToken( IToken fromToken );

        public abstract object Create( IToken payload );

        /** <summary>
         *  Duplicate a node.  This is part of the factory;
         *  override if you want another kind of node to be built.
         *  </summary>
         *
         *  <remarks>
         *  I could use reflection to prevent having to override this
         *  but reflection is slow.
         *  </remarks>
         */
        public virtual object DupNode(object treeNode)
        {
            ITree tree = GetTree(treeNode);
            if (tree == null)
                return null;

            return tree.DupNode();
        }

        public abstract IToken GetToken( object t );

        /** <summary>
         *  Track start/stop token for subtree root created for a rule.
         *  Only works with Tree nodes.  For rules that match nothing,
         *  seems like this will yield start=i and stop=i-1 in a nil node.
         *  Might be useful info so I'll not force to be i..i.
         *  </summary>
         */
        public virtual void SetTokenBoundaries(object t, IToken startToken, IToken stopToken)
        {
            ITree tree = GetTree(t);
            if (tree == null)
                return;

            int start = 0;
            int stop = 0;

            if (startToken != null)
                start = startToken.TokenIndex;
            if (stopToken != null)
                stop = stopToken.TokenIndex;

            tree.TokenStartIndex = start;
            tree.TokenStopIndex = stop;
        }

        public virtual int GetTokenStartIndex(object t)
        {
            ITree tree = GetTree(t);
            if (tree == null)
                return -1;

            return tree.TokenStartIndex;
        }

        public virtual int GetTokenStopIndex(object t)
        {
            ITree tree = GetTree(t);
            if (tree == null)
                return -1;

            return tree.TokenStopIndex;
        }

        public virtual object GetParent(object t)
        {
            ITree tree = GetTree(t);
            if (tree == null)
                return null;

            return tree.Parent;
        }

        public virtual void SetParent(object t, object parent)
        {
            ITree tree = GetTree(t);
            if (tree == null)
                return;

            ITree parentTree = GetTree(parent);
            tree.Parent = parentTree;
        }

        public virtual int GetChildIndex(object t)
        {
            ITree tree = GetTree(t);
            if (tree == null)
                return 0;

            return tree.ChildIndex;
        }

        public virtual void SetChildIndex(object t, int index)
        {
            ITree tree = GetTree(t);
            if (tree == null)
                return;

            tree.ChildIndex = index;
        }

        public virtual void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t)
        {
            ITree tree = GetTree(parent);
            if (tree == null)
                return;

            tree.ReplaceChildren(startChildIndex, stopChildIndex, t);
        }

        protected virtual ITree GetTree(object t)
        {
            if (t == null)
                return null;

            ITree tree = t as ITree;
            if (tree == null)
                throw new NotSupportedException();

            return tree;
        }
    }
}
