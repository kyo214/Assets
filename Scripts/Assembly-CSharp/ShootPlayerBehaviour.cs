using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShootPlayerBehaviour : StateMachineBehaviour
{
	private PlayerController player;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (player == null)
		{
			player = animator.transform.parent.GetComponent<PlayerController>();
		}
		if (((!player.isShooting && player.network.isLocalPlayer) || !player.network.isLocalPlayer) && player.network.GetHealth() > 0f)
		{
			player.animUpperChar.Play("Shoot" + player.weaponController.weaponStyle + player.angleRot, -1, 0f);
			player.weaponController.Shoot().Forget();
			player.isShooting = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		player.isShooting = false;
		player.isAttacking = false;
	}
}
