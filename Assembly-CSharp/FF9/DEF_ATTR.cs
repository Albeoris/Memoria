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

        public DEF_ATTR(ElementSet invalid, ElementSet absorb, ElementSet half, ElementSet weak)
        {
            this.invalid = invalid;
            this.absorb = absorb;
            this.half = half;
            this.weak = weak;
        }

        public ElementSet invalid;
        public ElementSet absorb;
        public ElementSet half;
        public ElementSet weak;
    }
}
