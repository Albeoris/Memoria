using System;

namespace Memoria.Assets
{
    public class FbxDeformer
    {
        public FbxNode DeformerNode;

        public FbxDeformer(FbxNode deformerNode)
        {
            DeformerNode = deformerNode;
        }

        public Boolean GetWeights(out Int32[] indices, out Single[] weights)
        {
            Array arrIndices = DeformerNode["Indexes"]?.AsArray ?? null;
            Array arrWeights = DeformerNode["Weights"]?.AsArray ?? null;
            indices = null;
            weights = null;
            if (arrIndices == null || arrWeights == null || arrIndices.Length != arrWeights.Length)
                return false;
            Int32 count = arrIndices.Length;
            indices = new Int32[count];
            weights = new Single[count];
            var enumIndices = arrIndices.GetEnumerator();
            var enumWeights = arrWeights.GetEnumerator();
            for (Int32 i = 0; i < count; i++)
            {
                enumIndices.MoveNext();
                enumWeights.MoveNext();
                indices[i] = Convert.ToInt32(enumIndices.Current);
                weights[i] = Convert.ToSingle(enumWeights.Current);
            }
            return true;
        }
    }
}
