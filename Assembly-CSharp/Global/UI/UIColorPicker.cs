using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class UIColorPicker : MonoBehaviour
{
	private void Start()
	{
		this.mTrans = base.transform;
		this.mUITex = base.GetComponent<UITexture>();
		this.mCam = UICamera.FindCameraForLayer(base.gameObject.layer);
		this.mWidth = this.mUITex.width;
		this.mHeight = this.mUITex.height;
		Color[] array = new Color[this.mWidth * this.mHeight];
		for (Int32 i = 0; i < this.mHeight; i++)
		{
			Single y = ((Single)i - 1f) / (Single)this.mHeight;
			for (Int32 j = 0; j < this.mWidth; j++)
			{
				Single x = ((Single)j - 1f) / (Single)this.mWidth;
				Int32 num = j + i * this.mWidth;
				array[num] = UIColorPicker.Sample(x, y);
			}
		}
		this.mTex = new Texture2D(this.mWidth, this.mHeight, TextureFormat.RGB24, false);
		this.mTex.SetPixels(array);
		this.mTex.filterMode = FilterMode.Trilinear;
		this.mTex.wrapMode = TextureWrapMode.Clamp;
		this.mTex.Apply();
		this.mUITex.mainTexture = this.mTex;
		this.Select(this.value);
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(this.mTex);
		this.mTex = (Texture2D)null;
	}

	private void OnPress(Boolean pressed)
	{
		if (base.enabled && pressed && UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			this.Sample();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (base.enabled)
		{
			this.Sample();
		}
	}

	private void OnPan(Vector2 delta)
	{
		if (base.enabled)
		{
			this.mPos.x = Mathf.Clamp01(this.mPos.x + delta.x);
			this.mPos.y = Mathf.Clamp01(this.mPos.y + delta.y);
			this.Select(this.mPos);
		}
	}

	private void Sample()
	{
		Vector3 vector = UICamera.lastEventPosition;
		vector = this.mCam.cachedCamera.ScreenToWorldPoint(vector);
		vector = this.mTrans.InverseTransformPoint(vector);
		Vector3[] localCorners = this.mUITex.localCorners;
		this.mPos.x = Mathf.Clamp01((vector.x - localCorners[0].x) / (localCorners[2].x - localCorners[0].x));
		this.mPos.y = Mathf.Clamp01((vector.y - localCorners[0].y) / (localCorners[2].y - localCorners[0].y));
		if (this.selectionWidget != (UnityEngine.Object)null)
		{
			vector.x = Mathf.Lerp(localCorners[0].x, localCorners[2].x, this.mPos.x);
			vector.y = Mathf.Lerp(localCorners[0].y, localCorners[2].y, this.mPos.y);
			vector = this.mTrans.TransformPoint(vector);
			this.selectionWidget.transform.OverlayPosition(vector, this.mCam.cachedCamera);
		}
		this.value = UIColorPicker.Sample(this.mPos.x, this.mPos.y);
		UIColorPicker.current = this;
		EventDelegate.Execute(this.onChange);
		UIColorPicker.current = (UIColorPicker)null;
	}

	public void Select(Vector2 v)
	{
		v.x = Mathf.Clamp01(v.x);
		v.y = Mathf.Clamp01(v.y);
		this.mPos = v;
		if (this.selectionWidget != (UnityEngine.Object)null)
		{
			Vector3[] localCorners = this.mUITex.localCorners;
			v.x = Mathf.Lerp(localCorners[0].x, localCorners[2].x, this.mPos.x);
			v.y = Mathf.Lerp(localCorners[0].y, localCorners[2].y, this.mPos.y);
			v = this.mTrans.TransformPoint(v);
			this.selectionWidget.transform.OverlayPosition(v, this.mCam.cachedCamera);
		}
		this.value = UIColorPicker.Sample(this.mPos.x, this.mPos.y);
		UIColorPicker.current = this;
		EventDelegate.Execute(this.onChange);
		UIColorPicker.current = (UIColorPicker)null;
	}

	public Vector2 Select(Color c)
	{
		if (this.mUITex == (UnityEngine.Object)null)
		{
			this.value = c;
			return this.mPos;
		}
		Single num = Single.MaxValue;
		for (Int32 i = 0; i < this.mHeight; i++)
		{
			Single y = ((Single)i - 1f) / (Single)this.mHeight;
			for (Int32 j = 0; j < this.mWidth; j++)
			{
				Single x = ((Single)j - 1f) / (Single)this.mWidth;
				Color color = UIColorPicker.Sample(x, y);
				Color color2 = color;
				color2.r -= c.r;
				color2.g -= c.g;
				color2.b -= c.b;
				Single num2 = color2.r * color2.r + color2.g * color2.g + color2.b * color2.b;
				if (num2 < num)
				{
					num = num2;
					this.mPos.x = x;
					this.mPos.y = y;
				}
			}
		}
		if (this.selectionWidget != (UnityEngine.Object)null)
		{
			Vector3[] localCorners = this.mUITex.localCorners;
			Vector3 vector;
			vector.x = Mathf.Lerp(localCorners[0].x, localCorners[2].x, this.mPos.x);
			vector.y = Mathf.Lerp(localCorners[0].y, localCorners[2].y, this.mPos.y);
			vector.z = 0f;
			vector = this.mTrans.TransformPoint(vector);
			this.selectionWidget.transform.OverlayPosition(vector, this.mCam.cachedCamera);
		}
		this.value = c;
		UIColorPicker.current = this;
		EventDelegate.Execute(this.onChange);
		UIColorPicker.current = (UIColorPicker)null;
		return this.mPos;
	}

	public static Color Sample(Single x, Single y)
	{
		if (UIColorPicker.mRed == null)
		{
			UIColorPicker.mRed = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 1f),
				new Keyframe(0.142857149f, 1f),
				new Keyframe(0.2857143f, 0f),
				new Keyframe(0.428571433f, 0f),
				new Keyframe(0.5714286f, 0f),
				new Keyframe(0.714285731f, 1f),
				new Keyframe(0.857142866f, 1f),
				new Keyframe(1f, 0.5f)
			});
			UIColorPicker.mGreen = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.142857149f, 1f),
				new Keyframe(0.2857143f, 1f),
				new Keyframe(0.428571433f, 1f),
				new Keyframe(0.5714286f, 0f),
				new Keyframe(0.714285731f, 0f),
				new Keyframe(0.857142866f, 0f),
				new Keyframe(1f, 0.5f)
			});
			UIColorPicker.mBlue = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.142857149f, 0f),
				new Keyframe(0.2857143f, 0f),
				new Keyframe(0.428571433f, 1f),
				new Keyframe(0.5714286f, 1f),
				new Keyframe(0.714285731f, 1f),
				new Keyframe(0.857142866f, 0f),
				new Keyframe(1f, 0.5f)
			});
		}
		Vector3 a = new Vector3(UIColorPicker.mRed.Evaluate(x), UIColorPicker.mGreen.Evaluate(x), UIColorPicker.mBlue.Evaluate(x));
		if (y < 0.5f)
		{
			y *= 2f;
			a.x *= y;
			a.y *= y;
			a.z *= y;
		}
		else
		{
			a = Vector3.Lerp(a, Vector3.one, y * 2f - 1f);
		}
		return new Color(a.x, a.y, a.z, 1f);
	}

	public static UIColorPicker current;

	public Color value = Color.white;

	public UIWidget selectionWidget;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	[NonSerialized]
	private Transform mTrans;

	[NonSerialized]
	private UITexture mUITex;

	[NonSerialized]
	private Texture2D mTex;

	[NonSerialized]
	private UICamera mCam;

	[NonSerialized]
	private Vector2 mPos;

	[NonSerialized]
	private Int32 mWidth;

	[NonSerialized]
	private Int32 mHeight;

	private static AnimationCurve mRed;

	private static AnimationCurve mGreen;

	private static AnimationCurve mBlue;
}
