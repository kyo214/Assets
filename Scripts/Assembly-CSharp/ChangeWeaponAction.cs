using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeWeaponAction : MonoBehaviour
{
	[SerializeField]
	private InputActionReference _changeWeapon;

	[SerializeField]
	private PlayerController _playerController;

	[SerializeField]
	private int _IdxCurrentInvSlotWeapon = -1;

	private void OnEnable()
	{
		_changeWeapon.action.started += OnChangeRangeWeaponPerform;
	}

	private void OnDisable()
	{
		_changeWeapon.action.started -= OnChangeRangeWeaponPerform;
	}

	private void Awake()
	{
		_changeWeapon.action.Enable();
	}

	public void OnChangeRangeWeaponPerform(InputAction.CallbackContext value)
	{
		if (!_playerController.network.isLocalPlayer)
		{
			base.enabled = false;
		}
		else if ((bool)_playerController.network.playerPhoton.enableControl && !_playerController.fsmUpperBody.GetBool("isReloading") && !_playerController.fsmUpperBody.GetBool("isShooting"))
		{
			ChangeRangeWeapon();
		}
	}

	private void ChangeRangeWeapon()
	{
		AudioManager.PlaySFX("pickup-heavy-guns");
		PlayerData data = _playerController.data;
		if (_IdxCurrentInvSlotWeapon == -1)
		{
			_IdxCurrentInvSlotWeapon = 1;
		}
		if (_playerController.network.GetIdWeapon1() > 0)
		{
			_playerController.network.GetIdWeapon1();
		}
		int num = _IdxCurrentInvSlotWeapon;
		for (int i = 2; i < data.arrInventory.Count; i++)
		{
			num++;
			if (num >= data.arrInventory.Count)
			{
				num = 2;
			}
			if (!(data.arrInventory[num].ItemType == "Weapon") || !(BGDatabase_Weapon.GetEntityByKeyid(data.arrInventory[num].ID).WeaponType == "Range"))
			{
				continue;
			}
			_IdxCurrentInvSlotWeapon = num;
			_playerController.inventoryManager.FunctionSwapSlot(1, num, isLocal: true);
			_playerController.inventoryManager.WeaponEquip(1, data.arrInventory[1].Ammo);
			_playerController.inventoryManager.txtAmountList[num].text = data.arrInventory[num].Ammo.ToString();
			if (!_playerController.isAiming)
			{
				break;
			}
			data.SetCurrentMoveSpeed(data.GetMoveAimSpeed());
			float num2 = 0.55f;
			if (data.GetMoveAimSpeed() < 1f)
			{
				num2 = 0.45f;
			}
			else if (data.GetMoveAimSpeed() > 1.5f)
			{
				num2 = 0.75f;
			}
			_playerController.SetAnimLowerSpeed(num2);
			_playerController.SetAnimUpperSpeed(num2);
			_playerController.weaponController.accuracy = _playerController.weaponController.maxRangeAccuracy;
			if (_playerController.network.GetAngledirection() == Vector3.zero)
			{
				DOTween.To(() => _playerController.weaponController.accuracy, (float x) =>
				{
					_playerController.weaponController.accuracy = x;
				}, _playerController.weaponController.minRangeAccuracy, _playerController.weaponController.timeAccuracy).SetId("AccuracySubtract").SetEase(Ease.Linear);
			}
			break;
		}
	}
}
