using UnityEngine;

public class SlowEnemyArea : MonoBehaviour
{
	[SerializeField]
	private float slowSpeedMultiplier = 0.6f;

	[SerializeField]
	private bool _isForNormalZombieOnly = true;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("EnemyFOV"))
		{
			EnemyController component = other.transform.parent.GetComponent<EnemyController>();
			if (component != null && (!_isForNormalZombieOnly || !component.isElite))
			{
				component.SetMultiplySpeed(slowSpeedMultiplier);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("EnemyFOV"))
		{
			EnemyController component = other.transform.parent.GetComponent<EnemyController>();
			if (component != null)
			{
				component.SetMultiplySpeed(1f);
			}
		}
	}
}
