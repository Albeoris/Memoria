namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Switches an object's primitives to semi-transparent drawing with blend mode a2 (tpage bits 5..6). Object:
    /// +0 flags (0x10 = leave alone), +5 byte = last blend mode, +8 PSX address of a header whose byte 3 is the part
    /// count, +24 primitive stream, +32 bit mask of skipped parts. For every part whose bit is clear the stream is
    /// walked up to the 0xFF terminator: 0xE1 entries get the draw-mode word a2 | 0xE1000000, textured primitives get
    /// the semi-transparent code bit and their tpage blend bits replaced by a2.
    /// </summary>
    public static void PsxObjectSetBlendMode(long a1, ushort a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxObjectSetBlendMode(a1, a2)", a1, a2);
        Shape_PsgBuildBoneMatrices_a1* shape_PsgBuildBoneMatrices_a1 = (Shape_PsgBuildBoneMatrices_a1*)a1;
        if ((shape_PsgBuildBoneMatrices_a1->field_0 & 0x10) != 0)
        {
            return;
        }

        long stream = (long)shape_PsgBuildBoneMatrices_a1->field_24;
        shape_PsgBuildBoneMatrices_a1->field_5 = (byte)a2;
        for (uint part = 0; ; ++part)
        {
            byte* header = ResolveGlobalPsxAddress(shape_PsgBuildBoneMatrices_a1->field_8);
            if (part >= header[3])
            {
                return;
            }

            int skipped = (int)shape_PsgBuildBoneMatrices_a1->field_32;
            if (((skipped >> (int)part) & 1) != 0)
            {
                continue;
            }

            sbyte code;
            do
            {
                code = *(sbyte*)(stream + 7);
                if (code == -1)
                {
                    stream += 8;
                }
                else if (code == -31)
                {
                    stream += 8;
                    *(uint*)(stream - 4) = a2 | 0xE1000000;
                }
                else
                {
                    code &= 0x3C;
                }

                switch (code)
                {
                    case 0x20:
                        stream += 20;
                        break;
                    case 0x24:
                        SetBlendBits(stream, 22, a2);
                        stream += 32;
                        break;
                    case 0x28:
                        stream += 24;
                        break;
                    case 0x2C:
                        SetBlendBits(stream, 22, a2);
                        stream += 40;
                        break;
                    case 0x30:
                        *(byte*)(stream + 7) |= 2;
                        stream += 28;
                        break;
                    case 0x34:
                        SetBlendBits(stream, 26, a2);
                        stream += 40;
                        break;
                    case 0x38:
                        *(byte*)(stream + 7) |= 2;
                        stream += 36;
                        break;
                    case 0x3C:
                        SetBlendBits(stream, 26, a2);
                        stream += 52;
                        break;
                }
            }
            while (code != -1);
        }
    }

    /// <summary>Clears tpage bits 5..6 at primitive + tpageOffset, sets the semi-transparent code bit, ORs in mode.</summary>
    private static void SetBlendBits(long primitive, int tpageOffset, ushort mode)
    {
        Recompiler.Runtime.Tracer.Enter("SetBlendBits(primitive, tpageOffset, mode)", primitive, tpageOffset, mode);
        *(ushort*)(primitive + tpageOffset) &= 0xFF9F;
        *(byte*)(primitive + 7) |= 2;
        *(ushort*)(primitive + tpageOffset) |= mode;
    }
}
