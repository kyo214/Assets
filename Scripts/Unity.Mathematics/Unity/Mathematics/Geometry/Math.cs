using System.Runtime.CompilerServices;

namespace Unity.Mathematics.Geometry;

internal static class Math
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MinMaxAABB Transform(RigidTransform transform, MinMaxAABB aabb)
	{
		float3 halfExtents = aabb.HalfExtents;
		float3 x = math.rotate(transform.rot, new float3(halfExtents.x, 0f, 0f));
		float3 x2 = math.rotate(transform.rot, new float3(0f, halfExtents.y, 0f));
		float3 x3 = math.rotate(transform.rot, new float3(0f, 0f, halfExtents.z));
		float3 float5 = math.abs(x) + math.abs(x2) + math.abs(x3);
		float3 float6 = math.transform(transform, aabb.Center);
		return new MinMaxAABB(float6 - float5, float6 + float5);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MinMaxAABB Transform(float4x4 transform, MinMaxAABB aabb)
	{
		MinMaxAABB result = Transform(new float3x3(transform), aabb);
		result.Min += transform.c3.xyz;
		result.Max += transform.c3.xyz;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MinMaxAABB Transform(float3x3 transform, MinMaxAABB aabb)
	{
		float3 float5 = transform.c0.xyz * aabb.Min.xxx;
		float3 float6 = transform.c0.xyz * aabb.Max.xxx;
		bool3 bool5 = float5 < float6;
		MinMaxAABB result = new MinMaxAABB(math.select(float6, float5, bool5), math.select(float6, float5, !bool5));
		float5 = transform.c1.xyz * aabb.Min.yyy;
		float6 = transform.c1.xyz * aabb.Max.yyy;
		bool5 = float5 < float6;
		result.Min += math.select(float6, float5, bool5);
		result.Max += math.select(float6, float5, !bool5);
		float5 = transform.c2.xyz * aabb.Min.zzz;
		float6 = transform.c2.xyz * aabb.Max.zzz;
		bool5 = float5 < float6;
		result.Min += math.select(float6, float5, bool5);
		result.Max += math.select(float6, float5, !bool5);
		return result;
	}
}
