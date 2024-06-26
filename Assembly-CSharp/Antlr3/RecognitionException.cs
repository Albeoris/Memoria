/*
 * [The "BSD license"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Tunnel Vision Laboratories, LLC
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
    using Antlr.Runtime.Tree;

    using ArgumentNullException = System.ArgumentNullException;
    using Exception = System.Exception;
    using NotSupportedException = System.NotSupportedException;

#if !PORTABLE
    using SecurityCriticalAttribute = System.Security.SecurityCriticalAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
#endif

    /** <summary>The root of the ANTLR exception hierarchy.</summary>
     *
     *  <remarks>
     *  To avoid English-only error messages and to generally make things
     *  as flexible as possible, these exceptions are not created with strings,
     *  but rather the information necessary to generate an error.  Then
     *  the various reporting methods in Parser and Lexer can be overridden
     *  to generate a localized error message.  For example, MismatchedToken
     *  exceptions are built with the expected token type.
     *  So, don't expect getMessage() to return anything.
     *
     *  Note that as of Java 1.4, you can access the stack trace, which means
     *  that you can compute the complete trace of rules from the start symbol.
     *  This gives you considerable context information with which to generate
     *  useful error messages.
     *
     *  ANTLR generates code that throws exceptions upon recognition error and
     *  also generates code to catch these exceptions in each rule.  If you
     *  want to quit upon first error, you can turn off the automatic error
     *  handling mechanism using rulecatch action, but you still need to
     *  override methods mismatch and recoverFromMismatchSet.
     *
     *  In general, the recognition exceptions can track where in a grammar a
     *  problem occurred and/or what was the expected input.  While the parser
     *  knows its state (such as current input symbol and line info) that
     *  state can change before the exception is reported so current token index
     *  is computed and stored at exception time.  From this info, you can
     *  perhaps print an entire line of input not just a single token, for example.
     *  Better to just say the recognizer had a problem and then let the parser
     *  figure out a fancy report.
     *  </remarks>
     */
    [System.Serializable]
    public class RecognitionException : Exception
    {
        /** <summary>What input stream did the error occur in?</summary> */
        private IIntStream _input;

        /// <summary>
        /// What was the lookahead index when this exception was thrown?
        /// </summary>
        private int _k;

        /** <summary>What is index of token/char were we looking at when the error occurred?</summary> */
        private int _index;

        /** <summary>
         *  The current Token when an error occurred.  Since not all streams
         *  can retrieve the ith Token, we have to track the Token object.
         *  For parsers.  Even when it's a tree parser, token might be set.
         *  </summary>
         */
        private IToken _token;

        /** <summary>
         *  If this is a tree parser exception, node is set to the node with
         *  the problem.
         *  </summary>
         */
        private object _node;

        /** <summary>The current char when an error occurred. For lexers.</summary> */
        private int _c;

        /** <summary>
         *  Track the line (1-based) at which the error occurred in case this is
         *  generated from a lexer.  We need to track this since the
         *  unexpected char doesn't carry the line info.
         *  </summary>
         */
        private int _line;

        /// <summary>
        /// The 0-based index into the line where the error occurred.
        /// </summary>
        private int _charPositionInLine;

        /** <summary>
         *  If you are parsing a tree node stream, you will encounter som
         *  imaginary nodes w/o line/col info.  We now search backwards looking
         *  for most recent token with line/col info, but notify getErrorHeader()
         *  that info is approximate.
         *  </summary>
         */
        private bool _approximateLineInfo;

        /** <summary>Used for remote debugger deserialization</summary> */
        public RecognitionException()
            : this("A recognition error occurred.", null, null)
        {
        }

        public RecognitionException(IIntStream input)
            : this("A recognition error occurred.", input, 1, null)
        {
        }

        public RecognitionException(IIntStream input, int k)
            : this("A recognition error occurred.", input, k, null)
        {
        }

        public RecognitionException(string message)
            : this(message, null, null)
        {
        }

        public RecognitionException(string message, IIntStream input)
            : this(message, input, 1, null)
        {
        }

        public RecognitionException(string message, IIntStream input, int k)
            : this(message, input, k, null)
        {
        }

        public RecognitionException(string message, Exception innerException)
            : this(message, null, innerException)
        {
        }

        public RecognitionException(string message, IIntStream input, Exception innerException)
            : this(message, input, 1, innerException)
        {
        }

        public RecognitionException(string message, IIntStream input, int k, Exception innerException)
            : base(message, innerException)
        {
            this._input = input;
            this._k = k;
            if (input != null)
            {
                this._index = input.Index + k - 1;
                if (input is ITokenStream)
                {
                    this._token = ((ITokenStream)input).LT(k);
                    this._line = _token.Line;
                    this._charPositionInLine = _token.CharPositionInLine;
                }

                ITreeNodeStream tns = input as ITreeNodeStream;
                if (tns != null)
                {
                    ExtractInformationFromTreeNodeStream(tns, k);
                }
                else
                {
                    ICharStream charStream = input as ICharStream;
                    if (charStream != null)
                    {
                        int mark = input.Mark();
                        try
                        {
                            for (int i = 0; i < k - 1; i++)
                                input.Consume();

                            this._c = input.LA(1);
                            this._line = ((ICharStream)input).Line;
                            this._charPositionInLine = ((ICharStream)input).CharPositionInLine;
                        }
                        finally
                        {
                            input.Rewind(mark);
                        }
                    }
                    else
                    {
                        this._c = input.LA(k);
                    }
                }
            }
        }

#if !PORTABLE
        protected RecognitionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            _index = info.GetInt32("Index");
            _c = info.GetInt32("C");
            _line = info.GetInt32("Line");
            _charPositionInLine = info.GetInt32("CharPositionInLine");
            _approximateLineInfo = info.GetBoolean("ApproximateLineInfo");
        }
