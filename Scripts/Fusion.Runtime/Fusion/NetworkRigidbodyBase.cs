#define DEBUG
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion;

[OrderAfter(new Type[] { typeof(NetworkTransform) })]
public abstract class NetworkRigidbodyBase : NetworkTransform, IStateAuthorityChanged
{
	[Flags]
	public enum NetworkRigidbodyFlags : byte
	{
		IsKinematic = 1,
		UseGravity = 2,
		IsSleeping = 4
	}

	protected bool _forceSnapshotInterpolated;

	private bool _initialized;

	private const int WORD_COUNT_DRAG = 1;

	private const int WORD_COUNT_ANG_DRAG = 1;

	private const int WORD_COUNT_MASS = 1;

	private const int WORD_COUNT_FLAGS = 1;

	private const int BASE_OFFSET = 20;

	private const int OFFSET_DRAG = 20;

	private const int OFFSET_ANG_DRAG = 21;

	private const int OFFSET_MASS = 22;

	private const int OFFSET_FLAGS = 23;

	protected const int WORD_COUNT_NRBB = 24;

	protected override int BaseWordCount => 24;

	protected abstract void SetIsKinematic(bool value);

	protected abstract void SetCollisionDetectionMode(CollisionDetectionMode mode);

	private void Init()
	{
		Assert.Check(condition: true);
		Assert.Check(condition: true);
		if (Runner.Config.Simulation.Topology == SimulationConfig.Topologies.Shared)
		{
			_forceSnapshotInterpolated = !Object.HasStateAuthority;
		}
		else if (Runner.Config.ServerPhysicsMode == NetworkProjectConfig.PhysicsModes.ClientPrediction)
		{
			_forceSnapshotInterpolated = false;
		}
		else
		{
			Assert.Check(Runner.Config.ServerPhysicsMode == NetworkProjectConfig.PhysicsModes.ServerOnly);
			_forceSnapshotInterpolated = Runner.IsClient;
		}
		_initialized = true;
	}

	public override void RemotePrefabCreated()
	{
		base.RemotePrefabCreated();
		if (!_initialized)
		{
			Init();
		}
	}

	public override void CopyBackingFieldsToState(bool firstTime)
	{
		base.CopyBackingFieldsToState(firstTime);
		if (!_initialized)
		{
			Init();
		}
	}

	public override void BeforeAllTicks(bool resimulation, int tickCount)
	{
		Assert.Check(_initialized, "Network Rigidbody not initialized.");
		base.BeforeAllTicks(resimulation, tickCount);
		if (_forceSnapshotInterpolated)
		{
			SetCollisionDetectionMode(CollisionDetectionMode.Discrete);
		}
	}

	public override void AfterAllTicks(bool resimulation, int tickCount)
	{
		Assert.Check(_initialized, "Network Rigidbody not initialized.");
		if (!_forceSnapshotInterpolated)
		{
			base.AfterAllTicks(resimulation, tickCount);
		}
	}

	public override void BeforeCopyPreviousState()
	{
		Assert.Check(_initialized, "Network Rigidbody not initialized.");
		if (!_forceSnapshotInterpolated)
		{
			base.BeforeCopyPreviousState();
		}
	}

	public virtual void StateAuthorityChanged()
	{
		if (Runner.Config.Simulation.Topology == SimulationConfig.Topologies.Shared)
		{
			Init();
			if (!Object.HasStateAuthority)
			{
				SetCollisionDetectionMode(CollisionDetectionMode.Discrete);
				SetIsKinematic(value: true);
			}
			else
			{
				SetIsKinematic((ReadNetworkRigidbodyFlags() & NetworkRigidbodyFlags.IsKinematic) == NetworkRigidbodyFlags.IsKinematic);
			}
		}
	}

