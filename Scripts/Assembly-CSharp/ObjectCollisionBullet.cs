using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DestroyIt;
using UnityEngine;
using UnityEngine.Events;

public class ObjectCollisionBullet : MonoBehaviour
{
	public enum SpawnItemMode
	{
		NO_SPAWN_ITEM = 0,
		SPAWN_ITEM_ID = 1,
		SPAWN_RANDOMIZE_ITEM = 2
	}

	public static string DESTRUCTABLE_OBJECT_TAG = "BreakableObject";

	public string typeCollision = "Brick";

	public EnumCollisionBullet typeCollisionBullet;

	public GameObject activateObject;

	public bool isExplosiveObject;

	public SpawnItemMode SpawnItem;

	public int spawnItemID = -1;

	public Transform spawnPos;

	public float delayDestroy = 1f;

	public string SFXName = "";

	public GameObject parentObject;

	public bool isDisabled;

	public int UID;

	public DestructibleObject destructObject;

	public Destructible destructibleComp;

	public Collider ObjectCollider;

	public bool isShaking;

	public List<ObjectAnimationToPlay> ObjectAnimationAfterDestroy = new List<ObjectAnimationToPlay>();

	public SpriteRenderer ItemMap;

	public bool IsSmallObject;

	public bool IsSpawnAgain;

	public bool IsCollisionDelayRunning;

	public UnityEvent onDestroyEvent;

	private void Awake()
	{
		ObjectCollider = GetComponent<Collider>();
	}

	private void Start()
	{
		if ((bool)destructibleComp && IsSpawnAgain)
		{
			destructibleComp.canBeDestroyed = false;
		}
	}

	public bool GetStatusDestroy()
	{
		return destructibleComp.currentHitPoints <= 0f;
	}

	public void HitDestructibleObject(float damage = 10f, PlayerController playerController = null)
	{
		destructibleComp.ApplyDamage(damage);
		if (!(destructibleComp.currentHitPoints <= 0f))
		{
			return;
		}
		if (activateObject != null && !activateObject.activeSelf)
		{
			activateObject.transform.parent = base.transform.parent;
			activateObject.SetActive(value: true);
			UniTaskUtil.DelayedCall(this, delayDestroy, () =>
			{
				Object.Destroy(activateObject);
			}).Forget();
		}
		if ((bool)destructibleComp && IsSpawnAgain)
		{
			destructibleComp.transform.DOScale(0f, 0f);
			destructibleComp.gameObject.SetActive(value: false);
			UniTaskUtil.DelayedCall(this, 3f, () =>
			{
				if (destructObject != null)
				{
					base.gameObject.layer = 9;
					destructibleComp.transform.DOScale(1f, 0.3f);
					destructObject.colliderObject.enabled = true;
				}
				destructibleComp.currentHitPoints = 1f;
				destructibleComp.gameObject.SetActive(value: true);
				isDisabled = false;
			}).Forget();
		}
		if ((bool)ItemMap)
		{
			ItemMap.enabled = false;
		}
		if (!isDisabled && NetworkGameManager.Instance.isServer)
		{
			int itemID = GetItemID(playerController);
			if (itemID != -1)
			{
				playerController.network.SetSpawnItem(itemID, spawnPos.position);
			}
		}
		isDisabled = true;
		if (ObjectAnimationAfterDestroy.Count > 0)
		{
			for (int num = 0; num < ObjectAnimationAfterDestroy.Count; num++)
			{
				ObjectAnimationAfterDestroy[num].AnimatorObject.SetTrigger(ObjectAnimationAfterDestroy[num].TriggerAnimation);
			}
		}
		if (destructObject != null)
		{
			base.gameObject.layer = 0;
			if (GameManager.Instance.AStarPath != null && !IsSmallObject)
			{
				destructObject.colliderObject.enabled = false;
				GameManager.Instance.AStarPath.UpdateGraphs(destructObject.colliderObject.bounds);
				GameManager.Instance.AStarPath.FlushGraphUpdates();
			}
		}
		onDestroyEvent?.Invoke();
	}

	public int GetItemID(PlayerController playerController)
	{
		int result = -1;
		if (SpawnItem == SpawnItemMode.SPAWN_RANDOMIZE_ITEM)
		{
			int num = Random.Range(0, 100);
			float health = playerController.network.GetHealth();
			int ammo = playerController.data.arrInventory[playerController.weaponController.idxWeaponRange].Ammo;
			if (num < 50 && health <= 60f && GlobalMissionManager.Instance.ModNoHealingItem.CurrentValue == 0f)
			{
				result = 201;
			}
			else if (num < 50 && playerController.weaponController.idWeaponRange > 0 && ammo < playerController.weaponController.GetMagazineSize(equipedWeapon: true) / 4 && !BGDatabase_Weapon.GetEntityByKeyid(playerController.weaponController.idWeaponRange).IsSpecialWeapon && GlobalMissionManager.Instance.ModNoAmmoLoot.CurrentValue == 0f)
			{
				result = BGDatabase_Weapon.GetEntityByKeyid(playerController.weaponController.idBaseWeaponRange).AmmoTypeID;
			}
			else
			{
				num = Random.Range(0, 100);
				if (num < 2)
				{
					result = 21;
				}
				else if (num < 5)
				{
					result = 24;
				}
				else if (num < 20)
				{
					result = 400;
				}
				else if (num < 40)
				{
					result = 203;
				}
				else if (num < 60 && GlobalMissionManager.Instance.ModNoHealingItem.CurrentValue < 1f)
				{
					result = 201;
				}
				else if (num < 80 && playerController.weaponController.idWeaponRange > 0)
				{
					result = BGDatabase_Weapon.GetEntityByKeyid(playerController.weaponController.idBaseWeaponRange).AmmoTypeID;
				}
				else
				{
					int num2 = -1;
					for (int i = 2; i < playerController.data.arrInventory.Count; i++)
					{
						if (playerController.data.arrInventory[i].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(playerController.data.arrInventory[i].ID).WeaponType == "Range" && !BGDatabase_Weapon.GetEntityByKeyid(playerController.data.arrInventory[i].ID).IsSpecialWeapon)
						{
							num2 = BGDatabase_Weapon.GetEntityByKeyid(DataManager.Instance.GetBaseWeapon(playerController.data.arrInventory[i].ID)).AmmoTypeID;
							break;
						}
					}
					result = ((num2 <= 0) ? (-1) : num2);
				}
			}
		}
		else if (SpawnItem == SpawnItemMode.SPAWN_ITEM_ID)
		{
			result = spawnItemID;
		}
		return result;
	}
}
