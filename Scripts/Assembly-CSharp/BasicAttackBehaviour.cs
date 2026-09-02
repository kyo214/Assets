using Toked;
using UnityEngine;

public class BasicAttackBehaviour : MonoBehaviour
{
	private EnemyController enemyController;

	private EnemyAttack enemyAttack;

	private void Start()
	{
		enemyController = base.transform.parent.GetComponent<EnemyController>();
		enemyAttack = base.transform.parent.GetComponent<EnemyAttack>();
	}

	public void Attack()
	{
		enemyAttack.SetAttackTarget();
		enemyController.isAttacking = true;
		if (enemyAttack.targetChasing != null)
		{
			enemyController.movement.angleAnim = enemyAttack.AngleEnemy((enemyAttack.targetChasing.transform.position - base.transform.position).normalized, enemyController.movement.angleAnim);
		}
		enemyController.network.SetAnimation("Attack" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
		if (((bool)enemyController.barricadeCollider && enemyController.barricadeCollider.barricade.Hp > 0) || enemyController.network.IsSpecialAttacking() || enemyController.aiPath.reachedDestination || (!enemyController.isElite && MathFunc.Distance(enemyController.middlePos.position, enemyAttack.targetChasing.position) < 1.5f) || (enemyController.isElite && MathFunc.Distance(enemyController.middlePos.position, enemyAttack.targetChasing.position) < 2f))
		{
			enemyController.SetAISpeed(0f);
		}
		else
		{
			enemyController.SetAISpeed(enemyController.data.GetAttackMoveSpeed());
		}
		if (enemyController.isAlwaysChasing)
		{
			enemyAttack.timerDelayAggro1.StartDuration(1f);
		}
		enemyAttack.timerTriggerAttack.StartDuration(1f);
	}
}
