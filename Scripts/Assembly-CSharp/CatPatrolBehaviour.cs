using Cysharp.Threading.Tasks;
using UnityEngine;

public class CatPatrolBehaviour : StateMachineBehaviour
{
	[SerializeField]
	private EnemyController enemyController;

	[SerializeField]
	private bool _restChasing;

	[SerializeField]
	private Transform _target;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (enemyController == null)
		{
			enemyController = animator.transform.parent.GetComponent<EnemyController>();
		}
		if (Random.Range(0, 100) <= 35)
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
			return;
		}
		enemyController.attack.SetAITargettoNull();
		enemyController.AIEnable = true;
		_restChasing = true;
		_target = enemyController.aiTarget.transform;
		UniTaskUtil.DelayedCall(enemyController, Random.Range(3, 5), () =>
		{
			if (enemyController.GetCurrentStateHash() == AnimatorHashManager.PatrolHash)
			{
				enemyController.SetEnableAI(value: true);
				enemyController.movement.MoveRandomPath();
				enemyController.SetAISpeed(enemyController.data.GetSpeed());
				enemyController.movement.timerChangeState.StartDuration(Random.Range(enemyController.data.minTimeRandomState, enemyController.data.maxTimeRandomState));
				_restChasing = false;
			}
		}).Forget();
		UniTaskUtil.DelayedCall(enemyController, Random.Range(12, 16), () =>
		{
			if (enemyController.GetCurrentStateHash() == AnimatorHashManager.PatrolHash)
			{
				enemyController.attack.SetAITarget(_target);
				enemyController.attack.StartChasing(playerSighted: true);
			}
		}).Forget();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (NetworkGameManager.Instance.isServer && enemyController.network.GetHealth() > 0f && !enemyController.isDead && !enemyController.isDown && !_restChasing && enemyController.aiPath.enabled && !enemyController.isFakeDead && !enemyController.isHurt && enemyController.isMoveable)
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
