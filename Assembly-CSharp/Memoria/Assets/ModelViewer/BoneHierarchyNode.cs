using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Assets
{
	public class BoneHierarchyNode : IEnumerable<BoneHierarchyNode>
    {
        public Transform Bone { get; set; }
        public BoneHierarchyNode Parent { get; set; }
        public ICollection<BoneHierarchyNode> Children { get; set; }

        public Vector3 Position => Bone.localToWorldMatrix.GetColumn(3);
        public Int32 Id
        {
            get
            {
                Int32 id;
                if (Int32.TryParse(Bone.name.Substring(4), out id))
                    return id;
                return -1;
            }
        }
        public String NameInHierarchy
		{
            get
			{
                String name = Bone.name;
                BoneHierarchyNode node = Parent;
                while (node != null)
				{
                    name = node.Bone.name + "/" + name;
                    node = node.Parent;
                }
                return name;
            }
        }

        public static BoneHierarchyNode CreateFromModel(GameObject go)
        {
            BoneHierarchyNode root = null;
            foreach (Transform t in go.transform)
			{
                if (t.name.StartsWith("bone"))
				{
                    root = new BoneHierarchyNode(t);
                    break;
                }
			}
            if (root == null)
                return null;
            root.CreateFromModelRecursive();
            return root;
        }

        public BoneHierarchyNode(Transform bone)
        {
            this.Bone = bone;
            this.Children = new LinkedList<BoneHierarchyNode>();
        }

        public BoneHierarchyNode AddChild(Transform child)
        {
            BoneHierarchyNode childNode = new BoneHierarchyNode(child) { Parent = this };
            this.Children.Add(childNode);
            return childNode;
        }

        public IEnumerator<BoneHierarchyNode> GetEnumerator()
        {
            yield return this;
            foreach (BoneHierarchyNode child in Children)
                foreach (BoneHierarchyNode recChild in child)
                    yield return recChild;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void CreateFromModelRecursive()
        {
            foreach (Transform t in Bone)
            {
                if (t.name.StartsWith("bone"))
                {
                    BoneHierarchyNode nextNode = AddChild(t);
                    nextNode.CreateFromModelRecursive();
                }
            }
        }
    }
}
