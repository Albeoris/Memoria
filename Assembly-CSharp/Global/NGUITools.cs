using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class NGUITools
{
    public static Single soundVolume
    {
        get
        {
            if (!NGUITools.mLoaded)
            {
                NGUITools.mLoaded = true;
                NGUITools.mGlobalVolume = PlayerPrefs.GetFloat("Sound", 1f);
            }
            return NGUITools.mGlobalVolume;
        }
        set
        {
            if (NGUITools.mGlobalVolume != value)
            {
                NGUITools.mLoaded = true;
                NGUITools.mGlobalVolume = value;
                PlayerPrefs.SetFloat("Sound", value);
            }
        }
    }

    public static Boolean fileAccess
    {
        get
        {
            return Application.platform != RuntimePlatform.WindowsWebPlayer && Application.platform != RuntimePlatform.OSXWebPlayer;
        }
    }

    public static AudioSource PlaySound(AudioClip clip)
    {
        return NGUITools.PlaySound(clip, 1f, 1f);
    }

    public static AudioSource PlaySound(AudioClip clip, Single volume)
    {
        return NGUITools.PlaySound(clip, volume, 1f);
    }

    public static AudioSource PlaySound(AudioClip clip, Single volume, Single pitch)
    {
        Single time = RealTime.time;
        if (NGUITools.mLastClip == clip && NGUITools.mLastTimestamp + 0.1f > time)
        {
            return (AudioSource)null;
        }
        NGUITools.mLastClip = clip;
        NGUITools.mLastTimestamp = time;
        volume *= NGUITools.soundVolume;
        if (clip != (UnityEngine.Object)null && volume > 0.01f)
        {
            if (NGUITools.mListener == (UnityEngine.Object)null || !NGUITools.GetActive(NGUITools.mListener))
            {
                AudioListener[] array = UnityEngine.Object.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
                if (array != null)
                {
                    for (Int32 i = 0; i < (Int32)array.Length; i++)
                    {
                        if (NGUITools.GetActive(array[i]))
                        {
                            NGUITools.mListener = array[i];
                            break;
                        }
                    }
                }
                if (NGUITools.mListener == (UnityEngine.Object)null)
                {
                    Camera camera = Camera.main;
                    if (camera == (UnityEngine.Object)null)
                    {
                        camera = (UnityEngine.Object.FindObjectOfType(typeof(Camera)) as Camera);
                    }
                    if (camera != (UnityEngine.Object)null)
                    {
                        NGUITools.mListener = camera.gameObject.AddComponent<AudioListener>();
                    }
                }
            }
            if (NGUITools.mListener != (UnityEngine.Object)null && NGUITools.mListener.enabled && NGUITools.GetActive(NGUITools.mListener.gameObject))
            {
                AudioSource audioSource = NGUITools.mListener.GetComponent<AudioSource>();
                if (audioSource == (UnityEngine.Object)null)
                {
                    audioSource = NGUITools.mListener.gameObject.AddComponent<AudioSource>();
                }
                audioSource.priority = 50;
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(clip, volume);
                return audioSource;
            }
        }
        return (AudioSource)null;
    }

    public static Int32 RandomRange(Int32 min, Int32 max)
    {
        if (min == max)
        {
            return min;
        }
        return UnityEngine.Random.Range(min, max + 1);
    }

    public static String GetHierarchy(GameObject obj)
    {
        if (obj == (UnityEngine.Object)null)
        {
            return String.Empty;
        }
        String text = obj.name;
        while (obj.transform.parent != (UnityEngine.Object)null)
        {
            obj = obj.transform.parent.gameObject;
            text = obj.name + "\\" + text;
        }
        return text;
    }

    public static T[] FindActive<T>() where T : Component
    {
        return UnityEngine.Object.FindObjectsOfType(typeof(T)) as T[];
    }

    public static Camera FindCameraForLayer(Int32 layer)
    {
        Int32 num = 1 << layer;
        Camera camera;
        for (Int32 i = 0; i < UICamera.list.size; i++)
        {
            camera = UICamera.list.buffer[i].cachedCamera;
            if (camera && (camera.cullingMask & num) != 0)
            {
                return camera;
            }
        }
        camera = Camera.main;
        if (camera && (camera.cullingMask & num) != 0)
        {
            return camera;
        }
        Camera[] array = new Camera[Camera.allCamerasCount];
        Int32 allCameras = Camera.GetAllCameras(array);
        for (Int32 j = 0; j < allCameras; j++)
        {
            camera = array[j];
            if (camera && camera.enabled && (camera.cullingMask & num) != 0)
            {
                return camera;
            }
        }
        return (Camera)null;
    }

    public static void AddWidgetCollider(GameObject go)
    {
        NGUITools.AddWidgetCollider(go, false);
    }

    public static void AddWidgetCollider(GameObject go, Boolean considerInactive)
    {
        if (go != (UnityEngine.Object)null)
        {
            Collider component = go.GetComponent<Collider>();
            BoxCollider boxCollider = component as BoxCollider;
            if (boxCollider != (UnityEngine.Object)null)
            {
                NGUITools.UpdateWidgetCollider(boxCollider, considerInactive);
                return;
            }
            if (component != (UnityEngine.Object)null)
            {
                return;
            }
            BoxCollider2D boxCollider2D = go.GetComponent<BoxCollider2D>();
            if (boxCollider2D != (UnityEngine.Object)null)
            {
                NGUITools.UpdateWidgetCollider(boxCollider2D, considerInactive);
                return;
            }
            UICamera uicamera = UICamera.FindCameraForLayer(go.layer);
            if (uicamera != (UnityEngine.Object)null && (uicamera.eventType == UICamera.EventType.World_2D || uicamera.eventType == UICamera.EventType.UI_2D))
            {
                boxCollider2D = go.AddComponent<BoxCollider2D>();
                boxCollider2D.isTrigger = true;
                UIWidget component2 = go.GetComponent<UIWidget>();
                if (component2 != (UnityEngine.Object)null)
                {
                    component2.autoResizeBoxCollider = true;
                }
                NGUITools.UpdateWidgetCollider(boxCollider2D, considerInactive);
                return;
            }
            boxCollider = go.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            UIWidget component3 = go.GetComponent<UIWidget>();
            if (component3 != (UnityEngine.Object)null)
            {
                component3.autoResizeBoxCollider = true;
            }
            NGUITools.UpdateWidgetCollider(boxCollider, considerInactive);
        }
    }

    public static void UpdateWidgetCollider(GameObject go)
    {
        NGUITools.UpdateWidgetCollider(go, false);
    }

    public static void UpdateWidgetCollider(GameObject go, Boolean considerInactive)
    {
        if (go != (UnityEngine.Object)null)
        {
            BoxCollider component = go.GetComponent<BoxCollider>();
            if (component != (UnityEngine.Object)null)
            {
                NGUITools.UpdateWidgetCollider(component, considerInactive);
                return;
            }
            BoxCollider2D component2 = go.GetComponent<BoxCollider2D>();
            if (component2 != (UnityEngine.Object)null)
            {
                NGUITools.UpdateWidgetCollider(component2, considerInactive);
            }
        }
    }

    public static void UpdateWidgetCollider(BoxCollider box, Boolean considerInactive)
    {
        if (box != (UnityEngine.Object)null)
        {
            GameObject gameObject = box.gameObject;
            UIWidget component = gameObject.GetComponent<UIWidget>();
            if (component != (UnityEngine.Object)null)
            {
                Vector4 drawRegion = component.drawRegion;
                if (drawRegion.x != 0f || drawRegion.y != 0f || drawRegion.z != 1f || drawRegion.w != 1f)
                {
                    Vector4 drawingDimensions = component.drawingDimensions;
                    box.center = new Vector3((drawingDimensions.x + drawingDimensions.z) * 0.5f, (drawingDimensions.y + drawingDimensions.w) * 0.5f);
                    box.size = new Vector3(drawingDimensions.z - drawingDimensions.x, drawingDimensions.w - drawingDimensions.y);
                }
                else
                {
                    Vector3[] localCorners = component.localCorners;
                    box.center = Vector3.Lerp(localCorners[0], localCorners[2], 0.5f);
                    box.size = localCorners[2] - localCorners[0];
                }
            }
            else
            {
                Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(gameObject.transform, considerInactive);
                box.center = bounds.center;
                box.size = new Vector3(bounds.size.x, bounds.size.y, 0f);
            }
        }
    }

    public static void UpdateWidgetCollider(BoxCollider2D box, Boolean considerInactive)
    {
        if (box != (UnityEngine.Object)null)
        {
            GameObject gameObject = box.gameObject;
            UIWidget component = gameObject.GetComponent<UIWidget>();
            if (component != (UnityEngine.Object)null)
            {
                Vector3[] localCorners = component.localCorners;
                box.offset = Vector3.Lerp(localCorners[0], localCorners[2], 0.5f);
                box.size = localCorners[2] - localCorners[0];
            }
            else
            {
                Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(gameObject.transform, considerInactive);
                box.offset = bounds.center;
                box.size = new Vector2(bounds.size.x, bounds.size.y);
            }
        }
    }

    public static String GetTypeName<T>()
    {
        String text = typeof(T).ToString();
        if (text.StartsWith("UI"))
        {
            text = text.Substring(2);
        }
        else if (text.StartsWith("UnityEngine."))
        {
            text = text.Substring(12);
        }
        return text;
    }

    public static String GetTypeName(UnityEngine.Object obj)
    {
        if (obj == (UnityEngine.Object)null)
        {
            return "Null";
        }
        String text = obj.GetType().ToString();
        if (text.StartsWith("UI"))
        {
            text = text.Substring(2);
        }
        else if (text.StartsWith("UnityEngine."))
        {
            text = text.Substring(12);
        }
        return text;
    }

    public static void RegisterUndo(UnityEngine.Object obj, String name)
    {
    }

    public static void SetDirty(UnityEngine.Object obj)
    {
    }

    public static GameObject AddChild(GameObject parent)
    {
        return NGUITools.AddChild(parent, true);
    }

    public static GameObject AddChild(GameObject parent, Boolean undo)
    {
        GameObject gameObject = new GameObject();
        if (parent != (UnityEngine.Object)null)
        {
            Transform transform = gameObject.transform;
            transform.parent = parent.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            gameObject.layer = parent.layer;
        }
        return gameObject;
    }

    public static GameObject AddChild(GameObject parent, GameObject prefab)
    {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
        if (gameObject != (UnityEngine.Object)null && parent != (UnityEngine.Object)null)
        {
            Transform transform = gameObject.transform;
            transform.parent = parent.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            gameObject.layer = parent.layer;
        }
        return gameObject;
    }

    public static Int32 CalculateRaycastDepth(GameObject go)
    {
        UIWidget component = go.GetComponent<UIWidget>();
        if (component != (UnityEngine.Object)null)
        {
            return component.raycastDepth;
        }
        UIWidget[] componentsInChildren = go.GetComponentsInChildren<UIWidget>();
        if (componentsInChildren.Length == 0)
        {
            return 0;
        }
        Int32 num = Int32.MaxValue;
        Int32 i = 0;
        Int32 num2 = (Int32)componentsInChildren.Length;
        while (i < num2)
        {
            if (componentsInChildren[i].enabled)
            {
                num = Mathf.Min(num, componentsInChildren[i].raycastDepth);
            }
            i++;
        }
        return num;
    }

    public static Int32 CalculateNextDepth(GameObject go)
    {
        if (go)
        {
            Int32 num = -1;
            UIWidget[] componentsInChildren = go.GetComponentsInChildren<UIWidget>();
            Int32 i = 0;
            Int32 num2 = (Int32)componentsInChildren.Length;
            while (i < num2)
            {
                num = Mathf.Max(num, componentsInChildren[i].depth);
                i++;
            }
            return num + 1;
        }
        return 0;
    }

    public static Int32 CalculateNextDepth(GameObject go, Boolean ignoreChildrenWithColliders)
    {
        if (go && ignoreChildrenWithColliders)
        {
            Int32 num = -1;
            UIWidget[] componentsInChildren = go.GetComponentsInChildren<UIWidget>();
            Int32 i = 0;
            Int32 num2 = (Int32)componentsInChildren.Length;
            while (i < num2)
            {
                UIWidget uiwidget = componentsInChildren[i];
                if (!(uiwidget.cachedGameObject != go) || (!(uiwidget.GetComponent<Collider>() != (UnityEngine.Object)null) && !(uiwidget.GetComponent<Collider2D>() != (UnityEngine.Object)null)))
                {
                    num = Mathf.Max(num, uiwidget.depth);
                }
                i++;
            }
            return num + 1;
        }
        return NGUITools.CalculateNextDepth(go);
    }

    public static Int32 AdjustDepth(GameObject go, Int32 adjustment)
    {
        if (!(go != (UnityEngine.Object)null))
        {
            return 0;
        }
        UIPanel uipanel = go.GetComponent<UIPanel>();
        if (uipanel != (UnityEngine.Object)null)
        {
            UIPanel[] componentsInChildren = go.GetComponentsInChildren<UIPanel>(true);
            for (Int32 i = 0; i < (Int32)componentsInChildren.Length; i++)
            {
                UIPanel uipanel2 = componentsInChildren[i];
                uipanel2.depth += adjustment;
            }
            return 1;
        }
        uipanel = NGUITools.FindInParents<UIPanel>(go);
        if (uipanel == (UnityEngine.Object)null)
        {
            return 0;
        }
        UIWidget[] componentsInChildren2 = go.GetComponentsInChildren<UIWidget>(true);
        Int32 j = 0;
        Int32 num = (Int32)componentsInChildren2.Length;
        while (j < num)
        {
            UIWidget uiwidget = componentsInChildren2[j];
            if (!(uiwidget.panel != uipanel))
            {
                uiwidget.depth += adjustment;
            }
            j++;
        }
        return 2;
    }

    public static void BringForward(GameObject go)
    {
        Int32 num = NGUITools.AdjustDepth(go, 1000);
        if (num == 1)
        {
            NGUITools.NormalizePanelDepths();
        }
        else if (num == 2)
        {
            NGUITools.NormalizeWidgetDepths();
        }
    }

    public static void PushBack(GameObject go)
    {
        Int32 num = NGUITools.AdjustDepth(go, -1000);
        if (num == 1)
        {
            NGUITools.NormalizePanelDepths();
        }
        else if (num == 2)
        {
            NGUITools.NormalizeWidgetDepths();
        }
    }

    public static void NormalizeDepths()
    {
        NGUITools.NormalizeWidgetDepths();
        NGUITools.NormalizePanelDepths();
    }

    public static void NormalizeWidgetDepths()
    {
        NGUITools.NormalizeWidgetDepths(NGUITools.FindActive<UIWidget>());
    }

    public static void NormalizeWidgetDepths(GameObject go)
    {
        NGUITools.NormalizeWidgetDepths(go.GetComponentsInChildren<UIWidget>());
    }

    public static void NormalizeWidgetDepths(UIWidget[] list)
    {
        Int32 num = (Int32)list.Length;
        if (num > 0)
        {
            Array.Sort<UIWidget>(list, new Comparison<UIWidget>(UIWidget.FullCompareFunc));
            Int32 num2 = 0;
            Int32 depth = list[0].depth;
            for (Int32 i = 0; i < num; i++)
            {
                UIWidget uiwidget = list[i];
                if (uiwidget.depth == depth)
                {
                    uiwidget.depth = num2;
                }
                else
                {
                    depth = uiwidget.depth;
                    num2 = (uiwidget.depth = num2 + 1);
                }
            }
        }
    }

    public static void NormalizePanelDepths()
    {
        UIPanel[] array = NGUITools.FindActive<UIPanel>();
        Int32 num = (Int32)array.Length;
        if (num > 0)
        {
            Array.Sort<UIPanel>(array, new Comparison<UIPanel>(UIPanel.CompareFunc));
            Int32 num2 = 0;
            Int32 depth = array[0].depth;
            for (Int32 i = 0; i < num; i++)
            {
                UIPanel uipanel = array[i];
                if (uipanel.depth == depth)
                {
                    uipanel.depth = num2;
                }
                else
                {
                    depth = uipanel.depth;
                    num2 = (uipanel.depth = num2 + 1);
                }
            }
        }
    }

    public static UIPanel CreateUI(Boolean advanced3D)
    {
        return NGUITools.CreateUI((Transform)null, advanced3D, -1);
    }

    public static UIPanel CreateUI(Boolean advanced3D, Int32 layer)
    {
        return NGUITools.CreateUI((Transform)null, advanced3D, layer);
    }

    public static UIPanel CreateUI(Transform trans, Boolean advanced3D, Int32 layer)
    {
        UIRoot uiroot = (!(trans != (UnityEngine.Object)null)) ? null : NGUITools.FindInParents<UIRoot>(trans.gameObject);
        if (uiroot == (UnityEngine.Object)null && UIRoot.list.Count > 0)
        {
            foreach (UIRoot uiroot2 in UIRoot.list)
            {
                if (uiroot2.gameObject.layer == layer)
                {
                    uiroot = uiroot2;
                    break;
                }
            }
        }
        if (uiroot == (UnityEngine.Object)null)
        {
            Int32 i = 0;
            Int32 count = UIPanel.list.Count;
            while (i < count)
            {
                UIPanel uipanel = UIPanel.list[i];
                GameObject gameObject = uipanel.gameObject;
                if (gameObject.hideFlags == HideFlags.None && gameObject.layer == layer)
                {
                    trans.parent = uipanel.transform;
                    trans.localScale = Vector3.one;
                    return uipanel;
                }
                i++;
            }
        }
        if (uiroot != (UnityEngine.Object)null)
        {
            UICamera componentInChildren = uiroot.GetComponentInChildren<UICamera>();
            if (componentInChildren != (UnityEngine.Object)null && componentInChildren.GetComponent<Camera>().orthographic == advanced3D)
            {
                trans = (Transform)null;
                uiroot = (UIRoot)null;
            }
        }
        if (uiroot == (UnityEngine.Object)null)
        {
            GameObject gameObject2 = NGUITools.AddChild((GameObject)null, false);
            uiroot = gameObject2.AddComponent<UIRoot>();
            if (layer == -1)
            {
                layer = LayerMask.NameToLayer("UI");
            }
            if (layer == -1)
            {
                layer = LayerMask.NameToLayer("2D UI");
            }
            gameObject2.layer = layer;
            if (advanced3D)
            {
                gameObject2.name = "UI Root (3D)";
                uiroot.scalingStyle = UIRoot.Scaling.Constrained;
            }
            else
            {
                gameObject2.name = "UI Root";
                uiroot.scalingStyle = UIRoot.Scaling.Flexible;
            }
        }
        UIPanel uipanel2 = uiroot.GetComponentInChildren<UIPanel>();
        if (uipanel2 == (UnityEngine.Object)null)
        {
            Camera[] array = NGUITools.FindActive<Camera>();
            Single num = -1f;
            Boolean flag = false;
            Int32 num2 = 1 << uiroot.gameObject.layer;
            for (Int32 j = 0; j < (Int32)array.Length; j++)
            {
                Camera camera = array[j];
                if (camera.clearFlags == CameraClearFlags.Color || camera.clearFlags == CameraClearFlags.Skybox)
                {
                    flag = true;
                }
                num = Mathf.Max(num, camera.depth);
                camera.cullingMask &= ~num2;
            }
            Camera camera2 = NGUITools.AddChild<Camera>(uiroot.gameObject, false);
            camera2.gameObject.AddComponent<UICamera>();
            camera2.clearFlags = (CameraClearFlags)((!flag) ? CameraClearFlags.Color : CameraClearFlags.Depth);
            camera2.backgroundColor = Color.grey;
            camera2.cullingMask = num2;
            camera2.depth = num + 1f;
            if (advanced3D)
            {
                camera2.nearClipPlane = 0.1f;
                camera2.farClipPlane = 4f;
                camera2.transform.localPosition = new Vector3(0f, 0f, -700f);
            }
            else
            {
                camera2.orthographic = true;
                camera2.orthographicSize = 1f;
                camera2.nearClipPlane = -10f;
                camera2.farClipPlane = 10f;
            }
            AudioListener[] array2 = NGUITools.FindActive<AudioListener>();
            if (array2 == null || array2.Length == 0)
            {
                camera2.gameObject.AddComponent<AudioListener>();
            }
            uipanel2 = uiroot.gameObject.AddComponent<UIPanel>();
        }
        if (trans != (UnityEngine.Object)null)
        {
            while (trans.parent != (UnityEngine.Object)null)
            {
                trans = trans.parent;
            }
            if (NGUITools.IsChild(trans, uipanel2.transform))
            {
                uipanel2 = trans.gameObject.AddComponent<UIPanel>();
            }
            else
            {
                trans.parent = uipanel2.transform;
                trans.localScale = Vector3.one;
                trans.localPosition = Vector3.zero;
                NGUITools.SetChildLayer(uipanel2.cachedTransform, uipanel2.cachedGameObject.layer);
            }
        }
        return uipanel2;
    }

    public static void SetChildLayer(Transform t, Int32 layer)
    {
        for (Int32 i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i);
            child.gameObject.layer = layer;
            NGUITools.SetChildLayer(child, layer);
        }
    }

    public static T AddChild<T>(GameObject parent) where T : Component
    {
        GameObject gameObject = NGUITools.AddChild(parent);
        gameObject.name = NGUITools.GetTypeName<T>();
        return gameObject.AddComponent<T>();
    }

    public static T AddChild<T>(GameObject parent, Boolean undo) where T : Component
    {
        GameObject gameObject = NGUITools.AddChild(parent, undo);
        gameObject.name = NGUITools.GetTypeName<T>();
        return gameObject.AddComponent<T>();
    }

    public static T AddWidget<T>(GameObject go) where T : UIWidget
    {
        Int32 depth = NGUITools.CalculateNextDepth(go);
        T result = NGUITools.AddChild<T>(go);
        result.width = 100;
        result.height = 100;
        result.depth = depth;
        return result;
    }

    public static T AddWidget<T>(GameObject go, Int32 depth) where T : UIWidget
    {
        T result = NGUITools.AddChild<T>(go);
        result.width = 100;
        result.height = 100;
        result.depth = depth;
        return result;
    }

    public static UISprite AddSprite(GameObject go, UIAtlas atlas, String spriteName)
    {
        UISpriteData uispriteData = (!(atlas != (UnityEngine.Object)null)) ? null : atlas.GetSprite(spriteName);
        UISprite uisprite = NGUITools.AddWidget<UISprite>(go);
        uisprite.type = (UIBasicSprite.Type)((uispriteData != null && uispriteData.hasBorder) ? UIBasicSprite.Type.Sliced : UIBasicSprite.Type.Simple);
        uisprite.atlas = atlas;
        uisprite.spriteName = spriteName;
        return uisprite;
    }

    public static GameObject GetRoot(GameObject go)
    {
        Transform transform = go.transform;
        for (; ; )
        {
            Transform parent = transform.parent;
            if (parent == (UnityEngine.Object)null)
            {
                break;
            }
            transform = parent;
        }
        return transform.gameObject;
    }

    public static T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null)
            return null;
        T component = go.GetComponent<T>();
        if (component == null)
        {
            Transform parent = go.transform.parent;
            while (parent != null && component == null)
            {
                component = parent.gameObject.GetComponent<T>();
                parent = parent.parent;
            }
        }
        return component;
    }

    public static T FindInParents<T>(Transform trans) where T : Component
    {
        return trans?.GetComponentInParent<T>();
    }

    public static void Destroy(UnityEngine.Object obj)
    {
        if (obj)
        {
            if (obj is Transform)
            {
                Transform transform = obj as Transform;
                GameObject gameObject = transform.gameObject;
                if (Application.isPlaying)
                {
                    transform.parent = (Transform)null;
                    UnityEngine.Object.Destroy(gameObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(gameObject);
                }
            }
            else if (obj is GameObject)
            {
                GameObject gameObject2 = obj as GameObject;
                Transform transform2 = gameObject2.transform;
                if (Application.isPlaying)
                {
                    transform2.parent = (Transform)null;
                    UnityEngine.Object.Destroy(gameObject2);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(gameObject2);
                }
            }
            else if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(obj);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
        }
    }

    public static void DestroyChildren(this Transform t)
    {
        Boolean isPlaying = Application.isPlaying;
        while (t.childCount != 0)
        {
            Transform child = t.GetChild(0);
            if (isPlaying)
            {
                child.parent = (Transform)null;
                UnityEngine.Object.Destroy(child.gameObject);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(child.gameObject);
            }
        }
    }

    public static void DestroyImmediate(UnityEngine.Object obj)
    {
        if (obj != (UnityEngine.Object)null)
        {
            if (Application.isEditor)
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
            else
            {
                UnityEngine.Object.Destroy(obj);
            }
        }
    }

    public static void Broadcast(String funcName)
    {
        GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        Int32 i = 0;
        Int32 num = (Int32)array.Length;
        while (i < num)
        {
            array[i].SendMessage(funcName, SendMessageOptions.DontRequireReceiver);
            i++;
        }
    }

    public static void Broadcast(String funcName, System.Object param)
    {
        GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        Int32 i = 0;
        Int32 num = (Int32)array.Length;
        while (i < num)
        {
            array[i].SendMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
            i++;
        }
    }

    public static Boolean IsChild(Transform parent, Transform child)
    {
        if (parent == (UnityEngine.Object)null || child == (UnityEngine.Object)null)
        {
            return false;
        }
        while (child != (UnityEngine.Object)null)
        {
            if (child == parent)
            {
                return true;
            }
            child = child.parent;
        }
        return false;
    }

    private static void Activate(Transform t)
    {
        NGUITools.Activate(t, false);
    }

    private static void Activate(Transform t, Boolean compatibilityMode)
    {
        NGUITools.SetActiveSelf(t.gameObject, true);
        if (compatibilityMode)
        {
            Int32 i = 0;
            Int32 childCount = t.childCount;
            while (i < childCount)
            {
                Transform child = t.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    return;
                }
                i++;
            }
            Int32 j = 0;
            Int32 childCount2 = t.childCount;
            while (j < childCount2)
            {
                Transform child2 = t.GetChild(j);
                NGUITools.Activate(child2, true);
                j++;
            }
        }
    }

    private static void Deactivate(Transform t)
    {
        NGUITools.SetActiveSelf(t.gameObject, false);
    }

    public static void SetActive(GameObject go, Boolean state)
    {
        NGUITools.SetActive(go, state, true);
    }

    public static void SetActive(GameObject go, Boolean state, Boolean compatibilityMode)
    {
        if (go)
        {
            if (state)
            {
                NGUITools.Activate(go.transform, compatibilityMode);
                NGUITools.CallCreatePanel(go.transform);
            }
            else
            {
                NGUITools.Deactivate(go.transform);
            }
        }
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    private static void CallCreatePanel(Transform t)
    {
        UIWidget component = t.GetComponent<UIWidget>();
        if (component != (UnityEngine.Object)null)
        {
            component.CreatePanel();
        }
        Int32 i = 0;
        Int32 childCount = t.childCount;
        while (i < childCount)
        {
            NGUITools.CallCreatePanel(t.GetChild(i));
            i++;
        }
    }

    public static void SetActiveChildren(GameObject go, Boolean state)
    {
        Transform transform = go.transform;
        if (state)
        {
            Int32 i = 0;
            Int32 childCount = transform.childCount;
            while (i < childCount)
            {
                Transform child = transform.GetChild(i);
                NGUITools.Activate(child);
                i++;
            }
        }
        else
        {
            Int32 j = 0;
            Int32 childCount2 = transform.childCount;
            while (j < childCount2)
            {
                Transform child2 = transform.GetChild(j);
                NGUITools.Deactivate(child2);
                j++;
            }
        }
    }

    [Obsolete("Use NGUITools.GetActive instead")]
    public static Boolean IsActive(Behaviour mb)
    {
        return mb != (UnityEngine.Object)null && mb.enabled && mb.gameObject.activeInHierarchy;
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static Boolean GetActive(Behaviour mb)
    {
        return mb && mb.enabled && mb.gameObject.activeInHierarchy;
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static Boolean GetActive(GameObject go)
    {
        return go && go.activeInHierarchy;
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static void SetActiveSelf(GameObject go, Boolean state)
    {
        go.SetActive(state);
    }

    public static void SetLayer(GameObject go, Int32 layer)
    {
        go.layer = layer;
        Transform transform = go.transform;
        Int32 i = 0;
        Int32 childCount = transform.childCount;
        while (i < childCount)
        {
            Transform child = transform.GetChild(i);
            NGUITools.SetLayer(child.gameObject, layer);
            i++;
        }
    }

    public static Vector3 Round(Vector3 v)
    {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        v.z = Mathf.Round(v.z);
        return v;
    }

    public static void MakePixelPerfect(Transform t)
    {
        UIWidget component = t.GetComponent<UIWidget>();
        if (component != (UnityEngine.Object)null)
        {
            component.MakePixelPerfect();
        }
        if (t.GetComponent<UIAnchor>() == (UnityEngine.Object)null && t.GetComponent<UIRoot>() == (UnityEngine.Object)null)
        {
            t.localPosition = NGUITools.Round(t.localPosition);
            t.localScale = NGUITools.Round(t.localScale);
        }
        Int32 i = 0;
        Int32 childCount = t.childCount;
        while (i < childCount)
        {
            NGUITools.MakePixelPerfect(t.GetChild(i));
            i++;
        }
    }

    public static Boolean Save(String fileName, Byte[] bytes)
    {
        if (!NGUITools.fileAccess)
        {
            return false;
        }
        String path = Application.persistentDataPath + "/" + fileName;
        if (bytes == null)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return true;
        }
        FileStream fileStream = (FileStream)null;
        try
        {
            fileStream = File.Create(path);
        }
        catch (Exception ex)
        {
            global::Debug.LogError(ex.Message);
            return false;
        }
        fileStream.Write(bytes, 0, (Int32)bytes.Length);
        fileStream.Close();
        return true;
    }

    public static Byte[] Load(String fileName)
    {
        if (!NGUITools.fileAccess)
        {
            return null;
        }
        String path = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(path))
        {
            return File.ReadAllBytes(path);
        }
        return null;
    }

    public static Color ApplyPMA(Color c)
    {
        if (c.a != 1f)
        {
            c.r *= c.a;
            c.g *= c.a;
            c.b *= c.a;
        }
        return c;
    }

    public static void MarkParentAsChanged(GameObject go)
    {
        UIRect[] componentsInChildren = go.GetComponentsInChildren<UIRect>();
        Int32 i = 0;
        Int32 num = (Int32)componentsInChildren.Length;
        while (i < num)
        {
            componentsInChildren[i].ParentHasChanged();
            i++;
        }
    }

    public static String clipboard
    {
        get
        {
            TextEditor textEditor = new TextEditor();
            textEditor.Paste();
            return textEditor.content.text;
        }
        set
        {
            TextEditor textEditor = new TextEditor();
            textEditor.content = new GUIContent(value);
            textEditor.OnFocus();
            textEditor.Copy();
        }
    }

    public static T AddMissingComponent<T>(this GameObject go) where T : Component
    {
        return go.GetComponent<T>() ?? go.AddComponent<T>();
    }

    public static Vector3[] GetSides(this Camera cam)
    {
        return cam.GetSides(Mathf.Lerp(cam.nearClipPlane, cam.farClipPlane, 0.5f), (Transform)null);
    }

    public static Vector3[] GetSides(this Camera cam, Single depth)
    {
        return cam.GetSides(depth, (Transform)null);
    }

    public static Vector3[] GetSides(this Camera cam, Transform relativeTo)
    {
        return cam.GetSides(Mathf.Lerp(cam.nearClipPlane, cam.farClipPlane, 0.5f), relativeTo);
    }

    public static Vector3[] GetSides(this Camera cam, Single depth, Transform relativeTo)
    {
        if (cam.orthographic)
        {
            Single orthographicSize = cam.orthographicSize;
            Single num = -orthographicSize;
            Single num2 = orthographicSize;
            Single y = -orthographicSize;
            Single y2 = orthographicSize;
            Rect rect = cam.rect;
            Vector2 screenSize = NGUITools.screenSize;
            Single num3 = screenSize.x / screenSize.y;
            num3 *= rect.width / rect.height;
            num *= num3;
            num2 *= num3;
            Transform transform = cam.transform;
            Quaternion rotation = transform.rotation;
            Vector3 position = transform.position;
            Int32 num4 = Mathf.RoundToInt(screenSize.x);
            Int32 num5 = Mathf.RoundToInt(screenSize.y);
            if ((num4 & 1) == 1)
            {
                position.x -= 1f / screenSize.x;
            }
            if ((num5 & 1) == 1)
            {
                position.y += 1f / screenSize.y;
            }
            NGUITools.mSides[0] = rotation * new Vector3(num, 0f, depth) + position;
            NGUITools.mSides[1] = rotation * new Vector3(0f, y2, depth) + position;
            NGUITools.mSides[2] = rotation * new Vector3(num2, 0f, depth) + position;
            NGUITools.mSides[3] = rotation * new Vector3(0f, y, depth) + position;
        }
        else
        {
            NGUITools.mSides[0] = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, depth));
            NGUITools.mSides[1] = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, depth));
            NGUITools.mSides[2] = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, depth));
            NGUITools.mSides[3] = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, depth));
        }
        if (relativeTo != (UnityEngine.Object)null)
        {
            for (Int32 i = 0; i < 4; i++)
            {
                NGUITools.mSides[i] = relativeTo.InverseTransformPoint(NGUITools.mSides[i]);
            }
        }
        return NGUITools.mSides;
    }

    public static Vector3[] GetWorldCorners(this Camera cam)
    {
        Single depth = Mathf.Lerp(cam.nearClipPlane, cam.farClipPlane, 0.5f);
        return cam.GetWorldCorners(depth, (Transform)null);
    }

    public static Vector3[] GetWorldCorners(this Camera cam, Single depth)
    {
        return cam.GetWorldCorners(depth, (Transform)null);
    }

    public static Vector3[] GetWorldCorners(this Camera cam, Transform relativeTo)
    {
        return cam.GetWorldCorners(Mathf.Lerp(cam.nearClipPlane, cam.farClipPlane, 0.5f), relativeTo);
    }

    public static Vector3[] GetWorldCorners(this Camera cam, Single depth, Transform relativeTo)
    {
        if (cam.orthographic)
        {
            Single orthographicSize = cam.orthographicSize;
            Single num = -orthographicSize;
            Single num2 = orthographicSize;
            Single y = -orthographicSize;
            Single y2 = orthographicSize;
            Rect rect = cam.rect;
            Vector2 screenSize = NGUITools.screenSize;
            Single num3 = screenSize.x / screenSize.y;
            num3 *= rect.width / rect.height;
            num *= num3;
            num2 *= num3;
            Transform transform = cam.transform;
            Quaternion rotation = transform.rotation;
            Vector3 position = transform.position;
            NGUITools.mSides[0] = rotation * new Vector3(num, y, depth) + position;
            NGUITools.mSides[1] = rotation * new Vector3(num, y2, depth) + position;
            NGUITools.mSides[2] = rotation * new Vector3(num2, y2, depth) + position;
            NGUITools.mSides[3] = rotation * new Vector3(num2, y, depth) + position;
        }
        else
        {
            NGUITools.mSides[0] = cam.ViewportToWorldPoint(new Vector3(0f, 0f, depth));
            NGUITools.mSides[1] = cam.ViewportToWorldPoint(new Vector3(0f, 1f, depth));
            NGUITools.mSides[2] = cam.ViewportToWorldPoint(new Vector3(1f, 1f, depth));
            NGUITools.mSides[3] = cam.ViewportToWorldPoint(new Vector3(1f, 0f, depth));
        }
        if (relativeTo != (UnityEngine.Object)null)
        {
            for (Int32 i = 0; i < 4; i++)
            {
                NGUITools.mSides[i] = relativeTo.InverseTransformPoint(NGUITools.mSides[i]);
            }
        }
        return NGUITools.mSides;
    }

    public static String GetFuncName(System.Object obj, String method)
    {
        if (obj == null)
            return "<null>";
        String typeName = obj.GetType().ToString();
        Int32 slashIndex = typeName.LastIndexOf('/');
        if (slashIndex > 0)
            typeName = typeName.Substring(slashIndex + 1);
        return String.IsNullOrEmpty(method) ? typeName : typeName + "/" + method;
    }

    public static void Execute<T>(GameObject go, String funcName) where T : Component
    {
        T[] components = go.GetComponents<T>();
        T[] array = components;
        for (Int32 i = 0; i < (Int32)array.Length; i++)
        {
            T t = array[i];
            MethodInfo method = t.GetType().GetMethod(funcName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(t, null);
            }
        }
    }

    public static void ExecuteAll<T>(GameObject root, String funcName) where T : Component
    {
        NGUITools.Execute<T>(root, funcName);
        Transform transform = root.transform;
        Int32 i = 0;
        Int32 childCount = transform.childCount;
        while (i < childCount)
        {
            NGUITools.ExecuteAll<T>(transform.GetChild(i).gameObject, funcName);
            i++;
        }
    }

    public static void ImmediatelyCreateDrawCalls(GameObject root)
    {
        NGUITools.ExecuteAll<UIWidget>(root, "Start");
        NGUITools.ExecuteAll<UIPanel>(root, "Start");
        NGUITools.ExecuteAll<UIWidget>(root, "Update");
        NGUITools.ExecuteAll<UIPanel>(root, "Update");
        NGUITools.ExecuteAll<UIPanel>(root, "LateUpdate");
    }

    public static Vector2 screenSize
    {
        get
        {
            return new Vector2((Single)Screen.width, (Single)Screen.height);
        }
    }

    public static String KeyToCaption(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.None:
                return (String)null;
            case (KeyCode)1:
            case (KeyCode)2:
            case (KeyCode)3:
            case (KeyCode)4:
            case (KeyCode)5:
            case (KeyCode)6:
            case (KeyCode)7:
            case (KeyCode)10:
            case (KeyCode)11:
            case (KeyCode)14:
            case (KeyCode)15:
            case (KeyCode)16:
            case (KeyCode)17:
            case (KeyCode)18:
            case (KeyCode)20:
            case (KeyCode)21:
            case (KeyCode)22:
            case (KeyCode)23:
            case (KeyCode)24:
            case (KeyCode)25:
            case (KeyCode)26:
            case (KeyCode)28:
            case (KeyCode)29:
            case (KeyCode)30:
            case (KeyCode)31:
            case (KeyCode)37:
            case (KeyCode)65:
            case (KeyCode)66:
            case (KeyCode)67:
            case (KeyCode)68:
            case (KeyCode)69:
            case (KeyCode)70:
            case (KeyCode)71:
            case (KeyCode)72:
            case (KeyCode)73:
            case (KeyCode)74:
            case (KeyCode)75:
            case (KeyCode)76:
            case (KeyCode)77:
            case (KeyCode)78:
            case (KeyCode)79:
            case (KeyCode)80:
            case (KeyCode)81:
            case (KeyCode)82:
            case (KeyCode)83:
            case (KeyCode)84:
            case (KeyCode)85:
            case (KeyCode)86:
            case (KeyCode)87:
            case (KeyCode)88:
            case (KeyCode)89:
            case (KeyCode)90:
            case (KeyCode)123:
            case (KeyCode)124:
            case (KeyCode)125:
            case (KeyCode)126:
            IL_208:
                switch (key)
                {
                    case KeyCode.Keypad0:
                        return "K0";
                    case KeyCode.Keypad1:
                        return "K1";
                    case KeyCode.Keypad2:
                        return "K2";
                    case KeyCode.Keypad3:
                        return "K3";
                    case KeyCode.Keypad4:
                        return "K4";
                    case KeyCode.Keypad5:
                        return "K5";
                    case KeyCode.Keypad6:
                        return "K6";
                    case KeyCode.Keypad7:
                        return "K7";
                    case KeyCode.Keypad8:
                        return "K8";
                    case KeyCode.Keypad9:
                        return "K9";
                    case KeyCode.KeypadPeriod:
                        return ".";
                    case KeyCode.KeypadDivide:
                        return "/";
                    case KeyCode.KeypadMultiply:
                        return "*";
                    case KeyCode.KeypadMinus:
                        return "-";
                    case KeyCode.KeypadPlus:
                        return "+";
                    case KeyCode.KeypadEnter:
                        return "NT";
                    case KeyCode.KeypadEquals:
                        return "=";
                    case KeyCode.UpArrow:
                        return "UP";
                    case KeyCode.DownArrow:
                        return "DN";
                    case KeyCode.RightArrow:
                        return "LT";
                    case KeyCode.LeftArrow:
                        return "RT";
                    case KeyCode.Insert:
                        return "Ins";
                    case KeyCode.Home:
                        return "Home";
                    case KeyCode.End:
                        return "End";
                    case KeyCode.PageUp:
                        return "PU";
                    case KeyCode.PageDown:
                        return "PD";
                    case KeyCode.F1:
                        return "F1";
                    case KeyCode.F2:
                        return "F2";
                    case KeyCode.F3:
                        return "F3";
                    case KeyCode.F4:
                        return "F4";
                    case KeyCode.F5:
                        return "F5";
                    case KeyCode.F6:
                        return "F6";
                    case KeyCode.F7:
                        return "F7";
                    case KeyCode.F8:
                        return "F8";
                    case KeyCode.F9:
                        return "F9";
                    case KeyCode.F10:
                        return "F10";
                    case KeyCode.F11:
                        return "F11";
                    case KeyCode.F12:
                        return "F12";
                    case KeyCode.F13:
                        return "F13";
                    case KeyCode.F14:
                        return "F14";
                    case KeyCode.F15:
                        return "F15";
                    case KeyCode.Numlock:
                        return "Num";
                    case KeyCode.CapsLock:
                        return "Cap";
                    case KeyCode.ScrollLock:
                        return "Scr";
                    case KeyCode.RightShift:
                        return "RS";
                    case KeyCode.LeftShift:
                        return "LS";
                    case KeyCode.RightControl:
                        return "RC";
                    case KeyCode.LeftControl:
                        return "LC";
                    case KeyCode.RightAlt:
                        return "RA";
                    case KeyCode.LeftAlt:
                        return "LA";
                    case KeyCode.Mouse0:
                        return "M0";
                    case KeyCode.Mouse1:
                        return "M1";
                    case KeyCode.Mouse2:
                        return "M2";
                    case KeyCode.Mouse3:
                        return "M3";
                    case KeyCode.Mouse4:
                        return "M4";
                    case KeyCode.Mouse5:
                        return "M5";
                    case KeyCode.Mouse6:
                        return "M6";
                    case KeyCode.JoystickButton0:
                        return "(A)";
                    case KeyCode.JoystickButton1:
                        return "(B)";
                    case KeyCode.JoystickButton2:
                        return "(X)";
                    case KeyCode.JoystickButton3:
                        return "(Y)";
                    case KeyCode.JoystickButton4:
                        return "(RB)";
                    case KeyCode.JoystickButton5:
                        return "(LB)";
                    case KeyCode.JoystickButton6:
                        return "(Back)";
                    case KeyCode.JoystickButton7:
                        return "(Start)";
                    case KeyCode.JoystickButton8:
                        return "(LS)";
                    case KeyCode.JoystickButton9:
                        return "(RS)";
                    case KeyCode.JoystickButton10:
                        return "J10";
                    case KeyCode.JoystickButton11:
                        return "J11";
                    case KeyCode.JoystickButton12:
                        return "J12";
                    case KeyCode.JoystickButton13:
                        return "J13";
                    case KeyCode.JoystickButton14:
                        return "J14";
                    case KeyCode.JoystickButton15:
                        return "J15";
                    case KeyCode.JoystickButton16:
                        return "J16";
                    case KeyCode.JoystickButton17:
                        return "J17";
                    case KeyCode.JoystickButton18:
                        return "J18";
                    case KeyCode.JoystickButton19:
                        return "J19";
                }
                return (String)null;
            case KeyCode.Backspace:
                return "BS";
            case KeyCode.Tab:
                return "Tab";
            case KeyCode.Clear:
                return "Clr";
            case KeyCode.Return:
                return "NT";
            case KeyCode.Pause:
                return "PS";
            case KeyCode.Escape:
                return "Esc";
            case KeyCode.Space:
                return "SP";
            case KeyCode.Exclaim:
                return "!";
            case KeyCode.DoubleQuote:
                return "\"";
            case KeyCode.Hash:
                return "#";
            case KeyCode.Dollar:
                return "$";
            case KeyCode.Ampersand:
                return "&";
            case KeyCode.Quote:
                return "'";
            case KeyCode.LeftParen:
                return "(";
            case KeyCode.RightParen:
                return ")";
            case KeyCode.Asterisk:
                return "*";
            case KeyCode.Plus:
                return "+";
            case KeyCode.Comma:
                return ",";
            case KeyCode.Minus:
                return "-";
            case KeyCode.Period:
                return ".";
            case KeyCode.Slash:
                return "/";
            case KeyCode.Alpha0:
                return "0";
            case KeyCode.Alpha1:
                return "1";
            case KeyCode.Alpha2:
                return "2";
            case KeyCode.Alpha3:
                return "3";
            case KeyCode.Alpha4:
                return "4";
            case KeyCode.Alpha5:
                return "5";
            case KeyCode.Alpha6:
                return "6";
            case KeyCode.Alpha7:
                return "7";
            case KeyCode.Alpha8:
                return "8";
            case KeyCode.Alpha9:
                return "9";
            case KeyCode.Colon:
                return ":";
            case KeyCode.Semicolon:
                return ";";
            case KeyCode.Less:
                return "<";
            case KeyCode.Equals:
                return "=";
            case KeyCode.Greater:
                return ">";
            case KeyCode.Question:
                return "?";
            case KeyCode.At:
                return "@";
            case KeyCode.LeftBracket:
                return "[";
            case KeyCode.Backslash:
                return "\\";
            case KeyCode.RightBracket:
                return "]";
            case KeyCode.Caret:
                return "^";
            case KeyCode.Underscore:
                return "_";
            case KeyCode.BackQuote:
                return "`";
            case KeyCode.A:
                return "A";
            case KeyCode.B:
                return "B";
            case KeyCode.C:
                return "C";
            case KeyCode.D:
                return "D";
            case KeyCode.E:
                return "E";
            case KeyCode.F:
                return "F";
            case KeyCode.G:
                return "G";
            case KeyCode.H:
                return "H";
            case KeyCode.I:
                return "I";
            case KeyCode.J:
                return "J";
            case KeyCode.K:
                return "K";
            case KeyCode.L:
                return "L";
            case KeyCode.M:
                return "M";
            case KeyCode.N:
                return "N0";
            case KeyCode.O:
                return "O";
            case KeyCode.P:
                return "P";
            case KeyCode.Q:
                return "Q";
            case KeyCode.R:
                return "R";
            case KeyCode.S:
                return "S";
            case KeyCode.T:
                return "T";
            case KeyCode.U:
                return "U";
            case KeyCode.V:
                return "V";
            case KeyCode.W:
                return "W";
            case KeyCode.X:
                return "X";
            case KeyCode.Y:
                return "Y";
            case KeyCode.Z:
                return "Z";
            case KeyCode.Delete:
                return "Del";
            default:
                goto IL_208;
        }
    }

    private static AudioListener mListener;

    private static Boolean mLoaded = false;

    private static Single mGlobalVolume = 1f;

    private static Single mLastTimestamp = 0f;

    private static AudioClip mLastClip;

    private static Vector3[] mSides = new Vector3[4];

    public static KeyCode[] keys = new KeyCode[]
    {
        KeyCode.Backspace,
        KeyCode.Tab,
        KeyCode.Clear,
        KeyCode.Return,
        KeyCode.Pause,
        KeyCode.Escape,
        KeyCode.Space,
        KeyCode.Exclaim,
        KeyCode.DoubleQuote,
        KeyCode.Hash,
        KeyCode.Dollar,
        KeyCode.Ampersand,
        KeyCode.Quote,
        KeyCode.LeftParen,
        KeyCode.RightParen,
        KeyCode.Asterisk,
        KeyCode.Plus,
        KeyCode.Comma,
        KeyCode.Minus,
        KeyCode.Period,
        KeyCode.Slash,
        KeyCode.Alpha0,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Colon,
        KeyCode.Semicolon,
        KeyCode.Less,
        KeyCode.Equals,
        KeyCode.Greater,
        KeyCode.Question,
        KeyCode.At,
        KeyCode.LeftBracket,
        KeyCode.Backslash,
        KeyCode.RightBracket,
        KeyCode.Caret,
        KeyCode.Underscore,
        KeyCode.BackQuote,
        KeyCode.A,
        KeyCode.B,
        KeyCode.C,
        KeyCode.D,
        KeyCode.E,
        KeyCode.F,
        KeyCode.G,
        KeyCode.H,
        KeyCode.I,
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.M,
        KeyCode.N,
        KeyCode.O,
        KeyCode.P,
        KeyCode.Q,
        KeyCode.R,
        KeyCode.S,
        KeyCode.T,
        KeyCode.U,
        KeyCode.V,
        KeyCode.W,
        KeyCode.X,
        KeyCode.Y,
        KeyCode.Z,
        KeyCode.Delete,
        KeyCode.Keypad0,
        KeyCode.Keypad1,
        KeyCode.Keypad2,
        KeyCode.Keypad3,
        KeyCode.Keypad4,
        KeyCode.Keypad5,
        KeyCode.Keypad6,
        KeyCode.Keypad7,
        KeyCode.Keypad8,
        KeyCode.Keypad9,
        KeyCode.KeypadPeriod,
        KeyCode.KeypadDivide,
        KeyCode.KeypadMultiply,
        KeyCode.KeypadMinus,
        KeyCode.KeypadPlus,
        KeyCode.KeypadEnter,
        KeyCode.KeypadEquals,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.Insert,
        KeyCode.Home,
        KeyCode.End,
        KeyCode.PageUp,
        KeyCode.PageDown,
        KeyCode.F1,
        KeyCode.F2,
        KeyCode.F3,
        KeyCode.F4,
        KeyCode.F5,
        KeyCode.F6,
        KeyCode.F7,
        KeyCode.F8,
        KeyCode.F9,
        KeyCode.F10,
        KeyCode.F11,
        KeyCode.F12,
        KeyCode.F13,
        KeyCode.F14,
        KeyCode.F15,
        KeyCode.Numlock,
        KeyCode.CapsLock,
        KeyCode.ScrollLock,
        KeyCode.RightShift,
        KeyCode.LeftShift,
        KeyCode.RightControl,
        KeyCode.LeftControl,
        KeyCode.RightAlt,
        KeyCode.LeftAlt,
        KeyCode.Mouse3,
        KeyCode.Mouse4,
        KeyCode.Mouse5,
        KeyCode.Mouse6,
        KeyCode.JoystickButton0,
        KeyCode.JoystickButton1,
        KeyCode.JoystickButton2,
        KeyCode.JoystickButton3,
        KeyCode.JoystickButton4,
        KeyCode.JoystickButton5,
        KeyCode.JoystickButton6,
        KeyCode.JoystickButton7,
        KeyCode.JoystickButton8,
        KeyCode.JoystickButton9,
        KeyCode.JoystickButton10,
        KeyCode.JoystickButton11,
        KeyCode.JoystickButton12,
        KeyCode.JoystickButton13,
        KeyCode.JoystickButton14,
        KeyCode.JoystickButton15,
        KeyCode.JoystickButton16,
        KeyCode.JoystickButton17,
        KeyCode.JoystickButton18,
        KeyCode.JoystickButton19
    };
}
