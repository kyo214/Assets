using DG.Tweening;
using UnityEngine;

public class IdlePlayerBehaviour : StateMachineBehaviour
{
	private static readonly int IsShooting = Animator.StringToHash("isShooting");

	private static readonly int IsMelee = Animator.StringToHash("isMelee");

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
		else if ((bool)player.weaponController.meleeSprite && player.weaponController.meleeSprite.material.GetFloat("_Brightness") > 0f)
		{
			player.weaponController.meleeSprite.material.DOKill();
			player.weaponController.meleeSprite.material.SetFloat("_Brightness", 0f);
			player.weaponController.meleeSprite.material.SetColor("_Tint", new Color(0f, 0f, 0f));
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!(player.network.GetHealth() > 0f) || (bool)player.network.playerPhoton.disconnected)
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
				if (player.weaponController.weaponSelect == 1)
				{
					if (player.isAiming)
					{
						player.animUpperChar.Play("Idle" + player.weaponController.weaponStyle + player.angleRot);
					}
					else if (player.isLowHealth)
					{
						player.animUpperChar.Play("IdleWounded" + player.angleRot);
					}
					else
					{
						player.animUpperChar.Play("IdleMelee" + player.angleRot);
					}
				}
				else
				{
					player.animUpperChar.Play("IdleWeaponless" + player.angleRot);
				}
			}
			else if (player.isAiming)
			{
				player.animUpperChar.Play("Idle" + player.weaponController.weaponStyle + player.angleRot);
			}
			else if (player.isLowHealth)
			{
				player.animUpperChar.Play("IdleWounded" + player.angleRot);
			}
			else
			{
				player.animUpperChar.Play("IdleMelee" + player.angleRot);
			}
		}
		else if (!player.fsmUpperBody.GetBool(IsMelee))
		{
			if (player.isLowHealth)
			{
				player.animUpperChar.Play("IdleWounded" + player.angleRot);
			}
			else
			{
				player.animUpperChar.Play("IdleMelee" + player.angleRot);
			}
		}
	}
}
