using Toked;
using UnityEngine;

public class BigHollowMotherAttackBehaviour : MonoBehaviour
{
	private EnemyController enemyController;

	private EnemyAttack enemyAttack;

	private int ctrSpecial2;

	private int ctrSpecial3;

	private void Start()
	{
		enemyController = base.transform.parent.GetComponent<EnemyController>();
		enemyAttack = base.transform.parent.GetComponent<EnemyAttack>();
		ctrSpecial2 = Random.Range(2, 4);
		ctrSpecial3 = Random.Range(2, 4);
	}

	public void Attack()
	{
		int num = 0;
		if (ctrSpecial3 <= 0)
		{
			if (NetworkGameManager.Instance.arrPlayerController.Count > 2)
			{
				ctrSpecial3 = Random.Range(2, 5);
			}
			else
			{
				ctrSpecial3 = Random.Range(3, 5);
			}
			num = 3;
		}
		else if (ctrSpecial2 <= 0)
		{
			ctrSpecial2 = Random.Range(2, 4);
			num = 2;
		}
		bool flag = false;
		if (num > 0)
		{
			if (enemyAttack.targetChasing != null)
			{
				enemyController.movement.angleAnim = enemyAttack.AngleEnemy((enemyAttack.targetChasing.transform.position - base.transform.position).normalized, enemyController.movement.angleAnim);
			}
			if (num == 2)
			{
				enemyController.network.SetAnimation("Special2" + enemyController.movement.angleAnim);
			}
			else
			{
				enemyController.network.SetAnimation("Special3" + enemyController.movement.angleAnim);
			}
			enemyAttack.timerTriggerAttack.StartDuration(2f);
			flag = true;
		}
		else
		{
			if (!enemyController.isAttacking)
			{
				enemyAttack.SetAttackTarget();
			}
			if (enemyAttack.targetChasing != null)
			{
				enemyController.movement.angleAnim = enemyAttack.AngleEnemy((enemyAttack.targetChasing.transform.position - base.transform.position).normalized, enemyController.movement.angleAnim);
			}
			enemyController.network.SetAnimation("Attack" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			enemyAttack.timerTriggerAttack.StartDuration(1f);
		}
		enemyController.isAttacking = true;
		if ((((bool)enemyController.barricadeCollider && enemyController.barricadeCollider.barricade.Hp > 0) || enemyController.network.IsSpecialAttacking() || enemyController.aiPath.reachedDestination || (!enemyController.isElite && MathFunc.Distance(enemyController.middlePos.position, enemyAttack.targetChasing.position) < 1.5f) || (enemyController.isElite && MathFunc.Distance(enemyController.middlePos.position, enemyAttack.targetChasing.position) < 3f)) | flag)
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
		ctrSpecial2--;
		ctrSpecial3--;
	}
}
