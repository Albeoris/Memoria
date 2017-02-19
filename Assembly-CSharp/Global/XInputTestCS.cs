using System;
using Assets.Scripts.Common;
using UnityEngine;
using XInputDotNetPure;
using Object = System.Object;

public class XInputTestCS : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		this.prevState = this.state;
		this.state = global::GamePad.GetState(this.playerIndex);
		if (this.prevState.Buttons.A == ButtonState.Released && this.state.Buttons.A == ButtonState.Pressed)
		{
			base.GetComponent<Renderer>().material.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
		}
		if (this.prevState.Buttons.A == ButtonState.Pressed && this.state.Buttons.A == ButtonState.Released)
		{
			base.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
		}
		global::GamePad.SetVibration(this.playerIndex, this.state.Triggers.Left, this.state.Triggers.Right);
		base.transform.localRotation *= Quaternion.Euler(0f, this.state.ThumbSticks.Left.X * 25f * Time.deltaTime, 0f);
		if (this.prevState.Buttons.Start == ButtonState.Released && this.state.Buttons.Start == ButtonState.Pressed)
		{
			SceneDirector.Replace("Bundle", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
	}

	private void OnGUI()
	{
		String text = "Use left stick to turn the cube, hold A to change color\n";
		text += String.Format("IsConnected {0} Packet #{1}\n", this.state.IsConnected, this.state.PacketNumber);
		text += String.Format("\tTriggers {0} {1}\n", this.state.Triggers.Left, this.state.Triggers.Right);
		text += String.Format("\tD-Pad {0} {1} {2} {3}\n", new Object[]
		{
			this.state.DPad.Up,
			this.state.DPad.Right,
			this.state.DPad.Down,
			this.state.DPad.Left
		});
		text += String.Format("\tButtons Start {0} Back {1} Guide {2}\n", this.state.Buttons.Start, this.state.Buttons.Back, this.state.Buttons.Guide);
		text += String.Format("\tButtons LeftStick {0} RightStick {1} LeftShoulder {2} RightShoulder {3}\n", new Object[]
		{
			this.state.Buttons.LeftStick,
			this.state.Buttons.RightStick,
			this.state.Buttons.LeftShoulder,
			this.state.Buttons.RightShoulder
		});
		text += String.Format("\tButtons A {0} B {1} X {2} Y {3}\n", new Object[]
		{
			this.state.Buttons.A,
			this.state.Buttons.B,
			this.state.Buttons.X,
			this.state.Buttons.Y
		});
		text += String.Format("\tSticks Left {0} {1} Right {2} {3}\n", new Object[]
		{
			this.state.ThumbSticks.Left.X,
			this.state.ThumbSticks.Left.Y,
			this.state.ThumbSticks.Right.X,
			this.state.ThumbSticks.Right.Y
		});
		text = text + "\tHono Axis Raw :" + PersistenSingleton<HonoInputManager>.Instance.GetAxis();
		text += (((Int32)Input.GetJoystickNames().Length <= 0) ? String.Empty : ("\nJoyStick Name : " + Input.GetJoystickNames()[0]));
		GUI.Label(new Rect(0f, 0f, (Single)Screen.width, (Single)Screen.height), text);
	}

	private Boolean playerIndexSet;

	private PlayerIndex playerIndex;

	private GamePadState state;

	private GamePadState prevState;
}
