using System;
using System.Runtime.CompilerServices;
using Unity.Burst;

namespace UnityEngine.U2D.Animation;

[BurstCompile]
internal static class BurstedSpriteSkinUtilities
{
	public delegate bool ValidateBoneWeights_0000011E_0024PostfixBurstDelegate(in NativeCustomSlice<BoneWeight> boneWeights, int bindPoseCount);

	internal static class ValidateBoneWeights_0000011E_0024BurstDirectCall
	{
		private static IntPtr Pointer;

		private static IntPtr DeferredCompilation;

		[BurstDiscard]
		private unsafe static void GetFunctionPointerDiscard(ref IntPtr P_0)
		{
			if (Pointer == (IntPtr)0)
			{
				Pointer = (nint)BurstCompiler.GetILPPMethodFunctionPointer2(DeferredCompilation, (RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/, typeof(ValidateBoneWeights_0000011E_0024PostfixBurstDelegate).TypeHandle);
			}
			P_0 = Pointer;
		}

		private static IntPtr GetFunctionPointer()
		{
			nint result = 0;
			GetFunctionPointerDiscard(ref result);
			return result;
		}

		public static void Constructor()
		{
			DeferredCompilation = BurstCompiler.CompileILPPMethod2((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/);
		}

		public static void Initialize()
		{
		}

		static ValidateBoneWeights_0000011E_0024BurstDirectCall()
		{
			Constructor();
		}

		public unsafe static bool Invoke(in NativeCustomSlice<BoneWeight> boneWeights, int bindPoseCount)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = GetFunctionPointer();
				if (functionPointer != (IntPtr)0)
				{
					return ((delegate* unmanaged[Cdecl]<ref NativeCustomSlice<BoneWeight>, int, bool>)functionPointer)(ref boneWeights, bindPoseCount);
				}
			}
			return ValidateBoneWeights_0024BurstManaged(in boneWeights, bindPoseCount);
		}
	}

	[BurstCompile]
	internal static bool ValidateBoneWeights(in NativeCustomSlice<BoneWeight> boneWeights, int bindPoseCount)
	{
		return ValidateBoneWeights_0000011E_0024BurstDirectCall.Invoke(in boneWeights, bindPoseCount);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[BurstCompile]
	public static bool ValidateBoneWeights_0024BurstManaged(in NativeCustomSlice<BoneWeight> boneWeights, int bindPoseCount)
	{
		int length = boneWeights.Length;
		for (int i = 0; i < length; i++)
		{
			BoneWeight boneWeight = boneWeights[i];
			int boneIndex = boneWeight.boneIndex0;
			int boneIndex2 = boneWeight.boneIndex1;
			int boneIndex3 = boneWeight.boneIndex2;
			int boneIndex4 = boneWeight.boneIndex3;
			if (boneIndex < 0 || boneIndex >= bindPoseCount || boneIndex2 < 0 || boneIndex2 >= bindPoseCount || boneIndex3 < 0 || boneIndex3 >= bindPoseCount || boneIndex4 < 0 || boneIndex4 >= bindPoseCount)
			{
				return false;
			}
		}
		return true;
	}
}
