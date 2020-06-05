using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// WorldMap
    /// Change the scene to a world map.
    /// 
    /// 1st argument: world map destination.
    /// AT_WORLDMAP World Map (2 bytes)
    /// WMAPJUMP = 0x0B6,
    /// </summary>
    internal sealed class WMAPJUMP : JsmInstruction
    {
        private readonly IJsmExpression _worldMap;

        private WMAPJUMP(IJsmExpression worldMap)
        {
            _worldMap = worldMap;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression worldMap = reader.ArgumentInt16();
            return new WMAPJUMP(worldMap);
        }
        public override String ToString()
        {
            return $"{nameof(WMAPJUMP)}({nameof(_worldMap)}: {_worldMap})";
        }
    }
}