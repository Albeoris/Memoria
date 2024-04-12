using System;
using UnityEngine;
using XInputDotNetPure;

namespace UnityXInput
{
	public class XInputManager : PersistenSingleton<XInputManager>
	{
		public GamePadState CurrentState
		{
			get
			{
				return this.state;
			}
		}

		public GamePadState PreviousState
		{
			get
			{
				return this.prevState;
			}
		}

		public PlayerIndex PlayerIndex
		{
			get
			{
				return this.playerIndex;
			}
		}

		public void SetVibration(Single leftMotor, Single rightMotor)
		{
			if (this.state.IsConnected)
			{
				XInputDotNetPure.GamePad.SetVibration(this.playerIndex, leftMotor, rightMotor);
			}
		}

		private void CheckConnection()
		{
			Int32 num = 0;
			Int32 num2 = 3;
			this.playerIndexSet = false;
			for (Int32 i = num; i <= num2; i++)
			{
				PlayerIndex playerIndex = (PlayerIndex)i;
				if (XInputDotNetPure.GamePad.GetState(playerIndex).IsConnected)
				{
					String arg = UnityEngine.Input.GetJoystickNames().Length <= 0 ? "Unknown Device" : UnityEngine.Input.GetJoystickNames()[0];
					global::Debug.Log(String.Format("GamePad found {0} => {1}", playerIndex, arg));
					UnityEngine.Input.ResetInputAxes();
					this.playerIndex = playerIndex;
					this.playerIndexSet = true;
					break;
				}
			}
		}

		private void Refresh()
		{
			if (this.resetAxesTime > 0f)
			{
				this.resetAxesTime -= Time.deltaTime;
				UnityEngine.Input.ResetInputAxes();
			}
			if (this.waitTime > 0f)
			{
				this.waitTime -= Time.deltaTime;
				return;
			}
			if (!this.playerIndexSet || !this.prevState.IsConnected)
			{
				this.CheckConnection();
			}
			this.prevState = this.state;
			this.state = XInputDotNetPure.GamePad.GetState(this.playerIndex);
			if (!this.prevState.IsConnected && !this.state.IsConnected)
			{
				if (this.waitTime <= 0f)
				{
					this.waitTime = 1f;
				}
			}
			else if (!this.state.IsConnected && this.prevState.IsConnected)
			{
				this.resetAxesTime = 1f;
				global::Debug.Log("Start reset input axes");
			}
		}

		private void Update()
		{
			this.Refresh();
		}

		private const Single ConnectionDelay = 1f;

		[SerializeField]
		private Boolean playerIndexSet;

		[SerializeField]
		private PlayerIndex playerIndex;

		private GamePadState state;

		private GamePadState prevState;

		private Single waitTime = 1f;

		private Single resetAxesTime = 1f;
	}
}
