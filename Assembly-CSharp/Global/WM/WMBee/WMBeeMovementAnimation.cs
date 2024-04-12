using System;
using UnityEngine;

public class WMBeeMovementAnimation : Singleton<WMBeeMovementAnimation>
{
	private WMActor ControlledActor
	{
		get
		{
			return WMActor.ControlledDebugDebugActor;
		}
	}

	public WMActor ChocoboActor { get; private set; }

	public void Initialize()
	{
		this.transform = 0;
	}

	private void UpdateSmoothedMovementDirectionHuman()
	{
		WMActor controlledDebugDebugActor = WMActor.ControlledDebugDebugActor;
		if (!this.cameraTransform)
		{
			this.cameraTransform = GameObject.Find("WorldCamera").transform;
		}
		Boolean flag = this.IsGrounded();
		Vector3 a = this.cameraTransform.TransformDirection(Vector3.forward);
		a.y = 0f;
		a = a.normalized;
		Vector3 a2 = new Vector3(a.z, 0f, -a.x);
		Vector2 axis = PersistenSingleton<HonoInputManager>.Instance.GetAxis();
		Single num = axis.x;
		Single y = axis.y;
		if (y < -0.2f)
		{
			controlledDebugDebugActor.MovingBack = true;
		}
		else
		{
			controlledDebugDebugActor.MovingBack = false;
		}
		Boolean isMoving = controlledDebugDebugActor.isMoving;
		controlledDebugDebugActor.isMoving = (Mathf.Abs(num) > 0.1f || Mathf.Abs(y) > 0.1f);
		if (Mathf.Abs(num) < 0.7f)
		{
			num = 0f;
		}
		Vector3 vector = num * a2 + y * a;
		if (flag)
		{
			controlledDebugDebugActor.LockCameraTimer += Time.deltaTime;
			if (controlledDebugDebugActor.isMoving != isMoving)
			{
				controlledDebugDebugActor.LockCameraTimer = 0f;
			}
			if (vector != Vector3.zero)
			{
				if (controlledDebugDebugActor.moveSpeed < controlledDebugDebugActor.walkSpeed * 0.9f && flag)
				{
					controlledDebugDebugActor.moveDirection = vector.normalized;
				}
				else
				{
					controlledDebugDebugActor.moveDirection = Vector3.RotateTowards(controlledDebugDebugActor.moveDirection, vector, controlledDebugDebugActor.rotateSpeed * 0.0174532924f * Time.deltaTime, 1000f);
					controlledDebugDebugActor.moveDirection = controlledDebugDebugActor.moveDirection.normalized;
				}
			}
			Single t = controlledDebugDebugActor.SpeedSmoothing * Time.deltaTime;
			Single num2 = Mathf.Min(vector.magnitude, 1f);
			Boolean flag2 = true;
			controlledDebugDebugActor.State = WMActorStateDebug.Idle;
			if (num != 0f || y != 0f)
			{
				flag2 = false;
			}
			Boolean flag3 = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			if (!flag2)
			{
				if (flag3)
				{
					num2 *= controlledDebugDebugActor.runSpeed;
					controlledDebugDebugActor.State = WMActorStateDebug.Sprint;
				}
				else
				{
					num2 *= controlledDebugDebugActor.walkSpeed;
					controlledDebugDebugActor.State = WMActorStateDebug.Running;
				}
			}
			controlledDebugDebugActor.moveSpeed = Mathf.Lerp(controlledDebugDebugActor.moveSpeed, num2, t);
			if (controlledDebugDebugActor.moveSpeed < controlledDebugDebugActor.walkSpeed * 0.3f)
			{
			}
		}
	}

