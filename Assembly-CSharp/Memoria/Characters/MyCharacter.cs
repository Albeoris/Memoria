using System;
using UnityEngine;

namespace Memoria
{
    public sealed class MyCharacter : ICharacterDescriptor
    {
        public String Name => nameof(MyCharacter);

        public UInt16 Model => 110; // GEO_NPC_F1_HUM
        public Int16 Eyes => -400; // GEO_NPC_F1_HUM
        public Byte NeckMyId => 0;
        public Byte NeckTargetId => 65;
        public ObjectFlags ObjectFlags => (ObjectFlags)5;
        public ActorFlags ActorFlags => ActorFlags.NeckT | ActorFlags.NeckM | ActorFlags.Look;
        public Byte TrackingRadius => 20;
        public Byte TalkingRadius => 30;
        public Vector3 StartupLocation => new Vector3(-205.0f, -34.4f, 3039.0f);

        public CharacterAnimation GetAnimation()
        {
            return new CharacterAnimation
            {
                IdleSpeed = new Byte[] { 14, 16, 18, 20 },
                Idle = 921, //ANH_NPC_F0_HUM_IDLE
                TurnRight = 922, //ANH_NPC_F0_HUM_TURN_R
                Run = 923, //ANH_NPC_F0_HUM_RUN
                Walk = 924, //ANH_NPC_F0_HUM_WALK
                TurnLeft = 925 //ANH_NPC_F0_HUM_TURN_L
            };
        }
    }
}
