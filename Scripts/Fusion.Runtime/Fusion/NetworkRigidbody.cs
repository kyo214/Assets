#define DEBUG
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion;

[NetworkBehaviourWeaved(37)]
[SimulationBehaviour(Stages = (SimulationStages.Forward | SimulationStages.Resimulate))]
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Fusion/Network Rigidbody")]
[HelpURL("https://doc.photonengine.com/fusion/current/manual/prebuilt-components#networkrigidbody")]
[DisallowMultipleComponent]
public class NetworkRigidbody : NetworkRigidbodyBase, IAfterPhysicsSyncTransforms3D
{
	private bool _pendingCopyRigidbodyFromBufferToEngine;

	private const int WORD_COUNT_RB_POS = 3;

	private const int WORD_COUNT_RB_ROT = 4;

	private const int WORD_COUNT_VEL = 3;

	private const int WORD_COUNT_ANG_VEL = 3;

	private const int BASE_OFFSET = 24;

	private const int OFFSET_RB_POS = 24;

	private const int OFFSET_RB_ROT = 27;

	private const int OFFSET_VEL = 31;

	private const int OFFSET_ANG_VEL = 34;

	protected const int WORD_COUNT_NRB = 37;

	public Rigidbody Rigidbody { get; private set; }

	protected override Vector3 DefaultTeleportInterpolationVelocity => Rigidbody.velocity;

	protected override Vector3 DefaultTeleportInterpolationAngularVelocity => Rigidbody.angularVelocity;

	protected override int BaseWordCount => 37;

	protected sealed override void SetIsKinematic(bool value)
	{
		Rigidbody.isKinematic = value;
	}

	protected sealed override void SetCollisionDetectionMode(CollisionDetectionMode mode)
	{
		Rigidbody.collisionDetectionMode = mode;
	}

	private void EnsureInitialized()
	{
		if (!Rigidbody)
		{
			Rigidbody = GetComponent<Rigidbody>();
			Assert.Check(Rigidbody != null, string.Format("An object with {0} must also have a {1} component.", GetType(), "Rigidbody"));
			Rigidbody.interpolation = RigidbodyInterpolation.None;
			if (Runner != null && Runner.Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.Physics2D)
			{
				base.enabled = false;
				Log.DebugWarn(string.Format("{0} found while {1} is set to 2D. Automatically disabling {2}.", GetType(), "PhysicsModes", GetType()));
			}
		}
	}

	protected override void Awake()
	{
		EnsureInitialized();
		base.Awake();
	}

	public override void Spawned()
	{
		EnsureInitialized();
		base.Spawned();
	}

	public override void RemotePrefabCreated()
	{
		EnsureInitialized();
		base.RemotePrefabCreated();
	}

	public override void StateAuthorityChanged()
	{
		EnsureInitialized();
		base.StateAuthorityChanged();
	}

	public override void CopyBackingFieldsToState(bool firstTime)
	{
		EnsureInitialized();
		base.CopyBackingFieldsToState(firstTime);
	}

	protected override void CopyFromBufferToEngine()
	{
		base.CopyFromBufferToEngine();
		if (!Physics.autoSyncTransforms && Runner.TryGetBehaviour<NetworkPhysicsSimulation3D>(out var behaviour))
		{
			behaviour.RequestPhysicsSyncTransform();
			_pendingCopyRigidbodyFromBufferToEngine = true;
		}
		else
		{
			CopyRigidbodyFromBufferToEngine();
		}
	}

	void IAfterPhysicsSyncTransforms3D.AfterPhysicsSyncTransforms3D()
	{
		if (_pendingCopyRigidbodyFromBufferToEngine)
		{
			CopyRigidbodyFromBufferToEngine();
			_pendingCopyRigidbodyFromBufferToEngine = false;
		}
	}

