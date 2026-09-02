using UnityEngine;

public class RevivePlayerBehaviour : StateMachineBehaviour
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
			float num = player.angleRot;
			if (player.angleRot == 0f)
			{
				num = 315f;
			}
			else if (player.angleRot == 90f)
			{
				num = 45f;
			}
			else if (player.angleRot == 180f)
			{
				num = 135f;
			}
			else if (player.angleRot == 270f)
			{
				num = 225f;
			}
			player.SetAnimUpperSpeed(1f);
			player.animLowerChar.Play("LegCrouch" + num);
			player.animUpperChar.Play("InteractCrouch" + num);
		}
	}
}
