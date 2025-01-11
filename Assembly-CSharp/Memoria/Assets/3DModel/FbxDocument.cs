using System;
using System.Linq;
using System.Collections.Generic;
using Memoria.Prime;

namespace Memoria.Assets
{
    /// <summary>A top-level FBX node</summary>
    public class FbxDocument : FbxNodeList
    {
        /// <summary>Describes the format and data of the document</summary>
        /// <remarks>
        /// It isn't recommended that you change this value directly, because
        /// it won't change any of the document's data which can be version-specific.
        /// Most FBX importers can cope with any version.
        /// </remarks>
        public FbxVersion Version { get; set; } = FbxVersion.v7_4;

        public FbxNode Definitions
        {
            get
            {
                if (_definitions == null)
                    _definitions = this["Definitions"];
                return _definitions;
            }
        }

        public FbxNode Objects
        {
            get
            {
                if (_objects == null)
                    _objects = this["Objects"];
                return _objects;
            }
        }

        public FbxNode Connections
        {
            get
            {
                if (_connections == null)
                    _connections = this["Connections"];
                return _connections;
            }
        }

        public List<FbxBone> GetAllBones()
        {
            if (_allBones != null)
                return _allBones;
            _allBones = new List<FbxBone>();
            FbxNode objects = Objects;
            if (objects == null)
                return _allBones;
            FbxNode rotationOrderNode = GetDefaultProperties("Model")?["P", "RotationOrder"];
            EulerRotationOrder rotationOrder = rotationOrderNode != null && rotationOrderNode.Properties.Count == 5 ? (EulerRotationOrder)Convert.ToInt32(rotationOrderNode.Properties[4]) : EulerRotationOrder.EULER_XYZ;
            foreach (FbxNode node in objects.GetNodesByName("Model"))
                _allBones.Add(new FbxBone(node, rotationOrder));
            foreach (FbxBone bone in _allBones)
                bone.Parent = GetFirstConnectedNode(bone.BoneNode.Id, true, _allBones, b => b.BoneNode.Id);
            return _allBones;
        }

        public List<FbxGeometry> GetGeometries()
        {
            if (_geometries != null)
                return _geometries;
            _geometries = new List<FbxGeometry>();
            FbxNode objects = Objects;
            if (objects == null)
                return _geometries;
            List<FbxBone> allBones = GetAllBones();
            foreach (FbxNode node in objects.GetNodesByName("Geometry"))
            {
                Int32 linkedBoneIndex = GetFirstConnectedIndex(node.Id, true, allBones, b => b.BoneNode.Id);
                if (linkedBoneIndex >= 0)
                    _geometries.Add(new FbxGeometry(node, allBones[linkedBoneIndex]));
                else
                    _geometries.Add(new FbxGeometry(node, null));
            }
            return _geometries;
        }

        public List<FbxMaterial> GetMaterials()
        {
            if (_materials != null)
                return _materials;
            _materials = new List<FbxMaterial>();
            FbxNode objects = Objects;
            FbxNode connections = Connections;
            if (objects == null || connections == null)
                return _materials;
            foreach (FbxNode node in objects.GetNodesByName("Material"))
                _materials.Add(new FbxMaterial(node, objects, connections));
            return _materials;
        }

        public FbxSkeleton GetSkeleton()
        {
            if (_skeleton != null)
                return _skeleton;
            FbxNode objects = Objects;
            FbxNode connections = Connections;
            if (objects == null || connections == null)
                return null;
            _skeleton = new FbxSkeleton(connections, GetAllBones());
            List<FbxGeometry> geometries = GetGeometries();
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
                        Int32 linkedBoneId = GetFirstConnectedIndex(clusterId, false, _skeleton.Bones, b => b.BoneNode.Id);
                        if (linkedBoneId >= 0)
                            _skeleton.Bones[linkedBoneId].SubdeformerNodes[geoId] = new FbxDeformer(clusters[linkedClusterIndex]);
                    }
                }
            }
            return _skeleton;
        }

        public Int32 GetMaterialIndex(FbxGeometry mesh)
        {
            FbxNode objects = Objects;
            if (objects == null)
                return -1;
            List<FbxNode> modelNodes = new List<FbxNode>(objects.GetNodesByName("Model"));
            Int32 modelIndex = GetFirstConnectedIndex(mesh.GeometryNode.Id, true, modelNodes, n => n.Id);
            if (modelIndex < 0)
                return -1;
            List<FbxMaterial> materials = GetMaterials();
            return GetFirstConnectedIndex(modelNodes[modelIndex].Id, false, materials, mat => mat.MaterialNode.Id);
        }

        // TODO: use default properties more exhaustively
        public FbxNode GetDefaultProperties(String objectType)
        {
            FbxNode definitions = Definitions;
            if (definitions == null)
                return null;
            FbxNode def = definitions.GetNodesByName("ObjectType").FirstOrDefault(n => String.Equals((String)n.Value, objectType));
            return def?["PropertyTemplate"]?["Properties70"];
        }

        public T GetFirstConnectedNode<T>(Int32 nodeId, Boolean asChild, List<T> objectPool, Func<T, Int32> getId) where T : class
        {
            Int32 index = GetFirstConnectedIndex(nodeId, asChild, objectPool, getId);
            return index >= 0 ? objectPool[index] : null;
        }

        public Int32 GetFirstConnectedIndex<T>(Int32 nodeId, Boolean asChild, List<T> objectPool, Func<T, Int32> getId) where T : class
        {
            foreach (Int32 index in GetConnectedIndex(nodeId, asChild, objectPool, getId))
                return index;
            return -1;
        }

        public IEnumerable<Int32> GetConnectedIndex<T>(Int32 nodeId, Boolean asChild, List<T> objectPool, Func<T, Int32> getId) where T: class
        {
            FbxNode connections = Connections;
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

        private FbxNode _definitions = null;
        private FbxNode _objects = null;
        private FbxNode _connections = null;
        private List<FbxBone> _allBones = null;
        private List<FbxGeometry> _geometries = null;
        private List<FbxMaterial> _materials = null;
        private FbxSkeleton _skeleton = null;
    }
}