	protected virtual void CopyRigidbodyFromBufferToEngine()
	{
		Rigidbody.position = ReadRigidbodyPosition() + base.Transform.position;
		Quaternion quaternion = ReadRigidbodyRotation();
		Quaternion rotation = base.Transform.rotation;
		quaternion.x += rotation.x;
		quaternion.y += rotation.y;
		quaternion.z += rotation.z;
		quaternion.w += rotation.w;
		Rigidbody.rotation = quaternion.normalized;
		ReadNetworkRigidbodyFlags(out var nrbFlags, out var rbConstraints);
		bool flag = Rigidbody.IsSleeping();
		bool flag2 = (nrbFlags & NetworkRigidbodyFlags.IsSleeping) == NetworkRigidbodyFlags.IsSleeping;
		bool flag3 = false;
		if (flag != flag2)
		{
			if (!flag2)
			{
				Rigidbody.WakeUp();
			}
			else
			{
				flag3 = IsRigidbodyBelowSleepingThresholds();
			}
		}
		Vector3 vector = ReadDragsAndMass();
		Rigidbody.drag = vector.x;
		Rigidbody.angularDrag = vector.y;
		Rigidbody.mass = vector.z;
		bool flag4 = _forceSnapshotInterpolated || (nrbFlags & NetworkRigidbodyFlags.IsKinematic) == NetworkRigidbodyFlags.IsKinematic || !Object.InSimulation;
		if (Rigidbody.isKinematic != flag4)
		{
			Rigidbody.isKinematic = flag4;
		}
		if (!flag4)
		{
			Rigidbody.velocity = ReadVelocity();
			Rigidbody.angularVelocity = ReadAngularVelocity();
		}
		Rigidbody.useGravity = (nrbFlags & NetworkRigidbodyFlags.UseGravity) == NetworkRigidbodyFlags.UseGravity;
		Rigidbody.constraints = rbConstraints;
		if (flag3)
		{
			Rigidbody.Sleep();
		}
	}

	protected override void CopyFromEngineToBuffer()
	{
		base.CopyFromEngineToBuffer();
		WriteRigidbodyPosition(Rigidbody.position - base.Transform.position);
		Quaternion rotation = Rigidbody.rotation;
		Quaternion rotation2 = base.Transform.rotation;
		rotation.x -= rotation2.x;
		rotation.y -= rotation2.y;
		rotation.z -= rotation2.z;
		rotation.w -= rotation2.w;
		WriteRigidbodyRotation(rotation);
		Vector3 values = default;
		values.x = Rigidbody.drag;
		values.y = Rigidbody.angularDrag;
		values.z = Rigidbody.mass;
		WriteDragsAndMass(values);
		WriteVelocity(Rigidbody.velocity);
		WriteAngularVelocity(Rigidbody.angularVelocity);
		NetworkRigidbodyFlags networkRigidbodyFlags = (NetworkRigidbodyFlags)0;
		if (Rigidbody.isKinematic)
		{
			networkRigidbodyFlags |= NetworkRigidbodyFlags.IsKinematic;
		}
		if (Rigidbody.useGravity)
		{
			networkRigidbodyFlags |= NetworkRigidbodyFlags.UseGravity;
		}
		if (Rigidbody.IsSleeping())
		{
			networkRigidbodyFlags |= NetworkRigidbodyFlags.IsSleeping;
		}
		WriteNetworkRigidbodyFlags(networkRigidbodyFlags, Rigidbody.constraints);
	}