	public override bool IsInterpolationDataPredicted()
	{
		Assert.Check(_initialized, "Network Rigidbody not initialized.");
		return _interpolationDataSource switch
		{
			InterpolationDataSources.Auto => !_forceSnapshotInterpolated && Object.InSimulation, 
			InterpolationDataSources.Snapshots => Object.HasStateAuthority, 
			InterpolationDataSources.Predicted => !_forceSnapshotInterpolated, 
			_ => !_forceSnapshotInterpolated, 
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe float ReadDrag()
	{
		return ReadWriteUtils.ReadFloat(Ptr + 20, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe float ReadDrag(int* ptr)
	{
		return ReadWriteUtils.ReadFloat(ptr + 20, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static float ReadDrag(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadFloat(ptr + 20, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe float ReadAngularDrag()
	{
		return ReadWriteUtils.ReadFloat(Ptr + 21, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe float ReadAngularDrag(int* ptr)
	{
		return ReadWriteUtils.ReadFloat(ptr + 21, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static float ReadAngularDrag(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadFloat(ptr + 21, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe float ReadMass()
	{
		return ReadWriteUtils.ReadFloat(Ptr + 22, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe float ReadMass(int* ptr)
	{
		return ReadWriteUtils.ReadFloat(ptr + 22, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static float ReadMass(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadFloat(ptr + 22, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe NetworkRigidbodyFlags ReadNetworkRigidbodyFlags()
	{
		return (NetworkRigidbodyFlags)Ptr[23];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static NetworkRigidbodyFlags ReadNetworkRigidbodyFlags(int* ptr)
	{
		return (NetworkRigidbodyFlags)ptr[23];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected unsafe int ReadNetworkRigidbodyRawFlags()
	{
		return Ptr[23];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected unsafe static int ReadNetworkRigidbodyRawFlags(int* ptr)
	{
		return ptr[23];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected unsafe Vector3 ReadDragsAndMass()
	{
		return ReadWriteUtils.ReadVector3(Ptr + 20, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected unsafe Vector3 ReadDragsAndMass(int* ptr)
	{
		return ReadWriteUtils.ReadVector3(ptr + 20, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected unsafe static Vector3 ReadDragsAndMass(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadVector3(ptr + 20, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteDrag(float drag)
	{
		ReadWriteUtils.WriteFloat(Ptr + 20, Runner._positionWriteAccuracy, drag);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteDrag(float drag, int* ptr)
	{
		ReadWriteUtils.WriteFloat(ptr + 20, Runner._positionWriteAccuracy, drag);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteDrag(float drag, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteFloat(ptr + 20, writeAccuracy, drag);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteAngularDrag(float angDrag)
	{
		ReadWriteUtils.WriteFloat(Ptr + 21, Runner._positionWriteAccuracy, angDrag);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteAngularDrag(float angDrag, int* ptr)
	{
		ReadWriteUtils.WriteFloat(ptr + 21, Runner._positionWriteAccuracy, angDrag);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteAngularDrag(float angDrag, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteFloat(ptr + 21, writeAccuracy, angDrag);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteMass(float mass)
	{
		ReadWriteUtils.WriteFloat(Ptr + 22, Runner._positionWriteAccuracy, mass);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteMass(float mass, int* ptr)
	{
		ReadWriteUtils.WriteFloat(ptr + 22, Runner._positionWriteAccuracy, mass);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteMass(float mass, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteFloat(ptr + 22, writeAccuracy, mass);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteNetworkRigidbodyFlags(NetworkRigidbodyFlags flags)
	{
		Ptr[23] |= (int)flags;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteNetworkRigidbodyFlags(NetworkRigidbodyFlags flags, int* ptr)
	{
		ptr[23] |= (int)flags;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected unsafe void WriteNetworkRigidbodyRawFlags(int rawFlags)
	{
		Ptr[23] = rawFlags;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected unsafe static void WriteNetworkRigidbodyRawFlags(int rawFlags, int* ptr)
	{
		ptr[23] = rawFlags;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected unsafe void WriteDragsAndMass(Vector3 values)
	{
		ReadWriteUtils.WriteVector3(Ptr + 20, Runner._positionWriteAccuracy, values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected unsafe void WriteDragsAndMass(Vector3 values, int* ptr)
	{
		ReadWriteUtils.WriteVector3(ptr + 20, Runner._positionWriteAccuracy, values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected unsafe static void WriteDragsAndMass(Vector3 values, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteVector3(ptr + 20, writeAccuracy, values);
	}
}
