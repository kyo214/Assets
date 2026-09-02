using UnityEngine;

public class ThrowPlayerBehaviour : StateMachineBehaviour
{
	private PlayerController player;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (player == null)
		{
			player = animator.transform.parent.GetComponent<PlayerController>();
		}
		if (player.network.GetHealth() > 0f)
		{
			player.animUpperChar.Play("Throw" + player.angleRot, -1, 0f);
			player.isThrowing = true;
			player.SetAnimUpperSpeed(0f);
		}
	}
}
