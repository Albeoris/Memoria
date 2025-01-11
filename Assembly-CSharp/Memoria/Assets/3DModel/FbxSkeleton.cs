using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Prime;

namespace Memoria.Assets
{
    public class FbxSkeleton
    {
        public List<FbxBone> Bones { get; } = new List<FbxBone>();
        public FbxBone this[UInt32 id] => Bones.Find(b => b.BoneId == id);

        public FbxSkeleton(FbxNode connections, List<FbxBone> allBones)
        {
            List<FbxNode> objectConnections = new List<FbxNode>(connections.GetNodesByName("C").Where(c => c.Properties.Count == 3 && (String)c.Properties[0] == "OO"));
            List<FbxBone> boneNodes = new List<FbxBone>();
            List<FbxBone> rootNodes = new List<FbxBone>();
            HashSet<Int32> boneIds = new HashSet<Int32>();
            HashSet<Int32> rootIds = new HashSet<Int32>();
            foreach (FbxBone bone in allBones)
            {
                if (bone.BoneNode.Properties.Count >= 3)
                {
                    if ((String)bone.BoneNode.Properties[2] == "LimbNode")
                    {
                        boneNodes.Add(bone);
                        boneIds.Add(bone.BoneNode.Id);
                    }
                    else if ((String)bone.BoneNode.Properties[2] == "Root")
                    {
                        rootNodes.Add(bone);
                        rootIds.Add(bone.BoneNode.Id);
                    }
                }
            }
            FbxNode rootConnection = objectConnections.FirstOrDefault(c => boneIds.Contains(Convert.ToInt32(c.Properties[1])) && rootIds.Contains(Convert.ToInt32(c.Properties[2])));
            Boolean tryFallbackRoot = rootConnection == null;
            Int32 rootId = -1;
            FbxBone rootNode = null;
            if (!tryFallbackRoot)
            {
                rootId = Convert.ToInt32(rootConnection.Properties[2]);
                rootNode = rootNodes.FirstOrDefault(b => b.BoneNode.Id == rootId);
                tryFallbackRoot = rootNode == null;
            }
            if (tryFallbackRoot)
            {
                // Root might be one of the LimbNodes
                foreach (FbxBone bone in boneNodes)
                {
                    rootId = bone.BoneNode.Id;
                    if (objectConnections.Any(c => Convert.ToInt32(c.Properties[1]) == rootId && boneIds.Contains(Convert.ToInt32(c.Properties[2]))))
                        continue;
                    rootNode = bone;
                    break;
                }
                if (rootNode == null)
                    return;
                boneIds.Remove(rootId);
                boneNodes.Remove(rootNode);
            }
            HashSet<Int32> assignedBoneNameIds = [0];
            Dictionary<Int32, FbxBone> processedBones = new Dictionary<Int32, FbxBone>()
                { { rootId, RegisterBone(rootNode, 0)} };
            Int32 nameIdCounter = 1;
            Boolean propagating = true;
            while (propagating && processedBones.Count <= boneIds.Count)
            {
                propagating = false;
                foreach (FbxBone bone in boneNodes)
                {
                    Int32 boneId = bone.BoneNode.Id;
                    if (processedBones.ContainsKey(boneId))
                        continue;
                    String boneName = (String)bone.BoneNode.Properties[1];
                    if (boneName.Length < 3 || !Int32.TryParse(boneName.Substring(boneName.Length - 3), out Int32 boneNameId))
                    {
                        while (assignedBoneNameIds.Contains(nameIdCounter))
                            nameIdCounter++;
                        boneNameId = nameIdCounter++;
                    }
                    if (!assignedBoneNameIds.Add(boneNameId))
                        throw new IndexOutOfRangeException($"Model bone '{boneName}': the bone with ID {boneNameId} is defined twice");
                    processedBones.Add(boneId, RegisterBone(bone, boneNameId));
                    propagating = true;
                }
            }
        }

        private FbxBone RegisterBone(FbxBone bone, Int32 id)
        {
            bone.BoneId = id;
            Bones.Add(bone);
            return bone;
        }

        public static void RegisterWeight(ref BoneWeight bw, Int32 boneIndex, Single weight)
        {
            if (bw.weight0 <= 0f)
            {
                bw.boneIndex0 = boneIndex;
                bw.weight0 = weight;
            }
            else if (bw.weight1 <= 0f)
            {
                bw.boneIndex1 = boneIndex;
                bw.weight1 = weight;
            }
            else if (bw.weight2 <= 0f)
            {
                bw.boneIndex2 = boneIndex;
                bw.weight2 = weight;
            }
            else if (bw.weight3 <= 0f)
            {
                bw.boneIndex3 = boneIndex;
                bw.weight3 = weight;
            }
            else
            {
                Log.Warning($"[FbxSkeleton] Error when importing bone weights: a vertex can be linked to 4 bones at most");
            }
        }
    }
}
