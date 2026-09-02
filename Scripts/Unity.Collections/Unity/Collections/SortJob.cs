using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Collections;

[BurstCompatible(GenericTypeArguments = new Type[]
{
	typeof(int),
	typeof(NativeSortExtension.DefaultComparer<int>)
}, RequiredUnityDefine = "UNITY_2020_2_OR_NEWER")]
public struct SortJob<T, U> where T : unmanaged where U : IComparer<T>
{
	[BurstCompile]
	private struct SegmentSort : IJobParallelFor
	{
		[NativeDisableUnsafePtrRestriction]
		public unsafe T* Data;

		public U Comp;

		public int Length;

		public int SegmentWidth;

		public unsafe void Execute(int index)
		{
			int num = index * SegmentWidth;
			int length = ((Length - num < SegmentWidth) ? (Length - num) : SegmentWidth);
			NativeSortExtension.Sort(Data + num, length, Comp);
		}
	}

	[BurstCompile]
	private struct SegmentSortMerge : IJob
	{
		[NativeDisableUnsafePtrRestriction]
		public unsafe T* Data;

		public U Comp;

		public int Length;

		public int SegmentWidth;

		public unsafe void Execute()
		{
			int num = (Length + (SegmentWidth - 1)) / SegmentWidth;
			int* ptr = stackalloc int[num];
			T* ptr2 = (T*)Memory.Unmanaged.Allocate(UnsafeUtility.SizeOf<T>() * Length, 16, Allocator.Temp);
			for (int i = 0; i < Length; i++)
			{
				int num2 = -1;
				T val = default;
				for (int j = 0; j < num; j++)
				{
					int num3 = j * SegmentWidth;
					int num4 = ptr[j];
					int num5 = ((Length - num3 < SegmentWidth) ? (Length - num3) : SegmentWidth);
					if (num4 != num5)
					{
						T val2 = Data[num3 + num4];
						if (num2 == -1 || Comp.Compare(val2, val) <= 0)
						{
							val = val2;
							num2 = j;
						}
					}
				}
				ptr[num2]++;
				ptr2[i] = val;
			}
			UnsafeUtility.MemCpy(Data, ptr2, UnsafeUtility.SizeOf<T>() * Length);
		}
	}

	public unsafe T* Data;

	public U Comp;

	public int Length;

	[NotBurstCompatible]
	public unsafe JobHandle Schedule(JobHandle inputDeps = default(JobHandle))
	{
		if (Length == 0)
		{
			return inputDeps;
		}
		int num = (Length + 1023) / 1024;
		int num2 = math.max(1, 128);
		int innerloopBatchCount = num / num2;
		JobHandle dependsOn = new SegmentSort
		{
			Data = Data,
			Comp = Comp,
			Length = Length,
			SegmentWidth = 1024
		}.Schedule(num, innerloopBatchCount, inputDeps);
		return new SegmentSortMerge
		{
			Data = Data,
			Comp = Comp,
			Length = Length,
			SegmentWidth = 1024
		}.Schedule(dependsOn);
	}
}
