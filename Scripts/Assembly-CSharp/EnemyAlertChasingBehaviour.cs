using UnityEngine;

public class EnemyAlertChasingBehaviour : StateMachineBehaviour
{
	[SerializeField]
	private EnemyController enemyController;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (enemyController == null)
		{
			enemyController = animator.transform.parent.GetComponent<EnemyController>();
		}
		if (NetworkGameManager.Instance.isServer && enemyController.attack.targetChasing == null)
		{
			enemyController.attack.fov.enabled = false;
			enemyController.AIEnable = false;
			enemyController.SetEnableAI(value: false);
			enemyController.SetAISpeed(0f);
			enemyController.SetState(EnemyState.Idle);
			enemyController.network.SetAnimation("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			if (enemyController.attack.targetChasing != null)
			{
				enemyController.movement.angleAnim = enemyController.attack.AngleEnemy((enemyController.attack.targetChasing.transform.position - enemyController.attack.transform.position).normalized, enemyController.movement.angleAnim);
			}
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!NetworkGameManager.Instance.isServer || !(enemyController.network.GetHealth() > 0f) || enemyController.isDead || enemyController.isDown || enemyController.attack.timerDelayChasing.isRunning)
		{
			return;
		}
		if (enemyController.attack.targetChasing == null)
		{
			PlayerController playerNearest = NetworkGameManager.Instance.GetPlayerNearest(isHaveHealth: true, enemyController.transform.position);
			if (playerNearest != null)
			{
				enemyController.attack.targetChasing = playerNearest.targetedPoint;
				enemyController.attack.EnemyAlertAndChasing();
			}
			else
			{
				enemyController.attack.SetStateToPatrol();
			}
		}
		else
		{
			enemyController.attack.EnemyAlertAndChasing();
		}
	}
}
