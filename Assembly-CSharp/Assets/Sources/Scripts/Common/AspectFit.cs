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
            Single canvasAspect = this.canvasWidth / this.canvasHeight;
            Single screenAspect = (Single)Screen.width / (Single)Screen.height;
            if (screenAspect < canvasAspect)
            {
                this.aspectCamera.orthographicSize = canvasAspect / screenAspect * this.canvasHeight;
            }
            else
            {
                this.aspectCamera.orthographicSize = this.canvasHeight;
            }
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
