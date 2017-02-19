using System;
using UnityEngine;

namespace Assets.Sources.Scripts.Common
{
	public class AspectFitHeight : AspectFit
	{
		public override void setAspectFit()
		{
			this.origin = Vector3.zero;
			Single num = this.canvasWidth / this.canvasHeight;
			Single num2 = (Single)Screen.width / (Single)Screen.height;
			this.aspectCamera.orthographicSize = this.canvasHeight;
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
	}
}
