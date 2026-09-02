using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.U2D.Animation;

[BurstCompile]
internal struct CopySpriteRendererBuffersJob : IJobParallelFor
{
	[ReadOnly]
	public NativeArray<bool> isSpriteSkinValidForDeformArray;

	[ReadOnly]
	public NativeArray<SpriteSkinData> spriteSkinData;

	[ReadOnly]
	[NativeDisableUnsafePtrRestriction]
	public IntPtr ptrVertices;

	[WriteOnly]
	public NativeArray<IntPtr> buffers;

	[WriteOnly]
	public NativeArray<int> bufferSizes;

	public void Execute(int i)
	{
		SpriteSkinData spriteSkinData = this.spriteSkinData[i];
		IntPtr value = default;
		int value2 = 0;
		if (isSpriteSkinValidForDeformArray[i])
		{
			value = ptrVertices + spriteSkinData.deformVerticesStartPos;
			value2 = spriteSkinData.spriteVertexCount * spriteSkinData.spriteVertexStreamSize;
		}
		buffers[i] = value;
		bufferSizes[i] = value2;
	}
}
