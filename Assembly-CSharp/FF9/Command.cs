using System;

namespace FF9
{
    public class Command
    {
        public const Byte CMDSYS_STATUS_ESCAPE = 1;

        public const Byte CMDSYS_STATUS_CURSOR_ON = 2;

        public const Byte CMDSYS_STATUS_PHANTOM = 4;

        public const Byte CMDSYS_STATUS_PHANTOM_SET = 8;

        public const Byte CMDSYS_STATUS_JUMP_SET = 16;

        public const Int32 TAR_MODE_OFF = 0;

        public const Int32 TAR_MODE_ON = 1;

        public const Int32 TAR_MODE_REQ_OFF = 2;

        public const Int32 TAR_MODE_REQ_ON = 3;

        public const Byte SEL_MODE_EMPTY = 0;

        public const Byte SEL_MODE_FILL = 1;

        public const Byte DEF_CUR_PLAYER = 1;

        public const Byte DEF_CUR_ENEMY = 0;
    }
}
