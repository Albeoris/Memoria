using System;
using UnityEngine;

public static class Math3D
{
	public static Single Fixed2Float(Int32 f)
	{
		return f / 65536f;
	}

	public static Int32 Float2Fixed(Single f)
	{
		return (Int32)(f * 65536f);
	}

	public static Single EulerAngleToNegative(Single euler)
	{
		return euler >= 180f ? euler - 360f : euler;
	}

	public static Vector3 ProjectPointOnLineSegment(Vector3 segA, Vector3 segB, Vector3 p)
	{
		Vector3 vecPA = p - segA;
		Vector3 projvec = Vector3.Project(vecPA, (segB - segA).normalized);
		return segA + projvec;
	}

	public static Boolean PointInsideTriangleTest(Vector3 point, Vector3 vA, Vector3 vB, Vector3 vC, Boolean largeBorder = false)
	{
		Vector3 AC = vC - vA;
		Vector3 AB = vB - vA;
		Vector3 AP = point - vA;
		Single ACsqr = Vector3.Dot(AC, AC);
		Single ACAB = Vector3.Dot(AC, AB);
		Single ACAP = Vector3.Dot(AC, AP);
		Single ABsqr = Vector3.Dot(AB, AB);
		Single ABAP = Vector3.Dot(AB, AP);
		Single d = 1f / (ACsqr * ABsqr - ACAB * ACAB);
		Single baryAB = (ABsqr * ACAP - ACAB * ABAP) * d;
		Single baryAC = (ACsqr * ABAP - ACAB * ACAP) * d;
		if (largeBorder)
			return baryAB >= 0f && baryAC >= 0f && baryAB + baryAC <= 1.000001f;
		return baryAB >= 0f && baryAC >= 0f && baryAB + baryAC < 1f;
	}

	public static Boolean PointInsideTriangleTestXZ(Vector3 point, Vector3 vA, Vector3 vB, Vector3 vC)
	{
		Vector3 planarFactor = new Vector3(1f, 0f, 1f);
		return Math3D.PointInsideTriangleTest2D(point, vA, vB, vC, planarFactor);
	}

	public static Boolean PointInsideTriangleTest2D(Vector3 point, Vector3 vA, Vector3 vB, Vector3 vC, Vector3 planarFactor)
	{
		point = Vector3.Scale(point, planarFactor);
		vA = Vector3.Scale(vA, planarFactor);
		vB = Vector3.Scale(vB, planarFactor);
		vC = Vector3.Scale(vC, planarFactor);
		return PointInsideTriangleTest(point, vA, vB, vC, true);
	}

	public static Vector3 CalculateBarycentricRatioXZ(Vector3 point, Vector3 vA, Vector3 vB, Vector3 vC)
	{
		point.y = 0f;
		vA.y = 0f;
		vB.y = 0f;
		vC.y = 0f;
		return Math3D.CalculateBarycentricRatio(point, vA, vB, vC);
	}

	public static Vector3 CalculateBarycentricRatio(Vector3 point, Vector3 vA, Vector3 vB, Vector3 vC)
	{
		Single areaABC = Vector3.Cross(vA - vB, vA - vC).magnitude;
		return CalculateBarycentric(point, vA, vB, vC) / areaABC;
	}

	public static Vector3 CalculateBarycentric(Vector3 point, Vector3 vA, Vector3 vB, Vector3 vC)
	{
		Vector3 PA = vA - point;
		Vector3 PB = vB - point;
		Vector3 PC = vC - point;
		Single areaPBC = Vector3.Cross(PB, PC).magnitude;
		Single areaPAC = Vector3.Cross(PC, PA).magnitude;
		Single areaPAB = Vector3.Cross(PA, PB).magnitude;
		return new Vector3(areaPBC, areaPAC, areaPAB);
	}

	public static Boolean FastLineSegmentIntersectionXZ(Vector3 a0, Vector3 a1, Vector3 b0, Vector3 b1)
	{
		Single num = a1.x - a0.x;
		Single num2 = a1.z - a0.z;
		Single num3 = b1.x - b0.x;
		Single num4 = b1.z - b0.z;
		Single num5 = num2 * num3 - num * num4;
		if (Mathf.Approximately(num5, 0f))
			return false;
		Single num6 = ((a0.x - b0.x) * num4 + (b0.z - a0.z) * num3) / num5;
		Single num7 = ((b0.x - a0.x) * num2 + (a0.z - b0.z) * num) / -num5;
		return num6 >= 0f && num6 <= 1f && num7 >= 0f && num7 <= 1f;
	}

