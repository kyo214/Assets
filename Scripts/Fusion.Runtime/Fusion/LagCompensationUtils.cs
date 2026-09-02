using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion;

internal static class LagCompensationUtils
{
	internal struct CustomPlanesBox
	{
		public CustomPlane P0;

		public CustomPlane P1;

		public CustomPlane P2;

		public CustomPlane P3;

		public CustomPlane P4;

		public CustomPlane P5;
	}

	internal struct CustomPlane(Vector3 normal, Vector3 pointOnPlane)
	{
		public Vector3 Normal = normal;

		public Vector3 PointOnPlane = pointOnPlane;
	}

	internal struct CustomEdgesBox
	{
		public Vector3 P0;

		public Vector3 P1;

		public Vector3 P2;

		public Vector3 P3;

		public Vector3 P4;

		public Vector3 P5;

		public Vector3 P6;

		public Vector3 P7;

		public CustomLine E00
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new CustomLine(P0, P1);
			}
		}

		public CustomLine E01
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new CustomLine(P1, P2);
			}
		}

		public CustomLine E02
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new CustomLine(P2, P3);
			}
		}

		public CustomLine E03
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new CustomLine(P3, P0);
			}
		}

		public CustomLine E04
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new CustomLine(P4, P5);
			}
		}

		public CustomLine E05
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new CustomLine(P5, P6);
			}
		}

		public CustomLine E06
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new CustomLine(P6, P7);
			}
		}

		public CustomLine E07
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new CustomLine(P7, P4);
			}
		}

		public CustomLine E08
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new CustomLine(P4, P0);
			}
		}

		public CustomLine E09
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new CustomLine(P5, P1);
			}
		}

		public CustomLine E10
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new CustomLine(P6, P2);
			}
		}

		public CustomLine E11
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new CustomLine(P7, P3);
			}
		}
	}

	internal struct CustomLine(Vector3 start, Vector3 end)
	{
		public Vector3 Start = start;

		public Vector3 End = end;
	}

	private struct RotationMatrix
	{
		public float M00;

		public float M01;

		public float M02;

		public float M10;

		public float M11;

		public float M12;

		public float M20;

		public float M21;

		public float M22;
	}

	internal struct BoxNarrowData
	{
		public Vector3 Position;

		public Vector3 Extents;

		public Vector3 RotatedRight;

		public Vector3 RotatedUp;

		public Vector3 RotatedForward;

		public CustomPlanesBox BoxPlanesRotated;

		public CustomEdgesBox BoxEdgesRotated;

		public BoxNarrowData(Vector3 pos, Quaternion rot, Vector3 extents)
		{
			Position = pos;
			Extents = extents;
			RotatedRight = rot * Vector3.right;
			RotatedUp = rot * Vector3.up;
			RotatedForward = rot * Vector3.forward;
			BoxEdgesRotated.P0 = rot * new Vector3(0f - extents.x, extents.y, extents.z);
			BoxEdgesRotated.P1 = rot * new Vector3(extents.x, extents.y, extents.z);
			BoxEdgesRotated.P2 = rot * new Vector3(extents.x, extents.y, 0f - extents.z);
			BoxEdgesRotated.P3 = rot * new Vector3(0f - extents.x, extents.y, 0f - extents.z);
			BoxEdgesRotated.P4 = rot * new Vector3(0f - extents.x, 0f - extents.y, extents.z);
			BoxEdgesRotated.P5 = rot * new Vector3(extents.x, 0f - extents.y, extents.z);
			BoxEdgesRotated.P6 = rot * new Vector3(extents.x, 0f - extents.y, 0f - extents.z);
			BoxEdgesRotated.P7 = rot * new Vector3(0f - extents.x, 0f - extents.y, 0f - extents.z);
			BoxPlanesRotated.P0.Normal = RotatedRight;
			BoxPlanesRotated.P0.PointOnPlane.x = RotatedRight.x * extents.x;
			BoxPlanesRotated.P0.PointOnPlane.y = RotatedRight.y * extents.x;
			BoxPlanesRotated.P0.PointOnPlane.z = RotatedRight.z * extents.x;
			BoxPlanesRotated.P1.Normal.x = 0f - RotatedRight.x;
			BoxPlanesRotated.P1.Normal.y = 0f - RotatedRight.y;
			BoxPlanesRotated.P1.Normal.z = 0f - RotatedRight.z;
			BoxPlanesRotated.P1.PointOnPlane.x = 0f - BoxPlanesRotated.P0.PointOnPlane.x;
			BoxPlanesRotated.P1.PointOnPlane.y = 0f - BoxPlanesRotated.P0.PointOnPlane.y;
			BoxPlanesRotated.P1.PointOnPlane.z = 0f - BoxPlanesRotated.P0.PointOnPlane.z;
			BoxPlanesRotated.P2.Normal = RotatedUp;
			BoxPlanesRotated.P2.PointOnPlane.x = RotatedUp.x * extents.y;
			BoxPlanesRotated.P2.PointOnPlane.y = RotatedUp.y * extents.y;
			BoxPlanesRotated.P2.PointOnPlane.z = RotatedUp.z * extents.y;
			BoxPlanesRotated.P3.Normal.x = 0f - RotatedUp.x;
			BoxPlanesRotated.P3.Normal.y = 0f - RotatedUp.y;
			BoxPlanesRotated.P3.Normal.z = 0f - RotatedUp.z;
			BoxPlanesRotated.P3.PointOnPlane.x = 0f - BoxPlanesRotated.P2.PointOnPlane.x;
			BoxPlanesRotated.P3.PointOnPlane.y = 0f - BoxPlanesRotated.P2.PointOnPlane.y;
			BoxPlanesRotated.P3.PointOnPlane.z = 0f - BoxPlanesRotated.P2.PointOnPlane.z;
			BoxPlanesRotated.P4.Normal = RotatedForward;
			BoxPlanesRotated.P4.PointOnPlane.x = RotatedForward.x * extents.z;
			BoxPlanesRotated.P4.PointOnPlane.y = RotatedForward.y * extents.z;
			BoxPlanesRotated.P4.PointOnPlane.z = RotatedForward.z * extents.z;
			BoxPlanesRotated.P5.Normal.x = 0f - RotatedForward.x;
			BoxPlanesRotated.P5.Normal.y = 0f - RotatedForward.y;
			BoxPlanesRotated.P5.Normal.z = 0f - RotatedForward.z;
			BoxPlanesRotated.P5.PointOnPlane.x = 0f - BoxPlanesRotated.P4.PointOnPlane.x;
			BoxPlanesRotated.P5.PointOnPlane.y = 0f - BoxPlanesRotated.P4.PointOnPlane.y;
			BoxPlanesRotated.P5.PointOnPlane.z = 0f - BoxPlanesRotated.P4.PointOnPlane.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 LocalToWorldPoint(Vector3 point)
		{
			return LocalToWorldVector(point) + Position;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 WorldToLocalPoint(Vector3 point)
		{
			return WorldToLocalVector(point - Position);
		}

		public Vector3 LocalToWorldVector(Vector3 vec)
		{
			Vector3 result = default;
			result.x = RotatedRight.x * vec.x + RotatedUp.x * vec.y + RotatedForward.x * vec.z;
			result.y = RotatedRight.y * vec.x + RotatedUp.y * vec.y + RotatedForward.y * vec.z;
			result.z = RotatedRight.z * vec.x + RotatedUp.z * vec.y + RotatedForward.z * vec.z;
			return result;
		}

		public Vector3 WorldToLocalVector(Vector3 vec)
		{
			Vector3 result = default;
			result.x = RotatedRight.x * vec.x + RotatedRight.y * vec.y + RotatedRight.z * vec.z;
			result.y = RotatedUp.x * vec.x + RotatedUp.y * vec.y + RotatedUp.z * vec.z;
			result.z = RotatedForward.x * vec.x + RotatedForward.y * vec.y + RotatedForward.z * vec.z;
			return result;
		}
	}

	public struct ContactData
	{
		public Vector3 Point;

		public Vector3 Normal;

		public float Penetration;
	}

	private const float ALLOWED_DOT_DIFF = 0.975f;

	private const float EXTENTS_EXPANSION_MULTIPLIER = 1.025f;

	private const float MIN_CROSS_THRESHOLD = 0.0001f;

	internal static bool NarrowBoxBox(ref BoxNarrowData aNarrow, ref BoxNarrowData bNarrow, bool detailedManifold, out Vector3 hitPoint, out Vector3 normal)
	{
		hitPoint = default;
		normal = default;
		Vector3 extents = aNarrow.Extents;
		Vector3 extents2 = bNarrow.Extents;
		RotationMatrix rotationMatrix = default;
		rotationMatrix.M00 = Vector3.Dot(aNarrow.RotatedRight, bNarrow.RotatedRight);
		rotationMatrix.M01 = Vector3.Dot(aNarrow.RotatedRight, bNarrow.RotatedUp);
		rotationMatrix.M02 = Vector3.Dot(aNarrow.RotatedRight, bNarrow.RotatedForward);
		rotationMatrix.M10 = Vector3.Dot(aNarrow.RotatedUp, bNarrow.RotatedRight);
		rotationMatrix.M11 = Vector3.Dot(aNarrow.RotatedUp, bNarrow.RotatedUp);
		rotationMatrix.M12 = Vector3.Dot(aNarrow.RotatedUp, bNarrow.RotatedForward);
		rotationMatrix.M20 = Vector3.Dot(aNarrow.RotatedForward, bNarrow.RotatedRight);
		rotationMatrix.M21 = Vector3.Dot(aNarrow.RotatedForward, bNarrow.RotatedUp);
		rotationMatrix.M22 = Vector3.Dot(aNarrow.RotatedForward, bNarrow.RotatedForward);
		RotationMatrix rotationMatrix2 = default;
		rotationMatrix2.M00 = Mathf.Abs(rotationMatrix.M00) + 0.0001f;
		rotationMatrix2.M01 = Mathf.Abs(rotationMatrix.M01) + 0.0001f;
		rotationMatrix2.M02 = Mathf.Abs(rotationMatrix.M02) + 0.0001f;
		rotationMatrix2.M10 = Mathf.Abs(rotationMatrix.M10) + 0.0001f;
		rotationMatrix2.M11 = Mathf.Abs(rotationMatrix.M11) + 0.0001f;
		rotationMatrix2.M12 = Mathf.Abs(rotationMatrix.M12) + 0.0001f;
		rotationMatrix2.M20 = Mathf.Abs(rotationMatrix.M20) + 0.0001f;
		rotationMatrix2.M21 = Mathf.Abs(rotationMatrix.M21) + 0.0001f;
		rotationMatrix2.M22 = Mathf.Abs(rotationMatrix.M22) + 0.0001f;
		float num = float.MaxValue;
		float num2 = 0f;
		Vector3 translation = bNarrow.Position - aNarrow.Position;
		Vector3 vector = new Vector3(Vector3.Dot(translation, aNarrow.RotatedRight), Vector3.Dot(translation, aNarrow.RotatedUp), Vector3.Dot(translation, aNarrow.RotatedForward));
		float x = extents.x;
		float num3 = extents2.x * rotationMatrix2.M00 + extents2.y * rotationMatrix2.M01 + extents2.y * rotationMatrix2.M02;
		num2 = x + num3 - Mathf.Abs(vector.x);
		if (num2 <= 0f)
		{
			return false;
		}
		if (detailedManifold && num2 < num)
		{
			num = num2;
			normal = aNarrow.RotatedRight;
		}
		x = extents.y;
		num3 = extents2.x * rotationMatrix2.M10 + extents2.y * rotationMatrix2.M11 + extents2.z * rotationMatrix2.M12;
		num2 = x + num3 - Mathf.Abs(vector.y);
		if (num2 <= 0f)
		{
			return false;
		}
		if (detailedManifold && num2 < num)
		{
			num = num2;
			normal = aNarrow.RotatedUp;
		}
		x = extents.z;
		num3 = extents2.x * rotationMatrix2.M20 + extents2.y * rotationMatrix2.M21 + extents2.z * rotationMatrix2.M22;
		num2 = x + num3 - Mathf.Abs(vector.z);
		if (num2 <= 0f)
		{
			return false;
		}
		if (detailedManifold && num2 < num)
		{
			num = num2;
			normal = aNarrow.RotatedForward;
		}
		x = extents.x * rotationMatrix2.M00 + extents.y * rotationMatrix2.M10 + extents.z * rotationMatrix2.M20;
		num3 = extents2.x;
		num2 = x + num3 - Mathf.Abs(vector.x * rotationMatrix.M00 + vector.y * rotationMatrix.M10 + vector.z * rotationMatrix.M20);
		if (num2 <= 0f)
		{
			return false;
		}
		if (detailedManifold && num2 < num)
		{
			num = num2;
			normal = bNarrow.RotatedRight;
		}
		x = extents.x * rotationMatrix2.M01 + extents.y * rotationMatrix2.M11 + extents.z * rotationMatrix2.M21;
		num3 = extents2.y;
		num2 = x + num3 - Mathf.Abs(vector.x * rotationMatrix.M01 + vector.y * rotationMatrix.M11 + vector.z * rotationMatrix.M21);
		if (num2 <= 0f)
		{
			return false;
		}
		if (detailedManifold && num2 < num)
		{
			num = num2;
			normal = bNarrow.RotatedUp;
		}
		x = extents.x * rotationMatrix2.M02 + extents.y * rotationMatrix2.M12 + extents.z * rotationMatrix2.M22;
		num3 = extents2.z;
		num2 = x + num3 - Mathf.Abs(vector.x * rotationMatrix.M02 + vector.y * rotationMatrix.M12 + vector.z * rotationMatrix.M22);
		if (num2 <= 0f)
		{
			return false;
		}
		if (detailedManifold && num2 < num)
		{
			num = num2;
			normal = bNarrow.RotatedForward;
		}
		if (rotationMatrix2.M00 < 0.975f)
		{
			x = extents.y * rotationMatrix2.M20 + extents.z * rotationMatrix2.M10;
			num3 = extents2.y * rotationMatrix2.M02 + extents2.z * rotationMatrix2.M01;
			num2 = x + num3 - Mathf.Abs(vector.z * rotationMatrix.M10 - vector.y * rotationMatrix.M20);
			if (num2 <= 0f)
			{
				return false;
			}
			if (detailedManifold && num2 < num)
			{
				Vector3 vector2 = Vector3.Cross(aNarrow.RotatedRight, bNarrow.RotatedRight);
				if (vector2.sqrMagnitude > 0.0001f)
				{
					normal = vector2;
					num = num2;
				}
			}
		}
		if (rotationMatrix2.M01 < 0.975f)
		{
			x = extents.y * rotationMatrix2.M21 + extents.z * rotationMatrix2.M11;
			num3 = extents2.x * rotationMatrix2.M02 + extents2.z * rotationMatrix2.M00;
			num2 = x + num3 - Mathf.Abs(vector.z * rotationMatrix.M11 - vector.y * rotationMatrix.M21);
			if (num2 <= 0f)
			{
				return false;
			}
			if (detailedManifold && num2 < num)
			{
				Vector3 vector3 = Vector3.Cross(aNarrow.RotatedRight, bNarrow.RotatedUp);
				if (vector3.sqrMagnitude > 0.0001f)
				{
					normal = vector3;
					num = num2;
				}
			}
		}
		if (rotationMatrix2.M02 < 0.975f)
		{
			x = extents.y * rotationMatrix2.M22 + extents.z * rotationMatrix2.M12;
			num3 = extents2.x * rotationMatrix2.M01 + extents2.y * rotationMatrix2.M00;
			num2 = x + num3 - Mathf.Abs(vector.z * rotationMatrix.M12 - vector.y * rotationMatrix.M22);
			if (num2 <= 0f)
			{
				return false;
			}
			if (detailedManifold && num2 < num)
			{
				Vector3 vector4 = Vector3.Cross(aNarrow.RotatedRight, bNarrow.RotatedForward);
				if (vector4.sqrMagnitude > 0.0001f)
				{
					normal = vector4;
					num = num2;
				}
			}
		}
		if (rotationMatrix2.M10 < 0.975f)
		{
			x = extents.x * rotationMatrix2.M20 + extents.z * rotationMatrix2.M00;
			num3 = extents2.y * rotationMatrix2.M12 + extents2.z * rotationMatrix2.M11;
			num2 = x + num3 - Mathf.Abs(vector.x * rotationMatrix.M20 - vector.z * rotationMatrix.M00);
			if (num2 <= 0f)
			{
				return false;
			}
			if (detailedManifold && num2 < num)
			{
				Vector3 vector5 = Vector3.Cross(aNarrow.RotatedUp, bNarrow.RotatedRight);
				if (vector5.sqrMagnitude > 0.0001f)
				{
					normal = vector5;
					num = num2;
				}
			}
		}
		if (rotationMatrix2.M11 < 0.975f)
		{
			x = extents.x * rotationMatrix2.M21 + extents.z * rotationMatrix2.M01;
			num3 = extents2.x * rotationMatrix2.M12 + extents2.z * rotationMatrix2.M10;
			num2 = x + num3 - Mathf.Abs(vector.x * rotationMatrix.M21 - vector.z * rotationMatrix.M01);
			if (num2 <= 0f)
			{
				return false;
			}
			if (detailedManifold && num2 < num)
			{
				Vector3 vector6 = Vector3.Cross(aNarrow.RotatedUp, bNarrow.RotatedUp);
				if (vector6.sqrMagnitude > 0.0001f)
				{
					normal = vector6;
					num = num2;
				}
			}
		}
		if (rotationMatrix2.M12 < 0.975f)
		{
			x = extents.x * rotationMatrix2.M22 + extents.z * rotationMatrix2.M02;
			num3 = extents2.x * rotationMatrix2.M11 + extents2.y * rotationMatrix2.M10;
			num2 = x + num3 - Mathf.Abs(vector.x * rotationMatrix.M22 - vector.z * rotationMatrix.M02);
			if (num2 <= 0f)
			{
				return false;
			}
			if (detailedManifold && num2 < num)
			{
				Vector3 vector7 = Vector3.Cross(aNarrow.RotatedUp, bNarrow.RotatedForward);
				if (vector7.sqrMagnitude > 0.0001f)
				{
					normal = vector7;
					num = num2;
				}
			}
		}
		if (rotationMatrix2.M20 < 0.975f)
		{
			x = extents.x * rotationMatrix2.M10 + extents.y * rotationMatrix2.M00;
			num3 = extents2.y * rotationMatrix2.M22 + extents2.z * rotationMatrix2.M21;
			num2 = x + num3 - Mathf.Abs(vector.y * rotationMatrix.M00 - vector.x * rotationMatrix.M10);
			if (num2 <= 0f)
			{
				return false;
			}
			if (detailedManifold && num2 < num)
			{
				Vector3 vector8 = Vector3.Cross(aNarrow.RotatedForward, bNarrow.RotatedRight);
				if (vector8.sqrMagnitude > 0.0001f)
				{
					normal = vector8;
					num = num2;
				}
			}
		}
		if (rotationMatrix2.M21 < 0.975f)
		{
			x = extents.x * rotationMatrix2.M11 + extents.y * rotationMatrix2.M01;
			num3 = extents2.x * rotationMatrix2.M22 + extents2.z * rotationMatrix2.M20;
			num2 = x + num3 - Mathf.Abs(vector.y * rotationMatrix.M01 - vector.x * rotationMatrix.M11);
			if (num2 <= 0f)
			{
				return false;
			}
			if (detailedManifold && num2 < num)
			{
				Vector3 vector9 = Vector3.Cross(aNarrow.RotatedForward, bNarrow.RotatedUp);
				if (vector9.sqrMagnitude > 0.0001f)
				{
					normal = vector9;
					num = num2;
				}
			}
		}
		if (rotationMatrix2.M22 < 0.975f)
		{
			x = extents.x * rotationMatrix2.M12 + extents.y * rotationMatrix2.M02;
			num3 = extents2.x * rotationMatrix2.M21 + extents2.y * rotationMatrix2.M20;
			num2 = x + num3 - Mathf.Abs(vector.y * rotationMatrix.M02 - vector.x * rotationMatrix.M12);
			if (num2 <= 0f)
			{
				return false;
			}
			if (detailedManifold && num2 < num)
			{
				Vector3 vector10 = Vector3.Cross(aNarrow.RotatedForward, bNarrow.RotatedForward);
				if (vector10.sqrMagnitude > 0.0001f)
				{
					normal = vector10;
					num = num2;
				}
			}
		}
		CustomPlanesBox planesB = GetPlanesBox(bNarrow.BoxPlanesRotated, ref translation);
		CustomEdgesBox edgesB = GetEdgesBox(bNarrow.BoxEdgesRotated, ref translation);
		if (GetHitPoint(ref aNarrow.BoxPlanesRotated, ref planesB, ref aNarrow.BoxEdgesRotated, ref edgesB, ref aNarrow, ref bNarrow, ref translation, detailedManifold, out hitPoint))
		{
			if (detailedManifold)
			{
				if (Vector3.Dot(translation, normal) < 0f)
				{
					normal = -normal;
				}
				normal = normal.normalized;
			}
			return true;
		}
		return false;
	}

	private static CustomEdgesBox GetEdgesBox(CustomEdgesBox edges, ref Vector3 translation)
	{
		edges.P0 += translation;
		edges.P1 += translation;
		edges.P2 += translation;
		edges.P3 += translation;
		edges.P4 += translation;
		edges.P5 += translation;
		edges.P6 += translation;
		edges.P7 += translation;
		return edges;
	}

	private static CustomPlanesBox GetPlanesBox(CustomPlanesBox planes, ref Vector3 translation)
	{
		planes.P0.PointOnPlane += translation;
		planes.P2.PointOnPlane += translation;
		planes.P4.PointOnPlane += translation;
		planes.P1.PointOnPlane += translation;
		planes.P3.PointOnPlane += translation;
		planes.P5.PointOnPlane += translation;
		return planes;
	}

	private static bool GetHitPoint(ref CustomPlanesBox planesA, ref CustomPlanesBox planesB, ref CustomEdgesBox edgesA, ref CustomEdgesBox edgesB, ref BoxNarrowData boxNarrowA, ref BoxNarrowData boxNarrowB, ref Vector3 boxAToBoxBOffset, bool computeDetailedInfo, out Vector3 contactPoint)
	{
		Vector3 offset = default;
		contactPoint = default;
		int cpCount = 0;
		GetContactPointPlaneEdge(ref planesA, ref edgesB, ref boxNarrowA, ref offset, ref boxNarrowA.Position, computeDetailedInfo, ref cpCount, ref contactPoint);
		if (cpCount == 0 || (computeDetailedInfo && cpCount < 4))
		{
			GetContactPointPlaneEdge(ref planesB, ref edgesA, ref boxNarrowB, ref boxAToBoxBOffset, ref boxNarrowA.Position, computeDetailedInfo, ref cpCount, ref contactPoint);
		}
		if (cpCount > 0)
		{
			contactPoint /= (float)cpCount;
			return true;
		}
		if (BoxInAABB(ref edgesB, ref boxNarrowA, ref offset))
		{
			contactPoint = boxAToBoxBOffset + boxNarrowA.Position;
			return true;
		}
		if (BoxInAABB(ref edgesA, ref boxNarrowB, ref boxAToBoxBOffset))
		{
			contactPoint = boxNarrowA.Position;
			return true;
		}
		return false;
	}

	private static void GetContactPointPlaneEdge(ref CustomPlanesBox planes, ref CustomEdgesBox edges, ref BoxNarrowData boxNarrow, ref Vector3 offset, ref Vector3 boxAPosition, bool detailedManifold, ref int cpCount, ref Vector3 contactPoint)
	{
		Vector3 max = boxNarrow.Extents * 1.025f;
		Vector3 intersection = default;
		if (ClipToPlane(ref planes.P0, ref edges.P0, ref edges.P1, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P0, ref edges.P1, ref edges.P2, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P0, ref edges.P2, ref edges.P3, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P0, ref edges.P3, ref edges.P0, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P0, ref edges.P4, ref edges.P5, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P0, ref edges.P5, ref edges.P6, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P0, ref edges.P6, ref edges.P7, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P0, ref edges.P7, ref edges.P4, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P0, ref edges.P4, ref edges.P0, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P0, ref edges.P5, ref edges.P1, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P0, ref edges.P6, ref edges.P2, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P0, ref edges.P7, ref edges.P3, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P1, ref edges.P0, ref edges.P1, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P1, ref edges.P1, ref edges.P2, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P1, ref edges.P2, ref edges.P3, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P1, ref edges.P3, ref edges.P0, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P1, ref edges.P4, ref edges.P5, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P1, ref edges.P5, ref edges.P6, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P1, ref edges.P6, ref edges.P7, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P1, ref edges.P7, ref edges.P4, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P1, ref edges.P4, ref edges.P0, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P1, ref edges.P5, ref edges.P1, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P1, ref edges.P6, ref edges.P2, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P1, ref edges.P7, ref edges.P3, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P2, ref edges.P0, ref edges.P1, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P2, ref edges.P1, ref edges.P2, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P2, ref edges.P2, ref edges.P3, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P2, ref edges.P3, ref edges.P0, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P2, ref edges.P4, ref edges.P5, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P2, ref edges.P5, ref edges.P6, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P2, ref edges.P6, ref edges.P7, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P2, ref edges.P7, ref edges.P4, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P2, ref edges.P4, ref edges.P0, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P2, ref edges.P5, ref edges.P1, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P2, ref edges.P6, ref edges.P2, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P2, ref edges.P7, ref edges.P3, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P3, ref edges.P0, ref edges.P1, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P3, ref edges.P1, ref edges.P2, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P3, ref edges.P2, ref edges.P3, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P3, ref edges.P3, ref edges.P0, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P3, ref edges.P4, ref edges.P5, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P3, ref edges.P5, ref edges.P6, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P3, ref edges.P6, ref edges.P7, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P3, ref edges.P7, ref edges.P4, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P3, ref edges.P4, ref edges.P0, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P3, ref edges.P5, ref edges.P1, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P3, ref edges.P6, ref edges.P2, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P3, ref edges.P7, ref edges.P3, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P4, ref edges.P0, ref edges.P1, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P4, ref edges.P1, ref edges.P2, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P4, ref edges.P2, ref edges.P3, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P4, ref edges.P3, ref edges.P0, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P4, ref edges.P4, ref edges.P5, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P4, ref edges.P5, ref edges.P6, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P4, ref edges.P6, ref edges.P7, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P4, ref edges.P7, ref edges.P4, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P4, ref edges.P4, ref edges.P0, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P4, ref edges.P5, ref edges.P1, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P4, ref edges.P6, ref edges.P2, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P4, ref edges.P7, ref edges.P3, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P5, ref edges.P0, ref edges.P1, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P5, ref edges.P1, ref edges.P2, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P5, ref edges.P2, ref edges.P3, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P5, ref edges.P3, ref edges.P0, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P5, ref edges.P4, ref edges.P5, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P5, ref edges.P5, ref edges.P6, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P5, ref edges.P6, ref edges.P7, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P5, ref edges.P7, ref edges.P4, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P5, ref edges.P4, ref edges.P0, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P5, ref edges.P5, ref edges.P1, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P5, ref edges.P6, ref edges.P2, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (!detailedManifold || cpCount >= 4)
			{
				return;
			}
		}
		if (ClipToPlane(ref planes.P5, ref edges.P7, ref edges.P3, ref intersection) && PointInAABB(intersection, ref boxNarrow, ref max, ref offset))
		{
			contactPoint += intersection + boxAPosition;
			cpCount++;
			if (detailedManifold && cpCount < 4)
			{
			}
		}
	}

	private static bool ClipToPlane(ref CustomPlane plane, ref Vector3 lineStart, ref Vector3 lineEnd, ref Vector3 intersection)
	{
		Vector3 vector = lineEnd - lineStart;
		float num = Vector3.Dot(vector, plane.Normal);
		if (num > -0.0001f && num < 0.0001f)
		{
			return false;
		}
		float num2 = Vector3.Dot(plane.PointOnPlane - lineStart, plane.Normal) / num;
		if (num2 >= 0f && num2 <= 1f)
		{
			intersection = lineStart + vector * num2;
			return true;
		}
		return false;
	}

	private static bool BoxInAABB(ref CustomEdgesBox boxEdges, ref BoxNarrowData boxNarrow, ref Vector3 offset)
	{
		Vector3 max = boxNarrow.Extents * 1.025f;
		if (!PointInAABB(boxEdges.P0, ref boxNarrow, ref max, ref offset))
		{
			return false;
		}
		if (!PointInAABB(boxEdges.P1, ref boxNarrow, ref max, ref offset))
		{
			return false;
		}
		if (!PointInAABB(boxEdges.P2, ref boxNarrow, ref max, ref offset))
		{
			return false;
		}
		if (!PointInAABB(boxEdges.P3, ref boxNarrow, ref max, ref offset))
		{
			return false;
		}
		if (!PointInAABB(boxEdges.P4, ref boxNarrow, ref max, ref offset))
		{
			return false;
		}
		if (!PointInAABB(boxEdges.P5, ref boxNarrow, ref max, ref offset))
		{
			return false;
		}
		if (!PointInAABB(boxEdges.P6, ref boxNarrow, ref max, ref offset))
		{
			return false;
		}
		if (!PointInAABB(boxEdges.P7, ref boxNarrow, ref max, ref offset))
		{
			return false;
		}
		return true;
	}

	private static bool PointInAABB(Vector3 point, ref BoxNarrowData boxNarrow, ref Vector3 max, ref Vector3 offset)
	{
		point = boxNarrow.WorldToLocalVector(point - offset);
		Vector3 vector = -max;
		if (point.x < vector.x || point.x > max.x || point.y < vector.y || point.y > max.y || point.z < vector.z || point.z > max.z)
		{
			return false;
		}
		return true;
	}

	public static bool LocalAABBSphereIntersection(Vector3 aabbExtents, Vector3 sphereCenter, float sphereRadius)
	{
		Vector3 vector = sphereCenter;
		if (vector.x > aabbExtents.x)
		{
			vector.x = aabbExtents.x;
		}
		else if (vector.x < 0f - aabbExtents.x)
		{
			vector.x = 0f - aabbExtents.x;
		}
		if (vector.y > aabbExtents.y)
		{
			vector.y = aabbExtents.y;
		}
		else if (vector.y < 0f - aabbExtents.y)
		{
			vector.y = 0f - aabbExtents.y;
		}
		if (vector.z > aabbExtents.z)
		{
			vector.z = aabbExtents.z;
		}
		else if (vector.z < 0f - aabbExtents.z)
		{
			vector.z = 0f - aabbExtents.z;
		}
		sphereCenter.x -= vector.x;
		sphereCenter.y -= vector.y;
		sphereCenter.z -= vector.z;
		return sphereCenter.sqrMagnitude < sphereRadius * sphereRadius;
	}

	public static bool LocalAABBSphereContact(Vector3 aabbExtents, Vector3 sphereCenter, float sphereRadius, out ContactData contact)
	{
		bool flag = true;
		Vector3 vector = sphereCenter;
		if (vector.x < 0f - aabbExtents.x)
		{
			flag = false;
			vector.x = 0f - aabbExtents.x;
		}
		else if (vector.x > aabbExtents.x)
		{
			flag = false;
			vector.x = aabbExtents.x;
		}
		if (vector.y < 0f - aabbExtents.y)
		{
			flag = false;
			vector.y = 0f - aabbExtents.y;
		}
		else if (vector.y > aabbExtents.y)
		{
			flag = false;
			vector.y = aabbExtents.y;
		}
		if (vector.z < 0f - aabbExtents.z)
		{
			flag = false;
			vector.z = 0f - aabbExtents.z;
		}
		else if (vector.z > aabbExtents.z)
		{
			flag = false;
			vector.z = aabbExtents.z;
		}
		if (flag)
		{
			contact.Point = sphereCenter;
			contact.Normal = default;
			Vector3 vector2 = new Vector3(Math.Abs(vector.x), Math.Abs(vector.y), Math.Abs(vector.z));
			Vector3 vector3 = aabbExtents - vector2;
			if (vector3.y < vector3.x)
			{
				if (vector3.y < vector3.z)
				{
					contact.Normal.y = ((vector.y > 0f) ? 1f : (-1f));
					contact.Penetration = vector3.y;
				}
				else
				{
					contact.Normal.z = ((vector.z > 0f) ? 1f : (-1f));
					contact.Penetration = vector3.z;
				}
			}
			else if (vector3.x < vector3.z)
			{
				contact.Normal.x = ((vector.x > 0f) ? 1f : (-1f));
				contact.Penetration = vector3.x;
			}
			else
			{
				contact.Normal.z = ((vector.z > 0f) ? 1f : (-1f));
				contact.Penetration = vector3.z;
			}
			contact.Penetration += sphereRadius;
			return true;
		}
		contact.Point = vector;
		contact.Normal = sphereCenter - vector;
		float sqrMagnitude = contact.Normal.sqrMagnitude;
		if (sqrMagnitude >= sphereRadius * sphereRadius)
		{
			contact = default;
			return false;
		}
		contact.Penetration = (float)(1.0 / Math.Sqrt(sqrMagnitude));
		contact.Normal *= contact.Penetration;
		contact.Penetration = sphereRadius - contact.Penetration * sqrMagnitude;
		return true;
	}

	internal static bool SphereSphere(Vector3 centerA, float radiusA, Vector3 centerB, float radiusB, out Vector3 intersection, out Vector3 normal)
	{
		intersection = default;
		normal = centerA - centerB;
		float sqrMagnitude = normal.sqrMagnitude;
		float num = radiusA + radiusB;
		if (sqrMagnitude >= num * num)
		{
			return false;
		}
		float num2 = Mathf.Sqrt(sqrMagnitude);
		if (num2 > float.Epsilon)
		{
			normal /= num2;
			intersection = centerB + normal * radiusB;
		}
		else
		{
			normal = Vector3.right;
			intersection = centerB;
		}
		return true;
	}

	internal static bool RayAABB(ref Vector3 minB, ref Vector3 maxB, ref Vector3 origin, ref Vector3 dir, float sqrMaxdistance, out Vector3 point, out Vector3 normal, out float distance)
	{
		bool flag = true;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		Vector3 vector = default;
		Vector3 vector2 = default;
		point = default;
		normal = default;
		distance = 0f;
		if (origin.x < minB.x)
		{
			vector2.x = minB.x;
			flag = false;
		}
		else if (origin.x > maxB.x)
		{
			vector2.x = maxB.x;
			flag = false;
		}
		else
		{
			flag2 = true;
		}
		if (origin.y < minB.y)
		{
			vector2.y = minB.y;
			flag = false;
		}
		else if (origin.y > maxB.y)
		{
			vector2.y = maxB.y;
			flag = false;
		}
		else
		{
			flag3 = true;
		}
		if (origin.z < minB.z)
		{
			vector2.z = minB.z;
			flag = false;
		}
		else if (origin.z > maxB.z)
		{
			vector2.z = maxB.z;
			flag = false;
		}
		else
		{
			flag4 = true;
		}
		if (flag)
		{
			point = origin;
			return false;
		}
		if (dir.x != 0f && !flag2)
		{
			vector.x = (vector2.x - origin.x) / dir.x;
		}
		else
		{
			vector.x = -1f;
		}
		if (dir.y != 0f && !flag3)
		{
			vector.y = (vector2.y - origin.y) / dir.y;
		}
		else
		{
			vector.y = -1f;
		}
		if (dir.z != 0f && !flag4)
		{
			vector.z = (vector2.z - origin.z) / dir.z;
		}
		else
		{
			vector.z = -1f;
		}
		int num = 0;
		float num2 = vector.x;
		if (num2 < vector.y)
		{
			num = 1;
			num2 = vector.y;
		}
		if (num2 < vector.z)
		{
			num = 2;
			num2 = vector.z;
		}
		if (num2 < 0f)
		{
			return false;
		}
		if (num != 0)
		{
			point.x = origin.x + num2 * dir.x;
			if (point.x < minB.x || point.x > maxB.x)
			{
				return false;
			}
		}
		else
		{
			point.x = vector2.x;
		}
		if (num != 1)
		{
			point.y = origin.y + num2 * dir.y;
			if (point.y < minB.y || point.y > maxB.y)
			{
				return false;
			}
		}
		else
		{
			point.y = vector2.y;
		}
		if (num != 2)
		{
			point.z = origin.z + num2 * dir.z;
			if (point.z < minB.z || point.z > maxB.z)
			{
				return false;
			}
		}
		else
		{
			point.z = vector2.z;
		}
		float sqrMagnitude = (origin - point).sqrMagnitude;
		if (sqrMagnitude <= sqrMaxdistance)
		{
			switch (num)
			{
			case 0:
				normal = ((origin.x > point.x) ? Vector3.right : Vector3.left);
				break;
			case 1:
				normal = ((origin.y > point.y) ? Vector3.up : Vector3.down);
				break;
			case 2:
				normal = ((origin.z > point.z) ? Vector3.forward : Vector3.back);
				break;
			}
			distance = Mathf.Sqrt(sqrMagnitude);
			return true;
		}
		return false;
	}

	internal static bool RaySphereIntersection(Vector3 p1, Vector3 dir, float length, Vector3 center, float radius, out Vector3 hitPoint, out Vector3 normal, out float distance)
	{
		float num = radius * radius;
		Vector3 lhs = p1 - center;
		float sqrMagnitude = lhs.sqrMagnitude;
		if (sqrMagnitude < num)
		{
			hitPoint = default;
			normal = default;
			distance = 0f;
			return false;
		}
		if (length < float.Epsilon)
		{
			hitPoint = default;
			normal = default;
			distance = 0f;
			return false;
		}
		float num2 = Vector3.Dot(lhs, -dir);
		if (num2 < 0f)
		{
			hitPoint = default;
			normal = default;
			distance = 0f;
			return false;
		}
		Vector3 vector = p1 + dir * num2;
		float sqrMagnitude2 = (center - vector).sqrMagnitude;
		if (sqrMagnitude2 > num)
		{
			hitPoint = default;
			normal = default;
			distance = 0f;
			return false;
		}
		float num3 = Mathf.Sqrt(num - sqrMagnitude2);
		hitPoint = vector - dir * num3;
		distance = num2 - num3;
		if (length < distance)
		{
			normal = default;
			distance = 0f;
			return false;
		}
		normal = (hitPoint - center).normalized;
		return true;
	}
}
