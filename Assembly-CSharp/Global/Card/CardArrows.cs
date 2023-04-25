using Memoria;
using System;
using UnityEngine;

public class CardArrows : MonoBehaviour
{
	public Int32 Arrow
	{
		get
		{
			return _arrow;
		}
		set
		{
			_arrow = value;
			if (_arrow > 255)
			{
				_arrow = 255;
			}
			if (_arrow < 0)
			{
				_arrow = 0;
			}
			for (Int32 i = 0; i < 8; i++)
			{
                ui[i].gameObject.SetActive((Configuration.TetraMaster.TripleTriad < 2) ? ((_arrow & 1 << i) > 0) : false); // HIDE ARROW
            }
        }
	}

	public Boolean Small
	{
		get
		{
			return _small;
		}
		set
		{
			_small = value;
			for (Int32 i = 0; i < (Int32)ui.Length; i++)
			{
				ui[i].Small = value;
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
