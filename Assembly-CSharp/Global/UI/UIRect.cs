using System;
using System.IO;
using UnityEngine;

public abstract class UIRect : MonoBehaviour
{
	public GameObject cachedGameObject
	{
		get
		{
			if (this.mGo == (UnityEngine.Object)null)
			{
				this.mGo = base.gameObject;
			}
			return this.mGo;
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (this.mTrans == (UnityEngine.Object)null)
			{
				this.mTrans = base.transform;
			}
			return this.mTrans;
		}
	}

	public Camera anchorCamera
	{
		get
		{
			if (!this.mAnchorsCached)
			{
				this.ResetAnchors();
			}
			return this.mCam;
		}
	}

	public Boolean isFullyAnchored
	{
		get
		{
			return this.leftAnchor.target && this.rightAnchor.target && this.topAnchor.target && this.bottomAnchor.target;
		}
	}

	public virtual Boolean isAnchoredHorizontally
	{
		get
		{
			return this.leftAnchor.target || this.rightAnchor.target;
		}
	}

	public virtual Boolean isAnchoredVertically
	{
		get
		{
			return this.bottomAnchor.target || this.topAnchor.target;
		}
	}

	public virtual Boolean canBeAnchored
	{
		get
		{
			return true;
		}
	}

	public UIRect parent
	{
		get
		{
			if (!this.mParentFound)
			{
				this.mParentFound = true;
				this.mParent = NGUITools.FindInParents<UIRect>(this.cachedTransform.parent);
			}
			return this.mParent;
		}
	}

	public UIRoot root
	{
		get
		{
			if (this.parent != (UnityEngine.Object)null)
			{
				return this.mParent.root;
			}
			if (!this.mRootSet)
			{
				this.mRootSet = true;
				this.mRoot = NGUITools.FindInParents<UIRoot>(this.cachedTransform);
			}
			return this.mRoot;
		}
	}

	public Boolean isAnchored
	{
		get
		{
			return (this.leftAnchor.target || this.rightAnchor.target || this.topAnchor.target || this.bottomAnchor.target) && this.canBeAnchored;
		}
	}

	public abstract Single alpha { get; set; }

	public abstract Single CalculateFinalAlpha(Int32 frameID);

	public abstract Vector3[] localCorners { get; }

	public abstract Vector3[] worldCorners { get; }

	protected Single cameraRayDistance
	{
		get
		{
			if (this.anchorCamera == (UnityEngine.Object)null)
			{
				return 0f;
			}
			if (!this.mCam.orthographic)
			{
				Transform cachedTransform = this.cachedTransform;
				Transform transform = this.mCam.transform;
				Plane plane = new Plane(cachedTransform.rotation * Vector3.back, cachedTransform.position);
				Ray ray = new Ray(transform.position, transform.rotation * Vector3.forward);
				Single result;
				if (plane.Raycast(ray, out result))
				{
					return result;
				}
			}
			return Mathf.Lerp(this.mCam.nearClipPlane, this.mCam.farClipPlane, 0.5f);
		}
	}

	public virtual void Invalidate(Boolean includeChildren)
	{
		this.mChanged = true;
		if (includeChildren)
		{
			for (Int32 i = 0; i < this.mChildren.size; i++)
			{
				this.mChildren.buffer[i].Invalidate(true);
			}
		}
	}

	public virtual Vector3[] GetSides(Transform relativeTo)
	{
		if (this.anchorCamera != (UnityEngine.Object)null)
		{
			return this.mCam.GetSides(this.cameraRayDistance, relativeTo);
		}
		Vector3 position = this.cachedTransform.position;
		for (Int32 i = 0; i < 4; i++)
		{
			UIRect.mSides[i] = position;
		}
		if (relativeTo != (UnityEngine.Object)null)
		{
			for (Int32 j = 0; j < 4; j++)
			{
				UIRect.mSides[j] = relativeTo.InverseTransformPoint(UIRect.mSides[j]);
			}
		}
		return UIRect.mSides;
	}

	protected Vector3 GetLocalPos(UIRect.AnchorPoint ac, Transform trans)
	{
		if (this.anchorCamera == (UnityEngine.Object)null || ac.targetCam == (UnityEngine.Object)null)
		{
			return this.cachedTransform.localPosition;
		}
		Rect rect = ac.targetCam.rect;
		Vector3 vector = ac.targetCam.WorldToViewportPoint(ac.target.position);
		Vector3 vector2 = new Vector3(vector.x * rect.width + rect.x, vector.y * rect.height + rect.y, vector.z);
		vector2 = this.mCam.ViewportToWorldPoint(vector2);
		if (trans != (UnityEngine.Object)null)
		{
			vector2 = trans.InverseTransformPoint(vector2);
		}
		vector2.x = Mathf.Floor(vector2.x + 0.5f);
		vector2.y = Mathf.Floor(vector2.y + 0.5f);
		return vector2;
	}

