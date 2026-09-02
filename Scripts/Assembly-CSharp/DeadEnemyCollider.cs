using UnityEngine;

public class DeadEnemyCollider : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("EnemyLightCollider"))
		{
			other.GetComponent<EnemyLightCollider>().enemyController.Hurt(999f, 0f, execShakingCam: true, 0, 0);
		}
	}
}
