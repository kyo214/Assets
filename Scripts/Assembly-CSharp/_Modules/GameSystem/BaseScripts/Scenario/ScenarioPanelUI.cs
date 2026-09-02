using System.Collections.Generic;
using UnityEngine;
using _Modules.UITitle.CreateRoom;

namespace _Modules.GameSystem.BaseScripts.Scenario;

public class ScenarioPanelUI : CreateGameSettingPanel<ScenarioScriptableObject>
{
	[SerializeField]
	private ScenarioScriptableObjectLibrary _scenarioScriptableObjectLibrary;

	protected override void InitDataList()
	{
		_listData.Clear();
		if ((object)_scenarioScriptableObjectLibrary == null)
		{
			_scenarioScriptableObjectLibrary = DataManager.Instance.Get<ScenarioScriptableObjectLibrary>();
		}
		foreach (ScenarioScriptableObject data in _scenarioScriptableObjectLibrary.DataList)
		{
			_listData.Add(data);
		}
		_index = 0;
	}

	protected override string GetTermValue()
	{
		return _listData[_index].ScenarioNameLocalization ?? "";
	}

	public override void SetDataWhenCreateGame(bool isLoad)
	{
		GameModes.Instance.SetScenarioId(_listData[_index].ScenarioId);
	}

	public override void OnValueChangedAction(int index)
	{
		base.OnValueChangedAction(index);
		base.IsCurrentValueValid = _listData[index].CheckDataValid();
		OnChangeValueEvent?.Invoke(base.IsCurrentValueValid);
	}

	protected override List<bool> GetDisableData()
	{
		List<bool> list = new List<bool>();
		foreach (ScenarioScriptableObject listDatum in _listData)
		{
			list.Add(listDatum.GetDisable());
		}
		return list;
	}

	protected override List<bool> GetLockData()
	{
		List<bool> list = new List<bool>();
		foreach (ScenarioScriptableObject listDatum in _listData)
		{
			list.Add(!listDatum.CheckRequirementUnlock());
		}
		return list;
	}
}
