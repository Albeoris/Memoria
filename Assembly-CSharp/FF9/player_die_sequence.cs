using System;

namespace FF9
{
    internal enum player_die_sequence
    {
        PLAYER_DIE_MOTION_SET = 1,
        PLAYER_DIE_IDLE_SELECT,
        PLAYER_DIE_DOWN_TO_DYING,
        PLAYER_DIE_DOWN_TO_DISABLE,
        PLAYER_DIE_DISABLE,
        PLAYER_DIE_DONE,
        PLAYER_DIE_RERAISE
    }
}
