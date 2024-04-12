using Assets.Scripts.Common;
using Memoria;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Event System (UICamera)")]
public class UICamera : MonoBehaviour
{
	[Obsolete("Use new OnDragStart / OnDragOver / OnDragOut / OnDragEnd events instead")]
	public Boolean stickyPress
	{
		get
		{
			return true;
		}
	}

	public static Boolean disableController
	{
		get
		{
			return UICamera.mDisableController && UIPopupList.current == (UnityEngine.Object)null;
		}
		set
		{
			UICamera.mDisableController = value;
		}
	}

	[Obsolete("Use lastEventPosition instead. It handles controller input properly.")]
	public static Vector2 lastTouchPosition
	{
		get
		{
			return UICamera.mLastPos;
		}
		set
		{
			UICamera.mLastPos = value;
		}
	}

	public static Vector2 lastEventPosition
	{
		get
		{
			UICamera.ControlScheme currentScheme = UICamera.currentScheme;
			if (currentScheme == UICamera.ControlScheme.Controller)
			{
				GameObject hoveredObject = UICamera.hoveredObject;
				if (hoveredObject != (UnityEngine.Object)null)
				{
					Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(hoveredObject.transform);
					Camera camera = NGUITools.FindCameraForLayer(hoveredObject.layer);
					return camera.WorldToScreenPoint(bounds.center);
				}
			}
			return UICamera.mLastPos;
		}
		set
		{
			UICamera.mLastPos = value;
		}
	}

	public static UICamera.ControlScheme currentScheme
	{
		get
		{
			if (UICamera.mCurrentKey == KeyCode.None)
			{
				return UICamera.ControlScheme.Touch;
			}
			if (UICamera.mCurrentKey >= KeyCode.JoystickButton0)
			{
				return UICamera.ControlScheme.Controller;
			}
			return UICamera.ControlScheme.Mouse;
		}
		set
		{
			if (value == UICamera.ControlScheme.Mouse)
			{
				UICamera.currentKey = KeyCode.Mouse0;
			}
			else if (value == UICamera.ControlScheme.Controller)
			{
				UICamera.currentKey = KeyCode.JoystickButton0;
			}
			else if (value == UICamera.ControlScheme.Touch)
			{
				UICamera.currentKey = KeyCode.None;
			}
			else
			{
				UICamera.currentKey = KeyCode.Alpha0;
			}
		}
	}

	public static KeyCode currentKey
	{
		get
		{
			return UICamera.mCurrentKey;
		}
		set
		{
			if (UICamera.mCurrentKey != value)
			{
				UICamera.ControlScheme currentScheme = UICamera.currentScheme;
				UICamera.mCurrentKey = value;
				UICamera.ControlScheme currentScheme2 = UICamera.currentScheme;
				if (currentScheme != currentScheme2)
				{
					UICamera.HideTooltip();
					if (currentScheme2 == UICamera.ControlScheme.Mouse)
					{
						Cursor.lockState = CursorLockMode.None;
						Cursor.visible = true;
					}
					else
					{
						Cursor.visible = false;
						Cursor.lockState = CursorLockMode.None;
						UICamera.mMouse[0].ignoreDelta = 2;
					}
					if (UICamera.onSchemeChange != null)
					{
						UICamera.onSchemeChange();
					}
				}
			}
		}
	}

	public static Ray currentRay
	{
		get
		{
			return (!(UICamera.currentCamera != (UnityEngine.Object)null) || UICamera.currentTouch == null) ? default(Ray) : UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
		}
	}

	public static Boolean inputHasFocus
	{
		get
		{
			if (UICamera.mInputFocus)
			{
				if (UICamera.mSelected && UICamera.mSelected.activeInHierarchy)
				{
					return true;
				}
				UICamera.mInputFocus = false;
			}
			return false;
		}
	}

	[Obsolete("Use delegates instead such as UICamera.onClick, UICamera.onHover, etc.")]
	public static GameObject genericEventHandler
	{
		get
		{
			return UICamera.mGenericHandler;
		}
		set
		{
			UICamera.mGenericHandler = value;
		}
	}

	private Boolean handlesEvents
	{
		get
		{
			return UICamera.eventHandler == this;
		}
	}

	public Camera cachedCamera
	{
		get
		{
			if (this.mCam == (UnityEngine.Object)null)
			{
				this.mCam = base.GetComponent<Camera>();
			}
			return this.mCam;
		}
	}

	public static GameObject tooltipObject
	{
		get
		{
			return UICamera.mTooltip;
		}
	}

	public static Boolean isOverUI
	{
		get
		{
			if (UICamera.currentTouch != null)
			{
				return UICamera.currentTouch.isOverUI;
			}
			return !(UICamera.mHover == (UnityEngine.Object)null) && !(UICamera.mHover == UICamera.fallThrough) && NGUITools.FindInParents<UIRoot>(UICamera.mHover) != (UnityEngine.Object)null;
		}
	}

	public static GameObject hoveredObject
	{
		get
		{
			if (UICamera.currentTouch != null && UICamera.currentTouch.dragStarted)
			{
				return UICamera.currentTouch.current;
			}
			if (UICamera.mHover && UICamera.mHover.activeInHierarchy)
			{
				return UICamera.mHover;
			}
			UICamera.mHover = (GameObject)null;
			return (GameObject)null;
		}
		set
		{
			if (UICamera.mHover == value)
			{
				return;
			}
			Boolean flag = false;
			UICamera uicamera = UICamera.current;
			if (UICamera.currentTouch == null)
			{
				flag = true;
				UICamera.currentTouchID = -100;
				UICamera.currentTouch = UICamera.controller;
			}
			UICamera.ShowTooltip((GameObject)null);
			if (UICamera.mSelected && UICamera.currentScheme == UICamera.ControlScheme.Controller)
			{
				UICamera.Notify(UICamera.mSelected, "OnSelect", false);
				if (UICamera.onSelect != null)
				{
					UICamera.onSelect(UICamera.mSelected, false);
				}
				UICamera.mSelected = (GameObject)null;
			}
			if (UICamera.mHover)
			{
				UICamera.Notify(UICamera.mHover, "OnHover", false);
				if (UICamera.onHover != null)
				{
					UICamera.onHover(UICamera.mHover, false);
				}
			}
			UICamera.mHover = value;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
			if (UICamera.mHover)
			{
				if (UICamera.mHover != UICamera.controller.current && UICamera.mHover.GetComponent<UIKeyNavigation>() != (UnityEngine.Object)null)
				{
					UICamera.controller.current = UICamera.mHover;
				}
				if (flag)
				{
					UICamera uicamera2 = (!(UICamera.mHover != (UnityEngine.Object)null)) ? UICamera.list[0] : UICamera.FindCameraForLayer(UICamera.mHover.layer);
					if (uicamera2 != (UnityEngine.Object)null)
					{
						UICamera.current = uicamera2;
						UICamera.currentCamera = uicamera2.cachedCamera;
					}
				}
				if (UICamera.onHover != null)
				{
					UICamera.onHover(UICamera.mHover, true);
				}
				UICamera.Notify(UICamera.mHover, "OnHover", true);
			}
			if (flag)
			{
				UICamera.current = uicamera;
				UICamera.currentCamera = ((!(uicamera != (UnityEngine.Object)null)) ? null : uicamera.cachedCamera);
				UICamera.currentTouch = (UICamera.MouseOrTouch)null;
				UICamera.currentTouchID = -100;
			}
		}
	}

