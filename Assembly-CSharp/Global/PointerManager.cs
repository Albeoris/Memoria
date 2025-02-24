using System;
using System.Collections.Generic;
using UnityEngine;

public class PointerManager : Singleton<PointerManager>
{
    public Vector2 PointerOffset
    {
        get => this.pointerOffset;
        set => this.pointerOffset = value;
    }

    public Vector4 PointerLimitRect
    {
        get => this.pointerLimitRect;
        set => this.pointerLimitRect = value;
    }

    public Int32 PointerDepth
    {
        get => this.pointerDepth;
        set => this.pointerDepth = value;
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
            if (this.attachList.TryGetValue(go, out UIPointer pointer))
            {
                pointer.PointerLimitRect = this.pointerLimitRect;
                pointer.PointerOffset = this.pointerOffset;
                this.SetPointerVisibility(go, true);
            }
            else
            {
                pointer = this.GetPointerFromPool(this.PointerDepth);
                pointer.name = go.name + " Pointer";
                pointer.transform.position = go.transform.position;
                pointer.AttachToGameObject(go.transform, !isSnapCenter, this.pointerOffset, this.pointerLimitRect);
                this.attachList.Add(go, pointer);
            }
        }
        else
        {
            global::Debug.LogWarning("AttachPointerToGameObject() : Input GameObject is null!");
        }
    }

    public void SetPointerBlinkAt(GameObject go, Boolean isBlink)
    {
        if (go != null && this.attachList.TryGetValue(go, out UIPointer pointer))
        {
            pointer.SetBlinkActive(isBlink);
            if (isBlink)
                this.SetPointerHelpAt(go, false, true);
        }
    }

    public void SetPointerHelpAt(GameObject go, Boolean isHelp, Boolean isImmediate)
    {
        if (go != null && this.attachList.TryGetValue(go, out UIPointer pointer))
            pointer.SetHelpActive(isHelp, isImmediate);
    }

    public void SetPointerNumberAt(GameObject go, Int32 number)
    {
        if (go != null && this.attachList.TryGetValue(go, out UIPointer pointer))
            pointer.SetNumberActive(true, number);
    }

    public void SetPointerLimitRectBehavior(GameObject go, PointerManager.LimitRectBehavior behavior)
    {
        if (go != null && this.attachList.TryGetValue(go, out UIPointer pointer))
            pointer.Behavior = behavior;
    }

    public void RemovePointerFromGameObject(GameObject go)
    {
        if (go != null && this.attachList.TryGetValue(go, out UIPointer pointer))
        {
            this.ReleasePointerToPool(pointer);
            this.attachList.Remove(go);
        }
    }

    public void SetPointerVisibility(GameObject go, Boolean isVisible)
    {
        if (this.attachList.TryGetValue(go, out UIPointer pointer))
            pointer.SetActive(isVisible);
    }

    public void SetAllPointerVisibility(Boolean isVisible)
    {
        foreach (KeyValuePair<GameObject, UIPointer> keyValuePair in this.attachList)
        {
            UIPointer pointer = keyValuePair.Value;
            pointer.SetActive(isVisible);
            if (!isVisible)
            {
                Singleton<HelpDialog>.Instance.HideDialog();
            }
            else if (isVisible && ButtonGroupState.HelpEnabled)
            {
                ButtonGroupState button = keyValuePair.Key.GetComponent<ButtonGroupState>();
                if (button != null && button.Help.Enable)
                    Singleton<HelpDialog>.Instance.ShowDialog();
            }
        }
    }

    public UIPointer GetPointerFromButton(GameObject go)
    {
        if (this.attachList.TryGetValue(go, out UIPointer pointer))
            return pointer;
        return null;
    }

    private UIPointer GetPointerFromPool(Int32 depth = 5)
    {
        UIPointer pointer;
        if (this.pointerPool.Count > 0)
        {
            GameObject panel = PointerManager.PointerPanelManager.GetPanel(depth);
            pointer = this.pointerPool.Pop();
            pointer.transform.parent = panel.transform;
        }
        else
        {
            GameObject panel = PointerManager.PointerPanelManager.GetPanel(depth);
            GameObject pointerInstance = NGUITools.AddChild(panel, this.PointerPrefab);
            pointer = pointerInstance.GetComponent<UIPointer>();
        }
        this.activePointerList.Push(pointer);
        pointer.SetActive(true);
        return pointer;
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
        foreach (UIPointer pointer in this.activePointerList)
            pointer.Reset();
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
            GameObject pointerPanelGo;
            if (!PointerManager.PointerPanelManager.pointerPanelPool.TryGetValue(depth, out pointerPanelGo))
            {
                pointerPanelGo = NGUITools.AddChild(Singleton<PointerManager>.Instance.gameObject, Singleton<PointerManager>.Instance.PointerPanelPrefab);
                pointerPanelGo.GetComponent<UIPanel>().depth = depth;
                PointerManager.PointerPanelManager.pointerPanelPool.Add(depth, pointerPanelGo);
                return pointerPanelGo;
            }
            if (pointerPanelGo != null)
                return pointerPanelGo;
            PointerManager.PointerPanelManager.pointerPanelPool.Remove(depth);
            pointerPanelGo = NGUITools.AddChild(Singleton<PointerManager>.Instance.gameObject, Singleton<PointerManager>.Instance.PointerPanelPrefab);
            pointerPanelGo.GetComponent<UIPanel>().depth = depth;
            PointerManager.PointerPanelManager.pointerPanelPool.Add(depth, pointerPanelGo);
            return pointerPanelGo;
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
