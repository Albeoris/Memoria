using System;
using UnityEngine;

public static class Math3D
{
	public static Single Fixed2Float(Int32 f)
	{
		return (Single)f / 65536f;
	}

	public static Int32 Float2Fixed(Single f)
	{
		return (Int32)(f * 65536f);
	}

	public static Single EulerAngleToNegative(Single euler)
	{
		if (euler >= 180f)
		{
			euler -= 360f;
		}
		return euler;
	}

	public static Vector3 ProjectPointOnLineSegment(Vector3 v0, Vector3 v1, Vector3 p)
	{
		Vector3 vector = p - v0;
		Vector3 b = Vector3.Project(vector, (v1 - v0).normalized);
		return v0 + b;
	}

	public static Boolean PointInsideTriangleTest(Vector3 point, Vector3 vA, Vector3 vB, Vector3 vC)
	{
		Vector3 vector = vC - vA;
		Vector3 vector2 = vB - vA;
		Vector3 rhs = point - vA;
		Single num = Vector3.Dot(vector, vector);
		Single num2 = Vector3.Dot(vector, vector2);
		Single num3 = Vector3.Dot(vector, rhs);
		Single num4 = Vector3.Dot(vector2, vector2);
		Single num5 = Vector3.Dot(vector2, rhs);
		Single num6 = 1f / (num * num4 - num2 * num2);
		Single num7 = (num4 * num3 - num2 * num5) * num6;
		Single num8 = (num * num5 - num2 * num3) * num6;
		return num7 >= 0f && num8 >= 0f && num7 + num8 < 1f;
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
		Vector3 vector = vC - vA;
		Vector3 vector2 = vB - vA;
		Vector3 rhs = point - vA;
		Single num = Vector3.Dot(vector, vector);
		Single num2 = Vector3.Dot(vector, vector2);
		Single num3 = Vector3.Dot(vector, rhs);
		Single num4 = Vector3.Dot(vector2, vector2);
		Single num5 = Vector3.Dot(vector2, rhs);
		Single num6 = 1f / (num * num4 - num2 * num2);
		Single num7 = (num4 * num3 - num2 * num5) * num6;
		Single num8 = (num * num5 - num2 * num3) * num6;
		return num7 >= 0f && num8 >= 0f && num7 + num8 <= 1.000001f;
	}

	public static Vector3 CalculateBarycentricRatioXZ(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
	{
		point.y = 0f;
		v0.y = 0f;
		v1.y = 0f;
		v2.y = 0f;
		return Math3D.CalculateBarycentricRatio(point, v0, v1, v2);
	}

	public static Vector3 CalculateBarycentricRatio(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
	{
		Vector3 vector = v0 - point;
		Vector3 vector2 = v1 - point;
		Vector3 vector3 = v2 - point;
		Single magnitude = Vector3.Cross(v0 - v1, v0 - v2).magnitude;
		Single x = Vector3.Cross(vector2, vector3).magnitude / magnitude;
		Single y = Vector3.Cross(vector3, vector).magnitude / magnitude;
		Single z = Vector3.Cross(vector, vector2).magnitude / magnitude;
		Vector3 result = new Vector3(x, y, z);
		return result;
	}

	public static Vector3 CalculateBarycentric(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
	{
		Vector3 vector = v0 - point;
		Vector3 vector2 = v1 - point;
		Vector3 vector3 = v2 - point;
		Single magnitude = Vector3.Cross(vector2, vector3).magnitude;
		Single magnitude2 = Vector3.Cross(vector3, vector).magnitude;
		Single magnitude3 = Vector3.Cross(vector, vector2).magnitude;
		Vector3 result = new Vector3(magnitude, magnitude2, magnitude3);
		return result;
	}

	public static Boolean FastLineSegmentIntersectionXZ(Vector3 a0, Vector3 a1, Vector3 b0, Vector3 b1)
	{
		Single num = a1.x - a0.x;
		Single num2 = a1.z - a0.z;
		Single num3 = b1.x - b0.x;
		Single num4 = b1.z - b0.z;
		Single num5 = num2 * num3 - num * num4;
		if (Mathf.Approximately(num5, 0f))
		{
			return false;
		}
		Single num6 = ((a0.x - b0.x) * num4 + (b0.z - a0.z) * num3) / num5;
		Single num7 = ((b0.x - a0.x) * num2 + (a0.z - b0.z) * num) / -num5;
		return num6 >= 0f && num6 <= 1f && num7 >= 0f && num7 <= 1f;
	}

	public static Single SqrDistanceToLine(Vector3 p, Vector3 a, Vector3 b)
	{
		Single num = Vector3.Dot(p - a, b - a);
		Single num2 = Vector3.Dot(p - b, a - b);
		Vector3 a2 = Vector3.zero;
		if (num <= 0f)
		{
			a2 = a;
		}
		else if (num2 <= 0f)
		{
			a2 = b;
		}
		else
		{
			Single d = num + num2;
			a2 = a + (b - a) * num / d;
		}
		return (a2 - p).sqrMagnitude;
	}

	public static Single SqrDistanceToLineXZ(Vector3 p, Vector3 a, Vector3 b)
	{
		p.y = 0f;
		a.y = 0f;
		b.y = 0f;
		Single num = Vector3.Dot(p - a, b - a);
		Single num2 = Vector3.Dot(p - b, a - b);
		Vector3 a2 = Vector3.zero;
		if (num <= 0f)
		{
			a2 = a;
		}
		else if (num2 <= 0f)
		{
			a2 = b;
		}
		else
		{
			Single d = num + num2;
			a2 = a + (b - a) * num / d;
		}
		return (a2 - p).sqrMagnitude;
	}

	public static Vector3 ClosestPointToLine(Vector3 p, Vector3 a, Vector3 b)
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		Single num = Vector3.Dot(p - a, b - a);
		Single num2 = Vector3.Dot(p - b, a - b);
		Vector3 a2 = Vector3.zero;
		if (num <= 0f)
		{
			a2 = a;
		}
		else if (num2 <= 0f)
		{
			a2 = b;
		}
		else
		{
			a2 = a + (b - a) * num / (num + num2);
		}
		result = a2 - p;
		return result;
	}

	public static Single SqrDistanceToLine(Vector3 p, Vector3 a, Vector3 b, out Vector3 vert, out Boolean isPerp)
	{
		Single num = Vector3.Dot(p - a, b - a);
		Single num2 = Vector3.Dot(p - b, a - b);
		Vector3 vector = Vector3.zero;
		Single num3;
		if (num <= 0f)
		{
			vector = a;
			if (Mathf.Approximately(num, 0f))
			{
				isPerp = true;
			}
			else
			{
				isPerp = false;
			}
		}
		else if (num2 <= 0f)
		{
			vector = b;
			if (Mathf.Approximately(num2, 0f))
			{
				isPerp = true;
			}
			else
			{
				isPerp = false;
			}
		}
		else
		{
			num3 = num + num2;
			vector = a + (b - a) * num / num3;
			isPerp = true;
		}
		num3 = (vector - p).sqrMagnitude;
		vert = vector;
		return num3;
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
		Single num = Mathf.Abs(a);
		Single num2 = Mathf.Abs(b);
		Single num3 = Mathf.Abs(a - b);
		if (a == b)
		{
			return true;
		}
		if (a == 0f || b == 0f || num3 < 1.401298E-45f)
		{
			return num3 < epsilon * Single.Epsilon;
		}
		return num3 / Mathf.Min(num + num2, Single.MaxValue) < epsilon;
	}
}