	public void OnUpdate()
	{
		if (!FF9StateSystem.World.IsBeeScene)
		{
			return;
		}
		if (this.ControlledActor.ControlNo == 0 || this.ControlledActor.ControlNo == 1 || this.ControlledActor.ControlNo == 2 || this.ControlledActor.ControlNo == 3 || this.ControlledActor.ControlNo == 4 || this.ControlledActor.ControlNo == 5)
		{
			this.UpdateHuman();
		}
		else if (this.ControlledActor.ControlNo == 7)
		{
			this.UpdateShip();
		}
		else if (this.ControlledActor.ControlNo == 8 || this.ControlledActor.ControlNo == 9 || this.ControlledActor.ControlNo == 6)
		{
			this.UpdatePlane();
		}
	}

	private void UpdateShip()
	{
		this.UpdateInputShip();
		WMActor controlledActor = this.ControlledActor;
		if (!controlledActor.isControllable)
		{
			Input.ResetInputAxes();
		}
		this.UpdateSmoothedMovementDirectionShip();
		controlledActor.moveDirection = controlledActor.transform.TransformDirection(Vector3.forward);
		controlledActor.moveDirection.Normalize();
		controlledActor.moveSpeed = controlledActor.walkSpeed * controlledActor.moveZ;
		Vector3 position = controlledActor.transform.position;
		position.y += controlledActor.MoveY * controlledActor.VerticalSpeed * Time.deltaTime;
		Single num = Mathf.Abs(0.164794922f);
		if (position.y > num)
		{
			position.y = num;
		}
		if (controlledActor.moveZ > 0f)
		{
			controlledActor.State = WMActorStateDebug.Running;
		}
		else if (controlledActor.moveZ < 0f)
		{
			controlledActor.State = WMActorStateDebug.Running;
		}
		else
		{
			controlledActor.State = WMActorStateDebug.Idle;
		}
	}

	private void UpdateSmoothedMovementDirectionShip()
	{
		WMActor controlledActor = this.ControlledActor;
		Vector2 axis = PersistenSingleton<HonoInputManager>.Instance.GetAxis();
		Single y = axis.y;
		Single x = axis.x;
		controlledActor.bodyRotateSpeedZ = controlledActor.RollSpeed;
		controlledActor.bodyMaxRotationZ = controlledActor.RollMaxAngle;
		if (x > 0f)
		{
			Quaternion rotation = controlledActor.transform.rotation;
			Vector3 angle = rotation.eulerAngles;
			angle.y += controlledActor.RotateSpeed * Time.deltaTime;
			rotation.eulerAngles = angle;
			controlledActor.transform.rotation = rotation;
		}
		else if (x < 0f)
		{
			Quaternion rotation = controlledActor.transform.rotation;
			Vector3 angle = rotation.eulerAngles;
			angle.y -= controlledActor.RotateSpeed * Time.deltaTime;
			rotation.eulerAngles = angle;
			controlledActor.transform.rotation = rotation;
		}
		else if (controlledActor.bodyRotationZ < 0f)
		{
			controlledActor.bodyRotationZ += controlledActor.bodyRotateSpeedZ * Time.deltaTime;
			if (controlledActor.bodyRotationZ > 0f)
			{
				controlledActor.bodyRotationZ = 0f;
			}
		}
		else if (controlledActor.bodyRotationZ > 0f)
		{
			controlledActor.bodyRotationZ += -controlledActor.bodyRotateSpeedZ * Time.deltaTime;
			if (controlledActor.bodyRotationZ < 0f)
			{
				controlledActor.bodyRotationZ = 0f;
			}
		}
	}

