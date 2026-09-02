using Toked.StatusEffect;
using UnityEngine;

public class PoisonArea : MonoBehaviour
{
	[SerializeField]
	private string poisonType = "Sanity";

	[SerializeField]
	private int type = 1;

	[SerializeField]
	private float decreasePerSec = 1f;

	[SerializeField]
	private float posionDuration = 10f;

	[SerializeField]
	private StatusEffectScriptableObject _statusEffectScriptableObject;

	private void Start()
	{
		decreasePerSec = BGDatabase_GameConfig.GetEntity("Default").DecSanity1ValuePerSec;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("PlayerCollider"))
		{
			PlayerController component = other.GetComponent<PlayerController>();
			if ((bool)component && component.network.isLocalPlayer)
			{
				component.GetComponent<StatusEffectController>().ApplyStatus(component, _statusEffectScriptableObject);
			}
		}
	}
}
