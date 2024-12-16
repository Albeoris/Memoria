using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Assets
{
    public class FbxBone
    {
        public UInt32 Id;
        public FbxNode BoneNode;
        public FbxBone Parent = null;
        public Vector3 Position = Vector3.zero;
        public Quaternion Rotation = Quaternion.identity;
        public Vector3 Scale = Vector3.one;
        public Dictionary<Int32, FbxDeformer> SubdeformerNodes;

        public FbxBone(FbxBone parent, UInt32 id, FbxNode boneNode)
        {
            Parent = parent;
            Id = id;
            BoneNode = boneNode;
            FbxNode propertyNode = boneNode["Properties70"];
            if (propertyNode != null)
            {
                FbxNode positionNode = propertyNode["P", "Lcl Translation"];
                FbxNode rotationNode = propertyNode["P", "Lcl Rotation"];
                FbxNode scalingNode = propertyNode["P", "Lcl Scaling"];
                if (positionNode != null && positionNode.Properties.Count == 7)
                    Position = FbxNode.Vector3FromObjects(positionNode.Properties[4], positionNode.Properties[5], positionNode.Properties[6]);
                if (rotationNode != null && rotationNode.Properties.Count == 7)
                    Rotation = Quaternion.Euler(FbxNode.Vector3FromObjects(rotationNode.Properties[4], rotationNode.Properties[5], rotationNode.Properties[6]));
                if (scalingNode != null && scalingNode.Properties.Count == 7)
                    Scale = FbxNode.Vector3FromObjects(scalingNode.Properties[4], scalingNode.Properties[5], scalingNode.Properties[6]);
            }
            SubdeformerNodes = new Dictionary<Int32, FbxDeformer>();
        }
    }
}
