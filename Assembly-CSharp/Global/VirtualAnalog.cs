using System;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAnalog : Singleton<VirtualAnalog>
{
    public static Boolean IsEnable
    {
        get
        {
            return Singleton<VirtualAnalog>.Instance.isEnable;
        }
    }

    public Boolean IsActive
    {
        get
        {
            return this.isActive;
        }
    }

    public Boolean IsShown
    {
        get
        {
            return this.isShown;
        }
        private set
        {
            if (value != this.isShown)
            {
                this.isShown = value;
                if (FF9StateSystem.MobilePlatform)
                {
                    this.Background.SetActive(value);
                    this.Center.SetActive(value);
                }
            }
        }
    }

    public static List<GameObject> FallbackTouchWidgetList
    {
        get
        {
            return Singleton<VirtualAnalog>.Instance.fallbackTouchWidgetList;
        }
        set
        {
            Singleton<VirtualAnalog>.Instance.fallbackTouchWidgetList = value;
        }
    }

    public static Vector2 Value
    {
        get
        {
            return Singleton<VirtualAnalog>.Instance.GetAnalogValueInst().normalized * Singleton<VirtualAnalog>.Instance.GetMagnitudeRatioInst();
        }
    }

    private void SetActive(Boolean value)
    {
        if (this.isEnable)
        {
            this.IsShown = false;
            this.curTouch.fingerId = -1;
            this.hasMouseInput = false;
            this.isActive = value;
        }
    }

    public Vector2 GetAnalogValueInst()
    {
        Vector2 result = this.centerPos - this.position;
        if (Mathf.Abs(result.x) <= this.deadZoneRadius)
        {
            result.x = 0f;
        }
        if (Mathf.Abs(result.y) <= this.deadZoneRadius)
        {
            result.y = 0f;
        }
        return result;
    }

    public static Vector2 GetAnalogValue()
    {
        return Singleton<VirtualAnalog>.Instance.GetAnalogValueInst();
    }

    public Boolean HasInputInst()
    {
        return this.isEnable && this.hasMoved && this.curTouch.fingerId != -1;
    }

    public static Boolean HasInput()
    {
        return Singleton<VirtualAnalog>.Instance.HasInputInst() && FF9StateSystem.MobilePlatform;
    }

    public static Boolean GetTap()
    {
        return Singleton<VirtualAnalog>.Instance.isTouchUp && !Singleton<VirtualAnalog>.Instance.lastHasMoved;
    }

    public static Boolean GetTouchUp()
    {
        return Singleton<VirtualAnalog>.Instance.isTouchUp;
    }

    public static Boolean GetTouchDown()
    {
        return Singleton<VirtualAnalog>.Instance.isTouchDown;
    }

    private void SetPosition(Vector2 pos)
    {
        this.position = pos;
        this.centerPos = pos;
        this.Background.transform.localPosition = new Vector3(pos.x, pos.y);
        this.Center.transform.localPosition = new Vector3(pos.x, pos.y);
    }

    public Vector2 GetDirectionInst()
    {
        return (this.centerPos - this.position).normalized;
    }

    public Vector2 GetDirection()
    {
        return Singleton<VirtualAnalog>.Instance.GetAnalogValueInst();
    }

    public Single GetMagnitudeRatioInst()
    {
        Single magnitude = (this.centerPos - this.position).magnitude;
        return magnitude / this.radius;
    }

    public static Single GetMagnitudeRatio()
    {
        return Singleton<VirtualAnalog>.Instance.GetMagnitudeRatioInst();
    }

    public Single GetRadiusInst()
    {
        return this.radius;
    }

    public static Single GetRadius()
    {
        return Singleton<VirtualAnalog>.Instance.GetRadiusInst();
    }

    protected override void Awake()
    {
        this.isActive = true;
        this.proportionY = 0.25f;
        this.Restart();
        this.SetPosition(new Vector2((Single)(Screen.width / 2), (Single)(Screen.height / 2)));
        this.touches = new List<VirtualAnalog.TouchWritable>();
        this.curTouch = new VirtualAnalog.TouchWritable();
        this.hasMouseInput = false;
        this.hasMoved = false;
        this.SetActive(false);
        this.lastHasMoved = this.hasMoved;
    }

    public static void Init(GameObject containerObject)
    {
        Singleton<VirtualAnalog>.Instance.fallbackTouchWidgetList.Clear();
        Singleton<VirtualAnalog>.Instance.sceneGameObject = containerObject;
        Singleton<VirtualAnalog>.Instance.UpdateProportion();
        Singleton<VirtualAnalog>.Instance.ResetValue();
    }

    public static void Enable()
    {
        if (!Singleton<VirtualAnalog>.Instance.isEnable)
        {
            Singleton<VirtualAnalog>.Instance.isEnable = true;
            Singleton<VirtualAnalog>.Instance.SetActive(false);
        }
    }

    public static void Disable()
    {
        if (Singleton<VirtualAnalog>.Instance.isEnable)
        {
            Singleton<VirtualAnalog>.Instance.SetActive(false);
            Singleton<VirtualAnalog>.Instance.isEnable = false;
            Singleton<VirtualAnalog>.Instance.ResetValue();
        }
    }

    public void Restart()
    {
        Single num = (Single)Screen.height / OverlayCanvas.ReferenceScreenSize.y;
        this.Background.transform.localScale = new Vector3(num, num, num);
        this.Center.transform.localScale = new Vector3(num, num, num);
        this.UpdateProportion();
    }

    private void ResetValue()
    {
        this.centerPos = this.position;
    }

    private void UpdateProportion()
    {
        this.radius = this.Background.GetComponent<RectTransform>().rect.height * this.Background.transform.localScale.y * 0.55f;
        this.maxRadius = this.radius * 1f;
        this.deadZoneRadius = this.radius * 0.25f;
    }

    private void Update()
    {
        if (this.isEnable)
        {
            if (Input.GetMouseButtonUp(0) && this.isActive)
            {
                this.SetActive(false);
                this.isActive = false;
                this.centerPos = this.position;
                this.isTouchUp = true;
            }
            else if (Input.GetMouseButtonDown(0) && !this.isActive)
            {
                Vector2 vector = this.ConvertPosition(Input.mousePosition);
                if (UICamera.selectedObject == (UnityEngine.Object)null || this.fallbackTouchWidgetList.Contains(UICamera.selectedObject))
                {
                    this.SetActive(true);
                    this.isActive = true;
                    this.SetPosition(vector);
                    this.isTouchDown = true;
                }
            }
            this.RunMechanic();
        }
    }

    private void LateUpdate()
    {
        if (this.isEnable)
        {
            this.isTouchDown = false;
            this.isTouchUp = false;
            this.lastHasMoved = this.hasMoved;
        }
    }

    private void RunMechanic()
    {
        if (!this.IsActive)
        {
            this.curTouch.fingerId = -1;
            this.hasMoved = false;
            return;
        }
        this.IsShown = this.hasMoved;
        this.GenerateTouches();
        for (Int32 i = 0; i < this.touches.Count; i++)
        {
            VirtualAnalog.TouchWritable touchWritable = this.touches[i];
            if (touchWritable.phase == TouchPhase.Began && !this.hasMoved)
            {
                this.curTouch = touchWritable;
                this.centerPos = this.curTouch.position;
            }
            if (touchWritable.fingerId == this.curTouch.fingerId)
            {
                this.curTouch = touchWritable;
            }
        }
        if (this.touches.Count == 0 || this.curTouch.phase == TouchPhase.Ended || this.curTouch.phase == TouchPhase.Canceled)
        {
            this.centerPos = this.position;
            return;
        }
        if (this.curTouch.phase == TouchPhase.Moved || this.curTouch.phase == TouchPhase.Stationary)
        {
            this.centerPos = this.curTouch.position;
            Vector2 vector = this.centerPos - this.position;
            Single num = this.maxRadius * this.maxRadius;
            if (vector.sqrMagnitude >= num)
            {
                vector.Normalize();
                vector *= this.maxRadius;
                this.centerPos = this.position + vector;
            }
        }
        if (!this.hasMoved)
        {
            Vector2 vector2 = this.centerPos - this.position;
            Single num2 = this.deadZoneRadius * this.deadZoneRadius;
            if (vector2.sqrMagnitude >= num2)
            {
                this.hasMoved = true;
            }
        }
        this.Center.transform.localPosition = this.centerPos;
    }

    private void GenerateTouches()
    {
        this.touches.Clear();
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.hasMouseInput = true;
                VirtualAnalog.TouchWritable touchWritable = new VirtualAnalog.TouchWritable();
                touchWritable.position = this.ConvertPosition(Input.mousePosition);
                touchWritable.phase = TouchPhase.Began;
                touchWritable.fingerId = -999;
                this.touches.Add(touchWritable);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                this.hasMouseInput = false;
                VirtualAnalog.TouchWritable touchWritable2 = new VirtualAnalog.TouchWritable();
                touchWritable2.position = this.ConvertPosition(Input.mousePosition);
                touchWritable2.phase = TouchPhase.Ended;
                touchWritable2.fingerId = -999;
                this.touches.Add(touchWritable2);
            }
            else if (this.hasMouseInput)
            {
                VirtualAnalog.TouchWritable touchWritable3 = new VirtualAnalog.TouchWritable();
                touchWritable3.position = this.ConvertPosition(Input.mousePosition);
                touchWritable3.phase = TouchPhase.Moved;
                touchWritable3.fingerId = -999;
                this.touches.Add(touchWritable3);
            }
        }
        for (Int32 i = 0; i < Input.touchCount; i++)
        {
            VirtualAnalog.TouchWritable touchWritable4 = new VirtualAnalog.TouchWritable();
            touchWritable4.CopyFrom(Input.GetTouch(i));
            touchWritable4.position = this.ConvertPosition(touchWritable4.position);
            this.touches.Add(touchWritable4);
        }
    }

    private Vector2 ConvertPosition(Vector2 position)
    {
        position.x -= (Single)Screen.width / 2f;
        position.y -= (Single)Screen.height / 2f;
        return position;
    }

    public Vector2 GetAxesRawInst()
    {
        Single y;
        Single x;
        if (this.HasInputInst())
        {
            Vector2 analogValueInst = this.GetAnalogValueInst();
            y = analogValueInst.y;
            x = analogValueInst.x;
        }
        else
        {
            y = Input.GetAxisRaw("Vertical");
            x = Input.GetAxisRaw("Horizontal");
        }
        return new Vector2(x, y);
    }

    public static Vector2 GetAxesRaw()
    {
        return Singleton<VirtualAnalog>.Instance.GetAxesRawInst();
    }

    public GameObject Background;

    public GameObject Center;

    private GameObject sceneGameObject;

    private Boolean isEnable;

    private Boolean isActive;

    private Boolean isShown;

    public Single proportionY;

    public Single radius;

    public Single deadZoneRadius;

    public Single maxRadius;

    public Vector2 position;

    public Vector2 centerPos;

    private List<VirtualAnalog.TouchWritable> touches;

    private VirtualAnalog.TouchWritable curTouch;

    private Boolean hasMouseInput;

    private Boolean hasMoved;

    public Boolean PreventFromSetActive;

    private Boolean isTouchUp;

    private Boolean isTouchDown;

    private List<GameObject> fallbackTouchWidgetList = new List<GameObject>();

    private Boolean lastHasMoved;

    private class TouchWritable
    {
        public TouchWritable()
        {
            this.fingerId = -1;
            this.phase = TouchPhase.Ended;
            this.position = Vector3.zero;
        }

        public void CopyFrom(Touch reference)
        {
            this.fingerId = reference.fingerId;
            this.phase = reference.phase;
            this.position = reference.position;
        }

        public Int32 fingerId;

        public TouchPhase phase;

        public Vector2 position;
    }
}