#endif

        /** <summary>Return the token type or char of the unexpected input element</summary> */
        public virtual int UnexpectedType
        {
            get
            {
                if ( _input is ITokenStream )
                {
                    return _token.Type;
                }

                ITreeNodeStream treeNodeStream = _input as ITreeNodeStream;
                if ( treeNodeStream != null )
                {
                    ITreeAdaptor adaptor = treeNodeStream.TreeAdaptor;
                    return adaptor.GetType( _node );
                }

                return _c;
            }
        }

        public bool ApproximateLineInfo
        {
            get
            {
                return _approximateLineInfo;
            }
            protected set
            {
                _approximateLineInfo = value;
            }
        }

        public IIntStream Input
        {
            get
            {
                return _input;
            }
            protected set
            {
                _input = value;
            }
        }

        public int Lookahead
        {
            get
            {
                return _k;
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

        public object Node
        {
            get
            {
                return _node;
            }
            protected set
            {
                _node = value;
            }
        }

        public int Character
        {
            get
            {
                return _c;
            }
            protected set
            {
                _c = value;
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
            protected set
            {
                _index = value;
            }
        }

        public int Line
        {
            get
            {
                return _line;
            }
            set
            {
                _line = value;
            }
        }

        public int CharPositionInLine
        {
            get
            {
                return _charPositionInLine;
            }
            set
            {
                _charPositionInLine = value;
            }
        }

#if !PORTABLE
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue("Index", _index);
            info.AddValue("C", _c);
            info.AddValue("Line", _line);
            info.AddValue("CharPositionInLine", _charPositionInLine);
            info.AddValue("ApproximateLineInfo", _approximateLineInfo);
        }
#endif

        protected virtual void ExtractInformationFromTreeNodeStream(ITreeNodeStream input)
        {
            this._node = input.LT(1);

            object positionNode = null;
            IPositionTrackingStream positionTrackingStream = input as IPositionTrackingStream;
            if (positionTrackingStream != null)
            {
                positionNode = positionTrackingStream.GetKnownPositionElement(false);
                if (positionNode == null)
                {
                    positionNode = positionTrackingStream.GetKnownPositionElement(true);
                    this._approximateLineInfo = positionNode != null;
                }
            }

            ITokenStreamInformation streamInformation = input as ITokenStreamInformation;
            if (streamInformation != null)
            {
                IToken lastToken = streamInformation.LastToken;
                IToken lastRealToken = streamInformation.LastRealToken;
                if (lastRealToken != null)
                {
                    this._token = lastRealToken;
                    this._line = lastRealToken.Line;
                    this._charPositionInLine = lastRealToken.CharPositionInLine;
                    this._approximateLineInfo = lastRealToken.Equals(lastToken);
                }
            }
            else
            {
                ITreeAdaptor adaptor = input.TreeAdaptor;
                IToken payload = adaptor.GetToken(positionNode ?? _node);
                if (payload != null)
                {
                    this._token = payload;
                    if (payload.Line <= 0)
                    {
                        // imaginary node; no line/pos info; scan backwards
                        int i = -1;
                        object priorNode = input.LT(i);
                        while (priorNode != null)
                        {
                            IToken priorPayload = adaptor.GetToken(priorNode);
                            if (priorPayload != null && priorPayload.Line > 0)
                            {
                                // we found the most recent real line / pos info
                                this._line = priorPayload.Line;
                                this._charPositionInLine = priorPayload.CharPositionInLine;
                                this._approximateLineInfo = true;
                                break;
                            }

                            --i;
                            try
                            {
                                priorNode = input.LT(i);
                            }
                            catch (NotSupportedException)
                            {
                                priorNode = null;
                            }
                        }
                    }
                    else
                    {
                        // node created from real token
                        this._line = payload.Line;
                        this._charPositionInLine = payload.CharPositionInLine;
                    }
                }
                else if (this._node is Tree.ITree)
                {
                    this._line = ((Tree.ITree)this._node).Line;
                    this._charPositionInLine = ((Tree.ITree)this._node).CharPositionInLine;
                    if (this._node is CommonTree)
                    {
                        this._token = ((CommonTree)this._node).Token;
                    }
                }
                else
                {
                    int type = adaptor.GetType(this._node);
                    string text = adaptor.GetText(this._node);
                    this._token = new CommonToken(type, text);
                }
            }
        }

        protected virtual void ExtractInformationFromTreeNodeStream(ITreeNodeStream input, int k)
        {
            int mark = input.Mark();
            try
            {
                for (int i = 0; i < k - 1; i++)
                    input.Consume();

                ExtractInformationFromTreeNodeStream(input);
            }
            finally
            {
                input.Rewind(mark);
            }
        }
    }
}
