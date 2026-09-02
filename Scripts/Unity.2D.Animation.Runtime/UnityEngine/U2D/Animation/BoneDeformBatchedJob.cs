using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.U2D.Animation;

[BurstCompile]
internal struct BoneDeformBatchedJob : IJobParallelFor
{
	[ReadOnly]
	public NativeArray<float4x4> boneTransform;

	[ReadOnly]
	public NativeArray<float4x4> rootTransform;

	[ReadOnly]
	public NativeArray<int2> boneLookupData;

	[ReadOnly]
	public NativeArray<SpriteSkinData> spriteSkinData;

	[ReadOnly]
	public NativeHashMap<int, TransformAccessJob.TransformData> rootTransformIndex;

	[ReadOnly]
	public NativeHashMap<int, TransformAccessJob.TransformData> boneTransformIndex;

	[WriteOnly]
	public NativeArray<float4x4> finalBoneTransforms;

	public void Execute(int i)
	{
		int x = boneLookupData[i].x;
		int y = boneLookupData[i].y;
		SpriteSkinData spriteSkinData = this.spriteSkinData[x];
		int key = spriteSkinData.boneTransformId[y];
		int transformIndex = boneTransformIndex[key].transformIndex;
		if (transformIndex >= 0)
		{
			float4x4 a = boneTransform[transformIndex];
			Matrix4x4 matrix4x = spriteSkinData.bindPoses[y];
			int transformIndex2 = rootTransformIndex[spriteSkinData.transformId].transformIndex;
			finalBoneTransforms[i] = math.mul(rootTransform[transformIndex2], math.mul(a, matrix4x));
		}
	}
}