	protected virtual void OnEnable()
	{
		this.mUpdateFrame = -1;
		if (this.updateAnchors == UIRect.AnchorUpdate.OnEnable)
		{
			this.mAnchorsCached = false;
			this.mUpdateAnchors = true;
		}
		if (this.mStarted)
		{
			this.OnInit();
		}
		this.mUpdateFrame = -1;
	}

	protected virtual void OnInit()
	{
		this.mChanged = true;
		this.mRootSet = false;
		this.mParentFound = false;
		if (this.parent != (UnityEngine.Object)null)
		{
			this.mParent.mChildren.Add(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (this.mParent)
		{
			this.mParent.mChildren.Remove(this);
		}
		this.mParent = (UIRect)null;
		this.mRoot = (UIRoot)null;
		this.mRootSet = false;
		this.mParentFound = false;
	}

	protected void Start()
	{
		this.mStarted = true;
		this.OnInit();
		this.OnStart();
	}

	public void Update()
	{
		if (!this.mAnchorsCached)
		{
			this.ResetAnchors();
		}
		Int32 frameCount = Time.frameCount;
		if (this.mUpdateFrame != frameCount)
		{
			if (this.updateAnchors == UIRect.AnchorUpdate.OnUpdate || this.mUpdateAnchors)
			{
				this.UpdateAnchorsInternal(frameCount);
			}
			this.OnUpdate();
		}
	}

	protected void UpdateAnchorsInternal(Int32 frame)
	{
		this.mUpdateFrame = frame;
		this.mUpdateAnchors = false;
		Boolean flag = false;
		if (this.leftAnchor.target)
		{
			flag = true;
			if (this.leftAnchor.rect != (UnityEngine.Object)null && this.leftAnchor.rect.mUpdateFrame != frame)
			{
				this.leftAnchor.rect.Update();
			}
		}
		if (this.bottomAnchor.target)
		{
			flag = true;
			if (this.bottomAnchor.rect != (UnityEngine.Object)null && this.bottomAnchor.rect.mUpdateFrame != frame)
			{
				this.bottomAnchor.rect.Update();
			}
		}
		if (this.rightAnchor.target)
		{
			flag = true;
			if (this.rightAnchor.rect != (UnityEngine.Object)null && this.rightAnchor.rect.mUpdateFrame != frame)
			{
				this.rightAnchor.rect.Update();
			}
		}
		if (this.topAnchor.target)
		{
			flag = true;
			if (this.topAnchor.rect != (UnityEngine.Object)null && this.topAnchor.rect.mUpdateFrame != frame)
			{
				this.topAnchor.rect.Update();
			}
		}
		if (flag)
		{
			this.OnAnchor();
		}
	}

	public void UpdateAnchors()
	{
		if (this.isAnchored)
		{
			this.mUpdateFrame = -1;
			this.mUpdateAnchors = true;
			this.UpdateAnchorsInternal(Time.frameCount);
		}
	}

	protected abstract void OnAnchor();

	public void SetAnchor(Transform t)
	{
		this.leftAnchor.target = t;
		this.rightAnchor.target = t;
		this.topAnchor.target = t;
		this.bottomAnchor.target = t;
		this.ResetAnchors();
		this.UpdateAnchors();
	}

	public void SetAnchor(GameObject go)
	{
		SetAnchor(go?.transform);
	}

	public void SetAnchor(Transform target = null, Single relLeft = 0f, Single relBottom = 0f, Single relRight = 1f, Single relTop = 1f, Single left = 0f, Single bottom = 0f, Single right = 0f, Single top = 0f)
	{
		this.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
		this.leftAnchor.target = target;
		this.rightAnchor.target = target;
		this.topAnchor.target = target;
		this.bottomAnchor.target = target;
		this.leftAnchor.relative = relLeft;
		this.rightAnchor.relative = relRight;
		this.bottomAnchor.relative = relBottom;
		this.topAnchor.relative = relTop;
		this.leftAnchor.absolute = (Int32)left;
		this.rightAnchor.absolute = (Int32)right;
		this.bottomAnchor.absolute = (Int32)bottom;
		this.topAnchor.absolute = (Int32)top;
		this.ResetAnchors();
		this.UpdateAnchors();
	}

	public void SetAnchor(GameObject go = null, Single relLeft = 0f, Single relBottom = 0f, Single relRight = 1f, Single relTop = 1f, Single left = 0f, Single bottom = 0f, Single right = 0f, Single top = 0f)
	{
		SetAnchor(go?.transform, relLeft, relBottom, relRight, relTop, left, bottom, right, top);
	}

	public void ResetAnchors()
	{
		this.mAnchorsCached = true;
		this.leftAnchor.rect = ((!this.leftAnchor.target) ? null : this.leftAnchor.target.GetComponent<UIRect>());
		this.bottomAnchor.rect = ((!this.bottomAnchor.target) ? null : this.bottomAnchor.target.GetComponent<UIRect>());
		this.rightAnchor.rect = ((!this.rightAnchor.target) ? null : this.rightAnchor.target.GetComponent<UIRect>());
		this.topAnchor.rect = ((!this.topAnchor.target) ? null : this.topAnchor.target.GetComponent<UIRect>());
		this.mCam = NGUITools.FindCameraForLayer(this.cachedGameObject.layer);
		this.FindCameraFor(this.leftAnchor);
		this.FindCameraFor(this.bottomAnchor);
		this.FindCameraFor(this.rightAnchor);
		this.FindCameraFor(this.topAnchor);
		this.mUpdateAnchors = true;
	}

	public void ResetAndUpdateAnchors()
	{
		this.ResetAnchors();
		this.UpdateAnchors();
	}

	public abstract void SetRect(Single x, Single y, Single width, Single height);

	private void FindCameraFor(UIRect.AnchorPoint ap)
	{
		if (ap.target == (UnityEngine.Object)null || ap.rect != (UnityEngine.Object)null)
		{
			ap.targetCam = (Camera)null;
		}
		else
		{
			ap.targetCam = NGUITools.FindCameraForLayer(ap.target.gameObject.layer);
		}
	}

	public virtual void ParentHasChanged()
	{
		this.mParentFound = false;
		UIRect y = NGUITools.FindInParents<UIRect>(this.cachedTransform.parent);
		if (this.mParent != y)
		{
			if (this.mParent)
			{
				this.mParent.mChildren.Remove(this);
			}
			this.mParent = y;
			if (this.mParent)
			{
				this.mParent.mChildren.Add(this);
			}
			this.mRootSet = false;
		}
	}

	protected abstract void OnStart();

	protected virtual void OnUpdate()
	{
	}

	public UIRect.AnchorPoint leftAnchor = new UIRect.AnchorPoint();

	public UIRect.AnchorPoint rightAnchor = new UIRect.AnchorPoint(1f);

	public UIRect.AnchorPoint bottomAnchor = new UIRect.AnchorPoint();

	public UIRect.AnchorPoint topAnchor = new UIRect.AnchorPoint(1f);

	public UIRect.AnchorUpdate updateAnchors = UIRect.AnchorUpdate.OnUpdate;

	protected GameObject mGo;

	protected Transform mTrans;

	protected BetterList<UIRect> mChildren = new BetterList<UIRect>();

	protected Boolean mChanged = true;

	protected Boolean mStarted;

	protected Boolean mParentFound;

	[NonSerialized]
	private Boolean mUpdateAnchors = true;

	[NonSerialized]
	private Int32 mUpdateFrame = -1;

	[NonSerialized]
	private Boolean mAnchorsCached;

	[NonSerialized]
	private UIRoot mRoot;

	[NonSerialized]
	private UIRect mParent;

	[NonSerialized]
	private Boolean mRootSet;

	[NonSerialized]
	protected Camera mCam;

	[NonSerialized]
	public Single finalAlpha = 1f;

	protected static Vector3[] mSides = new Vector3[4];

    public Position LeftAnchorPosition
    {
        get { return new Position(leftAnchor.relative, leftAnchor.absolute); }
        set
        {
            leftAnchor.relative = value.Relative;
            leftAnchor.absolute = value.Absolute;
        }
    }

    public Position RightAnchorPosition
    {
        get { return new Position(rightAnchor.relative, rightAnchor.absolute); }
        set
        {
            rightAnchor.relative = value.Relative;
            rightAnchor.absolute = value.Absolute;
        }
    }

    public Position TopAnchorPosition
    {
        get { return new Position(topAnchor.relative, topAnchor.absolute); }
        set
        {
            topAnchor.relative = value.Relative;
            topAnchor.absolute = value.Absolute;
        }
    }

    public Position BottomAnchorPosition
    {
        get { return new Position(bottomAnchor.relative, bottomAnchor.absolute); }
        set
        {
            bottomAnchor.relative = value.Relative;
            bottomAnchor.absolute = value.Absolute;
        }
    }

    public struct Position
    {
        public readonly Single Relative;
        public readonly Int32 Absolute;

        public Position(Single relative, Int32 absolute)
        {
            Relative = relative;
            Absolute = absolute;
        }

        public override Boolean Equals(System.Object obj)
        {
            if (!(obj is Position))
                return false;

            return Equals((Position)obj);
        }

        public Boolean Equals(Position other)
        {
            return Relative.Equals(other.Relative) && Absolute.Equals(other.Absolute);
        }

        public override Int32 GetHashCode()
        {
            return unchecked((Relative.GetHashCode() * 397) ^ Absolute.GetHashCode());
        }

        public static Boolean operator ==(Position x, Position y)
        {
            return x.Equals(y);
        }

        public static Boolean operator !=(Position x, Position y)
        {
            return !x.Equals(y);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Relative);
            bw.Write(Absolute);
        }

        public static Position Read(BinaryReader br)
        {
            return new Position(br.ReadSingle(), br.ReadInt32());
        }
    }

    [Serializable]
	public class AnchorPoint
	{
		public AnchorPoint()
		{
		}

		public AnchorPoint(Single relative)
		{
			this.relative = relative;
		}

		public void Set(Single relative, Single absolute)
		{
			this.relative = relative;
			this.absolute = Mathf.FloorToInt(absolute + 0.5f);
		}

		public void Set(Transform target, Single relative, Single absolute)
		{
			this.target = target;
			this.relative = relative;
			this.absolute = Mathf.FloorToInt(absolute + 0.5f);
		}

		public void SetToNearest(Single abs0, Single abs1, Single abs2)
		{
			this.SetToNearest(0f, 0.5f, 1f, abs0, abs1, abs2);
		}

		public void SetToNearest(Single rel0, Single rel1, Single rel2, Single abs0, Single abs1, Single abs2)
		{
			Single num = Mathf.Abs(abs0);
			Single num2 = Mathf.Abs(abs1);
			Single num3 = Mathf.Abs(abs2);
			if (num < num2 && num < num3)
			{
				this.Set(rel0, abs0);
			}
			else if (num2 < num && num2 < num3)
			{
				this.Set(rel1, abs1);
			}
			else
			{
				this.Set(rel2, abs2);
			}
		}

		public void SetHorizontal(Transform parent, Single localPos)
		{
			if (this.rect)
			{
				Vector3[] sides = this.rect.GetSides(parent);
				Single num = Mathf.Lerp(sides[0].x, sides[2].x, this.relative);
				this.absolute = Mathf.FloorToInt(localPos - num + 0.5f);
			}
			else
			{
				Vector3 position = this.target.position;
				if (parent != (UnityEngine.Object)null)
				{
					position = parent.InverseTransformPoint(position);
				}
				this.absolute = Mathf.FloorToInt(localPos - position.x + 0.5f);
			}
		}

		public void SetVertical(Transform parent, Single localPos)
		{
			if (this.rect)
			{
				Vector3[] sides = this.rect.GetSides(parent);
				Single num = Mathf.Lerp(sides[3].y, sides[1].y, this.relative);
				this.absolute = Mathf.FloorToInt(localPos - num + 0.5f);
			}
			else
			{
				Vector3 position = this.target.position;
				if (parent != (UnityEngine.Object)null)
				{
					position = parent.InverseTransformPoint(position);
				}
				this.absolute = Mathf.FloorToInt(localPos - position.y + 0.5f);
			}
		}

		public Vector3[] GetSides(Transform relativeTo)
		{
			if (this.target != (UnityEngine.Object)null)
			{
				if (this.rect != (UnityEngine.Object)null)
				{
					return this.rect.GetSides(relativeTo);
				}
				if (this.target.GetComponent<Camera>() != (UnityEngine.Object)null)
				{
					return this.target.GetComponent<Camera>().GetSides(relativeTo);
				}
			}
			return null;
		}

		public Transform target;

		public Single relative;

		public Int32 absolute;

		[NonSerialized]
		public UIRect rect;

		[NonSerialized]
		public Camera targetCam;
	}

	public enum AnchorUpdate
	{
		OnEnable,
		OnUpdate,
		OnStart
	}
}
