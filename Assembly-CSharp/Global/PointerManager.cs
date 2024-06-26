using System;
using System.Collections.Generic;
using UnityEngine;

public class PointerManager : Singleton<PointerManager>
{
    public Vector2 PointerOffset
    {
        get
        {
            return this.pointerOffset;
        }
        set
        {
            this.pointerOffset = value;
        }
    }

    public Vector4 PointerLimitRect
    {
        get
        {
            return this.pointerLimitRect;
        }
        set
        {
            this.pointerLimitRect = value;
        }
    }

    public Int32 PointerDepth
    {
        get
        {
            return this.pointerDepth;
        }
        set
        {
            this.pointerDepth = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        this.pointerOffset = new Vector2(0f, 0f);
        this.pointerLimitRect = UIManager.UIScreenCoOrdinate;
        this.pointerPool = new List<UIPointer>();
        this.activePointerList = new List<UIPointer>();
        this.attachList = new Dictionary<GameObject, UIPointer>();
    }

    public void AttachPointerToGameObject(GameObject go)
    {
        this.AttachPointerToGameObject(go, false);
    }

    public void AttachPointerToGameObject(GameObject go, Boolean isSnapCenter)
    {
        if (go != null)
        {
            if (!this.attachList.ContainsKey(go))
            {
                UIPointer pointerFromPool = this.GetPointerFromPool(this.PointerDepth);
                pointerFromPool.name = go.name + " Pointer";
                pointerFromPool.transform.position = go.transform.position;
                pointerFromPool.AttachToGameObject(go.transform, !isSnapCenter, this.pointerOffset, this.pointerLimitRect);
                this.attachList.Add(go, pointerFromPool);
            }
            else
            {
                UIPointer uipointer = this.attachList[go];
                uipointer.PointerLimitRect = this.pointerLimitRect;
                uipointer.PointerOffset = this.pointerOffset;
                this.SetPointerVisibility(go, true);
            }
        }
        else
        {
            global::Debug.LogWarning("AttachPointerToGameObject() : Input GameObject is null!");
        }
    }

    public void SetPointerBlinkAt(GameObject go, Boolean isBlink)
    {
        if (go != (UnityEngine.Object)null && this.attachList.ContainsKey(go))
        {
            UIPointer uipointer = this.attachList[go];
            uipointer.SetBlinkActive(isBlink);
            if (isBlink)
            {
                this.SetPointerHelpAt(go, false, true);
            }
        }
    }

    public void SetPointerHelpAt(GameObject go, Boolean isHelp, Boolean isImmediate)
    {
        if (go != (UnityEngine.Object)null && this.attachList.ContainsKey(go))
        {
            UIPointer uipointer = this.attachList[go];
            uipointer.SetHelpActive(isHelp, isImmediate);
        }
    }

    public void SetPointerNumberAt(GameObject go, Int32 number)
    {
        if (go != (UnityEngine.Object)null && this.attachList.ContainsKey(go))
        {
            UIPointer uipointer = this.attachList[go];
            uipointer.SetNumberActive(true, number);
        }
    }

    public void SetPointerLimitRectBehavior(GameObject go, PointerManager.LimitRectBehavior behavior)
    {
        if (go != (UnityEngine.Object)null && this.attachList.ContainsKey(go))
        {
            UIPointer uipointer = this.attachList[go];
            uipointer.Behavior = behavior;
        }
    }

    public void RemovePointerFromGameObject(GameObject go)
    {
        UIPointer pointer;
        if (go != null && this.attachList.TryGetValue(go, out pointer))
        {
            this.ReleasePointerToPool(pointer);
            this.attachList.Remove(go);
        }
    }

    public void SetPointerVisibility(GameObject go, Boolean isVisible)
    {
        if (this.attachList.ContainsKey(go))
        {
            UIPointer uipointer = this.attachList[go];
            uipointer.SetActive(isVisible);
        }
    }

    public void SetAllPointerVisibility(Boolean isVisible)
    {
        foreach (KeyValuePair<GameObject, UIPointer> keyValuePair in this.attachList)
        {
            UIPointer value = keyValuePair.Value;
            value.SetActive(isVisible);
            if (!isVisible)
            {
                Singleton<HelpDialog>.Instance.HideDialog();
            }
            else if (isVisible && ButtonGroupState.HelpEnabled)
            {
                ButtonGroupState component = keyValuePair.Key.GetComponent<ButtonGroupState>();
                if (component != (UnityEngine.Object)null && component.Help.Enable)
                {
                    Singleton<HelpDialog>.Instance.ShowDialog();
                }
            }
        }
    }

    public UIPointer GetPointerFromButton(GameObject go)
    {
        if (this.attachList.ContainsKey(go))
        {
            return this.attachList[go];
        }
        return (UIPointer)null;
    }

    private UIPointer GetPointerFromPool(Int32 depth = 5)
    {
        UIPointer uipointer;
        if (this.pointerPool.Count > 0)
        {
            GameObject panel = PointerManager.PointerPanelManager.GetPanel(depth);
            uipointer = this.pointerPool.Pop<UIPointer>();
            uipointer.transform.parent = panel.transform;
        }
        else
        {
            GameObject panel2 = PointerManager.PointerPanelManager.GetPanel(depth);
            GameObject gameObject = NGUITools.AddChild(panel2, this.PointerPrefab);
            uipointer = gameObject.GetComponent<UIPointer>();
        }
        this.activePointerList.Push(uipointer);
        uipointer.SetActive(true);
        return uipointer;
    }

    public void RemoveAllPointer()
    {
        this.ReleaseAllPointersToPool();
        this.attachList.Clear();
    }

    public void ReleasePointerToPool(UIPointer pointer)
    {
        pointer.Reset();
        this.activePointerList.Remove(pointer);
        this.pointerPool.Push(pointer);
    }

    private void ReleaseAllPointersToPool()
    {
        foreach (UIPointer uipointer in this.activePointerList)
        {
            uipointer.Reset();
        }
        this.pointerPool.AddRange(this.activePointerList);
        this.activePointerList.Clear();
    }

    public GameObject PointerPrefab;

    public GameObject PointerPanelPrefab;

    public static Vector2 PointerSize = new Vector2(114f, 62f);

    private List<UIPointer> pointerPool;

    private List<UIPointer> activePointerList;

    private Dictionary<GameObject, UIPointer> attachList;

    private Vector2 pointerOffset;

    private Vector4 pointerLimitRect;

    private Int32 pointerDepth = 5;

    private class PointerPanelManager
    {
        public static GameObject GetPanel(Int32 depth)
        {
            GameObject gameObject;
            if (!PointerManager.PointerPanelManager.pointerPanelPool.TryGetValue(depth, out gameObject))
            {
                gameObject = NGUITools.AddChild(Singleton<PointerManager>.Instance.gameObject, Singleton<PointerManager>.Instance.PointerPanelPrefab);
                gameObject.GetComponent<UIPanel>().depth = depth;
                PointerManager.PointerPanelManager.pointerPanelPool.Add(depth, gameObject);
                return gameObject;
            }
            if (gameObject != (UnityEngine.Object)null)
            {
                return gameObject;
            }
            PointerManager.PointerPanelManager.pointerPanelPool.Remove(depth);
            gameObject = NGUITools.AddChild(Singleton<PointerManager>.Instance.gameObject, Singleton<PointerManager>.Instance.PointerPanelPrefab);
            gameObject.GetComponent<UIPanel>().depth = depth;
            PointerManager.PointerPanelManager.pointerPanelPool.Add(depth, gameObject);
            return gameObject;
        }

        private static Dictionary<Int32, GameObject> pointerPanelPool = new Dictionary<Int32, GameObject>();
    }

    public enum LimitRectBehavior
    {
        Limit,
        Hide,
        None
    }
}
