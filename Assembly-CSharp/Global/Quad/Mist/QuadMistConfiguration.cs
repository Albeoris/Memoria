using System;
using UnityEngine;

public class QuadMistConfiguration : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		QuadMistConfiguration.s_dummyRatioX = this.dummyRatioX;
		QuadMistConfiguration.s_dummyRatioY = this.dummyRatioY;
		QuadMistConfiguration.s_dummyOffsetX = this.dummyOffsetX;
		QuadMistConfiguration.s_dummyOffsetY = this.dummyOffsetY;
		QuadMistConfiguration.s_initPositionX = this.initPositionX;
		QuadMistConfiguration.s_initPositionY = this.initPositionY;
		QuadMistConfiguration.s_destPositionX = this.destPositionX;
		QuadMistConfiguration.s_destPositionY = this.destPositionY;
		QuadMistConfiguration.s_pivotCode = this.pivotCode;
		QuadMistConfiguration.s_cardName = this.cardName;
	}

	public Single dummyRatioX;

	public Single dummyRatioY;

	public Single dummyOffsetX;

	public Single dummyOffsetY;

	public static Single s_dummyRatioX;

	public static Single s_dummyRatioY;

	public static Single s_dummyOffsetX;

	public static Single s_dummyOffsetY;

	public Single initPositionX;

	public Single initPositionY;

	public Single destPositionX;

	public Single destPositionY;

	public Int32 pivotCode;

	public static Single s_initPositionX;

	public static Single s_initPositionY;

	public static Single s_destPositionX;

	public static Single s_destPositionY;

	public static Int32 s_pivotCode;

	public String cardName;

	public static String s_cardName;
}
