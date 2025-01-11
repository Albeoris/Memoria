using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Prime;

namespace Memoria.Assets
{
    public class FbxBone
    {
        public readonly FbxNode BoneNode;
        public Int32 BoneId = -1; // Bone ID is -1 when it's not part of the skeleton (eg. for bones that define the position of Geometries)
        public FbxBone Parent = null;
        public Vector3 Position = Vector3.zero;
        public Quaternion Rotation = Quaternion.identity;
        public Vector3 Scale = Vector3.one;
        public Dictionary<Int32, FbxDeformer> SubdeformerNodes;

        public FbxBone(FbxNode boneNode, EulerRotationOrder rotationOrder)
        {
            Parent = null;
            BoneId = -1;
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
                    Rotation.SetupFromEulerAngles(FbxNode.Vector3FromObjects(rotationNode.Properties[4], rotationNode.Properties[5], rotationNode.Properties[6]), rotationOrder);
                if (scalingNode != null && scalingNode.Properties.Count == 7)
                    Scale = FbxNode.Vector3FromObjects(scalingNode.Properties[4], scalingNode.Properties[5], scalingNode.Properties[6]);
            }
            SubdeformerNodes = new Dictionary<Int32, FbxDeformer>();

            // Double-sided materials are not supported: better duplicate faces (or even vertices) of the model to import if needed
            //Boolean doubleSided = (String)BoneNode["Culling"]?.Value == "CullingOff";
        }

        public Matrix4x4 GetLocalToWorldMatrix()
        {
            if (Parent == null)
                return Matrix4x4.TRS(Position, Rotation, Scale);
            return Parent.GetLocalToWorldMatrix() * Matrix4x4.TRS(Position, Rotation, Scale);
        }

        public Quaternion GetComposedRotation()
        {
            Quaternion absoluteRotation = Rotation;
            FbxBone parent = Parent;
            while (parent != null)
            {
                absoluteRotation = parent.Rotation * absoluteRotation;
                parent = parent.Parent;
            }
            return absoluteRotation;
        }

        /// <summary>Multiply the scaling factors of the hierarchy... which makes no sense if they are not uniform and if there are rotations</summary>
        public Vector3 CombineScalingFactors()
        {
            Vector3 combinedScale = Scale;
            FbxBone parent = Parent;
            while (parent != null)
            {
                combinedScale.x *= parent.Scale.x;
                combinedScale.y *= parent.Scale.y;
                combinedScale.z *= parent.Scale.z;
                parent = parent.Parent;
            }
            return combinedScale;
        }
    }
}
