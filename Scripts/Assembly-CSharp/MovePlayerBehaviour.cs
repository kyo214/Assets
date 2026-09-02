using UnityEngine;

public class MovePlayerBehaviour : StateMachineBehaviour
{
	private static readonly int IsShooting = Animator.StringToHash("isShooting");

	private PlayerController player;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (player == null)
		{
			animator.transform.parent.TryGetComponent<PlayerController>(out player);
		}
		if (player.isAiming)
		{
			player.isShooting = false;
			player.isAttacking = false;
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!player.enableMoveChar)
		{
			return;
		}
		if (player.isRangeActive)
		{
			if (player.fsmUpperBody.GetBool(IsShooting))
			{
				return;
			}
			if (GlobalOptionsManager.Instance.usingWeaponSelect)
			{
				if (player.weaponController.weaponSelect == 0)
				{
					if (player.animLowerChar.GetCurrentAnimatorStateInfo(1).IsTag("BMove"))
					{
						player.animUpperChar.Play("BMoveWeaponless" + player.angleRot, 0, player.animLowerChar.GetCurrentAnimatorStateInfo(1).normalizedTime);
					}
					else
					{
						player.animUpperChar.Play("MoveWeaponless" + player.angleRot, 0, player.animLowerChar.GetCurrentAnimatorStateInfo(1).normalizedTime);
					}
				}
				else if (player.isAiming)
				{
					player.animUpperChar.Play("Move" + player.weaponController.weaponStyle + player.angleRot, 0, player.animLowerChar.GetCurrentAnimatorStateInfo(1).normalizedTime);
				}
				else if (player.isLowHealth)
				{
					player.animUpperChar.Play("MoveWounded" + player.angleRot, 0, player.animLowerChar.GetCurrentAnimatorStateInfo(1).normalizedTime);
				}
				else
				{
					player.animUpperChar.Play("MoveMelee" + player.angleRot, 0, player.animLowerChar.GetCurrentAnimatorStateInfo(1).normalizedTime);
				}
			}
			else if (player.isAiming)
			{
				player.animUpperChar.Play("Move" + player.weaponController.weaponStyle + player.angleRot, 0, player.animLowerChar.GetCurrentAnimatorStateInfo(1).normalizedTime);
			}
			else if (player.isLowHealth)
			{
				player.animUpperChar.Play("MoveWounded" + player.angleRot, 0, player.animLowerChar.GetCurrentAnimatorStateInfo(1).normalizedTime);
			}
			else
			{
				player.animUpperChar.Play("MoveMelee" + player.angleRot, 0, player.animLowerChar.GetCurrentAnimatorStateInfo(1).normalizedTime);
			}
		}
		else if (player.isLowHealth)
		{
			player.animUpperChar.Play("MoveWounded" + player.angleRot, 0, player.animLowerChar.GetCurrentAnimatorStateInfo(1).normalizedTime);
		}
		else
		{
			player.animUpperChar.Play("MoveMelee" + player.angleRot, 0, player.animLowerChar.GetCurrentAnimatorStateInfo(1).normalizedTime);
		}
	}
}
