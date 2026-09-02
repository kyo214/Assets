using UnityEngine;

namespace DestroyIt;

public class ActivateDestructibles : MonoBehaviour
{
	private DestructionManager _destructionManager;

	private void Start()
	{
		_destructionManager = DestructionManager.Instance;
		if (_destructionManager == null)
		{
			Debug.LogError("DestructionManager could not be found or created in the scene. Removing script.");
			Object.Destroy(this);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.CompareTag("Player"))
		{
			Destructible componentInParent = other.gameObject.GetComponentInParent<Destructible>();
			if (!(componentInParent == null) && (!componentInParent.isTerrainTree || !_destructionManager.destructibleTreesStayDeactivated) && !componentInParent.enabled)
			{
				componentInParent.enabled = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			return;
		}
		Destructible componentInParent = other.gameObject.GetComponentInParent<Destructible>();
		if (!(componentInParent == null))
		{
			if (componentInParent.enabled && !componentInParent.isTerrainTree && _destructionManager.autoDeactivateDestructibles)
			{
				componentInParent.shouldDeactivate = true;
			}
			else if (componentInParent.enabled && componentInParent.isTerrainTree && _destructionManager.autoDeactivateDestructibleTerrainObjects)
			{
				componentInParent.shouldDeactivate = true;
			}
		}
	}
}
