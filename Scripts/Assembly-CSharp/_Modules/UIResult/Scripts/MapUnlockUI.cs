using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Modules.UIResult.Scripts;

public class MapUnlockUI : MonoBehaviour
{
	[SerializeField]
	private MapUnlockItemUI _mapUnlockItemUIPrefab;

	[SerializeField]
	private ScrollRect _scrollRect;

	private List<SO_MissionMap> _checkMaps = new List<SO_MissionMap>();

	public void Show()
	{
		_scrollRect.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		_scrollRect.gameObject.SetActive(value: false);
	}

	public bool InitUI()
	{
		SO_MissionMap currentMap = GetCurrentMap();
		if (!GetWinCondition() || currentMap == null)
		{
			return false;
		}
		bool flag = false;
		foreach (SO_MissionMap item in currentMap.ListPossibleMapToUnlock)
		{
			if (!(item == null))
			{
				Object.Instantiate(_mapUnlockItemUIPrefab, _scrollRect.content).Init(item);
				if (!flag)
				{
					flag = true;
				}
			}
		}
		return flag;
	}

	private bool GetWinCondition()
	{
		return UIResultManager.Instance.WinCondition;
	}

	private SO_MissionMap GetCurrentMap()
	{
		return UIResultManager.Instance._resultMission;
	}

	private bool CheckRequiredMapToUnlock(List<SO_MissionMap> maps)
	{
		SO_MissionMap currentMap = GetCurrentMap();
		foreach (SO_MissionMap map in maps)
		{
			bool flag = GameManagerPhoton.Instance.ArrMissionCleared.Get(map.MissionID - 1);
			if (map != currentMap && !flag)
			{
				return false;
			}
		}
		return true;
	}

	private void CheckRequiredMapToUnlock()
	{
		Debug.Log($"result {CheckRequiredMapToUnlock(_checkMaps)}");
	}
}
