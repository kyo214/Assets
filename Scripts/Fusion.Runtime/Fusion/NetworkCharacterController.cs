using System;
using UnityEngine;

namespace Fusion;

[NetworkBehaviourWeaved(30)]
[DisallowMultipleComponent]
[Obsolete("This class has been deprecated. Use the new NetworkCharacterControllerPrototype, which uses Unity's CharacterController, or the Kinematic Character Controller package.")]
public sealed class NetworkCharacterController : NetworkTransformObsolete
{
	public interface ICallbacks
	{
		bool OnCharacterCollision3D(Hit other);

		void OnCharacterTrigger3D(Hit other);
	}

	public struct Hit
	{
		public Collider Collider;

		public Vector3 Normal;

		public float Penetration;

		internal float SortingDistance;

		public bool IsBump;
	}

	public enum BumpBehavior
	{
		AlwaysContact = 0,
		IgnoreOnStanding = 1,
		IgnoreOnBackside = 2
	}

	public enum MovementType
	{
		None = 0,
		FreeFall = 1,
		SlopeFall = 2,
		Horizontal = 3
	}

	[Serializable]
	public class Configuration
	{
		public float AllowedPenetration = 0.1f;

		public float PenetrationCorrection = 0.5f;

		public BumpBehavior BumpBehavior = BumpBehavior.IgnoreOnStanding;

		public LayerMask LayerMask = -1;

		public QueryTriggerInteraction TriggerInteraction = QueryTriggerInteraction.UseGlobal;

		public bool AirControl;

		public float Acceleration = 10f;

		public float Braking = 10f;

		public float BaseJumpImpulse = 5f;

		public float MaxSpeed = 4f;

		public float MaxSlope = 60f;

		public float MaxSlopeSpeed = 5f;

		[SerializeField]
		private Vector3 _gravity = new Vector3(0f, -9.81f, 0f);

		[NonSerialized]
		internal float _gravityStrength;

		[NonSerialized]
		internal Vector3 _gravityNormalized;

		public Vector3 Gravity
		{
			get
			{
				return _gravity;
			}
			set
			{
				_gravity = value;
				Init();
			}
		}

		internal void Init()
		{
			_gravityStrength = _gravity.magnitude;
			_gravityNormalized = _gravity.normalized;
		}
	}

	[Serializable]
	public struct Movement
	{
		public MovementType Type;

		public Vector3 NearestNormal;

		public Vector3 AvgNormal;

		public Vector3 GroundNormal;

		public Vector3 Tangent;

		public Vector3 SlopeTangent;

		public Vector3 Correction;

		public float Penetration;

		public bool Grounded;

		public int Contacts;
	}

	private const int DEFAULT_HITS_CAPACITY = 128;

	private const int FLAGS_WORD_COUNT = 1;

	private const int GROUNDED_FLAG = 1;

	private const int JUMPED_FLAG = 2;

	private const int FLOAT_WORD_COUNT = 1;

	private const int VECTOR_WORD_COUNT = 3;

	private const int BITFLAGS_OFFSET = 25;

	private const int MAX_SPEED_OFFSET = 26;

	private const int VELOCITY_OFFSET = 27;

	private const int NCC_WORD_COUNT = 30;

	private const int ROTATION_WORD_COUNT = 4;

	[SerializeField]
	private Collider _collider;

	[SerializeField]
	public Configuration Config;

	private Vector3 _offsetPosition;

	[SerializeField]
	private Movement LastMovement;

	private static Collider[] _hitsColliders;

	private static Hit[] _hits;

	public override int PositionWordOffset => 4;

	public unsafe bool Grounded
	{
		get
		{
			return (Ptr[25] & 1) == 1;
		}
		set
		{
			if (value)
			{
				Ptr[25] |= 1;
			}
			else
			{
				Ptr[25] &= -2;
			}
		}
	}

	public unsafe bool Jumped
	{
		get
		{
			return (Ptr[25] & 2) == 2;
		}
		set
		{
			if (value)
			{
				Ptr[25] |= 2;
			}
			else
			{
				Ptr[25] &= -3;
			}
		}
	}

