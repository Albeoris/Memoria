using System;
using UnityEngine;

public class CardArrows : MonoBehaviour
{
	public Int32 Arrow
	{
		get
		{
			return this._arrow;
		}
		set
		{
			this._arrow = value;
			if (this._arrow > 255)
			{
				this._arrow = 255;
			}
			if (this._arrow < 0)
			{
				this._arrow = 0;
			}
			for (Int32 i = 0; i < 8; i++)
			{
				this.ui[i].gameObject.SetActive((this._arrow & 1 << i) > 0);
			}
		}
	}

	public Boolean Small
	{
		get
		{
			return this._small;
		}
		set
		{
			this._small = value;
			for (Int32 i = 0; i < (Int32)this.ui.Length; i++)
			{
				this.ui[i].Small = value;
			}
		}
	}

	public CardArrow[] ui;

	[HideInInspector]
	[SerializeField]
	private Boolean _small;

	[HideInInspector]
	[SerializeField]
	private Int32 _arrow;
}
