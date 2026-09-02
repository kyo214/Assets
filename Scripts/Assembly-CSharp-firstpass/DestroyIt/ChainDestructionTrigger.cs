using UnityEngine;

namespace DestroyIt;

[RequireComponent(typeof(Collider))]
public class ChainDestructionTrigger : MonoBehaviour
{
	public ChainDestruction[] chainDestructions;

	private void Start()
	{
		if (!HasTriggerCollider())
		{
			Debug.LogWarning("No trigger collider found on ChainDestructionTrigger gameObject. You need a trigger collider for this script to work properly.");
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (chainDestructions == null || chainDestructions.Length == 0)
		{
			return;
		}
		for (int i = 0; i < chainDestructions.Length; i++)
		{
			if (!(chainDestructions[i] == null))
			{
				chainDestructions[i].destroySelf = true;
			}
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
