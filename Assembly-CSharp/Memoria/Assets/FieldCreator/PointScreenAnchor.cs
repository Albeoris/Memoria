using System;
using System.Linq;
using UnityEngine;

namespace Memoria.Assets
{
	public class PointScreenAnchor
	{
		private BGCAM_DEF bgCamera;
		private BGI_DEF bgi;

		public readonly Vector3[] WorldPoint = new Vector3[5];
		public readonly Vector2[] ScreenPoint = new Vector2[5];

		public Boolean CanBeUsed => bgCamera != null && bgi.vertexList.Count > 0;

		public void Init(BGCAM_DEF camera, BGI_DEF walkmesh)
		{
			bgCamera = camera;
			bgi = walkmesh;
		}

		public Vector2 GetWorldPointScreenPos(Int32 index)
		{
			Single viewDistance = bgCamera.GetViewDistance();
			Vector2 centerOffset = bgCamera.GetCenterOffset();
			Vector2 cameraOffset = new Vector2(
				(bgCamera.w / 2) + centerOffset.x - FieldMap.HalfFieldWidth,
				-(bgCamera.h / 2) - centerOffset.y + FieldMap.HalfFieldHeight);
			Matrix4x4 matrix = bgCamera.GetMatrixRT();
			return PSX.CalculateGTE_RTPT(WorldPoint[index], Matrix4x4.identity, matrix, viewDistance, cameraOffset);
		}

		public Vector3 GetMouseScreenPoint()
		{
			PSXCameraAspect psxcameraAspect = UnityEngine.Object.FindObjectOfType<PSXCameraAspect>();
			return psxcameraAspect.GetLocalMousePosRelative();
		}

		public Vector3 GetClosestWalkmeshPoint(Vector3 screenPos)
		{
			screenPos.z = 0f;
			Single minSqrDist = Single.MaxValue;
			Vector3 point = Vector3.zero;
			Vector2 centerOffset = bgCamera.GetCenterOffset();
			Vector2 cameraOffset = new Vector2(
				(bgCamera.w / 2) + centerOffset.x - FieldMap.HalfFieldWidth,
				-(bgCamera.h / 2) - centerOffset.y + FieldMap.HalfFieldHeight);
			for (Int32 i = 0; i < bgi.floorList.Count; i++)
			{
				BGI_FLOOR_DEF floor = bgi.floorList[i];
				for (Int32 j = 0; j < floor.triNdxList.Count; j++)
				{
					BGI_TRI_DEF tri = bgi.triList[floor.triNdxList[j]];
					for (Int32 k = 0; k < 3; k++)
					{
						Vector3 vert = bgi.orgPos.ToVector3() + floor.orgPos.ToVector3() + bgi.vertexList[tri.vertexNdx[k]].ToVector3();
						vert.y *= -1f;
						Vector3 projVert = PSX.CalculateGTE_RTPT(vert, Matrix4x4.identity, bgCamera.GetMatrixRT(), bgCamera.GetViewDistance(), cameraOffset);
						Single sqrDist = (projVert - screenPos).sqrMagnitude;
						if (sqrDist < minSqrDist)
						{
							point = vert;
							minSqrDist = sqrDist;
						}
					}
				}
			}
			return point;
		}

