using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

[RequireComponent(typeof(Destructible))]
[RequireComponent(typeof(Rigidbody))]
public class ChainDestruction : MonoBehaviour
{
	[Tooltip("The amount of damage to apply per second to adjacent Destructible objects in the destructible chain. This will control how fast objects are destroyed.")]
	public float damagePerSecond = 125f;

	[Tooltip("If you would like to apply force on the debris pieces from a specific position point, you can assign a specific Transform location for that here. If you leave this empty, the gameObject's position will be used as the force origin point.")]
	public Transform forcePosition;

	[Tooltip("The amount of force to apply to the debris pieces (if any) when they are destroyed.")]
	public float forceAmount = 300f;

	[Tooltip("The size in game units (usually meters) of the force radius. A larger force radius will make debris pieces (if any) fly farther away from the force origin point.")]
	public float forceRadius = 5f;

	[Tooltip("The amount of upward push exerted on the debris pieces (if any). More upward push can make the force look more interesting or cinematic, but too much (say, over 2) can be unrealistic.")]
	public float forceUpwardModifier;

	[HideInInspector]
	public List<Destructible> adjacentDestructibles;

	[Tooltip("Set to TRUE to cause this Destructible object to start taking damage at the predefined damage rate (Damage Per Second).")]
	public bool destroySelf;

	private Destructible _destObj;

	private void Start()
	{
		adjacentDestructibles = new List<Destructible>();
		_destObj = base.gameObject.GetComponent<Destructible>();
		if (_destObj != null)
		{
			_destObj.DestroyedEvent += OnDestroyed;
		}
		if (!HasTriggerCollider())
		{
			Debug.LogWarning("No trigger collider found on ChainDestruction gameObject. You need a trigger collider for this script to work properly.");
		}
	}

	private void Update()
	{
		if (destroySelf && damagePerSecond > 0f)
		{
			Damage damage = new ExplosiveDamage
			{
				DamageAmount = damagePerSecond * Time.deltaTime,
				BlastForce = forceAmount,
				Position = ((forcePosition != null) ? forcePosition.position : base.transform.position),
				Radius = forceRadius,
				UpwardModifier = forceUpwardModifier
			};
			_destObj.ApplyDamage(damage);
		}
	}

	private void OnDisable()
	{
		if (!(_destObj == null))
		{
			_destObj.DestroyedEvent -= OnDestroyed;
		}
	}

	private void OnDestroyed()
	{
		if (adjacentDestructibles == null || adjacentDestructibles.Count == 0)
		{
			return;
		}
		for (int i = 0; i < adjacentDestructibles.Count; i++)
		{
			Destructible destructible = adjacentDestructibles[i];
			if (!(destructible == null))
			{
				ChainDestruction component = destructible.gameObject.GetComponent<ChainDestruction>();
				if (!(component == null))
				{
					component.destroySelf = true;
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Destructible componentInParent = other.gameObject.GetComponentInParent<Destructible>();
		if (componentInParent != null && !adjacentDestructibles.Contains(componentInParent))
		{
			adjacentDestructibles.Add(componentInParent);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Destructible componentInParent = other.gameObject.GetComponentInParent<Destructible>();
		if (componentInParent != null && adjacentDestructibles.Contains(componentInParent))
		{
			adjacentDestructibles.Remove(componentInParent);
		}
	}

	private bool HasTriggerCollider()
	{
		Collider[] components = base.gameObject.GetComponents<Collider>();
		if (components == null)
		{
			return false;
		}
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i].isTrigger)
			{
				return true;
			}
		}
		return false;
	}
}
