using UnityEngine;

public class MeleePlayerBehaviour : StateMachineBehaviour
{
	private PlayerController player;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (player == null)
		{
			player = animator.transform.parent.GetComponent<PlayerController>();
		}
		if (!(player.network.GetHealth() > 0f))
		{
			return;
		}
		if (player.isThrowing)
		{
			player.animUpperChar.Play("Throw" + player.angleRot, -1, 0f);
			if (player.network.GetHealth() > 0f)
			{
				player.SetAnimUpperSpeed(0f);
			}
		}
		else
		{
			if (player.isAiming)
			{
				return;
			}
			if (player.weaponController.idWeaponMelee < 0)
			{
				player.animUpperChar.Play("AttackWeaponless" + player.angleRot, -1, 0f);
				return;
			}
			if (player.weaponController.isResetCombo)
			{
				player.weaponController.isResetCombo = false;
				player.weaponController.idxAttackCombo = -1;
			}
			player.weaponController.idxAttackCombo++;
			if (player.weaponController.idxAttackCombo >= player.weaponController.maxAttackCombo)
			{
				player.weaponController.idxAttackCombo = 0;
			}
			if (!player.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("AttackMelee"))
			{
				player.isAttackMeleeSwing = false;
			}
			player.weaponController.timerDelayAttackEnd.StartDuration(0.1f);
			player.animUpperChar.Play("AttackMelee" + player.angleRot + "-" + player.weaponController.idxAttackCombo, -1, 0f);
			player.SetAnimUpperSpeed(1f);
			player.weaponController.timerResetCombo.StartDuration(1f);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		player.SetAnimUpperSpeed(1f);
		if (!player.isLowHealth && player.isSprintDown && player.direction != Vector3.zero && (GlobalSaveData.instance.optionData.sprintModeToggle || (!GlobalSaveData.instance.optionData.sprintModeToggle && player.canSprint)))
		{
			player.StartSprint();
		}
		else
		{
			player.data.SetCurrentMoveSpeed(player.data.GetInitialMoveSpeed());
		}
		player.isThrowing = false;
		player.isAttackMeleeSwing = false;
	}
}