	public static GameObject controllerNavigationObject
	{
		get
		{
			if (UICamera.controller.current && UICamera.controller.current.activeInHierarchy)
			{
				return UICamera.controller.current;
			}
			if (UICamera.currentScheme == UICamera.ControlScheme.Controller && UICamera.current != (UnityEngine.Object)null && UICamera.current.useController && UIKeyNavigation.list.size > 0)
			{
				for (Int32 i = 0; i < UIKeyNavigation.list.size; i++)
				{
					UIKeyNavigation uikeyNavigation = UIKeyNavigation.list[i];
					if (uikeyNavigation && uikeyNavigation.constraint != UIKeyNavigation.Constraint.Explicit && uikeyNavigation.startsSelected)
					{
						UICamera.hoveredObject = uikeyNavigation.gameObject;
						UICamera.controller.current = UICamera.mHover;
						return UICamera.mHover;
					}
				}
				if (UICamera.mHover == (UnityEngine.Object)null)
				{
					for (Int32 j = 0; j < UIKeyNavigation.list.size; j++)
					{
						UIKeyNavigation uikeyNavigation2 = UIKeyNavigation.list[j];
						if (uikeyNavigation2 && uikeyNavigation2.constraint != UIKeyNavigation.Constraint.Explicit)
						{
							UICamera.hoveredObject = uikeyNavigation2.gameObject;
							UICamera.controller.current = UICamera.mHover;
							return UICamera.mHover;
						}
					}
				}
			}
			UICamera.controller.current = (GameObject)null;
			return (GameObject)null;
		}
		set
		{
			if (UICamera.controller.current != value && UICamera.controller.current)
			{
				UICamera.Notify(UICamera.controller.current, "OnHover", false);
				if (UICamera.onHover != null)
				{
					UICamera.onHover(UICamera.controller.current, false);
				}
				UICamera.controller.current = (GameObject)null;
			}
			UICamera.hoveredObject = value;
		}
	}

	public static GameObject selectedObject
	{
		get
		{
			if (UICamera.mSelected && UICamera.mSelected.activeInHierarchy)
			{
				return UICamera.mSelected;
			}
			UICamera.mSelected = (GameObject)null;
			return (GameObject)null;
		}
		set
		{
			if (UICamera.mSelected == value)
			{
				UICamera.hoveredObject = value;
				UICamera.controller.current = value;
				return;
			}
			UICamera.ShowTooltip((GameObject)null);
			Boolean flag = false;
			UICamera uicamera = UICamera.current;
			if (UICamera.currentTouch == null)
			{
				flag = true;
				UICamera.currentTouchID = -100;
				UICamera.currentTouch = UICamera.controller;
			}
			UICamera.mInputFocus = false;
			if (UICamera.mSelected)
			{
				UICamera.Notify(UICamera.mSelected, "OnSelect", false);
				if (UICamera.onSelect != null)
				{
					UICamera.onSelect(UICamera.mSelected, false);
				}
			}
			UICamera.mSelected = value;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
			if (value != (UnityEngine.Object)null)
			{
				UIKeyNavigation component = value.GetComponent<UIKeyNavigation>();
				if (component != (UnityEngine.Object)null)
				{
					UICamera.controller.current = value;
				}
			}
			if (UICamera.mSelected && flag)
			{
				UICamera uicamera2 = (!(UICamera.mSelected != (UnityEngine.Object)null)) ? UICamera.list[0] : UICamera.FindCameraForLayer(UICamera.mSelected.layer);
				if (uicamera2 != (UnityEngine.Object)null)
				{
					UICamera.current = uicamera2;
					UICamera.currentCamera = uicamera2.cachedCamera;
				}
			}
			if (UICamera.mSelected)
			{
				UICamera.mInputFocus = (UICamera.mSelected.activeInHierarchy && UICamera.mSelected.GetComponent<UIInput>() != (UnityEngine.Object)null);
				if (UICamera.onSelect != null)
				{
					UICamera.onSelect(UICamera.mSelected, true);
				}
				UICamera.Notify(UICamera.mSelected, "OnSelect", true);
			}
			if (flag)
			{
				UICamera.current = uicamera;
				UICamera.currentCamera = ((!(uicamera != (UnityEngine.Object)null)) ? null : uicamera.cachedCamera);
				UICamera.currentTouch = (UICamera.MouseOrTouch)null;
				UICamera.currentTouchID = -100;
			}
		}
	}

	public static Boolean IsPressed(GameObject go)
	{
		for (Int32 i = 0; i < 3; i++)
		{
			if (UICamera.mMouse[i].pressed == go)
			{
				return true;
			}
		}
		Int32 j = 0;
		Int32 count = UICamera.activeTouches.Count;
		while (j < count)
		{
			UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[j];
			if (mouseOrTouch.pressed == go)
			{
				return true;
			}
			j++;
		}
		return UICamera.controller.pressed == go;
	}

	[Obsolete("Use either 'CountInputSources()' or 'activeTouches.Count'")]
	public static Int32 touchCount
	{
		get
		{
			return UICamera.CountInputSources();
		}
	}

	public static Int32 CountInputSources()
	{
		Int32 num = 0;
		Int32 i = 0;
		Int32 count = UICamera.activeTouches.Count;
		while (i < count)
		{
			UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
			if (mouseOrTouch.pressed != (UnityEngine.Object)null)
			{
				num++;
			}
			i++;
		}
		for (Int32 j = 0; j < (Int32)UICamera.mMouse.Length; j++)
		{
			if (UICamera.mMouse[j].pressed != (UnityEngine.Object)null)
			{
				num++;
			}
		}
		if (UICamera.controller.pressed != (UnityEngine.Object)null)
		{
			num++;
		}
		return num;
	}

	public static Int32 dragCount
	{
		get
		{
			Int32 num = 0;
			Int32 i = 0;
			Int32 count = UICamera.activeTouches.Count;
			while (i < count)
			{
				UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
				if (mouseOrTouch.dragged != (UnityEngine.Object)null)
				{
					num++;
				}
				i++;
			}
			for (Int32 j = 0; j < (Int32)UICamera.mMouse.Length; j++)
			{
				if (UICamera.mMouse[j].dragged != (UnityEngine.Object)null)
				{
					num++;
				}
			}
			if (UICamera.controller.dragged != (UnityEngine.Object)null)
			{
				num++;
			}
			return num;
		}
	}

	public static Camera mainCamera
	{
		get
		{
			UICamera eventHandler = UICamera.eventHandler;
			return (!(eventHandler != (UnityEngine.Object)null)) ? null : eventHandler.cachedCamera;
		}
	}

	public static UICamera eventHandler
	{
		get
		{
			for (Int32 i = 0; i < UICamera.list.size; i++)
			{
				UICamera uicamera = UICamera.list.buffer[i];
				if (!(uicamera == (UnityEngine.Object)null) && uicamera.enabled && NGUITools.GetActive(uicamera.gameObject))
				{
					return uicamera;
				}
			}
			return (UICamera)null;
		}
	}

	private static Int32 CompareFunc(UICamera a, UICamera b)
	{
		if (a.cachedCamera.depth < b.cachedCamera.depth)
		{
			return 1;
		}
		if (a.cachedCamera.depth > b.cachedCamera.depth)
		{
			return -1;
		}
		return 0;
	}

	private static Rigidbody FindRootRigidbody(Transform trans)
	{
		while (trans != (UnityEngine.Object)null)
		{
			if (trans.GetComponent<UIPanel>() != (UnityEngine.Object)null)
			{
				return (Rigidbody)null;
			}
			Rigidbody component = trans.GetComponent<Rigidbody>();
			if (component != (UnityEngine.Object)null)
			{
				return component;
			}
			trans = trans.parent;
		}
		return (Rigidbody)null;
	}

	private static Rigidbody2D FindRootRigidbody2D(Transform trans)
	{
		while (trans != (UnityEngine.Object)null)
		{
			if (trans.GetComponent<UIPanel>() != (UnityEngine.Object)null)
			{
				return (Rigidbody2D)null;
			}
			Rigidbody2D component = trans.GetComponent<Rigidbody2D>();
			if (component != (UnityEngine.Object)null)
			{
				return component;
			}
			trans = trans.parent;
		}
		return (Rigidbody2D)null;
	}

	public static void Raycast(UICamera.MouseOrTouch touch)
	{
		if (!UICamera.Raycast(touch.pos))
		{
			UICamera.mRayHitObject = UICamera.fallThrough;
		}
		if (UICamera.mRayHitObject == (UnityEngine.Object)null)
		{
			UICamera.mRayHitObject = UICamera.mGenericHandler;
		}
		touch.last = touch.current;
		touch.current = UICamera.mRayHitObject;
		UICamera.mLastPos = touch.pos;
	}

