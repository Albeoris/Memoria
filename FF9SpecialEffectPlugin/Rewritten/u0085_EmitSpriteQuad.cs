using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// One sprite as a 40-byte textured quad packet. Registers the frame record <paramref name="a1"/>, the parameter
    /// block <paramref name="a2"/>, the packet <paramref name="a3"/> and the ordering table <paramref name="a4"/>
    /// with the module mapper and works on their PSX addresses. Params (uint): +0 colour bits, +4 depth bias,
    /// +8 depth-cue factor (negative: none), +12 y offset, +16 PSX address of the (u, v) byte pair, +20 colour index,
    /// +24 tpage bits; +28/+32 receive the corner vector. Frame (u16): +0 tpage, +2 clut, +4 height/4, +6 width/2,
    /// +8 u size, +10 v size, +12 colour table (u16 + u8 per 4-byte entry). Corners come from three RTPS calls, the
    /// fourth from a fourth RTPS; the packet is linked when ((SZ3 >> 2) - bias) >> 2 is below 4096. Returns the host
    /// address of the colour word written (its scratchpad offset when the packet lives in the scratchpad).
    /// </summary>
    public static long EmitSpriteQuad(ulong a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("EmitSpriteQuad(a1, a2, a3, a4)", a1, a2, a3, a4);
        long context = (long)unk_180576A10;
        uint frame = (uint)PsxAddrMapperRegister(context, a1);
        uint parameters = (uint)PsxAddrMapperRegister(context, a2);
        uint packet = (uint)PsxAddrMapperRegister(context, a3);
        uint ot = (uint)PsxAddrMapperRegister(context, a4);

        int colorBits = (int)BsqRead32(parameters);
        int depthBias = (int)BsqRead32(parameters + 4);
        int depthCue = (int)BsqRead32(parameters + 8);
        uint uvAddress = BsqRead32(parameters + 16);
        qword_180211F94 = 0;
        dword_180211F9C = 0;
        int halfWidth = *(ushort*)ResolveGlobalPsxAddress(frame + 6);
        int quarterHeight = *(ushort*)ResolveGlobalPsxAddress(frame + 4);
        int top = -(int)BsqRead32(parameters + 12);
        int right = 2 * halfWidth;
        int bottom = (4 * quarterHeight) + top;
        dword_180211FD0 = right;
        *(int*)ResolveGlobalPsxAddress(parameters + 28) = right;
        *(int*)unk_180211FC4 = top;
        dword_180211FD4 = top;
        uint* vector0 = (uint*)Unsafe.AsPointer(ref qword_180211FC0);
        *vector0 = (ushort)(-(short)right);
        dword_180211FC8 = (ushort)(-(short)right);
        dword_180211FCC = bottom;
        *(int*)ResolveGlobalPsxAddress(parameters + 32) = bottom;

        int u = *ResolveGlobalPsxAddress(uvAddress);
        int v = *ResolveGlobalPsxAddress(uvAddress + 1);
        GteRtps(0);
        GteRtps(1);
        GteRtps(2);
        int vShifted = v << 8;
        *(int*)ResolveGlobalPsxAddress(packet + 12) = (*(ushort*)ResolveGlobalPsxAddress(frame + 2) << 16) | u | vShifted;
        int uSize = *(ushort*)ResolveGlobalPsxAddress(frame + 8);
        int vSize = (short)*(ushort*)ResolveGlobalPsxAddress(frame + 10);
        int tpage = (int)BsqRead32(parameters + 24) | *(ushort*)ResolveGlobalPsxAddress(frame);
        int uRight = u + uSize - 1;
        *(int*)ResolveGlobalPsxAddress(packet + 20) = vShifted | uRight | (tpage << 16);
        int vBottom = (short)((vSize << 8) - 256 + vShifted);
        *(ushort*)ResolveGlobalPsxAddress(packet + 28) = (ushort)(u | vBottom);
        *(ushort*)ResolveGlobalPsxAddress(packet + 36) = (ushort)(vBottom | uRight);
        *(int*)ResolveGlobalPsxAddress(packet + 8) = dword_180211FF0;
        *(int*)ResolveGlobalPsxAddress(packet + 16) = dword_180211FF4;
        *(int*)ResolveGlobalPsxAddress(packet + 24) = dword_180211FF8;

        uint cornerX = BsqRead32(parameters + 28);
        uint cornerY = BsqRead32(parameters + 32);
        int depth = dword_18021200C;
        *vector0 = cornerX;
        *(uint*)unk_180211FC4 = cornerY;
        uint colorIndex = BsqRead32(parameters + 20);
        GteRtps(0);
        uint colorEntry = (4 * colorIndex) + frame + 12;
        int otIndex = ((depth >> 2) - depthBias) >> 2;
        if ((uint)otIndex < 0x1000)
        {
            var head = (uint*)ResolveGlobalPsxAddress(ot) + otIndex;
            byte* host = ResolveGlobalPsxAddress(packet);
            Site_ResolveGlobalPsxAddress3* site_ResolveGlobalPsxAddress3 = (Site_ResolveGlobalPsxAddress3*)host;
            host[3] = 9;
            site_ResolveGlobalPsxAddress3->union_0_4 ^= (*head ^ site_ResolveGlobalPsxAddress3->union_0_4) & 0xFFFFFF;
            *head ^= (*head ^ (uint)PsxPtrToAddr(context, (ulong)host)) & 0xFFFFFF;
        }

        int blue = *ResolveGlobalPsxAddress(colorEntry + 2);
        int redGreen = *(ushort*)ResolveGlobalPsxAddress(colorEntry);
        *(int*)ResolveGlobalPsxAddress(packet + 32) = dword_180211FF8;
        int color = colorBits | (blue << 16) | redGreen;
        if (depthCue >= 0)
        {
            dword_180211FD8 = color;
            dword_180211FE0 = 4096 - depthCue;
            GteDpcs();
            color = dword_180212018;
        }

        uint colorAddress = packet + 4;
        byte* target = ResolveGlobalPsxAddress(colorAddress);
        *(int*)target = color;
        bool ram = colorAddress >> 24 == 128 && (colorAddress & 0xFFFFFFF) < 0x200000;
        bool segment = colorAddress >> 24 != 128 && (colorAddress & 0xC00000) == 0xC00000;
        return ram || segment ? (long)target : colorAddress - 0x1F800000u;
    }
}
