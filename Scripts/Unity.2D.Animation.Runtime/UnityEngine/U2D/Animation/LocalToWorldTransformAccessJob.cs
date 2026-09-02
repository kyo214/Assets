using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace UnityEngine.U2D.Animation;

[BurstCompile]
internal struct LocalToWorldTransformAccessJob : IJobParallelForTransform
{
	[WriteOnly]
	public NativeArray<float4x4> outMatrix;

	public void Execute(int index, TransformAccess transform)
	{
		outMatrix[index] = transform.localToWorldMatrix;
	}
}
