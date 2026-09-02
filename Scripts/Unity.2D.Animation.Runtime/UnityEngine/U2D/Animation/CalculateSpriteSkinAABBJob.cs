using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.U2D.Animation;

[BurstCompile]
internal struct CalculateSpriteSkinAABBJob : IJobParallelFor
{
	public NativeSlice<byte> vertices;

	[ReadOnly]
	public NativeArray<bool> isSpriteSkinValidForDeformArray;

	[ReadOnly]
	public NativeArray<SpriteSkinData> spriteSkinData;

	[WriteOnly]
	public NativeArray<Bounds> bounds;

	public unsafe void Execute(int i)
	{
		if (isSpriteSkinValidForDeformArray[i])
		{
			SpriteSkinData spriteSkinData = this.spriteSkinData[i];
			byte* unsafePtr = (byte*)vertices.GetUnsafePtr();
			NativeSlice<float3> deformablePositions = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float3>(unsafePtr + spriteSkinData.deformVerticesStartPos, spriteSkinData.spriteVertexStreamSize, spriteSkinData.spriteVertexCount);
			bounds[i] = SpriteSkinUtility.CalculateSpriteSkinBounds(deformablePositions);
		}
	}
}
