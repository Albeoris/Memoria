using System;
using UnityEngine;

public static class WMEffectLoader
{
    public static GameObject LoadMesh(String assetPath, String objectName, Material material)
    {
        Mesh mesh = Resources.Load(assetPath) as Mesh;
        if (mesh != (UnityEngine.Object)null)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = objectName;
            gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            GameObject gameObject2 = new GameObject();
            gameObject2.name = "Model";
            gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
            gameObject2.transform.localScale = new Vector3(1f, -1f, 1f);
            gameObject2.transform.parent = gameObject.transform;
            MeshFilter meshFilter = gameObject2.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
            MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            return gameObject;
        }
        return (GameObject)null;
    }
}
