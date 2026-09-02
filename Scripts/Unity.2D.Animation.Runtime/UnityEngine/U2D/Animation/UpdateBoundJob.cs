using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.U2D.Animation;

[BurstCompile]
internal struct UpdateBoundJob : IJobParallelFor
{
	[ReadOnly]
	public NativeArray<int> rootTransformId;

	[ReadOnly]
	public NativeArray<int> rootBoneTransformId;

	[ReadOnly]
	public NativeArray<float4x4> rootTransform;

	[ReadOnly]
	public NativeArray<float4x4> boneTransform;

	[ReadOnly]
	public NativeHashMap<int, TransformAccessJob.TransformData> rootTransformIndex;

	[ReadOnly]
	public NativeHashMap<int, TransformAccessJob.TransformData> boneTransformIndex;

	[ReadOnly]
	public NativeArray<Bounds> spriteSkinBound;

	public NativeArray<Bounds> bounds;

	public void Execute(int i)
	{
		Bounds bounds = spriteSkinBound[i];
		int transformIndex = rootTransformIndex[rootTransformId[i]].transformIndex;
		int transformIndex2 = boneTransformIndex[rootBoneTransformId[i]].transformIndex;
		if (transformIndex >= 0 && transformIndex2 >= 0)
		{
			float4x4 a = rootTransform[transformIndex];
			float4x4 b = boneTransform[transformIndex2];
			float4x4 a2 = math.mul(a, b);
			float4 float5 = new float4(bounds.center, 1f);
			float4 float6 = new float4(bounds.extents, 0f);
			float4 x = math.mul(a2, float5 + new float4(0f - float6.x, 0f - float6.y, float6.z, float6.w));
			float4 x2 = math.mul(a2, float5 + new float4(0f - float6.x, float6.y, float6.z, float6.w));
			float4 x3 = math.mul(a2, float5 + float6);
			float4 y = math.mul(a2, float5 + new float4(float6.x, 0f - float6.y, float6.z, float6.w));
			float4 float7 = math.min(x, math.min(x2, math.min(x3, y)));
			float6 = (math.max(x, math.max(x2, math.max(x3, y))) - float7) * 0.5f;
			float5 = float7 + float6;
			this.bounds[i] = new Bounds
			{
				center = new Vector3(float5.x, float5.y, float5.z),
				extents = new Vector3(float6.x, float6.y, float6.z)
			};
		}
	}
}
