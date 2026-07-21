using System;
using System.Collections.Generic;
using System.Globalization;

namespace Memoria.Assets
{
    /// <summary>API for Material / Texture nodes</summary>
    public class FbxMaterial
    {
        public const String BlinkOpenUdimProperty = "MemoriaBlinkOpenUDIM";
        public const String BlinkClosedUdimProperty = "MemoriaBlinkClosedUDIM";

        public readonly FbxNode MaterialNode;
        public readonly FbxNode TextureNode = null;

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

        public Boolean HasProperty(String propertyName)
        {
            return GetProperty(propertyName) != null;
        }

        public Boolean TryGetInt32Property(String propertyName, out Int32 value)
        {
            value = 0;
            FbxNode property = GetProperty(propertyName);
            if (property == null || property.Properties.Count < 5 || property.Properties[4] == null)
                return false;

            String valueText = Convert.ToString(property.Properties[4], CultureInfo.InvariantCulture);
            return Int32.TryParse(valueText, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        private FbxNode GetProperty(String propertyName)
        {
            return MaterialNode?["Properties70"]?["P", propertyName];
        }
    }
}
