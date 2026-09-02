using System;
using UnityEngine;

namespace Fusion;

[NetworkBehaviourWeaved(-1)]
[SimulationBehaviour(Stages = (SimulationStages.Forward | SimulationStages.Resimulate))]
[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
[Obsolete("This class has been replaced by a new NetworkRigidbody class and is now obsolete.")]
public class NetworkRigidbodyObsolete : NetworkRigidbodyBaseObsolete, IStateAuthorityChanged
{
	private Vector3? _lastPos;

	private Quaternion? _lastRot;

	private Rigidbody _rigidbody;

	public Rigidbody Rigidbody => _rigidbody ? _rigidbody : (_rigidbody = GetComponent<Rigidbody>());

	internal override void SetIsKinematic(bool value)
	{
		_rigidbody.isKinematic = value;
	}

	protected override void Awake()
	{
		base.Awake();
		TryGetComponent<Rigidbody>(out _rigidbody);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if ((bool)_rigidbody)
		{
			_rigidbody.WakeUp();
		}
	}

	public override void CopyBackingFieldsToState(bool firstTime)
	{
		Rigidbody rigidbody = Rigidbody;
		rigidbody.position = base.Transform.position;
		rigidbody.rotation = base.Transform.rotation;
		base.CopyBackingFieldsToState(firstTime);
	}

	public override void Spawned()
	{
		if (Runner.Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.Physics2D)
		{
			base.enabled = false;
			Log.Error("NetworkRigidbodyObsolete found while PhysicsModes is set to 2D. Automatically disabling NetworkRigidbodyObsolete.");
			return;
		}
		base.Spawned();
		base.Transform.position = Rigidbody.position;
		base.Transform.rotation = Rigidbody.rotation;
		if ((bool)Rigidbody)
		{
			Rigidbody.interpolation = RigidbodyInterpolation.None;
		}
	}

	internal unsafe override void CopyBuffers2Engine(bool posRotOnly)
	{
		bool flag = IsRigidbodyBelowSleepingThresholds();
		var (position, rotation) = ReadBufferAndConvertToWorldSpace(4);
		Copy2EngineTRSState(position, rotation);
		Physics.SyncTransforms();
		bool forceTransform = SyncParent && Copy2EngineAnchorState();
		int num = Ptr[31];
		bool flag2 = (num & 1) == 1 || !Object.InSimulation;
		if (posRotOnly)
		{
			(Vector3, Quaternion) tuple2 = ReadBufferAndConvertToWorldSpace(4);
			Copy2EngineTRSState(tuple2.Item1, tuple2.Item2, forceTransform);
			return;
		}
		Rigidbody rigidbody = Rigidbody;
		bool flag3 = (num & 2) == 2;
		RigidbodyConstraints rigidbodyConstraints = (RigidbodyConstraints)(num >> 4);
		if (rigidbody.useGravity != flag3)
		{
			rigidbody.useGravity = flag3;
		}
		if (rigidbody.constraints != rigidbodyConstraints)
		{
			rigidbody.constraints = rigidbodyConstraints;
		}
		if (rigidbody.isKinematic != flag2)
		{
			rigidbody.isKinematic = flag2;
		}
		if (SyncDragAndMass)
		{
			rigidbody.drag = ReadWriteUtils.ReadFloat(Ptr + 32, Runner._positionReadAccuracy);
			rigidbody.angularDrag = ReadWriteUtils.ReadFloat(Ptr + 33, Runner._positionReadAccuracy);
			rigidbody.mass = ReadWriteUtils.ReadFloat(Ptr + 34, Runner._positionReadAccuracy);
		}
		bool flag4 = (num & 4) == 4;
		bool flag5 = rigidbody.IsSleeping();
		if (flag4)
		{
			if (flag5)
			{
				return;
			}
		}
		else if (flag5)
		{
			rigidbody.WakeUp();
		}
		if (flag)
		{
			rigidbody.Sleep();
			return;
		}
		rigidbody.velocity = ReadWriteUtils.ReadVector3(Ptr + 25, Runner._positionReadAccuracy);
		rigidbody.angularVelocity = ReadWriteUtils.ReadVector3(Ptr + 28, Runner._positionReadAccuracy);
	}

	internal unsafe override void Copy2BuffersFlags()
	{
		Rigidbody rigidbody = Rigidbody;
		int num = (int)rigidbody.constraints << 4;
		if (rigidbody.isKinematic)
		{
			num |= 1;
		}
		if (rigidbody.useGravity)
		{
			num |= 2;
		}
		if (rigidbody.IsSleeping())
		{
			num |= 4;
		}
		Ptr[31] = num;
	}

	protected override void GetEnginePositionRotation2Buffer(out Vector3 position, out Quaternion rotation)
	{
		Transform transform = base.transform;
		if (Space == Spaces.Local && (bool)transform.parent)
		{
			position = transform.localPosition;
			rotation = transform.localRotation;
		}
		else
		{
			Rigidbody rigidbody = Rigidbody;
			position = rigidbody.position;
			rotation = rigidbody.rotation;
		}
	}

	protected override void Copy2EngineTRSState(int offset)
	{
		(Vector3, Quaternion) tuple = ReadBufferAndConvertToWorldSpace(offset);
		Copy2EngineTRSState(tuple.Item1, tuple.Item2);
	}

	protected unsafe void Copy2EngineTRSState(Vector3 position, Quaternion rotation, bool forceTransform = false)
	{
		Rigidbody rigidbody = Rigidbody;
		Transform transform = base.Transform;
		forceTransform = forceTransform || (bool)transform.parent;
		rigidbody.position = position;
		if (forceTransform)
		{
			transform.position = position;
		}
		rigidbody.rotation = rotation;
		if (forceTransform)
		{
			transform.rotation = rotation;
		}
		if (SyncScale)
		{
			Vector3 localScale = ReadWriteUtils.ReadVector3(Ptr + 11, Runner._positionReadAccuracy);
			transform.localScale = localScale;
		}
	}

	private unsafe (Vector3, Quaternion) ReadBufferAndConvertToWorldSpace(int offset)
	{
		Vector3 vector = ReadWriteUtils.ReadVector3(Ptr + offset, Runner._positionReadAccuracy);
		Quaternion quaternion = Quaternion.Normalize(ReadWriteUtils.ReadQuaternion(Ptr + offset + 3, Runner._rotationReadAccuracy));
		if (Space == Spaces.Local)
		{
			Transform parent = base.Transform.parent;
			if (parent != null)
			{
				vector = parent.TransformPoint(vector);
				quaternion = parent.transform.rotation * quaternion;
			}
		}
		return (vector, quaternion);
	}

	internal unsafe override void CopyEngine2Buffers(bool posRotOnly)
	{
		Rigidbody rigidbody = Rigidbody;
		if (SyncParent)
		{
			Copy2BufferAnchorState();
		}
		Copy2BufferTRSState(4);
		if (!posRotOnly)
		{
			Copy2BuffersFlags();
			ReadWriteUtils.WriteVector3(Ptr + 25, Runner._positionWriteAccuracy, rigidbody.velocity);
			ReadWriteUtils.WriteVector3(Ptr + 28, Runner._positionWriteAccuracy, rigidbody.angularVelocity);
			if (SyncDragAndMass)
			{
				ReadWriteUtils.WriteFloat(Ptr + 32, Runner._positionWriteAccuracy, rigidbody.drag);
				ReadWriteUtils.WriteFloat(Ptr + 33, Runner._positionWriteAccuracy, rigidbody.angularDrag);
				ReadWriteUtils.WriteFloat(Ptr + 34, Runner._positionWriteAccuracy, rigidbody.mass);
			}
		}
	}

	protected unsafe override void ApplyQueuedTeleport()
	{
		Vector3? item = _queuedTeleport.Value.position;
		Rotation? item2 = _queuedTeleport.Value.rotation;
		Vector3? item3 = _queuedTeleport.Value.localScale;
		Vector3? item4 = _queuedTeleport.Value.velocity;
		Vector3? item5 = _queuedTeleport.Value.angularVelocity;
		bool item6 = _queuedTeleport.Value.reset;
		Transform transform = base.Transform;
		if (SyncParent)
		{
			Copy2BufferAnchorState(2);
		}
		Copy2BufferTRSState(14);
		if (_queuedTeleport.Value.includeParent)
		{
			transform.SetParent(_queuedTeleport.Value.parent);
		}
		if (item3.HasValue)
		{
			transform.localScale = item3.Value;
		}
		Rigidbody rigidbody = Rigidbody;
		if (item2.HasValue)
		{
			rigidbody.rotation = item2.Value;
			transform.rotation = item2.Value;
		}
		if (item.HasValue)
		{
			rigidbody.position = item.Value;
			transform.position = item.Value;
		}
		if (item4.HasValue)
		{
			rigidbody.velocity = item4.Value;
		}
		else if (item6)
		{
			rigidbody.velocity = default;
		}
		if (item5.HasValue)
		{
			rigidbody.angularVelocity = item5.Value;
		}
		else if (item6)
		{
			rigidbody.angularVelocity = default;
		}
		_queuedTeleport = null;
		(*base.TeleportCounter)++;
	}

	private bool LastPosRotMatches(Vector3 pos, Quaternion rot)
	{
		return _lastPos.HasValue && _lastPos.Value == pos && _lastRot.HasValue && _lastRot.Value == rot;
	}

	void IStateAuthorityChanged.StateAuthorityChanged()
	{
		Impl?.StateAuthorityChanged();
	}

	private bool IsRigidbodyBelowSleepingThresholds()
	{
		Rigidbody rigidbody = Rigidbody;
		float num = rigidbody.mass * rigidbody.velocity.sqrMagnitude;
		Vector3 angularVelocity = rigidbody.angularVelocity;
		Vector3 inertiaTensor = rigidbody.inertiaTensor;
		num += inertiaTensor.x * (angularVelocity.x * angularVelocity.x);
		num += inertiaTensor.y * (angularVelocity.y * angularVelocity.y);
		num += inertiaTensor.z * (angularVelocity.z * angularVelocity.z);
		num /= 2f * rigidbody.mass;
		return num <= Physics.sleepThreshold;
	}
}
