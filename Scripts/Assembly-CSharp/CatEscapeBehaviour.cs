using Pathfinding;
using Toked;
using UnityEngine;

public class CatEscapeBehaviour : StateMachineBehaviour
{
	[SerializeField]
	private EnemyController enemyController;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (enemyController == null)
		{
			enemyController = animator.transform.parent.GetComponent<EnemyController>();
		}
		enemyController.attack.fov.enabled = false;
		enemyController.SetEnableAI(value: false);
		enemyController.AIEnable = true;
		enemyController.SetEnableAI(value: true);
		enemyController.attack.SetAITargettoNull();
		Vector3 vector = MathFunc.FloatToPosition(animator.GetFloat("DangerPos"));
		FleePath p = FleePath.Construct(avoid: new Vector3(vector.x, enemyController.transform.position.y, vector.z), start: enemyController.transform.position, searchLength: 23000);
		enemyController.aiSeeker.CancelCurrentPathRequest();
		enemyController.aiSeeker.StartPath(p);
		enemyController.SetAISpeed(enemyController.data.GetSpeed() * 3f);
		AudioManager.StopSFXTransform(enemyController.transform);
		AudioManager.PlaySFXTransform("cat-normal-aggro", enemyController.transform, isLocalPlayerTrigger: false);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (NetworkGameManager.Instance.isServer && enemyController.network.GetHealth() > 0f && !enemyController.isDead && !enemyController.isDown)
		{
			if (enemyController.aiPath.velocity.magnitude > 1f)
			{
				enemyController.movement.angleAnim = enemyController.attack.AngleEnemy(enemyController.aiPath.desiredVelocity.normalized, enemyController.movement.angleAnim);
				enemyController.network.SetAnimation("MoveAggro" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			if (enemyController.aiPath.reachedDestination || MathFunc.Distance(enemyController.transform.position, enemyController.aiPath.destination) < 1f)
			{
				enemyController.attack.fov.enabled = false;
				enemyController.AIEnable = false;
				enemyController.SetEnableAI(value: false);
				enemyController.SetAISpeed(0f);
				enemyController.SetState(EnemyState.Idle);
				enemyController.movement.angleAnim = enemyController.attack.AngleEnemy(enemyController.aiPath.desiredVelocity.normalized, enemyController.movement.angleAnim);
			}
		}
	}
}
