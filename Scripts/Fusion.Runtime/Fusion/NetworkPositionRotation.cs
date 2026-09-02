using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Fusion;

[DisallowMultipleComponent]
[NetworkBehaviourWeaved(7)]
[OrderAfter(new Type[] { typeof(NetworkPosition) })]
public class NetworkPositionRotation : NetworkPosition
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct EncodedRotation
	{
		[FieldOffset(0)]
		public unsafe fixed ulong Data[2];
	}

	private const int WORD_COUNT_ROTATION = 4;

	private const int OFFSET_BASE = 3;

	private const int OFFSET_ROTATION = 3;

	protected const int WORD_COUNT_NPR = 7;

	protected new virtual int BaseWordCount => 7;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override void CopyFromBufferToEngine()
	{
		base.CopyFromBufferToEngine();
		SetEngineRotation(ReadRotation());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override void CopyFromEngineToBuffer()
	{
		base.CopyFromEngineToBuffer();
		WriteRotation(GetEngineRotation());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual Quaternion GetEngineRotation()
	{
		return base.Transform.rotation;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual void SetEngineRotation(Quaternion rot)
	{
		base.Transform.rotation = rot;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Quaternion ReadRotation()
	{
		return ReadWriteUtils.ReadQuaternion(Ptr + 3, Runner._rotationReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Quaternion ReadRotation(int* ptr)
	{
		return ReadWriteUtils.ReadQuaternion(ptr + 3, Runner._rotationReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Quaternion ReadRotation(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadQuaternion(ptr + 3, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteRotation(Quaternion rotation)
	{
		ReadWriteUtils.WriteQuaternion(Ptr + 3, Runner._rotationWriteAccuracy, rotation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteRotation(Quaternion rotation, int* ptr)
	{
		ReadWriteUtils.WriteQuaternion(ptr + 3, Runner._rotationWriteAccuracy, rotation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteRotation(Quaternion rotation, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteQuaternion(ptr + 3, writeAccuracy, rotation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe EncodedRotation* ReadEncodedRotation()
	{
		return (EncodedRotation*)(Ptr + 3);
	}
}
