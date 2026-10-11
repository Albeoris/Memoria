namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Adds the signed deltas a2/a3/a4 to the R/G/B bytes of the color word a1, clamping each channel to 0..255
    /// (results &lt;= 0 leave the channel 0); the top (GPU code) byte of a1 is kept.
    /// </summary>
    public static long ColorAddClamp(int a1, short a2, short a3, short a4)
    {
        Recompiler.Runtime.Tracer.Enter("ColorAddClamp(a1, a2, a3, a4)", a1, a2, a3, a4);
        short b = (short)((byte)(a1 >> 16) + a4);
        short g = (short)((byte)(a1 >> 8) + a3);
        short r = (short)((byte)a1 + a2);
        uint result = (uint)a1 & 0xFF000000;
        if (r > 255)
        {
            result |= 0xFF;
        }
        else if (r > 0)
        {
            result |= (ushort)r;
        }

        if (g > 255)
        {
            result |= 0xFF00;
        }
        else if (g > 0)
        {
            result |= (uint)(g << 8);
        }

        if (b > 255)
        {
            return result | 0xFF0000;
        }

        if (b <= 0)
        {
            return result;
        }

        return result | (uint)(b << 16);
    }

    /// <summary>
    /// Tints the GPU primitives of object a1 (skipped entirely when flag 0x10 of byte +0 is set). The part count is byte 3
    /// of the header at PSX address *(a1 + 8) (an unresolvable address is dereferenced as null, like the original). For
    /// every part i whose bit in *(a1 + 32) is clear, the primitives from the cursor *(a1 + 24) up to the 0xFF terminator
    /// get every color word passed through <see cref="ColorAddClamp"/>; 0xE1 (draw mode) entries are 8 bytes. A set bit
    /// leaves the cursor where it is. Codes whose (code &amp; 0x3C) is below 0x20 loop forever, as in the original.
    /// </summary>
    public static void TintObjectPrims(long a1, ushort a2, ushort a3, ushort a4)
    {
        Recompiler.Runtime.Tracer.Enter("TintObjectPrims(a1, a2, a3, a4)", a1, a2, a3, a4);
        Shape_PsgBuildBoneMatrices_a1* shape_PsgBuildBoneMatrices_a1 = (Shape_PsgBuildBoneMatrices_a1*)a1;
        if ((shape_PsgBuildBoneMatrices_a1->field_0 & 0x10) != 0)
        {
            return;
        }

        short dr = (short)a2;
        short dg = (short)a3;
        short db = (short)a4;
        byte* prim = (byte*)shape_PsgBuildBoneMatrices_a1->field_24;
        for (uint i = 0; ; ++i)
        {
            byte* header = (byte*)PsxAddrToPtr((long)unk_180576A10, shape_PsgBuildBoneMatrices_a1->field_8);
            if (i >= header[3])
            {
                return;
            }

            int skipMask = (int)shape_PsgBuildBoneMatrices_a1->field_32;
            if (((skipMask >> (int)i) & 1) != 0)
            {
                continue;
            }

            while (true)
            {
                byte code = prim[7];
                if (code == 0xFF)
                {
                    break;
                }

                if (code == 0xE1)
                {
                    prim += 8;
                    continue;
                }

                int size;
                switch (code & 0x3C)
                {
                    case 0x20: // F3
                        Tint(prim, 4, dr, dg, db);
                        size = 20;
                        break;
                    case 0x24: // FT3
                        Tint(prim, 4, dr, dg, db);
                        size = 32;
                        break;
                    case 0x28: // F4
                        Tint(prim, 4, dr, dg, db);
                        size = 24;
                        break;
                    case 0x2C: // FT4
                        Tint(prim, 4, dr, dg, db);
                        size = 40;
                        break;
                    case 0x30: // G3
                        Tint(prim, 4, dr, dg, db);
                        Tint(prim, 12, dr, dg, db);
                        Tint(prim, 20, dr, dg, db);
                        size = 28;
                        break;
                    case 0x34: // GT3
                        Tint(prim, 4, dr, dg, db);
                        Tint(prim, 16, dr, dg, db);
                        Tint(prim, 28, dr, dg, db);
                        size = 40;
                        break;
                    case 0x38: // G4
                        Tint(prim, 4, dr, dg, db);
                        Tint(prim, 12, dr, dg, db);
                        Tint(prim, 20, dr, dg, db);
                        Tint(prim, 28, dr, dg, db);
                        size = 36;
                        break;
                    case 0x3C: // GT4
                        Tint(prim, 4, dr, dg, db);
                        Tint(prim, 16, dr, dg, db);
                        Tint(prim, 28, dr, dg, db);
                        Tint(prim, 40, dr, dg, db);
                        size = 52;
                        break;
                    default:
                        // The original re-reads the same code without advancing: an endless loop.
                        continue;
                }

                prim[7] = code;
                prim += size;
            }

            prim += 8;
        }

        static void Tint(byte* prim, int offset, short dr, short dg, short db)
        {
            *(uint*)(prim + offset) = (uint)ColorAddClamp(*(int*)(prim + offset), dr, dg, db);
        }
    }
}
