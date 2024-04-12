using Assets.Scripts.Common;
using Memoria;
using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Key Navigation")]
public class UIKeyNavigation : MonoBehaviour
{
	public static UIKeyNavigation current
	{
		get
		{
			GameObject hoveredObject = UICamera.hoveredObject;
			if (hoveredObject == null)
				return null;
			return hoveredObject.GetComponent<UIKeyNavigation>();
		}
	}

	public Boolean isColliderEnabled
	{
		get
		{
			if (!base.enabled || !base.gameObject.activeInHierarchy)
				return false;
			Collider collider = base.GetComponent<Collider>();
			if (collider != null)
				return collider.enabled;
			Collider2D collider2D = base.GetComponent<Collider2D>();
			return collider2D != null && collider2D.enabled;
		}
	}

	protected virtual void OnEnable()
	{
		UIKeyNavigation.list.Add(this);
		if (this.mStarted)
			this.Start();
	}

	private void Start()
	{
		this.mStarted = true;
		if (this.startsSelected && this.isColliderEnabled && !this.isPreventAutoStartSelect)
			UICamera.selectedObject = base.gameObject;
	}

	protected virtual void OnDisable()
	{
		UIKeyNavigation.list.Remove(this);
	}

	private static Boolean IsActive(GameObject go)
	{
		if (!go || !go.activeInHierarchy)
			return false;
		Collider collider = go.GetComponent<Collider>();
		if (collider != null)
			return collider.enabled;
		Collider2D collider2D = go.GetComponent<Collider2D>();
		return collider2D != null && collider2D.enabled;
	}

	public GameObject GetLeft(Boolean skipWrap = false)
	{
		if (UIKeyNavigation.IsActive(this.onLeft))
			return this.onLeft;
		if (this.constraint == UIKeyNavigation.Constraint.Vertical || this.constraint == UIKeyNavigation.Constraint.Explicit)
			return null;
		GameObject leftObj = this.Get(Vector3.left, 1f, 2f);
		if (leftObj != null)
			return leftObj;
		if (wrapLeftRight)
			return GetWrap(navig => navig.GetRight(true));
		return null;
	}

	public GameObject GetRight(Boolean skipWrap = false)
	{
		if (UIKeyNavigation.IsActive(this.onRight))
			return this.onRight;
		if (this.constraint == UIKeyNavigation.Constraint.Vertical || this.constraint == UIKeyNavigation.Constraint.Explicit)
			return null;
		GameObject rightObj = this.Get(Vector3.right, 1f, 2f);
		if (rightObj != null)
			return rightObj;
		if (wrapLeftRight)
			return GetWrap(navig => navig.GetLeft(true));
		return null;
	}

	public GameObject GetUp(Boolean skipWrap = false)
	{
		if (UIKeyNavigation.IsActive(this.onUp))
			return this.onUp;
		if (this.constraint == UIKeyNavigation.Constraint.Horizontal || this.constraint == UIKeyNavigation.Constraint.Explicit)
			return null;
		GameObject upObj = this.Get(Vector3.up, 2f, 1f);
		if (upObj != null)
			return upObj;
		if (wrapUpDown)
			return GetWrap(navig => navig.GetDown(true));
		return null;
	}

	public GameObject GetDown(Boolean skipWrap = false)
	{
		if (UIKeyNavigation.IsActive(this.onDown))
			return this.onDown;
		if (this.constraint == UIKeyNavigation.Constraint.Horizontal || this.constraint == UIKeyNavigation.Constraint.Explicit)
			return null;
		GameObject downObj = this.Get(Vector3.down, 2f, 1f);
		if (downObj != null)
			return downObj;
		if (!skipWrap && wrapUpDown)
			return GetWrap(navig => navig.GetUp(true));
		return null;
	}

	public GameObject Get(Vector3 direction, Single x = 1f, Single y = 1f)
	{
		Transform transform = base.transform;
		direction = transform.TransformDirection(direction);
		Vector3 center = UIKeyNavigation.GetCenter(base.gameObject);
		Single bestDistance = Single.MaxValue;
		GameObject bestObject = null;
		for (Int32 i = 0; i < UIKeyNavigation.list.size; i++)
		{
			UIKeyNavigation candidate = UIKeyNavigation.list[i];
			if (candidate != this && candidate.constraint != UIKeyNavigation.Constraint.Explicit && candidate.isColliderEnabled)
			{
				UIWidget candidateWidget = candidate.GetComponent<UIWidget>();
				if (candidateWidget == null || candidateWidget.alpha != 0f)
				{
					Vector3 candidateToThis = UIKeyNavigation.GetCenter(candidate.gameObject) - center;
					Single angleFactor = Vector3.Dot(direction, candidateToThis.normalized);
					if (angleFactor >= 0.707f) // |Angle(direction, candidateToThis)| <= 45°
					{
						candidateToThis = transform.InverseTransformDirection(candidateToThis);
						candidateToThis.x *= x;
						candidateToThis.y *= y;
						Single candidateDistance = candidateToThis.sqrMagnitude;
						if (candidateDistance <= bestDistance)
						{
							bestObject = candidate.gameObject;
							bestDistance = candidateDistance;
						}
					}
				}
			}
		}
		return bestObject;
	}

