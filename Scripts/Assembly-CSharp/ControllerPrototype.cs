using System;
using Fusion;
using UnityEngine;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
[NetworkBehaviourWeaved(3)]
public class ControllerPrototype : NetworkBehaviour
{
	protected NetworkCharacterControllerPrototype _ncc;

	protected NetworkRigidbody _nrb;

	protected NetworkRigidbody2D _nrb2d;

	protected NetworkTransform _nt;

	[SerializeField]
	[DefaultForProperty("MovementDirection", 0, 3)]
	private Vector3 _MovementDirection;

	public bool TransformLocal;

	[DrawIf("ShowSpeed", Hide = true)]
	public float Speed = 6f;

	private static Changed<ControllerPrototype> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<ControllerPrototype> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<ControllerPrototype> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	[Networked]
	[NetworkedWeaved(0, 3)]
	public unsafe Vector3 MovementDirection
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ControllerPrototype.MovementDirection. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadVector3((int*)((byte*)Ptr + 0), 0.001f);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ControllerPrototype.MovementDirection. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteVector3((int*)((byte*)Ptr + 0), 999.99994f, value);
		}
	}

	private bool ShowSpeed
	{
		get
		{
			NetworkCharacterControllerPrototype component;
			if ((bool)this)
			{
				return !TryGetComponent<NetworkCharacterControllerPrototype>(out component);
			}
			return false;
		}
	}

	public void Awake()
	{
		CacheComponents();
	}

	public override void Spawned()
	{
		CacheComponents();
	}

	private void CacheComponents()
	{
		if (!_ncc)
		{
			_ncc = GetComponent<NetworkCharacterControllerPrototype>();
		}
		if (!_nrb)
		{
			_nrb = GetComponent<NetworkRigidbody>();
		}
		if (!_nrb2d)
		{
			_nrb2d = GetComponent<NetworkRigidbody2D>();
		}
		if (!_nt)
		{
			_nt = GetComponent<NetworkTransform>();
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (Runner.Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.None)
		{
			return;
		}
		Vector3 vector;
		if (GetInput<NetworkInputPrototype>(out var input))
		{
			vector = default;
			if (input.IsDown(3))
			{
				vector += (TransformLocal ? base.transform.forward : Vector3.forward);
			}
			if (input.IsDown(4))
			{
				vector -= (TransformLocal ? base.transform.forward : Vector3.forward);
			}
			if (input.IsDown(5))
			{
				vector -= (TransformLocal ? base.transform.right : Vector3.right);
			}
			if (input.IsDown(6))
			{
				vector += (TransformLocal ? base.transform.right : Vector3.right);
			}
			vector = (MovementDirection = vector.normalized);
			if (input.IsDown(7))
			{
				if ((bool)_ncc)
				{
					_ncc.Jump();
				}
				else
				{
					vector += (TransformLocal ? base.transform.up : Vector3.up);
				}
			}
		}
		else
		{
			vector = MovementDirection;
		}
		if ((bool)_ncc)
		{
			_ncc.Move(vector);
		}
		else if ((bool)_nrb && !_nrb.Rigidbody.isKinematic)
		{
			_nrb.Rigidbody.AddForce(vector * Speed);
		}
		else if ((bool)_nrb2d && !_nrb2d.Rigidbody.isKinematic)
		{
			Vector2 vector2 = new Vector2(vector.x, vector.y + vector.z);
			_nrb2d.Rigidbody.AddForce(vector2 * Speed);
		}
		else
		{
			base.transform.position += vector * Speed * Runner.DeltaTime;
		}
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		MovementDirection = _MovementDirection;
	}

	public override void CopyStateToBackingFields()
	{
		_MovementDirection = MovementDirection;
	}
}