	public static Boolean Raycast(Vector3 inPos)
	{
		for (Int32 i = 0; i < UICamera.list.size; i++)
		{
			UICamera uicamera = UICamera.list.buffer[i];
			if (uicamera.enabled && NGUITools.GetActive(uicamera.gameObject))
			{
				UICamera.currentCamera = uicamera.cachedCamera;
				Vector3 vector = UICamera.currentCamera.ScreenToViewportPoint(inPos);
				if (!Single.IsNaN(vector.x) && !Single.IsNaN(vector.y))
				{
					if (vector.x >= 0f && vector.x <= 1f && vector.y >= 0f && vector.y <= 1f)
					{
						Ray ray = UICamera.currentCamera.ScreenPointToRay(inPos);
						Int32 layerMask = UICamera.currentCamera.cullingMask & uicamera.eventReceiverMask;
						Single num = (uicamera.rangeDistance <= 0f) ? (UICamera.currentCamera.farClipPlane - UICamera.currentCamera.nearClipPlane) : uicamera.rangeDistance;
						if (uicamera.eventType == UICamera.EventType.World_3D)
						{
							if (Physics.Raycast(ray, out UICamera.lastHit, num, layerMask))
							{
								UICamera.lastWorldPosition = UICamera.lastHit.point;
								UICamera.mRayHitObject = UICamera.lastHit.collider.gameObject;
								if (!UICamera.list[0].eventsGoToColliders)
								{
									Rigidbody rigidbody = UICamera.FindRootRigidbody(UICamera.mRayHitObject.transform);
									if (rigidbody != (UnityEngine.Object)null)
									{
										UICamera.mRayHitObject = rigidbody.gameObject;
									}
								}
								return true;
							}
						}
						else if (uicamera.eventType == UICamera.EventType.UI_3D)
						{
							RaycastHit[] array = Physics.RaycastAll(ray, num, layerMask);
							if ((Int32)array.Length > 1)
							{
								Int32 j = 0;
								while (j < (Int32)array.Length)
								{
									GameObject gameObject = array[j].collider.gameObject;
									UIWidget component = gameObject.GetComponent<UIWidget>();
									if (component != (UnityEngine.Object)null)
									{
										if (component.isVisible)
										{
											if (component.hitCheck == null || component.hitCheck(array[j].point))
											{
												goto IL_260;
											}
										}
									}
									else
									{
										UIRect uirect = NGUITools.FindInParents<UIRect>(gameObject);
										if (!(uirect != (UnityEngine.Object)null) || uirect.finalAlpha >= 0.001f)
										{
											goto IL_260;
										}
									}
								IL_2E1:
									j++;
									continue;
								IL_260:
									UICamera.mHit.depth = NGUITools.CalculateRaycastDepth(gameObject);
									if (UICamera.mHit.depth != 2147483647)
									{
										UICamera.mHit.hit = array[j];
										UICamera.mHit.point = array[j].point;
										UICamera.mHit.go = array[j].collider.gameObject;
										UICamera.mHits.Add(UICamera.mHit);
										goto IL_2E1;
									}
									goto IL_2E1;
								}
								UICamera.mHits.Sort((UICamera.DepthEntry r1, UICamera.DepthEntry r2) => r2.depth.CompareTo(r1.depth));
								for (Int32 k = 0; k < UICamera.mHits.size; k++)
								{
									if (UICamera.IsVisible(ref UICamera.mHits.buffer[k]))
									{
										UICamera.lastHit = UICamera.mHits[k].hit;
										UICamera.mRayHitObject = UICamera.mHits[k].go;
										UICamera.lastWorldPosition = UICamera.mHits[k].point;
										UICamera.mHits.Clear();
										return true;
									}
								}
								UICamera.mHits.Clear();
							}
							else if ((Int32)array.Length == 1)
							{
								GameObject gameObject2 = array[0].collider.gameObject;
								UIWidget component2 = gameObject2.GetComponent<UIWidget>();
								if (component2 != (UnityEngine.Object)null)
								{
									if (!component2.isVisible)
									{
										goto IL_7E2;
									}
									if (component2.hitCheck != null && !component2.hitCheck(array[0].point))
									{
										goto IL_7E2;
									}
								}
								else
								{
									UIRect uirect2 = NGUITools.FindInParents<UIRect>(gameObject2);
									if (uirect2 != (UnityEngine.Object)null && uirect2.finalAlpha < 0.001f)
									{
										goto IL_7E2;
									}
								}
								if (UICamera.IsVisible(array[0].point, array[0].collider.gameObject))
								{
									UICamera.lastHit = array[0];
									UICamera.lastWorldPosition = array[0].point;
									UICamera.mRayHitObject = UICamera.lastHit.collider.gameObject;
									return true;
								}
							}
						}
						else if (uicamera.eventType == UICamera.EventType.World_2D)
						{
							if (UICamera.m2DPlane.Raycast(ray, out num))
							{
								Vector3 point = ray.GetPoint(num);
								Collider2D collider2D = Physics2D.OverlapPoint(point, layerMask);
								if (collider2D)
								{
									UICamera.lastWorldPosition = point;
									UICamera.mRayHitObject = collider2D.gameObject;
									if (!uicamera.eventsGoToColliders)
									{
										Rigidbody2D rigidbody2D = UICamera.FindRootRigidbody2D(UICamera.mRayHitObject.transform);
										if (rigidbody2D != (UnityEngine.Object)null)
										{
											UICamera.mRayHitObject = rigidbody2D.gameObject;
										}
									}
									return true;
								}
							}
						}
						else if (uicamera.eventType == UICamera.EventType.UI_2D)
						{
							if (UICamera.m2DPlane.Raycast(ray, out num))
							{
								UICamera.lastWorldPosition = ray.GetPoint(num);
								Collider2D[] array2 = Physics2D.OverlapPointAll(UICamera.lastWorldPosition, layerMask);
								if ((Int32)array2.Length > 1)
								{
									Int32 l = 0;
									while (l < (Int32)array2.Length)
									{
										GameObject gameObject3 = array2[l].gameObject;
										UIWidget component3 = gameObject3.GetComponent<UIWidget>();
										if (component3 != (UnityEngine.Object)null)
										{
											if (component3.isVisible)
											{
												if (component3.hitCheck == null || component3.hitCheck(UICamera.lastWorldPosition))
												{
													goto IL_639;
												}
											}
										}
										else
										{
											UIRect uirect3 = NGUITools.FindInParents<UIRect>(gameObject3);
											if (!(uirect3 != (UnityEngine.Object)null) || uirect3.finalAlpha >= 0.001f)
											{
												goto IL_639;
											}
										}
									IL_688:
										l++;
										continue;
									IL_639:
										UICamera.mHit.depth = NGUITools.CalculateRaycastDepth(gameObject3);
										if (UICamera.mHit.depth != 2147483647)
										{
											UICamera.mHit.go = gameObject3;
											UICamera.mHit.point = UICamera.lastWorldPosition;
											UICamera.mHits.Add(UICamera.mHit);
											goto IL_688;
										}
										goto IL_688;
									}
									UICamera.mHits.Sort((UICamera.DepthEntry r1, UICamera.DepthEntry r2) => r2.depth.CompareTo(r1.depth));
									for (Int32 m = 0; m < UICamera.mHits.size; m++)
									{
										if (UICamera.IsVisible(ref UICamera.mHits.buffer[m]))
										{
											UICamera.mRayHitObject = UICamera.mHits[m].go;
											UICamera.mHits.Clear();
											return true;
										}
									}
									UICamera.mHits.Clear();
								}
								else if ((Int32)array2.Length == 1)
								{
									GameObject gameObject4 = array2[0].gameObject;
									UIWidget component4 = gameObject4.GetComponent<UIWidget>();
									if (component4 != (UnityEngine.Object)null)
									{
										if (!component4.isVisible)
										{
											goto IL_7E2;
										}
										if (component4.hitCheck != null && !component4.hitCheck(UICamera.lastWorldPosition))
										{
											goto IL_7E2;
										}
									}
									else
									{
										UIRect uirect4 = NGUITools.FindInParents<UIRect>(gameObject4);
										if (uirect4 != (UnityEngine.Object)null && uirect4.finalAlpha < 0.001f)
										{
											goto IL_7E2;
										}
									}
									if (UICamera.IsVisible(UICamera.lastWorldPosition, gameObject4))
									{
										UICamera.mRayHitObject = gameObject4;
										return true;
									}
								}
							}
						}
					}
				}
			}
		IL_7E2:;
		}
		return false;
	}

	private static Boolean IsVisible(Vector3 worldPoint, GameObject go)
	{
		UIPanel uipanel = NGUITools.FindInParents<UIPanel>(go);
		while (uipanel != (UnityEngine.Object)null)
		{
			if (!uipanel.IsVisible(worldPoint))
			{
				return false;
			}
			uipanel = uipanel.parentPanel;
		}
		return true;
	}