	public unsafe float MaxSpeed
	{
		get
		{
			return ReadWriteUtils.ReadFloat(Ptr + 26, Runner._positionReadAccuracy);
		}
		set
		{
			ReadWriteUtils.WriteFloat(Ptr + 26, Runner._positionWriteAccuracy, value);
		}
	}

	public unsafe Vector3 Velocity
	{
		get
		{
			return ReadWriteUtils.ReadVector3(Ptr + 27, Runner._positionReadAccuracy);
		}
		set
		{
			ReadWriteUtils.WriteVector3(Ptr + 27, Runner._positionWriteAccuracy, value);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (!_interpolationTarget)
		{
			_interpolationTarget = base.transform;
		}
		if (!_collider)
		{
			_collider = base.transform.GetNestedComponentInChildren<Collider, NetworkObject>(includeInactive: true);
		}
	}

	protected override void Reset()
	{
		base.Reset();
		_collider = base.transform.GetNestedComponentInChildren<Collider, NetworkObject>(includeInactive: true);
	}

	protected unsafe override void ApplyQueuedTeleport()
	{
		Vector3? item = _queuedTeleport.Value.position;
		Rotation? item2 = _queuedTeleport.Value.rotation;
		Vector3? item3 = _queuedTeleport.Value.localScale;
		Vector3? item4 = _queuedTeleport.Value.velocity;
		bool item5 = _queuedTeleport.Value.reset;
		_queuedTeleport = null;
		Copy2BufferTRSState(14);
		if (item.HasValue)
		{
			base.Transform.position = item.Value;
		}
		if (item2.HasValue)
		{
			base.Transform.rotation = item2.Value;
		}
		if (item3.HasValue)
		{
			base.Transform.localScale = item3.Value;
		}
		*base.TeleportCounter = *base.TeleportCounter + 1;
		if (item4.HasValue)
		{
			Velocity = item4.Value;
		}
		if (item5)
		{
			if (!item4.HasValue)
			{
				Velocity = default;
			}
			Grounded = false;
			Jumped = false;
		}
	}

	public override void Spawned()
	{
		base.Spawned();
		Config.Init();
		Grounded = false;
		Velocity = default;
		MaxSpeed = Config.MaxSpeed;
	}

	public void Jump(bool ignoreGrounded = false, float? impulse = null)
	{
		if (Grounded | ignoreGrounded)
		{
			Vector3 velocity = Velocity;
			velocity.y = (impulse.HasValue ? impulse.Value : Config.BaseJumpImpulse);
			Velocity = velocity;
			Jumped = true;
		}
	}

	public void Move(Vector3 direction, ICallbacks callback = null, LayerMask? layerMask = null)
	{
		float deltaTime = Runner.DeltaTime;
		Movement movementPack = ComputeRawMovement(direction, callback, layerMask);
		ComputeRawSteer(ref movementPack, deltaTime);
		Vector3 vector = Velocity * deltaTime;
		if (movementPack.Penetration > float.Epsilon)
		{
			vector += movementPack.Correction;
		}
		base.Transform.position += vector;
		LastMovement = movementPack;
	}

	public Movement ComputeRawMovement(Vector3 direction, ICallbacks callback = null, LayerMask? layerMask = null)
	{
		if (_hitsColliders == null || _hits == null)
		{
			_hitsColliders = new Collider[128];
			_hits = new Hit[128];
		}
		Movement movement = new Movement
		{
			Type = MovementType.FreeFall,
			Tangent = direction
		};
		Vector3 position = base.Transform.position;
		PhysicsScene physicsScene = Runner.GetPhysicsScene();
		int num = 0;
		if (_collider is CapsuleCollider)
		{
			CapsuleCollider capsuleCollider = _collider as CapsuleCollider;
			float num2 = capsuleCollider.height / 2f;
			position += capsuleCollider.center;
			num = physicsScene.OverlapCapsule(position + Vector3.down * num2, position + Vector3.up * num2, capsuleCollider.radius, _hitsColliders, layerMask.GetValueOrDefault(Config.LayerMask), Config.TriggerInteraction);
		}
		else if (_collider is SphereCollider)
		{
			SphereCollider sphereCollider = _collider as SphereCollider;
			position += sphereCollider.center;
			num = physicsScene.OverlapSphere(position, sphereCollider.radius, _hitsColliders, layerMask.GetValueOrDefault(Config.LayerMask), Config.TriggerInteraction);
		}
		else
		{
			if (!(_collider is BoxCollider))
			{
				throw new Exception("CC only works with Capsule, Sphere or Box collider types");
			}
			BoxCollider boxCollider = _collider as BoxCollider;
			position += boxCollider.center;
			num = physicsScene.OverlapBox(position, boxCollider.size / 2f, _hitsColliders, base.transform.rotation, layerMask.GetValueOrDefault(Config.LayerMask), Config.TriggerInteraction);
		}
		_offsetPosition = position;
		int num3 = 0;
		for (int i = 0; i < num; i++)
		{
			Collider collider = _hitsColliders[i];
			if (Physics.ComputePenetration(_collider, base.transform.position, base.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out var direction2, out var distance))
			{
				_hits[num3].Collider = collider;
				_hits[num3].Normal = direction2;
				_hits[num3].Penetration = distance;
				_hits[num3].SortingDistance = 5f - distance;
				num3++;
			}
		}
		QuickSort(_hits, 0, num3 - 1);
		int num4 = num3;
		if (num4 > 0)
		{
			bool flag = false;
			for (int j = 0; j < num3; j++)
			{
				if (num4 <= 0)
				{
					break;
				}
				if (_collider == _hits[j].Collider)
				{
					continue;
				}
				Hit other = _hits[j];
				if (other.Collider.isTrigger)
				{
					if (callback != null && other.Penetration > 0f)
					{
						callback.OnCharacterTrigger3D(other);
					}
					continue;
				}
				Vector3 normal = other.Normal;
				if (other.Penetration <= Config.AllowedPenetration)
				{
					if (movement.Grounded)
					{
						break;
					}
					float num5 = Vector3.Angle(-Config._gravityNormalized, normal);
					if (num5 <= Config.MaxSlope)
					{
						movement.Grounded = true;
						movement.GroundNormal = normal;
						movement.Penetration = 0f;
						break;
					}
					num4--;
					continue;
				}
				bool flag2 = true;
				if (other.Collider.TryGetComponent<NetworkCharacterController>(out var _))
				{
					BumpBehavior bumpBehavior = Config.BumpBehavior;
					BumpBehavior bumpBehavior2 = bumpBehavior;
					if ((uint)(bumpBehavior2 - 1) <= 1u && Velocity == default(Vector3))
					{
						other.IsBump = true;
						flag2 = false;
					}
				}
				if (callback != null)
				{
					flag2 = callback.OnCharacterCollision3D(_hits[j]);
				}
				if (!flag2)
				{
					continue;
				}
				num4--;
				if (!flag)
				{
					movement.NearestNormal = normal;
					movement.Penetration = other.Penetration - Config.AllowedPenetration;
				}
				if (other.Penetration > Config.AllowedPenetration)
				{
					float num6 = Vector3.Angle(-Config._gravityNormalized, normal);
					if (num6 <= Config.MaxSlope)
					{
						if (!flag || !movement.Grounded)
						{
							float num7 = Vector3.Dot(direction, normal);
							movement.Tangent = (direction - normal * num7).normalized;
							movement.GroundNormal = normal;
						}
						movement.Grounded = true;
						if (direction != default(Vector3))
						{
							movement.Type = MovementType.Horizontal;
						}
						else
						{
							movement.Type = MovementType.None;
						}
					}
					else if (!flag)
					{
						float num8 = Vector3.Dot(Config._gravityNormalized, normal);
						if (num8 < 0f)
						{
							movement.SlopeTangent = (Config._gravityNormalized - normal * num8).normalized;
							movement.Type = MovementType.SlopeFall;
						}
					}
					movement.Contacts++;
					Vector3 correction = normal * (other.Penetration - Config.AllowedPenetration);
					if (!flag)
					{
						movement.Correction = correction;
					}
					else
					{
						Vector3 normalized = correction.normalized;
						float magnitude = correction.magnitude;
						float num9 = Vector3.Dot(movement.Correction, normalized);
						if (Mathf.Abs(num9) < magnitude)
						{
							movement.Correction += normalized * (magnitude - num9);
						}
					}
					movement.AvgNormal += normal;
				}
				flag = true;
			}
			if (movement.Contacts > 1)
			{
				movement.AvgNormal /= (float)movement.Contacts;
			}
		}
		LastMovement = movement;
		return movement;
	}

	private void ComputeRawSteer(ref Movement movementPack, float dt)
	{
		Grounded = movementPack.Grounded;
		float min = -100f;
		float max = 100f;
		Vector3 velocity = Velocity;
		switch (movementPack.Type)
		{
		case MovementType.FreeFall:
			velocity.y -= Config._gravityStrength * dt;
			if (!Config.AirControl || movementPack.Tangent == default(Vector3))
			{
				velocity.x = Mathf.Lerp(velocity.x, 0f, dt * Config.Braking);
				velocity.z = Mathf.Lerp(velocity.z, 0f, dt * Config.Braking);
			}
			else
			{
				velocity += movementPack.Tangent * Config.Acceleration * dt;
			}
			break;
		case MovementType.Horizontal:
		{
			velocity += movementPack.Tangent * Config.Acceleration * dt;
			float num = Vector3.Dot(velocity, movementPack.Tangent);
			Vector3 vector = num * movementPack.Tangent;
			float t2 = Config.Braking * dt;
			velocity.x = Mathf.Lerp(velocity.x, vector.x, t2);
			velocity.z = Mathf.Lerp(velocity.z, vector.z, t2);
			if (!Jumped)
			{
				velocity.y = Mathf.Lerp(velocity.y, vector.y, t2);
			}
			if (num > MaxSpeed)
			{
				velocity -= movementPack.Tangent * (num - MaxSpeed);
			}
			break;
		}
		case MovementType.SlopeFall:
			velocity += movementPack.SlopeTangent * Config.Acceleration * dt;
			min = 0f - Config.MaxSlopeSpeed;
			break;
		case MovementType.None:
		{
			float t = dt * Config.Braking;
			if (velocity.x != 0f)
			{
				velocity.x = Mathf.Lerp(velocity.x, 0f, t);
				if (Mathf.Abs(velocity.x) < float.Epsilon)
				{
					velocity.x = 0f;
				}
			}
			if (velocity.z != 0f)
			{
				velocity.z = Mathf.Lerp(velocity.z, 0f, t);
				if (Mathf.Abs(velocity.z) < float.Epsilon)
				{
					velocity.z = 0f;
				}
			}
			if (velocity.y != 0f && !Jumped)
			{
				velocity.y = Mathf.Lerp(velocity.y, 0f, t);
				if (Mathf.Abs(velocity.y) < float.Epsilon)
				{
					velocity.y = 0f;
				}
			}
			min = 0f;
			break;
		}
		}
		if (movementPack.Type != MovementType.Horizontal)
		{
			Vector2 vector2 = new Vector2(velocity.x, velocity.z);
			if (vector2.sqrMagnitude > MaxSpeed * MaxSpeed)
			{
				vector2 = vector2.normalized * MaxSpeed;
			}
			velocity.x = vector2.x;
			velocity.y = Mathf.Clamp(velocity.y, min, max);
			velocity.z = vector2.y;
		}
		Velocity = velocity;
		Jumped = false;
	}

	private static void QuickSort(Hit[] A, int lo, int hi)
	{
		if (lo >= hi)
		{
			return;
		}
		float sortingDistance = A[hi].SortingDistance;
		int num = lo;
		Hit hit;
		for (int i = lo; i < hi; i++)
		{
			if (A[i].SortingDistance < sortingDistance)
			{
				hit = A[num];
				A[num] = A[i];
				A[i] = hit;
				num++;
			}
		}
		hit = A[num];
		A[num] = A[hi];
		A[hi] = hit;
		int num2 = num;
		QuickSort(A, lo, num2 - 1);
		QuickSort(A, num2 + 1, hi);
	}
}
