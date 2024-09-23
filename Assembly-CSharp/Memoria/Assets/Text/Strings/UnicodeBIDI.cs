using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    // Bi-directional algorithm for displaying text in left-to-right, right-to-left and mixed reading directions
    // Note that Unity doesn't support Unicode characters that require UTF-16 surrogates (code points 0x10000 and higher)
    // Documentations:
    //  https://www.w3.org/International/articles/inline-bidi-markup/uba-basics (summary of the BIDI algorithm)
    //  https://www.unicode.org/reports/tr44/ (overall Unicode database algorithms)
    //  https://www.unicode.org/reports/tr9/ (BIDI algorithm)
    //  https://www.unicode.org/versions/Unicode10.0.0/ch09.pdf#G28212 (Character joinings and ligatures algorithm)
    // Databases:
    //  https://www.unicode.org/Public/16.0.0/ucd/extracted/DerivedBidiClass.txt (BIDI classes)
    //  https://www.unicode.org/Public/16.0.0/ucd/BidiMirroring.txt (BIDI characters to be mirrored in RTL)
    //  https://www.unicode.org/Public/16.0.0/ucd/extracted/DerivedJoiningType.txt (Character shaping types, for cursive languages)
    public class UnicodeBIDI
    {
        public const String DIRECTION_NAME_LEFT_TO_RIGHT = "Left_To_Right";
        public const String DIRECTION_NAME_RIGHT_TO_LEFT = "Right_To_Left";

        public readonly Char[] FullText;
        public readonly Int32[] Reposition;
        public readonly HashSet<Int32> MustMirror;

        public UnicodeBIDI(Char[] text, LanguageReadingDirection paragraphDirection)
        {
            // Non-compliant implementation of the algorithm
            FullText = text;
            Int32 textLength = text.Length;
            Int32 index = 0;
            Int32 isolateCount = 0;
            Int32 overrideCount = 0;
            DirectionalStatus currentState = new DirectionalStatus
            {
                ltr = paragraphDirection == LanguageReadingDirection.LeftToRight,
                isIsolate = false,
                isOverride = false,
                overrideStatus = OVERRIDE_STATUS_NEUTRAL,
                start = index,
                end = textLength
            };
            DirectionalStackRoot.Clear();
            DirectionalStackRoot.Add(currentState);
            while (index < textLength)
            {
                CharacterClass bidiClass = GetBIDIClass(text[index]);
                switch (bidiClass)
                {
                    case CharacterClass.Right_To_Left_Embedding:
                    case CharacterClass.Left_To_Right_Embedding:
                        DirectionalStatus.Push(ref currentState, index, bidiClass == CharacterClass.Left_To_Right_Embedding, OVERRIDE_STATUS_NEUTRAL, false, true);
                        overrideCount++;
                        break;
                    case CharacterClass.Right_To_Left_Override:
                        DirectionalStatus.Push(ref currentState, index, false, OVERRIDE_STATUS_RTL, false, true);
                        overrideCount++;
                        break;
                    case CharacterClass.Left_To_Right_Override:
                        DirectionalStatus.Push(ref currentState, index, true, OVERRIDE_STATUS_LTR, false, true);
                        overrideCount++;
                        break;
                    case CharacterClass.Right_To_Left_Isolate:
                    case CharacterClass.Left_To_Right_Isolate:
                    case CharacterClass.First_Strong_Isolate:
                        DirectionalStatus.Push(ref currentState, index, bidiClass == CharacterClass.Left_To_Right_Isolate || (bidiClass == CharacterClass.First_Strong_Isolate && paragraphDirection == LanguageReadingDirection.LeftToRight), OVERRIDE_STATUS_NEUTRAL, true, false);
                        isolateCount++;
                        break;
                    case CharacterClass.Pop_Directional_Format:
                        if (overrideCount == 0)
                            break;
                        while (!currentState.isIsolate && !currentState.isOverride)
                            DirectionalStatus.Pop(ref currentState, index);
                        if (currentState.isOverride)
                        {
                            DirectionalStatus.Pop(ref currentState, index);
                            overrideCount--;
                        }
                        break;
                    case CharacterClass.Pop_Directional_Isolate:
                        if (isolateCount == 0)
                            break;
                        while (!currentState.isIsolate)
                        {
                            if (currentState.isOverride)
                                overrideCount--;
                            DirectionalStatus.Pop(ref currentState, index);
                        }
                        if (currentState.isOverride)
                            overrideCount--;
                        DirectionalStatus.Pop(ref currentState, index);
                        isolateCount--;
                        break;
                    case CharacterClass.Boundary_Neutral:
                    case CharacterClass.Paragraph_Separator:
                        while (currentState.parent != null)
                            DirectionalStatus.Pop(ref currentState, index);
                        currentState.end = index;
                        currentState = new DirectionalStatus
                        {
                            ltr = paragraphDirection == LanguageReadingDirection.LeftToRight,
                            isIsolate = false,
                            isOverride = false,
                            overrideStatus = OVERRIDE_STATUS_NEUTRAL,
                            start = index + 1,
                            end = textLength
                        };
                        DirectionalStackRoot.Add(currentState);
                        overrideCount = isolateCount = 0;
                        break;
                    case CharacterClass.Left_To_Right:
                        if (currentState.overrideStatus == OVERRIDE_STATUS_NEUTRAL)
                        {
                            currentState.ltr = true;
                            currentState.overrideStatus = OVERRIDE_STATUS_LTR;
                        }
                        else
                        {
                            while (!currentState.isIsolate && !currentState.isOverride && !currentState.ltr && currentState.parent != null)
                                DirectionalStatus.Pop(ref currentState, index);
                            if (!currentState.ltr)
                                DirectionalStatus.Push(ref currentState, index, true, OVERRIDE_STATUS_LTR, false, false);
                        }
                        break;
                    case CharacterClass.Right_To_Left:
                    case CharacterClass.Arabic_Letter:
                        if (currentState.overrideStatus == OVERRIDE_STATUS_NEUTRAL)
                        {
                            currentState.ltr = false;
                            currentState.overrideStatus = OVERRIDE_STATUS_RTL;
                        }
                        else
                        {
                            while (!currentState.isIsolate && !currentState.isOverride && currentState.ltr && currentState.parent != null)
                                DirectionalStatus.Pop(ref currentState, index);
                            if (!currentState.ltr)
                                DirectionalStatus.Push(ref currentState, index, false, OVERRIDE_STATUS_RTL, false, false);
                        }
                        break;
                    case CharacterClass.Arabic_Number:
                    case CharacterClass.European_Number:
                        if (currentState.overrideStatus == OVERRIDE_STATUS_RTL)
                        {
                            DirectionalStatus.Push(ref currentState, index, true, OVERRIDE_STATUS_LTR, false, false);
                        }
                        else
                        {
                            currentState.ltr = true;
                            currentState.overrideStatus = OVERRIDE_STATUS_LTR;
                        }
                        break;
                    default:
                        // TODO: everything else is handled like a white space (eg. multiple European_Separator and/or Common_Separator cannot increase the level depth)
                        break;
                }
                index++;
            }
            HashSet<Int32> pairPositions = new HashSet<Int32>();
            MustMirror = new HashSet<Int32>();
            Reposition = new Int32[FullText.Length];
            for (Int32 i = 0; i < textLength; i++)
                Reposition[i] = i;
            foreach (DirectionalStatus lineStatus in DirectionalStackRoot)
            {
                foreach (DirectionalStatus status in lineStatus.GetAllDirectionChanges(true))
                {
                    ChangeDirection(Reposition, status.start, status.end - 1);
                    for (Int32 i = status.start; i < status.end; i++)
                    {
                        Char ch = FullText[i];
                        if (MirroringCharacters.ContainsKey(ch))
                        {
                            if (status.ltr)
                                pairPositions.Remove(i);
                            else
                                pairPositions.Add(i);
                        }
                        else if (MirroringCharactersNoEquivalent.Contains(ch))
                        {
                            if (status.ltr)
                                MustMirror.Remove(i);
                            else
                                MustMirror.Add(i);
                        }
                    }
                }
            }
            for (Int32 i = 0; i < textLength; i++)
                if (pairPositions.Contains(i))
                    FullText[i] = GetMirroredCharacter(FullText[i]);
            for (Int32 i = 0; i < textLength;)
                ApplyWordJoining(ref i);
            DirectionalStackRoot.Clear();
        }

        public static Char GetMirroredCharacter(Char c)
        {
            // TODO: Maybe apply the algorithm BD16 (https://www.unicode.org/reports/tr9/#BD16) instead of mirroring every bracket character, paired or not, in RTL
            if (MirroringCharacters.TryGetValue(c, out Char mirror))
                return mirror;
            return c;
        }

        public void ApplyWordJoining(ref Int32 index)
        {
            Int32 chPos = index;
            Char ch = FullText[index++];
            Char transform, transformNext;
            Boolean canJoin = false;
            if (BeforeJoiningCharacters.TryGetValue(ch, out transform) || DualJoiningCharacters.TryGetValue(ch, out transform))
            {
                canJoin = true;
            }
            else if (JoinCausingCharacters.Contains(ch))
            {
                transform = ch;
                canJoin = true;
            }
            while (canJoin && index < FullText.Length)
            {
                ch = FullText[index];
                if (DualJoiningCharacters.TryGetValue(ch, out transformNext))
                {
                    FullText[chPos] = transform;
                    chPos = index++;
                    transform = transformNext;
                    canJoin = true;
                }
                else if (AfterJoiningCharacters.TryGetValue(ch, out transformNext))
                {
                    FullText[chPos] = transform;
                    FullText[index++] = transformNext;
                    canJoin = false;
                }
                else if (JoinCausingCharacters.Contains(ch))
                {
                    FullText[chPos] = transform;
                    chPos = index++;
                    transform = ch;
                    canJoin = true;
                }
                else if (IsJoinTransparent(ch))
                {
                    canJoin = true;
                    ++index;
                }
                else
                {
                    canJoin = false;
                    ++index;
                }
            }
        }

        public static CharacterClass GetBIDIClass(Char c)
        {
            if (c <= 0x058F)
            {
                if (c == '[') // Brackets, used for text opcodes, force a left-to-right isolate
                    return CharacterClass.Left_To_Right_Isolate;
                if (c == ']') // The game uses the special characters【】(0x3010 and 0x3011) for non-opcode brackets
                    return CharacterClass.Pop_Directional_Isolate;
                if (c >= 0x0000 && c <= 0x0008)
                    return CharacterClass.Boundary_Neutral;
                if (c == 0x000C || c == 0x0020)
                    return CharacterClass.White_Space;
                if (c == 0x000A || c == 0x000D || c >= 0x001C && c <= 0x001E || c == 0x0085)
                    return CharacterClass.Paragraph_Separator;
                if (c == 0x0009 || c == 0x000B || c == 001F)
                    return CharacterClass.Segment_Separator;
                if (c >= 0x000E && c <= 0x001B)
                    return CharacterClass.Boundary_Neutral;
                if (c == 0x0021 || c == 0x0022)
                    return CharacterClass.Other_Neutral;
                if (c >= 0x0023 && c <= 0x0025)
                    return CharacterClass.European_Terminator;
                if (c >= 0x0026 && c <= 0x002A)
                    return CharacterClass.Other_Neutral;
                if (c == 0x002B || c == 0x002D)
                    return CharacterClass.European_Separator;
                if (c == 0x002C || c == 0x002E || c == 0x002F || c == 0x003A || c == 0x00A0)
                    return CharacterClass.Common_Separator;
                if (c >= 0x0030 && c <= 0x0039 || c == 0x00B2 || c == 0x00B3 || c == 0x00B9)
                    return CharacterClass.European_Number;
                if (c >= 0x003B && c <= 0x0040 || c >= 0x005B && c <= 0x0060 || c >= 0x007B && c <= 0x007E || c == 0x00A1)
                    return CharacterClass.Other_Neutral;
                if (c >= 0x007F && c <= 0x009F)
                    return CharacterClass.Boundary_Neutral;
                if (c >= 0x00A2 && c <= 0x00A5 || c == 0x00B0 || c == 0x00B1 || c == 0x058F)
                    return CharacterClass.European_Terminator;
                if (c >= 0x00A6 && c <= 0x00AF)
                    return c == 0x00AD ? CharacterClass.Boundary_Neutral : c == 0x00AA ? CharacterClass.Left_To_Right : CharacterClass.Other_Neutral;
                if (c == 0x00B4 || c >= 0x00B6 && c <= 0x00B8 || c >= 0x00BB && c <= 0x00BF || c == 0x00D7 || c == 0x00F7 || c >= 0x02C2 && c <= 0x02CF || c >= 0x02D2 && c <= 0x02DF)
                    return CharacterClass.Other_Neutral;
                if (c == 0x02B9 || c == 0x02BA)
                    return CharacterClass.Other_Neutral;
                if (c >= 0x02E5 && c <= 0x02FF)
                    return c == 0x02EE ? CharacterClass.Left_To_Right : CharacterClass.Other_Neutral;
                if (c >= 0x0300 && c <= 0x036F || c >= 0x0483 && c <= 0x0489)
                    return CharacterClass.Nonspacing_Mark;
                if (c == 0x0374 || c == 0x0375 || c == 0x037E || c == 0x0384 || c == 0x0385 || c == 0x0387 || c == 0x03F6 || c == 0x058A || c == 0x058D || c == 0x058E)
                    return CharacterClass.Other_Neutral;
                return CharacterClass.Left_To_Right;
            }
            //else if (c >= 0x0590 && c <= 0x05FF)
            //{
            //    if (c >= 0x0591 && c <= 0x05BD || c == 0x05BF || c == 0x05C1 || c == 0x05C2 || c == 0x05C4 || c == 0x05C5 || c == 0x05C7)
            //        return CharacterClass.Nonspacing_Mark;
            //    return CharacterClass.Right_To_Left;
            //}
            else if (c >= 0x0600 && c <= 0x07BF)
            {
                if (c >= 0x0600 && c <= 0x0605)
                    return CharacterClass.Arabic_Number;
                if (c == 0x0606 || c == 0x0607 || c == 0x060E || c == 0x060F)
                    return CharacterClass.Other_Neutral;
                if (c == 0x0609 || c == 0x060A)
                    return CharacterClass.European_Terminator;
                if (c == 0x060C)
                    return CharacterClass.Common_Separator;
                if (c >= 0x0610 && c <= 0x061A || c >= 0x064B && c <= 0x065F || c == 0x0670)
                    return CharacterClass.Nonspacing_Mark;
                if (c >= 0x0660 && c <= 0x066C)
                    return c == 0x066A ? CharacterClass.European_Terminator : CharacterClass.Arabic_Number;
                if (c == 0x06DE || c == 0x06E9)
                    return CharacterClass.Other_Neutral;
                if (c >= 0x06D6 && c <= 0x06E4)
                    return c == 0x06DD ? CharacterClass.Arabic_Number : CharacterClass.Nonspacing_Mark;
                if (c == 0x06E7 || c == 0x06E8 || c >= 0x06EA && c <= 0x06ED)
                    return CharacterClass.Nonspacing_Mark;
                if (c >= 0x06F0 && c <= 0x06F9)
                    return CharacterClass.European_Number;
                if (c == 0x0711 || c >= 0x0730 && c <= 0x074A || c >= 0x07A6 && c <= 0x07B0)
                    return CharacterClass.Nonspacing_Mark;
                return CharacterClass.Arabic_Letter;
            }
            else if (c >= 0x07C0 && c <= 0x085F)
            {
                if (c >= 0x07EB && c <= 0x07F3 || c == 0x07FD)
                    return CharacterClass.Nonspacing_Mark;
                if (c >= 0x07F6 && c <= 0x07F9)
                    return CharacterClass.Other_Neutral;
                if (c >= 0x0816 && c <= 0x0819 || c >= 0x081B && c <= 0x0823 || c >= 0x0825 && c <= 0x0827 || c >= 0x0829 && c <= 0x082D || c >= 0x0859 && c <= 0x085B)
                    return CharacterClass.Nonspacing_Mark;
                return CharacterClass.Right_To_Left;
            }
            else if (c >= 0x0860 && c <= 0x08FF)
            {
                if (c == 0x0890 || c == 0x0891 || c == 0x08E2)
                    return CharacterClass.Arabic_Number;
                if (c >= 0x0897 && c <= 0x089F || c >= 0x08CA && c <= 0x08E1 || c >= 0x08E3 && c <= 0x0902)
                    return CharacterClass.Nonspacing_Mark;
                return CharacterClass.Arabic_Letter;
            }
            else if (c >= 0x20A0 && c <= 0x20CF)
            {
                return CharacterClass.European_Terminator;
            }
            else if (c >= 0xFB00 && c <= 0xFB4F)
            {
                if (c == 0xFB1E)
                    return CharacterClass.Nonspacing_Mark;
                if (c == 0xFB29)
                    return CharacterClass.European_Separator;
                return CharacterClass.Right_To_Left;
            }
            else if (c >= 0xFB50 && c <= 0xFDFF)
            {
                if (c >= 0xFD3E && c <= 0xFD4F)
                    return CharacterClass.Other_Neutral;
                if (c >= 0xFDD0 && c <= 0xFDEF)
                    return CharacterClass.Boundary_Neutral;
                if (c == 0xFDCF || c >= 0xFDFD && c <= 0xFDFF)
                    return CharacterClass.Other_Neutral;
                return CharacterClass.Arabic_Letter;
            }
            else if (c >= 0xFE70 && c <= 0xFEFF)
            {
                if (c == 0xFEFF)
                    return CharacterClass.Boundary_Neutral;
                return CharacterClass.Arabic_Letter;
            }
            if (SingleCharClass.TryGetValue(c, out CharacterClass bidiClass))
                return bidiClass;
            if (c >= 0x0941 && c <= 0x0948 || c >= 0x0951 && c <= 0x0957 || c >= 0x09C1 && c <= 0x09C4 || c >= 0x0A41 && c <= 0x0A51 || c >= 0x0AC1 && c <= 0x0AC8 || c >= 0x0AFA && c <= 0x0AFF || c >= 0x0B41 && c <= 0x0B44)
                return CharacterClass.Nonspacing_Mark;
            if (c >= 0x0BF3 && c <= 0x0BFA)
                return CharacterClass.Other_Neutral;
            if (c >= 0x0C3E && c <= 0x0C40 || c >= 0x0C46 && c <= 0x0C56 || c >= 0x0D41 && c <= 0x0D44 || c >= 0x0DD2 && c <= 0x0DD6 || c >= 0x0E34 && c <= 0x0E3A || c >= 0x0E47 && c <= 0x0E4E || c >= 0x0EB4 && c <= 0x0EBC || c >= 0x0EC8 && c <= 0x0ECE || c >= 0x0F71 && c <= 0x0F7E)
                return CharacterClass.Nonspacing_Mark;
            if (c >= 0x0C78 && c <= 0x0C7E || c >= 0x0F3A && c <= 0x0F3D || c >= 0x1390 && c <= 0x1399 || c >= 0x17F0 && c <= 0x17F9 || c >= 0x1800 && c <= 0x180A || c >= 0x19DE && c <= 0x19FF)
                return CharacterClass.Other_Neutral;
            if (c >= 0x0F80 && c <= 0x0F87)
                return c == 0x0F85 ? CharacterClass.Left_To_Right : CharacterClass.Nonspacing_Mark;
            if (c >= 0x0F8D && c <= 0x0FBC || c >= 0x102D && c <= 0x1030 || c >= 0x1032 && c <= 0x1037 || c >= 0x105E && c <= 0x1060 || c >= 0x1071 && c <= 0x1074 || c >= 0x135D && c <= 0x135F || c >= 0x1712 && c <= 0x1714 || c >= 0x17B7 && c <= 0x17BD || c >= 0x17C9 && c <= 0x17D3)
                return CharacterClass.Nonspacing_Mark;
            if (c >= 0x180B && c <= 0x180D || c >= 0x1920 && c <= 0x1922 || c >= 0x1939 && c <= 0x193B || c >= 0x1A58 && c <= 0x1A60 || c >= 0x1A65 && c <= 0x1A6C || c >= 0x1A73 && c <= 0x1A7F || c >= 0x1AB0 && c <= 0x1ACE || c >= 0x1B00 && c <= 0x1B03 || c >= 0x1B36 && c <= 0x1B3A)
                return CharacterClass.Nonspacing_Mark;
            if (c >= 0x1B6B && c <= 0x1B73 || c >= 0x1BA2 && c <= 0x1BA5 || c >= 0x1BAB && c <= 0x1BAD || c >= 0x1BEF && c <= 0x1BF1 || c >= 0x1C2C && c <= 0x1C33 || c >= 0x1CD0 && c <= 0x1CD2 || c >= 0x1CD4 && c <= 0x1CE0 || c >= 0x1CE2 && c <= 0x1CE8 || c >= 0x1DC0 && c <= 0x1DFF)
                return CharacterClass.Nonspacing_Mark;
            if (c >= 0x1FBF && c <= 0x1FC1 || c >= 0x1FCD && c <= 0x1FCF || c >= 0x1FDD && c <= 0x1FDF || c >= 0x1FED && c <= 0x1FEF || c >= 0x1FFD && c <= 0x1FFF)
                return CharacterClass.Other_Neutral;
            if (c >= 0x2000 && c <= 0x200A)
                return CharacterClass.White_Space;
            if (c >= 0x200B && c <= 0x200D)
                return CharacterClass.Boundary_Neutral;
            if (c >= 0x2010 && c <= 0x2027)
                return CharacterClass.Other_Neutral;
            if (c >= 0x2030 && c <= 0x2034)
                return CharacterClass.European_Terminator;
            if (c >= 0x2035 && c <= 0x205E)
                return CharacterClass.Other_Neutral;
            if (c >= 0x2060 && c <= 0x206F)
                return CharacterClass.Boundary_Neutral;
            if (c >= 0x2070 && c <= 0x2079)
                return c == 0x2071 ? CharacterClass.Left_To_Right : CharacterClass.European_Number;
            if (c >= 0x2080 && c <= 0x2089)
                return CharacterClass.European_Number;
            if (c >= 0x20D0 && c <= 0x20F0)
                return CharacterClass.Nonspacing_Mark;
            if (c >= 0x2103 && c <= 0x2106 || c >= 0x211E && c <= 0x2123 || c >= 0x2140 && c <= 0x2144 || c >= 0x214A && c <= 0x214D || c >= 0x2150 && c <= 0x215F || c >= 0x2189 && c <= 0x218B || c >= 0x2190 && c <= 0x2335 || c >= 0x2116 && c <= 0x2118)
                return CharacterClass.Other_Neutral;
            if (c >= 0x237B && c <= 0x2429)
                return c == 0x2395 ? CharacterClass.Left_To_Right : CharacterClass.Other_Neutral;
            if (c >= 0x2440 && c <= 0x244A || c >= 0x2460 && c <= 0x2487)
                return CharacterClass.Other_Neutral;
            if (c >= 0x2488 && c <= 0x249B)
                return CharacterClass.European_Number;
            if (c >= 0x24EA && c <= 0x27FF)
                return c == 0x26AC ? CharacterClass.Left_To_Right : CharacterClass.Other_Neutral;
            if (c >= 0x2900 && c <= 0x2BFF || c >= 0x2CE5 && c <= 0x2CEA || c >= 0x2CF9 && c <= 0x2CFF || c >= 0x2E00 && c <= 0x2E5D || c >= 0x2E80 && c <= 0x3004 || c >= 0x3008 && c <= 0x3020 || c >= 0x303D && c <= 0x303F)
                return CharacterClass.Other_Neutral;
            if (c >= 0x2CEF && c <= 0x2CF1 || c >= 0x2DE0 && c <= 0x2DFF || c >= 0x302A && c <= 0x302D)
                return CharacterClass.Nonspacing_Mark;
            if (c >= 0x31C0 && c <= 0x31E5 || c >= 0x3250 && c <= 0x325F || c >= 0x327C && c <= 0x327E || c >= 0x32B1 && c <= 0x32BF || c >= 0x32CC && c <= 0x32CF || c >= 0x3377 && c <= 0x337A || c >= 0x4DC0 && c <= 0x4DFF)
                return CharacterClass.Other_Neutral;
            if (c >= 0xA490 && c <= 0xA4C6 || c >= 0xA60D && c <= 0xA60F || c >= 0xA700 && c <= 0xA721 || c >= 0xA828 && c <= 0xA82B || c >= 0xA874 && c <= 0xA877)
                return CharacterClass.Other_Neutral;
            if (c >= 0xA66F && c <= 0xA672 || c >= 0xA674 && c <= 0xA67D || c >= 0xA8E0 && c <= 0xA8F1 || c >= 0xA926 && c <= 0xA92D || c >= 0xA947 && c <= 0xA951 || c >= 0xA980 && c <= 0xA982 || c >= 0xA9B6 && c <= 0xA9B9 || c >= 0xAA29 && c <= 0xAA2E || c >= 0xAAB2 && c <= 0xAAB4)
                return CharacterClass.Nonspacing_Mark;
            if (c >= 0xFE00 && c <= 0xFE0F || c >= 0xFE20 && c <= 0xFE2F)
                return CharacterClass.Nonspacing_Mark;
            if (c >= 0xFE10 && c <= 0xFE19 || c >= 0xFE30 && c <= 0xFE4F || c >= 0xFE56 && c <= 0xFE6B)
                return CharacterClass.Other_Neutral;
            if (c >= 0xFF01 && c <= 0xFF0A)
                return CharacterClass.Other_Neutral;
            if (c >= 0xFF10 && c <= 0xFF19)
                return CharacterClass.European_Number;
            if (c >= 0xFF1B && c <= 0xFF20 || c >= 0xFF3B && c <= 0xFF40 || c >= 0xFF5B && c <= 0xFF65 || c >= 0xFFE2 && c <= 0xFFEE)
                return CharacterClass.Other_Neutral;
            if (c >= 0xFFF0 && c <= 0xFFF8)
                return CharacterClass.Boundary_Neutral;
            if (c >= 0xFFF9 && c <= 0xFFFD)
                return CharacterClass.Other_Neutral;
            return CharacterClass.Left_To_Right;
        }

        private static void ChangeDirection(Int32[] array, Int32 start, Int32 endInclusive)
        {
            Int32 count = (endInclusive - start + 1) / 2;
            Int32 tmp;
            for (Int32 i = 0; i < count; i++)
            {
                tmp = array[start + i];
                array[start + i] = array[endInclusive - i];
                array[endInclusive - i] = tmp;
            }
        }

        // Only supports (most) common and arabic characters
        private static Boolean IsJoinTransparent(Char c)
        {
            if (c == 0x00AD || c >= 0x0300 && c <= 0x036F || c >= 0x0483 && c <= 0x0489 || c >= 0x0591 && c <= 0x05BD)
                return true;
            if (c >= 0x0610 && c <= 0x061A || c == 0x061C || c >= 0x064B && c <= 0x065F || c == 0x0670 || c >= 0x06D6 && c <= 0x06DC || c >= 0x06DF && c <= 0x06E4 || c == 0x06E7 || c == 0x06E8 || c >= 0x06EA && c <= 0x06ED)
                return true;
            if (c >= 0x1AB0 && c <= 0x1ACE || c >= 0x1DC0 && c <= 0x1DFF || c == 0x200B || c == 0x200E || c == 0x200F || c >= 0x202A && c <= 0x202E || c >= 0x2060 && c <= 0x2064 || c >= 0x206A && c <= 0x206F || c == 0xFEFF || c == 0xFEFF || c >= 0xFFF9 && c <= 0xFFFB)
                return true;
            return false;
        }

        // Note: this dictionary is not even initialized if BIDI is never used (no call to constructor nor "GetBIDIClass")
        // Use of BIDI enum types don't trigger its static initialization
        private static Dictionary<Char, CharacterClass> SingleCharClass = new Dictionary<Char, CharacterClass>()
        {
            { (Char)0x093A, CharacterClass.Nonspacing_Mark },
            { (Char)0x093C, CharacterClass.Nonspacing_Mark },
            { (Char)0x094D, CharacterClass.Nonspacing_Mark },
            { (Char)0x0962, CharacterClass.Nonspacing_Mark },
            { (Char)0x0963, CharacterClass.Nonspacing_Mark },
            { (Char)0x0981, CharacterClass.Nonspacing_Mark },
            { (Char)0x09BC, CharacterClass.Nonspacing_Mark },
            { (Char)0x09CD, CharacterClass.Nonspacing_Mark },
            { (Char)0x09E2, CharacterClass.Nonspacing_Mark },
            { (Char)0x09E3, CharacterClass.Nonspacing_Mark },
            { (Char)0x09F2, CharacterClass.European_Terminator },
            { (Char)0x09F3, CharacterClass.European_Terminator },
            { (Char)0x09FB, CharacterClass.European_Terminator },
            { (Char)0x09FE, CharacterClass.Nonspacing_Mark },
            { (Char)0x0A01, CharacterClass.Nonspacing_Mark },
            { (Char)0x0A02, CharacterClass.Nonspacing_Mark },
            { (Char)0x0A3C, CharacterClass.Nonspacing_Mark },
            { (Char)0x0A70, CharacterClass.Nonspacing_Mark },
            { (Char)0x0A71, CharacterClass.Nonspacing_Mark },
            { (Char)0x0A75, CharacterClass.Nonspacing_Mark },
            { (Char)0x0A81, CharacterClass.Nonspacing_Mark },
            { (Char)0x0A82, CharacterClass.Nonspacing_Mark },
            { (Char)0x0ABC, CharacterClass.Nonspacing_Mark },
            { (Char)0x0ACD, CharacterClass.Nonspacing_Mark },
            { (Char)0x0AE2, CharacterClass.Nonspacing_Mark },
            { (Char)0x0AE3, CharacterClass.Nonspacing_Mark },
            { (Char)0x0AF1, CharacterClass.European_Terminator },
            { (Char)0x0B01, CharacterClass.Nonspacing_Mark },
            { (Char)0x0B3C, CharacterClass.Nonspacing_Mark },
            { (Char)0x0B3F, CharacterClass.Nonspacing_Mark },
            { (Char)0x0B4D, CharacterClass.Nonspacing_Mark },
            { (Char)0x0B55, CharacterClass.Nonspacing_Mark },
            { (Char)0x0B56, CharacterClass.Nonspacing_Mark },
            { (Char)0x0B62, CharacterClass.Nonspacing_Mark },
            { (Char)0x0B63, CharacterClass.Nonspacing_Mark },
            { (Char)0x0B82, CharacterClass.Nonspacing_Mark },
            { (Char)0x0BC0, CharacterClass.Nonspacing_Mark },
            { (Char)0x0BCD, CharacterClass.Nonspacing_Mark },
            { (Char)0x0BF9, CharacterClass.European_Terminator },
            { (Char)0x0C00, CharacterClass.Nonspacing_Mark },
            { (Char)0x0C04, CharacterClass.Nonspacing_Mark },
            { (Char)0x0C3C, CharacterClass.Nonspacing_Mark },
            { (Char)0x0C62, CharacterClass.Nonspacing_Mark },
            { (Char)0x0C63, CharacterClass.Nonspacing_Mark },
            { (Char)0x0C81, CharacterClass.Nonspacing_Mark },
            { (Char)0x0CBC, CharacterClass.Nonspacing_Mark },
            { (Char)0x0CCC, CharacterClass.Nonspacing_Mark },
            { (Char)0x0CCD, CharacterClass.Nonspacing_Mark },
            { (Char)0x0CE2, CharacterClass.Nonspacing_Mark },
            { (Char)0x0CE3, CharacterClass.Nonspacing_Mark },
            { (Char)0x0D00, CharacterClass.Nonspacing_Mark },
            { (Char)0x0D01, CharacterClass.Nonspacing_Mark },
            { (Char)0x0D3B, CharacterClass.Nonspacing_Mark },
            { (Char)0x0D3C, CharacterClass.Nonspacing_Mark },
            { (Char)0x0D4D, CharacterClass.Nonspacing_Mark },
            { (Char)0x0D62, CharacterClass.Nonspacing_Mark },
            { (Char)0x0D63, CharacterClass.Nonspacing_Mark },
            { (Char)0x0D81, CharacterClass.Nonspacing_Mark },
            { (Char)0x0DCA, CharacterClass.Nonspacing_Mark },
            { (Char)0x0E31, CharacterClass.Nonspacing_Mark },
            { (Char)0x0E3F, CharacterClass.European_Terminator },
            { (Char)0x0EB1, CharacterClass.Nonspacing_Mark },
            { (Char)0x0F18, CharacterClass.Nonspacing_Mark },
            { (Char)0x0F19, CharacterClass.Nonspacing_Mark },
            { (Char)0x0F35, CharacterClass.Nonspacing_Mark },
            { (Char)0x0F37, CharacterClass.Nonspacing_Mark },
            { (Char)0x0F39, CharacterClass.Nonspacing_Mark },
            { (Char)0x0FC6, CharacterClass.Nonspacing_Mark },
            { (Char)0x1039, CharacterClass.Nonspacing_Mark },
            { (Char)0x103A, CharacterClass.Nonspacing_Mark },
            { (Char)0x103D, CharacterClass.Nonspacing_Mark },
            { (Char)0x103E, CharacterClass.Nonspacing_Mark },
            { (Char)0x1058, CharacterClass.Nonspacing_Mark },
            { (Char)0x1059, CharacterClass.Nonspacing_Mark },
            { (Char)0x1082, CharacterClass.Nonspacing_Mark },
            { (Char)0x1085, CharacterClass.Nonspacing_Mark },
            { (Char)0x1086, CharacterClass.Nonspacing_Mark },
            { (Char)0x108D, CharacterClass.Nonspacing_Mark },
            { (Char)0x109D, CharacterClass.Nonspacing_Mark },
            { (Char)0x1400, CharacterClass.Other_Neutral },
            { (Char)0x1680, CharacterClass.White_Space },
            { (Char)0x169B, CharacterClass.Other_Neutral },
            { (Char)0x169C, CharacterClass.Other_Neutral },
            { (Char)0x1732, CharacterClass.Nonspacing_Mark },
            { (Char)0x1733, CharacterClass.Nonspacing_Mark },
            { (Char)0x1752, CharacterClass.Nonspacing_Mark },
            { (Char)0x1753, CharacterClass.Nonspacing_Mark },
            { (Char)0x1772, CharacterClass.Nonspacing_Mark },
            { (Char)0x1773, CharacterClass.Nonspacing_Mark },
            { (Char)0x17B4, CharacterClass.Nonspacing_Mark },
            { (Char)0x17B5, CharacterClass.Nonspacing_Mark },
            { (Char)0x17C6, CharacterClass.Nonspacing_Mark },
            { (Char)0x17DB, CharacterClass.European_Terminator },
            { (Char)0x17DD, CharacterClass.Nonspacing_Mark },
            { (Char)0x180E, CharacterClass.Boundary_Neutral },
            { (Char)0x180F, CharacterClass.Nonspacing_Mark },
            { (Char)0x1885, CharacterClass.Nonspacing_Mark },
            { (Char)0x1886, CharacterClass.Nonspacing_Mark },
            { (Char)0x18A9, CharacterClass.Nonspacing_Mark },
            { (Char)0x1927, CharacterClass.Nonspacing_Mark },
            { (Char)0x1928, CharacterClass.Nonspacing_Mark },
            { (Char)0x1932, CharacterClass.Nonspacing_Mark },
            { (Char)0x1940, CharacterClass.Other_Neutral },
            { (Char)0x1944, CharacterClass.Other_Neutral },
            { (Char)0x1945, CharacterClass.Other_Neutral },
            { (Char)0x1A17, CharacterClass.Nonspacing_Mark },
            { (Char)0x1A18, CharacterClass.Nonspacing_Mark },
            { (Char)0x1A1B, CharacterClass.Nonspacing_Mark },
            { (Char)0x1A56, CharacterClass.Nonspacing_Mark },
            { (Char)0x1A62, CharacterClass.Nonspacing_Mark },
            { (Char)0x1B34, CharacterClass.Nonspacing_Mark },
            { (Char)0x1B3C, CharacterClass.Nonspacing_Mark },
            { (Char)0x1B42, CharacterClass.Nonspacing_Mark },
            { (Char)0x1B80, CharacterClass.Nonspacing_Mark },
            { (Char)0x1B81, CharacterClass.Nonspacing_Mark },
            { (Char)0x1BA8, CharacterClass.Nonspacing_Mark },
            { (Char)0x1BA9, CharacterClass.Nonspacing_Mark },
            { (Char)0x1BE6, CharacterClass.Nonspacing_Mark },
            { (Char)0x1BE8, CharacterClass.Nonspacing_Mark },
            { (Char)0x1BE9, CharacterClass.Nonspacing_Mark },
            { (Char)0x1BED, CharacterClass.Nonspacing_Mark },
            { (Char)0x1C36, CharacterClass.Nonspacing_Mark },
            { (Char)0x1C37, CharacterClass.Nonspacing_Mark },
            { (Char)0x1CED, CharacterClass.Nonspacing_Mark },
            { (Char)0x1CF4, CharacterClass.Nonspacing_Mark },
            { (Char)0x1CF8, CharacterClass.Nonspacing_Mark },
            { (Char)0x1CF9, CharacterClass.Nonspacing_Mark },
            { (Char)0x1FBD, CharacterClass.Other_Neutral },
            { (Char)0x200F, CharacterClass.Right_To_Left },
            { (Char)0x2028, CharacterClass.White_Space },
            { (Char)0x2029, CharacterClass.Paragraph_Separator },
            { (Char)0x202A, CharacterClass.Left_To_Right_Embedding },
            { (Char)0x202B, CharacterClass.Right_To_Left_Embedding },
            { (Char)0x202C, CharacterClass.Pop_Directional_Format },
            { (Char)0x202D, CharacterClass.Left_To_Right_Override },
            { (Char)0x202E, CharacterClass.Right_To_Left_Override },
            { (Char)0x202F, CharacterClass.Common_Separator },
            { (Char)0x2044, CharacterClass.Common_Separator },
            { (Char)0x205F, CharacterClass.White_Space },
            { (Char)0x2066, CharacterClass.Left_To_Right_Isolate },
            { (Char)0x2067, CharacterClass.Right_To_Left_Isolate },
            { (Char)0x2068, CharacterClass.First_Strong_Isolate },
            { (Char)0x2069, CharacterClass.Pop_Directional_Isolate },
            { (Char)0x207A, CharacterClass.European_Separator },
            { (Char)0x207B, CharacterClass.European_Separator },
            { (Char)0x207C, CharacterClass.Other_Neutral },
            { (Char)0x207D, CharacterClass.Other_Neutral },
            { (Char)0x207E, CharacterClass.Other_Neutral },
            { (Char)0x208A, CharacterClass.European_Separator },
            { (Char)0x208B, CharacterClass.European_Separator },
            { (Char)0x208C, CharacterClass.Other_Neutral },
            { (Char)0x208D, CharacterClass.Other_Neutral },
            { (Char)0x208E, CharacterClass.Other_Neutral },
            { (Char)0x2100, CharacterClass.Other_Neutral },
            { (Char)0x2101, CharacterClass.Other_Neutral },
            { (Char)0x2108, CharacterClass.Other_Neutral },
            { (Char)0x2109, CharacterClass.Other_Neutral },
            { (Char)0x2114, CharacterClass.Other_Neutral },
            { (Char)0x2125, CharacterClass.Other_Neutral },
            { (Char)0x2127, CharacterClass.Other_Neutral },
            { (Char)0x2129, CharacterClass.Other_Neutral },
            { (Char)0x212E, CharacterClass.European_Terminator },
            { (Char)0x213A, CharacterClass.Other_Neutral },
            { (Char)0x213B, CharacterClass.Other_Neutral },
            { (Char)0x2212, CharacterClass.European_Separator },
            { (Char)0x2213, CharacterClass.European_Terminator },
            { (Char)0x2D7F, CharacterClass.Nonspacing_Mark },
            { (Char)0x3000, CharacterClass.White_Space },
            { (Char)0x3030, CharacterClass.Other_Neutral },
            { (Char)0x3036, CharacterClass.Other_Neutral },
            { (Char)0x3037, CharacterClass.Other_Neutral },
            { (Char)0x3099, CharacterClass.Nonspacing_Mark },
            { (Char)0x309A, CharacterClass.Nonspacing_Mark },
            { (Char)0x309B, CharacterClass.Other_Neutral },
            { (Char)0x309C, CharacterClass.Other_Neutral },
            { (Char)0x30A0, CharacterClass.Other_Neutral },
            { (Char)0x30FB, CharacterClass.Other_Neutral },
            { (Char)0x31EF, CharacterClass.Other_Neutral },
            { (Char)0x321D, CharacterClass.Other_Neutral },
            { (Char)0x321E, CharacterClass.Other_Neutral },
            { (Char)0x33DE, CharacterClass.Other_Neutral },
            { (Char)0x33DF, CharacterClass.Other_Neutral },
            { (Char)0x33FF, CharacterClass.Other_Neutral },
            { (Char)0xA673, CharacterClass.Other_Neutral },
            { (Char)0xA67E, CharacterClass.Other_Neutral },
            { (Char)0xA67F, CharacterClass.Other_Neutral },
            { (Char)0xA69E, CharacterClass.Nonspacing_Mark },
            { (Char)0xA69F, CharacterClass.Nonspacing_Mark },
            { (Char)0xA6F0, CharacterClass.Nonspacing_Mark },
            { (Char)0xA6F1, CharacterClass.Nonspacing_Mark },
            { (Char)0xA788, CharacterClass.Other_Neutral },
            { (Char)0xA802, CharacterClass.Nonspacing_Mark },
            { (Char)0xA806, CharacterClass.Nonspacing_Mark },
            { (Char)0xA80B, CharacterClass.Nonspacing_Mark },
            { (Char)0xA825, CharacterClass.Nonspacing_Mark },
            { (Char)0xA826, CharacterClass.Nonspacing_Mark },
            { (Char)0xA82C, CharacterClass.Nonspacing_Mark },
            { (Char)0xA838, CharacterClass.European_Terminator },
            { (Char)0xA839, CharacterClass.European_Terminator },
            { (Char)0xA8C4, CharacterClass.Nonspacing_Mark },
            { (Char)0xA8C5, CharacterClass.Nonspacing_Mark },
            { (Char)0xA8FF, CharacterClass.Nonspacing_Mark },
            { (Char)0xA9B3, CharacterClass.Nonspacing_Mark },
            { (Char)0xA9BC, CharacterClass.Nonspacing_Mark },
            { (Char)0xA9BD, CharacterClass.Nonspacing_Mark },
            { (Char)0xA9E5, CharacterClass.Nonspacing_Mark },
            { (Char)0xAA31, CharacterClass.Nonspacing_Mark },
            { (Char)0xAA32, CharacterClass.Nonspacing_Mark },
            { (Char)0xAA35, CharacterClass.Nonspacing_Mark },
            { (Char)0xAA36, CharacterClass.Nonspacing_Mark },
            { (Char)0xAA43, CharacterClass.Nonspacing_Mark },
            { (Char)0xAA4C, CharacterClass.Nonspacing_Mark },
            { (Char)0xAA7C, CharacterClass.Nonspacing_Mark },
            { (Char)0xAAB0, CharacterClass.Nonspacing_Mark },
            { (Char)0xAAB7, CharacterClass.Nonspacing_Mark },
            { (Char)0xAAB8, CharacterClass.Nonspacing_Mark },
            { (Char)0xAABE, CharacterClass.Nonspacing_Mark },
            { (Char)0xAABF, CharacterClass.Nonspacing_Mark },
            { (Char)0xAAC1, CharacterClass.Nonspacing_Mark },
            { (Char)0xAAEC, CharacterClass.Nonspacing_Mark },
            { (Char)0xAAED, CharacterClass.Nonspacing_Mark },
            { (Char)0xAAF6, CharacterClass.Nonspacing_Mark },
            { (Char)0xAB6A, CharacterClass.Other_Neutral },
            { (Char)0xAB6B, CharacterClass.Other_Neutral },
            { (Char)0xABE5, CharacterClass.Nonspacing_Mark },
            { (Char)0xABE8, CharacterClass.Nonspacing_Mark },
            { (Char)0xABED, CharacterClass.Nonspacing_Mark },
            //{ (Char)0xFB29, CharacterClass.European_Separator },
            { (Char)0xFE50, CharacterClass.Common_Separator },
            { (Char)0xFE51, CharacterClass.Other_Neutral },
            { (Char)0xFE52, CharacterClass.Common_Separator },
            { (Char)0xFE54, CharacterClass.Other_Neutral },
            { (Char)0xFE55, CharacterClass.Common_Separator },
            { (Char)0xFE5F, CharacterClass.European_Terminator },
            { (Char)0xFE62, CharacterClass.European_Separator },
            { (Char)0xFE63, CharacterClass.European_Separator },
            { (Char)0xFE69, CharacterClass.European_Terminator },
            { (Char)0xFE6A, CharacterClass.European_Terminator },
            { (Char)0xFF03, CharacterClass.European_Terminator },
            { (Char)0xFF04, CharacterClass.European_Terminator },
            { (Char)0xFF05, CharacterClass.European_Terminator },
            { (Char)0xFF0B, CharacterClass.European_Separator },
            { (Char)0xFF0C, CharacterClass.Common_Separator },
            { (Char)0xFF0D, CharacterClass.European_Separator },
            { (Char)0xFF0E, CharacterClass.Common_Separator },
            { (Char)0xFF0F, CharacterClass.Common_Separator },
            { (Char)0xFF1A, CharacterClass.Common_Separator },
            { (Char)0xFFE0, CharacterClass.European_Terminator },
            { (Char)0xFFE1, CharacterClass.European_Terminator },
            { (Char)0xFFE5, CharacterClass.European_Terminator },
            { (Char)0xFFE6, CharacterClass.European_Terminator },
            { (Char)0xFFFE, CharacterClass.Boundary_Neutral },
            { (Char)0xFFFF, CharacterClass.Boundary_Neutral }
        };

        private static Dictionary<Char, Char> MirroringCharacters = new Dictionary<Char, Char>()
        {
            { '(', ')' },
            { ')', '(' },
            { '<', '>' },
            { '>', '<' },
            //{ '[', ']' },
            //{ ']', '[' },
            { '{', '}' },
            { '}', '{' },
            { '«', '»' },
            { '»', '«' },
            { (Char)0x0F3A, (Char)0x0F3B },
            { (Char)0x0F3B, (Char)0x0F3A },
            { (Char)0x0F3C, (Char)0x0F3D },
            { (Char)0x0F3D, (Char)0x0F3C },
            { (Char)0x169B, (Char)0x169C },
            { (Char)0x169C, (Char)0x169B },
            { (Char)0x2039, (Char)0x203A },
            { (Char)0x203A, (Char)0x2039 },
            { (Char)0x2045, (Char)0x2046 },
            { (Char)0x2046, (Char)0x2045 },
            { (Char)0x207D, (Char)0x207E },
            { (Char)0x207E, (Char)0x207D },
            { (Char)0x208D, (Char)0x208E },
            { (Char)0x208E, (Char)0x208D },
            { (Char)0x2208, (Char)0x220B },
            { (Char)0x2209, (Char)0x220C },
            { (Char)0x220A, (Char)0x220D },
            { (Char)0x220B, (Char)0x2208 },
            { (Char)0x220C, (Char)0x2209 },
            { (Char)0x220D, (Char)0x220A },
            { (Char)0x2215, (Char)0x29F5 },
            { (Char)0x221F, (Char)0x2BFE },
            { (Char)0x2220, (Char)0x29A3 },
            { (Char)0x2221, (Char)0x299B },
            { (Char)0x2222, (Char)0x29A0 },
            { (Char)0x2224, (Char)0x2AEE },
            { (Char)0x223C, (Char)0x223D },
            { (Char)0x223D, (Char)0x223C },
            { (Char)0x2243, (Char)0x22CD },
            { (Char)0x2245, (Char)0x224C },
            { (Char)0x224C, (Char)0x2245 },
            { (Char)0x2252, (Char)0x2253 },
            { (Char)0x2253, (Char)0x2252 },
            { (Char)0x2254, (Char)0x2255 },
            { (Char)0x2255, (Char)0x2254 },
            { (Char)0x2264, (Char)0x2265 },
            { (Char)0x2265, (Char)0x2264 },
            { (Char)0x2266, (Char)0x2267 },
            { (Char)0x2267, (Char)0x2266 },
            { (Char)0x2268, (Char)0x2269 },
            { (Char)0x2269, (Char)0x2268 },
            { (Char)0x226A, (Char)0x226B },
            { (Char)0x226B, (Char)0x226A },
            { (Char)0x226E, (Char)0x226F },
            { (Char)0x226F, (Char)0x226E },
            { (Char)0x2270, (Char)0x2271 },
            { (Char)0x2271, (Char)0x2270 },
            { (Char)0x2272, (Char)0x2273 },
            { (Char)0x2273, (Char)0x2272 },
            { (Char)0x2274, (Char)0x2275 },
            { (Char)0x2275, (Char)0x2274 },
            { (Char)0x2276, (Char)0x2277 },
            { (Char)0x2277, (Char)0x2276 },
            { (Char)0x2278, (Char)0x2279 },
            { (Char)0x2279, (Char)0x2278 },
            { (Char)0x227A, (Char)0x227B },
            { (Char)0x227B, (Char)0x227A },
            { (Char)0x227C, (Char)0x227D },
            { (Char)0x227D, (Char)0x227C },
            { (Char)0x227E, (Char)0x227F },
            { (Char)0x227F, (Char)0x227E },
            { (Char)0x2280, (Char)0x2281 },
            { (Char)0x2281, (Char)0x2280 },
            { (Char)0x2282, (Char)0x2283 },
            { (Char)0x2283, (Char)0x2282 },
            { (Char)0x2284, (Char)0x2285 },
            { (Char)0x2285, (Char)0x2284 },
            { (Char)0x2286, (Char)0x2287 },
            { (Char)0x2287, (Char)0x2286 },
            { (Char)0x2288, (Char)0x2289 },
            { (Char)0x2289, (Char)0x2288 },
            { (Char)0x228A, (Char)0x228B },
            { (Char)0x228B, (Char)0x228A },
            { (Char)0x228F, (Char)0x2290 },
            { (Char)0x2290, (Char)0x228F },
            { (Char)0x2291, (Char)0x2292 },
            { (Char)0x2292, (Char)0x2291 },
            { (Char)0x2298, (Char)0x29B8 },
            { (Char)0x22A2, (Char)0x22A3 },
            { (Char)0x22A3, (Char)0x22A2 },
            { (Char)0x22A6, (Char)0x2ADE },
            { (Char)0x22A8, (Char)0x2AE4 },
            { (Char)0x22A9, (Char)0x2AE3 },
            { (Char)0x22AB, (Char)0x2AE5 },
            { (Char)0x22B0, (Char)0x22B1 },
            { (Char)0x22B1, (Char)0x22B0 },
            { (Char)0x22B2, (Char)0x22B3 },
            { (Char)0x22B3, (Char)0x22B2 },
            { (Char)0x22B4, (Char)0x22B5 },
            { (Char)0x22B5, (Char)0x22B4 },
            { (Char)0x22B6, (Char)0x22B7 },
            { (Char)0x22B7, (Char)0x22B6 },
            { (Char)0x22B8, (Char)0x27DC },
            { (Char)0x22C9, (Char)0x22CA },
            { (Char)0x22CA, (Char)0x22C9 },
            { (Char)0x22CB, (Char)0x22CC },
            { (Char)0x22CC, (Char)0x22CB },
            { (Char)0x22CD, (Char)0x2243 },
            { (Char)0x22D0, (Char)0x22D1 },
            { (Char)0x22D1, (Char)0x22D0 },
            { (Char)0x22D6, (Char)0x22D7 },
            { (Char)0x22D7, (Char)0x22D6 },
            { (Char)0x22D8, (Char)0x22D9 },
            { (Char)0x22D9, (Char)0x22D8 },
            { (Char)0x22DA, (Char)0x22DB },
            { (Char)0x22DB, (Char)0x22DA },
            { (Char)0x22DC, (Char)0x22DD },
            { (Char)0x22DD, (Char)0x22DC },
            { (Char)0x22DE, (Char)0x22DF },
            { (Char)0x22DF, (Char)0x22DE },
            { (Char)0x22E0, (Char)0x22E1 },
            { (Char)0x22E1, (Char)0x22E0 },
            { (Char)0x22E2, (Char)0x22E3 },
            { (Char)0x22E3, (Char)0x22E2 },
            { (Char)0x22E4, (Char)0x22E5 },
            { (Char)0x22E5, (Char)0x22E4 },
            { (Char)0x22E6, (Char)0x22E7 },
            { (Char)0x22E7, (Char)0x22E6 },
            { (Char)0x22E8, (Char)0x22E9 },
            { (Char)0x22E9, (Char)0x22E8 },
            { (Char)0x22EA, (Char)0x22EB },
            { (Char)0x22EB, (Char)0x22EA },
            { (Char)0x22EC, (Char)0x22ED },
            { (Char)0x22ED, (Char)0x22EC },
            { (Char)0x22F0, (Char)0x22F1 },
            { (Char)0x22F1, (Char)0x22F0 },
            { (Char)0x22F2, (Char)0x22FA },
            { (Char)0x22F3, (Char)0x22FB },
            { (Char)0x22F4, (Char)0x22FC },
            { (Char)0x22F6, (Char)0x22FD },
            { (Char)0x22F7, (Char)0x22FE },
            { (Char)0x22FA, (Char)0x22F2 },
            { (Char)0x22FB, (Char)0x22F3 },
            { (Char)0x22FC, (Char)0x22F4 },
            { (Char)0x22FD, (Char)0x22F6 },
            { (Char)0x22FE, (Char)0x22F7 },
            { (Char)0x2308, (Char)0x2309 },
            { (Char)0x2309, (Char)0x2308 },
            { (Char)0x230A, (Char)0x230B },
            { (Char)0x230B, (Char)0x230A },
            { (Char)0x2329, (Char)0x232A },
            { (Char)0x232A, (Char)0x2329 },
            { (Char)0x2768, (Char)0x2769 },
            { (Char)0x2769, (Char)0x2768 },
            { (Char)0x276A, (Char)0x276B },
            { (Char)0x276B, (Char)0x276A },
            { (Char)0x276C, (Char)0x276D },
            { (Char)0x276D, (Char)0x276C },
            { (Char)0x276E, (Char)0x276F },
            { (Char)0x276F, (Char)0x276E },
            { (Char)0x2770, (Char)0x2771 },
            { (Char)0x2771, (Char)0x2770 },
            { (Char)0x2772, (Char)0x2773 },
            { (Char)0x2773, (Char)0x2772 },
            { (Char)0x2774, (Char)0x2775 },
            { (Char)0x2775, (Char)0x2774 },
            { (Char)0x27C3, (Char)0x27C4 },
            { (Char)0x27C4, (Char)0x27C3 },
            { (Char)0x27C5, (Char)0x27C6 },
            { (Char)0x27C6, (Char)0x27C5 },
            { (Char)0x27C8, (Char)0x27C9 },
            { (Char)0x27C9, (Char)0x27C8 },
            { (Char)0x27CB, (Char)0x27CD },
            { (Char)0x27CD, (Char)0x27CB },
            { (Char)0x27D5, (Char)0x27D6 },
            { (Char)0x27D6, (Char)0x27D5 },
            { (Char)0x27DC, (Char)0x22B8 },
            { (Char)0x27DD, (Char)0x27DE },
            { (Char)0x27DE, (Char)0x27DD },
            { (Char)0x27E2, (Char)0x27E3 },
            { (Char)0x27E3, (Char)0x27E2 },
            { (Char)0x27E4, (Char)0x27E5 },
            { (Char)0x27E5, (Char)0x27E4 },
            { (Char)0x27E6, (Char)0x27E7 },
            { (Char)0x27E7, (Char)0x27E6 },
            { (Char)0x27E8, (Char)0x27E9 },
            { (Char)0x27E9, (Char)0x27E8 },
            { (Char)0x27EA, (Char)0x27EB },
            { (Char)0x27EB, (Char)0x27EA },
            { (Char)0x27EC, (Char)0x27ED },
            { (Char)0x27ED, (Char)0x27EC },
            { (Char)0x27EE, (Char)0x27EF },
            { (Char)0x27EF, (Char)0x27EE },
            { (Char)0x2983, (Char)0x2984 },
            { (Char)0x2984, (Char)0x2983 },
            { (Char)0x2985, (Char)0x2986 },
            { (Char)0x2986, (Char)0x2985 },
            { (Char)0x2987, (Char)0x2988 },
            { (Char)0x2988, (Char)0x2987 },
            { (Char)0x2989, (Char)0x298A },
            { (Char)0x298A, (Char)0x2989 },
            { (Char)0x298B, (Char)0x298C },
            { (Char)0x298C, (Char)0x298B },
            { (Char)0x298D, (Char)0x2990 },
            { (Char)0x298E, (Char)0x298F },
            { (Char)0x298F, (Char)0x298E },
            { (Char)0x2990, (Char)0x298D },
            { (Char)0x2991, (Char)0x2992 },
            { (Char)0x2992, (Char)0x2991 },
            { (Char)0x2993, (Char)0x2994 },
            { (Char)0x2994, (Char)0x2993 },
            { (Char)0x2995, (Char)0x2996 },
            { (Char)0x2996, (Char)0x2995 },
            { (Char)0x2997, (Char)0x2998 },
            { (Char)0x2998, (Char)0x2997 },
            { (Char)0x299B, (Char)0x2221 },
            { (Char)0x29A0, (Char)0x2222 },
            { (Char)0x29A3, (Char)0x2220 },
            { (Char)0x29A4, (Char)0x29A5 },
            { (Char)0x29A5, (Char)0x29A4 },
            { (Char)0x29A8, (Char)0x29A9 },
            { (Char)0x29A9, (Char)0x29A8 },
            { (Char)0x29AA, (Char)0x29AB },
            { (Char)0x29AB, (Char)0x29AA },
            { (Char)0x29AC, (Char)0x29AD },
            { (Char)0x29AD, (Char)0x29AC },
            { (Char)0x29AE, (Char)0x29AF },
            { (Char)0x29AF, (Char)0x29AE },
            { (Char)0x29B8, (Char)0x2298 },
            { (Char)0x29C0, (Char)0x29C1 },
            { (Char)0x29C1, (Char)0x29C0 },
            { (Char)0x29C4, (Char)0x29C5 },
            { (Char)0x29C5, (Char)0x29C4 },
            { (Char)0x29CF, (Char)0x29D0 },
            { (Char)0x29D0, (Char)0x29CF },
            { (Char)0x29D1, (Char)0x29D2 },
            { (Char)0x29D2, (Char)0x29D1 },
            { (Char)0x29D4, (Char)0x29D5 },
            { (Char)0x29D5, (Char)0x29D4 },
            { (Char)0x29D8, (Char)0x29D9 },
            { (Char)0x29D9, (Char)0x29D8 },
            { (Char)0x29DA, (Char)0x29DB },
            { (Char)0x29DB, (Char)0x29DA },
            { (Char)0x29E8, (Char)0x29E9 },
            { (Char)0x29E9, (Char)0x29E8 },
            { (Char)0x29F5, (Char)0x2215 },
            { (Char)0x29F8, (Char)0x29F9 },
            { (Char)0x29F9, (Char)0x29F8 },
            { (Char)0x29FC, (Char)0x29FD },
            { (Char)0x29FD, (Char)0x29FC },
            { (Char)0x2A2B, (Char)0x2A2C },
            { (Char)0x2A2C, (Char)0x2A2B },
            { (Char)0x2A2D, (Char)0x2A2E },
            { (Char)0x2A2E, (Char)0x2A2D },
            { (Char)0x2A34, (Char)0x2A35 },
            { (Char)0x2A35, (Char)0x2A34 },
            { (Char)0x2A3C, (Char)0x2A3D },
            { (Char)0x2A3D, (Char)0x2A3C },
            { (Char)0x2A64, (Char)0x2A65 },
            { (Char)0x2A65, (Char)0x2A64 },
            { (Char)0x2A79, (Char)0x2A7A },
            { (Char)0x2A7A, (Char)0x2A79 },
            { (Char)0x2A7B, (Char)0x2A7C },
            { (Char)0x2A7C, (Char)0x2A7B },
            { (Char)0x2A7D, (Char)0x2A7E },
            { (Char)0x2A7E, (Char)0x2A7D },
            { (Char)0x2A7F, (Char)0x2A80 },
            { (Char)0x2A80, (Char)0x2A7F },
            { (Char)0x2A81, (Char)0x2A82 },
            { (Char)0x2A82, (Char)0x2A81 },
            { (Char)0x2A83, (Char)0x2A84 },
            { (Char)0x2A84, (Char)0x2A83 },
            { (Char)0x2A85, (Char)0x2A86 },
            { (Char)0x2A86, (Char)0x2A85 },
            { (Char)0x2A87, (Char)0x2A88 },
            { (Char)0x2A88, (Char)0x2A87 },
            { (Char)0x2A89, (Char)0x2A8A },
            { (Char)0x2A8A, (Char)0x2A89 },
            { (Char)0x2A8B, (Char)0x2A8C },
            { (Char)0x2A8C, (Char)0x2A8B },
            { (Char)0x2A8D, (Char)0x2A8E },
            { (Char)0x2A8E, (Char)0x2A8D },
            { (Char)0x2A8F, (Char)0x2A90 },
            { (Char)0x2A90, (Char)0x2A8F },
            { (Char)0x2A91, (Char)0x2A92 },
            { (Char)0x2A92, (Char)0x2A91 },
            { (Char)0x2A93, (Char)0x2A94 },
            { (Char)0x2A94, (Char)0x2A93 },
            { (Char)0x2A95, (Char)0x2A96 },
            { (Char)0x2A96, (Char)0x2A95 },
            { (Char)0x2A97, (Char)0x2A98 },
            { (Char)0x2A98, (Char)0x2A97 },
            { (Char)0x2A99, (Char)0x2A9A },
            { (Char)0x2A9A, (Char)0x2A99 },
            { (Char)0x2A9B, (Char)0x2A9C },
            { (Char)0x2A9C, (Char)0x2A9B },
            { (Char)0x2A9D, (Char)0x2A9E },
            { (Char)0x2A9E, (Char)0x2A9D },
            { (Char)0x2A9F, (Char)0x2AA0 },
            { (Char)0x2AA0, (Char)0x2A9F },
            { (Char)0x2AA1, (Char)0x2AA2 },
            { (Char)0x2AA2, (Char)0x2AA1 },
            { (Char)0x2AA6, (Char)0x2AA7 },
            { (Char)0x2AA7, (Char)0x2AA6 },
            { (Char)0x2AA8, (Char)0x2AA9 },
            { (Char)0x2AA9, (Char)0x2AA8 },
            { (Char)0x2AAA, (Char)0x2AAB },
            { (Char)0x2AAB, (Char)0x2AAA },
            { (Char)0x2AAC, (Char)0x2AAD },
            { (Char)0x2AAD, (Char)0x2AAC },
            { (Char)0x2AAF, (Char)0x2AB0 },
            { (Char)0x2AB0, (Char)0x2AAF },
            { (Char)0x2AB1, (Char)0x2AB2 },
            { (Char)0x2AB2, (Char)0x2AB1 },
            { (Char)0x2AB3, (Char)0x2AB4 },
            { (Char)0x2AB4, (Char)0x2AB3 },
            { (Char)0x2AB5, (Char)0x2AB6 },
            { (Char)0x2AB6, (Char)0x2AB5 },
            { (Char)0x2AB7, (Char)0x2AB8 },
            { (Char)0x2AB8, (Char)0x2AB7 },
            { (Char)0x2AB9, (Char)0x2ABA },
            { (Char)0x2ABA, (Char)0x2AB9 },
            { (Char)0x2ABB, (Char)0x2ABC },
            { (Char)0x2ABC, (Char)0x2ABB },
            { (Char)0x2ABD, (Char)0x2ABE },
            { (Char)0x2ABE, (Char)0x2ABD },
            { (Char)0x2ABF, (Char)0x2AC0 },
            { (Char)0x2AC0, (Char)0x2ABF },
            { (Char)0x2AC1, (Char)0x2AC2 },
            { (Char)0x2AC2, (Char)0x2AC1 },
            { (Char)0x2AC3, (Char)0x2AC4 },
            { (Char)0x2AC4, (Char)0x2AC3 },
            { (Char)0x2AC5, (Char)0x2AC6 },
            { (Char)0x2AC6, (Char)0x2AC5 },
            { (Char)0x2AC7, (Char)0x2AC8 },
            { (Char)0x2AC8, (Char)0x2AC7 },
            { (Char)0x2AC9, (Char)0x2ACA },
            { (Char)0x2ACA, (Char)0x2AC9 },
            { (Char)0x2ACB, (Char)0x2ACC },
            { (Char)0x2ACC, (Char)0x2ACB },
            { (Char)0x2ACD, (Char)0x2ACE },
            { (Char)0x2ACE, (Char)0x2ACD },
            { (Char)0x2ACF, (Char)0x2AD0 },
            { (Char)0x2AD0, (Char)0x2ACF },
            { (Char)0x2AD1, (Char)0x2AD2 },
            { (Char)0x2AD2, (Char)0x2AD1 },
            { (Char)0x2AD3, (Char)0x2AD4 },
            { (Char)0x2AD4, (Char)0x2AD3 },
            { (Char)0x2AD5, (Char)0x2AD6 },
            { (Char)0x2AD6, (Char)0x2AD5 },
            { (Char)0x2ADE, (Char)0x22A6 },
            { (Char)0x2AE3, (Char)0x22A9 },
            { (Char)0x2AE4, (Char)0x22A8 },
            { (Char)0x2AE5, (Char)0x22AB },
            { (Char)0x2AEC, (Char)0x2AED },
            { (Char)0x2AED, (Char)0x2AEC },
            { (Char)0x2AEE, (Char)0x2224 },
            { (Char)0x2AF7, (Char)0x2AF8 },
            { (Char)0x2AF8, (Char)0x2AF7 },
            { (Char)0x2AF9, (Char)0x2AFA },
            { (Char)0x2AFA, (Char)0x2AF9 },
            { (Char)0x2BFE, (Char)0x221F },
            { (Char)0x2E02, (Char)0x2E03 },
            { (Char)0x2E03, (Char)0x2E02 },
            { (Char)0x2E04, (Char)0x2E05 },
            { (Char)0x2E05, (Char)0x2E04 },
            { (Char)0x2E09, (Char)0x2E0A },
            { (Char)0x2E0A, (Char)0x2E09 },
            { (Char)0x2E0C, (Char)0x2E0D },
            { (Char)0x2E0D, (Char)0x2E0C },
            { (Char)0x2E1C, (Char)0x2E1D },
            { (Char)0x2E1D, (Char)0x2E1C },
            { (Char)0x2E20, (Char)0x2E21 },
            { (Char)0x2E21, (Char)0x2E20 },
            { (Char)0x2E22, (Char)0x2E23 },
            { (Char)0x2E23, (Char)0x2E22 },
            { (Char)0x2E24, (Char)0x2E25 },
            { (Char)0x2E25, (Char)0x2E24 },
            { (Char)0x2E26, (Char)0x2E27 },
            { (Char)0x2E27, (Char)0x2E26 },
            { (Char)0x2E28, (Char)0x2E29 },
            { (Char)0x2E29, (Char)0x2E28 },
            { (Char)0x2E55, (Char)0x2E56 },
            { (Char)0x2E56, (Char)0x2E55 },
            { (Char)0x2E57, (Char)0x2E58 },
            { (Char)0x2E58, (Char)0x2E57 },
            { (Char)0x2E59, (Char)0x2E5A },
            { (Char)0x2E5A, (Char)0x2E59 },
            { (Char)0x2E5B, (Char)0x2E5C },
            { (Char)0x2E5C, (Char)0x2E5B },
            { (Char)0x3008, (Char)0x3009 },
            { (Char)0x3009, (Char)0x3008 },
            { (Char)0x300A, (Char)0x300B },
            { (Char)0x300B, (Char)0x300A },
            { (Char)0x300C, (Char)0x300D },
            { (Char)0x300D, (Char)0x300C },
            { (Char)0x300E, (Char)0x300F },
            { (Char)0x300F, (Char)0x300E },
            { (Char)0x3010, (Char)0x3011 },
            { (Char)0x3011, (Char)0x3010 },
            { (Char)0x3014, (Char)0x3015 },
            { (Char)0x3015, (Char)0x3014 },
            { (Char)0x3016, (Char)0x3017 },
            { (Char)0x3017, (Char)0x3016 },
            { (Char)0x3018, (Char)0x3019 },
            { (Char)0x3019, (Char)0x3018 },
            { (Char)0x301A, (Char)0x301B },
            { (Char)0x301B, (Char)0x301A },
            { (Char)0xFE59, (Char)0xFE5A },
            { (Char)0xFE5A, (Char)0xFE59 },
            { (Char)0xFE5B, (Char)0xFE5C },
            { (Char)0xFE5C, (Char)0xFE5B },
            { (Char)0xFE5D, (Char)0xFE5E },
            { (Char)0xFE5E, (Char)0xFE5D },
            { (Char)0xFE64, (Char)0xFE65 },
            { (Char)0xFE65, (Char)0xFE64 },
            { (Char)0xFF08, (Char)0xFF09 },
            { (Char)0xFF09, (Char)0xFF08 },
            { (Char)0xFF1C, (Char)0xFF1E },
            { (Char)0xFF1E, (Char)0xFF1C },
            { (Char)0xFF3B, (Char)0xFF3D },
            { (Char)0xFF3D, (Char)0xFF3B },
            { (Char)0xFF5B, (Char)0xFF5D },
            { (Char)0xFF5D, (Char)0xFF5B },
            { (Char)0xFF5F, (Char)0xFF60 },
            { (Char)0xFF60, (Char)0xFF5F },
            { (Char)0xFF62, (Char)0xFF63 },
            { (Char)0xFF63, (Char)0xFF62 }
        };

        private static HashSet<Char> MirroringCharactersNoEquivalent = new HashSet<Char>()
        {
            (Char)0x2140,
            (Char)0x2201,
            (Char)0x2202,
            (Char)0x2203,
            (Char)0x2204,
            (Char)0x2211,
            (Char)0x2216,
            (Char)0x221A,
            (Char)0x221B,
            (Char)0x221C,
            (Char)0x221D,
            (Char)0x2226,
            (Char)0x222B,
            (Char)0x222C,
            (Char)0x222D,
            (Char)0x222E,
            (Char)0x222F,
            (Char)0x2230,
            (Char)0x2231,
            (Char)0x2232,
            (Char)0x2233,
            (Char)0x2239,
            (Char)0x223B,
            (Char)0x223E,
            (Char)0x223F,
            (Char)0x2240,
            (Char)0x2241,
            (Char)0x2242,
            (Char)0x2244,
            (Char)0x2246,
            (Char)0x2247,
            (Char)0x2248,
            (Char)0x2249,
            (Char)0x224A,
            (Char)0x224B,
            (Char)0x225F,
            (Char)0x2260,
            (Char)0x2262,
            (Char)0x226D,
            (Char)0x228C,
            (Char)0x22A7,
            (Char)0x22AA,
            (Char)0x22AC,
            (Char)0x22AD,
            (Char)0x22AE,
            (Char)0x22AF,
            (Char)0x22BE,
            (Char)0x22BF,
            (Char)0x22F5,
            (Char)0x22F8,
            (Char)0x22F9,
            (Char)0x22FF,
            (Char)0x2320,
            (Char)0x2321,
            (Char)0x27C0,
            (Char)0x27CC,
            (Char)0x27D3,
            (Char)0x27D4,
            (Char)0x299C,
            (Char)0x299D,
            (Char)0x299E,
            (Char)0x299F,
            (Char)0x29A2,
            (Char)0x29A6,
            (Char)0x29A7,
            (Char)0x29C2,
            (Char)0x29C3,
            (Char)0x29C9,
            (Char)0x29CE,
            (Char)0x29DC,
            (Char)0x29E1,
            (Char)0x29E3,
            (Char)0x29E4,
            (Char)0x29E5,
            (Char)0x29F4,
            (Char)0x29F6,
            (Char)0x29F7,
            (Char)0x2A0A,
            (Char)0x2A0B,
            (Char)0x2A0C,
            (Char)0x2A0D,
            (Char)0x2A0E,
            (Char)0x2A0F,
            (Char)0x2A10,
            (Char)0x2A11,
            (Char)0x2A12,
            (Char)0x2A13,
            (Char)0x2A14,
            (Char)0x2A15,
            (Char)0x2A16,
            (Char)0x2A17,
            (Char)0x2A18,
            (Char)0x2A19,
            (Char)0x2A1A,
            (Char)0x2A1B,
            (Char)0x2A1C,
            (Char)0x2A1E,
            (Char)0x2A1F,
            (Char)0x2A20,
            (Char)0x2A21,
            (Char)0x2A24,
            (Char)0x2A26,
            (Char)0x2A29,
            (Char)0x2A3E,
            (Char)0x2A57,
            (Char)0x2A58,
            (Char)0x2A6A,
            (Char)0x2A6B,
            (Char)0x2A6C,
            (Char)0x2A6D,
            (Char)0x2A6F,
            (Char)0x2A70,
            (Char)0x2A73,
            (Char)0x2A74,
            (Char)0x2AA3,
            (Char)0x2ADC,
            (Char)0x2AE2,
            (Char)0x2AE6,
            (Char)0x2AF3,
            (Char)0x2AFB,
            (Char)0x2AFD
        };

        private static Dictionary<Char, Char> DualJoiningCharacters = new Dictionary<Char, Char>()
        {
            { (Char)0x0628, (Char)0xFE92 },
            { (Char)0x062A, (Char)0xFE98 },
            { (Char)0x062B, (Char)0xFE9C },
            { (Char)0x062C, (Char)0xFEA0 },
            { (Char)0x062D, (Char)0xFEA4 },
            { (Char)0x062E, (Char)0xFEA8 },
            { (Char)0x0633, (Char)0xFEB4 },
            { (Char)0x0634, (Char)0xFEB8 },
            { (Char)0x0635, (Char)0xFEBC },
            { (Char)0x0636, (Char)0xFEC0 },
            { (Char)0x0637, (Char)0xFEC4 },
            { (Char)0x0638, (Char)0xFEC8 },
            { (Char)0x0639, (Char)0xFECC },
            { (Char)0x063A, (Char)0xFED0 },
            { (Char)0x0641, (Char)0xFED4 },
            { (Char)0x0642, (Char)0xFED8 },
            { (Char)0x0643, (Char)0xFEDC },
            { (Char)0x0644, (Char)0xFEE0 },
            { (Char)0x0645, (Char)0xFEE4 },
            { (Char)0x0646, (Char)0xFEE8 },
            { (Char)0x0647, (Char)0xFEEC },
            { (Char)0x064A, (Char)0xFEF4 },
        };

        private static Dictionary<Char, Char> BeforeJoiningCharacters = new Dictionary<Char, Char>()
        {
            { (Char)0x0628, (Char)0xFE91 },
            { (Char)0x062A, (Char)0xFE97 },
            { (Char)0x062B, (Char)0xFE9B },
            { (Char)0x062C, (Char)0xFE9F },
            { (Char)0x062D, (Char)0xFEA3 },
            { (Char)0x062E, (Char)0xFEA7 },
            { (Char)0x0633, (Char)0xFEB3 },
            { (Char)0x0634, (Char)0xFEB7 },
            { (Char)0x0635, (Char)0xFEBB },
            { (Char)0x0636, (Char)0xFEBF },
            { (Char)0x0637, (Char)0xFEC3 },
            { (Char)0x0638, (Char)0xFEC7 },
            { (Char)0x0639, (Char)0xFECB },
            { (Char)0x063A, (Char)0xFECF },
            { (Char)0x0641, (Char)0xFED3 },
            { (Char)0x0642, (Char)0xFED7 },
            { (Char)0x0643, (Char)0xFEDB },
            { (Char)0x0644, (Char)0xFEDF },
            { (Char)0x0645, (Char)0xFEE3 },
            { (Char)0x0646, (Char)0xFEE7 },
            { (Char)0x0647, (Char)0xFEEB },
            { (Char)0x064A, (Char)0xFEF3 },
        };

        private static Dictionary<Char, Char> AfterJoiningCharacters = new Dictionary<Char, Char>()
        {
            { (Char)0x0627, (Char)0xFE8E },
            { (Char)0x0628, (Char)0xFE90 },
            { (Char)0x062A, (Char)0xFE96 },
            { (Char)0x062B, (Char)0xFE9A },
            { (Char)0x062C, (Char)0xFE9E },
            { (Char)0x062D, (Char)0xFEA2 },
            { (Char)0x062E, (Char)0xFEA6 },
            { (Char)0x062F, (Char)0xFEAA },
            { (Char)0x0630, (Char)0xFEAC },
            { (Char)0x0631, (Char)0xFEAE },
            { (Char)0x0632, (Char)0xFEB0 },
            { (Char)0x0633, (Char)0xFEB2 },
            { (Char)0x0634, (Char)0xFEB6 },
            { (Char)0x0635, (Char)0xFEBA },
            { (Char)0x0636, (Char)0xFEBE },
            { (Char)0x0637, (Char)0xFEC2 },
            { (Char)0x0638, (Char)0xFEC6 },
            { (Char)0x0639, (Char)0xFECA },
            { (Char)0x063A, (Char)0xFECE },
            { (Char)0x0641, (Char)0xFED2 },
            { (Char)0x0642, (Char)0xFED6 },
            { (Char)0x0643, (Char)0xFEDA },
            { (Char)0x0644, (Char)0xFEDE },
            { (Char)0x0645, (Char)0xFEE2 },
            { (Char)0x0646, (Char)0xFEE6 },
            { (Char)0x0647, (Char)0xFEEA },
            { (Char)0x0648, (Char)0xFEEE },
            { (Char)0x064A, (Char)0xFEF2 },
            { (Char)0x0622, (Char)0xFE82 },
            { (Char)0x0629, (Char)0xFE94 },
            { (Char)0x0649, (Char)0xFEF0 },
        };

        private static HashSet<Char> JoinCausingCharacters = new HashSet<Char>()
        {
            (Char)0x0640,
            (Char)0x07FA,
            (Char)0x0883,
            (Char)0x0884,
            (Char)0x0885,
            (Char)0x180A,
            (Char)0x200D
        };

        private static List<DirectionalStatus> DirectionalStackRoot = new List<DirectionalStatus>();

        private class DirectionalStatus
        {
            public Boolean ltr;
            public Boolean isIsolate;
            public Boolean isOverride;
            public Int32 overrideStatus;
            public Int32 start;
            public Int32 end;
            public List<DirectionalStatus> lowerLevels = new List<DirectionalStatus>();
            public DirectionalStatus parent = null;

            public static void Push(ref DirectionalStatus status, Int32 pos, Boolean leftToRight, Int32 overrideState, Boolean isolate, Boolean over)
            {
                DirectionalStatus nextState = new DirectionalStatus
                {
                    parent = status,
                    ltr = leftToRight,
                    overrideStatus = overrideState,
                    isIsolate = isolate,
                    isOverride = over,
                    start = pos
                };
                status.lowerLevels.Add(nextState);
                status = nextState;
            }

            public static void Pop(ref DirectionalStatus status, Int32 pos)
            {
                status.end = pos;
                status = status.parent;
            }

            public IEnumerable<DirectionalStatus> GetAllDirectionChanges(Boolean parentIsLTR)
            {
                if (parentIsLTR != ltr)
                    yield return this;
                foreach (DirectionalStatus status in lowerLevels)
                    foreach (DirectionalStatus lower in status.GetAllDirectionChanges(ltr))
                        yield return lower;
            }
        }

        private const Int32 OVERRIDE_STATUS_NEUTRAL = 0;
        private const Int32 OVERRIDE_STATUS_RTL = 1;
        private const Int32 OVERRIDE_STATUS_LTR = 2;

        public enum LanguageReadingDirection
        {
            // TODO: improve the support of reading directions
            LeftToRight,   // Supported
            RightToLeft,   // Partly supported
            Boustrophedon, // Not supported yet
            TopToBottom    // Not supported yet
        }

        public enum CharacterClass
        {
            // Strong types
            Left_To_Right,
            Right_To_Left,
            Arabic_Letter,
            // Weak types
            European_Number,
            European_Separator,
            European_Terminator,
            Arabic_Number,
            Common_Separator,
            Nonspacing_Mark,
            Boundary_Neutral,
            // Neutral types
            Paragraph_Separator,
            Segment_Separator,
            White_Space,
            Other_Neutral,
            // Explicit formatting types
            Left_To_Right_Embedding,
            Left_To_Right_Override,
            Right_To_Left_Embedding,
            Right_To_Left_Override,
            Pop_Directional_Format,
            Left_To_Right_Isolate,
            Right_To_Left_Isolate,
            First_Strong_Isolate,
            Pop_Directional_Isolate
        }
    }
}
