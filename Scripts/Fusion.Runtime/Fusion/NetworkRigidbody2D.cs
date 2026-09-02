#define DEBUG
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion;

[NetworkBehaviourWeaved(31)]
[SimulationBehaviour(Stages = (SimulationStages.Forward | SimulationStages.Resimulate))]
[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Fusion/Network Rigidbody 2D")]
[HelpURL("https://doc.photonengine.com/fusion/current/manual/prebuilt-components#networkrigidbody")]
[DisallowMultipleComponent]
public class NetworkRigidbody2D : NetworkRigidbodyBase, IAfterPhysicsSyncTransforms2D
{
	private bool _pendingCopyRigidbodyFromBufferToEngine;

	private const int WordCountRbPos = 2;

	private const int WordCountRbRot = 1;

	private const int WordCountVel = 2;

	private const int WordCountAngVel = 1;

	private const int WordCountGravityScl = 1;

	private const int BaseOffset = 24;

	private const int OffsetRbPos = 24;

	private const int OffsetRbRot = 26;

	private const int OffsetVel = 27;

	private const int OffsetAngVel = 29;

	private const int OffsetGravityScl = 30;

	protected const int WORD_COUNT_NRB_2D = 31;

	public Rigidbody2D Rigidbody { get; private set; }

	protected override Vector3 DefaultTeleportInterpolationVelocity => Rigidbody.velocity;

	protected override Vector3 DefaultTeleportInterpolationAngularVelocity => new Vector3(0f, 0f, Rigidbody.angularVelocity);

	protected override int BaseWordCount => 31;

	protected sealed override void SetIsKinematic(bool value)
	{
		Rigidbody.isKinematic = value;
	}

	protected sealed override void SetCollisionDetectionMode(CollisionDetectionMode mode)
	{
		Rigidbody.collisionDetectionMode = ((mode != CollisionDetectionMode.Discrete) ? CollisionDetectionMode2D.Continuous : CollisionDetectionMode2D.None);
	}

	private void EnsureInitialized()
	{
		if (!Rigidbody)
		{
			Rigidbody = GetComponent<Rigidbody2D>();
			Assert.Check(Rigidbody != null, string.Format("An object with {0} must also have a {1} component.", GetType(), "Rigidbody2D"));
			Rigidbody.interpolation = RigidbodyInterpolation2D.None;
			if (Runner != null && Runner.Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.Physics3D)
			{
				base.enabled = false;
				Log.DebugWarn(string.Format("{0} found while {1} is set to 3D. Automatically disabling {2}.", GetType(), "PhysicsModes", GetType()));
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
		if (!Physics2D.autoSyncTransforms && Runner.TryGetBehaviour<NetworkPhysicsSimulation2D>(out var behaviour))
		{
			behaviour.RequestPhysicsSyncTransform();
			_pendingCopyRigidbodyFromBufferToEngine = true;
		}
		else
		{
			CopyRigidbodyFromBufferToEngine();
		}
	}

	void IAfterPhysicsSyncTransforms2D.AfterPhysicsSyncTransforms2D()
	{
		if (_pendingCopyRigidbodyFromBufferToEngine)
		{
			CopyRigidbodyFromBufferToEngine();
			_pendingCopyRigidbodyFromBufferToEngine = false;
		}
	}

	private void CopyRigidbodyFromBufferToEngine()
	{
		Rigidbody.position = ReadRigidbodyPosition() + (Vector2)base.Transform.position;
		Rigidbody.rotation = ReadRigidbodyRotation();
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
		Rigidbody.gravityScale = ReadGravityScale();
		Rigidbody.velocity = ReadVelocity();
		Rigidbody.angularVelocity = ReadAngularVelocity();
		Rigidbody.isKinematic = _forceSnapshotInterpolated || (nrbFlags & NetworkRigidbodyFlags.IsKinematic) == NetworkRigidbodyFlags.IsKinematic || !Object.InSimulation;
		Rigidbody.constraints = rbConstraints;
		if (flag3)
		{
			Rigidbody.Sleep();
		}
	}

	protected override void CopyFromEngineToBuffer()
	{
		base.CopyFromEngineToBuffer();
		WriteRigidbodyPosition(Rigidbody.position - (Vector2)base.Transform.position);
		WriteRigidbodyRotation(Rigidbody.rotation);
		Vector3 values = default;
		values.x = Rigidbody.drag;
		values.y = Rigidbody.angularDrag;
		values.z = Rigidbody.mass;
		WriteDragsAndMass(values);
		WriteGravityScale(Rigidbody.gravityScale);
		WriteVelocity(Rigidbody.velocity);
		WriteAngularVelocity(Rigidbody.angularVelocity);
		NetworkRigidbodyFlags networkRigidbodyFlags = (NetworkRigidbodyFlags)0;
		if (Rigidbody.isKinematic)
		{
			networkRigidbodyFlags |= NetworkRigidbodyFlags.IsKinematic;
		}
		if (Rigidbody.IsSleeping())
		{
			networkRigidbodyFlags |= NetworkRigidbodyFlags.IsSleeping;
		}
		WriteNetworkRigidbodyFlags(networkRigidbodyFlags, Rigidbody.constraints);
	}

	private bool IsRigidbodyBelowSleepingThresholds()
	{
		if (Rigidbody.velocity.sqrMagnitude > Physics2D.linearSleepTolerance * Physics2D.linearSleepTolerance)
		{
			return false;
		}
		float angularVelocity = Rigidbody.angularVelocity;
		return angularVelocity * angularVelocity <= Physics2D.angularSleepTolerance * Physics2D.angularSleepTolerance;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector2 ReadRigidbodyPosition()
	{
		return ReadWriteUtils.ReadVector2(Ptr + 24, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector2 ReadRigidbodyPosition(int* ptr)
	{
		return ReadWriteUtils.ReadVector2(ptr + 24, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Vector2 ReadRigidbodyPosition(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadVector2(ptr + 24, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe float ReadRigidbodyRotation()
	{
		return ReadWriteUtils.ReadFloat(Ptr + 26, Runner._rotationReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe float ReadRigidbodyRotation(int* ptr)
	{
		return ReadWriteUtils.ReadFloat(ptr + 26, Runner._rotationReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static float ReadRigidbodyRotation(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadFloat(ptr + 26, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector2 ReadVelocity()
	{
		return ReadWriteUtils.ReadVector2(Ptr + 27, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector2 ReadVelocity(int* ptr)
	{
		return ReadWriteUtils.ReadVector2(ptr + 27, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Vector2 ReadVelocity(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadVector2(ptr + 27, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe float ReadAngularVelocity()
	{
		return ReadWriteUtils.ReadFloat(Ptr + 29, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe float ReadAngularVelocity(int* ptr)
	{
		return ReadWriteUtils.ReadFloat(ptr + 29, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static float ReadAngularVelocity(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadFloat(ptr + 29, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe float ReadGravityScale()
	{
		return ReadWriteUtils.ReadFloat(Ptr + 30, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe float ReadGravityScale(int* ptr)
	{
		return ReadWriteUtils.ReadFloat(ptr + 30, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static float ReadGravityScale(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadFloat(ptr + 30, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ReadNetworkRigidbodyFlags(out NetworkRigidbodyFlags nrbFlags, out RigidbodyConstraints2D rbConstraints)
	{
		Assert.Check(condition: true);
		int num = ReadNetworkRigidbodyRawFlags();
		nrbFlags = (NetworkRigidbodyFlags)num;
		rbConstraints = (RigidbodyConstraints2D)(num >> 8);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void ReadNetworkRigidbodyFlags(int* ptr, out NetworkRigidbodyFlags nrbFlags, out RigidbodyConstraints2D rbConstraints)
	{
		Assert.Check(condition: true);
		int num = NetworkRigidbodyBase.ReadNetworkRigidbodyRawFlags(ptr);
		nrbFlags = (NetworkRigidbodyFlags)num;
		rbConstraints = (RigidbodyConstraints2D)(num >> 8);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteRigidbodyPosition(Vector2 rbPos)
	{
		ReadWriteUtils.WriteVector2(Ptr + 24, Runner._positionWriteAccuracy, rbPos);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteRigidbodyPosition(Vector2 rbPos, int* ptr)
	{
		ReadWriteUtils.WriteVector2(ptr + 24, Runner._positionWriteAccuracy, rbPos);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteRigidbodyPosition(Vector2 rbPos, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteVector2(ptr + 24, writeAccuracy, rbPos);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteRigidbodyRotation(float rbRot)
	{
		ReadWriteUtils.WriteFloat(Ptr + 26, Runner._rotationWriteAccuracy, rbRot);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteRigidbodyRotation(float rbRot, int* ptr)
	{
		ReadWriteUtils.WriteFloat(ptr + 26, Runner._rotationWriteAccuracy, rbRot);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteRigidbodyRotation(float rbRot, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteFloat(ptr + 26, writeAccuracy, rbRot);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteVelocity(Vector2 velocity)
	{
		ReadWriteUtils.WriteVector2(Ptr + 27, Runner._positionWriteAccuracy, velocity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteVelocity(Vector2 velocity, int* ptr)
	{
		ReadWriteUtils.WriteVector2(ptr + 27, Runner._positionWriteAccuracy, velocity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteVelocity(Vector2 velocity, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteVector2(ptr + 27, writeAccuracy, velocity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteAngularVelocity(float angularVel)
	{
		ReadWriteUtils.WriteFloat(Ptr + 29, Runner._positionWriteAccuracy, angularVel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteAngularVelocity(float angularVel, int* ptr)
	{
		ReadWriteUtils.WriteFloat(ptr + 29, Runner._positionWriteAccuracy, angularVel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteAngularVelocity(float angularVel, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteFloat(ptr + 29, writeAccuracy, angularVel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteGravityScale(float gravityScale)
	{
		ReadWriteUtils.WriteFloat(Ptr + 30, Runner._positionWriteAccuracy, gravityScale);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteGravityScale(float gravityScale, int* ptr)
	{
		ReadWriteUtils.WriteFloat(ptr + 30, Runner._positionWriteAccuracy, gravityScale);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteGravityScale(float gravityScale, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteFloat(ptr + 30, writeAccuracy, gravityScale);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteNetworkRigidbodyFlags(NetworkRigidbodyFlags nrbFlags, RigidbodyConstraints2D rbConstraints)
	{
		Assert.Check(condition: true);
		WriteNetworkRigidbodyRawFlags((int)nrbFlags | ((int)rbConstraints << 8));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteNetworkRigidbodyFlags(NetworkRigidbodyFlags nrbFlags, RigidbodyConstraints2D rbConstraints, int* ptr)
	{
		Assert.Check(condition: true);
		NetworkRigidbodyBase.WriteNetworkRigidbodyRawFlags((int)nrbFlags | ((int)rbConstraints << 8), ptr);
	}
}
