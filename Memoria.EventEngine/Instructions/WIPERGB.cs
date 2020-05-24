using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// FadeFilter
    /// Apply a fade filter on the screen.
    /// 
    /// 1st argument: filter mode (0 for ADD, 2 for SUBTRACT).
    /// 2nd argument: fading time.
    /// 3rd argument: unknown.
    /// 4th to 6th arguments: color of the filter in (Cyan, Magenta, Yellow) format.
    /// AT_USPIN Fade In/Out (1 bytes)
    /// AT_USPIN Fading Time (1 bytes)
    /// AT_SPIN Unknown (1 bytes)
    /// AT_COLOR_CYAN ColorC (1 bytes)
    /// AT_COLOR_MAGENTA ColorM (1 bytes)
    /// AT_COLOR_YELLOW ColorY (1 bytes)
    /// WIPERGB = 0x0EC,
    /// </summary>
    internal sealed class WIPERGB : JsmInstruction
    {
        private readonly IJsmExpression _fadeInOut;
        private readonly IJsmExpression _fadingTime;
        private readonly IJsmExpression _unknown;
        private readonly IJsmExpression _colorC;
        private readonly IJsmExpression _colorM;
        private readonly IJsmExpression _colorY;

        private WIPERGB(IJsmExpression fadeInOut, IJsmExpression fadingTime, IJsmExpression unknown, IJsmExpression colorC, IJsmExpression colorM, IJsmExpression colorY)
        {
            _fadeInOut = fadeInOut;
            _fadingTime = fadingTime;
            _unknown = unknown;
            _colorC = colorC;
            _colorM = colorM;
            _colorY = colorY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression fadeInOut = reader.ArgumentByte();
            IJsmExpression fadingTime = reader.ArgumentByte();
            IJsmExpression unknown = reader.ArgumentByte();
            IJsmExpression colorC = reader.ArgumentByte();
            IJsmExpression colorM = reader.ArgumentByte();
            IJsmExpression colorY = reader.ArgumentByte();
            return new WIPERGB(fadeInOut, fadingTime, unknown, colorC, colorM, colorY);
        }
        public override String ToString()
        {
            return $"{nameof(WIPERGB)}({nameof(_fadeInOut)}: {_fadeInOut}, {nameof(_fadingTime)}: {_fadingTime}, {nameof(_unknown)}: {_unknown}, {nameof(_colorC)}: {_colorC}, {nameof(_colorM)}: {_colorM}, {nameof(_colorY)}: {_colorY})";
        }
    }
}