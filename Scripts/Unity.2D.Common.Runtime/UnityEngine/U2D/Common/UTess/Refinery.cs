using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.U2D.Common.UTess;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct Refinery
{
	private static readonly float kMinAreaFactor = 0.0482f;

	private static readonly float kMaxAreaFactor = 0.482f;

	private static readonly int kMaxSteinerCount = 4084;

	private static bool RequiresRefining(UTriangle tri, float maxArea)
	{
		return tri.area > maxArea;
	}

	private static void FetchEncroachedSegments(NativeArray<float2> pgPoints, int pgPointCount, NativeArray<int2> pgEdges, int pgEdgeCount, ref Array<UEncroachingSegment> encroach, ref int encroachCount, UCircle c)
	{
		for (int i = 0; i < pgEdgeCount; i++)
		{
			int2 int5 = pgEdges[i];
			float2 float5 = pgPoints[int5.x];
			float2 float6 = pgPoints[int5.y];
			if (math.any(c.center - float5) && math.any(c.center - float6))
			{
				float2 x = float5 - float6;
				float2 obj = (float5 + float6) * 0.5f;
				float num = math.length(x) * 0.5f;
				if (!(math.length(obj - c.center) > num))
				{
					UEncroachingSegment value = new UEncroachingSegment
					{
						a = float5,
						b = float6,
						index = i
					};
					encroach[encroachCount++] = value;
				}
			}
		}
	}

	private static void InsertVertex(ref NativeArray<float2> pgPoints, ref int pgPointCount, float2 newVertex, ref int nid)
	{
		nid = pgPointCount;
		pgPoints[nid] = newVertex;
		pgPointCount++;
	}

	private static void SplitSegments(ref NativeArray<float2> pgPoints, ref int pgPointCount, ref NativeArray<int2> pgEdges, ref int pgEdgeCount, UEncroachingSegment es)
	{
		int index = es.index;
		int2 int5 = pgEdges[index];
		float2 obj = pgPoints[int5.x];
		float2 float5 = pgPoints[int5.y];
		float2 float6 = (obj + float5) * 0.5f;
		int num = 0;
		if (math.abs(int5.x - int5.y) == 1)
		{
			num = ((int5.x > int5.y) ? int5.x : int5.y);
			InsertVertex(ref pgPoints, ref pgPointCount, float6, ref num);
			int2 int6 = pgEdges[index];
			pgEdges[index] = new int2(int6.x, num);
			for (int num2 = pgEdgeCount; num2 > index + 1; num2--)
			{
				pgEdges[num2] = pgEdges[num2 - 1];
			}
			pgEdges[index + 1] = new int2(num, int6.y);
			pgEdgeCount++;
		}
		else
		{
			num = pgPointCount;
			pgPoints[pgPointCount++] = float6;
			pgEdges[index] = new int2(math.max(int5.x, int5.y), num);
			pgEdges[pgEdgeCount++] = new int2(math.min(int5.x, int5.y), num);
		}
	}

	internal static bool Condition(Allocator allocator, float factorArea, float targetArea, ref NativeArray<float2> pgPoints, ref int pgPointCount, ref NativeArray<int2> pgEdges, ref int pgEdgeCount, ref NativeArray<float2> vertices, ref int vertexCount, ref NativeArray<int> indices, ref int indexCount, ref float maxArea)
	{
		maxArea = 0f;
		float minArea = 0f;
		float avgArea = 0f;
		bool flag = false;
		bool flag2 = true;
		int triangleCount = 0;
		int num = -1;
		int num2 = pgPointCount;
		Array<UEncroachingSegment> encroach = new Array<UEncroachingSegment>(num2, ModuleHandle.kMaxEdgeCount, allocator, NativeArrayOptions.UninitializedMemory);
		Array<UTriangle> triangles = new Array<UTriangle>(num2 * 4, ModuleHandle.kMaxTriangleCount, allocator, NativeArrayOptions.UninitializedMemory);
		ModuleHandle.BuildTriangles(vertices, vertexCount, indices, indexCount, ref triangles, ref triangleCount, ref maxArea, ref avgArea, ref minArea);
		factorArea = ((factorArea != 0f) ? math.clamp(factorArea, kMinAreaFactor, kMaxAreaFactor) : factorArea);
		float x = maxArea * factorArea;
		x = math.max(x, targetArea);
		while (!flag & flag2)
		{
			for (int i = 0; i < triangleCount; i++)
			{
				if (RequiresRefining(triangles[i], x))
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				UTriangle uTriangle = triangles[num];
				int encroachCount = 0;
				FetchEncroachedSegments(pgPoints, pgPointCount, pgEdges, pgEdgeCount, ref encroach, ref encroachCount, uTriangle.c);
				if (encroachCount != 0)
				{
					for (int j = 0; j < encroachCount; j++)
					{
						SplitSegments(ref pgPoints, ref pgPointCount, ref pgEdges, ref pgEdgeCount, encroach[j]);
					}
				}
				else
				{
					float2 center = uTriangle.c.center;
					pgPoints[pgPointCount++] = center;
				}
				indexCount = 0;
				vertexCount = 0;
				flag2 = Tessellator.Tessellate(allocator, pgPoints, pgPointCount, pgEdges, pgEdgeCount, ref vertices, ref vertexCount, ref indices, ref indexCount);
				encroachCount = 0;
				triangleCount = 0;
				num = -1;
				if (flag2)
				{
					ModuleHandle.BuildTriangles(vertices, vertexCount, indices, indexCount, ref triangles, ref triangleCount, ref maxArea, ref avgArea, ref minArea);
				}
				if (pgPointCount - num2 > kMaxSteinerCount)
				{
					break;
				}
			}
			else
			{
				flag = true;
			}
		}
		triangles.Dispose();
		encroach.Dispose();
		return flag;
	}
}
