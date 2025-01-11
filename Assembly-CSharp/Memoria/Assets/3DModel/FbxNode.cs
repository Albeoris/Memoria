using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Assets
{
    /// <summary>Represents a node in an FBX file</summary>
    public class FbxNode : FbxNodeList
    {
        /// <summary>The node name, which is often a class type</summary>
        /// <remarks>The name must be smaller than 256 characters to be written to a binary stream</remarks>
        public string Name { get; set; }

        /// <summary>The list of properties associated with the node</summary>
        /// <remarks>Supported types are primitives (apart from byte and char), arrays of primitives, and strings</remarks>
        public List<object> Properties { get; } = new List<object>();

        /// <summary>The first property element</summary>
        public object Value
        {
            get => Properties.Count < 1 ? null : Properties[0];
            set
            {
                if (Properties.Count < 1)
                    Properties.Add(value);
                else
                    Properties[0] = value;
            }
        }

        public Array AsArray => Value as Array;

        public Int32 Id => Properties.Count > 0 ? Convert.ToInt32(Properties[0]) : -1;

        /// <summary>Whether the node is empty of data</summary>
        public bool IsEmpty => string.IsNullOrEmpty(Name) && Properties.Count == 0 && Nodes.Count == 0;

        public static Vector2 Vector2FromObjects(object x, object y)
        {
            return new Vector2(Convert.ToSingle(x), Convert.ToSingle(y));
        }

        public static Vector3 Vector3FromObjects(object x, object y, object z)
        {
            return new Vector3(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));
        }

        public static Vector4 Vector4FromObjects(object x, object y, object z, object w)
        {
            return new Vector4(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z), Convert.ToSingle(w));
        }

        public static Color ColorFromObjects(object r, object g, object b, object a)
        {
            return new Color(Convert.ToSingle(r), Convert.ToSingle(g), Convert.ToSingle(b), Convert.ToSingle(a));
        }
    }
}
