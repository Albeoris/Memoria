using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteDisplay : MonoBehaviour
{
	public Int32 ID
	{
		get => _id;
		set
		{
			_id = value;
			if (sheet != null)
			{
				QuadMistResourceManager instance = QuadMistResourceManager.Instance;
				GetComponent<SpriteRenderer>().sprite = instance.GetResource(sheet.name, _id).Sprite;
			}
			else
			{
				_id = 0;
			}
		}
	}

	private void Start()
	{
		ID = _id;
	}

	[SerializeField]
	public SpriteSheet sheet;

	[SerializeField]
	public Int32 _id;
}
