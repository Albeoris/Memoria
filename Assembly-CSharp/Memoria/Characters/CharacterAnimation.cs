using System;

namespace Memoria
{
    public sealed class CharacterAnimation
    {
        public Byte[] IdleSpeed;
        public UInt16 Idle; //ANH_NPC_F0_HUM_IDLE
        public UInt16 Walk; //ANH_NPC_F0_HUM_WALK
        public UInt16 Run; //ANH_NPC_F0_HUM_RUN
        public UInt16 TurnLeft; //ANH_NPC_F0_HUM_TURN_L
        public UInt16 TurnRight; //ANH_NPC_F0_HUM_TURN_R
    }
}
