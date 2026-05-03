using System;

namespace Memoria.Data
{
    public enum FailedMixType
    {
        FIRST_ITEM,
        SECOND_ITEM,
        SKIP_TURN,
        USE_ITEMS,
        CANCEL_MENU,
        FAIL_ITEM
    }

    public class MixCommandType
    {
        public FailedMixType failType;
        public RegularItem failItem;
        public Boolean consumeOnFail;

        public MixCommandType(FailedMixType failType, RegularItem failFallback, Boolean consumeOnFail)
        {
            this.failType = failType;
            this.failItem = failFallback;
            this.consumeOnFail = consumeOnFail;
        }
    }
}
