using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.U2D.Animation;

[BurstCompile]
internal struct SkinDeformBatchedJob : IJobParallelFor
{
	public NativeSlice<byte> vertices;

	[ReadOnly]
	public NativeArray<float4x4> finalBoneTransforms;

	[ReadOnly]
	public NativeArray<PerSkinJobData> perSkinJobData;

	[ReadOnly]
	public NativeArray<SpriteSkinData> spriteSkinData;

	[ReadOnly]
	public NativeArray<int2> vertexLookupData;

	public unsafe void Execute(int i)
	{
		int x = vertexLookupData[i].x;
		int y = vertexLookupData[i].y;
		PerSkinJobData perSkinJobData = this.perSkinJobData[x];
		SpriteSkinData spriteSkinData = this.spriteSkinData[x];
		float3 b = spriteSkinData.vertices[y];
		float4 float5 = spriteSkinData.tangents[y];
		BoneWeight boneWeight = spriteSkinData.boneWeights[y];
		int index = boneWeight.boneIndex0 + perSkinJobData.bindPosesIndex.x;
		int index2 = boneWeight.boneIndex1 + perSkinJobData.bindPosesIndex.x;
		int index3 = boneWeight.boneIndex2 + perSkinJobData.bindPosesIndex.x;
		int index4 = boneWeight.boneIndex3 + perSkinJobData.bindPosesIndex.x;
		byte* unsafePtr = (byte*)vertices.GetUnsafePtr();
		byte* ptr = unsafePtr + spriteSkinData.deformVerticesStartPos;
		NativeSlice<float3> nativeSlice = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float3>(ptr, spriteSkinData.spriteVertexStreamSize, spriteSkinData.spriteVertexCount);
		if (spriteSkinData.hasTangents)
		{
			NativeSlice<float4> nativeSlice2 = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float4>(ptr + spriteSkinData.tangentVertexOffset, spriteSkinData.spriteVertexStreamSize, spriteSkinData.spriteVertexCount);
			float4 b2 = new float4(float5.xyz, 0f);
			nativeSlice2[y] = new float4(math.normalize((math.mul(finalBoneTransforms[index], b2) * boneWeight.weight0 + math.mul(finalBoneTransforms[index2], b2) * boneWeight.weight1 + math.mul(finalBoneTransforms[index3], b2) * boneWeight.weight2 + math.mul(finalBoneTransforms[index4], b2) * boneWeight.weight3).xyz), float5.w);
		}
		nativeSlice[y] = math.transform(finalBoneTransforms[index], b) * boneWeight.weight0 + math.transform(finalBoneTransforms[index2], b) * boneWeight.weight1 + math.transform(finalBoneTransforms[index3], b) * boneWeight.weight2 + math.transform(finalBoneTransforms[index4], b) * boneWeight.weight3;
	}
}
