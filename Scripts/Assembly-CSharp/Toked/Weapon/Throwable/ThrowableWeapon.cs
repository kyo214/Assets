using DG.Tweening;
using UnityEngine;

namespace Toked.Weapon.Throwable;

public class ThrowableWeapon : Weapon, IThrowable
{
	[SerializeField]
	private MeshFilter _objectThrowablePrefab;

	[SerializeField]
	private ObjectThrowable.ThrowableType _throwableType;

	[SerializeField]
	private bool _showCursor = true;

	private PlayerNetwork _playerNetwork;

	private PlayerData _playerData;

	private InventoryManager _inventoryManager;

	public bool ShowCursor => _showCursor;

	public override void Attack(PlayerController playerController)
	{
	}

	public override void Damage(PlayerController playerController)
	{
	}

	public virtual void Throw(PlayerController playerController, Vector3 targetPosition)
	{
		_playerNetwork = playerController.network;
		_playerData = playerController.data;
		_inventoryManager = playerController.inventoryManager;
		if (_playerNetwork.isLocalPlayer)
		{
			InventoryObject inventoryObject = _playerData.FindInventory(_weaponData.WeaponId);
			if (inventoryObject != null && inventoryObject.Amount > 0)
			{
				inventoryObject.Amount--;
				int num = _playerData.FindTotalInventory(_weaponData.WeaponId);
				if (inventoryObject.Amount <= 0)
				{
					_playerData.RemoveInventory(inventoryObject.IdxInventory);
				}
				if (num < 1)
				{
					bool flag = false;
					for (int i = 0; i < _playerData.arrInventory.Count; i++)
					{
						if (_playerData.arrInventory[i].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(_playerData.arrInventory[i].ID).WeaponType == "Throw")
						{
							flag = true;
							_playerData.idThrowable = _playerData.arrInventory[i].ID;
							UIGameManager.Instance.SetThrowableShortcutSprite(DataManager.Instance.GetItemSprite(_playerData.idThrowable.ToString()));
							_inventoryManager.txtAmountThrowableItem.text = _playerData.FindTotalInventory(_playerData.idThrowable).ToString();
							playerController.canGrenade = true;
							break;
						}
					}
					if (!flag)
					{
						UIGameManager.Instance.HideThrowableShortcutSprite();
						_playerData.idThrowable = -1;
						_inventoryManager.txtAmountThrowableItem.text = "";
					}
				}
				else
				{
					_inventoryManager.txtAmountThrowableItem.text = num.ToString();
				}
				_inventoryManager.txtAmountList[inventoryObject.IdxInventory].text = inventoryObject.Amount.ToString();
			}
		}
		playerController.canGrenade = false;
		playerController.isThrowing = false;
		playerController.animUpperChar.Play("Throw" + playerController.angleRot, -1, 0f);
		playerController.SetAnimUpperSpeed(1f);
		ObjectThrowable objectThrow = ThrowableSpawner.Instance.Get(_throwableType);
		OnThrow(playerController, objectThrow, targetPosition);
	}

	protected virtual void OnThrow(PlayerController playerController, ObjectThrowable objectThrow, Vector3 targetPosition)
	{
		objectThrow.rigidBody.DOKill();
		Vector3 position = playerController.weaponPos.position;
		objectThrow.transform.position = position;
		objectThrow.rigidBody.DOMove(position, 0f);
	}

	protected virtual void OnThrew(PlayerController playerController, ObjectThrowable objectThrow, Vector3 targetPosition)
	{
	}

	protected virtual Vector3 CalculateItemVelocity(Vector3 destination, Vector3 currentPosition)
	{
		return Vector3.zero;
	}
}
