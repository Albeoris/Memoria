using System;
using UnityEngine;

namespace Assets.Sources.Scripts.Common
{
    public class AspectFitHeight : AspectFit
    {
        public override void setAspectFit()
        {
            this.origin = Vector3.zero;
            Single canvasAspect = this.canvasWidth / this.canvasHeight;
            Single screenAspect = (Single)Screen.width / (Single)Screen.height;
            this.aspectCamera.orthographicSize = this.canvasHeight;
            Single cameraHeight = this.aspectCamera.orthographicSize * 2f;
            Single cameraWidth = cameraHeight * screenAspect;
            if (screenAspect > canvasAspect)
            {
                Single adjustedWidth = cameraHeight * canvasAspect;
                this.origin.x = (cameraWidth - adjustedWidth) * 0.5f;
            }
            else
            {
                Single adjustedHeight = cameraWidth / canvasAspect;
                this.origin.y = (cameraHeight - adjustedHeight) * 0.5f * -1f;
            }
        }
    }
}
