#pragma warning disable CS0675 // the packet words OR sign-extended s16 fields exactly like the original

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>OT depth every packet of the layer set goes to.</summary>
    private const int EmitScrollLayersDepth = 4094;

    /// <summary>Colour used for a layer without a colour ramp.</summary>
    private const int EmitScrollLayersGrey = 0x808080;

    /// <summary>
    /// Draws the scrolling layer set <paramref name="a1"/> at time <paramref name="a2"/> into a w x h window
    /// (+2/+3 bytes) at (+4, +6): pushes draw-area/offset packets (0xE4/0xE5) that clip to the window and restore the
    /// screen ones from the config at off_180066C68 + 28, a texpage pair, then each layer (s32 offsets at +8; count
    /// u16 at +0) by kind: 0 tiled scroll, 1/3 sine-wave strips, 2 sprite animation. Returns the last AddPrim result;
    /// with dword_1803678E8 == 126 nothing is drawn (see the divergence for the return value).
    /// </summary>
    public static int* EmitScrollLayers(ushort* a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScrollLayers(a1, a2)", (nint)a1, a2);
        if (CurrentEffectId == (int)SpecialEffectId.Slow)
        {
            return null;
        }

        ushort x = a1[2];
        ushort y = a1[3];
        int width = *((byte*)a1 + 2);
        int count = a1[0];
        int height = *((byte*)a1 + 3);
        nint config = off_180066C68;
        Table* table = (Table*)config;

        uint address = (uint)table->field_36;
        byte* area = ResolveGlobalPsxAddress(address);
        Area* area2 = (Area*)area;
        table->field_36 = (int)(address + 24);
        area[3] = 2;
        area2->union_4_4 = ((short)y << 9) | (short)height | unchecked((int)0xE4000000);
        area2->union_8_4 = ((short)x << 16) | (short)width;
        uint screen0 = *(uint*)ResolveGlobalPsxAddress((uint)table->field_28);
        uint screen1 = *(uint*)(ResolveGlobalPsxAddress((uint)table->field_28) + 4);
        area[15] = 2;
        area2->field_16 = ((short)(screen0 >> 16) << 9) | (short)(screen1 >> 16) | unchecked((int)0xE4000000);
        area2->field_20 = ((short)screen0 << 16) | (short)screen1;

        address = (uint)table->field_36;
        byte* offset = ResolveGlobalPsxAddress(address);
        Area* area3 = (Area*)offset;
        table->field_36 = (int)(address + 24);
        offset[3] = 2;
        area3->union_4_4 = unchecked((int)0xE5000000);
        area3->union_8_4 = (int)(x | ((uint)y << 16));
        uint screenOffset = *(uint*)(ResolveGlobalPsxAddress((uint)table->field_28) + 8);
        offset[15] = 2;
        area3->field_16 = unchecked((int)0xE5000000);
        area3->field_20 = (int)screenOffset;

        address = (uint)table->field_36;
        byte* texpage = ResolveGlobalPsxAddress(address);
        Area* area4 = (Area*)texpage;
        ((Table*)off_180066C68)->field_36 = (int)(address + 16);
        area4->union_4_4 = unchecked((int)0xE1000000);
        area4->union_12_4 = 0xE1000200;
        PsxAddPrimAtDepth(0x1000000, EmitScrollLayersDepth, (ulong)(texpage + 8));
        PsxAddPrimAtDepth(0x2000000, EmitScrollLayersDepth, (ulong)(area + 12));
        PsxAddPrimAtDepth(0x2000000, EmitScrollLayersDepth, (ulong)(offset + 12));

        int color = 0;
        for (long i = 0; i < count; i++)
        {
            byte* layer = (byte*)a1 + *(int*)((byte*)a1 + 8 + 4 * i);
            long ramp = *(int*)(layer + 12);
            if ((uint)ramp != 0)
            {
                SampleColorRamp((long)a1 + ramp, (byte*)&color, a2 % *(ushort*)(layer + 10));
            }
            else
            {
                color = EmitScrollLayersGrey;
            }

            switch (*(ushort*)layer)
            {
                case 0:
                    EmitScrollLayersTiled(layer, a2, width, height, &color);
                    break;
                case 1:
                    EmitScrollLayersWaveX(layer, a2, width, height, &color);
                    break;
                case 2:
                    EmitScrollLayersSprite(a1, layer, a2);
                    break;
                case 3:
                    EmitScrollLayersWaveY(layer, a2, width, height, &color);
                    break;
            }
        }

        address = (uint)((Table*)off_180066C68)->field_36;
        byte* reset = ResolveGlobalPsxAddress(address);
        Area* area5 = (Area*)reset;
        ((Table*)off_180066C68)->field_36 = (int)(address + 16);
        area5->union_4_8 = 0;
        reset[7] = 0x60;
        area5->union_12_2 = (ushort)width;
        area5->union_14_2 = (ushort)height;
        PsxAddPrimAtDepth(0x3000000, EmitScrollLayersDepth, (ulong)reset);
        PsxAddPrimAtDepth(0x1000000, EmitScrollLayersDepth, (ulong)texpage);
        PsxAddPrimAtDepth(0x2000000, EmitScrollLayersDepth, (ulong)area);
        return PsxAddPrimAtDepth(0x2000000, EmitScrollLayersDepth, (ulong)offset);
    }

    /// <summary>a mod b with the original's adjustment: the quotient is decremented for a negative a (so a multiple gives b).</summary>
    private static int EmitScrollLayersWrap(int value, int size)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScrollLayersWrap(value, size)", value, size);
        int quotient = value / size;
        if (value < 0)
        {
            quotient--;
        }

        return value - size * quotient;
    }

    /// <summary>(int)(sin(phase / 4096.8 * 2pi) * 4096.8): the s12 sine of a 4096.8-unit angle.</summary>
    private static int EmitScrollLayersSin(int phase)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScrollLayersSin(phase)", phase);
        return Recompiler.Runtime.X86.Cvttsd2si(Math.Sin((double)phase / 4096.8 * 2.0 * 3.141592653589793) * 4096.8);
    }

    /// <summary>
    /// Kind 0: offset u/v = a2 * s16 speed (+20/+22), optionally a sine of it scaled by +24/+26 (flags +16/+17), wrapped
    /// into the window; up to four tiles cover the wrap seams.
    /// </summary>
    private static void EmitScrollLayersTiled(byte* layer, int a2, int width, int height, int* color)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScrollLayersTiled(layer, a2, width, height, color)", (nint)layer, a2, width, height, (nint)color);
        int u = *(short*)(layer + 20) * a2;
        if (layer[16] != 0)
        {
            u = (EmitScrollLayersSin(u) * *(ushort*)(layer + 24)) >> 12;
        }

        u = EmitScrollLayersWrap(u, width);
        int v = *(short*)(layer + 22) * a2;
        if (layer[17] != 0)
        {
            v = (EmitScrollLayersSin(v) * *(ushort*)(layer + 26)) >> 12;
        }

        v = EmitScrollLayersWrap(v, height);
        ushort* quad = (ushort*)layer;
        int tu = layer[6];
        int tv = layer[7];
        EmitFt4Quad(quad, (short)u, (short)u, (short)v, (short)v, tu, tv, width, height, (long)color, 1, 1);
        if (u != 0)
        {
            EmitFt4Quad(quad, (short)(u - width), (short)(u - width), (short)v, (short)v, tu, tv, width, height, (long)color, 0, 1);
        }

        if (v != 0)
        {
            EmitFt4Quad(quad, (short)u, (short)u, (short)(v - height), (short)(v - height), tu, tv, width, height, (long)color, 1, 0);
        }

        if (u != 0 && v != 0)
        {
            EmitFt4Quad(
                quad,
                (short)(u - width),
                (short)(u - width),
                (short)(v - height),
                (short)(v - height),
                tu,
                tv,
                width,
                height,
                (long)color,
                0,
                0);
        }
    }

    /// <summary>
    /// Kind 1: horizontal strips (+16 strip height, +18 count) scrolling down by a2 * s16 +26, each strip's x edges
    /// swinging by a sine (amplitude +24, phase step +20, phase speed +22); a second pass covers the wrap seam.
    /// </summary>
    private static void EmitScrollLayersWaveX(byte* layer, int a2, int width, int height, int* color)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScrollLayersWaveX(layer, a2, width, height, color)", (nint)layer, a2, width, height, (nint)color);
        ushort* quad = (ushort*)layer;
        int v = EmitScrollLayersWrap(a2 * *(short*)(layer + 26), height);
        int step = *(ushort*)(layer + 20);
        int strip = *(ushort*)(layer + 16);
        int passes = (v != 0 ? 1 : 0) + 1;
        int phase = v * step / strip + a2 * *(ushort*)(layer + 22);
        do
        {
            if (*(ushort*)(layer + 18) > 0)
            {
                int half = width >> 1;
                int along = 0;
                for (int k = 0; k < *(ushort*)(layer + 18); k++)
                {
                    int amplitude = *(ushort*)(layer + 24);
                    int previous = phase;
                    phase += step;
                    int x1 = ((amplitude * EmitScrollLayersSin(phase)) >> 12) - half;
                    int x0 = ((amplitude * EmitScrollLayersSin(previous)) >> 12) - half;
                    EmitFt4Quad(quad, (short)x0, (short)x1, (short)v, (short)v, layer[6], along + layer[7], 2 * width, strip, (long)color, 0, 0);
                    along += strip;
                    v += strip;
                }
            }

            v -= 2 * height;
            passes--;
            phase -= 2 * step * *(ushort*)(layer + 18);
        }
        while (passes != 0);
    }

    /// <summary>Kind 3: <see cref="EmitScrollLayersWaveX"/> with the axes swapped (vertical strips scrolling right).</summary>
    private static void EmitScrollLayersWaveY(byte* layer, int a2, int width, int height, int* color)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScrollLayersWaveY(layer, a2, width, height, color)", (nint)layer, a2, width, height, (nint)color);
        ushort* quad = (ushort*)layer;
        int u = EmitScrollLayersWrap(a2 * *(short*)(layer + 26), width);
        int step = *(ushort*)(layer + 20);
        int strip = *(ushort*)(layer + 16);
        int passes = (u != 0 ? 1 : 0) + 1;
        int phase = u * step / strip + a2 * *(ushort*)(layer + 22);
        do
        {
            if (*(ushort*)(layer + 18) > 0)
            {
                int half = height >> 1;
                int along = 0;
                for (int k = 0; k < *(ushort*)(layer + 18); k++)
                {
                    int amplitude = *(ushort*)(layer + 24);
                    int previous = phase;
                    phase += step;
                    int y1 = ((amplitude * EmitScrollLayersSin(phase)) >> 12) - half;
                    int y0 = ((amplitude * EmitScrollLayersSin(previous)) >> 12) - half;
                    EmitFt4Quad(quad, (short)u, (short)u, (short)y0, (short)y1, along + layer[6], layer[7], strip, 2 * height, (long)color, 0, 0);
                    along += strip;
                    u += strip;
                }
            }

            u -= 2 * width;
            passes--;
            phase -= 2 * step * *(ushort*)(layer + 18);
        }
        while (passes != 0);
    }

    /// <summary>Kind 2: from frame +21 on, the sprite set at a1 + s32 +16 + 4 at (+24, +26), faded in over +22 frames.</summary>
    private static void EmitScrollLayersSprite(ushort* a1, byte* layer, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScrollLayersSprite(a1, layer, a2)", (nint)a1, (nint)layer, a2);
        int start = layer[21];
        if (start > a2)
        {
            return;
        }

        int fade = *(ushort*)(layer + 22);
        int elapsed = a2 - start;
        int scale = fade <= elapsed ? 4096 : (elapsed << 12) / fade;
        long set = *(int*)(layer + 16);
        if ((uint)set != 0)
        {
            EmitSpriteFrame((long)a1 + set + 4, *(short*)(layer + 24), *(short*)(layer + 26), elapsed, *(ushort*)(layer + 8), scale);
        }
    }
}
