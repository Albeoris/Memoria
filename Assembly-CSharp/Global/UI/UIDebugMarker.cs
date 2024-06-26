using System;
using UnityEngine;
using Object = System.Object;

public class UIDebugMarker
{
    public static GameObject fontResource
    {
        get
        {
            if (UIDebugMarker.fontCache == (UnityEngine.Object)null)
            {
                UIDebugMarker.fontCache = (GameObject)Resources.Load("EmbeddedAsset/UI/Prefabs/Mark Label");
            }
            return UIDebugMarker.fontCache;
        }
    }

    public static void MarkByUIScreenPoint(Camera UIMainCamera, Vector2 screenPosition, UIDebugMarker.AxisType axisType, String markText)
    {
        if (UIDebugMarker.enable)
        {
            if (axisType == UIDebugMarker.AxisType.FF9)
            {
                screenPosition.y = UIManager.UIContentSize.y - screenPosition.y;
            }
            screenPosition.x -= UIManager.UIContentSize.x / 2f;
            screenPosition.y -= UIManager.UIContentSize.y / 2f;
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(UIDebugMarker.fontResource);
            GameObject gameObject2;
            if (UIMainCamera.transform.childCount == 0)
            {
                gameObject2 = NGUITools.AddChild(UIMainCamera.gameObject, new GameObject());
                UIPanel uipanel = gameObject2.AddComponent<UIPanel>();
                uipanel.SetRect(0f, 0f, UIManager.UIContentSize.x, UIManager.UIContentSize.y);
                uipanel.depth = 1000;
            }
            else
            {
                gameObject2 = UIMainCamera.transform.GetChild(0).gameObject;
            }
            gameObject.transform.parent = gameObject2.transform;
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = screenPosition;
            gameObject.GetComponent<UILabel>().text = markText;
            UIDebugMarker.DebugLog(String.Concat(new Object[]
            {
                "Mark ",
                markText,
                " at ",
                screenPosition
            }));
        }
    }

    public static void DebugLog(String input)
    {
        if (UIDebugMarker.enable)
        {
            global::Debug.Log(input);
        }
    }

    public static void CleanUp(Camera UIMainCamera)
    {
        if (UIMainCamera.transform.childCount == 1)
        {
            foreach (Object obj in UIMainCamera.transform.GetChild(0))
            {
                Transform transform = (Transform)obj;
                UnityEngine.Object.Destroy(transform.gameObject);
            }
        }
    }

    private static GameObject fontCache;

    public static Boolean enable;

    public enum AxisType
    {
        NGUI,
        FF9
    }
}
