using System;
using UnityEngine;

namespace Fusion;

[NetworkBehaviourWeaved(-1)]
[SimulationBehaviour(Stages = (SimulationStages.Forward | SimulationStages.Resimulate))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
[Obsolete("This class has been replaced by a new NetworkRigidbody2D class and is now obsolete.")]
public class NetworkRigidbodyObsolete2D : NetworkRigidbodyBaseObsolete, IStateAuthorityChanged
{
	private Vector3? _lastPos;

	private float? _lastRot;

	private Rigidbody2D _rigidbody2D;

	public Rigidbody2D Rigidbody => _rigidbody2D ? _rigidbody2D : (_rigidbody2D = GetComponent<Rigidbody2D>());

	internal override void SetIsKinematic(bool value)
	{
		_rigidbody2D.isKinematic = value;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if ((bool)_rigidbody2D)
		{
			_rigidbody2D.WakeUp();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		TryGetComponent<Rigidbody2D>(out _rigidbody2D);
	}

	public override void CopyBackingFieldsToState(bool firstTime)
	{
		Transform transform = base.Transform;
		Rigidbody2D rigidbody = Rigidbody;
		rigidbody.position = transform.position;
		rigidbody.rotation = transform.rotation.eulerAngles.z;
		base.CopyBackingFieldsToState(firstTime);
	}

	public override void Spawned()
	{
		if (Runner.Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.Physics3D)
		{
			base.enabled = false;
			Log.Error("NetworkRigidbodyObsolete2D found while PhysicsModes is set to 3D. Automatically disabling NetworkRigidbodyObsolete2D.");
			return;
		}
		base.Spawned();
		base.Transform.position = _rigidbody2D.position;
		base.Transform.rotation = Quaternion.Euler(0f, 0f, _rigidbody2D.rotation);
		if ((bool)_rigidbody2D)
		{
			_rigidbody2D.interpolation = RigidbodyInterpolation2D.None;
		}
	}

	internal unsafe override void CopyBuffers2Engine(bool posRotOnly)
	{
		if (SyncParent)
		{
			Copy2EngineAnchorState();
		}
		Rigidbody2D rigidbody = Rigidbody;
		Transform transform = base.Transform;
		if (posRotOnly)
		{
			(Vector3, float) tuple = ReadBufferAndConvertToWorldSpace(4);
			rigidbody.position = tuple.Item1;
			rigidbody.rotation = tuple.Item2;
			return;
		}
		int num = Ptr[31];
		bool flag = (num & 1) == 1;
		bool flag2 = (num & 8) == 8;
		RigidbodyConstraints2D rigidbodyConstraints2D = (RigidbodyConstraints2D)(num >> 4);
		if (rigidbody.simulated != flag2)
		{
			rigidbody.simulated = flag2;
		}
		if (rigidbody.constraints != rigidbodyConstraints2D)
		{
			rigidbody.constraints = rigidbodyConstraints2D;
		}
		if (rigidbody.isKinematic != flag)
		{
			rigidbody.isKinematic = flag;
		}
		if (SyncDragAndMass)
		{
			rigidbody.drag = ReadWriteUtils.ReadFloat(Ptr + 32, Runner._positionReadAccuracy);
			rigidbody.angularDrag = ReadWriteUtils.ReadFloat(Ptr + 33, Runner._positionReadAccuracy);
			rigidbody.mass = ReadWriteUtils.ReadFloat(Ptr + 34, Runner._positionReadAccuracy);
		}
		bool flag3 = (num & 4) == 4;
		if (flag3 && rigidbody.IsSleeping())
		{
			return;
		}
		var (vector, num2) = ReadBufferAndConvertToWorldSpace(4);
		if (flag3 && (double)rigidbody.velocity.sqrMagnitude < 0.1 && rigidbody.angularVelocity < 0.01f)
		{
			if (rigidbody.position != (Vector2)vector && !LastPosRotMatches(vector, num2))
			{
				rigidbody.position = vector;
				rigidbody.rotation = num2;
				rigidbody.velocity = default;
				rigidbody.angularVelocity = 0f;
				_lastPos = vector;
				_lastRot = num2;
			}
		}
		else
		{
			rigidbody.position = vector;
			rigidbody.rotation = num2;
			transform.rotation = Quaternion.Euler(0f, 0f, num2);
			rigidbody.velocity = ReadWriteUtils.ReadVector2(Ptr + 25, Runner._positionReadAccuracy);
			rigidbody.angularVelocity = ReadWriteUtils.ReadFloat(Ptr + 28, Runner._positionReadAccuracy);
		}
	}

	internal unsafe override void Copy2BuffersFlags()
	{
		int num = (int)_rigidbody2D.constraints << 4;
		if (_rigidbody2D.isKinematic)
		{
			num |= 1;
		}
		if (_rigidbody2D.simulated)
		{
			num |= 8;
		}
		if (_rigidbody2D.IsSleeping())
		{
			num |= 4;
		}
		Ptr[31] = num;
	}

	protected unsafe override void Copy2BufferTRSState(int offset)
	{
		Rigidbody2D rigidbody = Rigidbody;
		Transform transform = base.Transform;
		Vector2 vector = rigidbody.position;
		float num = rigidbody.rotation;
		if (Space == Spaces.Local)
		{
			Transform parent = transform.parent;
			if ((bool)parent)
			{
				vector = parent.InverseTransformPoint(vector);
				num = 0f - parent.rotation.z + num;
			}
		}
		ReadWriteUtils.WriteVector3(Ptr + offset, Runner._positionWriteAccuracy, vector);
		ReadWriteUtils.WriteFloat(Ptr + offset + 3 + 2, Runner._rotationWriteAccuracy, num);
	}

	protected override void Copy2EngineTRSState(int offset)
	{
		Rigidbody2D rigidbody = Rigidbody;
		(Vector3, float) tuple = ReadBufferAndConvertToWorldSpace(offset);
		rigidbody.position = tuple.Item1;
		rigidbody.rotation = tuple.Item2;
	}

	protected unsafe (Vector3, float) ReadBufferAndConvertToWorldSpace(int offset)
	{
		Vector3 vector = ReadWriteUtils.ReadVector3(Ptr + offset, Runner._positionReadAccuracy);
		float num = ReadWriteUtils.ReadFloat(Ptr + offset + 3 + 2, Runner._rotationReadAccuracy);
		if (Space == Spaces.Local)
		{
			Transform parent = base.Transform.parent;
			if (parent != null)
			{
				vector = parent.TransformPoint(vector);
				num = parent.transform.rotation.z + num;
			}
		}
		return (vector, num);
	}

	internal unsafe override void CopyEngine2Buffers(bool posRotOnly)
	{
		if (SyncParent)
		{
			Copy2BufferAnchorState();
		}
		Copy2BufferTRSState(4);
		if (!posRotOnly)
		{
			Copy2BuffersFlags();
			Rigidbody2D rigidbody = Rigidbody;
			ReadWriteUtils.WriteVector2(Ptr + 25, Runner._positionWriteAccuracy, rigidbody.velocity);
			ReadWriteUtils.WriteFloat(Ptr + 28, Runner._positionWriteAccuracy, rigidbody.angularVelocity);
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
		Vector3? item3 = _queuedTeleport.Value.velocity;
		Vector3? item4 = _queuedTeleport.Value.angularVelocity;
		bool item5 = _queuedTeleport.Value.reset;
		Rigidbody2D rigidbody = Rigidbody;
		_queuedTeleport = null;
		Copy2BufferTRSState(14);
		if (item.HasValue)
		{
			rigidbody.position = item.Value;
		}
		if (item2.HasValue)
		{
			rigidbody.rotation = item2.Value;
		}
		if (item3.HasValue)
		{
			rigidbody.velocity = item3.Value;
		}
		else if (item5)
		{
			rigidbody.velocity = default;
		}
		if (item4.HasValue)
		{
			rigidbody.angularVelocity = item4.Value.z;
		}
		else if (item5)
		{
			rigidbody.angularVelocity = 0f;
		}
		*base.TeleportCounter = *base.TeleportCounter + 1;
	}

	private bool LastPosRotMatches(Vector3 pos, float rot)
	{
		return _lastPos.HasValue && _lastPos.Value == pos && _lastRot.HasValue && _lastRot.Value == rot;
	}

	protected unsafe override Quaternion ReadRotationToQuaternion(int* offset, ReadAccuracy readAccuracy)
	{
		float z = ReadWriteUtils.ReadFloat(offset + 2, readAccuracy);
		return Quaternion.Euler(0f, 0f, z);
	}

	void IStateAuthorityChanged.StateAuthorityChanged()
	{
		Impl?.StateAuthorityChanged();
	}
}
