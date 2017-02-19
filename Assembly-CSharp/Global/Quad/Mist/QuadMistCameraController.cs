using System;
using Assets.Sources.Scripts.Common;
using UnityEngine;

public class QuadMistCameraController : MonoBehaviour
{
	private void Start()
	{
		this.aspectFit = new AspectFit(1.6001482f, 1.12f, this.CanvasCamera);
	}

	private void Update()
	{
		if (this.aspectFit != null)
		{
			this.aspectFit.setAspectFit();
		}
	}

	public const Single canvasHeight = 1.12f;

	public const Single canvasWidth = 1.6001482f;

	public Camera CanvasCamera;

	private AspectFit aspectFit;
}
