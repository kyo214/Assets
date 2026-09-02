using UnityEngine;

public class EnemyBulletCollider : MonoBehaviour
{
	[SerializeField]
	private EnemyController enemy;

	private void Start()
	{
		Physics.IgnoreCollision(GetComponent<Collider>(), enemy.enemyCollider);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("PlayerCollider"))
		{
			PlayerController component = other.gameObject.GetComponent<PlayerController>();
			if (component.network.GetHealth() > 0f && !component.isDashing && !component.isHurt && !GameManagerPhoton.Instance.IsWin && component.network.isLocalPlayer)
			{
				component.network.ExecHurtEffect(component.network.GetIDX());
				component.network.AddSubHealth(0f - enemy.data.damage);
				CameraGame.Instance.CameraShake();
			}
		}
	}
}