	private GameObject GetWrap(Func<UIKeyNavigation, GameObject> invertGetter)
	{
		// Todo: this doesn't seem to work well with scrollable lists for some reason
		UIKeyNavigation invertNavig = this;
		GameObject invertObj = invertGetter(this);
		while (invertObj != null && invertObj != this.gameObject)
		{
			invertNavig = invertObj.GetComponent<UIKeyNavigation>();
			if (invertNavig == null)
				break;
			invertObj = invertGetter(invertNavig);
		}
		return invertNavig != this ? invertNavig.gameObject : null;
	}

	protected static Vector3 GetCenter(GameObject go)
	{
		UIWidget widget = go.GetComponent<UIWidget>();
		UICamera camera = UICamera.FindCameraForLayer(go.layer);
		if (camera != null)
		{
			Vector3 goPos = go.transform.position;
			if (widget != null)
			{
				Vector3[] widgetCorner = widget.worldCorners;
				goPos = (widgetCorner[0] + widgetCorner[2]) * 0.5f;
			}
			goPos = camera.cachedCamera.WorldToScreenPoint(goPos);
			goPos.z = 0f;
			return goPos;
		}
		if (widget != null)
		{
			Vector3[] widgetCorner = widget.worldCorners;
			return (widgetCorner[0] + widgetCorner[2]) * 0.5f;
		}
		return go.transform.position;
	}

	public virtual void OnNavigate(KeyCode key)
	{
		if (UIPopupList.isOpen)
			return;
		UIScene sceneFromState = PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State);
		GameObject newSelection = null;
		switch (key)
		{
			case KeyCode.UpArrow:
				newSelection = this.GetUp();
				break;
			case KeyCode.DownArrow:
				newSelection = this.GetDown();
				break;
			case KeyCode.RightArrow:
				newSelection = this.GetRight();
				break;
			case KeyCode.LeftArrow:
				newSelection = this.GetLeft();
				break;
		}
		if (sceneFromState != null)
			newSelection = sceneFromState.OnKeyNavigate(key, UICamera.selectedObject, newSelection);
		if (newSelection != null)
			UICamera.selectedObject = newSelection;
	}

	public virtual void OnKey(KeyCode key)
	{
		if (key == KeyCode.Tab)
		{
			GameObject newSelection = this.onTab;
			if (newSelection == null)
			{
				if (UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift))
				{
					newSelection = this.GetLeft();
					if (newSelection == null)
						newSelection = this.GetUp();
					if (newSelection == null)
						newSelection = this.GetDown();
					if (newSelection == null)
						newSelection = this.GetRight();
				}
				else
				{
					newSelection = this.GetRight();
					if (newSelection == null)
						newSelection = this.GetDown();
					if (newSelection == null)
						newSelection = this.GetUp();
					if (newSelection == null)
						newSelection = this.GetLeft();
				}
			}
			if (newSelection != null)
				UICamera.selectedObject = newSelection;
		}
	}

	protected virtual void OnHover(Boolean isOver)
	{
		if (!NGUITools.GetActive(this))
			return;
		if (!base.enabled)
			return;
		if (PersistenSingleton<UIManager>.Instance.IsLoading)
			return;
		if (SceneDirector.IsBattleScene() ? Configuration.Control.DisableMouseInBattles : Configuration.Control.DisableMouseForMenus)
			return;
		if (isOver)
			UICamera.selectedObject = base.gameObject;
	}

	public static BetterList<UIKeyNavigation> list = new BetterList<UIKeyNavigation>();

	public UIKeyNavigation.Constraint constraint;

	public GameObject onUp;

	public GameObject onDown;

	public GameObject onLeft;

	public GameObject onRight;

	public GameObject onClick;

	public GameObject onTab;

	public Boolean startsSelected;

	public Boolean isPreventAutoStartSelect;

	[NonSerialized]
	public Boolean wrapUpDown = false;
	[NonSerialized]
	public Boolean wrapLeftRight = false;

	[NonSerialized]
	private Boolean mStarted;

	public enum Constraint
	{
		None,
		Vertical,
		Horizontal,
		Explicit
	}
}
