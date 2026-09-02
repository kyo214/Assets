using UnityEngine;

public class ReloadPlayerBehaviour : StateMachineBehaviour
{
	private PlayerController player;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (player == null)
		{
			player = animator.transform.parent.GetComponent<PlayerController>();
		}
		player.animUpperChar.Play("Reload" + player.weaponController.weaponStyle + player.angleRot, -1, 0f);
		if (player.weaponController.rangeWeaponType == RangeWeaponType.Shotgun)
		{
			player.SetAnimUpperSpeed(1.2f * player.PlayerMultiplyStatsData.GetMultiplyAnimSpeedReload());
		}
		else if (player.weaponController.rangeWeaponType == RangeWeaponType.SMG)
		{
			player.SetAnimUpperSpeed(0.75f * player.PlayerMultiplyStatsData.GetMultiplyAnimSpeedReload());
		}
		else if (player.weaponController.rangeWeaponType == RangeWeaponType.Crossbow)
		{
			player.SetAnimUpperSpeed(0.75f * player.PlayerMultiplyStatsData.GetMultiplyAnimSpeedReload());
		}
		else if (player.weaponController.rangeWeaponType == RangeWeaponType.Pistol)
		{
			player.SetAnimUpperSpeed(0.65f * player.PlayerMultiplyStatsData.GetMultiplyAnimSpeedReload());
		}
		else
		{
			player.SetAnimUpperSpeed(player.PlayerMultiplyStatsData.GetMultiplyAnimSpeedReload());
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		player.SetAnimUpperSpeed(1f);
	}
}