	private void UpdateInputShip()
	{
		WMActor controlledActor = this.ControlledActor;
		if (Input.GetKey(KeyCode.N) || Input.GetKey(KeyCode.JoystickButton0))
		{
			controlledActor.moveZ = 1f;
		}
		else if (Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.JoystickButton1))
		{
			controlledActor.moveZ = -1f;
		}
		else
		{
			controlledActor.moveZ = 0f;
		}
	}

	private void UpdatePlane()
	{
		this.UpdateInputPlane();
		WMActor controlledActor = this.ControlledActor;
		if (!controlledActor.isControllable)
		{
			Input.ResetInputAxes();
		}
		controlledActor.moveDirection = controlledActor.transform.TransformDirection(Vector3.forward);
		controlledActor.moveDirection.Normalize();
		if (controlledActor.moveZ > 0f)
		{
			controlledActor.State = WMActorStateDebug.Running;
		}
		else if (controlledActor.moveZ < 0f)
		{
			controlledActor.State = WMActorStateDebug.Running;
		}
		else
		{
			controlledActor.State = WMActorStateDebug.Idle;
		}
		if (FF9StateSystem.World.IsBeeScene)
		{
			if (controlledActor.State == WMActorStateDebug.Idle)
			{
				if (!controlledActor.Animation.IsPlaying(controlledActor.GetAnimationClipName(WMActorStateDebug.Idle)))
				{
					controlledActor.Animation.Play(controlledActor.GetAnimationClipName(WMActorStateDebug.Idle));
				}
			}
			else if (controlledActor.State == WMActorStateDebug.Running && !controlledActor.Animation.IsPlaying(controlledActor.GetAnimationClipName(WMActorStateDebug.Running)))
			{
				controlledActor.Animation.Play(controlledActor.GetAnimationClipName(WMActorStateDebug.Running));
			}
		}
	}

	private void UpdateInputPlane()
	{
		WMActor controlledActor = this.ControlledActor;
		if (Input.GetKey(KeyCode.N) || Input.GetKey(KeyCode.JoystickButton0))
		{
			controlledActor.moveZ = 1f;
		}
		else if (Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.JoystickButton1))
		{
			controlledActor.moveZ = -1f;
		}
		else
		{
			controlledActor.moveZ = 0f;
		}
	}

	public void UpdateHuman()
	{
		WMActor controlledActor = this.ControlledActor;
		if (!controlledActor.isControllable)
		{
			Input.ResetInputAxes();
		}
		this.UpdateSmoothedMovementDirectionHuman();
		Vector3 vector = controlledActor.moveDirection * controlledActor.moveSpeed;
		vector *= Time.deltaTime;
		if (FF9StateSystem.World.IsBeeScene && controlledActor.Animation)
		{
			if (controlledActor.State == WMActorStateDebug.Idle)
			{
				if (!controlledActor.Animation.IsPlaying(controlledActor.GetAnimationClipName(WMActorStateDebug.Idle)))
				{
					controlledActor.Animation.Play(controlledActor.GetAnimationClipName(WMActorStateDebug.Idle));
				}
			}
			else if (controlledActor.State == WMActorStateDebug.Running)
			{
				controlledActor.Animation[controlledActor.GetAnimationClipName(WMActorStateDebug.Running)].speed = controlledActor.AnimationSpeed;
				if (!controlledActor.Animation.IsPlaying(controlledActor.GetAnimationClipName(WMActorStateDebug.Running)))
				{
					controlledActor.Animation.Play(controlledActor.GetAnimationClipName(WMActorStateDebug.Running));
				}
			}
			else if (controlledActor.State == WMActorStateDebug.Sprint)
			{
				controlledActor.Animation[controlledActor.GetAnimationClipName(WMActorStateDebug.Running)].speed = 1f;
				if (!controlledActor.Animation.IsPlaying(controlledActor.GetAnimationClipName(WMActorStateDebug.Running)))
				{
					controlledActor.Animation.Play(controlledActor.GetAnimationClipName(WMActorStateDebug.Running));
				}
			}
		}
		if (!this.IsGrounded())
		{
			Vector3 vector2 = vector;
			vector2.y = 0f;
			if (vector2.sqrMagnitude > 0.001f)
			{
			}
		}
		if (this.IsGrounded())
		{
			controlledActor.lastGroundedTime = Time.time;
		}
	}

	public Boolean IsGrounded()
	{
		return true;
	}

	private new Int32 transform;

	private Transform cameraTransform;
}
