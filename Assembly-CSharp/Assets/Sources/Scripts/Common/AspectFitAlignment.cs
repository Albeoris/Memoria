using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Sources.Scripts.Common
{
	public class AspectFitAlignment
	{
		public AspectFitAlignment()
		{
			this.alignToLeftEntities = new List<GameObject>();
			this.alignToLeftOrigins = new List<Vector3>();
			this.alignToTopEntities = new List<GameObject>();
			this.alignToTopOrigins = new List<Vector3>();
			this.alignToRightEntities = new List<GameObject>();
			this.alignToRightOrigins = new List<Vector3>();
			this.alignToBottomEntities = new List<GameObject>();
			this.alignToBottomOrigins = new List<Vector3>();
		}

		public void SetAlignmentToLeft(GameObject entity)
		{
			this.alignToLeftEntities.Add(entity);
			this.alignToLeftOrigins.Add(entity.transform.localPosition);
		}

		public void SetAlignmentToTop(GameObject entity)
		{
			this.alignToTopEntities.Add(entity);
			this.alignToTopOrigins.Add(entity.transform.localPosition);
		}

		public void SetAlignmentToRight(GameObject entity)
		{
			this.alignToRightEntities.Add(entity);
			this.alignToRightOrigins.Add(entity.transform.localPosition);
		}

		public void SetAlignmentToBottom(GameObject entity)
		{
			this.alignToBottomEntities.Add(entity);
			this.alignToBottomOrigins.Add(entity.transform.localPosition);
		}

		public void UpdatePositionWithAlignment(AspectFit aspectFit)
		{
			if (this.alignToLeftEntities != null)
			{
				for (Int32 i = 0; i < this.alignToLeftEntities.Count; i++)
				{
					GameObject gameObject = this.alignToLeftEntities[i];
					Vector3 vector = this.alignToLeftOrigins[i];
					Vector3 localPosition = gameObject.transform.localPosition;
					localPosition.x = vector.x - aspectFit.origin.x;
					gameObject.transform.localPosition = localPosition;
				}
			}
			if (this.alignToRightEntities != null)
			{
				for (Int32 j = 0; j < this.alignToRightEntities.Count; j++)
				{
					GameObject gameObject2 = this.alignToRightEntities[j];
					Vector3 vector2 = this.alignToRightOrigins[j];
					Vector3 localPosition2 = gameObject2.transform.localPosition;
					localPosition2.x = vector2.x + aspectFit.origin.x;
					gameObject2.transform.localPosition = localPosition2;
				}
			}
			if (this.alignToTopEntities != null)
			{
				for (Int32 k = 0; k < this.alignToTopEntities.Count; k++)
				{
					GameObject gameObject3 = this.alignToTopEntities[k];
					Vector3 vector3 = this.alignToTopOrigins[k];
					Vector3 localPosition3 = gameObject3.transform.localPosition;
					localPosition3.y = vector3.y - aspectFit.origin.y;
					gameObject3.transform.localPosition = localPosition3;
				}
			}
			if (this.alignToBottomEntities != null)
			{
				for (Int32 l = 0; l < this.alignToBottomEntities.Count; l++)
				{
					GameObject gameObject4 = this.alignToBottomEntities[l];
					Vector3 vector4 = this.alignToBottomOrigins[l];
					Vector3 localPosition4 = gameObject4.transform.localPosition;
					localPosition4.y = vector4.y + aspectFit.origin.y;
					gameObject4.transform.localPosition = localPosition4;
				}
			}
		}

		private List<GameObject> alignToLeftEntities;

		private List<Vector3> alignToLeftOrigins;

		private List<GameObject> alignToTopEntities;

		private List<Vector3> alignToTopOrigins;

		private List<GameObject> alignToRightEntities;

		private List<Vector3> alignToRightOrigins;

		private List<GameObject> alignToBottomEntities;

		private List<Vector3> alignToBottomOrigins;
	}
}
