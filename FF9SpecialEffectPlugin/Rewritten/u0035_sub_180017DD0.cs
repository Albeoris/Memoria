namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Vertex bytes of a PSG model (handle a1, PSX address of the model at +8): for part a2, or every part when a2 is -1,
    /// sums 4 x quad counts (+2/+6/+10/+14) and 3 x triangle counts (+4/+8/+12/+16), times 8 bytes per vertex.
    /// </summary>
    public static long ComputePsgVertexBytes(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("ComputePsgVertexBytes(a1, a2)", a1, a2);
        Shape_ComputePsgVertexBytes_a1* shape_ComputePsgVertexBytes_a1 = (Shape_ComputePsgVertexBytes_a1*)a1;
        long ctx = (long)unk_180576A10;
        PsgModel* model = (PsgModel*)PsxAddrToPtr(ctx, shape_ComputePsgVertexBytes_a1->field_8);
        long wanted = a2;
        int partCount = model->PartCount;
        uint vertices = 0;
        for (long index = 0; index < partCount; index++)
        {
            if (a2 != -1 && wanted != index)
            {
                continue;
            }

            PsgMeshPart* part = (PsgMeshPart*)(PsxAddrToPtr(ctx, model->PartsAddr) + 40 * index);
            int triangles = part->Tri20Count + part->Tri24Count + part->ExtraTriCount1 + part->ExtraTriCount2;
            int quads = part->Quad24Count + part->Quad32Count + part->ExtraQuadCount1 + part->ExtraQuadCount2;
            vertices += (uint)(2 * triangles + triangles + 4 * quads);
        }

        return 8L * vertices;
    }
}
