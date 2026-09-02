using System;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[OrderBefore(new Type[] { typeof(NetworkTransform) })]
[DisallowMultipleComponent]
[NetworkBehaviourWeaved(24)]
public class NetworkCharacterControllerPrototype : NetworkTransform
{
	[Header("Character Controller Settings")]
	public float gravity = -20f;

	public float jumpImpulse = 8f;

	public float acceleration = 10f;

	public float braking = 10f;

	public float maxSpeed = 2f;

	public float rotationSpeed = 15f;

	[HideInInspector]
	[SerializeField]
	[DefaultForProperty("IsGrounded", 20, 1)]
	private bool _IsGrounded;

	[HideInInspector]
	[SerializeField]
	[DefaultForProperty("Velocity", 21, 3)]
	private Vector3 _Velocity;

	private static Changed<NetworkCharacterControllerPrototype> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<NetworkCharacterControllerPrototype> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<NetworkCharacterControllerPrototype> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	[Networked]
	[HideInInspector]
	[NetworkedWeaved(20, 1)]
	public unsafe bool IsGrounded
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing NetworkCharacterControllerPrototype.IsGrounded. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 20);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing NetworkCharacterControllerPrototype.IsGrounded. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 20, value);
		}
	}

	[Networked]
	[HideInInspector]
	[NetworkedWeaved(21, 3)]
	public unsafe Vector3 Velocity
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing NetworkCharacterControllerPrototype.Velocity. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadVector3(Ptr + 21, 0.001f);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing NetworkCharacterControllerPrototype.Velocity. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteVector3(Ptr + 21, 999.99994f, value);
		}
	}

	protected override Vector3 DefaultTeleportInterpolationVelocity => Velocity;

	protected override Vector3 DefaultTeleportInterpolationAngularVelocity => new Vector3(0f, 0f, rotationSpeed);

	public CharacterController Controller { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		CacheController();
	}

	public override void Spawned()
	{
		base.Spawned();
		CacheController();
	}

	private void CacheController()
	{
		if (Controller == null)
		{
			Controller = GetComponent<CharacterController>();
		}
	}

	protected override void CopyFromBufferToEngine()
	{
		Controller.enabled = false;
		base.CopyFromBufferToEngine();
		Controller.enabled = true;
	}

	public virtual void Jump(bool ignoreGrounded = false, float? overrideImpulse = null)
	{
		if (IsGrounded | ignoreGrounded)
		{
			Vector3 velocity = Velocity;
			velocity.y += overrideImpulse ?? jumpImpulse;
			Velocity = velocity;
		}
	}

	public virtual void Move(Vector3 direction)
	{
		float deltaTime = Runner.DeltaTime;
		Vector3 position = base.transform.position;
		Vector3 velocity = Velocity;
		direction = direction.normalized;
		if (IsGrounded && velocity.y < 0f)
		{
			velocity.y = 0f;
		}
		velocity.y += gravity * Runner.DeltaTime;
		Vector3 vector = new Vector3
		{
			x = velocity.x,
			z = velocity.z
		};
		if (direction == default(Vector3))
		{
			vector = Vector3.Lerp(vector, default, braking * deltaTime);
		}
		else
		{
			vector = Vector3.ClampMagnitude(vector + direction * acceleration * deltaTime, maxSpeed);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Runner.DeltaTime);
		}
		velocity.x = vector.x;
		velocity.z = vector.z;
		Controller.Move(velocity * deltaTime);
		Velocity = (base.transform.position - position) * Runner.Simulation.Config.TickRate;
		IsGrounded = Controller.isGrounded;
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		base.CopyBackingFieldsToState(P_0);
		IsGrounded = _IsGrounded;
		Velocity = _Velocity;
	}

	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		_IsGrounded = IsGrounded;
		_Velocity = Velocity;
	}
}
