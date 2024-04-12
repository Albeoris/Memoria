using Assets.Sources.Scripts.Common;
using System;
using UnityEngine;

public class QuadMistCameraController : MonoBehaviour
{
	private void Start()
	{
		aspectFit = new AspectFit(1.6001482f, 1.12f, CanvasCamera);
	}

	private void Update()
	{
		if (aspectFit != null)
		{
			aspectFit.setAspectFit();
		}
	}

	public const Single canvasHeight = 1.12f;

	public const Single canvasWidth = 1.6001482f;

	public Camera CanvasCamera;

	private AspectFit aspectFit;
}
