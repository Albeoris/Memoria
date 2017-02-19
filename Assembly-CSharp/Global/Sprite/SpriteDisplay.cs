using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteDisplay : MonoBehaviour
{
	public Int32 ID
	{
		get
		{
			return this._id;
		}
		set
		{
			this._id = value;
			if (this.sheet != (UnityEngine.Object)null)
			{
				QuadMistResourceManager instance = QuadMistResourceManager.Instance;
				base.GetComponent<SpriteRenderer>().sprite = instance.GetResource(this.sheet.name, this._id).Sprite;
			}
			else
			{
				this._id = 0;
			}
		}
	}

	private void Start()
	{
		this.ID = this._id;
	}

	[SerializeField]
	private SpriteSheet sheet;

	[SerializeField]
	public Int32 _id;
}
