/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2011 Sam Harwell, Pixel Mine, Inc.
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
    using ConditionalAttribute = System.Diagnostics.ConditionalAttribute;

    /** <summary>
     *  A parser for TokenStreams.  "parser grammars" result in a subclass
     *  of this.
     *  </summary>
     */
    public class Parser : BaseRecognizer
    {
        public ITokenStream input;

        public Parser( ITokenStream input )
            : base()
        {
            //super(); // highlight that we go to super to set state object
            TokenStream = input;
        }

        public Parser( ITokenStream input, RecognizerSharedState state )
            : base(state) // share the state object with another parser
        {
            this.input = input;
        }

        public override void Reset()
        {
            base.Reset(); // reset all recognizer state variables
            if ( input != null )
            {
                input.Seek( 0 ); // rewind the input
            }
        }

        protected override object GetCurrentInputSymbol( IIntStream input )
        {
            return ( (ITokenStream)input ).LT( 1 );
        }

        protected override object GetMissingSymbol( IIntStream input,
                                          RecognitionException e,
                                          int expectedTokenType,
                                          BitSet follow )
        {
            string tokenText = null;
            if ( expectedTokenType == TokenTypes.EndOfFile )
                tokenText = "<missing EOF>";
            else
                tokenText = "<missing " + TokenNames[expectedTokenType] + ">";
            CommonToken t = new CommonToken( expectedTokenType, tokenText );
            IToken current = ( (ITokenStream)input ).LT( 1 );
            if ( current.Type == TokenTypes.EndOfFile )
            {
                current = ( (ITokenStream)input ).LT( -1 );
            }
            t.Line = current.Line;
            t.CharPositionInLine = current.CharPositionInLine;
            t.Channel = DefaultTokenChannel;
            t.InputStream = current.InputStream;
            return t;
        }

        /** <summary>Gets or sets the token stream; resets the parser upon a set.</summary> */
        public virtual ITokenStream TokenStream
        {
            get
            {
                return input;
            }
            set
            {
                input = null;
                Reset();
                input = value;
            }
        }

        public override string SourceName
        {
            get
            {
                return input.SourceName;
            }
        }

        [Conditional("ANTLR_TRACE")]
        public virtual void TraceIn( string ruleName, int ruleIndex )
        {
            base.TraceIn( ruleName, ruleIndex, input.LT( 1 ) );
        }

        [Conditional("ANTLR_TRACE")]
        public virtual void TraceOut( string ruleName, int ruleIndex )
        {
            base.TraceOut( ruleName, ruleIndex, input.LT( 1 ) );
        }
    }
}
