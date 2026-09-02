using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.U2D.Animation;

[BurstCompile]
internal struct FillPerSkinJobSingleThread : IJob
{
	public PerSkinJobData combinedSkinBatch;

	[ReadOnly]
	public NativeArray<bool> isSpriteSkinValidForDeformArray;

	public NativeArray<SpriteSkinData> spriteSkinDataArray;

	public NativeArray<PerSkinJobData> perSkinJobDataArray;

	public NativeArray<PerSkinJobData> combinedSkinBatchArray;

	public void Execute()
	{
		int length = spriteSkinDataArray.Length;
		for (int i = 0; i < length; i++)
		{
			SpriteSkinData value = spriteSkinDataArray[i];
			value.deformVerticesStartPos = -1;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (isSpriteSkinValidForDeformArray[i])
			{
				value.deformVerticesStartPos = combinedSkinBatch.deformVerticesStartPos;
				num = value.spriteVertexCount * value.spriteVertexStreamSize;
				num2 = value.spriteVertexCount;
				num3 = value.bindPoses.Length;
			}
			combinedSkinBatch.verticesIndex.x = combinedSkinBatch.verticesIndex.y;
			combinedSkinBatch.verticesIndex.y = combinedSkinBatch.verticesIndex.x + num2;
			combinedSkinBatch.bindPosesIndex.x = combinedSkinBatch.bindPosesIndex.y;
			combinedSkinBatch.bindPosesIndex.y = combinedSkinBatch.bindPosesIndex.x + num3;
			spriteSkinDataArray[i] = value;
			perSkinJobDataArray[i] = combinedSkinBatch;
			combinedSkinBatch.deformVerticesStartPos += num;
		}
		combinedSkinBatchArray[0] = combinedSkinBatch;
	}
}
