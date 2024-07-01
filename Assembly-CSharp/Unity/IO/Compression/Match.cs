using System;

namespace Unity.IO.Compression
{
    internal class Match
    {
        internal MatchState State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
            }
        }

        internal Int32 Position
        {
            get
            {
                return this.pos;
            }
            set
            {
                this.pos = value;
            }
        }

        internal Int32 Length
        {
            get
            {
                return this.len;
            }
            set
            {
                this.len = value;
            }
        }

        internal Byte Symbol
        {
            get
            {
                return this.symbol;
            }
            set
            {
                this.symbol = value;
            }
        }

        private MatchState state;

        private Int32 pos;

        private Int32 len;

        private Byte symbol;
    }
}
