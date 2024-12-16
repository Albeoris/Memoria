using System;
using System.Linq;
using System.Collections.Generic;

namespace Memoria.Assets
{
    /// <summary>
    /// A top-level FBX node
    /// </summary>
    public class FbxDocument : FbxNodeList
    {
        /// <summary>
        /// Describes the format and data of the document
        /// </summary>
        /// <remarks>
        /// It isn't recommended that you change this value directly, because
        /// it won't change any of the document's data which can be version-specific.
        /// Most FBX importers can cope with any version.
        /// </remarks>
        public FbxVersion Version { get; set; } = FbxVersion.v7_4;

        public List<FbxGeometry> GetGeometries()
        {
            FbxNode objects = this["Objects"];
            if (objects == null)
                return new List<FbxGeometry>();
            List<FbxGeometry> result = new List<FbxGeometry>();
            foreach (FbxNode node in objects.GetNodesByName("Geometry"))
                result.Add(new FbxGeometry(node));
            return result;
        }

        public List<FbxMaterial> GetMaterials()
        {
            FbxNode objects = this["Objects"];
            FbxNode connections = this["Connections"];
            if (objects == null || connections == null)
                return new List<FbxMaterial>();
            List<FbxMaterial> result = new List<FbxMaterial>();
            foreach (FbxNode node in objects.GetNodesByName("Material"))
                result.Add(new FbxMaterial(node, objects, connections));
            return result;
        }

        public FbxSkeleton GetSkeleton(List<FbxGeometry> geometries)
        {
            FbxNode objects = this["Objects"];
            FbxNode connections = this["Connections"];
            if (objects == null || connections == null)
                return null;
            FbxSkeleton skeleton = new FbxSkeleton(objects, connections);
            List<FbxNode> skins = new List<FbxNode>();
            List<FbxNode> clusters = new List<FbxNode>();
            foreach (FbxNode node in objects.GetNodesByName("Deformer"))
            {
                if (node.Properties.Count == 3)
                {
                    if ((String)node.Properties[2] == "Skin")
                        skins.Add(node);
                    else if ((String)node.Properties[2] == "Cluster")
                        clusters.Add(node);
                }
            }
            foreach (FbxGeometry geo in geometries)
            {
                Int32 geoId = geo.GeometryNode.Id;
                Int32 linkedSkinIndex = GetFirstConnectedIndex(geoId, false, skins, n => n.Id);
                if (linkedSkinIndex >= 0)
                {
                    Int32 skinId = skins[linkedSkinIndex].Id;
                    foreach (Int32 linkedClusterIndex in GetConnectedIndex(skinId, false, clusters, n => n.Id))
                    {
                        Int32 clusterId = clusters[linkedClusterIndex].Id;
                        Int32 linkedBoneId = GetFirstConnectedIndex(clusterId, false, skeleton.Bones, b => b.BoneNode.Id);
                        if (linkedBoneId >= 0)
                            skeleton.Bones[linkedBoneId].SubdeformerNodes[geoId] = new FbxDeformer(clusters[linkedClusterIndex]);
                    }
                }
            }
            return skeleton;
        }

        public Int32 GetMaterialIndex(FbxGeometry mesh, List<FbxMaterial> materials)
        {
            FbxNode objects = this["Objects"];
            if (objects == null)
                return -1;
            List<FbxNode> modelNodes = new List<FbxNode>(objects.GetNodesByName("Model"));
            Int32 modelIndex = GetFirstConnectedIndex(mesh.GeometryNode.Id, true, modelNodes, n => n.Id);
            if (modelIndex < 0)
                return -1;
            return GetFirstConnectedIndex(modelNodes[modelIndex].Id, false, materials, mat => mat.MaterialNode.Id);
        }

        public Int32 GetFirstConnectedIndex<T>(Int32 nodeId, Boolean asChild, List<T> objectPool, Func<T, Int32> getId) where T : class
        {
            foreach (Int32 index in GetConnectedIndex(nodeId, asChild, objectPool, getId))
                return index;
            return -1;
        }

        public IEnumerable<Int32> GetConnectedIndex<T>(Int32 nodeId, Boolean asChild, List<T> objectPool, Func<T, Int32> getId) where T: class
        {
            FbxNode connections = this["Connections"];
            if (connections == null)
                yield break;
            Int32 nodeSearch = asChild ? 1 : 2;
            Int32 poolSearch = asChild ? 2 : 1;
            foreach (FbxNode c in connections.GetNodesByName("C"))
            {
                if (c.Properties.Count >= 3 && Convert.ToInt32(c.Properties[nodeSearch]) == nodeId)
                {
                    Int32 objId = Convert.ToInt32(c.Properties[poolSearch]);
                    for (Int32 i = 0; i < objectPool.Count; i++)
                        if (getId(objectPool[i]) == objId)
                            yield return i;
                }
            }
            yield break;
        }
    }
}