	private static Boolean IsVisible(ref UICamera.DepthEntry de)
	{
		UIPanel uipanel = NGUITools.FindInParents<UIPanel>(de.go);
		while (uipanel != (UnityEngine.Object)null)
		{
			if (!uipanel.IsVisible(de.point))
			{
				return false;
			}
			uipanel = uipanel.parentPanel;
		}
		return true;
	}

	public static Boolean IsHighlighted(GameObject go)
	{
		return UICamera.hoveredObject == go;
	}

	public static UICamera FindCameraForLayer(Int32 layer)
	{
		Int32 num = 1 << layer;
		for (Int32 i = 0; i < UICamera.list.size; i++)
		{
			UICamera uicamera = UICamera.list.buffer[i];
			Camera cachedCamera = uicamera.cachedCamera;
			if (cachedCamera != (UnityEngine.Object)null && (cachedCamera.cullingMask & num) != 0)
			{
				return uicamera;
			}
		}
		return (UICamera)null;
	}

	private static Int32 GetDirection(KeyCode up, KeyCode down)
	{
		if (UICamera.GetKeyDown(up))
		{
			UICamera.currentKey = up;
			return 1;
		}
		if (UICamera.GetKeyDown(down))
		{
			UICamera.currentKey = down;
			return -1;
		}
		return 0;
	}

	private static Int32 GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
	{
		if (UICamera.GetKeyDown(up0))
		{
			UICamera.currentKey = up0;
			return 1;
		}
		if (UICamera.GetKeyDown(up1))
		{
			UICamera.currentKey = up1;
			return 1;
		}
		if (UICamera.GetKeyDown(down0))
		{
			UICamera.currentKey = down0;
			return -1;
		}
		if (UICamera.GetKeyDown(down1))
		{
			UICamera.currentKey = down1;
			return -1;
		}
		return 0;
	}

	public static Single EventWaitTime
	{
		set
		{
			UICamera.mEventWaitTime = value;
		}
	}

	private static Int32 GetDirection(String axisList)
	{
		Single time = RealTime.time;
		if (UICamera.mNextEvent < time && !String.IsNullOrEmpty(axisList))
		{
			String[] array = axisList.Split(new Char[]
			{
				','
			});
			String[] array2 = array;
			for (Int32 i = 0; i < (Int32)array2.Length; i++)
			{
				String axisName = array2[i];
				Single axisRaw = UnityXInput.Input.GetAxisRaw(axisName);
				if (axisRaw > 0.75f)
				{
					UICamera.currentKey = KeyCode.JoystickButton0;
					UICamera.mNextEvent = time + UICamera.mEventWaitTime;
					UICamera.mEventWaitTime = UICamera.mDefaultEventWaitTime;
					return 1;
				}
				if (axisRaw < -0.75f)
				{
					UICamera.currentKey = KeyCode.JoystickButton0;
					UICamera.mNextEvent = time + UICamera.mEventWaitTime;
					UICamera.mEventWaitTime = UICamera.mDefaultEventWaitTime;
					return -1;
				}
			}
		}
		return 0;
	}

