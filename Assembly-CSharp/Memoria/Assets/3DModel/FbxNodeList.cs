using System;
using System.Linq;
using System.Collections.Generic;

namespace Memoria.Assets
{
    /// <summary>Base class for nodes and documents</summary>
    public abstract class FbxNodeList
    {
        /// <summary>The list of child/nested nodes</summary>
        /// <remarks>
        /// A list with one or more null elements is treated differently than an empty list,
        /// and represented differently in all FBX output files.
        /// </remarks>
        public List<FbxNode> Nodes { get; } = new List<FbxNode>();

        /// <summary>Gets a named child node</summary>
        /// <param name="name"></param>
        /// <returns>The child node, or null</returns>
        public FbxNode this[string name] => Nodes.Find(n => n != null && n.Name == name);

        /// <summary>Gets a child node with a given name and a ID</summary>
        /// <param name="name"></param>
        /// <returns>The child node, or null</returns>
        public FbxNode this[string nodeName, int nodeId] => Nodes.Find(n => n != null && n.Name == nodeName && n.Properties.Count > 0 && Convert.ToInt32(n.Properties[0]) == nodeId);

        /// <summary>Gets a child node with a given name and a property name</summary>
        /// <param name="name"></param>
        /// <returns>The child node, or null</returns>
        public FbxNode this[string nodeName, string propertyName] => Nodes.Find(n => n != null && n.Name == nodeName && n.Properties.Count > 0 && (string)n.Properties[0] == propertyName);

        /// <summary>Gets all children nodes with a given name</summary>
        /// <param name="name"></param>
        /// <returns>The child node, or null</returns>
        public IEnumerable<FbxNode> GetNodesByName(string name)
        {
            return Nodes.Where(n => n != null && n.Name == name);
        }

        /// <summary>Gets a child node, using a '/' separated path</summary>
        /// <param name="path"></param>
        /// <returns>The child node, or null</returns>
        public FbxNode GetRelative(string path)
        {
            var tokens = path.Split('/');
            FbxNodeList n = this;
            foreach (var t in tokens)
            {
                if (t == "")
                    continue;
                n = n[t];
                if (n == null)
                    break;
            }
            return n as FbxNode;
        }
    }
}
