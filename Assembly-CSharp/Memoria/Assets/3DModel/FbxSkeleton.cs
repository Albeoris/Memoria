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
        public FbxBone this[UInt32 id] => Bones.Find(b => b.Id == id);

        public FbxSkeleton(FbxNode objects, FbxNode connections)
        {
            List<FbxNode> modelPossibleBones = new List<FbxNode>(objects.GetNodesByName("Model").Where(n => n.Properties.Count == 3));
            List<FbxNode> objectConnections = new List<FbxNode>(connections.GetNodesByName("C").Where(c => c.Properties.Count == 3 && (String)c.Properties[0] == "OO"));
            List<FbxNode> boneNodes = new List<FbxNode>();
            List<FbxNode> rootNodes = new List<FbxNode>();
            HashSet<Int32> boneIds = new HashSet<Int32>();
            HashSet<Int32> rootIds = new HashSet<Int32>();
            foreach (FbxNode node in modelPossibleBones)
            {
                if ((String)node.Properties[2] == "LimbNode")
                {
                    boneNodes.Add(node);
                    boneIds.Add(node.Id);
                }
                else if ((String)node.Properties[2] == "Root")
                {
                    rootNodes.Add(node);
                    rootIds.Add(node.Id);
                }
            }
            FbxNode rootConnection = objectConnections.FirstOrDefault(c => boneIds.Contains(Convert.ToInt32(c.Properties[1])) && rootIds.Contains(Convert.ToInt32(c.Properties[2])));
            Boolean tryFallbackRoot = rootConnection == null;
            Int32 rootId = -1;
            FbxNode rootNode = null;
            if (!tryFallbackRoot)
            {
                rootId = Convert.ToInt32(rootConnection.Properties[2]);
                rootNode = rootNodes.FirstOrDefault(n => n.Id == rootId);
                tryFallbackRoot = rootNode == null;
            }
            if (tryFallbackRoot)
            {
                // Root might be one of the LimbNodes
                foreach (FbxNode node in boneNodes)
                {
                    rootId = node.Id;
                    if (objectConnections.Any(c => Convert.ToInt32(c.Properties[1]) == rootId && boneIds.Contains(Convert.ToInt32(c.Properties[2]))))
                        continue;
                    rootNode = node;
                    break;
                }
                if (rootNode == null)
                    return;
                boneIds.Remove(rootId);
                boneNodes.Remove(rootNode);
            }
            HashSet<UInt32> assignedBoneNameIds = [0];
            Dictionary<Int32, FbxBone> processedBones = new Dictionary<Int32, FbxBone>()
                { { rootId, AddBone(null, 0, rootNode)} };
            UInt32 nameIdCounter = 1;
            Boolean propagating = true;
            while (propagating && processedBones.Count <= boneIds.Count)
            {
                propagating = false;
                foreach (FbxNode bone in boneNodes)
                {
                    Int32 boneId = bone.Id;
                    if (processedBones.ContainsKey(boneId))
                        continue;
                    FbxBone parentBone = null;
                    FbxNode boneConnection = objectConnections.FirstOrDefault(c => Convert.ToInt32(c.Properties[1]) == boneId && processedBones.TryGetValue(Convert.ToInt32(c.Properties[2]), out parentBone));
                    if (boneConnection == null)
                        continue;
                    Int32 boneParentId = Convert.ToInt32(boneConnection.Properties[2]);
                    String boneName = (String)bone.Properties[1];
                    if (boneName.Length < 3 || !UInt32.TryParse(boneName.Substring(boneName.Length - 3), out UInt32 boneNameId))
                    {
                        while (assignedBoneNameIds.Contains(nameIdCounter))
                            nameIdCounter++;
                        boneNameId = nameIdCounter++;
                    }
                    assignedBoneNameIds.Add(boneNameId);
                    processedBones.Add(boneId, AddBone(parentBone, boneNameId, bone));
                    propagating = true;
                }
            }
        }

        private FbxBone AddBone(FbxBone parent, UInt32 id, FbxNode boneNode)
        {
            FbxBone bone = new FbxBone(parent, id, boneNode);
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