	public static void Notify(GameObject go, String funcName, Object obj)
	{
		if (UICamera.mNotifying > 10)
		{
			return;
		}
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller && UIPopupList.isOpen && UIPopupList.current.source == go && UIPopupList.isOpen)
		{
			go = UIPopupList.current.gameObject;
		}
		if (go && go.activeInHierarchy)
		{
			UICamera.mNotifying++;
			go.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			if (UICamera.mGenericHandler != (UnityEngine.Object)null && UICamera.mGenericHandler != go)
			{
				UICamera.mGenericHandler.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			}
			UICamera.mNotifying--;
		}
	}

	public static UICamera.MouseOrTouch GetMouse(Int32 button)
	{
		return UICamera.mMouse[button];
	}

	public static UICamera.MouseOrTouch GetTouch(Int32 id, Boolean createIfMissing = false)
	{
		if (id < 0)
		{
			return UICamera.GetMouse(-id - 1);
		}
		Int32 i = 0;
		Int32 count = UICamera.mTouchIDs.Count;
		while (i < count)
		{
			if (UICamera.mTouchIDs[i] == id)
			{
				return UICamera.activeTouches[i];
			}
			i++;
		}
		if (createIfMissing)
		{
			UICamera.MouseOrTouch mouseOrTouch = new UICamera.MouseOrTouch();
			mouseOrTouch.pressTime = RealTime.time;
			mouseOrTouch.touchBegan = true;
			UICamera.activeTouches.Add(mouseOrTouch);
			UICamera.mTouchIDs.Add(id);
			return mouseOrTouch;
		}
		return (UICamera.MouseOrTouch)null;
	}

	public static void RemoveTouch(Int32 id)
	{
		Int32 i = 0;
		Int32 count = UICamera.mTouchIDs.Count;
		while (i < count)
		{
			if (UICamera.mTouchIDs[i] == id)
			{
				UICamera.mTouchIDs.RemoveAt(i);
				UICamera.activeTouches.RemoveAt(i);
				return;
			}
			i++;
		}
	}

	private void Awake()
	{
		UICamera.mWidth = Screen.width;
		UICamera.mHeight = Screen.height;
		UICamera.mMouse[0].pos = UnityXInput.Input.mousePosition;
		for (Int32 i = 1; i < 3; i++)
		{
			UICamera.mMouse[i].pos = UICamera.mMouse[0].pos;
			UICamera.mMouse[i].lastPos = UICamera.mMouse[0].pos;
		}
		UICamera.mLastPos = UICamera.mMouse[0].pos;
	}

	private void OnEnable()
	{
		UICamera.list.Add(this);
		UICamera.list.Sort(new BetterList<UICamera>.CompareFunc(UICamera.CompareFunc));
	}

	private void OnDisable()
	{
		UICamera.list.Remove(this);
	}

	private void Start()
	{
		if (this.eventType != UICamera.EventType.World_3D && this.cachedCamera.transparencySortMode != TransparencySortMode.Orthographic)
		{
			this.cachedCamera.transparencySortMode = TransparencySortMode.Orthographic;
		}
		if (Application.isPlaying)
		{
			if (UICamera.fallThrough == (UnityEngine.Object)null)
			{
				UIRoot uiroot = NGUITools.FindInParents<UIRoot>(base.gameObject);
				if (uiroot != (UnityEngine.Object)null)
				{
					UICamera.fallThrough = uiroot.gameObject;
				}
				else
				{
					Transform transform = base.transform;
					UICamera.fallThrough = ((!(transform.parent != (UnityEngine.Object)null)) ? base.gameObject : transform.parent.gameObject);
				}
			}
			this.cachedCamera.eventMask = 0;
		}
	}

	private void Update()
	{
		if (!this.handlesEvents)
		{
			return;
		}
		UICamera.current = this;
		NGUIDebug.debugRaycast = this.debug;
		if (this.useTouch)
		{
			this.ProcessTouches();
		}
		else if (this.useMouse)
		{
			this.ProcessMouse();
		}
		if (UICamera.onCustomInput != null)
		{
			UICamera.onCustomInput();
		}
		if ((this.useKeyboard || this.useController) && !UICamera.disableController)
		{
			this.ProcessOthers();
		}
		if (this.useMouse && UICamera.mHover != (UnityEngine.Object)null)
		{
			Single num = String.IsNullOrEmpty(this.scrollAxisName) ? 0f : UICamera.GetAxis(this.scrollAxisName);
			if (num != 0f)
			{
				if (UICamera.onScroll != null)
				{
					UICamera.onScroll(UICamera.mHover, num);
				}
				UICamera.Notify(UICamera.mHover, "OnScroll", num);
			}
			if (UICamera.showTooltips && UICamera.mTooltipTime != 0f && !UIPopupList.isOpen && (UICamera.mTooltipTime < RealTime.time || UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift)))
			{
				UICamera.currentTouch = UICamera.mMouse[0];
				UICamera.currentTouchID = -1;
				UICamera.ShowTooltip(UICamera.mHover);
			}
		}
		if (UICamera.mTooltip != (UnityEngine.Object)null && !NGUITools.GetActive(UICamera.mTooltip))
		{
			UICamera.ShowTooltip((GameObject)null);
		}
		UICamera.current = (UICamera)null;
		UICamera.currentTouchID = -100;
	}

	private void LateUpdate()
	{
		if (!this.handlesEvents)
		{
			return;
		}
		Int32 width = Screen.width;
		Int32 height = Screen.height;
		if (width != UICamera.mWidth || height != UICamera.mHeight)
		{
			UICamera.mWidth = width;
			UICamera.mHeight = height;
			UIRoot.Broadcast("UpdateAnchors");
			if (UICamera.onScreenResize != null)
			{
				UICamera.onScreenResize();
			}
		}
	}

	public void ProcessMouse()
	{
		if (SceneDirector.IsBattleScene() ? Configuration.Control.DisableMouseInBattles : Configuration.Control.DisableMouseForMenus)
			return;
		Boolean isClicking = false;
		Boolean isClickingNow = false;
		for (Int32 i = 0; i < 3; i++)
		{
			if (UnityXInput.Input.GetMouseButtonDown(i))
			{
				UICamera.currentKey = KeyCode.Mouse0 + i;
				isClickingNow = true;
				isClicking = true;
			}
			else if (UnityXInput.Input.GetMouseButton(i))
			{
				UICamera.currentKey = KeyCode.Mouse0 + i;
				isClicking = true;
			}
		}
		if (UICamera.currentScheme == UICamera.ControlScheme.Touch)
			return;
		UICamera.currentTouch = UICamera.mMouse[0];
		Vector2 mousePosition = UnityXInput.Input.mousePosition;
		if (UICamera.currentTouch.ignoreDelta == 0)
		{
			UICamera.currentTouch.delta = mousePosition - UICamera.currentTouch.pos;
		}
		else
		{
			UICamera.currentTouch.ignoreDelta--;
			UICamera.currentTouch.delta.x = 0f;
			UICamera.currentTouch.delta.y = 0f;
		}
		Single mouseMoveSqrDist = UICamera.currentTouch.delta.sqrMagnitude;
		UICamera.currentTouch.pos = mousePosition;
		UICamera.mLastPos = mousePosition;
		Boolean movingMouse = false;
		if (UICamera.currentScheme != UICamera.ControlScheme.Mouse)
		{
			if (mouseMoveSqrDist < 0.001f)
				return;
			UICamera.currentKey = KeyCode.Mouse0;
			movingMouse = true;
		}
		else if (mouseMoveSqrDist > 0.001f)
		{
			movingMouse = true;
		}
		for (Int32 i = 1; i < 3; i++)
		{
			UICamera.mMouse[i].pos = UICamera.currentTouch.pos;
			UICamera.mMouse[i].delta = UICamera.currentTouch.delta;
		}
		if (isClicking || movingMouse || this.mNextRaycast < RealTime.time)
		{
			this.mNextRaycast = RealTime.time + 0.02f;
			UICamera.Raycast(UICamera.currentTouch);
			for (Int32 i = 0; i < 3; i++)
				UICamera.mMouse[i].current = UICamera.currentTouch.current;
		}
		Boolean touchHasChanged = UICamera.currentTouch.last != UICamera.currentTouch.current;
		Boolean pressingButton = UICamera.currentTouch.pressed != null;
		if (!pressingButton)
			UICamera.hoveredObject = UICamera.currentTouch.current;
		UICamera.currentTouchID = -1;
		if (touchHasChanged)
			UICamera.currentKey = KeyCode.Mouse0;
		if (!isClicking && movingMouse && (!this.stickyTooltip || touchHasChanged))
		{
			if (UICamera.mTooltipTime != 0f)
				UICamera.mTooltipTime = Time.unscaledTime + this.tooltipDelay;
			else if (UICamera.mTooltip != null)
				UICamera.ShowTooltip(null);
		}
		if (movingMouse && UICamera.onMouseMove != null)
		{
			UICamera.onMouseMove(UICamera.currentTouch.delta);
			UICamera.currentTouch = null;
		}
		if (touchHasChanged && (isClickingNow || (pressingButton && !isClicking)))
			UICamera.hoveredObject = null;
		for (Int32 i = 0; i < 3; i++)
		{
			Boolean mouseButtonDown = UnityXInput.Input.GetMouseButtonDown(i);
			Boolean mouseButtonUp = UnityXInput.Input.GetMouseButtonUp(i);
			if (mouseButtonDown || mouseButtonUp)
				UICamera.currentKey = KeyCode.Mouse0 + i;
			UICamera.currentTouch = UICamera.mMouse[i];
			UICamera.currentTouchID = -1 - i;
			UICamera.currentKey = KeyCode.Mouse0 + i;
			if (mouseButtonDown)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
				UICamera.currentTouch.pressTime = RealTime.time;
			}
			else if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			this.ProcessTouch(mouseButtonDown, mouseButtonUp);
		}
		if (!isClicking && touchHasChanged)
		{
			UICamera.currentTouch = UICamera.mMouse[0];
			UICamera.mTooltipTime = RealTime.time + this.tooltipDelay;
			UICamera.currentTouchID = -1;
			UICamera.currentKey = KeyCode.Mouse0;
			UICamera.hoveredObject = UICamera.currentTouch.current;
		}
		UICamera.currentTouch = null;
		UICamera.mMouse[0].last = UICamera.mMouse[0].current;
		for (Int32 i = 1; i < 3; i++)
			UICamera.mMouse[i].last = UICamera.mMouse[0].last;
	}

	public void ProcessTouches()
	{
		Int32 num = (Int32)((UICamera.GetInputTouchCount != null) ? UICamera.GetInputTouchCount() : UnityXInput.Input.touchCount);
		for (Int32 i = 0; i < num; i++)
		{
			TouchPhase phase;
			Int32 fingerId;
			Vector2 position;
			Int32 tapCount;
			if (UICamera.GetInputTouch == null)
			{
				UnityEngine.Touch touch = UnityXInput.Input.GetTouch(i);
				phase = touch.phase;
				fingerId = touch.fingerId;
				position = touch.position;
				tapCount = touch.tapCount;
			}
			else
			{
				UICamera.Touch touch2 = UICamera.GetInputTouch(i);
				phase = touch2.phase;
				fingerId = touch2.fingerId;
				position = touch2.position;
				tapCount = touch2.tapCount;
			}
			UICamera.currentTouchID = (Int32)((!this.allowMultiTouch) ? 1 : fingerId);
			UICamera.currentTouch = UICamera.GetTouch(UICamera.currentTouchID, true);
			Boolean flag = phase == TouchPhase.Began || UICamera.currentTouch.touchBegan;
			Boolean flag2 = phase == TouchPhase.Canceled || phase == TouchPhase.Ended;
			UICamera.currentTouch.touchBegan = false;
			UICamera.currentTouch.delta = position - UICamera.currentTouch.pos;
			UICamera.currentTouch.pos = position;
			UICamera.currentKey = KeyCode.None;
			UICamera.Raycast(UICamera.currentTouch);
			if (flag)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
			}
			else if (UICamera.currentTouch.pressed != (UnityEngine.Object)null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			if (tapCount > 1)
			{
				UICamera.currentTouch.clickTime = RealTime.time;
			}
			this.ProcessTouch(flag, flag2);
			if (flag2)
			{
				UICamera.RemoveTouch(UICamera.currentTouchID);
			}
			UICamera.currentTouch.last = (GameObject)null;
			UICamera.currentTouch = (UICamera.MouseOrTouch)null;
			if (!this.allowMultiTouch)
			{
				break;
			}
		}
		if (num == 0)
		{
			if (UICamera.mUsingTouchEvents)
			{
				UICamera.mUsingTouchEvents = false;
				return;
			}
			if (this.useMouse)
			{
				this.ProcessMouse();
			}
		}
		else
		{
			UICamera.mUsingTouchEvents = true;
		}
	}

	private void ProcessFakeTouches()
	{
		Boolean mouseButtonDown = UnityXInput.Input.GetMouseButtonDown(0);
		Boolean mouseButtonUp = UnityXInput.Input.GetMouseButtonUp(0);
		Boolean mouseButton = UnityXInput.Input.GetMouseButton(0);
		if (mouseButtonDown || mouseButtonUp || mouseButton)
		{
			UICamera.currentTouchID = 1;
			UICamera.currentTouch = UICamera.mMouse[0];
			UICamera.currentTouch.touchBegan = mouseButtonDown;
			if (mouseButtonDown)
			{
				UICamera.currentTouch.pressTime = RealTime.time;
				UICamera.activeTouches.Add(UICamera.currentTouch);
			}
			Vector2 mousePosition = UnityXInput.Input.mousePosition;
			UICamera.currentTouch.delta = mousePosition - UICamera.currentTouch.pos;
			UICamera.currentTouch.pos = mousePosition;
			UICamera.Raycast(UICamera.currentTouch);
			if (mouseButtonDown)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
			}
			else if (UICamera.currentTouch.pressed != (UnityEngine.Object)null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			UICamera.currentKey = KeyCode.None;
			this.ProcessTouch(mouseButtonDown, mouseButtonUp);
			if (mouseButtonUp)
			{
				UICamera.activeTouches.Remove(UICamera.currentTouch);
			}
			UICamera.currentTouch.last = (GameObject)null;
			UICamera.currentTouch = (UICamera.MouseOrTouch)null;
		}
	}

	public void ProcessOthers()
	{
		UICamera.currentTouchID = -100;
		UICamera.currentTouch = UICamera.controller;
		Boolean flag = false;
		Boolean flag2 = false;
		if (this.submitKey0 != KeyCode.None && UICamera.GetKeyDown(this.submitKey0))
		{
			UICamera.currentKey = this.submitKey0;
			flag = true;
		}
		else if (this.submitKey1 != KeyCode.None && UICamera.GetKeyDown(this.submitKey1))
		{
			UICamera.currentKey = this.submitKey1;
			flag = true;
		}
		else if ((this.submitKey0 == KeyCode.Return || this.submitKey1 == KeyCode.Return) && UICamera.GetKeyDown(KeyCode.KeypadEnter))
		{
			UICamera.currentKey = this.submitKey0;
			flag = true;
		}
		if (this.submitKey0 != KeyCode.None && UICamera.GetKeyUp(this.submitKey0))
		{
			UICamera.currentKey = this.submitKey0;
			flag2 = true;
		}
		else if (this.submitKey1 != KeyCode.None && UICamera.GetKeyUp(this.submitKey1))
		{
			UICamera.currentKey = this.submitKey1;
			flag2 = true;
		}
		else if ((this.submitKey0 == KeyCode.Return || this.submitKey1 == KeyCode.Return) && UICamera.GetKeyUp(KeyCode.KeypadEnter))
		{
			UICamera.currentKey = this.submitKey0;
			flag2 = true;
		}
		if (flag)
		{
			UICamera.currentTouch.pressTime = RealTime.time;
		}
		if ((flag || flag2) && UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			UICamera.currentTouch.current = UICamera.controllerNavigationObject;
			this.ProcessTouch(flag, flag2);
			UICamera.currentTouch.last = UICamera.currentTouch.current;
		}
		KeyCode keyCode = KeyCode.None;
		if (this.useController)
		{
			if (!UICamera.disableController && UICamera.currentScheme == UICamera.ControlScheme.Controller && (UICamera.currentTouch.current == (UnityEngine.Object)null || !UICamera.currentTouch.current.activeInHierarchy))
			{
				UICamera.currentTouch.current = UICamera.controllerNavigationObject;
			}
			if (!String.IsNullOrEmpty(this.verticalAxisName))
			{
				Int32 direction = UICamera.GetDirection(this.verticalAxisName);
				if (direction != 0)
				{
					UICamera.ShowTooltip((GameObject)null);
					UICamera.currentScheme = UICamera.ControlScheme.Controller;
					UICamera.currentTouch.current = UICamera.controllerNavigationObject;
					if (UICamera.currentTouch.current != (UnityEngine.Object)null)
					{
						keyCode = (KeyCode)((direction <= 0) ? KeyCode.DownArrow : KeyCode.UpArrow);
						if (UICamera.onNavigate != null)
						{
							UICamera.onNavigate(UICamera.currentTouch.current, keyCode);
						}
						UICamera.Notify(UICamera.currentTouch.current, "OnNavigate", keyCode);
					}
				}
			}
			if (!String.IsNullOrEmpty(this.horizontalAxisName))
			{
				Int32 direction2 = UICamera.GetDirection(this.horizontalAxisName);
				if (direction2 != 0)
				{
					UICamera.ShowTooltip((GameObject)null);
					UICamera.currentScheme = UICamera.ControlScheme.Controller;
					UICamera.currentTouch.current = UICamera.controllerNavigationObject;
					if (UICamera.currentTouch.current != (UnityEngine.Object)null)
					{
						keyCode = (KeyCode)((direction2 <= 0) ? KeyCode.LeftArrow : KeyCode.RightArrow);
						if (UICamera.onNavigate != null)
						{
							UICamera.onNavigate(UICamera.currentTouch.current, keyCode);
						}
						UICamera.Notify(UICamera.currentTouch.current, "OnNavigate", keyCode);
					}
				}
			}
			Single num = String.IsNullOrEmpty(this.horizontalPanAxisName) ? 0f : UICamera.GetAxis(this.horizontalPanAxisName);
			Single num2 = String.IsNullOrEmpty(this.verticalPanAxisName) ? 0f : UICamera.GetAxis(this.verticalPanAxisName);
			if (num != 0f || num2 != 0f)
			{
				UICamera.ShowTooltip((GameObject)null);
				UICamera.currentScheme = UICamera.ControlScheme.Controller;
				UICamera.currentTouch.current = UICamera.controllerNavigationObject;
				if (UICamera.currentTouch.current != (UnityEngine.Object)null)
				{
					Vector2 vector = new Vector2(num, num2);
					vector *= Time.unscaledDeltaTime;
					if (UICamera.onPan != null)
					{
						UICamera.onPan(UICamera.currentTouch.current, vector);
					}
					UICamera.Notify(UICamera.currentTouch.current, "OnPan", vector);
				}
			}
		}
		if (UnityXInput.Input.anyKeyDown)
		{
			Int32 i = 0;
			Int32 num3 = (Int32)NGUITools.keys.Length;
			while (i < num3)
			{
				KeyCode keyCode2 = NGUITools.keys[i];
				if (keyCode != keyCode2)
				{
					if (UICamera.GetKeyDown(keyCode2))
					{
						if (this.useKeyboard || keyCode2 >= KeyCode.Mouse0)
						{
							if (this.useController || keyCode2 < KeyCode.JoystickButton0)
							{
								if (this.useMouse || (keyCode2 < KeyCode.Mouse0 && keyCode2 > KeyCode.Mouse6))
								{
									UICamera.currentKey = keyCode2;
									if (UICamera.onKey != null)
									{
										UICamera.onKey(UICamera.currentTouch.current, keyCode2);
									}
									UICamera.Notify(UICamera.currentTouch.current, "OnKey", keyCode2);
								}
							}
						}
					}
				}
				i++;
			}
		}
		UICamera.currentTouch = (UICamera.MouseOrTouch)null;
	}

	private void ProcessPress(Boolean pressed, Single click, Single drag)
	{
		if (pressed)
		{
			if (UICamera.mTooltip != (UnityEngine.Object)null)
			{
				UICamera.ShowTooltip((GameObject)null);
			}
			UICamera.currentTouch.pressStarted = true;
			if (UICamera.onPress != null && UICamera.currentTouch.pressed)
			{
				UICamera.onPress(UICamera.currentTouch.pressed, false);
			}
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
			UICamera.currentTouch.pressed = UICamera.currentTouch.current;
			UICamera.currentTouch.dragged = UICamera.currentTouch.current;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			UICamera.currentTouch.totalDelta = Vector2.zero;
			UICamera.currentTouch.dragStarted = false;
			if (UICamera.onPress != null && UICamera.currentTouch.pressed)
			{
				UICamera.onPress(UICamera.currentTouch.pressed, true);
			}
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", true);
			if (UICamera.mTooltip != (UnityEngine.Object)null)
			{
				UICamera.ShowTooltip((GameObject)null);
			}
			if (UICamera.mSelected != UICamera.currentTouch.pressed)
			{
				UICamera.mInputFocus = false;
				if (UICamera.mSelected)
				{
					UICamera.Notify(UICamera.mSelected, "OnSelect", false);
					if (UICamera.onSelect != null)
					{
						UICamera.onSelect(UICamera.mSelected, false);
					}
				}
				UICamera.mSelected = UICamera.currentTouch.pressed;
				if (UICamera.currentTouch.pressed != (UnityEngine.Object)null)
				{
					UIKeyNavigation component = UICamera.currentTouch.pressed.GetComponent<UIKeyNavigation>();
					if (component != (UnityEngine.Object)null)
					{
						UICamera.controller.current = UICamera.currentTouch.pressed;
					}
				}
				if (UICamera.mSelected)
				{
					UICamera.mInputFocus = (UICamera.mSelected.activeInHierarchy && UICamera.mSelected.GetComponent<UIInput>() != (UnityEngine.Object)null);
					if (UICamera.onSelect != null)
					{
						UICamera.onSelect(UICamera.mSelected, true);
					}
					UICamera.Notify(UICamera.mSelected, "OnSelect", true);
				}
			}
		}
		else if (UICamera.currentTouch.pressed != (UnityEngine.Object)null && (UICamera.currentTouch.delta.sqrMagnitude != 0f || UICamera.currentTouch.current != UICamera.currentTouch.last))
		{
			UICamera.currentTouch.totalDelta += UICamera.currentTouch.delta;
			Single sqrMagnitude = UICamera.currentTouch.totalDelta.sqrMagnitude;
			Boolean flag = false;
			if (!UICamera.currentTouch.dragStarted && UICamera.currentTouch.last != UICamera.currentTouch.current)
			{
				UICamera.currentTouch.dragStarted = true;
				UICamera.currentTouch.delta = UICamera.currentTouch.totalDelta;
				UICamera.isDragging = true;
				if (UICamera.onDragStart != null)
				{
					UICamera.onDragStart(UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.dragged, "OnDragStart", null);
				if (UICamera.onDragOver != null)
				{
					UICamera.onDragOver(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.last, "OnDragOver", UICamera.currentTouch.dragged);
				UICamera.isDragging = false;
			}
			else if (!UICamera.currentTouch.dragStarted && drag < sqrMagnitude)
			{
				flag = true;
				UICamera.currentTouch.dragStarted = true;
				UICamera.currentTouch.delta = UICamera.currentTouch.totalDelta;
			}
			if (UICamera.currentTouch.dragStarted)
			{
				if (UICamera.mTooltip != (UnityEngine.Object)null)
				{
					UICamera.ShowTooltip((GameObject)null);
				}
				UICamera.isDragging = true;
				Boolean flag2 = UICamera.currentTouch.clickNotification == UICamera.ClickNotification.None;
				if (flag)
				{
					if (UICamera.onDragStart != null)
					{
						UICamera.onDragStart(UICamera.currentTouch.dragged);
					}
					UICamera.Notify(UICamera.currentTouch.dragged, "OnDragStart", null);
					if (UICamera.onDragOver != null)
					{
						UICamera.onDragOver(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
					}
					UICamera.Notify(UICamera.currentTouch.current, "OnDragOver", UICamera.currentTouch.dragged);
				}
				else if (UICamera.currentTouch.last != UICamera.currentTouch.current)
				{
					if (UICamera.onDragOut != null)
					{
						UICamera.onDragOut(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
					}
					UICamera.Notify(UICamera.currentTouch.last, "OnDragOut", UICamera.currentTouch.dragged);
					if (UICamera.onDragOver != null)
					{
						UICamera.onDragOver(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
					}
					UICamera.Notify(UICamera.currentTouch.current, "OnDragOver", UICamera.currentTouch.dragged);
				}
				if (UICamera.onDrag != null)
				{
					UICamera.onDrag(UICamera.currentTouch.dragged, UICamera.currentTouch.delta);
				}
				UICamera.Notify(UICamera.currentTouch.dragged, "OnDrag", UICamera.currentTouch.delta);
				UICamera.currentTouch.last = UICamera.currentTouch.current;
				UICamera.isDragging = false;
				if (flag2)
				{
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				}
				else if (UICamera.currentTouch.clickNotification == UICamera.ClickNotification.BasedOnDelta && click < sqrMagnitude)
				{
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				}
			}
		}
	}

	private void ProcessRelease(Boolean isMouse, Single drag)
	{
		if (UICamera.currentTouch == null)
		{
			return;
		}
		UICamera.currentTouch.pressStarted = false;
		if (UICamera.currentTouch.pressed != (UnityEngine.Object)null)
		{
			if (UICamera.currentTouch.dragStarted)
			{
				if (UICamera.onDragOut != null)
				{
					UICamera.onDragOut(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.last, "OnDragOut", UICamera.currentTouch.dragged);
				if (UICamera.onDragEnd != null)
				{
					UICamera.onDragEnd(UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.dragged, "OnDragEnd", null);
			}
			if (UICamera.onPress != null)
			{
				UICamera.onPress(UICamera.currentTouch.pressed, false);
			}
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
			if (isMouse && this.HasCollider(UICamera.currentTouch.pressed))
			{
				if (UICamera.mHover == UICamera.currentTouch.current)
				{
					if (UICamera.onHover != null)
					{
						UICamera.onHover(UICamera.currentTouch.current, true);
					}
					UICamera.Notify(UICamera.currentTouch.current, "OnHover", true);
				}
				else
				{
					UICamera.hoveredObject = UICamera.currentTouch.current;
				}
			}
			if (UICamera.currentTouch.dragged == UICamera.currentTouch.current || (UICamera.currentScheme != UICamera.ControlScheme.Controller && UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None && UICamera.currentTouch.totalDelta.sqrMagnitude < drag))
			{
				if (UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None && UICamera.currentTouch.pressed == UICamera.currentTouch.current)
				{
					UICamera.ShowTooltip((GameObject)null);
					Single time = RealTime.time;
					if (UICamera.onClick != null)
					{
						UICamera.onClick(UICamera.currentTouch.pressed);
					}
					UICamera.Notify(UICamera.currentTouch.pressed, "OnClick", null);
					if (UICamera.currentTouch.clickTime + 0.35f > time)
					{
						if (UICamera.onDoubleClick != null)
						{
							UICamera.onDoubleClick(UICamera.currentTouch.pressed);
						}
						UICamera.Notify(UICamera.currentTouch.pressed, "OnDoubleClick", null);
					}
					UICamera.currentTouch.clickTime = time;
				}
			}
			else if (UICamera.currentTouch.dragStarted)
			{
				if (UICamera.onDrop != null)
				{
					UICamera.onDrop(UICamera.currentTouch.current, UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.current, "OnDrop", UICamera.currentTouch.dragged);
			}
		}
		UICamera.currentTouch.dragStarted = false;
		UICamera.currentTouch.pressed = (GameObject)null;
		UICamera.currentTouch.dragged = (GameObject)null;
	}

	private Boolean HasCollider(GameObject go)
	{
		if (go == (UnityEngine.Object)null)
		{
			return false;
		}
		Collider component = go.GetComponent<Collider>();
		if (component != (UnityEngine.Object)null)
		{
			return component.enabled;
		}
		Collider2D component2 = go.GetComponent<Collider2D>();
		return component2 != (UnityEngine.Object)null && component2.enabled;
	}

	public void ProcessTouch(Boolean pressed, Boolean released)
	{
		if (pressed)
		{
			UICamera.mTooltipTime = Time.unscaledTime + this.tooltipDelay;
		}
		Boolean flag = UICamera.currentScheme == UICamera.ControlScheme.Mouse;
		Single num = (!flag) ? this.touchDragThreshold : this.mouseDragThreshold;
		Single num2 = (!flag) ? this.touchClickThreshold : this.mouseClickThreshold;
		num *= num;
		num2 *= num2;
		if (UICamera.currentTouch.pressed != (UnityEngine.Object)null)
		{
			if (released)
			{
				this.ProcessRelease(flag, num);
			}
			this.ProcessPress(pressed, num2, num);
			if (UICamera.currentTouch.pressed == UICamera.currentTouch.current && UICamera.mTooltipTime != 0f && UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None && !UICamera.currentTouch.dragStarted && UICamera.currentTouch.deltaTime > this.tooltipDelay)
			{
				UICamera.mTooltipTime = 0f;
				UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				if (this.longPressTooltip)
				{
					UICamera.ShowTooltip(UICamera.currentTouch.pressed);
				}
				UICamera.Notify(UICamera.currentTouch.current, "OnLongPress", null);
			}
		}
		else if (flag || pressed || released)
		{
			this.ProcessPress(pressed, num2, num);
			if (released)
			{
				this.ProcessRelease(flag, num);
			}
		}
	}

	public static Boolean ShowTooltip(GameObject go)
	{
		if (UICamera.mTooltip != go)
		{
			if (UICamera.mTooltip != (UnityEngine.Object)null)
			{
				if (UICamera.onTooltip != null)
				{
					UICamera.onTooltip(UICamera.mTooltip, false);
				}
				UICamera.Notify(UICamera.mTooltip, "OnTooltip", false);
			}
			UICamera.mTooltip = go;
			UICamera.mTooltipTime = 0f;
			if (UICamera.mTooltip != (UnityEngine.Object)null)
			{
				if (UICamera.onTooltip != null)
				{
					UICamera.onTooltip(UICamera.mTooltip, true);
				}
				UICamera.Notify(UICamera.mTooltip, "OnTooltip", true);
			}
			return true;
		}
		return false;
	}

	public static Boolean HideTooltip()
	{
		return UICamera.ShowTooltip((GameObject)null);
	}

	public static BetterList<UICamera> list = new BetterList<UICamera>();

	public static UICamera.GetKeyStateFunc GetKeyDown = new UICamera.GetKeyStateFunc(UnityXInput.Input.GetKeyDown);

	public static UICamera.GetKeyStateFunc GetKeyUp = new UICamera.GetKeyStateFunc(UnityXInput.Input.GetKeyUp);

	public static UICamera.GetKeyStateFunc GetKey = new UICamera.GetKeyStateFunc(UnityXInput.Input.GetKey);

	public static UICamera.GetAxisFunc GetAxis = new UICamera.GetAxisFunc(UnityXInput.Input.GetAxis);

	public static UICamera.GetAnyKeyFunc GetAnyKeyDown;

	public static UICamera.OnScreenResize onScreenResize;

	public UICamera.EventType eventType = UICamera.EventType.UI_3D;

	public Boolean eventsGoToColliders;

	public LayerMask eventReceiverMask = -1;

	public Boolean debug;

	public Boolean useMouse = true;

	public Boolean useTouch = true;

	public Boolean allowMultiTouch = true;

	public Boolean useKeyboard = true;

	public Boolean useController = true;

	public Boolean stickyTooltip = true;

	public Single tooltipDelay = 1f;

	public Boolean longPressTooltip;

	public Single mouseDragThreshold = 4f;

	public Single mouseClickThreshold = 10f;

	public Single touchDragThreshold = 40f;

	public Single touchClickThreshold = 40f;

	public Single rangeDistance = -1f;

	public String horizontalAxisName = "Horizontal";

	public String verticalAxisName = "Vertical";

	public String horizontalPanAxisName;

	public String verticalPanAxisName;

	public String scrollAxisName = "Mouse ScrollWheel";

	public Boolean commandClick = true;

	public KeyCode submitKey0 = KeyCode.Return;

	public KeyCode submitKey1 = KeyCode.JoystickButton0;

	public KeyCode cancelKey0 = KeyCode.Escape;

	public KeyCode cancelKey1 = KeyCode.JoystickButton1;

	public static UICamera.OnCustomInput onCustomInput;

	public static Boolean showTooltips = true;

	private static Boolean mDisableController = false;

	private static Vector2 mLastPos = Vector2.zero;

	public static Vector3 lastWorldPosition = Vector3.zero;

	public static RaycastHit lastHit;

	public static UICamera current = (UICamera)null;

	public static Camera currentCamera = (Camera)null;

	public static UICamera.OnSchemeChange onSchemeChange;

	public static Int32 currentTouchID = -100;

	private static KeyCode mCurrentKey = KeyCode.Alpha0;

	public static UICamera.MouseOrTouch currentTouch = (UICamera.MouseOrTouch)null;

	private static Boolean mInputFocus = false;

	private static GameObject mGenericHandler;

	public static GameObject fallThrough;

	public static UICamera.VoidDelegate onClick;

	public static UICamera.VoidDelegate onDoubleClick;

	public static UICamera.BoolDelegate onHover;

	public static UICamera.BoolDelegate onPress;

	public static UICamera.BoolDelegate onSelect;

	public static UICamera.FloatDelegate onScroll;

	public static UICamera.VectorDelegate onDrag;

	public static UICamera.VoidDelegate onDragStart;

	public static UICamera.ObjectDelegate onDragOver;

	public static UICamera.ObjectDelegate onDragOut;

	public static UICamera.VoidDelegate onDragEnd;

	public static UICamera.ObjectDelegate onDrop;

	public static UICamera.KeyCodeDelegate onKey;

	public static UICamera.KeyCodeDelegate onNavigate;

	public static UICamera.VectorDelegate onPan;

	public static UICamera.BoolDelegate onTooltip;

	public static UICamera.MoveDelegate onMouseMove;

	private static UICamera.MouseOrTouch[] mMouse = new UICamera.MouseOrTouch[]
	{
		new UICamera.MouseOrTouch(),
		new UICamera.MouseOrTouch(),
		new UICamera.MouseOrTouch()
	};

	public static UICamera.MouseOrTouch controller = new UICamera.MouseOrTouch();

	public static List<UICamera.MouseOrTouch> activeTouches = new List<UICamera.MouseOrTouch>();

	private static List<Int32> mTouchIDs = new List<Int32>();

	private static Int32 mWidth = 0;

	private static Int32 mHeight = 0;

	private static GameObject mTooltip = (GameObject)null;

	private Camera mCam;

	private static Single mTooltipTime = 0f;

	private Single mNextRaycast;

	public static Boolean isDragging = false;

	private static GameObject mRayHitObject;

	private static GameObject mHover;

	private static GameObject mSelected;

	private static UICamera.DepthEntry mHit = default(UICamera.DepthEntry);

	private static BetterList<UICamera.DepthEntry> mHits = new BetterList<UICamera.DepthEntry>();

	private static Plane m2DPlane = new Plane(Vector3.back, 0f);

	private static Single mNextEvent = 0f;

	private static Single mDefaultEventWaitTime = 0.175f;

	private static Single mEventWaitTime = UICamera.mDefaultEventWaitTime;

	private static Int32 mNotifying = 0;

	private static Boolean mUsingTouchEvents = true;

	public static UICamera.GetTouchCountCallback GetInputTouchCount;

	public static UICamera.GetTouchCallback GetInputTouch;

	public enum ControlScheme
	{
		Mouse,
		Touch,
		Controller
	}

	public enum ClickNotification
	{
		None,
		Always,
		BasedOnDelta
	}

	public class MouseOrTouch
	{
		public Single deltaTime
		{
			get
			{
				return RealTime.time - this.pressTime;
			}
		}

		public Boolean isOverUI
		{
			get
			{
				return this.current != (UnityEngine.Object)null && this.current != UICamera.fallThrough && NGUITools.FindInParents<UIRoot>(this.current) != (UnityEngine.Object)null;
			}
		}

		public KeyCode key;

		public Vector2 pos;

		public Vector2 lastPos;

		public Vector2 delta;

		public Vector2 totalDelta;

		public Camera pressedCam;

		public GameObject last;

		public GameObject current;

		public GameObject pressed;

		public GameObject dragged;

		public Single pressTime;

		public Single clickTime;

		public UICamera.ClickNotification clickNotification = UICamera.ClickNotification.Always;

		public Boolean touchBegan = true;

		public Boolean pressStarted;

		public Boolean dragStarted;

		public Int32 ignoreDelta;
	}

	public enum EventType
	{
		World_3D,
		UI_3D,
		World_2D,
		UI_2D
	}

	private struct DepthEntry
	{
		public Int32 depth;

		public RaycastHit hit;

		public Vector3 point;

		public GameObject go;
	}

	public class Touch
	{
		public Int32 fingerId;

		public TouchPhase phase;

		public Vector2 position;

		public Int32 tapCount;
	}

	public delegate Boolean GetKeyStateFunc(KeyCode key);

	public delegate Single GetAxisFunc(String name);

	public delegate Boolean GetAnyKeyFunc();

	public delegate void OnScreenResize();

	public delegate void OnCustomInput();

	public delegate void OnSchemeChange();

	public delegate void MoveDelegate(Vector2 delta);

	public delegate void VoidDelegate(GameObject go);

	public delegate void BoolDelegate(GameObject go, Boolean state);

	public delegate void FloatDelegate(GameObject go, Single delta);

	public delegate void VectorDelegate(GameObject go, Vector2 delta);

	public delegate void ObjectDelegate(GameObject go, GameObject obj);

	public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

	public delegate Int32 GetTouchCountCallback();

	public delegate UICamera.Touch GetTouchCallback(Int32 index);
}
