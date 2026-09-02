using Cysharp.Threading.Tasks;
using Toked;
using UnityEngine;

public class CatIdleBehaviour : StateMachineBehaviour
{
	private static readonly int IsWakeUp = Animator.StringToHash("IsWakeUp");

	private static readonly int IsSleep = Animator.StringToHash("IsSleep");

	private static readonly int IsResting = Animator.StringToHash("IsResting");

	[SerializeField]
	private EnemyController enemyController;

	[SerializeField]
	private bool _isChasing;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		int num = Random.Range(0, 100);
		if (enemyController == null)
		{
			enemyController = animator.transform.parent.GetComponent<EnemyController>();
		}
		_isChasing = false;
		enemyController.animator.SetBool(IsWakeUp, value: false);
		enemyController.animator.SetBool(IsResting, value: true);
		if (num < 30)
		{
			enemyController.animator.SetBool(IsSleep, value: true);
			AudioManager.PlaySFXTransform("cat-normal-idle", enemyController.transform, isLocalPlayerTrigger: false);
			UniTaskUtil.DelayedCall(enemyController, Random.Range(10f, 20f), () =>
			{
				if (enemyController.GetCurrentStateHash() == AnimatorHashManager.IdleHash)
				{
					enemyController.animator.SetBool(IsResting, value: false);
				}
				AudioManager.StopSFXTransform(enemyController.transform);
			}).Forget();
		}
		else
		{
			enemyController.animator.SetBool(IsSleep, value: false);
			UniTaskUtil.DelayedCall(enemyController, Random.Range(5f, 10f), () =>
			{
				if (enemyController.GetCurrentStateHash() == AnimatorHashManager.IdleHash)
				{
					enemyController.animator.SetBool(IsResting, value: false);
				}
			}).Forget();
		}
		enemyController.network.networkPhoton.RPCPlayAnimation(Animator.StringToHash("ToSit" + enemyController.movement.angleAnim));
		enemyController.attack.timerRandomIdleChasing.StopDuration();
		enemyController.attack.fov.enabled = true;
		enemyController.attack.fov.SetDisable(value: false);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!NetworkGameManager.Instance.isServer || !(enemyController.network.GetHealth() > 0f) || enemyController.isDead || enemyController.isDown || enemyController.animator.GetBool(IsResting))
		{
			return;
		}
		EnemyAttack enemyAttack = enemyController.attack;
		if (_isChasing || enemyAttack.fov.visibleTargets.Count <= 0)
		{
			return;
		}
		enemyController.animator.SetBool(IsWakeUp, value: true);
		enemyController.animator.SetBool(IsSleep, value: false);
		_isChasing = true;
		UniTaskUtil.DelayedCall(enemyController, 2.3f, () =>
		{
			if (enemyAttack.fov.visibleTargets.Count > 0)
			{
				enemyController.movement.angleAnim = enemyAttack.AngleEnemy((enemyAttack.fov.visibleTargets[0].position - enemyAttack.transform.position).normalized, -1);
			}
			enemyController.network.SetAnimation("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
		}).Forget();
		UniTaskUtil.DelayedCall(enemyController, 2.8f, () =>
		{
			enemyAttack.StartChasing(playerSighted: true);
		}).Forget();
	}
}
