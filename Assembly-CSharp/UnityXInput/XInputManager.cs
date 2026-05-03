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
            playerIndexSet = false;
            for (Int32 i = 0; i <= 3; i++)
            {
                if (XInputDotNetPure.GamePad.GetState(playerIndex).IsConnected)
                {
                    UnityEngine.Input.ResetInputAxes();
                    playerIndex = (PlayerIndex)i;
                    playerIndexSet = true;
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

                // Pause the game if a controller has been disconnected
                if (!UIManager.Instance.IsPause)
                    UIManager.Instance.GetSceneFromState(UIManager.Instance.State).OnKeyPause(null);
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
