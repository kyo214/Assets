using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.U2D.Animation;

[BurstCompile]
internal struct PrepareDeformJob : IJob
{
	[ReadOnly]
	public NativeArray<PerSkinJobData> perSkinJobData;

	[ReadOnly]
	public int batchDataSize;

	[WriteOnly]
	public NativeArray<int2> boneLookupData;

	[WriteOnly]
	public NativeArray<int2> vertexLookupData;

	public void Execute()
	{
		for (int i = 0; i < batchDataSize; i++)
		{
			PerSkinJobData perSkinJobData = this.perSkinJobData[i];
			int num = 0;
			int num2 = perSkinJobData.bindPosesIndex.x;
			while (num2 < perSkinJobData.bindPosesIndex.y)
			{
				boneLookupData[num2] = new int2(i, num);
				num2++;
				num++;
			}
			int num3 = 0;
			int num4 = perSkinJobData.verticesIndex.x;
			while (num4 < perSkinJobData.verticesIndex.y)
			{
				vertexLookupData[num4] = new int2(i, num3);
				num4++;
				num3++;
			}
		}
	}
}