	public static Single SqrDistanceToLine(Vector3 point, Vector3 segA, Vector3 segB)
	{
		return ClosestPointToLine(point, segA, segB).sqrMagnitude;
	}

	public static Single SqrDistanceToLineXZ(Vector3 point, Vector3 segA, Vector3 segB)
	{
		point.y = 0f;
		segA.y = 0f;
		segB.y = 0f;
		return SqrDistanceToLine(point, segA, segB);
	}

	public static Vector3 ClosestPointToLine(Vector3 point, Vector3 segA, Vector3 segB)
	{
		Single ABAP = Vector3.Dot(point - segA, segB - segA);
		Single BABP = Vector3.Dot(point - segB, segA - segB);
		Vector3 proj;
		if (ABAP <= 0f)
			proj = segA;
		else if (BABP <= 0f)
			proj = segB;
		else
			proj = segA + (segB - segA) * ABAP / (ABAP + BABP);
		return proj - point;
	}

	public static Single SqrDistanceToLineUnlimited(Vector3 point, Vector3 lineA, Vector3 lineB)
	{
		return ClosestPointToLineUnlimited(point, lineA, lineB).sqrMagnitude;
	}

	public static Vector3 ClosestPointToLineUnlimited(Vector3 point, Vector3 lineA, Vector3 lineB)
	{
		Single ABAP = Vector3.Dot(point - lineA, lineB - lineA);
		Single BABP = Vector3.Dot(point - lineB, lineA - lineB);
		Vector3 proj = lineA + (lineB - lineA) * ABAP / (ABAP + BABP);
		return proj - point;
	}

	public static Single SqrDistanceToLine(Vector3 point, Vector3 segA, Vector3 segB, out Vector3 vert, out Boolean isPerp)
	{
		Single ABAP = Vector3.Dot(point - segA, segB - segA);
		Single BABP = Vector3.Dot(point - segB, segA - segB);
		if (ABAP <= 0f)
		{
			vert = segA;
			isPerp = Mathf.Approximately(ABAP, 0f);
		}
		else if (BABP <= 0f)
		{
			vert = segB;
			isPerp = Mathf.Approximately(BABP, 0f);
		}
		else
		{
			vert = segA + (segB - segA) * ABAP / (ABAP + BABP);
			isPerp = true;
		}
		return (vert - point).sqrMagnitude;
	}

	public static Single SqrDistanceTwoVectorsXZ(Vector3 first, Vector3 second)
	{
		first.y = 0f;
		second.y = 0f;
		return (first - second).sqrMagnitude;
	}

	public static Single SqrDistanceTwoVectors(Vector3 first, Vector3 second)
	{
		return (first - second).sqrMagnitude;
	}

	public static Single DistanceTwoVectors(Vector3 first, Vector3 second)
	{
		return (first - second).magnitude;
	}

	public static Single DistanceTwoLinesOnY(Vector3 vec1S, Vector3 vec1E, Vector3 vec2S, Vector3 vec2E)
	{
		Single y;
		Single y2;
		if (vec1S.y < vec1E.y)
		{
			y = vec1S.y;
			y2 = vec1E.y;
		}
		else
		{
			y = vec1E.y;
			y2 = vec1S.y;
		}
		Single y3;
		Single y4;
		if (vec2S.y < vec2E.y)
		{
			y3 = vec2S.y;
			y4 = vec2E.y;
		}
		else
		{
			y3 = vec2E.y;
			y4 = vec2S.y;
		}
		return Mathf.Max(Mathf.Max(y, y3) - Mathf.Min(y2, y4), 0f);
	}

	public static Boolean nearlyEqual(Single a, Single b, Single epsilon)
	{
		Single absa = Mathf.Abs(a);
		Single absb = Mathf.Abs(b);
		Single diff = Mathf.Abs(a - b);
		if (a == b)
			return true;
		if (a == 0f || b == 0f || diff < Single.Epsilon)
			return diff < epsilon * Single.Epsilon;
		return diff / Mathf.Min(absa + absb, Single.MaxValue) < epsilon;
	}

