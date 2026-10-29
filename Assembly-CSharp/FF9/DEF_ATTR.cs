using System;
using Memoria.Data;

namespace FF9
{
    public class DEF_ATTR
    {
        public DEF_ATTR()
        {
            this.invalid = new ElementSet();
            this.absorb = new ElementSet();
            this.half = new ElementSet();
            this.weak = new ElementSet();
        }

        public ElementSet invalid;
        public ElementSet absorb;
        public ElementSet half;
        public ElementSet weak;
    }
}
