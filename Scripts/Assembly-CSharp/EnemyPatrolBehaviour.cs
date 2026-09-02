using UnityEngine;

public class EnemyPatrolBehaviour : StateMachineBehaviour
{
	[SerializeField]
	private EnemyController enemyController;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (enemyController == null)
		{
			enemyController = animator.transform.parent.GetComponent<EnemyController>();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!NetworkGameManager.Instance.isServer || !(enemyController.network.GetHealth() > 0f) || enemyController.isDead || enemyController.isDown)
		{
			return;
		}
		EnemyAttack attack = enemyController.attack;
		if (attack.fov.visibleTargets.Count > 0)
		{
			attack.StartChasing(playerSighted: true);
		}
		if (enemyController.aiPath.enabled && !enemyController.isFakeDead && !enemyController.isHurt && enemyController.isMoveable)
		{
			if (enemyController.aiPath.remainingDistance > 0.3f)
			{
				enemyController.movement.angleAnim = enemyController.attack.AngleEnemy(enemyController.aiPath.desiredVelocity.normalized, enemyController.movement.angleAnim);
				enemyController.network.SetAnimation("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			else if (enemyController.aiPath.remainingDistance < 0.1f)
			{
				enemyController.network.SetAnimation("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			if (!enemyController.movement.timerChangeState.isRunning && enemyController.isMoveable && !enemyController.isHurt)
			{
				enemyController.movement.MoveRandomPath();
				enemyController.SetAISpeed(enemyController.data.GetSpeed());
				enemyController.movement.timerChangeState.StartDuration(Random.Range(enemyController.data.minTimeRandomState, enemyController.data.maxTimeRandomState));
			}
		}
	}
}
