#define DEBUG
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Fusion;

[DisallowMultipleComponent]
[NetworkBehaviourWeaved(3)]
[OrderAfter(new Type[] { typeof(NetworkAreaOfInterestBehaviour) })]
public class NetworkPosition : NetworkAreaOfInterestBehaviour, IBeforeUpdate, IBeforeAllTicks, IAfterAllTicks, IRemotePrefabCreated, IBeforeCopyPreviousState, IAfterHostMigration
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct EncodedPosition
	{
		[FieldOffset(0)]
		public unsafe fixed uint Data[3];
	}

	private bool _copiedFromBufferToEngineThisUpdate;

	protected NetworkPosition parentNP = null;

	private bool lookedForParentNP = false;

	private const int WORD_COUNT_POSITION = 3;

	private const int OFFSET_BASE = 0;

	private const int OFFSET_POSITION = 0;

	protected const int WORD_COUNT_NPOS = 3;

	public Transform Transform { get; private set; }

	protected virtual int BaseWordCount => 3;

	public override int PositionWordOffset => 0;

	protected virtual void Awake()
	{
		if (Transform == null)
		{
			Transform = base.transform;
		}
	}

	protected virtual void OnEnable()
	{
	}

	public override void Spawned()
	{
		base.Spawned();
		if (Transform == null)
		{
			Transform = base.transform;
		}
	}

	public virtual void BeforeUpdate()
	{
		_copiedFromBufferToEngineThisUpdate = false;
	}

	public virtual void BeforeAllTicks(bool resimulation, int tickCount)
	{
		if (!Object.HasStateAuthority && !_copiedFromBufferToEngineThisUpdate)
		{
			if (!lookedForParentNP && base.transform.parent != null)
			{
				parentNP = base.transform.parent.GetComponent<NetworkPosition>();
				lookedForParentNP = true;
			}
			if ((bool)parentNP)
			{
				parentNP.BeforeAllTicks(resimulation, tickCount);
			}
			CopyFromBufferToEngine();
			_copiedFromBufferToEngineThisUpdate = true;
		}
	}

	public virtual void AfterAllTicks(bool resimulation, int tickCount)
	{
		CopyFromEngineToBuffer();
	}

	public virtual void RemotePrefabCreated()
	{
		Assert.Check(!Object.HasStateAuthority);
		if (Transform == null)
		{
			Transform = base.transform;
		}
		CopyFromBufferToEngine();
	}

	public virtual void BeforeCopyPreviousState()
	{
		Assert.Check(Runner.IsPlayer);
		CopyFromEngineToBuffer();
	}

	public override void CopyBackingFieldsToState(bool firstTime)
	{
		if (Transform == null)
		{
			Transform = base.transform;
		}
		CopyFromEngineToBuffer();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual void CopyFromBufferToEngine()
	{
		SetEnginePosition(ReadPosition());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual void CopyFromEngineToBuffer()
	{
		WritePosition(GetEnginePosition());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual Vector3 GetEnginePosition()
	{
		return Transform.position;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual void SetEnginePosition(Vector3 pos)
	{
		Transform.position = pos;
	}

	public void AfterHostMigration()
	{
		CopyFromBufferToEngine();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector3 ReadPosition()
	{
		return ReadWriteUtils.ReadVector3(Ptr, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector3 ReadPosition(int* ptr)
	{
		return ReadWriteUtils.ReadVector3(ptr, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Vector3 ReadPosition(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadVector3(ptr, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WritePosition(Vector3 position)
	{
		ReadWriteUtils.WriteVector3(Ptr, Runner._positionWriteAccuracy, position);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WritePosition(Vector3 position, int* ptr)
	{
		ReadWriteUtils.WriteVector3(ptr, Runner._positionWriteAccuracy, position);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WritePosition(Vector3 position, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteVector3(ptr, writeAccuracy, position);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe EncodedPosition* ReadEncodedPosition()
	{
		return (EncodedPosition*)Ptr;
	}
}