	public static Single[] SolveMatrixEquation(Single[,] equationMatrix, Single[] rhs = null, Single[] initialSolution = null)
	{
		Int32 rowCount = equationMatrix.GetLength(0);
		Int32 colCount = equationMatrix.GetLength(1);
		Single[,] echelonForm = ComputeRowEchelonMatrix(equationMatrix, rhs);
		String str = "";
		for (Int32 i = 0; i < rowCount; i++)
			for (Int32 j = 0; j < colCount; j++)
				str += echelonForm[i, j] + (j + 1 < colCount ? ", " : "\n");
		Single[] result = new Single[colCount];
		if (initialSolution != null && initialSolution.Length != colCount)
			throw new RankException();
		if (rhs != null && rhs.Length != rowCount)
			throw new RankException();
		Int32 solIndex = colCount;
		for (Int32 row = rowCount - 1; row >= 0; row--)
		{
			Single val = rhs != null ? rhs[row] : 0f;
			Int32 nextSolIndex = row;
			while (nextSolIndex < solIndex && echelonForm[row, nextSolIndex] == 0f)
				nextSolIndex++;
			if (nextSolIndex >= solIndex)
				continue;
			solIndex--;
			while (nextSolIndex < solIndex)
			{
				result[solIndex] = initialSolution != null ? initialSolution[solIndex] : 1f;
				solIndex--;
			}
			String str2 = $"(rhs[{row}]";
			for (Int32 col = solIndex + 1; col < colCount; col++)
			{
				val -= result[col] * echelonForm[row, col];
				str2 += $" - Result[{col}] * echelonForm[{row}, {col}]";
			}
			result[solIndex] = val / echelonForm[row, solIndex];
		}
		while (solIndex > 0)
		{
			solIndex--;
			result[solIndex] = initialSolution != null ? initialSolution[solIndex] : 1f;
		}
		return result;
	}

	public static Single[,] ComputeRowEchelonMatrix(Single[,] matrix, Single[] rhs = null)
	{
		Int32 rowCount = matrix.GetLength(0);
		Int32 colCount = matrix.GetLength(1);
		if (rhs != null)
		{
			if (rhs.Length != rowCount)
				throw new RankException();
			Single[,] extendedMatrix = new Single[rowCount, colCount + 1];
			for (Int32 i = 0; i < rowCount; i++)
			{
				for (Int32 j = 0; j < colCount; j++)
					extendedMatrix[i, j] = matrix[i, j];
				extendedMatrix[i, colCount] = rhs[i];
			}
			matrix = extendedMatrix;
			rowCount = matrix.GetLength(0);
			colCount = matrix.GetLength(1);
		}
		Single[,] result = new Single[rowCount, colCount];
		Int32 row = 0;
		Int32 col = 0;
		for (Int32 i = 0; i < rowCount; i++)
			for (Int32 j = 0; j < colCount; j++)
				result[i, j] = matrix[i, j];
		while (row < rowCount && col < colCount)
		{
			Int32 pivotRow = 0;
			Single pivotValue = 0f;
			for (Int32 i = row; i < rowCount; i++)
			{
				if (Math.Abs(result[i, col]) > pivotValue)
				{
					pivotRow = i;
					pivotValue = Math.Abs(result[i, col]);
				}
			}
			if (pivotValue == 0f)
			{
				col++;
				continue;
			}
			SwapMatrixRows(result, row, pivotRow);
			for (Int32 i = row + 1; i < rowCount; i++)
			{
				Single factor = result[i, col] / result[row, col];
				result[i, col] = 0f;
				for (Int32 j = col + 1; j < colCount; j++)
					result[i, j] -= result[row, j] * factor;
			}
			row++;
			col++;
		}
		return result;
	}

	public static void SwapMatrixRows(Single[,] matrix, Int32 row1, Int32 row2)
	{
		Int32 colCount = matrix.GetLength(1);
		for (Int32 j = 0; j < colCount; j++)
		{
			Single tmp = matrix[row1, j];
			matrix[row1, j] = matrix[row2, j];
			matrix[row2, j] = tmp;
		}
	}

	public static void SwapMatrixColumns(Single[,] matrix, Int32 col1, Int32 col2)
	{
		Int32 rowCount = matrix.GetLength(0);
		for (Int32 i = 0; i < rowCount; i++)
		{
			Single tmp = matrix[i, col1];
			matrix[i, col1] = matrix[i, col2];
			matrix[i, col2] = tmp;
		}
	}
}