	private bool IsRigidbodyBelowSleepingThresholds()
	{
		float num = Rigidbody.mass * Rigidbody.velocity.sqrMagnitude;
		Vector3 angularVelocity = Rigidbody.angularVelocity;
		Vector3 inertiaTensor = Rigidbody.inertiaTensor;
		num += inertiaTensor.x * (angularVelocity.x * angularVelocity.x);
		num += inertiaTensor.y * (angularVelocity.y * angularVelocity.y);
		num += inertiaTensor.z * (angularVelocity.z * angularVelocity.z);
		num /= 2f * Rigidbody.mass;
		return num <= Physics.sleepThreshold;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector3 ReadRigidbodyPosition()
	{
		return ReadWriteUtils.ReadVector3(Ptr + 24, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector3 ReadRigidbodyPosition(int* ptr)
	{
		return ReadWriteUtils.ReadVector3(ptr + 24, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Vector3 ReadRigidbodyPosition(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadVector3(ptr + 24, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Quaternion ReadRigidbodyRotation()
	{
		return ReadWriteUtils.ReadQuaternion(Ptr + 27, Runner._rotationReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Quaternion ReadRigidbodyRotation(int* ptr)
	{
		return ReadWriteUtils.ReadQuaternion(ptr + 27, Runner._rotationReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Quaternion ReadRigidbodyRotation(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadQuaternion(ptr + 27, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector3 ReadVelocity()
	{
		return ReadWriteUtils.ReadVector3(Ptr + 31, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector3 ReadVelocity(int* ptr)
	{
		return ReadWriteUtils.ReadVector3(ptr + 31, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Vector3 ReadVelocity(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadVector3(ptr + 31, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector3 ReadAngularVelocity()
	{
		return ReadWriteUtils.ReadVector3(Ptr + 34, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector3 ReadAngularVelocity(int* ptr)
	{
		return ReadWriteUtils.ReadVector3(ptr + 34, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Vector3 ReadAngularVelocity(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadVector3(ptr + 34, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ReadNetworkRigidbodyFlags(out NetworkRigidbodyFlags nrbFlags, out RigidbodyConstraints rbConstraints)
	{
		Assert.Check(condition: true);
		int num = ReadNetworkRigidbodyRawFlags();
		nrbFlags = (NetworkRigidbodyFlags)num;
		rbConstraints = (RigidbodyConstraints)(num >> 8);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void ReadNetworkRigidbodyFlags(int* ptr, out NetworkRigidbodyFlags nrbFlags, out RigidbodyConstraints rbConstraints)
	{
		Assert.Check(condition: true);
		int num = NetworkRigidbodyBase.ReadNetworkRigidbodyRawFlags(ptr);
		nrbFlags = (NetworkRigidbodyFlags)num;
		rbConstraints = (RigidbodyConstraints)(num >> 8);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteRigidbodyPosition(Vector3 rbPos)
	{
		ReadWriteUtils.WriteVector3(Ptr + 24, Runner._positionWriteAccuracy, rbPos);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteRigidbodyPosition(Vector3 rbPos, int* ptr)
	{
		ReadWriteUtils.WriteVector3(ptr + 24, Runner._positionWriteAccuracy, rbPos);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteRigidbodyPosition(Vector3 rbPos, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteVector3(ptr + 24, writeAccuracy, rbPos);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteRigidbodyRotation(Quaternion rbRot)
	{
		ReadWriteUtils.WriteQuaternion(Ptr + 27, Runner._rotationWriteAccuracy, rbRot);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteRigidbodyRotation(Quaternion rbRot, int* ptr)
	{
		ReadWriteUtils.WriteQuaternion(ptr + 27, Runner._rotationWriteAccuracy, rbRot);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteRigidbodyRotation(Quaternion rbRot, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteQuaternion(ptr + 27, writeAccuracy, rbRot);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteVelocity(Vector3 velocity)
	{
		ReadWriteUtils.WriteVector3(Ptr + 31, Runner._positionWriteAccuracy, velocity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteVelocity(Vector3 velocity, int* ptr)
	{
		ReadWriteUtils.WriteVector3(ptr + 31, Runner._positionWriteAccuracy, velocity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteVelocity(Vector3 velocity, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteVector3(ptr + 31, writeAccuracy, velocity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteAngularVelocity(Vector3 angularVel)
	{
		ReadWriteUtils.WriteVector3(Ptr + 34, Runner._positionWriteAccuracy, angularVel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteAngularVelocity(Vector3 angularVel, int* ptr)
	{
		ReadWriteUtils.WriteVector3(ptr + 34, Runner._positionWriteAccuracy, angularVel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteAngularVelocity(Vector3 angularVel, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteVector3(ptr + 34, writeAccuracy, angularVel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteNetworkRigidbodyFlags(NetworkRigidbodyFlags nrbFlags, RigidbodyConstraints rbConstraints)
	{
		Assert.Check(condition: true);
		WriteNetworkRigidbodyRawFlags((int)nrbFlags | ((int)rbConstraints << 8));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteNetworkRigidbodyFlags(NetworkRigidbodyFlags nrbFlags, RigidbodyConstraints rbConstraints, int* ptr)
	{
		Assert.Check(condition: true);
		NetworkRigidbodyBase.WriteNetworkRigidbodyRawFlags((int)nrbFlags | ((int)rbConstraints << 8), ptr);
	}
}
