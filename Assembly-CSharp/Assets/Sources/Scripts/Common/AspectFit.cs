using System;
using UnityEngine;

namespace Assets.Sources.Scripts.Common
{
	public class AspectFit
	{
		public AspectFit()
		{
		}

		public AspectFit(Single canvasWidth, Single canvasHeight, Camera aspectCamera)
		{
			this.canvasWidth = canvasWidth;
			this.canvasHeight = canvasHeight;
			this.aspectCamera = aspectCamera;
		}

		public virtual void setAspectFit()
		{
			this.origin = Vector3.zero;
			Single num = this.canvasWidth / this.canvasHeight;
			Single num2 = (Single)Screen.width / (Single)Screen.height;
			if (num2 < num)
			{
				this.aspectCamera.orthographicSize = num / num2 * this.canvasHeight;
			}
			else
			{
				this.aspectCamera.orthographicSize = this.canvasHeight;
			}
			Single num3 = this.aspectCamera.orthographicSize * 2f;
			Single num4 = num3 * num2;
			if (num2 > num)
			{
				Single num5 = num3 * num;
				this.origin.x = (num4 - num5) * 0.5f;
			}
			else
			{
				Single num6 = num4 / num;
				this.origin.y = (num3 - num6) * 0.5f * -1f;
			}
		}

		public Vector3 getMouseLocalPosition()
		{
			return this.getMouseOrthographicPosition() - this.origin;
		}

		public Vector3 getMouseOrthographicPosition()
		{
			Single orthographicSize = this.aspectCamera.orthographicSize;
			Single num = orthographicSize * 2f;
			Single d = num / (Single)Screen.height;
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = 0f;
			Vector3 result = mousePosition * d;
			result.y -= num;
			result.z = 0f;
			return result;
		}

		public Single canvasWidth;

		public Single canvasHeight;

		public Camera aspectCamera;

		public Vector3 origin;
	}
}