		public void PerformAnchorOnCamera()
		{
			// Solve equations:
			// PSX.CalculateGTE_RTPT(WorldPoint[i], Matrix4x4.identity, MATRIX, viewDistance, cameraOffset) == ScreenPoint[i]
			// The unknowns are the MATRIX coefficients
			// These are equivalent to:
			// (m00 * WorldPoint[i].x - m01 * WorldPoint[i].y + m02 * WorldPoint[i].z + m03) * viewDistance + (m20 * WorldPoint[i].x - m21 * WorldPoint[i].y + m22 * WorldPoint[i].z + m23) * (cameraOffset.x - ScreenPoint[i].x) == 0
			// -(m10 * WorldPoint[i].x - m11 * WorldPoint[i].y + m12 * WorldPoint[i].z + m13) * viewDistance + (m20 * WorldPoint[i].x - m21 * WorldPoint[i].y + m22 * WorldPoint[i].z + m23) * (cameraOffset.y - ScreenPoint[i].y) == 0
			Single viewDistance = bgCamera.GetViewDistance();
			Vector2 centerOffset = bgCamera.GetCenterOffset();
			Vector2 cameraOffset = new Vector2(
				(bgCamera.w / 2) + centerOffset.x - FieldMap.HalfFieldWidth,
				-(bgCamera.h / 2) - centerOffset.y + FieldMap.HalfFieldHeight);
			Vector2[] targetOffset = ScreenPoint.Select(p => cameraOffset - p).ToArray();
			Int32[] reOrder = new Int32[] { 11, 10, 9, 4, 8, 3, 1, 5, 7, 0, 2, 6 };
			Single[,] equationMatrix = new Single[2 * WorldPoint.Length, 12];
			for (Int32 i = 0; i < WorldPoint.Length; i++)
			{
				// Order the columns to guide coefficient dependancies (and thus which "initialSolutions" coefficients will be used in priority)
				Int32 index = 2 * i;
				equationMatrix[index, reOrder[0]] = viewDistance * WorldPoint[i].x;			// m00
				equationMatrix[index, reOrder[1]] = -viewDistance * WorldPoint[i].y;		// m01
				equationMatrix[index, reOrder[2]] = viewDistance * WorldPoint[i].z;			// m02
				equationMatrix[index, reOrder[3]] = viewDistance;							// m03
				equationMatrix[index, reOrder[4]] = 0f;										// m10
				equationMatrix[index, reOrder[5]] = 0f;										// m11
				equationMatrix[index, reOrder[6]] = 0f;										// m12
				equationMatrix[index, reOrder[7]] = 0f;										// m13
				equationMatrix[index, reOrder[8]] = targetOffset[i].x * WorldPoint[i].x;	// m20
				equationMatrix[index, reOrder[9]] = -targetOffset[i].x * WorldPoint[i].y;	// m21
				equationMatrix[index, reOrder[10]] = targetOffset[i].x * WorldPoint[i].z;	// m22
				equationMatrix[index, reOrder[11]] = targetOffset[i].x;						// m23
				index++;
				equationMatrix[index, reOrder[0]] = 0f;
				equationMatrix[index, reOrder[1]] = 0f;
				equationMatrix[index, reOrder[2]] = 0f;
				equationMatrix[index, reOrder[3]] = 0f;
				equationMatrix[index, reOrder[4]] = -viewDistance * WorldPoint[i].x;
				equationMatrix[index, reOrder[5]] = viewDistance * WorldPoint[i].y;
				equationMatrix[index, reOrder[6]] = -viewDistance * WorldPoint[i].z;
				equationMatrix[index, reOrder[7]] = -viewDistance;
				equationMatrix[index, reOrder[8]] = targetOffset[i].y * WorldPoint[i].x;
				equationMatrix[index, reOrder[9]] = -targetOffset[i].y * WorldPoint[i].y;
				equationMatrix[index, reOrder[10]] = targetOffset[i].y * WorldPoint[i].z;
				equationMatrix[index, reOrder[11]] = targetOffset[i].y;
			}
			Single[] initialSolutions = new Single[12] { 1f, 0f, 0f, 0f, 0f, 1f, 0f, 1000f, 0f, 0f, 1f, 4000f };
			Single[] initialSolutionsReorder = new Single[12];
			for (Int32 i = 0; i < 12; i++)
				initialSolutionsReorder[reOrder[i]] = initialSolutions[i];
			// Linear solve
			Single[] matrixCoef = Math3D.SolveMatrixEquation(equationMatrix, null, initialSolutionsReorder);
			Matrix4x4 newCameraMatrix = Matrix4x4.identity;
			Int32 coefIndex = 0;
			for (Int32 i = 0; i < 3; i++)
				for (Int32 j = 0; j < 4; j++)
					newCameraMatrix[i, j] = matrixCoef[reOrder[coefIndex++]];
			Memoria.Prime.Log.Message($"[MATRIX] MatrixRT\n{newCameraMatrix}");
			bgCamera.SetMatrixRT(newCameraMatrix);
		}
	}
}
