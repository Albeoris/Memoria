using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Assets
{
    /// <summary>
    /// API for Material / Texture nodes
    /// </summary>
    public class FbxMaterial
    {
        public FbxNode MaterialNode;
        public FbxNode TextureNode = null;

        public String TexturePath => (String)TextureNode?["RelativeFilename"]?.Value;
        public String Shader
        {
            get
            {
                String shaderName = (String)MaterialNode["ShadingModel"]?.Value;
                return String.IsNullOrEmpty(shaderName) || String.Equals(shaderName, "Phong", StringComparison.InvariantCultureIgnoreCase) ? "Default" : shaderName;
            }
        }

        public FbxMaterial(FbxNode materialNode, FbxNode objects, FbxNode connections)
        {
            List<FbxNode> textureNodes = new List<FbxNode>(objects.GetNodesByName("Texture"));
            Int32 materialId = materialNode.Id;
            MaterialNode = materialNode;
            if (textureNodes.Count == 0)
                return;
            foreach (FbxNode c in connections.GetNodesByName("C"))
            {
                if (c.Properties.Count != 4 || (String)c.Properties[0] != "OP" || Convert.ToInt32(c.Properties[2]) != materialId)
                    continue;
                Int32 linkedId = Convert.ToInt32(c.Properties[1]);
                FbxNode linkedNode = textureNodes.Find(n => n.Id == linkedId);
                if (linkedNode != null)
                {
                    TextureNode = linkedNode;
                    break;
                }
            }
        }
    }
}
