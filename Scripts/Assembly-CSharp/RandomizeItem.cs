using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

public class RandomizeItem : MonoBehaviour
{
	public WeaponTypeEnum WeaponType;

	public List<ItemList> ItemLists = new List<ItemList>();

	private IEnumerator Start()
	{
		GameManager.Instance.ListRandomizeItem.Add(this);
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		if (!GlobalOptionsManager.Instance || ItemLists.Count <= 1 || WeaponType != WeaponTypeEnum.NONE)
		{
			yield break;
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		int num = UnityEngine.Random.Range(0, ItemLists.Count);
		for (int i = 0; i < ItemLists.Count; i++)
		{
			if (i == num)
			{
				continue;
			}
			foreach (GameObject gameObject in ItemLists[i].gameObjects)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		if ((bool)GameManager.Instance)
		{
			GameManager.Instance.arrInitPosEnemy.RemoveNulls();
			GameManager.Instance.arrItemPickable.Sort((ItemPickable p1, ItemPickable p2) => p1.uniqueID.CompareTo(p2.uniqueID));
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}
}
