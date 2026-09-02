using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toked.StatusEffect;
using UnityEngine;
using _Modules.Item.Scripts;

public class CurseItemsManager : MonoBehaviour
{
	[SerializeField]
	private List<StatusEffectScriptableObject> _statusEffectScriptableObjectList = new List<StatusEffectScriptableObject>();

	[SerializeField]
	private List<StatusEffectScriptableObject> _statusEffectScriptableObjectListSolo = new List<StatusEffectScriptableObject>();

	[SerializeField]
	private int totalItemCurse;

	[SerializeField]
	private int _minRandomizeItemCurseInTheGame;

	[SerializeField]
	private int _maxRandomizeItemCurseInTheGame;

	[SerializeField]
	private int _additionalMinMaxItemCoop;

	[SerializeField]
	private int _percentageAffectedOnQuestItem;

	public bool IsAffectedOnQuestItem;

	private bool isEventRemoved;

	[SerializeField]
	private bool testMode;

	private void OnEnable()
	{
		GameManager.OnSpawnNewItemFromDrop = (Action<ItemPickable>)Delegate.Combine(GameManager.OnSpawnNewItemFromDrop, new Action<ItemPickable>(SpawnNewItem));
	}

	private void OnDisable()
	{
		if (!isEventRemoved)
		{
			GameManager.OnSpawnNewItemFromDrop = (Action<ItemPickable>)Delegate.Remove(GameManager.OnSpawnNewItemFromDrop, new Action<ItemPickable>(SpawnNewItem));
		}
	}

	private void SpawnNewItem(ItemPickable item)
	{
		if (IsAffectedOnQuestItem && item.itemID == MissionManager.Instance.KeyItemToActivateCar && _statusEffectScriptableObjectList.Count > 0)
		{
			UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
			item.AddItemStatusEffect(_statusEffectScriptableObjectList[UnityEngine.Random.Range(0, _statusEffectScriptableObjectList.Count)]);
			item.IsCursedItem = true;
			IsAffectedOnQuestItem = false;
			GameManager.OnSpawnNewItemFromDrop = (Action<ItemPickable>)Delegate.Remove(GameManager.OnSpawnNewItemFromDrop, new Action<ItemPickable>(SpawnNewItem));
			isEventRemoved = true;
			UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
		}
	}

	public IEnumerator Start()
	{
		if (NetworkGameManager.Instance.arrPlayerController.Count == 1)
		{
			_statusEffectScriptableObjectList.Clear();
			_statusEffectScriptableObjectList = _statusEffectScriptableObjectListSolo;
		}
		yield return new WaitForSeconds(6f);
		if (GlobalMissionManager.Instance.ModCursedItem.CurrentValue >= 1f)
		{
			if (!(LobbyManager.Instance == null) && !testMode)
			{
				yield break;
			}
			while (GameManagerPhoton.Instance == null)
			{
				yield return new WaitForSeconds(0.1f);
			}
			UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
			List<ItemPickable> list = GameManager.Instance.arrItemPickable.ToList();
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (!list[num].isActiveAndEnabled || list[num].itemType != "Item" || list[num].IsSpawnedFromObject || BGDatabase_Item.GetEntityByKeyid(list[num].itemID).IsNotKeyItem)
				{
					list.RemoveAt(num);
				}
			}
			if ((UnityEngine.Random.Range(0, 100) < _percentageAffectedOnQuestItem && NetworkGameManager.Instance.arrPlayerController.Count > 1) || (list.Count == 1 && list[0].itemID == MissionManager.Instance.KeyItemToActivateCar))
			{
				IsAffectedOnQuestItem = true;
			}
			totalItemCurse = UnityEngine.Random.Range(_minRandomizeItemCurseInTheGame, _maxRandomizeItemCurseInTheGame + 1);
			if (IsAffectedOnQuestItem)
			{
				if (_statusEffectScriptableObjectList.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].itemID == MissionManager.Instance.KeyItemToActivateCar)
						{
							list[i].AddItemStatusEffect(_statusEffectScriptableObjectList[UnityEngine.Random.Range(0, _statusEffectScriptableObjectList.Count)]);
							list[i].IsCursedItem = true;
							IsAffectedOnQuestItem = false;
							list.RemoveAt(i);
							totalItemCurse--;
							break;
						}
					}
				}
			}
			else if (_statusEffectScriptableObjectList.Count > 0)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].itemID == MissionManager.Instance.KeyItemToActivateCar)
					{
						list.RemoveAt(j);
						break;
					}
				}
			}
			if (NetworkGameManager.Instance.arrPlayerController.Count >= 2)
			{
				totalItemCurse += _additionalMinMaxItemCoop;
			}
			for (int k = 0; k < totalItemCurse; k++)
			{
				if (_statusEffectScriptableObjectList.Count > 0 && list.Count > 0)
				{
					int index = UnityEngine.Random.Range(0, list.Count);
					int index2 = UnityEngine.Random.Range(0, _statusEffectScriptableObjectList.Count);
					list[index].AddItemStatusEffect(_statusEffectScriptableObjectList[index2]);
					list[index].IsCursedItem = true;
					list.RemoveAt(index);
					_statusEffectScriptableObjectList.RemoveAt(index2);
				}
			}
			UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
		}
		else
		{
			base.enabled = false;
		}
	}
}
