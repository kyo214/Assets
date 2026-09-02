using UnityEngine;

public class EnemyDeadBehaviour : StateMachineBehaviour
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
}
