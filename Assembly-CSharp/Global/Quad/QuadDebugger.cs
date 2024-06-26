using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadDebugger : PersistenSingleton<QuadDebugger>
{
	public Quad Target
	{
		get
		{
			return this.target;
		}
		set
		{
			this.target = value;
			if (this.target != null)
			{
				this.sid = (Int32)this.target.sid;
				this.CalculateHotFixParameters(this.target);
			}
		}
	}

	public Single OffsetY
	{
		get
		{
			return this.offsetY;
		}
		set
		{
			this.offsetY = value;
		}
	}

	private void CalculateHotFixParameters(Quad quad)
	{
		List<Single> list = new List<Single>();
		List<Single> list2 = new List<Single>();
		QuadPos[] q = quad.q;
		for (Int32 i = 0; i < (Int32)q.Length; i++)
		{
			QuadPos quadPos = q[i];
			if (quadPos.Vector3Val.x != 0f || quadPos.Vector3Val.z != 0f)
			{
				list.Add(quadPos.Vector3Val.x);
				list2.Add(quadPos.Vector3Val.z);
			}
		}
		Single num = list.Max();
		Single num2 = list.Min();
		Single num3 = list2.Max();
		Single num4 = list2.Min();
		Vector3 vector = new Vector3(Mathf.Abs((num - num2) / 2f), 0f, Mathf.Abs((num3 - num4) / 2f));
		this.hotFixCenter = new Vector3(list.Average(), this.offsetY, list2.Average());
		this.hotFixRadiant = ((vector.x <= vector.z) ? vector.x : vector.z);
	}

	public QuadDebugger.DrawMode drawMode;

	private Quad target;

	private Single offsetY;

	[SerializeField]
	private Int32 sid;

	[SerializeField]
	private Vector3 hotFixCenter;

	[SerializeField]
	private Single hotFixRadiant;

	public enum DrawMode
	{
		None,
		Alternative,
		HotFix,
		Both
	}
}
